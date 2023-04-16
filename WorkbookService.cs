using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataPuller.Records;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RiotSharp.Endpoints.MatchEndpoint;

public class WorkbookService
{
    private readonly HSSFWorkbook _wb;
    private readonly IFont _boldFont;
    private readonly ICellStyle _blueHeaderStyle;

    const string GAMESTATS_SHEET_NAME = "Game Stats";
    const string PLAYERSTATS_SHEET_NAME = "Player Stats";

    public WorkbookService()
    {
        _wb = new HSSFWorkbook();

        _boldFont = _wb.CreateFont();
        _boldFont.IsBold = true;
        _boldFont.Color = IndexedColors.White.Index;

        _blueHeaderStyle = _wb.CreateCellStyle();
        _blueHeaderStyle.SetFont(_boldFont);
        _blueHeaderStyle.FillPattern = FillPattern.SolidForeground;
        _blueHeaderStyle.FillForegroundColor = IndexedColors.RoyalBlue.Index;
    }

    public void Build(string filePath)
    {
        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            _wb.Write(fs);
        }
    }

    public void DoGameStatsSheet(IList<GameRecord> grs)
    {
        // Create sheet
        var sheet = _wb.CreateSheet(GAMESTATS_SHEET_NAME);

        // Create numbered headers
        var header1 = sheet.CreateRow(0);
        FillRow(header1, Enumerable.Range(1, 15).Select(x => new CellValue(x.ToString(), CellType.Numeric)).ToList());

        var header2 = sheet.CreateRow(1);
        FillRow(header2, new CellValue[]
            {
                new CellValue("Gametime"),
                new CellValue("Fractional Minutes"),
                new CellValue("Match ID"),
                new CellValue("Game ID"),
                new CellValue("Week"),
                new CellValue("Day"),
                new CellValue("Game #"),
                new CellValue("Team Name"),
                new CellValue("Side"),
                new CellValue("Opponent"),
                new CellValue("Outcome"),
                new CellValue("Total CS"),
                new CellValue("Total Gold"),
                new CellValue("Total Levels"),
                new CellValue("First Blood"),
                new CellValue("First Tower"),
                new CellValue("Dragon Kills"),
                new CellValue("Baron Kills"),
            }, _blueHeaderStyle);

        var rowInd = 2;
        foreach (var gr in grs)
        {
            var row = sheet.CreateRow(rowInd);

            FillRow(row, new CellValue[]
            {
                    new CellValue(gr.GameTime.ToString("mm\\:ss")),
                    new CellValue(gr.FractionalMinutes.ToString("N2"), CellType.Numeric),
                    new CellValue(gr.MatchId),
                    new CellValue(gr.GameId),
                    new CellValue(gr.Week),
                    new CellValue(gr.Day),
                    new CellValue(gr.GameNumber),
                    new CellValue(gr.TeamName),
                    new CellValue(gr.Side),
                    new CellValue(gr.Opponent),
                    new CellValue(gr.Outcome),
                    new CellValue(gr.TotalCS, CellType.Numeric),
                    new CellValue(gr.TotalGold, CellType.Numeric),
                    new CellValue(gr.TotalLevels, CellType.Numeric),
                    new CellValue(gr.FirstBlood.ToInt(), CellType.Boolean),
                    new CellValue(gr.FirstTower.ToInt(), CellType.Boolean),
                    new CellValue(gr.DragonKills, CellType.Numeric),
                    new CellValue(gr.BaronKills, CellType.Numeric),
            });

            rowInd++;
        }

        for (int i = 0; i < 24; i++)
            sheet.AutoSizeColumn(i);
    }


    public void DoPlayerStatsSheet(IList<PlayerGameRecord> pgrs)
    {
        // Create sheet
        var sheet = _wb.CreateSheet(PLAYERSTATS_SHEET_NAME);

        var header1 = sheet.CreateRow(0);
        FillRow(header1, new CellValue[]
            {
                new CellValue("MatchID"),
                new CellValue("GameID"),
                new CellValue("Game Time"),
                new CellValue("Fractional Minutes"),
                new CellValue("Week"),
                new CellValue("Day"),
                new CellValue("Game Number"),
                new CellValue("Team"),
                new CellValue("Player"),
                new CellValue("Role"),
                new CellValue("Champion"),
                new CellValue("Ban"),
                new CellValue("Lane Opponent"),
                new CellValue("Lane Opponent Champion"),
                new CellValue("Outcome"),
                new CellValue("Kill"),
                new CellValue("Death"),
                new CellValue("Assist"),
                new CellValue("KD Ratio"),
                new CellValue("CS"),
                new CellValue("Gold"),
                new CellValue("Level"),
                new CellValue("Damage Done"),
                new CellValue("Double Kills"),
                new CellValue("Triple Kills"),
                new CellValue("Quadra Kills"),
                new CellValue("Pentakills"),
                new CellValue("Total Heal"),
                new CellValue("Vision Score"),
                new CellValue("Time CCing Others"),
                new CellValue("Turret Kills"),
                new CellValue("Wards Placed"),
                new CellValue("Wards Killed"),
                new CellValue("First Blood?")
            }, _blueHeaderStyle);

        var rowInd = 1;

        foreach (var pgr in pgrs)
        {
            var row = sheet.CreateRow(rowInd);

            FillRow(row, new CellValue[]
            {
                    new CellValue(pgr.MatchID),
                    new CellValue(pgr.GameID, CellType.Numeric),
                    new CellValue(pgr.GameTime.ToString("mm\\:ss")),
                    new CellValue(pgr.FractionalMinutes.ToString("N2"), CellType.Numeric),
                    new CellValue(pgr.Week, CellType.Numeric),
                    new CellValue(pgr.Day, CellType.Numeric),
                    new CellValue(pgr.GameNumber, CellType.Numeric),
                    new CellValue(pgr.Team),
                    new CellValue(pgr.Player),
                    new CellValue(pgr.Role),
                    new CellValue(pgr.Champion),
                    new CellValue(pgr.Ban),
                    new CellValue(pgr.LaneOpponent),
                    new CellValue(pgr.LaneOpponentChampion),
                    new CellValue(pgr.Outcome),
                    new CellValue(pgr.Kill, CellType.Numeric),
                    new CellValue(pgr.Death, CellType.Numeric),
                    new CellValue(pgr.Assist, CellType.Numeric),
                    new CellValue(pgr.KDRatio.ToString("N2"), CellType.Numeric),
                    new CellValue(pgr.CS, CellType.Numeric),
                    new CellValue(pgr.Gold, CellType.Numeric),
                    new CellValue(pgr.Level, CellType.Numeric),
                    new CellValue(pgr.DamageDone, CellType.Numeric),
                    new CellValue(pgr.DoubleKills, CellType.Numeric),
                    new CellValue(pgr.TripleKills, CellType.Numeric),
                    new CellValue(pgr.QuadraKills, CellType.Numeric),
                    new CellValue(pgr.Pentakills, CellType.Numeric),
                    new CellValue(pgr.TotalHeal, CellType.Numeric),
                    new CellValue(pgr.VisionScore, CellType.Numeric),
                    new CellValue(pgr.TimeCCingOthers, CellType.Numeric),
                    new CellValue(pgr.TurretKills, CellType.Numeric),
                    new CellValue(pgr.WardsPlaced, CellType.Numeric),
                    new CellValue(pgr.WardsKilled, CellType.Numeric),
                    new CellValue(pgr.FirstBlood.ToInt(), CellType.Boolean),
            });

            rowInd++;
        }

        for (int i = 0; i < 36; i++)
            sheet.AutoSizeColumn(i);
    }

    private void FillRow(IRow row, IList<CellValue> values, ICellStyle style = null)
    {
        for (int i = 0; i < values.Count; i++)
        {
            var cell = row.CreateCell(i);
            cell.SetCellType(values[i].CellType);

            if (values[i].CellType == CellType.Formula)
                cell.SetCellFormula(values[i].Value);
            else
                cell.SetCellValue(values[i].Value);

            if (style != null)
            {
                cell.CellStyle = style;
            }
        }
    }

    public IList<MatchMetadata> GetMatchMetadata(string metadataFilePath)
    {
        XSSFWorkbook wbr;
        using (FileStream file = new FileStream(metadataFilePath, FileMode.Open, FileAccess.Read))
        {
            wbr = new XSSFWorkbook(file);
        }

        var sheet = wbr.GetSheetAt(0);
        var md = new List<MatchMetadata>();

        var rowInd = 1;
        var row = sheet.GetRow(rowInd);
        while (row.GetCell(0) != null && row.GetCell(0).CellType != CellType.Blank)
        {
            md.Add(new MatchMetadata
            {
                GameNumber = (int)row.GetCell(0).NumericCellValue,
                MatchKey = row.GetCell(1).StringCellValue,
                Week = (int)row.GetCell(2).NumericCellValue,
                Day = (int)row.GetCell(3).NumericCellValue,
                GameId = row.GetCell(4).StringCellValue,
                BlueTeamName = row.GetCell(5).StringCellValue,
                RedTeamName = row.GetCell(6).StringCellValue,
                BlueTeamPlayers = row.GetCell(7).StringCellValue,
                RedTeamPlayers = row.GetCell(8).StringCellValue
            });

            rowInd++;
            row = sheet.GetRow(rowInd);
        }

        return md;
    }
}

public static class BoolExtensions
{
    public static int ToInt(this bool b)
    {
        return b ? 1 : 0;
    }
}

public class CellValue
{
    public CellValue(object value, CellType ct = CellType.String)
    {
        Value = value?.ToString() ?? "";
        CellType = ct;
    }

    public string Value { get; set; }
    public CellType CellType { get; set; } = CellType.String;
}