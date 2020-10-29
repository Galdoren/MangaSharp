using Manga.Core.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Manga.Data.Mapping
{
    public partial class ChapterImageMap : EntityTypeConfiguration<ChapterImage>
    {
        public ChapterImageMap()
        {
            this.ToTable("Image");
            this.HasKey(ci => ci.Id);

            this.Ignore(ci => ci.LinkType);

            this.HasRequired(ci => ci.Chapter)
                .WithMany()
                .HasForeignKey(ci => ci.ChapterId)
                .WillCascadeOnDelete(false);
        }
    }
}
