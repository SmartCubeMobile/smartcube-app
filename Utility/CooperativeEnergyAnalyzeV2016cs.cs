//using iTextSharp.text.pdf;              // for reading the PDF files (add Itextshapr -> itextsharp.dll)
//using iTextSharp.text.pdf.parser;
using SmartCubeMobile;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartDashboard
{
    class CooperativeEnergyAnalyzeV2016
    {
        public static void ListContentStream(//string pdfFile,
                                            DashboardModel dashboardmodel,
                                             //rf string[] lines,
                                             DateTime prices_valid_from)
        {
            ObservableCollection<string> list_lines = new ObservableCollection<string>();
            string default_package = string.Empty;
            //bool supply_area;// = false;

            //PdfReader reader;// = new PdfReader(pdfFile);
            int maxPageNum = 0;// reader.NumberOfPages;
            for (int pageNum = 1; pageNum <= maxPageNum; pageNum++)
            {
                ListContentStreamForPage(//reader,
                                         //pageNum,
                                            ref list_lines,
                                            //ref supply_area,
                                            default_package,
                                            prices_valid_from);
            }
            dashboardmodel.lines = list_lines.ToArray();
            return;
        }

        public static void ListContentStreamForPage(//PdfReader reader,
                                                    //int pageNum,
                            ref ObservableCollection<string> list_lines,
                            //ref bool supply_area,
                            string default_package,
                            DateTime prices_valid_from)
        {
            // What an absolute fucking bitch this has been ...
            string extractedText;
            string[] pdf_lines;
            string[] fields;
            string heading = string.Empty,
                    vat = string.Empty,
                    rate = string.Empty,
                    effective_date = string.Empty,
                    //options,// = string.Empty,
                    package = string.Empty,
                    resource,// = string.Empty,
                    payment_plan = string.Empty,
                    //supply_code,// = string.Empty,
                    //supply_name,// = string.Empty,
                    csv_line;
            char[] crlf = new char[] { '\r', '\n' };
            char resource_code = ' ',
                    category = ' ';
            int line_index = 0,
                    field_count;
            bool store_it,
                    economy7 = false;

            extractedText = string.Empty; // PdfTextExtractor.GetTextFromPage(reader, pageNum, new LocationTextExtractionStrategy());
            extractedText = extractedText.Replace("\r", String.Empty);

            try
            {
                pdf_lines = extractedText.Split(crlf);
                if (extractedText.Length != 0)
                {
                    while (line_index < pdf_lines.Length)
                    {
                        string line = pdf_lines[line_index].Trim();
                        // Heading
                        if (string.IsNullOrEmpty(heading))
                        {
                            // Only look for the heading as the first line ..
                            if (line_index == 0)
                            {
                                heading = Look_For_Heading(line);
                                category = Look_For_Domestic(line); // Domestic
                            }
                            goto update;
                        }
                        // VAT
                        if (string.IsNullOrEmpty(vat))
                        {
                            vat = Look_For_Vat(line);
                            if (!string.IsNullOrEmpty(vat))
                            {
                                goto update;
                            }
                        }
                        // Category
                        if (category == ' ')
                        {
                            category = Look_For_Domestic(line); // Domestic
                            if (category != ' ')
                            {
                                goto update;
                            }
                        }

                        Look_For_Payment_Plan(line, ref payment_plan);

                        if (string.IsNullOrEmpty(effective_date))
                        {
                            effective_date = Look_For_Effective(line);
                            if (!string.IsNullOrEmpty(effective_date))
                            {
                                goto update;
                            }
                        }

                        resource = Look_For_Resource(line);
                        if (!string.IsNullOrEmpty(resource))
                        {
                            //payment_plan = string.Empty;
                            resource_code = Energy_Decode(resource);
                            economy7 = Look_For_Rate(line, ref rate);
                            // Good to go - add in the Category line
                            //list_lines.Add("Category:" + category);
                            //list_lines.Add("VAT:" + vat);
                            //list_lines.Add("Options:" + options);
                            if (string.IsNullOrEmpty(package))
                            {
                                package = default_package;
                            }
                            //list_lines.Add("Package:" + package);
                            list_lines.Add("Resource: " + resource_code.ToString());
                            if (string.IsNullOrEmpty(effective_date))
                            {
                                effective_date = prices_valid_from.ToString();
                            }

                            list_lines.Add("Prices valid from: " + effective_date);
                            if (string.IsNullOrEmpty(payment_plan))
                            {
                                payment_plan = "Monthly Direct Debit";
                            }
                            list_lines.Add(payment_plan);
                            if (economy7)
                            {
                                list_lines.Add("Economy7: " + "true");
                            }
                            goto update;
                        }
                        // Now try to skip some column heading lines
                        if ((resource_code != ' ') &&
                            (!string.IsNullOrEmpty(effective_date)))
                        {
                            if (Look_For_Column_Heading(line) ||
                            (string.IsNullOrEmpty(line)))
                            {
                                goto update;
                            }

                            // For when PDF fucks up and splits a line putting the second half first
                            if (line.Substring(0, 1) == ")")
                            {
                                line_index++;
                                line = pdf_lines[line_index].Trim() + line;
                            }
                            else
                            {
                                if ((line.Substring(0, 1) == "1") ||
                                    (line.Substring(0, 1) == "2"))
                                {
                                    line = line.Substring(2, line.Length - 2).Trim();
                                }
                            }

                            Underscore_Rate(ref line, rate);

                            line = line.Trim(' ');
                            fields = line.Split(' ');
                            field_count = 0;
                            store_it = true;
                            csv_line = String.Empty;
                            while (field_count < fields.Length)
                            {
                                fields[field_count] = fields[field_count].Trim(' ');
                                switch (resource_code)
                                {
                                    case SmartParametersV2016.Electricity:
                                        store_it = E_Line(vat,
                                                            economy7,
                                                            field_count,
                                                            fields,
                                                            ref csv_line,
                                                            ref store_it);
                                        break;
                                    case SmartParametersV2016.Gas:
                                        store_it = G_Line(vat,
                                                            field_count,
                                                            fields,
                                                            ref csv_line,
                                                            ref store_it);
                                        break;
                                    default:
                                        break;
                                }
                                field_count++;
                            }
                            if (!store_it)
                            {
                                // Reset this
                                resource_code = ' ';
                            }
                            else
                            {
                                list_lines.Add(csv_line);
                            }
                        }
                    update:
                        line_index++;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                //Engine_Test_Error.Text = e.ToString();
            }
        }

        // She is SUCH A FUCKING NOISY BITCH - she can't do **ANYTHING** quietly

        public static bool Look_For_Column_Heading(string line)
        {
            string column_heading1 = "Region Electricity Price",
                    column_heading2 = "pence per kWh",
                    column_heading3 = "Region Economy 7",
                    column_heading4 = "(use your electricity",
                    column_heading5 = "Region Gas Price Gas",
                    column_heading6 = "£ per annum";
            int heading_index;

            heading_index = line.IndexOf(column_heading1, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            heading_index = line.IndexOf(column_heading2, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            heading_index = line.IndexOf(column_heading3, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            heading_index = line.IndexOf(column_heading4, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            heading_index = line.IndexOf(column_heading5, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            heading_index = line.IndexOf(column_heading6, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            return false;
        }

        public static string Look_For_Heading(string line)
        {
            string heading1 = "Co-operative Energy Prices";
            int heading_index;

            heading_index = line.IndexOf(heading1, 0);
            if (heading_index >= 0)
            {
                return heading1;
            }
            return string.Empty;
        }

        public static char Look_For_Domestic(string line)
        {
            if ((line.IndexOf("domestic", 0) >= 0) ||
                (line.IndexOf("Domestic", 0) >= 0))
            {
                return 'D'; // Domestic
            }
            else
            {
                return ' ';
            }
        }

        public static void Look_For_Payment_Plan(string line, ref string payment_plan)
        {
            if ((line.IndexOf("direct debit", 0) >= 0) ||
                (line.IndexOf("Direct Debit", 0) >= 0))
            {
                payment_plan = "Monthly Direct Debit"; // Plan code A
            }
            else
            {
                if ((line.IndexOf("quarterly", 0) >= 0) ||
                (line.IndexOf("Quarterly", 0) >= 0))
                {
                    payment_plan = "Pay on receipt of Bill"; // Plan code F
                }

                // Usual NOISY FUCKING BASHING and CLATTERING around the kitchen BITCH
                else
                {
                    if ((line.IndexOf("prepayment", 0) >= 0) ||
                        (line.IndexOf("Prepayment", 0) >= 0))
                    {
                        payment_plan = "Prepayment"; // Plan code F
                    }
                }
            }
        }

        public static string Look_For_Vat(string line)
        {
            // For some peculiar PDF reason this string doesn't parse the spaces correctly ...
            if ((line.IndexOf("prices") >= 0) &&
                (line.IndexOf("include") >= 0) &&
                (line.IndexOf("VAT") >= 0))
            {
                return "Inclusive";
            }
            else
            {
                if ((line.IndexOf("prices") >= 0) &&
                    (line.IndexOf("exclude") >= 0) &&
                    (line.IndexOf("VAT") >= 0))
                {
                    return "Exclusive";
                }
            }
            return string.Empty;
        }

        public static string Look_For_Effective(string line)
        {
            string effective_date,// = string.Empty,
                    effective_from = "w.e.f";
            int effective_index;

            line = line.Replace("signing up from", effective_from);
            effective_index = line.IndexOf("(", 0);
            if (effective_index >= 0)
            {
                effective_date = line.Substring(effective_index + 1, line.Length - effective_index - 1);
                effective_index = effective_date.IndexOf(effective_from, 0);
                if (effective_index >= 0)
                {
                    effective_date = effective_date.Substring(effective_index +
                                                effective_from.Length,
                                                effective_date.Length -
                                                effective_index -
                                                effective_from.Length);
                }
                effective_date = effective_date.Replace("available", string.Empty);
                effective_date = effective_date.Replace("prices", string.Empty);
                effective_date = effective_date.Replace("from", string.Empty);
                effective_date = effective_date.Replace("(", string.Empty);
                effective_date = effective_date.Replace(")", string.Empty);
                effective_date = effective_date.Replace(".", string.Empty);
                effective_date = effective_date.Replace("st", string.Empty);
                effective_date = effective_date.Replace("nd", string.Empty);
                effective_date = effective_date.Replace("rd", string.Empty);
                effective_date = effective_date.Replace("th", string.Empty);
                int to = effective_date.IndexOf("to ", 0);
                if (to >= 0)
                {
                    effective_date = effective_date.Substring(0, to);
                }
                effective_date = effective_date.Trim();
                string[] components;
                components = effective_date.Split(' ');
                if (components.Length == 3)
                {
                    effective_date = components[0];
                    effective_date = effective_date + " " + components[1].Substring(0, 3);
                    effective_date = effective_date + " " + components[2];
                }

                return effective_date;
            }
            return string.Empty;
        }

        public static string Look_For_Resource(string line)
        {
            string electricity_prices = "Electricity Prices",
                    gas_prices = "Gas Prices";
            int resource_index;

            resource_index = line.IndexOf(electricity_prices, 0);
            if (resource_index >= 0)
            {
                return electricity_prices;
            }
            else
            {
                resource_index = line.IndexOf(gas_prices, 0);
                if (resource_index >= 0)
                {
                    return gas_prices;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static char Energy_Decode(string resource)
        {
            if (resource.IndexOf("Electricity") >= 0)
            {
                return SmartParametersV2016.Electricity;
            }
            else
            {
                if (resource.IndexOf("Gas") >= 0)
                {
                    return SmartParametersV2016.Gas;
                }
            }
            return ' ';
        }

        public static bool Look_For_Rate(string line, ref string rate)
        {
            if (line.IndexOf("single rate") >= 0)
            {
                rate = "Single_Rate";
                return false;
            }
            else
            {
                if (line.IndexOf("two rate") >= 0)
                {
                    rate = "Two_Rate";
                    return true;
                }
            }
            rate = string.Empty;
            return false;
        }

        // JUST WHAT I WANTED - The fucking radio is on A-G-A-I-N
        //                                              =========
        public static void Underscore_Rate(ref string line, string rate)
        {
            // Find the Area Id and bring it to the front
            int left_paren_index = line.IndexOf("(", 0);
            if (left_paren_index == -1)
            {
                if (line.IndexOf("North East England") >= 0)
                {
                    line = line.Replace("North East England", "North East England (15)");
                }
                else
                {
                    if (line.IndexOf("South East England") >= 0)
                    {
                        line = line.Replace("South East England", "South East England (19)");
                    }
                    else
                    {
                        line = line.Replace("East England", "East England (10)");
                        line = line.Replace("East Midlands", "East Midlands (11)");
                        line = line.Replace("London", "London (12)");
                        line = line.Replace("North Wales, Merseyside and Cheshire", "North Wales, Merseyside and Cheshire (13)");
                        line = line.Replace("West Midlands", "West Midlands (14)");
                        line = line.Replace("North West England", "North West England (16)");
                        line = line.Replace("North Scotland", "North Scotland (17)");
                        line = line.Replace("South Scotland", "South Scotland (18)");
                        line = line.Replace("Southern England", "Southern England (20)");
                        line = line.Replace("South Wales", "South Wales (21)");
                        line = line.Replace("South West England", "South West England (22)");
                        line = line.Replace("Yorkshire", "Yorkshire (23)");
                    }
                }
            }

            left_paren_index = line.IndexOf("(", 0);
            if (left_paren_index >= 0)
            {
                int right_paren_index = line.IndexOf(")", 0);
                if (right_paren_index >= 0)
                {
                    string area_id = line.Substring(left_paren_index,
                                                    right_paren_index - left_paren_index + 1);
                    right_paren_index++;
                    line = area_id + " " +
                            line.Substring(0, left_paren_index) +
                            rate + " 0" +
                            line.Substring(right_paren_index,
                                            line.Length - right_paren_index);
                    line = line.Replace("(", string.Empty);
                    line = line.Replace(")", string.Empty);
                    line = SmartParseV2016.Remove_Double_Spaces_V3(line);
                    line = line.Replace("East England", "East_England");
                    line = line.Replace("East Midlands", "East_Midlands");
                    line = line.Replace("West Midlands", "West_Midlands");
                    line = line.Replace("North Wales, Merseyside and Cheshire", "North_Wales_Merseyside_and_Cheshire");
                    line = line.Replace("North East_England", "North_East_England");
                    line = line.Replace("North West England", "North_West_England");
                    line = line.Replace("North Scotland", "North_Scotland");
                    line = line.Replace("South Scotland", "South_Scotland");
                    line = line.Replace("South East_England", "South_East_England");
                    line = line.Replace("Southern England", "Southern_England");
                    line = line.Replace("South Wales", "South_Wales");
                    line = line.Replace("South West England", "South_West_England");
                }
            }
        }

        public static bool E_Line(string vat,
                            bool economy7,
                            int field_count,
                            String[] fields,
                            ref string csv_line,
                            ref bool store_it)
        {
            decimal field;// = 0.0M;
            string field_s;// = string.Empty;

            if (!economy7)
            {
                switch (field_count)
                {
                    case 0:
                        // Supply Area Code
                        csv_line += fields[field_count];
                        if (csv_line.Length != 2)
                        {
                            return false;
                        }
                        break;
                    case 1:
                        // Supply Area Name
                        //if (store_it)
                        //{
                        //    csv_line = csv_line + "comma" + fields[field_count];
                        //}
                        break;
                    case 2:
                        // Meter Type
                        //if (store_it)
                        //{
                        //    csv_line = csv_line + "comma" + fields[field_count];
                        //}
                        break;
                    case 3:
                        // Yearly Service Charge in pounds (inc VAT)
                        if (store_it)
                        {
                            field_count = 5;
                            // Remove . to make it pence
                            fields[field_count] = fields[field_count].Replace(SmartParametersV2016.decimalPoint, String.Empty);
                            fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                            if (fields[field_count] == "0")
                            {
                                fields[field_count] = "0.0";
                            }
                            field = Convert.ToDecimal(fields[field_count]);
                            if (!string.IsNullOrEmpty(vat))
                            {
                                field /= 1.05M;
                            }
                            field /= 365.0M;
                            field_s = field.ToString("0.000");
                            csv_line = csv_line + '\t' + field_s;
                        }
                        break;
                    case 4:
                        // AllDay kWh (inc VAT)
                        if (store_it)
                        {
                            fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                            if (fields[field_count] == "0")
                            {
                                fields[field_count] = "0.0";
                            }
                            field = Convert.ToDecimal(fields[field_count]);
                            if (!string.IsNullOrEmpty(vat))
                            {
                                field /= 1.05M;
                            }
                            field_s = field.ToString("0.000");
                            csv_line = csv_line + '\t' + field_s;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (field_count)
                {
                    case 0:
                        // Supply Area Code
                        csv_line += fields[field_count];
                        if (csv_line.Length != 2)
                        {
                            return false;
                        }
                        break;
                    case 1:
                        // Supply Area Name
                        //if (store_it)
                        //{
                        //    csv_line = csv_line + "comma" + fields[field_count];
                        //}
                        break;
                    case 2:
                        // Meter Type
                        //if (store_it)
                        //{
                        //    csv_line = csv_line + "comma" + fields[field_count];
                        //}
                        break;
                    case 3:
                        // Yearly Service Charge in pounds (inc VAT)
                        if (store_it)
                        {
                            field_count = 6;
                            // Remove . to make it pence
                            fields[field_count] = fields[field_count].Replace(SmartParametersV2016.decimalPoint, String.Empty);
                            fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                            if (fields[field_count] == "0")
                            {
                                fields[field_count] = "0.0";
                            }
                            field = Convert.ToDecimal(fields[field_count]);
                            if (!string.IsNullOrEmpty(vat))
                            {
                                field /= 1.05M;
                            }
                            field /= 365.0M;
                            field_s = field.ToString("0.000");
                            csv_line = csv_line + '\t' + field_s;
                        }
                        break;
                    case 4:
                        // AllDay kWh (incc VAT)
                        if (store_it)
                        {
                            fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                            if (fields[field_count] == "0")
                            {
                                fields[field_count] = "0.0";
                            }
                            field = Convert.ToDecimal(fields[field_count]);
                            if (!string.IsNullOrEmpty(vat))
                            {
                                field /= 1.05M;
                            }
                            field_s = field.ToString("0.000");
                            csv_line = csv_line + '\t' + field_s;
                        }
                        break;
                    case 5:
                        // Night Rate/Low/Off-Peak (inc VAT)
                        if (store_it)
                        {
                            fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                            if (fields[field_count] == "0")
                            {
                                fields[field_count] = "0.0";
                            }
                            field = Convert.ToDecimal(fields[field_count]);
                            if (!string.IsNullOrEmpty(vat))
                            {
                                field /= 1.05M;
                            }
                            field_s = field.ToString("0.000");
                            csv_line = csv_line + '\t' + field_s;
                        }
                        break;
                    default:
                        break;
                }
            }
            return store_it;
        }

        public static bool G_Line(string vat,
                                    int field_count,
                                    String[] fields,
                                    ref string csv_line,
                                    ref bool store_it)
        {
            decimal field;// = 0.0M;
            string field_s;// = string.Empty;
            switch (field_count)
            {
                case 0:
                    // Supply Area Code
                    csv_line = fields[field_count];
                    if (csv_line.Length != 2)
                    {
                        return false;
                    }
                    break;
                case 1:
                    // Supply Area Name
                    //if (store_it)
                    //{
                    //    csv_line = csv_line + "comma" + fields[field_count];
                    //}
                    break;
                case 2:
                    // Yearly Service Charge in pounds (inc VAT)
                    if (store_it)
                    {
                        field_count = 4;
                        // Remove . to make it pence
                        fields[field_count] = fields[field_count].Replace(SmartParametersV2016.decimalPoint, String.Empty);
                        fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                        if (fields[field_count] == "0")
                        {
                            fields[field_count] = "0.0";
                        }
                        field = Convert.ToDecimal(fields[field_count]);
                        if (!string.IsNullOrEmpty(vat))
                        {
                            field /= 1.05M;
                        }
                        field /= 365.0M;
                        field_s = field.ToString("0.000");
                        csv_line = csv_line + '\t' + field_s;
                    }
                    break;
                case 3:
                    // AllDay kWh (inc VAT)
                    if (store_it)
                    {
                        fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                        if (fields[field_count] == "0")
                        {
                            fields[field_count] = "0.0";
                        }
                        field = Convert.ToDecimal(fields[field_count]);
                        if (!string.IsNullOrEmpty(vat))
                        {
                            field /= 1.05M;
                        }
                        field_s = field.ToString("0.000");
                        csv_line = csv_line + '\t' + field_s;
                    }
                    break;
                default:
                    break;
            }
            return store_it;
        }
    }
}

