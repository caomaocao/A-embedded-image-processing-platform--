
::@echo   off

cd /d %1
set CurrentProjectDir=%cd%

::��ȡ��һ��Ŀ¼  setlocal enabledelayedexpansion

::cd ..

::set preDir1=%cd%


::��ȡ����һ��Ŀ¼

::cd ..

::set preDir2=%cd%



::ɾ������ļ�
del %3\error.txt
del %CurrentProjectDir%\%2.exe
::del %CurrentProjectDir%\out\*.bmp

::  ����cl.exe 

call  vcvars32.bat

call cl.exe  %CurrentProjectDir%\main.c /I  %3\cass_h    -o  %CurrentProjectDir%\%2  >%3\out\error.txt


:: ����.exe
::cd  /d %CurrentProjectDir%

::%2.exe





