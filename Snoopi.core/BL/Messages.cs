using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.DAL;
using System;
using System.Collections.Generic;

namespace Snoopi.core.BL
{
    public static class MessagesController
    {
        public static List<MessageUI> GetAllMessages(string SearchText)
        {

            Query qry = new Query(Message.TableSchema);
            List<MessageUI> list = new List<MessageUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new MessageUI
                    {
                        MessageId = Convert.ToInt64(reader["MessageId"]),
                        Description = Convert.ToString(reader["Description"]),
                        SendingDate = Convert.ToDateTime(reader["SendingDate"]).ToLocalTime(),
                    });
                }
            }
            return list;
        }

        public static Int64 CreateNewMessage(MessageUI messageUI)
        {
            Message msg = new Message
            {
                Description = messageUI.Description,
                SendingDate = DateTime.Now,
                
            };
            msg.Save();
            return msg.MessageId;
        
        }
    }

    public class MessageUI {

        public Int64 MessageId { get; set; }
        public string Description { get; set; }
        public DateTime SendingDate { get; set; }

        public static MessageUI ConvertMessageToMessageUI(Message message)
        {
            if (message != null)
            {
                MessageUI messageUI = new MessageUI
                {
                    MessageId = message.MessageId,
                    Description = message.Description,
                    SendingDate = message.SendingDate,
                };
                return messageUI;
            }
            return null;
        }
    }
}
