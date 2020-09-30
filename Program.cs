using System.IO;
using System.Linq;
using RiotSharp;
using System;
using System.Configuration;
using System.Threading.Tasks;
using DataPuller.Records;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataPuller
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Config check
            var apiKey = ConfigurationManager.AppSettings.Get("ApiKey");
            if (string.IsNullOrWhiteSpace(apiKey)) throw new Exception("API key missing from config");
            var inputFile = ConfigurationManager.AppSettings.Get("InputFile");
            if (string.IsNullOrWhiteSpace(inputFile)) throw new Exception("Input file name missing from config");
            var outputPrefix = ConfigurationManager.AppSettings.Get("OutputFilePrefix");
            if (string.IsNullOrWhiteSpace(outputPrefix)) throw new Exception("Out file prefix missing from config");
            var funStatsPath = ConfigurationManager.AppSettings.Get("FunStatsTxtFileOutputPath");
            if (string.IsNullOrWhiteSpace(funStatsPath)) throw new Exception("Fun stats path missing from config");

            // Services
            var api = RiotApi.GetDevelopmentInstance(apiKey);
            var wbService = new WorkbookService();
            var dataService = new DataService(api);

            // Get TCL records. Check data file cache first. If doesn't exist, then get them from riot api.
            TCLRecords tclRecords;
            if (File.Exists("./cache.dat"))
            {
                tclRecords = GetCachedData("./cache.dat");
            }
            else
            {
                // Get metadata from local file
                var matchMetadata = wbService.GetMatchMetadata(inputFile)
                    .OrderBy(x => x.Week)
                        .ThenBy(x => x.Day)
                            .ThenBy(x => x.GameNumber)
                    .ToList();

                // Sanitize names because bove's input data sucks
                foreach (var md in matchMetadata)
                {
                    md.BlueTeamPlayers = md.BlueTeamPlayers.Replace(" ", "").ToLower();
                    md.RedTeamPlayers = md.RedTeamPlayers.Replace(" ", "").ToLower();
                }

                // Get data records
                tclRecords = new TCLRecords
                {
                    GameRecords = await dataService.GetGameRecords(matchMetadata),
                    PlayerGameRecords = await dataService.GetPlayerGameRecords(matchMetadata)
                };

                // Cache locally as file
                SetCachedData("./cache.dat", tclRecords);
            }

            // Build excel sheet
            wbService.DoGameStatsSheet(tclRecords.GameRecords);
            wbService.DoPlayerStatsSheet(tclRecords.PlayerGameRecords);
            wbService.Build($"./Data/{outputPrefix}_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.xls");

            // Do fun stats
            var fsService = new FunStatsService();
            await fsService.WriteFunStatsFile(tclRecords.GameRecords, tclRecords.PlayerGameRecords, funStatsPath);
        }

        static void SetCachedData(string path, TCLRecords records)
        {
            var ms = File.OpenWrite(path);
            var formatter = new BinaryFormatter();

            formatter.Serialize(ms, records);

            ms.Flush();
            ms.Close();
            ms.Dispose();
        }

        static TCLRecords GetCachedData(string path)
        {
            var formatter = new BinaryFormatter();
            var fs = File.Open(path, FileMode.Open);

            object obj = formatter.Deserialize(fs);
            var rec = (TCLRecords)obj;

            fs.Flush();
            fs.Close();
            fs.Dispose();

            return rec;
        }
    }
}