#if WINFORMS
using SmartDashboard;
#endif

#if ANDROIDX
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
#endif


using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if SMARTMAUI
#endif

namespace SmartCubeMobile
{
    public class SmartProfilesV2023
    {

        //        // This is CUMULATIVE!
        internal static async Task<bool> Start_Profiles_Display(
#if WINFORMS
                                                        MainProcess components,
#endif
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        ProfilesViewModel profileviewmodel,
                                                        char cubeface_code,
                                                        string face_last_display)
        {

#if ANDROIDX

            if (meterActivity == null || profileviewmodel == null || ourviewmodel == null)
                return false;
            if (profileviewmodel.cubefacesRecyclerView == null ||
                profileviewmodel.groupsRecyclerView == null ||
                profileviewmodel.AddButton == null)
            {
                return false;
            }
            List<CubefaceRecord> cubefaces = new List<CubefaceRecord>();

            List<SmartProfile.Cubefaces> cubefacesList =
                    SmartSpikeV2017.Load_CubefacesX(ourviewmodel);

            long Idindex = 0;

            foreach (SmartProfile.Cubefaces cubeface in cubefacesList)
            {
                string cubefaceName =
                        SmartSpikeV2017.LookupScreenDescription(
                            ourviewmodel.CUBEFACESList,
                            cubeface.CUBEFACE_CODE);

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

            profileviewmodel.cubefacesAdapter =
                    new CubefacesAdapter(cubefaces);

            if (profileviewmodel.cubefacesRecyclerView.GetLayoutManager() == null)
            {
                profileviewmodel.cubefacesRecyclerView.SetLayoutManager(
                    new LinearLayoutManager(meterActivity));
            }
            
            profileviewmodel.cubefacesRecyclerView.SetAdapter(
                    profileviewmodel.cubefacesAdapter);

            var groups = new List<GroupRecord>();

            Idindex = 0;

            foreach (SmartProfile.Groups group in ourviewmodel.SmartProfile.profilegroupsList)
            {
                Idindex++;

                groups.Add(new GroupRecord
                {
                    Id = Idindex,
                    GROUPNAME = group.GROUPNAME,
                    ACTIVEFLAG = group.ACTIVEFLAG,
                    SENDF = group.SENDF,
                    SENDU = group.SENDU,
                    RECEIVEALL = group.RECEIVEALL,
                    DISPLAYNAME = group.DISPLAYNAME
                });
            }

            profileviewmodel.groupsAdapter = new GroupsAdapter(groups);

            if (profileviewmodel.groupsRecyclerView.GetLayoutManager() == null)
            {
                profileviewmodel.groupsRecyclerView.SetLayoutManager(
                    new LinearLayoutManager(meterActivity));
            }
            profileviewmodel.groupsRecyclerView.SetAdapter(
                    profileviewmodel.groupsAdapter);

            // Add button
            // Remove existing handler first
            if (profileviewmodel.AddButton_Click_Handler != null)
            {
                profileviewmodel.AddButton.Click -= profileviewmodel.AddButton_Click_Handler;
            }

            // Create handler
            profileviewmodel.AddButton_Click_Handler = (s, e) =>
            {
                var vm = profileviewmodel;

                vm.groupsRecyclerView.Post(() =>
                {
                    // Check if there is already a new empty record
                    if (vm.groupsAdapter.Items.Any(
                        r => r.IsNew && string.IsNullOrWhiteSpace(r.GROUPNAME)))
                    {
                        vm.groupsRecyclerView.ScrollToPosition(
                            vm.groupsAdapter.Items.IndexOf(
                                vm.groupsAdapter.Items.Last(
                                    r => r.IsNew && string.IsNullOrWhiteSpace(r.GROUPNAME))));

                        return;
                    }

                    // Add a new empty row
                    var newRow = new GroupRecord
                    {
                        Id = Guid.NewGuid().GetHashCode(),
                        IsNew = true,
                        ChangeType = ChangeTypeEnum.Added
                    };

                    vm.groupsAdapter.Items.Add(newRow);

                    vm.groupsAdapter.NotifyItemInserted(
                        vm.groupsAdapter.Items.Count - 1);

                    vm.groupsRecyclerView.ScrollToPosition(
                        vm.groupsAdapter.Items.Count - 1);
                });
            };

            // Attach handler
            profileviewmodel.AddButton.Click += profileviewmodel.AddButton_Click_Handler;

            // Delete
            profileviewmodel.groupsAdapter.DeleteRequested += rec =>
            {
                profileviewmodel.groupsRecyclerView.Post(() =>
                {
                    // Remove the item
                    profileviewmodel.groupsAdapter.RemoveItem(rec);

                    Toast.MakeText(
                        meterActivity,
                        "Group Record " + rec.GROUPNAME + " Deleted",
                        ToastLength.Short).Show();

                    // Clear focus if needed
                    var current = meterActivity.CurrentFocus;

                    if (current != null)
                    {
                        current.ClearFocus();
                    }
                });
            };

            // Committed changes (text edits)
            profileviewmodel.groupsAdapter.Committed += (oldRec, rec) =>
            {
                // Mark record as no longer "new" when edited (non-empty GROUPNAME)
                if (!string.IsNullOrWhiteSpace(rec.GROUPNAME) ||
                    !string.IsNullOrWhiteSpace(rec.DISPLAYNAME))
                {
                    switch (rec.ChangeType)
                    {
                        case ChangeTypeEnum.Added:

                            Toast.MakeText(
                                meterActivity,
                                "Group Record " + rec.GROUPNAME + " Added",
                                ToastLength.Short).Show();

                            break;

                        case ChangeTypeEnum.Modified:

                            Toast.MakeText(
                                meterActivity,
                                "Group Record " + rec.GROUPNAME + " Modified",
                                ToastLength.Short).Show();

                            break;
                    }
                }

                // After modification, check for empty new records and remove them
                RemoveEmptyNewRecord(meterActivity, profileviewmodel);
            };

            // Explicitly check for empty new records on modification
            profileviewmodel.cubefacesAdapter.CheckboxChanged += rec =>
            {
                // Whenever any record is modified, check for and remove the empty new record
                Toast.MakeText(
                    meterActivity,
                    "Cubeface Record " + rec.CUBEFACENAME + " Modified",
                    ToastLength.Short).Show();
            };

#endif


            if (ourviewmodel.SmartProfile.profilesList.Count > 0)
            {

#if WINFORMS || WPF  || WINUI || SMARTMAUI

                profileviewmodel.PUserName =
                        ourviewmodel.SmartProfile.profilesList.First().USERNAME;

                profileviewmodel.LastName =
                        ourviewmodel.SmartProfile.profilesList.First().LASTNAME;

                profileviewmodel.FirstName =
                        ourviewmodel.SmartProfile.profilesList.First().FIRSTNAME;

                profileviewmodel.MiddleName =
                        ourviewmodel.SmartProfile.profilesList.First().MIDDLENAME;

                profileviewmodel.DateOfBirth =
                        ourviewmodel.SmartProfile.profilesList.First().DATE_OF_BIRTH;

                profileviewmodel.Title =
                        ourviewmodel.SmartProfile.profilesList.First().TITLE;

                profileviewmodel.Gender =
                        ourviewmodel.SmartProfile.profilesList.First().GENDER;

                profileviewmodel.ContactNo =
                        ourviewmodel.SmartProfile.profilesList.First().CONTACT_NO;

                profileviewmodel.EmailAddress =
                        ourviewmodel.SmartProfile.profilesList.First().EMAIL_ADDRESS;

                profileviewmodel.DisplayName =
                        ourviewmodel.SmartProfile.profilesList.First().DISPLAYNAME;

#endif

#if WINFORMS

                components.CubefacesDataGrid.DataSource =
                        ourviewmodel.SmartProfile.profilecubefacesList;

                components.GroupsDataGrid.DataSource =
                        ourviewmodel.SmartProfile.profilegroupsList;

#endif
            }

#if WINFORMS || WPF  || WINUI || SMARTMAUI

            // I don't understand what I'm trying to do here..Doh!
            //FrontEndGUI.DecodeGroupsDataGrid(ourviewmodel);

#endif
            TurnOnProfileStatus(
#if WINFORMS
                components,
#endif
                profileviewmodel);

            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " display");

            return true;
        }

#if ANDROIDX
        void AddButton_Click(object sender, EventArgs e,
                            ProfilesViewModel profileviewmodel)
        {
            var vm = profileviewmodel;
            var recycler = vm.groupsRecyclerView;

            recycler.Post(() =>
            {
                if (vm.groupsAdapter.Items.Any(
                    r => r.IsNew && string.IsNullOrWhiteSpace(r.GROUPNAME)))
                {
                    recycler.ScrollToPosition(
                        vm.groupsAdapter.Items.IndexOf(
                            vm.groupsAdapter.Items.Last(
                                r => r.IsNew && string.IsNullOrWhiteSpace(r.GROUPNAME))));
                    return;
                }

                var newRow = new GroupRecord
                {
                    Id = Guid.NewGuid().GetHashCode(),
                    IsNew = true,
                    ChangeType = ChangeTypeEnum.Added
                };

                vm.groupsAdapter.Items.Add(newRow);

                vm.groupsAdapter.NotifyItemInserted(
                    vm.groupsAdapter.Items.Count - 1);

                recycler.ScrollToPosition(
                    vm.groupsAdapter.Items.Count - 1);
            });
        }
#endif

#if ANDROIDX

        // Method to remove empty new records from the list
        private static void RemoveEmptyNewRecord(
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                ProfilesViewModel profileviewmodel)
        {
            // Find any record marked as "new" and with an empty GROUPNAME (this ensures it's the "empty" record)
            var emptyNewRecord = profileviewmodel.groupsAdapter.Items
                .FirstOrDefault(r => r.IsNew && string.IsNullOrWhiteSpace(r.GROUPNAME));

            if (emptyNewRecord != null)
            {
                // Remove the empty new record from the list
                profileviewmodel.groupsAdapter.Items.Remove(emptyNewRecord);
                profileviewmodel.groupsAdapter.NotifyDataSetChanged(); // Ensure UI is updated
                Toast.MakeText(meterActivity, "Empty Group Record " + emptyNewRecord.Id + " removed", ToastLength.Short).Show();
            }
        }


#endif
        internal static void TurnOnProfileStatus(
#if WINFORMS
                                                    MainProcess components,
#endif
                                                    ProfilesViewModel profileviewmodel)
        {
            if (profileviewmodel != null)
            {
                // Enable the Cubes Grid
                profileviewmodel.CubefacesGridEnabled =
                // Enable the Groups Grid
                profileviewmodel.GroupsGridEnabled = true;
#if WINFORMS
                // Manually add handler for when an item check state has been modified.
#endif
            }
            return;
        }

        internal void TurnOffProfileStatus(
#if WINFORMS
                                                    MainProcess components,
#endif
                                                    ProfilesViewModel profileviewmodel)
        {
            if (profileviewmodel != null)
            {
                profileviewmodel.CubefacesGridEnabled =
                profileviewmodel.GroupsGridEnabled = false;
#if WINFORMS
                // Manually add handler for when an item check state has been modified.
#endif
            }
            return;
        }
    }
}