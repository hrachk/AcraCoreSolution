using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AcraData.Models.Acra4;
using System.Linq;
namespace AcraData.Data
{
    public partial class Acra4DbContext : DbContext
    {       
        

        public virtual DbSet<ACRAIdentity> ACRAIdentities { get; set; }
        public virtual DbSet<ACRALegalMapper> ACRALegalMappers { get; set; }
        public virtual DbSet<ACRALegalMapperActivity> ACRALegalMapperActivities { get; set; }
        public virtual DbSet<ACRAPersonMapper> ACRAPersonMappers { get; set; }
        public virtual DbSet<ACRAPersonMapperActivity> ACRAPersonMapperActivities { get; set; }
        public virtual DbSet<ActualAddress> ActualAddresses { get; set; }
        public virtual DbSet<BPR_Addresses> BPR_Addresses { get; set; }
        public virtual DbSet<BPR_Documents> BPR_Documents { get; set; }      
        public virtual DbSet<BPR_Transaction> BPR_Transaction { get; set; }
        public virtual DbSet<BPR_Persons> BPR_Persons { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationOwner> OrganizationOwners { get; set; }
        public virtual DbSet<BPR_DocumentTypes> BPR_DocumentTypes { get; set; }
        public virtual DbSet<BPR_Genders> BPR_Genders { get; set; }
        public virtual DbSet<PekJournal> PekJournal { get; set; }

        public Acra4DbContext(DbContextOptions<Acra4DbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<BPR_Addresses>(entity =>
            {              
                entity.HasOne(e => e.BPR_Persons)
                .WithOne(p => p.BPR_Address)
                .HasForeignKey<BPR_Addresses>(f => f.AVVPersonID)
                .HasConstraintName("FK_AddPerson");               
            });

            

            modelBuilder.Entity<BPR_Documents>(entity =>
            {
                entity.HasIndex(e => e.DocumentNumber).HasDatabaseName("IX_DocNum");
                entity.HasIndex(e => new {e.AVVPersonID, e.DocumentType, e.DocumentNumber }).HasDatabaseName("IX_DocNumDocType").IsUnique();

               
                entity.HasOne(d => d.BPR_Persons)
                .WithMany(p => p.BPR_Documents)
                .HasForeignKey(f => f.AVVPersonID)
                .HasConstraintName("FK_DocPerson");
            });

           


            //AB: Check FK_PersonACRAID constraint
            modelBuilder.Entity<BPR_Persons>(entity =>
            {
                entity.HasIndex(e => e.PNum).HasDatabaseName("IX_PNum").IsUnique();
                entity.HasIndex(e => e.ACRAID).HasDatabaseName("IX_AVVACRAID");
              
            });

            modelBuilder.Entity<BPR_Addresses>()
                .Property(e => e.AVVGetDateTime)
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

            modelBuilder.Entity<BPR_Documents>()
                .Property(e => e.AVVGetDateTime)
                .HasDefaultValueSql("'CURRENT_TIMESTAMP'");


            modelBuilder.Entity<ACRAPersonMapper>(entity =>
            {
                entity.HasKey(e => e.PersonID).HasName("IX_PersonID");
                entity.HasIndex(e => e.IncomingDate).HasDatabaseName("IX_IncomingDate");
                entity.HasIndex(e => e.Status).HasDatabaseName("Status");
                entity.HasIndex(e => new { e.ACRAID, e.PersonID }).HasDatabaseName("IX_ACRAID");
                entity.HasIndex(e => new { e.StageID, e.ACRAID, e.PersonID }).HasDatabaseName("IX_Stage");

                entity.ToTable("ACRAPersonMapper");
            });

            modelBuilder.Entity<ACRALegalMapper>(entity =>
            {
                entity.HasKey(e => e.OrganizationID).HasName("IX_OrganizationID");
                entity.HasIndex(e => e.IncomingDate).HasDatabaseName("IX_IncomingDate");
                entity.HasIndex(e => e.Status).HasDatabaseName("Status");
                entity.HasIndex(e => new { e.ACRAID, e.OrganizationID }).HasDatabaseName("IX_ACRAID");
                entity.HasIndex(e => new { e.StageID, e.ACRAID, e.OrganizationID }).HasDatabaseName("IX_Stage");

                entity.ToTable("ACRALegalMapper");
            });


            modelBuilder.Entity<ACRAIdentity>(entity =>
            {
                entity.Property(e => e.ACRAGroup).HasColumnType("varchar(20)");

                entity.HasKey(e => e.ACRAID);
                entity.HasIndex(e => e.ACRAID).HasDatabaseName("IX_ACRAID");
               
                entity.ToTable("ACRAIdentity");
            });
            


        }
    }
}
