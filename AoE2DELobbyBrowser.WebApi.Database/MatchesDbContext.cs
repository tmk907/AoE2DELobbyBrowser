using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace AoE2DELobbyBrowser.WebApi.Database
{
    public class MatchesDbContextFactory : IDesignTimeDbContextFactory<MatchesDbContext>
    {
        public MatchesDbContext CreateDbContext(string[] args)
        {
            return new MatchesDbContext("aoe2lobby-design.db");
        }
    }

    public class MatchesDbContext : DbContext
    {
        private readonly string? _dbPath;
        public MatchesDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        [ActivatorUtilitiesConstructor]
        public MatchesDbContext(DbContextOptions<MatchesDbContext> options) : base(options)
        {
        }

        public DbSet<LobbiesTable> Lobbies { get; set; }
        public DbSet<MatchesTable> Matches { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!string.IsNullOrEmpty(_dbPath))
            {
                options.UseSqlite($"Data Source={_dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LobbiesTable>()
                .HasKey(x => x.MatchId);

            modelBuilder.Entity<MatchesTable>()
                .HasKey(x => x.MatchId);
        }
    }
}
