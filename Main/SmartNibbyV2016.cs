//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is
//using System.Net.Http;        // No more of this absolute bollocks shit

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCubeMobile
{
    internal class SmartNibbyV2016
    {
        internal static bool NetworkAvailability()
        {
            // For some peculiar reason (probably just because its a PILE OF SHIT)
            // when running this under the Android emulator (!!) it wants to 
            // generate an exception and tell you "Success" if it works (!!!)
            // rather than just returning 'true' or 'false' (You couldn't make
            // this shit up - you really couldn't ....)
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Success")
                {
                    return true;
                }
                else
                {
                    // Usually 'No such file or directory' (i.e. a REALLY
                    // unhelpful fucking message)
                    // Console.Write(ex.Message);
                }
            }
            return false;
        }

        internal static bool Find_A_Tag_Simple(UtilityViewModel utilityviewmodel,
                                                HtmlAgilityPack.HtmlDocument htmlDocument,
                                                string document_tag,
                                                bool only_consider_null_elements,
                                                string remove,
                                                string inner_text_comparison)
        //string next_routine)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;

            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(htmlDocument.DocumentNode, document_tag);
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1) // Checked
            {
                // YOU NEED TO REDUCE THIS Ray, IT CAN BE MADE A GREAT DEAL MORE CONCISE
                bool carry_on = false;
                if (only_consider_null_elements)
                {
                    if (string.IsNullOrEmpty(element1.Id))
                    {
                        carry_on = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(element1.Id))
                    {
                        carry_on = true;
                    }
                }
                if (carry_on)
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        string inner_text = element1.InnerText.Replace(Environment.NewLine, "").Trim();
                        // Anything to remove?
                        if (!string.IsNullOrEmpty(remove))
                        {
                            inner_text = inner_text.Replace(remove, "");
                        }

                        if (inner_text == inner_text_comparison)
                        {
                            // Get pathname
                            utilityviewmodel.href = element1.GetAttributeValue(SmartParametersV2016.href, "");
                            //                            if (!string.IsNullOrEmpty(href))
                            //                            {
                            //                                if (!string.IsNullOrEmpty(next_routine))
                            //                                {
                            //                                    utilityviewmodel.target_pathname = href;
                            //                                    // Hook up the next routine - for the next Object Moved 'Document Completed' delivery
                            //                                    utilityviewmodel.next_routine = next_routine;
                            //#if WINFORMS
                            //                                    if (utilityviewmodel.console)
                            //                                    {
                            //                                        await SmartRoutines_V2018.TextBlockUpdate(ourviewmodel, "Next routine ..." + utilityviewmodel.next_routine + Environment.NewLine.ToString());
                            //                                    }
                            //#endif
                            //                                }
                            //                                else
                            //                                {
                            //                                    utilityviewmodel.logout_pathname = href;
                            //                                    utilityviewmodel.logout_pathname = utilityviewmodel.logout_pathname.Replace(utilityviewmodel.prfix_xxx, "");
                            //#if WINFORMS
                            //                                    if (utilityviewmodel.console)
                            //                                    {
                            //                                        await SmartRoutines_V2018.TextBlockUpdate(ourviewmodel,"Logout path ..." + utilityviewmodel.logout_pathname + Environment.NewLine.ToString());
                            //                                    }
                            //#endif
                            //                                }
                            //                                clicked = true;
                            //                                break;
                            //                            }
                        }
                    }
                }
            }
            return clicked;
        }

#if WINFORMS
        internal static async Task<bool> Find_A_Tag(MainViewModel ourviewmodel,

#endif
#if WPF  || WINUI || SMARTMAUI
        internal static bool Find_A_Tag(MainViewModel ourviewmodel,

#endif
#if ANDROIDX
        internal static bool Find_A_Tag(MainViewModel ourviewmodel,
        
#endif
                                            HtmlAgilityPack.HtmlDocument htmlDocument,
                                            UtilityViewModel utilityviewmodel,
                                            string document_tag,
                                            bool only_consider_null_elements,
                                            string remove,
                                            string inner_text_comparison,
                                            string next_routine)
        {
            IList<HtmlAgilityPack.HtmlNode>
                        HtmlCol1;

            bool clicked = false;

            HtmlCol1 = YetAnotherFuckingHoop.SelectNodesAsList(htmlDocument.DocumentNode, document_tag);
            foreach (HtmlAgilityPack.HtmlNode element1 in HtmlCol1) // Checked
            {
                // YOU NEED TO REDUCE THIS Ray, IT CAN BE MADE A GREAT DEAL MORE CONCISE
                bool carry_on = false;
                if (only_consider_null_elements)
                {
                    if (string.IsNullOrEmpty(element1.Id))
                    {
                        carry_on = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(element1.Id))
                    {
                        carry_on = true;
                    }
                }
                if (carry_on)
                {
                    if (!string.IsNullOrEmpty(element1.InnerText))
                    {
                        string inner_text = element1.InnerText.Replace(Environment.NewLine, "").Trim();
                        // Anything to remove?
                        if (!string.IsNullOrEmpty(remove))
                        {
                            inner_text = inner_text.Replace(remove, "");
                        }

                        if (inner_text == inner_text_comparison)
                        {
                            // Get pathname
                            string href = element1.GetAttributeValue(SmartParametersV2016.href, "");
                            if (!string.IsNullOrEmpty(href))
                            {
                                if (!string.IsNullOrEmpty(next_routine))
                                {
                                    utilityviewmodel.target_pathname = href;
                                    // Hook up the next routine - for the next Object Moved 'Document Completed' delivery
                                    utilityviewmodel.next_routine = next_routine;
#if WINFORMS
                                    if (utilityviewmodel.console)
                                    {
                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Next routine ..." + utilityviewmodel.next_routine + Environment.NewLine.ToString());
                                    }
#endif
                                }
                                else
                                {
                                    utilityviewmodel.logout_pathname = href;
                                    utilityviewmodel.logout_pathname = utilityviewmodel.logout_pathname.Replace(utilityviewmodel.prfix_xxx, "");
#if WINFORMS
                                    if (utilityviewmodel.console)
                                    {
                                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, "Logout path ..." + utilityviewmodel.logout_pathname + Environment.NewLine.ToString());
                                    }
#endif
                                }
                                clicked = true;
                                break;
                            }
                        }
                    }
                }
            }
            return clicked;
        }

        internal static bool Create_Uri(MainViewModel ourviewmodel, string base_string, string relative_string)
        {
            // DON'T FUCK WITH THIS ROUTINE <=== THIS MEANS ***YOU***
            bool status = true;
            ourviewmodel.errorMessage = "";
            try
            {
                if (!string.IsNullOrEmpty(base_string))
                {
                    ourviewmodel.TargetUrl = new Uri(base_string);

                    if (!string.IsNullOrEmpty(relative_string))
                    {
                        if (relative_string.IndexOf(base_string) == -1)
                        {
                            Uri relative_uri = new Uri(relative_string, UriKind.Relative);
                            ourviewmodel.TargetUrl = new Uri(ourviewmodel.TargetUrl, relative_uri);
                        }
                        else
                        {
                            ourviewmodel.TargetUrl = new Uri(relative_string);
                        }
                    }
                }
                else
                {
                    ourviewmodel.errorMessage = "Uri base string is empty";
                    status = false;
                }
            }
            catch (ArgumentNullException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
                status = false;
            }
            catch (UriFormatException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
                status = false;
            }
            if (!status)
            {
                ourviewmodel.errorMessage = "Cannot create Uri from: " + base_string + relative_string + SmartParametersV2016.space + ourviewmodel.errorMessage;

            }
            return status;
        }

        internal static Uri Return_Uri(MainViewModel ourviewmodel, string base_string, string relative_string)
        {
            // DON'T FUCK WITH THIS ROUTINE <=== THIS MEANS ***YOU***
            bool status = true;
            ourviewmodel.errorMessage = "";
            try
            {
                if (!string.IsNullOrEmpty(base_string))
                {
                    Uri base_uri = new Uri(base_string);

                    if (!string.IsNullOrEmpty(relative_string))
                    {
                        if (relative_string.IndexOf(base_string) == -1)
                        {
                            Uri relative_uri = new Uri(relative_string, UriKind.Relative);
                            base_uri = new Uri(base_uri, relative_uri);
                        }
                        else
                        {
                            base_uri = new Uri(relative_string);
                        }
                        return base_uri;
                    }
                }
                else
                {
                    ourviewmodel.errorMessage = "Uri base string is empty";
                    status = false;
                }
            }
            catch (ArgumentNullException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
                status = false;
            }
            catch (UriFormatException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
                status = false;
            }
            if (!status)
            {
                ourviewmodel.errorMessage = "Cannot create Uri from: " + base_string + relative_string + SmartParametersV2016.space + ourviewmodel.errorMessage;
            }
            return new Uri(base_string);
        }

        internal static string Check_Timeout(MainViewModel ourviewmodel)
        {
            if ((ourviewmodel.timespanTimeout.TotalSeconds <= 0) ||       // Should be > 0
                 (ourviewmodel.timespanTimeout.TotalSeconds > 120)) // and <= 120 seconds
            {
                return " Timeout value invalid " + (ourviewmodel.timespanTimeout.TotalSeconds).ToString();
            }
            return "";
        }

        //private static void Check_Field_VALUE(MainViewModel ourviewmodel,
        //                                        rf string field_value)
        //{
        //    if (!string.IsNullOrEmpty(field_value))
        //    {
        //        string[] parts = field_value.Split(SmartParametersV2016.encryptSeparator);
        //        field_value = "";   // In case it all goes tits-up in the next section ...
        //        if (parts.Length == 2)
        //        {
        //            if (!string.IsNullOrEmpty(parts[0]) &&
        //                !string.IsNullOrEmpty(parts[1]))
        //            {
        //                string time_now = SmartEncryptionV2016.DoTheBiz("", SmartParametersV2016.private_key, parts[0], false);

        //                field_value = SmartEncryptionV2016.DoTheBiz("", time_now, parts[1], false);
        //            }
        //        }
        //    }
        //    return;
        //}


#if WINFORMS
        internal static string Check_Date(string item, string datenull_string)
        {
            // This is fine - we have to have something if no date is sent
            // But don't twat about changing 19-Sep-2015 00:00:00 into 19-Sep-2015 12:00:00
            // life is too difficult as it is ...
            if (string.IsNullOrEmpty(item))
            {
                return datenull_string;
            }
            return item;
        }
#endif
        //internal static bool Generic_Parse_Datetime_Culture(MainViewModel ourviewmodel,
        //                                                    string date_value, 
        //                                                    rf DateTime target_date)
        //{
        //    //if (date_value.Length > 10)
        //    //{
        //    //    if (date_value.IndexOf("00:00:00") == -1)
        //    //    {

        //    //    }
        //    //}
        //    if (DateTime.TryParse(date_value,
        //                            SmartParametersV2016.defaultCulture,
        //                            DateTimeStyles.None, 
        //                            out target_date))
        //    {
        //        return true;
        //    }
        //    ourviewmodel.errorMessage = "Cannot convert date: " + date_value;
        //    return false;
        //}

        // The problem is, is that once an Account record has been inserted, it never gets updated!!
        // Dog with a fucking bone ... saw, faux cousin, her life, where I should fucking work, television on as fucking usual
        internal static DateTime ConvertDate(string date_string,
                                            DateTime defaultDate,
                                            MainViewModel ourviewmodel)
        {
            DateTime date = defaultDate;
            // Clear this down
            //ourviewmodel.errorMessage = "";
            // Is there a time part?
            try
            {
                // Yes/No time part or not - use this
                date = SmartRoutinesV2018.DateTimeParseCulture(date_string, SmartParametersV2016.defaultCulture);
            }
            catch (ArgumentNullException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
            }
            catch (FormatException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
            }
            catch (ArgumentException exception)
            {
                ourviewmodel.errorMessage = exception.Message;
            }
            return date;
        }

        internal class Split
        {
            internal string
                OUTWARD,
                INWARD;
        }

        internal static List<SmartUsers.Working_Postcodes> Build_Postcodes(List<SmartUsers.InternalPostcodes> postcodesList)
        {
            List<SmartUsers.Working_Postcodes> workingPostcodesList = new List<SmartUsers.Working_Postcodes>();
            List<Split> splitList = new List<Split>();
            foreach (SmartUsers.InternalPostcodes postcodes_row in postcodesList) // Checked
            {
                Expand_Postcodes(postcodes_row.POSTCODE, splitList);
                if (splitList.Count > 0)
                {
                    // Now ... before we add it do we need to change it and/or add others?
                    foreach (Split split_row in splitList) // Checked
                    {
                        string postcode;
                        if (!string.IsNullOrEmpty(split_row.INWARD))
                        {
                            postcode = split_row.OUTWARD + SmartParametersV2016.space + split_row.INWARD;
                        }
                        else
                        {
                            postcode = split_row.OUTWARD;
                        }
                        //postcodes_row.LOCATION; // Out temporarily for memory reasons
                        SmartUsers.Working_Postcodes working_postcodes_row = new SmartUsers.Working_Postcodes(postcode, postcodes_row.AREA_CODE);
                        workingPostcodesList.Add(working_postcodes_row);
                    }
                    splitList.Clear();
                }
            }
            return workingPostcodesList;
        }

        internal static void Expand_Postcodes(string postal_string,
                                            List<Split> expandList)
        {
            string outward,
                    inward,
                    limit;
            int dash,
                    space,
                    lower,
                    upper,
                    pos,
                    len,
                    first;
            bool found;
            char chr;

            dash = postal_string.IndexOf('-');
            if (dash == -1)
            {
                Split expand_row = new Split();
                space = postal_string.IndexOf(' ');
                if (space == -1)
                {
                    expand_row.OUTWARD = postal_string;
                    expand_row.INWARD = ""; // Different from ""
                }
                else
                {
                    expand_row.OUTWARD = postal_string.Substring(0, space);
                    expand_row.INWARD = postal_string.Substring(space + 1, postal_string.Length - space - 1);
                }
                expandList.Add(expand_row);
                return;
            }
            else
            {
                outward = postal_string.Substring(0, dash);
                inward = "";
                limit = postal_string.Substring(dash + 1, postal_string.Length - dash - 1);
                len = outward.Length;
                found = false;
                lower = upper = pos = first = 0;
                while (pos < len)
                {
                    chr = Convert.ToChar(outward.Substring(pos, 1));// SmartParametersV2016.defaultCulture);
                    if (Char.IsDigit(chr))
                    {
                        lower *= 10;
                        lower += (chr - '0');
                        if (found == false)
                        {
                            first = pos;
                            found = true;
                        }
                    }
                    pos++;
                }
                if (found == true)
                {
                    outward = outward.Substring(0, first);
                    //first = 0;
                    pos = 0;
                    len = limit.Length;
                    found = false;
                    while (pos < len)
                    {
                        chr = Convert.ToChar(limit.Substring(pos, 1));// SmartParametersV2016.defaultCulture);
                        if (Char.IsDigit(chr))
                        {
                            upper *= 10;
                            upper += (chr - '0');
                            if (found == false)
                            {
                                //first = pos;
                                found = true;
                            }
                        }
                        pos++;
                    }
                }

                if ((found == true) &&
                        (upper >= lower)) // &&
                //(outward == limit.Substring(0, first)))
                {
                    for (int loop = lower; loop <= upper; loop++)
                    {
                        Split expand_row = new Split()
                        {
                            OUTWARD = outward + loop.ToString(),
                            INWARD = inward
                        };
                        expandList.Add(expand_row);
                    }
                }
            }
        }

        internal static bool Derive_Postcode_New(string account_postcode,
                                            List<SmartUsers.Working_Postcodes> postcodesList,
                                            UtilityViewModel utilityviewmodel)
        {
            // Only if we haven't found it before
            if (!string.IsNullOrEmpty(utilityviewmodel.postcode) &&
                utilityviewmodel.area_code > 0)
            {
                return true;
            }
            else
            {
                string[] words;
                int word_count;

                words = account_postcode.Split(' ');
                word_count = 0;
                while (word_count < words.Length)
                {
                    // Look for the PostCode first
                    List<SmartUsers.Working_Postcodes> filteredRows = SmartSpikeV2017.Find_Working_Postcodes(postcodesList,
                                                                                                                            words[word_count]);

                    // Did we find it?
                    if (filteredRows.Count == 1)
                    {
                        // We found it? Assume so - store away the part we found
                        foreach (SmartUsers.Working_Postcodes data_row in filteredRows) // Checked
                        {
                            utilityviewmodel.postcode = words[word_count];
                            if ((word_count + 1) < words.Length)
                            {
                                utilityviewmodel.postcode = utilityviewmodel.postcode + SmartParametersV2016.space + words[word_count + 1];
                            }
                            utilityviewmodel.area_code = data_row.AREA_CODE;
                            // Gets us out of the loop
                            word_count = words.Length;
                            // Only the first postcode 'pair' is ever considered
                            return true; // break;
                        }
                    }
                    word_count++;
                }
            }
            return false;
        }


        internal static string Build_StringNew<T>(FieldInfo[] myFields, T sqlite_row, string date_format,
                                                    bool obscure = false)
        {
            string row_as_string = "";
            // Filter out the Username (its 'set' on the other side in DbServer
            for (int index = 1; index < myFields.Length; index++)   // Miss out Username
            {
                // filter out "UPDATED" fields
                if (myFields[index].Name != "Updated" &&
                    myFields[index].Name != "Delete")
                {
                    if (!string.IsNullOrEmpty(row_as_string))
                    {
                        row_as_string += SmartParametersV2016.fieldSeparator;
                    }
                    if (myFields[index].FieldType.FullName == "System.DateTime")
                    {
                        // This SHOULD be in the right format ..
                        string icow = Convert.ToDateTime(myFields[index].GetValue(sqlite_row)).ToString(SmartParametersV2016.defaultCulture);
                        row_as_string += SmartTimeV2016.ConvertDateTime(icow).ToString(date_format);


                        //DateTime ray_date = SmartTimeV2016.ConvertDateTime(myFields[index].GetValue(sqlite_row).ToString());
                        //row_as_string += ray_date.ToString(date_format);
                    }
                    else
                    {
                        string raysname = myFields[index].Name;
                        row_as_string += myFields[index].GetValue(sqlite_row);
                    }
                }
            }
            return row_as_string;
        }


        internal static bool Check_Classname(string classname, string target, bool equals)
        {
            if (!string.IsNullOrEmpty(classname))
            {
                if (equals)
                {
                    if (classname == target)
                    {
                        return true;
                    }
                }
                else
                {
                    if (classname.IndexOf(target) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool Make_Out_Date(UtilityViewModel utilityviewmodel)
        {
            foreach (string date_row in SmartParametersV2016.dates) // Checked
            {
                utilityviewmodel.last_date_index = utilityviewmodel.token.IndexOf(date_row);
                if (utilityviewmodel.last_date_index >= 0)
                {
                    string replacement = date_row.Replace(SmartParametersV2016.space, "-");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(date_row, replacement);
                    return true;
                }
            }
            foreach (string date_row in SmartParametersV2016.months) // Checked
            {
                string month_row = SmartParametersV2016.space + date_row + SmartParametersV2016.space;
                utilityviewmodel.last_date_index = utilityviewmodel.token.IndexOf(month_row);
                if (utilityviewmodel.last_date_index >= 0)
                {
                    string replacement = month_row.Replace(SmartParametersV2016.space, "-");
                    utilityviewmodel.token = utilityviewmodel.token.Replace(month_row, replacement);
                    return true;
                }
            }
            return false;
        }

        internal static int Area_Matrix_Index(short area_code,
                                              FieldInfo[] myFields)
        {
            string target_name = "AREA_" + area_code.ToString("00");
            int field_index = 0;
            foreach (FieldInfo info in myFields) // Checked
            {
                if (info.Name == target_name)
                {
                    return field_index;
                }
                field_index++;
            }
            return -1;
        }

        internal static async Task<bool> MiserableBitch(SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        string UserName,        // Requesting Client
                                                        string GroupName,       // Target Client
                                                        string schemasList)     // List of schemas
        {
            if (string.IsNullOrEmpty(UserName))
            {
                return false;
            }
            // Talking herself up as usual ... she is SO full of shit
            // Look at me!  Look at ME!
            // The 'S' means we want all the data in all the tables but only for a particular username
            // But now we qualify the '*' with a list of all the tables we need to update
            string table_name;
            if (string.IsNullOrEmpty(ourviewmodel.utcList))
            {
                table_name = SmartParametersV2016.wildcard;
            }
            else
            {
                table_name = ourviewmodel.utcList;
            }

            object smartswitchList = await SmartBobV2017.LoadMultipleListAsyncX(ourviewmodel,
                                                                                        DateTime.Now + ourviewmodel.utcOffset,  // Local time
                                                                                        ourviewmodel.quitCts.Token,
                                                                                        signinviewmodel.loadtableUrl,
                                                                                        ourviewmodel.antiTokenString,
                                                                                        "S",
                                                                                        "SmartSwitch",
                                                                                        table_name,
                                                                                        GroupName,      // Which is the sqlclient
                                                                                        schemasList);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }
            if (smartswitchList != null)
            {
                // Add it to our list but how do we know
                // what index the GroupName is? Answer .. we dont
                // We will have to find that out!
                SmartSwitchItem abc = new SmartSwitchItem()
                {
                    User = UserName,
                    Group = GroupName,
                    SmartSwitchList = smartswitchList
                };
                ourviewmodel.myList.Add(abc);                
            }
            else
            {
                return false;
            }
            return true;
        }

        internal static async Task<bool> MiserableJokeSister(DateTime time_now,
                                                                SignInViewModel signinviewmodel,
                                                                CancellationToken quit_token,
                                                                string username,
                                                                string anti_token_string,
                                                                char database_code,
                                                                string databaseName,
                                                                string operation,
                                                                string parameter = "")
        {
            bool status = false;

            string database_name = "";
            switch (database_code)
            {
                case 'D':
                    database_name = databaseName;// "SMARTMUM"; // Load tables essential for the PROGRAM
                    break;
                default:
                    break;
            }

            string sql;
            if (string.IsNullOrEmpty(parameter))
            {
                sql = username;
            }
            else
            {
                sql = parameter;
            }

            // The 'T' means we want all the data in all the tables irrespective
            List<object> returnedList = await SmartBobV2017.LoadMultipleListAsyncSignIn(signinviewmodel,
                                                                                        time_now,
                                                                                        quit_token,
                                                                                        signinviewmodel.loadtableUrl,
                                                                                        anti_token_string,
                                                                                        operation,
                                                                                        database_name,
                                                                                        SmartParametersV2016.wildcard,
                                                                                        sql);            
            if (!string.IsNullOrEmpty(signinviewmodel.errorMessage))
            {
                return false;
            }
            else
            {
                if (returnedList.Count > 0)
                {
                    switch (operation)
                    {
                        case SmartParametersV2016.TotalTables:
                            switch (database_code)
                            {
                                // Load tables essential for the PROGRAM
                                case 'D':
                                    signinviewmodel.Fatah.buttonsList = SmartPhyllV2020.FindListTable<SmartData.Buttons>(returnedList);
                                    signinviewmodel.Fatah.cubefacesList = SmartPhyllV2020.FindListTable<SmartData.Cubefaces>(returnedList);
                                    signinviewmodel.Fatah.culturesList = SmartPhyllV2020.FindListTable<SmartData.Cultures>(returnedList);
                                    signinviewmodel.Fatah.currenciesList = SmartPhyllV2020.FindListTable<SmartData.Currencies>(returnedList);
                                    signinviewmodel.Fatah.handlersList = SmartPhyllV2020.FindListTable<SmartData.Handlers>(returnedList);
                                    signinviewmodel.Fatah.sqliteschemasList = SmartPhyllV2020.FindListTable<SmartData.SQLiteSchemas>(returnedList);
                                    signinviewmodel.Fatah.sqlitetablesList = SmartPhyllV2020.FindListTable<SmartData.SQLiteTables>(returnedList);
                                    signinviewmodel.Fatah.sqlitefieldsList = SmartPhyllV2020.FindListTable<SmartData.SQLiteFields>(returnedList);
                                    signinviewmodel.Fatah.tooltipsList = SmartPhyllV2020.FindListTable<SmartData.Tooltips>(returnedList);
                                    status = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return status;
        }

        internal static async Task<bool> MiserableJokeWife(DateTime time_now,
                                                                MainViewModel ourviewmodel,
                                                                CancellationToken quit_token,
                                                                //CookieContainer cookies,
                                                                //TimeSpan timespanTimeout,
                                                                string username,
                                                                string anti_token_string,
                                                                char database_code,
                                                                string operation,
                                                                string databaseName,
                                                                string parameter = "")
        {
            bool status = false;

            string database_name = "";
            switch (database_code)
            {
                case 'D':
                    database_name = databaseName;// Load tables essential for the USER
                    break;
                default:
                    break;
            }

            string sql;
            if (string.IsNullOrEmpty(parameter))
            {
                sql = username;
            }
            else
            {
                sql = parameter;
            }

            // The 'T' means we want all the data in all the tables irrespective
            List<object> returnedList = await SmartBobV2017.LoadMultipleListAsyncX(ourviewmodel,
                                                                                        time_now,
                                                                                        quit_token,
                                                                                        //cookies,
                                                                                        //timespanTimeout,
                                                                                        ourviewmodel.loadtableUrl,
                                                                                        anti_token_string,
                                                                                        operation,
                                                                                        database_name,
                                                                                        SmartParametersV2016.wildcard,
                                                                                        sql);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }
            else
            {
                if (returnedList.Count > 0)
                {
                    switch (operation)
                    {
                        case SmartParametersV2016.TotalTables:
                            switch (database_code)
                            {
                                // Load tables essential for the USER
                                case 'D':
                                    ourviewmodel.Blanche.exchangeRatesList = SmartPhyllV2020.FindListTable<SmartUsers.ExchangeRates>(returnedList);
                                    ourviewmodel.Blanche.externalResourcesList = SmartPhyllV2020.FindListTable<SmartUsers.ExternalResources>(returnedList);
                                    ourviewmodel.Blanche.postcodesList = SmartPhyllV2020.FindListTable<SmartUsers.InternalPostcodes>(returnedList);
                                    ourviewmodel.Blanche.vatRatesList = SmartPhyllV2020.FindListTable<SmartUsers.VatRates>(returnedList);
                                    ourviewmodel.Blanche.workingPostcodesList = Build_Postcodes(ourviewmodel.Blanche.postcodesList);
                                    status = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return status;
        }

        internal static async Task<bool> MiserableFuckingCow(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                char cubeface_code,
                                                                string operation,
                                                                string database_name,
                                                                string table_name, // Procedure name or wildcard
                                                                string parameter = "")
        {
            bool status = false;
            string sql;
            if (string.IsNullOrEmpty(parameter))
            {
                sql = ourviewmodel.UserName;
            }
            else
            {
                sql = parameter;
            }

            // The 'T' means we want all the data in all the tables irrespective
            List<object> returnedList = await SmartBobV2017.LoadMultipleListAsyncX(ourviewmodel,
                                                                                        DateTime.Now + ourviewmodel.utcOffset,   // Local time
                                                                                        ourviewmodel.quitCts.Token,
                                                                                        ourviewmodel.loadtableUrl,
                                                                                        ourviewmodel.antiTokenString,
                                                                                        operation,
                                                                                        database_name,
                                                                                        table_name,
                                                                                        sql);       // <= SQL
                                                                                                    //false,
                                                                                                    //em => ourviewmodel.errorMessage = em,
                                                                                                    //rto => ourviewmodel.request_timed_out = rto);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }
            else
            {
                if (returnedList.Count > 0)
                {
                    switch (operation)
                    {
                        case "T":
                            switch (cubeface_code)
                            {
                                case SmartParametersV2016.Profiles:
                                    break;
                                case SmartParametersV2016.Finance:
                                    financeviewmodel.PLO.category_codesList = SmartPhyllV2020.FindListTable<SmartFinance.CategoryCodes>(returnedList);
                                    financeviewmodel.PLO.brand_accountsList = SmartPhyllV2020.FindListTable<SmartFinance.BrandAccounts>(returnedList);
                                    financeviewmodel.PLO.brand_areasList = SmartPhyllV2020.FindListTable<SmartFinance.BrandAreas>(returnedList);
                                    financeviewmodel.PLO.brand_connectionList = SmartPhyllV2020.FindListTable<SmartFinance.BrandConnection>(returnedList);
                                    financeviewmodel.PLO.brand_matrixList = SmartPhyllV2020.FindListTable<SmartFinance.BrandMatrix>(returnedList);
                                    //financeviewmodel.PLO.brand_ordinalsList = SmartPhyllV2020.FindListTable<SmartFinance.BrandOrdinals>(returnedList);
                                    financeviewmodel.PLO.brandsList = SmartPhyllV2020.FindListTable<SmartFinance.Brands>(returnedList);
                                    financeviewmodel.PLO.institution_infoList = SmartPhyllV2020.FindListTable<SmartFinance.InstitutionInfo>(returnedList);
                                    financeviewmodel.PLO.institutionsList = SmartPhyllV2020.FindListTable<SmartFinance.Institutions>(returnedList);
                                    financeviewmodel.PLO.templatesList = SmartPhyllV2020.FindListTable<SmartFinance.Templates>(returnedList);
                                    //financeviewmodel.PLO.transaction_flowsList = SmartPhyllV2020.FindListTable<SmartFinance.Transaction_Flows>(returnedList);
                                    financeviewmodel.PLO.transaction_groupsList = SmartPhyllV2020.FindListTable<SmartFinance.Transaction_Groups>(returnedList);
                                    financeviewmodel.PLO.transaction_typesList = SmartPhyllV2020.FindListTable<SmartFinance.Transaction_Types>(returnedList);
                                    status = true;
                                    ourviewmodel.SMARTFINANCE = status;
                                    break;
                                case SmartParametersV2016.Utility:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Z":
                            switch (cubeface_code)
                            {
                                case SmartParametersV2016.Profiles:
                                    break;
                                case SmartParametersV2016.Finance:
                                    break;
                                case SmartParametersV2016.Utility:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return status;
        }

        internal static async Task<bool> MiserableFuckingCow(MainViewModel ourviewmodel,
                                                                UtilityViewModel utilityviewmodel,
                                                                char cubeface_code,
                                                                string operation,
                                                                string database_name,
                                                                string table_name, // Procedure name
                                                                string parameter = "")
        {
            bool status = false;
            string sql;
            if (string.IsNullOrEmpty(parameter))
            {
                sql = ourviewmodel.UserName;
            }
            else
            {
                sql = parameter;
            }

            // The 'T' means we want all the data in all the tables irrespective
            List<object> returnedList = await SmartBobV2017.LoadMultipleListAsyncX(ourviewmodel,
                                                                                        DateTime.Now + ourviewmodel.utcOffset,   // Local time
                                                                                        ourviewmodel.quitCts.Token,
                                                                                        ourviewmodel.loadtableUrl,
                                                                                        ourviewmodel.antiTokenString,
                                                                                        operation,
                                                                                        database_name,
                                                                                        table_name,  // Procedure name or wildcard
                                                                                        sql);       // <= SQL
                                                                                                    //false,
                                                                                                    //em => ourviewmodel.errorMessage = em,
                                                                                                    //rto => ourviewmodel.request_timed_out = rto);
            if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
            {
                return false;
            }
            else
            {
                if (returnedList.Count > 0)
                {
                    switch (operation)
                    {
                        case "T":
                            switch (cubeface_code)
                            {
                                case SmartParametersV2016.Profiles:
                                    break;
                                case SmartParametersV2016.Finance:
                                    break;
                                case SmartParametersV2016.Utility:
                                    // Keep it like ? - don't test for Count of 0
                                    utilityviewmodel.Hezbollah.payment_methodsList = SmartPhyllV2020.FindListTable<SmartUtility.PaymentMethods>(returnedList);
                                    utilityviewmodel.Hezbollah.supply_areasList = SmartPhyllV2020.FindListTable<SmartUtility.SupplyAreas>(returnedList);
                                    utilityviewmodel.Hezbollah.suppliersList = SmartPhyllV2020.FindListTable<SmartUtility.Suppliers>(returnedList);
                                    utilityviewmodel.Hezbollah.brandsList = SmartPhyllV2020.FindListTable<SmartUtility.Brands>(returnedList);
                                    utilityviewmodel.Hezbollah.distributor_infoList = SmartPhyllV2020.FindListTable<SmartUtility.DistributorInfo>(returnedList);
                                    utilityviewmodel.Hezbollah.supplier_typesList = SmartPhyllV2020.FindListTable<SmartUtility.SupplierTypes>(returnedList);
                                    utilityviewmodel.Hezbollah.supplier_infoList = SmartPhyllV2020.FindListTable<SmartUtility.SupplierInfo>(returnedList);
                                    utilityviewmodel.Hezbollah.brand_connectionList = SmartPhyllV2020.FindListTable<SmartUtility.BrandConnection>(returnedList);
                                    utilityviewmodel.Hezbollah.brand_matrixList = SmartPhyllV2020.FindListTable<SmartUtility.BrandMatrix>(returnedList);
                                    if (utilityviewmodel.Hezbollah.brand_matrixList.Count > 0)
                                    {
                                        if (!Check_Matrix(utilityviewmodel.Hezbollah.brand_matrixList[0]))
                                        {
                                            status = false;   // Should NEVER happen
                                            return status;
                                        }
                                    }

                                    utilityviewmodel.Hezbollah.tariff_matrixList = SmartPhyllV2020.FindListTable<SmartUtility.TariffMatrix>(returnedList);
                                    utilityviewmodel.Hezbollah.tariff_historyList = SmartPhyllV2020.FindListTable<SmartUtility.TariffHistory>(returnedList);
                                    utilityviewmodel.Hezbollah.tariffsList = SmartPhyllV2020.FindListTable<SmartUtility.Tariffs>(returnedList);


                                    //List<SmartUtility.Tariffs> tariffs_found = (from Tariff
                                    //        in utilityviewmodel.Hezbollah.tariffsList
                                    //                                            where ((Tariff.CUBEFACE_CODE == 'U') &&
                                    //                                                   (Tariff.SUPPLIER_CODE == 82) &&
                                    //                                                   (Tariff.RESOURCE_CODE == 'E') &&
                                    //                                                   (Tariff.RESOURCE_TYPE == "SR") &&
                                    //                                                   (Tariff.TARIFF_CODE == 19))
                                    //                                            select Tariff);

                                    utilityviewmodel.Hezbollah.payment_plansList = SmartPhyllV2020.FindListTable<SmartUtility.PaymentPlans>(returnedList);
                                    utilityviewmodel.Hezbollah.tariff_plansList = SmartPhyllV2020.FindListTable<SmartUtility.TariffPlans>(returnedList);
                                    utilityviewmodel.Hezbollah.post_codesList = SmartPhyllV2020.FindListTable<SmartUtility.Post_Codes>(returnedList);
                                    utilityviewmodel.Hezbollah.post_groupsList = SmartPhyllV2020.FindListTable<SmartUtility.Post_Groups>(returnedList);
                                    utilityviewmodel.Hezbollah.post_groupingsList = SmartPhyllV2020.FindListTable<SmartUtility.Post_Groupings>(returnedList);
                                    utilityviewmodel.Hezbollah.post_conditionsList = SmartPhyllV2020.FindListTable<SmartUtility.Post_Conditions>(returnedList);
                                    utilityviewmodel.Hezbollah.post_limitsList = SmartPhyllV2020.FindListTable<SmartUtility.Post_Limits>(returnedList);
                                    utilityviewmodel.Hezbollah.post_selectList = SmartPhyllV2020.FindListTable<SmartUtility.Post_Select>(returnedList);
                                    utilityviewmodel.Hezbollah.pre_conditionsList = SmartPhyllV2020.FindListTable<SmartUtility.PreConditions>(returnedList);
                                    utilityviewmodel.Hezbollah.conditions_areasList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsAreas>(returnedList);
                                    utilityviewmodel.Hezbollah.conditions_limitsList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsLimits>(returnedList);
                                    utilityviewmodel.Hezbollah.conditions_groupsList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsGroups>(returnedList);
                                    utilityviewmodel.Hezbollah.conditions_datesList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsDates>(returnedList);
                                    utilityviewmodel.Hezbollah.conditions_plansList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsPlans>(returnedList);
                                    utilityviewmodel.Hezbollah.resource_codesList = SmartPhyllV2020.FindListTable<SmartUtility.ResourceCodes>(returnedList);
                                    utilityviewmodel.Hezbollah.resource_typesList = SmartPhyllV2020.FindListTable<SmartUtility.ResourceTypes>(returnedList);
                                    utilityviewmodel.Hezbollah.gas_conversionList = SmartPhyllV2020.FindListTable<SmartUtility.GasConversion>(returnedList);
                                    utilityviewmodel.Hezbollah.templateList = SmartPhyllV2020.FindListTable<SmartUtility.Templates>(returnedList);

                                    // Fix these two numpties
                                    List<SmartUtility.GasConversion> gas_conversion_found = SmartSpikeUtilityV2017.Utility_Find_GasConversion(ourviewmodel, utilityviewmodel.Hezbollah.gas_conversionList);
                                    if (gas_conversion_found.Count > 0)
                                    {
                                        // These two sometimes aren't required
                                        if (utilityviewmodel.volumecorrection == 0.0M)
                                        {
                                            utilityviewmodel.volumecorrection = gas_conversion_found[0].CORRECTION_FACTOR;
                                        }
                                        if (utilityviewmodel.kwhconversion == 0.0M)
                                        {
                                            utilityviewmodel.kwhconversion = gas_conversion_found[0].KWH_CONVERSION;
                                        }
                                    }
                                    status = true;
                                    ourviewmodel.SMARTUTILITY = status;


                                    // Create the Hezbollah TCN
                                    utilityviewmodel.Hezbollah.tariff_codes_namesList = new List<SmartUtility.TariffCodesNames>();
                                    utilityviewmodel.Hezbollah.tariff_codes_namesList = SmartSpikeUtilityV2017.Utility_TariffCodesNames(ourviewmodel,
                                                                                        utilityviewmodel);  // <=UtilityViewModel not set yet?


                                    utilityviewmodel.Hezbollah.withdrawnDateList = SmartPhyllV2020.FindListTable<SmartUtility.WithdrawnDate>(returnedList);
                                    if (utilityviewmodel.Hezbollah.withdrawnDateList.Count > 0)
                                    {
                                        utilityviewmodel.withdrawn_date = utilityviewmodel.Hezbollah.withdrawnDateList.First().WITHDRAWN_DATE;
                                    }
                                    utilityviewmodel.Hezbollah.e_unit_ratesList = new List<SmartUtility.UnitRates>();
                                    utilityviewmodel.Hezbollah.g_unit_ratesList = new List<SmartUtility.UnitRates>();

                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Z":
                            switch (cubeface_code)
                            {
                                case SmartParametersV2016.Profiles:
                                    break;
                                case SmartParametersV2016.Finance:
                                    break;
                                case SmartParametersV2016.Utility:
                                    List<SmartUtility.TariffMatrix> tariff_matrixList = SmartPhyllV2020.FindListTable<SmartUtility.TariffMatrix>(returnedList);
                                    foreach (SmartUtility.TariffMatrix tariff_matrix_row in tariff_matrixList) // Checked
                                    {
                                        utilityviewmodel.Hezbollah.tariff_matrixList.Add(tariff_matrix_row);
                                    }
                                    List<SmartUtility.TariffHistory> tariff_historyList = SmartPhyllV2020.FindListTable<SmartUtility.TariffHistory>(returnedList);
                                    foreach (SmartUtility.TariffHistory tariff_history_row in tariff_historyList) // Checked
                                    {
                                        utilityviewmodel.Hezbollah.tariff_historyList.Add(tariff_history_row);
                                    }
                                    List<SmartUtility.Tariffs> tariffsList = SmartPhyllV2020.FindListTable<SmartUtility.Tariffs>(returnedList);
                                    foreach (SmartUtility.Tariffs tariffs_row in tariffsList) // Checked
                                    {
                                        utilityviewmodel.Hezbollah.tariffsList.Add(tariffs_row);
                                    }
                                    List<SmartUtility.ConditionsPlans> conditions_plansList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsPlans>(returnedList);
                                    foreach (SmartUtility.ConditionsPlans conditions_plans_row in conditions_plansList) // Checked
                                    {
                                        utilityviewmodel.Hezbollah.conditions_plansList.Add(conditions_plans_row);
                                    }
                                    List<SmartUtility.ConditionsDates> conditions_datesList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsDates>(returnedList);
                                    foreach (SmartUtility.ConditionsDates conditions_dates_row in conditions_datesList) // Checked
                                    {
                                        utilityviewmodel.Hezbollah.conditions_datesList.Add(conditions_dates_row);
                                    }
                                    List<SmartUtility.ConditionsGroups> conditions_groupsList = SmartPhyllV2020.FindListTable<SmartUtility.ConditionsGroups>(returnedList);
                                    foreach (SmartUtility.ConditionsGroups conditions_groups_row in conditions_groupsList) // Checked
                                    {
                                        utilityviewmodel.Hezbollah.conditions_groupsList.Add(conditions_groups_row);

                                    }
                                    List<SmartUtility.UnitRates> unit_ratesList = SmartPhyllV2020.FindListTable<SmartUtility.UnitRates>(returnedList);
                                    foreach (SmartUtility.UnitRates unit_rates_row in unit_ratesList) // Checked
                                    {
                                        switch (unit_rates_row.RESOURCE_CODE)
                                        {
                                            case SmartParametersV2016.Electricity:
                                                utilityviewmodel.Hezbollah.e_unit_ratesList.Add(unit_rates_row);
                                                break;
                                            case SmartParametersV2016.Gas:
                                                utilityviewmodel.Hezbollah.g_unit_ratesList.Add(unit_rates_row);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    status = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return status;
        }

        internal static bool Check_Matrix(SmartUtility.BrandMatrix brand_matrix_row)
        {
            if (brand_matrix_row.AREA_10 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_11 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_12 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_13 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_14 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_15 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_16 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_17 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_18 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_19 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_20 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_21 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_22 == SmartParametersV2016.defaultChar ||
                        brand_matrix_row.AREA_23 == SmartParametersV2016.defaultChar)
            {
                return false;
            }
            return true;
        }
        //internal static void MES_Do_FatahLists(MainViewModel ourviewmodel,
        //                                    UtilityViewModel utilityviewmodel)
        //{
        //    // Build a 'local' version of Tariffs table
        //    // However - tariffs are attached to Suppliers (and separated by the
        //    // Tariff Area Matrix) so we need to find the Supplier attached to the 'brand'

        //    //utilityviewmodel.scalarsList = ourviewmodel.Fatah.scalarsList;
        //    // As is this
        //    utilityviewmodel.vatRatesList = ourviewmodel.Fatah.vatRatesList;
        //    return;
        //}

        
        internal static bool PS_GetAddress(
                                            UtilityViewModel utilityviewmodel,
                                            string json,
                                            string target_postcode,
                                            string target_address,
                                            GenericAddress ADDRESS)
        {
            utilityviewmodel.udprn = "";
            int property_match = 0;
            bool decode_meter = false;

            // Normalize target address
            string[] target = target_address.Split(SmartParametersV2016.spaceSplit);
            for (int i = 0; i < target.Length; i++)
                target[i] = target[i].TrimEnd(Convert.ToChar(SmartParametersV2016.comma));

            if (string.IsNullOrEmpty(json))
                return false;

            // Clean potential junk before first brace
            int curly_brace = json.IndexOf("{");
            if (curly_brace >= 0)
                json = json.Substring(curly_brace);

            using JsonDocument jsonDoc = JsonDocument.Parse(json);
            JsonElement root = jsonDoc.RootElement;

            if (!root.TryGetProperty("Addresses", out JsonElement addresses))
                return false;

            foreach (JsonElement entry1 in addresses.EnumerateArray())
            {
                foreach (JsonElement addressEntry in entry1.EnumerateArray())
                {
                    foreach (JsonProperty entry2 in addressEntry.EnumerateObject())
                    {
                        if (entry2.Name == "Address")
                        {
                            JsonElement addressData = entry2.Value;
                            string value = addressData.GetProperty("Value").GetString() ?? "";
                            string text = addressData.GetProperty("Text").GetString() ?? "";

                            string possible_udprnzz = "";
                            string building_name = "", building_number = "", county = "",
                                   double_dependant_locality = "", dependant_thoroughfare = "",
                                   dependant_locality = "", organization = "", code = "",
                                   postcode = "", pobox = "", sub_building_name = "",
                                   thoroughfare = "", town = "";

                            foreach (JsonElement part in addressData.GetProperty("AddressParts").EnumerateArray())
                            {
                                string partName = part.GetProperty("Name").GetString() ?? "";
                                string partValue = part.GetProperty("Value").GetString() ?? "";

                                switch (partName)
                                {
                                    case "AKEY": possible_udprnzz = partValue; break;
                                    case "BNAM": building_name = partValue; break;
                                    case "BNUM": building_number = partValue; break;
                                    case "CNTY": county = partValue; break;
                                    case "DDLO": double_dependant_locality = partValue; break;
                                    case "DEPT": dependant_thoroughfare = partValue; break;
                                    case "DPLO": dependant_locality = partValue; break;
                                    case "DPTH": dependant_thoroughfare = partValue; break;
                                    case "ORGN": organization = partValue; break;
                                    case "OUTC": code = partValue; break;
                                    case "PCOD": postcode = partValue; break;
                                    case "PBOX": pobox = partValue; break;
                                    case "SUBB": sub_building_name = partValue; break;
                                    case "THOR": thoroughfare = partValue; break;
                                    case "TOWN": town = partValue; break;
                                }
                            }

                            // Compare text to target
                            int match_count = 0;
                            string solid_location = text.Replace(target_postcode, "").Trim();
                            string[] comparison = solid_location.Split(SmartParametersV2016.spaceSplit);

                            foreach (string tar in target)
                            {
                                foreach (string comp in comparison)
                                {
                                    if (string.Equals(tar, comp, StringComparison.OrdinalIgnoreCase))
                                    {
                                        match_count++;
                                        break;
                                    }
                                }
                            }

                            // Update utilityviewmodel and ADDRESS if better match
                            if (string.IsNullOrEmpty(utilityviewmodel.udprn) || match_count > property_match)
                            {
                                utilityviewmodel.udprn = SmartRoutinesV2018.FixUDPRN(possible_udprnzz);
                                property_match = match_count;
                                utilityviewmodel.property_value = value;
                                utilityviewmodel.House_No = building_number;
                                utilityviewmodel.House_Name = building_name;
                                utilityviewmodel.Street = thoroughfare;
                                utilityviewmodel.City_Town = town;

                                ADDRESS.text = text;
                                ADDRESS.value = value;
                                ADDRESS.udprn = utilityviewmodel.udprn;
                                ADDRESS.building_name = building_name;
                                ADDRESS.building_number = building_number;
                                ADDRESS.county = county;
                                ADDRESS.double_dependant_locality = double_dependant_locality;
                                ADDRESS.dependant_thoroughfare = dependant_thoroughfare;
                                ADDRESS.dependant_locality = dependant_locality;
                                ADDRESS.organization = organization;
                                ADDRESS.code = code;
                                ADDRESS.postcode = postcode;
                                ADDRESS.pobox = pobox;
                                ADDRESS.sub_building_name = sub_building_name;
                                ADDRESS.thoroughfare = thoroughfare;
                                ADDRESS.town = town;

                                decode_meter = true;
                            }
                        }

                        // Handle Meters
                        if (entry2.Name == "Meters" && decode_meter)
                        {
                            foreach (JsonElement meterType in entry2.Value.EnumerateArray())
                            {
                                foreach (JsonProperty resource in meterType.EnumerateObject())
                                {
                                    if (resource.Name == "Electricity")
                                        ParseElectricityMeters(resource.Value, ADDRESS);
                                    else if (resource.Name == "Gas")
                                        ParseGasMeters(resource.Value, ADDRESS);
                                }
                            }
                            decode_meter = false;
                        }
                    }
                }
            }

            return true;
        }

        // Helper for electricity meters
        private static void ParseElectricityMeters(JsonElement electricity, GenericAddress ADDRESS)
        {
            foreach (JsonElement meterInfo in electricity.EnumerateArray())
            {
                bool valid = meterInfo.GetProperty("Valid").GetBoolean();
                if (!valid)
                {
                    ADDRESS.electricity.meter_serial_no = "";
                    ADDRESS.electricity.MPAN = "";
                }

                foreach (JsonElement meterDetails in meterInfo.GetProperty("MeterInformation").EnumerateArray())
                {
                    foreach (JsonProperty meterEntry in meterDetails.EnumerateObject())
                    {
                        if (meterEntry.Name == "01")
                        {
                            foreach (JsonElement meterData in meterEntry.Value.EnumerateArray())
                            {
                                foreach (JsonProperty meterField in meterData.EnumerateObject())
                                {
                                    switch (meterField.Name)
                                    {
                                        case "MeterSerialNumber":
                                            ADDRESS.electricity.meter_serial_no = meterField.Value.GetString() ?? "";
                                            break;
                                        case "MPAN":
                                            string mpan = "";
                                            foreach (JsonElement mpanParts in meterField.Value.EnumerateArray())
                                            {
                                                foreach (JsonProperty part in mpanParts.EnumerateObject())
                                                {
                                                    switch (part.Name)
                                                    {
                                                        case "DistId":
                                                            mpan += part.Value.GetString();
                                                            ADDRESS.country = part.Value.GetString() switch
                                                            {
                                                                "17" or "18" => "Scotland",
                                                                "21" => "Wales",
                                                                _ => "England"
                                                            };
                                                            break;
                                                        case "Ident1":
                                                        case "Ident2":
                                                        case "CheckDigit":
                                                            mpan += part.Value.GetString();
                                                            break;
                                                    }
                                                }
                                            }
                                            ADDRESS.electricity.MPAN = mpan;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Helper for gas meters
        private static void ParseGasMeters(JsonElement gas, GenericAddress ADDRESS)
        {
            foreach (JsonElement meterInfo in gas.EnumerateArray())
            {
                bool valid = meterInfo.GetProperty("Valid").GetBoolean();
                if (!valid)
                {
                    ADDRESS.gas.meter_serial_no = "";
                    ADDRESS.gas.MPRN = "";
                }

                foreach (JsonElement meterDetails in meterInfo.GetProperty("MeterInformation").EnumerateArray())
                {
                    foreach (JsonProperty meterEntry in meterDetails.EnumerateObject())
                    {
                        if (meterEntry.Name == "01")
                        {
                            foreach (JsonElement meterData in meterEntry.Value.EnumerateArray())
                            {
                                foreach (JsonProperty meterField in meterData.EnumerateObject())
                                {
                                    switch (meterField.Name)
                                    {
                                        case "MeterSerialNumber":
                                            ADDRESS.gas.meter_serial_no = meterField.Value.GetString() ?? "";
                                            break;
                                        case "MPRN":
                                            ADDRESS.gas.MPRN = meterField.Value.GetString() ?? "";
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}