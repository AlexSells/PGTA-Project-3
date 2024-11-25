using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class AircraftAdd : DataItem
    {


        public string add {  get; private set; }

        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public AircraftAdd(string info)
            : base(info)
        {

        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                add = "N/A";
            }
            else
            {
                string address = string.Empty;
                for (int i = 0; i < base.info.Length; i += 4)
                {
                    string bits = base.info.Substring(i, 4); //Agafem grups de 4 per a passar-ho a hexadecimal
                    int decval = Convert.ToInt32(bits, 2); //Ho passem a decimal
                    string address_char = decval.ToString("X");
                    add += address_char;
                }
            }
                


        }
        public override string ObtenerAtributos()
        {
            string mensaje = add + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Aircraft_Address = add;
            return grid;

        }
    }
}
