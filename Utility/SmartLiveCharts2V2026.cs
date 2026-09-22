using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

using SkiaSharp;

namespace SmartCubeMobile
{
   public static class SmartLiveCharts2
   {
        // =========================================================
        // CREATE LINE SERIES
        // =========================================================

        public static async Task<List<ISeries>> CreateLineSeries(
                                                    string title,
                                                    MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel)
        {
            List<ISeries> seriesCollection = new();

            utilityviewmodel.ChartsEngineFromDate = SmartParametersV2016.defaultDate;
            utilityviewmodel.ChartsEngineToDate = SmartParametersV2016.defaultDate;

            List<SmartUtility.AnalysisCosts> analysis_costs_found = new();

            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    analysis_costs_found =
                        utilityviewmodel.Hezbollah.e_analysis_costsList;
                    break;

                case SmartParametersV2016.Gas:
                    analysis_costs_found =
                        utilityviewmodel.Hezbollah.g_analysis_costsList;
                    break;

                case SmartParametersV2016.DualFuel:
                    analysis_costs_found =
                        utilityviewmodel.Hezbollah.d_analysis_costsList;
                    break;
            }

            if (analysis_costs_found.Count <= 1)
            {
                return seriesCollection;
            }
            utilityviewmodel.ChartsMinimum =
                SmartParametersV2016.defaultDate;

            utilityviewmodel.ChartsMaximum =
                SmartParametersV2016.defaultDate;

            int tariff_engineList_count =
                Set_Which_Engine(
                    utilityviewmodel,
                    utilityviewmodel.resource_code,
                    utilityviewmodel.e_engine_dates,
                    utilityviewmodel.g_engine_dates);

            if (tariff_engineList_count <= 0)
            {
                return seriesCollection;
            }

            // =====================================================
            // CHEAPEST
            // =====================================================

            var cheapest =
                await ComputePointsLiveCharts(
                    ourviewmodel,
                    utilityviewmodel,
                    utilityviewmodel.area_code,
                    analysis_costs_found.First().BRAND_CODE,
                    analysis_costs_found.First().SUPPLIER_CODE,
                    analysis_costs_found.First().TARIFF_CODE,
                    Convert.ToChar(
                        analysis_costs_found.First().PAYMENT_PLAN),
                    utilityviewmodel.resource_code,
                    utilityviewmodel.resource_type,
                    utilityviewmodel.charts_brand_code,
                    utilityviewmodel.charts_supplier_code,
                    utilityviewmodel.ChartsEngineFromDate,
                    utilityviewmodel.age,
                    utilityviewmodel.withdrawn_date,
                    utilityviewmodel.already_doneList,
                    SKColors.Green);

            cheapest.Name =
                SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(
                    ourviewmodel,
                    utilityviewmodel,
                    analysis_costs_found.First().SUPPLIER_CODE,
                    analysis_costs_found.First().BRAND_CODE)
                + "-"
                +
                SmartSpikeUtilityV2017.Utility_Lookup_TariffName(
                    utilityviewmodel,
                    utilityviewmodel.resource_code,
                    analysis_costs_found.First().SUPPLIER_CODE,
                    analysis_costs_found.First().BRAND_CODE,
                    analysis_costs_found.First().RESOURCE_TYPE,
                    analysis_costs_found.First().TARIFF_CODE);

            seriesCollection.Add(cheapest);

            // =====================================================
            // DEAREST
            // =====================================================

            var dearest =
                await ComputePointsLiveCharts(
                    ourviewmodel,
                    utilityviewmodel,
                    utilityviewmodel.area_code,
                    analysis_costs_found.Last().BRAND_CODE,
                    analysis_costs_found.Last().SUPPLIER_CODE,
                    analysis_costs_found.Last().TARIFF_CODE,
                    Convert.ToChar(
                        analysis_costs_found.Last().PAYMENT_PLAN),
                    utilityviewmodel.resource_code,
                    utilityviewmodel.resource_type,
                    utilityviewmodel.charts_brand_code,
                    utilityviewmodel.charts_supplier_code,
                    utilityviewmodel.ChartsEngineFromDate,
                    utilityviewmodel.age,
                    utilityviewmodel.withdrawn_date,
                    utilityviewmodel.already_doneList,
                    SKColors.Red);

            dearest.Name =
                SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(
                    ourviewmodel,
                    utilityviewmodel,
                    analysis_costs_found.Last().SUPPLIER_CODE,
                    analysis_costs_found.Last().BRAND_CODE)
                + "-"
                +
                SmartSpikeUtilityV2017.Utility_Lookup_TariffName(
                    utilityviewmodel,
                    utilityviewmodel.resource_code,
                    analysis_costs_found.Last().SUPPLIER_CODE,
                    analysis_costs_found.Last().BRAND_CODE,
                    analysis_costs_found.Last().RESOURCE_TYPE,
                    analysis_costs_found.Last().TARIFF_CODE);

            seriesCollection.Add(dearest);

            // =====================================================
            // OURS
            // =====================================================

            var ours =
                await ComputePointsLiveCharts(
                    ourviewmodel,
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
                    utilityviewmodel.already_doneList,
                    SKColors.Blue);

            ours.Name =
                SmartSpikeUtilityV2017.Utility_Lookup_SupplierName(
                    ourviewmodel,
                    utilityviewmodel,
                    utilityviewmodel.charts_supplier_code,
                    utilityviewmodel.charts_brand_code)
                + "-"
                +
                SmartSpikeUtilityV2017.Utility_Lookup_TariffName(
                    utilityviewmodel,
                    utilityviewmodel.resource_code,
                    utilityviewmodel.charts_supplier_code,
                    utilityviewmodel.charts_brand_code,
                    utilityviewmodel.resource_type,
                    utilityviewmodel.tariff_code);

            seriesCollection.Add(ours);

            utilityviewmodel.ChartSeries1 =
                seriesCollection.ToArray();

            utilityviewmodel.CostsXAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Date",
                    LabelsRotation = 45,
                    Labeler = value =>
                    {
                        return new DateTime((long)value)
                        .ToString("dd-MMM-yyyy");
                    }
                }
            };

            utilityviewmodel.CostsYAxes = new Axis[]
            {
                new Axis
                {
                    Name = "Cost £"
                }
            };
            
            return seriesCollection;
            
        }

        // =========================================================
        // COMPUTE POINTS
        // =========================================================

        internal static async Task<LineSeries<DateTimePoint>>
            ComputePointsLiveCharts(
                MainViewModel ourviewmodel,
                UtilityViewModel utilityviewmodel,
                short area_code,
                short comparison_brand_code,
                short comparison_supplier_code,
                int comparison_tariff_code,
                char comparison_payment_plan,
                char resource_code,
                string resource_type,
                short displayed_brand_code,
                short displayed_supplier_code,
                DateTime engine_from_date,
                int age,
                DateTime withdrawn_date,
                List<SmartUtility.AnalysisConditions> already_doneList,
                SKColor colour)
        {
            List<DateTimePoint> points = new();

            short target_supplier_code = 0;
            int target_tariff_code = 0;

            char target_payment_plan =
                SmartParametersV2016.defaultChar;

            bool result =
                await SmartAnalyzeV2016.Analyze_Costs(
                    ourviewmodel,
                    utilityviewmodel,
                    false,
                    area_code,
                    resource_code,
                    resource_type,
                    displayed_brand_code,
                    displayed_supplier_code,
                    comparison_brand_code,
                    comparison_supplier_code,
                    comparison_tariff_code,
                    comparison_payment_plan,
                    target_supplier_code,
                    target_tariff_code,
                    target_payment_plan,
                    engine_from_date,
                    age,
                    withdrawn_date,
                    already_doneList);

            if (result)
            {
                int count = 0;

                double total_cost = 0;

                DateTime last_date =
                    SmartParametersV2016.defaultDate;

                foreach (
                    SmartUtility.TariffCosts tariff_costs_row
                    in utilityviewmodel.Hezbollah.tariff_costsList)
                {
                    DateTime read_date =
                        tariff_costs_row.PERIOD_END;

                    if ((SmartRoutinesV2018.DateTimeCompare(
                            read_date,
                            last_date) > 0)
                        &&
                        (last_date !=
                         SmartParametersV2016.defaultDate))
                    {
                        if (count == 0
                            ||
                            ((count / 100) * 100 == count))
                        {
                            points.Add(
                                new DateTimePoint(
                                    last_date,
                                    Math.Round(
                                        total_cost / 100,
                                        2)));
                        }
                    }

                    total_cost +=
                        Convert.ToDouble(
                            tariff_costs_row.FIGURE);

                    last_date = read_date;

                    count++;
                }

                points.Add(
                    new DateTimePoint(
                        last_date,
                        Math.Round(total_cost / 100, 2)));
            }

            return new LineSeries<DateTimePoint>
            {
                Values = points,

                GeometrySize = 0,

                Fill = null,

                Stroke = new SolidColorPaint(colour, 3)
            };
        }

        // =========================================================
        // CREATE READINGS SERIES
        // =========================================================

        internal static ISeries[] CreateReadingsSeries(
            string title,
            MainViewModel ourviewmodel,
            UtilityViewModel utilityviewmodel)
        {
            List<ISeries> readingsCollection = new();

            List<double> values = new();

            List<string> labels = new();

            List<SmartUtility.DReadingsCharts>
                d_readings_charts =
                Chart2Code(
                    utilityviewmodel,
                    utilityviewmodel.resource_code);

            foreach (var row in d_readings_charts)
            {
                values.Add(
                    Convert.ToDouble(row.UNITS_USED));

                labels.Add(
                    row.PERIOD_END.ToString(
                        SmartParametersV2016.ddmmmyyyyFormat,
                        utilityviewmodel.CultureINF));
            }

            SKColor colour = SKColors.Orange;

            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Gas:
                    colour = SKColors.Blue;
                    break;

                case SmartParametersV2016.DualFuel:
                    colour = SKColors.Magenta;
                    break;
            }

            utilityviewmodel.ChartSeries2 =
                new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = values,

                        Fill =
                            new SolidColorPaint(colour)
                    }
                };

            utilityviewmodel.ReadingsXAxes =
                new Axis[]
                {
                    new Axis
                    {
                        Labels = labels,

                        LabelsRotation = 45,

                        Name = "Date"
                    }
                };

            utilityviewmodel.ReadingsYAxes =
                new Axis[]
                {
                    new Axis
                    {
                        Name = "kWh"
                    }
                };
            return utilityviewmodel.ChartSeries2;
        }

        // =========================================================
        // CREATE USAGE SERIES
        // =========================================================

        internal static ISeries[] CreateUsageSeries(
            string title,
            MainViewModel ourviewmodel,
            UtilityViewModel utilityviewmodel)
        {
            List<ISeries> series = new();

            if ((utilityviewmodel.resource_code
                 == SmartParametersV2016.Electricity
                 &&
                 utilityviewmodel.Hezbollah.e_usageList.Count
                 == 0)
                ||
                (utilityviewmodel.resource_code
                 == SmartParametersV2016.Gas
                 &&
                 utilityviewmodel.Hezbollah.g_usageList.Count
                 == 0))
            {
                ChartThreeA(
                    ourviewmodel,
                    utilityviewmodel,
                    utilityviewmodel.resource_code,
                    title,
                    series);
            }
            else
            {
                ChartThreeB(
                    ourviewmodel,
                    utilityviewmodel,
                    utilityviewmodel.resource_code,
                    title,
                    series);
            }

            utilityviewmodel.ChartSeries3 =
                series.ToArray();

            utilityviewmodel.UsageXAxes =
                new Axis[]
                {
                new Axis
                {
                    Name = "Date",

                    LabelsRotation = 45
                }
                };

            utilityviewmodel.UsageYAxes =
                new Axis[]
                {
                    new Axis
                    {
                        Name = "kWh"
                    }
                };
            return utilityviewmodel.ChartSeries3;
        }

        // =========================================================
        // CHART THREE A
        // =========================================================

        internal static void ChartThreeA(
            MainViewModel ourviewmodel,
            UtilityViewModel utilityviewmodel,
            char resource_code,
            string displayed_tariff_name,
            List<ISeries> chartSeries)
        {
            List<SmartUtility.DReadingsCharts>
                d_usage_charts =
                Chart3A_Code(
                    utilityviewmodel,
                    resource_code);

            if (d_usage_charts.Count == 0)
                return;

            utilityviewmodel.UsageSeriesData =
                new List<
                    KeyValuePair<
                        DateTime,
                        KeyValuePair<decimal, string>>>();

            utilityviewmodel.SineWaveSeriesData =
                new List<KeyValuePair<DateTime, double>>();

            Sine_Wave_Points(
                utilityviewmodel,
                resource_code,
                d_usage_charts);

            List<DateTimePoint> usagePoints = new();

            foreach (var row
                     in utilityviewmodel.UsageSeriesData)
            {
                usagePoints.Add(
                    new DateTimePoint(
                        row.Key,
                        Convert.ToDouble(row.Value.Key)));
            }

            chartSeries.Add(
                new LineSeries<DateTimePoint>
                {
                    Name = "Usage",

                    Values = usagePoints,

                    GeometrySize = 0
                });

            List<DateTimePoint> sinePoints = new();

            foreach (var row
                     in utilityviewmodel.SineWaveSeriesData)
            {
                sinePoints.Add(
                    new DateTimePoint(
                        row.Key,
                        row.Value));
            }

            chartSeries.Add(
                new LineSeries<DateTimePoint>
                {
                    Name = "SineWave",

                    Values = sinePoints,

                    GeometrySize = 0
                });
        }

        // =========================================================
        // CHART THREE B
        // =========================================================

        internal static void ChartThreeB(
            MainViewModel ourviewmodel,
            UtilityViewModel utilityviewmodel,
            char resource_code,
            string displayed_tariff_name,
            List<ISeries> chartSeries)
        {
            utilityviewmodel.UsageSeriesData =
                Chart3B_Code(
                    ourviewmodel,
                    utilityviewmodel,
                    new List<string>(),
                    resource_code);

            List<DateTimePoint> usagePoints = new();

            foreach (var row
                     in utilityviewmodel.UsageSeriesData)
            {
                usagePoints.Add(
                    new DateTimePoint(
                        row.Key,
                        Convert.ToDouble(row.Value.Key)));
            }

            chartSeries.Add(
                new LineSeries<DateTimePoint>
                {
                    Name = "Usage",

                    Values = usagePoints,

                    GeometrySize = 0
                });
        }

        internal static List<KeyValuePair<DateTime, KeyValuePair<decimal, string>>> Chart3B_Code(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel,
                                                                        List<string> category,
                                                                        char resource_code)
        
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
                    usage_rows_count = utilityviewmodel.e_usage_view_found.Count;
                    if (usage_rows_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.e_usage_view_found.First().USAGE_DATETIME;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.e_usage_view_found.Last().USAGE_DATETIME;
                    }
                    break;
                case SmartParametersV2016.Gas:
                    SmartSpikeUtilityV2017.Utility_Analyze_Charts(utilityviewmodel,
                                                resource_code);
                    usage_rows_count = utilityviewmodel.g_usage_view_found.Count;
                    if (usage_rows_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.g_usage_view_found.First().USAGE_DATETIME;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.g_usage_view_found.Last().USAGE_DATETIME;
                    }
                    break;
                case SmartParametersV2016.DualFuel:
                    SmartSpikeUtilityV2017.Utility_Analyze_Charts(utilityviewmodel,
                                                resource_code);
                    usage_rows_count = utilityviewmodel.d_usage_view_found.Count;
                    if (usage_rows_count > 0)
                    {
                        utilityviewmodel.ChartsMinimum = utilityviewmodel.d_usage_view_found.First().USAGE_DATETIME;
                        utilityviewmodel.ChartsMaximum = utilityviewmodel.d_usage_view_found.Last().USAGE_DATETIME;
                    }
                    break;
                default:
                    break;
            }

            // Fucking bitch just dumped her bowl on the side for the skivvy to clean up into the dishwasher
            // Lazy fucking bitch
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

        internal static void Sine_Wave_Points(UtilityViewModel utilityviewmodel,
                                            char resource_code,
                                            List<SmartUtility.DReadingsCharts> d_usage_charts)
        {
            double a = (utilityviewmodel.chartsmax - utilityviewmodel.chartsmin) / 2;
            double B = (utilityviewmodel.chartsmax + utilityviewmodel.chartsmin) / 2;
            int period = 12;
            double w = (2 * Math.PI) / period;
            double phase_shift = 0;
            phase_shift = (w * utilityviewmodel.ChartsXValue) - (3 * Math.PI / 2);
            int local_x_value = utilityviewmodel.ChartsXValue;
            local_x_value = 1;
            // Get ready to store the points
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    break;
                case SmartParametersV2016.Gas:
                    break;
                case SmartParametersV2016.DualFuel:
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

        internal static void Choose(UtilityViewModel utilityviewmodel,
                                    int record_count,
                                    double units_used)
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

        internal static DateTime Convert_Date(UtilityViewModel utilityviewmodel,
                                                string date_string,
                                                DateTime defaultDate)
        {
            DateTime date = defaultDate;
            try
            {
                date = SmartRoutinesV2018.DateTimeParseCulture(date_string, SmartParametersV2016.defaultCulture);
            }
            catch (ArgumentNullException exception)
            {
                if (string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                {
                    utilityviewmodel.errorMessage = exception.Message;
                }
            }
            catch (FormatException exception)
            {
                if (string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                {
                    utilityviewmodel.errorMessage = exception.Message;
                }
            }
            catch (ArgumentException exception)
            {
                if (string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                {
                    utilityviewmodel.errorMessage = exception.Message;
                }
            }
            return date;
        }

        internal static void Force_Start_Of_Month(UtilityViewModel utilityviewmodel,
                                            DateTime defaultDate)
        {
            string start_temp;
            start_temp = utilityviewmodel.ChartsStartDate.ToString(SmartParametersV2016.defaultCulture); // Otherwise next line doesn't work!!
            start_temp = "01/" + start_temp.Substring(3);
            utilityviewmodel.ChartsStartDate = Convert_Date(utilityviewmodel, start_temp, defaultDate);
            return;
        }

        internal static void Force_End_Of_Month(UtilityViewModel utilityviewmodel,
                                                DateTime defaultDate)
        {
            string start_temp;
            utilityviewmodel.ChartsStartDate = utilityviewmodel.ChartsStartDate.AddMonths(1);
            start_temp = utilityviewmodel.ChartsStartDate.ToString(SmartParametersV2016.defaultCulture); // Otherwise next line doesn't work!!
            start_temp = "01/" + start_temp.Substring(3);
            utilityviewmodel.ChartsStartDate = Convert_Date(utilityviewmodel, start_temp, defaultDate);
            utilityviewmodel.ChartsStartDate = utilityviewmodel.ChartsStartDate.AddDays(-1);
            return;
        }

        internal static List<SmartUtility.DReadingsCharts> Chart3A_Code(UtilityViewModel utilityviewmodel,
                                                            char resource_code)
        {
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
                            short no_of_days = (short)d_readings_row.READINGS_PERIOD_END.Subtract(d_readings_row.READINGS_PERIOD_START).Days;
                            if (no_of_days > 0)
                            {
                                decimal daily_usage = (d_readings_row.D_UNITS_USED + d_readings_row.N_UNITS_USED) / no_of_days;
                                while (SmartRoutinesV2018.DateTimeCompare(start_date, d_readings_row.READINGS_PERIOD_END) <= 0)
                                {
                                    d_readings_temp.Add(new SmartUtility.DReadingsCharts(start_date, daily_usage, d_readings_row.METER_SERIAL_NO));
                                    start_date = start_date.AddDays(1);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                d_readings_temp.Sort();
                if (d_readings_temp.Count > 0)
                {
                    start_date = d_readings_temp.First().PERIOD_END;
                }
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
                    }
                    if (SmartRoutinesV2018.DateTimeCompare(d_readings_temp_row.PERIOD_END, match_date) == 0)
                    {
                        d_readings_charts.Add(new SmartUtility.DReadingsCharts(this_date, total_usage, d_readings_temp_row.METER_SERIAL_NO));
                        Choose(utilityviewmodel, record_count, (double)total_usage);//, rf max, rf min, rf x_value);
                        record_count++;
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
                    d_readings_charts.Add(new SmartUtility.DReadingsCharts(this_date, total_usage, this_meter_serial_no));
                    Choose(utilityviewmodel, record_count, (double)total_usage);// rf max, rf min, rf x_value);
                }
            }
            return d_readings_charts;
        }

        internal static List<SmartUtility.DReadingsCharts> Chart2Code(UtilityViewModel utilityviewmodel,
                                                                                    char resource_code)
        {
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
                decimal units_used;
                bool average_usage = false;
                decimal residue;
                switch (resource_code)
                {
                    case SmartParametersV2016.Electricity:
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
                d_readings_charts.Sort();
            }
            return d_readings_charts;
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
    }
}