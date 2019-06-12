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
using System.Collections;

namespace KakaoADRemover
{
    public partial class Form : System.Windows.Forms.Form
    {
        Logger log = new Logger();

        private string kakaoPath = null;
        private string kakaoReg = null;
        private IntPtr kakaoWnd;
        private List<IntPtr> kakaoWnd_list;
        
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
            kakaoWnd_list = new List<IntPtr>();
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
            
            kakaoWnd = catchKakaoTalk(kakaoPath, kakaoWnd_list);

            if(kakaoWnd == IntPtr.Zero)
            {
                log.Log_n_Alert("KakaoTalk Window Handler Not Found");
                return;
            }

            killKakaoADs(kakaoWnd);
            
            // 2 반복 더이상 카카오톡 없을때까지 //

            if (kakaoWnd != IntPtr.Zero)
            {
                // 정상 종료 시퀀스
                Application.DoEvents();
                Close();
            }
            else
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

        private IntPtr catchKakaoTalk(string kakaotalkPath, List<IntPtr> handle_list)
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

            /*
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
                            handle_list.Add(proc.MainWindowHandle);
                            handle = proc.MainWindowHandle;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log_n_Alert(ex);
                    }
                }

            }*/

            List<WindowsAPI.WindowInfo> procLists = WindowsAPI.GetWindowsProcs();
            foreach (WindowsAPI.WindowInfo proc in procLists)
            {
                if (kakaoEXEname.Equals(proc.ProcessName.ToLower()))
                {
                    try
                    {
                        if (kakaoEXE.FullName.Equals(proc.MainModule.FileName))
                        {
                            guiLog("process catch! : " + proc.MainWindowHandle.ToString());
                            handle_list.Add(proc.MainWindowHandle);
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

        

        public void killKakaoADs(IntPtr kakaoWnd)
        {
            int result = 0b0000;
            //string CLASSNAME_KAKAOROOM = "#32770";
            string CLASSNAME_KAKAOTALK = "EVA_Window_Dblclk"; 
            string CLASSNAME_KAKAOAD = "EVA_Window";
            string CLASSNAME_KAKAOFRIENDSLIST = "EVA_ChildWindow";
            string[] CLASSNAME_AD_STRS = { "FAKE_WND_REACHPOP" }; // if have new ADs, add this. 
            string[] TITLE_KAKAOTALK_STRS = { "카카오톡", "KakaoTalk" };

            try
            {
                IntPtr hWndParent = WindowsAPI.GetWindowLongPtr(kakaoWnd, (int)WindowsAPI.WindowLongFlags.GWLP_HWNDPARENT);
                bool isParentNull = IntPtr.Zero.Equals(hWndParent);
                bool isparentDesktop = WindowsAPI.GetDesktopWindow().Equals(hWndParent);
                bool isNotToolWindow = (WindowsAPI.GetWindowLongPtr(hWndParent, (int)WindowsAPI.WindowLongFlags.GWL_EXSTYLE).ToInt64() 
                                     & (long)WindowsAPI.ExtendedWindowStyles.WS_EX_TOOLWINDOW) 
                                     == 0L ;

                if((isParentNull || isparentDesktop) && isNotToolWindow) 
                {
                    WindowsAPI.WindowInfo mainWndInfo = WindowsAPI.WindowInfo.getInfo(kakaoWnd);

                    bool isKakaoWnd = false;

                    if( CLASSNAME_KAKAOTALK.Equals(mainWndInfo.ClassName) ) // kakao main
                    {
                        foreach (string title in TITLE_KAKAOTALK_STRS)
                        {
                            if (title.Equals(mainWndInfo.Title))
                            {
                                guiLog("Main Window FOUND : " + mainWndInfo.Title);
                                isKakaoWnd = true;

                                break;
                            }
                        }
                    }
                    /*
                    else if ( CLASSNAME_KAKAOROOM.Equals(mainWndInfo.ClassName) ) // kakao chat room
                    {
                        guiLog("Chatting Room FOUND");


                        isKakaoWnd = true;
                    }
                    else // ADs.
                    {
                        foreach (string classname in CLASSNAME_AD_STRS)
                        {
                            if (classname.Equals(mainWndInfo.ClassName))
                            {
                                WindowsAPI.SendMessage(kakaoWnd, (int)WindowsAPI.WindowMessages.WM_CLOSE, 0, null);
                                guiLog("AD CLOSED : " + mainWndInfo.ClassName);
                            }
                        }
                    }*/

                    if(isKakaoWnd)
                    {
                        WindowsAPI.RECT rectKakaoMain = new WindowsAPI.RECT();
                        WindowsAPI.GetWindowRect(kakaoWnd, out rectKakaoMain);
                        IntPtr hwndChildAd = WindowsAPI.FindWindowEx(kakaoWnd, IntPtr.Zero, CLASSNAME_KAKAOAD, null);
                        guiLog("CHILD AD FOUND : " + WindowsAPI.WindowInfo.getInfo(hwndChildAd));

                        if(!IntPtr.Zero.Equals(hwndChildAd))
                        {
                            IntPtr hwndFriendList = WindowsAPI.FindWindowEx(kakaoWnd, IntPtr.Zero, CLASSNAME_KAKAOFRIENDSLIST, null);
                            WindowsAPI.ShowWindow(hwndChildAd, (int)WindowsAPI.ShowWindowCommands.SW_HIDE);
                            WindowsAPI.SetWindowPos(hwndChildAd, WindowsAPI.hWndInsertAfter.HWND_BOTTOM, 0, 0, 0, 0, (int)WindowsAPI.SetWindowsPosFlags.SWP_NOMOVE);
                            WindowsAPI.SetWindowPos(hwndFriendList, WindowsAPI.hWndInsertAfter.HWND_BOTTOM, 0, 0, (rectKakaoMain.Right - rectKakaoMain.Left), (rectKakaoMain.Bottom - rectKakaoMain.Top - 36), (int)WindowsAPI.SetWindowsPosFlags.SWP_NOMOVE);
                        }
                    }
                }
            } catch (Exception e)
            {
                log.Log_n_Alert(e);
            }

            
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
