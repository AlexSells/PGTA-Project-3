using MultiCAT6.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    internal class WGS84Coordinates
    {
        /// <summary>
        /// Constans of radar: 
        /// height_radar = 2.007 + 25.25;     
        /// Lat_deg = 41.300702 * GeoUtils.DEGS2RADS;
        /// Lon_deg = 2.102058 * GeoUtils.DEGS2RADS;
        /// CoordinatesWGS84 radar_center = new CoordinatesWGS84(Lat_deg, Lon_deg, height_radar); 
        /// </summary>
        internal const double height_radar = 2.007 + 25.25;

        internal const double Lat_deg = 41.300702 * GeoUtils.DEGS2RADS;

        internal const double Lon_deg = 2.102058 * GeoUtils.DEGS2RADS;

        internal CoordinatesWGS84 radar_center = new CoordinatesWGS84(Lat_deg, Lon_deg, height_radar);
        /// <summary>
        /// Get_Latitude_Longitude:
        /// Calculates the latitude and longitud 
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="theta"></param>
        /// <param name="Flight_level"></param>
        /// <returns>
        /// Plane_lat_lon = CoordinatesWGS84 </returns>
        public CoordinatesWGS84 Get_Latitude_Longitude(double rho, double theta, double Flight_level)
        {
            // Radar Cartesian X_L, Y_L, Z_L
            double rho_meters = rho * GeoUtils.NM2METERS;
            GeoUtils geoUtils = new GeoUtils();

            double Radius_Earth = geoUtils.CalculateEarthRadius(radar_center);

            double elevation = GeoUtils.CalculateElevation(radar_center, Radius_Earth, rho_meters, Flight_level * 100 * GeoUtils.FEET2METERS);

            CoordinatesXYZ plane_cartesian = GeoUtils.change_radar_spherical2radar_cartesian(new CoordinatesPolar(rho_meters, theta * GeoUtils.DEGS2RADS, elevation));

            CoordinatesXYZ geocentric_coordinates = geoUtils.change_radar_cartesian2geocentric(radar_center, plane_cartesian);

            CoordinatesWGS84 Plane_lat_lon = geoUtils.change_geocentric2geodesic(geocentric_coordinates);

            return Plane_lat_lon;
        }
    }
}
