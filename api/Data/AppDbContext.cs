using api.Models.Clients;
using api.Models.Contacts;
using api.Models.Jobs;
using api.Models.Notes;
using api.Models.Projects;
using api.Models.Requests;
using api.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<UserInvite> UserInvites { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;

    public DbSet<Request> Requests { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Contact> Contacts { get; set; } = null!;

    public DbSet<Note> Notes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        builder.Entity<Client>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<Client>()
            .HasIndex(u => u.Name)
            .IsUnique();

        builder.Entity<Client>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();
    }
}
