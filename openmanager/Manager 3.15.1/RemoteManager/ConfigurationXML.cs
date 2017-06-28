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


namespace RemoteManager
{
    public class ConfigurationXML
    {

        // XML attributes
        public const string xmlVersionId = "Version";
        public const string xmlLangId = "lang";
        public const string xmlNumberId = "Number";
        public const string xmlTypeId = "Type";
        public const string xmlCheckedId = "Checked";
        public const string xmlEnabledId = "Enabled";
        public const string xmlTextId = "Text";
        public const string xmlTextValueId = "TextValue";
        public const string xmlDateTimeId = "DateTime";
        public const string xmlDateId = "Date";
        public const string xmlTimeId = "Time";


        public static bool getXmlChecked(System.Xml.XmlNode node, ref bool bChecked)
        {
            if (ReferenceEquals(node, null))
            {
                return false;
            }
            System.Xml.XmlAttribute xmlAttrib = node.Attributes[xmlCheckedId];
            if (xmlAttrib != null)
            {
                switch (xmlAttrib.Value.ToLower())
                {
                    case "false":
                        bChecked = false;
                        return true;
                    case "true":
                        bChecked = true;
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool getXmlEnabled(System.Xml.XmlNode node, ref bool bEnabled)
        {
            if (ReferenceEquals(node, null))
            {
                return false;
            }
            System.Xml.XmlAttribute xmlAttrib = node.Attributes[xmlEnabledId];
            if (xmlAttrib != null)
            {
                switch (xmlAttrib.Value.ToLower())
                {
                    case "false":
                        bEnabled = false;
                        return true;
                    case "true":
                        bEnabled = true;
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
