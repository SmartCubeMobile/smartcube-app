#if WPF  || WINUI
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
#endif

namespace SmartCubeMobile
{
    //
    //This is interface is defined in shared code. You then need a class in each project which
    //Implements this interface.
    //
#if WPF  || WINUI || ANDROID || WINDOWS || IOS || MACCATALYST
    public interface IPrint
    {
        Task Print(string filePath, object viewmodel);
    }
#endif
}