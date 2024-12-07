using System.Drawing;
using System.Windows.Forms;

namespace FormsAsterix
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.AcceptBut = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.A2_IDbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.A1_IDbox = new System.Windows.Forms.TextBox();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.Velocity_label_bar = new System.Windows.Forms.Label();
            this.Start_sim = new System.Windows.Forms.Button();
            this.timeTXT = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.GetKMLBut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.RestartSimBut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.NewDataBut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.ShowDataBut = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.CSV_File = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.Project3Btn = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TitolTXT = new System.Windows.Forms.Label();
            this.DescodBUT = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.gMapControl1);
            this.groupBox2.Controls.Add(this.Velocity_label_bar);
            this.groupBox2.Controls.Add(this.Start_sim);
            this.groupBox2.Controls.Add(this.timeTXT);
            this.groupBox2.Controls.Add(this.trackBar1);
            this.groupBox2.Controls.Add(this.toolStrip1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(1251, 651);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.AcceptBut);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.A2_IDbox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.A1_IDbox);
            this.groupBox3.Location = new System.Drawing.Point(57, 374);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(236, 225);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.UseWaitCursor = true;
            this.groupBox3.Visible = false;
            // 
            // AcceptBut
            // 
            this.AcceptBut.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AcceptBut.Location = new System.Drawing.Point(72, 158);
            this.AcceptBut.Name = "AcceptBut";
            this.AcceptBut.Size = new System.Drawing.Size(88, 39);
            this.AcceptBut.TabIndex = 8;
            this.AcceptBut.Text = "Accept";
            this.AcceptBut.UseVisualStyleBackColor = true;
            this.AcceptBut.UseWaitCursor = true;
            this.AcceptBut.Click += new System.EventHandler(this.AcceptBut_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(17, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Aircraft_2 ID:";
            this.label2.UseWaitCursor = true;
            // 
            // A2_IDbox
            // 
            this.A2_IDbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.A2_IDbox.Location = new System.Drawing.Point(18, 113);
            this.A2_IDbox.Name = "A2_IDbox";
            this.A2_IDbox.Size = new System.Drawing.Size(205, 22);
            this.A2_IDbox.TabIndex = 9;
            this.A2_IDbox.UseWaitCursor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(15, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Aircraft_1 ID:";
            this.label1.UseWaitCursor = true;
            // 
            // A1_IDbox
            // 
            this.A1_IDbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.A1_IDbox.Location = new System.Drawing.Point(16, 51);
            this.A1_IDbox.Name = "A1_IDbox";
            this.A1_IDbox.Size = new System.Drawing.Size(205, 22);
            this.A1_IDbox.TabIndex = 0;
            this.A1_IDbox.UseWaitCursor = true;
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.AutoSize = true;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(349, 62);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(890, 577);
            this.gMapControl1.TabIndex = 6;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl1_OnMarkerClick);
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            // 
            // Velocity_label_bar
            // 
            this.Velocity_label_bar.AutoSize = true;
            this.Velocity_label_bar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Velocity_label_bar.Location = new System.Drawing.Point(101, 266);
            this.Velocity_label_bar.Name = "Velocity_label_bar";
            this.Velocity_label_bar.Size = new System.Drawing.Size(126, 22);
            this.Velocity_label_bar.TabIndex = 5;
            this.Velocity_label_bar.Text = "Sim. Speed x1";
            // 
            // Start_sim
            // 
            this.Start_sim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Start_sim.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Start_sim.ForeColor = System.Drawing.Color.Black;
            this.Start_sim.Location = new System.Drawing.Point(110, 85);
            this.Start_sim.Name = "Start_sim";
            this.Start_sim.Size = new System.Drawing.Size(120, 82);
            this.Start_sim.TabIndex = 4;
            this.Start_sim.Text = " Run";
            this.Start_sim.UseVisualStyleBackColor = true;
            this.Start_sim.Click += new System.EventHandler(this.Start_sim_Click);
            // 
            // timeTXT
            // 
            this.timeTXT.AutoSize = true;
            this.timeTXT.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeTXT.Location = new System.Drawing.Point(124, 187);
            this.timeTXT.Name = "timeTXT";
            this.timeTXT.Size = new System.Drawing.Size(55, 28);
            this.timeTXT.TabIndex = 2;
            this.timeTXT.Text = "time";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(34, 218);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(277, 56);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GetKMLBut,
            this.toolStripButton3,
            this.RestartSimBut,
            this.toolStripButton4,
            this.NewDataBut,
            this.toolStripButton5,
            this.ShowDataBut,
            this.toolStripButton1,
            this.CSV_File,
            this.toolStripButton2,
            this.toolStripButton6,
            this.toolStripButton7,
            this.Project3Btn});
            this.toolStrip1.Location = new System.Drawing.Point(3, 17);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1245, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // GetKMLBut
            // 
            this.GetKMLBut.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.GetKMLBut.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GetKMLBut.Image = global::FormsAsterix.Properties.Resources.kml_file_format_variant;
            this.GetKMLBut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GetKMLBut.Name = "GetKMLBut";
            this.GetKMLBut.Size = new System.Drawing.Size(94, 24);
            this.GetKMLBut.Text = "Get KML";
            this.GetKMLBut.ToolTipText = "Get KML";
            this.GetKMLBut.Click += new System.EventHandler(this.GetKMLBut_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton3.Text = "   ";
            // 
            // RestartSimBut
            // 
            this.RestartSimBut.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.RestartSimBut.BackgroundImage = global::FormsAsterix.Properties.Resources.restart;
            this.RestartSimBut.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RestartSimBut.Image = global::FormsAsterix.Properties.Resources.restart;
            this.RestartSimBut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RestartSimBut.Name = "RestartSimBut";
            this.RestartSimBut.Size = new System.Drawing.Size(114, 24);
            this.RestartSimBut.Text = "Restart Sim";
            this.RestartSimBut.Click += new System.EventHandler(this.RestartSimBut_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(29, 24);
            // 
            // NewDataBut
            // 
            this.NewDataBut.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.NewDataBut.BackgroundImage = global::FormsAsterix.Properties.Resources.d2fcf1c06735d7899e5e0766edd9498e1;
            this.NewDataBut.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewDataBut.Image = global::FormsAsterix.Properties.Resources.folder;
            this.NewDataBut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewDataBut.Name = "NewDataBut";
            this.NewDataBut.Size = new System.Drawing.Size(102, 24);
            this.NewDataBut.Text = "New Data";
            this.NewDataBut.Click += new System.EventHandler(this.NewDataBut_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton5.Text = "toolStripButton5";
            // 
            // ShowDataBut
            // 
            this.ShowDataBut.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ShowDataBut.BackgroundImage = global::FormsAsterix.Properties.Resources.file_statistics_linear_icon_thin_line_illustration_document_with_diagram_contour_symbol_isolated_outline_drawing_vector;
            this.ShowDataBut.Image = global::FormsAsterix.Properties.Resources.sheet;
            this.ShowDataBut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowDataBut.Name = "ShowDataBut";
            this.ShowDataBut.Size = new System.Drawing.Size(108, 24);
            this.ShowDataBut.Text = "Show Data";
            this.ShowDataBut.Click += new System.EventHandler(this.ShowDataBut_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CSV_File
            // 
            this.CSV_File.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.CSV_File.Image = global::FormsAsterix.Properties.Resources.csv;
            this.CSV_File.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CSV_File.Name = "CSV_File";
            this.CSV_File.Size = new System.Drawing.Size(88, 24);
            this.CSV_File.Text = "CSV File";
            this.CSV_File.Click += new System.EventHandler(this.CSV_File_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(29, 24);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.toolStripButton6.Image = global::FormsAsterix.Properties.Resources.compass;
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(176, 24);
            this.toolStripButton6.Text = "Horitzontal Distance";
            this.toolStripButton6.Click += new System.EventHandler(this.toolStripButton6_Click);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(29, 24);
            this.toolStripButton7.Text = "toolStripButton7";
            // 
            // Project3Btn
            // 
            this.Project3Btn.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Project3Btn.Image = global::FormsAsterix.Properties.Resources.control_tower;
            this.Project3Btn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Project3Btn.Name = "Project3Btn";
            this.Project3Btn.Size = new System.Drawing.Size(95, 24);
            this.Project3Btn.Text = "Project 3";
            this.Project3Btn.Click += new System.EventHandler(this.Project3Btn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.BackgroundImage = global::FormsAsterix.Properties.Resources.fondo;
            this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.groupBox1.Controls.Add(this.TitolTXT);
            this.groupBox1.Controls.Add(this.DescodBUT);
            this.groupBox1.Location = new System.Drawing.Point(0, -14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(1248, 661);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // TitolTXT
            // 
            this.TitolTXT.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TitolTXT.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.TitolTXT.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitolTXT.Location = new System.Drawing.Point(195, 222);
            this.TitolTXT.Name = "TitolTXT";
            this.TitolTXT.Size = new System.Drawing.Size(840, 72);
            this.TitolTXT.TabIndex = 15;
            this.TitolTXT.Text = "ASTERIX Hackathon - P2 i P3 PGTA";
            this.TitolTXT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DescodBUT
            // 
            this.DescodBUT.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DescodBUT.BackColor = System.Drawing.SystemColors.ControlLight;
            this.DescodBUT.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescodBUT.Location = new System.Drawing.Point(456, 316);
            this.DescodBUT.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DescodBUT.Name = "DescodBUT";
            this.DescodBUT.Size = new System.Drawing.Size(296, 120);
            this.DescodBUT.TabIndex = 13;
            this.DescodBUT.Text = "Seleccionar fitxer .ast a descodificar";
            this.DescodBUT.UseVisualStyleBackColor = false;
            this.DescodBUT.Click += new System.EventHandler(this.DescodBUT_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1251, 651);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button DescodBUT;
        private Label TitolTXT;
        private GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
        private GroupBox groupBox2;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private Label Velocity_label_bar;
        private Button Start_sim;
        private Label timeTXT;
        private TrackBar trackBar1;
        private ToolStrip toolStrip1;
        private ToolStripButton GetKMLBut;
        private ToolStripButton toolStripButton3;
        private ToolStripButton RestartSimBut;
        private ToolStripButton toolStripButton4;
        private ToolStripButton NewDataBut;
        private ToolStripButton toolStripButton5;
        private ToolStripButton ShowDataBut;
        private ToolStripButton toolStripButton1;
        private ToolStripButton CSV_File;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton6;
        private GroupBox groupBox3;
        private Button AcceptBut;
        private Label label2;
        private TextBox A2_IDbox;
        private Label label1;
        private TextBox A1_IDbox;
        private ToolStripButton toolStripButton7;
        private ToolStripButton Project3Btn;
    }
}
