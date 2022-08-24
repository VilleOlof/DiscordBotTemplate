# Discord.NET Bot Template

This Is A Discord Bot Template Using Discord.NET  
Has The Standard 'Boiler Plate' Startup Bot Code.  

This Template Include Support For Slash Commands via Attributes,  
And Command Option Loading via JSON files.  

It Also Supports Component Buttons And Button Click Responses.

This Template Is Based On Beeper's Ground-Laying Code For Slash Commands & Discord Stuff.  
*(Beeper Is A Discord Bot In The Official Marble It Up! Discord Server, Also Developed By Me)*

## Getting Started
Quite Simple, Download The Files.  
And Change `"<ENV-TOKEN-KEY-NAME>"` In `Program.cs`,  
To Your Discord Bot Token, Which Should Be An Environment Variable.  

Just Like That, You Can Build It And Run It Just Fine,  
Well You Might Also Wanna Customize And Expand Upon This Template.  

### Simple Command Example
```cs
[Command("hello", "A Test Command To Show How It Works!", _Scope:CommandScope.Guild, _Options:new string[] { "amount" })]
public static async Task TestCommand(SocketSlashCommand command)
{
    //Get All The Current Command Option Values And Respond With It.
    var options = CommandHandler.GetCurrentOptions(command);
    await command.RespondAsync($"Hello World! ({options["amount"]})");

    return;
}
```

### Button Component Command Example
```cs
[Command("button","A Test Command To Show Discord Button Components!",_Scope:CommandScope.Global)]
public static async Task ButtonCommand(SocketSlashCommand command)
{
    //We Can DeferAsync Incase The Response Takes Extra Time To Respond.
    await command.DeferAsync();

    //Make A New Component, Add A Button And Respond With A Message Alongside The Component.
    var compBuilder = new ComponentBuilder();
    compBuilder.WithButton("Click Me!","testButton_ID",ButtonStyle.Primary);

    await command.ModifyOriginalResponseAsync(
        m => {
            m.Content = "This Message Has A Button!";
            m.Components = compBuilder.Build();
        });

    //We Add A Delegate To The Button Dictionary, So It Can Be Saved And Executed Later.
    Buttons["testButton_ID"] = delegate (SocketMessageComponent component)
    {
        component.UpdateAsync(m => m.Content = "You Clicked The Button!");
        return Task.CompletedTask;
    };

    return;
}
```
