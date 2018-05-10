using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System.IO;


namespace Virtual_WIFI_Csharp
{
    public partial class frmHotspot : Form
    {
        Process newProcess = new Process();
        bool connect = false;
        string oldSSID_oldPass;
        string oldSSID, oldPass;
        string[] readLine;
        public frmHotspot()
        {
           
            InitializeComponent();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtPass.PasswordChar = '\0';
            }
            else 
            {
                txtPass.PasswordChar = '☻';
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                StreamReader sReader = new StreamReader(Application.StartupPath + "\\config.txt");
                oldSSID_oldPass = sReader.ReadLine();
                readLine = oldSSID_oldPass.Split(',');
                oldSSID = readLine[0];
                oldPass = readLine[1];
                sReader.Close();
                txtSSID.Text = oldSSID;
                txtPass.Text = oldPass;
                if (!IsUseAdmin())
                {
                    Process_Start_1();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n Don't worry this file was created by Progam when you was starting hotspot","Warnning",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }


        public static bool IsUseAdmin()
        {
            WindowsIdentity user = WindowsIdentity.GetCurrent();
            WindowsPrincipal principle = new WindowsPrincipal(user);
            return principle.IsInRole(WindowsBuiltInRole.Administrator);

        }

        public void Process_Start_1()
        {
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = true;
            startinfo.CreateNoWindow = true;
            startinfo.WorkingDirectory = Environment .CurrentDirectory;
            startinfo.FileName = System.Windows.Forms.Application.ExecutablePath;
            startinfo.Verb = "runas";

            try 
            {
                Process process = Process.Start(startinfo);
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            System.Windows.Forms.Application.Exit();
        }

        
         private void Play_Stop_Button_Click(object sender, EventArgs e)
         {
             string ssid = txtSSID.Text, pass = txtPass.Text;

             if (!connect)
             {
                 if (String.IsNullOrEmpty(ssid))
                 {
                     MessageBox.Show("Enter SSID ");

                 }
                 else
                 {
                     if (txtPass.Text == null || txtPass.Text == "")
                     {
                         MessageBox.Show("Enter Password");
                     }
                     else
                     {
                         if (txtPass.TextLength >= 8)
                         {
                             Hotspot(ssid, pass, true);
                             txtSSID.Enabled = false;
                             txtPass.Enabled = false;
                             Play_Stop_Button.Text = "Stop";
                             connect = true;
                             try
                             {
                                 //FileStream fstream = new FileStream(Application.StartupPath + "\\config.ini", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                                 StreamWriter sWriter = new StreamWriter(Application.StartupPath + "\\config.txt");
                                 sWriter.WriteLine(ssid + "," + pass);
                                 sWriter.Close();
                             }
                             catch(Exception ex)
                             {
                                 MessageBox.Show(ex.Message);
                             }
                         }
                         else
                         {
                             MessageBox.Show("Password must be larger than 8 character");
                         }
                     }

                 }

             }
             else 
             {
                 Hotspot(null, null, false);
                 txtSSID.Enabled = true;
                 txtPass.Enabled = true;
                 Play_Stop_Button.Text = "Start";
             }

         }

         private void Hotspot(string ssid, string key, bool status)
         {
             ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
             processStartInfo.RedirectStandardInput = true;
             processStartInfo.RedirectStandardOutput = true;
             processStartInfo.CreateNoWindow = true;
             processStartInfo.UseShellExecute = false;
             Process process = Process.Start(processStartInfo);

             if (process != null)
             {
                 if (status)
                 {
                     process.StandardInput.WriteLine("netsh wlan set hostednetwork mode=allow ssid=" + ssid + " key=" + key);
                     process.StandardInput.WriteLine("netsh wlan start hosted network");
                     process.StandardInput.Close();
                 }
                 else
                 {
                     process.StandardInput.WriteLine("netsh wlan stop hostednetwork");
                     process.StandardInput.Close();
                 }
             }
         }

         private void Form1_FormClosed(object sender, FormClosedEventArgs e)
         {
             
         }

         private void Form1_FormClosing(object sender, FormClosingEventArgs e)
         {
             Hotspot(null, null, false);
             Application.Exit();
         }

    }

}
