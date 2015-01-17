using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Logic;
using Newtonsoft.Json;

namespace Interface.Models
{
    public class CalendarResponseModelEvent
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        
        [JsonProperty(PropertyName = "class")]
        public string _class { get; set; }

        public DateTime StartDate
        {
            set
            {
                start = value.StartOfDay().ToUnixTime();
            }
        }

        public DateTime EndDate
        {
            set
            {
                end = value.EndOfDay().ToUnixTime()-1;
            }
        }

        public long start { get; set; }
        public long end { get; set; }
    }
    public class CalendarResponseModel
    {
        public bool success { get; set; }
        public string error { get; set; }
        public List<CalendarResponseModelEvent> result { get; set; }
    }
}