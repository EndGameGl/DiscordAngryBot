using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Класс, содержащий параметры для запуска команды дискорда
    /// </summary>
    public class DiscordCommandParameterSet
    {
        /// <summary>
        /// Список параметров
        /// </summary>
        private List<object> parameterList { get; set; }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="parameter"></param>
        public DiscordCommandParameterSet(object parameter)
        {
            parameterList = new List<object>();
            parameterList.Add(parameter);
        }
        /// <summary>
        /// Добавление нового параметра в список
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public DiscordCommandParameterSet AddParameter(object parameter)
        {
            if (parameter != null)
                parameterList.Add(parameter);
            return this;
        }
        /// <summary>
        /// Получение массива параметров для метода
        /// </summary>
        /// <returns></returns>
        public object[] GetParameters(int amountOfParameters)
        {
            if (amountOfParameters > parameterList.Count)
            {
                for (int i=0; i < amountOfParameters - parameterList.Count; i++)
                {
                    parameterList.Add(null);
                }
                return parameterList.ToArray();
            }
            else
            {
                return parameterList.GetRange(0, amountOfParameters).ToArray();
            }          
        }
    }
}
