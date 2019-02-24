using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;

namespace ReadCSV
{
    class Program
    {
        static void Main(string[] args)
        {

            #region SQL Test

            string cs = @"Server=LAPTOP-7GJ721AJ;Database=Market_Prover;userid=LAPTOP-7GJ721AJ/dylan;password=Sci-ssors92";

            MySqlConnection conn = null;

            try
            {
                conn = new MySqlConnection(cs);
                conn.Open();
                Console.WriteLine("MySQL version : {0}", conn.ServerVersion);

            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());

            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }


            #endregion

            #region ReadCSVTest

            var daysStocks = ReadCSV(@"C:\Users\bwill\Documents\Code\ReadCSV\TSX_2017\TSX_20170102.csv");


            //*********************************************************************************************//
            //Trial to see that it reads in the data properly
            //int count = 0;
            //foreach (var price in daysStocks)
            //{
            //    Console.WriteLine(price.Symbol + "|" + price.Date + "|" + price.Open + "|" + price.High + "|" + price.Low + "|" + price.Close + "|" + price.Volume);
            //    count++;
            //    if (count > 30)
            //        break;
            //}

            #endregion

            #region ReadAllFilesTest

            string rootFolder = @"C:\Users\bwill\Documents\Code\ReadCSV\TSX_2017";
            var allFileNames = GetAllCSVs(rootFolder);


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
