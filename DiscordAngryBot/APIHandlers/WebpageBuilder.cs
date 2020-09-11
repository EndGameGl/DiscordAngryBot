using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace DiscordAngryBot.APIHandlers
{
    /// <summary>
    /// Class for building web pages
    /// </summary>
    public static class WebpageBuilder
    {
        /// <summary>
        /// Sets a doctype
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <returns></returns>
        public static WebPage SetDoctype(this WebPage webPage)
        {
            webPage.Doctype = "<!DOCTYPE html>";
            return webPage;
        }

        /// <summary>
        /// Sets page title
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="Title">Page title</param>
        /// <returns></returns>
        public static WebPage AddTitle(this WebPage webPage, string Title)
        {
            webPage.HeadContent += $"<title>{Title}</title>";
            return webPage;
        }

        /// <summary>
        /// Adds page styles
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="Styles">Page styles</param>
        /// <returns></returns>
        public static WebPage AddStyle(this WebPage webPage, string Styles)
        {
            webPage.HeadContent += $"<style>{Styles}</style>";
            return webPage;
        }

        /// <summary>
        /// Adds a header link
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="rel">Specifies the relationship between the current document and the linked document</param>
        /// <param name="href">Specifies the location of the linked document</param>
        /// <param name="integrity">Subresource Integrity</param>
        /// <param name="crossorigin">Specifies how the element handles cross-origin requests</param>
        /// <returns></returns>
        public static WebPage AddHeaderLink(this WebPage webPage, string rel, string href, string integrity, string crossorigin)
        {
            webPage.HeadContent += $"<link rel=\"{rel}\" href=\"{href}\" integrity=\"{integrity}\" crossorigin=\"{crossorigin}\">";
            return webPage;
        }

        /// <summary>
        /// Adds new header to the body
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="headerText">Header text</param>
        /// <param name="size">Header size</param>
        /// <returns></returns>
        public static WebPage AddBodyHeader(this WebPage webPage, string headerText, int size)
        {
            webPage.BodyContent += $"<h{size}>{headerText}</h{size}>";
            return webPage;
        }

        /// <summary>
        /// Opens a new div tag
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="tags">Tag class data</param>
        /// <returns></returns>
        public static WebPage AddOpenDiv(this WebPage webPage, string tags)
        {
            webPage.BodyContent += $"<div class=\"{tags}\">";
            return webPage;
        }

        /// <summary>
        /// Closes div tag
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <returns></returns>
        public static WebPage AddCloseDiv(this WebPage webPage)
        {
            webPage.BodyContent += $"</div>";
            return webPage;
        }

        /// <summary>
        /// Adds a new datatable
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="Headers">Table headers</param>
        /// <param name="data">Table data</param>
        /// <param name="tableClass">Table classes</param>
        /// <returns></returns>
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

        /// <summary>
        /// Adds a new paragraph
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="text">Paragraph text</param>
        /// <returns></returns>
        public static WebPage AddParagraph(this WebPage webPage, string text)
        {
            webPage.BodyContent += $"<p>{text}</p>";
            return webPage;
        }

        /// <summary>
        /// Adds new unordered list
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <returns></returns>
        public static WebPage AddOpenList(this WebPage webPage)
        {
            webPage.BodyContent += $"<ul>";
            return webPage;
        }

        /// <summary>
        /// Closes an unordered list
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <returns></returns>
        public static WebPage AddCloseList(this WebPage webPage)
        {
            webPage.BodyContent += $"</ul>";
            return webPage;
        }

        /// <summary>
        /// Adds an element to UL
        /// </summary>
        /// <param name="webPage">WebPage class object</param>
        /// <param name="element">Element data</param>
        /// <returns></returns>
        public static WebPage AddListElement(this WebPage webPage, string element)
        {
            webPage.BodyContent += $"<li>{element}</li>";
            return webPage;
        }
    }
}
