using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(SmartCubeMobile.Droid.ExternalDirectory))]
namespace SmartCubeMobile.Droid
{
    public class ExternalDirectory : IDirectory
    {
        bool IDirectory.GetDirectory(ref string directory)
        {
            // Android.OS.Environment.DirectoryDownloads <= (from the docs) 'Standard place to place files which have been downloaded by the user'
            try
            {
                directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
                //directory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath;
                if (!string.IsNullOrEmpty(directory))
                {
                    //File abc = new File("mine");
                    //if (abc.Exists())
                    //{
                    //    //Console.WriteLine("Its there");
                    //}
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}