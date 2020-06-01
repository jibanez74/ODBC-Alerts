using System;
using System.Data.Odbc;
using Microsoft.Win32;
using System.Reflection;

namespace ODBC_Connector
{
  class Program
  {
    static void Main(string[] args)
    {
      GetAppInfo();
      string selectedSource = GetSource();
      Console.WriteLine("You have selected {0}", selectedSource);
      Console.WriteLine("Enter the name of the table to evaluate");
      string table_name = Console.ReadLine();

      Console.WriteLine("Enter the name of the column");
      string column = Console.ReadLine();

      Console.WriteLine("Enter the result that will trigger an email alert");
      string result = Console.ReadLine();

      Console.WriteLine("Interval to read");
      int readInterval = Console.ReadLine();

      OdbcMonitorOne = new OdbcMonitor(selectedSource, tableName, column, result, readInterval);
      OdbcMonitor.ReadAndEval();
    }

    // Get and display app info
    public static void GetAppInfo()
    {
      // Set app vars
      string appName = "ODBC Connector";
      string appVersion = "0.5";
      string appAuthor = "Inteldot";

      // Change text color
      Console.ForegroundColor = ConsoleColor.White;

      // Write out app info
      Console.WriteLine("{0}: Version {1} by {2}", appName, appVersion, appAuthor);

      // Reset text color
      Console.ResetColor();
    }

    // selects the correct source 
    public static string GetSource()
    {
      try
      {

        RegistryKey reg = (Environment.Is64BitOperatingSystem)
                    ? RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software") :
                    RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("Software");

        reg = reg.OpenSubKey("ODBC");
        reg = reg.OpenSubKey("ODBC.INI");
        reg = reg.OpenSubKey("ODBC Data Sources");
        //reg.GetValue("Rafael");

        string[] sources = reg.GetValueNames();
        string instance = "";
        int optionNumber = 1;
        string userInput = "";
        int selected = 0;

        if (sources == null)
        {
          Console.WriteLine("No sources available");
          return "No sources available";
        }

        Console.WriteLine("Enter the number corresponding to the connection you wish to select");

        foreach (string option in sources)
        {
          instance = optionNumber + " - " + option;
          Console.WriteLine(instance);
          optionNumber++;
        }

        userInput = Console.ReadLine();
        selected = int.Parse(userInput);

        return sources[selected - 1];
      }
      catch (Exception ex)
      {
        Console.WriteLine("An error occurred " + x.message);
      }

      return "";
    }

    public static void ReadAndEval(string connectionString)
    {
      Console.WriteLine("Your connection name is: " + connectionString);
      Console.WriteLine("Enter the name of the table to evaluate");
      string table_name = Console.ReadLine();

      Console.WriteLine("Enter the name of the column");
      string column = Console.ReadLine();

      Console.WriteLine("Enter the result that will trigger an email alert");
      string result = Console.ReadLine();

      // simple query to select the correct column
      string sqlQuery = "select " + column + " from " + table_name;

      using (OdbcConnection connection =
          new OdbcConnection("DSN=" + connectionString + ";"))
      {
        // vars for cases when multiple columns and rows are being evaluated
        // bool read;
        // object[] columnValues = new object[1];

        // creates a new odbc command with a query and the connection
        OdbcCommand command = new OdbcCommand(sqlQuery, connection);

        // try to read and evaluate the value
        try
        {
          // parse column number to a number
          int columnNumber = int.Parse(column);

          // open connection
          connection.Open();

          // for loop can be later replace with a System.Timer func
          for (int i = 1; i < 91; i++)
          {
            // execute the query to read columns
            OdbcDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
              if (result == reader[columnNumber - 1])
              {
                Console.WriteLine("Results match!  No email alert is necesary.");
              }
              else
              {
                Console.WriteLine("About to send email");
              }

              reader.Close();
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine("An error occurred " + x.message);
        }
        // once the using block is over, the connection will automatically close
        // if an odbc connection is made outside the using block, then it must be close manually
      }
    }
  }