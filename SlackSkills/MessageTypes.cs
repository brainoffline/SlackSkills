using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using SlackSkills.Surface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlackSkills
{
    public class MessageTypes
    {
        private static readonly ConcurrentDictionary< Type, SlackMessageAttribute > EventMessageTypes = new();

        internal static void RegisterAll()
        {
            // Check if already registered
            if (!EventMessageTypes.IsEmpty)
                return;

            var assembly = Assembly.GetAssembly(typeof(SlackMessage));
            var types = assembly!.ExportedTypes.Where(t => !t.IsAbstract).ToList();

            foreach (var type in types.Where(t => typeof(SlackMessage).IsAssignableFrom(t)))
                RegisterEventMessage(type);
        }

        public static SlackMessageAttribute? RegisterEventMessage(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var messageType = typeInfo.GetCustomAttribute<SlackMessageAttribute>();

            if (messageType == null)
                return null;

            try
            {
                EventMessageTypes.TryAdd(type, messageType);
            }
            catch
            {
                // Ignore exception.  Possible race condition
            }

            return messageType;
        }

        public static SlackMessageAttribute? GetMessageAttributes( Type type )
        {
            if (EventMessageTypes.TryGetValue(type, out var messageType))
                return messageType;
            
            if (typeof(SlackMessage).IsAssignableFrom( type ))
            {
                return RegisterEventMessage(type);
            }

            return null;
        }

        public static SlackMessageAttribute? GetMessageAttributes<T>(T? value = default) where T : SlackMessage
        {
            var type = value?.GetType() ?? typeof(T);
            if (EventMessageTypes.TryGetValue(type, out var messageType))
                return messageType;

            return RegisterEventMessage(type);
        }

        public static (Type, SlackMessageAttribute) GetMessageAttributes(
            string type, 
            string? subType = null)
        {
            var pair = EventMessageTypes.FirstOrDefault(mt => mt.Value.Type == type && mt.Value.SubType == subType);
            return (pair.Key, pair.Value);
        }

        internal static readonly SlackMessageConverter   MessageConverter        = new();
        internal static readonly SurfaceTextConverter    TextConverter           = new();
        internal static readonly ElementConverter        ElementConverter        = new();
        internal static readonly LayoutConverter         LayoutConverter         = new();
        internal static readonly ActionResponseConverter ActionResponseConverter = new();
        public static SlackMessage? Expand( string json )
        {
            return (SlackMessage?)JsonConvert.DeserializeObject( 
                    json, 
                    typeof(SlackMessage), 
                    MessageConverter, 
                    TextConverter, 
                    ElementConverter, 
                    LayoutConverter, 
                    ActionResponseConverter);
        }
    }
    
    public class SlackMessageConverter : JsonConverter<SlackMessage?>
    {
        public override bool CanRead =>
            true;

        public override bool CanWrite =>
            false;
        
        public override void WriteJson(JsonWriter writer, SlackMessage? value, JsonSerializer serializer)
        {
            throw new Exception($"Unable to write SlackMessage");
        }

        public override SlackMessage? ReadJson(
            JsonReader reader,
            Type objectType,
            SlackMessage? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jo = JToken.ReadFrom( reader );

            if (!jo.HasValues)
                return default;

            var type    = jo[nameof(SlackMessage.type)]?.Value<string>();
            var subtype = jo[nameof(SlackMessage.subtype)]?.Value<string>();

            if (!string.IsNullOrWhiteSpace( type ))
            {
                var (classType, attrs) = MessageTypes.GetMessageAttributes( type, subtype );
                if (classType != null)
                    objectType = classType;
            }

            var obj = (SlackMessage?)Activator.CreateInstance( objectType );
            if (obj != null)
                serializer.Populate( jo.CreateReader(), obj );
            return obj;
        }

    }
}
