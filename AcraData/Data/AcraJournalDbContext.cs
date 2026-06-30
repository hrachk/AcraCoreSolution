using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AcraData.Models.AcraJournal;

namespace AcraData.Data
{
    public partial class AcraJournalDbContext : DbContext
    {
        public virtual DbSet<ATM> ATMlogs { get; set; }
        public virtual DbSet<Qkag_transaction> Qkag_Transaction { get; set; }
        public virtual DbSet<AcraData.Models.Acra4.BPR_Transaction> BPR_Transaction { get; set; }

        public virtual DbSet<Pek_Journal> Pek_Journal { get; set; }

        public AcraJournalDbContext(DbContextOptions<AcraJournalDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<ATM>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.ActivityId });

                entity.HasIndex(e => e.ActivityId)
                    .HasDatabaseName("fk_ATM_ActivityID");

                entity.Property(e => e.RequestId).ValueGeneratedOnAdd();

                entity.Property(e => e.ReqDateTime)
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();
            
                entity.ToTable("ATM");
            });

            modelBuilder.Entity<Qkag_transaction>(entity =>
            {
                entity.HasKey(e => e.ID).HasName("ID");

                entity.Property(e => e.Request).ValueGeneratedOnAdd();

                entity.Property(e => e.Response).ValueGeneratedOnAdd();

                entity.Property(e => e.ResponseDateTime).HasDefaultValueSql("'CURRENT_TIMESTAMP'").ValueGeneratedOnAdd();
            });
        }
    }
}
