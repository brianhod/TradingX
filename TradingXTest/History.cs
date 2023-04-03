using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingXDb;

namespace TradingXTestPort
{
    class History
    {
        public History(string symbol, string name, int quantity, decimal costPrice, decimal costValue, decimal currentPrice, decimal currentValue, decimal dayPercentage, decimal portfolioPercentage, decimal returnValue, decimal returnPercentage, int userid, bool ps)
        {
            Symbol = symbol;
            Name = name;
            Quantity = quantity;
            CostPrice = costPrice;
            CostValue = costValue;
            CurrentPrice = currentPrice;
            CurrentValue = currentValue;
            DayPercentage = dayPercentage;
            PortfolioPercentage = portfolioPercentage;
            ReturnValue = returnValue;
            ReturnPercentage = returnPercentage;
            Userid = userid;
            Ps = ps;
        }

        public int ID { get; set; }

        public string Symbol { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }

        public decimal CostPrice { get; set; }

        public decimal CostValue { get; set; }

        public decimal CurrentPrice { get; set; }

        public decimal CurrentValue { get; set; }

        public decimal DayPercentage { get; set; }

        public decimal PortfolioPercentage { get; set; }

        public decimal ReturnValue { get; set; }

        public decimal ReturnPercentage { get; set; }

        public int Userid { get; set; }

        public bool Ps { get; set; }

        public bool Insert()
        {
            var dataLayer = new DataLayer("Data Source=WIN-2EI5RJNONFE;Initial Catalog=TradingX;User Id=sa;Password=;");

            var dictionary = new Dictionary<string, object>
            {
                {"@Symbol", Symbol},
                {"@Name", Name},  
                {"@Quantity", Quantity},
                {"@CostPrice", CostPrice},
                {"@CostValue", CostValue},
                {"@CurrentPrice", CurrentPrice},
                {"@CurrentValue", CurrentValue },
                {"@DayPercentage", DayPercentage },
                {"@PortfolioPercentage", PortfolioPercentage },
                {"@ReturnValue", ReturnValue },
                {"@ReturnPercentage", ReturnPercentage},
                {"@Userid ", Userid },
                {"@PS ", Ps }
            };

            return dataLayer.ExecuteNonQueryStoredProcedure("spi_History", dictionary);
        }
    }
}
