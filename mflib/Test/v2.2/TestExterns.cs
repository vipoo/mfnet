using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using MediaFoundation.EVR;
using MediaFoundation.ReadWrite;

namespace Testv22
{
    class TestExterns : COMBase, IMFAttributes
    {
        Guid ng1 = Guid.NewGuid();
        Guid ng2 = Guid.NewGuid();
        PropVariant m_pv1;
        PropVariant m_pv2;
        int m_state = 0;

        public void DoTests()
        {
            TestPV();
            TestCoCreate();
        }

        private void TestCoCreate()
        {
            object o;
            int x = 0;

            try
            {
                o = new EnhancedVideoRenderer();
                x++;
            }
            catch
            {
                Console.WriteLine("EnhancedVideoRenderer");
            }
            try
            {
                o = new MFVideoMixer9();
                x++;
            }
            catch
            {
                Console.WriteLine("MFVideoMixer9");
            }
            try
            {
                o = new MFVideoPresenter9();
                x++;
            }
            catch
            {
                Console.WriteLine("MFVideoPresenter9");
            }
            try
            {
                o = new EVRTearlessWindowPresenter9();
                x++;
            }
            catch
            {
                Console.WriteLine("EVRTearlessWindowPresenter9");
            }

            try
            {
                o = new MFCaptureEngine();//w8
                x++;
            }
            catch
            {
                Console.WriteLine("MFCaptureEngine");
            }
            try
            {
                o = new MFCaptureEngineClassFactory();//w8
                x++;
            }
            catch
            {
                Console.WriteLine("MFCaptureEngineClassFactory");
            }
            try
            {
                o = new CColorConvertDMO();
                x++;
            }
            catch
            {
                Console.WriteLine("CColorConvertDMO");
            }
            try
            {
                o = new PlayToSourceClassFactory();//w8
                x++;
            }
            catch
            {
                Console.WriteLine("PlayToSourceClassFactory");
            }
            try
            {
                o = new MFMediaSharingEngineClassFactory();//w8
                x++;
            }
            catch
            {
                Console.WriteLine("MFMediaSharingEngineClassFactory");
            }
            try
            {
                o = new MFMediaEngineClassFactory();//w8
                x++;
            }
            catch
            {
                Console.WriteLine("MFMediaEngineClassFactory");
            }
            try
            {
                o = new HttpSchemePlugin();
                x++;
            }
            catch
            {
                Console.WriteLine("HttpSchemePlugin");
            }
            try
            {
                o = new NetSchemePlugin();
                x++;
            }
            catch
            {
                Console.WriteLine("NetSchemePlugin");
            }
            try
            {
                o = new MPEG2ByteStreamPlugin();//w8
                x++;
            }
            catch
            {
                Console.WriteLine("MPEG2ByteStreamPlugin");
            }
            try
            {
                o = new MFByteStreamProxyClassFactory();//w8
                x++;
            }
            catch
            {
                Console.WriteLine("MFByteStreamProxyClassFactory");
            }
            try
            {
                o = new UrlmonSchemePlugin();
                x++;
            }
            catch
            {
                Console.WriteLine("UrlmonSchemePlugin");
            }

            try
            {
                o = new MPEG2DLNASink();
                x++;
            }
            catch
            {
                Console.WriteLine("MPEG2DLNASink");
            }

            try
            {
                o = new MFReadWriteClassFactory();
                x++;
            }
            catch
            {
                Console.WriteLine("MFReadWriteClassFactory");
            }
            Console.WriteLine(x);
        }

        private void TestPV()
        {
            int hr;
            IMFAttributes ia = this;
            IStream str = new coo() as IStream;
            m_pv1 = new PropVariant((UInt64)5432);
            m_pv2 = new PropVariant((UInt64)2345);

            hr = MFExtern.MFSerializeAttributesToStream(ia, MFAttributeSerializeOptions.None, str);
            Marshal.ThrowExceptionForHR(hr);

            IMFAttributes ia2;
            hr = MFExtern.MFCreateAttributes(out ia2, 6);
            Marshal.ThrowExceptionForHR(hr);

            hr = ia2.SetItem(ng1, m_pv1);
            Marshal.ThrowExceptionForHR(hr);
            m_state |= 2;  // We are testing "Managed calling unmanaged."

            PropVariant pv3 = new PropVariant();
            hr = ia2.GetItem(ng1, pv3);
            Marshal.ThrowExceptionForHR(hr);

            Guid ng3;
            hr = ia2.GetItemByIndex(0, out ng3, null);
            Marshal.ThrowExceptionForHR(hr);
            m_state |= 4; // We are verifying that the marshaller doesn't get
                          // called for nulls (set BPs in PVMarshaler)

            Debug.Assert(m_state == 7);
        }

        public int GetItem(Guid guidKey, PropVariant pValue)
        {
            throw new NotImplementedException();
        }

        public int GetItemType(Guid guidKey, out MFAttributeType pType)
        {
            throw new NotImplementedException();
        }

        public int CompareItem(Guid guidKey, ConstPropVariant Value, out bool pbResult)
        {
            throw new NotImplementedException();
        }

        public int Compare(IMFAttributes pTheirs, MFAttributesMatchType MatchType, out bool pbResult)
        {
            throw new NotImplementedException();
        }

        public int GetUINT32(Guid guidKey, out int punValue)
        {
            throw new NotImplementedException();
        }

        public int GetUINT64(Guid guidKey, out long punValue)
        {
            throw new NotImplementedException();
        }

        public int GetDouble(Guid guidKey, out double pfValue)
        {
            throw new NotImplementedException();
        }

        public int GetGUID(Guid guidKey, out Guid pguidValue)
        {
            throw new NotImplementedException();
        }

        public int GetStringLength(Guid guidKey, out int pcchLength)
        {
            throw new NotImplementedException();
        }

        public int GetString(Guid guidKey, StringBuilder pwszValue, int cchBufSize, out int pcchLength)
        {
            throw new NotImplementedException();
        }

        public int GetAllocatedString(Guid guidKey, out string ppwszValue, out int pcchLength)
        {
            throw new NotImplementedException();
        }

        public int GetBlobSize(Guid guidKey, out int pcbBlobSize)
        {
            throw new NotImplementedException();
        }

        public int GetBlob(Guid guidKey, byte[] pBuf, int cbBufSize, out int pcbBlobSize)
        {
            throw new NotImplementedException();
        }

        public int GetAllocatedBlob(Guid guidKey, out IntPtr ip, out int pcbSize)
        {
            throw new NotImplementedException();
        }

        public int GetUnknown(Guid guidKey, Guid riid, out object ppv)
        {
            throw new NotImplementedException();
        }

        public int SetItem(Guid guidKey, ConstPropVariant Value)
        {
            throw new NotImplementedException();
        }

        public int DeleteItem(Guid guidKey)
        {
            throw new NotImplementedException();
        }

        public int DeleteAllItems()
        {
            return S_Ok;
        }

        public int SetUINT32(Guid guidKey, int unValue)
        {
            throw new NotImplementedException();
        }

        public int SetUINT64(Guid guidKey, long unValue)
        {
            if (guidKey == ng1)
                Debug.Assert(unValue == 5432);
            if (guidKey == ng2)
                Debug.Assert(unValue == 2345);

            return S_Ok;
        }

        public int SetDouble(Guid guidKey, double fValue)
        {
            throw new NotImplementedException();
        }

        public int SetGUID(Guid guidKey, Guid guidValue)
        {
            throw new NotImplementedException();
        }

        public int SetString(Guid guidKey, string wszValue)
        {
            throw new NotImplementedException();
        }

        public int SetBlob(Guid guidKey, byte[] pBuf, int cbBufSize)
        {
            throw new NotImplementedException();
        }

        public int SetUnknown(Guid guidKey, object pUnknown)
        {
            throw new NotImplementedException();
        }

        public int LockStore()
        {
            return S_Ok;
        }

        public int UnlockStore()
        {
            return S_Ok;
        }

        public int GetCount(out int pcItems)
        {
            pcItems = 2;
            return S_Ok;
        }

        public int GetItemByIndex(int unIndex, out Guid pguidKey, PropVariant pValue)
        {
            m_state |= 1; // we are testing "Unmanaged calling managed."

            if (unIndex > 1)
                throw new ArgumentException("Out of range", "unIndex");

            if (unIndex == 0)
            {
                pguidKey = ng1;
                m_pv1.Copy(pValue);
            }
            else
            {
                pguidKey = ng2;
                m_pv2.Copy(pValue);
            }

            return S_Ok;
        }

        public int CopyAllItems(IMFAttributes pDest)
        {
            throw new NotImplementedException();
        }
    }
    class coo : COMBase, IStream
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
                pv[x] = b[x + ipos];
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
    }
}
