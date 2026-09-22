#if WINFORMS
using SmartDashboard;
#endif
#if WPF
using System.Collections.Generic;
using System.Windows.Threading;
#endif
#if ANDROIDX
using Android.Content;
using AndroidX.AppCompat.App;
#endif



using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if SMARTMAUI
using Microsoft.Maui.ApplicationModel;

#endif

namespace SmartCubeMobile
{
    public class SmartKeepAliveTask : PeriodicTickTask
    {
#if WINFORMS
        private MainProcess mainComponents;
#endif
        private readonly SignInViewModel signinviewmodel;
        private readonly MainViewModel ourviewmodel;
        private readonly FinanceViewModel financeviewmodel;
        private readonly UtilityViewModel utilityviewmodel;
#if WPF
        private readonly Dispatcher _dispatcher;
#endif
#if ANDROIDX
        private readonly AppCompatActivity meterActivity;
#endif
#if SMARTMAUI
        private readonly IDispatcher _dispatcher;
#endif
        public SmartKeepAliveTask(
#if WINFORMS
                                    MainProcess processComponents,
#endif
#if WPF
                                    Dispatcher dispatcher,
#endif
#if ANDROIDX
                                    AppCompatActivity activity,
#endif
#if SMARTMAUI
                                    IDispatcher dispatcher,
#endif
                                    SignInViewModel signinvm,
                                    MainViewModel mainvm, 
                                    List<object> vmlist)
            : base(TimeSpan.FromSeconds(SmartParametersV2016.keepaliveLimit))
        {
            signinviewmodel = signinvm;
            ourviewmodel = mainvm;
#if WINFORMS
            mainComponents = processComponents;
#endif
#if WPF || SMARTMAUI
            _dispatcher = dispatcher;
#endif
#if ANDROIDX
#endif

            FinanceViewModel financevm = vmlist.OfType<FinanceViewModel>().FirstOrDefault();
            if (financevm != null)
            {
                financeviewmodel = financevm;
            }
            UtilityViewModel utilityvm = vmlist.OfType<UtilityViewModel>().FirstOrDefault();
            if (utilityvm != null)
            {
                utilityviewmodel = utilityvm;
            }
#if ANDROIDX
            meterActivity = activity;
#endif
        }

        //        protected override async Task ExecuteAsync()
        //        {
        //#if WPF
        //            await _dispatcher.InvokeAsync(async () =>
        //#endif
        //#if ANDROIDX

        //            // Run UI updates on main thread
        //            meterActivity.RunOnUiThread(async () =>
        //#endif
        //            {
        //                if (!SmartNibbyV2016.NetworkAvailability())
        //                {
        //                    FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.redColour);
        //                    // Try to carry on
        //                    return; // Pointless carrying on .. wait one minute
        //                }
        //                FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour);

        //                string lastDate = "";   // For Crypto stuff



        //                // We need to know if ANNA or MARIA has changed (when they've been added)
        //                // We will do this by checking the MARKER, but first we need to store
        //                // the current MARKER(s)
        //                List<SmartProfile.Groups> mmCopy = new List<SmartProfile.Groups>();
        //                foreach (SmartProfile.Groups groupRow in ourviewmodel.mmListX) // Checked
        //                {
        //                    SmartProfile.Groups mmRow = new SmartProfile.Groups()
        //                    {
        //                        USERNAME = groupRow.USERNAME,
        //                        GROUPNAME = groupRow.GROUPNAME,
        //                        ACTIVEFLAG = groupRow.ACTIVEFLAG,
        //                        MARKER = groupRow.MARKER,
        //                        PDEK = groupRow.PDEK,
        //                        SENDF = groupRow.SENDF,
        //                        FDEK = groupRow.FDEK,
        //                        SENDU = groupRow.SENDU,
        //                        UDEK = groupRow.UDEK,
        //                        RECEIVEALL = groupRow.RECEIVEALL,
        //                        DISPLAYNAME = groupRow.DISPLAYNAME
        //                    };
        //                    mmCopy.Add(mmRow);
        //                }
        //                // Now we have a good copy ...
        //                // In this routine we set Phase3 to false
        //                if (!await SmartBobV2017.ConnectDisconnectAsync(ourviewmodel,
        //                                                                    ourviewmodel.userToken,
        //                                                                    SmartParametersV2016.connectSymbol,
        //                                                                    ourviewmodel.multiuser,
        //                                                                    ourviewmodel.utcDates,
        //                                                                    lastDate))
        //                {
        //                    if (!ourviewmodel.quitCts.IsCancellationRequested)
        //                    {
        //                        // Set the second Led to Red
        //                        FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.redColour);
        //                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 26: Connecting"))
        //                        {
        //                            return;
        //                        }
        //                        return;
        //                    }
        //                }
        //                else
        //                {
        //                    // How about a test if it was red to see if this is a rest
        //                    if (FrontEndGUI.CompareLedColour(2, ourviewmodel.redColour))
        //                    {
        //                        await SmartRoutinesV2018.TextBlockUpdate(
        //#if ANDROIDX
        //                                                             meterActivity,
        //#endif
        //                                                            ourviewmodel, "Re-connected", true);
        //                    }
        //                    else
        //                    {
        //                        await SmartRoutinesV2018.TextBlockUpdate(
        //#if ANDROIDX
        //                                                             meterActivity,
        //#endif
        //                                                            ourviewmodel, "Connected", true);
        //                    }
        //                    // Set the second Led to Green
        //                    FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.greenColour);
        //                    if (ourviewmodel.mmListX.Count == 0)
        //                    {
        //                        return;
        //                    }
        //                    ProcessMMList(
        //#if ANDROIDX
        //                        meterActivity, 
        //#endif
        //                        mmCopy);

        //                }
        //            });
        //            return;
        //        }

        protected override async Task ExecuteAsync()
        {
#if WPF
    // --- Network check ---
    if (!SmartNibbyV2016.NetworkAvailability())
    {
        await _dispatcher.InvokeAsync(() =>
            FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.redColour));

        return;
    }

    await _dispatcher.InvokeAsync(() =>
        FrontEndGUI.SetLedColour(ourviewmodel, 5, ourviewmodel.greenColour));
#endif

#if SMARTMAUI
            
            // --- Network check ---
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    FrontEndGUI.SetLedColour(
                        ourviewmodel,
                        5,
                        ourviewmodel.redColour);
                });

                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                FrontEndGUI.SetLedColour(
                    ourviewmodel,
                    5,
                    ourviewmodel.greenColour);
            });
#endif

            string lastDate = "";

            // --- Copy list (no UI thread needed) ---
            List<SmartProfile.Groups> mmCopy = new List<SmartProfile.Groups>();

            foreach (var groupRow in ourviewmodel.mmListX)
            {
                mmCopy.Add(new SmartProfile.Groups()
                {
                    USERNAME = groupRow.USERNAME,
                    GROUPNAME = groupRow.GROUPNAME,
                    ACTIVEFLAG = groupRow.ACTIVEFLAG,
                    MARKER = groupRow.MARKER,
                    PDEK = groupRow.PDEK,
                    SENDF = groupRow.SENDF,
                    FDEK = groupRow.FDEK,
                    SENDU = groupRow.SENDU,
                    UDEK = groupRow.UDEK,
                    RECEIVEALL = groupRow.RECEIVEALL,
                    DISPLAYNAME = groupRow.DISPLAYNAME
                });
            }

            // --- Async network call ---
            bool connected = await SmartBobV2017.ConnectDisconnectAsync(
                ourviewmodel,
                SmartParametersV2016.connectSymbol,
                ourviewmodel.multiuser,
                ourviewmodel.utcDates,
                lastDate);

            if (!connected)
            {
                if (!ourviewmodel.quitCts.IsCancellationRequested)
                {
#if WPF
                    await _dispatcher.InvokeAsync(() =>
                    FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.redColour));
#endif
#if SMARTMAUI
                    await MainThread.InvokeOnMainThreadAsync(() =>
                                                        FrontEndGUI.SetLedColour(
                                                            ourviewmodel,
                                                            2,
                                                            ourviewmodel.redColour));
#endif

                    bool traceOk = await SmartRoutinesV2018.CheckTrace(
                        ourviewmodel,
                        ourviewmodel.quitCts.Token,
                        0,
                        0,
                        "Problem 26: Connecting");

                    if (!traceOk)
                        return;
                }

                return;
            }

            // --- UI updates after successful connection ---
#if WPF
            await _dispatcher.InvokeAsync(() =>
            {
                if (FrontEndGUI.CompareLedColour(ourviewmodel, 2, ourviewmodel.redColour))
                {
                    // NOTE: we can't await here, so we trigger separately below
                }

                FrontEndGUI.SetLedColour(ourviewmodel, 2, ourviewmodel.greenColour);
            });
#endif
#if SMARTMAUI
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (FrontEndGUI.CompareLedColour(
                    ourviewmodel,
                    2,
                    ourviewmodel.redColour))
                {
                    // NOTE: we can't await here, so we trigger separately below
                }

                FrontEndGUI.SetLedColour(
                    ourviewmodel,
                    2,
                    ourviewmodel.greenColour);
            });
#endif
            // --- Text update (async, NOT inside dispatcher) ---
            if (FrontEndGUI.CompareLedColour(ourviewmodel, 2, ourviewmodel.redColour))
            {
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                    meterActivity,
#endif
                    ourviewmodel,
                    "Re-connected",
                    true);
            }
            else
            {
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                    meterActivity,
#endif
                    ourviewmodel,
                    "Connected",
                    true);
            }

            // --- Process list ---
            if (ourviewmodel.mmListX.Count == 0)
                return;

            ProcessMMList(
#if WINFORMS
                mainComponents,
#endif
#if ANDROIDX
                meterActivity,
#endif
                mmCopy);
        }
        internal async void ProcessMMList(
#if WINFORMS
                                        MainProcess processComponents,
#endif
#if ANDROIDX
                                        AppCompatActivity meterActivity,
#endif
                                        List<SmartProfile.Groups> mmCopy)
        {
            bool primary = false;
            foreach (SmartProfile.Groups mmRow in ourviewmodel.mmListX) // Checked
            {
                // This test means we ignore our own record RAY = RAY
                if (mmRow.GROUPNAME != ourviewmodel.UserName)
                {
                    // Does it exist in SmartUsers.Consumers
                    bool exists = SmartSpikeV2017.CheckConsumerUser(ourviewmodel,
                                                                    mmRow.GROUPNAME);

                    if (!exists)
                    {
                        // We didn't find it in our target list
                        if (mmRow.PDEK != "")   // We must be able to DECODE!!! You twat!
                        {
                            // It doesn't exist so we need to MB:   SmartUsers
                            //                                    + SmartFinance (if SENDF)
                            //                                    + SmrtUtility  (if SENDU)
                            string ourSchemas = SmartRoutinesV2018.SchemasFromCubeface(signinviewmodel,
                                                                        ourviewmodel);

                            string mmSchemas = "";
                            mmSchemas = SmartRoutinesV2018.MMFromSchemas(mmSchemas, mmRow);

                            if (!await SmartNibbyV2016.MiserableBitch(signinviewmodel,
                                                        ourviewmodel,
                                                        ourviewmodel.UserName,
                                                        mmRow.GROUPNAME,
                                                        mmSchemas))
                            {
                                FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Miserable Bitch failed 1", true);
                                // This works because we're not updating the main thread
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 27: Miserable Bitch failed 1"))
                                {
                                    return;
                                }
                                return;
                            }

                            // Pretty sure this next section is USELESS
                            // Because ExtractSmartUsers on extracts CONSUMERS
                            // table and we're not interested in that for
                            // non-primarys e.g. ANNA or MRS_HAPPY


                            //// Now we have SmartProfile and SmartFinance and/or SmartUtility loaded
                            //// we need to extract them.  So do SmartUsers first:
                            //if (!await SmartPhyllV2020.ExtractSmartUsersX(ourviewmodel,
                            //                            ourviewmodel.sqlitetablesList,
                            //                            //ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
                            //                            primary,
                            //                            SmartParametersV2016.SmartUsersSchema,
                            //                            mmRow.GROUPNAME,
                            //                            mmRow.PDEK))
                            //{
                            //    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            //    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SQLite failed", true);
                            //    // This works because we're not updating the main thread
                            //    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "Problem 28: SQLite failed"))
                            //    {
                            //        return;
                            //    }
                            //    return;
                            //}

                            // Now we have SmartProfile and SmartFinance and/or SmartUtility loaded
                            // we need to extract them.  So do SmartProfile first:
                            // All we are interested in here is the AddressesViews btw
                            if (!await SmartPhyllV2020.ExtractSmartProfile(ourviewmodel,
                                                        ourviewmodel.sqlitetablesList,
                                                        //ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
                                                        primary,
                                                        SmartParametersV2016.SmartProfileSchema,
                                                        mmRow.GROUPNAME,
                                                        mmRow.PDEK))
                            {
                                FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "SQLite failed", true);
                                // This works because we're not updating the main thread
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 28: SQLite failed"))
                                {
                                    return;
                                }
                                return;
                            }

                            // Now we have to do SmartFinance and/or SmartUtility
                            // Check Finance
                            CheckFinanceAdd(
#if WINFORMS
                                            processComponents,
#endif
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            financeviewmodel,
                                            mmRow,
                                            primary);
                            // Check Utility
                            CheckUtilityAdd(
#if WINFORMS
                                            processComponents,
#endif
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            utilityviewmodel,
                                            mmRow,
                                            primary);
                        }
                    }
                    else
                    {
                        // It does exist
                        // Check to see if we need to remove it
                        // But only remove it if it was in before!!
                        foreach (SmartProfile.Groups groupRow in mmCopy) // Checked
                        {
                            if (groupRow.GROUPNAME == mmRow.GROUPNAME)
                            {
                                // If it was True before, but isn't True now..
                                if (groupRow.SENDF && !mmRow.SENDF)
                                {
                                    CheckFinanceRemove(
#if WINFORMS
                                                       processComponents,
#endif
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        mmRow,
                                                        primary);
                                }
                                if (groupRow.SENDU && !mmRow.SENDU)
                                {
                                    CheckUtilityRemove(
#if WINFORMS
                                                            processComponents,
#endif
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel,
                                                            mmRow,
                                                            primary);
                                }
                                break;
                            }
                        }
                        if (!mmRow.SENDF && !mmRow.SENDU)
                        {

                            // Do we REALLY need to RemoveSmartUsers as
                            // we never loaded ANNA or MRS_HAPPY in the first place??
                            SmartPhyllV2020.RemoveSmartUsersX(ourviewmodel,
                                                            ourviewmodel.sqlitetablesList,
                                                            //ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
                                                            SmartParametersV2016.SmartUsersSchema,
                                                            mmRow.GROUPNAME);



                            //int index = 0;
                            //List<SmartUsers.SmartView> temp = new List<SmartUsers.SmartView>();
                            //foreach (SmartUsers.SmartView abc in ourviewmodel.ssListX) // Checked
                            //{
                            //    if (abc.USERNAME != mmRow.GROUPNAME)
                            //    {
                            //        // Remove the one that matches!!!
                            //        // By re-creating all the ones that don't ....
                            //        temp.Add(abc);
                            //        //ourviewmodel.ssList.RemoveAt(index);
                            //        //break;
                            //    }
                            //    index++;
                            //}
                            //ourviewmodel.ssListX = temp;
                        }
                        else
                        {
                            // If there's no need to Remove it
                            // perhaps we should add it in?
                            // Don't forget - SmartUsers is still in at the moment < = NO IT ISN'T
                            // Check Finance against mmCopy: if the PDEK
                            // has changed - add it in
                            foreach (SmartProfile.Groups groupRow in mmCopy) // Checked
                            {
                                if (groupRow.GROUPNAME == mmRow.GROUPNAME)
                                {
                                    if (groupRow.PDEK != mmRow.PDEK)
                                    {
                                        // MB it in. Build the Schemas
                                        string ourSchemas = SmartRoutinesV2018.SchemasFromCubeface(signinviewmodel,
                                                                        ourviewmodel);

                                        //string mmSchemas = SmartRoutinesV2018.MMFromSchemas(mmRow);
                                        string mmSchemas = "";
                                        mmSchemas = SmartRoutinesV2018.MMFromSchemas(mmSchemas, mmRow);

                                        if (!await SmartNibbyV2016.MiserableBitch(signinviewmodel,
                                                        ourviewmodel,
                                                        ourviewmodel.UserName,
                                                        mmRow.GROUPNAME,
                                                        mmSchemas))
                                        {
                                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel, "Miserable Bitch failed 2", true);
                                            // This works because we're not updating the main thread
                                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 29: Miserable Bitch failed 2"))
                                            {
                                                return;
                                            }
                                            return;
                                        }
                                        // Check Finance
                                        CheckFinanceAdd(
#if WINFORMS
                                                       processComponents,
#endif
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        mmRow,
                                                        primary);
                                        // Check Utility
                                        CheckUtilityAdd(
#if WINFORMS
                                                        processComponents,
#endif
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        utilityviewmodel,
                                                        mmRow,
                                                        primary);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return;
        }
        internal static async void CheckFinanceAdd(
#if WINFORMS
                                                MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                SmartProfile.Groups mmRow,
                                                bool primary)
        {
            // Check Finance
            if (mmRow.SENDF &&
                mmRow.FDEK != "")
            {
                if (!await SmartPhyllV2020.ExtractSmartFinance(ourviewmodel,
                            financeviewmodel,
                            ourviewmodel.sqlitetablesList,
                            ourviewmodel.screenCode,
                            primary,
                            SmartParametersV2016.SmartFinanceSchema,
                            mmRow.USERNAME,
                            mmRow.GROUPNAME,
                            mmRow.FDEK))
                {
                    // Set the fourth Led to Red ... and duck out
                    FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 33: extracting PLO " + financeviewmodel.errorMessage))
                    {
                        return;
                    }
                    return;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel, "Loaded SmartFinance: " + mmRow.GROUPNAME, true);

                    if (!await SmartFinanceV2025.DisplayFinanceMeterAsync(
#if WINFORMS
                                                                        components,
#endif
#if ANDROIDX
                                                                        meterActivity,
#endif
                                                                        ourviewmodel,
                                                                        financeviewmodel,
                                                                        SmartParametersV2016.activeFlag,
                                                                        SmartParametersV2016.defaultDate))
                    {
                        // We failed because of a Cancellation ... but which one? Check
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            // If we DIDN'T request a cancellation
                            // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }
                    }
                }
            }
            return;
        }

        internal static async void CheckFinanceRemove(
#if WINFORMS
                                                MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity  meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                SmartProfile.Groups mmRow,
                                                bool primary)
        {
            // Check Finance
            if (!mmRow.SENDF)
            {
                // We already know it is in Consumers as either SENDF and/or SENDU
                // Remove it if we have it
                if (!SmartPhyllV2020.RemoveSmartFinance(ourviewmodel,
                                                            financeviewmodel,
                                                            ourviewmodel.sqlitetablesList,
                                                            ourviewmodel.screenCode,
                                                            SmartParametersV2016.SmartFinanceSchema,
                                                            mmRow.GROUPNAME))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "SQLite failed", true);
                    // This works because we're not updating the main thread
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 34: SQLite failed"))
                    {
                        return;
                    }
                    return;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Removed SmartFinance: " + mmRow.GROUPNAME, true);

                    if (!await SmartFinanceV2025.DisplayFinanceMeterAsync(
#if WINFORMS
                                                                            components,
#endif
#if ANDROIDX
                                                                            meterActivity,
#endif
                                                                            ourviewmodel,
                                                                            financeviewmodel,
                                                                            SmartParametersV2016.activeFlag,
                                                                            SmartParametersV2016.defaultDate))
                    {
                        // We failed because of a Cancellation ... but which one? Check
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            // If we DIDN'T request a cancellation
                            // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }
                    }
                }
            }
            return;
        }

        internal static async void CheckUtilityAdd(
#if WINFORMS
                                                MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                SmartProfile.Groups mmRow,
                                                bool primary)
        {
            if (mmRow.SENDU &&
                mmRow.UDEK != "")
            {
                if (!await SmartPhyllV2020.ExtractSmartUtility(ourviewmodel,
                        utilityviewmodel,
                        ourviewmodel.sqlitetablesList,
                        ourviewmodel.screenCode,
                        primary,
                        SmartParametersV2016.SmartUtilitySchema,
                        mmRow.USERNAME,
                        mmRow.GROUPNAME,
                        mmRow.UDEK))
                {
                    // Set the fourth Led to Red ... and duck out
                    FrontEndGUI.SetLedColour(ourviewmodel, 4, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 35: extracting PLO " + utilityviewmodel.errorMessage))
                    {
                        return;
                    }
                    return;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Loaded SmartUtility: " + mmRow.GROUPNAME, true);

                    if (!await SmartUtilityV2022.DisplayUtilityMeterAsync(
#if WINFORMS
                                                        components,
#endif
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        utilityviewmodel,
                                                        SmartParametersV2016.activeFlag,
                                                        SmartParametersV2016.defaultDate))
                    {
                        // We failed because of a Cancellation ... but which one? Check
                        if (!(ourviewmodel.quitCts.Token.IsCancellationRequested ||
                            utilityviewmodel.utilityToken.IsCancellationRequested))
                        {
                            // If we DIDN'T request a cancellation
                            // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }
                    }
                }
            }
            return;
        }

        internal static async void CheckUtilityRemove(
#if WINFORMS
                                                MainProcess components,
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
#endif
                                                MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                SmartProfile.Groups mmRow,
                                                bool primary)
        {
            if (!mmRow.SENDU)
            {
                // We already know it is in Consumers as either SENDF and/or SENDU
                // Remove it if we have it
                if (!SmartPhyllV2020.RemoveSmartUtility(ourviewmodel,
                                                            utilityviewmodel,
                                                            ourviewmodel.sqlitetablesList,
                                                            ourviewmodel.screenCode,
                                                            SmartParametersV2016.SmartUtilitySchema,
                                                            mmRow.GROUPNAME))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "SQLite failed", true);
                    // This works because we're not updating the main thread
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Problem 36: SQLite failed"))
                    {
                        return;
                    }
                    return;
                }
                else
                {
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel, "Removed SmartUtility: " + mmRow.GROUPNAME, true);

                    if (!await SmartUtilityV2022.DisplayUtilityMeterAsync(
#if WINFORMS
                                                            components,
#endif
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                            utilityviewmodel,
                                                            SmartParametersV2016.activeFlag,
                                                            SmartParametersV2016.defaultDate))
                    {
                        // We failed because of a Cancellation ... but which one? Check
                        if (!(ourviewmodel.quitCts.Token.IsCancellationRequested ||
                            utilityviewmodel.utilityToken.IsCancellationRequested))
                        {
                            // If we DIDN'T request a cancellation
                            // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        }
                    }
                }
            }
            return;
        }
    }
}
