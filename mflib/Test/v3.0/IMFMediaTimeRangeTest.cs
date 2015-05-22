using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv30
{
    class IMFMediaTimeRangeTest
    {
        public void DoTests()
        {
            IMFMediaEngineClassFactory mecf = new MFMediaEngineClassFactory() as IMFMediaEngineClassFactory;
            Debug.Assert(mecf != null);

            int hr;
            IMFMediaTimeRange tr;

            hr = mecf.CreateTimeRange(out tr);
            MFError.ThrowExceptionForHR(hr);

            hr = tr.AddRange(100000, 200000);
            MFError.ThrowExceptionForHR(hr);

            hr = tr.AddRange(800000, 900000);
            MFError.ThrowExceptionForHR(hr);

            int iLen = tr.GetLength();

            Debug.Assert(iLen == 2);

            double st;
            hr = tr.GetStart(0, out st);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(st == 100000);

            hr = tr.GetEnd(0, out st);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(st == 200000);

            bool b = tr.ContainsTime(850000);

            Debug.Assert(b);

            b = tr.ContainsTime(250);

            Debug.Assert(!b);

            hr = tr.Clear();
            MFError.ThrowExceptionForHR(hr);

            iLen = tr.GetLength();

            Debug.Assert(iLen == 0);
        }
    }
}
