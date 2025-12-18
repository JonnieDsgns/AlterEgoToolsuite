using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private ObservableCollection<FileNode> _rootNodes = new();

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private bool _isProjectModified = false;

        [ObservableProperty]
        private object _currentView; // <-- This property controls what the MainWindow displays

        [ObservableProperty]
        private ObservableCollection<ResourceNode> _erpResources = new();

        [ObservableProperty]
        private FileNode _selectedNode;

        public MainViewModel()
        {
            // 1. Initialize the services 
            // NOTE: The IndexerService and ProjectService should ideally be injected, but for simplicity:

            // Singleton Instance
            var erpManager = ErpHandleManager.Instance;
            _indexerService = new IndexerService(erpManager);
            _projectService = new ProjectService();

            InitializeView();
        }

        private void InitializeView()
        {
            // Set the initial view to the StartupViewModel
            var startupVm = new StartupViewModel();

            // Subscribe to the event 
            startupVm.ProjectCreated -= OnProjectCreated; // Prevent multiple subscriptions
            startupVm.ProjectCreated += OnProjectCreated;

            CurrentView = startupVm;
        }

        private void OnProjectCreated(ModProject newProject)
        {
            ActiveProject = newProject;

            CurrentView = this; // Switches the view from StartupViewModel to the main editor (MainViewModel)

            LoadGameFilesCommand.Execute(null); // Starts indexing
        }

        [RelayCommand]
        private void LoadGameFiles()
        {
            if (ActiveProject == null || IsLoading) return;

            IsLoading = true;

            try
            {
                RootNodes = _indexerService.BuildIndex(ActiveProject.GameRootPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load file index:\n{ex.Message}", "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void SelectNode(FileNode node)
        {
            if (node == null) return;

            ErpResources.Clear();

            // Use a case-insensitive check to be safe
            if (node.ResourceType.Equals("erp", StringComparison.OrdinalIgnoreCase))
            {
                var resources = _indexerService.GetDetailedResources(node.FullPath);
                foreach (var res in resources)
                {
                    ErpResources.Add(res);
                }
            }
        }

        partial void OnSelectedNodeChanged(FileNode value)
        {
            ErpResources.Clear();

            if (value != null && value.ResourceType == "erp")
            {
                LoadErpResources(value.FullPath);
            }
        }

        private void LoadErpResources(string path)
        {
            var resources = _indexerService.GetDetailedResources(path);
            foreach (var res in resources)
            {
                ErpResources.Add(res);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSaveProject))]
        private void SaveProject() // Save command
        {
            // 1. Check if a project is open
            if (ActiveProject == null)
            {
                MessageBox.Show("No project open to save.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(ActiveProject.ProjectFilePath)) // Check if project has been saved before
            {
                SaveProjectAsCommand.Execute(null); // Delegate to Save As if no path/not saved before
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
                return;
            }

        }

        [RelayCommand(CanExecute = nameof(CanSaveProjectAs))]
        private void SaveProjectAs() // Save as command
        {
            // Check if a project is open
            if (ActiveProject == null)
            {
                MessageBox.Show("No project open to save.", "Save As", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            // Ensure an "AlterEgoProjects" folder exists inside the user's MyDocuments and use it as InitialDirectory
            var myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var alterEgoProjectsDir = Path.Combine(myDocuments, "AlterEgoProjects");

            try
            {
                // Create directory if it doesn't exist (safe no-op if already exists)
                Directory.CreateDirectory(alterEgoProjectsDir);
            }
            catch
            {
                // If directory creation fails for any reason, fall back to MyDocuments
                alterEgoProjectsDir = myDocuments;
            }

            // Show SaveFileDialog to get the desired file path
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
                SaveProjectCommand.Execute(null); // Delegate to SaveProject Logic
            }
        }

        [RelayCommand]
        private void OpenProject()
        {
            // Show OpenFileDialog to select a project file
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
                    ActiveProject = project;
                    CurrentView = this; // Switches the view from StartupViewModel to the main editor (MainViewModel)
                    LoadGameFilesCommand.Execute(null); // Starts indexing
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading project:\n{ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanCloseProject))]
        private void CloseProject()
        {
            // 1. Check if there is an active project to close
            if (ActiveProject == null)
            {
                // If nothing is open, just ensure we are at the startup view (shouldn't happen)
                CurrentView = new StartupViewModel();
                return;
            }

            // A better, dedicated IsDirty property on ModProject is required for a reliable check.
            // For now, we will simply ask the user if they want to save if a project is open.
            var result = MessageBox.Show(
                $"Do you want to save changes to '{ActiveProject.ProjectName}' before closing?",
                "Unsaved Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Execute the Save logic
                SaveProjectCommand.Execute(null);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                // User cancelled the close operation
                return;
            }

            // 2. Clear Model State
            ActiveProject = null;
            RootNodes.Clear(); // Clears the TreeView

            // 3. Reset Services (This method needs to be added to IndexerService.cs)
            // _indexerService.ClearIndex(); 

            InitializeView(); // Reset to Startup View
        }

        private bool CanSaveProject()
        {
            return ActiveProject != null;
        }

        private bool CanSaveProjectAs()
        {
            return ActiveProject != null;
        }

        private bool CanCloseProject()
        {
            return ActiveProject != null;
        }

    }

}
