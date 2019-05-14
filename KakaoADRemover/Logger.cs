using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KakaoADRemover
{
    class Logger
    {
        private string path = "";
        private DirectoryInfo di = null;

        public Logger()
        {
            path = Directory.GetCurrentDirectory() + @"\Log";
            di = new DirectoryInfo(path);

            if (!di.Exists)
            {
                try
                {
                    di.Create();
                }
                catch (UnauthorizedAccessException uae)
                {
                    MessageBox.Show("폴더 생성 권한이 없습니다. 관리자 권한으로 실행바랍니다.", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void makeFolder(DateTime datetime)
        {
            path = Directory.GetCurrentDirectory() + @"\Log\" + datetime.ToString("yyyy") + @"\" + datetime.ToString("MM") + @"\" + datetime.ToString("dd");
            di = new DirectoryInfo(path);

            if (!di.Exists)
            {
                try
                {
                    di.Create();
                }
                catch (UnauthorizedAccessException uae)
                {
                    MessageBox.Show("폴더 생성 권한이 없습니다. 관리자 권한으로 실행바랍니다.", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Log_Query(string queryString, DataTable dt)
        {
            DateTime time = DateTime.Now;
            string datetime = time.ToString("yyyyMMddHHmmss");
            makeFolder(time);
            try
            {
                StreamWriter sw = File.AppendText(path + @"\query_" + datetime + ".txt");
                sw.WriteLine("QueryString : " + queryString);
                if (dt != null)
                {
                    sw.WriteLine("-------------------------------");
                    sw.WriteLine("DataTable.Rows.Count : " + dt.Rows.Count);
                    sw.WriteLine("-------------------------------");
                    foreach (DataColumn col in dt.Columns)
                    {
                        sw.Write(" {0,-15} ", col.ColumnName);
                        if (dt.Columns.IndexOf(col) == dt.Columns.Count - 1)
                        {
                            sw.Write("|");
                        }
                    }
                    sw.WriteLine("- - - - - - - - - - - - - - - -");
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            sw.Write(" {0,-15} ", row[col]);
                            if (dt.Columns.IndexOf(col) == dt.Columns.Count - 1)
                            {
                                sw.Write("|");
                            }
                        }
                        sw.WriteLine(" ");
                    }
                    sw.WriteLine("-------------------------------");
                }
                sw.Close();

            }
            catch (UnauthorizedAccessException uae)
            {
                MessageBox.Show("파일 생성/수정 권한이 없습니다. 관리자 권한으로 실행바랍니다.", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Write Log on "\Log" folder.
        /// </summary>
        /// <param name="msg">Messages</param>
        public void Log(string msg)
        {
            DateTime time = DateTime.Now;
            string datetime = time.ToString("yyyyMMddHHmmss");
            makeFolder(time);
            try
            {
                StreamWriter sw = File.AppendText(path + @"\" + datetime + ".txt");
                sw.WriteLine("Defined Message : ( " + msg + " )");
                sw.WriteLine("Message Time : " + time.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.Close();

            }
            catch (UnauthorizedAccessException uae)
            {
                MessageBox.Show("파일 생성/수정 권한이 없습니다. 관리자 권한으로 실행바랍니다.", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Write Log on "\Log" folder.
        /// </summary>
        /// <param name="ex">Exception</param>
        public void Log(Exception ex)
        {
            DateTime time = DateTime.Now;
            string datetime = time.ToString("yyyyMMddHHmmss");
            makeFolder(time);
            try
            {
                StreamWriter sw = File.AppendText(path + @"\" + datetime + ".txt");
                sw.WriteLine("Log : ( " + ex.InnerException + " )");
                sw.WriteLine("-------------------------------");
                sw.WriteLine("Line : " + Convert.ToInt32(ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '))));
                sw.WriteLine("Message : " + ex.Message);
                sw.WriteLine("Message Time : " + time.ToString("yyyy-MM-dd HH:mm:ss"));
                sw.WriteLine("Target Site : " + ex.TargetSite);
                sw.WriteLine("StackTrace : " + ex.StackTrace);
                sw.WriteLine("-------------------------------");
                sw.Close();

            }
            catch (UnauthorizedAccessException uae)
            {
                MessageBox.Show("파일 생성/수정 권한이 없습니다. 관리자 권한으로 실행바랍니다.", "UnauthorizedAccessException", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Show Alert Messagebox only.
        /// </summary>
        /// <param name="ex">Exception</param>
        public void Alert(Exception ex)
        {
            string msg = ex.Message;
            string title = "Alert ! " + ex.InnerException;

            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Show Custom Alert Messagebox only.
        /// </summary>
        /// <param name="ex">Exception</param>
        public void Alert(string msg)
        {
            string title = "Alert ! " + msg;

            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }


        /// <summary>
        /// Write the log and show alert messagebox.
        /// </summary>
        /// <param name="ex">Exception</param>
        public void Log_n_Alert(Exception ex)
        {
            Log(ex);
            Alert(ex);
        }

        /// <summary>
        /// Write the custom log and show alert messagebox.
        /// </summary>
        /// <param name="ex">Exception</param>
        public void Log_n_Alert(string msg)
        {
            Log(msg);
            Alert(msg);
        }

        /// <summary>
        /// Show messagebox with given message.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="alertmsg">Alert message to show</param>
        public void Alert_Msg(Exception ex, string alertmsg)
        {
            string title = "Alert ! " + ex.InnerException;
            MessageBox.Show(alertmsg, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
