using Newtonsoft.Json;

namespace Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );
        }
    }
}