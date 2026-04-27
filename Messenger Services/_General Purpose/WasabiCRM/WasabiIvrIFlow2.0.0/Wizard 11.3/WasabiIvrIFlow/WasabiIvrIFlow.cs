
//==========================================================================================================

// Copyright � IFM Infomaster. All rights reserved.

//==========================================================================================================

// Author    : Andrea Gheri [AGH]
// Date      : Dec 2024
// Revisions :

//==========================================================================================================

#region Namespaces

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using Ifm.Phones.Blocks.ApplicationManager.Attributes;
using Ifm.Phones.Blocks.ApplicationManager.Tools;
using Ifm.Phones.Blocks.Dialogs.Hosting;
using Ifm.Phones.Blocks.BaseMessengerServices.Dialogs;
#endregion Namespaces

namespace Ifm.Phones.Blocks.Customizations.GeneralPurpose {
    [BlockAttribute(XMLConstants.NODE_MESSENGER_BLOCK,                 // TagName
        "WasabiIvrIFlow.CService",                                     // Type (deve corrispondere alla classe del servizio nel Messenger
        "WasabiIvrIFlow.gif",                                          // Icon
        "WasabiIvrIFlow",                                              // Tooltip
        "WasabiIvrIFlow",                                              // DisplayName
        "WASABI Messenger Services",                                   // Category
        "phonesnet://",                                                // DialogUri
        "WasabiIvrIFlow.WasabiIvrIFlowRes",                            // ResourcePrefix
        ApplicationSubNames.Phone)]                                    // ApplicationSubName
    [PortAttribute("inputport", 1, Direction.input, 1)]
    //[PortAttribute("OK", 2, Direction.output, 2, "NextService")]
    [PortAttribute("Terminated", 3, Direction.output, 3, "NextServiceTerminated")]
    [PortAttribute("Operator", 4, Direction.output, 4, "NextServiceOperator")]
    //[PortAttribute("NoSelection", 5, Direction.output, 5, "NextServiceNoSelection")]    
    [PortAttribute("Error", 6, Direction.output, 6, "NextServiceError")]
    [PropertyAttribute("WS_URL", "", false)]
    [PropertyAttribute("WS_API_ID", "", false)]
    [PropertyAttribute("WS_API_SECRET", "", false)]
    [PropertyAttribute("WS_TIMEOUT", "", false)]
    [PropertyAttribute("FlowId", "", false)]
    [PropertyAttribute("MessagesPath", "", false)]
    [PropertyAttribute("EngineNameTTS", "", false)]
    [PropertyAttribute("UseASRForInput", "1", false)]
    [PropertyAttribute("EngineNameASR", "", false)]
    [PropertyAttribute("ASRConfidenceThreshold", "70", false)]
    [PropertyAttribute("ASRMaxSilence", "5", false)]
    [PropertyAttribute("ASRLookAheadTime", "800", false)]
    [PropertyAttribute("DTMFWaitTime", "5", false)]
    [PropertyAttribute("TTS_DTMF_MenuBegin", ", Digitare:", false)]
    [PropertyAttribute("TTS_DTMF_MenuFor", "per ", false)]
    [PropertyAttribute("TTS_DTMF_NumericMenuBegin", ", Digitare un numero compreso tra ", false)]
    [PropertyAttribute("TTS_DTMF_NumericMenuAnd", " e ", false)]
    [PropertyAttribute("TTS_DTMF_Numeric10", ", premere # per selezione 10 ", false)]
    [PropertyAttribute("TTS_ASR_MenuBegin", ", Scegliere tra: ", false)]
    [PropertyAttribute("TTS_ASR_NumericMenuBegin", ", Dire un numero compreso tra ", false)]
    [PropertyAttribute("TTS_ASR_NumericMenuAnd", " e ", false)]
    [PropertyAttribute("UseSTTForOpenAnswer", "1", false)]
    [PropertyAttribute("EngineNameSTT", "Google Speech API", false)]
    [PropertyAttribute("EngineLanguageSTT", "it-IT", false)]
    [PropertyAttribute("StopOnDigit", "1", false)]
    [PropertyAttribute("StopDelay", "1500", false)]
    [PropertyAttribute("STTMaxSilence", "20", false)]
    [PropertyAttribute("STTConfidenceThreshold", "70", false)]
    [PropertyAttribute("STTEnableBeep", "1", false)]
    [PropertyAttribute("STTStopOnDigitPhrase", "Premi un pulsante per interrompere vocalizzazione", false)]
    [PropertyAttribute("Telefonico", "0", false)]
    [PropertyAttribute("Chat", "1", false)]
    [PropertyAttribute("CHATtimeout", "30", false)]    
    [PropertyAttribute("MaxRetryTel", "3", false)]
    [PropertyAttribute("NumberOfAdditionalParameters", "6", false)]
    [PropertyAttribute("AdditionalParamName_1", "Model", false)]
    [PropertyAttribute("AdditionalParamValue_1", "&amp;quot;telephony&amp;quot;", false)]
    [PropertyAttribute("AdditionalParamName_2", "SpeechEndTimeout", false)]
    [PropertyAttribute("AdditionalParamValue_2", "5", false)]
    [PropertyAttribute("AdditionalParamName_3", "SpeechStartTimeout", false)]
    [PropertyAttribute("AdditionalParamValue_3", "10", false)]
    [PropertyAttribute("AdditionalParamName_4", "StopAtFirstIsFinal", false)]
    [PropertyAttribute("AdditionalParamValue_4", "false", false)]
    [PropertyAttribute("AdditionalParamName_5", "EnableAutomaticPunctuation", false)]
    [PropertyAttribute("AdditionalParamValue_5", "true", false)]
    [PropertyAttribute("AdditionalParamName_6", "ProfanityFilter", false)]
    [PropertyAttribute("AdditionalParamValue_6", "true", false)]


    public class WasabiIvrIFlow : BlockDialog {
        #region Resource Name Constants
        #endregion Resource Name Constants

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageQuestionTTS;
        private SelectEngineName EngineNameTTS;
        private CheckBox UseASRForInput;
        private TabPage tabPageQuestionTTSPhrases;
        private GroupBox groupBoxPhrasesASR;
        private SetParamValue TTS_ASR_NumericMenuAnd;
        private SetParamValue TTS_ASR_NumericMenuBegin;
        private SetParamValue TTS_ASR_MenuBegin;
        private GroupBox groupBoxPhrasesDTMF;
        private SetParamValue TTS_DTMF_NumericMenuAnd;
        private SetParamValue TTS_DTMF_NumericMenuBegin;
        private SetParamValue TTS_DTMF_MenuFor;
        private SetParamValue TTS_DTMF_MenuBegin;
        private GroupBox groupBoxASR;
        private SetParamValue ASRLookAheadTime;
        private SetParamValue ASRMaxSilence;
        private SetParamValue ASRConfidenceThreshold;
        private SelectEngineName EngineNameASR;
        private GroupBox groupBoxDTMF;
        private SetParamValue DTMFWaitTime;
        private SetParamValue TTS_DTMF_Numeric10;
        private TabPage tabPageWasabi;
        private TabPage tabPagAnswerOpen;
        private Label lblInstruction3;
        private CheckBox UseSTTForOpenAnswer;
        private Label lblLanguageCodeSTT;
        private ComboBox cmbEngineName;
        private Label lblEngineNameSTT;
        private SetParamValue txtLanguageCode;
        private Label lblMS;
        private TextBox StopDelay;
        private CheckBox StopOnDigits;
        private SetParamValue STTMaxSilence;
        private SetParamValue STTConfidenceThreshold;
        private CheckBox STTEnableBeep;
        private SetParamValue STTStopOnDigitPhrase;
        private TabPage tabPageTipoChiamata;
        private GroupBox groupBox1;
        private CheckBox CheckChat;
        private CheckBox CheckTelefonico;
        private Label label2;
        private SetParamValue setTimeoutChat;
        private SelectSubFolder MessagesPath;
        private SetParamValue maxRetry;
        private TabPage tabPageCustomParameters;
        private GenericListView lstViewCustomParameters;
        private Label lblInstruction_6;
        private TabPage tabPageAnswer;

        private GenericListViewColumnHeader clmnParamName;
        private SetParamValue WS_TIMEOUT;
        private SetParamValue WS_API_SECRET;
        private SetParamValue WS_SID;
        private SetParamValue WS_URL;
        private SetParamValue FlowId;
        private GenericListViewColumnHeader clmnParamValue;

        public WasabiIvrIFlow()
            : base() {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        public WasabiIvrIFlow(string language, string webService, IBlocksDotNetDialogSink sink)
            :
            base(language, webService, sink) {
            InitializeComponent();

            InitializeControlRules();
        }

        protected override void InitializeControlRules() {
            clmnParamName = new GenericListViewColumnHeader();
            lstViewCustomParameters.Add(clmnParamName);
            clmnParamName.CtrlType = GenericListViewColumnHeader.InputControlTypes.Combobox;
            clmnParamName.ComboStyle = ComboBoxStyle.DropDown;
            clmnParamName.Items = new string[] { "Model", "SpeechEndTimeout", "SpeechStartTimeout", "StopAtFirstIsFinal", "EnableAutomaticPunctuation", "ProfanityFilter" };
            clmnParamName.ComboFillingMode = GenericListViewColumnHeader.ComboFillingModes.ItemList;
            clmnParamName.IsFieldMandatory = true;
            clmnParamName.LabelText = GetResourceString("CustomParameterName", "&Nome:");
            clmnParamName.Text = GetResourceString("ColumnParameterName", "Nome");
            clmnParamName.Width = 120;

            clmnParamValue = new GenericListViewColumnHeader();
            lstViewCustomParameters.Add(clmnParamValue);
            clmnParamValue.CtrlType = GenericListViewColumnHeader.InputControlTypes.TextBox;
            clmnParamValue.Items = null;
            clmnParamValue.IsFieldMandatory = false;
            clmnParamValue.LabelText = GetResourceString("CustomParameterValue", "&Valore:");
            clmnParamValue.Text = GetResourceString("ColumnParameterValue", "Valore");
            clmnParamValue.Width = 120;

            ControlRules newRule = new ControlRules();
            newRule.CounterProperty = "NumberOfAdditionalParameters";
            newRule.SetColumnProperty(0, new ControlProperty("AdditionalParamName"));
            newRule.SetColumnProperty(1, new ControlProperty("AdditionalParamValue"));
            _controlRulesList.Add("AdditionalParameters", newRule);

        }

        protected override bool ValidateControl() {
            return true;
        }

        protected override void Initialized()
        {
            UseASRForInput_CheckedChanged(null, null);
            StopOnDigits_CheckedChanged(null, null);
            UseSTTForOpenAnswer_CheckedChanged(null, null);
            
            CheckTelefonico_CheckedChanged(null, null);
        }
        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageTipoChiamata = new System.Windows.Forms.TabPage();
            this.setTimeoutChat = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CheckChat = new System.Windows.Forms.CheckBox();
            this.CheckTelefonico = new System.Windows.Forms.CheckBox();
            this.tabPageWasabi = new System.Windows.Forms.TabPage();
            this.FlowId = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.WS_TIMEOUT = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.WS_API_SECRET = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.WS_SID = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.WS_URL = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.tabPageQuestionTTS = new System.Windows.Forms.TabPage();
            this.lblInstruction3 = new System.Windows.Forms.Label();
            this.EngineNameTTS = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SelectEngineName();
            this.tabPageQuestionTTSPhrases = new System.Windows.Forms.TabPage();
            this.groupBoxPhrasesASR = new System.Windows.Forms.GroupBox();
            this.TTS_ASR_NumericMenuAnd = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.TTS_ASR_NumericMenuBegin = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.TTS_ASR_MenuBegin = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.groupBoxPhrasesDTMF = new System.Windows.Forms.GroupBox();
            this.TTS_DTMF_Numeric10 = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.TTS_DTMF_NumericMenuAnd = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.TTS_DTMF_NumericMenuBegin = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.TTS_DTMF_MenuFor = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.TTS_DTMF_MenuBegin = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.tabPageAnswer = new System.Windows.Forms.TabPage();
            this.maxRetry = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.groupBoxASR = new System.Windows.Forms.GroupBox();
            this.MessagesPath = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SelectSubFolder();
            this.ASRLookAheadTime = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.ASRMaxSilence = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.ASRConfidenceThreshold = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.EngineNameASR = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SelectEngineName();
            this.groupBoxDTMF = new System.Windows.Forms.GroupBox();
            this.DTMFWaitTime = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.UseASRForInput = new System.Windows.Forms.CheckBox();
            this.tabPagAnswerOpen = new System.Windows.Forms.TabPage();
            this.STTStopOnDigitPhrase = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.STTEnableBeep = new System.Windows.Forms.CheckBox();
            this.STTMaxSilence = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.STTConfidenceThreshold = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.lblMS = new System.Windows.Forms.Label();
            this.StopDelay = new System.Windows.Forms.TextBox();
            this.StopOnDigits = new System.Windows.Forms.CheckBox();
            this.UseSTTForOpenAnswer = new System.Windows.Forms.CheckBox();
            this.lblLanguageCodeSTT = new System.Windows.Forms.Label();
            this.cmbEngineName = new System.Windows.Forms.ComboBox();
            this.lblEngineNameSTT = new System.Windows.Forms.Label();
            this.txtLanguageCode = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SetParamValue();
            this.tabPageCustomParameters = new System.Windows.Forms.TabPage();
            this.lstViewCustomParameters = new Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.GenericListView();
            this.lblInstruction_6 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageTipoChiamata.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPageWasabi.SuspendLayout();
            this.tabPageQuestionTTS.SuspendLayout();
            this.tabPageQuestionTTSPhrases.SuspendLayout();
            this.groupBoxPhrasesASR.SuspendLayout();
            this.groupBoxPhrasesDTMF.SuspendLayout();
            this.tabPageAnswer.SuspendLayout();
            this.groupBoxASR.SuspendLayout();
            this.groupBoxDTMF.SuspendLayout();
            this.tabPagAnswerOpen.SuspendLayout();
            this.tabPageCustomParameters.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageTipoChiamata);
            this.tabControl.Controls.Add(this.tabPageWasabi);
            this.tabControl.Controls.Add(this.tabPageQuestionTTS);
            this.tabControl.Controls.Add(this.tabPageQuestionTTSPhrases);
            this.tabControl.Controls.Add(this.tabPageAnswer);
            this.tabControl.Controls.Add(this.tabPagAnswerOpen);
            this.tabControl.Controls.Add(this.tabPageCustomParameters);
            this.tabControl.Location = new System.Drawing.Point(0, 59);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(432, 431);
            this.tabControl.TabIndex = 3;
            // 
            // tabPageTipoChiamata
            // 
            this.tabPageTipoChiamata.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageTipoChiamata.Controls.Add(this.setTimeoutChat);
            this.tabPageTipoChiamata.Controls.Add(this.label2);
            this.tabPageTipoChiamata.Controls.Add(this.groupBox1);
            this.tabPageTipoChiamata.Location = new System.Drawing.Point(4, 22);
            this.tabPageTipoChiamata.Name = "tabPageTipoChiamata";
            this.tabPageTipoChiamata.Size = new System.Drawing.Size(424, 405);
            this.tabPageTipoChiamata.TabIndex = 6;
            this.tabPageTipoChiamata.Text = "Tipo chiamata";
            // 
            // setTimeoutChat
            // 
            this.setTimeoutChat.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.setTimeoutChat.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.setTimeoutChat.BackColor = System.Drawing.SystemColors.Control;
            this.setTimeoutChat.Caption = "Tempo massimo di attesa chat (sec.):";
            this.setTimeoutChat.CurrentApplicationName = null;
            this.setTimeoutChat.LabelHeight = 16;
            this.setTimeoutChat.Location = new System.Drawing.Point(23, 181);
            this.setTimeoutChat.Margin = new System.Windows.Forms.Padding(0);
            this.setTimeoutChat.Name = "setTimeoutChat";
            this.setTimeoutChat.PasswordChar = '\0';
            this.setTimeoutChat.Size = new System.Drawing.Size(375, 38);
            this.setTimeoutChat.TabIndex = 20;
            this.setTimeoutChat.Tag = "CHATtimeout";
            this.setTimeoutChat.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.setTimeoutChat.Value = "";
            this.setTimeoutChat.VisibleLabel = true;
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(20, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(387, 26);
            this.label2.TabIndex = 18;
            this.label2.Text = "Configurare se il presente Servizio è utlizzato per chiamate di chat o telefonich" +
    "e.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CheckChat);
            this.groupBox1.Controls.Add(this.CheckTelefonico);
            this.groupBox1.Location = new System.Drawing.Point(23, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 74);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tipo servizio:";
            // 
            // CheckChat
            // 
            this.CheckChat.AutoSize = true;
            this.CheckChat.Location = new System.Drawing.Point(24, 26);
            this.CheckChat.Name = "CheckChat";
            this.CheckChat.Size = new System.Drawing.Size(48, 17);
            this.CheckChat.TabIndex = 2;
            this.CheckChat.Tag = "Chat";
            this.CheckChat.Text = "Chat";
            this.CheckChat.UseVisualStyleBackColor = true;
            this.CheckChat.CheckedChanged += new System.EventHandler(this.CheckChat_CheckedChanged);
            // 
            // CheckTelefonico
            // 
            this.CheckTelefonico.AutoSize = true;
            this.CheckTelefonico.Checked = true;
            this.CheckTelefonico.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckTelefonico.Location = new System.Drawing.Point(24, 49);
            this.CheckTelefonico.Name = "CheckTelefonico";
            this.CheckTelefonico.Size = new System.Drawing.Size(76, 17);
            this.CheckTelefonico.TabIndex = 1;
            this.CheckTelefonico.Tag = "Telefonico";
            this.CheckTelefonico.Text = "Telefonico";
            this.CheckTelefonico.UseVisualStyleBackColor = true;
            this.CheckTelefonico.CheckedChanged += new System.EventHandler(this.CheckTelefonico_CheckedChanged);
            // 
            // tabPageWasabi
            // 
            this.tabPageWasabi.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageWasabi.Controls.Add(this.FlowId);
            this.tabPageWasabi.Controls.Add(this.WS_TIMEOUT);
            this.tabPageWasabi.Controls.Add(this.WS_API_SECRET);
            this.tabPageWasabi.Controls.Add(this.WS_SID);
            this.tabPageWasabi.Controls.Add(this.WS_URL);
            this.tabPageWasabi.Location = new System.Drawing.Point(4, 22);
            this.tabPageWasabi.Name = "tabPageWasabi";
            this.tabPageWasabi.Size = new System.Drawing.Size(424, 405);
            this.tabPageWasabi.TabIndex = 4;
            this.tabPageWasabi.Text = "Wasabi";
            // 
            // FlowId
            // 
            this.FlowId.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.FlowId.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.FlowId.BackColor = System.Drawing.SystemColors.Control;
            this.FlowId.Caption = "FlowId:";
            this.FlowId.CurrentApplicationName = null;
            this.FlowId.LabelHeight = 16;
            this.FlowId.Location = new System.Drawing.Point(12, 250);
            this.FlowId.Margin = new System.Windows.Forms.Padding(0);
            this.FlowId.Name = "FlowId";
            this.FlowId.PasswordChar = '\0';
            this.FlowId.Size = new System.Drawing.Size(401, 38);
            this.FlowId.TabIndex = 8;
            this.FlowId.Tag = "FlowId";
            this.FlowId.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.FlowId.Value = "";
            this.FlowId.VisibleLabel = true;
            // 
            // WS_TIMEOUT
            // 
            this.WS_TIMEOUT.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_TIMEOUT.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_TIMEOUT.BackColor = System.Drawing.SystemColors.Control;
            this.WS_TIMEOUT.Caption = "Wasabi Web Service &Timeout (sec):";
            this.WS_TIMEOUT.CurrentApplicationName = null;
            this.WS_TIMEOUT.LabelHeight = 16;
            this.WS_TIMEOUT.Location = new System.Drawing.Point(12, 185);
            this.WS_TIMEOUT.Margin = new System.Windows.Forms.Padding(0);
            this.WS_TIMEOUT.Name = "WS_TIMEOUT";
            this.WS_TIMEOUT.PasswordChar = '\0';
            this.WS_TIMEOUT.Size = new System.Drawing.Size(401, 38);
            this.WS_TIMEOUT.TabIndex = 7;
            this.WS_TIMEOUT.Tag = "WS_TIMEOUT";
            this.WS_TIMEOUT.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.WS_TIMEOUT.Value = "";
            this.WS_TIMEOUT.VisibleLabel = true;
            // 
            // WS_API_SECRET
            // 
            this.WS_API_SECRET.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_API_SECRET.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_API_SECRET.BackColor = System.Drawing.SystemColors.Control;
            this.WS_API_SECRET.Caption = "App Secret:";
            this.WS_API_SECRET.CurrentApplicationName = null;
            this.WS_API_SECRET.LabelHeight = 16;
            this.WS_API_SECRET.Location = new System.Drawing.Point(12, 123);
            this.WS_API_SECRET.Margin = new System.Windows.Forms.Padding(0);
            this.WS_API_SECRET.Name = "WS_API_SECRET";
            this.WS_API_SECRET.PasswordChar = '\0';
            this.WS_API_SECRET.Size = new System.Drawing.Size(401, 38);
            this.WS_API_SECRET.TabIndex = 6;
            this.WS_API_SECRET.Tag = "WS_API_SECRET";
            this.WS_API_SECRET.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.WS_API_SECRET.Value = "";
            this.WS_API_SECRET.VisibleLabel = true;
            // 
            // WS_SID
            // 
            this.WS_SID.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_SID.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_SID.BackColor = System.Drawing.SystemColors.Control;
            this.WS_SID.Caption = "App Id:";
            this.WS_SID.CurrentApplicationName = null;
            this.WS_SID.LabelHeight = 16;
            this.WS_SID.Location = new System.Drawing.Point(12, 65);
            this.WS_SID.Margin = new System.Windows.Forms.Padding(0);
            this.WS_SID.Name = "WS_SID";
            this.WS_SID.PasswordChar = '\0';
            this.WS_SID.Size = new System.Drawing.Size(401, 38);
            this.WS_SID.TabIndex = 5;
            this.WS_SID.Tag = "WS_API_ID";
            this.WS_SID.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.WS_SID.Value = "";
            this.WS_SID.VisibleLabel = true;
            // 
            // WS_URL
            // 
            this.WS_URL.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_URL.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.WS_URL.BackColor = System.Drawing.SystemColors.Control;
            this.WS_URL.Caption = "Wasabi Base URL:";
            this.WS_URL.CurrentApplicationName = null;
            this.WS_URL.LabelHeight = 16;
            this.WS_URL.Location = new System.Drawing.Point(12, 11);
            this.WS_URL.Margin = new System.Windows.Forms.Padding(0);
            this.WS_URL.Name = "WS_URL";
            this.WS_URL.PasswordChar = '\0';
            this.WS_URL.Size = new System.Drawing.Size(401, 38);
            this.WS_URL.TabIndex = 4;
            this.WS_URL.Tag = "WS_URL";
            this.WS_URL.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.WS_URL.Value = "";
            this.WS_URL.VisibleLabel = true;
            // 
            // tabPageQuestionTTS
            // 
            this.tabPageQuestionTTS.Controls.Add(this.lblInstruction3);
            this.tabPageQuestionTTS.Controls.Add(this.EngineNameTTS);
            this.tabPageQuestionTTS.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuestionTTS.Name = "tabPageQuestionTTS";
            this.tabPageQuestionTTS.Size = new System.Drawing.Size(424, 405);
            this.tabPageQuestionTTS.TabIndex = 0;
            this.tabPageQuestionTTS.Text = "Domanda TTS:";
            // 
            // lblInstruction3
            // 
            this.lblInstruction3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblInstruction3.Location = new System.Drawing.Point(9, 21);
            this.lblInstruction3.Name = "lblInstruction3";
            this.lblInstruction3.Size = new System.Drawing.Size(404, 40);
            this.lblInstruction3.TabIndex = 9;
            this.lblInstruction3.Text = "Indicare i parametri per il TTS ";
            // 
            // EngineNameTTS
            // 
            this.EngineNameTTS.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.EngineNameTTS.BackColor = System.Drawing.SystemColors.Control;
            this.EngineNameTTS.Caption = "Nome motore TTS:";
            this.EngineNameTTS.CurrentApplicationName = null;
            this.EngineNameTTS.Enable = true;
            this.EngineNameTTS.FileType = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SelectEngineName.EngineTypes.TTS;
            this.EngineNameTTS.Location = new System.Drawing.Point(8, 64);
            this.EngineNameTTS.Name = "EngineNameTTS";
            this.EngineNameTTS.Padding = new System.Windows.Forms.Padding(2);
            this.EngineNameTTS.Size = new System.Drawing.Size(400, 42);
            this.EngineNameTTS.TabIndex = 8;
            this.EngineNameTTS.Tag = "EngineNameTTS";
            this.EngineNameTTS.Value = "";
            // 
            // tabPageQuestionTTSPhrases
            // 
            this.tabPageQuestionTTSPhrases.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageQuestionTTSPhrases.Controls.Add(this.groupBoxPhrasesASR);
            this.tabPageQuestionTTSPhrases.Controls.Add(this.groupBoxPhrasesDTMF);
            this.tabPageQuestionTTSPhrases.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuestionTTSPhrases.Name = "tabPageQuestionTTSPhrases";
            this.tabPageQuestionTTSPhrases.Size = new System.Drawing.Size(424, 405);
            this.tabPageQuestionTTSPhrases.TabIndex = 3;
            this.tabPageQuestionTTSPhrases.Text = "Frasi";
            // 
            // groupBoxPhrasesASR
            // 
            this.groupBoxPhrasesASR.Controls.Add(this.TTS_ASR_NumericMenuAnd);
            this.groupBoxPhrasesASR.Controls.Add(this.TTS_ASR_NumericMenuBegin);
            this.groupBoxPhrasesASR.Controls.Add(this.TTS_ASR_MenuBegin);
            this.groupBoxPhrasesASR.Location = new System.Drawing.Point(8, 244);
            this.groupBoxPhrasesASR.Name = "groupBoxPhrasesASR";
            this.groupBoxPhrasesASR.Size = new System.Drawing.Size(404, 140);
            this.groupBoxPhrasesASR.TabIndex = 3;
            this.groupBoxPhrasesASR.TabStop = false;
            this.groupBoxPhrasesASR.Text = "ASR";
            // 
            // TTS_ASR_NumericMenuAnd
            // 
            this.TTS_ASR_NumericMenuAnd.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_ASR_NumericMenuAnd.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_ASR_NumericMenuAnd.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_ASR_NumericMenuAnd.Caption = "Caso numerico: valore finale";
            this.TTS_ASR_NumericMenuAnd.CurrentApplicationName = null;
            this.TTS_ASR_NumericMenuAnd.LabelHeight = 16;
            this.TTS_ASR_NumericMenuAnd.Location = new System.Drawing.Point(8, 95);
            this.TTS_ASR_NumericMenuAnd.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_ASR_NumericMenuAnd.Name = "TTS_ASR_NumericMenuAnd";
            this.TTS_ASR_NumericMenuAnd.PasswordChar = '\0';
            this.TTS_ASR_NumericMenuAnd.Size = new System.Drawing.Size(392, 38);
            this.TTS_ASR_NumericMenuAnd.TabIndex = 8;
            this.TTS_ASR_NumericMenuAnd.Tag = "TTS_ASR_NumericMenuAnd";
            this.TTS_ASR_NumericMenuAnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_ASR_NumericMenuAnd.Value = " e ";
            this.TTS_ASR_NumericMenuAnd.VisibleLabel = true;
            // 
            // TTS_ASR_NumericMenuBegin
            // 
            this.TTS_ASR_NumericMenuBegin.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_ASR_NumericMenuBegin.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_ASR_NumericMenuBegin.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_ASR_NumericMenuBegin.Caption = "Caso numerico: valore iniziale";
            this.TTS_ASR_NumericMenuBegin.CurrentApplicationName = null;
            this.TTS_ASR_NumericMenuBegin.LabelHeight = 16;
            this.TTS_ASR_NumericMenuBegin.Location = new System.Drawing.Point(8, 53);
            this.TTS_ASR_NumericMenuBegin.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_ASR_NumericMenuBegin.Name = "TTS_ASR_NumericMenuBegin";
            this.TTS_ASR_NumericMenuBegin.PasswordChar = '\0';
            this.TTS_ASR_NumericMenuBegin.Size = new System.Drawing.Size(392, 38);
            this.TTS_ASR_NumericMenuBegin.TabIndex = 7;
            this.TTS_ASR_NumericMenuBegin.Tag = "TTS_ASR_NumericMenuBegin";
            this.TTS_ASR_NumericMenuBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_ASR_NumericMenuBegin.Value = ", Dire un numero compreso tra ";
            this.TTS_ASR_NumericMenuBegin.VisibleLabel = true;
            // 
            // TTS_ASR_MenuBegin
            // 
            this.TTS_ASR_MenuBegin.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_ASR_MenuBegin.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_ASR_MenuBegin.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_ASR_MenuBegin.Caption = "Elenco scelte: inizio frase menu";
            this.TTS_ASR_MenuBegin.CurrentApplicationName = null;
            this.TTS_ASR_MenuBegin.LabelHeight = 16;
            this.TTS_ASR_MenuBegin.Location = new System.Drawing.Point(8, 16);
            this.TTS_ASR_MenuBegin.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_ASR_MenuBegin.Name = "TTS_ASR_MenuBegin";
            this.TTS_ASR_MenuBegin.PasswordChar = '\0';
            this.TTS_ASR_MenuBegin.Size = new System.Drawing.Size(392, 38);
            this.TTS_ASR_MenuBegin.TabIndex = 6;
            this.TTS_ASR_MenuBegin.Tag = "TTS_ASR_MenuBegin";
            this.TTS_ASR_MenuBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_ASR_MenuBegin.Value = ", Scegliere tra: ";
            this.TTS_ASR_MenuBegin.VisibleLabel = true;
            // 
            // groupBoxPhrasesDTMF
            // 
            this.groupBoxPhrasesDTMF.Controls.Add(this.TTS_DTMF_Numeric10);
            this.groupBoxPhrasesDTMF.Controls.Add(this.TTS_DTMF_NumericMenuAnd);
            this.groupBoxPhrasesDTMF.Controls.Add(this.TTS_DTMF_NumericMenuBegin);
            this.groupBoxPhrasesDTMF.Controls.Add(this.TTS_DTMF_MenuFor);
            this.groupBoxPhrasesDTMF.Controls.Add(this.TTS_DTMF_MenuBegin);
            this.groupBoxPhrasesDTMF.Location = new System.Drawing.Point(8, 20);
            this.groupBoxPhrasesDTMF.Name = "groupBoxPhrasesDTMF";
            this.groupBoxPhrasesDTMF.Size = new System.Drawing.Size(404, 218);
            this.groupBoxPhrasesDTMF.TabIndex = 2;
            this.groupBoxPhrasesDTMF.TabStop = false;
            this.groupBoxPhrasesDTMF.Text = "DTMF";
            // 
            // TTS_DTMF_Numeric10
            // 
            this.TTS_DTMF_Numeric10.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_Numeric10.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_Numeric10.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_DTMF_Numeric10.Caption = "Caso numerico: scelta per 10 (es \'digitare # per 10\')";
            this.TTS_DTMF_Numeric10.CurrentApplicationName = null;
            this.TTS_DTMF_Numeric10.LabelHeight = 16;
            this.TTS_DTMF_Numeric10.Location = new System.Drawing.Point(3, 174);
            this.TTS_DTMF_Numeric10.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_DTMF_Numeric10.Name = "TTS_DTMF_Numeric10";
            this.TTS_DTMF_Numeric10.PasswordChar = '\0';
            this.TTS_DTMF_Numeric10.Size = new System.Drawing.Size(392, 38);
            this.TTS_DTMF_Numeric10.TabIndex = 10;
            this.TTS_DTMF_Numeric10.Tag = "TTS_DTMF_Numeric10";
            this.TTS_DTMF_Numeric10.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_DTMF_Numeric10.Value = ", premere # per selezione 10 ";
            this.TTS_DTMF_Numeric10.VisibleLabel = true;
            // 
            // TTS_DTMF_NumericMenuAnd
            // 
            this.TTS_DTMF_NumericMenuAnd.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_NumericMenuAnd.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_NumericMenuAnd.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_DTMF_NumericMenuAnd.Caption = "Caso numerico: valore finale";
            this.TTS_DTMF_NumericMenuAnd.CurrentApplicationName = null;
            this.TTS_DTMF_NumericMenuAnd.LabelHeight = 16;
            this.TTS_DTMF_NumericMenuAnd.Location = new System.Drawing.Point(3, 136);
            this.TTS_DTMF_NumericMenuAnd.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_DTMF_NumericMenuAnd.Name = "TTS_DTMF_NumericMenuAnd";
            this.TTS_DTMF_NumericMenuAnd.PasswordChar = '\0';
            this.TTS_DTMF_NumericMenuAnd.Size = new System.Drawing.Size(392, 38);
            this.TTS_DTMF_NumericMenuAnd.TabIndex = 5;
            this.TTS_DTMF_NumericMenuAnd.Tag = "TTS_DTMF_NumericMenuAnd";
            this.TTS_DTMF_NumericMenuAnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_DTMF_NumericMenuAnd.Value = " e ";
            this.TTS_DTMF_NumericMenuAnd.VisibleLabel = true;
            // 
            // TTS_DTMF_NumericMenuBegin
            // 
            this.TTS_DTMF_NumericMenuBegin.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_NumericMenuBegin.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_NumericMenuBegin.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_DTMF_NumericMenuBegin.Caption = "Caso numerico: valore iniziale";
            this.TTS_DTMF_NumericMenuBegin.CurrentApplicationName = null;
            this.TTS_DTMF_NumericMenuBegin.LabelHeight = 16;
            this.TTS_DTMF_NumericMenuBegin.Location = new System.Drawing.Point(3, 96);
            this.TTS_DTMF_NumericMenuBegin.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_DTMF_NumericMenuBegin.Name = "TTS_DTMF_NumericMenuBegin";
            this.TTS_DTMF_NumericMenuBegin.PasswordChar = '\0';
            this.TTS_DTMF_NumericMenuBegin.Size = new System.Drawing.Size(392, 38);
            this.TTS_DTMF_NumericMenuBegin.TabIndex = 4;
            this.TTS_DTMF_NumericMenuBegin.Tag = "TTS_DTMF_NumericMenuBegin";
            this.TTS_DTMF_NumericMenuBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_DTMF_NumericMenuBegin.Value = ", Digitare un numero compreso tra ";
            this.TTS_DTMF_NumericMenuBegin.VisibleLabel = true;
            // 
            // TTS_DTMF_MenuFor
            // 
            this.TTS_DTMF_MenuFor.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_MenuFor.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_MenuFor.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_DTMF_MenuFor.Caption = "Elenco scelte: per";
            this.TTS_DTMF_MenuFor.CurrentApplicationName = null;
            this.TTS_DTMF_MenuFor.LabelHeight = 16;
            this.TTS_DTMF_MenuFor.Location = new System.Drawing.Point(3, 55);
            this.TTS_DTMF_MenuFor.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_DTMF_MenuFor.Name = "TTS_DTMF_MenuFor";
            this.TTS_DTMF_MenuFor.PasswordChar = '\0';
            this.TTS_DTMF_MenuFor.Size = new System.Drawing.Size(392, 38);
            this.TTS_DTMF_MenuFor.TabIndex = 3;
            this.TTS_DTMF_MenuFor.Tag = "TTS_DTMF_MenuFor";
            this.TTS_DTMF_MenuFor.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_DTMF_MenuFor.Value = " per ";
            this.TTS_DTMF_MenuFor.VisibleLabel = true;
            // 
            // TTS_DTMF_MenuBegin
            // 
            this.TTS_DTMF_MenuBegin.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_MenuBegin.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.TTS_DTMF_MenuBegin.BackColor = System.Drawing.SystemColors.Control;
            this.TTS_DTMF_MenuBegin.Caption = "Elenco scelte: inizio frase menu";
            this.TTS_DTMF_MenuBegin.CurrentApplicationName = null;
            this.TTS_DTMF_MenuBegin.LabelHeight = 16;
            this.TTS_DTMF_MenuBegin.Location = new System.Drawing.Point(3, 16);
            this.TTS_DTMF_MenuBegin.Margin = new System.Windows.Forms.Padding(0);
            this.TTS_DTMF_MenuBegin.Name = "TTS_DTMF_MenuBegin";
            this.TTS_DTMF_MenuBegin.PasswordChar = '\0';
            this.TTS_DTMF_MenuBegin.Size = new System.Drawing.Size(392, 38);
            this.TTS_DTMF_MenuBegin.TabIndex = 2;
            this.TTS_DTMF_MenuBegin.Tag = "TTS_DTMF_MenuBegin";
            this.TTS_DTMF_MenuBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TTS_DTMF_MenuBegin.Value = ", Digitare:";
            this.TTS_DTMF_MenuBegin.VisibleLabel = true;
            // 
            // tabPageAnswer
            // 
            this.tabPageAnswer.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageAnswer.Controls.Add(this.maxRetry);
            this.tabPageAnswer.Controls.Add(this.groupBoxASR);
            this.tabPageAnswer.Controls.Add(this.groupBoxDTMF);
            this.tabPageAnswer.Controls.Add(this.UseASRForInput);
            this.tabPageAnswer.Location = new System.Drawing.Point(4, 22);
            this.tabPageAnswer.Name = "tabPageAnswer";
            this.tabPageAnswer.Size = new System.Drawing.Size(424, 405);
            this.tabPageAnswer.TabIndex = 1;
            this.tabPageAnswer.Text = "Risposta";
            // 
            // maxRetry
            // 
            this.maxRetry.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.maxRetry.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.maxRetry.BackColor = System.Drawing.SystemColors.Control;
            this.maxRetry.Caption = "Numero massimo tentativi input utente:";
            this.maxRetry.CurrentApplicationName = null;
            this.maxRetry.LabelHeight = 16;
            this.maxRetry.Location = new System.Drawing.Point(13, 10);
            this.maxRetry.Margin = new System.Windows.Forms.Padding(0);
            this.maxRetry.Name = "maxRetry";
            this.maxRetry.PasswordChar = '\0';
            this.maxRetry.Size = new System.Drawing.Size(229, 38);
            this.maxRetry.TabIndex = 18;
            this.maxRetry.Tag = "MaxRetryTel";
            this.maxRetry.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.maxRetry.Value = "3";
            this.maxRetry.VisibleLabel = true;
            // 
            // groupBoxASR
            // 
            this.groupBoxASR.Controls.Add(this.MessagesPath);
            this.groupBoxASR.Controls.Add(this.ASRLookAheadTime);
            this.groupBoxASR.Controls.Add(this.ASRMaxSilence);
            this.groupBoxASR.Controls.Add(this.ASRConfidenceThreshold);
            this.groupBoxASR.Controls.Add(this.EngineNameASR);
            this.groupBoxASR.Location = new System.Drawing.Point(8, 83);
            this.groupBoxASR.Name = "groupBoxASR";
            this.groupBoxASR.Size = new System.Drawing.Size(412, 246);
            this.groupBoxASR.TabIndex = 9;
            this.groupBoxASR.TabStop = false;
            this.groupBoxASR.Text = "ASR";
            // 
            // MessagesPath
            // 
            this.MessagesPath.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.MessagesPath.BackColor = System.Drawing.SystemColors.Control;
            this.MessagesPath.Caption = "Percorso messaggio Silence.wav";
            this.MessagesPath.CurrentApplicationName = null;
            this.MessagesPath.Enable = true;
            this.MessagesPath.Location = new System.Drawing.Point(4, 203);
            this.MessagesPath.MessageFolderLimited = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.BrowseLimitType.Messages;
            this.MessagesPath.Name = "MessagesPath";
            this.MessagesPath.Padding = new System.Windows.Forms.Padding(2);
            this.MessagesPath.Size = new System.Drawing.Size(396, 40);
            this.MessagesPath.TabIndex = 16;
            this.MessagesPath.Tag = "MessagesPath";
            this.MessagesPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.MessagesPath.Value = "";
            // 
            // ASRLookAheadTime
            // 
            this.ASRLookAheadTime.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.ASRLookAheadTime.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.ASRLookAheadTime.BackColor = System.Drawing.SystemColors.Control;
            this.ASRLookAheadTime.Caption = "Tempo Look ahead (msec):";
            this.ASRLookAheadTime.CurrentApplicationName = null;
            this.ASRLookAheadTime.LabelHeight = 16;
            this.ASRLookAheadTime.Location = new System.Drawing.Point(4, 109);
            this.ASRLookAheadTime.Margin = new System.Windows.Forms.Padding(0);
            this.ASRLookAheadTime.Name = "ASRLookAheadTime";
            this.ASRLookAheadTime.PasswordChar = '\0';
            this.ASRLookAheadTime.Size = new System.Drawing.Size(396, 38);
            this.ASRLookAheadTime.TabIndex = 11;
            this.ASRLookAheadTime.Tag = "ASRLookAheadTime";
            this.ASRLookAheadTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.ASRLookAheadTime.Value = "800";
            this.ASRLookAheadTime.VisibleLabel = true;
            // 
            // ASRMaxSilence
            // 
            this.ASRMaxSilence.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.ASRMaxSilence.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.ASRMaxSilence.BackColor = System.Drawing.SystemColors.Control;
            this.ASRMaxSilence.Caption = "Tempo massimo di attesa (secondi):";
            this.ASRMaxSilence.CurrentApplicationName = null;
            this.ASRMaxSilence.LabelHeight = 16;
            this.ASRMaxSilence.Location = new System.Drawing.Point(4, 63);
            this.ASRMaxSilence.Margin = new System.Windows.Forms.Padding(0);
            this.ASRMaxSilence.Name = "ASRMaxSilence";
            this.ASRMaxSilence.PasswordChar = '\0';
            this.ASRMaxSilence.Size = new System.Drawing.Size(396, 38);
            this.ASRMaxSilence.TabIndex = 10;
            this.ASRMaxSilence.Tag = "ASRMaxSilence";
            this.ASRMaxSilence.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.ASRMaxSilence.Value = "5";
            this.ASRMaxSilence.VisibleLabel = true;
            // 
            // ASRConfidenceThreshold
            // 
            this.ASRConfidenceThreshold.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.ASRConfidenceThreshold.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.ASRConfidenceThreshold.BackColor = System.Drawing.SystemColors.Control;
            this.ASRConfidenceThreshold.Caption = "Soglia confidenza (0-100):";
            this.ASRConfidenceThreshold.CurrentApplicationName = null;
            this.ASRConfidenceThreshold.LabelHeight = 16;
            this.ASRConfidenceThreshold.Location = new System.Drawing.Point(5, 158);
            this.ASRConfidenceThreshold.Margin = new System.Windows.Forms.Padding(0);
            this.ASRConfidenceThreshold.Name = "ASRConfidenceThreshold";
            this.ASRConfidenceThreshold.PasswordChar = '\0';
            this.ASRConfidenceThreshold.Size = new System.Drawing.Size(396, 38);
            this.ASRConfidenceThreshold.TabIndex = 9;
            this.ASRConfidenceThreshold.Tag = "ASRConfidenceThreshold";
            this.ASRConfidenceThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.ASRConfidenceThreshold.Value = "70";
            this.ASRConfidenceThreshold.VisibleLabel = true;
            // 
            // EngineNameASR
            // 
            this.EngineNameASR.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.EngineNameASR.BackColor = System.Drawing.SystemColors.Control;
            this.EngineNameASR.Caption = "Nome motore ASR:";
            this.EngineNameASR.CurrentApplicationName = null;
            this.EngineNameASR.Enable = true;
            this.EngineNameASR.FileType = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.SelectEngineName.EngineTypes.ASR;
            this.EngineNameASR.Location = new System.Drawing.Point(5, 19);
            this.EngineNameASR.Name = "EngineNameASR";
            this.EngineNameASR.Padding = new System.Windows.Forms.Padding(2);
            this.EngineNameASR.Size = new System.Drawing.Size(396, 41);
            this.EngineNameASR.TabIndex = 7;
            this.EngineNameASR.Tag = "EngineNameASR";
            this.EngineNameASR.Value = "";
            // 
            // groupBoxDTMF
            // 
            this.groupBoxDTMF.Controls.Add(this.DTMFWaitTime);
            this.groupBoxDTMF.Location = new System.Drawing.Point(10, 335);
            this.groupBoxDTMF.Name = "groupBoxDTMF";
            this.groupBoxDTMF.Size = new System.Drawing.Size(410, 66);
            this.groupBoxDTMF.TabIndex = 8;
            this.groupBoxDTMF.TabStop = false;
            this.groupBoxDTMF.Text = "DTMF";
            // 
            // DTMFWaitTime
            // 
            this.DTMFWaitTime.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.DTMFWaitTime.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.DTMFWaitTime.BackColor = System.Drawing.SystemColors.Control;
            this.DTMFWaitTime.Caption = "Tempo massimo di attesa (secondi):";
            this.DTMFWaitTime.CurrentApplicationName = null;
            this.DTMFWaitTime.LabelHeight = 16;
            this.DTMFWaitTime.Location = new System.Drawing.Point(5, 16);
            this.DTMFWaitTime.Margin = new System.Windows.Forms.Padding(0);
            this.DTMFWaitTime.Name = "DTMFWaitTime";
            this.DTMFWaitTime.PasswordChar = '\0';
            this.DTMFWaitTime.Size = new System.Drawing.Size(405, 38);
            this.DTMFWaitTime.TabIndex = 6;
            this.DTMFWaitTime.Tag = "DTMFWaitTime";
            this.DTMFWaitTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.DTMFWaitTime.Value = "5";
            this.DTMFWaitTime.VisibleLabel = true;
            // 
            // UseASRForInput
            // 
            this.UseASRForInput.AutoSize = true;
            this.UseASRForInput.Checked = true;
            this.UseASRForInput.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseASRForInput.Location = new System.Drawing.Point(13, 57);
            this.UseASRForInput.Name = "UseASRForInput";
            this.UseASRForInput.Size = new System.Drawing.Size(223, 17);
            this.UseASRForInput.TabIndex = 1;
            this.UseASRForInput.Tag = "UseASRForInput";
            this.UseASRForInput.Text = "Usa ASR (se non selezionato usa DTMF):";
            this.UseASRForInput.UseVisualStyleBackColor = true;
            this.UseASRForInput.CheckedChanged += new System.EventHandler(this.UseASRForInput_CheckedChanged);
            // 
            // tabPagAnswerOpen
            // 
            this.tabPagAnswerOpen.BackColor = System.Drawing.SystemColors.Control;
            this.tabPagAnswerOpen.Controls.Add(this.STTStopOnDigitPhrase);
            this.tabPagAnswerOpen.Controls.Add(this.STTEnableBeep);
            this.tabPagAnswerOpen.Controls.Add(this.STTMaxSilence);
            this.tabPagAnswerOpen.Controls.Add(this.STTConfidenceThreshold);
            this.tabPagAnswerOpen.Controls.Add(this.lblMS);
            this.tabPagAnswerOpen.Controls.Add(this.StopDelay);
            this.tabPagAnswerOpen.Controls.Add(this.StopOnDigits);
            this.tabPagAnswerOpen.Controls.Add(this.UseSTTForOpenAnswer);
            this.tabPagAnswerOpen.Controls.Add(this.lblLanguageCodeSTT);
            this.tabPagAnswerOpen.Controls.Add(this.cmbEngineName);
            this.tabPagAnswerOpen.Controls.Add(this.lblEngineNameSTT);
            this.tabPagAnswerOpen.Controls.Add(this.txtLanguageCode);
            this.tabPagAnswerOpen.Location = new System.Drawing.Point(4, 22);
            this.tabPagAnswerOpen.Name = "tabPagAnswerOpen";
            this.tabPagAnswerOpen.Size = new System.Drawing.Size(424, 405);
            this.tabPagAnswerOpen.TabIndex = 5;
            this.tabPagAnswerOpen.Text = "Risposta aperta";
            // 
            // STTStopOnDigitPhrase
            // 
            this.STTStopOnDigitPhrase.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.STTStopOnDigitPhrase.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.STTStopOnDigitPhrase.BackColor = System.Drawing.SystemColors.Control;
            this.STTStopOnDigitPhrase.Caption = "Frase invito a premere digit per interrompere vocalizzazione";
            this.STTStopOnDigitPhrase.CurrentApplicationName = null;
            this.STTStopOnDigitPhrase.LabelHeight = 16;
            this.STTStopOnDigitPhrase.Location = new System.Drawing.Point(14, 243);
            this.STTStopOnDigitPhrase.Margin = new System.Windows.Forms.Padding(0);
            this.STTStopOnDigitPhrase.Name = "STTStopOnDigitPhrase";
            this.STTStopOnDigitPhrase.PasswordChar = '\0';
            this.STTStopOnDigitPhrase.Size = new System.Drawing.Size(392, 38);
            this.STTStopOnDigitPhrase.TabIndex = 32;
            this.STTStopOnDigitPhrase.Tag = "STTStopOnDigitPhrase";
            this.STTStopOnDigitPhrase.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.STTStopOnDigitPhrase.Value = "Premi un pulsante per interrompere vocalizzazione";
            this.STTStopOnDigitPhrase.VisibleLabel = true;
            // 
            // STTEnableBeep
            // 
            this.STTEnableBeep.AutoSize = true;
            this.STTEnableBeep.Checked = true;
            this.STTEnableBeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.STTEnableBeep.Location = new System.Drawing.Point(14, 155);
            this.STTEnableBeep.Name = "STTEnableBeep";
            this.STTEnableBeep.Size = new System.Drawing.Size(250, 17);
            this.STTEnableBeep.TabIndex = 31;
            this.STTEnableBeep.Tag = "STTEnableBeep";
            this.STTEnableBeep.Text = "Abilita notifica del beep ad inizio riconoscimento";
            this.STTEnableBeep.UseVisualStyleBackColor = true;
            // 
            // STTMaxSilence
            // 
            this.STTMaxSilence.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.STTMaxSilence.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.STTMaxSilence.BackColor = System.Drawing.SystemColors.Control;
            this.STTMaxSilence.Caption = "Tempo massimo di attesa (secondi):";
            this.STTMaxSilence.CurrentApplicationName = null;
            this.STTMaxSilence.LabelHeight = 16;
            this.STTMaxSilence.Location = new System.Drawing.Point(14, 306);
            this.STTMaxSilence.Margin = new System.Windows.Forms.Padding(0);
            this.STTMaxSilence.Name = "STTMaxSilence";
            this.STTMaxSilence.PasswordChar = '\0';
            this.STTMaxSilence.Size = new System.Drawing.Size(396, 38);
            this.STTMaxSilence.TabIndex = 30;
            this.STTMaxSilence.Tag = "STTMaxSilence";
            this.STTMaxSilence.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.STTMaxSilence.Value = "20";
            this.STTMaxSilence.VisibleLabel = true;
            // 
            // STTConfidenceThreshold
            // 
            this.STTConfidenceThreshold.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.STTConfidenceThreshold.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.STTConfidenceThreshold.BackColor = System.Drawing.SystemColors.Control;
            this.STTConfidenceThreshold.Caption = "Soglia confidenza (0-100):";
            this.STTConfidenceThreshold.CurrentApplicationName = null;
            this.STTConfidenceThreshold.LabelHeight = 16;
            this.STTConfidenceThreshold.Location = new System.Drawing.Point(14, 354);
            this.STTConfidenceThreshold.Margin = new System.Windows.Forms.Padding(0);
            this.STTConfidenceThreshold.Name = "STTConfidenceThreshold";
            this.STTConfidenceThreshold.PasswordChar = '\0';
            this.STTConfidenceThreshold.Size = new System.Drawing.Size(396, 38);
            this.STTConfidenceThreshold.TabIndex = 29;
            this.STTConfidenceThreshold.Tag = "STTConfidenceThreshold";
            this.STTConfidenceThreshold.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.STTConfidenceThreshold.Value = "70";
            this.STTConfidenceThreshold.VisibleLabel = true;
            // 
            // lblMS
            // 
            this.lblMS.AutoSize = true;
            this.lblMS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMS.Location = new System.Drawing.Point(330, 197);
            this.lblMS.Name = "lblMS";
            this.lblMS.Size = new System.Drawing.Size(20, 13);
            this.lblMS.TabIndex = 28;
            this.lblMS.Text = "ms";
            // 
            // StopDelay
            // 
            this.StopDelay.Enabled = false;
            this.StopDelay.Location = new System.Drawing.Point(282, 195);
            this.StopDelay.Name = "StopDelay";
            this.StopDelay.Size = new System.Drawing.Size(42, 20);
            this.StopDelay.TabIndex = 27;
            this.StopDelay.Tag = "StopDelay";
            this.StopDelay.Text = "1500";
            this.StopDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // StopOnDigits
            // 
            this.StopOnDigits.Checked = true;
            this.StopOnDigits.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StopOnDigits.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.StopOnDigits.Location = new System.Drawing.Point(14, 193);
            this.StopOnDigits.Name = "StopOnDigits";
            this.StopOnDigits.Size = new System.Drawing.Size(286, 24);
            this.StopOnDigits.TabIndex = 26;
            this.StopOnDigits.Tag = "StopOnDigit";
            this.StopOnDigits.Text = "Termina riconoscimento su ricezione Digits dopo";
            this.StopOnDigits.CheckedChanged += new System.EventHandler(this.StopOnDigits_CheckedChanged);
            // 
            // UseSTTForOpenAnswer
            // 
            this.UseSTTForOpenAnswer.AutoSize = true;
            this.UseSTTForOpenAnswer.Checked = true;
            this.UseSTTForOpenAnswer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseSTTForOpenAnswer.Location = new System.Drawing.Point(14, 27);
            this.UseSTTForOpenAnswer.Name = "UseSTTForOpenAnswer";
            this.UseSTTForOpenAnswer.Size = new System.Drawing.Size(310, 17);
            this.UseSTTForOpenAnswer.TabIndex = 24;
            this.UseSTTForOpenAnswer.Tag = "UseSTTForOpenAnswer";
            this.UseSTTForOpenAnswer.Text = "Risposta aperta:  usa STT (se non selezionato viene saltata)";
            this.UseSTTForOpenAnswer.UseVisualStyleBackColor = true;
            this.UseSTTForOpenAnswer.CheckedChanged += new System.EventHandler(this.UseSTTForOpenAnswer_CheckedChanged);
            // 
            // lblLanguageCodeSTT
            // 
            this.lblLanguageCodeSTT.AutoSize = true;
            this.lblLanguageCodeSTT.Location = new System.Drawing.Point(18, 106);
            this.lblLanguageCodeSTT.Name = "lblLanguageCodeSTT";
            this.lblLanguageCodeSTT.Size = new System.Drawing.Size(161, 13);
            this.lblLanguageCodeSTT.TabIndex = 18;
            this.lblLanguageCodeSTT.Text = "Codice &Lingua (formato BCP-47):";
            // 
            // cmbEngineName
            // 
            this.cmbEngineName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEngineName.FormattingEnabled = true;
            this.cmbEngineName.Items.AddRange(new object[] {
            "Google Speech API"});
            this.cmbEngineName.Location = new System.Drawing.Point(228, 72);
            this.cmbEngineName.Name = "cmbEngineName";
            this.cmbEngineName.Size = new System.Drawing.Size(182, 21);
            this.cmbEngineName.TabIndex = 17;
            this.cmbEngineName.Tag = "EngineNameSTT";
            // 
            // lblEngineNameSTT
            // 
            this.lblEngineNameSTT.AutoSize = true;
            this.lblEngineNameSTT.Location = new System.Drawing.Point(18, 75);
            this.lblEngineNameSTT.Name = "lblEngineNameSTT";
            this.lblEngineNameSTT.Size = new System.Drawing.Size(163, 13);
            this.lblEngineNameSTT.TabIndex = 16;
            this.lblEngineNameSTT.Text = "&Motore di riconoscimento vocale:";
            // 
            // txtLanguageCode
            // 
            this.txtLanguageCode.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.txtLanguageCode.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.txtLanguageCode.BackColor = System.Drawing.SystemColors.Control;
            this.txtLanguageCode.Caption = "Codice &Lingua (formato BCP-47):";
            this.txtLanguageCode.CurrentApplicationName = null;
            this.txtLanguageCode.LabelHeight = 16;
            this.txtLanguageCode.Location = new System.Drawing.Point(228, 106);
            this.txtLanguageCode.Margin = new System.Windows.Forms.Padding(0);
            this.txtLanguageCode.Name = "txtLanguageCode";
            this.txtLanguageCode.PasswordChar = '\0';
            this.txtLanguageCode.Size = new System.Drawing.Size(182, 22);
            this.txtLanguageCode.TabIndex = 19;
            this.txtLanguageCode.Tag = "EngineLanguageSTT";
            this.txtLanguageCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.txtLanguageCode.Value = "it-IT";
            this.txtLanguageCode.VisibleLabel = false;
            // 
            // tabPageCustomParameters
            // 
            this.tabPageCustomParameters.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageCustomParameters.Controls.Add(this.lstViewCustomParameters);
            this.tabPageCustomParameters.Controls.Add(this.lblInstruction_6);
            this.tabPageCustomParameters.Location = new System.Drawing.Point(4, 22);
            this.tabPageCustomParameters.Name = "tabPageCustomParameters";
            this.tabPageCustomParameters.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCustomParameters.Size = new System.Drawing.Size(424, 405);
            this.tabPageCustomParameters.TabIndex = 7;
            this.tabPageCustomParameters.Text = "Parametri aggiuntivi";
            // 
            // lstViewCustomParameters
            // 
            this.lstViewCustomParameters.Application = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.lstViewCustomParameters.ApplicationName = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.MessengerDialogControl.ApplicationNames.PhonesEnterprise;
            this.lstViewCustomParameters.BackColor = System.Drawing.SystemColors.Control;
            this.lstViewCustomParameters.BtnAddText = "&Nuovo";
            this.lstViewCustomParameters.BtnDeleteText = "&Cancella";
            this.lstViewCustomParameters.BtnEditText = "&Modifica";
            this.lstViewCustomParameters.Caption = "";
            this.lstViewCustomParameters.Columns = null;
            this.lstViewCustomParameters.CurrentApplicationName = null;
            this.lstViewCustomParameters.EditDialogTitle = "Definizione parametro";
            this.lstViewCustomParameters.GridLines = true;
            this.lstViewCustomParameters.Location = new System.Drawing.Point(11, 51);
            this.lstViewCustomParameters.Name = "lstViewCustomParameters";
            this.lstViewCustomParameters.Padding = new System.Windows.Forms.Padding(2);
            this.lstViewCustomParameters.Size = new System.Drawing.Size(405, 342);
            this.lstViewCustomParameters.SortType = Ifm.Phones.Blocks.BaseMessengerServices.Dialogs.GenericListView.SortTypes.Automatic;
            this.lstViewCustomParameters.TabIndex = 4;
            this.lstViewCustomParameters.Tag = "AdditionalParameters";
            this.lstViewCustomParameters.Value = "";
            // 
            // lblInstruction_6
            // 
            this.lblInstruction_6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblInstruction_6.Location = new System.Drawing.Point(8, 11);
            this.lblInstruction_6.Name = "lblInstruction_6";
            this.lblInstruction_6.Size = new System.Drawing.Size(408, 37);
            this.lblInstruction_6.TabIndex = 3;
            this.lblInstruction_6.Text = "Indicare i parametri aggiuntivi da utilizzare per il riconoscimento.";
            // 
            // WasabiIvrIFlow
            // 
            this.Controls.Add(this.tabControl);
            this.Name = "WasabiIvrIFlow";
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.tabControl.ResumeLayout(false);
            this.tabPageTipoChiamata.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPageWasabi.ResumeLayout(false);
            this.tabPageQuestionTTS.ResumeLayout(false);
            this.tabPageQuestionTTSPhrases.ResumeLayout(false);
            this.groupBoxPhrasesASR.ResumeLayout(false);
            this.groupBoxPhrasesDTMF.ResumeLayout(false);
            this.tabPageAnswer.ResumeLayout(false);
            this.tabPageAnswer.PerformLayout();
            this.groupBoxASR.ResumeLayout(false);
            this.groupBoxDTMF.ResumeLayout(false);
            this.tabPagAnswerOpen.ResumeLayout(false);
            this.tabPagAnswerOpen.PerformLayout();
            this.tabPageCustomParameters.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void UseASRForInput_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxASR.Enabled = UseASRForInput.Checked;
            EngineNameASR.Enabled = UseASRForInput.Checked;
            EngineNameASR.Enable = UseASRForInput.Checked;
            groupBoxPhrasesASR.Enabled = UseASRForInput.Checked;
            groupBoxDTMF.Enabled = !UseASRForInput.Checked;
            groupBoxPhrasesDTMF.Enabled = !UseASRForInput.Checked;
            TTS_DTMF_MenuBegin.Enabled = !UseASRForInput.Checked;
            TTS_DTMF_MenuFor.Enabled = !UseASRForInput.Checked;
            TTS_DTMF_Numeric10.Enabled = !UseASRForInput.Checked;
            TTS_DTMF_NumericMenuAnd.Enabled = !UseASRForInput.Checked;
            TTS_DTMF_NumericMenuBegin.Enabled = !UseASRForInput.Checked;
            TTS_ASR_MenuBegin.Enabled = UseASRForInput.Checked;
            TTS_ASR_NumericMenuAnd.Enabled = UseASRForInput.Checked;
            TTS_ASR_NumericMenuBegin.Enabled = UseASRForInput.Checked;
            ASRConfidenceThreshold.Enabled = UseASRForInput.Checked;
            ASRLookAheadTime.Enabled = UseASRForInput.Checked;
            ASRMaxSilence.Enabled = UseASRForInput.Checked;
            DTMFWaitTime.Enabled = !UseASRForInput.Checked;
        }

        private void StopOnDigits_CheckedChanged(object sender, EventArgs e)
        {
            StopDelay.Enabled = StopOnDigits.Checked;
            STTStopOnDigitPhrase.Enabled = StopOnDigits.Checked;
        }

        private void UseSTTForOpenAnswer_CheckedChanged(object sender, EventArgs e)
        {
            cmbEngineName.Enabled = UseSTTForOpenAnswer.Checked;
            txtLanguageCode.Enabled = UseSTTForOpenAnswer.Checked;
            StopDelay.Enabled = UseSTTForOpenAnswer.Checked;
            STTMaxSilence.Enabled = UseSTTForOpenAnswer.Checked;
            STTConfidenceThreshold.Enabled = UseSTTForOpenAnswer.Checked;
            STTEnableBeep.Enabled = UseSTTForOpenAnswer.Checked;
            StopOnDigits.Enabled = UseSTTForOpenAnswer.Checked;
        }

        private void CheckTelefonico_CheckedChanged(object sender, EventArgs e)
        {
            CheckChat.Checked = !CheckTelefonico.Checked;
            setTimeoutChat.Visible = !CheckTelefonico.Checked;
            if (CheckTelefonico.Checked) // telefonico
            {
                //if (this.tabControl.TabPages.Contains(this.tabPageQuestionCHAT))
                //    this.tabControl.TabPages.Remove(this.tabPageQuestionCHAT);
    
                if (!this.tabControl.TabPages.Contains(this.tabPageQuestionTTS))
                    this.tabControl.TabPages.Insert(2, this.tabPageQuestionTTS);

                if (!this.tabControl.TabPages.Contains(this.tabPageQuestionTTSPhrases))
                    this.tabControl.TabPages.Insert(3,this.tabPageQuestionTTSPhrases);

                if (!this.tabControl.TabPages.Contains(this.tabPageAnswer))
                    this.tabControl.TabPages.Insert(4,this.tabPageAnswer);

                if (!this.tabControl.TabPages.Contains(this.tabPagAnswerOpen))
                    this.tabControl.TabPages.Insert(5,this.tabPagAnswerOpen);
            }
            else //chat
            {
                //if (!this.tabControl.TabPages.Contains(this.tabPageQuestionCHAT))
                //    this.tabControl.TabPages.Insert(1, this.tabPageQuestionCHAT);

                if (this.tabControl.TabPages.Contains(this.tabPageQuestionTTS))
                    this.tabControl.TabPages.Remove(this.tabPageQuestionTTS);

                if (this.tabControl.TabPages.Contains(this.tabPageQuestionTTSPhrases))
                    this.tabControl.TabPages.Remove(this.tabPageQuestionTTSPhrases);

                if (this.tabControl.TabPages.Contains(this.tabPageAnswer))
                    this.tabControl.TabPages.Remove(this.tabPageAnswer);

                if (this.tabControl.TabPages.Contains(this.tabPagAnswerOpen))
                    this.tabControl.TabPages.Remove(this.tabPagAnswerOpen);
            }
        }

        private void CheckChat_CheckedChanged(object sender, EventArgs e)
        {
            CheckTelefonico.Checked = !CheckChat.Checked;
            maxRetry.Visible = CheckTelefonico.Checked;
            setTimeoutChat.Visible = CheckChat.Checked;
            //lblInstructionPathMsg.Visible = !CheckChat.Checked;
            MessagesPath.Visible = !CheckChat.Checked;
        }
    }
}
