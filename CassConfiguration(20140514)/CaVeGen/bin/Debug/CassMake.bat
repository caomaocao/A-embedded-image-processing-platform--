
::@echo   off

cd /d %1
set CurrentProjectDir=%cd%

::获取上一级目录  setlocal enabledelayedexpansion

::cd ..

::set preDir1=%cd%


::获取上上一级目录

::cd ..

::set preDir2=%cd%



::删除相关文件
del %3\error.txt
del %CurrentProjectDir%\%2.exe
::del %CurrentProjectDir%\out\*.bmp

::  设置cl.exe 

call  vcvars32.bat

call cl.exe  %CurrentProjectDir%\main.c /I  %3\cass_h    -o  %CurrentProjectDir%\%2  >%3\out\error.txt


:: 调用.exe
::cd  /d %CurrentProjectDir%

::%2.exe





