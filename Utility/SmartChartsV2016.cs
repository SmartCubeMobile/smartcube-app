#if WINFORMS
using System.Drawing;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Legends;
#endif

#if WPF
using System.Windows.Media;     // This is in PresentationCore.dll <= FUCKING OBVIOUSLY!! Microshit fucking wanking idiots
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
#endif

#if UWP || WINUI
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI;
#endif

// In NuGET use 'Data Visualization Toolkit' to find it

#if ANDROIDX
using Android.Graphics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
#endif

#if SMARTMAUI
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
#endif

namespace SmartCubeMobile
{
    public class SmartChartsV2016
    {
        // THIS ROUTINE isn't SLOW!!  It generates the points very quickly,
        // However Silverligh takes FOR AGES to display the charts with the points.

        // What a FUCKER this was to get clean of CA1506 errors ... an absolute FUCKER
        //
        internal static async Task<PlotModel> CreateLineSeries(string title,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            // Now we have (or we SHOULD fucking have after all this pain)
            // all the facts and figures we need in usage_table and tariffs_engine_table
            // So we need to add the discounts table to the usage table
            // and then sort then by ascending READ_DATE and ascending CODE 

            // Don't really need to do it this way ... except if you want
            //  a true historical list in date order.  All you need to do
            // is add up the engine table (anyway you like) and then add
            // up the discounts table and add the two together.

            // The templateSetters are done in MainMeter module becausewe never invoke
            // UtilityView EVER whilst we are building the displays (!!) and we needed
            // the Styles moved OUT of App.xaml (not a sensible place for them to be)

            // Chart
            int series_no = 0;

            string displayed_supplier_name,// = "",
                            displayed_tariff_name;// = "";

            utilityviewmodel.ChartsEngineFromDate = SmartParametersV2016.defaultDate;
            utilityviewmodel.ChartsEngineToDate = SmartParametersV2016.defaultDate;
            // Set Alignment and Docking of the Legend chart to the Default Chart Area.

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

            //labelstyle.Setters.Add(new Setter(DateTimeAxisLabel.RenderTransformProperty, new RotateTransform()
            //  { Angle = -40, CenterX = 40, CenterY = 30 }));

            List<SmartUtility.AnalysisCosts> analysis_costs_found = new List<SmartUtility.AnalysisCosts>();
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    analysis_costs_found = utilityviewmodel.Hezbollah.e_analysis_costsList;
                    break;
                case SmartParametersV2016.Gas:
                    analysis_costs_found = utilityviewmodel.Hezbollah.g_analysis_costsList;
                    break;
                case SmartParametersV2016.DualFuel:
                    analysis_costs_found = utilityviewmodel.Hezbollah.d_analysis_costsList;
                    break;
                default:
                    break;
            }

            int analysis_costs_count = analysis_costs_found.Count - 1;
            if (analysis_costs_count > 1)   // Otherwise we get duplicates on Series name
            {
                // Do this ONCE
                utilityviewmodel.ChartsMinimum = SmartParametersV2016.defaultDate;
                utilityviewmodel.ChartsMaximum = SmartParametersV2016.defaultDate;
                int tariff_engineList_count = Set_Which_Engine(utilityviewmodel,
                                                                utilityviewmodel.resource_code,
                                                                utilityviewmodel.e_engine_dates,
                                                                utilityviewmodel.g_engine_dates);
                if (tariff_engineList_count > 0)
                {
                    switch (utilityviewmodel.resource_code)
                    {
                        case SmartParametersV2016.Electricity:
                            utilityviewmodel.analysisCost[0] = 0;
                            break;
                        case SmartParametersV2016.Gas:
                            utilityviewmodel.analysisCost[1] = 0;
                            break;
                        default:
                            break;
                    }
                    // The Legend Style is now done in XAML
                    // To get the Legend in the middle
                    while (series_no < 3)
                    {
                        switch (series_no)
                        {
                            case 0:
                                // Cheapest
                                Series cheapest =
                                        await Compute_Points(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.area_code,
                                                            analysis_costs_found.First().BRAND_CODE,
                                                            analysis_costs_found.First().SUPPLIER_CODE,
                                                            analysis_costs_found.First().TARIFF_CODE,
                                                            Convert.ToChar(analysis_costs_found.First().PAYMENT_PLAN),
                                                            utilityviewmodel.resource_code,
                                                            utilityviewmodel.resource_type,
                                                            utilityviewmodel.charts_brand_code,
                                                            utilityviewmodel.charts_supplier_code,
                                                            utilityviewmodel.ChartsEngineFromDate,
                                                            utilityviewmodel.age,
                                                            utilityviewmodel.withdrawn_date,
                                                            //vatRatesList,
                                                            //exchangeRatesList,
                                                            utilityviewmodel.already_doneList,
                                                            OxyColors.Green);


                                displayed_supplier_name = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                analysis_costs_found.First().SUPPLIER_CODE,
                                                                                                analysis_costs_found.First().BRAND_CODE);
                                displayed_tariff_name = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                            utilityviewmodel.resource_code,
                                                                                            analysis_costs_found.First().SUPPLIER_CODE,
                                                                                            analysis_costs_found.First().BRAND_CODE,
                                                                                            analysis_costs_found.First().RESOURCE_TYPE,
                                                                                            analysis_costs_found.First().TARIFF_CODE);
                                // Analysis Costs for Feed in tariffs could be < 0.0M
                                cheapest.Title = displayed_supplier_name + "-" + displayed_tariff_name;
                                plotmodel.Series.Add(cheapest);
                                break;
                            case 1:
                                // Most expensive
                                Series dearest =
                                        await Compute_Points(ourviewmodel,
                                                            utilityviewmodel,
                                                            utilityviewmodel.area_code,
                                                            analysis_costs_found.Last().BRAND_CODE,
                                                            analysis_costs_found.Last().SUPPLIER_CODE,
                                                            analysis_costs_found.Last().TARIFF_CODE,
                                                            Convert.ToChar(analysis_costs_found.Last().PAYMENT_PLAN),
                                                            utilityviewmodel.resource_code,
                                                            utilityviewmodel.resource_type,
                                                            utilityviewmodel.charts_brand_code,
                                                            utilityviewmodel.charts_supplier_code,
                                                            utilityviewmodel.ChartsEngineFromDate,
                                                            utilityviewmodel.age,
                                                            utilityviewmodel.withdrawn_date,
                                                            //vatRatesList,
                                                            //exchangeRatesList,
                                                            utilityviewmodel.already_doneList,
                                                            OxyColors.Red);

                                displayed_supplier_name = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                analysis_costs_found.Last().SUPPLIER_CODE,
                                                                                                analysis_costs_found.Last().BRAND_CODE);

                                displayed_tariff_name = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                           utilityviewmodel.resource_code,
                                                                                           analysis_costs_found.Last().SUPPLIER_CODE,
                                                                                           analysis_costs_found.Last().BRAND_CODE,
                                                                                           analysis_costs_found.Last().RESOURCE_TYPE,
                                                                                           analysis_costs_found.Last().TARIFF_CODE);
                                // Analysis Costs Feed-in tariffs could be < 0
                                dearest.Title = displayed_supplier_name + "-" + displayed_tariff_name;
                                plotmodel.Series.Add(dearest);
                                break;
                            case 2:
                                List<SmartUtility.TariffMatrix> tariff_matrix_found = SmartSpikeUtilityV2017.Utility_TariffMatrixList(ourviewmodel,
                                                                                                                utilityviewmodel,
                                                                                                                utilityviewmodel.resource_code,
                                                                                                                utilityviewmodel.resource_type,
                                                                                                                utilityviewmodel.charts_brand_code,
                                                                                                                utilityviewmodel.tariff_code);
                                if (tariff_matrix_found.Count == 0)
                                {

                                    string parameter = SmartParametersV2016.Utility.ToString() + SmartParametersV2016.unitSeparator.ToString() +
                                                                                utilityviewmodel.resource_code.ToString() + SmartParametersV2016.unitSeparator +
                                                                                utilityviewmodel.resource_type + SmartParametersV2016.unitSeparator +
                                                                                utilityviewmodel.charts_supplier_code.ToString() + SmartParametersV2016.unitSeparator +
                                                                                utilityviewmodel.charts_brand_code.ToString() + SmartParametersV2016.unitSeparator +
                                                                                utilityviewmodel.tariff_code.ToString() + SmartParametersV2016.unitSeparator +
                                                                                utilityviewmodel.area_code.ToString() + SmartParametersV2016.unitSeparator;
                                    if (!await SmartNibbyV2016.MiserableFuckingCow(ourviewmodel,
                                                                                utilityviewmodel,
                                                                                SmartParametersV2016.Utility,
                                                                                SmartParametersV2016.SingleProcedure,
                                                                                SmartParametersV2016.SmartUtilitySchema.ToUpper(),
                                                                                "SINGLE_TARIFFS",   // Procedure name or wildcard
                                                                                parameter))

                                    {
                                        // Set analysis Costs to zero because we cannot find the 
                                        // current (probably withdrawn) Tariff
                                        utilityviewmodel.projectedCost = 0;
                                        // Give up on ours
                                        return plotmodel;
                                    }
                                }
                                // Ours - its the CONTINUOUS running commentary on every mundane aspect of her life which gets me ...
                                displayed_supplier_name = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                utilityviewmodel.charts_supplier_code,
                                                                                                utilityviewmodel.charts_brand_code);
                                displayed_tariff_name = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                            utilityviewmodel.resource_code,
                                                                                            utilityviewmodel.charts_supplier_code,
                                                                                            utilityviewmodel.charts_brand_code,
                                                                                            utilityviewmodel.resource_type,
                                                                                            utilityviewmodel.tariff_code);
                                // Stupid bitch  Maldives <= Malvinas!!!!

                                Series ours =
                                    await Compute_Points(ourviewmodel,
                                                        utilityviewmodel,
                                                        utilityviewmodel.area_code,
                                                        utilityviewmodel.charts_brand_code,
                                                        utilityviewmodel.charts_supplier_code,
                                                        utilityviewmodel.tariff_code,
                                                        utilityviewmodel.payment_plan,
                                                        utilityviewmodel.resource_code,
                                                        utilityviewmodel.resource_type,
                                                        utilityviewmodel.charts_brand_code,
                                                        utilityviewmodel.charts_supplier_code,
                                                        utilityviewmodel.ChartsEngineFromDate,
                                                        utilityviewmodel.age,
                                                        utilityviewmodel.withdrawn_date,
                                                        //vatRatesList,
                                                        //exchangeRatesList,
                                                        utilityviewmodel.already_doneList,
                                                        OxyColors.Blue);

                                ours.Title = displayed_supplier_name + "-" + displayed_tariff_name;

                                switch (utilityviewmodel.resource_code)
                                {
                                    case SmartParametersV2016.Electricity:
                                        utilityviewmodel.e_total_cost = utilityviewmodel.analysisCost[0];
                                        utilityviewmodel.projectedCost = utilityviewmodel.analysisCost[0];
                                        break;
                                    case SmartParametersV2016.Gas:
                                        utilityviewmodel.g_total_cost = utilityviewmodel.analysisCost[1];
                                        utilityviewmodel.projectedCost = utilityviewmodel.analysisCost[1];
                                        break;
                                    case SmartParametersV2016.DualFuel:
                                        utilityviewmodel.e_total_cost = utilityviewmodel.analysisCost[0];
                                        utilityviewmodel.g_total_cost = utilityviewmodel.analysisCost[1];
                                        utilityviewmodel.projectedCost = utilityviewmodel.e_total_cost +
                                                                            utilityviewmodel.g_total_cost;
                                        break;
                                    default:
                                        break;
                                }
                                plotmodel.Series.Add(ours);
                                break;

                            default:
                                break;
                        }
                        series_no++;
                    }
                }
            }

            Double minValue = DateTimeAxis.ToDouble(utilityviewmodel.ChartsEngineFromDate);
            Double maxValue = DateTimeAxis.ToDouble(utilityviewmodel.ChartsEngineToDate);

            CategoryAxis chart1_category_axis = new CategoryAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = "Date",
                Minimum = minValue,
                Maximum = maxValue,
                //StringFormat = "MMM-yyyy",
                Angle = 90 //,
                //IntervalLength = 10
            };
            //DateTime date = engine_from_date;
            //while (true)
            //{
            //    chart1_category_axis.Labels.Add(date.ToString(SmartParametersV2016.ddmmmyyyyFormat, utilityviewmodel.utilityDisplayCulture));
            //    date = date.AddMonths(1);
            //    if (date >= EngineToDate)
            //    { 
            //        break;
            //    }
            //}
            plotmodel.Axes.Add(chart1_category_axis);

            LinearAxis chart1_linear_axis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Cost £",
                Key = "Value"
            };
            plotmodel.Axes.Add(chart1_linear_axis);
            return plotmodel;
        }

        internal static int Set_Which_Engine(UtilityViewModel utilityviewmodel,
                                        char resource_code,
                                        DateTime[] e_engine_dates,
                                        DateTime[] g_engine_dates)
        {
            int tariff_engine_count = 0;
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    tariff_engine_count = utilityviewmodel.Hezbollah.e_tariff_engineList.Count;
                    utilityviewmodel.ChartsEngineFromDate = e_engine_dates[0];
                    utilityviewmodel.ChartsEngineToDate = e_engine_dates[1];
                    if (tariff_engine_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.Hezbollah.e_tariff_engineList.First().PERIOD_END;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.Hezbollah.e_tariff_engineList.Last().PERIOD_END;
                    }
                    break;
                case SmartParametersV2016.Gas:
                    tariff_engine_count = utilityviewmodel.Hezbollah.g_tariff_engineList.Count;
                    utilityviewmodel.ChartsEngineFromDate = g_engine_dates[0];
                    utilityviewmodel.ChartsEngineToDate = g_engine_dates[1];
                    if (tariff_engine_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.Hezbollah.g_tariff_engineList.First().PERIOD_END;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.Hezbollah.g_tariff_engineList.Last().PERIOD_END;
                    }
                    break;
                case SmartParametersV2016.DualFuel:
                    if (utilityviewmodel.Hezbollah.e_tariff_engineList.Count > utilityviewmodel.Hezbollah.g_tariff_engineList.Count)
                    {
                        tariff_engine_count = utilityviewmodel.Hezbollah.e_tariff_engineList.Count;
                        utilityviewmodel.ChartsEngineFromDate = e_engine_dates[0];
                        utilityviewmodel.ChartsEngineToDate = e_engine_dates[1];
                        if (tariff_engine_count > 0)
                        {
                            utilityviewmodel.ChartsMinimum = utilityviewmodel.Hezbollah.e_tariff_engineList.First().PERIOD_END;
                            utilityviewmodel.ChartsMaximum = utilityviewmodel.Hezbollah.e_tariff_engineList.Last().PERIOD_END;
                        }
                    }
                    else
                    {
                        tariff_engine_count = utilityviewmodel.Hezbollah.g_tariff_engineList.Count;
                        utilityviewmodel.ChartsEngineFromDate = g_engine_dates[0];
                        utilityviewmodel.ChartsEngineToDate = g_engine_dates[1];
                        if (tariff_engine_count > 0)
                        {
                            utilityviewmodel.ChartsMinimum = utilityviewmodel.Hezbollah.g_tariff_engineList.First().PERIOD_END;
                            utilityviewmodel.ChartsMaximum = utilityviewmodel.Hezbollah.g_tariff_engineList.Last().PERIOD_END;
                        }
                    }
                    break;
                default:
                    break;
            }
            return tariff_engine_count;
        }

        internal static async Task<LineSeries> Compute_Points(MainViewModel ourviewmodel,
                                                                            UtilityViewModel utilityviewmodel,
                                                                            short area_code,
                                                                            short comparison_brand_code, //displayed_BRAND_CODE,
                                                                            short comparison_supplier_code, //short displayed_SUPPLIER_CODE,    // Supplier we might choose
                                                                            int comparison_tariff_code, //int displayed_TARIFF_CODE,                      // Tariff we might choose (wildcard or non-wildcard)
                                                                            char comparison_payment_plan, //char displayed_PAYMENT_PLAN,                  // Payment Plan we might choose 
                                                                            char resource_code,
                                                                            string resource_type,
                                                                            short displayed_brand_code,
                                                                            short displayed_supplier_code,
                                                                            DateTime engine_from_date,
                                                                            int age,
                                                                            DateTime withdrawn_date,
                                                                            List<SmartUtility.AnalysisConditions> already_doneList,
                                                                            OxyColor background)
        {
            short target_supplier_code = 0;             // For debugging
            int target_tariff_code = 0;                 // For debugging
            char target_payment_plan = SmartParametersV2016.defaultChar;  // For debugging

            LineSeries lineseries = new LineSeries()
            {
                XAxisKey = "Date",
                YAxisKey = "Value"
            };
            // I cannot think with that moron's relentless verbal diahorrea
            if (await SmartAnalyzeV2016.Analyze_Costs(ourviewmodel,
                            utilityviewmodel,
                            false,                      // Will not delete fron Analysis Costs
                            area_code,
                            resource_code,
                            resource_type,              // One currently in the display
                            displayed_brand_code,       // Brand we are on now (wildcard or non-wildcard) The one in the GREEN BOX
                            displayed_supplier_code,    // Supplier are on now (wildcard or non-wildcard) The one in the GREEN BOX
                            comparison_brand_code,      // Comparison Brand we might choose
                            comparison_supplier_code,   // Comparison Supplier we might choose
                            comparison_tariff_code,     // Comparison Tariff we might choose
                            comparison_payment_plan,    // Comparison Payment Plan we might choose
                            target_supplier_code,
                            target_tariff_code,
                            target_payment_plan,
                            engine_from_date,
                            age,
                            withdrawn_date,
                            //vatRatesList,
                            //exchangeRatesList,
                            already_doneList))
            {
                int count = 0;
                double total_cost = 0;

                DateTime read_date,
                            last_date = SmartParametersV2016.defaultDate;
                // Do this over entire days i.e. Sum by days
                foreach (SmartUtility.TariffCosts tariff_costs_row in utilityviewmodel.Hezbollah.tariff_costsList)
                {
                    // Read date should always be greater than or equal to last date,
                    // but there is no test because Tariff_Costs table has been sorted
                    // on READ_DATE before it leaves Analyze ...
                    read_date = tariff_costs_row.PERIOD_END;
                    if ((SmartRoutinesV2018.DateTimeCompare(read_date, last_date) > 0) &&
                        (last_date != SmartParametersV2016.defaultDate))
                    {
                        if (count == 0 || ((count / 100) * 100 == count))
                        {
                            DataPoint xyz_item = new DataPoint(DateTimeAxis.ToDouble(last_date), Math.Round(total_cost / 100, 2));
                            lineseries.Points.Add(xyz_item);
                        }
                    }
                    total_cost += Convert.ToDouble(tariff_costs_row.FIGURE);
                    last_date = read_date;
                    count++;
                }
                // Always do the last one if it hasn't been done
                DataPoint abc_item = new DataPoint(DateTimeAxis.ToDouble(last_date), Math.Round(total_cost / 100, 2));
                lineseries.Points.Add(abc_item);
            }
            lineseries.Color = background;
            return lineseries;
        }

        internal static PlotModel CreateReadingsSeries(string title,

                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            // Chart2 is Readings - doesn't depend on expiration
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

            string displayed_tariff_name = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                     utilityviewmodel.resource_code,
                                                                                     utilityviewmodel.charts_supplier_code,
                                                                                     utilityviewmodel.charts_brand_code,
                                                                                     utilityviewmodel.resource_type,
                                                                                     utilityviewmodel.tariff_code);
            ChartTwo(ourviewmodel,
                    utilityviewmodel,
                    utilityviewmodel.resource_code,
                    displayed_tariff_name,
                    plotmodel);
            return plotmodel;
        }

        internal static void ChartTwo(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            string displayed_tariff_name,
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
#if WINFORMS
            //chart2.ChartAreas[0].AxisX.LabelStyle = Chart2_Labelstyle_Datetime();
            //chart2.ChartAreas[0].AxisY.LabelStyle = Chart2_Labelstyle_Linear();

            
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

            
            //                // Fit this in sometime
            //                // LabelPlacement = LabelPlacement.Inside,
            //                // LabelFormatString = "{0:.00}%"

            //chart2 = new PlotModel();

            // Create ColumnSeries!!
            // YCMTSU = You COULDN'T make this shit up, you really just couldn't ..                    
            // Winforms OxyPlot.Core 2.0                <= YCMTSU !!
            // Winforms OxyPlot.WindowsForms 2.0        <= YCMTSU !!
            // UWP || WINUI OxyPlot.Core 2.0
            // UWP || WINUI OxyPlot.Windows 1.0
#if WINFORMS || WPF
            Legend legend = new Legend();
#endif
            if (utilityviewmodel.ElectricityChecked)
            {
#if WINFORMS || WPF
                legend.LegendTitle += "Electricity";
#endif
#if UWP || WINUI
                chart2.Title = "Electricity";
#endif
#if ANDROIDX
                chart2.Title = "Electricity";
#endif
            }
            if (utilityviewmodel.GasChecked)
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
                }
#if WINFORMS || WPF
                legend.LegendTitle += "Gas";
#endif
#if UWP || WINUI
                chart2.Title += "Gas";
#endif
#if ANDROIDX
                chart2.Title += "Gas";
#endif
            }
#if WINFORMS
            BarSeries chart2_points = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Date",
                //StrokeColor = OxyPlot.OxyColors.Black,
                //FillColor = OxyPlot.OxyColors.LightGreen,
                StrokeThickness = 1,
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}"

            };
#endif
            // Create Column Series
            // Winforms OxyPlot.Core 2.1
            // Winforms OxyPlot.WindowsForms 2.1
            // WPF OxyPlot.Core 2.1
            // WPF OxyPlot.Wpf  2.1
#if WPF || UWP || WINUI
            BarSeries chart2_points = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Date"
            };
#endif
#if ANDROIDX
            ColumnSeries chart2_points = new ColumnSeries
            {
                XAxisKey = "Date",
                YAxisKey = "Value"
            };
#endif
#if SMARTMAUI
            BarSeries chart2_points = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Date"
            };
#endif

#if UWP || WINUI
            //            ColumnSeries chart2_points = new ColumnSeries
            //            {
            //                XAxisKey = "Date",
            //                YAxisKey = "Value"
            //            };
#endif
            // Add Axis labels
            // Specify key and position
            CategoryAxis chart2_categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Date",
                Key = "Date",
                StringFormat = "MMM-yyyy",
                Angle = 45
            };

            // Specify key and position
            LinearAxis chart2_LinearAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Value",
                Title = "kWh"
            };
            chart2.Axes.Add(chart2_LinearAxis);

            List<SmartUtility.DReadingsCharts> d_readings_charts = Chart2Code(utilityviewmodel,
                                                                                                resource_code);
            if (d_readings_charts.Count > 0)
            {
                // Fix line colour here before we set the points
                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        // Set the column colour to Orange (DOESN'T WORK)
                        chart2_points.FillColor = OxyColors.Orange;
                        break;
                    case SmartParametersV2016.Gas:
                        // Set the column colour to Blue
                        chart2_points.FillColor = OxyColors.Blue;
                        break;
                    case SmartParametersV2016.DualFuel:
                        chart2_points.FillColor = OxyColors.Magenta;
                        break;
                    default:
                        break;
                }
                foreach (SmartUtility.DReadingsCharts d_readings_charts_row in d_readings_charts)
                {
#if WINFORMS
                    // YCMTSU = You COULDN'T make this shit up, you really just couldn't ..
                    BarItem valueitem = new BarItem
#endif
#if WPF || UWP || WINUI
                    BarItem valueitem = new BarItem
#endif
#if ANDROIDX
                    ColumnItem valueitem = new ColumnItem
#endif
#if SMARTMAUI
                    BarItem valueitem = new BarItem
#endif
                    {
                        Value = Convert.ToDouble(d_readings_charts_row.UNITS_USED)
                    };
                    chart2_points.Items.Add(valueitem);
                    chart2_categoryAxis.Labels.Add(d_readings_charts_row.PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat, utilityviewmodel.CultureINF));
                }
            }
            chart2.Series.Add(chart2_points);
            chart2.Axes.Add(chart2_categoryAxis);
#if WINFORMS || WPF
            chart2.Legends.Add(legend);
#endif
            return;
        }

        internal static PlotModel CreateUsageSeries(string title,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)

        {
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

            string displayed_supplier_name = SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(ourviewmodel,
                                                                                                utilityviewmodel,
                                                                                                utilityviewmodel.charts_supplier_code,
                                                                                                utilityviewmodel.charts_brand_code);
            string displayed_tariff_name = SmartSpikeUtilityV2017.Utility_Lookup_TariffName(utilityviewmodel,
                                                                                            utilityviewmodel.resource_code,
                                                                                            utilityviewmodel.charts_supplier_code,
                                                                                            utilityviewmodel.charts_brand_code,
                                                                                            utilityviewmodel.resource_type,
                                                                                            utilityviewmodel.tariff_code);
            List<string> category = new List<string>();

            // Chart3 is Usage - also doesn't depend on expiration
            if ((utilityviewmodel.resource_code == SmartParametersV2016.Electricity &&
                utilityviewmodel.Hezbollah.e_usageList.Count == 0) ||
                (utilityviewmodel.resource_code == SmartParametersV2016.Gas &&
                utilityviewmodel.Hezbollah.g_usageList.Count == 0))
            {
                // Chart 3A
                // No REAL usage figures, then mock-up by interpolating from the Readings
                //
                // We are going to dump this MS Chart shit and use Zedgraph ..
                //
                ChartThreeA(ourviewmodel,
                            utilityviewmodel,
                            utilityviewmodel.resource_code,
                            displayed_tariff_name,
                            plotmodel,
                            category);
            }
            else
            {
                // Chart 3
                // Use REAL usage figures
                //
                // One day We are going to dump this MS Chart shit and use Zedgraph ..
                // No!  One day WE DID dump this MS Chart shit and used OXYPLOT shit!!!
                //
                ChartThreeB(ourviewmodel,
                            utilityviewmodel,
                            utilityviewmodel.resource_code,
                            displayed_supplier_name,
                            displayed_tariff_name,
                            plotmodel,
                            category);

            }
            if (utilityviewmodel.UsageSeriesData.Count == 0)
            {
                return plotmodel;
            }
            else
            {
                DateTime first = utilityviewmodel.UsageSeriesData.First().Key.Date;
                DateTime last = utilityviewmodel.UsageSeriesData.Last().Key.Date;

                double minValue = DateTimeAxis.ToDouble(first);
                double maxValue = DateTimeAxis.ToDouble(last);
                CategoryAxis chart3_category_axis = new CategoryAxis()
                {
                    Position = AxisPosition.Bottom,
                    Title = "Date",
                    Key = "Key",
                    Minimum = minValue,
                    Maximum = maxValue,
                    StringFormat = "MMM-yyyy",
                    ItemsSource = category
                };
                plotmodel.Axes.Add(chart3_category_axis);
                LinearAxis chart3_linear_axis = new LinearAxis()
                {
                    Position = AxisPosition.Left,
                    Title = "kWh",
                    Key = "Value"
                };
                plotmodel.Axes.Add(chart3_linear_axis);
            }
            return plotmodel;
        }

        internal static List<SmartUtility.DReadingsCharts> Chart2Code(UtilityViewModel utilityviewmodel,
                                                                                    char resource_code)
        {
            // These may be Quarterly or Monthly ...
            utilityviewmodel.e_readings_found = new List<SmartUtility.EReadings>();
            utilityviewmodel.g_readings_found = new List<SmartUtility.GReadings>();
            utilityviewmodel.d_readings_found = new List<SmartUtility.EReadings>();

            List<SmartUtility.DReadingsCharts> d_readings_charts = new List<SmartUtility.DReadingsCharts>();

            int readings_rows_count = 0;
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:

                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                resource_code);
                    readings_rows_count += utilityviewmodel.e_readings_found.Count;
                    break;
                case SmartParametersV2016.Gas:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                resource_code);
                    readings_rows_count += utilityviewmodel.g_readings_found.Count;
                    break;
                case SmartParametersV2016.DualFuel:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                resource_code);
                    readings_rows_count += utilityviewmodel.d_readings_found.Count;
                    break;
                default:
                    break;
            }

            if (readings_rows_count > 0)
            {
                // Split the readings into Months (even thought they may already be in Months)

                // Series 0 is Monthly or Quarterly Bills
                decimal units_used;
                bool average_usage = false;

                decimal residue;

                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        // Need to re-do this cos never add residue in
                        foreach (SmartUtility.EReadings e_readings_row in utilityviewmodel.e_readings_found)
                        {
                            short no_of_days = (short)e_readings_row.READINGS_PERIOD_END.Subtract(e_readings_row.READINGS_PERIOD_START).Days;

                            units_used = e_readings_row.D_UNITS_USED + e_readings_row.N_UNITS_USED;
                            if (no_of_days == 0)
                            {
                                residue = units_used;
                            }
                            else
                            {
                                if (average_usage)
                                {
                                    if (no_of_days > 0)
                                    {
                                        units_used = Math.Round(units_used / no_of_days, 2);
                                    }
                                    else
                                    {
                                        units_used = 0;
                                    }
                                }
                                d_readings_charts.Add(new SmartUtility.DReadingsCharts(e_readings_row.READINGS_PERIOD_END, units_used, e_readings_row.METER_SERIAL_NO));
                            }
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        foreach (SmartUtility.GReadings g_readings_row in utilityviewmodel.g_readings_found)
                        {
                            short no_of_days = (short)g_readings_row.READINGS_PERIOD_END.Subtract(g_readings_row.READINGS_PERIOD_START).Days;

                            units_used = g_readings_row.D_UNITS_USED_KWH;
                            if (no_of_days == 0)
                            {
                                residue = units_used;
                            }
                            else
                            {
                                if (average_usage)
                                {
                                    if (no_of_days > 0)
                                    {
                                        units_used = Math.Round(units_used / no_of_days, 2);
                                    }
                                    else
                                    {
                                        units_used = 0;
                                    }
                                }
                                d_readings_charts.Add(new SmartUtility.DReadingsCharts(g_readings_row.READINGS_PERIOD_END, units_used, g_readings_row.METER_SERIAL_NO));
                            }
                        }
                        break;
                    case SmartParametersV2016.DualFuel:
                        // Need to re-do this cos never add residue in
                        //residue;// = 0.0M;
                        foreach (SmartUtility.EReadings d_readings_row in utilityviewmodel.d_readings_found)
                        {
                            short no_of_days = (short)d_readings_row.READINGS_PERIOD_END.Subtract(d_readings_row.READINGS_PERIOD_START).Days;

                            units_used = d_readings_row.D_UNITS_USED + d_readings_row.N_UNITS_USED;
                            if (no_of_days == 0)
                            {
                                residue = units_used;
                            }
                            else
                            {
                                if (average_usage)
                                {
                                    if (no_of_days > 0)
                                    {
                                        units_used = Math.Round(units_used / no_of_days, 2);
                                    }
                                    else
                                    {
                                        units_used = 0;
                                    }
                                }
                                d_readings_charts.Add(new SmartUtility.DReadingsCharts(d_readings_row.READINGS_PERIOD_END, units_used, d_readings_row.METER_SERIAL_NO));
                            }
                        }
                        break;
                    default:
                        break;
                }
                // Sort just in case we have a 'DualFuel' // M45 6UF 28
                d_readings_charts.Sort();
            }
            return d_readings_charts;
        }

        // For when we have to use the Readings
        internal static List<SmartUtility.DReadingsCharts> Chart3A_Code(UtilityViewModel utilityviewmodel,
                                                            char resource_code)
        //rf double max,    // For sine wave fitting
        //rf double min,    // For sine wave fitting
        //rf int x_value)
        //PlotModel chart3,
        //List<string> category)
        {
            // These may be Quarterly or Monthly ...
            utilityviewmodel.e_readings_found = new List<SmartUtility.EReadings>();
            utilityviewmodel.g_readings_found = new List<SmartUtility.GReadings>();
            utilityviewmodel.d_readings_found = new List<SmartUtility.EReadings>();

            // Get ready for the final output - do some accumulation
            List<SmartUtility.DReadingsCharts> d_readings_charts = new List<SmartUtility.DReadingsCharts>();

            int readings_rows_count = 0;
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                    resource_code);
                    readings_rows_count += utilityviewmodel.e_readings_found.Count;
                    break;
                case SmartParametersV2016.Gas:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                    resource_code);
                    readings_rows_count += utilityviewmodel.g_readings_found.Count;
                    break;
                case SmartParametersV2016.DualFuel:
                    SmartSpikeUtilityV2017.Utility_Find_Readings(utilityviewmodel,
                                                    resource_code);
                    readings_rows_count += utilityviewmodel.d_readings_found.Count;
                    break;
                default:
                    break;
            }

            if (readings_rows_count > 0)
            {
                // Split the readings into Months (even thought they may already be in Months)
                // Series 0 is Monthly or Quarterly Bills

                // Format the readings so that we can approximate a sine wave
                List<SmartUtility.DReadingsCharts> d_readings_temp = new List<SmartUtility.DReadingsCharts>();
                DateTime start_date = SmartParametersV2016.defaultDate;

                decimal total_usage = 0;
                DateTime match_date = SmartParametersV2016.defaultDate,
                         this_date = SmartParametersV2016.defaultDate;
                int months_count = 0;
                int record_count = 0;

                short days_count = 0;

                string this_meter_serial_no = "";

                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        foreach (SmartUtility.EReadings e_readings_row in utilityviewmodel.e_readings_found)
                        {
                            start_date = e_readings_row.READINGS_PERIOD_START;
                            // Don't forget the night units ...
                            short no_of_days = (short)e_readings_row.READINGS_PERIOD_END.Subtract(e_readings_row.READINGS_PERIOD_START).Days;
                            if (no_of_days > 0)
                            {
                                decimal daily_usage = (e_readings_row.D_UNITS_USED + e_readings_row.N_UNITS_USED) / no_of_days;
                                while (SmartRoutinesV2018.DateTimeCompare(start_date, e_readings_row.READINGS_PERIOD_END) <= 0)
                                {
                                    // Leap of faith!
                                    d_readings_temp.Add(new SmartUtility.DReadingsCharts(start_date, daily_usage, e_readings_row.METER_SERIAL_NO));
                                    start_date = start_date.AddDays(1);
                                    
                                }
                            }
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        foreach (SmartUtility.GReadings g_readings_row in utilityviewmodel.g_readings_found)
                        {
                            start_date = g_readings_row.READINGS_PERIOD_START;
                            short no_of_days = (short)g_readings_row.READINGS_PERIOD_END.Subtract(g_readings_row.READINGS_PERIOD_START).Days;
                            if (no_of_days > 0)
                            {
                                decimal daily_usage = g_readings_row.D_UNITS_USED_KWH;
                                //SmartEngineV2016.convert_to_kwh(g_readings_row.UNIT_OF_MEASURE,
                                //                                                            g_readings_row.D_UNITS_USED_M3,
                                //                                                            VOLUMECORRECTION,
                                //                                                            g_readings_row.CALORIFIC_VALUE,
                                //                                                            KWHCONVERSION);
                                daily_usage /= no_of_days;
                                while (SmartRoutinesV2018.DateTimeCompare(start_date, g_readings_row.READINGS_PERIOD_END) <= 0)
                                {
                                    d_readings_temp.Add(new SmartUtility.DReadingsCharts(start_date, daily_usage, g_readings_row.METER_SERIAL_NO));
                                    start_date = start_date.AddDays(1);
                                }
                            }
                        }
                        break;
                    case SmartParametersV2016.DualFuel:
                        foreach (SmartUtility.EReadings d_readings_row in utilityviewmodel.d_readings_found)
                        {
                            start_date = d_readings_row.READINGS_PERIOD_START;
                            // Don't forget the night units ...
                            short no_of_days = (short)d_readings_row.READINGS_PERIOD_END.Subtract(d_readings_row.READINGS_PERIOD_START).Days;
                            if (no_of_days > 0)
                            {
                                decimal daily_usage = (d_readings_row.D_UNITS_USED + d_readings_row.N_UNITS_USED) / no_of_days;
                                while (SmartRoutinesV2018.DateTimeCompare(start_date, d_readings_row.READINGS_PERIOD_END) <= 0)
                                {
                                    // Leap of faith!
                                    d_readings_temp.Add(new SmartUtility.DReadingsCharts(start_date, daily_usage, d_readings_row.METER_SERIAL_NO));
                                    start_date = start_date.AddDays(1);
                                    
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                // Should get all 'E's and 'G's in ascending sort order?
                d_readings_temp.Sort();
                // Find the start date - there should be at least ONE entry
                if (d_readings_temp.Count > 0)
                {
                    start_date = d_readings_temp.First().PERIOD_END;
                }
                // Force the start to be the end of the month
                // I have NO IDEA WHY ... but it seems to look better this way
                Force_Start_Of_Month(utilityviewmodel,
                                        //rf start_date,
                                        SmartParametersV2016.defaultDate);

                foreach (SmartUtility.DReadingsCharts d_readings_temp_row in d_readings_temp)
                {
                    if (days_count == 0)
                    {
                        months_count++;
                        start_date = start_date.AddMonths(months_count);
                        match_date = start_date;
                        Force_End_Of_Month(utilityviewmodel,
                                            //rf match_date, 
                                            SmartParametersV2016.defaultDate);
                        // As usual she is ==>  SHOUTING ALL THE TIME DOWN THE PHONE <===
                        // I simply cannot think when that bitch is in the room, she
                        // is SO NOISY and DISTRACTING
                    }
                    if (SmartRoutinesV2018.DateTimeCompare(d_readings_temp_row.PERIOD_END, match_date) == 0)
                    {
                        // Write out the total for the first of the month
                        d_readings_charts.Add(new SmartUtility.DReadingsCharts(this_date, total_usage, d_readings_temp_row.METER_SERIAL_NO));
                        // For the Sine Wave later on
                        Choose(utilityviewmodel, record_count, (double)total_usage);//, rf max, rf min, rf x_value);
                        record_count++;
                        // Clear these down
                        total_usage = days_count = 0;
                        months_count++;
                        match_date = start_date.AddMonths(months_count);
                    }
                    this_date = d_readings_temp_row.PERIOD_END;
                    this_meter_serial_no = d_readings_temp_row.METER_SERIAL_NO;
                    total_usage += d_readings_temp_row.UNITS_USED;
                    days_count = (short)(days_count + 1);
                }
                if (days_count > 0)
                {
                    // Do the last
                    d_readings_charts.Add(new SmartUtility.DReadingsCharts(this_date, total_usage, this_meter_serial_no));
                    Choose(utilityviewmodel, record_count, (double)total_usage);// rf max, rf min, rf x_value);
                }
            }
            return d_readings_charts;
        }

        // For when we have real Usage data
        //        internal static List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> Chart3B_Code(UtilityViewModel utilityviewmodel,
        //                                                                        char resource_code)
        //                                                                        //rf DateTime max,    // For sine wave fitting
        //                                                                        //rf DateTime min,    // For sine wave fitting
        //#if WINFORMS || ANDROID 
        //                                                                        //rf Color background)
        //#endif
        //#if WPF || UWP || WINUI
        //                                                                        //rf Color background)
        //#endif
        //        {
        //            List<SmartUtility.EUsageView> e_usage_view_found = new List<SmartUtility.EUsageView>();
        //            List<SmartUtility.GUsageView> g_usage_view_found = new List<SmartUtility.GUsageView>();
        //            List<SmartUtility.EUsageView> d_usage_view_found = new List<SmartUtility.EUsageView>();

        //            int usage_rows_count = 0;

        //            // Chart3B is Usage
        //            switch (resource_code)
        //            {
        //                case SmartParametersV2016.Electricity:
        //                    SmartSpikeUtilityV2017.Analyze_Charts(utilityviewmodel,
        //                                                resource_code,
        //                                                rf e_usage_view_found,
        //                                                rf g_usage_view_found,
        //                                                rf d_usage_view_found);
        //                    usage_rows_count = e_usage_view_found.Count;
        //                    if (usage_rows_count > 0)
        //                    {
        //                        min = e_usage_view_found.First().USAGE_DATETIME;
        //                        max = e_usage_view_found.Last().USAGE_DATETIME;
        //                    }

        //                    // Set the colour to Orange
        //                    //chart3_linestyle.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, "Orange"));
        //#if WINFORMS || ANDROID 
        //                    background = Color.Orange;
        //#endif
        //#if WPF || UWP || WINUI
        //                    background = Colors.Orange;
        //#endif
        //                    break;
        //                case SmartParametersV2016.Gas:
        //                    SmartSpikeUtilityV2017.Analyze_Charts(utilityviewmodel,
        //                                                resource_code,
        //                                                rf e_usage_view_found,
        //                                                rf g_usage_view_found,
        //                                                rf d_usage_view_found);
        //                    usage_rows_count = g_usage_view_found.Count;
        //                    if (usage_rows_count > 0)
        //                    {
        //                        min = g_usage_view_found.First().USAGE_DATETIME;
        //                        max = g_usage_view_found.Last().USAGE_DATETIME;
        //                    }
        //                    // Set the colour to Blue
        //                    //chart3_linestyle.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, "Blue"));
        //#if WINFORMS || ANDROID 
        //                    background = Color.Blue;
        //#endif
        //#if WPF || UWP || WINUI
        //                    background = Colors.Blue;
        //#endif
        //                    break;
        //                default:
        //                    SmartSpikeUtilityV2017.Analyze_Charts(utilityviewmodel,
        //                                                resource_code,
        //                                                rf e_usage_view_found,
        //                                                rf g_usage_view_found,
        //                                                rf d_usage_view_found);
        //                    usage_rows_count = d_usage_view_found.Count;
        //                    if (usage_rows_count > 0)
        //                    {
        //                        min = d_usage_view_found.First().USAGE_DATETIME;
        //                        max = d_usage_view_found.Last().USAGE_DATETIME;
        //                    }

        //                    // Set the colour to HotPink
        //                    //chart3_linestyle.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, "Orange"));
        //#if WINFORMS || ANDROID 
        //                    background = Color.HotPink;    //!!!!!
        //#endif
        //#if WPF || UWP || WINUI
        //                    background = Colors.HotPink;    //!!!!!
        //#endif
        //                    break;
        //            }

        
        //            List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> chart3_usage_points = new List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>>();
        //            if (usage_rows_count > 0)
        //            {
        //                // All this bollocks! Just to get the meter serial no on the tooktip ...
        //                switch (resource_code)
        //                {
        //                    case SmartParametersV2016.Electricity:
        //                        // We always lose (but need and use) the first entry in this set 
        //                        foreach (SmartUtility.EUsageView usage_view_row in e_usage_view_found)
        //                        {
        //                            KeyValuePair<decimal, string> amelia = new KeyValuePair<decimal, string>(usage_view_row.USAGE_TOTAL, usage_view_row.USAGE_VALUE + SmartParametersV2016.bar.ToString() + usage_view_row.METER_SERIAL_NO);
        //                            chart3_usage_points.Add(new KeyValuePair<DateTime, KeyValuePair<decimal, string>>(usage_view_row.USAGE_DATETIME, amelia));
        //                        }
        //                        break;
        //                    case SmartParametersV2016.Gas:
        //                        foreach (SmartUtility.GUsageView usage_view_row in g_usage_view_found)
        //                        {
        //                            KeyValuePair<decimal, string> amelia = new KeyValuePair<decimal, string>(usage_view_row.USAGE_TOTAL, usage_view_row.USAGE_VALUE + SmartParametersV2016.bar.ToString() + usage_view_row.METER_SERIAL_NO);
        //                            chart3_usage_points.Add(new KeyValuePair<DateTime, KeyValuePair<decimal, string>>(usage_view_row.USAGE_DATETIME, amelia));
        //                        }
        //                        break;
        //                    default:
        //                        // We always lose (but need and use) the first entry in this set 
        //                        foreach (SmartUtility.EUsageView usage_view_row in d_usage_view_found)
        //                        {
        //                            KeyValuePair<decimal, string> amelia = new KeyValuePair<decimal, string>(usage_view_row.USAGE_TOTAL, usage_view_row.USAGE_VALUE + SmartParametersV2016.bar.ToString() + usage_view_row.METER_SERIAL_NO);
        //                            chart3_usage_points.Add(new KeyValuePair<DateTime, KeyValuePair<decimal, string>>(usage_view_row.USAGE_DATETIME, amelia));
        //                        }
        //                        break;
        //                }
        //            }
        //            return chart3_usage_points;
        //        }

        // This won't be here in Production, but the alternatives are horrendous
        //#if Z!PRODUCTION
        internal static DateTime Convert_Date(UtilityViewModel utilityviewmodel,
                                                string date_string,
                                                DateTime defaultDate)
        //rf string urgent_message)
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
                if (string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                {
                    utilityviewmodel.errorMessage = exception.Message;
                }
            }
            catch (FormatException exception)
            {
                // If it already contains something, then don't overwrite it so we can
                // always track/trap the first error
                if (string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                {
                    utilityviewmodel.errorMessage = exception.Message;
                }
            }
            catch (ArgumentException exception)
            {
                // If it already contains something, then don't overwrite it so we can
                // always track/trap the first error
                if (string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                {
                    utilityviewmodel.errorMessage = exception.Message;
                }
            }
            return date;
        }
        //#endif

        internal static void Force_Start_Of_Month(UtilityViewModel utilityviewmodel,
                                            DateTime defaultDate)
        {
            string start_temp;
            //string urgent_message = "";
            start_temp = utilityviewmodel.ChartsStartDate.ToString(SmartParametersV2016.defaultCulture); // Otherwise next line doesn't work!!
            start_temp = "01/" + start_temp.Substring(3);
            //#if PRODUCTION
            //            utilityviewmodel.ChartsStartDate = SmartNibbyV2016.Convert_Date(utilityviewmodel, start_temp, defaultDate);
            //#Xelse
            utilityviewmodel.ChartsStartDate = Convert_Date(utilityviewmodel, start_temp, defaultDate);
            //#endif
            return;
        }

        internal static void Force_End_Of_Month(UtilityViewModel utilityviewmodel,
                                                DateTime defaultDate)
        {
            string start_temp;
            //string urgent_message = "";
            utilityviewmodel.ChartsStartDate = utilityviewmodel.ChartsStartDate.AddMonths(1);
            start_temp = utilityviewmodel.ChartsStartDate.ToString(SmartParametersV2016.defaultCulture); // Otherwise next line doesn't work!!
            start_temp = "01/" + start_temp.Substring(3);
            //#if PRODUCTION
            //            utilityviewmodel.ChartsStartDate = SmartNibbyV2016.Convert_Date(utilityviewmodel, start_temp, defaultDate);
            //#Xelse
            utilityviewmodel.ChartsStartDate = Convert_Date(utilityviewmodel, start_temp, defaultDate);
            //#endif
            utilityviewmodel.ChartsStartDate = utilityviewmodel.ChartsStartDate.AddDays(-1);
            return;
        }

        internal static void Choose(UtilityViewModel utilityviewmodel,
                                    int record_count,
                                    double units_used)
        //rf double max,
        //rf double min,
        //rf int x_value)
        {
            if (units_used > utilityviewmodel.chartsmax || record_count == 0)
            {
                utilityviewmodel.chartsmax = units_used;
            }
            if (units_used < utilityviewmodel.chartsmin || record_count == 0)
            {
                utilityviewmodel.chartsmin = units_used;
                utilityviewmodel.ChartsXValue = record_count + 1;
            }
        }

        internal static void Sine_Wave_Points(UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            List<SmartUtility.DReadingsCharts> d_usage_charts)
        {
            //int x_value = 0;  // For sine wave fitting
            //double max = 0;     // For sine wave fitting
            //double min = 0;    // For sine wave fitting


            //
            // Kris Dobratz http://www.youtube.com/watch?v=Q4z7CZD8bbI
            //
            // y = a * sin(w*x - %) + B
            // 1. Determine the amplitude (a)
            // How far high/low away from the midline
            double a = (utilityviewmodel.chartsmax - utilityviewmodel.chartsmin) / 2;
            // 2. Determine the midline (B)
            // also the up/down shift
            double B = (utilityviewmodel.chartsmax + utilityviewmodel.chartsmin) / 2;
            // Determine the period
            // How long before we get back to where we started? 12 months in our
            // case because we are essentially saying that:
            // Readings in Jan 2013 - more or less the same as Jan 2012, Jan 2011 etc.
            // Readings in Feb 2013 - more or less the same as Feb 2012, Feb 2011 etc.
            int period = 12;
            // and thuse determine w
            double w = (2 * Math.PI) / period;
            double phase_shift = 0;
            // Determine the phase shift % by plugging in the value for which y is the smallest
            // which in out case is (x_value, min) or (8, 90.0)
            // which gives us
            // 90.0 = 30.0 * sin(0.52*8 - %) + 120.0
            // (So now we only have ONE unknown)
            // Final equation:
            //  90.0 - 120.0 = 30.0 + sin(4.16 - %)
            // -30.0 = 30.0 * sin(4.16 - %)
            //  -1.0 = 1.0 * sin(4.16 - %)
            //  -1.0 = sin(4.16 - %)
            //  So ... sin of 'some value' equals -1 so that values must be 3pi/2
            // Now think of a circle describing values for sin... in radians.
            // When is sin('some value') = -1?  Only when 'some value' is 3 * 3.1412 /2
            // So what do we have to plug in for % to make it 3pi / 2
            // which means we have to solve:
            //   4.16 - % = 3pi / 2
            //   4.16 - 3pi / 2 = %
            // or for us:
            phase_shift = (w * utilityviewmodel.ChartsXValue) - (3 * Math.PI / 2);

            int local_x_value = utilityviewmodel.ChartsXValue;

            local_x_value = 1;
            // Get ready to store the points
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
#if WINFORMS
                    //utilityviewmodel.ChartsBackground = OxyColors.Orange;
#endif

#if WPF || UWP || WINUI
                    utilityviewmodel.ChartsBackground = OxyColors.Orange;
#endif
#if ANDROIDX
                    utilityviewmodel.ChartsBackground = OxyColors.Orange;
#endif
                    break;
                case SmartParametersV2016.Gas:
#if WINFORMS
                    //utilityviewmodel.ChartsBackground = OxyColors.Blue;
#endif

#if WPF || UWP || WINUI
                    utilityviewmodel.ChartsBackground = OxyColors.Blue;
#endif

#if ANDROIDX
                    utilityviewmodel.ChartsBackground = OxyColors.Blue;
#endif
                    break;
                case SmartParametersV2016.DualFuel:
#if WINFORMS
                    //utilityviewmodel.ChartsBackground = OxyColors.Magenta;
#endif

#if WPF || UWP || WINUI
                    utilityviewmodel.ChartsBackground = OxyColors.Magenta;
#endif
#if ANDROIDX
                    utilityviewmodel.ChartsBackground = OxyColors.Magenta;
#endif
                    break;
                default:
                    break;
            }
            foreach (SmartUtility.DReadingsCharts d_usage_charts_row in d_usage_charts)
            {
                double y = a * Math.Sin(w * local_x_value - phase_shift) + B;
                utilityviewmodel.SineWaveSeriesData.Add(new KeyValuePair<DateTime, double>(d_usage_charts_row.PERIOD_END, y));
                KeyValuePair<decimal, string> amelia =
                    new KeyValuePair<decimal, string>(d_usage_charts_row.UNITS_USED, String.Format("{0:0.00}", d_usage_charts_row.UNITS_USED) + SmartParametersV2016.bar.ToString() + d_usage_charts_row.METER_SERIAL_NO);
                utilityviewmodel.UsageSeriesData.Add(new KeyValuePair<DateTime, KeyValuePair<decimal, string>>(d_usage_charts_row.PERIOD_END, amelia));
                local_x_value++;
            }
            return;
        }

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
                                            OxyColor background,
                                            LineSeries lineseries,
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
            //chart1.lineseries.Color = background;


            //lineseries.XAxis.Title = "Time";
            //chart1.LegendTitle = legend;
            //chart1.Leg
            //((Legend)chart1.LegendItemOrder[series_no]).Content = legend;

            //#if WPF
            //            // Analysis Costs for Feed in tariffs could be < 0.0M
            //            LineSeries lineseries0 = new LineSeries
            //            {
            //                DependentValuePath = "Value",
            //                IndependentValuePath = "Key"
            //            };
            //            // Do the line Points and Colour

            //            // With thanks to Anders Gustafsson
            //            // https://stackoverflow.com/questions/16722542/performance-of-wpf-toolkit-line-chart
            //            Style linestyle0 = new Style(typeof(LineDataPoint));
            //            //NEED TO THINK ABOUT PUTTING THE LINE BELOW AS AN OPTION ON THE CHART
            //            // SO IT DRAWS FASTER INITIALLY AND THEN IF YOU WANT TO SEE THE VALUE AT EACH POINT
            //            // YOU CAN RE-DRAW IT ...

            //            linestyle0.Setters.Add(new Setter(templateSetter.Property, templateSetter.Value));

            //            // This next line is REDUNDANT and has NO EFFECT ... but it is EXACTLY
            //            // what we want to do i.e. format the Date for the Y-axiz values.
            //            // Just doesn't fucking work - because this entire Silverligh bollocks is just that.
            //            // Absolute fucking bollocks  Yes, but I have fixed it now!!!!
            //            linestyle0.Setters.Add(new Setter(LineDataPoint.IndependentValueStringFormatProperty, "{0:dd-MMM-yyyy}"));
            //            // This next line DOES work -
            //            linestyle0.Setters.Add(new Setter(LineDataPoint.DependentValueStringFormatProperty, "{0:C}"));
            //            // Do the Colour
            //            linestyle0.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(background)));

            //            lineseries0.DataPointStyle = linestyle0;
            //            // Add the Series into the Chart
            //            chart1.Series.Add(lineseries0);              // This adds LegendItem[0]

            //            // Now we can do the Legend
            //            ((LegendItem)chart1.LegendItems[series_no]).Content = legend;
            //#endif
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

        internal static void ChartThreeB(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        char resource_code,
                                        string displayed_supplier_name,
                                        string displayed_tariff_name,
                                        PlotModel chart3,
                                        List<string> category)
        {
#if WINFORMS
            //            utilityviewmodel.Chart3.ChartAreas[0].AxisX.LabelStyle = Chart3_Labelstyle_Datetime();
            //            utilityviewmodel.Chart3.ChartAreas[0].AxisY.LabelStyle = Chart3_Labelstyle_Linear();

            //            Series lineseries0 = new Series()
            //            {
            //                ChartType = SeriesChartType.Point
            //            };
            //            // Set the colour to Blue or Orange
            //            switch (resource_code)
            //            {
            //                case SmartParametersV2016.Electricity:
            //                    // Set the column colour to Orange (DOESN'T WORK)
            //                    lineseries0.Color = System.Drawing.Color.Orange;
            //                    break;
            //                case SmartParametersV2016.Gas:
            //                    // Set the column colour to Blue
            //                    lineseries0.Color = System.Drawing.Color.Blue;
            //                    break;
            //                default:
            //                    lineseries0.Color = System.Drawing.Color.Black;
            //                    break;
            //            }
            //            // Add the Series into the Chart
            //            utilityviewmodel.Chart3.Series.Add(lineseries0);              // This adds LegendItem[0]

            //            Color background = new Color();
            
            //            DateTime date_max = defaultDate;
            //            DateTime date_min = defaultDate;
            //            List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> chart3_points = Chart3B_Code(utilityviewmodel,
            //                                                                                resource_code);
            //                                                                                //rf date_max,
            //                                                                                //rf date_min,
            //                                                                                //rf background);
            //            utilityviewmodel.Chart3.ChartAreas[0].AxisX.Title = "Date";
            //            utilityviewmodel.Chart3.ChartAreas[0].AxisX.Minimum = date_min.ToOADate();
            //            utilityviewmodel.Chart3.ChartAreas[0].AxisX.Maximum = date_max.ToOADate();

            //            if (chart3_points.Count > 0)
            //            {
            //                utilityviewmodel.Chart3.Series[0].Points.DataBindXY(chart3_points, "Key", chart3_points, "Value");
            //            }
#endif

            //#if WPF || UWP || WINUI
            //            // Add Axis labels
            //            chart3.Axes.Clear();
            //            DateTimeAxis chart3_category_axis = new DateTimeAxis()
            //            {
            //                Orientation = AxisOrientation.X,
            //                Location = AxisLocation.Bottom,
            //                Title = "Date",
            //                AxisLabelStyle = Chart3_Labelstyle_Datetime(),
            //            };
            //            chart3.Axes.Add(chart3_category_axis);
            //            LinearAxis chart3_linear_axis = new LinearAxis()
            //            {
            //                Orientation = AxisOrientation.Y,
            //                Title = "kWh",
            //                AxisLabelStyle = Chart3_Labelstyle_Linear()
            //            };
            //            chart3.Axes.Add(chart3_linear_axis);

            //            //labelstyle.Setters.Add(new Setter(DateTimeAxisLabel.RenderTransformProperty, new RotateTransform()
            //            //                                                { Angle = -40, CenterX = 40, CenterY = 30 }));
            //            LineSeries chart3_lineseries = new LineSeries()
            //            {
            //                IndependentValuePath = "Key",
            //                DependentValuePath = "Value",
            //                IndependentValueBinding = new Binding("Key"),
            //                DependentValueBinding = new Binding("Value.Key") // <= because this a pair in itself
            //            };

            //            chart3.Series.Add(chart3_lineseries);              // This adds LegendItem[0]

            //            ((LegendItem)chart3.LegendItems[0]).Content = displayed_supplier_name + "-" + displayed_tariff_name;

            //            Color background = Colors.Transparent;
            
            //            DateTime max = defaultDate;
            //            DateTime min = defaultDate;

            //            utilityviewmodel.UsageSeriesData = Chart3B_Code(utilityviewmodel,
            //                                                                    resource_code);
            //                                                                    //rf max,
            //                                                                    //rf min,
            //                                                                    //rf background);
            //            chart3_category_axis.Maximum = max;
            //            chart3_category_axis.Minimum = min;

            //            if (utilityviewmodel.UsageSeriesData.Count > 0)
            //            {
            //                ((LineSeries)chart3.Series[0]).ItemsSource = utilityviewmodel.UsageSeriesData;
            //                ((LineSeries)chart3.Series[0]).DataPointStyle = Chart3_Labelstyle_Line(templateSetter, background);
            //            }
            //#endif

            // Add Axis labels
            //labelstyle.Setters.Add(new Setter(DateTimeAxisLabel.RenderTransformProperty, new RotateTransform()
            //                                                { Angle = -40, CenterX = 40, CenterY = 30 }));
            //((LegendItem)chart3.LegendItems[0]).Content = displayed_supplier_name + "-" + displayed_tariff_name;
#if WINFORMS
            //utilityviewmodel.ChartsBackground = OxyColors.Transparent;
#endif
#if WPF || UWP || WINUI
            utilityviewmodel.ChartsBackground = OxyColors.Transparent;
#endif
#if ANDROIDX
            utilityviewmodel.ChartsBackground = OxyColors.Transparent;
#endif
            
            utilityviewmodel.ChartsMinimum = SmartParametersV2016.defaultDate;
            utilityviewmodel.ChartsMaximum = SmartParametersV2016.defaultDate;
            LineSeries usage_lineseries = new LineSeries()
            {
                Title = "Usage",
                XAxisKey = "Key",
                YAxisKey = "Value"
            };

            utilityviewmodel.UsageSeriesData = Chart3B_Code(ourviewmodel,
                                                            utilityviewmodel,
                                                            category,
                                                            resource_code);
            //rf max,
            //rf min,
            //rf background);
            if (utilityviewmodel.UsageSeriesData.Count > 0)
            {
                foreach (KeyValuePair<DateTime, KeyValuePair<decimal, string>> d_readings_charts_row in utilityviewmodel.UsageSeriesData)
                {
                    DataPoint valueitem = new DataPoint(d_readings_charts_row.Key.ToOADate(), Convert.ToDouble(d_readings_charts_row.Value.Key));
                    usage_lineseries.Points.Add(valueitem);
                    //category.Add(d_readings_charts_row.Key.ToString(SmartParametersV2016.ddmmmyyyyFormat));
                }
                chart3.Series.Add(usage_lineseries);
                //((LineSeries)chart3.Series[0]).DataPointStyle = Chart3_Labelstyle_Line(templateSetter, background);
            }
            return;
        }

        //#if WPF || UWP || WINUI
        //        internal static Style Chart3a_Labelstyle_Datetime()
        //        {
        //            Style labelstyle_datetime = new Style(typeof(AxisLabel));
        //            labelstyle_datetime . Setters.Add(new Setter(AxisLabel.StringFormatProperty, "{0:MMM-yy}"));
        //            labelstyle_datetime . Setters.Add(new Setter(AxisLabel.RenderTransformProperty, new RotateTransform() { Angle = 60, CenterX = 17.5, CenterY = 17.5 }));
        //            labelstyle_datetime . Setters.Add(new Setter(AxisLabel.FontSizeProperty, 8.0));
        //            return labelstyle_datetime;
        //        }
        //#endif

        //#if WPF || UWP || WINUI
        //        internal static Style Chart3a_Labelstyle_Linear()
        //        {
        //            Style labelstyle_linear = new Style(typeof(NumericAxisLabel));
        //            Chart3_Labelstyle_Linear.Setters.Add(new Setter(NumericAxisLabel.StringFormatProperty, "#"));
        //            return labelstyle_linear;
        //        }
        //#endif

        //#if WPF || UWP || WINUI
        //        internal static Style Chart3a_Normalized_Style(Setter templateSetter,
        //                                                        Color background)
        //        {
        //            //Style normalized_style = new Style(typeof(LineDataPoint));  // To fix the line colour later
        //            Style normalized_style = new Style(typeof(LineDataPoint));
        //            normalized_style.Setters.Add(new Setter(templateSetter.Property, templateSetter.Value));
        //            normalized_style.Setters.Add(new Setter(LineDataPoint.IndependentValueStringFormatProperty, "{0:dd-MMM-yyyy}"));
        //            normalized_style.Setters.Add(new Setter(LineDataPoint.DependentValueStringFormatProperty, "{0:0.000} kWh"));
        //            normalized_style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(background)));
        //            return normalized_style;
        //        }
        //#endif

        //#if WPF || UWP || WINUI
        //        internal static Style Chart3a_Sinewave_Style()
        //        {
        //            //Style sinewave_style = new Style(typeof(LineDataPoint));  // To fix the line colour later
        //            // To fix the Tooltip now
        //            // I don't know why the tooltip still shows the y-value (but not the x-value) ??
        //            //chart3_linedatapoint_style.Setters.Add(new Setter(templateSetter[1].Property, templateSetter[1].Value));

        //            Style sinewave_style = new Style(typeof(LineDataPoint));
        //            sinewave_style.Setters.Add(new Setter(LineDataPoint.IndependentValueStringFormatProperty, "{0:dd-MMM-yyyy}"));
        //            sinewave_style.Setters.Add(new Setter(LineDataPoint.DependentValueStringFormatProperty, "{0:0.000} kWh"));

        //            sinewave_style.Setters.Add(new Setter(LineDataPoint.HeightProperty, 4.0));
        //            sinewave_style.Setters.Add(new Setter(LineDataPoint.WidthProperty, 4.0));
        //            sinewave_style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(Colors.Red)));
        //            return sinewave_style;
        //        }
        //#endif

        //#if WPF || UWP || WINUI
        //        internal static Style Chart3a_Polyline_Style()
        //        {
        //            Style polyline_style = new Style(typeof(Polyline));
        //            polyline_style.Setters.Add(new Setter(Polyline.StrokeDashArrayProperty, new DoubleCollection() { 2, 3, 2 }));// <= Don't ask me why .. it looks ok on the screen (!!)
        //            return polyline_style;
        //        }
        //#endif

#if WPF || UWP || WINUI
        internal static OxyPlot.LineStyle Chart3a_Polyline_Style()
        {
            //OxyPlot.LineStyle polyline_style = (typeof)(OxyPlot.LineStyle) as Style;
            //var rays = new XamarShit.Forms.Setter();
            //rays.Property = OxyPlot.;
            //polyline_style.Setters.Add(new Setter(new DoubleCollection() { 2, 3, 2 }));

            //rays.Property = polyline_style.Setters.;
            //rays.Value = new DoubleCollection() { 2, 3, 2 };
            //polyline_style.Setters.Add(new Setter(Polyline.StrokeDashArrayProperty, new DoubleCollection() { 2, 3, 2 }));// <= Don't ask me why .. it looks ok on the screen (!!)
            return new OxyPlot.LineStyle(); // polyline_style;
        }
#endif
#if ANDROIDX
        internal static OxyPlot.LineStyle Chart3a_Polyline_Style()
        {
            //OxyPlot.LineStyle polyline_style = (typeof)(OxyPlot.LineStyle) as Style;
            //var rays = new XamarShit.Forms.Setter();
            //rays.Property = OxyPlot.;
            //polyline_style.Setters.Add(new Setter(new DoubleCollection() { 2, 3, 2 }));

            //rays.Property = polyline_style.Setters.;
            //rays.Value = new DoubleCollection() { 2, 3, 2 };
            //polyline_style.Setters.Add(new Setter(Polyline.StrokeDashArrayProperty, new DoubleCollection() { 2, 3, 2 }));// <= Don't ask me why .. it looks ok on the screen (!!)
            return new OxyPlot.LineStyle(); // polyline_style;
        }
#endif

#if ANDROIDX
        //        internal static Style Chart3a_Polyline_Style()
        //        {
        //            Android.Graphics. .Style polyline_style = OxyPlot.LineStyle as PathDashPathEffect.Style.;
        //            //var rays = new XamarShit.Forms.Setter();
        //            //rays.Property = OxyPlot.;
        //            //polyline_style.Setters.Add(new Setter(new DoubleCollection() { 2, 3, 2 }));

        //            //rays.Property = polyline_style.Setters.;
        //            //rays.Value = new DoubleCollection() { 2, 3, 2 };
        //            //polyline_style.Setters.Add(new Setter(Polyline.StrokeDashArrayProperty, new DoubleCollection() { 2, 3, 2 }));// <= Don't ask me why .. it looks ok on the screen (!!)
        //            return polyline_style;
        //        }
#endif

        internal static void ChartThreeA(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            string displayed_tariff_name,
                                            PlotModel chart3,
                                            List<string> category)
        {
#if WINFORMS
            //            ChartArea area = new ChartArea();
            //            utilityviewmodel.Chart3.ChartAreas.Add(area);
            //#endif
            //#if WINFORMS
            //            utilityviewmodel.Chart3.Series.Clear();
            //#endif
            //#if WINFORMS
            //            utilityviewmodel.Chart3.ChartAreas[0].AxisX.LabelStyle = Chart3_Labelstyle_Datetime();
            //            utilityviewmodel.Chart3.ChartAreas[0].AxisY.LabelStyle = Chart3_Labelstyle_Linear();

            //            int x_value = 0;
            //            double max = 0,    // For sine wave fitting
            //                    min = 0;    // For sine wave fitting

            //            List<SmartUtility.DReadingsCharts> d_usage_charts = Chart3A_Code(ourviewmodel,
            //                                                                                    utilityviewmodel,
            //                                                                                    resource_code,
            //                                                                                    defaultDate);
            //                                                                                    //rf max,    // For sine wave fitting
            //                                                                                    //rf min,    // For sine wave fitting
            //                                                                                    //rf x_value);
            //            if (d_usage_charts.Count > 0)
            //            {
            //                // ... But we always display on a Monthly basis
            //                Series lineseries0 = new Series()
            //                {
            //                    ChartType = SeriesChartType.Line
            //                };
            //                switch (resource_code)
            //                {
            //                    case SmartParametersV2016.Electricity:
            //                        // Set the column colour to Orange (DOESN'T WORK)
            //                        lineseries0.Color = System.Drawing.Color.Orange;
            //                        break;
            //                    case SmartParametersV2016.Gas:
            //                        // Set the column colour to Blue
            //                        lineseries0.Color = System.Drawing.Color.Blue;
            //                        break;
            //                    default:
            //                        lineseries0.Color = System.Drawing.Color.Magenta;
            //                        break;
            //                }

            //                // Add the Series into the Chart
            //                utilityviewmodel.Chart3.Series.Add(lineseries0);              // This adds LegendItem[0]

            //                Series lineseries1 = new Series()
            //                {
            //                    ChartType = SeriesChartType.Point
            //                };
            //                utilityviewmodel.Chart3.Series.Add(lineseries1);


            //                // ... and YES it fucking works!!!  After ALL THIS TIME I can smooth
            //                // a graph to a sine wave !!!
            //                Color background = new Color();

            //                // Using this line below doesn't work so I'm going to go with 'double'
            //                // just for the sake of getting things going.  When I have PLENTY of time
            //                // - AND I'm rich - I'llcome back to it!!!!
            //                //utilityviewmodel.UsageSeriesData = new List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>>();

            //                utilityviewmodel.UsageSeriesData = new List<KeyValuePair<DateTime, double>>();
            //                utilityviewmodel.SineWaveSeriesData = new List<KeyValuePair<DateTime, double>>();

            //                Sine_Wave_Points(utilityviewmodel,
            //                                                    resource_code,
            //                                                    max,
            //                                                    min,
            //                                                    x_value,
            //                                                    //rf background,
            //                                                    d_usage_charts);

            //                utilityviewmodel.Chart3.Series[0].Points.DataBindXY(utilityviewmodel.UsageSeriesData, "Key", utilityviewmodel.UsageSeriesData, "Value");
            //                utilityviewmodel.Chart3.Series[1].Points.DataBindXY(utilityviewmodel.SineWaveSeriesData, "Key", utilityviewmodel.SineWaveSeriesData, "Value");
            //            }
#endif
            //#if WPF || UWP || WINUI
            
            //            // Add Axis labels
            //            chart3.Axes.Clear();

            //            CategoryAxis chart3_category_axis = new CategoryAxis()
            //            {
            //                Orientation = AxisOrientation.X,
            //                Location = AxisLocation.Bottom,
            //                Title = "Date",
            //                AxisLabelStyle = Chart3a_Labelstyle_Datetime(),
            //            };
            //            chart3.Axes.Add(chart3_category_axis);
            //            LinearAxis chart3_linear_axis = new LinearAxis()
            //            {
            //                Orientation = AxisOrientation.Y,
            //                Title = "kWh",
            //                AxisLabelStyle = Chart3a_Labelstyle_Linear()
            //            };
            //            chart3.Axes.Add(chart3_linear_axis);

            //            int x_value = 0;
            //            double max = 0,     // For sine wave fitting
            //                    min = 0;    // For sine wave fitting

            //            List<SmartUtility.DReadingsCharts> d_usage_charts = Chart3A_Code(ourviewmodel,
            //                                                                                utilityviewmodel,
            //                                                                                    resource_code,
            //                                                                                    defaultDate);
            //                                                                                    //rf max,    // For sine wave fitting
            //                                                                                    //rf min,    // For sine wave fitting
            //                                                                                    //rf x_value);
            //            if (d_usage_charts.Count > 0)
            //            {
            //                Color background = Colors.Transparent;

            //                // ... But we always display on a Monthly basis
            //                LineSeries usage_lineseries = new LineSeries()
            //                {
            //                    IndependentValuePath = "Key",
            //                    DependentValuePath = "Value.Key"

            //                };
            //                chart3.Series.Add(usage_lineseries);              // This adds LegendItem[0]            

            //                ((LegendItem)chart3.LegendItems[0]).Content = displayed_tariff_name + " - Monthly";
            //                ((LineSeries)chart3.Series[0]).DataPointStyle = Chart3a_Normalized_Style(templateSetter, background);

            //                // Now do the Sine Wave approximation
            //                LineSeries sinewave_lineseries = new LineSeries()
            //                {
            //                    IndependentValuePath = "Key",
            //                    DependentValuePath = "Value"
            //                };
            //                chart3.Series.Add(sinewave_lineseries);              // This adds LegendItem[0]            

            //                ((LegendItem)chart3.LegendItems[1]).Content = displayed_tariff_name + " - Monthly smoothed";

            //                // Fix line colour here before we set the points - Series 1 is the Sine Wave
            //                ((LineSeries)chart3.Series[1]).DataPointStyle = Chart3a_Sinewave_Style();
            //                ((LineSeries)chart3.Series[1]).PolylineStyle = Chart3a_Polyline_Style();

            //                // ... and YES it fucking works!!!  After ALL THIS TIME I can smooth
            //                // a graph to a sine wave !!! 

            //                utilityviewmodel.UsageSeriesData = new List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>>();
            //                utilityviewmodel.SineWaveSeriesData = new List<KeyValuePair<DateTime, double>>();

            //                // Setting the background here ... isn't it too late??
            //                Sine_Wave_Points(utilityviewmodel,
            //                                                    resource_code,
            //                                                    max,
            //                                                    min,
            //                                                    x_value,
            //                                                    //rf background,
            //                                                    d_usage_charts);
            //                ((LineSeries)chart3.Series[0]).ItemsSource = utilityviewmodel.UsageSeriesData;
            //                ((LineSeries)chart3.Series[1]).ItemsSource = utilityviewmodel.SineWaveSeriesData;
            //            }
            //#endif

            
            // Add Axis labels

            List<SmartUtility.DReadingsCharts> d_usage_charts = Chart3A_Code(utilityviewmodel,
                                                                                            resource_code);

            if (d_usage_charts.Count > 0)
            {
#if WINFORMS
                //utilityviewmodel.ChartsBackground = OxyColors.Transparent;
#endif
#if WPF || UWP || WINUI
                utilityviewmodel.ChartsBackground = OxyColors.Transparent;
#endif
#if ANDROIDX
                utilityviewmodel.ChartsBackground = OxyColors.Transparent;
#endif

                // ... But we always display on a Monthly basis
                LineSeries usage_lineseries = new LineSeries()
                {
                    Title = "Usage",
                    XAxisKey = "Key",
                    YAxisKey = "Value"
                };
                //chart3.Series.Add(usage_lineseries);              // This adds LegendItem[0]            

                //((LegendItem)chart3.LegendItems[0]).Content = displayed_tariff_name + " - Monthly";
                //((LineSeries)chart3.Series[0]).DataPointStyle = Chart3a_Normalized_Style(templateSetter, background);

                // Now do the Sine Wave approximation
                LineSeries sinewave_lineseries = new LineSeries()
                {
                    Title = "SineWave",
                    XAxisKey = "Key",
                    YAxisKey = "Value"
                };
                //chart3.Series.Add(sinewave_lineseries);              // This adds LegendItem[0]            

                //((LegendItem)chart3.LegendItems[1]).Content = displayed_tariff_name + " - Monthly smoothed";

                // Fix line colour here before we set the points - Series 1 is the Sine Wave
                //((LineSeries)chart3.Series[1]).DataPointStyle = Chart3a_Sinewave_Style();
                //((LineSeries)chart3.Series[1]).PolylineStyle = Chart3a_Polyline_Style();

                // ... and YES it fucking works!!!  After ALL THIS TIME I can smooth
                // a graph to a sine wave !!! 
                utilityviewmodel.UsageSeriesData = new List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>>();
                utilityviewmodel.SineWaveSeriesData = new List<KeyValuePair<DateTime, double>>();

                // Setting the background here ... isn't it too late??
                Sine_Wave_Points(utilityviewmodel,
                                    resource_code,
                                    d_usage_charts);
                foreach (KeyValuePair<DateTime, KeyValuePair<decimal, string>> d_readings_charts_row in utilityviewmodel.UsageSeriesData)
                {
                    DataPoint valueitem = new DataPoint(d_readings_charts_row.Key.ToOADate(), Convert.ToDouble(d_readings_charts_row.Value.Key));
                    usage_lineseries.Points.Add(valueitem);
                    //category.Add(d_readings_charts_row.Key.ToString(SmartParametersV2016.ddmmmyyyyFormat));
                }
                chart3.Series.Add(usage_lineseries);

                foreach (KeyValuePair<DateTime, double> d_readings_charts_row in utilityviewmodel.SineWaveSeriesData)
                {
                    DataPoint valueitem = new DataPoint(d_readings_charts_row.Key.ToOADate(), Convert.ToDouble(d_readings_charts_row.Value));
                    sinewave_lineseries.Points.Add(valueitem);
                }
                chart3.Series.Add(sinewave_lineseries);

                DateTime first = utilityviewmodel.UsageSeriesData.First().Key.Date;
                DateTime last = utilityviewmodel.UsageSeriesData.Last().Key.Date;

                double minValue = DateTimeAxis.ToDouble(first);
                double maxValue = DateTimeAxis.ToDouble(last);

                DateTimeAxis xAxis = new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    StringFormat = "MMM-yyyy",
                    Title = "Date",
                    Key = "Date",
                    Minimum = minValue,
                    Maximum = maxValue,
                    //MinorIntervalType = DateTimeIntervalType.Months,
                    IntervalType = DateTimeIntervalType.Months,
                    //MajorGridlineStyle = LineStyle.Solid,
                    //MinorGridlineStyle = LineStyle.None,


                    //AxisTickToLabelDistance = 1,
                    //MinorGridlineStyle = LineStyle.Dot,
                    //MajorGridlineStyle = LineStyle.Dot,
                    //MinorStep = 1,
                    //MajorStep = _lastNumOfDays == 90 ? 3 : 1, // #929
                    //MaximumPadding = 50,
                    //MinimumPadding = 50,
                    Angle = 90   // angle 35 with distance 1 works ok but not perfect
                };
                chart3.Axes.Add(xAxis);

            }
            return;
        }

#if WINFORMS
        //internal static LabelStyle Chart2_Labelstyle_Datetime()
        //{
        //   LabelStyle chart2_labelstyle_datetime = new LabelStyle()
        //   {
        //        IntervalType = new DateTimeIntervalType(),
        //        Format = "{0:MMM-yyyy}"
        //    };
        //    return chart2_labelstyle_datetime;
        //}
#endif
        //#if WPF || UWP || WINUI
        //        internal static Style Chart2_Labelstyle_Datetime()
        //        {
        //            // Don't know why this doesn't work
        //            Style labelstyle_datetime = new Style(typeof(AxisLabel));
        //            labelstyle_datetime . Setters.Add(new Setter(AxisLabel.StringFormatProperty, "{0:dd-MMM-yy}"));
        //            labelstyle_datetime . Setters.Add(new Setter(AxisLabel.RenderTransformProperty, new RotateTransform() { Angle = 60, CenterX = 17.5, CenterY = 25 }));
        //            labelstyle_datetime . Setters.Add(new Setter(AxisLabel.FontSizeProperty, 8.0));
        //            return labelstyle_datetime;
        //        }
        //#endif

#if WINFORMS
        //internal static LabelStyle Chart2_Labelstyle_Linear()
        //{
        //    LabelStyle chart2_labelstyle_linear = new LabelStyle()
        //    {
        //        Format = "{0:C}"
        //    };
        //    return chart2_labelstyle_linear;
        //}
#endif
        //#if WPF || UWP || WINUI
        //        internal static Style Chart2_Labelstyle_Linear()
        //        {
        //            Style labelstyle_linear = new Style(typeof(NumericAxisLabel));
        //            return labelstyle_linear;
        //        }
        //#endif
        //#if WPF || UWP || WINUI
        //        internal static Style Chart2_Labelstyle_Readings(char resource_code,
        //                                                        Setter templateSetter)
        //        {
        //            Style readings_style = new Style(typeof(ColumnDataPoint));  // To fix the line colour later
        //            readings_style.Setters.Add(new Setter(templateSetter.Property, templateSetter.Value));
        //            readings_style.Setters.Add(new Setter(ColumnDataPoint.IndependentValueStringFormatProperty, "{0:dd-MMM-yyyy}"));
        //            readings_style.Setters.Add(new Setter(ColumnDataPoint.DependentValueStringFormatProperty, "{0:0.000} kWh"));
        //            // Fix line colour here before we set the points
        //            // THANK FUCK FOR THAT - THIS ALL FUCKING WORKS AT LONG FUCKING LAST!!
        //            // Set the column colour to Magenta
        //            //    readings_style.Setters.Add(new Setter(ColumnDataPoint.BackgroundProperty, new SolidColorBrush(Colors.Magenta)));
        //            switch (resource_code)
        //            {
        //                case SmartParametersV2016.Electricity:
        //                    // Set the column colour to Orange (DOESN'T WORK)
        //                    readings_style.Setters.Add(new Setter(ColumnDataPoint.BackgroundProperty, new SolidColorBrush(Colors.Orange)));
        //                    break;
        //                case SmartParametersV2016.Gas:
        //                    // Set the column colour to Blue
        //                    readings_style.Setters.Add(new Setter(ColumnDataPoint.BackgroundProperty, new SolidColorBrush(Colors.Blue)));
        //                    break;
        //                default:
        //                    readings_style.Setters.Add(new Setter(ColumnDataPoint.BackgroundProperty, new SolidColorBrush(Colors.Magenta)));
        //                    break;
        //            }
        //            return readings_style;
        //        }
        //#endif

        // For when we have to use the Readings

        // For when we have real Usage data
        internal static List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> Chart3B_Code(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel,
                                                                        List<string> category,
                                                                        char resource_code)
        //rf DateTime max,    // For sine wave fitting
        //rf DateTime min,    // For sine wave fitting
        //#if WINFORMS || ANDROID
        //                                                                      //rf Color background)
        //#endif
        //#if WPF || UWP || WINUI
        //                                                                      //rf Color background)
        //#endif
        {
            utilityviewmodel.e_usage_view_found = new List<SmartUtility.EUsageView>();
            utilityviewmodel.g_usage_view_found = new List<SmartUtility.GUsageView>();
            utilityviewmodel.d_usage_view_found = new List<SmartUtility.EUsageView>();

            int usage_rows_count = 0;

            // Chart3B is Usage
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    SmartSpikeUtilityV2017.Utility_Analyze_Charts(utilityviewmodel,
                                                resource_code);
                    //rf e_usage_view_found,
                    //rf g_usage_view_found,
                    //rf d_usage_view_found);
                    usage_rows_count = utilityviewmodel.e_usage_view_found.Count;
                    if (usage_rows_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.e_usage_view_found.First().USAGE_DATETIME;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.e_usage_view_found.Last().USAGE_DATETIME;
                    }

                    // Set the colour to Orange
                    //chart3_linestyle.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, "Orange"));
#if WINFORMS
                    //utilityviewmodel.ChartsBackground = OxyColors.Orange;
#endif
#if ANDROIDX
                    utilityviewmodel.ChartsBackground = OxyColors.Orange;
#endif
#if WPF || UWP || WINUI
                    utilityviewmodel.ChartsBackground = OxyColors.Orange;
#endif
                    break;
                case SmartParametersV2016.Gas:
                    SmartSpikeUtilityV2017.Utility_Analyze_Charts(utilityviewmodel,
                                                resource_code);
                    //rf e_usage_view_found,
                    //rf g_usage_view_found,
                    //rf d_usage_view_found);
                    usage_rows_count = utilityviewmodel.g_usage_view_found.Count;
                    if (usage_rows_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.g_usage_view_found.First().USAGE_DATETIME;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.g_usage_view_found.Last().USAGE_DATETIME;
                    }
                    // Set the colour to Blue
                    //chart3_linestyle.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, "Blue"));
#if WINFORMS
                    //utilityviewmodel.ChartsBackground = OxyColors.Blue;
#endif
#if ANDROIDX
                    utilityviewmodel.ChartsBackground = OxyColors.Blue;
#endif
#if WPF || UWP || WINUI
                    utilityviewmodel.ChartsBackground = OxyColors.Blue;
#endif
                    break;
                case SmartParametersV2016.DualFuel:
                    SmartSpikeUtilityV2017.Utility_Analyze_Charts(utilityviewmodel,
                                                resource_code);
                    //rf e_usage_view_found,
                    //rf g_usage_view_found,
                    //rf d_usage_view_found);
                    usage_rows_count = utilityviewmodel.d_usage_view_found.Count;
                    if (usage_rows_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.d_usage_view_found.First().USAGE_DATETIME;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.d_usage_view_found.Last().USAGE_DATETIME;
                    }

                    // Set the colour to HotPink
                    //chart3_linestyle.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, "Orange"));
#if WINFORMS
                    //utilityviewmodel.ChartsBackground = OxyColors.HotPink;    //!!!!!
#endif
#if ANDROIDX
                    utilityviewmodel.ChartsBackground = OxyColors.HotPink;    //!!!!!
#endif
#if WPF || UWP || WINUI
                    utilityviewmodel.ChartsBackground = OxyColors.HotPink;    //!!!!!
#endif
                    break;
                default:
                    break;
            }

            
            List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> chart3_usage_points = new List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>>();
            if (usage_rows_count > 0)
            {
                // All this bollocks! Just to get the meter serial no on the tooktip ...
                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        // We always lose (but need and use) the first entry in this set 
                        foreach (SmartUtility.EUsageView usage_view_row in utilityviewmodel.e_usage_view_found)
                        {
                            KeyValuePair<decimal, string> amelia = new KeyValuePair<decimal, string>(usage_view_row.USAGE_TOTAL, usage_view_row.USAGE_VALUE + SmartParametersV2016.bar.ToString() + usage_view_row.METER_SERIAL_NO);
                            chart3_usage_points.Add(new KeyValuePair<DateTime, KeyValuePair<decimal, string>>(usage_view_row.USAGE_DATETIME, amelia));
                        }
                        break;
                    case SmartParametersV2016.Gas:
                        foreach (SmartUtility.GUsageView usage_view_row in utilityviewmodel.g_usage_view_found)
                        {
                            KeyValuePair<decimal, string> amelia = new KeyValuePair<decimal, string>(usage_view_row.USAGE_TOTAL, usage_view_row.USAGE_VALUE + SmartParametersV2016.bar.ToString() + usage_view_row.METER_SERIAL_NO);
                            chart3_usage_points.Add(new KeyValuePair<DateTime, KeyValuePair<decimal, string>>(usage_view_row.USAGE_DATETIME, amelia));
                        }
                        break;
                    case SmartParametersV2016.DualFuel:
                        // We always lose (but need and use) the first entry in this set 
                        foreach (SmartUtility.EUsageView usage_view_row in utilityviewmodel.d_usage_view_found)
                        {
                            KeyValuePair<decimal, string> amelia = new KeyValuePair<decimal, string>(usage_view_row.USAGE_TOTAL, usage_view_row.USAGE_VALUE + SmartParametersV2016.bar.ToString() + usage_view_row.METER_SERIAL_NO);
                            chart3_usage_points.Add(new KeyValuePair<DateTime, KeyValuePair<decimal, string>>(usage_view_row.USAGE_DATETIME, amelia));
                        }
                        break;
                    default:
                        break;
                }
            }
            return chart3_usage_points;
        }
    }
}