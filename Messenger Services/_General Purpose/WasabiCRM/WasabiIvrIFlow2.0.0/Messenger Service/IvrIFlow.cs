#define DEBUG

using Ifm.Components.Messenger.Blocks.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using WasabiCrm.IFlow.Api.FlowConversations;

namespace Ifm.Components.Messenger.Blocks.CustomMessengerServices
{
    public enum ServiceType
    {
        TELEFONICO = 0,
        CHAT = 1
    }

    public enum VocalInputType
    {
        DTMF = 0,
        ASR = 1,
        STT = 2
    }

    struct CurrentStepType_Type
    {
        public const string Domanda = "IFlow.Question";
        public const string DomandaBase = "IFlow.QuestionBase";
        public const string SceltaCalendario = "Calendars.SearchSlots";
        public const string AiAssistant= "IFlow.AiAssistant";
        public const string Messaggio = "IFlow.Message";
        public const string TrasferimentoBot = "IFlow.TransferToFlow";
        public const string TrasferimentoOperatore = "IFlow.TransferToAgent";
        public const string FineConversazione = "IFlow.ConversationEnd";
    }
    internal class IvrIFlow
    {
        HelperClass mTools;
        public string CurrentStepType { get; set; }
        public string QuestionText { get; set; }
        public bool IsTerminated { get; set; } // ---@@@ gestire caso finale IsTerminated=true && IsError=false      
        public bool IsError { get; set; }
        public bool? IsTransferToHumanAgent { get; set; }
        public bool UserInputRequired { get; set; } 
        public bool IsOpenAnswer { get; set; }

        public bool IsUserInputNumeric { get; set; }
        public int? AnswerMaxDigitsDTMF { get; set; }
        public string AnswerTermDigitDTMF { get; set; }
        public bool ErrorReadingAnswers { get; set; }

        string[] expectedAnswers;

        bool AnswersAreNumericConsecutive = false;
        int[] expectedAnswersNumeric;
        int numericConsecutiveMinValue = 0;
        int numericConsecutiveMaxValue = 0;

        public ServiceType ServiceType { get; set; }
        public VocalInputType vocalInputType { get; set; }

        public IvrIFlow(HelperClass tools)
        {
            mTools = tools;
            mTools.LogString($"IvrIFlow: Constructor - executed");

        }
        
        public void PopulateFromResponse(FlowConversationResponse result)
        {
            mTools.LogString($"IvrIFlow: PopulateFromResponse - begin");
            CurrentStepType = result.CurrentStepType;
            QuestionText = result.Message;
            IsTerminated = result.IsTerminated;
            IsError = result.IsError;
            UserInputRequired = result.UserInputRequired;
            IsTransferToHumanAgent = result.IsTransferToHumanAgent;

            // if (CurrentStepType != CurrentStepType_Type.Domanda 
            //    && CurrentStepType != CurrentStepType_Type.DomandaBase 
            //    && CurrentStepType != CurrentStepType_Type.SceltaCalendario
            //    && CurrentStepType != CurrentStepType_Type.AiAssistant)
            //{
            //    mTools.LogString($"IvrIFlow: CurrentStepType type not answer => don't read answers");
            //    IsOpenAnswer = false;
            //    return;
            //}

            if (result.Choices != null)
            {
                if (result.Choices.Count > 0)
                {
                    mTools.LogString($"IvrIFlow: found {result.Choices.Count} Choices");
                    expectedAnswers = result.Choices.ToArray();
                    IsOpenAnswer = false;
                    ErrorReadingAnswers = false;
                    return;
                }
            }
            mTools.LogString($"IvrIFlow: found no Choices");
            IsOpenAnswer = true;
            if (ServiceType == ServiceType.CHAT)
            {
                mTools.LogString($"PopulateFromResponse - readed no answers - CHAT Open Answer");
            }
            else
            {
                IsUserInputNumeric = result.UserInputNumeric;
                if (IsUserInputNumeric)
                {
                    mTools.LogString($"PopulateFromResponse - readed no answers - PHONE GetDTMF");
                    AnswerMaxDigitsDTMF = result.UserInputMaxLength;
                    mTools.LogString($"PopulateFromResponse - readed no answers - PHONE MaxDigits={AnswerMaxDigitsDTMF}");
                }
                else
                {
                    mTools.LogString($"PopulateFromResponse - readed no answers - PHONE Open Answer");
                }
            }
            ErrorReadingAnswers = false;
            return;
        }

        public void DetectVocalInputType(string useASRForInput)
        { 
            if (IsOpenAnswer)
            {
                vocalInputType = ((IsUserInputNumeric) && !(useASRForInput == "1")) ? VocalInputType.DTMF : VocalInputType.STT;
            }
            else
            {
                vocalInputType = (useASRForInput == "1") ? VocalInputType.ASR : VocalInputType.DTMF;
                if (CheckAnswersAreNumeric())
                {
                    if (CheckConsecutiveNumbers())
                    {
                        mTools.LogString("DetectVocalInputType - numeric answers with consecutive values");
                    }
                    else
                    {
                        mTools.LogString("DetectVocalInputType - numeric answers with not consecutive values");
                    }
                }
                else
                {
                    mTools.LogString("DetectVocalInputType - not numeric answers");
                }
            }
            mTools.LogString($"DetectVocalInputType - vocalInputType = {vocalInputType.ToString()}");
            return;
        }

        public string CreateChatMessage()
        {
            /*  
                *begin*IVRquestion%%%%%%Seleziona la lingua###Italiano###English###0$$end$$
                *begin* header protocollo
                IVRquestion comando
                %%%%%% separatore tra comando e lista parametri
                Seleziona la lingua domanda
                ###Italiano###English###   Lista scelte possibili separate da “###”
                Nota: si posono passare anche chiavi associate a ciascun testo, es: “Italiano|||it” e “English|||en” testo e chiave relativa)
                0   indica se ritornare testo o anche chiave associata (es 0=testo, 1=chiave)
                $$end$$ footer protocollo
            */
         
            mTools.LogString("CreateChatMessage: CreateChatMessage Begin");
            string message = QuestionText;
            if ((IsOpenAnswer) || (!UserInputRequired))
            {
                mTools.LogString($"CreateChatMessage: CreateChatMessage Chat message {message}");
                return message;
            }
            message = "*begin*IVRquestion%%%" + message;
            
            if (CurrentStepType == CurrentStepType_Type.Domanda || CurrentStepType == CurrentStepType_Type.DomandaBase || CurrentStepType == CurrentStepType_Type.SceltaCalendario || CurrentStepType == CurrentStepType_Type.AiAssistant)
            {
                for (int counter = 0; counter < expectedAnswers.Length; counter++)
                {
                    message = message + "###" + expectedAnswers[counter] + "|||" + (counter + 1).ToString();
                }
                message = message + "###1$end$";
            }
            else
                message = message + "$end$";

            mTools.LogString($"CreateChatMessage: CreateChatMessage Chat message {message}");
            return message;
        }

        public string ConvertDTMFToAnswer(string index)
        {
            if (index.Length == 0) return "";
            int scelta = Convert.ToInt16(index);
            if (scelta > 0 && scelta <= expectedAnswers.Length)
                return expectedAnswers.GetValue(scelta - 1).ToString();
            else return "";
        }

        public string CheckValidAnswer(string answer)
        {
            int idx = Array.FindIndex(expectedAnswers, t => t.Equals(answer, StringComparison.InvariantCultureIgnoreCase));
            if (idx >= 0)
                return answer;
            else
                return "";
        }

        public string AddAvailableAnswersToQuestionTextDTMF(string DTMFMenuBegin, string DTMFMenuFor, string DTMFNumericMenuBegin, string DTMFNumericMenuAnd, string DTMFNumeric10)
        {
            int position = 0;
            string question = this.QuestionText;

            mTools.LogString($"addAvailableAnswersToQuestionTextDTMF : begin questionText: {question}");
            if (expectedAnswers.Length > 0)
            {
                if (!AnswersAreNumericConsecutive)
                {
                    question = $"{question} {DTMFMenuBegin}";
                    foreach (string answer in expectedAnswers)
                    {
                        if (position==9)
                        {
                            question = question + $"{DTMFNumeric10}";
                            break;
                        }
                        else
                            question = question + $"{position + 1} {DTMFMenuFor} {answer}, ";
                        position++;
                    }
                }
                else
                {
                    question = question + $"{DTMFNumericMenuBegin} {numericConsecutiveMinValue} {DTMFNumericMenuAnd} {numericConsecutiveMaxValue}";
                    if (numericConsecutiveMaxValue == 10)
                    {
                        question = question + $"{DTMFNumeric10}";
                    }
                }
            }
            mTools.LogString($"addAvailableAnswersToQuestionTextDTMF : full question : {question}");
            return question;
        }

        public string AddAvailableAnswersToQuestionTextASR(string ASRMenuBegin, string ASRNumericMenuBegin, string ASRNumericMenuAnd)
        {
            int position = 0;
            string question = this.QuestionText;

            mTools.LogString($"addAvailableAnswersToQuestionTextASR : begin questionText: {question}");
            if (expectedAnswers.Length > 0)
            {
                if (!AnswersAreNumericConsecutive)
                {
                    question = $"{question} {ASRMenuBegin}";
                    foreach (string answer in expectedAnswers)
                    {
                        question = question + $" {answer}, ";
                        position++;
                    }
                }
                else
                {
                    question = question + $"{ASRNumericMenuBegin} {numericConsecutiveMinValue} {ASRNumericMenuAnd} {numericConsecutiveMaxValue}";
                }
            }
            mTools.LogString($"addAvailableAnswersToQuestionTextASR : full question : {question}");
            return question;
        }

        public bool CheckAnswersAreNumeric()
        {
            if (expectedAnswers != null && expectedAnswers.Length > 0)
            {
                expectedAnswersNumeric = new int[expectedAnswers.Length];
                for (int i = 0; i < expectedAnswers.Length; i++)
                {
                    int n;
                    bool isNumeric = int.TryParse(expectedAnswers[i], out n);
                    if (!isNumeric)
                    {
                        AnswerMaxDigitsDTMF = (expectedAnswers.Length > 10) ? expectedAnswers.Length + 1 : 1;
                        AnswerTermDigitDTMF = (AnswerMaxDigitsDTMF > 1) ? "#-" : "";
                        AnswersAreNumericConsecutive = false;
                        return false;
                    }
                    expectedAnswersNumeric[i] = n;
                }
                Array.Sort(expectedAnswersNumeric);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckConsecutiveNumbers()
        {
            for (int i = 1; i < expectedAnswersNumeric.Length; i++)
            {
                if ((expectedAnswersNumeric[i] - expectedAnswersNumeric[i - 1]) != 1)
                {
                    AnswersAreNumericConsecutive = false;
                    return false;
                }
            }
            numericConsecutiveMinValue = expectedAnswersNumeric[0];
            numericConsecutiveMaxValue = expectedAnswersNumeric[expectedAnswers.Length - 1];
            AnswerMaxDigitsDTMF = (numericConsecutiveMaxValue > 10) ? Convert.ToString(numericConsecutiveMaxValue).Length + 1 : 1;
            AnswerTermDigitDTMF = (AnswerMaxDigitsDTMF > 1) ? "#-" : "";
            AnswersAreNumericConsecutive = true;
            return true;
        }

        public string AddAvailableAnswerToASRDict()
        {
            string globalDict = "";
            string currentDict;
            string currentDESC;
            string currentASR;
            if (expectedAnswers.Length > 0)
            {
                foreach (string answer in expectedAnswers)
                {
                    currentDESC = answer;
                    currentASR = answer;
                    currentDict = $"{currentDESC}:{currentASR}|";
                    mTools.LogString($"addAvailableAnswerToASRDict : currentDict : {currentDict} added to globalDict");
                    globalDict = globalDict + currentDict;
                }
            }
            mTools.LogString($"addAvailableAnswerToASRDict : globalDict : {globalDict}");
            return globalDict;
        }
    }
}
