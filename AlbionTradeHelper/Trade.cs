using System;
using System.Collections.Generic;
using System.Text;

namespace AlbionTradeHelper
{
    class Trade
    {
        public string ItemName { get; set; }
        public int Tier { get; set; }
        public int Enchantment { get; set; }
        public string FirstCity { get; set; }
        public string SecondCity { get; set; }
        public string FirstCityPrice { get; set; }
        public string SecondCityPrice { get; set; }
        public string Profit { get; set; }
        public string ProfitPercentage { get; set; }
    }
}
