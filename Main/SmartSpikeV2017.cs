using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


#if WINFORMS
using System.Linq;
using MoreLinq;// <= THANK FUCK FOR THIS -- I have had **MAJOR BUGS*** in my code!!!!

#endif

#if WPF
using MoreLinq;// <= THANK FUCK FOR THIS -- I have had **MAJOR BUGS*** in my code!!!!

#endif

#if WINUI
using MoreLinq;// <= THANK FUCK FOR THIS -- I have had **MAJOR BUGS*** in my code!!!!
using System;
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

    internal class SmartSpikeV2017
    {
        internal static List<SmartData.SQLiteTables> Lookup_SQLiteTables(MainViewModel ourviewmodel)
        {
            List<SmartData.SQLiteTables> tables_found =
                                new List<SmartData.SQLiteTables>
                                (from Tables in ourviewmodel.sqlitetablesList
#if PRODUCTION
                                 where Tables.PRODUCTION == 'P'
#endif
                                 select Tables);
            return tables_found;
        }

        internal static List<SmartData.SQLiteFields> Lookup_SQLiteFields(MainViewModel ourviewmodel,
                                                                                        string schema_name,
                                                                                        string table_name)
        {
            List<SmartData.SQLiteFields> fields_found =
                                new List<SmartData.SQLiteFields>
                                (from Tables in ourviewmodel.sqlitetablesList
                                 join Fields in ourviewmodel.sqlitefieldsList
                                 on new { Tables.SCHEMA_NAME, Tables.TABLE_NAME }
                                 equals new { Fields.SCHEMA_NAME, Fields.TABLE_NAME }
                                 where Tables.SCHEMA_NAME == schema_name &&
                                     Tables.TABLE_NAME == table_name
                                 orderby Fields.ORDINAL ascending
                                 select Fields);
            return fields_found;
        }
#if WINFORMS
        internal static List<SmartProfile.AddressesView> Configure_Users_Addresses(MainViewModel ourviewmodel)
        {
            List<SmartProfile.AddressesView> addresses_found =
                new List<SmartProfile.AddressesView>(from Address in ourviewmodel.SmartProfile.addressesviewList
                                                                   select Address);
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

        internal static List<SmartUsers.Consumers> Configure_Users_Consumers(MainViewModel ourviewmodel)
        {
            List<SmartUsers.Consumers> consumers_found = new List<SmartUsers.Consumers>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 where Consumer.Include
                 select Consumer);
            return consumers_found;
        }

        internal static List<SmartProfile.Cubefaces> Configure_Users_Cubefaces(MainViewModel ourviewmodel)
        {
            List<SmartProfile.Cubefaces> cubefaces_found = new List<SmartProfile.Cubefaces>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                 on new { Consumer.USERNAME }
                 equals new { Cubeface.USERNAME }
                 select Cubeface);
            return cubefaces_found;
        }

        internal static List<SmartProfile.Groups> Configure_Users_Groups(MainViewModel ourviewmodel)
        {
            List<SmartProfile.Groups> groups_found =
                new List<SmartProfile.Groups>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Group in ourviewmodel.SmartProfile.profilegroupsList
                 on new { Consumer.USERNAME }
                 equals new { Group.USERNAME }
                 where Consumer.Include
                 select Group);
            return groups_found;
        }

        internal static List<SmartProfile.Profiles> Configure_Users_Profiles(MainViewModel ourviewmodel)
        {
            List<SmartProfile.Profiles> profiles_found =
                new List<SmartProfile.Profiles>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Profile in ourviewmodel.SmartProfile.profilesList
                 on new { Consumer.USERNAME }
                 equals new { Profile.USERNAME }
                 where Consumer.Include
                 select Profile);
            return profiles_found;
        }

        //internal static List<SmartProfile.ProfilesCube> Configure_Users_ProfilesCube(MainViewModel ourviewmodel)
        //{
        //    List<SmartProfile.ProfilesCube> profilescube_found =
        //        new List<SmartProfile.ProfilesCube>
        //        (from Consumer in ourviewmodel.Hamas.consumersList
        //         join ProfileCube in ourviewmodel.SmartProfile.profilescubeList
        //        on new { Consumer.USERNAME }
        //        equals new { ProfileCube.USERNAME }
        //         where Consumer.Include
        //         select ProfileCube);
        //    return profilescube_found;
        //}
#endif
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        internal static List<SmartProfile.Cubefaces> BasicCubeface(MainViewModel ourviewmodel,
                                                            char cubefaceCode = '\0',
                                                           string UserName = "")
        {
            var cface = (from Consumer in ourviewmodel.Hamas.consumersList
                         join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                         on new { Consumer.USERNAME }
                         equals new { Cubeface.USERNAME }
                         where Consumer.Include &&
                               Cubeface.FACE_ACTIVE
                         select Cubeface).ToList();  // Convert to List right away to avoid reassigning it

            // Apply filtering based on UserName if provided
            if (!string.IsNullOrEmpty(UserName))
            {
                cface = cface.Where(Cubeface => Cubeface.USERNAME == UserName).ToList();
            }

            // Apply filtering based on cubefaceCode if provided
            if (cubefaceCode != '\0')
            {
                cface = cface.Where(Cubeface => Cubeface.CUBEFACE_CODE == cubefaceCode).ToList();
            }

            // Convert to List at the end for efficiency
            return cface;            
        }
        internal static SmartData.CultureView GetCurrencyInfo(SignInViewModel signinviewmodel,
                                                                CultureInfo[] ray,
                                                                CultureInfo code)
        {
            SmartData.CultureView cv = new SmartData.CultureView();
            signinviewmodel.errorMessage = "";
            try
            {
                RegionInfo regionInfo = (from Culture in ray // CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                         where Culture.Name.Length > 0 && !Culture.IsNeutralCulture
                                         let region = new RegionInfo(Culture.Name) // was LCID) but on Android this gave an error System.ArgumentException: Customized cultures cannot be passed by LCID, only by name. (Parameter 'culture')
                                         where Culture.Name == code.Name
                                         //where string.Equals(region.ISOConvertToSymbol, code.Name, StringComparison.InvariantCultureIgnoreCase)
                                         select region).First();
                cv.SYMBOL = regionInfo.CurrencySymbol;
                cv.ISOCURRENCYSYMBOL = regionInfo.ISOCurrencySymbol;
                cv.CULTUREINFO = code;
                // regionInfo.CurrencySymbol + " " + regionInfo.ISOCurrencySymbol);

                List<SmartData.Currencies> currencies_found =
                     new List<SmartData.Currencies>(from Currency in signinviewmodel.Fatah.currenciesList
                                                    where Currency.ISOCURRENCYSYMBOL == cv.ISOCURRENCYSYMBOL &&
                                                            Currency.ACTIVEFLAG     // Is true
                                                    select Currency);
                if (currencies_found.Count > 0)
                {
                    cv.CURRENCY_ORDINAL = currencies_found.First().ORDINAL;
                }
                else
                {
                    // Default to None
                    cv.CURRENCY_ORDINAL = 0;
                }
            }
            catch (Exception ex)
            {
                signinviewmodel.errorMessage = ex.Message;
            }
            return cv;
        }

//internal static CultureInfo GetCultureInfoByConvertToSymbolX(MainViewModel ourviewmodel,
//                                                            string ConvertToSymbol)
//{
//    if (!string.string.IsNullOrEmpty(ConvertToSymbol))
//    {
//        foreach (SmartData.CultureView cult in ourviewmodel.cultureviewList) // Checked
//        {
//            if (cult.ISOCurrencySymbol == ConvertToSymbol)
//            {
//                return cult.CULTUREINFO;
//            }
//        }
//    }
//    return SmartParametersV2016.defaultCulture;
//}
        internal static string Lookup_Button_Description(List<SmartData.Buttons> buttonsList,
                                                         char cubeface_code,
                                                         char button_code)
        {
            List<SmartData.Buttons> descriptions_found =
                                new List<SmartData.Buttons>
                                (from Button in buttonsList
                                 where Button.CUBEFACE_CODE == cubeface_code &&
                                        Button.BUTTON_CODE == button_code
                                 select Button);
            if (descriptions_found.Count > 0)
            {
                return descriptions_found[0].DESCRIPTION;
            }
            return "";
        }

        internal static bool CheckConsumerUser(MainViewModel ourviewmodel,
                                               string groupname)
        {
            // Doesn't matter whether its included or not?
            List<SmartUsers.Consumers> consumers_found =
                 new List<SmartUsers.Consumers>
                 (from Consumer in ourviewmodel.Hamas.consumersList
                  where Consumer.USERNAME == groupname
                  select Consumer);
            if (consumers_found.Count > 0)
            {
                return true;
            }
            return false;
        }

        internal static List<SmartUsers.Consumers> Find_ValidCubefaces(MainViewModel ourviewmodel,
                                                                        char cubeface_code)
        {
            List<SmartUsers.Consumers> consumers_found =
                                new List<SmartUsers.Consumers>
                    (from Consumer in ourviewmodel.Hamas.consumersList
                     join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                     on new { Consumer.USERNAME }
                     equals new { Cubeface.USERNAME }
                     where Consumer.Include &&
                            Cubeface.FACE_ACTIVE &&
                            Cubeface.CUBEFACE_CODE == cubeface_code
                     select Consumer);
            return consumers_found;
        }

        internal static List<SmartProfile.Profiles> Lookup_Profile(MainViewModel ourviewmodel)
        {
            List<SmartProfile.Profiles> profiles_found =
                                new List<SmartProfile.Profiles>
                    (from Consumer in ourviewmodel.Hamas.consumersList
                     join Profile in ourviewmodel.SmartProfile.profilesList
                     on new { Consumer.USERNAME }
                     equals new { Profile.USERNAME }
                     where Consumer.USERNAME == ourviewmodel.UserName &&
                           Consumer.Include
                     select Profile);
            return profiles_found;
        }

        internal static decimal[] Lookup_Todays_Exchange_Rate(List<SmartUsers.ExchangeRates> exchangeRatesList)
        {
            // Pretty sure our Exchange Rates are (will be) stored with UTC
            DateTime today = SmartTimeV2016.ConvertDateTime(DateTime.Now.Date.ToString(SmartParametersV2016.sqliteformat)); // Local time
            List<SmartUsers.ExchangeRates> exchange_rates_found =
                                new List<SmartUsers.ExchangeRates>
                (from ExchangeRate in exchangeRatesList
                 where ExchangeRate.TRANSACTION_DATE == today
                 orderby ExchangeRate.TRANSACTION_DATE descending
                 select ExchangeRate);
            if (exchange_rates_found.Count > 0)
            {
                return new decimal[4]
                {
                    exchange_rates_found.First().CURRENCY_RATE_01,
                    exchange_rates_found.First().CURRENCY_RATE_02,
                    exchange_rates_found.First().CURRENCY_RATE_03,
                    exchange_rates_found.First().CURRENCY_RATE_04
                };
            }
            return new decimal[4] { 0, 0, 0, 0 };
        }

        internal static string Lookup_Currency_Symbol(List<SmartData.Currencies> currenciesList,
                                                        short currency_ordinal)
        {
            // Maybe we should include culture_Viewlist in this select??
            // No, because it has nothing to do with Culture..
            if (currency_ordinal > 0)
            {
                List<SmartData.Currencies> currencies_found =
                                new List<SmartData.Currencies>
                   (from Currency in currenciesList
                    where Currency.ORDINAL == currency_ordinal &&
                        Currency.ACTIVEFLAG     // Is true
                    select Currency);
                if (currencies_found.Count > 0)
                {
                    return currencies_found.First().ISOCURRENCYSYMBOL;
                }
            }
            return "";
        }

        internal static short Lookup_Currency_OrdinalOld(List<SmartData.Currencies> currenciesList,
                                                                            string currency_symbol)
        {
            List<SmartData.Currencies> currencies_found =
                                new List<SmartData.Currencies>
                   (from Currency in currenciesList
                    where Currency.ISOCURRENCYSYMBOL == currency_symbol
                    select Currency);
            if (currencies_found.Count > 0)
            {
                return currencies_found.First().ORDINAL;
            }
            return 0;
        }

        internal static SmartData.CultureView Lookup_Currency_OrdinalNew(List<SmartData.CultureView> culture_viewList,
                                                        List<SmartData.Currencies> currenciesList,
                                                        string currency_symbol)
        {
            List<SmartData.CultureView> currencies_found =
                                new List<SmartData.CultureView>
                   (from Culture in culture_viewList
                    join Currency in currenciesList
                    on new { Culture.ISOCURRENCYSYMBOL }
                    equals new { Currency.ISOCURRENCYSYMBOL }
                    where Culture.ISOCURRENCYSYMBOL == currency_symbol
                    select Culture);
            if (currencies_found.Count > 0)
            {
                return currencies_found.First();
            }
            return new SmartData.CultureView();
        }

        //internal static CultureInfo Lookup_CURRENCY_DisplayCulture(List<SmartData.CultureView> culture_viewList,
        //                                                List<SmartData.Currencies> currenciesList,
        //                                                short currency_ordinal,
        //                                                string culture_code)
        //{
        //    List<SmartData.CultureView> cultures_found =
        //                        new List<SmartData.CultureView>
        //           (from Culture in culture_viewList
        //            join Currency in currenciesList
        //            on new { Culture.ISOConvertToSymbol }
        //            equals new { Currency.ISOConvertToSymbol }
        //            where Currency.ORDINAL == currency_ordinal
        //            select Culture);
        //    if (cultures_found.Count == 1)
        //    {
        //        return cultures_found.First().CULTUREINFO;
        //    }
        //    else
        //    {
        //        if (cultures_found.Count > 1)
        //        {
        //            List<SmartData.CultureView> exactcultures_found =
        //                        new List<SmartData.CultureView>
        //           (from Culture in cultures_found
        //            where Culture.CULTURE_CODE == culture_code
        //            select Culture);
        //            if (exactcultures_found.Count > 0)
        //            {
        //                return exactcultures_found.First().CULTUREINFO; // Should only be 1!
        //            }
        //            else
        //            {
        //                return cultures_found.First().CULTUREINFO;
        //            }
        //        }
        //    }
        //    return null;
        //}

        internal static SmartData.CultureView Lookup_CULTURE_Info(List<SmartData.CultureView> culturesList,
                                                    string culture_code)
        {
            List<SmartData.CultureView> cultures_found =
                                new List<SmartData.CultureView>
                       (from Culture in culturesList
                        where Culture.CULTURE_CODE == culture_code
                        select Culture);
            if (cultures_found.Count > 0)
            {
                return cultures_found.First();
            }
            return null;
        }

        internal static int Lookup_CULTURE_Index(List<SmartData.CultureView> culturesList,
                                                    string culture_code)
        {
            int index = culturesList.FindIndex(p => p.CULTURE_CODE == culture_code);
            if (index == -1)
            {
                string cultureCode = SmartParametersV2016.defaultCulture.ToString();
                RegionInfo region = new RegionInfo(cultureCode);

                string countryCode = region.TwoLetterISORegionName.ToUpper(); // "GB"
                index = culturesList.FindIndex(p => p.CULTURE_CODE == countryCode);
            }            
            return index;
        }
        internal static CultureInfo Find_CULTURE_Code(List<SmartData.Cultures> culturesList,
                                                    string culture_code)
        {
            List<SmartData.Cultures> cultures_found =
                                new List<SmartData.Cultures>
                   (from Culture in culturesList
                    where Culture.CULTURE_CODE == culture_code
                    select Culture);
            if (cultures_found.Count > 0)
            {
                return new CultureInfo(cultures_found.First().CULTUREINFO);
            }
            return SmartParametersV2016.defaultCulture;
        }

        //internal static short Lookup_Screen_Active(MainViewModel ourviewmodel,
        //                                            short scode)
        //{
        //    foreach (SmartProfile.Cubefaces cubeface in ourviewmodel.)
        //    List<SmartData.Cubefaces> cubefaces_found =
        //                        new List<SmartData.Cubefaces>
        //        (from Cubeface in signinviewmodel.Fatah.cubefacesList
        //         where Cubeface.CUBEFACE_CODE == cubeface_code &&
        //                 Cubeface.ACTIVEFLAG
        //         select Cubeface);
        //    if (cubefaces_found.Count > 0)
        //    {
        //        return cubefaces_found.First().SCREEN_CODE;
        //    }
        //    return -1;
        //}

        internal static short LookupScreenCode(List<SmartData.Cubefaces> CUBEFACESList,
                                                    char cubeface_code)
        {
            List<SmartData.Cubefaces> cubefaces_found =
                                new List<SmartData.Cubefaces>
                (from Cubeface in CUBEFACESList
                 where Cubeface.CUBEFACE_CODE == cubeface_code &&
                         Cubeface.ACTIVEFLAG
                 select Cubeface);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().SCREEN_CODE;
            }
            return -1;
        }

        internal static string LookupScreenDescription(List<SmartData.Cubefaces> CUBEFACESList,
                                                            char cubeface_code)
        {
            List<SmartData.Cubefaces> cubefaces_found =
                                new List<SmartData.Cubefaces>
                (from Cubeface in CUBEFACESList
                 where Cubeface.CUBEFACE_CODE == cubeface_code &&
                         Cubeface.ACTIVEFLAG
                 select Cubeface);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().DESCRIPTION;
            }
            return "";
        }
        internal static char Lookup_SignInCubeface_Code(SignInViewModel signinviewmodel,
                                                    short CurrentPosition)
        {
            List<SmartData.Cubefaces> cubefaces_found =
                        new List<SmartData.Cubefaces>
            (from Cubeface in signinviewmodel.Fatah.cubefacesList
             where Cubeface.SCREEN_CODE == CurrentPosition &&
                     Cubeface.ACTIVEFLAG
             select Cubeface);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().CUBEFACE_CODE;
            }
            return SmartParametersV2016.defaultChar;
        }

        internal static short Lookup_Cubeface_ScreenCode(SignInViewModel signinviewmodel,
                                                        char CubefaceCode)
        {
            List<SmartData.Cubefaces> cubefaces_found =
                        new List<SmartData.Cubefaces>
            (from Cubeface in signinviewmodel.Fatah.cubefacesList
             where Cubeface.CUBEFACE_CODE == CubefaceCode &&
                     Cubeface.ACTIVEFLAG
             select Cubeface);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().SCREEN_CODE;
            }
            return -1;
        }
        internal static List<SmartProfile.Cubefaces> OrderCUBEFACECode(List<SmartData.Cubefaces> CUBEFACESList,
                                                     MainViewModel ourviewmodel)
        {
            List<SmartProfile.Cubefaces> cubefaces_found =
                                new List<SmartProfile.Cubefaces>
                                (from Consumer in ourviewmodel.Hamas.consumersList
                                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                                 on new { Consumer.USERNAME }
                                 equals new { Cubeface.USERNAME }
                                 join Cubefaces in CUBEFACESList
                                 on new { Cubeface.CUBEFACE_CODE }
                                 equals new { Cubefaces.CUBEFACE_CODE }
                                 where Consumer.USERNAME == ourviewmodel.UserName
                                 orderby Cubefaces.SCREEN_CODE ascending
                                 select Cubeface);
            return cubefaces_found;
        }

        internal static short LookupCubefacesScreenCode(SignInViewModel signinviewmodel,
                                                        char cubefaceCode)
        {
            List<SmartData.Cubefaces> cubefaces_found =
                                new List<SmartData.Cubefaces>
                                (from Cubefaces in signinviewmodel.Fatah.cubefacesList                                 
                                 where Cubefaces.CUBEFACE_CODE == cubefaceCode
                                 select Cubefaces);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().SCREEN_CODE;
            }
            return 0;
        }

        internal static char LookupCubefacesCubefaceCode(SignInViewModel signinviewmodel,
                                                        short screenCode)
        {
            List<SmartData.Cubefaces> cubefaces_found =
                                new List<SmartData.Cubefaces>
                                (from Cubefaces in signinviewmodel.Fatah.cubefacesList
                                 where Cubefaces.SCREEN_CODE == screenCode
                                 select Cubefaces);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().CUBEFACE_CODE;
            }
            return SmartParametersV2016.Profiles;   // Default
        }
        internal static bool Lookup_CUBEFACE_CodeX(MainViewModel ourviewmodel,
                                                     char cubeface_code)
        {
            List<SmartProfile.Cubefaces> cubefaces_found =
                                new List<SmartProfile.Cubefaces>
                                (from Consumer in ourviewmodel.Hamas.consumersList
                                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                                 on new { Consumer.USERNAME }
                                 equals new { Cubeface.USERNAME }
                                 where (Consumer.USERNAME == ourviewmodel.UserName &&
                                     (Cubeface.FACE_ACTIVE) &&
                                     (Cubeface.CUBEFACE_CODE == cubeface_code))
                                 select Cubeface);
            if (cubefaces_found.Count > 0)
            {
                return true;
            }
            return false;
        }

        internal static char FindCubefaceLastDisplay(MainViewModel ourviewmodel)
        {
            List<SmartProfile.Cubefaces> cubefaces_found =
                                new List<SmartProfile.Cubefaces>
                                (from Consumer in ourviewmodel.Hamas.consumersList
                                 join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                                 on new { Consumer.USERNAME }
                                 equals new { Cubeface.USERNAME }
                                 where (Consumer.USERNAME == ourviewmodel.UserName &&
                                     (Cubeface.FACE_ACTIVE) &&
                                     (Cubeface.FACE_LAST_DISPLAY != ""))
                                 select Cubeface);
            if (cubefaces_found.Count > 0)
            {
                return cubefaces_found.First().CUBEFACE_CODE;
            }
            return SmartParametersV2016.Profiles;
        }
        internal static List<SmartUsers.Consumers> Lookup_Consumers(MainViewModel ourviewmodel,
                                                                    string username)
        {
            List<SmartUsers.Consumers> consumers_found =
                            new List<SmartUsers.Consumers>
            (from Consumer in ourviewmodel.Hamas.consumersList
             where Consumer.USERNAME == username
             select Consumer);
            return consumers_found;
        }

        internal static List<SmartProfile.Cubefaces> Load_CubefacesX(MainViewModel ourviewmodel)
        {
            List<SmartProfile.Cubefaces> cubefaces_found =
                                new List<SmartProfile.Cubefaces>
            (from Consumer in ourviewmodel.Hamas.consumersList
             join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
             on new { Consumer.USERNAME }
             equals new { Cubeface.USERNAME }
             join MasterCubeface in ourviewmodel.CUBEFACESList
             on new { Cubeface.CUBEFACE_CODE }
             equals new { MasterCubeface.CUBEFACE_CODE }
             where MasterCubeface.ACTIVEFLAG &&
                   MasterCubeface.SCREEN_CODE > 0
             orderby MasterCubeface.SCREEN_CODE ascending
             select Cubeface);
            return cubefaces_found;
        }

        internal static List<SmartProfile.AddressesView> Users_Lookup_AddressesUDPRN(MainViewModel ourviewmodel,
                                                        string username,
                                                        string udprn)
        {
            List<SmartProfile.AddressesView> addresses_found =
                new List<SmartProfile.AddressesView>();
            // Only if its not 0 and we have something in the list
            if (!string.IsNullOrEmpty(udprn))
            {
                addresses_found = new List<SmartProfile.AddressesView>(from Address
                                        in ourviewmodel.SmartProfile.addressesviewList
                                                                     where (Address.USERNAME == username &&
                                                                         Address.UDPRN == udprn)
                                                                     select Address);
            }
            return addresses_found;
        }

        internal static List<SmartUsers.ExternalResources> Main_Lookup_External(List<SmartUsers.ExternalResources> externalResourcesList,
                                                                                            short external_id)
        {
            List<SmartUsers.ExternalResources> externalResources_found = new List<SmartUsers.ExternalResources>();
            // Only if its not 0 and we have something in the list
            if (external_id > 0)
            {
                externalResources_found = new List<SmartUsers.ExternalResources>(from ExternalResource
                                                                                in externalResourcesList
                                                                                 where (ExternalResource.EXTERNAL_ID == external_id)
                                                                                 select ExternalResource);
            }
            return externalResources_found;
        }

        internal static List<SmartProfile.AddressesView> UsersLookupTextAddress(MainViewModel ourviewmodel,
                                                                                            string address)
        {
            List<SmartProfile.AddressesView> addressesview_found = new List<SmartProfile.AddressesView>();
            // 'address' SHOULD already have been checked as *not" empty!
            // UDPRN not guaranteed unique!
            addressesview_found = new List<SmartProfile.AddressesView>
            (from Consumer in ourviewmodel.Hamas.consumersList
             join Address in ourviewmodel.SmartProfile.addressesviewList
             on new { Consumer.USERNAME }
             equals new { Address.USERNAME }
             where Consumer.Include &&
                 Address.USERNAME == ourviewmodel.UserName &&
                 Address.BASIC == address
             select Address);
            // UDPRN not guaranteed unique! But yes it will be
            // for each User they can have UDPRN F and UDPRN U
            // Just to be on the safe side (might not need this)
            addressesview_found = new List<SmartProfile.AddressesView>
                (addressesview_found.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.UDPRN       // !!!
                }));
            //List<SmartUsers.Addresses> addresses_found = new List<SmartUsers.Addresses>();
            //if (addressesview_found.Count > 0)
            //{
            //    addresses_found = new List<SmartUsers.Addresses>
            //    (from Addresses in ourviewmodel.Hamas.addressesList
            //     orderby Addresses.ADDRESS_CREATED descending
            //     where Addresses.UDPRN == addressesview_found.First().UDPRN
            //     select Addresses);
            //}
            //// Now hang on! we might have more than one
            //// matching UDPRN !!
            //// One for F and one for U !!!
            //addresses_found = new List<SmartUsers.Addresses>
            //    (addresses_found.DistinctBy(key => new
            //    {
            //        key.USERNAME,
            //        key.UDPRN       // !!!
            //    }));
            return addressesview_found;
        }

        internal static List<SmartProfile.Cubefaces> Lookup_Cubefaces(MainViewModel ourviewmodel,
                                                        string username,
                                                        char cubeface_code)
        {
            List<SmartProfile.Cubefaces> cubefaces_found = new List<SmartProfile.Cubefaces>();
            // Only if its not 0 and we have something in the list
            if (cubeface_code != SmartParametersV2016.defaultChar)
            {
                cubefaces_found = new List<SmartProfile.Cubefaces>
                    (from Consumer in ourviewmodel.Hamas.consumersList
                     join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                     on new { Consumer.USERNAME }
                     equals new { Cubeface.USERNAME }
                     where Consumer.USERNAME == username &&
                         Cubeface.CUBEFACE_CODE == cubeface_code
                     select Cubeface);
            }
            else
            {
                cubefaces_found = new List<SmartProfile.Cubefaces>
                    (from Consumer in ourviewmodel.Hamas.consumersList
                     join Cubeface in ourviewmodel.SmartProfile.profilecubefacesList
                     on new { Consumer.USERNAME }
                     equals new { Cubeface.USERNAME }
                     where Consumer.USERNAME == username
                     select Cubeface);
            }
            return cubefaces_found;
        }

        internal static List<SmartProfile.AddressesView> Users_Lookup_Addresses(MainViewModel ourviewmodel)
        {
            // There is NO JOIN to Cubefaces here, because addresses
            // are not limited to one Cubeface - in fact you WANT
            // addresses to be visible across ALL Cubefaces so that you
            // can link Finance AND Utility AND Insurance et. al.
            List<SmartProfile.AddressesView> addresses_found = new List<SmartProfile.AddressesView>();
            addresses_found = new List<SmartProfile.AddressesView>
                (from Consumer in ourviewmodel.Hamas.consumersList
                 join Address in ourviewmodel.SmartProfile.addressesviewList
                 on new { Consumer.USERNAME }
                 equals new { Address.USERNAME }
                 where Consumer.Include
                 select Address);
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
        internal static List<SmartUsers.Working_Postcodes> Find_Working_Postcodes(
                                                                        List<SmartUsers.Working_Postcodes> postcodesList,
                                                                        string postcode)
        {
            List<SmartUsers.Working_Postcodes> working_postcodes_found =
                                new List<SmartUsers.Working_Postcodes>
                (from Postcode in postcodesList
                 where (Postcode.POSTCODE == postcode)
                 select Postcode);
            return working_postcodes_found;
        }

        internal static List<SmartUsers.ExchangeRatesView> ExchangeRates_Reduced(MainViewModel ourviewmodel,
                                                                                                DateTime target_date)
        {
            return new List<SmartUsers.ExchangeRatesView>
                (from ER in ourviewmodel.Blanche.exchangeRatesList
                 where ER.TRANSACTION_DATE >= target_date
                 orderby ER.TRANSACTION_DATE descending
                 select new SmartUsers.ExchangeRatesView  // Can use 'new' because we specify the fields below
                 {
                     TRANSACTION_DATE = ER.TRANSACTION_DATE.ToString("ddd dd-MMM-yyyy"),
                     ECB = ER.ECB == 1 ? "Yes" : "No",
                     CURRENCY_RATE_01 = ER.CURRENCY_RATE_01,
                     CURRENCY_RATE_02 = ER.CURRENCY_RATE_02,
                     CURRENCY_RATE_03 = ER.CURRENCY_RATE_03,
                     //CURRENCY_RATE_04 = ER.CURRENCY_RATE_04,
                     CURRENCY_RATE_04 = ER.CURRENCY_RATE_04
                 });
        }
#endif
    }
}