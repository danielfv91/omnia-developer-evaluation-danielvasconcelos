using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SaleNumber)
                .IsRequired();

            builder.Property(s => s.SaleDate)
                .IsRequired();

            builder.Property(s => s.CustomerId)
                .IsRequired();

            builder.Property(s => s.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.Branch)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.IsCancelled)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt);

            builder.HasMany(s => s.Items)
                .WithOne()
                .HasForeignKey(i => i.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.HasKey(si => si.Id);

            builder.Property(si => si.ProductId)
                .IsRequired();

            builder.Property(si => si.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(si => si.Quantity)
                .IsRequired();

            builder.Property(si => si.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(si => si.DiscountPercentage)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(si => si.TotalItemAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(si => si.IsCancelled)
                .IsRequired();
        }
    }
}
