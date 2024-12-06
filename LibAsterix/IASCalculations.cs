﻿using Accord.Math;
using MultiCAT6.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using LibAsterix;

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
        public static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double lat1rad = lat1*GeoUtils.DEGS2RADS;
            double lat2rad = lat2* GeoUtils.DEGS2RADS;
            double lon1rad = lon1 * GeoUtils.DEGS2RADS;
            double lon2rad = lon2 * GeoUtils.DEGS2RADS;
            double dLat = lat2rad - lat1rad;
            double dLon = lon2rad - lon1rad;

            // Formula Harversine
            double a = (Math.Pow(Math.Sin(dLat / 2), 2)) +  (Math.Cos(lat1rad)) * (Math.Cos(lat2rad)) * (Math.Pow(Math.Sin(dLon / 2),2));
            Debug.WriteLine("a: " + a);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            Debug.WriteLine("c: " + c);

            return RADIUS_EARTH_METERS * c; // resultat en metres
        }

        internal const double height_radar_tang = 3438.954;

        internal const double Lat_deg_tang = 41.065656 * GeoUtils.DEGS2RADS;

        internal const double Lon_deg_tang = 1.413301 * GeoUtils.DEGS2RADS;

        internal CoordinatesWGS84 system_center_tang = new CoordinatesWGS84(Lat_deg_tang, Lon_deg_tang, height_radar_tang);

        public CoordinatesUVH GetUV(double latitude, double longitude, double height)
        {
            CoordinatesWGS84 Plane_lat_lon = new CoordinatesWGS84(latitude, longitude, height);

            GeoUtils geoUtils = new GeoUtils(); 

            geoUtils.setCenterProjection(system_center_tang);

            CoordinatesXYZ geocentric_coordinates = geoUtils.change_geodesic2geocentric(Plane_lat_lon);

            CoordinatesXYZ cartesian_system = geoUtils.change_geocentric2system_cartesian(geocentric_coordinates);

            CoordinatesUVH stereographic_system = geoUtils.change_system_cartesian2stereographic(cartesian_system);

            return stereographic_system;
        }

        public double DistanciaHoritzontal(double longitud_plane, double latitud_plane, double longitud_DEP, double latitud_DEP)
        {
            double lat1 = 0, long1 = 0;
            double lat2 = 0, long2 = 0;

            CoordinatesUVH coord1 = null, coord2 = null;

            lat1 = latitud_plane * GeoUtils.DEGS2RADS;
            long1 = longitud_plane * GeoUtils.DEGS2RADS;

            lat2 = latitud_DEP * GeoUtils.DEGS2RADS;
            long2 = longitud_DEP * GeoUtils.DEGS2RADS;

            coord1 = GetUV(lat1, long2, 0.0);
            coord2 = GetUV(lat2, long2, 0.0);

            // Calculate the Euclidean distance between the two aircraft's UV coordinates (U and V)
            double distancia = Math.Round(Math.Sqrt(Math.Pow(coord2.U - coord1.U, 2) + Math.Pow(coord2.V - coord1.V, 2)), 3);

           return distancia;
        }


        // Método para calcular el momento en que el avión cruza el umbral ThresORDder indica si estamos calculador thresold (true) or der (false)
        public List<IASData> CalculateThresholdCrossings( List<PlaneFilter> planes, string runway, double distanceThreshold, bool ThresORder)
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
            double thresholdLat = 0.0;
            double thresholdLon = 0.0;
            double DERLat = 0.0;
            double DERLon = 0.0;
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
            if (thresholdLat != 0.0 && thresholdLon != 0.0 && DERLat != 0.0 && DERLon != 0.0) 
            {
                int i = 0;
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

                                double distUV = DistanciaHoritzontal(plane.Lon, plane.Lat, thresholdLon, thresholdLat); 
                                if (distance > distUV)
                                {
                                    distance = distUV;
                                }

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
                                double distUV = DistanciaHoritzontal(plane.Lon, plane.Lat, thresholdLon, thresholdLat);
                                if (distanceDER > distUV)
                                {
                                    distanceDER = distUV;
                                }
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
                    i++;
                }
            }
            return thresholdCrossings;
        }

        public class DataSonometro
        {
            public string AircraftID { get; set; }
            public double time_sec { get; set; }
            public double distance { get; set; }
            public double altura { get; set; }
        }

        public List<DataSonometro> GetSonometroDist(List<PlaneFilter> list)
        {
            List<DataSonometro> distances = new List<DataSonometro>();
            list = list.OrderBy(item => item.ID).ToList();
            double Lat_Son = 41.2719444444;
            double Lon_Son = 2.04777777778;

            for (int i = 0; i < list.Count; i++)
            {
                if (distances.Count == 0)
                {
                    double dist = IASCalculations.HaversineDistance(list[i].Lat, list[i].Lon, Lat_Son, Lon_Son);
                    distances.Add(new DataSonometro { AircraftID = list[i].AircraftID, time_sec = list[i].time_sec, distance = dist, altura = list[i].Altitude });
                }
                else if (list[i].AircraftID != distances[distances.Count - 1].AircraftID)
                {
                    double dist = IASCalculations.HaversineDistance(list[i].Lat, list[i].Lon, Lat_Son, Lon_Son);
                    distances.Add(new DataSonometro { AircraftID = list[i].AircraftID, time_sec = list[i].time_sec, distance = dist, altura = list[i].Altitude });
                }
                else if (list[i].AircraftID == distances[distances.Count - 1].AircraftID)
                {
                    double dist = IASCalculations.HaversineDistance(list[i].Lat, list[i].Lon, Lat_Son, Lon_Son);
                    if (dist < distances[distances.Count - 1].distance)
                    {
                        distances[distances.Count - 1].distance = dist;
                        distances[distances.Count - 1].altura = list[i].Altitude;
                        distances[distances.Count - 1].time_sec = list[i].time_sec;
                    }
                }
            }

            return distances;
        }
    }
}
