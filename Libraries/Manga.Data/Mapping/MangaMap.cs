using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manga.Data.Mapping
{
    public class MangaMap : EntityTypeConfiguration<Manga.Core.Domain.Manga>
    {
        public MangaMap()
        {
            this.ToTable("Manga");
            this.HasKey(m => m.Id);

            this.Property(m => m.PublisherId)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute("IX_MANGA_URL") { IsUnique = true, IsClustered = true, Order = 1 }));

            this.Property(m => m.URL)
                .HasColumnAnnotation(IndexAnnotation.AnnotationName,
                new IndexAnnotation(new IndexAttribute("IX_MANGA_URL") { IsUnique = true, IsClustered = true, Order = 2 }));            

            

            this.HasRequired(m => m.Publisher)
                .WithMany()
                .HasForeignKey(m => m.PublisherId)
                .WillCascadeOnDelete(false);

        }
    }
}
