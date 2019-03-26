using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWorZonesCreation.Models
{
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

        #region Get WorkZone
        // "workZoneItemId": 326426,
        // "workZone": "D.F., BENITO JUÁREZ, C.P. 03010",
        // "startDate": "2018-06-01",
        // "ratio": 100,
        // "recurrence": "daily",
        // "recurEvery": 1,
        // "type": "regular"



        #endregion

    }
}
