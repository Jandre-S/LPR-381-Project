namespace LPR_381_Project
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.richTextBox_LoadFormFile = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox_SolveAlgorithms = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButton_LoadFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Solve = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_SaveFile = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox_Solved = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox_LoadFormFile
            // 
            this.richTextBox_LoadFormFile.Location = new System.Drawing.Point(6, 19);
            this.richTextBox_LoadFormFile.Name = "richTextBox_LoadFormFile";
            this.richTextBox_LoadFormFile.Size = new System.Drawing.Size(486, 118);
            this.richTextBox_LoadFormFile.TabIndex = 0;
            this.richTextBox_LoadFormFile.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox_LoadFormFile);
            this.groupBox1.Location = new System.Drawing.Point(12, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 153);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Load Form File";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripComboBox_SolveAlgorithms,
            this.toolStripButton_LoadFile,
            this.toolStripButton_Solve,
            this.toolStripButton_SaveFile});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(547, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(86, 22);
            this.toolStripLabel1.Text = "toolStripLabel1";
            // 
            // toolStripComboBox_SolveAlgorithms
            // 
            this.toolStripComboBox_SolveAlgorithms.AutoCompleteCustomSource.AddRange(new string[] {
            "Primal Simplex"});
            this.toolStripComboBox_SolveAlgorithms.Items.AddRange(new object[] {
            "Primal Simplex",
            "CuttingPlane"});
            this.toolStripComboBox_SolveAlgorithms.Name = "toolStripComboBox_SolveAlgorithms";
            this.toolStripComboBox_SolveAlgorithms.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripButton_LoadFile
            // 
            this.toolStripButton_LoadFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_LoadFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_LoadFile.Image")));
            this.toolStripButton_LoadFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_LoadFile.Name = "toolStripButton_LoadFile";
            this.toolStripButton_LoadFile.Size = new System.Drawing.Size(55, 22);
            this.toolStripButton_LoadFile.Text = "LoadFile";
            this.toolStripButton_LoadFile.Click += new System.EventHandler(this.toolStripButton_LoadFile_Click);
            // 
            // toolStripButton_Solve
            // 
            this.toolStripButton_Solve.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_Solve.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Solve.Image")));
            this.toolStripButton_Solve.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Solve.Name = "toolStripButton_Solve";
            this.toolStripButton_Solve.Size = new System.Drawing.Size(39, 22);
            this.toolStripButton_Solve.Text = "Solve";
            this.toolStripButton_Solve.Click += new System.EventHandler(this.toolStripButton_Solve_Click);
            // 
            // toolStripButton_SaveFile
            // 
            this.toolStripButton_SaveFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton_SaveFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SaveFile.Image")));
            this.toolStripButton_SaveFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_SaveFile.Name = "toolStripButton_SaveFile";
            this.toolStripButton_SaveFile.Size = new System.Drawing.Size(53, 22);
            this.toolStripButton_SaveFile.Text = "SaveFile";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox_Solved);
            this.groupBox2.Location = new System.Drawing.Point(12, 187);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(492, 316);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Solved";
            // 
            // richTextBox_Solved
            // 
            this.richTextBox_Solved.Location = new System.Drawing.Point(6, 19);
            this.richTextBox_Solved.Name = "richTextBox_Solved";
            this.richTextBox_Solved.Size = new System.Drawing.Size(480, 291);
            this.richTextBox_Solved.TabIndex = 0;
            this.richTextBox_Solved.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 528);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_LoadFormFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox_SolveAlgorithms;
        private System.Windows.Forms.ToolStripButton toolStripButton_LoadFile;
        private System.Windows.Forms.ToolStripButton toolStripButton_Solve;
        private System.Windows.Forms.ToolStripButton toolStripButton_SaveFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextBox_Solved;
    }
}

