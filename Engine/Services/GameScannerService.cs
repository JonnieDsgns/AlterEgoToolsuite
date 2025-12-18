using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Models;

namespace Engine.Services
{
    public class GameScannerService
    {
        public IEnumerable<GameInstallation> ScanForGames()
        {
            // Check for common installation paths for supported games
            // Placeholder implementation
            return new List<GameInstallation>
            {
                new GameInstallation {GameTitle = "F1 25",
                    RootPath = @"C:\Program Files (x86)\Steam\steamapps\common\F1 25",
                    Source = "Steam",
                    VersionIdentifier = "1.0.0"
                }
            };
        }
    }
}
