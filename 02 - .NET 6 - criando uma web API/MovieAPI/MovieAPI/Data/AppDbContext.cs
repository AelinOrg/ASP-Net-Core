using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>()
            .HasKey(session => new { session.MovieId, session.CineId });

        modelBuilder.Entity<Session>()
            .HasOne(session => session.Movie)
            .WithMany(movie => movie.Sessions)
            .HasForeignKey(session => session.MovieId);

        modelBuilder.Entity<Session>()
            .HasOne(session => session.Cine)
            .WithMany(cine => cine.Sessions)
            .HasForeignKey(session => session.CineId);
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Cine> Cines { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Session> Sessions { get; set; }
}