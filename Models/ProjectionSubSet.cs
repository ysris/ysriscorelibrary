using System;

namespace YsrisCoreLibrary.Models
{
    public class ProjectionSubSet
    {
        public string title { get; set; }

        public int? sourcecount { get; set; }
        public int? destinationcount { get; set; }

        public int? sourcenew { get; set; }
        public int? sourceprocessed { get; set; }
        public int? sourcefailed { get; set; }

        public Action Process { get; set; }
    }
}
