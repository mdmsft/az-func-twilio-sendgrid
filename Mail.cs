using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System.Net.Mime;

namespace Functions
{
    public static class Mail
    {
        [FunctionName(nameof(Mail))]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            [SendGrid(ApiKey = nameof(Settings.SendGridApiKey))] IAsyncCollector<SendGridMessage> messages)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            
            log.LogInformation("Sending message with text: {0}", body);

            var message = GenerateMessage(body);

            await messages.AddAsync(message);
        }

        private static SendGridMessage GenerateMessage(string body)
        {
            var message = new SendGridMessage();

            message.AddTo(Environment.GetEnvironmentVariable(nameof(Settings.SendGridRecipient)));
            message.AddContent(MediaTypeNames.Text.Plain, body);
            message.SetFrom(new EmailAddress(Environment.GetEnvironmentVariable(nameof(Settings.SendGridSender)), "Azure Functions"));
            message.SetSubject("Azure Functions");

            return message;
        }
    }
}
