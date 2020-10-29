using Manga.Core.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Manga.Data.Mapping
{
    public partial class ChapterMap : EntityTypeConfiguration<Chapter>
    {
        public ChapterMap()
        {
            this.ToTable("Chapter");
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.Manga)
                .WithMany(m => m.Chapters)
                .HasForeignKey(c => c.MangaId)
                .WillCascadeOnDelete(false);
        }
    }
}
