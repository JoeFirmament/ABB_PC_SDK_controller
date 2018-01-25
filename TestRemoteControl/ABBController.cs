using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using RobotStudio.Services.RobApi;
using RobotStudio.Services.RobApi.Desktop;
using System.Collections;

namespace TestRemoteControl
{
    class ABBController
    {
        public ControllerInfoCollection controllers = null;

        public List<string> errLogger(List<string> err,string str)
        {
            err.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "    " + str);
            Console.Write(err.ToArray());
            return err;
        }
        
        public void Scan()
        {
            NetworkScanner netscan = new NetworkScanner();
            netscan.Scan();
            controllers = netscan.Controllers;
        }

        public Controller GetController(int Select)
        {
            return new Controller(controllers[Select]);
        }
       

        public int PPtoMain(Controller c, out List<string> result)
        {
            result = new List<string>();
            //Controller c = Connect();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            if (c == null)
            {
                result.Add("[error]    No controller connect");
                return -1;
            }
            else
            {
                foreach (ABB.Robotics.Controllers.RapidDomain.Task t in c.Rapid.GetTasks())
                {
                    int re = RAPID_ProgramReset(c, out result, t.Name);
                    //int re = 0;
                    Console.WriteLine(t.Name);

                    //int re = RAPID_ProgramReset(c, out result, "T_ROB1");
                    //re = RAPID_ProgramReset(c, out result, "T_ROB2");
                    //re = RAPID_ProgramReset(c, out result, "T_ROB3");
                    //re = RAPID_ProgramReset(c, out result, "T_ROB4");

                    if (re == -1)
                    {
                        return -1;
                    }
                }
                return 0;
            }
        }

        public int Start(Controller c, out List<string> result)
        {
            result = new List<string>();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            //Controller c = Connect();
            if (c == null)
            {
                result.Add("[error]    No controller connect");
                return -1;
            }
            else
            {
                return RAPID_ProgramStart(c, out result);

            }
        }

        public int Stop(Controller c, out List<string> result)
        {
            result = new List<string>();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            //Controller c = Connect();
            if (c == null)
            {
                result.Add("[error]    No controller connect");
                return -1;
            }
            else
            {
                return RAPID_ProgramStop(c, out result);

            }
        }


        private int RAPID_ProgramReset(Controller c, out List<string> result, string taskname)
        //private int RAPID_ProgramReset(Controller c, out List<string> result, string taskname = "T_ROB1")
        {
            result = new List<string>();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            try
            {
                if (c.OperatingMode != ControllerOperatingMode.Auto)
                {
                    result.Add("[error]    Need Auto Mode");
                    return -1;
                }

                if (!c.AuthenticationSystem.CheckDemandGrant(Grant.ExecuteRapid))
                    c.AuthenticationSystem.DemandGrant(Grant.ExecuteRapid);
                using (Mastership m = Mastership.Request(c.Rapid))
                {
                    try
                    {

                        c.Rapid.GetTask(taskname).ResetProgramPointer();
                        result.Add("[msg]    Program Reset");

                        m.Release();
                    }
                    catch (Exception ex)
                    {
                        result.Add("[error]"+ex.ToString());
                        m.Release();
                        return -1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                result.Add("[error]"+ex.ToString());
                return -1;
            }
        }

        private int RAPID_ProgramStart(Controller c, out List<string> result)
        {
            result = new List<string>();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            try
            {
                if (c.OperatingMode != ControllerOperatingMode.Auto)
                {
                    result.Add("[error]    Need Auto Mode");
                    return -1;
                }
                if (c.State != ControllerState.MotorsOn)
                {
                    result.Add("[error]  Motor need ON");
                    return -1;
                }
                if (!c.AuthenticationSystem.CheckDemandGrant(Grant.ExecuteRapid))
                    c.AuthenticationSystem.DemandGrant(Grant.ExecuteRapid);
                using (Mastership m = Mastership.Request(c.Rapid))
                {
                    try
                    {
                        c.Rapid.Start();
                        m.Release();
                        result.Add("[msg]    Program Start");

                    }
                    catch (Exception ex)
                    {
                        result.Add("[error]" + ex.ToString());
                        m.Release();
                        return -1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                result.Add("[error]" + ex.ToString());
                return -1;
            }
        }

        private int RAPID_ProgramStop(Controller c, out List<string> result)
        {
            result = new List<string>();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            try
            {
                if (c.OperatingMode != ControllerOperatingMode.Auto)
                {
                    result.Add("[error]    Need Auto Mode");
                    return -1;
                }
                if (!c.AuthenticationSystem.CheckDemandGrant(Grant.ExecuteRapid))
                    c.AuthenticationSystem.DemandGrant(Grant.ExecuteRapid);
                using (Mastership m = Mastership.Request(c.Rapid))
                {
                    try
                    {
                        c.Rapid.Stop(StopMode.Immediate);
                        m.Release();
                        result.Add("[msg]    Program Stop");
                    }
                    catch (Exception ex)
                    {
                        result.Add("[error]" + ex.ToString());
                        m.Release();
                        return -1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                result.Add("[error]" + ex.ToString());
                return -1;
            }
        }
        public int SetMotorsOn(Controller c, out List<string> result)
        {
            result = new List<string>();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            if (c.State != ControllerState.MotorsOff)
            {
                result.Add("[warning]    Current state is not MotorsOff.Now is " + c.State.ToString());
                return -1;
            }
            else
            {
                try
                {
                    c.State = ControllerState.MotorsOn;
                    result.Add("[msg]    SetMotorsON");
                    return 0;
                }
                catch (Exception ex)
                {
                    result.Add("[error]" + ex.ToString());
                    return -1;
                }
            }
        }

        public int SetMotorsOff(Controller c, out List<string> result)
        {
            result = new List<string>();
            c.Logon(ABB.Robotics.Controllers.UserInfo.DefaultUser);
            if (c.State != ControllerState.MotorsOn)
            {
                result.Add("[warning]    Current controller state is not MotorsOn.Now is " + c.State.ToString());
                return -1;
            }
            else
            {
                if (c.Rapid.ExecutionStatus != ExecutionStatus.Stopped)
                {
                    result.Add("[error]    Rapid is executing or is in unknow state");
                    return -1;
                }
                try
                {
                    c.State = ControllerState.MotorsOff;
                    result.Add("[msg]    SetMotorsOFF");
                    return 0;
                }
                catch (Exception ex)
                {
                    result.Add("[error]" + ex.ToString());
                    return -1;
                }
            }
        }
    }
}
