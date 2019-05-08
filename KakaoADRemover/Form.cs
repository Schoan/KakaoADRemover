using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        // Step 0 :: Hide
        private void Form_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
