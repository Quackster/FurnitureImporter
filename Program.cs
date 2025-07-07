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

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"FurnitureImporter");
            Console.WriteLine(@"Written by Quackster");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("----------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Initializing FurnitureImporter... please wait.");
            Console.WriteLine();

            if (!TryDatabaseConnection())
            {
                return;
            }

            // if (Directory.Exists("output"))
            // {
            //    Directory.Delete("output", true);
            // }

            if (!Directory.Exists("input"))
            {
                Directory.CreateDirectory("input");
            }

            Directory.CreateDirectory("output");

            using var context = new StorageContext();

            var catalogueItems = context.CatalogueItems.ToList();
            var cataloguePage = context.CataloguePages.ToList();
            var itemDefinitions = context.ItemDefinitions.ToList();

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"Loaded {catalogueItems.Count} catalogue items");
            Console.WriteLine($"Loaded {cataloguePage.Count} catalogue pages");
            Console.WriteLine($"Loaded {itemDefinitions.Count} item definitions");

            var nextCataloguePage = (cataloguePage.OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0) + 1;

            Console.WriteLine("Loading furnidata...");

            var client = new FurnidataClient();

            var itemsFromXml = await client.FetchFurnidataAsync(
                "https://www.habbo.com/gamedata/furnidata_xml/1");

            Console.WriteLine($"Fetched {itemsFromXml.Count} furnidata items from XML endpoint.");

            var inputFurni = new List<string>();
            var outputFurni = new List<string>();

            Console.WriteLine("Loading input furniture...");

            foreach (var file in Directory.GetFiles("input"))
            {
                string className = Path.GetFileNameWithoutExtension(file)
                    .Replace("hh_furni_xx_", "")
                    .Replace("hh_furni_s_xx_", "");

                if (className.Contains("*"))
                    className = className.Split('*')[0];

                if (inputFurni.Contains(className))
                {
                    continue;
                }

                if (!itemsFromXml.Any(x => x.FileName == className))
                {
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Added ");

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(className);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" to queue");

                if (itemsFromXml.Count(x => x.Alias == className) > 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" (found " + itemsFromXml.Count(x => x.Alias == className) + " variants)");
                }

                Console.WriteLine();

                inputFurni.Add(className);
            }

            foreach (var furni in inputFurni)
            {
                foreach (var furniVariant in itemsFromXml.Where(x => x.Alias == furni))
                {
                    var itemDefinition = new ItemDefinition();
                    itemDefinition.Sprite = furniVariant.ClassName;
                    itemDefinition.Name = furniVariant.Name;
                    itemDefinition.Description = furniVariant.Description;
                    itemDefinition.SpriteId = furniVariant.Id;
                    itemDefinition.Colour = furniVariant.PartColors;
                    itemDefinition.Length = furniVariant.XDim;
                    itemDefinition.Width = furniVariant.YDim;
                    itemDefinition.Interactor = "default";
                    itemDefinition.Behaviour = "requires_rights_for_interaction";

                    if (furniVariant.Type == "i")
                    {
                        itemDefinition.Behaviour += ",wall_item";
                    }

                    if (furniVariant.Type == "s")
                    {
                        if (furniVariant.CanLayOn)
                        {
                            itemDefinition.Behaviour += ",can_lay_on_top";
                            itemDefinition.Interactor = "bed";
                            itemDefinition.TopHeight = 1;
                        }
                        else if (furniVariant.CanSitOn)
                        {
                            itemDefinition.Behaviour += ",can_sit_on_top";
                            itemDefinition.Interactor = "chair";
                            itemDefinition.TopHeight = 1;
                        }
                        else if (furniVariant.CanStandOn)
                        {
                            itemDefinition.Behaviour += ",can_stand_on_top";
                            itemDefinition.Interactor = "default";
                            itemDefinition.TopHeight = 0;

                            if (furniVariant.Category == "gate")
                            {
                                itemDefinition.Behaviour += ",solid,gate";
                                itemDefinition.TopHeight = 1;
                                itemDefinition.MaxStatus = "2";
                            }

                            if (furniVariant.Category == "teleport")
                            {
                                itemDefinition.Behaviour += "solid,requires_touching_for_interaction,custom_data_true_false,teleporter";
                                itemDefinition.Interactor += "teleport";
                                itemDefinition.TopHeight = 0;
                                itemDefinition.MaxStatus = "2";
                            }
                        }
                        else
                        {
                            if (furniVariant.Category == "table" ||
                                furniVariant.Category == "divider")
                            {
                                itemDefinition.Behaviour += ",can_stack_on_top";
                                itemDefinition.TopHeight = 1;
                                itemDefinition.MaxStatus = "2";
                            } 
                            else
                            {

                            }
                                itemDefinition.Behaviour += ",solid";
                        }
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

                    outputFurni.Add(itemDefinition.Sprite);

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("Added: ");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(furniVariant.ClassName);

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(", name: " + furniVariant.Name + ", description: " + furniVariant.Description);
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Processed " + inputFurni.Count + " furniture files (and included " + (outputFurni.Count - inputFurni.Count) + " extra furnis)");

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
