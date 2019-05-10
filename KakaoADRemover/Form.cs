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

namespace KakaoADRemover
{
    public partial class Form : System.Windows.Forms.Form
    {
        Logger log = new Logger();

        private string kakaoPath = "";
        private IntPtr kakaoWnd;
        
        /*
         * KaKao Ad Remover :: for windows
         * 
         * 0. hide 
         * 1. find kakao things
         * 2. catch & kill
         * 3. self destruct
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

            // goto Step 1
            openKakaoRegistryLocation();
        }



        // Kakaotalk의 kakaopen Key를 읽어옴.
        private string openKakaoRegistryLocation()
        {
            string kakaoReg = null;
            string kakaoPath = null;
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
