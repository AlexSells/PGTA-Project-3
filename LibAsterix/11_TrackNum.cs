using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class TrackNum : DataItem
    {
        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public string TrackNumber { get; private set; }
        public TrackNum(string info)
            : base(info)
        {

        }

        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                TrackNumber = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al TrackNum");
                string spare = base.info.Substring(0, 4);
                string TrackNum_bin = base.info.Substring(4, 12);
                TrackNumber = Convert.ToString(Convert.ToInt32(TrackNum_bin, 2));


                //Debug.WriteLine("Hem escrit al fitxer");
            }
        }
        public override string ObtenerAtributos()
        {
            string mensaje = TrackNumber + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Track_Number = TrackNumber;

            return grid;

        }
    }
}
