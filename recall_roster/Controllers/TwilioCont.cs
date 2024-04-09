using System;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace TwilioSMSReceiver.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : TwilioController
    {
        [HttpPost]
        public TwiMLResult Index()
        {
            var response = new MessagingResponse();
            var senderPhoneNumber = Request.Form["From"];
            response.Message($"Hello, you sent: {Request.Form["From"]}");

            return TwiML(response);
        }
    }
}
