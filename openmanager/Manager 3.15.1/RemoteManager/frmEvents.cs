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

using System.Threading;


namespace RemoteManager
{
    public partial class frmEvents
    {

        public enum eventType
        {
            TypeError,
            TypeConnection
        }

        private TreeNode rootErrors;
        private TreeNode rootConnections;

        public object objSync = new object();


        public frmEvents()
        {

            // Llamada necesaria para el diseñador.
            InitializeComponent();

            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            rootErrors = RoutinesLibrary.UI.TreeViewUtils.AddNode(treeEvents, Configuration.evErrorsId, Localization.getResStr(Configuration.evErrorsId));
            rootConnections = RoutinesLibrary.UI.TreeViewUtils.AddNode(treeEvents, Configuration.evConnectionsId, Localization.getResStr(Configuration.evConnectionsId));
            ReLoadTexts();
        }

        public void ReLoadTexts()
        {
            // titles
            this.Text = Localization.getResStr(Configuration.evEventsId);
            rootErrors.Text = Localization.getResStr(Configuration.evErrorsId);
            rootConnections.Text = Localization.getResStr(Configuration.evConnectionsId);
        }

        public void addEvent(eventType type, string text)
        {
            lock (objSync)
            {
                TreeNode node = default(TreeNode);
                switch (type)
                {
                    case eventType.TypeError:
                        node = rootErrors;
                        break;
                    case eventType.TypeConnection:
                        node = rootConnections;
                        break;
                    default:
                        node = rootErrors;
                        break;
                }

                text = DateTime.Now.ToString("[HH:mm:ss] ") + text;
                RoutinesLibrary.UI.TreeViewUtils.AddNode(node, string.Format("yyyyMMdd_HHmmss", DateTime.Now), text);
            }
        }

        public void frmEvents_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
