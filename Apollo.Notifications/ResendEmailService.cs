using Apollo.Config;
using Apollo.Notifications.Models;
using Resend;

namespace Apollo.Notifications;

public class ResendEmailService : IEmailService
{
    private readonly IResend _client;

    public ResendEmailService()
    {
        var options = new ResendClientOptions() { ApiToken = AppConfig.Resend.ApiKey };

        _client = ResendClient.Create(options);
    }

    public async Task<bool> SendResearchCompleteNotification(
        Recipient recipient,
        ResearchCompleteContent content
    )
    {
        var response = await _client.EmailSendAsync(
            new EmailMessage()
            {
                From = "Apollo <apollo@manasseh.dev>",
                To = recipient.Email,
                Subject =
                    $"Hey {recipient.Username} the team just completed your research, check it out.",
                HtmlBody = EmailTemplates.ResearchComplete(content),
            }
        );

        return response.Success;
    }
}
