using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class Position_Polar : DataItem
    {

       
        public string theta {get;private set;}
        public string rho { get;private set;}

        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public Position_Polar(string info)
            : base( info)
        {
           

        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                rho = "N/A";
                theta = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al Position Polar");
                int length = 16; //Cada octeto tiene 8 bits

                string rho_bin = base.info.Substring(0, length);
                string theta_bin = base.info.Substring(length);
                // Convertir rho y theta de binario a decimal
                double Rho = (Convert.ToInt32(rho_bin, 2)) * ((double)1 / 256);
                double Theta = Convert.ToInt32(theta_bin, 2) * (360 / Math.Pow(2, 16));

                rho = Convert.ToString(Rho);
                theta = Convert.ToString(Theta);
                
            }
        }
        public override string ObtenerAtributos()
        {
            string mensaje = rho + ";" + theta +";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Rho = rho;
            grid.Theta = theta;
            return grid;

        }
    }
}