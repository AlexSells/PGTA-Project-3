using LibAsterix;
using OfficeOpenXml;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace FormsAsterix
{
    public partial class Projecte3 : Form
    {
        internal int totalPlanes = 1;
        internal int click = 0;
        public Projecte3(List<PlaneFilter> FilteredList)
        {
            InitializeComponent();
            ListFilteredPlanes = FilteredList;
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(item => item.ID).ToList();
        }
        /*### LIST ############################################################################################################*/
        List<PlaneFilter> ListFilteredPlanes;
        private List<(string PlaneFront, string AircraftTypeFront, string EstelaFront, string ClassFront, string SIDfront, double time_front, string PlaneAfter, string AircraftTypeBack, string EstelaAfter, string ClassAfter, string SIDback, double time_back, bool SameSID, double U, double V, double DistanceDiff, double secondsDiff)> ListDistanceCSV;
        private List<(string PlaneFront, string AircraftTypeFront, string EstelaFront, string ClassFront, string SIDfront, double time_front, string PlaneAfter, string AircraftTypeBack, string EstelaAfter, string ClassAfter, string SIDback, double time_back, bool SameSID, double U, double V, double DistanceDiff, double secondsDiff)> FindDistances()
        {
            List<(string PlaneFront, string AircraftTypeFront, string EstelaFront, string ClassFront, string SIDfront, double time_front, string PlaneAfter, string AircraftTypeBack, string EstelaAfter, string ClassAfter, string SIDback, double time_back, bool SameSID, double U, double V, double DistanceDiff, double secondsDiff)> distances = new List<(string PlaneFront, string AircraftTypeFront, string EstelaFront, string ClassFront, string SIDfront, double time_front, string PlaneAfter, string AircraftTypeBack, string EstelaAfter, string ClassAfter, string SIDback, double time_back, bool SameSID, double U, double V, double DistanceDiff, double secondsDiff)>();
            totalPlanes = 1;
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(item => item.ID).ToList();
            int auxID = ListFilteredPlanes[0].ID; // ID con la que trabajamos
            bool first = false; // canviara al encontarr una ID distinta
            int auxSegimiento = 1;

            for (int i = 0; i < ListFilteredPlanes.Count - 1; i++)
            {
                string ClassFront = "R";
                if (ClasPlanes.ContainsKey(ListFilteredPlanes[i].AircraftType))
                {
                    ClassFront = ClasPlanes[ListFilteredPlanes[i].AircraftType];
                }
                string SameSIDFront = "";
                if (ClasSID.ContainsKey(ListFilteredPlanes[i].TakeoffProcess))
                {
                    SameSIDFront = ClasSID[ListFilteredPlanes[i].TakeoffProcess];
                }

                if (auxID != ListFilteredPlanes[i].ID && auxID < ListFilteredPlanes[i].ID)
                {
                    auxID = ListFilteredPlanes[i].ID;
                    totalPlanes++;
                    first = true;
                }
                
                for (int j = auxSegimiento; j < ListFilteredPlanes.Count; j++)
                {
                    double auxSeconds = ListFilteredPlanes[j].time_sec-ListFilteredPlanes[i].time_sec;
                    if (Math.Abs(auxSeconds) < 4 && ListFilteredPlanes[j].ID - ListFilteredPlanes[i].ID == 1)
                    {
                        if (first==true) //comprovamos que seguimos el orden correcto
                        {
                            double auxSecondsBack = ListFilteredPlanes[j].time_sec - ListFilteredPlanes[i + 1].time_sec;
                            if (Math.Abs(auxSeconds) > Math.Abs(auxSecondsBack))
                            {
                                auxSeconds = auxSecondsBack;
                                i++;
                            }
                            first = false;
                        }
                        else if (auxSegimiento - j < 0) // If some data is not avaible and it catches the data after
                        {
                            double auxSecondsBack = ListFilteredPlanes[j + 1].time_sec - ListFilteredPlanes[i].time_sec;
                            if (Math.Abs(auxSecondsBack) < Math.Abs(auxSeconds))
                            {
                                auxSeconds = auxSecondsBack;
                                j++;
                            }
                        }

                        double delta_U = Math.Abs(ListFilteredPlanes[i].U - ListFilteredPlanes[j].U);
                        double delta_V = Math.Abs(ListFilteredPlanes[i].V - ListFilteredPlanes[j].V);
                        double distanceDiff = Math.Sqrt(Math.Pow(delta_U, 2) + Math.Pow(delta_V, 2));

                        string ClassBack = "R";
                        if (ClasPlanes.ContainsKey(ListFilteredPlanes[j].AircraftType))
                        {
                            ClassBack = ClasPlanes[ListFilteredPlanes[j].AircraftType];
                        }
                        string SameSIDBack = "";
                        if (ClasSID.ContainsKey(ListFilteredPlanes[j].TakeoffProcess))
                        {
                            SameSIDBack = ClasSID[ListFilteredPlanes[j].TakeoffProcess];
                        }
                        bool SameSID = SameIDCheck(SameSIDFront, SameSIDBack);

                        bool boolSID = SameSIDFront == SameSIDBack ? true : false;
                        auxSegimiento = j+1;

                        distances.Add((ListFilteredPlanes[i].AircraftID, ListFilteredPlanes[i].AircraftType, ListFilteredPlanes[i].EstelaType, ClassFront, SameSIDFront, ListFilteredPlanes[i].time_sec, ListFilteredPlanes[j].AircraftID, ListFilteredPlanes[i].AircraftType, ListFilteredPlanes[j].EstelaType, ClassBack, SameSIDBack, ListFilteredPlanes[j].time_sec, boolSID, delta_U, delta_V, distanceDiff, auxSeconds));
                        break;
                    }
                    else if (ListFilteredPlanes[j].ID - ListFilteredPlanes[i].ID > 1) { break; }
                }
            }
            ANSTotalPlanes.Text = $"Total planes analazied = {totalPlanes}";
            return distances;
        }
        /*### DICTIONARIES ######################################################################################################*/
        Dictionary<(string, string), int> Estelas = new Dictionary<(string, string), int>
        {
            //_ Super Heavy _______________________
            {("Super Pesada","Pesada"), 6 }, {("Super Pesada","Media"), 7 }, {("Super Pesada","Ligera"), 8 },
            //_ Heavy _____________________________
            {("Pesada","Pesada"), 4 }, {("Pesada","Media"), 5 }, {("Pesada","Ligera"), 6 },
            //_Medium _____________________________
            {("Media","Ligera"), 5 }
        };
        Dictionary<(string, string, bool), int> LoA = new Dictionary<(string, string, bool), int>
        {
            //_ HP ________________________________
            {("HP","HP",true), 5}, {("HP","R",true), 5}, {("HP","LP",true), 5}, {("HP","NR+",true), 3}, {("HP","NR-",true), 3}, {("HP","NR",true), 3}, {("HP","HP",false), 3}, {("HP","R",false), 3}, {("HP","LP",false), 3}, {("HP","NR+",false), 3}, {("HP","NR-",false), 3}, {("HP","NR",false), 3},
            //_ R  ________________________________
            {("R","HP",true), 7}, {("R","R",true), 5}, {("R","LP",true), 5}, {("R","NR+",true), 3}, {("R","NR-",true), 3}, {("R","NR",true), 3}, {("R","HP",false), 5}, {("R","R",false), 3}, {("R","LP",false), 3}, {("R","NR+",false), 3}, {("R","NR-",false), 3}, {("R","NR",false), 3},
            //_ LP ________________________________
            {("LP","HP",true), 8}, {("LP","R",true), 6}, {("LP","LP",true), 5}, {("LP","NR+",true), 3}, {("LP","NR-",true), 3}, {("LP","NR",true), 3}, {("LP","HP",false), 6}, {("LP","R",false), 4}, {("LP","LP",false), 3}, {("LP","NR+",false), 3}, {("LP","NR-",false), 3}, {("LP","NR",false), 3},
            //_ NR+ ________________________________
            {("NR+","HP",true), 11}, {("NR+","R",true), 9}, {("NR+","LP",true), 9}, {("NR+","NR+",true), 5}, {("NR+","NR-",true), 3}, {("NR+","NR",true), 3}, {("NR+","HP",false), 8}, {("NR+","R",false), 6}, {("NR+","LP",false), 6}, {("NR+","NR+",false), 3}, {("NR+","NR-",false), 3}, {("NR+","NR",false), 3},
            //_ NR- ________________________________
            {("NR-","HP",true), 9}, {("NR-","R",true), 9}, {("NR-","LP",true), 9}, {("NR-","NR+",true), 9}, {("NR-","NR-",true), 5}, {("NR-","NR",true), 3}, {("NR-","HP",false), 9}, {("NR-","R",false), 9}, {("NR-","LP",false), 9}, {("NR-","NR+",false), 6}, {("NR-","NR-",false), 3}, {("NR-","NR",false), 3},
            //_ NR ________________________________
            {("NR","HP",true), 9}, {("NR","R",true), 9}, {("NR","LP",true), 9}, {("NR","NR+",true), 9}, {("NR","NR-",true), 9}, {("NR","NR",true), 5}, {("NR","HP",false), 9}, {("NR","R",false), 9}, {("NR","LP",false), 9}, {("NR","NR+",false), 9}, {("NR","NR-",false), 9}, {("NR","NR",false), 3},
        };
        Dictionary<string, string> ClasSID = new Dictionary<string, string> { };
        Dictionary<string, string> ClasPlanes = new Dictionary<string, string> { };
        /*### FUNCTIONS #########################################################################################################*/
        public bool SameIDCheck(string SameSIDFront, string SameSIDBack)
        {
            if (SameSIDFront == SameSIDBack)
            {
                return true;
            }
            return false;
        }
        public void Classifier(string filePath, ref Dictionary<string, string> dict)
        {
            try
            {
                ExcelPackage.LicenseContext = ExcelPackage.LicenseContext;
                using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;
                    int columnCount = worksheet.Dimension.Columns;

                    for (int col = 1; col <= columnCount; col++)
                    {
                        var First_Cell_Value = "";
                        for (int row = 1; row <= rowCount; row++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value?.ToString() ?? "";
                            if (row != 1 && cellValue != "" && !dict.ContainsKey(cellValue))
                            {
                                dict.Add(cellValue, First_Cell_Value);
                            }
                            else if (row == 1 && cellValue != "") { First_Cell_Value = cellValue; }
                            else { continue; }
                        }
                    }
                }
            } catch { };
            
        }
        public string SelectExcel()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Please, select file";
            ofd.InitialDirectory = @"C:\";
            ofd.Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();
            return ofd.FileName;
        }
        public bool StartClassification(string auxFile, ref Dictionary<string, string> dict)
        {
            bool aux = false;
            string filePath = SelectExcel();
            if (filePath != "" && filePath != null)
            {
                string lastFile = Path.GetFileName(filePath);
               // MessageBox.Show(lastFile);
                if (lastFile == auxFile)
                {
                    Classifier(filePath, ref dict);
                    click++;
                    aux = true;
                }
                else
                {
                    MessageBox.Show("Ooops! An error ocurred, please select the file again");
                }
            }
            return aux;
        }
        public void ClearNumAux()
        {
            numPlanesEstela = 0;
            numPlanesLOA = 0;
            numPlanesRadar = 0;
            numPlanesTotal = 0;
            countEstela = 0;
        }  
        public bool CheckRadarMinima(double DistDiff)
        {
            bool check = true;
            if (DistDiff <= 3.0) 
            { 
                check = false;
                numPlanesRadar++;
                TotalRadarIncidents++;
            }
            return check;
        }
        public bool CheckLoAminima(double DistDiff,string ClassFront, string ClassBack, bool SameSID)
        {
            bool check = true; 
            if (LoA.ContainsKey((ClassFront, ClassBack, SameSID)))
            {
                if (DistDiff <= LoA[(ClassFront, ClassBack, SameSID)])
                {
                    check = false;
                    TotalLoAIncidents++;
                    numPlanesLOA++;
                }
            }
            return check;
        }
        public bool CheckEstelaMinima(double DistDiff, string EstelaFront, string EstelaBack)
        {
            bool check = true;
            if (DistDiff <= Estelas[(EstelaFront, EstelaBack)])
            {
                check = false;
                numPlanesEstela++;
                TotalEstelaIncidents++;
            }
            return check;
        }
        public bool IncidenceDetector(bool auxDetection, string PlaneFront, string auxPlaneBack)
        {
            bool check = true;
            if (!auxDetection)
            {
                if (auxPlaneBack == PlaneFront) { TotalIncidencePlanes++; }
                else { TotalIncidencePlanes = TotalIncidencePlanes + 2; }
            }
            else
            {
                auxDetection = false;
                TotalIncidencePlanes = TotalIncidencePlanes + 2;
            }
            return check;
        }
        public void GetListDistanceCSV(bool aux)
        {
            if (aux == true)
            {
              ListDistanceCSV = FindDistances();
            }
        }
        public int CheckConsecutive(int auxSeguiment, int i, bool found, string SameSIDFront, string ClassFront)
        {
           
            return 0;
        }
        /*### EVENTS ############################################################################################################*/
        
        private void Back2P2Btn_Click(object sender, EventArgs e)
        {
            
        }

        private void LoadTableBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_Clasificacion_aeronaves.xlsx", ref ClasPlanes);
            GetListDistanceCSV(aux);
        }

        private void LoadSID06RBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_misma_SID_06R.xlsx", ref ClasSID);
            GetListDistanceCSV(aux);
        }

        private void LoadSID24LBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_misma_SID_24L.xlsx", ref ClasSID);
            GetListDistanceCSV(aux);
        }

        // main variables
        int TotalIncidencePlanes = 0;
        int TotalMessageComparation = 0;
        int TotalEstaleComparationMessages = 0;
        int TotalRadarIncidents = 0;
        int TotalEstelaIncidents = 0;
        int TotalLoAIncidents = 0;
        // Auxiliar variables (will be erased each iteration)
        int numPlanesTotal, numPlanesRadar, numPlanesEstela, numPlanesLOA, countEstela, numPlanesIncidence = 0, numPlanesComparision = 0;
        
        private void GenStatisticsBtn_Click(object sender, EventArgs e)
        {
            List<int> listIncidents = new List<int>();
            listIncidents.Add(TotalIncidencePlanes);
            listIncidents.Add(TotalMessageComparation);
            listIncidents.Add(TotalEstaleComparationMessages);
            listIncidents.Add(TotalRadarIncidents);
            listIncidents.Add(TotalEstelaIncidents);
            listIncidents.Add(TotalLoAIncidents);
            GeneralStatistics GenStats = new GeneralStatistics(ListDistanceCSV, listIncidents);
            GenStats.Show();
        }

        private void DistanceCSVBtn_Click(object sender, EventArgs e)
        {
            //GenStatisticsBtn.Enabled = true;
            //GenStatisticsBtn.Visible = true;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "File CSV| *.csv";
            saveFileDialog.Title = "Save CSV file";
            saveFileDialog.InitialDirectory = @"C:\"; //Punto de inicio

            //ListDistanceCSV = FindDistances();

            // Muestra que el archiva se ha guardado correctamente
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                List<(string planeFront, string planeBack, int totalRadar, int totalEstela, int totalLOA)> InfringementCSV = new List<(string planeFront, string planeBack, int totalRadar, int totalEstela, int totalLOA)>();
                string filePath = saveFileDialog.FileName;
                StringBuilder sbCSV = new StringBuilder();

                //Variables auxiliares
                string auxDatos;
                bool auxDetection = true;
                string auxPlaneFront = ListDistanceCSV[0].PlaneFront;
                string auxPlaneBack = ListDistanceCSV[0].PlaneAfter;


                // Preparamos las cabeceras donde el delimitador de columna sera el signo =
                sbCSV.AppendLine("Plane 1= Type_plane 1=Estela 1=Clasification 1=SID 1=Time_1=Plane 2= Type_plane 2=Estela 2=Clasification 2=SID 2=Time_2=Interval time (s)=Delta_U (NM)=Delta_V (NM)=Distance_between (NM)=Minima_radar=Minima_Estela=Minima_LoA=Total of both= Total estela= Radar= Estela = LoA ");

                for (int i = 1; i < ListDistanceCSV.Count; i++)
                {

                    var aux = ListDistanceCSV[i];
                    bool MinRadar = true;
                    bool MinEstela = true;
                    bool MinLoA = true;

                    // Contamos el numero de aviones analizados
                    numPlanesTotal++; 
                    TotalMessageComparation++;
                    // Comprovamos si se comple la distancia minima de radar en NM (Nautical Miles)
                    MinRadar = CheckRadarMinima(aux.DistanceDiff);
                    // Comprovamos si se comple la distancia minima de LoA
                    MinLoA=CheckLoAminima(aux.DistanceDiff,aux.ClassFront, aux.ClassAfter, aux.SameSID);    
                    // Comprovamos si se comple la distancia minima de estela
                    if (Estelas.ContainsKey((aux.EstelaFront, aux.EstelaAfter)))
                    {
                        countEstela++;
                        TotalEstaleComparationMessages++;
                        MinEstela = CheckEstelaMinima(aux.DistanceDiff, aux.EstelaFront, aux.EstelaAfter);

                        if ((i + 1) < ListDistanceCSV.Count && auxPlaneFront != ListDistanceCSV[i + 1].PlaneFront)
                        {
                            auxDatos = $"={Convert.ToString(numPlanesTotal)}={Convert.ToString(countEstela)}={Convert.ToString(numPlanesRadar)}={Convert.ToString(numPlanesEstela)}={Convert.ToString(numPlanesLOA)}";
                            if ((numPlanesRadar != 0 || numPlanesEstela != 0 || numPlanesLOA != 0))
                            {
                                auxDetection = IncidenceDetector(auxDetection, ListDistanceCSV[i].PlaneFront, auxPlaneBack);
                                auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                            }
                            auxPlaneFront = ListDistanceCSV[i + 1].PlaneFront;
                            ClearNumAux();
                        }
                        else { auxDatos = ""; }
                        if (aux.PlaneFront != null) //aux.PlaneAfter)
                        {
                            string data = $"{aux.PlaneFront}={aux.AircraftTypeFront}={aux.EstelaFront}={aux.ClassFront}={aux.SIDfront}={Convert.ToString(aux.time_front)}={aux.PlaneAfter}={aux.AircraftTypeBack}={aux.EstelaAfter}={aux.ClassAfter}={aux.SIDback}={Convert.ToString(aux.time_back)}={Convert.ToString(aux.secondsDiff)}={Convert.ToString(aux.U)}={Convert.ToString(aux.V)}={Convert.ToString(aux.DistanceDiff)}={TotalRadarIncidents}={TotalEstaleComparationMessages}={MinRadar}= N/A ={MinLoA}" + auxDatos;
                            sbCSV.AppendLine(data);
                        }
                    }
                    else
                    {
                        if ((i + 1) < ListDistanceCSV.Count && auxPlaneFront != ListDistanceCSV[i + 1].PlaneFront)
                        {
                            auxDatos = $"={Convert.ToString(numPlanesTotal)}={Convert.ToString(countEstela)}={Convert.ToString(numPlanesRadar)}={Convert.ToString(numPlanesEstela)}={Convert.ToString(numPlanesLOA)}";
                            if ((numPlanesRadar != 0 || numPlanesEstela != 0 || numPlanesLOA != 0))
                            {
                                /*if (!auxDetection)
                                {
                                    if (auxPlaneBack == ListDistanceCSV[i].PlaneFront) { TotalIncidencePlanes++; }
                                    else { TotalIncidencePlanes = TotalIncidencePlanes + 2; }
                                    auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                                }
                                else
                                {
                                    auxDetection = false;
                                    auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                                    TotalIncidencePlanes = TotalIncidencePlanes + 2;
                                }*/
                                auxDetection = IncidenceDetector(auxDetection, ListDistanceCSV[i].PlaneFront, auxPlaneBack);
                                auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                            }
                            auxPlaneFront = ListDistanceCSV[i + 1].PlaneFront;
                            ClearNumAux();

                        }
                        else { auxDatos = ""; }
                        //sbCSV.AppendLine("Plane 1= Type_plane 1=Estela 1=Clasification 1=SID 1=Time_1=Plane 2= Type_plane 2=Estela 2=Clasification 2=SID 2=Time_2=Interval time (s)=Delta_U (NM)=Delta_V (NM)=Distance_between (NM)=Minima_radar=Minima_Estela=Minima_LoA=Total of both= Total estela= Radar= Estela = LoA ");
                        if (aux.PlaneFront != null) {//aux.PlaneAfter){
                            string data = $"{aux.PlaneFront}={aux.AircraftTypeFront}={aux.EstelaFront}={aux.ClassFront}={aux.SIDfront}={Convert.ToString(aux.time_front)}={aux.PlaneAfter}={aux.AircraftTypeBack}={aux.EstelaAfter}={aux.ClassAfter}={aux.SIDback}={Convert.ToString(aux.time_back)}={Convert.ToString(aux.secondsDiff)}={Convert.ToString(aux.U)}={Convert.ToString(aux.V)}={Convert.ToString(aux.DistanceDiff)}={TotalRadarIncidents}={TotalEstaleComparationMessages}={MinRadar}= N/A ={MinLoA}" + auxDatos;
                            sbCSV.AppendLine(data);
                        }
                    }
                }
                

                File.WriteAllText(filePath, sbCSV.ToString());
                MessageBox.Show("CSV file generated");
            }
            else { MessageBox.Show("CSV file generation failed"); }
        }


        public void EreaseMainColumn(string nombreEncabezado)
        {
            // Buscar la columna por nombre de encabezado
            foreach (DataGridViewColumn columna in dataGridProject3.Columns)
            {
                if (columna.HeaderText == nombreEncabezado)
                {
                    // Eliminar la columna
                    dataGridProject3.Columns.Remove(columna);
                    return; // Salir del método después de eliminar la columna
                }
            }
        }
        public void EreaseFilteredColumn(string nombreEncabezado)
        {
            // Buscar la columna por nombre de encabezado
            foreach (DataGridViewColumn columna in dataGridProject3.Columns)
            {
                if (columna.HeaderText == nombreEncabezado)
                {
                    // Eliminar la columna
                    dataGridProject3.Columns.Remove(columna);
                    return; // Salir del método después de eliminar la columna
                }
            }
        }
        public void ReplaceString2(int column, string string2change, string replace, int selGrid)
        {
            
            if (selGrid == 0)
            {
                if (dataGridProject3.Rows.Count != 0)
                {
                    // Convertir la columna 7 en una columna de strings
                    ConvertMainColumnToString(column, "Roll Angle", "Roll Angle");

                    // Recorrer las filas y modificar los valores
                    foreach (DataGridViewRow fila in dataGridProject3.Rows)
                    {
                        if (!fila.IsNewRow) // Ignorar filas nuevas
                        {
                            // Obtener el valor actual de la celda
                            object valorActual = fila.Cells[column].Value;

                            // Reemplazar el valor si coincide con `string2change`
                            if (valorActual != null && valorActual.ToString() == string2change)
                            {
                                fila.Cells[column].Value = replace; // Asignar el nuevo valor como string
                            }
                            else if (valorActual != null)
                            {
                                fila.Cells[column].Value = valorActual.ToString(); // Asegurar que sea string
                            }
                        }
                    }
                }
            }
            else
            {
                if (DataGridFiltrado.Rows.Count != 0)
                {
                    // Convertir la columna 7 en una columna de strings
                    ConvertFilteredColumnToString(column, "Roll Angle", "Roll Angle");

                    // Recorrer las filas y modificar los valores
                    foreach (DataGridViewRow fila in DataGridFiltrado.Rows)
                    {
                        if (!fila.IsNewRow) // Ignorar filas nuevas
                        {
                            // Obtener el valor actual de la celda
                            object valorActual = fila.Cells[column].Value;

                            // Reemplazar el valor si coincide con `string2change`
                            if (valorActual != null && valorActual.ToString() == string2change)
                            {
                                fila.Cells[column].Value = replace; // Asignar el nuevo valor como string
                            }
                            else if (valorActual != null)
                            {
                                fila.Cells[column].Value = valorActual.ToString(); // Asegurar que sea string
                            }
                        }
                    }
                }
            }
            
        }

        // Método para convertir una columna en tipo string
        public void ConvertMainColumnToString(int column, string name, string header)
        {
            // Crear una nueva columna de tipo string
            DataGridViewTextBoxColumn nuevaColumna = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                ValueType = typeof(string)
            };

            // Guardar el índice de la columna actual
            int indiceColumna = column;

            // Eliminar la columna antigua
            dataGridProject3.Columns.RemoveAt(indiceColumna);

            // Insertar la nueva columna en la misma posición
            dataGridProject3.Columns.Insert(indiceColumna, nuevaColumna);
            int i = 0;
            // Copiar los valores antiguos a la nueva columna como strings
            foreach (DataGridViewRow fila in dataGridProject3.Rows)
            {
                if (!fila.IsNewRow) // Ignorar filas nuevas
                {
                    object valorAntiguo = fila.Cells[indiceColumna].Value;
                    fila.Cells[indiceColumna].Value = ListFilteredPlanes[i].RollAngle.ToString(); // Convertir a string
                }
                i++;
            }
            
        }
        
        // Método para convertir una columna en tipo string
        public void ConvertFilteredColumnToString(int column, string name, string header)
        {
            // Crear una nueva columna de tipo string
            DataGridViewTextBoxColumn nuevaColumna = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                ValueType = typeof(string)
            };

            // Guardar el índice de la columna actual
            int indiceColumna = column;

            // Eliminar la columna antigua
            DataGridFiltrado.Columns.RemoveAt(indiceColumna);

            // Insertar la nueva columna en la misma posición
            DataGridFiltrado.Columns.Insert(indiceColumna, nuevaColumna);
            int i = 0;
            // Copiar los valores antiguos a la nueva columna como strings
            foreach (DataGridViewRow fila in DataGridFiltrado.Rows)
            {
                if (!fila.IsNewRow) // Ignorar filas nuevas
                {
                    object valorAntiguo = fila.Cells[indiceColumna].Value;
                    fila.Cells[indiceColumna].Value = ListFilteredPlanes[fila.Index].RollAngle.ToString(); // Convertir a string
                }
                i++;
            }

        }
        private void Projecte3_Load(object sender, EventArgs e)
        {
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(data => data.time_sec).ToList();
            dataGridProject3.RowHeadersDefaultCellStyle.Font = new Font(dataGridProject3.Font, FontStyle.Bold);
            dataGridProject3.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;
            dataGridProject3.DataSource = ListFilteredPlanes;
            ReplaceString2(7,"-999", "NAN", 0);

            //GenStatisticsBtn.Enabled = false;
            //GenStatisticsBtn.Visible = false;
        }



        
        bool filterEnabled = false;
        private void Btn_Filter_Click(object sender, EventArgs e)
        {
            // Make the Filtered_Values, No_ground_flights, and blancos_puros controls visible.
            FilteredValues.Visible = true;
            //No_ground_flights.Visible = true;
            //blancos_puros.Visible = true;

            // Bring the toolStrip1 to the front of other controls, making it visible above other elements.
            //toolStrip1.BringToFront();

            // Toggle the state of the filterEnabled variable between true and false.
            filterEnabled = !filterEnabled;

            // If filter is enabled (i.e., filterEnabled is true):
            if (filterEnabled)
            {
                // Add arrows to column headers to indicate sorting or filtering.
                AddArrowToColumnHeaders(true);
                dataGridProject3.ColumnHeaderMouseClick += dataGridProjec3_ColumnHeaderMouseClick;
            }
            else
            {
                // Remove the arrows from the column headers when filtering is disabled.
                AddArrowToColumnHeaders(false);
                ResetForm();
            }

            // Refresh the DataGridView to reflect any changes in its UI.
            dataGridProject3.Refresh();
            ReplaceString2(7, "-999", "NAN", 0);
            // Eliminar la columna
            dataGridProject3.Columns.RemoveAt(0);
        }
        private Dictionary<string, List<Control>> filterControls = new Dictionary<string, List<Control>>();
        private Dictionary<string, Dictionary_Info> originalColumnNames = new Dictionary<string, Dictionary_Info>();
        bool filterDisabled = false;
        BindingSource bindingSource = new BindingSource();
        private void dataGridProjec3_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check if filtering is enabled
            if (filterEnabled)
            {
                if (!filterEnabled) return;

                string columnName = dataGridProject3.Columns[e.ColumnIndex].DataPropertyName;


                if (filterControls.ContainsKey(columnName))
                {
                    RemoveFilterControls(columnName);
                }
                else
                {
                    ShowFilterBox(columnName, e.ColumnIndex);
                }

            }

        }
        // Method to get distinct values from a specific column in the DataGridView
        private IEnumerable<object> GetDistinctValues(DataGridViewColumn column)
        {
            return dataGridProject3.Rows.Cast<DataGridViewRow>()
                .Select(row => row.Cells[column.Name].Value)
                .Distinct();
        }

        // Dictionary to store the selected filter values for each column
        private Dictionary<string, object> selectedFilters = new Dictionary<string, object>();

        // Dictionary to store the selected range (min, max) for columns that require a range filter
        private Dictionary<string, Tuple<int, int>> rangoSeleccionado = new Dictionary<string, Tuple<int, int>>();


        private void ShowFilterBox(string columna, int index)
        {

            // Remove any existing filter controls for the selected column.
            RemoveFilterControls(columna);
            //string propertyName = dataGridProject3.Columns[index].DataPropertyName;
            var uniques = ListFilteredPlanes.Select(x => x.GetType().GetProperty(columna)?.GetValue(x)?.ToString())
                            .Distinct()
                            .Where(val => val != null)
                            .ToList();


            Rectangle headerRect = dataGridProject3.GetCellDisplayRectangle(index, -1, true);
            var specificColumns = new List<string> { "Lat", "Lon", "Altitude"};
            List<Control> controlsToAdd = new List<Control>();

            // If there are more than 10 unique values, show a control to select a range
            if (columna == "Lat" || columna == "Lon" || columna == "Altitude")
            {
                // If the column is numeric, display a numeric range; otherwise, request a text range
                if (uniques.All(val => double.TryParse(val, out _)))
                {
                    // Determine the minimum and maximum values to set up the ranges
                    var minValue = uniques.Min(val => Convert.ToDouble(val));
                    var maxValue = uniques.Max(val => Convert.ToDouble(val));

                    // Factor for scaling double to integer
                    double range = maxValue - minValue;
                    double scaleFactor = range < 10 ? 1000 : 100;

                    // Normalize the min and max values to be integers
                    int minTrackBarValue = (int)(minValue * scaleFactor);
                    int maxTrackBarValue = (int)(maxValue * scaleFactor);

                    Label minLabel = new Label
                    {
                        Text = "Mínimo:",
                        Location = new Point(headerRect.X, headerRect.Bottom + 10),
                        Width = 60
                    };

                    Label maxLabel = new Label
                    {
                        Text = "Máximo:",
                        Location = new Point(headerRect.X, headerRect.Bottom + 50),
                        Width = 60
                    };

                    Label minValueLabel = new Label
                    {
                        Text = minValue.ToString(),
                        Location = new Point(headerRect.X + 70, headerRect.Bottom + 10),
                        Width = 80
                    };

                    Label maxValueLabel = new Label
                    {
                        Text = maxValue.ToString(),
                        Location = new Point(headerRect.X + 70, headerRect.Bottom + 50),
                        Width = 80
                    };

                    TrackBar minTrackBar = new TrackBar
                    {
                        Minimum = minTrackBarValue,
                        Maximum = maxTrackBarValue,
                        Value = minTrackBarValue,
                        Location = new Point(headerRect.X + 150, headerRect.Bottom + 10),
                        Width = 200
                    };

                    TrackBar maxTrackBar = new TrackBar
                    {
                        Minimum = minTrackBarValue,
                        Maximum = maxTrackBarValue,
                        Value = maxTrackBarValue,
                        Location = new Point(headerRect.X + 150, headerRect.Bottom + 50),
                        Width = 200
                    };

                    // Update values when TrackBar changes
                    minTrackBar.Scroll += (s, e) =>
                    {
                        double currentMinValue = minTrackBar.Value / scaleFactor;
                        minValueLabel.Text = currentMinValue.ToString("F2");
                    };

                    maxTrackBar.Scroll += (s, e) =>
                    {
                        double currentMaxValue = maxTrackBar.Value / scaleFactor;
                        maxValueLabel.Text = currentMaxValue.ToString("F2");
                    };

                    Button acceptButton = new Button()
                    {
                        Text = "Aceptar",
                        Height = 30,
                        Location = new Point(headerRect.X, maxTrackBar.Bottom + 10),
                        FlatStyle = FlatStyle.Flat
                    };

                    acceptButton.FlatAppearance.BorderSize = 3;
                    acceptButton.FlatAppearance.BorderColor = Color.Black;

                    acceptButton.Click += (s, e) =>
                    {
                        double selectedMin = minTrackBar.Value / scaleFactor;
                        double selectedMax = maxTrackBar.Value / scaleFactor;
                        // Apply the filter with the selected values
                        selectedFilters[columna] = new List<double> { selectedMin, selectedMax };

                        acceptButton.Dispose();
                        //RemoveFilterControls(columna);
                        dataGridProject3.Controls.Remove(minLabel);
                        dataGridProject3.Controls.Remove(maxLabel);
                        dataGridProject3.Controls.Remove(minValueLabel);
                        dataGridProject3.Controls.Remove(maxValueLabel);
                        dataGridProject3.Controls.Remove(minTrackBar);
                        dataGridProject3.Controls.Remove(maxTrackBar);
                        dataGridProject3.Controls.Remove(acceptButton);

                    };

                    // Add controls to the DataGridView so they are displayed on the screen
                    dataGridProject3.Controls.Add(minLabel);
                    dataGridProject3.Controls.Add(maxLabel);
                    dataGridProject3.Controls.Add(minValueLabel);
                    dataGridProject3.Controls.Add(maxValueLabel);
                    dataGridProject3.Controls.Add(minTrackBar);
                    dataGridProject3.Controls.Add(maxTrackBar);
                    dataGridProject3.Controls.Add(acceptButton);

                    // Bring each control to the front so they are visible
                    minLabel.BringToFront();
                    maxLabel.BringToFront();
                    minValueLabel.BringToFront();
                    maxValueLabel.BringToFront();
                    minTrackBar.BringToFront();
                    maxTrackBar.BringToFront();
                    acceptButton.BringToFront();

                    dataGridProject3.Controls.AddRange(new Control[] { minLabel, maxLabel, minValueLabel, maxValueLabel, minTrackBar, maxTrackBar, acceptButton });
                    controlsToAdd.AddRange(new Control[] { minLabel, maxLabel, minValueLabel, maxValueLabel, minTrackBar, maxTrackBar, acceptButton });
                }
                else
                {
                    // If the values are text, show a range of values using TextBox controls
                    TextBox minValueTextBox = new TextBox
                    {
                        Width = 100,
                        Location = new Point(headerRect.X, headerRect.Bottom + 10)
                    };

                    TextBox maxValueTextBox = new TextBox
                    {
                        Width = 100,
                        Location = new Point(minValueTextBox.Right + 10, headerRect.Bottom + 10)
                    };

                    Button acceptButton = new Button
                    {
                        Text = "Aceptar",
                        Height = 30,
                        Location = new Point(maxValueTextBox.Right + 10, headerRect.Bottom + 10),
                        FlatStyle = FlatStyle.Flat
                    };

                    acceptButton.Click += (s, e) =>
                    {
                        // Apply filter based on the text range
                        var minVal = minValueTextBox.Text;
                        var maxVal = maxValueTextBox.Text;

                        // Get values within the specified text range
                        var rangeValues = uniques.Where(val => string.Compare(val, minVal) >= 0 && string.Compare(val, maxVal) <= 0).ToList();

                        if (selectedFilters.ContainsKey(columna))
                        {
                            selectedFilters[columna] = rangeValues;
                        }
                        else
                        {
                            selectedFilters.Add(columna, rangeValues);
                        }

                        // Clean up controls
                        minValueTextBox.Dispose();
                        maxValueTextBox.Dispose();
                        acceptButton.Dispose();
                    };

                    dataGridProject3.Controls.Add(minValueTextBox);
                    dataGridProject3.Controls.Add(maxValueTextBox);
                    dataGridProject3.Controls.Add(acceptButton);
                    minValueTextBox.BringToFront();
                    maxValueTextBox.BringToFront();
                    acceptButton.BringToFront();


                }
            }
            else
            {
                // Show the filter with a CheckedListBox for columns with fewer than 10 unique values
                CheckedListBox filterBox = new CheckedListBox
                {
                    DataSource = uniques,
                    Width = dataGridProject3.Columns[index].Width,
                    Height = 150,
                    IntegralHeight = true,
                    Location = new Point(headerRect.X, headerRect.Bottom)
                };

                // Handle item check events in the CheckedListBox
                filterBox.ItemCheck += (s, e) =>
                {
                    var timer = new Timer();
                    timer.Interval = 100;
                    timer.Tick += (sender, args) =>
                    {
                        // Get all selected values
                        var values = filterBox.CheckedItems.Cast<string>().ToList();

                        // Add or update the selected filters for this column
                        if (selectedFilters.ContainsKey(columna))
                        {
                            selectedFilters[columna] = values;
                        }
                        else
                        {
                            selectedFilters.Add(columna, values);
                        }

                        timer.Stop();
                    };
                    timer.Start();
                };

                Button acceptButton = new Button
                {
                    Text = "Aceptar",
                    Height = 30,
                    Location = new Point(headerRect.X, filterBox.Bottom + 5),
                    FlatStyle = FlatStyle.Flat
                };

                acceptButton.Click += (s, e) =>
                {
                    // Clean up filter box and button when the user accepts
                    filterBox.Dispose();
                    acceptButton.Dispose();
                };

                // Set up the location for the filter box
                filterBox.Location = new Point(headerRect.X, headerRect.Bottom);

                // Add controls to the DataGridView
                dataGridProject3.Controls.Add(filterBox);
                dataGridProject3.Controls.Add(acceptButton);

                // Track the controls for this column
                controlsToAdd.AddRange(new Control[] { filterBox, acceptButton });

                // Bring controls to the front
                filterBox.BringToFront();
                acceptButton.BringToFront();
            }

            // Store the controls associated with the current column for later management
            filterControls[columna] = controlsToAdd;

        }

        public void ApplyFilter()
        {
            // Filter the data based on the selected filters
            var filteredData = ListFilteredPlanes.Where(item =>
            {
                foreach (var filter in selectedFilters)
                {
                    var propertyValue = item.GetType().GetProperty(filter.Key)?.GetValue(item)?.ToString();
                    if (propertyValue == null)
                    {
                        MessageBox.Show($"Property {filter.Key} does not exist in YourDataObject!");
                        return false;
                    }

                    if (filter.Value is List<string> stringValues && !stringValues.Contains(propertyValue))
                    {
                        return false;
                    }
                    else if (filter.Value is Tuple<double, double> range && double.TryParse(propertyValue, out double numericValue))
                    {
                        if (numericValue < range.Item1 || numericValue > range.Item2) return false;
                    }
                }
                return true;
            }).ToList();


            // If no data matches the filters, show a message and stop
            if (filteredData.Count == 0)
            {
                MessageBox.Show("No hay datos con los filtros");
                return;
            }

            DataGridFiltrado.DataSource = new BindingSource { DataSource = filteredData };
            DataGridFiltrado.Visible = true;

            ReplaceString2(7, "-999", "NAN", 1);
            // Eliminar la columna
            DataGridFiltrado.Columns.RemoveAt(0);
            EreaseMainColumn("RollAngle");
            // Ocultar el DataGridView original
            dataGridProject3.Visible = false;
        }
        private void RemoveFilterBoxes()
        {
            // Remove all controls of type CheckedListBox or Button from the DataGridView
            foreach (Control control in dataGridProject3.Controls)
            {
                if (control is CheckedListBox || control is Button)
                {
                    control.Dispose();
                }
            }
        }



        private void AddArrowToColumnHeaders(bool showArrows)
        {
            dataGridProject3.ColumnHeadersVisible = true;

            foreach (DataGridViewColumn column in dataGridProject3.Columns)
            {
                string headerText = column.HeaderText;

                // Check if the headerText contains an arrow and get the name without the arrow for comparison
                string columnNameToCompare = headerText.EndsWith(" ⬇")
                    ? headerText.Substring(0, headerText.Length - 2)  // Eliminar " ⬇"
                    : headerText;  // Mantener el nombre sin cambiar

                // Check if the name without the arrow exists in the dictionary
                if (originalColumnNames.ContainsKey(columnNameToCompare))
                {
                    var columnInfo = originalColumnNames[columnNameToCompare];

                    // If arrows should be displayed, show the name with the arrow
                    if (showArrows)
                    {
                        column.HeaderCell.Value = columnInfo.NameWithArrow;  // Display the name with the arrow
                    }
                    else
                    {
                        column.HeaderCell.Value = columnInfo.OriginalName;  // Display only the original name
                    }
                }
                else
                {
                    // If it does not exist in the dictionary, keep the current value (unchanged)
                    column.HeaderCell.Value = headerText;
                }
            }

            // Force the view to refresh
            dataGridProject3.Refresh();
        }
        private void RemoveFilterControls(string columna)
        {
            // Check if there are any filter controls associated with the given column
            if (filterControls.ContainsKey(columna))
            {
                // Get the list of controls (such as TextBox, ComboBox) for this column
                var controlsToRemove = filterControls[columna];

                // Remove the control from the DataGridView's control collection
                foreach (var control in controlsToRemove)
                {
                    dataGridProject3.Controls.Remove(control);
                    control.Dispose();
                }

                // Remove the entry from the filterControls dictionary
                filterControls.Remove(columna);
            }
        }
        private void ResetForm()
        {
            
            FilteredValues.Visible = false;
            Return_Btn.Visible = false;
            DataGridFiltrado.Visible = false;
            dataGridProject3.Visible = true;
            dataGridProject3.DataSource = null;
            dataGridProject3.DataSource = ListFilteredPlanes;
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(data => data.time_sec).ToList();
            

            // Refrescar el DataGrid
            dataGridProject3.Refresh();
            ReplaceString2(7, "-999", "NAN", 0);
            // Eliminar la columna
            dataGridProject3.Columns.RemoveAt(0);
            EreaseMainColumn("RollAngle");
        }

        private void FilteredValues_Click(object sender, EventArgs e)
        {
            ApplyFilter();
            Return_Btn.Visible = true;
        }

        private void Return_Btn_Click(object sender, EventArgs e)
        {
            ResetForm();
        }


        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            // Lista global para almacenar puntos de inicio de viraje
            var turnStartPoints = new List<TurnStartPoint>();
            List<string> turnStartPoints_list = new List<string>(); 

            // Conjunto para controlar los vuelos ya procesados
            var processedFlights = new HashSet<string>();
            var processedFlights_SID = new HashSet<string>();

            // Pasar del form principal llistes --> Roll Angle, True Track Angle, heading, posicion (lat,lon) i altitud. 
            ListFilteredPlanes = InterpolateData(ListFilteredPlanes);

            // 2. Calcular el inicio del viraje para cada avión
            foreach (var planeData in ListFilteredPlanes)
            {
                if (planeData.TrackAngleRate != -999 && planeData.RollAngle != -999 && planeData.Heading != -999 && planeData.TakeoffRWY == "LEBL-24L")
                {
                    // Se pasa la lista y el conjunto a la función
                    var turnStart = CalculateTurnStart(planeData, processedFlights, turnStartPoints);
                    if (turnStart != null)
                    {
                        turnStartPoints.Add(turnStart); // Agregar el punto a la lista si es válido
                        turnStartPoints_list.Add(Convert.ToString(turnStart.FlightId));
                        turnStartPoints_list.Add(Convert.ToString(turnStart.Latitude));
                        turnStartPoints_list.Add(Convert.ToString(turnStart.Longitude));
                        turnStartPoints_list.Add(Convert.ToString(turnStart.Altitude));
                        turnStartPoints_list.Add(Convert.ToString(turnStart.Radial));
                    }
                }
            }

            //MessageBox.Show(Convert.ToString(turnStartPoints.Count));
            //MessageBox.Show(Convert.ToString(turnStartPoints_list.Count/5));


            // 3. Calcular estadísticas de posición, altitud y radial
            var stats = CalculateStatistics(turnStartPoints);

            // 4. Mostrar resultados
            Debug.WriteLine("Estadísticas de posición y altitud:");
            Debug.WriteLine($"Latitud promedio: {stats.AverageLat}");
            Debug.WriteLine($"Longitud promedio: {stats.AverageLon}");
            Debug.WriteLine($"Altitud promedio: {stats.AverageAltitude}");
            Debug.WriteLine($"Radial promedio del DVOR BCN: {stats.AverageRadial}");

            List<string> statsList = new List<string>();
            statsList.Add(Convert.ToString(stats.AverageLat));
            statsList.Add(Convert.ToString(stats.AverageLon));
            statsList.Add(Convert.ToString(stats.AverageAltitude));
            statsList.Add(Convert.ToString(stats.AverageRadial));

            List<string> sidCompilantList = new List<string>();
            // Comprobar si se cumplen las condiciones de la SID
            foreach (var point in turnStartPoints)
            {
                bool sidCompliant = CheckSIDCompliance(point, processedFlights_SID);
                sidCompilantList.Add(sidCompliant ? "True" : "False");
            }

            // 5. Obtenir fitxer KML
            GetKML(ListFilteredPlanes);
            GetKML2(ListFilteredPlanes);

            Viraje formViraje = new Viraje(turnStartPoints_list, sidCompilantList, statsList);
            formViraje.Show();


            // Nos quedamos con los aviones que hacen departure por RWY 24L 

            // interpolar valors RA i TTA amb Heading

            // trobar lat i lon en el moment en que es segueix la condicio de l'angle --> tambe calcular alçada

            // calcul radial des del punt inici viratge al DVOR --> veure SID segons AIP

            // calcul estadisitiques --> segueixen SID?
        }

        static List<PlaneFilter> InterpolateData(List<PlaneFilter> originalData)
        {
            var interpolatedData = new List<PlaneFilter>();

            for (int i = 0; i < originalData.Count - 1; i++)
            {
                var current = originalData[i];
                var next = originalData[i + 1];

                // Intervalo de tiempo entre puntos consecutivos
                double timeDiff = next.time_sec - current.time_sec; 

                // Agregar el punto actual
                interpolatedData.Add(current);

                // Si el intervalo es mayor que la frecuencia deseada (4 segundos), interpolar
                if (timeDiff > 4)
                {
                    int steps = (int)(timeDiff / 4); // Número de pasos de 4 segundos a interpolar
                    for (int j = 1; j < steps; j++)
                    {
                        double ratio = (4.0 * j) / timeDiff;

                        // Crear un nuevo punto interpolado
                        interpolatedData.Add(new PlaneFilter
                        {
                            time_sec = current.time_sec + 4 * j, // Sumar los segundos directamente
                            RollAngle = current.RollAngle + ratio * (next.RollAngle - current.RollAngle),
                            TrueTrackAngle = current.TrueTrackAngle + ratio * (next.TrueTrackAngle - current.TrueTrackAngle),
                            Heading = current.Heading + ratio * (next.Heading - current.Heading),
                            Lat = current.Lat + ratio * (next.Lat - current.Lat),
                            Lon = current.Lon + ratio * (next.Lon - current.Lon),
                            Altitude = current.Altitude + ratio * (next.Altitude - current.Altitude)
                        });
                    }
                }
            }

            // Agregar el último punto
            interpolatedData.Add(originalData.Last());

            return interpolatedData;
        }

        static TurnStartPoint CalculateTurnStart(PlaneFilter flightData, HashSet<string> processedFlights, List<TurnStartPoint> list)
        {
            // Verificar si el vuelo ya ha sido procesado
            if (processedFlights.Contains(flightData.AircraftID))
                return null; // Ignorar vuelos ya detectados

            // Coordenadas iniciales (posición alineada con RWY 24L)
            const double initialHeading = 240.0;      // Rumbo inicial aproximado en grados
            const double rollAngleThreshold = 3.0;    // Umbral para detectar inicio de viraje (RollAngle)
            const double headingChangeThreshold = 3.0; // Umbral para detectar cambios en el Heading

            // Obtener valores actuales del avión
            double currentHeading = Convert.ToDouble(flightData.MagneticHeading);
            double currentRollAngle = Convert.ToDouble(flightData.RollAngle);
            double currentLatitude = Convert.ToDouble(flightData.Lat);
            double currentLongitude = Convert.ToDouble(flightData.Lon);
            double currentAltitude = Convert.ToDouble(flightData.Altitude);

            // Restricciones de altitud (según SID: ejemplo ficticio)
            const double minimumAltitude = 500; // Altitud mínima antes de realizar el viraje
            const double maximumAltitude = 3000; // Altitud máxima durante el viraje

            // Detectar si el avión está en el punto de inicio del viraje
            bool isAlignedWithRunway = Math.Abs(currentHeading - initialHeading) <= headingChangeThreshold;
            bool rollStart = Math.Abs(currentRollAngle) > rollAngleThreshold;

            // Detectar si el avión cumple con las restricciones de altitud
            bool validAltitude = currentAltitude >= minimumAltitude && currentAltitude <= maximumAltitude;

            if ((rollStart || isAlignedWithRunway) && validAltitude)
            {
                // Cálculo del radial al DVOR BCN
                double radial = CalculateRadial(currentLatitude, currentLongitude);

                // Verificar el cumplimiento del radial según las SID (234° ± 2°)
                if (radial >= 232 && radial <= 236)
                {
                    // Crear el objeto TurnStartPoint
                    TurnStartPoint turnStart = new TurnStartPoint
                    {
                        FlightId = flightData.AircraftID,
                        Latitude = currentLatitude,
                        Longitude = currentLongitude,
                        Altitude = currentAltitude,
                        Radial = radial
                    };

                    // Marcar el vuelo como procesado
                    processedFlights.Add(flightData.AircraftID);

                    // Guardar el punto en la lista y retornarlo
                    //list.Add(turnStart);
                    return turnStart;
                }
            }

            // Si no hay viraje detectado o no cumple con las restricciones, retornar null
            return null;
        }



        static double CalculateRadial(double lat, double lon)
        {
            // Calcular el radial desde el DVOR BCN (41.307222,  2.107778)
            double dLat = lat - 41.307222;
            double dLon = lon - 2.107778;
            double angle = Math.Atan2(dLon, dLat) * (180 / Math.PI);
            return (angle + 360) % 360; // Normalizar a 0-360 grados
        }

        static Statistics CalculateStatistics(List<TurnStartPoint> points)
        {
            return new Statistics
            {
                AverageLat = points.Average(p => p.Latitude),
                AverageLon = points.Average(p => p.Longitude),
                AverageAltitude = points.Average(p => p.Altitude),
                AverageRadial = points.Average(p => p.Radial)
            };
        }



        //*********************** FALTA MIRAR SI EL QUE RETORNA ES EL QUE ES DEMANA
        static bool CheckSIDCompliance(TurnStartPoint point, HashSet<string> processedFlights)
        {
            // Coordenadas del DVOR BCN (centro del círculo)
            const double DVOR_Lat = 41.307111; // 41°18’25.6”N en formato decimal
            const double DVOR_Lon = 2.107806;  // 002°06’28.1”E en formato decimal

            // Coordenadas del punto en la costa (para calcular el radio)
            const double Coast_Lat = 41.268167; // 41º16´05.4”N en formato decimal
            const double Coast_Lon = 2.033333;  // 002º02´00.0”E en formato decimal

            // Calcular el radio del círculo (distancia entre DVOR y punto en la costa)
            double radius = CalculateDistance(DVOR_Lat, DVOR_Lon, Coast_Lat, Coast_Lon);

            // Verificar si el vuelo ya ha sido procesado
            if (processedFlights.Contains(point.FlightId))
            {
                return false; // Si ya fue procesado, no cumple con la SID
            }

            // Calcular la distancia del punto al DVOR BCN (centro del círculo)
            double distanceToDVOR = CalculateDistance(DVOR_Lat, DVOR_Lon, point.Latitude, point.Longitude);

            // Comprobar si el punto cumple con las restricciones de posición y altitud
            bool isCompliant = distanceToDVOR <= radius && // Dentro del círculo
                               point.Altitude >= 500 &&    // Altitud mínima
                               point.Altitude <= 3000;     // Altitud máxima

            // Si cumple con las condiciones, se marca como procesado
            if (isCompliant)
            {
                processedFlights.Add(point.FlightId);
            }

            return isCompliant;
        }

        // Función para calcular la distancia entre dos puntos geográficos (en kilómetros)
        static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadius = 6371.0; // Radio de la Tierra en kilómetros

            // Convertir coordenadas a radianes
            double lat1Rad = ToRadians(lat1);
            double lon1Rad = ToRadians(lon1);
            double lat2Rad = ToRadians(lat2);
            double lon2Rad = ToRadians(lon2);

            // Fórmula de haversine
            double deltaLat = lat2Rad - lat1Rad;
            double deltaLon = lon2Rad - lon1Rad;
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadius * c; // Distancia en kilómetros
        }

        // Función auxiliar para convertir grados a radianes
        static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        // Clases de apoyo
        class FlightData
        {
            public string FlightId { get; set; }
            public DateTime Timestamp { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Altitude { get; set; }
            public double Heading { get; set; }
            public double RollAngle { get; set; }
            public double TrueTrackAngle { get; set; }
        }

        class TurnStartPoint
        {
            public string FlightId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Altitude { get; set; }
            public double Radial { get; set; }
        }
        public int CountTakeoffRWY(List<PlaneFilter> planes, string rwy)
        {
            int count = 0;
            for (int i = 0; i < planes.Count; i++)
            { 
                if (planes[i].TakeoffRWY == rwy)
                {
                    count++;
                }
            }
            return count;
        }
        private void BTNCountRight_Click(object sender, EventArgs e)
        {
            LBLNumRight.Text = "Total LEBL-06R takeoffs="+CountTakeoffRWY(ListFilteredPlanes, "LEBL-06R");
        }

        private void BTNCountLeft_Click(object sender, EventArgs e)
        {
            LBLNumLeft.Text = "Total LEBL-24L takeoffs=" + CountTakeoffRWY(ListFilteredPlanes, "LEBEL-24L");
        }

        class Statistics
        {
            public double AverageLat { get; set; }
            public double AverageLon { get; set; }
            public double AverageAltitude { get; set; }
            public double AverageRadial { get; set; }
        }

        // Method to generate a KML file ******************* NO ESTA BE
        static void GetKML(List<PlaneFilter> originalData)
        {
            // Initialize a SaveFileDialog to allow the user to choose where to save the KML file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivo KML|*.kml";
            saveFileDialog.Title = "Guardar archivo KML";


            DialogResult result = saveFileDialog.ShowDialog();

            // If the user selects a file path and clicks OK
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName; // Saves the file name 

                // Dictionary to store aircraft data with the name of the aircraft as the key
                Dictionary<string, KML_DATA> posicionesDeRepeticiones = new Dictionary<string, KML_DATA>();

                // Iterate over each item in the "bloque" list (aircraft data)
                for (int i = 0; i < originalData.Count; i++)
                {
                    if (originalData[i].TakeoffRWY == "LEBL-24L")
                    {
                        string nombre = originalData[i].AircraftID;
                        if (!posicionesDeRepeticiones.ContainsKey(nombre))
                        {
                            posicionesDeRepeticiones[nombre] = new KML_DATA();
                            posicionesDeRepeticiones[nombre].Positions = new List<Vector>();
                            posicionesDeRepeticiones[nombre].Description = "Aircraft address: " + nombre + " ; Aircraft indentification: " + originalData[i].AircraftID + " ; Track number: " + originalData[i].TrackNum;
                        }
                        posicionesDeRepeticiones[nombre].Positions.Add(new Vector(originalData[i].Lat, originalData[i].Lon, originalData[i].Altitude));
                    }
                }

                // Create the KML document and KML object
                var document = new Document();
                var kml = new Kml();

                int styleCount = 0; // Counter to create unique style IDs for each aircraft
                // Loop through the dictionary to create KML elements for each aircraft
                foreach (var kvp in posicionesDeRepeticiones)
                {
                    string nombreAeronave = kvp.Key;
                    string description = kvp.Value.Description;

                    var placemark = new SharpKml.Dom.Placemark();
                    placemark.Name = nombreAeronave;
                    placemark.Description = new Description { Text = description };

                    // Create a custom style for each placemark
                    var style = new Style();
                    style.Id = "Style" + styleCount;
                    style.Line = new LineStyle
                    {
                        Color = new Color32(255, 0, 0, 255),
                        Width = 0.5
                    };

                    placemark.StyleUrl = new Uri("#" + style.Id, UriKind.Relative); // Link the style to the placemark

                    // Create a LineString geometry (path of the aircraft's trajectory)
                    var lineString = new LineString();
                    lineString.Coordinates = new CoordinateCollection();

                    var point = new SharpKml.Dom.Point();

                    // Iterate through the positions of the aircraft and add them as coordinates in the LineString
                    foreach (Vector posicion in kvp.Value.Positions)
                    {
                        lineString.Coordinates.Add(posicion);
                        if (posicion.Altitude <= 1828.8)
                        {
                            lineString.AltitudeMode = AltitudeMode.RelativeToGround;
                        }
                        else
                        {
                            lineString.AltitudeMode = AltitudeMode.Absolute;
                        }

                    }

                    placemark.Geometry = lineString;

                    // Add the custom style to the KML
                    document.AddStyle(style);
                    document.AddFeature(placemark);

                    styleCount++;
                }

                Style lineStyle = new Style();
                lineStyle.Line = new LineStyle();
                lineStyle.Line.Width = 5;
                lineStyle.Line.Color = new Color32(0, 255, 0, 255);

                document.AddStyle(lineStyle);

                // Add the document (with all the placemarks) to the KML object
                kml.Feature = document;

                // Create a KML file from the document object
                KmlFile kmlFile = KmlFile.Create(kml, false);

                // Save file to a memory stream
                MemoryStream memStream = new MemoryStream();
                kmlFile.Save(memStream);

                // Write the KML data from the memory stream to the file
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.CopyTo(fileStream);
                }
            }
        }

        private class KML_DATA
        {
            public List<Vector> Positions { get; set; }
            public string Description { get; set; }
        }

        static void GetKML2(List<PlaneFilter> originalData)
        {

            // Coordenadas del DVOR BCN (centro del círculo)
            const double DVOR_Lat = 41.307111; // 41°18’25.6”N en formato decimal
            const double DVOR_Lon = 2.107806;  // 002°06’28.1”E en formato decimal

            // Coordenadas del punto en la costa (para calcular el radio)
            const double Coast_Lat = 41.268167; // 41º16´05.4”N en formato decimal
            const double Coast_Lon = 2.033333;  // 002º02´00.0”E en formato decimal

            // Calcular el radio del círculo (distancia entre DVOR y punto en la costa)
            double radius = CalculateDistance(DVOR_Lat, DVOR_Lon, Coast_Lat, Coast_Lon);

            // Initialize a SaveFileDialog to allow the user to choose where to save the KML file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivo KML|*.kml";
            saveFileDialog.Title = "Guardar archivo KML";

            DialogResult result = saveFileDialog.ShowDialog();

            // If the user selects a file path and clicks OK
            if (result == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName; // Saves the file name 

                // Dictionary to store aircraft data with the name of the aircraft as the key
                Dictionary<string, KML_DATA2> posicionesDeRepeticiones = new Dictionary<string, KML_DATA2>();

                // Iterate over each item in the "bloque" list (aircraft data)
                for (int i = 0; i < originalData.Count; i++)
                {
                    if (originalData[i].TakeoffRWY == "LEBL-24L")
                    {
                        string nombre = originalData[i].AircraftID;
                        if (!posicionesDeRepeticiones.ContainsKey(nombre))
                        {
                            posicionesDeRepeticiones[nombre] = new KML_DATA2();
                            posicionesDeRepeticiones[nombre].Positions = new List<Vector>();
                            posicionesDeRepeticiones[nombre].Description = "Aircraft address: " + nombre + " ; Aircraft indentification: " + originalData[i].AircraftID + " ; Track number: " + originalData[i].TrackNum;
                        }
                        // Filtrar las posiciones que están dentro del círculo
                        if (IsWithinCircle(DVOR_Lat, DVOR_Lon, radius, originalData[i].Lat, originalData[i].Lon))
                        {
                            posicionesDeRepeticiones[nombre].Positions.Add(new Vector(originalData[i].Lat, originalData[i].Lon, originalData[i].Altitude));
                        }
                    }
                }

                // Create the KML document and KML object
                var document = new Document();
                var kml = new Kml();

                

                int styleCount = 0; // Counter to create unique style IDs for each aircraft
                // Loop through the dictionary to create KML elements for each aircraft
                foreach (var kvp in posicionesDeRepeticiones)
                {
                    string nombreAeronave = kvp.Key;
                    string description = kvp.Value.Description;

                    var placemark = new SharpKml.Dom.Placemark();
                    placemark.Name = nombreAeronave;
                    placemark.Description = new Description { Text = description };

                    // Create a custom style for each placemark
                    var style = new Style();
                    style.Id = "Style" + styleCount;
                    style.Line = new LineStyle
                    {
                        Color = new Color32(255, 0, 0, 255),
                        Width = 0.5
                    };

                    placemark.StyleUrl = new Uri("#" + style.Id, UriKind.Relative); // Link the style to the placemark

                    // Create a LineString geometry (path of the aircraft's trajectory)
                    var lineString = new LineString();
                    lineString.Coordinates = new CoordinateCollection();

                    var point = new SharpKml.Dom.Point();

                    // Iterate through the positions of the aircraft and add them as coordinates in the LineString
                    foreach (Vector posicion in kvp.Value.Positions)
                    {
                        lineString.Coordinates.Add(posicion);
                        if (posicion.Altitude <= 1828.8)
                        {
                            lineString.AltitudeMode = AltitudeMode.RelativeToGround;
                        }
                        else
                        {
                            lineString.AltitudeMode = AltitudeMode.Absolute;
                        }

                    }

                    placemark.Geometry = lineString;

                    // Add the custom style to the KML
                    document.AddStyle(style);
                    document.AddFeature(placemark);

                    styleCount++;
                }

                Style lineStyle = new Style();
                lineStyle.Line = new LineStyle();
                lineStyle.Line.Width = 5;
                lineStyle.Line.Color = new Color32(0, 255, 0, 255);

                document.AddStyle(lineStyle);
                /*   FALTA ACABAR DIBUIXAR CERCLE 2 KML :) 
                // **Añadir el círculo** que representa el área del SID
                var circlePlacemark = new SharpKml.Dom.Placemark();
                circlePlacemark.Name = "Área del SID - Círculo de restricción";

                var circleStyle = new Style();
                circleStyle.Id = "CircleStyle";
                circleStyle.Line = new LineStyle
                {
                    Color = new Color32(0, 0, 255, 255),
                    Width = 2
                };
                circleStyle.Polygon = new PolygonStyle
                {
                    Color = new Color32(0, 0, 255, 128) // Azul semi-transparente
                };
                circlePlacemark.StyleUrl = new Uri("#" + circleStyle.Id, UriKind.Relative);

                // Círculo alrededor del DVOR
                var circle = new Polygon();
                var ring = new LinearRing();
                double step = 360 / 360; // Esto genera 360 puntos para formar un círculo

                for (int i = 0; i < 360; i++)
                {
                    double angle = i * step;
                    double lat = DVOR_Lat + (radius / 111.32) * Math.Cos(angle * Math.PI / 180); // Aproximación para latitudes
                    double lon = DVOR_Lon + (radius / 111.32) * Math.Sin(angle * Math.PI / 180) / Math.Cos(DVOR_Lat * Math.PI / 180); // Aproximación para longitudes
                    ring.Coordinates.Add(new Vector(lat, lon, 0)); // Círculo en el plano horizontal
                }

                // Crear un OuterBoundary y asignarle el LinearRing
                var outerBoundary = new OuterBoundary();
                outerBoundary.LinearRing = ring;

                // Asignamos el OuterBoundary al polígono
                circle.OuterBoundary = outerBoundary;

                circlePlacemark.Geometry = circle;

                // Add the circle Placemark to the KML document
                document.AddStyle(circleStyle);
                document.AddFeature(circlePlacemark);
                */

                // Add the document (with all the placemarks) to the KML object
                kml.Feature = document;

                // Create a KML file from the document object
                KmlFile kmlFile = KmlFile.Create(kml, false);

                // Save file to a memory stream
                MemoryStream memStream = new MemoryStream();
                kmlFile.Save(memStream);

                // Write the KML data from the memory stream to the file
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    memStream.CopyTo(fileStream);
                }
            }
        }

        // Función para verificar si un punto está dentro del círculo
        static bool IsWithinCircle(double centerLat, double centerLon, double radius, double pointLat, double pointLon)
        {
            double distance = CalculateDistance(centerLat, centerLon, pointLat, pointLon);
            return distance <= radius;
        }

        private class KML_DATA2
        {
            public List<Vector> Positions { get; set; }
            public string Description { get; set; }
        }


    }
}
