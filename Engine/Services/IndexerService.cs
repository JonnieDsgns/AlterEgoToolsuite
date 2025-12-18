using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Core.Models;

namespace Engine.Services
{
    public class IndexerService
    {
        private readonly ErpHandleManager _erpManager;

        // Dependency Injection
        public IndexerService(ErpHandleManager erpManager)
        {
            _erpManager = erpManager ?? throw new ArgumentNullException(nameof(erpManager));
        }

        public ObservableCollection<FileNode> BuildIndex(string gameRootPath)
        {
            var rootNodes = new ObservableCollection<FileNode>();

            if (!Directory.Exists(gameRootPath))
                return rootNodes;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(gameRootPath);

                // 1. Get all subdirectories in the game root
                foreach (var dir in dirInfo.GetDirectories())
                {
                    rootNodes.Add(CrawlDirectory(dir));
                }

                // 2. Get all files in the game root (that aren't in subfolders)
                foreach (var file in dirInfo.GetFiles())
                {
                    rootNodes.Add(CreateFileNode(file, null));
                }
            }
            catch (Exception ex)
            {
                // Handle unauthorized access or IO errors
            }

            return rootNodes;
        }

        private FileNode CrawlDirectory(DirectoryInfo dir, FileNode parent = null)
        {
            var folderNode = new FileNode(dir.Name, dir.FullName, "Folder", parent);

            // Recursively add subfolders
            foreach (var subDir in dir.GetDirectories())
            {
                folderNode.Children.Add(CrawlDirectory(subDir, folderNode));
            }

            // Add files in this folder
            foreach (var file in dir.GetFiles())
            {
                folderNode.Children.Add(CreateFileNode(file, folderNode));
            }

            return folderNode;
        }

        private FileNode CreateFileNode(FileInfo file, FileNode parent)
        {
            string extension = file.Extension.ToLower();

            // Standard file handling
            return new FileNode(file.Name, file.FullName, GetResourceTypeFromExtension(file.Name), parent);
        }

        public IEnumerable<string> GetErpResourceNames(string path) 
        {
            return _erpManager.GetInternalResourceNames(path);
        }

        public IEnumerable<ResourceNode> GetDetailedResources(string path)
        {
            // This calls the method you already wrote in ErpHandleManager
            return _erpManager.GetDetailedResources(path);
        }

        private FileNode CreateErpNode(FileInfo file, FileNode parent)
        {
            var erpNode = new FileNode(file.Name, file.FullName, "ERP", parent);

            // This is where we handle ERP resources. 
            // We can change this logic later to be "On-Demand" (lazy loading)
            try
            {
                var internalResources = _erpManager.GetInternalResourceNames(file.FullName);
                foreach (var resName in internalResources)
                {
                    var resNode = new FileNode(
                        resName,
                        $"{file.FullName}::{resName}",
                        GetResourceTypeFromExtension(resName),
                        erpNode);
                    erpNode.Children.Add(resNode);
                }
            }
            catch
            {
                // Add an error node or leave empty if ERP is corrupted
            }

            return erpNode;
        }

        public void ClearIndex()
        {
            _erpManager.ClearHandles();
        }
        
        private string GetResourceTypeFromExtension(string fileName)
        {
            if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) return "BinXML";
            if (fileName.EndsWith(".dds", StringComparison.OrdinalIgnoreCase)) return "Texture";
            if (fileName.EndsWith(".erp", StringComparison.OrdinalIgnoreCase)) return "erp";

            return "Unknown";
        }
    }
}
