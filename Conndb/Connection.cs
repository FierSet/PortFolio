namespace Portfolio.Controllers;
public class Connection
{
    public string SQLSTRING = string.Empty;

    public Connection()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

        SQLSTRING = builder.GetSection("ConnectionString:stringSQL").Value ?? "";
        // Console.WriteLine("-----" + SQLSTRING);
    }

    public string getstringsql()
    {
        return SQLSTRING;
    }

}