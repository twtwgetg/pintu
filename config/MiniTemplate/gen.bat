set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.
set OUTPUT_DATA_DIR=..\..\client\Assets\data
set OUTPUT_CODE_DIR=..\..\client\Assets\Scripts\Config

REM 创建输出目录
if not exist %OUTPUT_DATA_DIR% mkdir %OUTPUT_DATA_DIR%
if not exist %OUTPUT_CODE_DIR% mkdir %OUTPUT_CODE_DIR%

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%OUTPUT_DATA_DIR% ^
    -x outputCodeDir=%OUTPUT_CODE_DIR%

pause