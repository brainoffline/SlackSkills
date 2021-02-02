using System;
using System.Text.Json;

namespace SlackForDotNet
{
    public static class JsonHelpers
    {
#if SYSTEM_JSON
        public static readonly JsonSerializerOptions DefaultJsonOptions = new()
                                                                           {
                                                                               IgnoreNullValues    = true,
                                                                               ReadCommentHandling = JsonCommentHandling.Skip,
                                                                               WriteIndented       = true
                                                                           };
#endif
    }
}
