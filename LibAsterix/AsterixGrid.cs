using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibAsterix
{
    public class AsterixGrid
    {
        public string Num { get; set; }
        // 020
        public string SAC { get; set; }
        public string SIC { get; set; }
        // 140
        public string Time { get; set; }
        public string Latitude { get; set; }

        public string Longitude { get; set; }
        public string Height { get; set; }
        // 020
        public string TYP { get; set; }
        public string SIM { get; set; }
        public string RDP { get; set; }
        public string SPI { get; set; }
        public string RAB { get; set; }
        public string TST { get; set; }
        public string ERR { get; set; }
        public string XPP { get; set; }
        public string ME { get; set; }
        public string MI { get; set; }
        public string FOE { get; set; }
        public string ADS_EP { get; set; }
        public string ADS_VAL { get; set; }
        public string SCN_EP { get; set; }
        public string SCN_VAL { get; set; }
        public string PAI_EP { get; set; }
        public string PAI_VAL { get; set; }
        // 040
        public string Rho { get; set; }
        public string Theta { get; set; }

     
        // 070
        public string V_70 { get; set; }
        public string G_70 { get; set; }
        public string L_70 { get; set; }
        public string Mode3_A_Reply { get; set; }
        // 090
        public string V_90 { get; set; }
        public string G_90 { get; set; }
        public string Flight_Level { get; set; }
        public string Mode_C_Correction { get; set; }
        // 130
        public string SRL { get; set; }
        public string SRR { get; set; }
        public string SAM { get; set; }
        public string PRL { get; set; }
        public string PAM { get; set; }
        public string RPD { get; set; }
        public string APD { get; set; }

        // 220 
        public string Aircraft_Address { get; set; }
        // 240
        public string Aircraft_Indentification { get; set; }

        // 250
        public string BDS_4_0 { get; set; }
        public string MCP_FCUtxt { get; set; }
        public string FMStxt { get; set; }
        public string BARtxt { get; set; }
        public string VNAVMODEtxt { get; set; }
        public string ALTHOLDtxt { get; set; }
        public string Approachtxt { get; set; }
        public string Mode_stat_txt { get; set; }
        public string StatusTargAlt { get; set; }
        public string TargetAltSourcetxt { get; set; }
        public string BDS_5_0 { get; set; }
        public string Rolltxt { get; set; }
        public string TrueTracktxt { get; set; }
        public string TrackAngletxt { get; set; }
        public string TrueAirspeedtxt { get; set; }
        public string GroundSpeedtxt { get; set; }
        public string BDS_6_0 { get; set; }
        public string MagHeadtxt { get; set; }
        public string IndAirtxt { get; set; }
        public string MACHtxt { get; set; }
        public string BarAlttxt { get; set; }
        public string InerVerttxt { get; set; }

        // 161
        public string Track_Number { get; set; }

        // 042
        public string X_Component { get; set; }
        public string Y_Component { get; set; }

        // 200 
        public string Ground_Speed { get; set; }
        public string Heading { get; set; }

        // 170
        public string CNF { get; set; }
        public string RAD { get; set; }
        public string DOU { get; set; }
        public string MAH { get; set; }
        public string CDM { get; set; }
        public string TRE { get; set; }
        public string GHO { get; set; }
        public string SUP { get; set; }
        public string TCC { get; set; }

        // 110
        public string Height_3D { get; set; }

        // 230
        public string COM { get; set; }
        public string STAT { get; set; }
        public string SI { get; set; }
        public string MSSC { get; set; }
        public string ARC { get; set; }
        public string AIC { get; set; }
        public string B1A { get; set; }
        public string B1B { get; set; }

    }
}
