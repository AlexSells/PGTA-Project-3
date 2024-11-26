using LibAsterix;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsAsterix
{
    public partial class Projecte3 : Form
    {
        internal int totalPlanes = 1;
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
            int auxID = ListFilteredPlanes[0].ID; // ID con la que trabajamos
            bool first = true; // canviara al encontarr una ID distinta
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
                if (auxID != ListFilteredPlanes[i].ID)
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
                        if (first) //comprovamos que seguimos el orden correcto
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
                            double auxSecondsBack = ListFilteredPlanes[j+1].time_sec - ListFilteredPlanes[i].time_sec;
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
                        if (ClasPlanes.ContainsKey(ListFilteredPlanes[i].AircraftType))
                        {
                            ClassBack = ClasPlanes[ListFilteredPlanes[i].AircraftType];
                        }
                        string SameSIDBack = "";
                        if (ClasSID.ContainsKey(ListFilteredPlanes[i].TakeoffProcess))
                        {
                            SameSIDBack = ClasSID[ListFilteredPlanes[i].TakeoffProcess];
                        }
                        bool SameSID = SameIDCheck(SameSIDFront, SameSIDBack);

                        bool boolSID = SameSIDFront == SameSIDBack ? true : false;
                        auxSegimiento = j++;

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
            if (filePath != "")
            {
                string lastFile = Path.GetFileName(filePath);
                MessageBox.Show(lastFile);
                if (lastFile == auxFile)
                {
                    Classifier(filePath, ref dict);
                    aux = true;
                }
                else
                {
                    MessageBox.Show("Error loading the message");
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
        /*### EVENTS ############################################################################################################*/
        
        private void Back2P2Btn_Click(object sender, EventArgs e)
        {

        }

        private void LoadTableBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_Clasificacion_aeronaves.xlsx", ref ClasPlanes);
            if (aux == true) { ListDistanceCSV = FindDistances(); }
        }

        private void LoadSID06RBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_misma_SID_06R.xlsx", ref ClasSID);
            if (aux == true) { ListDistanceCSV = FindDistances(); }
        }

        private void LoadSID24LBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_misma_SID_24L.xlsx", ref ClasSID);
            if (aux == true) { ListDistanceCSV = FindDistances(); }
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
        private void DistanceCSVBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "File CSV| *.csv";
            saveFileDialog.Title = "Save CSV file";
            saveFileDialog.InitialDirectory = @"C:\";

            //ListDistanceCSV = FindDistances();

            // Muestra que el archiva se ha guardado correctamente
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                List<(string planeFront, string planeBack, int totalRadar, int totalEstela, int totalLOA)> InfringementCSV = new List<(string planeFront, string planeBack, int totalRadar, int totalEstela, int totalLOA)>();
                string filePath = saveFileDialog.FileName;
                StringBuilder sbCSV = new StringBuilder();

                string auxDatos;

                bool auxDetection = true;
                string auxPlaneFront = ListDistanceCSV[0].PlaneFront;
                string auxPlaneBack = ListDistanceCSV[0].PlaneAfter;

                // Preparemos las cabeceras
                sbCSV.AppendLine("Plane 1= Type_plane 1=Estela 1=Clasification 1=SID 1=Time_1=Plane 2= Type_plane 2=Estela 2=Clasification 2=SID 2=Time_2=Interval time (s)=Delta_U (NM)=Delta_V (NM)=Distance_between (NM)=Minima_radar=Minima_Estela=Minima_LoA=Total of both= Total estela= Radar= Estela = LoA ");

                for (int i = 1; i < ListDistanceCSV.Count; i++)
                {
                    var aux = ListDistanceCSV[i];
                    bool MinRadar = true;
                    bool MinEstela = true;
                    bool MinLoA = true;

                    numPlanesTotal++;
                    TotalMessageComparation++;
                    // Comprovamos si se comple la distancia minima de radar
                    if (aux.DistanceDiff <= 3)
                    {
                        MinRadar = false;
                        TotalRadarIncidents++;

                    }
                    // Comprovamos si se comple la distancia minima de LoA
                    if (LoA.ContainsKey((aux.ClassFront, aux.ClassAfter, aux.SameSID)))
                    {
                        if (aux.DistanceDiff <= LoA[(aux.ClassFront, aux.ClassAfter, aux.SameSID)])
                        {
                            MinLoA = false;
                            TotalLoAIncidents++;
                        }
                    }
                    // Comprovamos si se comple la distancia minima de estela
                    if (Estelas.ContainsKey((aux.EstelaFront, aux.EstelaAfter)))
                    {
                        countEstela++;
                        TotalEstaleComparationMessages++;

                        if (aux.DistanceDiff <= Estelas[(aux.EstelaFront, aux.EstelaAfter)])
                        {
                            MinEstela = false;
                            numPlanesEstela++;
                            TotalEstelaIncidents++;
                        }
                        if ((i + 1) < ListDistanceCSV.Count && auxPlaneFront != ListDistanceCSV[i + 1].PlaneFront)
                        {
                            auxDatos = $"={Convert.ToString(numPlanesTotal)}={Convert.ToString(countEstela)}={Convert.ToString(numPlanesRadar)}={Convert.ToString(numPlanesEstela)}={Convert.ToString(numPlanesLOA)}";
                            if ((numPlanesRadar != 0 || numPlanesEstela != 0 || numPlanesLOA != 0))
                            {
                                if (!auxDetection)
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
                                }
                                auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                            }
                            auxPlaneFront = ListDistanceCSV[i + 1].PlaneFront;
                            ClearNumAux();
                        }
                        else { auxDatos = ""; }
                        if (aux.PlaneFront != aux.PlaneAfter)
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
                                if (!auxDetection)
                                {
                                    if (auxPlaneBack == ListDistanceCSV[i].PlaneFront) { TotalIncidencePlanes++; }
                                    else
                                    {
                                        //auxDetection = false;
                                        TotalIncidencePlanes = TotalIncidencePlanes + 2;
                                    }
                                    auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                                }
                                else
                                {
                                    auxDetection = false;
                                    auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                                    TotalIncidencePlanes = TotalIncidencePlanes + 2;
                                }
                                auxPlaneBack = ListDistanceCSV[i].PlaneAfter;
                            }
                            auxPlaneFront = ListDistanceCSV[i + 1].PlaneFront;
                            ClearNumAux();

                        }
                        else { auxDatos = ""; }
                        //sbCSV.AppendLine("Plane 1= Type_plane 1=Estela 1=Clasification 1=SID 1=Time_1=Plane 2= Type_plane 2=Estela 2=Clasification 2=SID 2=Time_2=Interval time (s)=Delta_U (NM)=Delta_V (NM)=Distance_between (NM)=Minima_radar=Minima_Estela=Minima_LoA=Total of both= Total estela= Radar= Estela = LoA ");
                        if (aux.PlaneFront != aux.PlaneAfter){
                            string data = $"{aux.PlaneFront}={aux.AircraftTypeFront}={aux.EstelaFront}={aux.ClassFront}={aux.SIDfront}={Convert.ToString(aux.time_front)}={aux.PlaneAfter}={aux.AircraftTypeBack}={aux.EstelaAfter}={aux.ClassAfter}={aux.SIDback}={Convert.ToString(aux.time_back)}={Convert.ToString(aux.secondsDiff)}={Convert.ToString(aux.U)}={Convert.ToString(aux.V)}={Convert.ToString(aux.DistanceDiff)}={TotalRadarIncidents}={TotalEstaleComparationMessages}={MinRadar}= N/A ={MinLoA}" + auxDatos;
                            sbCSV.AppendLine(data);
                        }
                        
                        //string data = $"{aux.PlaneFront}={aux.AircraftTypeFront}={aux.EstelaFront}={aux.ClassFront}={aux.SIDfront}={Convert.ToString(aux.time_front)}={aux.PlaneAfter}={aux.AircraftTypeBack}={aux.EstelaAfter}={aux.ClassAfter}={aux.SIDback}={Convert.ToString(aux.time_back)}={Convert.ToString(aux.secondsDiff)}={Convert.ToString(aux.U)}={Convert.ToString(aux.V)}={Convert.ToString(aux.DistanceDiff)}={TotalRadarIncidents}={TotalEstaleComparationMessages}={MinRadar}= N/A ={MinLoA}" + auxDatos;
                        
                    }
                }

                ANSTotalComparision.Text = $"Total messages = {TotalMessageComparation}";
                ANSIncidentPlanes.Text = $"Total planes with incidents = {TotalIncidencePlanes}";
                ANSTotalEstelaComparations.Text = $"Total messages estela = {TotalEstaleComparationMessages}";
                ANSTotalRadar.Text = $"Total radar incidents = {TotalRadarIncidents}";
                ANSTotalEstela.Text = $"Total estela incidents = {TotalEstelaIncidents}";
                ANSTotalLoA.Text = $"Total LoA incidents = {TotalLoAIncidents}";

                File.WriteAllText(filePath, sbCSV.ToString());
                MessageBox.Show("CSV file generated");
            }
            else
            {
                MessageBox.Show("CSV file generation failed");
            }
        }

        private void Projecte3_Load(object sender, EventArgs e)
        {
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(data => data.time_sec).ToList();
            dataGridProject3.DataSource = ListFilteredPlanes;
        }
    }
}
