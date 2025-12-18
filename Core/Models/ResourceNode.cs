using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        // Like FileNode, you might want to track if a specific resource is edited
        [ObservableProperty]
        private bool _isModified;

        public ResourceNode() { }

        public ResourceNode(string name, string uri, string resourceType)
        {
            Name = name;
            Uri = uri;
            ResourceType = resourceType;
        }
    }
}

