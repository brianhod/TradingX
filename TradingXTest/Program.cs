using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using TradingXDb;
using TradingXTestPort;
//using TradingXDtos;
//using TradingXCalc;
using TradingXEngine;

namespace TradingX
{
    class Program
    {
        static void Main()
        {
            Forex forex = new Forex("USD/CHF", Convert.ToDateTime("2022-09-23 10:17:00.000"));

            while (true)
            {
                try
                {
                    const string uri = "http://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote";
                    var doc = XDocument.Load(uri);
                    foreach (var currency in from XElement node in (IEnumerable) doc.XPathEvaluate("//resource")
                        select new Currency(
                            0,
                            node.XPathSelectElement(@"field[@name='name']").Value,
                            node.XPathSelectElement(@"field[@name='price']").Value,
                            node.XPathSelectElement(@"field[@name='symbol']").Value,
                            node.XPathSelectElement(@"field[@name='ts']").Value,
                            node.XPathSelectElement(@"field[@name='type']").Value,
                            Convert.ToDateTime(node.XPathSelectElement(@"field[@name='utctime']").Value),
                            Convert.ToInt32(node.XPathSelectElement(@"field[@name='volume']").Value),
                            string.Empty
                            ))
                    {
                        currency.Insert();
                    }

                    forex.CalculatePriceAction(0.0009, "USD/CHF", Convert.ToDateTime("2015-09-23 10:17:00.000"));
                    Thread.Sleep(10000);
                }
                catch
                {
                    Thread.Sleep(5000);
                }
            }

            //var calc = new RnD.Calc(5, "RnD");
            //Console.WriteLine(calc.X + calc.Y);
            //Console.WriteLine(calc.Z);
            //Console.WriteLine(calc.C.Invoke(200.10));
            //Console.Read();

            
        }
    }
}