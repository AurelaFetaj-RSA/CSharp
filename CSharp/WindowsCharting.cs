using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Drawing;
using System.Data;

namespace ProductionLaunch
{
    sealed class WindowsCharting
    {
        //method generates the chart
        #pragma warning disable 0628
        protected internal Chart GenerateChart(DataTable dtChartDataSource, int width,int height,string bgColor,int intType )
        {
            Chart chart = new Chart()
            {
                Width = width,
                Height = height
            };
            chart.Legends.Add(new Legend(){Name = "Legend"});
            chart.Legends[0].Docking = Docking.Bottom;
            ChartArea chartArea = new ChartArea() { Name = "ChartArea" };

            //Remove X-axis grid lines
            chartArea.AxisX.MajorGrid.LineWidth = 0;
            //Remove Y-axis grid lines
            chartArea.AxisY.MajorGrid.LineWidth = 0;
            //Chart Area Back Color
            chartArea.BackColor = Color.FromName(bgColor);
            chart.ChartAreas.Add(chartArea);
            chart.Palette = ChartColorPalette.BrightPastel;
            string series = string.Empty;
            //create series and add data points to the series
            if (dtChartDataSource != null)
            {
                foreach (DataColumn dc in dtChartDataSource.Columns)
                {
                    //a series to the chart
                    if (chart.Series.FindByName(dc.ColumnName) == null)
                    {
                        series = dc.ColumnName;

                        chart.Series.Add(series);
                        chart.Series[series].ChartType = (SeriesChartType)intType;
                    }
                    //Add data points to the series
                    float i = 1f;
                    foreach (DataRow dr in dtChartDataSource.Rows)
                    {
                        double dataPoint = 0;
                        double.TryParse(dr[dc.ColumnName].ToString(), out dataPoint);
                        DataPoint objDataPoint = new DataPoint() { AxisLabel = "series", YValues = new double[] { dataPoint } };
                        chart.Series[series].Points.AddXY(i + 33, dataPoint);
                        if (series == "paia target")
                        {
                            chart.Series[series].Color = Color.SteelBlue;
                        }
                        else if (series == "paia caricate")
                        {
                            chart.Series[series].Color = Color.DarkOrange;
                        }
                        else
                        {
                            chart.Series[series].Color = Color.FromArgb(195, 222, 155);
                        }
                        //chart.Series[series].Color = seriesColor;
                       //chart.Series[series].Points.Add(dataPoint);
                       i = i + 0.5f;
                    }
                }
            }
            return chart;
        }
    }
}
