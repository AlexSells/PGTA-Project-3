namespace FormsAsterix
{
    partial class GeneralStatistics
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
            this.GenStatisticsGird = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.IncidentStatsGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.GenStatisticsGird)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncidentStatsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // GenStatisticsGird
            // 
            this.GenStatisticsGird.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GenStatisticsGird.Location = new System.Drawing.Point(29, 105);
            this.GenStatisticsGird.Name = "GenStatisticsGird";
            this.GenStatisticsGird.RowHeadersWidth = 51;
            this.GenStatisticsGird.RowTemplate.Height = 24;
            this.GenStatisticsGird.Size = new System.Drawing.Size(407, 223);
            this.GenStatisticsGird.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(23, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "General Statistics:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(489, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(313, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "Inciendents Statistics:";
            // 
            // IncidentStatsGrid
            // 
            this.IncidentStatsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.IncidentStatsGrid.Location = new System.Drawing.Point(495, 105);
            this.IncidentStatsGrid.Name = "IncidentStatsGrid";
            this.IncidentStatsGrid.RowHeadersWidth = 51;
            this.IncidentStatsGrid.RowTemplate.Height = 24;
            this.IncidentStatsGrid.Size = new System.Drawing.Size(407, 223);
            this.IncidentStatsGrid.TabIndex = 2;
            // 
            // GeneralStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.IncidentStatsGrid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GenStatisticsGird);
            this.Name = "GeneralStatistics";
            this.Text = "GeneralStatistics";
            this.Load += new System.EventHandler(this.GeneralStatistics_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GenStatisticsGird)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncidentStatsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView GenStatisticsGird;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView IncidentStatsGrid;
    }
}