using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgoSDK.FileFormats
{
    public class ERPFile
    {
        public string FilePath { get; private set; }
        public List<ERPResource> Resources { get; private set; }

        public ERPFile(string filePath)
        {
            FilePath = filePath;
            Resources = new List<ERPResource>();
        }

        public void Read()
        {
            // TODO: Implement actual binary parsing
            // For now, we will simulate some resources for testing UI
            
            Resources.Clear();
            
            // Simulate some textures
            Resources.Add(new ERPResource { Name = "test_texture_01.dds", Type = ResourceType.Texture, Size = 1024 });
            Resources.Add(new ERPResource { Name = "car_paint.dds", Type = ResourceType.Texture, Size = 2048 });

            // Simulate some PKG files
            Resources.Add(new ERPResource { Name = "config.pkg", Type = ResourceType.Pkg, Size = 512 });

            // Simulate some XML files
            Resources.Add(new ERPResource { Name = "settings.xml", Type = ResourceType.Xml, Size = 128 });

            // Simulate other files
            Resources.Add(new ERPResource { Name = "unknown.bin", Type = ResourceType.Other, Size = 256 });
        }
    }

    public enum ResourceType
    {
        Texture,
        Pkg,
        Xml,
        Other
    }

    public class ERPResource
    {
        public string Name { get; set; }
        public ResourceType Type { get; set; }
        public long Size { get; set; }
        public long Offset { get; set; } // Offset in the ERP file
    }
}
