using System.Configuration;
using DataPuller.Records;
using DataPuller;
using System.Text.Json;
using Camille.RiotGames;



// Config check
var apiKey = ConfigurationManager.AppSettings.Get("ApiKey");
if (string.IsNullOrWhiteSpace(apiKey)) throw new Exception("API key missing from config");
var inputFile = ConfigurationManager.AppSettings.Get("InputFile");
if (string.IsNullOrWhiteSpace(inputFile)) throw new Exception("Input file name missing from config");
var outputPrefix = ConfigurationManager.AppSettings.Get("OutputFilePrefix");
if (string.IsNullOrWhiteSpace(outputPrefix)) throw new Exception("Out file prefix missing from config");
var funStatsPath = ConfigurationManager.AppSettings.Get("FunStatsTxtFileOutputPath");
if (string.IsNullOrWhiteSpace(funStatsPath)) throw new Exception("Fun stats path missing from config");
var playerStatsPath = ConfigurationManager.AppSettings.Get("PlayerStatsTxtFileOutputPath");
if (string.IsNullOrWhiteSpace(playerStatsPath)) throw new Exception("Player stats path missing from config");
if (!int.TryParse(ConfigurationManager.AppSettings.Get("NumGames"), out int numgames))
    throw new Exception("Must include number of games in config");


// Services
// var api = RiotApi.GetDevelopmentInstance(apiKey);

var api = RiotGamesApi.NewInstance(apiKey);
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
await fsService.WriteFunStatsFile(tclRecords.GameRecords, tclRecords.PlayerGameRecords, funStatsPath, 10);

var pService = new StatsByPlayerService();
await pService.WriteStatsByPlayerFile(tclRecords.PlayerGameRecords, playerStatsPath);

static void SetCachedData(string path, TCLRecords records)
{
    File.WriteAllText(path, JsonSerializer.Serialize(records));
}

static TCLRecords GetCachedData(string path)
{
    return JsonSerializer.Deserialize<TCLRecords>(File.ReadAllText(path));
}