using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Groups;

namespace BotManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static DiscordSocketClient discordClient = new DiscordSocketClient();
        static SocketGuild guild;
        public static List<Group> groups = new List<Group>();
        public MainWindow()
        {
            InitializeComponent();
            ListView groupListView = new ListView() { Height = 200, Width = 200, ItemsSource = groups };
            mainGrid.Children.Add(groupListView);
            RunBot().GetAwaiter();
        }

        public static async Task RunBot()
        {
            string token = File.ReadAllText(@"F:\Programming Stuff\DToken\Token.txt");
            await discordClient.LoginAsync(Discord.TokenType.Bot, token);
            await discordClient.StartAsync();
            discordClient.Ready += Ready;
            await Task.Delay(Timeout.Infinite);
        }

        public static async Task Ready()
        {
            guild = discordClient.GetGuild(636208919114547212);
            string groupAPIUrl = "http://217.114.147.159:25565/";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(groupAPIUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync("api/Groups/");

            if (response.IsSuccessStatusCode)
            {
                var objects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GroupJSONObject>>(await response.Content.ReadAsStringAsync());
                foreach (var group in objects)
                {
                    groups.Add(await group.ConvertToGroup(guild));
                }
            }
        }
    }
}
