@echo off

set dllLocation=D:\Documents\Fakultet\Blok4\ProjekatDRS\Moduo2\ClientCommonTest\bin\Debug\ClientCommonTest.dll
set reportFolderLocation=.

echo * Runing unit test cover *
cd %reportFolderLocation%
call OpenCoverAPI.bat %dllLocation% %reportFolderLocation%\reports * *

pause