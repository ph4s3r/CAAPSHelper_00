using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAAPSHelper_00
{
    public partial class Form1 : Form
    {

        bool _monitorButtonActive = true;

        ProcWindowHandler myPWH = null;

        Helper myHelper = null;

        const string myProcname = "pcsws";

        IProcSearcher myPS = new ProcSearcher();

        PrivateFontCollection pfc = new PrivateFontCollection();

        List<string> myWindowtitles = new List<string>();

        //PCSWS target Screen Rect: x,y,w,h
        int[] myWindow_Rect_Selected = { 20, 30, 800, 500 };

        /// <summary>
        /// //EMBEDDING FONTS
        /// </summary>
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
          IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection MYpfc = new PrivateFontCollection();

        /// <summary>
        /// //Form Initialize - Set Label Fonts
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            
            try
            {
                unsafe
                {
                    fixed (byte* pFontData = Properties.Resources.WIPEOUTF)
                    {
                        uint dummy = 0;
                        MYpfc.AddMemoryFont((IntPtr)pFontData, Properties.Resources.WIPEOUTF.Length);
                        AddFontMemResourceEx((IntPtr)pFontData, (uint)Properties.Resources.WIPEOUTF.Length, IntPtr.Zero, ref dummy);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Font does not correctly appear");
            }

            Font WipeoutF9pt = new Font(MYpfc.Families[0], 9);
            Font WipeoutF8pt = new Font(MYpfc.Families[0], 8);
            Font WipeoutF7pt = new Font(MYpfc.Families[0], 7);

            titlelabel.Font = WipeoutF9pt;
            label3.Font = WipeoutF8pt;
            label18.Font = WipeoutF8pt;
            label16.Font = WipeoutF8pt;
            label4.Font = WipeoutF8pt;
            loaderLabel.Font = WipeoutF8pt;
            attachlabel.Font = WipeoutF8pt;
            loadButton.Font = WipeoutF7pt;
            monitorButton.Font = WipeoutF7pt;
            helperButton.Font = WipeoutF7pt;
            
        }
        

        /// <summary>
        /// //Get Handle from window title
        /// </summary>
        private void monitorButton_Click(object sender, EventArgs e)
        {
            if (procWindowBox.SelectedItem != null && procWindowBox.SelectedItem.ToString() != "")
            {
                //IDE EGY DISPOSE KELL!!!
                myPWH = null;

                //IDE EGY DISPOSE KELL!!!
                myPWH = new ProcWindowHandler(myProcname, procWindowBox.SelectedItem.ToString());
                if (myPWH.doWeHaveTheHandle() == true)
                {
                    attachlabel.Text = "";
                    myPWH.Signal += myProcMon1_Signal;
                    myPWH.StartMonitor();
                }
                else
                {
                    attachlabel.Text = "unable to attach";
                }
            }
            else
            {
                attachlabel.Text = "invalid window title selected";
            }
        }

        /// <summary>
        /// Handle Exited event and display process information. 
        /// </summary>
        void myProcMon1_Signal(bool isActive)
        {
            
            if (isActive)
            {
                try
                {
                    label18.Invoke((MethodInvoker)(() => label18.Text = "Active"));
                    if (_monitorButtonActive == true)
                    {
                        monitorButton.Invoke((MethodInvoker)(() => monitorButton.Enabled = false));
                        _monitorButtonActive = false;

                    }
                }
                catch
                {
                    //throw new NotImplementedException();
                }
            }
            else
            {
                try
                {
                    label18.Invoke((MethodInvoker)(() => label18.Text = "Inactive"));
                    if (_monitorButtonActive == false)
                    {
                        monitorButton.Invoke((MethodInvoker)(() => monitorButton.Enabled = true));
                        _monitorButtonActive = true;
                    }
                }
                catch
                {
                    //throw new NotImplementedException();
                }
            }
        }


        /// <summary>
        /// Load PCSWS window titles to combobox
        /// </summary>
        private void loadButton_Click(object sender, EventArgs e)
        {
            procWindowBox.Items.Clear();
            myWindowtitles = myPS.getWTitlesByProcName(myProcname);

            if (myWindowtitles.Count > 0)
            {
                procWindowBox.Enabled = true;
                foreach (string p in myWindowtitles)
                {
                    procWindowBox.Items.Add(p);
                }
                procWindowBox.SelectedItem = procWindowBox.Items[0];
                loaderLabel.Text = "Select window title";
                loadButton.Text = "Reload pcsws window titles";
            }
            else
            {
                loaderLabel.Text = "No process with name " + myProcname;
                procWindowBox.Enabled = false;
            }
        }

        /// <summary>
        /// Start helper: new Helper object, new thread: tempmatch & action
        /// </summary>
        private void helperButton_Click(object sender, EventArgs e)
        {

            myHelper = new Helper(myPWH);

            myHelper.StartHelperThread();

            //on match, input
        }
    }
}
