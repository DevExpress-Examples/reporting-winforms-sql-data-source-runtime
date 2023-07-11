#Region "usings"
Imports DevExpress.DataAccess.Sql
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI
Imports System.Drawing
#End Region

Namespace RuntimeSqlDataSourceReportSample
	Friend Class ReportCreator
		#Region "CreateReport"
		Public Shared Function CreateReport(ByVal dataSource As Object) As XtraReport
			Dim ds As SqlDataSource = TryCast(dataSource, SqlDataSource)
			If ds Is Nothing Then
				Return New XtraReport()
			End If

			' Create an empty report.
			Dim report As New XtraReport()

			' Bind the report to a data source.
			report.DataSource = ds
			report.DataMember = ds.Queries(0).Name

			' Create a master part.
			CreateReportHeader(report, "Products by Categories")
			CreateDetail(report)

			' Create a detail part.
			CreateDetailReport(report, ds.Queries(0).Name & "." & ds.Relations(0).Name)
			Return report
		End Function
		#End Region

		#Region "CreateMasterReport"
		Private Shared Sub CreateReportHeader(ByVal report As XtraReport, ByVal caption As String)
			' Create a report title.
			Dim label As New XRLabel()
			label.Font = New Font("Tahoma", 12, FontStyle.Bold)
			label.Text = caption
			label.WidthF = 300F

			' Create a report header and add the title to it.
			Dim reportHeader As New ReportHeaderBand()
			report.Bands.Add(reportHeader)
			reportHeader.Controls.Add(label)
			reportHeader.HeightF = label.HeightF
		End Sub

		Private Shared Sub CreateDetail(ByVal report As XtraReport)
			' Create a new label bound to the CategoryName data field.
			Dim labelDetail As New XRLabel()
			labelDetail.Font = New Font("Tahoma", 10, FontStyle.Bold)
			labelDetail.WidthF = 300.0F

			' Bind the label to the CategoryName data field.
			labelDetail.ExpressionBindings.Add(
				New ExpressionBinding("BeforePrint", "Text", "'Category: ' + [CategoryName]"))

			' Create a detail band and display the category name in it.
			Dim detailBand As New DetailBand()
			detailBand.Height = labelDetail.Height
			detailBand.KeepTogetherWithDetailReports = True
			report.Bands.Add(detailBand)
			labelDetail.TopF = detailBand.LocationFloat.Y + 20F
			detailBand.Controls.Add(labelDetail)
		End Sub
		#End Region 

		#Region "CreateDetailReport"
		Private Shared Sub CreateDetailReport(ByVal report As XtraReport, ByVal dataMember As String)
			' Create a detail report band and bind it to data.
			Dim detailReportBand As New DetailReportBand()
			report.Bands.Add(detailReportBand)
			detailReportBand.DataSource = report.DataSource
			detailReportBand.DataMember = dataMember

			' Add a header to the detail report.
			Dim detailReportHeader As New ReportHeaderBand()
			detailReportBand.Bands.Add(detailReportHeader)

			Dim tableHeader As New XRTable()
			tableHeader.BeginInit()
			tableHeader.Rows.Add(New XRTableRow())
			tableHeader.Borders = BorderSide.All
			tableHeader.BorderColor = Color.DarkGray
			tableHeader.Font = New Font("Tahoma", 10, FontStyle.Bold)
			tableHeader.Padding = 10
			tableHeader.TextAlignment = TextAlignment.MiddleLeft

			Dim cellHeader1 As New XRTableCell()
			cellHeader1.Text = "Product Name"
			Dim cellHeader2 As New XRTableCell()
			cellHeader2.Text = "Unit Price"
			cellHeader2.TextAlignment = TextAlignment.MiddleRight

			tableHeader.Rows(0).Cells.AddRange(New XRTableCell() { cellHeader1, cellHeader2 })
			detailReportHeader.Height = tableHeader.Height
			detailReportHeader.Controls.Add(tableHeader)

			' Adjust the table width.
			AddHandler tableHeader.BeforePrint, AddressOf tableHeader_BeforePrint
			tableHeader.EndInit()

			' Create a detail band.
			Dim tableDetail As New XRTable()
			tableDetail.BeginInit()
			tableDetail.Rows.Add(New XRTableRow())
			tableDetail.Borders = BorderSide.Left Or BorderSide.Right Or BorderSide.Bottom
			tableDetail.BorderColor = Color.DarkGray
			tableDetail.Font = New Font("Tahoma", 10)
			tableDetail.Padding = 10
			tableDetail.TextAlignment = TextAlignment.MiddleLeft

			Dim cellDetail1 As New XRTableCell()
			Dim cellDetail2 As New XRTableCell()
			cellDetail2.TextAlignment = TextAlignment.MiddleRight

			cellDetail1.ExpressionBindings.Add(
				New ExpressionBinding("BeforePrint", "Text", "[ProductName]"))
			cellDetail2.ExpressionBindings.Add(
				New ExpressionBinding("BeforePrint", "Text", "FormatString('{0:$0.00}', [UnitPrice])"))


			tableDetail.Rows(0).Cells.AddRange(New XRTableCell() { cellDetail1, cellDetail2 })

			Dim detailBand As New DetailBand()
			detailBand.Height = tableDetail.Height
			detailReportBand.Bands.Add(detailBand)
			detailBand.Controls.Add(tableDetail)

			' Adjust the table width.
			AddHandler tableDetail.BeforePrint, AddressOf tableDetail_BeforePrint
			tableDetail.EndInit()

			' Create and assign different odd and even styles.
			Dim oddStyle As New XRControlStyle()
			Dim evenStyle As New XRControlStyle()

			oddStyle.BackColor = Color.WhiteSmoke
			oddStyle.StyleUsing.UseBackColor = True
			oddStyle.Name = "OddStyle"

			evenStyle.BackColor = Color.White
			evenStyle.StyleUsing.UseBackColor = True
			evenStyle.Name = "EvenStyle"

			report.StyleSheet.AddRange(New XRControlStyle() { oddStyle, evenStyle })

			tableDetail.OddStyleName = "OddStyle"
			tableDetail.EvenStyleName = "EvenStyle"
		End Sub

		Private Shared Sub AdjustTableWidth(ByVal table As XRTable)
			Dim report As XtraReport = table.RootReport
			table.WidthF = report.PageWidth - report.Margins.Left - report.Margins.Right
		End Sub

		Private Shared Sub tableHeader_BeforePrint(ByVal sender As Object,
												   ByVal e As System.ComponentModel.CancelEventArgs)
			AdjustTableWidth(TryCast(sender, XRTable))
		End Sub

		Private Shared Sub tableDetail_BeforePrint(ByVal sender As Object,
												   ByVal e As System.ComponentModel.CancelEventArgs)
			AdjustTableWidth(TryCast(sender, XRTable))
		End Sub
#End Region
	End Class
End Namespace
