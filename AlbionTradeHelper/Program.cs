using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AlbionTradeHelper
{
    class Program
    {
        public static List<Armor> armorItems = new List<Armor>();
        public static List<Magic> magicItems = new List<Magic>();
        public static List<Ranged> rangedItems = new List<Ranged>();
        public static List<Melee> meleeItems = new List<Melee>();
        public static List<Mount> mountItems = new List<Mount>();
        public static List<Trade> armorTrades = new List<Trade>();
        public static List<Trade> magicTrades = new List<Trade>();
        public static List<Trade> rangedTrades = new List<Trade>();
        public static List<Trade> meleeTrades = new List<Trade>();
        public static List<Trade> mountTrades = new List<Trade>();
        public static bool PercentageOrder = false;

        public static ChromeDriver StartDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            var driver = new ChromeDriver(@"C:\Users\Kuki\source\repos\AlbionTradeHelper\AlbionTradeHelper\bin\Debug\netcoreapp2.1");
            driver.Manage().Window.Maximize();
            return driver;
        }

        public static void GoToUrl(ChromeDriver driver, string url)
        {
            driver.Navigate().GoToUrl(url);
        }

        private static bool IsElementPresent(By by, IWebElement item)
        {
            try
            {
                item.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static List<Item> GetItemPrices(ChromeDriver driver, int tier, int enchantment)
        {
            var items = driver.FindElementsByClassName("card-box");
            List<Item> itemList = new List<Item>();
            var skip = 0;
            foreach (var item in items)
            {
                if (skip > 1)
                {
                    var itemSplit = item.Text.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );

                    var count = 0;

                    if (IsElementPresent(By.CssSelector("span[title='Caerleon']"), item) && IsElementPresent(By.CssSelector("span[title='Thetford']"), item))
                    {
                        var caerleon = item.FindElement(By.CssSelector("span[title='Caerleon']")).GetAttribute("textContent");
                        var thetford = item.FindElement(By.CssSelector("span[title='Thetford']")).GetAttribute("textContent");
                        var caerleonPrice = "";
                        var thetfordPrice = "";
                        if (caerleon.Contains(","))
                        {
                            caerleonPrice = caerleon.Replace(",", "");
                        }
                        else
                        {
                            caerleonPrice = caerleon;
                        }

                        if (thetford.Contains(","))
                        {
                            thetfordPrice = thetford.Replace(",", "");
                        }
                        else
                        {
                            thetfordPrice = thetford;
                        }

                        itemList.Add(new Item
                        {
                            Name = itemSplit[0],
                            Tier = tier,
                            Enchantment = enchantment,
                            CaerleonPrice = caerleonPrice,
                            ThetfordPrice = thetfordPrice
                        });
                    }

                }
                skip++;
            }

            return itemList;
        }

        public static List<Item> GetItemPricesBlackMarket(ChromeDriver driver, int tier, int enchantment)
        {
            var items = driver.FindElementsByClassName("card-box");
            List<Item> itemList = new List<Item>();
            var skip = 0;
            foreach (var item in items)
            {
                if (skip > 1)
                {
                    var itemSplit = item.Text.Split(
                        new[] { "\r\n", "\r", "\n" },
                        StringSplitOptions.None
                    );

                    var count = 0;

                    if (IsElementPresent(By.CssSelector("span[title='Caerleon']"), item) && IsElementPresent(By.CssSelector("span[title='Black Market']"), item))
                    {
                        var caerleon = item.FindElement(By.CssSelector("span[title='Caerleon']")).GetAttribute("textContent");
                        var blackMarket = item.FindElement(By.CssSelector("span[title='Black Market']")).GetAttribute("textContent");
                        var caerleonPrice = "";
                        var blackMarketPrice = "";
                        if (caerleon.Contains(","))
                        {
                            caerleonPrice = caerleon.Replace(",", "");
                        }
                        else
                        {
                            caerleonPrice = caerleon;
                        }

                        if (blackMarket.Contains(","))
                        {
                            blackMarketPrice = blackMarket.Replace(",", "");
                        }
                        else
                        {
                            blackMarketPrice = blackMarket;
                        }

                        itemList.Add(new Item
                        {
                            Name = itemSplit[0],
                            Tier = tier,
                            Enchantment = enchantment,
                            CaerleonPrice = caerleonPrice,
                            BlackMarketPrice = blackMarketPrice
                        });
                    }

                }
                skip++;
            }

            return itemList;
        }

        public static List<Trade> GetTrades(List<Item> itemList)
        {
            List<Trade> tradeList = new List<Trade>();
            foreach (var item in itemList)
            {
                var profit = 0;
                double profitPercentage = 0;
                if (Int32.Parse(item.CaerleonPrice) > Int32.Parse(item.ThetfordPrice))
                {
                    profit = Int32.Parse(item.CaerleonPrice) - Int32.Parse(item.ThetfordPrice);
                    profitPercentage = (Convert.ToDouble(profit * 100)) / Double.Parse(item.ThetfordPrice) ;
                    profitPercentage = Math.Round(profitPercentage, 2);
                    tradeList.Add(new Trade
                    {
                        ItemName = item.Name,
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        FirstCity = "Thetford",
                        SecondCity = "Caerleon",
                        FirstCityPrice = item.ThetfordPrice,
                        SecondCityPrice = item.CaerleonPrice,
                        Profit = profit.ToString(),
                        ProfitPercentage = profitPercentage.ToString()
                    });
                }
                else if (Int32.Parse(item.CaerleonPrice) < Int32.Parse(item.ThetfordPrice))
                {
                    profit = Int32.Parse(item.ThetfordPrice) - Int32.Parse(item.CaerleonPrice);
                    profitPercentage = (Convert.ToDouble(profit * 100)) / Double.Parse(item.CaerleonPrice);
                    profitPercentage = Math.Round(profitPercentage, 2);
                    tradeList.Add(new Trade
                    {
                        ItemName = item.Name,
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        FirstCity = "Caerleon",
                        SecondCity = "Thetford",
                        FirstCityPrice = item.CaerleonPrice,
                        SecondCityPrice = item.ThetfordPrice,
                        Profit = profit.ToString(),
                        ProfitPercentage = profitPercentage.ToString()
                    });
                }
                else
                {
                    profit = 0;
                }
               
            }

            return tradeList;
        }

        public static List<Trade> GetTradesBlackMarket(List<Item> itemList)
        {
            List<Trade> tradeList = new List<Trade>();
            foreach (var item in itemList)
            {
                var profit = 0;
                double profitPercentage = 0;
                if (Int32.Parse(item.CaerleonPrice) > Int32.Parse(item.BlackMarketPrice))
                {
                    profit = Int32.Parse(item.CaerleonPrice) - Int32.Parse(item.BlackMarketPrice);
                    profitPercentage = (Convert.ToDouble(profit * 100)) / Double.Parse(item.BlackMarketPrice);
                    profitPercentage = Math.Round(profitPercentage, 2);
                    tradeList.Add(new Trade
                    {
                        ItemName = item.Name,
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        FirstCity = "BlackMarket",
                        SecondCity = "Caerleon",
                        FirstCityPrice = item.BlackMarketPrice,
                        SecondCityPrice = item.CaerleonPrice,
                        Profit = profit.ToString(),
                        ProfitPercentage = profitPercentage.ToString()
                    });
                }
                else if (Int32.Parse(item.CaerleonPrice) < Int32.Parse(item.BlackMarketPrice))
                {
                    profit = Int32.Parse(item.BlackMarketPrice) - Int32.Parse(item.CaerleonPrice);
                    profitPercentage = (Convert.ToDouble(profit * 100)) / Double.Parse(item.CaerleonPrice);
                    profitPercentage = Math.Round(profitPercentage, 2);
                    tradeList.Add(new Trade
                    {
                        ItemName = item.Name,
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        FirstCity = "Caerleon",
                        SecondCity = "BlackMarket",
                        FirstCityPrice = item.CaerleonPrice,
                        SecondCityPrice = item.BlackMarketPrice,
                        Profit = profit.ToString(),
                        ProfitPercentage = profitPercentage.ToString()
                    });
                }
                else
                {
                    profit = 0;
                }

            }

            return tradeList;
        }

        public static List<Trade> OrderListByProfit(List<Trade> tradeList)
        {
            List<Trade> finalTrades = tradeList.OrderBy(a => Int32.Parse(a.Profit)).Reverse().ToList();
            return finalTrades;
        }
        public static List<Trade> OrderListByProfitPercentage(List<Trade> tradeList)
        {
            List<Trade> finalTrades = tradeList.OrderBy(a => Double.Parse(a.ProfitPercentage)).Reverse().ToList();
            return finalTrades;
        }

        public static void WriteToFile(List<Trade> finalTrades, string fileName)
        {
            foreach (var trade in finalTrades)
            {
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(@"C:\Users\Kuki\Desktop\AlbionHelper\" + fileName + ".txt", true))
                {
                    file.WriteLine("Item: " + trade.ItemName);
                    file.WriteLine("Tier: " + trade.Tier);
                    file.WriteLine("Enchantment: " + trade.Enchantment);
                    file.WriteLine(trade.FirstCity + ": " + trade.FirstCityPrice);
                    file.WriteLine(trade.SecondCity + ": " + trade.SecondCityPrice);
                    file.WriteLine("Profit: " + trade.Profit);
                    file.WriteLine("Profit percentage: " + trade.ProfitPercentage + "%");
                    file.WriteLine(Environment.NewLine);
                }
            }
        }

        public static void EndDriver(ChromeDriver driver)
        {
            Console.WriteLine("Finished");
            driver.Quit();
        }

        public static void GetCategories()
        {
            List<Categories> items = new List<Categories>();
            using (StreamReader r = new StreamReader(@"C:\Users\Kuki\source\repos\AlbionTradeHelper\AlbionTradeHelper\Categories.json"))
            {
                string json = r.ReadToEnd();
                JsonConvert.DeserializeObject<List<Categories>>(json);
                //Console.WriteLine(json);
                items = JsonConvert.DeserializeObject<List<Categories>>(json);
            }

            foreach (var item in items)
            {
                if (item.Type == "armor")
                {
                    armorItems.Add(new Armor
                    {
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        Url = item.Url
                    });
                }
                else if (item.Type == "ranged")
                {
                    rangedItems.Add(new Ranged
                    {
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        Url = item.Url
                    });
                }
                else if (item.Type == "melee")
                {
                    meleeItems.Add(new Melee
                    {
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        Url = item.Url
                    });
                }
                else if (item.Type == "magic")
                {
                    magicItems.Add(new Magic
                    {
                        Tier = item.Tier,
                        Enchantment = item.Enchantment,
                        Url = item.Url
                    });
                }
                else if (item.Type == "mount")
                {
                    mountItems.Add(new Mount
                    {
                        Tier = item.Tier,
                        Url = item.Url
                    });
                }
            }
        }

        private static void ExecuteArmor(ChromeDriver driver)
        {
            int count = 1;

            foreach (var armor in armorItems)
            {
                GoToUrl(driver, armor.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPrices(driver, armor.Tier, armor.Enchantment);
                var tradeList = GetTrades(itemList);
                armorTrades.AddRange(tradeList);
                Console.WriteLine("Armor " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalArmorTrades = OrderListByProfit(armorTrades);
                WriteToFile(finalArmorTrades, "armor");
            }
            else
            {
                List<Trade> finalArmorTrades = OrderListByProfit(armorTrades);
                WriteToFile(finalArmorTrades, "armor");
            }
        }
        private static void ExecuteRanged(ChromeDriver driver)
        {
            int count = 1;
            foreach (var ranged in rangedItems)
            {
                GoToUrl(driver, ranged.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPrices(driver, ranged.Tier, ranged.Enchantment);
                var tradeList = GetTrades(itemList);
                rangedTrades.AddRange(tradeList);
                Console.WriteLine("Ranged " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalRangedTrades = OrderListByProfitPercentage(rangedTrades);
                WriteToFile(finalRangedTrades, "ranged");
            }
            else
            {
                List<Trade> finalRangedTrades = OrderListByProfit(rangedTrades);
                WriteToFile(finalRangedTrades, "ranged");
            }
        }
        private static void ExecuteMagic(ChromeDriver driver)
        {
            int count = 1;
            foreach (var magic in magicItems)
            {
                GoToUrl(driver, magic.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPrices(driver, magic.Tier, magic.Enchantment);
                var tradeList = GetTrades(itemList);
                magicTrades.AddRange(tradeList);
                Console.WriteLine("Magic " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalMagicTrades = OrderListByProfitPercentage(magicTrades);
                WriteToFile(finalMagicTrades, "magic");
            }
            else
            {
                List<Trade> finalMagicTrades = OrderListByProfit(magicTrades);
                WriteToFile(finalMagicTrades, "magic");
            }
        }
        private static void ExecuteMelee(ChromeDriver driver)
        {
            int count = 1;
            foreach (var melee in meleeItems)
            {
                GoToUrl(driver, melee.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPrices(driver, melee.Tier, melee.Enchantment);
                var tradeList = GetTrades(itemList);
                meleeTrades.AddRange(tradeList);
                Console.WriteLine("Melee " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalMeleeTrades = OrderListByProfitPercentage(meleeTrades);
                WriteToFile(finalMeleeTrades, "melee");
            }
            else
            {
                List<Trade> finalMeleeTrades = OrderListByProfit(meleeTrades);
                WriteToFile(finalMeleeTrades, "melee");
            }
            
        }
        private static void ExecuteMount(ChromeDriver driver)
        {
            int count = 1;
            foreach (var mount in mountItems)
            {
                GoToUrl(driver, mount.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPrices(driver, mount.Tier, 0);
                var tradeList = GetTrades(itemList);
                mountTrades.AddRange(tradeList);
                Console.WriteLine("Mount " + count + " done!");
                count++;
            }

            if (PercentageOrder)
            {
                List<Trade> finalMountTrades = OrderListByProfitPercentage(mountTrades);
                WriteToFile(finalMountTrades, "mount");
            }
            else
            {
                List<Trade> finalMountTrades = OrderListByProfit(mountTrades);
                WriteToFile(finalMountTrades, "mount");
            }
        }

        private static void ExecuteArmorBM(ChromeDriver driver)
        {
            int count = 1;

            foreach (var armor in armorItems)
            {
                GoToUrl(driver, armor.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPricesBlackMarket(driver, armor.Tier, armor.Enchantment);
                var tradeList = GetTradesBlackMarket(itemList);
                armorTrades.AddRange(tradeList);
                Console.WriteLine("Armor " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalArmorTrades = OrderListByProfit(armorTrades);
                WriteToFile(finalArmorTrades, "armor");
            }
            else
            {
                List<Trade> finalArmorTrades = OrderListByProfit(armorTrades);
                WriteToFile(finalArmorTrades, "armor");
            }
        }
        private static void ExecuteRangedBM(ChromeDriver driver)
        {
            int count = 1;
            foreach (var ranged in rangedItems)
            {
                GoToUrl(driver, ranged.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPricesBlackMarket(driver, ranged.Tier, ranged.Enchantment);
                var tradeList = GetTradesBlackMarket(itemList);
                rangedTrades.AddRange(tradeList);
                Console.WriteLine("Ranged " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalRangedTrades = OrderListByProfitPercentage(rangedTrades);
                WriteToFile(finalRangedTrades, "ranged");
            }
            else
            {
                List<Trade> finalRangedTrades = OrderListByProfit(rangedTrades);
                WriteToFile(finalRangedTrades, "ranged");
            }
        }
        private static void ExecuteMagicBM(ChromeDriver driver)
        {
            int count = 1;
            foreach (var magic in magicItems)
            {
                GoToUrl(driver, magic.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPricesBlackMarket(driver, magic.Tier, magic.Enchantment);
                var tradeList = GetTradesBlackMarket(itemList);
                magicTrades.AddRange(tradeList);
                Console.WriteLine("Magic " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalMagicTrades = OrderListByProfitPercentage(magicTrades);
                WriteToFile(finalMagicTrades, "magic");
            }
            else
            {
                List<Trade> finalMagicTrades = OrderListByProfit(magicTrades);
                WriteToFile(finalMagicTrades, "magic");
            }
        }
        private static void ExecuteMeleeBM(ChromeDriver driver)
        {
            int count = 1;
            foreach (var melee in meleeItems)
            {
                GoToUrl(driver, melee.Url);
                Thread.Sleep(6000);
                var itemList = GetItemPrices(driver, melee.Tier, melee.Enchantment);
                var tradeList = GetTradesBlackMarket(itemList);
                meleeTrades.AddRange(tradeList);
                Console.WriteLine("Melee " + count + " done!");
                count++;
            }
            if (PercentageOrder)
            {
                List<Trade> finalMeleeTrades = OrderListByProfitPercentage(meleeTrades);
                WriteToFile(finalMeleeTrades, "melee");
            }
            else
            {
                List<Trade> finalMeleeTrades = OrderListByProfit(meleeTrades);
                WriteToFile(finalMeleeTrades, "melee");
            }

        }

        public static void ExecuteItems(ChromeDriver driver)
        {
            //ExecuteArmor(driver);
            //ExecuteRanged(driver);
            //ExecuteMagic(driver);
            //ExecuteMelee(driver);
            ExecuteMount(driver);
        }

        public static void ExecuteItemsBM(ChromeDriver driver)
        {
            ExecuteArmorBM(driver);
            ExecuteRangedBM(driver);
            ExecuteMagicBM(driver);
            ExecuteMeleeBM(driver);
            //ExecuteMount(driver);
        }

        static void Main(string[] args)
        {
            GetCategories();

            var menu = new Menu();
            PercentageOrder = menu.StartMenu();

            var driver = StartDriver();
            
            ExecuteItems(driver);

            EndDriver(driver);
          
            Console.ReadLine();
        }
    }
}
