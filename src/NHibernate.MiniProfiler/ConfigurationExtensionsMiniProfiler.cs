using NHibernate.Cache;
using NHibernate.Cfg.Loquacious;
using NHibernate.Driver;
using NHibernate.MiniProfiler;

// ReSharper disable once CheckNamespace
namespace NHibernate.Cfg
{
    public static class ConfigurationExtensionsMiniProfiler
    {
        /// <summary>
        /// Enable profiling for the given driver.
        /// </summary>
        /// <typeparam name="TDriver">The driver to profile.</typeparam>
        /// <param name="cfg"></param>
        public static void ProfiledDriver<TDriver>(this IDbIntegrationConfigurationProperties cfg) where TDriver : DriverBase, new() => cfg.Driver<ProfiledDriver<TDriver>>();

        /// <summary>
        /// Enable profiling for the cache created by the given provider.
        /// </summary>
        /// <typeparam name="TCacheProvider">The cache provider whose cache should be profiled.</typeparam>
        /// <param name="cfg"></param>
        public static void ProfiledCache<TCacheProvider>(this ICacheConfigurationProperties cfg) where TCacheProvider : ICacheProvider, new() => cfg.Provider<ProfiledCacheProvider<TCacheProvider>>();
    }
}