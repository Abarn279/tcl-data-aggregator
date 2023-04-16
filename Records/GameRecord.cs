using System;
using DataPuller.Enums;

namespace DataPuller.Records
{
    [Serializable]
    public class GameRecord
    {
        public GameRecord(TimeSpan gameTime, string matchId, int week, int day, int gameNumber, string teamName, Side side, string opponent, Outcome outcome, int totalCS, int totalGold, int totalLevels, bool firstBlood, bool firstTower, int inhibitorsKilled, int dragonKills, int baronKills)
        {
            GameTime = gameTime;
            MatchId = matchId;
            Week = week;
            Day = day;
            GameNumber = gameNumber;
            TeamName = teamName;
            Side = side;
            Opponent = opponent;
            Outcome = outcome;
            TotalCS = totalCS;
            TotalGold = totalGold;
            TotalLevels = totalLevels;
            FirstBlood = firstBlood;
            FirstTower = firstTower;
            InhibitorsKilled = inhibitorsKilled;
            DragonKills = dragonKills;
            BaronKills = baronKills;
        }

        public TimeSpan GameTime { get; set; }
        public double FractionalMinutes => GameTime.TotalMinutes;
        public string MatchId { get; set; }
        public long GameId { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public int GameNumber { get; set; }
        public string TeamName { get; set; }
        public Side Side { get; set; }
        public string Opponent { get; set; }
        public Outcome Outcome { get; set; }
        public int TotalCS { get; set; }
        public int TotalGold { get; set; }
        public int TotalLevels { get; set; }
        public bool FirstBlood { get; set; }
        public bool FirstTower { get; set; }
        public int InhibitorsKilled { get; set; }
        public int DragonKills { get; set; }
        public int BaronKills { get; set; }
    }
}