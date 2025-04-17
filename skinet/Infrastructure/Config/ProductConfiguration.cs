using System;
using System.Linq.Expressions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /* builder lets you tell EF Core how to map the Product class to a 
    table in the database.*/
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        /*Property(x=>x.Price) selects a property from the class using a
        lambda expression. given class (x), we want the Price (x.price)*/
        /*HasColumnType(decimal(18,2)") sets database to have total of 
        18 digits and 2 digits after decimal point*/
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Name).IsRequired();
    }
}
