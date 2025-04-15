@echo off
setlocal enabledelayedexpansion

REM Source image file name
set "fileName=mario.png"

REM Base URL
set "baseUrl=http://localhost:5277/Images/Resize"

REM Loop through 20 iterations
for /L %%i in (1,1,20) do (
    set /a "width=100 + (%%i * 20) %% 400"
    set /a "height=100 + (%%i * 30) %% 300"

    set "outputFile=mario_resized_%%i_!width!x!height!.png"

    echo Requesting !outputFile! with size !width!x!height!
    curl -k -o "!outputFile!" "!baseUrl!?fileName=!fileName!&width=!width!&height=!height!"
)

echo All done!
pause
