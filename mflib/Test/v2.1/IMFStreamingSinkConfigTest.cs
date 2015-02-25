using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFStreamingSinkConfigTest : COMBase, IMFByteStream
    {
        public void DoTests()
        {
            int hr;
            IMFMediaSink ms;

            hr = MFExtern.MFCreateMP3MediaSink(this, out ms);
            MFError.ThrowExceptionForHR(hr);

            IMFStreamingSinkConfig ssc = (IMFStreamingSinkConfig)ms;

            hr = ssc.StartStreaming(true, 12);
            MFError.ThrowExceptionForHR(hr);
        }

        public int GetCapabilities(out MFByteStreamCapabilities pdwCapabilities)
        {
            throw new NotImplementedException();
        }

        public int GetLength(out long pqwLength)
        {
            throw new NotImplementedException();
        }

        public int SetLength(long qwLength)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentPosition(out long pqwPosition)
        {
            throw new NotImplementedException();
        }

        public int SetCurrentPosition(long qwPosition)
        {
            throw new NotImplementedException();
        }

        public int IsEndOfStream(out bool pfEndOfStream)
        {
            throw new NotImplementedException();
        }

        public int Read(IntPtr pb, int cb, out int pcbRead)
        {
            throw new NotImplementedException();
        }

        public int BeginRead(IntPtr pb, int cb, IMFAsyncCallback pCallback, object pUnkState)
        {
            throw new NotImplementedException();
        }

        public int EndRead(IMFAsyncResult pResult, out int pcbRead)
        {
            throw new NotImplementedException();
        }

        public int Write(IntPtr pb, int cb, out int pcbWritten)
        {
            throw new NotImplementedException();
        }

        public int BeginWrite(IntPtr pb, int cb, IMFAsyncCallback pCallback, object pUnkState)
        {
            throw new NotImplementedException();
        }

        public int EndWrite(IMFAsyncResult pResult, out int pcbWritten)
        {
            throw new NotImplementedException();
        }

        public int Seek(MFByteStreamSeekOrigin SeekOrigin, long llSeekOffset, MFByteStreamSeekingFlags dwSeekFlags, out long pqwCurrentPosition)
        {
            throw new NotImplementedException();
        }

        public int Flush()
        {
            throw new NotImplementedException();
        }

        public int Close()
        {
            throw new NotImplementedException();
        }
    }
}
