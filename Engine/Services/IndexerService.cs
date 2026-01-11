using System;
using System.Collections.Generic;
using System.IO;
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

        private static readonly (string displayName, string filterType)[] _erpCategories =
        {
            ("Textures", "GfxSRVResource"),
            ("Materials", "GfxMaterialRes"),
            ("Models", "GfxMeshRes"),
            ("Xml Files", "VTF"),
            ("Pkg Files", "World, WoInstances, EventGraph"),
            ("Other", null) // null means "everything else"
        };

        public (string displayName, string filterType)[] ErpCategories => _erpCategories;

        public ObservableCollection<FileNode> BuildIndex(string gameRootPath)
        {
            var rootNodes = new ObservableCollection<FileNode>();

            if (!Directory.Exists(gameRootPath))
                return rootNodes; // returns the root Folder of the game installation

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

            // Special handling for ERP files
            if (extension == ".erp")
            {
                return CreateErpNode(file, parent);
            }

            // Standard file handling
            return new FileNode(file.Name, file.FullName, GetResourceTypeFromExtension(file.Name), parent);
        }

        public IEnumerable<ResourceNode> GetDetailedResources(string path)
        {
            // This calls the method in ErpHandleManager
            return _erpManager.GetDetailedResources(path);
        }

        private FileNode CreateErpNode(FileInfo file, FileNode parent)
        {
            var erpNode = new FileNode(file.Name, file.FullName, "ERP", parent);
            // Add a placeholder child to indicate lazy loading is needed
            erpNode.Children.Add(new FileNode("Loading...", "", "Placeholder", erpNode));
            return erpNode;
        }

        public void LoadErpCategories(FileNode erpNode)
        {
            if (erpNode == null || !erpNode.ResourceType.Equals("ERP", StringComparison.OrdinalIgnoreCase))
                return;

            // If already loaded, don't reload
            if (erpNode.IsLoaded)
                return;

            // Clear any placeholder children
            erpNode.Children.Clear();

            // Get all resources from the ERP file
            var allResources = GetDetailedResources(erpNode.FullPath).ToList();
            
            // Group resources by category
            foreach (var (displayName, filterType) in _erpCategories)
            {
                IEnumerable<ResourceNode> categoryResources;
                
                if (filterType == null)
                {
                    // "Other" category - get resources that don't match any other category
                    var matchedTypes = new HashSet<string>();
                    foreach (var (_, type) in _erpCategories.Where(c => c.filterType != null))
                    {
                        var types = type.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var t in types)
                            matchedTypes.Add(t);
                    }
                    categoryResources = allResources.Where(r => !matchedTypes.Contains(r.ResourceType)).ToList();
                }
                else
                {
                    // Parse the filter type (could be comma-separated like "World, WoInstances, EventGraph")
                    var filterTypes = filterType.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    categoryResources = allResources.Where(r => filterTypes.Contains(r.ResourceType)).ToList();
                }

                // Only add category if it has resources
                if (categoryResources.Any())
                {
                    var categoryNode = new FileNode(displayName, "", "Category", erpNode);
                    
                    // Add resources as children of the category
                    foreach (var resource in categoryResources)
                    {
                        var resourceNode = new FileNode(
                            resource.Name,
                            erpNode.FullPath, // Store the ERP path so we can extract resources later
                            resource.ResourceType,
                            categoryNode
                        );
                        categoryNode.Children.Add(resourceNode);
                    }
                    
                    erpNode.Children.Add(categoryNode);
                }
            }

            erpNode.IsLoaded = true;
        }

        public void ClearIndex()
        {
            _erpManager.ClearHandles();
        }
        
        private string GetResourceTypeFromExtension(string fileName)
        {
            if (fileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) return "binXML";
            if (fileName.EndsWith(".dds", StringComparison.OrdinalIgnoreCase)) return "dds";
            if (fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) return "png";
            if (fileName.EndsWith(".erp", StringComparison.OrdinalIgnoreCase)) return "erp";
            if (fileName.EndsWith(".pkg", StringComparison.OrdinalIgnoreCase)) return "pkg";
            if (fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) return "dll";
            if (fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) return "exe";
            if (fileName.EndsWith(".dat", StringComparison.OrdinalIgnoreCase)) return "dat";
            if (fileName.EndsWith(".daf", StringComparison.OrdinalIgnoreCase)) return "daf";
            if (fileName.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase)) return "cfg";
            return "Unknown";
        }
    }
}
