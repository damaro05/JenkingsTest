// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
using JBC_ConnectRemote.JBCService;
using DataJBC;
// End of VB project level imports


namespace JBC_ConnectRemote
{
    public class CConvertRobotConfigurationFromDC
    {

        public static void CopyData(CRobotData robot,
                                    dc_RobotConfiguration dcRobot)
        {

            robot.Status = (DataJBC.OnOff)dcRobot.Status;
            robot.Protocol = (CRobotData.RobotProtocol)dcRobot.Protocol;
            robot.Address = dcRobot.Address;
            robot.Speed = (CRobotData.RobotSpeed)dcRobot.Speed;
            robot.DataBits = dcRobot.DataBits;
            robot.StopBits = (CRobotData.RobotStop)dcRobot.StopBits;
            robot.Parity = (CRobotData.RobotParity)dcRobot.Parity;
        }

    }
}
