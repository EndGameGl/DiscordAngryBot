namespace DiscordAngryBot.CustomObjects
{
    /// <summary>
    /// Inteface for object that are loadable into original self
    /// </summary>
    /// <typeparam name="T">Type of origin</typeparam>
    public interface ILoadableInto<T>
    {
        /// <summary>
        /// Load into origin object
        /// </summary>
        /// <returns></returns>
        T LoadOrigin();
    }
}
