using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Testv22
{
    // The goal of IMFObjectReferenceStream is to extend IStream
    // so that it can write (specific types of) IUnknown.  'boo'
    // is my IStream class.  It read/writes to a byte[].
    class IMFObjectReferenceStreamTest 
    {
        public void DoTests()
        {
            int hr;

            // Create an IMFAttributes
            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 1);
            MFError.ThrowExceptionForHR(hr);

            // Populate an IUnknown on IMFAttributes
            Guid gn = Guid.NewGuid();

            orsTest before = new orsTest(17);

            hr = ia.SetUnknown(gn, before);
            MFError.ThrowExceptionForHR(hr);

            // Create a stream to write to
            IStream str = new boo();

            // Write ia to str
            hr = MFExtern.MFSerializeAttributesToStream(ia, MFAttributeSerializeOptions.UnknownByRef, str);
            MFError.ThrowExceptionForHR(hr);

            // Create a second IMFAttributes
            IMFAttributes ia2;
            hr = MFExtern.MFCreateAttributes(out ia2, 1);
            MFError.ThrowExceptionForHR(hr);

            // Try to read the data back into the new IMFAttributes
            hr = MFExtern.MFDeserializeAttributesFromStream(ia2, MFAttributeSerializeOptions.UnknownByRef, str);
            MFError.ThrowExceptionForHR(hr);

            Guid iunk = new Guid("00000000-0000-0000-C000-000000000046");
            object io;
            hr = ia2.GetUnknown(gn, iunk, out io);
            MFError.ThrowExceptionForHR(hr);

            orsTest o2 = io as orsTest;
            Debug.Assert(o2 != null); // Should be an orsTest
            Debug.Assert(o2 != before); // Should be a *different* orsTest
            Debug.Assert(o2.GetValue() == before.GetValue()); // But it should have the same value
        }
    }

    class orsTest : COMBase
    {
        int TestMe = 0;

        public orsTest(int v)
        {
            TestMe = v;
        }

        public int GetValue()
        {
            return TestMe;
        }

        public int GetValue(out int i)
        {
            i = TestMe;
            return S_Ok;
        }
    }

    class boo : COMBase, IStream, IMFObjectReferenceStream
    {
        byte[] b = new byte[100];
        int isize = 0;
        int ipos = 0;

        public void Clone(out IStream ppstm)
        {
            throw new NotImplementedException();
        }

        public void Commit(int grfCommitFlags)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotImplementedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            int doit = Math.Min(cb, isize - ipos);
            for (int x = 0; x < doit; x++)
            {
                pv[x] = b[x+ipos];
            }
            ipos += doit;

            if (pcbRead != IntPtr.Zero)
                Marshal.WriteInt32(pcbRead, doit);
        }

        public void Revert()
        {
            throw new NotImplementedException();
        }

        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            throw new NotImplementedException();
        }

        public void SetSize(long libNewSize)
        {
            throw new NotImplementedException();
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            throw new NotImplementedException();
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            for (int x = 0; x < cb; x++)
            {
                b[isize + x] = pv[x];
            }
            isize += cb;

            if (pcbWritten != IntPtr.Zero)
                Marshal.WriteInt32(pcbWritten, cb);
        }

        public int SaveReference(Guid riid, object pUnk)
        {
            orsTest x = pUnk as orsTest;
            byte[] b;
            int y = x.GetValue();
            b = BitConverter.GetBytes(y);
            Write(b, b.Length, IntPtr.Zero);
            return S_Ok;
        }

        public int LoadReference(Guid riid, out object ppv)
        {
            byte[] b = new byte[4];
            Read(b, 4, IntPtr.Zero);
            int y = BitConverter.ToInt32(b, 0);
            orsTest t = new orsTest(y);

            ppv = t;
            return S_Ok;
        }
    }
}
