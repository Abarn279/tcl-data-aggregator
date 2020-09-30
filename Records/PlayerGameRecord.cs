using System.Runtime.CompilerServices;
using System;
using DataPuller.Enums;

namespace DataPuller.Records
{
    [Serializable]
    public class PlayerGameRecord
    {
        public PlayerGameRecord(string matchID, long gameID, TimeSpan gameTime, int week, int day, int gameNumber, string team, string player, Role role, string champion, int? pickOrder, string ban, string banTarget, string laneOpponent, string laneOpponentChampion, Outcome outcome, int kill, int death, int assist, int cS, int gold, int level, int damageDone, int doubleKills, int tripleKills, int quadraKills, int pentakills, int totalHeal, int visionScore, int timeCCingOthers, int turretKills, int wardsPlaced, int wardsKilled, bool firstBlood)
        {
            MatchID = matchID;
            GameID = gameID;
            GameTime = gameTime;
            Week = week;
            Day = day;
            GameNumber = gameNumber;
            Team = team;
            Player = player;
            Role = role;
            Champion = champion;
            PickOrder = pickOrder;
            Ban = ban;
            BanTarget = banTarget;
            LaneOpponent = laneOpponent;
            LaneOpponentChampion = laneOpponentChampion;
            Outcome = outcome;
            Kill = kill;
            Death = death;
            Assist = assist;
            CS = cS;
            Gold = gold;
            Level = level;
            DamageDone = damageDone;
            DoubleKills = doubleKills;
            TripleKills = tripleKills;
            QuadraKills = quadraKills;
            Pentakills = pentakills;
            TotalHeal = totalHeal;
            VisionScore = visionScore;
            TimeCCingOthers = timeCCingOthers;
            TurretKills = turretKills;
            WardsPlaced = wardsPlaced;
            WardsKilled = wardsKilled;
            FirstBlood = firstBlood;
        }

        public string MatchID { get; set; }
        public long GameID { get; set; }
        public TimeSpan GameTime { get; set; }
        public double FractionalMinutes => GameTime.TotalMinutes;
        public int Week { get; set; }
        public int Day { get; set; }
        public int GameNumber { get; set; }
        public string Team { get; set; }
        public string Player { get; set; }
        public Role Role { get; set; }
        public string Champion { get; set; }
        public int? PickOrder { get; set; }
        public string Ban { get; set; }
        public string BanTarget { get; set; }
        public string LaneOpponent { get; set; }
        public string LaneOpponentChampion { get; set; }
        public Outcome Outcome { get; set; }
        public int Kill { get; set; }
        public int Death { get; set; }
        public int Assist { get; set; }
        public double KDRatio => ((double)this.Kill + this.Assist) / Math.Max((double) this.Death, 1);
        public int CS { get; set; }
        public int Gold { get; set; }
        public int Level { get; set; }
        public int DamageDone { get; set; }
        public int DoubleKills { get; set; }
        public int TripleKills { get; set; }
        public int QuadraKills { get; set; }
        public int Pentakills { get; set; }
        public int TotalHeal { get; set; }
        public int VisionScore { get; set; }
        public int TimeCCingOthers { get; set; }
        public int TurretKills { get; set; }
        public int WardsPlaced { get; set; }
        public int WardsKilled { get; set; }
        public bool FirstBlood { get; set; }
        public double CSPerMinute => this.CS / this.FractionalMinutes;
    }
}