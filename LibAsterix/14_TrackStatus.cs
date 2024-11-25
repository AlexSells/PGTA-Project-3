using System;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class TrackStatus : DataItem
    {


        public string TRE {  get; private set; }
        public string GHO { get; private set; }
        public string SUP {  get; private set; }
        public string TCC { get; private set; }
        public string CNF { get; private set; }
        public string RAD { get; private set; }
        public string DOU { get; private set; }
        public string MAH { get; private set; }
        public string CDM { get; private set; }
        public string FX { get; private set; }

        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public TrackStatus(string info)
            : base(info)
        {

        }


        // Implementación del método abstracto Descodificar
        public override void Descodificar()
        {
            if (base.info == "N/A")
            {
                CNF = "N/A";
                RAD = "N/A";
                DOU = "N/A";
                MAH = "N/A";
                CDM = "N/A";
                FX = "N/A";
                TRE = "N/A";
                GHO = "N/A";
                SUP = "N/A";
                TCC = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al track status");
                CNF = base.info.Substring(0, 1);
                if (CNF == "0")
                {
                    CNF = "Confirmed Track";
                }
                else
                {
                    CNF = "Tentative Track";
                }

                RAD = base.info.Substring(1, 2);
                switch (RAD)
                {
                    case "00":
                        RAD = "Combined Track";
                        break;
                    case "01":
                        RAD = "PSR Track";
                        break;
                    case "10":
                        RAD = "SSR/Mode S Track";
                        break;
                    case "11":
                        RAD = "Invalid";
                        break;
                }

                DOU = base.info.Substring(3, 1);
                if (DOU == "0")
                {
                    DOU = "Normal confidence";
                }
                else
                {
                    DOU = "Low confidence in plot to track association.";
                }

                MAH = base.info.Substring(4, 1);
                if (MAH == "0")
                {
                    MAH = "No horizontal man.sensed";
                }
                else
                {
                    MAH = "Horizontal man.sensed";
                }

                CDM = base.info.Substring(5, 2);
                switch (CDM)
                {
                    case "00":
                        CDM = "Maintaining";
                        break;
                    case "01":
                        CDM = "Climbing";
                        break;
                    case "10":
                        CDM = "Descending";
                        break;
                    case "11":
                        CDM = "Unknown";
                        break;
                }

                FX = base.info.Substring(7, 1);

                if (FX == "1")
                {
                    TRE = base.info.Substring(8, 1);
                    if (TRE == "0")
                    {
                        TRE = "Track still alive";
                    }
                    else
                    {
                        TRE = "End of track lifetime";
                    }
                    GHO = base.info.Substring(9, 1);
                    if (GHO == "0")
                    {
                        GHO = "True target track";
                    }
                    else
                    {
                        GHO = "Ghost target track";
                    }
                    SUP = base.info.Substring(10, 1);
                    if (SUP == "0")
                    {
                        SUP = "NO";
                    }
                    else
                    {
                        SUP = "YES";
                    }
                    TCC = base.info.Substring(11, 1);
                    if (TCC == "0")
                    {
                        TCC = "Tracking performed in so-called Radar Plane";
                    }
                    else
                    {
                        TCC = "Slant range correction and a suitable projection technique are used to track in a 2D";
                    }
                    string Spare = base.info.Substring(12, 4);

                }
                else
                {
                    TRE = "N/A";
                    GHO = "N/A";
                    SUP = "N/A";
                    TCC = "N/A";
                }
            }

            
        }
        public override string ObtenerAtributos()
        {
            string mensaje = CNF + ";" + RAD + ";" + DOU + ";" + MAH + ";" + CDM + ";" + TRE + ";" + GHO + ";" + SUP + ";" + TCC + ";" ;
            return mensaje;
        }
        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.CNF = CNF;
            grid.RAD = RAD;
            grid.DOU = DOU;
            grid.MAH = MAH;
            grid.CDM = CDM;
            grid.TRE = TRE;
            grid.GHO = GHO;
            grid.SUP = SUP;
            grid.TCC = TCC;

            return grid;
        }
    }
}