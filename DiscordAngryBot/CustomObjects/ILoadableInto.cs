namespace DiscordAngryBot.CustomObjects
{
    public interface ILoadableInto<T>
    {
        T LoadOrigin();
    }
}
