namespace NftGraphApi.Models;

public class Nft
{
  public int Id { get; set; }
  public string Name { get; set; } = default!;
  public string ImageUrl { get; set; } = default!;
  public string Trait { get; set; } = default!;
  public double RarityScore { get; set; }
  public int CollectionId { get; set; }
  public Collection? Collection { get; set; }
}