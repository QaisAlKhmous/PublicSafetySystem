using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories
{
    public class AppDbContext : DbContext
    {

        public AppDbContext() : base("Data Source=localhost\\SQLEXPRESS;Initial Catalog=PublicSafety;User ID=Qais;Password=Qais_2004;TrustServerCertificate=True") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Section> Sections {  get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Disposal> Disposals { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Issuance> Issuances { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<JobTitleCategory> JobTitleCategories { get; set; }
        public DbSet<Matrix> Matrices { get; set; }

        public DbSet<MatrixItem> MatrixItems { get; set; }
        public DbSet<ChangeRequest> ChangeRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
            .Property(u => u.UserId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Category>()
            .Property(u => u.CategoryId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Disposal>()
            .Property(u => u.DisposalId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            modelBuilder.Entity<Employee>()
            .Property(u => u.EmployeeId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Issuance>()
            .Property(u => u.IssuanceId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            modelBuilder.Entity<JobTitle>()
            .Property(u => u.JobTitleId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<JobTitleCategory>()
            .Property(u => u.JobTitleCategoryId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<MatrixItem>()
            .Property(u => u.MatrixId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<ChangeRequest>()
            .Property(u => u.RequestId)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);


            modelBuilder.Entity<ChangeRequest>()
                .HasKey(cr => cr.RequestId);

            modelBuilder.Entity<ChangeRequest>()
                .Property(cr => cr.ApprovedDate)
                .IsOptional();

            modelBuilder.Entity<ChangeRequest>()
            .HasRequired(cr => cr.ChangedBy)
            .WithMany(u => u.ChangeRequests) 
            .HasForeignKey(cr => cr.ChangedById)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChangeRequest>()
           .HasOptional(cr => cr.ApprovedBy)
           .WithMany(u => u.ChangeRequestsApproved)
           .HasForeignKey(cr => cr.ApprovedById)
           .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                 .HasRequired(e => e.JobTitle)
                 .WithMany(j => j.Employees)
                 .HasForeignKey(e => e.JobTitleId)
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Section)
                .WithMany(j => j.Employees)
                .HasForeignKey(e => e.SectionId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Department)
                .WithMany(j => j.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.RetirementDate)
                .IsOptional();




            modelBuilder.Entity<JobTitleCategory>()
                .HasRequired(j => j.JobTitle)
                .WithMany(j => j.jobTitleCategories)
                .HasForeignKey(j => j.JobTitleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<JobTitleCategory>()
               .HasRequired(j => j.Category)
               .WithMany(j => j.jobTitleCategories)
               .HasForeignKey(j => j.CategoryId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Disposal>()
              .HasRequired(d => d.CreatedBy)
              .WithMany(u => u.Disposals)
              .HasForeignKey(d => d.CreatedById)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Disposal>()
              .HasOptional(d => d.ApprovedBy)
              .WithMany(u => u.ApprovedDisposals)
              .HasForeignKey(d => d.ApprovedById)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Issuance>()
                .HasRequired(i => i.Employee)
                .WithMany(e => e.Issuances)
                .HasForeignKey(i => i.EmployeeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Issuance>()
               .HasRequired(ish => ish.Item)
               .WithMany(i => i.Issuances)
               .HasForeignKey(i => i.ItemId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Issuance>()
             .HasRequired(ish => ish.CreatedBy)
             .WithMany(i => i.Issues)
             .HasForeignKey(i => i.CreatedById)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<MatrixItem>()
                .HasRequired(m => m.Matrix)
                .WithMany(c => c.MatrixItems)
                .HasForeignKey(m => m.MatrixId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MatrixItem>()
               .HasRequired(m => m.Item)
               .WithMany(i => i.MatrixItems)
               .HasForeignKey(m => m.ItemId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<MatrixItem>()
              .HasRequired(m => m.CreatedBy)
              .WithMany(u => u.MatrixItems)
              .HasForeignKey(m => m.CreatedById)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Matrix>()
              .HasRequired(m => m.Category)
              .WithMany(u => u.Matrices)
              .HasForeignKey(m => m.CategoryId)
              .WillCascadeOnDelete(false);


            modelBuilder.Entity<Item>()
            .HasRequired(i => i.AddedBy)
            .WithMany(u => u.Items)
            .HasForeignKey(m => m.AddedById)
            .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

    }
}
