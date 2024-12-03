using MultiCAT6.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class UVCoordinates
    {
        internal const double height_radar_tang = 3438.954;

        internal const double Lat_deg_tang = 41.065656 * GeoUtils.DEGS2RADS;

        internal const double Lon_deg_tang = 1.413301 * GeoUtils.DEGS2RADS;

        internal static CoordinatesWGS84 system_center_tang = new CoordinatesWGS84(Lat_deg_tang, Lon_deg_tang, height_radar_tang);

        public static CoordinatesUVH GetUV(double latitude, double longitude, double height)
        {
            CoordinatesWGS84 Plane_lat_lon = new CoordinatesWGS84(latitude, longitude, height);

            GeoUtils geoUtils = new GeoUtils();

            geoUtils.setCenterProjection(system_center_tang);

            CoordinatesXYZ geocentric_coordinates = geoUtils.change_geodesic2geocentric(Plane_lat_lon);

            CoordinatesXYZ cartesian_system = geoUtils.change_geocentric2system_cartesian(geocentric_coordinates);

            CoordinatesUVH stereographic_system = geoUtils.change_system_cartesian2stereographic(cartesian_system);

            return stereographic_system;
        }
    }
}
