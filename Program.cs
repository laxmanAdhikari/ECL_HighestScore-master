using ECL_HighestScore.Models;
using System;
using System.IO;

namespace ECL_HighestScore
{
    class Program
    {
        /// <summary>
        /// Process HighestScore.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            try
            {
                int exitCode = (int)ProgramExitCode.Success;

                // Avoid Argumentnull exception
                if (args.Length >= 2)
                {
                    string DataFilePath = args[0];
                    // Avoid File Not Found exception
                    if (!File.Exists(args[0]))
                    {
                        Console.WriteLine("Data File does not exits. Please enter valid data file path");
                        return (int)ProgramExitCode.FileNotFound;
                    }

                    // Avoid invalid Number of record value.
                    bool isSuccess = int.TryParse(args[1], out int num);
                    if (!isSuccess)
                    {
                        Console.WriteLine("Please enter valid number of records to output");
                        return (int)ProgramExitCode.InvalidInput;
                    }

                    // Retrieve json string for score and id from the given data file path and record limit number.
                    var jsonResult = Helper.GetNthScoreDescendingOrder(DataFilePath, num, out exitCode);
                    if (exitCode == (int)ProgramExitCode.Success)
                    {
                        if (!string.IsNullOrEmpty(jsonResult))
                        {
                            Console.Write(jsonResult);
                            Console.ReadLine();
                            return exitCode;
                        }
                    }
                    else
                    {
                        Console.Write($"No score to show due to invalid record exists in a data file.");
                        return exitCode;
                    }
                    return (int)ProgramExitCode.Fail;
                }
                Console.WriteLine("Please enter input data file and the number of records to output");
                return (int)ProgramExitCode.InvalidInput;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} error occured while processing the file to get the number of records in descending order");
                return (int)ProgramExitCode.InvalidInput;
            }
        }

    }
}
