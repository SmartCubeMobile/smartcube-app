using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

//using System.Data.SqlClient;// For SQLServer2014 Connection (but the 'instance' is still SQLEXPRESS ... =:-O((
using System.Linq;
using System.Reflection;
using System.Text;

#if DBSERVER
using SmartDBServer;
using Windows.Media.Protection.PlayReady;
#endif
#if WINFORMS
using SmartDBServer;
using Microsoft.Data.SqlClient;
#endif

namespace SmartCubeMobile
{
    public class UnitRatesStruct
    {
        internal string Name = string.Empty,
                Supplier = string.Empty;
    }

    public class SqlTables
    {
        // See what tables we have in the database
        internal string TABLE_SCHEMA = string.Empty,
                        TABLE_NAME = string.Empty;
    }

    public class SqlKeys
    {
        internal string TABLE_SCHEMA = string.Empty,
                TABLE_NAME = string.Empty,
                KEY_NAME = string.Empty;
    }

    public class SqlFields
    {
        internal string
            TABLE_SCHEMA = string.Empty,
            TABLE_NAME = string.Empty,
            FIELD_NAME = string.Empty,
            DATA_TYPE = string.Empty,
            DATA_DELIMITER = string.Empty,
            CHARACTER_MAXIMUM = string.Empty;
    }

    public class Come
    {
        internal string column_name;
    }
    public class Drivel
    {
        internal string schema_name;
    }

    public class Uppance
    {
        internal string column_name = "";
        internal string data_type = "";
        internal string dataDelimiter = "";
        internal string character_maximum = "";
    }

    public class Cow
    {
        internal string schema_name = "";
        internal string tableName = "";
        internal List<Uppance> fields = new List<Uppance>();
        internal List<Come> keys = new List<Come>();
    }

    public class Tiresome
    {
        internal string procedure_name = "";
        internal int parameters = 0;
    }

    public class Petulant
    {
        internal short ordinal = -1;
        internal string databaseName = "";
        internal string connectionString = "";
        internal SqlConnection connection = new SqlConnection();
        internal StringBuilder table_data = new StringBuilder();
        internal int record_count = 0;
        internal List<Drivel> schemas = new List<Drivel>();
        internal List<Cow> tables = new List<Cow>();
        internal List<Tiresome> procedures = new List<Tiresome>();
    }

    public class SmartProgramV2016
    {
#if WINFORMS || DBSERVER
#if WINFORMS
        internal static List<Petulant> FindAllDatabases(ServerModel servermodel,
                                                                        SqlConnection database_connection,
                                                                        string client,
                                                                        string keyword)
#endif
#if DBSERVER
        internal static List<Petulant> FindAllDatabases(ServerModel servermodel,
                                                                        SqlConnection database_connection,
                                                                        string client,
                                                                        string keyword,
                                                                        Program program)

#endif
        {
        // Well I'm going to go with this 'cos it appears to work

        //string sql = "SELECT db.[name] FROM[master].[sys].[databases] db";
        //sql = sql + " LEFT OUTER JOIN[master].[sys].[sysusers] su on su.sid = db.owner_sid";
        //sql = sql + " WHERE su.sid is null and db.is_broker_enabled = 0 and db.[name] like " + SmartParametersV2016.dataDelimiter + keyword + SmartParametersV2016.dataDelimiter + " order by db.[name]";

            servermodel.outputResult = string.Empty;
            string db_databaseName;
            //
            //http://sqlrus.com/2011/10/add-a-procedure-to-master-database-yes-please/
            // These two lines just set up the keys table
            List<Petulant> databases = new List<Petulant>();

            bool usp_exists = false;
            string sql_query = "select * from master.sys.procedures where name LIKE " + SmartParametersV2016.dataDelimiter + "usp_%" + SmartParametersV2016.dataDelimiter;
            try
            {
                using (SqlCommand command = new SqlCommand(sql_query, database_connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usp_exists = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                servermodel.outputResult = e.Message;
#if DBSERVER
                program.OutputText(client, e.Message, true, ConsoleColor.Red);
#endif
            }
            if (!usp_exists)
            {
                servermodel.outputResult = "Cannot find usp_% proccedure in master database";
            }
            else
            {
                string parameters = keyword;
                try
                {
                    SqlDataReader sql_reader = Execute_Procedure("1",
                                                                database_connection,
                                                                parameters,
                                                                SmartParametersV2016.unitSeparator);
                    while (sql_reader.Read())
                    {
                        db_databaseName = sql_reader.GetValue(0).ToString();
                        if (db_databaseName == "SmartSwitch")
                        {
#if DBSERVER
                            program.OutputText(client, "Excluding SmartSwitch", true, ConsoleColor.Red);
#endif
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(db_databaseName))
                            {
                                Petulant item = new Petulant()
                                {
                                    databaseName = db_databaseName,
                                    connectionString = SmartParametersV2016.connectionString.Replace("Database=",
                                                                                    "Database=" + db_databaseName),
                                    connection = new SqlConnection()
                                };
                                databases.Add(item);
                            }
                        }
                    }
                    sql_reader.Close();
                }
                catch (SqlException exception)
                {
                    servermodel.outputResult = exception.Message;
                }
            }
            return databases;
        }

        internal static short FindAllProperties(ServerModel servermodel,
                                                SqlConnection connection,
                                                string keyword)
        {
            // select * from sys.schemas where name LIKE 'keyword'
            // select * from sys.extended_properties where name = 'ORDINAL'

            short ordinal = -1;
            //List<Drivel> schemas = new List<Drivel>();

            // Get the schema name
            servermodel.outputResult = string.Empty;
            // Get the procedure names
            string parameters = keyword;
            try
            {
                SqlDataReader sql_reader = Execute_Procedure("8",
                                                            connection,
                                                            parameters,
                                                            SmartParametersV2016.unitSeparator);
                while (sql_reader.Read())
                {
                    string s_n = sql_reader["VALUE"].ToString();
                    ordinal = Convert.ToInt16(s_n);

                    //Drivel schema_item = new Drivel()
                    //{
                    //    schema_name = s_n,
                    //    cubeface_code = c_code
                    //};
                    //schemas.Add(schema_item);
                }
                sql_reader.Close();
            }
            catch (SqlException exception)
            {
                servermodel.outputResult = exception.Message;
            }
            return ordinal;
        }

        //internal static List<Drivel> FindAllSchemas(ServerModel servermodel, SqlConnection connection, string keyword)
        //{
        //    // select * from sys.schemas where name LIKE 'keyword'

        //    List<Drivel> schemas = new List<Drivel>();

        //    // Get the schema name
        //    servermodel.errorMessage = string.Empty;
        //    // Get the procedure names
        //    string parameters = keyword;
        //    try
        //    {
        //        SqlDataReader sql_reader = Execute_Procedure("4",
        //                                                    connection,
        //                                                    parameters,
        //                                                    SmartParametersV2016.unitSeparator);
        //        while (sql_reader.Read())
        //        {
        //            string s_n = sql_reader["name"].ToString();
        //            //char c_code = Convert.ToChar(s_n.Replace("Smart", string.Empty).ToUpper());
        //            Drivel schema_item = new Drivel()
        //            {
        //                schema_name = s_n//,
        //                //cubeface_code = c_code
        //            };
        //            schemas.Add(schema_item);
        //        }
        //        sql_reader.Close();
        //    }
        //    catch (SqlException exception)
        //    {
        //        servermodel.errorMessage = exception.Message;
        //    }
        //    return schemas;
        //}
        //internal static async List<Drivel> FindAllSchemasSQLite(MainViewModel ourviewFormModel, SQLiteAsyncConnection abc, string keyword)
        //{
        //    List<Drivel> schemas = new List<Drivel>();

        //    // Get the schema name
        //    ourviewFormModel.errorMessage = string.Empty;
        //    // Get the procedure names
        //    string parameters = keyword;
        //    try
        //    {
        //        return await abc.QueryAsync<Drivel>("SELECT * FROM ");                
                
        //    }
        //    catch (SqlException exception)
        //    {
        //        ourviewFormModel.errorMessage = exception.Message;
        //    }
        //    return schemas;
        //}


        internal static List<Drivel> FindAllSchemas(ServerModel servermodel, SqlConnection connection, string keyword)
        {
            // select * from sys.schemas where name LIKE 'keyword'

            List<Drivel> schemas = new List<Drivel>();

            // Get the schema name
            servermodel.errorMessage = string.Empty;
            // Get the procedure names
            string parameters = keyword;
            try
            {
                SqlDataReader sql_reader = Execute_Procedure("4",
                                                            connection,
                                                            parameters,
                                                            SmartParametersV2016.unitSeparator);
                while (sql_reader.Read())
                {
                    string sqlReaderName = sql_reader["name"].ToString();
                    //char codeReaderName = Convert.ToChar(sqlReaderName.Replace("Smart", string.Empty).ToUpper());
                    //foreach (SmartData.Cubefaces cubeface_row in SmartMumList.cubefacesList)
                    //{

                    //}
                    Drivel schema_item = new Drivel()
                    {
                        schema_name = sqlReaderName//,
                        //cubeface_code = codeReaderName
                    };
                    schemas.Add(schema_item);
                }
                sql_reader.Close();
            }
            catch (SqlException exception)
            {
                servermodel.errorMessage = exception.Message;
            }
            return schemas;
        }

        internal static List<Cow> FindAllDBTables(ServerModel servermodel,
                                            SqlConnection database_connection,
                                             List<Drivel> database_schemas,
                                             bool must_be_uppercase)
        {
            // SELECT c.tableName, COLUMN_NAME AS primary_key" +
            // from information_schema.table_constraints pk" +
            // inner join information_schema.key_column_usage c" +
            // on c.tableName = pk.tableName" +
            // and c.constraint_name = pk.constraint_name" +
            // where pk.TABLE_SCHEMA = 'keyword'
            // and constraint_type = 'primary key'" +
            // order by tableName, ORDINAL_POSITION"; // <= Gets the keys in the real order

            List<Cow> cowList = new List<Cow>();

            // Get the schema name
            servermodel.errorMessage = string.Empty;
            // Get the procedure names
            foreach (Drivel database_schema in database_schemas)
            {
                string db_schema_name = database_schema.schema_name;
                string parameters = db_schema_name;
                // Find All Tables
                try
                {
                    SqlDataReader sql_table_reader = Execute_Procedure("5",
                                                                database_connection,
                                                                parameters,
                                                                SmartParametersV2016.unitSeparator);
                    // Store away any tables
                    Read_All_Tables(sql_table_reader,
                                            must_be_uppercase,
                                            db_schema_name,
                                            cowList);
                    sql_table_reader.Close();

                    // Find all the Primary Key columns
                    try
                    {
                        SqlDataReader sql_key_reader = Execute_Procedure("6",
                                                                    database_connection,
                                                                    parameters,
                                                                    SmartParametersV2016.unitSeparator);
                        // Store away any keys
                        // The schema is passed in to cater for the same table name in different schemas
                        Read_All_Keys(sql_key_reader, cowList, db_schema_name);
                        sql_key_reader.Close();

                        // Fields - find all Columns in all Tables in our schema
                        try
                        {
                            SqlDataReader sql_column_reader = Execute_Procedure("7",
                                                                        database_connection,
                                                                        parameters,
                                                                        SmartParametersV2016.unitSeparator);
                            // Store away the columns
                            // The schema is passed in to cater for the same table name in different schemas
                            Read_All_Columns(servermodel, sql_column_reader, cowList, db_schema_name);
                            sql_column_reader.Close();
                        }
                        catch (SqlException exception)
                        {
                            servermodel.errorMessage = exception.Message;
                        }
                    }
                    catch (SqlException exception)
                    {
                        servermodel.errorMessage = exception.Message;
                    }
                }
                catch (SqlException exception)
                {
                    servermodel.errorMessage = exception.Message;
                }
            }
            return cowList;
        }

        internal static void Determine_Data_Type(ServerModel servermodel,
                                                string data_type,
                                                string max_length)
        {
            // No more fucking NTEXTs ...
            if (data_type == "NVARCHAR" || data_type == "NCHAR" || data_type == "CHAR")
            {
                servermodel.field.data_type = data_type;
                servermodel.field.dataDelimiter = SmartParametersV2016.dataDelimiter;
                servermodel.field.character_maximum = max_length;
            }
            else
            {
                if (data_type == "DATE" || data_type == "DATETIME" || data_type == "BIT")
                {
                    servermodel.field.data_type = data_type;
                    servermodel.field.dataDelimiter = SmartParametersV2016.dataDelimiter;
                    servermodel.field.character_maximum = string.Empty; // I hate 'null's
                }
                else
                {
                    if (data_type == "INT" || data_type == "SMALLINT" || data_type == "TINYINT" || data_type == "NUMERIC" || data_type == "DECIMAL")
                    {
                        servermodel.field.data_type = data_type;
                        servermodel.field.dataDelimiter = string.Empty;
                        servermodel.field.character_maximum = string.Empty; // I hate 'null's
                    }
                    else
                    {
                        servermodel.field.dataDelimiter = string.Empty;
                        servermodel.field.character_maximum = string.Empty;
                    }
                }
            }
            return;
        }

        internal static void Read_All_Columns(ServerModel servermodel,
                                            SqlDataReader sql_column_reader,
                                            List<Cow> cowList,
                                            string db_schema_name)
        {
            // Store away the columns
            while (sql_column_reader.Read())
            {
                List<Cow> table_found = new List<Cow>(from Tables
                                               in cowList
                                                      where (Tables.schema_name == db_schema_name &&
                                                              Tables.tableName == sql_column_reader.GetString(0))
                                                      select Tables);
                if (table_found.Count > 0)
                {
                    servermodel.field = new Uppance()
                    {
                        column_name = sql_column_reader.GetValue(1).ToString()
                    };
                    if (string.IsNullOrEmpty(servermodel.field.data_type))
                    {
                        servermodel.field.data_type = "UNKNOWN";
                    }
                    // Add the bits and pieces depending on the Data Type
                    string data_type = sql_column_reader.GetValue(2).ToString().ToUpper();
                    Determine_Data_Type(servermodel, data_type, sql_column_reader["character_maximum_length"].ToString());

                    // Do the 'specials'
                    if ((data_type == "NUMERIC") ||
                        (data_type == "DECIMAL"))
                    {
                        // Decimal(6,3) type components
                        servermodel.field.character_maximum = sql_column_reader.GetValue(4).ToString() +
                                                            SmartParametersV2016.comma +
                                                            sql_column_reader.GetValue(5).ToString();
                    }
                    // Add it in ... at last ...!!
                    // All I've got to do now is figure out how to update (automatically) new tables
                    table_found[0].fields.Add(servermodel.field);
                }
            }
            return;
        }

        internal static List<Tiresome> FindAllProcedures(ServerModel servermodel, SqlConnection connection, string keyword, string databaseName)
        {
            // select * from " + databaseName + ".sys.procedures where name NOT LIKE " + SmartParametersV2016.dataDelimiter + keyword + SmartParametersV2016.dataDelimiter, connection);
            // then
            // select * from information_schema.parameters where specific_name = " + SmartParametersV2016.dataDelimiter + tiresome_item.procedure_name + SmartParametersV2016.dataDelimiter, connection);

            List<Tiresome> proceduresList = new List<Tiresome>();

            servermodel.errorMessage = string.Empty;
            // Get the procedure names
            string parameters = databaseName + SmartParametersV2016.unitSeparator + keyword;
            try
            {
                SqlDataReader sql_reader = Execute_Procedure("2",
                                                            connection,
                                                            parameters,
                                                            SmartParametersV2016.unitSeparator);
                while (sql_reader.Read())
                {
                    Tiresome tiresome_item = new Tiresome()
                    {
                        procedure_name = sql_reader["name"].ToString(),
                        parameters = 0
                    };
                    proceduresList.Add(tiresome_item);
                }
                sql_reader.Close();
            }
            catch (SqlException exception)
            {
                servermodel.errorMessage = exception.Message;
            }

            // Get the parameter count
            foreach (Tiresome tiresome_item in proceduresList)
            {
                int parameter_count = 0;
                parameters = tiresome_item.procedure_name;
                try
                {
                    SqlDataReader sql_reader = Execute_Procedure("3",
                                                                connection,
                                                                parameters,
                                                                SmartParametersV2016.unitSeparator);
                    while (sql_reader.Read())
                    {
                        parameter_count++;
                    }
                    sql_reader.Close();
                    tiresome_item.parameters = parameter_count;
                }
                catch (SqlException exception)
                {
                    servermodel.errorMessage = exception.Message;
                }
            }
            return proceduresList;
        }

        //Continual fucking running drivel commentary


#endif

        // use [SMARTMUM]
        // select*
        // from sys.extended_properties
        // where NAME = 'ORDINAL'

        internal static SqlDataReader Execute_Procedure(string procedure_code,
                                                        SqlConnection database_connection,
                                                        string parameters,
                                                        char parameter_split)
        {
            SqlCommand database_command = new SqlCommand();
            switch (procedure_code)
            {
                case "1":
                    database_command = new SqlCommand(SmartParametersV2016.uspFindAllDatabases, database_connection);
                    break;
                case "2":
                    database_command = new SqlCommand(SmartParametersV2016.findAllProcedures, database_connection);
                    break;
                case "3":
                    database_command = new SqlCommand(SmartParametersV2016.findAllParameters, database_connection);
                    break;
                case "4":
                    database_command = new SqlCommand(SmartParametersV2016.findAllSchemas, database_connection);
                    break;
                case "5":
                    database_command = new SqlCommand(SmartParametersV2016.findAllTables, database_connection);
                    break;
                case "6":
                    database_command = new SqlCommand(SmartParametersV2016.findAllPrimaryKeys, database_connection);
                    break;
                case "7":
                    database_command = new SqlCommand(SmartParametersV2016.findAllColumns, database_connection);
                    break;
                case "8":   // Extended Properties - lookup the ORDINAL
                    database_command = new SqlCommand(SmartParametersV2016.findAllProperties, database_connection);
                    break;
                default:
                    break;
            }
            database_command.CommandType = CommandType.StoredProcedure;

            string[] p1 = parameters.Split(parameter_split);

            SqlCommandBuilder.DeriveParameters(database_command);
            int p1_count = 0;
            bool enough_parameters = true;
            foreach (SqlParameter parameter in database_command.Parameters)
            {
                switch (parameter.ParameterName)
                {
                    case "@RETURN_VALUE":
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
            // If we had enough parameters then execute the procedure, otherwise return nothing
            if (enough_parameters)
            {
                return database_command.ExecuteReader();
            }
            return null;
        }



        internal static void Read_All_Keys(SqlDataReader sql_key_reader,
                                            List<Cow> cowList,
                                            string db_schema_name)
        {
            while (sql_key_reader.Read())
            {
                List<Cow> table_found = new List<Cow>(from Tables
                                            in cowList
                                                      where (Tables.schema_name == db_schema_name &&
                                                             Tables.tableName == sql_key_reader.GetString(0))
                                                      select Tables);
                if (table_found.Count > 0)
                {
                    Come key = new Come()
                    {
                        column_name = sql_key_reader.GetString(1)
                    };
                    table_found[0].keys.Add(key);
                }
            }
        }

        internal static void Read_All_Tables(SqlDataReader sql_table_reader,
                                            bool must_be_uppercase,
                                            string db_schema_name,
                                            List<Cow> cowList)
        {
            string db_tableName;
            while (sql_table_reader.Read())
            {
                db_tableName = sql_table_reader.GetValue(0).ToString();

                if (!string.IsNullOrEmpty(db_tableName))
                {
                    Cow tableItem = new Cow();
                    if (must_be_uppercase)
                    {
                        if (db_tableName == db_tableName.ToUpper())
                        {
                            tableItem.schema_name = db_schema_name;
                            tableItem.tableName = db_tableName;
                        }
                        else
                        {
                            // Don't add in null item
                            continue;
                        }
                    }
                    else
                    {
                        tableItem.schema_name = db_schema_name;
                        tableItem.tableName = db_tableName;
                    }
                    cowList.Add(tableItem);
                }
            }
        }

#if WINFORMS
        internal static bool InsertTable(ServerModel servermodel,
                                            string tableName,
                                            string schema_name,
                                            SqlConnection database_connection,
                                            List<SqlKeys> keysList,
                                            List<SqlFields> fieldsList)
        {
            //foreach (SqlFields temp_fields_table_row in temp_fieldsList)
            //{
            //    Uppance fields_table_row = new Uppance()
            //    {
            //        column_name = temp_fields_table_row.FIELD_NAME,
            //        data_type = temp_fields_table_row.DATA_TYPE,
            //        dataDelimiter = temp_fields_table_row.DATA_DELIMITER,
            //        character_maximum = temp_fields_table_row.CHARACTER_MAXIMUM
            //    };
            //    fields_table.Add(fields_table_row);
            //}
            //foreach (SqlKeys temp_keys_table_row in temp_keysList)
            //{
            //    Come keys_table_row = new Come()
            //    {
            //        column_name = temp_keys_table_row.KEY_NAME
            //    };
            //    keys_table.Add(keys_table_row);
            //}

            if (!Create_Table(servermodel,
                                database_connection,
                                keysList,
                                fieldsList,
                                schema_name,
                                tableName))
            {
                return false;
            }
            return true;
        }

        internal static bool IsTableValid(ServerModel servermodel,
                                            string tableName,
                                            string schema_name,
                                            List<SqlTables> sql_tablesList,
                                            List<SqlKeys> sql_keysList,
                                            List<SqlFields> sql_fieldsList,
                                            List<SqlKeys> temp_keysList,
                                            List<SqlFields> temp_fieldsList)
        {
            bool rebuild = false;   // Assume everything is good

            // See if the receiving table name is there ... if the schema isn't empty
            string receiving_tableName = tableName;

            string query = "tableName = '" + tableName + "'";
            List<SqlTables> sql_tables_found = new List<SqlTables>(from Sql_Table in sql_tablesList
                                                                                                   where (Sql_Table.TABLE_SCHEMA == schema_name &&
                                                                                                          Sql_Table.TABLE_NAME == tableName)
                                                                                                   select Sql_Table);
            if (sql_tables_found.Count == 0)
            {
                servermodel.itsThere = false;
                rebuild = true; // Tables not there - need to make it
            }
            else
            {
                servermodel.itsThere = true; // Its there - see if the keys match!!
                List<SqlKeys> sql_keys_found = new List<SqlKeys>(from Sql_Key in sql_keysList
                                                                                                 where (Sql_Key.TABLE_SCHEMA == schema_name &&
                                                                                                        Sql_Key.TABLE_NAME == tableName)
                                                                                                 select Sql_Key);
                if (sql_keys_found.Count != temp_keysList.Count)
                {
                    rebuild = true; // Need to drop and rebuild
                }
                else
                {
                    int key_index = 0;
                    foreach (SqlKeys sql_key_row in sql_keys_found)
                    {
                        if (sql_key_row.KEY_NAME != temp_keysList.ElementAt(key_index).KEY_NAME)
                        {
                            rebuild = true; // Key names don't match need to drop and rebuild
                            break;
                        }
                        key_index++;
                    }
                }
                // Check the fields if needs be
                if (!rebuild)
                {
                    List<SqlFields> sql_fields_found = new List<SqlFields>(from Sql_Field in sql_fieldsList
                                                                                                           where (Sql_Field.TABLE_SCHEMA == schema_name &&
                                                                                                                  Sql_Field.TABLE_NAME == tableName)
                                                                                                           select Sql_Field);
                    if (sql_fields_found.Count != temp_fieldsList.Count)
                    {
                        rebuild = true; // Field count doesn't match - need to drop and rebuild
                    }
                    else
                    {
                        int field_index = 0;
                        foreach (SqlFields sql_field_row in sql_fields_found)
                        {
                            string data_type = sql_field_row.DATA_TYPE;
                            string temp_data_type = temp_fieldsList.ElementAt(field_index).DATA_TYPE;
                            if (!string.IsNullOrEmpty(sql_field_row.CHARACTER_MAXIMUM))
                            {
                                data_type = data_type + "(" + sql_field_row.CHARACTER_MAXIMUM + ")";
                                temp_data_type = temp_data_type + "(" +
                                                temp_fieldsList.ElementAt(field_index).CHARACTER_MAXIMUM +
                                                ")";
                            }
                            if ((sql_field_row.FIELD_NAME != temp_fieldsList.ElementAt(field_index).FIELD_NAME) ||
                                (data_type != temp_data_type))
                            {
                                rebuild = true;  // Field names don't match - need to drop and rebuild
                                break;
                            }
                            field_index++;
                        }
                    }
                }
            }
            return rebuild;
        }

        //internal static void Create_Outline(ServerModel servermodel,
        //                                    List<Cow> tablesList)
        //                                    //rf List<SqlTables> sql_tablesList,
        //                                    //rf List<SqlKeys> sql_keysList,
        //                                    //rf List<SqlFields> sql_fieldsList)
        //{
        //    // See what tables we have in the database
        //    servermodel.sql_tablesList.Clear();

        //    servermodel.sql_keysList.Clear();

        //    servermodel.sql_fieldsList.Clear();

        //    foreach (Cow tableItem in tablesList)
        //    {
        //        SqlTables sql_tables_row = new SqlTables()
        //        {
        //            TABLE_SCHEMA = tableItem.schema_name,
        //            TABLE_NAME = tableItem.tableName
        //        };

        //        servermodel.sql_tablesList.Add(sql_tables_row);

        //        foreach (Come key_item in tableItem.keys)
        //        {
        //            SqlKeys sql_keys_row = new SqlKeys()
        //            {
        //                TABLE_SCHEMA = tableItem.schema_name,
        //                TABLE_NAME = tableItem.tableName,
        //                KEY_NAME = key_item.column_name
        //            };
        //            servermodel.sql_keysList.Add(sql_keys_row);
        //        }
        //        foreach (Uppance field_item in tableItem.fields)
        //        {
        //            SqlFields sql_fields_row = new SqlFields()
        //            {
        //                TABLE_SCHEMA = tableItem.schema_name,
        //                TABLE_NAME = tableItem.tableName,
        //                FIELD_NAME = field_item.column_name,
        //                DATA_TYPE = field_item.data_type,
        //                DATA_DELIMITER = field_item.dataDelimiter,
        //                CHARACTER_MAXIMUM = field_item.character_maximum
        //            };
        //            servermodel.sql_fieldsList.Add(sql_fields_row);
        //        }
        //    }
        //    return;
        //}

        internal static bool DeleteDBRows(ServerModel servermodel,
                                            SqlConnection database_connection,
                                            string schema_name,
                                            string tableName)
        {
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();
            string sql;
            if (!string.IsNullOrEmpty(schema_name))
            {
                sql = "DELETE FROM " + schema_name + "." + tableName;
            }
            else
            {
                sql = "DELETE FROM " + tableName;
            }
            SqlCommand sqlcommand = new SqlCommand(sql, database_connection);
            try
            {
                sqlcommand.ExecuteNonQuery();
                servermodel.errorMessage = routine + "Deleted all rows from: " + schema_name + "." + tableName;
                // No need to tidy up sql_table, sql_keys or sql_fields these will all disappear
                return true;
            }
            catch (SqlException sqlexception)
            {
                servermodel.errorMessage = sqlexception.Message;
            }
            catch (Exception exception)
            {
                servermodel.errorMessage = exception.Message;
            }
            return false;
        }

        internal static bool Drop_DBTable(ServerModel servermodel,
                                            SqlConnection database_connection,
                                            string schema_name,
                                            string tableName)
        {
            string routine = MethodBase.GetCurrentMethod().Name.ToUpper();
            string sql;
            if (!string.IsNullOrEmpty(schema_name))
            {
                sql = "DROP TABLE " + schema_name + "." + tableName;
            }
            else
            {
                sql = "DROP TABLE " + tableName;
            }
            SqlCommand sqlcommand = new SqlCommand(sql, database_connection);
            try
            {
                sqlcommand.ExecuteNonQuery();
                servermodel.errorMessage = routine + "Dropped table: " + schema_name + "." + tableName;
                // No need to tidy up sql_table, sql_keys or sql_fields these will all disappear
                return true;
            }
            catch (SqlException sqlexception)
            {
                servermodel.errorMessage = sqlexception.Message;
            }
            catch (Exception exception)
            {
                servermodel.errorMessage = exception.Message;
            }
            return false;
        }

        internal static bool Create_Table(ServerModel servermodel,
                                        SqlConnection database_connection,
                                        List<SqlKeys> keys_table,
                                        List<SqlFields> fields_table,
                                        string schema_name,
                                        string tableName)
        {
            servermodel.errorMessage = string.Empty;
            string sql = Build_Sql(fields_table, keys_table, schema_name, tableName);
            SqlCommand sqlcommand = new SqlCommand(sql, database_connection);
            try
            {
                sqlcommand.ExecuteNonQuery();
                string info = string.Empty;
                int come_count = 0;
                while (come_count < keys_table.Count)
                {
                    SqlKeys temp_key_row = keys_table[come_count];
                    if (string.IsNullOrEmpty(info))
                    {
                        info = "Table " +
                                schema_name + "." +
                                tableName +
                                " created with" +
                                " KEY " +
                                temp_key_row.KEY_NAME +
                                info;
                    }
                    else
                    {
                        info = info + " KEY " +
                                temp_key_row.KEY_NAME;
                    }

                    come_count++;
                }
                //foreach (DataRow temp_key_row in keys_table.Rows)
                //{
                //    if (string.IsNullOrEmpty(info))
                //    {
                //        info = "Table " +
                //                schema_name + "." +
                //                tableName +
                //                " created with" +
                //                " KEY " +
                //                temp_key_row["KEY_NAME"] +
                //                info;
                //    }
                //    else
                //    {
                //        info = info + " KEY " +
                //                temp_key_row["KEY_NAME"];
                //    }
                //}
                if (string.IsNullOrEmpty(info))
                {
                    info = "Table " + schema_name + "." + tableName + " created";
                }
            }
            catch (SqlException sqlexception)
            {
                StringBuilder errorMessages = new StringBuilder();
                for (int i = 0; i < sqlexception.Errors.Count; i++)
                {
                    errorMessages.Append("Index #" + i + "\n" +
                                    "Message   : " + sqlexception.Errors[i].Message + "\n" +
                                    "ourviewmodel    : " + sqlexception.Errors[i].Source + "\n");
                }
                // Put this out on the Console board
                servermodel.errorMessage = errorMessages.ToString();
                return false;
            }
            catch (Exception exception)
            {
                // Put this out on the Console board
                servermodel.errorMessage = exception.Message;
                return false;
            }
            return true;
        }

        internal static string Build_Sql(List<SqlFields> fields_table,
                                List<SqlKeys> keys_table,
                                string schema_name,
                                string tableName)
        {
            String[] data;

            string sql,
                    dname,
                    dtype = string.Empty,
                    dnull;
            if (!string.IsNullOrEmpty(schema_name))
            {
                sql = "CREATE TABLE " + schema_name + "." + tableName + " (";
            }
            else
            {
                sql = "CREATE TABLE " + tableName + " (";
            }

            int data_index;

            bool done_any_key;

            int uppance_count = 0;
            while (uppance_count < fields_table.Count)
            {
                SqlFields field_row = fields_table[uppance_count];
                if (uppance_count > 0)
                {
                    sql += ", ";
                }
                if (string.IsNullOrEmpty(field_row.DATA_TYPE))
                {
                    dname = field_row.FIELD_NAME;
                    dtype = "NCHAR(1)";
                    dnull = "NOT NULL";
                }
                else
                {
                    dname = field_row.FIELD_NAME;
                    data = field_row.DATA_TYPE.ToString().Split(' ');
                    // Default this
                    dnull = "NOT NULL";
                    data_index = 0;
                    while (data_index < data.Length)
                    {
                        switch (data_index)
                        {
                            case 0:
                                dtype = data[data_index];
                                if (field_row.CHARACTER_MAXIMUM != string.Empty)
                                {
                                    dtype = dtype + "(" + field_row.CHARACTER_MAXIMUM + ")";
                                }
                                break;
                            case 1:
                                dnull = data[data_index];
                                break;
                            default:
                                break;
                        }
                        data_index++;
                    }
                }
                sql = sql + dname + " " + dtype + " " + dnull;

                uppance_count++;
            }

            //foreach (DataRow field_row in fields_table.Rows)
            //{
            //    if (string.IsNullOrEmpty(comma))
            //    {
            //        comma = ", ";
            //    }
            //    else
            //    {
            //        sql = sql + comma;
            //    }
            //    if (string.IsNullOrEmpty(field_row["DATA_TYPE"].ToString()))
            //    {
            //        dname = field_row["FIELD_NAME"].ToString();
            //        dtype = "NCHAR(1)";
            //        dnull = "NOT NULL";
            //    }
            //    else
            //    {
            //        dname = field_row["FIELD_NAME"].ToString();
            //        data = field_row["DATA_TYPE"].ToString().Split(' ');
            //        // Default this
            //        dnull = "NOT NULL";
            //        data_index = 0;
            //        while (data_index < data.Count())
            //        {
            //            switch (data_index)
            //            {
            //                case 0:
            //                    dtype = data[data_index];
            //                    break;
            //                case 1:
            //                    dnull = data[data_index];
            //                    break;
            //                default:
            //                    break;
            //            }
            //            data_index = data_index + 1;
            //        }
            //    }
            //    sql = sql + dname + " " + dtype + " " + dnull;
            //}

            done_any_key = false;

            int come_count = 0;
            while (come_count < keys_table.Count)
            {
                SqlKeys key_row = keys_table[come_count];

                if (!done_any_key)
                {
                    sql = sql + ", CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED (" + key_row.KEY_NAME;
                    done_any_key = true;
                }
                else
                {
                    sql = sql + "," + key_row.KEY_NAME;
                }
                come_count++;
            }

            //foreach (DataRow key_row in keys_table.Rows)
            //{
            //    if (!done_any_key)
            //    {
            //        sql = sql + ", CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED (" + key_row["KEY_NAME"].ToString();
            //        done_any_key = true;
            //    }
            //    else
            //    {
            //        sql = sql + "," + key_row["KEY_NAME"].ToString();
            //    }
            //}
            if (done_any_key)
            {
                sql += ")";
            }
            return sql + ")";
        }

        internal static bool Insert_Rows_DB(ServerModel servermodel,
                                string routine,
                                string yymmdd_format,
                                SqlConnection sql_connection,
                                List<SqlFields> fields_table,
                                List<string> records,
                                string schema_name,
                                string tableName,
                                string defaultDate_string,
                                string today_date,
                                string now_date)
        {
            string sql = string.Empty,
                    dname;

            int space,
                    row_count,
                    column_count,
                    insert_limit;

            if (records.Count > 0)
            {
                foreach (string record in records)
                {
                    sql = "INSERT INTO " + tableName + " (";
                    int loop = 0;
                    foreach (SqlFields column in fields_table)
                    {
                        if (loop > 0)
                        {
                            sql += ", ";
                        }
                        space = column.FIELD_NAME.IndexOf(' ');
                        if (space == -1)
                        {
                            dname = column.FIELD_NAME;
                        }
                        else
                        {
                            dname = column.FIELD_NAME.Substring(0, space);
                        }
                        sql += dname;
                        loop++;
                    }
                    sql += ")";
                    sql += " VALUES (";

                    column_count = 0;
                    loop = 0;
                    foreach (SqlFields column in fields_table)
                    {
                        if (loop > 0)
                        {
                            sql += ", ";
                        }
                        column_count++;
                        dname = "@VALUE" + column_count.ToString();  // All rows are fixed length
                        sql += dname;
                        loop++;
                    }
                    sql += ")";
                }

                row_count = 0;
                insert_limit = 0;   // Me, me, me ... all the time

                SqlTransaction sqlTran = null;
                SqlCommand insert_cmd = null;

                foreach (string record in records)
                {
                    if (row_count == 0)
                    {
                        sqlTran = sql_connection.BeginTransaction();
                        insert_cmd = new SqlCommand(sql, sql_connection)
                        {
                            Transaction = sqlTran
                        };
                    }

                    // Now go round the columns
                    string[] fields = record.Split(SmartParametersV2016.unitSeparator);

                    column_count = 0;
                    string value = string.Empty;
                    foreach (SqlFields column in fields_table)
                    {
                        string datatype = column.DATA_TYPE;
                        if (datatype == "System.DateTime")
                        {
                            switch (fields[column_count].ToString())
                            {
                                case "<today>":
                                    value = SmartTimeV2016.ConvertDateTime(today_date).ToString(yymmdd_format);
                                    break;
                                case "<null>":
                                    value = SmartTimeV2016.ConvertDateTime(defaultDate_string).ToString(yymmdd_format);
                                    break;
                                case "<now>":
                                    value = SmartTimeV2016.ConvertDateTime(now_date).ToString(yymmdd_format);
                                    break;
                                case "":
                                    value = SmartTimeV2016.ConvertDateTime(defaultDate_string).ToString(yymmdd_format);
                                    break;
                                default:
                                    value = SmartTimeV2016.ConvertDateTime(fields[column_count]).ToString(yymmdd_format);
                                    break;
                            }
                        }
                        else
                        {
                            value = fields[column_count].ToString();
                        }
                        column_count++;

                        insert_cmd.Parameters.AddWithValue("@VALUE" + column_count.ToString(), value);
                    }
                    try
                    {
                        insert_cmd.ExecuteNonQuery();
                    }
                    catch (SqlException sqlexception)
                    {
                        servermodel.errorMessage = routine + " " + sqlexception.Message;
                        return false;
                    }
                    catch (Exception exception)
                    {
                        servermodel.errorMessage = routine + " " + exception.Message;
                        return false;
                    }
                    insert_cmd.Parameters.Clear();
                    insert_limit++;
                    if (insert_limit == 1000)
                    {
                        try
                        {
                            sqlTran.Commit();
                            //insert_cmd.Dispose();
                            //sqlTran.Dispose();
                            // Restart a new transaction
                            row_count = -1;
                            // AND RESET THE FUCKING LIMIT COUNT YOU DICKHEAD!
                            // **NO WONDER** IT WAS ONLY INSERTING JUST 1000 LINES!!!
                            insert_limit = 0;
                        }
                        catch (SqlException sqlexception)
                        {
                            servermodel.errorMessage = routine + " " + sqlexception.Message;
                            return false;
                        }
                        catch (Exception exception)
                        {
                            servermodel.errorMessage = routine + " " + exception.Message;
                            return false;
                        }
                    }
                    row_count++;
                }
                if (row_count > 0)
                {
                    try
                    {
                        // Commit the remainder
                        sqlTran.Commit();
                        //insert_cmd.Dispose();
                        //sqlTran.Dispose();
                    }
                    catch (SqlException sqlexception)
                    {
                        servermodel.errorMessage = routine + " " + sqlexception.Message;
                        return false;
                    }
                    catch (Exception exception)
                    {
                        servermodel.errorMessage = routine + " " + exception.Message;
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool TwatAbout(ServerModel servermodel,
                                        string tableName,
                                        string schema_name,
                                        List<SqlTables> sql_tablesList,
                                        List<SqlKeys> sql_keysList,
                                        List<SqlFields> sql_fieldsList,
                                        List<SqlKeys> mooList,
                                        List<SqlFields> wookyList,
                                        SqlConnection database_connection)
        {
            bool rebuild = false;
            servermodel.itsThere = false;
            if (IsTableValid(servermodel,
                            tableName,
                            schema_name,
                            sql_tablesList,
                            sql_keysList,
                            sql_fieldsList,
                            mooList,
                            wookyList))
            {
                // Yes? no?
                if (servermodel.itsThere)
                {
                    if (!Drop_DBTable(servermodel, database_connection, schema_name, tableName))
                    {
                        // Put this out on the Scraper board
                        //Form1.Output_Message(ourviewmodel, problem, SmartParametersV2016.trace);
                        return false;
                    }
                }
                // Its been dropped - we need to re-insert it
                rebuild = true;
            }

            // If its not there OR the fields don't match then rebuild it
            if (rebuild)
            {
                if (!InsertTable(servermodel,
                            tableName,
                            schema_name,
                            database_connection,
                            mooList,
                            wookyList))
                {
                    //Form1.Output_Message(ourviewmodel, problem, SmartParametersV2016.trace);
                    return false;
                }
            }
            else
            {
                // Delete all records ready for reload
                if (!DeleteDBRows(servermodel, database_connection, schema_name, tableName))
                {
                    // Put this out on the Scraper board
                    //Form1.Output_Message(ourviewmodel, problem, SmartParametersV2016.trace);
                    return false;
                }
            }
            return true;
        }

        internal static void AddField(string schema_name,
                                   string tableName,
                                   string column_name,
                                   string data_type,
                                   string data_delimiter,
                                   string char_max,
                                   List<SqlFields> temp_fieldsList)
        {
            SqlFields temp_fields_row = new SqlFields()
            {
                TABLE_SCHEMA = schema_name,
                TABLE_NAME = tableName,
                FIELD_NAME = column_name,
                DATA_TYPE = data_type.ToUpper(),
                DATA_DELIMITER = data_delimiter,
                CHARACTER_MAXIMUM = char_max
            };
            temp_fieldsList.Add(temp_fields_row);
            return;
        }

        internal static void AddKey(string schema_name,
                                    string tableName,
                                    string column_name,
                                    List<SqlKeys> temp_keysList)
        {
            SqlKeys temp_keys_row = new SqlKeys()
            {
                TABLE_SCHEMA = schema_name,
                TABLE_NAME = tableName,
                KEY_NAME = column_name
            };
            temp_keysList.Add(temp_keys_row);
            return;
        }

        public static bool Compare_Schema_Tables(Cow local_table, Cow remote_table)
        {
            if ((local_table.schema_name != remote_table.schema_name) ||
                (local_table.tableName != remote_table.tableName))
            {
                return false;
            }
            else
            {
                List<Uppance> local_fields = local_table.fields;
                List<Uppance> remote_fields = remote_table.fields;
                if (local_fields.Count != remote_fields.Count)
                {
                    return false;
                }
                else
                {
                    int field_count = 0;
                    while (field_count < local_fields.Count)
                    {
                        Uppance local_uppance = local_fields[field_count];
                        Uppance remote_uppance = remote_fields[field_count];
                        if ((local_uppance.column_name != remote_uppance.column_name) ||
                            (local_uppance.data_type != remote_uppance.data_type) ||
                            (local_uppance.dataDelimiter != remote_uppance.dataDelimiter) ||
                            (local_uppance.character_maximum != remote_uppance.character_maximum))
                        {
                            return false;
                        }
                        field_count = field_count + 1;
                    }

                    List<Come> local_keys = local_table.keys;
                    List<Come> remote_keys = remote_table.keys;
                    if (local_keys.Count != remote_keys.Count)
                    {
                        return false;
                    }
                    else
                    {
                        int keys_count = 0;
                        while (keys_count < local_keys.Count)
                        {
                            Come local_come = local_keys[keys_count];
                            Come remote_come = remote_keys[keys_count];
                            if (local_come.column_name != remote_come.column_name)
                            {
                                return false;
                            }
                            keys_count = keys_count + 1;
                        }
                    }
                }
            }
            return true;
        }
#endif
    }
}