Attribute VB_Name = "Constants"
Option Explicit

Public Const mFilterMask = 58

Public Const SpecialEventLogFileName = "SpecialEvent.log"

'*** Timer Interval for event log database file check
Public Const EventLogDatabaseFileCheckInterval = 60000
   ' this is the number of milliseconds between two consecutive checks
   ' of the existence of the event log database

'*** Current version of Configuration DB
Public Const dbLastVersion = 6

'*** PhoneBar LH second line extension prefix
Public Const ctPhonabarLHSecondLinePrefix = "SIP:"

'*** ActiveX telephony application name
Public Const telephonyClassName = "LineInterface.LinePort"
Public Const callForwarderClassName = "CallForwarder.LinePort"

Public Const CantCreateClassId_1 = 101       ' "Impossibile creare una nuova istanza di:"
Public Const CantCreateClassId_2 = 102       ' "Verificare  che tale  programma  sia correttamente registrato."
                                        
Public Const vStopInstance = 10
Public Const vStartInstance = 11

'*** ASCII Code for special separator character in configuration string
Public Const ctParamListSeparatorChar = &H81
Public Const ctParamListHeaderChar = &H82

'*** CallData
Public Const callDataExternalParty = "ExternalParty"
Public Const callDataDDI = "DDI"
Public Const callDataMediaType = "_MediaType_"

'*** Transfer Properties predefinite
Public Const tpCallerParty = "__CallerParty"
Public Const tpCalledParty = "__CalledParty"
Public Const tpCallMediaType = "__CallMediaType"

'*** Marker per le chiamate con MediaType di tipo Video
Public Const audioMediaTypeMarker = "1"
Public Const atPhonesMediaTypeMarker = "2"
Public Const videoMediaTypeMarker = "5"

'*** Estensioni per i file dei messaggi
Public Const audioWavMessageFileExtension = ".WAV"
Public Const audioVoxMessageFileExtension = ".VOX"
Public Const videoAviMessageFileExtension = ".AVI"
Public Const tif_FaxDocumentFileExtension = ".TIF"
Public Const tiffFaxDocumentFileExtension = "TIFF"

'********************************************************************************
'********************************************************************************
'********************************************************************************
' constants regarding the TCP communications between IVRMonitor and LineInterface
'********************************************************************************
Public Const callRecordingServicePort = "messengervv"
Public Const channelManagerServicePort = "messengercm"
Public Const monitorProxyServicePort = "messengermp"

' types of messages
Public Const setChannelName_Type = "type 01"
Public Const StatusBarPanelsText_Type = "type 02"
Public Const updateStateChanged_Type = "type 03"
Public Const UpdateCampaignNameOnGui_Type = "type 04"
Public Const System_Type = "type 05"
Public Const SingleChannelCommand_Type = "type 06"
Public Const IncrementCallCounter_Type = "type 07"
Public Const DecrementCallCounter_Type = "type 08"
Public Const ResetCallCounter_Type = "type 09"

' message part identifiers
Public Const header = "*begin*"
Public Const typeSeparator = "%%%"
Public Const fieldSeparator = "###"
Public Const footer = "§end§"

' codes
Public Const StartSystemCode = "StartSystem"
Public Const StopSystemCode = "StopSystem"
Public Const SystemStartedSuccessfullyCode = "SystemStartedSuccessfully"
Public Const SystemStartFailedCode = "SystemStartFailed"
Public Const SystemStoppedSuccessfullyCode = "SystemStoppedSuccessfully"
Public Const SystemStopFailedCode = "SystemStopFailed"
Public Const LineInterfaceVersionCode = "LineInterfaceVersion"

'Command codes
Public Const OnHookCommand = "OnHook"
Public Const OffHookCommand = "OffHook"
Public Const LoginRequestCommand = "LoginRequest"
Public Const LogoutRequestCommand = "LogoutRequest"
Public Const PauseRequestCommand = "PauseRequest"
Public Const ReadyRequestCommand = "ReadyRequest"
Public Const VStopInstanceCommand = "VStopInstance"
Public Const VStartInstanceCommand = "VStartInstance"
Public Const DisableStateUpdateCommand = "DisableStateUpdate"
Public Const DisableCmpNameUpdateCommand = "DisableCmpNameUpdate"
Public Const DisableSrvNameUpdateCommand = "DisableSrvNameUpdate"
Public Const DisableCallCounterUpdateCommand = "DisableCallCounterUpdate"

'********************************************************************************
'********************************************************************************

'*** Timer for Login status polling
Public Const loginRetryTimerLen = 15000

'*** Call Forwarder state
Public Const ForwarderUnavailable = "ForwUnavailable"
Public Const ForwarderAvailable = "ForwAvailable"
Public Const ForwarderBusy = "ForwBusy"

'*** Dialserver Connection States
Public Const svpConnected = 1
Public Const svpNotConnected = 0

'*** Agent state string
Public Const sNotConfigured = 50
Public Const sNotLoggedIn = 51
Public Const sLoggedIn = 52
Public Const sPaused = 53
Public Const sTalking = 54
Public Const sOtherCall = 55
Public Const sAssigned = 56
Public Const sChannelStarted = 57   ' added by lto
Public Const sExecutingClientRequest = 59

'*** Messenger --> Phones messages code
Public Const PauseRequest = &H2003
Public Const ReadyRequest = &H2002
Public Const LogoutRequest = &H2001
Public Const LoginRequest = &H2000
Public Const AssignmentReply = &H4004
Public Const NewCallRequest = &H2005
Public Const TransferRequest = &H2006
Public Const CampaignListRequest = &H2007
Public Const AgentListRequest = &H2008
Public Const CallInfoRequest = &H200D
Public Const AbortCallRequest = &H200E
Public Const KeepAliveReply = &H40FF
Public Const SetCallResultEvent = &H880B

'*** Phones --> Messenger messages code
Public Const assignmentRequest = &H3004
Public Const LoginReply = &H5000
Public Const LogoutReply = &H5001
Public Const ReadyReply = &H5002
Public Const PauseReply = &H5003
Public Const newCallReply = &H5005
Public Const TransferReply = &H5006
Public Const CampaignListReply = &H5007
Public Const AgentListReply = &H5008
Public Const CallInfoReply = &H500D
Public Const AbortCallReply = &H500E
Public Const alertingEvent = &H9800
Public Const answeredEvent = &H9801
Public Const terminatedEvent = &H9802
Public Const callFailureEvent = &H9803
Public Const OtherCallEvent = &H9804
Public Const ReadyEvent = &H9002
Public Const PauseEvent = &H9003
Public Const SupervisorMsgEvent = &H9807
Public Const ReadyForTransferEvent = &H9808
Public Const ReadyForDetachEvent = &H9809
' This is an internal event, used during call transfer...
'Public Const TransferInitiatedEvent = &H8006
Public Const PostCallWorkEvent = &H9805
Public Const KeepAliveRequest = &H30FF

'*** Messenger <--> Dialserver messages code
Public Const DeviceSignInEvent = &H8880
Public Const DeviceSignOutEvent = &H8881
Public Const DeviceStateEvent = &H8882
Public Const DeviceDialRequest = &H3280
Public Const DeviceHangupRequest = &H3281

'*** RPC Constants
Public Const rpcOleErrorBase = vbObjectError + 500
Public Const rpcErrNestedCall = -2 + rpcOleErrorBase
Public Const rpcErrConnectionFailure = -5 + rpcOleErrorBase
Public Const rpcErrTimeout = -4 + rpcOleErrorBase
Public Const rpcOK = 0
Public Const rpcAvailable = 1
Public Const rpcBusy = 2
Public Const rpcReplyReceived = 3
Public Const rpcTimeout = 4
Public Const rpcConnectionFailure = 5
Public Const rpcTimeoutValue = 3000

'*** Connection to use to send a message
Public Const sendToPhones = "Phones"
Public Const sendToDialServ = "DialServ"

'*** Call states
Public Const Alerting = "Alerting"
Public Const assigned = "Assigned"
Public Const Failed = "Failed"
Public Const Established = "Established"
Public Const Undefined = "Undefined"

Public Const MaxAttemptedContactFetches = 4

' Constant for ChannelType
Public Const chTypeAnalog = "A"
Public Const chTypeAnalogNameId = 301              ' "Analogico"
Public Const chTypeDigital = "G"
Public Const chTypeDigitalNameId = 302             ' "GlobalCall"
Public Const chTypePhoneSwitch = "P"
Public Const chTypePhoneSwitchNameId = 303         ' "PhoneSwitch"
Public Const chTypePhoneSwitch_20 = "P2"
Public Const chTypePhoneSwitchNameId_20 = 304      ' "PhoneSwitch 2.0"
Public Const chTypePhoneSwitch_40 = "P4"
Public Const chTypePhoneSwitchNameId_40 = 305      ' "PhoneSwitch 4.0"
Public Const chTypePhoneSwitch_VK = "VK"
Public Const chTypePhoneSwitchNameId_VK = 306      ' "PhoneSwitch VK"

' Constant for Run Mode setting.
Public Const smgProduction = 1
Public Const smgTest = 2
Public Const smgTestDB = 3

' Constant for operation type in frmNewCampaign
Public Const opCreateNew = 1
Public Const opRename = 2

' Channel Types (Adding type remeber to modify FrmSetExtension's combo
Public Const typCallCenterOperator = "CallCenter Operator"
Public Const typCallCenterOperatorExternalRepresentationId = 545     ' "Agente di CallCenter"
Public Const typCallForwarder = "Call Forwarder"

'*** VarType
Public Const vrType_String = 0
Public Const vrType_Number = 1
Public Const vrType_Date = 2

'*** DeviceState event
Public Const msgOnHook = 1
Public Const msgOffHook = 2
Public Const msgRing = 3

' Continue Mode constant
Public Const modeHangUp = 0
Public Const modeTransfer = 1
Public Const modeVoiceMail = 2
Public Const modeRaccoltaDati = 3
Public Const modeMenuDriven = 4
Public Const modeFlashAndDial = 5

'*** StopCall reasons
Public Const msgNull = -1
Public Const msgTransfer = 0
Public Const msgEndCall = 1
Public Const msgPreQueueTransfer = 2
Public Const msgRemoteHangUp = 3
Public Const MsgNoResources = 4
Public Const msgReadyForTransfer = 5
Public Const msgMaxCallTimerExp = 6

'*** Agent states
Public Const NotConfiguredState = "notConfigured"
Public Const NotLoggedInState = "notLoggedIn"
Public Const LoggedInState = "LoggedIn"
Public Const PausedState = "Paused"
Public Const ReadyState = "Ready"
Public Const TalkingState = "Talking"
Public Const PostCallState = "PostCall"
Public Const OtherCallState = "OtherCall"
Public Const AssignedState = "Assigned"
Public Const WaitingOutboundState = "WaitingOutbound"
Public Const AlertingState = "Alerting"
Public Const ChannelStartedState = "ChannelStarted"
Public Const PendingClientRequestState = "PendingClientRequest"
Public Const ExecutingClientRequestState = "ExecutingClientRequestState"

'*** Client TCP messages Code
'*** Incoming Message Codes
Public Const PlayRequest = &H2101
Public Const StartRecordingRequest = &H2102
Public Const StopRecordingRequest = &H2103
Public Const StartCallRecordingRequest = &H2104
Public Const StopCallRecordingRequest = &H2105
Public Const MakeNewCallRequest = &H2106
Public Const GetLightHouseServersListRequest = &H2107
Public Const ConnectionLostEvent = &H8101
Public Const RecorderKeepAliveRequest = &HFFFF

'*** OutGoing Message Codes
Public Const PlayReply = &H5101
Public Const StartRecordingReply = &H5102
Public Const StopRecordingReply = &H5103
Public Const StartCallRecordingReply = &H5104
Public Const StopCallRecordingReply = &H5105
Public Const MakeNewCallReply = &H5106
Public Const GetLightHouseServersListReply = &H5107

Public Const EndPlayEvent = &H9101
Public Const OperationFailureEvent = &H9102
Public Const MakeNewCallEvent = &H9106
Public Const LightHouseServersListEvent = &H9107

'*** Event Logging Type
Public Const logOnTextFile = 0
Public Const logOnMDBFile = 1

'*** Campaign Parameters Name
Public Const cpPrmNum = 4
Public Const cpPrmDisableCutOff = "DisableCutOff"
Public Const cpPrmTerminationCp = "TerminationCp"
Public Const cpPrmMaxCallTime = "MaxCallTime"
Public Const cpPrmTimeoutService = "TimeoutService"
Public Const cpPrmEnableSaveTalkingTime = "EnableSaveTalkingTime"
Public Const cpPrmSaveTalkingTimeCallDataName = "SaveTalkingTimeCallDataName"

Public Function IsPhoneBarLHSecondLine(extension As String) As Boolean
   IsPhoneBarLHSecondLine = (InStr(1, UCase(CStr(extension)), ctPhonabarLHSecondLinePrefix) > 0)
End Function


