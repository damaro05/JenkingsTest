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
    public partial class ProfileOptions
    {

        private string m_selectedPageName = "";


        //Navigation
        public delegate void ProfileNewEventHandler();
        private ProfileNewEventHandler ProfileNewEvent;

        public event ProfileNewEventHandler ProfileNew
        {
            add
            {
                ProfileNewEvent = (ProfileNewEventHandler)System.Delegate.Combine(ProfileNewEvent, value);
            }
            remove
            {
                ProfileNewEvent = (ProfileNewEventHandler)System.Delegate.Remove(ProfileNewEvent, value);
            }
        }

        public delegate void ProfileEditEventHandler();
        private ProfileEditEventHandler ProfileEditEvent;

        public event ProfileEditEventHandler ProfileEdit
        {
            add
            {
                ProfileEditEvent = (ProfileEditEventHandler)System.Delegate.Combine(ProfileEditEvent, value);
            }
            remove
            {
                ProfileEditEvent = (ProfileEditEventHandler)System.Delegate.Remove(ProfileEditEvent, value);
            }
        }

        public delegate void ProfileDeleteEventHandler();
        private ProfileDeleteEventHandler ProfileDeleteEvent;

        public event ProfileDeleteEventHandler ProfileDelete
        {
            add
            {
                ProfileDeleteEvent = (ProfileDeleteEventHandler)System.Delegate.Combine(ProfileDeleteEvent, value);
            }
            remove
            {
                ProfileDeleteEvent = (ProfileDeleteEventHandler)System.Delegate.Remove(ProfileDeleteEvent, value);
            }
        }

        public delegate void ProfileCopyEventHandler();
        private ProfileCopyEventHandler ProfileCopyEvent;

        public event ProfileCopyEventHandler ProfileCopy
        {
            add
            {
                ProfileCopyEvent = (ProfileCopyEventHandler)System.Delegate.Combine(ProfileCopyEvent, value);
            }
            remove
            {
                ProfileCopyEvent = (ProfileCopyEventHandler)System.Delegate.Remove(ProfileCopyEvent, value);
            }
        }

        public delegate void ProfileSyncEventHandler();
        private ProfileSyncEventHandler ProfileSyncEvent;

        public event ProfileSyncEventHandler ProfileSync
        {
            add
            {
                ProfileSyncEvent = (ProfileSyncEventHandler)System.Delegate.Combine(ProfileSyncEvent, value);
            }
            remove
            {
                ProfileSyncEvent = (ProfileSyncEventHandler)System.Delegate.Remove(ProfileSyncEvent, value);
            }
        }

        //Edit
        public delegate void AddPointEventHandler();
        private AddPointEventHandler AddPointEvent;

        public event AddPointEventHandler AddPoint
        {
            add
            {
                AddPointEvent = (AddPointEventHandler)System.Delegate.Combine(AddPointEvent, value);
            }
            remove
            {
                AddPointEvent = (AddPointEventHandler)System.Delegate.Remove(AddPointEvent, value);
            }
        }

        public delegate void RemovePointEventHandler();
        private RemovePointEventHandler RemovePointEvent;

        public event RemovePointEventHandler RemovePoint
        {
            add
            {
                RemovePointEvent = (RemovePointEventHandler)System.Delegate.Combine(RemovePointEvent, value);
            }
            remove
            {
                RemovePointEvent = (RemovePointEventHandler)System.Delegate.Remove(RemovePointEvent, value);
            }
        }

        public delegate void SaveEditedProfileEventHandler();
        private SaveEditedProfileEventHandler SaveEditedProfileEvent;

        public event SaveEditedProfileEventHandler SaveEditedProfile
        {
            add
            {
                SaveEditedProfileEvent = (SaveEditedProfileEventHandler)System.Delegate.Combine(SaveEditedProfileEvent, value);
            }
            remove
            {
                SaveEditedProfileEvent = (SaveEditedProfileEventHandler)System.Delegate.Remove(SaveEditedProfileEvent, value);
            }
        }

        public delegate void CancelEditedProfileEventHandler();
        private CancelEditedProfileEventHandler CancelEditedProfileEvent;

        public event CancelEditedProfileEventHandler CancelEditedProfile
        {
            add
            {
                CancelEditedProfileEvent = (CancelEditedProfileEventHandler)System.Delegate.Combine(CancelEditedProfileEvent, value);
            }
            remove
            {
                CancelEditedProfileEvent = (CancelEditedProfileEventHandler)System.Delegate.Remove(CancelEditedProfileEvent, value);
            }
        }



        public ProfileOptions()
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();

            pageProfilesOptions.Location = new Point(0, 0);
            pageProfilesOptionsEdit.Location = new Point(0, 0);

            CurrentPage = pageProfilesOptions.Name;
        }

        public string CurrentPage
        {
            set
            {
                System.Windows.Forms.Control[] aControls = this.Controls.Find(value, true);
                if (aControls.Length > 0)
                {
                    aControls[0].BringToFront();
                    m_selectedPageName = System.Convert.ToString(aControls[0].Name);
                }
            }
            get
            {
                if (!string.IsNullOrEmpty(m_selectedPageName))
                {
                    return m_selectedPageName;
                }
                else
                {
                    return "";
                }
            }
        }

        #region Navigation

        public void butProfilesNew_Click(object sender, EventArgs e)
        {
            if (ProfileNewEvent != null)
                ProfileNewEvent();
        }

        public void butProfilesEdit_Click(object sender, EventArgs e)
        {
            if (ProfileEditEvent != null)
                ProfileEditEvent();
        }

        public void butProfilesDelete_Click(object sender, EventArgs e)
        {
            if (ProfileDeleteEvent != null)
                ProfileDeleteEvent();
        }

        public void butProfilesCopy_Click(object sender, EventArgs e)
        {
            if (ProfileCopyEvent != null)
                ProfileCopyEvent();
        }

        public void butProfilesSync_Click(object sender, EventArgs e)
        {
            if (ProfileSyncEvent != null)
                ProfileSyncEvent();
        }

        #endregion

        #region Edit

        public void butAddPoint_Click(object sender, EventArgs e)
        {
            if (AddPointEvent != null)
                AddPointEvent();
        }

        public void butRemovePoint_Click(object sender, EventArgs e)
        {
            if (RemovePointEvent != null)
                RemovePointEvent();
        }

        public void butSaveEditedProfile_Click(object sender, EventArgs e)
        {
            if (SaveEditedProfileEvent != null)
                SaveEditedProfileEvent();
        }

        public void butCancelEditedProfile_Click(object sender, EventArgs e)
        {
            if (CancelEditedProfileEvent != null)
                CancelEditedProfileEvent();
        }

        #endregion

    }
}
