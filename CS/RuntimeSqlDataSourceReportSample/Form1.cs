﻿#region usingsSql
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
#endregion
#region usings
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using System.ComponentModel.Design;
using System;
#endregion

namespace RuntimeSqlDataSourceReportSample
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        #region datasource
        SqlDataSource DataSource { get; set; }
        #endregion
        public Form1()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            OpenReportDesigner();
        }

        private void OpenReportDesigner()
        {
            #region OpenReportDesigner
            CreateSqlDataSource();
            XtraReport rep = ReportCreator.CreateReport(DataSource);
            ReportDesignTool designer = new ReportDesignTool(rep);
            designer.ShowRibbonDesignerDialog();
            #endregion
        }

        #region CreateSqlDataSource
        public void CreateSqlDataSource()
        {
            InitializeSqlDataSource();
            AddQueryRelations();

            DataSource.Queries.Add(QueryHelper.CreateSelectQuery());
            DataSource.Queries.Add(QueryHelper.CreateCustomSqlQuery());
            DataSource.Queries.Add(QueryHelper.CreateStoredProcedureQuery());

            DataSource.RebuildResultSchema();
        }
        #endregion

        #region InitializeSqlDataSource
        void InitializeSqlDataSource()
        {
            //CreateConnectionFromParameters();

            CreateConnectionFromAppConfig();

            //CreateConnectionFromString();

            //UseConfigureDataConnectionEvent();
        }
        #endregion
        #region CreateConnectionFromParameters
        private void CreateConnectionFromParameters()
        {
            MsSqlConnectionParameters connectionParameters = new MsSqlConnectionParameters()
            {
                ServerName = "localhost",
                DatabaseName = "NorthWind",
                UserName = null,
                Password = null,
                AuthorizationType = MsSqlAuthorizationType.Windows
            };
            DataSource = new SqlDataSource(connectionParameters);
        }
        #endregion
        #region CreateConnectionFromAppConfig
        private void CreateConnectionFromAppConfig()
        {
            DataSource = new SqlDataSource("TestConnectionString");
        }
        #endregion
        #region CreateConnectionFromString
        private void CreateConnectionFromString()
        {
            string connectionString = "XpoProvider=MSSqlServer;Data Source=(LocalDB)\\MSSQLLocalDB;" +
                "AttachDbFilename=|DataDirectory|\\Test.mdf;" +
                "Integrated Security=True;Connect Timeout=30";
            CustomStringConnectionParameters connectionParameters =
                new CustomStringConnectionParameters(connectionString);
            DataSource = new SqlDataSource(connectionParameters);
        }
        #endregion

        #region AddRelations
        void AddQueryRelations()
        {
            SelectQuery categories = SelectQueryFluentBuilder
                .AddTable("Categories")
                .SelectAllColumns()
                .Build("Categories");
            SelectQuery products = SelectQueryFluentBuilder
                .AddTable("Products")
                .SelectAllColumns()
                .Build("Products");

            DataSource.Queries.AddRange(new SqlQuery[] { categories, products });
            DataSource.Relations.Add(
                new MasterDetailInfo("Categories", "Products", "CategoryID", "CategoryID"));
        }
        #endregion
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            OpenReportDesignerWithService();
        }
        #region OpenReportDesignerWithService
        private void OpenReportDesignerWithService()
        {
            CustomConnectionProviderService connectionProviderService =
                new CustomConnectionProviderService();

            DataSource = new SqlDataSource("MyRuntimeConnection");
            AddQueryRelations();

            XtraReport rep = ReportCreator.CreateReport(DataSource);
            ReportDesignTool designer = new ReportDesignTool(rep);
            ReplaceService(designer.DesignRibbonForm.DesignMdiController,
                typeof(IConnectionProviderService),
                connectionProviderService);
            designer.DesignRibbonForm.DesignMdiController.DesignPanelLoaded +=
                DesignMdiControllerOnDesignPanelLoaded;
            designer.ShowRibbonDesignerDialog();
        }
        private void ReplaceService(IServiceContainer container,
            Type serviceType,
            object serviceInstance)
        {
            if (container.GetService(serviceType) != null)
                container.RemoveService(serviceType);
            container.AddService(serviceType, serviceInstance);
        }
        private void DesignMdiControllerOnDesignPanelLoaded(object sender, DesignerLoadedEventArgs e)
        {
            ReplaceService(e.DesignerHost, typeof(IConnectionProviderService),
                new CustomConnectionProviderService());
        }
        #endregion
    }
}
