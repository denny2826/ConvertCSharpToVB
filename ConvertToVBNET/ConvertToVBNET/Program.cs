using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace ConvertToVBNET
{
    /*
     * Defining a string, boolean, int or float                     
     * Concatenating a string with another                          
     * Replaceing a substring with a string                         
     * The methematical operators: plus, minus, times and divide    
     * Logging a string to the console                              
    */
    class Program
    {
        //".Substring", ".Replace", "string.Concat", "string.Join" are not required from the checklist because they are used the same way in VB.NET
        //Makeing the values to check configurable eventhough the functions supported are limited to sepcs above.
        static List<string> valuesToCheck = ConfigurationManager.AppSettings["ConvertList"].Split(',').ToList();
        static void Main(string[] files)
        {
            //Check for one/more input files, loop them one by one
            foreach (string file in files)
            {
                FileInfo input = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\" +file);
                //Check if file exists
                if (input.Exists)
                {
                    //Define the output file name which is the original file name with a "vb" file extension.
                    string outputFileName = input.FullName.Replace(input.Extension, ".vb");
                    bool bAppend = false;
                    //Check if output file exists
                    if (File.Exists(outputFileName))
                    {
                        //Get answer from user. (Overwrite existing file or append)
                        Console.WriteLine("Output File for " + file + " already exists in folder. Would you like to overwrite (O) or appeend (A) ?");
                        string ans = Console.ReadLine().ToUpper();
                        //Make sure a valid response is enterred by the user
                        while (ans != "O" && ans != "A")
                        {
                            Console.WriteLine("Unknown command. Valid values are: 'O', 'A'.  Would you like to overwrite (O) or appeend (A) ?");
                            ans = Console.ReadLine().ToUpper();
                        }
                        switch (ans)
                        {
                            case "O":
                                break;
                            case "A":
                                bAppend = true;
                                break;
                            default:
                                //Console.WriteLine("Unknown command. Valid values are: 'O', 'A' ");
                                break;
                        }
                    }
                    //Call convert function
                    Convert(file, outputFileName, bAppend);
                }
                else
                {
                    //File doesn't exist, print error
                    Console.WriteLine("Source file does not exist: " + file + " Please verify the file path.");
                }
            }
        }

        static void Convert(string input, string output, bool bAppend)
        {
            StringBuilder sb = new StringBuilder();
            string textLine = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(input))
                {
                    //Read the input file line by line
                    while ((textLine = sr.ReadLine()) != null)
                    {
                        //Check if conversion is required
                        if (valuesToCheck.Any(textLine.Contains))
                        {
                            string iniValue = string.Empty;
                            //Assign into string array and remove empty elements in the array.
                            string[] inputArray = textLine.Replace(";", "").Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            int iEqual = Array.IndexOf(inputArray, "=");
                            //Check if object is initiated
                            if (iEqual > 0)
                            {
                                //Remove everything after the "=" sign
                                iniValue = " = " + inputArray[iEqual + 1];
                                inputArray = inputArray.Where((source, index) => index < iEqual).ToArray();
                            }
                            string varName = string.Join("", inputArray.Skip(1));
                            //Declaration type is always at the begining
                            switch (inputArray[0])
                            {
                                case "string":
                                    textLine = "Dim " + varName + " as String " + iniValue;
                                    break;
                                case "bool":
                                    textLine = "Dim " + varName + " as Boolean " + iniValue;
                                    break;
                                case "int":
                                    textLine = "Dim " + varName + " as Integer " + iniValue;
                                    break;
                            }
                            textLine = textLine.Replace(" % ", "Mod");
                        }
                        sb.AppendLine(textLine);
                    }
                }
                //Write into output file. If file exists, append to it.
                using (StreamWriter sw = new StreamWriter(output, bAppend))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                //Exception handling
                Console.WriteLine("Error converting file: " + input + ". " + ex.Message +". Please confirm the source file.");
            }
        }
    }
}
