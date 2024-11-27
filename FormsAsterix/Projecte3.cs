﻿using LibAsterix;
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
                    click++;
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
        public void WriteANS()
        {
            ANSTotalComparision.Text = $"Total messages = {TotalMessageComparation}";
            ANSIncidentPlanes.Text = $"Total planes with incidents = {TotalIncidencePlanes}";
            ANSTotalEstelaComparations.Text = $"Total messages estela = {TotalEstaleComparationMessages}";
            ANSTotalRadar.Text = $"Total radar incidents = {TotalRadarIncidents}";
            ANSTotalEstela.Text = $"Total estela incidents = {TotalEstelaIncidents}";
            ANSTotalLoA.Text = $"Total LoA incidents = {TotalLoAIncidents}";
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
            GenStatisticsBtn.Enabled = true;
            GenStatisticsBtn.Visible = true;
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
                WriteANS();

                File.WriteAllText(filePath, sbCSV.ToString());
                MessageBox.Show("CSV file generated");
            }
            else { MessageBox.Show("CSV file generation failed"); }
        }

        private void Projecte3_Load(object sender, EventArgs e)
        {
            ListFilteredPlanes = ListFilteredPlanes.OrderBy(data => data.time_sec).ToList();
            dataGridProject3.DataSource = ListFilteredPlanes;
            GenStatisticsBtn.Enabled = false;
            GenStatisticsBtn.Visible = false;
        }



        private void InicioViraje ()
        {
            // Pasar del form principal llistes --> Roll Angle, True Track Angle, heading, posicion (lat,lon) i altitud. 
            // List<PlaneFilter> FilteredList


            // 2. Calcular el inicio del viraje para cada avión
            var turnStartPoints = new List<TurnStartPoint>();
            

            foreach (var planeData in ListFilteredPlanes)
            {
                var turnStart = CalculateTurnStart(planeData);
                if (turnStart != null)
                    turnStartPoints.Add(turnStart);
            }

            // 3. Calcular estadísticas de posición, altitud y radial
            var stats = CalculateStatistics(turnStartPoints);

            // 4. Mostrar resultados
            Console.WriteLine("Estadísticas de posición y altitud:");
            Console.WriteLine($"Latitud promedio: {stats.AverageLat}");
            Console.WriteLine($"Longitud promedio: {stats.AverageLon}");
            Console.WriteLine($"Altitud promedio: {stats.AverageAltitude}");
            Console.WriteLine($"Radial promedio del DVOR BCN: {stats.AverageRadial}");

            // Comprobar si se cumplen las condiciones de la SID
            foreach (var point in turnStartPoints)
            {
                bool sidCompliant = CheckSIDCompliance(point);
                Console.WriteLine($"Vuelo {point.FlightId} cumple con SID: {sidCompliant}");
            }


            // Nos quedamos con los aviones que hacen departure por RWY 24L 

            // interpolar valors RA i TTA amb Heading

            // trobar lat i lon en el moment en que es segueix la condicio de l'angle --> tambe calcular alçada

            // calcul radial des del punt inici viratge al DVOR --> veure SID segons AIP

            // calcul estadisitiques --> segueixen SID?
        }

        static TurnStartPoint CalculateTurnStart(PlaneFilter flightData)
        {
            // Coordenadas iniciales (posición alineada con RWY 24L)
            const double initialLatitude = 41.296944;  // Ejemplo: Coordenadas aproximadas de la cabecera de RWY 24L
            const double initialLongitude = 2.078333;
            const double initialHeading = 240.0;      // Rumbo inicial aproximado en grados
            const double rollAngleThreshold = 5.0;    // Umbral para detectar inicio de viraje (RollAngle)
            const double headingChangeThreshold = 5.0; // Umbral para detectar cambios en el Heading

            // Obtener valores actuales del avión
            double currentHeading = 1; // Convert.ToDouble(flightData.Heading); //***************** FALTA PASSAR?
            double currentRollAngle = Convert.ToDouble(flightData.RollAngle);
            double currentLatitude = Convert.ToDouble(flightData.Lat);
            double currentLongitude = Convert.ToDouble(flightData.Lon);

            // Detectar si hay un cambio significativo en RollAngle o Heading
            bool rollStart = Math.Abs(currentRollAngle) > rollAngleThreshold;
            bool headingStart = Math.Abs(currentHeading - initialHeading) > headingChangeThreshold;

            // Si detectamos inicio de viraje
            if (rollStart || headingStart)
            {
                // Cálculo del radial al DVOR BCN
                double radial = CalculateRadial(currentLatitude, currentLongitude);

                // Retornar el punto donde se inicia el viraje
                return new TurnStartPoint
                {
                    FlightId = flightData.AircraftID, // Cambiar según tu propiedad para ID del avión
                    Latitude = currentLatitude,
                    Longitude = currentLongitude,
                    Altitude = Convert.ToDouble(flightData.Altitude),
                    Radial = radial
                };
            }

            // Si no hay viraje detectado, retornar null
            return null;
        }

        

        static double CalculateRadial(double lat, double lon)
        {
            // Calcular el radial desde el DVOR BCN (41.297445, 2.083294)
            double dLat = lat - 41.297445;
            double dLon = lon - 2.083294;
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

        static bool CheckSIDCompliance(TurnStartPoint point)
        {
            // Comprobar si el radial y la posición cumplen con la nota de la SID
            double requiredRadial = 234;
            return point.Radial >= requiredRadial - 2 && point.Radial <= requiredRadial + 2;
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

        class Statistics
        {
            public double AverageLat { get; set; }
            public double AverageLon { get; set; }
            public double AverageAltitude { get; set; }
            public double AverageRadial { get; set; }
        }

    }
}
