// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

//using JBC_ConnectRemote.JBC_API_Remote;
using Microsoft.VisualBasic.CompilerServices;
using DataJBC;


namespace RemoteManager
{
    public partial class frmCartridges
    {

        private struct strImgs
        {
            public string sFilename;
            public string sCartridgeModel;
            public int iCartridgeNbr;
        }


        private List<strImgs> listImgs = new List<strImgs>();
        private GenericStationTools tool = GenericStationTools.NO_TOOL;
        private string sStationModel;
        private string sCartridgeModel = "";
        private int iCurrIdx = 0;
        private int iSelectedCartridge = 0;


        public frmCartridges(GenericStationTools pTool, int pSelectedCartridge, string pStationModel)
        {
            // Required by the designer
            InitializeComponent();

            tool = pTool;
            sStationModel = pStationModel;
            iSelectedCartridge = pSelectedCartridge;
        }

        public void frmCartridges_Load(System.Object sender, System.EventArgs e)
        {
            butOk.Text = Localization.getResStr(Configuration.gralOkId);

            switch (tool)
            {
                case GenericStationTools.T210:
                    if (sStationModel == "HD" || sStationModel == "HDR")
                    {
                        sCartridgeModel = "470";
                    }
                    else
                    {
                        sCartridgeModel = "210";
                    }
                    break;
                case GenericStationTools.T245:
                    sCartridgeModel = "245";
                    break;
                case GenericStationTools.PA:
                    sCartridgeModel = "120";
                    break;
                case GenericStationTools.HT:
                    sCartridgeModel = "420";
                    break;
                case GenericStationTools.DS:
                    sCartridgeModel = "360";
                    break;
                case GenericStationTools.DR:
                    sCartridgeModel = "560";
                    break;
            }
            mySearchImages();
            iCurrIdx = getIdx(iSelectedCartridge);
            if (iCurrIdx < 0)
            {
                iCurrIdx = 0;
            }
            showImg(0);
        }

        private void mySearchImages()
        {
            string sFilter = "C" + sCartridgeModel + "_*_datasheet";
            string[] aSplit = null;
            listImgs.Clear();
            try
            {
                List<string> aImgs = new List<string>();

                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("");
                System.Resources.ResourceSet resSet = My.Resources.ResourceCartridge.ResourceManager.GetResourceSet(culture, true, false);
                IDictionaryEnumerator listaCartuchos = resSet.GetEnumerator();
                listaCartuchos.Reset();
                bool bOk = listaCartuchos.MoveNext();
                while (bOk)
                {
                    if (StringType.StrLike((string)listaCartuchos.Key, sFilter, CompareMethod.Binary))
                    {
                        aImgs.Add((string)listaCartuchos.Key);
                    }
                    bOk = listaCartuchos.MoveNext();
                }

                aImgs.Sort();
                for (var i = 0; i <= aImgs.Count - 1; i++)
                {
                    string sDummy = aImgs[i];
                    aSplit = sDummy.Split('_');
                    sDummy = aSplit[1];
                    var str = new strImgs();
                    str.sFilename = aImgs[i];
                    str.sCartridgeModel = sCartridgeModel;
                    str.iCartridgeNbr = int.Parse(sDummy);
                    listImgs.Add(str);
                }
            }
            catch (Exception)
            {
            }
        }

        private void showImg(int iStep)
        {
            if (listImgs.Count == 0)
            {
                return;
            }
            iCurrIdx += iStep;
            if (iCurrIdx > listImgs.Count - 1)
            {
                iCurrIdx = 0;
            }
            if (iCurrIdx < 0)
            {
                iCurrIdx = listImgs.Count - 1;
            }
            pictCart.Image = (Image)My.Resources.ResourceCartridge.ResourceManager.GetObject(listImgs[iCurrIdx].sFilename);
            tbCartridgeNumber.Text = Strings.Format(listImgs[iCurrIdx].iCartridgeNbr, "000");
        }

        private void showNbr(int iCartridgeNumber)
        {
            int idx = getIdx(iCartridgeNumber);
            if (idx >= 0)
            {
                iCurrIdx = idx;
                pictCart.Image = (Image)My.Resources.ResourceCartridge.ResourceManager.GetObject(listImgs[iCurrIdx].sFilename);
                tbCartridgeNumber.Text = Strings.Format(listImgs[iCurrIdx].iCartridgeNbr, "000");
            }
            else
            {
                MessageBox.Show(string.Format(Localization.getResStr(Configuration.cartNotFoundId), Strings.Format(iCartridgeNumber, "000")));
            }
        }

        private int getIdx(int iCartridgeNumber)
        {
            if (listImgs.Count == 0)
            {
                return -1;
            }
            for (var i = 0; i <= listImgs.Count - 1; i++)
            {
                if (listImgs[i].iCartridgeNbr == iCartridgeNumber)
                {
                    return System.Convert.ToInt32(i);
                }
            }
            return -1;
        }

        public void butNext_Click(System.Object sender, System.EventArgs e)
        {
            showImg(1);
        }

        public void butPrevious_Click(System.Object sender, System.EventArgs e)
        {
            showImg(-1);
        }

        public void tbCartridgeNumber_KeyPress(System.Object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // only numbers and backspace
            if (e.KeyChar == Microsoft.VisualBasic.Strings.ChrW((System.Int32)Keys.Enter))
            {
                showNbr(int.Parse(tbCartridgeNumber.Text));
            }
            else
            {
                e.Handled = !RoutinesLibrary.Data.DataType.IntegerUtils.KeyIsNumber(e.KeyChar);
            }
        }

        public void butOk_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
