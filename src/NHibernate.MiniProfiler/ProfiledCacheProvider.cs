using System;
using System.Collections.Generic;

using NHibernate.Cache;

namespace NHibernate.MiniProfiler
{
    /// <summary>
    /// The profiled cache provider.
    /// </summary>
    /// <typeparam name="TCacheProvider">The cache provider to profile.</typeparam>
    public sealed class ProfiledCacheProvider<TCacheProvider> : ICacheProvider where TCacheProvider : ICacheProvider, new()
    {
        private readonly ICacheProvider profiledCacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfiledCacheProvider{T}"/> class with the default options.
        /// </summary>
        public ProfiledCacheProvider() => profiledCacheProvider = new TCacheProvider();

        /// <summary>Configure the cache</summary>
        /// <param name="regionName">the name of the cache region</param>
        /// <param name="properties">configuration settings</param>
        public ICache BuildCache(string regionName, IDictionary<string, string> properties) => new ProfiledCache(profiledCacheProvider.BuildCache(regionName, properties), CacheProfilingOptions.FromProperties(properties));

        /// <summary>Generate a timestamp</summary>
        public long NextTimestamp() => profiledCacheProvider.NextTimestamp();

        /// <summary>
        /// Callback to perform any necessary initialization of the underlying cache implementation
        /// during ISessionFactory construction.
        /// </summary>
        /// <param name="properties">current configuration settings</param>
        public void Start(IDictionary<string, string> properties) => profiledCacheProvider.Start(properties);

        /// <summary>
        /// Callback to perform any necessary cleanup of the underlying cache implementation
        /// during <see cref="M:NHibernate.ISessionFactory.Close" />.
        /// </summary>
        public void Stop() => profiledCacheProvider.Stop();
    }
}