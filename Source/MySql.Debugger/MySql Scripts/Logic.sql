CREATE DATABASE  IF NOT EXISTS `.serversidedebugger` /*!40100 DEFAULT CHARACTER SET latin1 */ //

USE `.serversidedebugger` //



CREATE FUNCTION `Peek`( pDebugSessionId int ) RETURNS varchar(64) CHARSET latin1 DETERMINISTIC
begin

    declare nextId int;
    declare returnValue varchar( 64 );
    
    set nextId = ( select max( `.serversidedebugger`.`debugcallstack`.`Id` ) from `.serversidedebugger`.`debugcallstack` where `.serversidedebugger`.`debugcallstack`.`DebugSessionId` = pDebugSessionId );
    set returnValue = ( select RoutineName from `.serversidedebugger`.`debugcallstack` 
        where ( `.serversidedebugger`.`debugcallstack`.`DebugSessionId` = pDebugSessionId ) and ( `.serversidedebugger`.`debugcallstack`.`Id` = nextId ));
    return returnValue;

end //

CREATE PROCEDURE `CleanupScope`( pDebugSessionId int ) DETERMINISTIC
begin
  
  delete from `.serversidedebugger`.`debugscope` 
	where ( `.serversidedebugger`.`debugscope`.`DebugSessionId` = pDebugSessionId ) and 
		( `.serversidedebugger`.`debugscope`.`DebugScopeLevel` = (select `.serversidedebugger`.`debugdata`.`Val` from `.serversidedebugger`.`debugdata` where `.serversidedebugger`.`debugdata`.`id` = 1 limit 1 ) );
  update `.serversidedebugger`.`debugdata` set `.serversidedebugger`.`debugdata`.`Val` = `.serversidedebugger`.`debugdata`.`Val` - 1 
	where `.serversidedebugger`.`debugdata`.`Id` = 1;

end //


CREATE PROCEDURE `DumpScopeVar`( pDebugSessionId int, pDebugScopeLevel int, pVarName varchar( 256 ), pVarValue binary ) DETERMINISTIC
begin
  
  replace `.serversidedebugger`.`debugscope`( DebugSessionId, DebugScopeLevel, VarName, VarValue ) values ( pDebugSessionId, pDebugScopeLevel, pVarName, pVarValue );
  
end //


CREATE PROCEDURE `ExitEnterCriticalSection`( spName varchar( 64 ), lineNumber int ) DETERMINISTIC
begin
    
  declare gblNetWriteTimeout int;
  declare garbage int;

  set @@global.net_write_timeout = 999998;
  set garbage = ( select release_lock( 'lock1' ) );
  repeat 
	set gblNetWriteTimeout = @@global.net_write_timeout;
  until gblNetWriteTimeout <> 999998 end repeat;
  set garbage = (select get_lock( 'lock1', 999999 ) );

end //


CREATE PROCEDURE `Pop`( pDebugSessionId int ) DETERMINISTIC
begin

    declare nextId int;
    set nextId = ( select max( `.serversidedebugger`.`debugcallstack`.`Id` ) from `.serversidedebugger`.`debugcallstack` 
		where `.serversidedebugger`.`debugcallstack`.`DebugSessionId` = pDebugSessionId );
    delete from `.serversidedebugger`.`debugcallstack` where ( `.serversidedebugger`.`debugcallstack`.`DebugSessionId` = pDebugSessionId ) 
		and ( `.serversidedebugger`.`debugcallstack`.`Id` = nextId );

end //


CREATE PROCEDURE `Push`( pDebugSessionId int, pRoutineName varchar( 64 ) ) DETERMINISTIC
begin

    insert into `.serversidedebugger`.`debugcallstack`( DebugSessionId, RoutineName ) values ( pDebugSessionId, pRoutineName );

end //

create procedure SetDebugScopeVar( pDebugSessionId int, pDebugScopeLevel int, pVarName varchar( 256 ), pVarValue varbinary( 50000 ) ) DETERMINISTIC
begin

	insert into `.serversidedebugger`.`debugscope`( DebugSessionId, DebugScopeLevel, VarName, VarValue ) values ( pDebugSessionId, pDebugScopeLevel, pVarName, pVarValue );

end //

create procedure GetDebugScopeVar( pDebugSessionId int, pDebugScopeLevel int, pVarName varchar( 256 ), out pVarValue varbinary( 50000 )) DETERMINISTIC
begin
	
	declare pId int;
	
	set pId = ( select max( Id ) from `.serversidedebugger`.`debugscope` 
		where ( DebugSessionId = pDebugSessionId ) and ( DebugScopeLevel = pDebugScopeLevel ) and ( VarName = pVarName ) );
	select pVarValue = VarValue from `.serversidedebugger`.`debugscope` 
		where ( DebugSessionId = pDebugSessionId ) and ( DebugScopeLevel = pDebugScopeLevel ) and ( VarName = pVarName ) and ( Id = pId );
	
end //
