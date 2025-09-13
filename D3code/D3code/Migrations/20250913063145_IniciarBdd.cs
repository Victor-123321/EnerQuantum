using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace D3code.Migrations
{
    /// <inheritdoc />
    public partial class IniciarBdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    GridType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "ClimateEvents",
                columns: table => new
                {
                    StartTimestamp = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    AreaId = table.Column<int>(type: "integer", nullable: false),
                    TempC = table.Column<double>(type: "double precision", nullable: true),
                    TempMinC = table.Column<double>(type: "double precision", nullable: true),
                    TempMaxC = table.Column<double>(type: "double precision", nullable: true),
                    ClimateCondition = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Phenomenon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DurationHours = table.Column<double>(type: "double precision", nullable: true),
                    PrecipitationMm = table.Column<double>(type: "double precision", nullable: true),
                    WindSpeedMps = table.Column<double>(type: "double precision", nullable: true),
                    PressureHpa = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClimateEvents", x => new { x.StartTimestamp, x.AreaId });
                    table.ForeignKey(
                        name: "FK_ClimateEvents_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnergyUsage",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    AreaId = table.Column<int>(type: "integer", nullable: false),
                    GenerationMWh = table.Column<double>(type: "double precision", nullable: true),
                    DemandMWh = table.Column<double>(type: "double precision", nullable: true),
                    ServiceStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LossesPct = table.Column<double>(type: "double precision", nullable: true),
                    Circuit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DurationHours = table.Column<double>(type: "double precision", nullable: true),
                    ImportMWh = table.Column<double>(type: "double precision", nullable: true),
                    ExportMWh = table.Column<double>(type: "double precision", nullable: true),
                    NetExchangeMWh = table.Column<double>(type: "double precision", nullable: true),
                    AreaId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyUsage", x => new { x.Timestamp, x.AreaId });
                    table.ForeignKey(
                        name: "FK_EnergyUsage_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "AreaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnergyUsage_Areas_AreaId1",
                        column: x => x.AreaId1,
                        principalTable: "Areas",
                        principalColumn: "AreaId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClimateEvents_AreaId",
                table: "ClimateEvents",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyUsage_AreaId",
                table: "EnergyUsage",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyUsage_AreaId1",
                table: "EnergyUsage",
                column: "AreaId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClimateEvents");

            migrationBuilder.DropTable(
                name: "EnergyUsage");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
