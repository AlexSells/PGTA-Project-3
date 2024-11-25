using System.Drawing;
using System.Windows.Forms;

namespace FormsAsterix
{
    partial class Filter
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
            FilterCombBox = new ComboBox();
            txtBox_Start = new TextBox();
            txtBox_End = new TextBox();
            BtnFilter = new Button();
            SuspendLayout();
            // 
            // FilterCombBox
            // 
            FilterCombBox.FormattingEnabled = true;
            FilterCombBox.Location = new Point(12, 12);
            FilterCombBox.Name = "FilterCombBox";
            FilterCombBox.Size = new Size(151, 28);
            FilterCombBox.TabIndex = 0;
            // 
            // txtBox_Start
            // 
            txtBox_Start.Location = new Point(357, 11);
            txtBox_Start.Name = "txtBox_Start";
            txtBox_Start.Size = new Size(125, 27);
            txtBox_Start.TabIndex = 1;
            // 
            // txtBox_End
            // 
            txtBox_End.Location = new Point(357, 82);
            txtBox_End.Name = "txtBox_End";
            txtBox_End.Size = new Size(125, 27);
            txtBox_End.TabIndex = 2;
            // 
            // BtnFilter
            // 
            BtnFilter.Location = new Point(388, 154);
            BtnFilter.Name = "BtnFilter";
            BtnFilter.Size = new Size(94, 29);
            BtnFilter.TabIndex = 3;
            BtnFilter.Text = "Filter";
            BtnFilter.UseVisualStyleBackColor = true;
            BtnFilter.Click += BtnFilter_Click;
            // 
            // Filter
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(497, 193);
            Controls.Add(BtnFilter);
            Controls.Add(txtBox_End);
            Controls.Add(txtBox_Start);
            Controls.Add(FilterCombBox);
            Name = "Filter";
            Text = "Filter";
            Load += Filter_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox FilterCombBox;
        private TextBox txtBox_Start;
        private TextBox txtBox_End;
        private Button BtnFilter;
    }
}