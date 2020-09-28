using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAAPSHelper_00
{
    public class ProcHandler : ProcMonitor
    {
        protected IntPtr cl_handle;

        protected string cl_procname, cl_windowtitle;

        protected ProcHandler(string in_procname, string in_windowtitle) : base ()
        {
            this.cl_handle = getHandleByWindowTitleAndProcName(in_procname, in_windowtitle);
        }

        public bool doWeHaveTheHandle()
        {
            if (cl_handle == IntPtr.Zero) return false;
            else return true;
        }

        public IntPtr getHandleByPID()
        {
            //Main window handle
            IntPtr myhandle = IntPtr.Zero;

                try
                {
                    Process myProc = Process.GetProcessById(cl_PID);

                    while (myProc.MainWindowHandle == IntPtr.Zero)
                    {
                        // Discard cached information about the process
                        // because MainWindowHandle might be cached.
                        myProc.Refresh();

                        System.Threading.Thread.Sleep(100);
                    }

                    myhandle = myProc.MainWindowHandle;

                }
                catch (Exception e)
                {
                    // The process has probably exited,
                    // so accessing MainWindowHandle threw an exception
                    MessageBox.Show("GetHandler: " + e.ToString(), "Unable to get process handle! ", MessageBoxButtons.OK);
                }

            return myhandle;

        }

        public IntPtr getHandleByWindowTitleAndProcName(string procname, string windowTitle)
        {

            //Main window handle
            IntPtr handle = IntPtr.Zero;

            try
            {
                Process[] localByName = Process.GetProcessesByName(procname);

                foreach (Process p in localByName)
                {
                    if (p.MainWindowTitle == windowTitle)
                    {
                        Process myProc = p;
                        while (myProc.MainWindowHandle == IntPtr.Zero)
                        {
                            // Discard cached information about the process
                            // because MainWindowHandle might be cached.
                            myProc.Refresh();

                            System.Threading.Thread.Sleep(100);
                        }

                        handle = myProc.MainWindowHandle;
                        this.cl_PID = myProc.Id;
                    }

                }

            }
            catch (Exception e)
            {
                // The process has probably exited,
                // so accessing MainWindowHandle threw an exception
                MessageBox.Show("Cannot Find Process!", "Baj van Houston! ", MessageBoxButtons.OK);
            }
            return handle;
        }
    }
}
