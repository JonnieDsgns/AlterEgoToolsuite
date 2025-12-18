using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;

namespace Core.Models
{
    // Setup for Mod Project data model
    public partial class ModProject : ObservableObject
    {
        [ObservableProperty] // Name of the game the mod is for
        private string _gameTitle;

        [ObservableProperty] // Root path of the game installation
        private string _gameRootPath;

        [ObservableProperty] // The name of the project
        private string _projectName;

        [ObservableProperty] // The name of the mod author
        private string _authorName;

        [ObservableProperty] // The category of the mod
        private string _modCategory;

        [ObservableProperty] // The mod's description
        private string _description;

        [ObservableProperty] // The file path to the project file
        private string _projectFilePath;

        [ObservableProperty] // The mod's version number
        private string _modVersion = "0.1";

        [ObservableProperty] // The file path to the mod's icon
        private string _modIconPath;

        [ObservableProperty] // List of file paths to mod screenshots
        private ObservableCollection<string> _screenshotPaths = new();

        [ObservableProperty]
        private string _tempWorkspacePath; // Temporary workspace path for building the mod project

        public string MetadataPath => Path.Combine(TempWorkspacePath, "metadata"); // Path for metadata storage
        public string UserdataPath => Path.Combine(TempWorkspacePath, "userdata"); // Path for metadata storage
        public string SourcePath => Path.Combine(TempWorkspacePath, "source"); // Path for metadata storage


        public bool IsValid => !string.IsNullOrEmpty(GameRootPath) && Directory.Exists(GameRootPath);
    }
}
