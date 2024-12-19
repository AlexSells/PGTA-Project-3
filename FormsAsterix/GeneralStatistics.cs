using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibAsterix;

namespace FormsAsterix
{
    public partial class GeneralStatistics : Form
    {
        List<DistanceList> StatsList;
        List<int> incidentData;
        List<int> incidentDataTwr;
        public GeneralStatistics(List<DistanceList> List, List<int> ListData, List<int> ListData_twr)
        {
            InitializeComponent();
            StatsList = List;
            incidentData = ListData;
            incidentDataTwr = ListData_twr;
        }

        public void SetGenStatsGrid()
        {

            List<string> lista = new List<string> {
            "Average","Variance", "Standard Deviation","Percentil 95", "Percentil 99", "Minimum", "Maximum"};

            GenStatisticsGird.ColumnCount = 1;
            GenStatisticsGird_twr.ColumnCount = 1;

            foreach (string headerText in lista)
            {
                int rowIndex = GenStatisticsGird.Rows.Add();
                int rowIndexTwr = GenStatisticsGird_twr.Rows.Add();
                GenStatisticsGird.Rows[rowIndex].HeaderCell.Value = headerText; // Assign the header text as the row header
                GenStatisticsGird_twr.Rows[rowIndexTwr].HeaderCell.Value= headerText;
            }

            GenStatisticsGird.RowHeadersDefaultCellStyle.Font = new Font(GenStatisticsGird.Font, FontStyle.Bold);
            GenStatisticsGird.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;

            GenStatisticsGird_twr.RowHeadersDefaultCellStyle.Font = new Font(GenStatisticsGird.Font, FontStyle.Bold);
            GenStatisticsGird_twr.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;


            // Añadimos los valores respecto a la lista introducida
            GenStatisticsGird.Rows[0].Cells[0].Value = Functions4Statistics.CalculateAverageDistanceDiff(StatsList, false);
            GenStatisticsGird.Rows[1].Cells[0].Value = Functions4Statistics.CalculateVarianceDistanceDiff(StatsList, false);
            GenStatisticsGird.Rows[2].Cells[0].Value = Functions4Statistics.CalculateStandardDeviatioDistanceDiff(StatsList, false);
            GenStatisticsGird.Rows[3].Cells[0].Value = Functions4Statistics.CalculatePercentile95DistanceDiff(StatsList, false);
            GenStatisticsGird.Rows[4].Cells[0].Value = Functions4Statistics.CalculatePercentile99DistanceDiff(StatsList, false);
            GenStatisticsGird.Rows[5].Cells[0].Value = Functions4Statistics.FindMinDistanceDiff(StatsList, false);
            GenStatisticsGird.Rows[6].Cells[0].Value = Functions4Statistics.FindMaxDistanceDiff(StatsList, false);

            // Añadimos los valores respecto a la lista introducida
            GenStatisticsGird_twr.Rows[0].Cells[0].Value = Functions4Statistics.CalculateAverageDistanceDiff(StatsList, true);
            GenStatisticsGird_twr.Rows[1].Cells[0].Value = Functions4Statistics.CalculateVarianceDistanceDiff(StatsList, true);
            GenStatisticsGird_twr.Rows[2].Cells[0].Value = Functions4Statistics.CalculateStandardDeviatioDistanceDiff(StatsList, true);
            GenStatisticsGird_twr.Rows[3].Cells[0].Value = Functions4Statistics.CalculatePercentile95DistanceDiff(StatsList, true);
            GenStatisticsGird_twr.Rows[4].Cells[0].Value = Functions4Statistics.CalculatePercentile99DistanceDiff(StatsList, true);
            GenStatisticsGird_twr.Rows[5].Cells[0].Value = Functions4Statistics.FindMinDistanceDiff(StatsList, true);
            GenStatisticsGird_twr.Rows[6].Cells[0].Value = Functions4Statistics.FindMaxDistanceDiff(StatsList, true);

            //Ajustamos el datagrid al contenido
            GenStatisticsGird.RowHeadersWidth = 210;
            GenStatisticsGird.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            GenStatisticsGird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            GenStatisticsGird.AllowUserToAddRows = false; // como evitar que salga una fila extra

            //Ajustamos el datagrid al contenido
            GenStatisticsGird_twr.RowHeadersWidth = 210;
            GenStatisticsGird_twr.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            GenStatisticsGird_twr.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            GenStatisticsGird_twr.AllowUserToAddRows = false; // como evitar que salga una fila extra
        }

        public void SetIncidentStatsGrid()
        {

            List<string> lista = new List<string> {
            "Total Planes","Incident Planes", "Total Estela Comparations","Total Radar Incidents", "Total Estela Incidents", "Total LoA Incidents"};

            // Crear columnas y filas para el DataGridView
            IncidentStatsGrid.ColumnCount = 1;
            IncidentStatsGrid_twr.ColumnCount = 1;

            foreach (string headerText in lista)
            {
                int rowIndex = IncidentStatsGrid.Rows.Add();
                IncidentStatsGrid.Rows[rowIndex].HeaderCell.Value = headerText; // Assign the header text as the row header

                int rowIndexTwr = IncidentStatsGrid_twr.Rows.Add();
                IncidentStatsGrid_twr.Rows[rowIndexTwr].HeaderCell.Value = headerText; // Assign the header text as the row header
            }

            IncidentStatsGrid.RowHeadersWidth = 210;
            IncidentStatsGrid.RowHeadersDefaultCellStyle.Font = new Font(IncidentStatsGrid.Font, FontStyle.Bold);
            IncidentStatsGrid.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;

            IncidentStatsGrid_twr.RowHeadersWidth = 210;
            IncidentStatsGrid_twr.RowHeadersDefaultCellStyle.Font = new Font(IncidentStatsGrid_twr.Font, FontStyle.Bold);
            IncidentStatsGrid_twr.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;


            // Añadimos los valores respecto a la lista introducida
            for (int i = 0; i < 6; i++)
            {
                IncidentStatsGrid.Rows[i].Cells[0].Value = incidentData[i];
                IncidentStatsGrid_twr.Rows[i].Cells[0].Value = incidentDataTwr[i];
            }

            //Ajustamos el datagrid al contenido
            IncidentStatsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            IncidentStatsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            IncidentStatsGrid.AllowUserToAddRows = false; // como evitar que salga una fila extra

            //Ajustamos el datagrid al contenido
            IncidentStatsGrid_twr.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            IncidentStatsGrid_twr.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            IncidentStatsGrid_twr.AllowUserToAddRows = false; // como evitar que salga una fila extra


        }
        private void GeneralStatistics_Load(object sender, EventArgs e)
        {
            // Hacer que el usuario pueda agregar filas manualmente
            IncidentStatsGrid.AllowUserToAddRows = true;
            IncidentStatsGrid.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            SetGenStatsGrid();
            SetIncidentStatsGrid();
        }
    }
}
