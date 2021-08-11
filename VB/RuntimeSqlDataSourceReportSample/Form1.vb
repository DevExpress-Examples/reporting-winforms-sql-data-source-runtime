#Region "usingsSql"
Imports DevExpress.DataAccess.Sql
Imports DevExpress.DataAccess.ConnectionParameters
#End Region
#Region "usings"
Imports DevExpress.DataAccess.Wizard.Services
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.UserDesigner
Imports System.ComponentModel.Design
Imports System
#End Region

Namespace RuntimeSqlDataSourceReportSample
	Partial Public Class Form1
		Inherits DevExpress.XtraEditors.XtraForm

		#Region "datasource"
		Private Property DataSource() As SqlDataSource
		#End Region
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub simpleButton1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton1.Click
			OpenReportDesigner()
		End Sub

		Private Sub OpenReportDesigner()
#Region "OpenReportDesigner"
			CreateSqlDataSource()
			Dim rep As XtraReport = ReportCreator.CreateReport(DataSource)
			Dim designer As New ReportDesignTool(rep)
			designer.ShowRibbonDesignerDialog()
#End Region
		End Sub

		#Region "CreateSqlDataSource"
		Public Sub CreateSqlDataSource()
			InitializeSqlDataSource()
			AddQueryRelations()

			DataSource.Queries.Add(QueryHelper.CreateSelectQuery())
			DataSource.Queries.Add(QueryHelper.CreateCustomSqlQuery())
			DataSource.Queries.Add(QueryHelper.CreateStoredProcedureQuery())

			DataSource.RebuildResultSchema()
		End Sub
		#End Region

		#Region "InitializeSqlDataSource"
		Private Sub InitializeSqlDataSource()
			'CreateConnectionFromParameters();

			CreateConnectionFromAppConfig()

			'CreateConnectionFromString();

			'UseConfigureDataConnectionEvent();
		End Sub
		#End Region
		#Region "CreateConnectionFromParameters"
		Private Sub CreateConnectionFromParameters()
			Dim connectionParameters As New MsSqlConnectionParameters() With
				{
				.ServerName = "localhost",
				.DatabaseName = "NorthWind",
				.UserName = Nothing,
				.Password = Nothing,
				.AuthorizationType = MsSqlAuthorizationType.Windows}
			DataSource = New SqlDataSource(connectionParameters)
		End Sub
		#End Region
		#Region "CreateConnectionFromAppConfig"
		Private Sub CreateConnectionFromAppConfig()
			DataSource = New SqlDataSource("TestConnectionString")
		End Sub
		#End Region
		#Region "CreateConnectionFromString"
		Private Sub CreateConnectionFromString()
			Dim connectionString As String = "XpoProvider=MSSqlServer;Data Source=(LocalDB)\MSSQLLocalDB;" _
				& "AttachDbFilename=|DataDirectory|\Test.mdf;" _
				& "Integrated Security=True;Connect Timeout=30"
			Dim connectionParameters As New CustomStringConnectionParameters(connectionString)
			DataSource = New SqlDataSource(connectionParameters)
		End Sub
#End Region

#Region "AddRelations"
		Private Sub AddQueryRelations()
			Dim categories As SelectQuery = SelectQueryFluentBuilder.
				AddTable("Categories").
				SelectAllColumns().Build("Categories")
			Dim products As SelectQuery = SelectQueryFluentBuilder.
				AddTable("Products").
				SelectAllColumns().Build("Products")

			DataSource.Queries.AddRange(New SqlQuery() { categories, products })
			DataSource.Relations.Add(New MasterDetailInfo("Categories", "Products", "CategoryID", "CategoryID"))
		End Sub
		#End Region
		Private Sub simpleButton2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles simpleButton2.Click
			OpenReportDesignerWithService()
		End Sub
		#Region "OpenReportDesignerWithService"
		Private Sub OpenReportDesignerWithService()
			Dim connectionProviderService As New CustomConnectionProviderService()

			DataSource = New SqlDataSource("MyRuntimeConnection")
			AddQueryRelations()

			Dim rep As XtraReport = ReportCreator.CreateReport(DataSource)
			Dim designer As New ReportDesignTool(rep)
			ReplaceService(designer.DesignRibbonForm.DesignMdiController,
						   GetType(IConnectionProviderService), connectionProviderService)
			AddHandler designer.DesignRibbonForm.DesignMdiController.DesignPanelLoaded,
				AddressOf DesignMdiControllerOnDesignPanelLoaded
			designer.ShowRibbonDesignerDialog()
		End Sub
		Private Sub ReplaceService(ByVal container As IServiceContainer,
								   ByVal serviceType As Type,
								   ByVal serviceInstance As Object)
			If container.GetService(serviceType) IsNot Nothing Then
				container.RemoveService(serviceType)
			End If
			container.AddService(serviceType, serviceInstance)
		End Sub
		Private Sub DesignMdiControllerOnDesignPanelLoaded(ByVal sender As Object,
														   ByVal e As DesignerLoadedEventArgs)
			ReplaceService(e.DesignerHost, GetType(IConnectionProviderService),
						   New CustomConnectionProviderService())
		End Sub
#End Region
	End Class
End Namespace
