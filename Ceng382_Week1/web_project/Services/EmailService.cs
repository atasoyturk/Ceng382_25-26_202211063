using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using tastemam.Data;

namespace tastemam.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly LogService _logService;

        public EmailService(IConfiguration config, LogService logService)
        {
            _config = config;
            _logService = logService;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"],
                _config["EmailSettings:SenderEmail"]
            ));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
                _config["EmailSettings:SenderEmail"],
                _config["EmailSettings:SenderPassword"]
            );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            await _logService.LogAsync("Email", $"Email gönderildi: {toEmail} — {subject}", toEmail);
        }

        public async Task SendOrderConfirmationAsync(string toEmail, int orderId, decimal totalPrice, string items)
        {
            var subject = $"TasteMam - Sipariş Onayı #{orderId}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #C0392B;'>Siparişiniz Alındı!</h2>
                    <p>Sipariş numaranız: <strong>#{orderId}</strong></p>
                    <p>Sipariş içeriği:</p>
                    <pre style='background:#f5f5f5; padding:10px;'>{items}</pre>
                    <p>Toplam tutar: <strong style='color:#C0392B;'>{totalPrice}₺</strong></p>
                    <hr/>
                    <p style='color:#888; font-size:0.85rem;'>TasteMam Catering</p>
                </div>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendOrderNotificationToCaretakerAsync(string caretakerEmail, int orderId, string customerEmail, decimal totalPrice)
        {
            var subject = $"TasteMam - Yeni Sipariş #{orderId}";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #C0392B;'>Yeni Sipariş Geldi!</h2>
                    <p>Sipariş numarası: <strong>#{orderId}</strong></p>
                    <p>Müşteri: <strong>{customerEmail}</strong></p>
                    <p>Toplam tutar: <strong style='color:#C0392B;'>{totalPrice}₺</strong></p>
                    <hr/>
                    <p style='color:#888; font-size:0.85rem;'>TasteMam Catering</p>
                </div>";

            await SendEmailAsync(caretakerEmail, subject, body);
        }
    }
}