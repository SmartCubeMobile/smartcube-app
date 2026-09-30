using System.Reflection;

#if WINFORMS
using System.Linq;
using MoreLinq;
#endif

#if WPF
using MoreLinq;
#endif

#if WINUI
using System.Linq;
using MoreLinq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;         // <= THANK FUCK FOR THIS -- I have had **MAJOR BUGS*** in my code!!!!
#endif

#if ANDROIDX
using static System.Linq.Queryable;
using static System.Linq.Enumerable;
using MoreLinq;
#endif

#if SMARTMAUI
#endif

namespace SmartCubeMobile
{

    internal class SmartSpikeUtilityV2017
    {
        #region SmartUtility

#if WINFORMS

        internal static List<SmartUtility.Bills> Utility_Find_Bill(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                short supplier_code,
                                                short brand_code,
                                                string account_no,
                                                DateTime created)
        {

            List<SmartUtility.Bills> bills_found =
                new List<SmartUtility.Bills>
                (from Bills_Out in utilityviewmodel.Hezbollah.bills_changesList
                 where Bills_Out.USERNAME == ourviewmodel.UserName &&
                         Bills_Out.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                         Bills_Out.SUPPLIER_CODE == supplier_code &&
                         Bills_Out.BRAND_CODE == brand_code &&
                         Bills_Out.ACCOUNT_CREATED == created &&
                         Bills_Out.ACCOUNT_NO == account_no &&
                         Bills_Out.PAYMENT_PLAN == SmartParametersV2016.defaultChar
                 select Bills_Out);

            return bills_found;
        }

        internal static List<SmartUsers.Consumers> Utility_Configure_Consumers(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
            List<SmartUsers.Consumers> consumers_found =
                new List<SmartUsers.Consumers>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 where Consumer.Include && 
                         Cubeface.FACE_ACTIVE &&
                         Cubeface.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 select Consumer);
            return consumers_found;
        }
        internal static List<SmartProfile.Cubefaces> Utility_Configure_Cubefaces(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel)
        {
            List<SmartProfile.Cubefaces> cubefaces_found =
                new List<SmartProfile.Cubefaces>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)
                 select Cubeface);
            return cubefaces_found;
        }

        internal static List<SmartUtility.Resources> Utility_Configure_Resources(MainViewModel ourviewmodel,
                                                                                        UtilityViewModel utilityviewmodel)
        {
            // The way this works is ... if 'resource_code = D' then you
            // get BOTH 'E' and 'G' returned. However if 'resource_code = E'
            // or 'resource_code = G' you only get that one returned (if, of
            // cours, it exists)
            List<SmartUtility.Resources> resource_found =
            new List<SmartUtility.Resources>
            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)
             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
             on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
             equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
             select Resource);
            if (utilityviewmodel.resource_code != SmartParametersV2016.DualFuel)
            {
                resource_found =
                    new List<SmartUtility.Resources>(from Resource_New in resource_found
                                                                     where Resource_New.RESOURCE_CODE == utilityviewmodel.resource_code
                                                                     select Resource_New);
            }
            return resource_found;
        }

        internal static List<SmartUtility.ResourcesTypes> Utility_Configure_ResourcesTypes(MainViewModel ourviewmodel,
                                                                                        UtilityViewModel utilityviewmodel)
        {
            // The way this works is ... if 'resource_code = D' then you
            // get BOTH 'E' and 'G' returned. However if 'resource_code = E'
            // or 'resource_code = G' you only get that one returned (if, of
            // cours, it exists)
            List<SmartUtility.ResourcesTypes> resources_types_found =
                                new List<SmartUtility.ResourcesTypes>
                                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)
                                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                                 join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                                 equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                                 select ResourcesType);
            if (utilityviewmodel.resource_code != SmartParametersV2016.DualFuel)
            {
                resources_types_found =
                    new List<SmartUtility.ResourcesTypes>
                    (from Resources_Type_New in resources_types_found
                     where Resources_Type_New.RESOURCE_CODE == utilityviewmodel.resource_code
                     select Resources_Type_New);
            }
            return resources_types_found;
        }

        internal static List<SmartUtility.Accounts> Utility_Configure_Accounts(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.Accounts> accounts_found = new List<SmartUtility.Accounts>();

            if (utilityviewmodel.resource_code == SmartParametersV2016.DualFuel)
            {
                accounts_found =
                    new List<SmartUtility.Accounts>(from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                                                    join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                                                                    on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                                                                    equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                                                                    join Account in utilityviewmodel.Hezbollah.utility_accountsList
                                                                    on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                                                                    equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                                                                    select Account);
            }
            else
            {
                accounts_found =
                    new List<SmartUtility.Accounts>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)
                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join Account in utilityviewmodel.Hezbollah.utility_accountsList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                     where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                     select Account);
            }
            return accounts_found;
        }

        internal static List<SmartUtility.Logins> Utility_Configure_Logins(MainViewModel ourviewmodel,
                                                                                            UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.Logins> logins_found =
                new List<SmartUtility.Logins>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Login in utilityviewmodel.Hezbollah.utility_loginsList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Login.USERNAME, Login.CUBEFACE_CODE }
                 select Login);
            return logins_found;
        }

        internal static List<SmartUtility.Switches> Utility_Configure_Switches(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.Switches> switches_found =
               new List<SmartUtility.Switches>
               (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                join Switch in utilityviewmodel.Hezbollah.utility_switchesList
                on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                equals new { Switch.USERNAME, Switch.CUBEFACE_CODE }
                select Switch);
            return switches_found;
        }

        internal static List<SmartUtility.Meters> Utility_Configure_Meters(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.Meters> meters_found = new List<SmartUtility.Meters>();
            if (utilityviewmodel.resource_code == SmartParametersV2016.DualFuel)
            {
                meters_found = new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE, ResourcesType.RESOURCE_TYPE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE, Meter.RESOURCE_TYPE }
                     select Meter);
            }
            else
            {
                meters_found = new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE, ResourcesType.RESOURCE_TYPE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE, Meter.RESOURCE_TYPE }
                     where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                     select Meter);
            }
            return meters_found;
        }

        internal static List<SmartUtility.BankDetails> Utility_Configure_BankDetails(MainViewModel ourviewmodel,
                                                                                                UtilityViewModel utilityviewmodel)
        {
            // Have ABSOLUTELY NO IDEA why I joined the Account with BankDetails
            // over Account.ACCOUNT_CREATED = BankDetails.ACCOUNT_CREATED
            // because there's no obvious link between the two
            List<SmartUtility.BankDetails> bank_details_found =
                new List<SmartUtility.BankDetails>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)
                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 join BankDetail in utilityviewmodel.Hezbollah.utility_bankdetailsList
                 on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.SUPPLIER_CODE, Account.BRAND_CODE, Account.ACCOUNT_NO }
                 equals new { BankDetail.USERNAME, BankDetail.CUBEFACE_CODE, BankDetail.SUPPLIER_CODE, BankDetail.BRAND_CODE, BankDetail.ACCOUNT_NO }
                 where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                 select BankDetail);
            return bank_details_found;
        }

        internal static List<SmartUtility.Bills> Utility_Configure_Bills_Winforms(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                string statement_id,
                                                                DateTime bill_date)
        {
            List<SmartUtility.Bills> bills_found = new List<SmartUtility.Bills>();
            // Note: This is ACROSS all Suppliers ... and all MPANs
            bills_found = new List<SmartUtility.Bills>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 join Bill in utilityviewmodel.Hezbollah.billsList
                 on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.ACCOUNT_CREATED, Account.ACCOUNT_NO }
                 equals new { Bill.USERNAME, Bill.CUBEFACE_CODE, Bill.ACCOUNT_CREATED, Bill.ACCOUNT_NO }
                 orderby Bill.BILL_PERIOD_START ascending
                 select Bill);
            bills_found = new List<SmartUtility.Bills>(bills_found.DistinctBy(key => new
            {
                key.USERNAME,
                key.CUBEFACE_CODE,
                key.SUPPLIER_CODE,
                key.BRAND_CODE,
                key.ACCOUNT_CREATED,
                key.ACCOUNT_NO,
                key.BILL_DATE,
                key.STATEMENT_ID
            }));
            if (!string.IsNullOrEmpty(statement_id))
            {
                bills_found = new List<SmartUtility.Bills>
                    (from BillsFound in bills_found
                     where ((BillsFound.BILL_DATE == bill_date) &&
                             (BillsFound.STATEMENT_ID == statement_id))
                     orderby BillsFound.BILL_PERIOD_START ascending
                     select BillsFound);
            }
            else
            {
                if (utilityviewmodel.resource_code != SmartParametersV2016.DualFuel)
                {
                    bills_found = new List<SmartUtility.Bills>
                        (from BillsFound in bills_found
                         join BillsResource in utilityviewmodel.Hezbollah.bills_resourceList
                         on (BillsFound.USERNAME,
                             BillsFound.CUBEFACE_CODE,
                             BillsFound.SUPPLIER_CODE,
                             BillsFound.BRAND_CODE,
                             BillsFound.ACCOUNT_CREATED,
                             BillsFound.ACCOUNT_NO,
                             BillsFound.STATEMENT_ID,
                             BillsFound.BILL_DATE)
                         equals (BillsResource.USERNAME,
                             BillsResource.CUBEFACE_CODE,
                             BillsResource.SUPPLIER_CODE,
                             BillsResource.BRAND_CODE,
                             BillsResource.ACCOUNT_CREATED,
                             BillsResource.ACCOUNT_NO,
                             BillsResource.STATEMENT_ID,
                             BillsResource.BILL_DATE)
                         join Meters in utilityviewmodel.Hezbollah.utility_metersList
                         on (BillsResource.USERNAME, BillsResource.CUBEFACE_CODE, BillsResource.MPAN_MPRN)
                         equals (Meters.USERNAME, Meters.CUBEFACE_CODE, Meters.MPAN_MPRN)
                         where (Meters.RESOURCE_CODE == utilityviewmodel.resource_code)
                         orderby BillsFound.BILL_PERIOD_START ascending
                         select BillsFound);
                }
            }
            return bills_found;
        }
        internal static List<SmartUtility.BillsResource> Utility_Configure_BillsResource(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        // How could I miss this when MPAN_MPRN is a key???!!??
        {
            List<SmartUtility.BillsResource> bills_resource_found = new List<SmartUtility.BillsResource>();
            if (utilityviewmodel.resource_code == SmartParametersV2016.defaultResourceCode)
            {
                bills_resource_found =
                        new List<SmartUtility.BillsResource>
                        (from Bill in utilityviewmodel.Hezbollah.working_billsList
                         join Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                         on new
                         {
                             Bill.USERNAME,
                             Bill.CUBEFACE_CODE,
                             Bill.SUPPLIER_CODE,
                             Bill.BRAND_CODE,
                             Bill.ACCOUNT_CREATED,
                             Bill.ACCOUNT_NO,
                             Bill.STATEMENT_ID,
                             Bill.BILL_DATE
                         }
                         equals new
                         {
                             Bill_Resource.USERNAME,
                             Bill_Resource.CUBEFACE_CODE,
                             Bill_Resource.SUPPLIER_CODE,
                             Bill_Resource.BRAND_CODE,
                             Bill_Resource.ACCOUNT_CREATED,
                             Bill_Resource.ACCOUNT_NO,
                             Bill_Resource.STATEMENT_ID,
                             Bill_Resource.BILL_DATE
                         }
                         join Meter in utilityviewmodel.Hezbollah.utility_metersList
                         on new
                         {
                             Bill_Resource.USERNAME,
                             Bill_Resource.CUBEFACE_CODE,
                             Bill_Resource.MPAN_MPRN
                         }
                         equals new
                         {
                             Meter.USERNAME,
                             Meter.CUBEFACE_CODE,
                             Meter.MPAN_MPRN
                         }
                         //where //Bill.USERNAME == ourviewmodel.UserName &&
                         //        Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                         orderby Bill.BILL_PERIOD_START ascending
                         select Bill_Resource);
                bills_resource_found = new List<SmartUtility.BillsResource>
                    (bills_resource_found.DistinctBy(key => new
                    {
                        key.USERNAME,
                        key.CUBEFACE_CODE,
                        key.SUPPLIER_CODE,
                        key.BRAND_CODE,
                        key.ACCOUNT_CREATED,
                        key.ACCOUNT_NO,
                        key.STATEMENT_ID,
                        key.BILL_DATE,
                        key.MPAN_MPRN
                    }
                ));
            }
            else
            {
                bills_resource_found = new List<SmartUtility.BillsResource>
                    (from Bill in utilityviewmodel.Hezbollah.working_billsList
                     join Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                         on new
                         {
                             Bill.USERNAME,
                             Bill.CUBEFACE_CODE,
                             Bill.SUPPLIER_CODE,
                             Bill.BRAND_CODE,
                             Bill.ACCOUNT_CREATED,
                             Bill.ACCOUNT_NO,
                             Bill.STATEMENT_ID,
                             Bill.BILL_DATE
                         }
                         equals new
                         {
                             Bill_Resource.USERNAME,
                             Bill_Resource.CUBEFACE_CODE,
                             Bill_Resource.SUPPLIER_CODE,
                             Bill_Resource.BRAND_CODE,
                             Bill_Resource.ACCOUNT_CREATED,
                             Bill_Resource.ACCOUNT_NO,
                             Bill_Resource.STATEMENT_ID,
                             Bill_Resource.BILL_DATE
                         }
                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new
                     {
                         Bill_Resource.USERNAME,
                         Bill_Resource.CUBEFACE_CODE,
                         Bill_Resource.MPAN_MPRN
                     }
                     equals new
                     {
                         Meter.USERNAME,
                         Meter.CUBEFACE_CODE,
                         Meter.MPAN_MPRN
                     }
                     where //Bill.USERNAME == ourviewmodel.UserName &&
                           //Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                           Meter.RESOURCE_CODE == utilityviewmodel.resource_code
                     orderby Bill.BILL_PERIOD_START ascending
                     select Bill_Resource);
            }
            return bills_resource_found;
        }

        internal static void Utility_Configure_Readings(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            char resource_code)
        {
            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Note: This is ACROSS all Suppliers ... and all MPANs
                    utilityviewmodel.e_readings_found = Utility_EReadings(utilityviewmodel);
                    break;
                case SmartParametersV2016.Gas:
                    // Note: This is ACROSS all Suppliers ... and all MPRNs
                    utilityviewmodel.g_readings_found = Utility_GReadings(utilityviewmodel);
                    break;
                default:
                    // Do the impossible!!
                    utilityviewmodel.d_readings_found = Utility_EReadings(utilityviewmodel);
                    utilityviewmodel.g_readings_found = Utility_GReadings(utilityviewmodel);
                    foreach (SmartUtility.GReadings g_readings_row in utilityviewmodel.g_readings_found)
                    {
                        SmartUtility.EReadings d_readings_row = new()
                        // Common
                        {
                            USERNAME = g_readings_row.USERNAME,
                            CUBEFACE_CODE = g_readings_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_readings_row.SUPPLIER_CODE,
                            BRAND_CODE = g_readings_row.BRAND_CODE,
                            ACCOUNT_CREATED = g_readings_row.ACCOUNT_CREATED,
                            ACCOUNT_NO = g_readings_row.ACCOUNT_NO,
                            STATEMENT_ID = g_readings_row.STATEMENT_ID,
                            BILL_DATE = g_readings_row.BILL_DATE,
                            MPAN_MPRN = g_readings_row.MPAN_MPRN,
                            READINGS_PERIOD_START = g_readings_row.READINGS_PERIOD_START,
                            READINGS_PERIOD_END = g_readings_row.READINGS_PERIOD_END,
                            METER_SERIAL_NO = g_readings_row.METER_SERIAL_NO,
                            READ_TYPE = g_readings_row.READ_TYPE,
                            D_LAST_READ = g_readings_row.D_LAST_READ,
                            D_THIS_READ = g_readings_row.D_THIS_READ,
                            D_UNITS_USED = g_readings_row.D_UNITS_USED_KWH,
                            N_LAST_READ = 0.0M,
                            N_THIS_READ = 0.0M,
                            N_UNITS_USED = 0.0M,
                            UNIT_OF_MEASURE = SmartParametersV2016.defaultUoM
                        };
                        utilityviewmodel.d_readings_found.Add(d_readings_row);
                    }
                    utilityviewmodel.d_readings_found =
                        new List<SmartUtility.EReadings>
                        (from D_Reading in utilityviewmodel.d_readings_found
                         orderby D_Reading.BILL_DATE ascending
                         select D_Reading);
                    break;
            }
            return;
        }

        internal static void Utility_Configure_UnitCharges(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        {
            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.e_unit_charges_found = Utility_EUnitCharges(utilityviewmodel);
                    break;
                case SmartParametersV2016.Gas:
                    utilityviewmodel.g_unit_charges_found = Utility_GUnitCharges(utilityviewmodel);
                    break;
                default:
                    utilityviewmodel.d_unit_charges_found = Utility_EUnitCharges(utilityviewmodel);
                    utilityviewmodel.g_unit_charges_found = Utility_GUnitCharges(utilityviewmodel);
                    foreach (SmartUtility.GUnitCharges g_unit_charges_row in utilityviewmodel.g_unit_charges_found)
                    {
                        SmartUtility.EUnitCharges d_unit_charges_row = new SmartUtility.EUnitCharges()
                        {
                            USERNAME = g_unit_charges_row.USERNAME,
                            CUBEFACE_CODE = g_unit_charges_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_unit_charges_row.SUPPLIER_CODE,
                            BRAND_CODE = g_unit_charges_row.BRAND_CODE,
                            ACCOUNT_CREATED = g_unit_charges_row.ACCOUNT_CREATED,
                            ACCOUNT_NO = g_unit_charges_row.ACCOUNT_NO,
                            STATEMENT_ID = g_unit_charges_row.STATEMENT_ID,
                            BILL_DATE = g_unit_charges_row.BILL_DATE,
                            MPAN_MPRN = g_unit_charges_row.MPAN_MPRN,
                            UNIT_CHARGES_PERIOD_START = g_unit_charges_row.UNIT_CHARGES_PERIOD_START,
                            UNIT_CHARGES_PERIOD_END = g_unit_charges_row.UNIT_CHARGES_PERIOD_END,
                            UNITS_TIME = 'D',
                            UNITS_BAND = g_unit_charges_row.UNITS_BAND,
                            UNITS_TYPE = g_unit_charges_row.UNITS_TYPE,
                            UNITS = g_unit_charges_row.UNITS,
                            UNITS_RATE = g_unit_charges_row.UNITS_RATE,
                            UNIT_OF_MEASURE = g_unit_charges_row.UNIT_OF_MEASURE,
                            UNITS_COST = g_unit_charges_row.UNITS_COST
                        };
                        utilityviewmodel.d_unit_charges_found.Add(d_unit_charges_row);
                    }
                    utilityviewmodel.d_unit_charges_found =
                        new List<SmartUtility.EUnitCharges>
                        (from D_Unit_Charges in utilityviewmodel.d_unit_charges_found
                         orderby D_Unit_Charges.BILL_DATE ascending
                         select D_Unit_Charges);
                    break;
            }
            return;
        }

        internal static void Utility_Configure_StandingCharges(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        {
            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Note: This is ACROSS all Suppliers ... and all MPANs
                    utilityviewmodel.e_standing_charges_found = Utility_EStandingCharges(utilityviewmodel);
                    break;
                case SmartParametersV2016.Gas:
                    // Note: This is ACROSS all Suppliers ... and all MPRNs
                    utilityviewmodel.g_standing_charges_found = Utility_GStandingCharges(utilityviewmodel);
                    break;
                default:
                    utilityviewmodel.d_standing_charges_found = Utility_EStandingCharges(utilityviewmodel);
                    utilityviewmodel.g_standing_charges_found = Utility_GStandingCharges(utilityviewmodel);

                    foreach (SmartUtility.GStandingCharges g_standing_charges_row in utilityviewmodel.g_standing_charges_found)
                    {
                        SmartUtility.EStandingCharges d_standing_charges_row = new SmartUtility.EStandingCharges
                        {
                            USERNAME = g_standing_charges_row.USERNAME,
                            CUBEFACE_CODE = g_standing_charges_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_standing_charges_row.SUPPLIER_CODE,
                            BRAND_CODE = g_standing_charges_row.BRAND_CODE,
                            ACCOUNT_CREATED = g_standing_charges_row.ACCOUNT_CREATED,
                            ACCOUNT_NO = g_standing_charges_row.ACCOUNT_NO,
                            STATEMENT_ID = g_standing_charges_row.STATEMENT_ID,
                            BILL_DATE = g_standing_charges_row.BILL_DATE,
                            MPAN_MPRN = g_standing_charges_row.MPAN_MPRN,
                            STANDING_CHARGES_PERIOD_START = g_standing_charges_row.STANDING_CHARGES_PERIOD_START,
                            STANDING_CHARGES_PERIOD_END = g_standing_charges_row.STANDING_CHARGES_PERIOD_END,
                            CHARGES_ITEM = g_standing_charges_row.CHARGES_ITEM,
                            CHARGES_TYPE = g_standing_charges_row.CHARGES_TYPE,
                            STANDING_CHARGE = g_standing_charges_row.STANDING_CHARGE,
                            CHARGES_DAYS = g_standing_charges_row.CHARGES_DAYS,
                            CHARGES_COST = g_standing_charges_row.CHARGES_COST,
                        };
                        utilityviewmodel.d_standing_charges_found.Add(d_standing_charges_row);
                    }
                    utilityviewmodel.d_standing_charges_found = new List<SmartUtility.EStandingCharges>
                        (from D_Standing_Charges in utilityviewmodel.d_standing_charges_found
                         orderby D_Standing_Charges.BILL_DATE ascending
                         select D_Standing_Charges);
                    break;
            }
            return;
        }

        internal static void Utility_Configure_Discounts(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (utilityviewmodel.resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Note: This is ACROSS all Suppliers ... and all MPANs
                    utilityviewmodel.e_discounts_found = Utility_EDiscounts(utilityviewmodel);
                    break;
                case SmartParametersV2016.Gas:
                    // Note: This is ACROSS all Suppliers ... and all MPRNs
                    utilityviewmodel.g_discounts_found = Utility_GDiscounts(utilityviewmodel);
                    break;
                default:
                    utilityviewmodel.d_discounts_found = Utility_EDiscounts(utilityviewmodel);
                    utilityviewmodel.g_discounts_found = Utility_GDiscounts(utilityviewmodel);
                    foreach (SmartUtility.GDiscounts g_discounts_row in utilityviewmodel.g_discounts_found)
                    {
                        SmartUtility.EDiscounts d_discounts_row = new SmartUtility.EDiscounts
                        {
                            USERNAME = g_discounts_row.USERNAME,
                            CUBEFACE_CODE = g_discounts_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_discounts_row.SUPPLIER_CODE,
                            BRAND_CODE = g_discounts_row.BRAND_CODE,
                            ACCOUNT_CREATED = g_discounts_row.ACCOUNT_CREATED,
                            ACCOUNT_NO = g_discounts_row.ACCOUNT_NO,
                            STATEMENT_ID = g_discounts_row.STATEMENT_ID,
                            BILL_DATE = g_discounts_row.BILL_DATE,
                            MPAN_MPRN = g_discounts_row.MPAN_MPRN,
                            DISCOUNT_DATE = g_discounts_row.DISCOUNT_DATE,
                            DISCOUNT_ITEM = g_discounts_row.DISCOUNT_ITEM,
                            DISCOUNT_TYPE = g_discounts_row.DISCOUNT_TYPE,
                            DISCOUNT_VAT_CODE = g_discounts_row.DISCOUNT_VAT_CODE,
                            DISCOUNT_AMOUNT = g_discounts_row.DISCOUNT_AMOUNT
                        };
                        utilityviewmodel.d_discounts_found.Add(d_discounts_row);
                    }
                    utilityviewmodel.d_discounts_found = new List<SmartUtility.EDiscounts>
                        (from D_Discounts in utilityviewmodel.d_discounts_found
                         orderby D_Discounts.BILL_DATE ascending
                         select D_Discounts);
                    break;
            }
            return;
        }

        internal static void Utility_Configure_Charts(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        char resource_code)
        {
            utilityviewmodel.e_readings_found = new List<SmartUtility.EReadings>();
            utilityviewmodel.g_readings_found = new List<SmartUtility.GReadings>();
            utilityviewmodel.d_readings_found = new List<SmartUtility.EReadings>();

            Utility_Configure_Readings(ourviewmodel,
                            utilityviewmodel,
                            resource_code);
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    foreach (SmartUtility.EUsage e_usage_row in utilityviewmodel.Hezbollah.e_usageList)
                    {
                        List<SmartUtility.EReadings> e_readings =
                            new List<SmartUtility.EReadings>
                            (from E_Read in utilityviewmodel.e_readings_found
                             where ((E_Read.USERNAME == e_usage_row.USERNAME &&
                                     E_Read.CUBEFACE_CODE == e_usage_row.CUBEFACE_CODE &&
                                     E_Read.MPAN_MPRN == e_usage_row.MPAN_MPRN &&
                                     E_Read.READINGS_PERIOD_START <= e_usage_row.USAGE_DATETIME) &&
                                     (e_usage_row.USAGE_DATETIME <= E_Read.READINGS_PERIOD_END))
                             select E_Read);
                        if (e_readings.Count > 0)
                        {
                            foreach (SmartUtility.EReadings e_readings_row in e_readings)
                            {
                                SmartUtility.EUsageView e_usage_view_row = new SmartUtility.EUsageView()
                                {
                                    USERNAME = e_readings_row.USERNAME,
                                    CUBEFACE_CODE = e_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = e_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_CREATED = e_readings_row.ACCOUNT_CREATED,
                                    ACCOUNT_NO = e_readings_row.ACCOUNT_NO,
                                    MPAN_MPRN = e_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = e_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = e_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = e_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = e_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.e_usage_chart_found.Add(e_usage_view_row);
                            }
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    foreach (SmartUtility.GUsage g_usage_row in utilityviewmodel.Hezbollah.g_usageList)
                    {
                        List<SmartUtility.GReadings> g_readings = new List<SmartUtility.GReadings>
                            (from G_Read in utilityviewmodel.g_readings_found
                             where ((G_Read.USERNAME == g_usage_row.USERNAME &&
                                     G_Read.CUBEFACE_CODE == g_usage_row.CUBEFACE_CODE &&
                                     G_Read.MPAN_MPRN == g_usage_row.MPAN_MPRN &&
                                     G_Read.READINGS_PERIOD_START <= g_usage_row.USAGE_DATETIME) &&
                                     (g_usage_row.USAGE_DATETIME <= G_Read.READINGS_PERIOD_END))
                             select G_Read);
                        if (g_readings.Count > 0)
                        {
                            foreach (SmartUtility.GReadings g_readings_row in g_readings)
                            {
                                SmartUtility.GUsageView g_usage_view_row = new SmartUtility.GUsageView()
                                {
                                    USERNAME = g_readings_row.USERNAME,
                                    CUBEFACE_CODE = g_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = g_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_CREATED = g_readings_row.ACCOUNT_CREATED,
                                    ACCOUNT_NO = g_readings_row.ACCOUNT_NO,
                                    MPAN_MPRN = g_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = g_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = g_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = g_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = g_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.g_usage_chart_found.Add(g_usage_view_row);
                            }
                        }
                    }
                    break;
                default:
                    foreach (SmartUtility.EUsage e_usage_row in utilityviewmodel.Hezbollah.e_usageList)
                    {
                        List<SmartUtility.EReadings> e_readings =
                            new List<SmartUtility.EReadings>
                            (from E_Read in utilityviewmodel.e_readings_found
                             where ((E_Read.USERNAME == e_usage_row.USERNAME &&
                                     E_Read.CUBEFACE_CODE == e_usage_row.CUBEFACE_CODE &&
                                     E_Read.MPAN_MPRN == e_usage_row.MPAN_MPRN &&
                                     E_Read.READINGS_PERIOD_START <= e_usage_row.USAGE_DATETIME) &&
                                     (e_usage_row.USAGE_DATETIME <= E_Read.READINGS_PERIOD_END))
                             select E_Read);
                        if (e_readings.Count > 0)
                        {
                            foreach (SmartUtility.EReadings e_readings_row in e_readings)
                            {
                                SmartUtility.EUsageView d_usage_view_row = new SmartUtility.EUsageView()
                                {
                                    USERNAME = e_readings_row.USERNAME,
                                    CUBEFACE_CODE = e_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = e_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_CREATED = e_readings_row.ACCOUNT_CREATED,
                                    ACCOUNT_NO = e_readings_row.ACCOUNT_NO,
                                    MPAN_MPRN = e_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = e_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = e_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = e_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = e_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.d_usage_chart_found.Add(d_usage_view_row);
                            }
                        }
                    }
                    foreach (SmartUtility.GUsage g_usage_row in utilityviewmodel.Hezbollah.g_usageList)
                    {
                        List<SmartUtility.GReadings> g_readings =
                            new List<SmartUtility.GReadings>
                            (from G_Read in utilityviewmodel.g_readings_found
                             where ((G_Read.USERNAME == g_usage_row.USERNAME &&
                                     G_Read.CUBEFACE_CODE == g_usage_row.CUBEFACE_CODE &&
                                     G_Read.MPAN_MPRN == g_usage_row.MPAN_MPRN &&
                                     G_Read.READINGS_PERIOD_START <= g_usage_row.USAGE_DATETIME) &&
                                     (g_usage_row.USAGE_DATETIME <= G_Read.READINGS_PERIOD_END))
                             select G_Read);
                        if (g_readings.Count > 0)
                        {
                            foreach (SmartUtility.GReadings g_readings_row in g_readings)
                            {
                                SmartUtility.EUsageView d_usage_view_row = new SmartUtility.EUsageView()
                                {
                                    USERNAME = g_readings_row.USERNAME,
                                    CUBEFACE_CODE = g_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = g_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_CREATED = g_readings_row.ACCOUNT_CREATED,
                                    ACCOUNT_NO = g_readings_row.ACCOUNT_NO,
                                    MPAN_MPRN = g_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = g_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = g_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = g_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = g_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.d_usage_chart_found.Add(d_usage_view_row);
                            }
                        }
                    }
                    utilityviewmodel.d_usage_chart_found =
                        new List<SmartUtility.EUsageView>
                        (from D_Usage_View in utilityviewmodel.d_usage_chart_found
                         orderby D_Usage_View.USAGE_DATETIME ascending
                         select D_Usage_View);
                    break;
            }
            return;
        }

        internal static List<SmartUtility.Payments> Utility_Configure_PaymentsAllocated(MainViewModel ourviewmodel,
                                                                                                        UtilityViewModel utilityviewmodel)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs ... not
            List<SmartUtility.Accounts> accounts_found = Utility_Configure_Accounts(ourviewmodel,
                                                                                                    utilityviewmodel);
            List<SmartUtility.Payments> payments_found =
                new List<SmartUtility.Payments>
                (from Bill in utilityviewmodel.Hezbollah.billsList
                 join Payment in utilityviewmodel.Hezbollah.paymentsList
                 on new
                 {
                     Bill.USERNAME,
                     Bill.CUBEFACE_CODE,
                     Bill.SUPPLIER_CODE,
                     Bill.BRAND_CODE,
                     Bill.ACCOUNT_CREATED,
                     Bill.ACCOUNT_NO,
                     Bill.STATEMENT_ID,
                     Bill.BILL_DATE
                 }
                 equals new
                 {
                     Payment.USERNAME,
                     Payment.CUBEFACE_CODE,
                     Payment.SUPPLIER_CODE,
                     Payment.BRAND_CODE,
                     Payment.ACCOUNT_CREATED,
                     Payment.ACCOUNT_NO,
                     Payment.STATEMENT_ID,
                     Payment.BILL_DATE
                 }
                 join Accounts in accounts_found
                 on new
                 {
                     Payment.USERNAME,
                     Payment.CUBEFACE_CODE,
                     Payment.ACCOUNT_CREATED,
                     Payment.ACCOUNT_NO
                 }
                 equals new
                 {
                     Accounts.USERNAME,
                     Accounts.CUBEFACE_CODE,
                     Accounts.ACCOUNT_CREATED,
                     Accounts.ACCOUNT_NO
                 }
                 orderby Payment.PAYMENT_DATE ascending
                 select Payment);
            return payments_found;
        }

        internal static List<SmartUtility.AccChargesCredits> Utility_Configure_AccountChargesCredits(MainViewModel ourviewmodel,
                                                                                                                    UtilityViewModel utilityviewmodel)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs ... not
            List<SmartUtility.AccChargesCredits> account_charges_credits_found =
                        new List<SmartUtility.AccChargesCredits>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                         on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                         equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                         join Account in utilityviewmodel.Hezbollah.utility_accountsList
                         on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                         join AccCharges in utilityviewmodel.Hezbollah.account_charges_creditsList
                         on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.SUPPLIER_CODE, Account.BRAND_CODE, Account.ACCOUNT_CREATED, Account.ACCOUNT_NO }
                         equals new { AccCharges.USERNAME, AccCharges.CUBEFACE_CODE, AccCharges.SUPPLIER_CODE, AccCharges.BRAND_CODE, AccCharges.ACCOUNT_CREATED, AccCharges.ACCOUNT_NO }
                         where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                         select AccCharges);

            return account_charges_credits_found;
        }

        internal static List<SmartUtility.SupChargesCredits> Utility_Configure_SupplyChargesCredits(MainViewModel ourviewmodel,
                                                                                                            UtilityViewModel utilityviewmodel)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs ... not
            List<SmartUtility.SupChargesCredits> supply_charges_credits_found =
                        new List<SmartUtility.SupChargesCredits>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                         on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                         equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                         join Account in utilityviewmodel.Hezbollah.utility_accountsList
                         on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                         join SupCharges in utilityviewmodel.Hezbollah.supply_charges_creditsList
                         on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.SUPPLIER_CODE, Account.BRAND_CODE, Account.ACCOUNT_CREATED, Account.ACCOUNT_NO }
                         equals new { SupCharges.USERNAME, SupCharges.CUBEFACE_CODE, SupCharges.SUPPLIER_CODE, SupCharges.BRAND_CODE, SupCharges.ACCOUNT_CREATED, SupCharges.ACCOUNT_NO }
                         where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                         select SupCharges);

            return supply_charges_credits_found;
        }

        internal static List<SmartUtility.TariffDetails> Utility_Configure_TariffDetails(MainViewModel ourviewmodel,
                                                                                                UtilityViewModel utilityviewmodel)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs!!
            // And by implication ... across all RESOURCE_CODES!
            List<SmartUtility.TariffDetails> TariffDetails_found =
                new List<SmartUtility.TariffDetails>();
            TariffDetails_found =
                new List<SmartUtility.TariffDetails>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join TariffDetail in utilityviewmodel.Hezbollah.tariff_detailsList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { TariffDetail.USERNAME, TariffDetail.CUBEFACE_CODE }
                 join BillResource in utilityviewmodel.Hezbollah.bills_resourceList
                 on new
                 {
                     TariffDetail.USERNAME,
                     TariffDetail.CUBEFACE_CODE,
                     TariffDetail.SUPPLIER_CODE,
                     TariffDetail.BRAND_CODE,
                     TariffDetail.ACCOUNT_CREATED,
                     TariffDetail.ACCOUNT_NO,
                     TariffDetail.STATEMENT_ID,
                     TariffDetail.BILL_DATE,
                     TariffDetail.MPAN_MPRN
                 }
                equals new
                {
                    BillResource.USERNAME,
                    BillResource.CUBEFACE_CODE,
                    BillResource.SUPPLIER_CODE,
                    BillResource.BRAND_CODE,
                    BillResource.ACCOUNT_CREATED,
                    BillResource.ACCOUNT_NO,
                    BillResource.STATEMENT_ID,
                    BillResource.BILL_DATE,
                    BillResource.MPAN_MPRN
                }
                //where //BillResource.USERNAME == ourviewmodel.UserName &&
                //BillResource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 orderby TariffDetail.PRICES_VALID_FROM ascending
                 select TariffDetail);

            return TariffDetails_found;
        }

        internal static List<SmartUtility.TariffDetails> Utility_Find_TariffDetails(MainViewModel ourviewmodel,
                                                               UtilityViewModel utilityviewmodel,
                                                               string statement_id,
                                                              DateTime bill_date,
                                                              List<SmartUtility.TariffDetails> TariffDetailsList,
                                                              List<SmartUtility.Bills> working_billsList,
                                                              List<SmartUtility.BillsResource> bills_resourceList)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs
            List<SmartUtility.TariffDetails> TariffDetails_found = new List<SmartUtility.TariffDetails>();
            TariffDetails_found = new List<SmartUtility.TariffDetails>
                (from Tariff_Detail in TariffDetailsList
                 join Bill_Resource in bills_resourceList
                 on new
                 {
                     Tariff_Detail.USERNAME,
                     Tariff_Detail.CUBEFACE_CODE,
                     Tariff_Detail.SUPPLIER_CODE,
                     Tariff_Detail.BRAND_CODE,
                     Tariff_Detail.ACCOUNT_CREATED,
                     Tariff_Detail.ACCOUNT_NO,
                     Tariff_Detail.STATEMENT_ID,
                     Tariff_Detail.BILL_DATE,
                     Tariff_Detail.MPAN_MPRN
                 }
                 equals new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN
                 }
                 //where //Bill_Resource.USERNAME == ourviewmodel.UserName &&
                 //Bill_Resource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 orderby Tariff_Detail.PRICES_VALID_FROM ascending
                 select Tariff_Detail);
            return TariffDetails_found;
        }
#endif

        internal static List<SmartUtility.ResourcesTypes> Utility_Find_ResourcesTypes(MainViewModel ourviewmodel,
                                                                                                UtilityViewModel utilityviewmodel,
                                                                                                char resource_code)
        {
            // The way this works is ... if 'resource_code = D' then you
            // get BOTH 'E' and 'G' returned. However if 'resource_code = E'
            // or 'resource_code = G' you only get that one returned (if, of
            // course, it exists)
            List<SmartUtility.ResourcesTypes> resources_types_found =
                                new List<SmartUtility.ResourcesTypes>
                                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                                 join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                                 equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                                 select ResourcesType);
            if (resource_code != SmartParametersV2016.DualFuel)
            {
                resources_types_found =
                    new List<SmartUtility.ResourcesTypes>
                    (from Resources_Type_New in resources_types_found
                     where Resources_Type_New.RESOURCE_CODE == resource_code
                     select Resources_Type_New);
            }
            resources_types_found = new List<SmartUtility.ResourcesTypes>(resources_types_found.Distinct());
            return resources_types_found;
        }

        internal static List<SmartUtility.Bills> Utility_Configure_Bills(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                string statement_id = "")
        {
            List<SmartUtility.Bills> bills_found = new List<SmartUtility.Bills>();
            // Note: This is ACROSS all Suppliers ... and all MPANs
            bills_found = new List<SmartUtility.Bills>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 join Bill in utilityviewmodel.Hezbollah.working_billsList
                 on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.ACCOUNT_CREATED, Account.ACCOUNT_NO }
                 equals new { Bill.USERNAME, Bill.CUBEFACE_CODE, Bill.ACCOUNT_CREATED, Bill.ACCOUNT_NO }
                 orderby Bill.BILL_PERIOD_START ascending
                 select Bill);
            bills_found = new List<SmartUtility.Bills>(bills_found.DistinctBy(key => new
            {
                key.USERNAME,
                key.CUBEFACE_CODE,
                key.SUPPLIER_CODE,
                key.BRAND_CODE,
                key.ACCOUNT_CREATED,
                key.ACCOUNT_NO,
                key.BILL_DATE,
                key.STATEMENT_ID
            }));
            if (!string.IsNullOrEmpty(statement_id))
            {
                bills_found = new List<SmartUtility.Bills>
                    (from BillsFound in bills_found
                     where (//(BillsFound.BILL_DATE == bill_date) &&
                             (BillsFound.STATEMENT_ID == statement_id))
                     orderby BillsFound.BILL_PERIOD_START ascending
                     select BillsFound);
            }
            else
            {
                if (utilityviewmodel.resource_code != SmartParametersV2016.DualFuel)
                {
                    bills_found = new List<SmartUtility.Bills>
                        (from BillsFound in bills_found
                         join BillsResource in utilityviewmodel.Hezbollah.bills_resourceList
                         on (BillsFound.USERNAME,
                             BillsFound.CUBEFACE_CODE,
                             BillsFound.SUPPLIER_CODE,
                             BillsFound.BRAND_CODE,
                             BillsFound.ACCOUNT_CREATED,
                             BillsFound.ACCOUNT_NO,
                             BillsFound.STATEMENT_ID,
                             BillsFound.BILL_DATE)
                         equals (BillsResource.USERNAME,
                             BillsResource.CUBEFACE_CODE,
                             BillsResource.SUPPLIER_CODE,
                             BillsResource.BRAND_CODE,
                             BillsResource.ACCOUNT_CREATED,
                             BillsResource.ACCOUNT_NO,
                             BillsResource.STATEMENT_ID,
                             BillsResource.BILL_DATE)
                         join Meters in utilityviewmodel.Hezbollah.utility_metersList
                         on (BillsResource.USERNAME, BillsResource.CUBEFACE_CODE, BillsResource.MPAN_MPRN)
                         equals (Meters.USERNAME, Meters.CUBEFACE_CODE, Meters.MPAN_MPRN)
#if WINFORMS
                         where (Meters.RESOURCE_CODE == utilityviewmodel.resource_code &&
                                BillsFound.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                                BillsFound.BILL_PERIOD_END <= utilityviewmodel.EndDate)
#endif
#if WPF  || WINUI || SMARTMAUI
                         where (Meters.RESOURCE_CODE == utilityviewmodel.resource_code &&
                                BillsFound.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                                BillsFound.BILL_PERIOD_END <= utilityviewmodel.EndDate)
#endif
#if ANDROIDX
                         where (Meters.RESOURCE_CODE == utilityviewmodel.resource_code &&
                                BillsFound.BILL_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                                BillsFound.BILL_PERIOD_END <= utilityviewmodel.EndDate.DateTime)
#endif
                         orderby BillsFound.BILL_PERIOD_START ascending
                         select BillsFound);
                }
            }
            return bills_found;
        }

        internal static List<SmartProfile.AddressesView> Utility_Analyze_Addresses(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {
            List<SmartProfile.AddressesView> addresses_found = new List<SmartProfile.AddressesView>();
            if (ourviewmodel.Hamas.consumersList.Count > 0) // Should one be one!!
            {
                addresses_found = new List<SmartProfile.AddressesView>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         //join Address in addressesList
                         //on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                         //equals new { Address.USERNAME, Address.CUBEFACE_CODE }
                     join Account in utilityviewmodel.Hezbollah.utility_accountsList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Account.USERNAME, Account.CUBEFACE_CODE }
                     join AddressView in ourviewmodel.SmartProfile.addressesviewList
                     on new { Account.USERNAME, Account.UDPRN }
                     equals new { AddressView.USERNAME, AddressView.UDPRN }
                     orderby AddressView.ADDRESS_CREATED descending
                     select AddressView);
                // UDPRN not guaranteed unique!
                addresses_found = new List<SmartProfile.AddressesView>
                (addresses_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.UDPRN       // !!!
                }));
                //addresses_found = new List<SmartUsers.Addresses>
                //    (from Address in addresses_found orderby
                //     Address.ADDRESS_CREATED descending
                //     select Address);
            }
            return addresses_found;
        }

        //    Its different for Finance.  Whereas with Utility you ALWAYS know the address
        //    ('cos a meter has to be located physically SOMEWHERE) that's not the case
        //    with Finance, 'cos you might not know the address until you decode a statement
        //    and you might not always have a statement to decode ....
        internal static List<SmartProfile.AddressesView> Utility_Configure_Addresses(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel)
        {
            // There IS A JOIN to Cubefaces here, because addresses
            // HERE are limited to the Utility Cubeface - in fact you WANT
            // addresses to be visible across ALL Cubefaces so that you
            // can link Finance AND Utility AND Insurance et. al.
            //
            // However HERE we just want the Addresses for which our
            // Accounts/Meters apply to (assuming of course that we can always
            // stick a UDPRN in!)
            //
            // NEW WAY OF DOING THINGS!!!  We are on the BRINK of joining
            // Finance to Utility (or Utility to Finance) so - yes - we
            // have a Join to CUBEFACES ... but that's ONLY TO GET AT ACCOUNTS!
            //                                         =======================
            // Once we have got an Account (which IS Cubeface dependent), we then
            // use its UDPRN to get an Address which ISN'T Cubeface dependent!!
            // So we can Join with Resources but not ResourceTypes (not needed)

            // Don't forget: UDPRNs do not have leading zeros!!!


            List<SmartProfile.AddressesView> addresses_found = new List<SmartProfile.AddressesView>();
            addresses_found = new List<SmartProfile.AddressesView>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 //join ResourceTypes in utilityviewmodel.Hezbollah.utility_resources_typesList
                 //on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 //equals new { ResourceTypes.USERNAME, ResourceTypes.CUBEFACE_CODE, ResourceTypes.RESOURCE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 //join Address in utilityviewmodel.Hezbollah.utility_addressesList
                 //on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.UDPRN }
                 //equals new { Address.USERNAME, Address.CUBEFACE_CODE, Address.UDPRN }
                 join AddressView in ourviewmodel.SmartProfile.addressesviewList
                 on new { Account.USERNAME, Account.UDPRN }
                 equals new { AddressView.USERNAME, AddressView.UDPRN }
                 orderby AddressView.ADDRESS_CREATED descending
                 select AddressView);
            // UDPRN not guaranteed unique!
            addresses_found = new List<SmartProfile.AddressesView>
            (addresses_found.DistinctBy(key => new
            {
                key.USERNAME,
                key.UDPRN       // !!!
            }));
            //addresses_found = new List<SmartUsers.Addresses>
            //    (from Address in addresses_found
            //     orderby Address.ADDRESS_CREATED descending
            //     select Address);
            return addresses_found;
        }

        internal static List<SmartUtility.BrandConnection> Utility_FindBrandConnections(MainViewModel ourviewmodel,
                                                                                    UtilityViewModel utilityviewmodel,
                                                                                    short supplier_code,
                                                                                    short brand_code)
        {
            List<SmartUtility.BrandConnection> brand_connections_found = new List<SmartUtility.BrandConnection>();

            if (utilityviewmodel.Hezbollah.brand_connectionList.Count > 0)
            {
                brand_connections_found = new List<SmartUtility.BrandConnection>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                    on new { Cubeface.CUBEFACE_CODE }
                    equals new { Resource.CUBEFACE_CODE }
                     join ResourceType in utilityviewmodel.Hezbollah.utility_resources_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE, ResourceType.RESOURCE_TYPE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE, SupplierType.RESOURCE_TYPE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }
                     join BrandConnection in utilityviewmodel.Hezbollah.brand_connectionList
                     on new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }
                     equals new { BrandConnection.CUBEFACE_CODE, BrandConnection.SUPPLIER_CODE }
                     where
                     (BrandConnection.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                     //BrandConnection.CATEGORY_CODE == utilityviewmodel.resource_code &&
                     BrandConnection.SUPPLIER_CODE == supplier_code &&
                     BrandConnection.BRAND_CODE == brand_code)
                     orderby BrandConnection.ORDINAL ascending
                     select BrandConnection);
            }
            brand_connections_found = new List<SmartUtility.BrandConnection>(brand_connections_found.Distinct());
            return brand_connections_found;
        }

        internal static List<SmartProfile.Profiles> Utility_Find_PersonalDetails(MainViewModel ourviewmodel)
                                                                        //char cubeface_code)
        {
            //List<SmartProfile.ProfilesView> profilesview_found = new List<SmartProfile.ProfilesView>();

            List<SmartProfile.Profiles> profiles_found =
                new List<SmartProfile.Profiles>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Profile in ourviewmodel.SmartProfile.profilesList
                 on new { Consumer.USERNAME }
                 equals new { Profile.USERNAME }
                 where Consumer.Include
                 select Profile).ToList();
            return profiles_found;
            //if (profiles_found.Count > 0)
            //{
            //    profilesview_found = new List<SmartProfile.ProfilesView>
            //        (from Profile in ourviewmodel.SmartProfile.profilesList
            //         join ProfilesCube in ourviewmodel.SmartProfile.profilescubeList
            //         on new { Profile.USERNAME, Profile.PROFILE_CREATED }
            //         equals new { ProfilesCube.USERNAME, ProfilesCube.PROFILE_CREATED }
            //         where ProfilesCube.CUBEFACE_CODE == cubeface_code
            //         select new SmartProfile.ProfilesView  // Can use 'new' because we specify the fields below
            //         {
            //             USERNAME = Profile.USERNAME,
            //             CUBEFACE_CODE = ProfilesCube.CUBEFACE_CODE,
            //             PROFILE_CREATED = ProfilesCube.PROFILE_CREATED,
            //             LASTNAME = Profile.LASTNAME,
            //             FIRSTNAME = Profile.FIRSTNAME,
            //             MIDDLENAME = Profile.MIDDLENAME,
            //             DATE_OF_BIRTH = Profile.DATE_OF_BIRTH,
            //             TITLE = Profile.TITLE,
            //             GENDER = Profile.GENDER,
            //             CONTACT_NO = Profile.CONTACT_NO,
            //             EMAIL_ADDRESS = Profile.EMAIL_ADDRESS,
            //             DISPLAY_NAME = Profile.DISPLAYNAME,
            //             NAME = ProfilesCube.NAME,
            //             DOB = ProfilesCube.DOB,
            //             PHONE_NO = ProfilesCube.PHONE_NO
            //         });
            //    profilesview_found = new List<SmartProfile.ProfilesView>
            //    (profilesview_found.DistinctBy(key => new
            //    {
            //        key.USERNAME,
            //        key.CUBEFACE_CODE,
            //        key.PROFILE_CREATED
            //    }
            //    ));
            //}
            //return profilesview_found;   // Maybe this gives more than one record?  Hence the Distinct above ..
        }

        internal static List<SmartUtility.Brands> Utility_Find_Supplier(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        char resource_code,
                                                        short supplier_code,
                                                        short brand_code)
        {
            List<SmartUtility.Brands> brands_found = new List<SmartUtility.Brands>();

            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                brands_found = new List<SmartUtility.Brands>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                    on new { Cubeface.CUBEFACE_CODE }
                    equals new { Resource.CUBEFACE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                     join Brand in utilityviewmodel.Hezbollah.brandsList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                     where Resource.RESOURCE_CODE == utilityviewmodel.resource_code &&
                             Supplier.SUPPLIER_CODE == supplier_code &&
                             Brand.BRAND_CODE == brand_code
                     select Brand);
            }
            else
            {
                brands_found = new List<SmartUtility.Brands>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Resource.CUBEFACE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                     join Brand in utilityviewmodel.Hezbollah.brandsList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                     where Resource.RESOURCE_CODE == resource_code &&
                             Supplier.SUPPLIER_CODE == supplier_code &&
                             Brand.BRAND_CODE == brand_code
                     select Brand);
            }
            brands_found = new List<SmartUtility.Brands>(brands_found.Distinct());    // Because get SR and VR for 'E'
            return brands_found;
            
        }

        internal static List<SmartUtility.Suppliers> Utility_Just_Lookup_Supplier(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        char resource_code,
                                                        short supplier_code)
        {
            List<SmartUtility.Suppliers> suppliers_found = new List<SmartUtility.Suppliers>();

            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                suppliers_found = new List<SmartUtility.Suppliers>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                    on new { Cubeface.CUBEFACE_CODE }
                    equals new { Resource.CUBEFACE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                     where Resource.RESOURCE_CODE == resource_code &&
                             Supplier.SUPPLIER_CODE == supplier_code
                     select Supplier);
            }
            else
            {
                suppliers_found = new List<SmartUtility.Suppliers>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                    on new { Cubeface.CUBEFACE_CODE }
                    equals new { Resource.CUBEFACE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                     where Resource.RESOURCE_CODE == resource_code &&
                             Supplier.SUPPLIER_CODE == supplier_code &&
                             SupplierType.RESOURCE_CODE == resource_code
                     select Supplier);
            }
            suppliers_found = new List<SmartUtility.Suppliers>(suppliers_found.Distinct());    // Because get SR and VR for 'E'
            return suppliers_found;
        }

        internal static bool Utility_Lookup_ResourceName(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        string resource_description)
        {
            // Only if its not empty
            if (utilityviewmodel.resource_code != SmartParametersV2016.defaultChar)
            {
                if (!string.IsNullOrEmpty(resource_description))
                {
                    string upper = resource_description.Substring(0, 1).ToUpper();
                    // Look for the description OR the description with an uppercased first letter
                    List<SmartUtility.ResourceCodes> resource_codes_found =
                        new List<SmartUtility.ResourceCodes>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.resource_codesList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Resource.CUBEFACE_CODE }
                         where Resource.RESOURCE_CODE == utilityviewmodel.resource_code &&
                                (Resource.DESCRIPTION == resource_description ||
                                    Resource.DESCRIPTION == upper + resource_description.Substring(1))
                         select Resource);
                    resource_codes_found = new List<SmartUtility.ResourceCodes>(resource_codes_found.Distinct());
                    if (resource_codes_found.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool Utility_Lookup_ResourceDescription(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        {
            // Only if its not empty
            if (utilityviewmodel.resource_code != SmartParametersV2016.defaultChar)
            {
                // Look for the description 
                List<SmartUtility.ResourceCodes> resource_codes_found =
                    new List<SmartUtility.ResourceCodes>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.resource_codesList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Resource.CUBEFACE_CODE }
                     where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                     select Resource);
                resource_codes_found = new List<SmartUtility.ResourceCodes>(resource_codes_found.Distinct());
                if (resource_codes_found.Count > 0)
                {
                    utilityviewmodel.resource_description = resource_codes_found.First().DESCRIPTION;
                    return true;
                }
                utilityviewmodel.resource_description = "";
            }
            return false;
        }

        internal static bool Utility_Lookup_ResourceType(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                string resource_type_description)
        {
            // Only if resource_code not empty - description CAN be empty!
            if (resource_code != SmartParametersV2016.defaultChar)
            {
                List<SmartUtility.ResourceTypes> resource_types_found = new List<SmartUtility.ResourceTypes>();
                if (!string.IsNullOrEmpty(resource_type_description))
                {
                    // Look for the description if its been given
                    resource_types_found =
                        new List<SmartUtility.ResourceTypes>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.resource_codesList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Resource.CUBEFACE_CODE }
                         join ResourceType in utilityviewmodel.Hezbollah.resource_typesList
                         on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE }
                         where Resource.RESOURCE_CODE == resource_code &&
                                 ResourceType.DESCRIPTION == resource_type_description
                         select ResourceType);
                }
                else
                {
                    // Otherwise look for the FIRST in the list which SHOULD come out in ascending ORDINAL order
                    // i.e. we get back SR before VR !!!
                    resource_types_found =
                        new List<SmartUtility.ResourceTypes>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.resource_codesList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Resource.CUBEFACE_CODE }
                         join ResourceType in utilityviewmodel.Hezbollah.resource_typesList
                         on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE }
                         where Resource.RESOURCE_CODE == resource_code
                         select ResourceType);
                }
                resource_types_found = new List<SmartUtility.ResourceTypes>(resource_types_found.Distinct());

                if (resource_types_found.Count > 0)
                {
                    // Should only be one or two, in any case
                    utilityviewmodel.np_resource_type = resource_types_found.First().RESOURCE_TYPE;
                    return true;
                }
            }
            return false;
        }

        internal static bool Utility_Lookup_TariffCode(UtilityViewModel utilityviewmodel,
                                            short brand_code,
                                            char resource_code,
                                            string resource_type)
        {
            // This is boollocks Laurence Slade Energy UK
            // 1st Approach - look for what we've found in the List
            //
            // Only if its not empty and we have something in the list
            if (!string.IsNullOrEmpty(utilityviewmodel.TARIFF_NAME) &&
                (utilityviewmodel.tariff_codes_names_subsetList.Count > 0))
            {
                string temp_name = utilityviewmodel.TARIFF_NAME;
                List<SmartUtility.TariffCodesNames> tariff_codes_names_found =
                    new List<SmartUtility.TariffCodesNames>
                    (from TCN in utilityviewmodel.tariff_codes_names_subsetList
                     where TCN.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                     TCN.RESOURCE_CODE == resource_code &&
                     TCN.RESOURCE_TYPE == resource_type &&
                     TCN.BRAND_CODE == brand_code &&
                     (TCN.TARIFF_NAME == temp_name ||
                     TCN.TARIFF_PREVIOUS_NAME == temp_name)
                     select TCN);

                // Should be quicker (and more accurate!) than the old version?
                if (tariff_codes_names_found.Count == 0)
                {
                    // Try the compressed version
                    tariff_codes_names_found =
                        new List<SmartUtility.TariffCodesNames>
                        (from TCN in utilityviewmodel.tariff_codes_names_subsetList
                         where TCN.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                             TCN.RESOURCE_CODE == resource_code &&
                             TCN.RESOURCE_TYPE == resource_type &&
                             TCN.BRAND_CODE == brand_code &&
                             (TCN.TARIFF_NAME.Replace(SmartParametersV2016.space, "") == temp_name ||
                             TCN.TARIFF_PREVIOUS_NAME.Replace(SmartParametersV2016.space, "") == temp_name)
                         select TCN);
                    if (tariff_codes_names_found.Count == 0)
                    {
                        // Try the compressed + compressed version
                        temp_name = temp_name.Replace(SmartParametersV2016.space, "");
                        tariff_codes_names_found =
                            new List<SmartUtility.TariffCodesNames>
                            (from TCN in utilityviewmodel.tariff_codes_names_subsetList
                             where TCN.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                                 TCN.RESOURCE_CODE == resource_code &&
                                 TCN.RESOURCE_TYPE == resource_type &&
                                 TCN.BRAND_CODE == brand_code &&
                                 (TCN.TARIFF_NAME.Replace(SmartParametersV2016.space, "") == temp_name ||
                                 TCN.TARIFF_PREVIOUS_NAME.Replace(SmartParametersV2016.space, "") == temp_name)
                             select TCN);
                    }
                }
                if (tariff_codes_names_found.Count > 0)
                {
                    FieldInfo[] myFields = typeof(SmartUtility.TariffCodesNames).GetFields(SmartParametersV2016.bindingFlags);
                    int brand_field_index = SmartNibbyV2016.Area_Matrix_Index(utilityviewmodel.area_code, myFields);
                    if (brand_field_index >= 0)
                    {
                        foreach (SmartUtility.TariffCodesNames tariff_codes_names_row in tariff_codes_names_found)
                        {
                            if (Convert.ToChar(myFields[brand_field_index].GetValue(tariff_codes_names_row).ToString()) == SmartParametersV2016.brandMatrixValid)
                            {
                                utilityviewmodel.TARIFF_NAME = tariff_codes_names_row.TARIFF_NAME;
                                utilityviewmodel.TARIFF_CODE = Convert.ToInt32(tariff_codes_names_row.TARIFF_CODE); // Only the first   
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal static bool Utility_Lookup_PaymentCode(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                string methodx)
        {
            // Only if its not empty and we have something in the list
            if ((!string.IsNullOrEmpty(methodx)) &&
                (utilityviewmodel.Hezbollah.payment_methodsList.Count > 0))
            {
                string temp_method = methodx;
                List<SmartUtility.PaymentMethods> payment_methods_found =
                    new List<SmartUtility.PaymentMethods>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Payment_Method in utilityviewmodel.Hezbollah.payment_methodsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Payment_Method.CUBEFACE_CODE }
                 where Payment_Method.PAYMENT_METHOD1.Contains(temp_method) ||
                         Payment_Method.PAYMENT_METHOD2.Contains(temp_method)
                 select Payment_Method);
                payment_methods_found = new List<SmartUtility.PaymentMethods>(payment_methods_found.Distinct());
                if (payment_methods_found.Count > 0)
                {
                    foreach (SmartUtility.PaymentMethods payment_methods_row in payment_methods_found)
                    {
                        utilityviewmodel.PAYMENT_CODE = payment_methods_row.PAYMENT_CODE.ToString(); // Only first
                        return true;
                    }
                }
                else
                {
                    string payment_method1 = "",
                            payment_method2 = "";
                    // ! Where we run the compressed Payment Plans through the string!
                    foreach (SmartUtility.PaymentMethods payment_methods_row in utilityviewmodel.Hezbollah.payment_methodsList)
                    {
                        payment_method1 = payment_methods_row.PAYMENT_METHOD1.Replace(SmartParametersV2016.space, "");
                        if (methodx.IndexOf(payment_method1) >= 0)
                        {
                            utilityviewmodel.PAYMENT_METHOD = payment_methods_row.PAYMENT_METHOD1;
                            utilityviewmodel.PAYMENT_CODE = payment_methods_row.PAYMENT_CODE.ToString();
                            return true;
                        }
                        else
                        {
                            payment_method2 = payment_methods_row.PAYMENT_METHOD2.Replace(SmartParametersV2016.space, "");
                            if (!string.IsNullOrEmpty(payment_method2))
                            {
                                if (methodx.IndexOf(payment_method2) >= 0)
                                {
                                    utilityviewmodel.PAYMENT_METHOD = payment_methods_row.PAYMENT_METHOD2;
                                    utilityviewmodel.PAYMENT_CODE = payment_methods_row.PAYMENT_CODE.ToString();
                                    return true;
                                }
                            }
                            else
                            {
                                if (methodx.ToUpper().IndexOf(payment_methods_row.PAYMENT_METHOD1.ToUpper()) >= 0)
                                {
                                    utilityviewmodel.PAYMENT_METHOD = payment_methods_row.PAYMENT_METHOD1;
                                    utilityviewmodel.PAYMENT_CODE = payment_methods_row.PAYMENT_CODE.ToString();
                                    return true;
                                }
                            }
                        }
                    }   // Give up! Really!
                }
            }
            utilityviewmodel.errorMessage = "Lookup_Payment_Code: " + methodx + " not found";
            return false;
        }

        internal static bool Utility_Lookup_PaymentMethod(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    short payment_code)
        {
            // Only if its not 0 and we have something in the list
            if (utilityviewmodel.Hezbollah.payment_methodsList.Count > 0)
            {
                List<SmartUtility.PaymentMethods> payment_methods_found =
                    new List<SmartUtility.PaymentMethods>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Payment_Method in utilityviewmodel.Hezbollah.payment_methodsList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Payment_Method.CUBEFACE_CODE }
                     where Payment_Method.PAYMENT_CODE == payment_code
                     select Payment_Method);
                payment_methods_found = new List<SmartUtility.PaymentMethods>(payment_methods_found.Distinct());
                if (payment_methods_found.Count > 0)
                {
                    foreach (SmartUtility.PaymentMethods payment_methods_row in payment_methods_found)
                    {
                        utilityviewmodel.payment_method = payment_methods_row.PAYMENT_METHOD1; // Only first
                        return true;
                    }
                }
            }
            utilityviewmodel.errorMessage = "Lookup_Payment_Method: " + payment_code + " not found";
            return false;
        }

        //
        // This looks like an IMPORTANT routine ... but I cant think of anywhere to use it!!
        // And I think this is the only place I use "LOOKUP_CONDITIONS_PLAN" !!!!
        //
        internal static async Task<char> Utility_Lookup_RemotePaymentPlan(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short brand_code,
                                                                short supplier_code,
                                                                char resource_code,
                                                                string resource_type,
                                                                int tariff_code)
        {
            char payment_plan = SmartParametersV2016.defaultResourceCode; // We expect the worst

            // Go and look it up from HQ - use SUPPLIER **not** BRAND!!
            string P1 = utilityviewmodel.cubeface_code.ToString() + SmartParametersV2016.unitSeparator +
                        supplier_code.ToString() + SmartParametersV2016.unitSeparator +
                        resource_code.ToString() + SmartParametersV2016.unitSeparator +
                        resource_type + SmartParametersV2016.unitSeparator +
                        tariff_code.ToString();

            // The GUI should **NEVER** have to worry about the Schema - SmartDBServer should sort that out
            List<SmartUtility.ConditionsPlansView> conditions_plans_viewList =
                await SmartBobV2017.Load_SingleList_Async<SmartUtility.ConditionsPlansView>(ourviewmodel,
                                                                        utilityviewmodel,
                                                                        utilityviewmodel.utilityToken,
                                                                        "SMARTUTILITY",
                                                                        "LOOKUP_CONDITIONS_PLANS",
                                                                        "P",
                                                                        P1);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return SmartParametersV2016.defaultChar;
            }
            else
            {
                if (conditions_plans_viewList.Count > 0)
                {
                    foreach (SmartUtility.ConditionsPlansView conditions_plans_view_row in conditions_plans_viewList)
                    {
                        payment_plan = Convert.ToChar(conditions_plans_view_row.PAYMENT_PLANS);
                        break;  // Only the first                 
                    }
                }
                else
                {
                    // One of the very few occasions when we need to tell HQ something -
                    // (make sure NONE of these have embedded commas, cos this will fuck up SmartDbserver!
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, utilityviewmodel.utilityToken, supplier_code, brand_code, utilityviewmodel.cubeface_code.ToString() + " Cannot find Payment Plan"))
                    {
                        return SmartParametersV2016.defaultChar;
                    }
                }
            }
            return payment_plan;
        }

        internal static bool Utility_Lookup_PaymentPlan(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    string payment_name)
        {
            // Only if its not empty and we have something in the list
            if ((!string.IsNullOrEmpty(payment_name)) &&
                (utilityviewmodel.Hezbollah.payment_plansList.Count > 0))
            {
                utilityviewmodel.temp_payment_name = payment_name;
                List<SmartUtility.PaymentPlans> payment_plans_found =
                    new List<SmartUtility.PaymentPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Payment_Plan in utilityviewmodel.Hezbollah.payment_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Payment_Plan.CUBEFACE_CODE }
                     where Payment_Plan.PAYMENT_NAME == utilityviewmodel.temp_payment_name ||
                            Payment_Plan.ALTERNATE_NAME1 == utilityviewmodel.temp_payment_name ||
                            Payment_Plan.ALTERNATE_NAME2 == utilityviewmodel.temp_payment_name  // 3 Only applies to SP Analyze (hey that rhymes!)
                     select Payment_Plan);
                payment_plans_found = new List<SmartUtility.PaymentPlans>(payment_plans_found.Distinct());
                if (payment_plans_found.Count > 0)
                {
                    foreach (SmartUtility.PaymentPlans payment_plans_row in payment_plans_found)
                    {
                        utilityviewmodel.PAYMENT_PLAN = payment_plans_row.PAYMENT_PLAN; // Only first AAA or BBB or whatever
                        return true;
                    }
                }
                else
                {
                    // Now try and find if its anywhere IN the table
                    utilityviewmodel.temp_payment_name = payment_name;
                    payment_plans_found = new List<SmartUtility.PaymentPlans>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Payment_Plan in utilityviewmodel.Hezbollah.payment_plansList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Payment_Plan.CUBEFACE_CODE }
                         where Payment_Plan.PAYMENT_NAME.Contains(utilityviewmodel.temp_payment_name) ||
                                Payment_Plan.ALTERNATE_NAME1.Contains(utilityviewmodel.temp_payment_name) ||
                                Payment_Plan.ALTERNATE_NAME2.Contains(utilityviewmodel.temp_payment_name)  // 3 Only applies to SP Analyze (hey that rhymes!)
                         select Payment_Plan);
                    payment_plans_found = new List<SmartUtility.PaymentPlans>(payment_plans_found.Distinct());
                    if (payment_plans_found.Count > 0)
                    {
                        foreach (SmartUtility.PaymentPlans payment_plans_row in payment_plans_found)
                        {
                            utilityviewmodel.PAYMENT_PLAN = payment_plans_row.PAYMENT_PLAN; // Only first AAA or BBB or whatever
                            return true;
                        }
                    }
                    else
                    {
                        // ! Where we run the compressed Payment Plans through the string!
                        foreach (SmartUtility.PaymentPlans payment_plans_row in utilityviewmodel.Hezbollah.payment_plansList)
                        {
                            if (Check_Payment_Plan(utilityviewmodel,
                                                    //rf temp_payment_name, 
                                                    payment_plans_row.PAYMENT_NAME))
                            {
                                payment_name = utilityviewmodel.temp_payment_name;
                                utilityviewmodel.PAYMENT_PLAN = payment_plans_row.PAYMENT_PLAN;
                                return true;
                            }
                            if (Check_Payment_Plan(utilityviewmodel,
                                                    //rf temp_payment_name, 
                                                    payment_plans_row.ALTERNATE_NAME1))
                            {
                                payment_name = utilityviewmodel.temp_payment_name;
                                utilityviewmodel.PAYMENT_PLAN = payment_plans_row.PAYMENT_PLAN;
                                return true;
                            }
                            if (Check_Payment_Plan(utilityviewmodel,
                                                    //rf temp_payment_name, 
                                                    payment_plans_row.ALTERNATE_NAME2))
                            {
                                payment_name = utilityviewmodel.temp_payment_name;
                                utilityviewmodel.PAYMENT_PLAN = payment_plans_row.PAYMENT_PLAN;
                                return true;
                            }
                            // Don't do ALTERNATE_NAME3 its only for SP_Analyze
                        }
                    }
                    // Give up! Really!
                }
            }
            utilityviewmodel.errorMessage = "Cannot resolve " + payment_name;
            return false;
        }

        internal static string[] Utility_Lookup_PaymentName(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    char payment_plan)
        {
            string[] names = new string[3] { "", String.Empty, "" };
            // Only if its not empty and we have something in the list
            if (!string.IsNullOrEmpty(payment_plan.ToString()) &&
                (utilityviewmodel.Hezbollah.payment_plansList.Count > 0))
            {
                List<SmartUtility.PaymentPlans> payment_plans_found =
                    new List<SmartUtility.PaymentPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Payment_Plan in utilityviewmodel.Hezbollah.payment_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Payment_Plan.CUBEFACE_CODE }
                     where Payment_Plan.PAYMENT_PLAN == payment_plan
                     select Payment_Plan);
                payment_plans_found = new List<SmartUtility.PaymentPlans>(payment_plans_found.Distinct());
                if (payment_plans_found.Count > 0)
                {
                    foreach (SmartUtility.PaymentPlans payment_plans_row in payment_plans_found)
                    {
                        names[0] = payment_plans_row.PAYMENT_NAME;
                        names[1] = payment_plans_row.ALTERNATE_NAME1;
                        names[2] = payment_plans_row.ALTERNATE_NAME2;
                        return names;
                    }
                }
            }
            return names;
        }

        internal static bool Check_Payment_Plan(UtilityViewModel utilityviewmodel,
                                            //rf string payment_name, 
                                            string PAYMENT_NAME)
        {
            string table_payment_name = PAYMENT_NAME.Replace(SmartParametersV2016.space, "");
            utilityviewmodel.temp_payment_name = utilityviewmodel.temp_payment_name.Replace(SmartParametersV2016.space, "");
            if (!string.IsNullOrEmpty(table_payment_name))
            {
                if (table_payment_name.IndexOf(utilityviewmodel.temp_payment_name) >= 0)
                {
                    utilityviewmodel.temp_payment_name = PAYMENT_NAME;
                    return true;
                }
                else
                {
                    if (table_payment_name.ToUpper().IndexOf(utilityviewmodel.temp_payment_name.ToUpper()) >= 0)
                    {
                        utilityviewmodel.temp_payment_name = PAYMENT_NAME;
                        return true;
                    }
                    else
                    {
                        if (utilityviewmodel.temp_payment_name.IndexOf(table_payment_name) >= 0)
                        {
                            utilityviewmodel.temp_payment_name = PAYMENT_NAME;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal static List<SmartUtility.BrandsView> Utility_Determine_Brands(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            bool wildcard,
                                                            char resource_code,
                                                            short comparison_supplier_code,
                                                            DateTime withdrawn_date)
        {
            // Its not often we cross SmartSwitch with SmartUtility but when we do ...
            // ... we need to take into account our fucking USERNAME !!!!
            List<SmartUtility.BrandsView> brandsview_found = new List<SmartUtility.BrandsView>();
            if (wildcard)
            {
                brandsview_found =
                    new List<SmartUtility.BrandsView>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Resource.CUBEFACE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                     join Brand in utilityviewmodel.Hezbollah.brandsList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                     where (Resource.RESOURCE_CODE == resource_code) &&
                            (Supplier.VALID_TO >= withdrawn_date)
                     select new SmartUtility.BrandsView
                     {
                         CUBEFACE_CODE = Brand.CUBEFACE_CODE,
                         RESOURCE_CODE = SupplierType.RESOURCE_CODE,
                         RESOURCE_TYPE = SupplierType.RESOURCE_TYPE,
                         SUPPLIER_CODE = Brand.SUPPLIER_CODE,
                         BRAND_CODE = Brand.BRAND_CODE,
                         BRAND_NAME = Brand.BRAND_NAME
                     });
            }
            else
            {
                brandsview_found =
                    new List<SmartUtility.BrandsView>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Resource.CUBEFACE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                     join Brand in utilityviewmodel.Hezbollah.brandsList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                     where (Resource.RESOURCE_CODE == resource_code) &&
                            (Supplier.SUPPLIER_CODE == comparison_supplier_code) &&
                            (Supplier.VALID_TO >= withdrawn_date)
                     select new SmartUtility.BrandsView
                     {
                         CUBEFACE_CODE = Brand.CUBEFACE_CODE,
                         RESOURCE_CODE = SupplierType.RESOURCE_CODE,
                         RESOURCE_TYPE = SupplierType.RESOURCE_TYPE,
                         SUPPLIER_CODE = Brand.SUPPLIER_CODE,
                         BRAND_CODE = Brand.BRAND_CODE,
                         BRAND_NAME = Brand.BRAND_NAME
                     });
            }
            brandsview_found = new List<SmartUtility.BrandsView>(brandsview_found.DistinctBy(key => new
            {
                key.CUBEFACE_CODE,
                key.RESOURCE_CODE,
                key.RESOURCE_TYPE,
                key.SUPPLIER_CODE,
                key.BRAND_CODE,
                key.BRAND_NAME
            }
            ));
            return brandsview_found;
        }

        internal static List<SmartUtility.Brands> Utility_Lookup_Brands(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        char resource_code,
                                                        string resource_type,
                                                        char active_flag,       // Either 'Y' for scrapeables or ALL of them
                                                        short supplier_code,
                                                        short brand_code)
        {
            // ACTIVE_CODE ... is for those Brands you can S C R A P E you dickhead
            // These are the ones you want to see in your Supplier dropdown!
            List<SmartUtility.Brands> brands_found = new List<SmartUtility.Brands>();

            if (supplier_code == 0 &&
                    brand_code == 0)
            {
                if (resource_code == SmartParametersV2016.defaultResourceCode)
                {
                    if (active_flag != SmartParametersV2016.activeFlag)
                    {
                        brands_found = new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where SupplierType.RESOURCE_TYPE == resource_type
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                        brands_found = new List<SmartUtility.Brands>
                            (brands_found.DistinctBy(key => new
                            {
                                key.CUBEFACE_CODE,
                                key.SUPPLIER_CODE,
                                key.BRAND_CODE
                            }
                            ));
                    }
                    else
                    {
                        brands_found =
                            new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where Supplier.ACTIVE_FLAG == active_flag &&
                                    SupplierType.RESOURCE_TYPE == resource_type
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                        brands_found = new List<SmartUtility.Brands>
                            (brands_found.DistinctBy(key => new
                            {
                                key.CUBEFACE_CODE,
                                key.SUPPLIER_CODE,
                                key.BRAND_CODE
                            }
                            ));
                    }
                }
                else
                {
                    if (active_flag != SmartParametersV2016.activeFlag)
                    {
                        brands_found =
                            new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where SupplierType.RESOURCE_CODE == resource_code &&
                                     SupplierType.RESOURCE_TYPE == resource_type
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                    }
                    else
                    {
                        brands_found =
                            new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where SupplierType.RESOURCE_CODE == resource_code &&
                                     SupplierType.RESOURCE_TYPE == resource_type &&
                                     Supplier.ACTIVE_FLAG == active_flag
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                    }
                }
            }
            else
            {
                if (supplier_code == 0 &&
                    (brand_code > 0))
                {
                    if (active_flag != SmartParametersV2016.activeFlag)
                    {
                        brands_found =
                            new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where (SupplierType.RESOURCE_CODE == resource_code &&
                                     SupplierType.RESOURCE_TYPE == resource_type &&
                                     Brand.BRAND_CODE == brand_code)
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                    }
                    else
                    {
                        brands_found =
                            new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where (Supplier.ACTIVE_FLAG == active_flag &&
                                     SupplierType.RESOURCE_CODE == resource_code &&
                                     SupplierType.RESOURCE_TYPE == resource_type &&
                                     Brand.BRAND_CODE == brand_code)
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                    }
                }
                else
                {
                    if (active_flag != SmartParametersV2016.activeFlag)
                    {
                        brands_found =
                            new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where SupplierType.RESOURCE_CODE == resource_code &&
                                     SupplierType.RESOURCE_TYPE == resource_type &&
                                     Supplier.SUPPLIER_CODE == supplier_code &&
                                     Brand.BRAND_CODE == brand_code
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                    }
                    else
                    {
                        brands_found =
                            new List<SmartUtility.Brands>
                            (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                             join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                             on new { Cubeface.CUBEFACE_CODE }
                             equals new { Resource.CUBEFACE_CODE }
                             join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                             on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                             equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                             join Supplier in utilityviewmodel.Hezbollah.suppliersList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                             join Brand in utilityviewmodel.Hezbollah.brandsList
                             on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                             equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                             where (Supplier.ACTIVE_FLAG == active_flag &&
                                     SupplierType.RESOURCE_CODE == resource_code &&
                                     SupplierType.RESOURCE_TYPE == resource_type &&
                                     Supplier.SUPPLIER_CODE == supplier_code &&
                                     Brand.BRAND_CODE == brand_code)
                             orderby Brand.BRAND_NAME ascending
                             select Brand);
                    }
                }
            }
            brands_found = new List<SmartUtility.Brands>(brands_found.Distinct());
            return brands_found;
        }

        internal static List<SmartUtility.Brands> Utility_Lookup_BrandName(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            char resource_code,
                                                            string brand_name)
        {
            List<SmartUtility.Brands> brands_found = new List<SmartUtility.Brands>();

            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                if (!string.IsNullOrEmpty(brand_name))
                {
                    brands_found =
                        new List<SmartUtility.Brands>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Resource.CUBEFACE_CODE }
                         join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                         on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                         join Supplier in utilityviewmodel.Hezbollah.suppliersList
                         on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                         equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                         join Brand in utilityviewmodel.Hezbollah.brandsList
                         on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                         equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                         where Brand.BRAND_NAME == brand_name
                         select Brand);
                }
                else
                {
                    // For when we are CANCEL (ing) and we have the BRAND_CODE
                    // from Consumer_Energy but no SUPPLIER_CODE and no BRAND_NAME
                    brands_found =
                        new List<SmartUtility.Brands>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Resource.CUBEFACE_CODE }
                         join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                         on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                         join Supplier in utilityviewmodel.Hezbollah.suppliersList
                         on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                         equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                         join Brand in utilityviewmodel.Hezbollah.brandsList
                         on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                         equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                         select Brand);
                }
            }
            else
            {
                brands_found =
                    new List<SmartUtility.Brands>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Resource.CUBEFACE_CODE }
                     join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                     on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                     join Supplier in utilityviewmodel.Hezbollah.suppliersList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                     join Brand in utilityviewmodel.Hezbollah.brandsList
                     on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                     equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                     where Brand.BRAND_NAME == brand_name &&
                            SupplierType.RESOURCE_CODE == resource_code
                     select Brand);
            }
            brands_found =
                new List<SmartUtility.Brands>
                (brands_found.Distinct());    // Because get SR and VR for 'E'
            return brands_found;
        }

        internal static string Utility_Lookup_SupplierName(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            short supplier_code,
                                                            short brand_code)
        {
            if (utilityviewmodel.analysisbrandsviewList.Count == 0)
            {
                if ((supplier_code > 0) &&
                (brand_code > 0) &&
                (utilityviewmodel.Hezbollah.suppliersList.Count > 0) &&
                (utilityviewmodel.Hezbollah.brandsList.Count > 0))

                {
                    utilityviewmodel.analysisbrandsviewList =
                        new List<SmartUtility.AnalysisBrandsView>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Resource.CUBEFACE_CODE }
                         join ResourceType in utilityviewmodel.Hezbollah.utility_resources_typesList
                         on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE }
                         join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                         on new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE, ResourceType.RESOURCE_TYPE }
                         equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE, SupplierType.RESOURCE_TYPE }
                         join Supplier in utilityviewmodel.Hezbollah.suppliersList
                         on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                         equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                         join Brand in utilityviewmodel.Hezbollah.brandsList
                         on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                         equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }

                         select new SmartUtility.AnalysisBrandsView // Can use 'new' because we specify the fields below
                         {
                             CUBEFACE_CODE = Cubeface.CUBEFACE_CODE,
                             RESOURCE_CODE = Resource.RESOURCE_CODE,
                             RESOURCE_TYPE = ResourceType.RESOURCE_TYPE,
                             SUPPLIER_CODE = Supplier.SUPPLIER_CODE,
                             BRAND_CODE = Brand.BRAND_CODE,
                             BRAND_NAME = Brand.BRAND_NAME
                         });
                    utilityviewmodel.analysisbrandsviewList =
                            new List<SmartUtility.AnalysisBrandsView>(utilityviewmodel.analysisbrandsviewList.Distinct());
                }
            }
            if ((supplier_code > 0) &&
                    (brand_code > 0))
            {
                // Form collection
                List<string> brands_found = new List<string>
                    (from BrandsView in utilityviewmodel.analysisbrandsviewList
                     where (BrandsView.SUPPLIER_CODE == supplier_code &&
                             BrandsView.BRAND_CODE == brand_code)
                     select BrandsView.BRAND_NAME);
                // Test collection
                if (brands_found.Count > 0)
                {
                    return brands_found.First(); // Which will be a BRAND_NAME
                }
            }
            return "";
        }
        // Lookup_Tariff_Name moved to Spike

        internal static string Utility_Lookup_ResourceTypeDescription(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            char resource_code,
                                                            string resource_type)
        {
            // Only if its not empty and there is something in the table
            if ((!string.IsNullOrEmpty(resource_type)) &&
                (utilityviewmodel.Hezbollah.resource_typesList.Count > 0))
            {
                if (resource_code != SmartParametersV2016.defaultChar)
                {
                    // Look for ones that aren't deleted
                    // Form collection
                    List<SmartUtility.ResourceTypes>
                        resource_types_found =
                        new List<SmartUtility.ResourceTypes>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.resource_codesList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Resource.CUBEFACE_CODE }
                         join ResourceType in utilityviewmodel.Hezbollah.resource_typesList
                         on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE }
                         where Resource.RESOURCE_CODE == resource_code &&
                                ResourceType.RESOURCE_TYPE == resource_type
                         select ResourceType);
                    resource_types_found = new List<SmartUtility.ResourceTypes>(resource_types_found.Distinct());
                    // Test collection
                    if (resource_types_found.Count > 0)
                    {
                        return resource_types_found.First().DESCRIPTION;
                    }
                }
            }
            return "";
        }

        internal static List<SmartUtility.Logins> Utility_Lookup_Logins(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        short supplier_code,
                                                        short brand_code)

        {
            List<SmartUtility.Logins> logins_found = new List<SmartUtility.Logins>();
            if (utilityviewmodel.resource_code != SmartParametersV2016.defaultResourceCode)
            {
                logins_found = new List<SmartUtility.Logins>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Logins in utilityviewmodel.Hezbollah.utility_loginsList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Logins.USERNAME, Logins.CUBEFACE_CODE }
                     where Logins.SUPPLIER_CODE == supplier_code &&
                             Logins.BRAND_CODE == brand_code // &&
                                                             //Logins.LOGIN_CREATED != SmartParametersV2016.defaultDate)
                     orderby Logins.LOGIN_CREATED descending
                     select Logins);
                logins_found = new List<SmartUtility.Logins>(logins_found.Distinct());
            }
            return logins_found;
        }

        internal static List<SmartUtility.Accounts> Utility_Lookup_Accounts(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        DateTime account_created,
                                                        short supplier_code = 0,
                                                        short brand_code = 0,
                                                        string account_no = "")
        {
            List<SmartUtility.Accounts> accounts_found = new List<SmartUtility.Accounts>();

            if (supplier_code == 0 ||
                brand_code == 0 ||
                account_created == SmartParametersV2016.defaultDate ||
                string.IsNullOrEmpty(account_no))
            {
                // This works because RESOURCE_TYPE in ResourceTypes is *not* a key
                // For ResourceTypes you can’t have
                // RAY U   E SR
                // RAY U   E VR
                // Because SR and VR are mutually exclusive (in reality, you only
                // have one or the other!) And Gas only ever has SR
                // RAY U   G SR
                // Because there are no variable Tariffs on Gas.

                accounts_found = new List<SmartUtility.Accounts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 select Account);
                // First time through, resource_code won't be set to anything ...
                if (utilityviewmodel.resource_code != SmartParametersV2016.defaultResourceCode &&
                    utilityviewmodel.resource_code != 'D')
                {
                    // Reduce accounts to 'E' or 'G'
                    accounts_found = new List<SmartUtility.Accounts>
                                    (from Accounts in accounts_found
                                     where Accounts.RESOURCE_CODE == utilityviewmodel.resource_code
                                     select Accounts);
                }
            }
            else
            {
                accounts_found = new List<SmartUtility.Accounts>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                     join Account in utilityviewmodel.Hezbollah.utility_accountsList
                     on new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                     equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                     where Resource.RESOURCE_CODE == utilityviewmodel.resource_code &&
                            Account.SUPPLIER_CODE == supplier_code &&
                            Account.BRAND_CODE == brand_code &&
                            Account.ACCOUNT_NO == account_no &&
                            Account.ACCOUNT_CREATED == account_created
                     select Account);
            }
            accounts_found = new List<SmartUtility.Accounts>(accounts_found.Distinct());
            return accounts_found;
        }

        internal static List<SmartUtility.Meters> Utility_Lookup_Meters(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        char resource_code,
                                                        //string resource_type,
                                                        string mpan_mprn)
        {
            List<SmartUtility.Meters> meters_found =
                    new List<SmartUtility.Meters>();
            if (!string.IsNullOrEmpty(mpan_mprn))
            {
                meters_found =
                    new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE, ResourcesType.RESOURCE_TYPE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE, Meter.RESOURCE_TYPE }
                     where Resource.RESOURCE_CODE == resource_code &&
                           //ResourcesType.RESOURCE_TYPE == resource_type &&
                           Meter.MPAN_MPRN == mpan_mprn
                     select Meter);
            }
            else
            {
                meters_found =
                    new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join ResourcesType in utilityviewmodel.Hezbollah.utility_resources_typesList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE }
                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { ResourcesType.USERNAME, ResourcesType.CUBEFACE_CODE, ResourcesType.RESOURCE_CODE, ResourcesType.RESOURCE_TYPE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE, Meter.RESOURCE_TYPE }
                     where Resource.RESOURCE_CODE == resource_code
                     //ResourcesType.RESOURCE_TYPE == resource_type
                     select Meter);
            }
            meters_found = new List<SmartUtility.Meters>(meters_found.Distinct());
            return meters_found;
        }

        internal static List<SmartUtility.Switches> Utility_Lookup_Switches(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        short supplier_code,
                                                        short brand_code)
        //string account_no,
        //string mpan_mprn)
        {
            List<SmartUtility.Switches> switches_found =
                new List<SmartUtility.Switches>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 join Switch in utilityviewmodel.Hezbollah.utility_switchesList
                 on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE, Account.SUPPLIER_CODE, Account.BRAND_CODE, Account.ACCOUNT_NO }
                 equals new { Switch.USERNAME, Switch.CUBEFACE_CODE, Switch.RESOURCE_CODE, Switch.SUPPLIER_CODE, Switch.BRAND_CODE, Switch.ACCOUNT_NO }
                 where Resource.RESOURCE_CODE == utilityviewmodel.resource_code &&
                     Account.SUPPLIER_CODE == supplier_code &&
                     Account.BRAND_CODE == brand_code
                 //Account.ACCOUNT_NO == account_no // &&
                 //Switches.MPAN_MPRN == mpan_mprn
                 select Switch);
            switches_found = new List<SmartUtility.Switches>(switches_found.Distinct());
            return switches_found;
        }

        internal static SmartUtility.Resources Utility_Extract_MatchingSingleResource(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                //string username,
                                                                short supplier_code,
                                                                short brand_code,
                                                                DateTime created,
                                                                string account_no,
                                                                string mpan_mprn)
        {
            List<SmartUtility.Resources> resources_found =
                new List<SmartUtility.Resources>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE }
                 join Switches in utilityviewmodel.Hezbollah.utility_switchesList
                 on new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
                 equals new { Switches.USERNAME, Switches.CUBEFACE_CODE, Switches.MPAN_MPRN }
                 where Switches.SUPPLIER_CODE == supplier_code &&
                        Switches.BRAND_CODE == brand_code &&
                        Switches.RESOURCE_CODE == utilityviewmodel.resource_code &&
                        Switches.SWITCH_CREATED == created &&
                        Switches.ACCOUNT_NO == account_no &&
                        Switches.MPAN_MPRN == mpan_mprn
                 orderby Switches.SWITCH_CREATED descending // Don't really need this because cvr is already sorted
                 select Resource);
            resources_found = new List<SmartUtility.Resources>(resources_found.Distinct());
            if (resources_found.Count == 1)
            {
                return resources_found.First();
            }
            return new SmartUtility.Resources();
        }

        //internal static SmartUtility.Switches Utility_Extract_MatchingSingleSwitch(MainViewModel ourviewmodel,
        //                                                       UtilityViewModel utilityviewmodel,
        //                                                       string username,
        //                                                       short supplier_code,
        //                                                       short brand_code,
        //                                                       DateTime created,
        //                                                       string account_no,
        //                                                       string mpan_mprn)
        //{
        //    List<SmartUtility.Switches> switches_found = new List<SmartUtility.Switches>(from Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
        //                                                  join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
        //                                                  on new { Cubeface.CUBEFACE_CODE }
        //                                                  equals new { Resource.CUBEFACE_CODE }
        //                                                  join Meter in utilityviewmodel.Hezbollah.utility_metersList
        //                                                  on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
        //                                                  equals new { Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE }
        //                                                  join Switches in utilityviewmodel.Hezbollah.utility_switchesList
        //                                                  on new { Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
        //                                                  equals new { Switches.CUBEFACE_CODE, Switches.MPAN_MPRN }
        //                                                  where (Cubeface.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
        //                                                          Switches.USERNAME == username &&
        //                                                          Switches.SUPPLIER_CODE == supplier_code &&
        //                                                          Switches.BRAND_CODE == brand_code &&
        //                                                          Switches.SWITCH_CREATED == created &&
        //                                                          Switches.ACCOUNT_NO == account_no &&
        //                                                          Switches.MPAN_MPRN == mpan_mprn)
        //                                                  orderby Switches.SWITCH_CREATED descending // Don't really need this because cvr is already sorted
        //                                                  select Switches);
        //    if (switches_found.Count > 0)
        //    {
        //        return switches_found.First();
        //    }
        //    return new SmartUtility.Switches();
        //}

        internal static SmartUtility.Resources Utility_Extract_MatchingResources(MainViewModel ourviewmodel,
                                                               UtilityViewModel utilityviewmodel)
        //string username)
        {
            List<SmartUtility.Resources> resources_found =
                new List<SmartUtility.Resources>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                 select Resource);
            resources_found = new List<SmartUtility.Resources>(resources_found.Distinct());
            if (resources_found.Count > 0)
            {
                return resources_found.First();
            }
            return new SmartUtility.Resources();
        }

        internal static List<SmartUtility.Resources> UtilityFindResourcesList(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel,
                                                                                char resource_code)
        {
            List<SmartUtility.Resources> resources_found =
                new List<SmartUtility.Resources>();
            if (resource_code == SmartParametersV2016.defaultResourceCode ||
                (resource_code == SmartParametersV2016.DualFuel))
            {
                resources_found = new List<SmartUtility.Resources>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     orderby Resource.RESOURCE_CODE ascending // So E comes before G when D
                     select Resource);
            }
            else
            {
                resources_found = new List<SmartUtility.Resources>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 orderby Resource.RESOURCE_CODE ascending // So E comes before G when D
                 where Resource.RESOURCE_CODE == resource_code
                 select Resource);
            }
            resources_found = new List<SmartUtility.Resources>(resources_found.Distinct());
            return resources_found;
        }

        internal static List<SmartUtility.Accounts> Utility_Lookup_Resources(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel,
                                                                                char resource_code)
        {
            List<SmartUtility.Accounts> accounts_found =
                new List<SmartUtility.Accounts>();
            if (resource_code == SmartParametersV2016.defaultResourceCode ||
                (resource_code == SmartParametersV2016.DualFuel))
            {
                accounts_found = new List<SmartUtility.Accounts>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join Account in utilityviewmodel.Hezbollah.utility_accountsList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                     orderby Resource.RESOURCE_CODE ascending // So E comes before G when D
                     select Account);
            }
            else
            {
                accounts_found = new List<SmartUtility.Accounts>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                     join Account in utilityviewmodel.Hezbollah.utility_accountsList
                     on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                     equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                     orderby Resource.RESOURCE_CODE ascending // So E comes before G when D
                     where Resource.RESOURCE_CODE == resource_code
                     select Account);
            }
            accounts_found = new List<SmartUtility.Accounts>(accounts_found.Distinct());
            return accounts_found;
        }
        internal static List<SmartUtility.Switches> Utility_SwitchesList(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel)
        //char resource_code)
        {
            //List<SmartUtility.Meters> mpan_mprnList = Lookup_Meters_By_Resource(ourviewmodel,
            //                                                                                    utilityviewmodel, 
            //                                                                                    resource_code);
            List<SmartUtility.Switches> switches_found =
                new List<SmartUtility.Switches>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE }
                 join Switches in utilityviewmodel.Hezbollah.utility_switchesList
                 on new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
                 equals new { Switches.USERNAME, Switches.CUBEFACE_CODE, Switches.MPAN_MPRN }
                 where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                 select Switches);
            switches_found = new List<SmartUtility.Switches>(switches_found.Distinct());
            return switches_found;
        }

        internal static List<SmartUtility.Switches> Utility_Consumer_CancelList(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.Switches> switches_found =
                new List<SmartUtility.Switches>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)


                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.RESOURCE_CODE }
                 join Switches in utilityviewmodel.Hezbollah.utility_switchesList
                 on new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
                 equals new { Switches.USERNAME, Switches.CUBEFACE_CODE, Switches.MPAN_MPRN }
                 where Resource.RESOURCE_CODE == utilityviewmodel.resource_code
                 select Switches);
            switches_found = new List<SmartUtility.Switches>(switches_found.Distinct());
            return switches_found;
        }

        internal static List<SmartUtility.UnitRates> Utility_UnitRates(UtilityViewModel utilityviewmodel,
                                                    short supplier_code,
                                                    char resource_code,
                                                    string resource_type,
                                                    int tariff_code,
                                                    short payment_code,
                                                    DateTime prices_valid_from,
                                                    short tier_count,
                                                    short tier_level,
                                                    short area_code)
        {
            List<SmartUtility.UnitRates> unit_rates_found = new List<SmartUtility.UnitRates>();
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    unit_rates_found = new List<SmartUtility.UnitRates>
                        (from Unit_Rate in utilityviewmodel.Hezbollah.e_unit_ratesList
                         where (Unit_Rate.CUBEFACE_CODE == utilityviewmodel.cubeface_code) &&
                                 (Unit_Rate.SUPPLIER_CODE == supplier_code) &&
                                 (Unit_Rate.RESOURCE_CODE == resource_code) &&
                                 (Unit_Rate.RESOURCE_TYPE == resource_type) &&
                                 (Unit_Rate.TARIFF_CODE == tariff_code) &&
                                 (Unit_Rate.PAYMENT_CODE == payment_code) &&
                                 (Unit_Rate.PRICES_VALID_FROM == prices_valid_from) &&
                                 (Unit_Rate.TIER_COUNT == tier_count) &&
                                 (Unit_Rate.TIER_LEVEL == tier_level) &&
                                 (Unit_Rate.AREA_CODE == area_code)
                         select Unit_Rate);
                    break;
                case SmartParametersV2016.Gas:
                    unit_rates_found = new List<SmartUtility.UnitRates>
                        (from Unit_Rate in utilityviewmodel.Hezbollah.g_unit_ratesList
                         where (Unit_Rate.CUBEFACE_CODE == utilityviewmodel.cubeface_code) &&
                                 (Unit_Rate.SUPPLIER_CODE == supplier_code) &&
                                 (Unit_Rate.RESOURCE_CODE == resource_code) &&
                                 (Unit_Rate.RESOURCE_TYPE == resource_type) &&
                                 (Unit_Rate.TARIFF_CODE == tariff_code) &&
                                 (Unit_Rate.PAYMENT_CODE == payment_code) &&
                                 (Unit_Rate.PRICES_VALID_FROM == prices_valid_from) &&
                                 (Unit_Rate.TIER_COUNT == tier_count) &&
                                 (Unit_Rate.TIER_LEVEL == tier_level) &&
                                 (Unit_Rate.AREA_CODE == area_code)
                         select Unit_Rate);
                    break;
            }
            return unit_rates_found;
        }

        internal static List<SmartUtility.Suppliers> Utility_Lookup_Suppliers(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    short supplier_code)
        {
            List<SmartUtility.Suppliers> suppliers_found = new List<SmartUtility.Suppliers>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                 on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                 join Supplier in utilityviewmodel.Hezbollah.suppliersList
                 on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                 equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                 where (Resource.RESOURCE_CODE == utilityviewmodel.resource_code &&
                        Supplier.SUPPLIER_CODE == supplier_code)
                 select Supplier);
            suppliers_found =
                new List<SmartUtility.Suppliers>
                (suppliers_found.DistinctBy(key => new
                {
                    key.CUBEFACE_CODE,
                    key.SUPPLIER_CODE
                }
                ));
            return suppliers_found;
        }

        internal static string Utility_Lookup_TariffName(UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                short supplier_code,
                                                short brand_code,
                                                string resource_type,
                                                int tariff_code)
        {
            if (utilityviewmodel.analysistariffsnameviewList.Count == 0)
            {
                utilityviewmodel.analysistariffsnameviewList =
                    new List<SmartUtility.AnalysisTariffsNameView>
                    (from Tariff in utilityviewmodel.Hezbollah.tariffsList
                     join Tariff_Matrix in utilityviewmodel.Hezbollah.tariff_matrixList
                     on new { Tariff.RESOURCE_CODE, Tariff.RESOURCE_TYPE, Tariff.TARIFF_CODE }
                     equals new { Tariff_Matrix.RESOURCE_CODE, Tariff_Matrix.RESOURCE_TYPE, Tariff_Matrix.TARIFF_CODE }
                     where Tariff_Matrix.PLACEHOLDER == SmartParametersV2016.noFlag
                     select new SmartUtility.AnalysisTariffsNameView
                     {
                         CUBEFACE_CODE = utilityviewmodel.cubeface_code,
                         RESOURCE_CODE = Tariff.RESOURCE_CODE,
                         RESOURCE_TYPE = Tariff.RESOURCE_TYPE,
                         SUPPLIER_CODE = Tariff.SUPPLIER_CODE,
                         TARIFF_CODE = Tariff.TARIFF_CODE,
                         BRAND_CODE = Tariff_Matrix.BRAND_CODE,
                         VERSION_CODE = Tariff_Matrix.VERSION_CODE,
                         TARIFF_NAME = Tariff_Matrix.TARIFF_NAME
                     });
            }

            List<SmartUtility.AnalysisTariffsNameView> tariffnamesview_found =
                new List<SmartUtility.AnalysisTariffsNameView>();

            if (resource_code == SmartParametersV2016.DualFuel)
            {
                tariffnamesview_found =
                    new List<SmartUtility.AnalysisTariffsNameView>
                    (from TariffsNameView in utilityviewmodel.analysistariffsnameviewList
                     where ((TariffsNameView.SUPPLIER_CODE == supplier_code) &&
                             (TariffsNameView.RESOURCE_TYPE == resource_type) &&
                             (TariffsNameView.TARIFF_CODE == tariff_code) &&
                             (TariffsNameView.BRAND_CODE == brand_code))
                     orderby TariffsNameView.VERSION_CODE descending
                     select TariffsNameView);
            }
            else
            {
                tariffnamesview_found =
                    new List<SmartUtility.AnalysisTariffsNameView>
                    (from TariffsNameView in utilityviewmodel.analysistariffsnameviewList
                     where ((TariffsNameView.SUPPLIER_CODE == supplier_code) &&
                     (TariffsNameView.RESOURCE_CODE == resource_code) &&
                     (TariffsNameView.RESOURCE_TYPE == resource_type) &&
                     (TariffsNameView.TARIFF_CODE == tariff_code) &&
                     (TariffsNameView.BRAND_CODE == brand_code))
                     orderby TariffsNameView.VERSION_CODE descending
                     select TariffsNameView);
            }
            if (tariffnamesview_found.Count > 0)
            {
                return tariffnamesview_found.First().TARIFF_NAME;  // Only the first because for ALL TARIFF CODES!!! ....
                                                                   // ... the name is the same!  It has to be!!!  We check!
            }
            return "";
        }

        internal static string Utility_Lookup_TariffType(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                            char resource_code,
                                                            string resource_type)
        {
            // Only if its not empty and there is something in the table
            if ((!string.IsNullOrEmpty(resource_type)) &&
                (utilityviewmodel.Hezbollah.resource_typesList.Count > 0))
            {
                if (resource_code != SmartParametersV2016.defaultChar)
                {
                    // Look for ones that aren't deleted
                    // Form collection
                    List<SmartUtility.ResourceTypes> resource_type_found =
                        new List<SmartUtility.ResourceTypes>(
                            from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                            join Resource in utilityviewmodel.Hezbollah.resource_codesList
                            on new { Cubeface.CUBEFACE_CODE }
                            equals new { Resource.CUBEFACE_CODE }
                            join ResourceType in utilityviewmodel.Hezbollah.resource_typesList
                            on new { Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                            equals new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE }
                            where Resource.RESOURCE_CODE == resource_code &&
                                    ResourceType.RESOURCE_TYPE == resource_type
                            select ResourceType);
                    resource_type_found = new List<SmartUtility.ResourceTypes>(resource_type_found.Distinct());
                    // Test collection
                    if (resource_type_found.Count > 0)
                    {
                        return resource_type_found.First().DESCRIPTION;
                    }
                }
            }
            return "";
        }

        internal static List<SmartUtility.Tariffs> Utility_TariffsList(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel,
                                                short supplier_code,
                                                char resource_code,
                                                string resource_type,
                                                int tariff_code)
        {
            // Look for all the Tariffs in this Supplier
            // Tariffs are Supplier specific .....
            List<SmartUtility.Tariffs> tariffs_found =
                new List<SmartUtility.Tariffs>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Tariff in utilityviewmodel.Hezbollah.tariffsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Tariff.CUBEFACE_CODE }
                 where (Tariff.SUPPLIER_CODE == supplier_code) &&
                         (Tariff.RESOURCE_CODE == resource_code) &&
                         (Tariff.RESOURCE_TYPE == resource_type) &&
                         (Tariff.TARIFF_CODE == tariff_code)
                 select Tariff);
            tariffs_found = new List<SmartUtility.Tariffs>(tariffs_found.Distinct());
            return tariffs_found;
        }

        internal static List<SmartUtility.ConditionsPlans> Utility_ConditionsPlans(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,  // Shouldn't this be a short??
                                                                char resource_code,
                                                                string resource_type,
                                                                int tariff_code,
                                                                char payment_plan)
        {
            List<SmartUtility.ConditionsPlans> conditions_plans_found =
                new List<SmartUtility.ConditionsPlans>();
            if (payment_plan == SmartParametersV2016.defaultChar)
            {
                conditions_plans_found = new List<SmartUtility.ConditionsPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Conditions_Plan in utilityviewmodel.Hezbollah.conditions_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Conditions_Plan.CUBEFACE_CODE }
                     where (Conditions_Plan.SUPPLIER_CODE == supplier_code) &&
                             (Conditions_Plan.RESOURCE_CODE == resource_code) &&
                             (Conditions_Plan.RESOURCE_TYPE == resource_type) &&
                             (Conditions_Plan.TARIFF_CODE == tariff_code)
                     select Conditions_Plan);
            }
            else
            {
                conditions_plans_found = new List<SmartUtility.ConditionsPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Conditions_Plan in utilityviewmodel.Hezbollah.conditions_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Conditions_Plan.CUBEFACE_CODE }
                     where (Conditions_Plan.SUPPLIER_CODE == supplier_code) &&
                             (Conditions_Plan.RESOURCE_CODE == resource_code) &&
                             (Conditions_Plan.RESOURCE_TYPE == resource_type) &&
                             (Conditions_Plan.TARIFF_CODE == tariff_code) &&
                             (Conditions_Plan.PAYMENT_PLANS.Contains(payment_plan))
                     select Conditions_Plan);
            }
            conditions_plans_found = new List<SmartUtility.ConditionsPlans>(conditions_plans_found.Distinct());
            return conditions_plans_found;
        }

        internal static List<SmartUtility.ConditionsView> Utility_ConditionsView(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                char resource_code,
                                                                string resource_type,
                                                                int tariff_code,
                                                                short payment_code,
                                                                short version_code)
        {
            List<SmartUtility.ConditionsView> conditions_view_found =
                new List<SmartUtility.ConditionsView>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Conditions_Date in utilityviewmodel.Hezbollah.conditions_datesList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Conditions_Date.CUBEFACE_CODE }
                 join Conditions_Group in utilityviewmodel.Hezbollah.conditions_groupsList
                 on new { Conditions_Date.CUBEFACE_CODE, Conditions_Date.SUPPLIER_CODE, Conditions_Date.RESOURCE_CODE, Conditions_Date.RESOURCE_TYPE, Conditions_Date.TARIFF_CODE, Conditions_Date.VERSION_CODE, Conditions_Date.PAYMENT_CODE, Conditions_Date.TIER_COUNT }
                 equals new { Conditions_Group.CUBEFACE_CODE, Conditions_Group.SUPPLIER_CODE, Conditions_Group.RESOURCE_CODE, Conditions_Group.RESOURCE_TYPE, Conditions_Group.TARIFF_CODE, Conditions_Group.VERSION_CODE, Conditions_Group.PAYMENT_CODE, Conditions_Group.TIER_COUNT }
                 where (Conditions_Date.SUPPLIER_CODE == supplier_code) &&
                         (Conditions_Date.RESOURCE_CODE == resource_code) &&
                         (Conditions_Date.RESOURCE_TYPE == resource_type) &&
                         (Conditions_Date.TARIFF_CODE == tariff_code) &&
                         (Conditions_Date.VERSION_CODE == version_code) &&
                         (Conditions_Date.PAYMENT_CODE == payment_code)
                 orderby Conditions_Date.PRICES_VALID_FROM descending,
                         Conditions_Date.TIER_COUNT descending
                 select new SmartUtility.ConditionsView  // Can use 'new' because we specify the fields below
                 {
                     SUPPLIER_CODE = Conditions_Date.SUPPLIER_CODE,
                     RESOURCE_CODE = Conditions_Date.RESOURCE_CODE,
                     RESOURCE_TYPE = Conditions_Date.RESOURCE_TYPE,
                     TARIFF_CODE = Conditions_Date.TARIFF_CODE,
                     VERSION_CODE = Conditions_Date.VERSION_CODE,
                     PAYMENT_CODE = Conditions_Date.PAYMENT_CODE,
                     PRICES_VALID_FROM = Conditions_Date.PRICES_VALID_FROM,
                     TIER_COUNT = Conditions_Date.TIER_COUNT,
                     TIER_LEVEL = Conditions_Group.TIER_LEVEL //,
                                                              //LIMIT_CODE = Conditions_Group.LIMIT_CODE,
                                                              //GROUP_NAME = Conditions_Date.GROUP_NAME
                 });
            conditions_view_found =
                new List<SmartUtility.ConditionsView>(conditions_view_found.Distinct());
            return conditions_view_found;
        }

        internal static List<SmartUtility.ConditionsGroups> Utility_Find_ConditionsGroups(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                int tariff_code,
                                                                char resource_code,
                                                                string resource_type,
                                                                short version_code,
                                                                short payment_code,
                                                                short tier_count)
        {
            List<SmartUtility.ConditionsGroups> conditions_groups_found =
                new List<SmartUtility.ConditionsGroups>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Conditions_Groups in utilityviewmodel.Hezbollah.conditions_groupsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Conditions_Groups.CUBEFACE_CODE }
                 where (Conditions_Groups.SUPPLIER_CODE == supplier_code) &&
                         (Conditions_Groups.RESOURCE_CODE == resource_code) &&
                         (Conditions_Groups.RESOURCE_TYPE == resource_type) &&
                         (Conditions_Groups.TARIFF_CODE == tariff_code) &&
                         (Conditions_Groups.VERSION_CODE == version_code) &&
                         (Conditions_Groups.PAYMENT_CODE == payment_code) &&
                         (Conditions_Groups.TIER_COUNT == tier_count)
                 orderby Conditions_Groups.TIER_LEVEL ascending
                 select Conditions_Groups);
            conditions_groups_found = new List<SmartUtility.ConditionsGroups>(conditions_groups_found.Distinct());
            return conditions_groups_found;
        }

        internal static List<SmartUtility.TariffCodesNames> Utility_TariffCodesNames(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel)
        {

            //      select TAM.BRAND_CODE,
            //      TAM.RESOURCE_CODE,
            //      TAM.RESOURCE_TYPE,
            //      TAM.TARIFF_CODE,
            //      TAM.VERSION_CODE,
            //TAM.PLACEHOLDER,
            //      TNH.TARIFF_NAME
            //       from SmartData.CATEGORIES AS C
            //       JOIN SmartData.SUPPLIERS_NEW as SN
            //       on(C.CATEGORY_CODE = SN.CATEGORY_CODE)
            //       join SmartData.SUPPLIER_TYPES as ST
            //       on(SN.CATEGORY_CODE = ST.CATEGORY_CODE and SN.SUPPLIER_CODE = ST.SUPPLIER_CODE)
            //       join SmartData.BRANDS as B
            //       on(ST.CATEGORY_CODE = B.CATEGORY_CODE and ST.SUPPLIER_CODE = B.SUPPLIER_CODE)
            //       join SmartUtility.TARIFFS as T

            //       on(B.SUPPLIER_CODE = T.SUPPLIER_CODE and ST.RESOURCE_CODE = T.RESOURCE_CODE and ST.RESOURCE_TYPE = T.RESOURCE_TYPE)
            //       join SmartUtility.TARIFF_MATRIX as TAM

            //       on(B.BRAND_CODE = TAM.BRAND_CODE and T.RESOURCE_CODE = TAM.RESOURCE_CODE and T.RESOURCE_TYPE = TAM.RESOURCE_TYPE and T.TARIFF_CODE = TAM.TARIFF_CODE)
            //       join SmartUtility.TARIFF_HISTORY as TNH

            //       on(TAM.BRAND_CODE = TNH.BRAND_CODE and TAM.RESOURCE_CODE = TNH.RESOURCE_CODE and TAM.RESOURCE_TYPE = TNH.RESOURCE_TYPE and TAM.TARIFF_CODE = TNH.TARIFF_CODE)
            //       -- into nibby
            //       --from N in nibby.DefaultIfEmpty()
            //         where (TAM.PLACEHOLDER = 'N')

            List<SmartUtility.TariffCodesNames> tariff_codes_names_found =
                new List<SmartUtility.TariffCodesNames>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join ResourceType in utilityviewmodel.Hezbollah.resource_codesList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { ResourceType.CUBEFACE_CODE }
                 join SupplierType in utilityviewmodel.Hezbollah.supplier_typesList
                 on new { ResourceType.CUBEFACE_CODE, ResourceType.RESOURCE_CODE }
                 equals new { SupplierType.CUBEFACE_CODE, SupplierType.RESOURCE_CODE }
                 join Supplier in utilityviewmodel.Hezbollah.suppliersList
                 on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                 equals new { Supplier.CUBEFACE_CODE, Supplier.SUPPLIER_CODE }  // Lose the RESOURCE_CODE because most Suppliers aren't Resource specific
                 join Brand in utilityviewmodel.Hezbollah.brandsList
                 on new { SupplierType.CUBEFACE_CODE, SupplierType.SUPPLIER_CODE }
                 equals new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE }
                 join T in utilityviewmodel.Hezbollah.tariffsList
                 on new { Brand.CUBEFACE_CODE, Brand.SUPPLIER_CODE, SupplierType.RESOURCE_CODE, SupplierType.RESOURCE_TYPE }
                 equals new { T.CUBEFACE_CODE, T.SUPPLIER_CODE, T.RESOURCE_CODE, T.RESOURCE_TYPE }
                 join TAM in utilityviewmodel.Hezbollah.tariff_matrixList
                 on new { T.CUBEFACE_CODE, Brand.BRAND_CODE, T.RESOURCE_CODE, T.RESOURCE_TYPE, T.TARIFF_CODE }
                 equals new { TAM.CUBEFACE_CODE, TAM.BRAND_CODE, TAM.RESOURCE_CODE, TAM.RESOURCE_TYPE, TAM.TARIFF_CODE }
                 join TNH in utilityviewmodel.Hezbollah.tariff_historyList
                 on new { TAM.CUBEFACE_CODE, TAM.BRAND_CODE, TAM.RESOURCE_CODE, TAM.RESOURCE_TYPE, TAM.TARIFF_CODE, TAM.VERSION_CODE }
                 equals new { TNH.CUBEFACE_CODE, TNH.BRAND_CODE, TNH.RESOURCE_CODE, TNH.RESOURCE_TYPE, TNH.TARIFF_CODE, TNH.VERSION_CODE } into nibby
                 from N in nibby.DefaultIfEmpty()
                 where TAM.PLACEHOLDER == SmartParametersV2016.noFlag
                 select new SmartUtility.TariffCodesNames
                 {
                     CUBEFACE_CODE = TAM.CUBEFACE_CODE,
                     BRAND_CODE = TAM.BRAND_CODE,
                     RESOURCE_CODE = TAM.RESOURCE_CODE,
                     RESOURCE_TYPE = TAM.RESOURCE_TYPE,
                     TARIFF_CODE = TAM.TARIFF_CODE,
                     VERSION_CODE = TAM.VERSION_CODE,
                     PLACEHOLDER = TAM.PLACEHOLDER,
                     TARIFF_NAME = TAM.TARIFF_NAME, // Why isn't this TNH.TARIFF_NAME
                     TARIFF_PREVIOUS_NAME = (N != null ? N.TARIFF_PREVIOUS_NAME : ""),
                     AREA_10 = TAM.AREA_10,
                     AREA_11 = TAM.AREA_11,
                     AREA_12 = TAM.AREA_12,
                     AREA_13 = TAM.AREA_13,
                     AREA_14 = TAM.AREA_14,
                     AREA_15 = TAM.AREA_15,
                     AREA_16 = TAM.AREA_16,
                     AREA_17 = TAM.AREA_17,
                     AREA_18 = TAM.AREA_18,
                     AREA_19 = TAM.AREA_19,
                     AREA_20 = TAM.AREA_20,
                     AREA_21 = TAM.AREA_21,
                     AREA_22 = TAM.AREA_22,
                     AREA_23 = TAM.AREA_23
                 });
            tariff_codes_names_found = new List<SmartUtility.TariffCodesNames>(tariff_codes_names_found.Distinct());
            return tariff_codes_names_found;
        }

        internal static List<SmartUtility.TariffCodesNames> Utility_TCNSubsetList(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            short brand_code)
        {
            List<SmartUtility.TariffCodesNames> reduced_found =
                new List<SmartUtility.TariffCodesNames>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join TCN in utilityviewmodel.Hezbollah.tariff_codes_namesList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { TCN.CUBEFACE_CODE }
                 where TCN.BRAND_CODE == brand_code &&
                     TCN.RESOURCE_CODE != SmartParametersV2016.defaultResourceCode &&
                     !string.IsNullOrEmpty(TCN.RESOURCE_TYPE) &&
                     TCN.TARIFF_CODE > 0 &&
                     TCN.VERSION_CODE > 0 &&
                     TCN.PLACEHOLDER == SmartParametersV2016.noFlag &&
                     !string.IsNullOrEmpty(TCN.TARIFF_NAME) &&
                     TCN.AREA_10 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_11 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_12 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_13 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_14 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_15 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_16 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_17 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_18 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_19 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_20 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_21 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_22 != SmartParametersV2016.defaultChar &&
                     TCN.AREA_23 != SmartParametersV2016.defaultChar
                 select TCN);
            reduced_found = new List<SmartUtility.TariffCodesNames>(reduced_found.Distinct());
            return reduced_found;
        }

        internal static List<SmartUtility.AmeliasView> Utility_AmeliaDesc()
        {
            List<SmartUtility.AmeliasView> amelia_found =
                new List<SmartUtility.AmeliasView>();
            return amelia_found;
        }

        internal static List<SmartUtility.Switches> Utility_Lookup_Switches(MainViewModel ourviewmodel,
                                                                                    UtilityViewModel utilityviewmodel)

        {
            List<SmartUtility.Switches> switches_found = new List<SmartUtility.Switches>();
            // I have no idea what this bollocks is all about =>
            //                // Yeah, because if we have three Utility.Accounts one with SR and one with VR
            //                // and we want to see ALL of the 'E' anf 'G' history, then we DO want to see
            //                // BOTH of these in the list ... its only when we get down to
            //                // indvidual Bills/Bills Resource that we need to distinguish between
            //                // 'E' SR and VR and 'G' and 'SR'
            switches_found = new List<SmartUtility.Switches>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 join Switch in utilityviewmodel.Hezbollah.utility_switchesList
                 on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE, Account.SUPPLIER_CODE, Account.BRAND_CODE, Account.ACCOUNT_NO }
                 equals new { Switch.USERNAME, Switch.CUBEFACE_CODE, Switch.RESOURCE_CODE, Switch.SUPPLIER_CODE, Switch.BRAND_CODE, Switch.ACCOUNT_NO }
                 orderby Switch.SWITCH_CREATED descending
                 select Switch);
            switches_found = new List<SmartUtility.Switches>(switches_found.Distinct());
            return switches_found;
        }

        internal static bool Utility_Find_OpenAccounts(MainViewModel ourviewmodel,
                                                UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.Accounts> accounts_found =
                new List<SmartUtility.Accounts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 join Account in utilityviewmodel.Hezbollah.utility_accountsList
                 on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                 equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 join Meters in utilityviewmodel.Hezbollah.utility_metersList
                 on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                 equals new { Meters.USERNAME, Meters.CUBEFACE_CODE, Meters.RESOURCE_CODE }
                 where Account.STATUS != 'N'
                 orderby Account.ACCOUNT_CREATED descending    // So we get the latest
                 select Account);
            accounts_found =
                new List<SmartUtility.Accounts>(accounts_found.Distinct());
            if (accounts_found.Count > 1)
            {
                return true;
            }
            return false;
        }

        internal static List<SmartUtility.Bills> Utility_BillsList(UtilityViewModel utilityviewmodel,
                                                short brand_code,
                                                short supplier_code,
                                                string account_no,
                                                DateTime created,
                                                string statement_id,
                                                DateTime bill_date)
        {
            // Well ... we cannot use Account No because we might not know what it is!!
            // (we haven't opened the Bill yet, and it may be a different account no from
            // the current one. NPower have more than one account no for the same 'Customer')
            // So to be on the safe side, we need to get all the Bills AND the Resources at the same
            // time .... BUT .. we should always know the Bill Number/Bill Date because 
            // ...Because WHAT?
            // We may *NOT* know the Bill Date but we should always have the Statement Id becuase
            // that is how the supplier identifies their bills.  We may alos have MORE THAN ONE
            // Statement (probably with different Ids) on the same Bill Date (Sothern Electric?)
            // However - FU (or Fuck ME as it should be known) DON'T give a Statement ID which I can
            // convert into a Bill Date e.g. 394361-001 so if the bill date is the default 01-Jan-1900
            // (which means I couldn't convert it) then use only the Statement Id.  I will have to keep
            // my fingers crossed that FMe don't fuck about with this...
            List<SmartUtility.Bills> billsList_found = new List<SmartUtility.Bills>();

            if (bill_date != SmartParametersV2016.defaultDate)
            {
                billsList_found = new List<SmartUtility.Bills>
                    (from Bill in utilityviewmodel.Hezbollah.working_billsList
                     where Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                             Bill.SUPPLIER_CODE == supplier_code &&
                             Bill.BRAND_CODE == brand_code &&
                             Bill.ACCOUNT_NO == account_no &&
                             Bill.ACCOUNT_CREATED == created &&
                             Bill.STATEMENT_ID == statement_id &&
                             Bill.BILL_DATE == bill_date // Can use this as its derived from the filename
                     select Bill);
            }
            else
            {
                billsList_found = new List<SmartUtility.Bills>
                    (from Bill in utilityviewmodel.Hezbollah.working_billsList
                     where Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                             Bill.SUPPLIER_CODE == supplier_code &&
                             Bill.BRAND_CODE == brand_code &&
                             Bill.ACCOUNT_NO == account_no &&
                             Bill.ACCOUNT_CREATED == created &&
                             Bill.STATEMENT_ID == statement_id //
                                                               //(Bill.BILL_DATE == bill_date)))  // Can't look up on BILL_DATE as I have no idea what it could be
                                                               // It is derived from being embedded in the Bill, which I
                                                               // Haven't opened or read yet ... =:-{
                     select Bill);
            }
            return billsList_found;
        }

        internal static List<SmartUtility.BillsResource> Utility_Bills_ResourceListNew(UtilityViewModel utilityviewmodel,
                                                                        string mpan_mprn,
                                                                        SmartUtility.Bills bills_row)
        {
            List<SmartUtility.BillsResource> bills_resourceList_found =
                new List<SmartUtility.BillsResource>();
            bills_resourceList_found = new List<SmartUtility.BillsResource>
                (from BR in utilityviewmodel.Hezbollah.bills_resourceList
                 where (BR.USERNAME == bills_row.USERNAME &&
                     BR.CUBEFACE_CODE == bills_row.CUBEFACE_CODE &&
                     BR.SUPPLIER_CODE == bills_row.SUPPLIER_CODE &&
                     BR.BRAND_CODE == bills_row.BRAND_CODE &&
                     BR.ACCOUNT_NO == bills_row.ACCOUNT_NO &&
                     BR.ACCOUNT_CREATED == bills_row.ACCOUNT_CREATED &&
                     BR.STATEMENT_ID == bills_row.STATEMENT_ID &&
                     BR.BILL_DATE == bills_row.BILL_DATE &&
                     BR.MPAN_MPRN == mpan_mprn)
                 select BR);
            return bills_resourceList_found;
        }

        // This one appears for the Utility tab in SmartDashboard??
        internal static List<SmartUtility.BillsResource> Utility_Configure_BillResource(UtilityViewModel utilityviewmodel)
        {
            // The Distinct() ensures that when we have two Account records with the
            // same ACCOUNT_NO for both 'E' and 'G' i.e. we have a combined usage
            // bill, we only get ONE Bill returned for BOTH Account Numbers

            List<SmartUtility.BillsResource> bills_resource_found = new List<SmartUtility.BillsResource>();
            bills_resource_found = new List<SmartUtility.BillsResource>
                (from Bill in utilityviewmodel.Hezbollah.billsList
                 join Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 on new
                 {
                     Bill.USERNAME,
                     Bill.CUBEFACE_CODE,
                     Bill.SUPPLIER_CODE,
                     Bill.BRAND_CODE,
                     Bill.ACCOUNT_NO,
                     Bill.ACCOUNT_CREATED,
                     Bill.STATEMENT_ID,
                     Bill.BILL_DATE
                 }
                 equals new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE
                 }
                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     Meter.USERNAME,
                     Meter.CUBEFACE_CODE,
                     Meter.MPAN_MPRN
                 }
                 orderby Bill.BILL_PERIOD_START ascending
                 //where //Bill.USERNAME == ourviewmodel.UserName &&
                 //Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 select Bill_Resource);
            bills_resource_found = new List<SmartUtility.BillsResource>
                (bills_resource_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.CUBEFACE_CODE,
                    key.SUPPLIER_CODE,
                    key.BRAND_CODE,
                    key.ACCOUNT_NO,
                    key.ACCOUNT_CREATED,
                    key.STATEMENT_ID,
                    key.BILL_DATE,
                    key.MPAN_MPRN
                }));
            if (utilityviewmodel.resource_code != SmartParametersV2016.DualFuel)
            {
                bills_resource_found = new List<SmartUtility.BillsResource>
                    (from BillsResource in bills_resource_found
                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on (BillsResource.USERNAME, BillsResource.CUBEFACE_CODE, BillsResource.MPAN_MPRN)
                     equals (Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN)
                     where Meter.RESOURCE_CODE == utilityviewmodel.resource_code
                     select BillsResource);
            }
            return bills_resource_found;
        }

        internal static List<SmartUtility.BillsResource> Utility_Find_BillResource(UtilityViewModel utilityviewmodel,
                                                                            SmartUtility.Bills working_bills_row)
        {
            // The Distinct() ensures that when we have two Account records with the
            // same ACCOUNT_NO for both 'E' and 'G' i.e. we have a combined usage
            // bill, we only get ONE Bill returned for BOTH Account Numbers

            List<SmartUtility.Bills> working_billsList = new List<SmartUtility.Bills> { working_bills_row };

            List<SmartUtility.BillsResource> bills_resource_found = new List<SmartUtility.BillsResource>();
            bills_resource_found = new List<SmartUtility.BillsResource>
                (from Bill in working_billsList
                 join Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 on new
                 {
                     Bill.USERNAME,
                     Bill.CUBEFACE_CODE,
                     Bill.SUPPLIER_CODE,
                     Bill.BRAND_CODE,
                     Bill.ACCOUNT_NO,
                     Bill.ACCOUNT_CREATED,
                     Bill.STATEMENT_ID,
                     Bill.BILL_DATE
                 }
                 equals new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE
                 }
                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     Meter.USERNAME,
                     Meter.CUBEFACE_CODE,
                     Meter.MPAN_MPRN
                 }
                 orderby Bill.BILL_PERIOD_START ascending
                 //where //Bill.USERNAME == ourviewmodel.UserName &&
                 //      Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 select Bill_Resource);
            bills_resource_found = new List<SmartUtility.BillsResource>
            (bills_resource_found.DistinctBy(key => new
            {
                key.USERNAME,
                key.CUBEFACE_CODE,
                key.SUPPLIER_CODE,
                key.BRAND_CODE,
                key.ACCOUNT_NO,
                key.ACCOUNT_CREATED,
                key.STATEMENT_ID,
                key.BILL_DATE,
                key.MPAN_MPRN
            }));
            if (utilityviewmodel.resource_code != SmartParametersV2016.DualFuel)
            {
                bills_resource_found = new List<SmartUtility.BillsResource>
                    (from BillsResource in bills_resource_found
                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on (BillsResource.USERNAME, BillsResource.CUBEFACE_CODE, BillsResource.MPAN_MPRN)
                     equals (Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN)
                     where Meter.RESOURCE_CODE == utilityviewmodel.resource_code
                     select BillsResource);
            }
            return bills_resource_found;
        }


        internal static List<SmartUtility.Bills> Utility_Loaded_BillList(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        string mpan_mprn)
        // See note below string resource_type,

        {
            List<SmartUtility.Bills> loaded_bills = new List<SmartUtility.Bills>();

            if (ourviewmodel.Hamas.consumersList.Count > 0)
            {

                // Now this should find ALL Utility.Accounts past AND present cos we need the Billing
                // info for ALL past and present Suppliers and Utility.Accounts
                List<SmartUtility.Accounts> accounts_found =
                    Utility_Lookup_Accounts(ourviewmodel,
                                    utilityviewmodel,
                                    SmartParametersV2016.defaultDate);
                if (accounts_found.Count > 0)
                {
                    List<SmartUtility.Bills> loaded_bills_found = new List<SmartUtility.Bills>();
                    if (string.IsNullOrEmpty(mpan_mprn))
                    {
                        loaded_bills_found = new List<SmartUtility.Bills>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                         on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                         equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                         join Account in utilityviewmodel.Hezbollah.utility_accountsList
                         on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                         equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                         join Bill in utilityviewmodel.Hezbollah.billsList
                         on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.SUPPLIER_CODE, Account.BRAND_CODE, Account.ACCOUNT_NO, Account.ACCOUNT_CREATED }
                         equals new { Bill.USERNAME, Bill.CUBEFACE_CODE, Bill.SUPPLIER_CODE, Bill.BRAND_CODE, Bill.ACCOUNT_NO, Bill.ACCOUNT_CREATED }
                         join Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                        on new
                        {
                            Bill.USERNAME,
                            Bill.CUBEFACE_CODE,
                            Bill.SUPPLIER_CODE,
                            Bill.BRAND_CODE,
                            Bill.ACCOUNT_NO,
                            Bill.ACCOUNT_CREATED,
                            Bill.STATEMENT_ID,
                            Bill.BILL_DATE
                        }
                        equals new
                        {
                            Bill_Resource.USERNAME,
                            Bill_Resource.CUBEFACE_CODE,
                            Bill_Resource.SUPPLIER_CODE,
                            Bill_Resource.BRAND_CODE,
                            Bill_Resource.ACCOUNT_NO,
                            Bill_Resource.ACCOUNT_CREATED,
                            Bill_Resource.STATEMENT_ID,
                            Bill_Resource.BILL_DATE
                        }
                        //orderby Bill.ACCOUNT_NO, Bill.BILL_PERIOD_START ascending, Bill.BILL_PERIOD_END descending //STATEMENT_ID descending
                         orderby Bill.ACCOUNT_NO, Bill.BILL_PERIOD_START ascending
                         select Bill);
                        loaded_bills_found = new List<SmartUtility.Bills>
                            (loaded_bills_found.DistinctBy(key => new
                            {
                                key.USERNAME,
                                key.CUBEFACE_CODE,
                                key.SUPPLIER_CODE,
                                key.BRAND_CODE,
                                key.ACCOUNT_NO,
                                key.ACCOUNT_CREATED,
                                key.STATEMENT_ID,
                                key.BILL_DATE
                            }));
                        // The DistinctBy() ensures that when we have two Account records with the
                        // same ACCOUNT_NO for both 'E' and 'G' i.e. we have a combined usage
                        // bill, we only get ONE Bill returned for BOTH Account Numbers
                    }
                    else
                    {
                        loaded_bills_found = new List<SmartUtility.Bills>
                           (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                            join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                            on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                            equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                            join Account in utilityviewmodel.Hezbollah.utility_accountsList
                            on new { Resource.USERNAME, Resource.CUBEFACE_CODE, Resource.RESOURCE_CODE }
                            equals new { Account.USERNAME, Account.CUBEFACE_CODE, Account.RESOURCE_CODE }
                            join Bill in utilityviewmodel.Hezbollah.billsList
                            on new { Account.USERNAME, Account.CUBEFACE_CODE, Account.SUPPLIER_CODE, Account.BRAND_CODE, Account.ACCOUNT_NO, Account.ACCOUNT_CREATED }
                            equals new { Bill.USERNAME, Bill.CUBEFACE_CODE, Bill.SUPPLIER_CODE, Bill.BRAND_CODE, Bill.ACCOUNT_NO, Bill.ACCOUNT_CREATED }
                            join Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                            on new
                            {
                                Bill.CUBEFACE_CODE,
                                Bill.SUPPLIER_CODE,
                                Bill.BRAND_CODE,
                                Bill.ACCOUNT_NO,
                                Bill.ACCOUNT_CREATED,
                                Bill.STATEMENT_ID,
                                Bill.BILL_DATE
                            }
                            equals new
                            {
                                Bill_Resource.CUBEFACE_CODE,
                                Bill_Resource.SUPPLIER_CODE,
                                Bill_Resource.BRAND_CODE,
                                Bill_Resource.ACCOUNT_NO,
                                Bill_Resource.ACCOUNT_CREATED,
                                Bill_Resource.STATEMENT_ID,
                                Bill_Resource.BILL_DATE
                            }
                            //orderby Bill.ACCOUNT_NO, Bill.BILL_PERIOD_START ascending, Bill.BILL_PERIOD_END descending //STATEMENT_ID descending
                            where Bill_Resource.MPAN_MPRN == mpan_mprn // &&
                                                                       // && No because its by Resource Code and ACROSS all Resource Types => Bill_Resource.RESOURCE_TYPE == resource_type
                            orderby Bill.ACCOUNT_NO, Bill.BILL_PERIOD_START ascending
                            select Bill);
                        loaded_bills_found = new List<SmartUtility.Bills>
                            (loaded_bills_found.DistinctBy(key => new
                            {
                                key.USERNAME,
                                key.CUBEFACE_CODE,
                                key.SUPPLIER_CODE,
                                key.BRAND_CODE,
                                key.ACCOUNT_NO,
                                key.ACCOUNT_CREATED,
                                key.STATEMENT_ID,
                                key.BILL_DATE
                            }));
                    }
                    loaded_bills.AddRange(loaded_bills_found);
                }
                // The DistinctBy() ensures that when we have two Account records with the
                // same ACCOUNT_NO for both 'E' and 'G' i.e. we have a combined usage
                // bill, we only get ONE Bill returned for BOTH Account Numbers
            }
            return loaded_bills;
        }

        internal static List<SmartUtility.EReadings> Utility_EReadings(UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.EReadings> e_readings_found =
                new List<SmartUtility.EReadings>
                (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 join E_Reading in utilityviewmodel.Hezbollah.e_readingsList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     E_Reading.USERNAME,
                     E_Reading.CUBEFACE_CODE,
                     E_Reading.SUPPLIER_CODE,
                     E_Reading.BRAND_CODE,
                     E_Reading.ACCOUNT_NO,
                     E_Reading.ACCOUNT_CREATED,
                     E_Reading.STATEMENT_ID,
                     E_Reading.BILL_DATE,
                     E_Reading.MPAN_MPRN
                 }
#if WINFORMS
                 where E_Reading.READINGS_PERIOD_START >= utilityviewmodel.StartDate &&
                       E_Reading.READINGS_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where E_Reading.READINGS_PERIOD_START >= utilityviewmodel.StartDate &&
                       E_Reading.READINGS_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where E_Reading.READINGS_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                       E_Reading.READINGS_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                 orderby E_Reading.BILL_DATE descending,
                 E_Reading.READINGS_PERIOD_START ascending,
                 E_Reading.READINGS_PERIOD_END ascending // So matching START/END dates come before unmatched START/END dates
                                                         // This pile of bollocks picks out the LAST (i.e. highest BILL_DATE, FROM_DATE, READ_DATE) from E_Reading                                                                                         
                                                         // Took me all fucking day ....
                                                         //where //Bill_Resource.USERNAME == ourviewmodel.UserName &&
                                                         //        Bill_Resource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 select E_Reading);
            e_readings_found = new List<SmartUtility.EReadings>(e_readings_found.GroupBy(p => new
            {
                p.READINGS_PERIOD_START,
                p.READINGS_PERIOD_END
            }).Select(q => q.First()).OrderBy(r => r.READINGS_PERIOD_START));

            return e_readings_found;
        }

        internal static List<SmartUtility.GReadings> Utility_GReadings(UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.GReadings> g_readings_found =
                new List<SmartUtility.GReadings>
                (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 join G_Reading in utilityviewmodel.Hezbollah.g_readingsList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     G_Reading.USERNAME,
                     G_Reading.CUBEFACE_CODE,
                     G_Reading.SUPPLIER_CODE,
                     G_Reading.BRAND_CODE,
                     G_Reading.ACCOUNT_NO,
                     G_Reading.ACCOUNT_CREATED,
                     G_Reading.STATEMENT_ID,
                     G_Reading.BILL_DATE,
                     G_Reading.MPAN_MPRN
                 }
#if WINFORMS
                 where G_Reading.READINGS_PERIOD_START >= utilityviewmodel.StartDate &&
                       G_Reading.READINGS_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where G_Reading.READINGS_PERIOD_START >= utilityviewmodel.StartDate &&
                       G_Reading.READINGS_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where G_Reading.READINGS_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                       G_Reading.READINGS_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                 orderby G_Reading.BILL_DATE descending,
                     G_Reading.READINGS_PERIOD_START ascending,
                     G_Reading.READINGS_PERIOD_END ascending // See above
                                                             // This pile of bollocks picks out the LAST (i.e. highest BILL_DATE, PERIOD_START, PERIOD_END) from E_Reading
                                                             // Took me all fucking day ....
                                                             //where //Bill_Resource.USERNAME == ourviewmodel.UserName &&
                                                             //      Bill_Resource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 select G_Reading);
            g_readings_found = new List<SmartUtility.GReadings>
                (g_readings_found.GroupBy(p => new
                {
                    p.READINGS_PERIOD_START,
                    p.READINGS_PERIOD_END
                }).Select(q => q.First()).OrderBy(r => r.READINGS_PERIOD_START));
            return g_readings_found;
        }

        internal static List<SmartUtility.EUnitCharges> Utility_EUnitCharges(UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.EUnitCharges> e_unit_charges_found = // Note: This is ACROSS all Suppliers ... and all MPANs
                new List<SmartUtility.EUnitCharges>
                (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 join E_Unit_Charge in utilityviewmodel.Hezbollah.e_unit_chargesList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     E_Unit_Charge.USERNAME,
                     E_Unit_Charge.CUBEFACE_CODE,
                     E_Unit_Charge.SUPPLIER_CODE,
                     E_Unit_Charge.BRAND_CODE,
                     E_Unit_Charge.ACCOUNT_NO,
                     E_Unit_Charge.ACCOUNT_CREATED,
                     E_Unit_Charge.STATEMENT_ID,
                     E_Unit_Charge.BILL_DATE,
                     E_Unit_Charge.MPAN_MPRN
                 }
#if WINFORMS
                 where E_Unit_Charge.UNIT_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                       E_Unit_Charge.UNIT_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where E_Unit_Charge.UNIT_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                       E_Unit_Charge.UNIT_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where E_Unit_Charge.UNIT_CHARGES_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                       E_Unit_Charge.UNIT_CHARGES_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                 orderby E_Unit_Charge.BILL_DATE descending,
                             E_Unit_Charge.UNIT_CHARGES_PERIOD_START ascending,
                             E_Unit_Charge.UNIT_CHARGES_PERIOD_END ascending // So matching START/END dates come before unmatched START/END dates
                 //where //Bill_Resource.USERNAME == ourviewmodel.UserName &&
                 //      Bill_Resource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 select E_Unit_Charge);
            e_unit_charges_found =
                new List<SmartUtility.EUnitCharges>(e_unit_charges_found.GroupBy(p => new
                {
                    p.UNIT_CHARGES_PERIOD_START,
                    p.UNIT_CHARGES_PERIOD_END
                }).Select(q => q.First()).OrderBy(r => r.UNIT_CHARGES_PERIOD_START));
            return e_unit_charges_found;
        }

        internal static List<SmartUtility.GUnitCharges> Utility_GUnitCharges(UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.GUnitCharges> g_unit_charges_found =
                new List<SmartUtility.GUnitCharges>
                (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 join G_Unit_Charge in utilityviewmodel.Hezbollah.g_unit_chargesList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     G_Unit_Charge.USERNAME,
                     G_Unit_Charge.CUBEFACE_CODE,
                     G_Unit_Charge.SUPPLIER_CODE,
                     G_Unit_Charge.BRAND_CODE,
                     G_Unit_Charge.ACCOUNT_NO,
                     G_Unit_Charge.ACCOUNT_CREATED,
                     G_Unit_Charge.STATEMENT_ID,
                     G_Unit_Charge.BILL_DATE,
                     G_Unit_Charge.MPAN_MPRN
                 }
#if WINFORMS
                 where G_Unit_Charge.UNIT_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                       G_Unit_Charge.UNIT_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where G_Unit_Charge.UNIT_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                       G_Unit_Charge.UNIT_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where G_Unit_Charge.UNIT_CHARGES_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                       G_Unit_Charge.UNIT_CHARGES_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                 orderby G_Unit_Charge.BILL_DATE descending,
                         G_Unit_Charge.UNIT_CHARGES_PERIOD_START ascending,
                         G_Unit_Charge.UNIT_CHARGES_PERIOD_END ascending // See above
                 //where //Bill_Resource.USERNAME == ourviewmodel.UserName &&
                 //      Bill_Resource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 select G_Unit_Charge);
            g_unit_charges_found =
                new List<SmartUtility.GUnitCharges>
                (g_unit_charges_found.GroupBy(p => new
                {
                    p.UNIT_CHARGES_PERIOD_START,
                    p.UNIT_CHARGES_PERIOD_END
                }).Select(q => q.First()).OrderBy(r => r.UNIT_CHARGES_PERIOD_START));
            return g_unit_charges_found;
        }

        internal static List<SmartUtility.EStandingCharges> Utility_EStandingCharges(UtilityViewModel utilityviewmodel,
                                                                                                string statement_id = "")
        {
            List<SmartUtility.EStandingCharges> e_standing_charges_found = new List<SmartUtility.EStandingCharges>();
            if (string.IsNullOrEmpty(statement_id))
            {
                e_standing_charges_found = new List<SmartUtility.EStandingCharges>
                    (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                     join E_Standing_Charge in utilityviewmodel.Hezbollah.e_standing_chargesList
                     on new
                     {
                         Bill_Resource.USERNAME,
                         Bill_Resource.CUBEFACE_CODE,
                         Bill_Resource.SUPPLIER_CODE,
                         Bill_Resource.BRAND_CODE,
                         Bill_Resource.ACCOUNT_NO,
                         Bill_Resource.ACCOUNT_CREATED,
                         Bill_Resource.STATEMENT_ID,
                         Bill_Resource.BILL_DATE,
                         Bill_Resource.MPAN_MPRN
                     }
                     equals new
                     {
                         E_Standing_Charge.USERNAME,
                         E_Standing_Charge.CUBEFACE_CODE,
                         E_Standing_Charge.SUPPLIER_CODE,
                         E_Standing_Charge.BRAND_CODE,
                         E_Standing_Charge.ACCOUNT_NO,
                         E_Standing_Charge.ACCOUNT_CREATED,
                         E_Standing_Charge.STATEMENT_ID,
                         E_Standing_Charge.BILL_DATE,
                         E_Standing_Charge.MPAN_MPRN
                     }
#if WINFORMS
                     where E_Standing_Charge.STANDING_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                            E_Standing_Charge.STANDING_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                     where E_Standing_Charge.STANDING_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                            E_Standing_Charge.STANDING_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                     where E_Standing_Charge.STANDING_CHARGES_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                            E_Standing_Charge.STANDING_CHARGES_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                     // This pile of bollocks picks out the LAST (i.e. highest BILL_DATE, FROM_DATE, CHARGES_ITEM) from E_Standing_Charges
                     // Took me all fucking day ....
                     //orderby E_Standing_Charge.BILL_DATE descending, E_Standing_Charge.FROM_DATE ascending, E_Standing_Charge.READ_DATE ascending // So matching FROM/READ dates come before unmatch FROM/READ dates
                     select E_Standing_Charge);
            }
            else
            {
                e_standing_charges_found = new List<SmartUtility.EStandingCharges>
                    (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                     join E_Standing_Charge in utilityviewmodel.Hezbollah.e_standing_chargesList
                     on new
                     {
                         Bill_Resource.USERNAME,
                         Bill_Resource.CUBEFACE_CODE,
                         Bill_Resource.SUPPLIER_CODE,
                         Bill_Resource.BRAND_CODE,
                         Bill_Resource.ACCOUNT_NO,
                         Bill_Resource.ACCOUNT_CREATED,
                         Bill_Resource.STATEMENT_ID,
                         Bill_Resource.BILL_DATE,
                         Bill_Resource.MPAN_MPRN
                     }
                     equals new
                     {
                         E_Standing_Charge.USERNAME,
                         E_Standing_Charge.CUBEFACE_CODE,
                         E_Standing_Charge.SUPPLIER_CODE,
                         E_Standing_Charge.BRAND_CODE,
                         E_Standing_Charge.ACCOUNT_NO,
                         E_Standing_Charge.ACCOUNT_CREATED,
                         E_Standing_Charge.STATEMENT_ID,
                         E_Standing_Charge.BILL_DATE,
                         E_Standing_Charge.MPAN_MPRN
                     }
                     where Bill_Resource.STATEMENT_ID == statement_id
                     // This pile of bollocks picks out the LAST (i.e. highest BILL_DATE, FROM_DATE, CHARGES_ITEM) from E_Standing_Charges
                     // Took me all fucking day ....
                     //orderby E_Standing_Charge.BILL_DATE descending, E_Standing_Charge.FROM_DATE ascending, E_Standing_Charge.READ_DATE ascending // So matching FROM/READ dates come before unmatch FROM/READ dates
                     select E_Standing_Charge);
            }
            return e_standing_charges_found;
        }

        internal static List<SmartUtility.GStandingCharges> Utility_GStandingCharges(UtilityViewModel utilityviewmodel,
                                                                                                        string statement_id = "")
        {
            List<SmartUtility.GStandingCharges> g_standing_charges_found = new List<SmartUtility.GStandingCharges>();
            if (string.IsNullOrEmpty(statement_id))
            {
                g_standing_charges_found = new List<SmartUtility.GStandingCharges>
                    (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                     join G_Standing_Charge in utilityviewmodel.Hezbollah.g_standing_chargesList
                     on new
                     {
                         Bill_Resource.USERNAME,
                         Bill_Resource.CUBEFACE_CODE,
                         Bill_Resource.SUPPLIER_CODE,
                         Bill_Resource.BRAND_CODE,
                         Bill_Resource.ACCOUNT_NO,
                         Bill_Resource.ACCOUNT_CREATED,
                         Bill_Resource.STATEMENT_ID,
                         Bill_Resource.BILL_DATE,
                         Bill_Resource.MPAN_MPRN
                     }
                     equals new
                     {
                         G_Standing_Charge.USERNAME,
                         G_Standing_Charge.CUBEFACE_CODE,
                         G_Standing_Charge.SUPPLIER_CODE,
                         G_Standing_Charge.BRAND_CODE,
                         G_Standing_Charge.ACCOUNT_NO,
                         G_Standing_Charge.ACCOUNT_CREATED,
                         G_Standing_Charge.STATEMENT_ID,
                         G_Standing_Charge.BILL_DATE,
                         G_Standing_Charge.MPAN_MPRN
                     }
#if WINFORMS
                     where G_Standing_Charge.STANDING_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                           G_Standing_Charge.STANDING_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                     where G_Standing_Charge.STANDING_CHARGES_PERIOD_START >= utilityviewmodel.StartDate &&
                           G_Standing_Charge.STANDING_CHARGES_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                     where G_Standing_Charge.STANDING_CHARGES_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                           G_Standing_Charge.STANDING_CHARGES_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                     // This pile of bollocks picks out the LAST (i.e. highest BILL_DATE, FROM_DATE, CHARGES_ITEM) from G_Standing_Charges
                     // Took me all fucking day ....
                     //orderby G_Standing_Charge.BILL_DATE descending, G_Standing_Charge.FROM_DATE ascending, G_Standing_Charge.READ_DATE ascending // So matching FROM/READ dates come before unmatch FROM/READ dates
                     select G_Standing_Charge);
            }
            else
            {
                g_standing_charges_found = new List<SmartUtility.GStandingCharges>
                (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 join G_Standing_Charge in utilityviewmodel.Hezbollah.g_standing_chargesList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,       // This order is correct
                     Bill_Resource.ACCOUNT_CREATED,  // This order is correct
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN,
                 }
                 equals new
                 {
                     G_Standing_Charge.USERNAME,
                     G_Standing_Charge.CUBEFACE_CODE,
                     G_Standing_Charge.SUPPLIER_CODE,
                     G_Standing_Charge.BRAND_CODE,
                     G_Standing_Charge.ACCOUNT_NO,
                     G_Standing_Charge.ACCOUNT_CREATED,
                     G_Standing_Charge.STATEMENT_ID,
                     G_Standing_Charge.BILL_DATE,
                     G_Standing_Charge.MPAN_MPRN
                 }
                 where Bill_Resource.STATEMENT_ID == statement_id
                 // This pile of bollocks picks out the LAST (i.e. highest BILL_DATE, FROM_DATE, CHARGES_ITEM) from G_Standing_Charges
                 // Took me all fucking day ....
                 //orderby G_Standing_Charge.BILL_DATE descending, G_Standing_Charge.FROM_DATE ascending, G_Standing_Charge.READ_DATE ascending // So matching FROM/READ dates come before unmatch FROM/READ dates
                 select G_Standing_Charge);
            }
            return g_standing_charges_found;
        }

        internal static List<SmartUtility.EDiscounts> Utility_EDiscounts(UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.EDiscounts> e_discounts_found =
                new List<SmartUtility.EDiscounts>
                (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 join E_Discounts in utilityviewmodel.Hezbollah.e_discountsList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     E_Discounts.USERNAME,
                     E_Discounts.CUBEFACE_CODE,
                     E_Discounts.SUPPLIER_CODE,
                     E_Discounts.BRAND_CODE,
                     E_Discounts.ACCOUNT_NO,
                     E_Discounts.ACCOUNT_CREATED,
                     E_Discounts.STATEMENT_ID,
                     E_Discounts.BILL_DATE,
                     E_Discounts.MPAN_MPRN
                 }
#if WINFORMS
                 where E_Discounts.DISCOUNT_DATE >= utilityviewmodel.StartDate &&
                       E_Discounts.DISCOUNT_DATE <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where E_Discounts.DISCOUNT_DATE >= utilityviewmodel.StartDate &&
                       E_Discounts.DISCOUNT_DATE <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where E_Discounts.DISCOUNT_DATE >= utilityviewmodel.StartDate.DateTime &&
                       E_Discounts.DISCOUNT_DATE <= utilityviewmodel.EndDate.DateTime
#endif
                 //where //Bill_Resource.USERNAME == ourviewmodel.UserName &&
                 //    Bill_Resource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 orderby E_Discounts.DISCOUNT_DATE ascending
                 select E_Discounts);
            return e_discounts_found;
        }

        internal static List<SmartUtility.GDiscounts> Utility_GDiscounts(UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.GDiscounts> g_discounts_found =
                new List<SmartUtility.GDiscounts>
                (from Bill_Resource in utilityviewmodel.Hezbollah.bills_resourceList
                 join G_Discounts in utilityviewmodel.Hezbollah.g_discountsList
                 on new
                 {
                     Bill_Resource.USERNAME,
                     Bill_Resource.CUBEFACE_CODE,
                     Bill_Resource.SUPPLIER_CODE,
                     Bill_Resource.BRAND_CODE,
                     Bill_Resource.ACCOUNT_NO,
                     Bill_Resource.ACCOUNT_CREATED,
                     Bill_Resource.STATEMENT_ID,
                     Bill_Resource.BILL_DATE,
                     Bill_Resource.MPAN_MPRN
                 }
                 equals new
                 {
                     G_Discounts.USERNAME,
                     G_Discounts.CUBEFACE_CODE,
                     G_Discounts.SUPPLIER_CODE,
                     G_Discounts.BRAND_CODE,
                     G_Discounts.ACCOUNT_NO,
                     G_Discounts.ACCOUNT_CREATED,
                     G_Discounts.STATEMENT_ID,
                     G_Discounts.BILL_DATE,
                     G_Discounts.MPAN_MPRN
                 }
#if WINFORMS
                 where G_Discounts.DISCOUNT_DATE >= utilityviewmodel.StartDate &&
                       G_Discounts.DISCOUNT_DATE <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where G_Discounts.DISCOUNT_DATE >= utilityviewmodel.StartDate &&
                       G_Discounts.DISCOUNT_DATE <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where G_Discounts.DISCOUNT_DATE >= utilityviewmodel.StartDate.DateTime &&
                       G_Discounts.DISCOUNT_DATE <= utilityviewmodel.EndDate.DateTime
#endif
                 //where //Bill_Resource.USERNAME == ourviewmodel.UserName &&
                 //      Bill_Resource.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 orderby G_Discounts.DISCOUNT_DATE ascending
                 select G_Discounts);
            return g_discounts_found;
        }

        internal static List<SmartUtility.EDiscounts> Utility_EDiscountsFound(UtilityViewModel utilityviewmodel,
                                                            short supplier_code,
                                                            short brand_code,
                                                            string account_no,
                                                            DateTime created,
                                                            string statement_id,
                                                            DateTime bill_date,
                                                            DateTime discount_date,
                                                            short discount_item)
        {
            List<SmartUtility.EDiscounts> e_discounts_found =
                new List<SmartUtility.EDiscounts>
                (from E_Discount in utilityviewmodel.Hezbollah.e_discounts_changesList
                 where (E_Discount.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                         E_Discount.SUPPLIER_CODE == supplier_code &&
                         E_Discount.BRAND_CODE == brand_code &&
                         E_Discount.ACCOUNT_NO == account_no &&
                         E_Discount.ACCOUNT_CREATED == created &&
                         E_Discount.STATEMENT_ID == statement_id &&
                         E_Discount.BILL_DATE == bill_date &&
                         E_Discount.DISCOUNT_DATE == discount_date &&
                         E_Discount.DISCOUNT_ITEM == discount_item)
                 select E_Discount);
            return e_discounts_found;
        }

        internal static List<SmartUtility.GDiscounts> Utility_GDiscountsFound(UtilityViewModel utilityviewmodel,
                                                            short supplier_code,
                                                            short brand_code,
                                                            string account_no,
                                                            DateTime created,
                                                            string statement_id,
                                                            DateTime bill_date,
                                                            DateTime discount_date,
                                                            short discount_item)
        {

            List<SmartUtility.GDiscounts> g_discounts_found =
                new List<SmartUtility.GDiscounts>
                (from G_Discount in utilityviewmodel.Hezbollah.g_discounts_changesList
                 where (G_Discount.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                         G_Discount.SUPPLIER_CODE == supplier_code &&
                         G_Discount.BRAND_CODE == brand_code &&
                         G_Discount.ACCOUNT_NO == account_no &&
                         G_Discount.ACCOUNT_CREATED == created &&
                         G_Discount.STATEMENT_ID == statement_id &&
                         G_Discount.BILL_DATE == bill_date &&
                         G_Discount.DISCOUNT_DATE == discount_date &&
                         G_Discount.DISCOUNT_ITEM == discount_item)
                 select G_Discount);
            return g_discounts_found;
        }

        internal static List<SmartUtility.EUsage> Utility_EUsage(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.EUsage> e_usage_found =
                new List<SmartUtility.EUsage>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                 join E_Usage in utilityviewmodel.Hezbollah.e_usageList
                 on new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
                 equals new { E_Usage.USERNAME, E_Usage.CUBEFACE_CODE, E_Usage.MPAN_MPRN }
                 orderby E_Usage.USERNAME,
                         E_Usage.UNIQUE_INDEX ascending
                 select E_Usage);
            e_usage_found = new List<SmartUtility.EUsage>(e_usage_found.Distinct());
            return e_usage_found;
        }

        internal static List<SmartUtility.GUsage> Utility_GUsage(MainViewModel ourviewmodel,
                                                                        UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.GUsage> g_usage_found =
                new List<SmartUtility.GUsage>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Meter in utilityviewmodel.Hezbollah.utility_metersList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                 join G_Usage in utilityviewmodel.Hezbollah.g_usageList
                 on new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
                 equals new { G_Usage.USERNAME, G_Usage.CUBEFACE_CODE, G_Usage.MPAN_MPRN }
                 orderby G_Usage.USERNAME,
                            G_Usage.UNIQUE_INDEX ascending
                 select G_Usage);
            g_usage_found = new List<SmartUtility.GUsage>(g_usage_found.Distinct());
            return g_usage_found;
        }

        internal static List<SmartUtility.Payments> Utility_PaymentsIn(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    short supplier_code,
                                                    short brand_code,
                                                    string account_no,
                                                    DateTime created,
                                                    string statement_id,
                                                    DateTime bill_date,
                                                    DateTime payment_date)
        {
            List<SmartUtility.Payments> payments_in_found =
                new List<SmartUtility.Payments>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Payment in utilityviewmodel.Hezbollah.paymentsList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Payment.USERNAME, Payment.CUBEFACE_CODE }
                 where Payment.SUPPLIER_CODE == supplier_code &&
                         Payment.BRAND_CODE == brand_code &&
                         Payment.ACCOUNT_NO == account_no &&
                         Payment.ACCOUNT_CREATED == created &&
                         Payment.STATEMENT_ID == statement_id &&
                         Payment.BILL_DATE == bill_date &&
                         Payment.PAYMENT_DATE == payment_date
                 orderby Payment.PAYMENT_DATE ascending
                 select Payment);
            payments_in_found = new List<SmartUtility.Payments>(payments_in_found.Distinct());
            return payments_in_found;
        }

        internal static List<SmartUtility.Payments> Utility_PaymentsOut(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    short supplier_code,
                                                    short brand_code,
                                                    string account_no,
                                                    DateTime created,
                                                    string statement_id,
                                                    DateTime bill_date,
                                                    DateTime payment_date)
        {
            List<SmartUtility.Payments> payments_out_found =
                new List<SmartUtility.Payments>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Payment in utilityviewmodel.Hezbollah.payments_changesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Payment.USERNAME, Payment.CUBEFACE_CODE }
                 where Payment.SUPPLIER_CODE == supplier_code &&
                         Payment.BRAND_CODE == brand_code &&
                         Payment.ACCOUNT_NO == account_no &&
                         Payment.ACCOUNT_CREATED == created &&
                         Payment.STATEMENT_ID == statement_id &&
                         Payment.BILL_DATE == bill_date &&
                         Payment.PAYMENT_DATE == payment_date
                 orderby Payment.PAYMENT_ITEM descending
                 select Payment);
            payments_out_found = new List<SmartUtility.Payments>(payments_out_found.Distinct());
            return payments_out_found;
        }
        internal static List<SmartUtility.Unallocated> Utility_PaymentsUnallocated(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                List<SmartUtility.Unallocated> unallocatedList)
        {
            // NO Bills to go to!!!!

            List<SmartUtility.Bills> bills_found =
                Utility_Configure_Bills(ourviewmodel,
                                utilityviewmodel);
            //SmartParametersV2016.defaultDate);
            //               string statement_id,
            //               DateTime bill_date)

            // Note: This is ACROSS all Suppliers ... and all MPANs
            List<SmartUtility.Unallocated> unallocated_found =
                new List<SmartUtility.Unallocated>
                (from Bill in bills_found
                 join Unallocate in unallocatedList
                 on new
                 {
                     Bill.USERNAME,
                     Bill.CUBEFACE_CODE,
                     Bill.SUPPLIER_CODE,
                     Bill.BRAND_CODE,
                     Bill.ACCOUNT_CREATED,
                     Bill.ACCOUNT_NO
                 }
                 equals new
                 {
                     Unallocate.USERNAME,
                     Unallocate.CUBEFACE_CODE,
                     Unallocate.SUPPLIER_CODE,
                     Unallocate.BRAND_CODE,
                     Unallocate.ACCOUNT_CREATED,
                     Unallocate.ACCOUNT_NO
                 }
#if WINFORMS
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate &&
#endif
#if WPF  || WINUI || SMARTMAUI
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate &&
#endif
#if ANDROIDX
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate.DateTime &&
#endif
                     //Unallocate.USERNAME == ourviewmodel.UserName &&
                     //Unallocate.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                     (string.IsNullOrEmpty(Unallocate.STATEMENT_ID) ||
                     Unallocate.BILL_DATE == SmartParametersV2016.defaultDate)
                 orderby Unallocate.PAYMENT_DATE ascending
                 select Unallocate);
            return unallocated_found;
        }

        internal static List<SmartUtility.Payments> Utility_PaymentsAllocated(UtilityViewModel utilityviewmodel,
                                                            List<SmartUtility.BillsResource> working_bills_resourceList,
                                                            List<SmartUtility.Payments> paymentsList)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs
            List<SmartUtility.Payments> payments_view_found =
                new List<SmartUtility.Payments>
                (from Bill in working_bills_resourceList
                 join Payment in paymentsList
                 on new
                 {
                     Bill.USERNAME,
                     Bill.CUBEFACE_CODE,
                     Bill.SUPPLIER_CODE,
                     Bill.BRAND_CODE,
                     Bill.ACCOUNT_NO,
                     Bill.ACCOUNT_CREATED,
                     Bill.STATEMENT_ID,
                     Bill.BILL_DATE
                 }
                 equals new
                 {
                     Payment.USERNAME,
                     Payment.CUBEFACE_CODE,
                     Payment.SUPPLIER_CODE,
                     Payment.BRAND_CODE,
                     Payment.ACCOUNT_NO,
                     Payment.ACCOUNT_CREATED,
                     Payment.STATEMENT_ID,
                     Payment.BILL_DATE
                 }
#if WINFORMS
                 where Payment.PAYMENT_DATE >= utilityviewmodel.StartDate &&
                       Payment.PAYMENT_DATE <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where Payment.PAYMENT_DATE >= utilityviewmodel.StartDate &&
                       Payment.PAYMENT_DATE <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where Payment.PAYMENT_DATE >= utilityviewmodel.StartDate.DateTime &&
                       Payment.PAYMENT_DATE <= utilityviewmodel.EndDate.DateTime
#endif
                 //where //Bill.USERNAME == ourviewmodel.UserName &&
                 //      Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 orderby Payment.PAYMENT_DATE ascending
                 select Payment);
            return payments_view_found;
        }

        internal static List<SmartUtility.SupChargesCredits> Utility_SupplyChargesCreditsList(UtilityViewModel utilityviewmodel,
                                                                                List<SmartUtility.Bills> working_billsList,
                                                                                List<SmartUtility.SupChargesCredits> supply_charges_creditsList)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs

            List<SmartUtility.SupChargesCredits> supply_charges_credits_found =
                new List<SmartUtility.SupChargesCredits>
                (from SupplyCharges in supply_charges_creditsList
                 join Bill in working_billsList
                 on new
                 {
                     SupplyCharges.USERNAME,
                     SupplyCharges.CUBEFACE_CODE,
                     SupplyCharges.SUPPLIER_CODE,
                     SupplyCharges.BRAND_CODE,
                     SupplyCharges.ACCOUNT_NO,
                     SupplyCharges.ACCOUNT_CREATED,
                     SupplyCharges.STATEMENT_ID,
                     SupplyCharges.BILL_DATE
                 }
                 equals new
                 {
                     Bill.USERNAME,
                     Bill.CUBEFACE_CODE,
                     Bill.SUPPLIER_CODE,
                     Bill.BRAND_CODE,
                     Bill.ACCOUNT_NO,
                     Bill.ACCOUNT_CREATED,
                     Bill.STATEMENT_ID,
                     Bill.BILL_DATE
                 }
#if WINFORMS
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                 //where //Bill.USERNAME == ourviewmodel.UserName &&
                 //    Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 orderby SupplyCharges.SUPPLY_DATE ascending
                 select SupplyCharges);
            return supply_charges_credits_found;
        }

        internal static List<SmartUtility.AccChargesCredits> Utility_AccountChargesCreditsList(UtilityViewModel utilityviewmodel,
                                                                                List<SmartUtility.Bills> working_billsList,
                                                                                List<SmartUtility.AccChargesCredits> account_charges_creditsList)
        {
            // Note: This is ACROSS all Suppliers ... and all MPANs
            List<SmartUtility.AccChargesCredits> account_charges_credits_found =
                new List<SmartUtility.AccChargesCredits>
                (from AccountCharges in account_charges_creditsList
                 join Bill in working_billsList
                 on new
                 {
                     AccountCharges.USERNAME,
                     AccountCharges.CUBEFACE_CODE,
                     AccountCharges.SUPPLIER_CODE,
                     AccountCharges.BRAND_CODE,
                     AccountCharges.ACCOUNT_NO,
                     AccountCharges.ACCOUNT_CREATED,
                     AccountCharges.STATEMENT_ID,
                     AccountCharges.BILL_DATE
                 }
                 equals new
                 {
                     Bill.USERNAME,
                     Bill.CUBEFACE_CODE,
                     Bill.SUPPLIER_CODE,
                     Bill.BRAND_CODE,
                     Bill.ACCOUNT_NO,
                     Bill.ACCOUNT_CREATED,
                     Bill.STATEMENT_ID,
                     Bill.BILL_DATE
                 }
#if WINFORMS
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if WPF  || WINUI || SMARTMAUI
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate
#endif
#if ANDROIDX
                 where Bill.BILL_PERIOD_START >= utilityviewmodel.StartDate.DateTime &&
                       Bill.BILL_PERIOD_END <= utilityviewmodel.EndDate.DateTime
#endif
                 //where //Bill.USERNAME == ourviewmodel.UserName &&
                 //    Bill.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                 orderby AccountCharges.ACCOUNT_DATE ascending
                 select AccountCharges);

            return account_charges_credits_found;
        }

        internal static List<SmartUtility.EReadings> Utility_Find_EReadings(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.EReadings> e_readingsList)
        {
            List<SmartUtility.EReadings> e_readings_found =
                new List<SmartUtility.EReadings>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join E_Reading in e_readingsList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { E_Reading.USERNAME, E_Reading.CUBEFACE_CODE }
                 where E_Reading.CUBEFACE_CODE == cubeface_code &&
                     E_Reading.SUPPLIER_CODE == supplier_code &&
                     E_Reading.BRAND_CODE == brand_code &&
                     E_Reading.ACCOUNT_NO == account_no &&
                     E_Reading.ACCOUNT_CREATED == created &&
                     E_Reading.STATEMENT_ID == statement_id &&
                     E_Reading.BILL_DATE == bill_date &&
                     E_Reading.MPAN_MPRN == mpan_mprn
                 orderby E_Reading.READINGS_PERIOD_END ascending
                 select E_Reading);
            e_readings_found = new List<SmartUtility.EReadings>(e_readings_found.Distinct());
            return e_readings_found;
        }

        internal static List<SmartUtility.EUnitCharges> Utility_Find_EUnitCharges(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.EUnitCharges> e_unit_chargesList)
        {
            List<SmartUtility.EUnitCharges> e_unit_charges_found =
                new List<SmartUtility.EUnitCharges>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join E_Unit_Charge in e_unit_chargesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { E_Unit_Charge.USERNAME, E_Unit_Charge.CUBEFACE_CODE }
                 where E_Unit_Charge.CUBEFACE_CODE == cubeface_code &&
                         E_Unit_Charge.SUPPLIER_CODE == supplier_code &&
                         E_Unit_Charge.BRAND_CODE == brand_code &&
                         E_Unit_Charge.ACCOUNT_NO == account_no &&
                         E_Unit_Charge.ACCOUNT_CREATED == created &&
                         E_Unit_Charge.STATEMENT_ID == statement_id &&
                         E_Unit_Charge.BILL_DATE == bill_date &&
                         E_Unit_Charge.MPAN_MPRN == mpan_mprn
                 select E_Unit_Charge);
            e_unit_charges_found = new List<SmartUtility.EUnitCharges>(e_unit_charges_found.Distinct());
            return e_unit_charges_found;
        }

        internal static List<SmartUtility.EStandingCharges> Utility_Find_EStandingCharges(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.EStandingCharges> e_standing_chargesList)
        {
            List<SmartUtility.EStandingCharges> e_standing_charges_found =
                new List<SmartUtility.EStandingCharges>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join E_Standing_Charge in e_standing_chargesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { E_Standing_Charge.USERNAME, E_Standing_Charge.CUBEFACE_CODE }
                 where E_Standing_Charge.CUBEFACE_CODE == cubeface_code &&
                         E_Standing_Charge.SUPPLIER_CODE == supplier_code &&
                         E_Standing_Charge.BRAND_CODE == brand_code &&
                         E_Standing_Charge.ACCOUNT_NO == account_no &&
                         E_Standing_Charge.ACCOUNT_CREATED == created &&
                         E_Standing_Charge.STATEMENT_ID == statement_id &&
                         E_Standing_Charge.BILL_DATE == bill_date &&
                         E_Standing_Charge.MPAN_MPRN == mpan_mprn
                 select E_Standing_Charge);
            e_standing_charges_found = new List<SmartUtility.EStandingCharges>(e_standing_charges_found.Distinct());
            return e_standing_charges_found;
        }

        internal static List<SmartUtility.EDiscounts> Utility_Find_EDiscounts(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.EDiscounts> e_discountsList)
        {
            List<SmartUtility.EDiscounts> e_discounts_found =
                new List<SmartUtility.EDiscounts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join E_Discount in e_discountsList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { E_Discount.USERNAME, E_Discount.CUBEFACE_CODE }
                 where E_Discount.CUBEFACE_CODE == cubeface_code &&
                     E_Discount.SUPPLIER_CODE == supplier_code &&
                     E_Discount.BRAND_CODE == brand_code &&
                     E_Discount.ACCOUNT_NO == account_no &&
                     E_Discount.ACCOUNT_CREATED == created &&
                     E_Discount.STATEMENT_ID == statement_id &&
                     E_Discount.BILL_DATE == bill_date &&
                     E_Discount.MPAN_MPRN == mpan_mprn
                 select E_Discount);
            e_discounts_found = new List<SmartUtility.EDiscounts>(e_discounts_found.Distinct());
            return e_discounts_found;
        }

        internal static List<SmartUtility.GReadings> Utility_Find_GReadings(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.GReadings> g_readingsList)
        {
            List<SmartUtility.GReadings> g_readings_found =
                new List<SmartUtility.GReadings>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join G_Reading in g_readingsList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { G_Reading.USERNAME, G_Reading.CUBEFACE_CODE }
                 where G_Reading.CUBEFACE_CODE == cubeface_code &&
                     G_Reading.SUPPLIER_CODE == supplier_code &&
                     G_Reading.BRAND_CODE == brand_code &&
                     G_Reading.ACCOUNT_NO == account_no &&
                     G_Reading.ACCOUNT_CREATED == created &&
                     G_Reading.STATEMENT_ID == statement_id &&
                     G_Reading.BILL_DATE == bill_date &&
                     G_Reading.MPAN_MPRN == mpan_mprn
                 orderby G_Reading.READINGS_PERIOD_END ascending
                 select G_Reading);
            g_readings_found = new List<SmartUtility.GReadings>(g_readings_found.Distinct());
            return g_readings_found;
        }

        internal static List<SmartUtility.GUnitCharges> Utility_Find_GUnitCharges(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.GUnitCharges> g_unit_chargesList)
        {
            List<SmartUtility.GUnitCharges> g_unit_charges_found =
                new List<SmartUtility.GUnitCharges>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join G_Unit_Charge in g_unit_chargesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { G_Unit_Charge.USERNAME, G_Unit_Charge.CUBEFACE_CODE }
                 where G_Unit_Charge.CUBEFACE_CODE == cubeface_code &&
                         G_Unit_Charge.SUPPLIER_CODE == supplier_code &&
                         G_Unit_Charge.BRAND_CODE == brand_code &&
                         G_Unit_Charge.ACCOUNT_NO == account_no &&
                         G_Unit_Charge.ACCOUNT_CREATED == created &&
                         G_Unit_Charge.STATEMENT_ID == statement_id &&
                         G_Unit_Charge.BILL_DATE == bill_date &&
                         G_Unit_Charge.MPAN_MPRN == mpan_mprn
                 select G_Unit_Charge);
            g_unit_charges_found = new List<SmartUtility.GUnitCharges>(g_unit_charges_found.Distinct());
            return g_unit_charges_found;
        }

        internal static List<SmartUtility.GStandingCharges> Utility_Find_GStandingCharges(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.GStandingCharges> g_standing_chargesList)
        {
            List<SmartUtility.GStandingCharges> g_standing_charges_found =
                new List<SmartUtility.GStandingCharges>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join G_Standing_Charge in g_standing_chargesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { G_Standing_Charge.USERNAME, G_Standing_Charge.CUBEFACE_CODE }
                 where G_Standing_Charge.CUBEFACE_CODE == cubeface_code &&
                         G_Standing_Charge.SUPPLIER_CODE == supplier_code &&
                         G_Standing_Charge.BRAND_CODE == brand_code &&
                         G_Standing_Charge.ACCOUNT_NO == account_no &&
                         G_Standing_Charge.ACCOUNT_CREATED == created &&
                         G_Standing_Charge.STATEMENT_ID == statement_id &&
                         G_Standing_Charge.BILL_DATE == bill_date &&
                         G_Standing_Charge.MPAN_MPRN == mpan_mprn
                 select G_Standing_Charge);
            g_standing_charges_found = new List<SmartUtility.GStandingCharges>(g_standing_charges_found.Distinct());
            return g_standing_charges_found;
        }

        internal static List<SmartUtility.GDiscounts> Utility_Find_GDiscounts(MainViewModel ourviewmodel,
                                                        char cubeface_code,
                                                        short supplier_code,
                                                        short brand_code,
                                                        string account_no,
                                                        DateTime created,
                                                        string statement_id,
                                                        DateTime bill_date,
                                                        string mpan_mprn,
                                                        List<SmartUtility.GDiscounts> g_discountsList)
        {
            List<SmartUtility.GDiscounts> g_discounts_found =
                new List<SmartUtility.GDiscounts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join G_Discount in g_discountsList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { G_Discount.USERNAME, G_Discount.CUBEFACE_CODE }
                 where G_Discount.CUBEFACE_CODE == cubeface_code &&
                         G_Discount.SUPPLIER_CODE == supplier_code &&
                         G_Discount.BRAND_CODE == brand_code &&
                         G_Discount.ACCOUNT_NO == account_no &&
                         G_Discount.ACCOUNT_CREATED == created &&
                         G_Discount.STATEMENT_ID == statement_id &&
                         G_Discount.BILL_DATE == bill_date &&
                         G_Discount.MPAN_MPRN == mpan_mprn
                 select G_Discount);
            g_discounts_found = new List<SmartUtility.GDiscounts>(g_discounts_found.Distinct());
            return g_discounts_found;
        }

        internal static List<SmartUtility.TariffMatrix> Utility_TariffMatrixList(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            char resource_code,
                                                            string resource_type,
                                                            short brand_code,
                                                            int tariff_code)
        {
            List<SmartUtility.TariffMatrix> tariff_matrix_found = new List<SmartUtility.TariffMatrix>();
            if (tariff_code == 0)
            {
                tariff_matrix_found = new List<SmartUtility.TariffMatrix>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff_Matrix in utilityviewmodel.Hezbollah.tariff_matrixList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff_Matrix.CUBEFACE_CODE }
                     where (Tariff_Matrix.BRAND_CODE == brand_code) &&
                             (Tariff_Matrix.RESOURCE_CODE == resource_code) &&
                             (Tariff_Matrix.RESOURCE_TYPE == resource_type) &&
                             (Tariff_Matrix.PLACEHOLDER == SmartParametersV2016.noFlag)
                     select Tariff_Matrix);
            }
            else
            {
                if (resource_code == SmartParametersV2016.DualFuel)
                {
                    tariff_matrix_found = new List<SmartUtility.TariffMatrix>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Tariff_Matrix in utilityviewmodel.Hezbollah.tariff_matrixList
                         on new { Cubeface.CUBEFACE_CODE }
                         equals new { Tariff_Matrix.CUBEFACE_CODE }
                         where (Tariff_Matrix.BRAND_CODE == brand_code) &&
                                 //(Tariff_Matrix.RESOURCE_CODE == resource_code) && // Because we want both 'E' and 'G'
                                 //(Tariff_Matrix.RESOURCE_TYPE == resource_type) && // Because whilst 'G' is always 'SR', 'E' might be 'SR' or 'DR'
                                 (Tariff_Matrix.TARIFF_CODE == tariff_code) &&
                                 (Tariff_Matrix.PLACEHOLDER == SmartParametersV2016.noFlag)
                         select Tariff_Matrix);
                }
                else
                {
                    tariff_matrix_found = new List<SmartUtility.TariffMatrix>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff_Matrix in utilityviewmodel.Hezbollah.tariff_matrixList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff_Matrix.CUBEFACE_CODE }
                     where (Tariff_Matrix.BRAND_CODE == brand_code) &&
                             (Tariff_Matrix.RESOURCE_CODE == resource_code) &&
                             (Tariff_Matrix.RESOURCE_TYPE == resource_type) &&
                             (Tariff_Matrix.TARIFF_CODE == tariff_code) &&
                             (Tariff_Matrix.PLACEHOLDER == SmartParametersV2016.noFlag)
                     select Tariff_Matrix);
                }
            }
            tariff_matrix_found = new List<SmartUtility.TariffMatrix>(tariff_matrix_found.DistinctBy(key => new { key.BRAND_CODE }));
            return tariff_matrix_found;
        }

        
        internal static void Utility_Find_Readings(UtilityViewModel utilityviewmodel,
                                                    char resource_code)
        {
            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Note: This is ACROSS all Suppliers ... and all MPANs
                    utilityviewmodel.e_readings_found = Utility_EReadings(utilityviewmodel);
                    break;
                case SmartParametersV2016.Gas:
                    // Note: This is ACROSS all Suppliers ... and all MPRNs
                    utilityviewmodel.g_readings_found = Utility_GReadings(utilityviewmodel);
                    break;
                default:
                    // Do the impossible!!
                    utilityviewmodel.e_readings_found = Utility_EReadings(utilityviewmodel);
                    utilityviewmodel.g_readings_found = Utility_GReadings(utilityviewmodel);
                    foreach (SmartUtility.EReadings e_readings_row in utilityviewmodel.Hezbollah.e_readingsList)
                    {
                        SmartUtility.EReadings d_readings_row = new SmartUtility.EReadings
                        {
                            // Common
                            USERNAME = e_readings_row.USERNAME,
                            CUBEFACE_CODE = e_readings_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = e_readings_row.SUPPLIER_CODE,
                            BRAND_CODE = e_readings_row.BRAND_CODE,
                            ACCOUNT_NO = e_readings_row.ACCOUNT_NO,
                            ACCOUNT_CREATED = e_readings_row.ACCOUNT_CREATED,
                            STATEMENT_ID = e_readings_row.STATEMENT_ID,
                            BILL_DATE = e_readings_row.BILL_DATE,
                            MPAN_MPRN = e_readings_row.MPAN_MPRN,
                            READINGS_PERIOD_START = e_readings_row.READINGS_PERIOD_START,
                            READINGS_PERIOD_END = e_readings_row.READINGS_PERIOD_END,
                            METER_SERIAL_NO = e_readings_row.METER_SERIAL_NO,
                            READ_TYPE = e_readings_row.READ_TYPE,
                            D_LAST_READ = e_readings_row.D_LAST_READ,
                            D_THIS_READ = e_readings_row.D_THIS_READ,
                            D_UNITS_USED = e_readings_row.D_UNITS_USED,
                            N_LAST_READ = e_readings_row.N_LAST_READ,
                            N_THIS_READ = e_readings_row.N_THIS_READ,
                            N_UNITS_USED = e_readings_row.N_UNITS_USED,
                            UNIT_OF_MEASURE = SmartParametersV2016.defaultUoM
                        };
                        utilityviewmodel.d_readings_found.Add(d_readings_row);
                    }
                    foreach (SmartUtility.GReadings g_readings_row in utilityviewmodel.Hezbollah.g_readingsList)
                    {
                        SmartUtility.EReadings d_readings_row = new SmartUtility.EReadings
                        {
                            // Common
                            USERNAME = g_readings_row.USERNAME,
                            CUBEFACE_CODE = g_readings_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_readings_row.SUPPLIER_CODE,
                            BRAND_CODE = g_readings_row.BRAND_CODE,
                            ACCOUNT_NO = g_readings_row.ACCOUNT_NO,
                            ACCOUNT_CREATED = g_readings_row.ACCOUNT_CREATED,
                            STATEMENT_ID = g_readings_row.STATEMENT_ID,
                            BILL_DATE = g_readings_row.BILL_DATE,
                            MPAN_MPRN = g_readings_row.MPAN_MPRN,
                            READINGS_PERIOD_START = g_readings_row.READINGS_PERIOD_START,
                            READINGS_PERIOD_END = g_readings_row.READINGS_PERIOD_END,
                            METER_SERIAL_NO = g_readings_row.METER_SERIAL_NO,
                            READ_TYPE = g_readings_row.READ_TYPE,
                            D_LAST_READ = g_readings_row.D_LAST_READ,
                            D_THIS_READ = g_readings_row.D_THIS_READ,
                            D_UNITS_USED = g_readings_row.D_UNITS_USED_KWH,
                            N_LAST_READ = 0.0M,
                            N_THIS_READ = 0.0M,
                            N_UNITS_USED = 0.0M,
                            UNIT_OF_MEASURE = SmartParametersV2016.defaultUoM
                        };
                        utilityviewmodel.d_readings_found.Add(d_readings_row);
                    }
                    utilityviewmodel.d_readings_found =
                        new List<SmartUtility.EReadings>(from D_Reading in utilityviewmodel.d_readings_found
                                                         orderby D_Reading.BILL_DATE ascending
                                                         select D_Reading);
                    break;
            }
            return;
        }

        // All swivelled up ... <- All SHRIVELLED up
        // Her face looks all taunt ....  <- Her face looks all TAUT 
        internal static void Utility_Analyze_Charts(UtilityViewModel utilityviewmodel,
                                                    char resource_code)
        {
            utilityviewmodel.e_readings_found = new List<SmartUtility.EReadings>();
            utilityviewmodel.g_readings_found = new List<SmartUtility.GReadings>();
            utilityviewmodel.d_readings_found = new List<SmartUtility.EReadings>();

            Utility_Find_Readings(utilityviewmodel,
                            resource_code);
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    foreach (SmartUtility.EUsage e_usage_row in utilityviewmodel.Hezbollah.e_usageList)
                    {
                        List<SmartUtility.EReadings> e_readings =
                            new List<SmartUtility.EReadings>(from E_Read in utilityviewmodel.e_readings_found
                                                             where ((E_Read.USERNAME == e_usage_row.USERNAME &&
                                                                     E_Read.CUBEFACE_CODE == e_usage_row.CUBEFACE_CODE &&
                                                                     E_Read.MPAN_MPRN == e_usage_row.MPAN_MPRN &&
                                                                     E_Read.READINGS_PERIOD_START <= e_usage_row.USAGE_DATETIME) &&
                                                                     (e_usage_row.USAGE_DATETIME <= E_Read.READINGS_PERIOD_END))
                                                             select E_Read);
                        if (e_readings.Count > 0)
                        {
                            foreach (SmartUtility.EReadings e_readings_row in e_readings)
                            {
                                SmartUtility.EUsageView e_usage_view_row = new SmartUtility.EUsageView()
                                {
                                    USERNAME = e_readings_row.USERNAME,
                                    CUBEFACE_CODE = e_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = e_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_NO = e_readings_row.ACCOUNT_NO,
                                    ACCOUNT_CREATED = e_readings_row.ACCOUNT_CREATED,
                                    MPAN_MPRN = e_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = e_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = e_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = e_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = e_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.e_usage_view_found.Add(e_usage_view_row);
                            }
                        }
                    }
                    break;
                case SmartParametersV2016.Gas:
                    foreach (SmartUtility.GUsage g_usage_row in utilityviewmodel.Hezbollah.g_usageList)
                    {
                        List<SmartUtility.GReadings> g_readings =
                            new List<SmartUtility.GReadings>(from G_Read in utilityviewmodel.g_readings_found
                                                             where ((G_Read.USERNAME == g_usage_row.USERNAME &&
                                                                     G_Read.CUBEFACE_CODE == g_usage_row.CUBEFACE_CODE &&
                                                                     G_Read.MPAN_MPRN == g_usage_row.MPAN_MPRN &&
                                                                     G_Read.READINGS_PERIOD_START <= g_usage_row.USAGE_DATETIME) &&
                                                                     (g_usage_row.USAGE_DATETIME <= G_Read.READINGS_PERIOD_END))
                                                             select G_Read);
                        if (g_readings.Count > 0)
                        {
                            foreach (SmartUtility.GReadings g_readings_row in g_readings)
                            {
                                SmartUtility.GUsageView g_usage_view_row = new SmartUtility.GUsageView()
                                {
                                    USERNAME = g_readings_row.USERNAME,
                                    CUBEFACE_CODE = g_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = g_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_NO = g_readings_row.ACCOUNT_NO,
                                    ACCOUNT_CREATED = g_readings_row.ACCOUNT_CREATED,
                                    MPAN_MPRN = g_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = g_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = g_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = g_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = g_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.g_usage_view_found.Add(g_usage_view_row);
                            }
                        }
                    }
                    break;
                default:
                    foreach (SmartUtility.EUsage e_usage_row in utilityviewmodel.Hezbollah.e_usageList)
                    {
                        List<SmartUtility.EReadings> e_readings =
                            new List<SmartUtility.EReadings>(from E_Read in utilityviewmodel.e_readings_found
                                                             where ((E_Read.USERNAME == e_usage_row.USERNAME &&
                                                                     E_Read.CUBEFACE_CODE == e_usage_row.CUBEFACE_CODE &&
                                                                     E_Read.MPAN_MPRN == e_usage_row.MPAN_MPRN &&
                                                                     E_Read.READINGS_PERIOD_START <= e_usage_row.USAGE_DATETIME) &&
                                                                     (e_usage_row.USAGE_DATETIME <= E_Read.READINGS_PERIOD_END))
                                                             select E_Read);
                        if (e_readings.Count > 0)
                        {
                            foreach (SmartUtility.EReadings e_readings_row in e_readings)
                            {
                                SmartUtility.EUsageView d_usage_view_row = new SmartUtility.EUsageView()
                                {
                                    USERNAME = e_readings_row.USERNAME,
                                    CUBEFACE_CODE = e_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = e_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_NO = e_readings_row.ACCOUNT_NO,
                                    ACCOUNT_CREATED = e_readings_row.ACCOUNT_CREATED,
                                    MPAN_MPRN = e_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = e_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = e_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = e_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = e_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.d_usage_view_found.Add(d_usage_view_row);
                            }
                        }
                    }
                    foreach (SmartUtility.GUsage g_usage_row in utilityviewmodel.Hezbollah.g_usageList)
                    {
                        List<SmartUtility.GReadings> g_readings =
                            new List<SmartUtility.GReadings>
                            (from G_Read in utilityviewmodel.g_readings_found
                             where ((G_Read.USERNAME == g_usage_row.USERNAME &&
                                     G_Read.CUBEFACE_CODE == g_usage_row.CUBEFACE_CODE &&
                                     G_Read.MPAN_MPRN == g_usage_row.MPAN_MPRN &&
                                     G_Read.READINGS_PERIOD_START <= g_usage_row.USAGE_DATETIME) &&
                                     (g_usage_row.USAGE_DATETIME <= G_Read.READINGS_PERIOD_END))
                             select G_Read);
                        if (g_readings.Count > 0)
                        {
                            foreach (SmartUtility.GReadings g_readings_row in g_readings)
                            {
                                SmartUtility.EUsageView d_usage_view_row = new SmartUtility.EUsageView()
                                {
                                    USERNAME = g_readings_row.USERNAME,
                                    CUBEFACE_CODE = g_readings_row.CUBEFACE_CODE,
                                    SUPPLIER_CODE = g_readings_row.SUPPLIER_CODE,
                                    ACCOUNT_NO = g_readings_row.ACCOUNT_NO,
                                    ACCOUNT_CREATED = g_readings_row.ACCOUNT_CREATED,
                                    MPAN_MPRN = g_readings_row.MPAN_MPRN,
                                    METER_SERIAL_NO = g_readings_row.METER_SERIAL_NO,
                                    USAGE_DATETIME = g_usage_row.USAGE_DATETIME,
                                    USAGE_TOTAL = g_usage_row.USAGE_TOTAL,
                                    USAGE_VALUE = g_usage_row.USAGE_VALUE
                                };
                                utilityviewmodel.d_usage_view_found.Add(d_usage_view_row);
                            }
                        }
                    }
                    utilityviewmodel.d_usage_view_found = new List<SmartUtility.EUsageView>
                        (from D_Usage_View in utilityviewmodel.d_usage_view_found
                         orderby D_Usage_View.USAGE_DATETIME ascending
                         select D_Usage_View);
                    break;
            }
            return;
        }

        internal static void Utility_Analyze_UnitCharges(UtilityViewModel utilityviewmodel,
                                                    char resource_code,
                                                    SmartUtility.BillsResource working_bills_resource_row)
        {
            List<SmartUtility.BillsResource> working_bills_resourceList = new List<SmartUtility.BillsResource> { working_bills_resource_row };
            //working_bills_resourceList.Add(working_bills_resource_row);

            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    utilityviewmodel.bill_e_unit_charges = Utility_EUnitCharges(utilityviewmodel);
                    break;
                case SmartParametersV2016.Gas:
                    utilityviewmodel.bill_g_unit_charges = Utility_GUnitCharges(utilityviewmodel);
                    break;
                default:
                    utilityviewmodel.bill_d_unit_charges = Utility_EUnitCharges(utilityviewmodel);
                    utilityviewmodel.bill_g_unit_charges = Utility_GUnitCharges(utilityviewmodel);
                    foreach (SmartUtility.GUnitCharges g_unit_charges_row in utilityviewmodel.bill_g_unit_charges)
                    {
                        SmartUtility.EUnitCharges d_unit_charges_row = new SmartUtility.EUnitCharges
                        {
                            USERNAME = g_unit_charges_row.USERNAME,
                            CUBEFACE_CODE = g_unit_charges_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_unit_charges_row.SUPPLIER_CODE,
                            BRAND_CODE = g_unit_charges_row.BRAND_CODE,
                            ACCOUNT_NO = g_unit_charges_row.ACCOUNT_NO,
                            ACCOUNT_CREATED = g_unit_charges_row.ACCOUNT_CREATED,
                            STATEMENT_ID = g_unit_charges_row.STATEMENT_ID,
                            BILL_DATE = g_unit_charges_row.BILL_DATE,
                            MPAN_MPRN = g_unit_charges_row.MPAN_MPRN,
                            UNIT_CHARGES_PERIOD_START = g_unit_charges_row.UNIT_CHARGES_PERIOD_START,
                            UNIT_CHARGES_PERIOD_END = g_unit_charges_row.UNIT_CHARGES_PERIOD_END,
                            UNITS_TIME = 'D',
                            UNITS_BAND = g_unit_charges_row.UNITS_BAND,
                            UNITS_TYPE = g_unit_charges_row.UNITS_TYPE,
                            UNITS = g_unit_charges_row.UNITS,
                            UNITS_RATE = g_unit_charges_row.UNITS_RATE,
                            UNIT_OF_MEASURE = g_unit_charges_row.UNIT_OF_MEASURE,
                            UNITS_COST = g_unit_charges_row.UNITS_COST
                        };
                        utilityviewmodel.bill_d_unit_charges.Add(d_unit_charges_row);
                    }
                    utilityviewmodel.bill_d_unit_charges = new List<SmartUtility.EUnitCharges>
                        (from D_Unit_Charges in utilityviewmodel.bill_d_unit_charges
                         orderby D_Unit_Charges.BILL_DATE ascending
                         select D_Unit_Charges);
                    break;
            }
        }

        internal static void Utility_Analyze_StandingCharges(UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                string statement_id,
                                                SmartUtility.BillsResource working_bills_resource_row)
        {
            List<SmartUtility.BillsResource> working_bills_resourceList = new List<SmartUtility.BillsResource> { working_bills_resource_row };
            //working_bills_resourceList.Add(working_bills_resource_row);

            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Note: This is ACROSS all Suppliers ... and all MPANs
                    utilityviewmodel.bill_e_standing_charges = Utility_EStandingCharges(utilityviewmodel,
                                                                    statement_id);
                    break;
                case SmartParametersV2016.Gas:
                    // Note: This is ACROSS all Suppliers ... and all MPRNs
                    utilityviewmodel.bill_g_standing_charges = Utility_GStandingCharges(utilityviewmodel,
                                                                    statement_id);
                    break;
                default:
                    utilityviewmodel.bill_d_standing_charges = Utility_EStandingCharges(utilityviewmodel,
                                                                    statement_id);
                    utilityviewmodel.bill_g_standing_charges = Utility_GStandingCharges(utilityviewmodel,
                                                                    statement_id);

                    foreach (SmartUtility.GStandingCharges g_standing_charges_row in utilityviewmodel.bill_g_standing_charges)
                    {
                        SmartUtility.EStandingCharges d_standing_charges_row = new SmartUtility.EStandingCharges
                        {
                            USERNAME = g_standing_charges_row.USERNAME,
                            CUBEFACE_CODE = g_standing_charges_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_standing_charges_row.SUPPLIER_CODE,
                            BRAND_CODE = g_standing_charges_row.BRAND_CODE,
                            ACCOUNT_NO = g_standing_charges_row.ACCOUNT_NO,
                            ACCOUNT_CREATED = g_standing_charges_row.ACCOUNT_CREATED,
                            STATEMENT_ID = g_standing_charges_row.STATEMENT_ID,
                            BILL_DATE = g_standing_charges_row.BILL_DATE,
                            MPAN_MPRN = g_standing_charges_row.MPAN_MPRN,
                            STANDING_CHARGES_PERIOD_START = g_standing_charges_row.STANDING_CHARGES_PERIOD_START,
                            STANDING_CHARGES_PERIOD_END = g_standing_charges_row.STANDING_CHARGES_PERIOD_END,
                            CHARGES_ITEM = g_standing_charges_row.CHARGES_ITEM,
                            CHARGES_TYPE = g_standing_charges_row.CHARGES_TYPE,
                            STANDING_CHARGE = g_standing_charges_row.STANDING_CHARGE,
                            CHARGES_DAYS = g_standing_charges_row.CHARGES_DAYS,
                            CHARGES_COST = g_standing_charges_row.CHARGES_COST
                        };
                        utilityviewmodel.bill_d_standing_charges.Add(d_standing_charges_row);
                    }
                    utilityviewmodel.bill_d_standing_charges =
                        new List<SmartUtility.EStandingCharges>
                        (from D_Standing_Charges in utilityviewmodel.bill_d_standing_charges
                         orderby D_Standing_Charges.BILL_DATE ascending
                         select D_Standing_Charges);
                    break;
            }
            return;
        }

        internal static void Utility_Analyze_Discounts(UtilityViewModel utilityviewmodel,
                                                char resource_code,
                                                SmartUtility.BillsResource working_bills_resource_row)
        {
            List<SmartUtility.BillsResource> working_bills_resourceList = new List<SmartUtility.BillsResource> { working_bills_resource_row };
            //working_bills_resourceList.Add(working_bills_resource_row);

            //Tab "Readings" (across ALL Account Nos (e.g. Pat's Npower)
            switch (resource_code)
            {
                case SmartParametersV2016.Electricity:
                    // Note: This is ACROSS all Suppliers ... and all MPANs
                    utilityviewmodel.bill_e_discounts = Utility_EDiscounts(utilityviewmodel);
                    break;
                case SmartParametersV2016.Gas:
                    // Note: This is ACROSS all Suppliers ... and all MPRNs
                    utilityviewmodel.bill_g_discounts = Utility_GDiscounts(utilityviewmodel);
                    break;
                default:
                    utilityviewmodel.bill_d_discounts = Utility_EDiscounts(utilityviewmodel);
                    utilityviewmodel.bill_g_discounts = Utility_GDiscounts(utilityviewmodel);
                    foreach (SmartUtility.GDiscounts g_discounts_row in utilityviewmodel.bill_g_discounts)
                    {
                        SmartUtility.EDiscounts d_discounts_row = new SmartUtility.EDiscounts
                        {
                            USERNAME = g_discounts_row.USERNAME,
                            CUBEFACE_CODE = g_discounts_row.CUBEFACE_CODE,
                            SUPPLIER_CODE = g_discounts_row.SUPPLIER_CODE,
                            BRAND_CODE = g_discounts_row.BRAND_CODE,
                            ACCOUNT_NO = g_discounts_row.ACCOUNT_NO,
                            ACCOUNT_CREATED = g_discounts_row.ACCOUNT_CREATED,
                            STATEMENT_ID = g_discounts_row.STATEMENT_ID,
                            BILL_DATE = g_discounts_row.BILL_DATE,
                            MPAN_MPRN = g_discounts_row.MPAN_MPRN,
                            DISCOUNT_DATE = g_discounts_row.DISCOUNT_DATE,
                            DISCOUNT_ITEM = g_discounts_row.DISCOUNT_ITEM,
                            DISCOUNT_TYPE = g_discounts_row.DISCOUNT_TYPE,
                            DISCOUNT_VAT_CODE = g_discounts_row.DISCOUNT_VAT_CODE,
                            DISCOUNT_AMOUNT = g_discounts_row.DISCOUNT_AMOUNT
                        };
                        utilityviewmodel.bill_d_discounts.Add(d_discounts_row);
                    }
                    utilityviewmodel.bill_d_discounts = new List<SmartUtility.EDiscounts>
                        (from D_Discounts in utilityviewmodel.bill_d_discounts
                         orderby D_Discounts.BILL_DATE ascending
                         select D_Discounts);
                    break;
            }
            return;
        }

        internal static List<SmartUtility.ConditionsPlans> Utility_ConditionsPlansList(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    char resource_code,
                                                                    string resource_type,
                                                                    short supplier_code,
                                                                    int tariff_code,
                                                                    short version_code,
                                                                    char payment_plan)
        {
            List<SmartUtility.ConditionsPlans> conditions_plans_found = new List<SmartUtility.ConditionsPlans>();
            if (payment_plan == SmartParametersV2016.defaultChar)
            {
                conditions_plans_found = new List<SmartUtility.ConditionsPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Conditions_Payment in utilityviewmodel.Hezbollah.conditions_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Conditions_Payment.CUBEFACE_CODE }
                     where (Conditions_Payment.SUPPLIER_CODE == supplier_code) &&
                         (Conditions_Payment.RESOURCE_CODE == resource_code) &&
                         (Conditions_Payment.RESOURCE_TYPE == resource_type) &&
                         (Conditions_Payment.TARIFF_CODE == tariff_code) &&
                         (Conditions_Payment.VERSION_CODE == version_code)
                     select Conditions_Payment); // FirstOrDefault();
            }
            else
            {
                conditions_plans_found = new List<SmartUtility.ConditionsPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Conditions_Payment in utilityviewmodel.Hezbollah.conditions_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Conditions_Payment.CUBEFACE_CODE }
                     where (Conditions_Payment.SUPPLIER_CODE == supplier_code) &&
                         (Conditions_Payment.RESOURCE_CODE == resource_code) &&
                         (Conditions_Payment.RESOURCE_TYPE == resource_type) &&
                         (Conditions_Payment.TARIFF_CODE == tariff_code) &&
                         (Conditions_Payment.VERSION_CODE == version_code) &&
                         (Conditions_Payment.PAYMENT_PLANS.Contains(payment_plan))
                     select Conditions_Payment); // FirstOrDefault();
            }
            conditions_plans_found = new List<SmartUtility.ConditionsPlans>(conditions_plans_found.Distinct());
            return conditions_plans_found;
        }

        internal static List<SmartUtility.ConditionsDates> Utility_Find_ConditionsDates(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    short comparison_supplier_code,
                                                                    int comparison_tariff_code,
                                                                    char displayed_resource_code,
                                                                    string displayed_resource_type,
                                                                    short version_code,
                                                                    short payment_code)
        {
            List<SmartUtility.ConditionsDates> conditions_dates_found =
                new List<SmartUtility.ConditionsDates>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Conditions_Dates in utilityviewmodel.Hezbollah.conditions_datesList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Conditions_Dates.CUBEFACE_CODE }
                 where (Conditions_Dates.SUPPLIER_CODE == comparison_supplier_code) &&
                        (Conditions_Dates.TARIFF_CODE == comparison_tariff_code) &&
                        (Conditions_Dates.RESOURCE_CODE == displayed_resource_code) &&
                        (Conditions_Dates.RESOURCE_TYPE == displayed_resource_type) &&
                        (Conditions_Dates.VERSION_CODE == version_code) &&
                        (Conditions_Dates.PAYMENT_CODE == payment_code)
                 orderby Conditions_Dates.PRICES_VALID_FROM descending
                 select Conditions_Dates);
            conditions_dates_found = new List<SmartUtility.ConditionsDates>(conditions_dates_found.Distinct());
            return conditions_dates_found;
        }

        internal static List<SmartUtility.ConditionsLimits> Utility_Find_ConditionsLimits(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    short comparison_supplier_code,
                                                                    char displayed_resource_code,
                                                                    string displayed_resource_type,
                                                                    short limit_code)
        {
            List<SmartUtility.ConditionsLimits> conditions_limits_found =
                new List<SmartUtility.ConditionsLimits>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Conditions_Limit in utilityviewmodel.Hezbollah.conditions_limitsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Conditions_Limit.CUBEFACE_CODE }
                 where (Conditions_Limit.SUPPLIER_CODE == comparison_supplier_code) &&
                          (Conditions_Limit.RESOURCE_CODE == displayed_resource_code) &&
                          (Conditions_Limit.RESOURCE_TYPE == displayed_resource_type) &&
                          (Conditions_Limit.LIMIT_CODE == limit_code)
                 select Conditions_Limit);
            conditions_limits_found = new List<SmartUtility.ConditionsLimits>(conditions_limits_found.Distinct());
            return conditions_limits_found;
        }

        internal static List<SmartUtility.UnitRates> Utility_Find_UnitRates(MainViewModel ourviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            short comparison_supplier_code,
                                            char displayed_resource_code,
                                            string displayed_resource_type,
                                            int comparison_tariff_code,
                                            short payment_code,
                                            DateTime prices_valid_from,
                                            short tier_count,
                                            short tier_level,
                                            List<SmartUtility.UnitRates> unit_ratesList)
        {
            List<SmartUtility.UnitRates> unit_rates_found =
                new List<SmartUtility.UnitRates>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Unit_Rates in unit_ratesList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Unit_Rates.CUBEFACE_CODE }
                 where (Unit_Rates.SUPPLIER_CODE == comparison_supplier_code) &&
                         (Unit_Rates.RESOURCE_CODE == displayed_resource_code) &&
                         (Unit_Rates.RESOURCE_TYPE == displayed_resource_type) &&
                         (Unit_Rates.TARIFF_CODE == comparison_tariff_code) &&
                         (Unit_Rates.PAYMENT_CODE == payment_code) &&
                         (Unit_Rates.PRICES_VALID_FROM == prices_valid_from) &&
                         (Unit_Rates.TIER_COUNT == tier_count) &&
                         (Unit_Rates.TIER_LEVEL == tier_level) &&
                         (Unit_Rates.AREA_CODE == utilityviewmodel.area_code)
                 select Unit_Rates);
            unit_rates_found = new List<SmartUtility.UnitRates>(unit_rates_found.Distinct());
            return unit_rates_found;
        }

        internal static List<SmartUtility.BankDetails> Utility_Lookup_BankDetailsX(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                short brand_code)
        {
            List<SmartUtility.BankDetails> bank_details_found = new List<SmartUtility.BankDetails>();
            // Only if its not 0 and we have something in the list
            if (brand_code > 0)
            {
                bank_details_found = new List<SmartUtility.BankDetails>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Bank_Detail in utilityviewmodel.Hezbollah.utility_bankdetailsList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Bank_Detail.USERNAME, Bank_Detail.CUBEFACE_CODE }
                     where Bank_Detail.SUPPLIER_CODE == supplier_code &&
                         Bank_Detail.BRAND_CODE == brand_code
                     select Bank_Detail);
            }
            else
            {
                bank_details_found = new List<SmartUtility.BankDetails>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Bank_Detail in utilityviewmodel.Hezbollah.utility_bankdetailsList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Bank_Detail.USERNAME, Bank_Detail.CUBEFACE_CODE }
                     select Bank_Detail);
            }
            bank_details_found = new List<SmartUtility.BankDetails>(bank_details_found.Distinct());
            return bank_details_found;
        }

        internal static List<SmartUtility.SupplyAreas> Utility_Find_SupplyAreas(MainViewModel ourviewmodel,
                                                             UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.SupplyAreas> supply_areas_found =
                new List<SmartUtility.SupplyAreas>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Supply_Area in utilityviewmodel.Hezbollah.supply_areasList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Supply_Area.CUBEFACE_CODE }
                 where Supply_Area.AREA_CODE == utilityviewmodel.area_code
                 select Supply_Area);
            supply_areas_found = new List<SmartUtility.SupplyAreas>(supply_areas_found.Distinct());
            return supply_areas_found;
        }

        internal static List<SmartUtility.DistributorInfo> Utility_Find_DistributorInfo(MainViewModel ourviewmodel,
                                                             UtilityViewModel utilityviewmodel,
                                                             string marketId)
        {
            List<SmartUtility.DistributorInfo> distributorInfo_found =
                new List<SmartUtility.DistributorInfo>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join DistributorInfo in utilityviewmodel.Hezbollah.distributor_infoList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { DistributorInfo.CUBEFACE_CODE }
                 where DistributorInfo.MARKET_ID == marketId
                 select DistributorInfo);
            return distributorInfo_found;
        }

        internal static List<SmartUtility.AnalysisCosts> Utility_Find_DualFuel(List<SmartUtility.AnalysisCosts> e_analysis_costsList,
                                                        List<SmartUtility.AnalysisCosts> g_analysis_costsList)
        {
            List<SmartUtility.AnalysisCosts> analysis_costs_found =
                new List<SmartUtility.AnalysisCosts>(
                    from E in e_analysis_costsList
                    join G in g_analysis_costsList
                    on new
                    {
                        E.SUPPLIER_CODE,
                        E.BRAND_CODE,
                        E.TARIFF_CODE,
                        E.PAYMENT_PLAN
                    }
                    equals new
                    {
                        G.SUPPLIER_CODE,
                        G.BRAND_CODE,
                        G.TARIFF_CODE,
                        G.PAYMENT_PLAN
                    }
                    select new SmartUtility.AnalysisCosts
                    {
                        USERNAME = E.USERNAME,
                        CUBEFACE_CODE = E.CUBEFACE_CODE,
                        RESOURCE_CODE = SmartParametersV2016.DualFuel,
                        UNIQUE_NUMBER = E.UNIQUE_NUMBER,
                        SUPPLIER_CODE = E.SUPPLIER_CODE,
                        BRAND_CODE = E.BRAND_CODE,
                        TARIFF_CODE = E.TARIFF_CODE,
                        TCR = 0.0M,
                        RESOURCE_TYPE = E.RESOURCE_TYPE + SmartParametersV2016.bar + G.RESOURCE_TYPE,
                        PAYMENT_PLAN = E.PAYMENT_PLAN,
                        TOTAL_COST = E.TOTAL_COST + G.TOTAL_COST
                    });
            //{ }


            //
            //    .DistinctBy(key => new
            //    {
            //        key.USERNAME,
            //        key.CUBEFACE_CODE,
            //        key.RESOURCE_CODE,
            //        key.SUPPLIER_CODE,
            //        key.BRAND_CODE,
            //        key.TARIFF_CODE,
            //        key.RESOURCE_TYPE,
            //        key.PAYMENT_PLAN
            //    }
            // );
            //analysis_costs_found = new List<SmartUtility.AnalysisCosts>
            //    (analysis_costs_found.DistinctBy(key => new
            //    {
            //        key.USERNAME,
            //        key.CUBEFACE_CODE,
            //        key.RESOURCE_CODE,
            //        key.SUPPLIER_CODE,
            //        key.BRAND_CODE,
            //        key.TARIFF_CODE,
            //        key.RESOURCE_TYPE,
            //        key.PAYMENT_PLAN
            //    }));
            analysis_costs_found = new List<SmartUtility.AnalysisCosts>(analysis_costs_found.Distinct());
            return analysis_costs_found;
        }

        internal static List<SmartUtility.AnalysisCosts> Utility_Find_AnalysisCosts_Brand(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel, short supplier_code,
                                                                    short brand_code,
                                                                    List<SmartUtility.AnalysisCosts> analysis_costsList)
        {
            List<SmartUtility.AnalysisCosts> analysis_costs_found =
                new List<SmartUtility.AnalysisCosts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Analysis_Cost in analysis_costsList
                on new { Cubeface.CUBEFACE_CODE }
                equals new { Analysis_Cost.CUBEFACE_CODE }
                 where Analysis_Cost.SUPPLIER_CODE == supplier_code &&
                     Analysis_Cost.BRAND_CODE == brand_code
                 orderby Analysis_Cost.TOTAL_COST ascending
                 select Analysis_Cost);
            analysis_costs_found = new List<SmartUtility.AnalysisCosts>(analysis_costs_found.Distinct());
            return analysis_costs_found;
        }

        internal static List<SmartUtility.AnalysisCosts> Utility_Find_AnalysisCosts_Resource(List<SmartUtility.AnalysisCosts> analysis_costsList)
        {
            List<SmartUtility.AnalysisCosts> analysis_costs_found =
                new List<SmartUtility.AnalysisCosts>
                (from Analysis_Cost in analysis_costsList
                     //where Analysis_Cost.CUBEFACE_CODE == utilityviewmodel.cubeface_code
                     //where resource_codeList.Any(rc => rc == Analysis_Cost.RESOURCE_CODE)
                 orderby Analysis_Cost.TOTAL_COST ascending
                 select Analysis_Cost);
            return analysis_costs_found;
        }

        internal static List<string> Utility_Find_PaymentPlansX(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    short supplier_code,
                                                                    char resource_code,
                                                                    string resource_type,
                                                                    int tariff_code)
        {
            List<string> conditions_plans_found = new List<string>();

            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                // You need a DISTINCT here or you will get 3 times the tariffs you think
                // you will get!  You will get E:SR, E:VR and G:SR !!!!
                conditions_plans_found = new List<string>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Conditions_Plan in utilityviewmodel.Hezbollah.conditions_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Conditions_Plan.CUBEFACE_CODE }
                     where (Conditions_Plan.SUPPLIER_CODE == supplier_code) &&
                             (Conditions_Plan.RESOURCE_TYPE == resource_type) &&
                             (Conditions_Plan.TARIFF_CODE == tariff_code)
                     orderby Conditions_Plan.VERSION_CODE descending
                     select Conditions_Plan.PAYMENT_PLANS);
            }
            else
            {
                conditions_plans_found = new List<string>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Conditions_Plan in utilityviewmodel.Hezbollah.conditions_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Conditions_Plan.CUBEFACE_CODE }
                     where (Conditions_Plan.SUPPLIER_CODE == supplier_code) &&
                             (Conditions_Plan.RESOURCE_CODE == resource_code) && // <= How could you miss this!!!??!!
                             (Conditions_Plan.RESOURCE_TYPE == resource_type) &&
                             (Conditions_Plan.TARIFF_CODE == tariff_code)
                     orderby Conditions_Plan.VERSION_CODE descending
                     select Conditions_Plan.PAYMENT_PLANS);
            }
            conditions_plans_found = new List<string>(conditions_plans_found.Distinct());
            return conditions_plans_found;
        }

        internal static List<SmartUtility.PaymentPlans> Utility_Find_PaymentPlansY(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                char payment_plan)
        {
            List<SmartUtility.PaymentPlans> payment_names_found =
                new List<SmartUtility.PaymentPlans>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Payment_Plan in utilityviewmodel.Hezbollah.payment_plansList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Payment_Plan.CUBEFACE_CODE }
                 where Payment_Plan.PAYMENT_PLAN == payment_plan
                 select Payment_Plan);
            payment_names_found = new List<SmartUtility.PaymentPlans>(payment_names_found.Distinct());
            return payment_names_found;
        }

        internal static List<SmartUtility.TariffCodesNames> Utility_Find_Tariff_Codes_Names(MainViewModel ourviewmodel,
                                                                            UtilityViewModel utilityviewmodel,
                                                                            short brand_code,
                                                                            char resource_code,
                                                                            string resource_type)
        {
            List<SmartUtility.TariffCodesNames> tariff_codes_names_found =
                new List<SmartUtility.TariffCodesNames>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join TCN in utilityviewmodel.Hezbollah.tariff_codes_namesList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { TCN.CUBEFACE_CODE }
                 where TCN.BRAND_CODE == brand_code &&
                         TCN.RESOURCE_CODE == resource_code &&
                         TCN.RESOURCE_TYPE == resource_type
                 select TCN);
            tariff_codes_names_found = new List<SmartUtility.TariffCodesNames>(tariff_codes_names_found.Distinct());
            return tariff_codes_names_found;
        }

        internal static List<SmartUtility.Switches> Utility_Find_Switches_UDPRN(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel,
                                                                                char resource_code)
        {
            List<SmartUtility.Switches> switches_found = new List<SmartUtility.Switches>();

            List<SmartUtility.Meters> udprnList =
                    Utility_Lookup_UDPRN(ourviewmodel, utilityviewmodel, resource_code);
            if (udprnList.Count > 0)
            {
                if (resource_code == SmartParametersV2016.defaultResourceCode)
                {
                    switches_found = new List<SmartUtility.Switches>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Switches in utilityviewmodel.Hezbollah.utility_switchesList
                         on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                         equals new { Switches.USERNAME, Switches.CUBEFACE_CODE }
                         join Meter in udprnList
                         on new { Switches.USERNAME, Switches.CUBEFACE_CODE, Switches.MPAN_MPRN }
                         equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
                         select Switches);
                }
                else
                {
                    switches_found = new List<SmartUtility.Switches>
                        (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                         join Switches in utilityviewmodel.Hezbollah.utility_switchesList
                         on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                         equals new { Switches.USERNAME, Switches.CUBEFACE_CODE }
                         join Meter in udprnList
                         on new { Switches.USERNAME, Switches.CUBEFACE_CODE, Switches.MPAN_MPRN }
                         equals new { Meter.USERNAME, Meter.CUBEFACE_CODE, Meter.MPAN_MPRN }
                         where Meter.RESOURCE_CODE == resource_code
                         select Switches);
                }
            }
            switches_found = new List<SmartUtility.Switches>(switches_found.Distinct());
            return switches_found;
        }

        internal static List<SmartUtility.Resources> Utility_Find_View_LAST_CHECKED(MainViewModel ourviewmodel,
                                                                                                    UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.Resources> resource_found =
                new List<SmartUtility.Resources>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                 where Resource.CHECKED == SmartParametersV2016.lastChecked
                 select Resource);
            resource_found = new List<SmartUtility.Resources>(resource_found.Distinct());
            return resource_found;
        }

        internal static List<SmartUtility.Resources> Utility_Find_Switches_LAST_CHECKED_ANY(MainViewModel ourviewmodel,
                                                                                            UtilityViewModel utilityviewmodel)//,
                                                                                                                              //List<SmartUtility.Resources> utility_resourcesList)
        {
            List<SmartUtility.Resources> resources_found =
                new List<SmartUtility.Resources>(
                from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                join Resource in utilityviewmodel.Hezbollah.utility_resourcesList
                on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                equals new { Resource.USERNAME, Resource.CUBEFACE_CODE }
                where (Resource.CHECKED == SmartParametersV2016.lastChecked)
                select Resource);
            resources_found = new List<SmartUtility.Resources>(resources_found.Distinct());
            return resources_found;
        }

        internal static List<SmartUtility.TariffCosts> Utility_Find_Tariff_Costs_Years(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel)
        {
            List<SmartUtility.TariffCosts> tariff_costs_found =
                new List<SmartUtility.TariffCosts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Tariff_Cost in utilityviewmodel.Hezbollah.tariff_costsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Tariff_Cost.CUBEFACE_CODE }
                 orderby Tariff_Cost.PERIOD_END ascending,
                         Tariff_Cost.CODE ascending
                 select Tariff_Cost);
            tariff_costs_found = new List<SmartUtility.TariffCosts>(tariff_costs_found.Distinct());
            return tariff_costs_found;
        }

        internal static List<SmartUtility.TariffCosts> Utility_Find_Tariff_Costs_Months(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel,
                                                                                List<SmartUtility.TariffCosts> tariff_costsList)
        {
            List<SmartUtility.TariffCosts> tariff_costs_found =
                new List<SmartUtility.TariffCosts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Tariff_Cost_Month in tariff_costsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Tariff_Cost_Month.CUBEFACE_CODE }
                 orderby Tariff_Cost_Month.CODE ascending,
                     Tariff_Cost_Month.PERIOD_END ascending
                 select Tariff_Cost_Month);
            tariff_costs_found = new List<SmartUtility.TariffCosts>(tariff_costs_found.Distinct());
            return tariff_costs_found;
        }

        internal static List<SmartUtility.TariffCosts> Utility_Find_Tariff_Costs_Months_Total(MainViewModel ourviewmodel,
                                                                                    UtilityViewModel utilityviewmodel,
                                                                                    List<SmartUtility.TariffCosts> tariff_costsList)
        {
            List<SmartUtility.TariffCosts> tariff_costs_found =
                new List<SmartUtility.TariffCosts>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Tariff_Month_Total in tariff_costsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Tariff_Month_Total.CUBEFACE_CODE }
                 orderby Tariff_Month_Total.PERIOD_END ascending,
                         Tariff_Month_Total.UPPER_DATE ascending,
                         Tariff_Month_Total.CODE ascending
                 select Tariff_Month_Total);
            tariff_costs_found = new List<SmartUtility.TariffCosts>(tariff_costs_found.Distinct());
            return tariff_costs_found;
        }
        internal static List<SmartUtility.BrandMatrix> Utility_Find_Brand_Matrix(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            short brand_code)
        {
            List<SmartUtility.BrandMatrix> brand_matrix_found =
                new List<SmartUtility.BrandMatrix>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Brand_Area in utilityviewmodel.Hezbollah.brand_matrixList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Brand_Area.CUBEFACE_CODE }
                 where Brand_Area.BRAND_CODE == brand_code
                 select Brand_Area);
            brand_matrix_found = new List<SmartUtility.BrandMatrix>(brand_matrix_found.Distinct());
            return brand_matrix_found;
        }

        internal static List<SmartUtility.Tariffs> Utility_Find_Tariffs(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    short supplier_code,
                                                    char resource_code,
                                                    string resource_type)
        {
            List<SmartUtility.Tariffs> tariffs_found = new List<SmartUtility.Tariffs>();
            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                // You need a DISTINCT here or you will get 3 times the tariffs you think
                // you will get!  You will get E:SR, E:VR and G:SR !!!!
                tariffs_found = new List<SmartUtility.Tariffs>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff in utilityviewmodel.Hezbollah.tariffsList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff.CUBEFACE_CODE }
                     where Tariff.SUPPLIER_CODE == supplier_code
                     select Tariff);
                tariffs_found = new List<SmartUtility.Tariffs>
                    (tariffs_found.DistinctBy(key => new
                    {
                        key.SUPPLIER_CODE,
                        key.TARIFF_CODE
                    }));
            }
            else
            {
                // you need a DISTINCT here or you will - for SSE 81 - get 5 times
                // the tariffs you think you will get!  Tariffs are SUPPLIER specific
                // and not BRAND specific!
                tariffs_found = new List<SmartUtility.Tariffs>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff in utilityviewmodel.Hezbollah.tariffsList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff.CUBEFACE_CODE }
                     where (Tariff.CUBEFACE_CODE == utilityviewmodel.cubeface_code) &&
                             (Tariff.SUPPLIER_CODE == supplier_code) &&
                             (Tariff.RESOURCE_CODE == resource_code) &&
                             (Tariff.RESOURCE_TYPE == resource_type)
                     select Tariff);
                tariffs_found = new List<SmartUtility.Tariffs>
                   (tariffs_found.DistinctBy(key => new
                   {
                       key.SUPPLIER_CODE,
                       key.TARIFF_CODE
                   }));
            }
            tariffs_found = new List<SmartUtility.Tariffs>(tariffs_found.Distinct());
            return tariffs_found;
        }

        internal static List<SmartUtility.Tariffs> Utility_Find_Tariffs_Fallback(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    short supplier_code,
                                                    char resource_code,
                                                    string resource_type,
                                                    int tariff_code)
        {
            List<SmartUtility.Tariffs> tariffs_found =
                new List<SmartUtility.Tariffs>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Tariff in utilityviewmodel.Hezbollah.tariffsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Tariff.CUBEFACE_CODE }
                 where (Tariff.SUPPLIER_CODE == supplier_code) &&
                     (Tariff.RESOURCE_CODE == resource_code) &&
                     (Tariff.RESOURCE_TYPE == resource_type) &&
                     (Tariff.TARIFF_CODE == tariff_code) &&
                     (Tariff.FALLBACK == "Y")
                 select Tariff);
            tariffs_found = new List<SmartUtility.Tariffs>(tariffs_found.Distinct());
            return tariffs_found;
        }

        internal static List<SmartUtility.TariffMatrix> Utility_Find_Tariff_Matrix(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short brand_code,
                                                                char resource_code,
                                                                string resource_type,
                                                                int tariff_code)
        {
            List<SmartUtility.TariffMatrix> tariff_matrix_found =
                new List<SmartUtility.TariffMatrix>();
            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                tariff_matrix_found = new List<SmartUtility.TariffMatrix>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff_Matrix in utilityviewmodel.Hezbollah.tariff_matrixList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff_Matrix.CUBEFACE_CODE }
                     where (Tariff_Matrix.BRAND_CODE == brand_code) &&
                             //(Tariff_Area.RESOURCE_CODE == resource_code) &&
                             //(Tariff_Area.RESOURCE_TYPE == resource_type) &&
                             (Tariff_Matrix.TARIFF_CODE == tariff_code) &&
                             (Tariff_Matrix.PLACEHOLDER == "N")
                     orderby Tariff_Matrix.TARIFF_NAME ascending
                     select Tariff_Matrix);
            }
            else
            {
                tariff_matrix_found = new List<SmartUtility.TariffMatrix>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff_Matrix in utilityviewmodel.Hezbollah.tariff_matrixList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff_Matrix.CUBEFACE_CODE }
                     where (Tariff_Matrix.BRAND_CODE == brand_code) &&
                             (Tariff_Matrix.RESOURCE_CODE == resource_code) &&
                             (Tariff_Matrix.RESOURCE_TYPE == resource_type) &&
                             (Tariff_Matrix.TARIFF_CODE == tariff_code) &&
                             (Tariff_Matrix.PLACEHOLDER == "N")
                     orderby Tariff_Matrix.TARIFF_NAME ascending
                     select Tariff_Matrix);
            }
            tariff_matrix_found = new List<SmartUtility.TariffMatrix>(tariff_matrix_found.Distinct());
            return tariff_matrix_found;
        }

        internal static List<SmartUtility.PreConditions> Utility_Find_PreConditions(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                char resource_code,
                                                                int tariff_code)
        {
            List<SmartUtility.PreConditions> pre_conditions_found =
                new List<SmartUtility.PreConditions>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Pre_Condition in utilityviewmodel.Hezbollah.pre_conditionsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Pre_Condition.CUBEFACE_CODE }
                 where Pre_Condition.SUPPLIER_CODE == supplier_code &&
                     Pre_Condition.RESOURCE_CODE == resource_code &&
                     Pre_Condition.TARIFF_CODE == tariff_code
                 select Pre_Condition);
            pre_conditions_found = new List<SmartUtility.PreConditions>(pre_conditions_found.Distinct());
            return pre_conditions_found;
        }

        internal static List<SmartUtility.TariffPlans> Utility_Find_Tariff_Plans(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                bool wildcard,
                                                                short supplier_code,
                                                                char payment_plan)
        {
            List<SmartUtility.TariffPlans> tariff_plans_found =
                new List<SmartUtility.TariffPlans>();
            if (!wildcard)
            {
                tariff_plans_found = new List<SmartUtility.TariffPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff_Plan in utilityviewmodel.Hezbollah.tariff_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff_Plan.CUBEFACE_CODE }
                     where Tariff_Plan.SUPPLIER_CODE == supplier_code &&
                            Tariff_Plan.PAYMENT_PLANS.Contains(payment_plan)
                     select Tariff_Plan);
            }
            else
            {
                tariff_plans_found = new List<SmartUtility.TariffPlans>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tariff_Plan in utilityviewmodel.Hezbollah.tariff_plansList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tariff_Plan.CUBEFACE_CODE }
                     where Tariff_Plan.SUPPLIER_CODE == supplier_code
                     select Tariff_Plan);
            }
            tariff_plans_found = new List<SmartUtility.TariffPlans>(tariff_plans_found.DistinctBy(key => new
            {
                key.CUBEFACE_CODE,
                key.SUPPLIER_CODE,
                key.PAYMENT_PLANS
            }));
            return tariff_plans_found;
        }

        internal static List<SmartUtility.Post_Codes> Utility_Find_PostCodes(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                char resource_code,
                                                                short supplier_code,
                                                                int tariff_code)
        {
            List<SmartUtility.Post_Codes> post_codes_found =
                new List<SmartUtility.Post_Codes>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Tariff_Code in utilityviewmodel.Hezbollah.post_codesList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Tariff_Code.CUBEFACE_CODE }
                 where Tariff_Code.SUPPLIER_CODE == supplier_code &&
                       Tariff_Code.RESOURCE_CODE == resource_code &&
                       Tariff_Code.TARIFF_CODE == tariff_code
                 select Tariff_Code);
            post_codes_found = new List<SmartUtility.Post_Codes>(post_codes_found.Distinct());
            return post_codes_found;
        }

        internal static List<SmartUtility.Post_Groupings> Utility_Find_PostGroupings(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                short group_code)
        {
            List<SmartUtility.Post_Groupings> post_groupings_found =
                new List<SmartUtility.Post_Groupings>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Post_Conditions_Grouping in utilityviewmodel.Hezbollah.post_groupingsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Post_Conditions_Grouping.CUBEFACE_CODE }
                 where Post_Conditions_Grouping.SUPPLIER_CODE == supplier_code &&
                         Post_Conditions_Grouping.GROUP_CODE == group_code
                 select Post_Conditions_Grouping);
            post_groupings_found = new List<SmartUtility.Post_Groupings>(post_groupings_found.Distinct());
            return post_groupings_found;
        }

        internal static List<SmartUtility.Post_Conditions> Utility_Find_PostConditions(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                char resource_code,
                                                                short selection_code)
        {
            List<SmartUtility.Post_Conditions> post_conditions_found =
                new List<SmartUtility.Post_Conditions>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Post_Condition in utilityviewmodel.Hezbollah.post_conditionsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Post_Condition.CUBEFACE_CODE }
                 where Post_Condition.SUPPLIER_CODE == supplier_code &&
                         Post_Condition.RESOURCE_CODE == resource_code &&
                         Post_Condition.SELECTION_CODE == selection_code
                 select Post_Condition);
            post_conditions_found = new List<SmartUtility.Post_Conditions>(post_conditions_found.Distinct());
            return post_conditions_found;
        }

        internal static List<SmartUtility.Post_Select> Utility_Find_PostSelect(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                short selection_code)
        {
            List<SmartUtility.Post_Select> post_select_found =
                new List<SmartUtility.Post_Select>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Post_Conditions_Select_Group in utilityviewmodel.Hezbollah.post_selectList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Post_Conditions_Select_Group.CUBEFACE_CODE }
                 where Post_Conditions_Select_Group.CUBEFACE_CODE == utilityviewmodel.cubeface_code &&
                     Post_Conditions_Select_Group.SUPPLIER_CODE == supplier_code &&
                     Post_Conditions_Select_Group.SELECTION_CODE == selection_code
                 select Post_Conditions_Select_Group);
            post_select_found = new List<SmartUtility.Post_Select>(post_select_found.Distinct());
            return post_select_found;
        }

        internal static List<SmartUtility.Post_Limits> Utility_Find_PostLimits(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                short supplier_code,
                                                                short limit_code)
        {
            List<SmartUtility.Post_Limits> post_limits_found =
                new List<SmartUtility.Post_Limits>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                 join Post_Limit in utilityviewmodel.Hezbollah.post_limitsList
                 on new { Cubeface.CUBEFACE_CODE }
                 equals new { Post_Limit.CUBEFACE_CODE }
                 where Post_Limit.SUPPLIER_CODE == supplier_code &&
                         Post_Limit.LIMIT_CODE == limit_code
                 select Post_Limit);
            post_limits_found = new List<SmartUtility.Post_Limits>(post_limits_found.Distinct());
            return post_limits_found;
        }

        internal static List<SmartUtility.ConditionsAreas> Utility_Find_ConditionsAreas(MainViewModel ourviewmodel,
                                                                    UtilityViewModel utilityviewmodel,
                                                                    short supplier_code,
                                                                    char resource_code,
                                                                    string resource_type,
                                                                    short limit_code)
        {
            List<SmartUtility.ConditionsAreas> conditions_areas_found =
                new List<SmartUtility.ConditionsAreas>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)
                 join Conditions_Limits_Area in utilityviewmodel.Hezbollah.conditions_areasList
                  on new { Cubeface.CUBEFACE_CODE }
                  equals new { Conditions_Limits_Area.CUBEFACE_CODE }
                 where (Conditions_Limits_Area.SUPPLIER_CODE == supplier_code) &&
                     (Conditions_Limits_Area.RESOURCE_CODE == resource_code) &&
                     (Conditions_Limits_Area.RESOURCE_TYPE == resource_type) &&
                     (Conditions_Limits_Area.LIMIT_CODE == limit_code) &&
                     (Conditions_Limits_Area.AREA_CODE == utilityviewmodel.area_code)
                 select Conditions_Limits_Area);
            conditions_areas_found = new List<SmartUtility.ConditionsAreas>(conditions_areas_found.DistinctBy(key => new
            {
                key.CUBEFACE_CODE,
                key.SUPPLIER_CODE,
                key.RESOURCE_CODE,
                key.RESOURCE_TYPE,
                key.LIMIT_CODE,
                key.AREA_CODE,
                key.LOWER_LIMIT,
                key.UPPER_LIMIT
            }));
            return conditions_areas_found;
        }

        internal static List<Amelia> Utility_Find_Amelia(MainViewModel ourviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    char resource_code,
                                                    string resource_type,
                                                    string account_no,
                                                    string mpan_mprn)
        {
            List<Amelia> amelia_found = new List<Amelia>();

            if (string.IsNullOrEmpty(mpan_mprn))
            {
                amelia_found = new List<Amelia>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tiddles in utilityviewmodel.Hezbollah.ameliaList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tiddles.CUBEFACE_CODE }
                     where (Tiddles.RESOURCE_CODE == resource_code) &&
                             (Tiddles.RESOURCE_TYPE == resource_type) &&
                             (Tiddles.ACCOUNT_NO == account_no)
                     select Tiddles);
            }
            else
            {
                amelia_found = new List<Amelia>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Tiddles in utilityviewmodel.Hezbollah.ameliaList
                     on new { Cubeface.CUBEFACE_CODE }
                     equals new { Tiddles.CUBEFACE_CODE }
                     where (Tiddles.RESOURCE_CODE == resource_code) &&
                             (Tiddles.RESOURCE_TYPE == resource_type) &&
                             (Tiddles.ACCOUNT_NO == account_no) &&
                             (Tiddles.MPAN_MPRN == mpan_mprn)
                     select Tiddles);
            }
            return amelia_found;
        }

        internal static List<SmartUtility.GasConversion> Utility_Find_GasConversion(MainViewModel ourviewmodel,
                                                        List<SmartUtility.GasConversion> gas_conversionList)
        {
            DateTime localTimeNow = DateTime.Now + ourviewmodel.utcOffset;  // Local time
            List<SmartUtility.GasConversion> gas_conversion_found =
                new List<SmartUtility.GasConversion>
                (from GC in gas_conversionList
                 where (GC.VALID_FROM <= localTimeNow &&
                 localTimeNow <= GC.VALID_TO)
                 select GC);
            return gas_conversion_found;
        }

        internal static List<SmartUtility.Meters> Utility_Lookup_UDPRN(MainViewModel ourviewmodel,
                                                                                UtilityViewModel utilityviewmodel,
                                                                                char resource_code)
        {
            List<SmartUtility.Meters> meters_found = new List<SmartUtility.Meters>();

            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                meters_found = new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                     select Meter);
            }
            else
            {
                meters_found = new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                     where Meter.RESOURCE_CODE == resource_code
                     select Meter);
            }
            // Should only be ONE!  'A-cos we only have one MPAN per resource code and/or one MPRN per resource_code
            meters_found = new List<SmartUtility.Meters>(meters_found.Distinct());
            return meters_found;
        }

        internal static List<SmartUtility.Meters> Utility_Lookup_MetersByResource(MainViewModel ourviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            char resource_code)
        {
            List<SmartUtility.Meters> meters_found = new List<SmartUtility.Meters>();

            if (resource_code == SmartParametersV2016.defaultResourceCode)
            {
                meters_found = new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                     select Meter);
            }
            else
            {
                meters_found = new List<SmartUtility.Meters>
                    (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, utilityviewmodel.cubeface_code)

                     join Meter in utilityviewmodel.Hezbollah.utility_metersList
                     on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                     equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                     where Meter.RESOURCE_CODE == resource_code
                     select Meter);
            }
            // Should only be ONE!  'A-cos we only have one MPAN per resource code and/or one MPRN per resource_code
            meters_found = new List<SmartUtility.Meters>(meters_found.Distinct());
            return meters_found; //[0].MPAN_MPRN
        }

        //internal static string Utility_Lookup_Meters_UDPRN(MainViewModel ourviewmodel,
        //                                    char cubeface_code,
        //                                    string mpan_mprn,
        //                                    List<SmartUtility.Meters> metersList)
        //{
        //    List<SmartUtility.Meters> meters_found = new List<SmartUtility.Meters>();
        //    if (string.IsNullOrEmpty(mpan_mprn))
        //    {
        //        meters_found = new List<SmartUtility.Meters>
        //            (from Consumer in ourviewmodel.Hamas.consumersList
        //             join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
        //             on new { Consumer.USERNAME }
        //             equals new { Cubeface.USERNAME }
        //             join Meter in metersList
        //             on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
        //             equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
        //             where Consumer.Include && Cubeface.FACE_ACTIVE &&
        //                     Cubeface.CUBEFACE_CODE == cubeface_code
        //             select Meter);
        //    }
        //    else
        //    {
        //        meters_found = new List<SmartUtility.Meters>
        //            (from Consumer in ourviewmodel.Hamas.consumersList
        //             join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
        //             on new { Consumer.USERNAME }
        //             equals new { Cubeface.USERNAME }
        //             join Meter in metersList
        //             on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
        //             equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
        //             where Consumer.Include && Cubeface.FACE_ACTIVE &&
        //                     Cubeface.CUBEFACE_CODE == cubeface_code &&
        //                     Meter.MPAN_MPRN == mpan_mprn
        //             select Meter);
        //    }
        //    if (meters_found.Count > 0)
        //    {
        //        return meters_found.First().UDPRN;
        //    }
        //    return "";
        //}

        internal static string Utility_Lookup_Meters_MPANMPRN(MainViewModel ourviewmodel,
                                            char cubeface_code,
                                            char resource_code,
                                            List<SmartUtility.Meters> metersList)
        {
            List<SmartUtility.Meters> meters_found =
                new List<SmartUtility.Meters>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join Meter in metersList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                 where Meter.RESOURCE_CODE == resource_code
                 select Meter);
            meters_found = new List<SmartUtility.Meters>(meters_found.Distinct());
            if (meters_found.Count > 0)
            {
                return meters_found.First().MPAN_MPRN;
            }
            return "";
        }

        internal static char Utility_Lookup_Meters_ResourceCode(MainViewModel ourviewmodel,
                                            char cubeface_code,
                                            string mpan_mprn,
                                            List<SmartUtility.Meters> metersList)
        {
            List<SmartUtility.Meters> meters_found =
                new List<SmartUtility.Meters>
                (from Cubeface in SmartSpikeV2017.BasicCubeface(ourviewmodel, cubeface_code)

                 join Meter in metersList
                 on new { Cubeface.USERNAME, Cubeface.CUBEFACE_CODE }
                 equals new { Meter.USERNAME, Meter.CUBEFACE_CODE }
                 where Meter.MPAN_MPRN == mpan_mprn
                 select Meter);
            meters_found = new List<SmartUtility.Meters>(meters_found.Distinct());
            if (meters_found.Count > 0)
            {
                return meters_found.First().RESOURCE_CODE;
            }
            return SmartParametersV2016.defaultChar;
        }
        #endregion Smartutility
    }
}