# Remote Control Bot Sample #

This sample demonstrates how to control a chatbot, built on the
[Microsoft Bot Framework](https://dev.botframework.com/), utilizing backchannel
messages. The covering note (a blog post) for this sample is here:
[Remotely Controlled Bots](http://tomipaananen.azurewebsites.net/?p=2231).
It is recommended to read that first to understand how the backchannel messages
work.

![Sample in action](Documentation/Screenshot.png?raw=true)

## Running and testing ##

1. Clone or copy the repository
2. Publish the bot ([see here how](https://docs.microsoft.com/en-us/bot-framework/bot-service-continuous-deployment))
   * Unfortunately registering and publishing the bot is the easiest way to get
     to test drive it, because the implementation is dependent on
     [Direct Line](https://docs.botframework.com/en-us/restapi/directline3/)
3. Get the **app ID** and **app password** (a step in registration phase) and
   insert them into the [Web.config](RemoteControlBotSample/Web.config)
4. Publish the bot (and insert the newly created endpoint to the bot portal)
5. In the portal, activate Direct Line and get the first secret key
6. Insert the secret key into [Program.cs](RemoteControlBotControllerSample/Program.cs)
7. Say something to the bot so that it knows who you are (your virtual address),
   you can do this on any channel the bot is on or using emulator
   * If you don't say anything the bot won't know you and cannot notify you
8. Run the console app (RemoteControlBotControllerSample)
9. Enjoy your three notifications

## Implementation ##

The implementation consists of roughly 3 different concepts:

1. Backchannel receiver (implemented in this sample by
   [NotificationsScorable](RemoteControlBotSample/Notifications/NotificationsScorable.cs),
   see `PrepareAsync` method)
2. Notification type and handler (in [Notifications](RemoteControlBotSample/Notifications) folder)
3. Message routing for delivering the notifications to (specific) users
   * This message routing logic is a subset of features implemented and documented
     in [Bot Message Routing (component) project](https://github.com/tompaana/bot-message-routing)

### How does it work? ###

Let's look at two scenarios. First, when the bot receives a normal message from
the user:

1. A user sends a message to the bot
   * The Microsoft Bot Framework translates this to an object called `Activity`,
     where its `Text` property will contain the message
2. The received `Activity` instance  is **seemingly** passed to the root dialog in
   [MessagesController](RemoteControlBotSample/Controllers/MessagesController.cs)
   class
3. Autofac checks for registered handlers at the previous step (step 2)
   * It will find two: [MessageRouterScorable](RemoteControlBotSample/MessageRouting/MessageRouterScorable.cs)
     and [NotificationsScorable](RemoteControlBotSample/Notifications/NotificationsScorable.cs)
   * `MessageRouterScorable` always stores the sender and the receiver of a
     message, if they have not been seen before (see
     [Chatbots as Middlemen article](http://tomipaananen.azurewebsites.net/?p=1851)
     to understand why)
   * `NotificationsScorable` on the other hand will look for specific
     backchannel messages, where the `Text` property of the `Activity` will read
     `"notification"` and then extracts the actual notification content from
     another property of `Activity` named `ChannelData`
   * In this case no backchannel message is detected and the root dialog is
     invoked
4. Root dialog handles the message as it is defined to

In the second scenario a backend (or other entity) sends a backchannel message
to the bot according to the notifications protocol we've agreed on:

1. A notification specific backchannel message is sent to the bot
2. The received `Activity` instance  is **seemingly** passed to the root dialog
   in `MessagesController` class - however it the dialog will never receive the
   `Activity` because of the `NotificationsScorable` class, and that is because...
3. Autofac happens
   * Now a backchannel message is detected by `NotificationScorable`, the score
     will exist and its value will be `1.0d` (fancy way of saying the value of
     action double type is exactly 1), and as a result,
     [NotificationsManager](RemoteControlBotSample/Notifications/NotificationsManager.cs)
     will be told to handle the `Activity` instance and the root dialog is never
     invoked
4. `NotificationsManager` uses `MessageRouterManager` to deliver the
   notifications to desired users

### Why Autofac? ###

As you noticed from the descriptions of the previous scenarios,
[Autofac](https://autofac.org/) - an implementation of inversion of control
(IoC) container - makes it harder to understand the execution flow of the code
without a thorough inspection (or proper documentation). In other words, IoC
makes it more difficult to have self-documenting code. That's why I don't like
Autofac or IoC containers in general. I'd rather have the backchannel detection
(and other things) written in
[MessagesController class](/RemoteControlBotSample/Controllers/MessagesController.cs),
before the decision to forward the `Activity` instance to the root dialog is made:

```cs
WebApiConfig.MessageRouterManager.MakeSurePartiesAreTracked(activity);
string notificationData = string.Empty;

if (NotificationsManager.TryGetNotificationData(activity, out notificationData))
{
    // A notification related backchannel message was detected
    await NotificationsManager.SendNotificationAsync(notificationData);
}
else
{
    await Conversation.SendAsync(activity, () => new RootDialog());
}
```

However, if you are using the Microsoft Bot Builder SDK, there is no escape from
Autofac since the SDK is built using it. You can still avoid the use in the code
you write. But that's my preference. You will have yours and it's neither right
nor wrong. Only debatable :)

## See also ##

* [Remotely Controlled Bots article](http://tomipaananen.azurewebsites.net/?p=2231)
* [Interruption Bot Sample](https://github.com/svandenhoven/Bots/tree/master/BotNotificationInteruptions): *"Contains a bot that can handle interuptions from a proactive bot. Low Prio interruptions are stacked and only shown when a defined dialog has stopped."*
* [Bot Message Routing (component) project](https://github.com/tompaana/bot-message-routing)
* [Intermediator Bot Sample](https://github.com/tompaana/intermediator-bot-sample)
