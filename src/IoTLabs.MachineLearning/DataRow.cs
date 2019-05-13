using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsAiEdgeLabTabular
{
    public class DataRow
    {
        [Index(0)]
        public float Temperature { get; set; }

        [Index(1)]
        public float Pressure { get; set; }

        [Index(2)]
        public float Humidity { get; set; }

        [Index(3)]
        public float ExternalTemperature { get; set; }


        [Index(4)]
        public float PowerOutput { get; set; }
    }
}
