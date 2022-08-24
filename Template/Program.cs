using Discord;
using Discord.WebSocket;

namespace DiscordBotTemplate;

internal class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    public static DiscordSocketClient? _client;
    public static ulong[] server_Ids = Array.Empty<ulong>();

    public static readonly Dictionary<string, Func<SocketMessageComponent, Task>> Buttons = new();

    public readonly static string FilePath = @"..\..\..\";

    private readonly static string token =
        Environment.GetEnvironmentVariable("<ENV-TOKEN-KEY-NAME>", 
            EnvironmentVariableTarget.User) 
        ?? throw new NullReferenceException("No ENV Token Found.");

    public async Task MainAsync()
    {
        //Makes A New Discord Client And Sets Up The Log Event.
        _client = new DiscordSocketClient();
        _client.Log += Log;
        
        //Logins And Starts The Bot On Discord.
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        //Runs A  Method To Initialize Everything That Is Dependent On The Client Being Ready.
        _client.Ready += Ready_Client;

        //Triggers The SlashCommandHandlers When A Command Has Been Executed.
        _client.SlashCommandExecuted += CommandHandler.SlashCommandHandler;

        //Triggers Button Components When Clicked.
        _client.ButtonExecuted += async (SocketMessageComponent component) => { 
            await Buttons[component.Data.CustomId](component); 
            Console.WriteLine($"{component.User.Username}#{component.User.Discriminator} Clicked On '{component.Data.CustomId}'"); 
        };
        /*-------- Connected To Discord, Rest of The Bot's Code Go Below This Line --------*/



        await Task.Delay(-1);
    }

    private static async Task Ready_Client()
    {
        //Gets All The Guild IDs.
        server_Ids = _client!.Guilds.Select(g => g.Id).ToArray();

        //Initialize The Slash Commands.
        await CommandHandler.SlashCommandInit();
    }
    
    private Task Log(LogMessage msg) {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    //Example Commands:

    [Command("hello", "A Test Command To Show How It Works!", _Scope:CommandScope.Guild, _Options:new string[] { "amount" })]
    public static async Task TestCommand(SocketSlashCommand command)
    {
        //Get All The Current Command Option Values And Respond With It.
        var options = CommandHandler.GetCurrentOptions(command);
        await command.RespondAsync($"Hello World! ({options["amount"]})");
        
        return;
    }

    [Command("button","A Test Command To Show Discord Button Components!",_Scope:CommandScope.Guild)]
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
}