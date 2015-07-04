/****************************************************************************
While the underlying library is covered by LGPL or BSD, this sample is released
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

// c:\Windows\Microsoft.NET\Framework64\v2.0.50727\regasm /tlb /codebase WavSource.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Utils
{
    public class xLog : IDisposable
    {
#if DEBUG
        static int iCount = 0;
        private TextWriter tw;
        private string m_Name;
#endif

        public xLog(string s)
        {
#if DEBUG
            const string sPath = @"C:\sourceforge\mflib\samples";
            string fn = string.Format("{0}\\{1}.txt", sPath, s);
            m_Name = s;
            tw = new StreamWriter(fn);
#endif
        }

#if DEBUG

        ~xLog()
        {
            if (tw != null)
            {
                try
                {
                    tw.Close();
                }
                catch { }

                tw = null;
            }

        }

#endif

        public void WriteLine(string s)
        {
#if DEBUG
            string w = string.Format("{0:000} {1:00} {2} {3}::{4}", iCount++, System.Threading.Thread.CurrentThread.ManagedThreadId, System.Threading.Thread.CurrentThread.GetApartmentState(), m_Name, s);
            tw.WriteLine(w);
            tw.Flush();
#endif
        }

        #region IDisposable Members

        public void Dispose()
        {
#if DEBUG
            if (tw != null)
            {
                tw.WriteLine("Closing");
                tw.Close();
                tw = null;
            }
#endif
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
