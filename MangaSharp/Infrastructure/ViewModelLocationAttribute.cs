using System;
using System.ComponentModel.Composition;

namespace MangaSharp.Infrastructure
{
    [MetadataAttribute]
    public class ViewModelLocationAttribute : Attribute
    {
        public string Location { get; set; }

        public ViewModelLocationAttribute(string location)
        {
            this.Location = location;
        }
    }
}
