using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class Functions4Statistics
    {
        public static double CalculateVarianceDouble(List<double> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            double mean = values.Average();
            double variance = values.Average(v => Math.Pow(v - mean, 2));
            return variance;
        }
        public static double CalculateVarianceInt(List<int> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            double mean = values.Average(); // Promedio como double para mayor precisión
            double variance = values.Average(v => Math.Pow(v - mean, 2));
            return variance;
        }
        public static double CalculateStandardDeviationDouble(List<double> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            double mean = values.Average();
            double variance = values.Average(v => Math.Pow(v - mean, 2));
            return Math.Sqrt(variance); // Raíz cuadrada de la varianza
        }
        public static double CalculateStandardDeviationInt(List<int> values)
        {
            if (values == null || values.Count == 0)
                throw new ArgumentException("La lista no puede estar vacía.");

            double mean = values.Average(); // Promedio como double para mayor precisión
            double variance = values.Average(v => Math.Pow(v - mean, 2));
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
