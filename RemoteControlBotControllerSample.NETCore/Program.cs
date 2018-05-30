using Microsoft.Bot.Schema;
using RemoteControlBotControllerSample.NETCore.Notifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteControlBotControllerSample.NETCore
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
            NotificationSender notificationSender =
                new NotificationSender("INSERT YOUR DIRECT LINE SECRET HERE");

            IList<ConversationReference> usersToNotify = new List<ConversationReference>();

            /*
             * You can set the parties to notify here. In order to do that you need to access the
             * database of users collected by the bot somehow. You could also send a backchannel
             * message to the bot asking for suitable parties. However, that is not the point of
             * this sample so I will leave the implementation to you.
             */

            string message = null;

            if (usersToNotify.Count == 0)
            {
                Console.Write("Enter the message to send > ");
                message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine("So you couldn't come up with anything, huh?");
                    message = "Message test";
                }
            }

            for (int i = 0; i < NumberOfNotificationsToSend; ++i)
            {
                Microsoft.Bot.Connector.DirectLine.ResourceResponse resourceResponse = null;

                if (usersToNotify.Count == 0)
                {
                    Log($"Sending message \"{message}\" {(i + 1)}/{NumberOfNotificationsToSend}...");
                    resourceResponse = await notificationSender.PostMessageAsync($"{message} {(i + 1)}", ActivityTypes.Event);
                }
                else
                {
                    Log($"Sending notification {(i + 1)}/{NumberOfNotificationsToSend}...");
                    resourceResponse = await notificationSender.NotifyAsync(usersToNotify, $"Notification test {(i + 1)}");
                }

                Log($"{((resourceResponse == null) ? "Received no response" : $"Received resource response with ID {resourceResponse.Id}")}");

#if DEBUG
                // The following will dump the activity info into Output (console)
                Microsoft.Bot.Connector.DirectLine.Activity activity = await notificationSender.GetLatestReplyAsync();
#endif

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
