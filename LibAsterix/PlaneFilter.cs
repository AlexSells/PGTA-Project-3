using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class PlaneFilter
    {
        /*### DOUBLES & INTEGRERS #####################*/
        public string num { get; set; }
        
        public double time_sec { get; set; }
        public string AircraftID { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Altitude { get; set; }
        public double Heading { get; set; }
        public double RollAngle { get; set; }
        public double U_tma { get; set; }
        public double V_tma { get; set; }
        public double U_twr { get; set; }
        public double V_twr { get; set; }
        public int ID { get; set; }

        /*### STRING ##################################*/
        public string AircraftAddress { get; set; }
        public string TrackNum { get; set; }
        public string BDS50 { get; set; }
        public double TrueTrackAngle { get; set; }
        public string GroundSpeed { get; set; }
        public double TrackAngleRate { get; set; }
        public string TrueAirSpeed { get; set; }
        public string BDS60 { get; set; }
        public double MagneticHeading { get; set; }
        public double IndicatedAirspeed { get; set; }
        public string Mach { get; set; }
        public string BarometricAltitudeRate { get; set; }
        public string InertialVerticalVelocity { get; set; }
        public string EstelaType { get; set; }
        public string TakeoffProcess { get; set; }
        public string TakeoffRWY { get; set; }
        public string AircraftType { get; set; }
        public double init_takeoff { get; set; }
    }
}
