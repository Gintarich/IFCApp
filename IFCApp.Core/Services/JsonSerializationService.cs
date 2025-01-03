using IFCApp.Core.Elements;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IFCApp.Core.Services
{
    public class JsonSerializationService
    {
        private JsonSerializerOptions _options;
        private readonly string _path;
        private string json;

        public JsonSerializationService(string path)
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new ElementBaseConverter(), new ModelConverter() },
                WriteIndented = true
            };
            _path = path;
        }
        public void Write(Model model)
        {
            json = JsonSerializer.Serialize(model, _options);
        }
        public Model Read()
        {
            var model = JsonSerializer.Deserialize<Model>(json, _options);
            return model;
        }
    }

    // Custom converter for polymorphic serialization
    public class ElementBaseConverter : JsonConverter<ElementBase>
    {
        public override ElementBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var root = jsonDoc.RootElement;
                var type = root.GetProperty("Type").GetString();

                ElementBase? value = type switch
                {
                    nameof(SandwichPanel) => JsonSerializer.Deserialize<SandwichPanel>(root.GetRawText(), options),
                    nameof(WallPanel) => JsonSerializer.Deserialize<WallPanel>(root.GetRawText(), options),
                    nameof(Door) => JsonSerializer.Deserialize<Door>(root.GetRawText(), options),
                    nameof(Window) => JsonSerializer.Deserialize<Window>(root.GetRawText(), options),
                    _ => throw new NotSupportedException($"Type {type} is not supported")
                };
                return value;
            }
        }

        public override void Write(Utf8JsonWriter writer, ElementBase value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // Include the type information
            writer.WriteString("Type", value.GetType().Name);

            // Serialize all properties of the object
            foreach (var property in value.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(value);
                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, propertyValue, propertyValue?.GetType() ?? typeof(object), options);
            }

            writer.WriteEndObject();
        }
    }
    public class ModelConverter : JsonConverter<Model>
    {
        public override Model Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = new Model();
            var jsonDoc = JsonDocument.ParseValue(ref reader);

            if (jsonDoc.RootElement.TryGetProperty("Elements", out var elementsJson))
            {
                model.Elements = JsonSerializer.Deserialize<List<ElementBase>>(elementsJson.GetRawText(), options);
            }

            return model;
        }

        public override void Write(Utf8JsonWriter writer, Model value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Elements");
            JsonSerializer.Serialize(writer, value.Elements, options);
            writer.WriteEndObject();
        }
    }
    public class OpeningConverter : JsonConverter<Opening>
    {
        public override Opening? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Opening value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
