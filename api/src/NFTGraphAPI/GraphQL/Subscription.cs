using NftGraphApi.Models;

namespace NftGraphApi.GraphQL;

public class Subscription
{
    [Subscribe]
    [Topic]
    public Nft OnNewSale([EventMessage] Nft nft) => nft;
}
