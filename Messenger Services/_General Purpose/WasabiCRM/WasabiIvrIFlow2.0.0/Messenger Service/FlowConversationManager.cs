using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WasabiCrm.Base.Api;
using WasabiCrm.IFlow.Api.FlowConversations;
using Ifm.Components.Messenger.Blocks.Utilities;

namespace Ifm.Components.Messenger.Blocks.CustomMessengerServices
{
    internal class FlowConversationManager
    {
        public FlowConversationClient flowConversationClient;
        private HelperClass mTools;

        public const string prefixTPVariable = "IFLOW_V_"; // prefisso per tutte le transfer property in cui salvare le variables delle risposte (es: se la variabile si chiama "esitoPagamento" salvo in transfer property "IFLOW_V_esitoPagamento")   
        public const string prefixTPPropertyValue = "IFLOW_P_"; // prefisso per tutte le transfer property in cui salvare le varaibles delle risposte (es: se la variabile si chiama "esitoPagamento" salvo in transfer property "IFLOW_V_esitoPagamento")   
        public const string TPStepId = "IFLOW_STEPID";

        // Costruttore - corrisponde a buttonCreateClient_Click
        public FlowConversationManager(string wasabiUrl, string appId, string appSecret, int wasabiTimeout, HelperClass helperClass)
        {
            this.mTools = helperClass;
            LogString("FlowConversationManager constructor begin");

            flowConversationClient = new FlowConversationClient(wasabiUrl);
            WasabiApiClient.SetTimeout(new TimeSpan(0, 0, wasabiTimeout));

            flowConversationClient.AppId = new Guid(appId);
            flowConversationClient.AppSecret = appSecret;
            
            LogString("FlowConversationManager constructor end");
        }

        public void LogString(string message)
        {
            mTools.LogString(message);
        }

        // Corrisponde a buttonCreateConversation_Click
        public FlowConversationResponse CreateConversation(string flowId, string interactionId, string assistantId = null)
        {
            LogString("CreateConversation begin");

            LogString("CreateConversation Creating FlowConversationCreateRequest");
            FlowConversationCreateRequest request = new FlowConversationCreateRequest();
            request.AssistantId = string.IsNullOrWhiteSpace(assistantId) ? (Guid?)null : Guid.Parse(assistantId);
            request.FlowId = Guid.Parse(flowId);
            request.InteractionId = int.Parse(interactionId);
            LogString($"CreateConversation Created FlowConversationCreateRequest - flowId={request.FlowId}, interactionId={request.InteractionId}");

            LogString("CreateConversation flowConversationClient.CreateAsync()");
            Stopwatch swLog = Stopwatch.StartNew();
            Task<FlowConversationResponse> task = flowConversationClient.CreateAsync(request);
            swLog.Stop();
            LogString($"CreateConversation [CreateAsync][TIMER][ms] elapsed={swLog.ElapsedMilliseconds}");

            if (task.Result != null)
            {
                LogAndStoreResponseData(task.Result, "CreateAsync");
                LogString("CreateConversation end");
                return task.Result;
            }
            else
            {
                LogString("CreateConversation Wasabi flowConversationClient.CreateAsync FAILED: task.Result is null");
                LogString("CreateConversation end");
                return null;
            }
        }

        // Corrisponde a buttonContinueConversation_Click
        public FlowConversationResponse ContinueConversation(int conversationId, string userInput)
        {
            LogString("ContinueConversation begin");

            LogString("ContinueConversation Creating FlowConversationContinueRequest");
            FlowConversationContinueRequest request = new FlowConversationContinueRequest();
            request.UserInput = userInput;
            LogString($"ContinueConversation Created FlowConversationContinueRequest - conversationId={conversationId}, userInput={request.UserInput}");

            LogString("ContinueConversation flowConversationClient.ContinueAsync()");
            Stopwatch swLog = Stopwatch.StartNew();
            Task<FlowConversationResponse> task = flowConversationClient.ContinueAsync(conversationId, request);
            swLog.Stop();
            LogString($"CreateConversation [ContinueAsync][TIMER][ms] elapsed={swLog.ElapsedMilliseconds}");
            
            if (task.Result != null)
            {
                LogAndStoreResponseData(task.Result, "ContinueAsync");
                LogString("ContinueConversation end");
                return task.Result;
            }
            else
            {
                LogString("ContinueConversation Wasabi flowConversationClient.ContinueAsync FAILED: task.Result is null");
                LogString("ContinueConversation end");
                return null;
            }
        }

        private void LogAndStoreResponseData(FlowConversationResponse result, string operationName)
        {
            LogString($"LogAndStoreResponseData - flowConversationClient {operationName} OK - response: ConversationId = {result.ConversationId}");
            LogString($"    CurrentStepType = {result.CurrentStepType}");
            LogString($"    CurrentFlowId = {result.CurrentFlowId}");
            LogString($"    CurrentStepId = {result.CurrentStepId}");
            mTools.SetTransferPropertyValue(TPStepId, result.CurrentStepId.ToString()); // salvo in transfer property la variabile (es: se la variabile si chiama "Prova" salvo in transfer property "IFLOW_V_Prova")
            LogString($"    IsError = {result.IsError}");
            LogString($"    IsTerminated = {result.IsTerminated}");
            LogString($"    IsTransferToHumanAgent = {result.IsTransferToHumanAgent}");
            LogString($"    Message = {result.Message}");
            LogString($"    UserInputRequired = {result.UserInputRequired}");
            LogString($"    UserInputNumeric = {result.UserInputNumeric}");
            LogString($"    UserInputMaxLength = {result.UserInputMaxLength}");
            
            if (result.Choices != null)
            {
                List<string> responseList = result.Choices.ToList();
                foreach (string response in responseList)
                {
                    LogString($"   Choice = {response}");
                }
            }
            else
            {
                LogString("    Choices is null");
            }

            if (result.Variables != null)
            {
                foreach (KeyValuePair<string, object> item in result.Variables)
                {
                    LogString($"    Variable - Key: {item.Key}, Value: {item.Value}");
                    mTools.SetTransferPropertyValue($"{prefixTPVariable}{item.Key}", item.Value.ToString()); // salvo in transfer property la variabile (es: se la variabile si chiama "Prova" salvo in transfer property "IFLOW_V_Prova")
                }
            }
            else            
            {
                LogString("    Variables is null");
            }

            if (result.PropertyValues != null)
            {
                foreach (KeyValuePair<string, object> item in result.PropertyValues)
                {
                    LogString($"    PropertyValue - Key: {item.Key}, Value: {item.Value}");
                    mTools.SetTransferPropertyValue($"{prefixTPPropertyValue}{item.Key}", item.Value.ToString()); // salvo in transfer property la variabile (es: se la variabile si chiama "Prova2" salvo in transfer property "IFLOW_P_Prova2")
                }
            }
            else
            {
                LogString("    PropertyValues is null");
            }
        }
    }
}