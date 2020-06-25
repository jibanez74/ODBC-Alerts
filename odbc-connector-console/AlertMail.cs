using System;
using System.Threading.Tasks;
using MailKit;
using MimeKit;

namespace odbc_connector_console
{
  public class AlertMail
  {
    private int smtpPort = 587;
    private string smtpUrl = "http://smtp-url-here";
    private MimeMessage message;
    private string messageFrom = "no-reply@inteldot.com";
    private string messageTo = "recipient-email@email.com";
    private string subject = "Warning from ODBC middleware";
    private string username = "smtp-username";
    private string password = "smtp-password";
    private bool emailSent = false;

    // public contructor for class
    public AlertMail()
    {
      Console.WriteLine("About to init contructor for AlertMail");
      this.message = new MimeMessage();
      this.message.From.Add(new MailboxAddress(this.messageFrom));
      this.message.To.Add(new MailboxAddress(this.messageTo));
      this.message.Subject = this.subject;
      this.message.Body = new TextPart("plain", "The result does not match the value on the column");
    }

    public void SetEmailSent(bool value)
    {
      this.emailSent = value;
    }

    public bool GetEmailSent()
    {
      return this.emailSent;
    }

    public async void SendEmailAlert()
    {
      try
      {
        var smtpClient = new MailKit.Net.Smtp.SmtpClient();

        await smtpClient.ConnectAsync(this.smtpUrl, this.smtpPort, true);

        // this option is only needed if server requires it
        // the port will also depend on the provider/hosting provider
        await smtpClient.AuthenticateAsync(this.username, this.password);

        await smtpClient.SendAsync(this.message);
        await smtpClient.DisconnectAsync(true);
      }
      catch (Exception error)
      {
        Console.WriteLine("An error occurred with the email \n {0}", error.Message);
      }
    }
  }
}