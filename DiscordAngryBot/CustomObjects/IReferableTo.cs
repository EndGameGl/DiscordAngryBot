namespace DiscordAngryBot.CustomObjects
{
    public interface IReferableTo<T>
    {
        T GetReference();
    }
}
