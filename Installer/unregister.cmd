@echo off

REM Make sure we are called with a version
IF [%1] == [] GOTO Usage

REM Make sure our files are ready
IF NOT EXIST MySql.Data\provider\bin\debug\mysql.data.dll GOTO NOTREADY
IF NOT EXIST MySql.Data\provider\bin\debug\mysql.data.CF.dll GOTO NOTREADY
IF NOT EXIST MySql.Web\providers\bin\debug\mysql.web.dll GOTO NOTREADY
IF NOT EXIST mysql.visualstudio\bin\debug\mysql.visualstudio.dll GOTO NOTREADY
IF NOT %1 == 2005 AND NOT EXIST MySql.Data.Entity\provider\bin\debug\mysql.data.entity.dll GOTO NOTREADY

REM Unregister our assemblies (this will work if they are not registered)
gacutil /u mysql.data
gacutil /u mysql.data.cf
gacutil /u mysql.web

REM Now uninstall the core assembly
installutil /u mysql.data\provider\bin\debug\mysql.data.dll

REM Uninstall web assembly
installutil /u mysql.web\providers\bin\debug\mysql.web.dll

REM If we are not on 2005 then register the entity assembly
if NOT %1 == 2005 gacutil /u mysql.data.entity

REm Now unregister the visual studio bits
set cmd=version=VS%1 debug=%2 ranu=%3
if %1 == 2005 SET cmd=version=VS2005 debug=%2
installer\binary\globalinstaller /u mysql.visualstudio\bin\debug\mysql.visualstudio.dll %cmd%
EXIT /B 0

:NOTREADY
ECHO some files are not ready
EXIT /B 1

:USAGE
ECHO version missing