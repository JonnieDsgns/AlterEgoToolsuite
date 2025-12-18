using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using AlterEgoEditor.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Models;
using AlterEgoEditor.Views;
using Engine.Services; // Required for GameScannerService

namespace AlterEgoEditor.ViewModels
{
    public partial class StartupViewModel : ObservableObject
    {
        private readonly GameScannerService _scannerService = new();
        private readonly IndexerService _indexerService;

        [ObservableProperty] // List of games detected on the user's system
        private ObservableCollection<GameInstallation> _availableGames = new();

        [ObservableProperty] // The game installation selected by the user
        private GameInstallation _selectedGame;

        public event Action<ModProject> ProjectCreated;

        private readonly ProjectService _projectService = new ProjectService();
        public StartupViewModel()
        {
            LoadAvailableGames(); // Load games when ViewModel is initialized
        }

        private void LoadAvailableGames()
        {
            try
            {
                var games = _scannerService.ScanForGames();

                AvailableGames.Clear();
                foreach (var game in games)
                {
                    AvailableGames.Add(game);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scanning for games: {ex.Message}", "Initialization Error");
                // Optionally load a mock/placeholder game if scan fails
            }
        }

        [RelayCommand]
        private void CreateNewProject()
        {
            // 1. Check if we have any games to offer
            if (AvailableGames == null || AvailableGames.Count == 0)
            {
                MessageBox.Show("No game installations found. Please check scanner settings.", "Error");
                return;
            }

            // 2. Create the ViewModel for the modal, passing the list of available games.
            var createVm = new CreateProjectViewModel(AvailableGames);

            // 3. Set up the event handler: When the modal VM successfully creates a project, 
            // this method relays that project back to the MainViewModel.
            createVm.ProjectCreated += (newProject) =>
            {
                // Execute the event, which the MainViewModel is listening to
                ProjectCreated?.Invoke(newProject);
            };

            // 4. Create and show the modal window.
            var window = new CreateProjectWindow(createVm);
            window.ShowDialog();
        }

        [RelayCommand]
        private void OpenProjectFromStart()
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
                    ProjectCreated?.Invoke(project);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading project:\n{ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void OpenLink(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to open link: {ex.Message}", "Error");
            }
        }
    }
}
