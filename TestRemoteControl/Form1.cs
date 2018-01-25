using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;

namespace TestRemoteControl
{
    public partial class RobotArmController : Form
    {
        ABBController robot;
        List<string> errLog = new List<string>();
       
        public RobotArmController()
        {
            InitializeComponent();//InitUserFace
            robot = new ABBController();
            //robot.Scan();
            this.listView1.Items.Clear();
            robot.Scan();
            ListViewItem item = null;
            if (robot.controllers == null) return;
            for (int i = 0; i < robot.controllers.Count(); i++)
               
            {
                item = new ListViewItem(robot.controllers[i].IPAddress.ToString());
                item.SubItems.Add(robot.controllers[i].Id);
                item.SubItems.Add(robot.controllers[i].Availability.ToString());
                item.SubItems.Add(robot.controllers[i].IsVirtual.ToString());
                item.SubItems.Add(robot.controllers[i].SystemName);
                item.SubItems.Add(robot.controllers[i].Version.ToString());
                item.SubItems.Add(robot.controllers[i].ControllerName);
                item.SubItems.Add(robot.GetController(i).OperatingMode.ToString());
                item.SubItems.Add(robot.controllers[i].SystemId.ToString());
                this.listView1.Items.Add(item);
                item.Tag = robot.controllers[i];
            }
            errLog = robot.errLogger(errLog, "[Scan]             " + robot.controllers.Count().ToString() + "  controllers");
            richTextBox1.Lines=errLog.ToArray();

        }

        private void button1_Click(object sender, EventArgs e)
            //scan button
        {
            //NetworkScanner netscan = new NetworkScanner();
            //netscan.Scan();
            //controllers = netscan.Controllers;
            this.listView1.Items.Clear();
            robot.Scan();
            ListViewItem item = null;
            if (robot.controllers == null) return;
            for (int i = 0; i < robot.controllers.Count(); i++) 
            {
                item = new ListViewItem(robot.controllers[i].IPAddress.ToString());
                
                item.SubItems.Add(robot.controllers[i].Id);
                item.SubItems.Add(robot.controllers[i].Availability.ToString());
                item.SubItems.Add(robot.controllers[i].IsVirtual.ToString());
                item.SubItems.Add(robot.controllers[i].SystemName);
                item.SubItems.Add(robot.controllers[i].Version.ToString());
                item.SubItems.Add(robot.controllers[i].ControllerName);
                //item = new ListViewItem(robot.controllers[i].SystemId.ToString());//Guid SystemId
                item.SubItems.Add(robot.GetController(i).OperatingMode.ToString());
                item.SubItems.Add(robot.controllers[i].SystemId.ToString());
                //item.SubItems.Add(robot.controllers[i].ControllerName);//PP to Main
                this.listView1.Items.Add(item);
                item.Tag = robot.controllers[i];
            }
            errLog = robot.errLogger(errLog, "[Scan]             " + robot.controllers.Count().ToString() + "  controllers");
            richTextBox1.Lines = errLog.ToArray();
        }

        private void button2_Click(object sender, EventArgs e) //motorON
        {
            try
            {
                List<string> result = new List<string>();
                if (this.listView1.SelectedIndices.Count < 1)
                {
                    MessageBox.Show("Please select at least 1 controller to send command.");
                    return;
                }
                for (int i = 0; i < this.listView1.SelectedIndices.Count; i++)
                {
                    robot.PPtoMain(robot.GetController(i), out result);
                    errLog = robot.errLogger(errLog, "[PP_To_Main]" + result[0]);
                    errLog = robot.errLogger(errLog, "[PP_To_Main]:       "+robot.controllers[i].ControllerName+" Done.");
                    richTextBox1.Lines = errLog.ToArray();

                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

            errLog = robot.errLogger(errLog, "[PP_To_Main]       all set done.");
            richTextBox1.Lines = errLog.ToArray();
        }

        private void button5_Click(object sender, EventArgs e)
            //motors on 
        {
            try
            {
                List<string> result = new List<string>();
                if (this.listView1.SelectedIndices.Count < 1)
                {
                    MessageBox.Show("Please select at least 1 controller to send command.");
                    return;
                }
                for (int i = 0; i < this.listView1.SelectedIndices.Count; i++)
                {
                    robot.SetMotorsOn(robot.GetController(i), out result);
                    errLog = robot.errLogger(errLog, "[Motors_ON]" + result[0]);

                    errLog = robot.errLogger(errLog, "[Motors_ON]       :  " + robot.controllers[i].ControllerName+ " Done.");
                    richTextBox1.Lines = errLog.ToArray();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
            errLog = robot.errLogger(errLog, "[Motors_ON]        all set done.");
            richTextBox1.Lines = errLog.ToArray();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            try
            {
                List<string> result = new List<string>();
                if (this.listView1.SelectedIndices.Count < 1)
                {
                    MessageBox.Show("Please select at least 1 controller to send command.");
                    return;
                }
                for (int i = 0; i < this.listView1.SelectedIndices.Count; i++)
                {
                    robot.SetMotorsOff(robot.GetController(i), out result);
                    errLog = robot.errLogger(errLog, "[Motors_OFF]" + result[0]);

                    errLog = robot.errLogger(errLog, "[Motors_OFF]:     " + robot.controllers[i].ControllerName+ " Done.");
                    richTextBox1.Lines = errLog.ToArray();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
            errLog = robot.errLogger(errLog, "[Motors_OFF]       all set done.");
            richTextBox1.Lines = errLog.ToArray();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> result = new List<string>();
                if (this.listView1.SelectedIndices.Count < 1)
                {
                    MessageBox.Show("Please select at least 1 controller to send command.");
                    return;
                }
                for (int i = 0; i < this.listView1.SelectedIndices.Count; i++)
                {
                    robot.Start(robot.GetController(i), out result);
                    errLog = robot.errLogger(errLog, "[Program_Start]" + result[0]);

                    errLog = robot.errLogger(errLog, "[Program_Start]:  " + robot.controllers[i].ControllerName+ " Done.");
                    richTextBox1.Lines = errLog.ToArray();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
            errLog = robot.errLogger(errLog, "[Program_Start]    all set done.");
            richTextBox1.Lines = errLog.ToArray();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> result = new List<string>();
                if (this.listView1.SelectedIndices.Count < 1)
                {
                    MessageBox.Show("Please select at least 1 controller to send command.");
                    return;
                }
                for (int i = 0; i < this.listView1.SelectedIndices.Count; i++)
                {
                    robot.Stop(robot.GetController(i), out result);
                    errLog = robot.errLogger(errLog, "[Program_Stop]" + result[0]);

                    errLog = robot.errLogger(errLog, "[Program_Stop]:   " + robot.controllers[i].ControllerName+ " Done.");
                    richTextBox1.Lines = errLog.ToArray();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
            errLog = robot.errLogger(errLog, "[Program_Stop]     all set done.");
            richTextBox1.Lines = errLog.ToArray();
        }



        private void button7_Click_1(object sender, EventArgs e)
        {

            listView1.MultiSelect = true;
            foreach (ListViewItem item in listView1.Items)
            {
                item.Selected = true;
                item.BackColor = SystemColors.Highlight;
            }
        }

        private void button7_Click_2(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            //move to cursor position
           richTextBox1.ScrollToCaret();

            this.CheckKeyword("[Scan]", Color.Purple, 0);
            this.CheckKeyword("[Motors_ON]", Color.Green, 0);
            this.CheckKeyword("Motors_OFF", Color.Green, 0);
            this.CheckKeyword("[PP_To_Main]", Color.Green, 0);
            this.CheckKeyword("[Program_Start]", Color.Green, 0);
            this.CheckKeyword("[Program_Stop]", Color.Green, 0);
            this.CheckKeyword("[error", Color.DarkRed, 0);
            this.CheckKeyword("[msg]", Color.Green, 0);
            this.CheckKeyword("[warning]", Color.Tomato, 0);




    }

        private void button8_Click(object sender, EventArgs e)
        {
              System.String logPath = "./"+ DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss").ToString()+"_log.txt";
            //System.String logPath = "./log.txt";
            File.WriteAllLines(logPath, this.errLog);
            errLog = robot.errLogger(errLog, "Log Save done.");
            richTextBox1.Lines = errLog.ToArray();

        }

        private void CheckKeyword(string word, Color color, int startIndex)
        {
            if (this.richTextBox1.Text.Contains(word))
            {
                int index = -1;
                int selectStart = this.richTextBox1.SelectionStart;

                while ((index = this.richTextBox1.Text.IndexOf(word, (index + 1))) != -1)
                {
                    this.richTextBox1.Select((index + startIndex), word.Length);
                    this.richTextBox1.SelectionColor = color;
                   this.richTextBox1.Select(selectStart, 0);
                    this.richTextBox1.SelectionColor = Color.Black;
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            errLog = robot.errLogger(errLog, "[e-mail]          send 2018_xx_xx_xx_log.txt to  yu.qiao@intplusplus.com.");
            richTextBox1.Lines = errLog.ToArray();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
