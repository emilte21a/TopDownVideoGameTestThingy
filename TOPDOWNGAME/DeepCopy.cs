
public static class DeepCopyExtensions
{
    public static T DeepCopy<T>(this T self)
    {
        var json = JsonSerializer.Serialize(self);
        return JsonSerializer.Deserialize<T>(json);
    }

    // public static T DeepClone<T>(this T obj)
    // {
    //     using (var ms = new MemoryStream())
    //     {
    //         var formatter = new BinaryFormatter();
    //         formatter.Serialize(ms, obj);
    //         ms.Position = 0;

    //         return (T)formatter.Deserialize(ms);
    //     }
    // }

    
}