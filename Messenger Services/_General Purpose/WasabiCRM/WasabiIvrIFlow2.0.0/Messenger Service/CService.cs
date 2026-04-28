//==========================================================================================================

// Copyright © IFM Infomaster. All rights reserved.

//==========================================================================================================

// Author    : Claudia Viale [CVI]
// Date      : 15/04/2024
// Revisions :

//==========================================================================================================

#region Namespaces

using Ifm.Components.Messenger.Blocks.Interfaces;
using Ifm.Components.Messenger.Blocks.Utilities;
using Microsoft.SqlServer.Server;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using WasabiCrm.IFlow.Api.FlowConversations;
using static System.Net.WebRequestMethods;

#endregion Namespaces

namespace Ifm.Components.Messenger.Blocks.CustomMessengerServices
{
    [ComVisible(true)]
    [Guid("A96D0B8B-BC9F-4A49-9BF5-6B97F179303A")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("WasabiIvrIFlow.CService")]
    public class CService : IService
    {
        #region Constants
        private const string prmWasabiURL = "WS_URL";
        private const string prmWasabiApiId = "WS_API_ID";
        private const string prmWasabiApiSecret = "WS_API_SECRET";
        private const string prmWasabiTimeout = "WS_TIMEOUT";
        private const string prmFlowId = "FlowId";

        private const string prmMaxRetryTel = "MaxRetryTel";
        private const string prmTelefonico = "Telefonico";
        private const string prmCHATtimeout = "CHATtimeout";

        private const string prmMessagesPath = "MessagesPath";       // Folder contenente i messaggi da suonare (WaitMessage.wav e WaitMusic.wav)
        private const string prmUseASRForInput = "UseASRForInput";   // configurazione che indica se usare ASR (se no usa DTMF)
        private const string prmNextServiceTerminated = "NextServiceTerminated";          // Nome servizio successivo su Terminated
        private const string prmNextServiceTransferToOperator = "NextServiceOperator";
        private const string prmNextServiceError = "NextServiceError";             // Nome servizio successivo su Errore
        private const string prmEngineNameTTS = "EngineNameTTS";           // Engine Name TTS
        private const string prmEngineNameASR = "EngineNameASR";           // Engine Name TTS
        private const string prmASRConfidenceThreshold = "ASRConfidenceThreshold";  // Soglia minima confidenza riconoscimento ASR
        private const string prmASRMaxSilence = "ASRMaxSilence";                    // Tempo massimo di riconoscimento
        private const string prmASRLookAheadTime = "ASRLookAheadTime";              // Tempo LookAhead (seconds)
        private const string prmDTMFWaitTime = "DTMFWaitTime";                    // Tempo massimo di digitazione

        private const string prmTTS_DTMF_MenuBegin = "TTS_DTMF_MenuBegin";
        private const string prmTTS_DTMF_MenuFor = "TTS_DTMF_MenuFor";
        private const string prmTTS_DTMF_NumericMenuBegin = "TTS_DTMF_NumericMenuBegin";
        private const string prmTTS_DTMF_NumericMenuAnd = "TTS_DTMF_NumericMenuAnd";
        private const string prmTTS_DTMF_Numeric10 = "TTS_DTMF_Numeric10";
        private const string prmTTS_ASR_MenuBegin = "TTS_ASR_MenuBegin";
        private const string prmTTS_ASR_NumericMenuBegin = "TTS_ASR_NumericMenuBegin";
        private const string prmTTS_ASR_NumericMenuAnd = "TTS_ASR_NumericMenuAnd";

        private const string prmUseSTTForOpenAnswer = "UseSTTForOpenAnswer";
        private const string prmEngineNameSTT = "EngineNameSTT";
        private const string prmEngineLanguageSTT = "EngineLanguageSTT";
        private const string prmStopOnDigit = "StopOnDigit";
        private const string prmStopDelay = "StopDelay";
        private const string prmSTTStopOnDigitPhrase = "STTStopOnDigitPhrase";
        private const string prmSTTEnableBeep = "STTEnableBeep";
        private const string prmSTTMaxSilence = "STTMaxSilence";
        private const string prmSTTConfidenceThreshold = "STTConfidenceThreshold";

        private const string prmSTTNumOfAdditionalParameters = "NumberOfAdditionalParameters";
        private const string prmSTTAdditionalParamName = "AdditionalParamName_";
        private const string prmSTTAdditionalParamValue = "AdditionalParamValue_";

        //  *** StopCall reasons
        private const int msgNull = -1;

        #endregion Constants
        //  *** Constant for ChannelType
        private const string chTypePhoneSwitch_VK = "VK";
        private const string chTypePhoneSwitch_MS = "MS";

        private const string QueryIFlowState = "QueryIFlowState";
        private const string VoiceInteractionState = "VoiceInteractionState";
        private const string TerminateState = "TerminateState";

        bool mInsideRecognize = false;
        bool mSecondAsyncPlay = false;
        int mPlayQueueResult = -1;

        #region Enum
        public enum EngineType
        {
            TTS = 0,
            ASR = 1,
            STT = 4
        }

        #endregion Enum

        #region Data Members

        private dynamic mHandler = null;
        private HelperClass mTools = null;
        private FlowConversationManager flowConversationManager;
        private bool firstIFlowCall = true;

        private string mWasabiUrl = "";
        private string mWasabiApiSecret = "";
        private string mWasabiApiId = "";
        private int mWasabiTimeout = 30;
        private string mFlowId = "";
        private string mInteractionId = "";
        private int mConversationId = 0;

        private string messagePath = "";
        string fileNameSilence = ""; // file to play silence during ASR with full path
        string fileNameWaitMusic = ""; // file to play as wait music during Wasabi call
        string fileNameBeep = "";
        IvrIFlow ivrIFlow;
        private string answerDone = "";
        //private string nextService = "";
        private string nextServiceTerminated = "";
        //private string nextServiceNoSelection = "";
        private string nextServiceTransferToOperator = "";
        private string nextServiceError = "";
        private string engineNameTTS = "";
        private string engineNameASR = "";
        private string mEngLang = "";
        int mEngRate = 0;
        int lang = 0;

        //private bool ASRBeepEnable = false;
        private int ASRConfidenceThreshold = 70;
        private int ASRMaxSilence = 20;
        private int ASRLookAheadTime = 800;

        private int DTMFMaxWaitTime = 5;

        private string useASRForInput = "";
        private string TTS_DTMF_MenuBegin = "";
        private string TTS_DTMF_MenuFor = "";
        private string TTS_DTMF_NumericMenuBegin = "";
        private string TTS_DTMF_NumericMenuAnd = "";
        private string TTS_DTMF_Numeric10 = "";
        private string TTS_ASR_MenuBegin = "";
        private string TTS_ASR_NumericMenuBegin = "";
        private string TTS_ASR_NumericMenuAnd = "";

        private string UseSTTForOpenAnswer = "";
        private string EngineNameSTT = "";
        private string EngineLanguageSTT = "";

        private bool StopRecOnDigit = true;
        private int StopDelay = 0;
        private string STTStopOnDigitPhrase = "";
        private bool STTEnableBeep = true;
        private int STTConfidenceThreshold = 70;
        private int STTMaxSilence = 20;
        private string STTAdditionalParameters = "";

        private int CHATtimeout = 30;
        private string CHATmessage = "";
        private const int DefaultWasabiTimeout = 25;

        #endregion Data Members

        #region Constructors
        public CService()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        }
        #endregion Constructors

        #region Public Methods
        public void inizialize(dynamic voiceHandler)
        {
            useASRForInput = "";
            mHandler = voiceHandler;
            mTools = new HelperClass(mHandler, "WasabiIvrIFlow");
            mTools.LogString($"Initialize - Loading configuration parameters - Service Version={System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()}");
            ivrIFlow = new IvrIFlow(mTools);

            // Wasabi URL
            mWasabiUrl = mTools.GetParamValue(prmWasabiURL);
            // Wasabi API ID
            mWasabiApiId = mTools.GetParamValue(prmWasabiApiId);
            // Wasabi API SECRET
            mWasabiApiSecret = mTools.GetParamValue(prmWasabiApiSecret);
            // Request TIMEOUT
            int.TryParse(mTools.GetParamValue(prmWasabiTimeout), out mWasabiTimeout);
            mWasabiTimeout = (mWasabiTimeout == 0) ? DefaultWasabiTimeout : mWasabiTimeout;
            mTools.LogString($"Initialize - mWasabiTimeout={mWasabiTimeout.ToString()}");
            mFlowId = mTools.GetParamValue(prmFlowId);
            mInteractionId = mTools.GetCallDataValue("InteractionId");

            mTools.LogString("Initialize - Creating FlowConversationManager object");
            flowConversationManager = new FlowConversationManager(mWasabiUrl, mWasabiApiId, mWasabiApiSecret, mWasabiTimeout, mTools);
            mTools.LogString("Initialize - Created FlowConversationManager object");

            ivrIFlow.ServiceType = (mTools.GetParamValue(prmTelefonico) == "1") ? ServiceType.TELEFONICO : ivrIFlow.ServiceType = ServiceType.CHAT;

            if (ivrIFlow.ServiceType == ServiceType.CHAT) // blocchetto Chat
            {
                mTools.LogString($"Read CHAT Parameters --------");
                CHATtimeout = Convert.ToInt32(mTools.GetParamValue(prmCHATtimeout));
            }
            else  // blocchetto Telefonico
            {
                mTools.LogString($"Read PHONES Parameters --------");
                messagePath = mHandler.baseMsgPath + mTools.GetParamValue(prmMessagesPath);
                useASRForInput = mTools.GetParamValue(prmUseASRForInput);
                engineNameTTS = mTools.GetParamValue(prmEngineNameTTS).ToUpper();

                mTools.LogString($"useASRForInput = {useASRForInput}");
                if (useASRForInput=="1")
                {
                    mTools.LogString($"Read ASR Parameters ----");
                    engineNameASR = mTools.GetParamValue(prmEngineNameASR).ToUpper();
                    //ASRBeepEnable = (mTools.GetParamValue(prmASRBeepEnable) == "1"); // ---@@@                 
                    ASRConfidenceThreshold = Convert.ToInt32(mTools.GetParamValue(prmASRConfidenceThreshold));
                    ASRConfidenceThreshold = (ASRConfidenceThreshold == 0 ? 70 : ASRConfidenceThreshold);
                    ASRMaxSilence = Convert.ToInt32(mTools.GetParamValue(prmASRMaxSilence));
                    ASRMaxSilence = (ASRMaxSilence == 0 ? 20 : ASRMaxSilence);
                    ASRLookAheadTime = Convert.ToInt32(mTools.GetParamValue(prmASRLookAheadTime));
                    ASRLookAheadTime = (ASRLookAheadTime == 0 ? 20 : ASRLookAheadTime);
                    TTS_ASR_MenuBegin = mTools.GetParamValue(prmTTS_ASR_MenuBegin);
                    TTS_ASR_NumericMenuBegin = mTools.GetParamValue(prmTTS_ASR_NumericMenuBegin);
                    TTS_ASR_NumericMenuAnd = mTools.GetParamValue(prmTTS_ASR_NumericMenuAnd);
                }
                mTools.LogString($"Read STT Parameters ----");
                UseSTTForOpenAnswer = mTools.GetParamValue(prmUseSTTForOpenAnswer);
                EngineNameSTT = mTools.GetParamValue(prmEngineNameSTT);
                EngineLanguageSTT = mTools.GetParamValue(prmEngineLanguageSTT);
                STTEnableBeep = (mTools.GetParamValue(prmSTTEnableBeep) == "1");
                STTConfidenceThreshold = Convert.ToInt32(mTools.GetParamValue(prmSTTConfidenceThreshold));
                STTConfidenceThreshold = (STTConfidenceThreshold == 0 ? 70 : STTConfidenceThreshold);
                STTMaxSilence = Convert.ToInt32(mTools.GetParamValue(prmSTTMaxSilence));
                STTMaxSilence = (STTMaxSilence == 0 ? 20 : STTMaxSilence);

                STTAdditionalParameters = @"{""Version"": 1, ""GoogleSTTSpecificParameters"": {";
                string maxValStr = mHandler.getParamValue(prmSTTNumOfAdditionalParameters);
                if (!string.IsNullOrEmpty(maxValStr))
                {
                    int maxVal = 0;
                    int.TryParse(maxValStr, out maxVal);
                    if (maxVal > 0)
                    {
                        for (int ind = 1; ind <= maxVal; ind++)
                        {
                            string pName = mHandler.getParamValue(prmSTTAdditionalParamName + ind.ToString());
                            string pValue = mHandler.getParamValue(prmSTTAdditionalParamValue + ind.ToString());

                            if (ind > 1)
                            {
                                STTAdditionalParameters += $@", ""{pName}"": {pValue}";
                            }
                            else
                            {
                                STTAdditionalParameters += $@"""{pName}"": {pValue}";
                            }
                        }
                    }
                }

                STTAdditionalParameters += "}}";


                StopRecOnDigit = (mTools.GetParamValue(prmStopOnDigit) == "1");
                if (StopRecOnDigit)
                {
                    STTStopOnDigitPhrase = mTools.GetParamValue(prmSTTStopOnDigitPhrase);
                    StopDelay = Convert.ToInt32(mTools.GetParamValue(prmStopDelay));
                }

                if (StopDelay < 0)
                {
                    StopDelay = 0;
                };
                if (StopDelay > 5000)
                {
                    StopDelay = 5000;
                };



                mTools.LogString($"Read DTMF Parameters ----");
                DTMFMaxWaitTime = Convert.ToInt32(mTools.GetParamValue(prmDTMFWaitTime));
                DTMFMaxWaitTime = (DTMFMaxWaitTime == 0 ? 10 : DTMFMaxWaitTime);

                TTS_DTMF_MenuBegin = mTools.GetParamValue(prmTTS_DTMF_MenuBegin);
                TTS_DTMF_MenuFor = mTools.GetParamValue(prmTTS_DTMF_MenuFor);
                TTS_DTMF_NumericMenuBegin = mTools.GetParamValue(prmTTS_DTMF_NumericMenuBegin);
                TTS_DTMF_NumericMenuAnd = mTools.GetParamValue(prmTTS_DTMF_NumericMenuAnd);
                TTS_DTMF_Numeric10 = mTools.GetParamValue(prmTTS_DTMF_Numeric10);

                fileNameSilence = Path.Combine(messagePath, "Silence.wav");
                fileNameWaitMusic = Path.Combine(messagePath, "WaitMusic.wav");
                fileNameBeep = Path.Combine(messagePath, "Beep.wav");
            }

            nextServiceTerminated = mTools.GetParamValue(prmNextServiceTerminated);
            nextServiceError = mTools.GetParamValue(prmNextServiceError);
            nextServiceTransferToOperator = mTools.GetParamValue(prmNextServiceTransferToOperator);
            mTools.LogString("Initialize - End");
        }

        public void execute()
        {
            try
            {
                mTools.LogString("Execute - Begin");
                mTools.SetTransferPropertyValue("InteractionClosed", "N");

                string currentState = QueryIFlowState;
                string strProssimoServizio = "";
                bool exit = false;

                while (!exit)
                {
                    // in caso di richiesta di arresto accettabile interrompe
                    if (CheckRemoteHangUp() == true)
                    {
                        mTools.LogString("Execute - RemoteHangUp detected");
                        break;
                    }
                    switch (currentState)
                    {
                        // -----------------------
                        // -- Stato Query IFlow --
                        // -----------------------
                        case (QueryIFlowState):
                            mTools.LogString("Execute - ******* [State: QueryIFlow] ******* ");
                            mHandler.playFileA(fileNameWaitMusic);
                            bool flowControl = QueryIFlow();
                            mHandler.stopVoice();

                            if (!flowControl)
                            {
                                mTools.LogString("Execute - ERROR in QueryIFlow");
                                strProssimoServizio = nextServiceError;
                                currentState = TerminateState;
                            }
                            else
                            {
                                mTools.LogString("Execute - QueryIFlow executed correctly");
                                currentState = VoiceInteractionState;
                            }
                            break;

                        // --------------------------------
                        // -- Stato Voice Interaction    --
                        // --------------------------------
                        case (VoiceInteractionState):
                            mTools.LogString("Execute - ******* [State: VoiceInteractionState] ******* ");

                            // "isTerminated":true,"isError":true <-> GENERICO MESSAGGIO DI FINE FLUSSO
                            // "IsTransferToHumanAgent": true <-> TRASFERIMENTO A OPERATORE UMANO
                            // FALLBACK rientra in qs tipologia trattato come generico errore (=> IsError=true e suono contenuto Message)
                            if (ivrIFlow.IsTerminated && ivrIFlow.IsError)
                            {
                                if (ivrIFlow.ServiceType == ServiceType.CHAT) // blocchetto Chat
                                {
                                    mTools.LogString("Execute - Terminated with Error - writeOnlyCHAT()");
                                    CHATmessage = ivrIFlow.CreateChatMessage();
                                    writeOnlyCHAT();
                                }
                                else // blocchetto Telefonico
                                {
                                    mTools.LogString("Execute - Terminated with Error - playOnlyTTS()");
                                    playOnlyTTS();
                                }
                                mTools.SetTransferPropertyValue("InteractionClosed", "Y"); //---@@@ continuo a gestire chiusura interazione allo stesso modo di prima
                                strProssimoServizio = nextServiceTerminated;
                                currentState = TerminateState;
                                break;
                            }

                            // se API Wasabi vecchia versione (WasabiCrm.IFlow.Api vers < 1.12.0) non ha IsTransferToHumanAgent, gestisco i trasferimenti in base al CurrentStepType
                            if (ivrIFlow.IsTransferToHumanAgent == null)
                            {
                                mTools.LogString("Execute - IsTransferToHumanAgent = null (WasabiCrm.IFlow.Api vers < 1.12.0)");
                                if (ivrIFlow.CurrentStepType == CurrentStepType_Type.TrasferimentoBot)
                                {
                                    // IFlow.TransferToFlow non dovrebbe capitare
                                    mTools.LogString("Execute - Transfer to Bot CurrentStepType");
                                    strProssimoServizio = nextServiceError;
                                    currentState = TerminateState;
                                    break;
                                }
                                else if (ivrIFlow.CurrentStepType == CurrentStepType_Type.TrasferimentoOperatore)
                                { 
                                    if (ivrIFlow.ServiceType == ServiceType.CHAT) // blocchetto Chat
                                    {
                                        mTools.LogString("Execute - Transfer to Operator CurrentStepType");
                                        CHATmessage = ivrIFlow.CreateChatMessage();
                                        writeOnlyCHAT();
                                    }
                                    else // blocchetto Telefonico
                                    {
                                        mTools.LogString("Execute - Transfer to Operator CurrentStepType - playOnlyTTS()");
                                        playOnlyTTS();
                                    }
                                    strProssimoServizio = nextServiceTransferToOperator;
                                    currentState = TerminateState;
                                    break;
                                }
                            }

                            mTools.LogString("Execute - CurrentStepType = " + ivrIFlow.CurrentStepType);
                            if (ivrIFlow.IsTerminated) // terminated
                            {
                                OutputMessage(); // message
                                mTools.SetTransferPropertyValue("InteractionClosed", "Y");
                                strProssimoServizio = nextServiceTerminated;
                                currentState = TerminateState;
                                break;
                            }
                            else if (ivrIFlow.IsTransferToHumanAgent == true) // transfer To operator
                            {
                                if (ivrIFlow.ServiceType == ServiceType.CHAT) // blocchetto Chat
                                {
                                    mTools.LogString("Execute - Transfer to Operator CurrentStepType");
                                    CHATmessage = ivrIFlow.CreateChatMessage();
                                    writeOnlyCHAT();
                                }
                                else // blocchetto Telefonico
                                {
                                    mTools.LogString("Execute - Transfer to Operator CurrentStepType - playOnlyTTS()");
                                    playOnlyTTS();
                                }
                                strProssimoServizio = nextServiceTransferToOperator;
                                currentState = TerminateState;
                                break;
                            }
                            else if (!ivrIFlow.UserInputRequired) // not user input required
                            {
                                OutputMessage(); // message
                                currentState = QueryIFlowState;
                                break;
                            }

                            // user input required => gestisco raccolta input e validazione
                            if (ivrIFlow.ServiceType == ServiceType.CHAT) // blocchetto Chat
                            {
                                mTools.LogString("Execute - input=Chat");
                                CHATmessage = ivrIFlow.CreateChatMessage();
                                answerDone = WriteAndReadCHAT();
                            }
                            else // blocchetto Telefonico
                            {
                                switch (ivrIFlow.vocalInputType)
                                {
                                    case VocalInputType.DTMF:
                                        mTools.LogString("Execute - input=DTMF - playAndRecognizeDTMF()");
                                        answerDone = playAndRecognizeDTMF();
                                        break;

                                    case VocalInputType.STT:
                                        mTools.LogString("Execute - input=STT - playAndRecognizeSTT()");
                                        answerDone = playAndRecognizeSTT();
                                        break;

                                    case VocalInputType.ASR:
                                        mTools.LogString("Execute - input=ASR - playAndRecognizeASR()");
                                        answerDone = playAndRecognizeASR();
                                        break;

                                    default:
                                        Console.WriteLine($"Execute - undefined input");
                                        break;
                                }
                            }

                            mTools.LogString($"Execute - answerDone : {answerDone}");
                            if (answerDone.Length > 0) // ---@@@ check degli output
                            {
                                mTools.LogString($"Execute - answerDone: {answerDone}");
                                currentState = QueryIFlowState;
                            }
                            else
                            {
                                answerDone = "-";
                                mTools.LogString($"Execute - answerDone empty");
                                currentState = QueryIFlowState;
                            }
                            break;

                        // --------------------
                        // -- Stato Terminate --
                        // --------------------
                        case (TerminateState):
                            mTools.LogString("Execute - ******* [State: Terminate] ******* ");
                            mTools.LogString("Execute - Posting next service "  + nextServiceTerminated);
                            mHandler.postNextService(strProssimoServizio);
                            return;
                    }
                }
                mTools.LogString("Execute - End");
            }
            catch (Exception exc)
            {
                mTools.LogString("Execute - Error:" + exc.Message);
                mHandler.postNextService(nextServiceError);
                return;
            }
        }

        private string WriteAndReadCHAT()
        {
            if (ivrIFlow.QuestionText.Length > 0)
            {
                mHandler.WriteChatMessage(CHATmessage);
                string scelta = CheckNull(mHandler.ReadChatMessage(CHATtimeout));
                mTools.LogString($"WriteAndReadCHAT - scelta first output: {scelta}"); // if firtst output empty => jump
                if (scelta.Length > 0)
                {
                    // parse first output and check if valid
                    int startIndex1 = scelta.IndexOf("__Body__ = ") + "__Body__ = ".Length;
                    if (startIndex1 >= "__Body__ = ".Length)
                    {
                        string valueSubstring1 = scelta.Substring(startIndex1);
                        int endIndex1 = valueSubstring1.IndexOf("}");
                        if (endIndex1 >= 0)
                        {
                            scelta = valueSubstring1.Substring(0, endIndex1);
                            mTools.LogString($"WriteAndReadCHAT ReadTextMessage: {scelta}");
                            if (!ivrIFlow.IsOpenAnswer) // se risposta aperta mon leggo il secondo msg
                            {
                                scelta = ivrIFlow.CheckValidAnswer(scelta);   //"controllare che scelta valida, tra quelle possibili"                
                                if (scelta.Length > 0)
                                {
                                    scelta = mHandler.ReadChatMessage();
                                    mTools.LogString($"WriteAndReadCHAT - scelta second output: {scelta}");
                                    if (scelta.Length > 0)
                                    {
                                        // trova l'indice di inizio di "*begin*ButtonValue%%%"
                                        int startIndex = scelta.IndexOf("__Body__ = *begin*ButtonValue%%%") + "__Body__ = *begin*ButtonValue%%%".Length;
                                        if (startIndex >= 0)
                                        {
                                            // estrae la sottostringa dopo "*begin*ButtonValue%%%"
                                            string valueSubstring = scelta.Substring(startIndex);
                                            // trova l'indice del carattere terminatore "§end§"
                                            int endIndex = valueSubstring.IndexOf("§end§");
                                            if (endIndex1 >= 0)
                                            {
                                                // estrai il valore numerico
                                                scelta = valueSubstring.Substring(0, endIndex);
                                                mTools.LogString($"WriteAndReadCHAT ReadTextMessage: {scelta}");
                                                if (scelta.Length > 0)
                                                {
                                                    return ivrIFlow.ConvertDTMFToAnswer(scelta); // ricavo dalla chiave (id) lo startDate     
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                mTools.LogString($"WriteAndReadCHAT ReadTextMessage (openAnswer): {scelta}");
                                return scelta; // risposta aperta
                            }
                        }
                    }
                }
            }
            else
            {
                mTools.LogString($"Execute - QuestionText is empty");
            }
            return "";
        }

        private bool QueryIFlow()
        {
            if (firstIFlowCall)
            {
                mTools.LogString("QueryIFlow first call");
                firstIFlowCall = false;
                if (flowConversationManager == null)
                {
                    mTools.LogString("QueryIFlow ERROR: flowConversationManager == null (CreateConversation)");
                    return false;
                }
                mTools.LogString($"QueryIFlow - Calling CreateConversation: {mFlowId}, {mInteractionId}");
                FlowConversationResponse response = flowConversationManager.CreateConversation(mFlowId, mInteractionId, null);
                mTools.LogString("QueryIFlow - Called CreateConversation");

                if (response != null)
                {
                    ivrIFlow.PopulateFromResponse(response);
                    ivrIFlow.DetectVocalInputType(useASRForInput);
                    mConversationId = response.ConversationId;
                    mTools.LogString($"QueryIFlow - ConversationId = {mConversationId}");
                }
                else
                {
                    mTools.LogString("QueryIFlow ERROR: response == null");
                    return false;
                }
            }
            else
            {
                mTools.LogString("QueryIFlow following calls");
                if (flowConversationManager == null)
                {
                    mTools.LogString("QueryIFlow ERROR: flowConversationManager == null (ContinueConversation)");
                    return false;
                }
                mTools.LogString($"QueryIFlow  - Calling ContinueConversation: {mFlowId}, {mInteractionId}");
                FlowConversationResponse response = flowConversationManager.ContinueConversation(mConversationId, answerDone);
                mTools.LogString("QueryIFlow - Called ContinueConversation");

                if (response != null)
                {
                    ivrIFlow.PopulateFromResponse(response);
                    ivrIFlow.DetectVocalInputType(useASRForInput);
                    mConversationId = response.ConversationId;
                    mTools.LogString($"QueryIFlow  - ConversationId = {mConversationId}");
                }
                else
                {
                    mTools.LogString("QueryIFlow ERROR: response == null");
                    return false;
                }
            }
            mTools.LogString($"check IvrBot.IsOpenAnswer={ivrIFlow.IsOpenAnswer}");
            return true;
        }

        private void OutputMessage()
        {
            // message
            if (ivrIFlow.ServiceType == ServiceType.CHAT) // blocchetto Chat
            {
                mTools.LogString("OutputMessage - chat message");
                CHATmessage = ivrIFlow.CreateChatMessage();
                writeOnlyCHAT();
            }
            else // blocchetto Telefonico
            {
                mTools.LogString("OutputMessage - phone message");
                playOnlyTTS();
            }
        }

        private string CheckNull(string Value)
        {
            return (Value == null) ? "" : Value;
        }

        public void PlayQueueDone(long QueueResult)
        {
            mPlayQueueResult = Convert.ToInt32(QueueResult);
            if (ivrIFlow.vocalInputType == VocalInputType.ASR)
            {
                mTools.LogString($"PlayQueueDone - ASR Case");
                mTools.LogString($"PlayQueueDone - Received PlayQueueDone: QueueResult = {QueueResult}");
                mTools.LogString($"PlayQueueDone - Received PlayQueueDone: mSecondAsyncPlay = {mSecondAsyncPlay}");
                mTools.LogString($"PlayQueueDone - Received PlayQueueDone: mInsideRecognize = {mInsideRecognize}");

                if (mPlayQueueResult == (int)HelperClass.ActionResults.vvpTermDigit)
                {
                    mHandler.stopVoice();
                    mHandler.lhASRTTSCtrl.DeallocateEngine();
                }

                if ((!mSecondAsyncPlay) && mInsideRecognize)
                {
                    //mTools.LogString("PlayQueueDone - firstAsyncPlay and insideRecognize >> Calling AsrPromptMessagesTerminated");
                    //mHandler.lhASRTTSCtrl.AsrPromptMessagesTerminated();
                    mSecondAsyncPlay = true;

                    // Non e' stata ricevuta alcuna richiesta di end call
                    if (mHandler.stopReason == msgNull)
                    {
                        mPlayQueueResult = -1;
                        mHandler.playFileA(fileNameSilence);
                    }
                }
            }
            else
            {
                mTools.LogString($"PlayQueueDone - STT Case");
                mTools.LogString($"PlayQueueDone - Received PlayQueueDone: QueueResult = {QueueResult}");
                mTools.LogString($"PlayQueueDone - Received PlayQueueDone: mSecondAsyncPlay = {mSecondAsyncPlay}");
                mTools.LogString($"PlayQueueDone - Received PlayQueueDone: mInsideRecognize = {mInsideRecognize}");
                mTools.LogString($"PlayQueueDone - Received PlayQueueDone: StopRecOnDigit = {StopRecOnDigit}");

                mPlayQueueResult = Convert.ToInt32(QueueResult);
                if ((!mSecondAsyncPlay) && mInsideRecognize)
                {
                    mTools.LogString("PlayQueueDone - Calling SttPromptMessagesTerminated");
                    mHandler.lhASRTTSCtrl.SttPromptMessagesTerminated();
                    //mSecondAsyncPlay = true;
                }

                if (mSecondAsyncPlay && StopRecOnDigit && mInsideRecognize)
                {
                    if (mPlayQueueResult == (int)HelperClass.ActionResults.vvpTermDigit)
                    {
                        mTools.LogString($"PlayQueueDone - TermDigit detected: stopping recognition. Delay = {StopDelay} ms");
                        if (StopDelay > 0)
                        {
                            Thread.Sleep(StopDelay);
                        }
                        mHandler.stopVoice();
                        mHandler.lhASRTTSCtrl.StopSpeechToText();
                    }
                    else
                    {
                        mTools.LogString("PlayQueueDone - Starting new async play");
                        mHandler.playFileA(fileNameSilence, "@", false);
                    }
                }
            }
        }
        public void AsrRecognitionEvent(int eventType)
        {
            mTools.LogString($"AsrRecognitionEvent - EventType : {eventType} - isSecondAsyncPlay : {mSecondAsyncPlay}");
            if ((!mSecondAsyncPlay) && (eventType == 1))
            {
                mTools.LogString($"AsrRecognitionEvent - firstAsyncPlay and stopPrompt >> stopVoice");
                mHandler.stopVoice();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private bool CheckRemoteHangUp()
        {
            //mTools.LogString($"@@@@@@ CheckRemoteHangUp - <stopReason,interruptable>=<{mHandler.stopReason},{mHandler.interruptable}>");
            if (mHandler.stopReason != msgNull && mHandler.interruptable == true)
            {
                mTools.LogString("CheckRemoteHangUp - true");
                return true;
            }
            return false;
        }

        private int ConvertParamToInt(string paramValue, int defaultValue)
        {
            int number = 0;
            bool isParsable = Int32.TryParse(paramValue, out number);
            if (isParsable)
                return number;
            else
                return defaultValue;
        }

        private void playOnlyTTS()
        {
            mTools.LogString($"playOnlyTTS - TTS: {ivrIFlow.QuestionText}");
            Stopwatch swLog = Stopwatch.StartNew();
            PlayStringTTS(ivrIFlow.QuestionText, "@", false);
            swLog.Stop();
            mTools.LogString($"playOnlyTTS - [playOnlyTTS][TIMER][ms] elapsed={swLog.ElapsedMilliseconds}");
            mHandler.lhASRTTSCtrl.DeallocateEngine();
            return;
        }

        private void writeOnlyCHAT()
        {
            mTools.LogString($"writeOnlyCHAT - text: {ivrIFlow.QuestionText}");
            if (ivrIFlow.QuestionText.Length > 0)
                mHandler.WriteChatMessage(CHATmessage);
            return;
        }
        private string playAndRecognizeDTMF()
        {
            string sceltaDTMF;
            string answer;
            string questionTextWithAnswers;

            mTools.LogString("playAndRecognizeDTMF - Begin");
         
            mTools.LogString($"playAndDTMFInput TTS: {ivrIFlow.QuestionText}");
            if (ivrIFlow.IsUserInputNumeric)
            {
                PlayStringTTS(ivrIFlow.QuestionText, "@", false);
            }
            else
            {
                questionTextWithAnswers = ivrIFlow.AddAvailableAnswersToQuestionTextDTMF(TTS_DTMF_MenuBegin, TTS_DTMF_MenuFor, TTS_DTMF_NumericMenuBegin, TTS_DTMF_NumericMenuAnd, TTS_DTMF_Numeric10);
                PlayStringTTS(questionTextWithAnswers, "@", false);
            }
            mTools.LogString($"playAndRecognizeDTMF -  DTMFMaxWaitTime : {DTMFMaxWaitTime}; AnswerMaxDigitsDTMF : {ivrIFlow.AnswerMaxDigitsDTMF}");
            sceltaDTMF = mHandler.getDigits("", 0, null, ivrIFlow.AnswerMaxDigitsDTMF, "", DTMFMaxWaitTime, DTMFMaxWaitTime, false);
            mTools.LogString($"playAndRecognizeDTMF - selected : {sceltaDTMF}");

            mHandler.lhASRTTSCtrl.DeallocateEngine();
            mTools.LogString($"playAndRecognizeDTMF - ConvertDTMFToAnswer");
            if (ivrIFlow.IsUserInputNumeric)
            {
                answer = sceltaDTMF;
            }
            else
            {
                answer = ivrIFlow.ConvertDTMFToAnswer(sceltaDTMF);
            }
            mTools.LogString($"Execute - playAndRecognizeDTMF - answer : {answer}");
            mTools.LogString("Execute - playAndRecognizeDTMF End");
            return answer;
        }
        private string playAndRecognizeASR()
        {
            string recognized = "";
            string dictAnswers = "";
            string questionTextWithAnswers;

            mTools.LogString($"playAndRecognizeASR - begin");
            if (AllocateEngineASR(engineNameASR))
            {
                mSecondAsyncPlay = false;
                dictAnswers = ivrIFlow.AddAvailableAnswerToASRDict();               
                mTools.LogString($"playAndRecognizeASR TTS: {ivrIFlow.QuestionText}");
                questionTextWithAnswers = ivrIFlow.AddAvailableAnswersToQuestionTextASR(TTS_ASR_MenuBegin, TTS_ASR_NumericMenuBegin, TTS_ASR_NumericMenuAnd);
                PlayStringTTS(questionTextWithAnswers, "", true);
                mInsideRecognize = true;
                mTools.LogString($"playAndRecognizeASR - Start RecognizeStringFromListEx");
                Stopwatch swLog = Stopwatch.StartNew();
                recognized = mHandler.lhASRTTSCtrl.RecognizeStringFromListEx(ASRConfidenceThreshold, ASRMaxSilence, ASRLookAheadTime, false, true, dictAnswers, false, "").Trim();
                swLog.Stop();
                mTools.LogString($"Execute - [RecognizeStringFromListEx][TIMER][ms] elapsed={swLog.ElapsedMilliseconds}");

                HelperClass.ActionResults actResult = mTools.ActionResult;
                mTools.LogString($"playAndRecognizeASR - RecognizeStringFromListEx - actionResult : {actResult} - recognized : {recognized}");
                mInsideRecognize = false;

                mHandler.stopVoice();
                mHandler.lhASRTTSCtrl.DeallocateEngine();
                mTools.LogString($"playAndRecognizeASR - end");
                return recognized;
            }
            else
            {
                mTools.LogString($"playAndRecognizeASR - failed");
                return "";
            }
        }

        private string playAndRecognizeSTT()
        {
            string recognized = "";

            mTools.LogString($"playAndRecognizeSTT - begin");

            if (AllocateEngineSTT(EngineNameSTT))
            {
                mSecondAsyncPlay = true;
                ivrIFlow.QuestionText = ivrIFlow.QuestionText + STTStopOnDigitPhrase;
                mTools.LogString($"Execute - playAndRecognizeSTT TTS: {ivrIFlow.QuestionText}");
                Stopwatch swLog = Stopwatch.StartNew();
                PlayStringTTS(ivrIFlow.QuestionText, "", false);
                swLog.Stop();
                mTools.LogString($"Execute - [PlayStringTTS][TIMER][ms] elapsed={swLog.ElapsedMilliseconds}");
                if (STTEnableBeep)
                    mHandler.playFile(fileNameBeep);
                mHandler.playFileA(fileNameSilence, "@", false);
                mInsideRecognize = true;
                mTools.LogString($"Calling SpeechToTextEx2 - EngineName = {EngineNameSTT} - Language = {EngineLanguageSTT} - Confidence = {STTConfidenceThreshold} - MaxSilence = {STTMaxSilence}");
                mTools.LogString($"                        - STTAdditionalParameters= {STTAdditionalParameters}");
                recognized = mHandler.lhASRTTSCtrl.SpeechToTextEx2(EngineNameSTT, EngineLanguageSTT, STTConfidenceThreshold, STTMaxSilence, false, false, "", 0, true, STTAdditionalParameters);
                HelperClass.ActionResults actResult = mTools.ActionResult;
                mTools.LogString($"playAndRecognizeSTT - SpeechToTextEx - actionResult : {actResult} - recognized : {recognized}");
                mInsideRecognize = false;

                mHandler.stopVoice();
                mHandler.lhASRTTSCtrl.DeallocateEngine();
                mTools.LogString($"playAndRecognizeSTT - end");
                return recognized;
            }
            else
            {
                mTools.LogString($"playAndRecognizeSTT - failed");
                return "";
            }
        }


        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.RequestingAssembly == null)
            {
                if (null != mTools)
                {
                    mTools.LogString(string.Format("MediaManager:: OnAssemblyResolve - AssemblyResolve ERROR loading {0}: RequestingAssembly is null", args.Name));
                }
            }
            string location = (new FileInfo(args.RequestingAssembly.Location)).DirectoryName;
            string name = args.Name.Substring(0, args.Name.IndexOf(','));
            Assembly asm = null;

            if (null != mTools)
            {
                mTools.LogString(string.Format("MediaManager:: OnAssemblyResolve - Resolving assembly: Name = {0} - Directory = {1}", name, location));
            }

            try
            {
                asm = Assembly.LoadFile(Path.Combine(location, string.Format("{0}.dll", name)));
                if (null != mTools)
                {
                    mTools.LogString(string.Format("MediaManager:: OnAssemblyResolve - Resolved in {0}", location));
                }
            }
            catch (Exception ex)
            {
                if (null != mTools)
                {
                    mTools.LogString(string.Format("MediaManager:: OnAssemblyResolve - AssemblyResolve ERROR: unable to load assembly {0} from folder {1}: /n {2}", args.Name, location, ex.ToString()));
                }

                try
                {
                    asm = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.dll", name)));
                    if (null != mTools)
                    {
                        mTools.LogString(string.Format("MediaManager:: OnAssemblyResolve - Resolved in {0}", AppDomain.CurrentDomain.BaseDirectory));
                    }
                }
                catch (Exception ex1)
                {
                    if (null != mTools)
                    {
                        mTools.LogString(string.Format("MediaManager:: OnAssemblyResolve - AssemblyResolve ERROR: unable to load assembly {0} from folder {1}: /n {2}", args.Name, AppDomain.CurrentDomain.BaseDirectory, ex1.ToString()));
                    }
                }
            }

            return asm;
        }

        #endregion Private Methods

        #region TTS ASR Methods
        private bool AllocateEngineTTS()
        {
            string engineName = "";
            try
            {
                mTools.LogString("AllocateEngineTTS - EngName = " + engineNameTTS + " Language = " + mEngLang);
                if (mHandler.boardChType == chTypePhoneSwitch_VK || mHandler.boardChType == chTypePhoneSwitch_MS)
                {
                    mTools.LogString($"AllocateEngineTTS - boardChType = {mHandler.boardChType}");
                    if (mHandler.lhASRTTSCtrl != null)
                    {
                        string[] _params = engineNameTTS.Split('|');
                        mTools.LogString(string.Format("AllocateEngineTTS - _params.Length {0}", _params.Length));
                        switch (_params.Length)
                        { //ti ricordiamo .... è...
                            case (2):
                                engineName = _params[0].Trim();
                                mEngRate = Convert.ToInt16(_params[1]);
                                break;
                            case (3):
                                engineName = _params[0].Trim();
                                mEngRate = Convert.ToInt16(_params[1]);
                                mEngLang = _params[2].Replace('(', ' ').Replace(')', ' ').Trim();
                                break;
                            default:
                                mTools.LogString("AllocateEngineTTS - parameters lenght unable");
                                return false;
                        }
                        mTools.LogString("AllocateEngineTTS - EngName=" + engineName + " Language=" + mEngLang);
                        if (mHandler.lhASRTTSCtrl.AllocateEngine(EngineType.TTS, mEngLang, engineName, Convert.ToInt16(mEngRate)) == true)
                        {
                            mTools.LogString("AllocateEngineTTS - AllocateEngine successfully");
                            return true;
                        }
                        else
                        {
                            mTools.LogString("AllocateEngineTTS - AllocateEngine failed - error");
                            return false;
                        }
                    }
                    else
                    {
                        mTools.LogString("AllocateEngineTTS - TTS control unavailable - error");
                        return false;
                    }
                }
                else
                {
                    if (mHandler.VTtsCtrl != null)
                    {
                        mTools.LogString("AllocateEngineTTS - Searching TTS Engine. Available engines: " + mHandler.VTtsCtrl.EngineTotal.ToString());
                        int totalEngines = mHandler.VTtsCtrl.EngineTotal;
                        for (int i = 0; i <= totalEngines; i++)
                        {
                            mHandler.VTtsCtrl.EngineIndex = i;
                            if (Convert.ToInt16(mEngLang) == 0)
                            {
                                mTools.LogString("AllocateEngineTTS - Setting emgLang to " + mHandler.VTtsCtrl.EngineLanguage.ToString());
                                lang = mHandler.VTtsCtrl.EngineLanguage;
                            }
                            else
                                lang = Convert.ToInt16(mEngLang);
                            mTools.LogString("Engine #" + i.ToString() + " " + mHandler.VTtsCtrl.EngineName + " Lang = " + mHandler.VTtsCtrl.EngineLanguage.ToString());
                            if (mHandler.VTtsCtrl.EngineName.ToUpper() == engineNameTTS && mHandler.VTtsCtrl.EngineLanguage == lang)
                            {
                                mTools.LogString("TTS Engine founded: allocating");
                                mHandler.VTtsCtrl.AllocateEngine(i, null, 10);
                                if (mHandler.VTtsCtrl.actionResult == 0)
                                { // vtsOk 
                                    mTools.LogString("TTS Engine allocated");
                                    return true;
                                }
                                else
                                {
                                    mTools.LogString("TTS AllocateEngine ERROR");
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        mTools.LogString("VTtsCtrl unavailable: TTS Engine not found - " + engineNameTTS);
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                mTools.LogString(string.Format("AllocateEngineTTS - ERROR: {0}", ex.ToString()));
                return false;
            }
        }

        private bool AllocateEngineASR(string engineName)
        {
            mTools.LogString("AllocateEngineASR - begin");
            if (mHandler.lhASRTTSCtrl.AllocateEngine(EngineType.ASR, engineName, "a", 0))
            {
                mTools.LogString("AllocateEngineASR - ASR Engine allocated");
                //if (mHandler.VTtsCtrl.actionResult == 0)
                //{ // vtsOk 
                //    mTools.LogString("ASR Engine allocated");
                //    return true;
                //}
                //else
                //{
                //    mTools.LogString("ASR AllocateEngine ERROR");
                //    return false;
                //}

                return true;
            }
            else
            {
                mTools.LogString("AllocateEngineASR - ASR Engine not allocated");
                return false;
            }

        }

        private bool AllocateEngineSTT(string engineName)
        {
            mTools.LogString("AllocateEngineSTT - begin");
            if (mHandler.lhASRTTSCtrl.AllocateEngine(EngineType.STT, "None", engineName, 0))
            {
                mTools.LogString("AllocateEngineASR - STT Engine allocated");
                return true;
            }
            else
            {
                mTools.LogString("AllocateEngineASR - STT Engine not allocated");
                return false;
            }
        }
        private bool PlayStringTTS(string message, string termDigit, bool async)
        {
            try
            {
                mTools.LogString("PlayStringTTS - AllocateEngineTTS()");

                //return true;
                if (AllocateEngineTTS() == true)
                {
                    mTools.LogString("PlayStringTTS - AllocateEngineTTS() done");
                    if (mHandler.boardChType == chTypePhoneSwitch_VK || mHandler.boardChType == chTypePhoneSwitch_MS)
                    {
                        if (async)
                        {
                            mTools.LogString("PlayStringTTS - say string async: " + message);
                            mHandler.lhASRTTSCtrl.SayStringEx(message, "", true, true);
                            mTools.LogString("PlayStringTTS - say string async started");
                        }
                        else
                        {
                            mTools.LogString("PlayStringTTS - say string sync: " + message);
                            mHandler.lhASRTTSCtrl.SayString(message, termDigit);
                            mTools.LogString("PlayStringTTS - say string done!");
                        }
                    }
                    else
                    {
                        mHandler.VTtsCtrl.SayString(message);
                        mTools.LogString("PlayStringTTS - say string done!");
                    }
                }
                else
                {
                    mTools.LogString("PlayStringTTS - AllocateEngineTTS() Failed");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                mTools.LogString(string.Format("PlayStringTTS - ERROR: {0}", ex.ToString()));
                return false;
            }
        }
        #endregion
           
    }
}
