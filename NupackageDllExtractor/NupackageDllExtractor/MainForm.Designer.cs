namespace NupackageDllExtractor
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.ChkClearDestinationFolder = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RadioCustom = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.TxtCustom = new System.Windows.Forms.TextBox();
            this.RadioRelease = new System.Windows.Forms.RadioButton();
            this.radioDebug = new System.Windows.Forms.RadioButton();
            this.ChkLastVersion = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.TxtVersion = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.DestinationFolderComponent = new NupackageDllExtractorLib.FormControls.DestinationFolderExplorerControl();
            this.SourceFolderComponent = new NupackageDllExtractorLib.FormControls.SourceFolderExplorerControl();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // ChkClearDestinationFolder
            // 
            this.ChkClearDestinationFolder.AutoSize = true;
            this.ChkClearDestinationFolder.Checked = true;
            this.ChkClearDestinationFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkClearDestinationFolder.Location = new System.Drawing.Point(109, 116);
            this.ChkClearDestinationFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ChkClearDestinationFolder.Name = "ChkClearDestinationFolder";
            this.ChkClearDestinationFolder.Size = new System.Drawing.Size(15, 14);
            this.ChkClearDestinationFolder.TabIndex = 3;
            this.ChkClearDestinationFolder.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RadioCustom);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.TxtCustom);
            this.groupBox1.Controls.Add(this.RadioRelease);
            this.groupBox1.Controls.Add(this.radioDebug);
            this.groupBox1.Controls.Add(this.ChkLastVersion);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.TxtVersion);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.groupBox1.Location = new System.Drawing.Point(9, 143);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(681, 172);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filters";
            // 
            // RadioCustom
            // 
            this.RadioCustom.AutoSize = true;
            this.RadioCustom.Location = new System.Drawing.Point(100, 74);
            this.RadioCustom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RadioCustom.Name = "RadioCustom";
            this.RadioCustom.Size = new System.Drawing.Size(14, 13);
            this.RadioCustom.TabIndex = 13;
            this.RadioCustom.TabStop = true;
            this.RadioCustom.UseVisualStyleBackColor = true;
            this.RadioCustom.CheckedChanged += new System.EventHandler(this.RadioCustom_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 71);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "Custom";
            // 
            // TxtCustom
            // 
            this.TxtCustom.Enabled = false;
            this.TxtCustom.Location = new System.Drawing.Point(125, 70);
            this.TxtCustom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TxtCustom.Name = "TxtCustom";
            this.TxtCustom.Size = new System.Drawing.Size(152, 23);
            this.TxtCustom.TabIndex = 11;
            // 
            // RadioRelease
            // 
            this.RadioRelease.AutoSize = true;
            this.RadioRelease.Location = new System.Drawing.Point(100, 50);
            this.RadioRelease.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RadioRelease.Name = "RadioRelease";
            this.RadioRelease.Size = new System.Drawing.Size(14, 13);
            this.RadioRelease.TabIndex = 9;
            this.RadioRelease.TabStop = true;
            this.RadioRelease.UseVisualStyleBackColor = true;
            // 
            // radioDebug
            // 
            this.radioDebug.AutoSize = true;
            this.radioDebug.Checked = true;
            this.radioDebug.Location = new System.Drawing.Point(100, 27);
            this.radioDebug.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioDebug.Name = "radioDebug";
            this.radioDebug.Size = new System.Drawing.Size(14, 13);
            this.radioDebug.TabIndex = 8;
            this.radioDebug.TabStop = true;
            this.radioDebug.UseVisualStyleBackColor = true;
            // 
            // ChkLastVersion
            // 
            this.ChkLastVersion.AutoSize = true;
            this.ChkLastVersion.Checked = true;
            this.ChkLastVersion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ChkLastVersion.Location = new System.Drawing.Point(103, 106);
            this.ChkLastVersion.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ChkLastVersion.Name = "ChkLastVersion";
            this.ChkLastVersion.Size = new System.Drawing.Size(15, 14);
            this.ChkLastVersion.TabIndex = 7;
            this.ChkLastVersion.UseVisualStyleBackColor = true;
            this.ChkLastVersion.CheckedChanged += new System.EventHandler(this.ChkLastVersion_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 104);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 17);
            this.label5.TabIndex = 6;
            this.label5.Text = "Last Version";
            // 
            // TxtVersion
            // 
            this.TxtVersion.Enabled = false;
            this.TxtVersion.Location = new System.Drawing.Point(103, 130);
            this.TxtVersion.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TxtVersion.Name = "TxtVersion";
            this.TxtVersion.Size = new System.Drawing.Size(152, 23);
            this.TxtVersion.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 132);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "Version";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 47);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Release";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Debug";
            // 
            // BtnClose
            // 
            this.BtnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClose.Location = new System.Drawing.Point(548, 336);
            this.BtnClose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(142, 36);
            this.BtnClose.TabIndex = 6;
            this.BtnClose.Text = "Quit";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.Location = new System.Drawing.Point(9, 336);
            this.BtnSave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(142, 36);
            this.BtnSave.TabIndex = 8;
            this.BtnSave.Text = "Apply";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label1.Location = new System.Drawing.Point(9, 116);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Clean Destination";
            // 
            // DestinationFolderComponent
            // 
            this.DestinationFolderComponent.ColumnCount = 3;
            this.DestinationFolderComponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.88144F));
            this.DestinationFolderComponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.11855F));
            this.DestinationFolderComponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.DestinationFolderComponent.Location = new System.Drawing.Point(9, 58);
            this.DestinationFolderComponent.Margin = new System.Windows.Forms.Padding(2);
            this.DestinationFolderComponent.Name = "DestinationFolderComponent";
            this.DestinationFolderComponent.RowCount = 1;
            this.DestinationFolderComponent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DestinationFolderComponent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DestinationFolderComponent.Size = new System.Drawing.Size(681, 41);
            this.DestinationFolderComponent.TabIndex = 9;
            // 
            // SourceFolderComponent
            // 
            this.SourceFolderComponent.AccessibleName = "";
            this.SourceFolderComponent.ColumnCount = 3;
            this.SourceFolderComponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.88144F));
            this.SourceFolderComponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.11855F));
            this.SourceFolderComponent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 102F));
            this.SourceFolderComponent.DestinationFolderExplorerControl = null;
            this.SourceFolderComponent.Location = new System.Drawing.Point(9, 11);
            this.SourceFolderComponent.Margin = new System.Windows.Forms.Padding(2);
            this.SourceFolderComponent.Name = "SourceFolderComponent";
            this.SourceFolderComponent.RowCount = 1;
            this.SourceFolderComponent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.78431F));
            this.SourceFolderComponent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 39.21569F));
            this.SourceFolderComponent.Size = new System.Drawing.Size(681, 41);
            this.SourceFolderComponent.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 392);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DestinationFolderComponent);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ChkClearDestinationFolder);
            this.Controls.Add(this.SourceFolderComponent);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Text = "Nupkg Dll Extractor";
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private NupackageDllExtractorLib.FormControls.SourceFolderExplorerControl SourceFolderComponent;
        private System.Windows.Forms.CheckBox ChkClearDestinationFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TxtVersion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ChkLastVersion;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.RadioButton RadioRelease;
        private System.Windows.Forms.RadioButton radioDebug;
        private NupackageDllExtractorLib.FormControls.DestinationFolderExplorerControl DestinationFolderComponent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtCustom;
        private System.Windows.Forms.RadioButton RadioCustom;
        private System.Windows.Forms.Label label7;
    }
}

