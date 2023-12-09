using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tax.Matters.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IncomeTax",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FlatRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTax", x => x.Id);
                    table.CheckConstraint("CK_IncomeTax_FlatRate", "[FlatRate] <= 100.00");
                });

            migrationBuilder.CreateTable(
                name: "FlatValueIncomeTax",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Threshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThresholdRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IncomeTaxId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlatValueIncomeTax", x => x.Id);
                    table.CheckConstraint("CK_FlatValueIncomeTax_ThresholdRate", "[ThresholdRate] <= 100.00");
                    table.ForeignKey(
                        name: "FK_FlatValueIncomeTax_IncomeTax_IncomeTaxId",
                        column: x => x.IncomeTaxId,
                        principalTable: "IncomeTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostalCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IncomeTaxId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostalCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostalCode_IncomeTax_IncomeTaxId",
                        column: x => x.IncomeTaxId,
                        principalTable: "IncomeTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressiveIncomeTax",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinimumIncome = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaximumIncome = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IncomeTaxId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressiveIncomeTax", x => x.Id);
                    table.CheckConstraint("CK_ProgressiveIncomeTax_MinimumIncome_vs_MaximumIncome", "[MaximumIncome] IS NULL OR ([MinimumIncome] < [MaximumIncome])");
                    table.CheckConstraint("CK_ProgressiveIncomeTax_Rate", "[Rate] <= 100.00");
                    table.ForeignKey(
                        name: "FK_ProgressiveIncomeTax_IncomeTax_IncomeTaxId",
                        column: x => x.IncomeTaxId,
                        principalTable: "IncomeTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaxCalculation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AnnualIncome = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PostalCodeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCalculation", x => x.Id);
                    table.CheckConstraint("CK_TaxCalculation_AnnualIncome", "[AnnualIncome] >= 0.00");
                    table.ForeignKey(
                        name: "FK_TaxCalculation_PostalCode_PostalCodeId",
                        column: x => x.PostalCodeId,
                        principalTable: "PostalCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlatValueIncomeTax_IncomeTaxId",
                table: "FlatValueIncomeTax",
                column: "IncomeTaxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostalCode_Code",
                table: "PostalCode",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostalCode_IncomeTaxId",
                table: "PostalCode",
                column: "IncomeTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressiveIncomeTax_IncomeTaxId",
                table: "ProgressiveIncomeTax",
                column: "IncomeTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressiveIncomeTax_MinimumIncome_IncomeTaxId",
                table: "ProgressiveIncomeTax",
                columns: new[] { "MinimumIncome", "IncomeTaxId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxCalculation_PostalCodeId",
                table: "TaxCalculation",
                column: "PostalCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "FlatValueIncomeTax");

            migrationBuilder.DropTable(
                name: "ProgressiveIncomeTax");

            migrationBuilder.DropTable(
                name: "TaxCalculation");

            migrationBuilder.DropTable(
                name: "PostalCode");

            migrationBuilder.DropTable(
                name: "IncomeTax");
        }
    }
}
