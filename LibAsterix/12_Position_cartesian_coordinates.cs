using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class Position_Cartesian : DataItem
    {

        public string X { get; private set; }
        public string Y { get; private set; }


        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public Position_Cartesian(string info)
            : base(info)
        {

        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                X = "N/A";
                Y = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al Pos Cartes");
                int length = 16; //Cada octeto tiene 8 bits

                string x_coordinate = base.info.Substring(0, length);

                //Estan expressats amb el complement A2
                bool isNegative = x_coordinate[0] == '1';
                int X_num;
                int Y_num;
                if (isNegative)
                {

                    X_num = Convert.ToInt32(InvertirBits(x_coordinate), 2) + 1;
                    X_num = -X_num; //Passem el valor a negatiu
                }
                else
                {
                    X_num = Convert.ToInt32(x_coordinate, 2); //El número està en positiu
                }

                string y_coordinate = base.info.Substring(length);
                isNegative = y_coordinate[0] == '1';
                if (isNegative)
                {

                    Y_num = Convert.ToInt32(InvertirBits(y_coordinate), 2) + 1;
                    Y_num = -Y_num; //Passem el valor a negatiu
                }
                else
                {
                    Y_num = Convert.ToInt32(y_coordinate, 2); //El número està en positiu
                }
                X = Convert.ToString(X_num);
                Y = Convert.ToString(Y_num);

            }


        }
        public string InvertirBits(string message)
        {
            char[] bitsinvertidos = new char[message.Length];
            for (int i = 0; i < message.Length; i++)
            {
                bitsinvertidos[i] = message[i] == '0' ? '1' : '0'; //Invertim els bits
            }
            return new string(bitsinvertidos);
        }
        public override string ObtenerAtributos()
        {
            string mensaje = X + ";" + Y + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.X_Component = X;
            grid.Y_Component = Y;
            return grid;


        }
    }
}