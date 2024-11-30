namespace FormsAsterix
{
    partial class IASInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IASInformation));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Back2P3 = new System.Windows.Forms.ToolStripButton();
            this.IASdatagrid = new System.Windows.Forms.DataGridView();
            this.Btn850 = new System.Windows.Forms.Button();
            this.Btn1500 = new System.Windows.Forms.Button();
            this.Btn3500 = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IASdatagrid)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Back2P3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 27);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Back2P3
            // 
            this.Back2P3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Back2P3.Image = ((System.Drawing.Image)(resources.GetObject("Back2P3.Image")));
            this.Back2P3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Back2P3.Name = "Back2P3";
            this.Back2P3.Size = new System.Drawing.Size(102, 24);
            this.Back2P3.Text = "Back to P3";
            this.Back2P3.Click += new System.EventHandler(this.Back2P3_Click);
            // 
            // IASdatagrid
            // 
            this.IASdatagrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.IASdatagrid.Location = new System.Drawing.Point(147, 100);
            this.IASdatagrid.Name = "IASdatagrid";
            this.IASdatagrid.RowHeadersWidth = 51;
            this.IASdatagrid.RowTemplate.Height = 24;
            this.IASdatagrid.Size = new System.Drawing.Size(419, 250);
            this.IASdatagrid.TabIndex = 1;
            // 
            // Btn850
            // 
            this.Btn850.Location = new System.Drawing.Point(12, 100);
            this.Btn850.Name = "Btn850";
            this.Btn850.Size = new System.Drawing.Size(100, 50);
            this.Btn850.TabIndex = 2;
            this.Btn850.Text = "Load 850 ft";
            this.Btn850.UseVisualStyleBackColor = true;
            this.Btn850.Click += new System.EventHandler(this.Btn850_Click);
            // 
            // Btn1500
            // 
            this.Btn1500.Location = new System.Drawing.Point(12, 200);
            this.Btn1500.Name = "Btn1500";
            this.Btn1500.Size = new System.Drawing.Size(100, 50);
            this.Btn1500.TabIndex = 3;
            this.Btn1500.Text = "Load 1500 ft";
            this.Btn1500.UseVisualStyleBackColor = true;
            this.Btn1500.Click += new System.EventHandler(this.Btn1500_Click);
            // 
            // Btn3500
            // 
            this.Btn3500.Location = new System.Drawing.Point(12, 300);
            this.Btn3500.Name = "Btn3500";
            this.Btn3500.Size = new System.Drawing.Size(100, 50);
            this.Btn3500.TabIndex = 4;
            this.Btn3500.Text = "Load 3500 ft";
            this.Btn3500.UseVisualStyleBackColor = true;
            this.Btn3500.Click += new System.EventHandler(this.Btn3500_Click);
            // 
            // IASInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Btn3500);
            this.Controls.Add(this.Btn1500);
            this.Controls.Add(this.Btn850);
            this.Controls.Add(this.IASdatagrid);
            this.Controls.Add(this.toolStrip1);
            this.Name = "IASInformation";
            this.Text = "IASInformation";
            this.Load += new System.EventHandler(this.IASInformation_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IASdatagrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Back2P3;
        private System.Windows.Forms.DataGridView IASdatagrid;
        private System.Windows.Forms.Button Btn850;
        private System.Windows.Forms.Button Btn1500;
        private System.Windows.Forms.Button Btn3500;
    }
}