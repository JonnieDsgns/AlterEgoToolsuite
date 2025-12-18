using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text.Json; // You'll need this for the filelist.json later
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Models;
using Engine.Services;
using Microsoft.Win32;

namespace AlterEgoEditor.ViewModels
{
    public partial class EditProjectViewModel : ObservableObject
    {
        // Data Properties for the Modal UI
        [ObservableProperty]
        private string _newProjectName;

        [ObservableProperty]
        private string _authorName;
        public ObservableCollection<string> ModCategories { get; } = new ObservableCollection<string>
        {
            "MyTeam",
            "Tracks",
            "Teams",
            "Liveries",
            "Helmets",
            "Seasons"
        };

        [ObservableProperty]
        private string _selectedCategory = "MyTeam";

        [ObservableProperty]
        private ObservableCollection<GameInstallation> _availableGames;

        [ObservableProperty]
        private GameInstallation _selectedGame;

        [ObservableProperty]
        private string _modIconPath;

        [ObservableProperty]
        private string _description;

        private readonly ProjectService _projectService = new();

        // Action to signal the creation of a new project
        public event Action<ModProject> ProjectCreated;

        // Action to close the modal window
        public Action CloseWindow { get; set; }

        public EditProjectViewModel(IEnumerable<GameInstallation> detectedGames)
        {
            AvailableGames = new ObservableCollection<GameInstallation>(detectedGames);
            SelectedGame = AvailableGames.FirstOrDefault();
        }

        [RelayCommand]
        private void BrowseModIcon()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Mod Icon",
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ModIconPath = openFileDialog.FileName;
            }
        }

        [RelayCommand]
        private void CreateProjectAndClose()
        {
            if (string.IsNullOrWhiteSpace(NewProjectName) || SelectedGame == null) // || !SelectedGame.IsValid currently deactivated for testing purposes
            {
                // Shows an error message to the user
                MessageBox.Show("Please ensure a valid Name and Game are selected.", "Validation Error");
                return;
            }

            var newProject = new ModProject
            {
                ProjectName = NewProjectName,
                AuthorName = AuthorName,
                ModCategory = SelectedCategory,
                GameTitle = SelectedGame.GameTitle,
                GameRootPath = SelectedGame.RootPath,
                ModIconPath = ModIconPath,
                Description = Description,
                ModVersion = "0.1"
            };

            try
            {
                // Initialize the physical workspace folders
                _projectService.InitializeProjectWorkspace(newProject);

                // If a ModIcon was selected, we should copy it to the userdata folder
                if (!string.IsNullOrEmpty(ModIconPath) && File.Exists(ModIconPath))
                {
                    string destination = Path.Combine(newProject.UserdataPath, "mod_icon" + Path.GetExtension(ModIconPath));
                    File.Copy(ModIconPath, destination, true);
                    newProject.ModIconPath = destination; // Point to the internal workspace copy
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize project workspace: {ex.Message}");
                return;
            }

            ProjectCreated?.Invoke(newProject);

            CloseWindow?.Invoke();
        }

        [RelayCommand]
        private void CancelAndClose()
        {
            CloseWindow?.Invoke();
        }
    }
}
