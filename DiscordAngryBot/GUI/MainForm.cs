using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordAngryBot.GUI
{
    public partial class MainForm : Form
    {
        DiscordServerObject serverObject;
        public MainForm(DiscordServerObject serverObject)
        {
            InitializeComponent();
            this.serverObject = serverObject;
            serverChooserBox.Items.Add(serverObject.server.Name);
            foreach (var channel in serverObject.server.Channels)
            {
                channelChooserBox.Items.Add(channel.Name);
            }
        }

        private async void messageSender_Click(object sender, EventArgs e)
        {
            SocketGuildChannel channel = serverObject.server.Channels.Where(x => x.Name == channelChooserBox.SelectedItem.ToString()).SingleOrDefault();
            await ((ISocketMessageChannel)channel).SendMessageAsync(messageTextBox.Text);
        }
    }
}
