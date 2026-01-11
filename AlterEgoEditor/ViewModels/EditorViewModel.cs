using System;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Models;
using Engine.Services;

namespace AlterEgoEditor.ViewModels
{
    public partial class EditorViewModel : ObservableObject
    {
        private readonly IndexerService _indexerService;

        [ObservableProperty]
        private ModProject _activeProject;

        [ObservableProperty]
        private ObservableCollection<FileNode> _rootNodes = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private ObservableCollection<ResourceNode> _erpResources = new();


        [ObservableProperty]
        private FileNode _selectedNode;

        [ObservableProperty]
        private ResourceNode _selectedResource;

        public EditorViewModel(ModProject project, IndexerService indexerService)
        {
            _indexerService = indexerService;
            ActiveProject = project;

            LoadGameFiles();
        }

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
                // Load categories when ERP is selected
                EnsureErpCategoriesLoaded(node);
                
                // Load resources for display in the DataGrid
                LoadErpResources(node.FullPath);
            }

        }

        partial void OnSelectedNodeChanged(FileNode value)
        {
            ErpResources.Clear();

            if (value != null && value.ResourceType.Equals("erp", StringComparison.OrdinalIgnoreCase))
            {
                // Load categories when ERP is selected
                EnsureErpCategoriesLoaded(value);
                
                // Also load resources for display in the DataGrid
                LoadErpResources(value.FullPath);
            }
        }

        private void EnsureErpCategoriesLoaded(FileNode node)
        {
            if (!node.IsLoaded)
            {
                _indexerService.LoadErpCategories(node);
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

        public void ClearState()
        {
            RootNodes.Clear();
            ErpResources.Clear();
            SelectedNode = null;
            SelectedResource = null;
        }
    }
}
