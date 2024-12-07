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
using LibAsterix;
using MultiCAT6.Utils;
using OfficeOpenXml.Drawing.Slicer.Style;

namespace FormsAsterix
{
    public partial class IASInformation : Form
    {
        List<IASData> IASList;
        List<IASData> ListThresholdRight;
        List<IASData> ListThresholdLeft;
        List<IASData> ListDERRight;
        List<IASData> ListDERLeft;
        public IASInformation(List<IASData> IASListFromP3, List<IASData> ThresholListRight, List<IASData> ThresholListLeft, List<IASData> DERListRight, List<IASData> DERListLeft, List<Projecte3.DataSonometro> datSon)
        {
            InitializeComponent();
            this.IASList = IASListFromP3;
            ListThresholdRight = ThresholListRight.OrderBy(item => item.Time).ToList();
            ListThresholdLeft = ThresholListLeft.OrderBy(item => item.Time).ToList();
            ListDERRight = DERListRight.OrderBy(item => item.Time).ToList();
            ListDERLeft = DERListLeft.OrderBy(item => item.Time).ToList();
            dataSonometro = datSon.OrderBy(item => item.time_sec).ToList();
        }

        private void Back2P3_Click(object sender, EventArgs e)
        {

        }

        List<IASData> IASList850 = new List<IASData>();
        List<IASData> IASList1500 = new List<IASData>();
        List<IASData> IASList3500 = new List<IASData>();
        List<IASData> ThresholdRight = new List<IASData>();
        List<IASData> ThresholdLeft = new List<IASData>();
        List<IASData> DERRight = new List<IASData>();
        List<IASData> DERLeft = new List<IASData>();
        List<Projecte3.DataSonometro> dataSonometro= new List<Projecte3.DataSonometro>(); 
        public void AjustarDataGrid()
        {
            // Calcular el ancho total de todas las columnas
            int totalWidth = 0;
            foreach (DataGridViewColumn column in IASdatagrid.Columns)
            {
                totalWidth += column.Width;
            }

            // Calcular la altura total de todas las filas
            int totalHeight = 0;
            foreach (DataGridViewRow row in IASdatagrid.Rows)
            {
                totalHeight += row.Height;
            }

            // Ajustar el tamaño del DataGridView según el contenido (añadir un pequeño margen)
            //IASdatagrid.Size = new Size(totalWidth + 20, totalHeight + 20); // El +20 es para el margen

            // Si quieres que el DataGridView se ajuste a la pantalla, puedes establecer un tamaño máximo:
            IASdatagrid.ScrollBars = ScrollBars.Both; // Activar barras de desplazamiento si es necesario

            // Ajusta las columnas al contenido
            //IASdatagrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            IASdatagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Ajusta las filas al contenido
            IASdatagrid.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);

        }
    
        public void ClassifiyIASList(List<IASData> IASList)
        { 
            for (int i = 0; i < IASList.Count; i++)
            {
                if (Math.Abs(850 * GeoUtils.FEET2METERS - IASList[i].Altitude) < 50)
                {
                    IASList850.Add(IASList[i]);
                }
                else if (Math.Abs(1500 * GeoUtils.FEET2METERS - IASList[i].Altitude) < 50)
                {
                    IASList1500.Add(IASList[i]);
                }
                else if ((Math.Abs(3500 * GeoUtils.FEET2METERS - IASList[i].Altitude) < 50))
                {
                    IASList3500.Add(IASList[i]);
                }
            }
        }
      
        public void SetButtons(int i)
        {
            if (i > 0)
            {
                BtnThres24L.Visible = true;
                BtnThres24L.Enabled = true;
                BtnThres06R.Visible = true;
                BtnThres06R.Enabled = true;

                BtnDER24L.Visible = true;
                BtnDER24L.Enabled = true;
                BtnDER06R.Visible = true;
                BtnDER06R.Enabled = true;

                BtnSonometro.Visible = true;
                BtnSonometro.Enabled = true;
                if (i == 1)
                {
                    Btn850.Visible = false;
                    Btn850.Enabled = false;
                    Btn1500.Visible = true;
                    Btn1500.Enabled = true;
                    Btn3500.Visible = true;
                    Btn3500.Enabled = true;
                }
                else if (i == 2)
                {
                    Btn850.Visible = true;
                    Btn850.Enabled = true;
                    Btn1500.Visible = false;
                    Btn1500.Enabled = false;
                    Btn3500.Visible = true;
                    Btn3500.Enabled = true;
                }
                else if (i == 3)
                {
                    Btn850.Visible = true;
                    Btn850.Enabled = true;
                    Btn1500.Visible = true;
                    Btn1500.Enabled = true;
                    Btn3500.Visible = false;
                    Btn3500.Enabled = false;
                }
            }
            else if (i < 0)
            {
                Btn850.Visible = true;
                Btn850.Enabled = true;
                Btn1500.Visible = true;
                Btn1500.Enabled = true;
                Btn3500.Visible = true;
                Btn3500.Enabled = true;

                BtnSonometro.Visible = true;
                BtnSonometro.Enabled = true;
                if (i == -1)
                {
                    BtnThres24L.Visible = false;
                    BtnThres24L.Enabled = false;
                    BtnThres06R.Visible = true;
                    BtnThres06R.Enabled = true;

                    BtnDER24L.Visible = true;
                    BtnDER24L.Enabled = true;
                    BtnDER06R.Visible = true;
                    BtnDER06R.Enabled = true;
                }
                else if (i == -2)
                {
                    BtnThres24L.Visible = true;
                    BtnThres24L.Enabled = true;
                    BtnThres06R.Visible = false;
                    BtnThres06R.Enabled = false;

                    BtnDER24L.Visible = true;
                    BtnDER24L.Enabled = true;
                    BtnDER06R.Visible = true;
                    BtnDER06R.Enabled = true;
                }
                else if (i == -3) 
                {
                    BtnThres24L.Visible = true;
                    BtnThres24L.Enabled = true;
                    BtnThres06R.Visible = true;
                    BtnThres06R.Enabled = true;

                    BtnDER24L.Visible = false;
                    BtnDER24L.Enabled = false;
                    BtnDER06R.Visible = true;
                    BtnDER06R.Enabled = true;
                }
                else if (i == -4)
                {
                    BtnThres24L.Visible = true;
                    BtnThres24L.Enabled = true;
                    BtnThres06R.Visible = true;
                    BtnThres06R.Enabled = true;

                    BtnDER24L.Visible = true;
                    BtnDER24L.Enabled = true;
                    BtnDER06R.Visible = false;
                    BtnDER06R.Enabled = false;
                }
            }
            else if (i == 0)
            {
                BtnThres24L.Visible = true;
                BtnThres24L.Enabled = true;
                BtnThres06R.Visible = true;
                BtnThres06R.Enabled = true;

                BtnDER24L.Visible = true;
                BtnDER24L.Enabled = true;
                BtnDER06R.Visible = true;
                BtnDER06R.Enabled = true;

                Btn850.Visible = true;
                Btn850.Enabled = true;
                Btn1500.Visible = true;
                Btn1500.Enabled = true;
                Btn3500.Visible = true;
                Btn3500.Enabled = true;

                BtnSonometro.Visible = false;
                BtnSonometro.Enabled = false;
            }
            
        }
        private void IASInformation_Load(object sender, EventArgs e)
        {
            ClassifiyIASList(IASList);

            IASdatagrid.DataSource = IASList850.OrderBy(item => item.Time).ToList();
            AjustarDataGrid();
            SetButtons(1);
        }

        private void Btn850_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = IASList850.OrderBy(item => item.Time).ToList();
            SetButtons(1);
        }

        private void Btn1500_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = IASList1500.OrderBy(item => item.Time).ToList();
            SetButtons(2);
        }

        private void Btn3500_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = IASList3500.OrderBy(item => item.Time).ToList();
            SetButtons(3);
        }

        private void BtnThres24L_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = ListThresholdLeft;
            SetButtons(-1);
        }

        private void BtnThres06R_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = ListThresholdRight;
            SetButtons(-2);
        }

        private void BtnDER24L_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = ListDERLeft;
            SetButtons(-3);
        }

        private void BtnDER06R_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = ListDERRight;
            SetButtons(-4);
        }

        private void BtnSonometro_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = dataSonometro;
            SetButtons(0);
        }
    }
}
