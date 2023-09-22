using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.VisualBasic.FileIO;
using Otus_prof_homework_13_Reflection;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Serialization;

new Bench().BinarySerialize();
new Bench().XmlSerialize();
new Bench().JsonSerialize();

BenchmarkRunner.Run<Bench>();

[SimpleJob(RuntimeMoniker.Net70, 10, 10, 10, 3)]
public class Bench
{
    [Benchmark]
    public void BinarySerialize()
    {
        Type fieldsType = typeof(Foo);
        FieldInfo[] fields = fieldsType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        IEnumerable<Foo> foos = GetFoos();

        using var fs = new FileStream("bin.bin", FileMode.Create);
        using var bw = new BinaryWriter(fs, Encoding.UTF8, false);
        foreach (var item in foos)
        {
            for (int j = 0; j < fields.Length; j++)
            {
                bw.Write((int)fields[j].GetValue(item)!);
            }
        }
    }

    [Benchmark]
    public void BinaryDeserialize()
    {
        Type fieldsType = typeof(Foo);
        FieldInfo[] fields = fieldsType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        using var fs = new FileStream("bin.bin", FileMode.Open);
        using var br = new BinaryReader(fs, Encoding.UTF8);
        List<Foo> foosDes = new();
        while (br.PeekChar() > -1)
        {
            Foo fooDes = new();
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i].SetValue(fooDes, br.ReadInt32());
            }
            foosDes.Add(fooDes);
        }
    }

    [Benchmark]
    public void XmlSerialize()
    {
        IEnumerable<Foo> foos = GetFoos();

        XmlSerializer ser = new(typeof(Foo[]));

        using FileStream fs = new("MyXml.xml", FileMode.Create);
        ser.Serialize(fs, foos.ToArray());
    }

    [Benchmark]
    public void XmlDeserialize()
    {
        XmlSerializer ser = new(typeof(Foo[]));

        using FileStream fsRead = new("MyXml.xml", FileMode.Open);
        var foos = (IEnumerable<Foo>)ser.Deserialize(fsRead)!;
    }

    [Benchmark]
    public void JsonSerialize()
    {
        var opt = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
        };
        var s = JsonSerializer.Serialize(GetFoos(), opt);
        File.WriteAllText("MyJson.json", s);
    }

    [Benchmark]
    public void JsonDeserialize()
    {
        var file = File.ReadAllText("MyJson.json");
        var foos = JsonSerializer.Deserialize<IEnumerable<Foo>>(file)!;
    }

    private IEnumerable<Foo> GetFoos()
    {
        Foo foo = new();
        return Enumerable.Range(1, 10000).Select(c => foo.Get());
    }    
}