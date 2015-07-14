using MediaFoundation;
using MediaFoundation.Alt;
using MediaFoundation.Misc;
using MediaFoundation.ReadWrite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Testv31
{
    // untested: OnStreamError
    class IMFSourceReaderCallback2Test : COMBase, IMFSourceReaderCallback2, ICustomQueryInterface
    {
        int m_iDid = 0;

        public void Dotests()
        {
            Init();

            Thread.Sleep(1000);
            Debug.Assert(m_iDid == 3 - 1);
        }
        IMFSourceReader rdr;

        void Init()
        {
            HResult hr;

            IMFMediaSource ms;
            object pSource;
            IMFSourceResolver sr;
            MFObjectType pObjectType;

            hr = MFExtern.MFCreateSourceResolver(out sr);
            MFError.ThrowExceptionForHR(hr);

            hr = sr.CreateObjectFromURL(
                Program.File1,
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            ms = pSource as IMFMediaSource;

            IMFAttributes ia;
            hr = MFExtern.MFCreateAttributes(out ia, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = ia.SetUnknown(MFAttributesClsid.MF_SOURCE_READER_ASYNC_CALLBACK, this);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateSourceReaderFromMediaSource(ms, ia, out rdr);
            MFError.ThrowExceptionForHR(hr);

            int iStream = (int)MF_SOURCE_READER.FirstVideoStream;

            IMFMediaType mt;
            rdr.GetCurrentMediaType(iStream, out mt);
            Debug.WriteLine(MFDump.DumpAttribs(mt));

            IMFMediaType mt2;
            MFExtern.MFCreateMediaType(out mt2);
            mt2.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            mt2.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.NV12);
            hr = rdr.SetCurrentMediaType(iStream, null, mt2);
            Thread.Sleep(1000);

            IMFSourceReaderAsync sr2 = (IMFSourceReaderAsync)rdr;

            hr = sr2.ReadSample(iStream, MF_SOURCE_READER_CONTROL_FLAG.None, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            MFError.ThrowExceptionForHR(hr);

            Thread.Sleep(1000);
        }

        public HResult OnTransformChange()
        {
            m_iDid |= 2;
            return HResult.S_OK;
        }

        public HResult OnStreamError(int dwStreamIndex, HResult hrStatus)
        {
            m_iDid |= 1;
            return HResult.S_OK;
        }

        public HResult OnReadSample(HResult hrStatus, int dwStreamIndex, MF_SOURCE_READER_FLAG dwStreamFlags, long llTimestamp, IMFSample pSample)
        {
            return 0;
        }

        public HResult OnFlush(int dwStreamIndex)
        {
            throw new NotImplementedException();
        }

        public HResult OnEvent(int dwStreamIndex, IMFMediaEvent pEvent)
        {
            throw new NotImplementedException();
        }
        public CustomQueryInterfaceResult GetInterface(ref Guid iid, out IntPtr ppv)
        {
            ppv = IntPtr.Zero;
            if (
                iid != typeof(IMFMediaEventGenerator).GUID &&
                iid != typeof(IMFShutdown).GUID)
            {
                string sGuidString = iid.ToString("B");
                string sKeyName = string.Format("HKEY_CLASSES_ROOT\\Interface\\{0}", sGuidString);
                string sName = (string)Microsoft.Win32.Registry.GetValue(sKeyName, null, sGuidString);

                if (sName == null || sName.Trim().Length == 0)
                    sName = sGuidString.ToString();

                Debug.WriteLine(string.Format("Unhandled interface requested: {0}", sName));
            }
            return CustomQueryInterfaceResult.NotHandled;
        }
    }
}
