// <auto-generated />
using System;
using System.Collections.Generic;
using CaC2O4.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CaC2O4.Repositories.Migrations
{
    [DbContext(typeof(DbCtx))]
    [Migration("20220316154155_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "doc_subject", new[] { "not_applicable", "vehicle", "employee", "contract" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "file_state", new[] { "unavailable", "blank", "current", "editing", "archived" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "fuel_type", new[] { "unknow_type", "electric", "hybrid_cng", "hybrid_electric" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_acl", new[] { "forbidden", "staff", "archivist", "admin", "supvr", "super" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CaC2O4.Repositories.Contract", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uuid")
                        .HasColumnName("employee_id");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_removed");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("log");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uuid")
                        .HasColumnName("vehicle_id");

                    b.HasKey("Id")
                        .HasName("pk_contracts");

                    b.HasIndex("EmployeeId")
                        .HasDatabaseName("ix_contracts_employee_id");

                    b.HasIndex("VehicleId", "EmployeeId")
                        .IsUnique()
                        .HasDatabaseName("ix_contracts_vehicle_id_employee_id");

                    b.ToTable("contracts", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("log");

                    b.Property<int>("State")
                        .HasColumnType("integer")
                        .HasColumnName("state");

                    b.Property<int>("Subject")
                        .HasColumnType("integer")
                        .HasColumnName("subject");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uuid")
                        .HasColumnName("subject_id");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<List<string>>("Versions")
                        .IsRequired()
                        .HasColumnType("text[]")
                        .HasColumnName("versions");

                    b.HasKey("Id")
                        .HasName("pk_documents");

                    b.HasIndex("SubjectId", "Title")
                        .IsUnique()
                        .HasDatabaseName("ix_documents_subject_id_title");

                    b.ToTable("documents", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("address");

                    b.Property<string>("CertificateNo")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("cert_no");

                    b.Property<DateOnly>("Dob")
                        .HasColumnType("date")
                        .HasColumnName("dob");

                    b.Property<string>("EducationLevel")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("education_level");

                    b.Property<string>("Ethnicity")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("ethnicity");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("gender");

                    b.Property<string>("Household")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("household");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_removed");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("log");

                    b.Property<long>("LogicId")
                        .HasColumnType("bigint")
                        .HasColumnName("logic_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("NameIndex")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name_index");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone");

                    b.Property<long>("RecordNo")
                        .HasColumnType("bigint")
                        .HasColumnName("record_no");

                    b.Property<DateOnly>("SCardExpireAt")
                        .HasColumnType("date")
                        .HasColumnName("scard_exp");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<string>("Tin")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tin");

                    b.HasKey("Id")
                        .HasName("pk_employees");

                    b.ToTable("employees", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.Login", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("IssueAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("issue_at");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_logins");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_logins_user_id");

                    b.ToTable("logins", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.Paper", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("log");

                    b.Property<long>("SerialNo")
                        .HasColumnType("bigint")
                        .HasColumnName("serial_no");

                    b.Property<int>("State")
                        .HasColumnType("integer")
                        .HasColumnName("state");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uuid")
                        .HasColumnName("vehicle_id");

                    b.HasKey("Id")
                        .HasName("pk_papers");

                    b.HasIndex("VehicleId")
                        .HasDatabaseName("ix_papers_vehicle_id");

                    b.ToTable("papers", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.UploadingRecord", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text")
                        .HasColumnName("key");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("Creator")
                        .HasColumnType("uuid")
                        .HasColumnName("creator");

                    b.Property<bool>("IsSuccess")
                        .HasColumnType("boolean")
                        .HasColumnName("is_success");

                    b.Property<DateTimeOffset?>("SuccessedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("successed_at");

                    b.HasKey("Key")
                        .HasName("pk_uploading_records");

                    b.HasIndex("Creator")
                        .HasDatabaseName("ix_uploading_records_creator");

                    b.ToTable("uploading_records", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    b.Property<UserAcl>("Acl")
                        .HasColumnType("user_acl")
                        .HasColumnName("acl");

                    b.Property<DateTimeOffset>("LastAccess")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_access");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("phone");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_users_name");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateOnly>("BusinessLicenseExpireAt")
                        .HasColumnType("date")
                        .HasColumnName("b_license_exp");

                    b.Property<string>("BusinessName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("business_name");

                    b.Property<string>("EngineNo")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("engine_no");

                    b.Property<FuelType>("FuelType")
                        .HasColumnType("fuel_type")
                        .HasColumnName("fuel_type");

                    b.Property<bool>("IsRemoved")
                        .HasColumnType("boolean")
                        .HasColumnName("is_removed");

                    b.Property<string>("LicenseNo")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("license_no");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("log");

                    b.Property<long>("LogicId")
                        .HasColumnType("bigint")
                        .HasColumnName("logic_id");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("model");

                    b.Property<string>("NumberPlate")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("number_plate");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<long>("RecordNo")
                        .HasColumnType("bigint")
                        .HasColumnName("record_no");

                    b.Property<DateOnly>("RegisterDate")
                        .HasColumnType("date")
                        .HasColumnName("register_date");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<DateOnly>("TransportPermitExpireAt")
                        .HasColumnType("date")
                        .HasColumnName("t_permit_exp");

                    b.Property<string>("Usci")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("usci");

                    b.Property<string>("Vin")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("vin");

                    b.HasKey("Id")
                        .HasName("pk_vehicles");

                    b.ToTable("vehicles", (string)null);
                });

            modelBuilder.Entity("CaC2O4.Repositories.Contract", b =>
                {
                    b.HasOne("CaC2O4.Repositories.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_contracts_employees_employee_id");

                    b.HasOne("CaC2O4.Repositories.Vehicle", null)
                        .WithMany()
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_contracts_vehicles_vehicle_id");
                });

            modelBuilder.Entity("CaC2O4.Repositories.Login", b =>
                {
                    b.HasOne("CaC2O4.Repositories.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired()
                        .HasConstraintName("fk_logins_users_user_id");
                });

            modelBuilder.Entity("CaC2O4.Repositories.Paper", b =>
                {
                    b.HasOne("CaC2O4.Repositories.Vehicle", null)
                        .WithMany()
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_papers_vehicles_vehicle_id");
                });

            modelBuilder.Entity("CaC2O4.Repositories.UploadingRecord", b =>
                {
                    b.HasOne("CaC2O4.Repositories.User", null)
                        .WithMany()
                        .HasForeignKey("Creator")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_uploading_records_users_user_id");
                });
#pragma warning restore 612, 618
        }
    }
}
