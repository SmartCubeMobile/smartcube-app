using Microsoft.Maui.Storage;

namespace SmartCubeMobile
{
    public class PrintService : IPrint
    {
        private FinanceViewModel financeviewmodel;
        private UtilityViewModel utilityviewmodel;

        public async Task Print(string filePath, object viewmodel)
        {
            switch (viewmodel)
            {
                case FinanceViewModel financeVm:
                    financeviewmodel = financeVm;
                    break;

                case UtilityViewModel utilityVm:
                    utilityviewmodel = utilityVm;
                    break;
            }

            try
            {
                // Ensure file exists
                if (!File.Exists(filePath))
                    return;

                // Let OS handle PDF viewing/printing
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });

                // Re-enable button
                if (financeviewmodel != null)
                    financeviewmodel.PrintButtonEnabled = true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Printing error",
                    $"Failed to open PDF.\n{ex.Message}",
                    "OK");

                if (financeviewmodel != null)
                    financeviewmodel.PrintButtonEnabled = true;
            }
        }
    }
}