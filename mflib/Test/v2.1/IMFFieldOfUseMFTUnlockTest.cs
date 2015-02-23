using System;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;
using MediaFoundation.Transform;
using Microsoft.Win32;

namespace Testv21
{
    class IMFFieldOfUseMFTUnlockTest : IMFFieldOfUseMFTUnlock
    {
        private bool m_Done = false;
        private Guid m_MFTGuid = new Guid("{5B91187F-3C42-409e-A8C9-7F637708D724}"); // delay mft
        private int m_Flags = (int)(MFT_EnumFlag.SyncMFT | MFT_EnumFlag.FieldOfUse);

        public void DoTests()
        {
            RegistryKey rk = Registry.ClassesRoot.OpenSubKey(@"MediaFoundation\Transforms\" + m_MFTGuid.ToString("D"), true);
            if (rk == null)
                throw new Exception("You must build (and regsvr) mft_audiodelay for the current platform (probably x86, even if on x64)");

            // Temporarily set flags to include FieldOfUse to trigger IMFFieldOfUseMFTUnlock
            rk.SetValue("MFTFlags", m_Flags, RegistryValueKind.DWord);

            try
            {
                IMFActivate ia;

                // Create a blank Activate
                int hr = MFExtern.MFCreateTransformActivate(out ia);
                MFError.ThrowExceptionForHR(hr);

                // Set the clsid
                hr = ia.SetGUID(MFAttributesClsid.MFT_TRANSFORM_CLSID_Attribute, this.m_MFTGuid);
                MFError.ThrowExceptionForHR(hr);

                // Set the flags
                hr = ia.SetUINT32(MFAttributesClsid.MF_TRANSFORM_FLAGS_Attribute, m_Flags);
                MFError.ThrowExceptionForHR(hr);

                // Tell it that on unlock, it should call the current object
                hr = ia.SetUnknown(MFAttributesClsid.MFT_FIELDOFUSE_UNLOCK_Attribute, this);
                MFError.ThrowExceptionForHR(hr);

                // It also needs to know the category
                hr = ia.SetGUID(MFAttributesClsid.MF_TRANSFORM_CATEGORY_Attribute, MFTransformCategory.MFT_CATEGORY_AUDIO_EFFECT);
                MFError.ThrowExceptionForHR(hr);

                // Should trigger a call to Unlock (below)
                object o;
                hr = ia.ActivateObject(typeof(IMFTransform).GUID, out o);
                MFError.ThrowExceptionForHR(hr);
            }
            finally
            {
                // Reset to normal
                rk.DeleteValue("MFTFlags");
            }

            Debug.Assert(m_Done);
        }

        public int Unlock(object pUnkMFT)
        {
            IMFAttributes pa;
            Guid g;

            // We should get passed a pointer to the transform.  Make sure it's the
            // one we expect.
            IMFTransform a = (IMFTransform)pUnkMFT;

            int hr = a.GetAttributes(out pa);
            MFError.ThrowExceptionForHR(hr);

            hr = pa.GetGUID(MFAttributesClsid.MFT_TRANSFORM_CLSID_Attribute, out g);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(g == m_MFTGuid);

            m_Done = true;

            return hr;
        }
    }
}
