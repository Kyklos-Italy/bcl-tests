namespace NupackageDllExtractor
{
    partial class DuplicatesForm
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
            this.lblProjectList = new System.Windows.Forms.Label();
            this.CheckedListPackages = new System.Windows.Forms.CheckedListBox();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblProjectList
            // 
            this.lblProjectList.AutoSize = true;
            this.lblProjectList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProjectList.Location = new System.Drawing.Point(12, 9);
            this.lblProjectList.Name = "lblProjectList";
            this.lblProjectList.Size = new System.Drawing.Size(182, 20);
            this.lblProjectList.TabIndex = 9;
            this.lblProjectList.Text = "Duplicate Package List";
            // 
            // CheckedListPackages
            // 
            this.CheckedListPackages.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckedListPackages.FormattingEnabled = true;
            this.CheckedListPackages.Location = new System.Drawing.Point(16, 32);
            this.CheckedListPackages.Name = "CheckedListPackages";
            this.CheckedListPackages.Size = new System.Drawing.Size(772, 312);
            this.CheckedListPackages.TabIndex = 10;
            // 
            // BtnClose
            // 
            this.BtnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClose.Location = new System.Drawing.Point(209, 363);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(190, 44);
            this.BtnClose.TabIndex = 12;
            this.BtnClose.Text = "Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            // 
            // BtnOk
            // 
            this.BtnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnOk.Location = new System.Drawing.Point(412, 363);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.Size = new System.Drawing.Size(190, 44);
            this.BtnOk.TabIndex = 11;
            this.BtnOk.Text = "Ok";
            this.BtnOk.UseVisualStyleBackColor = true;
            // 
            // Duplicates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 432);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.BtnOk);
            this.Controls.Add(this.CheckedListPackages);
            this.Controls.Add(this.lblProjectList);
            this.Name = "Duplicates";
            this.Text = "Select Packages to process";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProjectList;
        private System.Windows.Forms.CheckedListBox CheckedListPackages;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnOk;
    }
}