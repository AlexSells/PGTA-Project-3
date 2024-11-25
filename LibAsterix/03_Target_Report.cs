using System;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class TargetReportDescriptor : DataItem
    {

        
        public string TYP {  get; private set; }
        public string SIM { get; private set; }
        public string RDP { get; private set; }
        public string SPI { get; private set; }
        public string RAB { get; private set; }
        public string TST { get; private set; }
        public string ERR { get; private set; }
        public string XPP { get; private set; }
        public string ME { get; private set; }
        public string MI { get; private set; }
        public string FOE { get; private set; }
        public string ADSBEP { get; private set; }
        public string ADSBVAL { get; private set; }
        public string SCNEP { get; private set; }
        public string SCNVAL { get; private set; }

        public string PAIEP { get; private set; }
        public string PAIVAL { get; private set; }


        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public TargetReportDescriptor(string info)
            : base( info)
        {
            
        }


        // Implementación del método abstracto Descodificar
        string FX;
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                TYP = "N/A";
                SIM = "N/A";
                RDP = "N/A";
                SPI = "N/A";
                RAB = "N/A";
                FX = "N/A";
                TST = "N/A";
                ERR = "N/A";
                XPP = "N/A";
                ME = "N/A";
                MI = "N/A";
                FOE = "N/A";
                ADSBEP = "N/A";
                ADSBVAL = "N/A";
                SCNEP = "N/A";
                SCNVAL = "N/A";
                PAIEP = "N/A";
                PAIVAL = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al TargetReport");
                //primero separaremos todos los valores y luego los decodificaremos
                TYP = base.info.Substring(0, 3);
                switch (TYP)
                {
                    case "000":
                        TYP = "No detection";
                        break;
                    case "001":
                        TYP = "Single PSR detection";
                        break;
                    case "010":
                        TYP = "Single SSR detection";
                        break;
                    case "011":
                        TYP = "SSR + PSR detection";
                        break;
                    case "100":
                        TYP = "Single ModeS All-Call";
                        break;
                    case "101":
                        TYP = "Single ModeS Roll-Call";
                        break;
                    case "110":
                        TYP = "ModeS All-Call + PSR";
                        break;
                    case "111":
                        TYP = "ModeS Roll-Call +PSR";
                        break;
                }

                SIM = base.info.Substring(3, 1);
                if (SIM == "0")
                {
                    SIM = "Actual target report";
                }
                else
                {
                    SIM = "Simulated target report";
                }
                RDP = base.info.Substring(4, 1);
                if (RDP == "0")
                {
                    RDP = "Report from RDP Chain 1";
                }
                else
                {
                    RDP = "Report from RDP Chain 2";
                }
                SPI = base.info.Substring(5, 1);
                if (SPI == "0")
                {
                    SPI = "Absence of SPI";
                }
                else
                {
                    SPI = "Special Position Identification";
                }
                RAB = base.info.Substring(6, 1);
                if (RAB == "0")
                {
                    RAB = "Report from aircraft transponder";
                }
                else
                {
                    RAB = "Report from field monitor (fixed transponder)";
                }
                FX = base.info.Substring(7, 1);

                //Debug.WriteLine("Hem escrit al fitxer");
                if (FX == "1")
                {
                    TST = base.info.Substring(8, 1);
                    if (TST == "0")
                    {
                        TST = "Real target report";
                    }
                    else
                    {
                        TST = "Test target report";
                    }
                    ERR = base.info.Substring(9, 1);
                    if (ERR == "0")
                    {
                        ERR = "No Extended Range";
                    }
                    else
                    {
                        ERR = "Extended Range present";
                    }
                    XPP = base.info.Substring(10, 1);
                    if (XPP == "0")
                    {
                        XPP = "No X-Pulse present";
                    }
                    else
                    {
                        XPP = "X-Pulse present";
                    }
                    ME = base.info.Substring(11, 1);
                    if (ME == "0")
                    {
                        ME = "No military emergency";
                    }
                    else
                    {
                        ME = "Military emergency";
                    }
                    MI = base.info.Substring(12, 1);
                    if (MI == "0")
                    {
                        MI = "No military identification";
                    }
                    else
                    {
                        MI = "Military identification";
                    }
                    FOE = base.info.Substring(13, 2);
                    switch (FOE)
                    {
                        case "00":
                            FOE = "No Mode 4 interrogation";
                            break;
                        case "01":
                            FOE = "Friendly target";
                            break;
                        case "10":
                            FOE = "Unknown target";
                            break;
                        case "11":
                            FOE = "No reply";
                            break;
                    }

                    FX = base.info.Substring(15, 1);




                    if (FX == "1")
                    {
                        ADSBEP = base.info.Substring(16, 1);
                        if (ADSBEP == "0")
                        {
                            ADSBEP = "ADSB not populated";
                        }
                        else
                        {
                            ADSBEP = "ADSB populated";
                        }
                        ADSBVAL = base.info.Substring(17, 1);
                        if (ADSBVAL == "0")
                        {
                            ADSBVAL = "On-Site ADS-B Information not available";
                        }
                        else
                        {
                            ADSBVAL = "On-Site ADS-B Information available";
                        }
                        SCNEP = base.info.Substring(18, 1);
                        if (SCNEP == "0")
                        {
                            SCNEP = "SCN not populated";
                        }
                        else
                        {
                            SCNEP = "SCN populated";
                        }
                        SCNVAL = base.info.Substring(19, 1);
                        if (SCNVAL == "0")
                        {
                            SCNVAL = "Surveillance Cluster Network Information not available";
                        }
                        else
                        {
                            SCNVAL = "Surveillance Cluster Network Information available";
                        }
                        PAIEP = base.info.Substring(20, 1);
                        if (PAIEP == "0")
                        {
                            PAIEP = "PAI not populated";
                        }
                        else
                        {
                            PAIEP = "PAI populated";
                        }
                        PAIVAL = base.info.Substring(21, 1);
                        if (PAIVAL == "0")
                        {
                            PAIVAL = "Passive Acquisition Interface Information not available";
                        }
                        else
                        {
                            PAIVAL = "Passive Acquisition Interface Information available";
                        }
                        string SPARE = base.info.Substring(22, 1); //Siempre será 0
                        string FX = base.info.Substring(23, 1);

                        //Debug.WriteLine("Hem escrit al fitxer");
                    }
                    else
                    {
                        ADSBEP = "N/A";
                        ADSBVAL = "N/A";
                        SCNEP = "N/A";
                        SCNVAL = "N/A";
                        PAIEP = "N/A";
                        PAIVAL = "N/A";

                    }
                }
                else
                {
                    TST = "N/A";
                    ERR = "N/A";
                    XPP = "N/A";
                    ME = "N/A";
                    MI = "N/A";
                    FOE = "N/A";
                    ADSBEP = "N/A";
                    ADSBVAL = "N/A";
                    SCNEP = "N/A";
                    SCNVAL = "N/A";
                    PAIEP = "N/A";
                    PAIVAL = "N/A";

                }
            }


            



        }
        public override string ObtenerAtributos()
        {
            string mensaje = TYP + ";" + SIM + ";" + RDP +";" + SPI + ";" + RAB + ";" + TST + ";" + ERR + ";" + XPP +";" + ME + ";" + MI + ";" + FOE + ";" + ADSBEP + ";" + ADSBVAL + ";" + SCNEP + ";" + SCNVAL + ";" + PAIEP + ";" + PAIVAL + ";";
            return mensaje;
        }

        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.TYP = TYP;
            grid.SIM = SIM;
            grid.RDP = RDP;
            grid.SPI = SPI;
            grid.RAB = RAB;
            grid.TST = TST;
            grid.ERR = ERR;
            grid.XPP = XPP;
            grid.ME = ME;
            grid.MI = MI;
            grid.FOE = FOE;
            grid.ADS_EP = ADSBEP;
            grid.ADS_VAL = ADSBVAL;
            grid.PAI_EP = PAIEP;
            grid.PAI_VAL = PAIVAL;
            grid.SCN_EP = SCNVAL;
            grid.SCN_VAL = SCNVAL;
            return grid;
        }
    }

}