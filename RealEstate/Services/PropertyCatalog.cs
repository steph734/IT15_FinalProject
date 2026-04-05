using System.Linq;
using RealEstate.Models;

namespace RealEstate.Services;

public class PropertyCatalog
{
    private readonly IReadOnlyList<Agent> _agents;
    private readonly IReadOnlyList<Property> _properties;

    public PropertyCatalog()
    {
        _agents =
        [
            new Agent
            {
                Id = 1,
                Name = "Maria Santos",
                Title = "Senior Property Consultant",
                Email = "maria.santos@estateflow.ph",
                Phone = "+63 (2) 8812-4400",
                PhotoUrl = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?auto=format&fit=crop&w=400&q=80",
                Bio = "Maria focuses on condos and townhouses across Metro Manila—BGC, Makati, and Ortigas—with 10+ years of experience."
            },
            new Agent
            {
                Id = 2,
                Name = "James Reyes",
                Title = "Visayas & Mindanao Listings",
                Email = "james.reyes@estateflow.ph",
                Phone = "+63 (32) 255-9100",
                PhotoUrl = "https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=400&q=80",
                Bio = "James leads Cebu and Davao developments, from IT Park condos to suburban family homes."
            },
            new Agent
            {
                Id = 3,
                Name = "Liza Cruz",
                Title = "Leasing & Rentals",
                Email = "liza.cruz@estateflow.ph",
                Phone = "+63 (2) 8812-4401",
                PhotoUrl = "https://images.unsplash.com/photo-1580489944761-15a19d654956?auto=format&fit=crop&w=400&q=80",
                Bio = "Liza helps tenants find flexible leases in Makati, BGC, and Quezon City with clear move-in timelines."
            }
        ];

        _properties =
        [
            new Property
            {
                Id = 1,
                Title = "Two Serendra Corner Unit",
                Location = "Bonifacio Global City, Taguig",
                Sqft = 78,
                Price = 24_500_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description =
                    "Bright two-bedroom facing the amenity deck, with balcony, split-type AC, and one parking slot. Near High Street and international schools.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1523217582562-09d0def993a6?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 2,
                Title = "Legazpi Village 1BR",
                Location = "Legazpi Village, Makati",
                Sqft = 42,
                Price = 10_500_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description =
                    "Walk to Greenbelt and Ayala Triangle. Semi-furnished, good for young professionals; building has pool and gym.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1600585154526-990dced4db3d?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600566752355-35792bedcfea?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 3,
                Title = "Loyola Heights Townhouse",
                Location = "Loyola Heights, Quezon City",
                Sqft = 165,
                Price = 12_800_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description =
                    "Quiet residential street near Ateneo and Katipunan. Four bedrooms, carport, and small garden—ideal for families.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1600566752355-35792bedcfea?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600047509807-ba8f99d2cdde?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 4,
                Title = "Cebu IT Park Studio Loft",
                Location = "Cebu IT Park, Cebu City",
                Sqft = 48,
                Price = 6_200_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 2,
                Description =
                    "High-floor studio with loft layout, fiber-ready, walking distance to offices and nightlife. Strong rental demand in the area.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 5,
                Title = "Ortigas Center 2BR",
                Location = "Ortigas Center, Pasig",
                Sqft = 72,
                Price = 18_900_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description =
                    "Corner unit with city view, two baths, and maid's room. Direct bridge access to malls and MRT-3 connection points.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600585154084-4e5fe7c39198?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600607687644-c7171b42498f?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 6,
                Title = "Rockwell Proscenium Lease",
                Location = "Rockwell Center, Makati",
                Sqft = 95,
                Price = 72_000m,
                ListingType = PropertyListingType.Rent,
                AgentId = 3,
                Description =
                    "Fully furnished two-bedroom with Rockwell amenities, concierge, and basement parking. Minimum 12-month lease; pets on approval.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600585154084-4e5fe7c39198?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 7,
                Title = "Tomas Morato 1BR",
                Location = "Tomas Morato, Quezon City",
                Sqft = 38,
                Price = 22_000m,
                ListingType = PropertyListingType.Rent,
                AgentId = 3,
                Description =
                    "Newly painted unit near restaurants and QCP nightlife. Inverter AC, own meter, with balcony overlooking the street.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1600585154526-990dced4db3d?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600566753086-00f18fb6b3ea?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600047509358-9edc377768e0?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 8,
                Title = "Lanang Studio",
                Location = "Lanang, Davao City",
                Sqft = 32,
                Price = 14_500m,
                ListingType = PropertyListingType.Rent,
                AgentId = 2,
                Description =
                    "Compact studio near SM Lanang with 24/7 security and pool. Ideal for students or young professionals; utilities billed separately.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600566752355-35792bedcfea?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 9,
                Title = "Forbes Park Estate Home",
                Location = "Forbes Park, Makati",
                Sqft = 450,
                Price = 185_000_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description =
                    "Gated village property with pool, lanai, and staff quarters. Rare listing with wide frontage and mature trees.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?auto=format&fit=crop&w=1200&q=80",
                    "https://images.unsplash.com/photo-1600047509807-ba8f99d2cdde?auto=format&fit=crop&w=1200&q=80"
                ]
            }
            ,
            new Property
            {
                Id = 10,
                Title = "BGC High-Rise 1BR",
                Location = "Bonifacio Global City, Taguig",
                Sqft = 50,
                Price = 9_500_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description = "One-bedroom high-rise with amenity deck view and quick access to retail and transit.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1580587771525-78b9dba3b914?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 11,
                Title = "Makati Serviced Studio",
                Location = "Makati CBD, Makati",
                Sqft = 30,
                Price = 28_000m,
                ListingType = PropertyListingType.Rent,
                AgentId = 3,
                Description = "Compact studio ideal for single professionals, close to offices and nightlife.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1505691723518-36a5f9985b4c?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 12,
                Title = "Quezon City Family Home",
                Location = "Teacher's Village, Quezon City",
                Sqft = 210,
                Price = 32_000_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description = "Four-bedroom family house with garden and garage. Quiet street near schools.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1507089947368-19c1da9775ae?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 13,
                Title = "Cebu Seaview Condo",
                Location = "Cebu City",
                Sqft = 85,
                Price = 11_500_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 2,
                Description = "Panoramic sea views, three-bedroom unit with modern finishes and concierge service.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1493809842364-78817add7ffb?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 14,
                Title = "Davao Affordable Studio",
                Location = "Davao City",
                Sqft = 28,
                Price = 8_500m,
                ListingType = PropertyListingType.Rent,
                AgentId = 2,
                Description = "Budget-friendly studio near universities and transit with shared amenities.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1496307042754-b4aa456c4a2d?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 15,
                Title = "Ortigas Luxury 3BR",
                Location = "Ortigas Center, Pasig",
                Sqft = 160,
                Price = 42_000_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description = "Spacious three-bedroom in a premium tower with pool and function rooms.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 16,
                Title = "Pasig Riverview 2BR",
                Location = "Ortigas, Pasig",
                Sqft = 75,
                Price = 24_000m,
                ListingType = PropertyListingType.Rent,
                AgentId = 3,
                Description = "Two-bedroom unit with river views and nearby mall access. Ideal for small families.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1505691723518-36a5f9985b4c?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 17,
                Title = "Suburban House with Garden",
                Location = "Alabang, Muntinlupa",
                Sqft = 300,
                Price = 55_000_000m,
                ListingType = PropertyListingType.Buy,
                AgentId = 1,
                Description = "Large family home in gated community with landscaped garden and pool.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1493809842364-78817add7ffb?auto=format&fit=crop&w=1200&q=80"
                ]
            },
            new Property
            {
                Id = 18,
                Title = "Manila Studio Near University",
                Location = "Tondo, Manila",
                Sqft = 25,
                Price = 6_500m,
                ListingType = PropertyListingType.Rent,
                AgentId = 3,
                Description = "Simple studio suitable for students, close to public transport and universities.",
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1505691723518-36a5f9985b4c?auto=format&fit=crop&w=1200&q=80"
                ]
            }
        ];
    }

    public IReadOnlyList<Agent> GetAgents() => _agents;

    public Agent? GetAgent(int id) => _agents.FirstOrDefault(a => a.Id == id);

    public IReadOnlyList<Property> GetProperties() => _properties;

    /// <summary>
    /// Get properties assigned to a broker username. Mapping rule:
    /// - "admin" returns all properties.
    /// - usernames containing digits (e.g. agen001) map trailing digits to Agent.Id (001 -> 1).
    /// - otherwise returns empty list.
    /// </summary>
    public IReadOnlyList<Property> GetPropertiesForBroker(string? brokerUsername)
    {
        if (string.IsNullOrWhiteSpace(brokerUsername))
            return Array.Empty<Property>();

        if (brokerUsername.Equals("admin", StringComparison.OrdinalIgnoreCase))
            return _properties;

        var digits = new string(brokerUsername.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, out var agentId))
        {
            return _properties.Where(p => p.AgentId == agentId).ToList();
        }

        return Array.Empty<Property>();
    }

    public Property? GetProperty(int id) => _properties.FirstOrDefault(p => p.Id == id);

    public IReadOnlyList<Property> Filter(string? location, string? priceRange, decimal? maxPrice = null)
    {
        IEnumerable<Property> q = _properties;

        if (!string.IsNullOrWhiteSpace(location))
        {
            var loc = location.Trim();
            q = q.Where(p => p.Location.Contains(loc, StringComparison.OrdinalIgnoreCase));
        }

        if (maxPrice is { } cap && cap > 0m)
            q = q.Where(p => p.Price <= cap);
        else if (!string.IsNullOrWhiteSpace(priceRange) && !priceRange.Equals("any", StringComparison.OrdinalIgnoreCase))
        {
            q = q.Where(p => MatchesPriceRange(p, priceRange));
        }

        return q.ToList();
    }

    private static bool MatchesPriceRange(Property p, string range)
    {
        var r = range.ToLowerInvariant();
        if (p.ListingType == PropertyListingType.Buy)
        {
            var peso = p.Price;
            return r switch
            {
                "p1" => peso <= 8_000_000m,
                "p2" => peso > 8_000_000m && peso <= 15_000_000m,
                "p3" => peso > 15_000_000m && peso <= 30_000_000m,
                "p4" => peso > 30_000_000m,
                _ => true
            };
        }

        var monthly = p.Price;
        return r switch
        {
            "p1" => monthly <= 25_000m,
            "p2" => monthly > 25_000m && monthly <= 45_000m,
            "p3" => monthly > 45_000m && monthly <= 75_000m,
            "p4" => monthly > 75_000m,
            _ => true
        };
    }
}
