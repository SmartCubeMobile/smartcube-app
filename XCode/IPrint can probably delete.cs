using System.IO;

namespace SmartCubeMobile
{
    //This is interface is defined in shared code. You then need a class in each project which
    //Implements this interface.
    //
#if WPF
    public interface IPrint
    {
        void Print(string filename);
    }
#endif
#if XAMARIN
    public interface IPrintService
    {
        void Print(Stream printStream, string fileName);
    }
#endif
#if ANDROID
    // Miss out all the interface shit ...
#endif
}