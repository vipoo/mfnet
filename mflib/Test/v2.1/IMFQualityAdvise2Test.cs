using System;
using System.Text;

using MediaFoundation;
using MediaFoundation.Misc;
using System.Diagnostics;

namespace Testv21
{
    class IMFQualityAdvise2Test : COMBase, IMFMediaEvent
    {
        Guid m_MFTGuid = new Guid("5686a0d9-fe39-409f-9dff-3fdbc849f9f5");
        IMFQualityAdvise2 m_qa;
        bool m_Done = false;

        private void GetInterface()
        {
            IMFActivate ia;

            // Create a blank Activate
            int hr = MFExtern.MFCreateTransformActivate(out ia);
            MFError.ThrowExceptionForHR(hr);

            // Set the clsid
            hr = ia.SetGUID(MFAttributesClsid.MFT_TRANSFORM_CLSID_Attribute, m_MFTGuid);
            MFError.ThrowExceptionForHR(hr);

            // It also needs to know the category
            hr = ia.SetGUID(MFAttributesClsid.MF_TRANSFORM_CATEGORY_Attribute, MFTransformCategory.MFT_CATEGORY_VIDEO_DECODER);
            MFError.ThrowExceptionForHR(hr);

            object o;
            hr = ia.ActivateObject(typeof(IMFQualityAdvise2).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            m_qa = (IMFQualityAdvise2)o;
        }

        public void DoTests()
        {
            int hr;

            GetInterface();

            // Test the IMFQualityAdvise methods, since they used to be untestable

            hr = m_qa.SetDropMode(MFQualityDropMode.Mode2);
            MFError.ThrowExceptionForHR(hr);

            MFQualityDropMode dm;
            hr = m_qa.GetDropMode(out dm);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(dm == MFQualityDropMode.Mode2);

            hr = m_qa.SetQualityLevel(MFQualityLevel.NormalMinus3);
            MFError.ThrowExceptionForHR(hr);

            MFQualityLevel ql;
            hr = m_qa.GetQualityLevel(out ql);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(ql == MFQualityLevel.NormalMinus3);

            // Walking all video decoders, nothing supports this.  Except
            // maybe MPEG-2
            hr = m_qa.DropTime(1);
            if (hr != MFError.MF_E_DROPTIME_NOT_SUPPORTED)
            {
                MFError.ThrowExceptionForHR(hr);
            }

            // Now test the IMFQualityAdvise2 method

            MFQualityAdviseFlags af;
            hr = m_qa.NotifyQualityEvent(this, out af);
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(m_Done);
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
            throw new NotImplementedException();
        }

        public int SetUINT32(Guid guidKey, int unValue)
        {
            throw new NotImplementedException();
        }

        public int SetUINT64(Guid guidKey, long unValue)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int UnlockStore()
        {
            throw new NotImplementedException();
        }

        public int GetCount(out int pcItems)
        {
            throw new NotImplementedException();
        }

        public int GetItemByIndex(int unIndex, out Guid pguidKey, PropVariant pValue)
        {
            throw new NotImplementedException();
        }

        public int CopyAllItems(IMFAttributes pDest)
        {
            throw new NotImplementedException();
        }

        public int GetType(out MediaEventType pmet)
        {
            pmet = 0;
            m_Done = true;
            return COMBase.E_Abort;
        }

        public int GetExtendedType(out Guid pguidExtendedType)
        {
            throw new NotImplementedException();
        }

        public int GetStatus(out int phrStatus)
        {
            throw new NotImplementedException();
        }

        public int GetValue(PropVariant pvValue)
        {
            throw new NotImplementedException();
        }
    }
}
