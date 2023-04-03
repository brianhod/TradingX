using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TradingXDb;

namespace TradingXTestPort
{
    public class Profits
    {
        public Profits()
        {
        }

        public bool Insert(decimal buyInValue, decimal currentValue, decimal profit, decimal percentageProfit)
        {
            var dataLayer = new DataLayer("Data Source=WIN-2EI5RJNONFE;Initial Catalog=TradingX;User Id=sa;Password=;");

            var dictionary = new Dictionary<string, object>
            {
                {"@BuyInValue", buyInValue},
                {"@currentValue", currentValue},  
                {"@Profit", profit},
                {"@PercentageProfit", percentageProfit},
            };

            return dataLayer.ExecuteNonQueryStoredProcedure("spi_Profit", dictionary);
        }
    }
}
