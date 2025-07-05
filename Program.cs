using FurnidataParser;
using Helios.Storage;
using Microsoft.Extensions.Configuration;
using System;

namespace FurnitureImporter
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        static async Task Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            if (!TryDatabaseConnection())
            {
                return;
            }

            using var context = new StorageContext();

            var catalogueItems = context.CatalogueItems.ToList();
            var cataloguePage = context.CataloguePages.ToList();
            var itemDefinitions = context.ItemDefinitions.ToList();

            Console.WriteLine($"Loaded {catalogueItems.Count} catalogue items");
            Console.WriteLine($"Loaded {cataloguePage.Count} catalogue pages");
            Console.WriteLine($"Loaded {itemDefinitions.Count} item definitions");

            var nextCataloguePage = (cataloguePage.OrderByDescending( x => x.Id ).FirstOrDefault()?.Id ?? 0) + 1;

            Console.WriteLine("Loading furnidata...");

            var client = new FurnidataClient();

            var itemsFromXml = await client.FetchFurnidataAsync(
                "https://www.habbo.com/gamedata/furnidata_xml/1");

            Console.WriteLine($"Fetched {itemsFromXml.Count} furnidata items from XML endpoint.");

            /*
            foreach (var file in Directory.GetFiles("swf_files"))
            {
                string className = Path.GetFileNameWithoutExtension(file).Replace("hh_furni_xx_", "");

                FurniItem? furniEntry = itemsFromXml.FirstOrDefault(x => x.ClassName == className);

                if (furniEntry is FurniItem furni)
                {
                    var itemDefinition = new ItemDefinition();
                    itemDefinition.Sprite = furni.ClassName;
                    itemDefinition.Name = furni.Name;
                    itemDefinition.Description = furni.Description;
                    itemDefinition.SpriteId = furni.Id;
                    itemDefinition.Colour = furni.PartColors;
                    itemDefinition.Length = furni.XDim;
                    itemDefinition.Width = furni.YDim;
                    itemDefinition.Interactor = "default";
                    itemDefinition.Behaviour = "requires_rights_for_interaction";

                    if (furni.Type == "i")
                    {
                        itemDefinition.Behaviour += ",wall_item";
                    }

                    if (furni.Type == "s")
                    {
                        itemDefinition.Behaviour += ",solid";
                    }

                    context.Add(itemDefinition);
                    context.SaveChanges();

                    var catalogueItem = new CatalogueItem();
                    catalogueItem.SaleCode = itemDefinition.Sprite;
                    catalogueItem.Name = itemDefinition.Name;
                    catalogueItem.Description = itemDefinition.Description;
                    catalogueItem.Amount = 1;
                    catalogueItem.Price = 3;
                    catalogueItem.DefinitionId = itemDefinition.Id;
                    catalogueItem.PageId = "" + nextCataloguePage;


                    context.Add(catalogueItem);
                    context.SaveChanges();

                }
            }*/


            /*
            foreach (var itemDef in itemDefinitions.Where(x => x.Id >= 1417))
            {
                FurniItem? furnidataEntry = itemsFromXml.FirstOrDefault(x => x.ClassName == itemDef.Sprite);

                if (furnidataEntry is not FurniItem)
                {
                    continue;
                }


                itemDef.Colour = furnidataEntry.Value.PartColors;
                itemDef.Length = furnidataEntry.Value.YDim;
                itemDef.Width = furnidataEntry.Value.XDim;
                itemDef.Name = furnidataEntry.Value.Name;
                itemDef.Description = furnidataEntry.Value.Description;

                context.Update(itemDef);

                var cataItem = catalogueItems.Where(x => x.DefinitionId == itemDef.Id).FirstOrDefault();

                if (cataItem is not null)
                {
                    cataItem.Name =  furnidataEntry.Value.Name;
                    cataItem.Description = furnidataEntry.Value.Description;

                    context.Update(cataItem);
                }

                Console.WriteLine("Item def: " + itemDef.Sprite);
            }*/

            context.SaveChanges();
        }

        private static bool TryDatabaseConnection()
        {
            try
            {
                Console.WriteLine("Attempting to connect to MySQL database");

                using var context = new StorageContext();
                context.Database.EnsureCreated();

                Console.WriteLine("Connection to database is successful!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred attempting to connect to the database: " + ex.ToString());
                return false;
            }

            return true;
        }
    }
}