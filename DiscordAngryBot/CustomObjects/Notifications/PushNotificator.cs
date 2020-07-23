using Discord;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace DiscordAngryBot.CustomObjects.Notifications
{
    public static class PushNotificator
    {
        public static async Task Notify(LogMessage e)
        {
            await Task.Run(() =>
            {
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                stringElements[0].AppendChild(toastXml.CreateTextNode("Bot error:"));
                string message = e.Message;
                if (e.Exception != null)
                {
                    message += $" {e.Exception.Message}";
                }
                stringElements[1].AppendChild(toastXml.CreateTextNode($"{message}"));
                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier("DiscordBotAPP").Show(toast);
            });
        }
    }
}
