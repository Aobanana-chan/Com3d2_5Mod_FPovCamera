using System;
using System.IO;
using System.Runtime.InteropServices;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Puerts;
using UnityEngine;
namespace Com3d2Mod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public Action onTriggerPov;
        public static Action onWorldReset;

        private ConfigEntry<KeyboardShortcut> TriggerPovKey { get; set; }

        JsEnv jsEnv;
        private readonly Harmony harmony = new(MyPluginInfo.PLUGIN_GUID);

        FileSystemWatcher watcher;

        bool needReload = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loading!");
            harmony.PatchAll();

            SetLoadDependency();
#if Dev
            jsEnv = new JsEnv(new DefaultLoader(Path.Combine(Directory.GetCurrentDirectory(), "Com3d2Mod/TypeScripts/")), 16333);
            var mainFunc = jsEnv.ExecuteModule<Action>("main.mjs", "main");
            mainFunc?.Invoke();
            SetHotReload();
#else
            jsEnv = new JsEnv(new DefaultLoader(Path.Combine(Directory.GetCurrentDirectory(), "BepInEx/plugins/Com3d2Mod/dist")));
            var mainFunc = jsEnv.ExecuteModule<Action>("bundle.js", "main");
            mainFunc?.Invoke();
#endif
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        void Start()
        {
            TriggerPovKey = Config.Bind("Hotkeys", "TriggerPovKey", new KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftControl), "按键：切换视角");
        }

        void Update()
        {
            if (needReload)
            {
                needReload = false;
                OnHotReload();
            }
            jsEnv.Tick();
            if (TriggerPovKey.Value.IsDown())
            {
                onTriggerPov?.Invoke();
            }

            jsEnv.ExecuteModule<Action>("ticker.mjs", "update")?.Invoke();
        }

        void LateUpdate()
        {
            jsEnv.ExecuteModule<Action>("ticker.mjs", "lateUpdate")?.Invoke();
        }

        void SetLoadDependency()
        {
            NativeLoader.SetDllDirectory(Path.Combine(Directory.GetCurrentDirectory(), "BepInEx\\plugins\\Com3d2Mod\\"));
        }

        void OnDestroy()
        {
            jsEnv.Dispose();
            harmony.UnpatchSelf();
            watcher?.Dispose();
        }

        void SetHotReload()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Com3d2Mod/TypeScripts/");
            // 创建 FileSystemWatcher 实例
            watcher = new()
            {
                Path = folderPath,

                // 2. 设置监控的过滤器：仅.js文件
                Filter = "*.mjs",

                // 3. 设置监控的子目录（可选）
                IncludeSubdirectories = true, // 设置为 false 则只监控当前目录

                // 4. 启用监控的事件类型
                NotifyFilter = NotifyFilters.LastWrite // 文件内容最后写入时间
                                     | NotifyFilters.FileName   // 文件名
                                     | NotifyFilters.DirectoryName // 目录名
            };

            // 5. 绑定事件处理程序
            // 文件/目录重命名
            watcher.Renamed += (_, _) => { needReload = true; };
            // 文件/目录创建、删除、修改等
            watcher.Changed += (_, _) => { needReload = true; };
            watcher.Created += (_, _) => { needReload = true; };
            watcher.Deleted += (_, _) => { needReload = true; };

            // 6. 开始监控
            watcher.EnableRaisingEvents = true;
        }

        void OnHotReload()
        {
            onTriggerPov = null;
            onWorldReset?.Invoke();
            onWorldReset = null;
            Logger.LogInfo("文件变动，正在热重载...");
            jsEnv.ClearModuleCache();
            var mainFunc = jsEnv.ExecuteModule<Action>("main.mjs", "main");
            Logger.LogInfo("执行");
            mainFunc.Invoke();
            Logger.LogInfo("热重载完成！");
        }

        [HarmonyPatch]
        public static class YotogiManagerPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(YotogiManager), "ResetWorld")]
            public static void Prefix_ResetWorld()
            {
                try
                {
                    Plugin.onWorldReset?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[补丁] ResetWorld 前缀执行出错: {ex}");
                }
            }

            // 补丁 OnDestroy 方法（前缀补丁）
            [HarmonyPrefix]
            [HarmonyPatch(typeof(YotogiManager), "OnDestroy")]
            public static void Prefix_OnDestroy()
            {
                try
                {
                    Plugin.onWorldReset?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[补丁] OnDestroy 前缀执行出错: {ex}");
                }
            }
        }
    }

}
