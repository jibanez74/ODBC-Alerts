using System;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using System.Data.Odbc;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace odbc_connector
{
  class Program
  {
    static void Main(string[] args)
    {
      // display some basic program information
      GetAppInfo();

      string selectedOption = GetSource();

      // executes queries on a specific column or row
      // ExecuteTransaction(selectedOption);

      // selects columns and compares them to an expected result
      ReadAndEval(selectedOption);

      Console.ReadKey();
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

    // Print color message
    public static void PrintColorMessage(ConsoleColor color, string message)
    {
      // Change text color
      Console.ForegroundColor = color;

      // print message to the console
      Console.WriteLine(message);

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
          PrintColorMessage(ConsoleColor.Yellow, "No connections available");
          return "No sources available";
        }

        PrintColorMessage(ConsoleColor.Yellow, "Enter the number of the connection");

        foreach (string option in sources)
        {
          instance = optionNumber + " - " + option;
          PrintColorMessage(ConsoleColor.Yellow, instance);
          optionNumber++;
        }

        userInput = Console.ReadLine();
        selected = int.Parse(userInput);

        return sources[selected - 1];
      }
      catch (FormatException)
      {
        PrintColorMessage(ConsoleColor.Red, "Only numbers are accepted");
      }
      catch (ArgumentNullException)
      {
        PrintColorMessage(ConsoleColor.Red, "Please enter a number");
      }
      catch (System.IndexOutOfRangeException)
      {
        PrintColorMessage(ConsoleColor.Red, "You have an option that does not exist");
      }

      return "";
    }

    // will execute a query for a specific amount of time
    public static void ReadAndEval(string connectionString)
    {
      PrintColorMessage(ConsoleColor.White, "Your connection name is: " + connectionString);
      PrintColorMessage(ConsoleColor.Green, "Enter the table name");
      string table_name = Console.ReadLine();

      PrintColorMessage(ConsoleColor.Green, "Enter column number");
      string column = Console.ReadLine();

      PrintColorMessage(ConsoleColor.Green, "Enter result to evaluate");
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
                PrintColorMessage(ConsoleColor.Yellow, "The value of result and column match");
              }

              reader.Close();
            }
          }
        }
        catch (Exception ex)
        {
          PrintColorMessage(ConsoleColor.Red, ex.Message);
        }
        // once the using block is over, the connection will automatically close
        // if an odbc connection is made outside the using block, then it must be close manually
      }
    }

    // method to execute queries on tables
    public static void ExecuteTransaction(string connectionString)
    {
      PrintColorMessage(ConsoleColor.White, "Your connection name is: " + connectionString);

      PrintColorMessage(ConsoleColor.Green, "Enter a sql query");
      string sqlQuery = Console.ReadLine();

      using (OdbcConnection connection =
          new OdbcConnection("DSN=" + connectionString + ";"))
      {
        OdbcCommand command = new OdbcCommand();
        OdbcTransaction transaction = null;

        // Set the Connection to the new OdbcConnection.
        command.Connection = connection;

        // Open the connection and execute the transaction.
        try
        {
          connection.Open();

          // Start a local transaction
          transaction = connection.BeginTransaction();

          // Assign transaction object for a pending local transaction.
          command.Connection = connection;
          command.Transaction = transaction;

          // Execute the commands.
          command.CommandText = sqlQuery;
          command.ExecuteNonQuery();

          // Commit the transaction.
          transaction.Commit();
          PrintColorMessage(ConsoleColor.Blue, "Transaction was executed");
        }
        catch (Exception ex)
        {
          PrintColorMessage(ConsoleColor.Red, ex.Message);
          try
          {
            // Attempt to roll back the transaction.
            transaction.Rollback();
          }
          catch
          {
            // nothing to do here - transaction is not active
          }
        }
        // once the using block is over, the connection will automatically close
        // if an odbc connection is made outside the using block, then it must be close manually
      }
    }
  }
