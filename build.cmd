
@echo off

if "%1" == "" (
	goto :format
)
if "%2" == "" (
	goto :format
)

if "%3" == "" (
	%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild build\msbuild.proj /p:Configuration="%1" /p:Version="%2" /p:Platform="AnyCPU"
)
if "%3" == "/publish" (
	%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild build\msbuild.proj /p:Configuration="%1" /p:Version="%2" /p:Platform="AnyCPU" /target:Publish
)
goto :end

:format
echo.
echo Format: build Debug^|Release VersionNumber [/publish]
echo.

:end

