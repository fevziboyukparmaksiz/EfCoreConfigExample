## Appsettings.json
```json
{
  "DatabaseOptions": {
    "MaxRetryCount": 3,
    "CommandTimeout": 30,
    "EnableDetailedErrors": false,
    "EnableSensitiveDataLogging": true
  }
}
```
## The property names should match those in the appsettings.json file.
```chasrp
public class DatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetryCount { get; set; }
    public int CommandTimeOut { get; set; }
    public bool EnableDetailedErrors { get; set; }
    public bool EnableSensitiveDataLogging { get; set; }
}
```
## Bind the DatabaseOptions class to the DatabaseOptions section
```chasrp
public class DatabaseOptionsSetup : IConfigureOptions<DatabaseOptions>
{
    private readonly IConfiguration _configuration;
    private const string ConfigurationSectionName = "DatabaseOptions";
    public DatabaseOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(DatabaseOptions options)
    {
        var connectionString = _configuration.GetConnectionString("Database");
        options.ConnectionString = connectionString;
        _configuration.GetSection(ConfigurationSectionName).Bind(options);
    }
}
```
## Implemented DatabaseOptions using the Options Pattern
```chasrp
builder.Services.ConfigureOptions<DatabaseOptionsSetup>();

builder.Services.AddDbContext<AppDbContext>(
    (serviceProvider,options) =>
{
    var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;

    options.UseSqlServer(databaseOptions.ConnectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
        sqlServerOptionsAction.CommandTimeout(databaseOptions.CommandTimeOut);
    });

    options.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);

    options.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);

});
```

