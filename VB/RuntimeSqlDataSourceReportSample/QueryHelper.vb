#Region "#using"
Imports DevExpress.DataAccess.Sql
#End Region

Namespace RuntimeSqlDataSourceReportSample
	Friend NotInheritable Class QueryHelper

		Private Sub New()
		End Sub

		#Region "CreateSelectQuery"
		Public Shared Function CreateSelectQuery() As SqlQuery
			Dim query As SelectQuery = SelectQueryFluentBuilder.AddTable("Categories").
				SelectColumn("CategoryName").
				GroupBy("CategoryName").
				Join("Products", "CategoryID").
				SelectColumn("ProductName",
							 AggregationType.Count,
							 "ProductCount").
							 SortBy("ProductName", AggregationType.Count,
									System.ComponentModel.ListSortDirection.Descending).
									GroupFilter("[ProductCount] > 7").
									Build("Categories")
			query.Name = "Categories with 7 and More Products"
			Return query
		End Function
		#End Region
		#Region "CreateStoredProcedureQuery"
		Public Shared Function CreateStoredProcedureQuery() As SqlQuery
			Dim spQuery As New StoredProcQuery("StoredProcedure", "Ten Most Expensive Products")
			Return spQuery
		End Function
		#End Region
		#Region "CreateCustomSqlQuery"
		Public Shared Function CreateCustomSqlQuery() As SqlQuery
			Dim query As New CustomSqlQuery()
			query.Name = "CustomQuery"
			query.Sql = "Select top 10 * from Products"
			Return query
		End Function
		#End Region
	End Class
End Namespace
