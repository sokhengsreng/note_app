namespace NotesApplication.Data;

public interface IDbConnectionFactory
{
    System.Data.IDbConnection CreateConnection();
}

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public System.Data.IDbConnection CreateConnection()
    {
        return new System.Data.SqlClient.SqlConnection(_connectionString);
    }
}
