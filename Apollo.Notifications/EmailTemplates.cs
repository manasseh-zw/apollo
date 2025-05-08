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
                  background-color: #f8f8f8; /* Light grey background */
                }
                .container {
                  max-width: 700px;
                  margin: 0 auto;
                  background-color: white;
                  border: 1px solid #d0d0d0; /* Lighter grey border */
                  overflow: hidden;
                }
                .section {
                  padding: 20px;
                }
                .header {
                  display: flex;
                  justify-content: space-between;
                  align-items: center;
                  border-bottom: 1px solid #d0d0d0; /* Lighter grey border */
                }
                .header-text {
                    font-size: 18px;
                    font-weight: bold;
                    color: #333; /* Dark grey text */
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
                  display: inline-block;
                  background-color: #333; /* Black button background */
                  color: white !important; /* White text */
                  border: none;
                  padding: 12px 25px;
                  text-align: center;
                  text-decoration: none;
                  font-size: 16px;
                  cursor: pointer;
                  margin-left: 20px;
                }
                 .button:hover { /* Hover effect */
                    background-color: #555; /* Darker grey on hover */
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
                  color: #333; /* Dark grey heading */
                }
                .message {
                  color: #555; /* Darker grey message text */
                  line-height: 1.6;
                  margin: 0 auto;
                  max-width: 500px;
                  margin-bottom: 30px;
                }
                .footer {
                    text-align: center;
                    padding: 20px;
                    font-size: 12px;
                    color: #999; /* Light grey footer text */
                    border-top: 1px solid #d0d0d0; /* Lighter grey border */
                }
              </style>
            </head>
            <body>
              <div class="container">
                <div class="section header">
                  <div class="header-text"> Apollo ~ Deep Research
                  </div>
                  <a href="{ClientUrl}/research/{ResearchId}" class="button">View Research</a> </div>

                <div class="section main-content">
                  <h1 class="heading">Your Research Is Complete.</h1>
                  <p class="message">
                    Your research has been completed. Use the button below to view the final report and insights for: <strong>{Title}</strong> </p>
                  <a href="{ClientUrl}/research/{ResearchId}" class="button">View Research</a>
                </div>

                <div class="footer">
                    &copy; Apollo. All rights reserved.
                </div>
              </div>
            </body>
            </html>
            """.Replace("{ResearchId}", content.ResearchId).Replace(
            "{Title}",
            content.Title
        ).Replace("{ClientUrl}", clientUrl);
}
