using System.Collections.Generic;

namespace DiscordAngryBot.CustomObjects.DiscordCommands
{
    /// <summary>
    /// Class for holding parameters to run discord command
    /// </summary>
    public class DiscordCommandParameterSet
    {
        /// <summary>
        /// Parameter list
        /// </summary>
        private List<object> parameterList { get; set; }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="parameter">Initial parameter</param>
        public DiscordCommandParameterSet(object parameter)
        {
            parameterList = new List<object>();
            parameterList.Add(parameter);
        }

        /// <summary>
        /// Add new parameter to the list
        /// </summary>
        /// <param name="parameter">Additional parameter</param>
        /// <returns></returns>
        public DiscordCommandParameterSet AddParameter(object parameter)
        {
            if (parameter != null)
                parameterList.Add(parameter);
            return this;
        }

        /// <summary>
        /// Get all parameters as object array
        /// </summary>
        /// <param name="amountOfParameters">Required number of parameters</param>
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
