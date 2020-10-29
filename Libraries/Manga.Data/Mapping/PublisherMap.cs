using Manga.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manga.Data.Mapping
{
    public class PublisherMap : EntityTypeConfiguration<Publisher>
    {
        public PublisherMap()
        {
            this.ToTable("Publisher");
            this.HasKey(p => p.Id);

            this.Property(p => p.Name).IsRequired().HasMaxLength(255);
            this.Property(p => p.BaseUrl).IsRequired();
        }
    }
}
