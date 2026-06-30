using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AcraData.Models.ATM;

namespace AcraData.Data
{
    public partial class ATMDbContext : DbContext
    {
        public virtual DbSet<ActivityLogDetail> ActivityLogDetails { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<ActivityOrg> ActivityOrgs { get; set; }
        public virtual DbSet<ActivityPerson> ActivityPersons { get; set; }
        public virtual DbSet<ActivitySearchOrgParam> ActivitySearchOrgParams { get; set; }
        public virtual DbSet<ActivitySearchOrg> ActivitySearchOrgs { get; set; }
        public virtual DbSet<ActivitySearchPersonParam> ActivitySearchPersonParams { get; set; }
        public virtual DbSet<ActivitySearchPerson> ActivitySearchPersons { get; set; }
        public virtual DbSet<DicApplication> DicApplications { get; set; }
        public virtual DbSet<DicError> DicErrors { get; set; }
        public virtual DbSet<DicPersonType> DicPersonTypes { get; set; }
        public virtual DbSet<DicReport> DicReports { get; set; }
        public virtual DbSet<DicSource> DicSources { get; set; }
        public virtual DbSet<RepRequest> RepRequests { get; set; }

        public ATMDbContext(DbContextOptions<ATMDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLogDetail>(entity =>
            {
                entity.HasIndex(e => e.PersonType)
                    .HasName("fk_PersonType");

                entity.HasOne(d => d.Activity)
                    .WithOne(p => p.ActivityLogDetail)
                    .HasForeignKey<ActivityLogDetail>(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ActivityID");

                entity.HasOne(d => d.PersonTypeNavigation)
                    .WithMany(p => p.ActivityLogDetails)
                    .HasForeignKey(d => d.PersonType)
                    .HasConstraintName("fk_PersonType");

                entity.ToTable("ActivityLogDetails");
            });

            modelBuilder.Entity<ActivityLog>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("IX_ActivityID");

                entity.HasIndex(e => new { e.SessionId, e.UserId })
                    .HasName("IX_SessionID");

                entity.Property(e => e.IsReported).HasDefaultValueSql("'b\\'0\\''");

                entity.ToTable("ActivityLogs");
            });

            modelBuilder.Entity<ActivityOrg>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("fk_ASP_ActivityID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityOrgs)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activityorgs_ibfk_1");

                entity.ToTable("ActivityOrgs");
            });

            modelBuilder.Entity<ActivityPerson>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("fk_ASP_ActivityID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivityPersons)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activitypersons_ibfk_1");

                entity.ToTable("ActivityPersons");
            });

            modelBuilder.Entity<ActivitySearchOrgParam>(entity =>
            {
                entity.HasOne(d => d.Activity)
                    .WithOne(p => p.ActivitySearchOrgParam)
                    .HasForeignKey<ActivitySearchOrgParam>(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activitysearchorgparams_ibfk_1");
                entity.ToTable("ActivitySearchOrgParams");
            });

            modelBuilder.Entity<ActivitySearchOrg>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("fk_ASP_ActivityID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivitySearchOrgs)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("activitysearchorgs_ibfk_1");

                entity.ToTable("ActivitySearchOrgs");
            });

            modelBuilder.Entity<ActivitySearchPersonParam>(entity =>
            {
                entity.HasOne(d => d.Activity)
                    .WithOne(p => p.ActivitySearchPersonParam)
                    .HasForeignKey<ActivitySearchPersonParam>(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkASPP_ActivityID");

                entity.ToTable("ActivitySearchPersonParams");
            });

            modelBuilder.Entity<ActivitySearchPerson>(entity =>
            {
                entity.HasIndex(e => e.ActivityId)
                    .HasName("fk_ASP_ActivityID");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.ActivitySearchPersons)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ASP_ActivityID");

                entity.ToTable("ActivitySearchPersons");
            });

            modelBuilder.Entity<RepRequest>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.ActivityId });

                entity.HasIndex(e => e.ActivityId)
                    .HasName("fkRepReq_ActivityID");

                entity.Property(e => e.RequestId).ValueGeneratedOnAdd();

                entity.Property(e => e.ReqDateTime)
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.RepRequests)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkRepReq_ActivityID");

                entity.ToTable("RepRequests");
            });
        }
    }
}