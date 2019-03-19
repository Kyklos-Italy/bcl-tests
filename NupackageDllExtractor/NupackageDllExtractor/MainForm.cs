using NupackageDllExtractor.FormUtils;
using NupackageDllExtractorLib;
using System;
using System.Windows.Forms;

namespace NupackageDllExtractor
{
	public partial class MainForm : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("DEBUG");

        public MainForm()
        {
            InitializeComponent();
            ConfigureForm();
            AssingConnectedControls();
        }

        private void ConfigureForm()
        {
            FormConfigurator.SetAsFixedForm(this);
        }

        private void AssingConnectedControls()
        {
            SourceFolderComponent.DestinationFolderExplorerControl = DestinationFolderComponent;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
			try
			{
				DllExtractor extractor = new DllExtractor(SourceFolderComponent.FolderTextBox.Text, DestinationFolderComponent.FolderTextBox.Text, ChkClearDestinationFolder.Checked, GetFilterFolder(), TxtVersion.Text);
				string resultMessage = extractor.Extract();
				MessageBox.Show(resultMessage, "Extraction result");
			}
			catch(Exception ex)
			{
				MessageBox.Show("Unexpected error, check the logs for details!", "Extraction error");
				log.Error("Error during extraction", ex);
			}
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ChkLastVersion_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkLastVersion.Checked)
            {
                TxtVersion.Text = "";
                TxtVersion.Enabled = false;
            }
            else
            {
                TxtVersion.Enabled = true;
            }
        }

        private string GetFilterFolder()
        {
            string filterFolder = "";
            if (radioDebug.Checked)
            {
                filterFolder = "debug";
            }
            if (RadioRelease.Checked)
            {
                filterFolder = "release";
            }
            if (RadioCustom.Checked)
            {
                filterFolder = TxtCustom.Text;
            }
            return filterFolder;
        }

        private void RadioCustom_CheckedChanged(object sender, EventArgs e)
        {
            TxtCustom.Enabled = RadioCustom.Checked;
            if (!RadioCustom.Checked)
            {
                TxtCustom.Text = "";
            }
        }
    }
}