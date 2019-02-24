using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace ReadCSV
{
    public class SQLMethods
    {
        SqlConnection conn;


        public SQLMethods()
        {
            conn = new SqlConnection
            {
                ConnectionString = "Data Source=LAPTOP-7GJ721AJ;Initial Catalog=Market_Prover; User id=Dwillick;Password=Sci-ssors92;"
            };
        }




        //Checks to see if symbol exists in DB, if not it returns 0
        public int GetSymbolId(string symbolIn)
        {
            conn.Open();

            int id = 0;

            SqlCommand getCommand = new SqlCommand(String.Format("select * from Stock where Name = '{0}'", symbolIn), conn);
            SqlDataReader myReader = getCommand.ExecuteReader();

            if (myReader.HasRows)
            {
                //Get the ID of the stock that is in the DB
                int IDOrdinal = myReader.GetOrdinal("StockID");
                while (myReader.Read())
                {
                    id = myReader.GetInt32(IDOrdinal);
                }
                myReader.Close();
            }
            conn.Close();
            return id;

        }


        //Creates a stock in the DB. 
        //Does NOT return the ID of the new stock.
        public void CreateStock(string symbolIn)
        {
            conn.Open();
            try
            {
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO Stock (name) VALUES(@Symbol)", conn))
                {
                    command.Parameters.Add(new SqlParameter("Symbol", symbolIn));                   
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            conn.Close();
        }

        //Creates a new stock day in the DB
        public void CreateDay(int stockId, StockDay dayIn)
        {
            conn.Open();
            try
            {
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO Day (stockid, dayopen, dayclose, dayhigh, daylow, date, volume) VALUES(@StockId, @DayOpen, @DayClose, " +
                    "@DayHigh, @DayLow, @Date, @Volume)", conn))
                {
                    command.Parameters.Add(new SqlParameter("StockId", stockId));
                    command.Parameters.Add(new SqlParameter("DayOpen", dayIn.Open));
                    command.Parameters.Add(new SqlParameter("DayClose", dayIn.Close));
                    command.Parameters.Add(new SqlParameter("DayHigh", dayIn.High));
                    command.Parameters.Add(new SqlParameter("DayLow", dayIn.Low));
                    command.Parameters.Add(new SqlParameter("Date", dayIn.Date));
                    command.Parameters.Add(new SqlParameter("Volume", dayIn.Volume));
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            conn.Close();
        }
    }
}
