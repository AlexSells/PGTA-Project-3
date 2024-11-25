using System;
using System.Diagnostics;
using System.Security.Authentication;

namespace LibAsterix
{
    // Clase hija que hereda de DataItem
    public class ModeS4 : DataItem
    {
        public string MCP_FCUtxt {  get; private set; }
        public string FMStxt { get; private set; }
        public string BARtxt { get; private set; }
        public string Mode_stat_txt { get; private set; }
        public string VNAVMODEtxt { get; private set; }
        public string ALTHOLDtxt { get; private set; }
        public string Approachtxt { get; private set; }
        public string StatusTargAlt { get; private set; }
        public string TargetAltSourcetxt { get; private set; }
        public string BDS4 { get; private set; }
        // Constructor que inicializa las variables utilizando el constructor de la clase base
        public ModeS4(string info)
            : base(info)
        {

        }


        public override void Descodificar()
        {
            
            if (base.info == "N/A")
            {
                MCP_FCUtxt = "N/A";
                FMStxt = "N/A";
                BARtxt = "N/A";
                VNAVMODEtxt = "N/A";
                ALTHOLDtxt = "N/A";
                Approachtxt = "N/A";
                Mode_stat_txt = "N/A";
                StatusTargAlt = "N/A";
                TargetAltSourcetxt = "N/A";
                BDS4 = "N/A";
            }
            else
            {
                //Debug.WriteLine("Estem al ModeS MB-4");
                BDS4 = "4,0";
                long MCP_FCU = Convert.ToInt64(base.info.Substring(0, 1),2);
                if (MCP_FCU == 1)
                {
                    MCP_FCU = Convert.ToInt64(base.info.Substring(1, 12), 2) * 16;
                    MCP_FCUtxt = Convert.ToString(MCP_FCU);
                }
                else
                {
                    MCP_FCUtxt = "N/A";
                }


                long FMS = Convert.ToInt64(base.info.Substring(13, 1), 2);
                if (FMS == 1)
                {
                    FMS = Convert.ToInt64(base.info.Substring(14, 12), 2) * 16;
                    FMStxt = Convert.ToString(FMS);
                }
                else
                {
                    FMStxt = "0";
                }


                long BAR = Convert.ToInt64(base.info.Substring(26, 1), 2);
                if (BAR == 1)
                {
                    BAR = Convert.ToInt64(base.info.Substring(27, 12), 2);
                    double BARdou = ((BAR)/ 10.0) + 800;
                    if (BARdou > 1209 || BARdou < 800)
                    {
                        BARtxt = "NV";
                    }
                    else
                    {
                        BARtxt = Convert.ToString(BARdou);
                    }
                }
                else
                {
                    BARtxt = "N/A";
                }

                int Mode_stat = Convert.ToInt32(base.info.Substring(49, 1), 2);
                Mode_stat_txt = Convert.ToString(Mode_stat);
                if (Mode_stat == 1)
                {
                    VNAVMODEtxt = Convert.ToString(Convert.ToInt32(base.info.Substring(48, 1)));
                    ALTHOLDtxt = Convert.ToString(Convert.ToInt32(base.info.Substring(49, 1)));
                    Approachtxt = Convert.ToString(Convert.ToInt32(base.info.Substring(50, 1)));
                }
                else
                {
                    VNAVMODEtxt = "0";
                    ALTHOLDtxt = "0";
                    Approachtxt = "0";
                }

                int targ_stat = Convert.ToInt32(base.info.Substring(49, 1), 2);
                StatusTargAlt = Convert.ToString(targ_stat);
                if (targ_stat == 1)
                {
                    TargetAltSourcetxt = Convert.ToString(Convert.ToInt32(base.info.Substring(54, 1)));
                }
                else
                {
                    TargetAltSourcetxt = "0";
                }
            }
            



        }
        public override string ObtenerAtributos()
        {
            string mensaje = BDS4 + ";" + MCP_FCUtxt + ";" + FMStxt + ";" + BARtxt + ";"+ Mode_stat_txt + ";" + VNAVMODEtxt + ";" + ALTHOLDtxt + ";" + Approachtxt + ";" + StatusTargAlt + ";" + TargetAltSourcetxt + ";";
            return mensaje;
        }

        public override AsterixGrid ObtenerAsterix()
        {
            AsterixGrid grid = new AsterixGrid();
            grid.BDS_4_0 = BDS4;
            grid.MCP_FCUtxt = MCP_FCUtxt;
            grid.FMStxt = FMStxt;
            grid.BARtxt = BARtxt;
            grid.Mode_stat_txt = Mode_stat_txt;
            grid.VNAVMODEtxt = VNAVMODEtxt;
            grid.ALTHOLDtxt = ALTHOLDtxt;
            grid.Approachtxt = Approachtxt;
            grid.StatusTargAlt = StatusTargAlt;
            grid.TargetAltSourcetxt = TargetAltSourcetxt;
            return grid;
        }
    }
}