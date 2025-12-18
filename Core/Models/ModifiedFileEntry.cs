using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ModifiedFileEntry
    {
        public string ModifiedFileName { get; set; }  // Name in the 'source' folder
        public string OriginalFileName { get; set; }  // Original filename in Game
        public string EgoIdentifier { get; set; }     // URI (EgoEngineLibrary)
        public string TargetErpPath { get; set; }     // Relative path to the ERP
    }
}
