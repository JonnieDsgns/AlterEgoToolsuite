using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Core.Models
{
    public partial class AppConfiguration : ObservableObject
    {
        [ObservableProperty] // List of all detected game installations
        private ObservableCollection<GameInstallation> _detectedGames = new();
    }
}
