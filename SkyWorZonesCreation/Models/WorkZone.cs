using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWorZonesCreation.Models
{

    public class RootWorkZone
    {
        public List<WorkZone> items { get; set; }
    }
    public class WorkZone
    {
        public List<string> id { get; set; }
        public string source { get; set; }
        #region Create WorkZone
        public string workZoneName { get; set; }
        public string workZoneLabel { get; set; }
        public string travelArea { get; set; }
        public string status { get; set; } = "active";
        public List<string> keylabel { get; set; } = new List<string>();
        public List<string> keys { get; set; } = new List<string>();

        #endregion
        /*
         * {"items": [
	{
  "workZoneLabel": "NI9350556",
  "status": "active",
  "travelArea": "NI",
  "workZoneName": "RAAS, EL AYOTE, C.P. 9350556",
  "keys": [
    "NI9350556"
  ]
}
	]
	}
         */

    }
}
