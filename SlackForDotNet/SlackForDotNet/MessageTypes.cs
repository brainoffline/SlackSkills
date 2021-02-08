using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

using SlackForDotNet.Surface;
#if SYSTEM_JSON
using System.Text.Json;
#else
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace SlackForDotNet
{
    public class MessageTypes
    {
        private static readonly ConcurrentDictionary< Type, SlackMessageAttribute > EventMessageTypes = new();
        //private static readonly ConcurrentDictionary< Type, SlackBlockAttribute >   BlockTypes        = new();

        internal static void RegisterAll()
        {
            // Check if already registered
            if (!EventMessageTypes.IsEmpty)
                return;

            var assembly = Assembly.GetAssembly(typeof(SlackMessage));
            var types = assembly!.ExportedTypes.Where(t => !t.IsAbstract).ToList();

            foreach (var type in types.Where(t => typeof(SlackMessage).IsAssignableFrom(t)))
                RegisterEventMessage(type);

            //foreach (var type in types.Where(t => typeof(Block).IsAssignableFrom(t)))
            //    RegisterBlock(type);
        }

        public static void RegisterEventMessage(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var messageType = typeInfo.GetCustomAttribute<SlackMessageAttribute>();

            if (messageType == null)
                return;

            try
            {
                EventMessageTypes.TryAdd(type, messageType);
            }
            catch
            {
                // Ignore exception.  Possible race condition
            }
        }

        //public static void RegisterBlock(Type type)
        //{
        //    var typeInfo = type.GetTypeInfo();

        //    foreach (var slackBlock in typeInfo.GetCustomAttributes<SlackBlockAttribute>())
        //    {
        //        try
        //        {
        //            BlockTypes.TryAdd(type, slackBlock);
        //        }
        //        catch
        //        {
        //            // Ignore exception.  Possible race condition
        //        }
        //    }
        //}

        public static SlackMessageAttribute? GetMessageAttributes( Type type )
        {
            if (EventMessageTypes.TryGetValue(type, out var messageType))
                return messageType;
            
            if (typeof(SlackMessage).IsAssignableFrom( type ))
            {
                RegisterEventMessage(type);
                return EventMessageTypes[type];
            }

            return null;
        }

        public static SlackMessageAttribute GetMessageAttributes<T>(T? value = default) where T : SlackMessage
        {
            var type = value?.GetType() ?? typeof(T);
            if (EventMessageTypes.TryGetValue(type, out var messageType))
                return messageType;

            RegisterEventMessage(type);
            return EventMessageTypes[type];
        }

        public static (Type, SlackMessageAttribute) GetMessageAttributes(
            [NotNull] string type, 
            string? subType = null)
        {
            var pair = EventMessageTypes.FirstOrDefault(mt => mt.Value.Type == type && mt.Value.SubType == subType);
            return (pair.Key, pair.Value);
        }

        //public static SlackBlockAttribute GetBlockAttributes<T>(T? value = default) where T : Block
        //{
        //    var type = value?.GetType() ?? typeof(T);
        //    if (BlockTypes.TryGetValue(type, out var blockAttribute))
        //        return blockAttribute;

        //    RegisterBlock(type);
        //    return BlockTypes[type];
        //}

        //public static (Type, SlackBlockAttribute) GetBlockAttributes([NotNull] string type)
        //{
        //    var pair = BlockTypes.FirstOrDefault(mt => mt.Value.Type == type);
        //    return (pair.Key, pair.Value);
        //}

        internal static readonly SlackMessageConverter MessageConverter = new ();
        //internal static readonly SlackBlockConverter   BlockConverter   = new ();
        internal static readonly SurfaceTextConverter    TextConverter    = new();
        internal static readonly ElementConverter        ElementConverter = new();
        internal static readonly LayoutConverter         LayoutConverter  = new();
        internal static readonly ActionResponseConverter ActionResponseConverter  = new();
        public static SlackMessage? Expand( string json )
        {
            return (SlackMessage?)JsonConvert.DeserializeObject( 
                    json, 
                    typeof(SlackMessage), 
                    MessageConverter, TextConverter, ElementConverter, LayoutConverter, ActionResponseConverter);
        }
    }

#if SYSTEM_JSON
#else

   
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
            var jo = JObject.ReadFrom( reader );

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

    //public class SlackBlockConverter : JsonConverter<Block?>
    //{
    //    public override bool CanRead =>
    //        true;

    //    public override bool CanWrite =>
    //        false;

    //    public override void WriteJson(JsonWriter writer, Block? value, JsonSerializer serializer)
    //    {
    //        throw new Exception($"Unable to write SlackMessage");
    //    }

    //    public override Block? ReadJson(
    //        JsonReader     reader,
    //        Type           objectType,
    //        Block?         existingValue,
    //        bool           hasExistingValue,
    //        JsonSerializer serializer)
    //    {
    //        var jo = JObject.ReadFrom(reader);

    //        var type    = jo[nameof(SlackMessage.type)]?.Value<string>();

    //        if (!string.IsNullOrWhiteSpace(type))
    //        {
    //            var (classType, attrs) = MessageTypes.GetBlockAttributes( type );
    //            if (classType != null)
    //                objectType = classType;
    //        }

    //        var obj = (Block?)Activator.CreateInstance(objectType);
    //        if (obj != null)
    //            serializer.Populate(jo.CreateReader(), obj);
    //        return obj;
    //    }

    //}
#endif

}
