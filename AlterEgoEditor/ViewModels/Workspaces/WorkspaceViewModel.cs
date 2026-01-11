using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Core.Models;

namespace AlterEgoEditor.ViewModels.Workspaces
{

    // Base class for different types of workspace view models
    public abstract partial class WorkspaceViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _displayName; // Name shown in the UI tab

        [ObservableProperty]
        private string _filePath; // Full file path of the workspace

        [ObservableProperty]
        private bool _isModified; // Indicates if there are unsaved changes

        [ObservableProperty]
        private bool _isLoading; // Indicates if the workspace is currently loading

        public ResourceNode Resource { get; } // The resource associated with this workspace

        public WorkspaceViewModel(ResourceNode resource)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(Resource));
            FilePath = resource.Uri;
            DisplayName = resource.Name;
        }

        // Abstract method to load the workspace data - called when opening the workspace for the first time
        public abstract void Load();

        // Abstract method to close the workspace - called when closing the tab
        public virtual void Close()
        {
        }

        protected void SetModified() => IsModified = true;

        protected void ClearModified() => IsModified = false;

        public event Action<WorkspaceViewModel> WorkspaceClosed;

        [RelayCommand]
        private void CloseWorkspace()
        {
            WorkspaceClosed?.Invoke(this);
        }

    }
}
