# NHibernate.MiniProfiler

[![NuGet](https://img.shields.io/nuget/vpre/NHibernate.MiniProfiler.svg)](https://www.nuget.org/packages/NHibernate.MiniProfiler/)

MiniProfiler integration for NHibernate, suitable for all database drivers.

## Usage

Install <a href="https://www.nuget.org/packages/NHibernate.MiniProfiler" target="_blank">NHibernate.MiniProfiler</a>

### SQL Profiling:
Using the *ProfiledDriver* extension method:
```
var cfg = new Configuration().DataBaseIntegration(c => { c.ProfiledDriver<MySqlConnectorDriver>(); });
```
or by decorating the driver class:
```
// NHibernate
var cfg = new Configuration().DataBaseIntegration(c => { c.Driver<ProfiledDriver<MySqlConnectorDriver>>(); });

// Fluent NHibernate
MySQLConfiguration.Standard.Driver<ProfiledDriver<MySqlConnectorDriver>>();
```

### Query Cache/2nd Level Cache Profiling:
In addition to profiling SQL statements you can also profile the Query/2nd Level Cache.

Using the *ProfiledCache* extension method:
```
var cfg = new Configuration().Cache(c => { c.ProfiledCache<CoreMemoryCacheProvider>(); });
```
Decorating the cache provider class:
```
var cfg = new Configuration().Cache(c => { c.Provider<ProfiledCacheProvider<CoreMemoryCacheProvider>>(); });
```

#### Options

There are a couple of options available for profiling the NHibernate caches:

| **Config Key** | **Description**
| ----------- | ----------- |
| cache.profiling.includestacktracesnippet    | Configures whether to include the stack trace in the profiling results. Default is <b>false</b>.<br/>This setting will not have any effect, if the MiniProfiler setting *ExcludeStackTraceSnippetFromCustomTimings* is set to <b>true</b>.|
| cache.profiling.categoryname                | Configures the category name to use in the MiniProfiler results view. Default is <b>NHibernate Cache</b>.|
| cache.profiling.includeregionincategoryname | Configures whether to include the name of the cache region that is being profiled in the MiniProfiler results view. Default is <b>false</b>.|

To set these options, use the *SetProperty* method of the NHibernate `Configuration` class e.g.: 
``` 
var cfg = new Configuration();
cfg.SetProperty(CacheProfilingOptions.ConfigKeys.IncludeStackTraceSnippet, "true");
cfg.SetProperty(CacheProfilingOptions.ConfigKeys.IncludeRegionInCategoryName, "true");
```

### Common Issues:

#### System.InvalidCastException: Unable to cast object of type 'StackExchange.Profiling.Data.ProfiledDbConnection' to type 'Your data provider's DbConnection'.

This may happen under ASP.NET on .NET Framework when using async/await, because `MiniProfiler.Current` is null after an async transition. The causes `ProfiledDriver` to return the implementation of `DbCommand` from the database provider instead of returning an instance of `ProfiledDbCommand`.

To resolve this, configure MiniProfiler's `ProfilerProvider` setting as follows:

```
MiniProfiler.Configure(new MiniProfilerOptions
{
    ProfilerProvider = new AspNetRequestProvider(true)
});
```

### License

This software is distributed under the terms of the Free Software Foundation [Lesser GNU Public License (LGPL), version 2.1][D1] (see [LICENSE.txt][D2]).

[D1]: http://www.gnu.org/licenses/lgpl-2.1-standalone.html
[D2]: LICENSE.txt
