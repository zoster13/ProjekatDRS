@echo off

set dllLocation=C:\Users\radis\Documents\GitHub\ProjekatDRS\Moduo2\ServerTest\bin\Debug\ServerTest.dll
set reportFolderLocation=.

echo * Runing unit test cover *
cd %reportFolderLocation%
call OpenCoverAPI.bat %dllLocation% %reportFolderLocation%\reports * *

pause