; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Service Manager"
#define MyAppVersion "1.0"
#define MyAppPublisher "Simon Carter"
#define MyAppURL "https://github.com/k3ldar/ServiceManager"
#define MyAppExeName "SM.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{9EF3222F-190A-48E5-B723-3F60A9BD057E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "T:\GitProjects\Builds\ServerManager\Release\SM.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\ServiceAdminConsole.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\ServiceManager.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\SharedControls.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\SharedLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Threading.Overlapped.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Web.Http.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Web.Http.WebHost.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Xml.XPath.XDocument.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\Microsoft.Web.Administration.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\Microsoft.Win32.Registry.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\MySql.Data.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\Newtonsoft.Json.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Data.Common.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Diagnostics.DiagnosticSource.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Diagnostics.EventLog.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Diagnostics.StackTrace.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Diagnostics.Tracing.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Globalization.Extensions.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.IO.Compression.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Net.Http.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Net.Http.Formatting.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Net.Sockets.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Reflection.TypeExtensions.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Runtime.Serialization.Primitives.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Security.AccessControl.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Security.Cryptography.Algorithms.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Security.Permissions.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Security.Principal.Windows.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.Security.SecureString.dll"; DestDir: "{app}\System"; Flags: ignoreversion
Source: "T:\GitProjects\Builds\ServerManager\Release\System.ServiceProcess.ServiceController.dll"; DestDir: "{app}\System"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
;Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Dirs]
Name: "{app}\"; Permissions: everyone-modify
Name: "{app}\System"


[Run]
Filename: "{app}\sm.exe"; Parameters: "/i"; WorkingDir: "{app}"; Flags: waituntilterminated runhidden; Description: "Installing WebDefender Service"; StatusMsg: "Installing WebDefender Service"; Check: ServiceManagerInstalled; AfterInstall: ServiceManagerStart

[UninstallRun]
Filename: "{app}\sm.exe"; Parameters: "/u"; WorkingDir: "{app}"; Flags: waituntilterminated runhidden; BeforeInstall: ServiceManagerStop

[Code]
//  Code pasted from the following address, for examples and more visit it:
//  http://www.vincenzo.net/isxkb/index.php?title=Service_-_Functions_to_Start%2C_Stop%2C_Install%2C_Remove_a_Service


// function IsServiceInstalled(ServiceName: string) : boolean;
// function IsServiceRunning(ServiceName: string) : boolean;
// function InstallService(FileName, ServiceName, DisplayName, Description : string;ServiceType,StartType :cardinal) : boolean;
// function RemoveService(ServiceName: string) : boolean;
// function StartService(ServiceName: string) : boolean;
// function StopService(ServiceName: string) : boolean;
// function SetupService(service, port, comment: string) : boolean;

type
    SERVICE_STATUS = record
        dwServiceType               : cardinal;
        dwCurrentState              : cardinal;
        dwControlsAccepted          : cardinal;
        dwWin32ExitCode             : cardinal;
        dwServiceSpecificExitCode   : cardinal;
        dwCheckPoint                : cardinal;
        dwWaitHint                  : cardinal;
    end;
    HANDLE = cardinal;

const
    SERVICE_QUERY_CONFIG        = $1;
    SERVICE_CHANGE_CONFIG       = $2;
    SERVICE_QUERY_STATUS        = $4;
    SERVICE_START               = $10;
    SERVICE_STOP                = $20;
    SERVICE_ALL_ACCESS          = $f01ff;
    SC_MANAGER_ALL_ACCESS       = $f003f;
    SERVICE_WIN32_OWN_PROCESS   = $10;
    SERVICE_WIN32_SHARE_PROCESS = $20;
    SERVICE_WIN32               = $30;
    SERVICE_INTERACTIVE_PROCESS = $100;
    SERVICE_BOOT_START          = $0;
    SERVICE_SYSTEM_START        = $1;
    SERVICE_AUTO_START          = $2;
    SERVICE_DEMAND_START        = $3;
    SERVICE_DISABLED            = $4;
    SERVICE_DELETE              = $10000;
    SERVICE_CONTROL_STOP        = $1;
    SERVICE_CONTROL_PAUSE       = $2;
    SERVICE_CONTROL_CONTINUE    = $3;
    SERVICE_CONTROL_INTERROGATE = $4;
    SERVICE_STOPPED             = $1;
    SERVICE_START_PENDING       = $2;
    SERVICE_STOP_PENDING        = $3;
    SERVICE_RUNNING             = $4;
    SERVICE_CONTINUE_PENDING    = $5;
    SERVICE_PAUSE_PENDING       = $6;
    SERVICE_PAUSED              = $7;

// #######################################################################################
// nt based service utilities
// #######################################################################################
function OpenSCManager(lpMachineName, lpDatabaseName: string; dwDesiredAccess :cardinal): HANDLE;
external 'OpenSCManagerA@advapi32.dll stdcall';

function OpenService(hSCManager :HANDLE;lpServiceName: string; dwDesiredAccess :cardinal): HANDLE;
external 'OpenServiceA@advapi32.dll stdcall';

function CloseServiceHandle(hSCObject :HANDLE): boolean;
external 'CloseServiceHandle@advapi32.dll stdcall';

function CreateService(hSCManager :HANDLE;lpServiceName, lpDisplayName: string;dwDesiredAccess,dwServiceType,dwStartType,dwErrorControl: cardinal;lpBinaryPathName,lpLoadOrderGroup: String; lpdwTagId : cardinal;lpDependencies,lpServiceStartName,lpPassword :string): cardinal;
external 'CreateServiceA@advapi32.dll stdcall';

function DeleteService(hService :HANDLE): boolean;
external 'DeleteService@advapi32.dll stdcall';

function StartNTService(hService :HANDLE;dwNumServiceArgs : cardinal;lpServiceArgVectors : cardinal) : boolean;
external 'StartServiceA@advapi32.dll stdcall';

function ControlService(hService :HANDLE; dwControl :cardinal;var ServiceStatus :SERVICE_STATUS) : boolean;
external 'ControlService@advapi32.dll stdcall';

function QueryServiceStatus(hService :HANDLE;var ServiceStatus :SERVICE_STATUS) : boolean;
external 'QueryServiceStatus@advapi32.dll stdcall';

function QueryServiceStatusEx(hService :HANDLE;ServiceStatus :SERVICE_STATUS) : boolean;
external 'QueryServiceStatus@advapi32.dll stdcall';

function GetLastError() : cardinal;
external 'GetLastError@kernel32.dll stdcall';

function OpenServiceManager() : HANDLE;
begin
    if UsingWinNT() = true then begin
        Result := OpenSCManager('','',SC_MANAGER_ALL_ACCESS);
        if Result = 0 then
            MsgBox('the servicemanager is not available', mbError, MB_OK)
    end
    else begin
            MsgBox('only nt based systems support services', mbError, MB_OK)
            Result := 0;
    end
end;

function IsServiceInstalled(ServiceName: string) : boolean;
var
    hSCM    : HANDLE;
    hService: HANDLE;
begin
    hSCM := OpenServiceManager();
    Result := false;
    if hSCM <> 0 then begin
        hService := OpenService(hSCM,ServiceName,SERVICE_QUERY_CONFIG);
        if hService <> 0 then begin
            Result := true;
            CloseServiceHandle(hService)
        end;
        CloseServiceHandle(hSCM)
    end
end;

function InstallService(FileName, ServiceName, DisplayName, Description : string;ServiceType,StartType :cardinal) : boolean;
var
    hSCM    : HANDLE;
    hService: HANDLE;
begin
    hSCM := OpenServiceManager();
    Result := false;
    if hSCM <> 0 then begin
        hService := CreateService(hSCM,ServiceName,DisplayName,SERVICE_ALL_ACCESS,ServiceType,StartType,0,FileName,'',0,'','','');
        if hService <> 0 then begin
            Result := true;
            // Win2K & WinXP supports aditional description text for services
            if Description<> '' then
                RegWriteStringValue(HKLM,'System\CurrentControlSet\Services\' + ServiceName,'Description',Description);
            CloseServiceHandle(hService)
        end;
        CloseServiceHandle(hSCM)
    end
end;

function RemoveService(ServiceName: string) : boolean;
var
    hSCM    : HANDLE;
    hService: HANDLE;
begin
    hSCM := OpenServiceManager();
    Result := false;
    if hSCM <> 0 then begin
        hService := OpenService(hSCM,ServiceName,SERVICE_DELETE);
        if hService <> 0 then begin
            Result := DeleteService(hService);
            CloseServiceHandle(hService)
        end;
        CloseServiceHandle(hSCM)
    end
end;

function StartService(ServiceName: string) : boolean;
var
    hSCM    : HANDLE;
    hService: HANDLE;
begin
    hSCM := OpenServiceManager();
    Result := false;
    if hSCM <> 0 then begin
        hService := OpenService(hSCM,ServiceName,SERVICE_START);
        if hService <> 0 then begin
            Result := StartNTService(hService,0,0);
            CloseServiceHandle(hService)
        end;
        CloseServiceHandle(hSCM)
    end;
end;

function StopService(ServiceName: string) : boolean;
var
    hSCM    : HANDLE;
    hService: HANDLE;
    Status  : SERVICE_STATUS;
begin
    hSCM := OpenServiceManager();
    Result := false;
    if hSCM <> 0 then begin
        hService := OpenService(hSCM,ServiceName,SERVICE_STOP);
        if hService <> 0 then begin
            Result := ControlService(hService,SERVICE_CONTROL_STOP,Status);
            CloseServiceHandle(hService)
        end;
        CloseServiceHandle(hSCM)
    end;
end;

function IsServiceRunning(ServiceName: string) : boolean;
var
    hSCM    : HANDLE;
    hService: HANDLE;
    Status  : SERVICE_STATUS;
begin
    hSCM := OpenServiceManager();
    Result := false;
    if hSCM <> 0 then begin
        hService := OpenService(hSCM,ServiceName,SERVICE_QUERY_STATUS);
        if hService <> 0 then begin
            if QueryServiceStatus(hService,Status) then begin
                Result :=(Status.dwCurrentState = SERVICE_RUNNING)
            end;
            CloseServiceHandle(hService)
            end;
        CloseServiceHandle(hSCM)
    end
end;



// end service methods


procedure StopServiceRunning(ServiceName: string);
var
  i: integer;
begin
  if (IsServiceRunning(ServiceName)) then
  begin
    StopService(ServiceName);

    i := 0;

    while ((i < 20) and (IsServiceRunning(ServiceName))) do
    begin
      i := i + 1;
      Sleep(300);
    end;

  end;
end;

procedure ServiceManagerStop();
begin
  StopServiceRunning('ServiceManager');
end;

procedure ServiceManagerStart();
begin
  StartService('ServiceManager');
end;

function ServiceManagerInstalled(): Boolean;
begin
  Result := not IsServiceInstalled('ServiceManager');
end;

procedure DeinitializeSetup();
begin
  if (not ServiceManagerInstalled()) then
  begin
    if (not IsServiceRunning('ServiceManager')) then
    begin
      ServiceManagerStart();
    end;
  end;
end;
