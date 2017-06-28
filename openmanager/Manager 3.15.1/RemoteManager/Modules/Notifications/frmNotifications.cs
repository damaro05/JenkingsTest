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
    public partial class frmNotifications
    {

        public struct tNotificationMessage
        {
            public string message;
            public DateTime time;
            public typeNotificationMessage type;
        }

        public enum typeNotificationMessage
        {
            WARNING,
            INFORMATION,
            UPDATE,
            HOSTCONTROLLER
        }


        private const int NOTIFICATION_DISTANCE_ROWS = 3; //Distancia entre filas
        private const int NOTIFICATION_HEIGHT = 80; //Altura del contenedor

        private const int TIME_VISIBLE_NOTIFICATIONS = 10 * 1000; //10 seconds
        private const int TIME_GARBAGE_COLLECTOR_NOTIFICATIONS = 10 * 1000; //10 seconds
        private const int TIME_WARNING_LIFE = 5 * 60 * 1000; //5 minutes
        private const int TIME_INFORMATION_LIFE = 2 * 60 * 1000; //2 minutes


        private System.Timers.Timer m_timerVisibleNotifications = new System.Timers.Timer();
        private System.Timers.Timer m_timerGarbageCollectorNotifications = new System.Timers.Timer();
        private List<tNotificationMessage> m_notificationsList = new List<tNotificationMessage>();
        private List<tNotificationMessage> m_notificationsListPermanent = new List<tNotificationMessage>();
        private int m_indexList = 0;
        private int m_indexListPermanent = 0;

        public delegate void NotificationAvailableEventHandler();
        private NotificationAvailableEventHandler NotificationAvailableEvent;

        public event NotificationAvailableEventHandler NotificationAvailable
        {
            add
            {
                NotificationAvailableEvent = (NotificationAvailableEventHandler)System.Delegate.Combine(NotificationAvailableEvent, value);
            }
            remove
            {
                NotificationAvailableEvent = (NotificationAvailableEventHandler)System.Delegate.Remove(NotificationAvailableEvent, value);
            }
        }

        public delegate void NotificationUnavailableEventHandler();
        private NotificationUnavailableEventHandler NotificationUnavailableEvent;

        public event NotificationUnavailableEventHandler NotificationUnavailable
        {
            add
            {
                NotificationUnavailableEvent = (NotificationUnavailableEventHandler)System.Delegate.Combine(NotificationUnavailableEvent, value);
            }
            remove
            {
                NotificationUnavailableEvent = (NotificationUnavailableEventHandler)System.Delegate.Remove(NotificationUnavailableEvent, value);
            }
        }



        /// <summary>
        /// Class constructor
        /// </summary>
        public frmNotifications()
        {
            InitializeComponent();

            //BackColor
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.uControlNotification.BackColor = SystemColors.Control;
            this.uControlNotificationPermanent.BackColor = SystemColors.Control;

            RefreshShowedNotification();

            //Activamos los timers de visibilidad de las notificaciones
            m_timerVisibleNotifications.AutoReset = false;
            m_timerVisibleNotifications.Elapsed += TimerEvent_TimerVisibileNotifications;

            m_timerGarbageCollectorNotifications.Interval = TIME_GARBAGE_COLLECTOR_NOTIFICATIONS;
            m_timerGarbageCollectorNotifications.Elapsed += TimerEvent_TimerNotificationLife;
        }

        /// <summary>
        /// Show the notifications panel
        /// </summary>
        public void ShowNotifications()
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => ShowNotifications()));
                return;
            }

            this.Show();
            m_timerVisibleNotifications.Interval = TIME_VISIBLE_NOTIFICATIONS;
            m_timerVisibleNotifications.Start();

            this.Refresh();
        }

        /// <summary>
        /// Hide the notifications panel
        /// </summary>
        private void HideNotifications()
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => HideNotifications()));
                return;
            }

            m_timerVisibleNotifications.Stop();

            //Existen notificaciones permanentes para mostrar
            if (m_notificationsListPermanent.Count > 0)
            {

                //Paneles
                this.uControlNotificationPermanent.Visible = true;
                this.uControlNotification.Visible = false;

                //Contenedor
                this.Size = new Size(this.Width, NOTIFICATION_HEIGHT);

                //No existen notificaciones
            }
            else
            {
                this.Hide();
            }
        }

        /// <summary>
        /// Add a new notification
        /// </summary>
        /// <param name="noti">New notification</param>
        public void Add(tNotificationMessage noti)
        {

            noti.time = DateTime.Now;

            //Añadimos la notificación a la lista correspondiente
            if (noti.type == typeNotificationMessage.INFORMATION |
                    noti.type == typeNotificationMessage.WARNING)
            {
                noti.message = noti.time.ToString("[HH:mm:ss] ") + noti.message;
                m_notificationsList.Insert(0, noti);
                m_indexList = 0;

                m_timerGarbageCollectorNotifications.Start();

            }
            else if (noti.type == typeNotificationMessage.UPDATE)
            {
                m_notificationsListPermanent.Insert(0, noti);
                m_indexListPermanent = 0;

            }
            else if (noti.type == typeNotificationMessage.HOSTCONTROLLER)
            {
                m_notificationsListPermanent.Insert(0, noti);
                m_indexListPermanent = 0;
            }

            RefreshShowedNotification();

            ShowNotifications();
            if (NotificationAvailableEvent != null)
                NotificationAvailableEvent();
        }

        public void Remove(typeNotificationMessage type)
        {
            int i = 0;

            //Eliminamos todas las notificaciones del tipo seleccionado
            while (i < m_notificationsListPermanent.Count)
            {
                if (m_notificationsListPermanent[i].type == type)
                {
                    m_notificationsListPermanent.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            RefreshShowedNotification();

            //Si no hay notificaciones oculta el panel
            if (m_notificationsList.Count == 0 &
                    m_notificationsListPermanent.Count == 0)
            {
                HideNotifications();
            }
        }

        /// <summary>
        /// Update notification text
        /// </summary>
        private void RefreshShowedNotification()
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => RefreshShowedNotification()));
                return;
            }

            //Calcular el correcto valor de index
            if (m_indexList < 0)
            {
                m_indexList = 0;
            }
            if (m_indexList > m_notificationsList.Count - 1)
            {
                m_indexList = m_notificationsList.Count - 1;
            }

            if (m_indexListPermanent < 0)
            {
                m_indexListPermanent = 0;
            }
            if (m_indexListPermanent > m_notificationsListPermanent.Count - 1)
            {
                m_indexListPermanent = m_notificationsListPermanent.Count - 1;
            }


            //
            //Notificaciones permanentes
            //

            //Existen notificaciones
            if (m_notificationsListPermanent.Count > 0)
            {

                //Mostrar las flechas de dirección
                if (m_notificationsListPermanent.Count == 1)
                {
                    this.uControlNotificationPermanent.button_left.Hide();
                    this.uControlNotificationPermanent.button_right.Hide();

                }
                else if (m_indexListPermanent == 0)
                {
                    this.uControlNotificationPermanent.button_left.Hide();
                    this.uControlNotificationPermanent.button_right.Show();

                }
                else if (m_indexListPermanent == m_notificationsListPermanent.Count - 1)
                {
                    this.uControlNotificationPermanent.button_left.Show();
                    this.uControlNotificationPermanent.button_right.Hide();

                }
                else
                {
                    this.uControlNotificationPermanent.button_left.Show();
                    this.uControlNotificationPermanent.button_right.Show();
                }

                //Actualizar textos
                this.uControlNotificationPermanent.textDescription.Text = System.Convert.ToString(m_notificationsListPermanent[m_indexListPermanent].message);
                this.uControlNotificationPermanent.textBox_page.Text = (m_indexListPermanent + 1) + "/" + System.Convert.ToString(m_notificationsListPermanent.Count);

                //Actualizar icono
                this.uControlNotificationPermanent.imgNotif.Show();

                if (m_notificationsListPermanent[m_indexListPermanent].type == typeNotificationMessage.UPDATE)
                {
                    this.uControlNotificationPermanent.imgNotif.Image = My.Resources.Resources.update;
                }
                else if (m_notificationsListPermanent[m_indexListPermanent].type == typeNotificationMessage.HOSTCONTROLLER)
                {
                    this.uControlNotificationPermanent.imgNotif.Image = My.Resources.Resources.HostControllerServer;
                }
                else
                {
                    this.uControlNotificationPermanent.imgNotif.Hide();
                }
            }


            //
            //Notificaciones
            //

            //Existen notificaciones
            if (m_notificationsList.Count > 0)
            {

                //Mostrar las flechas de dirección
                if (m_notificationsList.Count == 1)
                {
                    this.uControlNotification.button_left.Hide();
                    this.uControlNotification.button_right.Hide();

                }
                else if (m_indexList == 0)
                {
                    this.uControlNotification.button_left.Hide();
                    this.uControlNotification.button_right.Show();

                }
                else if (m_indexList == m_notificationsList.Count - 1)
                {
                    this.uControlNotification.button_left.Show();
                    this.uControlNotification.button_right.Hide();

                }
                else
                {
                    this.uControlNotification.button_left.Show();
                    this.uControlNotification.button_right.Show();
                }

                //Actualizar textos
                this.uControlNotification.textDescription.Text = System.Convert.ToString(m_notificationsList[m_indexList].message);
                this.uControlNotification.textBox_page.Text = (m_indexList + 1) + "/" + System.Convert.ToString(m_notificationsList.Count);

                //Actualizar icono
                this.uControlNotification.imgNotif.Show();

                if (m_notificationsList[m_indexList].type == typeNotificationMessage.WARNING)
                {
                    this.uControlNotification.imgNotif.Image = My.Resources.Resources.warning_icon;
                }
                else if (m_notificationsList[m_indexList].type == typeNotificationMessage.INFORMATION)
                {
                    this.uControlNotification.imgNotif.Image = My.Resources.Resources.information;
                }
                else
                {
                    this.uControlNotification.imgNotif.Hide();
                }
            }


            //
            //Posición y visibilidad de los paneles de notificación
            //

            //Existen notificaciones en los dos paneles
            if (m_notificationsListPermanent.Count > 0 & m_notificationsList.Count > 0)
            {

                //Paneles
                this.uControlNotificationPermanent.Visible = true;
                this.uControlNotification.Visible = true;
                this.uControlNotification.Location = new Point(0, NOTIFICATION_HEIGHT + NOTIFICATION_DISTANCE_ROWS);

                //Contenedor
                this.Size = new Size(this.Width, NOTIFICATION_HEIGHT * 2 + NOTIFICATION_DISTANCE_ROWS);

                //Existen notificaciones solo en el panel permanente
            }
            else if (m_notificationsListPermanent.Count > 0)
            {

                //Paneles
                this.uControlNotificationPermanent.Visible = true;
                this.uControlNotification.Visible = false;

                //Contenedor
                this.Size = new Size(this.Width, NOTIFICATION_HEIGHT);

                //Existen notificaciones solo en el panel
            }
            else if (m_notificationsList.Count > 0)
            {

                //Paneles
                this.uControlNotificationPermanent.Visible = false;
                this.uControlNotification.Visible = true;
                this.uControlNotification.Location = new Point(0, 0);

                //Contenedor
                this.Size = new Size(this.Width, NOTIFICATION_HEIGHT);

                //No existen notificaciones
            }
            else
            {
                //Paneles
                this.uControlNotificationPermanent.Visible = false;
                this.uControlNotification.Visible = true;
                this.uControlNotification.Location = new Point(0, 0);

                //Contenedor
                this.Size = new Size(this.Width, NOTIFICATION_HEIGHT);

                ShowDefaultText();
            }

            this.uControlNotificationPermanent.Refresh();
            this.uControlNotification.Refresh();
            this.Refresh();
        }

        /// <summary>
        /// Show the default notification
        /// </summary>
        private void ShowDefaultText()
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => ShowDefaultText()));
                return;
            }

            this.uControlNotification.button_left.Hide();
            this.uControlNotification.button_right.Hide();

            this.uControlNotification.textDescription.Text = Localization.getResStr(Configuration.notifNoNotifId);
            this.uControlNotification.textBox_page.Text = "";
            this.uControlNotification.imgNotif.Hide();

            this.uControlNotification.Refresh();
            this.Refresh();

            if (NotificationUnavailableEvent != null)
                NotificationUnavailableEvent();
        }

        /// <summary>
        /// Reload all texts
        /// </summary>
        public void ReLoadTexts()
        {
            if (m_notificationsList.Count == 0)
            {
                ShowDefaultText();
            }

            this.Refresh();
        }

        #region UI interaction

        /// <summary>
        /// On mouse move over the panel restart the timer to hide the notifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void frmNotifications_MouseMove(object sender, EventArgs e)
        {
            m_timerVisibleNotifications.Interval = TIME_VISIBLE_NOTIFICATIONS;
        }

        /// <summary>
        /// Display the previous notification
        /// </summary>
        public void frmNotifications_Click_ButtonLeft()
        {
            m_indexList--;
            RefreshShowedNotification();
        }

        /// <summary>
        /// Display the subsequent notification
        /// </summary>
        public void frmNotifications_Click_ButtonRight()
        {
            m_indexList++;
            RefreshShowedNotification();
        }

        /// <summary>
        /// Erase the current notification
        /// </summary>
        public void frmNotifications_Click_ButtonClose()
        {

            //Si no hay notificaciones oculta el panel
            if (m_notificationsList.Count == 0)
            {
                HideNotifications();
                return;
            }

            //Elimina la notificación y si no hay notificaciones oculta el panel
            m_notificationsList.RemoveAt(m_indexList);
            if (m_notificationsList.Count == 0)
            {
                ShowDefaultText();
                HideNotifications();
                return;
            }

            RefreshShowedNotification();
        }

        /// <summary>
        /// Display the previous notification
        /// </summary>
        public void frmNotificationsPermanent_Click_ButtonLeft()
        {
            m_indexListPermanent--;
            RefreshShowedNotification();
        }

        /// <summary>
        /// Display the subsequent notification
        /// </summary>
        public void frmNotificationsPermanent_Click_ButtonRight()
        {
            m_indexListPermanent++;
            RefreshShowedNotification();
        }

        /// <summary>
        /// Erase the current notification
        /// </summary>
        public void frmNotificationsPermanent_Click_ButtonClose()
        {

            //Elimina la notificación
            m_notificationsListPermanent.RemoveAt(m_indexListPermanent);

            RefreshShowedNotification();

            //Si no hay notificaciones oculta el panel
            if (m_notificationsList.Count == 0 &
                    m_notificationsListPermanent.Count == 0)
            {
                HideNotifications();
            }
        }

        #endregion


        #region Timers

        /// <summary>
        /// Elapsed visible time. Hide notification panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerEvent_TimerVisibileNotifications(System.Object sender, System.Timers.ElapsedEventArgs e)
        {
            HideNotifications();
        }

        /// <summary>
        /// Erase the oldest notifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerEvent_TimerNotificationLife(System.Object sender, System.Timers.ElapsedEventArgs e)
        {

            DateTime actDate = DateTime.Now;
            bool bContWarningEraser = true;
            bool bContInformationEraser = true;
            int i = m_notificationsList.Count - 1;
            bool bChanges = false;

            while (i >= 0)
            {
                if (!bContWarningEraser && !bContInformationEraser)
                {
                    break;
                }

                if (m_notificationsList.ElementAt(i).type == typeNotificationMessage.WARNING)
                {
                    if (!bContWarningEraser)
                    {
                        i--;
                        continue;
                    }
                    else if (m_notificationsList.ElementAt(i).time.AddMilliseconds(TIME_WARNING_LIFE) < actDate)
                    {
                        m_notificationsList.RemoveAt(i);
                        bChanges = true;
                    }
                    else
                    {
                        bContWarningEraser = false;
                    }
                }
                else if (m_notificationsList.ElementAt(i).type == typeNotificationMessage.INFORMATION)
                {
                    if (!bContInformationEraser)
                    {
                        i--;
                        continue;
                    }
                    else if (m_notificationsList.ElementAt(i).time.AddMilliseconds(TIME_INFORMATION_LIFE) < actDate)
                    {
                        m_notificationsList.RemoveAt(i);
                        bChanges = true;
                    }
                    else
                    {
                        bContInformationEraser = false;
                    }
                }
                else
                {
                    m_notificationsList.RemoveAt(i);
                    bChanges = true;
                }

                i--;
            }

            if (m_notificationsList.Count == 0)
            {
                m_timerGarbageCollectorNotifications.Stop();
            }

            if (bChanges)
            {
                RefreshShowedNotification();
            }
        }

        #endregion

    }
}
