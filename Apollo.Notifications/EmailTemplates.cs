using Apollo.Config;
using Apollo.Notifications.Models;

namespace Apollo.Notifications;

public static class EmailTemplates
{
    private static readonly string clientUrl = AppConfig.Client.Url;

    public static string ResearchComplete(ResearchCompleteContent content) =>
        """
            <!DOCTYPE html>
            <html>
            <head>
              <style>
                body {
                  font-family: Arial, sans-serif;
                  margin: 0;
                  padding: 0;
                  background-color: #f8f8f8;
                }
                .container {
                  max-width: 700px;
                  margin: 0 auto;
                  background-color: white;
                  border: 1px solid #e0e0e0;
                  overflow: hidden;
                }
                .section {
                  padding: 20px;
                  border-bottom: 1px solid #e0e0e0;
                }
                .header {
                  display: flex;
                  justify-content: space-between;
                  align-items: center;
                }
                .tracking-title {
                  font-weight: bold;
                  font-size: 16px;
                  margin: 0;
                }
                .tracking-number {
                  color: #707070;
                  margin-top: 5px;
                }
                .button {
                  background-color: white;
                  border: 1px solid #e0e0e0;
                  padding: 12px 20px;
                  text-align: center;
                  text-decoration: none;
                  font-size: 14px;
                  color: black;
                  cursor: pointer;
                }
                .main-content {
                  text-align: center;
                  padding: 40px 20px;
                }
                .logo {
                  margin-bottom: 20px;
                }
                .heading {
                  font-size: 28px;
                  font-weight: bold;
                  margin-bottom: 25px;
                }
                .message {
                  color: #707070;
                  line-height: 1.6;
                  margin: 0 auto;
                  max-width: 500px;
                }
              </style>
            </head>
            <body>
              <div class="container">
                <div class="section header">
                  <div>
                   Apollo ~ Deep Research
                  </div>
                  <a href="{ClientUrl}/research/{ResearchId}" class="button">View</a>
                </div>

                <div class="section main-content">
                  <h1 class="heading">Your Research Is Complete.</h1>
                  <p class="message">
                    Your research has been completed. Use the button above to view the final report and insights for :  {Title}
                  </p>
                </div>
              </div>
            </body>
            </html>
            """.Replace("{ResearchId}", content.ResearchId).Replace(
            "{Title}",
            content.Title
        ).Replace("{ClientUrl}", clientUrl);
}
