namespace Pustok.Services
{
    public interface IEmailSender
    {
       
            void Send(string to, string subject, string html);
            void Send(string[] allto, string subject, string html);

        
    }
}
