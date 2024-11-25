using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class RadarPlotChar : DataItem
    {
        public string SRL {  get; private set; }
        public string SRR { get; private set; }
        public string SAM { get; private set; }
        public string PRL { get; private set; }
        public string PAM { get; private set; }
        public string RPD { get; private set; }
        public string APD { get; private set; }

        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public RadarPlotChar(string info)
            : base(info)
        {

        }
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                SRL = "N/A";
                SRR = "N/A";
                SAM = "N/A";
                PRL = "N/A";
                PAM = "N/A";
                RPD = "N/A";
                APD = "N/A";
            }
            else
            {
                int bits = 8;
                double LSB = 360 / Math.Pow(2, 13);
                SRL = base.info.Substring(0, 1);
                if (SRL == "0")
                {
                    SRL = "N/A";
                }
                else
                {
                    double SRL_num = Convert.ToInt32(base.info.Substring(bits, 8), 2) * LSB;
                    SRL = Convert.ToString(SRL_num);
                    bits = bits + 8;

                }

                SRR = base.info.Substring(1, 1);
                if (SRR == "0")
                {
                    SRR = "N/A";
                }
                else
                {
                    SRR = Convert.ToString(Convert.ToInt32(base.info.Substring(bits, 8), 2) * (1));
                    bits = bits + 8;

                }

                SAM = base.info.Substring(2, 1);
                if (SAM == "0")
                {
                    SAM = "N/A";

                }
                else
                {
                    string message = base.info.Substring(bits, 8);
                    bits = bits + 8;
                    bool isNegative = message[0] == '1';
                    int SAMint;
                    if (isNegative)
                    {

                        SAMint = Convert.ToInt32(InvertirBits(message), 2) + 1; // si son dBms cal sumar-ho?
                        SAMint = -SAMint; //Passem el valor a negatiu
                    }
                    else
                    {
                        SAMint = Convert.ToInt32(message, 2) + 1; // si son dBms cal sumar-ho?
                    }
                    SAM = Convert.ToString(SAMint) + "dBm";

                }

                PRL = base.info.Substring(3, 1);
                if (PRL == "0")
                {
                    PRL = "N/A";
                }
                else
                {
                    PRL = Convert.ToString(Convert.ToInt32(base.info.Substring(bits, 8), 2) * LSB);
                    bits = bits + 8;
                }

                PAM = base.info.Substring(4, 1);
                if (PAM == "0")
                {
                    PAM = "N/A";
                }
                else
                {
                    string message = base.info.Substring(bits, 8);
                    bits = bits + 8;
                    bool isNegative = message[0] == '1';
                    int PAMint;
                    if (isNegative)
                    {

                        PAMint = Convert.ToInt32(InvertirBits(message), 2); // si son dBms cal sumar-ho?
                        PAMint = -PAMint; //Passem el valor a negatiu
                    }
                    else
                    {
                        PAMint = Convert.ToInt32(message, 2); // si son dBms cal sumar-ho?
                    }
                    PAM = Convert.ToString(PAMint) + "dBm";

                }

                RPD = base.info.Substring(5, 1);
                if (RPD == "0")
                {
                    RPD = "N/A";
                }
                else
                {
                    float RPD_num = (float)Convert.ToInt32(base.info.Substring(bits, 8), 2) / 256;
                    RPD = Convert.ToString(RPD_num);
                    bits = bits + 8;
                }

                APD = base.info.Substring(6, 1);
                if (APD == "0")
                {
                    APD = "N/A";
                }
                else
                {
                    APD = Convert.ToString(Convert.ToInt32(base.info.Substring(bits, 8), 2) * (360 / Math.Pow(2, 14)));
                    bits = bits + 8;
                }

            }
            //Debug.WriteLine("Hem escrit al fitxer");
        }

        
        //Funció on invertim els bits per a fer el complement A2    
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
            string mensaje = SRL + ";" + SRR + ";" + SAM + ";" + PRL + ";" + PAM + ";" + RPD + ";" + APD + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.SRL = SRL;
            grid.SRR = SRR;
            grid.SAM = SAM;
            grid.PRL = PRL;
            grid.PAM = PAM;
            grid.RPD = RPD;
            grid.APD = APD;
            return grid;

        }
    }
}
