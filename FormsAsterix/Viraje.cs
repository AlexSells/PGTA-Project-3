using LibAsterix;
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
    public partial class Viraje : Form
    {

        List<string> turnStartPoints = new List<string>();
        List<string> sidCompilantList = new List<string>();
        List<string> statsList = new List<string>();

        public Viraje(List<string> turnStartPoints_sub, List<string> sidCompilantList_sub, List<string> statsList_sub)
        {
            InitializeComponent();

            this.turnStartPoints = turnStartPoints_sub;
            this.sidCompilantList = sidCompilantList_sub;
            this.statsList = statsList_sub;

            SetHeaders(turnStartPoints, sidCompilantList, statsList);

            

            label1.Text = $"Average Latitude: {statsList[0]:F2}\n" +
              $"Average Longitude: {statsList[1]:F2}\n" +
              $"Average Altitude: {statsList[2]:F2}\n" +
              $"Average Radial: {statsList[3]:F2}";
        }

        private void SetHeaders(List<string> turnStartPoints_DG, List<string> sidCompilantList_DG, List<string> statsList_DG)
        {
            // List of column headers to display
            List<string> lista = new List<string> {
            "Aircraft ID","Latitude", "Longitude","Altitude", "Radial","Initial Mag. Heading ","Viraje Mag. Heading","Viraje RA","Check SID"};

            // Set some design parameters
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.RowHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkTurquoise;

            dataGridView1.AllowUserToAddRows = false;

            // Definir encabezados de columnas
            dataGridView1.ColumnCount = lista.Count; // Define el número de columnas
            for (int i = 0; i < lista.Count; i++)
            {
                dataGridView1.Columns[i].HeaderText = lista[i]; // Asignar nombres a las columnas
            }

            // Mostrar la información
            int maxRows = Math.Max(turnStartPoints_DG.Count / 8, sidCompilantList_DG.Count); // Número de filas necesarias
            for (int i = 0; i < maxRows; i++)
            {
                int rowIndex = dataGridView1.Rows.Add(); // Agregar una nueva fila

                // Rellenar datos de turnStartPoints_DG (columnas 0 a 4)
                for (int j = 0; j < 8; j++)
                {
                    if (i * 8 + j < turnStartPoints_DG.Count) // Validar índice
                    {
                        if (turnStartPoints_DG[i * 8 + j] != "-999")
                        {
                            dataGridView1.Rows[rowIndex].Cells[j].Value = turnStartPoints_DG[i * 8 + j];
                        }
                        else
                        {
                            dataGridView1.Rows[rowIndex].Cells[j].Value = "N/A";
                        }
                    }
                }

                // Rellenar datos de sidCompilantList_DG (columna 5)
                if (i < sidCompilantList_DG.Count) // Validar índice
                {
                    dataGridView1.Rows[rowIndex].Cells[8].Value = sidCompilantList_DG[i];
                }
            }
        }
    }
}
