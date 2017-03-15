# Remote Control Bot Sample #

This sample demonstrates how to control a chatbot, built on the
[Microsoft Bot Framework](https://dev.botframework.com/), utilizing backchannel
messages. The covering note (a blog post) for this sample is here:
[Remotely Controlled Bots](http://tomipaananen.azurewebsites.net/?p=2231).

![Sample in action](Documentation/Screenshot.png?raw=true)

## Running and testing ##

1. Clone or copy the repository
2. Register a bot for this sample in [the bot portal](https://dev.botframework.com/)
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

## See also ##

* [Remotely Controlled Bots article](http://tomipaananen.azurewebsites.net/?p=2231)
