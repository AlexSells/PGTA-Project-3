using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class ModeS6 : DataItem
    {
        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public string MagHeadtxt { get; private set; }
        public string IndAirtxt { get; private set; }
        public string MACHtxt { get; private set; }
        public string BarAlttxt { get; private set; }
        public string InerVerttxt { get; private set; }
        public string BDS6 { get; private set; }
        public ModeS6(string info)
            : base(info)
        {

        }



        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                MagHeadtxt = "N/A";
                IndAirtxt = "N/A";
                MACHtxt = "N/A";
                BarAlttxt = "N/A";
                InerVerttxt = "N/A";
                BDS6 = "N/A";
            }
            else
            {

                BDS6 = "6,0";
                double MagHead = Convert.ToInt32(base.info.Substring(0, 1), 2);
                int SIGN_MagHead = Convert.ToInt32(base.info.Substring(1, 1), 2); // SIGN 1 = West (e.g. 315 = -45°) 
                if (MagHead == 1)
                {
                    string msg = base.info.Substring(2, 10);
                    if (SIGN_MagHead == 1)
                    {
                        MagHead = (Convert.ToInt32(InvertirBits(msg), 2) +1) * ((double)90 / 512);
                        MagHead = -MagHead;
                    }

                    else
                    {
                        MagHead = Convert.ToInt32(msg, 2) * ((double)90 / 512);
                    }
                    MagHeadtxt = Convert.ToString(MagHead);
                  
                }
                else
                {
                    MagHeadtxt = "N/A";
                }
                

                int IndAir = Convert.ToInt32(base.info.Substring(12, 1), 2);
                if (IndAir == 1)
                {
                    IndAir = Convert.ToInt32(base.info.Substring(13, 10), 2) * 1;
                    IndAirtxt = Convert.ToString(IndAir);
                }
                else
                {
                    IndAirtxt = "N/A";
                }
              


                int MACH = Convert.ToInt32(base.info.Substring(23, 1), 2);
                if (MACH == 1)
                {
                    double MACHdou = Convert.ToDouble(Convert.ToInt32(base.info.Substring(24, 10),2)) * (2.048 / 512);
                    MACHtxt = Convert.ToString(MACHdou);
                }
                else
                {
                    MACHtxt = "N/A";
                }
                


                double BarAlt = Convert.ToInt32(base.info.Substring(34, 1), 2);
                int SIGN_BarAlt = Convert.ToInt32(base.info.Substring(35, 1), 2); // SIGN 1 = Below
                if (BarAlt == 1)
                {
                    string msg = base.info.Substring(36, 9);
                    if (SIGN_BarAlt == 1)
                    {
                        BarAlt = Convert.ToInt32(InvertirBits(msg), 2) * 32;
                        if (BarAlt != 0)
                        {
                            BarAlt = -BarAlt;
                        }
                        
                    }
                    else
                    {
                        BarAlt = Convert.ToInt32(msg, 2) * 32;
                    }
                    BarAlttxt = Convert.ToString(BarAlt);
                }
                else
                {
                    BarAlttxt = "N/A";
                }
                

                double InerVert = Convert.ToInt32(base.info.Substring(45, 1), 2);
                int SIGN_InerVert = Convert.ToInt32(base.info.Substring(46, 1), 2); // SIGN 1 = Below
                if (InerVert == 1)
                {
                    string msg = base.info.Substring(47, 9);
                    if (SIGN_InerVert == 1)
                    {
                        InerVert = Convert.ToInt32(InvertirBits(msg), 2) * 32;
                        if (InerVert != 0)
                        {
                            InerVert = -InerVert;
                        }

                    }
                    else
                    {
                        InerVert = Convert.ToInt32(msg, 2) * 32;
                    }
                    InerVerttxt = Convert.ToString(InerVert);
                }
                else
                {
                    InerVerttxt = "N/A";
                }
                
               
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
            string mensaje = BDS6 + ";" + MagHeadtxt + ";" + IndAirtxt + ";" + MACHtxt + ";" + BarAlttxt + ";" + InerVerttxt + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.BDS_6_0 = BDS6;
            grid.MagHeadtxt = MagHeadtxt;
            grid.IndAirtxt = IndAirtxt;
            grid.MACHtxt = MACHtxt;
            grid.BarAlttxt = BarAlttxt;
            grid.InerVerttxt = InerVerttxt;
            return grid;

        }
    }
}
