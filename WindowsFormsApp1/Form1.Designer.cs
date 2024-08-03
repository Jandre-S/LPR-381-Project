namespace WindowsFormsApp1
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
            this.rtbOperations = new System.Windows.Forms.RichTextBox();
            this.rtbInitialProgram = new System.Windows.Forms.RichTextBox();
            this.grpMain = new System.Windows.Forms.GroupBox();
            this.btnSolve = new System.Windows.Forms.Button();
            this.lblAlgorithmSelect = new System.Windows.Forms.Label();
            this.cmbAlgorithmSelect = new System.Windows.Forms.ComboBox();
            this.btnSelectTextFile = new System.Windows.Forms.Button();
            this.grpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbOperations
            // 
            this.rtbOperations.Location = new System.Drawing.Point(12, 12);
            this.rtbOperations.Name = "rtbOperations";
            this.rtbOperations.Size = new System.Drawing.Size(361, 426);
            this.rtbOperations.TabIndex = 0;
            this.rtbOperations.Text = "";
            // 
            // rtbInitialProgram
            // 
            this.rtbInitialProgram.Location = new System.Drawing.Point(379, 41);
            this.rtbInitialProgram.Name = "rtbInitialProgram";
            this.rtbInitialProgram.Size = new System.Drawing.Size(411, 137);
            this.rtbInitialProgram.TabIndex = 1;
            this.rtbInitialProgram.Text = "";
            this.rtbInitialProgram.TextChanged += new System.EventHandler(this.rtbInitialProgram_TextChanged);
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.btnSolve);
            this.grpMain.Controls.Add(this.lblAlgorithmSelect);
            this.grpMain.Controls.Add(this.cmbAlgorithmSelect);
            this.grpMain.Location = new System.Drawing.Point(377, 184);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(411, 118);
            this.grpMain.TabIndex = 2;
            this.grpMain.TabStop = false;
            this.grpMain.Text = "Program Settings";
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(330, 67);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(75, 23);
            this.btnSolve.TabIndex = 2;
            this.btnSolve.Text = "Solve";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // lblAlgorithmSelect
            // 
            this.lblAlgorithmSelect.AutoSize = true;
            this.lblAlgorithmSelect.Location = new System.Drawing.Point(6, 22);
            this.lblAlgorithmSelect.Name = "lblAlgorithmSelect";
            this.lblAlgorithmSelect.Size = new System.Drawing.Size(86, 13);
            this.lblAlgorithmSelect.TabIndex = 1;
            this.lblAlgorithmSelect.Text = "Select Algorithm:";
            // 
            // cmbAlgorithmSelect
            // 
            this.cmbAlgorithmSelect.FormattingEnabled = true;
            this.cmbAlgorithmSelect.Items.AddRange(new object[] {
            "Primal Simplex",
            "Primal Simplex Revised"});
            this.cmbAlgorithmSelect.Location = new System.Drawing.Point(98, 19);
            this.cmbAlgorithmSelect.Name = "cmbAlgorithmSelect";
            this.cmbAlgorithmSelect.Size = new System.Drawing.Size(307, 21);
            this.cmbAlgorithmSelect.TabIndex = 0;
            // 
            // btnSelectTextFile
            // 
            this.btnSelectTextFile.Location = new System.Drawing.Point(379, 12);
            this.btnSelectTextFile.Name = "btnSelectTextFile";
            this.btnSelectTextFile.Size = new System.Drawing.Size(109, 23);
            this.btnSelectTextFile.TabIndex = 3;
            this.btnSelectTextFile.Text = "Read Program";
            this.btnSelectTextFile.UseVisualStyleBackColor = true;
            this.btnSelectTextFile.Click += new System.EventHandler(this.btnSelectTextFile_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSelectTextFile);
            this.Controls.Add(this.grpMain);
            this.Controls.Add(this.rtbInitialProgram);
            this.Controls.Add(this.rtbOperations);
            this.Name = "Form1";
            this.Text = "Form1";
            this.grpMain.ResumeLayout(false);
            this.grpMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbOperations;
        private System.Windows.Forms.RichTextBox rtbInitialProgram;
        private System.Windows.Forms.GroupBox grpMain;
        private System.Windows.Forms.Label lblAlgorithmSelect;
        private System.Windows.Forms.ComboBox cmbAlgorithmSelect;
        private System.Windows.Forms.Button btnSelectTextFile;
        private System.Windows.Forms.Button btnSolve;
    }
}

