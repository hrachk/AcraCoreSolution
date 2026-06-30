using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using AcraData.Models.CB;

namespace AcraData.Data
{
    public partial class CBDbContext : DbContext
    {
        public virtual DbSet<A001> A001 { get; set; }
        public virtual DbSet<A002> A002 { get; set; }
        public virtual DbSet<AcraAnswer> AcraAnswers { get; set; }
        public virtual DbSet<AcraAnswerDouble> AcraAnswersDouble { get; set; }
        public virtual DbSet<AcraPerson> Acrapersons { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<CbRefAcra> CbrefAcra { get; set; }
        public virtual DbSet<CbRefActivity> CbrefActivities { get; set; }
        public virtual DbSet<CbRefCollateral> CbrefCollateral { get; set; }
        public virtual DbSet<CbRefCountry> CbrefCountries { get; set; }
        public virtual DbSet<CbRefCurrency> CbrefCurrency { get; set; }
        public virtual DbSet<CbRefEducation> CbrefEducation { get; set; }
        public virtual DbSet<CbrefEmploymentStatus> CbrefEmploymentStatus { get; set; }
        public virtual DbSet<CbrefFcDelegate> CbrefFcDelegate { get; set; }
        public virtual DbSet<CbrefIdentityType> CbrefIdentityType { get; set; }
        public virtual DbSet<CbrefInterestRateType> CbrefInterestRateType { get; set; }
        public virtual DbSet<CbrefInterProgram> CbrefInterProgram { get; set; }
        public virtual DbSet<CbrefIoDelegate> CbrefIoDelegate { get; set; }
        public virtual DbSet<CbrefLegalType> CbrefLegalType { get; set; }
        public virtual DbSet<CbrefLoanContractType> CbrefLoanContractType { get; set; }
        public virtual DbSet<CbrefLoanRisk> CbrefLoanRisk { get; set; }
        public virtual DbSet<CbrefLoanType> CbrefLoanType { get; set; }
        public virtual DbSet<CbrefLoanUsePurpose> CbrefLoanUsePurpose { get; set; }
        public virtual DbSet<CbrefMartialStatus> CbrefMartialStatus { get; set; }
        public virtual DbSet<CbrefOrgStatus> CbrefOrgStatus { get; set; }
        public virtual DbSet<CbrefOwnershipType> CbrefOwnershipType { get; set; }
        public virtual DbSet<CbrefPawnSubject> CbrefPawnSubject { get; set; }
        public virtual DbSet<CbrefPersonType> CbrefPersonType { get; set; }
        public virtual DbSet<CbrefRaDelegate> CbrefRaDelegate { get; set; }
        public virtual DbSet<CbrefRegion> CbrefRegion { get; set; }
        public virtual DbSet<CbrefRepaymentSource> CbrefRepaymentSource { get; set; }
        public virtual DbSet<CbrefRevisionReason> CbrefRevisionReason { get; set; }
        public virtual DbSet<ConverterMessage> ConverterMessages { get; set; }
        public virtual DbSet<DeletedLoan> DeletedLoans { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<FullAntp> FullAntps { get; set; }
        public virtual DbSet<I000> I000 { get; set; }
        public virtual DbSet<I001> I001 { get; set; }
        public virtual DbSet<I002> I002 { get; set; }
        public virtual DbSet<I003> I003 { get; set; }
        public virtual DbSet<I010> I010 { get; set; }
        public virtual DbSet<I011> I011 { get; set; }
        public virtual DbSet<Identity> Identities { get; set; }
        public virtual DbSet<L001> L001 { get; set; }
        public virtual DbSet<L002> L002 { get; set; }
        public virtual DbSet<L003> L003 { get; set; }
        public virtual DbSet<L004> L004 { get; set; }
        public virtual DbSet<LoanAffiliate> LoanAffiliates { get; set; }
        public virtual DbSet<LoanCoDebtor> LoanCoDebtors { get; set; }
        public virtual DbSet<LoanCollateral> LoanCollaterals { get; set; }
        public virtual DbSet<LoanDetail> LoanDetails { get; set; }
        public virtual DbSet<LoanGuarantor> LoanGuarantors { get; set; }
        public virtual DbSet<LoanModificationDate> LoanModificationDates { get; set; }
        public virtual DbSet<LoanOwner> LoanOwners { get; set; }
        public virtual DbSet<LoanPawn> LoanPawns { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<OverdueDaysOfLoan> OverdueDaysOfLoans { get; set; }
        public virtual DbSet<P001> P001 { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonsBankId> PersonsBankId { get; set; }
        public virtual DbSet<PersonsBankIdReq> PersonsBankIdReqs { get; set; }
        public virtual DbSet<PersonsFC> PersonsFc { get; set; }
        public virtual DbSet<PersonsFP> PersonsFp { get; set; }
        public virtual DbSet<PersonsLE> PersonsLe { get; set; }
        public virtual DbSet<PersonsRA> PersonsRa { get; set; }
        public virtual DbSet<ReferenceList> ReferenceList { get; set; }
        public virtual DbSet<RefType> RefTypes { get; set; }
        public virtual DbSet<Register> Registers { get; set; }
        public virtual DbSet<RegPhoto> RegPhotos { get; set; }
        public virtual DbSet<SourceReference> SourceReferences { get; set; }

        public CBDbContext(DbContextOptions<CBDbContext> contextOptions) : base(contextOptions)
        {
        }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<A001>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.LastReqId)
                    .HasColumnName("LastReqID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ReqCount).HasColumnType("int(20)");

                entity.Property(e => e.ReqId)
                    .HasColumnName("ReqID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.RespCount).HasColumnType("int(20)");

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<A002>(entity =>
            {
                entity.HasIndex(e => e.RegisterId)
                    .HasName("fk_Register_ID");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ErrorCode).HasMaxLength(255);

                entity.Property(e => e.ErrorMessage).HasMaxLength(255);

                entity.Property(e => e.IdentityNumber).HasMaxLength(250);

                entity.Property(e => e.PersonId)
                    .HasColumnName("PersonID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.RegisterId)
                    .HasColumnName("RegisterID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Register)
                    .WithMany(p => p.A002s)
                    .HasForeignKey(d => d.RegisterId)
                    .HasConstraintName("fk_Register_ID");
            });

            modelBuilder.Entity<AcraAnswer>(entity =>
            {
                entity.HasIndex(e => e.A001id)
                    .HasName("fk_A001id");

                entity.HasIndex(e => e.ReqId)
                    .HasName("IX_ReqID")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.A001id)
                    .HasColumnName("A001Id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AppName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DestinationOrgCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DocType)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ReqId).HasColumnType("bigint(20)");

                entity.Property(e => e.ResponseDate).HasColumnType("datetime(4)");

                entity.Property(e => e.SenderOrgCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");

                entity.Property(e => e.SystemErrorCode).HasColumnType("int(11)");

                entity.Property(e => e.SystemErrorDesc).HasMaxLength(255);

                entity.Property(e => e.SystemStatus).HasColumnType("int(11)");

                entity.Property(e => e.XmlReq)
                    .IsRequired()
                    .HasColumnType("mediumtext");

                entity.Property(e => e.XmlResp)
                    .IsRequired()
                    .HasColumnType("text");

                entity.ToTable("AcraAnswers");
            });

            modelBuilder.Entity<AcraAnswerDouble>(entity =>
            {
                entity.HasIndex(e => e.A001id)
                    .HasName("fk_A001id");

                entity.HasIndex(e => e.ReqId)
                    .HasName("IX_ReqID")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.A001id)
                    .HasColumnName("A001Id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.AppName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DestinationOrgCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DocType)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ReqId).HasColumnType("bigint(20)");

                entity.Property(e => e.ResponseDate).HasColumnType("datetime");

                entity.Property(e => e.SenderOrgCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.SystemDate).HasColumnType("datetime");

                entity.Property(e => e.SystemErrorCode).HasColumnType("int(11)");

                entity.Property(e => e.SystemErrorDesc).HasMaxLength(255);

                entity.Property(e => e.SystemStatus).HasColumnType("int(11)");

                entity.Property(e => e.XmlReq)
                    .IsRequired()
                    .HasColumnType("mediumtext");

                entity.Property(e => e.XmlResp)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.A001)
                    .WithMany(p => p.AcraAnswersDouble)
                    .HasForeignKey(d => d.A001id)
                    .HasConstraintName("acraanswersdouble_ibfk_1");

                entity.ToTable("AcraAnswersDouble");
            });

            modelBuilder.Entity<AcraPerson>(entity =>
            {
                entity.HasKey(e => e.PersonId);

                entity.ToTable("ACRAPersons");

                entity.HasIndex(e => new { e.PersonId, e.Status })
                    .HasName("IX_Status")
                    .IsUnique();

                entity.Property(e => e.PersonId)
                    .HasColumnName("PersonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BankId)
                    .HasColumnName("BankID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Status)
                    .HasColumnType("int(4)")
                    .HasDefaultValueSql("'0'");

                entity.ToTable("ACRAPersons");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasIndex(e => e.PersonId)
                    .HasName("PersonId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Appartment).HasMaxLength(255);

                entity.Property(e => e.BuildNumber).HasMaxLength(255);

                entity.Property(e => e.Country).HasMaxLength(255);

                entity.Property(e => e.PersonId).HasColumnType("bigint(20)");

                entity.Property(e => e.Region).HasMaxLength(255);

                entity.Property(e => e.Street).HasMaxLength(255);

                entity.Property(e => e.TypeId).HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Addresses)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("addresses_ibfk_1");

                entity.ToTable("Addresses");
            });

            modelBuilder.Entity<CbRefAcra>(entity =>
            {
                entity.HasKey(e => e.RefName);

                entity.ToTable("CBREF_ACRA");

                entity.Property(e => e.RefName)
                    .HasColumnName("REF_NAME")
                    .HasMaxLength(255);

                entity.Property(e => e.ColumnName).HasColumnType("text");

                entity.Property(e => e.Description).HasColumnType("text");

                entity.Property(e => e.RefDescription)
                    .HasColumnName("REF_DESCRIPTION")
                    .HasMaxLength(255);

                entity.Property(e => e.TableName).HasColumnType("text");

                entity.ToTable("CBREF_ACRA");
            });

            modelBuilder.Entity<CbRefActivity>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_ACTIVITIES");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_ACTIVITIES");
            });

            modelBuilder.Entity<CbRefCollateral>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_COLLATERAL");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_COLLATERAL");
            });

            modelBuilder.Entity<CbRefCountry>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_COUNTRIES");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Ename)
                    .HasColumnName("ENAME")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_COUNTRIES");
            });

            modelBuilder.Entity<CbRefCurrency>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_CURRENCY");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_CURRENCY");
            });

            modelBuilder.Entity<CbRefEducation>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_EDUCATION");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_EDUCATION");
            });

            modelBuilder.Entity<CbrefEmploymentStatus>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_EMPLOYMENT_STATUS");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_EMPLOYMENT_STATUS");
            });

            modelBuilder.Entity<CbrefFcDelegate>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_FC_DELEGATE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_FC_DELEGATE");
            });

            modelBuilder.Entity<CbrefIdentityType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_IDENTITY_TYPE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_IDENTITY_TYPE");
            });

            modelBuilder.Entity<CbrefInterestRateType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_INTEREST_RATE_TYPE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_INTEREST_RATE_TYPE");
            });

            modelBuilder.Entity<CbrefInterProgram>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_INTER_PROGRAM");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_INTER_PROGRAM");
            });

            modelBuilder.Entity<CbrefIoDelegate>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_IO_DELEGATE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Ename)
                    .HasColumnName("ENAME")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_IO_DELEGATE");
            });

            modelBuilder.Entity<CbrefLegalType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_LEGAL_TYPE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_LEGAL_TYPE");
            });

            modelBuilder.Entity<CbrefLoanContractType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_LOAN_CONTRACT_TYPE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_LOAN_CONTRACT_TYPE");
            });

            modelBuilder.Entity<CbrefLoanRisk>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_LOAN_RISK");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_LOAN_RISK");
            });

            modelBuilder.Entity<CbrefLoanType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_LOAN_TYPE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_LOAN_TYPE");
            });

            modelBuilder.Entity<CbrefLoanUsePurpose>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_LOAN_USE_PURPOSE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_LOAN_USE_PURPOSE");
            });

            modelBuilder.Entity<CbrefMartialStatus>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_MARTIAL_STATUS");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_MARTIAL_STATUS");
            });

            modelBuilder.Entity<CbrefOrgStatus>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_ORG_STATUS");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_ORG_STATUS");
            });

            modelBuilder.Entity<CbrefOwnershipType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_OWNERSHIP_TYPE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_OWNERSHIP_TYPE");
            });

            modelBuilder.Entity<CbrefPawnSubject>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_PAWN_SUBJECT");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_PAWN_SUBJECT");
            });

            modelBuilder.Entity<CbrefPersonType>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_PERSON_TYPE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_PERSON_TYPE");
            });

            modelBuilder.Entity<CbrefRaDelegate>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_RA_DELEGATE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.IsRa)
                    .HasColumnName("IS_RA")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_RA_DELEGATE");
            });

            modelBuilder.Entity<CbrefRegion>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_REGION");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasMaxLength(255);

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_REGION");
            });

            modelBuilder.Entity<CbrefRepaymentSource>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_REPAYMENT_SOURCE");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_REPAYMENT_SOURCE");
            });

            modelBuilder.Entity<CbrefRevisionReason>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("CBREF_REVISION_REASON");

                entity.Property(e => e.Code)
                    .HasColumnName("CODE")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AcraId)
                    .HasColumnName("ACRA_ID")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(255);

                entity.ToTable("CBREF_REVISION_REASON");
            });

            modelBuilder.Entity<ConverterMessage>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.InsertDate).HasColumnType("datetime");

                entity.Property(e => e.RawData)
                    .IsRequired()
                    .HasColumnType("mediumtext");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.ToTable("ConverterMessages");
            });

            modelBuilder.Entity<DeletedLoan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CreditCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DeleteReason).HasMaxLength(512);

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.L003)
                    .WithOne(p => p.DeletedLoan)
                    .HasForeignKey<DeletedLoan>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Id");

                entity.ToTable("DeletedLoans");
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.HasKey(e => e.EntityTypeId);

                entity.Property(e => e.EntityTypeId).HasColumnType("int(11)");

                entity.Property(e => e.EntityDesc)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.ToTable("Entities");
            });

            modelBuilder.Entity<FullAntp>(entity =>
            {
                entity.ToTable("FullANTPs");

                entity.HasIndex(e => e.PersonId)
                    .HasName("fullantps_ibfk_1");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ActivityField).HasMaxLength(255);

                entity.Property(e => e.Antp)
                    .IsRequired()
                    .HasColumnName("ANTP")
                    .HasMaxLength(255);

                entity.Property(e => e.PersonId).HasColumnType("bigint(20)");

                entity.Property(e => e.RegNum).HasMaxLength(255);

                entity.Property(e => e.RegDate).HasColumnType("date");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.FullAntps)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fullantps_ibfk_1");

                entity.ToTable("FullANTPs");
            });

            modelBuilder.Entity<I000>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AppName).HasMaxLength(20);

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<I001>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AppName).HasMaxLength(50);

                entity.Property(e => e.Data)
                    .HasColumnName("data")
                    .HasMaxLength(10);

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<I002>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AppName).HasMaxLength(20);

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<I003>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.AppName).HasMaxLength(20);

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasMaxLength(10);

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<I010>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<I011>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.ErrorMessage).HasMaxLength(255);

                entity.Property(e => e.ParseStatus)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.RefName)
                    .HasColumnName("REF_NAME")
                    .HasMaxLength(255);

                entity.Property(e => e.SendDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");
            });

            modelBuilder.Entity<Identity>(entity =>
            {
                entity.HasIndex(e => e.PersonId)
                    .HasName("PersonId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.IssuingAuthority).HasMaxLength(255);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PersonId).HasColumnType("bigint(20)");

                entity.Property(e => e.TypeId).HasColumnType("int(11)");

                entity.Property(e => e.DateOfIssue).HasColumnType("date");

                entity.Property(e => e.DateOfExpiry).HasColumnType("date");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.Identities)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("identities_ibfk_1");

                entity.ToTable("Identities");
            });

            modelBuilder.Entity<L001>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.OrganisationBranchCode).HasMaxLength(255);

                entity.Property(e => e.OrganisationCode).HasMaxLength(255);

                entity.Property(e => e.OrganizationStatus).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationStatusSpecified).HasColumnType("tinyint(4)");

                entity.Property(e => e.SendDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.AcraAnswer)
                    .WithOne(p => p.L001)
                    .HasForeignKey<L001>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("l001_ibfk_1");
            });

            modelBuilder.Entity<L002>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.OrganisationBranchCode).HasMaxLength(255);

                entity.Property(e => e.OrganisationCode).HasMaxLength(255);

                entity.Property(e => e.OrganizationStatus).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationStatusSpecified).HasColumnType("tinyint(4)");

                entity.Property(e => e.SendDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.AcraAnswer)
                    .WithOne(p => p.L002)
                    .HasForeignKey<L002>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("l002_ibfk_1");
            });

            modelBuilder.Entity<L003>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.OrganisationBranchCode).HasMaxLength(255);

                entity.Property(e => e.OrganisationCode).HasMaxLength(255);

                entity.Property(e => e.OrganizationStatus).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationStatusSpecified).HasColumnType("tinyint(4)");

                entity.Property(e => e.SendDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.acraAnswer)
                    .WithOne(p => p.L003)
                    .HasForeignKey<L003>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("l003_ibfk_1");
            });

            modelBuilder.Entity<L004>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.OrganisationBranchCode).HasMaxLength(255);

                entity.Property(e => e.OrganisationCode).HasMaxLength(255);

                entity.Property(e => e.OrganizationStatus).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationStatusSpecified).HasColumnType("tinyint(4)");

                entity.Property(e => e.SendDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.AcraAnswer)
                    .WithOne(p => p.L004)
                    .HasForeignKey<L004>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("l004_ibfk_1");
            });

            modelBuilder.Entity<LoanAffiliate>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanId_fk");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AffiliateId)
                    .IsRequired()
                    .HasColumnName("AffiliateID")
                    .HasMaxLength(50);

                entity.Property(e => e.AffiliateNotes).HasMaxLength(512);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("'b\\'0\\''");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanAffiliates)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("loanaffiliates_ibfk_1");

                entity.ToTable("LoanAffiliates");
            });

            modelBuilder.Entity<LoanCoDebtor>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanId_fk");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CoDebtorId)
                    .IsRequired()
                    .HasColumnName("CoDebtorID")
                    .HasMaxLength(50);

                entity.Property(e => e.CoDebtorNotes).HasMaxLength(512);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("'b\\'0\\''");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.Proportion).HasColumnType("decimal(20,2)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanCoDebtors)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LoanIdCD_fk");

                entity.ToTable("LoanCoDebtors");
            });

            modelBuilder.Entity<LoanCollateral>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanIdLP_fk");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ExternalId).HasColumnType("int(10)");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("'b\\'0\\''");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanCollaterals)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("loancollaterals_ibfk_1");

                entity.ToTable("LoanCollaterals");
            });

            modelBuilder.Entity<LoanDetail>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanId");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ActualInterestRate).HasColumnType("decimal(20,2)");

                entity.Property(e => e.AffectionWithCreditor).HasColumnType("tinyint(1)");

                entity.Property(e => e.AmountOff).HasColumnType("decimal(20,2)");

                entity.Property(e => e.AmountsPaid).HasColumnType("decimal(20,2)");

                entity.Property(e => e.AnnualInterestRate).HasColumnType("decimal(20,2)");

                entity.Property(e => e.CalculatedOtherObligations).HasColumnType("decimal(20,2)");

                entity.Property(e => e.CalculatedPenalties).HasColumnType("decimal(20,2)");

                entity.Property(e => e.ConditionsChangeCount).HasColumnType("int(11)");

                entity.Property(e => e.ContractAmount).HasColumnType("decimal(20,2)");

                entity.Property(e => e.ContractModifiedAmount).HasColumnType("decimal(20,2)");

                entity.Property(e => e.ContractTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Currency).HasMaxLength(255);

                entity.Property(e => e.DebtorNotes)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.InterestRateTypeId).HasColumnType("int(11)");

                entity.Property(e => e.IsInterestSubsidy).HasColumnType("tinyint(1)");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.LoanStatus).HasColumnType("int(11)");

                entity.Property(e => e.LoanTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Notes).HasMaxLength(255);

                entity.Property(e => e.OverdueDays).HasColumnType("int(11)");

                entity.Property(e => e.OverduePercent).HasColumnType("decimal(20,2)");

                entity.Property(e => e.OverduePrincipalAmount).HasColumnType("decimal(20,2)");

                entity.Property(e => e.PercentsPaid).HasColumnType("decimal(20,2)");

                entity.Property(e => e.PrincipalAmount).HasColumnType("decimal(20,2)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.Property(e => e.RepaymentSourceId).HasColumnType("int(11)");

                entity.Property(e => e.RevisedDays).HasColumnType("int(11)");

                entity.Property(e => e.RevisionReasonId).HasColumnType("int(11)");

                entity.Property(e => e.RiskId).HasColumnType("int(11)");

                entity.Property(e => e.SubsidyAmount).HasColumnType("decimal(20,2)");

                entity.Property(e => e.GrantingDate).HasColumnType("date");

                entity.Property(e => e.LastClassificationDate).HasColumnType("date");

                entity.Property(e => e.LastExpirationDate).HasColumnType("date");

                entity.Property(e => e.RepaymentDate).HasColumnType("date");

                entity.Property(e => e.RepaymentActualDate).HasColumnType("date");

                entity.Property(e => e.RevisionDate).HasColumnType("date");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanDetails)
                    .HasForeignKey(d => d.LoanId)
                    .HasConstraintName("loandetails_ibfk_1");

                entity.ToTable("LoanDetails");
            });

            modelBuilder.Entity<LoanGuarantor>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanId_fk");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.GuarantorId)
                    .IsRequired()
                    .HasColumnName("GuarantorID")
                    .HasMaxLength(50);

                entity.Property(e => e.GuarantorNotes).HasMaxLength(512);

                entity.Property(e => e.GuarantyAmount).HasColumnType("decimal(20,2)");

                entity.Property(e => e.GuarantyCurrency).HasMaxLength(10);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("'b\\'0\\''");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanGuarantors)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("loanguarantors_ibfk_1");

                entity.ToTable("LoanGuarantors");
            });

            modelBuilder.Entity<LoanModificationDate>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanId");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.Property(e => e.ModificationDateTime).HasColumnType("date");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanModificationDates)
                    .HasForeignKey(d => d.LoanId)
                    .HasConstraintName("loanmodificationdates_ibfk_1");

                entity.ToTable("LoanModificationDates");
            });

            modelBuilder.Entity<LoanOwner>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanId_fk");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("'b\\'0\\''");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.OwnerId)
                    .IsRequired()
                    .HasColumnName("OwnerID")
                    .HasMaxLength(50);

                entity.Property(e => e.OwnerNotes).HasMaxLength(512);

                entity.Property(e => e.Proportion).HasColumnType("decimal(20,2)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanOwners)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("loanowners_ibfk_1");

                entity.ToTable("LoanOwners");
            });

            modelBuilder.Entity<LoanPawn>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanIdLP_fk");

                entity.HasIndex(e => e.RefId)
                    .HasName("RefId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CurrencyCode).HasMaxLength(255);

                entity.Property(e => e.EstimatedValue).HasColumnType("decimal(20,2)");

                entity.Property(e => e.ExternalId)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("'b\\'0\\''");

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.Notes).HasMaxLength(512);

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.Property(e => e.Subject).HasColumnType("int(11)");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.LoanPawns)
                    .HasForeignKey(d => d.LoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LoanIdLP_fk");

                entity.ToTable("LoanPawns");
            });

            modelBuilder.Entity<Loan>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ContractNumber).HasMaxLength(255);

                entity.Property(e => e.CreditCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.DebtorId).HasColumnType("bigint(20)");

                entity.Property(e => e.DelRefId).HasColumnType("bigint(20)");

                entity.Property(e => e.DeleteReason).HasMaxLength(512);

                entity.Property(e => e.InterOrg).HasMaxLength(255);

                entity.Property(e => e.InterProgram).HasMaxLength(255);

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("isDeleted")
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("'b\\'0\\''");

                entity.Property(e => e.IsPe)
                    .HasColumnName("IsPE")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.OldCreditCode).HasMaxLength(255);

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.Property(e => e.UseCountry).HasMaxLength(255);

                entity.Property(e => e.UseField).HasMaxLength(255);

                entity.Property(e => e.UsePurpose).HasColumnType("int(11)");

                entity.Property(e => e.UseRegion).HasMaxLength(255);

                entity.Property(e => e.ContractDate).HasColumnType("date");

                entity.ToTable("Loans");
            });

            modelBuilder.Entity<OverdueDaysOfLoan>(entity =>
            {
                entity.HasIndex(e => e.LoanId)
                    .HasName("LoanIdODL_fk");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.CreditCode)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LoanId).HasColumnType("bigint(20)");

                entity.Property(e => e.OverdueDaysOfMonth).HasColumnType("int(11)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Loan)
                    .WithMany(p => p.OverdueDaysOfLoans)
                    .HasForeignKey(d => d.LoanId)
                    .HasConstraintName("LoanIdODL_fk");

                entity.ToTable("OverdueDaysOfLoans");
            });

            modelBuilder.Entity<P001>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.OrganisationBranchCode).HasMaxLength(255);

                entity.Property(e => e.OrganisationCode).HasMaxLength(255);

                entity.Property(e => e.OrganizationStatus).HasColumnType("int(11)");

                entity.Property(e => e.OrganizationStatusSpecified).HasColumnType("tinyint(1)");

                entity.Property(e => e.SendDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.AcraAnswer)
                    .WithOne(p => p.P001)
                    .HasForeignKey<P001>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Request");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BankId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Notes).HasMaxLength(255);

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.Property(e => e.SystemDate).HasColumnType("datetime");

                entity.Property(e => e.SystemStatus).HasColumnType("int(11)");

                entity.ToTable("Persons");
            });

            modelBuilder.Entity<PersonsBankId>(entity =>
            {
                entity.ToTable("PersonsBankID");

                entity.HasIndex(e => e.A002Id)
                    .HasName("fk_A002Id");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.A002Id)
                    .HasColumnName("A002Id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.BankId)
                    .HasColumnName("BankID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ErrorCode).HasMaxLength(255);

                entity.Property(e => e.ErrorText).HasColumnType("text");

                entity.Property(e => e.Gender).HasMaxLength(1);

                entity.Property(e => e.IdentityType).HasMaxLength(255);

                entity.Property(e => e.ResidencyCountry).HasMaxLength(255);

                entity.Property(e => e.XmlReq)
                    .HasColumnName("xmlReq")
                    .HasColumnType("text");

                entity.Property(e => e.XmlResp)
                    .HasColumnName("xmlResp")
                    .HasColumnType("text");

                entity.HasOne(d => d.A002)
                    .WithMany(p => p.PersonsBankId)
                    .HasForeignKey(d => d.A002Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_A002Id");

                entity.ToTable("PersonsBankID");
            });

            modelBuilder.Entity<PersonsBankIdReq>(entity =>
            {
                entity.HasKey(e => e.AutoNumber);

                entity.ToTable("PersonsBankIDReqs");

                entity.HasIndex(e => e.PersonId)
                    .HasName("fk_PersonID");

                entity.Property(e => e.AutoNumber).HasColumnType("bigint(20)");

                entity.Property(e => e.BankId)
                    .HasColumnName("BankID")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.ErrorCode).HasMaxLength(255);

                entity.Property(e => e.ErrorText).HasColumnType("text");

                entity.Property(e => e.Gender).HasMaxLength(1);

                entity.Property(e => e.IdentityType).HasMaxLength(255);

                entity.Property(e => e.PersonId)
                    .HasColumnName("PersonID")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ResidencyCountry).HasMaxLength(255);

                entity.Property(e => e.XmlReq)
                    .HasColumnName("xmlReq")
                    .HasColumnType("text");

                entity.Property(e => e.XmlResp)
                    .HasColumnName("xmlResp")
                    .HasColumnType("text");

                entity.HasOne(d => d.AcraPerson)
                    .WithMany(p => p.PersonsBankIdreqs)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("fk_PersonID");

                entity.ToTable("PersonsBankIDReqs");
            });

            modelBuilder.Entity<PersonsFC>(entity =>
            {
                entity.ToTable("PersonsFC");

                entity.HasIndex(e => e.PersonId)
                    .HasName("personsfc_ibfk_1");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Fcdelegate)
                    .HasColumnName("FCDelegate")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Fcname)
                    .IsRequired()
                    .HasColumnName("FCName")
                    .HasMaxLength(255);

                entity.Property(e => e.PersonId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonsFC)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("personsfc_ibfk_1");
            });

            modelBuilder.Entity<PersonsFP>(entity =>
            {
                entity.ToTable("PersonsFP");

                entity.HasIndex(e => e.PersonId)
                    .HasName("personsfp_ibfk_1");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Education).HasColumnType("int(11)");

                entity.Property(e => e.EmploymentStatus).HasColumnType("int(11)");

                entity.Property(e => e.FamilyMembers).HasColumnType("int(11)");

                entity.Property(e => e.FamilyName).HasMaxLength(255);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Gender).HasColumnType("int(11)");

                entity.Property(e => e.HasNoSsncertificate)
                    .HasColumnName("HasNoSSNCertificate")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.IncomesAbroad).HasColumnType("decimal(20,2)");

                entity.Property(e => e.IncomesFamily).HasColumnType("decimal(20,2)");

                entity.Property(e => e.IncomesPersonal).HasColumnType("decimal(20,2)");

                entity.Property(e => e.IsPe)
                    .HasColumnName("IsPE")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.MartialStatus).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("bigint(20)");

                entity.Property(e => e.ResidencyCountry).HasMaxLength(255);

                entity.Property(e => e.Ssn)
                    .HasColumnName("SSN")
                    .HasMaxLength(255);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonsFP)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("personsfp_ibfk_1");
            });

            modelBuilder.Entity<PersonsLE>(entity =>
            {
                entity.ToTable("PersonsLE");

                entity.HasIndex(e => e.PersonId)
                    .HasName("personsle_ibfk_1");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ExecutiveDirectorBankId).HasMaxLength(50);

                entity.Property(e => e.Incomes).HasColumnType("decimal(20,2)");

                entity.Property(e => e.LegalTypeId).HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.OwnershipTypeId).HasColumnType("int(11)");

                entity.Property(e => e.PersonId).HasColumnType("bigint(20)");

                entity.Property(e => e.ResidencyCountry).HasMaxLength(255);

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonsLE)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("personsle_ibfk_1");
            });

            modelBuilder.Entity<PersonsRA>(entity =>
            {
                entity.ToTable("PersonsRA");

                entity.HasIndex(e => e.PersonId)
                    .HasName("personsra_ibfk_1");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.IsRa)
                    .HasColumnName("IsRA")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.PersonId).HasColumnType("bigint(20)");

                entity.Property(e => e.Radelegate)
                    .HasColumnName("RADelegate")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonsRA)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("personsra_ibfk_1");
            });

            modelBuilder.Entity<ReferenceList>(entity =>
            {
                entity.HasKey(e => e.RefName);

                entity.Property(e => e.RefName)
                    .HasColumnName("REF_NAME")
                    .HasMaxLength(255);

                entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");

                entity.Property(e => e.RefDescription)
                    .HasColumnName("REF_DESCRIPTION")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<RefType>(entity =>
            {
                entity.HasKey(e => e.RefTypeId);

                entity.Property(e => e.RefTypeId).HasColumnType("int(11)");

                entity.Property(e => e.RefTypeDesc)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.ToTable("RefTypes");
            });

            modelBuilder.Entity<Register>(entity =>
            {
                entity.HasIndex(e => e.A002Id)
                    .HasName("ReqID");

                entity.HasIndex(e => new { e.SocCard, e.Number })
                    .HasName("IX_Identity")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.A002Id)
                    .HasColumnName("A002Id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.BirthAddress).HasMaxLength(255);

                entity.Property(e => e.BirthCommunity).HasMaxLength(255);

                entity.Property(e => e.BirthCoountryName).HasMaxLength(255);

                entity.Property(e => e.BirthCountryCode).HasMaxLength(255);

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.BirthRegion).HasMaxLength(255);

                entity.Property(e => e.BirthResidence).HasMaxLength(255);

                entity.Property(e => e.CertificateNumber).HasMaxLength(255);

                entity.Property(e => e.CitizensCountryCode).HasMaxLength(255);

                entity.Property(e => e.CitizensCountryName).HasMaxLength(255);

                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.DeathDate).HasColumnType("datetime");

                entity.Property(e => e.Department).HasMaxLength(10);

                entity.Property(e => e.FirstName).HasMaxLength(255);

                entity.Property(e => e.FirstNameEng).HasMaxLength(255);

                entity.Property(e => e.IsDead).HasColumnType("smallint(4)");

                entity.Property(e => e.LastName).HasMaxLength(255);

                entity.Property(e => e.LastNameEng).HasMaxLength(255);

                entity.Property(e => e.Nationality).HasMaxLength(10);

                entity.Property(e => e.Number)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.PresidentOrder).HasMaxLength(255);

                entity.Property(e => e.SecondName).HasMaxLength(255);

                entity.Property(e => e.SecondNameEng).HasMaxLength(255);

                entity.Property(e => e.Sex).HasMaxLength(10);

                entity.Property(e => e.SocCard)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.SsnIndicator)
                    .HasColumnName("SSN_Indicator")
                    .HasColumnType("smallint(4)");

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.Type).HasMaxLength(255);

                entity.HasOne(d => d.A002)
                    .WithMany(p => p.Registers)
                    .HasForeignKey(d => d.A002Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_a002");

                entity.ToTable("Registers");
            });

            modelBuilder.Entity<RegPhoto>(entity =>
            {
                entity.HasIndex(e => e.A002Id)
                    .HasName("ReqID");

                entity.HasIndex(e => e.SocCard)
                    .HasName("IX_Identity")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.A002Id)
                    .HasColumnName("A002Id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Photo).HasColumnType("blob");

                entity.Property(e => e.SocCard)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.A002)
                    .WithMany(p => p.RegPhotos)
                    .HasForeignKey(d => d.A002Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("regphotos_ibfk_1");

                entity.ToTable("RegPhotos");
            });

            modelBuilder.Entity<SourceReference>(entity =>
            {
                entity.HasIndex(e => e.EntityTypeId)
                    .HasName("EntityType_fk");

                entity.HasIndex(e => e.RefTypeId)
                    .HasName("RefTypeId");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.EntityId).HasColumnType("bigint(20)");

                entity.Property(e => e.EntityTypeId).HasColumnType("int(11)");

                entity.Property(e => e.RefId).HasColumnType("bigint(20)");

                entity.Property(e => e.RefTypeId).HasColumnType("int(11)");

                entity.Property(e => e.SourceId).HasColumnType("bigint(20)");

                entity.HasOne(d => d.EntityType)
                    .WithMany(p => p.SourceReferences)
                    .HasForeignKey(d => d.EntityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EntityType_fk");

                entity.HasOne(d => d.RefType)
                    .WithMany(p => p.SourceReferences)
                    .HasForeignKey(d => d.RefTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RefType_fk");

                entity.ToTable("SourceReferences");
            });
        }
    }
}
