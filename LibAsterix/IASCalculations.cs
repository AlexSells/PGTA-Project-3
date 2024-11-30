using MultiCAT6.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class IASCalculations
    {
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
            if (InitPF.Altitude== altitude)
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
    }
}
