namespace Tamarix
{
    public class ResourceManager
    {
        /// <summary>
        /// Returns a manifest resource stream with the exact name
        /// </summary>
        /// <param name="ResourceHolder">The class that holds the resource</param>
        /// <param name="name">The resource name, e.g. 'Resources.file.png' will points to 'Resources/file.png'</param>
        /// <returns></returns>
        public static Stream? GetAsset(Type ResourceHolder, string name)
        {
            var assembly = ResourceHolder.Assembly;
            foreach (var n in assembly.GetManifestResourceNames())
            {
                if (n.EndsWith(name))
                    return assembly.GetManifestResourceStream(n);
            }
            throw new Exception($"Resource not found: {name}");
            return null;
        }

        public static string ReadAssetFile(Type ResourceHolder, string name)
        {
            var stream = GetAsset(ResourceHolder, name);
            if (stream == null)
            {
                throw new Exception($"Could not read resource: {name}");
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
            throw new Exception($"Can't read resource as string: {name}");
        }
    }
}
