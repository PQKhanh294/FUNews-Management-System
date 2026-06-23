using FUNewsManagementSystem.BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DataAccess.Context;

public partial class FUNewsManagementContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NewsArticle>()
            .Property(e => e.ApprovalStatus)
            .HasColumnType("tinyint")
            .HasDefaultValue((byte)0);

        modelBuilder.Entity<SystemAccount>(entity =>
        {
            entity.Property(e => e.GoogleId).HasMaxLength(100);
            entity.Property(e => e.AvatarUrl).HasMaxLength(300);
            entity.Property(e => e.IsExternalLogin).HasDefaultValue(false);
        });
    }
}

