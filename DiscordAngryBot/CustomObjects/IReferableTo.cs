namespace DiscordAngryBot.CustomObjects
{
    /// <summary>
    /// Interface for object that are referable
    /// </summary>
    /// <typeparam name="T">Reference type</typeparam>
    public interface IReferableTo<T>
    {
        /// <summary>
        /// Get object reference
        /// </summary>
        /// <returns></returns>
        T GetReference();
    }
}
