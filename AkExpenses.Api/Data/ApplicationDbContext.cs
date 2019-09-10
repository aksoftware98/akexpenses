using AkExpenses.Api.Models;
using AkExpenses.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkExpenses.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        // Tables 
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Outcome> Outcomes { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<DebtPayment> DebtPayment { get; set; }
        public DbSet<MoneyType> MoneyTypes { get; set; }

        public DbSet<Debt> Debts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Income>()
                .ToTable("Incomes");
            modelBuilder.Entity<Outcome>()
                .ToTable("Outcomes"); 

            base.OnModelCreating(modelBuilder);
        }

    }
}
