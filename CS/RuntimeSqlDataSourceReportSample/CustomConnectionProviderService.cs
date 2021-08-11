using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;

class CustomConnectionProviderService : IConnectionProviderService
{
    public SqlDataConnection LoadConnection(string connectionName)
    {
        if (connectionName == "MyRuntimeConnection")
        {
            MsSqlConnectionParameters connectionParameters = new MsSqlConnectionParameters()
            {
                ServerName = "localhost",
                DatabaseName = "NorthWind",
                UserName = null,
                Password = null,
                AuthorizationType = MsSqlAuthorizationType.Windows
            };
            return new SqlDataConnection("MyRuntimeConnection", connectionParameters);
        }
        string connectionString = "XpoProvider=MSSqlServer;Data Source=(LocalDB)\\MSSQLLocalDB;" +
            "AttachDbFilename=|DataDirectory|\\Test.mdf;" +
            "Integrated Security=True;Connect Timeout=30";
        CustomStringConnectionParameters fallbackConnectionParameters =
            new CustomStringConnectionParameters(connectionString);
        return new SqlDataConnection(connectionName, fallbackConnectionParameters);
    }
}
