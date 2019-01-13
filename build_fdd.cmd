@echo on

REM FDD - framework-dependent deployment (FDD) relies on the presence of a shared system-wide version of .NET Core on the target system.	
REM Set variables in order to run msbuild and other tools.
CALL "C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\Tools\VsDevCmd.bat"


REM ******************* variables **************************
if "%time:~0,1%" == " " (set tmp_time=0%time:~1,1%) ELSE set tmp_time=%time:~0,2%
set name_YYYYMMDD=%date:~6,8%%date:~3,2%%date:~0,2%_%tmp_time%
set dropfolder=Published\%name_YYYYMMDD%


REM database
cd database
call BuildDatabase.cmd
cd..
if not %ERRORLEVEL%==0 goto fail


REM main site
REM msbuild src\Site\SubtitlesLearn.Site.csproj /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile
dotnet publish src\Site\SubtitlesLearn.Site.csproj --force -o ..\..\Published\Site

if not %ERRORLEVEL%==0 goto fail


REM ******************* publishing *************************

REM remove existing folder (if it is)
if exist %dropfolder% RD /S /Q %dropfolder%

md %dropfolder%


REM create folder for source data
md %dropfolder%\Source
md %dropfolder%\Source\DataBase
md %dropfolder%\Source\Service

REM copy database (files only)
xcopy /Y Database\Changes\*.* %dropfolder%\Source\Database\

REM whole site
xcopy /E /Y Published\Site\*.* %dropfolder%\Source\Service\

pause
exit /b 0 

:fail 
echo Error!!!!!


pause
exit /b 1
