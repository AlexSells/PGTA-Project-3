using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using LibAsterix;

namespace FormsAsterix
{
    public partial class DataGridView : Form
    {
        // falta fer que si es canvia de mida el form, s'amplii bé el formulari i es segueixi veient be (project requeriment que puja punts jeje)
        // falta posar relacio entre formularis, que aquest quan s'obri l'altre es tanqui
        //InformationList list; // passem una llista amb tota la info dels paquets?
        public DataGridView()
        {
            InitializeComponent();
        }

        // public void GetList(InformationList list)
        //{
        // this.list = list;
        // }

        private void DataGrid(object sender, EventArgs e)
        {
            dataGridView1.RowHeadersVisible = false;
            //dataGridView1.RowCount = list.DameNum(); 
            dataGridView1.ColumnCount = 92; // he agafat els mateixos que mostra el SW d'exemple del profe
            dataGridView1.Columns[0].HeaderText = "NUM";
            dataGridView1.Columns[1].HeaderText = "SAC";
            dataGridView1.Columns[2].HeaderText = "SIC";
            dataGridView1.Columns[3].HeaderText = "Time";
            dataGridView1.Columns[4].HeaderText = "Latitud";
            dataGridView1.Columns[5].HeaderText = "Longitud";
            dataGridView1.Columns[6].HeaderText = "h";
            dataGridView1.Columns[7].HeaderText = "TYP020";
            dataGridView1.Columns[8].HeaderText = "SIM020";
            dataGridView1.Columns[9].HeaderText = "RDP020";
            dataGridView1.Columns[10].HeaderText = "SPI020";
            dataGridView1.Columns[11].HeaderText = "RAB020";
            dataGridView1.Columns[12].HeaderText = "TST020";
            dataGridView1.Columns[13].HeaderText = "ERR020";
            dataGridView1.Columns[14].HeaderText = "XPP020";
            dataGridView1.Columns[15].HeaderText = "ME020";
            dataGridView1.Columns[16].HeaderText = "MI020";
            dataGridView1.Columns[17].HeaderText = "FOEFRI_020";
            dataGridView1.Columns[18].HeaderText = "RHO";
            dataGridView1.Columns[19].HeaderText = "THETA";
            dataGridView1.Columns[20].HeaderText = "V070";
            dataGridView1.Columns[21].HeaderText = "G070";
            dataGridView1.Columns[22].HeaderText = "Mode_3A";
            dataGridView1.Columns[23].HeaderText = "V090";
            dataGridView1.Columns[24].HeaderText = "G090";
            dataGridView1.Columns[25].HeaderText = "Flight_Level";
            dataGridView1.Columns[26].HeaderText = "ModeC_corrected";
            dataGridView1.Columns[27].HeaderText = "SRL130";
            dataGridView1.Columns[28].HeaderText = "SRR130";
            dataGridView1.Columns[29].HeaderText = "SAM130";
            dataGridView1.Columns[30].HeaderText = "PRL130";
            dataGridView1.Columns[31].HeaderText = "PAM130";
            dataGridView1.Columns[32].HeaderText = "RPD130";
            dataGridView1.Columns[33].HeaderText = "APD130";
            dataGridView1.Columns[34].HeaderText = "Target_Address";
            dataGridView1.Columns[35].HeaderText = "Target_ID";
            dataGridView1.Columns[36].HeaderText = "Mode_S";
            dataGridView1.Columns[37].HeaderText = "MCP_Status";
            dataGridView1.Columns[38].HeaderText = "MCP_ALT";
            dataGridView1.Columns[39].HeaderText = "FMS_Status";
            dataGridView1.Columns[40].HeaderText = "FMS_ALT";
            dataGridView1.Columns[41].HeaderText = "BP_Status";
            dataGridView1.Columns[42].HeaderText = "BP";
            dataGridView1.Columns[43].HeaderText = "MODE_Status";
            dataGridView1.Columns[44].HeaderText = "VNAV";
            dataGridView1.Columns[45].HeaderText = "ALT_HOLD";
            dataGridView1.Columns[46].HeaderText = "APP";
            dataGridView1.Columns[47].HeaderText = "TARGETALT_Status";
            dataGridView1.Columns[48].HeaderText = "TARGETALT_Source";
            dataGridView1.Columns[49].HeaderText = "RS_Status";
            dataGridView1.Columns[50].HeaderText = "RA";
            dataGridView1.Columns[51].HeaderText = "TTA_Status";
            dataGridView1.Columns[52].HeaderText = "TTA";
            dataGridView1.Columns[53].HeaderText = "GSS_Status";
            dataGridView1.Columns[54].HeaderText = "GS";
            dataGridView1.Columns[55].HeaderText = "TAR_Status";
            dataGridView1.Columns[56].HeaderText = "TAR";
            dataGridView1.Columns[57].HeaderText = "TAS_Status";
            dataGridView1.Columns[58].HeaderText = "TAS";
            dataGridView1.Columns[59].HeaderText = "HDG_Status";
            dataGridView1.Columns[60].HeaderText = "HDG";
            dataGridView1.Columns[61].HeaderText = "IAS_Status";
            dataGridView1.Columns[62].HeaderText = "IAS";
            dataGridView1.Columns[63].HeaderText = "MACH_Status";
            dataGridView1.Columns[64].HeaderText = "MACH";
            dataGridView1.Columns[65].HeaderText = "BAR_Status";
            dataGridView1.Columns[66].HeaderText = "BAR";
            dataGridView1.Columns[67].HeaderText = "IVV_Status";
            dataGridView1.Columns[68].HeaderText = "IVV";
            dataGridView1.Columns[69].HeaderText = "Track_number";
            dataGridView1.Columns[70].HeaderText = "X_component";
            dataGridView1.Columns[71].HeaderText = "Y_component";
            dataGridView1.Columns[72].HeaderText = "Ground_SpeedKT";
            dataGridView1.Columns[73].HeaderText = "Heading";
            dataGridView1.Columns[74].HeaderText = "CNF170";
            dataGridView1.Columns[75].HeaderText = "RAD170";
            dataGridView1.Columns[76].HeaderText = "DOU170";
            dataGridView1.Columns[77].HeaderText = "MAH170";
            dataGridView1.Columns[78].HeaderText = "CDM170";
            dataGridView1.Columns[79].HeaderText = "TRE170";
            dataGridView1.Columns[80].HeaderText = "GHO170";
            dataGridView1.Columns[81].HeaderText = "SUP170";
            dataGridView1.Columns[82].HeaderText = "TCC170";
            dataGridView1.Columns[83].HeaderText = "Measured_Height";
            dataGridView1.Columns[84].HeaderText = "COM230";
            dataGridView1.Columns[85].HeaderText = "STAT230";
            dataGridView1.Columns[86].HeaderText = "SI230";
            dataGridView1.Columns[87].HeaderText = "MSCC230";
            dataGridView1.Columns[88].HeaderText = "ARC230";
            dataGridView1.Columns[89].HeaderText = "AIC230";
            dataGridView1.Columns[90].HeaderText = "B1A230";
            dataGridView1.Columns[91].HeaderText = "B1B230";
            //
            //for (int i = 0; i < list.DameNum(); i++) //Ponemos los valores de cada plan de vuelo en su casilla correspondiente de la tabla, y para ello debemos recorrer toda la lista
            //{
            //  dataGridView1.Rows[i].Cells[0].Value = 1; // fer-ho per cada fila --> pensar com omplir

            //}
            //
        }

        /* 
            jo faria funcio exportar CSV aquí --> canviar funcio escribir fichero
                - en decodificar afegir a una llista i llavors la buidem aqui a dins --> aquest es el proces que crec que ha de ser < 1 min
                - fer botó dins del form de taula que permeti exportar CSV --> exemple profe té dues funcions: sencer o columnes reduides (podriem fer que es pogues escollir quins parametres concrets es vol --> funcionalitats extres)
                - el del profe exporta també kml --> xulissim pq es pot obrir amb el google earth
                    - si mes no, cal pensar noves coses a poder afegir :) --> que donen punts extres jeje
        */

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private ToolStrip toolStrip1;
        private ToolStripButton BtnFilter;
        private ToolStripButton bot2;
        private ToolStripButton CSV_File;
        private ToolStripButton bot5;
        private ToolStripButton Filtered_Values;
        private ToolStripButton toolStripButton2;
        private ToolStripButton No_ground_flights;
        private ToolStripButton toolStripButton3;
        private ToolStripButton blancos_puros;
    }
}