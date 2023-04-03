using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingXDb;
using System.Globalization;

namespace TradingXTestPort
{
    public class Currency
    {
        public Currency(int id, string name, string price, string symbol, string ts, string type, DateTime utctime, int volume, string action)
        {
            ID = id;
            Name = name;
            Price = decimal.Parse(price, CultureInfo.InvariantCulture);
            Symbol = symbol;
            Ts = ts;
            Type = type;
            Utctime = utctime;
            Volume = volume;
            Action = action;
        }

        public Currency(string name, DateTime utctime)
        {
            Name = name;
            Utctime = utctime;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Symbol { get; set; }

        public string Ts { get; set; }

        public string Type { get; set; }

        public DateTime Utctime { get; set; }

        public int Volume { get; set; }

        public string Action { get; set; }

        public bool Insert()
        {
            if (Utctime <= LatestUtctime()) return false;

            var dataLayer = new DataLayer("Data Source=WIN-2EI5RJNONFE;Initial Catalog=TradingX;User Id=sa;Password=;");

            var dictionary = new Dictionary<string, object>
            {
                {"@name", Name},
                {"@price", Price},  
                {"@symbol", Symbol},
                {"@ts", Ts},
                {"@type ", Type},
                {"@Utctime ", Utctime},
                {"@volume ", Volume },
                {"@Action ", Action }
            };

            return dataLayer.ExecuteNonQueryStoredProcedure("spi_Currency", dictionary);
        }

        public bool Update()
        {
            var dataLayer = new DataLayer("Data Source=WIN-2EI5RJNONFE;Initial Catalog=TradingX;User Id=sa;Password=;");
            var dictionary = new Dictionary<string, object>
            {
                {"@Name", Name},
                {"@price", Price},  
                {"@utctime", Utctime},
                {"@Action", Action}
            };

            return dataLayer.ExecuteNonQueryStoredProcedure("spu_Currency", dictionary);
        }

        public DateTime LatestUtctime()
        {
            var dataLayer = new DataLayer("Data Source=WIN-2EI5RJNONFE;Initial Catalog=TradingX;User Id=sa;Password=;");
            var dictionary = new Dictionary<string, object>
            {
                {"@name", Name}
            };
            var myTable = dataLayer.GetDataTable("sps_LatestSharePrice", dictionary);
            try
            {
                return Convert.ToDateTime(myTable.Rows[0].ItemArray[6].ToString());
            }
            catch
            {

                return DateTime.Now;
            }

        }

        public DataTable CurrencyPerName()
        {
            var dataLayer = new DataLayer("Data Source=WIN-2EI5RJNONFE;Initial Catalog=TradingX;User Id=sa;Password=;");
            var dictionary = new Dictionary<string, object>
            {
                {"@Name", Name},
                {"@startdate", Utctime}
            };
            return dataLayer.GetDataTable("sps_CurrencyPerName", dictionary);
        }

        protected bool Equals(Currency other)
        {
            return ID == other.ID && string.Equals(Name, other.Name) && Price == other.Price && string.Equals(Symbol, other.Symbol) && string.Equals(Ts, other.Ts) && string.Equals(Type, other.Type) && Utctime.Equals(other.Utctime) && Volume == other.Volume;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Currency)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ID;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Price.GetHashCode();
                hashCode = (hashCode * 397) ^ (Symbol != null ? Symbol.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Ts != null ? Ts.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Utctime.GetHashCode();
                hashCode = (hashCode * 397) ^ Volume;
                return hashCode;
            }
        }

        private sealed class currencyEqualityComparer : IEqualityComparer<Currency>
        {
            public bool Equals(Currency x, Currency y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.ID == y.ID && string.Equals(x.Name, y.Name) && x.Price == y.Price && string.Equals(x.Symbol, y.Symbol) && string.Equals(x.Ts, y.Ts) && string.Equals(x.Type, y.Type) && x.Utctime.Equals(y.Utctime) && x.Volume == y.Volume;
            }

            public int GetHashCode(Currency obj)
            {
                unchecked
                {
                    int hashCode = obj.ID;
                    hashCode = (hashCode * 397) ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Price.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.Symbol != null ? obj.Symbol.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Ts != null ? obj.Ts.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Type != null ? obj.Type.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Utctime.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Volume;
                    return hashCode;
                }
            }
        }

        private static readonly IEqualityComparer<Currency> CurrencyComparerInstance = new currencyEqualityComparer();

        public static IEqualityComparer<Currency> CurrencyComparer
        {
            get { return CurrencyComparerInstance; }
        }
    }
}
