#region usings
using DevExpress.DataAccess.Sql;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System.Drawing;
#endregion

namespace RuntimeSqlDataSourceReportSample
{
    class ReportCreator
    {
        #region CreateReport
        public static XtraReport CreateReport(object dataSource)
        {
            SqlDataSource ds = dataSource as SqlDataSource;
            if (ds == null) return new XtraReport();

            // Create an empty report.
            XtraReport report = new XtraReport();
            
            // Bind the report to a data source.
            report.DataSource = ds;
            report.DataMember = ds.Queries[0].Name;

            // Create a master part.
            CreateReportHeader(report, "Products by Categories");
            CreateDetail(report);

            // Create a detail part.
            CreateDetailReport(report, ds.Queries[0].Name + "." + ds.Relations[0].Name);
            return report;
        }
        #endregion

        #region CreateMasterReport
        private static void CreateReportHeader(XtraReport report, string caption)
        {
            // Create a report title.
            XRLabel label = new XRLabel();
            label.Font = new Font("Tahoma", 12, FontStyle.Bold);
            label.Text = caption;
            label.WidthF = 300F;

            // Create a report header and add the title to it.
            ReportHeaderBand reportHeader = new ReportHeaderBand();
            report.Bands.Add(reportHeader);
            reportHeader.Controls.Add(label);
            reportHeader.HeightF = label.HeightF;
        }

        private static void CreateDetail(XtraReport report)
        {
            // Create a new label bound to the CategoryName data field.
            XRLabel labelDetail = new XRLabel();
            labelDetail.Font = new Font("Tahoma", 10, FontStyle.Bold);
            labelDetail.WidthF = 300F;

            // Bind the label to the CategoryName data field.
            labelDetail.ExpressionBindings.Add(
                new ExpressionBinding("BeforePrint", "Text", "'Category: ' + [CategoryName]"));

            // Create a detail band and display the category name in it.
            DetailBand detailBand = new DetailBand();
            detailBand.Height = labelDetail.Height;
            detailBand.KeepTogetherWithDetailReports = true;
            report.Bands.Add(detailBand);
            labelDetail.TopF = detailBand.LocationFloat.Y + 20F;
            detailBand.Controls.Add(labelDetail);
        }
        #endregion 

        #region CreateDetailReport
        private static void CreateDetailReport(XtraReport report, string dataMember)
        {
            // Create a detail report band and bind it to data.
            DetailReportBand detailReportBand = new DetailReportBand();
            report.Bands.Add(detailReportBand);
            detailReportBand.DataSource = report.DataSource;
            detailReportBand.DataMember = dataMember;

            // Add a header to the detail report.
            ReportHeaderBand detailReportHeader = new ReportHeaderBand();
            detailReportBand.Bands.Add(detailReportHeader);

            XRTable tableHeader = new XRTable();
            tableHeader.BeginInit();
            tableHeader.Rows.Add(new XRTableRow());
            tableHeader.Borders = BorderSide.All;
            tableHeader.BorderColor = Color.DarkGray;
            tableHeader.Font = new Font("Tahoma", 10, FontStyle.Bold);
            tableHeader.Padding = 10;
            tableHeader.TextAlignment = TextAlignment.MiddleLeft;

            XRTableCell cellHeader1 = new XRTableCell();
            cellHeader1.Text = "Product Name";
            XRTableCell cellHeader2 = new XRTableCell();
            cellHeader2.Text = "Unit Price";
            cellHeader2.TextAlignment = TextAlignment.MiddleRight;

            tableHeader.Rows[0].Cells.AddRange(new XRTableCell[] { cellHeader1, cellHeader2 });
            detailReportHeader.Height = tableHeader.Height;
            detailReportHeader.Controls.Add(tableHeader);

            // Adjust the table width.
            tableHeader.BeforePrint += tableHeader_BeforePrint;
            tableHeader.EndInit();

            // Create a detail band.
            XRTable tableDetail = new XRTable();
            tableDetail.BeginInit();
            tableDetail.Rows.Add(new XRTableRow());
            tableDetail.Borders = BorderSide.Left | BorderSide.Right | BorderSide.Bottom;
            tableDetail.BorderColor = Color.DarkGray;
            tableDetail.Font = new Font("Tahoma", 10);
            tableDetail.Padding = 10;
            tableDetail.TextAlignment = TextAlignment.MiddleLeft;

            XRTableCell cellDetail1 = new XRTableCell();
            XRTableCell cellDetail2 = new XRTableCell();
            cellDetail2.TextAlignment = TextAlignment.MiddleRight;

            cellDetail1.ExpressionBindings.Add(
                new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
            cellDetail2.ExpressionBindings.Add(
                new ExpressionBinding("BeforePrint", "Text",
                "FormatString('{0:$0.00}', [UnitPrice])"));


            tableDetail.Rows[0].Cells.AddRange(new XRTableCell[] { cellDetail1, cellDetail2 });

            DetailBand detailBand = new DetailBand();
            detailBand.Height = tableDetail.Height;
            detailReportBand.Bands.Add(detailBand);
            detailBand.Controls.Add(tableDetail);

            // Adjust the table width.
            tableDetail.BeforePrint += tableDetail_BeforePrint;
            tableDetail.EndInit();

            // Create and assign different odd and even styles.
            XRControlStyle oddStyle = new XRControlStyle();
            XRControlStyle evenStyle = new XRControlStyle();

            oddStyle.BackColor = Color.WhiteSmoke;
            oddStyle.StyleUsing.UseBackColor = true;
            oddStyle.Name = "OddStyle";

            evenStyle.BackColor = Color.White;
            evenStyle.StyleUsing.UseBackColor = true;
            evenStyle.Name = "EvenStyle";

            report.StyleSheet.AddRange(new XRControlStyle[] { oddStyle, evenStyle });

            tableDetail.OddStyleName = "OddStyle";
            tableDetail.EvenStyleName = "EvenStyle";
        }

        private static void AdjustTableWidth(XRTable table)
        {
            XtraReport report = table.RootReport;
            table.WidthF = report.PageWidth - report.Margins.Left - report.Margins.Right;
        }

        static void tableHeader_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AdjustTableWidth(sender as XRTable);
        }

        static void tableDetail_BeforePrint(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AdjustTableWidth(sender as XRTable);
        }
        #endregion
    }
}