using System.Text.Json;

namespace Tests.BackRoll.Telegram.Integration.TestsInfrastructure
{
    public static class TestsHelper
    {
        public static string SerializePretty<T>(T value)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            return JsonSerializer.Serialize(value, options);
        }
    }
}
