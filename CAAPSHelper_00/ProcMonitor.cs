using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CAAPSHelper_00
{
    public class ProcMonitor
    {
        protected int cl_PID = 0;

        protected Thread cl_t = null;

        protected ProcMonitor()
        {

        }

        public int getPID()
        {
            return this.cl_PID;
        }

        public delegate void SignalHandler(bool isActive);

        //SIGNAL EVENT with a type of SIGNALHANDLER
        public event SignalHandler Signal;

        public void StartMonitor()
        {
            if (cl_t != null)
            {
                if (cl_t.ThreadState.ToString() != "Running")
                {
                    cl_t = new Thread(ActivityChecker);
                    cl_t.Start();
                }
                else
                {
                    int i = 1;
                }
            }
            else
            {
                cl_t = new Thread(ActivityChecker);
                cl_t.Start();
            }
        }

        private void ActivityChecker()
        {
            try
            {
                Process myProc = Process.GetProcessById(this.cl_PID);

                while (true)
                {
                    // Discard cached information about the process
                    // because MainWindowHandle might be cached.
                    myProc.Refresh();

                    if (myProc.HasExited)
                    {
                        //Event: process on PID x is INACTIVE
                        Signal(false);
                        if (cl_t.IsAlive)
                        {
                            cl_t.Abort();
                        }
                    }
                    else
                    {
                        //Event: process on PID x is ACTIVE
                        Signal(true);
                    }

                    Thread.Sleep(2000);
                }
            }
            catch(Exception ex)
            {
                Signal(false);
                if (cl_t.IsAlive)
                {
                    cl_t.Abort();
                }
                //MessageBox.Show("Thread error:    " + ex);
            }

        }
    }
}
