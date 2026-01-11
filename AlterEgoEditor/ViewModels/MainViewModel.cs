using System;
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Models;
using Engine.Services;

namespace AlterEgoEditor.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IndexerService _indexerService;
        private readonly ProjectService _projectService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CloseProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveProjectCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveProjectAsCommand))]
        private ModProject _activeProject;

        [ObservableProperty]
        private object _currentView;

        public MainViewModel()
        {
            var erpManager = ErpHandleManager.Instance;
            _indexerService = new IndexerService(erpManager);
            _projectService = new ProjectService();

            ShowStartupView();
        }

        private void ShowStartupView()
        {
            var startupVm = new StartupViewModel();
            startupVm.ProjectCreated += OnProjectOpened;
            CurrentView = startupVm;
        }

        private void ShowEditorView(ModProject project)
        {
            ActiveProject = project;
            CurrentView = new EditorViewModel(project, _indexerService);
        }

        private void OnProjectOpened(ModProject project)
        {
            ShowEditorView(project);
        }

        [RelayCommand]
        private void OpenProject()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "AlterEgo Project Files (*.aeproject)|*.aeproject",
                Title = "Open AlterEgo Project"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var project = _projectService.ReadProjectFile(openFileDialog.FileName);
                    ShowEditorView(project);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading project:\n{ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanSaveProject))]
        private void SaveProject()
        {
            if (ActiveProject == null)
            {
                MessageBox.Show("No project open to save.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(ActiveProject.ProjectFilePath))
            {
                SaveProjectAsCommand.Execute(null);
                return;
            }

            try
            {
                _projectService.WriteProjectFile(ActiveProject);
                MessageBox.Show($"Project saved to:\n{ActiveProject.ProjectFilePath}", "Save Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving project:\n{ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSaveProjectAs))]
        private void SaveProjectAs()
        {
            if (ActiveProject == null)
            {
                MessageBox.Show("No project open to save.", "Save As", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var alterEgoProjectsDir = Path.Combine(myDocuments, "AlterEgoProjects");

            try
            {
                Directory.CreateDirectory(alterEgoProjectsDir);
            }
            catch
            {
                alterEgoProjectsDir = myDocuments;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "AlterEgo Project Files (*.aeproject)|*.aeproject",
                FileName = ActiveProject.ProjectName + ".aeproject",
                Title = "Save AlterEgo Project As",
                InitialDirectory = alterEgoProjectsDir
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ActiveProject.ProjectFilePath = saveFileDialog.FileName;
                SaveProjectCommand.Execute(null);
            }
        }

        [RelayCommand(CanExecute = nameof(CanCloseProject))]
        private void CloseProject()
        {
            if (ActiveProject == null)
            {
                ShowStartupView();
                return;
            }

            var result = MessageBox.Show(
                $"Do you want to save changes to '{ActiveProject.ProjectName}' before closing?",
                "Unsaved Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                SaveProjectCommand.Execute(null);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            // Clear editor state if applicable
            if (CurrentView is EditorViewModel editorVm)
            {
                editorVm.ClearState();
            }

            ActiveProject = null;
            ShowStartupView();
        }

        private bool CanSaveProject() => ActiveProject != null;
        private bool CanSaveProjectAs() => ActiveProject != null;
        private bool CanCloseProject() => ActiveProject != null;
    }
}
