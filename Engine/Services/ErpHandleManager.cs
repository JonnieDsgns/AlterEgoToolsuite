using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                using var fs = new FileStream(fullErpPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                container.Read(fs);
                _openErpFiles.Add(fullErpPath, container);
            }

            return container;
        }

        public IEnumerable<ResourceNode> GetDetailedResources(string fullErpPath)
        {
            var container = GetErpContainer(fullErpPath);
            return container.Resources.Select(r => new ResourceNode(
                name: r.FileName,
                uri: r.Identifier,
                resourceType: r.ResourceType,
                folder: r.Folder,
                size: r.Size,
                packedSize: r.PackedSize
            )).ToList();
        }

        public ErpResource? GetResource(string fullErpPath, string identifier)
        {
            var container = GetErpContainer(fullErpPath);
            return container.Resources.FirstOrDefault(r => r.Identifier == identifier);
        }

        public void ClearHandles()
        {
            _openErpFiles.Clear();
        }
    }
}
