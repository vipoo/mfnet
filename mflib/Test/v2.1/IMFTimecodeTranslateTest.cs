/* https://msdn.microsoft.com/en-us/library/windows/desktop/ee663577%28v=vs.85%29.aspx
 * For presentation time-to-time code translation, the ASF file must 
 * contain a Simple Index Object; for time code-to-presentation time 
 * translation, the ASF file must have an Index Object. 
 * https://msdn.microsoft.com/en-us/library/windows/desktop/dd757932%28v=vs.85%29.aspx
 * asf spec: http://drang.s4.xrea.com/program/tips/id3tag/wmp/10_asf_guids.html
 */

using System;
using System.Text;
using System.Threading;
using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    public class IMFTimecodeTranslateTest : IMFAsyncCallback
    {
        /*
         * 
         * BeginConvertHNSToTimecode Starts an asynchronous call to convert time in 100-nanosecond units to SMPTE time code.
 
            BeginConvertTimecodeToHNS Starts an asynchronous call to convert SMPTE time code to 100-nanosecond units.
 
            EndConvertHNSToTimecode Completes an asynchronous request to convert time in 100-nanosecond units to SMPTE time code.
 
            EndConvertTimecodeToHNS Completes an asynchronous request to convert time in SMPTE time code to 100-nanosecond units.
 

         * */

        public void DoTests()
        {
            //TestBeginConvertHNSToTimecode(@"C:\Users\Public\Videos\Sample Videos\Wildlife.wmv");
            TestBeginConvertHNSToTimecode(@"C:\sourceforge\mflib\test\media\smpte.wmv");
        }

        public void TestBeginConvertHNSToTimecode(string sFileName)
        {
            IMFSourceResolver source;
            int hr = MFExtern.MFCreateSourceResolver(out source);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType objType;

            object obj;
            hr = source.CreateObjectFromURL(sFileName, MFResolution.MediaSource, null, out objType, out obj);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaSource src = obj as IMFMediaSource;
            object timecode;
            hr = MFExtern.MFGetService(src, MFServices.MF_TIMECODE_SERVICE, typeof(IMFTimecodeTranslate).GUID, out timecode);
            MFError.ThrowExceptionForHR(hr);

            translate = (IMFTimecodeTranslate)timecode;
            bDone = false;
            hr = translate.BeginConvertHNSToTimecode(1990000, this, null);
            MFError.ThrowExceptionForHR(hr);

            while (!bDone)
                Thread.Sleep(100);

            bDone = false;

            PropVariant pv = new PropVariant((Int64)3);
            hr = translate.BeginConvertTimecodeToHNS(pv, this, this);
            MFError.ThrowExceptionForHR(hr);

            while (!bDone)
                Thread.Sleep(100);
        }

        private IMFTimecodeTranslate translate;
        private bool bDone;

        int IMFAsyncCallback.GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            //throw new NotImplementedException();
            pdwFlags = MFASync.None;
            pdwQueue = MFAsyncCallbackQueue.Standard;
            return 0;
        }

        int IMFAsyncCallback.Invoke(IMFAsyncResult pAsyncResult)
        {
            object o;
            int hr;

            hr = pAsyncResult.GetState(out o);
            //MFError.ThrowExceptionForHR(hr);

            if (o == null)
            {
                PropVariant var = new PropVariant();

                hr = translate.EndConvertHNSToTimecode(pAsyncResult, var);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(var.GetVariantType() == ConstPropVariant.VariantType.Int64 && var.GetLong() == 3);
            }
            else
            {
                long t;

                hr = translate.EndConvertTimecodeToHNS(pAsyncResult, out t);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(t == 1990000);
            }


            bDone = true;

            return hr;
        }
    }
}
