using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class TrackVelocityPolar : DataItem
    {

        public string groundspeed {  get; private set; }
        public string heading {  get; private set; }


        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public TrackVelocityPolar(string info)
            : base(info)
        {

        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                groundspeed = "N/A";
                heading = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al track vel");
                int length = 16; //Cada octeto tiene 8 bits

                groundspeed = Convert.ToString(Convert.ToInt32(base.info.Substring(0, length), 2)*0.22);
                heading = Convert.ToString(Convert.ToInt32(base.info.Substring(length), 2)*((double)360/Math.Pow(2, 16)));
            }
            
        }
        public override string ObtenerAtributos()
        {
            string mensaje = groundspeed + ";" + heading + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Ground_Speed = groundspeed;
            grid.Heading = heading;
            return grid;


        }
    }
}