

using System.Reflection;
using System.Runtime.Loader;
using Puerts;

[Configure]
public class ExamplesCfg
{
    [Binding]
    static IEnumerable<Type> Bindings
    {
        get
        {
            return
            [
                typeof(UnityEngine.Vector2),
                typeof(UnityEngine.Vector3),
                typeof(UnityEngine.Quaternion),
                typeof(UnityEngine.GameObject),
                typeof(UnityEngine.Object),
                typeof(UnityEngine.Transform),
                typeof(UnityEngine.Component),
                typeof(UnityEngine.Input),
                typeof(UnityEngine.Time),
                typeof(UnityEngine.Cursor),

                typeof(System.IO.Directory),
                typeof(System.IO.File),

                // 游戏相关
                typeof(GameMain),
                typeof(CameraMain),
                typeof(YotogiManager),
                typeof(YotogiPlayManager),
                typeof(Maid),
                typeof(TBody),
                typeof(TBodySkin),

                // Mod
                typeof(BepInEx.BaseUnityPlugin),
                typeof(BepInEx.Configuration.ConfigFile),
                typeof(BepInEx.Configuration.ConfigEntry<>),
                typeof(BepInEx.Configuration.ConfigDefinition),
                typeof(Com3d2Mod.Plugin),
            ];
        }
    }
}

public class Gen
{
    private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        // 当需要解析的程序集名称匹配 "UnityEngine" 时
        if (args.Name.StartsWith("UnityEngine, Version=0.0.0.0"))
        {
            try
            {
                string assemblyPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "UnityEngine.dll"
                );

                if (File.Exists(assemblyPath))
                {
                    Console.WriteLine($"[AssemblyResolve] 加载: {assemblyPath}");
                    return Assembly.LoadFile(assemblyPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AssemblyResolve] 加载失败: {ex.Message}");
            }
        }

        // 对于其他程序集，返回 null 以使用默认解析逻辑
        return null;
    }

    const string saveTo = "../../../../Com3d2Mod/Gen/StaticWrapper/";
    const string dtsTo = "../../../../TypeScripts/";
    public static void Main()
    {
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        ClearAll();
        GenerateCode();
        GenerateDTS();
    }

    public static void GenerateDTS()
    {
        var start = DateTime.Now;
        Directory.CreateDirectory(dtsTo);
        Directory.CreateDirectory(Path.Combine(dtsTo, "Typing/csharp"));
        Puerts.Editor.Generator.FileExporter.ExportDTS(dtsTo);
        Console.WriteLine("GenerateDTS finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
    }

    public static void GenerateCode()
    {
        var start = DateTime.Now;
        Directory.CreateDirectory(saveTo);

        Puerts.Editor.Generator.FileExporter.ExportWrapper(saveTo);
        Console.WriteLine("ExportWrapper finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");

        GenRegisterInfo();
        Console.WriteLine("GenerateCode finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms");
    }

    public static void ClearAll()
    {
        if (Directory.Exists(saveTo))
        {
            Directory.Delete(saveTo, true);
        }
    }

    public static void GenRegisterInfo()
    {
        var start = DateTime.Now;
        Directory.CreateDirectory(saveTo);
        Puerts.Editor.Generator.FileExporter.GenRegisterInfo(saveTo);
        Console.WriteLine("GenRegisterInfo finished! use " + (DateTime.Now - start).TotalMilliseconds + " ms Outputed to " + saveTo);
    }
}