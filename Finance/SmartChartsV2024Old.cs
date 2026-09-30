using System;
using System.Collections.ObjectModel;



#if WINFORMS
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
#endif

#if WPF
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
#endif

#if UWP
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
#endif

#if ANDROID
using OxyPlot.Series;
using OxyPlot.Axes;
using Android.Graphics;
using OxyPlot;
using static Android.Graphics.PathDashPathEffect;
//using OxyPlot.Legends;
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
        //short institution_code,
        //short brand_code)
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
#if WPF || ANDROID
            chart2.Series.Clear();
#endif

#if WINFORMS
            //chart2.ChartAreas[0].AxisX.LabelStyle = Chart2_Labelstyle_Datetime();
            //chart2.ChartAreas[0].AxisY.LabelStyle = Chart2_Labelstyle_Linear();

            // Contribution?  Instead of fucking off to the front room like she
            // normally does, she has chosen to have the telly on at full blast
            // right next me ..

            // This fucking bitch NEVER shuts the fuck up ...

            //ObservableCollection<SmartUtility.DReadingsCharts> d_readings_charts = Chart2Code(utilityviewmodel,
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

            //    ObservableCollection<KeyValuePair<DateTime, double>> chart2_reading_points = new ObservableCollection<KeyValuePair<DateTime, double>>();

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
            // UWP OxyPlot.Core 2.0
            // UWP OxyPlot.Windows 1.0
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
#if WPF || WINFORMS || ANDROIDY
            Legend legend = new Legend();

            if (financeviewmodel.BanksChecked)
            {
                legend.LegendTitle += "Banks";
            }
            if (financeviewmodel.SavingsChecked)
            {
                if (!string.IsNullOrEmpty(legend.LegendTitle))
                {
                    legend.LegendTitle += "/";
                }
                legend.LegendTitle += "Savings";
            }
            if (financeviewmodel.InvestmentsChecked)
            {
                if (!string.IsNullOrEmpty(legend.LegendTitle))
                {
                    legend.LegendTitle += "/";
                }
                legend.LegendTitle += "Investments";
            }
#endif
            BarSeries chart2_debits = new BarSeries
            {
                Title = "Debits",
                StrokeColor = OxyPlot.OxyColors.Black,
                FillColor = OxyPlot.OxyColors.Pink,
                StrokeThickness = 1,
                XAxisKey = "Value",
                YAxisKey = "Date",
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}"
            };
            BarSeries chart2_credits = new BarSeries
            {
                Title = "Credits",
                StrokeColor = OxyPlot.OxyColors.Black,
                FillColor = OxyPlot.OxyColors.LightGreen,
                StrokeThickness = 1,
                XAxisKey = "Value",
                YAxisKey = "Date",
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}"
            };
            //#if UWP
            //            ColumnSeries chart2_points = new ColumnSeries
            //
            //            { 
            //                XAxisKey = "Date", 
            //                YAxisKey = "Value" 
            //            };
            //#endif
#if ANDROID
            BarSeries chart2_points = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Date"
            };
#endif

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

            ObservableCollection<SmartFinance.DebitsCreditsByMonth> d_debitscredits_charts = Chart2Code(financeviewmodel);
            if (d_debitscredits_charts.Count > 0)
            {
                // Fix line colour here before we set the points
                // Set the column colour to Red (DOESN'T WORK)
                foreach (SmartFinance.DebitsCreditsByMonth d_debitscredits_charts_row in d_debitscredits_charts)
                {
#if WINFORMS
                    // YCMTSU = You COULDN'T make this shit up, you really just couldn't ..
                    BarItem valueitemDebits = new BarItem
#endif
#if WPF
                    BarItem valueitemDebits = new BarItem
#endif
#if UWP
                    BarItem valueitemDebits = new BarItem
#endif
#if ANDROID
                    BarItem valueitemDebits = new BarItem
#endif
                    {
                        Value = Convert.ToDouble(d_debitscredits_charts_row.DEBITS / 100.0),
                    };
                    chart2_debits.Items.Add(valueitemDebits);
                    BarItem valueitemCredits = new BarItem
                    {
                        Value = Convert.ToDouble(d_debitscredits_charts_row.CREDITS / 100.0),

                    };
                    chart2_credits.Items.Add(valueitemCredits);
                    chart2_categoryAxis.Labels.Add(d_debitscredits_charts_row.PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat, financeviewmodel.financeDisplayCulture));
                }
            }
            chart2.Series.Add(chart2_debits);
            chart2.Series.Add(chart2_credits);
            chart2.Axes.Add(chart2_categoryAxis);
#if !(UWP || ANDROID)
            chart2.Legends.Add(legend);
#endif
            return;
        }

        internal static ObservableCollection<SmartFinance.DebitsCreditsByMonth> Chart2Code(FinanceViewModel financeviewmodel)
        {
            // These may be Quarterly or Monthly ... not yet!!
            ObservableCollection<SmartFinance.DebitsCreditsByMonth> totals_found = new ObservableCollection<SmartFinance.DebitsCreditsByMonth>();

            int debits_total = 0;
            int credits_total = 0;

            DateTime last_booking_date = SmartParametersV2016.defaultDate;// SmartTimeV2016.ConvertDateTime("1900-01-01");
            foreach (SmartFinance.TransactionsView transactionView in financeviewmodel.FinanceTransactions)
            {
                DateTime rays = Convert.ToDateTime(transactionView.BOOKING_DATE, financeviewmodel.financeDisplayCulture);
                // Force it to be Day 1
                DateTime this_booking_date = new DateTime(rays.Year, rays.Month, 1);
                if (DateTime.Compare(this_booking_date, last_booking_date) < 0)
                {
                    SmartFinance.DebitsCreditsByMonth monthlyTotal =
                        new SmartFinance.DebitsCreditsByMonth(

                        last_booking_date.AddDays(-1).AddMonths(1),
                        debits_total,
                        credits_total
                    );
                    totals_found.Add(monthlyTotal);
                    debits_total = 0;
                    credits_total = 0;
                }
                if (!transactionView.CREDITDEBIT_INDICATOR)
                {
                    debits_total += Math.Abs(transactionView.AMOUNT);
                }
                else
                {
                    credits_total += transactionView.AMOUNT;
                }
                last_booking_date = this_booking_date;
            }
            // Don't forget last one!! I won't ... here it is ...
            SmartFinance.DebitsCreditsByMonth finalTotal = new SmartFinance.DebitsCreditsByMonth(

                last_booking_date.AddDays(-1).AddMonths(1),
                debits_total,
                credits_total
            );
            totals_found.Add(finalTotal);
            // Try and get them in ascending date order?
            totals_found.Sort();
            return totals_found;
        }





        // This won't be here in Production, but the alternatives are horrendous
#if !PRODUCTION
        internal static DateTime Convert_Date(FinanceViewModel financeviewmodel,
                                                string date_string,
                                                DateTime defaultDate)
        {
            DateTime date = defaultDate;
            // Is there a time part?
            try
            {
                // Yes or no - use this
                date = SmartRoutinesV2018.DateTimeParseCulture(date_string, SmartParametersV2016.defaultCulture);
            }
            catch (ArgumentNullException exception)
            {
                // If it already contains something, then don't overwrite it so we can
                // always track/trap the first error
                if (string.IsNullOrEmpty(financeviewmodel.errorMessage))
                {
                    financeviewmodel.errorMessage = exception.Message;
                }
            }
            catch (FormatException exception)
            {
                // If it already contains something, then don't overwrite it so we can
                // always track/trap the first error
                if (string.IsNullOrEmpty(financeviewmodel.errorMessage))
                {
                    financeviewmodel.errorMessage = exception.Message;
                }
            }
            catch (ArgumentException exception)
            {
                // If it already contains something, then don't overwrite it so we can
                // always track/trap the first error
                if (string.IsNullOrEmpty(financeviewmodel.errorMessage))
                {
                    financeviewmodel.errorMessage = exception.Message;
                }
            }
            return date;
        }
#endif

        //        internal static void Force_Start_Of_Month(FinanceViewModel financeviewmodel,
        //                                                    DateTime defaultDate)
        //        {
        //            string start_temp;
        //            string urgent_message = string.Empty;
        //            start_temp = financeviewmodel.chartsstartDate.ToString(SmartParametersV2016.defaultCulture); // Otherwise next line doesn't work!!
        //            start_temp = "01/" + start_temp.Substring(3);
        //#if PRODUCTION
        //            financeviewmodel.chartsstartDate = SmartNibbyV2016.Convert_Date(financeviewmodel, start_temp, defaultDate, rf  urgent_message);
        //#else
        //            financeviewmodel.chartsstartDate = Convert_Date(financeviewmodel, start_temp, defaultDate);
        //#endif
        //            return;
        //        }

        //        internal static void Force_End_Of_Month(FinanceViewModel financeviewmodel,
        //                                                DateTime defaultDate)
        //        {
        //            string start_temp;
        //            string urgent_message = string.Empty;
        //            financeviewmodel.chartsstartDate = financeviewmodel.chartsstartDate.AddMonths(1);
        //            start_temp = financeviewmodel.chartsstartDate.ToString(SmartParametersV2016.defaultCulture); // Otherwise next line doesn't work!!
        //            start_temp = "01/" + start_temp.Substring(3);
        //#if PRODUCTION
        //            financeviewmodel.chartsstartDate = SmartNibbyV2016.Convert_Date(financeviewmodel, start_temp, defaultDate);
        //#else
        //            financeviewmodel.chartsstartDate = Convert_Date(financeviewmodel, start_temp, defaultDate);
        //#endif
        //            financeviewmodel.chartsstartDate = financeviewmodel.chartsstartDate.AddDays(-1);
        //            return;
        //        }

        //internal static void Choose(FinanceViewModel financeviewmodel,
        //                            int record_count,
        //                            double units_used)
        //{
        //    if (units_used > financeviewmodel.chartsmax || record_count == 0)
        //    {
        //        financeviewmodel.chartsmax = units_used;
        //    }
        //    if (units_used < financeviewmodel.chartsmin || record_count == 0)
        //    {
        //        financeviewmodel.chartsmin = units_used;
        //        financeviewmodel.chartsx_value = record_count + 1;
        //    }
        //}

        // Churning, churning, churning ... huffing, puffing, churning .. sighing, churning
        // Add Axis labels
#if WINFORMS
        //internal static LabelStyle Chart1_Labelstyle_Datetime()
        //{
        //    LabelStyle chart1_labelstyle_datetime = new LabelStyle()
        //    {
        //        IntervalType = new DateTimeIntervalType(),
        //        Format = "{0:MMM-yyyy}"
        //    };
        //    return chart1_labelstyle_datetime;
        //}
#endif

#if WINFORMS
        //internal static LabelStyle Chart1_Labelstyle_Linear()
        //{
        //    LabelStyle chart1_labelstyle_linear = new LabelStyle()
        //    {
        //        Format = "{0:C}"
        //    };
        //    return chart1_labelstyle_linear;
        //}
#endif
        internal static void Setup_Chart_Line(
#if WINFORMS || WPF || UWP || ANDROID
                                            OxyColor background,
                                            LineSeries lineseries,
#endif
                                            int series_no,
                                            string legend)
        {

#if WINFORMS
            //Series lineseries0 = new Series()
            //{
            //    ChartType = SeriesChartType.Line
            //};

            //// Do the Colour
            //lineseries0.Color = background;

            //// Add the Series into the Chart
            //chart1.Series.Add(lineseries0);              


            //chart1.ChartAreas[0].AxisX.LabelStyle = Chart1_Labelstyle_Datetime();
            //chart1.ChartAreas[0].AxisY.LabelStyle = Chart1_Labelstyle_Linear();
            //// Add the Series into the Chart
            //Legend rays = new Legend()
            //{
            //    Title = legend
            //};
            //chart1.Legends.Add(rays); // This adds LegendItem[0]

#endif
            return;
        }

#if WINFORMS
        //internal static LabelStyle Chart3_Labelstyle_Datetime()
        //{
        //    LabelStyle chart3_labelstyle_datetime = new LabelStyle()
        //    {
        //        IntervalType = new DateTimeIntervalType(),
        //        Format = "{0:MMM-yyyy}"
        //    };
        //    return chart3_labelstyle_datetime;
        //}
#endif

#if WINFORMS
        //internal static LabelStyle Chart3_Labelstyle_Linear()
        //{
        //    LabelStyle chart3_labelstyle_linear = new LabelStyle()
        //    {
        //        Format = "{0:C}"
        //    };
        //    return chart3_labelstyle_linear;
        //}

#endif
    }
}