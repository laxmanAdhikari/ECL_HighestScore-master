using ECL_HighestScore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Numerics;

namespace ECL_HighestScore
{
    /// <summary>
    /// Helper Class for Score Id generation process
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Get Nth Highest Score.
        /// </summary>
        /// <param name="dataPath"></param>
        /// <param name="recordLimits"></param>
        /// <param name="ReturnCode"></param>
        /// <returns>Json String with Score and Id</returns>
        public static string GetNthScoreDescendingOrder(string dataPath, int recordLimit, out int ReturnCode)
        {
            ReturnCode =  (int)ProgramExitCode.Success;
            Dictionary<BigInteger, string> records = new Dictionary<BigInteger, string>();
            string[] lines = File.ReadAllLines(dataPath);
            List<HighestScore> highestScore = new List<HighestScore>();
            Records currentRecord = new Records();
            string exitIfInvalidRecord = ConfigurationManager.AppSettings["exitIfInvalidRecord"];

            foreach (string entry in lines)
            {
                if (string.IsNullOrEmpty(entry)) continue;
                // Get index to split the line to Score and record value
                int index = entry.IndexOf(':');
                string score = entry.Substring(0, index);
                string record = entry.Substring(index + 1);

                // Process until valid JSON Records
                if (!IsValidJson(record))
                {
                    // User can still able to process all valid records until found invalid record using config value exitIfInvalidRecord = 0 in appsetting.
                    if (records?.Count <= 0 || exitIfInvalidRecord == "1")
                    {
                        ReturnCode = (int)ProgramExitCode.InvalidData;
                        Console.WriteLine($"Prcocessing of Higest Score is terminated due to presence of the invalid record is {currentRecord.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"Prcocessing of Higest Score is partially completed and program terminated due to presence of the invalid record is {currentRecord.Id}");
                        ReturnCode = (int)ProgramExitCode.Success;
                        highestScore = BuildScoreIdKeyValuePair(recordLimit, records);
                    }
                    return JsonConvert.SerializeObject(highestScore);
                }
                else
                {
                    // Deserialize record. This can be scaled to get data when required.
                    currentRecord = JsonConvert.DeserializeObject<Records>(record);
                    // If record does not have Id, set exit code to 2
                    if (!string.IsNullOrEmpty(currentRecord.Id))
                    {
                        // Build dictionary of big integer and string 
                        if (BigInteger.TryParse(score, out BigInteger scoredId))
                        {
                            if (!records.ContainsKey(scoredId))
                                records.Add(scoredId, currentRecord.Id);
                        }
                    }
                    else
                    {
                        ReturnCode = (int)ProgramExitCode.InvalidData;
                        // Get Top Nth Scores in descending order.
                        highestScore = BuildScoreIdKeyValuePair(recordLimit, records);
                    }
                }

            }

            highestScore = BuildScoreIdKeyValuePair(recordLimit, records);
            Console.WriteLine($"Processing of Highest Score is completed. There were {records.Count} records out of which {recordLimit} recrods are pulled.");
            
            return JsonConvert.SerializeObject(highestScore);
        }

        /// <summary>
        /// Build  dictionary for Score for valid Record Id
        /// </summary>
        /// <param name="recordLimits"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        private static List<HighestScore> BuildScoreIdKeyValuePair(int recordLimits, Dictionary<BigInteger, string> records)
        {
            List<HighestScore> topNthScoreDescending = new List<HighestScore>();
            var sorted = records.OrderByDescending(n => n.Key).Take(recordLimits);
            foreach (KeyValuePair<BigInteger, string> kvp in sorted)
            {
                HighestScore currentScore = new HighestScore();
                currentScore.Score = kvp.Key;
                currentScore.Id = kvp.Value;
                topNthScoreDescending.Add(currentScore);
            }

            return topNthScoreDescending;
        }
        /// <summary>
        /// Check if JSON string is valid or not
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Return true if it is valid otherwise false</returns>
        private static bool IsValidJson(string data)
        {
            data = data.Trim();
            try
            {
                if (data.StartsWith("{") && data.EndsWith("}"))
                {
                    JToken.Parse(data);
                }
                else if (data.StartsWith("[") && data.EndsWith("]"))
                {
                    JArray.Parse(data);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}