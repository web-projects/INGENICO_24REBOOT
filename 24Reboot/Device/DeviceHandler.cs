using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBA_SDK;

namespace DeviceUpdater
{
    public static class DeviceHandler
    {
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) where T : EventArgs
        {
            EventHandler<T> copy = handler;
            if (copy != null)
            {
                copy(sender, args);
            }
        }
    }

    public class Device
    {
        #region -- attributes ---
        public delegate void TerminalDiscoveryCallback();

        public event EventHandler<DeviceEventArgs> DeviceInputReceived;
        public event EventHandler<DeviceConnectionArgs> DeviceConnectionChanged;

        //public delegate void EventHandler(object sender, NotificationEventArgs args);
        //public event EventHandler<NotificationEventArgs> OnNotification = delegate { };
        
        public class DeviceEventArgs : EventArgs
        {
            private readonly MESSAGE_ID _messageID;
            private readonly string _deviceForm;
            private readonly string _keyPressID;
            
            public MESSAGE_ID MessageID
            {
                get { return _messageID; }
            }

            public string DeviceForm
            {
                get { return _deviceForm; }
            }
            
            public string KeyPressID
            {
                get { return _keyPressID; }
            }

            public DeviceEventArgs(MESSAGE_ID messageID, string deviceForm, string keyPressID)
            {
                _messageID = messageID;
                _deviceForm = deviceForm;
                _keyPressID = keyPressID;
            }
        }

        public class DeviceConnectionArgs : EventArgs
        {
            private readonly CONNECTION_STATUS _connectionStatus;
            public CONNECTION_STATUS ConnectionStatus
            {
                get { return _connectionStatus;  }
            }
            public DeviceConnectionArgs(CONNECTION_STATUS connectionStatus)
            {
                _connectionStatus = connectionStatus;
            }
        }

        public enum DeviceForms
        {
            [StringValue("MSG.K3Z")]
            Message = 1,
            [StringValue("AMTV.K3Z")]
            Amount = 2,
            [StringValue("COD.K3Z")]
            CardReadRequest = 3,
            [StringValue("APPDAPP.K3Z")]
            ApprovedDeclined = 4,
            [StringValue("ADS.K3Z")]
            Adverstisement = 5,
            [StringValue("OFFILINE.K3Z")]
            Offline = 6,
            [StringValue("SWIPE.K3Z")]
            Swipe = 7,
            [StringValue("PAY1.K3Z")]
            PaymentType = 8,
            [StringValue("INPUT.K3Z")]
            Pin = 9,
            [StringValue("CASHB.K3Z")]
            CashBack = 10,
            [StringValue("CASHBO.K3Z")]
            CashBackAmount = 11,
            [StringValue("CASHBV.K3Z")]
            CashBackVerify = 12,
            [StringValue("PRESIGN.K3Z")]
            Signature = 13,
            [StringValue("CCOD.K3Z")]
            CardReadContactLess = 14,
            [StringValue("TC-WAIT.K3Z")]
            DeviceBusy = 15,
            [StringValue("ECONFIRM.K3Z")]
            EMVAmountConfirm = 16               
        }

        public enum DeviceFormTypeOf
        {
            [StringValue("T")]
            Text,
            [StringValue("B")]
            Button
        }

        public enum DeviceFormElementID
        {
            [StringValue("PROMPTLINE1")]
            PromptLine1,
            [StringValue("PROMPTLINE2")]
            PromptLine2,
            [StringValue("PROMPTLINE3")]
            PromptLine3,
            [StringValue("PROMPTLINE4")]
            PromptLine4,
            [StringValue("bbtnyes")]
            ButtonYes,
            [StringValue("bbtnno")]
            ButtonNo,
            [StringValue("bbtnc")]
            ButtonCashBack,
            [StringValue("bbtnpp")]
            ButtonPartial,
            [StringValue("bbtnman")]
            ButtonManual,
            [StringValue("PROMPT3")]
            Prompt3,
            [StringValue("linedisplay1")]
            LineDisplay1,
            [StringValue("bbtndebit")]
            ButtonDebit,
            [StringValue("bbtncred")]
            ButtonCredit,
            [StringValue("bbtncash")]
            ButtonEBTCash,
            [StringValue("bbtnfood")]
            ButtonEBTFood,
            [StringValue("bbtnst")]
            ButtonStore,
            [StringValue("bbtnclear")]
            ButtonClear
         
        }

        public enum TransactionType
        {
            [StringValue("01")]
            Sale = 1,
            [StringValue("02")]
            Void = 2,
            [StringValue("03")]
            Return = 3,
            [StringValue("04")]
            VoidReturn = 4
        }

        public enum ManualCardOptions
        {
            [StringValue("1")]
            DisplayAll,
            [StringValue("2")]
            DisplayNoCVV,
            [StringValue("3")]
            DisplayNoExp,
            [StringValue("4")]
            DisplayNoExpNoCVV
        }

        public enum DeviceEquipment
        {
            [StringValue("iSC250")]
            iSC250 = 1,
            [StringValue("iPP320")]
            iPP3250 = 2,
            [StringValue("iPP350")]
            iPP350 = 3,
            [StringValue("iSC480")]
            iSC480 = 4,
            [StringValue("iUP250")]
            iUP250 = 5
        }

        public enum DeviceUpdate
        {
            [StringValue("Firmware")]
            Firmware = 1,
            [StringValue("Forms")]
            Forms = 2
        }

        public string Result { get; set; }
        public string CurrentForm { get; set; }
        public bool OnDemandSet { get; set; }
        public bool Connected { get; set; }
        public bool OnGuardEnabled { get; set; }
        public bool DebitKey { get; set; }
//        public CardInfo CardDetails { get; set; }
//        public CardInfo.OnGuardInfo OnGaurdData { get; set; }
        public string DeviceCardSource { get; set; }
        //public Form connectedForm { get; set; }
        public bool EncryptionEnabled { get; set; }
        public bool EncryptionKeyFound { get; set; }
        public string FormsVersion { get; set; }
        public string CardSource { get; set; }

//        public ParseTags pT = new ParseTags();

        public List<string> AIDs = new List<string>();

        public string OfflinePINDectected { get; private set; }
        public string OnlinePINReqested { get; private set; }
        public string EMVStarted { get; private set; }
        public string EMVCompleted { get; private set; }
        public string LanguageSelected { get; private set; }
        public string AppSelected { get; private set; }
        public string AppConfirmed { get; private set; }
        public string RewardReqReceived { get; private set; }
        public string PaymentTypeReceived { get; private set; }
        public string AmountConfirmed { get; private set; }
        public string LastPinTry { get; private set; }
        public string OfflinePINEntered { get; private set; }
        public string AccountTypeSelect { get; private set; }
        public string AuthReqSent { get; private set; }
        public string AuthResReceived { get; private set; }
        public string ConfirmationResSent { get; private set; }
        public string TransactionCancelled { get; private set; }
        public string CardCannotRead { get; private set; }
        public string CardOrAppBlocked { get; private set; }
        public string ErrorDetected { get; private set; }
        public string PrematureCardRemoval { get; private set; }
        public string CardNotSupported { get; private set; }
        public string MacVerification { get; private set; }
        public string PostConfirmStartToWait { get; private set; }
        public string SignatureRequest { get; private set; }
        public string TransactionPreparationSent { get; private set; }
        public string EMVFlowSuspended { get; private set; }
        public string CurrentEMVStep { get; private set; }
        public string EMVCashback { get; private set; }
        public int ComBaudRate;
        public int ComDataBits;
        #endregion

        public Device()
        {
//            CardDetails = new CardInfo();
//            OnGaurdData = new CardInfo.OnGuardInfo();
        }
        public string Connect(string port, LogHandler traceLog, IngenicoLoggingLevel logLevel)
        {
            RBA_API.logHandler = new LogHandler(traceLog);
            RBA_API.SetDefaultLogLevel((LOG_LEVEL)logLevel);

            //TODO - look at this to see if detecting error here helps?
            ERROR_ID InitResult = RBA_API.Initialize();

            RBA_API.SetNotifyRbaConnected(new ConnectHandler(DeviceConnectionEvent));
            RBA_API.SetNotifyRbaDisconnected(new DisconnectHandler(DeviceConnectionEvent));

            RBA_API.pinpadHandler = new PinPadMessageHandler(pinpadHandler);

            ERROR_ID Result = SetDeviceCommunications(port);

            //set configuration for on demand
            //TODO - make this configuration driven
            SetDeviceToOnDemand();
            SoftReset();

            if (Result.ToString().Contains("SUCCESS") || Result.ToString().Contains("RESULT_ERROR_ALREADY_CONNECTED"))
            {
                Connected = true;
                
                //make sure Encryption is on
//                if (!IsEncryptionOn(CheckKeys()))
//                {
//                    Connected = false;
//                }
            }

            return Result.ToString();
        }

        public void DeviceConnectionEvent()
        {
            CONNECTION_STATUS ConnectionStatus = RBA_API.GetConnectionStatus();

            if (ConnectionStatus == CONNECTION_STATUS.DISCONNECTED || ConnectionStatus == CONNECTION_STATUS.CONNECTED_NOT_READY)
            {
                Connected = false;
            }
            
            DeviceConnectionChanged.Raise(null, new DeviceConnectionArgs(RBA_API.GetConnectionStatus()));
        }

        public void pinpadHandler(MESSAGE_ID msgID)
        {
            ERROR_ID Result;
            string message = string.Empty;

            switch (msgID)
            {
           
                case MESSAGE_ID.M00_OFFLINE:
                    {
                        
                        RBA_API.GetParam(PARAMETER_ID.P00_RES_REASON_CODE);
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P33_00_RES_STATUS), message));
                        break;
                    }
                case MESSAGE_ID.M09_SET_ALLOWED_PAYMENT:
                    {
                        string CardType = RBA_API.GetParam(PARAMETER_ID.P09_RES_CARD_TYPE);
                        string CardStatus = RBA_API.GetParam(PARAMETER_ID.P09_RES_CARD_STATUS);
                        if ((CardType == "02" || CardType == "99") && CardStatus == "I")
                        {
                            message = "**Card inserted";
                        }
                        else if ((CardType == "02" || CardType == "99") && CardStatus == "R")
                        {
                             message = "**Card Removed";
                        }
                        else if (CardStatus == "P")
                        {
                            message = "**Unknown problem with Card Insertion / Removal";
                        }
                        else
                        {
                            message = "**Unknown Card activity";
                        }

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P09_RES_CARD_TYPE), message));
                        break;
                    }
                case MESSAGE_ID.M10_HARD_RESET:
                    {

                        DeviceInputReceived(null, new DeviceEventArgs(MESSAGE_ID.M10_HARD_RESET, "", "EMVAmtVerifyNo"));

                        break;
                    }
                case MESSAGE_ID.M19_BIN_LOOKUP:
                    {
                        message = "Unsolicited message: " + msgID + "\n";
//                        message += Send19Response();

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P19_REQ_LAYOUT_ID), message));
                        break;
                    }

                case MESSAGE_ID.M20_SIGNATURE:
                    {
                        message = ("Unsolicited message: " + msgID + "\n");
                        
                        var key_press = RBA_API.GetParam(PARAMETER_ID.P20_RES_KEY);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P20_REQ_FORM_NAME), key_press));
                        break;

                    }
                case MESSAGE_ID.M21_NUMERIC_INPUT:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("P21_RES_EXIT_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P21_RES_EXIT_TYPE));
                        message += ("P21_RES_INPUT_DATA: " + RBA_API.GetParam(PARAMETER_ID.P21_RES_INPUT_DATA).ToString());
                        
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P21_REQ_FORMAT_SPECIFIER), message));
                        break;
                        
                    }
                case MESSAGE_ID.M23_CARD_READ:
                    {
                        
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("P23_RES_EXIT_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P23_RES_EXIT_TYPE));

                        var track1 = (RBA_API.GetParam(PARAMETER_ID.P23_RES_TRACK1));

/*                        CardDetails.Track1 = track1;

                        var track2  = (RBA_API.GetParam(PARAMETER_ID.P23_RES_TRACK2));

                        CardDetails.Track2 = track2;

                        var track3 = (RBA_API.GetParam(PARAMETER_ID.P23_RES_TRACK3));

                        CardDetails.Track3 = track3;

                        CardDetails = ParseTrackData(CardDetails);
                        CardDetails = PatchUpTrackData(CardDetails);

                        CardSource = (RBA_API.GetParam(PARAMETER_ID.P23_RES_CARD_SOURCE));
                        DeviceCardSource = CardSource;

                        //Update account number in 31 request section
                        string PAN = GetVariable_29("000398");
                        CardDetails.PAN = PAN;
                        CardDetails.EncryptedTrack = string.Format("{0}|{1}|{2}", CardDetails.Track1, CardDetails.Track2, CardDetails.Track3);
                        
                        //this.Invoke((MethodInvoker)delegate ()
                        //{
                        //    txtPinAccountNumber.Text = PAN;
                        //});
                        /* // Following retrieves ETB if Voltage Encryption is enabled
                        Result = RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "91");
                        Result = RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "1");
                        Result = RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);
                        string dataConfig =RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);
                        if (dataConfig == "4" || dataConfig == "5" || dataConfig == "6")
                        {
                            Result = RBA_API.SetParam(PARAMETER_ID.P90_REQ_FUNCTION, "1");
                            Result = RBA_API.ProcessMessage(MESSAGE_ID.M90_MSR_ENCRYPTION);
                            string status = RBA_API.GetParam(PARAMETER_ID.P90_RES_STATUS);
                            pinpadLogger("Status: " + status);
                            if (status == "0")
                            {
                                string ETB ="";
                                ETB = RBA_API.GetParam(PARAMETER_ID.P90_RES_ETB);
                                pinpadLogger("ETB Length: " + ETB.Length);
                                pinpadLogger("ETB: " + ETB);
                            }
                         }* /
*/
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P23_REQ_FORM_NAME), message));
                        break;
                    }
                case MESSAGE_ID.M24_FORM_ENTRY:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        string ExitType = RBA_API.GetParam(PARAMETER_ID.P24_RES_EXIT_TYPE);
                        message += ("P24_RES_EXIT_TYPE: " + ExitType);
                        //pinpadLogger("P24_RES_BUTTON_STATE: " + RBA_API.GetParam(PARAMETER_ID.P24_RES_BUTTON_STATE));
                        string keyID = RBA_API.GetParam(PARAMETER_ID.P24_RES_KEYID);
                        message += ("P24_RES_KEYID: " + keyID);
                        int length = RBA_API.GetParamLen(PARAMETER_ID.P24_RES_BUTTONID);
                        string buttonState;
                        string buttonID;
                        
                        //For checkboxes / Radio buttons
                        while (length > 0)
                        {
                            buttonState = RBA_API.GetParam(PARAMETER_ID.P24_RES_BUTTON_STATE);
                            buttonID = RBA_API.GetParam(PARAMETER_ID.P24_RES_BUTTONID);
                            message += ("P24_REQ_BUTTONID: : " + buttonID);
                            message += ("P24_REQ_BUTTON_STATE : " + buttonState);
                            length = RBA_API.GetParamLen(PARAMETER_ID.P24_RES_BUTTONID);
                        }
                        
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER), keyID));
                        break;
                    }

                case MESSAGE_ID.M25_TERMS_AND_CONDITIONS:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        string SigOnAccept = RBA_API.GetParam(PARAMETER_ID.P25_RES_SIGNATURE_ON_ACCEPT);
                        message += ("P25_RES_SIGNATURE_ON_ACCEPT: " + SigOnAccept);
                        string KeyPressed = RBA_API.GetParam(PARAMETER_ID.P25_RES_KEY_PRESSED);
                        message += ("P25_RES_KEY_PRESSED: " + KeyPressed);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER), KeyPressed));
                        break;
                    }

                case MESSAGE_ID.M27_ALPHA_INPUT:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("P27_RES_EXIT_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P27_RES_EXIT_TYPE));
                        message += ("P27_RES_DATA_INPUT: " + RBA_API.GetParam(PARAMETER_ID.P27_RES_DATA_INPUT));

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P27_REQ_FORM_SPECIFICID), message));
                        break;
                    }
                case MESSAGE_ID.M31_PIN_ENTRY:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("P31_RES_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P31_RES_STATUS));

//                        CardDetails.EncryptedPIN = RBA_API.GetParam(PARAMETER_ID.P31_RES_PIN_DATA);     //RBA_API only allows this value to be retrieved once; second read returns empty string

//                        message += ("P31_RES_PIN_DATA: " + CardDetails.EncryptedPIN);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P31_REQ_FORM_NAME), message));
                        break;
                    }

                case MESSAGE_ID.M33_01_EMV_STATUS:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("P33_01_RES_TRANSACTION_CODE: " + RBA_API.GetParam(PARAMETER_ID.P33_01_RES_TRANSACTION_CODE));
                        string Flag1 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F1_CHIP_CARD);
                        EMVStarted = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F2_EMV_STARTED);
                        EMVCompleted = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F3_EMV_COMPLETED);
                        LanguageSelected = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F4_LANGUAGE_SELECTED);
                        AppSelected = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F5_APP_SELECTED);
                        AppConfirmed = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F6_APP_CONFIRMED);
                        RewardReqReceived = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F7_REWARD_REQ_RECEIVED);
                        PaymentTypeReceived = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F8_PAYMENT_TYPE_RECEIVED);
                        AmountConfirmed = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F9_AMOUNT_CONFIRMED);
                        LastPinTry = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F10_LAST_PIN_TRY);
                        OfflinePINEntered = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F11_OFFLINE_PIN_ENTERED);
                        AccountTypeSelect = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F12_ACCOUNT_TYPE_SELECTED);
                        AuthReqSent = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F13_AUTH_REQ_SENT);
                        AuthResReceived = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F14_AUTH_RES_RECEIVED);
                        ConfirmationResSent = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F15_CONFIRMATION_RES_SENT);
                        TransactionCancelled = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F16_TRANSACTION_CANCELLED);
                        CardCannotRead = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F17_CARD_CANNOT_READ);
                        CardOrAppBlocked = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F18_CARD_OR_APP_BLOCKED);
                        ErrorDetected = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F19_ERROR_DETECTED);
                        PrematureCardRemoval = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F20_PREMATURE_CARD_REMOVAL);
                        CardNotSupported = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F21_CARD_NOT_SUPPORTED);
                        MacVerification = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F22_MAC_VERIFICATION);
                        PostConfirmStartToWait = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F23_POST_CONFIRM_START_TO_WAIT);
                        SignatureRequest = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F24_SIGNATURE_REQUEST);
                        TransactionPreparationSent = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F25_TRANSACTION_PREPARATION_SENT);
                        EMVFlowSuspended = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F26_EMV_FLOW_SUSPENDED);
                        OnlinePINReqested = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F27_ONLINE_PIN_REQUESTED);
                        CurrentEMVStep = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F28_CURRENT_EMV_STEP);
                        string Flag29 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F29_RESERVED);
                        string Flag30 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F30_RESERVED);
                        EMVCashback = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F31_EMV_CASHBACK);
                        string Flag32 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F32_CONTACTLESS_STATUS);
                        string Flag33 = RBA_API.GetParam(PARAMETER_ID.P33_01_RES_F33_CONTACTLESS_ERROR);
                        
//                        ParseStatusEventFlags(Flag1, EMVStarted, EMVCompleted, LanguageSelected, AppSelected, AppConfirmed, RewardReqReceived, PaymentTypeReceived, AmountConfirmed, LastPinTry, OfflinePINEntered,
//                            AccountTypeSelect, AuthReqSent, AuthResReceived, ConfirmationResSent, TransactionCancelled, CardCannotRead, CardOrAppBlocked, ErrorDetected, PrematureCardRemoval, CardNotSupported,
//                            MacVerification, PostConfirmStartToWait, SignatureRequest, TransactionPreparationSent, EMVFlowSuspended, OnlinePINReqested, CurrentEMVStep, Flag29, Flag30, EMVCashback, Flag32, Flag33);
                        
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P33_01_REQ_STATUS), message));
                        //TODO - need to send back the EMV flags
                        break;
                    }

                case MESSAGE_ID.M33_02_EMV_TRANSACTION_PREPARATION_RESPONSE:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("****************************************************************");
                        string Status = RBA_API.GetParam(PARAMETER_ID.P33_02_RES_STATUS);

                        ///*Method to retrieve EMV Tag Data returns everything in a byte array, 
                        //application does not need to convert the data to the required data type*/
                        byte[] byteTagData = new byte[1];
                        if (Status == "E")
                            message += (" ERROR RECEIVED");
                        while (true)
                        {
                            int TagParamLength = RBA_API.GetTagParamLen(msgID);
                            if (TagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgID, out byteTagData);
//                            string strTagData = ByteArrayToString(byteTagData);
//                            pT.ParseEMVTags(tagId.ToString("X"), TagParamLength, strTagData, byteTagData);
                        }
                        ///End of Method1
                        message += ("****************************************************************");
                        message += ("");
                        
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);

                        //set the emconfirm.k3z form as current form for device
//                        CurrentForm = StringEnum.GetStringValue(DeviceForms.EMVAmountConfirm);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P33_02_RES_STATUS), message));
                        break;
                    }


                case MESSAGE_ID.M33_03_EMV_AUTHORIZATION_REQUEST:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("****************************************************************");
                        string Status = RBA_API.GetParam(PARAMETER_ID.P33_03_REQ_STATUS);
                        
                        ///*Method to retrieve EMV Tag Data returns everything in a byte array, 
                        //application does not need to convert the data to the required data type*/
                        byte[] byteTagData = new byte[1];
                        if (Status == "E")
                            message += (" ERROR RECEIVED");

                        while (true)
                        {
                            int TagParamLength = RBA_API.GetTagParamLen(msgID);
                            if (TagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgID, out byteTagData);
//                            string strTagData = ByteArrayToString(byteTagData);
//                            pT.ParseEMVTags(tagId.ToString("X"), TagParamLength, strTagData, byteTagData);
                            

                        }
                        message += ("****************************************************************");
                        message += ("");
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);
                        //For Testing
                        //Use "33.04 Authorization Response" button from "EMV Messages" group box in On-demand messages list box
                        //SendAuthorizationReponse();

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P33_03_REQ_STATUS), message));
                        break;
                    }

                case MESSAGE_ID.M33_05_EMV_AUTHORIZATION_CONFIRMATION:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("****************************************************************");
                        string Status = RBA_API.GetParam(PARAMETER_ID.P33_05_RES_STATUS);

                        ///*Method to retrieve EMV Tag Data gives everything as a byte array, 
                        //application does not need to convert the data to the required type*/
                        byte[] byteTagData = new byte[1];
                        if (Status == "E")
                            message += (" ERROR RECEIVED");

                        while (true)
                        {
                            int TagParamLength = RBA_API.GetTagParamLen(msgID);
                            if (TagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgID, out byteTagData);
//                            string strTagData = ByteArrayToString(byteTagData);
//                            pT.ParseEMVTags(tagId.ToString("X"), TagParamLength, strTagData, byteTagData);
                            
                        }
                        message += ("****************************************************************");
                        message += ("");
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);
                        
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P33_05_RES_STATUS), message));
                        break;
                    }
                case MESSAGE_ID.M33_07_EMV_TERMINAL_CAPABILITIES:
                    {
                        message += ("");
                        message += ("****************************************************************");
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("****************************************************************");
                        string Status = RBA_API.GetParam(PARAMETER_ID.P33_07_REQ_STATUS);

                        byte[] byteTagData = new byte[1];
                        byte[] byteAID = new byte[1];
                        if (Status == "E")
                            message += (" ERROR RECEIVED");

                        while (true)
                        {
                            int TagParamLength = RBA_API.GetTagParamLen(msgID);
                            if (TagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgID, out byteTagData);
//                            message += ("TAG ID = " + tagId.ToString("X") + " Tag Param Length = " + TagParamLength + " TagData = " + ByteArrayToString(byteTagData));
                            int Tag84 = 0;

                            if (tagId == 132)
                            {
                                Tag84 = tagId;
                                byteAID = byteTagData;
                                //this.Invoke((MethodInvoker)delegate ()
                                //{
                                //    txtAID.Text = ByteArrayToString(byteTagData).ToUpper();
                                //});
                            }
                        }
                        message += ("****************************************************************");
                        message += ("");
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P33_07_RES_STATUS), message));
                        ////To automatically send 33.07 response for testing purposes.
                        //SetEMVTerminalCapabilities();
                        break;
                    }
/*                case MESSAGE_ID.M33_11_EMV_EXTERNAL_AID_SELECT_NOTIFICATION:
                    {
                        message += "";
                        message += "****************************************************************";
                        message += "Unsolicited message: " + msgID + "\n";
                        message += "****************************************************************";
                        string Status = RBA_API.GetParam(PARAMETER_ID.P33_11_REQ_STATUS);

                        AIDs = new List<string>();

                        byte[] byteTagData = new byte[1];
                        byte[] byteAID = new byte[1];
                        if (Status == "E")
                            message += "ERROR RECEIVED";

                        string strTagData = string.Empty;
                        
                        while (true)
                        {
                            int TagParamLength = RBA_API.GetTagParamLen(msgID);
                            if (TagParamLength <= 0)
                                break;
                            int tagId = RBA_API.GetTagParam(msgID, out byteTagData);
                            int Tag79 = 0;

                            strTagData = ByteArrayToString(byteTagData);

                            pT.ParseEMVTags(tagId.ToString("X"), TagParamLength, strTagData, byteTagData);
                            
                            if (tagId == 79)
                            {
                                Tag79 = tagId;
                                byteAID = byteTagData;

                                AIDs.Add(strTagData);
                            }
                            
                        }
                        
                        message += "****************************************************************";
                        message += "";
                        RBA_API.ResetParam(PARAMETER_ID.P_ALL_PARAMS);
                        message += "****************************************************************";
                        message += "";
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P33_11_REQ_STATUS), message));

                        break;
                    }
                case MESSAGE_ID.M35_MENU:
                    {
                        message += "Unsolicited message: " + msgID + "\n";
                        message += "P35_RES_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P35_RES_STATUS);
                        message += "P35_RES_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P35_RES_ID);
                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P35_RES_ID), message));
                        break;
                    }
*/
                case MESSAGE_ID.M41_CARD_READ:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("P41_RES_SOURCE: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_SOURCE));
                        message += ("P41_RES_ENCRYPTION: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_ENCRYPTION));
                        message += ("P41_RES_TRACK_1_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_1_STATUS));
                        message += ("P41_RES_TRACK_2_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_2_STATUS));
                        message += ("P41_RES_TRACK_3_STATUS: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_3_STATUS));
                        message += ("P41_RES_TRACK_1: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_1));
                        message += ("P41_RES_TRACK_2: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_2));
                        message += ("P41_RES_TRACK_3: " + RBA_API.GetParam(PARAMETER_ID.P41_RES_TRACK_3));
                        string PAN = RBA_API.GetParam(PARAMETER_ID.P41_RES_PAN);
                        if (!string.IsNullOrEmpty(PAN))
                            message += ("P41_RES_PAN: " + PAN);
                        string Masked_PAN = RBA_API.GetParam(PARAMETER_ID.P41_RES_MASKED_PAN);
                        if (!string.IsNullOrEmpty(Masked_PAN))
                            message += ("P41_RES_MASKED_PAN: " + Masked_PAN);
                        string Expiry_Date = RBA_API.GetParam(PARAMETER_ID.P41_RES_EXPIRATION_DATE);
                        if (!string.IsNullOrEmpty(Expiry_Date))
                            message += ("P41_RES_EXPIRATION_DATE: " + Expiry_Date);
                        string Account_Name = RBA_API.GetParam(PARAMETER_ID.P41_RES_ACCOUNT_NAME);
                        if (!string.IsNullOrEmpty(Account_Name))
                            message += ("P41_RES_ACCOUNT_NAME: " + Account_Name);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P41_RES_PAN), message));
                        break;
                    }

                case MESSAGE_ID.M50_AUTHORIZATION:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        // Displaying the parameters received from 50. request
                        message += ("P50_REQ_ACCQUIRING_BANK: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_ACQUIRING_BANK));
                        message += ("P50_REQ_MERCHANT_NUMBER: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_MERCHANT_ID));
                        message += ("P50_REQ_STORE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_STORE_ID));
                        message += ("P50_REQ_PIN_PAD: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_PAD_ID));
                        message += ("P50_REQ_STANDARD_INDUSTRY_CLASSIFICAION: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_STANDARD_INDUSTRY_CLASSIFICATION));
                        message += ("P50_REQ_CURRENCY_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_COUNTRY_OR_CURRENCY_TYPE));
                        message += ("P50_REQ_ZIP_CODE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_ZIP_CODE));
                        message += ("P50_REQ_TIME_ZONE_DIFF_FROM_GMT: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_TIME_ZONE_DIFF_FROM_GMT));
                        message += ("P50_REQ_TRANSACTION_CODE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_TRANSACTION_CODE));
                        string PIN_PAD_Serial_Number = RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_PAD_SERIAL_NUM);
                        message += ("P50_REQ_PIN_PAD_SERIAL_NUM: " + PIN_PAD_Serial_Number);
                        string POS_Transaction_Number = RBA_API.GetParam(PARAMETER_ID.P50_REQ_POS_TRANSACTION_NUM);
                        message += ("P50_REQ_POS_TRANSACTION_NUM: " + POS_Transaction_Number);
                        message += ("P50_REQ_ACC_DATA_SOURCE: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_ACC_DATA_SOURCE));
                        message += ("P50_REQ_MAG_SWIPE_INFO: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_MAG_SWIPE_INFO));
                        message += ("P50_REQ_PIN_LENGTH: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_LENGTH));
                        message += ("P50_REQ_PIN_BLOCK: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_ENCRYPTED_BLOCK));
                        message += ("P50_REQ_PIN_KEY_SET_IDENTIFIER: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_KEY_SET_IDENTIFIER));
                        message += ("P50_REQ_PIN_DEVICE_ID: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_DEVICE_ID));
                        message += ("P50_REQ_PIN_ENCRYPTION_COUNTER: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_ENCRYPTION_COUNTER));
                        message += ("P50_REQ_TRANSACTION_AMOUNT: " + RBA_API.GetParam(PARAMETER_ID.P50_REQ_TRANSACTION_AMOUNT));

                        //Sending a dummy approval message
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_PIN_PAD_SERIAL_NUM, PIN_PAD_Serial_Number);
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_POS_TXN_NUM, POS_Transaction_Number);
                        //if (rbAuthApprove.Checked)
                        //{
                        //    Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_RESPONSE_CODE, "AA");
                        //    Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_PROMPT_INDEX_NUM, "Approved");
                        //}
                        //else
                        //{
                        //    Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_RESPONSE_CODE, "EE");
                        //    Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_PROMPT_INDEX_NUM, "Declined");
                        //}
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_APPROVAL_CODE, "100001");
                        DateTime dtNow = DateTime.Now;
                        string sDate = dtNow.ToString("yyMMdd");
                        Result = RBA_API.SetParam(PARAMETER_ID.P50_RES_TODAYS_DATE_YYMMDD, sDate);
                        Result = RBA_API.ProcessMessage(MESSAGE_ID.M50_AUTHORIZATION);
                        message += ("ProcessMessage 50. " + Result);

                        DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P50_REQ_PIN_DEVICE_ID), message));
                        break;
                    }
                case MESSAGE_ID.M58_DISCOVER_DEVICES:
                    {
                        //not implemented this way - call device health or device info - it will have data
                        //if any other protocol besides Serial / USBCDC - revisit this method
                        //get58ResponseData();
                        break;
                    }

/*                case MESSAGE_ID.M65_RETRIVE_FILE:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        message += ("P65_RES_RESULT: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_RESULT));
                        message += ("P65_RES_TOTAL_NUMBER_OF_BLOCKS: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_TOTAL_NUMBER_OF_BLOKS));
                        message += ("P65_RES_RESULT: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_BLOK_NUMBER));
                        message += ("P65_RES_CRC: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_CRC));
                        message += ("P65_RES_DATA: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_DATA));
                        message += ("P65_RES_DATA_TYPE: " + RBA_API.GetParam(PARAMETER_ID.P65_RES_DATA_TYPE));
                        break;
                    }
*/
                case MESSAGE_ID.M87_E2EE_CARD_READ:
                    {
                        message += ("Unsolicited message: " + msgID + "\n");
                        try
                        {
                            //Removal of FS  - workaround for RBASDK older than 5.3.0
                            int FSint = 28;//0x1C
                            char FSChar = (char)FSint;
                            //
                            // Add this to 23 event handler also
                            string exitType = RBA_API.GetParam(PARAMETER_ID.P87_RES_EXIT_TYPE);
                            message += ("P87_RES_EXIT_TYPE: " + exitType);
                            
                            //check if device canceled
                            if (exitType != "0")
                            {
                                DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), exitType));
                                break;
                            }
                                
                            string cardData = RBA_API.GetParam(PARAMETER_ID.P87_RES_CARD_DATA);
                            //Removal of redundant FS character from cardData  - workaround for RBASDK older than 5.3.0
                            if (!string.IsNullOrEmpty(cardData))
                            {
                                if (cardData.IndexOf(FSChar) != -1)
                                {
                                    cardData = cardData.Replace(FSChar.ToString(), "");
                                }
                            }

                            CardSource = RBA_API.GetParam(PARAMETER_ID.P87_RES_CARD_SOURCE);
                            
                            if (!string.IsNullOrEmpty(CardSource))
                            {
                                message += ("P87_RES_CARD_SOURCE: " + CardSource);
                            }
                            else
                            {
                                message += ("P87_RES_CARD_SOURCE: " + CardSource);
                                message += ("Could not detect card source, assuming source as MSD");
                            }
                            if (!(CardSource == "M" || CardSource == "C" || CardSource == "H"))
                            {
                                DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), message));
                                break;
                            }
                                
                            message += ("P87_RES_CARD_DATA: " + cardData);
                            if (!(cardData.Length > 0))
                            {
                                DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), message));
                                break;
                            }

                            //Parse OnGuard data if it's onGuard encrypted
//                            ParseOnGuardData(cardData, CardSource, out message);
                            //Update account number in 31 request section
//                            string PAN = GetVariable_29("000398");
//                            CardDetails.PAN = GetVariable_29("000398");
//                            CardDetails.EncryptedTrack = string.Format("ONGUARD{0}", cardData);
                            DeviceCardSource = CardSource;
                            //this.Invoke((MethodInvoker)delegate ()
                            //{
                            //    txtPinAccountNumber.Text = PAN;
                            //});
                            DeviceInputReceived.Raise(null, new DeviceEventArgs(msgID, RBA_API.GetParam(PARAMETER_ID.P87_REQ_FORM_NAME), message));
                        }
                        catch (Exception ex)
                        {
                            message += ("Exception occurred" + ex.ToString());
                        }
                        break;
                    }

                case MESSAGE_ID.M95_BARCODE_GET:
                    {
                        //not implemented in this application
                    }
                    break;
            }

        }

        private ERROR_ID SetDeviceCommunications(string port)
        {
            SETTINGS_COMMUNICATION CommSet = new SETTINGS_COMMUNICATION();

            SETTINGS_COMM_TIMEOUTS CommTimeouts;
            uint comm_timeouts;

            comm_timeouts = 5000;

            CommTimeouts.ConnectTimeout = comm_timeouts;
            CommTimeouts.ReceiveTimeout = comm_timeouts;
            CommTimeouts.SendTimeout = comm_timeouts;

            RBA_API.SetCommTimeouts(CommTimeouts);

            CommSet.interface_id = (uint)COMM_INTERFACE.SERIAL_INTERFACE;
            CommSet.rs232_config.ComPort = port;
            CommSet.rs232_config.BaudRate = Convert.ToUInt32(ComBaudRate);
            CommSet.rs232_config.DataBits = Convert.ToUInt32(ComDataBits);
            CommSet.rs232_config.Parity = (uint)0;
            CommSet.rs232_config.StopBits = Convert.ToUInt32(1);
            CommSet.rs232_config.FlowControl = (uint)0;

            //Connect to pin pad
            RBA_SDK.ERROR_ID Result = RBA_API.Connect(CommSet);
            return Result;
        }

        public void Offline()
        {
            ERROR_ID Result = RBA_API.ProcessMessage(MESSAGE_ID.M00_OFFLINE);
        }

        private void SetDeviceToOnDemand()
        {
            //check to see if the device is already in OnDemand
            ERROR_ID Result;
            Result = RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "7");
            Result = RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "15");
            Result = RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);

            string dataValue = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);
            
            //Not Set - so set it
            if (!OnDemandSet || dataValue != "1")
            {
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "7");
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "15");
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "1");
                Result = RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

                OnDemandSet = true;
            }
        }

        public ERROR_ID SoftReset()
        {
            ERROR_ID Result;
            Result = RBA_API.SetParam(PARAMETER_ID.P15_REQ_RESET_TYPE, "9");
            Result = RBA_API.ProcessMessage(MESSAGE_ID.M15_SOFT_RESET);

            return Result;
        }

        public class Health
        {
            public ERROR_ID RESULT { get; set; }
            public string MSR_SWIPES { get; set; }
            public string BAD_TRACK1_READS { get; set; }
            public string BAD_TRACK2_READS { get; set; }
            public string BAD_TRACK3_READS { get; set; }
            public string SIGNATURES { get; set; }
            public string REBOOT { get; set; }
            public string DEVICE_NAME { get; set; }
            public string SERIAL_NUMBER { get; set; }
            public string OS_VERSION { get; set; }
            public string APP_VERSION { get; set; }
            public string SECURITY_LIB_VERSION { get; set; }
            public string EFTL_VERSION { get; set; }
            public string EFTP_VERSION { get; set; }
            public string RAM_SIZE { get; set; }
            public string FLASH_SIZE { get; set; }
            public string MANUFACTURE_DATE { get; set; }
            public string CPEM_TYPE { get; set; }
            public string PEN_STATUS { get; set; }
            public string APP_NAME { get; set; }
            public string MANUFACTURE_ID { get; set; }
            public string DIGITIZER_VERSION { get; set; }
            public string MANUFACTURING_SERIAL_NUMBER { get; set; }

            public Health()
            {
                MSR_SWIPES = string.Empty;
                BAD_TRACK1_READS = string.Empty;
                BAD_TRACK2_READS = string.Empty;
                BAD_TRACK3_READS = string.Empty;
                SIGNATURES = string.Empty;
                REBOOT = string.Empty;
                DEVICE_NAME = string.Empty;
                SERIAL_NUMBER = string.Empty;
                OS_VERSION = string.Empty;
                APP_VERSION = string.Empty;
                SECURITY_LIB_VERSION = string.Empty;
                EFTL_VERSION = string.Empty;
                EFTP_VERSION = string.Empty;
                RAM_SIZE = string.Empty;
                FLASH_SIZE = string.Empty;
                MANUFACTURE_DATE = string.Empty;
                CPEM_TYPE = string.Empty;
                PEN_STATUS = string.Empty;
                APP_NAME = string.Empty;
                MANUFACTURE_ID = string.Empty;
                DIGITIZER_VERSION = string.Empty;
                MANUFACTURING_SERIAL_NUMBER = string.Empty;
            }

            public void GetDeviceHealth()
            {
                RESULT = RBA_API.SetParam(PARAMETER_ID.P08_REQ_REQUEST_TYPE, "0");
                RESULT = RBA_API.ProcessMessage(MESSAGE_ID.M08_HEALTH_STAT);
                MSR_SWIPES = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_MSR_SWIPES);
                BAD_TRACK1_READS = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_BAD_TRACK1_READS);
                BAD_TRACK2_READS = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_BAD_TRACK2_READS);
                BAD_TRACK3_READS = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_BAD_TRACK3_READS);
                SIGNATURES = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_SIGNATURES);
                REBOOT = RBA_API.GetParam(PARAMETER_ID.P08_RES_COUNT_REBOOT);
                DEVICE_NAME = RBA_API.GetParam(PARAMETER_ID.P08_RES_DEVICE_NAME);
                SERIAL_NUMBER = RBA_API.GetParam(PARAMETER_ID.P08_RES_SERIAL_NUMBER);
                OS_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_OS_VERSION);
                APP_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_APP_VERSION);
                SECURITY_LIB_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_SECURITY_LIB_VERSION);
                EFTL_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_EFTL_VERSION);
                EFTP_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_EFTP_VERSION);
                RAM_SIZE = RBA_API.GetParam(PARAMETER_ID.P08_RES_RAM_SIZE);
                FLASH_SIZE = RBA_API.GetParam(PARAMETER_ID.P08_RES_FLASH_SIZE);
                MANUFACTURE_DATE = RBA_API.GetParam(PARAMETER_ID.P08_RES_MANUFACTURE_DATE);
                CPEM_TYPE = RBA_API.GetParam(PARAMETER_ID.P08_RES_CPEM_TYPE);
                PEN_STATUS = RBA_API.GetParam(PARAMETER_ID.P08_RES_PEN_STATUS);
                APP_NAME = RBA_API.GetParam(PARAMETER_ID.P08_RES_APP_NAME);
                MANUFACTURE_ID = RBA_API.GetParam(PARAMETER_ID.P08_RES_MANUFACTURE_ID);
                DIGITIZER_VERSION = RBA_API.GetParam(PARAMETER_ID.P08_RES_DIGITIZER_VERSION);
                MANUFACTURING_SERIAL_NUMBER = RBA_API.GetParam(PARAMETER_ID.P08_RES_MANUFACTURING_SERIAL_NUMBER);
            }
        }

        public class Info
        {
            public ERROR_ID RESULT { get; set; }
            public string MANUFACTURE { get; set; }
            public string DEVICE { get; set; }
            public string UNIT_SERIAL_NUMBER { get; set; }
            public string RAM_SIZE { get; set; }
            public string FLASH_SIZE { get; set; }
            public string DIGITIZER_VERSION { get; set; }
            public string SECURITY_MODULE_VERSION { get; set; }
            public string OS_VERSION { get; set; }
            public string APPLICATION_VERSION { get; set; }
            public string EFTL_VERSION { get; set; }
            public string EFTP_VERSION { get; set; }
            public string MANUFACTURING_SERIAL_NUMBER { get; set; }
            public string EMV_DC_KERNEL_TYPE { get; set; }
            public string EMV_ENGINE_KERNEL_TYPE { get; set; }
            public string CLESS_DISCOVER_KERNEL_TYPE { get; set; }
            public string CLESS_EXPRESSPAY_V3_KERNEL_TYPE { get; set; }
            public string CLESS_EXPRESSPAY_V2_KERNEL_TYPE { get; set; }
            public string CLESS_PAYPASS_V3_KERNEL_TYPE { get; set; }
            public string CLESS_PAYPASS_V3_APP_TYPE { get; set; }
            public string CLESS_VISA_PAYWAVE_KERNEL_TYPE { get; set; }
            public string CLESS_INTERAC_KERNEL_TYPE { get; set; }

            public Info()
            {
                MANUFACTURE = string.Empty;
                DEVICE = string.Empty;
                UNIT_SERIAL_NUMBER = string.Empty;
                RAM_SIZE = string.Empty;
                FLASH_SIZE = string.Empty;
                DIGITIZER_VERSION = string.Empty;
                SECURITY_MODULE_VERSION = string.Empty;
                OS_VERSION = string.Empty;
                APPLICATION_VERSION = string.Empty;
                EFTL_VERSION = string.Empty;
                EFTP_VERSION = string.Empty;
                MANUFACTURING_SERIAL_NUMBER = string.Empty;
                EMV_DC_KERNEL_TYPE = string.Empty;
                EMV_ENGINE_KERNEL_TYPE = string.Empty;
                CLESS_DISCOVER_KERNEL_TYPE = string.Empty;
                CLESS_EXPRESSPAY_V2_KERNEL_TYPE = string.Empty;
                CLESS_EXPRESSPAY_V3_KERNEL_TYPE = string.Empty;
                CLESS_PAYPASS_V3_KERNEL_TYPE = string.Empty;
                CLESS_PAYPASS_V3_APP_TYPE = string.Empty;
                CLESS_VISA_PAYWAVE_KERNEL_TYPE = string.Empty;
                CLESS_INTERAC_KERNEL_TYPE = string.Empty;
            }

            public void GetDeviceInfo()
            {
                // Enable Extended Info
                SetDeviceExtendedInfo(true);

                ERROR_ID RESULT = RBA_API.ProcessMessage(MESSAGE_ID.M07_UNIT_DATA);
                MANUFACTURE =  RBA_API.GetParam(PARAMETER_ID.P07_RES_MANUFACTURE);
                DEVICE = RBA_API.GetParam(PARAMETER_ID.P07_RES_DEVICE);//iMP350
                UNIT_SERIAL_NUMBER =  RBA_API.GetParam(PARAMETER_ID.P07_RES_UNIT_SERIAL_NUMBER);
                RAM_SIZE = RBA_API.GetParam(PARAMETER_ID.P07_RES_RAM_SIZE);
                FLASH_SIZE =  RBA_API.GetParam(PARAMETER_ID.P07_RES_FLASH_SIZE);
                DIGITIZER_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_DIGITIZER_VERSION);
                SECURITY_MODULE_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_SECURITY_MODULE_VERSION);
                OS_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_OS_VERSION);
                APPLICATION_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_APPLICATION_VERSION);
                EFTL_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_EFTL_VERSION);
                EFTP_VERSION = RBA_API.GetParam(PARAMETER_ID.P07_RES_EFTP_VERSION);
                MANUFACTURING_SERIAL_NUMBER = RBA_API.GetParam(PARAMETER_ID.P07_RES_MANUFACTURING_SERIAL_NUMBER);
                EMV_DC_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_EMV_DC_KERNEL_TYPE);
                EMV_ENGINE_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_EMV_ENGINE_KERNEL_TYPE);
                CLESS_DISCOVER_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_DISCOVER_KERNEL_TYPE);
                CLESS_EXPRESSPAY_V3_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_EXPRESSPAY_V3_KERNEL_TYPE);
                CLESS_EXPRESSPAY_V2_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_EXPRESSPAY_V2_KERNEL_TYPE);
                CLESS_PAYPASS_V3_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_PAYPASS_V3_KERNEL_TYPE);
                CLESS_PAYPASS_V3_APP_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_PAYPASS_V3_APP_TYPE);
                CLESS_VISA_PAYWAVE_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_VISA_PAYWAVE_KERNEL_TYPE);
                CLESS_INTERAC_KERNEL_TYPE = RBA_API.GetParam(PARAMETER_ID.P07_RES_CLESS_INTERAC_KERNEL_TYPE);
            }

            public int SetDeviceExtendedInfo(bool enabled)
            {
                ERROR_ID Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "0013");
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "0023");
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, enabled ? "1" : "0");
                Result = RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);
                int result = 0;
                int.TryParse(RBA_API.GetParam(PARAMETER_ID.P60_RES_STATUS), out result);
                return result;
            }
        }
    }
}
