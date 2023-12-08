## About

MiniProfiler integration for NHibernate, suitable for all database drivers.

## How to Use

### SQL Profiling:
Using the *ProfiledDriver* extension method:
```csharp
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