using Apollo.Notifications.Models;

namespace Apollo.Notifications;

public interface IEmailService
{
    Task<bool> SendResearchCompleteNotification(
        Recipient recipient,
        ResearchCompleteContent content
    );
}
