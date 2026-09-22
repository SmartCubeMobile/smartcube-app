#if WINFORMS
using System.Drawing;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
#endif

#if WPF
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
#endif

#if UWP || WINUI
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.Generic;
using System;
#endif

// In NuGET use 'Data Visualization Toolkit' to find it

#if ANDROIDX
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin;
#endif

#if SMARTMAUI
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

#endif
namespace SmartCubeMobile
{
    public class SmartChartsV2024
    {
        // THIS ROUTINE isn't SLOW!!  It generates the points very quickly,
        // However Silverligh takes FOR AGES to display the charts with the points.

        // What a FUCKER this was to get clean of CA1506 errors ... an absolute FUCKER
        //

        internal static PlotModel CreateDebitsCreditsSeries(string title,

                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
            // Chart2 is Debits/Credits by Month - doesn't depend on expiration
            //
            // We are going to dump this MS Chart shit and use Zedgraph ..
            // THAT DAY CAME!   We dumped it for OxyPlot!!!
            //
            // ... and what a FUCKING BIG MISTAKE *THAT* WAS!!!!!!!!!

            PlotModel plotmodel = new PlotModel()
            {
                PlotType = PlotType.XY
            };
            if (!string.IsNullOrEmpty(title))
            {
                plotmodel.Title = title;
            }
            plotmodel.Axes.Clear();
            plotmodel.Series.Clear();

            ChartTwo(ourviewmodel,
                    financeviewmodel,
                    plotmodel);
            return plotmodel;
        }

        internal static void ChartTwo(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            PlotModel chart2)
        {
#if WINFORMS
            //ChartArea area = new ChartArea();
            //chart2.ChartAreas.Add(area);
#endif
#if WINFORMS
            //chart2.Series.Clear();
#endif
#if WPF || UWP || WINUI
            chart2.Series.Clear();
#endif
#if ANDROIDX
            chart2.Series.Clear();
#endif
#if SMARTMAUI

            chart2.Series.Clear();
#endif
#if WINFORMS
            //chart2.ChartAreas[0].AxisX.LabelStyle = Chart2_Labelstyle_Datetime();
            //chart2.ChartAreas[0].AxisY.LabelStyle = Chart2_Labelstyle_Linear();

            // Contribution?  Instead of fucking off to the front room like she
            // normally does, she has chosen to have the telly on at full blast
            // right next me ..

            // This fucking bitch NEVER shuts the fuck up ...

            //List<SmartUtility.DReadingsCharts> d_readings_charts = Chart2Code(utilityviewmodel,
            //                                                                            resource_code);
            //if (d_readings_charts.Count > 0)
            //{
            //    Series lineseries0 = new Series()
            //    {
            //        ChartType = SeriesChartType.Column
            //    };

            //    // Fix line colour here before we set the points
            //    switch (resource_code)
            //    {
            //        case SmartParametersV2016.Electricity:
            //            // Set the column colour to Orange (DOESN'T WORK)
            //            lineseries0.Color = System.Drawing.Color.Orange;
            //            break;
            //        case SmartParametersV2016.Gas:
            //            // Set the column colour to Blue
            //            lineseries0.Color = System.Drawing.Color.Blue;
            //            break;
            //        default:
            //            lineseries0.Color = System.Drawing.Color.Magenta;
            //            break;
            //    }
            //    // Add the Series into the Chart
            //    chart2.Series.Add(lineseries0);              // This adds LegendItem[0]

            //    List<KeyValuePair<DateTime, double>> chart2_reading_points = new List<KeyValuePair<DateTime, double>>();

            //    foreach (SmartUtility.DReadingsCharts d_readings_charts_row in d_readings_charts)
            //    {
            //        chart2_reading_points.Add(new KeyValuePair<DateTime, double>(d_readings_charts_row.PERIOD_END, Convert.ToDouble(d_readings_charts_row.UNITS_USED)));
            //    }

            //    // Display Series 0 is the Quarterly or Monthly Bills
            //    chart2.Series[0].Points.DataBindXY(chart2_reading_points, "Key", chart2_reading_points, "Value");
            //}

#endif

            // Contribution?  Instead of fucking off to the front room like she
            // normally does, she has chosen to have the telly on at full blast
            // right next me ..

#if WINFORMS
            // Create ColumnSeries!!
            // YCMTSU = You COULDN'T make this shit up, you really just couldn't ..                    
            // Winforms OxyPlot.Core 2.0                <= YCMTSU !!
            // Winforms OxyPlot.WindowsForms 2.0        <= YCMTSU !!
            // UWP || WINUI OxyPlot.Core 2.0
            // UWP || WINUI OxyPlot.Windows 1.0
            //BarSeries chart2_points = new BarSeries
            //{
            //    XAxisKey = "Date",
            //    YAxisKey = "Value"
            //};
#endif
            // Create Column Series
            // Winforms OxyPlot.Core 2.1
            // Winforms OxyPlot.WindowsForms 2.1
            // WPF OxyPlot.Core 2.1
            // WPF OxyPlot.Wpf  2.1
#if WINFORMS || WPF
            Legend legend = new Legend();
#endif
            if (financeviewmodel.BanksChecked)
            {
#if WINFORMS || WPF
                legend.LegendTitle += "Banks";
#endif
#if UWP || WINUI
                chart2.Title += "Banks";
#endif
#if ANDROIDX
                chart2.Title += "Banks";
#endif
#if SMARTMAUI
                chart2.Title += "Banks";
#endif
            }
            if (financeviewmodel.SavingsChecked)
            {
#if WINFORMS || WPF
                if (!string.IsNullOrEmpty(legend.LegendTitle))
#endif
#if UWP || WINUI
                if (!string.IsNullOrEmpty(chart2.Title))
#endif
#if ANDROIDX
                if (!string.IsNullOrEmpty(chart2.Title))
#endif
#if SMARTMAUI
                if (!string.IsNullOrEmpty(chart2.Title))
#endif
                {
#if WINFORMS || WPF
                    legend.LegendTitle += "/";
#endif
#if UWP || WINUI
                    chart2.Title += "/";
#endif
#if ANDROIDX
                    chart2.Title += "/";
#endif
#if SMARTMAUI
                    chart2.Title += "/";
#endif
                }
#if WINFORMS || WPF
                legend.LegendTitle += "Savings";
#endif
#if UWP || WINUI
                chart2.Title += "Savings";
#endif
#if ANDROIDX
                chart2.Title += "Savings";
#endif
#if SMARTMAUI
                chart2.Title += "Savings";
#endif
            }
            if (financeviewmodel.InvestmentsChecked)
            {
#if WINFORMS || WPF
                if (!string.IsNullOrEmpty(legend.LegendTitle))
#endif
#if UWP || WINUI
                if (!string.IsNullOrEmpty(chart2.Title))
#endif
#if ANDROIDX
                if (!string.IsNullOrEmpty(chart2.Title))
#endif
#if SMARTMAUI
                if (!string.IsNullOrEmpty(chart2.Title))
#endif
                {
#if WINFORMS || WPF
                    legend.LegendTitle += "/";
#endif
#if UWP || WINUI
                    chart2.Title += "/";
#endif
#if ANDROIDX
                    chart2.Title += "/";
#endif
#if SMARTMAUI
                    chart2.Title += "/";
#endif
                }
#if WINFORMS || WPF
                legend.LegendTitle += "Investments";
#endif
#if UWP || WINUI
                chart2.Title += "Investments";
#endif
#if ANDROIDX
                chart2.Title += "Investments";
#endif
#if SMARTMAUI
                chart2.Title += "Investments";
#endif
            }
#if WINFORMS || WPF || UWP || WINUI
            BarSeries chart2_debits = new BarSeries
#endif
#if ANDROIDX
            ColumnSeries chart2_debits = new ColumnSeries
#endif
#if SMARTMAUI
            BarSeries chart2_debits = new BarSeries
#endif
            {
                Title = "Debits",
                StrokeColor = OxyPlot.OxyColors.Black,
                FillColor = OxyPlot.OxyColors.Pink,
                StrokeThickness = 1,
#if WINFORMS || WPF || WINUI
                XAxisKey = "Value",
                YAxisKey = "Date",
#endif
#if UWP
                XAxisKey = "Date",
                YAxisKey = "Value",
#endif
#if ANDROIDX
                XAxisKey = "Date",
                YAxisKey = "Value",
#endif
#if SMARTMAUI
                XAxisKey = "Value",
                YAxisKey = "Date",
#endif
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}"
            };
#if WINFORMS || WPF || UWP || WINUI
            BarSeries chart2_credits = new BarSeries
#endif
#if ANDROIDX
            ColumnSeries chart2_credits = new ColumnSeries
#endif
#if SMARTMAUI
            BarSeries chart2_credits = new BarSeries
#endif
            {
                Title = "Credits",
                StrokeColor = OxyPlot.OxyColors.Black,
                FillColor = OxyPlot.OxyColors.LightGreen,
                StrokeThickness = 1,
#if WINFORMS || WPF || WINUI
                XAxisKey = "Value",
                YAxisKey = "Date",
#endif
#if UWP
                XAxisKey = "Date",
                YAxisKey = "Value",
#endif
#if ANDROIDX
                XAxisKey = "Date",
                YAxisKey = "Value",
#endif
#if SMARTMAUI
                XAxisKey = "Value",
                YAxisKey = "Date",
#endif
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}"
            };

            // Add Axis labels
            // Specify key and position
            CategoryAxis chart2_categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = "Date",
                StringFormat = "MMM/yyyy",
                Angle = 45
            };

            // Specify key and position
            LinearAxis chart2_LinearAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Value",
                Title = "£"
            };
            // This fucking bitch NEVER shuts the fuck up ...
            chart2.Axes.Add(chart2_LinearAxis);

            List<SmartFinance.DebitsCreditsByMonth> d_debitscredits_charts = Chart2Code(financeviewmodel);
            if (d_debitscredits_charts.Count > 0)
            {
                // Fix line colour here before we set the points
                // Set the column colour to Red (DOESN'T WORK)
                foreach (SmartFinance.DebitsCreditsByMonth d_debitscredits_charts_row in d_debitscredits_charts)
                {
                    // YCMTSU = You COULDN'T make this shit up, you really just couldn't ..
#if WINFORMS || WPF || UWP || WINUI
                    BarItem valueitemDebits = new BarItem
#endif
#if ANDROIDX
                    ColumnItem valueitemDebits = new ColumnItem
#endif
#if SMARTMAUI
                    BarItem valueitemDebits = new BarItem 
#endif
                    {
                        Value = Convert.ToDouble(d_debitscredits_charts_row.DEBITS / 100.0),
                    };
                    chart2_debits.Items.Add(valueitemDebits);
#if WINFORMS || WPF || UWP || WINUI
                    BarItem valueitemCredits = new BarItem
#endif
#if ANDROIDX
                    ColumnItem valueitemCredits = new ColumnItem
#endif
#if SMARTMAUI
                    BarItem valueitemCredits = new BarItem
#endif
                    {
                        Value = Convert.ToDouble(d_debitscredits_charts_row.CREDITS / 100.0),

                    };
                    chart2_credits.Items.Add(valueitemCredits);
                    chart2_categoryAxis.Labels.Add(d_debitscredits_charts_row.PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat, financeviewmodel.CultureINF));
                }
            }
            chart2.Series.Add(chart2_debits);
            chart2.Series.Add(chart2_credits);
            chart2.Axes.Add(chart2_categoryAxis);
#if WINFORMS || WPF
            chart2.Legends.Add(legend);
#endif
            return;
        }

        internal static List<SmartFinance.DebitsCreditsByMonth> Chart2Code(FinanceViewModel financeviewmodel)
        {
            // These may be Quarterly or Monthly ... not yet!!
            List<SmartFinance.DebitsCreditsByMonth> totals_found = new List<SmartFinance.DebitsCreditsByMonth>();

            double debits_total = 0;
            double credits_total = 0;

            DateTime last_transaction_date = SmartParametersV2016.defaultDate;
            foreach (SmartFinance.CommonTransactionsView transactionView in financeviewmodel.FinanceTransactions)
            {
                DateTime rays = Convert.ToDateTime(transactionView.TRANSACTION_DATE, financeviewmodel.CultureINF);
                // Force it to be Day 1
                DateTime this_transaction_date = new DateTime(rays.Year, rays.Month, 1);
                if (DateTime.Compare(this_transaction_date, last_transaction_date) < 0)
                {
                    SmartFinance.DebitsCreditsByMonth monthlyTotal =
                        new SmartFinance.DebitsCreditsByMonth(

                        last_transaction_date.AddDays(-1).AddMonths(1),
                        debits_total,
                        credits_total
                    );
                    totals_found.Add(monthlyTotal);
                    debits_total = 0;
                    credits_total = 0;
                }
                if (transactionView.CREDITDEBIT_INDICATOR == 0)
                {
                    debits_total += Math.Abs(transactionView.AMOUNT);
                }
                else
                {
                    credits_total += transactionView.AMOUNT;
                }
                last_transaction_date = this_transaction_date;
            }
            // Don't forget last one!! I won't ... here it is ...
            SmartFinance.DebitsCreditsByMonth finalTotal = new SmartFinance.DebitsCreditsByMonth(

                last_transaction_date.AddDays(-1).AddMonths(1),
                debits_total,
                credits_total
            );
            totals_found.Add(finalTotal);
            // Try and get them in ascending date order?
            totals_found.Sort();
            return totals_found;
        }
    }
}