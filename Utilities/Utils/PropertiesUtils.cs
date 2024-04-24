using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Singer.Utilities.Utils;

public class PropertiesUtils
{
    private static readonly IConfiguration Configuration;

    static PropertiesUtils()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Configuration = builder.Build();
    }

    public static string GetMessages()
    {
        return Configuration["EnvironmentVariables:Messages"];
    }

    public static JObject GetConfig()
    {
        string configString = Configuration["EnvironmentVariables:Config"];
        return JObject.Parse(configString);
    }

    public static string VersionBase64()
    {
        string os = GetOs();
        string type = "LIBRERIA";
        string version = (string)GetConfig()["Version"];
        string hash = "sha12313";

        string jsonVersion = JsonSerializer.Serialize(new
        {
            os,
            type,
            version,
            hash
        });

        string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonVersion));
        return base64;
    }

    private static string GetOs()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "Windows";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "macOS";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "Linux";
        }
        else
        {
            return "Unknown";
        }
    }
}
