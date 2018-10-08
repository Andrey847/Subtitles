@echo off

..\Binaries\DatabaseCompiler\DatabaseCompiler.EXE "..\Database\!Database.sql" "..\Database\Changes\9999_Logic.sql"
if %ERRORLEVEL% NEQ 0 goto errors

exit /b 0

goto done

:errors

exit /b 1
	
goto done

:done

