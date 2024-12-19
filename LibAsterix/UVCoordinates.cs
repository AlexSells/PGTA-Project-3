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
        internal const double height_radar_tang = 6368942.808;

        internal const double Lat_tma_tang = 41.1157111111 * GeoUtils.DEGS2RADS;

        internal const double Lon_tma_tang = 1.6925027778 * GeoUtils.DEGS2RADS;

        internal static CoordinatesWGS84 system_center_tang_tma = new CoordinatesWGS84(Lat_tma_tang, Lon_tma_tang, height_radar_tang);
        internal const double height_twr_tang = 6368942.808;

        internal const double Lat_twr_tang = 41.295552 * GeoUtils.DEGS2RADS;

        internal const double Lon_twr_tang = 2.095078 * GeoUtils.DEGS2RADS;

        internal static CoordinatesWGS84 system_center_tang_twr = new CoordinatesWGS84(Lat_twr_tang, Lon_twr_tang, height_twr_tang);

        public static CoordinatesUVH GetUV(double latitude, double longitude, double height, bool isTWR)
        {
            CoordinatesWGS84 Plane_lat_lon = new CoordinatesWGS84(latitude, longitude, height);

            GeoUtils geoUtils = new GeoUtils();
            if (isTWR == true)
            {
                geoUtils.setCenterProjection(system_center_tang_twr);
            }
            else
            {
                geoUtils.setCenterProjection(system_center_tang_tma);
            }

            CoordinatesXYZ geocentric_coordinates = geoUtils.change_geodesic2geocentric(Plane_lat_lon);

            CoordinatesXYZ cartesian_system = geoUtils.change_geocentric2system_cartesian(geocentric_coordinates);

            CoordinatesUVH stereographic_system = geoUtils.change_system_cartesian2stereographic(cartesian_system);

            return stereographic_system;
        }
    }
}
