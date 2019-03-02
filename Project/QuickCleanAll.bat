@echo. 
@echo WARNING: This script will remove all subdirectories named "bin" and "obj".
@echo off
SET /P _continue= Are you sure you want to conitinue (y/n)?

IF "%_continue%"=="y" (
	CALL :removeDir bin 
	CALL :removeDir obj
)

goto end

:removeDir
@echo off
setlocal
setlocal enabledelayedexpansion
@echo off
for /d /r %CD% %%i in (%1) do (
  @if exist "%%i" (
    @set _variable=%%i
    @echo   removing !_variable!
	@RD /S /Q !_variable!
    )
  )
endlocal
@echo.Done removing %1 directories.
EXIT /B

:end
pause