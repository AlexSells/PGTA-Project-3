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
            ((System.ComponentModel.ISupportInitialize)(this.GenStatisticsGird)).BeginInit();
            this.SuspendLayout();
            // 
            // GenStatisticsGird
            // 
            this.GenStatisticsGird.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GenStatisticsGird.Location = new System.Drawing.Point(12, 63);
            this.GenStatisticsGird.Name = "GenStatisticsGird";
            this.GenStatisticsGird.RowHeadersWidth = 51;
            this.GenStatisticsGird.RowTemplate.Height = 24;
            this.GenStatisticsGird.Size = new System.Drawing.Size(327, 245);
            this.GenStatisticsGird.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "General Statistics";
            // 
            // GeneralStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GenStatisticsGird);
            this.Name = "GeneralStatistics";
            this.Text = "GeneralStatistics";
            this.Load += new System.EventHandler(this.GeneralStatistics_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GenStatisticsGird)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView GenStatisticsGird;
        private System.Windows.Forms.Label label1;
    }
}