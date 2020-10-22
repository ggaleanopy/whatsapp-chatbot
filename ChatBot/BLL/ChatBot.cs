using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Configuration;
using System.IO;
using ChatBot.Types;
using System.Net;
using Newtonsoft.Json.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ChatBot.BLL
{
    public class ChatBot : BLLBase
    {
        #region Constantes
        private readonly string PHONE_NUMBER = (string) ConfigurationManager.AppSettings["PHONE_NUMBER"]; //"whatsapp:{0}";
        private readonly string WELCOME = (string)ConfigurationManager.AppSettings["WELCOME"];
        private readonly string MSG = (string)ConfigurationManager.AppSettings["MSG"];
        private readonly string RESET_WORD = (string)ConfigurationManager.AppSettings["RESET_WORD"];
        private readonly string RESET_MSG = (string)ConfigurationManager.AppSettings["RESET_MSG"];
        private readonly string WARNING_MSG = (string)ConfigurationManager.AppSettings["WARNING_MSG"];
        #endregion Constantes

        public void ProcessMessage(WhatsAppChatBot messageModel)
        {
            try
            {
                List<string> persistentAction = null;

                Nullable<int> messageCount = null;

                if (HttpContext.Current.Session["MessageCount"] != null)
                {
                    messageCount = Convert.ToInt32(HttpContext.Current.Session["MessageCount"].ToString());
                }

                if (messageCount == null)
                {
                    messageModel.Body = WELCOME;
                    HttpContext.Current.Session["MessageCount"] = 0;
                }
                else
                {
                    if (messageModel.Body.Trim().ToUpper() == RESET_WORD)
                    {
                        messageModel.Body = WELCOME;
                        messageCount = null;
                    }
                    else
                    {
                        messageCount++;
                        messageModel.Body = string.Format(MSG, messageCount.ToString(), messageModel.Body);
                    }

                    if (messageCount.HasValue)
                    {
                        HttpContext.Current.Session["MessageCount"] = messageCount.ToString();

                    }
                    else HttpContext.Current.Session["MessageCount"] = null;
                }

                this.SendWhatsApp(messageModel, persistentAction);

                if ((messageCount.HasValue) && (messageCount % 2 == 0))
                {
                    messageModel.Body = RESET_MSG;
                    this.SendWhatsApp(messageModel);
                }
            }
            catch(Exception ex)
            {
                messageModel.Body = WARNING_MSG;
                this.SendWhatsApp(messageModel);
            }
        }
        
        #region Helpers
        private void SendWhatsApp(WhatsAppChatBot messageModel, List<string> persistentAction = null)
        {
            string accountSid = (string)ConfigurationManager.AppSettings["TWILIO_ACCOUNT_SID"];
            string authToken = (string)ConfigurationManager.AppSettings["TWILIO_AUTH_TOKEN"];
            string senderPhoneNumber = (string)ConfigurationManager.AppSettings["TWILIO_SENDER_PHONE_NUMBER"];

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                            body: string.Format(messageModel.Body),
                            from: new Twilio.Types.PhoneNumber(String.Format(PHONE_NUMBER, senderPhoneNumber)),
                            persistentAction: persistentAction,
                            to: new Twilio.Types.PhoneNumber(messageModel.From)
                          );
        }
        #endregion Helpers


    }
}