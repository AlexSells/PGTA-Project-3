using System.Drawing;
using System.Windows.Forms;

namespace FormsAsterix
{
    partial class Search
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
            SearchCombBox = new ComboBox();
            SearchTxtBox = new TextBox();
            BtnSearch = new Button();
            SuspendLayout();
            // 
            // SearchCombBox
            // 
            SearchCombBox.FormattingEnabled = true;
            SearchCombBox.Location = new Point(12, 12);
            SearchCombBox.Name = "SearchCombBox";
            SearchCombBox.Size = new Size(179, 28);
            SearchCombBox.TabIndex = 0;
            // 
            // SearchTxtBox
            // 
            SearchTxtBox.Location = new Point(250, 12);
            SearchTxtBox.Name = "SearchTxtBox";
            SearchTxtBox.Size = new Size(237, 27);
            SearchTxtBox.TabIndex = 1;
            // 
            // BtnSearch
            // 
            BtnSearch.Location = new Point(393, 106);
            BtnSearch.Name = "BtnSearch";
            BtnSearch.Size = new Size(94, 29);
            BtnSearch.TabIndex = 2;
            BtnSearch.Text = "Search";
            BtnSearch.UseVisualStyleBackColor = true;
            BtnSearch.Click += BtnSearch_Click;
            // 
            // Search
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(497, 143);
            Controls.Add(BtnSearch);
            Controls.Add(SearchTxtBox);
            Controls.Add(SearchCombBox);
            Name = "Search";
            Text = "Search";
            Load += Search_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox SearchCombBox;
        private TextBox SearchTxtBox;
        private Button BtnSearch;
    }
}