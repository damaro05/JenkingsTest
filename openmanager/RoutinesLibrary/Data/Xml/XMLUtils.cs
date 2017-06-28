// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports


    namespace RoutinesLibrary.Data.Xml
    {

        public class XMLUtils
        {

            #region Create document

            public static System.Xml.XmlDocument CreateNewDoc()
            {
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                System.Xml.XmlDeclaration xmlDeclaration = default(System.Xml.XmlDeclaration);
                xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                xmlDoc.AppendChild(xmlDeclaration);

                return xmlDoc;
            }

            #endregion

            #region Convertions

            public static string ConvertTextToXML(string sText)
            {
                sText = sText.Replace("&", "&amp;");
                sText = sText.Replace("<", "&lt;");
                sText = sText.Replace(">", "&gt;");
                sText = sText.Replace("\"", "&quot;");
                sText = sText.Replace("'", "&apos;");

                return sText;
            }

            public static string ConvertTextFromXML(string sText)
            {
                sText = sText.Replace("&amp;", "&");
                sText = sText.Replace("&lt;", "<");
                sText = sText.Replace("&gt;", ">");
                sText = sText.Replace("&quot;", "\"");
                sText = sText.Replace("&apos;", "'");

                return sText;
            }

            #endregion

            #region Load / Save

            public static System.Xml.XmlDocument LoadFromFile(string fileName, ref string sError)
            {
                System.Xml.XmlDocument returnValue = default(System.Xml.XmlDocument);
                System.Xml.XmlReaderSettings xmlSet = new System.Xml.XmlReaderSettings();
                xmlSet.CheckCharacters = false;
                xmlSet.DtdProcessing = System.Xml.DtdProcessing.Ignore;
                //xmlSet.ProhibitDtd = False
                xmlSet.ValidationType = System.Xml.ValidationType.None;
                xmlSet.XmlResolver = null;

                System.Xml.XmlReader xmlr = System.Xml.XmlTextReader.Create(fileName, xmlSet);
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();

                sError = "";
                try
                {
                    xmlDoc.Load(xmlr);
                    xmlr.Close();
                    returnValue = xmlDoc;
                }
                catch (Exception ex)
                {
                    sError = ex.Message;
                    returnValue = null;
                }
                return returnValue;
            }

            public static void SaveToFile(System.Xml.XmlDocument xmlDoc, string fileName)
            {
                if (xmlDoc != null && fileName != "")
                {
                    xmlDoc.Save(fileName);
                }
            }

            #endregion

            #region Add / Remove Nodes

            public static System.Xml.XmlNode AddNode(System.Xml.XmlDocument parentNode, string nodeName, string nodeValue = "")
            {
                System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
                xmlDoc = parentNode;

                System.Xml.XmlElement xmlNewElem = xmlDoc.CreateElement(nodeName);
                if (nodeValue != "")
                {
                    xmlNewElem.InnerText = nodeValue;
                }
                xmlDoc.AppendChild(xmlNewElem);

                return xmlNewElem;
            }

            public static System.Xml.XmlNode AddNode(System.Xml.XmlNode parentNode, string nodeName, string nodeValue = "")
            {
                System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
                xmlDoc = ((System.Xml.XmlNode)parentNode).OwnerDocument;

                System.Xml.XmlElement xmlNewElem = xmlDoc.CreateElement(nodeName);
                if (nodeValue != "")
                {
                    xmlNewElem.InnerText = nodeValue;
                }
                parentNode.AppendChild(xmlNewElem);

                return xmlNewElem;
            }

            public static System.Xml.XmlNode SetNode(System.Xml.XmlDocument parentNode, string nodeName, string nodeValue = "")
            {
                System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
                xmlDoc = parentNode;


                // search node
                System.Xml.XmlNode xmlNewElem = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(parentNode, nodeName);
                if (xmlNewElem != null)
                {
                    if (nodeValue != "")
                    {
                        xmlNewElem.InnerText = nodeValue;
                    }
                }
                else
                {
                    // if not exists, create and append
                    xmlNewElem = xmlDoc.CreateElement(nodeName);
                    if (nodeValue != null)
                    {
                        xmlNewElem.InnerText = nodeValue;
                    }
                    xmlDoc.AppendChild(xmlNewElem);
                }

                return xmlNewElem;
            }

            public static System.Xml.XmlNode SetNode(System.Xml.XmlNode parentNode, string nodeName, string nodeValue = "")
            {
                System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
                xmlDoc = parentNode.OwnerDocument;

                // search node
                System.Xml.XmlNode xmlNewElem = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(parentNode, nodeName);
                if (xmlNewElem != null)
                {
                    if (nodeValue != "")
                    {
                        xmlNewElem.InnerText = nodeValue;
                    }
                }
                else
                {
                    // if not exists, create and append
                    xmlNewElem = xmlDoc.CreateElement(nodeName);
                    if (nodeValue != "")
                    {
                        xmlNewElem.InnerText = nodeValue;
                    }
                    parentNode.AppendChild(xmlNewElem);
                }

                return xmlNewElem;
            }

            #endregion

            #region Nodes attributes

            public static System.Xml.XmlAttribute AddAttrib(System.Xml.XmlNode node, string nodeAttrib, string nodeAttribValue)
            {
                if (node != null)
                {
                    System.Xml.XmlDocument xmlDoc = node.OwnerDocument;
                    System.Xml.XmlAttribute xmlAttr = xmlDoc.CreateAttribute(nodeAttrib);
                    xmlAttr.Value = nodeAttribValue;
                    node.Attributes.Append(xmlAttr);

                    return xmlAttr;
                }
                else
                {
                    return null;
                }
            }

            public static System.Xml.XmlAttribute SetAttrib(System.Xml.XmlNode node, string nodeAttrib, string nodeAttribValue)
            {
                if (node != null)
                {
                    System.Xml.XmlAttribute xmlAttr = default(System.Xml.XmlAttribute);
                    xmlAttr = node.Attributes[nodeAttrib];

                    if (ReferenceEquals(xmlAttr, null))
                    {
                        System.Xml.XmlDocument xmlDoc = node.OwnerDocument;
                        xmlAttr = xmlDoc.CreateAttribute(nodeAttrib);
                        xmlAttr.Value = nodeAttribValue;
                        node.Attributes.Append(xmlAttr);
                    }
                    else
                    {
                        xmlAttr.Value = nodeAttribValue;
                    }

                    return xmlAttr;
                }
                else
                {
                    return null;
                }
            }

            #endregion

            #region  Nodes value

            public string GetValue(System.Xml.XmlNode node, string defaultvalue = "")
            {
                if (ReferenceEquals(node, null))
                {
                    return "";
                }
                if (node.InnerText == "" && defaultvalue != null)
                {
                    node.InnerText = defaultvalue;
                }

                return node.InnerText;
            }

            public void SetValue(System.Xml.XmlNode node, string value)
            {
                if (ReferenceEquals(node, null))
                {
                    return;
                }
                node.InnerText = value;
            }

            #endregion

            #region Search Node

            public static System.Xml.XmlNode GetFirstChild(object xmlParent, string sChildName)
            {
                if (ReferenceEquals(xmlParent, null))
                {
                    return null;
                }

                System.Xml.XmlNodeList nodes = default(System.Xml.XmlNodeList);
                if (xmlParent is System.Xml.XmlDocument)
                {
                    nodes = ((System.Xml.XmlDocument)xmlParent).ChildNodes;
                }
                else
                {
                    nodes = ((System.Xml.XmlNode)xmlParent).ChildNodes;
                }

                foreach (System.Xml.XmlNode node in nodes)
                {
                    if (node.Name == sChildName)
                    {
                        return node;
                    }
                }

                return null;
            }

            public static System.Xml.XmlNodeList GetChilds(object xmlParent, string sChildName)
            {
                if (ReferenceEquals(xmlParent, null))
                {
                    return null;
                }

                System.Xml.XmlNodeList nodes = default(System.Xml.XmlNodeList);
                if (xmlParent is System.Xml.XmlDocument)
                {
                    nodes = ((System.Xml.XmlDocument)xmlParent).ChildNodes;
                }
                else
                {
                    nodes = ((System.Xml.XmlNode)xmlParent).ChildNodes;
                }

                return nodes;
            }

            public static string GetAbsXPathFromNode(System.Xml.XmlNode xmlNode)
            {
                string XPath = "";
                do
                {
                    XPath = "/" + xmlNode.Name + XPath;
                    xmlNode = xmlNode.ParentNode;
                } while (!(ReferenceEquals(xmlNode, null) | xmlNode.NodeType == System.Xml.XmlNodeType.Document));

                return XPath;
            }

            public static System.Xml.XmlNode GetNodeFromXPath(System.Xml.XmlDocument xmlDoc, string XPath)
            {
                if (ReferenceEquals(xmlDoc, null))
                {
                    return null;
                }
                return xmlDoc.SelectSingleNode(XPath);
            }

            #endregion

        }

    }

