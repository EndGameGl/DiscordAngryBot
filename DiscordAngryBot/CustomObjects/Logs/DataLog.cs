using System;

namespace DiscordAngryBot.CustomObjects.Logs
{
    /// <summary>
    /// Class for storing logs
    /// </summary>
    public class DataLog
    {
        /// <summary>
        /// Message passed to logs, if any
        /// </summary>
        private string _message;
        /// <summary>
        /// Short summary for event that took place
        /// </summary>
        public string LogInfo { get; private set; }
        /// <summary>
        /// Exception, if catched any
        /// </summary>
        public Exception Exception { get; private set; }
        /// <summary>
        /// Message for display
        /// </summary>
        public string Message 
        { 
            get 
            {
                if (Exception != null)
                {
                    return Exception.Message;
                }
                else
                    return _message;
            } 
            private set 
            { } 
        }
        /// <summary>
        /// Timestamp for this log
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Data log constructor
        /// </summary>
        /// <param name="exception">Exception to log</param>
        /// <param name="logInfo">Info about log</param>
        /// <param name="message">Message to log</param>
        public DataLog(Exception exception, string logInfo, string message = "")
        {
            Exception = exception;
            LogInfo = logInfo;
            Time = DateTime.Now;
            _message = message;
        }
    }
}
