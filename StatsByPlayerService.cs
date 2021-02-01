using System.IO;
using System.Text;
using System.Collections.Generic;
using DataPuller.Records;
using System.Linq;
using System.Threading.Tasks;

namespace DataPuller
{
    public class StatsByPlayerService
    {
        public async Task WriteStatsByPlayerFile(IList<PlayerGameRecord> playerGameRecords, string outputPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<h1>Stats by player</h1>");

            var playerRecordsByPlayer = playerGameRecords.GroupBy(x => x.Player).ToDictionary(x => x.Key);

            foreach (var prgroupKey in playerRecordsByPlayer.Keys)
            {
                var playerGames = playerRecordsByPlayer[prgroupKey].ToList();

                // name
                sb.AppendLine($"<h3>{prgroupKey}</h3>");

                foreach (var game in playerGames)
                {
                    sb.AppendLine($"<p>Match: W{game.Week}D{game.Day}");
                    sb.AppendLine($"<p>Champ: {game.Champion}, vs. {game.LaneOpponent}'s {game.LaneOpponentChampion}</p>");
                    sb.AppendLine($"<p>KDA: {game.Kill}/{game.Death}/{game.Assist}</p>");
                    sb.AppendLine($"<p>Outcome: {game.Outcome}</p>");
                    sb.AppendLine("<br>");
                }

                sb.AppendLine("<hr>");
            }

            await File.WriteAllTextAsync(outputPath, sb.ToString());
        }
    }
}

