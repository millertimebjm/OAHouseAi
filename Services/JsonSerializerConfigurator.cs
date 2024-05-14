using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// Assuming you have a method or class where you configure JsonSerializerOptions
public class JsonSerializerConfigurator
{
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        // Create an instance of JsonSerializerOptions
        var options = new JsonSerializerOptions();

        // Configure the DefaultIgnoreCondition to allow reflection-based serialization
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;

        return options;
    }
}