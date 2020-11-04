using Manga.Core.Domain;
using System.Data.Entity.ModelConfiguration;

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
