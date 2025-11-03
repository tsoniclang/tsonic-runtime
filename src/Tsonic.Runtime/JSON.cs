/**
 * JavaScript JSON object implementation
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Tsonic.Runtime
{
    /// <summary>
    /// JSON parsing and stringification (AOT-friendly, no reflection)
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// Parse JSON string to object (returns JS-shaped objects: DynamicObject, Array, primitives)
        /// TypeScript treats JSON.parse as 'any', so generic T is for type hints only
        /// If T is a concrete type with a constructor, attempts structural cloning from DynamicObject to T
        /// </summary>
        public static T parse<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string text) where T : new()
        {
            using var doc = JsonDocument.Parse(text);
            var result = ConvertJsonElement(doc.RootElement);

            // If T is object or dynamic, return as-is
            if (typeof(T) == typeof(object))
            {
                return (T)(object?)result!;
            }

            // For concrete types, try structural cloning from DynamicObject to T
            if (result is DynamicObject dynObj)
            {
                var tempDict = new Dictionary<string, object?>();
                foreach (var key in dynObj.GetKeys())
                {
                    tempDict[key] = dynObj[key];
                }
                return Structural.CloneFromDictionary<T>(tempDict)!;
            }

            // For primitives and arrays, direct cast
            return (T)(object?)result!;
        }

        /// <summary>
        /// Convert JsonElement to runtime objects
        /// </summary>
        private static object? ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Number => element.GetDouble(),
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Array => ConvertJsonArray(element),
                JsonValueKind.Object => ConvertJsonObject(element),
                _ => null
            };
        }

        /// <summary>
        /// Convert JSON array to Tsonic.Runtime.Array
        /// </summary>
        private static object ConvertJsonArray(JsonElement element)
        {
            var items = new List<object?>();
            foreach (var item in element.EnumerateArray())
            {
                items.Add(ConvertJsonElement(item));
            }
            var array = new Array<object?>();
            for (var i = 0; i < items.Count; i++)
            {
                array[i] = items[i];
            }
            return array;
        }

        /// <summary>
        /// Convert JSON object to DynamicObject
        /// </summary>
        private static object ConvertJsonObject(JsonElement element)
        {
            var obj = new DynamicObject();
            foreach (var prop in element.EnumerateObject())
            {
                obj[prop.Name] = ConvertJsonElement(prop.Value);
            }
            return obj;
        }

        /// <summary>
        /// Convert object to JSON string (handles primitives, Array, DynamicObject, IDictionary, IEnumerable)
        /// </summary>
        public static string stringify(object? value)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            WriteValue(writer, value);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Write value to Utf8JsonWriter
        /// </summary>
        private static void WriteValue(Utf8JsonWriter writer, object? value)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    break;
                case bool b:
                    writer.WriteBooleanValue(b);
                    break;
                case string s:
                    writer.WriteStringValue(s);
                    break;
                case double d:
                    writer.WriteNumberValue(d);
                    break;
                case float f:
                    writer.WriteNumberValue(f);
                    break;
                case int i:
                    writer.WriteNumberValue(i);
                    break;
                case long l:
                    writer.WriteNumberValue(l);
                    break;
                case uint ui:
                    writer.WriteNumberValue(ui);
                    break;
                case byte bt:
                    writer.WriteNumberValue(bt);
                    break;
                case short sh:
                    writer.WriteNumberValue(sh);
                    break;
                case DynamicObject dynObj:
                    WriteDynamicObject(writer, dynObj);
                    break;
                case IDictionary<string, object?> dict:
                    WriteObject(writer, dict);
                    break;
                case IEnumerable<object?> enumerable:
                    WriteArray(writer, enumerable);
                    break;
                default:
                    // For unknown types (regular C# objects), convert to dictionary first
                    var objDict = Structural.ToDictionary(value);
                    WriteObject(writer, objDict);
                    break;
            }
        }

        /// <summary>
        /// Write DynamicObject as JSON object
        /// </summary>
        private static void WriteDynamicObject(Utf8JsonWriter writer, DynamicObject obj)
        {
            writer.WriteStartObject();
            foreach (var key in obj.GetKeys())
            {
                writer.WritePropertyName(key);
                WriteValue(writer, obj[key]);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write dictionary as JSON object
        /// </summary>
        private static void WriteObject(Utf8JsonWriter writer, IDictionary<string, object?> dict)
        {
            writer.WriteStartObject();
            foreach (var kvp in dict)
            {
                writer.WritePropertyName(kvp.Key);
                WriteValue(writer, kvp.Value);
            }
            writer.WriteEndObject();
        }

        /// <summary>
        /// Write enumerable as JSON array
        /// </summary>
        private static void WriteArray(Utf8JsonWriter writer, IEnumerable enumerable)
        {
            writer.WriteStartArray();
            foreach (var item in enumerable)
            {
                WriteValue(writer, item);
            }
            writer.WriteEndArray();
        }
    }
}
