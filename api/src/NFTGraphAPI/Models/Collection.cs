namespace NftGraphApi.Models;

public class Collection
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public ICollection<Nft> Nfts { get; set; } = new List<Nft>();
}