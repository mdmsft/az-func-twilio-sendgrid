using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Functions
{
    public static class Sms
    {
        [FunctionName(nameof(Sms))]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            [TwilioSms(AccountSidSetting = nameof(Settings.TwilioAccountSid), AuthTokenSetting = nameof(Settings.TwilioAuthToken))] IAsyncCollector<CreateMessageOptions> sms)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation("Sending SMS with text: {sms}", body);

            var message = GenerateMessage(body);

            await sms.AddAsync(message);
        }

        private static CreateMessageOptions GenerateMessage(string body)
        {
            var sender = new PhoneNumber(Environment.GetEnvironmentVariable(nameof(Settings.TwilioSender)));
            var recipient = new PhoneNumber(Environment.GetEnvironmentVariable(nameof(Settings.TwilioRecipient)));

            return new CreateMessageOptions(recipient) { From = sender, Body = body };
        }
    }
}
