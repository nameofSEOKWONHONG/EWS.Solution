using eXtensionSharp;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using static Microsoft.Win32.Registry;

namespace EWS.Domain.Infra.Service;

public interface IHostNotificationService
{
    Task NotificationAsync();
}

public class HostNotificationService : IHostNotificationService
{
    private readonly INodeHostService _nodeHostService;
    public HostNotificationService(INodeHostService nodeHostService)
    {
        _nodeHostService = nodeHostService;
    }

    public async Task NotificationAsync()
    {
        // var javascript =
        //     File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SendMail/src/index.js"));
        // await _nodeHostService.NpmVoidRun(javascript);

//         #region [native]
//
//         var from = CurrentUser.OpenSubKey("Software").OpenSubKey("EWS").GetValue("Email").xValue<string>();
//         var to =   CurrentUser.OpenSubKey("Software").OpenSubKey("EWS").GetValue("To").xValue<string>();
//         var host = CurrentUser.OpenSubKey("Software").OpenSubKey("EWS")?.GetValue("Host").xValue<string>();
//         var port = CurrentUser.OpenSubKey("Software").OpenSubKey("EWS").GetValue("Port").xValue<string>();
//         var pw =   CurrentUser.OpenSubKey("Software").OpenSubKey("EWS").GetValue("Password").xValue<string>();
//         var user = from;
//         
//         var email = new MimeMessage();
//         email.From.Add(MailboxAddress.Parse(from));
//         email.To.Add(MailboxAddress.Parse(to));
//         email.Subject = "Service Working";
//         email.Body = new TextPart(TextFormat.Text)
//         {
//             Text = @$"
// Local IP : {XEnvExtensions.GetLocalIPAddress()},
// outer IP : {XEnvExtensions.GetExternalIPAddress()},
// {nameof(Environment.MachineName)} : {Environment.MachineName},
// {nameof(Environment.ProcessPath)} : {Environment.ProcessPath},
// {nameof(Environment.UserDomainName)} :{Environment.UserDomainName}
// "
//         };
//         using var smtp = new SmtpClient();
//         await smtp.ConnectAsync(host, port.xValue<int>(0), true);
//         await smtp.AuthenticateAsync(user, pw);
//         await smtp.SendAsync(email);
//         await smtp.DisconnectAsync(true);        
//
//         #endregion
    }
}