using GoldenCrown.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            SeedUserData(userentity);


            var accountentity = modelBuilder.Entity<Account>()
                .ToTable("account");
            accountentity.HasKey(a => a.Id);
            accountentity.Property(a => a.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            accountentity.Property(a => a.Balance)
                .HasColumnName("balance")
                .HasPrecision(19, 4)
                .IsRequired();
            accountentity.Property(a => a.UserId)
                .HasColumnName("user_id")
                .IsRequired();
            accountentity.HasOne<User>() //Связь 1 к 1
                .WithOne()
                .HasForeignKey<Account>(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            var sessionentity = modelBuilder.Entity<Session>()
                .ToTable("session");
            sessionentity.HasKey(s => s.UserId);
            sessionentity.Property(s => s.UserId)
                .HasColumnName("user_Id")
                .IsRequired();
            sessionentity.Property(s => s.Token)
                .HasColumnName("token")
                .IsRequired();
            sessionentity.Property(s => s.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired();
            sessionentity.HasOne<User>()
                .WithOne()
                .HasForeignKey<Session>(s => s.UserId);

            var transactionentity = modelBuilder.Entity<Transaction>()
                .ToTable("transaction");
            transactionentity.HasKey(t => t.Id);
            transactionentity.Property(t => t.Id)
                .HasColumnName("session_id")
                .UseIdentityColumn();
            transactionentity.Property(t => t.Amoutn)
                .HasColumnName("amoutn")
                .HasPrecision(19, 4)
                .IsRequired();
            transactionentity.Property(t => t.CreateAt)
                .HasColumnName("create_at")
                .IsRequired();
            transactionentity.Property(t => t.SenderAccountId)
                .HasColumnName("sender_account_id")
                .IsRequired();
            transactionentity.Property(t => t.ReceiverAccountId)
                .HasColumnName("reciever_account_id")
                .IsRequired();
            transactionentity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.SenderAccountId)
                .OnDelete(DeleteBehavior.NoAction);
            transactionentity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.NoAction);

        }

        private void SeedUserData(EntityTypeBuilder<User> userentity)
        {
            userentity.HasData(
                new User { Id = 1, Name = "Kostya", Login = "Kostya", Password = "123" },
                new User { Id = 2, Name = "Mark", Login = "Mark", Password = "1234" },
                new User { Id = 3, Name = "Tom", Login = "Tom", Password = "1235" }
                );
        }
    }
}
