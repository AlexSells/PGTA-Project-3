using Accord.Collections;
using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class CommACAS : DataItem
    {


        public string communication {  get; private set; }
        public string status { get; private set; }
        public string SI {  get; private set; }
        public string MSSC { get; private set; }
        public string ARC { get; private set; }
        public string AIC { get; private set; }
        public string B1A_mess { get; private set; }
        public string B1B_mess { get; private set; }

        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public CommACAS(string info)
            : base(info)
        {

        }



        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                communication = "N/A";
                status = "N/A";
                SI = "N/A";
                MSSC = "N/A";
                ARC = "N/A";
                AIC = "N/A";
                B1A_mess = "N/A";
                B1B_mess = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al com ACAS");
                int COM = Convert.ToInt32(base.info.Substring(0, 3), 2);
                //Debug.WriteLine("Hem agafat el COM");
                switch (COM)
                {
                    case 0:
                        communication = "No communications capability";
                        break;
                    case 1:
                        communication = "Comm. A and Comm. B capability";
                        break;
                    case 2:
                        communication = "Comm. A, Comm. B and Uplink ELM";
                        break;
                    case 3:
                        communication = "Comm. A, Comm. B, Uplink ELM and Downlink ELM";
                        break;
                    case 4:
                        communication = "Level 5 Transponder capability";
                        break;
                    case 5:
                        communication = "Not assigned";
                        break;
                    case 6:
                        communication = "Not assigned";
                        break;
                    case 7:
                        communication = "Not assigned";
                        break;

                }

                int STAT = Convert.ToInt32(base.info.Substring(3, 3), 2);
                //Debug.WriteLine("Hem agafat el STAT");
                switch (STAT)
                {
                    case 0:
                        status = "No alert, no SPI, aircraft airborne";
                        break;
                    case 1:
                        status = "No alert, no SPI, aircraft on ground";
                        break;
                    case 2:
                        status = "Alert, no SPI, aircraft airborne";
                        break;
                    case 3:
                        status = "Alert, no SPI, aircraft on ground";
                        break;
                    case 4:
                        status = "Alert, SPI, aircraft airborne or on ground";
                        break;
                    case 5:
                        status = "No alert, SPI, aircraft airborne or on ground";
                        break;
                    case 6:
                        status = "Not assigned";
                        break;
                    case 7:
                        status = "Unknown";
                        break;
                }
                SI = base.info.Substring(6, 1);
                //Debug.WriteLine("Hem agafat el SI");
                if (SI == "0")
                {
                    SI = "SI-Code Capable";
                }
                else
                {
                    SI = "II-Code Capable";
                }

                MSSC = base.info.Substring(8, 1); // El bit anterior es un spare bit
                                                  //Debug.WriteLine("Hem agafat el MSSC");
                if (MSSC == "0")
                {
                    MSSC = "No";
                }
                else
                {
                    MSSC = "Yes";
                }
                ARC = base.info.Substring(9, 1);
                //Debug.WriteLine("Hem agafat el ARC");
                if (ARC == "0")
                {
                    ARC = "100 ft resolution";
                }
                else
                {
                    ARC = "25 ft resolution";
                }
                AIC = base.info.Substring(10, 1);
                //Debug.WriteLine("Hem agafat el AIC");
                if (AIC == "0")
                {
                    AIC = "No";
                }
                else
                {
                    AIC = "Yes";
                }
                int B1A = Convert.ToInt32(base.info.Substring(11, 1), 2);
                B1A_mess = "BDS 1,0 bit 16=" + Convert.ToString(B1A);
              
                B1B_mess = "BDS 1,0 bits 37/40=" + base.info.Substring(12, 4);
                //Debug.WriteLine("Hem agafat el B1B_mess");
                // Llamada al método EscribirEnFichero de la clase base

                //Debug.WriteLine("Hem escrit al fitxer");
            }

        }
        public override string ObtenerAtributos()
        {
            string mensaje = communication + ";" + status + ";" + SI + ";" + MSSC + ";" + ARC + ";" + AIC + ";" + B1A_mess + ";" + B1B_mess + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.COM = communication;
            grid.STAT = status;
            grid.SI = SI;
            grid.MSSC = MSSC;
            grid.ARC = ARC;
            grid.AIC = AIC;
            grid.B1A = B1A_mess;
            grid.B1B = B1B_mess;

            return grid;
         


        }
    }
}
