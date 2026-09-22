using System.Windows;
using System.Windows.Controls;
using SmartCubeMobile;

public class ScreenService : IScreen
{
    private MainViewModel ourviewmodel;
    public ScreenService(MainViewModel mainvm)
    { 
        this.ourviewmodel = mainvm;
    }
    // Explicit interface member implementation:
    void IScreen.SwitchScreen(bool quit, MainMeter components)
    {
        if (quit)
        {
            if (ourviewmodel.isfullscreen)
            {
                components.WindowState = ourviewmodel.state;
                ourviewmodel.isfullscreen = false;
            }
        }
        else
        {
            Grid grid = (Grid)components.FindName("DontDeleteMe");
            if (!ourviewmodel.isfullscreen)
            {
                // hide these three
                ourviewmodel.rowheight = grid.RowDefinitions[0].Height;
                grid.RowDefinitions[0].Height = new GridLength(0);
                //ScaleTransform abc = Calc_ScaleX(ourviewmodel.actualWidth, ourviewmodel.actualHeight);
                //components.RenderTransform = abc;
                // Re-size window
                ourviewmodel.state = components.WindowState;
                components.WindowState = WindowState.Maximized;
                ourviewmodel.isfullscreen = true;
            }
            else
            {
                // Put everything back
                grid.RowDefinitions[0].Height = ourviewmodel.rowheight;
                components.WindowState = ourviewmodel.state;
                ourviewmodel.isfullscreen = false;
            }
            SmartRoutinesV2018.Record_ScreenSwitch(ourviewmodel);
        }
        return;
    }
}