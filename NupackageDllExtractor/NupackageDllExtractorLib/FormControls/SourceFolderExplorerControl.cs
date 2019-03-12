using Kyklos.Kernel.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NupackageDllExtractorLib.FormControls
{
    public class SourceFolderExplorerControl : CustomTableLayoutPanel
    {
        public Label LabelOfTextBox;
        public TextBox FolderTextBox;
        public Button FolderBrowserButton;
        public DestinationFolderExplorerControl DestinationFolderExplorerControl { get; set; }

        public SourceFolderExplorerControl() : this
        (
           3, "", ""
        )
        {
            FolderBrowserButton.Click += new EventHandler(FolderBrowserButtonClickEvent);
        }

        public SourceFolderExplorerControl(int columnCount, string labelText, string buttonText) : base(columnCount)
        {
            InitializeComponents();
            ConfigureLabel("Source Folder");
            ConfigureTextBox();
            ConfigureButton("...");
        }

        private void InitializeComponents()
        {
            LabelOfTextBox = new Label();
            FolderTextBox = new TextBox();
            FolderBrowserButton = new Button();
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
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            folderBrowserDialog.SelectedPath = Path.GetDirectoryName(Utilities.GetFilePathFromAssemblyCodeBase(Assembly.GetExecutingAssembly()));
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                FolderTextBox.Text = folderBrowserDialog.SelectedPath;
                DestinationFolderExplorerControl.FolderTextBox.Text = Path.Combine(folderBrowserDialog.SelectedPath, "dest"); 
            }
        }

        #endregion

        #endregion

    }
}
