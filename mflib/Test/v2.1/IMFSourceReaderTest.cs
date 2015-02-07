using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.ReadWrite;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFSourceReaderTest
    {
        const int i = 1;
        IMFSourceReader rdr;

        public void DoTests()
        {
            Init();

            TestSel();
            TestType();
            TestRead();
            TestService();
        }

        void TestSel()
        {
            int hr;
            bool b;

            hr = rdr.SetStreamSelection(i, false);
            MFError.ThrowExceptionForHR(hr);

            hr = rdr.GetStreamSelection(i, out b);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(b == false);

            hr = rdr.SetStreamSelection(i, true);
            MFError.ThrowExceptionForHR(hr);

            hr = rdr.GetStreamSelection(i, out b);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(b == true);
        }

        void TestType()
        {
            int hr;
            bool b;

            IMFMediaType mt;
            hr = rdr.GetCurrentMediaType(i, out mt);
            MFError.ThrowExceptionForHR(hr);

            int c;
            hr = mt.GetCount(out c);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(c != 0);

            hr = rdr.SetCurrentMediaType(i, IntPtr.Zero, mt);
            MFError.ThrowExceptionForHR(hr);

            IMFMediaType mt2;
            hr = rdr.GetNativeMediaType(0, 0, out mt2);
            MFError.ThrowExceptionForHR(hr);

            hr = mt2.Compare(mt, MFAttributesMatchType.AllItems, out b);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(b == false);
        }

        void TestRead()
        {
            int hr;

            hr = rdr.Flush(i);
            MFError.ThrowExceptionForHR(hr);

            PropVariant pv = new PropVariant((long)32240000);
            hr = rdr.SetCurrentPosition(Guid.Empty, pv);
            MFError.ThrowExceptionForHR(hr);

            int ai;
            MF_SOURCE_READER_FLAG sf;
            long ts;
            IMFSample samp;

            // While you might expect ts to be ~the same value we set in SetCurrentPosition, it isn't.  This
            // is the same as what happens in c++, so I guess it's right.

            hr = rdr.ReadSample(0, MF_SOURCE_READER_CONTROL_FLAG.None, out ai, out sf, out ts, out samp);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(ai == 0);
            Debug.Assert(sf == MF_SOURCE_READER_FLAG.None);
            Debug.Assert(ts == 0);
            Debug.Assert(samp != null);

            hr = rdr.GetPresentationAttribute((int)MF_SOURCE_READER.MediaSource, MFAttributesClsid.MF_SOURCE_READER_MEDIASOURCE_CHARACTERISTICS, pv);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert((uint)pv == (uint)(MFMediaSourceCharacteristics.CanPause | MFMediaSourceCharacteristics.CanSeek));
        }

        void TestService()
        {
            int hr;
            int c;
            object obj1;

            hr = rdr.GetServiceForStream((int)MF_SOURCE_READER.MediaSource, Guid.Empty, typeof(IPropertyStore).GUID, out obj1);
            MFError.ThrowExceptionForHR(hr);

            IPropertyStore ips1 = (IPropertyStore)obj1;

            hr = ips1.GetCount(out c);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(c == 0);

            object obj2;
            Guid gUnk = new Guid("00000000-0000-0000-C000-000000000046");
            hr = rdr.GetServiceForStream((int)MF_SOURCE_READER.MediaSource, MFServices.MFNETSOURCE_STATISTICS_SERVICE, gUnk, out obj2);
            MFError.ThrowExceptionForHR(hr);

            IPropertyStore ips2 = (IPropertyStore)obj2;

            hr = ips2.GetCount(out c);
            MFError.ThrowExceptionForHR(hr);
            Debug.Assert(c != 0);
        }

        void Init()
        {
            int hr;

            IMFMediaSource ms;
            object pSource;
            IMFSourceResolver sr;
            MFObjectType pObjectType;

            hr = MFExtern.MFCreateSourceResolver(out sr);
            MFError.ThrowExceptionForHR(hr);

            hr = sr.CreateObjectFromURL(
                @"file://C:/Users/Public/Videos/Sample Videos/Wildlife.wmv",
                MFResolution.MediaSource,
                null,
                out pObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            ms = pSource as IMFMediaSource;

            hr = MFExtern.MFCreateSourceReaderFromMediaSource(ms, null, out rdr);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
