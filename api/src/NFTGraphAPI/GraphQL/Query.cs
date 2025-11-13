using NftGraphApi.Models;
using NftGraphApi.Data;

namespace NftGraphApi.GraphQL;

public class Query
{
    public IQueryable<Nft> GetNfts(AppDbContext context) => context.Nfts;
}