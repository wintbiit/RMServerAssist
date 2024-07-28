using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Swan.Formatters;

namespace RMServerAssist;

public static class ProtoDumper
{
    public static void DumpProtoId(string filePath)
    {
        // get private static Dictionary<int, Type> sProtoTypeMap of static class S1ProtoDef
        var sProtoTypeMap = typeof(S1ProtoDef).GetField("sProtoTypeMap", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<int, Type>;
        
        // reverse to Name-Value pair
        var protoTypeMap = new Dictionary<string, int>();
        foreach (var pair in sProtoTypeMap)
        {
            protoTypeMap.Add(pair.Value.Name, pair.Key);
        }
        
        CsvWriter.SaveRecords(protoTypeMap, filePath);
    }

    public static void DumpBattleProtoId(string filePath)
    {
        // get all elements in public enum enumS1BattleProtoID
        var s1BattleProtoID = Enum.GetValues(typeof(enumS1BattleProtoID));
        
        // convert to Name-Value pair
        var s1BattleProtoIDDict = new Dictionary<string, int>();
        foreach (var value in s1BattleProtoID)
        {
            s1BattleProtoIDDict.Add(value.ToString(), (int)value);
        }
        
        CsvWriter.SaveRecords(s1BattleProtoIDDict, filePath);
    }

    public static async Task DumpProtoDef(string dir)
    {
        // find all classes inheriting from S1ProtoBase of all loaded assemblies
        var protoTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && type.IsSubclassOf(typeof(S1ProtoBase)));
        
        // write proto def to file
        var types = await Task.WhenAll(protoTypes.Select(type => Task.Run(() => WriteProtoDef(dir, type))));
        
        var typesSet = new SortedSet<string>();
        foreach (var type in types)
        {
            typesSet.UnionWith(type);
        }
        
        // write proto def used Types
        await using var stream = File.Create(Path.Combine(dir, "types.txt"));
        await using var writer = new StreamWriter(stream);
        
        foreach (var type in typesSet)
        {
            await writer.WriteLineAsync(type);
        }
        
        var sizes = protoTypes.ToDictionary(type => type.Name, GetProtoSize);
        CsvWriter.SaveRecords(sizes, Path.Combine(dir, "sizes.csv"));
    }

    private static async Task<ISet<string>> WriteProtoDef(string dir, Type type)
    {
        var types = new HashSet<string>();
        await using var stream = File.Create(Path.Combine(dir, $"{type.Name}.csv"));
        
        await using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync("Field,Type");
        
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        { 
            await writer.WriteLineAsync($"{field.Name},{field.FieldType.Name}");
            types.Add(field.FieldType.Name);
        }
        
        await writer.FlushAsync();
        
        Console.WriteLine($"Proto def of {type.Name} has been written to {dir}");
        
        return types;
    }

    private static int GetProtoSize(Type type)
    {
        // S1ProtoBase has a virtual method GetSize() that returns its size
        var getSizeMethod = type.GetMethod("GetSize", BindingFlags.Public | BindingFlags.Instance);
        var size = 0;
        try
        {
            size = (int)getSizeMethod.Invoke(Activator.CreateInstance(type), null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return size;
    }
}