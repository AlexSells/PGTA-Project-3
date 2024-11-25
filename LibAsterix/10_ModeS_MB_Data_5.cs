using Microsoft.VisualBasic;
using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class ModeS5 : DataItem
    {
        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public string Rolltxt {  get; private set; }
        public string TrueTracktxt { get; private set; }
        public string TrackAngletxt { get; private set; }
        public string GroundSpeedtxt { get; private set; }
        public string TrueAirspeedtxt {  get; private set; }
        public string BDS5 { get; private set; }
        public ModeS5(string info)
            : base(info)
        {

        }



        public override void Descodificar()
        {
            
            if (base.info == "N/A")
            {
                Rolltxt = "N/A";
                TrueTracktxt = "N/A";
                TrackAngletxt = "N/A";
                TrueAirspeedtxt = "N/A";
                GroundSpeedtxt = "N/A";
                BDS5 = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al ModeS MB-5");
                BDS5 = "5,0";
                double Roll = Convert.ToInt64(base.info.Substring(0, 1), 2);
                long SIGN_Roll = Convert.ToInt64(base.info.Substring(1, 1), 2); // SIGN 1 = Left Wing Down
                if (Roll == 1)
                {
                    string msg = base.info.Substring(2, 9);
                    if (SIGN_Roll == 1)
                    {
                        Roll = Convert.ToInt32(InvertirBits(msg), 2) * ((double)45 / 256);
                        if (Roll != 0)
                        {
                            Roll = -Roll;
                        }
                        
                    }
                    else
                    {
                        Roll = Convert.ToInt64(msg, 2) * ((double)45 / 256);
                    }
                    Rolltxt = Convert.ToString(Roll);
                    
                }
                else
                {
                    Rolltxt = "N/A";
                }


                double TrueTrack = Convert.ToInt64(base.info.Substring(11, 1), 2);
                long SIGN_TrueTrack = Convert.ToInt64(base.info.Substring(12, 1), 2); // SIGN 1 = West (e.g. 315 = -45°) 
                if (TrueTrack == 1)
                {
                    string msg = base.info.Substring(13, 10);
                    if (SIGN_TrueTrack == 1)
                    {
                        TrueTrack = Convert.ToInt64(InvertirBits(msg), 2) * ((double)90 / 512);
                        if (TrueTrack != 0)
                        {
                            TrueTrack = -TrueTrack;
                        }

                    }
                    else
                    {
                        TrueTrack = Convert.ToInt64(msg, 2) * ((double)90 / 512);
                    }

                    TrueTracktxt = Convert.ToString(TrueTrack);
                    
                }
                else
                {
                    TrueTracktxt = "N/A";
                }


                double GroundSpeed = Convert.ToInt64(base.info.Substring(23, 1), 2);
                if (GroundSpeed == 1)
                {
                    GroundSpeed = Convert.ToInt64(base.info.Substring(24, 10), 2) * ((double)1024 / 512);
                    GroundSpeedtxt = Convert.ToString(GroundSpeed);
                    
                }
                else
                {
                    GroundSpeedtxt = "N/A";
                }


                double TrackAngle = Convert.ToInt64(base.info.Substring(34, 1), 2);
                long SIGN_TrackAngle = Convert.ToInt64(base.info.Substring(35, 1), 2); // SIGN 1 = Minus
                if (TrackAngle == 1)
                {
                    string msg = base.info.Substring(36, 9);
                    if (SIGN_TrackAngle == 1)
                    {
                        TrackAngle = Convert.ToInt64(InvertirBits(msg), 2) * ((double)6 / 256);
                        if (TrackAngle !=0)
                        {
                            TrackAngle = -TrackAngle;
                        }

                    }
                    else
                    {
                        TrackAngle = Convert.ToInt64(msg, 2) * ((double)6 / 256);
                    }
                    TrackAngletxt = Convert.ToString(TrackAngle);
                    
                }
                else
                {
                    TrackAngletxt = "N/A";
                }


                long TrueAirspeed = Convert.ToInt64(base.info.Substring(45, 1), 2);
                if (TrueAirspeed == 1)
                {
                    TrueAirspeed = Convert.ToInt64(base.info.Substring(46, 10), 2) * (2);
                    TrueAirspeedtxt = Convert.ToString(TrueAirspeed);
                   
                }
                else
                {
                    TrueAirspeedtxt = "N/A";
                }
            }
            



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
            string mensaje = BDS5 + ";" + Rolltxt + ";" + TrueTracktxt + ";" + GroundSpeedtxt + ";" + TrackAngletxt + ";" + TrueAirspeedtxt + ";";
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.BDS_5_0 = BDS5;
            grid.Rolltxt = Rolltxt;
            grid.TrueTracktxt = TrueTracktxt;
            grid.GroundSpeedtxt = GroundSpeedtxt;
            grid.TrackAngletxt = TrackAngletxt;
            grid.TrueAirspeedtxt = TrueAirspeedtxt;
            return grid;

        }
    }
}