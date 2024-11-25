using System;
using System.Diagnostics;
using System.Text;


namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class DataSourceIdentifier : DataItem
    {
        public string SIC {  get; private set; }
        public string SAC { get; private set; }
       
       
        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public DataSourceIdentifier(string info)
            : base(info)
        {
            
        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {

            if (base.info == "N/A")
            {
                SIC = "N/A";
                SAC = "N/A";
            }
            else
            {
                int length = 8; //Cada octeto tiene 8 bits

                string SAC_bit = base.info.Substring(0, length);

                string SIC_bit = base.info.Substring(length);

                // Convertir SAC y SIC de binario a decimal

                SAC = Convert.ToString(Convert.ToInt32(SAC_bit, 2));
                SIC = Convert.ToString(Convert.ToInt32(SIC_bit, 2));
             
            }


        }
        public override string ObtenerAtributos()
        {
            string mensaje = SAC + ";" + SIC + ";";
            return mensaje;
        }

        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.SAC=SAC;
            grid.SIC=SIC;
            return grid;
        }
    }
}
