using Newtonsoft.Json;
using RemoteControlBotSample.MessageRouting;
using System;
using System.Collections.Generic;

namespace RemoteControlBotSample.Notifications
{
    [Serializable]
    public class Notification
    {
        public List<Party> PartiesToNotify
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
            PartiesToNotify = new List<Party>();
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