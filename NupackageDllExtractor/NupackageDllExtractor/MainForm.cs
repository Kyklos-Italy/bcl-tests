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
            string errorMessage = CheckApplyConditionsAreMet();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Apply filters");
            }
            else
            {
                IList<string> nupkgFiles = GetAllNupkgFilesInSourceFolder();
                if (nupkgFiles.Count == 0)
                {
                    MessageBox.Show("No nupkg files found in Source folder and its subdirectories", "Apply filters");
                }
                else
                {
                    string filterFolder = GetFilterFolder();
                    nupkgFiles = ApplyFiltersToNupkgFiles(nupkgFiles, filterFolder);
                    if (nupkgFiles.Count == 0)
                    {
                        MessageBox.Show("No nupkg files found in Source folder and its subdirectories for folder filter: " + filterFolder, "Apply filters");
                    }
                    else
                    {
                        HandleDestinationDirectory();
                        IList<string> duplicatedNupkgFiles = GetDuplicatesInNupkgFiles(nupkgFiles);
                        if (duplicatedNupkgFiles.Count > 0)
                        {
                            foreach(string duplicatedNupkgFile in duplicatedNupkgFiles)
                            {
                                log.Debug("found duplicate NupkgFile: " + duplicatedNupkgFile);
                            }
                            MessageBox.Show($"Some nuget packages files are duplicated, please see log", "Apply filters");
                        }
                        else
                        {
                            ExtractNupkgFiles(nupkgFiles);
                        }
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
                filterFolder = "release";
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

        private void ExtractNupkgFiles(IList<string> nupkgFiles)
        {
            string filterVersion = "";
            if (ChkLastVersion.Checked)
            {
                nupkgFiles = FileSystemUtils.ExtractNukpgFilesByLastVersionOfEachPackage(nupkgFiles, DestinationFolderComponent.FolderTextBox.Text);
            }
            else
            {
                filterVersion = TxtVersion.Text;
                IList<string> filteredNupkgFiles = FileSystemUtils.FilterNukpgFilesByFilter(nupkgFiles, filterVersion);
                FileSystemUtils.ExtractNukpgFilesByVersion(filteredNupkgFiles, DestinationFolderComponent.FolderTextBox.Text, filterVersion);
            }
            MessageBox.Show("Extraction completed successfully");
        }

        private IList<string> GetAllNupkgFilesInSourceFolder()
        {
            IList<string> nupkgFiles = FileSystemUtils.GetFilesInFolderByExtension(SourceFolderComponent.FolderTextBox.Text, "*.nupkg", SearchOption.AllDirectories);
            return nupkgFiles;
        }

        private IList<string> ApplyFiltersToNupkgFiles(IList<string> nupkgFiles, string filterFolder)
        {
            nupkgFiles = FileSystemUtils.FilterNukpgFilesByFilter(nupkgFiles, filterFolder);
            return nupkgFiles;
        }

        private IList<string> GetDuplicatesInNupkgFiles(IList<string> nupkgFiles)
        {
            IList<string> duplicatenupkgFiles = new List<string>();
            List<string> duplicateNupkgFileNames = new List<string>();
            List<IGrouping<string, string>> duplicatedNupkgFiles = nupkgFiles.GroupBy(x => Path.GetFileName(x))
                .Where(x => x.Count() > 1).ToList();
            foreach(IGrouping<string, string> duplicateNupkgFile in duplicatedNupkgFiles)
            {
                duplicateNupkgFileNames.AddRange(duplicatedNupkgFiles.Select(x => x.Key).ToList());   
            }
            foreach(string nupkgFile in nupkgFiles)
            {
                foreach(string duplicateNupkgFileName in duplicateNupkgFileNames)
                {
                    if (nupkgFile.Contains(duplicateNupkgFileName))
                    {
                        duplicatenupkgFiles.Add(nupkgFile);
                        break;
                    }
                }
            }
            return duplicatenupkgFiles;
        }

        private void HandleDestinationDirectory()
        {
            if (!Directory.Exists(DestinationFolderComponent.FolderTextBox.Text))
            {
                Directory.CreateDirectory(DestinationFolderComponent.FolderTextBox.Text);
            }
            if (ChkClearDestinationFolder.Checked)
            {
                FileSystemUtils.CleanDirectory(DestinationFolderComponent.FolderTextBox.Text);
            }
        }
    }
}