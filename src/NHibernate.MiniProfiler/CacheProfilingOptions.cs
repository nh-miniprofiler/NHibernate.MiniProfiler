using System;
using System.Collections.Generic;

namespace NHibernate.MiniProfiler
{
    public sealed class CacheProfilingOptions
    {
        private static readonly CacheProfilingOptions Default = new CacheProfilingOptions();

        /// <summary>
        /// Get or sets a value indicating whether to include the stack trace in the profiling results.
        /// Default is <b>false</b>.
        /// </summary>
        /// <remarks>
        /// This setting will not have any effect, if the MiniProfiler setting <c>ExcludeStackTraceSnippetFromCustomTimings</c> is set to <b>true</b>.
        /// </remarks>
        public bool IncludeStackTraceSnippet { get; set; }

        /// <summary>
        /// Gets or sets the category name to use in the MiniProfiler results view. Default is <b>NHibernate Cache</b>.
        /// </summary>
        public string CategoryName { get; set; } = "NHibernate Cache";

        /// <summary>
        /// Gets or sets a value indicating whether to include the name of the cache region that is being profiled in the MiniProfiler results view.
        /// Default is <b>false</b>.
        /// </summary>
        public bool IncludeRegionInCategoryName { get; set; }

        internal static CacheProfilingOptions FromProperties(IDictionary<string, string> props)
        {
            if (props == null)
                return Default;

            var options = new CacheProfilingOptions();
            if (props.TryGetValue(ConfigKeys.IncludeStackTraceSnippet, out string includestacktracesnippet))
                options.IncludeStackTraceSnippet = Convert.ToBoolean(includestacktracesnippet);
            if (props.TryGetValue(ConfigKeys.IncludeRegionInCategoryName, out string includeregionincategoryname))
                options.IncludeRegionInCategoryName = Convert.ToBoolean(includeregionincategoryname);
            if (props.TryGetValue(ConfigKeys.CategoryName, out string categoryname))
                options.CategoryName = categoryname;

            return options;
        }

        public class ConfigKeys
        {
            public const string IncludeStackTraceSnippet = "cache.profiling.includestacktracesnippet";
            public const string CategoryName = "cache.profiling.categoryname";
            public const string IncludeRegionInCategoryName = "cache.profiling.includeregionincategoryname";
        }
    }
}