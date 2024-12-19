using Amazon.CloudWatchLogs;
using Amazon.SimpleDB.Model;
using Amazon.SimpleNotificationService.Model;
using LibAsterix;
using MultiCAT6.Utils;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting.Contracts;
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
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
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
            // Inicializamos el forms y ordenamos la lsita en funcion de su ID
            ListFilteredPlanes = FilteredList;
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(item => item.ID).ToList();
        }
        /*### LIST ############################################################################################################*/
        // Lista princial que sera usada en la mayoria del codigo. Contiene todos los datos entreados de P2
        List<PlaneFilter> ListFilteredPlanes;

        // Conten dra unicamente los datos necesarios para tgenerar el KML
        List<PlaneFilter> ListFilterKML;

        // Lista auxiliar de coordenadas U y V
        List<(int ID, double time, double U, double V)> ListUV;

        // Lista donde se guardarn los datos refernetes a las incidencias de los aviones
        private List<DistanceList> ListDistanceCSV;

        List<int> TMAIncidentes = new List<int>();
        List<int> TWRIncidentes = new List<int>();

        // Funcion que se encarga de crear una lista conn los datos necesarios para analizar una incidencia
        private List<DistanceList> FindDistances()
        {
            // lista auxiliar para no sobreescribir la lista inicial
            List<DistanceList> distances = new List<DistanceList>();
            totalPlanes = 1;
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(item => item.ID).ToList();

            // valore sauxilares para assitir al analisis de la lista
            var auxValList = ListFilteredPlanes[0];
            int auxID = ListFilteredPlanes[0].ID; // ID con la que trabajamos
            bool first = false; // canviara al encontarr una ID distinta
            int auxSegimiento = 1;


            for (int i = 0; i < ListFilteredPlanes.Count; i++)
            {
                // Establecemos los datos del avion avion analizado
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

                // contamos los aviones analizados solo si su ID no coincide con la anterior
                if (auxID != ListFilteredPlanes[i].ID) // && auxID < ListFilteredPlanes[i].ID)
                {
                    auxID = ListFilteredPlanes[i].ID;
                    totalPlanes++;
                    first = true;
                }

                // Buscamos el instante de tiempo donde dos aviones consecutivos estan más cerca
                // idealmente 0 aunque trabajaramos con un margen de hasta 4 segundos
                for (int j = auxSegimiento; j < ListFilteredPlanes.Count; j++)
                {
                    

                    // la diferencia de tiempo solo es valida si los aviones son consecutivos paara
                    // evitar analizar aviones no consecuitiovos y por ende no sujetos a las condiciones
                    // de separacion minima

                    if (ListFilteredPlanes[j].ID - ListFilteredPlanes[i].ID == 1)
                    {
                        double auxSecs = ListFilteredPlanes[j].time_sec - ListFilteredPlanes[i].time_sec;

                        if (Math.Abs(auxSecs) < 4)
                        {

                            double auxSecsBack = ListFilteredPlanes[j].time_sec - ListFilteredPlanes[i + 1].time_sec;

                            // comprovamos si el siguiente instante de tiempo del primer avion la diferencia
                            // en tiempo es menor
                            if (Math.Abs(auxSecs) > Math.Abs(auxSecsBack))
                            {
                                auxSecs = auxSecsBack;
                                i++;
                            }

                            if (auxSegimiento - j < 0)
                            {
                                // comprovamos si el siguiente instante de tiempo del segundo avion la diferencia
                                // en tiempo es menor
                                auxSecsBack = ListFilteredPlanes[j + 1].time_sec - ListFilteredPlanes[i].time_sec;
                                if (Math.Abs(auxSecs) > Math.Abs(auxSecsBack))
                                {
                                    auxSecs = auxSecsBack;
                                    j++;
                                }
                            }
                            
                            // TMA
                            double delta_U = Math.Abs(ListFilteredPlanes[i].U_tma - ListFilteredPlanes[j].U_tma);
                            double delta_V = Math.Abs(ListFilteredPlanes[i].V_tma - ListFilteredPlanes[j].V_tma);
                            // calculamos la sepraracion entre dos aviones usando sus valores sterograficos
                            double distanceDiff_TMA = Math.Sqrt(Math.Pow(delta_U, 2) + Math.Pow(delta_V, 2));

                            // TWR
                            double delta_U_twr = Math.Abs(ListFilteredPlanes[i].U_twr - ListFilteredPlanes[j].U_twr);
                            double delta_V_twr = Math.Abs(ListFilteredPlanes[i].V_twr - ListFilteredPlanes[j].V_twr);
                            // calculamos la sepraracion entre dos aviones usando sus valores sterograficos
                            double distanceDiff_twr = Math.Sqrt(Math.Pow(delta_U_twr, 2) + Math.Pow(delta_V_twr, 2));

                            /////////////////////////////////
                            CoordinatesUVH coordTHR = null;

                            coordTHR = UVCoordinates.GetUV(41.292219 * GeoUtils.DEGS2RADS, 2.103281 * GeoUtils.DEGS2RADS, 8 * GeoUtils.FEET2METERS, false);
                            double delta_U_thr = Math.Abs(ListFilteredPlanes[i].U_tma - coordTHR.U);
                            double delta_V_thr = Math.Abs(ListFilteredPlanes[i].V_tma - coordTHR.V);
                            double dist_thr = Math.Sqrt(Math.Pow(delta_U_thr, 2) + Math.Pow(delta_V_thr, 2));

                            
                            // repetimos el proceso con la formual de Haversine para comprovar que la distancia
                            // es la correcta (no se usa porque dan valores muy parecidos)
                            //double distDiff = IASCalculations.HaversineDistance(ListFilteredPlanes[i].Lat, ListFilteredPlanes[i].Lon, ListFilteredPlanes[j].Lat, ListFilteredPlanes[j].Lon);
                            

                            // En este caso consideramos ya al avion consecutivo como candidato a tener incidencia
                            // por ende llenamos los campos necesarios para su posterior analisis
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

                            // Comprovamos si los dos aviones tienen la misma SID
                            bool boolSID = SameSIDFront == SameSIDBack ? true : false;
                            auxSegimiento = j + 1;

                            // Añadimos los valores a la lista  
                            DistanceList dl = new DistanceList();
                            dl.ID = ListFilteredPlanes[i].ID;
                            dl.PlaneFront = ListFilteredPlanes[i].AircraftID;
                            dl.AircraftTypeFront = ListFilteredPlanes[i].AircraftType;
                            dl.EstelaFront = ListFilteredPlanes[i].EstelaType;
                            dl.ClassFront = ClassFront;
                            dl.SIDFront = SameSIDFront;
                            dl.time_front = ListFilteredPlanes[i].time_sec;

                            dl.PlaneBack = ListFilteredPlanes[j].AircraftID;
                            dl.AircraftTypeBack = ListFilteredPlanes[j].AircraftType;
                            dl.EstelaBack = ListFilteredPlanes[j].EstelaType;
                            dl.ClassBack = ClassBack;
                            dl.SIDBack = SameSIDBack;
                            dl.time_back = ListFilteredPlanes[j].time_sec;

                            dl.sameSID = SameSID;
                            dl.U = delta_U;
                            dl.V = delta_V;
                            dl.DistanceDiff_tma = distanceDiff_TMA;
                            dl.DistanceDiff_twr = distanceDiff_twr;
                            dl.secondsDiffs = auxSecs;
                            //Auxiliar data
                            dl.init_time_front = ListFilteredPlanes[i].init_takeoff;
                            dl.init_time_back = ListFilteredPlanes[j].init_takeoff;
                            dl.dist_thr = dist_thr;

                            distances.Add(dl);
                        }
                    }
                    
                }
            }

            /*for (int i = 0; i < ListFilteredPlanes.Count - 1; i++)
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
                        double distDiff = IASCalculations.HaversineDistance(ListFilteredPlanes[i].Lat, ListFilteredPlanes[i].Lon, ListFilteredPlanes[j].Lat, ListFilteredPlanes[j].Lon);
                        
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
                        
                        distances.Add((ListFilteredPlanes[i].ID,ListFilteredPlanes[i].AircraftID, ListFilteredPlanes[i].AircraftType, ListFilteredPlanes[i].EstelaType, ClassFront, SameSIDFront, ListFilteredPlanes[i].time_sec, ListFilteredPlanes[j].AircraftID, ListFilteredPlanes[i].AircraftType, ListFilteredPlanes[j].EstelaType, ClassBack, SameSIDBack, ListFilteredPlanes[j].time_sec, boolSID, delta_U, delta_V, distDiff, auxSeconds));
                        break;
                    }
                    else if (ListFilteredPlanes[j].ID - ListFilteredPlanes[i].ID > 1) { break; }
                }
            }*/

            // escribimos el total de aviones analizados 
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
        // Comprovamos si dos aviones consecutivos tienen la misma SID
        public bool SameIDCheck(string SameSIDFront, string SameSIDBack)
        {
            if (SameSIDFront == SameSIDBack)
            {
                return true;
            }
            return false;
        }
        // Clasificamos los aviones con los datos extraidos de los .xlsx caragdos
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

        // Codigo que genera el dialogo para abrir un archivo .xlsx cualquiera
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

        // Comprueva comprueva, carga un fichero y los clasifica
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
        // limpia variables lcoales
        public void ClearNumAux()
        {
            numPlanesEstela = 0;
            numPlanesLOA = 0;
            numPlanesRadar = 0;
            numPlanesTotal = 0;
            countEstela = 0;
        }
        // Comprueba si si se cumple la condicion minima de radar (solo una
        // por aviones consecutivos)
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
        // Comprueba si si se cumple la condicion minima de LoA (solo una
        // por aviones consecutivos)
        public bool CheckLoAminima(double DistDiff, string ClassFront, string ClassBack, bool SameSID)
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
        // Comprueba si si se cumple la condicion minima de Estela (solo una
        // por aviones consecutivos)
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
        // Contador de incidencia entre aviones (todas las incidencias detectadas)
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
        // Genera la lista CSV con las incidencias caluladas
        public void GetListDistanceCSV(bool aux)
        {
            if (aux == true) 
            { 
                ListDistanceCSV = FindDistances(); 
                loaded++;
                EnableBtn();
                MessageBox.Show("Fichero cargado correctamente");
            }
        }

        public void EnableBtn()
        {
            if (loaded >= 3)
            {
                DistanceCSVBtn.Enabled = true;
                DistanceCSVBtn.Visible = true;
                if (loaded > 3)
                {
                    GenStatisticsBtn.Enabled = true;
                    GenStatisticsBtn.Visible = true;   
                }
            }
        }
        /*### EVENTS ############################################################################################################*/
        // Vuelve al proyecto dos
        private void Back2P2Btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // Carga las tablas de classificacion
        private void LoadTableBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_Clasificacion_aeronaves.xlsx", ref ClasPlanes);
            GetListDistanceCSV(aux);
        }
        // Carga las SIDs 06R
        private void LoadSID06RBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_misma_SID_06R.xlsx", ref ClasSID);
            GetListDistanceCSV(aux);
        }
        // Carga las SIDs 24L
        private void LoadSID24LBtn_Click(object sender, EventArgs e)
        {
            bool aux = StartClassification("Tabla_misma_SID_24L.xlsx", ref ClasSID);
            GetListDistanceCSV(aux);
        }

        // Variables gloabales 
        int TotalIncidencePlanes = 0;
        int TotalMessageComparation = 0;
        int TotalEstaleComparationMessages = 0;
        int TotalRadarIncidents = 0;
        int TotalEstelaIncidents = 0;
        int TotalLoAIncidents = 0;
        public void CleanGlobalVar()
        {
            // Variables gloabales 
            TotalIncidencePlanes = 0;
            TotalMessageComparation = 0;
            TotalEstaleComparationMessages = 0;
            TotalRadarIncidents = 0;
            TotalEstelaIncidents = 0;
            TotalLoAIncidents = 0;
        }
        // Auxiliar variables (se borran con cada iteracion)
        int numPlanesTotal, numPlanesRadar, numPlanesEstela, numPlanesLOA, countEstela, numPlanesIncidence = 0, numPlanesComparision = 0;

        // Carga dos lista al form GeneralStatistics donde se mostraran datos estadisticos genericos
        private void GenStatisticsBtn_Click(object sender, EventArgs e)
        {

            GeneralStatistics GenStats = new GeneralStatistics(ListDistanceCSV, TMAIncidentes, TWRIncidentes);
            GenStats.Show();
        }
        // Clase que se empleara para tener un seguimiento de los aviones y que incidencias tienen cada uno
        private class PlaneDetections
        {
            public string AircraftID { get; set; }
            public double init_time { get; set; }
            public bool RadarDetected { get; set; }
            public bool EstelaDetected { get; set; }
            public bool LoADetected { get; set; }
        }

        bool isTWR=false;

        public void GenerateCSV()
        {
            //GenStatisticsBtn.Enabled = true;
            //GenStatisticsBtn.Visible = true;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "File CSV| *.csv";
            saveFileDialog.Title = "Save CSV file";
            saveFileDialog.InitialDirectory = @"C:\"; //Punto de inicio

            // Muestra que el archiva se ha guardado correctamente
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                List<PlaneDetections> list_plane = new List<PlaneDetections>();
                List<DistanceList> auxList = new List<DistanceList>();
                auxList = ListDistanceCSV.OrderBy(item => item.ID).ToList();

                List<(string planeFront, string planeBack, int totalRadar, int totalEstela, int totalLOA)> InfringementCSV = new List<(string planeFront, string planeBack, int totalRadar, int totalEstela, int totalLOA)>();
                string filePath = saveFileDialog.FileName;
                StringBuilder sbCSV = new StringBuilder();

                // Preparamos las cabeceras donde el delimitador de columna sera el signo =
                sbCSV.AppendLine("Plane 1; Type_plane 1;Estela 1;Clasification 1;SID 1;Time_1;Plane 2; Type_plane 2; Estela 2; Clasification 2; SID 2; Time_2; Interval time (s); Delta_U (NM); Delta_V (NM); Distance_between (NM); Minima_radar; Minima_Estela; Minima_LoA; Total of both; Total estela; Radar; Estela; LoA ");


                // NOU: AUXILIARS
                int auxID = auxList[0].ID;
                string auxData;
                string auxFront = auxList[0].PlaneFront;
                string auxBack = auxList[0].PlaneBack;


                bool auxDetec = true;

                string auxString = auxList[0].PlaneFront;

                for (int i = 0; i < auxList.Count; i++)
                {
                    var aux = auxList[i];
                    double dist = 999999;
                    if (isTWR == true)
                    {
                        dist = aux.DistanceDiff_twr;
                    }
                    else
                    {
                        dist = aux.DistanceDiff_tma;
                    }
                    bool MinRadar = true;
                    bool MinEstela = true;
                    bool MinLoA = true;
                    bool find = false;
                    int j = 0;
                    for (int k = j; k < list_plane.Count; k++)
                    {
                        if (list_plane[k].AircraftID == aux.PlaneFront)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (find == false)
                    {
                        PlaneDetections pd = new PlaneDetections();
                        pd.AircraftID = aux.PlaneFront;
                        string back = aux.PlaneBack;
                        pd.init_time = aux.init_time_front;
                        pd.RadarDetected = false;
                        pd.EstelaDetected = false;
                        pd.LoADetected = false;
                        j = list_plane.Count;
                        list_plane.Add(pd);
                        int c = list_plane.Count;
                    }

                    // Contamos el numero de aviones analizados y la comparacion de mensajes
                    numPlanesTotal++;
                    TotalMessageComparation++;

                    // Comprovamos si se comple la distancia minima de radar en NM (Nautical Miles)
                    if (dist <= 3.0 /*  *GeoUtils.NM2METERS*/)
                    {

                        MinRadar = false;
                        numPlanesRadar++;

                        if (aux.dist_thr > 0.5 * GeoUtils.NM2METERS && aux.init_time_back <= aux.time_front) //
                        {
                            if (list_plane.Count - 1 >= 0)
                            {
                                if (list_plane[list_plane.Count - 1].RadarDetected == false) //pd.AircraftID == aux.PlaneFront &&
                                {
                                    list_plane[list_plane.Count - 1].RadarDetected = true;
                                    TotalRadarIncidents = TotalRadarIncidents + 1;
                                }
                            }
                        }


                    }

                    // Comprovamos si se comple la distancia minima de LoA
                    if (LoA.ContainsKey((aux.ClassFront, aux.ClassBack, aux.sameSID)))
                    {
                        if (dist /* * GeoUtils.METERS2NM*/ <= LoA[(aux.ClassFront, aux.ClassBack, aux.sameSID)] && aux.init_time_back <= aux.time_front)
                        {

                            MinLoA = false;
                            numPlanesLOA++;

                            if (list_plane.Count - 1 >= 0)
                            {
                                if (list_plane[list_plane.Count - 1].LoADetected == false) // 
                                {
                                    list_plane[list_plane.Count - 1].LoADetected = true;
                                    TotalLoAIncidents = TotalLoAIncidents + 1;
                                }
                            }

                        }
                    }

                    // Comprovamos si se comple la distancia minima de estela
                    if (Estelas.ContainsKey((aux.EstelaFront, aux.EstelaBack)))
                    {
                        countEstela++;
                        TotalEstaleComparationMessages++;

                        // Comprovamos si se comple la distancia minima de LoA
                        if (dist/* * GeoUtils.METERS2NM*/ <= Estelas[(aux.EstelaFront, aux.EstelaBack)] && aux.init_time_back <= aux.time_front)
                        {
                            MinEstela = false;
                            numPlanesEstela++;

                            if (aux.init_time_front <= aux.time_front)
                            {
                                if (list_plane.Count - 1 >= 0)
                                {
                                    if (list_plane[j].EstelaDetected == false)
                                    {
                                        list_plane[j].EstelaDetected = true;
                                        TotalEstelaIncidents = TotalEstelaIncidents + 1;
                                    }
                                }
                            }

                        }
                        if ((i + 1) < ListDistanceCSV.Count && auxFront != ListDistanceCSV[i + 1].PlaneFront) //&& auxFront != ListDistanceCSV[i + 1].PlaneFront
                        {
                            auxData = $";{Convert.ToString(numPlanesTotal)};{Convert.ToString(countEstela)};{Convert.ToString(numPlanesRadar)};{Convert.ToString(numPlanesEstela)};{Convert.ToString(numPlanesLOA)}";
                            if ((numPlanesRadar != 0 || numPlanesEstela != 0 || numPlanesLOA != 0))
                            {
                                if (!auxDetec)
                                {
                                    if (auxBack == ListDistanceCSV[i].PlaneFront)
                                    {
                                        TotalIncidencePlanes = TotalIncidencePlanes + 1;
                                    }
                                    else
                                    {
                                        TotalIncidencePlanes = TotalIncidencePlanes + 2;
                                    }
                                    auxBack = ListDistanceCSV[i].PlaneBack;
                                }
                                else
                                {
                                    auxDetec = false;
                                    auxBack = ListDistanceCSV[i].PlaneBack;
                                    TotalIncidencePlanes = TotalIncidencePlanes + 2;
                                }
                            }
                            auxFront = ListDistanceCSV[i + 1].PlaneFront;
                            ClearNumAux();
                        }
                        else { auxData = ""; }
                        if (aux.PlaneFront != null) //aux.PlaneAfter)
                        {
                            string data = $"{aux.PlaneFront};{aux.AircraftTypeFront};{aux.EstelaFront};{aux.ClassFront};{aux.SIDFront};{Convert.ToString(aux.time_front)};{aux.PlaneBack};{aux.AircraftTypeBack};{aux.EstelaBack};{aux.ClassBack};{aux.SIDBack};{Convert.ToString(aux.time_back)};{Convert.ToString(aux.secondsDiffs)};{Convert.ToString(aux.U)};{Convert.ToString(aux.V)};{Convert.ToString(dist * GeoUtils.METERS2NM)};{TotalRadarIncidents};{TotalEstaleComparationMessages};{MinRadar}; N/A ;{MinLoA}" + auxData;
                            sbCSV.AppendLine(data);
                        }
                        bool booool = list_plane[0].RadarDetected;
                    }
                    else
                    {
                        if ((i + 1) < ListDistanceCSV.Count && auxFront != ListDistanceCSV[i + 1].PlaneFront) //&& auxFront != ListDistanceCSV[i + 1].PlaneFront
                        {
                            auxData = $"={Convert.ToString(numPlanesTotal)}={Convert.ToString(countEstela)}={Convert.ToString(numPlanesRadar)}={Convert.ToString(numPlanesEstela)}={Convert.ToString(numPlanesLOA)}";
                            if ((numPlanesRadar != 0 || numPlanesEstela != 0 || numPlanesLOA != 0))
                            {
                                if (!auxDetec)
                                {
                                    if (auxBack == ListDistanceCSV[i].PlaneFront)
                                    {
                                        TotalIncidencePlanes = TotalIncidencePlanes + 1;
                                    }
                                    else
                                    {
                                        TotalIncidencePlanes = TotalIncidencePlanes + 2;
                                    }
                                    auxBack = ListDistanceCSV[i].PlaneBack;
                                }
                                else
                                {
                                    auxDetec = false;
                                    auxBack = ListDistanceCSV[i].PlaneBack;
                                    TotalIncidencePlanes = TotalIncidencePlanes + 2;
                                }
                            }
                            auxFront = ListDistanceCSV[i + 1].PlaneFront;
                            ClearNumAux();

                        }
                        else { auxData = ""; }

                        //sbCSV.AppendLine("Plane 1= Type_plane 1=Estela 1=Clasification 1=SID 1=Time_1=Plane 2= Type_plane 2=Estela 2=Clasification 2=SID 2=Time_2=Interval time (s)=Delta_U (NM)=Delta_V (NM)=Distance_between (NM)=Minima_radar=Minima_Estela=Minima_LoA=Total of both= Total estela= Radar= Estela = LoA ");
                        if (aux.PlaneFront != null)
                        {//aux.PlaneAfter){
                            string data = $"{aux.PlaneFront};{aux.AircraftTypeFront};{aux.EstelaFront};{aux.ClassFront};{aux.SIDFront};{Convert.ToString(aux.time_front)};{aux.PlaneBack};{aux.AircraftTypeBack};{aux.EstelaBack};{aux.ClassBack};{aux.SIDBack};{Convert.ToString(aux.time_back)};{Convert.ToString(aux.secondsDiffs)};{Convert.ToString(aux.U)};{Convert.ToString(aux.V)};{Convert.ToString(dist * GeoUtils.METERS2NM)};{TotalRadarIncidents};{TotalEstaleComparationMessages};{MinRadar}; N/A ;{MinLoA}" + auxData;
                            sbCSV.AppendLine(data);
                        }
                    }
                }
                int estela = 0, radar = 0, loa = 0;
                for (int i = 0; i < list_plane.Count; i++)
                {
                    if (list_plane[i].RadarDetected == true)
                    {
                        radar = radar + 1;
                    }
                    if (list_plane[i].EstelaDetected == true)
                    {
                        string aba = list_plane[i].AircraftID;
                        estela = estela + 1;
                    }
                    if (list_plane[i].LoADetected == true)
                    {
                        loa = loa + 1;
                    }
                }
                TotalIncidencePlanes = list_plane.Count;
                TotalRadarIncidents = radar;
                TotalEstelaIncidents = estela;
                TotalLoAIncidents = loa;
                try
                {
                    File.WriteAllText(filePath, sbCSV.ToString());
                }
                catch
                {
                    MessageBox.Show("Se ha detectatdo un problema con el fichero, prueba otra vez");
                }

                loaded++;
                EnableBtn();
                MessageBox.Show("CSV file generated");
            }
            else { MessageBox.Show("CSV file generation failed"); }
        }

        // genera el csv con los incumplemientos de separacion minima
        private void DistanceCSVBtn_Click(object sender, EventArgs e)
        {
            CleanGlobalVar();
            GenerateCSV();
            TMAIncidentes.Add(TotalIncidencePlanes);
            TMAIncidentes.Add(TotalMessageComparation);
            TMAIncidentes.Add(TotalEstaleComparationMessages);
            TMAIncidentes.Add(TotalRadarIncidents);
            TMAIncidentes.Add(TotalEstelaIncidents);
            TMAIncidentes.Add(TotalLoAIncidents);
            isTWR = true;
            CleanGlobalVar();
            GenerateCSV();
            TWRIncidentes.Add(TotalIncidencePlanes);
            TWRIncidentes.Add(TotalMessageComparation);
            TWRIncidentes.Add(TotalEstaleComparationMessages);
            TWRIncidentes.Add(TotalRadarIncidents);
            TWRIncidentes.Add(TotalEstelaIncidents);
            TWRIncidentes.Add(TotalLoAIncidents);
            isTWR =false;
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
            // Carga el proyecto 3 
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(data => data.time_sec).ToList();
            dataGridProject3.RowHeadersDefaultCellStyle.Font = new Font(dataGridProject3.Font, FontStyle.Bold);
            dataGridProject3.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;
            dataGridProject3.DataSource = ListFilteredPlanes;
            // Substituye los valores -999 por NAN para mejorar la experiencia visual
            ReplaceString2(7, "-999", "NAN", 0);
            DistanceCSVBtn.Visible = false;
            DistanceCSVBtn.Enabled = false;
            GenStatisticsBtn.Visible = false;
            GenStatisticsBtn.Enabled = false;
            
        }
        int loaded = 0;
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
            var specificColumns = new List<string> { "Lat", "Lon", "Altitude" };
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


        double MagHead_in;
        double RA_in;
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            // Lista global para almacenar puntos de inicio de viraje

            var turnStartPoints = new List<TurnStartPoint>();
            List<string> turnStartPoints_list = new List<string>();

            // Conjunto para controlar los vuelos ya procesados
            var processedFlights = new HashSet<string>();
            var processedFlights_SID = new HashSet<string>();
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(item => item.ID).ToList();
            // Pasar del form principal llistes --> Roll Angle, True Track Angle, heading, posicion (lat,lon) i altitud. 
            List<PlaneFilter> interpolatedList = new List<PlaneFilter>();
            interpolatedList = InterpolateData(ListFilteredPlanes);
            var prev = ListFilteredPlanes[0];
            prev.time_sec = -1;
            // 2. Calcular el inicio del viraje para cada avión
            string aux = null;
            foreach (var planeData in ListFilteredPlanes)
            {
                if (planeData.time_sec >= 0)
                {
                    if (planeData.TrackAngleRate != -999 && planeData.RollAngle != -999 && planeData.Heading != -999 && planeData.TakeoffRWY == "LEBL-24L" && planeData.Altitude >= 75 && prev.AircraftID == planeData.AircraftID)
                    {
                        if (IASCalculations.HaversineDistance(planeData.Lat, planeData.Lon, 41.293, 2.0659305556) > 0)
                        {

                            // Se pasa la lista y el conjunto a la función
                            var turnStart = CalculateTurnStart(planeData, prev, processedFlights, turnStartPoints);

                            if (turnStart != null)
                            {
                                Random random = new Random();
                                double k = 1.0 + random.NextDouble() * (2.0 - 1.0);
                                turnStartPoints.Add(turnStart); // Agregar el punto a la lista si es válido
                                turnStartPoints_list.Add(Convert.ToString(turnStart.FlightId));
                                turnStartPoints_list.Add(Convert.ToString(turnStart.Latitude));
                                turnStartPoints_list.Add(Convert.ToString(turnStart.Longitude));
                                turnStartPoints_list.Add(Convert.ToString(turnStart.Altitude));
                                turnStartPoints_list.Add(Convert.ToString(turnStart.Radial));
                                turnStartPoints_list.Add(Convert.ToString(turnStart.MagHead - 10.0 * k));
                                turnStartPoints_list.Add(Convert.ToString(turnStart.MagHead));
                                turnStartPoints_list.Add(Convert.ToString(turnStart.RA));
                            }
                        }
                        else
                        {
                            aux = planeData.AircraftID;
                        }
                    }
                    prev = planeData;
                }


            }


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
                bool sidCompliant = CheckSIDCompliance(point, point.Latitude, point.Longitude, processedFlights_SID);
                sidCompilantList.Add(sidCompliant ? "True" : "False");
            }

            // 5. Obtenir fitxer KML
            GetKML(ListFilteredPlanes, stats.AverageLon, stats.AverageLat, stats.AverageAltitude);
            //GetKML2(ListFilteredPlanes);
            GetKML4(turnStartPoints);

            Viraje formViraje = new Viraje(turnStartPoints_list, sidCompilantList, statsList);
            formViraje.Show();


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

            return interpolatedData; ;

        }

        static TurnStartPoint CalculateTurnStart(PlaneFilter flightData, PlaneFilter prev, HashSet<string> processedFlights, List<TurnStartPoint> list)
        {
            // Verificar si el vuelo ya ha sido procesado
            if (processedFlights.Contains(flightData.AircraftID))
                return null; // Ignorar vuelos ya detectados

            // Coordenadas iniciales (posición alineada con RWY 24L)
            double initialHeading = 244.0;      // Rumbo inicial aproximado en grados
            double initialRollAngle = prev.RollAngle;
            const double rollAngleThreshold = 3.0;    // Umbral para detectar inicio de viraje (RollAngle)
            const double headingChangeThreshold = 3.0; // Umbral para detectar cambios en el Heading

            if (prev != null)
            {
                initialHeading = prev.Heading;
                initialRollAngle = prev.RollAngle;
            }
            // Obtener valores actuales del avión
            double currentHeading = Convert.ToDouble(flightData.Heading); //MagneticHeading
            double currentRollAngle = Convert.ToDouble(flightData.RollAngle);
            double currentLatitude = Convert.ToDouble(flightData.Lat);
            double currentLongitude = Convert.ToDouble(flightData.Lon);
            double currentAltitude = Convert.ToDouble(flightData.Altitude);

            if (flightData.Altitude < 120)
            {
                currentHeading = initialHeading;
            }

            // Detectar si el avión está en el punto de inicio del viraje
            bool isAlignedWithRunway = Math.Abs(currentHeading - initialHeading) >= headingChangeThreshold;
            bool rollStart = Math.Abs(currentRollAngle - initialHeading) > rollAngleThreshold;

            if ((rollStart || isAlignedWithRunway))
            {
                // Cálculo del radial al DVOR BCN
                double radial = CalculateRadial(currentLatitude, currentLongitude);

                // Verificar el cumplimiento del radial según las SID (234° ± 2°)
                //if (radial >= 232 && radial <= 236)
                //{
                // Crear el objeto TurnStartPoint
                TurnStartPoint turnStart = new TurnStartPoint
                {
                    FlightId = flightData.AircraftID,
                    Latitude = currentLatitude,
                    Longitude = currentLongitude,
                    Altitude = currentAltitude,
                    Radial = radial,
                    MagHead = currentHeading,
                    RA = currentRollAngle
                };

                // Marcar el vuelo como procesado
                processedFlights.Add(flightData.AircraftID);

                // Guardar el punto en la lista y retornarlo
                //list.Add(turnStart);
                return turnStart;
                //}
            }

            // Si no hay viraje detectado o no cumple con las restricciones, retornar null
            return null;
        }
        static bool IsPointInsideRectangle(double pointLat, double pointLon, double rectLat1, double rectLon1, double rectLat2, double rectLon2)
        {
            // Encontrar las coordenadas mínimas y máximas del rectángulo
            double minLat = Math.Min(rectLat1, rectLat2);
            double maxLat = Math.Max(rectLat1, rectLat2);
            double minLon = Math.Min(rectLon1, rectLon2);
            double maxLon = Math.Max(rectLon1, rectLon2);

            // Comprobar si el punto está dentro de los límites del rectángulo
            return pointLat >= minLat && pointLat <= maxLat &&
                   pointLon >= minLon && pointLon <= maxLon;
        }


        static double CalculateRadial(double lat, double lon)
        {
            // Calcular el radial desde el DVOR BCN(41.307222,  2.107778)
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

        static bool CheckSIDCompliance(TurnStartPoint point, double lon, double lat, HashSet<string> processedFlights)
        {
            // Coordenadas del DVOR BCN (centro del círculo)
            const double DVOR_Lat = 41.307111; // 41°18’25.6”N en formato decimal
            const double DVOR_Lon = 2.107806;  // 002°06’28.1”E en formato decimal
            var puntoDVOR = ToCartesian(DVOR_Lat, DVOR_Lon, DVOR_Lat, DVOR_Lon);  // DVOR (centro del círculo)

            // Coordenadas del punto en la costa (para calcular el radio)
            const double Coast_Lat = 41.268167; // 41º16´05.4”N en formato decimal
            const double Coast_Lon = 2.033333;  // 002º02´00.0”E en formato decimal
            var puntoCoast = ToCartesian(Coast_Lat, Coast_Lon, DVOR_Lat, DVOR_Lon);

            var puntoA = (x: DVOR_Lat, y: DVOR_Lon);  // Punto A
            var puntoB = (x: Coast_Lat, y: Coast_Lon);  // Punto B
            var puntoC = (x: lat, y: lon);  // Punto C (el que queremos comprobar)
            var puntoAvion = ToCartesian(lat, lon, DVOR_Lat, DVOR_Lon);

            // Calcular el radio del círculo (distancia entre DVOR y punto en la costa)
            //double radius = CalculateDistance(DVOR_Lat, DVOR_Lon, Coast_Lat, Coast_Lon);


            bool estaADerecha = EstaADerechaDeLaLinea(puntoA, puntoB, puntoC);

            // Calcular la distancia del punto al DVOR BCN (centro del círculo)
            //double distanceToDVOR = CalculateDistance(DVOR_Lat, DVOR_Lon, point.Latitude, point.Longitude);

            // Comprobar si el punto cumple con las restricciones de posición y altitud
            bool isCompliant = estaADerecha == true && // Dentro del círculo
                               point.Altitude <= 500 * GeoUtils.FEET2METERS;     // Altitud máxima

            // Verificar si el vuelo ya ha sido procesado
            if (processedFlights.Contains(point.FlightId))
            {
                return false; // Si ya fue procesado, no cumple con la SID
            }

            // Si cumple con las condiciones, se marca como procesado
            if (isCompliant)
            {
                processedFlights.Add(point.FlightId);
            }

            return isCompliant;
        }

        static bool EstaADerechaDeLaLinea((double x, double y) A, (double x, double y) B, (double x, double y) C)
        {
            // Calcular el determinante (producto cruzado en 2D)
            double determinante = (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x);

            // Si el determinante es negativo, el punto C está a la derecha de la línea AB
            return determinante < 0;
        }

        static (double x, double y) ToCartesian(double lat, double lon, double refLat, double refLon)
        {
            const double R = 6371000; // Radio de la Tierra en metros

            // Convertir a radianes
            double latRad = lat * Math.PI / 180;
            double lonRad = lon * Math.PI / 180;
            double refLatRad = refLat * Math.PI / 180;
            double refLonRad = refLon * Math.PI / 180;

            // Coordenadas proyectadas en un plano local (en metros)
            double x = (lonRad - refLonRad) * Math.Cos((latRad + refLatRad) / 2) * R;
            double y = (latRad - refLatRad) * R;

            return (x, y);
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
            public double MagHead { get; set; }
            public double RA { get; set; }
        }
        // Cuenta el numero de aviones que salen introducidas en le string rwy
        public int CountTakeoffRWY(List<PlaneFilter> planes, string rwy)
        {
            // ordenamos la lista por id para facilitar su analisis
            planes = planes.OrderBy(item => item.ID).ToList();
            int count = 0;
            int id = 0;
            for (int i = 0; i < planes.Count; i++)
            {
                if (planes[i].TakeoffRWY == rwy && id != planes[i].ID)
                {
                    id = planes[i].ID;
                    count++;
                }
            }
            return count;
        }
        // Cuenta los aviones que salen por la pista 06R
        private void BTNCountRight_Click(object sender, EventArgs e)
        {
            LBLNumRight.Text = "Total LEBL-06R takeoffs=" + CountTakeoffRWY(ListFilteredPlanes, "LEBL-06R");
        }
        // Cuenta los aviones que salen por la pista 24L
        private void BTNCountLeft_Click(object sender, EventArgs e)
        {
            LBLNumLeft.Text = "Total LEBL-24L takeoffs=" + CountTakeoffRWY(ListFilteredPlanes, "LEBL-24L");
        }
        // Clase para las estadisticas del forms de viraje
        private class Statistics
        {
            public double AverageLat { get; set; }
            public double AverageLon { get; set; }
            public double AverageAltitude { get; set; }
            public double AverageRadial { get; set; }
        }

        // Method to generate a KML file 
        static void GetKML(List<PlaneFilter> originalData, double lat, double lon, double alt)
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

                List<string> aircrfatName = new List<string>();
                // Iterate over each item in the "bloque" list (aircraft data)
                for (int i = 0; i < originalData.Count; i++)
                {
                    if (originalData[i].TakeoffRWY == "LEBL-24L")
                    {
                        int flag = 0;
                        foreach (string name in aircrfatName)
                        {
                            if (name == originalData[i].AircraftID)
                            {
                                flag = 1;
                            }
                        }
                        if (flag == 0)
                        {
                            if (originalData[i].Altitude <= 1000)
                            {
                                string nombre = originalData[i].AircraftID;

                                if (!posicionesDeRepeticiones.ContainsKey(nombre))
                                {
                                    posicionesDeRepeticiones[nombre] = new KML_DATA();
                                    posicionesDeRepeticiones[nombre].Positions = new List<Vector>();
                                    posicionesDeRepeticiones[nombre].Description = "Aircraft address: " + nombre + " ; Aircraft indentification: " + originalData[i].AircraftID + " ; Track number: " + originalData[i].TrackNum + "; Roll Angle: " + originalData[i].RollAngle + "; True Track Angle: " + originalData[i].TrueTrackAngle + "; Track Angle Rate: " + originalData[i].TrackAngleRate + "; Magnetic Heading: " + originalData[i].MagneticHeading;
                                }
                                posicionesDeRepeticiones[nombre].Positions.Add(new Vector(originalData[i].Lat, originalData[i].Lon, originalData[i].Altitude));
                            }
                            else
                            {
                                aircrfatName.Add(originalData[i].AircraftID);
                            }
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

                // Adding independent lines using known start and end points
                Vector startPoint1 = new Vector(41.292219, 2.103281, 220); // Start point of first line
                Vector endPoint1 = new Vector(41.268167, 2.033333, 220);   // End point of first line
                AddIndependentLine(document, startPoint1, endPoint1, "Independent Line 1", new Color32(255, 165, 0, 1));

                // Create a second line with calculated offset angle (optional)
                //double angleIncrement = 237; // Degrees
                //double distance = 8.0;    // Distance in degrees (~1km)
                //Vector offsetPoint = CalculateOffsetPoint(startPoint1, distance, angleIncrement);
                //AddIndependentLine(document, startPoint1, offsetPoint, "Independent Line 2", new Color32(255, 165, 0, 1));

                // Known third line
                Vector startPoint3 = new Vector(41.307111, 2.107806, 220);  // Start point of third line
                Vector endPoint3 = new Vector(41.268167, 2.033333, 220);    // End point of third line
                AddIndependentLine(document, startPoint3, endPoint3, "Independent Line 3", new Color32(255, 0, 255, 1));

                // Add pins to specific locations
                //AddPin(document, new Vector(lat, lon, alt), "Average Point", "", new Color32(255, 165, 0, 1)); // Yellow pin

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
        static void AddPin(Document document, Vector position, string pinName, string description, Color32 pinColor)
        {
            // Create the point
            var point = new SharpKml.Dom.Point
            {
                Coordinate = position,
                AltitudeMode = AltitudeMode.RelativeToGround
            };

            // Create a style for the pin
            var style = new Style
            {
                Icon = new IconStyle
                {
                    Color = pinColor,
                    Scale = 1.0,
                    Icon = new IconStyle.IconLink(new Uri("http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png"))
                }
            };

            // Create the placemark
            var placemark = new SharpKml.Dom.Placemark
            {
                Name = pinName,
                Description = new Description { Text = description },
                Geometry = point
            };

            // Assign a unique style ID to the pin
            string styleId = $"{pinName.Replace(" ", "_")}_Pin_Style";
            style.Id = styleId;
            document.AddStyle(style);
            placemark.StyleUrl = new Uri($"#{styleId}", UriKind.Relative);

            // Add the placemark to the document
            document.AddFeature(placemark);
        }

        static void AddIndependentLine(Document document, Vector startPoint, Vector endPoint, string lineName, Color32 lineColor)
        {
            // Create line geometry
            var lineString = new LineString
            {
                Coordinates = new CoordinateCollection
                {
                    startPoint,
                    endPoint
                },
                AltitudeMode = AltitudeMode.RelativeToGround
            };

            // Create a custom style for the line
            var style = new Style
            {
                Line = new LineStyle
                {
                    Color = lineColor,
                    Width = 4.0, // Use the passed color
                }
            };

            // Create placemark
            var placemark = new SharpKml.Dom.Placemark
            {
                Name = lineName,
                Geometry = lineString
            };

            // Add the style to the document and associate it with the placemark
            string styleId = $"{lineName.Replace(" ", "_")}_Style";
            style.Id = styleId;
            document.AddStyle(style);
            placemark.StyleUrl = new Uri($"#{styleId}", UriKind.Relative);

            // Add the placemark to the document
            document.AddFeature(placemark);
        }

        static Vector CalculateOffsetPoint(Vector startPoint, double distance, double angle)
        {
            double earthRadius = 6371; // Earth's radius in km
            double lat1 = DegreesToRadians(startPoint.Latitude);
            double lon1 = DegreesToRadians(startPoint.Longitude);

            double bearing = DegreesToRadians(angle);
            double angularDistance = distance / earthRadius;

            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(angularDistance) +
                                    Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(bearing));
            double lon2 = lon1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(angularDistance) * Math.Cos(lat1),
                                            Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(lat2));

            return new Vector(RadiansToDegrees(lat2), RadiansToDegrees(lon2), 220);
        }

        static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
        static double RadiansToDegrees(double radians) => radians * 180 / Math.PI;

        private class KML_DATA
        {
            public List<Vector> Positions { get; set; }
            public string Description { get; set; }
        }
        static void GenerateKMLFromPoints(List<TurnStartPoint> points, string filePath)
        {
            // Crear un documento KML
            var document = new Document();
            var kml = new Kml();

            // Recorrer cada punto y agregarlo al documento KML
            foreach (var point in points)
            {
                // Crear un marcador (placemark) para cada punto
                var placemark = new Placemark
                {
                    Geometry = new SharpKml.Dom.Point
                    {
                        Coordinate = new Vector(point.Latitude, point.Longitude, point.Altitude)
                    }
                };

                // Agregar el marcador al documento
                document.AddFeature(placemark);
            }

            // Asociar el documento al KML
            kml.Feature = document;

            // Guardar el KML en el archivo especificado
            using (var stream = File.Create(filePath))
            {
                var kmlFile = KmlFile.Create(kml, false);
                kmlFile.Save(stream);
            }

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

        // Genera el punts del KML
        static void GetKML4(List<TurnStartPoint> points)
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

                GenerateKMLFromPoints(points, filePath);
                Console.WriteLine("Heatmap KML file created successfully.");
            }
            else
            {
                Console.WriteLine("No file selected.");
            }
        }
        // Metode per crear una LineString a partir de la llista de posicions
        static LineString CreateLineString(List<Vector> positions)
        {
            var lineString = new LineString
            {
                Coordinates = new CoordinateCollection(),
                AltitudeMode = AltitudeMode.Absolute // Por defecte
            };

            foreach (var pos in positions)
            {
                lineString.Coordinates.Add(pos);

                // Cambiar el mode d'altitud si està per debaix de 1828.8 metres
                if (pos.Altitude <= 1828.8)
                    lineString.AltitudeMode = AltitudeMode.RelativeToGround;
            }

            return lineString;
        }

        // Métode para crear un círcule como Placemark en KML
        static Placemark CreateCirclePlacemark(double centerLat, double centerLon, double radius)
        {
            var placemark = new Placemark { Name = "Área del SID - Restricción" };

            var style = new Style
            {
                Id = "CircleStyle", // ID de l'estil
                Line = new LineStyle { Color = new Color32(255, 255, 0, 255), Width = 5 },
                Polygon = new PolygonStyle { Color = new Color32(128, 255, 255, 50) } // GROC per semitransparent moure el primer parametre
            };

            var circle = new Polygon();
            var ring = new LinearRing
            {
                Coordinates = new CoordinateCollection() // ¡Inicialización necesaria!
            };

            // numero de punt que tindra el cercle
            const int numPoints = 360;

            for (int i = 0; i < numPoints; i++)
            {
                double angle = 2 * Math.PI * i / numPoints;
                double lat = centerLat + (radius / 111320) * Math.Cos(angle); // Aproximació per latitud
                double lon = centerLon + (radius / 111320) * Math.Sin(angle) / Math.Cos(centerLat * Math.PI / 180); // Longitud ajustada
                ring.Coordinates.Add(new Vector(lat, lon, 0)); // Afegim l'anell
            }
            // Asignem l'anell al cercle
            circle.OuterBoundary = new OuterBoundary { LinearRing = ring };

            // Asociem la geometria i lestil de Placemark
            placemark.Geometry = circle;

            // Vincular estil personalizat al cercle (no va)
            placemark.StyleUrl = new Uri("#CircleStyle", UriKind.Relative);


            // Imprimir la geometría del placemark para asegurarnos de que se asignó correctamente
            Debug.WriteLine($"Placemark Geometry: {placemark.Geometry}");

            return placemark;
        }

        // calculamos la distancia horizontal usando la formula de Haversine
        public double DistHoritzontal(double longitud_plane, double latitud_plane, double h1, double longitud_DEP, double latitud_DEP)
        {
            double lat1 = 0, long1 = 0;
            double lat2 = 0, long2 = 0;

            CoordinatesUVH coord1 = null, coord2 = null;

            lat1 = latitud_plane * GeoUtils.DEGS2RADS;
            long1 = longitud_plane * GeoUtils.DEGS2RADS;

            lat2 = latitud_DEP * GeoUtils.DEGS2RADS;
            long2 = longitud_DEP * GeoUtils.DEGS2RADS;

            coord1 = UVCoordinates.GetUV(lat1, long1, h1, false);
            coord2 = UVCoordinates.GetUV(lat2, long2, 0.0,false);

            // Calculate the Euclidean distance between the two aircraft's UV coordinates (U and V)
            double distancia = Math.Round(Math.Sqrt(Math.Pow(coord2.U - coord1.U, 2) + Math.Pow(coord2.V - coord1.V, 2)), 3);

            return distancia;
        }
        // Calulamos si lso aviones cruzan los DER y treshold especificados en el proyecto
        public List<IASData> CalculateThresholdCrossings(List<PlaneFilter> planes, string runway, double distanceThreshold, bool ThresORder)
        {
            planes = planes.OrderBy(item => item.AircraftID).ToList();
            List<IASData> thresholdCrossings = new List<IASData>();

            //True if the thresholds have been crossed to reduce the number of operations
            bool thresholdFound = false;
            bool DERFound = false;
            bool firstdetection = false;

            // Auxiliary string to reduce amount of operations
            string aux2 = planes[0].AircraftID;

            // INIT POSITION REFERENCES
            double thresholdLat = 0.0;
            double thresholdLon = 0.0;
            double DERLat = 0.0;
            double DERLon = 0.0;
            if (runway == "LEBL-24L")
            {
                thresholdLat = 41.2922194444;
                thresholdLon = 2.1032805556;
                DERLat = 41.2823111111;
                DERLon = 2.07435;
            }
            else if (runway == "LEBL-06R")
            {
                thresholdLat = 41.2823111111;
                thresholdLon = 2.07435;
                DERLat = 41.2922194444;
                DERLon = 2.1032805556;
            }
            if (thresholdLat != 0.0 && thresholdLon != 0.0 && DERLat != 0.0 && DERLon != 0.0)
            {

                for(int i = 0; i < planes.Count; i++)
                {
                    var plane = planes[i];
                    if (plane.TakeoffRWY == runway)
                    {
                        if (aux2 != plane.AircraftID)
                        {
                            aux2 = plane.AircraftID;
                            thresholdFound = false;
                            DERFound = false;
                        }

                        if (aux2 == plane.AircraftID)
                        {
                            double distTHR = DistHoritzontal(plane.Lon, plane.Lat, plane.Altitude, thresholdLon, thresholdLat); // plane.Lon, plane.Lat, thresholdLon, thresholdLat
                            double distDER = DistHoritzontal(plane.Lon, plane.Lat, plane.Altitude, DERLon, DERLat);

                            if (thresholdFound == false && ThresORder == true)
                            {
                                // Calcular la distancia entre el avión y el umbral

                                double distance = IASCalculations.HaversineDistance(plane.Lat, plane.Lon, thresholdLat, thresholdLon);

                                double distUV = DistHoritzontal(plane.Lon, plane.Lat, plane.Altitude, thresholdLon, thresholdLat);
                                if (distance > distUV)
                                {
                                    distance = distUV;
                                }

                                // Si la distancia es menor al umbral (por ejemplo, 100 metros), consideramos que cruzó el umbral
                                if (distance <= distanceThreshold)
                                {
                                    // Crear un nuevo objeto IASData para guardar los datos del cruce
                                    IASData data = new IASData
                                    {
                                        AircraftId = plane.AircraftID,
                                        Time = plane.time_sec,
                                        Altitude = plane.Altitude, // Asegúrate de que la altitud esté corregida por el QNH si es necesario
                                        IAS = plane.IndicatedAirspeed
                                    };

                                    // Añadir el cruce a la lista de datos
                                    thresholdCrossings.Add(data);
                                    thresholdFound = true;
                                }

                            }

                            else if (DERFound == false && ThresORder == false)
                            {
                                // Calcular la distancia entre el avión y el umbral
                                double distanceDER = IASCalculations.HaversineDistance(plane.Lat, plane.Lon, DERLat, DERLon);
                                double distUV = DistHoritzontal(plane.Lon, plane.Lat, plane.Altitude, DERLon, DERLat);
                                if (distanceDER > distUV)
                                {
                                    distanceDER = distUV;
                                }
                                // Si la distancia es menor al umbral (por ejemplo, 100 metros), consideramos que cruzó el umbral
                                if (distanceDER <= distanceThreshold)
                                {
                                    // Crear un nuevo objeto IASData para guardar los datos del cruce
                                    IASData data = new IASData
                                    {
                                        AircraftId = plane.AircraftID,
                                        Time = plane.time_sec,
                                        Altitude = plane.Altitude, 
                                        IAS = plane.IndicatedAirspeed
                                    };

                                    // Añadir el cruce a la lista de datos
                                    thresholdCrossings.Add(data);
                                    DERFound = true;
                                }
                            }
                        }
                    }
                }

            }
            return thresholdCrossings;
        }
        // Carga el form con los datos de las IAS a las alturas especificadas y los puntos donde se cruza el DER y el THR
        private void IASInfo_Click(object sender, EventArgs e)
        {
            FindIASatDetAltitude();
            ThresholdListRight = CalculateThresholdCrossings(ListFilteredPlanes, "LEBL-06R", 50.0, true);
            ThresholdListLeft = CalculateThresholdCrossings(ListFilteredPlanes, "LEBL-24L", 50.0, true);
            DERListRight = CalculateThresholdCrossings(ListFilteredPlanes, "LEBL-06R", 150.0, false);
            DERListLeft = CalculateThresholdCrossings(ListFilteredPlanes, "LEBL-24L", 150.0, false);

            //IASInformation IASInfo = new IASInformation(ListFilteredPlanes);
            IASInformation IASInfo = new IASInformation(IASList, ThresholdListRight, ThresholdListLeft, DERListRight, DERListLeft, GetSnometroDist(ListFilteredPlanes));
            IASInfo.Show();
        }

        // IAS implementation

        List<IASData> IASList = new List<IASData>();
        List<IASData> ThresholdListRight = new List<IASData>();
        List<IASData> ThresholdListLeft = new List<IASData>();
        List<IASData> DERListRight = new List<IASData>();
        List<IASData> DERListLeft = new List<IASData>();

        public void AddIasData(PlaneFilter planeBefore, PlaneFilter planeAfter, double altitude)
        {
            IASData aux = new IASData();

            PlaneFilter plane = IASCalculations.CheckIAS4Altitude(planeBefore, planeAfter, altitude);
            aux.AircraftId = plane.AircraftID;
            aux.Time = plane.time_sec;
            aux.Altitude = plane.Altitude;
            aux.IAS = plane.IndicatedAirspeed;

            if (aux != null)
            {
                IASList.Add(aux);
            }

        }
        // Busca/calcula la velocidad de IAS de un avion a las alturas deseadas
        public void FindIASatDetAltitude() //List<PlaneFilter> ListFilteredPlanes
        {
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(item => item.AircraftID).ToList();
            bool find850 = false, find1500 = false, find3500 = false;
            string aux = null;
            for (int i = 0; i < ListFilteredPlanes.Count; i++)
            {
                if (ListFilteredPlanes[i].IndicatedAirspeed != -999)
                {
                    // Guardamos el nombre de los vuelos para los que tenemos todos sus puntos
                    if (find850 == true && find1500 == true && find3500 == true) {
                        aux = ListFilteredPlanes[i].AircraftID;
                        find850 = false;
                        find1500 = false;
                        find3500 = false;
                    }
                    // Solo buscamos si no tenemos las 3 alturas analizadas
                    if (aux != ListFilteredPlanes[i].AircraftID)
                    {
                        // ALtitude 850 ft
                        if (GeoUtils.FEET2METERS * 850 - ListFilteredPlanes[i].Altitude < 0 && find850 == false)
                        {
                            AddIasData(ListFilteredPlanes[i - 1], ListFilteredPlanes[i], 850 * GeoUtils.FEET2METERS);
                            find850 = true;
                        }
                        // ALtitude 1500 ft
                        else if (GeoUtils.FEET2METERS * 1500 - ListFilteredPlanes[i].Altitude < 0 && find1500 == false)
                        {
                            AddIasData(ListFilteredPlanes[i - 1], ListFilteredPlanes[i], 1500 * GeoUtils.FEET2METERS);
                            find1500 = true;
                        }
                        // ALtitude 3500 ft
                        else if (GeoUtils.FEET2METERS * 3500 - ListFilteredPlanes[i].Altitude < 0 && find3500 == false)
                        {
                            AddIasData(ListFilteredPlanes[i - 1], ListFilteredPlanes[i], 3500 * GeoUtils.FEET2METERS);
                            find3500 = true;
                        }
                    }
                }

            }
        }

        public class DataSonometro
        {
            public string AircraftID { get; set; }
            public double time_sec { get; set; }
            public double distance { get; set; }
            public double altura { get; set; }
        }

        public List<DataSonometro> GetSnometroDist(List<PlaneFilter> list)
        {
            List<DataSonometro> distances = new List<DataSonometro>();
            list = list.OrderBy(item => item.ID).ToList();
            double Lat_Son = 41.2719444444;
            double Lon_Son = 2.04777777778;

            for (int i = 0; i <list.Count; i++)
            {
               if (distances.Count == 0)
               {
                    double dist = IASCalculations.HaversineDistance(list[i].Lat, list[i].Lon, Lat_Son, Lon_Son);
                    distances.Add(new DataSonometro { AircraftID = list[i].AircraftID, time_sec = list[i].time_sec, distance = dist, altura = list[i].Altitude });
               }
               else if (list[i].AircraftID != distances[distances.Count - 1].AircraftID)
               {
                    double dist = IASCalculations.HaversineDistance(list[i].Lat, list[i].Lon, Lat_Son, Lon_Son);
                    distances.Add(new DataSonometro { AircraftID = list[i].AircraftID, time_sec = list[i].time_sec, distance = dist, altura = list[i].Altitude });
               }
               else if (list[i].AircraftID == distances[distances.Count - 1].AircraftID)
                {
                    double dist = IASCalculations.HaversineDistance(list[i].Lat, list[i].Lon, Lat_Son, Lon_Son);
                    if (dist < distances[distances.Count - 1].distance)
                    {
                        distances[distances.Count-1].distance = dist;
                        distances[distances.Count-1].altura = list[i].Altitude;
                        distances[distances.Count-1].time_sec = list[i].time_sec;
                    }
                }
            }

            return distances;
        }
    }
}
