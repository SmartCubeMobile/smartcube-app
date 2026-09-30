using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using SmartCubeMobile;
using SQLite;

namespace SmartDBServer
{
    public class ServerModel
    {
        internal SQLiteAsyncConnection sqliteDatabase = new SQLiteAsyncConnection("");

        internal SQLite.SQLiteOpenFlags CreateFlags =
                    // Open the database in read/write mode
                    SQLite.SQLiteOpenFlags.ReadWrite |
                    // Create the database if it doesn't exist
                    SQLite.SQLiteOpenFlags.Create;// Enable multi-threaded database access

        internal SQLite.SQLiteOpenFlags ReadWriteFlags =
                                    // Open the database in read/write mode 
                                    SQLite.SQLiteOpenFlags.ReadWrite;

        internal List<RequestVerificationToken> tokensList = new();

        internal RequestVerificationToken antitoken = new();

        internal List<SmartData.Currencies> currencies_list = new();

        internal List<SmartUsers.ExchangeRates> ERList = new();

        //internal List<SmartData.SQLiteTables> sqlitetables = new();
        //internal List<SmartData.SQLiteSchemas> sqliteschemas = new(); 
        //internal List<SmartData.SQLiteFields> sqlitefields = new();

        internal string UserName = "Remote";

        public string DataBasePath
        {
            get
            {
                string basePath = "";
                // DON'T forget this!!!! (You dork)
#if !CRYPTOS
                switch (UserName)
                {
                    case SmartParametersV2016.WPF:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    case SmartParametersV2016.UWP:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    case SmartParametersV2016.WinUI:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                    case SmartParametersV2016.Android:
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        //if (!DependencyService.Get<IDirectory>().GetDirectory(rf basePath))
                        //{
                        //    return "";
                        //}
                        break;
                    default:
                        // Winforms?
                        basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        break;
                }
#endif
                return basePath;
            }
        }

        
        // All of this stuff below can be commented out ...
        // (We'll see, eh, Ray???)
        internal string errorMessage = "";

        internal StringBuilder data_stream = new StringBuilder();
        
        internal int record_count = 0;
        internal int tableCount = 0;

        internal DateTime lastdate = SmartParametersV2016.defaultDate;

        internal Petulant databaseItem = new();
        internal int databaseIndex = 0;
        internal string schemaName = "";
        internal int tableIndex = 0;
        internal string tableName = string.Empty;

        internal int smartswitchIndex = -1;
        internal int consumersschemaIndex = -1;
        internal int consumerstableIndex = 1;
        internal string withdrawnDate = "";
        internal string updatedDate = "";
        internal int[] refreshArray = new int[1];// It is

        internal string outputResult = "";
        internal Uppance field;

        internal bool itsThere = false;

        internal List<Petulant> databases = new List<Petulant>();

        internal System.Timers.Timer timerClock = new(SmartParametersV2016.ECBCheckTimer);
        internal string tracePath = "";

        // chatGPT says decimal is better for exchange rates
        // than double, so I'm sticking with DECIMALs!!
        internal DateTime lastGoodDate = SmartParametersV2016.defaultDate;
        internal short lastECB = 0;
        internal decimal lastGBP = 0;
        internal decimal lastEUR = 0;
        internal decimal lastUSD = 0;
        //internal decimal lastCAN = 0;
        internal decimal lastJPY = 0;
    }
}
