
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


namespace SmartCubeMobile
{
    public class SmartLiveCharts2V2026
    {
        internal static ISeries[] CreateDebitsCreditsSeries(
            string title,
            MainViewModel ourviewmodel,
            FinanceViewModel financeviewmodel,
            out Axis[] xAxes,
            out Axis[] yAxes)
        {
            List<SmartFinance.DebitsCreditsByMonth> data =
                Chart2Code(financeviewmodel);

            List<double> debitsValues = new();
            List<double> creditsValues = new();
            List<string> labels = new();

            foreach (SmartFinance.DebitsCreditsByMonth row in data)
            {
                debitsValues.Add(row.DEBITS / 100.0);
                creditsValues.Add(row.CREDITS / 100.0);

                labels.Add(
                    row.PERIOD_END.ToString(
                        SmartParametersV2016.ddmmmyyyyFormat,
                        financeviewmodel.CultureINF));
            }

            // X Axis (dates)
            xAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsRotation = 45,
                    Name = "Date"
                }
            };

            // Y Axis (£ values)
            yAxes = new Axis[]
            {
                new Axis
                {
                    Name = "£"
                }
            };

            // Chart Series
            ISeries[] series =
            {
                new ColumnSeries<double>
                {
                    Name = "Debits",
                    Values = debitsValues,

                    Fill = new SolidColorPaint(SKColors.Pink),
                    Stroke = new SolidColorPaint(SKColors.Black)
                    {
                        StrokeThickness = 1
                    },

                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsPosition =
                        LiveChartsCore.Measure.DataLabelsPosition.Top
                },

                new ColumnSeries<double>
                {
                    Name = "Credits",
                    Values = creditsValues,

                    Fill = new SolidColorPaint(SKColors.LightGreen),
                    Stroke = new SolidColorPaint(SKColors.Black)
                    {
                        StrokeThickness = 1
                    },

                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsPosition =
                        LiveChartsCore.Measure.DataLabelsPosition.Top
                }
            };

            return series;
        }

        internal static List<SmartFinance.DebitsCreditsByMonth> Chart2Code(
            FinanceViewModel financeviewmodel)
        {
            List<SmartFinance.DebitsCreditsByMonth> totals_found =
                new List<SmartFinance.DebitsCreditsByMonth>();

            double debits_total = 0;
            double credits_total = 0;

            DateTime last_transaction_date =
                SmartParametersV2016.defaultDate;

            foreach (SmartFinance.CommonTransactionsView transactionView
                in financeviewmodel.FinanceTransactions)
            {
                DateTime rays =
                    Convert.ToDateTime(
                        transactionView.TRANSACTION_DATE,
                        financeviewmodel.CultureINF);

                DateTime this_transaction_date =
                    new DateTime(rays.Year, rays.Month, 1);

                if (DateTime.Compare(
                    this_transaction_date,
                    last_transaction_date) < 0)
                {
                    SmartFinance.DebitsCreditsByMonth monthlyTotal =
                        new SmartFinance.DebitsCreditsByMonth(

                        last_transaction_date
                            .AddDays(-1)
                            .AddMonths(1),

                        debits_total,
                        credits_total
                    );

                    totals_found.Add(monthlyTotal);

                    debits_total = 0;
                    credits_total = 0;
                }

                if (transactionView.CREDITDEBIT_INDICATOR == 0)
                {
                    debits_total +=
                        Math.Abs(transactionView.AMOUNT);
                }
                else
                {
                    credits_total +=
                        transactionView.AMOUNT;
                }

                last_transaction_date = this_transaction_date;
            }

            SmartFinance.DebitsCreditsByMonth finalTotal =
                new SmartFinance.DebitsCreditsByMonth(

                last_transaction_date
                    .AddDays(-1)
                    .AddMonths(1),

                debits_total,
                credits_total
            );

            totals_found.Add(finalTotal);

            totals_found.Sort();

            return totals_found;
        }
    }
}