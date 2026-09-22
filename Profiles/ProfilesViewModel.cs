using System.ComponentModel;
using System.Globalization;

#if WINFORMS
//using System.Windows.Data;

using System.Windows.Markup;
using Windows.UI.Notifications.Management;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Windows;
#endif

#if WPF
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;
#endif

#if WINUI
using System.Threading;
using System;
using Microsoft.UI.Xaml.Data;
#endif

#if ANDROIDX
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
#endif

namespace SmartCubeMobile
{
#if WPF || SMARTMAUI
    public class ProfileCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = value as DateTime?;
            if (!date.HasValue) return value;

            // Use the passed-in culture (from binding or fallback to default)
            return date.Value.ToString("d", culture ?? CultureInfo.CurrentCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && DateTime.TryParse(str, culture, DateTimeStyles.None, out var result))
            {
                return result;
            }
#if WINFORMS || WPF
            return DependencyProperty.UnsetValue;
#endif
#if SMARTMAUI
            return Binding.DoNothing;
#endif
        }
    }

    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i) return i != 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return b ? 1 : 0;
            return 0;
        }
    }
#endif
#if WINUI
    public class ProfileCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var code = value as char?;
            if (!code.HasValue)
                return "";

            var vm = parameter as MainViewModel;
            if (vm == null)
                return "";

            return SmartSpikeV2017.LookupScreenDescription(vm.CUBEFACESList, code.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is char c ? c : '\0';
        }
    }

    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int i) return i != 0;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b) return b ? 1 : 0;
            return 0;
        }
    }
#endif

    public enum ChangeTypeEnum { None, Added, Modified, Deleted }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
    public class CubefaceRecord
    {
        public long Id { get; set; }
        public char CUBEFACECODE { get; set; }
        public string CUBEFACENAME { get; set; }
        public bool FACE_ACTIVE { get; set; }
        public bool Updated { get; set; }
    }
    public class GroupRecord : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public bool IsNew { get; set; } = true;
        public bool IsDeleted { get; set; }

        public bool HasBeenLogged { get; set; } = false;   // <<<<<< ADD THIS HERE

        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            private set { _isDirty = value; OnPropertyChanged(nameof(IsDirty)); }
        }

        public ChangeTypeEnum ChangeType { get; set; } = ChangeTypeEnum.None;

        private string _groupName = "";
        public string GROUPNAME { get => _groupName; set { _groupName = value; OnPropertyChanged(nameof(GROUPNAME)); RecalcDirty(); } }

        private bool _active;
        public bool ACTIVEFLAG { get => _active; set { _active = value; OnPropertyChanged(nameof(ACTIVEFLAG)); RecalcDirty(); } }

        private bool _sendF;
        public bool SENDF { get => _sendF; set { _sendF = value; OnPropertyChanged(nameof(SENDF)); RecalcDirty(); } }

        private bool _sendU;
        public bool SENDU { get => _sendU; set { _sendU = value; OnPropertyChanged(nameof(SENDU)); RecalcDirty(); } }

        private bool _receiveAll;
        public bool RECEIVEALL { get => _receiveAll; set { _receiveAll = value; OnPropertyChanged(nameof(RECEIVEALL)); RecalcDirty(); } }

        private string _displayName = "";
        public string DISPLAYNAME { get => _displayName; set { _displayName = value; OnPropertyChanged(nameof(DISPLAYNAME)); RecalcDirty(); } }

        private GroupRecord snapshot;

        public void CaptureSnapshot()
        {
            snapshot = (GroupRecord)this.MemberwiseClone();
            IsDirty = false;
            IsNew = false;
            ChangeType = ChangeTypeEnum.None;
        }

        public void RecalcDirty()
        {
            if (IsDeleted)
            {
                IsDirty = false;
                return;
            }

            if (IsNew)
            {
                IsDirty = true;
                return;
            }

            bool changed =
                (GROUPNAME ?? "") != (snapshot.GROUPNAME ?? "") ||
                ACTIVEFLAG != (snapshot?.ACTIVEFLAG ?? false) ||
                SENDF != (snapshot?.SENDF ?? false) ||
                SENDU != (snapshot?.SENDU ?? false) ||
                RECEIVEALL != (snapshot?.RECEIVEALL ?? false ||
                (DISPLAYNAME ?? "") != (snapshot.DISPLAYNAME ?? ""));

            IsDirty = changed;
        }
    }

#endif

#if ANDROIDX
    public class GroupRecord : Java.Lang.Object

    {
        public long Id { get; set; }
        public string GROUPNAME { get; set; }
        public string DISPLAYNAME { get; set; }
        public bool ACTIVEFLAG { get; set; }
        public bool SENDF { get; set; }
        public bool SENDU { get; set; }
        public bool RECEIVEALL { get; set; }
        public bool IsNew { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasBeenLogged { get; set; }
        public ChangeTypeEnum ChangeType { get; set; }

        public void CaptureSnapshot() => HasBeenLogged = true;
        public void RecalcDirty() => ChangeType = ChangeTypeEnum.Modified;

        public GroupRecord ShallowCopy()
        {
            return (GroupRecord)this.MemberwiseClone();
        }

    }

    public class CubefaceRecord : Java.Lang.Object
    {
        public long Id { get; set; }
        public char CUBEFACECODE { get; set; }
        public string CUBEFACENAME { get; set; }
        public bool FACE_ACTIVE { get; set; }
        
        public ChangeTypeEnum ChangeType { get; set; }

        
    }
#endif
    public class ProfilesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null && propName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        // Not sure these are ever used?
        internal CancellationTokenSource profileCts = new CancellationTokenSource();
        internal CancellationToken profileToken {get; set;}



#if ANDROIDX
        // Profile Cubefaces/Groups
        internal CheckBox CubeCheckBox { get; set; }

        internal Button SaveButton { get; set; }
        internal TextView PUserName { get; set; }
        internal EditText DisplayName { get; set; }
        internal EditText FirstName { get; set; }
        internal EditText LastName { get; set; }
        internal EditText MiddleName { get; set; }
        internal EditText DateOfBirth { get; set; }
        internal EditText Title { get; set; }
        internal EditText Gender { get; set; }
        internal EditText ContactNo { get; set; }
        internal EditText EmailAddress { get; set; }
        
        internal Button AddButton { get; set; }
        // Stores the click handler so it can be removed safely
        public EventHandler AddButton_Click_Handler { get; set; }

        internal TextView Description { get; set; }

        // RecyclerView instance that displays the UsersGroups:
        internal RecyclerView cubefacesRecyclerView {get; set;}
        // Layout manager that lays out each Cubeface in the RecyclerView:
        internal RecyclerView.LayoutManager cubefacesLayoutManager {get; set;}
        // Adapter that accesses the data set (Rays.profilecubefacesList):
        
        internal CubefacesAdapter cubefacesAdapter;

        // RecyclerView instance that displays the UsersGroups:
        internal RecyclerView groupsRecyclerView;
        // Layout manager that lays out each Group in the RecyclerView:
        internal RecyclerView.LayoutManager groupsLayoutManager {get; set;}
        // Adapter that accesses the data set (Hamase.groupsList):
        internal GroupsAdapter groupsAdapter;
#endif

#if !WINFORMS
        public string SaveProfile
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "SaveProfileTooltip");
            }
        }

        public string AddGroups
        {
            get
            {
                return SignIn.BesetByChimps(SignIn.signinviewmodel,
                                            "AddGroupsTooltip");
            }
        }
#endif

        private bool cubefacesgridenabled = false;
        public bool CubefacesGridEnabled
        {
            get
            {
                return cubefacesgridenabled;
            }
            set
            {
                cubefacesgridenabled = value;
                NotifyPropertyChanged(nameof(CubefacesGridEnabled));
            }
        }

        private bool groupsgridenabled = false;
        public bool GroupsGridEnabled
        {
            get
            {
                return groupsgridenabled;
            }
            set
            {
                groupsgridenabled = value;
                NotifyPropertyChanged(nameof(GroupsGridEnabled));
            }
        }

        internal string multiuser = "";
#if WPF
        private XmlLanguage profilelanguage;
        public XmlLanguage ProfileLanguage
        {
            get
            {
                return profilelanguage;
            }
            set
            {
                if (profilelanguage != value)
                {
                    profilelanguage = value;
                    this.NotifyPropertyChanged(nameof(ProfileLanguage));
                }
            }
        }
#endif
#if WINUI || SMARTMAUI
        private string profilelanguage;
        public string ProfileLanguage
        {
            get
            {
                return profilelanguage;
            }
            set
            {
                if (profilelanguage != value)
                {
                    profilelanguage = value;
                    this.NotifyPropertyChanged(nameof(ProfileLanguage));
                }
            }
        }
#endif
#if ANDROIDX
        private string profilelanguage;
        public string ProfileLanguage
        {
            get
            {
                return profilelanguage;
            }
            set
            {
                if (profilelanguage != value)
                {
                    profilelanguage = value;
                    this.NotifyPropertyChanged(nameof(ProfileLanguage));
                }
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
        private string displaynameDefault = "";
        public string DisplayName
        {
            get
            {
                return displaynameDefault;
            }
            set
            {
                if (displaynameDefault != value)
                {
                    displaynameDefault = value;
                    this.NotifyPropertyChanged(nameof(DisplayName));
                }
            }
        }

        private string pusernameDefault = "";
        public string PUserName
        {
            get
            {
                return pusernameDefault;
            }
            set
            {
                if (pusernameDefault != value)
                {
                    pusernameDefault = value;
                    this.NotifyPropertyChanged(nameof(PUserName));
                }
            }
        }

        private string plastnameDefault = "";
        public string LastName
        {
            get
            {
                return plastnameDefault;
            }
            set
            {
                if (plastnameDefault != value)
                {
                    plastnameDefault = value;
                    this.NotifyPropertyChanged(nameof(LastName));
                }
            }
        }

        private string fnameDefault = "";
        public string FirstName
        {
            get
            {
                return fnameDefault;
            }
            set
            {
                if (fnameDefault != value)
                {
                    fnameDefault = value;
                    this.NotifyPropertyChanged(nameof(FirstName));
                }
            }
        }

        private string mnameDefault = "";
        public string MiddleName
        {
            get
            {
                return mnameDefault;
            }
            set
            {
                if (mnameDefault != value)
                {
                    mnameDefault = value;
                    this.NotifyPropertyChanged(nameof(MiddleName));
                }
            }
        }

        private string titleDefault = "";
        public string Title
        {
            get
            {
                return titleDefault;
            }
            set
            {
                if (titleDefault != value)
                {
                    titleDefault = value;
                    this.NotifyPropertyChanged(nameof(Title));
                }
            }
        }

        private string dateofbirthDefault = "";
        public string DateOfBirth
        {
            get
            {
                return dateofbirthDefault;
            }
            set
            {
                if (dateofbirthDefault != value)
                {
                    dateofbirthDefault = value;
                    this.NotifyPropertyChanged(nameof(DateOfBirth));
                }
            }
        }

        private string genderDefault = "";
        public string Gender
        {
            get
            {
                return genderDefault;
            }
            set
            {
                if (genderDefault != value)
                {
                    genderDefault = value;
                    this.NotifyPropertyChanged(nameof(Gender));
                }
            }
        }

        private string contactnoDefault = "";
        public string ContactNo
        {
            get
            {
                return contactnoDefault;
            }
            set
            {
                if (contactnoDefault != value)
                {
                    contactnoDefault = value;
                    this.NotifyPropertyChanged(nameof(ContactNo));
                }
            }
        }

        private string emailaddressDefault = "";
        public string EmailAddress
        {
            get
            {
                return emailaddressDefault;
            }
            set
            {
                if (emailaddressDefault != value)
                {
                    emailaddressDefault = value;
                    this.NotifyPropertyChanged(nameof(EmailAddress));
                }
            }
        }
#endif
    }
}