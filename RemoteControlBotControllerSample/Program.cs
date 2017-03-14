using Microsoft.Bot.Connector;
using RemoteControlBotControllerSample.Notifications;
using RemoteControlBotSample.MessageRouting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlBotControllerSample
{
    class Program
    {
        const int NumberOfNotificationsToSend = 3;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            NotificationSender notificationSender = new NotificationSender("INSERT YOUR DIRECT LINE SECRET HERE");

            IList<Party> partiesToNotify = new List<Party>();
            
            /*
             * You can set the parties to notify here. In order to do that you need to access the
             * database of users collected by the bot somehow. You could also send a backchannel
             * message to the bot asking for suitable parties. However, that is not the point of
             * this sample so I will leave the implementation to you.
             */

            for (int i = 0; i < NumberOfNotificationsToSend; ++i)
            {
                Log($"Sending notification {(i + 1)}/{NumberOfNotificationsToSend}...");

                Microsoft.Bot.Connector.DirectLine.ResourceResponse resourceResponse =
                    await notificationSender.NotifyAsync(partiesToNotify, $"Notification test {(i + 1)}");

                Log($"{((resourceResponse == null) ? "Received no response" : $"Received resource response with ID {resourceResponse.Id}")}");
                Thread.Sleep(3000);
            }

            Log("Done");
        }

        static void Log(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
