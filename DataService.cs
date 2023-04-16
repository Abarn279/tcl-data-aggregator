using DataPuller.Enums;
using DataPuller.Records;
using Camille.RiotGames;
using Camille.Enums;

namespace DataPuller
{
    public class DataService
    {
        private readonly RiotGamesApi _riotApi;
        private readonly List<Role> _positions = new List<Role> { Role.TOP, Role.JG, Role.MID, Role.ADC, Role.SUP };

        public DataService(RiotGamesApi api)
        {
            _riotApi = api;
        }

        public async Task<List<GameRecord>> GetGameRecords(List<MatchMetadata> matchMetadata)
        {
            var grs = new List<GameRecord>();

            foreach (var matchMd in matchMetadata)
            {
                // var match = await Retry<Match>(async () => await _riotApi.Match.GetMatchAsync(RiotSharp.Misc.Region.Na, matchMd.GameId), 20);
                var matchFull = await _riotApi.MatchV5().GetMatchAsync(Camille.Enums.RegionalRoute.AMERICAS, matchMd.GameId);
                var match = matchFull.Info;

                foreach (var team in match.Teams)
                {
                    var myParticipants = match.Participants.Where(x => x.TeamId == team.TeamId).ToList();
                    if (myParticipants.Count != 5) throw new System.Exception("Bad participant count");

                    grs.Add(new GameRecord(
                        gameTime: TimeSpan.FromSeconds(match.GameDuration),
                        matchId: matchMd.MatchKey,
                        week: matchMd.Week,
                        day: matchMd.Day,
                        gameNumber: matchMd.GameNumber,
                        teamName: team.TeamId == Camille.RiotGames.Enums.Team.Blue ? matchMd.BlueTeamName : matchMd.RedTeamName,
                        side: team.TeamId == Camille.RiotGames.Enums.Team.Blue ? Side.Blue : Side.Red,
                        opponent: team.TeamId == Camille.RiotGames.Enums.Team.Blue ? matchMd.RedTeamName : matchMd.BlueTeamName,
                        outcome: team.Win == true ? Outcome.Victory : Outcome.Defeat,
                        totalCS: (int)myParticipants.Sum(x => x.TotalMinionsKilled),
                        totalGold: (int)myParticipants.Sum(x => x.GoldEarned),
                        totalLevels: (int)myParticipants.Sum(x => x.ChampLevel),
                        firstBlood: myParticipants.Any(x => x.FirstBloodKill),
                        firstTower: myParticipants.Any(x => x.FirstTowerKill),
                        inhibitorsKilled: myParticipants.Sum(x => x.InhibitorKills),
                        dragonKills: myParticipants.Sum(x => x.DragonKills),
                        baronKills: myParticipants.Sum(x => x.BaronKills)
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
                var matchFull = await _riotApi.MatchV5().GetMatchAsync(Camille.Enums.RegionalRoute.AMERICAS, matchMd.GameId);
                var match = matchFull.Info;
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
                        gameTime: TimeSpan.FromSeconds(match.GameDuration),
                        week: matchMd.Week,
                        day: matchMd.Day,
                        gameNumber: matchMd.GameNumber,
                        team: myTeam.TeamId == Camille.RiotGames.Enums.Team.Blue ? matchMd.BlueTeamName : matchMd.RedTeamName,
                        player: myName,
                        role: _positions[pnum % 5],
                        champion: participant.ChampionName,
                        pickOrder: null,
                        ban: myTeam.Bans.Length >= (pnum % 5 + 1) ? ((Champion) myTeam.Bans[pnum % 5].ChampionId).ToString() : "",
                        banTarget: "",
                        laneOpponent: laneOpponentName,
                        laneOpponentChampion: ((Champion) match.Participants[laneOpponentPnum].ChampionId).ToString(),
                        outcome: myTeam.Win ? Outcome.Victory : Outcome.Defeat,
                        kill: (int)participant.Kills,
                        death: (int)participant.Deaths,
                        assist: (int)participant.Assists,
                        cS: (int)participant.TotalMinionsKilled,
                        gold: (int)participant.GoldEarned,
                        level: (int)participant.ChampLevel,
                        damageDone: (int)participant.TotalDamageDealtToChampions,
                        doubleKills: (int)participant.DoubleKills,
                        tripleKills: (int)participant.TripleKills,
                        quadraKills: (int)participant.QuadraKills,
                        pentakills: (int)participant.PentaKills,
                        totalHeal: (int)participant.TotalHeal,
                        visionScore: (int)participant.VisionScore,
                        timeCCingOthers: (int)participant.TotalTimeCCDealt,
                        turretKills: (int)participant.TurretKills,
                        wardsPlaced: (int)participant.WardsPlaced,
                        wardsKilled: (int)participant.WardsKilled,
                        firstBlood: participant.FirstBloodKill
                    ));

                    pnum++;
                }
            }

            return pgrs;
        }

        private async Task<T> Retry<T>(Func<Task<T>> func, int tries)
        {
            Exception exception = null;
            int sleep = 500;
            for (var i = 0; i < tries; i++)
            {
                try
                {
                    return await func();
                }
                catch (Exception e)
                {
                    exception = e;
                    Thread.Sleep(sleep);
                    sleep += 500;
                }
            }

            throw exception;
        }
    }
}