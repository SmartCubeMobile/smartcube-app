
namespace SmartCubeMobile
{
    //This is interface is defined in shared code. You then need a class in each project which
    //Implements this interface.
    //
#if WPF
    public interface IScreen
    {
        void SwitchScreen(bool quit, MainMeter components);
    }

#else
    public interface IScreen
    {
        void SwitchScreen(bool quit);
    }
#endif
}