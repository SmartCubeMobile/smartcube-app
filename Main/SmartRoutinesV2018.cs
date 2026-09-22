using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Layer;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
//using MetalPerformanceShaders;
using System.Globalization;
using System.Net;
//using System.Windows.Media;     // This is in PresentationCore.dll <= FUCKING OBVIOUSLY!! Microshit fucking wanking idiots


using System.Reflection;
using System.Text;
using System.Text.Json;


#if WINFORMS
using System.Windows;
using System.Windows.Markup;
using OxyPlot;
using OxyPlot.WindowsForms;
using System.IO;
using Microsoft.Web.WebView2.Core;
#endif

#if WPF
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using MoreLinq;
using System.Windows.Markup;
using System.Xml.Linq;
using System.Xml;
using OxyPlot;
using OxyPlot.Wpf;
using System.Windows.Data;
using System.IO;
//using com.keasdon.messagereceiver;
using Microsoft.Web.WebView2.Wpf;
using MoreLinq.Extensions;
//using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
#endif

#if WINUI
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using OxyPlot;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI;
using System.IO;
#endif

#if ANDROIDX
using Android.OS;
//using Android.Widget;           // <= Get this out one day! That day arrived!!
using System.Xml.Linq;
using Android.Animation;
using System.Xml;
using Android.Content;
using Android.Views;
using OxyPlot.Xamarin.Android;
using OxyPlot;
using AndroidX.RecyclerView.Widget;
using static Android.Widget.AdapterView;
using static Android.Views.View;
using AndroidX.AppCompat.App;
using System.Runtime.Versioning;
using static SmartCubeMobile.SmartUtility;

#endif

#if SMARTMAUI
using System.Xml.Linq;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Dispatching;
#endif

namespace SmartCubeMobile
{
#if ANDROIDX

    [SupportedOSPlatform("android")]
#endif
    //#if WPF
    // https://justgivemeanexample.com/example/await-a-WINUI-storyboard-animation-in-c

    // usage:
    // var animator = new Animator(MyStoryboard);
    // await animator.Begin();

    //internal class Animator
    //{
    //    private TaskCompletionSource<bool> _completion;
    //    private .Xaml.Media.Animation.Storyboard _storyboard;

    //    internal Animator(.Xaml.Media.Animation.Storyboard storyboard)
    //    {
    //        _storyboard = storyboard;
    //        //storyboard.Completed += OnStoryboardCompleted;
    //    }

    //    internal Task Begin()
    //    {
    //        _completion = new TaskCompletionSource<bool>();
    //        _storyboard.Begin();
    //        return _completion.Task;
    //    }

    //    private void OnStoryboardCompleted(object send, object e)
    //    {
    //        //_storyboard.Completed -= OnStoryboardCompleted;
    //        _completion.SetResult(true);
    //    }
    //}

    // https://www.sharpgis.net/post/2012/09/05/Running-a-Storyboard-as-a-Task
    //    internal static class StoryboardExtensions
    //    {
    //        internal static Task BeginAsync(this .Xaml.Media.Animation.Storyboard storyboard)
    //        {
    //            System.Threading.Tasks.TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
    //            if (storyboard == nll)
    //                tcs.SetException(new ArgumentNullException());
    //            else
    //            {
    //#if WINFORMS || WPF
    //                EventHandler onComplete = nll;
    //#endif
    //                onComplete = (s, e) => {
    //                    //storyboard.Completed -= onComplete;
    //                    tcs.SetResult(true);
    //                };
    //                //storyboard.Completed += onComplete;
    //                storyboard.Begin();
    //            }
    //            return tcs.Task;
    //        }
    //    }
    //#endif


    //#if WINFORMS || WPF
    //    // Thanks to : http://www.tyrodeveloper.com/2012/04/color-in-combobox-item.html
    //    internal class ComboBoxItem
    //    {
    //        internal ComboBoxItem() { }

    //        internal ComboBoxItem(string myText, object myValue)
    //        {
    //            Text = myText; val = myValue;
    //        }

    //#if WINFORMS
    //        internal ComboBoxItem(string myText, object myValue, Color myColor)
    //        {
    //            Text = myText; val = myValue; foreColor = myColor;
    //        }
    //#Xelse
    //        internal ComboBoxItem(string myText, object myValue, Color myColor)
    //        {
    //            Text = myText; val = myValue; foreColor = new SolidColorBrush(myColor);
    //        }
    //#endif
    //        string Text = "";
    //        internal string Title
    //        {
    //            get { return Text; }
    //            set { Text = value; }
    //        }

    //        object val;
    //        internal object Value
    //        {
    //            get { return val; }
    //            set { val = value; }
    //        }

    //#if WINFORMS
    //        Color foreColor = Color.Black;
    //        internal Color Foreground
    //        {
    //            get { return foreColor; }
    //            set { foreColor = value; }
    //        }
    //#Xelse
    //        SolidColorBrush foreColor = new SolidColorBrush(Colors.Black);
    //        internal SolidColorBrush Foreground
    //        {
    //            get { return foreColor; }
    //            set { foreColor = value; }
    //        }
    //#endif

    //        internal override string ToString()
    //        {
    //            return Text;
    //        }
    //    }
    //#endif

    public static class StructExts
    {
        public static T Clone<T>(this T val) where T : struct => val;
    }

    //public static class FindListTable
    //{
    //    //https://stackoverflow.com/questions/19112922/sort-observablecollectionstring-through-c-sharp
    //    public static void Sort<T>(this List<T> list) where T : IComparable<T>
    //    {
    //        int index = 0;
    //        foreach (var item in list.OrderBy(x => x))
    //        {
    //            if (!item.Equals(list[index]))
    //            {
    //                list[index] = item;
    //            }
    //            index++;
    //        }
    //    }
    //}

    public class SmartRoutinesV2018
    {

        // Left in because they might come in useful one day ...!
        // (I need all the help I can get)
        //internal static XDocument LoadFromStream(Stream stream)
        //{
        //    using (XmlReader reader = XmlReader.Create(stream))
        //    {
        //        return XDocument.Load(reader);
        //    }
        //}

        //internal static XDocument LoadFromStreamNew(Stream stream)
        //{
        //    XmlReader reader = XmlReader.Create(stream);
        //    return XDocument.Load(reader);            
        //}

        internal static int GetNextRandomNew(System.Random random_r)
        {
            // Ok. Well. I was shaken up a bit when solving the "dollar in"
            // and "dollar out" problem which I had managed to introduce (!!)
            // which caused the insertion of two records with the same Random number
            // to fail!  Thinking about it - of they key is the same (which it might
            // well be - then we rely on the uniqueness of the random number
            // associated with it to be exactly that - random.  If it is the
            // same number as that associated with a previous key, then the
            // database will fall over as the keys + random are *not* unique.
            // There is a web page:
            // https://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
            // with a kind of solution which I am going to implement JUST to be
            // on the safe side, as its statistically possible to generate an
            // integer number within my range which IS the same i.e. a duplicate
            // Gets ints in the range -2,147,483,648 to 2,147,483,647
            return random_r.Next(SmartParametersV2016.randomLowerLimit, SmartParametersV2016.randomUpperLimit);
        }

        internal static int GetHashCodeNew()
        {
            // Ok. Well. I was shaken up a bit when solving the "dollar in"
            // and "dollar out" problem which I had managed to introduce (!!)
            // which caused the insertion of two records with the same Random number
            // to fail!  Thinking about it - of they key is the same (which it might
            // well be - then we rely on the uniqueness of the random number
            // associated with it to be exactly that - random.  If it is the
            // same number as that associated with a previous key, then the
            // database will fall over as the keys + random are *not* unique.
            // There is a web page:
            // https://stackoverflow.com/questions/14473321/generating-random-unique-values-c-sharp
            // with a kind of solution which I am going to implement JUST to be
            // on the safe side, as its statistically possible to generate an
            // integer number within my range which IS the same i.e. a duplicate
            // So I'm going to add in a HashCode based on a GetNextRandomNew (which aren't themselves
            // guaranteed unique, but hey ho) and also the HashCode which is an integer
            // might just possibly be the same as another ... but all I'm doing is
            // reducing (not eliminating) the probability of a duplicate.
            //
            // Returns an original HashCode which will be BEFORE KEY_DETAILS
            // in all the tables that need it
            return Guid.NewGuid().GetHashCode();
        }
        internal static List<LanguageDictionary> BuildLanguageDictionaries(string directoryPath)
        {
           List<LanguageDictionary> languageDictionaries
                    = new List<LanguageDictionary>();
            // Make sure the .xaml files have "Embedded Resource"
            // and "Copy always" attributes ...
            // With thanks to: https://www.wpfsharp.com/2012/01/26/how-to-load-a-dictionarystyle-xaml-file-at-run-time/
#if WINFORMS
            string Directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string targetPath = System.IO.Path.Combine(Directory, directoryPath);
            string[] fileEntries = System.IO.Directory.GetFiles(targetPath);
            string extension = ".xaml";

            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(extension))
                {
                    string lang_code = fileName.Replace(targetPath, "");
                    lang_code = lang_code.Replace(extension, "").Trim('\\');
                    LanguageDictionary language_dictionary = new LanguageDictionary()
                    {
                        //languageCode = lang_code,
                        //resourceDictionary = new Dictionary<"a","b">()//   LoadStringDictionaryFromFile(fileName)
                    };
                    languageDictionaries.Add(language_dictionary);
                }
            }
#endif
#if WINUI || MAUI
            string Directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string targetPath = System.IO.Path.Combine(Directory, directoryPath);
            string[] fileEntries = System.IO.Directory.GetFiles(targetPath);
            string extension = ".xml";
            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(extension))
                {
                    string lang_code = ""; // bobbins.Replace(@"SmartCubeMobile.Resources.values.", "");
                    int ffs = fileName.IndexOf("Resources");
                    if (ffs >= 0)
                    {
                        lang_code = fileName.Substring(ffs); // i.e. from ffs onwards
                        lang_code = lang_code.Replace("Resources", "");
                        lang_code = lang_code.Replace(extension, "").Trim('\\');
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        XmlReader reader = XmlReader.Create(fileName);
                        XDocument mybob = XDocument.Load(reader);

                        foreach (XElement element in mybob.Descendants("string"))
                        {
                            dictionary.Add(element.FirstAttribute.Value, element.Value);
                        }
                        LanguageDictionary language_dictionary = new LanguageDictionary()
                        {
                            languageCode = lang_code,
                            resourceDictionary = dictionary
                        };
                        languageDictionaries.Add(language_dictionary);
                    }               
                }
            }
#endif
#if WPF 
            string[] bob = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string bobbins in bob)
            {
                // I've left this next line IN - because (I think)
                // it's needed for SmartCubeMobileV2023
                string lang_code = ""; // bobbins.Replace(@"SmartCubeMobile.Resources.values.", "");
                int ffs = bobbins.IndexOf(".Resources.");
                if (ffs >= 0)
                {
                    lang_code = bobbins.Substring(ffs); // i.e. from ffs onwards
                    lang_code = lang_code.Replace(".Resources.", "");
                    lang_code = lang_code.Replace(@".xml", "");
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    XmlReader reader = XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream(bobbins));
                    XDocument mybob = XDocument.Load(reader);

                    foreach (XElement element in mybob.Descendants("string"))
                    {
                        dictionary.Add(element.FirstAttribute.Value, element.Value);
                    }
                    LanguageDictionary language_dictionary = new LanguageDictionary()
                    {
                        languageCode = lang_code,
                        resourceDictionary = dictionary
                    };
                    languageDictionaries.Add(language_dictionary);
                }
            }
#endif
#if ANDROIDX
            string resourcesValues = ".Resources.values.";
            string[] bob = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string bobbins in bob) // Checked
            {
                string lang_code = "";
                int ffs = bobbins.IndexOf(resourcesValues);
                if (ffs >= 0)
                {
                    lang_code = bobbins.Substring(ffs);
                    lang_code = lang_code.Replace(resourcesValues, "");
                    lang_code = lang_code.Replace(@".xml", "");

                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    XmlReader reader = XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream(bobbins));
                    XDocument mybob = XDocument.Load(reader);

                    foreach (XElement element in mybob.Descendants("string")) // Checked
                    {
                        dictionary.Add(element.FirstAttribute.Value, element.Value);
                    }
                    LanguageDictionary language_dictionary = new LanguageDictionary()
                    {
                        languageCode = lang_code,
                        resourceDictionary = dictionary
                    };
                    languageDictionaries.Add(language_dictionary);
                }
            }
#endif
#if SMARTMAUI
            string mauiDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string mauiTargetPath = System.IO.Path.Combine(mauiDir, directoryPath);
            if (System.IO.Directory.Exists(mauiTargetPath))
            {
                string[] mauiFiles = System.IO.Directory.GetFiles(mauiTargetPath);
                string mauiExtension = ".xml";
                foreach (string fileName in mauiFiles)
                {
                    if (fileName.Contains(mauiExtension))
                    {
                        string lang_code = "";
                        int ffs = fileName.IndexOf("Resources");
                        if (ffs >= 0)
                        {
                            lang_code = fileName.Substring(ffs);
                            lang_code = lang_code.Replace("Resources", "");
                            lang_code = lang_code.Replace(mauiExtension, "").Trim('\\');
                            Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            System.Xml.XmlReader reader = System.Xml.XmlReader.Create(fileName);
                            XDocument mybob = XDocument.Load(reader);
                            foreach (XElement element in mybob.Descendants("string"))
                            {
                                dictionary.Add(element.FirstAttribute.Value, element.Value);
                            }
                            LanguageDictionary language_dictionary = new LanguageDictionary()
                            {
                                languageCode = lang_code,
                                resourceDictionary = dictionary
                            };
                            languageDictionaries.Add(language_dictionary);
                        }
                    }
                }
            }
#endif

            return languageDictionaries;
        }

        // <summary>
        // This function loads a ResourceDictionary from a file at runtime
        // </summary>

#if WPF 
        internal static ResourceDictionary LoadStringDictionaryFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
#if WINFORMS
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        //return XamlReader.Load(fs) as ResourceDictionary;
                    }
#endif
#if WPF
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return XamlReader.Load(fs) as ResourceDictionary;
                    }
#endif
                }
                catch (Exception ex)
                {
                    System.Console.Write(ex.Message);
                }
            }
#if WPF
            return new ResourceDictionary();
#endif
        }
#endif

        internal static async Task<bool> ProfileModule(
#if WINFORMS
                                    SmartDashboard.MainProcess components,
#endif
#if ANDROIDX
                                    AppCompatActivity meterActivity,
                                    Context meterContext,
#endif
                                    SignInViewModel signinviewmodel,
                                    MainViewModel ourviewmodel,
                                    ProfilesViewModel profileviewmodel,
                                    List<object> vmlist,
                                    char cubeface_code,
                                    string culture_code,
                                    string face_last_display)
        {
            try
            {
                if (!CubefaceViewExists(ourviewmodel, cubeface_code))
                {
#if ANDROIDX
                    LayoutInflater inflater = meterActivity.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
#endif
#if WINFORMS
                components.tabControlCategories.Controls.Add(components.CategoryP);

                components.ProfilesBindingSource = new();
                components.ProfilesBindingSource.DataSource = profileviewmodel;

                components.textBoxProfileEmailAddress.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "EmailAddress", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileContactNo.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "ContactNo", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileGender.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "Gender", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileDateOfBirth.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "DateOfBirth", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileTitle.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileMiddlename.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileFirstname.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileLastname.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "LastName", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileDisplayName.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "DisplayName", true, DataSourceUpdateMode.OnPropertyChanged));
                components.textBoxProfileUsername.DataBindings.Add(new Binding("Text", components.ProfilesBindingSource, "PUserName", true, DataSourceUpdateMode.OnPropertyChanged));


#endif
#if WPF  || WINUI
                    SmartUsers.ConsumerViews profileView = new SmartUsers.ConsumerViews()
                {
                    CUBEFACE_CODE = cubeface_code,
                    View = new ProfilesView(signinviewmodel, ourviewmodel, profileviewmodel, vmlist)
                    {
                        DataContext = profileviewmodel
                    }
                };
#endif
#if ANDROIDX
                    SmartUsers.ConsumerViews profileView = new SmartUsers.ConsumerViews()
                    {
                        CUBEFACE_CODE = cubeface_code,
                        View = inflater.Inflate(Resource.Layout.ProfilesView, null)
                    };
#endif
#if SMARTMAUI
                    SmartUsers.ConsumerViews profileView = new SmartUsers.ConsumerViews()
                    {
                        CUBEFACE_CODE = cubeface_code,
                        View = new ProfilesView(signinviewmodel, ourviewmodel, profileviewmodel, vmlist)
                        {
                            BindingContext = profileviewmodel
                        }
                    };
#endif
                    await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " view");
                    // Token setting
                    // Isn't this true by default??
                    //ourviewmodel.loadRemote[ourviewmodel.screenCode] = false;
#if ANDROIDX
                    //profileviewmodel.context = ourviewmodel.context;

                    meterActivity.RunOnUiThread(() =>
                    {
                        profileviewmodel.SaveButton = profileView.View.FindViewById<Button>(Resource.Id.SaveButton);
                        //profileviewmodel.SaveButton.Click += new EventHandler((s, e) => ProfilesView.ProfileSave(s, e, ourviewmodel, profileviewmodel));

                        // Next time! => DON'T try to convert a TextView to an EditText!!!!
                        profileviewmodel.PUserName = profileView.View.FindViewById<TextView>(Resource.Id.UserName);

                        profileviewmodel.DisplayName = profileView.View.FindViewById<EditText>(Resource.Id.DisplayName);
                        profileviewmodel.LastName = profileView.View.FindViewById<EditText>(Resource.Id.LastName);
                        profileviewmodel.MiddleName = profileView.View.FindViewById<EditText>(Resource.Id.MiddleName);
                        profileviewmodel.FirstName = profileView.View.FindViewById<EditText>(Resource.Id.FirstName);

                        profileviewmodel.DateOfBirth = profileView.View.FindViewById<EditText>(Resource.Id.DateOfBirth);
                        profileviewmodel.Title = profileView.View.FindViewById<EditText>(Resource.Id.Title);
                        profileviewmodel.Gender = profileView.View.FindViewById<EditText>(Resource.Id.Gender);

                        profileviewmodel.ContactNo = profileView.View.FindViewById<EditText>(Resource.Id.ContactNo);
                        profileviewmodel.EmailAddress = profileView.View.FindViewById<EditText>(Resource.Id.EmailAddress);

                        profileviewmodel.cubefacesRecyclerView = profileView.View.FindViewById<RecyclerView>(Resource.Id.profileCubefaces);

                        profileviewmodel.groupsRecyclerView = profileView.View.FindViewById<RecyclerView>(Resource.Id.profileGroups);

                        // Get the button for adding a Group:
                        profileviewmodel.AddButton = profileView.View.FindViewById<Button>(Resource.Id.AddButton);
                    });
                    // When (if??) we ever get the Groups working??? WE DID!!! 10-May-2024!!!
                    // we do it by a RecyclerView (fuck knows why)
#endif

                    if (!await SmartProfilesV2023.Start_Profiles_Display(
#if WINFORMS
                                                                components,
#endif
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                signinviewmodel,
                                                                ourviewmodel,
                                                                profileviewmodel,
                                                                cubeface_code,
                                                                face_last_display))
                    {
                        // SOMETHING has gone amiss ...
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            ourviewmodel.errorMessage = "Problem 51: " + "Start Profile Display failed";
                            if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        return false;
                    }


#if WPF  || WINUI || SMARTMAUI
                    ourviewmodel.viewCollection.Add(profileView);
#endif
#if ANDROIDX
                    ourviewmodel.viewCollection.Add(profileView);
#endif
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        internal static async Task<bool> UnLoadProfileModule(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                ProfilesViewModel profileviewmodel,
                                                char cubeface_code)
        {
            //profileviewmodel.SaveButton.Click -= new EventHandler((s, e) => ProfilesView.ProfileSave(s, e, ourviewmodel, profileviewmodel));            
            await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " removed");
            return true;
        }

        internal static async Task<bool> LoadFinanceModule(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                char cubeface_code,
                                                string culture_code,
                                                short faceCurrency,
                                                string face_last_display,
                                                DateTime next_connection)
        {
#if WINFORMS
            // One day maybe ...
            // However, the financeviewmodel SHOULD be available here
            //=============== Finance ======================
            // 
            // FinanceBindingSource
            //
            components.FinanceBindingSource = new();
            components.FinanceBindingSource.DataSource = financeviewmodel;
            components.financeInstitutionsListBindingSource = new();
            components.financeProvidersListBindingSource = new();
            components.financeAccountsListBindingSource = new();
            components.FinanceTransactionGroupsListBindingSource = new();
            components.financeAddressesListBindingSource = new();
            components.financeCulturesListBindingSource = new();

            components.FinanceCurrGBP.DataBindings.Add(new Binding("Text", components.MainBindingSource, "GBPRate", true));
            components.FinanceCurrEUR.DataBindings.Add(new Binding("Text", components.MainBindingSource, "EURRate", true));
            components.FinanceCurrUSD.DataBindings.Add(new Binding("Text", components.MainBindingSource, "USDRate", true));
            components.FinanceCurrJPY.DataBindings.Add(new Binding("Text", components.MainBindingSource, "JPYRate", true));

            components.FinanceRBGBP.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "FinanceConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceRBEUR.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "FinanceConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceRBUSD.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "FinanceConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceRBJPY.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "FinanceConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));

            components.FinanceNextConnection.DataBindings.Add(new("Text", components.FinanceBindingSource, "NextConnectionMessage", true, DataSourceUpdateMode.OnPropertyChanged));

            components.FinanceStartDate.DataBindings.Add(new("Value", components.FinanceBindingSource, "StartDate", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceStartDate.DataBindings.Add(new("Enabled", components.FinanceBindingSource, "StartDateEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceEndDate.DataBindings.Add(new("Value", components.FinanceBindingSource, "EndDate", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceEndDate.DataBindings.Add(new("Enabled", components.FinanceBindingSource, "EndDateEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceResultStartDate.DataBindings.Add(new("Text", components.FinanceBindingSource, "ResultStartDate", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceResultEndDate.DataBindings.Add(new("Text", components.FinanceBindingSource, "ResultEndDate", true, DataSourceUpdateMode.OnPropertyChanged));

            components.FinanceAreaId.DataBindings.Add(new("Text", components.FinanceBindingSource, "AreaId", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceAreaId.DataBindings.Add(new("ForeColor", components.FinanceBindingSource, "AreaIdBackground", true, DataSourceUpdateMode.OnPropertyChanged));

            // 
            // financeInstitutionsListBindingSource
            // 
            components.comboBoxGUIFinanceInstitutions.DataSource = components.financeInstitutionsListBindingSource;
            components.financeInstitutionsListBindingSource.DataMember = "FinanceInstitutionsList";
            components.financeInstitutionsListBindingSource.DataSource = components.FinanceBindingSource;

            // 
            // financeProvidersListBindingSource
            //
            components.comboBoxGUIFinanceProviders.DataSource = components.financeProvidersListBindingSource;
            components.financeProvidersListBindingSource.DataMember = "FinanceProvidersList";
            components.financeProvidersListBindingSource.DataSource = components.FinanceBindingSource;

            // 
            // financeAccountsListBindingSource
            components.comboBoxGUIFinanceAccounts.DataSource = components.financeAccountsListBindingSource;
            components.financeAccountsListBindingSource.DataMember = "FinanceAccountsList";
            components.financeAccountsListBindingSource.DataSource = components.FinanceBindingSource;

            // 
            // FinanceTransactionGroupsListBindingSource
            components.comboBoxGUIFinanceTransactionGroups.DataSource = components.FinanceTransactionGroupsListBindingSource;
            components.FinanceTransactionGroupsListBindingSource.DataMember = "FinanceTransactionGroupsList";
            components.FinanceTransactionGroupsListBindingSource.DataSource = components.FinanceBindingSource;
            //components.comboBoxGUIFinanceTransactionGroups.DataSource = financeFormModel.FinanceTransactionGroupsList;

            // 
            // financeAddressesListBindingSource
            // 
            components.comboBoxGUIFinanceAddresses.DataSource = components.financeAddressesListBindingSource;
            components.financeAddressesListBindingSource.DataMember = "FinanceAddressesList";
            components.financeAddressesListBindingSource.DataSource = components.FinanceBindingSource;
            
            components.RBBanks.DataBindings.Add(new Binding("Tag", components.FinanceBindingSource, "ButtonBanks", true, DataSourceUpdateMode.OnPropertyChanged));
            components.RBBanks.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "CategoriesEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.RBSavings.DataBindings.Add(new Binding("Tag", components.FinanceBindingSource, "ButtonSavings", true, DataSourceUpdateMode.OnPropertyChanged));
            components.RBSavings.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "CategoriesEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.RBInvestments.DataBindings.Add(new Binding("Tag", components.FinanceBindingSource, "ButtonInvestments", true, DataSourceUpdateMode.OnPropertyChanged));
            components.RBInvestments.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "CategoriesEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.RBCryptos.DataBindings.Add(new Binding("Tag", components.FinanceBindingSource, "ButtonCryptos", true, DataSourceUpdateMode.OnPropertyChanged));
            components.RBCryptos.DataBindings.Add(new Binding("Enabled", components.FinanceBindingSource, "CategoriesEnabled", true, DataSourceUpdateMode.OnPropertyChanged));

            components.FinancePostCode.DataBindings.Add(new Binding("Text", components.FinanceBindingSource, "PostCode", true, DataSourceUpdateMode.OnPropertyChanged));
            components.pictureBoxFinanceLogo.DataBindings.Add(new Binding("Image", components.FinanceBindingSource, "PictureBoxLOGO", true, DataSourceUpdateMode.OnPropertyChanged));

            components.TransactionsDataGrid.DataSource = components.FinanceBindingSource;

            components.BankingSite.DataBindings.Add(new Binding("Text", components.MainBindingSource, "BankingSite", true, DataSourceUpdateMode.OnPropertyChanged));
            components.Banking_Brands.DataBindings.Add(new Binding("Text", components.financeInstitutionsListBindingSource, "Content", true, DataSourceUpdateMode.OnPropertyChanged));

            components.FinanceUsername.DataBindings.Add(new Binding("Text", components.FinanceBindingSource, "Username", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceSortCode.DataBindings.Add(new Binding("Text", components.FinanceBindingSource, "SortCode", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceAccountNo.DataBindings.Add(new Binding("Text", components.FinanceBindingSource, "AccountNo", true, DataSourceUpdateMode.OnPropertyChanged));
            components.FinanceTotalTransactions.DataBindings.Add(new Binding("Text", components.FinanceBindingSource, "TotalTransactions", true, DataSourceUpdateMode.OnPropertyChanged));
            
            components.buttonFinanceSubmit.Click += new EventHandler((s, e) => components.buttonFinanceSubmit_Click(s, e, ourviewmodel,financeviewmodel));
#endif

            try
            {
                if (!ourviewmodel.SMARTFINANCE)
                {
                    financeviewmodel.PLO = new SmartFinanceList();
                    // Set this now and forever whilst we run this application
                    if (!await SmartNibbyV2016.MiserableFuckingCow(ourviewmodel,
                                                                financeviewmodel,
                                                                cubeface_code,
                                                                SmartParametersV2016.TotalTables,
                                                                SmartParametersV2016.SmartFinanceSchema.ToUpper(),
                                                                SmartParametersV2016.wildcard))
                    {
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            // Set the fourth Led to Red ... and duck out
                            FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                            if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 47: loading " + cubeface_code))
                            {
                                return false;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        await TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    "Cubeface " + cubeface_code + " Data");
                    }
                }

                if (ourviewmodel.tablesCreated > 0)
                {
                    if (!await SmartPhyllV2020.LoadSmartFinance(ourviewmodel,
                                                            ourviewmodel.screenCode,
                                                            SmartParametersV2016.SmartFinanceSchema,
                                                            ourviewmodel.UserName,
                                                            ourviewmodel.UserName))// For consistency
                    {
                        // Never happens in SmartPhyll as there is no reference to SmartBob
                        // Set the fourth Led to Red ... and duck out
                        FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                        if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 48: loading PLO " + financeviewmodel.errorMessage))
                        {
                            return false;
                        }
                        return false;
                    }
                }
                                    
                //int ssindex = 0;
                bool primary = true;
                if (!await SmartPhyllV2020.ExtractSmartFinance(ourviewmodel,
                                            financeviewmodel,
                                            ourviewmodel.sqlitetablesList,
                                            ourviewmodel.screenCode,
                                            primary,
                                            SmartParametersV2016.SmartFinanceSchema,
                                            ourviewmodel.UserName,
                                            ourviewmodel.UserName,
                                            ourviewmodel.FDEK))
                {
                    // Set the fourth Led to Red ... and duck out
                    FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                    if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 49: extracting PLO " + financeviewmodel.errorMessage))
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Loaded SmartFinances: " + ourviewmodel.UserName);
                    primary = false;
                }

//                foreach (var groupRow in ourviewmodel.SmartProfile.profilegroupsList.Where(g => g.RECEIVEALL))
//                {
//                    string groupName = groupRow.GROUPNAME;

//                    string schemas = string.Join(
//                        SmartParametersV2016.unitSeparator,
//                        SmartParametersV2016.SmartUsersSchema,
//                        SmartParametersV2016.SmartProfileSchema,
//                        SmartParametersV2016.SmartFinanceSchema);

//                    if (!await SmartNibbyV2016.MiserableBitch(
//                            signinviewmodel,
//                            ourviewmodel,
//                            ourviewmodel.UserName,
//                            groupName,
//                            schemas))
//                    {
//                        await Fail(
//#if ANDROIDX
//                            meterActivity,
//#endif
//                            ourviewmodel, "Miserable Bitch failed 4 " + ourviewmodel.errorMessage, 8);
//                        return false;
//                    }

//                    if (!await SmartPhyllV2020.ExtractSmartUsersX(
//                            ourviewmodel,
//                            ourviewmodel.sqlitetablesList,
//                            primary,
//                            SmartParametersV2016.SmartUsersSchema,
//                            ourviewmodel.UserName,
//                            groupName))
//                    {
//                        await Fail(
//#if ANDROIDX
//                            meterActivity,
//#endif
//                            ourviewmodel, "Problem 17: SQLite failed", 8);
//                        return false;
//                    }

//                    await TextBlockUpdate(
//#if ANDROIDX
//                                            meterActivity,
//#endif
//                                            ourviewmodel, $"Loaded SmartUsers: {groupName}");

//                    if (!await SmartPhyllV2020.ExtractSmartProfile(
//                            ourviewmodel,
//                            ourviewmodel.sqlitetablesList,
//                            ourviewmodel.screenCode,
//                            primary,
//                            SmartParametersV2016.SmartProfileSchema,
//                            groupName,
//                            ""))
//                    {
//                        await Fail(
//#if ANDROIDX
//                            meterActivity,
//#endif
//                            ourviewmodel, "Problem 49: extracting PLO " + financeviewmodel.errorMessage, 4);
//                        return false;
//                    }

//                    await TextBlockUpdate(
//#if ANDROIDX
//                                            meterActivity,
//#endif
//                                            ourviewmodel, $"Loaded SmartProfile: {groupName}");

//                    var profileGroup = ourviewmodel.SmartProfile.profilegroupsList
//                        .FirstOrDefault(g => g.USERNAME == groupName);

//                    if (profileGroup == null)
//                        continue;

//                    if (!await SmartPhyllV2020.ExtractSmartFinance(
//                            ourviewmodel,
//                            financeviewmodel,
//                            ourviewmodel.sqlitetablesList,
//                            ourviewmodel.screenCode,
//                            primary,
//                            SmartParametersV2016.SmartFinanceSchema,
//                            groupRow.USERNAME,
//                            groupName,
//                            profileGroup.FDEK))
//                    {
//                        await Fail(
//#if ANDROIDX
//                            meterActivity,
//#endif
//                            ourviewmodel, "Problem 49: extracting PLO " + financeviewmodel.errorMessage, 4);
//                        return false;
//                    }

//                    await TextBlockUpdate(
//#if ANDROIDX
//                                            meterActivity,
//#endif
//                                            ourviewmodel, $"Loaded SmartFinances: {groupName}");
//                }

                financeviewmodel.FinanceCultureCode = culture_code;
                financeviewmodel.CultureINF = SmartSpikeV2017.Find_CULTURE_Code(signinviewmodel.Fatah.culturesList,
                                                                                culture_code);
                await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Finance culture set to: " + culture_code);
                financeviewmodel.LastCultureSelectedIndex = SmartSpikeV2017.Lookup_CULTURE_Index(ourviewmodel.cultureviewList,
                                                                culture_code);

                if (!CubefaceViewExists(ourviewmodel, cubeface_code))
                {
#if ANDROIDX
                    // Huge problem fixed inflating FinanceView which had
                    // <com.google.android.material.tabs.TabLayout> inside it
                    // Thanks to Chanh at https://stackoverflow.com/questions/51204733/error-inflating-class-com-google-android-material-tabs-tablayout
                    // who told me to put a fucking theme inside it! Something
                    // I'd never needed before but ... hey ho.. this Android Shite
                    // is forever pulling the rug out from under my feet
                    LayoutInflater inflater = LayoutInflater.From(meterActivity);
#endif
                    // We need this View whether its active or not
#if WINFORMS
                    components.tabControlCategories.Controls.Add(components.CategoryF);
#endif
#if WPF  || WINUI
                    SmartUsers.ConsumerViews financeView = new SmartUsers.ConsumerViews()
                    {
                        CUBEFACE_CODE = cubeface_code,
                        View = new FinanceView(signinviewmodel, ourviewmodel, financeviewmodel)
                        {
                            DataContext = financeviewmodel
                        }
                    };
#endif
#if ANDROIDX
                    SmartUsers.ConsumerViews financeView = new SmartUsers.ConsumerViews()
                    {
                        CUBEFACE_CODE = cubeface_code,
                        View = inflater.Inflate(Resource.Layout.FinanceView, null)
                    };
#endif
#if SMARTMAUI

                    SmartUsers.ConsumerViews financeView = new SmartUsers.ConsumerViews()
                    {
                        CUBEFACE_CODE = cubeface_code,
                        View = new FinanceView(signinviewmodel, ourviewmodel, financeviewmodel)
                        {
                            BindingContext = financeviewmodel
                        }
                    };
#endif

#if WINFORMS
                    financeviewmodel.TabControlPanel = FrontEndGUI.FindTabControl(components, "CurrencyPanel");
#endif
#if WPF
                    financeviewmodel.TabControlPanel = FrontEndGUI.FindTabControl(financeView.View, "CurrencyPanel");
#endif
#if WINUI
                    financeviewmodel.TabControlPanel = FrontEndGUI.FindTabControl(financeView.View, "CurrencyPanel");
#endif
#if SMARTMAUI
                    //financeviewmodel.TabControlPanel = FrontEndGUI.FindTabControl(financeView.View, "CurrencyPanel");
#endif
                    // It appears ... if you want to add
                    // FinanceView to viewCollections then
                    // there is a Binding for FinanceLanguage
                    // that needs to be honoured!
                    // You MUST honour the binding for FinanceLanguage before adding this!
                    FrontEndGUI.SetFinanceLanguage(financeviewmodel, financeviewmodel.CultureINF);

#if WPF  || WINUI || SMARTMAUI
                    // You MUST honour the binding for FinanceLanguage before adding this!
                    ourviewmodel.viewCollection.Add(financeView);
#endif
#if ANDROIDX
                    // You MUST honour the binding for FinanceLanguage before adding this!
                    ourviewmodel.viewCollection.Add(financeView);

                    SmartFinanceV2025.SetUpFinanceViewPager(SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), ourviewmodel.Fm, ourviewmodel.Lfc, ourviewmodel, financeviewmodel);
                    SmartFinanceV2025.SetUpFinanceCurrencyPager(SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), ourviewmodel.Fm, ourviewmodel.Lfc, financeviewmodel);

#endif                    
                    financeviewmodel.CurrencyOrdinal = faceCurrency;

                    financeviewmodel.ConvertToSymbol = SmartSpikeV2017.Lookup_Currency_Symbol(ourviewmodel.currenciesList, financeviewmodel.CurrencyOrdinal);
                    await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Finance exchange rate set to: " +
                                            financeviewmodel.ConvertToSymbol);


                    await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " view");


#if ANDROIDX
#pragma warning disable CA1416
                    // Here we set the context for the Finance logos and the Address adapter
                    // ... and all the other adapters
                    //financeviewmodel.context = ourviewmodel.context;

                    meterActivity.RunOnUiThread(() =>
                    {
                        financeviewmodel.Submit = financeView.View.FindViewById<Button>(Resource.Id.Submit);
                        financeviewmodel.Cancel = financeView.View.FindViewById<Button>(Resource.Id.Cancel);
                        financeviewmodel.NextConnectionMessage = financeView.View.FindViewById<TextView>(Resource.Id.NextConnectionMessage);
                        financeviewmodel.FinanceInstitutions = financeView.View.FindViewById<Spinner>(Resource.Id.institutionsSpinner);
                        financeviewmodel.FinanceProviders = financeView.View.FindViewById<MultiSpinner>(Resource.Id.providersSpinner);
                        financeviewmodel.FinanceAccounts = financeView.View.FindViewById<MultiSpinner>(Resource.Id.accountsSpinner);
                        financeviewmodel.FinanceTransactionGroups = financeView.View.FindViewById<MultiSpinner>(Resource.Id.transactiongroupsSpinner);
                        // Categories
                        financeviewmodel.RBBanks = financeView.View.FindViewById<RadioButton>(Resource.Id.RBBanks);
                        financeviewmodel.RBSavings = financeView.View.FindViewById<RadioButton>(Resource.Id.RBSavings);
                        financeviewmodel.RBInvestments = financeView.View.FindViewById<RadioButton>(Resource.Id.RBInvestments);
                        financeviewmodel.RBCryptos = financeView.View.FindViewById<RadioButton>(Resource.Id.RBCryptos);
                        financeviewmodel.LedSwitched = financeView.View.FindViewById<TextView>(Resource.Id.LedSwitched);
                        financeviewmodel.SortCode = financeView.View.FindViewById<TextView>(Resource.Id.SortCodeDynamic);
                        financeviewmodel.AccountNo = financeView.View.FindViewById<TextView>(Resource.Id.AccountNoDynamic);
                        financeviewmodel.AreaId = financeView.View.FindViewById<TextView>(Resource.Id.AreaIdDynamic);
                        // Addresses
                        financeviewmodel.Addresses = financeView.View.FindViewById<Spinner>(Resource.Id.Addresses);
                        // Dates
                        financeviewmodel.StartDate = financeView.View.FindViewById<DatePicker>(Resource.Id.StartDate);
                        financeviewmodel.ResetDatesButton = financeView.View.FindViewById<Button>(Resource.Id.ResetDates);
                        financeviewmodel.EndDate = financeView.View.FindViewById<DatePicker>(Resource.Id.EndDate);
                        financeviewmodel.ResultStartDate = financeView.View.FindViewById<TextView>(Resource.Id.ResultStartDate);
                        financeviewmodel.ResultEndDate = financeView.View.FindViewById<TextView>(Resource.Id.ResultEndDate);
                        // Totals
                        financeviewmodel.TotalTransactions = financeView.View.FindViewById<TextView>(Resource.Id.TotalTransactions);
                        financeviewmodel.TotalValue = financeView.View.FindViewById<TextView>(Resource.Id.TotalValue);
                        // Last bit now Ray ..
                        financeviewmodel.PictureBoxLOGO = financeView.View.FindViewById<ImageView>(Resource.Id.pictureBoxLOGO);
                        // Postcode
                        financeviewmodel.PostCode = financeView.View.FindViewById<TextView>(Resource.Id.Postcode);
                        // Cultures
                        financeviewmodel.FinanceCultures = financeView.View.FindViewById<Spinner>(Resource.Id.financeCultures);
                        // Rate Conversions
                        financeviewmodel.FinanceRBNON = financeView.View.FindViewById<RadioButton>(Resource.Id.FinanceRBNON);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        financeviewmodel.FinanceRBGBP = financeView.View.FindViewById<RadioButton>(Resource.Id.FinanceRBGBP);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        financeviewmodel.GBPRate = financeView.View.FindViewById<TextView>(Resource.Id.GBPRate);
                        // 'Binding' is done in 'ourviewmodel' of all places!!                
                        financeviewmodel.FinanceRBEUR = financeView.View.FindViewById<RadioButton>(Resource.Id.FinanceRBEUR);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        financeviewmodel.EURRate = financeView.View.FindViewById<TextView>(Resource.Id.EURRate);
                        // 'Binding' is done in 'ourviewmodel' of all places!!                
                        financeviewmodel.FinanceRBUSD = financeView.View.FindViewById<RadioButton>(Resource.Id.FinanceRBUSD);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        financeviewmodel.USDRate = financeView.View.FindViewById<TextView>(Resource.Id.USDRate);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        financeviewmodel.FinanceRBJPY = financeView.View.FindViewById<RadioButton>(Resource.Id.FinanceRBJPY);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        financeviewmodel.JPYRate = financeView.View.FindViewById<TextView>(Resource.Id.JPYRate);
                    });
                    if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    {
                        financeviewmodel.Submit.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceSubmitTooltip");
                        financeviewmodel.Cancel.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceCancelTooltip");
                        // Spinners
                        financeviewmodel.FinanceInstitutions.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceInstitutionsTooltip");
                        financeviewmodel.FinanceProviders.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceProvidersTooltip");
                        financeviewmodel.FinanceAccounts.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceAccountsTooltip");
                        financeviewmodel.FinanceTransactionGroups.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceTransactionsTooltip");
                        financeviewmodel.RBBanks.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceBanksTooltip");
                        financeviewmodel.RBSavings.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceSavingsTooltip");
                        financeviewmodel.RBInvestments.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceInvestmentsTooltip");
                        financeviewmodel.RBCryptos.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceCryptosTooltip");
                        financeviewmodel.Addresses.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceAddressesTooltip");
                        financeviewmodel.StartDate.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceStartDateTooltip");
                        financeviewmodel.ResetDatesButton.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceResetDatesTooltip");
                        financeviewmodel.EndDate.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceEndDateTooltip");
                        financeviewmodel.ResultStartDate.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceResultStartDateTooltip");
                        financeviewmodel.ResultEndDate.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceResultEndDateTooltip");
                        financeviewmodel.PostCode.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinancePostCodeTooltip");
                        financeviewmodel.FinanceCultures.TooltipText = SignIn.BesetByChimps(signinviewmodel, "FinanceCultureTooltip");
                        financeviewmodel.FinanceRBNON.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        financeviewmodel.FinanceRBGBP.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        financeviewmodel.FinanceRBEUR.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        financeviewmodel.FinanceRBUSD.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        financeviewmodel.FinanceRBJPY.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                    }
                    financeviewmodel.Submit.Click += new EventHandler((s, e) => FinanceView.Finance_ButtonSubmitClick(s, e, meterActivity, ourviewmodel, financeviewmodel));
                    financeviewmodel.FinanceInstitutions.ItemSelected += new EventHandler<ItemSelectedEventArgs>((s, e) => FinanceView.FinanceInstitutions_SelectionChanged(s, e, ourviewmodel, financeviewmodel));
                    //financeviewmodel.FinanceInstitutions.Touch += new EventHandler<TouchEventArgs>((s, e) => FinanceView.FinanceMouseRightButtonDown(s, e, ourviewmodel, financeviewmodel));
                    // Here it is!
                    financeviewmodel.FinanceInstitutions.LongClick += new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_InstitutionMouseDoubleClick(s, e, meterActivity, ourviewmodel, financeviewmodel));
                    // Do *not* set the StartDate DateTime here!
                    meterActivity.RunOnUiThread(() =>
                    {
                        financeviewmodel.StartDate.DateTime = SmartParametersV2016.defaultDate;
                        // Do *not* set the EndDate DateTime here!
                        financeviewmodel.EndDate.DateTime = SmartParametersV2016.defaultMaxdate;
                    });
                    // Configure the date events
                    financeviewmodel.StartDate.DateChanged += new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => FinanceView.StartingDateChanged(s, e, ourviewmodel, financeviewmodel));
                    financeviewmodel.ResetDatesButton.Click += new EventHandler((s, e) => FinanceView.Finance_ResetDates(s, e, ourviewmodel, financeviewmodel));
                    financeviewmodel.EndDate.DateChanged += new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => FinanceView.EndingDateChanged(s, e, ourviewmodel, financeviewmodel));
                    // This DOES deserver a fucking Scotch!  Worked first time!!!!
                    // Unfuckingbelieveable!!!!
                    financeviewmodel.FinanceCultures.OnItemSelectedListener = new FinanceView.CultureItemSelectedListener(meterActivity, ourviewmodel, financeviewmodel);

                    financeviewmodel.GBPRate.Text = ourviewmodel.GBPRate;
                    financeviewmodel.EURRate.Text = ourviewmodel.EURRate;
                    financeviewmodel.USDRate.Text = ourviewmodel.USDRate;
                    financeviewmodel.JPYRate.Text = ourviewmodel.JPYRate;

#pragma warning restore CA1416
#endif

                    if (!await SmartFinanceV2025.Start_Finance_Display(
#if WINFORMS
                                                                components,
#endif
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                next_connection,
                                                                cubeface_code,
                                                                face_last_display))
                    {
                        // SOMETHING has gone amiss ...
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            ourviewmodel.errorMessage = "Problem 50: " + "Start Finance Display failed";
                            if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        return false;
                    }                    
                }

            }
            catch (Exception ex)
            {
                ourviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        internal static async Task Fail(
#if ANDROIDX
                                AppCompatActivity meterActivity,
#endif
                                MainViewModel ourviewmodel,
                                string message, int led)
        {
            FrontEndGUI.SetLedColour(ourviewmodel, led, ourviewmodel.redColour);

            await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, message);

            await SmartRoutinesV2018.CheckTrace(
                ourviewmodel,
                ourviewmodel.quitCts.Token,
                0,
                0,
                message);
            return;
        }
        internal static async Task<bool> UnLoadFinanceModule(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                List<object> vmList,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                char cubeface_code)
        {
            ourviewmodel.SMARTFINANCE = false;

            if (!SmartPhyllV2020.RemoveSmartFinance(ourviewmodel,
                                                        financeviewmodel,
                                                        ourviewmodel.sqlitetablesList,
                                                        ourviewmodel.screenCode,
                                                        SmartParametersV2016.SmartFinanceSchema,
                                                        ourviewmodel.UserName))// For consistency
            {
                // Never happens in SmartPhyll as there is no reference to SmartBob
                // Set the fourth Led to Red ... and duck out
                FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 51: loading PLO " + financeviewmodel.errorMessage))
                {
                    return false;
                }
                return false;
            }

            FinanceView financeView = ourviewmodel.viewCollection
                .Select(x => x.View)
                .OfType<FinanceView>()
                .FirstOrDefault();
            financeView.FinanceCleanup();
            
            // See the notes
            // I've given up trying to remove the UserControl because
            // a) the OxyPlot shit won't let me create a new Plotview and
            
#if WPF
            //financeviewmodel.Submit.Click -= new RoutedEventHandler((s, e) => FinanceView.Finance_ButtonSubmitClick(s, e, ourviewmodel, financeviewmodel));
            
            
            
            //financeviewmodel.FinanceInstitutions.ItemSelected -= new EventHandler<ItemSelectedEventArgs>((s, e) => FinanceView.FinanceInstitutions_SelectionChanged(s, e, ourviewmodel, financeviewmodel));
            // Here it is!
            //financeviewmodel.FinanceInstitutions.LongClick -= new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_InstitutionMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
            // Configure the date events

#endif
#if ANDROIDX
            financeviewmodel.Submit.Click -= new EventHandler((s, e) => FinanceView.Finance_ButtonSubmitClick(s, e, meterActivity, ourviewmodel, financeviewmodel));
            financeviewmodel.FinanceInstitutions.ItemSelected -= new EventHandler<ItemSelectedEventArgs>((s, e) => FinanceView.FinanceInstitutions_SelectionChanged(s, e, ourviewmodel, financeviewmodel));
            //financeviewmodel.FinanceInstitutions.Touch += new EventHandler<TouchEventArgs>((s, e) => FinanceView.FinanceMouseRightButtonDown(s, e, ourviewmodel, financeviewmodel));
            // Here it is!
            financeviewmodel.FinanceInstitutions.LongClick -= new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_InstitutionMouseDoubleClick(s, e, meterActivity, ourviewmodel, financeviewmodel));
            // Configure the date events
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                financeviewmodel.StartDate.DateChanged -= new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => FinanceView.StartingDateChanged(s, e, ourviewmodel, financeviewmodel));
                financeviewmodel.ResetDatesButton.Click -= new EventHandler((s, e) => FinanceView.Finance_ResetDates(s, e, ourviewmodel, financeviewmodel));
                financeviewmodel.EndDate.DateChanged -= new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => FinanceView.EndingDateChanged(s, e, ourviewmodel, financeviewmodel));
            }
#endif

            // Remove the viewmodel from vmList
            vmList.RemoveAll(x => x is FinanceViewModel);
            // Remove the view itself from Consumers
            List<SmartUsers.ConsumerViews> viewtoremove = ourviewmodel.viewCollection
                                .Where(x => x.View is FinanceView)
                                .ToList();
            foreach (SmartUsers.ConsumerViews item in viewtoremove)
            {
                ourviewmodel.viewCollection.Remove(item);
            }


            await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " removed");
            return true;
        }

        internal static async Task<bool> LoadInsuranceModule(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                char cubeface_code,
                                                string culture_code)
        {
#if WINFORMS
            //components.tabControlCategories.Controls.Add(components.CategoryP);
#endif

            if (!CubefaceViewExists(ourviewmodel, cubeface_code))
            {
                //SmartUsers.ConsumerViews insuranceView = new SmartUsers.ConsumerViews()
                //{
                //    CUBEFACE_CODE = cubeface_code,
                //    View = new InsuranceView()
                //};
                //ourviewmodel.viewCollection.Add(insuranceView);
            }
            else
            {
                //ourviewmodel.loadRemote[insuranceviewmodel.screenCode] = false;
            }

            await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " view");
            return true;
        }

        internal static async Task<bool> LoadUtilityModule(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char cubeface_code,
                                                string culture_code,
                                                short faceCurrency,
                                                string face_last_display,
                                                DateTime next_connection)
        {
#if WINFORMS

            //=============== Utility ======================
            components.UtilityBindingSource = new();
            components.UtilityBindingSource.DataSource = utilityviewmodel;
            components.utilitySuppliersListBindingSource = new();
            components.utilityTariffsListBindingSource = new();
            components.utilityPaymentPlansListBindingSource = new();
            components.utilityAddressesListBindingSource = new();
            components.utilityCulturesListBindingSource = new();

            components.UtilityNextConnection.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "NextConnectionMessage", true, DataSourceUpdateMode.OnPropertyChanged));

            // 
            // utilitySuppliersListBindingSource
            // 
            components.comboBoxGUIUtilitySuppliers.DataSource = components.utilitySuppliersListBindingSource;
            components.comboBoxGUIUtilitySuppliers.DataBindings.Add(new("SelectedItem", components.utilitySuppliersListBindingSource, "Content", true, DataSourceUpdateMode.OnPropertyChanged));
            components.utilitySuppliersListBindingSource.DataMember = "UtilitySuppliersList";
            components.utilitySuppliersListBindingSource.DataSource = components.UtilityBindingSource;

            // 
            // utilityTariffsListBindingSource
            //
            components.comboBoxGUIUtilityTariffs.DataSource = components.utilityTariffsListBindingSource;
            components.utilityTariffsListBindingSource.DataMember = "UtilityTariffsList";
            components.utilityTariffsListBindingSource.DataSource = components.UtilityBindingSource;

            // 
            // utilityPaymentPlansListBindingSource
            // 
            components.comboBoxGUIUtilityPaymentPlans.DataSource = components.utilityPaymentPlansListBindingSource;
            components.utilityPaymentPlansListBindingSource.DataMember = "UtilityPaymentPlansList";
            components.utilityPaymentPlansListBindingSource.DataSource = components.UtilityBindingSource;


            components.UtilityCurrGBP.DataBindings.Add(new Binding("Text", components.MainBindingSource, "GBPRate", true));
            components.UtilityCurrEUR.DataBindings.Add(new Binding("Text", components.MainBindingSource, "EURRate", true));
            components.UtilityCurrUSD.DataBindings.Add(new Binding("Text", components.MainBindingSource, "USDRate", true));
            components.UtilityCurrJPY.DataBindings.Add(new Binding("Text", components.MainBindingSource, "JPYRate", true));

            components.UtilityRBGBP.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "UtilityConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityRBEUR.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "UtilityConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityRBUSD.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "UtilityConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityRBJPY.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "UtilityConversionsEnabled", true, DataSourceUpdateMode.OnPropertyChanged));

            components.UtilityStartDate.DataBindings.Add(new Binding("Value", components.UtilityBindingSource, "StartDate", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityStartDate.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "StartDateEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityEndDate.DataBindings.Add(new Binding("Value", components.UtilityBindingSource, "EndDate", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityEndDate.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "EndDateEnabled", true, DataSourceUpdateMode.OnPropertyChanged));


            //components.labelNightRate.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "NightRate", true, DataSourceUpdateMode.OnPropertyChanged));
            //components.labelDayRate.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "DayRate", true, DataSourceUpdateMode.OnPropertyChanged));
            components.DayRate1.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "DayRate1", true, DataSourceUpdateMode.OnPropertyChanged));
            components.NightRate1.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "NightRate1", true, DataSourceUpdateMode.OnPropertyChanged));
            components.StandingCharge1.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "StandingCharge1", true, DataSourceUpdateMode.OnPropertyChanged));
            components.DayRate2.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "DayRate2", true, DataSourceUpdateMode.OnPropertyChanged));
            components.NightRate2.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "NightRate2", true, DataSourceUpdateMode.OnPropertyChanged));
            components.StandingCharge2.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "StandingCharge2", true, DataSourceUpdateMode.OnPropertyChanged));


            components.pictureBoxUtilityDNO.DataBindings.Add(new Binding("Image", components.UtilityBindingSource, "PictureBoxDNO", true, DataSourceUpdateMode.OnPropertyChanged));
            components.labelTotalReadings.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "TotalReadings", true, DataSourceUpdateMode.OnPropertyChanged));

            components.labelTotalCosts.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "TotalCost", true, DataSourceUpdateMode.OnPropertyChanged));
            components.labelEconomy7.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "Economy7State", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityAreaId.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "AreaId", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityAccountNo.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "AccountNo", true, DataSourceUpdateMode.OnPropertyChanged));
            components.UtilityPostCode.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "PostCode", true, DataSourceUpdateMode.OnPropertyChanged));

            // 
            // utilityAddressesListBindingSource
            // 
            components.comboBoxGUIUtilityAddresses.DataSource = components.utilityAddressesListBindingSource;
            components.utilityAddressesListBindingSource.DataMember = "UtilityAddressesList";
            components.utilityAddressesListBindingSource.DataSource = components.UtilityBindingSource;

            //
            // utilityCulturesListBindingSource
            //
            //components.utilityCulturesListBindingSource.DataMember = "UtilityCulturesList";
            //components.utilityCulturesListBindingSource.DataSource = components.UtilityBindingSource;
            //components.comboBoxGUIUtilityCultures.DataSource = components.utilityCulturesListBindingSource;
            //components.comboBoxGUIUtilityCultures.DataBindings.Add(new("SelectedItem", components.utilityCulturesListBindingSource, "Content", true, DataSourceUpdateMode.OnPropertyChanged));
            //components.comboBoxGUIUtilityCultures.SelectedIndexChanged += (s, e) => components.UtilityCultures_SelectedIndexChanged(s, e);

            components.URDualFuel.DataBindings.Add(new Binding("Tag", components.UtilityBindingSource, "ButtonDualFuel", true, DataSourceUpdateMode.OnPropertyChanged));
            components.URDualFuel.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "ResourcesEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.URGas.DataBindings.Add(new Binding("Tag", components.UtilityBindingSource, "ButtonGas", true, DataSourceUpdateMode.OnPropertyChanged));
            components.URGas.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "ResourcesEnabled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.URElectricity.DataBindings.Add(new Binding("Tag", components.UtilityBindingSource, "ButtonElectricity", true, DataSourceUpdateMode.OnPropertyChanged));
            components.URElectricity.DataBindings.Add(new Binding("Enabled", components.UtilityBindingSource, "ResourcesEnabled", true, DataSourceUpdateMode.OnPropertyChanged));

            components.labelUtilityTotalkWh.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "TotalkWh", true, DataSourceUpdateMode.OnPropertyChanged));
            components.labelUtilityTotalBills.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "TotalBilled", true, DataSourceUpdateMode.OnPropertyChanged));
            components.labelUtilityProjectedCost.DataBindings.Add(new Binding("Text", components.UtilityBindingSource, "ProjectedCost", true, DataSourceUpdateMode.OnPropertyChanged));

            components.CostsDataGrid.DataSource = components.UtilityBindingSource;
            components.BillsDataGrid.DataSource = components.UtilityBindingSource;
            components.ReadingsDataGrid.DataSource = components.UtilityBindingSource; ;
            components.BreakdownDataGrid.DataSource = components.UtilityBindingSource;

            components.buttonUtilitySubmit.Click += new EventHandler((s, e) => components.buttonUtilitySubmit_Click(s, e, ourviewmodel, utilityviewmodel));


#endif
            try
            {
                //utilityviewmodel.Hezbollah = new SmartUtilityList();
                // Set this now and forever whilst we run this application
                if (!ourviewmodel.SMARTUTILITY)
                {
                    if (!await SmartNibbyV2016.MiserableFuckingCow(ourviewmodel,
                                                                utilityviewmodel,
                                                                cubeface_code,
                                                                SmartParametersV2016.TotalTables,
                                                                SmartParametersV2016.SmartUtilitySchema.ToUpper(),
                                                                SmartParametersV2016.wildcard))
                    {
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            // Set the fourth Led to Red ... and duck out
                            FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                            if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 52: loading " + cubeface_code))
                            {
                                return false;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        await TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    "Cubeface " + cubeface_code + " Data");
                    }
                }

                if (ourviewmodel.tablesCreated > 0)
                {

                    if (!await SmartPhyllV2020.LoadSmartUtility(ourviewmodel,
                                                                ourviewmodel.screenCode,
                                                                SmartParametersV2016.SmartUtilitySchema,
                                                                ourviewmodel.UserName,
                                                                ourviewmodel.UserName))// For consistency
                    {
                        // Never happens in SmartPhyll as there is no reference to SmartBob
                        // Set the fourth Led to Red ... and duck out
                        FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                        if (!await CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, "Problem 53: loading Hezbollah " + utilityviewmodel.errorMessage))
                        {
                            return false;
                        }
                        return false;
                    }
                }

                int ssindex = 0;
                bool primary = true;
                foreach (SmartProfile.Groups mm_row in ourviewmodel.mmListX) // Checked
                {
                    if (mm_row.SENDU &&
                        mm_row.UDEK != "")
                    {
                        if (!await SmartPhyllV2020.ExtractSmartUtility(ourviewmodel,
                                utilityviewmodel,
                                ourviewmodel.sqlitetablesList,
                                ourviewmodel.screenCode,
                                primary,
                                SmartParametersV2016.SmartUtilitySchema,
                                mm_row.USERNAME,
                                mm_row.GROUPNAME,
                                mm_row.UDEK))
                        {
                            // Set the fourth Led to Red ... and duck out
                            FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                            if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 54: extracting PLO " + utilityviewmodel.errorMessage))
                            {
                                return false;
                            }
                            return false;
                        }
                        else
                        {
                            await TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, "Loaded SmartUtility: " + mm_row.GROUPNAME);
                        }
                    }
                    ssindex++;
                    if (ssindex > 0)
                    {
                        primary = false;
                    }
                }


                // Check to see if we have all two Resources in the User for Utility
                if (utilityviewmodel.Hezbollah.utility_resourcesList.Count == 0)
                {
                    int index = 0;
                    foreach (SmartUtility.ResourceCodes resource_codes_row in utilityviewmodel.Hezbollah.resource_codesList) // Checked
                    {
                        SmartUtility.Resources abc = new SmartUtility.Resources()
                        {
                            USERNAME = ourviewmodel.UserName,
                            CUBEFACE_CODE = cubeface_code,
                            RESOURCE_CODE = resource_codes_row.RESOURCE_CODE,
                            CHECKED = "",
                            TOTAL_USAGE = 0m,
                            UNITRATES_UPDATED = SmartParametersV2016.defaultDate,
                            LAST_DATETIME = SmartParametersV2016.defaultDate,
                            LAST_UPDATE = SmartParametersV2016.defaultDate,
                            EXPIRY_DATE = SmartParametersV2016.defaultDate,
                            AUTOSWITCH = SmartParametersV2016.defaultAutoSwitch,
                            Updated = false
                        };
                        if (index == 0)
                        {
                            abc.CHECKED = SmartParametersV2016.lastChecked;
                        }
                        utilityviewmodel.Hezbollah.utility_resourcesList.Add(abc);
                        utilityviewmodel.Hezbollah.utility_resources_changesList.Add(abc);
                        index++;
                    }
                    //if (utilityviewmodel.Hezbollah.utility_resources_changesList.Count > 0)
                    //{
                    //  /  utilityviewmodel.Hezbollah.utility_resources_changesList[0].CHECKED = SmartParametersV2016.lastChecked;
                    //}

                    if (!await SmartUtilityScrapeV2022.DoAllSmartUtility(ourviewmodel,
                                                    utilityviewmodel,
                                                    true,
                                                    SmartParametersV2016.sqliteformat,
                                                    0,
                                                    0))
                    {
                        if (!await CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, 0, 0, ourviewmodel.errorMessage))
                        {
                            return false;
                        }
                        //  Led8 turned to Red on return
                        return false;
                    }
                }

                utilityviewmodel.UtilityCultureCode = culture_code;
                utilityviewmodel.CultureINF = SmartSpikeV2017.Find_CULTURE_Code(signinviewmodel.Fatah.culturesList,
                                                                                culture_code);
                await TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, "Utility culture set to: " + culture_code);
                utilityviewmodel.LastCultureSelectedIndex = SmartSpikeV2017.Lookup_CULTURE_Index(ourviewmodel.cultureviewList,
                                                                    culture_code);


#if WINFORMS || WPF  || WINUI || SMARTMAUI
                ourviewmodel.WithdrawnDateMessage = "Tariffs and Prices as of: " + utilityviewmodel.withdrawn_date.ToString(signinviewmodel.signincultureinfo); // possibly non en-GB
#endif
                // Skip loading the Tariff info on an auto-scrape if we are behind because
                // it will be done again in check_meter unless set_skip is false
                // No ... the above is bollocks and has been re-done
                utilityviewmodel.tariffs_last_loaded = DateTime.Now + ourviewmodel.utcOffset;    // Local time
                                                                                                 // Its safe to turn on the Electricity, Duel Fuel and Gas 'No Display' radio buttons now


                if (!CubefaceViewExists(ourviewmodel, cubeface_code))
                {
#if ANDROIDX
                    LayoutInflater inflater = meterActivity.GetSystemService(Context.LayoutInflaterService) as LayoutInflater;
                    // Huge problem fixed inflating FinanceView which had
                    // <com.google.android.material.tabs.TabLayout> inside it
                    // Thanks to Chanh at https://stackoverflow.com/questions/51204733/error-inflating-class-com-google-android-material-tabs-tablayout
                    // who told me to put a fucking theme inside it! Something
                    // I'd never needed before but ... hey ho.. this Android Shite
                    // is forever pulling the rug out from under my feet
#endif
                    // We need this View whether its active or not
#if WINFORMS
                components.tabControlCategories.Controls.Add(components.CategoryU);
#endif
#if WPF  || WINUI || SMARTMAUI
                    SmartUsers.ConsumerViews utilityView = new SmartUsers.ConsumerViews()
                {
                    CUBEFACE_CODE = cubeface_code,
                    View = new UtilityView(signinviewmodel, ourviewmodel, utilityviewmodel)
                    {
#if WPF  || WINUI
                        DataContext = utilityviewmodel
#endif
#if SMARTMAUI
                        BindingContext = utilityviewmodel
#endif
                    }
                };
#endif
#if ANDROIDX
                SmartUsers.ConsumerViews utilityView = new SmartUsers.ConsumerViews()
                {
                    CUBEFACE_CODE = cubeface_code,
                    View = inflater.Inflate(Resource.Layout.UtilityView, null)
                };
#endif
#if WPF  || WINUI || SMARTMAUI
                utilityviewmodel.CostsDataGrid = FrontEndGUI.FindDataGrid(utilityView.View, "UtilityCostsGrid");
                utilityviewmodel.BillsDataGrid = FrontEndGUI.FindDataGrid(utilityView.View, "UtilityBillsGrid");
                utilityviewmodel.ReadingsDataGrid = FrontEndGUI.FindDataGrid(utilityView.View, "UtilityReadingsGrid");
                utilityviewmodel.BreakdownDataGrid = FrontEndGUI.FindDataGrid(utilityView.View, "UtilityBreakdownGrid");



#endif
                // It appears ... if you want to add
                // UtilityView to viewCollections then
                // there is a Binding for UtilityLanguage
                // that needs to be honoured!
                FrontEndGUI.SetUtilityLanguage(utilityviewmodel, utilityviewmodel.CultureINF);

#if WPF  || WINUI || SMARTMAUI
                    // You MUST honour the binding for UtilityLanguage before adding this!
                    ourviewmodel.viewCollection.Add(utilityView);
#endif
#if ANDROIDX
                    // You MUST honour the binding for UtilityLanguage before adding this!
                    ourviewmodel.viewCollection.Add(utilityView);
#endif
                    utilityviewmodel.CurrencyOrdinal = faceCurrency;

                    utilityviewmodel.ConvertToSymbol = SmartSpikeV2017.Lookup_Currency_Symbol(ourviewmodel.currenciesList, utilityviewmodel.CurrencyOrdinal);
                    await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Utility exchange rate set to: " +
                                            utilityviewmodel.ConvertToSymbol);

                    // Even enabling the drop-downs HERE ... the SelectedIndex event
                    // still gets called (what a pile of bollocks)
                    await TextBlockUpdate(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel,
                                                "Cubeface: " + cubeface_code + " view");


#if ANDROIDX
#pragma warning disable CA1416
                    // Here we set the context for the Finance logos and the Address adapter
                    // ... and all the other adapters
                    //utilityviewmodel.context = ourviewmodel.context;

                    meterActivity.RunOnUiThread(() =>
                    {
                        utilityviewmodel.TotalkWh = utilityView.View.FindViewById<TextView>(Resource.Id.TotalkWh);
                        utilityviewmodel.TotalBilled = utilityView.View.FindViewById<TextView>(Resource.Id.TotalBilled);
                        utilityviewmodel.ProjectedCost = utilityView.View.FindViewById<TextView>(Resource.Id.Projected);

                        utilityviewmodel.Submit = utilityView.View.FindViewById<Button>(Resource.Id.Submit);
                        utilityviewmodel.Cancel = utilityView.View.FindViewById<Button>(Resource.Id.Cancel);

                        utilityviewmodel.NextConnectionMessage = utilityView.View.FindViewById<TextView>(Resource.Id.NextConnectionMessage);

                        // Spinners
                        utilityviewmodel.Suppliers = utilityView.View.FindViewById<Spinner>(Resource.Id.Suppliers);
                        //utilityviewmodel.Suppliers.ItemSelected += UtilityView.UtilitySuppliers_SelectionChanged;
                        utilityviewmodel.Tariffs = utilityView.View.FindViewById<Spinner>(Resource.Id.Tariffs);
                        utilityviewmodel.PaymentPlans = utilityView.View.FindViewById<Spinner>(Resource.Id.PaymentPlans);

                        // Resources
                        utilityviewmodel.RBElectricity = utilityView.View.FindViewById<RadioButton>(Resource.Id.RBElectricity);
                        utilityviewmodel.RBGas = utilityView.View.FindViewById<RadioButton>(Resource.Id.RBGas);
                        utilityviewmodel.RBDualFuel = utilityView.View.FindViewById<RadioButton>(Resource.Id.RBDualFuel);

                        utilityviewmodel.LedSwitched = utilityView.View.FindViewById<TextView>(Resource.Id.LedSwitched);
                        utilityviewmodel.AccountNo = utilityView.View.FindViewById<TextView>(Resource.Id.AccountNoDynamic);
                        utilityviewmodel.AreaId = utilityView.View.FindViewById<TextView>(Resource.Id.AreaIdDynamic);
                        //utilityviewmodel.Economy7State = utilityView.View.FindViewById<TextView>(Resource.Id.Economy7);
                        utilityviewmodel.CheckBox60 = utilityView.View.FindViewById<CheckBox>(Resource.Id.checkBox60);

                        utilityviewmodel.Addresses = utilityView.View.FindViewById<Spinner>(Resource.Id.Addresses);

                        // Rates
                        utilityviewmodel.DayRate1 = utilityView.View.FindViewById<TextView>(Resource.Id.DayRate1);
                        utilityviewmodel.NightRate1 = utilityView.View.FindViewById<TextView>(Resource.Id.NightRate1);
                        utilityviewmodel.StandingCharge1 = utilityView.View.FindViewById<TextView>(Resource.Id.StandingCharge1);
                        utilityviewmodel.DayRate2 = utilityView.View.FindViewById<TextView>(Resource.Id.DayRate2);
                        utilityviewmodel.NightRate2 = utilityView.View.FindViewById<TextView>(Resource.Id.NightRate2);
                        utilityviewmodel.StandingCharge2 = utilityView.View.FindViewById<TextView>(Resource.Id.StandingCharge2);

                        // Dates
                        // Do *not* set the StartDate DateTime here!
                        utilityviewmodel.StartDate = utilityView.View.FindViewById<DatePicker>(Resource.Id.StartDate);
                        utilityviewmodel.ResetDates = utilityView.View.FindViewById<Button>(Resource.Id.ResetDates);
                        // Do *not* set the EndDate DateTime here!
                        utilityviewmodel.EndDate = utilityView.View.FindViewById<DatePicker>(Resource.Id.EndDate);
                        // Totals
                        utilityviewmodel.TotalCost = utilityView.View.FindViewById<TextView>(Resource.Id.TotalCost);
                        utilityviewmodel.TotalReadings = utilityView.View.FindViewById<TextView>(Resource.Id.TotalReadings);

                        // Last bit now Ray ..
                        utilityviewmodel.PictureBoxDNO = utilityView.View.FindViewById<ImageView>(Resource.Id.pictureBoxDNO);
                        // Postcode
                        utilityviewmodel.PostCode = utilityView.View.FindViewById<TextView>(Resource.Id.Postcode);
                        // Cultures
                        // This DOES deserve a fucking Scotch!  Worked first time!!!!
                        // Unfuckingbelieveable!!!!
                        // No it didn't,  but I fixed it in the end
                        utilityviewmodel.UtilityCultures = utilityView.View.FindViewById<Spinner>(Resource.Id.utilityCultures);
                        // Rate Conversions
                        utilityviewmodel.UtilityRBNON = utilityView.View.FindViewById<RadioButton>(Resource.Id.UtilityRBNON);
                        utilityviewmodel.UtilityRBGBP = utilityView.View.FindViewById<RadioButton>(Resource.Id.UtilityRBGBP);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        utilityviewmodel.GBPRate = utilityView.View.FindViewById<TextView>(Resource.Id.GBPRate);
                        // 'Binding' is done in 'ourviewmodel' of all places!!                
                        utilityviewmodel.UtilityRBEUR = utilityView.View.FindViewById<RadioButton>(Resource.Id.UtilityRBEUR);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        utilityviewmodel.EURRate = utilityView.View.FindViewById<TextView>(Resource.Id.EURRate);
                        // 'Binding' is done in 'ourviewmodel' of all places!!                
                        utilityviewmodel.UtilityRBUSD = utilityView.View.FindViewById<RadioButton>(Resource.Id.UtilityRBUSD);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        utilityviewmodel.USDRate = utilityView.View.FindViewById<TextView>(Resource.Id.USDRate);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        utilityviewmodel.UtilityRBJPY = utilityView.View.FindViewById<RadioButton>(Resource.Id.UtilityRBJPY);
                        // 'Binding' is done in 'ourviewmodel' of all places!!
                        utilityviewmodel.JPYRate = utilityView.View.FindViewById<TextView>(Resource.Id.JPYRate);

                    });
                    // Configure the date events
                    utilityviewmodel.StartDate.DateChanged += new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => UtilityView.StartDateChanged(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                    utilityviewmodel.ResetDates.Click += new EventHandler((s, e) => UtilityView.Utility_ResetDates(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                    utilityviewmodel.EndDate.DateChanged += new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => UtilityView.EndDateChanged(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                    
                    // Cultures
                    // This DOES deserve a fucking Scotch!  Worked first time!!!!
                    // Unfuckingbelieveable!!!!
                    // No it didn't,  but I fixed it in the end
                    utilityviewmodel.UtilityCultures.OnItemSelectedListener = new UtilityView.CultureItemSelectedListener(meterActivity, ourviewmodel, utilityviewmodel);

                    // 'Binding' is done in 'ourviewmodel' of all places!!
                    if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    {
                        utilityviewmodel.UtilityRBNON.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        utilityviewmodel.UtilityRBGBP.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        utilityviewmodel.UtilityRBEUR.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        utilityviewmodel.UtilityRBUSD.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                        utilityviewmodel.UtilityRBJPY.TooltipText = SignIn.BesetByChimps(signinviewmodel, "ConversionsTooltip");
                    }
                    utilityviewmodel.GBPRate.Text = ourviewmodel.GBPRate;
                    utilityviewmodel.EURRate.Text = ourviewmodel.EURRate;
                    utilityviewmodel.USDRate.Text = ourviewmodel.USDRate;
                    utilityviewmodel.JPYRate.Text = ourviewmodel.JPYRate;

#pragma warning restore CA1416
#endif
                    if (!await SmartUtilityV2022.Start_Utility_Display(
#if WINFORMS
                                                        components,
#endif
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel,
                                                            next_connection,
                                                            cubeface_code,
                                                            face_last_display))
                    {
                        // SOMETHING has gone amiss ...
                        //if (!(ourviewmodel.quitCts.IsCancellationRequested ||
                        //        utilityviewmodel.utilityToken.IsCancellationRequested))
                        if (!ourviewmodel.quitCts.IsCancellationRequested) // <= like Smart_Finance_Disp
                        {
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            ourviewmodel.errorMessage = "Problem 55: " + "Start Utility Display failed";
                            if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                        }
                        return false;
                    }
                    //if (face_last_display != "")
                    //{
                    //    ourviewmodel.screenCode = utilityviewmodel.ScreenCode;
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        internal static async Task<bool> UnLoadUtilityModule(
#if WINFORMS
                                                SmartDashboard.MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                List<object> vmList,
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char cubeface_code)
        {
            ourviewmodel.SMARTUTILITY = false;
            // Configure the date events
#if WPF
           
#endif
#if ANDROIDX
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                utilityviewmodel.StartDate.DateChanged -= new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => UtilityView.StartDateChanged(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.ResetDates.Click -= new EventHandler((s, e) => UtilityView.Utility_ResetDates(s, e, meterActivity, ourviewmodel, utilityviewmodel));
                utilityviewmodel.EndDate.DateChanged -= new EventHandler<DatePicker.DateChangedEventArgs>((s, e) => UtilityView.EndDateChanged(s, e, meterActivity, ourviewmodel, utilityviewmodel));
            }
#endif
            if (!SmartPhyllV2020.RemoveSmartUtility(ourviewmodel,
                                                        utilityviewmodel,
                                                        ourviewmodel.sqlitetablesList,
                                                        ourviewmodel.screenCode,
                                                        SmartParametersV2016.SmartUtilitySchema,
                                                        ourviewmodel.UserName))// For consistency
            {
                // Never happens in SmartPhyll as there is no reference to SmartBob
                // Set the fourth Led to Red ... and duck out
                FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 56: loading PLO " + utilityviewmodel.errorMessage))
                {
                    return false;
                }
                return false;
            }

            UtilityView utilityView = ourviewmodel.viewCollection
                .Select(x => x.View)
                .OfType<UtilityView>()
                .FirstOrDefault();
            utilityView.UtilityCleanup();

            // Remove the viewmodel from vmList
            vmList.RemoveAll(x => x is UtilityViewModel);
            // Remove the view itself from Consumers
            List<SmartUsers.ConsumerViews> viewtoremove = ourviewmodel.viewCollection
                                .Where(x => x.View is UtilityView)
                                .ToList();
            foreach (SmartUsers.ConsumerViews item in viewtoremove)
            {
                ourviewmodel.viewCollection.Remove(item);
            }

            // See the notes for Finance
            await TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " removed");
            return true;
        }

        internal static bool CubefaceActive(MainViewModel ourviewmodel,
                                            char cubeface_code)
        {
            foreach (SmartProfile.Cubefaces aargh in ourviewmodel.SmartProfile.profilecubefacesList) // Checked
            {
                if (aargh.CUBEFACE_CODE == cubeface_code &&
                    aargh.FACE_ACTIVE)
                {
                    return true;
                }
            }
            ;
            return false;
        }

        internal static bool CubefaceViewExists(MainViewModel ourviewmodel,
                                            char cubeface_code)
        {
            foreach (SmartUsers.ConsumerViews aargh in ourviewmodel.viewCollection) // Checked
            {
                if (aargh.CUBEFACE_CODE == cubeface_code)
                {
                    return true;
                }
            }
            ;
            return false;
        }

#if WINFORMS || WPF  || WINUI
        internal static UserControl CubefaceView(MainViewModel ourviewmodel,
                                            char cubeface_code)
        {
            foreach (SmartUsers.ConsumerViews aargh in ourviewmodel.viewCollection)
            {
                if (aargh.CUBEFACE_CODE == cubeface_code)
                {
                    return aargh.View;
                }
            };
            return new UserControl();
        }
#endif

#if SMARTMAUI
        internal static ContentView CubefaceView(MainViewModel ourviewmodel,
                                            char cubeface_code)
        {
            foreach (SmartUsers.ConsumerViews aargh in ourviewmodel.viewCollection)
            {
                if (aargh.CUBEFACE_CODE == cubeface_code)
                {
                    return aargh.View as ContentView;
                }
            }
            ;
            return new ContentView();
        }
#endif
#if ANDROIDX
        internal static View CubefaceView(MainViewModel ourviewmodel,
                                            char cubeface_code)
        {
            foreach (SmartUsers.ConsumerViews aargh in ourviewmodel.viewCollection) // Checked
            {
                if (aargh.CUBEFACE_CODE == cubeface_code)
                {
                    return aargh.View;
                }
            }
            // Should - in theory - never get here EVER!
            return ourviewmodel.viewCollection.First().View;
        }

        //// Handler for the item click event:
        //internal static void OnItemClick(object sender, int position)
        //{
        //    if (sender != null)
        //    {
        //        // Display a toast that briefly shows the enumeration of the selected photo:
        //        int itemNum = position + 1;
        //        Toast.MakeText(ourviewmodel.activity, "This is item number " + itemNum, ToastLength.Short).Show();
        //    }
        //}
#endif
        //internal static SmartProfile.CubefacesView CreateCubefacesView(SmartProfile.Cubefaces cubeface_row,
        //                                                            string description)
        //{
        //    SmartProfile.CubefacesView aargh = new SmartProfile.CubefacesView()
        //    {
        //        CUBEFACE_CODE = cubeface_row.CUBEFACE_CODE,
        //        DESCRIPTION = description,
        //        ACTIVEFLAG = cubeface_row.FACE_ACTIVE
        //    };
        //    return aargh;
        //}


        internal static void DisableCubefaces(MainViewModel ourviewmodel,
                                                SmartProfile.GroupsSQLite mm_row)
        {
            if (mm_row.SENDF == 0)
            {
                foreach (SmartProfile.Cubefaces cube_row in ourviewmodel.SmartProfile.profilecubefacesList) // Checked
                {
                    if (cube_row.USERNAME == mm_row.GROUPNAME &&
                        cube_row.FACE_ACTIVE &&
                        cube_row.CUBEFACE_CODE == SmartParametersV2016.Finance)
                    {
                        cube_row.FACE_ACTIVE = false;
                    }
                }
            }

            if (mm_row.SENDU == 0)
            {
                foreach (SmartProfile.Cubefaces cube_row in ourviewmodel.SmartProfile.profilecubefacesList) // Checked
                {
                    if (cube_row.USERNAME == mm_row.GROUPNAME &&
                        cube_row.FACE_ACTIVE &&
                        cube_row.CUBEFACE_CODE == SmartParametersV2016.Utility)
                    {
                        cube_row.FACE_ACTIVE = false;
                    }
                }
            }
            return;
        }

        internal static string SchemasFromCubeface(SignInViewModel signinviewmodel,
                                                            MainViewModel ourviewmodel)
        {
            string schemasList = "";

            List<SmartData.Cubefaces> cubefaces_found = new List<SmartData.Cubefaces>
                (from MainFace in signinviewmodel.Fatah.cubefacesList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { MainFace.CUBEFACE_CODE }
                 equals new { Cubeface.CUBEFACE_CODE }
                 select MainFace);
            foreach (SmartData.Cubefaces cube in cubefaces_found) // Checked
            {
                if (schemasList != "")
                {
                    schemasList += SmartParametersV2016.unitSeparator;
                }
                schemasList += SmartParametersV2016.keystone.Replace("%", "") + cube.DESCRIPTION;
            }
            // This is a list of schemas for which we have cubes
            // within which we can display data from ourselves
            // and others
            return schemasList;
        }

        internal static string MMFromSchemas(string mmschemas, SmartProfile.Groups mm_row)
        {
            if (mmschemas != "")
            {
                mmschemas += SmartParametersV2016.unitSeparator;
            }
            // For RAY this gets all Profiles, for ANNA and SFB this just gets AddressesView
            mmschemas += SmartParametersV2016.SmartProfileSchema;
            // Do we need the Finance schema?
            if (mm_row.SENDF)
            {
                if (mmschemas != "")
                {
                    mmschemas += SmartParametersV2016.unitSeparator;
                }
                mmschemas += SmartParametersV2016.SmartFinanceSchema;
            }
            // Do we need the Utility schema?
            if (mm_row.SENDU)
            {
                if (mmschemas != "")
                {
                    mmschemas += SmartParametersV2016.unitSeparator;
                }
                mmschemas += SmartParametersV2016.SmartUtilitySchema;
            }

            // This is a list of schemas for which we require data
            return mmschemas;
        }

        internal static string FixDEKPassword(string PassWordHash, int substring)
        {
            // Return last ClearPassword no of characters from the end of PaswordHash
            // So we don't blow the space for the KEY_DETAILS and DETAILS fields
            if (PassWordHash.Length > substring)
            {
                return PassWordHash.Substring(PassWordHash.Length - substring);
            }
            return PassWordHash;
        }
        internal static string CreateDEK(string PassWord, string Id, string schema)
        {
            // Now each Cube has a different DEK based on the users
            // Password, the Cube/Schema they are using and their Id
            string schemaName = schema.Replace(SmartParametersV2016.keystone.TrimEnd('%'), "");
            // Yes this could be more elegant, but at least here I can see
            // the schema's name ..
            return SmartEncryptionV2016.DoTheBiz(schemaName + PassWord,
                                                Id,
                                                "",
                                                true);
        }

        internal static string GetGroupName(MainViewModel ourviewmodel, string username)
        {
            // We  can only have entries in Rays.profilegroupsList
            // IF we have MultiMeter granted
            if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
            {
                // This look-up includes RAY as well as ANNA
                foreach (SmartProfile.Groups user in ourviewmodel.SmartProfile.profilegroupsList) // Checked
                {
                    if (user.USERNAME == username)
                    {
                        if (user.GROUPNAME != "")
                        {
                            return user.GROUPNAME;
                        }
                        else
                        {
                            return user.USERNAME;
                        }
                    }
                }
            }
            //else
            //{
            //    if (ourviewmodel.ProfileDisplayName != "")
            //    {
            //        return ourviewmodel.ProfileDisplayName;
            //    }
            //}
            return "";
        }

        internal static string GetDisplayName(MainViewModel ourviewmodel, string username)
        {
            foreach (SmartUsers.Consumers cons in ourviewmodel.Hamas.consumersList) // Checked
            {
                if (cons.USERNAME == username &&
                    cons.Include)
                {
                    // We  can only have entries in Rays.profilesList
                    if (ourviewmodel.SmartProfile.profilesList.Count > 0)
                    {
                        // This look-up includes RAY as well as ANNA
                        foreach (SmartProfile.Profiles user in ourviewmodel.SmartProfile.profilesList) // Checked
                        {
                            if (user.USERNAME == username &&
                                user.USERNAME == ourviewmodel.UserName)
                            {
                                if (user.DISPLAYNAME != "")
                                {
                                    return user.DISPLAYNAME;
                                }
                                else
                                {
                                    return user.USERNAME;
                                }
                            }
                            else
                            {
                                foreach (SmartProfile.Groups group_view in ourviewmodel.SmartProfile.profilegroupsList) // Checked
                                {
                                    if (username == group_view.GROUPNAME)
                                    {
                                        return group_view.DISPLAYNAME;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }
        internal static async Task<SmartProfile.AddressesView> UsersUDPRNLookup(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                                        string basic)
        {
            CancellationToken cancel_token = new CancellationToken();

            SmartProfile.AddressesView addressview = new SmartProfile.AddressesView();

            List<SmartUsers.ExternalResources> externalresource_found =
                SmartSpikeV2017.Main_Lookup_External(ourviewmodel.Blanche.externalResourcesList, 1);
            if (externalresource_found.Count > 0)
            {
                //string wot = @"https://api.ideal-postcodes.co.uk/v1/addresses?api_key=ak_l0uytcdoys1UbYOMsFvighNOVqJ0S&query=3%20Wythens%20Road&postcode=SK8%203JH";

                string[] param = new string[2] { "", "" };
                param[0] = "api_key" + "|" + externalresource_found.First().API_KEY; // "ak_l1osty86uMKmvlm7wtnsFgCvDEFke";
                param[1] = "query" + "|" + basic;

                int field_count = 0;
                string result = "";
                foreach (string vals in param) // Checked
                {
                    string[] fields = vals.Split('|');
                    if (fields.Length == 2)
                    {
                        if (result.Length > 0)
                        {
                            result += "&";
                        }
                        string fields1 = "";
                        if (field_count == 0)
                        {
                            fields1 = fields[1];
                        }
                        else
                        {
                            fields1 = Uri.EscapeDataString(fields[1]);
                        }
                        if (fields1.Contains('('))
                        {
                            fields1 = fields1.Replace("(", "%28");
                        }
                        if (fields1.Contains(')'))
                        {
                            fields1 = fields1.Replace(")", "%29");
                        }
                        result = result + fields[0] + "=" + fields1;
                        field_count++;
                    }
                }

                //string url_base = externalresource_found.First().EXTERNALUrl;//    @"https://api.ideal-postcodes.co.uk/v1/addresses";

                Uri token_uri = new Uri(externalresource_found.First().EXTERNAL_URL + "?" + result);

                string responseData = await SmartBobV2017.HTTP_UDPRN_GET(ourviewmodel,
                                          token_uri,
                                          SmartParametersV2016.timespanTimeout,
                                          cancel_token);
                if (responseData == "")
                {
                    await TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, "Address no JSON response: " + basic);
                    return addressview;
                }

                addressview = DecodeAddress(ourviewmodel, responseData, basic);
                if (ourviewmodel.errorMessage != "" ||
                    addressview.UDPRN == "")
                {
                    await TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, "Address UDPRN not found: " + basic);
                    return addressview;
                }
                // Address *should* be filled in by here ...
            }
            // Address may well be empty here ...
            return addressview;
        }

        
        internal static SmartProfile.AddressesView DecodeAddress(
                    MainViewModel ourviewmodel,
                    string json,
                    string basic)
        {
            SmartProfile.AddressesView addressview = new SmartProfile.AddressesView()
            {
                BASIC = basic
            };

            try
            {
                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("result", out JsonElement result))
                {
                    if (result.TryGetProperty("hits", out JsonElement hits))
                    {
                        foreach (JsonElement level2 in hits.EnumerateArray())
                        {
                            foreach (JsonProperty level3 in level2.EnumerateObject())
                            {
                                string name = level3.Name;
                                string value = level3.Value.ToString();

                                switch (name)
                                {
                                    // Ignore fields you don't care about
                                    case "administrative_county":
                                    case "country":
                                    case "country_iso":
                                    case "dataset":
                                    case "delivery_point_suffix":
                                    case "department_name":
                                    case "district":
                                    case "eastings":
                                    case "id":
                                    case "latitude":
                                    case "line_1":
                                    case "line_2":
                                    case "line_3":
                                    case "longitude":
                                    case "northings":
                                    case "postcode_type":
                                    case "premise":
                                    case "su_organisation_indicator":
                                    case "postal_county":
                                    case "postcode_inward":
                                    case "traditional_county":
                                    case "umprn":
                                    case "uprn":
                                    case "ward":
                                        break;

                                    // Fields you DO want
                                    case "building_name":
                                        addressview.BUILDING_NAME = value;
                                        break;
                                    case "building_number":
                                        addressview.BUILDING_NUMBER = value;
                                        break;
                                    case "county":
                                        addressview.COUNTY = value;
                                        break;
                                    case "dependant_locality":
                                        addressview.DEPENDANT_LOCALITY = value;
                                        break;
                                    case "dependant_thoroughfare":
                                        addressview.DEPENDANT_THOROUGHFARE = value;
                                        break;
                                    case "double_dependant_locality":
                                        addressview.DOUBLE_DEPENDANT_LOCALITY = value;
                                        break;
                                    case "organisation_name":
                                        addressview.ORGANIZATION = value;
                                        break;
                                    case "po_box":
                                        addressview.POBOX = value;
                                        break;
                                    case "post_town":
                                        addressview.TOWN = value;
                                        break;
                                    case "postcode":
                                        addressview.POSTCODE = value;
                                        break;
                                    case "postcode_outward":
                                        addressview.POSTCODE_OUTWARD = value;
                                        break;
                                    case "sub_building_name":
                                        addressview.SUB_BUILDING_NAME = value;
                                        break;
                                    case "thoroughfare":
                                        addressview.THOROUGHFARE = value;
                                        break;
                                    case "udprn":
                                        addressview.UDPRN = value;
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ourviewmodel.errorMessage = ex.Message;
            }

            return addressview;
        }

#if WINUI
        internal static async void OnSwipedLeftActual(object sender, ManipulationDeltaRoutedEventArgs e,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            if (sender != null &&
                (e != null))
            {
                await ButtonRightClickedActual(signinviewmodel, ourviewmodel);
            }
            return;
        }
#endif

#if ANDROIDX
    internal static async void OnSwipedLeftActual(object sender, EventArgs e,
                                                        AppCompatActivity meterActivity,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            await ButtonRightClickedActual(meterActivity, signinviewmodel, ourviewmodel);
            return;
        }
#endif
#if SMARTMAUI
        internal static async void OnSwipedLeftActual(object sender, EventArgs e,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            if (sender != null &&
                (e != null))
            {
                await ButtonRightClickedActual(signinviewmodel, ourviewmodel);
            }
            return;
        }
#endif
        internal static async Task<bool> ButtonRightClickedActual(
#if ANDROIDX
                                                                    AppCompatActivity meterActivity,
#endif
                                                                    SignInViewModel signinviewmodel,
                                                                    MainViewModel ourviewmodel)
        {
            short lower = 0;
            short upper = (short)ourviewmodel.SmartProfile.profilecubefacesList.Count;
            short scode = ourviewmodel.screenCode;
            bool limit = true;
            while (true)
            {
                scode++;
                if (scode > upper)
                {
                    if (limit)
                    {
                        break;
                    }
                    scode = lower;
                }
                char oldcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(signinviewmodel,
                                                                                    ourviewmodel.screenCode);

                char newcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(signinviewmodel,
                                                                                    scode);
                if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, newcubefaceCode))
                {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    ourviewmodel.MyContent = SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode);
#endif
#if ANDROIDX
                    ourviewmodel.MyContent.RemoveView(SmartRoutinesV2018.CubefaceView(ourviewmodel, oldcubefaceCode));
                    ourviewmodel.MyContent.AddView(SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode));
#endif
                    await SmartRoutinesV2018.RecordCubeSwitch(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, newcubefaceCode);
                    ourviewmodel.screenCode = scode;
                    break;
                }
            }
            return true;
        }
#if WINUI
        internal static async void OnSwipedRightActual(object sender, ManipulationDeltaRoutedEventArgs e,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            if (sender != null &&
                e != null)
            {
                await ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
            }
            return;
        }
#endif

#if ANDROIDX
        internal static async void OnSwipedRightActual(object sender, EventArgs e,
                                                        AppCompatActivity meterActivity,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            await ButtonLeftClickedActual(meterActivity, signinviewmodel, ourviewmodel);
            return;
        }
#endif
#if SMARTMAUI
        internal static async void OnSwipedRightActual(object sender, EventArgs e,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            if (sender != null &&
                e != null)
            {
                await ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
            }
            return;
        }
#endif
        internal static async Task<bool> ButtonLeftClickedActual(
#if ANDROIDX
                                                                    AppCompatActivity meterActivity,
#endif
                                                                    SignInViewModel signinviewmodel,
                                                                    MainViewModel ourviewmodel)
        {
            short lower = 0;
            short upper = (short)ourviewmodel.SmartProfile.profilecubefacesList.Count;
            short scode = ourviewmodel.screenCode;
            bool limit = true;
            while (true)
            {
                scode--;
                if (scode < lower)
                {
                    if (limit)
                    {
                        break;
                    }
                    scode = upper;
                }
                char oldcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(signinviewmodel,
                                                                                    ourviewmodel.screenCode);


                char newcubefaceCode = SmartSpikeV2017.LookupCubefacesCubefaceCode(signinviewmodel,
                                                                                    scode);
                if (SmartRoutinesV2018.CubefaceActive(ourviewmodel, newcubefaceCode))
                {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    ourviewmodel.MyContent = SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode);
#endif
#if ANDROIDX
                    // WTF are you doing here? Why are you removing all the views
                    // and then adding everything back EXCEPT Profile???
                    ourviewmodel.MyContent.RemoveView(SmartRoutinesV2018.CubefaceView(ourviewmodel, oldcubefaceCode));
                    ourviewmodel.MyContent.AddView(SmartRoutinesV2018.CubefaceView(ourviewmodel, newcubefaceCode));
#endif
                    await SmartRoutinesV2018.RecordCubeSwitch(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, newcubefaceCode);
                    ourviewmodel.screenCode = scode;
                    break;
                }
            }
            // Cuppa tea time Raymondo!!  Think this fucking shit actually works!!!!!
            return true;
        }
        internal static bool CreateHandlers(SignInViewModel signinviewmodel,
                                             MainViewModel ourviewmodel)
        {
            bool handler_status = true;
            // We've got to be able to communicate
            foreach (SmartData.Handlers handler_row in signinviewmodel.Fatah.handlersList) // Checked
            {
                switch (handler_row.DESCRIPTION)
                {
                    case "webproxy":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.webproxyUrl = ourviewmodel.TargetUrl;
                        break;
                    case "listener":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.listenerUrl = ourviewmodel.TargetUrl;
                        break;
                    case "dbserver":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.dbserverUrl = ourviewmodel.TargetUrl;
                        break;
                    case "connect":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.connectUrl = ourviewmodel.TargetUrl;
                        break;
                    case "insertcommon":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.insertcommonUrl = ourviewmodel.TargetUrl;
                        break;
                    case "storedprocedures":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.storedproceduresUrl = ourviewmodel.TargetUrl;
                        break;
                    case "lookuplogo":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.lookuplogoUrl = ourviewmodel.TargetUrl;
                        break;
                    //case "lookupdno":
                    //    handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                    //    ourviewmodel.lookupdnoUrl = ourviewmodel.TargetUrl;
                    //    break;
                    case "loadtable":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.loadtableUrl = ourviewmodel.TargetUrl;
                        break;
                    case "downloadpdf":
                        handler_status = CreateUri(ourviewmodel, ourviewmodel.website, @handler_row.PATH);
                        ourviewmodel.downloadpdfUrl = ourviewmodel.TargetUrl;
                        break;
                    default:
                        break;
                }
                if (!handler_status)
                {
                    signinviewmodel.errorMessage = "CreateHandlerFailure";
                    return handler_status;
                }
            }
            return handler_status;
        }

        internal static bool CreateUri(MainViewModel ourviewmodel, string base_string, string relative_string)//, rf Uri base_uri)
        {
            // DON'T FUCK WITH THIS ROUTINE <=== THIS MEANS ***YOU***
            bool status = true;
            ourviewmodel.errorMessage = "";
            try
            {
                if (!string.IsNullOrEmpty(base_string))
                {
                    //Uri 
                    ourviewmodel.TargetUrl = new Uri(base_string);

                    UriBuilder uri_builder = new UriBuilder()
                    {
                        Scheme = ourviewmodel.TargetUrl.Scheme,
                        Host = ourviewmodel.TargetUrl.Host,
                        Port = ourviewmodel.TargetUrl.Port,   // 80 gets removed, 81 or 12345 doesn't! (its not default)
                        Path = relative_string
                    };
                    ourviewmodel.TargetUrl = new Uri(uri_builder.Uri.OriginalString);
                    if (ourviewmodel.TargetUrl != null)
                    {
                        return true;
                    }
                }
                else
                {
                    ourviewmodel.errorMessage = "Uri base string is empty";
                    status = false;
                }
            }
            catch (ArgumentNullException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
                status = false;
            }
            catch (UriFormatException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
                status = false;
            }
            if (!status)
            {
                ourviewmodel.errorMessage = "Cannot create Uri from: " + base_string + relative_string + SmartParametersV2016.space + ourviewmodel.errorMessage;
            }
            return status;
        }

        internal static bool CreateUriLogin(SignInViewModel signinviewmodel, string base_string, string relative_string)//, rf Uri base_uri)
        {
            // DON'T FUCK WITH THIS ROUTINE <=== THIS MEANS ***YOU***
            bool status = true;
            signinviewmodel.errorMessage = "";
            try
            {
                if (!string.IsNullOrEmpty(base_string))
                {
                    //Uri 
                    signinviewmodel.TargetUrl = new Uri(base_string);

                    UriBuilder uri_builder = new UriBuilder()
                    {
                        Scheme = signinviewmodel.TargetUrl.Scheme,
                        Host = signinviewmodel.TargetUrl.Host,
                        Port = signinviewmodel.TargetUrl.Port,   // 80 gets removed, 81 or 12345 doesn't! (its not default)
                        Path = relative_string
                    };
                    signinviewmodel.TargetUrl = new Uri(uri_builder.Uri.OriginalString);
                    if (signinviewmodel.TargetUrl != null)
                    {
                        return true;
                    }
                }
                else
                {
                    signinviewmodel.errorMessage = "Uri base string is empty";
                    status = false;
                }
            }
            catch (ArgumentNullException exception)
            {
                signinviewmodel.errorMessage = exception.Message;
                status = false;
            }
            catch (UriFormatException exception)
            {
                signinviewmodel.errorMessage = exception.Message;
                status = false;
            }
            if (!status)
            {
                signinviewmodel.errorMessage = "Cannot create Uri from: " + base_string + relative_string + SmartParametersV2016.space + signinviewmodel.errorMessage;
            }
            return status;
        }
        internal static DateTime DateTimeParse(string parse)
        {
            return DateTime.Parse(parse);
        }

        internal static DateTime DateTimeParseCulture(string parse, CultureInfo culture)
        {
            return DateTime.Parse(parse, culture);
        }

        internal static DateTime DateTimeParseExact(string parse, string format, CultureInfo culture)
        {
            return DateTime.ParseExact(parse, format, culture);
        }

        internal static int DateTimeCompare(DateTime time1, DateTime time2)
        {
            return DateTime.Compare(time1, time2);
        }

        internal static bool DecodeCookies(string key,
                                        string cookie_name,
                                        CookieContainer cookieJar,
                                        AntiXsrfToken anti_token)
        {
            try
            {
                //Hashtable table = (Hashtable)cookieJar
                //    .GetType().InvokeMember("m_domainTable",
                //SmartParametersV2016.bindingFlags,
                //    null,
                //    cookieJar,
                //    new object[] { });


                //foreach (var key in table.Keys)
                //{
                //string key = SignIn.signinviewmodel.website;//  "m_domainTable";   // Worth a try ...?
                // Look for https cookies
                Uri new_uri = new Uri(key);

                if (cookieJar.GetCookies(new_uri).Count > 0)
                {
#if WINFORMS
                    //await TextBlockUpdate(ourviewmodel, cookieJar.Count + " HTTPS COOKIES FOUND:" + Environment.NewLine.ToString());
                    //await TextBlockUpdate(ourviewmodel, "----------------------------------" + Environment.NewLine.ToString());
#endif
                    System.Net.CookieCollection bollocks = cookieJar.GetCookies(new_uri);
                    foreach (Cookie cookie in bollocks) // Checked
                    {
                        if (cookie.Name == cookie_name)
                        {
                            anti_token.cookie_value = cookie.Value;
                            anti_token.cookie_path = cookie.Path;
                            anti_token.cookie_domain = cookie.Domain;
                            anti_token.cookie_secure = cookie.Secure;
                            anti_token.cookie_timestamp = cookie.TimeStamp;
#if WINFORMS
                            //await TextBlockUpdate(ourviewmodel, 
                            //"Name = " + cookie.Name + SmartParametersV2016.space +
                            //"Value = " + cookie.Value + SmartParametersV2016.space +
                            //                        "Domain = " + cookie.Domain +
                            //                        "Path = " + cookie.Path + SmartParametersV2016.space +
                            //                        "Secure = " + cookie.Secure + SmartParametersV2016.space +
                            //                        "Timestamp = " + cookie.TimeStamp + SmartParametersV2016.space +
                            //                        Environment.NewLine.ToString());
#endif
                            return true;
                        }
                    }
                }
            }
            catch (InvalidCastException e)
            {
                System.Console.Write(e);
            }
            return false;
        }

        //internal static decimal ReturnDecimal(int amount_int,
        //                                    bool zero_as_empty)
        //{
        //    string amount_str = Math.Abs(amount_int).ToString();
        //    int len = amount_str.Length;

        //    switch (len)
        //    {
        //        case 0:
        //            if (zero_as_empty)
        //            {
        //                amount_str = "";
        //            }
        //            else
        //            {
        //                amount_str = "0.00";
        //            }
        //            break;
        //        case 1:
        //            amount_str = "0.0" + amount_str;
        //            break;
        //        case 2:
        //            amount_str = "0." + amount_str;
        //            break;
        //        default:
        //            len = len - 2;
        //            amount_str = amount_str.Substring(0, len) + "." + amount_str.Substring(len);
        //            break;
        //    }
        //    if (!string.IsNullOrEmpty(amount_str))
        //    {
        //        //amount_str = GetConvertToSymbol(ourviewmodel,
        //        //                                currency) + amount_str;//   "£" + amount_str;
        //        if (amount_int < 0)
        //        {
        //            amount_str = "-" + amount_str;
        //        }
        //    }
        //    return ConvertDecimal(amount_str);
        //}


#if WINFORMS
        internal static System.Drawing.Color WhiteBackground(System.Drawing.Color brush, MainViewModel ourviewmodel)
        {
            // MAKE SURE EVERY LABEL CALLED BY THIS ROUTINE HAS AN INITAL BACKGROUND E.G. "TRANSPARENT"
            // OTHERWISE THIS WILL FALL OVER WITH AN UNHANDLED NULL EXCEPTION
            if (brush == ourviewmodel.transparentColour)
            {
                brush = ourviewmodel.whiteColour;
            }
            return brush;
        }
#endif

        //#if WPF  || WINUI || MAUI
        //        internal static Brush WhiteBackground(Brush brush, MainViewModel ourviewmodel)

        //        {
        //            // MAKE SURE EVERY LABEL CALLED BY THIS ROUTINE HAS AN INITAL BACKGROUND E.G. "TRANSPARENT"
        //            // OTHERWISE THIS WILL FALL OVER WITH AN UNHANDLED NULL EXCEPTION
        //#if WPF
        //            if (brush.ToString(SmartParametersV2016.defaultCulture) == ourviewmodel.transparentColour.ToString(SmartParametersV2016.defaultCulture))
        //#endif
        //#if WINUI || MAUI || SMARTMAUI
        //            if (brush.ToString() == ourviewmodel.transparentColour.ToString())

        //#endif
        //            {
        //                brush = ourviewmodel.whiteColour;
        //            }
        //            return brush;
        //        }
        //#endif

#if ANDROIDX
//        internal static Android.Graphics.Color WhiteBackground(Android.Graphics.Color brush, MainViewModel ourviewmodel)
//        {
//            // MAKE SURE EVERY LABEL CALLED BY THIS ROUTINE HAS AN INITAL BACKGROUND E.G. "TRANSPARENT"
//            // OTHERWISE THIS WILL FALL OVER WITH AN UNHANDLED NULL EXCEPTION
//            if (brush == ourviewmodel.transparentColour)
//            {
//                brush = ourviewmodel.whiteColour;
//            }
//            return brush;
//        }
#endif

#if WINFORMS
        internal static System.Drawing.Color WhiteBackgroundDate(System.Drawing.Color foreground, System.Drawing.Color background, MainViewModel ourviewmodel)
        {
            // MAKE SURE EVERY LABEL CALLED BY THIS ROUTINE HAS AN INITAL BACKGROUND E.G. "TRANSPARENT"
            // OTHERWISE THIS WILL FALL OVER WITH AN UNHANDLED NULL EXCEPTION
            if (GetDatePickerColor_New(foreground, background, false) == ourviewmodel.transparentColour)
            {
                return ourviewmodel.whiteColour;
            }
            return ourviewmodel.transparentColour;
        }
#endif

#if WPF  || WINUI || SMARTMAUI
#if WPF  || WINUI
        internal static Brush WhiteBackgroundDate(Brush foreground, Brush background, MainViewModel ourviewmodel)
#endif
#if SMARTMAUI
        internal static Microsoft.Maui.Graphics.Color WhiteBackgroundDate(Microsoft.Maui.Graphics.Color foreground, Microsoft.Maui.Graphics.Color background, MainViewModel ourviewmodel)
#endif
        {
            // MAKE SURE EVERY LABEL CALLED BY THIS ROUTINE HAS AN INITAL BACKGROUND E.G. "TRANSPARENT"
            // OTHERWISE THIS WILL FALL OVER WITH AN UNHANDLED NULL EXCEPTION
            SolidColorBrush newBrush = (SolidColorBrush)ourviewmodel.transparentColour;
#if WPF
            if (GetDatePickerColor_New(foreground, background, false).ToString(SmartParametersV2016.defaultCulture) == ourviewmodel.transparentColour.ToString(SmartParametersV2016.defaultCulture))
#endif
#if WINUI
            if (GetDatePickerColor_New(foreground, background, false).ToString() == ourviewmodel.transparentColour.ToString())
#endif
#if SMARTMAUI
            if (GetDatePickerColor_SmartMaui(foreground, background, false).ToString() == ourviewmodel.transparentColour.ToString())
#endif
            {
                    return ourviewmodel.whiteColour;
            }
            return ourviewmodel.transparentColour;
        }
#endif

#if ANDROIDX
        internal static Android.Graphics.Color WhiteBackgroundDate(Android.Graphics.Color foreground, Android.Graphics.Color background, MainViewModel ourviewmodel)
        {
            // MAKE SURE EVERY LABEL CALLED BY THIS ROUTINE HAS AN INITAL BACKGROUND E.G. "TRANSPARENT"
            // OTHERWISE THIS WILL FALL OVER WITH AN UNHANDLED NULL EXCEPTION
            if (GetDatePickerColor_New(foreground, background, false) == ourviewmodel.transparentColour)
            {
                return ourviewmodel.whiteColour;
            }
            return ourviewmodel.transparentColour;
        }
#endif

#if WINFORMS
        internal static string CheckBox(ComboBox dropDown)
#endif
#if WPF  || WINUI
        internal static string CheckBox(ComboBox dropDown)
#endif
#if ANDROIDX
        internal static string CheckBoxX(Spinner dropDown)
#endif
#if SMARTMAUI
        internal static string CheckBox(Picker dropDown)
#endif
        {
            if (dropDown != null)
            {
                if (dropDown.SelectedItem != null)
                {
                    return dropDown.SelectedItem.ToString();
                }
            }
            return "";
        }

        internal static string FixUDPRN(string udprn)
        {
            // There's probably a quicker way to do this
            // but hey it should work (??)
            // and we don't check UDPRNs that often anyway ..
            int len = udprn.Length;
            if (len < 8)
            {
                return new string('0', 8 - len) + udprn;
            }
            else
            {
                if (len > 8)
                {
                    // Get last 8
                    return udprn.Substring(len - 8, 8);
                }
            }
            return udprn;
        }
        internal static bool Check_These_Two_Determine_UDPRN(UtilityViewModel utilityviewmodel,
                                                        short area_code,
                                                        short brand_code,
                                                        int brand_index_in)
        {
            if (area_code > 0)
            {
                utilityviewmodel.utility_brand_index = brand_index_in;
                return true;
            }
            if (brand_code >= 0)
            {
                // So area_code = 0 AND brand_code >= 0
                // There is only one occasion when this happens,
                // and that is right at the start when we need to build the
                // complete Supplier list
                utilityviewmodel.utility_brand_index = brand_index_in;
                return true;
            }
            return false;
        }

        internal static void FocusOpenClose(MainViewModel ourviewmodel)
        {
            if (!ourviewmodel.OpenCloseButtonFocus)
            {
                ourviewmodel.OpenCloseButtonFocus = true;
            }
            return;
        }

        internal static string Check_Next_Connection(CultureInfo culture,
                                                    string none,
                                                    DateTime next_connection,
                                                    DateTime defaultDate)
        {
            DateTime next_conn = next_connection;
            if (next_conn == defaultDate)
            {
                return none;
            }
            else
            {
                return next_conn.ToString(culture);
            }
        }

        internal static string Check_Postcodes(string postcode)
        {
            string content = postcode;

            int length_before = content.Length;
            if (length_before > 0)
            {
                // and the field is 12 chars long
                return SmartParametersV2016.space.PadLeft(12 - length_before) + content;
            }
            return content;
        }

#if WINFORMS
        internal static void SetColor(Control led, System.Drawing.Color shade)
        {
            if (led != null)
            {
                led.ForeColor = shade;
            }
            return;
        }

        internal static System.Drawing.Color GetColor(Control button, bool ground)
        {
            if (button != null)
            {
                if (ground)
                {
                    System.Drawing.Color foreground = (System.Drawing.Color)button.ForeColor;
                    return foreground;
                }
                else
                {
                    System.Drawing.Color background = (System.Drawing.Color)button.BackColor;
                    return background;
                }
            }
            return System.Drawing.Color.Black;
        }

        internal static void SetBackgroundColor(System.Windows.Forms.Control button, System.Drawing.Color shade)
        {
            if (button != null)
            {
                button.BackColor = shade;
            }
            return;
        }

        internal static bool CheckColor(System.Windows.Forms.Control led, System.Drawing.Color shade)
        {
            if (led != null)
            {
                if (led.ForeColor == shade)
                {
                    return true;
                }
            }
            return false;
        }
#endif

#if WINFORMS
        internal static string CheckButtonContents(string button_contents)
        {
            if (!string.IsNullOrEmpty(button_contents))
            {
                return button_contents;
            }
            return "";
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        internal static string CheckButtonContents(string button_contents)
        {
            if (!string.IsNullOrEmpty(button_contents))
            {
                return button_contents;
            }
            return "";
        }
#endif

#if ANDROIDX
        internal static string CheckButtonContents(string button_contents)
        {
            if (!string.IsNullOrEmpty(button_contents))
            {
                return button_contents;
            }
            return "";
        }
#endif

#if WINFORMS
        internal static async Task<bool> TextBlockUpdate(
#endif
#if WPF  || WINUI || SMARTMAUI
        internal static async Task<bool> TextBlockUpdate(
#endif
#if ANDROIDX
        public static async Task<bool> TextBlockUpdate(
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    string message,
                                                    bool thread = false)
        {
            // This should fire the Update_Layout event in MainMeter Ha Ha Ha 
#if WINFORMS || WPF  || WINUI || SMARTMAUI

            if (FrontEndGUI.GetScrollViewerVisible(ourviewmodel))
            {
#if WINFORMS
                ourviewmodel.ScrollViewer.AppendText(ourviewmodel.DateTimeNowMessage +  // Local time
                                                    SmartParametersV2016.space +
                                                    message +
                                                    System.Environment.NewLine);
                // Don't use await Task.Run(() in Winforms!
                // The following code works FINE!!
                try
                {
                    ourviewmodel.ScrollViewer.ScrollToCaret();
#endif
#if WPF  || WINUI || SMARTMAUI
#if WPF || SMARTMAUI
                //ourviewmodel.TextBlockContent = ourviewmodel.TextBlockContent +
                //(DateTime.Now + ourviewmodel.utcOffset).ToString() +// signinviewmodel.signincultureinfo) +  // Local time
                //SmartParametersV2016.space +
                //message +
                //System.Environment.NewLine;

                string outputLine =
                    (DateTime.Now + ourviewmodel.utcOffset)
                    .ToString(SmartParametersV2016.defaultCulture)
                    + " "
                    + message
                    + Environment.NewLine;

#if WPF
                ourviewmodel.TextBlockContent.Text += outputLine;
#endif
#if SMARTMAUI
                ourviewmodel.TextBlockContent += outputLine;
#endif



#else

#endif
                try
                {
#if WPF
                    await ourviewmodel.ScrollViewer.Dispatcher.InvokeAsync(() =>
                    {
                        ourviewmodel.ScrollViewer.UpdateLayout();
                        ourviewmodel.ScrollViewer.ScrollToEnd();
                    });
#endif
#if WINUI
                    // Thanks chatGPT!
                    await ourviewmodel.ScrollViewer.DispatcherQueue.EnqueueAsync(() =>
                    {
                        ourviewmodel.ScrollViewer.UpdateLayout();
                        double verticalOffset = ourviewmodel.ScrollViewer.ExtentHeight - ourviewmodel.ScrollViewer.ViewportHeight;
                        ourviewmodel.ScrollViewer.ChangeView(null, verticalOffset, null);
                        
                    });
#endif
#if SMARTMAUI
                    // Thanks chatGPT!
                    await Task.Delay(50);

                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        ourviewmodel.ScrollViewer.ScrollToAsync(
                            0,
                            ourviewmodel.ScrollViewer.ContentSize.Height,//double.MaxValue,
                            false);
                    });                    
#endif

                    //await Task.Run(() => ourviewmodel.scrollViewer.UpdateLayout());  // Doesn't work

                    // I have ABSOLUTELY NO FUCKING IDEA why it suddenly goes to null ...
                    // But it will if you try and convert a ListView to a DataGrid =:-[
                    //ourviewmodel.ScrollViewer.UpdateLayout();
#endif
                }
                catch (Exception exception)
                {
                    // Console.FuckingWriteline.FuckingWhere for God's sake????
                    // You DON'T have a console you plonker
                    //"Error " + exception.Message;
                    if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 57: Cannot update Scrollviewer " + exception.Message))
                    {
                        return false;
                    }
                    return false;
                }
            }
#endif
#if ANDROIDX
            // 1st April 2024 - this HASN'T got an 'await' Does it need it??
            if (ourviewmodel.ScrollViewerVisible)
            {
                // This should fire the Update_Layout event in MainMeter
                //ourviewmodel.TextBlockContent.Text = ourviewmodel.TextBlockContent.Text +
                //        (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture) +  // Local time
                //        SmartParametersV2016.space +
                //        message + System.Environment.NewLine;
                // This next bit of Chimp-ness hangs and we won't (possibly) be able to take
                // out the Delay until VFour.4 is released.  What twaddle this bollocks is

                // Can't seem to get this to complete when 'Release' is configured
                // God .. this stuff is absolute bollocks
                //double X = 0;
                // https://forums.xamarshit.com/discussion/43797/scrollview-scrolltoasync-to-bottom-problem-on-android
                //double heightContentScroll = ourviewmodel.scrollViewer.ContentSize.Height;
                // Worth a try ???
                //await Task.Delay(10); //UI will be updated by XamaShit

                //await Task.Run(() => Device.BeginInvokeOnMainThread(async () => await ourviewmodel.scrollViewer.ScrollToAsync(X, Convert.ToDouble(ScrollToPosition.End), false))); // <= Doesn't await ScrollToAsync

                //await Task.Run(() => Application.Current.Dispatcher.BeginInvokeOnMainThread(async () => await ourviewmodel.scrollViewer.ScrollToAsync(X, Convert.ToDouble(ScrollToPosition.End), false))); // <= Doesn't await ScrollToAsync

                //#Xelse

                //await Task.Run(() => Application.Current.Dispatcher.Dispatch(async () => await ourviewmodel.scrollViewer.ScrollToAsync(height, Convert.ToDouble(ScrollToPosition.End), false))); // <= Doesn't await ScrollToAsync


                // Jesus H. Christ - what an ABSOLUTE FUCKING PAIN
                // this all was to get going ...
                string outputLine = (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.defaultCulture) +  // Local time
                                    SmartParametersV2016.space +
                                    message + System.Environment.NewLine;
                if (thread)
                {
                    // https://stackoverflow.com/questions/50973167/accessing-ui-thread-in-xamarin-android
                    // The await here allows us to overcome the 'lacks await error'
                    await Task.Run(() =>
                    {
                        // This is running on a background thread
                        // Switch to the main thread to update the UI
                        meterActivity.RunOnUiThread(() =>
                        {
                            ourviewmodel.TextBlockContent.Text += outputLine;
                            ourviewmodel.ScrollViewer.FullScroll(Android.Views.FocusSearchDirection.Down);
                        });
                    });                       
                    //ourviewmodel.activity.RunOnUiThread(() =>
                    //{
                    //    ourviewmodel.TextBlockContent.Text += outputLine;
                    //    ourviewmodel.ScrollViewer.FullScroll(Android.Views.FocusSearchDirection.Down);
                    //});
                }
                else
                {
                    ourviewmodel.TextBlockContent.Text += outputLine;
                    ourviewmodel.ScrollViewer.FullScroll(Android.Views.FocusSearchDirection.Down);
                }
            }
#endif
            return true;
        }

        private static void Finish()
        {
            throw new NotImplementedException();
        }

        // But what do we IF (for some obscure bug?) Led2 is Red?
        // We've said 'we want to go' so should the colour of Led2
        // be something that stops us?  In reality, no it shouldn't
        // be - the decision of a human should always be able to
        // override the state of a piece of computer software, no
        // matter how brilliant it is ...

        
        internal static async Task<bool> ClosingDown(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    SignInViewModel signinviewmodel, 
                                                    MainViewModel ourviewmodel)
        {
#if WPF  || WINUI
            var _window = ((App)Application.Current).MainWindow;
#endif
#if SMARTMAUI
            Page _window = Application.Current.MainPage;
#endif
            // Disconnect from SmartDBServer
            // ourviewmodel.quitCts SHOULD be set to a
            // new CancellationSource (if it isn't there's gonna be trouble)
            if (!await SmartBobV2017.ConnectDisconnectAsync(
                    ourviewmodel,
                    SmartParametersV2016.disconnectSymbol,
                    ourviewmodel.multiuser,
                    "",
                    ""))
            {
#if WPF 
                // Set LED to red if disconnect failed
                Application.Current.Dispatcher.Invoke(() =>
                {
#endif
#if WINUI
                
                _window.DispatcherQueue.TryEnqueue(() =>
                {
#endif
#if WPF  || WINUI
                    FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.redColour);

                });
#endif
#if SMARTMAUI
                ourviewmodel.Dispatcher.Dispatch(() =>
                {
                    FrontEndGUI.SetLedColour(
                        ourviewmodel,
                        2,
                        ourviewmodel.redColour);
                });
#endif
                if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 75: disconnecting"))
                {
                    return false;
                }
            }
            else
            {
                if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Disconnected"))
                {
                    return false;
                }
            }

            // Step 2: Turn off LEDs
#if WPF 
            Application.Current.Dispatcher.Invoke(() =>
            {
#endif
#if WINUI
            _window.DispatcherQueue.TryEnqueue(() =>
            {
#endif
#if WPF  || WINUI
                for (int i = 1; i <= 8; i++)
                {
                    if (!FrontEndGUI.CompareLedColour(ourviewmodel, i, ourviewmodel.whiteColour))
                        FrontEndGUI.SetLedColour(ourviewmodel, i, ourviewmodel.whiteColour);
                }
            });
#endif
#if SMARTMAUI
            ourviewmodel.Dispatcher.Dispatch(() =>
            {
                for (int i = 1; i <= 8; i++)
                {
                    if (!FrontEndGUI.CompareLedColour(ourviewmodel, i, ourviewmodel.whiteColour))
                        FrontEndGUI.SetLedColour(ourviewmodel, i, ourviewmodel.whiteColour);
                }
            });
#endif
            if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Lights out"))
            {
                return false;
            }
            // Step 3: Logout from system
            if (!await SmartLoginV2016.Logout(ourviewmodel))
            {
                if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Logout failed"))
                {
                    return false;
                }
            }
            else
            {
                if (!await CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Logged out", true))
                {
                    return false;
                }
#if WPF  || WINUI || SMARTMAUI
                ourviewmodel.LastLoginStatusMessage = "(You are now logged-out  - see you again soon...)";
#endif
            }

            // Step 4: Clear sensitive references
            signinviewmodel.UserIsLoggedIn = false;
            ourviewmodel.UserName = "";
#if WPF 
            Application.Current.Dispatcher.Invoke(() =>
            {
            // Close all current windows
                foreach (Window window in Application.Current.Windows)
                {
                    if (!(window is SignIn)) // avoid closing it if already open
                    {
                        window.Close();
                    }
                }
            });
#endif
#if WINUI
            _window.DispatcherQueue.TryEnqueue(() =>
            {
                //var window = App.MainWindow;
                
                _window.Close();
                // Close all current windows
                //foreach (Window window in Application.Current.Windows)
                //{
                //    if (!(window is SignIn)) // avoid closing it if already open
                //    {
                //        window.Close();
                //    }
                //}
            });
#endif
#if ANDROIDX
            if (meterActivity != null &&
                !meterActivity.IsFinishing)
            {
                meterActivity.RunOnUiThread(() =>
                {                    
                    // Step 6: Finish MeterActivity safely
                    meterActivity.Finish();
                });
            }            
#endif
#if SMARTMAUI
            ourviewmodel.Dispatcher.Dispatch(async () =>
            {
                // Close all current windows
                await Shell.Current.GoToAsync("//SignIn");
            });
#endif
            return true;
        }

        public static async Task<bool> CheckTrace(MainViewModel ourviewmodel, CancellationToken cancellation_token, short supplier, short brand, string message, bool mustlog = false, [System.Runtime.CompilerServices.CallerMemberName] string routine = "")
        {
            if (ourviewmodel.trace || mustlog)
            {
                if (message == "")
                {
                    message = "<Empty>";
                }
                if (!await SmartBobV2017.ListenerAsync(ourviewmodel, cancellation_token, supplier, brand, routine, message)) // Why are brand and supplier always zero??!!!?!
                {
                    return false;
                }
            }
            return true;
        }

        internal static async Task<bool> CheckTraceSignIn(SignInViewModel signinviewmodel, CancellationToken cancellation_token, string message, bool mustlog = false)
        {
            if (mustlog)
            {
                if (!await SmartBobV2017.ListenerAsyncSignIn(signinviewmodel, cancellation_token, 0, 0, message)) // Why are brand and supplier always zero??!!!?! WTF??
                {
                    return false;
                }
            }
            return true;
        }

#if WINFORMS
        internal static bool PopupNo_Actual()
#endif
#if WPF  || WINUI || SMARTMAUI
        internal static bool PopupNo_Actual()
#endif
#if ANDROIDX
        internal static bool PopupNo_Actual()
#endif
        {
#if WPF  || WINUI || SMARTMAUI
            //MessageBox.IsOpen = false;
#endif
            return true;
        }

#if WINUI
        public static Popup PopupTitle(string title)
        {
            Popup myPopup = new Popup
            {
                IsLightDismissEnabled = true,
                Child = new Border
                {
                    Padding = new Thickness(16),
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Microsoft.UI.Colors.White),
                    Child = new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto }, // title
                            new RowDefinition { Height = GridLength.Auto }  // content
                        },
                        Children =
                        {
                            new TextBlock
                            {
                                Text = title,
                                FontSize = 18,
                                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                                Margin = new Thickness(0, 0, 0, 10)
                            }
                        }
                    }
                }
            };
            return myPopup;
        }
#endif


#if WINFORMS
        internal static bool Submit_Check_Meter_Display(MainViewModel ourviewmodel, System.Windows.Forms.Label Led)
#endif
#if WPF
        internal static bool Submit_Check_Meter_Display(MainViewModel ourviewmodel, Label Led)
#endif
#if WINUI
        internal static bool Submit_Check_Meter_Display(MainViewModel ourviewmodel, TextBlock Led)
#endif
#if ANDROIDX
        internal static bool Submit_Check_Meter_Display(MainViewModel ourviewmodel, TextView Led)
#endif
#if SMARTMAUI
        internal static bool Submit_Check_Meter_Display(MainViewModel ourviewmodel, Label Led)
#endif
        {
            if (Led != null)
            {
                if (CheckColor(Led, ourviewmodel.redColour))
                {
                    // We went in on a SUBMIT and failed so its THIS ONE we have to fix up
                    // Keep the LED on Red
                    // And try and put back whatever was there (if something was there)
                    return false;
                }
                else
                {
                    // Show the SUBMITTED thing
                    return true;
                }
            }
            return false;   // Led was null
        }

#if WINFORMS
        internal static string CheckButtonContent(System.Windows.Forms.Button button)
#endif
#if WPF  || WINUI
        internal static string CheckButtonContent(ContentControl button)
#endif
#if ANDROIDX
        internal static string CheckButtonContent(Android.Widget.Button button)
#endif
#if SMARTMAUI
        internal static string CheckButtonContent(Microsoft.Maui.Controls.Button button)
#endif
        {
            if (button != null)
            {
#if WINFORMS
                return button.Text.ToString();
#endif
#if WPF  || WINUI
                return button.Content.ToString();
#endif
#if ANDROIDX
                return button.Text.ToString();
#endif
#if SMARTMAUI
                return button.Text.ToString();
#endif
            }
            return "";
        }

#if WINFORMS
        internal static void SetButton(System.Windows.Forms.Button button, bool status)
#endif
#if WPF
        internal static void SetButton(System.Windows.UIElement button, bool status)
#endif
#if WINUI
        internal static void SetButton(Button button, bool status)
#endif
#if ANDROIDX
        internal static void SetButton(Android.Widget.Button button, bool status)
#endif
#if SMARTMAUI
        internal static void SetButton(Microsoft.Maui.Controls.Button button, bool status)
#endif
        {
            if (button != null)
            {
#if WINFORMS
                button.Enabled = status;
#endif
#if WPF  || WINUI
                button.IsEnabled = status;
#endif
#if ANDROIDX
                button.Enabled = status;
#endif
#if SMARTMAUI
                button.IsEnabled = status;
#endif
            }
            return;
        }

#if WINFORMS
        internal static System.Drawing.Color GetLabelBackground(MainViewModel ourviewmodel,
                                                    Label label)
#endif
#if WPF
        internal static System.Windows.Media.Color GetLabelBackground(MainViewModel ourviewmodel,
                                                    Label label)
#endif
#if WINUI
        internal static Windows.UI.Color GetLabelBackground(MainViewModel ourviewmodel,
                                                    TextBlock label)
#endif
#if ANDROIDX
        internal static Android.Graphics.Color GetLabelBackground(MainViewModel ourviewmodel, TextView label)
#endif
#if SMARTMAUI
        internal static Microsoft.Maui.Graphics.Color GetLabelBackground(MainViewModel ourviewmodel, Label label)
#endif
        {
            
#if WINFORMS
            System.Drawing.Color thecolour = System.Drawing.Color.FromArgb(label.BackColor.A,
                                                        label.BackColor.R,
                                                        label.BackColor.G,
                                                        label.BackColor.B);
#endif
#if WPF
            SolidColorBrush newBrushx = (SolidColorBrush)ourviewmodel.blackColour;
            System.Windows.Media.Color thecolour = newBrushx.Color;  // Default
#endif
#if WINUI || MAUI
            SolidColorBrush newBrushx = (SolidColorBrush)ourviewmodel.blackColour;
            Windows.UI.Color thecolour = newBrushx.Color;  // Default
#endif
#if ANDROIDX
            Android.Graphics.Color thecolour = ourviewmodel.blackColour;   // default
#endif
#if SMARTMAUI
            Microsoft.Maui.Graphics.Color thecolour = label.TextColor;
#endif
            return thecolour;
        }

#if WINFORMS
        internal static void SetLabelBackground(System.Windows.Forms.Label label, System.Drawing.Color background)
#endif
#if WPF
        internal static void SetLabelBackground(Label label, System.Windows.Media.Color background)
#endif
#if WINUI || MAUI
        internal static void SetLabelBackground(TextBlock label, Windows.UI.Color background)
#endif
#if ANDROIDX
        internal static void SetLabelBackground(TextView label, Android.Graphics.Color background)
#endif
#if SMARTMAUI
        internal static void SetLabelBackground(Label label, Microsoft.Maui.Graphics.Color background)
#endif
        {
            if (label != null)
            {
#if WINFORMS
                label.BackColor = background;
#endif
#if WPF
                label.Background = new SolidColorBrush(background);
#endif
#if WINUI || MAUI
                label.Foreground = new SolidColorBrush(background);
#endif
#if ANDROIDX
                label.SetBackgroundColor(background);
#endif
#if SMARTMAUI
                label.BackgroundColor = background;
#endif
            }
            return;
        }

#if WINFORMS
        internal static string SetLabelContent(string label_content, string content)
#endif
#if WPF  || WINUI
        internal static string SetLabelContent(string label_content, string content)
#endif
#if ANDROIDX
        internal static string SetLabelContent(string label_content, string content)
#endif
#if SMARTMAUI
        internal static string SetLabelContent(string label_content, string content)
#endif
        {
            if (!string.IsNullOrEmpty(label_content) &&
                !string.IsNullOrEmpty(content))
            {
                label_content = content;
            }
            return label_content;
        }

#if WINFORMS
        internal static string GetTextBlockContent(TextBox textbloc)
#endif
#if WPF  || WINUI || MAUI
        internal static string GetTextBlockContent(TextBlock textbloc)
#endif
#if ANDROIDX
        internal static string GetTextBlockContent(TextView textbloc)
#endif
#if SMARTMAUI
        internal static string GetTextBlockContent(Label textbloc)
#endif
        {
            string text = ""; // Default
            if (textbloc != null)
            {
                text = textbloc.Text;
            }
            return text;
        }

#if WINFORMS
        internal static void SetTextBlockContent(TextBox textbloc, string text)
#endif
#if WPF  || WINUI
        internal static void SetTextBlockContent(TextBlock textbloc, string text)
#endif
#if ANDROIDX
        internal static void SetTextBlockContent(TextView textbloc, string text)
#endif
#if SMARTMAUI
        internal static void SetTextBlockContent(Label textbloc, string text)
#endif
        {
            if (textbloc != null &&
                text != null)
            {
                textbloc.Text = text;
            }
            return;
        }

#if WINFORMS
        internal static void SetLabelVisibility(Label label, bool new_visibility)
#endif
#if WPF
        internal static void SetLabelVisibility(Label label, System.Windows.Visibility new_visibility)
#endif
#if WINUI
        internal static void SetLabelVisibility(TextBlock label, Visibility new_visibility)
#endif
#if ANDROIDX
        internal static void SetLabelVisibility(TextView label, ViewStates new_visibility)
#endif
#if SMARTMAUI
        internal static void SetLabelVisibility(Label label, Visibility new_visibility)
#endif
        {
            if (label != null)
            {
#if WINFORMS
                if (new_visibility == true)
                {
                    label.Visible = true;
                }
                else
                {
                    label.Visible = false;
                }
#endif
#if WPF  || WINUI || SMARTMAUI
                if (new_visibility == Visibility.Visible)
                {
#if SMARTMAUI
                    label.IsVisible = true;
#else
                    label.Visibility = Visibility.Visible;
#endif
                }
                else
                {
#if WPF
                    label.Visibility = Visibility.Hidden;
#endif
#if WINUI
                    label.Visibility = Visibility.Collapsed;
#endif
#if SMARTMAUI
                    label.IsVisible = false;
#endif
                }
#endif
#if ANDROIDX
                if (new_visibility == ViewStates.Visible)
                {
                    label.Visibility = ViewStates.Visible;
                }
                else
                {
                    label.Visibility = ViewStates.Invisible;
                }
#endif
            }
            return;
        }


#if WINFORMS
        internal static void SetRadioButtonVisibility(System.Windows.Forms.RadioButton button, bool new_visibility)
#endif
#if WPF
        internal static void SetRadioButtonVisibility(RadioButton button, System.Windows.Visibility new_visibility)
#endif
#if WINUI
        internal static void SetRadioButtonVisibility(RadioButton button, Visibility new_visibility)
#endif
#if ANDROIDX
        internal static void SetRadioButtonVisibility(Android.Widget.RadioButton button, bool new_visibility)
#endif
#if SMARTMAUI
        internal static void SetRadioButtonVisibility(Microsoft.Maui.Controls.RadioButton button, Visibility new_visibility)
#endif
        {
            if (button != null)
            {
#if WINFORMS
                if (new_visibility == true)
                {
                    button.Visible = true;
                }
                else
                {
                    button.Visible = false;
                }
#endif
#if WPF  || WINUI || SMARTMAUI
                if (new_visibility == Visibility.Visible)
                {
#if SMARTMAUI
                    button.IsVisible = true;
#else
                    button.Visibility = Visibility.Visible;
#endif
                }
                else
                {
#if WPF
                    button.Visibility = Visibility.Hidden;
#endif
#if WINUI
                    button.Visibility = Visibility.Collapsed;
#endif
#if SMARTMAUI
                    button.IsVisible = false;
#endif
                }
#endif
#if ANDROIDX
                if (new_visibility == false)
                {
                    button.Visibility = ViewStates.Visible;
                }
                else
                {
                    button.Visibility = ViewStates.Invisible;
                }
#endif
                }
                return;
        }

#if WINFORMS
        internal static string GetLabelContent(string label_content)
#endif
#if WPF  || WINUI
        internal static string GetLabelContent(string label_content)
#endif
#if ANDROIDX
        internal static string GetLabelContent(string label_content)
#endif
#if SMARTMAUI
        internal static string GetLabelContent(string label_content)
#endif
        {
            string content = "";
            if (!(string.IsNullOrEmpty(label_content)))
            {
                content = label_content;
            }
            return content;
        }

#if WPF  || WINUI || SMARTMAUI
#if WPF
        internal static ImageSource GetImageSource(System.Windows.Controls.Image image)
#endif
#if WINUI
        internal static ImageSource GetImageSource(Microsoft.UI.Xaml.Controls.Image image)
#endif
#if SMARTMAUI
        internal static ImageSource GetImageSource(Microsoft.Maui.Controls.Image image)
#endif
        {
            ImageSource source = null; // "keasdon_energy_small.jpg";
            if (image != null)
            {
                source = image.Source;
            }
            return source;
        }
#endif

#if ANDROIDX
        internal static ImageView GetImageSource(ImageView image, string source)
        {
            int resourceId = (int)typeof(Resource.Drawable).GetField(source).GetValue(null);
            image.SetImageResource(resourceId);// Resource.Drawable.source);
            return image;
            //ImageView source = null;// = "Images/keasdon_energy_small.jpg";
            //if (image != null)
            //{
            //    source = image.Source;
            //}
            //return source;
        }
#endif

#if WPF  || WINUI || SMARTMAUI
#if WPF
        internal static void SetImageSource(System.Windows.Controls.Image image, ImageSource source)
#endif
#if WINUI
        internal static void SetImageSource(Microsoft.UI.Xaml.Controls.Image image, ImageSource source)
#endif
#if SMARTMAUI
        internal static void SetImageSource(Microsoft.Maui.Controls.Image image, ImageSource source)
#endif
        {
            if (image != null &&
                source != null)
            {
                image.Source = source;
            }
            return;
        }
#endif

#if ANDROIDX
        internal static void SetImageSource(ImageView image, string source)
        {
            int resourceId = (int)typeof(Resource.Drawable).GetField(source).GetValue(null);
            image.SetImageResource(resourceId);// Resource.Drawable.source);
            //if (image != null &&
            //    source != null)
            //{
            //    image.Source = source;
            //}
            return;
        }
#endif

#if WINFORMS
        internal static System.Drawing.Color GetDatePickerColor_New(System.Drawing.Color foreground, System.Drawing.Color background, bool ground)
#endif
#if WPF  || WINUI
        internal static Brush GetDatePickerColor_New(Brush foreground, Brush background, bool ground)
#endif
#if ANDROIDX
        internal static Android.Graphics.Color GetDatePickerColor_New(Android.Graphics.Color foreground, Android.Graphics.Color background, bool ground)
#endif
#if SMARTMAUI
        internal static Microsoft.Maui.Graphics.Color GetDatePickerColor_SmartMaui(Microsoft.Maui.Graphics.Color foreground, Microsoft.Maui.Graphics.Color background, bool ground)
#endif
        {
#if WINFORMS
            System.Drawing.Color thecolor = System.Drawing.Color.Black;
#endif
#if WPF  || WINUI
            Brush thecolor = new SolidColorBrush(Colors.Black);  // Default
#endif
#if ANDROIDX
            Android.Graphics.Color thecolor = Android.Graphics.Color.Black;  // Default
#endif
#if SMARTMAUI
            Microsoft.Maui.Graphics.Color thecolor = Colors.Black; 
#endif

            if (ground)
            {
                //Color foreground
                thecolor = foreground;
            }
            else
            {
                //Color background 
                thecolor = background;
            }
            return thecolor;
        }

#if WINFORMS
        internal static System.Drawing.Color GetDatePickerColor(DateTimePicker picker, bool ground)
#endif
#if WPF  || WINUI
        internal static Brush GetDatePickerColor(DatePicker picker, bool ground)
#endif
#if ANDROIDX
        internal static Android.Graphics.Color GetDatePickerColor(Android.Widget.DatePicker picker, bool ground)
#endif
#if SMARTMAUI
        internal static Microsoft.Maui.Graphics.Color GetDatePickerColor(Microsoft.Maui.Controls.DatePicker picker, bool ground)
#endif
        {
#if WINFORMS
            System.Drawing.Color thecolor = System.Drawing.Color.Black;  // Default
#endif
#if WPF  || WINUI
            Brush thecolor = new SolidColorBrush(Colors.Black);  // Default
#endif
#if ANDROIDX
            Android.Graphics.Color thecolor = Android.Graphics.Color.Black;  // Default
#endif
#if SMARTMAUI
            Microsoft.Maui.Graphics.Color thecolor = Colors.Black;
#endif
            if (picker != null)
            {
                if (ground)
                {
                    //Color foreground
#if WINFORMS
                    thecolor = picker.CalendarForeColor;
#endif
#if WPF  || WINUI
                    thecolor = picker.Foreground;
#endif
#if ANDROIDX
                    thecolor = Android.Graphics.Color.Black; // picker.GetBackgroundColor;
#endif
#if SMARTMAUI
                    thecolor = picker.TextColor;
#endif
                }
                else
                {
                    //Color background 
#if WINFORMS
                    thecolor = picker.CalendarForeColor; // Needs re-doing!! 
#endif
#if WPF  || WINUI
                    thecolor = picker.Background;
#endif
#if ANDROIDX
                    thecolor = Android.Graphics.Color.Black; // picker.GetBackgroundColor;
#endif
#if SMARTMAUI
                    thecolor = picker.BackgroundColor;
#endif
                }
            }
            return thecolor;
        }

#if WINFORMS
        internal static bool CheckColor(Label led, System.Drawing.Color shade)
#endif
#if WPF
        internal static bool CheckColor(Label led, Brush shade)
#endif
#if WINUI
        internal static bool CheckColor(TextBlock led, Brush shade)
#endif
#if ANDROIDX
        internal static bool CheckColor(TextView led, Android.Graphics.Color shade)
#endif
#if SMARTMAUI
        internal static bool CheckColor(Label led, Microsoft.Maui.Graphics.Color shade)
#endif
        {
            if (led != null)
            {
#if WINFORMS
                if (led.ForeColor == System.Drawing.Color.FromArgb(shade.A, shade.R, shade.G, shade.B))
                {
                    return true;
                }
#endif
#if WPF
                if (led.Foreground.ToString(SmartParametersV2016.defaultCulture) == shade.ToString(SmartParametersV2016.defaultCulture))
                {
                    return true;
                }
#endif
#if WINUI || MAUI
                if (led.Foreground.ToString() == shade.ToString())
                {
                    return true;
                }
#endif
#if ANDROIDX
                //if (led. == shade)
                //{
                return true;
                //}
#endif
#if SMARTMAUI
                if (led.TextColor == shade)
                {
                    return true;
                }
#endif
            }
            return false;
        }

#if WINFORMS
        internal static void SetColor(Label led, System.Drawing.Color shade)
#endif
#if WPF
        internal static void SetColor(Label led, System.Windows.Media.Color shade)
#endif
#if WINUI
        internal static void SetColor(TextBlock led, Windows.UI.Color shade)
#endif
#if ANDROIDX
        internal static void SetColor(TextView led, Android.Graphics.Color shade)
#endif
#if SMARTMAUI
        internal static void SetColor(Label led, Microsoft.Maui.Graphics.Color shade)
#endif
        {
            if (led != null)
            {
#if WINFORMS
                led.ForeColor = System.Drawing.Color.FromArgb(shade.A, shade.R, shade.G, shade.B);
#endif
#if WPF  || WINUI || MAUI
                led.Foreground = new SolidColorBrush(shade);
#endif
#if ANDROIDX
                led.SetBackgroundColor(shade);
#endif
#if SMARTMAUI
                led.TextColor = shade;               
#endif
            }
            return;
        }

#if WINFORMS
        internal static void SetDatePickerColor_New(System.Drawing.Color background, System.Drawing.Color shade)
#endif
#if WPF  || WINUI
        internal static void SetDatePickerColor_New(Brush background, Brush shade)
#endif
#if ANDROIDX
        internal static void SetDatePickerColor_New(Android.Graphics.Color background, Android.Graphics.Color shade)
#endif
#if SMARTMAUI
        internal static void SetDatePickerColor_New(Microsoft.Maui.Graphics.Color background, Microsoft.Maui.Graphics.Color shade)
#endif
        {
            background = shade;

            return;
        }

        //#if WINFORMS
        //        internal static void SetDatePickerValue(//DateTime picker, 
        //                                                DateTime date)
        //#endif
        //#if WPF  || WINUI  || MAUI
        //        internal static void SetDatePickerValue(//rf DateTime picker, 
        //                                                DateTime date)
        //#endif
        //#if ANDROIDX
        //        internal static void SetDatePickerValue(//rf DateTime picker, 
        //                                                DateTime date)
        //#endif
        //        {
        //            picker = date;
        //            return;
        //        }

        //#if ANDROIDX
        //        internal static void SetComboColorX(NumberPicker box, Android.Graphics.Color shade)
        //#endif
        //        {

        //#if X!ANDROID
        //            if (box != null &&
        //                shade != null)
        //            {
        //                box.BackgroundColor = shade;
        //#Xelse
        //            if (box != null)
        //            {
        //                box.SetBackgroundColor(shade);
        //#endif
        //            }
        //            return;
        //        }
#if ANDROIDX
        internal static void SetVisibility(TextView textBox, Android.Views.ViewStates new_visibility)
        {
            if (textBox != null)
            {
                textBox.Visibility = new_visibility;
            }
            return;
        }
#endif

#if WPF  || WINUI || SMARTMAUI

#if WPF  || WINUI
        internal static string CheckBox2(Selector dropDown)
#endif
#if SMARTMAUI
        internal static string CheckBox2(Picker dropDown)
#endif
        {
            if (dropDown != null &&
                dropDown.SelectedItem != null)
            {
                //ComboBoxItem selected_item = (ComboBoxItem)dropDown.SelectedItem;
                return dropDown.SelectedItem.ToString();
            }
            return "";
        }

#if WPF  || WINUI
        internal static int GetBox(Selector dropDown)
#endif
#if SMARTMAUI
        internal static int GetBox(Picker dropDown)
#endif
        {
            if (dropDown != null &&
                dropDown.SelectedItem != null)
            {
                return dropDown.SelectedIndex;  // This may even be -1 ...
            }
            return -1;
        }
#endif

#if WPF  || WINUI
        internal static DataGridTextColumn TextColumn(string header, string bindingPath)
        {
#if WINUI
            Microsoft.UI.Xaml.Data.Binding binding = new Microsoft.UI.Xaml.Data.Binding()
            {
                Source = bindingPath
            };
#endif
#if SMARTMAUI
            Binding binding = new Binding()
            {
                Source = bindingPath
            };
#endif
#if WPF
            var binding = new Binding(bindingPath);
#endif
            DataGridTextColumn text_column = new DataGridTextColumn
            {
                Header = header,
                Binding = binding
            };
            return text_column;
        }
#endif

//#if WPF  || WINUI || SMARTMAUI

//        internal static Label TextColumn(string bindingPath, string v)
//        {
//            var label = new Label
//            {
//                VerticalOptions = LayoutOptions.Center,
//                HorizontalOptions = LayoutOptions.Start
//            };

//            label.SetBinding(Label.TextProperty, bindingPath);

//            return label;
//        }
//#endif

#if WPF  || WINUI
        internal static DataGrid CreateDataGrid(DataGridTextColumn textColumn, string header)
        {
            DataGrid child_datagrid = new DataGrid()
            {
                IsReadOnly = true
            };
            child_datagrid.Columns.Add(textColumn);
#if WPF
            child_datagrid.Items.Add(new { description = header });
#endif
#if WINUI
            child_datagrid.ItemsSource = new object[] { header };
#endif
#if SMARTMAUI
            child_datagrid.ItemsSource = new object[] { header };
#endif
            return child_datagrid;
        }
#endif

#if SMARTMAUI
        internal static CollectionView CreateDataGrid(string bindingPath, string header)
        {
            var collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.None,
                ItemsSource = new List<object>
                {
                    new { Value = header }
                },

                ItemTemplate = new DataTemplate(() =>
                {
                    var label = new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Start
                    };

                    label.SetBinding(Label.TextProperty, "Value");

                    return new Grid
                    {
                        Children = { label }
                    };
                })
            };

            return collectionView;
        }
#endif

#if WINFORMS
        internal static void FullScreenButtonClickActual(//MainProcess components,
                                                        MainViewModel ourviewmodel)
        {
            // Well ... its not perfect but its 1000 times better than it was ...
            if (!ourviewmodel.isfullscreen)
            {

            }
            return;
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        internal static ScaleTransform Calc_ScaleX(double width, double height)
        {
            ScaleTransform scale = new ScaleTransform
            {
                ScaleX = width,
                ScaleY = height
            };
            return scale;
        }
#endif
        
#if WPF  || WINUI
        internal static void FullScreenButtonClickActual(MainMeter components,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            // Only for WPF and WINUI?
            if (signinviewmodel.Platform == SmartParametersV2016.WPF ||
                signinviewmodel.Platform == SmartParametersV2016.WinUI)
            {
                // Cannot use DependencyService in WPF ... or at least I
                // can't figure out how the fucking thing works
                // Declare an interface instance.

                IScreen screen_object = new ScreenService(ourviewmodel);
                // Call the member.
                screen_object.SwitchScreen(false, components); // We AREN'T quitting
            }
            return;
        }
#endif


#if SMARTMAUI
        internal static void FullScreenButtonClickActual(Microsoft.Maui.Controls.Grid greed,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel)
        {
            // Only for WPF and WINUI?
            if (signinviewmodel.Platform == SmartParametersV2016.WPF ||
                signinviewmodel.Platform == SmartParametersV2016.WinUI)
            {
                // Cannot use DependencyService in WPF ... or at least I
                // can't figure out how the fucking thing works
                // Declare an interface instance.
                IScreen screen_object = new ScreenService(ourviewmodel);
                // Call the member.
                screen_object.SwitchScreen(false, greed); // We AREN'T quitting
            }
            return;
        }
#endif
#if ANDROIDX
        internal static void FullScreenButtonClickActual(MainMeter components,
                                                            SignInViewModel signinviewmodel,
                                                            MainViewModel ourviewmodel)
        {
            // Only for WPF and WINUI - Android is always full screen as is WINFORMS
            // But - OF COURSE - it always fucks up the CarouselView!  No surprise there!!
            if (signinviewmodel.Platform == SmartParametersV2016.WinUI)
            {
#if WPF  || WINUI || MAUI
                DependencyService.Get<IScreen>().SwitchScreen(false);   // We are not Quitting
#endif
            }
            return;
        }
#endif

#if WINFORMS
        internal static bool OpenCloseClickedActual(MainViewModel ourviewmodel,
                                                        bool user_isloggedin)
#endif
#if WPF  || WINUI
        internal static bool OpenCloseClickedActual(MainViewModel ourviewmodel,
                                                        bool user_isloggedin)
#endif
#if ANDROIDX
        internal static bool OpenCloseClickedActual(MainViewModel ourviewmodel,
                                                        bool user_isloggedin)
#endif
#if SMARTMAUI
        internal static async Task<bool> OpenCloseClickedActual(MainViewModel ourviewmodel,
                                                        bool user_isloggedin)
#endif
        {
#if WINFORMS
            MoveTheDoorsAsync(ourviewmodel);
#endif
#if WPF  || WINUI
            if (!MoveTheDoorsAsync(ourviewmodel))
            {
                return false;
            }
#endif
#if ANDROIDX
            if (!MoveTheDoorsAsync(ourviewmodel))
            {
                return false;
            }
#endif
#if SMARTMAUI
            if (!await MoveTheDoorsAsync(ourviewmodel))
            {
                return false;
            }
#endif
            return true;
        }

#if WINFORMS
        internal static bool MoveTheDoorsAsync(MainViewModel ourviewmodel)
#endif
#if WPF
        internal static bool MoveTheDoorsAsync(MainViewModel ourviewmodel)
#endif
#if WINUI
        internal static bool MoveTheDoorsAsync(MainViewModel ourviewmodel)
#endif
#if ANDROIDX
        internal static bool MoveTheDoorsAsync(MainViewModel ourviewmodel)
#endif
#if SMARTMAUI
        internal static async Task<bool> MoveTheDoorsAsync(MainViewModel ourviewmodel)
#endif
        {
            // RED      - doors are open
            // GREEN    - doors are closed
#if ANDROIDX
            ourviewmodel.MeterTop.PivotY = 0;
            ourviewmodel.MeterBottom.PivotY = ourviewmodel.MeterBottom.Height;
            if (ourviewmodel.isExpanded == 1.0f)
            {
                ourviewmodel.isExpanded = 0.0f;
            }
            else
            {
                ourviewmodel.isExpanded = 1.0f;
            }

            ObjectAnimator topScale = ObjectAnimator.OfPropertyValuesHolder(
                                ourviewmodel.MeterTop,
                                PropertyValuesHolder.OfFloat("scaleY", ourviewmodel.isExpanded));
            ObjectAnimator bottomScale = ObjectAnimator.OfPropertyValuesHolder(
                            ourviewmodel.MeterBottom,
                            PropertyValuesHolder.OfFloat("scaleY", ourviewmodel.isExpanded));
            topScale.SetDuration(2000);
            bottomScale.SetDuration(2000);
            // For reasons which I don't quite understand..
            // .. these two seem to work together quite happily
            topScale.Start();
            bottomScale.Start();
#endif
            if (!ourviewmodel.doorDirection)
            {
                // Button is GREEN
                // We are OPENING the Doors
#if WINFORMS
                ourviewmodel.timerDoor.Start();
                ourviewmodel.OpenCloseColour = ourviewmodel.redColour;             
#endif
#if WPF  || WINUI
                // door_direction set in shrink_the_doors routine
                ourviewmodel.ShrinkTheDoors.Begin();
                ourviewmodel.OpenCloseColour = ourviewmodel.redColour;
#endif
#if ANDROIDX
                // We are OPENing (shrinking) the Doors

                // So target Y needs to be 0.0f
               
                //ourviewmodel.AMeterTop.PivotY = 0;
                //ourviewmodel.AMeterBottom.PivotY = ourviewmodel.AMeterBottom.Height;
                //ObjectAnimator topScale = ObjectAnimator.OfPropertyValuesHolder(
                //                ourviewmodel.MeterTop,
                //                PropertyValuesHolder.OfFloat("scaleY", 0.0f));
                //ObjectAnimator bottomScale = ObjectAnimator.OfPropertyValuesHolder(
                //                ourviewmodel.MeterBottom,
                //                PropertyValuesHolder.OfFloat("scaleY", 0.0f));
                //topScale.SetDuration(SmartParametersV2016.doorDuration);
                //bottomScale.SetDuration(SmartParametersV2016.doorDuration);
                //SmartParametersV2016.doorDuration = 2000;
                //// For reasons which I don't quite understand..
                //// .. these two seem to work together quite happily
                //topScale.Start();
                //bottomScale.Start();

                // Doors should now be OPEN
                ourviewmodel.OpenClose0 = ourviewmodel.openred0;
                ourviewmodel.OpenClose25 = ourviewmodel.openred25;
                ourviewmodel.OpenClose50 = ourviewmodel.openred50;
                ourviewmodel.OpenClose75 = ourviewmodel.openred75;
#endif
#if SMARTMAUI
                // door_direction set in shrink_the_doors routine
                //ourviewmodel.ShrinkTheDoors.Commit(this, "ShrinkTheDoors");
                await Task.WhenAll(
                    ourviewmodel.MeterTop.ScaleYTo(0.01, 5000),
                    ourviewmodel.MeterBottom.ScaleYTo(0.01, 5000)
                );

                ourviewmodel.OpenCloseColour = ourviewmodel.redColour;
#endif

                // DOORS should now be open (with Button RED)
                ourviewmodel.doorDirection = true;
                // Hide 'em!!
#if WPF  || WINUI || SMARTMAUI
                ourviewmodel.MeterVisible = false;
#endif
#if ANDROIDX
                ourviewmodel.MeterVisible = false;
#endif
                return true;
            }
            else
            {
                // We are CLOSING the Doors
                // Button is RED
#if WPF  || WINUI || SMARTMAUI
                ourviewmodel.MeterVisible = true;
#endif
#if ANDROIDX
                ourviewmodel.MeterVisible = true;
#endif
#if WINFORMS
                ourviewmodel.timerDoor.Start();
                ourviewmodel.OpenCloseColour = ourviewmodel.greenColour;

#endif
#if WPF  || WINUI
                // door_direction set in expand_the_doors routines
                ourviewmodel.ExpandTheDoors.Begin();
                ourviewmodel.OpenCloseColour = ourviewmodel.greenColour;
#endif
#if ANDROIDX
                //if (user_isloggedin)
                //{
                //    // Doesn't block
                //    await Task.WhenAll(ourviewmodel.MeterTop.ScaleYTo(1, 250, Easing.Linear),
                //                    ourviewmodel.MeterBottom.ScaleYTo(1, 250, Easing.Linear));
                //}
                //else
                //{
                // Doesn't block either ... I don't know WHY this fucker works ... but it does!
                // It lets me close the doors before setting the 'doors_are_open' to false
                // so that when we enter the TimerTick, we can shutdown gently
                // We are CLOSING the Doors

                // So target Y needs to be 1.0f
                //ourviewmodel.AMeterTop.PivotY = 0;
                //ourviewmodel.AMeterBottom.PivotY = ourviewmodel.AMeterBottom.Height;
                //ObjectAnimator topScale = ObjectAnimator.OfPropertyValuesHolder(
                //                ourviewmodel.AMeterTop,
                //                PropertyValuesHolder.OfFloat("scaleY", 1.0f));
                //ObjectAnimator bottomScale = ObjectAnimator.OfPropertyValuesHolder(
                //                ourviewmodel.AMeterBottom,
                //                PropertyValuesHolder.OfFloat("scaleY", 1.0f));
                //topScale.SetDuration(SmartParametersV2016.doorDuration);
                //bottomScale.SetDuration(SmartParametersV2016.doorDuration);
                //// For reasons which I don't quite understand..
                //// .. these two seem to work together quite happily
                //topScale.Start();
                //bottomScale.Start();

                // DOORS should now be closed
                ourviewmodel.OpenClose0 = ourviewmodel.opengreen0;
                ourviewmodel.OpenClose25 = ourviewmodel.opengreen25;
                ourviewmodel.OpenClose50 = ourviewmodel.opengreen50;
                ourviewmodel.OpenClose75 = ourviewmodel.opengreen75;
#endif
#if SMARTMAUI
                // door_direction set in expand_the_doors routines
                await Task.WhenAll(
                    ourviewmodel.MeterTop.ScaleYTo(1, 5000, Easing.CubicOut),
                    ourviewmodel.MeterBottom.ScaleYTo(1, 5000, Easing.CubicOut)
                    );
                ourviewmodel.OpenCloseColour = ourviewmodel.greenColour;
#endif



                // Doors should now be CLOSED (with button GREEN)
                ourviewmodel.doorDirection = false;
            }
            return true;
        }

        internal static async void Record_ScreenSwitch(
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel)
        {
            if (ourviewmodel.Hamas.consumersList.Count > 0)
            {
                bool do_something = false;
                // Find which one to take out
                foreach (SmartUsers.Consumers consumer in ourviewmodel.Hamas.consumersList) // Checked
                {
                    // If they match (!) either True or False, then do nothing otherwise send the change
                    if (consumer.FULL_SCREEN != ourviewmodel.isfullscreen)
                    {
                        consumer.FULL_SCREEN = ourviewmodel.isfullscreen;
                        //SmartUsers.Consumers consumer_row = consumer;
                        consumer.Updated = true;
                        ourviewmodel.Hamas.consumersChangesList.Add(consumer);
                        // Go and update it
                        do_something = true;
                    }
                }

                if (do_something)
                {
                    if (await DoAllSmartUsers(ourviewmodel,
                                               SmartParametersV2016.sqliteformat))
                    {
                        string screensize = ourviewmodel.isfullscreen ? "Full" : "Normal";
                        // Tell the console we have switched
                        await TextBlockUpdate(
#if ANDROIDX
                                               meterActivity,
#endif
                                                ourviewmodel,
                                              "Screen switched to: " +
                                              screensize);
                        return;
                    }
                }
            }
            return;
        }

        // Mobility <= mortality !!!!
        internal static async Task<bool> RecordCubeSwitch(
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                            char cubeface_code)
        {
            if (ourviewmodel.SmartProfile.profilecubefacesList.Count > 0)
            {
                bool do_something = false;
                //string face;
                // Find which one to take out
                foreach (SmartProfile.Cubefaces cubeface in ourviewmodel.SmartProfile.profilecubefacesList) // Checked
                {
                    if (cubeface.USERNAME == ourviewmodel.UserName)
                    {
                        if (cubeface.FACE_LAST_DISPLAY == SmartParametersV2016.lastChecked)
                        {
                            if (cubeface.CUBEFACE_CODE != cubeface_code)
                            {
                                cubeface.FACE_LAST_DISPLAY = "";
                                //SmartProfile.Cubefaces cubeface_row = cubeface;
                                cubeface.Updated = true;
                                ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubeface);
                                // Go and update it
                                do_something = true;
                            }
                        }
                        else
                        {
                            if (cubeface.CUBEFACE_CODE == cubeface_code)
                            {
                                cubeface.FACE_LAST_DISPLAY = SmartParametersV2016.lastChecked;
                                //SmartProfile.Cubefaces cubeface_row = cubeface;
                                cubeface.Updated = true;
                                ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubeface);
                                //face = cubeface.CUBEFACE_CODE.ToString();
                                // Go and update it
                                do_something = true;
                            }
                        }
                    }
                }

                if (do_something)
                {
                    if (await DoAllSmartProfile(ourviewmodel,
                                                SmartParametersV2016.sqliteformat))
                    {
                        // Tell the console we have switched
                        if (!await TextBlockUpdate(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel, "Cube switched to: " + cubeface_code))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal static async Task<bool> DefeatTheChimps(MainViewModel ourviewmodel,
                                                        string result,
                                                        string schema_name,
                                                        string table_name)
        {
            bool beenherebefore = true;
            if (ourviewmodel.sqliteDatabase != null)
            {
                List<SmartUsers.ConsumersSQLite> sqlite_tempList = new List<SmartUsers.ConsumersSQLite>();
                foreach (SmartUsers.ConsumersSQLite abc in ourviewmodel.Hamas.sqliteConsumersList) // Checked
                {
                    SmartUsers.ConsumersSQLite xyz = new SmartUsers.ConsumersSQLite()
                    {
                        USERNAME = abc.USERNAME,
                        LOCAL_ONLY = abc.LOCAL_ONLY,
                        FULL_SCREEN = abc.FULL_SCREEN,
                        MULTIMETER_ACTIVE = abc.MULTIMETER_ACTIVE,
                        CONSUMER_CREATED = abc.CONSUMER_CREATED,
                        SmartUsersConsumers = abc.SmartUsersConsumers,
                        SmartProfileCubefaces = abc.SmartProfileCubefaces,
                        SmartProfileGroups = abc.SmartProfileGroups,
                        SmartProfileProfiles = abc.SmartProfileProfiles,
                        //SmartProfileProfilesCube = abc.SmartProfileProfilesCube,
                        SmartProfileAddresses = abc.SmartProfileAddresses,
                        SmartFinanceAccounts = abc.SmartFinanceAccounts,
                        SmartFinanceCategories = abc.SmartFinanceCategories,
                        SmartFinanceCategoryTypes = abc.SmartFinanceCategoryTypes,
                        SmartFinanceLogins = abc.SmartFinanceLogins,
                        SmartFinanceSwitches = abc.SmartFinanceSwitches,
                        SmartFinanceTransactions = abc.SmartFinanceTransactions,
                        SmartFinanceTransactionsCategories = abc.SmartFinanceTransactionsCategories,
                        SmartUtilityAccChargesCredits = abc.SmartUtilityAccChargesCredits,
                        SmartUtilityAccounts = abc.SmartUtilityAccounts,
                        SmartUtilityBankDetails = abc.SmartUtilityBankDetails,
                        SmartUtilityBills = abc.SmartUtilityBills,
                        SmartUtilityBillsResource = abc.SmartUtilityBillsResource,
                        SmartUtilityECosts = abc.SmartUtilityECosts,
                        SmartUtilityEDiscounts = abc.SmartUtilityEDiscounts,
                        SmartUtilityEReadings = abc.SmartUtilityEReadings,
                        SmartUtilityEStandingCharges = abc.SmartUtilityEStandingCharges,
                        SmartUtilityEUnitCharges = abc.SmartUtilityEUnitCharges,
                        SmartUtilityEUsage = abc.SmartUtilityEUsage,
                        SmartUtilityGCosts = abc.SmartUtilityGCosts,
                        SmartUtilityGDiscounts = abc.SmartUtilityGDiscounts,
                        SmartUtilityGReadings = abc.SmartUtilityGReadings,
                        SmartUtilityGStandingCharges = abc.SmartUtilityGStandingCharges,
                        SmartUtilityGUnitCharges = abc.SmartUtilityGUnitCharges,
                        SmartUtilityGUsage = abc.SmartUtilityGUsage,
                        SmartUtilityLogins = abc.SmartUtilityLogins,
                        SmartUtilityMeters = abc.SmartUtilityMeters,
                        SmartUtilityPayments = abc.SmartUtilityPayments,
                        SmartUtilityResources = abc.SmartUtilityResources,
                        SmartUtilityResourcesTypes = abc.SmartUtilityResourcesTypes,
                        SmartUtilitySupChargesCredits = abc.SmartUtilitySupChargesCredits,
                        SmartUtilitySwitches = abc.SmartUtilitySwitches,
                        SmartUtilityTariffDetails = abc.SmartUtilityTariffDetails,
                        SmartUtilityUnallocated = abc.SmartUtilityUnallocated,
                        Updated = abc.Updated // Its an insert
                    };
                    sqlite_tempList.Add(xyz);
                }

                string sql = SmartPhyllV2020.Build_Select(schema_name, table_name, ourviewmodel.UserName);
                try
                {
                    ourviewmodel.Hamas.sqliteConsumersList = new List<SmartUsers.ConsumersSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUsers.ConsumersSQLite>(sql));
                    if (ourviewmodel.Hamas.sqliteConsumersList.Count == 0)
                    {
                        foreach (SmartUsers.ConsumersSQLite xyz in sqlite_tempList) // Checked
                        {
                            SmartUsers.ConsumersSQLite abc = new SmartUsers.ConsumersSQLite()
                            {
                                USERNAME = xyz.USERNAME,
                                LOCAL_ONLY = xyz.LOCAL_ONLY,
                                FULL_SCREEN = xyz.FULL_SCREEN,
                                MULTIMETER_ACTIVE = xyz.MULTIMETER_ACTIVE,
                                CONSUMER_CREATED = xyz.CONSUMER_CREATED,
                                SmartUsersConsumers = xyz.SmartUsersConsumers,
                                SmartProfileCubefaces = xyz.SmartProfileCubefaces,
                                SmartProfileGroups = xyz.SmartProfileGroups,
                                SmartProfileProfiles = xyz.SmartProfileProfiles,
                                //SmartProfileProfilesCube = xyz.SmartProfileProfilesCube,
                                SmartProfileAddresses = xyz.SmartProfileAddresses,
                                SmartFinanceAccounts = xyz.SmartFinanceAccounts,
                                SmartFinanceCategories = xyz.SmartFinanceCategories,
                                SmartFinanceCategoryTypes = xyz.SmartFinanceCategoryTypes,
                                SmartFinanceLogins = xyz.SmartFinanceLogins,
                                SmartFinanceSwitches = xyz.SmartFinanceSwitches,
                                SmartFinanceTransactions = xyz.SmartFinanceTransactions,
                                SmartFinanceTransactionsCategories = xyz.SmartFinanceTransactionsCategories,
                                SmartUtilityAccChargesCredits = xyz.SmartUtilityAccChargesCredits,
                                SmartUtilityAccounts = xyz.SmartUtilityAccounts,
                                SmartUtilityBankDetails = xyz.SmartUtilityBankDetails,
                                SmartUtilityBills = xyz.SmartUtilityBills,
                                SmartUtilityBillsResource = xyz.SmartUtilityBillsResource,
                                SmartUtilityECosts = xyz.SmartUtilityECosts,
                                SmartUtilityEDiscounts = xyz.SmartUtilityEDiscounts,
                                SmartUtilityEReadings = xyz.SmartUtilityEReadings,
                                SmartUtilityEStandingCharges = xyz.SmartUtilityEStandingCharges,
                                SmartUtilityEUnitCharges = xyz.SmartUtilityEUnitCharges,
                                SmartUtilityEUsage = xyz.SmartUtilityEUsage,
                                SmartUtilityGCosts = xyz.SmartUtilityGCosts,
                                SmartUtilityGDiscounts = xyz.SmartUtilityGDiscounts,
                                SmartUtilityGReadings = xyz.SmartUtilityGReadings,
                                SmartUtilityGStandingCharges = xyz.SmartUtilityGStandingCharges,
                                SmartUtilityGUnitCharges = xyz.SmartUtilityGUnitCharges,
                                SmartUtilityGUsage = xyz.SmartUtilityGUsage,
                                SmartUtilityLogins = xyz.SmartUtilityLogins,
                                SmartUtilityMeters = xyz.SmartUtilityMeters,
                                SmartUtilityPayments = xyz.SmartUtilityPayments,
                                SmartUtilityResources = xyz.SmartUtilityResources,
                                SmartUtilityResourcesTypes = xyz.SmartUtilityResourcesTypes,
                                SmartUtilitySupChargesCredits = xyz.SmartUtilitySupChargesCredits,
                                SmartUtilitySwitches = xyz.SmartUtilitySwitches,
                                SmartUtilityTariffDetails = xyz.SmartUtilityTariffDetails,
                                SmartUtilityUnallocated = xyz.SmartUtilityUnallocated,

                                Updated = xyz.Updated
                            };
                            ourviewmodel.Hamas.sqliteConsumersList.Add(abc);
                        }
                        beenherebefore = false;
                    }
                }
                catch (Exception sqlex)
                {
                    ourviewmodel.errorMessage = sqlex.Message;
                    return false;
                }
            }
            if (ourviewmodel.Hamas.sqliteConsumersList.Count > 0)
            {
                foreach (SmartUsers.ConsumersSQLite consumer_row in ourviewmodel.Hamas.sqliteConsumersList) // Checked
                {
                    // WHY WOULD I EVER HAVE MULTIPLE CONSUMERSLIST RETURNED???
                    // IUS THIS REALLY NEEDED??
                    string[] groups = result.Split(SmartParametersV2016.groupSeparator);
                    foreach (string group in groups) // Checked
                    {
                        string[] record_fields = group.Split(SmartParametersV2016.unitSeparator);
                        if (record_fields.Length == 2)
                        {
                            DateTime utcDate = SmartTimeV2016.ConvertDateTime(record_fields[1]);
                            switch (record_fields[0])
                            {
                                case "SmartUsersConsumers":
                                    consumer_row.SmartUsersConsumers = utcDate;
                                    break;
                                case "SmartProfileCubefaces":
                                    consumer_row.SmartProfileCubefaces = utcDate;
                                    break;
                                case "SmartProfileGroups":
                                    consumer_row.SmartProfileGroups = utcDate;
                                    break;
                                case "SmartProfileProfiles":
                                    consumer_row.SmartProfileProfiles = utcDate;
                                    break;
                                //case "SmartProfileProfilesCube":
                                //    consumer_row.SmartProfileProfilesCube = utcDate;
                                //    break;
                                case "SmartProfileAddresses":
                                    consumer_row.SmartProfileAddresses = utcDate;
                                    break;
                                case "SmartFinanceAccounts":
                                    consumer_row.SmartFinanceAccounts = utcDate;
                                    break;
                                case "SmartFinanceCategories":
                                    consumer_row.SmartFinanceCategories = utcDate;
                                    break;
                                case "SmartFinanceCategoryTypes":
                                    consumer_row.SmartFinanceCategoryTypes = utcDate;
                                    break;
                                case "SmartFinanceLogins":
                                    consumer_row.SmartFinanceLogins = utcDate;
                                    break;
                                case "SmartFinanceSwitches":
                                    consumer_row.SmartFinanceSwitches = utcDate;
                                    break;
                                case "SmartFinanceTransactions":
                                    consumer_row.SmartFinanceTransactions = utcDate;
                                    break;
                                case "SmartFinanceTransactionsCategories":
                                    consumer_row.SmartFinanceTransactionsCategories = utcDate;
                                    break;
                                case "SmartUtilityAccChargesCredits":
                                    consumer_row.SmartUtilityAccChargesCredits = utcDate;
                                    break;
                                case "SmartUtilityAccounts":
                                    consumer_row.SmartUtilityAccounts = utcDate;
                                    break;
                                case "SmartUtilityBankDetails":
                                    consumer_row.SmartUtilityBankDetails = utcDate;
                                    break;
                                case "SmartUtilityBills":
                                    consumer_row.SmartUtilityBills = utcDate;
                                    break;
                                case "SmartUtilityBillsResource":
                                    consumer_row.SmartUtilityBillsResource = utcDate;
                                    break;
                                case "SmartUtilityECosts":
                                    consumer_row.SmartUtilityECosts = utcDate;
                                    break;
                                case "SmartUtilityEDiscounts":
                                    consumer_row.SmartUtilityEDiscounts = utcDate;
                                    break;
                                case "SmartUtilityEReadings":
                                    consumer_row.SmartUtilityEReadings = utcDate;
                                    break;
                                case "SmartUtilityEStandingCharges":
                                    consumer_row.SmartUtilityEStandingCharges = utcDate;
                                    break;
                                case "SmartUtilityEUnitCharges":
                                    consumer_row.SmartUtilityEUnitCharges = utcDate;
                                    break;
                                case "SmartUtilityEUsage":
                                    consumer_row.SmartUtilityEUsage = utcDate;
                                    break;
                                case "SmartUtilityGCosts":
                                    consumer_row.SmartUtilityGCosts = utcDate;
                                    break;
                                case "SmartUtilityGDiscounts":
                                    consumer_row.SmartUtilityGDiscounts = utcDate;
                                    break;
                                case "SmartUtilityGReadings":
                                    consumer_row.SmartUtilityGReadings = utcDate;
                                    break;
                                case "SmartUtilityGStandingCharges":
                                    consumer_row.SmartUtilityGStandingCharges = utcDate;
                                    break;
                                case "SmartUtilityGUnitCharges":
                                    consumer_row.SmartUtilityGUnitCharges = utcDate;
                                    break;
                                case "SmartUtilityGUsage":
                                    consumer_row.SmartUtilityGUsage = utcDate;
                                    break;
                                case "SmartUtilityLogins":
                                    consumer_row.SmartUtilityLogins = utcDate;
                                    break;
                                case "SmartUtilityMeters":
                                    consumer_row.SmartUtilityMeters = utcDate;
                                    break;
                                case "SmartUtilityPayments":
                                    consumer_row.SmartUtilityPayments = utcDate;
                                    break;
                                case "SmartUtilityResources":
                                    consumer_row.SmartUtilityResources = utcDate;
                                    break;
                                case "SmartUtilityResourcesTypes":
                                    consumer_row.SmartUtilityResourcesTypes = utcDate;
                                    break;
                                case "SmartUtilitySupChargesCredits":
                                    consumer_row.SmartUtilitySupChargesCredits = utcDate;
                                    break;
                                case "SmartUtilitySwitches":
                                    consumer_row.SmartUtilitySwitches = utcDate;
                                    break;
                                case "SmartUtilityTariffDetails":
                                    consumer_row.SmartUtilityTariffDetails = utcDate;
                                    break;
                                case "SmartUtilityUnallocated":
                                    consumer_row.SmartUtilityUnallocated = utcDate;
                                    break;
                                default:
                                    break;
                            }
#if WPF  || WINUI || SMARTMAUI
                            //await TextBlockUpdate(ourviewmodel, record_fields[0] + ": " + utcDate.ToString(SmartParametersV2016.sqliteformat));
#endif
#if ANDROIDX
                            //await TextBlockUpdate(ourviewmodel, record_fields[0] + ": " + utcDate.ToString(SmartParametersV2016.sqliteformat));
#endif
                        }
                    }
                    consumer_row.Updated = beenherebefore;
                }
            }
            return true;
        }

        internal static async Task<bool> DoAllSmartUsers(MainViewModel ourviewmodel,
                                                         string sqliteformat,
                                                         bool delete = false)   // Treat update = true as delete
        {
            bool status = false;
            // Now do all the Finance.Accounts etc which may be needed for keys?
            //
            // So we can store it in the fucking SQL Server Express datanase in
            // fucking stupid US "English" format
            //
            ourviewmodel.bollocks = new StringBuilder();

            // These can be updated and inserted
            DoAllUsersConsumers(ourviewmodel,
                            sqliteformat);
            if (ourviewmodel.bollocks.Length == 0 ||
                ourviewmodel.UserNameColour != ourviewmodel.greenColour)
            {
                return true;
            }

            // DO I IGNORE LOCALONLY HERE,
            // OR DO I TAKE IT INTO ACCOUNT FOR
            // THIS ONE TABLE????

            // This might return true or false
            if (await SmartBobV2017.Insert_COMMON_Async(ourviewmodel,
                                                        ourviewmodel.quitCts.Token,
                                                        ourviewmodel.bollocks))
            {
                // Update the internal DB
                if (await Update_SQLite_Users(ourviewmodel,
                                                SmartParametersV2016.SmartUsersSchema,
                                                //ourviewmodel.PDEK,
                                                delete))
                {
                    status = true;
                }
            }
            // THank FUCK for that!!
            // If we haven't anything to send ... then that's good because any inserts or updates won't fail!
            return status;
        }

        internal static async Task<bool> Update_SQLite_Users(MainViewModel ourviewmodel,
                                                                string schema_name,
                                                                // string PDEK,  Nothing secure in this one
                                                                bool delete = false)
        {
            // Here we successfully updated the remote SERVER db
            // Now we have to UPDATE the SQLite record already in Profiles
            // or INSERT in the new SQLite record into Profiles
            // We never DELETE records btw
            // But we have to do that with the ENCRYPTED record
            // because WPF.db3 holds ENCRYPTED records

            // Consumers
            if (ourviewmodel.Hamas.sqliteConsumersList.Count > 0)
            {
                foreach (SmartUsers.ConsumersSQLite consumers_row in ourviewmodel.Hamas.sqliteConsumersList) // Checked
                {
                    if (consumers_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                        schema_name,
                                                        "Consumers",
                                                        consumers_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Consumers",
                                                            consumers_row))
                        {
                            return false;
                        }
                    }
                }
                // Clear down all our crimes!
                ourviewmodel.Hamas.sqliteConsumersList.Clear();
                ourviewmodel.Hamas.consumersList = new List<SmartUsers.Consumers>();
                if (!await SmartPhyllV2020.LoadCommonUsers(ourviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Consumers",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                false))  // Not a DBServer request
                                                            //, 
                                                                       //PDEK))
                {
                    return false;
                }
            }            
            return true;
        }

        internal static async Task<bool> DoAllSmartProfile(MainViewModel ourviewmodel,
                                                         string sqliteformat,
                                                         bool delete = false)   // Treat update = true as delete
        {
            bool status = false;
            // Now do all the Finance.Accounts etc which may be needed for keys?
            //
            // So we can store it in the fucking SQL Server Express datanase in
            // fucking stupid US "English" format
            //
            ourviewmodel.bollocks = new StringBuilder();

            // These can be updated and inserted
            DoAllProfileCubefaces(ourviewmodel,
                            sqliteformat);
            DoAllProfileAddressesView(ourviewmodel,
                            sqliteformat);   // <= Because data is encrypted inside
            DoAllProfileGroups(ourviewmodel,
                            sqliteformat,
                            delete);
            DoAllProfileProfiles(ourviewmodel,
                            sqliteformat); // <= Because data is encrypted inside
            if (ourviewmodel.bollocks.Length == 0 ||
                ourviewmodel.UserNameColour != ourviewmodel.greenColour)
            {
                return true;
            }

            // This might return true or false
            if (!ourviewmodel.localOnly)
            {
                status = await SmartBobV2017.Insert_COMMON_Async(ourviewmodel,
                                                        ourviewmodel.quitCts.Token,
                                                        ourviewmodel.bollocks);
            }
            if (status || ourviewmodel.localOnly)
            {
                // Update the internal DB
                status = await Update_SQLite_Profile(ourviewmodel,
                                                SmartParametersV2016.SmartProfileSchema,
                                                ourviewmodel.PDEK,
                                                delete);
            }
            // THank FUCK for that!!
            // If we haven't anything to send ... then that's good because any inserts or updates won't fail!
            return status;
        }

        internal static async Task<bool> Update_SQLite_Profile(MainViewModel ourviewmodel,
                                                                string schema_name,
                                                                string PDEK,
                                                                bool delete = false)
        {
            // Here we successfully updated the remote SERVER db
            // Now we have to UPDATE the SQLite record already in Profiles
            // or INSERT in the new SQLite record into Profiles
            // We never DELETE records btw
            // But we have to do that with the ENCRYPTED record
            // because WPF.db3 holds ENCRYPTED records

            // Cubefaces
            DateTime abc = DateTime.Now;
            if (ourviewmodel.SmartProfile.sqlite_cubefacesList.Count > 0)
            {
                foreach (SmartProfile.CubefacesSQLite cubefaces_row in ourviewmodel.SmartProfile.sqlite_cubefacesList.ToList()) // Checked
                {
                    if (cubefaces_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Cubefaces",
                                                            cubefaces_row))
                        { 
                            return false; 
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                        schema_name,
                                                        "Cubefaces",
                                                        cubefaces_row))
                        { 
                            return false; 
                        }
                    }
                    
                }
                // Clear down all our crimes!
                ourviewmodel.SmartProfile.sqlite_cubefacesList.Clear();
                ourviewmodel.SmartProfile.profilecubefacesList = new List<SmartProfile.Cubefaces>();
                if (!await SmartPhyllV2020.LoadCommonProfile(ourviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Cubefaces",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                PDEK,
                                                false))
                {
                    return false;
                }
            }

            // AddressesView
            if (ourviewmodel.SmartProfile.sqlite_addressesviewList.Count > 0)
            {
                foreach (SmartProfile.AddressesViewSQLite sqlite_addresses_row in ourviewmodel.SmartProfile.sqlite_addressesviewList) // Checked
                {
                    if (sqlite_addresses_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                            schema_name,
                                                            "AddressesView",
                                                            sqlite_addresses_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                        schema_name,
                                                        "AddressesView",
                                                        sqlite_addresses_row))
                        {
                            return false;
                        }
                    }
                }
                // Clear down all our crimes!
                ourviewmodel.SmartProfile.sqlite_addressesviewList.Clear();
                ourviewmodel.SmartProfile.addressesviewList = new List<SmartProfile.AddressesView>();
                if (!await SmartPhyllV2020.LoadCommonProfile(ourviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "AddressesView",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                PDEK,
                                                false)) // No DBServer
                {
                    return false;
                }
            }

            // Groups
            // One of the VERY FEW?? where we DELETE??
            if (ourviewmodel.SmartProfile.sqlite_groupsList.Count > 0)
            {
                string sql_operation = "";
                if (delete)
                {
                    // Only tested to do one at a time!!
                    foreach (SmartProfile.GroupsSQLite sqlite_groups_row in ourviewmodel.SmartProfile.sqlite_groupsList)
                    {
                        sql_operation += sql_operation +
                            "DELETE FROM [SmartProfile.Groups] " +
                            "WHERE " +
                            "USERNAME = '" + sqlite_groups_row.USERNAME + "' " +
                            "AND " +
                            "GROUPNAME = '" + sqlite_groups_row.GROUPNAME + "';";
                    }
                    if (sql_operation.Length > 0)
                    {
                        if (!await SmartPhyllV2020.ExecuteSQLite(ourviewmodel, sql_operation, em => ourviewmodel.errorMessage = em))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    foreach (SmartProfile.GroupsSQLite sqlite_groups_row in ourviewmodel.SmartProfile.sqlite_groupsList)
                    {
                        if (sqlite_groups_row.Updated)
                        {
                            if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                                schema_name,
                                                                "Groups",
                                                                sqlite_groups_row))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Groups",
                                                            sqlite_groups_row))
                            {
                                return false;
                            }
                        }
                    }
                }
                // Clear down all our crimes!
                ourviewmodel.SmartProfile.sqlite_groupsList.Clear();
                ourviewmodel.SmartProfile.profilegroupsList = new List<SmartProfile.Groups>();
                if (!await SmartPhyllV2020.LoadCommonProfile(ourviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Groups",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                PDEK,
                                                false)) // NO DBServer
                {
                    return false;
                }
            }

            // Profiles
            if (ourviewmodel.SmartProfile.sqlite_profilesList.Count > 0)
            {
                // Ok ... KEY_DETAILS is *not really* a Key, 'cos we might
                // want to change it e.g. unmarried name to married name et. al.
                // So we have to use PROFILE_CREATED which should never change!
                foreach (SmartProfile.ProfilesSQLite sqlite_profiles_row in ourviewmodel.SmartProfile.sqlite_profilesList) // Checked
                {
                    if (sqlite_profiles_row.Updated)
                    {
                        if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                                            schema_name,
                                                            "Profiles",
                                                            sqlite_profiles_row))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                                        schema_name,
                                                        "Profiles",
                                                        sqlite_profiles_row))
                        {
                            return false;
                        }
                    }
                }
                // Clear down all our crimes!
                ourviewmodel.SmartProfile.sqlite_profilesList.Clear();
                ourviewmodel.SmartProfile.profilesList = new List<SmartProfile.Profiles>();
                if (!await SmartPhyllV2020.LoadCommonProfile(ourviewmodel,
                                                true,//primary,
                                                schema_name,
                                                "Profiles",
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                ourviewmodel.UserName,
                                                PDEK,
                                                false)) // NO DBServer
                {
                    return false;
                }
            }
            return true;
        }

        internal static void DoAllUsersConsumers(MainViewModel ourviewmodel,
                                        string sqliteformat)
        {
            ourviewmodel.Hamas.sqliteConsumersList.Clear();
            if (ourviewmodel.Hamas.consumersChangesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartUsers.ConsumersSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartUsers.Consumers consumers_row in ourviewmodel.Hamas.consumersChangesList) // Checked
                {
                    // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                    if (consumers_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUsers.ConsumersSQLite abc = ConvertUserConsumers(consumers_row);
                        ourviewmodel.Hamas.sqliteConsumersList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUsers.ConsumersSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Consumers": followed by update_common in SmartDBServer 
                    // for this to work
                    ourviewmodel.bollocks.AppendLine("SmartUsers.Consumers" +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartUsers.Consumers consumers_row in ourviewmodel.Hamas.consumersChangesList) // Checked
                {
                    // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                    if (consumers_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartUsers.ConsumersSQLite abc = ConvertUserConsumers(consumers_row);
                        ourviewmodel.Hamas.sqliteConsumersList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUsers.ConsumersSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Consumers": followed by update_common in SmartDBServer 
                    // for this to work
                    ourviewmodel.bollocks.AppendLine("SmartUsers.Consumers" +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                ourviewmodel.Hamas.consumersChangesList.Clear();
            }
            return;
        }

        internal static SmartUsers.ConsumersSQLite ConvertUserConsumers(SmartUsers.Consumers consumer_row)
        {
            SmartUsers.ConsumersSQLite consumers_enc = new SmartUsers.ConsumersSQLite()
            {
                USERNAME = consumer_row.USERNAME,
                LOCAL_ONLY = Convert.ToInt16(consumer_row.LOCAL_ONLY),
                FULL_SCREEN = Convert.ToInt16(consumer_row.FULL_SCREEN),
                MULTIMETER_ACTIVE = Convert.ToInt16(consumer_row.MULTIMETER_ACTIVE),
                CONSUMER_CREATED = consumer_row.CONSUMER_CREATED,
                SmartUsersConsumers = consumer_row.SmartUsersConsumers,
                SmartProfileCubefaces = consumer_row.SmartProfileCubefaces,
                SmartProfileGroups = consumer_row.SmartProfileGroups,
                SmartProfileProfiles = consumer_row.SmartProfileProfiles,
                //SmartProfileProfilesCube = consumer_row.SmartProfileProfilesCube,
                SmartProfileAddresses = consumer_row.SmartProfileAddresses,
                SmartFinanceAccounts = consumer_row.SmartFinanceAccounts,
                SmartFinanceCryptoAccounts = consumer_row.SmartFinanceCryptoAccounts,
                SmartFinanceCryptoAddresses = consumer_row.SmartFinanceCryptoAddresses,
                SmartFinanceCryptoCurrencyRates = consumer_row.SmartFinanceCryptoCurrencyRates,
                SmartFinanceCryptoLedgers = consumer_row.SmartFinanceCryptoLedgers,
                SmartFinanceCryptoTransactions = consumer_row.SmartFinanceCryptoTransactions,
                SmartFinanceCryptoWallets = consumer_row.SmartFinanceCryptoWallets,
                SmartFinanceCryptoWalletTotals = consumer_row.SmartFinanceCryptoWalletTotals,
                SmartFinanceCategories = consumer_row.SmartFinanceCategories,
                SmartFinanceCategoryTypes = consumer_row.SmartFinanceCategoryTypes,
                SmartFinanceConnections = consumer_row.SmartFinanceConnections,
                SmartFinanceLogins = consumer_row.SmartFinanceLogins,
                SmartFinanceTransactions = consumer_row.SmartFinanceTransactions,
                SmartFinanceTransactionsCategories = consumer_row.SmartFinanceTransactionsCategories,
                SmartFinanceSwitches = consumer_row.SmartFinanceSwitches,
                SmartUtilityAccChargesCredits = consumer_row.SmartUtilityAccChargesCredits,
                SmartUtilityAccounts = consumer_row.SmartUtilityAccounts,
                SmartUtilityBankDetails = consumer_row.SmartUtilityBankDetails,
                SmartUtilityBills = consumer_row.SmartUtilityBills,
                SmartUtilityBillsResource = consumer_row.SmartUtilityBillsResource,
                SmartUtilityECosts = consumer_row.SmartUtilityECosts,
                SmartUtilityEDiscounts = consumer_row.SmartUtilityEDiscounts,
                SmartUtilityEReadings = consumer_row.SmartUtilityEReadings,
                SmartUtilityEStandingCharges = consumer_row.SmartUtilityEStandingCharges,
                SmartUtilityEUnitCharges = consumer_row.SmartUtilityEUnitCharges,
                SmartUtilityEUsage = consumer_row.SmartUtilityEUsage,
                SmartUtilityGCosts = consumer_row.SmartUtilityGCosts,
                SmartUtilityGDiscounts = consumer_row.SmartUtilityGDiscounts,
                SmartUtilityGReadings = consumer_row.SmartUtilityGReadings,
                SmartUtilityGStandingCharges = consumer_row.SmartUtilityGStandingCharges,
                SmartUtilityGUnitCharges = consumer_row.SmartUtilityGUnitCharges,
                SmartUtilityGUsage = consumer_row.SmartUtilityGUsage,
                SmartUtilityLogins = consumer_row.SmartUtilityLogins,
                SmartUtilityMeters = consumer_row.SmartUtilityMeters,
                SmartUtilityPayments = consumer_row.SmartUtilityPayments,
                SmartUtilityResources = consumer_row.SmartUtilityResources,
                SmartUtilityResourcesTypes = consumer_row.SmartUtilityResourcesTypes,
                SmartUtilitySupChargesCredits = consumer_row.SmartUtilitySupChargesCredits,
                SmartUtilitySwitches = consumer_row.SmartUtilitySwitches,
                SmartUtilityTariffDetails = consumer_row.SmartUtilityTariffDetails,
                SmartUtilityUnallocated = consumer_row.SmartUtilityUnallocated,
                Updated = consumer_row.Updated
            };
            return consumers_enc;
        }

        internal static void DoAllProfileCubefaces(MainViewModel ourviewmodel,
                                            string sqliteformat)
        {
            ourviewmodel.SmartProfile.sqlite_cubefacesList.Clear();
            if (ourviewmodel.SmartProfile.profilecubefacesChangesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartProfile.CubefacesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartProfile.Cubefaces cubefaces_row in ourviewmodel.SmartProfile.profilecubefacesChangesList) // Checked
                {
                    // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                    if (cubefaces_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartProfile.CubefacesSQLite abc = ConvertUserCubefaces(cubefaces_row);
                        ourviewmodel.SmartProfile.sqlite_cubefacesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.CubefacesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Cubefaces": followed by update_common in SmartDBServer 
                    // for this to work  NO!! Don't think you need this anymore!!!!!
                    ourviewmodel.bollocks.AppendLine("SmartProfile.Cubefaces" +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartProfile.Cubefaces cubefaces_row in ourviewmodel.SmartProfile.profilecubefacesChangesList) // Checked
                {
                    // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                    if (cubefaces_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartProfile.CubefacesSQLite abc = ConvertUserCubefaces(cubefaces_row);
                        ourviewmodel.SmartProfile.sqlite_cubefacesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.CubefacesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Cubefaces": followed by update_common in SmartDBServer 
                    // for this to work
                    ourviewmodel.bollocks.AppendLine("SmartProfile.Cubefaces" +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                ourviewmodel.SmartProfile.profilecubefacesChangesList.Clear();
            }
            return;
        }

        internal static SmartProfile.CubefacesSQLite ConvertUserCubefaces(SmartProfile.Cubefaces cubeface_row)
        {

            SmartProfile.CubefacesSQLite cubefaces_enc = new SmartProfile.CubefacesSQLite()
            {
                USERNAME = cubeface_row.USERNAME,
                CUBEFACE_CODE = cubeface_row.CUBEFACE_CODE.ToString(),
                FACE_ACTIVE = Convert.ToInt16(cubeface_row.FACE_ACTIVE),
                CUBEFACE_CREATED = cubeface_row.CUBEFACE_CREATED,
                FACE_LAST_DISPLAY = cubeface_row.FACE_LAST_DISPLAY.ToString(),
                FACE_CULTURE_CODE = cubeface_row.FACE_CULTURE_CODE,
                FACE_AUTOSWITCH = cubeface_row.FACE_AUTOSWITCH.ToString(),
                FACE_CURRENCY = Convert.ToInt16(cubeface_row.FACE_CURRENCY),
                NEXT_CONNECTION = cubeface_row.NEXT_CONNECTION,
                Updated = cubeface_row.Updated
            };
            return cubefaces_enc;
        }

        internal static void DoAllProfileAddressesView(MainViewModel ourviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartProfile.AddressesView";
            // Nope, there's no Addresses changes allowed, mate
            // You CAN include the Username with local SQLite .. but
            // *not* with remote SQL Server!!!!
            ourviewmodel.SmartProfile.sqlite_addressesviewList.Clear();
            if (ourviewmodel.SmartProfile.addressesview_changesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartProfile.AddressesViewSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartProfile.AddressesView addressesview_row in ourviewmodel.SmartProfile.addressesview_changesList) // Checked
                {
                    if (addressesview_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartProfile.AddressesViewSQLite abc = ConvertUserAddressesView(ourviewmodel, addressesview_row, deriveds.Length);
                        ourviewmodel.SmartProfile.sqlite_addressesviewList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.AddressesViewSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "AddressesView": followed by update_common in SmartDBServer 
                    // for this to work
                    ourviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                // No changes allowed!!

                //rowAsString = "";
                //foreach (SmartUsers.Addresses addresses_row in ourviewmodel.Hamas.addresses_changesList)
                //{
                //    if (addresses_row.Updated == true)
                //    {
                //        if (!string.IsNullOrEmpty(rowAsString))
                //        {
                //            // Separate each record line with this
                //            rowAsString += SmartParametersV2016.recordSeparator;
                //        }
                //        SmartUsers.AddressesSQLite abc = ConvertUserAddresses(ourviewmodel, addresses_row);
                //        ourviewmodel.Hamas.sqlite_addressesList.Add(abc);
                //        rowAsString += SmartNibbyV2016.Build_StringNew<SmartUsers.AddressesSQLite>(myFields, abc, sqliteformat);
                //    }
                //}
                //if (!string.IsNullOrEmpty(rowAsString))
                //{
                //    // You have to have a case "Addresses": followed by update_common in SmartDBServer 
                //    // for this to work
                //    ourviewmodel.bollocks.AppendLine("SmartUsers.Addresses" +
                //                            SmartParametersV2016.groupSeparator +
                //                            "U" + SmartParametersV2016.groupSeparator +
                //                            rowAsString);
                //}
                ourviewmodel.SmartProfile.addressesview_changesList.Clear();
            }
            return;
        }

        internal static SmartProfile.AddressesViewSQLite ConvertUserAddressesView(MainViewModel ourviewmodel,
                                                                SmartProfile.AddressesView address_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = address_row.UDPRN;

            string key_details = (ourviewmodel.PDEK == "" ?
                key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PDEK, address_row.RANDOMKEY1, derived), "", true));

            string details_unencrypted = address_row.BASIC +
                                SmartParametersV2016.fieldSeparator +
                                address_row.BUILDING_NAME +
                                SmartParametersV2016.fieldSeparator +
                                address_row.BUILDING_NUMBER +
                                SmartParametersV2016.fieldSeparator +
                                address_row.COUNTY +
                                SmartParametersV2016.fieldSeparator +
                                address_row.DOUBLE_DEPENDANT_LOCALITY +
                                SmartParametersV2016.fieldSeparator +
                                address_row.DEPENDANT_THOROUGHFARE +
                                SmartParametersV2016.fieldSeparator +
                                address_row.DEPENDANT_LOCALITY +
                                SmartParametersV2016.fieldSeparator +
                                address_row.ORGANIZATION +
                                SmartParametersV2016.fieldSeparator +
                                address_row.POSTCODE_OUTWARD +
                                SmartParametersV2016.fieldSeparator +
                                address_row.POSTCODE +
                                SmartParametersV2016.fieldSeparator +
                                address_row.POBOX +
                                SmartParametersV2016.fieldSeparator +
                                address_row.SUB_BUILDING_NAME +
                                SmartParametersV2016.fieldSeparator +
                                address_row.THOROUGHFARE +
                                SmartParametersV2016.fieldSeparator +
                                address_row.TOWN +
                                SmartParametersV2016.fieldSeparator +
                                address_row.AREA_CODE.ToString();


            // Time for a new RandomKey2
            address_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details = (ourviewmodel.PDEK == "" ?
                details_unencrypted : SmartEncryptionV2016.DoTheBiz(details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PDEK, address_row.RANDOMKEY2, derived), "", true));

            SmartProfile.AddressesViewSQLite address_enc = new SmartProfile.AddressesViewSQLite()
            {
                USERNAME = address_row.USERNAME,
                KEY_DETAILS = key_details,
                RANDOMKEY1 = address_row.RANDOMKEY1,
                ADDRESS_CREATED = address_row.ADDRESS_CREATED,
                DETAILS = details,
                RANDOMKEY2 = address_row.RANDOMKEY2,
                Updated = address_row.Updated
            };
            return address_enc;
        }

        internal static void DoAllProfileGroups(MainViewModel ourviewmodel,
                                            string sqliteformat,
                                            bool deleteFlag = false) // When true, treat Update = true as 'Delete'
        {
            if (deleteFlag)
            {
                // Now, smarty pants ....delete this 'one' record
                if (ourviewmodel.SmartProfile.profilegroupsChangesList.Count > 0)
                {
                    // Delete all Groups
                    FieldInfo[] myFields = typeof(SmartProfile.GroupsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                    string rowAsString = "";
                    foreach (SmartProfile.Groups group_row in ourviewmodel.SmartProfile.profilegroupsChangesList) // Checked
                    {
                        if (group_row.Updated == true)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            //string sql = sqldelete.Replace("<groupname>", group_row.GROUPNAME);
                            SmartProfile.GroupsSQLite abc = ConvertUserGroups(group_row);
                            ourviewmodel.SmartProfile.sqlite_groupsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.GroupsSQLite>(myFields, abc, sqliteformat);

                            //rowAsString += sql;
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        ourviewmodel.bollocks.AppendLine("SmartProfile.Groups" +
                                                SmartParametersV2016.groupSeparator +
                                                "D" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }
                }
                ourviewmodel.SmartProfile.profilegroupsChangesList.Clear();
            }
            else
            {
                ourviewmodel.SmartProfile.sqlite_groupsList.Clear();
                if (ourviewmodel.SmartProfile.profilegroupsChangesList.Count > 0)
                {
                    FieldInfo[] myFields = typeof(SmartProfile.GroupsSQLite).GetFields(SmartParametersV2016.bindingFlags);

                    string rowAsString = "";
                    foreach (SmartProfile.Groups group_row in ourviewmodel.SmartProfile.profilegroupsChangesList) // Checked
                    {
                        if (group_row.Updated == false)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartProfile.GroupsSQLite abc = ConvertUserGroups(group_row);
                            ourviewmodel.SmartProfile.sqlite_groupsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.GroupsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        ourviewmodel.bollocks.AppendLine("SmartProfile.Groups" +
                                                SmartParametersV2016.groupSeparator +
                                                "I" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }

                    rowAsString = "";
                    foreach (SmartProfile.Groups group_row in ourviewmodel.SmartProfile.profilegroupsChangesList)
                    {
                        // What is this saying?  Its saying you can't update the CATEGORY, SUPPLIER, BRAND or CREATED
                        if (group_row.Updated == true)
                        {
                            if (!string.IsNullOrEmpty(rowAsString))
                            {
                                // Separate each record line with this
                                rowAsString += SmartParametersV2016.recordSeparator;
                            }
                            SmartProfile.GroupsSQLite abc = ConvertUserGroups(group_row);
                            ourviewmodel.SmartProfile.sqlite_groupsList.Add(abc);
                            rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.GroupsSQLite>(myFields, abc, sqliteformat);
                        }
                    }
                    if (!string.IsNullOrEmpty(rowAsString))
                    {
                        ourviewmodel.bollocks.AppendLine("SmartProfile.Groups" +
                                                SmartParametersV2016.groupSeparator +
                                                "U" + SmartParametersV2016.groupSeparator +
                                                rowAsString);
                    }
                    ourviewmodel.SmartProfile.profilegroupsChangesList.Clear();
                }
            }
            return;
        }

        internal static SmartProfile.GroupsSQLite ConvertUserGroups(SmartProfile.Groups groups_row)
        {
            SmartProfile.GroupsSQLite groups_enc = new SmartProfile.GroupsSQLite()
            {
                USERNAME = groups_row.USERNAME,
                GROUPNAME = groups_row.GROUPNAME,
                ACTIVEFLAG = Convert.ToInt16(groups_row.ACTIVEFLAG),
                MARKER = groups_row.MARKER,
                PDEK = groups_row.PDEK,
                SENDF = Convert.ToInt16(groups_row.SENDF),
                FDEK = groups_row.FDEK,
                SENDU = Convert.ToInt16(groups_row.SENDU),
                UDEK = groups_row.UDEK,
                RECEIVEALL = Convert.ToInt16(groups_row.RECEIVEALL),
                DISPLAYNAME = groups_row.DISPLAYNAME,
                Updated = groups_row.Updated //? Just a guess!!! Well it was a bad one
            };
            return groups_enc;
        }
        internal static void DoAllProfileProfiles(MainViewModel ourviewmodel,
                                            string sqliteformat)
        {
            string deriveds = "SmartProfile.Profiles";
            // You CAN include the Username with local SQLite .. but
            // *not* with remote SQL Server!!!!
            ourviewmodel.SmartProfile.sqlite_profilesList.Clear();
            if (ourviewmodel.SmartProfile.profilesChangesList.Count > 0)
            {
                FieldInfo[] myFields = typeof(SmartProfile.ProfilesSQLite).GetFields(SmartParametersV2016.bindingFlags);

                string rowAsString = "";
                foreach (SmartProfile.Profiles profile_row in ourviewmodel.SmartProfile.profilesChangesList) // Checked
                {
                    if (profile_row.Updated == false)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartProfile.ProfilesSQLite abc = ConvertUserProfiles(ourviewmodel, profile_row, deriveds.Length);
                        ourviewmodel.SmartProfile.sqlite_profilesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.ProfilesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Profiles": followed by update_common in SmartDBServer 
                    // for this to work
                    ourviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "I" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }

                rowAsString = "";
                foreach (SmartProfile.Profiles profile_row in ourviewmodel.SmartProfile.profilesChangesList) // Checked
                {
                    if (profile_row.Updated == true)
                    {
                        if (!string.IsNullOrEmpty(rowAsString))
                        {
                            // Separate each record line with this
                            rowAsString += SmartParametersV2016.recordSeparator;
                        }
                        SmartProfile.ProfilesSQLite abc = ConvertUserProfiles(ourviewmodel, profile_row, deriveds.Length);
                        ourviewmodel.SmartProfile.sqlite_profilesList.Add(abc);
                        rowAsString += SmartNibbyV2016.Build_StringNew<SmartProfile.ProfilesSQLite>(myFields, abc, sqliteformat);
                    }
                }
                if (!string.IsNullOrEmpty(rowAsString))
                {
                    // You have to have a case "Profiles": followed by update_common in SmartDBServer 
                    // for this to work
                    ourviewmodel.bollocks.AppendLine(deriveds +
                                            SmartParametersV2016.groupSeparator +
                                            "U" + SmartParametersV2016.groupSeparator +
                                            rowAsString);
                }
                ourviewmodel.SmartProfile.profilesChangesList.Clear();
            }
            return;
        }

        internal static SmartProfile.ProfilesSQLite ConvertUserProfiles(MainViewModel ourviewmodel,
                                                                SmartProfile.Profiles profile_row,
                                                                int derived)
        {
            // Encrypt all of these into DETAILS with randomkey
            string key_details_unencrypted = profile_row.LASTNAME +
                                            SmartParametersV2016.fieldSeparator +
                                            profile_row.FIRSTNAME +
                                            SmartParametersV2016.fieldSeparator +
                                            profile_row.MIDDLENAME +
                                            SmartParametersV2016.fieldSeparator +
                                            profile_row.DATE_OF_BIRTH;

            string key_details_encrypted = (ourviewmodel.PDEK == "" ? key_details_unencrypted : SmartEncryptionV2016.DoTheBiz(key_details_unencrypted, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PDEK, profile_row.RANDOMKEY1, derived), "", true));

            string details = profile_row.TITLE +
                                SmartParametersV2016.fieldSeparator +
                                profile_row.GENDER +
                                SmartParametersV2016.fieldSeparator +
                                profile_row.CONTACT_NO +
                                SmartParametersV2016.fieldSeparator +
                                profile_row.EMAIL_ADDRESS +
                                SmartParametersV2016.fieldSeparator +
                                profile_row.DISPLAYNAME;
            // Time for a new RandomKey2
            profile_row.RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            string details_encrypted = (ourviewmodel.PDEK == "" ? details : SmartEncryptionV2016.DoTheBiz(details, SmartPhyllV2020.BuildEncryptKey(ourviewmodel.PDEK, profile_row.RANDOMKEY2, derived), "", true));

            SmartProfile.ProfilesSQLite profile_enc = new SmartProfile.ProfilesSQLite()
            {
                USERNAME = profile_row.USERNAME,
                PROFILE_CREATED = profile_row.PROFILE_CREATED,
                KEY_DETAILS = key_details_encrypted,
                RANDOMKEY1 = profile_row.RANDOMKEY1,
                DETAILS = details_encrypted,
                RANDOMKEY2 = profile_row.RANDOMKEY2,
                Updated = profile_row.Updated
            };
            return profile_enc;
        }

#if WINFORMS
        internal static void Check_Selected_Text(ComboBox combobox)
        {
            if (combobox.Text.Length > 0)
            {
                if (combobox.Text.Substring(0, 1) == "~")
                {
                    combobox.ForeColor = System.Drawing.Color.Red;
                    combobox.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        combobox.Text = combobox.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation);
                    });
                }
                else
                {
                    if (combobox.Text.Substring(0, 1) == "^")
                    {
                        combobox.ForeColor = System.Drawing.Color.Blue;
                        combobox.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
                        {
                            combobox.Text = combobox.Text.Trim(SmartParametersV2016.placeholderDesignation);
                        });
                    }
                    else
                    {
                        if (combobox.Text.Substring(combobox.Text.Length - 1, 1) == "~")
                        {
                            combobox.ForeColor = System.Drawing.Color.Orange;
                            combobox.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
                            {
                                combobox.Text = combobox.Text.Trim('~').Trim(SmartParametersV2016.placeholderDesignation);
                            });
                        }
                        else
                        {
                            combobox.ForeColor = System.Drawing.Color.Black;
                        }
                    }
                }
            }
            return;
        }

#endif
        internal static decimal ConvertDecimal(string number)
        {
            // Make sure EVERY decimal gets converted to something we know and love ...
            return Convert.ToDecimal(number, SmartParametersV2016.defaultCulture); // en-GB
        }



#if WINFORMS
        internal static List<DataGridViewColumn> CopyColumns(DataGridView from_grid)
        {
            List<DataGridViewColumn> popup_grid = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in from_grid.Columns)
            {
                DataGridViewColumn newcol = new DataGridViewColumn()
                {
                    HeaderText = col.HeaderText
                };
                //newcol. = colBinding;
                newcol = col;
                //if (col. != null)
                //{
                //    newcol.CellType = col.CellType;
                //}
                // DON'T COPY the Width because Description has '*' and that fucks the pop-up
                //newcol.Width = col.Width;
                popup_grid.Add(newcol);
            }
            return popup_grid;
        }
#endif

#if WPF  || WINUI
        internal static List<DataGridTextColumn> CopyColumns(DataGrid from_grid)
        {
            List<DataGridTextColumn> popup_grid = new List<DataGridTextColumn>();
            foreach (DataGridColumn col in from_grid.Columns)
            {
                DataGridTextColumn textcol = col as DataGridTextColumn;
                DataGridTextColumn newcol = new DataGridTextColumn()
                {
                    Header = textcol.Header,
                    Binding = textcol.Binding,
                    HeaderStyle = textcol.HeaderStyle
                };
                if (col.CellStyle != null)
                {
                    newcol.CellStyle = textcol.CellStyle;
                }
                // DON'T COPY the Width because Description has '*' and that fucks the pop-up
                //newcol.Width = col.Width;
                popup_grid.Add(newcol);
            }
            return popup_grid;
        }
#endif

#if SMARTMAUI
        public class ColumnDefinitionModel
        {
            public string Header { get; set; }
            public string BindingPath { get; set; }
            public string Format { get; set; }
            public double Width { get; set; } = -1; // -1 = auto
            public TextAlignment Alignment { get; set; } = TextAlignment.Start;
            public Microsoft.Maui.Controls.Style Style { get; set; }
            public Visibility Visible { get; set; }
        }

        public class CollectionViewFactory
        {
            public static CollectionView BuildCollectionView(List<ColumnDefinitionModel> columns)
            {
                return new CollectionView
                {
                    SelectionMode = Microsoft.Maui.Controls.SelectionMode.Single,

                    ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical),

                    ItemTemplate = new DataTemplate(() =>
                    {
                        var grid = new Grid
                        {
                            ColumnSpacing = 5,
                            RowDefinitions =
                            {
                            new RowDefinition(GridLength.Auto)
                            }
                        };

                        // Build "columns"
                        for (int i = 0; i < columns.Count; i++)
                        {
                            var col = columns[i];

                            grid.ColumnDefinitions.Add(
                                new ColumnDefinition
                                {
                                    Width = col.Width > 0
                                        ? new GridLength(col.Width)
                                        : GridLength.Star
                                });

                            var label = new Label
                            {
                                FontSize = 11,
                                VerticalTextAlignment = TextAlignment.Center,
                                HorizontalTextAlignment = col.Alignment
                            };

                            // Binding (with optional format)
                            Binding binding;

                            if (!string.IsNullOrWhiteSpace(col.Format))
                            {
                                binding = new Binding(col.BindingPath, stringFormat: col.Format);
                            }
                            else
                            {
                                binding = new Binding(col.BindingPath);
                            }

                            label.SetBinding(Label.TextProperty, binding);

                            grid.Add(label, i, 0);
                        }

                        return grid;
                    })
                };
            }

            // COPY ROUTINE (replacement for copying DataGridColumns)
            public static List<ColumnDefinitionModel> CopyColumns(List<ColumnDefinitionModel> source)
            {
                return source
                    .Select(c => new ColumnDefinitionModel
                    {
                        Header = c.Header,
                        BindingPath = c.BindingPath,
                        Format = c.Format,
                        Width = c.Width,
                        Alignment = c.Alignment,
                        Visible = c.Visible
                    })
                    .ToList();
            }

            // OPTIONAL: Convert old-style DataGridTextColumn (WPF migration helper idea)
            public static List<ColumnDefinitionModel> FromLegacyLikeStructure(
                IEnumerable<(string Header, string Path, string Format, double Width, TextAlignment Align)> cols)
            {
                return cols.Select(c => new ColumnDefinitionModel
                {
                    Header = c.Header,
                    BindingPath = c.Path,
                    Format = c.Format,
                    Width = c.Width,
                    Alignment = c.Align
                    //Visible = c.Visible
                }).ToList();
            }
        }
        internal static List<ColumnDefinitionModel> CopyColumns(List<ColumnDefinitionModel> source)
        {
            return source.Select(col => new ColumnDefinitionModel
            {
                Header = col.Header,
                BindingPath = col.BindingPath,
                Style = col.Style,
                Visible = col.Visible
            }).ToList();
        }
        internal static DataTemplate BuildTemplate(List<ColumnDefinitionModel> columns)
        {
            return new DataTemplate(() =>
            {
                var grid = new Grid();
                int colIndex = 0;
                foreach (var col in columns)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    var label = new Label();
                    label.SetBinding(Label.TextProperty, col.BindingPath);
                    if (col.Style != null)
                    {
                        label.Style = col.Style;
                    }
                    grid.Add(label, colIndex, 0);
                    colIndex++;
                }
                return grid;
            });
        }
        internal static Microsoft.Maui.Controls.Grid BuildHeader(List<ColumnDefinitionModel> columns)
        {
            var grid = new Microsoft.Maui.Controls.Grid();
            int colIndex = 0;
            foreach (var col in columns)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.Add(new Label
                {
                    Text = col.Header,
                    FontAttributes = FontAttributes.Bold
                }, colIndex, 0);
                colIndex++;
            }
            return grid;
        }

        internal static List<ColumnDefinitionModel> GetColumnsFromCollectionView(CollectionView collectionView)
        {
            List<ColumnDefinitionModel> columns = new List<ColumnDefinitionModel>();

            var content = collectionView.ItemTemplate.CreateContent();

            if (content is Grid grid)
            {
                foreach (var child in grid.Children)
                {
                    if (child is Label label)
                    {
                        Binding binding = BindingHelpers.GetBinding(label, Label.TextProperty);
                        if (binding != null)
                        {
                            string path = binding.Path;
                        
                            columns.Add(new ColumnDefinitionModel
                            {
                                Header = binding.Path,
                                BindingPath = binding.Path,
                                Style = label.Style,
                                //Visible = label.
                            });
                        }
                    }
                }
            }
            return columns;
        }

        public static class BindingHelpers
        {
            public static Binding GetBinding(BindableObject bindable,
                BindableProperty property)
            {
                var context = typeof(BindableObject)
                    .GetField("_properties",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                var dictionary =
                    context?.GetValue(bindable) as System.Collections.IDictionary;

                if (dictionary == null)
                    return null;

                foreach (System.Collections.DictionaryEntry item in dictionary)
                {
                    var value = item.Value;

                    var bindingField = value.GetType()
                        .GetField("Binding",
                            BindingFlags.Instance | BindingFlags.Public |
                            BindingFlags.NonPublic);

                    if (bindingField != null)
                    {
                        var binding = bindingField.GetValue(value) as Binding;

                        if (binding != null)
                            return binding;
                    }
                }
                return null;
            }
        }
#endif
        internal static void CreateFinanceList(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string whichList,
                                                bool subscriber,
                                                string filePath,
                                                string title)
        {
            int headersize = SmartParametersV2016.HeaderFontSize;
            int bodysize = SmartParametersV2016.BodyFontSize;

            PdfWriter pdfwriter = new PdfWriter(filePath);
            PdfDocument pdfdocument = new PdfDocument(pdfwriter);

            // Create an instance of the document class which represents the PDF document itself.              
            iText.Layout.Document document = new iText.Layout.Document(pdfdocument, pageSize: iText.Kernel.Geom.PageSize.A4, false); // <= no immediate flush so we can write page numbers

            document.SetBottomMargin(50);

            // Add meta information to the document  
            //document.AddAuthor("Micke Blomquist");
            //document.AddCreator("Sample application using iText7");
            //document.AddKeywords("PDF tutorial education");
            //document.AddSubject("Document subject - Describing the steps creating a PDF document");
            //document.AddTitle("The document title - PDF creation using iText7");

            //Paragraph header = new Paragraph("HEADER")
            //.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            //.SetFontSize(20);            
            //document.Add(header);

            Paragraph subheader = new Paragraph(title)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(15);
            document.Add(subheader);

            Table table;
            bool border = true;
            switch (whichList)
            {
                case "Transactions":
                    table = new Table(10, false);

                    BuildTable(table, "Booking Date", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Sort Code", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Account No", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Description", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "Currency", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Code", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Sub Code", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Debits", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Credits", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Balance", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);

                    // Only allowed FOUR currencies
                    double[,] totals = new double[4, 2];
                    foreach (SmartFinance.CommonTransactionsView transview in financeviewmodel.FinanceTransactions) // Checked
                    {
                        if (transview.CREDITDEBIT_INDICATOR == 1)
                        {
                            totals[transview.CURRENCY_ORDINAL, 0] += transview.NATIVE_AMOUNT;
                        }
                        else
                        {
                            totals[transview.CURRENCY_ORDINAL, 1] += transview.NATIVE_AMOUNT;
                        }

                        BuildTable(table, transview.TRANSACTION_DATE.ToString("d", financeviewmodel.CultureINF), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, transview.SORTCODE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, transview.ACCOUNT_NO, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, transview.DESCRIPTION, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, transview.CURRENCY_DISPLAY, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, transview.TRANSGROUP_CODE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, transview.TRANSACTION_CODE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, transview.DEBITS, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, transview.CREDITS, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, transview.BALANCE_DISPLAY, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                    }

                    for (int ordinal = 0; ordinal < 4; ordinal++)
                    {
                        string ConvertFromSymbol = "";
                        int ConvertFromOrdinal = 0;
                        foreach (SmartData.Currencies abcd in ourviewmodel.currenciesList)
                        {
                            if (abcd.ORDINAL == ordinal)
                            {
                                ConvertFromSymbol = abcd.ISOCURRENCYSYMBOL;
                                ConvertFromOrdinal = abcd.ORDINAL;
                                break;
                            }
                        }
                        string Total_Credits = SmartFinanceV2025.DisplayValue(ourviewmodel,
                                                            financeviewmodel,
                                                            financeviewmodel.ConvertToSymbol,
                                                            ConvertFromSymbol,
                                                            ConvertFromOrdinal,
                                                            ourviewmodel.exchangeRate,
                                                           (decimal)(totals[ordinal, 0] / 100.0));

                        string Total_Debits = SmartFinanceV2025.DisplayValue(ourviewmodel,
                                                                financeviewmodel,
                                                                financeviewmodel.ConvertToSymbol,
                                                                ConvertFromSymbol,
                                                                ConvertFromOrdinal,
                                                                ourviewmodel.exchangeRate,
                                                               (decimal)(totals[ordinal, 1] / 100.0));

                        // Trailing space on Totals means its not right up to the margin (hopefully)
                        BuildTable(table, "Totals ", 1, 4, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, ConvertFromSymbol, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, "", 1, 2, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, Total_Debits, 1, 1, 6, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, Total_Credits, 1, 1, 6, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, "", 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);

                        document.Add(table);
                    }
                    break;

                case "ExchangeRates":
                    table = new Table(7, true);
                    border = false;
                    BuildTable(table, "Date", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "ECB", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "GBP", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "EUR", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "USD", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    //BuildTable(table, "CAD", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "JPY", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);

                    foreach (SmartUsers.ExchangeRatesView view in ourviewmodel.ExchangeRatesReduced) // Checked
                    {
                        BuildTable(table, view.TRANSACTION_DATE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.ECB, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.CURRENCY_RATE_01.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.CURRENCY_RATE_02.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.CURRENCY_RATE_03.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        //BuildTable(table, view.CURRENCY_RATE_04.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.CURRENCY_RATE_04.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                    }

                    document.Add(table);

                    break;

                default:
                    break;
            }

            // Line separator
            //LineSeparator lsend = new LineSeparator(new SolidLine());
            //document.Add(lsend);
            //document.Add(lsend);

            // Don't need any totals

            // Page numbers and Watermark
            FixPageNosAndWatermark(subscriber,
                                    pdfdocument,
                                    document);


            // Close everything down
            document.Close();
            pdfdocument.Close();
            pdfwriter.Close();

            return;
        }

        internal static void CreateUtilityList(MainViewModel ourviewmodel, UtilityViewModel utilityviewmodel,
                                                string whichList, bool subscriber, string filePath, string title)
        {
            int headersize = SmartParametersV2016.HeaderFontSize;
            int bodysize = SmartParametersV2016.BodyFontSize;

            PdfWriter pdfwriter = new PdfWriter(filePath);
            PdfDocument pdfdocument = new PdfDocument(pdfwriter);

            // Create an instance of the document class which represents the PDF document itself.              
            iText.Layout.Document document = new iText.Layout.Document(pdfdocument, pageSize: iText.Kernel.Geom.PageSize.A4, false); // <= no immediate flush so we can write page numbers

            document.SetBottomMargin(50);

            // Add meta information to the document  
            //document.AddAuthor("Micke Blomquist");
            //document.AddCreator("Sample application using iText7");
            //document.AddKeywords("PDF tutorial education");
            //document.AddSubject("Document subject - Describing the steps creating a PDF document");
            //document.AddTitle("The document title - PDF creation using iText7");

            //Paragraph header = new Paragraph("HEADER")
            //.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            //.SetFontSize(20);            
            //document.Add(header);

            Paragraph subheader = new Paragraph(title)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(15);
            document.Add(subheader);

            Table table;
            bool border = true;
            switch (whichList)
            {
                case "Costs":
                    if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                    {
                        table = new Table(6, false);
                        BuildTable(table, "Username", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    }
                    else
                    {
                        table = new Table(5, false);
                    }
                    BuildTable(table, "Supplier Name", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "Tariff Name", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "Meter Type", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Payment Name", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "Total Cost", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);

                    foreach (SmartUtility.AnalysisCostsView view in utilityviewmodel.UtilityCosts) // Checked
                    {
                        if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                        {
                            BuildTable(table, SmartRoutinesV2018.GetDisplayName(ourviewmodel, view.USERNAME), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        }
                        BuildTable(table, view.SUPPLIER_NAME, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, view.TARIFF_NAME, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, view.METER_TYPE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.PAYMENT_NAME, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, view.TOTAL_AMOUNT, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                    }
                    document.Add(table);
                    break;
                case "Bills":
                    if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                    {
                        table = new Table(9, false);
                        BuildTable(table, "Username", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    }
                    else
                    {
                        table = new Table(8, false);
                    }
                    BuildTable(table, "Account No", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Bill Date", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Statement Id", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Date", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Code", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Description", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "Amount", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Balance", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);

                    foreach (SmartUtility.AnalysisBillsView view in utilityviewmodel.UtilityBills) // Checked
                    {
                        if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                        {
                            BuildTable(table, SmartRoutinesV2018.GetDisplayName(ourviewmodel, view.USERNAME), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        }
                        BuildTable(table, view.ACCOUNT_NO, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.BILL_DATE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.STATEMENT_ID, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.DATE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.CODE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.DESCRIPTION, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, view.AMOUNT, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.BALANCE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                    }
                    document.Add(table);
                    break;
                case "Readings":
                    if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                    {
                        table = new Table(10, false);
                        BuildTable(table, "Username", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    }
                    else
                    {
                        table = new Table(9, false);
                    }

                    BuildTable(table, "Account No", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Statement Id", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Reading Date", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Meter No", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Read Type", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "This Reading", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "Last Reading", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Units Used", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "UoM", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);

                    foreach (SmartUtility.AnalysisReadingsView view in utilityviewmodel.UtilityReadings) // Checked
                    {
                        if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                        {
                            BuildTable(table, SmartRoutinesV2018.GetDisplayName(ourviewmodel, view.USERNAME), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        }
                        BuildTable(table, view.ACCOUNT_NO, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.STATEMENT_ID, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.READINGS_PERIOD_END.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.METER_SERIAL_NO, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.READ_TYPE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, view.THIS_READ, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.LAST_READ, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.UNITS_USED, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.UNIT_OF_MEASURE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                    }
                    document.Add(table);
                    break;
                case "Breakdown":
                    if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                    {
                        table = new Table(13, false);
                        BuildTable(table, "Username", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    }
                    else
                    {
                        table = new Table(12, false);
                    }

                    BuildTable(table, "From", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "To", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Code", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Description", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.LEFT, ColorConstants.GRAY);
                    BuildTable(table, "Items", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.CENTER, ColorConstants.GRAY);
                    BuildTable(table, "Charges/Discounts", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Day Units", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Day Rate", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Night Units", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Night Rate", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Total Units", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);
                    BuildTable(table, "Total", 1, 1, headersize, border, iText.Layout.Properties.TextAlignment.RIGHT, ColorConstants.GRAY);

                    foreach (SmartUtility.AnalysisBreakdownView view in utilityviewmodel.UtilityBreakdown) // Checked
                    {
                        if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                        {
                            BuildTable(table, SmartRoutinesV2018.GetDisplayName(ourviewmodel, view.USERNAME), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        }
                        BuildTable(table, view.FROM_DATE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.TO_DATE.ToString(), 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.CODE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.DESCRIPTION, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.LEFT);
                        BuildTable(table, view.ITEMS, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.CENTER);
                        BuildTable(table, view.CHARGES_DISCOUNTS, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.DAY_UNITS, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.DAY_RATE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.NIGHT_UNITS, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.NIGHT_RATE, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.TOTAL_UNITS, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                        BuildTable(table, view.TOTAL, 1, 1, bodysize, border, iText.Layout.Properties.TextAlignment.RIGHT);
                    }
                    document.Add(table);
                    break;
                default:
                    break;
            }

            // Line separator
            //LineSeparator lsend = new LineSeparator(new SolidLine());
            //document.Add(lsend);
            //document.Add(lsend);

            // Don't need any totals

            // Page numbers and Watermark
            FixPageNosAndWatermark(subscriber,
                                    pdfdocument,
                                    document);
            // Close everything down
            document.Close();
            pdfdocument.Close();
            pdfwriter.Close();
            return;
        }

        internal static void BuildTable(Table table,
                            string paragraph_name,
                            int rowspan,
                            int colspan,
                            int fontsize,
                            bool border,
                            iText.Layout.Properties.TextAlignment alignment,
                            iText.Kernel.Colors.Color color = null)
        {

            if (color == null)
            {
                iText.Layout.Element.Cell element_cell = new iText.Layout.Element.Cell(rowspan, colspan)
               .SetTextAlignment(alignment)
               .SetFontSize(fontsize)
               .Add(new Paragraph(paragraph_name));
                if (!border)
                {
                    element_cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                }
                table.AddCell(element_cell);
            }
            else
            {
                iText.Layout.Element.Cell element_cell = new iText.Layout.Element.Cell(rowspan, colspan)
               .SetBackgroundColor(color)
               .SetTextAlignment(alignment)
               .SetFontSize(fontsize)
               .Add(new Paragraph(paragraph_name));
                if (!border)
                {
                    element_cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                }
                table.AddCell(element_cell);
            }
        }

        internal static void CreateOxyPlotChart(MainViewModel ourviewmodel,
#if OXYPLOT
            PlotView PopupPlotView, 
#endif
            bool subscriber, string filePath, string title)
        {
            if (!subscriber || filePath == "")
            {
                return;
            }
            //int headersize = SmartParametersV2016.HeaderFontSize;
            //int bodysize = SmartParametersV2016.BodyFontSize;

#if OXYPLOT
            PlotModel popup_plotmodel = PopupPlotView.ActualModel;
#endif
            // In memory of Joy - 8th July 1926 - 25th June 2021 - kind, warm-hearted, generous to a fault                
            // You bring me luck, Joy ...
            string oxyplotPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "UtilityOxyPlot.pdf");
            string sourcePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "UtilityTemp.pdf");
            if (File.Exists(sourcePath))
            {
                File.Delete(sourcePath);
            }
            string destPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "UtilityCharts.pdf");
            if (File.Exists(destPath))
            {
                File.Delete(destPath);
            }
            System.IO.FileStream oxyplotstream = File.Create(oxyplotPath);
            float height = iText.Kernel.Geom.PageSize.A4.GetHeight();
            float width = iText.Kernel.Geom.PageSize.A4.GetWidth();
#if WINFORMS || WPF  || WINUI
            OxyPlot.SkiaSharp.PdfExporter pdfExporter = new OxyPlot.SkiaSharp.PdfExporter()

            // Can't use OxyPlot.SkiaSharp in WINUI because to get
            // the plots to work I must use OxyPlot.Core 2.0 which is
            // not compatible with OxyPlot.SkiaSharp
#if WINUI || SMARTMAUI
            //OxyPlot.PdfExporter pdfExporter = new OxyPlot.PdfExporter
#endif
            {
                Width = width,
                Height = height / 2
            };
#if OXYPLOT
            pdfExporter.Export(popup_plotmodel, oxyplotstream);
#endif
            oxyplotstream.Close();
#endif
            PdfWriter pdfwriter = new PdfWriter(sourcePath);
            PdfDocument pdfdocument = new PdfDocument(pdfwriter);
            float topmargin = 50;

            // Create an instance of the document class which represents the PDF document itself.              
            iText.Layout.Document document = new iText.Layout.Document(pdfdocument, pageSize: iText.Kernel.Geom.PageSize.A4, false); // <= no immediate flush so we can write page numbers

            document.SetBottomMargin(topmargin);
            Paragraph subheader = new Paragraph(title)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(15);
            document.Add(subheader);

            FixPageNosAndWatermark(ourviewmodel.subscriber,
                                pdfdocument,
                                document);
            document.Close();
            pdfdocument.Close();
            pdfwriter.Close();

            ManipulatePdf(sourcePath, destPath, oxyplotPath, topmargin * 4);

            if (File.Exists(oxyplotPath))
            {
                File.Delete(oxyplotPath);
            }
            if (File.Exists(sourcePath))
            {
                File.Delete(sourcePath);
            }
            return;
        }

        //https://kb.itextpdf.com/home/it7kb/examples/superimposing-content-from-one-pdf-into-another-pdf
        internal static void ManipulatePdf(string source, string dest, string extra, float topmargin)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(source), new PdfWriter(dest));
            PdfCanvas canvas = new PdfCanvas(pdfDoc.GetFirstPage().NewContentStreamBefore(),
                    pdfDoc.GetFirstPage().GetResources(), pdfDoc);

            PdfDocument sourceDoc = new PdfDocument(new PdfReader(extra));
            PdfFormXObject page = sourceDoc.GetFirstPage().CopyAsFormXObject(pdfDoc);
            canvas.AddXObjectAt(page, 0, topmargin);  // x is distance from Left, y is distance from Bottom !
            sourceDoc.Close();

            pdfDoc.Close();
        }

        internal static void FixPageNosAndWatermark(bool subscriber,
                                                    PdfDocument pdfdocument,
                                                    iText.Layout.Document document)
        {
            // Thanks to Ivan
            // https://stackoverflow.com/questions/44836260/itext-7-add-and-remove-watermark-on-a-pdf
            //iText.Kernel.Geom.Rectangle ps = pdfdocument.GetDefaultPageSize();

            // Page numbers
            int n = pdfdocument.GetNumberOfPages();
            for (int i = 1; i <= n; i++)
            {
                document.ShowTextAligned(new Paragraph(String.Format("Page" + i + " of " + n)),
                    300, 30, i, iText.Layout.Properties.TextAlignment.CENTER,
                    iText.Layout.Properties.VerticalAlignment.MIDDLE, 0).SetFontSize(10);

                if (!subscriber)
                {
                    // Thanks to Ivan
                    // https://stackoverflow.com/questions/44836260/itext-7-add-and-remove-watermark-on-a-pdf

                    iText.Kernel.Geom.Rectangle ps = pdfdocument.GetDefaultPageSize();

                    PdfPage page = pdfdocument.GetPage(i);
                    PdfLayer layer = new PdfLayer("watermark", pdfdocument);
                    var canvas = new PdfCanvas(page);
                    var pageSize = page.GetPageSize();
                    var paragraph = new Paragraph(SmartParametersV2016.KEASDONENERGYLTD).SetFontSize(60);
                    paragraph.SetFontColor(ColorConstants.BLACK, 0.2f);

                    iText.Layout.Canvas canvasModel;
                    canvas.BeginLayer(layer);
                    canvasModel = new iText.Layout.Canvas(canvas, ps);
                    canvasModel.ShowTextAligned(paragraph,
                                                pageSize.GetWidth() / 2,
                                                pageSize.GetHeight() / 2,
                                                pdfdocument.GetPageNumber(page),
                                                iText.Layout.Properties.TextAlignment.CENTER, iText.Layout.Properties.VerticalAlignment.MIDDLE, 45);
                    canvasModel.SetFontColor(ColorConstants.GREEN, 0.2f);
                    canvas.EndLayer();
                }
            }
            return;
        }
    }

    public interface ICloseApplication
    {
        void CloseApplication();
    }

#if ANDROIDX
    //    public class UtilityChartsAdapter : FragmentStateAdapter
    //    {
    //        public UtilityChartsAdapter(AndroidX.Fragment.App.FragmentManager fragmentManager, Lifecycle lifecycle) : base(fragmentManager, lifecycle)
    //        {
    //        }
    //        private AndroidX.Fragment.App.Fragment fragment = new AndroidX.Fragment.App.Fragment();

    //        public override int ItemCount => 3;

    //        public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
    //        {
    //            switch (position)
    //            {
    //                case 0:
    //                    fragment = new UtilityChartsCostsByTimeFragment();
    //                    break;
    //                case 1:
    //                    fragment = new UtilityChartsPastUsageByTimeFragment();
    //                    break;
    //                case 2:
    //                    fragment = new UtilityChartsReadingsByTimeFragment();
    //                    break;
    //            }
    //            return fragment;
    //        }
    //        //List<AndroidX.Fragment.App.Fragment> fragments = new List<AndroidX.Fragment.App.Fragment>();
    //        //List<string> fragmentTitles = new List<string>();

    //        //public UtilityChartsAdapter(AndroidX.Fragment.App.FragmentManager fm) : base(fm)
    //        //{ }
    //        //public void AddFragment(AndroidX.Fragment.App.Fragment fragment, String title)
    //        //{
    //        //    fragments.Add(fragment);
    //        //    fragmentTitles.Add(title);
    //        //}
    //        //public override AndroidX.Fragment.App.Fragment GetItem(int position)
    //        //{
    //        //    return fragments[position];
    //        //}
    //        //public override int Count
    //        //{
    //        //    get { return fragments.Count; }
    //        //}

    //        //public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
    //        //{
    //        //    return new Java.Lang.String(fragmentTitles[position]);
    //        //}
    //    }
#endif
}