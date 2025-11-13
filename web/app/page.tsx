"use client";

import { gql } from "@apollo/client";
import { useQuery, useSubscription } from "@apollo/client/react";
import { useMemo, useState, useEffect } from "react";

const GET_COLLECTIONS = gql`
  query {
    collections {
      id
      name
      slug
      nfts {
        id
        name
        trait
        rarityScore
        imageUrl
      }
    }
  }
`;

const NEW_SALE = gql`
  subscription {
    onNewSale {
      name
      trait
    }
  }
`;

export default function Home() {
  const { data, loading, error } = useQuery<any>(GET_COLLECTIONS);
  const [traitFilter, setTraitFilter] = useState("");
  const [sales, setSales] = useState<any[]>([]);

  const collections = data?.collections ?? [];

  const filteredCollections = useMemo(() => {
    const query = traitFilter.trim().toLowerCase();
    if (!query) return collections;

    return collections.map((col: any) => ({
      ...col,
      nfts: col.nfts.filter((nft: any) =>
        nft.trait?.toLowerCase().includes(query)
      ),
    }));
  }, [collections, traitFilter]);

  const { data: saleData } = useSubscription<any>(NEW_SALE);
  useEffect(() => {
    console.log("Received subscription data:", saleData);
    if (saleData?.onNewSale) {
      console.log("New sale:", saleData.onNewSale);
      setSales(prev => [{ ...saleData.onNewSale, time: new Date().toLocaleTimeString() }, ...prev]);
    }
  }, [saleData]);

  if (loading) return <p className="p-4">Loading…</p>;
  if (error) return <p className="p-4 text-red-500">Error: {error.message}</p>;

  // Assuming we display the first collection for now
  const collection = filteredCollections[0];
  if (!collection) return <p className="p-4">No collections found.</p>;

  return (
    <div className="p-6 space-y-4">
      <div>
        <label className="block text-sm font-medium text-zinc-600 mb-1">
          Filter by trait
        </label>
        <input
          type="text"
          placeholder="Type a trait name…"
          value={traitFilter}
          onChange={(event) => setTraitFilter(event.target.value)}
          className="w-full rounded-xl border border-stone-600 px-3 py-2 focus:outline-none focus:ring-2 focus:ring-black/40"
        />
      </div>
      <main className="min-h-screen bg-gray-50 p-6 grid grid-cols-1 lg:grid-cols-[1fr_300px] gap-6">
        {/* Main NFT Grid */}
        <section>
          <header className="mb-4 flex justify-between items-center">
            <h1 className="text-3xl font-bold text-gray-800">
              {collection.name}
            </h1>
            <p className="text-sm text-gray-600">
              {collection.nfts.length} NFTs • Avg rarity{" "}
              {(
                collection.nfts.reduce(
                  (a: number, n: any) => a + n.rarityScore,
                  0
                ) / collection.nfts.length
              ).toFixed(2)}
            </p>
          </header>

          <div className="grid sm:grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-4">
            {collection.nfts.map((nft: any) => (
              <div
                key={nft.id}
                className="bg-white rounded-2xl shadow-sm hover:shadow-md transition-shadow overflow-hidden border border-gray-100"
              >
                <img
                  src={nft.imageUrl}
                  alt={nft.name}
                  className="w-full h-56 object-cover"
                />
                <div className="p-4">
                  <h3 className="font-semibold text-lg text-gray-800 mb-1">
                    {nft.name}
                  </h3>
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-600">{nft.trait}</span>
                    <span className="bg-indigo-100 text-indigo-700 px-2 py-0.5 rounded-full">
                      {nft.rarityScore.toFixed(2)}
                    </span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </section>

        {/* Sidebar */}
        <aside className="bg-white rounded-2xl shadow-sm p-4 border border-gray-100 flex flex-col">
          <h2 className="font-semibold text-xl mb-3">Live Sales</h2>
          <div className="flex flex-col gap-2 overflow-auto">
            {sales.length === 0 ? (
              <p className="text-gray-500 text-sm">Waiting for new sales…</p>
            ) : (
              sales.map((s, i) => (
                <div
                  key={i}
                  className="bg-gray-50 border border-gray-100 p-3 rounded-xl text-sm"
                >
                  <p>
                    <span className="font-medium text-indigo-700">
                      {s.name}
                    </span>{" "}
                    sold!
                  </p>
                  <p className="text-gray-600 text-xs">
                    Trait: {s.trait} • {s.time}
                  </p>
                </div>
              ))
            )}
          </div>
        </aside>
      </main>
      {/* <main className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {filteredCollections.map((col: any) => (
          <div
            key={col.slug}
            className="bg-white rounded-2xl border border-stone-600 shadow p-4"
          >
            <h2 className="font-bold text-lg mb-2">{col.name}</h2>
            <div className="grid grid-cols-3 gap-2">
              {col.nfts.length === 0 ? (
                <p className="col-span-3 text-sm text-zinc-500">
                  No matching NFTs.
                </p>
              ) : (
                col.nfts.map((nft: any) => (
                  <>
                    <img
                      key={nft.id}
                      src={nft.imageUrl}
                      alt={nft.name}
                      className="rounded-xl"
                    />
                    <p className="text-sm text-center mt-1">{nft.name}</p>
                    <span className="text-xs bg-blue-100 text-blue-600 px-2 py-1 rounded-full">
                      Rarity {nft.rarityScore.toFixed(2)}
                    </span>
                  </>
                ))
              )}
            </div>
          </div>
        ))}
      </main> */}
    </div>
  );
}
