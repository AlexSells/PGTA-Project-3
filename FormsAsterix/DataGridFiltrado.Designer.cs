using System.Drawing;
using System.Windows.Forms;


namespace FormsAsterix
{
    partial class DataGridFiltrado
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataGridFiltrado));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Fitxer_CSV = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.DataGrid_No_Filter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DataGrid_No_Filter,
            this.toolStripButton1,
            this.Fitxer_CSV});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1000, 39);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Fitxer_CSV
            // 
            this.Fitxer_CSV.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Fitxer_CSV.Image = global::FormsAsterix.Properties.Resources.csv;
            this.Fitxer_CSV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Fitxer_CSV.Name = "Fitxer_CSV";
            this.Fitxer_CSV.Size = new System.Drawing.Size(99, 36);
            this.Fitxer_CSV.Text = "Fitxer CSV";
            this.Fitxer_CSV.Click += new System.EventHandler(this.Fitxer_CSV_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(0, 24);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(800, 338);
            this.dataGridView1.TabIndex = 1;
            // 
            // DataGrid_No_Filter
            // 
            this.DataGrid_No_Filter.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.DataGrid_No_Filter.Image = global::FormsAsterix.Properties.Resources.back;
            this.DataGrid_No_Filter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DataGrid_No_Filter.Name = "DataGrid_No_Filter";
            this.DataGrid_No_Filter.Size = new System.Drawing.Size(235, 36);
            this.DataGrid_No_Filter.Text = "Tornar al DataGrid sense filtrar";
            this.DataGrid_No_Filter.Click += new System.EventHandler(this.DataGrid_No_Filter_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.BackColor = System.Drawing.Color.Transparent;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(29, 36);
            // 
            // DataGridFiltrado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 360);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "DataGridFiltrado";
            this.Text = "DataGridFiltrado";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton DataGrid_No_Filter;
        private ToolStripButton toolStripButton1;
        private ToolStripButton Fitxer_CSV;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}