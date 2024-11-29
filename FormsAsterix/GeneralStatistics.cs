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
        List<(string PlaneFront, string AircraftTypeFront, string EstelaFront, string ClassFront, string SIDfront, double time_front, string PlaneAfter, string AircraftTypeBack, string EstelaAfter, string ClassAfter, string SIDback, double time_back, bool SameSID, double U, double V, double DistanceDiff, double secondsDiff)> StatsList;
        List<int> incidentData;
        public GeneralStatistics(List<(string PlaneFront, string AircraftTypeFront, string EstelaFront, string ClassFront, string SIDfront, double time_front, string PlaneAfter, string AircraftTypeBack, string EstelaAfter, string ClassAfter, string SIDback, double time_back, bool SameSID, double U, double V, double DistanceDiff, double secondsDiff)> List, List<int> ListData)
        {
            InitializeComponent();
            StatsList = List;
            incidentData = ListData;
        }

        public void SetGenStatsGrid()
        {

            List<string> lista = new List<string> {
            "Average","Variance", "Standard Deviation","Percentil 95", "Minimum", "Maximum"};

            GenStatisticsGird.ColumnCount = 1;

            foreach (string headerText in lista)
            {
                int rowIndex = GenStatisticsGird.Rows.Add();
                GenStatisticsGird.Rows[rowIndex].HeaderCell.Value = headerText; // Assign the header text as the row header
            }

            GenStatisticsGird.RowHeadersDefaultCellStyle.Font = new Font(GenStatisticsGird.Font, FontStyle.Bold);

            GenStatisticsGird.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;


            // Añadimos los valores respecto a la lista introducida
            GenStatisticsGird.Rows[0].Cells[0].Value = Functions4Statistics.CalculateAverageDistanceDiff(StatsList);
            GenStatisticsGird.Rows[1].Cells[0].Value = Functions4Statistics.CalculateVarianceDistanceDiff(StatsList);
            GenStatisticsGird.Rows[2].Cells[0].Value = Functions4Statistics.CalculateStandardDeviatioDistanceDiff(StatsList);
            GenStatisticsGird.Rows[3].Cells[0].Value = Functions4Statistics.CalculatePercentile95DistanceDiff(StatsList);
            GenStatisticsGird.Rows[4].Cells[0].Value = Functions4Statistics.FindMinDistanceDiff(StatsList);
            GenStatisticsGird.Rows[5].Cells[0].Value = Functions4Statistics.FindMaxDistanceDiff(StatsList);


            //Ajustamos el datagrid al contenido
            GenStatisticsGird.RowHeadersWidth = 210;
            GenStatisticsGird.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            GenStatisticsGird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            GenStatisticsGird.AllowUserToAddRows = false; // como evitar que salga una fila extra
        }

        public void SetIncidentStatsGrid()
        {

            List<string> lista = new List<string> {
            "Total Planes","Incident Planes", "Total Estela Comparations","Total Radar Incidents", "Total Estela Incidents", "Total LoA Incidents"};

            // Crear columnas y filas para el DataGridView
            IncidentStatsGrid.ColumnCount = 1;

            foreach (string headerText in lista)
            {
                int rowIndex = IncidentStatsGrid.Rows.Add();
                IncidentStatsGrid.Rows[rowIndex].HeaderCell.Value = headerText; // Assign the header text as the row header
            }

            IncidentStatsGrid.RowHeadersWidth = 210;

            IncidentStatsGrid.RowHeadersDefaultCellStyle.Font = new Font(IncidentStatsGrid.Font, FontStyle.Bold);

            IncidentStatsGrid.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;




            // Añadimos los valores respecto a la lista introducida
            for (int i = 0; i < GenStatisticsGird.RowCount; i++)
            {
                IncidentStatsGrid.Rows[i].Cells[0].Value = incidentData[i];
            }

            //Ajustamos el datagrid al contenido
            IncidentStatsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            IncidentStatsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            IncidentStatsGrid.AllowUserToAddRows = false; // como evitar que salga una fila extra


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
