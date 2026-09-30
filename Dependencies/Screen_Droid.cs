
[assembly: Xamarin.Forms.Dependency(typeof(SmartCubeMobile.Droid.ScreenSwitch))]
namespace SmartCubeMobile.Droid
{
    public class ScreenSwitch : IScreen
    {
        // Quit has no relevance here - Android is always full-screen
        void IScreen.SwitchScreen(bool quit) 
        {
            return;
        }
    }
}