using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Models;

using EgoEngineLibrary.Archive.Erp;

namespace Engine.Services
{
    public class ErpHandleManager
    {
        private readonly Dictionary<string, ErpFile> _openErpFiles = new();

        private ErpHandleManager() { }

        public static ErpHandleManager Instance { get; } = new ErpHandleManager();

        public ErpFile GetErpContainer(string fullErpPath)
        {
            if (!_openErpFiles.TryGetValue(fullErpPath, out var container))
            {
                container = new ErpFile();
                using (var fs = new FileStream(fullErpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    container.Read(fs);
                }

                _openErpFiles.Add(fullErpPath, container);
            }

            return container;
        }

        public IEnumerable<string> GetInternalResourceNames(string fullErpPath)
        {
            try
            {
                var container = GetErpContainer(fullErpPath);
                return container.Resources.Select(f => f.Identifier);

            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        public IEnumerable<ResourceNode> GetDetailedResources(string fullErpPath)
        {
            var container = GetErpContainer(fullErpPath);
            return container.Resources.Select(r => new ResourceNode
            {
                Name = r.FileName,
                ResourceType = r.ResourceType,
                Uri = r.Identifier
            }).ToList(); // .ToList() is important to finalize the collection for the UI [cite: 1]
        }

        public void ClearHandles()
        {
            foreach (var erpFile in _openErpFiles.Values)
            {
                
            }
            _openErpFiles.Clear();
        }
    }
}
