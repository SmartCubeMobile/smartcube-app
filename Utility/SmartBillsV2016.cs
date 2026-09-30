#if WINFORMS
using System.Linq;
using MoreLinq;
#endif

#if WPF
using MoreLinq;
#endif

#if WINUI
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
#endif

#if ANDROIDX
using MoreLinq;
using static System.Linq.Enumerable;
using static System.Linq.Queryable;
#endif

#if SMARTMAUI
#endif

namespace SmartCubeMobile
{
    public enum AnalysisCode : int
    {
        // This add and sub bollocks is no longer needed ...

        AddBbf,    // Balance brought forward
        AddPay,    // Payment - Good for us (a Payment IN makes your account go UP)
        SubRef,    // Refund - ALL amounts get added because a Refund goes BACK
                   // to your bank account - its like a Payment OUT
                   // (A Payment OUT makes your account go DOWN)
        SubAcc,    // Account Charges Credits - Bad for us except Rebate which are good for us
        SubAvt,    // Account Charges VAT - Bad for us <= NOT IMPLEMENTED YET <= Ignore these if the are 0
        SubScc,    // Supply Charges - Bad for us except for duel fuel discounts which are good for us
        SubSvt,    // Supply VAT - Bad for us except for the above
        SubEnc,    // Electricity New Charges - Bad for us
        SubEuc,    // Electricity Unit Charges - Bad for us
        SubEsc,    // Electricity Standing Charges - Bad for us
        AddED,     // Electricity Discount - Good for us
        SubEvt,    // Electricity VAT - Bad for us
        SubGnc,    // Gas New Charges - Bad for us
        SubGuc,    // Gas Unit Charges - Bad for us
        SubGsc,    // Gas Standing Charges - Bad for us
        AddGD,     // Gas Discount - Good for us
        SubGvt,    // Gas VAT - Bad for us
        AddBcf     // Outstanding balance (balance carried forward)
    };

    public class SmartBillsV2016
    {
        internal static bool Analyze_Bills(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel)
        {
            int payments_received = 0;

            // The one we eventually return ...(after lots of fucking about)
            utilityviewmodel.analysis_bills_found.Clear();

            // Load up all the bits and pieces we need ...

            // Select on the Energy Code ... <= NO! Its the ACCOUNT NUMBER!!! 
            // For those Users with ONE Bill for both Electricity and Gas who
            // make ONE payment for both - this will work for either Gas or
            // Electricity as we have a Bill Resource for each (i.e. we double up)
            // (so we use the Account No/Bill/Bill Resource combination later on to select
            // Electricty and Gas - and BOTH selections should work)
            // For those Users with SEPARATE Bills for Electricty and Gas
            // this selection will STILL work as we will have one set of Bills
            // for Electricty with one Supplier .. and another set for Gas
            // with another Supplier.  And the Electricity and Gas selections
            // later on?  They will still work as well, because we won't find
            // anything under Electricty for Gas or anything under Gas for
            // Electricity ... you are SO clever Ray!

            // WHY ARE WE TAKING MPAN OUT OF Utility.Accounts ????
            // Because we don't need it ... Account records get INSERTED but
            // (unlike Consumer_Energy records) they never get UPDATED.  SO if
            // an Account record ends up with MPAN/MPRN as "Unknown" (which is HIGHLY
            // LIKELY) then we can never cross the Account.MPAN_MPRN with anything else.
            // The MPAN/MPRN is really tied to the Username rather than the Account
            //
            // So someone has an Account with a SUpplier, we may or may not know the
            // MPAN/MPRN when we do the first scrape (they may not have any readings OR
            // it may be impossible to obtain e.g. Scottish Power where we have used
            // the Meter Serial No (this is wrong btw))
            // If we DON'T know the MPAN, they still (Scottish Power) might have a Bill
            // and or readings.
            // If we DO know the MPAN, it will be tied in with Consumer_Energy record.
            //
            // What do we really use the MPAN/MPRN for?  For identification on the screen
            // and for making sure that people don't use TWO different Usernames to try
            // and circumvent the Expiry Date restrictions.  That's all.  We only ever
            // use it for on-screen information, never for analysis.  Its just a unique
            // number after all, and we may never even know it on occasion..


            // You have bills the size of Flying Saucers, Ray ....

            // So we don't put these two fuckers in twice on every resource
            List<SmartUtility.Payments> payments_temp = new List<SmartUtility.Payments>();
            foreach (SmartUtility.Payments payments_row in utilityviewmodel.Hezbollah.paymentsList)
            {
                payments_temp.Add(payments_row);
            }
            List<SmartUtility.Unallocated> unallocated_temp = new List<SmartUtility.Unallocated>();
            foreach (SmartUtility.Unallocated unallocated_row in utilityviewmodel.Hezbollah.unallocatedList)
            {
                unallocated_temp.Add(unallocated_row);
            }

            // Build new one starting from scratch
            List<SmartUtility.Analysis_Bills> analysis_billsList = new List<SmartUtility.Analysis_Bills>();


            List<SmartUtility.Bills> working_bills_found =
                SmartSpikeUtilityV2017.Utility_Configure_Bills(ourviewmodel,
                                                                utilityviewmodel);

            if (working_bills_found.Count > 0)
            {
                // Use accounts_found from above
                List<SmartUtility.AccChargesCredits> account_charges_credits_found =
                    SmartSpikeUtilityV2017.Utility_AccountChargesCreditsList(utilityviewmodel, working_bills_found, utilityviewmodel.Hezbollah.account_charges_creditsList);
                // Use Utility.Accounts found from above
                List<SmartUtility.SupChargesCredits> supply_charges_credits_found = SmartSpikeUtilityV2017.Utility_SupplyChargesCreditsList(utilityviewmodel, working_bills_found,
                                                                                                                        utilityviewmodel.Hezbollah.supply_charges_creditsList);
                // Well.. the first check we should make is that (apart from the first Bill)
                // the OUTSTANDING_BALANCE from one Bill should match the PREVIOUS_BALANCE from the next Bill
                utilityviewmodel.PREVIOUS_BALANCE = "";
                utilityviewmodel.OUTSTANDING_BALANCE = "";

                string description = "Previous balance";

                List<SmartUtility.BillsResource> working_bills_resource_temp = new List<SmartUtility.BillsResource>();
                foreach (SmartUtility.Bills working_bills_row in working_bills_found)
                {
                    List<SmartUtility.BillsResource> working_bills_resource_found =
                                    SmartSpikeUtilityV2017.Utility_Find_BillResource(utilityviewmodel,
                                                                                     working_bills_row);
                    if (working_bills_resource_found.Count > 0)
                    {
                        Set_Balances(utilityviewmodel,
                                    working_bills_row,
                                    working_bills_resource_found);
                        //rf PREVIOUS_BALANCE,
                        //rf OUTSTANDING_BALANCE);

                        if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                                        working_bills_row.ACCOUNT_NO,
                                                        working_bills_row.BILL_DATE,
                                                        working_bills_row.STATEMENT_ID,
                                                        working_bills_row.BILL_PERIOD_START,
                                                        AnalysisCode.AddBbf,
                                                        description,
                                                        Convert.ToInt32(utilityviewmodel.PREVIOUS_BALANCE),    // Because BBF is sub
                                                        Convert.ToInt32(utilityviewmodel.OUTSTANDING_BALANCE),
                                                        analysis_billsList))
                        {
                            utilityviewmodel.analysis_bills_found = analysis_billsList;
                            return true;
                        }
                        description = "Balance brought forward";
                        if (!ABC(ourviewmodel,
                            utilityviewmodel,
                            working_bills_row,
                            analysis_billsList,
                            working_bills_row,
                            working_bills_resource_found,
                            account_charges_credits_found,
                            supply_charges_credits_found))
                        {
                            return false;
                        }
                        foreach (SmartUtility.BillsResource bill_resource_row in working_bills_resource_found)
                        {
                            working_bills_resource_temp.Add(bill_resource_row);
                        }
                    }
                }


                // Any allocated Payments to do?
                // The temp helps ensure we don't put them in twice
                List<SmartUtility.Payments> payments_found = SmartSpikeUtilityV2017.Utility_PaymentsAllocated(utilityviewmodel,
                                                                                    working_bills_resource_temp,
                                                                                    payments_temp);
                payments_received = 0;
                if (payments_found.Count > 0)
                {
                    foreach (SmartUtility.Payments payments_view_row in payments_found)
                    {
                        payments_received += payments_view_row.PAYMENT_AMOUNT;
                        utilityviewmodel.payment_method = "";
                        if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentMethod(ourviewmodel,
                                                                utilityviewmodel,
                                                                payments_view_row.PAYMENT_CODE))
                        {
                            return false;
                        }
                        if (!Insert_Analysis_Bills(payments_view_row.USERNAME,
                                                    payments_view_row.ACCOUNT_NO,
                                                    payments_view_row.BILL_DATE,
                                                    payments_view_row.STATEMENT_ID,
                                                    payments_view_row.PAYMENT_DATE,
                                                    AnalysisCode.AddPay,
                                                    utilityviewmodel.payment_method,
                                                    payments_view_row.PAYMENT_AMOUNT,
                                                    payments_view_row.PAYMENT_BALANCE,
                                                    analysis_billsList))
                        {
                            utilityviewmodel.analysis_bills_found = analysis_billsList;
                            return false;
                        }
                        payments_temp.Remove(payments_view_row);
                    }
                }

                // Any unallocated Payments to do?
                // The temp helps ensure we don't put them in twice
                List<SmartUtility.Unallocated> payments_unallocated_found = SmartSpikeUtilityV2017.Utility_PaymentsUnallocated(ourviewmodel,
                                                                                                                                        utilityviewmodel,
                                                                                                                                        unallocated_temp);

                // This select is done across Suppliers and Utility.Accounts because it is BILL_DATE
                // which is most important and should make any Supplier and/or Account appear in order
                // Well, that's the theory ..... !
                List<SmartUtility.Analysis_Bills> analysis_bills_temp = new List<SmartUtility.Analysis_Bills>(from Analysis_Bill
                                                                                                                                                in analysis_billsList
                                                                                                              orderby Analysis_Bill.BILL_DATE ascending,
                                                                                                                      Analysis_Bill.DATE ascending,
                                                                                                                      Analysis_Bill.ITEM ascending
                                                                                                              select Analysis_Bill);
                if (payments_unallocated_found.Count > 0)
                {
                    foreach (SmartUtility.Unallocated payments_unallocated_row in payments_unallocated_found)
                    {
                        payments_received += payments_unallocated_row.PAYMENT_AMOUNT;
                        utilityviewmodel.payment_method = "";
                        if (!SmartSpikeUtilityV2017.Utility_Lookup_PaymentMethod(ourviewmodel,
                                                                    utilityviewmodel,
                                                                    payments_unallocated_row.PAYMENT_CODE))
                        {
                            return false;
                        }
                        if (!Insert_Analysis_Bills(payments_unallocated_row.USERNAME,
                                                payments_unallocated_row.ACCOUNT_NO,
                                                payments_unallocated_row.BILL_DATE,
                                                payments_unallocated_row.STATEMENT_ID,
                                                payments_unallocated_row.PAYMENT_DATE,
                                                AnalysisCode.AddPay,
                                                utilityviewmodel.payment_method,
                                                payments_unallocated_row.PAYMENT_AMOUNT,
                                                payments_unallocated_row.PAYMENT_BALANCE,
                                                analysis_bills_temp))
                        {
                            utilityviewmodel.analysis_bills_found = analysis_bills_temp;
                            return false;
                        }
                        List<SmartUtility.Unallocated> temp = new List<SmartUtility.Unallocated>();
                        foreach (SmartUtility.Unallocated temp_row in unallocated_temp)
                        {
                            // If it DOESN'T match then keep it in 
                            // If it DOES match then DON'T keep it in
                            // because we are REMOVING it
                            if (!(temp_row.USERNAME == payments_unallocated_row.USERNAME &&
                                temp_row.CUBEFACE_CODE == payments_unallocated_row.CUBEFACE_CODE &&
                                temp_row.SUPPLIER_CODE == payments_unallocated_row.SUPPLIER_CODE &&
                                temp_row.BRAND_CODE == payments_unallocated_row.BRAND_CODE &&
                                temp_row.ACCOUNT_CREATED == payments_unallocated_row.ACCOUNT_CREATED &&
                                temp_row.ACCOUNT_NO == payments_unallocated_row.ACCOUNT_NO &&
                                temp_row.STATEMENT_ID == payments_unallocated_row.STATEMENT_ID &&
                                temp_row.BILL_DATE == payments_unallocated_row.BILL_DATE &&
                                temp_row.PAYMENT_DATE == payments_unallocated_row.PAYMENT_DATE &&
                                temp_row.PAYMENT_ITEM == payments_unallocated_row.PAYMENT_ITEM &&
                                temp_row.PAYMENT_CODE == payments_unallocated_row.PAYMENT_CODE &&
                                temp_row.PAYMENT_AMOUNT == payments_unallocated_row.PAYMENT_AMOUNT &&
                                temp_row.PAYMENT_BALANCE == payments_unallocated_row.PAYMENT_BALANCE))
                            {
                                temp.Add(temp_row);
                            }
                        }
                        unallocated_temp = temp;
                        //unallocated_temp.Remove(payments_unallocated_row);
                    }
                }

                // Phew!!  Got there in the end!!
                int outstanding_balance = 0;
                int running_total = 0;

                // Anything to do?
                if (analysis_bills_temp.Count > 0)
                {
                    int count = 0;

                    foreach (SmartUtility.Accounts accounts_row in utilityviewmodel.Hezbollah.utility_accountsList)
                    {
                        // Phew!!  Got there in the end!!
                        outstanding_balance = 0;
                        running_total = 0;
                        count = 0;

                        foreach (SmartUtility.Analysis_Bills analysis_bills_row in analysis_bills_temp)
                        {
                            if (analysis_bills_row.ACCOUNT_NO == accounts_row.ACCOUNT_NO)
                            {
                                analysis_bills_row.AMOUNT *= -1;    // Make it negative
                                analysis_bills_row.BALANCE *= -1;   // Make it negative

                                analysis_bills_row.CODE = analysis_bills_row.CODE.Substring(0, 3).ToUpper();
                                if (analysis_bills_row.CODE.Contains("BBF"))
                                {
                                    // So we are not comparing Decimals (which might fail) but output strings
                                    if ((count > 0) &&
                                        (outstanding_balance != analysis_bills_row.AMOUNT))
                                    {
                                        analysis_bills_row.DESCRIPTION = ">> MISMATCH <<";
                                        // They don't match - all is good - fix the last_total
                                        running_total = analysis_bills_row.AMOUNT;
                                    }
                                    else
                                    {
                                        // They match - all is good - fix the last_total
                                        running_total = analysis_bills_row.AMOUNT; // ? not AMOUNT
                                    }
                                    outstanding_balance = analysis_bills_row.BALANCE; // Holds OB for a Bill
                                }
                                else
                                {
                                    running_total += analysis_bills_row.AMOUNT;
                                    analysis_bills_row.BALANCE = running_total;
                                }
                                count++;
                            }
                        }
                    }

                    utilityviewmodel.analysis_bills_found.AddRange(analysis_bills_temp);
                }

                // All this JUST to get rid of duplicate BBFs!!!
                utilityviewmodel.analysis_bills_found = new List<SmartUtility.Analysis_Bills>(from Analysis_Bill
                                        in utilityviewmodel.analysis_bills_found
                                                                                              orderby Analysis_Bill.BILL_DATE ascending,
                                                                                                      Analysis_Bill.DATE ascending,
                                                                                                      Analysis_Bill.ITEM ascending
                                                                                              select Analysis_Bill);
                utilityviewmodel.analysis_bills_found =
                    new List<SmartUtility.Analysis_Bills>(utilityviewmodel.analysis_bills_found.DistinctBy(key => new
                    {
                        key.USERNAME,
                        key.ACCOUNT_NO,
                        key.STATEMENT_ID,
                        key.BILL_DATE,
                        key.DATE,
                        key.ITEM,
                        key.CODE,
                        key.DESCRIPTION,
                        key.AMOUNT,
                        key.BALANCE
                    }));
            }
            return true;
        }

        private static void Set_Balances(UtilityViewModel utilityviewmodel,
                                            SmartUtility.Bills bills_row,
                                            List<SmartUtility.BillsResource> bills_resource_found)
        //rf int PREVIOUS_BALANCE,
        //rf int OUTSTANDING_BALANCE)
        {
            if (bills_row.RESOURCE_BALANCES)
            {
                utilityviewmodel.PREVIOUS_BALANCE = bills_resource_found[0].PREVIOUS_RESOURCE_BALANCE.ToString();
                utilityviewmodel.OUTSTANDING_BALANCE = bills_resource_found[0].OUTSTANDING_RESOURCE_BALANCE.ToString();
            }
            else
            {
                utilityviewmodel.PREVIOUS_BALANCE = bills_row.PREVIOUS_BALANCE.ToString();
                utilityviewmodel.OUTSTANDING_BALANCE = bills_row.OUTSTANDING_BALANCE.ToString();
            }
            return;
        }

        internal static bool Account_Tables(UtilityViewModel utilityviewmodel,
                                SmartUtility.Bills bills_row,
                                List<SmartUsers.VatRates> vatRatesList,
                                List<SmartUtility.Analysis_Bills> analysis_billsList,
                                List<SmartUtility.AccChargesCredits> account_charges_credits_found,
                                List<SmartUtility.SupChargesCredits> supply_charges_credits_found)
        {
            string account_type = "",
                        supply_type = "";

            int account_amount = 0,
                        account_vat_amount = 0,
                        supply_amount = 0,
                        supply_vat_amount = 0;

            decimal supply_vat_ratex = 0.0M;

            string account_include_bills;

            DateTime account_date = SmartParametersV2016.defaultDate,
                        supply_date = SmartParametersV2016.defaultDate;

            List<SmartUtility.AccChargesCredits> bill_account_charges =
                new List<SmartUtility.AccChargesCredits>
                (from Account_Charges in account_charges_credits_found
                 where ((Account_Charges.CUBEFACE_CODE == bills_row.CUBEFACE_CODE) &&
                     (Account_Charges.SUPPLIER_CODE == bills_row.SUPPLIER_CODE) &&
                     (Account_Charges.BRAND_CODE == bills_row.BRAND_CODE) &&
                     (Account_Charges.ACCOUNT_NO == bills_row.ACCOUNT_NO) &&
                     (Account_Charges.ACCOUNT_CREATED == bills_row.ACCOUNT_CREATED) &&
                     (Account_Charges.STATEMENT_ID == bills_row.STATEMENT_ID) &&
                     (Account_Charges.BILL_DATE == bills_row.BILL_DATE))
                 select Account_Charges);
            if (bill_account_charges.Count > 0)
            {
                foreach (SmartUtility.AccChargesCredits account_charges_credits_row in bill_account_charges)
                {
                    account_date = account_charges_credits_row.ACCOUNT_DATE;
                    account_type = account_charges_credits_row.ACCOUNT_TYPE;
                    account_amount = account_charges_credits_row.ACCOUNT_AMOUNT;
                    account_include_bills = account_charges_credits_row.INCLUDE_BILLS;

                    // There aren't many Bill items we ignore (in fact this is the only one)
                    // But SP Foreigners again ... see Bill 29-Oct-2015 .. they show it but don't include it
                    if (account_include_bills == SmartParametersV2016.yesFlag)
                    {
                        if (!Insert_Analysis_Bills(bills_row.USERNAME,
                                                bills_row.ACCOUNT_NO,
                                                bills_row.BILL_DATE,
                                                bills_row.STATEMENT_ID,
                                                account_date,
                                                AnalysisCode.SubAcc,
                                                account_type,
                                                account_amount,
                                                0,
                                                analysis_billsList))
                        {
                            return false;
                        }

                        // VAT each individual ACC item using the onboard Vat code
                        if (account_charges_credits_row.ACCOUNT_VAT_CODE > 0)
                        {
                            utilityviewmodel.VAT_RATE = 0.0M;
                            if (!SmartParseV2016.Lookup_Vat_Rate(account_charges_credits_row.ACCOUNT_VAT_CODE,
                                                                account_date,
                                                                account_date,
                                                                vatRatesList,
                                                                utilityviewmodel))
                            {
                                return false;
                            }
                            account_type = "VAT @ " + utilityviewmodel.VAT_RATE.ToString();
                            account_vat_amount = Convert.ToInt32((account_amount * utilityviewmodel.VAT_RATE) / 100.0M);
                            if (account_vat_amount != 0)
                            {
                                if (!Insert_Analysis_Bills(bills_row.USERNAME,
                                                        bills_row.ACCOUNT_NO,
                                                        bills_row.BILL_DATE,
                                                        bills_row.STATEMENT_ID,
                                                        account_date,
                                                        AnalysisCode.SubAvt,
                                                        account_type,
                                                        account_vat_amount,
                                                        0,
                                                        analysis_billsList))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            List<SmartUtility.SupChargesCredits> bill_supply_charges = new List<SmartUtility.SupChargesCredits>(from Supply_Charges
                                                                  in supply_charges_credits_found
                                                                                                                where ((Supply_Charges.CUBEFACE_CODE == bills_row.CUBEFACE_CODE) &&
                                                                                                                        (Supply_Charges.SUPPLIER_CODE == bills_row.SUPPLIER_CODE) &&
                                                                                                                        (Supply_Charges.BRAND_CODE == bills_row.BRAND_CODE) &&
                                                                                                                        (Supply_Charges.ACCOUNT_NO == bills_row.ACCOUNT_NO) &&
                                                                                                                        (Supply_Charges.ACCOUNT_CREATED == bills_row.ACCOUNT_CREATED) &&
                                                                                                                        (Supply_Charges.STATEMENT_ID == bills_row.STATEMENT_ID) &&
                                                                                                                        (Supply_Charges.BILL_DATE == bills_row.BILL_DATE))
                                                                                                                select Supply_Charges);
            if (bill_supply_charges.Count > 0)
            {
                foreach (SmartUtility.SupChargesCredits supply_charges_credits_row in bill_supply_charges)
                {
                    supply_date = supply_charges_credits_row.SUPPLY_DATE;
                    supply_type = supply_charges_credits_row.SUPPLY_TYPE;
                    supply_amount = supply_charges_credits_row.SUPPLY_AMOUNT;

                    if (!Insert_Analysis_Bills(bills_row.USERNAME,
                                                bills_row.ACCOUNT_NO,
                                                bills_row.BILL_DATE,
                                                bills_row.STATEMENT_ID,
                                                supply_date,
                                                AnalysisCode.SubScc,
                                                supply_type,
                                                supply_amount,
                                                0,
                                                analysis_billsList))
                    {
                        return false;
                    }

                    // VAT each individual SCC using the onboard code
                    if (supply_charges_credits_row.SUPPLY_VAT_CODE > 0)
                    {
                        supply_vat_ratex = 0.0M;
                        if (!SmartParseV2016.Lookup_Vat_Rate(supply_charges_credits_row.SUPPLY_VAT_CODE, supply_date, supply_date, vatRatesList, utilityviewmodel))// rf supply_vat_ratex))
                        {
                            return false;
                        }
                        supply_type = "VAT @ " + supply_vat_ratex.ToString();
                        supply_vat_amount = Convert.ToInt32((supply_amount * supply_vat_ratex) / 100.0M);
                        if (!Insert_Analysis_Bills(bills_row.USERNAME,
                                                    bills_row.ACCOUNT_NO,
                                                    bills_row.BILL_DATE,
                                                    bills_row.STATEMENT_ID,
                                                    bills_row.BILL_PERIOD_END,
                                                    AnalysisCode.SubSvt,
                                                    supply_type,
                                                    supply_vat_amount,
                                                    0,
                                                    analysis_billsList))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal static bool ABC(MainViewModel ourviewmodel,
                                UtilityViewModel utilityviewmodel,
                                SmartUtility.Bills bills_row,
                                List<SmartUtility.Analysis_Bills> analysis_billsList,
                                SmartUtility.Bills working_bills_row,
                                List<SmartUtility.BillsResource> working_bills_resource_found,
                                List<SmartUtility.AccChargesCredits> account_charges_credits_found,
                                List<SmartUtility.SupChargesCredits> supply_charges_credits_found)
        {

            if (!Account_Tables(utilityviewmodel,
                                bills_row,
                                ourviewmodel.Blanche.vatRatesList,
                                analysis_billsList,
                                account_charges_credits_found,
                                supply_charges_credits_found))
            {
                return false;
            }

            utilityviewmodel.bill_e_unit_charges = new List<SmartUtility.EUnitCharges>();
            utilityviewmodel.bill_g_unit_charges = new List<SmartUtility.GUnitCharges>();
            utilityviewmodel.bill_d_unit_charges = new List<SmartUtility.EUnitCharges>();

            utilityviewmodel.bill_e_standing_charges = new List<SmartUtility.EStandingCharges>();
            utilityviewmodel.bill_g_standing_charges = new List<SmartUtility.GStandingCharges>();
            utilityviewmodel.bill_d_standing_charges = new List<SmartUtility.EStandingCharges>();

            utilityviewmodel.bill_e_discounts = new List<SmartUtility.EDiscounts>();
            utilityviewmodel.bill_g_discounts = new List<SmartUtility.GDiscounts>();
            utilityviewmodel.bill_d_discounts = new List<SmartUtility.EDiscounts>();

            foreach (SmartUtility.BillsResource working_bills_resource_row in working_bills_resource_found)
            {
                char local_resource_code = SmartSpikeUtilityV2017.Utility_Lookup_Meters_ResourceCode(ourviewmodel,
                                                                                            working_bills_resource_row.CUBEFACE_CODE,
                                                                                           working_bills_resource_row.MPAN_MPRN,
                                                                                           utilityviewmodel.Hezbollah.utility_metersList);
                switch (local_resource_code)
                {
                    case SmartParametersV2016.Electricity:
                        if (!Do_The_Electricity(ourviewmodel,
                                    utilityviewmodel,
                                    local_resource_code,
                                    working_bills_row,
                                    working_bills_resource_row,
                                    analysis_billsList))
                        {
                            return false;
                        }
                        break;

                    case SmartParametersV2016.Gas:
                        if (!Do_The_Gas(ourviewmodel,
                                    utilityviewmodel,
                                    local_resource_code,
                                    working_bills_row,
                                    working_bills_resource_row,
                                    analysis_billsList))
                        {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        internal static bool Do_The_Electricity(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        char local_resource_code,
                                        SmartUtility.Bills working_bills_row,
                                        SmartUtility.BillsResource working_bills_resource_row,
                                        List<SmartUtility.Analysis_Bills> analysis_billsList)
        {
            string e_unit_of_measure;
            short e_vat_code = 0;

            int total_e_units_amount = 0,
                    total_e_charges_amount = 0,
                    total_e_discount_amount = 0;

            decimal e_unitsx,
                    e_units_ratex,
                    e_vat_ratex = 0.0M;

            DateTime e_read_date = SmartParametersV2016.defaultDate;

            SmartSpikeUtilityV2017.Utility_Analyze_UnitCharges(utilityviewmodel,
                                                    local_resource_code,
                                                    working_bills_resource_row);
            if (utilityviewmodel.bill_e_unit_charges.Count > 0)
            {
                foreach (SmartUtility.EUnitCharges e_unit_charges_row in utilityviewmodel.bill_e_unit_charges)
                {
                    e_read_date = e_unit_charges_row.UNIT_CHARGES_PERIOD_START;
                    e_unitsx = e_unit_charges_row.UNITS;
                    e_units_ratex = e_unit_charges_row.UNITS_RATE;
                    e_unit_of_measure = e_unit_charges_row.UNIT_OF_MEASURE;

                    if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                                working_bills_row.ACCOUNT_NO,
                                                working_bills_row.BILL_DATE,
                                                working_bills_row.STATEMENT_ID,
                                                e_unit_charges_row.UNIT_CHARGES_PERIOD_END,
                                                AnalysisCode.SubEuc,
                                                e_unitsx.ToString() + e_unit_of_measure + " x " + e_units_ratex.ToString() + " per " + e_unit_of_measure,
                                                e_unit_charges_row.UNITS_COST,
                                                0,
                                                analysis_billsList))
                    {
                        return false;
                    }
                    total_e_units_amount += e_unit_charges_row.UNITS_COST;
                }
            }

            SmartSpikeUtilityV2017.Utility_Analyze_StandingCharges(utilityviewmodel,
                                                        local_resource_code,
                                                        working_bills_resource_row.STATEMENT_ID,  // <= This is NEVER empty??
                                                        working_bills_resource_row);
            if (utilityviewmodel.bill_e_standing_charges.Count > 0)
            {
                foreach (SmartUtility.EStandingCharges e_standing_charges_row in utilityviewmodel.bill_e_standing_charges)
                {
                    if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                                working_bills_row.ACCOUNT_NO,
                                                working_bills_row.BILL_DATE,
                                                working_bills_row.STATEMENT_ID,
                                                e_standing_charges_row.STANDING_CHARGES_PERIOD_END,
                                                AnalysisCode.SubEsc,
                                                e_standing_charges_row.CHARGES_DAYS + " days x " + e_standing_charges_row.STANDING_CHARGE + " per day",
                                                e_standing_charges_row.CHARGES_COST,
                                                0,
                                                analysis_billsList))
                    {
                        return false;
                    }
                    total_e_charges_amount += e_standing_charges_row.CHARGES_COST;
                }
            }

            SmartSpikeUtilityV2017.Utility_Analyze_Discounts(utilityviewmodel,
                                                local_resource_code,
                                                working_bills_resource_row);
            if (utilityviewmodel.bill_e_discounts.Count > 0)
            {
                foreach (SmartUtility.EDiscounts e_discounts_row in utilityviewmodel.bill_e_discounts)
                {
                    if (e_vat_code == 0) { e_vat_code = e_discounts_row.DISCOUNT_VAT_CODE; }
                    if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                                working_bills_row.ACCOUNT_NO,
                                                working_bills_row.BILL_DATE,
                                                working_bills_row.STATEMENT_ID,
                                                e_discounts_row.DISCOUNT_DATE, // When it applies, not the credit date which is when it is shown on the bill
                                                AnalysisCode.AddED,
                                                e_discounts_row.DISCOUNT_TYPE,
                                                e_discounts_row.DISCOUNT_AMOUNT, // A postive discount is a debit for VAT
                                                0,
                                                analysis_billsList))
                    {
                        return false;
                    }
                    total_e_discount_amount += e_discounts_row.DISCOUNT_AMOUNT;
                }
            }

            // VAT the Units and the Standing Charges together - 01 is the standard rate
            if ((total_e_units_amount + total_e_charges_amount + total_e_discount_amount) != 0.0M)
            {
                e_vat_code = working_bills_resource_row.RESOURCE_VAT_CODE;
                if (!SmartParseV2016.Lookup_Vat_Rate(e_vat_code, e_read_date, e_read_date, ourviewmodel.Blanche.vatRatesList, utilityviewmodel)) //rf e_vat_ratex))
                {
                    return false;
                }

                if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                            working_bills_row.ACCOUNT_NO,
                                            working_bills_row.BILL_DATE,
                                            working_bills_row.STATEMENT_ID,
                                            working_bills_row.BILL_PERIOD_END,
                                            AnalysisCode.SubEvt,
                                            "VAT @ " + e_vat_ratex.ToString(), //e_type,
                                            working_bills_resource_row.RESOURCE_VAT_AMOUNT, //e_vat_amount,
                                            0,
                                            analysis_billsList))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool Do_The_Gas(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        char local_resource_code,
                                        SmartUtility.Bills working_bills_row,
                                        SmartUtility.BillsResource working_bills_resource_row,
                                        List<SmartUtility.Analysis_Bills> analysis_billsList)
        {
            string g_unit_of_measure;

            short g_vat_code;

            int total_g_units_amount = 0,
                    total_g_charges_amount = 0,
                    total_g_discount_amount = 0;

            decimal g_unitsx,
                    g_units_ratex,
                    g_vat_ratex = 0.0M;

            DateTime g_read_date = SmartParametersV2016.defaultDate;

            SmartSpikeUtilityV2017.Utility_Analyze_UnitCharges(utilityviewmodel,
                                                    local_resource_code,
                                                    working_bills_resource_row);
            if (utilityviewmodel.bill_g_unit_charges.Count > 0)
            {
                foreach (SmartUtility.GUnitCharges g_unit_charges_row in utilityviewmodel.bill_g_unit_charges)
                {
                    g_read_date = g_unit_charges_row.UNIT_CHARGES_PERIOD_START;
                    g_unitsx = g_unit_charges_row.UNITS;
                    g_units_ratex = g_unit_charges_row.UNITS_RATE;
                    g_unit_of_measure = g_unit_charges_row.UNIT_OF_MEASURE;

                    if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                                working_bills_row.ACCOUNT_NO,
                                                working_bills_row.BILL_DATE,
                                                working_bills_row.STATEMENT_ID,
                                                g_unit_charges_row.UNIT_CHARGES_PERIOD_END,
                                                AnalysisCode.SubGuc,
                                                g_unitsx + g_unit_of_measure + " x " + g_units_ratex + " per " + g_unit_of_measure,
                                                g_unit_charges_row.UNITS_COST,
                                                0,
                                                analysis_billsList))
                    {
                        return false;
                    }
                    total_g_units_amount += g_unit_charges_row.UNITS_COST;
                }
            }

            SmartSpikeUtilityV2017.Utility_Analyze_StandingCharges(utilityviewmodel,
                                                        local_resource_code,
                                                        working_bills_resource_row.STATEMENT_ID,
                                                        working_bills_resource_row);
            if (utilityviewmodel.bill_g_standing_charges.Count > 0)
            {
                foreach (SmartUtility.GStandingCharges g_standing_charges_row in utilityviewmodel.bill_g_standing_charges)
                {
                    if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                                working_bills_row.ACCOUNT_NO,
                                                working_bills_row.BILL_DATE,
                                                working_bills_row.STATEMENT_ID,
                                                g_standing_charges_row.STANDING_CHARGES_PERIOD_END,
                                                AnalysisCode.SubGsc,
                                                g_standing_charges_row.CHARGES_DAYS + " days x " + g_standing_charges_row.STANDING_CHARGE + " per day",
                                                g_standing_charges_row.CHARGES_COST,
                                                0,
                                                analysis_billsList))
                    {
                        return false;
                    }
                    total_g_charges_amount += g_standing_charges_row.CHARGES_COST;
                }
            }

            SmartSpikeUtilityV2017.Utility_Analyze_Discounts(utilityviewmodel,
                                                local_resource_code,
                                                working_bills_resource_row);
            if (utilityviewmodel.bill_g_discounts.Count > 0)
            {
                foreach (SmartUtility.GDiscounts g_discounts_row in utilityviewmodel.bill_g_discounts)
                {
                    g_vat_code = g_discounts_row.DISCOUNT_VAT_CODE;
                    if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                                working_bills_row.ACCOUNT_NO,
                                                working_bills_row.BILL_DATE,
                                                working_bills_row.STATEMENT_ID,
                                                g_discounts_row.DISCOUNT_DATE, // When it applies, not the credit date which is when it is shown on the bill
                                                AnalysisCode.AddGD,
                                                g_discounts_row.DISCOUNT_TYPE,
                                                g_discounts_row.DISCOUNT_AMOUNT,
                                                0,
                                                analysis_billsList))
                    {
                        return false;
                    }
                    total_g_discount_amount += g_discounts_row.DISCOUNT_AMOUNT;
                }
            }

            // VAT the Units and the Standing Charges together - 01 is the standard rate
            if ((total_g_units_amount + total_g_charges_amount + total_g_discount_amount) != 0.0M)
            {
                g_vat_code = working_bills_resource_row.RESOURCE_VAT_CODE;
                if (!SmartParseV2016.Lookup_Vat_Rate(g_vat_code, g_read_date, g_read_date, ourviewmodel.Blanche.vatRatesList, utilityviewmodel)) //rf g_vat_ratex))
                {
                    return false;
                }

                if (!Insert_Analysis_Bills(working_bills_row.USERNAME,
                                            working_bills_row.ACCOUNT_NO,
                                            working_bills_row.BILL_DATE,
                                            working_bills_row.STATEMENT_ID,
                                            working_bills_row.BILL_PERIOD_END,
                                            AnalysisCode.SubGvt,
                                            "VAT @ " + g_vat_ratex.ToString(),
                                            working_bills_resource_row.RESOURCE_VAT_AMOUNT,
                                            0,
                                            analysis_billsList))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool Insert_Analysis_Bills(string username,
                                                    string account_no,
                                                    DateTime bill_date,
                                                    string bill_number,
                                                    DateTime date,
                                                    Enum mnemonic,
                                                    string description,
                                                    int amount,
                                                    int total,
                                                    List<SmartUtility.Analysis_Bills> analysis_billsList)
        {
            SmartUtility.Analysis_Bills analysis_bills_row = new SmartUtility.Analysis_Bills()
            {
                USERNAME = username,
                ACCOUNT_NO = account_no,
                BILL_DATE = bill_date,
                STATEMENT_ID = bill_number,
                DATE = date,
                ITEM = Convert.ToInt16(mnemonic),
                CODE = mnemonic.ToString(),
                DESCRIPTION = description,
                AMOUNT = amount,
                BALANCE = total
            };
            analysis_billsList.Add(analysis_bills_row);
            return true;
        }
    }
}