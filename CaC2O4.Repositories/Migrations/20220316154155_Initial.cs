using System;
using System.Collections.Generic;
using CaC2O4.Repositories;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaC2O4.Repositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:doc_subject", "not_applicable,vehicle,employee,contract")
                .Annotation("Npgsql:Enum:file_state", "unavailable,blank,current,editing,archived")
                .Annotation("Npgsql:Enum:fuel_type", "unknow_type,electric,hybrid_cng,hybrid_electric")
                .Annotation("Npgsql:Enum:user_acl", "forbidden,staff,archivist,admin,supvr,super");

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject = table.Column<int>(type: "integer", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    versions = table.Column<List<string>>(type: "text[]", nullable: false),
                    log = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    logic_id = table.Column<long>(type: "bigint", nullable: false),
                    tin = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    name_index = table.Column<string>(type: "text", nullable: false),
                    gender = table.Column<string>(type: "text", nullable: false),
                    ethnicity = table.Column<string>(type: "text", nullable: false),
                    dob = table.Column<DateOnly>(type: "date", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    household = table.Column<string>(type: "text", nullable: false),
                    cert_no = table.Column<string>(type: "text", nullable: false),
                    scard_exp = table.Column<DateOnly>(type: "date", nullable: false),
                    record_no = table.Column<long>(type: "bigint", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    education_level = table.Column<string>(type: "text", nullable: false),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    log = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    acl = table.Column<UserAcl>(type: "user_acl", nullable: false),
                    last_access = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    logic_id = table.Column<long>(type: "bigint", nullable: false),
                    number_plate = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    vin = table.Column<string>(type: "text", nullable: false),
                    engine_no = table.Column<string>(type: "text", nullable: false),
                    fuel_type = table.Column<FuelType>(type: "fuel_type", nullable: false),
                    register_date = table.Column<DateOnly>(type: "date", nullable: false),
                    usci = table.Column<string>(type: "text", nullable: false),
                    license_no = table.Column<string>(type: "text", nullable: false),
                    business_name = table.Column<string>(type: "text", nullable: false),
                    t_permit_exp = table.Column<DateOnly>(type: "date", nullable: false),
                    b_license_exp = table.Column<DateOnly>(type: "date", nullable: false),
                    record_no = table.Column<long>(type: "bigint", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    log = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vehicles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "logins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    issue_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logins", x => x.id);
                    table.ForeignKey(
                        name: "fk_logins_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "uploading_records",
                columns: table => new
                {
                    key = table.Column<string>(type: "text", nullable: false),
                    creator = table.Column<Guid>(type: "uuid", nullable: false),
                    is_success = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    successed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_uploading_records", x => x.key);
                    table.ForeignKey(
                        name: "fk_uploading_records_users_user_id",
                        column: x => x.creator,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    log = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contracts", x => x.id);
                    table.ForeignKey(
                        name: "fk_contracts_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_contracts_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "papers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_no = table.Column<long>(type: "bigint", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    log = table.Column<string>(type: "text", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_papers", x => x.id);
                    table.ForeignKey(
                        name: "fk_papers_vehicles_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contracts_employee_id",
                table: "contracts",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_vehicle_id_employee_id",
                table: "contracts",
                columns: new[] { "vehicle_id", "employee_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_documents_subject_id_title",
                table: "documents",
                columns: new[] { "subject_id", "title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_logins_user_id",
                table: "logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_papers_vehicle_id",
                table: "papers",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "ix_uploading_records_creator",
                table: "uploading_records",
                column: "creator");

            migrationBuilder.CreateIndex(
                name: "ix_users_name",
                table: "users",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "logins");

            migrationBuilder.DropTable(
                name: "papers");

            migrationBuilder.DropTable(
                name: "uploading_records");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
