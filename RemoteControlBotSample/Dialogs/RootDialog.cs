using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Underscore.Bot.MessageRouting;
using Underscore.Bot.Models;

namespace RemoteControlBotSample.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
#pragma warning disable 1998
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(OnMessageReceivedAsync);
        }
#pragma warning restore 1998

        private async Task OnMessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity messageActivity = await result;

            if (messageActivity != null)
            {
                await context.PostAsync($"The text part of the received activity reads \"{messageActivity.Text}\"");

                if (messageActivity.ChannelData != null)
                {
                    // For debugging let's broadcast the received channel data content
                    MessageRouterManager messageRouterManager = WebApiConfig.MessageRouterManager;
                    IList<Party> userParties = messageRouterManager.RoutingDataManager.GetUserParties();

                    if (userParties.Count == 0)
                    {
                        await context.PostAsync("I've got no user parties! This should not happen!");
                    }
                    else
                    {
                        // Broadcast the channel data content
                        foreach (Party party in userParties)
                        {
                            await messageRouterManager.SendMessageToPartyByBotAsync(
                                party, $"Received the following channel data (as string): {messageActivity.ChannelData.ToString()}");
                        }
                    }
                }
            }
            else
            {
                await context.PostAsync("The received activity was null or not of type IMessageActivity!");
            }

            context.Done(new object());
        }
    }
}