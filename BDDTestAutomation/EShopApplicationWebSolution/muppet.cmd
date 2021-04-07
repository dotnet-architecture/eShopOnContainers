@echo off
setlocal disabledelayedexpansion
REM https://stackoverflow.com/a/40104177

:: MuPPET: Multi Platform Parallel Execution Tests
:: USAGE: muppet.cmd -config=Release -platforms="Safari.Mac;Chrome.Windows" -user=cloud_user -key=cloud_key

set "currentDir=%~dp0"
for %%x in (%currentDir%*.csproj) do if not defined app set "app=%%~nx"
set "filter=(TestCategory!=ignore&TestCategory!=manual&TestCategory!=localization&TestCategory=bvt&TestCategory=ui)"
set "config=Debug"
set "platforms=Edge.Windows;Safari.Mac"

:parseArgs
:: Asks for the -argument and store the value in the variable (Credit: https://stackoverflow.com/a/47169024)
call :getArgWithValue "-app" "app" "%~1" "%~2" && shift && shift && goto :parseArgs
call :getArgWithValue "-config" "config" "%~1" "%~2" && shift && shift && goto :parseArgs
call :getArgWithValue "-platforms" "platforms" "%~1" "%~2" && shift && shift && goto :parseArgs
call :getArgWithValue "-filter" "filter" "%~1" "%~2" && shift && shift && goto :parseArgs
call :getArgWithValue "-user" "user" "%~1" "%~2" && shift && shift && goto :parseArgs
call :getArgWithValue "-key" "key" "%~1" "%~2" && shift && shift && goto :parseArgs
call :getArgFlag "-debug" "VSTEST_RUNNER_DEBUG" "%~1" && shift && goto :parseArgs

:: Start
echo app: %app%
echo config: %config%
echo platforms: %platforms%
echo user: %user%
setlocal enabledelayedexpansion
echo filter: !filter!

goto runSettingFiles

:: This function sets a variable from a cli arg with value
:: 1 cli argument name
:: 2 variable name
:: 3 current Argument Name
:: 4 current Argument Value
:getArgWithValue
if "%~3"=="%~1" (
  if "%~4"=="" (
    REM unset the variable if value is not provided
    set "%~2="
    exit /B 1
  )
  set "%~2=%~4"
  exit /B 0
)
exit /B 1
goto:end

:: This function sets a variable to value "TRUE" from a cli "flag" argument
:: 1 cli argument name
:: 2 variable name
:: 3 current Argument Name
:getArgFlag
if "%~3"=="%~1" (
  set %~2=1
  exit /B 0
)
exit /B 1
goto:end

:runSettingFiles
echo Using runSetting files
for %%i in (%platforms%) do (
	echo %%i
	set "int=%%i"
	set "int=!int:.= !"
	for /f "tokens=1,2 delims= " %%a in ("!int!") do (
		echo CrossBrowserEnvironment=%%a%%b

		REM Credit: https://www.dostips.com/forum/viewtopic.php?t=6044
		call %currentDir%jrepl.bat "(name=\qappSettings\.browser\q value=\q)(.*?)(\q)" "$1RemoteWebDriver$3" /x /f "%currentDir%%app%.runsettings" /o "%currentDir%%app%.Remote.%%a.%%b.runsettings"
		call %currentDir%jrepl.bat "(name=\qappSettings\.DriverCapabilities\q value=\q)(.*?)(\q)" "$1CloudProvider$3" /x /f "%currentDir%%app%.Remote.%%a.%%b.runsettings" /o -
		call %currentDir%jrepl.bat "(name=\qappSettings\.CrossBrowserEnvironment\q value=\q)(.*?)(\q)" "$1%%a%%b$3" /x /f "%currentDir%%app%.Remote.%%a.%%b.runsettings" /o -
		call %currentDir%jrepl.bat "(name=\qDriverCapabilities\.browserstack\.user\q value=\q)(.*?)(\q)" "$1%user%$3" /x /f "%currentDir%%app%.Remote.%%a.%%b.runsettings" /o -
		call %currentDir%jrepl.bat "(name=\qDriverCapabilities\.browserstack\.key\q value=\q)(.*?)(\q)" "$1%key%$3" /x /f "%currentDir%%app%.Remote.%%a.%%b.runsettings" /o -
		echo starting "%%i" dotnet test "%currentDir%%app%.csproj" -f "net471" -c "%config%" -a "%currentDir%bin\%config%" -v "n" --logger "trx" -r ".\TestResults" --filter !filter! --settings "%currentDir%%app%.Remote.%%a.%%b.runsettings" --no-restore --no-build
		start "%%i" dotnet test "%currentDir%%app%.csproj" -f "net471" -c "%config%" -a "%currentDir%bin\%config%" -v "n" --logger "trx" -r ".\TestResults" --filter !filter! --settings "%currentDir%%app%.Remote.%%a.%%b.runsettings" --no-restore --no-build
	)
)
goto end

REM Not-used / Doesn't work (https://github.com/Microsoft/vstest/issues/862)
:runSettingArgs
echo Using runSetting args
for %%i in (%input%) do (
	echo %%i
	set "int=%%i"
	set "int=!int:.= !"
	for /f "tokens=1,2 delims= " %%a in ("!int!") do (
		echo CrossBrowserEnvironment=$1%%a$3$1%%b$3
		REM /B /NEWWINDOW
		start "%%i" /B dotnet test "%currentDir%%app%.csproj" -f "net471" -c "%config%" -a "./" --no-build --logger "trx" -r "./TestResults" --filter !filter! --settings "%currentDir%%app%.runsettings" -- appSettings.browser=RemoteWebDriver appSettings.DriverCapabilities=CloudProvider appSettings.CrossBrowserEnvironment=%%a%%b
	)
)
goto end

:end
endlocal