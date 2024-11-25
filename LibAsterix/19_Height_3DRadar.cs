using Accord.Collections;
using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class H_3D_RADAR : DataItem
    {



        public string height {  get; private set; }
        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public H_3D_RADAR(string info)
            : base(info)
        {

        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                height = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al height radar");
                string SPARE = base.info.Substring(0, 2); //Siempre seran 0


                string message = base.info.Substring(2);
                bool isNegative = message[0] == '1';
                int height_rad;
                if (isNegative)
                {

                    height_rad = Convert.ToInt32(InvertirBits(message), 2) + 1;
                    height_rad = -height_rad; //Passem el valor a negatiu
                }
                else
                {
                    height_rad = Convert.ToInt32(message, 2); //El número està en positiu
                }

                height = Convert.ToString(height_rad);
            }
        }

        //Funció on invertim els bits per a fer el complement A2    

        public string InvertirBits(string message)
        {
            char[] bitsinvertidos = new char[message.Length];
            for (int i = 0; i<message.Length; i++)
            {
                bitsinvertidos[i] = message[i] == '0' ? '1' : '0'; //Invertim els bits
            }
            return new string (bitsinvertidos);
        }
        public override string ObtenerAtributos()
        {
            string mensaje = height + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.Height_3D = height;

            return grid;

        }
    }
}