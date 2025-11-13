using Microsoft.EntityFrameworkCore;
using NftGraphApi.Models;

namespace NftGraphApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Nft> Nfts => Set<Nft>();
    
    public DbSet<Collection> Collections => Set<Collection>();
}