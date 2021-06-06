using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Bot_Dotnet
{
    public partial class textContext : DbContext
    {
        public textContext()
        {
        }

        public textContext(DbContextOptions<textContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Trigger> Triggers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("Data Source=/Users/alex/Dev/Bot-Dotnet/text.sqlite");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("integer")
                    .HasColumnName("id");

                entity.Property(e => e.Answer1)
                    .HasColumnType("varchar")
                    .HasColumnName("answer");

                entity.Property(e => e.TriggerId)
                    .HasColumnType("integer")
                    .HasColumnName("trigger_id");

                entity.HasOne(d => d.Trigger)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.TriggerId);
            });

            modelBuilder.Entity<Trigger>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Searchstring)
                    .HasColumnType("VARCHAR")
                    .HasColumnName("searchstring");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
