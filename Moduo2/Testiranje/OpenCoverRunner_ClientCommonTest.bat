@echo off

set dllLocation=C:\Users\radis\Documents\GitHub\ProjekatDRS\Moduo2\ClientCommonTest\bin\Debug\ClientCommonTest.dll
set reportFolderLocation=.

echo * Runing unit test cover *
cd %reportFolderLocation%
call OpenCoverAPI.bat %dllLocation% %reportFolderLocation%\reports * *

pause