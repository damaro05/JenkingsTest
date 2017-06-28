// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.Reflection;
using System.Xml;

namespace DataJBC
{
    public class CCartridgeData : ICloneable
    {

        public ushort CartridgeNbr = System.Convert.ToUInt16(0);
        public OnOff CartridgeOnOff = OnOff._OFF;
        public short CartridgeAdj300 = (short)0;
        public short CartridgeAdj400 = (short)0;
        public byte CartridgeGroup = (byte)0;
        public byte CartridgeFamily = (byte)0;


        public dynamic Clone()
        {
            CCartridgeData cls_Cartridge_Clonado = new CCartridgeData();
            cls_Cartridge_Clonado.CartridgeNbr = this.CartridgeNbr;
            cls_Cartridge_Clonado.CartridgeOnOff = this.CartridgeOnOff;
            cls_Cartridge_Clonado.CartridgeAdj300 = this.CartridgeAdj300;
            cls_Cartridge_Clonado.CartridgeAdj400 = this.CartridgeAdj400;
            cls_Cartridge_Clonado.CartridgeGroup = this.CartridgeGroup;
            cls_Cartridge_Clonado.CartridgeFamily = this.CartridgeFamily;

            return cls_Cartridge_Clonado;
        }

        /// <summary>
        /// Calculate the parameters of a cartridge given a tool and a station model
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="stationModel"></param>
        /// <returns>True if the operation was succesfull</returns>
        /// <remarks></remarks>
        public bool CalculateParametersFromNumber(GenericStationTools tool, string stationModel)
        {
            bool bOk = false;

            //Family
            string sCartridgeFamily = "";

            switch (tool)
            {
                case GenericStationTools.T210:
                    if (stationModel == "HD" || stationModel == "HDR")
                    {
                        CartridgeFamily = byte.Parse("470");
                        sCartridgeFamily = "C470";
                    }
                    else
                    {
                        CartridgeFamily = (byte)0; //C210
                        sCartridgeFamily = "C210";
                    }
                    break;
                case GenericStationTools.T245:
                    CartridgeFamily = (byte)1; //C245
                    sCartridgeFamily = "C245";
                    break;
                case GenericStationTools.PA:
                    CartridgeFamily = (byte)2; //C120
                    sCartridgeFamily = "C120";
                    break;
                case GenericStationTools.HT:
                    CartridgeFamily = (byte)3; //C420
                    sCartridgeFamily = "C420";
                    break;
                case GenericStationTools.DS:
                    CartridgeFamily = (byte)4; //C360
                    sCartridgeFamily = "C360";
                    break;
                case GenericStationTools.DR:
                    CartridgeFamily = (byte)5; //C560
                    sCartridgeFamily = "C560";
                    break;
            }

            XmlDocument cartridgesXML = new XmlDocument();
            try
            {
                Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("JBC_Connect.cartridges.xml");
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.XmlResolver = null;
                settings.DtdProcessing = DtdProcessing.Ignore;
                XmlReader reader = XmlReader.Create(s, settings);
                cartridgesXML.Load(reader);
                reader.Close();
            }
            catch (Exception)
            {
                cartridgesXML = null;
            }

            if (cartridgesXML != null)
            {

                XmlNode cartridgesNode = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(cartridgesXML, "Cartridges");
                if (cartridgesNode != null)
                {

                    XmlNode cartridgesListNode = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(cartridgesNode, "Cartridge_list");
                    XmlNode cartridgesGroupNode = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(cartridgesNode, "Cartridge_group");

                    if (cartridgesListNode != null && cartridgesGroupNode != null)
                    {

                        //Group
                        System.Xml.XmlNodeList cartridgesRecordListNode = RoutinesLibrary.Data.Xml.XMLUtils.GetChilds(cartridgesListNode, "Cartridge_record");

                        if (cartridgesRecordListNode != null)
                        {
                            foreach (System.Xml.XmlNode node in cartridgesRecordListNode)
                            {
                                try
                                {
                                    if (double.Parse(node["Name"].InnerText) == CartridgeNbr && node["Family_name"].InnerText == sCartridgeFamily)
                                    {
                                        CartridgeGroup = byte.Parse(node["Group_id"].InnerText);
                                        goto endOfForLoop;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            endOfForLoop:
                            1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.
                        }

                        //Adj 300, Adj 400
                        System.Xml.XmlNodeList cartridgesRecordGroupNode = RoutinesLibrary.Data.Xml.XMLUtils.GetChilds(cartridgesGroupNode, "Group_record");

                        if (cartridgesRecordGroupNode != null)
                        {
                            foreach (System.Xml.XmlNode node in cartridgesRecordGroupNode)
                            {
                                try
                                {
                                    if (double.Parse(node["Group_id"].InnerText) == CartridgeGroup && node["Family_name"].InnerText == sCartridgeFamily)
                                    {
                                        CartridgeAdj300 = short.Parse(node["Point_300"].InnerText);
                                        CartridgeAdj400 = short.Parse(node["Point_400"].InnerText);
                                        bOk = true;
                                        goto endOfForLoop1;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            endOfForLoop1:
                            1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.
                        }
                    }
                }
            }

            return bOk;
        }

    }

}