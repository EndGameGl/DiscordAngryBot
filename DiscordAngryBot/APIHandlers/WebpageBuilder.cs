using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.APIHandlers
{
    public static class WebpageBuilder
    {
        public static WebPage SetDoctype(this WebPage webPage)
        {
            webPage.Doctype = "<!DOCTYPE html>";
            return webPage;
        }

        public static WebPage AddTitle(this WebPage webPage, string Title)
        {
            webPage.HeadContent += $"<title>{Title}</title>";
            return webPage;
        }

        public static WebPage AddStyle(this WebPage webPage, string Styles)
        {
            webPage.HeadContent += $"<style>{Styles}</style>";
            return webPage;
        }

        public static WebPage AddHeaderLink(this WebPage webPage, string rel, string href, string integrity, string crossorigin)
        {
            webPage.HeadContent += $"<link rel=\"{rel}\" href=\"{href}\" integrity=\"{integrity}\" crossorigin=\"{crossorigin}\">";
            return webPage;
        }

        public static WebPage AddBodyHeader(this WebPage webPage, string headerText, int size)
        {
            webPage.BodyContent += $"<h{size}>{headerText}</h{size}>";
            return webPage;
        }

        public static WebPage AddOpenDiv(this WebPage webPage, string tags)
        {
            webPage.BodyContent += $"<div class=\"{tags}\">";
            return webPage;
        }

        public static WebPage AddCloseDiv(this WebPage webPage)
        {
            webPage.BodyContent += $"</div>";
            return webPage;
        }

        public static WebPage AddTable(this WebPage webPage, string[] Headers, IEnumerable<dynamic> data, string tableClass)
        {
            try
            {
                webPage.BodyContent += $"<table class=\"{tableClass}\">";
                webPage.BodyContent += $"<thead><tr>";
                foreach (var header in Headers)
                {
                    webPage.BodyContent += $"<th>{header}</th>";
                }
                webPage.BodyContent += $"</tr></thead>";
                Type type = data.GetType().GetGenericArguments()[0];
                PropertyInfo[] propertyInfo = type.GetProperties();
                webPage.BodyContent += $"<tbody>";
                foreach (var entry in data)
                {
                    webPage.BodyContent += $"<tr>";
                    foreach (var property in propertyInfo)
                    {
                        var value = property.GetValue(entry, null);
                        if (value is IList)
                        {
                            Type valueType = value.GetType().GetGenericArguments()[0];
                            PropertyInfo valuePropertyInfo = valueType.GetProperties()[0];
                            webPage.BodyContent += $"<td><ul>";
                            foreach (var val in (IList)value)
                            {
                                webPage.BodyContent += $"<li>{valuePropertyInfo.GetValue(val, null)}</li>";
                            }
                            webPage.BodyContent += $"</ul></td>";
                        }
                        else
                        {
                            webPage.BodyContent += $"<td>{property.GetValue(entry, null)}</td>";
                        }
                    }
                    webPage.BodyContent += $"</tr>";
                }
                webPage.BodyContent += $"</tbody>";
                webPage.BodyContent += $"</table>";

                return webPage;
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                return webPage;
            }
        }

        public static WebPage AddParagraph(this WebPage webPage, string text)
        {
            webPage.BodyContent += $"<p>{text}</p>";
            return webPage;
        }

        public static WebPage AddOpenList(this WebPage webPage)
        {
            webPage.BodyContent += $"<ul>";
            return webPage;
        }

        public static WebPage AddCloseList(this WebPage webPage)
        {
            webPage.BodyContent += $"</ul>";
            return webPage;
        }

        public static WebPage AddListElement(this WebPage webPage, string element)
        {
            webPage.BodyContent += $"<li>{element}</li>";
            return webPage;
        }
    }
}
