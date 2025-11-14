using Microsoft.EntityFrameworkCore;
using NftGraphApi.Data;
using NftGraphApi.GraphQL;
using NftGraphApi.Models;
using HotChocolate.Subscriptions;


var builder = WebApplication.CreateBuilder(args);

// CORS policy
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowFrontend",
      policy =>
      {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3002")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
      });
});

// Registering classes to container
builder.Services
    .AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=nfts.db"))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<CollectionQuery>()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions();

var app = builder.Build();

// Ensure database exists & seed NFTs
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  db.Database.EnsureCreated();

  if (!db.Collections.Any())
  {
    var coolCats = new Collection { Name = "Cool Cats", Slug = "cool-cats" };

    foreach (var nft in new[]
       {
        new Nft { Name = "Aurora Cat", ImageUrl = "https://cdn2.thecatapi.com/images/MTY3ODIyMQ.jpg", Trait = "Home", RarityScore = 0.96 },
        new Nft { Name = "Cyber Cat", ImageUrl = "https://cdn2.thecatapi.com/images/bpc.jpg", Trait = "Nature", RarityScore = 0.91 },
        new Nft { Name = "Pharaoh Cat", ImageUrl = "https://www.alleycat.org/wp-content/uploads/2019/03/FELV-cat.jpg", Trait = "Nature", RarityScore = 0.88 },
        new Nft { Name = "Shadow Cat", ImageUrl = "https://hips.hearstapps.com/hmg-prod/images/white-cat-breeds-kitten-in-grass-67bf648a54a3b.jpg", Trait = "Nature", RarityScore = 0.82 },
        new Nft { Name = "Solar Cat", ImageUrl = "https://cdn2.thecatapi.com/images/6qi.jpg", Trait = "Home", RarityScore = 0.95 },
        new Nft { Name = "Lunar Cat", ImageUrl = "https://i.natgeofe.com/n/9135ca87-0115-4a22-8caf-d1bdef97a814/75552.jpg", Trait = "Home", RarityScore = 0.89 },
        new Nft { Name = "Neon Cat", ImageUrl = "https://cdn2.thecatapi.com/images/9si.jpg", Trait = "Home", RarityScore = 0.9 },
        new Nft { Name = "Crimson Cat", ImageUrl = "https://cdn2.thecatapi.com/images/d1a.jpg", Trait = "Fire", RarityScore = 0.86 },
        new Nft { Name = "Frost Cat", ImageUrl = "https://cataas.com/cat", Trait = "Random", RarityScore = 0.84 },
        new Nft { Name = "Emerald Cat", ImageUrl = "https://cataas.com/cat", Trait = "Random", RarityScore = 0.8 }
    }
    )
    {
      coolCats.Nfts.Add(nft);
    }
    db.Collections.Add(coolCats);

    db.SaveChanges();
  }

  // Event publisher simulation
  var publisher = app.Services.GetRequiredService<ITopicEventSender>();

  _ = Task.Run(async () =>
  {
    try
    {
      Console.WriteLine("Starting sale publishing task...");
      var random = new Random();

      while (true)
      {
        // Each loop creates a new service scope
        using var scope = app.Services.CreateScope();
        var scopedDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var nft = scopedDb.Nfts.ToList().OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
        if (nft is not null)
        {
          Console.WriteLine($"📢 Publishing new sale: {nft.Name}");
          await publisher.SendAsync(nameof(Subscription.OnNewSale), nft);
        }
        else
        {
          Console.WriteLine("No NFTs found in database.");
        }

        await Task.Delay(TimeSpan.FromSeconds(10));
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in publishing task: {ex.Message}");
    }
  });

  app.UseCors("AllowFrontend");
  app.MapGet("/", () => "Hello World!");
  app.UseWebSockets();
  app.MapGraphQL("/graphql");
  app.UseRouting();

  app.Run();
}
