using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JBC_Connect;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace JBCTests
{
    [TestClass]
    public class JBC_Connect_Tests
    {

        static int EVENT_TIMEOUT = 5000;
        static AutoResetEvent _autoResetEvent;
        static long station_id = -1;
        static JBC_API jbc_api;
        static bool transaction_finished = false;
        static CFeaturesData features;
        static int port_count = 0;
        static GenericStationTools generic_tool = GenericStationTools.HT;


        [ClassInitialize]
        public static void initLibrary(TestContext testContext)
        {
            jbc_api = new JBC_Connect.JBC_API();
            _autoResetEvent = new AutoResetEvent(false);
            jbc_api.NewStationConnected += JBC_NewStationConnected;
            jbc_api.StationDisconnected += JBC_StationDisconnected;
            jbc_api.TransactionFinished += JBC_TransactionFinished;
            

            jbc_api.StartSearch();

            _autoResetEvent.WaitOne(EVENT_TIMEOUT);
        }

        [ClassCleanup]
        public static void closeLibrary()
        {
            jbc_api.ResetStation(station_id);
            jbc_api.Close();
        }
        
        [TestInitialize]
        public void SetErrorHandler()
        {
            jbc_api.UserError += JBC_UserError;
        }

        [TestCleanup]
        public void RemoveErrorHandler()
        {
            jbc_api.UserError -= JBC_UserError;
        }

        private static void JBC_NewStationConnected(long stationID)
        {
            station_id = stationID;
            features = jbc_api.GetStationFeatures(station_id);
            port_count = jbc_api.GetPortCount(station_id);

            jbc_api.SetControlMode(station_id, ControlModeConnection.CONTROL);

            _autoResetEvent.Set();
            //Assert.AreEqual("NewStation", "");
        }

        private static void JBC_StationDisconnected(long stationID)
        {
            Debug.WriteLine("DISCO STATION ID:" + stationID);
            Assert.AreEqual("StationDisconnected", "");
        }

        private static void JBC_UserError(long stationID, JBC_Connect.Cerror err)
        {
            Debug.WriteLine("ERR STATION ID:" + stationID + " ERROR MSG: " + err.GetMsg());
            //FIXME There is something in the error manager
            //Assert.AreEqual("ERROR", "");
            if (err.GetCode() != Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED)
            {
                Assert.AreEqual("ERROR", "");
            }

        }

        private static void JBC_UserNotSupported(long stationID, JBC_Connect.Cerror err)
        {
            Debug.WriteLine("ERR STATION ID:" + stationID + " ERROR MSG: " + err.GetMsg());
            jbc_api.UserError -= JBC_UserNotSupported;
            

            if (err.GetCode() != Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED)
            {
                //FIXME: Deal with not supported error
                Assert.AreEqual("ERROR", "");
            }
        }


        private static void JBC_TransactionFinished(long stationID, uint transactionID)
        {
            Debug.WriteLine("TRANS STATION ID:" + stationID);
            transaction_finished = true;
            Assert.AreEqual("TransactionFinished", "");
        }


        private static void SetNotSupportedErrorCallback()
        {
            /*
            jbc_api.UserError -= JBC_UserError;
            jbc_api.UserError += JBC_UserNotSupported;*/

        }

        public void PrintStationFeatures(CFeaturesData features)
        {

            Debug.WriteLine("STATION ID: " + station_id);
            Debug.WriteLine("Alarms: " + features.Alarms);
            Debug.WriteLine("AllToolsSamePortSettings:" + features.AllToolsSamePortSettings);
            Debug.WriteLine("Cartridges:" + features.Cartridges);
            Debug.WriteLine("DelayWithStatus: " + features.DelayWithStatus);
            Debug.WriteLine("DisplaySettings: " + features.DisplaySettings);
            Debug.WriteLine("Ethernet: " + features.Ethernet);
            Debug.WriteLine("FirmwareUpdate: " + features.FirmwareUpdate);
            Debug.WriteLine("MaxTemp: " + features.MaxTemp.UTI);
            Debug.WriteLine("MinTemp: " + features.MinTemp.UTI);
            Debug.WriteLine("MaxPowerLimit: " + features.MaxPowerLimit.ToString());
            Debug.WriteLine("PartialCounters: " + features.PartialCounters);
            Debug.WriteLine("Peripherals: " + features.PartialCounters);
            Debug.WriteLine("Robot: " + features.Robot);
            Debug.WriteLine("SubStations: " + features.SubStations);
            Debug.WriteLine("TempLevelsWithStatus: " + features.TempLevelsWithStatus);
            Debug.WriteLine("TempLevels: " + features.TempLevels);
        }

        // Station Info Methods

        [TestMethod]
        public void GetStationType() {
            eStationType type = jbc_api.GetStationType(station_id);
            Trace.WriteLine("STATION: " + station_id + " TYPE: " + type.ToString());
            Assert.AreNotEqual(eStationType.UNKNOWN, type);
        }

        [TestMethod]
        public void GetStationCOM() {
            String addr = jbc_api.GetStationCOM(station_id);
            bool isValid = true;

            Trace.WriteLine("STATION: " + station_id + " PORT: " + addr);

            IPAddress address;
            if (!IPAddress.TryParse(addr, out address))
            {
                isValid = addr.StartsWith("COM");
            }
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void GetStationConnectionType() {
            String connType = jbc_api.GetStationConnectionType(station_id);
            Trace.WriteLine("STATION: " + station_id + " CONNECTION: " + connType);
            Assert.IsTrue(connType == "U" || connType == "E");
        }

        [TestMethod]
        public void GetStationProtocol() {
            String protocol = jbc_api.GetStationProtocol(station_id);
            Trace.WriteLine("STATION: " + station_id + " PROTOCOL: " + protocol);
            int _;
            Assert.IsTrue(Int32.TryParse(protocol, out _));
        }

        [TestMethod]
        public void GetStationModel() {
            String modelName = jbc_api.GetStationModel(station_id);
            Trace.WriteLine("STATION: " + station_id + " MODEL: " + modelName);
            Assert.AreNotEqual("", modelName);
        }
        [TestMethod]
        public void GetStationModelType() {
            String modelType = jbc_api.GetStationModelType(station_id);
            Trace.WriteLine("STATION: " + station_id + " MODELTYPE: " + modelType);
        }

        [TestMethod]
        public void GetStationModelVersion() {
            int modelVersion = jbc_api.GetStationModelVersion(station_id);
            Trace.WriteLine("STATION: " + station_id + " MODELVERSION: " + modelVersion);
            Assert.IsTrue(modelVersion >= 0);
        }


        [TestMethod]
        public void GetStationHWversion() {
            String version = jbc_api.GetStationHWversion(station_id);
            Trace.WriteLine("STATION: " + station_id + " HWVERSION: " + version);
            int _;
            Assert.AreNotEqual("", version);
        }

        [TestMethod]
        public void GetStationSWversion() {
            String version = jbc_api.GetStationSWversion(station_id);
            Trace.WriteLine("STATION: " + station_id + " SWVERSION: " + version);
            int _;
            Assert.IsTrue(Int32.TryParse(version, out _));
        }
        [TestMethod]
        public void GetPortCount() {
            int port_count = jbc_api.GetPortCount(station_id);
            Trace.WriteLine("PORTS: " + port_count);
            Assert.IsTrue(port_count > 0, "PORT COUNT WAS NOT GREATER 0");
            //TODO: Check code for min port num
        }

        [TestMethod]
        public void GetStationTools() {
            var tools = jbc_api.GetStationTools(station_id);
            Assert.IsNotNull(tools);
            Trace.WriteLine("NUM OF TOOLS: " + tools.Length);
        }

        [TestMethod]
        public void GetStationFeatures() {
            CFeaturesData features = jbc_api.GetStationFeatures(station_id);
            Assert.IsNotNull(features);
        }


        // Station Status Methods
        [TestMethod]
        public void SetControlMode() {
            jbc_api.SetControlMode(station_id, ControlModeConnection.MONITOR);
            jbc_api.SetControlMode(station_id, ControlModeConnection.CONTROL);
        }

        [TestMethod]
        public void GetControlMode() {
            ControlModeConnection control_mode = jbc_api.GetControlMode(station_id);
            Trace.WriteLine("CONTROL MODE: " + control_mode.ToString());
        }

        [TestMethod]
        public void GetRemoteMode() {
            OnOff remote_mode = jbc_api.GetRemoteMode(station_id);
            Trace.WriteLine("REMOTE MODE: " + remote_mode.ToString());
        }

        [TestMethod]
        public void SetRemoteMode() {
            jbc_api.SetRemoteMode(station_id, OnOff._ON);
            jbc_api.SetRemoteMode(station_id, OnOff._OFF);
        }

        [TestMethod]
        public void GetStationError() {
            StationError err = jbc_api.GetStationError(station_id);
            Trace.WriteLine("STATION ERROR: " + err.ToString());
        }

        [TestMethod]
        public void GetStationTransformerTemp() {
            CTemperature temp = jbc_api.GetStationTransformerTemp(station_id);
            Trace.WriteLine("STATION: " + station_id + " TEMP VALID: " + temp.isValid() + " TEMP: " + temp.ToRoundCelsius());
        }


        // Station Data Methods
        [TestMethod]
        public void GetStationName() {
            string name = jbc_api.GetStationName(station_id);
            Trace.WriteLine("STATION: " + station_id + " NAME: " + name);
        }

        [TestMethod]
        public void SetStationName() {
            string name = "Test Name 1";
            jbc_api.SetStationName(station_id, name);
        }

        [TestMethod]
        public void GetStationInternalUID() {
            string uid = jbc_api.GetStationInternalUID(station_id);
            Trace.WriteLine("STATION: " + station_id + " UUID: " + uid);
        }


        private static void JBC_StationIDError(long stationID, JBC_Connect.Cerror err)
        {
            Debug.WriteLine("ERR STATION ID:" + stationID + " Error: " + err.GetMsg());
            jbc_api.UserError -= JBC_StationIDError;

            Assert.AreEqual(Cerror.cErrorCodes.STATION_ID_NOT_FOUND, err.GetCode());
        }

        [TestMethod]
        public void GetStationID() {


            string uid = jbc_api.GetStationInternalUID(station_id);
            long id = jbc_api.GetStationID(uid);
            Debug.WriteLine("STATION ID:" + station_id + "UID: " + uid);

            Assert.AreEqual(station_id, id);

            // Checking the error case
            jbc_api.UserError -= JBC_UserError;
            jbc_api.UserError += JBC_StationIDError;

            string wrong_uid = "wrong_uid";
            long wrong_id = jbc_api.GetStationID(wrong_uid);
        }

        [TestMethod]
        public void GetStationPIN() {
            string pin = jbc_api.GetStationPIN(station_id);
        }

        [TestMethod]
        public void SetStationPIN() {
            string pin = "0000";
            jbc_api.SetStationPIN(station_id, pin);
        }

        [TestMethod]
        public void SetStationPINVerification()
        {
            string pin = "0000";
            jbc_api.SetStationPIN(station_id, pin);
            _autoResetEvent.WaitOne(EVENT_TIMEOUT);

            string new_pin = jbc_api.GetStationPIN(station_id);

            Assert.AreEqual(pin, new_pin);

        }


        [TestMethod]
        public void GetStationPINEnabled() {
            SetNotSupportedErrorCallback();

            OnOff enabled = jbc_api.GetStationPINEnabled(station_id);
            Debug.WriteLine("STATION ID:" + station_id + "ENABLED: " + enabled.ToString());
        }

        [TestMethod]
        public void SetStationPINEnabled() {
            SetNotSupportedErrorCallback();
            OnOff enabled = OnOff._ON;
            jbc_api.SetStationPINEnabled(station_id, enabled);

            SetNotSupportedErrorCallback();
            enabled = OnOff._OFF;
            jbc_api.SetStationPINEnabled(station_id, enabled);
        }

        [TestMethod]
        public void GetStationMaxTemp() {
            CTemperature max_temp = jbc_api.GetStationMaxTemp(station_id);
            Debug.WriteLine("STATION ID:" + station_id + "MAX TEMP: " + max_temp.UTI);
        }

        [TestMethod]
        public void GetStationMinTemp()
        {
            CTemperature min_temp = jbc_api.GetStationMinTemp(station_id);
            Debug.WriteLine("STATION ID:" + station_id + "MIN TEMP: " + min_temp.UTI);
        }

        private static void JBC_StationTempError(long stationID, JBC_Connect.Cerror err)
        {
            Debug.WriteLine("ERR STATION ID:" + stationID + " Error: " + err.GetMsg());
            jbc_api.UserError -= JBC_StationTempError;
            

            Assert.AreEqual(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, err.GetCode());
        }

        [TestMethod]
        public void SetStationMaxTemp() {

            if (features.TempLevels == false)
            {
                Debug.WriteLine("STATION ID:" + station_id + " TEMP LEVELS NOT SUPPORTED");

                jbc_api.UserError -= JBC_UserError;
                jbc_api.UserError += JBC_StationTempError;

                jbc_api.SetStationMaxTemp(station_id, new CTemperature(0));
                return;
            }

            CTemperature max_temp = features.MaxTemp;
            CTemperature min_temp = features.MinTemp;


            Random r = new Random();
            int new_max_temp_uti = r.Next(min_temp.UTI, max_temp.UTI);

            jbc_api.SetStationMaxTemp(station_id, new CTemperature(new_max_temp_uti));
        }

        [TestMethod]
        public void SetStationMaxTempOutOfRange()
        {
            if (features.TempLevels == false)
            {
                return;
            }

            CTemperature max_temp = features.MaxTemp;

            jbc_api.UserError -= JBC_UserError;
            jbc_api.UserError += JBC_StationTempError;

            jbc_api.SetStationMaxTemp(station_id, new CTemperature(max_temp.UTI + 1));
        }



        [TestMethod]
        public void SetStationMinTemp() {

            if (features.TempLevels == false)
            {
                Debug.WriteLine("STATION ID:" + station_id + "TEMP LEVELS NOT SUPPORTED");

                SetNotSupportedErrorCallback();

                jbc_api.SetStationMinTemp(station_id, new CTemperature(0));
                return;
            }

            CTemperature max_temp = features.MaxTemp;
            CTemperature min_temp = features.MinTemp;

            Random r = new Random();
            int new_min_temp_uti = r.Next(min_temp.UTI, max_temp.UTI);

            jbc_api.SetStationMinTemp(station_id, new CTemperature(new_min_temp_uti));

            
            jbc_api.UserError -= JBC_UserError;
            jbc_api.UserError += JBC_StationTempError;

            jbc_api.SetStationMinTemp(station_id, new CTemperature(min_temp.UTI - 1));

        }


        [TestMethod]
        public void GetStationMaxExternalTemp() {
            SetNotSupportedErrorCallback();

            CTemperature max_temp = jbc_api.GetStationMaxExternalTemp(station_id);
            if (max_temp != null)
            {
                Debug.WriteLine("STATION ID:" + station_id + " MAX EXT TEMP: " + max_temp.UTI);
            }
        }

       [TestMethod]
        public void SetStationMaxExternalTemp() {
            SetNotSupportedErrorCallback();
            jbc_api.SetStationMaxExternalTemp(station_id, new CTemperature(2000));
        }

       [TestMethod]
        public void GetStationMinExternalTemp() {
            SetNotSupportedErrorCallback();

            CTemperature min_temp = jbc_api.GetStationMinExternalTemp(station_id);
            if (min_temp != null)
            {
                Debug.WriteLine("STATION ID:" + station_id + " MIN EXT TEMP: " + min_temp.UTI);
            }
        }

       [TestMethod]
        public void SetStationMinExternalTemp() {
            SetNotSupportedErrorCallback();
            jbc_api.SetStationMinExternalTemp(station_id, new CTemperature(2000));
        }

       [TestMethod]
        public void GetStationMaxFlow() {
            int max_flow = jbc_api.GetStationMaxFlow(station_id);
            Debug.WriteLine("STATION ID:" + station_id + " MAX FLOW: " + max_flow);
        }

       [TestMethod]
        public void SetStationMaxFlow() {
            jbc_api.SetStationMaxFlow(station_id, 1000);
        }

       [TestMethod]
        public void GetStationMinFlow() {
            int min_flow = jbc_api.GetStationMinFlow(station_id);
            Debug.WriteLine("STATION ID:" + station_id + " MIN FLOW: " + min_flow);
        }

       [TestMethod]
        public void SetStationMinFlow() {
            jbc_api.SetStationMinFlow(station_id, 1000);
        }

       [TestMethod]
        public void GetStationTempUnits() {
            if (features.DisplaySettings)
            {
                CTemperature.TemperatureUnit unit = jbc_api.GetStationTempUnits(station_id);
                Debug.WriteLine("STATION ID:" + station_id + " TEMP UNITS: " + unit.ToString());
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void SetStationTempUnits() {
            if (features.DisplaySettings)
            {
                jbc_api.SetStationTempUnits(station_id, CTemperature.TemperatureUnit.Fahrenheit);
                jbc_api.SetStationTempUnits(station_id, CTemperature.TemperatureUnit.Celsius);
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void GetStationN2Mode() {
            if (features.DisplaySettings)
            {
                OnOff n2_mode = jbc_api.GetStationN2Mode(station_id);
                Debug.WriteLine("STATION ID:" + station_id + " N2 MODE: " + n2_mode.ToString());
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void SetStationN2Mode() {
            if (features.DisplaySettings)
            {
                jbc_api.SetStationN2Mode(station_id, OnOff._ON);
                jbc_api.SetStationN2Mode(station_id, OnOff._OFF);
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void GetStationHelpText() {
            if (features.DisplaySettings)
            {
                OnOff text = jbc_api.GetStationHelpText(station_id);
                Debug.WriteLine("STATION ID:" + station_id + " HELP TEXT: " + text.ToString());
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void SetStationHelpText() {
            if (features.DisplaySettings)
            {
                jbc_api.SetStationHelpText(station_id, OnOff._ON);
                jbc_api.SetStationHelpText(station_id, OnOff._OFF);
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void GetStationBeep() {
            if (features.DisplaySettings)
            {
                OnOff beep = jbc_api.GetStationBeep(station_id);
                Debug.WriteLine("STATION ID:" + station_id + " BEEP: " + beep.ToString());
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void SetStationBeep() {
            if (features.DisplaySettings)
            {
                jbc_api.SetStationBeep(station_id, OnOff._ON);
                jbc_api.SetStationBeep(station_id, OnOff._OFF);
            }
            else
            {
                Debug.WriteLine("Function not implemented for this model");
            }
        }

       [TestMethod]
        public void GetStationLocked() {
            SetNotSupportedErrorCallback();

            OnOff locked = jbc_api.GetStationLocked(station_id);
            Debug.WriteLine("STATION ID:" + station_id + " LOCKED: " + locked.ToString());
        }

       [TestMethod]
        public void SetStationLocked() {
            SetNotSupportedErrorCallback();
            jbc_api.SetStationLocked(station_id, OnOff._OFF);

            SetNotSupportedErrorCallback();
            jbc_api.SetStationLocked(station_id, OnOff._ON);

        }


        private static void JBC_PortError_Handler(long stationID, JBC_Connect.Cerror err)
        {
            Debug.WriteLine("ERR STATION ID:" + stationID + " ERROR MSG: " + err.GetMsg());

            jbc_api.UserError -= JBC_PortError_Handler;
            

            if (err.GetCode() != Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED && 
                err.GetCode() != Cerror.cErrorCodes.TOOL_NOT_SUPPORTED)
            {
                Assert.AreEqual("ERROR PORT HANDLER", "");
            }


        }

        // Remote Mode and Tool Status Methods 

        private static void PortStatusGetTestHelper(String method_name, object[] method_params = null)
        {
            Assert.IsTrue(station_id >= 0);

            if (port_count > 0)
            {
                jbc_api.UserError -= JBC_UserError;
                jbc_api.UserError += JBC_PortError_Handler;


                Type jbc_api_type = jbc_api.GetType();
                var method = jbc_api_type.GetMethod(method_name);
                Object obj;
                object[] parameters = new object[] { station_id, Port.NUM_1 };

                if (method_params != null)
                {
                    int old_length = parameters.Length;
                    Array.Resize(ref parameters, parameters.Length + method_params.Length);
                    for (int i = 0; i < method_params.Length; i++)
                    {
                        parameters[old_length + i] = method_params[i];
                    }
                }
                obj = method.Invoke(jbc_api, parameters);
                if (obj != null)
                {
                    Debug.WriteLine("STATION ID:" + station_id + " PORT 1 Object: " + obj.ToString());
                }
                else
                {
                    Debug.WriteLine("STATION ID:" + station_id + " PORT 1 Object: NULL");
                }

            }
            else
            {
                Debug.WriteLine("NO PORTS FOR STATION ID:" + station_id);
            }
        }

        private static void PortStatusSetTestOnOffHelper(String method_name)
        {
            Assert.IsTrue(station_id >= 0);

            if (port_count > 0)
            {
                jbc_api.UserError -= JBC_UserError;
                jbc_api.UserError += JBC_PortError_Handler;

                Type jbc_api_type = jbc_api.GetType();
                var method = jbc_api_type.GetMethod(method_name);
                method.Invoke(jbc_api, new object[] { station_id, Port.NUM_1, OnOff._ON });
                method.Invoke(jbc_api, new object[] { station_id, Port.NUM_1, OnOff._OFF });
            }
            else
            {
                Debug.WriteLine("NO PORTS FOR STATION ID:" + station_id);
            }
        }

        private static void PortStatusSetterTestHelper(String get_name, String method_name, object[] method_params = null)
        {
            Assert.IsTrue(station_id >= 0);

            if (port_count > 0)
            {
                jbc_api.UserError -= JBC_UserError;
                jbc_api.UserError += JBC_PortError_Handler;

               Type jbc_api_type = jbc_api.GetType();
                var get_method = jbc_api_type.GetMethod(get_name);
                object[] parameters = new object[] { station_id, Port.NUM_1 };

                if (method_params != null)
                {
                    int old_length = parameters.Length;
                    Array.Resize(ref parameters, parameters.Length + method_params.Length);
                    for (int i = 0; i < method_params.Length; i++)
                    {
                       parameters[old_length + i] = method_params[i];
                    }
                }

                var output = get_method.Invoke(jbc_api, parameters);
                Array.Resize(ref parameters, parameters.Length + 1);
                parameters[parameters.Length - 1] = output;

                var method = jbc_api_type.GetMethod(method_name);
                method.Invoke(jbc_api, parameters);
            }
            else
            {
                Debug.WriteLine("NO PORTS FOR STATION ID:" + station_id);
            }
        }

        [TestMethod]
        public void GetPortToolStandStatus() {
            PortStatusGetTestHelper("GetPortToolStandStatus");
        }

        [TestMethod]
        public void GetPortToolSleepStatus() {
            PortStatusGetTestHelper("GetPortToolSleepStatus");
        }
        
       [TestMethod]
        public void GetPortToolHibernationStatus() {
            PortStatusGetTestHelper("GetPortToolHibernationStatus");
        }

       [TestMethod]
        public void GetPortToolExtractorStatus() {
            PortStatusGetTestHelper("GetPortToolExtractorStatus");
        }

       [TestMethod]
        public void GetPortToolDesolderStatus() {
            PortStatusGetTestHelper("GetPortToolDesolderStatus");
        }

       [TestMethod]
        public void GetPortToolPedalStatus() {
            PortStatusGetTestHelper("GetPortToolPedalStatus");
        }

       [TestMethod]
        public void GetPortToolPedalConnectedStatus() {
            PortStatusGetTestHelper("GetPortToolPedalConnectedStatus");
        }

       [TestMethod]
        public void GetPortToolSuctionRequestedStatus() {
            PortStatusGetTestHelper("GetPortToolSuctionRequestedStatus");
        }

       [TestMethod]
        public void GetPortToolSuctionStatus() {
            PortStatusGetTestHelper("GetPortToolSuctionStatus");
        }

       [TestMethod]
        public void GetPortToolHeaterRequestedStatus() {
            PortStatusGetTestHelper("GetPortToolHeaterRequestedStatus");
        }

       [TestMethod]
        public void GetPortToolHeaterStatus() {
            PortStatusGetTestHelper("GetPortToolHeaterStatus");
        }

       [TestMethod]
        public void GetPortToolCoolingStatus() {
            PortStatusGetTestHelper("GetPortToolCoolingStatus");
        }

       [TestMethod]
        public void GetPortToolTimeToStopStatus() {
            Assert.IsTrue(station_id >= 0);

            if (port_count > 0)
            {
                int time = jbc_api.GetPortToolTimeToStopStatus(station_id, Port.NUM_1);
                Debug.WriteLine("STATION ID:" + station_id + " PORT 1 TIME: " + time);
 
            }
            else
            {
                Debug.WriteLine("NO PORTS FOR STATION ID:" + station_id);
            }
        }

        [TestMethod]
        public void SetPortToolStandStatus() {
            PortStatusSetTestOnOffHelper("SetPortToolStandStatus");
        }

       [TestMethod]
        public void SetPortToolExtractorStatus() {
            PortStatusSetTestOnOffHelper("SetPortToolExtractorStatus");
        }

        [TestMethod]
        public void SetPortToolDesolderStatus() {
            PortStatusSetTestOnOffHelper("SetPortToolDesolderStatus");
        }

        [TestMethod]
        public void SetPortToolHeaterStatus() {
            PortStatusSetTestOnOffHelper("SetPortToolHeaterStatus");

        }

        [TestMethod]
        public void SetPortToolSuctionStatus() {
            PortStatusSetTestOnOffHelper("SetPortToolSuctionStatus");
        }



        // Port and Tool Info Methods
        [TestMethod]
        public void GetPortToolID() {
            PortStatusGetTestHelper("GetPortToolID");
        }

       [TestMethod]
        public void GetPortToolActualTemp() {
            PortStatusGetTestHelper("GetPortToolActualTemp");
        }

       [TestMethod]
        public void GetPortToolActualPower() {
            PortStatusGetTestHelper("GetPortToolActualPower");
        }
        
       [TestMethod]
        public void GetPortToolActualFlow() {
            PortStatusGetTestHelper("GetPortToolActualFlow");
        }

       [TestMethod]
        public void GetPortToolActualExternalTemp() {
            PortStatusGetTestHelper("GetPortToolActualExternalTemp");
        }

       [TestMethod]
        public void GetPortToolProtectionTCTemp() {
            PortStatusGetTestHelper("GetPortToolProtectionTCTemp");
        }

       [TestMethod]
        public void GetPortToolError() {
            PortStatusGetTestHelper("GetPortToolError");
        }

       [TestMethod]
        public void GetPortToolMOStemp() {
            PortStatusGetTestHelper("GetPortToolMOStemp");
        }

       [TestMethod]
        public void GetPortToolFutureMode() {
            PortStatusGetTestHelper("GetPortToolFutureMode");
        }

       [TestMethod]
        public void GetPortToolTimeToFutureMode() {
            PortStatusGetTestHelper("GetPortToolTimeToFutureMode");
        }


      // Port and Tool Data Methods
        [TestMethod]
        public void GetPortToolSelectedTemp() {
            PortStatusGetTestHelper("GetPortToolSelectedTemp");
        }

        [TestMethod]
        public void SetPortToolSelectedTemp() {
            PortStatusSetterTestHelper("GetPortToolSelectedTemp", "SetPortToolSelectedTemp");
        }

        [TestMethod]
        public void GetPortToolSelectedFlow() {
            PortStatusGetTestHelper("GetPortToolSelectedFlow");
        }

        [TestMethod]
        public void SetPortToolSelectedFlow() {
            PortStatusSetterTestHelper("GetPortToolSelectedFlow", "SetPortToolSelectedFlow");
        }

        [TestMethod]
        public void GetPortToolSelectedExternalTemp() {
            PortStatusGetTestHelper("GetPortToolSelectedExternalTemp");
        }

        [TestMethod]
        public void SetPortToolSelectedExternalTemp() {
            PortStatusSetterTestHelper("GetPortToolSelectedExternalTemp", "SetPortToolSelectedExternalTemp");
        }

        [TestMethod]
        public void GetPortWorkMode() {
            PortStatusGetTestHelper("GetPortWorkMode");
        }

        [TestMethod]
        public void SetPortWorkMode() {
            PortStatusSetterTestHelper("GetPortWorkMode", "SetPortWorkMode");
        }

        [TestMethod]
        public void GetPortToolFixTemp() {            
            PortStatusGetTestHelper("GetPortToolFixTemp", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolFixTemp() {

            Assert.IsTrue(station_id >= 0);

            if (port_count > 0)
            {
                jbc_api.UserError -= JBC_UserError;
                jbc_api.UserError += JBC_PortError_Handler;

                CTemperature temp = jbc_api.GetPortToolFixTemp(station_id, Port.NUM_1, generic_tool);
                jbc_api.SetPortToolFixTemp(station_id, Port.NUM_1, generic_tool, temp);

            }
            else
            {
                Debug.WriteLine("NO PORTS FOR STATION ID:" + station_id);
            }        
        }

        [TestMethod]
        public void GetPortToolSelectedTempLevels() {
            PortStatusGetTestHelper("GetPortToolSelectedTempLevels", new object[] { generic_tool });
        }

        [TestMethod]
        public void GetPortToolSelectedTempLevelsEnabled() {
            PortStatusGetTestHelper("GetPortToolSelectedTempLevelsEnabled", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolSelectedTempLevels() {
            PortStatusSetterTestHelper("GetPortToolSelectedTempLevels",
                "SetPortToolSelectedTempLevels", new object[] { generic_tool });
        }
        [TestMethod]
        public void SetPortToolSelectedTempLevelsEnabled() {
            PortStatusSetterTestHelper("GetPortToolSelectedTempLevelsEnabled",
                "SetPortToolSelectedTempLevelsEnabled", new object[] { generic_tool });
        }
        [TestMethod]
        public void GetPortToolTempLevel() {
            PortStatusGetTestHelper("GetPortToolTempLevel", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void SetPortToolTempLevel() {
            PortStatusSetterTestHelper("GetPortToolTempLevel",
                "SetPortToolTempLevel", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void GetPortToolFlowLevel() {
            PortStatusGetTestHelper("GetPortToolFlowLevel", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void SetPortToolFlowLevel() {
            PortStatusSetterTestHelper("GetPortToolFlowLevel",
                "SetPortToolFlowLevel", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void GetPortToolExternalTempLevel() {
            PortStatusGetTestHelper("GetPortToolExternalTempLevel", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void SetPortToolExternalTempLevel() {
            PortStatusSetterTestHelper("GetPortToolExternalTempLevel",
                    "SetPortToolExternalTempLevel", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void GetPortToolTempLevelEnabled() {
            PortStatusGetTestHelper("GetPortToolTempLevelEnabled", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void SetPortToolTempLevelEnabled() {
            PortStatusSetterTestHelper("GetPortToolTempLevelEnabled",
                    "SetPortToolTempLevelEnabled", new object[] { generic_tool, ToolTemperatureLevels.FIRST_LEVEL });
        }

        [TestMethod]
        public void SetPortToolLevels() {
            jbc_api.UserError -= JBC_UserError;
            jbc_api.UserError += JBC_PortError_Handler;

            jbc_api.SetPortToolLevels(station_id,
                                       Port.NUM_1,
                                       generic_tool,
                                       OnOff._OFF,
                                       ToolTemperatureLevels.FIRST_LEVEL,
                                       OnOff._OFF,
                                       features.MinTemp,
                                       OnOff._OFF,
                                       features.MinTemp,
                                       OnOff._OFF,
                                       features.MinTemp);

        }

        [TestMethod]
        public void SetPortToolLevels_HA() {
            jbc_api.UserError -= JBC_UserError;
            jbc_api.UserError += JBC_PortError_Handler;

            jbc_api.SetPortToolLevels_HA(station_id,
                                        Port.NUM_1,
                                        generic_tool,
                                        OnOff._OFF,
                                        ToolTemperatureLevels.FIRST_LEVEL,
                                        OnOff._OFF,
                                        features.MinTemp,
                                        0,
                                        features.MinTemp,
                                        OnOff._OFF,
                                        features.MinTemp,
                                        0,
                                        features.MinTemp,
                                        OnOff._OFF,
                                        features.MinTemp,
                                        0,
                                        features.MinTemp);

           /// Assert.AreEqual("TestNotImplementedError", "");
        }

        [TestMethod]
        public void GetPortToolSleepDelay() {
            PortStatusGetTestHelper("GetPortToolSleepDelay", new object[] { generic_tool });
        }

        [TestMethod]
        public void GetPortToolSleepDelayEnabled() {
            PortStatusGetTestHelper("GetPortToolSleepDelayEnabled", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolSleepDelay() {
            PortStatusSetterTestHelper("GetPortToolSleepDelay",
                "SetPortToolSleepDelay", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolSleepDelayEnabled() {
            PortStatusSetterTestHelper("GetPortToolSleepDelayEnabled",
                "SetPortToolSleepDelayEnabled", new object[] {  generic_tool });
        }

        [TestMethod]
        public void GetPortToolSleepTemp() {
            PortStatusGetTestHelper("GetPortToolSleepTemp", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolSleepTemp() {
            PortStatusSetterTestHelper("GetPortToolSleepTemp",
                "SetPortToolSleepTemp", new object[] { generic_tool });
        }

        [TestMethod]
        public void GetPortToolHibernationDelay() {
            PortStatusGetTestHelper("GetPortToolHibernationDelay", new object[] { generic_tool });
        }

        [TestMethod]
        public void GetPortToolHibernationDelayEnabled() {
            PortStatusGetTestHelper("GetPortToolHibernationDelayEnabled", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolHibernationDelay() {
            PortStatusSetterTestHelper("GetPortToolHibernationDelay",
                "SetPortToolHibernationDelay", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolHibernationDelayEnabled() {
            PortStatusSetterTestHelper("GetPortToolHibernationDelayEnabled",
                "SetPortToolHibernationDelayEnabled", new object[] { generic_tool });
        }

        [TestMethod]
        public void GetPortToolAdjustTemp() {
            PortStatusGetTestHelper("GetPortToolAdjustTemp", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolAdjustTemp() {
            PortStatusSetterTestHelper("GetPortToolAdjustTemp",
                "SetPortToolAdjustTemp", new object[] { generic_tool });
        }

        [TestMethod]
        public void GetPortToolTimeToStop() {
            PortStatusGetTestHelper("GetPortToolTimeToStop", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolTimeToStop() {
            PortStatusSetterTestHelper("GetPortToolTimeToStop",
                "SetPortToolTimeToStop", new object[] { generic_tool });
        }

        [TestMethod]
        public void GetPortToolStartMode() {
            PortStatusGetTestHelper("GetPortToolStartMode", new object[] { generic_tool });
        }

        [TestMethod]
        public void SetPortToolStartMode() {
            PortStatusSetterTestHelper("GetPortToolStartMode",
                "SetPortToolStartMode", new object[] { generic_tool });
        }

        [TestMethod]
        public void ResetPortToolSettings() {
            PortStatusGetTestHelper("ResetPortToolSettings", new object[] { generic_tool });
        }


        // Counter Methods

        private static void CounterTestHelper(String method_name)
        {
            Type jbc_api_type = jbc_api.GetType();
            var method = jbc_api_type.GetMethod(method_name);
            Object obj0 = method.Invoke(jbc_api, new object[] { station_id, Port.NUM_1, CounterTypes.GLOBAL_COUNTER });
            Debug.WriteLine("STATION ID:" + station_id + " PORT 1 GLOBAL COUNTER: " + obj0.ToString());
            Object obj1 = method.Invoke(jbc_api, new object[] { station_id, Port.NUM_1, CounterTypes.PARTIAL_COUNTER });
            Debug.WriteLine("STATION ID:" + station_id + " PORT 1 PARTIAL COUNTER: " + obj1.ToString());
        }

        [TestMethod]
        public void GetStationPluggedMinutes() {
            int counter0 = jbc_api.GetStationPluggedMinutes(station_id);
            Debug.WriteLine("STATION ID:" + station_id + " GLOBAL COUNTER: " + counter0);

            int counter1 = jbc_api.GetStationPluggedMinutes(station_id, CounterTypes.PARTIAL_COUNTER);
            Debug.WriteLine("STATION ID:" + station_id + " PARTIAL COUNTER: " + counter1);
        }

        [TestMethod]
        public void GetPortToolWorkMinutes() {
            CounterTestHelper("GetPortToolWorkMinutes");
        }
        [TestMethod]
        public void GetPortToolSleepMinutes() {
            CounterTestHelper("GetPortToolSleepMinutes");
        }
        [TestMethod]
        public void GetPortToolHibernationMinutes() {
            CounterTestHelper("GetPortToolHibernationMinutes");
        }
        [TestMethod]
        public void GetPortToolIdleMinutes() {
            CounterTestHelper("GetPortToolIdleMinutes");
        }
        [TestMethod]
        public void GetPortToolSleepCycles() {
            CounterTestHelper("GetPortToolSleepCycles");
        }
        [TestMethod]
        public void GetPortToolDesolderCycles() {
            CounterTestHelper("GetPortToolDesolderCycles");
        }
        [TestMethod]
        public void GetPortToolWorkCycles() {
            CounterTestHelper("GetPortToolWorkCycles");
        }
        [TestMethod]
        public void GetPortToolSuctionCycles() {
            CounterTestHelper("GetPortToolSuctionCycles");
        }
        [TestMethod]
        public void ResetPortToolStationPartialCounters() {
            jbc_api.ResetPortToolStationPartialCounters(station_id, Port.NUM_1);
        }

        // Continuous Mode Methods
        private static void JBC_Continues_Error_Handler1(long stationID, JBC_Connect.Cerror err)
        {
            Debug.WriteLine("ERR STATION ID:" + stationID +" ERROR CODE: "+ err.GetCode() + " ERROR MSG: " + err.GetMsg());
            jbc_api.UserError -= JBC_Continues_Error_Handler1;
            

            if (err.GetCode() != Cerror.cErrorCodes.CONTINUOUS_MODE_ON_WITHOUT_PORTS)
            {
                //FIXME: Deal with not supported error
                Assert.AreEqual("ERROR", "");
            }
        }

        [TestMethod]
        public void SetContinuousMode() {
            jbc_api.UserError -= JBC_UserError;
            jbc_api.UserError += JBC_Continues_Error_Handler1;
            jbc_api.SetContinuousMode(station_id, SpeedContinuousMode.T_1000mS);
            jbc_api.SetContinuousMode(station_id, SpeedContinuousMode.T_1000mS, portA: Port.NUM_1);
        }

        private static void JBC_Continues_Error_Handler2(long stationID, JBC_Connect.Cerror err)
        {
            Debug.WriteLine("ERR STATION ID:" + stationID + " ERROR CODE: " + err.GetCode() + " ERROR MSG: " + err.GetMsg());
            jbc_api.UserError -= JBC_Continues_Error_Handler2;
            

            if (err.GetCode() != Cerror.cErrorCodes.COMMUNICATION_ERROR)
            {
                Assert.AreEqual("ERROR", "");
            }
        }

        [TestMethod]
        public void StartAndStopContinuousMode() {
            uint queue_id = jbc_api.StartContinuousMode(station_id);
            Debug.WriteLine("STATION ID:" + station_id + " QUEUE CODE: " + queue_id);

            jbc_api.StopContinuousMode(station_id, queue_id);
            jbc_api.StopContinuousMode(station_id, queue_id + 100);
        }

        [TestMethod]
        public void GetContinuousMode() {
            CContinuousModeStatus status0 = jbc_api.GetContinuousMode(station_id);
            Debug.WriteLine("STATION ID:" + station_id + " PORT 1: " + status0.port1 + " SPEED: " + status0.speed);

            jbc_api.SetContinuousMode(station_id, SpeedContinuousMode.T_1000mS, portA: Port.NUM_1);
            CContinuousModeStatus status1 = jbc_api.GetContinuousMode(station_id);
            Debug.WriteLine("STATION ID:" + station_id + " PORT 1: " + status1.port1 + " SPEED: " + status1.speed);

        }

        [TestMethod]
        public void GetContinuousModeDeliverySpeed() {
            uint queue_id = jbc_api.StartContinuousMode(station_id);

            SpeedContinuousMode speed = jbc_api.GetContinuousModeDeliverySpeed(station_id, queue_id);
            Debug.WriteLine("STATION ID:" + station_id + " SPEED: " + speed.ToString());

            jbc_api.StopContinuousMode(station_id, queue_id);
        }

        [TestMethod]
        public void GetContinuousModeDataCount() {
            uint queue_id = jbc_api.StartContinuousMode(station_id);

            int count = jbc_api.GetContinuousModeDataCount(station_id, queue_id);

            Debug.WriteLine("STATION ID:" + station_id + " COUNT: " + count);

            jbc_api.StopContinuousMode(station_id, queue_id);
        }
        [TestMethod]
        public void GetContinuousModeNextData() {
            uint queue_id = jbc_api.StartContinuousMode(station_id);

            int count = jbc_api.GetContinuousModeDataCount(station_id, queue_id);

            stContinuousModeData data = jbc_api.GetContinuousModeNextData(station_id, queue_id);

            Debug.WriteLine("STATION ID:" + station_id + " DATA: " + data.data);

            jbc_api.StopContinuousMode(station_id, queue_id);
        }

        [TestMethod]
        public void GetContinuousModeNextData_HA() {
            uint queue_id = jbc_api.StartContinuousMode(station_id);

            int count = jbc_api.GetContinuousModeDataCount(station_id, queue_id);

            stContinuousModeData_HA data = jbc_api.GetContinuousModeNextData_HA(station_id, queue_id);

            Debug.WriteLine("STATION ID:" + station_id + " DATA: " + data.data);

            jbc_api.StopContinuousMode(station_id, queue_id);
        }

// Miscelaneous Methods
        [TestMethod]
        public void DefaultStationParameters() {
            jbc_api.DefaultStationParameters(station_id);
        }

        [TestMethod]
        public void SetTransaction() {
            uint transaction = jbc_api.SetTransaction(station_id);
        }

        [TestMethod]
        public void QueryTransaction()
        {
            uint transaction = jbc_api.SetTransaction(station_id);
            bool finished0 = jbc_api.QueryTransaction(station_id, transaction);
            bool finished1 = jbc_api.QueryTransaction(station_id, transaction + 1000);
            Debug.WriteLine("STATION ID: " + station_id + " TRANSACTION ID :" + transaction + " FINISHED 0: " + finished0 + " FINISHED 1:" + finished1);
        }

    }

}
