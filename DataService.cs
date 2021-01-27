using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataPuller.Enums;
using DataPuller.Records;
using RiotSharp;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Endpoints.StaticDataEndpoint.Champion;

namespace DataPuller
{
    public class DataService
    {
        private readonly RiotApi _riotApi;
        private readonly List<Role> _positions = new List<Role> { Role.TOP, Role.JG, Role.MID, Role.ADC, Role.SUP };
        private readonly ChampionListStatic _champs;

        public DataService(RiotApi api)
        {
            _riotApi = api;
            _champs = _riotApi.StaticData.Champions.GetAllAsync("11.1.1").Result;
        }

        public async Task<List<GameRecord>> GetGameRecords(List<MatchMetadata> matchMetadata)
        {
            var grs = new List<GameRecord>();

            foreach (var matchMd in matchMetadata)
            {
                var match = await Retry<Match>(async () => await _riotApi.Match.GetMatchAsync(RiotSharp.Misc.Region.Na, matchMd.GameId), 20);

                foreach (var team in match.Teams)
                {
                    var myParticipants = match.Participants.Where(x => x.TeamId == team.TeamId).ToList();
                    if (myParticipants.Count != 5) throw new System.Exception("Bad participant count");

                    grs.Add(new GameRecord(
                        matchDate: match.GameCreation,
                        gameTime: match.GameDuration,
                        matchId: matchMd.MatchKey,
                        gameId: match.GameId,
                        week: matchMd.Week,
                        day: matchMd.Day,
                        gameNumber: matchMd.GameNumber,
                        teamName: team.TeamId == 100 ? matchMd.BlueTeamName : matchMd.RedTeamName,
                        side: team.TeamId == 100 ? Side.Blue : Side.Red,
                        opponent: team.TeamId == 100 ? matchMd.RedTeamName : matchMd.BlueTeamName,
                        outcome: team.Win == "Win" ? Outcome.Victory : Outcome.Defeat,
                        totalCS: (int)myParticipants.Sum(x => x.Stats.TotalMinionsKilled),
                        totalGold: (int)myParticipants.Sum(x => x.Stats.GoldEarned),
                        totalLevels: (int)myParticipants.Sum(x => x.Stats.ChampLevel),
                        firstBlood: team.FirstBlood,
                        firstTower: team.FirstTower,
                        firstInhibitor: team.FirstInhibitor,
                        firstRiftHerald: team.FirstRiftHerald,
                        firstBaron: team.FirstBaron,
                        dragonKills: team.DragonKills,
                        baronKills: team.BaronKills,
                        riftHeraldKills: team.RiftHeraldKills
                    ));
                }
            }

            return grs;
        }

        public async Task<List<PlayerGameRecord>> GetPlayerGameRecords(List<MatchMetadata> matchMetadata)
        {
            var pgrs = new List<PlayerGameRecord>();

            foreach (var matchMd in matchMetadata)
            {
                var match = await Retry<Match>(async () => await _riotApi.Match.GetMatchAsync(RiotSharp.Misc.Region.Na, matchMd.GameId), 20);
                var pnum = 0; // The participant index 1-10
                foreach (var participant in match.Participants)
                {
                    var myTeam = match.Teams.Single(x => x.TeamId == participant.TeamId);
                    var myName = pnum <= 4 ? matchMd.BlueTeamPlayers.Split(',')[pnum] : matchMd.RedTeamPlayers.Split(',')[pnum % 5];
                    var laneOpponentName = pnum <= 4 ? matchMd.RedTeamPlayers.Split(',')[pnum] : matchMd.BlueTeamPlayers.Split(',')[pnum % 5];
                    var laneOpponentPnum = (pnum + 5) % 10;

                    pgrs.Add(new PlayerGameRecord(
                        matchID: matchMd.MatchKey,
                        gameID: match.GameId,
                        gameTime: match.GameDuration,
                        week: matchMd.Week,
                        day: matchMd.Day,
                        gameNumber: matchMd.GameNumber,
                        team: myTeam.TeamId == 100 ? matchMd.BlueTeamName : matchMd.RedTeamName,
                        player: myName,
                        role: _positions[pnum % 5],
                        champion: _champs.Keys[participant.ChampionId],
                        pickOrder: null,
                        ban: _champs.Keys[myTeam.Bans[pnum % 5].ChampionId],
                        banTarget: "",
                        laneOpponent: laneOpponentName,
                        laneOpponentChampion: _champs.Keys[match.Participants[laneOpponentPnum].ChampionId],
                        outcome: myTeam.Win == "Win" ? Outcome.Victory : Outcome.Defeat,
                        kill: (int)participant.Stats.Kills,
                        death: (int)participant.Stats.Deaths,
                        assist: (int)participant.Stats.Assists,
                        cS: (int)participant.Stats.TotalMinionsKilled,
                        gold: (int)participant.Stats.GoldEarned,
                        level: (int)participant.Stats.ChampLevel,
                        damageDone: (int)participant.Stats.TotalDamageDealtToChampions,
                        doubleKills: (int)participant.Stats.DoubleKills,
                        tripleKills: (int)participant.Stats.TripleKills,
                        quadraKills: (int)participant.Stats.QuadraKills,
                        pentakills: (int)participant.Stats.PentaKills,
                        totalHeal: (int)participant.Stats.TotalHeal,
                        visionScore: (int)participant.Stats.VisionScore,
                        timeCCingOthers: (int)participant.Stats.TotalTimeCrowdControlDealt,
                        turretKills: (int)participant.Stats.TowerKills,
                        wardsPlaced: (int)participant.Stats.WardsPlaced,
                        wardsKilled: (int)participant.Stats.WardsKilled,
                        firstBlood: participant.Stats.FirstBloodKill
                    ));

                    pnum++;
                }
            }

            return pgrs;
        }

        private async Task<T> Retry<T>(Func<Task<T>> func, int tries)
        {
            Exception exception = null;
            for (var i = 0; i < tries; i++)
            {
                try
                {
                    return await func();
                }
                catch (Exception e)
                {
                    exception = e;
                    Thread.Sleep(500);
                }
            }

            throw exception;
        }
    }
}