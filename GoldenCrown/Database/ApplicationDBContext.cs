using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Database
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Account> Accounts { get; set; } = null!;

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userentity = modelBuilder.Entity<User>()
                .ToTable("users"); //Название таблицы
            userentity.HasKey(u => u.Id); //Первичный ключ
            userentity.Property(u => u.Id) 
                .HasColumnName("id") //Название колонки
                .UseIdentityColumn();//AutoIncrement
            userentity.Property(u => u.Login)
                .HasColumnName("login")
                .IsRequired(); // Not Null
            userentity.Property(u => u.Name)
                .HasColumnName("name")
                .IsRequired();
            userentity.Property(u => u.Password)
                .HasColumnName("password")
                .IsRequired();

            var accountentity = modelBuilder.Entity<Account>()
                .ToTable("account");
            accountentity.HasKey(a => a.Id);
            accountentity.Property(a => a.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            accountentity.Property(a => a.Balance)
                .HasColumnName("balance")
                .IsRequired();
            accountentity.Property(a => a.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            accountentity.HasOne<User>() //Связь 1 к 1
                .WithOne()
                .HasForeignKey<Account>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление (удаление зависимой сущности после удаления главной)

            var sessionentity = modelBuilder.Entity<Session>()
                .ToTable("session");
            sessionentity.HasKey(s => s.UserId);
            sessionentity.Property(s => s.UserId)
                .HasColumnName("user_Id")
                .UseIdentityColumn();
            sessionentity.Property(s => s.Token)
                .HasColumnName("token")
                .IsRequired();
            sessionentity.Property(s => s.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired();
            sessionentity.HasOne<User>()
                .WithOne()
                .HasForeignKey<Session>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("");
        }
    }
}
