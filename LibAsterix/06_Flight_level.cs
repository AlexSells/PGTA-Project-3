using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class FlightLevel : DataItem
    {

        public string V {  get; private set; }
        public string G { get; private set; }
        public string FL { get; private set; }
        public string Alt_correct { get; set; }


        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public FlightLevel(string info)
            : base(info)
        {

        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {

            if (base.info == "N/A")
            {
                V = "N/A";
                G = "N/A";
                FL = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al Flight Level");
                V = base.info.Substring(0, 1);
                if (V == "0")
                {
                    V = "Code validated";
                }
                else
                {
                    V = "Code not validated";
                }
                G = base.info.Substring(1, 1);
                if (G == "0")
                {
                    G = "Default";
                }
                else
                {
                    G = "Garbled code";
                }
                string message = base.info.Substring(2);
                bool isNegative = message[0] == '1';
                double FL_num;
                int Numero;
                if (isNegative)
                {
                    Numero = Convert.ToInt32(InvertirBits(message),2) +1;
                    FL_num = Convert.ToDouble(Numero);
                    FL_num = -FL_num / 4;
                }
                else
                {
                    Numero = Convert.ToInt32(message,2);
                    FL_num = Convert.ToDouble(Numero) / 4;
                }
                FL = Convert.ToString(FL_num);
            }
            //Debug.WriteLine("Hem escrit al fitxer");
            
        }
        public override string ObtenerAtributos()
        {
            string mensaje = V + ";" + G + ";"+ FL + ";" + Alt_correct + ";";
            return mensaje;
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

        public override void CorrectedAltitude(double p)
        {

            double FL_number = Convert.ToDouble(FL);
            if (FL_number < 0.0)
            {
                double Alt = FL_number * 100 + (p - 1013.2) * 30;
                Alt_correct = Convert.ToString(Math.Round(Alt,2));
            }
            else
            {
                Alt_correct = " ";
            }
       
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.V_90 = V;
            grid.G_90 = G;
            grid.Flight_Level = FL;
            grid.Mode_C_Correction = Alt_correct;
            return grid;

        }
    }
}
