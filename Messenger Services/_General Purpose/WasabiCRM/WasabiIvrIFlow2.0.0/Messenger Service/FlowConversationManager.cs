using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WasabiCrm.Base.Api;
using WasabiCrm.IFlow.Api.FlowConversations;

namespace Ifm.Components.Messenger.Blocks.CustomMessengerServices
{
    public class FlowConversationManager
    {
        private FlowConversationClient flowConversationClient;
        private Action<string> logAction;

        // Costruttore - corrisponde a buttonCreateClient_Click
        public FlowConversationManager(string wasabiUrl, string appId, string appSecret, int wasabiTimeout, Action<string> logAction)
        {
            this.logAction = logAction;
            LogString("FlowConversationManager constructor begin");

            flowConversationClient = new FlowConversationClient(wasabiUrl);
            WasabiApiClient.SetTimeout(new TimeSpan(0, 0, wasabiTimeout));

            flowConversationClient.AppId = new Guid(appId);
            flowConversationClient.AppSecret = appSecret;
            
            LogString("FlowConversationManager constructor end");
        }

        private void LogString(string message)
        {
            logAction?.Invoke(message);
        }

        // Corrisponde a buttonCreateConversation_Click
        public FlowConversationResponse CreateConversation(string flowId, string interactionId, string assistantId = null)
        {
            LogString("CreateConversation begin");

            LogString("Creating FlowConversationCreateRequest");
            FlowConversationCreateRequest request = new FlowConversationCreateRequest();
            request.AssistantId = string.IsNullOrWhiteSpace(assistantId) ? (Guid?)null : Guid.Parse(assistantId);
            request.FlowId = Guid.Parse(flowId);
            request.InteractionId = int.Parse(interactionId);
            LogString($"Created FlowConversationCreateRequest - flowId={request.FlowId}, interactionId={request.InteractionId}");

            LogString("flowConversationClient calling CreateAsync");
            Task<FlowConversationResponse> task = flowConversationClient.CreateAsync(request);
            
            if (task.Result != null)
            {
                LogResult(task.Result, "CreateAsync");
                LogString("CreateConversation end");
                return task.Result;
            }
            else
            {
                LogString("Execute - Wasabi flowConversationClient.CreateAsync FAILED: task.Result is null");
                LogString("CreateConversation end");
                return null;
            }
        }

        // Corrisponde a buttonContinueConversation_Click
        public FlowConversationResponse ContinueConversation(int conversationId, string userInput)
        {
            LogString("ContinueConversation begin");

            LogString("Creating FlowConversationContinueRequest");
            FlowConversationContinueRequest request = new FlowConversationContinueRequest();
            request.UserInput = userInput;
            LogString($"Created FlowConversationContinueRequest - conversationId={conversationId}, userInput={request.UserInput}");

            LogString("flowConversationClient calling ContinueAsync");
            Task<FlowConversationResponse> task = flowConversationClient.ContinueAsync(conversationId, request);
            
            if (task.Result != null)
            {
                LogResult(task.Result, "ContinueAsync");
                LogString("ContinueConversation end");
                return task.Result;
            }
            else
            {
                LogString("Execute - Wasabi flowConversationClient.ContinueAsync FAILED: task.Result is null");
                LogString("ContinueConversation end");
                return null;
            }
        }

        private void LogResult(FlowConversationResponse result, string operationName)
        {
            LogString($"Execute - Wasabi flowConversationClient {operationName} OK - response: ConversationId = {result.ConversationId}");
            LogString($"CurrentStepType = {result.CurrentStepType}");
            LogString($"CurrentFlowId = {result.CurrentFlowId}");
            LogString($"CurrentStepId = {result.CurrentStepId}");
            LogString($"IsError = {result.IsError}");
            LogString($"IsTerminated = {result.IsTerminated}");
            LogString($"IsTransferToHumanAgent = {result.IsTransferToHumanAgent}");
            LogString($"Message = {result.Message}");
            LogString($"UserInputRequired = {result.UserInputRequired}");
            LogString($"UserInputNumeric = {result.UserInputNumeric}");
            LogString($"UserInputMaxLength = {result.UserInputMaxLength}");
            
            if (result.Choices != null)
            {
                List<string> responseList = result.Choices.ToList();
                foreach (string response in responseList)
                {
                    LogString($"Choice = {response}");
                }
            }
            else
            {
                LogString("Choices is null");
            }

            if (result.Variables != null)
            {
                foreach (KeyValuePair<string, object> item in result.Variables)
                {
                    LogString($"Variable - Key: {item.Key}, Value: {item.Value}");
                }
            }
            else            
            {
                LogString("Variables is null");
            }

            if (result.PropertyValues != null)
            {
                foreach (KeyValuePair<string, object> item in result.PropertyValues)
                {
                    LogString($"PropertyValue - Key: {item.Key}, Value: {item.Value}");
                }
            }
            else
            {
                LogString("PropertyValues is null");
            }
        }
    }
}