using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAAPSHelper_00
{
    class ProcSearcher : IProcSearcher
    {
        public List<string> getWTitlesByProcName(string in_procname)
        {
            Process[] my_locals = Process.GetProcessesByName(in_procname);

            List<string> out_wnames = new List<string>();

                if (my_locals.Length == 0)
                {
                    return out_wnames;
                }
                else
                {    
                    foreach (Process p in my_locals)
                    {
                        out_wnames.Add(p.MainWindowTitle);
                    }
                    return out_wnames;
                }
        }


        public List<string> getAllProcessNames()
        {
            Process[] my_locals = Process.GetProcesses();

            List<string> out_procnames = new List<string>();

            if (my_locals.Length == 0)
            {
                return null;
            }
            else
            {
                foreach (Process p in my_locals)
                {
                    out_procnames.Add(p.ProcessName);
                }
                return out_procnames;
            }
        }
    }
}
