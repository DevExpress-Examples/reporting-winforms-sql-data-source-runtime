Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Sql
Imports DevExpress.DataAccess.Wizard.Services

Friend Class CustomConnectionProviderService
	Implements IConnectionProviderService

	Public Function LoadConnection(ByVal connectionName As String) As SqlDataConnection Implements IConnectionProviderService.LoadConnection
		If connectionName = "MyRuntimeConnection" Then
			Dim connectionParameters As New MsSqlConnectionParameters() With
				{.ServerName = "localhost\SQLEXPRESS",
				.DatabaseName = "NorthWind",
				.UserName = Nothing,
				.Password = Nothing,
				.AuthorizationType = MsSqlAuthorizationType.Windows}
			Return New SqlDataConnection("MyRuntimeConnection", connectionParameters)
		End If
		Dim connectionString As String = "XpoProvider=MSSqlServer;Data Source=(LocalDB)\MSSQLLocalDB;" _
		& "AttachDbFilename=|DataDirectory|\Test.mdf;" _
		& "Integrated Security=True;Connect Timeout=30"
		Dim fallbackConnectionParameters As New CustomStringConnectionParameters(connectionString)
		Return New SqlDataConnection(connectionName, fallbackConnectionParameters)
	End Function
End Class
