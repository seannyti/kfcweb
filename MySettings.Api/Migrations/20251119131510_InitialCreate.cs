using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySettings.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneralSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Tagline = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Favicon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timezone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AllowRegistration = table.Column<bool>(type: "bit", nullable: false),
                    ForceHttps = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tagline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Favicon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timezone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AllowRegistration = table.Column<bool>(type: "bit", nullable: false),
                    ForceHttps = table.Column<bool>(type: "bit", nullable: false),
                    EmailEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SmtpServer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmtpPort = table.Column<int>(type: "int", nullable: false),
                    UseSsl = table.Column<bool>(type: "bit", nullable: false),
                    SmtpUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmtpPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaintenanceMode = table.Column<bool>(type: "bit", nullable: false),
                    MaintenanceMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnableApiAccess = table.Column<bool>(type: "bit", nullable: false),
                    Enforce2FA = table.Column<bool>(type: "bit", nullable: false),
                    SessionTimeout = table.Column<int>(type: "int", nullable: false),
                    MaxLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    EnableIpWhitelist = table.Column<bool>(type: "bit", nullable: false),
                    WhitelistedIps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinPasswordLength = table.Column<int>(type: "int", nullable: false),
                    RequireUppercase = table.Column<bool>(type: "bit", nullable: false),
                    RequireNumbers = table.Column<bool>(type: "bit", nullable: false),
                    RequireSpecialChars = table.Column<bool>(type: "bit", nullable: false),
                    PrimaryColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuccessColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DangerColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WarningColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InfoColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DarkBg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DarkSurface = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DarkText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LightBg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LightSurface = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LightText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BorderRadius = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FontFamily = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DarkModeDefault = table.Column<bool>(type: "bit", nullable: false),
                    ForceDarkMode = table.Column<bool>(type: "bit", nullable: false),
                    UseGradientBg = table.Column<bool>(type: "bit", nullable: false),
                    UseGlassmorphism = table.Column<bool>(type: "bit", nullable: false),
                    AnimatedBg = table.Column<bool>(type: "bit", nullable: false),
                    AccentGradient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomCss = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSettings");

            migrationBuilder.DropTable(
                name: "SiteSettings");
        }
    }
}
