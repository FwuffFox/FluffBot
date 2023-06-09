namespace FluffBot.Extensions;

public static class Extensions
{
    public static T With<T>(this T obj, Action<T> action)
    {
        action.Invoke(obj);
        return obj;
    }
    
    public static T With<T>(this T obj, bool pred, Action<T> action)
    {
        if (pred) action.Invoke(obj);
        return obj;
    }
}