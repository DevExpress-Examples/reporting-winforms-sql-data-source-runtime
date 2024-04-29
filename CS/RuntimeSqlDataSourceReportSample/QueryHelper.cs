#region #using
using DevExpress.DataAccess.Sql;
#endregion

namespace RuntimeSqlDataSourceReportSample
{
    static class QueryHelper
    {
        #region CreateSelectQuery
        public static SqlQuery CreateSelectQuery()
        {
            SelectQuery query = SelectQueryFluentBuilder
                .AddTable("Categories")
                .SelectColumn("CategoryName")
                                .GroupBy("CategoryName")
                                .Join("Products", "CategoryID")
                                .SelectColumn("ProductName", AggregationType.Count, "ProductCount")
                                .SortBy("ProductName", AggregationType.Count,
                                    System.ComponentModel.ListSortDirection.Descending)
                                .GroupFilter("[ProductCount] > 7")
                                .Build("Categories");
            query.Name = "Categories with 7 or More Products";
            return query;
        }
        #endregion
        #region CreateStoredProcedureQuery
        public static SqlQuery CreateStoredProcedureQuery()
        {
            StoredProcQuery spQuery =
                new StoredProcQuery("StoredProcedure", "SalesByCategory");
            spQuery.Parameters.Add(new QueryParameter("@CategoryName", typeof(string), "SeaFood"));
            spQuery.Parameters.Add(new QueryParameter("@OrdYear", typeof(string), "1997"));
            return spQuery;
        }
        #endregion
        #region CreateCustomSqlQuery
        public static SqlQuery CreateCustomSqlQuery()
        {
            CustomSqlQuery query = new CustomSqlQuery();
            query.Name = "CustomQuery";
            query.Sql = "Select top 10 * from Products";
            return query;
        }
        #endregion
    }
}