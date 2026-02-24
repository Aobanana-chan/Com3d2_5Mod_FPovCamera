# 简介 📖
Com3d2.5 女仆第一人称视角相机 Mod

主要尝试 **BepInEx + PuerTS** 组合开发，整体研究目的大于实际用途 🔬。看起来这样的技术栈可以用于很多其他有趣的用例。

例如，用 **BepInEx + PuerTS** 开发一个新的框架，之后的 Mod 都可以基于这个框架开发（全 Typescript 🤖），并且支持热更新，开发体验应该会比直接用 C# 开发要好很多。

魔改了小部分 PuerTS 代码（主要是类型导出相关）。

# 安装 📥
前往 Release 页面下载最新版本压缩包，然后解压到游戏目录（如果没有安装 BepInEx，则需要先安装 BepInEx）。

# 使用方法 🎮
在夜伽场景，按下 Ctrl + 1 即可切换到第一人称视角相机模式 🎥，此时按下鼠标右键可以让相机跟随鼠标移动 🖱️，并且 WASD 可以移动相机的位置 🏃，再次按下 Ctrl + 1 即可退出。

# 开发 🔧
~~虽然应该没人会继续开发这个 Mod 了…~~

1.  直接 Clone 项目到游戏目录下。
2.  执行 `pnpm install` 安装依赖。
3.  **GenCode** 项目主要用于生成 Typescript 类型定义文件和 StaticWrapper，可以用 `gencode.bat` 脚本执行生成。
4.  **Com3d2Mod** 是 BepInEx 插件主项目。

开发过程中：
-   执行 `debug_build.bat`，会将 Debug 版本的 dll 生成并复制到合适的位置。Debug 版本的 Dll 支持热重载 🔄。
-   可以运行 `pnpm run watch` 来监听文件变化，自动编译 TypeScript 脚本，更新并重新执行 `main.mts` 的 `main` 函数。

`release_build.bat` 用于构建最后的 Release 版本，会将脚本打包，并将 Release 版本的 dll 复制到合适的位置。

---

# Introduction 📖
Com3d2.5 First-Person Camera Mod for Maid Characters

This project primarily experiments with the **BepInEx + PuerTS** development stack. Its purpose is more about research than practical use, as this technology combination seems suitable for many other interesting applications 🔬.

For example, using **BepInEx + PuerTS** to develop a new framework would allow subsequent mods to be built entirely in TypeScript 🤖, supporting hot-reloading. The development experience should be significantly better than developing directly in C#.

A small portion of the PuerTS code has been modified (mainly related to type exporting).

# Installation 📥
Go to the Release page and download the latest version of the compressed package. Extract it to your game directory. (If BepInEx is not installed, you need to install it first.)

# Usage 🎮
In the night scene, press Ctrl + 1 to switch to first-person camera mode 🎥. Then, pressing the right mouse button allows the camera to follow the mouse movement 🖱️, and you can move the camera position with WASD 🏃. Press Ctrl + 1 again to exit.

# Development 🔧
~~Although it's unlikely anyone will continue developing this mod...~~

1.  Clone the project directly into your game directory.
2.  Run `pnpm install` to install dependencies.
3.  The **GenCode** project is mainly used to generate TypeScript type definition files and StaticWrappers. You can run the `gencode.bat` script to generate them.
4.  **Com3d2Mod** is the main BepInEx plugin project.

During development:
-   Run `debug_build.bat`. This will build and copy the Debug version of the DLL to the appropriate location. The Debug DLL supports hot-reloading 🔄.
-   You can use `pnpm run watch` to monitor file changes, automatically compile TypeScript scripts, and update and re-execute the `main` function in `main.mts`.

Use `release_build.bat` to build the final Release version. It will bundle the scripts and copy the Release version of the DLL to the appropriate location.