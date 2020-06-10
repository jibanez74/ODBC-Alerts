using System;
using System.Text;
using System.Timers;
using Microsoft.Win32;

namespace odbc_connector_console
{
  class Program
  {
    static void Main(string[] args)
    {
      GetAppInfo();
      string selectedSource = GetSource();

      Console.WriteLine("Please enter the name of the table");
      string tableName = Console.ReadLine();

      Console.WriteLine("The name of the column to evaluate");
      string columnName = Console.ReadLine();

      Console.WriteLine("Enter the result to evaluate");
      string result = Console.ReadLine();

      OdbcMonitor odbcMonitorOne = new OdbcMonitor(selectedSource, tableName, columnName, columnNumber, result);
      odbcMonitorOne.ReadAndEval();
    }

    // Get and display app info
    private static void GetAppInfo()
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
    private static string GetSource()
    {
      try
      {

        RegistryKey reg = (Environment.Is64BitOperatingSystem) ?
          RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("Software") :
          RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("Software");

        reg = reg.OpenSubKey("ODBC");
        reg = reg.OpenSubKey("ODBC.INI");
        reg = reg.OpenSubKey("ODBC Data Sources");

        string[] sources = reg.GetValueNames();
        string instance = "";
        int optionNumber = 1;
        string userInput = "";
        int selected = 0;

        if (sources == null)
        {
          Console.WriteLine("No connections available");
          return "No sources available";
        }

        Console.WriteLine("Enter the number of the option you wish to select");

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
      catch (Exception error)
      {
        Console.WriteLine(error.Message);
      }
      return "";
    }
  }
}