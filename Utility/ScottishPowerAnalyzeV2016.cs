using iText.Kernel.Pdf;
//using iTextSharp.text.pdf;              // for reading the PDF files (add Itextshapr -> itextsharp.dll)
//using iTextSharp.text.pdf.parser;
using SmartCubeMobile;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SmartDashboard
{
    internal class ScottishPowerAnalyzeV2016
    {

        //  Writes information about each page in a PDF file to the specified output stream.
        //  @since 2.1.5
        //  @param pdfFile   a File instance referring to a PDF file
        //  @param out       the output stream to send the content to
        //  @throws IOException
        internal static void ListContentStream(DashboardModel dashboardmodel,
                                                string pdfFile,
                                                //rf string[] lines,
                                                //rf string filePath_s,
                                                string resource_type,
                                                bool two_tier,
                                                string short_format,
                                                List<SmartUtility.PaymentPlans> payment_plans_list)
        {
            dashboardmodel.list_lines = new List<string>();
            string default_package = string.Empty;
            dashboardmodel.supply_area = false;

            PdfReader reader = new PdfReader(pdfFile);
            int maxPageNum = 0; // reader.NumberOfPages;
            if (pdfFile.IndexOf("StandardDomesticPrices") >= 0)
            {
                // Each one should have a package name
                default_package = string.Empty;
            }
            if (pdfFile.IndexOf("SimplyGreenEnergy") >= 0)
            {
                default_package = "Simply Green Energy";
            }
            if (pdfFile.IndexOf("FixedSaverv1s") >= 0)
            {
                default_package = "Fixed Saver June 2013";
            }
            if (pdfFile.IndexOf("L201307_PlatinumFixedEnergy") >= 0)
            {
                default_package = "Plantinum Fixed Energy July 2013";
            }
            if (pdfFile.IndexOf("L201404_FixedPriceV4") >= 0)
            {
                default_package = "Fixed Price Energy January 2015";
            }
            if (pdfFile.IndexOf("SCP3345DiscountedEnergy") >= 0)
            {
                default_package = "Discounted Energy January 2015";
            }

            StreamWriter sw_e = null,
                            sw_e7 = null,
                            sw_g = null;

            dashboardmodel.filePath_s = pdfFile.Replace(".pdf", string.Empty);
            string filePath_e = dashboardmodel.filePath_s + "_E.txt";
            if (File.Exists(filePath_e))
            {
                // Create a file to write to. 
                File.Delete(filePath_e);
            }

            if (!File.Exists(filePath_e))
            {
                // Create a file to write to. 
                sw_e = File.CreateText(filePath_e);
                sw_e.WriteLine("// Created: " + DateTime.Now + " ..you ARE a fucking genius, Ray!");
                sw_e.Close();
            }
            sw_e = new StreamWriter(filePath_e, true);

            string filePath_e7 = dashboardmodel.filePath_s + "_E7.txt";
            if (File.Exists(filePath_e7))
            {
                // Create a file to write to. 
                File.Delete(filePath_e7);
            }

            if (!File.Exists(filePath_e7))
            {
                // Create a file to write to. 
                sw_e7 = File.CreateText(filePath_e7);
                sw_e7.WriteLine("// Created: " + DateTime.Now + " ..you ARE a fucking genius, Ray!");
                sw_e7.Close();
            }
            sw_e7 = new StreamWriter(filePath_e7, true);

            string filePath_g = dashboardmodel.filePath_s + "_G.txt";
            if (File.Exists(filePath_g))
            {
                // Create a file to write to. 
                File.Delete(filePath_g);
            }

            if (!File.Exists(filePath_g))
            {
                // Create a file to write to. 
                sw_g = File.CreateText(filePath_g);
                sw_g.WriteLine("// Created: " + DateTime.Now + " ..you ARE a fucking genius, Ray!");
                sw_g.Close();
            }
            sw_g = new StreamWriter(filePath_g, true);

            for (int pageNum = 1; pageNum <= maxPageNum; pageNum++)
            {
                // All because one of the Supply Area pages for
                // Standard Prices has NO FUCKING INFORMATION ON IT
                // to say it is a continuation fucking Supply Area page
                if ((dashboardmodel.supply_area) &&
                    (string.IsNullOrEmpty(default_package)))
                {
                    break;
                }
                else
                {
                    ListContentStreamForPage(dashboardmodel,
                                                sw_e,
                                                sw_e7,
                                                sw_g,
                                                reader, pageNum,
                                                //rf list_lines,
                                                //rf supply_area,
                                                default_package,
                                                resource_type,
                                                two_tier,
                                                short_format,
                                                payment_plans_list);
                }
            }
            dashboardmodel.lines = dashboardmodel.list_lines.ToArray();
            sw_e.Close();
            sw_e7.Close();
            sw_g.Close();
            return;
        }

        internal static void ListContentStreamForPage(DashboardModel dashboardmodel,
                                                    StreamWriter sw_e,
                                                    StreamWriter sw_e7,
                                                    StreamWriter sw_g,
                                                    PdfReader reader,
                                                    int pageNum,
                                                    //rf List<string> list_lines,
                                                    //rf bool supply_area,
                                                    string default_package,
                                                    string resource_type,
                                                    bool two_tier,
                                                    string short_format,
                                                    List<SmartUtility.PaymentPlans> payment_plans_list)
        {
            // What an absolute fucking bitch this has been ...
            String extractedText;
            String[] pdf_lines;
            String[] fields;
            string heading = string.Empty,
                    effective_date = string.Empty,
                    options = string.Empty,
                    package = string.Empty,
                    resource = string.Empty,
                    payment_plan = string.Empty,
                    supply_code = string.Empty,
                    supply_name = string.Empty,
                    text_line;
            char[] crlf = new char[] { '\r', '\n' };
            char resource_code = ' ',
                    category = ' ';
            int line_index = 0,
                    field_count;
            bool store_it;

            string economy7 = string.Empty; // For G

            extractedText = null; // PdfTextExtractor.GetTextFromPage(reader, pageNum, new LocationTextExtractionStrategy());
            extractedText = extractedText.Replace("\r", String.Empty);

            try
            {
                pdf_lines = extractedText.Split(crlf);
                if (extractedText.Length != 0)
                {
                    if (pdf_lines.Count() > 0)
                    {
                        if (pdf_lines[0] == "The enclosed booklet details our new domestic gas and")
                        {
                            // Ignore this first page as it contains parts of table
                            // which confuse the logic below!!  These are the pages
                            // which tell the Consumer how to look up the tables!!
                            return;
                        }
                    }
                    while (line_index < pdf_lines.Count())
                    {
                        string line = pdf_lines[line_index].Trim(' ');
                        // Heading
                        if (string.IsNullOrEmpty(heading))
                        {
                            // Look for the heading on any line ..
                            heading = Look_For_Heading(line);
                            goto update;
                        }
                        // Category
                        if (category == ' ')
                        {
                            category = Look_For_Domestic(line); // Domestic
                            if (line_index > 0)
                            {
                                effective_date = Look_For_Effective(line, pdf_lines[line_index - 1].Trim(' '));
                            }
                            goto update;
                        }
                        if (string.IsNullOrEmpty(effective_date))
                        {
                            if (line_index > 0)
                            {
                                effective_date = Look_For_Effective(line, pdf_lines[line_index - 1].Trim(' '));
                            }
                            goto update;
                        }
                        Look_For_Package(line, ref package);
                        Look_For_Payments(line, ref payment_plan, payment_plans_list);
                        Look_For_Options(line, two_tier, ref options);
                        dashboardmodel.supply_area = Look_For_Supply(ref line,
                                                            ref supply_code,
                                                            ref supply_name);
                        if (string.IsNullOrEmpty(resource))
                        {
                            resource = Look_For_Resource(line);
                            if (!string.IsNullOrEmpty(resource))
                            {
                                resource_code = Energy_Decode(resource);
                                // Good to go - add in the Category line
                                if (!dashboardmodel.supply_area)
                                {
                                    switch (resource_code)
                                    {
                                        case SmartParametersV2016.Electricity:
                                            // When resource_code = "D" that means Dual-Rate i.e. E7
                                            sw_e7.WriteLine();
                                            sw_e7.WriteLine("Resource: " + resource_code);
                                            // Here we attempt to open the output file on the path passed in
                                            sw_e7.WriteLine("Valid from: " + Convert.ToDateTime(effective_date).ToString(short_format));
                                            sw_e7.WriteLine("Prices exclude VAT");
                                            sw_e7.WriteLine("Category: " + category);
                                            if (string.IsNullOrEmpty(package))
                                            {
                                                package = default_package;
                                            }
                                            sw_e7.WriteLine("Package: " + package);

                                            sw_e7.WriteLine("Payment Plan: " + payment_plan);
                                            //sw_e7.WriteLine("Options: " + options);

                                            // When resource_code = "D" that means Dual-Rate i.e. E7
                                            sw_e.WriteLine();
                                            sw_e.WriteLine("Resource: " + resource_code);
                                            // Here we attempt to open the output file on the path passed in
                                            sw_e.WriteLine("Valid from: " + Convert.ToDateTime(effective_date).ToString(short_format));
                                            sw_e.WriteLine("Prices exclude VAT");
                                            sw_e.WriteLine("Category: " + category);
                                            if (string.IsNullOrEmpty(package))
                                            {
                                                package = default_package;
                                            }
                                            sw_e.WriteLine("Package: " + package);

                                            sw_e.WriteLine("Payment Plan: " + payment_plan);
                                            //sw_e.WriteLine("Options: " + options);
                                            break;
                                        case SmartParametersV2016.Gas:
                                            // When resource_code = "D" that means Dual-Rate i.e. E7
                                            sw_g.WriteLine();
                                            sw_g.WriteLine("Resource: " + resource_code);
                                            // Here we attempt to open the output file on the path passed in
                                            sw_g.WriteLine("Valid from: " + Convert.ToDateTime(effective_date).ToString(short_format));
                                            sw_g.WriteLine("Prices exclude VAT");
                                            sw_g.WriteLine("Category: " + category);
                                            if (string.IsNullOrEmpty(package))
                                            {
                                                package = default_package;
                                            }
                                            sw_g.WriteLine("Package: " + package);

                                            sw_g.WriteLine("Payment Plan: " + payment_plan);
                                            //sw_g.WriteLine("Options: " + options);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            goto update;
                        }
                        // Now try to skip some column heading lines
                        if (Look_For_Column_Heading(line))
                        {
                            goto update;
                        }

                        line = line.Replace("¥", string.Empty);
                        if (string.IsNullOrEmpty(line))
                        {
                            goto update;
                        }

                        if (dashboardmodel.supply_area)
                        {
                            if (line.IndexOf("The Gas & Electricity Offer", 0) == -1)
                            {
                                line = supply_code + " " +
                                        supply_name.Replace(" ", "_") + " " +
                                        line;
                            }
                        }
                        Underscore_Rate(ref line);
                        line = line.Replace("---", "0p");
                        line = line.Trim(' ');
                        fields = line.Split(' ');
                        field_count = 0;
                        store_it = true;
                        text_line = string.Empty;
                        while (field_count < fields.Count())
                        {
                            fields[field_count] = fields[field_count].Trim(' ');
                            switch (resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    store_it = E_Line(field_count,
                                                        fields,
                                                        ref text_line,
                                                        ref economy7);
                                    break;
                                case SmartParametersV2016.Gas:
                                    economy7 = string.Empty;
                                    store_it = G_Line(field_count,
                                                        fields,
                                                        ref text_line);
                                    break;
                                default:
                                    break;
                            }
                            field_count = field_count + 1;
                        }
                        if (!string.IsNullOrEmpty(text_line))
                        {
                            switch (resource_code)
                            {
                                case SmartParametersV2016.Electricity:
                                    if (economy7 == "true")
                                    {
                                        sw_e7.WriteLine(text_line);
                                    }
                                    else
                                    {
                                        sw_e.WriteLine(text_line);
                                    }
                                    break;
                                case SmartParametersV2016.Gas:
                                    sw_g.WriteLine(text_line);
                                    break;
                                default:
                                    break;
                            }
                        }

                        // Mrs Fucking Noisy Clumsy Bitch at it again
                        if (!store_it)
                        {
                            // Reset this
                            //resource = string.Empty;
                        }
                        else
                        {
                            if (!dashboardmodel.supply_area)
                            {
                                dashboardmodel.list_lines.Add(text_line);
                            }
                        }
                    update:
                        line_index = line_index + 1;
                    }
                }
            }
            catch (Exception e)
            {
                // Add the error back ... as part of the decoded text!
                // Can you tell - I'm fairly desperate at this stage??!?
                dashboardmodel.list_lines.Add(e.ToString());
            }
            return;
        }

        internal static bool Look_For_Column_Heading(string line)
        {
            string column_heading1 = "VAT VAT",
                    column_heading2 = "Supply Supply",
                    column_heading3 = "Area ",
                    column_heading4 = "Code ",
                    column_heading5 = "quarter ",
                    column_heading6 = "Electricity Daily",
                    column_heading7 = "Rates Service",
                    column_heading8 = "(ScottishPower Area)";
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
            heading_index = line.IndexOf(column_heading7, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            heading_index = line.IndexOf(column_heading8, 0);
            if (heading_index >= 0)
            {
                return true;
            }
            return false;
        }

        internal static string Look_For_Heading(string line)
        {
            string heading1 = "ScottishPower Gas and Electricity Prices",
                    heading2 = "ScottishPower Offer Gas and Electricity Prices",
                    heading3 = "ScottishPower Electricity Prices";
            int heading_index;

            heading_index = line.IndexOf(heading1, 0);
            if (heading_index >= 0)
            {
                return heading1;
            }
            else
            {
                heading_index = line.IndexOf(heading2, 0);
                if (heading_index >= 0)
                {
                    return heading2;
                }
                else
                {
                    heading_index = line.IndexOf(heading3, 0);
                    if (heading_index >= 0)
                    {
                        return heading3;
                    }
                    return string.Empty;
                }
            }
        }

        internal static char Look_For_Domestic(string line)
        {
            if (line.IndexOf("domestic", 0) >= 0)
            {
                return 'D'; // Domestic
            }
            else
            {
                return ' ';
            }
        }

        internal static string Look_For_Effective(string line, string previous_line)
        {
            string effective_date = string.Empty,
                    effective_from = "effective";
            int effective_index;

            effective_index = line.IndexOf(effective_from, 0);
            if (effective_index == -1)
            {
                // join the previous_line to the current one
                line = previous_line + " " + line;
            }
            // Try again
            effective_index = line.IndexOf(effective_from, 0);
            if (effective_index >= 0)
            {
                effective_date = line.Substring(effective_index +
                                            effective_from.Length,
                                            line.Length -
                                            effective_index -
                                            effective_from.Length);
                effective_date = effective_date.Replace("from", string.Empty);
                effective_date = effective_date.Trim(' ');
                effective_date = effective_date.TrimEnd('.');
                effective_date = effective_date.Replace("st", string.Empty);
                effective_date = effective_date.Replace("Augu ", "August ");
                effective_date = effective_date.Replace("nd", string.Empty);
                effective_date = effective_date.Replace("rd", string.Empty);
                effective_date = effective_date.Replace("th", string.Empty);
            }
            return effective_date;
        }

        internal static bool Look_For_Package(string line, ref string package)
        {
            string package1 = " package",
                    package2 = " Package";
            int package_index;
            bool found_one = false;

            package_index = line.IndexOf(package1, 0);
            if (package_index >= 0)
            {
                package = line.Substring(0, package_index);
                found_one = true;
            }
            else
            {
                package_index = line.IndexOf(package2, 0);
                if (package_index >= 0)
                {
                    package = line.Substring(0, package_index);
                    found_one = true;
                }
            }
            return found_one;
        }

        internal static bool Look_For_Payments(string line, ref string payments, List<SmartUtility.PaymentPlans> payment_plans_list)
        {
            // One day you might have to do "Standing Order" for completeless
            // with monthly direct debit on some of the Standard tariffs
            string pay_monthly = "Pay monthly by Direct Debit",
                    pay_quarterly1 = "Pay quarterly by Direct Debit",
                    pay_quarterly2 = "Pay quarterly",
                    pay_weekly1 = "Pay weekly by payment book or card",
                    pay_weekly2 = "weekly by",
                    pay_prepayment = "Pay as you use with a prepayment meter",
                    pay_debit_card = "Debit Card",
                    pay_cash_cheque = "cash, cheque or postal order",
                    local_payments = string.Empty;
            int payments_index;
            bool found_one = false;

            payments_index = line.IndexOf(pay_monthly, 0);
            if (payments_index >= 0)
            {
                local_payments = pay_monthly + ", ";
                found_one = true;
            }
            payments_index = line.IndexOf(pay_quarterly2, 0);
            if (payments_index >= 0)
            {
                local_payments = local_payments + pay_quarterly1 + ", ";
                found_one = true;
            }
            payments_index = line.IndexOf(pay_weekly2, 0);
            if (payments_index >= 0)
            {
                local_payments = local_payments + pay_weekly1 + ", ";
                found_one = true;
            }
            payments_index = line.IndexOf(pay_prepayment, 0);
            if (payments_index >= 0)
            {
                local_payments = local_payments + pay_prepayment + ", ";
                found_one = true;
            }
            payments_index = line.IndexOf(pay_cash_cheque, 0);
            if (payments_index >= 0)
            {
                local_payments = local_payments + "Cash/Cheque/Postal Order" + ", ";
                found_one = true;
            }
            payments_index = line.IndexOf(pay_debit_card, 0);
            if (payments_index >= 0)
            {
                local_payments = local_payments + "Debit Card" + ", ";
                found_one = true;
            }
            if (found_one)
            {
                local_payments = local_payments.TrimEnd(' ');
                payments = local_payments.TrimEnd(',');
                string[] payment = payments.Split(',');
                int payment_count = 0;
                while (payment_count < payment.Count())
                {
                    foreach (SmartUtility.PaymentPlans payment_plans_row in payment_plans_list)
                    {
                        if (payment[payment_count] == payment_plans_row.ALTERNATE_NAME3)
                        {
                            payments = payment_plans_row.PAYMENT_NAME;
                            break;
                        }
                    }
                    payment_count = payment_count + 1;
                }
            }
            return found_one;
        }

        internal static bool Look_For_Options(string line, bool two_tier, ref string options)
        {
            string standing_charge_options = "Standing Charge", // Options",
                    no_standing_charge_options = "No Standing Charge"; // Options";
            int options_index;

            // Do the 'No' one first!!
            options_index = line.IndexOf(no_standing_charge_options, 0);
            if (options_index >= 0)
            {
                //options = no_standing_charge_options;
                options = "Two-Tier";
                return true;
            }
            else
            {
                options_index = line.IndexOf(standing_charge_options, 0);
                if (options_index >= 0)
                {
                    if (two_tier)
                    {
                        options = "Two-Tier";   // Sometimes the ScottishPower gingers headline the tariff
                                                // as 'Standing Charge' when it CLEARLY isn'r
                    }
                    else
                    {
                        options = standing_charge_options;
                    }
                    return true;
                }
            }
            return false;
        }

        internal static bool Look_For_Supply(ref string line,
                                                ref string supply_code,
                                                ref string supply_name)
        {
            string first_two = string.Empty;

            if (line.Length >= 2)
            {
                first_two = line.Substring(0, 2);
                try
                {
                    int supply_no = Convert.ToInt32(first_two);
                    supply_code = supply_no.ToString();
                    line = line.Replace(supply_code, string.Empty).Trim();
                    int space = line.IndexOf(" ");
                    if (space >= 0)
                    {
                        supply_name = line.Substring(0, space);
                        if ((supply_name == "East") ||
                            (supply_name == "Scottish"))
                        {
                            string temp_line = line.Replace(supply_name, string.Empty).Trim();
                            space = temp_line.IndexOf(" ");
                            if (space >= 0)
                            {
                                supply_name = supply_name + " " + temp_line.Substring(0, space);
                            }
                        }
                        line = line.Replace(supply_name, string.Empty).Trim();
                        return true;
                    }
                }
                catch (Exception exception)
                {
                    if (line.Length == 0)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }
            }
            return false;
        }

        internal static string Look_For_Resource(string line)
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

        internal static char Energy_Decode(string resource)
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

        internal static void Underscore_Rate(ref string rate)
        {
            rate = rate.Replace("East Midlands", "East_Midlands");
            rate = rate.Replace("Scottish Hydro", "Scottish_Hydro");
            rate = rate.Replace("Single Rate", "Single_Rate");
            rate = rate.Replace("Two Rate", "Two_Rate");
            rate = rate.Replace("ComfortPlus Control", "ComfortPlus_Control");
            rate = rate.Replace("ComfortPlus White Meter", "ComfortPlus_White_Meter");
            rate = rate.Replace("Domestic & Economy 2000", "Domestic_&_Economy_2000");
            rate = rate.Replace("Domestic & Off Peak ", "Domestic_&_Off_Peak_");
            rate = rate.Replace("Option 14", "Option_14");
            rate = rate.Replace("TwinHeat A and B", "TwinHeat_A_and_B");
            rate = rate.Replace("Economy 7 Plus", "Economy_7_Plus");
            rate = rate.Replace("Economy 7", "Economy_7");
            rate = rate.Replace("White Meter 8", "White_Meter_8");
            rate = rate.Replace("Domestic 'S' & Off Peak ", "Domestic_'S'_&_Off_Peak_");
            rate = rate.Replace("Domestic 'S'", "Domestic_'S'");
            rate = rate.Replace("Domestic ‘S’", "Domestic_'S'");
            rate = rate.Replace("White Meter No.1", "White_Meter_No.1");
            rate = rate.Replace("White Meter No. 1", "White_Meter_No._1");
        }

        internal static bool E_Line(int field_count,
                            String[] fields,
                            ref string text_line,
                            ref string economy7)
        {
            switch (field_count)
            {
                case 0:
                    // Supply Area Code
                    if (fields[field_count].Length != 2)
                    {
                        return false;
                    }
                    text_line = text_line + fields[field_count];
                    break;
                case 1:
                    // Supply Area Name
                    break;
                case 2:
                    // Meter Type
                    switch (fields[field_count])
                    {
                        case "Single_Rate":
                        case "Domestic_'S'":            // means the same as single
                        case "Domestic":                // means the same as single
                            economy7 = "false";
                            break;
                        case "Two_Rate":
                        case "Economy_7":               // means the same as double
                        case "White_Meter_No.1":        // means the same as double
                            economy7 = "true";
                            break;
                        default:
                            break;
                    }
                    break;
                case 3:
                    // Daily Service Charge (exc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    if (fields[field_count] == "0")
                    {
                        fields[field_count] = "0.0";
                    }
                    text_line = text_line + " " + fields[field_count];
                    break;
                case 4:
                    // AllDay kWh (exc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    if (fields[field_count] == "0")
                    {
                        fields[field_count] = "0.0";
                    }
                    text_line = text_line + " " + fields[field_count];
                    break;
                case 5:
                    // Night Rate (exc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    if (fields[field_count] == "0")
                    {
                        fields[field_count] = "0.0";
                    }
                    if (economy7 == "true")
                    {
                        text_line = text_line + " " + fields[field_count];
                    }
                    break;
                case 6:
                    // Daily Service Charge (inc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    if (fields[field_count] == "0")
                    {
                        fields[field_count] = "0.0";
                    }
                    break;
                case 7:
                    // AllDay kWh (inc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    if (fields[field_count] == "0")
                    {
                        fields[field_count] = "0.0";
                    }
                    break;
                case 8:
                    // Night Rate (inc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    if (fields[field_count] == "0")
                    {
                        fields[field_count] = "0.0";
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        internal static bool G_Line(int field_count,
                                    String[] fields,
                                    ref string text_line)
        {
            switch (field_count)
            {
                case 0:
                    // Supply Area Code
                    if (fields[field_count].Length != 2)
                    {
                        return false;
                    }
                    text_line = fields[field_count];
                    break;
                case 1:
                    // Supply Area Name
                    break;
                case 2:
                    // Daily Service Charge (exc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    text_line = text_line + " " + fields[field_count];
                    break;
                case 3:
                    // AllDay kWh (exc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    text_line = text_line + " " + fields[field_count];
                    break;
                case 4:
                    // Daily Service Charge (inc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    break;
                case 5:
                    // AllDay kWh (inc VAT)
                    fields[field_count] = fields[field_count].Replace(SmartParametersV2016.defaultDenominationSymbol.ToString(), String.Empty);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
