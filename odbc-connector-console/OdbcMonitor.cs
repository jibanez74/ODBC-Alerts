using System;
using System.Data.Odbc;
using System.Timers;
using MailKit;
using MimeKit;

namespace odbc_connector_console
{
  public class OdbcMonitor
  {
    // static props of class
    private static System.Timers.Timer aTimer;
    private static MailKit.Net.Smtp.SmtpClient smtpClient;

    // props for odbc tasks
    private string dsnConnection;
    private string tableName;
    private string colName;
    private int colNumber;
    private string result;

    // props related to emails
    private int smtpPort = 587;
    private string smtpUrl = "http://smtp-url-here";
    private MimeMessage message;
    private string messageFrom = "no-reply@inteldot.com";
    private string messageTo = "recipient-email@email.com";
    private string subject = "Warning from ODBC middleware";
    private string username = "smtp-username";
    private string password = "smtp-password";

    public OdbcMonitor(string selectedSource, string tableName, string colName, string colNumber, string result)
    {
      Console.WriteLine("Running constructor for OdbcMonitor");
      this.dsnConnection = selectedSource;
      this.tableName = tableName;
      this.colName = colName;
      this.result = result;

      // init instance of smtpClient
      this.message = new MimeMessage();
      this.message.From.Add(new MailboxAddress(this.messageFrom));
      this.message.To.Add(new MailboxAddress(this.messageTo));
      this.message.Subject = this.subject;
      this.message.Body = new TextPart("plain", "The result does not match the value on the column " + this.colName);
      try
      {
        this.colNumber = int.Parse(colNumber);
      }
      catch (Exception error)
      {
        Console.WriteLine(error.Message);
      }
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
      string sqlQuery = "select " + this.colName + " from " + this.tableName + ";";

      OdbcConnection connection = new OdbcConnection("DSN=" + this.dsnConnection + ";");

      // vars for cases when multiple columns and rows are being evaluated
      // bool read;
      // object[] columnValues = new object[1];

      // creates a new odbc command with a query and the connection
      OdbcCommand command = new OdbcCommand(sqlQuery, connection);

      try
      {
        connection.Open();
        OdbcDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
          if (this.result == reader.GetString(this.colNumber))
          {
            Console.WriteLine("Values match");
          }
          else
          {
            Console.WriteLine("Values do not match - About to send email alert");
            smtpClient.Connect(this.smtpUrl, this.smtpPort);

            // this option is only needed if server requires it
            // the port will also depend on the provider/hosting provider
            smtpClient.Authenticate(this.username, this.password);

            smtpClient.Send(this.message);
            smtpClient.Disconnect(true);
          }
          reader.Close();
        }

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