using Microsoft.Data.Sqlite;
namespace Portfolio.Controllers;

public class Transfer {

    public List<Dictionary<string, object>> Loaddata(string QRY)
    {
        List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();
        var conn = new Connection().getstringsql();
        
        try
        {
            using var connection = new SqliteConnection(conn);

            //Console.WriteLine("-----" + conn);

            connection.Open();
            SqliteCommand cmd = new SqliteCommand(QRY, connection);
            SqliteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>{};

                for(int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                
                dataList.Add(row);
            }
            reader.Close(); // Close the reader
            connection.Close(); // Close connection
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in list method: " + ex.Message + ":"  + ex.StackTrace);
            dataList.DefaultIfEmpty();
        }

        return dataList;
    }
}