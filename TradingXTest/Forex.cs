using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradingXTestPort;
//using TradingXDtos;

namespace TradingXEngine
{
    public class Forex
    {
        /// <summary>
        /// BH
        /// 1. read trade the price action
        /// 2. read XLS document on notes
        /// 2. Use trade the price action HH HL LH LL on USD CHF
        /// 3. Identify trade the price action HH HL LH LL on USD CHF
        /// 4. Identify the change Page 10 from H to L and from L to High
        /// 5. Insert data into Portfolio to P and S
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="name"></param>
        /// <param name="utctime"></param>
        /// 

        public Forex(string name, DateTime utctime)
        {
            writeToLogFile("Constructor entered " + DateTime.Now.ToString("h:mm:ss tt"));
            highs = new ArrayList();

            lows = new ArrayList();

            startLow = false;
            newLowFound = false;
            newHighFound = false;
            tradeStart = false;

            currentVal = 'l';

            quantity = 100;

            prevCriticalHigh = 0;
            //testLow = 0;
            //testHigh = 0;
            prevCriticalLow = 0;


            myTable = new DataTable();

            myTable = new Currency(name, utctime).CurrencyPerName();

            currentPoint = Convert.ToDouble(myTable.Rows[0].ItemArray[2]);

            buyInValue = currentPoint;

            portfolio = new Portfolio("Symbol", "USD/CHF", quantity, Convert.ToDecimal(buyInValue), quantity * Convert.ToDecimal(buyInValue), Convert.ToDecimal(currentPoint), quantity * Convert.ToDecimal(currentPoint), 0, 0, 1, 1, 1, true);
            portfolio.Insert();
            displayAndUpdatePortfolio();

        }

        public double high { get; set; }

        public double nextHigh { get; set; }

        public double nextNextHigh { get; set; }

        public double low { get; set; }

        public double nextLow { get; set; }

        public double nextNextLow { get; set; }

        public double buyInValue { get; set; }

        public double currentPoint { get; set; }

        private char currentVal { get; set; }

        private Portfolio portfolio;

        private ArrayList highs;

        private ArrayList lows;

        private bool startLow;

        private bool newLowFound;

        private bool newHighFound;

        private bool tradeStart;

        private DataTable myTable;

        private int quantity;

        private string hHigh;

        private string hLow;

        private double prevCriticalHigh;

        private double prevCriticalLow;

        private bool buyOrSell;

        public void CalculatePriceAction(double ratio, string name, DateTime utctime)
        {

            myTable = new Currency(name, utctime).CurrencyPerName();
            //myTable.ToString();
            displayAndUpdatePortfolio();

            //Console.WriteLine("NOT TRADING. Waiting for graph points...");

            FindOverallHighsAndLows(Convert.ToDouble(myTable.Rows[2].ItemArray[2]), Convert.ToDouble(myTable.Rows[1].ItemArray[2]), Convert.ToDouble(myTable.Rows[0].ItemArray[2]));
                //Find overall highs and lows
            currentPoint = Convert.ToDouble(myTable.Rows[0].ItemArray[2]);

            if (highs.Count >= 2 && lows.Count >= 2) //only start after 2 highs and lows are found
            {
                if (!tradeStart)
                {
                    prevCriticalLow = Convert.ToDouble(lows[lows.Count - 2]);
                    //buyInValue = prevCriticalLow;
                    portfolio.CostPrice = Convert.ToDecimal(prevCriticalLow);
                    //writeToLogFile("\nBuy in Value(to sell): " + buyInValue);
                    tradeStart = true;
                    displayAndUpdatePortfolio();
                }
                

                if (currentVal == 'l')
                {

                    /*if (newLowFound)
                    {
                        high = Convert.ToDouble(highs[highs.Count - 2]);
                        nextLow = Convert.ToDouble(lows[lows.Count - 2]);
                        nextHigh = Convert.ToDouble(highs[highs.Count - 1]);
                    }*/
                    if (newHighFound)
                    {
                        high = Convert.ToDouble(highs[highs.Count - 2]);
                        nextLow = Convert.ToDouble(lows[lows.Count - 1]);
                        nextHigh = Convert.ToDouble(highs[highs.Count - 1]);


                        writeToLogFile("CurrVal: " + currentVal + " High : " + high + " nextLow: " + nextLow + " nextHigh: " + nextHigh);

                        if (((nextHigh < high) && ((high - prevCriticalLow) > ratio)) || ((high - nextLow) > ratio))
                        {
                            //high is critical
                            if ((high > prevCriticalHigh))
                            {
                                hHigh = "HH";
                                writeToLogFile("                                        Found Critical High " + high + "  " + hHigh);

                                //Console.WriteLine("BUY NOW!!!!!!!!!");
                            }
                            else if (high < prevCriticalHigh)
                            {
                                if (hHigh.Equals("HH") && buyOrSell)
                                {
                                    buyOrSell = false;//becomes a buy, thus false (buy = 0)
                                    writeToLogFile("\n!!!!!!!SELLING Curr L step 6: " + DateTime.Now.ToString("h:mm:ss tt") + "\n");
                                    writeToLogFile("Selling Value: " + currentPoint);
                                    writeToLogFile("                                                Profit Percentage: " + ((currentPoint - buyInValue)*quantity) + " " + DateTime.Now.ToString("h:mm:ss tt") + "\n");
                                    buyInValue = currentPoint;
                                    writeToLogFile("Sell in Value(to buy): " + currentPoint);

                                    HandlePortfolioAndHistory();
                                }
                                hHigh = "LH";
                                writeToLogFile("                                         Found Critical High " + high + "  " + hHigh);
                                

                                //Console.WriteLine("SELL NOW!!!!!!!!!");
                            }
                            

                            prevCriticalHigh = high;
                            currentVal = 'h';
                        }
                    }
                }


                else if (currentVal == 'h')
                {
                    if (newLowFound)
                    {
                        nextLow = Convert.ToDouble(lows[lows.Count - 2]);
                        nextHigh = Convert.ToDouble(highs[highs.Count - 1]);
                        nextNextLow = Convert.ToDouble(lows[lows.Count - 1]);

                        /*else if (newHighFound)
                        {
                            nextLow = Convert.ToDouble(lows[lows.Count - 2]);
                            nextHigh = Convert.ToDouble(highs[highs.Count - 2]);
                            nextNextLow = Convert.ToDouble(lows[lows.Count - 1]);
                        }*/

                        writeToLogFile("CurrVal: " + currentVal + " nextLow: " + nextLow + " nextHigh: " + nextHigh + " nextNextLow: " + nextNextLow);


                        if (((nextNextLow > nextLow) && ((prevCriticalHigh - nextLow) > ratio)) || ((nextHigh - nextLow) > ratio))
                        {
                            if (prevCriticalLow < nextLow) 
                            {
                                if (hLow.Equals("LL") && !buyOrSell)
                                {
                                    writeToLogFile("\n!!!!!!!!!BUYING Curr H step 6: " + DateTime.Now.ToString("h:mm:ss tt") + "\n");
                                    writeToLogFile("Buying Value: " + currentPoint);
                                    writeToLogFile("                                           Profit: " + ((buyInValue - currentPoint)*quantity) + " " + DateTime.Now.ToString("h:mm:ss tt") + "\n");
                                    buyInValue = currentPoint;
                                    writeToLogFile("Buy in Value(to sell): " + currentPoint);
                                    buyOrSell = true;//becomes a sell, thus true (sell = 1)
                                    HandlePortfolioAndHistory();
                                }
                                hLow = "HL";
                                writeToLogFile("                                      Found Critical Low " + nextLow + "  " + hLow);
                            }
                            else if (prevCriticalLow > nextLow)
                            {
                                hLow = "LL";
                                writeToLogFile("                                      Found Critical Low " + nextLow + "  " + hLow);
                            }

                            prevCriticalLow = nextLow;
                            currentVal = 'l';

                        }
                    }
                }
                
            }

        
        }

        private void FindOverallHighsAndLows(double previousPoint, double currPoint, double nextPoint)
        {
            writeToLogFile("FindOverallHighsAndLows() " + previousPoint + " " + currPoint + " " + nextPoint + " " + DateTime.Now.ToString("h:mm:ss tt"));

            if (((previousPoint >= currPoint) && (currPoint < nextPoint)) && (!startLow))
            {
                writeToLogFile("Found new low " + DateTime.Now.ToString("h:mm:ss tt"));
                newLowFound = true;
                newHighFound = false;
                lows.Add(currPoint);
                startLow = true;
            }
            else if ((previousPoint < currPoint) && (currPoint >= nextPoint) && (startLow))  //make sure to start on a low
            {
                writeToLogFile("Found new high  " + DateTime.Now.ToString("h:mm:ss tt"));
                highs.Add(currPoint);
                newLowFound = false;
                newHighFound = true;
                startLow = false;
            }
        }

        private void displayAndUpdatePortfolio()
        {
            //writeToLogFile("displayAndUpdatePortfolio() " + Convert.ToDecimal(myTable.Rows[0].ItemArray[2]) +" "+ DateTime.Now.ToString("h:mm:ss tt"));
            ////////PORTFOLIO PART/////////
            portfolio.CurrentPrice = Convert.ToDecimal(myTable.Rows[0].ItemArray[2]);
            portfolio.CurrentValue = quantity * Convert.ToDecimal(myTable.Rows[0].ItemArray[2]);
            portfolio.ReturnPercentage = ((portfolio.CurrentValue - portfolio.CostValue) / portfolio.CostValue) * 100;

            if (!buyOrSell)  //Sell, make profit negative
                portfolio.ReturnPercentage = portfolio.ReturnPercentage * (-1);

            portfolio.ReturnValue = (portfolio.ReturnPercentage/100) * portfolio.CostValue;
            portfolio.Ps = buyOrSell;
          

            portfolio.Update();
            //print portfolio to console
            Console.Clear(); //clear console first

            Console.WriteLine("{0,-20}:{1:0.00}", "Name", portfolio.Name);
            Console.WriteLine("{0,-20}:{1:0.00}", "Quantity", portfolio.Quantity);
            Console.WriteLine("{0,-20}:{1:0.0000}", "Cost Price", portfolio.CostPrice);
            Console.WriteLine("{0,-20}:{1:0.0000}", "Cost Value", portfolio.CostValue);
            Console.WriteLine("{0,-20}:{1:0.0000}", "Current Price", portfolio.CurrentPrice);
            Console.WriteLine("{0,-20}:{1:0.0000}", "Current Value", portfolio.CurrentValue);
            Console.WriteLine("{0,-20}:{1:0.00}", "Day Percentage", portfolio.DayPercentage);
            Console.WriteLine("{0,-20}:{1:0.00}", "Portfolio Percentage", portfolio.PortfolioPercentage);
            Console.WriteLine("{0,-20}:{1:0.0000}", "Return Value", portfolio.ReturnValue);
            Console.WriteLine("{0,-20}:{1:0.00}", "Return Percentage", portfolio.ReturnPercentage);
            Console.WriteLine("{0,-20}:{1:0.00}", "Purchase/Sell", portfolio.Ps);
        }
        void HandlePortfolioAndHistory()
        {
            writeToLogFile("HandlePortfolioAndHistory() " + DateTime.Now.ToString("h:mm:ss tt"));
            ////////HISTORY PART///////////
            History history = new History(portfolio.Symbol, portfolio.Name, portfolio.Quantity, portfolio.CostPrice, portfolio.CostValue, portfolio.CurrentPrice, portfolio.CurrentValue, portfolio.DayPercentage, portfolio.PortfolioPercentage, portfolio.ReturnValue, portfolio.ReturnPercentage, portfolio.Userid, portfolio.Ps);
            history.Insert();
            ///////////////////////////////

            ////////PORTFOLIO PART/////////
            portfolio.CostPrice = portfolio.CurrentPrice;
            portfolio.CostValue = portfolio.CurrentValue;
            portfolio.ReturnPercentage = ((portfolio.CurrentValue - portfolio.CostValue) / portfolio.CostValue) * 100;

            if (!buyOrSell)  //Sell, make profit negative
                portfolio.ReturnPercentage = portfolio.ReturnPercentage * (-1);

            portfolio.DayPercentage += portfolio.ReturnPercentage;

            portfolio.Ps = buyOrSell;

            portfolio.Update();
            //////////////////////////////
        }

        static private void writeToLogFile(string stringToAppend)
        {
            StreamWriter file = new StreamWriter("C:\\TradingXLogFile.txt", true);
            file.WriteLine(stringToAppend);
            file.Close();
        }
    }
}
