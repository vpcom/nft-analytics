using Microsoft.EntityFrameworkCore;
using NftGraphApi.Models;
using NftGraphApi.Data;

namespace NftGraphApi.GraphQL;

[ExtendObjectType(typeof(Query))]
public class CollectionQuery
{
    public IQueryable<Collection> GetCollections(AppDbContext db) =>
        db.Collections
          .Include(c => c.Nfts);

    public Collection? GetCollection(AppDbContext db, string slug) =>
        db.Collections
          .Include(c => c.Nfts)
          .FirstOrDefault(c => c.Slug == slug);
}
