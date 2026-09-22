using static SmartCubeMobile.DeviceDependency;

[assembly: Xamarin.Forms.Dependency(typeof(SmartCubeMobile.Droid.DependencyImplementation))]
namespace SmartCubeMobile.Droid
{
    public class DependencyImplementation : IDeviceDependency
    {
        public string GetThePlatformMessage()
        {
            return SmartParametersV2016.Android; // Android
        }
    }
}