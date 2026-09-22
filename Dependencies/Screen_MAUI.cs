
namespace SmartCubeMobile
{
    public class ScreenService : IScreen
    {
        private readonly MainViewModel ourviewmodel;

        public ScreenService(MainViewModel mainvm)
        {
            ourviewmodel = mainvm;
        }

        void IScreen.SwitchScreen(bool quit, Grid grid)
        {
            //var page = components as ContentPage;

            if (grid == null)
            {
                return;
            }

            //var grid = page.FindByName<Grid>("DontDeleteMe");

            if (quit)
            {
                if (ourviewmodel.isfullscreen)
                {
                    // Restore UI state (no WindowState in MAUI)
                    RestoreLayout(grid);
                    ourviewmodel.isfullscreen = false;
                }

                return;
            }

            if (!ourviewmodel.isfullscreen)
            {
                // Save current row height
                ourviewmodel.rowheight = grid.RowDefinitions[0].Height;

                // Hide row (same as WPF logic)
                grid.RowDefinitions[0].Height = new GridLength(0);

                // Store state
                ourviewmodel.isfullscreen = true;
            }
            else
            {
                RestoreLayout(grid);
                ourviewmodel.isfullscreen = false;
            }

            SmartRoutinesV2018.Record_ScreenSwitch(ourviewmodel);
        }

        private void RestoreLayout(Grid grid)
        {
            if (grid == null)
                return;

            grid.RowDefinitions[0].Height = ourviewmodel.rowheight;
        }
    }
}