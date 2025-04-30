using Microsoft.Extensions.Configuration;

public class DatabaseConnectionService
{
    private readonly IConfiguration _configuration;
    private string _activeDatabaseKey;

    public DatabaseConnectionService(IConfiguration configuration)
    {
        _configuration = configuration;
        _activeDatabaseKey = _configuration.GetValue<string>("AppSettings:SelectedDatabase") ?? "DefaultConnection";
    }

    // ✅ Get current database key
    public string GetActiveDatabaseKey()
    {
        return _activeDatabaseKey;
    }

    // ✅ Change the active database dynamically
    public void SetActiveDatabase(string newDatabaseKey)
    {
        _activeDatabaseKey = newDatabaseKey;
    }

    // ✅ Retrieve the correct connection string
    public string GetConnectionString()
    {
        return _configuration.GetConnectionString(_activeDatabaseKey)
               ?? throw new InvalidOperationException("Invalid database selection.");
    }
}
