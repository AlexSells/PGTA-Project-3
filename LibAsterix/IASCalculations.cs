using MultiCAT6.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class IASCalculations
    {

        /*### IAS CALCULATIONS ONLY ###############################################################################################################*/
        // Método para calcular la altitud corregida
        public static double CalculateAltitude(double currentAltitude, double ias, double tta, int N)
        {
            // Fórmula: Alt(t+N) = Alt(t) + (IVV(t) * N / 60) 
            // N és el segundo que estamos interpolando
            double ivv = 101.269 * ias * Math.Sin(tta); ; // pasamos de kt a ft/min
            return currentAltitude + (ivv * N / 60);
        }
        // Función para interpolar IAS en función del tiempo
        public static double CalculateIAS(double InitIAS, double FinalIAS, double DiffTime)
        {
            // Aplicar la fórmula de interpolación lineal
            return InitIAS + (DiffTime) / (4) * (InitIAS - FinalIAS);
        }
        public static double CalculeX(double IAS, double heading, double X)
        {
            double ias_x = Math.Sin(heading) * 0.514444 * IAS;
            X = X + ias_x;
            return X;
        }
        public static double CalculeY(double IAS, double heading, double Y)
        {
            double ias_y = Math.Cos(heading) * 0.514444 * IAS;
            Y = Y + ias_y;
            return Y;
        }
        // Método para verificar si el cruce de los waypoints es válido
        public static bool CheckWaypointAltitude(double altitude, string runway)
        {
            // Condiciones según la pista:
            // Retorna true --> se cumple la condicion
            // Retorna false --> no se cumple la c
            if (runway == "24L" && altitude >= 3000 * GeoUtils.METERS2FEET) return true;
            if (runway == "06R" && altitude >= 4000 * GeoUtils.METERS2FEET) return true;
            return false;
        }
        // Método para calcular la velocidad IAS y validar si es válida para el despegue
        public static bool CheckIASValid(double ias)
        {
            // Velocidad máxima IAS permitida para el cruze de DEP es 205 kt
            return ias <= 205;
        }
        public static PlaneFilter CheckIAS4Altitude(PlaneFilter InitPF, PlaneFilter FinalPF, double altitude)
        {
            PlaneFilter aux = new PlaneFilter();
            if (InitPF.Altitude == altitude)
            {
                return InitPF;
            }
            else
            {
                aux = InitPF;
            }
            for (int i = 1; i < 4; i++)
            {
                double newAlt = CalculateAltitude(InitPF.Altitude, InitPF.IndicatedAirspeed, InitPF.TrueTrackAngle, i);
                if (newAlt == altitude)
                {
                    aux.Altitude = newAlt;
                    aux.IndicatedAirspeed = CalculateIAS(InitPF.IndicatedAirspeed, FinalPF.IndicatedAirspeed, i);
                    aux.time_sec = InitPF.time_sec + i;
                    return aux;
                }
                else
                {
                    if (Math.Abs(altitude - newAlt) < Math.Abs(altitude - aux.Altitude))
                    {
                        aux.Altitude = newAlt;
                        aux.IndicatedAirspeed = CalculateIAS(InitPF.IndicatedAirspeed, FinalPF.IndicatedAirspeed, i);
                        aux.time_sec = InitPF.time_sec + i;
                    }
                }
            }
            return aux;
        }

        /*### WAYPOINTS CALCULATION ###############################################################################################################*/
        
        // Radio de la Tierra en metros (promedio)
        public const double RADIUS_EARTH_METERS = 6371000;

        // Función para calcular la distancia entre dos puntos usando la fórmula de Haversine
        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = (lat2 - lat1)*GeoUtils.DEGS2RADS;
            double dLon = (lon2 - lon1) * GeoUtils.DEGS2RADS;

            // Formula Harversine
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +  Math.Cos(lat1*GeoUtils.DEGS2RADS) * Math.Cos(lat2 * GeoUtils.DEGS2RADS) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return RADIUS_EARTH_METERS * c; // resultat en metres
        }
    
        // Método para calcular el momento en que el avión cruza el umbral ThresORDder indica si estamos calculador thresold (true) or der (false)
        public static List<IASData> CalculateThresholdCrossings( List<PlaneFilter> planes, string runway, double distanceThreshold, bool ThresORder)
        {
            planes = planes.OrderBy(item => item.AircraftID).ToList();
            List<IASData> thresholdCrossings = new List<IASData>();

            //True if the thresholds have been crossed to reduce the number of operations
            bool thresholdFound = false;
            bool DERFound = false;

            // Auxiliary string to reduce amount of operations
            string aux = null;
            string aux2 = planes[0].AircraftID;

            // INIT POSITION REFERENCES
            double thresholdLat = 0;
            double thresholdLon = 0;
            double DERLat = 0;
            double DERLon = 0;
            if (runway == "LEBL-24L") 
            {
                thresholdLat = 41.2922194444;
                thresholdLon = 2.1032805556;
                DERLat = 41.2823111111;
                DERLon = 2.07435;
            }
            else if (runway == "LEBL-06R") { 
                thresholdLat = 41.2823111111;
                thresholdLon = 2.07435;
                DERLat = 41.2922194444;
                DERLon = 2.1032805556;
            }
            if (thresholdLat != 0 && thresholdLon != 0 && DERLat != 0 && DERLon != 0) 
            {
                foreach (var plane in planes)
                {
                    if (plane.TakeoffRWY == runway)
                    {
                        if (aux2 != plane.AircraftID)
                        {
                            aux2 = plane.AircraftID;
                            thresholdFound = false;
                            DERFound = false;
                        }

                        if (aux != plane.AircraftID)
                        {
                            if (thresholdFound == false && ThresORder == true)
                            {
                                // Calcular la distancia entre el avión y el umbral
                                double distance = HaversineDistance(plane.Lat, plane.Lon, thresholdLat, thresholdLon);

                                // Si la distancia es menor al umbral (por ejemplo, 100 metros), consideramos que cruzó el umbral
                                if (distance <= distanceThreshold)
                                {
                                    // Crear un nuevo objeto IASData para guardar los datos del cruce
                                    IASData data = new IASData
                                    {
                                        AircraftId = plane.AircraftID,
                                        Time = plane.time_sec,
                                        Altitude = plane.Altitude, // Asegúrate de que la altitud esté corregida por el QNH si es necesario
                                        IAS = plane.IndicatedAirspeed
                                    };

                                    // Añadir el cruce a la lista de datos
                                    thresholdCrossings.Add(data);
                                    thresholdFound = true;
                                }
                            }
                            if (DERFound == false && ThresORder == false)
                            {
                                // Calcular la distancia entre el avión y el umbral
                                double distanceDER = HaversineDistance(plane.Lat, plane.Lon, DERLat, DERLon);

                                // Si la distancia es menor al umbral (por ejemplo, 100 metros), consideramos que cruzó el umbral
                                if (distanceDER <= distanceThreshold)
                                {
                                    // Crear un nuevo objeto IASData para guardar los datos del cruce
                                    IASData data = new IASData
                                    {
                                        AircraftId = plane.AircraftID,
                                        Time = plane.time_sec,
                                        Altitude = plane.Altitude, // Asegúrate de que la altitud esté corregida por el QNH si es necesario
                                        IAS = plane.IndicatedAirspeed
                                    };

                                    // Añadir el cruce a la lista de datos
                                    thresholdCrossings.Add(data);
                                    DERFound = true;
                                }
                            }
                            if (thresholdFound == true && DERFound == true)
                            {
                                aux = plane.AircraftID;
                                thresholdFound = false;
                                DERFound = false;
                            }
                        }
                    }
                    
                }
            }
            return thresholdCrossings;
        }
    }
}
