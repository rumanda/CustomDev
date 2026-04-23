
//==========================================================================================================

// Copyright © IFM Infomaster. All rights reserved.

//==========================================================================================================

// Author    : Marco Solinas [MSO]
// Date      : Mar 2017
// Revisions : 

//==========================================================================================================

#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion Namespaces

namespace Ifm.Components.Messenger.Blocks.Utilities {
    internal class HelperClass {
	
        #region Enums
		
        public enum StopReasons : int {
            msgNull = -1,
            msgTransfer = 0,
            msgEndCall = 1,
            msgPreQueueTransfer = 2,
            msgRemoteHangUp = 3,
            MsgNoResources = 4,
            msgReadyForTransfer = 5,
            msgMaxCallTimerExp = 6,
            msgLineDropped = 7
        }

        public enum ActionResults : int {
            vvpErrorResolvingHostName = -6,
            vvpResponseTimeout = -5,
            vvpVKloneServerConnectionFailed = -4,
            vvpAlreadyInitialized = -3,
            vvpNotConnected = -2,
            vvpRuntimeError = -1,
            vvpOK = 0,
            vvpStopped = 1,
            vvpTermDigit = 2,
            vvpMaxSilence = 3,
            vvpMaxNonSilence = 4,
            vvpMaxDigits = 5,
            vvpTimeOut = 6,
            vvpTimeOutInterDigit = 7,
            vvpConnected = 8,
            vvpBusy = 9,
            vvpNoRing = 10,
            vvpNoAnswer = 11,
            vvpOperatorIntercept = 12,
            vvpCallError = 13,
            vvpFax = 14,
            vvpNoDialTone = 15,
            vvpInvalidCallId = 16,
            vvpInvalidEngine = 17,
            vvpEngineUnavailable = 18,
            vvpNotRecognized = 19
        };

        #endregion Enums

        #region Data Members
		
        private dynamic mHandler;
        private string className;
		
        #endregion Data Members

        #region Constructors
		
        public HelperClass(dynamic voiceHandler, string serviceName) {
            mHandler = voiceHandler;
            className = serviceName;
        }

        ~HelperClass() {
            mHandler = null;
        }

        #endregion Constructors

        #region Public Methods

        public void LogString(string message) {
            if (mHandler != null) {
                mHandler.LogString(string.Format("{0}::{1}", className, message));
            }
        }

        public string GetParamValue(string name, bool printValue = true) {
            string rc = "";

            if (mHandler != null) {
                rc = mHandler.getParamValue(name);
                LogString(string.Format("GetParamValue - Name = {0} - Value = {1}", name, printValue ? rc : "*****"));
            }

            return rc;
        }

        public void SetTransferPropertyValue(string name, string value, bool printValue = true) {
            if (mHandler != null) {
                LogString(string.Format("Setting TransferPropertyValue: Name='{0}', Value='{1}'", name, printValue ? value : "*****"));
                mHandler.TransferPropertyValue[name] = value;
            }
        }

        public string GetTransferPropertyValue(string name, bool printValue = true) {
            string tpValue = "";

            if (mHandler != null) {
                tpValue = mHandler.TransferPropertyValue[name];
                LogString(string.Format("Get TransferPropertyValue: Name='{0}', Value='{1}'", name, printValue ? tpValue : "*****"));
            }

            return tpValue;
        }


        public void SetCallDataValue(string name, string value, bool printValue = true) {
            if (mHandler != null) {
                LogString(string.Format("Setting CallData Value: Name='{0}', Value='{1}'", name, printValue ? value : "*****"));
                mHandler.monitorManager.currentCall.callData.setValue(name, value);
            }
        }

        public string GetCallDataValue(string name, bool printValue = true) {
            string cdValue = "";

            if (mHandler != null) {
                cdValue = mHandler.monitorManager.currentCall.callData.getPropertyValue(name);
                LogString(string.Format("Get CallData Value: Name='{0}', Value='{1}'", name, printValue ? cdValue : "*****"));
            }

            return cdValue;
        }

        public void PlayGreeting(IList<CGreeting> greetingList, string termDigits = "", bool autoClear = true, bool asyncPlay = false) {
            try {
                foreach (CGreeting gr in greetingList) {
                    if ((StopReason != StopReasons.msgNull) && (Interruptable)) {
                        LogString(string.Format("PlayGreeting Stopped = {0}", StopReason));
                        return;
                    }

                    if (asyncPlay) {
                        if (gr.GreetingType == CGreeting.GreetingTypes.Wav) {
                            mHandler.playFileA(gr.Message, termDigits, autoClear);
                        }
                        else {
                            LogString(string.Format("PlayGreetingsList WARNING: {0} cannot be played in ASYNC mode - SKIPPED", gr.GreetingType));
                        }
                    }
                    else {
                        switch (gr.GreetingType) {
                            case CGreeting.GreetingTypes.Wav:
                                mHandler.PlayFile(gr.Message, 0, termDigits, autoClear);
                                break;

                            case CGreeting.GreetingTypes.Date:
                                DateTime date;

                                if (DateTime.TryParse(gr.Message, out date)) {
                                    mHandler.PlayDate(date, gr.Format, termDigits, autoClear, gr.LanguageParameters);
                                }
                                else {
                                    LogString(string.Format("ERROR - Invalid date = {0}" + gr.Message));
                                }
                                break;

                            case CGreeting.GreetingTypes.Today:
                                mHandler.PlayDate(DateTime.Now, gr.Format, termDigits, autoClear, gr.LanguageParameters);
                                break;

                            case CGreeting.GreetingTypes.FileDate:
                                mHandler.PlayDate(mHandler.FileDateTime(gr.Message), gr.Format, termDigits, autoClear, gr.LanguageParameters);
                                break;

                            case CGreeting.GreetingTypes.Number:
                                mHandler.PlayNumber(gr.Message, gr.Format, termDigits, autoClear, gr.LanguageParameters);
                                break;

                            case CGreeting.GreetingTypes.Money:
                                mHandler.PlayMoney(gr.Message, gr.Format, termDigits, autoClear, gr.LanguageParameters);
                                break;

                            case CGreeting.GreetingTypes.Ordinal:
                                int gender;

                                if (int.TryParse(gr.Format.Substring(0, 1), out gender)) {
                                    if (gender == 0) {
                                        // Female
                                        mHandler.PlayOrdinal(gr.Message, true, termDigits, autoClear, gr.LanguageParameters);
                                    }
                                    else {
                                        // Male
                                        mHandler.PlayOrdinal(gr.Message, false, termDigits, autoClear, gr.LanguageParameters);
                                    }
                                }
                                else {
                                    if ((gr.Format.Substring(0, 1).ToUpper() == "F") || (gr.Format.ToUpper() == "WEIBLICH")) {
                                        // Female
                                        mHandler.PlayOrdinal(gr.Message, true, termDigits, autoClear, gr.LanguageParameters);
                                    }
                                    else {
                                        // Male
                                        mHandler.PlayOrdinal(gr.Message, false, termDigits, autoClear, gr.LanguageParameters);
                                    }
                                }
                                break;

                            case CGreeting.GreetingTypes.Digits:
                                mHandler.PlayCharacters(gr.Message, termDigits, autoClear, gr.LanguageParameters);
                                break;

                            case CGreeting.GreetingTypes.FormattedDigits:
                                string tempStr = gr.Message.Trim();
                                string curVal = "";
                                int tmpVal = 0;

                                for (int pos = 0; pos < tempStr.Length; pos++) {
                                    if (int.TryParse(tempStr.Substring(pos, 1), out tmpVal)) {
                                        curVal += tempStr.Substring(pos, 1);
                                    }
                                    else {
                                        if (curVal.Length > 0) {
                                            mHandler.PlayNumber(curVal, 0, termDigits, autoClear, gr.LanguageParameters);
                                            curVal = "";
                                        }

                                        int pauseTm = mHandler.pauseTime(tempStr.Substring(pos, 1), gr.Format);

                                        if (pauseTm < 0) {
                                            mHandler.PlayCharacters(tempStr.Substring(pos, 1), termDigits, autoClear, gr.LanguageParameters);
                                            curVal = "";
                                        }
                                        else if (pauseTm > 0) {
                                            mHandler.mySleep(pauseTm);
                                        }
                                    }

                                    if ((ActionResult == ActionResults.vvpStopped) || (ActionResult == ActionResults.vvpTermDigit) || (StopReason != StopReasons.msgNull && Interruptable)) {
                                        return;
                                    }
                                }

                                if (curVal.Length > 0) {
                                    mHandler.PlayNumber(curVal, 0, termDigits, autoClear, gr.LanguageParameters);
                                    curVal = "";
                                }
                                break;
                        }
                    }

                    if ((ActionResult == ActionResults.vvpStopped) || (ActionResult == ActionResults.vvpTermDigit) || (StopReason != StopReasons.msgNull && Interruptable)) {
                        return;
                    }
                }
            }
            catch (Exception ex) {
                LogString(string.Format("ERROR in PlayGreeting: {0}", ex));
            }
        }
		
        #endregion Public Methods

        #region Properties
		
        public ActionResults ActionResult {
            get {
                if (mHandler != null) {     
					return (ActionResults)((int)mHandler.actionResult);
                }

                return ActionResults.vvpOK;
            }
        }
        public StopReasons StopReason {
            get {
                if (mHandler != null) {
                    return (StopReasons)((int)mHandler.stopReason);
                }

                return StopReasons.msgNull;
            }
        }

        public bool Interruptable {
            get {
                if (mHandler != null) {
                    return mHandler.interruptable;
                }

                return true;
            }
        }
		
        #endregion Properties
    }

    internal class CGreeting {
        #region Enums
		
        public enum GreetingTypes: int {
            Wav = 0,
            Date = 1,
            Today = 2,
            FileDate = 3,
            Number = 4,
            Money = 5,
            Digits = 6,
            Ordinal = 7,
            FormattedDigits = 8
        }
		
        #endregion Enums

        #region Data Members
		
        private GreetingTypes _type = GreetingTypes.Wav;
        private string _body;
        private string _format;
        private string _languageParameters = "";
		
        #endregion Data Members

        #region Constructors
		
        public CGreeting() { }

        public CGreeting(GreetingTypes type, string message, string format, string languageParams) {
            _type = type;
            _body = message;
            _format = format;
            _languageParameters = languageParams;
        }
		
        #endregion Constructors

        #region Properties
		
        public GreetingTypes GreetingType {
            get { return _type; }
            set { _type = value; }
        }

        public string Message {
            get { return _body; }
            set { _body = value; }
        }

        public string Format {
            get { return _format; }
            set { _format = value; }
        }

        public string LanguageParameters {
            get { return _languageParameters; }
            set { _languageParameters = value; }
        }
		
        #endregion Properties
    }
}
