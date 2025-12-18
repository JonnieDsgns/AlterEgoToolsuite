using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgoSDK.FileFormats
{
    public class TextureResource
    {
        // TODO: Implement DDS reading logic
        public int Width { get; set; }
        public int Height { get; set; }
        public int MipMaps { get; set; }
        public string Format { get; set; }
    }
}
