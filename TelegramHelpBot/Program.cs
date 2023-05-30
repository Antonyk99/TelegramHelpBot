using Telegram.Bot.Polling;
using HtmlAgilityPack;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using System.Xml.Linq;
using System.Xml;

internal class Program
{
    static ITelegramBotClient bot = new TelegramBotClient("5935980402:AAEq6uZKACvGRSfnk9dqwvb0lq8rcky1y5s");

    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Console.WriteLine("Bot was started!!!");

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        Console.ReadLine();
        Console.WriteLine("Bot was stopped!!!");

        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }

    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
        {
            var message = update.Message;
            if (message.Text.ToLower() == "/start")
            {
                HtmlWeb web = new HtmlWeb();

                web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36";
                HtmlDocument doc = web.Load("https://www.bloomberg.com/quote/USDGEL:CUR");
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//span[contains(@class, 'priceText__0550103750')]");

                if (nodes != null)
                {
                    foreach (HtmlNode node in nodes)
                        await botClient.SendTextMessageAsync(message.Chat, node.InnerHtml + "GEL");
                }

                return;
            }
            if (message.Text.ToLower() == "/who")
            {
                await botClient.SendTextMessageAsync(message.Chat, "Ирка казюля");
                return;
            }
            await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!");
        }
    }
}