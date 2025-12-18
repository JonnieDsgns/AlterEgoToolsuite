using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Core.Models
{
    public partial class FileNode : ObservableObject
    {
        public string Name { get; set; } // Display name in the TreeView

        public string FullPath { get; set; } // Full file system path

        public string ResourceType { get; set; } // GfxResource, XML, PKG, etc.

        public FileNode Parent { get; private set; } // Parent node reference

        public ObservableCollection<FileNode> Children { get; set; } = new ObservableCollection<FileNode>(); // Defines the nodes and allows one FileNode to have multiple children FileNodes (Nesting)

        [ObservableProperty]
        private bool _isLoaded;

        [ObservableProperty]
        private bool _isExpanded;

        [ObservableProperty] // Code generator for mvvm toolkit
        private bool _isModified;

        [ObservableProperty] // Code generator for mvvm toolkit
        private bool _hasModifiedChildren;

        public FileNode(string name, string fullPath, string resourceType, FileNode parent = null)
        {
            // Assigning properties
            Name = name;
            FullPath = fullPath;
            ResourceType = resourceType;
            Parent = parent;

            // Setup for internal listeners
            Children.CollectionChanged += (s, e) => UpdateParentModificationState();

            PropertyChanged += FileNode_PropertyChanged;
        }

        // Listen for changes in child nodes so that the parent will be updated accordingly
        private void FileNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Parent != null && (e.PropertyName == nameof(IsModified) || e.PropertyName == nameof(HasModifiedChildren)))
            {
                UpdateParentModificationState();
            }
        }

        public void UpdateParentModificationState()
        {
            // Check if any child is modified or has modified children
            bool anyChildModified = Children.Any(c => c.IsModified || c.HasModifiedChildren);

            // Update the Node's HasModifiedChildren property
            HasModifiedChildren = anyChildModified;

            // Recursively update the parent node
            Parent?.UpdateParentModificationState();
        }
    }
}
