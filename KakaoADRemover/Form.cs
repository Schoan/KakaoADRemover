using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace KakaoADRemover
{
    public partial class Form : System.Windows.Forms.Form
    {
        Logger log = new Logger();

        private string kakaoPath = null;
        private string kakaoReg = null;
        private IntPtr kakaoWnd;
        
        /*
         * KaKao Ad Remover :: for windows
         * 
         * 0. hide 
         * 1. find kakao things
         * 2. catch 
         * 3. kill
         * 4. self destruct
         * 
         */

        public Form()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // initialize something?
            textBox.Clear();
        }

        // Step 0 :: Hide
        private void Form_Shown(object sender, EventArgs e)
        {
            //this.Hide();

            guiLog("STEP 0 : HIDE this Program.");
            guiLogLine();

            // step 1
            kakaoReg = openKakaoRegistryLocation();
            if(string.IsNullOrEmpty(kakaoReg))
            {
                log.Log_n_Alert("Kakaotalk Not Found.");
                return;
            }

            // step 2
            kakaoWnd = catchKakaoTalk(kakaoPath);
            if(kakaoWnd == IntPtr.Zero)
            {
                log.Log_n_Alert("KakaoTalk Window Handler Not Found");
                return;
            }


        }



        // Kakaotalk의 kakaopen Key를 읽어옴.
        private string openKakaoRegistryLocation()
        {
            string kakaoReg = null;
            RegistryKey kakaoOpen = null;

            try
            {
                kakaoOpen = Registry.ClassesRoot.OpenSubKey(@"kakaoopen\shell\open\command\", false);

                if (kakaoOpen == null)
                {
                    guiLog("REGISTRY NOT FOUND : KAKAOOPEN");

                    return null;
                }

                kakaoReg = kakaoOpen.GetValue("").ToString();
                guiLog("REGISTRY FOUND : " + kakaoReg);

                string[] temp = kakaoReg.Trim().Split('"');
                foreach (string tmpstr in temp)
                {
                    if(Regex.IsMatch(tmpstr, @"([A-Z]:\\)(.*[\\])(KakaoTalk.exe)"))
                    {
                        kakaoPath = tmpstr;
                        guiLog("KAKOTALK PATH FOUND : " + kakaoPath);

                        break;
                    }
                }

                if(string.IsNullOrEmpty(kakaoPath))
                {
                    guiLog("KAKAOTALK PATH NOT FOUND");

                    return null;
                }
                
            }
            catch (Exception e)
            {
                log.Log("Exception raised during searching registry key.");
                log.Log_n_Alert(e);
            }
            finally
            {
                if(kakaoOpen != null)
                {
                    try
                    {
                        guiLog("CLOSE REGISTY HANDLE");
                        kakaoOpen.Close();
                    }
                    catch (Exception e)
                    {
                        log.Log("Exception raised during close handle after search registry key.");
                        log.Log_n_Alert(e);
                    }
                }
            }

            return kakaoReg;
        }

        private IntPtr catchKakaoTalk(string kakaotalkPath)
        {
            IntPtr handle = IntPtr.Zero;

            if(string.IsNullOrEmpty(kakaotalkPath))
            {
                log.Log_n_Alert("KAKAOTALK PATH NOT FOUND");

                return handle;
            }

            guiLog("CHECK KAKAOTALK PATH IS : " + kakaoPath);
            FileInfo kakaoEXE = new FileInfo(kakaoPath);

            if(!kakaoEXE.Exists)
            {
                log.Log_n_Alert("KAKAOTALK FILE NOT EXIST : " + kakaoPath);

                return handle;
            }

            string kakaoEXEname = kakaoEXE.Name.ToLower();
            if (!kakaoEXEname.Contains(".exe"))
            {
                log.Log_n_Alert("KAKAOTALK PATH IS SOMETHING WRONG : " + kakaoEXEname);

                return handle;
            }
            else
            {
                kakaoEXEname = kakaoEXEname.Substring(0, kakaoEXEname.IndexOf(".exe"));
            }

            Process[] procs = Process.GetProcesses();
            foreach (Process proc in procs)
            {
                if(kakaoEXEname.Equals(proc.ProcessName.ToLower()))
                {
                    try
                    {
                        if(kakaoEXE.FullName.Equals(proc.MainModule.FileName))
                        {
                            guiLog("process catch! : " + proc.MainWindowHandle.ToString());
                            handle = proc.MainWindowHandle;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log_n_Alert(ex);
                    }
                }

            }

            return handle;
        }

        public void guiLog(string msg)
        {
            textBox.Invoke((MethodInvoker)delegate
            {
                textBox.AppendText("\r\n" + msg);
                textBox.ScrollToCaret();
            });
        }

        public void guiLogLine()
        {
            guiLog("---------------------------");
        }
    }
}
