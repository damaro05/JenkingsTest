// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Manage updates schedule events
    /// </summary>
    /// <remarks></remarks>
    public class CScheduleUpdates
    {

        private const int TIMER_RESCHEDULE = 60 * 60 * 1000; //1h


        private CLocalData m_localData;
        private System.Timers.Timer m_timerPeriodicUpdate = new System.Timers.Timer();
        private System.Timers.Timer m_timerSpecificUpdate = new System.Timers.Timer();
        private System.Timers.Timer m_timerSchedule = new System.Timers.Timer();

        public delegate void Event_UpdateSystemEventHandler();
        private Event_UpdateSystemEventHandler Event_UpdateSystemEvent;

        public event Event_UpdateSystemEventHandler Event_UpdateSystem
        {
            add
            {
                Event_UpdateSystemEvent = (Event_UpdateSystemEventHandler)System.Delegate.Combine(Event_UpdateSystemEvent, value);
            }
            remove
            {
                Event_UpdateSystemEvent = (Event_UpdateSystemEventHandler)System.Delegate.Remove(Event_UpdateSystemEvent, value);
            }
        }



        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="localData">Local data reference</param>
        public CScheduleUpdates(CLocalData localData)
        {
            m_localData = localData;

            m_timerPeriodicUpdate.Elapsed += TimerEvent_PeriodicUpdate;
            m_timerSpecificUpdate.Elapsed += TimerEvent_SpecificUpdate;
            m_timerSchedule.Elapsed += TimerEvent_ReScheduleTimers;

            m_timerSchedule.Interval = TIMER_RESCHEDULE;
            m_timerSchedule.Start();

            ReScheduleTimers();
        }

        /// <summary>
        /// Release resources
        /// </summary>
        public void Dispose()
        {
            m_localData = null;

            m_timerPeriodicUpdate.Dispose();
            m_timerSpecificUpdate.Dispose();
            m_timerSchedule.Dispose();
        }


        #region Schedule specific time

        /// <summary>
        /// Get the scheduled time to start an update in a concrete date
        /// </summary>
        /// <returns>Scheduled time</returns>
        public dc_InfoUpdateSpecificTime GetUpdateSpecificTime()
        {
            return m_localData.GetSpecificTimeInfo();
        }

        /// <summary>
        /// Set the scheduled time to start a update in a concrete date
        /// </summary>
        /// <param name="infoUpdateSpecificTime">Scheduled time</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetUpdateSpecificTime(dc_InfoUpdateSpecificTime infoUpdateSpecificTime)
        {
            bool bOk = m_localData.SetSpecificTimeInfo(infoUpdateSpecificTime);
            m_timerSpecificUpdate.Stop();
            ReScheduleTimers();

            return bOk;
        }

        #endregion


        #region Schedule periodic time

        /// <summary>
        /// Get the scheduled time of periodic updates
        /// </summary>
        /// <returns>Scheduled periodic updates</returns>
        public dc_InfoUpdatePeriodicTime GetUpdatePeriodicTime()
        {
            return m_localData.GetPeriodicTimeInfo();
        }

        /// <summary>
        /// Set the scheduled time of periodic updates
        /// </summary>
        /// <param name="infoUpdatePeriodicTime">Scheduled time</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetScheduleUpdatePeriodic(dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime)
        {
            bool bOk = m_localData.SetPeriodicTimeInfo(infoUpdatePeriodicTime);
            m_timerPeriodicUpdate.Stop();
            ReScheduleTimers();

            return bOk;
        }

        /// <summary>
        /// Get the information of periodic updates check
        /// </summary>
        /// <returns>Information of periodic updates check</returns>
        public dc_InfoCheckPeriodicTime GetCheckPeriodicTime()
        {
            return m_localData.GetCheckPeriodicTimeInfo();
        }

        /// <summary>
        /// Set periodic updates check
        /// </summary>
        /// <param name="infoCheckPeriodicTime">Periodic updates check configuration</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetCheckPeriodicTime(dc_InfoCheckPeriodicTime infoCheckPeriodicTime)
        {
            bool bOk = m_localData.SetCheckPeriodicTimeInfo(infoCheckPeriodicTime);
            m_timerPeriodicUpdate.Stop();
            ReScheduleTimers();

            return bOk;
        }

        #endregion


        #region Timers schedule

        /// <summary>
        /// Timer event to start a periodic update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerEvent_PeriodicUpdate(System.Object sender, System.Timers.ElapsedEventArgs e)
        {
            m_timerPeriodicUpdate.Stop();
            ReScheduleTimers();

            if (Event_UpdateSystemEvent != null)
                Event_UpdateSystemEvent();
        }

        /// <summary>
        /// Timer event to start a update in a concrete date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerEvent_SpecificUpdate(System.Object sender, System.Timers.ElapsedEventArgs e)
        {
            m_localData.SetUpdateSpecificAvailable(false);
            ReScheduleTimers();

            if (Event_UpdateSystemEvent != null)
                Event_UpdateSystemEvent();
        }

        /// <summary>
        /// Timer event to re-schedule the timers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>The scheduled time can exceed the maximum timer interval (Int32 ~ 24.8 days)</remarks>
        private void TimerEvent_ReScheduleTimers(System.Object sender, System.Timers.ElapsedEventArgs e)
        {
            ReScheduleTimers();
        }

        /// <summary>
        /// Re-schedule the timers
        /// </summary>
        /// <remarks>The scheduled time can exceed the maximum timer interval (Int32 ~ 24.8 days)</remarks>
        private void ReScheduleTimers()
        {

            //Periodic Update
            dc_InfoUpdatePeriodicTime infoUpdatePeriodic = m_localData.GetPeriodicTimeInfo();

            if (infoUpdatePeriodic.available)
            {
                if (!m_timerPeriodicUpdate.Enabled)
                {

                    DateTime actDate = DateTime.Now;
                    int interval = 0;

                    interval += System.Convert.ToInt32((infoUpdatePeriodic.hour - actDate.Hour) * 60 * 60);
                    interval += System.Convert.ToInt32((infoUpdatePeriodic.minute - actDate.Minute) * 60);

                    if (infoUpdatePeriodic.modeDaily)
                    {
                        if (interval <= 0)
                        {
                            interval += 24 * 60 * 60; //le sumamos un dia
                        }
                    }
                    else
                    {
                        int dayDiff = infoUpdatePeriodic.weekday - (int)actDate.DayOfWeek;
                        if (dayDiff < 0)
                        {
                            dayDiff += 7;
                        }

                        interval += dayDiff * 24 * 60 * 60;
                        if (interval <= 0)
                        {
                            interval += 7 * 24 * 60 * 60; //le sumamos una semana
                        }
                    }

                    m_timerPeriodicUpdate.Interval = interval * 1000; //miliseconds
                    m_timerPeriodicUpdate.Start();
                }
            }
            else
            {
                m_timerPeriodicUpdate.Stop();
            }


            //Specific Update
            dc_InfoUpdateSpecificTime infoUpdateSpecific = m_localData.GetSpecificTimeInfo();

            if (infoUpdateSpecific.available)
            {
                if (!m_timerSpecificUpdate.Enabled)
                {
                    DateTime actDate = DateTime.Now;

                    if ((infoUpdateSpecific.time - actDate).TotalMilliseconds < int.MaxValue &&
                        (infoUpdateSpecific.time - actDate).TotalMilliseconds > 0)
                    {
                        m_timerSpecificUpdate.Interval = System.Convert.ToDouble((infoUpdateSpecific.time - actDate).TotalMilliseconds);
                        m_timerSpecificUpdate.Start();
                    }
                    else
                    {
                        m_timerSpecificUpdate.Stop();
                    }
                }
            }
            else
            {
                m_timerSpecificUpdate.Stop();
            }

        }

        #endregion

    }
}
