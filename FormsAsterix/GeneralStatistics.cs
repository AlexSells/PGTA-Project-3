﻿using System;
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
            // Crear columnas y filas para el DataGridView
            GenStatisticsGird.ColumnCount = 2;
            GenStatisticsGird.Rows.Add("Averge");
            GenStatisticsGird.Rows.Add("Variance");
            GenStatisticsGird.Rows.Add("Standard deviation");
            GenStatisticsGird.Rows.Add("Percentil 95");
            GenStatisticsGird.Rows.Add("Minimum");
            GenStatisticsGird.Rows.Add("Maximum");
            // Añadimos los valores respecto a la lista introducida
            GenStatisticsGird.Rows[0].Cells[1].Value = Functions4Statistics.CalculateAverageDistanceDiff(StatsList);
            GenStatisticsGird.Rows[1].Cells[1].Value = Functions4Statistics.CalculateVarianceDistanceDiff(StatsList);
            GenStatisticsGird.Rows[2].Cells[1].Value = Functions4Statistics.CalculateStandardDeviatioDistanceDiff(StatsList);
            GenStatisticsGird.Rows[3].Cells[1].Value = Functions4Statistics.CalculatePercentile95DistanceDiff(StatsList);
            GenStatisticsGird.Rows[4].Cells[1].Value = Functions4Statistics.FindMinDistanceDiff(StatsList);
            GenStatisticsGird.Rows[5].Cells[1].Value = Functions4Statistics.FindMaxDistanceDiff(StatsList);
            //Ajustamos el datagrid al contenido
            GenStatisticsGird.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            GenStatisticsGird.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            GenStatisticsGird.AllowUserToAddRows = false; // como evitar que salga una fila extra

            for (int i = 0; i < GenStatisticsGird.RowCount; i++)
            {
                GenStatisticsGird.Rows[i].Cells[0].Style.Font = new Font(GenStatisticsGird.Font, FontStyle.Bold); // diseño de las filas 
                GenStatisticsGird.Rows[i].Cells[0].Style.BackColor = Color.LightCyan;
            }
        }

        public void SetIncidentStatsGrid()
        {
            // Crear columnas y filas para el DataGridView
            IncidentStatsGrid.ColumnCount = 2;
            IncidentStatsGrid.Rows.Add("Total Planes");
            IncidentStatsGrid.Rows.Add("Incident Planes");
            IncidentStatsGrid.Rows.Add("Total Estela Comparations");
            IncidentStatsGrid.Rows.Add("Total Radar Incidents");
            IncidentStatsGrid.Rows.Add("Total Estela Incidents");
            IncidentStatsGrid.Rows.Add("Total LoA Incidents");
            // Añadimos los valores respecto a la lista introducida
            for (int i = 0; i < GenStatisticsGird.RowCount; i++)
            {
                IncidentStatsGrid.Rows[i].Cells[1].Value = incidentData[i];
            }
            //Ajustamos el datagrid al contenido
            IncidentStatsGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            IncidentStatsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Otros parametros de diseño
            IncidentStatsGrid.AllowUserToAddRows = false; // como evitar que salga una fila extra

            for (int i = 0; i < IncidentStatsGrid.RowCount; i++)
            {
                IncidentStatsGrid.Rows[i].Cells[0].Style.Font = new Font(IncidentStatsGrid.Font, FontStyle.Bold); // diseño de las filas 
                IncidentStatsGrid.Rows[i].Cells[0].Style.BackColor = Color.LightCyan;
            }
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
