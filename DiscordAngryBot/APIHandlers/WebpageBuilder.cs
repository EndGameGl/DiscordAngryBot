using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace DiscordAngryBot.APIHandlers
{
    /// <summary>
    /// Класс для сборка веб-страниц
    /// </summary>
    public static class WebpageBuilder
    {
        /// <summary>
        /// Установка тега DOCTYPE
        /// </summary>
        /// <param name="webPage"></param>
        /// <returns></returns>
        public static WebPage SetDoctype(this WebPage webPage)
        {
            webPage.Doctype = "<!DOCTYPE html>";
            return webPage;
        }
        /// <summary>
        /// Установка названия страницы
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        public static WebPage AddTitle(this WebPage webPage, string Title)
        {
            webPage.HeadContent += $"<title>{Title}</title>";
            return webPage;
        }
        /// <summary>
        /// Добавление стилей к странице
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="Styles"></param>
        /// <returns></returns>
        public static WebPage AddStyle(this WebPage webPage, string Styles)
        {
            webPage.HeadContent += $"<style>{Styles}</style>";
            return webPage;
        }
        /// <summary>
        /// Добавление ссылок на скрипты к странице
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="rel"></param>
        /// <param name="href"></param>
        /// <param name="integrity"></param>
        /// <param name="crossorigin"></param>
        /// <returns></returns>
        public static WebPage AddHeaderLink(this WebPage webPage, string rel, string href, string integrity, string crossorigin)
        {
            webPage.HeadContent += $"<link rel=\"{rel}\" href=\"{href}\" integrity=\"{integrity}\" crossorigin=\"{crossorigin}\">";
            return webPage;
        }
        /// <summary>
        /// Добавление тега header
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="headerText"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static WebPage AddBodyHeader(this WebPage webPage, string headerText, int size)
        {
            webPage.BodyContent += $"<h{size}>{headerText}</h{size}>";
            return webPage;
        }
        /// <summary>
        /// Добавление открывающего тега div
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static WebPage AddOpenDiv(this WebPage webPage, string tags)
        {
            webPage.BodyContent += $"<div class=\"{tags}\">";
            return webPage;
        }
        /// <summary>
        /// Добавление закрывающего тега div
        /// </summary>
        /// <param name="webPage"></param>
        /// <returns></returns>
        public static WebPage AddCloseDiv(this WebPage webPage)
        {
            webPage.BodyContent += $"</div>";
            return webPage;
        }
        /// <summary>
        /// Добавление таблицы с данными
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="Headers"></param>
        /// <param name="data"></param>
        /// <param name="tableClass"></param>
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
        /// Добавление тега p
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static WebPage AddParagraph(this WebPage webPage, string text)
        {
            webPage.BodyContent += $"<p>{text}</p>";
            return webPage;
        }
        /// <summary>
        /// Добавление тега ul
        /// </summary>
        /// <param name="webPage"></param>
        /// <returns></returns>
        public static WebPage AddOpenList(this WebPage webPage)
        {
            webPage.BodyContent += $"<ul>";
            return webPage;
        }
        /// <summary>
        /// Закрытие тега ul
        /// </summary>
        /// <param name="webPage"></param>
        /// <returns></returns>
        public static WebPage AddCloseList(this WebPage webPage)
        {
            webPage.BodyContent += $"</ul>";
            return webPage;
        }
        /// <summary>
        /// Добалвение элемента в список
        /// </summary>
        /// <param name="webPage"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static WebPage AddListElement(this WebPage webPage, string element)
        {
            webPage.BodyContent += $"<li>{element}</li>";
            return webPage;
        }
    }
}
