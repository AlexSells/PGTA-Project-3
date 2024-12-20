using OfficeOpenXml.Drawing.Slicer.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using static OfficeOpenXml.ExcelErrorValue;

namespace LibAsterix
{
    public class Functions4Statistics
    {
        /*##### DISTANCE DIFFERENCE FUNCTIONS ##############################################*/
        public static double CalculateAverageDistanceDiff(List<DistanceList> List, bool isTWR)
        {
            try
            {
                if (isTWR == true)
                {
                    if (List == null || List.Count == 0)
                        throw new ArgumentException("La lista no puede estar vacía.");
                    List<DistanceList> dl = new List<DistanceList>();
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].time_front > List[i].init_time_front)
                        {
                            dl.Add(List[i]);
                        }
                    }
                    // Calcular el promedio (media) de los valores de DistanceDiff
                    return dl.Average(item => item.DistanceDiff_twr);
                }
                else
                {
                    if (List == null || List.Count == 0)
                        throw new ArgumentException("La lista no puede estar vacía.");
                    List<DistanceList> dl = new List<DistanceList>();
                    for (int i = 0; i < List.Count; i++)
                    {
                        if (List[i].time_front > List[i].init_time_front)
                        {
                            dl.Add(List[i]);
                        }
                    }
                    // Calcular el promedio (media) de los valores de DistanceDiff
                    return dl.Average(item => item.DistanceDiff_tma);
                }
            }
            catch
            {
                return - 999;
            }
           
            
        }
        public static double CalculateVarianceDistanceDiff(List<DistanceList> List, bool isTWR)
        {
            // Calcular el promedio (media) de los valores de DistanceDiff
            double aux = CalculateAverageDistanceDiff(List,isTWR);
            if (aux == -999)
            {
                return -999;
            }
            if (isTWR == true)
            {
                double variance = List.Average(item => Math.Pow(item.DistanceDiff_twr - aux, 2));
                return variance;
            }
            else
            {
                double variance = List.Average(item => Math.Pow(item.DistanceDiff_tma - aux, 2));
                return variance;
            }  
        }
        public static double CalculateStandardDeviatioDistanceDiff(List<DistanceList> List, bool isTWR)
        {
            // Calcular la varianza
            double aux = CalculateVarianceDistanceDiff(List, isTWR);

            if (aux == -999)
            {
                return -999;
            }
            // Raíz cuadrada de la varianza
            return Math.Sqrt(aux); 
        }
        public static double CalculatePercentile95DistanceDiff(List<DistanceList> List, bool isTWR)
        {
            try
            {
                if (isTWR == true)
                {
                    if (List == null || List.Count == 0)
                        throw new ArgumentException("La lista no puede estar vacía.");

                    // Ordenar la lista por la propiedad DistanceDiff
                    List = List.OrderBy(item => item.DistanceDiff_twr).ToList();

                    // Calcular el índice del percentil 95
                    int index = (int)Math.Ceiling(0.95 * List.Count) - 1;

                    // Retornar el valor en ese índice
                    return List[index].DistanceDiff_twr;
                }
                else
                {
                    if (List == null || List.Count == 0)
                        throw new ArgumentException("La lista no puede estar vacía.");

                    // Ordenar la lista por la propiedad DistanceDiff
                    List = List.OrderBy(item => item.DistanceDiff_tma).ToList();

                    // Calcular el índice del percentil 95
                    int index = (int)Math.Ceiling(0.95 * List.Count) - 1;

                    // Retornar el valor en ese índice
                    return List[index].DistanceDiff_tma;
                }
                
            } catch { return -999;  }
        }

        public static double CalculatePercentile99DistanceDiff(List<DistanceList> List, bool isTWR)
        {
            
            try
            {
                if (isTWR == true)
                {
                    if (List == null || List.Count == 0)
                        throw new ArgumentException("La lista no puede estar vacía.");

                    // Ordenar la lista por la propiedad DistanceDiff
                    List = List.OrderBy(item => item.DistanceDiff_twr).ToList();

                    // Calcular el índice del percentil 95
                    int index = (int)Math.Ceiling(0.99 * List.Count) - 1;

                    // Retornar el valor en ese índice
                    return List[index].DistanceDiff_twr;
                }
                else
                {
                    if (List == null || List.Count == 0)
                        throw new ArgumentException("La lista no puede estar vacía.");

                    // Ordenar la lista por la propiedad DistanceDiff
                    List = List.OrderBy(item => item.DistanceDiff_tma).ToList();

                    // Calcular el índice del percentil 95
                    int index = (int)Math.Ceiling(0.99 * List.Count) - 1;

                    // Retornar el valor en ese índice
                    return List[index].DistanceDiff_tma;
                }
            } catch {return -999; }
            
            
        }

        public static double FindMinDistanceDiff(List<DistanceList> List, bool isTWR)
        {
            if (isTWR == true)
            {
                if (List == null || List.Count == 0)
                    throw new ArgumentException("La lista no puede estar vacía.");

                // Buscamos el minimo
                double aux = List.Min(item => item.DistanceDiff_twr);

                return aux;
            }
            else
            {
                if (List == null || List.Count == 0)
                    throw new ArgumentException("La lista no puede estar vacía.");

                // Buscamos el minimo
                double aux = List.Min(item => item.DistanceDiff_tma);

                return aux;
            }
            
        }
        public static double FindMaxDistanceDiff(List<DistanceList> List, bool isTWR)
        {
            if (isTWR == true)
            {
                if (List == null || List.Count == 0)
                    throw new ArgumentException("La lista no puede estar vacía.");

                // Buscamos el maximo
                double aux = List.Max(item => item.DistanceDiff_twr);

                return aux;
            }
            else
            {
                if (List == null || List.Count == 0)
                    throw new ArgumentException("La lista no puede estar vacía.");

                // Buscamos el maximo
                double aux = List.Max(item => item.DistanceDiff_tma);

                return aux;
            }
            
        }

        /*##### GENERAL LISTS ##############################################################*/
        public static double CalculateVarianceDouble(List<double> values)
        {
            double mean = CalculateMeanDouble(values);
            double variance = values.Average(v => Math.Pow(v - mean, 2));
            return variance;
        }
        public static double CalculateVarianceInt(List<int> values)
        {
            double mean = CalculateMeanInt(values); // Promedio como double para mayor precisión
            double variance = values.Average(v => Math.Pow(v - mean, 2));
            return variance;
        }
        public static double CalculateStandardDeviationDouble(List<double> values)
        {
            double variance = CalculateVarianceDouble(values);
            return Math.Sqrt(variance); // Raíz cuadrada de la varianza
        }
        public static double CalculateStandardDeviationInt(List<int> values)
        {
            double variance = CalculateVarianceInt(values);
            return Math.Sqrt(variance); // Raíz cuadrada de la varianza
        }
        public static double CalculatePercentile95Double(List<double> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            values.Sort(); // Ordenar los valores
            int index = (int)Math.Ceiling(0.95 * values.Count) - 1; // Índice para el percentil 95
            return values[index];
        }
        public static double CalculatePercentile95Int(List<int> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            values.Sort(); // Ordenar los valores
            int index = (int)Math.Ceiling(0.95 * values.Count) - 1; // Índice para el percentil 95
            return values[index];
        }
        public static double FindMinimumDouble(List<double> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            return values.Min(); // Utiliza la función Min para encontrar el menor valor
        }
        public static int FindMinimumInt(List<int> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            return values.Min(); // Utiliza la función Min para encontrar el menor valor
        }
        public static double FindMaximumDouble(List<double> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            return values.Max(); // Utiliza la función Max para encontrar el mayor valor
        }
        public static int FindMaximumInt(List<int> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            return values.Max(); // Utiliza la función Max para encontrar el mayor valor
        }
        public static double CalculateMeanDouble(List<double> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            return values.Average(); // Utiliza la función Average de LINQ
        }
        public static double CalculateMeanInt(List<int> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            return values.Average(); // Utiliza la función Average de LINQ
        }
    }
}
