using System;
using System.Collections.Generic;
using Loujico.BL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Loujico.Models;

public partial class CompanySystemContext : IdentityDbContext<ApplicationUser>
{
    public CompanySystemContext()
    {
    }

    public CompanySystemContext(DbContextOptions<CompanySystemContext> options)
        : base(options)
    {
    }

    public DbSet<CompanyActivity> Co_CompanyActivities { get; set; }
    public virtual DbSet<TbCountry> TbCountries { get; set; }
    public virtual DbSet<TbContact> TbContact { get; set; }
    public virtual DbSet<TbState> TbStates { get; set; }
    public virtual DbSet<TbCity> TbCities { get; set; }
    public virtual DbSet<Co_Industry> Co_Industries { get; set; }
    public virtual DbSet<Co_Activity> Co_Activities { get; set; }
    public virtual DbSet<Co_Address> Co_Address { get; set; }
    public virtual DbSet<Co_Company_Name> Co_Companies { get; set; }
    public virtual DbSet<Co_CompanyEmployee> Co_CompanyEmployees { get; set; }
    public virtual DbSet<Co_Contact> Co_Contacts { get; set; }
    public virtual DbSet<Co_Legal> Co_Legals { get; set; }
    public DbSet<TbProjectTask> TbProjectTasks { get; set; }
    public DbSet<TbProjectTaskEmployee> TbProjectTaskEmployees { get; set; }

    public virtual DbSet<TbCompanyProduct> TbCompanyProducts { get; set; }

    public virtual DbSet<TbEmployee> TbEmployees { get; set; }

    public virtual DbSet<TbFile> TbFiles { get; set; }

    public virtual DbSet<TbHistory> TbHistories { get; set; }

    //public virtual DbSet<TbInvoice> TbInvoices { get; set; }

    public virtual DbSet<TbLog> TbLogs { get; set; }

    public virtual DbSet<TbProduct> TbProducts { get; set; }

    public virtual DbSet<TbProductsEmployee> TbProductsEmployees { get; set; }

    public virtual DbSet<TbProject> TbProjects { get; set; }

    public virtual DbSet<TbProjectsEmployee> TbProjectsEmployees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
      
    
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TbEmployee>()
    .HasIndex(e => e.UserId)
    .IsUnique()
    .HasFilter("[UserId] IS NOT NULL");

        modelBuilder.Entity<Co_Company_Name>()
      .HasOne(c => c.Legal)
      .WithMany() // بدون "Company"
      .HasForeignKey(c => c.LegalId)
      .HasConstraintName("FK_Companies_Legals");
        modelBuilder.Entity<Co_Company_Name>()
          .ToTable(tb => tb.HasTrigger("TRG_CoCompanies_History"));

    
        modelBuilder.Entity<TbCompanyProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbCustom__3213E83FAAA2AEA6");

            entity.ToTable("TbCompanyProducts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyId");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.StatusCp)
                .HasMaxLength(50)
                .HasColumnName("status_CP");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Company).WithMany(p => p.TbCompanysProducts)
                .HasForeignKey(d => d.CompanyId)
           .HasConstraintName("FK_TbCompanyProducts_CoCompanies_CompanyId");

            entity.HasOne(d => d.Product).WithMany(p => p.TbCompanyProducts)
                .HasForeignKey(d => d.ProductId)
                 .OnDelete(DeleteBehavior.Restrict)
           .HasConstraintName("FK_TbCompanyProducts_TbProducts_ProductId");
        });

        modelBuilder.Entity<TbEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbEmploy__3213E83F3315B6BA");

            entity.ToTable(tb => tb.HasTrigger("TRG_Employees_History"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EmployeesAddress).HasMaxLength(255);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsPresent)
                .HasDefaultValue(true)
                .HasColumnName("is_present");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.LastVisit).HasColumnName("last_visit");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.ProfileImage).HasColumnName("profile_image");
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salary");
            entity.Property(e => e.ServiceDuration).HasColumnName("service_duration");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<TbFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbFiles__3213E83F4A7F65AD");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EntityId).HasColumnName("entityId");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file__Name");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .HasColumnName("file_type");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.UploadedBy).HasColumnName("uploaded_by");
        });

        modelBuilder.Entity<TbHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbHistor__3213E83FC1BE5429");

            entity.ToTable("TbHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionTime)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("action_time");
            entity.Property(e => e.ActionType)
                .HasMaxLength(50)
                .HasColumnName("action_type");
            entity.Property(e => e.ColumnName)
                .HasMaxLength(100)
                .HasColumnName("column_name");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .HasColumnName("table_name");
        });

      

        modelBuilder.Entity<TbLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbLogs__3213E83F79A60170");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(200)
                .HasColumnName("action_");
            entity.Property(e => e.ActionType)
                .HasMaxLength(50)
                .HasColumnName("action_type");
            entity.Property(e => e.TimeStamp)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("time_stamp");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("User_Id");
        });

        modelBuilder.Entity<TbProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbProduc__3213E83FFBEAA31D");

            entity.ToTable(tb => tb.HasTrigger("TRG_Products_History"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BillingCycle)
                .HasMaxLength(100)
                .HasColumnName("billing_cycle");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Price)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        });

        modelBuilder.Entity<TbProductsEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbProduc__3213E83F15E855CF");

            entity.ToTable("TbProducts_Employees");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("joined_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.RoleOnProduct)
                .HasMaxLength(100)
                .HasColumnName("role_on_product");

            entity.HasOne(d => d.Employee).WithMany(p => p.TbProductsEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_products_employees_employees");

            entity.HasOne(d => d.Product).WithMany(p => p.TbProductsEmployees)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_products_employees_products");
        });

        modelBuilder.Entity<TbProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TbProjec__3213E83F4F2AA381");

            entity.ToTable(tb => tb.HasTrigger("TRG_Projects_History"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyId");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.LastVisit).HasColumnName("last_visit");
            entity.Property(e => e.Price)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProjectStatus).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Title)
                .HasMaxLength(150)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

        /*    entity.HasOne(d => d.Customer).WithMany(p => p.TbProjects)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_projects_customers");*/
        });

        modelBuilder.Entity<TbProjectsEmployee>(entity =>
        {
           entity .HasQueryFilter(pe => !pe.IsDeleted);

            entity.HasKey(e => e.Id).HasName("PK__TbProjec__3213E83F7A181059");

            entity.ToTable("TbProjects_Employees");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(sysutcdatetime())")
                .HasColumnName("joined_at");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RoleOnProject)
                .HasMaxLength(100)
                .HasColumnName("role_on_project");

            entity.HasOne(d => d.Employee).WithMany(p => p.TbProjectsEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_projects_employees_employees");

            entity.HasOne(d => d.Project).WithMany(p => p.TbProjectsEmployees)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_projects_employees_projects");
        });
        modelBuilder.Entity<TbProjectTask>()
      .HasQueryFilter(t => !t.IsDeleted)
      .ToTable(tb => tb.HasTrigger("TRG_TbProjectTasks_History"));

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
