#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;

public static class SDKEnumGenerator
{
    private const string EnumName = "TypeSDK";
    private const string FilePath = "Assets/Scripts/SDKIntegration/View/TypeSDK.cs";
    
    [InitializeOnLoadMethod]
    private static void GenerateSDKEnum()
    {
        var adapterTypes = TypeCache.GetTypesDerivedFrom<AbstractSDKAdapter>()
            .Where(t => !t.IsAbstract)
            .OrderBy(t => t.Name)
            .ToList();

        if (!adapterTypes.Any()) return;

        using (var stream = new StreamWriter(FilePath))
        {
            stream.WriteLine("public enum " + EnumName);
            stream.WriteLine("{");
            
            for (int i = 0; i < adapterTypes.Count; i++)
            {
                var typeName = adapterTypes[i].Name.Replace("SDKAdapter", "");
                stream.WriteLine($"    {typeName} = {i},");
            }
            
            stream.WriteLine("}");
        }
        
        AssetDatabase.Refresh();
    }
}
#endif