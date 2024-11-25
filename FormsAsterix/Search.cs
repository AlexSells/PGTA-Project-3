using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsAsterix
{
    public partial class Search : Form
    {
        string[] lista = {
            "NUM", "SAC", "SIC", "Time", "Latitud", "Longitud", "h", "TYP020", "SIM020", "RDP020","SPI020", "RAB020", "TST020", "ERR020", "XPP020", "ME020", "MI020", "FOEFRI_020", "RHO",
            "THETA", "V070", "G070", "Mode_3A", "V090", "G090", "Flight_Level", "ModeC_corrected","SRL130", "SRR130", "SAM130", "PRL130", "PAM130", "RPD130", "APD130", "Target_Address",
            "Target_ID", "Mode_S", "MCP_Status", "MCP_ALT", "FMS_Status", "FMS_ALT", "BP_Status","BP", "MODE_Status", "VNAV", "ALT_HOLD", "APP", "TARGETALT_Status", "TARGETALT_Source",
            "RS_Status", "RA", "TTA_Status", "TTA", "GSS_Status", "GS", "TAR_Status", "TAR", "TAS_Status", "TAS", "HDG_Status", "HDG", "IAS_Status", "IAS", "MACH_Status", "MACH",
            "BAR_Status", "BAR", "IVV_Status", "IVV", "Track_number", "X_component", "Y_component", "Ground_SpeedKT", "Heading", "CNF170", "RAD170", "DOU170", "MAH170", "CDM170",
            "TRE170", "GHO170", "SUP170", "TCC170", "Measured_Height", "COM230", "STAT230", "SI230", "MSCC230", "ARC230", "AIC230", "B1A230", "B1B230"
        };
        public string cmd { get; private set; }
        public Search()
        {
            InitializeComponent();
            cmd = "null;";
        }
        /*### INIT FUNCTIONS ######################################*/
        private void CreateCombBox()
        {
            foreach (var name in lista) { SearchCombBox.Items.Add(name); }
            SearchCombBox.Items.Add("-None-");
        }
        /*### CMD FUNCTIONS #######################################*/
        private void SearchCmd()
        {
            try
            {
                if (SearchCombBox.SelectedIndex == -1) { MessageBox.Show("[Search] [SearchCmd] [Select a field]"); }
                else if (SearchCombBox.SelectedIndex == 92) { cmd = "0;"; }
                else { cmd = "1;" + SearchCombBox.SelectedIndex.ToString() + ";" + SearchTxtBox.Text + ";"; }
            }
            catch { MessageBox.Show("[Search] [SearchCmd] [Error loading the commands]"); }
        }
        /*### EVENT FUNCTION #######################################*/
        private void Search_Load(object sender, EventArgs e)
        {
            CreateCombBox();
        }
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchCmd();
            MessageBox.Show(cmd);
            if (cmd != "null;")
            {
                this.DialogResult = DialogResult.OK; // Indica que se aceptó el diálogo
                this.Close();
            }
        }
    }
}
