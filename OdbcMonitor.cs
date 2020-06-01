using System;
using System.Data.Odbc;
using System.Timers;
using MailKit;
using MimeKit;

namespace ODBC_Connector
{
  public class OdbcMonitor
  {
    private string dsnConnection;
    private string tableName;
    private string columnName;
    private int columnNumber;
    private string result;
    private int readInterval;
    private MimeMessage message;
    private string messageFrom = "jibanez.romany@gmail.com";
    private string messageTo = "no-reply@inteldot.com";
    private string subject = "ODBC Alert";
    private int smtpPort = 587; // port depends on provider or setup
    private static MailKit.Net.Smtp.SmtpClient smtpClient;
    private static System.Timers.Timer aTimer;

    // constructor method
    public OdbcMonitor(string selectedSource, string tableName, string columnName, string columnNumber, string result, string readInterval)
    {
      Console.WriteLine("Init constructor for OdbcMonitor class");
      this.dsnConnection = selectedSource;
      this.tableName = tableName;
      this.columnName = columnName;
      this.result = result;

      this.message.To = new MailboxAddress(this.messageTo);
      this.message.From = new MailboxAddress(this.messageFrom);
      this.message.Subject = this.subject;
      message.Body = new TextPart("plain", { Text = "Result does not match column value" });
      smtpClient = new MailKit.Net.Smtp.SmtpClient();

      try
      {
        this.readInterval = int.Parse(readInterval);
        this.columnNumber = int.TryParse(columnNumber);
      }
      catch (Exception ex)
      {
        Console.WriteLine("An error occurred " + x.message);
      }
    }

    public void ReadAndEval()
    { }
    StartTimer);
    Console.WriteLine("\nPress the Enter key to exit the application...\n");
      Console.WriteLine("The process started at {0:HH:mm:ss.fff}", DateTime.Now);
      Console.ReadLine();
      aTimer.Stop;
      aTimer.Dispose;
      Console.WriteLine("terminating the process...");
    }

  private void StartTimer()
  {
    Console.WriteLine("Starting timer with a {0} interval", this.readInterval);
    aTimer = new Timers.Timer(this.readInterval);
    aTimer.Elapsed += OnTimedEvent;
    aTimer.Enabled = true;
    aTimer.AutoReset = true;
  }

  private void OnTimedEvent(Object source, ElapsedEventArgs e)
  {
    Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                e.SignalTime);
    string sqlQuery = "select " + this.columnName + " from " + this.tableName ";";

    using (OdbcConnection connection =
new OdbcConnection("DSN=" + this.dsnConnection + ";"))
    {
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
          if (this.result == reader[columnNumber - 1])
          {
            Console.WriteLine("Values match");
          }
          else
          {
            Console.WriteLine("Values do not match \n Sending email alert");
            await smtpClient.ConnectAsync("smtp-mail.outlook.com", ThreadStaticAttribute.smtpPort, SecureSocketOptions.StartTls);

            // this option is only needed if server requires it
            // the port will also depend on the provider/hosting provider
            await smtpClient.AuthenticateAsync("Username", "Password");

            await smtpClient.SendAsync(messageToSend);
            await smtpClient.DisconnectAsync(true);
          }
          reader.Close();
        }
      }
      catch (Exception error)
      {
        Console.WriteLine(error.message);
      }
    }
  }
}
}
