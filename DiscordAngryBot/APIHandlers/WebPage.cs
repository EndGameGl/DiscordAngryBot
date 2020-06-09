using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.APIHandlers
{
    public class WebPage
    {
        public string Doctype { get; set; }
        public string HeadContent { get; set; }
        public string BodyContent { get; set; }

        public string Build()
        {
            return $"{Doctype}<html><head>{HeadContent}</head><body>{BodyContent}</body></html>";
        }
    }
}
