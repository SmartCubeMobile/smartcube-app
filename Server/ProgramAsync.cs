//  Insofar as I can deduce, UTC and GMT display the same time, except that GMT
//  diplays UTC + 1 in the summer, as GMT is just now a time zone.
//  So .. when I poll pool.nt.org, I am actually being returned UTC time all along
//  although I have been displaying it as 'GMT'.  Wrong.  So let me find all the 'GMT'
//  monickers in this program and convert them to UTC ...
//
// There is no Find_All_Schemas!!!  We only need Find_All_Databases in master
// because every database has its own set of schemas so ... find the databases
// and you can find all the schemas.
// At some point in the future we need to create this in master if its not there when we start

//USE[master]
//GO
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
///****** Object:  StoredProcedure [dbo].[usp_FIND_ALL_DATABASES]    Script Date: 16/07/2018 23:44:03 ******/
//SET ANSI_NULLS ON
//GO
//SET QUOTED_IDENTIFIER ON
//GO


//-- =============================================
//-- Author:		Ray Chapman
//-- Create date:   10-Jul-2016
//-- .Net Core V5:  02-Nov-2021
//-- Description:	Find All Databases
//--
//-- 16-Jul-2018  Modified to remove the su.sid = null part as I 
//--              gave each of the DBs an owner(!!) in order to overcome
//--              the error this shit kindly gave me when I tried to
//--              create database diagrams. Usual Microshit bollocks...
//
// We never do this Encryption Bollocks here now
// DBServer *NEVER* encrypts anything ... if its
// not encrypted by THE USER, then it doesn't get encrypted
// and therefor never needs decrypting
//
//-- =============================================
//CREATE PROCEDURE [dbo].[usp_FIND_ALL_DATABASES]
//	-- Add the parameters for the stored procedure here

//    @param1 nvarchar (MAX) --  Use SMART%   here
//AS
//BEGIN
//	-- SET NOCOUNT ON added to prevent extra result sets from
//	-- interfering with SELECT statements.

//    SET NOCOUNT ON;

//    -- Insert statements for procedure here

//    SELECT db.[name] FROM[master].[sys].[databases] db
//     LEFT OUTER JOIN[master].[sys].[sysusers] su on su.sid = db.owner_sid
//     WHERE db.is_broker_enabled = 0 and db.[name]
//     LIKE '' + @param1 + ''

//     ORDER BY db.[name]
//END

//          The way it works ....
//          POST_CONDITIONS_GROUPS
//          Has a Supplier (5) a resource (E) and a Group Code (1) which defines the
//          resource for the Group Code of the Supplier
//
//          POST_CONDITIONS_CODES
//          Has a Supplier (5) a Tariff (2) and a Group Code(1) which defines the
//          Group Code for the Tariff for the Supplier
//
//          POST_CONDITIONS_GROUPINGS
//          Has a Supplier (5) a Group Code (1) and a Selection Code (1) which defines
//          the Selections for the Group for the Supplier 

//          So ... given a Supplier, a Tariff and a Resource
//          Look up the SUPPLIERS
//            Look up the TARIFFS
//                Look up the POST_CONDITIONS_GROUPS
//                      (cross PCG.RESOURCE_CODE with TARIFFS.RESOURCE_CODE)
//                    Look up POST_CONDITIONS_CODES
//                       (cross TC.TARIFF with TARIFFS.TARIFF and TC.GROUP with PCG.Group
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SQLite;
//using System.Data.SqlClient;        // For SQLServer2014 Connection (but the 'instance' is still SQLEXPRESS ... =:-O((
using System.Diagnostics;           // For trace amongst others
using System.Dynamic;
using System.IO;
//using System.IO.Pipelines;
using System.IO.Pipes;              // For Pipes
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using SmartCubeMobile;
using SQLite;
using Windows.Media.Protection.PlayReady;

namespace SmartDBServer
{
    //public class Listener   // Just in for convenience - never used in program but by DBServer
    //{
    //    internal string
    //        USERNAME;
    //    internal DateTime
    //        DATE;
    //    internal short
    //        BRAND_CODE;
    //    internal short
    //        SUPPLIER_CODE;'
    //    internal string
    //        ROUTINE,
    //        MESSAGE;
    //}

    [SupportedOSPlatform("windows")]

    public class Program
    {
        // 29th October 2025
        // Completely re-written to eliminate SmartSwitch database
        // from SQL Server as a Remote and replace it with SQLite as a Remote
        // Fingers scrossed it works ... =:-{   It DID, Ray! Well done!!

        //
        // The mighty codes:
        //  reload =    "R"
        //  logfile =   "L"
        //  execute =   "P"        
        //  select =    "S" REMOTE (Users (only Consumers), Profile, Finance, Utility)
        //  select =    "S" LOCAL (Users (only Consumers))
        //  insert =    "I"
        //  update =    "U"
        //  delete =    "D"
        //  table =     "T"
        //  configure = "C"
        //  GUI       = "G"
        //  singles   = "Z"
        //  exchange  = 'X'
        //  
        
        internal static ConsoleTraceListener ctl;
        internal static SqlConnection master_connection;
        internal static TextWriterTraceListener tr1;

        internal static TimeSpan utcOffset;

        // Also utcTime is defined as 'static' here at the top of the program;
        // don't ask me why - you can even call me a twat(!) but I feel
        // safer with it defined as being a static rather than passed into this
        // routine as a local from the previous calling routine .... A great
        // deal ('The Defeat Of The Chimps') depends on this date being EXACTLY
        // the same (down to the millisecond?) with that sent back to the calling
        // SmartSwitch GUI
        internal static DateTime utcTime = DateTime.MinValue;
        // Check out Stephen Cleary in the Main.cs program as to why.how we do this
        // He says console programs work differently for async voids, but I wanted to eliminate
        // them entirely from SmartSwitch (rxcept on Event Handlers where SC says they're ok ...)
        internal static async Task Main(string[] args)
        {
            // All this bollocks - just to get tid os 'static's!!!!
            Program program = new Program();
            await program.DoSomething(args, program); // ✅ Works
            return;
        }
        internal async Task DoSomething(string[] args, Program program)
        {
            ServerModel servermodel = new ServerModel();
            MainViewModel ourviewmodel = new MainViewModel();
            FinanceViewModel financeviewmodel = new FinanceViewModel();
            UtilityViewModel utilityviewmodel = new UtilityViewModel();

            string client = SmartParametersV2016.pipename;

            
            if (args.Length == 3)
            {
                client = args[0];
                servermodel.withdrawnDate = args[1];
            }
            
#if DEVELOPMENT
            int width = 128,
            height = 32;
            if (Environment.UserInteractive)
            {
                Console.SetWindowSize(width, height);
            }
#endif
            servermodel.tracePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Trace_Files(client, ref servermodel.tracePath);

            // This is ONLY for the PC on which SmartDBServer runs!  It has nothing
            // to do with any of the PCs or devices which connect to it - they have
            // to figure out their OWN UTC and utcOffset!!!!
            string errorMessage = "";
            DateTime? networkTime = await SmartTimeV2016.GetNetworkTimeAsync();
            if (networkTime == null)
            {                
                OutputText(client, "Problem getting UTC ... no network perhaps? " + errorMessage, true);
            }
            else
            {
                    DateTime UTC = networkTime.Value;
                    // Our GMT time zone in the summer is 1 hour ahead of UTC
                    // So when UTC returns 20:00pm on 16th Oct, DateTime.Now - which
                    // is our PC's time and obeys British Summer Time, give 21:00pm
                    // So the offset in Summer in the UK is +1 hour.
                    // However, when the clocks go back then our PC's time should be
                    // the same as UTC. But if our PC is 10 seconds faster or 5 seconds
                    // slower than UTC, we still need an offset .. don't we? No, because
                    // UTC is UTC regardless of what our PC is set to.  That's the whole
                    // point of using UTC you dumb fucker ... it's PC independent
                    // But people want to see THEIR dates and times in SmartSwitch
                    // so we SHOW UTC + offset but we STORE UTC in the database ...
                    // But why do I still use an offset?  Its because its possible for
                    // some plonkers to ignore Windows automatic synchronization of time
                    // (it uses windows.time.bollocks or something) and set their own PC
                    // time.  DateTime.UtcNow looks up this dumb time and uses it.
                    // SO our offset based on something independent is still uselful.
                    // Ok, it might show 3 msecs difference ... but that proves it works, yes?

                    utcOffset = UTC - DateTime.UtcNow;

                // I think  I need to add the utcOffset in here to get the PROPER local time
                // Yes, because you can change the Local Time of this PC to something dumb
                // The utcOffset should be milliseconds btw
                DateTime localPCTime = DateTime.Now + utcOffset;

                string ffs = TimeZoneInfo.Local.IsDaylightSavingTime(localPCTime) ?
                                  TimeZoneInfo.Local.DaylightName : TimeZoneInfo.Local.StandardName;
                OutputText(client, "Local PC Time in " + ffs + " zone: " + localPCTime, true);
                OutputText(client, "UTC derived locally: " + TimeZoneInfo.ConvertTimeToUtc(localPCTime, TimeZoneInfo.Local), true);
                OutputText(client, "UTC derived network: " + UTC.ToString(SmartParametersV2016.defaultCulture) + SmartParametersV2016.operationSuccess.ToString(), true);
                OutputText(client, "UTC Offset: " + utcOffset, true);

                // Proof that irrespective of the current PC time, PROVIDED (!!) we add in the UTC Offset,
                // then the UnixMilliseconds always increases
                // No it doesn't!!! If you take away the OutputText you
                // can get the same UnixMilliseconds even adding in the offset
                // You found a better solution anyway, so I'm tsaking this stuff OUT!                
                //ulong prev_sequence_no = 0;
                //ulong sequence_no = 0;

                //for (int i = 0; i < 10; i++)
                //{
                //redo:
                //    sequence_no = (ulong)new DateTimeOffset(DateTime.UtcNow + utcOffset).ToUnixTimeMilliseconds();
                //    // Ensure uniqueness!
                //    if (prev_sequence_no == sequence_no)
                //    {
                //        goto redo;
                //    }
                //    OutputText(client, "Sequence_no " + sequence_no, true);
                //}                

                //var builder = new SqlConnectionStringBuilder(SmartParametersV2016.connectionString)
                //{
                //    InitialCatalog = "master"
                //};
                //master_connection = new SqlConnection(builder.ConnectionString);

                

                string machine_name = Environment.MachineName;
                // Try and get a connection in, just to the master for starters ...
                master_connection = new SqlConnection(SmartParametersV2016.connectionString.Replace("Database=", "Database=" + "master"));
                try
                {
                    master_connection.Open();
                }
                catch (InvalidOperationException exception)
                {
                    OutputError(client, exception.Message, machine_name, "master", master_connection.ConnectionString);
                }
                catch (SqlException exception)
                {
                    OutputError(client, exception.Message, machine_name, "master", master_connection.ConnectionString);
                }
                catch (ConfigurationErrorsException exception)
                {
                    OutputError(client, exception.Message, machine_name, "master", master_connection.ConnectionString);
                }
                if (master_connection.State == ConnectionState.Open)
                {
                    servermodel.errorMessage = string.Empty;
                    servermodel.databases = SmartProgramV2016.FindAllDatabases(servermodel, master_connection, client, SmartParametersV2016.keystone, program);
                    // Close down the master connection
                    master_connection.Close();
                    if (!string.IsNullOrEmpty(servermodel.errorMessage) ||
                        (servermodel.databases.Count == 0))
                    {
                        if (string.IsNullOrEmpty(servermodel.errorMessage))
                        {
                            servermodel.errorMessage = "No " + SmartParametersV2016.keystone + " databases found (perhaps FIND_ALL_DATABASES is missing?)";
                        }
                        OutputText(client, servermodel.errorMessage, true);
                    }
                    else
                    {
                        // Try and attach to the all the databases we need - give up after
                        // 10 attempts with increasing time between attempts
                        // EVEN IF this fails, then the Windows Task Scheduler SHOULD
                        // restart and we go through this all over again ..
                        // EVEN IF the Windows Task Scheduler has *not* been configured,
                        // then the retries below should be sufficient (fingers crossed)
                        // to get us going ...

                        List<string> allSchemas = new List<string>();
                        if (!Sort_Out_Databases(servermodel, client, machine_name, allSchemas))
                        {
                            OutputText(client, "CLOSING DOWN ...", true);
                            // Give up - one or both of them couldn't be opened
                            Environment.Exit(0);
                        }
                        else
                        {
                            // Sort the dbs by their ORDINAL
                            servermodel.databases = new List<Petulant>(from DB in servermodel.databases
                                                           orderby DB.ordinal ascending
                                                           select DB);


                            // DON'T change this Withdrawn Date Format!!!!! (Or SQLREADER falls over!!)
                            servermodel.withdrawnDate = DateTimeNow(utcOffset).ToString(SmartParametersV2016.yymmddShortFormat, SmartParametersV2016.defaultCulture);
                            OutputText(client, "Withdrawn date: " + servermodel.withdrawnDate, true, ConsoleColor.Yellow);

                            // Insert the extracted Schemas into a real table
                            string sql_delete = "DELETE FROM [SmartData].[SCHEMAS];";
                            using SqlCommand sql_command = new()
                            {
                                Connection = servermodel.databases[0].connection,
                                CommandText = sql_delete,
                                CommandTimeout = SmartParametersV2016.commandTimeout
                            };
                            int record_count = sql_command.ExecuteNonQuery();
                            OutputText(client, "Deleted " + record_count.ToString() + " schemas from Table SCHEMAS", true);

                            List<string> distinctSchemas = allSchemas.Distinct().ToList();
                            int indexY = 0;

                            try
                            {
                                foreach (string schema_name in distinctSchemas)
                                {
                                    string sql_insert = "INSERT INTO [SmartData].[SCHEMAS] ";
                                    sql_insert += "([INDEX],[SCHEMA_NAME],[Created],[Updated],[Status_Flag],[Deactivated])";
                                    sql_insert += " VALUES( " + indexY.ToString() + "," +
                                                    "'" + schema_name + "'," +
                                                    "'" + DateTime.Now.ToString(SmartParametersV2016.sqliteformat) + "'," +
                                                    "'" + DateTime.Now.ToString(SmartParametersV2016.sqliteformat) + "'," +
                                                    "'', " +
                                                    "'" + SmartParametersV2016.defaultDates + "');";

                                    // Stop injection
                                    //string sql = "INSERT INTO [SmartData].[SCHEMAS] " +
                                    // "([INDEX],[SCHEMA_NAME],[Created],[Updated],[Status_Flag],[Deactivated]) " +
                                    // "VALUES (@Index, @SchemaName, @Created, @Updated, @StatusFlag, @Deactivated);";

                                    //using SqlCommand cmd = new SqlCommand(sql, databases[0].connection);
                                    //cmd.Parameters.AddWithValue("@Index", index);
                                    //cmd.Parameters.AddWithValue("@SchemaName", schema_name);
                                    //...
                                    //cmd.ExecuteNonQuery();

                                    using SqlCommand sql_command1 = new()
                                    {
                                        Connection = servermodel.databases[0].connection,
                                        CommandText = sql_insert,
                                        CommandTimeout = SmartParametersV2016.commandTimeout
                                    };
                                    record_count = sql_command1.ExecuteNonQuery();
                                    OutputText(client, "Inserted " + schema_name + " schema into Table SCHEMAS", true);
                                    indexY++;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                        int[] pads = new int[5] { 19, 31, 4, 15, 7 };
                        // Heading
                        //OutputText("Client".PadRight(pads[0]) +
                        //                    "Info".PadRight(pads[1]) +
                        //                    "Op".PadRight(pads[2]) +
                        //                    "Response".PadRight(pads[3]) + // Response is at column 47
                        //                    "Status".PadRight(pads[4]) +
                        //                    "Message", true);

                        // This could be an array
                        servermodel.refreshArray = new int[servermodel.databases.Count]; // It is
                        for (int i = 0; i < servermodel.databases.Count; i++)
                        {
                            servermodel.refreshArray[i] = -1;
                        }

                        await Sort_Out_Other_Shit(servermodel,
                                            client,
                                            servermodel.currencies_list,
                                            ourviewmodel.sqlitetablesList,
                                            ourviewmodel.sqliteschemasList,
                                            ourviewmodel.sqlitefieldsList);
                        servermodel.smartswitchIndex = -1;  // SQLite
                        servermodel.consumersschemaIndex = 0;
                        servermodel.consumerstableIndex = 0;
                        
                        //foreach (Petulant databaseItem in servermodel.databases)
                        //{
                        //    if (databaseItem.databaseName == SmartParametersV2016.MainDatabase)
                        //    {
                        //        foreach (Cow tableItem in databaseItem.tables)
                        //        {
                        //            if (tableItem.schema_name == "SmartUsers" &&
                        //                 tableItem.tableName == "Consumers")
                        //            {
                        //                foreach (Drivel schema_item in databaseItem.schemas)
                        //                {
                        //                    if (schema_item.schema_name == tableItem.schema_name)
                        //                    {
                        //                        break;
                        //                    }
                        //                    servermodel.consumersschemaIndex++;
                        //                }
                        //                break;
                        //            }
                        //            servermodel.consumerstableIndex++;
                        //        }
                        //        break;
                        //    }
                        //    servermodel.smartswitchIndex++;
                        //}


                        OutputText(client, "SmartUsers.Consumers " +
                                        servermodel.smartswitchIndex.ToString() + " " +
                                        servermodel.consumersschemaIndex.ToString() + " " +
                                        servermodel.consumerstableIndex.ToString(), true);

                        if (!await CheckSQLiteNew(ourviewmodel,
                                                    servermodel))
                        {
                            Console.Write("SQLite creation problem");
                        }
                        servermodel.tokensList = new();
                        OutputText(client, "Tokens list created", true);

                        servermodel.timerClock.Elapsed += new System.Timers.ElapsedEventHandler(async (s, e) => await OnCheckECBEvent(servermodel,
                                                                                    e,
                        
                                                                                    client,
                                                                                    servermodel.timerClock));


                        TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

                        // I think  I need to add the utcOffset in here to get the PROPER local time
                        // Yes, because its possible to set the Local PC time to something stupid
                        // The utcOffset should be milliseconds btw
                        DateTime cetTime = TimeZoneInfo.ConvertTime(DateTime.Now + utcOffset, TimeZoneInfo.Local, tst);
                        if (!servermodel.timerClock.Enabled)
                        {
                            //string rays = tst.IsDaylightSavingTime(cetTime) ? tst.DaylightName : tst.StandardName;
                            //OutputText(client, "Time in " + rays + " zone: " + cetTime, true);
                            //Console.WriteLine("   UTC Time: {0}", TimeZoneInfo.ConvertTimeToUtc(tstTime, tst));
                            if ((cetTime.Hour * 60 + cetTime.Minute) >= 855) // 855 = 14*60 + 15 i.e. 14:15pm?
                            {
                                servermodel.timerClock.Enabled = true;
                                OutputText("pipe", "ECB list check event - enabled", true);
                            }
                        }
                        else
                        {
                            if ((cetTime.Hour * 60 + cetTime.Minute) >= 915) // 915 = 15*60 + 15
                            {
                                servermodel.timerClock.Enabled = false;
                                OutputText("pipe", "ECB list check event - disabled", true);
                            }
                        }



                        // Make the Pipe Security 'SYSTEM'  NO!!  Make it DefaultAppPool as well!!!
                        //PipeSecurity pipe_security = new();
                        ////Is this okay to do?  SYSTEM Read/Write?
                        //string[] identities = "SYSTEM, IIS APPPOOL\\DefaultAppPool".Split(Convert.ToChar(SmartParametersV2016.comma));
                        //foreach (string identity in identities)
                        //{
                        //    string pipe_identity = @identity.Trim();
                        //    PipeAccessRule pipe_security_rule = new(pipe_identity,
                        //                                            PipeAccessRights.ReadWrite,
                        //                                            AccessControlType.Allow);
                        //    pipe_security.AddAccessRule(pipe_security_rule);
                        //    OutputText(client, SmartParametersV2016.accessInfo + pipe_identity, true);
                        //}
                        OutputText(client, SmartParametersV2016.displayInfo + (SmartParametersV2016.displayLowercaseTables ? "=BOTHCASES" : "=UPPERCASE"), true);

                        OutputText(client, "Server timeout (secs) is " + SmartParametersV2016.serverTimeoutSecs, true);

                        // This routine never exits UNLESS there is an error
                        try
                        {
                            await Task.Run(() => ListenForClientsAsync(servermodel,
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    utilityviewmodel,
                                                    SmartParametersV2016.pipename));
                            await Task.Delay(Timeout.Infinite);
                            //{
                            //    OutputText(client, "Pipe has encountered a problem", true);
                            //}
                        }
                        catch (Exception ex)
                        {
                            OutputText(client, "Unhandled exception: " + ex.Message, true, ConsoleColor.Red);
                        }
                    }
                }
            }
            // This needs to be here EVEN FOR NON-INTERACTIVE (or batch or Task Scheduler runs)
            // because the wait on the pipe is an async one, and it completes immediately
            // if there is no outstanding connection.  If we don't have the following line
            // in, then the program exits, and we (I) look stupid.  SO leave this one alone ..
            // On VMS, we would have just done a SYS$HIBER() and that would have been that,
            // but the spotty-faced Micromasturbatingshit wankers who seem to spend their entire
            // lives jerking off in the Microshit toilets have no concept of proper
            // real-time programming. Just all this async/await bollocks….
            OutputText(client, "Press any key to exit ... ", false);
            // Don't forget 
            Trace.Flush();

            Console.ReadLine();   // <= Microshit's 'SYS$HIBER()' !!!
        }

        internal async Task<bool> CheckSQLiteNew(MainViewModel ourviewmodel,
                                                        ServerModel servermodel)
        {
            try
            {
                if (string.IsNullOrEmpty(ourviewmodel.DataBasePath)) // System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                {
                    ourviewmodel.errorMessage = "Database path is empty ..";
#if !DBSERVER
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ourviewmodel.errorMessage);
#else
                    Console.WriteLine(ourviewmodel.errorMessage);
#endif
                    return false;
                }
                // Where is our Username?  Suppose two different users
                // use the same laptop???  Suppose one users uses WPF and then UWP || WINUI???
                // No Platform specific ao we all use the same DB!!
                string user = ourviewmodel.UserName;
                if (user != "")
                {
                    user += "_";  
                }
                string fullfilepath = System.IO.Path.Combine(ourviewmodel.DataBasePath,
                                                    user + 
                                                    SmartParametersV2016.SmartSwitchFilename);

                // Either open an existing DB or create one if its not there
                // Any new DB will have NO TABLES inside it!
                if (!SmartSwitchDatabase.OpenDatabase(ourviewmodel, fullfilepath, flags: ourviewmodel.CreateFlags))
                {
                    // error message should be set in ourviewmodel
#if !DBSERVER
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ourviewmodel.errorMessage);
#else
                    Console.WriteLine(ourviewmodel.errorMessage);
#endif
                    return false;
                }

                string sql = "SELECT name FROM sqlite_master WHERE type = 'table'";
                List<TableName> table_names_unencryptedList = await SmartSwitchDatabase.GetTableNamesAsync(ourviewmodel, ourviewmodel.sqliteDatabase, sql);

                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
                {
#if !DBSERVER
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ourviewmodel.errorMessage);
#else
                    Console.WriteLine(ourviewmodel.errorMessage);
#endif
                    return false;
                }
                sql = "";
                int result = 0;
                List<TableName> table_namesList = new List<TableName>();

                List<SmartData.SQLiteTables> sqlitetables = SmartSpikeV2017.Lookup_SQLiteTables(ourviewmodel);
                //string names = "";
#if !DBSERVER
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SSQLite table: " + sqlitetables.Count);
#endif
                string tname = "";
                DateTime utcnow = DateTime.UtcNow;  // UTC time
                foreach (SmartData.SQLiteTables sqlite_table in sqlitetables)
                {
                    tname = sqlite_table.SCHEMA_NAME + "." + sqlite_table.TABLE_NAME;
                    TableName tnamex = new TableName()
                    {
                        Name = tname,
                        Updated = utcnow    // So they are all the same
                    };
                    table_namesList.Add(tnamex);
                }
#if !DBSERVER
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "SSQLite table name list: " + table_namesList.Count);
#endif
                //ourviewmodel.loadRemote[0] = false; // Profile?

                // Now do the rest because SmartUsers.Consumers is either already THERE 
                // OR we have just created it.
                foreach (TableName table_name in table_namesList)
                {
                    sql = "";
                    if (!LookupTableName(table_names_unencryptedList, table_name.Name.ToString()))
                    {
                        sql = BuildSQLite(ourviewmodel, table_name.Name);
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
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, ourviewmodel.errorMessage);
#else
                                Console.WriteLine(ourviewmodel.errorMessage);
#endif
                                return false;
                            }
                        }
                    }
                }
            
                // Now here the tables could all be NEW or, they
                // could already exist.  But to get a flag to tell us
                // to load SmartSwitch from SQLServer we need to
                // see if SmartUser.Consumers has a record for the Username
                // So here goes

                //string sqlSelect = "SELECT * FROM " + "[" + "SmartUsers.Consumers" + "]" + ";";
            
                //ourviewmodel.Hamas.sqliteConsumersList = new List<SmartUsers.ConsumersSQLite>(await ourviewmodel.sqliteDatabase.QueryAsync<SmartUsers.ConsumersSQLite>(sqlSelect));
                //Console.WriteLine("Consumers: " + ourviewmodel.Hamas.sqliteConsumersList.Count.ToString());
                //if (ourviewmodel.Hamas.sqliteConsumersList.Count == 0)
                //{
                //    ourviewmodel.localRemote = true;  // Go get it from DBServer
                //}
                //else
                //{
                //    ourviewmodel.localRemote = false;
                //}
            }
            catch (Exception sqlex)
            {
                // Its not there OR there was a problem

                ourviewmodel.errorMessage = sqlex.Message;
#if !DBSERVER
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Remote status: " + ourviewmodel.localRemote);
#else
                Console.WriteLine(ourviewmodel.errorMessage);
#endif
                return false;
            }
            return true;
        }

        internal bool LookupTableName(List<TableName> table_namesList, string table)
        {
            foreach (TableName table_name in table_namesList)
            {
                if (table_name.Name == table)
                {
                    return true;
                }
            }
            return false;
        }

        internal string BuildSQLite(MainViewModel ourviewmodel, string table_name)
        {
            string constraint = "";
            string sql = "CREATE TABLE [" + table_name + "] " + "(";
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
        internal DateTime DateTimeNow(TimeSpan utcOffset)
        {
            return DateTime.UtcNow.Add(utcOffset); // Local time
        }

        internal async Task Sort_Out_Other_Shit(ServerModel servermodel,
                                                string ssclient,
                                                List<SmartData.Currencies> currencies_list,
                                                //List<SmartUsers.ExchangeRates> ERList,
                                                List<SmartData.SQLiteTables> sqlitetables,
                                                List<SmartData.SQLiteSchemas> sqliteschemas,
                                                List<SmartData.SQLiteFields> sqlitefields)
        {
            string errorMessage = string.Empty;
            int tablesCount = 0;

            int databaseIndex = 0;
            int schemaIndex = 0;        // Dont know why this always stays zero? Bug??
            foreach (Petulant databaseItem in servermodel.databases)
            {
                // See what tables we have in the database
                servermodel.errorMessage = string.Empty;
                List<Cow> table_list = SmartProgramV2016.FindAllDBTables(servermodel,
                                                                    databaseItem.connection,
                                                                    databaseItem.schemas,
                                                                    false);
                if (!string.IsNullOrEmpty(servermodel.errorMessage))
                {
                    // There was an exception message returned
                    OutputText(ssclient, servermodel.errorMessage, true);
                }
                else
                {
                    // All is good ..
                    int tableIndex = 0;
                    foreach (Cow tableItem in table_list)
                    {
                        // Hey!! All this shit works!!!!  Yipppeeee!!!
                        servermodel.databases[databaseIndex].tables.Add(tableItem);
                        switch (databaseItem.databaseName)
                        {
                            case "SMARTMUM":
                                switch (tableItem.tableName)
                                {
                                    case "CURRENCIES":
                                
                                        OutputText(ssclient, "Loading currencies", true);
                                        // Get the list of currencies we deal with
                                        currencies_list.Clear();
                                        currencies_list.AddRange(await GetCurrencies(servermodel,
                                                                        ssclient,
                                                                        databaseIndex,
                                                                        schemaIndex, // (SmartData),
                                                                        em => errorMessage = em));
                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {
                                            OutputText(ssclient, "Error :" + errorMessage, true);
                                        }
                                        break;
                                    case "SQLITETABLES":
                                        OutputText(ssclient, "Loading SQLITETABLES", true);
                                        // Get the list of currencies we deal with
                                        sqlitetables.Clear();
                                        sqlitetables.AddRange(await GetSQLiteTables(servermodel,
                                                                        ssclient,
                                                                        databaseIndex,
                                                                        schemaIndex, // (SmartData),
                                                                        tableItem.tableName,
                                                                        em => errorMessage = em));
                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {
                                            OutputText(ssclient, "Error :" + errorMessage, true);
                                        }
                                        break;
                                    case "SQLITESCHEMAS":
                                        OutputText(ssclient, "Loading SQLITESCHEMAS", true);
                                        // Get the list of currencies we deal with
                                        sqliteschemas.Clear();
                                        sqliteschemas.AddRange(await GetSQLiteSchemas(servermodel,
                                                                        ssclient,
                                                                        databaseIndex,
                                                                        schemaIndex, // (SmartData),
                                                                        tableItem.tableName,
                                                                        em => errorMessage = em));
                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {
                                            OutputText(ssclient, "Error :" + errorMessage, true);
                                        }
                                        break;
                                    case "SQLITEFIELDS":                                
                                        OutputText(ssclient, "Loading SQLITEFIELDS", true);
                                        // Get the list of currencies we deal with
                                        sqlitefields.Clear();
                                        sqlitefields.AddRange(await GetSQLiteFields(servermodel,
                                                                        ssclient,
                                                                        databaseIndex,
                                                                        schemaIndex, // (SmartData),
                                                                        tableItem.tableName,
                                                                        em => errorMessage = em));
                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {
                                            OutputText(ssclient, "Error :" + errorMessage, true);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case "SMARTUSERS":
                                if (tableItem.tableName == "EXCHANGE_RATES")
                                {
                                    servermodel.databaseItem = databaseItem;
                                    servermodel.databaseIndex = databaseIndex;
                                    servermodel.schemaName = "SmartUsers";
                                    servermodel.tableIndex = tableIndex;
                                    servermodel.tableName = tableItem.tableName;


                                    if (await GetLastExchangeRate(servermodel,
                                                ssclient,
                                                databaseIndex,
                                                schemaIndex,
                                                em => errorMessage = em))
                                    {
                                        OutputText(ssclient, "Loaded Last Exchange Rate", true);
                                        await CheckECB(servermodel, ssclient);
                                    }
                                    else
                                    {
                                        OutputText(ssclient, "PROBLEM with Last Exchange Rate!", true);
                                    }

                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        tableIndex++;
                    }
                }
                if (string.Equals(databaseItem.databaseName,databaseItem.databaseName.ToUpper()))
                {
                    databaseItem.table_data = await Build_Database_TablesX(servermodel,
                                                            ssclient,
                                                            databaseIndex,
                                                            () => tablesCount,
                                                            tc => tablesCount = tc,
                                                            em => errorMessage = em);
                    // This could be an array?
                    servermodel.refreshArray[databaseIndex] = databaseIndex;
                    tablesCount = 0;
                }
                databaseIndex++;
            }
            return;
        }

        internal async Task OnCheckECBEvent(ServerModel servermodel,
                                            ElapsedEventArgs e,
                                            string client,
                                            System.Timers.Timer ecbclock)
        {
            OutputText(client, "ECB list check event raised at " + e.SignalTime.ToString() + " local time", true);
            // Go and suck the ECB list
            await CheckECB(servermodel, client);
            
            //string err = await CheckECB(servermodel,
            //                    client);
            //if (!string.IsNullOrEmpty(err))
            //{
            //    OutputText(client, "Error :" + err, true);
            //}
            //else
            //{
            //    // Cancel the timer!
            //    ecbclock.Enabled = false;
            //    OutputText(client, "ECB check list timer cancelled", true);
            //}
            return;
        }

        internal async Task<bool> CheckECB(ServerModel servermodel,
                                                    string client)
        {
            // Go and suck the ECB list
            List<SmartUsers.ExchangeRates> poss = await LoadECBExchangeRatesNew(client,
                                                                servermodel.lastGoodDate,
                                                                servermodel.lastECB,
                                                                servermodel.lastGBP,
                                                                servermodel.lastEUR,
                                                                servermodel.lastUSD,
                                                                servermodel.lastJPY);//, currencies_list);
            if (poss.Count == 0)
            {
                OutputText(client, "No ECB records found", true);
                return false;
            }
            else
            {
                // Make sure nothing is in the list
                foreach (SmartUsers.ExchangeRates p in poss)
                {
                    bool match = servermodel.ERList.Any(e => e.TRANSACTION_DATE == p.TRANSACTION_DATE);

                    if (match)
                    {
                        Console.WriteLine($"Match found for {p.TRANSACTION_DATE}");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"No match for {p.TRANSACTION_DATE}");                    
                    }
                }
            }
            char result = SmartParametersV2016.defaultChar;
            string param1 = "";

            foreach (SmartUsers.ExchangeRates ecbRow in poss)
            {
                // Find the ECB date
                DateTime ecbDate = ecbRow.TRANSACTION_DATE;
                // Need to Insert ERList with ecbRow
                if (!string.IsNullOrEmpty(param1))
                {
                    param1 += SmartParametersV2016.recordSeparator;
                }
                param1 += ecbDate.ToString(SmartParametersV2016.sqldateFormat) + SmartParametersV2016.unitSeparator +
                        // Because we now have a good value
                        ecbRow.ECB.ToString() + SmartParametersV2016.unitSeparator +
                        ecbRow.CURRENCY_RATE_01 + SmartParametersV2016.unitSeparator +
                        ecbRow.CURRENCY_RATE_02 + SmartParametersV2016.unitSeparator +
                        ecbRow.CURRENCY_RATE_03 + SmartParametersV2016.unitSeparator +
                        ecbRow.CURRENCY_RATE_04 + SmartParametersV2016.unitSeparator +
                        DateTime.UtcNow.ToString(SmartParametersV2016.sqldateFormat) + SmartParametersV2016.unitSeparator +
                        DateTime.UtcNow.ToString(SmartParametersV2016.sqldateFormat) + SmartParametersV2016.unitSeparator +
                        string.Empty + SmartParametersV2016.unitSeparator +
                        SmartParametersV2016.sqldefaultdates;
            }
            string[] pee_one = param1.Split(SmartParametersV2016.recordSeparator);

            result = await Insert_Common(servermodel,
                                servermodel.databaseIndex,
                                string.Empty,         // Its a generic table entry
                                servermodel.schemaName,
                                servermodel.tableIndex,
                                servermodel.tableName,
                                pee_one,
                                SmartParametersV2016.unitSeparator,   // Field split 'cos ',' not allowed in cookies and I have a whitespace in the date/time
                                em => servermodel.errorMessage = em);
            if (result != SmartParametersV2016.operationFailure &&
                string.IsNullOrEmpty(servermodel.errorMessage))
            {
                foreach (string param2 in pee_one)
                {

                    string outdate = string.Empty;
                    DecompressRate(param2, ref outdate);
                    OutputText(client, "ER update: " +
                                    outdate,
                                    true);
                }
                // Rebuild the latest
                servermodel.ERList.Clear();
                servermodel.ERList.Add(poss.Last());
            }
            else
            {
                OutputText(client, "ER update failed: " +
                                servermodel.errorMessage,
                                true);
            }
            return true;
        }
        internal string DecompressRate(string param1, ref string outdate)
        {
            string todaysRate = string.Empty;
            string[] newsx = param1.Split(SmartParametersV2016.unitSeparator);
            int count_p1 = 0;
            outdate = string.Empty;
            foreach (string p1 in newsx)
            {
                switch (count_p1)
                {
                    case 0:
                        outdate = p1;
                        todaysRate = Convert.ToDateTime(p1).AddDays(1).ToString(SmartParametersV2016.sqldateFormat) +
                        SmartParametersV2016.unitSeparator;
                        break;
                    case 1:
                        outdate += SmartParametersV2016.space + Convert.ToInt16(p1).ToString();
                        todaysRate = todaysRate + p1 + SmartParametersV2016.unitSeparator;
                        break;
                    case 2:
                        outdate += " GBP " + p1;
                        todaysRate = todaysRate + p1 + SmartParametersV2016.unitSeparator;
                        break;
                    case 3:
                        outdate += " EUR " + p1;
                        todaysRate = todaysRate + p1 + SmartParametersV2016.unitSeparator;
                        break;
                    case 4:
                        outdate += " USD " + p1;
                        todaysRate = todaysRate + p1 + SmartParametersV2016.unitSeparator;
                        break;
                    //case 5:
                    //    outdate += " CAD " + p1;
                    //    todaysRate = todaysRate + p1 + SmartParametersV2016.unitSeparator;
                    //    break;
                    case 5:
                        outdate += " JPY " + p1;
                        todaysRate = todaysRate + p1 + SmartParametersV2016.unitSeparator;
                        break;
                    default:
                        break;
                }
                count_p1++;
            }
            return todaysRate;
        }

        internal bool Sort_Out_Databases(ServerModel servermodel,
                                                string client,
                                                string machine_name,
                                                List<string> allSchemas)
        {
            foreach (Petulant databaseItem in servermodel.databases)
            {
                databaseItem.connection.ConnectionString = databaseItem.connectionString;
                int new_attempts = 1;
                while (new_attempts <= 10)
                {
                    try
                    {
                        if (databaseItem.connection.State == ConnectionState.Open)
                        {
                            break;
                        }
                        databaseItem.connection.Open();

                        OutputText(client, machine_name + SmartParametersV2016.space + databaseItem.databaseName + " Opened", true);

                        short ordinal = SmartProgramV2016.FindAllProperties(servermodel, databaseItem.connection, "ORDINAL");
                        if (ordinal != -1)
                        {
                            databaseItem.ordinal = ordinal;
                        }

                        List<Drivel> schemas = SmartProgramV2016.FindAllSchemas(servermodel, databaseItem.connection, SmartParametersV2016.keystone);
                        if (schemas.Count > 0)
                        {
                            foreach (Drivel schema in schemas)
                            {
                                databaseItem.schemas.Add(schema);

                                OutputText(client, databaseItem.databaseName + " schema: " + schema.schema_name, true);
                                allSchemas.Add(schema.schema_name);
                            }
                        }
                        else
                        {
                            OutputText(client, machine_name + " Cannot determine " + databaseItem.databaseName + " schema name: ", true);
                            break;
                        }
                        List<Tiresome> procedures_list = SmartProgramV2016.FindAllProcedures(servermodel, databaseItem.connection, "sp_%", databaseItem.databaseName);
                        databaseItem.procedures = procedures_list;
                    }
                    catch (InvalidOperationException exception)
                    {
                        OutputError(client, exception.Message, machine_name, databaseItem.databaseName, databaseItem.connection.ConnectionString);
                    }
                    catch (SqlException exception)
                    {
                        OutputError(client, exception.Message, machine_name, databaseItem.databaseName, databaseItem.connection.ConnectionString);
                    }
                    catch (ConfigurationErrorsException exception)
                    {
                        OutputError(client, exception.Message, machine_name, databaseItem.databaseName, databaseItem.connection.ConnectionString);
                    }
                    if (databaseItem.connection.State == ConnectionState.Closed)
                    {
                        TimeSpan waitTime = new(0, 0, new_attempts * 6);
                        OutputText(client, "Re-trying in " + new_attempts * 6 + " seconds...", true);
                        Thread.Sleep(waitTime);
                        new_attempts++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (new_attempts > 10)
                {
                    return false;
                }
            }
            return true;
        }

        internal void Trace_Files(string client, ref string tracePath)
        {
            if (!string.IsNullOrEmpty(tracePath))
            {
                try
                {
                    tracePath = Path.Combine(tracePath, SmartParametersV2016.serverTraceFilename);
                    bool tracefound = File.Exists(tracePath);
                    Trace.AutoFlush = true;
                    // https://support.microsoft.com/en-gb/help/815788/how-to-trace-and-debug-in-visual-c
                    tr1 = new TextWriterTraceListener(tracePath);
                    Trace.Listeners.Clear();
                    Trace.Listeners.Add(tr1);

                    // Well check out http://stackoverflow.com/questions/12250735/c-sharp-textwriter-allow-reading-of-files
                    // for this change ... which required that I comment out the TextWriter overload at the bottom of the file
                    // to get it to work ...

                    if (Environment.UserInteractive)
                    {
                        // Colours happen interactively ... but not in the background file!
                        ctl = new ConsoleTraceListener(false);
                        Trace.Listeners.Add(ctl);
                    }
                    if (tracefound)
                    {
                        OutputText(client, tracePath + " re-opened", true);
                    }
                    else
                    {
                        OutputText(client, tracePath + " created", true);
                    }
                }
                catch (IOException ex)
                {
                    OutputText(client, $"Failed to initialize trace: {ex.Message}", true);
                }
            }
            return;
        }

        internal void OutputError(string client,
                                            string exception_message,
                                            string machine_name,
                                            string databaseName,
                                            string connectionString)
        {
            OutputText(client, exception_message, true);
            OutputText(client, machine_name + " Cannot open " +
                            databaseName + " Database: " +
                            "connectionString", true);  // Deliberately obsfucated
            return;
        }

        private async Task ListenForClientsAsync(ServerModel servermodel, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, UtilityViewModel utilityviewmodel, string pipeName)//, PipeSecurity pipeSecurity)
        {
            const int inbuf = 4096, outbuf = 4096;

            PipeSecurity pipeSecurity = new();
            //Is this okay to do?  SYSTEM Read/Write?
            //string[] identities = "SYSTEM, IIS APPPOOL\\DefaultAppPool".Split(',');
            //foreach (string identity in identities)
            //{
                //string pipe_identity = @identity.Trim();
                //PipeAccessRule pipe_security_rule = new(pipe_identity,
                //                                        PipeAccessRights.ReadWrite,
                //                                        AccessControlType.Allow);
                
                
                //pipeSecurity.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.FullControl, AccessControlType.Allow));
                //pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().Name, PipeAccessRights.FullControl, AccessControlType.Allow));


                //pipeSecurity.AddAccessRule(pipe_security_rule);


                // Allow SYSTEM (optional, often harmless allows me t
                // to run up ANNA to test)
                pipeSecurity.AddAccessRule(new PipeAccessRule(
                    new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null),
                    PipeAccessRights.ReadWrite,
                    AccessControlType.Allow));

                // Allow your user account (pipe creator)
                SecurityIdentifier currentUser = WindowsIdentity.GetCurrent().User;
                pipeSecurity.AddAccessRule(new PipeAccessRule(
                    currentUser,
                    PipeAccessRights.FullControl,
                    AccessControlType.Allow));

                // 🔑 Allow all IIS AppPool identities (S-1-5-82-0)
                // This covers IIS APPPOOL\DefaultAppPool and others
                SecurityIdentifier allAppPoolsSid = new SecurityIdentifier("S-1-5-82-0");
                pipeSecurity.AddAccessRule(new PipeAccessRule(
                    allAppPoolsSid,
                    PipeAccessRights.ReadWrite,
                    AccessControlType.Allow));




                //OutputText(pipeName, SmartParametersV2016.accessInfo + pipe_identity, true);
            //}
            
            //OutputText(pipeName, $"🟢 Starting pipe server '{pipeName}'...", true);
            OutputText(pipeName, $"Starting pipe server '{pipeName}'...", true);

            try
            {
                while (true)
                {
                    // Create a new server pipe instance for the next client
                    var pipeServer = NamedPipeServerStreamConstructors.New(
                                   pipeName,
                                   PipeDirection.InOut,
                                   NamedPipeServerStream.MaxAllowedServerInstances,
                                   PipeTransmissionMode.Message,
                                   PipeOptions.Asynchronous,
                                   inbuf,
                                   outbuf,
                                   pipeSecurity);

                    //OutputText(pipeName, "Waiting for a client to connect...", true);
                    await pipeServer.WaitForConnectionAsync();
                    
                    // Handle client in a background task
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await HandleClientAsync(servermodel, ourviewmodel, financeviewmodel, utilityviewmodel, pipeServer, pipeName);
                        }
                        catch (Exception ex)
                        {
                            OutputText(pipeName, $"❌ Error handling client: {ex.Message}", true);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task HandleClientAsync(ServerModel servermodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    UtilityViewModel utilityviewmodel,
                                                    NamedPipeServerStream pipe, 
                                                    string pipeName)
        {
            var buffer = new byte[4096];
            var sb = new StringBuilder();

            try
            {
                while (pipe.IsConnected)
                {
                    int bytesRead = await pipe.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string clientUser = pipe.GetImpersonationUserName();
                    //OutputText(pipeName, "✅ Client connected " + clientUser, true); ;

                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    sb.Append(chunk);

                    if (pipe.IsMessageComplete)
                    {
                        string message = sb.ToString();
                        sb.Clear();

                        string response = await ProcessMessageAsync(servermodel, ourviewmodel, financeviewmodel, utilityviewmodel, pipeName, message, 0);

                        using var writer = new StreamWriter(pipe) { AutoFlush = true };
                        await writer.WriteAsync(response);

                    }
                }

                //OutputText(pipeName, "Client disconnected", true);
            }
            catch (IOException)
            {
                OutputText(pipeName, "Client disconnected unexpectedly.", true);
            }
            catch (Exception ex)
            {
                OutputText(pipeName, ex.Message, true);
            }
            finally
            {
                pipe.Dispose();
            }
        }


        private async Task<string> ProcessMessageAsync(ServerModel servermodel, 
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            UtilityViewModel utilityviewmodel,
                                                            string client, 
                                                            string messageIn, int clientId)
        {
            servermodel.data_stream = new StringBuilder();
            TimeSpan killer = TimeSpan.FromSeconds(SmartParametersV2016.keepaliveLimit * 2);

            string[] fields = Array.Empty<string>();
            string output_string = string.Empty;

            char result = SmartParametersV2016.defaultChar;
            string errorMessage = string.Empty;

            string exception_message = string.Empty;
            List<string> utctables = new();

            // This works in a 'using' block as well, just take out the pipeServer.Dispose()
            //int maxservers = 254,
            //    inbuf = 4096,
            //    outbuf = 4096;
            // What complete BOLLOCKS all this Microshit twaddle is
            // Couldn't have done it without this:
            // https://stackoverflow.com/questions/50711518/attempted-to-perform-an-unauthorized-operation-when-calling-namedpipeserverstr

            // Today 30th March 2020 my beloved cousin Martin Burns died
            // He was a wonderful son, brother and father and although
            // we didn't see much of each other, we got on ok.
            // We had one minor contretemps when I was about 15 and he was
            // about 5 when he took the piss out of my new shoes; I never
            // knew why he did that, because he had SO much - a stable home,
            // a solid father and mother and two normal brothers - when I
            // had none of these, but he did, and I remember it to this day.
            // I think maybe he was jealous of my brains, but over the years 
            // we came to 'get on well'; I loved his Mum and liked his Dad, 
            // and there was never a cross word between myself and him.
            // And there was never a cross word between myself and Stephen.
            // So there you have it - goodbye and all the best big fella,
            // see you when I get to heaven ...
            //

            
            // Continuous unwanted fucking running commentary as per usual
            try
            {
                // A message has come in ... why do we wait for this to
                // happen BEFORE we check whether to enable the timerClock?
                // Well ... its because UNLESS some message or other e.g. 'Connect'
                // HAS come in, we don't need to do ANYTHING with Exchange Rates
                // because we have no-one to tell!  We can only hope that SOMEONE
                // 'Connect's before the 90 day period is up!
                // Get Central European Standard Time zone

                //TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

                //// I think  I need to add the utcOffset in here to get the PROPER local time
                //// Yes, because its possible to set the Local PC time to something stupid
                //// The utcOffset should be milliseconds btw
                //DateTime cetTime = TimeZoneInfo.ConvertTime(DateTime.Now + utcOffset, TimeZoneInfo.Local, tst);
                //if (!servermodel.timerClock.Enabled)
                //{
                //    //string rays = tst.IsDaylightSavingTime(cetTime) ? tst.DaylightName : tst.StandardName;
                //    //OutputText(client, "Time in " + rays + " zone: " + cetTime, true);
                //    //Console.WriteLine("   UTC Time: {0}", TimeZoneInfo.ConvertTimeToUtc(tstTime, tst));
                //    if ((cetTime.Hour * 60 + cetTime.Minute) >= 855) // 855 = 14*60 + 15 i.e. 14:15pm?
                //    {
                //        servermodel.timerClock.Enabled = true;
                //        OutputText("pipe", "ECB list check event - enabled", true);
                //    }
                //}
                //else
                //{
                //    if ((cetTime.Hour * 60 + cetTime.Minute) >= 915) // 915 = 15*60 + 15
                //    {
                //        servermodel.timerClock.Enabled = false;
                //        OutputText("pipe", "ECB list check event - disabled", true);
                //    }
                //}

                // ONE DAY REPLACE THIS WITH BINARYWRITER AND BINARYREADER - <= THAT DAY HAS ARRIVED!!
                // NO IT HASN'T - for a start these two don't do async/await (useless fuckers)
                //stream_reader = new StreamReader(pipeServer);
                // We DON'T use ReadToEndAsync here because that will only complete
                // when any CLient closes the pipe .. and we don't want that!  We want
                // the pipe to stay open to receive the response!!!
                //string message_in = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(messageIn))
                {
                    OutputText("pipe", "Empty message received - exiting loop", true);
                    return "Empty";
                }

                //else
                //{
                //    OutputText(client, DateTime.Now.ToString() + " Message: " + message_in, true);
                //}
                
                // Uncheck the next line to see what was received
                //bool allowed = false;
                //foreach (string permitted_sender in SmartParametersV2016.permittedSenders)
                //{
                //    if (requester == permitted_sender)
                //    {
                //        if (SmartParametersV2016.displayServerMessage)
                //        {
                //            OutputText(client, "Received - " + messageIn + " from " + requester, true);
                //        }
                //        allowed = true;
                //        break;
                //    }
                //}
                //if (!allowed)
                //{
                //    OutputText(client, "Unexpected receipt - " + messageIn + " from " + requester, true);
                //}

                int database_target = 0;
                int table_target = 0;
                //int procedure_target = 0;

                //
                // YOU CAN LEARN SOMETHING NEW EVERY DAY, RAY!!! (IF you have an open mind, that is)
                //
                // We don't check for an empty message
                // Now we try and do multi-lines ... oh oh ...
                string[] multi_line = messageIn.Split(SmartParametersV2016.fileSeparator);

                int message_count = 0;
                string trans_string_on = "",
                        trans_string_off = "";

                string requestingClient = "";
                string targetClient = "";
                string schemaName = "";
                string target_name = "";
                string targetRange = "";
                string operation = "";
                servermodel.antitoken = new();
                string display_targetX = "";

                string p1x;
                string p2;
                string p3;
                string p4;
                string p5;
                
                string update_token = string.Empty;
                List<RequestVerificationToken> username_list = new List<RequestVerificationToken>();


                while (message_count < multi_line.Length)
                {
                    trans_string_on = trans_string_off = string.Empty;

                    string message = multi_line[message_count];
                    fields = message.Split(SmartParametersV2016.ourSeparator);
                    if (fields.Length > 0)
                    {
                        try
                        {
                            // Here is where I split the anti token
                            string[] temp = fields[0].Split(SmartParametersV2016.unitSeparator);
                            if (temp.Length != 3)
                            {
                                errorMessage = "**Invalid token**";
                                servermodel.data_stream.Append(SmartParametersV2016.operationFailure);
                                break;
                            }
                            fields[0] = temp[0];
                            servermodel.antitoken.username = temp[0];
                            servermodel.antitoken.request_value = temp[1];
                            //antitoken.cookie_path = temp[2];
                            //antitoken.cookie_domain = temp[3];
                            //antitoken.cookie_secure = Convert.ToBoolean(temp[4]);
                            servermodel.antitoken.request_timestamp = Convert.ToDateTime(temp[2], SmartParametersV2016.defaultCulture);

                            // Remove all who have been in the list without a "Connect" update
                            // for over 2 minutes
                            PurgeList(utcOffset, servermodel, FindUsernames(servermodel, fields[0]), killer);//, fields[0]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    List<string> targetRangeX = new List<string>();

                    // Clear down the output stream
                    servermodel.data_stream.Clear();
                    result = Do_Some_Decoding(servermodel,
                                            ref servermodel.databases,
                                            fields,
                                            ref database_target,
                                            ref schemaName,
                                            ref table_target,
                                            ref target_name,
                                            ref targetRange,
                                            ref requestingClient,
                                            ref targetClient,
                                            ref operation,
                                            ref targetRangeX);
                    if (result != SmartParametersV2016.operationSuccess)
                    {
                        errorMessage = "**Not Found**";
                        servermodel.data_stream.Append(SmartParametersV2016.operationFailure);
                        break;
                    }
                    else
                    {
                        errorMessage = string.Empty;
                        // Was the username sent in UPPER case?
                        // Yes? then it means connect and check for DB
                        //

                        p1x = string.Empty;
                        p2 = string.Empty;
                        p3 = string.Empty;
                        p4 = string.Empty;
                        p5 = string.Empty;
                        DateTime lastdate = SmartParametersV2016.defaultDate;
                        switch (operation)
                        {
                            case SmartParametersV2016.connectSymbol:
                                try
                                {
                                    // When you come to 'G' all of
                                    // database, schema and table are 0 ...
                                    if (fields.Length >= 3)
                                    {
                                        result = ConnectRoutine(utcOffset,
                                                                servermodel,
                                                                ourviewmodel,
                                                                ourviewmodel.sqliteDatabase,
                                                                requestingClient,
                                                                fields,
                                                                operation,
                                                                "SmartProfile.Groups",
                                                                servermodel.withdrawnDate,
                                                                username_list);
                                    }
                                    else
                                    {
                                        result = SmartParametersV2016.operationFailure;
                                    }
                                    display_targetX = "Connect"; // Cannot Disconnect anybody else
                                    servermodel.record_count = username_list.Count;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                break;
                            // Work out what to request and ...
                            case SmartParametersV2016.disconnectSymbol:   // Disconnect
                                username_list =
                                                FindUsernames(servermodel, requestingClient);
                                if (username_list.Count == 0)
                                {
                                    servermodel.data_stream.Append(SmartParametersV2016.operationFailure);
                                }
                                else
                                {
                                    update_token = TokenRemoval(servermodel, username_list);

                                    servermodel.data_stream.Append(SmartParametersV2016.operationSuccess);
                                }
                                display_targetX = "Disconnect"; // Cannot Disconnect anybody else
                                servermodel.record_count = username_list.Count;
                                break;
                            case "R":   // For reload
                                        // Bump the comparison date
                                display_targetX = target_name + " " + targetRange + " " + targetClient;

                                servermodel.withdrawnDate = DateTimeNow(utcOffset).ToString(SmartParametersV2016.defaultCulture);
                                if (!string.IsNullOrEmpty(targetRange))
                                {
                                    servermodel.withdrawnDate = targetRange;
                                }                                
                                int tablesCount = 0;
                                for (int databaseIndex = 0; databaseIndex < servermodel.refreshArray.Length; databaseIndex++)
                                {
                                    if (servermodel.refreshArray[databaseIndex] >= 0)
                                    {
                                        servermodel.databases[databaseIndex].table_data.Clear();
                                        servermodel.databases[databaseIndex].table_data = await Build_Database_TablesX(servermodel,
                                                                                    requestingClient,
                                                                                    databaseIndex,
                                                                                    () => tablesCount,
                                                                                    tc => tablesCount = tc,
                                                                                    em => errorMessage = em);
                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {
                                            break;
                                        }
                                    }
                                }
                                if (string.IsNullOrEmpty(errorMessage))
                                {
                                    result = SmartParametersV2016.operationSuccess;
                                    servermodel.data_stream.Append(tablesCount);
                                }
                                else
                                {
                                    servermodel.data_stream.Append(result);
                                }
                                // So the last time we sucked SmartProfile/SmartUtility/SmartFinance etc is 'now'
                                // I don't know what's happening here - after a reload
                                // I cannot Connect (I can Disconnect but not COnnect)
                                // Too tired today after sorting out SmartDashboard!!
                                servermodel.record_count = tablesCount;
                                break;
                            case "L":   // For DBServer log
                                bool exception_caught = false;
                                display_targetX = target_name + " " + targetRange + " " + targetClient;
                                if (File.Exists(servermodel.tracePath))
                                {
                                    try
                                    {
                                        // Don't change this!  Its the ONLY way I ever got this to work!!
                                        Stream stream = File.Open(servermodel.tracePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                        using (StreamReader Lreader = new(stream))
                                        {
                                            servermodel.data_stream.AppendLine(await Lreader.ReadToEndAsync());
                                        }
                                        stream.Close();
                                        result = SmartParametersV2016.operationSuccess;
                                        // Sometimes .. we  don't return 'result'
                                    }
                                    catch (IOException)
                                    {
                                        exception_caught = true;
                                        OutputText(requestingClient, "IO Exception", true);
                                    }
                                    catch (UnauthorizedAccessException)
                                    {
                                        exception_caught = true;
                                        OutputText(requestingClient, "Unauthorized access", true);
                                    }
                                    catch (Exception ex)
                                    {
                                        exception_caught = true;
                                        OutputText(requestingClient, "Exception " + ex.Message, true);
                                    }
                                    if (exception_caught)
                                    {
                                        servermodel.data_stream.Clear();
                                        servermodel.data_stream.Append(result);
                                    }
                                }
                                else
                                {
                                    servermodel.data_stream.Append(SmartParametersV2016.operationFailure);
                                }
                                break;
                            case "P":   // For Stored Procedure
                                try
                                {
                                    display_targetX = target_name;
                                    if (!string.IsNullOrEmpty(targetClient) &&
                                        requestingClient != targetClient)
                                    {
                                        display_targetX += "(" + targetClient + ")";
                                    }
                                    result = await Decode_Procedure(servermodel,
                                                requestingClient,
                                                database_target,
                                                schemaName,
                                                table_target,  // Should be -1 // Why not procedure_target?
                                                target_name, // This is the procedure_name
                                                operation,
                                                targetRange,
                                                em => errorMessage = em);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                // Sometimes .. we  don't return 'result'
                                break;
                            case "T":
                                try
                                {
                                    display_targetX = target_name;
                                    if (!string.IsNullOrEmpty(targetClient) &&
                                        requestingClient != targetClient)
                                    {
                                        display_targetX += "(" + targetClient + ")";
                                    }
                                    result = await Decode_Table(servermodel,
                                        ourviewmodel.sqliteDatabase,
                                            requestingClient,
                                            database_target,
                                            schemaName,
                                            table_target,  // Should be -1 // Why not procedure_target?
                                            target_name,
                                            targetRange,
                                            fields,
                                            operation,
                                            p1x,
                                            em => errorMessage = em);
                                    
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                // Sometimes .. we  don't return 'result'
                                break;
                            case "S":
                                if (target_name == SmartParametersV2016.wildcard)
                                {
                                    string[] schemas_list = fields[5].Split(SmartParametersV2016.unitSeparator);
                                    servermodel.tableCount = 0;
                                    servermodel.record_count = 0;
                                    
                                    bool allowed = true;
                                    List<RequestVerificationToken> usernameList = new List<RequestVerificationToken>();
                                    SmartProfile.Groups nibs = new SmartProfile.Groups();

                                    if (!string.IsNullOrEmpty(targetClient) &&
                                        requestingClient != targetClient)                                        
                                    {
                                        usernameList = FindUsernamesValueTrue(servermodel, targetClient);
                                        // Is this the 'True' one?
                                        if (usernameList.Count > 0)
                                        {
                                            if (FixAnnaDEKs(servermodel,
                                                                usernameList,
                                                                targetClient,
                                                                ref nibs))
                                            {
                                                // At this point !! We have ANNA's DEKS (all of em)
                                                // in nibs .. which we can send back. We *may not*
                                                // use all of 'em btw, but at least we have them! 
                                                servermodel.data_stream.Append("SmartProfile.Groups");
                                                servermodel.data_stream.Append(SmartParametersV2016.recordSeparator);
                                                servermodel.data_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartProfile.Groups>(nibs, false, SmartParametersV2016.fieldSeparator));
                                                servermodel.record_count = 1;
                                                servermodel.tableCount = 1;
                                            }
                                        }
                                        // Now we have nibs which may or may not contain Deks
                                        // What we need to do now, is see if ANNA has entries
                                        // for us as requestingClient for the Finance and Utility
                                        // schemas. If not, then don't send data back, but if she does
                                        // then check to see if see if we CAN send data back; we
                                        // test to see if we have a corresponding DEK 
                                    }
                                    foreach (string schema_name in schemas_list)
                                    {
                                        try
                                        {
                                            if (targetClient != requestingClient)
                                            {
                                                allowed = DoesAnnaAllowRay(usernameList,
                                                            schema_name,
                                                            requestingClient,
                                                            targetClient);
                                            }
                                            if (allowed)      // Requester
                                            {
                                                result = await Decode_Select(servermodel,
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        utilityviewmodel,
                                                        requestingClient,
                                                        targetClient,
                                                        database_target,
                                                        schema_name,
                                                        table_target,
                                                        target_name,
                                                        operation,
                                                        em => errorMessage = em);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }
                                        if (result != SmartParametersV2016.operationSuccess)
                                        {
                                            break;
                                        }
                                    }
                                    display_targetX = "SmartSwitch";
                                    if (!string.IsNullOrEmpty(targetClient) &&
                                        requestingClient != targetClient)
                                    {
                                        display_targetX += "(" + targetClient + ")";
                                    }
                               }
                                else
                                {
                                    // Single table
                                    try
                                    {
                                        result = await Decode_Select(servermodel,
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    utilityviewmodel,
                                                    requestingClient,
                                                    targetClient,
                                                    database_target,
                                                    string.Empty,   // schema_name?
                                                    table_target,
                                                    target_name,
                                                    operation,
                                                    em => errorMessage = em);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                                // Sometimes .. we  don't return 'result'
                                break;
                            
                            case "D":
                            case "I":
                            case "U":
                                try
                                {
                                    display_targetX = schemaName + "." + target_name;
                                    trans_string_on = SmartParametersV2016.transactionOn;
                                    result = await Decode_InsertUpdate(servermodel,
                                            ourviewmodel,
                                            ourviewmodel.sqliteDatabase,
                                            database_target,  // For Listener Inserts
                                            requestingClient,
                                            schemaName,
                                            table_target,
                                            target_name,
                                            targetRangeX,
                                            operation,
                                            em => errorMessage = em);
                                    if (result == SmartParametersV2016.operationFailure)
                                    {
                                        Console.WriteLine(operation + " failure");
                                    }                                
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                trans_string_off = SmartParametersV2016.transactionOff;
                                break;
                                
                            case SmartParametersV2016.SingleProcedure:
                                // Need to change this to 'List of Singles'
                                try
                                {
                                    display_targetX = target_name;

                                    result = await Decode_Singles(servermodel,
                                            ourviewmodel.sqliteDatabase,
                                            requestingClient,
                                            database_target,
                                            schemaName,
                                            table_target,
                                            target_name,
                                            targetRange,
                                            targetClient,
                                            em => errorMessage = em);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                
                                // Sometimes .. we  don't return 'result'
                                break;
                            default:
                                break;

                                // She is ALWAYS talking about her fucking self ALWAYS
                                // ALWAYS ALWAYS ALWAYS ALWAYS FUCKING ALWAYS FUCK FUCK
                                // FUCK THE SHUT UP TALKING ABOUT YOUR FUCKING SELF
                                // *****SHUT THE FUCK UP***********
                                // >-------------SHUT UP ------------<                     
                        }
                        if (result != SmartParametersV2016.operationSuccess)
                        {
                            break;
                        }
                    }
                    message_count++;
                    if (message_count < multi_line.Length)
                    {
                        //operation = trans_string_on + operation;
                        Info_Line(requestingClient, display_targetX, operation, servermodel.record_count, true);
                    }
                }
                if (result == SmartParametersV2016.operationSuccess)
                {
                    // If we come in with a 'T' then sqlIUDTran WILL be 'null'
                    // because we don't create a transaciton for SELECTs!!
                    if (operation == "D" ||
                        operation == "I" ||
                        operation == "U")
                    {
                        if (schemaName == "SmartData" &&
                            target_name == "Listener")
                        {
                            // For Listener - hopefully this is always 'success'
                            // but in theory it could be a 'failure'
                            servermodel.data_stream.Append(result);
                            display_targetX = ""; // So no Info comes out
                        }
                        else
                        {
                            // For our SQLite inserts, updates and deletes
                            if (await SmartPhyllV2020.LoadCommonUsers(ourviewmodel,
                                                                            true,
                                                                            "SmartUsers",
                                                                            "Consumers",
                                                                            requestingClient,
                                                                            requestingClient, // Although we never get SmartUsers if targetClient <> requestingClient
                                                                            requestingClient,
                                                                            false,
                                                                            true))
                            {
                                servermodel.data_stream.Append(result);
                                servermodel.data_stream.Append(SmartParametersV2016.fileSeparator);
                                // Extract everything including the Username
                                servermodel.data_stream.Append(SmartPhyllV2020.DecodeToSQLNoUsername<SmartUsers.ConsumersSQLite>(ourviewmodel.Hamas.sqliteConsumersList.First(), false, SmartParametersV2016.fieldSeparator, true));

                            }
                            else
                            {
                                // Something went wrong
                                result = SmartParametersV2016.operationFailure;
                                servermodel.data_stream.Append(result);
                            }
                        }
                    }                    
                }
                // We use WriteLineAsync here because we want the waiting Client
                // to use ReadLineAsync on the pipe.  THEN (and only then) if
                // we close the pipe 'cos its out pipe and we are in charge
                Info_Line(requestingClient, display_targetX, operation, servermodel.record_count, false);
                if (!string.IsNullOrEmpty(display_targetX))
                {
                    FourA(result, errorMessage, true);
                }
                return servermodel.data_stream.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        // Managed to extract this bastard at last!!
        // Now ... lets see if we can do a 'SmartSwitch' load by Schema!
        internal char ConnectRoutine(TimeSpan utcOffset,
                                    ServerModel servermodel,
                                    MainViewModel ourviewmodel,
                                    SQLiteAsyncConnection connection,
                                    string requester,
                                    string[] fields,
                                    string operation,
                                    string target_name,
                                    string withdrawnDate,
                                    List<RequestVerificationToken> usernameList)
        {
            // All this just for Multi-Meters!!!
            char result = SmartParametersV2016.operationSuccess;
            List<SmartProfile.Groups> groups_list = new List<SmartProfile.Groups>();
            
            // Its a first connection
            // Now ... supposing SmartDBServer has been left ticking away
            // and we've gone past midnight (as its designed to do)
            // And its now 19th Sept ... but the last 'good date'
            // was 18th Sept ... and someone connects.
            // We haven't hit 14:15 yet, so we won't be attempting
            // to contact the ECB for 19th Sept's date, and we
            // haven't re-started SmartDBServer which would have 'mocked up'
            // a date for 19th.  So we now need to check to see if we
            // have a date for 19th and mock one up if we haven't got it
            // So the check is: we have received 'lastdate' which says
            // 'gimme one for 19th' but is it greater than the last ECB
            // date  If it IS, then mock up a date for 19th from the
            // last previous date ...
            DateTime today = DateTimeNow(utcOffset).Date;
            DateTime lastTransactionDate = today;
            if (servermodel.ERList.Count > 0)
            {
                lastTransactionDate = servermodel.ERList.Last().TRANSACTION_DATE.Date;
            }
            if ((operation == SmartParametersV2016.connectSymbol) &&
                lastTransactionDate <= today)
            {
                // Check groups list actually gets updated!!
                result = FirstRoutine(servermodel,
                                ourviewmodel,
                                connection,
                                requester,
                                fields,
                                target_name,
                                groups_list);
                if (result != SmartParametersV2016.operationSuccess)
                {
                    return result;
                }
                // All the good stuff is in groups_list
            }
            // All good so far ...
            
            // TOKENS - Parameter #1
            // Lookup this Username SET in tokens_list            
            string update_token = TokenInsertionAndUpdate(utcOffset, servermodel, FindUsernames(servermodel, requester), groups_list);

            usernameList = FindUsernamesValueTrue(servermodel, requester);
            // Is this the 'True' one?
            if (usernameList.Count == 1)   // Should only be one?? One True love??
            {
                servermodel.data_stream.Append('T'); // Should set UserName to GREEN
            }
            else
            {
                servermodel.data_stream.Append('F');  // should set Username to Black
            }
            //
            // Exchange Rates - Parameter #2  Make sure we only send ONE line back!!
            // and not a fucking list ...
            if (servermodel.ERList.Count > 0)
            {
                if (servermodel.data_stream.Length > 0)
                {
                    servermodel.data_stream.Append(SmartParametersV2016.fileSeparator);
                }
                if ((servermodel.lastdate == SmartParametersV2016.defaultDate) ||
                         (servermodel.lastdate == servermodel.ERList.Last().TRANSACTION_DATE))
                {
                    string latestrates = servermodel.ERList.Last().TRANSACTION_DATE.ToString(SmartParametersV2016.sqldateFormat) + SmartParametersV2016.fieldSeparator +
                                    servermodel.ERList.Last().ECB + SmartParametersV2016.fieldSeparator +
                                    servermodel.ERList.Last().CURRENCY_RATE_01 + SmartParametersV2016.fieldSeparator +
                                    servermodel.ERList.Last().CURRENCY_RATE_02 + SmartParametersV2016.fieldSeparator +
                                    servermodel.ERList.Last().CURRENCY_RATE_03 + SmartParametersV2016.fieldSeparator +
                                    //servermodel.ERList.Last().CURRENCY_RATE_04 + SmartParametersV2016.fieldSeparator +
                                    servermodel.ERList.Last().CURRENCY_RATE_04;
                    servermodel.data_stream.Append(latestrates);
                }
                else
                {
                    // Send the dates requested
                    List<SmartUsers.ExchangeRates> er_update_list =
                        new(from ER in servermodel.ERList
                            where ER.TRANSACTION_DATE >= servermodel.lastdate
                            orderby ER.TRANSACTION_DATE ascending
                            select ER);
                    string er_rates = "";
                    foreach (SmartUsers.ExchangeRates er_row in er_update_list)
                    {
                        if (er_rates != "")
                        {
                            er_rates += SmartParametersV2016.unitSeparator;
                        }
                        er_rates += er_row.TRANSACTION_DATE.ToString(SmartParametersV2016.sqldateFormat) + SmartParametersV2016.fieldSeparator +
                                        er_row.ECB + SmartParametersV2016.fieldSeparator +
                                        er_row.CURRENCY_RATE_01 + SmartParametersV2016.fieldSeparator +
                                        er_row.CURRENCY_RATE_02 + SmartParametersV2016.fieldSeparator +
                                        er_row.CURRENCY_RATE_03 + SmartParametersV2016.fieldSeparator +
                                        //er_row.CURRENCY_RATE_04 + SmartParametersV2016.fieldSeparator +
                                        er_row.CURRENCY_RATE_04;
                    }
                    servermodel.data_stream.Append(SmartParametersV2016.fileSeparator);
                    servermodel.data_stream.Append(er_rates);
                }
            }
            return result;
        }

        internal char MultiUser(ServerModel servermodel,
                                        string ssclient,
                                        string[] fields,
                                        List<SmartProfile.Groups> groups_list)

        {
            // MultiMeter - Parameter #3
            // Check the Groups genius code!
            try
            {
                StringBuilder mmBuilder = new StringBuilder();
                int mm_count = 0;
                if (fields.Length >= 4)
                {
                    //string RECEIVEDEK = string.Empty;

                    foreach (SmartProfile.Groups group_row in groups_list)
                    {
                        //Groups list contains ours so return every one
                        //but try and find a matching PDEK for it if it
                        // hasn't got one
                        SmartProfile.Groups groupNew = new SmartProfile.Groups()
                        {
                            USERNAME = group_row.USERNAME,
                            GROUPNAME = group_row.GROUPNAME,
                            ACTIVEFLAG = group_row.ACTIVEFLAG,
                            MARKER = group_row.MARKER,
                            PDEK = group_row.PDEK,
                            SENDF = group_row.SENDF,
                            FDEK = group_row.FDEK,
                            SENDU = group_row.SENDU,
                            UDEK = group_row.UDEK,
                            RECEIVEALL = group_row.RECEIVEALL,
                            DISPLAYNAME = group_row.DISPLAYNAME
                        };
                        if (string.IsNullOrEmpty(groupNew.PDEK))
                        {
                            bool done = false;
                            // Try and find a matching PDEK for one in our group
                            List<RequestVerificationToken> usernameListx =
                                    FindUsernamesValueTrue(servermodel, group_row.GROUPNAME);
                            // Is this the 'True' one?
                            if (usernameListx.Count > 0)   // Should only be one?? One True love??
                            {
                                string ownerPDEK = "",
                                        ownerFDEK = "",
                                        ownerUDEK = "";
                                foreach (RequestVerificationToken rv_token in usernameListx)
                                {
                                    // For reasons I cannot BEGIN to fathom, inner_row was getting updated
                                    foreach (SmartProfile.Groups inner_row in rv_token.groupsList)
                                    {
                                        // Is this the Owner's group i.e. the one with the KEY??
                                        if (inner_row.USERNAME == inner_row.GROUPNAME)
                                        {
                                            // You've found the Major key of the GroupName
                                            // you are looking for ... so what else do you need
                                            // to do?
                                            ownerPDEK = inner_row.PDEK;
                                            ownerFDEK = inner_row.FDEK;
                                            ownerUDEK = inner_row.UDEK;
                                        }
                                        else
                                        {
                                            // Does ANNA have an entry for RAY ?
                                            if (inner_row.GROUPNAME == group_row.USERNAME)
                                            {
                                                // Does ANNA allow RAY to get her data?
                                                if (inner_row.SENDF || inner_row.SENDU)
                                                {
                                                    // Oh. This isn't going to work
                                                    // You are making a PERMANENT change
                                                    // somehow
                                                    groupNew = new SmartProfile.Groups()
                                                    {
                                                        USERNAME = group_row.USERNAME,
                                                        GROUPNAME = group_row.GROUPNAME,
                                                        ACTIVEFLAG = group_row.ACTIVEFLAG,
                                                        MARKER = inner_row.MARKER,
                                                        PDEK = ownerPDEK,
                                                        SENDF = inner_row.SENDF,
                                                        FDEK = ownerFDEK,
                                                        SENDU = inner_row.SENDU,
                                                        UDEK = ownerUDEK,
                                                        RECEIVEALL = group_row.RECEIVEALL,
                                                        DISPLAYNAME = group_row.DISPLAYNAME
                                                    };
                                                    done = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (done)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        if (mmBuilder.Length > 0)
                        {
                            mmBuilder.Append(SmartParametersV2016.unitSeparator);
                        }
                        // Even if DEK is 'empty' still send it
                        // cos that means the sender wants out!
                        // Nothing about Cube faces here (as yet) though ...
                        // ..and there won't be. 
                        mmBuilder.Append(groupNew.USERNAME +
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.GROUPNAME +       // Send RAY or MARIA back
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.ACTIVEFLAG +     // Should be true?
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.MARKER +  // Only if DEK isn't empty
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.PDEK +     // Might well be empty?
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.SENDF +    // May be true or false (now)
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.FDEK +     // Might well be empty?
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.SENDU +    // May be true or false (now)
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.UDEK +     // Might well be empty?
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.RECEIVEALL + // May be true or false (now)
                                    SmartParametersV2016.fieldSeparator +
                                    groupNew.DISPLAYNAME);
                        mm_count++;

                        OutputText(ssclient, groupNew.USERNAME + " " +
                                                    (groupNew.ACTIVEFLAG ? "A+" : "A-") + " " +
                                                    groupNew.GROUPNAME + " " +
                                                    (groupNew.SENDF ? "F+" : "F-") + " " +
                                                    (groupNew.SENDU ? "U+" : "U-") + " " +
                                                    (groupNew.RECEIVEALL ? "R+" : "R-") + " " +
                                                    groupNew.DISPLAYNAME, true);
                    }
                }
                // Not yet!!
                if (mmBuilder.Length > 0)
                {
                    servermodel.data_stream.Append(SmartParametersV2016.fileSeparator);
                    servermodel.data_stream.Append(mmBuilder);
                    servermodel.record_count = mm_count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("here" + ex.Message);
                return SmartParametersV2016.operationFailure;

            }
            return SmartParametersV2016.operationSuccess;
        }

        internal char FirstRoutine(ServerModel servermodel,
                                        MainViewModel ourviewmodel,
                                        SQLiteAsyncConnection connection,
                                        string ssclient,
                                        string[] fields,
                                        string tableName,
                                        List<SmartProfile.Groups> groups_list)
        {
            // Assume success first
            char result = SmartParametersV2016.operationSuccess;
            
            // Here is where we slot in our own placeholder (sans DEK)
            if (fields[2].Length > 0)
            {
                string[] unpackedStrings = fields[2].Split(SmartParametersV2016.unitSeparator,
                                                            StringSplitOptions.RemoveEmptyEntries);
                StringBuilder[] unpackedBuilders = new StringBuilder[unpackedStrings.Length];
                for (int i = 0; i < unpackedStrings.Length; i++)
                {
                    unpackedBuilders[i] = new StringBuilder(unpackedStrings[i]);
                    string groupRecord = unpackedBuilders[i].ToString();
                    SmartProfile.Groups group = new SmartProfile.Groups();
                    string[] components = groupRecord.Split(SmartParametersV2016.fieldSeparator);
                    if (components.Length < 11)
                    {
                        // Log, skip, or throw depending on use case
                        OutputText(ssclient, $"Invalid group record: {groupRecord}", true);
                        return SmartParametersV2016.operationFailure;
                    }

                    int fieldIndex = 0;
                    foreach (string comp_row in components)
                    {
                        switch (fieldIndex)
                        {
                            case 0:
                                // Sender's Username is SET in tokens_list
                                group.USERNAME = comp_row;
                                break;
                            case 1:
                                group.GROUPNAME = comp_row;
                                break;
                            case 2:
                                group.ACTIVEFLAG = Convert.ToBoolean(comp_row);
                                break;
                            case 3:
                                group.MARKER = Convert.ToDateTime(comp_row);
                                break;
                            case 4:
                                group.PDEK = comp_row;
                                break;
                            case 5:
                                group.SENDF = Convert.ToBoolean(comp_row);
                                break;
                            case 6:
                                group.FDEK = comp_row;
                                break;
                            case 7:
                                group.SENDU = Convert.ToBoolean(comp_row);
                                break;
                            case 8:
                                group.UDEK = comp_row;
                                break;
                            case 9:
                                group.RECEIVEALL = Convert.ToBoolean(comp_row);
                                break;
                            case 10:
                                group.DISPLAYNAME = comp_row;
                                break;
                            default:
                                break;
                        }
                        fieldIndex++;
                    }
                    groups_list.Add(group);
                    OutputText(ssclient, group.USERNAME + " " +
                                                    (group.ACTIVEFLAG ? "A+" : "A-") + " " +
                                                    group.GROUPNAME + " " +
                                                    (group.SENDF  ? "F+" : "F-") + " " +
                                                    (group.SENDU  ? "U+" : "U-") + " " +
                                                    (group.RECEIVEALL ? "R+" : "R-") + " " +
                                                    group.DISPLAYNAME, true);
                }
            }
            return result;
        }

        internal async Task<char> UTCTables(ServerModel servermodel,
                                        MainViewModel ourviewmodel,
                                        SQLiteAsyncConnection connection,
                                        string ssclient,
                                        string[] fields)
        {
            // Assume success first
            char result = SmartParametersV2016.operationSuccess;
            string utcDates = fields[3];
            // We get sent an '*' if the Local is requesting ALL tables
            // The Local either hasn't got ANY data OR its been deleted
            // and needs whatever we can send it
            if (utcDates != SmartParametersV2016.wildcard)
            {
                // Get Consumer record!  Again!!
                result = await CompareConsumer(servermodel,
                                            ourviewmodel,
                                            connection,
                                            ssclient,
                                            em => servermodel.errorMessage = em,
                                            utcDates);
                // There may be more than one 'success' code!
                if (result == SmartParametersV2016.operationFailure ||
                    !string.IsNullOrEmpty(servermodel.errorMessage))
                {
                    OutputText(ssclient, "Compare Consumer problem", true);
                }                
            }
            return result;
        }

        internal async Task<char> CompareConsumer(ServerModel servermodel,
                                                    MainViewModel ourviewmodel,
                                                    SQLiteAsyncConnection connection,
                                                    string requestingClient,
                                                    Action<string> set_errorMessage,
                                                    string utcDates)
        {
            // Assume failure first, always
            char result = SmartParametersV2016.operationFailure;
            string errorMessage = string.Empty;

            if (await SmartPhyllV2020.LoadCommonUsers(ourviewmodel,
                                            true,
                                            "SmartUsers",
                                            "Consumers",
                                            requestingClient,
                                            requestingClient,
                                            requestingClient,
                                            false))  // DBServer request
            {
                // Its succeeded but how many have we got
                if (ourviewmodel.Hamas.consumersList.Count != 1)
                {
                    return SmartParametersV2016.operationFailure;
                }
                servermodel.record_count = 1;
            }
            SmartUsers.Consumers consumer_row = ourviewmodel.Hamas.consumersList[0];

            StringBuilder jesusH = new StringBuilder();
            // Get all public properties of the object
            FieldInfo[] myFields = typeof(SmartUsers.Consumers).GetFields(SmartParametersV2016.bindingFlags); // | BindingFlags.NonPublic because SQLite can't read Internal fields!!
            int index = 0;
            // Well .. SOMETHING sent across ISN'T a default date, so find out
            // which one
            try
            {
                foreach (FieldInfo myFieldInfo in myFields) // or here
                {
                    if (myFieldInfo.Name.Contains("Smart"))
                    {
                        DateTime value = Convert.ToDateTime(myFieldInfo.GetValue(consumer_row));
                        if (DateTime.Compare(Convert.ToDateTime(ourviewmodel.utcList[index]), value) != 0)
                        {
                            // Send the field name - converted to a schema and table - back!
                            // Now turn the field name into a schema + table
                            // name BECAUSE in SmartDBServer we know the schemas!
                            if (jesusH.Length > 0)
                            {
                                jesusH.Append(SmartParametersV2016.unitSeparator);
                            }
                            jesusH.Append(myFieldInfo.Name + 
                                            SmartParametersV2016.fieldSeparator +
                                            value.ToString());
                        }
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                servermodel.errorMessage = ex.Message;
                return result;
            }
            servermodel.data_stream.Append(jesusH);
            return result;
        }
        internal void AddGroup(List<SmartProfile.Groups> groups_list,
                                        string groupRecord,
                                        string username)    // fields[0]
        {
            SmartProfile.Groups group = new SmartProfile.Groups();
            string[] components = groupRecord.Split(SmartParametersV2016.fieldSeparator);
            if (components.Length < 10)
            {
                // Log, skip, or throw depending on use case
                OutputText(username, $"Invalid group record: {groupRecord}", true);
                return;
            }
            int fieldIndex = 0;
            foreach (string comp_row in components)
            {
                switch (fieldIndex)
                {
                    case 0:
                        // Sender's Username is SET in tokens_list
                        group.USERNAME = username;
                        group.GROUPNAME = comp_row;
                        break;
                    case 1:
                        group.ACTIVEFLAG = Convert.ToBoolean(comp_row);
                        break;
                    case 2:
                        group.MARKER = Convert.ToDateTime(comp_row);
                        break;
                    case 3:
                        group.PDEK = comp_row;
                        break;
                    case 4:
                        group.SENDF = Convert.ToBoolean(comp_row);
                        break;
                    case 5:
                        group.FDEK = comp_row;
                        break;
                    case 6:
                        group.SENDU = Convert.ToBoolean(comp_row);
                        break;
                    case 7:
                        group.UDEK = comp_row;
                        break;
                    case 8:
                        group.RECEIVEALL = Convert.ToBoolean(comp_row);
                        break;
                    case 9:
                        // Never get sent this - its always empty!
                        // But it shouldn't be fucking NULL
                        group.DISPLAYNAME = comp_row;
                        break;
                    default:
                        break;
                }
                fieldIndex++;
            }
            // Only Active records get in the group
            if (group.ACTIVEFLAG)
            {
                groups_list.Add(group);
            }
            return;
        }

        internal string TokenInsertionAndUpdate(TimeSpan utcOffset,
                                    ServerModel servermodel,
                                    List<RequestVerificationToken> username_list,
                                    List<SmartProfile.Groups> groups_list)
        {
            string update_token;
            // Am I already in the list
            List<RequestVerificationToken> values_list = FindValues(username_list,
                                                         servermodel.antitoken.request_value);
            if (values_list.Count == 0)
            {
                // No, I'm not .. so I need to be added
                // Is there anything in the username_list with 'True' 
                // Should either be 1 or 0 in list
                values_list = FindValueTrue(username_list);
                if (values_list.Count == 0)
                {
                    // No Trues, so add me in as one
                    servermodel.antitoken.update = true;
                    servermodel.antitoken.keepalive_timestamp = DateTimeNow(utcOffset);
                    // Does this work??
                    servermodel.antitoken.groupsList = groups_list;
                    servermodel.tokensList.Add(servermodel.antitoken);
                    update_token = servermodel.antitoken.update ? "T" : "F";
                }
                else
                {
                    // There is a True so add me in as a False
                    servermodel.antitoken.update = false;
                    servermodel.antitoken.keepalive_timestamp = DateTimeNow(utcOffset);
                    servermodel.antitoken.groupsList = groups_list;
                    servermodel.tokensList.Add(servermodel.antitoken);
                    update_token = servermodel.antitoken.update ? "T" : "F";
                }
            }
            else
            {
                // I am in the list
                // What's my status - am I in as a True
                if (values_list[0].update)
                {
                    // Yes - update my TimeStamp
                    values_list[0].keepalive_timestamp = DateTimeNow(utcOffset);
                    values_list[0].groupsList = groups_list;
                    update_token = values_list[0].update ? "T" : "F";
                }
                else
                {
                    // No - I'm in as a False
                    // ... but could I become a True
                    // Only if there isn't a single True already
                    if (FindValueTrue(username_list).Count == 0)
                    {
                        // I need changing as each list must contain 1 True
                        values_list[0].update = true;
                        // Yes - update my TimeStamp
                        values_list[0].keepalive_timestamp = DateTimeNow(utcOffset);
                        values_list[0].groupsList = groups_list;
                        update_token = values_list[0].update ? "T" : "F";
                    }
                    else
                    {
                        // Don't change status (I'm in as false
                        // But update timestamp
                        values_list[0].keepalive_timestamp = DateTimeNow(utcOffset);
                        values_list[0].groupsList = groups_list;
                        update_token = values_list[0].update ? "T" : "F";
                    }
                }
            }
            List<RequestVerificationToken> values_listX = FindValues(username_list,
                                                                     servermodel.antitoken.request_value);
            servermodel.record_count = values_listX.Count;
            return update_token;
        }

        internal string TokenRemoval(ServerModel servermodel,
                                    List<RequestVerificationToken> username_list)
        {
            string update_token = "";

            // Its a Disconnection
            // This Username is in the Tokens List
            // So remove it and make the 'next' earliest one
            // updatable
            List<RequestVerificationToken> values_list = FindValues(username_list,
                                                        servermodel.antitoken.request_value);

            foreach (RequestVerificationToken rvt in values_list)
            {
                // Yes - we need to update the keep alive login
                rvt.update = false;
                //update_token = rvt.update.ToString().Substring(0, 1);
                update_token = rvt.update ? "T" : "F";
                servermodel.tokensList.Remove(rvt);
                List<SmartProfile.Groups> mmList = new List<SmartProfile.Groups>();
                foreach (SmartProfile.Groups group_row in rvt.groupsList)
                {
                    if (group_row.GROUPNAME == group_row.USERNAME)
                    {
                        SmartProfile.Groups mm_row = new SmartProfile.Groups()
                        {
                            USERNAME = group_row.USERNAME,
                            GROUPNAME = group_row.GROUPNAME,
                            ACTIVEFLAG = group_row.ACTIVEFLAG,
                            MARKER = group_row.MARKER,
                            PDEK = group_row.PDEK,
                            SENDF = group_row.SENDF,
                            FDEK = group_row.FDEK,
                            SENDU = group_row.SENDU,
                            UDEK = group_row.UDEK,
                            RECEIVEALL = group_row.RECEIVEALL,
                            DISPLAYNAME = group_row.DISPLAYNAME
                        };
                        mmList.Add(mm_row);
                        break;
                    }
                }
                // Should leave RAY = RAY in this
                rvt.groupsList = mmList;
                break;  // Only the first Token
            }
            return update_token;
        }

        internal List<RequestVerificationToken> FindUsernames(ServerModel servermodel,
                                                                    string client)
        {
            // May return several entries or none at all
            List<RequestVerificationToken> usernameList = (from token in servermodel.tokensList
                                                           where token.username == client
                                                           select token).ToList();
            return usernameList;
        }

        internal List<RequestVerificationToken> FindUsernamesValueTrue(ServerModel servermodel,

                                                                                                string client)
        {
            // May return several entries or none at all
            List<RequestVerificationToken> usernameList = (from Tokens in servermodel.tokensList
                                                           where (Tokens.username == client &&
                                                                   Tokens.update == true)
                                                           select Tokens).ToList();
            return usernameList;
        }

        internal List<RequestVerificationToken> FindValueTrue(List<RequestVerificationToken> usernames_list)
        {
            // Should only ever return ONE entry or none at all
            List<RequestVerificationToken> values_list = (from Tokens in usernames_list
                                                          where (Tokens.update == true)
                                                          select Tokens).ToList();
            return values_list;
        }

        internal List<RequestVerificationToken> FindValues(List<RequestVerificationToken> usernames_list,
                                                        string request_value)
        {
            // Should only ever return ONE entry or none at all
            List<RequestVerificationToken> values_list = (from Username in usernames_list
                                                          where (Username.request_value == request_value)
                                                          select Username).ToList();
            return values_list;
        }

        internal void PurgeList(TimeSpan utcOffset,
                                        ServerModel servermodel,
                                        List<RequestVerificationToken> usernameList,
                                        TimeSpan killer)
        {
            if (usernameList.Count > 0)
            {
                DateTime time_now = DateTimeNow(utcOffset);
                foreach (RequestVerificationToken username_row in usernameList.ToList())
                {
                    if ((time_now - username_row.keepalive_timestamp) > killer)
                    {
                        servermodel.tokensList.Remove(username_row);
                    }
                }
            }
            return;
        }

        internal void Info_Line(string client,
                                    string what_we_display,
                                    string operation,
                                    int record_count,
                                    bool newline)
        {
            if (!string.IsNullOrEmpty(what_we_display))
            {
                if (string.IsNullOrEmpty(operation))
                {
                    OutputText(client, what_we_display, newline);
                }
                else
                {
                    OutputText_Info(client,
                                   what_we_display,
                                   operation,
                                   record_count.ToString(),
                                   newline);
                }
            }
            return;
        }

        internal char Do_Some_Decoding(ServerModel servermodel,
                                            ref List<Petulant> databases,
                                            string[] fields,
                                            ref int database_target,
                                            ref string schema_target,
                                            ref int table_target,
                                            ref string target_name,
                                            ref string targetRange,
                                            ref string requestingClient,
                                            ref string targetClient,
                                            ref string operation,
                                            ref List<string> targetRangeX)
        {
            char status = SmartParametersV2016.operationSuccess;
            try
            {
                string requester = "";
                string toperation = "";
                string tdatabase = "";
                string trange = "";
                string ttargetClient = "";
                database_target = -1;
                List<Petulant> raysDBs = new List<Petulant>();
                List<Tiresome> raysProcs = new List<Tiresome>();
                for (int tibs = 0; tibs < fields.Length; tibs++)
                {
                    switch (tibs)
                    {
                        case 0:
                            requester = fields[tibs];
                            break;
                        case 1:
                            toperation = fields[tibs];
                            break;
                        case 2:
                            switch (toperation)
                            {
                                case SmartParametersV2016.SingleProcedure:
                                    tdatabase = fields[tibs];
                                    if (string.Equals(tdatabase, tdatabase.ToUpper()))
                                    {
                                        raysDBs = databases
                                        .Where(s => s.databaseName.Equals(tdatabase, StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                                        if (raysDBs.Count > 0)
                                        {
                                            requestingClient = requester;
                                            operation = toperation;
                                            database_target = raysDBs.First().ordinal;
                                        }
                                    }
                                    break;
                                case "L":
                                    requestingClient = requester;
                                    operation = toperation;
                                    target_name = fields[tibs];
                                    break;
                                case "R":
                                    requestingClient = requester;
                                    operation = toperation;
                                    target_name = fields[tibs];
                                    break;
                                case SmartParametersV2016.connectSymbol:
                                    requestingClient = requester;
                                    operation = toperation;
                                    return status;
                                case SmartParametersV2016.disconnectSymbol:
                                    requestingClient = requester;
                                    operation = toperation;
                                    return status;
                                case "S":
                                    requestingClient = requester;
                                    operation = toperation;
                                    tdatabase = fields[tibs];
                                    break;
                                case "T":
                                    tdatabase = fields[tibs];
                                    if (string.Equals(tdatabase,tdatabase.ToUpper()))
                                    {
                                        raysDBs = databases
                                        .Where(s => s.databaseName.Equals(tdatabase, StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                                        if (raysDBs.Count > 0)
                                        {
                                            requestingClient = requester;
                                            operation = toperation;
                                            database_target = raysDBs.First().ordinal;
                                            target_name = raysDBs[0].databaseName;
                                        }
                                    }
                                    break;
                                case "P":
                                    tdatabase = fields[tibs];
                                    if (string.Equals(tdatabase,tdatabase.ToUpper()))
                                    {
                                        raysDBs = databases
                                        .Where(s => s.databaseName.Equals(tdatabase, StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                                        if (raysDBs.Count > 0)
                                        {
                                            requestingClient = requester;
                                            operation = toperation;
                                            database_target = raysDBs.First().ordinal;
                                            target_name = raysDBs[0].databaseName;
                                        }
                                    }
                                    break;
                                case "D":
                                case "I":
                                case "U":
                                    requestingClient = requester;
                                    operation = toperation;
                                    tdatabase = fields[tibs];
                                    if (toperation == "I" &&
                                        tdatabase == "SMARTMUM")
                                    {
                                        // Find the SMARTMUM index
                                        raysDBs = databases
                                        .Where(s => s.databaseName.Equals(tdatabase, StringComparison.OrdinalIgnoreCase))
                                        .ToList();
                                        if (raysDBs.Count > 0)
                                        {
                                            requestingClient = requester;
                                            operation = toperation;
                                            database_target = raysDBs.First().ordinal;
                                            target_name = raysDBs[0].databaseName;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 3:
                            trange = fields[tibs];
                            switch (toperation)
                            {
                                case SmartParametersV2016.SingleProcedure:
                                    if (!string.IsNullOrEmpty(requester) &&
                                        !string.IsNullOrEmpty(toperation) &&
                                        !string.IsNullOrEmpty(tdatabase))
                                    {
                                        if (tdatabase == tdatabase.ToUpper())
                                        {
                                            List<Tiresome> raysPRs = raysDBs[0].procedures
                                            .Where(s => s.procedure_name.Equals(trange, StringComparison.OrdinalIgnoreCase))
                                            .ToList();
                                            if (raysPRs.Count > 0)
                                            {
                                                target_name = raysPRs[0].procedure_name;
                                            }
                                        }
                                    }
                                    break;
                                case "L":
                                    targetRange = trange; // Withdrawn date
                                    break;
                                case "R":
                                    targetRange = trange; // Withdrawn date
                                    break;
                                case "S":
                                    if (!string.IsNullOrEmpty(requester) &&
                                        !string.IsNullOrEmpty(toperation) &&
                                        !string.IsNullOrEmpty(tdatabase))
                                    {
                                        if (trange == SmartParametersV2016.wildcard)
                                        {
                                            target_name = trange;
                                        }
                                    }
                                    break;
                                case "T":
                                    if (!string.IsNullOrEmpty(requester) &&
                                        !string.IsNullOrEmpty(toperation) &&
                                        !string.IsNullOrEmpty(tdatabase) &&
                                        database_target >= 0)
                                    {
                                        if (trange == SmartParametersV2016.wildcard)
                                        {
                                            targetRange = trange;
                                            return status;
                                        }
                                    }
                                    break;
                                case "P":
                                    if (!string.IsNullOrEmpty(requester) &&
                                        !string.IsNullOrEmpty(toperation) &&
                                        !string.IsNullOrEmpty(tdatabase) &&
                                        database_target >= 0)
                                    {
                                        if (trange == SmartParametersV2016.wildcard)
                                        {
                                            targetRange = trange;
                                            return status;
                                        }
                                        else
                                        {
                                            raysProcs = raysDBs[0].procedures
                                            .Where(s => s.procedure_name.Equals(trange, StringComparison.OrdinalIgnoreCase))
                                            .ToList();
                                            if (raysProcs.Count > 0)
                                            {
                                                target_name = raysProcs[0].procedure_name;
                                            }
                                        }
                                    }
                                    break;
                                case "D":
                                case "I":
                                case "U":
                                    if (!string.IsNullOrEmpty(requester) &&
                                        !string.IsNullOrEmpty(toperation) &&
                                        !string.IsNullOrEmpty(tdatabase))
                                    {
                                        string[] bob = trange.Split('.');
                                        if (bob.Length > 1)
                                        {
                                            schema_target = bob[0];
                                            target_name = bob[1];
                                            if (toperation == "I" &&
                                                tdatabase == "SMARTMUM")
                                            {
                                                table_target = -1;
                                                foreach (Cow table in raysDBs[0].tables)
                                                {
                                                    table_target++;
                                                    if (table.schema_name == schema_target &&
                                                        table.tableName == target_name)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // Should never happen?
                                            target_name = trange;
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 4:
                            switch (toperation)
                            {
                                case SmartParametersV2016.SingleProcedure:
                                    if (!string.IsNullOrEmpty(requester) &&
                                        !string.IsNullOrEmpty(toperation) &&
                                        !string.IsNullOrEmpty(tdatabase) &&
                                        !string.IsNullOrEmpty(target_name))
                                    {
                                        targetRange = fields[tibs]; // parameter
                                        return status;
                                    }
                                    break;
                                case "L":
                                    targetClient = fields[tibs]; // Admin or not
                                    return status;
                                case "R":
                                    targetClient = fields[tibs]; // Admin or not
                                    return status;
                                case "S":
                                    ttargetClient = fields[tibs];
                                    if (!string.IsNullOrEmpty(requester) &&
                                                !string.IsNullOrEmpty(toperation) &&
                                                !string.IsNullOrEmpty(tdatabase) &&
                                                !string.IsNullOrEmpty(trange))
                                    {
                                        targetClient = ttargetClient;
                                        return status;
                                    }
                                    break;
                                case "P":
                                    targetRange = fields[tibs];
                                    return status;
                                case "D":
                                case "I":
                                case "U":
                                    if (!string.IsNullOrEmpty(requester) &&
                                                !string.IsNullOrEmpty(toperation) &&
                                                !string.IsNullOrEmpty(tdatabase) &&
                                                !string.IsNullOrEmpty(target_name))
                                    {
                                        targetRangeX.Add(fields[tibs]);
                                        return status;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            switch (operation)
                            {
                                case "D":
                                case "I":
                                case "U":
                                    targetRangeX.Add(fields[tibs]);
                                    status = status;
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                status = SmartParametersV2016.operationFailure;
                servermodel.errorMessage = ex.Message;                
            }
            return status;            
        }

        internal async Task<char> Decode_InsertUpdate(ServerModel servermodel,
                                            MainViewModel ourviewmodel,
                                            SQLiteAsyncConnection connection,
                                            int databaseIndex,
                                            string requester,
                                            string schemaName,
                                            int tableIndex,
                                            string tableName,
                                            List<string> targetRecords,
                                            string operation,
                                            Action<string> set_errorMessage)
        {
            // Assume success first, always
            char result = SmartParametersV2016.operationSuccess;
            string errorMessage = "";
            if (targetRecords.Count > 0)
            {
                string[] ray = targetRecords[0].Split(SmartParametersV2016.recordSeparator);

                switch (schemaName)
                {
                    // For the Listener
                    case "SmartData":
                        servermodel.record_count = 0;
                        result = await SwitchTableName(servermodel,
                                        requester,
                                        databaseIndex,
                                        schemaName,
                                        tableIndex,
                                        tableName,
                                        requester,
                                        operation,
                                        targetRecords[0],
                                        set_errorMessage);                        
                        break;
                    case "SmartProfile":
                        switch (tableName)
                        {
                            case "Addresses":
                                List<SmartProfile.AddressesViewSQLite> addresseslist = SmartPhyllV2020.BuildSQLiteRecords<SmartProfile.AddressesViewSQLite>(servermodel, ray, requester);
                                foreach (SmartProfile.AddressesViewSQLite sqlite_row in addresseslist)
                                {
                                    result = await DoOperation<SmartProfile.AddressesViewSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = addresseslist.Count;
                                break;
                            case "Cubefaces":
                                List<SmartProfile.CubefacesSQLite> cubefaceslist = SmartPhyllV2020.BuildSQLiteRecords<SmartProfile.CubefacesSQLite>(servermodel, ray, requester);
                                foreach (SmartProfile.CubefacesSQLite sqlite_row in cubefaceslist)
                                {
                                    result = await DoOperation<SmartProfile.CubefacesSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = cubefaceslist.Count;
                                break;
                            case "Groups":
                                List<SmartProfile.GroupsSQLite> groupslist = SmartPhyllV2020.BuildSQLiteRecords<SmartProfile.GroupsSQLite>(servermodel, ray, requester);
                                foreach (SmartProfile.GroupsSQLite sqlite_row in groupslist)
                                {
                                    result = await DoOperation<SmartProfile.GroupsSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = groupslist.Count;
                                break;
                            case "Profiles":
                                List<SmartProfile.ProfilesSQLite> profileslist = SmartPhyllV2020.BuildSQLiteRecords<SmartProfile.ProfilesSQLite>(servermodel, ray, requester);
                                foreach (SmartProfile.ProfilesSQLite sqlite_row in profileslist)
                                {
                                    result = await DoOperation<SmartProfile.ProfilesSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = profileslist.Count;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "SmartFinance":
                        switch (tableName)
                        {
                            case "Accounts":
                                List<SmartFinance.AccountsSQLite> accountslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.AccountsSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.AccountsSQLite sqlite_row in accountslist)
                                {
                                    result = await DoOperation<SmartFinance.AccountsSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = accountslist.Count;
                                break;
                            case "Categories":
                                List<SmartFinance.CategoriesSQLite> categorieslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.CategoriesSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.CategoriesSQLite sqlite_row in categorieslist)
                                {
                                    result = await DoOperation<SmartFinance.CategoriesSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = categorieslist.Count;
                                break;
                            case "CategoryTypes":
                                List<SmartFinance.CategoryTypesSQLite> categorytypeslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.CategoryTypesSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.CategoryTypesSQLite sqlite_row in categorytypeslist)
                                {
                                    result = await DoOperation<SmartFinance.CategoryTypesSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = categorytypeslist.Count;
                                break;
                            case "Connections":
                                List<SmartFinance.ConnectionsSQLite> connectionslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.ConnectionsSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.ConnectionsSQLite sqlite_row in connectionslist)
                                {
                                    result = await DoOperation<SmartFinance.ConnectionsSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = connectionslist.Count;
                                break;
                            case "Logins":
                                List<SmartFinance.LoginsSQLite> loginslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.LoginsSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.LoginsSQLite sqlite_row in loginslist)
                                {
                                    result = await DoOperation<SmartFinance.LoginsSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = loginslist.Count;
                                break;
                            case "Switches":
                                List<SmartFinance.SwitchesSQLite> switcheslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.SwitchesSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.SwitchesSQLite sqlite_row in switcheslist)
                                {
                                    result = await DoOperation<SmartFinance.SwitchesSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = switcheslist.Count;
                                break;
                            case "Transactions":
                                List<SmartFinance.TransactionsSQLite> transactionslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.TransactionsSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.TransactionsSQLite sqlite_row in transactionslist)
                                {
                                    result = await DoOperation<SmartFinance.TransactionsSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = transactionslist.Count;
                                break;
                            case "TransactionsCategories":
                                List<SmartFinance.TransactionsCategoriesSQLite> transactionscategorieslist = SmartPhyllV2020.BuildSQLiteRecords<SmartFinance.TransactionsCategoriesSQLite>(servermodel, ray, requester);
                                foreach (SmartFinance.TransactionsCategoriesSQLite sqlite_row in transactionscategorieslist)
                                {
                                    result = await DoOperation<SmartFinance.TransactionsCategoriesSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = transactionscategorieslist.Count;
                                break;
                        }
                        break;
                    case "SmartUtility":
                        switch (tableName)
                        {
                            case "Accounts":
                                List<SmartUtility.AccountsSQLite> accountslist = SmartPhyllV2020.BuildSQLiteRecords<SmartUtility.AccountsSQLite>(servermodel, ray, requester);
                                foreach (SmartUtility.AccountsSQLite sqlite_row in accountslist)
                                {
                                    result = await DoOperation<SmartUtility.AccountsSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = accountslist.Count;
                                break;
                            case "Logins":
                                List<SmartUtility.LoginsSQLite> loginslist = SmartPhyllV2020.BuildSQLiteRecords<SmartUtility.LoginsSQLite>(servermodel, ray, requester);
                                foreach (SmartUtility.LoginsSQLite sqlite_row in loginslist)
                                {
                                    result = await DoOperation<SmartUtility.LoginsSQLite>(ourviewmodel,
                                                operation,
                                                schemaName,
                                                tableName,
                                                sqlite_row);
                                }
                                servermodel.record_count = loginslist.Count;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if (schemaName != "SmartData")
                {
                    string sql_update_con = "UPDATE [SmartUsers.Consumers] SET " + schemaName + tableName + " = " + // Note: no '.' bewtween them!
                                "'" + DateTimeNow(utcOffset).ToString(SmartParametersV2016.sqliteformat) +
                                "' WHERE USERNAME = '" + requester + "';";
                    if (!await SmartPhyllV2020.ExecuteSQLite(ourviewmodel, sql_update_con, em => ourviewmodel.errorMessage = em))
                    {
                        return SmartParametersV2016.operationFailure;
                    }
                }
                        
            }
            set_errorMessage(errorMessage);
            return result;
        }

        internal async Task<char> DoOperation<T>(MainViewModel ourviewmodel,
                                                        string operation,
                                                        string schemaName,
                                                        string tableName,
                                                        T sqlite_row)
        {
            switch (operation)
            {
                case "I":
                    if (!await SmartPhyllV2020.InsertSQLite(ourviewmodel,
                                        schemaName,
                                        tableName,
                                        sqlite_row))
                    {
                        return SmartParametersV2016.operationFailure;
                    }
                    break;
                case "U":
                    if (!await SmartPhyllV2020.UpdateSQLite(ourviewmodel,
                                        schemaName,
                                        tableName,
                                        sqlite_row))
                    {
                        return SmartParametersV2016.operationFailure;
                    }
                    break;
                case "D":
                    if (!await SmartPhyllV2020.DeleteSQLite(ourviewmodel,
                                        schemaName,
                                        tableName,
                                        sqlite_row))
                    {
                        return SmartParametersV2016.operationFailure;
                    }
                    break;
                default:
                    break;
            }
            return SmartParametersV2016.operationSuccess;
        }
        
        internal async Task<char> Decode_Select(ServerModel servermodel,
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            UtilityViewModel utilityviewmodel,
                                            string requestingClient,
                                            string targetClient,
                                            int database_target,
                                            string schemaName,
                                            int table_target,
                                            string targetRange,
                                            string operation,
                                            Action<string> set_errorMessage)
        {
            // Assume success first, always
            char result = SmartParametersV2016.operationSuccess;
            // Check the schema
            List<SmartData.SQLiteSchemas> raysSchemas = ourviewmodel.sqliteschemasList
                .Where(s => s.SCHEMA_NAME.Equals(schemaName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (raysSchemas.Count > 0)
            {
                // Do we want an entire DB or just a single table?
                if (targetRange == SmartParametersV2016.wildcard)
                {
                    List<SmartData.SQLiteTables> raysTables = ourviewmodel.sqlitetablesList
                    .Where(s => s.SCHEMA_NAME.Equals(schemaName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                    foreach (SmartData.SQLiteTables tableName in raysTables)
                    {
                        ourviewmodel.temp_stream = new StringBuilder();
                        switch (schemaName)
                        {
                            case "SmartUsers":
                                if (!await SmartPhyllV2020.LoadCommonUsers(ourviewmodel,
                                                                            true,
                                                                            schemaName,
                                                                            tableName.TABLE_NAME,
                                                                            requestingClient,
                                                                            targetClient, // Although we never get SmartUsers if targetClient <> requestingClient
                                                                            targetClient,
                                                                            true))
                                {
                                    result = SmartParametersV2016.operationFailure;
                                }
                                break;
                            case "SmartProfile":
                                if (!await SmartPhyllV2020.LoadCommonProfile(ourviewmodel,
                                                                            true,
                                                                            schemaName,
                                                                            tableName.TABLE_NAME,
                                                                            requestingClient,
                                                                            targetClient,
                                                                            targetClient,
                                                                            string.Empty,
                                                                            true))
                                {
                                    result = SmartParametersV2016.operationFailure;
                                }
                                break;
                            case "SmartFinance":
                                if (!await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                                            financeviewmodel,
                                                                            true,
                                                                            schemaName,
                                                                            tableName.TABLE_NAME,
                                                                            requestingClient,
                                                                            targetClient,
                                                                            targetClient,
                                                                            string.Empty,
                                                                        true))
                                {
                                    result = SmartParametersV2016.operationFailure;
                                }
                                break;
                            case "SmartUtility":
                                if (!await SmartPhyllV2020.LoadCommonUtility(ourviewmodel,
                                                                            utilityviewmodel,
                                                                            true,
                                                                            schemaName,
                                                                            tableName.TABLE_NAME,
                                                                            requestingClient,
                                                                            targetClient,
                                                                            targetClient,
                                                                            string.Empty,
                                                                            true))
                                {
                                    result = SmartParametersV2016.operationFailure;
                                }                                
                                break;
                            default:
                                break;
                        }
                        if (ourviewmodel.temp_stream.Length > 0)
                        {
                            servermodel.record_count += ourviewmodel.recordCount;
                            if (servermodel.tableCount > 0)
                            {
                                // Keep the files (tables) apart with a group separator
                                servermodel.data_stream.Append(SmartParametersV2016.groupSeparator);
                            }
                            // Make sure the table name appears as the first line ...
                            servermodel.data_stream.Append(schemaName + "." + tableName.TABLE_NAME);  
                            servermodel.data_stream.Append(SmartParametersV2016.recordSeparator);
                            servermodel.data_stream.Append(ourviewmodel.temp_stream);
                            servermodel.tableCount++;
                        }
                    }
                }
                else
                {
                    // For indivduals (from SmartSwitch) we also 'add'
                    // the SmartUsers.Consumers 'date'
                    string[] tableNames = targetRange.Split(SmartParametersV2016.unitSeparator);
                    int tables_done = 0;
                    foreach (string tableName in tableNames)
                    {
                        string sql_command;
                        if ((tableName == "EUsage") ||
                            (tableName == "GUsage"))
                        {
                            sql_command = "SELECT * FROM " + "[" + schemaName + "." + tableName + "]" + " WHERE USERNAME = '" + targetClient + "'" +
                                " ORDER BY USAGE_DATETIME";
                        }
                        else
                        {
                            sql_command = "SELECT * FROM " + "[" + schemaName + "." + tableName + "]" + " WHERE USERNAME = '" + targetClient + "'";
                        }
                        int this_record_count = 0;

                        StringBuilder temp_stream = new StringBuilder();
                        servermodel.record_count += this_record_count;
                        if (tables_done > 0)
                        {
                            // Keep the files apart with a record separator
                            servermodel.data_stream.Append(SmartParametersV2016.groupSeparator);
                        }
                        // Make sure the table name appears as the first line ...
                        servermodel.data_stream.Append(schemaName + "." + tableName);
                        if (ourviewmodel.temp_stream.Length > 0)
                        {
                            servermodel.data_stream.Append(SmartParametersV2016.recordSeparator);
                            servermodel.data_stream.Append(ourviewmodel.temp_stream);
                        }
                        tables_done++;                  
                    }
                }
            }
            if (result == SmartParametersV2016.operationFailure)
            {
                OutputText(requestingClient, "Select * failure", true);
            }
            return result;
        }
        internal bool FixAnnaDEKs(ServerModel servermodel,
                                                List<RequestVerificationToken> usernameList,
                                                string targetClient,
                                                ref SmartProfile.Groups multiuser)
        {
            // We can assume that requestingClient has requested targetClient but 
            // does targetClient allow this?
            // If it doesn't, then return nothing
            // (but it's still a success i.e SmartDBServer hasn't failed
            // If it does, then extract the FDEK or UDEK and pass it
            // back with the data so that it can be decoded on the
            // local system
            // Now the client HAS TO BE connected, otherwise .. how can
            // we know what they allow?
            // ANNA is logged in
            foreach (RequestVerificationToken rv_token in usernameList)
            {
                // For reasons I cannot BEGIN to fathom, inner_row was getting updated
                foreach (SmartProfile.Groups inner_row in rv_token.groupsList)
                {
                    // Is this the Owner's group i.e. the one with the KEY??
                    if (inner_row.USERNAME == inner_row.GROUPNAME)
                    {
                        multiuser = inner_row;
                        return true;
                    }
                }
            }
            return false;
        }
        internal bool DoesAnnaAllowRay(List<RequestVerificationToken> usernameList,
                                                string schemaName,
                                                string requestingClient,
                                                string targetClient)
        {
            // We can assume that requestingClient has requested targetClient but 
            // does targetClient allow this?
            // If it doesn't, then return nothing
            // (but it's still a success i.e SmartDBServer hasn't failed
            // If it does, then extract the FDEK or UDEK and pass it
            // back with the data so that it can be decoded on the
            // local system
            // Now the client HAS TO BE connected, otherwise .. how can
            // we know what they allow?
            // ANNA is logged in
            
            foreach (RequestVerificationToken rv_token in usernameList)
            {
                // For reasons I cannot BEGIN to fathom, inner_row was getting updated
                foreach (SmartProfile.Groups inner_row in rv_token.groupsList)
                {
                    // You've found the Major key of the GroupName
                    // you are looking for ... so what else do you need
                    // to do?
                    // Does ANNA have an entry for RAY ?
                    if (inner_row.GROUPNAME == requestingClient )
                    {
                        switch (schemaName)
                        {
                            case SmartParametersV2016.SmartUsersSchema:
                                // Always allowed
                                return true;
                            case SmartParametersV2016.SmartProfileSchema:
                                // Always allowed - but only Cubefaces and Addresses etc. not Profile(s)
                                return true;
                            case SmartParametersV2016.SmartFinanceSchema:
                                // Does ANNA allow SmartFinance data?
                                if (inner_row.SENDF)
                                {
                                    return true;
                                }
                                break;
                            case SmartParametersV2016.SmartUtilitySchema:
                                // Does Anna allow SmartUtility data
                                if (inner_row.SENDU)
                                {
                                    return true;
                                }
                                break;
                            default:
                                break;
                        }                        
                    }
                }
            }  
            return false;
        }

        internal async Task<char> Decode_Procedure(ServerModel servermodel,
                                                string ssclient,
                                                int databaseIndex,
                                                string schemaName,
                                                int cow_index,
                                                string procedure_name,
                                                string operation,
                                                string p1,
                                                Action<string> set_errorMessage)
        {
            string sql = string.Empty;
            // This SHOULD always return 'Success' as we have tested
            // ALL the procedures beforehand, and they SHOULDN'T fail!!
            // (but you can never tell ...)
            return await SwitchTableName(servermodel,
                            ssclient,
                            databaseIndex,
                            schemaName,
                            cow_index,      // Should be -1
                            procedure_name,
                            sql,
                            operation,
                            p1,
                            set_errorMessage);
        }

        internal async Task<char> Decode_Table(ServerModel servermodel,
                                    SQLiteAsyncConnection connection,
                                    string client,
                                    int databaseIndex,
                                    string schemaName,
                                    int tableIndex,
                                    string tableName,
                                    string targetRange,
                                    string[] fields,
                                    string operation,
                                    string p1,
                                    Action<string> set_errorMessage)
        {
            string sql = "";
            // Assume failure first, always
            char result = SmartParametersV2016.operationFailure;
            // Return a list of Tables - this does SmartMum/SmartProfile/SmartUtility/SmartFInance
            if (targetRange == SmartParametersV2016.wildcard)
            {
                servermodel.data_stream.Append(servermodel.databases[databaseIndex].table_data);
                servermodel.record_count = servermodel.databases[databaseIndex].tables.Count;
                result = SmartParametersV2016.operationSuccess;
            }
            else
            {
                if (fields.Length > 4)
                {
                    sql = fields[4];
                    p1 = fields[4];
                    result = await SwitchTableName(servermodel,
                                    client,
                                    databaseIndex,
                                    schemaName,
                                    tableIndex,
                                    tableName,
                                    sql,
                                    operation,
                                    p1,
                                    set_errorMessage);
                }
                else
                {
                    // Clear this down in case of provlems
                    servermodel.record_count = 0;
                    servermodel.data_stream = new StringBuilder();
                }
            }
            if (result == SmartParametersV2016.operationFailure)
            {
                OutputText(client, "Decode table failure", true);
            }
            return result;
        }

        internal async Task<char> Decode_Singles(ServerModel servermodel,
                                    SQLiteAsyncConnection connection,
                                    string requesterClient,
                                    int databaseIndex,
                                    string schemaName,
                                    int table_index,
                                    string tableName,   // Procedure name
                                    string targetRange,
                                    string targetClient,
                                    Action<string> set_errorMessage)
        {
            // Assume success first, always
            char result = SmartParametersV2016.operationSuccess;
            int procedure_record_count;
            procedure_record_count = 0;
            result = await SwitchTableName(servermodel,
                            requesterClient,
                            databaseIndex,
                            schemaName,
                            table_index,      // Should be -1
                            tableName,
                            "",//sql,
                            "P",
                            targetRange,
                            set_errorMessage);
            servermodel.record_count += procedure_record_count;
            if (result == SmartParametersV2016.operationFailure)
            {
                servermodel.data_stream = new StringBuilder();
                servermodel.record_count = 0;
                OutputText(requesterClient, "Decode singles failure " + tableName, true);
            }                
            return result;
        }

        internal void OutputText(string user, string text, bool newline = true, ConsoleColor color = ConsoleColor.White)
        {
            if (user.Length > 0 && user.Length < 18)
            {
                string user18 = user + new string(' ', 18 - user.Length);
                Trace.Write(DateTime.Now.ToString(SmartParametersV2016.sqliteformat) + //.Replace(".-", "-") +
                                                    SmartParametersV2016.tab +
                                                    user18 +
                                                    " ");
                if (color != ConsoleColor.White)
                {
                    Console.ForegroundColor = color;
                }
                if (newline)
                {
                    Trace.WriteLine(text);
                }
                else
                {
                    Trace.Write(text);
                }
                if (color != ConsoleColor.White)
                {
                    Console.ResetColor();
                }
            }
            return;
        }

        internal void OutputText_Info(string user,
                                                string text,
                                                string operation,
                                                string record_count,
                                                bool newline = true)
        {
            string user18 = user + new string(' ', 18 - user.Length);
            if (newline)
            {
                Trace.WriteLine(DateTime.Now.ToString(SmartParametersV2016.sqliteformat).Replace(".-", "-") +
                                                SmartParametersV2016.tab +
                                                user18 +
                                                " " +
                                                text +
                                                SmartParametersV2016.tab +
                                                operation +
                                                SmartParametersV2016.tab +
                                                record_count);
            }
            else
            {
                Trace.Write(DateTime.Now.ToString(SmartParametersV2016.sqliteformat).Replace(".-", "-") +
                                                                SmartParametersV2016.tab +
                                                                user18 +
                                                                " " +
                                                                text +
                                                                SmartParametersV2016.tab +
                                                                operation +
                                                                SmartParametersV2016.tab +
                                                                record_count);
            }
            return;
        }

        internal void Output_Char(char text, bool date, bool newline = true)
        {
            if (date)
            {
                Trace.Write(DateTime.Now.ToString(SmartParametersV2016.defaultCulture) + SmartParametersV2016.space);
            }
            if (newline)
            {
                Trace.WriteLine(text);
            }
            else
            {
                Trace.Write(text);
            }
            return;
        }

        internal void FourA(char result, string errorMessage, bool newline)
        {
            bool isInteractive = Environment.UserInteractive;

            switch (result)
            {
                case SmartParametersV2016.operationSuccess:                
                    if (isInteractive)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Output_Char(result, false, newline);
                    if (isInteractive)
                    {
                        Console.ResetColor();
                    }
                    break;

                case SmartParametersV2016.operationFailure:
                    if (isInteractive)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    Output_Char(result, false, string.IsNullOrEmpty(errorMessage) ? newline : false);
                    if (isInteractive)
                    {
                        Console.ResetColor();
                    }
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        Trace.WriteLine("      " + errorMessage);
                    }
                    break;

                default:
                    Output_Char(result, false, newline);
                    break;
            }
        }
        internal async Task<char> SwitchTableName(ServerModel servermodel,
                                            string ssclient,
                                            int databaseIndex,
                                            string schemaName,
                                            int tableIndex,
                                            string tableName,
                                            string sql_client,
                                            string operation,
                                            string p1,
                                            Action<string> set_errorMessage)
        {
            const string execute = "P",
                            select = "S",
                            insert = "I",
                            //update = "U",
                            //delete = "D",
                            table = "T";
            char result = SmartParametersV2016.defaultChar;

            servermodel.updatedDate = SmartParametersV2016.defaultDates;

            switch (operation)
            {
                case select:
                    // Does this fucking stupid bitch EVER shut up?
                    // with her false laugh ... and false shallow sentiment?
                    // The woman who spend £500 on herself in Turkey and bought a fucking 60p
                    // fridge magent for her sister who has cancer ??!!!???
                    //case 'EUSAGE':     // Didn't have p1 = p1.Replace line
                    //                    // What a fuck up this stuff is .. First Utility have TWO
                    //                    // fucking midnights for each day and NO entry at 2pm
                    //                    // This must be to do with daylight saving?
                    //                    // In any case we need to smooth this out and make:
                    //                    //    19-Sep-2013 23:30 => 19-Sep-2013 23:30
                    //                    //    19-Sep-2013 00:00 => 20-Sep-2013 00:00
                    //                    //    20-Sep-2013 00:00 => 20-Sep-2013 00:30
                    //                    //    20-Sep-2013 00:30 => 20-Sep-2013 01:00
                    //                    //    20-Sep-2013 01:00 => 20-Sep-2013 01:30
                    //                    //    20-Sep-2013 01:30 => 20-Sep-2013 02:00
                    //                    //                                            <== Note no entry at 2am!!!
                    //                    //    20-Sep-2013 02:30 => 20-Sep-2013 02:30
                    //                    //
                    //                    //    Otherwise the Chart3 is going to look really stupid
                    //case 'GUSAGE':     // Didn't have p1 = p1.Replace line
                    //                    // This is hard, but the alternatives are horrendous (in terms of space)
                    //                    // and performance.  for Smart meters we need to send the G_Usage
                    //                    // and have it built by joining it with G_Readings on the client side.
                    //                    // We need to join it and build it only for the Chart3 as all other
                    //                    // display is pretty meaningless.  But we have to included the Meter
                    //                    // Serial Number in Chart3 (because it changes).  We take the Supplier
                    //                    // and Account from the main display.
                    //                    // No more than 1000 lines in any one go
                    switch (tableName)
                    {
                        default:
                            //if (!string.IsNullOrEmpty(sql_client))
                            //{
                            //    result = await BuildRecordsSQLite(servermodel,
                            //                                servermodel.sqliteDatabase,
                            //                                sql_client,
                            //                                tableName,
                            //                                get_updated_date,
                            //                                set_updated_date,
                            //                                set_errorMessage);
                            //}
                            break;
                    }
                    break;
                case insert:
                    switch (tableName)
                    {
                        case "COOKIE_CONTAINER":
                            if (!string.IsNullOrEmpty(p1))
                            {
                                // We are only ever sent ONE ROW with this call
                                string[] pee_one = new string[1];
                                pee_one[0] = p1;
                                result = await Insert_Common(servermodel,
                                                    databaseIndex,
                                                    ssclient,
                                                    schemaName,
                                                    tableIndex,
                                                    tableName,
                                                    pee_one,
                                                    SmartParametersV2016.unitSeparator,
                                                    set_errorMessage);
                            }
                            break;
                        default:
                            // Its the hhhh, hh, hhhh, hhhhhh false laugh that gets me she is SUCH a fake
                            if (!string.IsNullOrEmpty(p1))
                            {
                                // Put back any & sent across as $ - might have to re-do this
                                p1 = p1.Replace('$', '&');
                                string[] pee_one = p1.Split(SmartParametersV2016.recordSeparator);

                                result = await Insert_Common(servermodel,
                                                    databaseIndex,
                                                    ssclient,
                                                    schemaName,
                                                    tableIndex,
                                                    tableName,
                                                    pee_one,
                                                    SmartParametersV2016.unitSeparator,
                                                    set_errorMessage);
                            }
                            break;
                    }
                    break;                
                case table:
                    // Only Resources?? No ... UNIT_RATES as well (I think)
                    switch (tableName)
                    {
                        // No Ba! on this table
                        default:
                            result = await ExecuteProcedure(servermodel,
                                                            ssclient,
                                                            servermodel.databases,
                                                            databaseIndex,
                                                            p1, // Not record separator you fucking idiot
                                                            tableName,
                                                            SmartParametersV2016.unitSeparator,
                                                            set_errorMessage);
                            break;
                    }
                    break;
                case execute:
                    switch (tableName)
                    {
                        case "CLEAN_BILLS":
                            // p1 should not be empty
                            // Put back any & sent across as $ - might have to re-do this
                            p1 = p1.Replace('$', '&');
                            result = await ExecuteProcedure(servermodel,
                                                ssclient,
                                                servermodel.databases,
                                                databaseIndex,
                                                p1, // Not record you fucking moron
                                                tableName,
                                                SmartParametersV2016.unitSeparator,
                                                set_errorMessage);
                            //if (result == SmartParametersV2016.operationSuccess)
                            //// Appends either !Green or !Red here
                            //{
                            //    set_data_stream(get_data_stream().Append(execute_stream));
                            //}
                            break;
                        case "CLEAN_USER":
                            // p1 should be Empty
                            result = await ExecuteProcedure(servermodel,
                                                ssclient,
                                                servermodel.databases,
                                                databaseIndex,
                                                p1,  // Should be empty, always
                                                tableName,
                                                SmartParametersV2016.unitSeparator,
                                                set_errorMessage);
                            //if (result == SmartParametersV2016.operationSuccess)
                            //// Appends either !Green or !Red here
                            //{
                            //    set_data_stream(get_data_stream().Append(execute_stream));
                            //}
                            break;
                        case "TIDY_USER":
                            // p1 should be Empty
                            result = await ExecuteProcedure(servermodel,
                                                ssclient,
                                                servermodel.databases,
                                                databaseIndex,
                                                p1,  // Should be empty, always
                                                tableName,
                                                SmartParametersV2016.unitSeparator,
                                                set_errorMessage);
                            //if (result == SmartParametersV2016.operationSuccess)
                            //// Appends either !Green or !Red here
                            //{
                            //    set_data_stream(get_data_stream().Append(execute_stream));
                            //}
                            break;
                        case "CLONE_USER":
                            // p1 should be Empty
                            result = await ExecuteProcedure(servermodel,
                                                ssclient,
                                                servermodel.databases,
                                                databaseIndex,
                                                p1,  // Should be empty, always
                                                tableName,
                                                SmartParametersV2016.unitSeparator,
                                                set_errorMessage);
                            //if (result == SmartParametersV2016.operationSuccess)
                            //// Appends either !Green or !Red here
                            //{
                            //    set_data_stream(get_data_stream().Append(execute_stream));
                            //}
                            break;
                        default:
                            // p1 should not be Empty
                            result = await ExecuteProcedure(servermodel,
                                                ssclient,
                                                servermodel.databases,
                                                databaseIndex,
                                                p1,  // Should contain the parameters - always
                                                tableName,
                                                SmartParametersV2016.unitSeparator,
                                                set_errorMessage);
                            if (result == SmartParametersV2016.operationSuccess)
                            {
                                Console.WriteLine("Success:" + tableName);
                                if (tableName == "LOAD_UNIT_RATES")// No Ba! on this table, which IS a 'special' ..
                                {
                                    if (servermodel.data_stream.Length > 0)
                                    {
                                        // Jesus if this next line works, it'll be a fucking MIRACLE!!

                                        servermodel.data_stream.Insert(0, servermodel.updatedDate + SmartParametersV2016.recordSeparator); // Should be the latest Updated date!!

                                        servermodel.data_stream.Insert(0, servermodel.withdrawnDate + SmartParametersV2016.recordSeparator); // Should be the withdrawn date from the start
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Failure:" + tableName);

                            }
                            break;
                    }
                    break;
                default:
                    switch (tableName)
                    {
                        // Eric Bandana!!  Kate Blanchard!!! What EVER does go in in shit-for-brains-3's mind?
                        // Withdrawn date is sent with the 'dummy' Unit_Rates loaded up in SMARTUTILITY

                        // All the Tariff tables
                        // Yes but she forgets that last night (09-Dec-14), she was thumping and beating and
                        // lashing the bed
                        // saying something like 'I cant fucking get to sleep' and she was thumping
                        // the bed with her legs and waking me up.  ' wish I could get some fucking sleep'.
                        // SHe is an inconsiderate, Petulant,
                        // aggressive, rude, assertive bitch.  Last night she was going to fucking
                        // ambush me about tea but I pre-empted her by saying I could eat an omlette.
                        // She was on the verge of blanking me - she can dish it out but she can't take it ..
                        default:
                            if (!string.IsNullOrEmpty(sql_client))
                            {                                
                            }
                            break;
                    }
                    break;
            }
            return result;
        }
        
        internal async Task<char> BuildRecords(ServerModel servermodel,
                                                string sql,
                                                int databaseIndex,
                                                int schemaIndex,
                                                string tableName,
                                                Action<StringBuilder> set_data_stream,
                                                Action<string> set_errorMessage)
        {
            // This routine has been changed .. because we now send anything text as TEXT
            // Assume success first, just this once ...
            char result = SmartParametersV2016.operationSuccess;
            // Ensure we are pointing to the right schema
            sql = sql.Replace(" FROM ", " FROM " + servermodel.databases[databaseIndex].schemas[schemaIndex].schema_name + ".");
            sql = sql.Replace("JOIN ", " JOIN " + servermodel.databases[databaseIndex].schemas[schemaIndex].schema_name + ".");
            sql = sql.Replace(servermodel.databases[databaseIndex].schemas[schemaIndex].schema_name + "." + "(", "(");
            // Initialize these three
            set_data_stream(new StringBuilder());
            //int record_count = get_record_count();
            if (sql.Contains("WHERE USERNAME = "))
            {
                sql = "BEGIN TRANSACTION; " + sql;
                sql = sql.Replace("WHERE USERNAME = ", "WHERE USERNAME = N");
                sql += " OPTION(RECOMPILE); ROLLBACK;";
            }
            using SqlCommand sql_command = new()
            {
                Connection = servermodel.databases[databaseIndex].connection,
                CommandTimeout = SmartParametersV2016.commandTimeout,
                CommandText = sql
            };
            string errorMessage = string.Empty;
            try
            {
                using (SqlDataReader sql_reader = await sql_command.ExecuteReaderAsync())
                {
                    if (sql_reader.HasRows)
                    {
                        set_data_stream(await Core_Read(servermodel,
                                                    sql_reader,
                                                    string.Empty,
                                                    false,
                                                    set_errorMessage));                        
                    }
                    sql_reader.Close();
                }
            }
            catch (SqlException)
            {
                result = SmartParametersV2016.operationFailure;
            }
            set_errorMessage(errorMessage);
            return result;
        }
        internal async Task<StringBuilder> Core_Read(ServerModel servermodel,
                                                SqlDataReader sql_reader,
                                                string procedure_name,
                                                bool send_username,
                                                Action<string> set_errorMessage_out)
        {
            set_errorMessage_out(""); // still unused
            StringBuilder data_stream = new StringBuilder();
            HashSet<string> excludedFields = new(StringComparer.OrdinalIgnoreCase)
            {
                "Created", "Updated", "Delete", "Status_Flag"
            };
            try
            {
                int record_count = 0;
                while (await sql_reader.ReadAsync())
                {
                    StringBuilder data_string = new();
                    bool first_field = true;

                    for (int i = 0; i < sql_reader.FieldCount; i++)
                    {
                        string fieldName = sql_reader.GetName(i);

                        // Handle special exclusions
                        if (fieldName.Equals("USERNAME", StringComparison.OrdinalIgnoreCase) && !send_username)
                        {
                            continue;
                        }
                        if (fieldName.Equals("Deactivated", StringComparison.OrdinalIgnoreCase))
                        {
                            goto SkipRow;
                        }
                        bool includeField = true;

                        // Exclude fields by name (case-insensitive)
                        if (!fieldName.Equals(fieldName.ToUpper(), StringComparison.Ordinal)) // Only for mixed/lowercase fields
                        {
                            // Special logic for LOAD_UNIT_RATES and Updated
                            if (includeField && procedure_name == "LOAD_UNIT_RATES" && fieldName == "Updated")
                            {
                                if (servermodel.updatedDate == SmartParametersV2016.defaultDates)
                                {
                                    servermodel.updatedDate = Check_Date(sql_reader.GetValue(i));
                                }
                            }
                            if (excludedFields.Contains(fieldName))
                            {
                                includeField = false;
                            }
                        }

                        if (!includeField)
                        {
                            continue;
                        }
                        // Handle nulls
                        var rawValue = sql_reader.IsDBNull(i) ? string.Empty : sql_reader.GetValue(i);
                        string stringValue;

                        // Handle type-specific formatting
                        if (rawValue is DateTime dt)
                        {
                            stringValue = Check_Date(dt);
                        }
                        else
                        {
                            stringValue = rawValue.ToString();
                        }
                        if (!first_field)
                        {
                            data_string.Append(SmartParametersV2016.fieldSeparator);
                        }
                        data_string.Append(stringValue);
                        first_field = false;
                    SkipRow:
                        continue;
                    }

                    if (data_stream.Length > 0)
                    {
                        data_stream.Append(SmartParametersV2016.unitSeparator);
                    }
                    data_stream.Append(data_string);
                    record_count++;
                }
                servermodel.record_count = record_count;
            }
            catch (Exception ex)
            {
                set_errorMessage_out(ex.Message);
            }
            return data_stream;
        }

        //There is no way I can think with this fucking idiot blathering down the phone to her sister
        internal string Check_Date(object db_date)
        {
            string date = Convert.ToDateTime(db_date).ToString(SmartParametersV2016.sqldateFormat);
            if (date.Length >= 19)
            {
                if (date.Substring(11, 8) == "00:00:00")
                {
                    date = date.Substring(0, 10);
                }
            }
            return date;
        }

        internal async Task<char> ExecuteProcedure(ServerModel servermodel,
                                                        string client,
                                                        List<Petulant> databases,
                                                        int databaseIndex,
                                                        string parameters,      // Can contain many parameters
                                                        string procedure_name,
                                                        char parameter_split,
                                                        Action<string> set_errorMessage)
        {
            // Assume failure first
            char result = SmartParametersV2016.operationSuccess;
            string errorMessage = string.Empty;
            // AND no records
            string[] p1 = parameters.Split(parameter_split);

            // Some house-keeping Procedures may need this, but we can always
            // put 'COMMITs' in those procedures can't we??

            using SqlCommand sql_command = new()
            {
                Connection = databases[databaseIndex].connection,
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = SmartParametersV2016.commandTimeout * 2,
                CommandText = procedure_name
            };
            SqlCommandBuilder.DeriveParameters(sql_command);

            int p1_count = 0;
            bool enough_parameters = true;

            foreach (SqlParameter parameter in sql_command.Parameters)
            {
                switch (parameter.ParameterName)
                {
                    case "@RETURN_VALUE":
                        break;
                    case "@username":
                        parameter.Value = client;
                        break;
                    default:
                        if (p1.Length > p1_count)
                        {
                            parameter.Value = p1[p1_count];
                            p1_count++;
                        }
                        else
                        {
                            enough_parameters = false;
                        }
                        break;
                }
            }

            // Miserable Bitch is clomping and stomping around the kitchen dropping
            // shit like the clumsy oaf she is and moaning ... I give up
            // If we had enough parameters then execute the procedure, otherwise return nothing
            if (enough_parameters)
            {
                try
                {
                    SqlDataReader sql_reader = await sql_command.ExecuteReaderAsync();
                    if (sql_reader.HasRows)
                    {
                        servermodel.data_stream = (await Core_Read(servermodel,
                                                sql_reader,
                                                procedure_name,
                                                false,
                                                set_errorMessage));
                    }
                    sql_reader.Close();

                    // Need - for some obscure reason - to check this AFTER the reader code above (then it works)
                    int return_value = -1;
                    bool dorealbreak = false;

                    foreach (SqlParameter parameter in sql_command.Parameters)
                    {
                        switch (parameter.ParameterName)
                        {
                            case "@RETURN_VALUE":
                                return_value = (int)parameter.Value;
                                dorealbreak = true;
                                break;
                            default:
                                break;
                        }
                        if (dorealbreak)
                        {
                            break;
                        }
                    }
                    if (return_value > 0)
                    {
                        result = SmartParametersV2016.operationFailure;
                        // We failed - clear this down 'cos it might contain something
                        servermodel.data_stream = new StringBuilder();
                    }
                }
                catch (SqlException sqlexception)
                {
                    errorMessage = sqlexception.Message;
                    Fixup_Sql_Error(sqlexception, ref errorMessage);
                    result = SmartParametersV2016.operationFailure;
                    // Result may be !Red here
                }
            }
            set_errorMessage(errorMessage);
            return result;
        }

        internal async Task<char> Insert_Common(ServerModel servermodel,
                                        int databaseIndex,
                                        string ssclient,
                                        string schemaName,
                                        int tableIndex,
                                        string tableName,
                                        string[] rows,      // Can contain many lines
                                        char unit_split,    // Unit_Separator as far as I can see
                                        Action<string> set_errorMessage)
        {
            char result = SmartParametersV2016.operationSuccess;
            string errorMessage = string.Empty;
            int record_count = servermodel.record_count;
            // Do we have something to do ...
            if (rows.Length > 0)
            {
                string full_sql = string.Empty,
                    insert_sql,
                    comma = SmartParametersV2016.comma;
                insert_sql = "INSERT INTO " + schemaName + "." + tableName + " (";
                int column_count = 0;
                foreach (Uppance field_row in servermodel.databases[databaseIndex].tables[tableIndex].fields)
                {
                    if (column_count > 0)
                    {
                        insert_sql += comma;
                    }
                    insert_sql += field_row.column_name;
                    column_count += 1;
                }
                insert_sql += ")";

                using SqlCommand sql_command = new()
                {
                    Connection = servermodel.databases[databaseIndex].connection,
                    CommandTimeout = SmartParametersV2016.commandTimeout
                };
                int row_count = 0;
                int insert_limit = 0;
                while (row_count < rows.Length)
                {
                    if (string.IsNullOrEmpty(full_sql))
                    {
                        full_sql = insert_sql + " VALUES (";
                    }
                    else
                    {
                        full_sql += ",(";
                    }

                    // Username is always data_type nvarchar which means it SHOULD
                    // HAVE preceding and trailing ' characters around it ...
                    string character_maximum;
                    string dataDelimiter;

                    // BUG!! Its field_split in the next line!!!!!!
                    // And you have to be SO CAREFUL because if you send data which
                    // has 'field_split' embedded in it, then you will blow this section up!
                    // You need to send DETAILS = "ABC" + '|' + "DEF
                    // not DETAILS = "ABC" + field_split + "DEF"

                    string[] values;
                    if (!string.IsNullOrEmpty(ssclient))
                    {
                        values = (ssclient + unit_split + rows[row_count]).Split(unit_split);
                    }
                    else
                    {
                        values = (rows[row_count]).Split(unit_split);
                    }

                    int value_count = 0;
                    while (value_count < values.Length)
                    {
                        character_maximum = servermodel.databases[databaseIndex].tables[tableIndex].fields[value_count].character_maximum;
                        dataDelimiter = servermodel.databases[databaseIndex].tables[tableIndex].fields[value_count].dataDelimiter;
                        if (value_count > 0)
                        {
                            full_sql += comma;
                        }
                        if (!string.IsNullOrEmpty(character_maximum))
                        {
                            // If there is a comma embedded in the char max, then it means a numeric not a string!
                            if (!character_maximum.Contains(comma, StringComparison.CurrentCulture))
                            {
                                // Check to see that our dataDelimiter isn't passed in the string
                                if (values[value_count].Contains(dataDelimiter, StringComparison.CurrentCulture))
                                {
                                    // Otherwise the SQL will fall over.  So weed them out ...
                                    values[value_count] = values[value_count].Replace(dataDelimiter, SmartParametersV2016.delimiterSubstitute);
                                }
                                int max_length = Convert.ToInt32(character_maximum);
                                // This takes care of nvarchar(max) types which return max_length = -1 because character_maximum is "-1"
                                if (max_length == -1)
                                {
                                    max_length = values[value_count].Length;
                                }
                                if (values[value_count].Length > max_length)
                                {
                                    full_sql += dataDelimiter + values[value_count].Substring(0, max_length) + dataDelimiter;
                                    goto next;
                                }
                                else
                                {
                                    full_sql += dataDelimiter + values[value_count] + dataDelimiter;
                                    goto next;
                                }
                            }
                        }
                        full_sql += dataDelimiter + values[value_count] + dataDelimiter;
                    next:
                        value_count++;
                    }

                    full_sql += ")";
                    insert_limit++;
                    row_count++;
                    // This 1000 is a SQL Server limit and it should *never*
                    // happen for us ... except for (perhaps) BankTransactions
                    // So we are going to have to live with the fact that
                    // Because I am trying (very hard) to store the UtcNow
                    // date and time of the LAST Commit (which is at the end)
                    if (insert_limit == 1000)
                    {
                        // Now do the insert of all the rows in this batch
                        sql_command.CommandText = full_sql;
                        try
                        {
                            int this_count = await sql_command.ExecuteNonQueryAsync();
                            record_count += this_count;
                        }
                        catch (SqlException sqlexception)
                        {
                            set_errorMessage(sqlexception.Message);
                            Fixup_Sql_Error(sqlexception, ref errorMessage);
                            // This is pretty useless in a catch
                            // lets hope the outer call clears it up
                            // Try and Rollback
                            // Data string may be !Red here
                            result = SmartParametersV2016.operationFailure;
                            break;
                        }
                        catch (InvalidOperationException exception)
                        {
                            errorMessage = exception.Message;
                            // Data string should be !Red here
                            result = SmartParametersV2016.operationFailure;
                            break;
                        }
                        insert_limit = 0;           // Bashing, crashing, bashing, bustling
                        full_sql = string.Empty;    // BUstling Bertha is in full maximum noisy bustle ...
                    }
                }
                if (result == SmartParametersV2016.operationSuccess)
                {
                    // Now do the insert of all the remaining rows in the batch
                    sql_command.CommandText = full_sql;

                    
                    //
                    // No the previous discussion(s) never happen because the Green and Red
                    // USERNAME(s) system prevent multiple updater(s) using the same USERNAME
                    // Therefore we are *never* going to get two people with the same USERNAME
                    // updating the same data at the same time; only ONE Green USERNAME as an
                    // 'upadater' is ever allowed.
                    // Also Insert_Common is only ever used for Cookie_Container and Listener
                    try
                    {
                        int this_count = await sql_command.ExecuteNonQueryAsync();
                        // So Listener inserts aren't shown with '*' and not '#'
                        record_count += this_count;                        
                        servermodel.record_count = record_count;
                    }
                    catch (SqlException sqlexception)
                    {
                        errorMessage = sqlexception.Message;
                        Fixup_Sql_Error(sqlexception, ref errorMessage);
                        // Try and Rollback no good in a catch
                        // Operation string may be !Red here
                        result = SmartParametersV2016.operationFailure;

                    }
                    catch (InvalidOperationException exception)
                    {
                        // Operation string should be !Red here
                        errorMessage = exception.Message;
                        result = SmartParametersV2016.operationFailure;
                    }
                }
            }
            set_errorMessage(errorMessage);
            return result;
        }

        internal void Fixup_Sql_Error(SqlException sqlexception, ref string errorMessage)
        {
            if (sqlexception.Errors.Count > 0) // Assume the interesting stuff is in the first error
            {
                switch (sqlexception.Errors[0].Number)
                {
                    case 109: // More COLUMNS than VALUES - not enough data sent .. perhaps SmartSwitch DB has changed?
                        errorMessage = "More COLUMNS than VALUES";
                        break;
                    case 207: // Invalid COLUMN name .. perhaps SmartSwitch Db has changed?
                              // And this alsoo happens if you get the columns in the wrong order!
                              // Seems that if you add 502 columns with the data in the wrong order
                              // .. you get 502 error messages!!! So we just need the first one, thanks SQL ..
                        int cr_index = errorMessage.IndexOf(SmartParametersV2016.carriageReturn.ToString(), StringComparison.CurrentCulture);
                        if (cr_index > 0)
                        {
                            errorMessage = errorMessage.Substring(0, cr_index);
                        }
                        break;
                    case 547: // Foreign Key violation
                        errorMessage = "Foreign Key Violation";
                        break;
                    case 2627: // Primary key violation
                        errorMessage = "Primary Key violation";
                        break;
                    default:
                        errorMessage = "Unknown error";
                        break;  // Leave it at unsure
                }                
            }
            return;
        }

        internal async Task<char> Update_Common(ServerModel servermodel,
                                        int databaseIndex,
                                        string schemaName,
                                        int tableIndex,
                                        string p1,  // For Resources its P3, for COOKIES its P2 For Accounts its SQL Hey ho ..
                                        string tableName,
                                        string client = null,
                                        List<string> utctables = null)
        {
            // Assume success first
            char result = SmartParametersV2016.operationSuccess;
            
            // UPDATE ONLY EVER DOES **ONE** SQL RECORD AT A TIME
            // But we can do lots of 'Single' UPDATEs
            string errorMessage = string.Empty;
            int localRecordCount = 0;
            // Sanity check ...
            if (servermodel.databases[databaseIndex].tables[tableIndex].keys.Count == 0)
            {
                errorMessage = "Table " +
                                servermodel.databases[databaseIndex].tables[tableIndex].ToString() +
                                " has no keys";
            }
            else
            {
                // p1 DOESN'T contain the Username ... AND IT MAY NEVER!! YOU DOLT!!!! 
                string[] records = p1.Split(SmartParametersV2016.recordSeparator);
                foreach (string record_string in records)
                {
                    // ... so we stick it on here
                    string vals = record_string;
                    if (!string.IsNullOrEmpty(client))
                    {
                        vals = client + SmartParametersV2016.unitSeparator + vals;
                    }
                    string[] values = vals.Split(SmartParametersV2016.unitSeparator);

                    string where_sql = " WHERE ";
                    int key_count = 0;
                    string dataDelimiter;

                    foreach (Come key_row in servermodel.databases[databaseIndex].tables[tableIndex].keys)
                    {
                        if (key_count > 0)
                        {
                            where_sql += " AND ";
                        }
                        where_sql = where_sql + key_row.column_name + " = ";
                        dataDelimiter = servermodel.databases[databaseIndex].tables[tableIndex].fields[key_count].dataDelimiter;
                        where_sql += dataDelimiter + values[key_count] + dataDelimiter;
                        key_count++;
                    }

                    string sql_update = "UPDATE " + schemaName + "." + tableName + " SET ";
                    int field_ordinal = 0;
                    string comma = string.Empty;

                    foreach (Uppance field_row in servermodel.databases[databaseIndex].tables[tableIndex].fields)
                    {
                        if (field_ordinal >= values.Length)
                        {
                            break;
                        }
                        else
                        {
                            // Eliminate any primary keys
                            if (field_ordinal >= key_count)
                            {
                                if (string.IsNullOrEmpty(comma))
                                {
                                    comma = SmartParametersV2016.comma;
                                }
                                else
                                {
                                    sql_update += comma;
                                }
                                sql_update = sql_update + field_row.column_name + " = " +
                                        "@VALUE" + Convert.ToString(field_ordinal);
                            }
                        }
                        field_ordinal++;
                    }
                    sql_update += where_sql;

                    // Build the Command parameters
                    using SqlCommand sql_command = new()
                    {
                        Connection = servermodel.databases[databaseIndex].connection,
                        CommandTimeout = SmartParametersV2016.commandTimeout,
                        CommandText = sql_update
                    };

                    field_ordinal = 0;
                    while (field_ordinal < values.Length)
                    {
                        if (field_ordinal >= key_count)
                        {
                            sql_command.Parameters.AddWithValue("@VALUE" +
                                                            Convert.ToString(field_ordinal),
                                                            values[field_ordinal]); // How did this EVER work before?
                        }
                        field_ordinal++;
                    }

                    // See the rationale above for insert_column_multi
                    try
                    {
                        int sql_count = await sql_command.ExecuteNonQueryAsync();
                        localRecordCount += sql_count;
                        servermodel.record_count = localRecordCount;
                    }
                    catch (SqlException sqlexception)
                    {
                        errorMessage = sqlexception.Message;
                        Fixup_Sql_Error(sqlexception, ref errorMessage);
                        // Try and Rollback
                        // Pretty useless in a catch.
                        // Hope the calling program does this for us
                        result = SmartParametersV2016.operationFailure;
                    }
                    catch (InvalidOperationException exception)
                    {
                        errorMessage = exception.Message;
                        result = SmartParametersV2016.operationFailure;
                    }
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        // There was a problem ... give up
                        break;
                    }
                }
                if (string.IsNullOrEmpty(errorMessage) &&
                    result == SmartParametersV2016.operationSuccess)
                {
                    if (databaseIndex == servermodel.smartswitchIndex)
                    {
                        if (utctables != null)
                        {
                            // Field names don't have '.' in them - bad practice, really, even I think that
                            utctables.Add(schemaName + tableName);
                        }
                    }
                }
            }
            servermodel.errorMessage = errorMessage;
            return result;
        }

        // That's the end of my day .... this fucking cow is going to BASH AND CRASH
        // AND THUMP AND LUMP and disturb me until I give up.  Why don't you put the
        // fucking radio on as well you fucking moron???? SHE FUCKING DID!!!!!!!!!!!!!
        internal async Task<StringBuilder> Build_Database_TablesX(ServerModel servermodel,
                                                        string ssclient,
                                                        int databaseIndex,
                                                        Func<int> get_tables_count,
                                                        Action<int> set_tables_count,
                                                        Action<string> set_errorMessage)
        {
            string errorMessage = string.Empty;
            String[] fields = Array.Empty<String>();
            StringBuilder database_tables = new();
            string p1,
                    p2 = string.Empty,
                    p3 = string.Empty,
                    operation = "T";
            char result = SmartParametersV2016.operationFailure;

            foreach (Cow tableItem in servermodel.databases[databaseIndex].tables)
            {
                // Don't build full UNIT_RATES ... its not needed only the Area Specific version
                // is required and thats done by LOAD_UNIT_RATES
                p1 = string.Empty;
                Tiresome procedure_item = servermodel.databases[databaseIndex].procedures
                        .FirstOrDefault(p => p.procedure_name == tableItem.tableName);
                if (procedure_item != null)
                {
                    // Until I THINK ABOUT IT, I can only cope with TWO loading parameters as
                    // of today because my brain aches ...
                    if (procedure_item.parameters > 0)
                    {
                        foreach (Drivel schema in servermodel.databases[databaseIndex].schemas)
                        {
                            switch (schema.schema_name)
                            {
                                case "SmartProfile":
                                    p1 = SmartParametersV2016.Profiles.ToString();
                                    break;
                                case "SmartFinance":
                                    p1 = SmartParametersV2016.Finance.ToString();
                                    break;
                                case "SmartUtility":
                                    p1 = SmartParametersV2016.Utility.ToString();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                        if (p1 == "U" && procedure_item.procedure_name == "UNIT_RATES")
                        {
                            p1 = servermodel.withdrawnDate.ToString();
                        }
                        else
                        {
                            if (procedure_item.parameters > 1)
                            {
                                p1 = p1 + SmartParametersV2016.unitSeparator + servermodel.withdrawnDate;
                            }
                        }
                    }

                    // Here we go again - this fucking noisy idiot banging on giving me an unwanted
                    // running commentary on her stupid life, bashing and crashing around the kitchen
                    // The usual TOTAL fucking distraction
                    
                    result = await Decode_Procedure(servermodel,
                                    ssclient,
                                    databaseIndex,
                                    "",  // Procedures don't have a Schema
                                    -1,  // or a Table
                                    tableItem.tableName, // This is the procedure_name
                                    operation,
                                    p1,
                                    em => errorMessage = em);

                    string display_target = tableItem.schema_name + "." + tableItem.tableName;

                    Info_Line(ssclient, display_target, operation, servermodel.record_count, false);
                    if (!string.IsNullOrEmpty(display_target))
                    {
                        FourA(result, errorMessage, true);
                    }
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        // There was a problem - return what we-ve got
                        return database_tables;
                    }
                    servermodel.databases[databaseIndex].record_count = servermodel.databases[databaseIndex].record_count + get_tables_count();
                    if (database_tables.Length > 0)
                    {
                        // Keep the files apart with a Group Separator
                        database_tables.Append(SmartParametersV2016.groupSeparator);
                    }
                    // Make sure the table name appears as the first line ...
                    database_tables.Append(tableItem.schema_name + SmartParametersV2016.period + tableItem.tableName);
                    if (servermodel.data_stream.Length > 0)
                    {
                        database_tables.Append(SmartParametersV2016.recordSeparator);
                        database_tables.Append(servermodel.data_stream);
                    }
                    // Don't forget to clear this down ...
                    servermodel.data_stream.Clear();
                }
            }
            set_errorMessage(errorMessage);
            return database_tables;
        }

        public void Dispose()
        {
            if (ctl != null)
            {
                ctl.Dispose();
            }
            if (master_connection != null)
            {
                master_connection.Dispose();
            }
            if (tr1 != null)
            {
                tr1.Dispose();
            }
            //GC.SuppressFinalize(this);
        }

        internal async Task<char> UpdateConsumer(ServerModel servermodel,
                                            MainViewModel ourviewmodel,
                                            SQLiteAsyncConnection connection,
                                            string client,
                                            string tableName,
                                            Action<string> set_consumer_record,
                                            DateTime utcTime,
                                            List<string> utctables)
        {
            // Assume failure first, always
            char result = SmartParametersV2016.operationFailure;
            //string errorMessage = string.Empty;
            string returned_param = string.Empty;
            string param1 = string.Empty;

            StringBuilder dataStream = new();

            ourviewmodel.UserName = client;
            if (!await SmartPhyllV2020.GetDates(ourviewmodel))
            { 
                result = SmartParametersV2016.operationFailure;
                return result;
            }
            
            //// There may be more than one 'success' code!
            //if (result == SmartParametersV2016.operationFailure)
            //{
            //    return result;
            //}
            string[] feelds = ourviewmodel.utcDates.Split(SmartParametersV2016.fieldSeparator);


            List<SmartData.SQLiteFields> consrecords = ourviewmodel.sqlitefieldsList
                                        .Where(f =>
                                            string.Equals(f.TABLE_NAME, "Consumers") &&
                                            string.Equals(f.SCHEMA_NAME, "SmartUsers"))
                                        .ToList();
            // Construct the SQL
            string sql = "UPDATE " + "[" + "SmartUsers.Consumers" + "]";
            sql = sql + " SET ";
            FieldInfo[] myFields = typeof(SmartUsers.Consumers).GetFields(SmartParametersV2016.bindingFlags); // | BindingFlags.NonPublic because SQLite can't read Internal fields!!
            foreach (string utcTable in feelds)
            {
                for (int i = 0; i < myFields.Length; i++) // or here
                {
                    if (myFields[i].Name == utcTable)
                    {
                        // -1 because of the feelds doesn't hold a value for USERNAME
                        // Format the string HERE rather than on the next line into 'feelds'
                        // This is because there was a bit of milliseconds 'creep' added to
                        // the utcValue value when it was stored in SQL Server.  I think
                        // the problem MAY have been that the Date/Time conversion in
                        // SQL Server was slightly different from that in C#? I could well
                        // be wrong, but I couldn't for the life of me store a Date/Time
                        // WITH milliseconds AND get the returned value the same to the
                        // EXACT millisecond.  BUT if I did the millisecond conversion FIRST
                        // then I would be storing a (long)number which wouldn't need
                        // converting.  Go figure ... I spent days trying to get the two
                        // millisecond values from Date/Time conversions to match without success!
                        feelds[i] = utcTime.ToString(SmartParametersV2016.sqldateFormat); // We have to use this format in SQLite local
                        if (!string.IsNullOrEmpty(param1))
                        {
                            returned_param += SmartParametersV2016.groupSeparator;
                        }
                        returned_param += utcTable +
                                    SmartParametersV2016.unitSeparator +
                                    feelds[i];
                        break;
                    }
                }
            }

            // Now update CONSUMERS **Ray**
            if (feelds.Length > 0)
            {
                // Exclude the UserName - it gets 'tacked on' in the next routine
                for (int i = 1; i < feelds.Length; i++)
                {
                    if (!string.IsNullOrEmpty(param1))
                    {
                        param1 += SmartParametersV2016.unitSeparator;
                    }
                    param1 += feelds[i];
                }
            }
            sql = sql + " WHERE USERNAME = " + "'" + client + "';";
            result = SmartParametersV2016.operationFailure;            
            // There may be more than one 'success' code!
            if (result != SmartParametersV2016.operationFailure)
            {
                set_consumer_record(returned_param);
            }
            return result;
        }
        internal async Task<List<SmartData.Currencies>> GetCurrencies(ServerModel servermodel,
                                                                string client,
                                                                int databaseIndex,
                                                                int schemaIndex,
                                                                Action<string> set_errorMessage)
        {
            char result = SmartParametersV2016.defaultChar;
            List<SmartData.Currencies> currencies_list = new();
            string sql = "SELECT * FROM CURRENCIES;";

            try
            {
                StringBuilder data_stream = new();
                result = await BuildRecords(servermodel,
                                    sql,
                                    databaseIndex,
                                    schemaIndex,
                                    "CURRENCIES",
                                    ds => data_stream = ds,
                                    set_errorMessage);
                if (result != SmartParametersV2016.operationFailure &&
                    data_stream.Length > 0)
                {
                    string[] currency_records = data_stream.ToString().Split(SmartParametersV2016.unitSeparator);
                    foreach (string currency_record in currency_records)
                    {
                        string[] currency_fields = currency_record.Split(SmartParametersV2016.fieldSeparator);
                        if (currency_fields.Length >= 3)
                        {
                            SmartData.Currencies def = new()
                            {
                                ISOCURRENCYSYMBOL = currency_fields[0],
                                ORDINAL = Convert.ToInt16(currency_fields[1]),
                                DESCRIPTION = currency_fields[2]
                            };
                            currencies_list.Add(def);
                            OutputText(client,
                                        def.ISOCURRENCYSYMBOL + " " +
                                        def.ORDINAL + " " +
                                        def.DESCRIPTION, true);
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                OutputText(client, sqlex.Message, true);
            }
            return currencies_list;
        }

        internal async Task<List<SmartData.SQLiteTables>> GetSQLiteTables(ServerModel servermodel,
                                                        string client,
                                                        int databaseIndex,
                                                        int schemaIndex,
                                                        string tableName,
                                                        Action<string> set_errorMessage)
        {
            char result = SmartParametersV2016.defaultChar;
            List<SmartData.SQLiteTables> sqlitetables_list = new();
            string sql = "SELECT * FROM " + tableName + ";";
            try
            {
                StringBuilder data_stream = new();
                result = await BuildRecords(servermodel,
                                    sql,
                                    databaseIndex,
                                    schemaIndex,
                                    tableName,
                                    ds => data_stream = ds,
                                    set_errorMessage);
                if (result != SmartParametersV2016.operationFailure &&
                    data_stream.Length > 0)
                {
                    string[] sqlitetablerecords = data_stream.ToString().Split(SmartParametersV2016.unitSeparator);
                    foreach (string sqllitetablerecord in sqlitetablerecords)
                    {
                        string[] sqlitetables = sqllitetablerecord.Split(SmartParametersV2016.fieldSeparator);
                        if (sqlitetables.Length >= 5)
                        {
                            SmartData.SQLiteTables def = new()
                            {
                                SCHEMA_NAME = sqlitetables[0],
                                TABLE_NAME = sqlitetables[1],
                                ORDINAL = Convert.ToInt16(sqlitetables[2]),
                                PRODUCTION = sqlitetables[3],
                                DESCRIPTION = sqlitetables[4]
                            };
                            sqlitetables_list.Add(def);
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                OutputText(client, sqlex.Message, true);
            }
            return sqlitetables_list;
        }

        internal async Task<List<SmartData.SQLiteSchemas>> GetSQLiteSchemas(ServerModel servermodel,
                                                        string client,
                                                        int databaseIndex,
                                                        int schemaIndex,
                                                        string tableName,
                                                        Action<string> set_errorMessage)
        {
            char result = SmartParametersV2016.defaultChar;
            List<SmartData.SQLiteSchemas> sqliteschemas_list = new();
            string sql = "SELECT * FROM " + tableName + ";";
            try
            {
                StringBuilder data_stream = new();
                result = await BuildRecords(servermodel,
                                    sql,
                                    databaseIndex,
                                    schemaIndex,
                                    tableName,
                                    ds => data_stream = ds,
                                    set_errorMessage);
                if (result != SmartParametersV2016.operationFailure &&
                    data_stream.Length > 0)
                {
                    string[] sqliteschemarecords = data_stream.ToString().Split(SmartParametersV2016.unitSeparator);
                    foreach (string sqlliteschemarecord in sqliteschemarecords)
                    {
                        string[] sqliteschemas = sqlliteschemarecord.Split(SmartParametersV2016.fieldSeparator);
                        if (sqliteschemas.Length >= 2)
                        {
                            SmartData.SQLiteSchemas def = new()
                            {
                                SCHEMA_NAME = sqliteschemas[0],
                                DESCRIPTION = sqliteschemas[1]
                            };
                            sqliteschemas_list.Add(def);
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                OutputText(client, sqlex.Message, true);
            }
            return sqliteschemas_list;
        }
        internal async Task<List<SmartData.SQLiteFields>> GetSQLiteFields(ServerModel servermodel,
                                                        string client,
                                                        int databaseIndex,
                                                        int schemaIndex,
                                                        string tableName,
                                                        Action<string> set_errorMessage)
        {
            char result = SmartParametersV2016.defaultChar;
            List<SmartData.SQLiteFields> sqlitefields_list = new();
            string sql = "SELECT * FROM " + tableName + ";";
            try
            {
                StringBuilder data_stream = new();
                result = await BuildRecords(servermodel,
                                    sql,
                                    databaseIndex,
                                    schemaIndex,
                                    tableName,
                                    ds => data_stream = ds,
                                    set_errorMessage);
                if (result != SmartParametersV2016.operationFailure &&
                    data_stream.Length > 0)
                {
                    string[] sqlitefieldrecords = data_stream.ToString().Split(SmartParametersV2016.unitSeparator);
                    foreach (string sqllitefieldrecord in sqlitefieldrecords)
                    {
                        string[] sqlitefields = sqllitefieldrecord.Split(SmartParametersV2016.fieldSeparator);
                        if (sqlitefields.Length >= 7)
                        {
                            SmartData.SQLiteFields def = new()
                            {
                                SCHEMA_NAME = sqlitefields[0],
                                TABLE_NAME = sqlitefields[1],
                                ORDINAL = Convert.ToInt16(sqlitefields[2]),
                                KEY_FIELD = Convert.ToChar(sqlitefields[3]),
                                FIELD_NAME = sqlitefields[4],
                                FIELD_TYPE = sqlitefields[5],
                                FIELD_LENGTH = Convert.ToInt16(sqlitefields[6])
                            };
                            sqlitefields_list.Add(def);
                        }                       
                    }
                }
            }
            catch (SqlException sqlex)
            {
                OutputText(client, sqlex.Message, true);
            }
            return sqlitefields_list;
        }
        internal async Task<bool> GetLastExchangeRate(ServerModel servermodel,
                                                                            string client,
                                                                            int databaseIndex,
                                                                            int schemaIndex,
                                                                            Action<string> set_errorMessage)
        {
            servermodel.ERList = new();
            string sql = "SELECT TOP 1 * FROM EXCHANGE_RATES ORDER BY TRANSACTION_DATE DESC";
            //string sql = "SELECT * FROM EXCHANGE_RATES ORDER BY TRANSACTION_DATE DESC";
            try
            {
                StringBuilder data_stream = new();
                if (await BuildRecords(servermodel,
                                    sql,
                                    databaseIndex,
                                    schemaIndex,
                                    "EXCHANGE_RATES",
                                    ds => data_stream = ds,
                                    set_errorMessage) == SmartParametersV2016.operationSuccess)
                {
                    if (data_stream.Length > 0)
                    {
                        string[] er_records = data_stream.ToString().Split(SmartParametersV2016.unitSeparator);
                        foreach (string er_record in er_records)
                        {
                            string[] er_fields = er_record.Split(SmartParametersV2016.fieldSeparator);
                            if (er_fields.Length >= 0)
                            {
                                servermodel.lastGoodDate = Convert.ToDateTime(er_fields[0]);
                                servermodel.lastECB = Convert.ToInt16(er_fields[1]);
                                servermodel.lastGBP = Convert.ToDecimal(er_fields[2]);
                                servermodel.lastEUR = Convert.ToDecimal(er_fields[3]);
                                servermodel.lastUSD = Convert.ToDecimal(er_fields[4]);
                                servermodel.lastJPY = Convert.ToDecimal(er_fields[5]);
                                //break; // Only the first (there should be only 1)
                                SmartUsers.ExchangeRates rays = new SmartUsers.ExchangeRates()
                                {
                                    TRANSACTION_DATE = servermodel.lastGoodDate,
                                    ECB = servermodel.lastECB,
                                    CURRENCY_RATE_01 = servermodel.lastGBP,
                                    CURRENCY_RATE_02 = servermodel.lastEUR,
                                    CURRENCY_RATE_03 = servermodel.lastUSD,
                                    CURRENCY_RATE_04 = servermodel.lastJPY
                                };
                                servermodel.ERList.Add(rays);
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlex)
            {
                OutputText(client, sqlex.Message, true);
                return false;
            }
            OutputText(client,
                            "ER last : " + servermodel.lastGoodDate.ToString(SmartParametersV2016.sqldateFormat) +
                            " " + servermodel.lastECB.ToString() +
                            " GBP " + servermodel.lastGBP +
                            " EUR " + servermodel.lastEUR +
                            " USD " + servermodel.lastUSD +
                            //" CAD " + servermodel.lastCAN +
                            " JPY " + servermodel.lastJPY, true);            
            return true;
        }
        internal async Task<List<SmartUsers.ExchangeRates>> LoadECBExchangeRatesNew(string dbclient,
                                                                                            DateTime lastGoodDate,
                                                                                            short lastECB,
                                                                                            decimal lastGBP,
                                                                                            decimal lastEUR,
                                                                                            decimal lastUSD,
                                                                                            decimal lastJPY)
        {
            List<SmartUsers.ExchangeRates> exchange_rates_list = new();
            string url = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml";

            using (HttpClient client = new HttpClient())
            {
                string xmlContent = await client.GetStringAsync(url);
                XDocument doc = XDocument.Parse(xmlContent);
                XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";
                
                // ---------- STEP 1: LOAD ECB DATA AFTER lastGoodDate ----------
                var rawDays = doc.Descendants(ns + "Cube")
                    .Where(x => x.Attribute("time") != null)
                    .Select(day =>
                    {
                        var rates = day.Elements(ns + "Cube")
                            .ToDictionary(
                                x => x.Attribute("currency").Value,
                                x => decimal.Parse(x.Attribute("rate").Value)
                            );

                        decimal gbp = rates["GBP"];

                        return new
                        {
                            Date = DateTime.Parse(day.Attribute("time").Value),
                            EUR = 1.0m / gbp,
                            GBP = 1.0m,
                            USD = rates["USD"] / gbp,
                            JPY = rates["JPY"] / gbp,
                            IsReal = 1
                        };
                    })
                    .Where(d => d.Date > lastGoodDate)
                    .OrderBy(d => d.Date)
                    .ToList();

                List<dynamic> result = new List<dynamic>();

                // ---------- STEP 2: INITIAL GAP FILL ----------
                DateTime currentDate = lastGoodDate.AddDays(1);

                dynamic lastKnown = new
                {
                    Date = lastGoodDate,
                    EUR = lastEUR,
                    GBP = lastGBP,
                    USD = lastUSD,
                    JPY = lastJPY,
                    IsReal = 1
                };

                int index = 0;

                // Fill gap BEFORE first ECB date
                if (rawDays.Count > 0)
                {
                    while (currentDate < rawDays[0].Date)
                    {
                        result.Add(new
                        {
                            Date = currentDate,
                            EUR = lastKnown.EUR,
                            GBP = lastKnown.GBP,
                            USD = lastKnown.USD,
                            JPY = lastKnown.JPY,
                            IsReal = 0
                        });
                        currentDate = currentDate.AddDays(1);
                    }
                }

                // ---------- STEP 3: PROCESS ECB DATA + GAP FILL ----------
                while (index < rawDays.Count)
                {
                    var day = rawDays[index];

                    if (day.Date == currentDate)
                    {
                        result.Add(day);
                        lastKnown = day;
                        index++;
                        currentDate = currentDate.AddDays(1);
                    }
                    else if (day.Date > currentDate)
                    {
                        // Fill gap
                        result.Add(new
                        {
                            Date = currentDate,
                            EUR = lastKnown.EUR,
                            GBP = lastKnown.GBP,
                            USD = lastKnown.USD,
                            JPY = lastKnown.JPY,
                            IsReal = 0
                        });
                        currentDate = currentDate.AddDays(1);
                    }
                }

                // ---------- OUTPUT ----------
                if (result.Count > 0)
                {
                    OutputText(dbclient,
                            "Date       | R | GBP   | EUR     | USD     | JPY",
                            true);
                }
                // Leave them in ascending order so we can update the
                // LastGoodDate
                foreach (var d in result)//.OrderByDescending(x => x.Date))
                {
                    OutputText(dbclient,
                        $"{d.Date:yyyy-MM-dd} | " +
                        $"{d.IsReal} | " +
                        $"{d.GBP:F4} | " +
                        $"{d.EUR:F4} | " +
                        $"{d.USD:F4} | " +
                        $"{d.JPY:F2}", true);
                    
                    SmartUsers.ExchangeRates exchange_rate = new()
                    {
                        TRANSACTION_DATE = d.Date,
                        ECB = d.IsReal,
                        CURRENCY_RATE_01 = Convert.ToDecimal(string.Format(SmartParametersV2016.fourdecplaces, d.GBP)),
                        CURRENCY_RATE_02 = Convert.ToDecimal(string.Format(SmartParametersV2016.fourdecplaces, d.EUR)),
                        CURRENCY_RATE_03 = Convert.ToDecimal(string.Format(SmartParametersV2016.fourdecplaces, d.USD)),
                        //CURRENCY_RATE_04 = Convert.ToDecimal(string.Format(SmartParametersV2016.fourdecplaces, d.CAD)),
                        CURRENCY_RATE_04 = Convert.ToDecimal(string.Format(SmartParametersV2016.fourdecplaces, d.JPY))
                    };
                    exchange_rates_list.Add(exchange_rate);
                }
            }            
            return exchange_rates_list;
        }
    }
}