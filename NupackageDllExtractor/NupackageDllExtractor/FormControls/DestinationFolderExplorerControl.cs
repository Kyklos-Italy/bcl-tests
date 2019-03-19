using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NupackageDllExtractor.FormControls
{
    public class DestinationFolderExplorerControl : CustomTableLayoutPanel
    {
        public Label LabelOfTextBox;
        public TextBox FolderTextBox;
        public Button FolderBrowserButton;

        public DestinationFolderExplorerControl() : this
        (
           3, "", ""
        )
        {
            FolderBrowserButton.Click += new EventHandler(FolderBrowserButtonClickEvent);
            SeDefaultValues();
        }

        public DestinationFolderExplorerControl(int columnCount, string labelText, string buttonText) : base(columnCount)
        {
            InitializeComponents();
            ConfigureLabel("Destination Folder");
            ConfigureTextBox();
            ConfigureButton("...");
        }

        private void InitializeComponents()
        {
            LabelOfTextBox = new Label();
            FolderTextBox = new TextBox();
            FolderBrowserButton = new Button();
        }

        private void SeDefaultValues()
        {
            FolderTextBox.Text = @"C:\tmp\nugetdlls";
        }

        private void ConfigureLabel(string labelText)
        {
            LabelOfTextBox.Text = labelText;
            StyleLabel(LabelOfTextBox);
            Controls.Add(LabelOfTextBox);
        }

        private void ConfigureTextBox()
        {
            StyleTextBox(FolderTextBox);
            Controls.Add(FolderTextBox);
        }

        private void ConfigureButton(string buttonText)
        {
            FolderBrowserButton.Text = buttonText;
            StyleButton(FolderBrowserButton);
            Controls.Add(FolderBrowserButton);
        }

        #region events

        #region FolderBrowserButton events

        private void FolderBrowserButtonClickEvent(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                FolderTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        #endregion

        #endregion

    }
}
