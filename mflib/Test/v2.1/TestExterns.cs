using System;
using MediaFoundation;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;

using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Testv21
{
    class TestExterns
    {
        string FILENAME = @"c:/sourceforge/mflib/test/media/AspectRatio4x3.wmv";

        public void DoTests()
        {
            Test1();
            Test2();
        }

        private IMFByteStream GetByteStream(IStream isw)
        {
            int hr;
            IMFByteStream bs;

            hr = MFExtern.MFCreateMFByteStreamOnStream(isw, out bs);
            MFError.ThrowExceptionForHR(hr);

            return bs;
        }

        private void Test1()
        {
            int hr;
            IStreamWrapper isw = new IStreamWrapper(FILENAME);

            using (isw)
            {
                IMFByteStream bs = GetByteStream(isw);

                IMFSourceReader sr;
                hr = MFExtern.MFCreateSourceReaderFromByteStream(bs, null, out sr);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(sr != null);
            }
        }
        private void Test2()
        {
            int hr;
            IStreamWrapper isw = new IStreamWrapper(FILENAME);

            using (isw)
            {
                IMFByteStream bs = GetByteStream(isw);

                IMFAttributes attr;
                hr = MFExtern.MFCreateAttributes(out attr, 1);
                MFError.ThrowExceptionForHR(hr);

                hr = attr.SetUINT32(MFAttributesClsid.MF_LOW_LATENCY, 1);
                MFError.ThrowExceptionForHR(hr);

                IMFSourceReader sr;
                hr = MFExtern.MFCreateSourceReaderFromByteStream(bs, attr, out sr);
                MFError.ThrowExceptionForHR(hr);

                Debug.Assert(sr != null);
            }
        }
    }

    public class IStreamWrapper : IStream, IDisposable
    {
        public IStreamWrapper(string file)
        {
            if (file == null)
                throw new ArgumentNullException("stream", "Can't wrap null stream.");
            this.stream = new FileStream(file, FileMode.Open);
        }

        public IStreamWrapper(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream", "Can't wrap null stream.");
            this.stream = stream;
        }

        Stream stream;

        public void Clone(out System.Runtime.InteropServices.ComTypes.IStream ppstm)
        {
            throw new Exception("not implemented");
        }

        public void Commit(int grfCommitFlags) 
        {
            throw new Exception("not implemented");
        }

        public void CopyTo(System.Runtime.InteropServices.ComTypes.IStream pstm,
          long cb, System.IntPtr pcbRead, System.IntPtr pcbWritten)
        {
            throw new Exception("not implemented");
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new Exception("not implemented");
        }

        public void Read(byte[] pv, int cb, System.IntPtr pcbRead)
        {
            if (pcbRead != IntPtr.Zero)
                Marshal.WriteInt64(pcbRead, (Int64)stream.Read(pv, 0, cb));
            else
                stream.Read(pv, 0, cb);
        }

        public void Revert()
        {
            throw new Exception("not implemented");
        }

        public void Seek(long dlibMove, int dwOrigin, System.IntPtr plibNewPosition)
        {
            if (plibNewPosition != IntPtr.Zero)
                Marshal.WriteInt64(plibNewPosition, stream.Seek(dlibMove, (SeekOrigin)dwOrigin));
            else
                stream.Seek(dlibMove, (SeekOrigin)dwOrigin);
        }

        public void SetSize(long libNewSize)
        {
            throw new Exception("not implemented");
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG();

            pstatstg.cbSize = stream.Length;
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new Exception("not implemented");
        }

        public void Write(byte[] pv, int cb, System.IntPtr pcbWritten)
        {
            throw new Exception("not implemented");
        }


        public void Dispose()
        {
            stream.Close();
            stream = null;
        }
    }
}
