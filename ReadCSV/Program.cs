using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace ReadCSV
{
    class Program
    {
        static void Main(string[] args)
        {

            #region Putting it all together

            string rootFolder = @"C:\Users\dylan\Documents\Stocks\2017";
            var allFileNames = GetAllCSVs(rootFolder);
            int totalFiles = allFileNames.Count();
            int fileCount = 1;
            foreach(var file in allFileNames)
            {
                Console.WriteLine(String.Format("Working in the file {0} of {1}", fileCount, totalFiles));
                var allStocks = ReadCSV(file);
                int daysStockCount = allStocks.Count();
                int stockCount = 1;
                foreach(StockDay stock in allStocks)
                {
                    Console.WriteLine(String.Format("/t/tWorking on stock {0} of {1} for the file", stockCount, daysStockCount));
                    InputDay(stock);
                    stockCount++;
                }
                fileCount++;
            }

            #endregion


            #region SQL Test

            //************************************************************************************************************************//
            //A Microsoft SQL connection for trials on my home computers server

            //try
            //{

            //    StockDay day = new StockDay
            //    {
            //        Symbol = "test1",
            //        Open = 20,
            //        Close = 20,
            //        High = 20,
            //        Low = 20,
            //        Volume = 20,
            //        Date = DateTime.Now.Date
            //    };


            //    InputDay(day);

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Error: {0}", ex.ToString());

            //}



            #region MySQL Region (All Commented Out)
            //************************************************************************************************************************//
            //A MySQL connection 
            //string cs = @"Server=LAPTOP-7GJ721AJ;Database=Market_Prover;Uid=Dwillick;Pwd=Sci-ssors92;";
            //MySqlConnection conn = null;

            //try
            //{
            //    conn = new MySqlConnection(cs);
            //    conn.Open();
            //    Console.WriteLine("MySQL version : {0}", conn.ServerVersion);

            //}
            //catch (MySqlException ex)
            //{
            //    Console.WriteLine("Error: {0}", ex.ToString());

            //}
            //finally
            //{
            //    if (conn != null)
            //    {
            //        conn.Close();
            //    }
            //}
            #endregion

            #endregion
            #region ReadAllFilesTest

            //string rootFolder = @"C:\Users\dylan\Documents\Stocks\2017";
            //var allFileNames = GetAllCSVs(rootFolder);


            //**************************************************************************************************//
            //Double check we read the directory
            //foreach(var file in allFileNames)
            //{
            //    Console.WriteLine(file);
            //}
            #endregion
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();

        }

        //Create a method that creates one Stock (checks unique name**) given one StockDay item
        //Returns the unique ID of the stock in the DB
        private static int InputDay(StockDay dayIn)
        {
            SQLMethods connection = new SQLMethods();
            int SymbolId = -1;
            try
            {
                SymbolId = connection.GetSymbolId(dayIn.Symbol);
                if(SymbolId == 0)
                {
                    connection.CreateStock(dayIn.Symbol);
                    SymbolId = connection.GetSymbolId(dayIn.Symbol);
                }

                connection.CreateDay(SymbolId, dayIn);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }



            return SymbolId;
        }

        
        //Receivesa a path to the CSV file to be parsed
        //Returns the CSV in a IEnumerable list of type StockDay
        //Each stock day is set to be a individual row in the DB.

        private static IEnumerable<StockDay> ReadCSV(string CSVPath)
        {
            var historicalData = File.ReadAllLines(CSVPath);


            try
            {
                IEnumerable<StockDay> daysStocks = from historyPrice in historicalData.Skip(1)
                                                   let data = historyPrice.Split(',')
                                                   select new StockDay
                                                   {
                                                       Symbol = data[0],
                                                       Date = Convert.ToDateTime(data[1]),
                                                       Open = Convert.ToDouble(data[2]),
                                                       High = Convert.ToDouble(data[3]),
                                                       Low = Convert.ToDouble(data[4]),
                                                       Close = Convert.ToDouble(data[5]),
                                                       Volume = Convert.ToInt32(data[6])
                                                   };

                return daysStocks;


            }
            catch (Exception e)
            {
                Console.Write("Excel file format earror.");
                Console.WriteLine(e.Message);
                return null;
            }

        }

        //Takes the years directory as input
        //Reads through the folder and for each file path to be parsed
        private static string[] GetAllCSVs(string rootDirectory)
        {
            string[] filePaths = Directory.GetFiles(rootDirectory, "*.csv", SearchOption.TopDirectoryOnly);
            return filePaths;
        }

        //Need to be able to write to a Database (Can start with csv for practice purposes)


    }
}
