using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class Mode3A : DataItem
    {
        public string V {  get; private set; }
        public string G { get; private set; }
        public string L { get; private set; }
        public string message { get; private set; }



        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public Mode3A(string info)
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
                L = "N/A";
                message = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al Mode3A");
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
                L = base.info.Substring(2, 1);
                if (L == "0")
                {
                    L = "Mode-3/A code derived from the reply of the transponder";
                }
                else
                {
                    L = "Mode-3/A code not extracted during the last scan";
                }
                string SPARE = base.info.Substring(3, 1); //Spare bit que siempre será 0
                int message_bit = Convert.ToInt32(base.info.Substring(4), 2);
                message = Convert.ToString(message_bit, 8).PadLeft(4, '0');
                //Debug.WriteLine("Tenim el missatge");
            }
           

        }
        public override string ObtenerAtributos()
        {
            string mensaje = V + ";" + G + ";" + L + ";" + message + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.V_70 = V;
            grid.G_70 = G;
            grid.L_70 = L;
            grid.Mode3_A_Reply = message;
            return grid;

        }
    }
}