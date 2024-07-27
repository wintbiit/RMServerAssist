using System.IO;
using System.Reflection;

namespace RMServerAssist;

public static class ResourceHelper
{
    public static string GetEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null) return null;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}