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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Net;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Xml.Linq;
using MultiCAT6.Utils;
using System.Runtime.CompilerServices;
using ComboBox = System.Windows.Forms.ComboBox;
using Amazon.DirectoryService.Model;
using Button = System.Windows.Forms.Button;
using Amazon.DynamoDBv2.DocumentModel;
using System.Web.UI.WebControls;
using TrackBar = System.Windows.Forms.TrackBar;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;

namespace FormsAsterix
{
    public partial class DataGridView : Form
    {
        // Variables to store search folder, user computer information, and data structures
        string CarpetaBusqueda;
        Computer usr = new Computer();
        List<List<DataItem>> bloque = new List<List<DataItem>>(); // list for each block
        List<AsterixGrid> asterixGrids = new List<AsterixGrid>();
        BindingSource bindingSource = new BindingSource();
        bool filterEnabled = false;
        bool filterDisabled = false;
        private Dictionary<string, Dictionary_Info> originalColumnNames = new Dictionary<string, Dictionary_Info>();


        int index = 0;
        public int dgv_index { get; set; }

        // Constructor: Initializes the form with Asterix grid data
        public DataGridView(List<AsterixGrid> blok)
        {
            InitializeComponent();
            this.asterixGrids = blok;
            bindingSource.DataSource = asterixGrids;

            // Do DataGridView setup (customization for appearance)

            dataGridView2.ColumnHeadersVisible = true;

            dataGridView2.EnableHeadersVisualStyles = false;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkTurquoise;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.RowHeadersDefaultCellStyle.BackColor = Color.LightCyan;
            dataGridView2.RowHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView2.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = bindingSource;

            // List of column names to be displayed in the DataGridView
            List<string> lista = new List<string> {
            "Num","SAC", "SIC", "Time of Day", "Latitud", "Longitud", "Height","TYP", "SIM", "RDP", "SPI", "RAB", "TST", "ERR", "XPP", "ME",
            "MI", "FOE", "ADSBEP", "ADSBVAL", "SCNEP", "SCNVAL", "PAIEP", "PAIVAL", "RHO", "THETA", "Mode-3/A V", "Mode-3/A G",
            "Mode-3/A L", "Mode-3/A reply", "FL V", "FL G", "Flight level", "Modo C_corrected", "SRL", "SRR", "SAM", "PRL", "PAM", "RPD", "APD",
            "Aircraft address", "Aircraft Identification", "BDS4", "MCPU/FCU Selected altitude", "FMS Selected Altitude", "Barometric pressure setting",
             "VNAV", "ALTHOLD", "Approach", "Mode status","Target status", "Target altitude source","BDS_5_0", "Roll angle", "True track angle",
            "Track angle rate","True Airspeed","Ground Speed", "BDS_6_0","Magnetic heading", "Indicated airspeed", "Mach", "Barometric altitude rate",
            "Inertial Vertical Velocity", "Track Number", "X-Cartesian", "Y-Cartesian", "Calculated groundspeed", "Calculated heading",
            "CNF", "RAD", "DOU", "MAH", "CDM", "TRE", "GHO", "SUP", "TCC", "Height Measured by a 3D Radar", "COM", "STATUS",
            "SI", "MSSC", "ARC", "AIC", "B1A_message", "B1B_message"};


            // Set the column headers based on the provided list
            SetColumnHeaders(lista);

            this.WindowState = FormWindowState.Maximized;

        }

        // Sets the column headers for the DataGridView based on a provided list
        private void SetColumnHeaders(List<string> lista)
        {
            // Mapping of human-readable column names to the actual data property names
            Dictionary<string, string> columnMapping = new Dictionary<string, string>
            {
                { "Num", "Num" },
                     { "SAC", "SAC" },
                    { "SIC", "SIC" },
                    { "Time of Day", "Time" },
                    { "Latitud", "Latitude" },
                    { "Longitud", "Longitude" },
                    { "Height", "Height" },
                    { "TYP", "TYP" },
                    { "SIM", "SIM" },
                    { "RDP", "RDP" },
                    { "SPI", "SPI" },
                    { "RAB", "RAB" },
                    { "TST", "TST" },
                    { "ERR", "ERR" },
                    { "XPP", "XPP" },
                    { "ME", "ME" },
                    { "MI", "MI" },
                    { "FOE", "FOE" },
                    { "ADSBEP", "ADS_EP" },
                    { "ADSBVAL", "ADS_VAL" },
                    { "SCNEP", "SCN_EP" },
                    { "SCNVAL", "SCN_VAL" },
                    { "PAIEP", "PAI_EP" },
                    { "PAIVAL", "PAI_VAL" },
                    { "RHO", "Rho" },
                    { "THETA", "Theta" },
                    { "Mode-3/A V", "V_70" },
                    { "Mode-3/A G", "G_70" },
                    { "Mode-3/A L", "L_70" },
                    { "Mode-3/A reply", "Mode3_A_Reply" },
                    { "FL V", "V_90" },
                    { "FL G", "G_90" },
                    { "Flight level", "Flight_Level" },
                    { "Modo C_corrected", "Mode_C_Correction" },
                    { "SRL", "SRL" },
                    { "SRR", "SRR" },
                    { "SAM", "SAM" },
                    { "PRL", "PRL" },
                    { "PAM", "PAM" },
                    { "RPD", "RPD" },
                    { "APD", "APD" },
                    { "Aircraft address", "Aircraft_Address" },
                    { "Aircraft Identification", "Aircraft_Indentification" },
                    { "BDS4", "BDS_4_0" },
                    { "MCPU/FCU Selected altitude", "MCP_FCUtxt" },
                    { "FMS Selected Altitude", "FMStxt" },
                    { "Barometric pressure setting", "BARtxt" },
                    { "VNAV", "VNAVMODEtxt" },
                    { "ALTHOLD", "ALTHOLDtxt" },
                    { "Approach", "Approachtxt" },
                    { "Mode status", "Mode_stat_txt" },
                    { "Target status", "StatusTargAlt" },
                    { "Target altitude source", "TargetAltSourcetxt" },
                    { "BDS_5_0", "BDS_5_0" },
                    { "Roll angle", "Rolltxt" },
                    { "True track angle", "TrueTracktxt" },
                    { "Track angle rate", "TrackAngletxt" },
                    { "True Airspeed", "TrueAirspeedtxt" },
                    { "Ground Speed", "GroundSpeedtxt" },
                    { "BDS_6_0", "BDS_6_0" },
                    { "Magnetic heading", "MagHeadtxt" },
                    { "Indicated airspeed", "IndAirtxt" },
                    { "Mach", "MACHtxt" },
                    { "Barometric altitude rate", "BarAlttxt" },
                    { "Inertial Vertical Velocity", "InerVerttxt" },
                    { "Track Number", "Track_Number" },
                    { "X-Cartesian", "X_Component" },
                    { "Y-Cartesian", "Y_Component" },
                    { "Calculated groundspeed", "Ground_Speed" },
                    { "Calculated heading", "Heading" },
                    { "CNF", "CNF" },
                    { "RAD", "RAD" },
                    { "DOU", "DOU" },
                    { "MAH", "MAH" },
                    { "CDM", "CDM" },
                    { "TRE", "TRE" },
                    { "GHO", "GHO" },
                    { "SUP", "SUP" },
                    { "TCC", "TCC" },
                    { "Height Measured by a 3D Radar", "Height_3D" },
                    { "COM", "COM" },
                    { "STATUS", "STAT" },
                    { "SI", "SI" },
                    { "MSSC", "MSSC" },
                    { "ARC", "ARC" },
                    { "AIC", "AIC" },
                    { "B1A_message", "B1A" },
                    { "B1B_message", "B1B" }
            };
            dataGridView2.Columns.Clear();

            // Loop through each column name in the 'lista' (the list of column headers to display)
            foreach (var columnName in lista)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    // Set the header text of the column to the name from the list
                    HeaderText = columnName,
                    DataPropertyName = columnMapping.ContainsKey(columnName) ? columnMapping[columnName] : columnName
                };

                // Check if the columnName is not already in the originalColumnNames dictionary
                if (!originalColumnNames.ContainsKey(columnName))
                {
                    // Add the column name to the dictionary with a new Dictionary_Info object
                    originalColumnNames[columnName] = new Dictionary_Info(columnName);  
                }

                dataGridView2.Columns.Add(column);
            }
        }


        private void InitializeComponent()
        {
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bot2 = new System.Windows.Forms.ToolStripButton();
            this.bot5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.blancos_puros = new System.Windows.Forms.ToolStripButton();
            this.BtnFilter = new System.Windows.Forms.ToolStripButton();
            this.CSV_File = new System.Windows.Forms.ToolStripButton();
            this.Filtered_Values = new System.Windows.Forms.ToolStripButton();
            this.No_ground_flights = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidth = 51;
            this.dataGridView2.Size = new System.Drawing.Size(1484, 763);
            this.dataGridView2.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnFilter,
            this.bot2,
            this.CSV_File,
            this.bot5,
            this.Filtered_Values,
            this.toolStripButton2,
            this.No_ground_flights,
            this.toolStripButton3,
            this.blancos_puros});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1855, 39);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bot2
            // 
            this.bot2.BackColor = System.Drawing.Color.Transparent;
            this.bot2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bot2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bot2.Name = "bot2";
            this.bot2.Size = new System.Drawing.Size(29, 36);
            // 
            // bot5
            // 
            this.bot5.BackColor = System.Drawing.Color.Transparent;
            this.bot5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.bot5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bot5.Name = "bot5";
            this.bot5.Size = new System.Drawing.Size(29, 36);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.BackColor = System.Drawing.Color.Transparent;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(29, 36);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.BackColor = System.Drawing.Color.Transparent;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(29, 36);
            // 
            // blancos_puros
            // 
            this.blancos_puros.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.blancos_puros.Image = global::FormsAsterix.Properties.Resources.cancel;
            this.blancos_puros.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.blancos_puros.Name = "blancos_puros";
            this.blancos_puros.Size = new System.Drawing.Size(183, 36);
            this.blancos_puros.Text = "Eliminar blancos puros";
            this.blancos_puros.Visible = false;
            this.blancos_puros.Click += new System.EventHandler(this.blancos_puros_Click);
            // 
            // BtnFilter
            // 
            this.BtnFilter.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.BtnFilter.Image = global::FormsAsterix.Properties.Resources.filter;
            this.BtnFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnFilter.Name = "BtnFilter";
            this.BtnFilter.Size = new System.Drawing.Size(66, 36);
            this.BtnFilter.Text = "Filter";
            this.BtnFilter.Click += new System.EventHandler(this.BtnFilter_Click);
            // 
            // CSV_File
            // 
            this.CSV_File.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.CSV_File.Image = global::FormsAsterix.Properties.Resources.csv;
            this.CSV_File.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CSV_File.Name = "CSV_File";
            this.CSV_File.Size = new System.Drawing.Size(86, 36);
            this.CSV_File.Text = "CSV File";
            this.CSV_File.Click += new System.EventHandler(this.CSV_File_Click);
            // 
            // Filtered_Values
            // 
            this.Filtered_Values.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Filtered_Values.Image = global::FormsAsterix.Properties.Resources.statistics;
            this.Filtered_Values.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Filtered_Values.Name = "Filtered_Values";
            this.Filtered_Values.Size = new System.Drawing.Size(129, 36);
            this.Filtered_Values.Text = "Filtered Values";
            this.Filtered_Values.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Filtered_Values.ToolTipText = "Filtered Values";
            this.Filtered_Values.Visible = false;
            this.Filtered_Values.Click += new System.EventHandler(this.Filtered_Values_Click);
            // 
            // No_ground_flights
            // 
            this.No_ground_flights.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.No_ground_flights.Image = global::FormsAsterix.Properties.Resources.airport;
            this.No_ground_flights.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.No_ground_flights.Name = "No_ground_flights";
            this.No_ground_flights.Size = new System.Drawing.Size(213, 36);
            this.No_ground_flights.Text = "Eliminate on ground flights";
            this.No_ground_flights.Visible = false;
            this.No_ground_flights.Click += new System.EventHandler(this.No_ground_flights_Click);
            // 
            // DataGridView
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1484, 763);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView2);
            this.Name = "DataGridView";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        
        private void DataGridView_Load(object sender, EventArgs e)
        {
            if (dgv_index == 0) 
            {
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e)
        {
            // Make the Filtered_Values, No_ground_flights, and blancos_puros controls visible.
            Filtered_Values.Visible = true;
            No_ground_flights.Visible = true;
            blancos_puros.Visible = true;
            
            // Bring the toolStrip1 to the front of other controls, making it visible above other elements.
            toolStrip1.BringToFront();

            // Toggle the state of the filterEnabled variable between true and false.
            filterEnabled = !filterEnabled;

            // If filter is enabled (i.e., filterEnabled is true):
            if (filterEnabled)
            {
                // Add arrows to column headers to indicate sorting or filtering.
                AddArrowToColumnHeaders(true);
                dataGridView2.ColumnHeaderMouseClick += DataGridView2_ColumnHeaderMouseClick;         
            }
            else
            {
                // Remove the arrows from the column headers when filtering is disabled.
                AddArrowToColumnHeaders(false);
                ResetForm();
            }

            // Refresh the DataGridView to reflect any changes in its UI.
            dataGridView2.Refresh();
        }

        // Dictionary to store filter controls for each column, mapped by column name.
        private Dictionary<string, List<Control>> filterControls = new Dictionary<string, List<Control>>();

        // Event handler for when a column header is clicked.
        private void DataGridView2_ColumnHeaderMouseClick (object sender, DataGridViewCellMouseEventArgs e)
        {
            // Check if filtering is enabled
            if (filterEnabled)
            {
                filterDisabled = !filterDisabled;
                string columna = dataGridView2.Columns[e.ColumnIndex].DataPropertyName;
                if (filterDisabled)
                {
                    ShowFilterBox(columna, e.ColumnIndex);
                }
                else
                {
                    // If the filter is not visible, we remove it
                    RemoveFilterControls(columna);
                }

            }

        }

        // Method to remove filter controls for a specific column
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
                    dataGridView2.Controls.Remove(control);
                    control.Dispose();
                }

                // Remove the entry from the filterControls dictionary
                filterControls.Remove(columna);
            }
        }

        // Method to get distinct values from a specific column in the DataGridView
        private IEnumerable<object> GetDistinctValues(DataGridViewColumn column)
        {
            return dataGridView1.Rows.Cast<DataGridViewRow>()
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
            string propertyName = dataGridView2.Columns[index].DataPropertyName;
            var uniques = asterixGrids.Select(x => x.GetType().GetProperty(propertyName)?.GetValue(x)?.ToString())
                                       .Distinct()
                                       .Where(val => val != null)
                                       .ToList();
            Rectangle headerRect = dataGridView2.GetCellDisplayRectangle(index, -1, true);
            var specificColumns = new List<string> { "Latitude", "Longitude", "Height", "Flight_Level" };
            List<Control> controlsToAdd = new List<Control>();

            // If there are more than 10 unique values, show a control to select a range
            if (specificColumns.Contains(columna))
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
                        RemoveFilterControls(columna);
                        dataGridView2.Controls.Remove(minLabel);
                        dataGridView2.Controls.Remove(maxLabel);
                        dataGridView2.Controls.Remove(minValueLabel);
                        dataGridView2.Controls.Remove(maxValueLabel);
                        dataGridView2.Controls.Remove(minTrackBar);
                        dataGridView2.Controls.Remove(maxTrackBar);
                        dataGridView2.Controls.Remove(acceptButton);

                    };

                    // Add controls to the DataGridView so they are displayed on the screen
                    dataGridView2.Controls.Add(minLabel);
                    dataGridView2.Controls.Add(maxLabel);
                    dataGridView2.Controls.Add(minValueLabel);
                    dataGridView2.Controls.Add(maxValueLabel);
                    dataGridView2.Controls.Add(minTrackBar);
                    dataGridView2.Controls.Add(maxTrackBar);
                    dataGridView2.Controls.Add(acceptButton);

                    // Bring each control to the front so they are visible
                    minLabel.BringToFront();
                    maxLabel.BringToFront();
                    minValueLabel.BringToFront();
                    maxValueLabel.BringToFront();
                    minTrackBar.BringToFront();
                    maxTrackBar.BringToFront();
                    acceptButton.BringToFront();

                    dataGridView2.Controls.AddRange(new Control[] { minLabel, maxLabel, minValueLabel, maxValueLabel, minTrackBar, maxTrackBar, acceptButton });
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

                    dataGridView2.Controls.Add(minValueTextBox);
                    dataGridView2.Controls.Add(maxValueTextBox);
                    dataGridView2.Controls.Add(acceptButton);
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
                    Width = dataGridView2.Columns[index].Width,
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
                dataGridView2.Controls.Add(filterBox);
                dataGridView2.Controls.Add(acceptButton);

                // Track the controls for this column
                controlsToAdd.AddRange(new Control[] { filterBox, acceptButton });

                // Bring controls to the front
                filterBox.BringToFront();
                acceptButton.BringToFront();
            }

            // Store the controls associated with the current column for later management
            filterControls[columna] = controlsToAdd;

        }

        public void ApplyFilter ()
        {
            // Filter the data based on the selected filters
            var filteredData = asterixGrids.Where(x =>
            {
                foreach (var filter in selectedFilters)
                {
                    var propertyValue = x.GetType().GetProperty(filter.Key)?.GetValue(x)?.ToString();
                    if (propertyValue == null) return false;

                    // Determine the type of filter
                    if (filter.Value is List<string> stringValues)
                    {
                        // Filter for selected values (CheckedListBox)
                        if (!stringValues.Contains(propertyValue.Trim(), StringComparer.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                    else if (filter.Value is List<double> numericRange)
                    {
                        // Filter for numeric range
                        if (double.TryParse(propertyValue, out double numericValue))
                        {
                            double minRange = numericRange[0];
                            double maxRange = numericRange[1];
                            if (numericValue < minRange || numericValue > maxRange)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
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

            // Apply the filtered data to a new DataGridView
            BindingSource newBindingSource = new BindingSource();
            newBindingSource.DataSource = filteredData;

            // Create a new DataGridView and show the filtered results
            DataGridFiltrado newDataGridView = new DataGridFiltrado(newBindingSource);
            this.Hide();
            newDataGridView.ShowDialog();

            // Reset the form to its original state
            ResetForm();
        }
        private void RemoveFilterBoxes()
        {
            // Remove all controls of type CheckedListBox or Button from the DataGridView
            foreach (Control control in dataGridView2.Controls)
            {
                if (control is CheckedListBox || control is Button)
                {
                    control.Dispose();
                }
            }
        }
     

        private void AddArrowToColumnHeaders(bool showArrows)
        {
            dataGridView2.ColumnHeadersVisible = true;

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                string headerText = column.HeaderText;

                // Check if the headerText contains an arrow and get the name without the arrow for comparison
                string columnNameToCompare = headerText.EndsWith(" ⬇")
                    ? headerText.Substring(0, headerText.Length-2)  // Eliminar " ⬇"
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
            dataGridView2.Refresh();
        }

        private void ResetForm()
        {
            dataGridView2.SuspendLayout();

            // Remove all subscribed events (to disable any filtering in action)
            dataGridView2.ColumnHeaderMouseClick -= DataGridView2_ColumnHeaderMouseClick;

            // Remove any applied filters
            bindingSource.RemoveFilter();
            selectedFilters.Clear();

            // Restore the original data (this ensures no filters are applied)
            dataGridView2.DataSource = bindingSource;

            // Hide the "Filtered_Values" button and other controls related to filtering
            Filtered_Values.Visible = false;
            No_ground_flights.Visible = false;
            blancos_puros.Visible = false;

            // Remove any CheckedListBox that might still be visible
            RemoveFilterBoxes();

            // Ensure filtering arrows are removed from column headers
            AddArrowToColumnHeaders(false);
            dataGridView2.ColumnHeadersVisible = true;
            dataGridView2.ResumeLayout();

            // If you need to show the form again in a "clean" state:
            this.Show();
            
        }



        private void Filtered_Values_Click(object sender, EventArgs e)
        {
            // Trigger the application of the filters configured by the user
            ApplyFilter();
        }

        private void CSV_File_Click(object sender, EventArgs e)
        {
            // Open a SaveFileDialog to allow the user to select the file path and name for the CSV
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.Title = "Seleccionar la ubicación y el nombre del fichero";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // Write the data to the selected file
                    EscribirFichero(filePath);
                    MessageBox.Show("S'ha escrit el fitxer correctament");
                }
            }
        }
        private void EscribirFichero(string filePath)
        {
            // Generate a CSV file with the data from the DataGridView
            StringBuilder csvfile = new StringBuilder();

            // Write column headers
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                csvfile.Append(dataGridView1.Columns[i].HeaderText);

                if (i < dataGridView1.Columns.Count - 1)
                {
                    csvfile.Append(";"); // Add a delimiter between columns
                }

            }
            csvfile.AppendLine();

            // Write rows data
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    csvfile.Append(row.Cells[i].Value?.ToString());

                    if (i < dataGridView1.Columns.Count - 1)
                    {
                        csvfile.Append(";"); // Add a delimiter between columns
                    }

                }
                csvfile.AppendLine();
            }

            // Save the CSV content to the specified file path
            File.WriteAllText(filePath, csvfile.ToString(), Encoding.UTF8);
        }

        private void No_ground_flights_Click(object sender, EventArgs e)
        {
            // Remove rows where the flight level is "N/A"
            RemoveRowsWithNAInFlightLevel();
        }

        private void RemoveRowsWithNAInFlightLevel()
        {
            // Create a list to store the filtered rows
            var filteredData = new List<object>();

            // Iterate over the rows in the DataGridView
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                var flightLevel = row.Cells[32].Value?.ToString();

                // Add rows to the list only if the flight level is not "N/A"
                if (flightLevel != "N/A")
                {
                    filteredData.Add(row.DataBoundItem); 
                }
            }

            // If no rows remain after filtering
            if (filteredData.Count == 0)
            {
                MessageBox.Show("No hay datos después de aplicar el filtro.");
                return;
            }

            // Create a new BindingSource with the filtered data
            BindingSource newBindingSource = new BindingSource();
            newBindingSource.DataSource = filteredData;

            // Open a new DataGridView to display the filtered data
            DataGridFiltrado newDataGridView = new DataGridFiltrado(newBindingSource);
            this.Hide();
            newDataGridView.ShowDialog();

            // Reset the form to its original state
            ResetForm();
        }
        private void RemoveRowsWithInvalidTYP()
        {
            // Filter rows based on the TYP column's value
            var filteredData = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                var typValue = row.Cells[7].Value?.ToString();  // Verify the column index is correct

                // Add rows where TYP matches specific values
                if (typValue == "Single ModeS All-Call" || typValue == "Single ModeS Roll-Call" || typValue == "ModeS All-Call + PSR" || typValue == "ModeS Roll-Call +PSR")
                {
                    filteredData.Add(row);  // Agregar a la lista de filas filtradas
                }
            }

            // If filtered data is available, display it in a new DataGridView
            if (filteredData.Count > 0)
            {
                OpenFilteredDataGridForm(filteredData);
            }
            else
            {
                MessageBox.Show("No hay datos que coincidan con el filtro.");
            }
        }

        private void OpenFilteredDataGridForm(List<DataGridViewRow> filteredData)
        {
            // Filter data from the original list (e.g., asterixGrids) to include only matching rows
            DataTable filteredDataTable = new DataTable();

            // Add columns to the new DataTable
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                filteredDataTable.Columns.Add(column.HeaderText);  // Use column.HeaderText or column.Name
            }

            // Add the filtered rows to the DataTable
            foreach (var row in filteredData)
            {
                DataRow dataRow = filteredDataTable.NewRow();
                for (int i = 0; i < dataGridView2.Columns.Count; i++)
                {
                    dataRow[i] = row.Cells[i].Value;  // Copy cell values to the new row
                }
                filteredDataTable.Rows.Add(dataRow);
            }

            // Create a new BindingSource for the filtered data
            BindingSource newBindingSource = new BindingSource();
            newBindingSource.DataSource = filteredDataTable;

            // Open a new DataGridView to display the filtered data
            DataGridFiltrado newDataGridView = new DataGridFiltrado(newBindingSource);
            this.Hide();
            newDataGridView.ShowDialog();

            // Reset the form to its original state
            ResetForm();
        }

        private void blancos_puros_Click(object sender, EventArgs e)
        {
            // Remove rows that do not have valid TYP values
            RemoveRowsWithInvalidTYP();
        }
    }
}
