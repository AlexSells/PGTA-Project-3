using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class DistanceList
    {
        public int ID { get; set; }
        public string PlaneFront { get; set; }
        public string AircraftTypeFront { get; set; }
        public string EstelaFront { get; set; }
        public string ClassFront { get; set; }
        public string SIDFront { get; set; }
        public double time_front { get; set; }
        public string PlaneBack { get; set; }
        public string AircraftTypeBack { get; set; }
        public string EstelaBack { get; set; }
        public string ClassBack { get; set; }
        public string SIDBack { get; set; }
        public double time_back { get; set; }
        public bool sameSID { get; set; }
        public double U {  get; set; }
        public double V { get; set; }
        public double DistanceDiff_tma { get; set; }
        public double DistanceDiff_twr { get; set; }
        public double secondsDiffs { get; set; }

        // Auxiliar data
        public double init_time_front { get; set; }
        public double init_time_back { get; set; }
        public double dist_thr {  get; set; }


       
    }
}
