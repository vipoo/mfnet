/* https://msdn.microsoft.com/en-us/library/windows/desktop/dd369146%28v=vs.85%29.aspx
 */

using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFTranscodeSinkInfoProviderTest
    {
        public void DoTests()
        {
            IMFActivate a;
            int hr;

            hr = MFExtern.MFCreateTranscodeSinkActivate(out a);
            MFError.ThrowExceptionForHR(hr);

            IMFTranscodeSinkInfoProvider sip = (IMFTranscodeSinkInfoProvider)a;

            ///////////////////////////////
            IMFTranscodeProfile tp;
            hr = MFExtern.MFCreateTranscodeProfile(out tp);
            MFError.ThrowExceptionForHR(hr);

            IMFAttributes tpa;
            hr = MFExtern.MFCreateAttributes(out tpa, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = tpa.SetGUID(MFAttributesClsid.MF_TRANSCODE_CONTAINERTYPE, MFTranscodeContainerType.MPEG4);
            MFError.ThrowExceptionForHR(hr);

            hr = tp.SetContainerAttributes(tpa);
            MFError.ThrowExceptionForHR(hr);
            ////////////////
            IMFAttributes vid;
            hr = MFExtern.MFCreateAttributes(out vid, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = vid.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.YV12);
            MFError.ThrowExceptionForHR(hr);

            hr = tp.SetVideoAttributes(vid);
            MFError.ThrowExceptionForHR(hr);
            ////////////////
            ////////////////
            IMFAttributes aud;
            hr = MFExtern.MFCreateAttributes(out aud, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = aud.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.PCM);
            MFError.ThrowExceptionForHR(hr);

            hr = tp.SetAudioAttributes(aud);
            MFError.ThrowExceptionForHR(hr);
            ////////////////

            hr = sip.SetProfile(tp);
            MFError.ThrowExceptionForHR(hr);

            hr = sip.SetOutputFile(@"c:\moo.x");
            MFError.ThrowExceptionForHR(hr);

            MFTranscodeSinkInfo si;
            hr = sip.GetSinkInfo(out si);
            MFError.ThrowExceptionForHR(hr);

            Guid g;
            hr = si.pVideoMediaType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out g);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(g == MFMediaType.YV12);

            hr = sip.SetOutputByteStream(a);
            MFError.ThrowExceptionForHR(hr);
        }
    }
}
