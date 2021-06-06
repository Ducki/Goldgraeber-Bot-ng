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

        public virtual DbSet<Response> Responses { get; set; }
        public virtual DbSet<Trigger> Triggers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Data Source=text.sqlite");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Response>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("integer")
                    .HasColumnName("id");

                entity.Property(e => e.ResponseText)
                    .HasColumnType("varchar")
                    .HasColumnName("response_text");

                entity.Property(e => e.TriggerId)
                    .HasColumnType("integer")
                    .HasColumnName("trigger_id");

                entity.HasOne(d => d.Trigger)
                    .WithMany(p => p.Responses)
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
