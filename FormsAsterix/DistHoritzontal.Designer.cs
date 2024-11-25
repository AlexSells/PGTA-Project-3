namespace FormsAsterix
{
    partial class DistHoritzontal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.Velocity_label_bar = new System.Windows.Forms.Label();
            this.Start_sim = new System.Windows.Forms.Button();
            this.timeTXT = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.RestartSimBut = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.valueTXT = new System.Windows.Forms.Label();
            this.valueNM = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
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
            this.gMapControl1.Location = new System.Drawing.Point(663, 54);
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
            this.gMapControl1.Size = new System.Drawing.Size(861, 699);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMapControl1_OnMarkerClick);
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            // 
            // Velocity_label_bar
            // 
            this.Velocity_label_bar.AutoSize = true;
            this.Velocity_label_bar.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Velocity_label_bar.Location = new System.Drawing.Point(247, 243);
            this.Velocity_label_bar.Name = "Velocity_label_bar";
            this.Velocity_label_bar.Size = new System.Drawing.Size(126, 22);
            this.Velocity_label_bar.TabIndex = 9;
            this.Velocity_label_bar.Text = "Sim. Speed x1";
            // 
            // Start_sim
            // 
            this.Start_sim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Start_sim.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Start_sim.ForeColor = System.Drawing.Color.Black;
            this.Start_sim.Location = new System.Drawing.Point(256, 62);
            this.Start_sim.Name = "Start_sim";
            this.Start_sim.Size = new System.Drawing.Size(120, 82);
            this.Start_sim.TabIndex = 8;
            this.Start_sim.Text = " Run";
            this.Start_sim.UseVisualStyleBackColor = true;
            this.Start_sim.Click += new System.EventHandler(this.Start_sim_Click);
            // 
            // timeTXT
            // 
            this.timeTXT.AutoSize = true;
            this.timeTXT.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeTXT.Location = new System.Drawing.Point(270, 164);
            this.timeTXT.Name = "timeTXT";
            this.timeTXT.Size = new System.Drawing.Size(55, 28);
            this.timeTXT.TabIndex = 7;
            this.timeTXT.Text = "time";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(180, 195);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(277, 56);
            this.trackBar1.TabIndex = 6;
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
            this.RestartSimBut});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1552, 27);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "toolStrip1";
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
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(24, 334);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(597, 228);
            this.dataGridView1.TabIndex = 11;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(92, 612);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(205, 28);
            this.label1.TabIndex = 12;
            this.label1.Text = "Horizontal Distance:";
            // 
            // valueTXT
            // 
            this.valueTXT.AutoSize = true;
            this.valueTXT.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueTXT.Location = new System.Drawing.Point(170, 648);
            this.valueTXT.Name = "valueTXT";
            this.valueTXT.Size = new System.Drawing.Size(59, 25);
            this.valueTXT.TabIndex = 13;
            this.valueTXT.Text = "value";
            // 
            // valueNM
            // 
            this.valueNM.AutoSize = true;
            this.valueNM.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valueNM.Location = new System.Drawing.Point(170, 687);
            this.valueNM.Name = "valueNM";
            this.valueNM.Size = new System.Drawing.Size(59, 25);
            this.valueNM.TabIndex = 14;
            this.valueNM.Text = "value";
            // 
            // DistHoritzontal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1552, 765);
            this.Controls.Add(this.valueNM);
            this.Controls.Add(this.valueTXT);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.Velocity_label_bar);
            this.Controls.Add(this.Start_sim);
            this.Controls.Add(this.timeTXT);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.gMapControl1);
            this.Name = "DistHoritzontal";
            this.Text = "DistHoritzontal";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Label Velocity_label_bar;
        private System.Windows.Forms.Button Start_sim;
        private System.Windows.Forms.Label timeTXT;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton RestartSimBut;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label valueTXT;
        private System.Windows.Forms.Label valueNM;
    }
}