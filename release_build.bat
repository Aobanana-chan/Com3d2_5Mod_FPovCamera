echo 正在执行 dotnet Release 构建...
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo dotnet 构建失败！错误代码: %errorlevel%
    pause
    exit /b %errorlevel%
)
echo dotnet 构建成功！

echo.
echo 正在执行 pnpm 构建...
call pnpm run build
if %errorlevel% neq 0 (
    echo pnpm 构建失败！错误代码: %errorlevel%
    pause
    exit /b %errorlevel%
)
echo pnpm 构建成功！


echo.
echo 正在复制 ./dist 文件夹...
if not exist ".\dist" (
    echo 错误：源文件夹 .\dist 不存在！
    pause
    exit /b 1
)

xcopy ".\dist" "..\BepInEx\plugins\Com3d2Mod\dist\" /s /e /y /q
if %errorlevel% neq 0 (
    echo 复制 ./dist 文件夹失败！错误代码: %errorlevel%
    pause
    exit /b %errorlevel%
)
echo ./dist 文件夹复制完成！

echo.
echo 正在复制 TypeScripts/puerts 文件夹...
if not exist "TypeScripts\puerts" (
    echo 错误：源文件夹 TypeScripts\puerts 不存在！
    pause
    exit /b 1
)

xcopy "TypeScripts\puerts" "..\BepInEx\plugins\Com3d2Mod\dist\puerts\" /s /e /y /q
if %errorlevel% neq 0 (
    echo 复制 TypeScripts/puerts 文件夹失败！错误代码: %errorlevel%
    pause
    exit /b %errorlevel%
)
echo TypeScripts/puerts 文件夹复制完成！

echo.
echo 正在复制 DLL 文件...

if exist "Com3d2Mod\bin\Release\net35\Com3d2Mod.dll" (
    copy "Com3d2Mod\bin\Release\net35\Com3d2Mod.dll" "..\BepInEx\plugins\Com3d2Mod\" /y
    echo Com3d2Mod.dll 复制完成
) else (
    echo 警告：Com3d2Mod.dll 不存在！
)

if exist "Com3d2Mod\bin\Release\net35\puerts.dll" (
    copy "Com3d2Mod\bin\Release\net35\puerts.dll" "..\BepInEx\plugins\Com3d2Mod\" /y
    echo puerts.dll 复制完成
) else (
    echo 警告：puerts.dll 不存在！
)

if exist "Com3d2Mod\bin\Release\net35\libnode.dll" (
    copy "Com3d2Mod\bin\Release\net35\libnode.dll" "..\BepInEx\plugins\Com3d2Mod\" /y
    echo libnode.dll 复制完成
) else (
    echo 警告：libnode.dll 不存在！
)

echo.
echo 所有任务执行完毕！
pause