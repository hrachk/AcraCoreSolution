using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AcraData.Models.Trigger;

namespace AcraData.Data
{
    public partial class TriggerDbContext : DbContext
    {       
        public virtual DbSet<DicTriggerReportReason> DicTriggerReportReasons { get; set; }     
        public virtual DbSet<TriggerPerson> TriggerPersons { get; set; }
        public virtual DbSet<TriggerPersonsDetail> TriggerPersonsDetails { get; set; }
        public virtual DbSet<TriggerReport> TriggerReports { get; set; }
        public virtual DbSet<TriggerSource> TriggerSources { get; set; }
        public virtual DbSet<TriggerVolume> TriggerVolumes { get; set; }
        public virtual DbSet<TriggerTmp> TriggerTmps { get; set; }
        public virtual DbSet<DicOnTriggerFields> DicOnTriggerFields { get; set; }

        public TriggerDbContext(DbContextOptions<TriggerDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<DicTriggerReportReason>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReportReasonId)
                    .HasColumnName("ReportReasonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReportSubReasonId)
                    .HasColumnName("ReportSubReasonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SourceId)
                    .HasColumnName("SourceId")
                    .HasColumnType("int(11)");

                entity.ToTable("DicTriggerReportReasons");
            });
          
            modelBuilder.Entity<TriggerPerson>(entity =>
            {
                entity.HasIndex(e => e.Tsid)
                    .HasName("fk_TSID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Tsid)
                 .HasColumnName("TSID")
                 .HasColumnType("int(11)");

                entity.Property(e => e.PersonId)
                    .HasColumnName("PersonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PersonType).HasColumnType("int(11)");

                entity.Property(e => e.SysDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.HasOne(d => d.TriggerSource)
                    .WithMany(p => p.TriggerPersons)
                    .HasForeignKey(d => d.Tsid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_TSID");

                entity.HasIndex(e => e.Status)
                   .HasName("IX_Status");

                entity.ToTable("TriggerPersons");
            });

            modelBuilder.Entity<TriggerPersonsDetail>(entity =>
            {
                entity.HasIndex(e => e.Tpid)
                    .HasName("fk_TPID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.PersonId)
                    .HasColumnName("PersonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SysDate).HasColumnType("datetime");

                entity.Property(e => e.Tpid)
                    .HasColumnName("TPID")
                    .HasColumnType("bigint(20)");

                entity.HasOne(d => d.TriggerPerson)
                    .WithMany(p => p.TriggerPersonsDetails)
                    .HasForeignKey(d => d.Tpid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_TPID");

                entity.ToTable("TriggerPersonsDetails");
            });

            modelBuilder.Entity<TriggerReport>(entity =>
            {
                entity.HasIndex(e => e.Tsid)
                    .HasName("fk_RepTSID");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.ReasonId).HasColumnType("int(11)");

                entity.Property(e => e.ReportId).HasColumnType("int(11)");

                entity.Property(e => e.ReportInfo).HasColumnType("text");

                entity.Property(e => e.SourceId).HasColumnType("int(11)");

                entity.Property(e => e.SubReasonId).HasColumnType("int(11)");

                entity.Property(e => e.SysDate).HasColumnType("datetime");

                entity.Property(e => e.Tsid)
                    .HasColumnName("TSID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserActivityId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.TriggerSource)
                    .WithMany(p => p.TriggerReports)
                    .HasForeignKey(d => d.Tsid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_RepTSID");

                entity.ToTable("TriggerReports");
            });

            modelBuilder.Entity<TriggerSource>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Filter)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.SourceId)
                    .HasColumnName("SourceID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.ToTable("TriggerSources");
            });

            modelBuilder.Entity<TriggerVolume>(entity =>
            {
                entity.HasIndex(e => e.Tsid)
                    .HasName("fk_SourceVolume");

                entity.HasIndex(e => new { e.Date, e.Tsid })
                    .HasName("pk_Date")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Tsid)
                    .HasColumnName("TSID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Volume).HasColumnType("int(11)");

                entity.HasOne(d => d.Ts)
                    .WithMany(p => p.TriggerVolumes)
                    .HasForeignKey(d => d.Tsid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_SourceVolume");

                entity.ToTable("TriggerVolumes");
            });

            modelBuilder.Entity<TriggerTmp>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("ActivityID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ActivityType).HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.ToTable("TriggerTmp");
            });

            modelBuilder.Entity<DicOnTriggerFields>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ColumnName).HasMaxLength(50);

                entity.Property(e => e.TableName).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(100);
            });
        }
    }
}
