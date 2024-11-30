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

namespace FormsAsterix
{
    public partial class IASInformation : Form
    {
        List<IASData> IASList;
        List<IASData> ListRight;
        List<IASData> ListLeft;
        public IASInformation(List<IASData> IASListFromP3, List<IASData> ThresholListRight, List<IASData> ThresholListLeft)
        {
            InitializeComponent();
            this.IASList = IASListFromP3;
            ListRight = ThresholListRight;
            ListLeft = ThresholListLeft;    
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
            IASdatagrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

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
        public void ClassifiyThresholdRight(List<IASData> ThresholdList)
        {
            for (int i = 0; i <= ThresholdList.Count-1; i++)
            {
                if (ThresholdList.Count <= 2)
                {
                    ThresholdRight.Add(ThresholdList[i]);
                    if (i + 1 < 2)
                    {
                        if (ThresholdList[i].AircraftId == ThresholdList[i + 1].AircraftId)
                        {
                            try
                            {
                                DERRight.Add(ThresholdList[i + 1]);
                            }
                            catch { continue; }
                        }
                    }
                }
                else
                {
                    if (i+1 > ThresholdList.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        if (ThresholdList[i].Time < ThresholdList[i + 1].Time && ThresholdList[i].AircraftId == ThresholdList[i + 1].AircraftId)
                        {
                            ThresholdRight.Add(ThresholdList[i]);
                            DERRight.Add(ThresholdList[i + 1]);
                            i++;
                        }
                    }
                    
                }
                
            }
        }
        public void ClassifiyThresholdLeft(List<IASData> ThresholdList)
        {
            
            for (int i = 0; i < ThresholdList.Count - 1; i++)
            {
                if (ThresholdList.Count == 2)
                {
                    ThresholdLeft.Add(ThresholdList[i]);
                    if (i+1 < 2)
                    {
                        if (ThresholdList[i].AircraftId == ThresholdList[i + 1].AircraftId)
                        {
                            try
                            {
                                DERLeft.Add(ThresholdList[i + 1]);
                            }
                            catch { continue; }
                        }
                    }
                }
                else
                {
                    if (i+1 >= ThresholdList.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        if (ThresholdList[i].Time < ThresholdList[i + 1].Time && ThresholdList[i].AircraftId == ThresholdList[i + 1].AircraftId)
                        {
                            ThresholdLeft.Add(ThresholdList[i]);
                            DERLeft.Add(ThresholdList[i + 1]);
                            i++;
                        }
                    }
                    
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
            else
            {
                Btn850.Visible = true;
                Btn850.Enabled = true;
                Btn1500.Visible = true;
                Btn1500.Enabled = true;
                Btn3500.Visible = true;
                Btn3500.Enabled = true;
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
            
        }
        private void IASInformation_Load(object sender, EventArgs e)
        {
            ClassifiyIASList(IASList);
            ClassifiyThresholdRight(ListRight);
            ClassifiyThresholdLeft(ListLeft);
            IASdatagrid.DataSource = IASList850;
            AjustarDataGrid();
            SetButtons(1);
        }

        private void Btn850_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = IASList850;
            SetButtons(1);
        }

        private void Btn1500_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = IASList1500;
            SetButtons(2);
        }

        private void Btn3500_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = IASList3500;
            SetButtons(3);
        }

        private void BtnThres24L_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = ThresholdLeft;
            SetButtons(-1);
        }

        private void BtnThres06R_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = ThresholdRight;
            SetButtons(-2);
        }

        private void BtnDER24L_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = DERLeft;
            SetButtons(-3);
        }

        private void BtnDER06R_Click(object sender, EventArgs e)
        {
            IASdatagrid.DataSource = DERRight;
            SetButtons(-4);
        }
    }
}
