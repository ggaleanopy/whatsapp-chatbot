using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ChatBot.Types;
using ChatBot.BLL;

namespace ChatBot.Controllers
{
    [RoutePrefix("api/chatbot")]
    public class ChatBotController : ApiController
    {
        public IHttpActionResult PostWhatsAppChatBot(WhatsAppChatBot messageModel)
        {
            using (BLL.ChatBot bll = new BLL.ChatBot())
            {
                bll.ProcessMessage(messageModel);
            }

            return Ok();
        }
    }
}
