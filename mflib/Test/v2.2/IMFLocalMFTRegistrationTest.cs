using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.Transform;
using System.Runtime.InteropServices;

namespace Testv22
{
    class IMFLocalMFTRegistrationTest
    {
        public void DoTests()
        {
            int hr;

            IMFActivate act;
            IMFMediaSession ms;
            hr = MFExtern.MFCreatePMPMediaSession(MFPMPSessionCreationFlags.None, null, out ms, out act);

            IMFGetService gs = (IMFGetService)ms;
            object o;
            hr = gs.GetService(MFServices.MF_LOCAL_MFT_REGISTRATION_SERVICE, typeof(IMFLocalMFTRegistration).GUID, out o);

            IMFLocalMFTRegistration lmr = (IMFLocalMFTRegistration)o;

            MFT_REGISTRATION_INFO []mfr = new MFT_REGISTRATION_INFO[2];

            Guid g1 = Guid.NewGuid();
            mfr[0] = new MFT_REGISTRATION_INFO();
            mfr[0].clsid = g1;
            mfr[0].guidCategory = MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT;
            mfr[0].uiFlags = MFT_EnumFlag.AsyncMFT;
            mfr[0].pszName = "boo";

            Guid g2 = Guid.NewGuid();
            mfr[1] = new MFT_REGISTRATION_INFO();
            mfr[1].clsid = g2;
            mfr[1].guidCategory = MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT;
            mfr[1].uiFlags = MFT_EnumFlag.AsyncMFT;
            mfr[1].pszName = "moo";

            hr = lmr.RegisterMFTs(mfr, mfr.Length);
            MFError.ThrowExceptionForHR(hr);

            // If the structs passed correctly, we should be able to read them back with MFTEnumEx
            int i;
            IMFActivate[] ia;
            hr = MFExtern.MFTEnumEx(MFTransformCategory.MFT_CATEGORY_VIDEO_EFFECT, MFT_EnumFlag.AsyncMFT | MFT_EnumFlag.LocalMFT, null, null, out ia, out i);
            MFError.ThrowExceptionForHR(hr);

            int stat = 0;
            for (int x = 0; x < i; x++)
            {
                Guid g3;
                hr = ia[x].GetGUID(MFAttributesClsid.MFT_TRANSFORM_CLSID_Attribute, out g3);
                MFError.ThrowExceptionForHR(hr);

                if (g3 == g1)
                    stat |= 1;
                else if (g3 == g2)
                    stat |= 2;
            }

            Debug.Assert(stat == 3);
        }
    }
}
