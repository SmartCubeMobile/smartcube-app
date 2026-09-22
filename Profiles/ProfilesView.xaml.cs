#if WINFORMS
using SmartDashboard;
using System.Windows.Forms;
using System.Windows.Markup;
#endif

#if WPF
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Windows.Devices.Geolocation;
#endif

#if WINUI
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endif

#if SMARTMAUI
using System.Collections.ObjectModel;
using System.ComponentModel;
#endif

namespace SmartCubeMobile
{
#if !ANDROIDX
#if WINFORMS
    internal partial class ProfilesView : UserControl
#endif
#if WPF  || WINUI
    public partial class ProfilesView : UserControl
#endif
#if SMARTMAUI
    public partial class ProfilesView : ContentView
#endif
    {
        /// <summary>
        /// Interaction logic for Profiles.xaml
        /// </summary>
        internal static SmartProfile.Profiles oldprofile_info = new SmartProfile.Profiles();
        internal static bool local_found_it = false;

        // For the Groups
        public ObservableCollection<GroupRecord> Groups { get; set; } = new ObservableCollection<GroupRecord>();
        public ObservableCollection<GroupRecord> ChangeLog { get; set; } = new ObservableCollection<GroupRecord>();

#if WPF  || WINUI
        //public object GroupsGrid { get; private set; }

#endif

        private static SignInViewModel signinviewmodel;
        private static MainViewModel ourviewmodel;
        private static ProfilesViewModel profileviewmodel;
        // For the Finance and Utility - which may be null
        private static List<object> vmlist;

        public ProfilesView(SignInViewModel signinvm, MainViewModel mainvm, ProfilesViewModel profilevm, List<object>vms)
        { 
            signinviewmodel = signinvm;
            ourviewmodel = mainvm;
            profileviewmodel = profilevm;
            vmlist = vms;

#if WINFORMS || WPF  || WINUI || SMARTMAUI
            //internal ProfilesView()//MainViewModel ourviewmodel, ProfilesViewModel profileviewmodel)
            //{
#endif

#if WPF  || WINUI || SMARTMAUI
            InitializeComponent();
#endif
#if WPF 
            // Because you can't have the DataContext set on the UserControl iteself <= Chimps!
            DataContext = profileviewmodel;
#endif
#if SMARTMAUI
            BindingContext = profileviewmodel;
#endif
#if WINUI
            // Because you can't have the DataContext set on the UserControl iteself <= Chimps!
            this.DataContext = profileviewmodel;
#endif
#if WPF 
            SaveButton.Click += new RoutedEventHandler((s, e) => ProfileSave(s, e, ourviewmodel, profileviewmodel));
            AddButton.Click += new RoutedEventHandler((s, e) => AddRow_Click(s, e));
            CubefacesGrid.ItemsSource = LoadInitialCubefaceData(ourviewmodel);
            //this.PreviewKeyDown += new KeyEventHandler((s, e) => GroupsGrid_PreviewKeyDown(s, e, ourviewmodel));
#endif
#if SMARTMAUI
            SaveButton.Clicked += ((s, e) => ProfileSave(s, e, ourviewmodel, profileviewmodel));
            AddButton.Clicked += (s, e) => AddRow_Click(s, e);
            CubefacesGrid.ItemsSource = LoadInitialCubefaceData(ourviewmodel);
#if WINDOWS
            SaveButton.SetBinding(ToolTipProperties.TextProperty,"SaveProfile");
            AddButton.SetBinding(ToolTipProperties.TextProperty,"AddGroups");
#endif
#endif

#if WPF  || WINUI
            //this.GroupsGrid.ItemsSource = ourviewmodel.SmartProfile.profilegroupsList;
            GroupsGrid.ItemsSource = Groups;
            //ChangeLogGrid.ItemsSource = ChangeLog;
#endif
#if SMARTMAUI
            GroupsGrid.ItemsSource = Groups;
#endif

            LoadInitialGroupData(ourviewmodel);
            return;
        }

#if WINFORMS
        internal async void ProfileSave(object sender, EventArgs e,
                                        this, MainViewModel ourviewmodel,
                                        ProfilesViewModel profileviewmodel)
#endif
#if WPF  || WINUI
        internal async void ProfileSave(object sender, RoutedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        ProfilesViewModel profileviewmodel)
#endif
#if SMARTMAUI
        internal async void ProfileSave(object sender, EventArgs e,
                                        MainViewModel ourviewmodel,
                                        ProfilesViewModel profileviewmodel)
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
        {
            //string errorMessage = "";

            List<SmartProfile.Profiles> profiles_found = SmartSpikeV2017.Lookup_Profile(ourviewmodel);
            if (profiles_found.Count == 1)
            {
                oldprofile_info = profiles_found[0];
                local_found_it = true;
            }

            SmartProfile.Profiles newprofile_info = new SmartProfile.Profiles()
            {
                USERNAME = ourviewmodel.UserName,
                FIRSTNAME = profileviewmodel.FirstName,
                LASTNAME = profileviewmodel.LastName,
                MIDDLENAME = profileviewmodel.MiddleName,
                DATE_OF_BIRTH = profileviewmodel.DateOfBirth,
                RANDOMKEY1 = 0,
                TITLE = profileviewmodel.Title,
                GENDER = profileviewmodel.Gender,
                EMAIL_ADDRESS = profileviewmodel.EmailAddress,
                CONTACT_NO = profileviewmodel.ContactNo,
                DISPLAYNAME = profileviewmodel.DisplayName,
                RANDOMKEY2 = 0
            };
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (!await Users_ProfileSetup(ourviewmodel,
                                                    newprofile_info,
                                                    oldprofile_info,
                                                    local_found_it))
            {
                //LoginStatusMessage.Text = errorMessage;
            }
            else
            {
#if WPF
                //this.IsOpen = false;
#endif
            }
            return;
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        internal static async Task<bool> Users_ProfileSetup(MainViewModel ourviewmodel,
                                                SmartProfile.Profiles newprofile_info,
                                                SmartProfile.Profiles oldprofile_info,
                                                bool local_found_it)
        {
            try
            {
                if ((newprofile_info.FIRSTNAME.Trim() == oldprofile_info.FIRSTNAME) &&
                        (newprofile_info.MIDDLENAME.Trim() == oldprofile_info.MIDDLENAME) &&
                        (newprofile_info.LASTNAME.Trim() == oldprofile_info.LASTNAME) &&
                        (newprofile_info.TITLE.Trim() == oldprofile_info.TITLE) &&
                        (newprofile_info.GENDER.Trim() == oldprofile_info.GENDER) &&
                        (newprofile_info.DATE_OF_BIRTH.Trim() == oldprofile_info.DATE_OF_BIRTH) &&
                        (newprofile_info.EMAIL_ADDRESS.Trim() == oldprofile_info.EMAIL_ADDRESS) &&
                        (newprofile_info.CONTACT_NO.Trim() == oldprofile_info.CONTACT_NO) &&
                        (newprofile_info.DISPLAYNAME.Trim() == oldprofile_info.DISPLAYNAME))
                {
                    // They are both the same - do nothing - for now
                }
                else
                {
                    int randomkey1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                    int randomkey2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
                    SmartProfile.Profiles profile_row = new SmartProfile.Profiles()
                    {
                        USERNAME = oldprofile_info.USERNAME,
                        // Profile Created done in a mo ...
                        FIRSTNAME = newprofile_info.FIRSTNAME,
                        MIDDLENAME = newprofile_info.MIDDLENAME,
                        LASTNAME = newprofile_info.LASTNAME,
                        DATE_OF_BIRTH = newprofile_info.DATE_OF_BIRTH,
                        RANDOMKEY1 = randomkey1,
                        TITLE = newprofile_info.TITLE,
                        GENDER = newprofile_info.GENDER,
                        CONTACT_NO = newprofile_info.CONTACT_NO,
                        EMAIL_ADDRESS = newprofile_info.EMAIL_ADDRESS,
                        DISPLAYNAME = newprofile_info.DISPLAYNAME,
                        RANDOMKEY2 = randomkey2
                    };
                    // Profile Created done here (a mo later ...)
                    if (!local_found_it)
                    {
                        profile_row.PROFILE_CREATED = DateTime.UtcNow;   // UTC time
                        profile_row.Updated = false;
                    }
                    else
                    {
                        profile_row.PROFILE_CREATED = oldprofile_info.PROFILE_CREATED;
                        profile_row.Updated = true;
                    }
                    // Insert or Update? Its all in Finance_Logins_ChangesList
                    ourviewmodel.SmartProfile.profilesChangesList.Add(profile_row);

                    // If Updated = true, then the record already exists
                    // If Updated = false then we need to create a new record ..

                    // Record the change
                    if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                    SmartParametersV2016.sqliteformat))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "User profile update"))
                        {
                            return false;
                        }
                        return false;
                    }
                    else
                    {
                        // Here we successfully updated the remote SERVER db
                        // Now we have to UPDATE the SQLite record already in Profiles
                        // or INSERT in the new SQLite record into Profiles
                        // We never DELETE records btw
                        // But we have to do that with the ENCRYPTED record
                        // because WPF.db3 holds ENCRYPTED records

                        //    // Tell the console we have switched
                        if (!await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "User profile: " + "Updated"))
                        {
                            return false;
                        }
                    }
                    // Tell the console we have switched
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Profile info changed for: " + oldprofile_info.USERNAME);
                }
            }
            catch (Exception ex)
            {
                ourviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }
#endif
#if WPF
        private async void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                // Go right
                await SmartRoutinesV2018.ButtonRightClickedActual(signinviewmodel, ourviewmodel);
            }
            else
            {
                if (e.Key == Key.Left)
                {
                    // Go Left
                    await SmartRoutinesV2018.ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
                }
            }
            return;
        }
#endif
#if WINUI
        // https://stackoverflow.com/questions/46962632/use-swipe-gesture-in-WINUI
        // https://stackoverflow.com/questions/45550684/horizontal-swipe-gesture-on-WINUI
        // Thanks to Justin XL 

        private void SwipeableTextBlock_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.IsInertial)
            {
                var swipedDistance = e.Cumulative.Translation.X;

                if (Math.Abs(swipedDistance) <= 2) return;

                if (swipedDistance > 0)
                {
                    SmartRoutinesV2018.OnSwipedRightActual(sender, e, signinviewmodel, ourviewmodel);
                }
                else
                {
                    SmartRoutinesV2018.OnSwipedLeftActual(sender, e, signinviewmodel, ourviewmodel);
                }
            }
            return;
        }

        private void SwipeableTextBlock_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (sender != null && e != null)
            {
                // Left in for future compatibility
            }
            return;
        }
#endif
#if SMARTMAUI
        private async void OnRightKey(object sender, EventArgs e)
        {
            await SmartRoutinesV2018.ButtonRightClickedActual(signinviewmodel, ourviewmodel);
        }

        private async void OnLeftKey(object sender, EventArgs e)
        {
            await SmartRoutinesV2018.ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
        }
#endif

        // Read the notes in the journal about HOW this fucking
        // CheckBox will fire when it's checked EVEN WHEN its
        // not fuckng enabled and how I DON'T want it to fire
        // when it comes in checked ... which it fucking does
        // Chimps!  The Fucking Chimps who wrote this WPF SHIT
#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS || WPF  || WINUI
        private async void CubefaceCheckedUnchecked(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        private async void CubefaceCheckedUnchecked(object sender, TappedEventArgs e)

#endif
        {
            if (sender != null && e != null)
            {
                if (profileviewmodel.CubefacesGridEnabled)
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.whiteColour);
                    await CubefaceCheckedUnchecked_Actual(sender, e);
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private async Task<bool> CubefaceCheckedUnchecked_Actual(

                                                    object sender,
#if WINFORMS || WPF  || WINUI
                                                    RoutedEventArgs e
#endif
#if SMARTMAUI
                                                    EventArgs e
#endif

                                                    )
        {
#if WINFORMS || WPF
            DataGridCellInfo cell = this.CubefacesGrid.CurrentCell;
            int rowIndex = this.CubefacesGrid.Items.IndexOf(cell.Item);
            if (rowIndex < 0)
            {
                return false;
            }
            CubefaceRecord cubeface_row = this.CubefacesGrid.Items[rowIndex] as CubefaceRecord;
#endif
#if SMARTMAUI
            CubefaceRecord cubeface_row = CubefacesGrid.SelectedItem as CubefaceRecord;
            if (cubeface_row == null)
            {
                return false;
            }
#endif
#if WINUI
            if (this.CubefacesGrid.SelectedIndex == -1)
            {
                return false;
            }
            CubefaceRecord cubeface_row = this.CubefacesGrid.SelectedItem as CubefaceRecord;
#endif
#if WINFORMS || WPF || WINUI || SMARTMAUI
            cubeface_row.Updated = true;
            if (!await DoTheCubefacesCheckBoxes(cubeface_row))
            {
                return false;
            }
#if WPF
            e.Handled = true;
#endif
#endif
            return true;
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        internal static async Task<bool> DoTheCubefacesCheckBoxes(CubefaceRecord cubeface_record)
        {
            SmartProfile.Cubefaces change = new SmartProfile.Cubefaces();
            change.USERNAME = ourviewmodel.UserName;
            change.CUBEFACE_CODE = cubeface_record.CUBEFACECODE;
            change.FACE_ACTIVE = cubeface_record.FACE_ACTIVE;
            change.Updated = true;
            ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(change);

            if (ourviewmodel.SmartProfile.profilecubefacesChangesList.Count > 0)
            {
                if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                SmartParametersV2016.sqliteformat))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "User profile update"))
                    {
                        return false;
                    }
                    return false;
                }
                else
                {
                    // Here we successfully updated the remote SERVER db
                    // Now we have to UPDATE the SQLite record already in Profiles
                    // or INSERT in the new SQLite record into Profiles
                    // We never DELETE records btw Oh yes we do!
                    // But we have to do that with the ENCRYPTED record
                    // because WPF.db3 holds ENCRYPTED records

                    // Tell the console we have updated
                    if (!await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "User profile: " + "Updated"))
                    {
                        return false;
                    }
                    if (ourviewmodel.SmartProfile.profilecubefacesList.Count > 0)
                    {
                        foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList)
                        {
                            if (cubeface_row.CUBEFACE_CODE == cubeface_record.CUBEFACECODE)
                            {
                                short screen_code = SmartSpikeV2017.LookupScreenCode(ourviewmodel.CUBEFACESList,
                                                                                    cubeface_row.CUBEFACE_CODE);

                                switch (cubeface_row.CUBEFACE_CODE)
                                {
                                    case SmartParametersV2016.Finance:
                                        if (cubeface_row.FACE_ACTIVE)
                                        {
                                            // Here we make the Forms Design GUI tabs visible?
                                            FinanceViewModel financeviewmodel = vmlist.OfType<FinanceViewModel>().FirstOrDefault();

                                            if (financeviewmodel != null)
                                            {
                                                if (!await SmartRoutinesV2018.LoadFinanceModule(
#if WINFORMS
                                                    MainProcess components,
#endif
                                                    signinviewmodel,
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    cubeface_row.CUBEFACE_CODE,
                                                    cubeface_row.FACE_CULTURE_CODE,
                                                    cubeface_row.FACE_CURRENCY,
                                                    cubeface_row.FACE_LAST_DISPLAY,
                                                    cubeface_row.NEXT_CONNECTION))
                                                {
                                                    // SOMETHING has gone amiss ...
                                                    if (!(ourviewmodel.quitCts.Token.IsCancellationRequested ||
                                                            financeviewmodel.financeToken.IsCancellationRequested))
                                                    {
                                                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                                        ourviewmodel.errorMessage = "Problem: " + "Start Finance Display failed";
                                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, ourviewmodel.errorMessage))
                                                        {
                                                            return false;
                                                        }
                                                    }
                                                    return false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // Here we make the Forms Design GUI tabs visible?
                                            FinanceViewModel financeviewmodel = vmlist.OfType<FinanceViewModel>().FirstOrDefault();
                                            if (financeviewmodel != null)
                                            {
                                                if (!await SmartRoutinesV2018.UnLoadFinanceModule(
#if WINFORMS
                                                    MainProcess components,
#endif
                                                    vmlist,
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    cubeface_row.CUBEFACE_CODE))
                                                {
                                                    // SOMETHING has gone amiss ...
                                                    if (!(ourviewmodel.quitCts.Token.IsCancellationRequested ||
                                                            financeviewmodel.financeToken.IsCancellationRequested))
                                                    {
                                                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                                        ourviewmodel.errorMessage = "Problem: " + "Start Finance Display failed";
                                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, ourviewmodel.errorMessage))
                                                        {
                                                            return false;
                                                        }
                                                    }
                                                    return false;
                                                }
                                            }
                                        }
                                        break;
                                    case SmartParametersV2016.Utility:
                                        if (cubeface_row.FACE_ACTIVE)
                                        {
                                            UtilityViewModel utilityviewmodel = vmlist.OfType<UtilityViewModel>().FirstOrDefault();
                                            // Here we make the Forms Design GUI tabs visible?
                                            if (utilityviewmodel != null)
                                            {
                                                if (!await SmartRoutinesV2018.LoadUtilityModule(
#if WINFORMS
                                                MainProcess components,
#endif
                                                    signinviewmodel,
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    cubeface_row.CUBEFACE_CODE,
                                                    cubeface_row.FACE_CULTURE_CODE,
                                                    cubeface_row.FACE_CURRENCY,
                                                    cubeface_row.FACE_LAST_DISPLAY,
                                                    cubeface_row.NEXT_CONNECTION))
                                                {
                                                    // SOMETHING has gone amiss ...
                                                    if (!(ourviewmodel.quitCts.Token.IsCancellationRequested ||
                                                            utilityviewmodel.utilityToken.IsCancellationRequested))
                                                    {
                                                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                                        ourviewmodel.errorMessage = "Problem: " + "Start Utility Display failed";
                                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, ourviewmodel.errorMessage))
                                                        {
                                                            return false;
                                                        }
                                                    }
                                                    return false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            UtilityViewModel utilityviewmodel = vmlist.OfType<UtilityViewModel>().FirstOrDefault();
                                            // Here we make the Forms Design GUI tabs visible?
                                            if (utilityviewmodel != null)
                                            {
                                                if (!await SmartRoutinesV2018.UnLoadUtilityModule(
#if WINFORMS
                                                MainProcess components,
#endif
                                                    vmlist,
                                                    ourviewmodel,
                                                    utilityviewmodel,
                                                    cubeface_row.CUBEFACE_CODE))
                                                {
                                                    // SOMETHING has gone amiss ...
                                                    if (!(ourviewmodel.quitCts.Token.IsCancellationRequested ||
                                                            utilityviewmodel.utilityToken.IsCancellationRequested))
                                                    {
                                                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                                        ourviewmodel.errorMessage = "Problem: " + "Start Utility Display failed";
                                                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, ourviewmodel.errorMessage))
                                                        {
                                                            return false;
                                                        }
                                                    }
                                                    return false;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        //
        // Cubefaces ==============================================
        //  
        private List<CubefaceRecord> LoadInitialCubefaceData(MainViewModel ourviewmodel)
        {
            List<CubefaceRecord> cubefaces = new List<CubefaceRecord>();
            List<SmartProfile.Cubefaces> cubefacesList = SmartSpikeV2017.Load_CubefacesX(ourviewmodel);
            long Idindex = 0;
            foreach (SmartProfile.Cubefaces cubeface in cubefacesList)
            {
                string cubefaceName = SmartSpikeV2017.LookupScreenDescription(ourviewmodel.CUBEFACESList, cubeface.CUBEFACE_CODE);
                if (!string.IsNullOrEmpty(cubefaceName))
                {
                    Idindex++;
                    cubefaces.Add(new CubefaceRecord
                    {
                        Id = Idindex,
                        CUBEFACECODE = cubeface.CUBEFACE_CODE,
                        CUBEFACENAME = cubefaceName,
                        FACE_ACTIVE = cubeface.FACE_ACTIVE
                    });
                }
            }
            return cubefaces;
        }
        //
        // Groups ==============================================
        //    
        private void LoadInitialGroupData(MainViewModel ourviewmodel)
        {
            foreach (SmartProfile.Groups groupie in ourviewmodel.SmartProfile.profilegroupsList)
            {
                AddRecord(groupie.GROUPNAME,
                        groupie.ACTIVEFLAG,
                        groupie.SENDF,
                        groupie.SENDU,
                        groupie.RECEIVEALL,
                        groupie.DISPLAYNAME,
                        false);          // IsNew!!!!!!!!!!!      
            }
            return;
        }

        private void AddRecord(string group, bool active, bool sendF, bool sendU, bool receive, string display, bool isNew)
        {
            GroupRecord rec = new GroupRecord
            {
                GROUPNAME = group,
                ACTIVEFLAG = active,
                SENDF = sendF,
                SENDU = sendU,
                RECEIVEALL = receive,
                DISPLAYNAME = display,
                IsNew = isNew
            };
            rec.PropertyChanged += Group_PropertyChanged;
            if (!isNew)
            {
                rec.CaptureSnapshot();
            }
            Groups.Add(rec);
        }

#if WPF
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            GroupRecord rec = new GroupRecord();
            rec.PropertyChanged += Group_PropertyChanged;
            Groups.Add(rec);

            GroupsGrid.SelectedItem = rec;
            GroupsGrid.ScrollIntoView(rec);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                GroupsGrid.CurrentCell = new DataGridCellInfo(rec, GroupsGrid.Columns[0]);
                GroupsGrid.BeginEdit();
            }));
        }
#endif
#if SMARTMAUI
        private void AddRow_Click(object sender, EventArgs e)
        {
            var rec = new GroupRecord();
            rec.PropertyChanged += Group_PropertyChanged;

            Groups.Add(rec);

            GroupsGrid.SelectedItem = rec;

            // Scroll into view (MAUI-safe way)
            GroupsGrid.ScrollTo(rec, position: ScrollToPosition.MakeVisible);

            // No BeginInvoke needed in MAUI in most cases
        }
#endif

#if WINUI
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            GroupRecord rec = new GroupRecord();
            rec.PropertyChanged += Group_PropertyChanged;
            Groups.Add(rec);

            GroupsGrid.SelectedItem = rec;
#if WINUI
            GroupsGrid.DispatcherQueue.TryEnqueue(() =>
            {
                GroupsGrid.UpdateLayout();
                GroupsGrid.ScrollIntoView(rec, null);
            });
#endif
        }
#endif
#if WINFORMS || WPF
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
#endif
#if WINUI
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        private async void DeleteButton_Click(object sender, EventArgs e)
#endif
        {
            if (sender is RadioButton btn &&
#if WINFORMS || WPF  || WINUI
                btn.DataContext is GroupRecord rec)
#endif
#if SMARTMAUI
                btn.BindingContext is GroupRecord rec)
#endif
            {
#if WINUI
                var dialog = new ContentDialog
                {
                    Title = "Confirm Delete",
                    Content = $"Delete group '{rec.GROUPNAME}'?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    XamlRoot = this.XamlRoot   // 🔥 REQUIRED
                };

                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    rec.IsDeleted = true;
                    AppendToChangeLog(ourviewmodel, rec);
                    Groups.Remove(rec);
                }
#endif
#if WINFORMS || WPF
                if (MessageBox.Show($"Delete group '{rec.GROUPNAME}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    rec.IsDeleted = true;
                    AppendToChangeLog(ourviewmodel, rec);
                    Groups.Remove(rec);
                }
#endif
#if SMARTMAUI
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                                "Confirm Delete",
                                $"Delete group '{rec.GROUPNAME}'?",
                                "Yes",
                                "No");
                if (confirm)
                {
                    rec.IsDeleted = true;
                    AppendToChangeLog(ourviewmodel, rec);
                    Groups.Remove(rec);
                }
#endif
            }
        }

#if WPF
        private void GroupsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
#endif
#if WINUI
        private async void GroupsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
#endif
#if SMARTMAUI
        private async void GroupsGrid_CellEditEnding(object sender, PropertyChangedEventArgs e)
#endif
        {
#if WPF  || WINUI
            if (e.EditAction != DataGridEditAction.Commit)
            {
                return;
            }
#endif
            // Force binding update
#if SMARTMAUI
            // This replaces your "Group column" logic
            if (e.PropertyName == nameof(GroupRecord.GROUPNAME))
#endif
#if WPF  || WINUI
            if (e.EditingElement is TextBox tb)
#endif
            {
#if WPF
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
#endif
#if SMARTMAUI
                // GroupRecord.GROUPNAME.UpdateSource(); This stays commented out!
#endif
#if WINUI
                tb.Text = tb.Text; // forces update via binding (hacky but works)
#endif
            }

#if WPF
            if (e.Row.Item is GroupRecord rec)
#endif
#if WINUI
            if (e.Row.DataContext is GroupRecord rec)
#endif
#if SMARTMAUI
            if (sender is GroupRecord rec)
#endif

            {
                // VALIDATE ONLY WHEN GROUPNAME IS EDITED
#if WPF  || WINUI
                if ((e.Column.Header as string) == "Group")
#endif
#if SMARTMAUI
                if (e.PropertyName == nameof(GroupRecord.GROUPNAME))
#endif
                {
                    string name = rec.GROUPNAME?.Trim() ?? "";

                    // 1️⃣ If blank → remove the new row
                    if (string.IsNullOrWhiteSpace(name))
                    {
#if WPF
                        MessageBox.Show("Group Name cannot be blank.", "Invalid Name");
#endif
#if WINUI
                        ContentDialog dialog = new ContentDialog
                        {
                            Title = "Invalid Name",
                            Content = "Group Name cannot be blank.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await dialog.ShowAsync();
#endif
#if SMARTMAUI
                        await Application.Current.MainPage.DisplayAlert(
                                                        "Warning",
                                                        "Group Name cannot be blank",
                                                        "OK");
#endif

#if WPF  || SMARTMAUI
                        Groups.Remove(rec);
#endif
#if WINUI
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Groups.Remove(rec);
                        });
#endif
                        return;
                    }

                    // 2️⃣ Check duplicates (ignore this row)
                    bool isDuplicate = Groups
                        .Where(r => r != rec)
                        .Any(r => string.Equals(r.GROUPNAME, name, StringComparison.OrdinalIgnoreCase));

                    if (isDuplicate)
                    {
#if WPF
                        MessageBox.Show($"Group '{name}' already exists", "Duplicate Name");
#endif
#if WINUI
                        ContentDialog dialog = new ContentDialog
                        {
                            Title = "Duplicate Name",
                            Content = $"Group '{name}' already exists",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await dialog.ShowAsync();
#endif
#if SMARTMAUI
                        await Application.Current.MainPage.DisplayAlert(
                                                        "Duplicate Name",
                                                        $"Group '{name}' already exists",
                                                        "OK");
#endif
#if WPF  || SMARTMAUI
                        Groups.Remove(rec);
#endif
#if WINUI
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            Groups.Remove(rec);
                        });
#endif
                        return;
                    }
                }

                // Normal dirty tracking + logging
                rec.RecalcDirty();
                AppendToChangeLog(ourviewmodel, rec);
            }
            return;
        }

#if WINFORMS || WPF  || WINUI
        private void CheckBox_Click(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        private void CheckBox_Click(object sender, EventArgs e)
#endif
        {
            if (sender is CheckBox cb &&
#if WINFORMS || WPF  || WINUI
                cb.DataContext is GroupRecord rec)
#endif
#if SMARTMAUI
                cb.BindingContext is GroupRecord rec)
#endif
            {
                rec.RecalcDirty();
                AppendToChangeLog(ourviewmodel, rec);
            }
        }

        private void Group_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // No longer used for logging; logging is done after committed edits
        }

        private async void AppendToChangeLog(MainViewModel ourviewmodel,
                                            GroupRecord rec)
        {
            // Determine ChangeType for logging
            ChangeTypeEnum changeType;

            if (rec.IsDeleted)
            {
                changeType = ChangeTypeEnum.Deleted;
            }
            else if (rec.IsNew && !rec.HasBeenLogged)
            {
                changeType = ChangeTypeEnum.Added;
            }
            else
            {
                changeType = ChangeTypeEnum.Modified;
            }

            GroupRecord snapshot = new GroupRecord
            {
                GROUPNAME = rec.GROUPNAME,
                ACTIVEFLAG = rec.ACTIVEFLAG,
                SENDF = rec.SENDF,
                SENDU = rec.SENDU,
                RECEIVEALL = rec.RECEIVEALL,
                DISPLAYNAME = rec.DISPLAYNAME,
                IsNew = rec.IsNew,
                IsDeleted = rec.IsDeleted,
                ChangeType = changeType
            };

            ChangeLog.Add(snapshot);

            // Mark that the new row has been logged
            if (rec.IsNew)
            {
                rec.HasBeenLogged = true;
            }

            SmartProfile.Groups group_record = new SmartProfile.Groups();
            group_record.USERNAME = ourviewmodel.UserName;
            group_record.GROUPNAME = snapshot.GROUPNAME;
            group_record.ACTIVEFLAG = snapshot.ACTIVEFLAG;
            group_record.MARKER = SmartParametersV2016.defaultDate;
            group_record.PDEK = "";
            group_record.SENDF = snapshot.SENDF;
            group_record.FDEK = "";
            group_record.SENDU = snapshot.SENDU;
            group_record.UDEK = "";
            group_record.RECEIVEALL = snapshot.RECEIVEALL;
            group_record.DISPLAYNAME = snapshot.DISPLAYNAME;

            bool deleteFlag = false;
            switch (changeType)
            {
                case ChangeTypeEnum.Deleted:
                    group_record.Updated = true;  // Its a Delete with Delete flag set
                    deleteFlag = true;
                    break;
                case ChangeTypeEnum.Added:
                    group_record.Updated = false;  // Its an Add
                    break;
                case ChangeTypeEnum.Modified:
                    group_record.Updated = true;  // Its a modify
                    break;
                default:
                    break;

            }
            ourviewmodel.SmartProfile.profilegroupsChangesList.Add(group_record);
            if (ourviewmodel.SmartProfile.profilegroupsChangesList.Count > 0)
            {
                if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                SmartParametersV2016.sqliteformat,
                                                                deleteFlag))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "User profile update"))
                    {
                        return;
                    }
                    return;
                }
                else
                {
                    // Tell the console we have updated
                    if (!await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "User profile: " + "Updated"))
                    {
                        return;
                    }
                }
            }
        }
#endif
    }


#if WPF
    // All this shit just to get default 'watermarks' in the UserName and PassWord boxes ..
    public class PasscodeBoxMonitor : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject dependency)
        {
            if (dependency != null)
            {
                return (bool)dependency.GetValue(IsMonitoringProperty);
            }
            // Need to test this!!!!!
            return false;
        }

        public static void SetIsMonitoring(DependencyObject dependency, bool value)
        {
            //if (dependency != null)
            //{
            dependency?.SetValue(IsMonitoringProperty, value);
            //}
            return;
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasscodeBoxMonitor), new PropertyMetadata(false, OnIsMonitoringChanged));

        public static int GetPasscodeNumberLength(DependencyObject dependency)
        {
            if (dependency != null)
            {
                return (int)dependency.GetValue(PasscodeLengthProperty);
            }
            // Need to test this!!!!
            return 0;
        }

        public static void SetPasscodeLength(DependencyObject dependency, int value)
        {
            //if (dependency != null)
            //{
            dependency?.SetValue(PasscodeLengthProperty, value);
            //}
            return;
        }

        public static readonly DependencyProperty PasscodeLengthProperty =
            DependencyProperty.RegisterAttached("PasscodeLength", typeof(int), typeof(PasscodeBoxMonitor), new PropertyMetadata(0));

        private static void OnIsMonitoringChanged(DependencyObject depobj, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox Passcode = (PasswordBox)depobj;
            if (Passcode == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                Passcode.PasswordChanged += PasscodeChanged;
            }
            else
            {
                Passcode.PasswordChanged -= PasscodeChanged;
            }
            return;
        }

        internal static void PasscodeChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox Passcode = (PasswordBox)sender;
            if (Passcode == null)
            {
                return;
            }
            SetPasscodeLength(Passcode, Passcode.Password.Length);
            return;
        }
    }
#endif
#endif
#endif
}