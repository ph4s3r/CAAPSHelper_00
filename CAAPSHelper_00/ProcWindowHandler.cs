using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAAPSHelper_00
{
    public class ProcWindowHandler : ProcHandler
    {

        public ProcWindowHandler(string in_procname, string in_windowtitle) : base (in_procname, in_windowtitle)
        {
            this.cl_handle = getHandleByWindowTitleAndProcName(in_procname, in_windowtitle);
        }

        public struct Rect
        {
            public int Left { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int Top { get; set; }
        }

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public void bringToFront()
        {
            try
            {
                SetForegroundWindow(this.getHandleByPID());
            }
            catch
            {
                //MessageBox.Show("SetForegroundWindow: Process Inactive", "Baj van Houston! ", MessageBoxButtons.OK);
            }
        }

        public Rect GetWRect()
        {

            Rect WinRect = new Rect();

            try
            {
                GetWindowRect(this.getHandleByPID(), ref WinRect);
            }
            catch
            {
                //MessageBox.Show("GetWRect: Process Inactive", "Baj van Houston! ", MessageBoxButtons.OK);
            }
            return WinRect;
        }

        public void SetWRect(int x, int y, int w, int h)
        {
            try
            {
                MoveWindow(this.getHandleByPID(), x, y, w, h, true);
            }
            catch
            {
               //MessageBox.Show("SetWRect: Process Inactive", "Baj van Houston! ", MessageBoxButtons.OK);
            }
        }
    }
}
