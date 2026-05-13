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
                Bio = "Maria focuses on condos and townhouses across Metro Manilaâ€”BGC, Makati, and Ortigasâ€”with 10+ years of experience."
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
                PropertyId = 1,
                Title = "Two Serendra Corner Unit",
                Location = "Bonifacio Global City, Taguig",
                Sqft = 78,
                BasePrice = 24_500_000m,
                PropertyType = "Condo",
                SellerId = 1,
                Description =
                    "Bright two-bedroom facing the amenity deck, with balcony, split-type AC, and one parking slot. Near High Street and international schools.",
                Status = "Available",
                Bedrooms = 2,
                Bathrooms = 1,
                ParkingSlots = 1
            },
            new Property
            {
                PropertyId = 2,
                Title = "Legazpi Village 1BR",
                Location = "Legazpi Village, Makati",
                Sqft = 42,
                BasePrice = 10_500_000m,
                PropertyType = "Condo",
                SellerId = 1,
                Description =
                    "Walk to Greenbelt and Ayala Triangle. Semi-furnished, good for young professionals; building has pool and gym.",
                Status = "Available",
                Bedrooms = 1,
                Bathrooms = 1,
                ParkingSlots = 0
            },
            new Property
            {
                PropertyId = 3,
                Title = "Loyola Heights Townhouse",
                Location = "Loyola Heights, Quezon City",
                Sqft = 165,
                BasePrice = 12_800_000m,
                PropertyType = "House",
                SellerId = 1,
                Status = "Available",
                Description =
                    "Quiet residential street near Ateneo and Katipunan. Four bedrooms, carport, and small gardenâ€”ideal for families.",
            },
            new Property
            {
                PropertyId = 4,
                Title = "Cebu IT Park Studio Loft",
                Location = "Cebu IT Park, Cebu City",
                Sqft = 48,
                BasePrice = 6_200_000m,
                PropertyType = "House",
                SellerId = 2,
                Status = "Available",
                Description =
                    "High-floor studio with loft layout, fiber-ready, walking distance to offices and nightlife. Strong rental demand in the area.",
            },
            new Property
            {
                PropertyId = 5,
                Title = "Ortigas Center 2BR",
                Location = "Ortigas Center, Pasig",
                Sqft = 72,
                BasePrice = 18_900_000m,
                PropertyType = "House",
                SellerId = 1,
                Status = "Available",
                Description =
                    "Corner unit with city view, two baths, and maid's room. Direct bridge access to malls and MRT-3 connection points.",
            },
            new Property
            {
                PropertyId = 6,
                Title = "Rockwell Proscenium Lease",
                Location = "Rockwell Center, Makati",
                Sqft = 95,
                BasePrice = 72_000m,
                PropertyType = "House",
                SellerId = 3,
                Status = "Available",
                Description =
                    "Fully furnished two-bedroom with Rockwell amenities, concierge, and basement parking. Minimum 12-month lease; pets on approval.",
            },
            new Property
            {
                PropertyId = 7,
                Title = "Tomas Morato 1BR",
                Location = "Tomas Morato, Quezon City",
                Sqft = 38,
                BasePrice = 22_000m,
                PropertyType = "House",
                SellerId = 3,
                Status = "Available",
                Description =
                    "Newly painted unit near restaurants and QCP nightlife. Inverter AC, own meter, with balcony overlooking the street.",
            },
            new Property
            {
                PropertyId = 8,
                Title = "Lanang Studio",
                Location = "Lanang, Davao City",
                Sqft = 32,
                BasePrice = 14_500m,
                PropertyType = "House",
                SellerId = 2,
                Status = "Available",
                Description =
                    "Compact studio near SM Lanang with 24/7 security and pool. Ideal for students or young professionals; utilities billed separately.",
            },
            new Property
            {
                PropertyId = 9,
                Title = "Forbes Park Estate Home",
                Location = "Forbes Park, Makati",
                Sqft = 450,
                BasePrice = 185_000_000m,
                PropertyType = "House",
                SellerId = 1,
                Status = "Available",
                Description =
                    "Gated village property with pool, lanai, and staff quarters. Rare listing with wide frontage and mature trees.",
            }
            ,
            new Property
            {
                PropertyId = 10,
                Title = "BGC High-Rise 1BR",
                Location = "Bonifacio Global City, Taguig",
                Sqft = 50,
                BasePrice = 0m,
                PropertyType = "House",
                SellerId = 1,
                Status = "Available",
                Description = "One-bedroom high-rise with amenity deck view and quick access to retail and transit.",
            },
            new Property
            {
                PropertyId = 11,
                Title = "Makati Serviced Studio",
                Location = "Makati CBD, Makati",
                Sqft = 30,
                BasePrice = 28_000m,
                PropertyType = "House",
                SellerId = 3,
                Status = "Available",
                Description = "Compact studio ideal for single professionals, close to offices and nightlife.",
            },
            new Property
            {
                PropertyId = 12,
                Title = "Quezon City Family Home",
                Location = "Teacher's Village, Quezon City",
                Sqft = 210,
                BasePrice = 32_000_000m,
                PropertyType = "House",
                SellerId = 1,
                Status = "Available",
                Description = "Four-bedroom family house with garden and garage. Quiet street near schools.",
            },
            new Property
            {
                PropertyId = 13,
                Title = "Cebu Seaview Condo",
                Location = "Cebu City",
                Sqft = 85,
                BasePrice = 11_500_000m,
                PropertyType = "House",
                SellerId = 2,
                Status = "Available",
                Description = "Panoramic sea views, three-bedroom unit with modern finishes and concierge service.",
            },
            new Property
            {
                PropertyId = 14,
                Title = "Davao Affordable Studio",
                Location = "Davao City",
                Sqft = 28,
                BasePrice = 8_500m,
                PropertyType = "House",
                SellerId = 2,
                Status = "Available",
                Description = "Budget-friendly studio near universities and transit with shared amenities.",
            },
            new Property
            {
                PropertyId = 15,
                Title = "Ortigas Luxury 3BR",
                Location = "Ortigas Center, Pasig",
                Sqft = 160,
                BasePrice = 42_000_000m,
                PropertyType = "House",
                SellerId = 1,
                Status = "Available",
                Description = "Spacious three-bedroom in a premium tower with pool and function rooms.",
            },
            new Property
            {
                PropertyId = 16,
                Title = "Pasig Riverview 2BR",
                Location = "Ortigas, Pasig",
                Sqft = 75,
                BasePrice = 24_000m,
                PropertyType = "House",
                SellerId = 3,
                Status = "Available",
                Description = "Two-bedroom unit with river views and nearby mall access. Ideal for small families.",
            },
            new Property
            {
                PropertyId = 17,
                Title = "Suburban House with Garden",
                Location = "Alabang, Muntinlupa",
                Sqft = 300,
                BasePrice = 55_000_000m,
                PropertyType = "House",
                SellerId = 1,
                Status = "Available",
                Description = "Large family home in gated community with landscaped garden and pool.",
            },
            new Property
            {
                PropertyId = 18,
                Title = "Manila Studio Near University",
                Location = "Tondo, Manila",
                Sqft = 25,
                BasePrice = 6_500m,
                PropertyType = "House",
                SellerId = 3,
                Status = "Available",
                Description = "Simple studio suitable for students, close to public transport and universities.",
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

        // Broker assignment logic removed - Property model no longer has AgentId
        // Return all properties for any broker (simplified for demo)
        return _properties;
    }

    public Property? GetProperty(int id) => _properties.FirstOrDefault(p => p.PropertyId == id);

    public IReadOnlyList<Property> Filter(string? location, string? priceRange, decimal? maxPrice = null)
    {
        IEnumerable<Property> q = _properties;

        if (!string.IsNullOrWhiteSpace(location))
        {
            var loc = location.Trim();
            q = q.Where(p => p.Location.Contains(loc, StringComparison.OrdinalIgnoreCase));
        }

        if (maxPrice is { } cap && cap > 0m)
            q = q.Where(p => p.BasePrice <= cap);
        else if (!string.IsNullOrWhiteSpace(priceRange) && !priceRange.Equals("any", StringComparison.OrdinalIgnoreCase))
        {
            q = q.Where(p => MatchesPriceRange(p, priceRange));
        }

        return q.ToList();
    }

    private static bool MatchesPriceRange(Property p, string range)
    {
        var r = range.ToLowerInvariant();
        if (p.PropertyType == "House" || p.PropertyType == "Condo")
        {
            var peso = p.BasePrice;
            return r switch
            {
                "p1" => peso <= 8_000_000m,
                "p2" => peso > 8_000_000m && peso <= 15_000_000m,
                "p3" => peso > 15_000_000m && peso <= 30_000_000m,
                "p4" => peso > 30_000_000m,
                _ => true
            };
        }

        var monthly = p.BasePrice;
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
