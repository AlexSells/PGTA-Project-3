using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
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
using SharpKml.Dom;
using SharpKml.Base;
using SharpKml.Engine;
using Vector = SharpKml.Base.Vector;
using Document = SharpKml.Dom.Document;
using GMap.NET.MapProviders;
using GMap.NET;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using LibAsterix;
using System.Security.Cryptography;
using Amazon.IdentityManagement.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace FormsAsterix
{
    public partial class DistHoritzontal : Form
    {

        // veure a on declarar per rebre info de l'altre formulari :)
        string Aircraft1;
        string Aircraft2;

        List<double> longitudList = new List<double>();
        List<double> latitudList = new List<double>();
        List<double> AltitudeList = new List<double>();

        List<String> AircraftIDList = new List<String>();
        List<string> AircraftAddrList = new List<string>();
        List<string> TrackNumList = new List<string>();
        List<string> Mode3AList = new List<string>();
        List<string> SACList = new List<string>();
        List<string> SICList = new List<string>();

        List<double> DistHor = new List<double>();



        List<long> time = new List<long>();
        long timeInicial;

        List<List<DataItem>> bloque = new List<List<DataItem>>();


        private Dictionary<string, PointLatLng> lastPositions = new Dictionary<string, PointLatLng>(); // Dictionary to track previous positions
        private HashSet<string> Sim_diccionary = new HashSet<string>(); // Using HashSet for better performance
        private GMapOverlay aircraftOverlay = new GMapOverlay("aircraftOverlay"); // Single overlay for all markers

        public DistHoritzontal(string A1, string A2, List<double> longitudList_sub, List<double> latitudList_sub, List<String> AircraftIDList_sub, List<string> AircraftAddrList_sub, List<string> TrackNumList_sub, List<string> Mode3AList_sub, List<string> SACList_sub, List<string> SICList_sub, List<double> AltitudeList_sub, List<long> time_sub, List<List<DataItem>> bloque_sub, List<double> DistHor_sub)
        {
            InitializeComponent();
            // Set some initialization parameters
            Start_sim.FlatAppearance.BorderSize = 0;
            Start_sim.FlatAppearance.MouseDownBackColor = Color.Transparent;
            Start_sim.FlatAppearance.MouseOverBackColor = Color.Transparent;
            Start_sim.MouseEnter += (s, e) => Start_sim.Cursor = Cursors.Hand;
            Start_sim.MouseLeave += (s, e) => Start_sim.Cursor = Cursors.Default;

            this.Aircraft1 = A1;
            this.Aircraft2 = A2;
            this.AircraftIDList = AircraftIDList_sub;
            this.AircraftAddrList = AircraftAddrList_sub;
            this.TrackNumList = TrackNumList_sub;
            this.Mode3AList = Mode3AList_sub;
            this.SACList = SACList_sub;
            this.SICList = SICList_sub;
            this.AltitudeList = AltitudeList_sub;
            this.longitudList = longitudList_sub;
            this.latitudList = latitudList_sub;
            this.DistHor = DistHor_sub;

            this.time = time_sub;
            this.timeInicial = time[0];

            this.bloque = bloque_sub;


            // Define DataGridView
            SetHeaders(Aircraft1, Aircraft2);

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(40, 40);
            imageList.Images.Add(Properties.Resources.play_button);
            Start_sim.Image = imageList.Images[0];
            Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
            Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
            Start_sim.ImageAlign = ContentAlignment.TopCenter;
            Start_sim.TextAlign = ContentAlignment.BottomCenter;
            timeInicial = time[0];
            timeTXT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)(timeInicial / 3600), (int)((timeInicial % 3600) / 60), (int)(timeInicial % 60));

            valueTXT.Text = "";
            valueNM.Text = "";

        }

        private void SetHeaders(string A1, string A2)
        {
            // List of column headers to display
            List<string> lista = new List<string> {
            "Aircraft address","Track Number", "Mode-3/A reply","Latitud", "Longitud", "Height", "SAC", "SIC"};


            // Adding two columns for the two aircraft (A1 and A2)
            dataGridView1.Columns.Add(A1, A1);
            dataGridView1.Columns.Add(A2, A2);


            // Set some design parameters
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.RowHeadersWidth = 150;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.RowHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);

            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkTurquoise;


            dataGridView1.AllowUserToAddRows = false;

            foreach (string headerText in lista)
            {
                int rowIndex = dataGridView1.Rows.Add(); 
                dataGridView1.Rows[rowIndex].HeaderCell.Value = headerText; // Assign the header text as the row header
            }


            int flag1 = 0;
            int flag2 = 0;
            int iter = 0;

            // Loop through the list of aircraft and populate the DataGridView with their data
            for (int i = 0; i < AircraftAddrList.Count; i++)
            {
                if (AircraftIDList[i].Trim() == A1.Trim() && flag1 == 0)
                {
                    // Check if the current aircraft matches A1 and fill in its data
                    dataGridView1.Rows[0].Cells[0].Value = AircraftAddrList[iter];
                    dataGridView1.Rows[1].Cells[0].Value = TrackNumList[iter];
                    dataGridView1.Rows[2].Cells[0].Value = Mode3AList[iter];
                    dataGridView1.Rows[3].Cells[0].Value = latitudList[iter];
                    dataGridView1.Rows[4].Cells[0].Value = longitudList[iter];
                    dataGridView1.Rows[5].Cells[0].Value = AltitudeList[iter];
                    dataGridView1.Rows[6].Cells[0].Value = SACList[iter];
                    dataGridView1.Rows[7].Cells[0].Value = SICList[iter];
                    flag1 = 1;
                }
                else if (AircraftIDList[i].Trim() == A2.Trim() && flag2 == 0)
                {
                    // Check if the current aircraft matches A2 and fill in its data
                    dataGridView1.Rows[0].Cells[1].Value = AircraftAddrList[iter];
                    dataGridView1.Rows[1].Cells[1].Value = TrackNumList[iter];
                    dataGridView1.Rows[2].Cells[1].Value = Mode3AList[iter];
                    dataGridView1.Rows[3].Cells[1].Value = latitudList[iter];
                    dataGridView1.Rows[4].Cells[1].Value = longitudList[iter];
                    dataGridView1.Rows[5].Cells[1].Value = AltitudeList[iter];
                    dataGridView1.Rows[6].Cells[1].Value = SACList[iter];
                    dataGridView1.Rows[7].Cells[1].Value = SICList[iter];
                    flag2 = 1;
                }
                iter++;
            }
        }


        // This method is designed to initialize the GMapControl with a set of configurations to ensure the map behaves as expected during the app's lifecycle
        int zoom = 7;
        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            gMapControl1.PolygonsEnabled = true;
            gMapControl1.MarkersEnabled = true;
            gMapControl1.NegativeMode = false;
            gMapControl1.RetryLoadTile = 0;
            gMapControl1.ShowTileGridLines = false;
            gMapControl1.AllowDrop = true;
            gMapControl1.IgnoreMarkerOnMouseWheel = true;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.DisableFocusOnMouseEnter = true;
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = zoom;
            gMapControl1.Position = new PointLatLng(41.300702, 2.102058);
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.ShowCenter = false;
        }


        int num_loop = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Increment the initial time every time the timer triggers
            timeInicial++;

            // Run the simulation and update the markers if necessary
            Tick(ref timeInicial, ref num_loop, Aircraft1, Aircraft2);

            // Update the UI with the current time in a formatted manner (HH:MM:SS)
            timeTXT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)(timeInicial / 3600), (int)((timeInicial % 3600) / 60), (int)(timeInicial % 60));

            // Refresh the map only every few ticks to improve performance
            gMapControl1.Refresh();
        }

        private void Tick(ref long timeTick, ref int num_loop, string a1, string a2)
        {
            // Check if num_loop is less than the number of aircraft in the list
            if (num_loop != AircraftIDList.Count)
            {
                for (int i = 0; num_loop < AircraftIDList.Count && timeTick >= time[num_loop]; num_loop++)
                {
                    if (AircraftIDList[num_loop] != "N/A")
                    {
                        SetValuesCells(AircraftIDList[num_loop], num_loop);
                        AddMarkerToMap(latitudList[num_loop], longitudList[num_loop], AircraftIDList[num_loop], num_loop);
                        valueTXT.Text = Convert.ToString(Math.Round(DistHor[num_loop], 3)) + " km ";
                        valueNM.Text = Convert.ToString(Math.Round(DistHor[num_loop] * 1000 / 1852, 3)) + " NM ";
                        dataGridView1.Refresh();
                    }


                }
                gMapControl1.Update();
            }
            else
            {
                // If num_loop reaches the total number of aircraft, stop the timer and adjust the timeTick
                timer1.Stop();
                timeTick = timeTick - 2;
                Start_sim.Visible = false;
            }
        }

        private void SetValuesCells(string Aid, int iter)
        {
            if (Aid.Trim() == Aircraft1.Trim())
            {
                dataGridView1.Rows[3].Cells[0].Value = latitudList[iter];
                dataGridView1.Rows[4].Cells[0].Value = longitudList[iter];
                dataGridView1.Rows[5].Cells[0].Value = AltitudeList[iter];
            }
            else if (Aid.Trim() == Aircraft2.Trim())
            {
                dataGridView1.Rows[3].Cells[1].Value = latitudList[iter];
                dataGridView1.Rows[4].Cells[1].Value = longitudList[iter];
                dataGridView1.Rows[5].Cells[1].Value = AltitudeList[iter];
            }
        }

        private void AddMarkerToMap(double lat, double lon, string name, int currentIndex)
        {
            bool nameAppearsInFuture1 = AircraftIDList.Skip(currentIndex + 1).Contains(Aircraft1.Trim());
            bool nameAppearsInFuture2 = AircraftIDList.Skip(currentIndex + 1).Contains(Aircraft2.Trim());

            if (!nameAppearsInFuture1 && !nameAppearsInFuture2)
            {
                Start_sim.Hide();
                timer1.Stop();
                MessageBox.Show("No more data is available for both aircrafts.");
                return;
            }

            PointLatLng newPosition = new PointLatLng(lat, lon);

            // Verificar si el marcador ya está en el overlay y actualizar su posición solo si cambió
            if (lastPositions.ContainsKey(name) && lastPositions[name] == newPosition)
            {
                return; // La posición no cambió, no es necesario actualizar
            }
                

            // Actualizar o agregar la nueva posición en el diccionario
            lastPositions[name] = newPosition;

            // Actualizar marcador existente o agregar nuevo marcador al overlay
            GMapMarker existingMarker = aircraftOverlay.Markers.FirstOrDefault(m => m.Tag?.ToString() == name);

            if (existingMarker != null)
            {
                // Update the position of the existing marker
                MoveMarkerSmoothly(existingMarker, newPosition); // Call interpolation for smooth movement
            }
            else
            {
                Sim_diccionary.Add(name);
                GMapMarker marker = new GMarkerGoogle(newPosition, GMarkerGoogleType.blue_dot)
                {
                    Tag = name
                };
                aircraftOverlay.Markers.Add(marker);
            }

            // Si el overlay aún no se ha agregado al control, agrégalo
            if (!gMapControl1.Overlays.Contains(aircraftOverlay))
            {
                gMapControl1.Overlays.Add(aircraftOverlay);
            }

            gMapControl1.UpdateMarkerLocalPosition(existingMarker ?? aircraftOverlay.Markers.Last()); 
        }

        private void MoveMarkerSmoothly(GMapMarker marker, PointLatLng targetPosition)
        {
            // Calculate the distance in latitude and longitude between the current position and the target
            double latDiff = targetPosition.Lat - marker.Position.Lat;
            double lonDiff = targetPosition.Lng - marker.Position.Lng;

            // Define a lower speed factor to make the movement more gradual and precise
            double moveFactor = 0.12;

            // Ensure the difference values are not too small, or the movement will appear choppy
            if (Math.Abs(latDiff) > 0.000001 || Math.Abs(lonDiff) > 0.000001)
            {
                // Calculate the new position by applying the movement factor
                marker.Position = new PointLatLng(
                    marker.Position.Lat + latDiff * moveFactor,
                    marker.Position.Lng + lonDiff * moveFactor
                );
            }
            else
            {
                // If the difference is too small, place the marker directly at the target position
                marker.Position = targetPosition;
            }
        }


        // This function handles the Start/Stop simulation button click event
        int Click_times = 0;
        private void Start_sim_Click(object sender, EventArgs e)
        {
            // If the button text is "Start" or "Continue"
            if (Start_sim.Text == " Start" || Start_sim.Text == " Continue")
            {
                // Set up the "play" button image
                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(40, 40);
                imageList.Images.Add(Properties.Resources.play_button);
                Start_sim.Image = imageList.Images[0];
                Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
                Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
                Start_sim.ImageAlign = ContentAlignment.TopCenter;
                Start_sim.TextAlign = ContentAlignment.BottomCenter;

                // Button configuration for the first click
                if (Click_times == 0)
                {
                    // Set up the "pause" button image
                    ImageList imageListStop = new ImageList();
                    imageListStop.ImageSize = new Size(40, 40);
                    imageListStop.Images.Add(Properties.Resources.pause);
                    Start_sim.Image = imageListStop.Images[0];
                    Start_sim.Text = " Stop";

                    // Sort the 'bloque' data by the third element (index 2)
                    bloque = bloque.OrderBy(data => Convert.ToString(data[2])).ToList();

                    // Show the time control and start the timer
                    timeTXT.Show();
                    timer1.Start();

                    // Increment the click count and clear the simulation dictionary
                    Click_times++;
                    Sim_diccionary.Clear();
                    lastPositions.Clear();  
                }
                else
                {
                    // Set up the "pause" button image
                    ImageList imageListStop = new ImageList();
                    imageListStop.ImageSize = new Size(40, 40);
                    imageListStop.Images.Add(Properties.Resources.pause);
                    Start_sim.Image = imageListStop.Images[0];
                    Start_sim.Text = " Stop";
                    timer1.Start();
                }
            }
            else
            {
                // Set up the "play" button image when paused
                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(40, 40);
                imageList.Images.Add(Properties.Resources.play_button);
                Start_sim.Image = imageList.Images[0];
                Start_sim.ImageAlign = ContentAlignment.MiddleCenter;
                Start_sim.TextImageRelation = TextImageRelation.ImageAboveText;
                Start_sim.ImageAlign = ContentAlignment.TopCenter;
                Start_sim.TextAlign = ContentAlignment.BottomCenter;
                Start_sim.Text = " Continue";
                timer1.Stop();
            }
        }

        private void RestartSimBut_Click(object sender, EventArgs e)
        {
            // Stop the timer and reset simulation variables
            timer1.Stop();
            timeInicial = time[0];
            num_loop = 0;
            Click_times = 0;

            lastPositions.Clear(); 
            Sim_diccionary.Clear(); 
            aircraftOverlay.Markers.Clear(); 

            gMapControl1.Overlays.Clear();
            gMapControl1.ReloadMap();

            // Reset time text display
            timeTXT.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", (int)(timeInicial / 3600), (int)((timeInicial % 3600) / 60), (int)(timeInicial % 60));

            // Set up the "start" button image and text
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(40, 40);
            imageList.Images.Add(Properties.Resources.play_button);
            Start_sim.Image = imageList.Images[0];
            Start_sim.Text = " Start";
            Start_sim.Visible = true;

            // Reset timer interval and simulation speed
            timer1.Interval = 1000;
            Velocity_label_bar.Text = "Sim. Speed x1";
            trackBar1.Value = 0;

            int flag1 = 0;
            int flag2 = 0;
            int iter = 0;

            // Populate the data grid with initial aircraft data
            for (int i = 0; i < AircraftAddrList.Count; i++)
            {
                if (AircraftIDList[i].Trim() == Aircraft1.Trim() && flag1 == 0)
                {
                    dataGridView1.Rows[0].Cells[0].Value = AircraftAddrList[iter];
                    dataGridView1.Rows[1].Cells[0].Value = TrackNumList[iter];
                    dataGridView1.Rows[2].Cells[0].Value = Mode3AList[iter];
                    dataGridView1.Rows[3].Cells[0].Value = latitudList[iter];
                    dataGridView1.Rows[4].Cells[0].Value = longitudList[iter];
                    dataGridView1.Rows[5].Cells[0].Value = AltitudeList[iter];
                    dataGridView1.Rows[6].Cells[0].Value = SACList[iter];
                    dataGridView1.Rows[7].Cells[0].Value = SICList[iter];
                    flag1 = 1;
                }
                else if (AircraftIDList[i].Trim() == Aircraft2.Trim() && flag2 == 0)
                {
                    dataGridView1.Rows[0].Cells[1].Value = AircraftAddrList[iter];
                    dataGridView1.Rows[1].Cells[1].Value = TrackNumList[iter];
                    dataGridView1.Rows[2].Cells[1].Value = Mode3AList[iter];
                    dataGridView1.Rows[3].Cells[1].Value = latitudList[iter];
                    dataGridView1.Rows[4].Cells[1].Value = longitudList[iter];
                    dataGridView1.Rows[5].Cells[1].Value = AltitudeList[iter];
                    dataGridView1.Rows[6].Cells[1].Value = SACList[iter];
                    dataGridView1.Rows[7].Cells[1].Value = SICList[iter];
                    flag2 = 1;
                }
                iter++;
            }

            valueTXT.Text = "";
            valueNM.Text = "";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Get the current value of the trackbar (the velocity level selected by the user)
            int velocity = trackBar1.Value;
            switch (velocity)
            {
                case 0:
                case 1:
                    timer1.Interval = 1000;
                    Velocity_label_bar.Text = "Sim. Speed x1";
                    break;

                case 2:
                case 3:
                    timer1.Interval = 500;
                    Velocity_label_bar.Text = "Sim. Speed x2";
                    break;

                case 4:
                case 5:
                    timer1.Interval = 250;
                    Velocity_label_bar.Text = "Sim. Speed x4";
                    break;

                case 6:
                case 7:
                    timer1.Interval = 200;
                    Velocity_label_bar.Text = "Sim. Speed x5";
                    break;

                case 8:
                case 9:
                    timer1.Interval = 100;
                    Velocity_label_bar.Text = "Sim. Speed x10";
                    break;

                case 10:
                    timer1.Interval = 10;
                    Velocity_label_bar.Text = "Sim. Speed x100";
                    break;
            }
        }

        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            // Get the aircraft name from the marker's tag
            string name = item.Tag?.ToString();

            // Search for the corresponding marker within the single overlay
            GMapMarker existingMarker = aircraftOverlay.Markers.FirstOrDefault(m => m.Tag?.ToString() == name);

            // Find the index of the aircraft in AircraftIDList based on its name
            int indexAir = AircraftIDList.FindIndex(id => id == name);

            // Verify if the aircraft name exists in the simulation dictionary (Sim_diccionary)
            if (existingMarker != null && Sim_diccionary.Contains(name))
            {

                // Get the position of the existing marker on the map
                var position = existingMarker.Position;

                List<int> index_ID = new List<int>();
                for (int i = 0; i < AircraftIDList.Count; i++)
                {
                    if (AircraftIDList[i] == name)
                    {
                        index_ID.Add(i);
                    }
                }

                List<int> index_time = new List<int>();

                for (int i = 0; i < time.Count; i++)
                {
                    if (time[i] < timeInicial)
                    {
                        index_time.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }

                int indexAlt = indexAir; // Initial value to indicate no match was found

                List<int> index_sub = new List<int>();

                foreach (int id in index_ID)
                {
                    if (index_time.Contains(id))
                    {
                        index_sub.Add(id);
                    }
                }

                if (index_sub.Count == 0)
                {
                    indexAlt = indexAir;
                }
                else
                {
                    indexAlt = index_sub.Last();
                }

                // Prepare the information to display about the selected aircraft
                string info = $"Aircraft address: {AircraftAddrList[indexAir]}\n" +
                              $"Track number: {TrackNumList[indexAir]}\n" +
                              $"Mode 3A Reply: {Mode3AList[indexAir]}\n" +
                              $"\n" +
                              $"Lat: {position.Lat}º\n" +
                              $"Lon: {position.Lng}º\n" +
                              $"Altitude: {AltitudeList[indexAlt]} ft\n" +
                              $"\n" +
                              $"SAC: {SACList[indexAir]}\n" +
                              $"SIC: {SICList[indexAir]}\n";

                // Display the information in a message box with the aircraft name as the title
                MessageBox.Show(info, $"Aircraft identification: {name}");
            }
        }
    }
}
