using System.IO;
using System.Text;
using System.Collections.Generic;
using DataPuller.Records;
using System.Linq;
using System.Threading.Tasks;

namespace DataPuller
{
    public class FunStatsService
    {
        public async Task WriteFunStatsFile(IList<GameRecord> gameRecords, IList<PlayerGameRecord> playerGameRecords, string outputPath, int take = 30)
        {
            var sb = new StringBuilder();

            var playerRecordsByPlayer = playerGameRecords.GroupBy(x => x.Player).ToList();
            var playerRecordsByChampion = playerGameRecords.GroupBy(x => x.Champion).ToList();
            var playerRecordsByBannedChampion = playerGameRecords.GroupBy(x => x.Ban).ToList();

            var totalGames = gameRecords.Count / 2;

            // Highest Avg KDA's
            var highestAverageKDA = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgKDA = x.Average(y => y.KDRatio)
                })
                .OrderByDescending(x => x.AvgKDA)
                .Take(take)
                .ToList();
            sb.AppendLine($"KDA Player (Highest Average Game KDA):");
            highestAverageKDA.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgKDA}"));
            sb.AppendLine();

            // // Highest TOTAL KDAs
            // var highestTotalKDA = playerRecordsByPlayer
            //     .Select(x => new
            //     {
            //         Name = x.Key,
            //         TotalKDA = ((double)x.Sum(y => y.Kill) + x.Sum(y => y.Assist)) / x.Sum(y => y.Death)
            //     })
            //     .OrderByDescending(x => x.TotalKDA)
            //     .Take(30)
            //     .ToList();
            // sb.AppendLine($"KDA Player (Highest TOTAL Game KDA):");
            // highestTotalKDA.ForEach(x => sb.AppendLine($"{x.Name}: {x.TotalKDA}"));
            // sb.AppendLine();

            // Lowest Avg KDA's
            var lowestAvgKDAs = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgKDA = x.Average(y => y.KDRatio)
                })
                .OrderBy(x => x.AvgKDA)
                .Take(take)
                .ToList();
            sb.AppendLine($"(NOT) KDA Player (Lowest Average Game KDA):");
            lowestAvgKDAs.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgKDA}"));
            sb.AppendLine();

            // Highest Avg DMG / minute
            var highestAverageDMG = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgDMG = x.Average(y => y.DamageDone / y.FractionalMinutes)
                })
                .OrderByDescending(x => x.AvgDMG)
                .Take(take)
                .ToList();
            sb.AppendLine($"Paddin' the stats (Highest Average DMG per minute):");
            highestAverageDMG.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgDMG}"));
            sb.AppendLine();

            // Highest Avg CS / Min
            var highestAverageCS = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgCS = x.Average(y => y.CS / y.FractionalMinutes)
                })
                .OrderByDescending(x => x.AvgCS)
                .Take(take)
                .ToList();
            sb.AppendLine($"Minion Slayer (Highest Average CS per minute):");
            highestAverageCS.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgCS}"));
            sb.AppendLine();

            // Highest gold/min
            var highestAvgGold = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgGoldMin = x.Average(y => y.Gold / y.FractionalMinutes)
                })
                .OrderByDescending(x => x.AvgGoldMin)
                .Take(take)
                .ToList();
            sb.AppendLine($"Money on my mind, mind on my money (Highest Average Gold per minute):");
            highestAvgGold.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgGoldMin}"));
            sb.AppendLine();

            // Highest Avg Vision Score / Min
            var highestAvgVisionScore = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgVision = x.Average(y => y.VisionScore / y.FractionalMinutes)
                })
                .OrderByDescending(x => x.AvgVision)
                .Take(take)
                .ToList();
            sb.AppendLine($"omniscient (Highest vision score per minute):");
            highestAvgVisionScore.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgVision}"));
            sb.AppendLine();

            // lowest Avg Vision Score / Min
            var lowestAvgVisionScore = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgVision = x.Average(y => y.VisionScore / y.FractionalMinutes)
                })
                .OrderBy(x => x.AvgVision)
                .Take(take)
                .ToList();
            sb.AppendLine($"Not even a ward (Lowest vision score per minute):");
            lowestAvgVisionScore.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgVision}"));
            sb.AppendLine();

            // Most CC time / min
            var highestCCTime = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    AvgCCTime = x.Average(y => y.TimeCCingOthers / y.FractionalMinutes)
                })
                .OrderByDescending(x => x.AvgCCTime)
                .Take(take)
                .ToList();
            sb.AppendLine($"Can't touch my carries (Highest average time spent CCing others per minute):");
            highestCCTime.ForEach(x => sb.AppendLine($"{x.Name}: {x.AvgCCTime}"));
            sb.AppendLine();

            // Most first bloods
            var mostFB = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    totalFb = x.Count(y => y.FirstBlood == true)
                })
                .OrderByDescending(x => x.totalFb)
                .Take(take)
                .ToList();
            sb.AppendLine($"Bloodthirsty (Most first bloods):");
            mostFB.ForEach(x => sb.AppendLine($"{x.Name}: {x.totalFb}"));
            sb.AppendLine();

            // Highest total kills
            var highestKills = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    totalKills = x.Sum(y => y.Kill)
                })
                .OrderByDescending(x => x.totalKills)
                .Take(take)
                .ToList();
            sb.AppendLine($"Champion Slayer (Most total kills):");
            highestKills.ForEach(x => sb.AppendLine($"{x.Name}: {x.totalKills}"));
            sb.AppendLine();

            // highest total deaths
            var highestDeaths = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    totalDeath = x.Sum(y => y.Death)
                })
                .OrderByDescending(x => x.totalDeath)
                .Take(take)
                .ToList();
            sb.AppendLine($"Inter (Most total deaths):");
            highestDeaths.ForEach(x => sb.AppendLine($"{x.Name}: {x.totalDeath}"));
            sb.AppendLine();

            // Lowest total deaths
            var leastDeaths = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    totalDeath = x.Sum(y => y.Death)
                })
                .OrderBy(x => x.totalDeath)
                .Take(take)
                .ToList();
            sb.AppendLine($"Least fun player (Least total deaths):");
            leastDeaths.ForEach(x => sb.AppendLine($"{x.Name}: {x.totalDeath}"));
            sb.AppendLine();

            // Most multikills
            var nostMK = playerRecordsByPlayer
                .Select(x => new
                {
                    Name = x.Key,
                    MK = x.Sum(y => y.DoubleKills + y.TripleKills + y.QuadraKills + y.Pentakills)
                })
                .OrderByDescending(x => x.MK)
                .Take(take)
                .ToList();
            sb.AppendLine($"Playmaker (Most Multikills):");
            nostMK.ForEach(x => sb.AppendLine($"{x.Name}: {x.MK}"));
            sb.AppendLine();

            // Highest WR Champion
            var highestWRChamp = playerRecordsByChampion
                .Select(x => new
                {
                    Name = x.Key,
                    Winrate = (double)x.Count(y => y.Outcome == Enums.Outcome.Victory) / x.Count() * 100,
                    TotalGames = x.Count()
                })
                .Where(x => x.TotalGames >= 5)
                .OrderByDescending(x => x.Winrate)
                .Take(take)
                .ToList();
            sb.AppendLine($"200 Years (Champion with highest winrate and at least 5 games):");
            highestWRChamp.ForEach(x => sb.AppendLine($"{x.Name}: {x.Winrate}%, {x.TotalGames} Games"));
            sb.AppendLine();

            // Lowest WR Champions
            var lowestWrChampions = playerRecordsByChampion
                .Select(x => new
                {
                    Name = x.Key,
                    Winrate = (double)x.Count(y => y.Outcome == Enums.Outcome.Victory) / x.Count() * 100,
                    TotalGames = x.Count()
                })
                .Where(x => x.TotalGames >= 5)
                .OrderBy(x => x.Winrate)
                .Take(take)
                .ToList();
            sb.AppendLine($"Dog Champ (Champion with lowest winrate and at least 5 games):");
            lowestWrChampions.ForEach(x => sb.AppendLine($"{x.Name}: {x.Winrate}%, {x.TotalGames} Games"));
            sb.AppendLine();

            // Highest picked champs
            var highestPickChampions = playerRecordsByChampion
                .Select(x => new
                {
                    Name = x.Key,
                    Pickrate = (double)x.Count() / totalGames * 100,
                    TotalGames = x.Count()
                })
                .OrderByDescending(x => x.Pickrate)
                .Take(take)
                .ToList();
            sb.AppendLine($"Comfort champs (Highest pickrate):");
            highestPickChampions.ForEach(x => sb.AppendLine($"{x.Name}: {x.Pickrate}%, {x.TotalGames} Games"));
            sb.AppendLine();

            // Highest banned champs
            var highestBanChampions = playerRecordsByBannedChampion
                .Select(x => new
                {
                    Name = x.Key,
                    Banrate = (double)x.Count() / totalGames * 100,
                    TotalGames = x.Count()
                })
                .OrderByDescending(x => x.Banrate)
                .Take(take)
                .ToList();
            sb.AppendLine($"Get out of my game (Highest banrate):");
            highestBanChampions.ForEach(x => sb.AppendLine($"{x.Name}: {x.Banrate}%, {x.TotalGames} Games"));
            sb.AppendLine();

            // Champs only picked once
            var champsPickedonce = playerRecordsByChampion
                .Where(x => x.All(y => y.Player == x.First().Player))
                .Select(x => new
                {
                    ChampName = x.First().Champion,
                    PlayerName = x.First().Player
                })
                .OrderBy(x => x.ChampName)
                .ToList();
            sb.AppendLine($"Pocket picks (Champs picked by only one player):");
            champsPickedonce.ForEach(x => sb.AppendLine($"{x.ChampName}: {x.PlayerName}"));
            sb.AppendLine();

            // Champs only picked once
            var allBlueSide = gameRecords.Where(x => x.Side == Enums.Side.Blue).ToList();
            var blueSideWinrate = allBlueSide
                .Where(x => x.Outcome == Enums.Outcome.Victory)
                .Count() / (double)allBlueSide.Count;

            sb.AppendLine($"Blue Side Winrate: {blueSideWinrate * 100}%");
            sb.AppendLine();

            await File.WriteAllTextAsync(outputPath, sb.ToString());
        }
    }
}