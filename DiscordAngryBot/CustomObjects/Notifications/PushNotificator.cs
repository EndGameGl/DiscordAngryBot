using Discord;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace DiscordAngryBot.CustomObjects.Notifications
{
    /// <summary>
    /// Class for Windows 10 push notifications
    /// </summary>
    public static class PushNotificator
    {
        private static bool _isEnabled = false;

        /// <summary>
        /// Push new notification
        /// </summary>
        /// <param name="e">Message to push</param>
        /// <returns></returns>
        public static async Task Notify(LogMessage e)
        {
            if (_isEnabled)
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
