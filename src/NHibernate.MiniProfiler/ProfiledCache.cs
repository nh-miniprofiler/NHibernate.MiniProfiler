using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using NHibernate.Cache;

using StackExchange.Profiling;

namespace NHibernate.MiniProfiler
{
    /// <summary>
    /// Profiling decorator for a cache.
    /// </summary>
    public sealed class ProfiledCache : ICache
    {
        private readonly string category;
        private readonly ICache profiledCache;
        private readonly CacheProfilingOptions profilingOptions;

        public int Timeout => profiledCache.Timeout;

        public string RegionName => profiledCache.RegionName;

        public ProfiledCache(ICache profiledCache, CacheProfilingOptions profilingOptions)
        {
            this.profiledCache = profiledCache;
            this.profilingOptions = profilingOptions;
            this.category = profilingOptions.IncludeRegionInCategoryName ? $"{profilingOptions.CategoryName} ({profiledCache.RegionName})" : profilingOptions.CategoryName;
        }

        public Task<object> GetAsync(object key, CancellationToken cancellationToken)
        {
            using (TimeWithKey(key))
                return profiledCache.GetAsync(key, cancellationToken);
        }

        public Task PutAsync(object key, object value, CancellationToken cancellationToken)
        {
            using (TimeWithKey(key))
                return profiledCache.PutAsync(key, value, cancellationToken);
        }

        public Task RemoveAsync(object key, CancellationToken cancellationToken)
        {
            using (TimeWithKey(key))
                return profiledCache.RemoveAsync(key, cancellationToken);
        }

        public Task ClearAsync(CancellationToken cancellationToken)
        {
            using (Time())
                return profiledCache.ClearAsync(cancellationToken);
        }

        public Task LockAsync(object key, CancellationToken cancellationToken)
        {
            using (TimeWithKey(key))
                return profiledCache.LockAsync(key, cancellationToken);
        }

        public Task UnlockAsync(object key, CancellationToken cancellationToken)
        {
            using (TimeWithKey(key))
                return profiledCache.UnlockAsync(key, cancellationToken);
        }

        public object Get(object key)
        {
            using (TimeWithKey(key))
                return profiledCache.Get(key);
        }

        public void Put(object key, object value)
        {
            using (TimeWithKey(key))
                profiledCache.Put(key, value);
        }

        public void Remove(object key)
        {
            using (TimeWithKey(key))
                profiledCache.Remove(key);
        }

        public void Clear()
        {
            using (Time())
                profiledCache.Clear();
        }

        public void Destroy()
        {
            using (Time())
                profiledCache.Destroy();
        }

        public void Lock(object key)
        {
            using (TimeWithKey(key))
                profiledCache.Lock(key);
        }

        public void Unlock(object key)
        {
            using (TimeWithKey(key))
                profiledCache.Unlock(key);
        }

        //Only include calls to NextTimestamp in the profiling result, if it takes more than one millisecond.
        private const int NextTimestampMinSaveMs = 1;

        public long NextTimestamp()
        {
            var profiler = StackExchange.Profiling.MiniProfiler.Current;
            IDisposable timing = null;
            if (profiler != null)
                timing = profiler.CustomTimingIf(category, String.Empty, NextTimestampMinSaveMs, $"{nameof(ICache.NextTimestamp)}", profilingOptions.IncludeStackTraceSnippet); //Using executeType instead of commandString parameter to avoid duplicate warnings by MP

            using (timing)
                return profiledCache.NextTimestamp();
        }

        private IDisposable Time([CallerMemberName] string caller = "")
        {
            var profiler = StackExchange.Profiling.MiniProfiler.Current;
            IDisposable timing = null;
            if (profiler != null)
                timing = profiler.CustomTiming(category, caller, includeStackTrace: profilingOptions.IncludeStackTraceSnippet);

            return timing;
        }

        private IDisposable TimeWithKey(object key, [CallerMemberName] string caller = "")
        {
            var profiler = StackExchange.Profiling.MiniProfiler.Current;
            IDisposable timing = null;
            if (profiler != null)
                timing = profiler.CustomTiming(category, $"{caller}: {key}", includeStackTrace: profilingOptions.IncludeStackTraceSnippet);

            return timing;
        }
    }
}