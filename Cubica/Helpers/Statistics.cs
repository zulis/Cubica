using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;

namespace Cubica
{
    //[Serializable]
    //public class Statistics
    //{
    //    public Statistics()
    //    {
    //        DateList = new List<DateTime>();
    //        SceneIdList = new List<int>();
    //        MoveList = new List<int>();
    //        TimeList = new List<string>();
    //    }

    //    public List<DateTime> DateList;
    //    public List<int> SceneIdList;
    //    public List<int> MoveList;
    //    public List<string> TimeList;
    //}

    [Serializable]
    public class Statistics
    {
        public Statistics()
        {
            RecordList = new List<Record>();
        }

        public List<Record> RecordList;
    }

    public class Record
    {
        public Record()
        {
            Date = new DateTime();
            SceneId = 0;
            Move = 0;
            Time = string.Empty;
        }

        public DateTime Date;
        public int SceneId;
        public int Move;
        public string Time;
    }
}
