namespace connectwise_export_formatter
{
    partial class FRMMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRMMain));
            this.lblInput = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnBrowseInput = new System.Windows.Forms.Button();
            this.lblOutput = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnOpenOutput = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.LBLLicense = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(16, 18);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(62, 13);
            this.lblInput.TabIndex = 0;
            this.lblInput.Text = "Input JSON";
            // 
            // txtInput
            // 
            this.txtInput.AllowDrop = true;
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(39)))), ((int)(((byte)(45)))));
            this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput.ForeColor = System.Drawing.Color.White;
            this.txtInput.Location = new System.Drawing.Point(19, 34);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(430, 20);
            this.txtInput.TabIndex = 1;
            this.txtInput.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtInput_DragDrop);
            this.txtInput.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtInput_DragEnter);
            // 
            // btnBrowseInput
            // 
            this.btnBrowseInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseInput.Location = new System.Drawing.Point(455, 32);
            this.btnBrowseInput.Name = "btnBrowseInput";
            this.btnBrowseInput.Size = new System.Drawing.Size(92, 23);
            this.btnBrowseInput.TabIndex = 2;
            this.btnBrowseInput.Text = "Browse...";
            this.btnBrowseInput.UseVisualStyleBackColor = true;
            this.btnBrowseInput.Click += new System.EventHandler(this.btnBrowseInput_Click);
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Location = new System.Drawing.Point(16, 70);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(68, 13);
            this.lblOutput.TabIndex = 3;
            this.lblOutput.Text = "Output folder";
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(39)))), ((int)(((byte)(45)))));
            this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOutput.ForeColor = System.Drawing.Color.White;
            this.txtOutput.Location = new System.Drawing.Point(19, 86);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(430, 20);
            this.txtOutput.TabIndex = 4;
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseOutput.Location = new System.Drawing.Point(455, 84);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(92, 23);
            this.btnBrowseOutput.TabIndex = 5;
            this.btnBrowseOutput.Text = "Browse...";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(126)))), ((int)(((byte)(255)))));
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(455, 124);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(92, 30);
            this.btnRun.TabIndex = 6;
            this.btnRun.Text = "Generate";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnOpenOutput
            // 
            this.btnOpenOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenOutput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenOutput.Location = new System.Drawing.Point(357, 124);
            this.btnOpenOutput.Name = "btnOpenOutput";
            this.btnOpenOutput.Size = new System.Drawing.Size(92, 30);
            this.btnOpenOutput.TabIndex = 7;
            this.btnOpenOutput.Text = "Open Output";
            this.btnOpenOutput.UseVisualStyleBackColor = true;
            this.btnOpenOutput.Click += new System.EventHandler(this.btnOpenOutput_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(21)))), ((int)(((byte)(26)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtLog.Location = new System.Drawing.Point(19, 172);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(528, 160);
            this.txtLog.TabIndex = 10;
            this.txtLog.WordWrap = false;
            // 
            // LBLLicense
            // 
            this.LBLLicense.AutoSize = true;
            this.LBLLicense.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LBLLicense.Location = new System.Drawing.Point(19, 349);
            this.LBLLicense.Name = "LBLLicense";
            this.LBLLicense.Size = new System.Drawing.Size(44, 13);
            this.LBLLicense.TabIndex = 11;
            this.LBLLicense.Text = "License";
            this.LBLLicense.Click += new System.EventHandler(this.LBLLicense_Click);
            // 
            // FRMMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(26)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(564, 371);
            this.Controls.Add(this.LBLLicense);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnOpenOutput);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnBrowseOutput);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.btnBrowseInput);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblInput);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FRMMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connectwise Export Formatter";
            this.Load += new System.EventHandler(this.FRMMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnBrowseInput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnOpenOutput;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label LBLLicense;
    }
}