using NupackageDllExtractorLib.FileUtils;
using NupackageDllExtractorLib.FormUtils;
using Semver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NupackageDllExtractor
{
    public partial class MainForm : Form
    {
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
            string errorMessage = CheckApplyConditionsAreMet();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Apply filters");
            }
            else
            {
                List<string> nupkgFiles = FileSystemUtils.GetFilesInFolderByExtension(SourceFolderComponent.FolderTextBox.Text, "*.nupkg", SearchOption.AllDirectories);
                if (nupkgFiles.Count == 0)
                {
                    MessageBox.Show("No nupkg files found in Source folder and its subdirectories", "Apply filters");
                }
                else
                {
                    string filterFolder = GetFilterFolder();
                    nupkgFiles = FileSystemUtils.FilterNukpgFilesByFilter(nupkgFiles, filterFolder);
                    if (nupkgFiles.Count == 0)
                    {
                        MessageBox.Show("No nupkg files found in Source folder and its subdirectories for folder filter: " + filterFolder, "Apply filters");
                    }
                    else
                    {
                        if (!Directory.Exists(DestinationFolderComponent.FolderTextBox.Text))
                        {
                            Directory.CreateDirectory(DestinationFolderComponent.FolderTextBox.Text);
                        }
                        if (ChkClearDestinationFolder.Checked)
                        {
                            FileSystemUtils.CleanDirectory(DestinationFolderComponent.FolderTextBox.Text);
                        }
                        string filterVersion = "";
                        if (ChkLastVersion.Checked)
                        {
                            nupkgFiles = FileSystemUtils.ExtractNukpgFilesByLastVersionOfEachPackage(nupkgFiles, DestinationFolderComponent.FolderTextBox.Text);
                        }
                        else
                        {
                            filterVersion = TxtVersion.Text;
                            List<string> filteredNupkgFiles = FileSystemUtils.FilterNukpgFilesByFilter(nupkgFiles, filterVersion);
                            FileSystemUtils.ExtractNukpgFilesByVersion(filteredNupkgFiles, DestinationFolderComponent.FolderTextBox.Text, filterVersion);
                        }
                        MessageBox.Show("Extraction completed successfully");
                    }
                }
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

        private string CheckApplyConditionsAreMet()
        {
            if (string.IsNullOrEmpty(SourceFolderComponent.FolderTextBox.Text))
            {
                return "Please fill source folder";
            }
            else
            {
                if (!Directory.Exists(SourceFolderComponent.FolderTextBox.Text))
                {
                    return "Source folder not found";
                }
                else
                {
                    if (string.IsNullOrEmpty(DestinationFolderComponent.FolderTextBox.Text))
                    {
                        return "Please fill destination folder";
                    }
                    else
                    {
                        if (TxtCustom.Enabled && string.IsNullOrEmpty(TxtCustom.Text))
                        {
                            return "please fill custom Source Folder filter";
                        }
                        else
                        {
                            if (TxtVersion.Enabled)
                            {
                                if (string.IsNullOrEmpty(TxtVersion.Text))
                                {
                                    return "Please fill version textbox";
                                }
                            }
                        }
                    }
                }
                return "";
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
                filterFolder = "debug";
            }
            if (RadioCustom.Checked)
            {
                filterFolder = TxtCustom.Text;
            }
            return filterFolder;
        }

        private string GetFilterVersion(List<string> nupkgPathFiles)
        {
            string filterVersion = "";
            if (ChkLastVersion.Checked)
            {
                filterVersion = SemVersionUtils.GetLastVersionOfNugetPackages(nupkgPathFiles);
            }
            else
            {
                filterVersion = TxtVersion.Text;
            }
            return filterVersion;
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
