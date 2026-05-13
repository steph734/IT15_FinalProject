using Microsoft.EntityFrameworkCore;
using RealEstate.Models;

namespace RealEstate.Services
{
    public static class InspectionItemSeeder
    {
        public static async Task SeedInspectionItemsAsync(ApplicationDBContext context)
        {
            // Check if inspection items already exist
            if (await context.InspectionItems.AnyAsync())
            {
                Console.WriteLine("Inspection items already exist, skipping seed.");
                return;
            }

            var inspectionItems = new List<InspectionItem>
            {
                // Property Structure (Category 1)
                new InspectionItem { Category = "Property Structure", Name = "Foundation", Icon = "fa-layer-group", Criteria = "No visible structural cracks or signs of shifting.", SortOrder = 1 },
                new InspectionItem { Category = "Property Structure", Name = "Roof", Icon = "fa-home", Criteria = "No missing shingles; no signs of active leaks in attic/ceiling.", SortOrder = 2 },
                new InspectionItem { Category = "Property Structure", Name = "Walls", Icon = "fa-square", Criteria = "Interior/exterior surfaces are free of holes or major cracks.", SortOrder = 3 },
                new InspectionItem { Category = "Property Structure", Name = "Windows", Icon = "fa-window-maximize", Criteria = "Open/close smoothly; locks are functional; seals are intact.", SortOrder = 4 },
                new InspectionItem { Category = "Property Structure", Name = "Flooring", Icon = "fa-th-large", Criteria = "No trip hazards, loose tiles, or significant soft spots.", SortOrder = 5 },

                // Utilities (Category 2)
                new InspectionItem { Category = "Utilities", Name = "Electrical System", Icon = "fa-plug", Criteria = "All outlets/switches functional; no exposed wiring.", SortOrder = 6 },
                new InspectionItem { Category = "Utilities", Name = "Water Supply", Icon = "fa-tint", Criteria = "Adequate pressure at all faucets; water runs clear.", SortOrder = 7 },
                new InspectionItem { Category = "Utilities", Name = "Gas Connection", Icon = "fa-fire", Criteria = "No odors detected; valves are accessible and functional.", SortOrder = 8 },
                new InspectionItem { Category = "Utilities", Name = "Plumbing System", Icon = "fa-faucet", Criteria = "No active leaks under sinks or around toilets.", SortOrder = 9 },
                new InspectionItem { Category = "Utilities", Name = "Drainage System", Icon = "fa-water", Criteria = "Sinks and tubs drain at a normal rate without backup.", SortOrder = 10 },

                // Safety (Category 3)
                new InspectionItem { Category = "Safety", Name = "Fire/Emergency Exits", Icon = "fa-door-open", Criteria = "Pathways are clear; doors/windows open easily for egress.", SortOrder = 11 },
                new InspectionItem { Category = "Safety", Name = "Smoke Detectors", Icon = "fa-bell", Criteria = "Audio test successful; unit is within its expiration date.", SortOrder = 12 },
                new InspectionItem { Category = "Safety", Name = "Structural Safety", Icon = "fa-hard-hat", Criteria = "Railings are secure; no signs of rot in support beams.", SortOrder = 13 },
                new InspectionItem { Category = "Safety", Name = "Electrical Safety", Icon = "fa-bolt", Criteria = "GFCIs trip/reset correctly in wet areas (kitchen/bath).", SortOrder = 14 },

                // Amenities (Category 4)
                new InspectionItem { Category = "Amenities", Name = "Kitchen Facilities", Icon = "fa-utensils", Criteria = "Appliances are clean and operational; cabinets are secure.", SortOrder = 15 },
                new InspectionItem { Category = "Amenities", Name = "Bathroom Facilities", Icon = "fa-bath", Criteria = "Toilet flushes/fills correctly; exhaust fan is operational.", SortOrder = 16 },
            };

            await context.InspectionItems.AddRangeAsync(inspectionItems);
            await context.SaveChangesAsync();

            Console.WriteLine($"Seeded {inspectionItems.Count} inspection items successfully.");
        }
    }
}
