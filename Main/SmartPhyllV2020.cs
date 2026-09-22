// Auntie Phyll's birthday - 17th April 1921?
// In memory of my good friend Penny White who died in June 2014.  She of the pig-tails which I 
// Sent Amanda a Christmas message yesterday 5th Dec 2025
// foolishly yanked and earned a thumping when she caught up with me!
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using SQLite;

//using static System.Runtime.InteropServices.JavaScript.JSType;

#if WINFORMS
using System.Windows;
#endif

#if WPF
using System.Windows;
using System.Xml.Serialization;
using static SmartCubeMobile.SmartData;
using System.Diagnostics.Eventing.Reader;
using System.Security.Policy;

#endif

#if WINUI
using Microsoft.UI.Xaml;
#endif

#if ANDROIDX
using Android.Views;
//using System.Runtime.InteropServices.Marshalling;
using AndroidX.AppCompat.App;
using Java.Lang.Annotation;
#endif


namespace SmartCubeMobile
{
    public static class SmartPhyllV2020
    {
        internal static List<T> FindListTable<T>(List<object> smartcubeList)
        {
            // I'm desperate ...
            foreach (object item in smartcubeList) // Checked
            {
                Type t = item.GetType();
                if (t == typeof(List<T>))
                {
                    return (List<T>)Convert.ChangeType(item, typeof(List<T>), null);
                }
            }
            return new List<T>();
        }

        internal static string DecodeToSQLUsername<T>(T sqlite_row, bool add_apostrophes, char separator)
        {
            FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);
            string sql = "";

            // Include the Username!!
            for (int index = 0; index < myFields.Length; index++)
                {
                    if (myFields[index].Name != "Updated" &&
                        myFields[index].Name != "Delete")
                    {
                        if (!string.IsNullOrEmpty(sql))
                        {
                            // There are no strings in Consumers.SQLite (hence no embedded commas!)
                            // Yes but to be consistent we need to be able to split in the
                            // same manner we combined!
                            // But we also need to build to insert into SQLite wiv commas!!
                            sql += separator;
                        }
                        switch (myFields[index].FieldType.FullName)
                        {
                            case "System.Byte":
                                sql += myFields[index].GetValue(sqlite_row).ToString();
                                break;
                            case "System.DateTime":
                                if (add_apostrophes) sql += "'";
                                // Yes, yes, yes, yes. I know. If the underlying system's culture
                                // is "en-GB" then this next conversion is a waste of time,
                                // as it's unnecessary.  However - for an Android emulator -
                                // and for those millions of systems throughout the world
                                // where the underlying system's culture is *not* "en-GB"
                                // then this really is necessary as I need to keep ALL the
                                // extracted and converted dates in one format i.e. "en-GB"
                                // So LIVE WITH IT for 'en-GB' systems. Don't FUCK with it
                                // - it works, and if it ain't broke, then don't fix it ...
                                string icow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                                sql += SmartTimeV2016.ConvertDateTime(icow).ToString(SmartParametersV2016.sqliteformat);

                                if (add_apostrophes) sql += "'";
                                break;
                            case "System.Long":
                                //string tyme = myFields[index].GetValue(target_row).ToString();
                                sql += DateTime.UtcNow.Ticks.ToString();    // UTC time
                                break;
                            case "System.Char":
                                char temp = Convert.ToChar(myFields[index].GetValue(sqlite_row).ToString());
                                if (temp == SmartParametersV2016.defaultChar)
                                {
                                    if (add_apostrophes)
                                    {
                                        sql += "''";
                                    }
                                    else
                                    {
                                        sql += "";
                                    }
                                }
                                else
                                {
                                    if (add_apostrophes) sql += "'";
                                    sql += temp;
                                    if (add_apostrophes) sql += "'";
                                }
                                break;
                            case "System.Int16":
                                sql += myFields[index].GetValue(sqlite_row).ToString();
                                break;
                            case "System.Int32":
                                sql += myFields[index].GetValue(sqlite_row).ToString();
                                break;
                            case "System.Int64":
                                sql += myFields[index].GetValue(sqlite_row).ToString();
                                break;
                            case "System.Decimal":
                                sql += myFields[index].GetValue(sqlite_row).ToString();
                                break;
                            case "System.Decimal[]":
                                decimal[] ffs;// = new decimal[4] { 0, 0, 0, 0 };
                                ffs = (decimal[])myFields[index].GetValue(sqlite_row);
                                //target_row[index] = Convert.ToDecimal(ffs[0]);
                                if (add_apostrophes) sql += "'";
                                sql += ffs[0].ToString() + SmartParametersV2016.unitSeparator +
                                        ffs[1].ToString() + SmartParametersV2016.unitSeparator +
                                        ffs[2].ToString() + SmartParametersV2016.unitSeparator +
                                        ffs[3].ToString();
                                if (add_apostrophes) sql += "'";
                                break;
                            case "System.String":
                                if (add_apostrophes) sql += "'";
                                sql += myFields[index].GetValue(sqlite_row).ToString();
                                if (add_apostrophes) sql += "'";
                                break;
                            case "System.Boolean":
                                sql += myFields[index].GetValue(sqlite_row).ToString().ToLower();
                                break;
                            case "System.Double":
                            case "System.Float":
                            case "System.Single":
                                sql += myFields[index].GetValue(sqlite_row).ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
            return sql;
        }
        internal static string DecodeToSQLNoUsername<T>(T sqlite_row, bool add_apostrophes, char separator, bool userReqd = false)
        {
            FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);
            string sql = "";
            // Check to see if we send back the Username
            int start = 1;
            if (userReqd)
            {
                start = 0;
            }
            // Miss out Username?
            for (int index = start; index < myFields.Length; index++)
            {
                if (myFields[index].Name != "Updated" &&
                    myFields[index].Name != "Delete")
                {
                    if (!string.IsNullOrEmpty(sql))
                    {
                        // There are no strings in Consumers.SQLite (hence no embedded commas!)
                        // Yes but to be consistent we need to be able to split in the
                        // same manner we combined!
                        // But we also need to build to insert into SQLite wiv commas!!
                        sql += separator;
                    }
                    switch (myFields[index].FieldType.FullName)
                    {
                        case "System.Byte":
                            sql += myFields[index].GetValue(sqlite_row).ToString();
                            break;
                        case "System.DateTime":
                            if (add_apostrophes) sql += "'";
                            // Yes, yes, yes, yes. I know. If the underlying system's culture
                            // is "en-GB" then this next conversion is a waste of time,
                            // as it's unnecessary.  However - for an Android emulator -
                            // and for those millions of systems throughout the world
                            // where the underlying system's culture is *not* "en-GB"
                            // then this really is necessary as I need to keep ALL the
                            // extracted and converted dates in one format i.e. "en-GB"
                            // So LIVE WITH IT for 'en-GB' systems. Don't FUCK with it
                            // - it works, and if it ain't broke, then don't fix it ...
                            string icow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                            sql += SmartTimeV2016.ConvertDateTime(icow).ToString(SmartParametersV2016.sqliteformat);

                            if (add_apostrophes) sql += "'";
                            break;
                        case "System.Long":
                            //string tyme = myFields[index].GetValue(target_row).ToString();
                            sql += DateTime.UtcNow.Ticks.ToString();    // UTC time
                            break;
                        case "System.Char":
                            char temp = Convert.ToChar(myFields[index].GetValue(sqlite_row).ToString());
                            if (temp == SmartParametersV2016.defaultChar)
                            {
                                if (add_apostrophes)
                                {
                                    sql += "''";
                                }
                                else
                                {
                                    sql += "";
                                }
                            }
                            else
                            {
                                if (add_apostrophes) sql += "'";
                                sql += temp;
                                if (add_apostrophes) sql += "'";
                            }
                            break;
                        case "System.Int16":
                            sql += myFields[index].GetValue(sqlite_row).ToString();
                            break;
                        case "System.Int32":
                            sql += myFields[index].GetValue(sqlite_row).ToString();
                            break;
                        case "System.Int64":
                            sql += myFields[index].GetValue(sqlite_row).ToString();
                            break;
                        case "System.Decimal":
                            sql += myFields[index].GetValue(sqlite_row).ToString();
                            break;
                        case "System.Decimal[]":
                            decimal[] ffs;// = new decimal[4] { 0, 0, 0, 0 };
                            ffs = (decimal[])myFields[index].GetValue(sqlite_row);
                            //target_row[index] = Convert.ToDecimal(ffs[0]);
                            if (add_apostrophes) sql += "'";
                            sql += ffs[0].ToString() + SmartParametersV2016.unitSeparator +
                                    ffs[1].ToString() + SmartParametersV2016.unitSeparator +
                                    ffs[2].ToString() + SmartParametersV2016.unitSeparator +
                                    ffs[3].ToString();
                            if (add_apostrophes) sql += "'";
                            break;
                        case "System.String":
                            if (add_apostrophes) sql += "'";
                            sql += myFields[index].GetValue(sqlite_row).ToString();
                            if (add_apostrophes) sql += "'";
                            break;
                        case "System.Boolean":
                            sql += myFields[index].GetValue(sqlite_row).ToString().ToLower();
                            break;
                        case "System.Double":
                        case "System.Float":
                        case "System.Single":
                            sql += myFields[index].GetValue(sqlite_row).ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
            return sql;
        }
        internal static List<T> BuildSQLiteRecords<T>(object viewmodel,
                                                      string[] ray,
                                                      string username) where T: new()// Always valid
        {
            FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);

            List<T> list_records = new List<T>();
            int ray_count = 0;
            while (ray_count < ray.Length) // or here
            {
                if (!string.IsNullOrEmpty(ray[ray_count]))
                {
                    // https://msdn.microsoft.com/en-us/library/0hcyx2kd(v=vs.110).aspx
                    T target_row = Activator.CreateInstance<T>();

                    // If we are loading SMARTMUM, then the Username will be empty
                    // If we are loading SmartSwitch then the Username should contain something
                    string[] items;
                    if (!string.IsNullOrEmpty(username))
                    {
                        items = (username + SmartParametersV2016.fieldSeparator + ray[ray_count]).Split(SmartParametersV2016.fieldSeparator);
                    }
                    else
                    {
                        items = ray[ray_count].Split(SmartParametersV2016.fieldSeparator);
                    }
                    int upper_limit = Foreigners_Preamble(myFields,
                                                                items.Length);
                    for (int index = 0; index < upper_limit; index++)
                    {
                        string item = items[index];
                        switch (myFields[index].FieldType.FullName)
                        {
                            case "System.Byte":
                                myFields[index].SetValue(target_row, Convert.ToByte(item));
                                break;
                            case "System.DateTime":
                                DateTime targetDate = SmartParametersV2016.defaultDate;
                                if (Generic_Parse_Datetime_Culture(viewmodel,
                                                                    SmartParametersV2016.defaultCulture,
                                                                    items[index].ToString(),
                                                                    out targetDate))
                                {
                                    myFields[index].SetValue(target_row, targetDate);
                                }
                                else
                                {
                                    myFields[index].SetValue(target_row, SmartParametersV2016.defaultDate.ToString(SmartParametersV2016.sqldateFormat));
                                }
                                break;
                            case "System.Char":
                                myFields[index].SetValue(target_row, Convert.ToChar(item, SmartParametersV2016.defaultCulture));
                                break;
                            case "System.Int16":
                                myFields[index].SetValue(target_row, Convert.ToInt16(item));
                                break;
                            case "System.Int32":
                                myFields[index].SetValue(target_row, Convert.ToInt32(item));

                                break;
                            case "System.Int64":
                                // This is an alias for System.Long (which doesn't exist)
                                // There are two 'long' fields defined in SmartSwicth
                                // both for SEQUENCE_NOs. TransactionsCategories is ??? encrypted
                                // but CryptoTransaction is not.
                                // Why ticks? 'Cos I used to think it was a date!
                                long ticks = Convert.ToInt64(item);
                                myFields[index].SetValue(target_row, ticks); // Convert.ToInt64(item));
                                break;
                            case "System.Decimal":
                                myFields[index].SetValue(target_row, Convert.ToDecimal(item, SmartParametersV2016.defaultCulture)); // en-GB);
                                break;
                            case "System.String":
                                myFields[index].SetValue(target_row, item.ToString().Trim());
                                break;
                            case "System.Boolean":
                                myFields[index].SetValue(target_row, Convert.ToBoolean(item));
                                break;
                            case "System.Double":
                                myFields[index].SetValue(target_row, Convert.ToDouble(item));
                                break;
                            case "System.Float":
                            case "System.Single":
                                myFields[index].SetValue(target_row, float.Parse(item));
                                break;
                            default:
                                break;
                        }
                    }
                    list_records.Add(target_row);
                }
                ray_count++;
            }
            return list_records;
        }

        private static int Foreigners_Preamble(FieldInfo[] myFields,
                                                int items_count)
        {
            int upper_limit = items_count;
            if (myFields.Length < upper_limit)
            {
                upper_limit = myFields.Length;
            }
            return upper_limit;
        }

        internal static void BuildSQLiteRecordsMultiple<T>(object viewmodel,
                                        string[] ray,
                                        List<object> multipleList,
                                        string username = "") where T : new()
        {
            multipleList.Add(BuildSQLiteRecords<T>(viewmodel, ray, username));
        }
        internal static string BuildEncryptKey(string DEK, int random, int derived)
        {
            if (random < 0)
            {
                random += derived;
                return random.ToString() + DEK;
            }
            random -= derived;
            return DEK + random.ToString();
        }

        
        internal static bool Generic_Parse_Datetime_Culture(object viewmodel,
                                                            CultureInfo culture,
                                                            string date_value,
                                                            out DateTime targetDate)

        {
            // Always uses en-GB

            //if (date_value.Length > 10)
            //{
            //    if (date_value.IndexOf("00:00:00") == -1)
            //    {

            //    }
            //}

            // Doesn't seem to work for Utc dates =:-[
            if (DateTime.TryParse(date_value, culture, DateTimeStyles.AdjustToUniversal, out targetDate))
            {
                return true;
            }
            switch (viewmodel)
            {
                case MainViewModel ourviewmodel:
                    ourviewmodel.errorMessage = "Cannot convert date: " + date_value;
                    break;
                case SignInViewModel signinviewmodel:
                    signinviewmodel.errorMessage = "Cannot convert date: " + date_value;
                    break;
#if DBSERVER
                case SmartDBServer.ServerModel servermodel:
                    servermodel.errorMessage = "Cannot convert date: " + date_value;
                    break;
#endif
                default:
                    break;
            }
            return false;
        }
        
        internal static async Task<bool> GetDates(MainViewModel ourviewmodel)
        {
            try
            {
                string sql_select = "SELECT * FROM [SmartUsers.Consumers] WHERE USERNAME = '" +
                                    ourviewmodel.UserName + "';";
                List<SmartUsers.ConsumersSQLite> sqliteList = new List<SmartUsers.ConsumersSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUsers.ConsumersSQLite>(sql_select));
                if (sqliteList.Count > 0)
                {
                    ourviewmodel.utcDates = DecodeToSQLUsername<SmartUsers.ConsumersSQLite>(sqliteList.First(), false, SmartParametersV2016.commachar);
                }
            }
            catch (Exception sqlex)
            {
                // Its not there OR there was a problem
                ourviewmodel.errorMessage = sqlex.Message;
                return false;
            }
            return true;
        }

#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
        internal static async Task<bool> CheckSQLiteNew(
#if ANDROIDX
                                            AppCompatActivity meterActivity,
#endif
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel, 
                                            string dbName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(ourviewmodel.DataBasePath)) // System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                {
                    ourviewmodel.errorMessage = "Database path is empty ..";
#if !DBSERVER
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, ourviewmodel.errorMessage);
#endif
                    return false;
                }
                // Where is our Username?  Suppose two different users
                // use the same laptop???  Suppose one users uses WPF and then WINUI???
                string fullfilepath = "";
                if (string.IsNullOrEmpty(dbName))
                {
                    fullfilepath = System.IO.Path.Combine(ourviewmodel.DataBasePath,
                                                    ourviewmodel.UserName +
                                                    //SignIn.signinviewmodel.Platform + 
                                                    "_" +
                                                    SmartParametersV2016.DatabaseFilename);
                }
                else
                {
                    fullfilepath = System.IO.Path.Combine(ourviewmodel.DataBasePath,
                                                    dbName);

                }
#if WINFORMS
                fullfilepath = ourviewmodel.DataBasePath;
#endif
                // Either open an existing DB or create one if its not there
                // Any new DB will have NO TABLES inside it!
                if (!SmartSwitchDatabase.OpenDatabase(ourviewmodel, fullfilepath, flags: ourviewmodel.CreateFlags))
                //if (!ok)
                {
                    // error message should be set in ourviewmodel
#if !DBSERVER
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, ourviewmodel.errorMessage);
#endif
                    return false;
                }
            }
            catch (Exception sqlex)
            {
                // Its not there OR there was a problem
                ourviewmodel.errorMessage = sqlex.Message;
                return false;
            }
#if !DBSERVER
            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Load status: " + "Database Open or Created");
#endif

            
            try 
            {
                string sql = "SELECT name FROM sqlite_master WHERE type = 'table'";
                List<TableName> sqltablesList = await SmartSwitchDatabase.GetTableNamesAsync(ourviewmodel, ourviewmodel.sqliteDatabase, sql);

                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
#if !DBSERVER
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, ourviewmodel.errorMessage);
#endif
                    return false;
                }                
#if !DBSERVER
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "SSQLite table name list: " + sqltablesList.Count);
#endif
                
                // Now do the rest because SmartUsers.Consumers is either already THERE 
                // OR we have just created it.
                ourviewmodel.tablesCreated = 0;
                int result = 0;
                foreach (SmartData.SQLiteTables sqlitename in ourviewmodel.sqlitetablesList)
                {
                    sql = "";
                    string target = sqlitename.SCHEMA_NAME + "." + sqlitename.TABLE_NAME;

                    TableName match = sqltablesList.FirstOrDefault(t =>
                        string.Equals(t.Name, target, StringComparison.OrdinalIgnoreCase));
                    if (match == null)
                    {
                        sql = BuildSQLite(ourviewmodel, target);
                        if (!string.IsNullOrEmpty(sql))
                        {
                            try
                            {
                                result = await SmartSwitchDatabase.ExecuteSmartSwitchTableAsync(ourviewmodel.sqliteDatabase, sql);
                            }
                            catch (Exception ex)
                            {
                                ourviewmodel.errorMessage = "SQLite table problem " + ex.Message;
#if !DBSERVER
                                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                    meterActivity,
#endif
                                    ourviewmodel, ourviewmodel.errorMessage);
#endif
                                return false;
                            }
                            ourviewmodel.tablesCreated++;
                        }
                    }
                }

                // Now here the tables could all be NEW or, they
                // could already exist.  But to get a flag to tell us
                // to load SmartSwitch from SQLServer we need to
                // see if SmartUser.Consumers has a record for the Username
                // So here goes

                //
                // IF all the tables have been created from scratch!
                // Then the DB didn't exists so we have to populate it
#if !DBSERVER
                if (ourviewmodel.tablesCreated > 0)
                {
                    ourviewmodel.localOnly = false;  // Go get it from DBServer

                    // Mock up a blank record and get the default dates
                    // This means 'every date is 1900-01-01 so replace every table'
                    ourviewmodel.utcDates = SmartParametersV2016.wildcard;


                    // So HERE is where we download OUR SmartSwitch from the Remote
                    // DBServer SQLite DB because we haven't got a Consumer record
                    // and if we haven't got one of those we can do FUCK ALL.


                    // Load from Remote, but if its not our Username
                    // we only bring back AddressesView
                    string mmSchemas = SmartParametersV2016.SmartUsersSchema +
                                        SmartParametersV2016.unitSeparator +
                                        SmartParametersV2016.SmartProfileSchema +
                                        SmartParametersV2016.unitSeparator +
                                        SmartParametersV2016.SmartFinanceSchema +
                                        SmartParametersV2016.unitSeparator +
                                        SmartParametersV2016.SmartUtilitySchema;


                    // In here we get all the data for the list above
                    // FOR OUR CURRENT USERNAME!
                    if (!await SmartNibbyV2016.MiserableBitch(signinviewmodel,
                                                                ourviewmodel,
                                                                ourviewmodel.UserName,
                                                                ourviewmodel.UserName,
                                                                mmSchemas))
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Miserable Bitch failed 3" + ourviewmodel.errorMessage);
                        // This works because we're not updating the main thread
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "Miserable Bitch failed 3 " + ourviewmodel.errorMessage);
                        return false;
                    }
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "SQLite database: " + "Restored");

                    if (!await SmartPhyllV2020.LoadSmartUsersX(ourviewmodel,
                                                    //ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
                                                    SmartParametersV2016.SmartUsersSchema,
                                                    ourviewmodel.UserName,
                                                    ourviewmodel.UserName))
                    {
                        return false;
                    }

                    if (!await SmartPhyllV2020.LoadSmartProfile(ourviewmodel,
                                                    ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
                                                    SmartParametersV2016.SmartProfileSchema,
                                                    ourviewmodel.UserName,
                                                    ourviewmodel.UserName))
                    {
                        return false;
                    }
                }


                if (!await ExtractSmartUsersX(ourviewmodel,
                                                    ourviewmodel.sqlitetablesList,
                                                    true,
                                                    SmartParametersV2016.SmartUsersSchema,
                                                    ourviewmodel.UserName,
                                                    ourviewmodel.UserName)) // Nothing to decode in SmartUsers

                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "SQLite failed 2");
                    // This works because we're not updating the main thread
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "SQLite failed 2"))
                    {
                        return false;
                    }
                    return false;
                }

                if (!await SmartPhyllV2020.ExtractSmartProfile(ourviewmodel,
                                                        ourviewmodel.sqlitetablesList,
                                                        //ourviewmodel.CUBEFACESList[0].SCREEN_CODE,
                                                        true,
                                                        SmartParametersV2016.SmartProfileSchema,
                                                        ourviewmodel.UserName,
                                                        ourviewmodel.PDEK))    // Used to decode AddressesView and our Profile

                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "SQLite failed 3");
                    // This works because we're not updating the main thread
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "SQLite failed 3"))
                    {
                        return false;
                    }
                    return false;
                }
                
                if (ourviewmodel.Hamas.consumersList.Count == 0)
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "No Consumer list found");
                    // This works because we're not updating the main thread
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.quitCts.Token, 0, 0, "SQLite failed 4"))
                    {
                        return false;
                    }
                    return false;
                }
                
                ourviewmodel.localOnly = ourviewmodel.Hamas.consumersList.First().LOCAL_ONLY;
                // Get the UtcDates from the first record
                if (!await SmartPhyllV2020.GetDates(ourviewmodel))
                {
#if !DBSERVER
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "GetDates failed");
#endif
                    return false;
                }
                else
                {
                    bool setAsterisk = true;
                    string[] stems = ourviewmodel.utcDates.Split(SmartParametersV2016.comma);
                    for (int i = 5; i < stems.Length; i++)
                    {
                        if (stems[i] != SmartParametersV2016.sqldefaultdates)
                        {
                            setAsterisk = false;
                            break;
                        }
                    }
                    if (setAsterisk)
                    {
                        ourviewmodel.utcDates = SmartParametersV2016.wildcard;
                    }
                }
#endif
            }
            catch (Exception sqlex)
            {
                // Its not there OR there was a problem
                ourviewmodel.errorMessage = sqlex.Message;
                return false;
            }
#if !DBSERVER
            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Load status: " + ourviewmodel.localOnly);
#endif
            return true;
        }
#endif
        internal static string BuildSQLite(MainViewModel ourviewmodel, string table_name)
        {
            string constraint = "";
            string sql = "CREATE TABLE " + '"' + table_name + '"' + "(";
            string[] items = table_name.Split('.');

            List<SmartData.SQLiteFields> abc = SmartSpikeV2017.Lookup_SQLiteFields(ourviewmodel, items[0], items[1]);
            int count = 0;
            foreach (SmartData.SQLiteFields field in abc)
            {
                if (count > 0)
                {
                    sql += ",";
                }
                sql = sql + field.FIELD_NAME + " ";
                if (field.KEY_FIELD == 'Y')
                {
                    if (count > 0)
                    {
                        constraint += ",";
                    }
                    constraint += field.FIELD_NAME;
                }
                sql += field.FIELD_TYPE;
                switch (field.FIELD_TYPE)
                {
                    case "nvarchar":
                        if (field.FIELD_LENGTH < 0)
                        {
                            sql += "(" + "2147483647" + ")";
                        }
                        else
                        {
                            sql += "(" + field.FIELD_LENGTH + ")";
                        }
                        break;
                    case "char":
                    case "nchar":
                        sql += "(" + field.FIELD_LENGTH + ")";
                        break;
                    case "text":
                        if (field.FIELD_LENGTH > 0)
                        {
                            sql += "(" + field.FIELD_LENGTH + ")";
                        }
                        break;
                    case "numeric":
                        short lhs = Convert.ToInt16(field.FIELD_LENGTH / 16);
                        short rhs = Convert.ToInt16(field.FIELD_LENGTH - (lhs * 16));
                        sql += "(" + lhs + "," + rhs + ")";
                        break;
                    case "int":
                        if (field.FIELD_LENGTH > 0)
                        {
                            sql += "(" + field.FIELD_LENGTH + ")";
                        }
                        break;
                    case "bit":
                    case "smallint":
                    case "datetime":
                        break;
                    default:
                        break;
                }
                sql += " NOT NULL";
                count++;
            }
            if (!string.IsNullOrEmpty(constraint))
            {
                sql = sql + ", CONSTRAINT[sqlite_autoindex_" + 
                                table_name + 
                                "_1] PRIMARY KEY(" +
                                constraint + ")";
            }
            sql += ");";
            return sql;
        }

        internal static async Task<bool> StoreSQLite<T1>(MainViewModel ourviewmodel,
                                                            string schema_name,
                                                            string table_name,
                                                            string username,
                                                            string groupname)
        {
            // Build the internal table from the downloaded stuff
            SmartSwitchItem match = ourviewmodel.myList
                                    .FirstOrDefault(x => x.User == username && 
                                                    x.Group == groupname);

            object smartSwitchList = match?.SmartSwitchList;
            List<T1> sqliteList = FindListTable<T1>((List<object>)smartSwitchList);

            //List<T1> match = ourviewmodel.myList
            //    .OfType<SmartSwitchItem<T1>>()
            //    .FirstOrDefault(x => x.User == username && x.Group == groupname);

            if (match == null)
            {
                return false;
            }
            //List<T1> sqliteList = new List<T1>();
            //sqliteList = FindListTable<T1>(match.SmartSwitchList);

            //if (match.SmartSwitchList is not List<T1> sqliteList)
            //{
            //    return false;
            //}
            ourviewmodel.ssCount = sqliteList.Count;
            if (sqliteList.Count > 0)
            {
                if (!string.IsNullOrEmpty(username))
                {
                    if (!await DeleteAllSQLite(ourviewmodel,
                                              schema_name,
                                              table_name,
                                              username))
                    {
                        return false;
                    }
                }
                string sql_insert_con = GenerateInsertStore(ourviewmodel,
                                                            schema_name,
                                                            table_name);

                var sql_con_records = new StringBuilder();
                foreach (T1 orig_row in sqliteList)
                {
                    if (sql_con_records.Length > 0)
                        sql_con_records.Append(",");

                    sql_con_records.Append("(");
                    sql_con_records.Append(
                        DecodeToSQLUsername<T1>(orig_row, true, SmartParametersV2016.commachar));
                    sql_con_records.Append(")");
                }
                if (sql_con_records.Length > 0)
                {
                    if (!await ExecuteSQLite(
                            ourviewmodel,
                            sql_insert_con + sql_con_records + ";",
                            em => ourviewmodel.errorMessage = em))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static async Task<bool> DeleteAllSQLite(MainViewModel ourviewmodel,
                                                        string schema_name,
                                                        string table_name,
                                                        string username)
        {
            // When we Delete (!!) we delete everything
            // Why is this necessary when we've just created the table?
            string sql_delete = "DELETE FROM [" + schema_name + "." + table_name + "] WHERE USERNAME = '" +
                                username + "'";
            try
            {
                string sql = sql_delete + ";";
                int res2 = await SmartSwitchDatabase.ExecuteSmartSwitchTableAsync(ourviewmodel.sqliteDatabase, sql);
            }
            catch (Exception sqlex)
            {
                ourviewmodel.errorMessage = sqlex.Message;
                return false;
            }
            return true;
        }
        // I dont care about Loading Consumers into SQLite
        internal static async Task<bool> LoadSmartUsersX(MainViewModel ourviewmodel,
                                                        //short screenCode,
                                                        string schema_name,
                                                        string username,
                                                        string groupname)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                ourviewmodel.ssCount = 0;
                if (!await StoreSQLite<SmartUsers.ConsumersSQLite>(ourviewmodel,
                        schema_name,
                        "Consumers",
                        username,
                        groupname))
                {
                    return false;
                }
                else
                {
                    if (ourviewmodel.ssCount == 0)
                    {
                        // Start initialization sequence
                        return true;
                    }
                }
            }
            return true;
        }

        internal static async Task<bool> LoadSmartProfile(MainViewModel ourviewmodel,
                                                        short screenCode,
                                                        string schema_name,
                                                        string username,
                                                        string groupname)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                if (ourviewmodel.cubeCode[screenCode])
                {
                    ourviewmodel.ssCount = 0;
                    //Cubefaces
                    if (!await StoreSQLite<SmartProfile.CubefacesSQLite>(ourviewmodel,
                        schema_name,
                        "Cubefaces",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // AddressesView
                    if (!await StoreSQLite<SmartProfile.AddressesViewSQLite>(ourviewmodel,
                        schema_name,
                        "AddressesView",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // Now - we need to build any REMOTEs in to the LOCAL Finance Logins SQLite database?

                    // No - we're not building database tables based on REMOTE(s)
                    // anymore because this mixture isn't sustainable.
                    // We either use TOTALLY SmartDBServer based OR we use
                    // TOTALLY local 'external' storage which persists even 
                    // when the app is un-installed.  Hopefully Android 11 or
                    // Android 12 will let us do that - but if it DOESN'T then
                    // we'll just have to be patient (as ever!) and wait for it
                    // to catch up with us eventually...
                    //
                    // So Profiles Enc (local) is just Profiles (because it's ALWAYS
                    // encrypted) and ProfilesCubeEnc is just PRofiles Cube (because
                    // that's always encrypted as well ).

                    // Profiles
                    if (!await StoreSQLite<SmartProfile.ProfilesSQLite>(ourviewmodel,
                        schema_name,
                        "Profiles",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // Groups
                    if (!await StoreSQLite<SmartProfile.GroupsSQLite>(ourviewmodel,
                            schema_name,
                            "Groups",
                            username,
                        groupname))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        internal static async Task<bool> ExtractSmartUsersX(MainViewModel ourviewmodel,
                                                        List<SmartData.SQLiteTables> Fatahtables,
                                                        //short screenCode,
                                                        bool primary,
                                                        string schema_name,
                                                        string requester,
                                                        string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schema_name
                                select Tables);
                foreach (SmartData.SQLiteTables table in subsetTables)
                {
                    // All Smart Users tables
                    if (!await LoadCommonUsers(ourviewmodel,
                                            primary,
                                            schema_name,
                                            table.TABLE_NAME,
                                            requester,
                                            username,
                                            username,
                                            false)) // Not DBServer
                    {
                        return false;
                    }                        
                }
            }
            return true;
        }

        internal static async Task<bool> LoadCommonUsers(MainViewModel ourviewmodel,
                                            bool primary,
                                            string schema_name,
                                            string table_name,
                                            string requester,
                                            string username,
                                            string groupname,
                                            bool DBServer,
                                            bool oneoff = false)
        {
            string sql;
            string deriveds = schema_name + "." + table_name;
            switch (table_name)
            {
                case "Consumers":
                    if (primary)
                    {
                        ourviewmodel.Hamas.sqliteConsumersList = new List<SmartUsers.ConsumersSQLite>();
                        sql = Build_Select(schema_name, table_name, username);
                        try
                        {
                            ourviewmodel.Hamas.sqliteConsumersList = new List<SmartUsers.ConsumersSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUsers.ConsumersSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartUsers.ConsumersSQLite sqliteRow in ourviewmodel.Hamas.sqliteConsumersList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUsers.ConsumersSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                        else
                        {
                            if (oneoff)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        ourviewmodel.Hamas.sqliteConsumersList = Add_Secondary<SmartUsers.ConsumersSQLite>(ourviewmodel, username, groupname);
                    }
                    if (ourviewmodel.Hamas.sqliteConsumersList.Count > 0)
                    {
                        foreach (SmartUsers.ConsumersSQLite consumer_row in ourviewmodel.Hamas.sqliteConsumersList)
                        {
                            SmartUsers.Consumers consumer = new SmartUsers.Consumers()
                            {
                                USERNAME = consumer_row.USERNAME,
                                LOCAL_ONLY = consumer_row.LOCAL_ONLY == 0 ? false : true,
                                FULL_SCREEN = consumer_row.FULL_SCREEN == 0 ? false : true,
                                MULTIMETER_ACTIVE = consumer_row.MULTIMETER_ACTIVE == 0 ? false : true,
                                CONSUMER_CREATED = consumer_row.CONSUMER_CREATED,
                                SmartUsersConsumers = consumer_row.SmartUsersConsumers,
                                SmartProfileCubefaces = consumer_row.SmartProfileCubefaces,
                                SmartProfileGroups = consumer_row.SmartProfileGroups,
                                SmartProfileProfiles = consumer_row.SmartProfileProfiles,
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
                                SmartFinanceSwitches = consumer_row.SmartFinanceSwitches,
                                SmartFinanceTransactions = consumer_row.SmartFinanceTransactions,
                                SmartFinanceTransactionsCategories = consumer_row.SmartFinanceTransactionsCategories,
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
                                Updated = false
                            };
                            ourviewmodel.Hamas.consumersList.Add(consumer);
                        }
                    }
                    ourviewmodel.Hamas.sqliteConsumersList.Clear();
                    break;
                default:
                    break;
            }
            return true;
        }
        internal static async Task<bool> ExtractSmartProfile(MainViewModel ourviewmodel,
                                                        List<SmartData.SQLiteTables> Fatahtables,
                                                        //short screenCode,
                                                        bool primary,
                                                        string schema_name,
                                                        string username,
                                                        string DEK)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schema_name
                                select Tables);
                foreach (SmartData.SQLiteTables table in subsetTables)
                {
                    // All Smart Users tables
                    if (!await LoadCommonProfile(ourviewmodel,
                                        primary,
                                        schema_name,
                                        table.TABLE_NAME,
                                        username,
                                        username,
                                        username,
                                        DEK,
                                        false))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        
        internal static async Task<bool> LoadCommonProfile(MainViewModel ourviewmodel,
                                            bool primary,
                                            string schema_name,
                                            string table_name,
                                            string requester,
                                            string username,
                                            string groupname,
                                            string PDEK,
                                            bool DBServer)
        {
            string sql;
            string key_details_unencrypted;
            string details_unencrypted;
            string deriveds = schema_name + "." + table_name;
            int derived = deriveds.Length;
            switch (table_name)
            {
                case "Cubefaces":
                    if (primary)
                    {
                        ourviewmodel.SmartProfile.sqlite_cubefacesList = new List<SmartProfile.CubefacesSQLite>();
                        sql = Build_Select(schema_name, table_name, username);
                        try
                        {
                            ourviewmodel.SmartProfile.sqlite_cubefacesList = new List<SmartProfile.CubefacesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartProfile.CubefacesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartProfile.CubefacesSQLite sqliteRow in ourviewmodel.SmartProfile.sqlite_cubefacesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartProfile.CubefacesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        ourviewmodel.SmartProfile.sqlite_cubefacesList = Add_Secondary<SmartProfile.CubefacesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (ourviewmodel.SmartProfile.sqlite_cubefacesList.Count > 0)
                    {
                        foreach (SmartProfile.CubefacesSQLite cubeface_row in ourviewmodel.SmartProfile.sqlite_cubefacesList)
                        {
                            SmartProfile.Cubefaces cubeface = new SmartProfile.Cubefaces()
                            {
                                USERNAME = cubeface_row.USERNAME,
                                CUBEFACE_CODE = Convert.ToChar(cubeface_row.CUBEFACE_CODE),
                                FACE_ACTIVE = Convert.ToBoolean(cubeface_row.FACE_ACTIVE),
                                CUBEFACE_CREATED = cubeface_row.CUBEFACE_CREATED,
                                FACE_LAST_DISPLAY = cubeface_row.FACE_LAST_DISPLAY,
                                FACE_CULTURE_CODE = cubeface_row.FACE_CULTURE_CODE,
                                FACE_AUTOSWITCH = Convert.ToChar(cubeface_row.FACE_AUTOSWITCH),
                                FACE_CURRENCY = Convert.ToInt16(cubeface_row.FACE_CURRENCY),
                                NEXT_CONNECTION = cubeface_row.NEXT_CONNECTION,
                                Updated = false
                            };
                            ourviewmodel.SmartProfile.profilecubefacesList.Add(cubeface);
                        }
                    }
                    ourviewmodel.SmartProfile.sqlite_cubefacesList.Clear();
                    break;
                case "AddressesView":
                    if (primary)
                    {
                        ourviewmodel.SmartProfile.sqlite_addressesviewList = new List<SmartProfile.AddressesViewSQLite>();
                        sql = Build_Select(schema_name, table_name, username);
                        try
                        {
                            ourviewmodel.SmartProfile.sqlite_addressesviewList = new List<SmartProfile.AddressesViewSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartProfile.AddressesViewSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartProfile.AddressesViewSQLite sqliteRow in ourviewmodel.SmartProfile.sqlite_addressesviewList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartProfile.AddressesViewSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        ourviewmodel.SmartProfile.sqlite_addressesviewList = Add_Secondary<SmartProfile.AddressesViewSQLite>(ourviewmodel, username, groupname);
                    }
                    if (ourviewmodel.SmartProfile.sqlite_addressesviewList.Count > 0)
                    {
                        foreach (SmartProfile.AddressesViewSQLite addressview_row in ourviewmodel.SmartProfile.sqlite_addressesviewList)
                        {
                            key_details_unencrypted = (string.IsNullOrEmpty(PDEK) ? addressview_row.KEY_DETAILS : SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(PDEK, addressview_row.RANDOMKEY1, derived), addressview_row.KEY_DETAILS, false));
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            string[] key_details = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                            if (key_details.Length == 1)    // Just the UDPRN?
                            {
                                details_unencrypted = (string.IsNullOrEmpty(PDEK) ? addressview_row.DETAILS : SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(PDEK, addressview_row.RANDOMKEY2, derived), addressview_row.DETAILS, false));
                                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                                {
                                    return false;
                                }
                                string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (details_fields.Length == 15)
                                {
                                    SmartProfile.AddressesView addressview = new SmartProfile.AddressesView()
                                    {
                                        USERNAME = addressview_row.USERNAME,
                                        UDPRN = key_details[0],
                                        RANDOMKEY1 = addressview_row.RANDOMKEY1,
                                        ADDRESS_CREATED = addressview_row.ADDRESS_CREATED,
                                        AREA_CODE = Convert.ToInt16(details_fields[0]),
                                        BASIC = details_fields[1],
                                        BUILDING_NAME = details_fields[2],
                                        BUILDING_NUMBER = details_fields[3],
                                        COUNTY = details_fields[4],
                                        DOUBLE_DEPENDANT_LOCALITY = details_fields[5],
                                        DEPENDANT_THOROUGHFARE = details_fields[6],
                                        DEPENDANT_LOCALITY = details_fields[7],
                                        ORGANIZATION = details_fields[8],
                                        POSTCODE_OUTWARD = details_fields[9],
                                        POSTCODE = details_fields[10],
                                        POBOX = details_fields[11],
                                        SUB_BUILDING_NAME = details_fields[12],
                                        THOROUGHFARE = details_fields[13],
                                        TOWN = details_fields[14],

                                        RANDOMKEY2 = addressview_row.RANDOMKEY2,
                                        CHECKED = true,
                                        Updated = false
                                    };
                                    ourviewmodel.SmartProfile.addressesviewList.Add(addressview);
                                }
                            }
                        }
                    }
                    ourviewmodel.SmartProfile.sqlite_addressesviewList.Clear();
                    break;
                case "Profiles":
                    ourviewmodel.SmartProfile.sqlite_profilesList = new List<SmartProfile.ProfilesSQLite>();
                    if (primary)
                    {
                        sql = Build_Select(schema_name, table_name, username);
                        try
                        {
                            ourviewmodel.SmartProfile.sqlite_profilesList = new List<SmartProfile.ProfilesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartProfile.ProfilesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer && (requester == username))
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartProfile.ProfilesSQLite sqliteRow in ourviewmodel.SmartProfile.sqlite_profilesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartProfile.ProfilesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        ourviewmodel.SmartProfile.sqlite_profilesList = Add_Secondary<SmartProfile.ProfilesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (ourviewmodel.SmartProfile.sqlite_profilesList.Count > 0)
                    {
                        foreach (SmartProfile.ProfilesSQLite profiles_row in ourviewmodel.SmartProfile.sqlite_profilesList)
                        {
                            key_details_unencrypted = (string.IsNullOrEmpty(PDEK) ? profiles_row.KEY_DETAILS : SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(PDEK, profiles_row.RANDOMKEY1, derived), profiles_row.KEY_DETAILS, false));
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                            details_unencrypted = (string.IsNullOrEmpty(PDEK) ? profiles_row.DETAILS : SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(PDEK, profiles_row.RANDOMKEY2, derived), profiles_row.DETAILS, false));
                            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                            {
                                return false;
                            }
                            string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                            if (details_fields.Length == 5)
                            {
                                SmartProfile.Profiles profile = new SmartProfile.Profiles()
                                {
                                    USERNAME = profiles_row.USERNAME,
                                    PROFILE_CREATED = profiles_row.PROFILE_CREATED,
                                    LASTNAME = key_details_fields[0],
                                    FIRSTNAME = key_details_fields[1],
                                    MIDDLENAME = key_details_fields[2],
                                    DATE_OF_BIRTH = key_details_fields[3],
                                    RANDOMKEY1 = profiles_row.RANDOMKEY1,
                                    TITLE = details_fields[0],
                                    GENDER = details_fields[1],
                                    CONTACT_NO = details_fields[2],
                                    EMAIL_ADDRESS = details_fields[3],
                                    DISPLAYNAME = details_fields[4],
                                    RANDOMKEY2 = profiles_row.RANDOMKEY2
                                };
                                ourviewmodel.SmartProfile.profilesList.Add(profile);
                            }
                        }
                    }
                    ourviewmodel.SmartProfile.sqlite_profilesList.Clear();
                    break;                
                case "Groups":
                    if (primary)
                    {
                        ourviewmodel.SmartProfile.sqlite_groupsList = new List<SmartProfile.GroupsSQLite>();
                        sql = Build_Select(schema_name, table_name, username);
                        try
                        {
                            ourviewmodel.SmartProfile.sqlite_groupsList = new List<SmartProfile.GroupsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartProfile.GroupsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer && (requester == username))
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartProfile.GroupsSQLite sqliteRow in ourviewmodel.SmartProfile.sqlite_groupsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartProfile.GroupsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        ourviewmodel.SmartProfile.sqlite_groupsList = Add_Secondary<SmartProfile.GroupsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (ourviewmodel.SmartProfile.sqlite_groupsList.Count > 0)
                    {
                        foreach (SmartProfile.GroupsSQLite groups_row in ourviewmodel.SmartProfile.sqlite_groupsList)
                        {
                            SmartProfile.Groups group = new SmartProfile.Groups()
                            {
                                USERNAME = groups_row.USERNAME,
                                GROUPNAME = groups_row.GROUPNAME,
                                ACTIVEFLAG = Convert.ToBoolean(groups_row.ACTIVEFLAG),
                                MARKER = groups_row.MARKER,
                                PDEK = groups_row.PDEK,
                                SENDF = Convert.ToBoolean(groups_row.SENDF),
                                FDEK = groups_row.FDEK,
                                SENDU = Convert.ToBoolean(groups_row.SENDU),
                                UDEK = groups_row.UDEK,
                                RECEIVEALL = Convert.ToBoolean(groups_row.RECEIVEALL),
                                DISPLAYNAME = groups_row.DISPLAYNAME
                            };
                            ourviewmodel.SmartProfile.profilegroupsList.Add(group);
                        }
                    }
                    ourviewmodel.SmartProfile.sqlite_groupsList.Clear();
                    break;
                default:
                    break;
            }
            return true;
        }
        internal static bool RemoveSmartUsersX(MainViewModel ourviewmodel,
                                                List<SmartData.SQLiteTables> Fatahtables,
                                                //short screenCode,
                                                string schemaName,
                                                string username)
        {
            // Cannot Unload ourviewmodel.UserName btw!!
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schemaName
                                select Tables);
                // Now for DEV OR PROD read the encrypted local table
                // and build our unencrypted working copy
                // Now build the unencrypted internal table from the LOCAL records (if there are any)

                foreach (SmartData.SQLiteTables table in subsetTables)
                {
                    // All tables
                    //if (ourviewmodel.loadRemote[screenCode])
                    //{
                        if (!UnloadCommonUsersX(ourviewmodel,
                                            table.TABLE_NAME,
                                            username))
                        {
                            return false;
                        }
                    //}
                }
            }
            return true;
        }

        // Why should we ever remove the SmartProfile because
        // we never load it?  But we do - the AddressesView!!
        // THIS IS ONLY USED IN SMARTDASHBOARD
        internal static bool RemoveSmartProfile(MainViewModel ourviewmodel,
                                                List<SmartData.SQLiteTables> Fatahtables,
                                                short screenCode,
                                                string schemaName,
                                                string username)
        {
            // Cannot Unload ourviewmodel.UserName btw!!
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                //int ss_count = 1;
                List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schemaName
                                select Tables);
                // Now for DEV OR PROD read the encrypted local table
                // and build our unencrypted working copy
                // Now build the unencrypted internal table from the LOCAL records (if there are any)

                foreach (SmartData.SQLiteTables table in subsetTables)
                {
                    // All tables
                    //if ((ourviewmodel.loadRemote[screenCode] && ss_count > 0) ||
                    //    !ourviewmodel.loadRemote[screenCode])
                    if (ourviewmodel.cubeCode[screenCode])
                    {
                        if (!UnloadCommonProfile(ourviewmodel,
                                            table.TABLE_NAME,
                                            username))
                        {
                            return false;
                        }
                    }
                }
                

                //// Now for DEV OR PROD read the encrypted local table
                //// and build our unencrypted working copy
                //// Now build the unencrypted internal table from the LOCAL records (if there are any)
                //if ((ourviewmodel.loadRemote && ss_count > 0) ||
                //    !ourviewmodel.loadRemote)
                //{
                //    if (!UnloadCommonUsers(ourviewmodel,
                //                    "Cubefaces",
                //                    username))
                //    {
                //        return false;
                //    }
                //}

                //// Now for DEV OR PROD read the encrypted local table
                //// and build our unencrypted working copy
                //// Now build the unencrypted internal table from the LOCAL records (if there are any)
                //if ((ourviewmodel.loadRemote && ss_count > 0) ||
                //    !ourviewmodel.loadRemote)
                //{
                //    if (!UnloadCommonUsers(ourviewmodel,
                //                "AddressesView",
                //                username))
                //    {
                //        return false;
                //    }
                //}

                //// Now for DEV OR PROD read the encrypted local table
                //// and build our unencrypted working copy
                //// Now build the unencrypted internal table from the LOCAL records (if there are any)
                //if ((ourviewmodel.loadRemote && ss_count > 0) ||
                //    !ourviewmodel.loadRemote)
                //{
                //    if (!UnloadCommonUsers(ourviewmodel,
                //                "Profiles",
                //                username))
                //    {
                //        return false;
                //    }
                //}


                //// Now for DEV OR PROD read the encrypted local table
                //// and build our unencrypted working copy
                //// Now build the unencrypted internal table from the LOCAL records (if there are any)
                //if ((ourviewmodel.loadRemote && ss_count > 0) ||
                //    !ourviewmodel.loadRemote)
                //{
                //    if (!UnloadCommonUsers(ourviewmodel,
                //                "ProfilesCube",
                //                username))
                //    {
                //        return false;
                //    }
                //}

                //// Now for DEV OR PROD read the encrypted local table
                //// and build our unencrypted working copy
                //// Now build the unencrypted internal table from the LOCAL records (if there are any)
                //if ((ourviewmodel.loadRemote && ss_count > 0) ||
                //    !ourviewmodel.loadRemote)
                //{
                //    if (!UnloadCommonUsers(ourviewmodel,
                //                 "Groups",
                //                username))
                //    {
                //        return false;
                //    }
                //}
            }
            return true;
        }
        internal static string Build_Select(string schema_name,
                                            string table_name,
                                            string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return "SELECT * FROM [" + schema_name + "." + table_name + "];";
            }
            return "SELECT * FROM [" 
                + schema_name + "." + table_name +
                "] WHERE USERNAME = '" + username + "';";
        }

        internal static List<T1> Add_Secondary<T1>(MainViewModel ourviewmodel, string username, string groupname)
        {
            //List<T1> sqliteList = new List<T1>();
            //sqliteList = FindListTable<T1>(ourviewmodel.smartswitchList);
            //List<T1> sqliteList = new List<T1>();
            //foreach (SmartUsers.SmartView sview in smartswitchListX)//ourviewmodel.ssList)
            //{
            //    if (sview.USERNAME == username)
            //    {
            //        sqliteList = FindListTable<T1>(sview.BOLLOCKS);
            //    }
            //}
            // Build the internal table from the downloaded stuff
            SmartSwitchItem match = ourviewmodel.myList
                .OfType<SmartSwitchItem>()
                .FirstOrDefault(x => x.User == username && x.Group == groupname);

            if (match == null)
            {
                return null;
            }
            if (match.SmartSwitchList is not List<T1> sqliteList)
            {
                return null;
            }
            return sqliteList;
        }

        internal static bool UnloadCommonUsersX(MainViewModel ourviewmodel,
                                            string table_name,
                                            string username)
        {
            switch (table_name)
            {
                case "Consumers":
                    // Cannot unload Primary!
                    if (ourviewmodel.Hamas.consumersList.Count > 0)
                    {
                        List<SmartUsers.Consumers> temp = new List<SmartUsers.Consumers>();
                        foreach (SmartUsers.Consumers consumer_row in ourviewmodel.Hamas.consumersList)
                        {
                            if (consumer_row.USERNAME != username)
                            {
                                temp.Add(consumer_row);
                            }
                        }
                        ourviewmodel.Hamas.consumersList = temp;
                    }
                    break;
                
                default:
                    break;
            }
            return true;
        }

        internal static bool UnloadCommonProfile(MainViewModel ourviewmodel,
                                            string table_name,
                                            string username)
        {
            switch (table_name)
            {       
                // This is the only one were are interested in for a non-User Profile
                case "AddressesView":
                    // Cannot unload Primary!
                    if (ourviewmodel.SmartProfile.addressesviewList.Count > 0)
                    {
                        List<SmartProfile.AddressesView> temp = new List<SmartProfile.AddressesView>();
                        foreach (SmartProfile.AddressesView address_row in ourviewmodel.SmartProfile.addressesviewList)
                        {
                            if (address_row.USERNAME != username)
                            {
                                temp.Add(address_row);
                            }
                        }
                        ourviewmodel.SmartProfile.addressesviewList = temp;
                    }
                    break;
                //case "Profiles":
                //    // Cannot unload Primary!
                //    if (ourviewmodel.SmartProfile.profilesList.Count > 0)
                //    {
                //        List<SmartProfile.Profiles> temp = new List<SmartProfile.Profiles>();
                //        foreach (SmartProfile.Profiles profile_row in ourviewmodel.SmartProfile.profilesList)
                //        {
                //            if (profile_row.USERNAME != username)
                //            {
                //                temp.Add(profile_row);
                //            }
                //        }
                //        ourviewmodel.SmartProfile.profilesList = temp;
                //    }
                //    break;
                //case "Groups":
                //    // Cannot unload Primary!
                //    if (ourviewmodel.SmartProfile.profilegroupsList.Count > 0)
                //    {
                //        List<SmartProfile.Groups> temp = new List<SmartProfile.Groups>();
                //        foreach (SmartProfile.Groups group_row in ourviewmodel.SmartProfile.profilegroupsList)
                //        {
                //            if (group_row.USERNAME != username)
                //            {
                //                temp.Add(group_row);
                //            }
                //        }
                //        ourviewmodel.SmartProfile.profilegroupsList = temp;
                //    }
                //    break;
                default:
                    break;
            }
            return true;
        }

        internal static async Task<bool> LoadSmartFinance(MainViewModel ourviewmodel,
                                                            short screenCode,
                                                            string schemaName,
                                                            string username,
                                                            string groupname)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                if (ourviewmodel.cubeCode[screenCode])
                {
                    // Accounts
                    if (!await StoreSQLite<SmartFinance.AccountsSQLite>(ourviewmodel,
                        schemaName,
                        "Accounts",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // Categories
                    if (!await StoreSQLite<SmartFinance.CategoriesSQLite>(ourviewmodel,
                                            schemaName,
                                            "Categories",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // CategoryTypes
                    if (!await StoreSQLite<SmartFinance.CategoryTypesSQLite>(ourviewmodel,
                                            schemaName,
                                            "CategoryTypes",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // CryptoAccounts
                    if (!await StoreSQLite<SmartFinance.CryptoAccountsSQLite>(ourviewmodel,
                                            schemaName,
                                            "CryptoAccounts",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // CryptoAddresses
                    if (!await StoreSQLite<SmartFinance.CryptoAddressesSQLite>(ourviewmodel,
                                            schemaName,
                                            "CryptoAddresses",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                
                    // CryptoCurrencyRates - why would we ever want to load these from someone else?
                    // Because a family might want to see what others have got!
                    // Because they may have rates we haven't got
                    if (!await StoreSQLite<SmartFinance.CryptoCurrencyRatesSQLite>(ourviewmodel,
                                            schemaName,
                                            "CryptoCurrencyRates",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // CryptoLedgers
                    if (!await StoreSQLite<SmartFinance.CryptoLedgersSQLite>(ourviewmodel,
                                            schemaName,
                                            "CryptoLedgers",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // CryptoWallets
                    if (!await StoreSQLite<SmartFinance.CryptoWalletsSQLite>(ourviewmodel,
                                            schemaName,
                                            "CryptoWallets",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // CryptoWalletTotals
                    if (!await StoreSQLite<SmartFinance.CryptoWalletTotalsSQLite>(ourviewmodel,
                                            schemaName,
                                            "CryptoWalletTotals",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // Connections
                    if (!await StoreSQLite<SmartFinance.ConnectionsSQLite>(ourviewmodel,
                        schemaName,
                        "Connections",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // Logins
                    if (!await StoreSQLite<SmartFinance.LoginsSQLite>(ourviewmodel,
                        schemaName,
                        "Logins",
                        username,
                        groupname))
                    {
                        return false;
                    }
                

                // Now - we need to build any REMOTEs in to the LOCAL Finance Logins SQLite database?

                // No - we're not building database tables based on REMOTE(s)
                // anymore because this mixture isn't sustainable.
                // We either use TOTALLY SmartDBServer based OR we use
                // TOTALLY local 'external' storage which persists even 
                // when the app is un-installed.  Hopefully Android 11 or
                // Android 12 will let us do that - but if it DOESN'T then
                // we'll just have to be patient (as ever!) and wait for it
                // to catch up with us eventually...
                //
                // So Profiles Enc (local) is just Profiles (because it's ALWAYS
                // encrypted) and ProfilesCubeEnc is just PRofiles Cube (because
                // that's always encrypted as well ).

                // Switches
                // For reasons I don't quite understand, but which don't
                // surprise me as this entire development 'scenario'
                // is a piece of SHIT, the Switches SQLite doesn't
                // work for Android.  Have no idea why, but it seems
                // to say there is no RANDOMKEY field in the
                // SQLite.Finance.Switches table.  What bollocks.
                    if (!await StoreSQLite<SmartFinance.SwitchesSQLite>(ourviewmodel,
                                                        schemaName,
                                                        "Switches",
                                                        username,
                                                        groupname))
                    {
                        return false;
                    }
                    // Transactions
                    if (!await StoreSQLite<SmartFinance.TransactionsSQLite>(ourviewmodel,
                                            schemaName,
                                            "Transactions",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                    // TransactionsCategories
                    if (!await StoreSQLite<SmartFinance.TransactionsCategoriesSQLite>(ourviewmodel,
                                            schemaName,
                                            "TransactionsCategories",
                                            username,
                                            groupname))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static async Task<bool> ExtractSmartFinance(MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        List<SmartData.SQLiteTables> Fatahtables,
                                                        short screenCode,
                                                        bool primary,
                                                        string schemaName,
                                                        string requester,
                                                        string username,
                                                        string FDEK)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schemaName
                                select Tables);
                foreach (SmartData.SQLiteTables table in subsetTables)
                {
                    // All tables
                    if (ourviewmodel.cubeCode[screenCode])
                    {
                        if (!await LoadCommonFinance(ourviewmodel,
                                            financeviewmodel,
                                            primary,
                                            schemaName,
                                            table.TABLE_NAME,
                                            requester,
                                            username,
                                            username,
                                            FDEK,
                                            false))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal static async Task<bool> LoadCommonFinance(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            bool primary,
                                            string schemaName,
                                            string table_name,
                                            string requester,
                                            string username,
                                            string groupname,
                                            string FDEK,
                                            bool DBServer)
        {
            string sql = "";
            string key_details_unencrypted;
            string key_details;
            string deriveds = schemaName + "." + table_name;
            int derived = deriveds.Length;

            switch (table_name)
            {
                case "Accounts":
                    // Load Common Finance
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_finance_accountsList = new List<SmartFinance.AccountsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.AccountsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.AccountsSQLite sqliteRow in financeviewmodel.PLO.sqlite_finance_accountsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.AccountsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_finance_accountsList = Add_Secondary<SmartFinance.AccountsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_finance_accountsList.Count > 0)
                    {
                        foreach (SmartFinance.AccountsSQLite accounts_row in financeviewmodel.PLO.sqlite_finance_accountsList)
                        {
                            key_details = accounts_row.KEY_DETAILS;
                            if (string.IsNullOrEmpty(FDEK))
                            {
                                key_details_unencrypted = key_details;
                            }
                            else
                            {
                                key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, accounts_row.RANDOMKEY1, derived), key_details, false);
                                if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                            string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                            if (key_details_fields.Length == 3)
                            {
                                string details_unencrypted;
                                string details;
                                details = accounts_row.DETAILS;
                                if (string.IsNullOrEmpty(FDEK))
                                {
                                    details_unencrypted = details;
                                }
                                else
                                {
                                    details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, accounts_row.RANDOMKEY2, derived), details, false);
                                    if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (details_fields.Length == 4)
                                {
                                    SmartFinance.Accounts account = new SmartFinance.Accounts()
                                    {
                                        USERNAME = accounts_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(accounts_row.CUBEFACE_CODE),
                                        INSTITUTION_CODE = accounts_row.INSTITUTION_CODE,
                                        BRAND_CODE = accounts_row.BRAND_CODE,
                                        SORTCODE = key_details_fields[0],
                                        ACCOUNT_NO = key_details_fields[1],
                                        UDPRN = key_details_fields[2],
                                        RANDOMKEY1 = accounts_row.RANDOMKEY1,
                                        ACCOUNT_CREATED = accounts_row.ACCOUNT_CREATED,
                                        CATEGORY_CODE = Convert.ToChar(accounts_row.CATEGORY_CODE),
                                        CURRENCY_ORDINAL = Convert.ToInt16(details_fields[0]),
                                        ACCOUNT_TITLE = details_fields[1],
                                        ACCOUNT_BALANCE = Convert.ToInt32(details_fields[2]),
                                        //OTHER
                                        STATUS = Convert.ToChar(details_fields[3]),
                                        //ACTIVE = details_fields[3],
                                        //TYPE = details_fields[4],
                                        //UUID = details_fields[5],
                                        //AVAILABLE_BALANCE_VALUE = details_fields[6],
                                        //AVAILABLE_BALANCE_CURRENCY = details_fields[7],
                                        //DEFAULT =  details_fields[8],
                                        //DELETED_AT = Convert.ToDateTime(details_fields[9]),
                                        //HOLD_VALUE = details_fields[10],
                                        //HOLD_CURRENCY = details_fields[11],
                                        //NAME = details_fields[12],
                                        //PLATFORM = details_fields[13],
                                        //READY = details_fields[14],
                                        //RETAIL_PORTFOLIO_ID = details_fields[15],
                                        //UPDATED_AT = Convert.ToDateTime(details_fields[16]),
                                        RANDOMKEY2 = accounts_row.RANDOMKEY2,
                                        Updated = false
                                    };
                                        financeviewmodel.PLO.finance_accountsList.Add(account);
                                    
                                }
                            }
                        }
                    }
                    financeviewmodel.PLO.sqlite_finance_accountsList.Clear();
                    break;                
                case "Categories":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_finance_categoriesList = new List<SmartFinance.CategoriesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CategoriesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CategoriesSQLite sqliteRow in financeviewmodel.PLO.sqlite_finance_categoriesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CategoriesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_finance_categoriesList = Add_Secondary<SmartFinance.CategoriesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_finance_categoriesList.Count > 0)
                    {
                        foreach (SmartFinance.CategoriesSQLite sqlite_row in financeviewmodel.PLO.sqlite_finance_categoriesList)
                        {
                                SmartFinance.Categories category = new SmartFinance.Categories()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    CATEGORY_CODE = Convert.ToChar(sqlite_row.CATEGORY_CODE),
                                    CHECKED = sqlite_row.CHECKED,
                                    Updated = false
                                };
                                financeviewmodel.PLO.finance_categoriesList.Add(category);
                           
                        }
                    }
                    financeviewmodel.PLO.sqlite_finance_categoriesList.Clear();
                    break;
                case "CategoryTypes":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_finance_categorytypesList = new List<SmartFinance.CategoryTypesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CategoryTypesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CategoryTypesSQLite sqliteRow in financeviewmodel.PLO.sqlite_finance_categorytypesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CategoryTypesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_finance_categorytypesList = Add_Secondary<SmartFinance.CategoryTypesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_finance_categorytypesList.Count > 0)
                    {
                        foreach (SmartFinance.CategoryTypesSQLite sqlite_row in financeviewmodel.PLO.sqlite_finance_categorytypesList)
                        {
                            SmartFinance.CategoryTypes categorytype = new SmartFinance.CategoryTypes()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    CATEGORY_CODE = Convert.ToChar(sqlite_row.CATEGORY_CODE),
                                    CURRENCY_ORDINAL = sqlite_row.CURRENCY_ORDINAL,
                                    Updated = false
                                };
                                financeviewmodel.PLO.finance_categorytypesList.Add(categorytype);
                               
                        }
                    }
                    financeviewmodel.PLO.sqlite_finance_categorytypesList.Clear();
                    break;
                case "Connections":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_finance_connectionsList = new List<SmartFinance.ConnectionsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.ConnectionsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer && (username == requester))
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.ConnectionsSQLite sqliteRow in financeviewmodel.PLO.sqlite_finance_connectionsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.ConnectionsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_finance_connectionsList = Add_Secondary<SmartFinance.ConnectionsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_finance_connectionsList.Count > 0)
                    {
                        foreach (SmartFinance.ConnectionsSQLite connections_row in financeviewmodel.PLO.sqlite_finance_connectionsList)
                        {
                            key_details = connections_row.DETAILS;
                            if (string.IsNullOrEmpty(FDEK))
                            {
                                key_details_unencrypted = key_details;
                            }
                            else
                            {
                                key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, connections_row.RANDOMKEY, derived), key_details, false);
                                if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                            string[] details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                            if (details_fields.Length == 3)
                            {
                                SmartFinance.Connections connection = new SmartFinance.Connections()
                                    {
                                        USERNAME = connections_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(connections_row.CUBEFACE_CODE),
                                        INSTITUTION_CODE = connections_row.INSTITUTION_CODE,
                                        BRAND_CODE = connections_row.BRAND_CODE,
                                        LOGIN_METHOD = connections_row.LOGIN_METHOD,
                                        ACTIVE_FLAG = Convert.ToChar(connections_row.ACTIVE_FLAG),
                                        DETAILS = connections_row.DETAILS,
                                        RANDOMKEY = connections_row.RANDOMKEY,
                                        Parameter1 = details_fields[0], // E-Mail Address   / Customer No
                                        Parameter2 = details_fields[1], // APIKey Public    / DoB
                                        Parameter3 = details_fields[2], // APIKey SECRET    / Passcode

#if WPF  || WINUI || SMARTMAUI
                                        Parameter1Visibility = Visibility.Collapsed,
                                        Parameter2Visibility = Visibility.Collapsed,
                                        Parameter3Visibility = Visibility.Collapsed,
#endif
#if ANDROIDX
                                        Parameter1Visibility = ViewStates.Invisible,
                                        Parameter2Visibility = ViewStates.Invisible,
                                        Parameter3Visibility = ViewStates.Invisible,
#endif
                                        Updated = false,
                                        Delete = false
                                    };
                                    financeviewmodel.PLO.finance_connectionsList.Add(connection);
                                
                            }
                        }
                    }
                    financeviewmodel.PLO.sqlite_finance_connectionsList.Clear();
                    break;
                case "CryptoAccounts":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_crypto_accountsList = new List<SmartFinance.CryptoAccountsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CryptoAccountsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CryptoAccountsSQLite sqliteRow in financeviewmodel.PLO.sqlite_crypto_accountsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CryptoAccountsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_crypto_accountsList = Add_Secondary<SmartFinance.CryptoAccountsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_crypto_accountsList.Count > 0)
                    {
                        foreach (SmartFinance.CryptoAccountsSQLite sqlite_row in financeviewmodel.PLO.sqlite_crypto_accountsList)
                        {
                            string[] details = sqlite_row.DETAILS.Split(SmartParametersV2016.dc2);  // Special for compressed fields
                                //SmartFinance.CryptoAccounts generic = new SmartFinance.CryptoAccounts()
                                //{
                                //    USERNAME = sqlite_row.USERNAME,
                                //    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                //    INSTITUTION_CODE = Convert.ToInt16(sqlite_row.INSTITUTION_CODE),
                                //    BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                                //    SORTCODE = sqlite_row.SORTCODE,
                                //    ACCOUNT_NO = sqlite_row.ACCOUNT_NO,
                                //    UDPRN = sqlite_row.UDPRN,
                                //    CREATED_AT = Convert.ToDateTime(sqlite_row.CREATED_AT),
                                //    //ACTIVE = details[0].ToString(),
                                //    TYPE = details[0],
                                //    AVAILABLE_BALANCE_VALUE = details[1],
                                //    AVAILABLE_BALANCE_CURRENCY = details[2],
                                //    DEFAULT = details[3],
                                //    DELETED_AT = Convert.ToDateTime(details[4]),
                                //    HOLD_VALUE = details[5],
                                //    HOLD_CURRENCY = details[6],
                                //    NAME = details[7],
                                //    //PLATFORM = details[9],
                                //    READY = details[8],
                                //    RETAIL_PORTFOLIO_ID = details[9],
                                //    UPDATED_AT = Convert.ToDateTime(details[10]),
                                //    Updated = false
                                //};
                                //financeviewmodel.PLO.crypto_accountsList.Add(generic);
                            
                            //financeviewmodel.PLO.crypto_accountsList.Add(generic);
                        }
                    }
                    financeviewmodel.PLO.sqlite_crypto_accountsList.Clear();
                    break;
                case "CryptoAddresses":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_crypto_addressesList = new List<SmartFinance.CryptoAddressesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CryptoAddressesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CryptoAddressesSQLite sqliteRow in financeviewmodel.PLO.sqlite_crypto_addressesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CryptoAddressesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_crypto_addressesList = Add_Secondary<SmartFinance.CryptoAddressesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_crypto_addressesList.Count > 0)
                    {
                        foreach (SmartFinance.CryptoAddressesSQLite sqlite_row in financeviewmodel.PLO.sqlite_crypto_addressesList)
                        {
                            string[] details = sqlite_row.DETAILS.Split(SmartParametersV2016.dc2);  // Special for compressed fields

                            SmartFinance.CryptoAddresses generic = new SmartFinance.CryptoAddresses()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    INSTITUTION_CODE = Convert.ToInt16(sqlite_row.INSTITUTION_CODE),
                                    BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                                    SORTCODE = sqlite_row.SORTCODE,
                                    ACCOUNT_NO = sqlite_row.ACCOUNT_NO,
                                    UDPRN = sqlite_row.UDPRN,
                                    ADDRESS = details[0],
                                    ADDRESS_INFO_ADDRESS = details[1],
                                    ADDRESS_LABEL = details[2],
                                    CALLBACK_URL = details[3],
                                    CREATED_AT = Convert.ToDateTime(details[4]),
                                    DEFAULT_RECEIVE = details[5],
                                    DEPOSIT_URI = details[6],
                                    DESTINATION_TAG = details[7],
                                    ID = details[8],
                                    INLINE_WARNING_TEXT = details[9],
                                    INLINE_WARNING_TOOLTIP = details[10],
                                    NAME = details[11],
                                    NETWORK = details[12],
                                    QR_CODE_IMAGE_URL = details[13],
                                    RECEIVE_SUBTITLE = details[14],
                                    RESOURCE = details[15],
                                    RESOURCE_PATH = details[16],
                                    SHARE_ADDRESS_COPY_LINE1 = details[17],
                                    SHARE_ADDRESS_COPY_LINE2 = details[18],
                                    UPDATED_AT = Convert.ToDateTime(details[19]),
                                    URI_SCHEME = details[20],
                                    Updated = false
                                };
                                financeviewmodel.PLO.crypto_addressesList.Add(generic);
                            
                        }
                    }
                    financeviewmodel.PLO.sqlite_crypto_addressesList.Clear();
                    break;
                case "CryptoCurrencyRates":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_crypto_currenciesList = new List<SmartFinance.CryptoCurrencyRatesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CryptoCurrencyRatesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CryptoCurrencyRatesSQLite sqliteRow in financeviewmodel.PLO.sqlite_crypto_currenciesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CryptoCurrencyRatesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        // Because they may have Currencies we don't
                        financeviewmodel.PLO.sqlite_crypto_currenciesList = Add_Secondary<SmartFinance.CryptoCurrencyRatesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_crypto_currenciesList.Count > 0)
                    {
                        foreach (SmartFinance.CryptoCurrencyRatesSQLite sqlite_row in financeviewmodel.PLO.sqlite_crypto_currenciesList)
                        {
                            //string[] details = sqlite_row.DETAILS.Split(SmartParametersV2016.dc2);  // Special for compressed fields

                            SmartFinance.CryptoCurrencyRates generic = new SmartFinance.CryptoCurrencyRates()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    CRYPTO_ORDINAL = Convert.ToInt16(sqlite_row.CRYPTO_ORDINAL),
                                    CRYPTO_RATE = Convert.ToDouble(sqlite_row.CRYPTO_RATE),
                                    Updated = false
                                };
                                financeviewmodel.PLO.crypto_currenciesList.Add(generic);
                            
                        }
                    }
                    financeviewmodel.PLO.sqlite_crypto_currenciesList.Clear();
                    break;

                case "CryptoLedgers":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_crypto_ledgersList = new List<SmartFinance.CryptoLedgersSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CryptoLedgersSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CryptoLedgersSQLite sqliteRow in financeviewmodel.PLO.sqlite_crypto_ledgersList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CryptoLedgersSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        // Because they may have Ledgers we don't
                        financeviewmodel.PLO.sqlite_crypto_ledgersList = Add_Secondary<SmartFinance.CryptoLedgersSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_crypto_ledgersList.Count > 0)
                    {
                        foreach (SmartFinance.CryptoLedgersSQLite sqlite_row in financeviewmodel.PLO.sqlite_crypto_ledgersList)
                        {
                            //string[] details = sqlite_row.DETAILS.Split(SmartParametersV2016.dc2);  // Special for compressed fields

                            SmartFinance.CryptoLedgers generic = new SmartFinance.CryptoLedgers()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    INSTITUTION_CODE = Convert.ToInt16(sqlite_row.INSTITUTION_CODE),
                                    BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                                    LEDGER_NAME = sqlite_row.LEDGER_NAME,
                                    WALLET = sqlite_row.WALLET,
                                    ACCOUNT = sqlite_row.ACCOUNT,
                                    TRANSACTION_DATE = Convert.ToDateTime(sqlite_row.TRANSACTION_DATE),
                                    AMOUNT = Convert.ToDouble(sqlite_row.AMOUNT),
                                    SOURCE = sqlite_row.SOURCE,
                                    DESTINATION = sqlite_row.DESTINATION,
                                    CURRENCY_DISPLAY = sqlite_row.CURRENCY_DISPLAY,
                                    FEE = sqlite_row.FEE,
                                    TRANSACTION_TYPE = sqlite_row.TRANSACTION_TYPE,
                                    TO_ADDRESS = sqlite_row.TO_ADDRESS,
                                    NETWORK_NAME = sqlite_row.NETWORK_NAME,
                                    HASH = sqlite_row.HASH,
                                    Updated = false
                                };
                                financeviewmodel.PLO.crypto_ledgersList.Add(generic);
                            
                        }
                    }
                    financeviewmodel.PLO.sqlite_crypto_ledgersList.Clear();
                    break;

                case "CryptoTransactions":
                //    if (primary)
                //    {
                //        sql = Build_Select(schemaName, table_name, username);
                //        try
                //        {
                //            financeviewmodel.PLO.sqlite_crypto_transactionsList = new List<SmartFinance.CryptoTransactionsViewSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CryptoTransactionsViewSQLite>(sql));
                //        }
                //        catch (Exception sqlex)
                //        {
                //            ourviewmodel.errorMessage = sqlex.Message;
                //            return false;
                //        }
                //      if (DBServer)
                //      {
                //          ourviewmodel.recordCount = 0;
                //          foreach (SmartFinance.CryptoTransactionsSQLite sqliteRow in financeviewmodel.PLO.sqlite_crypto_transactionsList)
                //          {
                //              if (ourviewmodel.recordCount > 0)
                //              {
                //                  ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                //              }
                //              ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQL<SmartFinance.CryptoTransactionsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                //              ourviewmodel.recordCount++;
                //          }
                //          return true;
                //      }
                //    }
                //    else
                //    {
                //        financeviewmodel.PLO.sqlite_crypto_transactionsList = Add_Secondary<SmartFinance.CryptoTransactionsViewSQLite>(ourviewmodel, username, groupname);
                //    }
                //    if (financeviewmodel.PLO.sqlite_crypto_transactionsList.Count > 0)
                //    {
                //        foreach (SmartFinance.CryptoTransactionsViewSQLite sqlite_row in financeviewmodel.PLO.sqlite_crypto_transactionsList)
                //        {
                //            key_details = sqlite_row.KEY_DETAILS;
                //            if (string.IsNullOrEmpty(FDEK))
                //            {
                //                key_details_unencrypted = key_details;
                //            }
                //            else
                //            {
                //                key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                //                if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                //                {
                //                    return false;
                //                }
                //            }
                //            string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                //            if (key_details_fields.Length == 4)
                //            {
                //                string details_unencrypted;
                //                string details;

                //                details = sqlite_row.DETAILS;
                //                if (string.IsNullOrEmpty(FDEK))
                //                {
                //                    details_unencrypted = details;
                //                }
                //                else
                //                {
                //                    details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, sqlite_row.RANDOMKEY2, derived), details, false);
                //                    if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                //                    {
                //                        return false;
                //                    }
                //                }
                //                string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                //                if (details_fields.Length == 13)
                //                {
                //                    try
                //                    {
                //                        SmartFinance.CryptoTransactionsView generic = new SmartFinance.CryptoTransactionsView()
                //                        {
                //                            USERNAME = sqlite_row.USERNAME,
                //                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                //                            INSTITUTION_CODE = Convert.ToInt16(sqlite_row.INSTITUTION_CODE),
                //                            BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                //                            EXCHANGE = key_details_fields[0],
                //                            UUID = key_details_fields[1],
                //                            UDPRN = key_details_fields[2],
                //                            CREATED_AT = Convert.ToDateTime(key_details_fields[3]),
                //                            SEQUENCE_NO = sqlite_row.SEQUENCE_NO,
                //                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                //                            TYPE = details_fields[0],
                //                            CRYPTO_CURRENCY = details_fields[1],
                //                            CRYPTO_CURRENCY_ORDINAL = Convert.ToInt16(details_fields[2]),
                //                            CRYPTO_AMOUNT = Convert.ToDouble(details_fields[3]),
                //                            CRYPTO_RATE = Convert.ToDouble(details_fields[4]),
                //                            CURRENCY_DISPLAY = details_fields[5],
                //                            CREDITDEBIT_INDICATOR = Convert.ToInt16(details_fields[6]),
                //                            NATIVE_AMOUNT = Convert.ToDouble(details_fields[7]),
                //                            NATIVE_CURRENCY = details_fields[8],
                //                            FEE_AMOUNT = Convert.ToDouble(details_fields[9]),
                //                            FEE_CURRENCY = details_fields[10],
                //                            TOTAL_COST = details_fields[11],
                //                            TO_ADDRESS = details_fields[12],
                //                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                //                            Updated = false
                //                        };
                //                        financeviewmodel.PLO.crypto_transactionsList.Add(generic);
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        Console.WriteLine(ex.Message);
                //                    }


                //                }
                //            }
                //        }
                //    }
                //    financeviewmodel.PLO.sqlite_crypto_transactionsList.Clear();
                    break;

                case "CryptoWallets":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_crypto_walletsList = new List<SmartFinance.CryptoWalletsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CryptoWalletsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CryptoWalletsSQLite sqliteRow in financeviewmodel.PLO.sqlite_crypto_walletsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CryptoWalletsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        // Because they may have Wallets we don't
                        financeviewmodel.PLO.sqlite_crypto_walletsList = Add_Secondary<SmartFinance.CryptoWalletsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_crypto_walletsList.Count > 0)
                    {
                        foreach (SmartFinance.CryptoWalletsSQLite sqlite_row in financeviewmodel.PLO.sqlite_crypto_walletsList)
                        {
                            //string[] details = sqlite_row.DETAILS.Split(SmartParametersV2016.dc2);  // Special for compressed fields

                            SmartFinance.CryptoWallets generic = new SmartFinance.CryptoWallets()
                            {
                                USERNAME = sqlite_row.USERNAME,
                                CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                CRYPTO_DESTINATION = sqlite_row.CRYPTO_DESTINATION,
                                CRYPTO_NAME = sqlite_row.CRYPTO_NAME,
                                BALANCE = Convert.ToDouble(sqlite_row.BALANCE),
                                Updated = false
                            };
                            financeviewmodel.PLO.crypto_walletsList.Add(generic);
                        }
                    }
                    financeviewmodel.PLO.sqlite_crypto_walletsList.Clear();
                    break;

                case "CryptoWalletTotals":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_crypto_wallettotalsList = new List<SmartFinance.CryptoWalletTotalsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.CryptoWalletTotalsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.CryptoWalletTotalsSQLite sqliteRow in financeviewmodel.PLO.sqlite_crypto_wallettotalsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.CryptoWalletTotalsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        // Because they may have Wallet Totals we don't
                        financeviewmodel.PLO.sqlite_crypto_wallettotalsList = Add_Secondary<SmartFinance.CryptoWalletTotalsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_crypto_wallettotalsList.Count > 0)
                    {
                        foreach (SmartFinance.CryptoWalletTotalsSQLite sqlite_row in financeviewmodel.PLO.sqlite_crypto_wallettotalsList)
                        {
                            //string[] details = sqlite_row.DETAILS.Split(SmartParametersV2016.dc2);  // Special for compressed fields

                            SmartFinance.CryptoWalletTotals generic = new SmartFinance.CryptoWalletTotals()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    INSTITUTION_CODE = sqlite_row.INSTITUTION_CODE,
                                    BRAND_CODE = sqlite_row.BRAND_CODE,
                                    ADDRESS = sqlite_row.ADDRESS,
                                    CRYPTO_WALLET_NAME = sqlite_row.CRYPTO_WALLET_NAME,
                                    CRYPTO_CURRENCY_ORDINAL = sqlite_row.CRYPTO_CURRENCY_ORDINAL,
                                    CRYPTO_CURRENCY = sqlite_row.CRYPTO_CURRENCY,
                                    CRYPTO_AMOUNT = Convert.ToDouble(sqlite_row.CRYPTO_AMOUNT),
                                    RATE = Convert.ToDouble(sqlite_row.RATE),
                                    VALUE = sqlite_row.VALUE,
                                    Updated = false
                                };
                                financeviewmodel.PLO.crypto_wallettotalsList.Add(generic);
                            
                        }
                    }
                    financeviewmodel.PLO.sqlite_crypto_wallettotalsList.Clear();
                    break;
                case "Logins":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_finance_loginsList = new List<SmartFinance.LoginsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.LoginsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer && (username == requester))
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.LoginsSQLite sqliteRow in financeviewmodel.PLO.sqlite_finance_loginsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.LoginsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_finance_loginsList = Add_Secondary<SmartFinance.LoginsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_finance_loginsList.Count > 0)
                    {
                        foreach (SmartFinance.LoginsSQLite logins_row in financeviewmodel.PLO.sqlite_finance_loginsList)
                        {
                            key_details = logins_row.DETAILS;
                            if (string.IsNullOrEmpty(FDEK))
                            {
                                key_details_unencrypted = key_details;
                            }
                            else
                            {
                                key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, logins_row.RANDOMKEY, derived), key_details, false);
                                if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                            string[] details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                            if (details_fields.Length == 3)
                            {
                                SmartFinance.Logins login = new SmartFinance.Logins()
                                    {
                                        USERNAME = logins_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(logins_row.CUBEFACE_CODE),
                                        INSTITUTION_CODE = logins_row.INSTITUTION_CODE,
                                        BRAND_CODE = logins_row.BRAND_CODE,
                                        LOGIN_METHOD = logins_row.LOGIN_METHOD,
                                        ACTIVE_FLAG = Convert.ToChar(logins_row.ACTIVE_FLAG),
                                        OWNER = details_fields[0],
                                        CONTACT_EMAIL = details_fields[1],
                                        CONTACT_PHONENO = details_fields[2],
                                        RANDOMKEY = logins_row.RANDOMKEY,
                                        BANKSCHECKED = Convert.ToBoolean(logins_row.BANKSCHECKED),
                                        SAVINGSCHECKED = Convert.ToBoolean(logins_row.SAVINGSCHECKED),
                                        INVESTMENTSCHECKED = Convert.ToBoolean(logins_row.INVESTMENTSCHECKED),
                                        CRYPTOSCHECKED = Convert.ToBoolean(logins_row.CRYPTOSCHECKED),
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
                                        AvailableBrands = new List<BrandItem>(),
                                        AvailableConnections = new List<EntryItem>(),
                                        BrandName = "",
#endif
#if WPF  || WINUI || SMARTMAUI
                                        BanksVisibility = Visibility.Collapsed,
                                        SavingsVisibility = Visibility.Collapsed,
                                        InvestmentsVisibility = Visibility.Collapsed,
                                        CryptosVisibility = Visibility.Collapsed,
#endif
#if ANDROIDX
                                        BanksVisibility = ViewStates.Invisible,
                                        SavingsVisibility = ViewStates.Invisible,
                                        InvestmentsVisibility = ViewStates.Invisible,
                                        CryptosVisibility = ViewStates.Invisible,
#endif
                                        Updated = false,
                                        Delete = false
                                    };
                                    financeviewmodel.PLO.finance_loginsList.Add(login);
                                
                            }
                        }
                    }
                    financeviewmodel.PLO.sqlite_finance_loginsList.Clear();
                    break;
                case "Switches":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_finance_switchesList = new List<SmartFinance.SwitchesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.SwitchesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer && (username == requester))
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.SwitchesSQLite sqliteRow in financeviewmodel.PLO.sqlite_finance_switchesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.SwitchesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_finance_switchesList = Add_Secondary<SmartFinance.SwitchesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_finance_switchesList.Count > 0)
                    {
                        foreach (SmartFinance.SwitchesSQLite switches_row in financeviewmodel.PLO.sqlite_finance_switchesList)
                        {
                            key_details = switches_row.KEY_DETAILS;
                            if (string.IsNullOrEmpty(FDEK))
                            {
                                key_details_unencrypted = key_details;
                            }
                            else
                            {
                                key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, switches_row.RANDOMKEY, derived), key_details, false);
                                if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                            string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                            if (key_details_fields.Length == 3)
                            {
                                //string details_unencrypted;
                                //string details;//
                                //details = switches_row.DETAILS;
                                //if (string.IsNullOrEmpty(FDEK))
                                //{
                                //    details_unencrypted = details;
                                //}
                                //else
                                //{
                                //    details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, switches_row.RANDOMKEY2, derived), details, false);
                                //    if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                //    {
                                //        return false;
                                //    }
                                //}
                                //string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                //if (details_fields.Length == 1)
                                //{
                                SmartFinance.Switches switcher = new SmartFinance.Switches()
                                    {
                                        USERNAME = switches_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(switches_row.CUBEFACE_CODE),
                                        INSTITUTION_CODE = switches_row.INSTITUTION_CODE,
                                        BRAND_CODE = switches_row.BRAND_CODE,
                                        CATEGORY_CODE = Convert.ToChar(switches_row.CATEGORY_CODE),//Convert.ToChar(key_details_fields[0]),
                                        SWITCH_CREATED = switches_row.SWITCH_CREATED,
                                        SORTCODE = key_details_fields[0],
                                        ACCOUNT_NO = key_details_fields[1],
                                        UDPRN = key_details_fields[2],
                                        RANDOMKEY = switches_row.RANDOMKEY,
                                        AUTOSWITCH = Convert.ToChar(switches_row.AUTOSWITCH),
                                        LAST_DATETIME = switches_row.LAST_DATETIME,
                                        LAST_UPDATE = switches_row.LAST_UPDATE,
                                        EXPIRY_DATE = switches_row.EXPIRY_DATE,
                                        AUTOSWITCH_DESTINATION = switches_row.AUTOSWITCH_DESTINATION,
                                        AUTOSWITCH_INSTITUTION_CODE = switches_row.AUTOSWITCH_INSTITUTION_CODE,
                                        AUTOSWITCH_BRAND_CODE = switches_row.AUTOSWITCH_BRAND_CODE,
                                        AUTOSWITCH_EMAIL_SENT = switches_row.AUTOSWITCH_EMAIL_SENT
                                        //RANDOMKEY2 = switches_row.RANDOMKEY2
                                    };
                                    financeviewmodel.PLO.finance_switchesList.Add(switcher);
                                
                            }
                        }
                    }
                    financeviewmodel.PLO.sqlite_finance_switchesList.Clear();
                    break;
                case "Transactions":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_transactionsList = new List<SmartFinance.TransactionsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.TransactionsSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.TransactionsSQLite sqliteRow in financeviewmodel.PLO.sqlite_transactionsList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.TransactionsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_transactionsList = Add_Secondary<SmartFinance.TransactionsSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_transactionsList.Count > 0)
                    {
                        foreach (SmartFinance.TransactionsSQLite sqlite_row in financeviewmodel.PLO.sqlite_transactionsList)
                        {
                            key_details = sqlite_row.KEY_DETAILS;
                            if (string.IsNullOrEmpty(FDEK))
                            {
                                key_details_unencrypted = key_details;
                            }
                            else
                            {
                                key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                {
                                    return false;
                                }
                            }
                            string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                            if (key_details_fields.Length == 5)
                            {
                                SmartFinance.Transactions transaction = new SmartFinance.Transactions()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        INSTITUTION_CODE = Convert.ToInt16(sqlite_row.INSTITUTION_CODE),
                                        BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                                        SORTCODE = key_details_fields[0],
                                        ACCOUNT_NO = key_details_fields[1],
                                        UDPRN = key_details_fields[2],
                                        //CURRENCY_ORDINAL = Convert.ToInt16(key_details_fields[3]),
                                        STATEMENT_DATE = Convert.ToDateTime(key_details_fields[3]),
                                        STATEMENT_NO = Convert.ToInt16(key_details_fields[4]),
                                        RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                        Updated = false
                                    };
                                    financeviewmodel.PLO.transactionsList.Add(transaction);
                                
                            }
                        }
                    }
                    financeviewmodel.PLO.sqlite_transactionsList.Clear();
                    break;
                case "TransactionsCategories":
                    if (primary)
                    {
                        sql = Build_Select(schemaName, table_name, username);
                        try
                        {
                            financeviewmodel.PLO.sqlite_transactionscategoriesList = new List<SmartFinance.TransactionsCategoriesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartFinance.TransactionsCategoriesSQLite>(sql));
                        }
                        catch (Exception sqlex)
                        {
                            ourviewmodel.errorMessage = sqlex.Message;
                            return false;
                        }
                        if (DBServer)
                        {
                            ourviewmodel.recordCount = 0;
                            foreach (SmartFinance.TransactionsCategoriesSQLite sqliteRow in financeviewmodel.PLO.sqlite_transactionscategoriesList)
                            {
                                if (ourviewmodel.recordCount > 0)
                                {
                                    ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                }
                                ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartFinance.TransactionsCategoriesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                ourviewmodel.recordCount++;
                            }
                            return true;
                        }
                    }
                    else
                    {
                        financeviewmodel.PLO.sqlite_transactionscategoriesList = Add_Secondary<SmartFinance.TransactionsCategoriesSQLite>(ourviewmodel, username, groupname);
                    }
                    if (financeviewmodel.PLO.sqlite_transactionscategoriesList.Count > 0)
                    {
                        foreach (SmartFinance.TransactionsCategoriesSQLite sqlite_row in financeviewmodel.PLO.sqlite_transactionscategoriesList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(FDEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (key_details_fields.Length == 5)
                                {
                                    string details_unencrypted;
                                    string details;

                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(FDEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(FDEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 14)
                                    {
                                        SmartFinance.TransactionsCategories generic = new SmartFinance.TransactionsCategories()
                                            {
                                                USERNAME = sqlite_row.USERNAME,
                                                CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                                INSTITUTION_CODE = Convert.ToInt16(sqlite_row.INSTITUTION_CODE),
                                                BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                                                SORTCODE = key_details_fields[0],
                                                ACCOUNT_NO = key_details_fields[1],
                                                UDPRN = key_details_fields[2],
                                                STATEMENT_DATE = Convert.ToDateTime(key_details_fields[3]),
                                                STATEMENT_NO = Convert.ToInt16(key_details_fields[4]),
                                                SEQUENCE_NO = sqlite_row.SEQUENCE_NO,
                                                RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                                //
                                                TRANSACTION_DATE = Convert.ToDateTime(details_fields[0]),
                                                CREDITDEBIT_INDICATOR = Convert.ToInt16(details_fields[1]),
                                                PTC_CODE = Convert.ToInt32(details_fields[2]),
                                                TRANSGROUP_CODE = Convert.ToInt16(details_fields[3]),
                                                TRANSACTION_CODE = Convert.ToInt16(details_fields[4]),
                                                DESCRIPTION = details_fields[5],
                                                TYPE = details_fields[6],
                                                CRYPTO_AMOUNT = Convert.ToDouble(details_fields[7]),
                                                CRYPTO_CURRENCY_ORDINAL = Convert.ToInt16(details_fields[8]),
                                                AMOUNT = Convert.ToDouble(details_fields[9]),
                                                AMOUNT_CURRENCY_ORDINAL = Convert.ToInt16(details_fields[10]),
                                                BALANCE_AMOUNT = Convert.ToDouble(details_fields[11]),
                                                BALANCE_CURRENCY_ORDINAL = Convert.ToInt16(details_fields[12]),
                                                BALANCE_CREDITDEBIT_INDICATOR = Convert.ToInt16(details_fields[13]),
                                                //EXCHANGE_RATES = 0,
                                                RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                                Updated = false
                                            };
                                            financeviewmodel.PLO.transactionscategoriesList.Add(generic);
                                        
                                    }
                                }
                            }
                    }
                    financeviewmodel.PLO.sqlite_transactionscategoriesList.Clear();
                    break;
                default:
                    break;
            }
            return true;
        }
        internal static bool RemoveSmartFinance(MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    List<SmartData.SQLiteTables> Fatahtables,
                                                    short screenCode,
                                                    string schemaName,
                                                    string username)
        {
            // Cannot remove ourviewmodel.UserName btw
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            else
            {
                List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schemaName
                                select Tables);
                foreach (SmartData.SQLiteTables table in subsetTables)
                {
                    // All tables
                    if (ourviewmodel.cubeCode[screenCode])
                    {
                        if (!UnloadCommonFinance(financeviewmodel,
                                            //schema_name,
                                            table.TABLE_NAME,
                                            username))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal static bool UnloadCommonFinance(FinanceViewModel financeviewmodel,
                                                string table_name,
                                                string username)
        {
            switch (table_name)
            {
                case "Accounts":
                    // Unload Common Finance Cannot unload Primary!
                    if (financeviewmodel.PLO.finance_accountsList.Count > 0)
                    {
                        financeviewmodel.PLO.finance_accountsList = financeviewmodel.PLO.finance_accountsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                    }
                    break;                
                case "Categories":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.finance_categoriesList.Count > 0)
                    {
                        financeviewmodel.PLO.finance_categoriesList = financeviewmodel.PLO.finance_categoriesList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                    }
                    break;
                case "CategoryTypes":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.finance_categorytypesList.Count > 0)
                    {
                        financeviewmodel.PLO.finance_categorytypesList = financeviewmodel.PLO.finance_categorytypesList
                                .Where(row => row.USERNAME != username)
                                .ToList();                        
                    }
                    break;
                case "CryptoAccounts":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.crypto_accountsList.Count > 0)
                    {
                        financeviewmodel.PLO.crypto_accountsList = financeviewmodel.PLO.crypto_accountsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                    }
                    break;
                case "CryptoAddresses":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.crypto_addressesList.Count > 0)
                    {
                        financeviewmodel.PLO.crypto_addressesList = financeviewmodel.PLO.crypto_addressesList
                                .Where(row => row.USERNAME != username)
                                .ToList();                       
                    }
                    break;
                case "CryptoCurrencyRates":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.crypto_currenciesList.Count > 0)
                    {
                        financeviewmodel.PLO.crypto_currenciesList = financeviewmodel.PLO.crypto_currenciesList
                                .Where(row => row.USERNAME != username)
                                .ToList();                        
                    }
                    break;
                case "CryptoLedgers":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.crypto_ledgersList.Count > 0)
                    {
                        financeviewmodel.PLO.crypto_ledgersList = financeviewmodel.PLO.crypto_ledgersList
                                .Where(row => row.USERNAME != username)
                                .ToList();                        
                    }
                    break;
                case "CryptoWallets":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.crypto_walletsList.Count > 0)
                    {
                        financeviewmodel.PLO.crypto_walletsList = financeviewmodel.PLO.crypto_walletsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                        
                    }
                    break;
                case "CryptoWalletTotals":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.crypto_wallettotalsList.Count > 0)
                    {
                        financeviewmodel.PLO.crypto_wallettotalsList = financeviewmodel.PLO.crypto_wallettotalsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                        
                    }
                    break;
                case "Logins":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.finance_loginsList.Count > 0)
                    {
                        financeviewmodel.PLO.finance_loginsList = financeviewmodel.PLO.finance_loginsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                       
                    }
                    break;
                case "Switches":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.finance_switchesList.Count > 0)
                    {
                        financeviewmodel.PLO.finance_switchesList = financeviewmodel.PLO.finance_switchesList
                                .Where(row => row.USERNAME != username)
                                .ToList();                        
                    }
                    break;
                case "Transactions":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.transactionsList.Count > 0)
                    {
                        financeviewmodel.PLO.transactionsList = financeviewmodel.PLO.transactionsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                        
                    }
                    break;
                case "TransactionsCategories":
                    // Cannot unload Primary!
                    if (financeviewmodel.PLO.transactionscategoriesList.Count > 0)
                    {
                        financeviewmodel.PLO.transactionscategoriesList = financeviewmodel.PLO.transactionscategoriesList
                                .Where(row => row.USERNAME != username)
                                .ToList();                       
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        internal static async Task<bool> LoadSmartUtility(MainViewModel ourviewmodel,
                                                        short screenCode,
                                                        string schemaName,
                                                        string username,
                                                        string groupname)
        {
            bool status = false;
            if (string.IsNullOrEmpty(username))
            {
                ourviewmodel.errorMessage = "Username is empty";
                return status;
            }
            else
            {
                if (ourviewmodel.cubeCode[screenCode])
                {
                    // Account Charges Credits
                    if (!await StoreSQLite<SmartUtility.AccChargesCreditsSQLite>(ourviewmodel,
                        schemaName,
                        "AccChargesCredits",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // Accounts
                    if (!await StoreSQLite<SmartUtility.AccountsSQLite>(ourviewmodel,
                        schemaName,
                        "Accounts",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // BankDetails                
                    if (!await StoreSQLite<SmartUtility.BankDetailsSQLite>(ourviewmodel,
                        schemaName,
                        "BankDetails",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // Bills
                    if (!await StoreSQLite<SmartUtility.BillsSQLite>(ourviewmodel,
                       schemaName,
                        "Bills",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // Bills Resource
                    if (!await StoreSQLite<SmartUtility.BillsResourceSQLite>(ourviewmodel,
                        schemaName,
                        "BillsResource",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // ECosts
                    if (!await StoreSQLite<SmartUtility.ECostsSQLite>(ourviewmodel,
                        schemaName,
                        "ECosts",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // EDiscounts
                    if (!await StoreSQLite<SmartUtility.EDiscountsSQLite>(ourviewmodel,
                        schemaName,
                        "EDiscounts",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // EReadings
                    if (!await StoreSQLite<SmartUtility.EReadingsSQLite>(ourviewmodel,
                        schemaName,
                        "EReadings",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // EStandingCharges
                    if (!await StoreSQLite<SmartUtility.EStandingChargesSQLite>(ourviewmodel,
                        schemaName,
                        "EStandingCharges",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // EUnitCharges
                    if (!await StoreSQLite<SmartUtility.EUnitChargesSQLite>(ourviewmodel,
                        schemaName,
                        "EUnitCharges",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // EUsage
                    if (!await StoreSQLite<SmartUtility.EUsageSQLite>(ourviewmodel,
                        schemaName,
                        "EUsage",
                        username,
                        groupname))
                    {
                        return false;
                    }
                    // GCosts
                    if (!await StoreSQLite<SmartUtility.GCostsSQLite>(ourviewmodel,
                        schemaName,
                        "GCosts",
                        username,
                        groupname))
                    {
                        return false;
                    }

                    // GDiscounts
                    if (!await StoreSQLite<SmartUtility.GDiscountsSQLite>(ourviewmodel,
                        schemaName,
                        "GDiscounts",
                        username,
                        groupname))
                    {
                        return false;
                    }

                    // GReadings
                    if (!await StoreSQLite<SmartUtility.GReadingsSQLite>(ourviewmodel,
                        schemaName,
                        "GReadings",
                        username,
                        groupname))
                    {
                        return false;
                    }

                    // GStanding Charges
                    if (!await StoreSQLite<SmartUtility.GStandingChargesSQLite>(ourviewmodel,
                        schemaName,
                        "GStandingCharges",
                        username,
                        groupname))
                    {
                        return false;
                    }

                    // GUnit Charges
                    if (!await StoreSQLite<SmartUtility.GUnitChargesSQLite>(ourviewmodel,
                    schemaName,
                    "GUnitCharges",
                    username,
                        groupname))
                    {
                        return false;
                    }

                    // GUsage
                    if (!await StoreSQLite<SmartUtility.GUsageSQLite>(ourviewmodel,
                        schemaName,
                        "GUsage",
                        username,
                        groupname))
                    {
                        return false;
                    }

                    // Logins
                    if (!await StoreSQLite<SmartUtility.LoginsSQLite>(ourviewmodel,
                        schemaName,
                        "Logins",
                        username,
                        groupname))
                    {
                        return false;
                    }

                    // Meters
                    if (!await StoreSQLite<SmartUtility.MetersSQLite>(ourviewmodel,
                        schemaName,
                        "Meters",
                        username,
                        groupname))
                    {
                        return false;
                    }

                    // Payments
                    if (!await StoreSQLite<SmartUtility.PaymentsSQLite>(ourviewmodel,
                                        schemaName,
                                        "Payments",
                                        username,
                        groupname))
                    {
                        return false;
                    }

                    // Resources
                    if (!await StoreSQLite<SmartUtility.ResourcesSQLite>(ourviewmodel,
                                        schemaName,
                                        "Resources",
                                        username,
                        groupname))
                    {
                        return false;
                    }

                    // ResourcesTypes
                    if (!await StoreSQLite<SmartUtility.ResourcesTypesSQLite>(ourviewmodel,
                                        schemaName,
                                        "ResourcesTypes",
                                        username,
                        groupname))
                    {
                        return false;
                    }

                    // SupChargesCredits
                    if (!await StoreSQLite<SmartUtility.SupChargesCreditsSQLite>(ourviewmodel,
                                        schemaName,
                                        "SupChargesCredits",
                                        username,
                        groupname))
                    {
                        return false;
                    }

                    // Switches
                    if (!await StoreSQLite<SmartUtility.SwitchesSQLite>(ourviewmodel,
                                        schemaName,
                                        "Switches",
                                        username,
                        groupname))
                    {
                        return false;
                    }

                    // Tariff Details
                    if (!await StoreSQLite<SmartUtility.TariffDetailsSQLite>(ourviewmodel,
                                        schemaName,
                                        "TariffDetails",
                                        username,
                        groupname))
                    {
                        return false;
                    }

                    // Unallocated
                    if (!await StoreSQLite<SmartUtility.UnallocatedSQLite>(ourviewmodel,
                                        schemaName,
                                        "Unallocated",
                                        username,
                        groupname))
                    {
                        return false;
                    }                
                }
                // I will do it without that stuck up pompous bitch's help
                // Who needs your fucking measly £500??? Stupid ignorant overbearing cow
                // "Oh, I have to see a prospectus - " Shove your stupid prospectus up your arse you fucking blowhard bitch
                // Silly "I need to see a prospectus" fucking bitch
            }
            status = true;
            return status;
        }

        internal static async Task<bool> ExtractSmartUtility(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        List<SmartData.SQLiteTables> Fatahtables,
                                                        short screenCode,
                                                        bool primary,
                                                        string schema_name,
                                                        string requester,
                                                        string username,
                                                        string DEK)

        {
            bool status = false;
            if (string.IsNullOrEmpty(username))
            {
                utilityviewmodel.errorMessage = "Username is empty";
                return status;
            }

            utilityviewmodel.errorMessage = "";

            // Now for DEV OR PROD read the encrypted local table
            // and build our unencrypted working copy
            // Now build the unencrypted internal table from the LOCAL records (if there are any)
            List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schema_name
                                select Tables);

            foreach (SmartData.SQLiteTables table in subsetTables)
            {
                // All tables
                if (ourviewmodel.cubeCode[screenCode])
                {
                    if (!await LoadCommonUtility(ourviewmodel,
                                        utilityviewmodel,
                                        primary,
                                        schema_name,
                                        table.TABLE_NAME,
                                        requester,
                                        username,
                                        username,
                                        DEK,
                                        false))
                    {
                        return false;
                    }
                }
            }

            utilityviewmodel.Hezbollah.e_tariff_engineList = new List<SmartUtility.TariffEngine>();
            utilityviewmodel.Hezbollah.g_tariff_engineList = new List<SmartUtility.TariffEngine>();
            utilityviewmodel.Hezbollah.simulateList = new List<SmartUtility.Simulate>();
            // This has no USERNAME field because these are all calculated on the fly and never stored back at HQ
            // Could have more than one cost per engine item
            //utilityviewmodel.Hezbollah.tariff_costsList = new List<SmartUtility.TariffCosts>();


            //utilityviewmodel.Hezbollah.working_billsList = new List<SmartUtility.Bills>();

            // Do all this setup bollocks
            // But!! You need a utilityviewmodel.resourceCodes Set!!!
            // And! You havevn't got one!!
            //if (utilityviewmodel.Hezbollah.billsList.Count > 0)
            //{
            //    utilityviewmodel.Hezbollah.working_billsListx = SmartUtilityV2022.Working_BillsX(ourviewmodel,
            //                                                            utilityviewmodel,
            //                                                            SmartParametersV2016.defaultDate); // Remove is true here
            //
            //}
            utilityviewmodel.Hezbollah.d_analysis_costsList = new List<SmartUtility.AnalysisCosts>();
            status = true;
            return status;
        }

        internal static bool RemoveSmartUtility(MainViewModel ourviewmodel,
                                                        UtilityViewModel utilityviewmodel,
                                                        List<SmartData.SQLiteTables> Fatahtables,
                                                       short screenCode,
                                                       string schemaName,
                                                       string username)
        {
            bool status = false;
            if (string.IsNullOrEmpty(username))
            {
                utilityviewmodel.errorMessage = "Username is empty";
                return status;
            }
            else
            {
                utilityviewmodel.errorMessage = "";
                //int ss_count = 1;
                List<SmartData.SQLiteTables> subsetTables = new List<SmartData.SQLiteTables>(
                                from Tables in Fatahtables
                                where Tables.SCHEMA_NAME == schemaName
                                select Tables);
                foreach (SmartData.SQLiteTables table in subsetTables)
                {
                    // All tables
                    if (ourviewmodel.cubeCode[screenCode])
                    {
                        if (!UnLoadCommonUtility(utilityviewmodel,
                                            table.TABLE_NAME,
                                            username))
                        {
                            return false;
                        }
                    }
                }

                // I will do it without that stuck up pompous bitch's help
                // Who needs your fucking measly £500??? Stupid ignorant overbearing cow
                // "Oh, I have to see a prospectus - " Shove your stupid prospectus up your arse you fucking blowhard bitch
                // Silly "I need to see a prospectus" fucking bitch
                // with your paedophile fucking blowhard husband
            }
            return true;
        }

        internal static async Task<bool> LoadCommonUtility(MainViewModel ourviewmodel,
                                        UtilityViewModel utilityviewmodel,
                                        bool primary,
                                        string schemaName,
                                        string table_name,
                                        string requester,
                                        string username,
                                        string groupname,
                                        string DEK,
                                        bool DBServer)

        {
            bool status = false;
            if (string.IsNullOrEmpty(username))
            {
                utilityviewmodel.errorMessage = "Username is empty";
                return status;
            }
            else
            {
                utilityviewmodel.errorMessage = "";

                string sql;
                string key_details_unencrypted;
                string key_details;
                string deriveds = schemaName + "." + table_name;
                int derived = deriveds.Length;

                switch (table_name)
                {
                    case "AccChargesCredits":
                        // Now for DEV OR PROD read the encrypted local table
                        // and build our unencrypted working copy
                        // Now build the unencrypted internal table from the LOCAL records (if there are any)
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList = new List<SmartUtility.AccChargesCreditsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.AccChargesCreditsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.AccChargesCreditsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.AccChargesCreditsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList = Add_Secondary<SmartUtility.AccChargesCreditsSQLite>(ourviewmodel, username, groupname);
                        }

                        if (utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList.Count > 0)
                        {
                            foreach (SmartUtility.AccChargesCreditsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 5)
                                {
                                    string details_unencrypted;
                                    string details;
                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 4)
                                    {
                                        SmartUtility.AccChargesCredits table_row = new SmartUtility.AccChargesCredits()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            ACCOUNT_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[3]),
                                            ACCOUNT_ITEM = Convert.ToInt16(key_details_fields[4]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            ACCOUNT_TYPE = details_fields[0],
                                            ACCOUNT_VAT_CODE = Convert.ToInt16(details_fields[1]),
                                            ACCOUNT_AMOUNT = Convert.ToInt32(details_fields[2]),
                                            INCLUDE_BILLS = details_fields[3],
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.account_charges_creditsList.Add(table_row);
                                    }
                                }
                            }
                        }
                        // No Updates cos its derived However! There will be Inserts!
                        utilityviewmodel.Hezbollah.sqlite_account_charges_creditsList.Clear();
                        break;
                    case "Accounts":
                        // Load Common Utility
                        // Now for DEV OR PROD read the encrypted local table
                        // and build our unencrypted working copy
                        // Now build the unencrypted internal table from the LOCAL records (if there are any)
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_utility_accountsList = new List<SmartUtility.AccountsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.AccountsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.AccountsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_utility_accountsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.AccountsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_utility_accountsList = Add_Secondary<SmartUtility.AccountsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_utility_accountsList.Count > 0)
                        {
                            foreach (SmartUtility.AccountsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_utility_accountsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 2)
                                {
                                    string details_unencrypted;
                                    string details;

                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 4)
                                    {
                                        SmartUtility.Accounts accounts_row = new SmartUtility.Accounts()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            UDPRN = key_details_fields[1],
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            STATUS = Convert.ToChar(details_fields[0]),
                                            CURRENCY_ORDINAL = Convert.ToInt16(details_fields[1]),
                                            TARIFF_CODE = Convert.ToInt32(details_fields[2]),
                                            PAYMENT_PLAN = Convert.ToChar(details_fields[3]),
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2
                                        };
                                        utilityviewmodel.Hezbollah.utility_accountsList.Add(accounts_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_utility_accountsList.Clear();
                        break;                    
                    case "BankDetails":
                        // Now for DEV OR PROD read the encrypted local table
                        // and build our unencrypted working copy
                        // Now build the unencrypted internal table from the LOCAL records (if there are any)
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList = new List<SmartUtility.BankDetailsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.BankDetailsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.BankDetailsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.BankDetailsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList = Add_Secondary<SmartUtility.BankDetailsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList.Count > 0)
                        {
                            foreach (SmartUtility.BankDetailsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (key_details_fields.Length == 1)
                                {
                                    string details_unencrypted;
                                    string details;
                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 5)
                                    {
                                        SmartUtility.BankDetails bankdetails_row = new SmartUtility.BankDetails()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            FINANCE_INSTITUTION_CODE = Convert.ToInt16(details_fields[0]),
                                            FINANCE_BRAND_CODE = Convert.ToInt16(details_fields[1]),
                                            FINANCE_SORTCODE = details_fields[2],
                                            FINANCE_ACCOUNT_NO = details_fields[3],
                                            BANK_PAYDAY = Convert.ToInt16(details_fields[4]),
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.utility_bankdetailsList.Add(bankdetails_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_utility_bankdetailsList.Clear();
                        break;
                    case "Bills":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_billsList = new List<SmartUtility.BillsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.BillsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.BillsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_billsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.BillsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_billsList = Add_Secondary<SmartUtility.BillsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_billsList.Count > 0)
                        {
                            foreach (SmartUtility.BillsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_billsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 3)
                                {
                                    string details_unencrypted;
                                    string details;
                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 22)
                                    {
                                        SmartUtility.Bills table_row = new SmartUtility.Bills()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            BILL_PERIOD_START = SmartTimeV2016.ConvertDateTime(details_fields[0]),
                                            BILL_PERIOD_END = SmartTimeV2016.ConvertDateTime(details_fields[1]),
                                            PAYMENT_PLAN = Convert.ToChar(details_fields[2]),
                                            RESOURCE_BALANCES = Convert.ToBoolean(details_fields[3]),
                                            PREVIOUS_BALANCE = Convert.ToInt32(details_fields[4]),
                                            PAYMENTS_RECEIVED = Convert.ToInt32(details_fields[5]),
                                            ACCOUNT_CHARGES_CREDITS = Convert.ToInt32(details_fields[6]),
                                            ACCOUNT_CHARGES_CREDITS_VAT_AMOUNT = Convert.ToInt32(details_fields[7]),
                                            SUPPLY_CHARGES_CREDITS = Convert.ToInt32(details_fields[8]),
                                            SUPPLY_CHARGES_CREDITS_VAT_AMOUNT = Convert.ToInt32(details_fields[9]),
                                            BILL_VAT_AMOUNT = Convert.ToInt32(details_fields[10]),
                                            BILL_VAT_CODE = Convert.ToInt16(details_fields[11]),
                                            OUTSTANDING_BALANCE = Convert.ToInt32(details_fields[12]),
                                            TOTAL_NOW_DUE = Convert.ToInt32(details_fields[13]),
                                            MONTHLY_PAYMENT = Convert.ToInt32(details_fields[14]),
                                            PAYMENT_DUE_DATE = SmartTimeV2016.ConvertDateTime(details_fields[15]),
                                            PAYMENT_TYPE = details_fields[16],
                                            DIRECT_DEBIT_DATE = SmartTimeV2016.ConvertDateTime(details_fields[17]),
                                            LOYALTY_BONUS = Convert.ToDecimal(details_fields[18]),
                                            DISCOUNT_CREDIT_DATE = SmartTimeV2016.ConvertDateTime(details_fields[19]),
                                            FIRST_YEARS_DISCOUNT = details_fields[20],
                                            REWARDS = details_fields[21],
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.billsList.Add(table_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_billsList.Clear();
                        break;
                    case "BillsResource":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_bills_resourceList = new List<SmartUtility.BillsResourceSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.BillsResourceSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.BillsResourceSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_bills_resourceList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.BillsResourceSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_bills_resourceList = Add_Secondary<SmartUtility.BillsResourceSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_bills_resourceList.Count > 0)
                        {
                            foreach (SmartUtility.BillsResourceSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_bills_resourceList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 4)
                                {
                                    string details_unencrypted;
                                    string details;
                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 8)
                                    {
                                        SmartUtility.BillsResource table_row = new SmartUtility.BillsResource()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            MPAN_MPRN = key_details_fields[3],
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            RESOURCE_ACCOUNT_NO = details_fields[0],
                                            TARIFF_CODE = Convert.ToInt32(details_fields[1]),
                                            NEW_CHARGES = Convert.ToInt32(details_fields[2]),
                                            RESOURCE_DISCOUNTS = Convert.ToInt32(details_fields[3]),
                                            RESOURCE_VAT_AMOUNT = Convert.ToInt32(details_fields[4]),
                                            RESOURCE_VAT_CODE = Convert.ToInt16(details_fields[5]),
                                            PREVIOUS_RESOURCE_BALANCE = Convert.ToInt32(details_fields[6]),
                                            OUTSTANDING_RESOURCE_BALANCE = Convert.ToInt32(details_fields[7]),
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.bills_resourceList.Add(table_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_bills_resourceList.Clear();
                        break;
                    case "ECosts":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_e_costsList = new List<SmartUtility.ECostsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.ECostsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.ECostsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_e_costsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.ECostsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_e_costsList = Add_Secondary<SmartUtility.ECostsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_e_costsList.Count > 0)
                        {
                            foreach (SmartUtility.ECostsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_e_costsList)
                            {
                                SmartUtility.AnalysisCosts abc = new SmartUtility.AnalysisCosts()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                    UNIQUE_NUMBER = Convert.ToInt16(sqlite_row.UNIQUE_NUMBER),
                                    SUPPLIER_CODE = Convert.ToInt16(sqlite_row.SUPPLIER_CODE),
                                    BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                                    TARIFF_CODE = Convert.ToInt32(sqlite_row.TARIFF_CODE),
                                    TCR = Convert.ToDecimal(sqlite_row.TCR),
                                    RESOURCE_TYPE = sqlite_row.RESOURCE_TYPE,
                                    PAYMENT_PLAN = sqlite_row.PAYMENT_PLAN,
                                    TOTAL_COST = Convert.ToInt32(sqlite_row.TOTAL_COST)
                                };
                                utilityviewmodel.Hezbollah.e_analysis_costsList.Add(abc);
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_e_costsList.Clear();
                        break;
                    case "EDiscounts":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_e_discountsList = new List<SmartUtility.EDiscountsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.EDiscountsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.EDiscountsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_e_discountsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.EDiscountsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_e_discountsList = Add_Secondary<SmartUtility.EDiscountsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_e_discountsList.Count > 0)
                        {
                            foreach (SmartUtility.EDiscountsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_e_discountsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (key_details_fields.Length == 6)
                                {
                                    SmartUtility.EDiscounts ediscounts_row = new SmartUtility.EDiscounts()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                        ACCOUNT_NO = key_details_fields[0],
                                        STATEMENT_ID = key_details_fields[1],
                                        BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                        MPAN_MPRN = key_details_fields[3],
                                        DISCOUNT_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                        DISCOUNT_ITEM = Convert.ToInt16(key_details_fields[5]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        DISCOUNT_TYPE = sqlite_row.DISCOUNT_TYPE,
                                        DISCOUNT_VAT_CODE = sqlite_row.DISCOUNT_VAT_CODE,
                                        DISCOUNT_AMOUNT = sqlite_row.DISCOUNT_AMOUNT,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.e_discountsList.Add(ediscounts_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_e_discountsList.Clear();
                        break;
                    case "EReadings":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_e_readingsList = new List<SmartUtility.EReadingsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.EReadingsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.EReadingsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_e_readingsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.EReadingsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_e_readingsList = Add_Secondary<SmartUtility.EReadingsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_e_readingsList.Count > 0)
                        {
                            foreach (SmartUtility.EReadingsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_e_readingsList)
                            {
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    return false;
                                }
                                key_details = sqlite_row.KEY_DETAILS;
                                key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                {
                                    return false;
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (key_details_fields.Length == 6)
                                {
                                    string details = sqlite_row.DETAILS;
                                    string details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 1)
                                    {
                                        SmartUtility.EReadings ereadings_row = new SmartUtility.EReadings()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            MPAN_MPRN = key_details_fields[3],
                                            READINGS_PERIOD_START = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                            READINGS_PERIOD_END = SmartTimeV2016.ConvertDateTime(key_details_fields[5]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            METER_SERIAL_NO = details_fields[0],
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            READ_TYPE = sqlite_row.READ_TYPE,
                                            D_LAST_READ = sqlite_row.D_LAST_READ,
                                            D_THIS_READ = sqlite_row.D_THIS_READ,
                                            D_UNITS_USED = sqlite_row.D_UNITS_USED,
                                            N_LAST_READ = sqlite_row.N_LAST_READ,
                                            N_THIS_READ = sqlite_row.N_THIS_READ,
                                            N_UNITS_USED = sqlite_row.N_UNITS_USED,
                                            UNIT_OF_MEASURE = sqlite_row.UNIT_OF_MEASURE,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.e_readingsList.Add(ereadings_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_e_readingsList.Clear();
                        break;
                    case "EStandingCharges":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList = new List<SmartUtility.EStandingChargesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.EStandingChargesSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.EStandingChargesSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.EStandingChargesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList = Add_Secondary<SmartUtility.EStandingChargesSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList.Count > 0)
                        {
                            foreach (SmartUtility.EStandingChargesSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 7)
                                {
                                    SmartUtility.EStandingCharges estandingcharges_row = new SmartUtility.EStandingCharges()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                        ACCOUNT_NO = key_details_fields[0],
                                        STATEMENT_ID = key_details_fields[1],
                                        BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                        MPAN_MPRN = key_details_fields[3],
                                        STANDING_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                        STANDING_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(key_details_fields[5]),
                                        CHARGES_ITEM = Convert.ToInt16(key_details_fields[6]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        CHARGES_TYPE = sqlite_row.CHARGES_TYPE,
                                        STANDING_CHARGE = sqlite_row.STANDING_CHARGE,
                                        CHARGES_DAYS = sqlite_row.CHARGES_DAYS,
                                        CHARGES_COST = sqlite_row.CHARGES_COST,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.e_standing_chargesList.Add(estandingcharges_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_e_standing_chargesList.Clear();
                        break;
                    case "EUnitCharges":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList = new List<SmartUtility.EUnitChargesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.EUnitChargesSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.EUnitChargesSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.EUnitChargesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList = Add_Secondary<SmartUtility.EUnitChargesSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList.Count > 0)
                        {
                            foreach (SmartUtility.EUnitChargesSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 8)
                                {
                                    SmartUtility.EUnitCharges eunitcharges_row = new SmartUtility.EUnitCharges()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                        ACCOUNT_NO = key_details_fields[0],
                                        STATEMENT_ID = key_details_fields[1],
                                        BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                        MPAN_MPRN = key_details_fields[3],
                                        UNIT_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                        UNIT_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(key_details_fields[5]),
                                        UNITS_TIME = Convert.ToChar(key_details_fields[6]),
                                        UNITS_BAND = Convert.ToInt16(key_details_fields[7]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        UNITS_TYPE = sqlite_row.UNITS_TYPE,
                                        UNITS = sqlite_row.UNITS,
                                        UNITS_RATE = sqlite_row.UNITS_RATE,
                                        UNIT_OF_MEASURE = sqlite_row.UNIT_OF_MEASURE,
                                        UNITS_COST = sqlite_row.UNITS_COST,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.e_unit_chargesList.Add(eunitcharges_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_e_unit_chargesList.Clear();
                        break;
                    case "EUsage":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_e_usageList = new List<SmartUtility.EUsageSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.EUsageSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.EUsageSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_e_usageList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.EUsageSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_e_usageList = Add_Secondary<SmartUtility.EUsageSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_e_usageList.Count > 0)
                        {
                            foreach (SmartUtility.EUsageSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_e_usageList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 2)
                                {
                                    SmartUtility.EUsage eusage_row = new SmartUtility.EUsage()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        MPAN_MPRN = key_details_fields[0],
                                        UNIQUE_INDEX = Convert.ToInt16(key_details_fields[1]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        USAGE_DATETIME = sqlite_row.USAGE_DATETIME,
                                        USAGE_TOTAL = sqlite_row.USAGE_TOTAL,
                                        USAGE_VALUE = sqlite_row.USAGE_VALUE,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.e_usageList.Add(eusage_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_e_usageList.Clear();
                        break;
                    case "GCosts":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_g_costsList = new List<SmartUtility.GCostsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.GCostsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.GCostsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_g_costsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.GCostsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_g_costsList = Add_Secondary<SmartUtility.GCostsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_g_costsList.Count > 0)
                        {
                            foreach (SmartUtility.GCostsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_g_costsList)
                            {
                                SmartUtility.AnalysisCosts abc = new SmartUtility.AnalysisCosts()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                    UNIQUE_NUMBER = Convert.ToInt16(sqlite_row.UNIQUE_NUMBER),
                                    SUPPLIER_CODE = Convert.ToInt16(sqlite_row.SUPPLIER_CODE),
                                    BRAND_CODE = Convert.ToInt16(sqlite_row.BRAND_CODE),
                                    TARIFF_CODE = Convert.ToInt32(sqlite_row.TARIFF_CODE),
                                    TCR = Convert.ToDecimal(sqlite_row.TCR),
                                    RESOURCE_TYPE = sqlite_row.RESOURCE_TYPE,
                                    PAYMENT_PLAN = sqlite_row.PAYMENT_PLAN,
                                    TOTAL_COST = Convert.ToInt32(sqlite_row.TOTAL_COST)
                                };
                                utilityviewmodel.Hezbollah.g_analysis_costsList.Add(abc);
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_g_costsList.Clear();
                        break;
                    case "GDiscounts":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_g_discountsList = new List<SmartUtility.GDiscountsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.GDiscountsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.GDiscountsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_g_discountsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.GDiscountsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_g_discountsList = Add_Secondary<SmartUtility.GDiscountsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_g_discountsList.Count > 0)
                        {
                            foreach (SmartUtility.GDiscountsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_g_discountsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 6)
                                {
                                    SmartUtility.GDiscounts gdiscounts_row = new SmartUtility.GDiscounts()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                        ACCOUNT_NO = key_details_fields[0],
                                        STATEMENT_ID = key_details_fields[1],
                                        BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                        MPAN_MPRN = key_details_fields[3],
                                        DISCOUNT_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                        DISCOUNT_ITEM = Convert.ToInt16(key_details_fields[5]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        DISCOUNT_TYPE = sqlite_row.DISCOUNT_TYPE,
                                        DISCOUNT_VAT_CODE = sqlite_row.DISCOUNT_VAT_CODE,
                                        DISCOUNT_AMOUNT = sqlite_row.DISCOUNT_AMOUNT,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.g_discountsList.Add(gdiscounts_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_g_discountsList.Clear();
                        break;
                    case "GReadings":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_g_readingsList = new List<SmartUtility.GReadingsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.GReadingsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.GReadingsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_g_readingsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.GReadingsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_g_readingsList = Add_Secondary<SmartUtility.GReadingsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_g_readingsList.Count > 0)
                        {
                            foreach (SmartUtility.GReadingsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_g_readingsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 6)
                                {
                                    string details_unencrypted;
                                    string details;
                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 1)
                                    {
                                        SmartUtility.GReadings greadings_row = new SmartUtility.GReadings()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            MPAN_MPRN = key_details_fields[3],
                                            READINGS_PERIOD_START = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                            READINGS_PERIOD_END = SmartTimeV2016.ConvertDateTime(key_details_fields[5]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            METER_SERIAL_NO = details_fields[0],
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            READ_TYPE = sqlite_row.READ_TYPE,
                                            D_LAST_READ = sqlite_row.D_LAST_READ,
                                            D_THIS_READ = sqlite_row.D_THIS_READ,
                                            D_UNITS_USED_M3 = sqlite_row.D_UNITS_USED_M3,
                                            UNIT_OF_MEASURE = sqlite_row.UNIT_OF_MEASURE,
                                            D_UNITS_USED_KWH = sqlite_row.D_UNITS_USED_KWH,
                                            CALORIFIC_VALUE = sqlite_row.CALORIFIC_VALUE,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.g_readingsList.Add(greadings_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_g_readingsList.Clear();
                        break;
                    case "GStandingCharges":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList = new List<SmartUtility.GStandingChargesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.GStandingChargesSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.GStandingChargesSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.GStandingChargesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList = Add_Secondary<SmartUtility.GStandingChargesSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList.Count > 0)
                        {
                            foreach (SmartUtility.GStandingChargesSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 7)
                                {
                                    SmartUtility.GStandingCharges gstandingcharges_row = new SmartUtility.GStandingCharges()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                        ACCOUNT_NO = key_details_fields[0],
                                        STATEMENT_ID = key_details_fields[1],
                                        BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                        MPAN_MPRN = key_details_fields[3],
                                        STANDING_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                        STANDING_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(key_details_fields[5]),
                                        CHARGES_ITEM = Convert.ToInt16(key_details_fields[6]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        CHARGES_TYPE = sqlite_row.CHARGES_TYPE,
                                        STANDING_CHARGE = sqlite_row.STANDING_CHARGE,
                                        CHARGES_DAYS = sqlite_row.CHARGES_DAYS,
                                        CHARGES_COST = sqlite_row.CHARGES_COST,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.g_standing_chargesList.Add(gstandingcharges_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_g_standing_chargesList.Clear();
                        break;
                    case "GUnitCharges":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList = new List<SmartUtility.GUnitChargesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.GUnitChargesSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.GUnitChargesSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.GUnitChargesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList = Add_Secondary<SmartUtility.GUnitChargesSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList.Count > 0)
                        {
                            foreach (SmartUtility.GUnitChargesSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 7)
                                {
                                    SmartUtility.GUnitCharges gunitcharges_row = new SmartUtility.GUnitCharges()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                        ACCOUNT_NO = key_details_fields[0],
                                        STATEMENT_ID = key_details_fields[1],
                                        BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                        MPAN_MPRN = key_details_fields[3],
                                        UNIT_CHARGES_PERIOD_START = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                        UNIT_CHARGES_PERIOD_END = SmartTimeV2016.ConvertDateTime(key_details_fields[5]),
                                        UNITS_BAND = Convert.ToInt16(key_details_fields[6]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        UNITS_TYPE = sqlite_row.UNITS_TYPE,
                                        UNITS = sqlite_row.UNITS,
                                        UNITS_RATE = sqlite_row.UNITS_RATE,
                                        UNIT_OF_MEASURE = sqlite_row.UNIT_OF_MEASURE,
                                        UNITS_COST = sqlite_row.UNITS_COST,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.g_unit_chargesList.Add(gunitcharges_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_g_unit_chargesList.Clear();
                        break;
                    case "GUsage":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_g_usageList = new List<SmartUtility.GUsageSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.GUsageSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.GUsageSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_g_usageList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.GUsageSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_g_usageList = Add_Secondary<SmartUtility.GUsageSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_g_usageList.Count > 0)
                        {
                            foreach (SmartUtility.GUsageSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_g_usageList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 2)
                                {
                                    SmartUtility.GUsage gusage_row = new SmartUtility.GUsage()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        MPAN_MPRN = key_details_fields[0],
                                        UNIQUE_INDEX = Convert.ToInt16(key_details_fields[1]),
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        USAGE_DATETIME = sqlite_row.USAGE_DATETIME,
                                        USAGE_TOTAL = sqlite_row.USAGE_TOTAL,
                                        USAGE_VALUE = sqlite_row.USAGE_VALUE
                                    };
                                    utilityviewmodel.Hezbollah.g_usageList.Add(gusage_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_g_usageList.Clear();
                        break;
                    case "Logins":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_utility_loginsList = new List<SmartUtility.LoginsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.LoginsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer && (username == requester))
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.LoginsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_utility_loginsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.LoginsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_utility_loginsList = Add_Secondary<SmartUtility.LoginsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_utility_loginsList.Count > 0)
                        {
                            string details_unencrypted;
                            string details;

                            foreach (SmartUtility.LoginsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_utility_loginsList)
                            {
                                details = sqlite_row.DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    details_unencrypted = details;
                                }
                                else
                                {
                                    details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                // https://www.codegrepper.com/code-examples/csharp/split+nullable+in+c%23
                                // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator
                                // The null - coalescing operator ?? returns the value of its
                                // left - hand operand if it isn't null; otherwise, it
                                // evaluates the right-hand operand and returns its result.
                                // The ?? operator doesn't evaluate its right-hand operand
                                // if the left - hand operand evaluates to non-null.

                                // Available in C# 8.0 and later, the null-coalescing
                                // assignment operator ??= assigns the value of its
                                // right-hand operand to its left-hand operand only if the
                                // left-hand operand evaluates to null. The ??= operator
                                // doesn't evaluate its right-hand operand if the left-hand
                                // operand evaluates to non-null.                                
                                string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (details_fields.Length == 2)
                                {
                                    SmartUtility.Logins logins_row = new SmartUtility.Logins()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        //RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        LOGIN_CREATED = sqlite_row.LOGIN_CREATED,
                                        USER_ID = details_fields[0],
                                        USER_PASSWORD = details_fields[1],
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.utility_loginsList.Add(logins_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_utility_loginsList.Clear();
                        break;
                    case "Meters":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_utility_metersList = new List<SmartUtility.MetersSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.MetersSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.MetersSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_utility_metersList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.MetersSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_utility_metersList = Add_Secondary<SmartUtility.MetersSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_utility_metersList.Count > 0)
                        {
                            foreach (SmartUtility.MetersSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_utility_metersList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                // https://www.codegrepper.com/code-examples/csharp/split+nullable+in+c%23
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 1)
                                {
                                    string details_unencrypted;
                                    string details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 1)
                                    {
                                        SmartUtility.Meters meters_row = new SmartUtility.Meters()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                            RESOURCE_TYPE = sqlite_row.RESOURCE_TYPE,
                                            MPAN_MPRN = key_details_fields[0],
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            METER_SERIAL_NO = details_fields[0],
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.utility_metersList.Add(meters_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_utility_metersList.Clear();
                        break;
                    case "Payments":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_paymentsList = new List<SmartUtility.PaymentsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.PaymentsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.PaymentsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_paymentsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.PaymentsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_paymentsList = Add_Secondary<SmartUtility.PaymentsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_paymentsList.Count > 0)
                        {
                            foreach (SmartUtility.PaymentsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_paymentsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                // https://www.codegrepper.com/code-examples/csharp/split+nullable+in+c%23
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (key_details_fields.Length == 5)
                                {
                                    string details_unencrypted;
                                    string details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    // https://www.codegrepper.com/code-examples/csharp/split+nullable+in+c%23
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 3)
                                    {
                                        SmartUtility.Payments payments_row = new SmartUtility.Payments()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            PAYMENT_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[3]),
                                            PAYMENT_ITEM = Convert.ToInt16(key_details_fields[4]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            PAYMENT_CODE = Convert.ToInt16(details_fields[0]),
                                            PAYMENT_AMOUNT = Convert.ToInt32(details_fields[1]),
                                            PAYMENT_BALANCE = Convert.ToInt32(details_fields[2]),
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.paymentsList.Add(payments_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_paymentsList.Clear();
                        break;
                    case "Resources":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_utility_resourcesList = new List<SmartUtility.ResourcesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.ResourcesSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.ResourcesSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_utility_resourcesList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.ResourcesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_utility_resourcesList = Add_Secondary<SmartUtility.ResourcesSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_utility_resourcesList.Count > 0)
                        {
                            foreach (SmartUtility.ResourcesSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_utility_resourcesList)
                            {
                                SmartUtility.Resources resources_row = new SmartUtility.Resources()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                    CHECKED = sqlite_row.CHECKED,
                                    TOTAL_USAGE = sqlite_row.TOTAL_USAGE,
                                    UNITRATES_UPDATED = sqlite_row.UNITRATES_UPDATED,
                                    LAST_DATETIME = sqlite_row.LAST_DATETIME,
                                    LAST_UPDATE = sqlite_row.LAST_UPDATE,
                                    EXPIRY_DATE = sqlite_row.EXPIRY_DATE,
                                    AUTOSWITCH = Convert.ToChar(sqlite_row.AUTOSWITCH),
                                    Updated = false
                                };
                                utilityviewmodel.Hezbollah.utility_resourcesList.Add(resources_row);
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_utility_resourcesList.Clear();
                        break;
                    case "ResourcesTypes":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList = new List<SmartUtility.ResourcesTypesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.ResourcesTypesSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.ResourcesTypesSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.ResourcesTypesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList = Add_Secondary<SmartUtility.ResourcesTypesSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList.Count > 0)
                        {
                            foreach (SmartUtility.ResourcesTypesSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList)
                            {
                                SmartUtility.ResourcesTypes resourcestypes_row = new SmartUtility.ResourcesTypes()
                                {
                                    USERNAME = sqlite_row.USERNAME,
                                    CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                    RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                    RESOURCE_TYPE = sqlite_row.RESOURCE_TYPE,
                                    Updated = false
                                };
                                utilityviewmodel.Hezbollah.utility_resources_typesList.Add(resourcestypes_row);
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_utility_resources_typesList.Clear();
                        break;
                    case "SupChargesCredits":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList = new List<SmartUtility.SupChargesCreditsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.SupChargesCreditsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.SupChargesCreditsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.SupChargesCreditsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList = Add_Secondary<SmartUtility.SupChargesCreditsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList.Count > 0)
                        {
                            foreach (SmartUtility.SupChargesCreditsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                // https://www.codegrepper.com/code-examples/csharp/split+nullable+in+c%23
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 5)
                                {
                                    string details_unencrypted;
                                    string details;

                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    // https://www.codegrepper.com/code-examples/csharp/split+nullable+in+c%23
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                    if (details_fields.Length == 5)
                                    {
                                        SmartUtility.SupChargesCredits supply_charges_credits_row = new SmartUtility.SupChargesCredits()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            SUPPLY_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[3]),
                                            SUPPLY_ITEM = Convert.ToInt16(key_details_fields[4]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            SUPPLY_TYPE = details_fields[0],
                                            SUPPLY_VAT_CODE = Convert.ToInt16(details_fields[1]),
                                            SUPPLY_AMOUNT = Convert.ToInt32(details_fields[2]),
                                            SUPPLY_DUE_DATE = SmartTimeV2016.ConvertDateTime(details_fields[3]),
                                            SUPPLY_CREDIT_BILL = details_fields[4],
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = true
                                        };
                                        utilityviewmodel.Hezbollah.supply_charges_creditsList.Add(supply_charges_credits_row);
                                    }
                                }
                            }
                        }
                        // No Updates cos its derived However! There will be Inserts!
                        utilityviewmodel.Hezbollah.sqlite_supply_charges_creditsList.Clear();
                        break;
                    case "Switches":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_utility_switchesList = new List<SmartUtility.SwitchesSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.SwitchesSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer && (username == requester))
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.SwitchesSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_utility_switchesList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.SwitchesSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_utility_switchesList = Add_Secondary<SmartUtility.SwitchesSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_utility_switchesList.Count > 0)
                        {
                            foreach (SmartUtility.SwitchesSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_utility_switchesList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                // https://www.codegrepper.com/code-examples/csharp/split+nullable+in+c%23
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                if (key_details_fields.Length == 2)
                                {
                                    SmartUtility.Switches table_row = new SmartUtility.Switches()
                                    {
                                        USERNAME = sqlite_row.USERNAME,
                                        CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                        SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                        BRAND_CODE = sqlite_row.BRAND_CODE,
                                        RESOURCE_CODE = Convert.ToChar(sqlite_row.RESOURCE_CODE),
                                        SWITCH_CREATED = sqlite_row.SWITCH_CREATED,
                                        ACCOUNT_NO = key_details_fields[0],
                                        MPAN_MPRN = key_details_fields[1],
                                        RANDOMKEY = sqlite_row.RANDOMKEY,
                                        AGE_60PLUS = sqlite_row.AGE_60PLUS,
                                        AUTOSWITCH_DESTINATION = sqlite_row.AUTOSWITCH_DESTINATION,
                                        AUTOSWITCH_SUPPLIER_CODE = sqlite_row.AUTOSWITCH_SUPPLIER_CODE,
                                        AUTOSWITCH_BRAND_CODE = sqlite_row.AUTOSWITCH_BRAND_CODE,
                                        AUTOSWITCH_TARIFF_CODE = sqlite_row.AUTOSWITCH_TARIFF_CODE,
                                        AUTOSWITCH_EMAIL_SENT = sqlite_row.AUTOSWITCH_EMAIL_SENT,
                                        Updated = false
                                    };
                                    utilityviewmodel.Hezbollah.utility_switchesList.Add(table_row);
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_utility_switchesList.Clear();
                        break;
                    case "TariffDetails":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_tariff_detailsList = new List<SmartUtility.TariffDetailsSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.TariffDetailsSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.TariffDetailsSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_tariff_detailsList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.TariffDetailsSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_tariff_detailsList = Add_Secondary<SmartUtility.TariffDetailsSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_tariff_detailsList.Count > 0)
                        {
                            foreach (SmartUtility.TariffDetailsSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_tariff_detailsList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 9)
                                {
                                    string details_unencrypted;
                                    string details;

                                    details = sqlite_row.DETAILS;
                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 5)
                                    {
                                        SmartUtility.TariffDetails tariff_details_row = new SmartUtility.TariffDetails()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            MPAN_MPRN = key_details_fields[3],
                                            PRICES_VALID_FROM = SmartTimeV2016.ConvertDateTime(key_details_fields[4]),
                                            PRICES_VALID_TO = SmartTimeV2016.ConvertDateTime(key_details_fields[5]),
                                            TARIFF_CODE = Convert.ToInt32(key_details_fields[6]),
                                            PAYMENT_PLAN = Convert.ToChar(key_details_fields[7]),
                                            TIER_LEVEL = Convert.ToInt16(key_details_fields[8]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            AREA_CODE = Convert.ToInt16(details_fields[0]),
                                            SC = details_fields[1],
                                            DR = details_fields[2],
                                            NR = details_fields[3],
                                            TCR = details_fields[4],
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.tariff_detailsList.Add(tariff_details_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_tariff_detailsList.Clear();
                        break;
                    case "Unallocated":
                        if (primary)
                        {
                            sql = Build_Select(schemaName, table_name, username);
                            try
                            {
                                utilityviewmodel.Hezbollah.sqlite_unallocatedList = new List<SmartUtility.UnallocatedSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUtility.UnallocatedSQLite>(sql));
                            }
                            catch (Exception sqlex)
                            {
                                ourviewmodel.errorMessage = sqlex.Message;
                                return false;
                            }
                            if (DBServer)
                            {
                                ourviewmodel.recordCount = 0;
                                foreach (SmartUtility.UnallocatedSQLite sqliteRow in utilityviewmodel.Hezbollah.sqlite_unallocatedList)
                                {
                                    if (ourviewmodel.recordCount > 0)
                                    {
                                        ourviewmodel.temp_stream.Append(SmartParametersV2016.unitSeparator);
                                    }
                                    ourviewmodel.temp_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUtility.UnallocatedSQLite>(sqliteRow, false, SmartParametersV2016.fieldSeparator));
                                    ourviewmodel.recordCount++;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            utilityviewmodel.Hezbollah.sqlite_unallocatedList = Add_Secondary<SmartUtility.UnallocatedSQLite>(ourviewmodel, username, groupname);
                        }
                        if (utilityviewmodel.Hezbollah.sqlite_unallocatedList.Count > 0)
                        {
                            foreach (SmartUtility.UnallocatedSQLite sqlite_row in utilityviewmodel.Hezbollah.sqlite_unallocatedList)
                            {
                                key_details = sqlite_row.KEY_DETAILS;
                                if (string.IsNullOrEmpty(DEK))
                                {
                                    key_details_unencrypted = key_details;
                                }
                                else
                                {
                                    key_details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY1, derived), key_details, false);
                                    if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                    {
                                        return false;
                                    }
                                }
                                string[] key_details_fields = key_details_unencrypted.Split(SmartParametersV2016.fieldSeparator);

                                if (key_details_fields.Length == 5)
                                {
                                    string details_unencrypted;
                                    string details;

                                    details = sqlite_row.DETAILS;

                                    if (string.IsNullOrEmpty(DEK))
                                    {
                                        details_unencrypted = details;
                                    }
                                    else
                                    {
                                        details_unencrypted = SmartEncryptionV2016.DoTheBiz("", BuildEncryptKey(DEK, sqlite_row.RANDOMKEY2, derived), details, false);
                                        if (!string.IsNullOrEmpty(utilityviewmodel.errorMessage))
                                        {
                                            return false;
                                        }
                                    }
                                    string[] details_fields = details_unencrypted.Split(SmartParametersV2016.fieldSeparator);
                                    if (details_fields.Length == 3)
                                    {
                                        SmartUtility.Unallocated unallocated_row = new SmartUtility.Unallocated()
                                        {
                                            USERNAME = sqlite_row.USERNAME,
                                            CUBEFACE_CODE = Convert.ToChar(sqlite_row.CUBEFACE_CODE),
                                            SUPPLIER_CODE = sqlite_row.SUPPLIER_CODE,
                                            BRAND_CODE = sqlite_row.BRAND_CODE,
                                            ACCOUNT_CREATED = sqlite_row.ACCOUNT_CREATED,
                                            ACCOUNT_NO = key_details_fields[0],
                                            STATEMENT_ID = key_details_fields[1],
                                            BILL_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[2]),
                                            PAYMENT_DATE = SmartTimeV2016.ConvertDateTime(key_details_fields[3]),
                                            PAYMENT_ITEM = Convert.ToInt16(key_details_fields[4]),
                                            RANDOMKEY1 = sqlite_row.RANDOMKEY1,
                                            PAYMENT_CODE = Convert.ToInt16(details_fields[0]),
                                            PAYMENT_AMOUNT = Convert.ToInt32(details_fields[1]),
                                            PAYMENT_BALANCE = Convert.ToInt32(details_fields[2]),
                                            RANDOMKEY2 = sqlite_row.RANDOMKEY2,
                                            Updated = false
                                        };
                                        utilityviewmodel.Hezbollah.unallocatedList.Add(unallocated_row);
                                    }
                                }
                            }
                        }
                        utilityviewmodel.Hezbollah.sqlite_unallocatedList.Clear();
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        internal static bool UnLoadCommonUtility(UtilityViewModel utilityviewmodel,
                                                //string schema_name,
                                                string table_name,
                                                string username)

        {
            // Cannot unload ourviewmodel.UserName btw 
            bool status = false;
            if (string.IsNullOrEmpty(username))
            {
                utilityviewmodel.errorMessage = "Username is empty";
                return status;
            }
            else
            {
                utilityviewmodel.errorMessage = "";

                switch (table_name)
                {
                    case "AccChargesCredits":
                        if (utilityviewmodel.Hezbollah.account_charges_creditsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.account_charges_creditsList = utilityviewmodel.Hezbollah.account_charges_creditsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "Accounts":
                        // Unload Common Utility
                        if (utilityviewmodel.Hezbollah.utility_accountsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.utility_accountsList = utilityviewmodel.Hezbollah.utility_accountsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;                    
                    case "BankDetails":
                        if (utilityviewmodel.Hezbollah.utility_bankdetailsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.utility_bankdetailsList = utilityviewmodel.Hezbollah.utility_bankdetailsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "Bills":
                        if (utilityviewmodel.Hezbollah.billsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.billsList = utilityviewmodel.Hezbollah.billsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "BillsResource":
                        if (utilityviewmodel.Hezbollah.bills_resourceList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.bills_resourceList = utilityviewmodel.Hezbollah.bills_resourceList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "ECosts":
                        if (utilityviewmodel.Hezbollah.e_costsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.e_costsList = utilityviewmodel.Hezbollah.e_costsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "EDiscounts":
                        if (utilityviewmodel.Hezbollah.e_discountsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.e_discountsList = utilityviewmodel.Hezbollah.e_discountsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "EReadings":
                        if (utilityviewmodel.Hezbollah.e_readingsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.e_readingsList = utilityviewmodel.Hezbollah.e_readingsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "EStandingCharges":
                        if (utilityviewmodel.Hezbollah.e_standing_chargesList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.e_standing_chargesList = utilityviewmodel.Hezbollah.e_standing_chargesList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "EUnitCharges":
                        if (utilityviewmodel.Hezbollah.e_unit_chargesList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.e_unit_chargesList = utilityviewmodel.Hezbollah.e_unit_chargesList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "EUsage":
                        if (utilityviewmodel.Hezbollah.e_usageList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.e_usageList = utilityviewmodel.Hezbollah.e_usageList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "GCosts":
                        if (utilityviewmodel.Hezbollah.g_costsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.g_costsList = utilityviewmodel.Hezbollah.g_costsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "GDiscounts":
                        if (utilityviewmodel.Hezbollah.g_discountsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.g_discountsList = utilityviewmodel.Hezbollah.g_discountsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "GReadings":
                        if (utilityviewmodel.Hezbollah.g_readingsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.g_readingsList = utilityviewmodel.Hezbollah.g_readingsList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "GStandingCharges":
                        if (utilityviewmodel.Hezbollah.g_standing_chargesList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.g_standing_chargesList = utilityviewmodel.Hezbollah.g_standing_chargesList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "GUnitCharges":
                        if (utilityviewmodel.Hezbollah.g_unit_chargesList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.g_unit_chargesList = utilityviewmodel.Hezbollah.g_unit_chargesList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "GUsage":
                        if (utilityviewmodel.Hezbollah.g_usageList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.g_usageList = utilityviewmodel.Hezbollah.g_usageList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;

                    case "Logins":
                        if (utilityviewmodel.Hezbollah.utility_loginsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.utility_loginsList = utilityviewmodel.Hezbollah.utility_loginsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "Meters":
                        if (utilityviewmodel.Hezbollah.utility_metersList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.utility_metersList = utilityviewmodel.Hezbollah.utility_metersList
                                .Where(row => row.USERNAME != username)
                                .ToList();                            
                        }
                        break;
                    case "Payments":
                        if (utilityviewmodel.Hezbollah.paymentsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.paymentsList = utilityviewmodel.Hezbollah.paymentsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "Resources":
                        if (utilityviewmodel.Hezbollah.utility_resourcesList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.utility_resourcesList = utilityviewmodel.Hezbollah.utility_resourcesList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "ResourcesTypes":
                        if (utilityviewmodel.Hezbollah.utility_resources_typesList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.utility_resources_typesList = utilityviewmodel.Hezbollah.utility_resources_typesList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "SupChargesCredits":
                        if (utilityviewmodel.Hezbollah.supply_charges_creditsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.supply_charges_creditsList = utilityviewmodel.Hezbollah.supply_charges_creditsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "Switches":
                        if (utilityviewmodel.Hezbollah.utility_switchesList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.utility_switchesList = utilityviewmodel.Hezbollah.utility_switchesList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "TariffDetails":
                        if (utilityviewmodel.Hezbollah.tariff_detailsList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.tariff_detailsList = utilityviewmodel.Hezbollah.tariff_detailsList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    case "Unallocated":
                        if (utilityviewmodel.Hezbollah.unallocatedList.Count > 0)
                        {
                            utilityviewmodel.Hezbollah.unallocatedList = utilityviewmodel.Hezbollah.unallocatedList
                                .Where(row => row.USERNAME != username)
                                .ToList();
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        internal static string GenerateInsertStore(MainViewModel ourviewmodel,
                                            string schema_name,
                                            string table_name)
        {

            string sql = "INSERT INTO [" + schema_name + "." + table_name + "] " + "(";

            List<SmartData.SQLiteFields> abc = SmartSpikeV2017.Lookup_SQLiteFields(ourviewmodel, schema_name, table_name);
            int count = 0;

            foreach (SmartData.SQLiteFields field in abc)
            {
                if (count > 0)
                {
                    sql += ",";
                }
                sql += field.FIELD_NAME;
                count++;
            }
            sql += ") ";
            sql += "VALUES ";            
            return sql;
        }
        internal static string GenerateInsert<T1>(MainViewModel ourviewmodel,
                                            string schema_name,
                                            string table_name,
                                            T1 sqlite_row,
                                            FieldInfo[] myFields)
        {

            string sql = "INSERT INTO [" + schema_name + "." + table_name + "] " + "(";

            List<SmartData.SQLiteFields> abc = SmartSpikeV2017.Lookup_SQLiteFields(ourviewmodel, schema_name, table_name);
            int index = 0;
            int count = 0;

            foreach (SmartData.SQLiteFields field in abc)
                {
                    if (count > 0)
                    {
                        sql += ",";
                    }
                    sql += field.FIELD_NAME;
                    count++;
                }
                sql += ") ";
                sql += "VALUES ";
                sql += "(";
                foreach (SmartData.SQLiteFields field in abc)
                {
                    if (index > 0)
                    {
                        sql += ",";
                    }
                    switch (myFields[index].FieldType.FullName)
                    {
                        case "System.DateTime":
                            string icow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                            sql += "'" + SmartTimeV2016.ConvertDateTime(icow).ToString(SmartParametersV2016.sqliteformat) + "'";
                            break;
                        case "System.Boolean":
                            sql += myFields[index].GetValue(sqlite_row).ToString().ToLower();
                            break;
                        case "System.String":
                            sql += "'" + myFields[index].GetValue(sqlite_row) + "'";
                            break;
                        default:
                            sql += myFields[index].GetValue(sqlite_row);
                            break;
                    }
                    index++;
                }
            sql += ");";
            return sql;
        }

        internal static string GenerateUpdate<T>(MainViewModel ourviewmodel,
                                            string schema_name,
                                            string table_name,
                                            T sqlite_row,
                                            FieldInfo[] myFields)
        {
            string sql = "UPDATE " + "[" + schema_name + "." + table_name + "] SET ";
            string sql_where = " WHERE ";

            List<SmartData.SQLiteFields> abc = SmartSpikeV2017.Lookup_SQLiteFields(ourviewmodel, schema_name, table_name);
            int index = 0;
            int key_count = 0;
            int nonkey_count = 0;
            foreach (SmartData.SQLiteFields field in abc)
                {
                    switch (field.KEY_FIELD)
                    {
                        case 'Y':
                            if (key_count > 0)
                            {
                                sql_where += " AND ";
                            }
                            sql_where += field.FIELD_NAME + "=";
                            switch (myFields[index].FieldType.FullName)
                            {
                                case "System.DateTime":
                                    // This SHOULD be in the right format ..
                                    string dcow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                                    sql_where += "'" + SmartTimeV2016.ConvertDateTime(dcow).ToString(SmartParametersV2016.sqliteformat) + "'";
                                    break;
                                case "System.Boolean":
                                    sql_where += myFields[index].GetValue(sqlite_row).ToString().ToLower();
                                    break;
                                case "System.String":
                                    sql_where += "'" + myFields[index].GetValue(sqlite_row) + "'";
                                    break;
                                case "System.Char":
                                    sql_where += "'" + myFields[index].GetValue(sqlite_row) + "'";
                                    break;
                                default:
                                    sql_where += myFields[index].GetValue(sqlite_row);
                                    break;

                            }
                            key_count++;
                            break;
                        case 'N':
                            if (nonkey_count > 0)
                            {
                                sql += ",";
                            }
                            sql += field.FIELD_NAME + "=";
                            switch (myFields[index].FieldType.FullName)
                            {
                                case "System.DateTime":
                                    // This SHOULD be in the right format ..
                                    string icow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                                    sql += "'" + SmartTimeV2016.ConvertDateTime(icow).ToString(SmartParametersV2016.sqliteformat) + "'";
                                    break;
                                case "System.Boolean":
                                    sql += myFields[index].GetValue(sqlite_row).ToString().ToLower();
                                    break;
                                case "System.String":
                                    sql += "'" + myFields[index].GetValue(sqlite_row) + "'";
                                    break;
                                case "System.Char":
                                    sql += "'" + myFields[index].GetValue(sqlite_row) + "'";
                                    break;
                                default:
                                    sql += myFields[index].GetValue(sqlite_row);
                                    break;

                            }
                            nonkey_count++;
                            break;
                        default:
                            break;
                    }
                    index++;
                }
            sql += sql_where + ";";
            return sql;
        }

        internal static string GenerateDelete<T>(MainViewModel ourviewmodel,
                                            string schema_name,
                                            string table_name,
                                            T sqlite_row,
                                            FieldInfo[] myFields)
        {
            string sql = "DELETE " + "FROM [" + schema_name + "." + table_name + "]";
            string sql_where = " WHERE ";

            List<SmartData.SQLiteFields> abc = SmartSpikeV2017.Lookup_SQLiteFields(ourviewmodel, schema_name, table_name);
            int index = 0;
            int key_count = 0;
            //int nonkey_count = 0;
            foreach (SmartData.SQLiteFields field in abc)
                {
                    switch (field.KEY_FIELD)
                    {
                        case 'Y':
                            if (key_count > 0)
                            {
                                sql_where += " AND ";
                            }
                            sql_where += field.FIELD_NAME + "=";
                            switch (myFields[index].FieldType.FullName)
                            {
                                case "System.DateTime":
                                    // This SHOULD be in the right format ..
                                    string dcow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                                    sql_where += "'" + SmartTimeV2016.ConvertDateTime(dcow).ToString(SmartParametersV2016.sqliteformat) + "'";
                                    break;
                                case "System.Boolean":
                                    sql_where += myFields[index].GetValue(sqlite_row).ToString().ToLower();
                                    break;
                                case "System.String":
                                    sql_where += "'" + myFields[index].GetValue(sqlite_row) + "'";
                                    break;
                                case "System.Char":
                                    sql_where += "'" + myFields[index].GetValue(sqlite_row) + "'";
                                    break;
                                default:
                                    sql_where += myFields[index].GetValue(sqlite_row);
                                    break;

                            }
                            key_count++;
                            break;
                        //case 'N':
                        //    if (nonkey_count > 0)
                        //    {
                        //        sql += ",";
                        //    }
                        //    sql += field.FIELD_NAME + "=";
                        //    switch (myFields[index].FieldType.FullName)
                        //    {
                        //        case "System.DateTime":
                        //            // This SHOULD be in the right format ..
                        //            string icow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                        //            sql += "'" + SmartTimeV2016.ConvertDateTime(icow).ToString(SmartParametersV2016.sqliteformat) + "'";
                        //            break;
                        //        case "System.Boolean":
                        //            sql += myFields[index].GetValue(sqlite_row).ToString().ToLower();
                        //            break;
                        //        case "System.String":
                        //            sql += "'" + myFields[index].GetValue(sqlite_row) + "'";
                        //            break;
                        //        case "System.Char":
                        //            sql += "'" + myFields[index].GetValue(sqlite_row) + "'";
                        //            break;
                        //        default:
                        //            sql += myFields[index].GetValue(sqlite_row);
                        //            break;

                        //    }
                        //    nonkey_count++;
                        //    break;
                        default:
                            break;
                    }
                    index++;
                }
            sql += sql_where + ";";
            return sql;
        }

        internal static List<T1> StoreX<T1, T2>(List<T2> smartswitchList) where T1 : new()
        {
            List<T1> list_records = new List<T1>();

            FieldInfo[] myFields = typeof(T2).GetFields(SmartParametersV2016.bindingFlags);

                FieldInfo[] recFields = typeof(T1).GetFields(SmartParametersV2016.bindingFlags);
                if (recFields.Length == myFields.Length)
                {
                    foreach (T2 sqlite_row in smartswitchList)
                    {
                        T1 target_row = Activator.CreateInstance<T1>();

                        for (int index = 0; index < myFields.Length; index++)
                        {
                            switch (myFields[index].FieldType.FullName)
                            {
                                case "System.Char":
                                    recFields[index].SetValue(target_row, myFields[index].GetValue(sqlite_row).ToString());
                                    break;
                                default:
                                    recFields[index].SetValue(target_row, myFields[index].GetValue(sqlite_row));
                                    break;
                            }
                        }
                        list_records.Add(target_row);
                    }
                }
            return list_records;
        }

        internal static async Task<bool> InsertSQLite<T>(MainViewModel ourviewmodel,
                                                            string schema_name,
                                                            string table_name,
                                                            T sqlite_row)
        {
            // What a piece of shit this bollocks is. This binding
            // wont find my private or internal fields so I have
            // to make them all Public !!! What BOLLOCKS!!!!
            // I cannot have multiple bindings or I get IL2090 error
            // Fucking Chimps!!  ANyway I've checked all the tables
            // and only Utility Logins had the updated field as 'internal'
            // and I've changed that so I THINK I'm good to go ...
            FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);
            
            string sql_insert_con = GenerateInsert(ourviewmodel,
                                                        schema_name,
                                                        table_name,
                                                        sqlite_row,
                                                        myFields);


            if (!await ExecuteSQLite(ourviewmodel, sql_insert_con, em => ourviewmodel.errorMessage = em))
            {
                return false;
            }
            return true;
        }
        internal static async Task<bool> UpdateSQLite<T>(MainViewModel ourviewmodel,
                                                            string schema_name,
                                                            string table_name,
                                                            T sqlite_row)
        {
            FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);

            string sql_update_con = GenerateUpdate(ourviewmodel,
                                                        schema_name,
                                                        table_name,
                                                        sqlite_row,
                                                        myFields);


            if (!await ExecuteSQLite(ourviewmodel, sql_update_con, em => ourviewmodel.errorMessage = em))
            {
                return false;
            }            
            return true;
        }

        internal static async Task<bool> DeleteSQLite<T>(MainViewModel ourviewmodel,
                                                            string schema_name,
                                                            string table_name,
                                                            T sqlite_row)
        {
            FieldInfo[] myFields = typeof(T).GetFields(SmartParametersV2016.bindingFlags);

            string sql_delete_con = GenerateDelete(ourviewmodel,
                                                        schema_name,
                                                        table_name,
                                                        sqlite_row,
                                                        myFields);


            if (!await ExecuteSQLite(ourviewmodel, sql_delete_con, em => ourviewmodel.errorMessage = em))
            {
                return false;
            }
            return true;
        }
        internal static List<T1> RetrieveNotEncrypted<T1, T2>(List<T2> sqliteList) where T1 : new()
        {
            List<T1> list_records = new List<T1>();

           FieldInfo[] myFields = typeof(T2).GetFields(SmartParametersV2016.bindingFlags);

                FieldInfo[] recFields = typeof(T1).GetFields(SmartParametersV2016.bindingFlags);
                if (recFields.Length == myFields.Length)
                {
                    foreach (T2 sqlite_row in sqliteList)
                    {
                        T1 target_row = Activator.CreateInstance<T1>();

                        for (int index = 0; index < recFields.Length; index++)
                        {
                            switch (recFields[index].FieldType.FullName)
                            {
                                case "System.Char":
                                    recFields[index].SetValue(target_row, Convert.ToChar(myFields[index].GetValue(sqlite_row).ToString()));
                                    break;
                                default:
                                    recFields[index].SetValue(target_row, myFields[index].GetValue(sqlite_row));
                                    break;
                            }
                        }
                        list_records.Add(target_row);
                    }
                }
            return list_records;
        }

        internal static async Task<bool> ExecuteSQLite(MainViewModel ourviewmodel,
                                                        string sqlOperation,
                                                        Action<string> set_errorMessage)
        {
            try
            {
                int res2 = await SmartSwitchDatabase.ExecuteSmartSwitchTableAsync(ourviewmodel.sqliteDatabase, sqlOperation);
            }
            catch (Exception sqlex)
            {
                set_errorMessage(sqlex.Message);
                return false;
            }
            return true;
        }
    }
}