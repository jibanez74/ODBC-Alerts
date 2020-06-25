using System;
using System.Data.Odbc;
using System.Timers;

namespace odbc_connector_console
{
  public class OdbcMonitor
  {
    // static props of class
    private static System.Timers.Timer aTimer;

    // props for odbc tasks
    private string dsnConnection;
    private string tableName;
    private string colName;
    private int colNumber = 0;
    private string result;
    private bool wrongValue = false;
    private AlertMail mailer = new AlertMail();

    public OdbcMonitor(string selectedSource, string tableName, string colName, string result)
    {
      Console.WriteLine("Running constructor for OdbcMonitor");
      this.dsnConnection = selectedSource;
      this.tableName = tableName;
      this.colName = colName;
      this.result = result;
    }

    public void ReadAndEval()
    {
      StartTimer();
      Console.WriteLine("\nPress the Enter key to exit the application...\n");
      Console.WriteLine("The process started at {0:HH:mm:ss.fff}", DateTime.Now);
      Console.ReadKey();
      aTimer.Stop();
      aTimer.Dispose();
      Console.WriteLine("terminating the process...");
    }

    private void StartTimer()
    {
      Console.WriteLine("About to start timer with a 3000 ms interval");
      aTimer = new System.Timers.Timer(3000);
      aTimer.Elapsed += OnTimedEvent;
      aTimer.Enabled = true;
      aTimer.AutoReset = true;
    }

    private void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
      Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
        e.SignalTime);
      string sqlQuery = "SELECT " + this.colName + " FROM " + this.tableName + " LIMIT 1;";

      OdbcConnection connection = new OdbcConnection("DSN=" + this.dsnConnection + ";");

      // creates a new odbc command with a query and the connection
      OdbcCommand command = new OdbcCommand(sqlQuery, connection);

      try
      {
        connection.Open();
        OdbcDataReader reader = command.ExecuteReader();

        reader.Read();

        if (this.result == reader.GetString(this.colNumber))
        {
          Console.WriteLine("Values match");
          this.wrongValue = false;
          mailer.SetEmailSent(false);
        }
        else
        {
          if (this.wrongValue && this.mailer.GetEmailSent())
          {
            Console.WriteLine("Result and value from column still do not match");
          }
          else
          {
            Console.WriteLine("Value from column does not match \n About to send email alert");
            this.wrongValue = true;
            mailer.SetEmailSent(true);
            mailer.SendEmailAlert();
          }
        }

        reader.Close();
        connection.Close();
      }
      catch (Exception error)
      {
        Console.WriteLine(error.Message);
        connection.Close();
      }
    }
  }
}