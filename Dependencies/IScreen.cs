namespace SmartCubeMobile
{
    //This is interface is defined in shared code. You then need a class in each project which
    //Implements this interface.
    //
#if WPF  || WINUI
    public interface IScreen
    {
        void SwitchScreen(bool quit, MainMeter components);
    }
#endif

#if SMARTMAUI
    public interface IScreen
    {
        void SwitchScreen(bool quit, Grid grid);
    }
#endif
#if WINFORMS
    public interface IScreen
    {
        void SwitchScreen(bool quit);
    }
#endif
}