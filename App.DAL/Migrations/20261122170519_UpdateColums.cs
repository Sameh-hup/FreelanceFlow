using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_AspNetUsers_CreatedByUserId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Clients_ClientID",
                table: "Invitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations");

            migrationBuilder.RenameTable(
                name: "Invitations",
                newName: "Invitation");

            migrationBuilder.RenameColumn(
                name: "TaskID",
                table: "Tasks",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ProjectID",
                table: "Projects",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "PaymentID",
                table: "Payments",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "MilestoneID",
                table: "Milestones",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "InvoiceID",
                table: "Invoices",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "ClientID",
                table: "Clients",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "AuditLogID",
                table: "AuditLogs",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "InvitationID",
                table: "Invitation",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_TokenHash",
                table: "Invitation",
                newName: "IX_Invitation_TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_CreatedByUserId",
                table: "Invitation",
                newName: "IX_Invitation_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitations_ClientID",
                table: "Invitation",
                newName: "IX_Invitation_ClientID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invitation",
                table: "Invitation",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_AspNetUsers_CreatedByUserId",
                table: "Invitation",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_Clients_ClientID",
                table: "Invitation",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_AspNetUsers_CreatedByUserId",
                table: "Invitation");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_Clients_ClientID",
                table: "Invitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invitation",
                table: "Invitation");

            migrationBuilder.RenameTable(
                name: "Invitation",
                newName: "Invitations");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Tasks",
                newName: "TaskID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Projects",
                newName: "ProjectID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Payments",
                newName: "PaymentID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Milestones",
                newName: "MilestoneID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Invoices",
                newName: "InvoiceID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Clients",
                newName: "ClientID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "AuditLogs",
                newName: "AuditLogID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Invitations",
                newName: "InvitationID");

            migrationBuilder.RenameIndex(
                name: "IX_Invitation_TokenHash",
                table: "Invitations",
                newName: "IX_Invitations_TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_Invitation_CreatedByUserId",
                table: "Invitations",
                newName: "IX_Invitations_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Invitation_ClientID",
                table: "Invitations",
                newName: "IX_Invitations_ClientID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invitations",
                table: "Invitations",
                column: "InvitationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_AspNetUsers_CreatedByUserId",
                table: "Invitations",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Clients_ClientID",
                table: "Invitations",
                column: "ClientID",
                principalTable: "Clients",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
