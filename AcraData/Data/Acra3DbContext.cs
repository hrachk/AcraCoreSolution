using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AcraData.Models.Acra3;
using System.Linq;

namespace AcraData.Data
{
    public partial class Acra3DbContext : DbContext
    {
        public virtual DbSet<IdCard> IdCards { get; set; }
        public virtual DbSet<LoginLog> LoginLogs { get; set; }
        public virtual DbSet<Passport> Passports { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<SourceReference> SourceReferences { get; set; }
        public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public virtual DbSet<UserActivityParam> UserActivityParams { get; set; }
        public virtual DbSet<UserActivityReportID> UserActivityReportIDs { get; set; }
        public virtual DbSet<UserInfo> UserInfos { get; set; }
        public virtual DbSet<CleansingHistory> CleansingHistories { get; set; }     
        public virtual DbSet<DicFirstName> DicFirstNames { get; set; }
        public virtual DbSet<DicLastName> DicLastNames { get; set; }
        public virtual DbSet<Predicat> Predicats { get; set; }
        public virtual DbSet<DicReportReason> DicReportReasons { get; set; }
        public virtual DbSet<DicReport> DicReports { get; set; }
        public virtual DbSet<DicReportSubReason> DicReportSubReasons { get; set; }        
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<Credit> Credits { get; set; }
        public virtual DbSet<CreditOwner> CreditOwners { get; set; }
        public virtual DbSet<GuaranteeCancellation> GuaranteeCancellations { get; set; }
        public virtual DbSet<Guarantor> Guarantors { get; set; }
        public virtual DbSet<OrgOwner> OrgOwners { get; set; }
        public virtual DbSet<DicCountries> DicCountries { get; set; }
        public virtual DbSet<DicResident> DicResident { get; set; }
        public virtual DbSet<DicSex> DicSex { get; set; }
        public virtual DbSet<DicSynonym> DicSynonyms { get; set; }
        public virtual DbSet<OrganizationNames> OrganizationNames { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<LoanActivityTmp> LoanActivityTmps { get; set; }
        public virtual DbSet<TriggerActivityTemp> TriggerActivityTmps { get; set; }
        public virtual DbSet<MonitoringPlusActivityTemp> MonitoringPlusActivityTmps { get; set; }
        public virtual DbSet<CreditStatusLog> CreditStatusLogs { get; set; }
        public virtual DbSet<ReceivedPackage> ReceivedPackages { get; set; }
        public virtual DbSet<PackageFile> PackageFiles { get; set; }
        public virtual DbSet<ReceivedPacket> ReceivedPackets { get; set; }
        public virtual DbSet<ScorePerson> ScorePersons { get; set; }
        public virtual DbSet<AcraPersonsBySource> AcraPersonsBySources { get; set; }
        public virtual DbSet<MonitoringPlusByAcraID> MonitoringPlusByAcraID { get; set; }
        public virtual DbSet<BankIDs> BankIDs { get; set; }
        public virtual DbSet<BankIDLegal> BankIDLegals { get; set; }
        public virtual DbSet<MonitoringPlus_Source> MonitoringPlus_Sources { get; set; }
        public virtual DbSet<UserInterfacePrivilege> UserInterfacePrivileges { get; set; }
        public virtual DbSet<Pek_Definition> Pek_Definitions { get; set; }
        public virtual DbSet<Pek_ActivityLog> Pek_ActivityLogs { get; set; }

        public virtual DbSet<DeadPersons> DeadPersons { get; set; }
        // public virtual DbSet<ACRAPersonMapper> ACRAPersonMappers { get; set; }
        public virtual DbSet<ACRAID_MAPPER> ACRAID_MAPPER { get; set; }
        public virtual DbSet<AcraID_Errors> AcraID_Errors { get; set; }

        public Acra3DbContext(DbContextOptions<Acra3DbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CreditStatusLog>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CreditId)
                    .HasColumnName("CreditID")
                     .HasColumnType("bigint(20)");

                entity.Property(e => e.FirstState_InternalID).HasColumnType("int(11)");
                entity.Property(e => e.OldValue).HasColumnType("int(11)");
                entity.Property(e => e.NewValue).HasColumnType("int(11)");

                entity.Property(e => e.StatusModifyDate).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.HasIndex(e => new { e.CreditId, e.StatusModifyDate})
                    .HasDatabaseName("IX_Credit");


                entity.ToTable("CreditStatusLogs");
            });


            modelBuilder.Entity<TriggerActivityTemp>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("ActivityID")
                     .HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityType).HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.ToTable("TriggerActivityTemp");
            });

            modelBuilder.Entity<MonitoringPlusActivityTemp>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityId)
                    .HasColumnName("ActivityID")
                     .HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityType).HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.ToTable("MonitoringPlusActivityTemp");
            });


            modelBuilder.Entity<IdCard>(entity =>
            {
                entity.HasIndex(e => e.IdCardNum)
                    .HasDatabaseName("IdCards_IdCardNum");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("IdCards_PersonID");

                //entity.HasIndex(e => e.SourceId)
                //    .HasDatabaseName("idcards_ibfk_2");

                entity.HasIndex(e => new { e.PersonId, e.IdCardNum })
                    .HasDatabaseName("PersonID_IdCardNum");

                entity.Property(e => e.IncomingDate).HasDefaultValueSql("'0000-00-00 00:00:00'");

                //entity.HasOne(e => e.Person)
                //    .WithMany(p => p.IdCards)
                //    .HasForeignKey(e => e.PersonId)
                //    .HasConstraintName("idcards_ibfk_1");

                entity.ToTable("IdCards");
            });

            modelBuilder.Entity<LoanActivityTmp>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.CreditID)
                    .HasColumnName("CreditID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityType).HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.ToTable("LoanActivityTmp");
            });


            modelBuilder.Entity<LoginLog>(entity =>
            {
                entity.HasIndex(e => e.Ipaddress)
                    .HasDatabaseName("IPAddress");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("UserID_2");

                entity.HasIndex(e => e.UserLogin)
                    .HasDatabaseName("UserLogin");

                entity.ToTable("LoginLogs");
            });

            modelBuilder.Entity<Passport>(entity =>
            {
                entity.HasIndex(e => e.PassportNum)
                    .HasDatabaseName("Passports_PassportNum");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("Passports_PersonID");

                entity.HasIndex(e => e.SourceId)
                    .HasDatabaseName("passports_ibfk_10");

                entity.HasIndex(e => new { e.PersonId, e.PassportNum })
                    .HasDatabaseName("PersonID_PassportNum");

                entity.Property(e => e.IncomingDate).HasDefaultValueSql("'0000-00-00 00:00:00'");

                //entity.HasOne(d => d.Person)
                //    .WithMany(p => p.Passports)
                //    .HasForeignKey(d => d.PersonId)
                //    .HasConstraintName("passports_ibfk_11");

                entity.ToTable("Passports");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasIndex(e => e.FirstName)
                    .HasDatabaseName("FirstName");

                entity.HasIndex(e => e.LastName)
                    .HasDatabaseName("LastName");
                
                entity.HasIndex(e => e.BirthDate)
                    .HasDatabaseName("BirthDate");

                entity.HasIndex(e => new { e.FirstName, e.LastName, e.BirthDate, e.SocialCard })
                    .HasDatabaseName("PersonID");

                entity.HasIndex(e => e.SocialCard)
                    .HasDatabaseName("SocialCard");

                entity.HasIndex(e => e.SourceId)
                    .HasDatabaseName("SourceID");

                //entity.HasIndex(e => e.BlockStatus)
                //    .HasDatabaseName("BlockStatus");

                 //entity.Property(e => e.BlockStatus).HasColumnName("BlockStatus").HasDefaultValueSql("'0'");

                entity.Property(e => e.IncomingDate).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.ToTable("Persons");
            });

            modelBuilder.Entity<SourceReference>(entity =>
            {
                entity.HasKey(e => new { e.RecordId, e.ReferenceTable, e.SourceId });

                entity.HasIndex(e => e.IncomingDate)
                    .HasDatabaseName("IncomingDate");

                entity.HasIndex(e => e.ReceivedPackageId)
                    .HasDatabaseName("ReceivedPackageID");

                entity.HasIndex(e => e.RecordId)
                    .HasDatabaseName("SourceReference_RecordID");

                entity.HasIndex(e => e.ReferenceTable)
                    .HasDatabaseName("SourceReference_ReferenceTable");

                entity.HasIndex(e => e.SourceId)
                    .HasDatabaseName("SourceReference_SourceID");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("SourceReference_Status");

                entity.HasIndex(e => new { e.ReferenceTable, e.Status, e.SourceId })
                    .HasDatabaseName("source_ref_status");

                entity.Property(e => e.RecordId).HasDefaultValueSql("'0'");

                entity.Property(e => e.ReferenceTable).HasDefaultValueSql("'0'");

                entity.Property(e => e.SourceId).HasDefaultValueSql("'0'");

                entity.Property(e => e.Status).HasDefaultValueSql("'1'");

                entity.ToTable("SourceReference");
            });

            modelBuilder.Entity<UserActivityLog>(entity =>
            {
                entity.HasIndex(e => e.ActivityTime)
                    .HasDatabaseName("ActivityTime");

                entity.HasIndex(e => e.ActivityType)
                    .HasDatabaseName("ActivityType");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("UserID");

                entity.Property(e => e.ActivityTime).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.ActivityType).HasDefaultValueSql("'0'");

                entity.Property(e => e.CleansingStatus).HasDefaultValueSql("'1'");

                entity.Property(e => e.UserId).HasDefaultValueSql("'0'");

                entity.ToTable("UserActivityLog");
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .HasDatabaseName("UserInfo_ClientId");

                entity.HasIndex(e => e.UserLogin)
                    .HasDatabaseName("UserInfo_UserLogin");

                entity.HasIndex(e => e.UserPassword)
                    .HasDatabaseName("UserInfo_UserPassword");

                entity.Property(e => e.Ipaddress).HasDefaultValueSql("''");

                entity.ToTable("UserInfo");
            });

            modelBuilder.Entity<UserInterfacePrivilege>(entity =>
              {
                  entity.HasIndex(e => e.UserID).HasDatabaseName("UserInterfeacePrivileges_UserID");
                  entity.HasKey(e => new { e.UserID, e.InterfeaceID, e.Actions })
                  .HasName("UserInterfacePrivilege_primary");
                  entity.ToTable("UserInterfeacePrivileges");
              }
                );

            modelBuilder.Entity<UserActivityParam>(entity =>
            {
                entity.HasIndex(e => e.UserActivityId)
                    .HasDatabaseName("UserActivityId");

                entity.HasIndex(e => e.UserActivityParamId)
                    .HasDatabaseName("UserActivityParamID");

                entity.HasIndex(e => e.UserActivityParamValue)
                    .HasDatabaseName("UserActivityParamValue");

                entity.HasIndex(e => new { e.UserActivityId, e.UserActivityParamId })
                    .HasDatabaseName("UserActivityId_ParamID");

                entity.HasIndex(e => new { e.UserActivityParamId, e.UserActivityParamValue })
                    .HasDatabaseName("UserActivityParamID_value");

                entity.HasKey(e => new { e.UserActivityId,e.UserActivityParamId, e.UserActivityParamValue })
                   .HasName("UserActivityParamID_primary");

                entity.Property(e => e.Status)
                              .HasColumnName("Status")
                              .HasColumnType("tinyint(4) unsigned")
                              .HasDefaultValueSql("1");
               
                entity.Property(e => e.UserActivityId).HasDefaultValueSql("'0'");

                entity.Property(e => e.UserActivityParamId).HasDefaultValueSql("'0'");

                entity.Property(e => e.UserActivityParamValue).HasDefaultValueSql("'0'");
                

                //entity.HasOne(e => e.UserActivityLog)
                //    .WithMany(u => u.UserActivityParams)
                //    .HasForeignKey(e => e.UserActivityId)
                //    .OnDelete(DeleteBehavior.Restrict)
                    //.HasConstraintName("FK_UserActivityParams");
                //.HasConstraintName("useractivityparams_ibfk_1");

                entity.ToTable("UserActivityParams");
            });

            modelBuilder.Entity<UserActivityReportID> (entity =>
            {
                entity.HasKey(e => e.UserActivityId);                   
              
                entity.ToTable("UserActivityReportID");
            });

            modelBuilder.Entity<CleansingHistory>(entity =>
            {
                entity.HasIndex(e => e.ClerckId)
                    .HasDatabaseName("ClerckID");

                entity.HasIndex(e => e.NewId)
                    .HasDatabaseName("NewID");

                entity.HasIndex(e => e.RemovedId)
                    .HasDatabaseName("RemovedID");

                entity.HasIndex(e => e.SourceId)
                    .HasDatabaseName("SourceID");

                entity.ToTable("CleansingHistory");
            });          
            
            modelBuilder.Entity<DicFirstName>(entity =>
            {
                entity.HasKey(e => e.FirstNameId);

                entity.HasIndex(e => e.FirstName)
                    .HasDatabaseName("FirstName");

                entity.Property(e => e.FirstNameId)
                    .HasColumnName("FirstNameID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("''");

                entity.ToTable("DicFirstNames");
            });

            modelBuilder.Entity<DicLastName>(entity =>
            {
                entity.HasKey(e => e.LastNameId);

                entity.HasIndex(e => e.LastName)
                    .HasDatabaseName("LastName");

                entity.Property(e => e.LastNameId)
                    .HasColumnName("LastNameID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("''");

                entity.ToTable("DicLastNames");
            });          
     
            modelBuilder.Entity<Predicat>(entity =>
            {
                entity.HasKey(e => e.ID);              

                entity.HasIndex(e => new { e.PersonId, e.PassportNum, e.IdCardNum, e.FirstNameID, e.LastNameID, e.SocialCard })
                    .HasDatabaseName("IX_Primary").IsUnique(true);

                entity.HasIndex(e => new { e.PersonId, e.FirstNameID})
                    .HasDatabaseName("IX_FirstName");

                entity.HasIndex(e => new { e.PersonId, e.LastNameID })
                    .HasDatabaseName("IX_LastName");

                entity.HasIndex(e => new { e.PersonId, e.PassportNum })
                    .HasDatabaseName("IX_Passport");

                entity.HasIndex(e => new { e.PersonId, e.IdCardNum })
                    .HasDatabaseName("IX_IDCard");

                entity.HasIndex(e => new { e.PersonId, e.SocialCard })
                    .HasDatabaseName("IX_SocCard");

                entity.HasIndex(e => e.Criteria1)
                    .HasDatabaseName("IX_Crit1");

                entity.HasIndex(e => e.Criteria2)
                    .HasDatabaseName("IX_Crit2");

                entity.HasIndex(e => e.Criteria3)
                    .HasDatabaseName("IX_Crit3");

                entity.HasIndex(e => e.Criteria4)
                    .HasDatabaseName("IX_Crit4");

                entity.HasIndex(e => e.Criteria5)
                    .HasDatabaseName("IX_Crit5");

                entity.HasIndex(e => e.Criteria6)
                    .HasDatabaseName("IX_Crit6");

                entity.HasIndex(e => e.Criteria7)
                    .HasDatabaseName("IX_Crit7");

                entity.HasIndex(e => e.Criteria8)
                    .HasDatabaseName("IX_Crit8");

                entity.HasIndex(e => e.PersonId)
                    .HasDatabaseName("IX_PersonID");

                entity.Property(e => e.PersonId).HasColumnType("int(11)");

                entity.Property(e => e.FirstNameID).HasColumnType("int(11)");

                entity.Property(e => e.LastNameID).HasColumnType("int(11)");

                entity.Property(e => e.PassportNum).HasMaxLength(30);

                entity.Property(e => e.IdCardNum).HasMaxLength(30);

                entity.Property(e => e.SocialCard).HasMaxLength(20);

                entity.Property(e => e.Criteria1).HasColumnType("bigint(20)");

                entity.Property(e => e.SC1).HasMaxLength(20);

                entity.Property(e => e.Criteria2).HasColumnType("bigint(20)");

                entity.Property(e => e.SC2).HasMaxLength(20);

                entity.Property(e => e.Criteria3).HasColumnType("bigint(20)");

                entity.Property(e => e.SC3).HasMaxLength(20);

                entity.Property(e => e.Criteria4).HasColumnType("bigint(20)");

                entity.Property(e => e.SC4).HasMaxLength(20);

                entity.Property(e => e.Criteria5).HasColumnType("bigint(20)");

                entity.Property(e => e.SC5).HasMaxLength(20);

                entity.Property(e => e.Criteria6).HasColumnType("bigint(20)");

                entity.Property(e => e.SC6).HasMaxLength(20);

                entity.Property(e => e.Criteria7).HasColumnType("bigint(20)");

                entity.Property(e => e.SC7).HasMaxLength(20);

                entity.Property(e => e.Criteria8).HasColumnType("bigint(20)");

                entity.Property(e => e.SC8).HasMaxLength(20);

                entity.Property(e => e.AcraGroup).HasMaxLength(20);

                entity.Property(e => e.IsDeleted)
                  .HasColumnName("isDeleted")
                  .HasColumnType("bit(1)")
                  .HasDefaultValueSql("'b\\'0\\''");

                entity.ToTable("Predicats");

            });

            modelBuilder.Entity<DicReportReason>(entity =>
            {
                entity.HasKey(e => e.ReportReasonId);

                entity.Property(e => e.ReportReasonId)
                    .HasColumnName("ReportReasonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReportReason)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("''");

                entity.ToTable("DicReportReasons");
            });

            modelBuilder.Entity<DicReport>(entity =>
            {
                entity.HasKey(e => e.ReportId);

                entity.Property(e => e.ReportId)
                    .HasColumnName("ReportID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Report).HasMaxLength(200);

                entity.Property(e => e.ReportPrice).HasColumnType("int(11)");

                entity.Property(e => e.ReportType)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ScoreReport)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.ToTable("DicReports");
            });

            modelBuilder.Entity<DicReportSubReason>(entity =>
            {
                entity.HasKey(e => e.ReportSubReasonId);

                entity.HasIndex(e => e.ReportReasonId)
                    .HasDatabaseName("ReportReasonID");

                entity.Property(e => e.ReportSubReasonId)
                    .HasColumnName("ReportSubReasonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ReportReasonId)
                    .HasColumnName("ReportReasonID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ReportSubReason)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasDefaultValueSql("''");

                entity.ToTable("DicReportSubReasons");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.HasKey(e => e.SourceId);

                entity.HasIndex(e => e.CreditorTypeId)
                    .HasDatabaseName("CreditorTypeID");

                entity.Property(e => e.SourceId)
                    .HasColumnName("SourceID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AccountNumber).HasMaxLength(100);

                entity.Property(e => e.Accountant).HasMaxLength(100);

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Bank).HasMaxLength(100);

                entity.Property(e => e.ContractDate).HasColumnType("date");

                entity.Property(e => e.ContractId).HasMaxLength(100);

                entity.Property(e => e.CreditorCode).HasMaxLength(50);

                entity.Property(e => e.CreditorTypeId)
                    .HasColumnName("CreditorTypeID")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.EMail)
                    .HasColumnName("eMail")
                    .HasMaxLength(200);

                entity.Property(e => e.Fax).HasMaxLength(100);

                entity.Property(e => e.HomePage).HasMaxLength(100);

                entity.Property(e => e.Hvhh)
                    .HasColumnName("HVHH")
                    .HasMaxLength(100);

                entity.Property(e => e.Manager).HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(100);

                entity.Property(e => e.ShortName).HasMaxLength(6);

                entity.Property(e => e.ShowInReport).HasColumnType("tinyint(1)");

                entity.Property(e => e.SourceName).HasMaxLength(200);

                entity.Property(e => e.SourceType).HasColumnType("int(11)");

                entity.Property(e => e.SpecialDiscount)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Xmlname)
                    .HasColumnName("XMLName")
                    .HasMaxLength(100);

                entity.ToTable("Sources");
            });

            modelBuilder.Entity<Credit>(entity =>
            {
                entity.Property(c => c.CreditStart).HasColumnType("date");
                entity.Property(c => c.ActualCreditStart).HasColumnType("date");
                entity.Property(c => c.FirstInstallment).HasColumnType("date");
                entity.Property(c => c.LastInstallment).HasColumnType("date");
                entity.Property(c => c.LastPaymentDate).HasColumnType("date");
                entity.Property(c => c.OutstandingDate).HasColumnType("date");
                entity.Property(c => c.ClassificationLastDate).HasColumnType("date");
                entity.Property(e => e.IncomingDate)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("'0000-00-00 00:00:00'");

                // entity.HasIndex(e => e.InternalID).HasDatabaseName("PRIMARY").IsUnique(true);
                entity.HasIndex(e => e.ExternalID).HasDatabaseName("ExternalID");
                entity.HasIndex(e => e.CreditID).HasDatabaseName("CreditID");
                entity.HasIndex(e => e.SourceID).HasDatabaseName("SourceID");
                entity.HasIndex(e => new { e.ExternalID, e.SourceID }).HasDatabaseName("ExternalID_SourceID");
                entity.HasIndex(e => e.ReceivedPackageID).HasDatabaseName("ReceivedPackageID");
                entity.HasIndex(e => e.IncomingDate).HasDatabaseName("IncomingDate");
                entity.HasIndex(e => e.CreditScopeID).HasDatabaseName("CreditScopeID");
                entity.HasIndex(e => e.CreditUsePlace).HasDatabaseName("CreditUsePlace");
                entity.HasIndex(e => e.CreditStatus).HasDatabaseName("credits_ibfk_1");
                entity.HasIndex(e => e.CreditType).HasDatabaseName("credits_ibfk_2");
                entity.HasIndex(e => e.CreditClassification).HasDatabaseName("credits_ibfk_3");
                entity.HasIndex(e => e.Currency).HasDatabaseName("credits_ibfk_7");
                
                entity.HasMany(c => c.CreditOwners);
                entity.HasMany(c => c.Guarantors);                

                entity.ToTable("Credits");

            });

            modelBuilder.Entity<CreditOwner>(entity =>
            {
                entity.HasIndex(e => e.PersonID).HasDatabaseName("CreditOwners_PersonID");
                entity.HasIndex(e => e.SourceID).HasDatabaseName("CreditOwners_SourceID");
                entity.HasIndex(e => e.CreditID).HasDatabaseName("CreditOwners_CreditID");
                entity.HasIndex(e => e.OrganizationID).HasDatabaseName("OrganizationID");

               // entity.HasOne(g => g.Credit);
                   //.WithMany(c => c.CreditOwners);

                entity.ToTable("CreditOwners");
            });

            modelBuilder.Entity<Guarantor>(entity =>
            {
                entity.HasIndex(e => e.CreditID).HasDatabaseName("Guarantors_CreditID");
                entity.HasIndex(e => e.PersonID).HasDatabaseName("Guarantors_PersonID");
                entity.HasIndex(e => e.OrganizationID).HasDatabaseName("Guarantors_OrganizationID");
                entity.HasIndex(e => e.SourceID).HasDatabaseName("Guarantors_SourceID");

                entity.HasMany(g => g.GuaranteeCancellations);
                // entity.HasOne(g => g.Credit);
                    //.WithMany(c => c.Guarantors);


                entity.ToTable("Guarantors");
            });

            modelBuilder.Entity<GuaranteeCancellation>(entity =>
            {
                entity.Property(gc => gc.CancellationDate).HasColumnType("date");

               // entity.HasOne(g => g.Guarantor);
                    // .WithMany(c => c.GuaranteeCancellations);

                entity.ToTable("GuaranteeCancellation");
            });

            modelBuilder.Entity<OrgOwner>(entity =>
            {
                entity.HasKey(e => e.OrgOwnerID);

                entity.HasIndex(e => new { e.OrganizationID, e.PersonID, e.OwnerOrgID })
                    .HasDatabaseName("OrganizationID");

                entity.ToTable("OrgOwners");
            });

            modelBuilder.Entity<DicCountries>(entity =>
            {
                entity.HasKey(e => e.CountryId);

                entity.HasIndex(e => e.Country)
                    .HasDatabaseName("Country")
                    .IsUnique();

                entity.HasIndex(e => e.CountryShortName)
                    .HasDatabaseName("CountryShortName")
                    .IsUnique();

                entity.Property(e => e.CountryId).HasColumnType("int(11)");

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.CountryShortName).HasColumnType("char(2)");
            });

            modelBuilder.Entity<DicResident>(entity =>
            {
                entity.HasKey(e => e.ResidentId);

                entity.Property(e => e.ResidentId)
                    .HasColumnName("ResidentID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Resident).HasMaxLength(200);
            });

            modelBuilder.Entity<DicSex>(entity =>
            {
                entity.HasKey(e => e.SexId);

                entity.Property(e => e.SexId).HasColumnName("SexID");

                entity.Property(e => e.Sex).HasMaxLength(100);
            });

            modelBuilder.Entity<DicSynonym>(entity =>
            {
                entity.HasKey(e => e.ID);
            });

            modelBuilder.Entity<OrganizationNames>(entity =>
            {
                entity.HasKey(e => e.OrganizationNameId);

                entity.HasIndex(e => e.OrganizationId)
                    .HasDatabaseName("OrganizationNames_OrganizationID");

                entity.HasIndex(e => e.OrganizationName)
                    .HasDatabaseName("OrganizationNames_OrganizationName");

                entity.Property(e => e.OrganizationNameId)
                    .HasColumnName("OrganizationNameID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IncomingDate).HasColumnType("datetime");

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("OrganizationID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OrganizationName).HasMaxLength(200);

                entity.Property(e => e.SourceId)
                    .HasColumnName("SourceID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ValidFlag).HasColumnType("tinyint(4)");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.OrganizationId);

                entity.HasIndex(e => e.Hvhh)
                    .HasDatabaseName("HVHH")
                    .IsUnique();

                entity.HasIndex(e => e.OrgPropertyTypeId)
                    .HasDatabaseName("OrgPropertyTypeID");

                entity.HasIndex(e => e.OrganizationType)
                    .HasDatabaseName("organizations_ibfk_1");

                entity.HasIndex(e => e.ResidentId)
                    .HasDatabaseName("organizations_ibfk_3");

                entity.HasIndex(e => e.SourceId)
                    .HasDatabaseName("organizations_ibfk_4");

                entity.HasIndex(e => e.StateRegistryNumber)
                    .HasDatabaseName("Organizations_StateRegistryNumber");

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("OrganizationID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Hvhh)
                    .IsRequired()
                    .HasColumnName("HVHH")
                    .HasMaxLength(30)
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IncomingDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.OrgPropertyTypeId)
                    .HasColumnName("OrgPropertyTypeID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.OrganizationType).HasColumnType("int(11)");

                entity.Property(e => e.ResidentId)
                    .HasColumnName("ResidentID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SourceId)
                    .HasColumnName("SourceID")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.FoundationDate).HasColumnName("FoundationDate").HasColumnType("date");

                entity.Property(e => e.StateRegistryNumber)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("''");
            }
            );

            modelBuilder.Entity<ReceivedPackage>(entity =>
            {
                entity.HasKey(e => e.RPId);

                entity.Property(e => e.RPId)
                    .HasColumnName("RPID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PackageSourceId)
                   .HasColumnName("PackageSourceID")
                   .HasColumnType("int(11)");

                entity.Property(e => e.FileStatus).HasDefaultValue(0);

                entity.Property(e => e.UploadDate).HasDefaultValueSql("'0000-00-00 00:00:00'");
                entity.Property(e => e.StartDate).HasDefaultValueSql("'0000-00-00 00:00:00'");
                entity.Property(e => e.EndDate).HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.ToTable("ReceivedPackages");              
            });

            modelBuilder.Entity<ReceivedPacket>(entity =>
            {
                entity.HasKey(e => e.ReceivedPackageID);                

                entity.ToTable("ReceivedPackets");
            });

            modelBuilder.Entity<PackageFile>(entity =>
            {
                entity.HasKey(e => e.PackageFileID);

                entity.ToTable("PackageFiles");
            });

            modelBuilder.Entity<ScorePerson>(entity =>
            {
                entity.HasKey(e => e.PersonID);

                entity.HasIndex(e => e.ScorePersonID).HasDatabaseName("ScorePersonID");
                entity.HasIndex(e => e.IncomingDate).HasDatabaseName("IncomingDate");
                entity.HasIndex(e => e.Status).HasDatabaseName("Status");

                entity.ToTable("ScorePersons");
            });



            modelBuilder.Entity<AcraPersonsBySource>(entity =>
            {
                entity.HasKey(e => e.AutoID);
                entity.Property(e => e.Status).HasDefaultValue(1);
                entity.HasIndex(e => e.ACRAID).HasDatabaseName("IX_ACRAID");
                entity.HasIndex(e => new { e.SourceID , e.ACRAID, e.Status}).HasDatabaseName("IX_BySources");                

                entity.ToTable("AcraPersonsBySource");
            });


            modelBuilder.Entity<MonitoringPlusByAcraID>(entity =>
            {
                entity.HasKey(e => e.AutoID);
                
                entity.HasIndex(e => e.ACRAID).HasDatabaseName("IX_ByACRAID");
                entity.HasIndex(e => new { e.InfoDate, e.ACRAID }).HasDatabaseName("IX_ByDate");

                entity.ToTable("MonitoringPlusByAcraID");
            });


            modelBuilder.Entity<BankIDs>(entity =>
            {
                entity.Property(e=>e.BankID).HasColumnType("varchar(20)");
                entity.Property(e => e.FirstName).HasColumnType("varchar(200)");
                entity.Property(e => e.LastName).HasColumnType("varchar(200)");
                entity.Property(e => e.PassportNum).HasColumnType("varchar(30)");
                entity.Property(e => e.SocialCard).HasColumnType("varchar(20)");
                entity.Property(e => e.HasNSocialCard).HasColumnType("varchar(20)");

                entity.HasKey(e => new { e.BankID, e.FirstName, e.LastName, e.PassportNum, e.SocialCard });

               // entity.HasIndex(e => new { e.BankID,e.FirstName, e.LastName, e.PassportNum,e.SocialCard }).HasDatabaseName("IX_Key").IsUnique();
                entity.HasIndex(e => e.BankID).HasDatabaseName("BankID");
                entity.HasIndex(e => e.FirstName).HasDatabaseName("FirstName");
                entity.HasIndex(e => e.LastName).HasDatabaseName("LastName");
                entity.HasIndex(e => e.PassportNum).HasDatabaseName("PassportNum");
                entity.HasIndex(e => e.SocialCard).HasDatabaseName("SocialCard");
                entity.HasIndex(e => e.HasNSocialCard).HasDatabaseName("HasNSocialCard");
                entity.HasIndex(e => e.BirthDate).HasDatabaseName("BirthDate");
                entity.HasIndex(e => new { e.FirstName, e.LastName, e.SocialCard }).HasDatabaseName("PersonID");
                
                entity.ToTable("BankIDs");
            });


            modelBuilder.Entity<BankIDLegal>(entity =>
            {
                entity.Property(e => e.BankID).HasColumnType("varchar(20)");
                entity.Property(e => e.Name).HasColumnType("varchar(200)");
                entity.Property(e => e.ANTP).HasColumnType("varchar(20)");

                entity.HasKey(e => e.BankID);

                entity.HasIndex(e => e.BankID).HasDatabaseName("BankID").IsUnique();
                entity.HasIndex(e => e.Name).HasDatabaseName("Name");
                entity.HasIndex(e => e.ANTP).HasDatabaseName("ANTP");
                entity.HasIndex(e => new { e.Name, e.ANTP }).HasDatabaseName("Info");


                entity.ToTable("BankIDsLegal");
            });

            modelBuilder.Entity<MonitoringPlus_Source>(entity =>
            {
                entity.HasKey(e => e.SourceID);

                entity.HasIndex(e => e.ResultSourceID).HasDatabaseName("ResultSourceID");
              
                entity.ToTable("MonitoringPlus_Sources");
            });
            modelBuilder.Entity<ACRAID_MAPPER>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedOnAdd().HasColumnType("MEDIUMINT UNSIGNED");
                entity.HasKey(e => e.ID);
                
                entity.Property(e => e.ACRAID).HasColumnType("bigint(20)");
                entity.Property(e => e.PersonID).HasColumnType("int(11)");
                entity.Property(e => e.MatchingID).HasColumnType("tinyint(3)");
                entity.Property(e => e.GenerationDate).HasColumnType("date");
                entity.HasIndex(e => e.PersonID).HasDatabaseName("PersonID");
                entity.HasIndex(e => e.ACRAID).HasDatabaseName("ACRAID");
                entity.ToTable("ACRAID_MAPPER");
            });
            modelBuilder.Entity<AcraID_Errors>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnType("MEDIUMINT UNSIGNED");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PersonId).HasColumnType("int(11)");
                entity.Property(e => e.Isavv).HasColumnType("int(1)");
                entity.Property(e => e.Status).HasColumnType("int(3)");
                entity.Property(e => e.Field).HasColumnType("varchar(50)");
                entity.Property(e => e.Value1).HasColumnType("varchar(50)");
                entity.Property(e => e.Value2).HasColumnType("varchar(50)");
                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<Pek_Definition>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.HasIndex(e => e.parameter).HasDatabaseName("parameter");

                entity.HasIndex(e => e.acceptablevalue).HasDatabaseName("acceptablevalue");
            });

            modelBuilder.Entity<Pek_ActivityLog>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.HasIndex(e => e.message).HasDatabaseName("message");

                entity.HasIndex(e => e.userActivityId).HasDatabaseName("userActivityId");

                entity.HasIndex(e => e.date).HasDatabaseName("date");
            });
            modelBuilder.Entity<DeadPersons>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.HasIndex(e => e.first_name).HasDatabaseName("first_name");

                entity.HasIndex(e => e.last_name).HasDatabaseName("last_name");

                entity.HasIndex(e => e.patronymic_name).HasDatabaseName("patronymic_name");

                entity.HasIndex(e => e.social_card).HasDatabaseName("social_card");

                entity.HasIndex(e => e.document_num).HasDatabaseName("document_num");

                entity.HasIndex(e => e.birth_date).HasDatabaseName("birth_date");

                entity.HasIndex(e => e.request_code).HasDatabaseName("request_code");

                entity.HasIndex(e => e.reg_date).HasDatabaseName("reg_date");

                entity.HasIndex(e => e.create_date).HasDatabaseName("create_date");

                entity.HasIndex(e => e.modify_date).HasDatabaseName("modify_date");

                entity.HasIndex(e => e.modifier).HasDatabaseName("modifier");

                entity.HasIndex(e => e.death_date).HasDatabaseName("death_date");

                entity.HasIndex(e => e.citizenship).HasDatabaseName("citizenship");

                entity.HasIndex(e => e.nationality).HasDatabaseName("nationality");

                entity.HasIndex(e => e.death_certificate_number).HasDatabaseName("death_certificate_number");

                entity.HasIndex(e => e.death_certificate_issuance_date).HasDatabaseName("death_certificate_issuance_date");

                entity.HasIndex(e => e.document_type).HasDatabaseName("document_type");

                entity.HasIndex(e => e.genus).HasDatabaseName("genus");

                entity.HasIndex(e => e.country).HasDatabaseName("country");

                entity.HasIndex(e => e.modifier).HasDatabaseName("modifier");
            });


            ////Remove cascading deletes
            //foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            //}

            //base.OnModelCreating(modelBuilder);

        }
    }
}
