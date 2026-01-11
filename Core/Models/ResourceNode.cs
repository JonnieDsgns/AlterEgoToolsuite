using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Core.Models
{
    public partial class ResourceNode : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _uri;

        [ObservableProperty]
        private string _resourceType;

        [ObservableProperty]
        private string _folder;

        [ObservableProperty]
        private ulong _size;

        [ObservableProperty]
        private ulong _packedSize;

        [ObservableProperty]
        private bool _isModified;

        public ResourceNode() { }

        public ResourceNode(string name, string uri, string resourceType, string folder = "", ulong size = 0, ulong packedSize = 0)
        {
            Name = name;
            Uri = uri;
            ResourceType = resourceType;
            Folder = folder;
            Size = size;
            PackedSize = packedSize;
        }
    }
}

