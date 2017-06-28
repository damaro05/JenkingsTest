// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
// End of VB project level imports

namespace RemoteManRegister
{
    sealed class ManRegGlobal
    {

        // for station lists
        public class cListStation
        {
            private string myID;
            private string myName;
            private string myModel;
            private string myText;

            public cListStation(string strID, string strName, string strModel)
            {
                this.myID = strID;
                this.myName = strName;
                this.myModel = strModel;
                this.myText = strModel + " - " + strName + " (" + strID + ")";
            } //New

            public string ID
            {
                get
                {
                    return myID;
                }
            }

            public string Name
            {
                get
                {
                    return myName;
                }
            }

            public string Model
            {
                get
                {
                    return myModel;
                }
            }

            public string Text
            {
                get
                {
                    return myText;
                }
            }

        } //cListStation

        // for connected station lists
        public class cConnectedStation
        {
            private string myID;
            private int myFrmPlottingID;

            public cConnectedStation(string strID, int frmID)
            {
                this.myID = strID;
                this.myFrmPlottingID = frmID;
            } //New

            public string ID
            {
                get
                {
                    return myID;
                }
            }

            public int frmPlottingID
            {
                get
                {
                    return myFrmPlottingID;
                }
                set
                {
                    myFrmPlottingID = value;
                }
            }

        } //cConnectedStation

        internal static List<cConnectedStation> connectedStations = new List<cConnectedStation>();

        internal static System.String templateExtension = "tpt";

        internal static string tempunitCELSIUS = Strings.Chr(176).ToString() + "C";
        internal static string tempunitFAHRENHEIT = Strings.Chr(176).ToString() + "F";
        internal static string tempunitUTI = "UTI";
        internal static string CELSIUS_TEXT = "ºC";
        internal static string FAHRENHEIT_TEXT = "ºF";

        internal static string sDatetimeForLBRFormat = "yyyy/MM/dd - HH:mm:ss";

        // Resource texts
        internal static System.String regDataLostId = "regDataLost";
        internal static System.String gralDoneId = "gralDone";
        internal static System.String gralWarningId = "gralWarning";
        internal static System.String gralTemperatureId = "gralTemperature";
        internal static System.String gralPowerId = "gralPower";
        internal static System.String gralFlowId = "gralFlow";
        internal static System.String regCannotCreateFileId = "regCannotCreateFile";
        internal static System.String regCannotOpenFileId = "regCannotOpenFile";
        internal static System.String regNotLBRFileId = "regNotLBRFile";
        internal static System.String regPlottingInAnotherWindowID = "regPlottingInAnotherWindow";
        internal static System.String regSecondsPerTickId = "regSecondsPerTick";
        internal static System.String regCoordTimeId = "regCoordTime";
        internal static System.String filterCommaSeparatedId = "filterCommaSeparated";
        internal static System.String filterLabRegisterId = "filterLabRegister";

        internal static System.String regMnuFileId = "regMnuFile";
        internal static System.String regMnuFileNewId = "regMnuFileNew";
        internal static System.String regMnuFileOpenId = "regMnuFileOpen";
        internal static System.String regMnuFileSaveAsId = "regMnuFileSaveAs";
        internal static System.String regMnuFilePrintId = "regMnuFilePrint";
        internal static System.String regMnuFileExportToCSVId = "regMnuFileExportToCSV";

        internal static System.String regMnuConfigId = "regMnuConfig";
        internal static System.String regMnuConfigWizardId = "regMnuConfigWizard";
        internal static System.String regMnuConfigSeriesId = "regMnuConfigSeries";
        internal static System.String regMnuConfigAxisId = "regMnuConfigAxis";
        internal static System.String regMnuConfigOptionsId = "regMnuConfigOptions";
        internal static System.String regMnuConfigTitleId = "regMnuConfigTitle";
        internal static System.String regMnuConfigTemplatesId = "regMnuConfigTemplates";
        internal static System.String regMnuConfigTemplatesLoadId = "regMnuConfigTemplatesLoad";
        internal static System.String regMnuConfigTemplatesSaveId = "regMnuConfigTemplatesSave";

        internal static System.String regStripCoordinatesId = "regStripCoordinates";
        internal static System.String regStripZoomId = "regStripZoom";
        internal static System.String regStripDefaultZoomId = "regStripDefaultZoom";
        internal static System.String regStripTriggerId = "regStripTrigger";
        internal static System.String regStripTriggerAutoId = "regStripTriggerAuto";
        internal static System.String regStripTriggerSingleId = "regStripTriggerSingle";
        internal static System.String regStripTriggerManualId = "regStripTriggerManual";
        internal static System.String regStripResetTriggerId = "regStripResetTrigger";
        internal static System.String regStripPlayId = "regStripPlay";
        internal static System.String regStripPauseId = "regStripPause";
        internal static System.String regStripStopId = "regStripStop";
        internal static System.String regStripRecordId = "regStripRecord";
        internal static System.String regStripStatusId = "regStripStatus";
        internal static System.String regStripStatusPlayId = "regStripStatusPlay";
        internal static System.String regStripStatusPauseId = "regStripStatusPause";
        internal static System.String regStripStatusStopId = "regStripStatusStop";
        internal static System.String regStripStatusRecordId = "regStripStatusRecord";

        internal static System.String regSeriesSerieStationNoNameId = "regSeriesSerieStationNoName";

        internal static System.String regSeriesListOfSeriesId = "regSeriesListOfSeries";
        internal static System.String regSeriesSerieDataId = "regSeriesSerieData";
        internal static System.String regSeriesSerieNameId = "regSeriesSerieName";
        internal static System.String regSeriesSerieStationId = "regSeriesSerieStation";
        internal static System.String regSeriesSeriePortId = "regSeriesSeriePort";
        internal static System.String regSeriesSerieMagnitudeId = "regSeriesSerieMagnitude";
        internal static System.String regSeriesSerieColorId = "regSeriesSerieColor";
        internal static System.String regSeriesSerieColorSelectId = "regSeriesSerieColorSelect";
        internal static System.String regSeriesSerieShowPointsId = "regSeriesSerieShowPoints";
        internal static System.String regSeriesSerieShowLineId = "regSeriesSerieShowLine";
        internal static System.String regSeriesSeriePointsId = "regSeriesSeriePoints";
        internal static System.String regSeriesSerieLineId = "regSeriesSerieLine";
        internal static System.String regSeriesCannotSelectMagnitudeId = "regSeriesCannotSelectMagnitude";

        internal static System.String regAxisGridGridStepId = "regAxisGridGridStep";
        internal static System.String regAxisGridTimeWindowId = "regAxisGridTimeWindow";
        internal static System.String regAxisGridSecondsId = "regAxisGridSeconds";
        internal static System.String regAxisGridAxisId = "regAxisGridAxis";
        internal static System.String regAxisGridGridId = "regAxisGridGrid";

        internal static System.String regOptColorsId = "regOptColors";
        internal static System.String regOptStartSideId = "regOptStartSide";
        internal static System.String regOptStartSideLeftId = "regOptStartSideLeft";
        internal static System.String regOptStartSideRightId = "regOptStartSideRight";
        internal static System.String regOptTriggerAutoInfoId = "regOptTriggerAutoInfo";
        internal static System.String regOptTriggerSingleInfoId = "regOptTriggerSingleInfo";
        internal static System.String regOptTriggerManualInfoId = "regOptTriggerManualInfo";
        internal static System.String regOptClrTempAxisId = "regOptClrTempAxis";
        internal static System.String regOptClrPowerAxisId = "regOptClrPowerAxis";
        internal static System.String regOptClrTimeAxisId = "regOptClrTimeAxis";
        internal static System.String regOptClrGridDivId = "regOptClrGridDiv";
        internal static System.String regOptClrSeriesTextId = "regOptClrSeriesText";
        internal static System.String regOptClrBackgroundId = "regOptClrBackground";
        internal static System.String regOptClrTitleId = "regOptClrTitle";
        internal static System.String regOptLinesAndPointsId = "regOptLinesAndPoints";
        internal static System.String regOptLineWidthId = "regOptLineWidth";
        internal static System.String regOptPointWidthId = "regOptPointWidth";
        internal static System.String regOptClrDefaultsId = "regOptClrDefaults";

        internal static System.String regEnterPlotTitleId = "regEnterPlotTitle";

        internal static System.String regTemplateId = "regTemplate";
        internal static System.String regTemplateDefaultNameId = "regTemplateDefaultName";
        internal static System.String regTemplateEnterNameId = "regTemplateEnterName";

        internal static System.String regTemplateListId = "regTemplateList";
        internal static System.String regTemplateParamsId = "regTemplateParams";
        internal static System.String regTemplateSeriesDataId = "regTemplateSeriesData";
        internal static System.String regTemplateTriggerId = "regTemplateTrigger";

        internal static System.String regButOkId = "regButOk";
        internal static System.String regButCancelId = "regButCancel";
        internal static System.String regAddId = "regAdd";
        internal static System.String regModifyId = "regModify";
        internal static System.String regRemoveId = "regRemove";
        internal static System.String regTimeId = "regTime";
        internal static System.String regPlotTitleId = "regPlotTitle";
        internal static System.String regNextId = "regNext";
        internal static System.String regPreviousId = "regPrevious";
        internal static System.String regFinishId = "regFinish";

        internal static System.String regSeriesMagnitudeTempId = "regSeriesMagnitudeTemp";
        internal static System.String regSeriesMagnitudePowerId = "regSeriesMagnitudePower";
        internal static System.String regSeriesMagnitudeFlowId = "regSeriesMagnitudeFlow";

        internal static System.String regWiz1InfoId = "regWiz1Info";
        internal static System.String regWiz2InfoId = "regWiz2Info";
        internal static System.String regWizardSeriesAndTitleId = "regWizardSeriesAndTitle";
        internal static System.String regWizard2AxisRangeId = "regWizard2AxisRange";

        internal static System.String regMsgGeneratingFileId = "regMsgGeneratingFile";
        internal static System.String regMsgGeneratingExtId = "regMsgGeneratingExt";
        internal static System.String regMsgLoadingFileId = "regMsgLoadingFile";
        internal static System.String regMsgLoadingExtId = "regMsgLoadingExt";
        internal static System.String regMsgExtNotValidId = "regMsgExtNotValid";

        #region General Routines
        // Localization routines

        // culture is culture of exe
        //Friend Function changeCulture(ByVal sCultureName As String) As Boolean
        //    changeCulture = True
        //    curCulture = sCultureName
        //    If sCultureName = "en" Then sCultureName = "" ' neutral = English
        //    My.Application.ChangeUICulture(sCultureName)
        //End Function

        // others routines
        internal static bool myControlExists(Form frm, string sControlName, ref System.Windows.Forms.Control myControl)
        {
            bool returnValue = false;
            System.Windows.Forms.Control[] aControls = frm.Controls.Find(sControlName, true);
            returnValue = false;
            if (aControls.Length > 0)
            {
                myControl = aControls[0];
                returnValue = true;
            }
            return returnValue;
        }

        internal static string getTriggerText(Cplot.cTrigger pTrigger)
        {
            switch (pTrigger)
            {
                case Cplot.cTrigger.TRG_AUTO:
                    return Localization.getResStr(regStripTriggerAutoId);
                case Cplot.cTrigger.TRG_MANUAL:
                    return Localization.getResStr(regStripTriggerManualId);
                case Cplot.cTrigger.TRG_SINGLE:
                    return Localization.getResStr(regStripTriggerSingleId);
                default:
                    return Localization.getResStr(regStripTriggerManualId);
            }
        }

        #endregion

        internal static int lastError = int.MaxValue;
        internal static bool errorCatch = false;

        internal static bool checkAPIerror()
        {
            if (errorCatch)
            {
                errorCatch = false;
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
