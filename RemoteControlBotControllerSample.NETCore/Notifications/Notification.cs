using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RemoteControlBotControllerSample.NETCore.Notifications
{
    [Serializable]
    public class Notification
    {
        public List<ConversationReference> UsersToNotify
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public Notification()
        {
            UsersToNotify = new List<ConversationReference>();
            Message = string.Empty;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Notification FromJsonString(string jsonString)
        {
            return JsonConvert.DeserializeObject<Notification>(jsonString);
        }
    }
}