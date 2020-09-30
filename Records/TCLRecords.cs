using System;
using System.Collections.Generic;

namespace DataPuller.Records
{
    [Serializable]
    public class TCLRecords
    {
        public List<GameRecord> GameRecords { get; set; }
        public List<PlayerGameRecord> PlayerGameRecords { get; set; }
    }
}