using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace SmartCubeMobile
{
    // Its across ALL of them!  That's the point!!!
    // THE CHIMPS HAVE BEEN DEFEATED!!!!
    //#if WINFORMS || WPF  || WINUI
    public class SmartSwitchDatabase
    {
        public static bool OpenDatabase(MainViewModel ourviewmodel, string dbpath, SQLite.SQLiteOpenFlags flags)
        {
            // Now, the way this works 
            // for Android devices, is that the SQLite
            // database is created in INTERNAL STORAGE so it exists whilst the
            // Application is installed i.e. if the SmartSwitch application is Uninstalled
            // ========================
            // the the SQLite database is deleted along with it!!! (Its all bound up in the
            // APK or whatever the fuck that is)
            // So when SmartSwitch is installed and run THE FIRST TIME, the
            // SQLite database IS created, but when it is run on subsequent
            // occasions the SQLite database ISN'T created - the one created
            // on the first execution is used!
            try
            {
                // Optional: check if DB already exists
                //bool dbExists = File.Exists(dbpath);

                // Unbelieveably (!!!) if the ACTUAL database doesn't exist at this point -
                // then this statement **DOES NOT** throw an Error !!!!
                // NONE of the catch(es) below work!  None of them!  What a crock of shit
                // this stuff is - its complete BOLLOCKS

                ourviewmodel.sqliteDatabase = new SQLiteAsyncConnection(dbpath, flags, false);
                //ourviewmodel.sqliteDatabase.ExecuteAsync(@"
                //        PRAGMA journal_mode = WAL;
                //        PRAGMA synchronous = NORMAL;           
                //        ");
                return true;
            }
            catch (SQLite.SQLiteException sqlite_ex)
            {
                ourviewmodel.errorMessage = sqlite_ex.Message;
            }
            catch (Exception ex)
            {
                // How do we get THIS back into the main program??
                ourviewmodel.errorMessage = ex.Message;
            }
            return false;
        }

        internal static async Task<List<TableName>> GetTableNamesAsync(MainViewModel ourviewmodel,
                                                                        SQLiteAsyncConnection sqlite_database,
                                                                        string sql)
        {
            List<TableName> tablenamesList = new List<TableName>();
            try
            {
                // Believeably (!!!) if the ACTUAL database doesn't exist at this point -
                // then this statement **DOES** throw an Error !!!!
                tablenamesList = await sqlite_database.QueryAsync<TableName>(sql);
            }
            catch (Exception ex)
            {
                ourviewmodel.errorMessage = ex.Message;
            }
            return tablenamesList;
        }

        internal static async Task<int> ExecuteSmartSwitchTableAsync(SQLiteAsyncConnection sqlite_database, string sql)
        {
            return await sqlite_database.ExecuteAsync(sql);
        }

        internal static async Task<List<SQLite.SQLiteConnection.ColumnInfo>> ExecuteSmartSwitchTableInfoAsync(SQLiteAsyncConnection sqlite_database, string table_name)
        {
            return await sqlite_database.GetTableInfoAsync(table_name);
        }
    }
}