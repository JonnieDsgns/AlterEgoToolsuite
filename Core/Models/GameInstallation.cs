using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Core.Models
{
    public partial class GameInstallation : ObservableObject
    {
        [ObservableProperty] // The title of the game
        private string _gameTitle;

        [ObservableProperty] // The root path where the game is installed
        private string _rootPath;

        [ObservableProperty] // The source or platform of the game installation (e.g., Steam, Epic Games, EA, GOG)
        private string _source;

        [ObservableProperty] // The version identifier of the game installation. Used to match project files to correct game version.
        private string _versionIdentifier;

        public bool IsValid => !string.IsNullOrEmpty(RootPath) && System.IO.Directory.Exists(RootPath);
    }
}
