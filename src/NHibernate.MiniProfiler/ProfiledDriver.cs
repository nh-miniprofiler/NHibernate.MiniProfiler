using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using NHibernate.AdoNet;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

using StackExchange.Profiling.Data;

namespace NHibernate.MiniProfiler
{
    /// <summary>
    /// Profiling decorator for a driver.
    /// </summary>
    /// <typeparam name="TDriver">The driver to profile.</typeparam>
    /// <remarks>
    /// This class implements <see cref="IEmbeddedBatcherFactoryProvider"/> although the driver under profiling might not.
    /// If <typeparamref name="TDriver"/> implements <see cref="IEmbeddedBatcherFactoryProvider"/>, <see cref="BatcherFactoryClass"/>
    /// will return the value provided by <typeparamref name="TDriver"/> and null otherwise. When creating an instance of <see cref="IBatcherFactory"/>
    /// NHibernate will check if BatcherFactoryClass is null, so this won't cause any exceptions.
    /// </remarks>
    /// <example>
    /// <code>
    /// var cfg = new Configuration().DataBaseIntegration(c => { c.ProfiledDriver&lt;MySqlConnectorDriver&gt;(); });
    /// </code>
    /// or
    /// <code>
    /// var cfg = new Configuration().DataBaseIntegration(c => { c.Driver&lt;ProfiledDriver&lt;MySqlConnectorDriver&gt;&gt;(); });
    /// </code>
    /// </example>
    public sealed class ProfiledDriver<TDriver> : DriverBase, IEmbeddedBatcherFactoryProvider where TDriver : DriverBase, new()
    {
        private readonly TDriver driver;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfiledDriver{TDriver}"/> class.
        /// </summary>
        public ProfiledDriver()
        {
            driver = new TDriver();
            BatcherFactoryClass = driver is IEmbeddedBatcherFactoryProvider embeddedBatcherFactoryProvider ? embeddedBatcherFactoryProvider.BatcherFactoryClass : null;
        }

        public System.Type BatcherFactoryClass { get; }

        public override bool UseNamedPrefixInSql => driver.UseNamedPrefixInSql;

        public override bool UseNamedPrefixInParameter => driver.UseNamedPrefixInParameter;

        public override string NamedPrefix => driver.NamedPrefix;

        public override bool SupportsMultipleOpenReaders => driver.SupportsMultipleOpenReaders;

        public override bool SupportsMultipleQueries => driver.SupportsMultipleQueries;

        public override bool RequiresTimeSpanForTime => driver.RequiresTimeSpanForTime;

        public override bool SupportsSystemTransactions => driver.SupportsSystemTransactions;

        public override bool SupportsNullEnlistment => driver.SupportsNullEnlistment;

        public override bool SupportsEnlistmentWhenAutoEnlistmentIsDisabled => driver.SupportsEnlistmentWhenAutoEnlistmentIsDisabled;

        public override bool HasDelayedDistributedTransactionCompletion => driver.HasDelayedDistributedTransactionCompletion;

        public override DateTime MinDate => driver.MinDate;

        public override int CommandTimeout => driver.CommandTimeout;

        public override void Configure(IDictionary<string, string> settings) => driver.Configure(settings);

        public override DbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
        {
            var command = driver.GenerateCommand(type, sqlString, parameterTypes);

            var profiler = StackExchange.Profiling.MiniProfiler.Current;
            return profiler != null ? new ProfiledDbCommand(command, command.Connection, profiler) : command;
        }

        public override DbConnection CreateConnection()
        {
            var connection = driver.CreateConnection();

            var profiler = StackExchange.Profiling.MiniProfiler.Current;
            return profiler != null ? new ProfiledDbConnection(connection, profiler) : connection;
        }

        public override DbCommand CreateCommand()
        {
            var command = driver.CreateCommand();

            var profiler = StackExchange.Profiling.MiniProfiler.Current;
            return profiler != null ? new ProfiledDbCommand(command, command.Connection, profiler) : command;
        }

        public override void AdjustCommand(DbCommand command) => driver.AdjustCommand(command);

        public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session) => driver.GetResultSetsCommand(session);

        public override void ExpandQueryParameters(DbCommand cmd, SqlString sqlString, SqlType[] parameterTypes) => driver.ExpandQueryParameters(cmd, sqlString, parameterTypes);

        // Called from AbstractBatcher
        // ReSharper disable once UnusedMember.Global
        public new void PrepareCommand(DbCommand command) => driver.PrepareCommand(command);

        // Called from OutputParamReturningDelegate
        // ReSharper disable once UnusedMember.Global
        public new DbParameter GenerateParameter(DbCommand command, string name, SqlType sqlType) => driver.GenerateParameter(command, name, sqlType);

        // Called from Loader
        // ReSharper disable once UnusedMember.Global
        public new void RemoveUnusedCommandParameters(DbCommand cmd, SqlString sqlString) => driver.RemoveUnusedCommandParameters(cmd, sqlString);

        public override DbTransaction BeginTransaction(IsolationLevel isolationLevel, DbConnection connection) => driver.BeginTransaction(isolationLevel, connection);
    }
}
