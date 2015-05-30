using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaFoundation;
using MediaFoundation.EVR;
using System.Runtime.InteropServices.ComTypes;
using MediaFoundation.Misc;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using MediaFoundation.Transform;
using System.Threading;

namespace Testv30
{
    class ExternTest : COMBase, IMFActivate
    {
        public void DoTests()
        {
            int hr;

            QueueStuff();

            IMFByteStream imfByteStreamOut = null;
            IMFMediaType imfMediaTypeIn = null;
            IMFMediaType imfMediaTypeOut = null;
            IMFMediaSink imfMediaSinkOut = null;
            Guid g = Guid.Empty;
            Guid g2 = Guid.Empty;
            Guid gOut = Guid.Empty;
            IMFAttributes imfAttributesIn = null;
            object objectOut = null;
            int intOut = 0;
            IMFMediaSource imfMediaSourceIn = null;
            IMFTranscodeProfile imfTranscodeProfileIn = null;
            IMFTopology imfTopologyOut = null;
            IMFTrackedSample imfTrackedSampleOut = null;
            IStream iStreamOut = null;
            IntPtr intPtrIn = IntPtr.Zero;
            IMFDXGIDeviceManager imfDXGIDeviceManagerOut = null;
            IMFMediaBuffer imfMediaBufferOut = null;

            IMFByteStream imfByteStreamWritable = null;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.ReadWrite, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, "output.wmv", out imfByteStreamWritable);
            MFError.ThrowExceptionForHR(hr);

            /////////////////////////////
            hr = MFExtern.MFCreateMediaType(out imfMediaTypeIn);
            MFError.ThrowExceptionForHR(hr);
            hr = imfMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Audio);
            MFError.ThrowExceptionForHR(hr);
            hr = imfMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.MFAudioFormat_Dolby_AC3);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateAC3MediaSink(
                imfByteStreamWritable,
                imfMediaTypeIn,
                out imfMediaSinkOut
            );
            MFError.ThrowExceptionForHR(hr);

            ////////////////////////////
            hr = imfMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.AAC);
            MFError.ThrowExceptionForHR(hr);
            hr = imfMediaTypeIn.SetUINT32(MFAttributesClsid.MF_MT_AAC_PAYLOAD_TYPE, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateADTSMediaSink(
                imfByteStreamWritable,
                imfMediaTypeIn,
                out imfMediaSinkOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateTrackedSample(
                out imfTrackedSampleOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateStreamOnMFByteStream(
                imfByteStreamWritable,
                out iStreamOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFAllocateSerialWorkQueue(
                1,
                out intOut
            );
            MFError.ThrowExceptionForHR(hr);

            int pdwTaskId = 0;
            int pID = 0;

            hr = MFExtern.MFLockSharedWorkQueue(
                "audio",
                7,
                ref pdwTaskId,
                out pID
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFGetWorkQueueMMCSSPriority(
                pID,
                out intOut
            );
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(intOut == 7);

            int dx9 = MFExtern.MFMapDXGIFormatToDX9Format(2);

            int dxgi = MFExtern.MFMapDX9FormatToDXGIFormat(dx9);

            Debug.Assert(dxgi == 2);

            hr = MFExtern.MFLockDXGIDeviceManager(
                out intOut,
                out imfDXGIDeviceManagerOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFUnlockDXGIDeviceManager();
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateVideoSampleAllocatorEx(
                typeof(IMFVideoSampleAllocator).GUID,
                out objectOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateDXGIDeviceManager(
                out intOut,
                out imfDXGIDeviceManagerOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFRegisterLocalSchemeHandler(
                "moo",
                this
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFRegisterLocalByteStreamHandler(
                ".wav",
                null,
                this
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateMFByteStreamWrapper(
                imfByteStreamWritable,
                out imfByteStreamOut
            );
            MFError.ThrowExceptionForHR(hr);

            Guid gKey = new Guid("{9F41D4A8-5B4B-4183-923C-FC1BFF3BD886}");
            Guid gClsid = new Guid("{68A367A5-A8E1-43B9-8D27-371CC3544D50}");

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Media Foundation\ContentProtectionSystems", true);
            key = key.CreateSubKey(gKey.ToString("B"));
            key.SetValue("CLSID", gClsid.ToString("B"));
            key.Close();

            hr = MFExtern.MFGetContentProtectionSystemCLSID(
                new Guid(gKey.ToString("B")),
                out gOut
            );
            MFError.ThrowExceptionForHR(hr);

            Debug.Assert(gOut == gClsid);

            hr = MFExtern.MFCreate2DMediaBuffer(800, 600, 116, true, out imfMediaBufferOut);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateMediaBufferFromMediaType(imfMediaTypeIn, 10000, 10000 * 4, 0, out imfMediaBufferOut);
            MFError.ThrowExceptionForHR(hr);

            object o;
            hr = MFExtern.MFCreateMediaExtensionActivate(gKey.ToString("B"), null, typeof(IMFActivate).GUID, out o);
            MFError.ThrowExceptionForHR(hr);

            CreateTranscodeProfile(out imfTranscodeProfileIn);
            CreateMediaSource(Program.File1, out imfMediaSourceIn);

            hr = MFExtern.MFCreateTranscodeTopologyFromByteStream(
                imfMediaSourceIn,
                imfByteStreamWritable,
                imfTranscodeProfileIn,
                out imfTopologyOut
            );
            MFError.ThrowExceptionForHR(hr);

            // Since imfMediaTypeIn is an audio type, this is creating an
            // AudioEncodingProperties.  That object supports these
            // interfaces:

            // {00000000-0000-0000-C000-000000000046} IUnknown
            // {00000038-0000-0000-C000-000000000046} IWeakReferenceSource
            // {62BC7A16-005C-4B3B-8A0B-0A090E9687F3} IAudioEncodingProperties
            // {98f10d79-13ea-49ff-be70-2673db69702c} IAudioEncodingPropertiesWithFormatUserData
            // {AF86E2E0-B12D-4c6a-9C5A-D7AA65101E90} IInspectable
            // {B4002AF6-ACD4-4E5A-A24B-5D7498A8B8C4} IMediaEncodingProperties

            Guid iae = new Guid("{62BC7A16-005C-4B3B-8A0B-0A090E9687F3}");

            hr = MFExtern.MFCreatePropertiesFromMediaType(
                imfMediaTypeIn,
                iae,
                out objectOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateMediaTypeFromProperties(
                objectOut,
                out imfMediaTypeOut
            );
            MFError.ThrowExceptionForHR(hr);

            // {0000000c-0000-0000-C000-000000000046}
            // {905A0FE1-BC53-11DF-8C49-001E4FC686DA}
            // {905A0FE2-BC53-11DF-8C49-001E4FC686DA}
            // {905A0FE6-BC53-11DF-8C49-001E4FC686DA}
            // {CC254827-4B3D-438F-9232-10C76BC7E038}

            Guid IID_IRandomAccessStream = new Guid("905A0FE1-BC53-11DF-8C49-001E4FC686DA");

            hr = MFExtern.MFCreateStreamOnMFByteStreamEx(
                imfByteStreamWritable,
                IID_IRandomAccessStream,
                out objectOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateMFByteStreamOnStreamEx(
                objectOut,
                out imfByteStreamOut
            );
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFCreateMuxSink(
                MFMediaType.MPEG2Program,
                imfAttributesIn,
                imfByteStreamWritable,
                out imfMediaSinkOut
            );
            MFError.ThrowExceptionForHR(hr);

            MP4Sink();

        }

        void MP4Sink()
        {
            IMFMediaType imfMediaTypeIn;
            int hr;

            hr = MFExtern.MFCreateMediaType(out imfMediaTypeIn);
            imfMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_MAJOR_TYPE, MFMediaType.Video);
            imfMediaTypeIn.SetGUID(MFAttributesClsid.MF_MT_SUBTYPE, MFMediaType.H264);

            imfMediaTypeIn.SetUINT32(MFAttributesClsid.MF_MT_INTERLACE_MODE, (int)(MFVideoInterlaceMode.Progressive));

            imfMediaTypeIn.SetUINT64(MFAttributesClsid.MF_MT_FRAME_SIZE, MFExtern.PackSize(1920, 1080));
            imfMediaTypeIn.SetUINT64(MFAttributesClsid.MF_MT_FRAME_RATE, MFExtern.Pack2UINT32AsUINT64(10, 1)); // The input is a GOP of 10 frames, 1 second long
            imfMediaTypeIn.SetUINT64(MFAttributesClsid.MF_MT_PIXEL_ASPECT_RATIO, MFExtern.PackRatio(1, 1));

            // Initialize media sink and output file/stream:
            string filename = "output.mp4";
            IMFByteStream _byteStream;
            hr = MFExtern.MFCreateFile(MFFileAccessMode.ReadWrite, MFFileOpenMode.DeleteIfExist, MFFileFlags.None, filename, out _byteStream);

            IMFMediaSink _mediaSink;
            hr = MFExtern.MFCreateMPEG4MediaSink(_byteStream, imfMediaTypeIn, null, out _mediaSink);
        }

        void QueueStuff()
        {
            int hr;

            IntPtr intPtrIn = IntPtr.Zero;
            long longOut;

            Something st = new Something();

            hr = MFExtern.MFBeginRegisterWorkQueueWithMMCSSEx(0, "audio", 0, 1, st, st);
            MFError.ThrowExceptionForHR(hr);

            int tid = 0;
            hr = MFExtern.MFRegisterPlatformWithMMCSS("audio", ref tid, 2);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFUnregisterPlatformFromMMCSS();
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFPutWorkItem2(0, 0, st, st);
            MFError.ThrowExceptionForHR(hr);

            IMFAsyncResult ar;
            hr = MFExtern.MFCreateAsyncResult(null, st, null, out ar);
            MFError.ThrowExceptionForHR(hr);

            hr = MFExtern.MFPutWorkItemEx2(0, 0, ar);
            MFError.ThrowExceptionForHR(hr);

            AutoResetEvent are = new AutoResetEvent(false);

            // 'System.Threading.WaitHandle.Handle' is obsolete: 'Use the SafeWaitHandle property instead.'
#pragma warning disable 618
            hr = MFExtern.MFPutWaitingWorkItem(are.Handle, 0, ar, out longOut);
            MFError.ThrowExceptionForHR(hr);
#pragma warning disable 

            System.Threading.Thread.Sleep(2000);
            Debug.Assert(st.iCount == 3);
        }

        void Untestable()
        {
#if false

            Guid IID_ID3D11Texture2D = new Guid(0x6f15aaf2, 0xd208, 0x4e89, 0x9a, 0xb4, 0x48, 0x95, 0x35, 0xd3, 0x4f, 0x9c);
            hr = MFExtern.MFCreateDXGISurfaceBuffer(
                IID_ID3D11Texture2D,
                objectIn,
                intIn,
                boolIn,
                out imfMediaBufferOut
            );

            // Tested in IMFMediaEngineTest
            hr = MFExtern.MFCreateWICBitmapBuffer(
                Guid.Empty, //typeof(IWICBitmap).GUID,
                null,
                out imfMediaBufferOut
            );

            hr = MFExtern.MFCreateProtectedEnvironmentAccess(
                out imfProtectedEnvironmentAccessOut
            );

            hr = MFExtern.MFLoadSignedLibrary(
                stringIn,
                out imfSignedLibraryOut
            );

            hr = MFExtern.MFGetSystemId(
                out imfSystemIdOut
            );
#endif
        }

        #region Helpers

        int CreateTranscodeProfile(out IMFTranscodeProfile pProfile)
        {
            IMFCollection pAvailableTypes = null;  // List of audio media types.
            IMFMediaType pAudioType = null;       // Audio media type.
            IMFAttributes pAudioAttrs = null;      // Copy of the audio media type.
            IMFAttributes pContainer = null;       // Container attributes.

            int dwMTCount = 0;

            // Create an empty transcode profile.
            int hr = MFExtern.MFCreateTranscodeProfile(out pProfile);
            MFError.ThrowExceptionForHR(hr);

            // Get output media types for the Windows Media audio encoder.

            // Enumerate all codecs except for codecs with field-of-use restrictions.
            // Sort the results.

            hr = MFExtern.MFTranscodeGetAudioOutputAvailableTypes(MFMediaType.WMAudioV9, (MFT_EnumFlag.All & (~ MFT_EnumFlag.FieldOfUse)) | MFT_EnumFlag.SortAndFilter, null, out pAvailableTypes);
            MFError.ThrowExceptionForHR(hr);

            hr = pAvailableTypes.GetElementCount(out dwMTCount);
            MFError.ThrowExceptionForHR(hr);
            if (dwMTCount == 0)
            {
                MFError.ThrowExceptionForHR(-1);
            }

            // Get the first audio type in the collection and make a copy.
            object o;
            hr = pAvailableTypes.GetElement(0, out o);
            MFError.ThrowExceptionForHR(hr);

            pAudioType = o as IMFMediaType;

            hr = MFExtern.MFCreateAttributes(out pAudioAttrs, 0);
            MFError.ThrowExceptionForHR(hr);

            hr = pAudioType.CopyAllItems(pAudioAttrs);
            MFError.ThrowExceptionForHR(hr);

            // Set the audio attributes on the profile.
            hr = pProfile.SetAudioAttributes(pAudioAttrs);
            MFError.ThrowExceptionForHR(hr);

            // Set the container attributes.
            hr = MFExtern.MFCreateAttributes(out pContainer, 1);
            MFError.ThrowExceptionForHR(hr);

            hr = pContainer.SetGUID(MFAttributesClsid.MF_TRANSCODE_CONTAINERTYPE, MFTranscodeContainerType.ASF);
            MFError.ThrowExceptionForHR(hr);

            hr = pProfile.SetContainerAttributes(pContainer);
            MFError.ThrowExceptionForHR(hr);

            return hr;
        }

        private void CreateMediaSource(string pszURL, out IMFMediaSource ppSource)
        {
            IMFSourceResolver pSourceResolver = null;
            object pSource;

            int hr;
            // Create the source resolver.
            hr = MFExtern.MFCreateSourceResolver(out pSourceResolver);
            MFError.ThrowExceptionForHR(hr);

            MFObjectType ObjectType;
            hr = pSourceResolver.CreateObjectFromURL(
                pszURL,
                MFResolution.MediaSource,
                null,
                out ObjectType,
                out pSource);
            MFError.ThrowExceptionForHR(hr);

            ppSource = (IMFMediaSource)pSource;

            //done:
            //SafeRelease(&pSourceResolver);
            //SafeRelease(&pSource);
            //return hr;
        }

        #endregion

        #region IMFActivate

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

        public int ActivateObject(Guid riid, out object ppv)
        {
            throw new NotImplementedException();
        }

        public int ShutdownObject()
        {
            throw new NotImplementedException();
        }

        public int DetachObject()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class Something : COMBase, IMFAsyncCallback
    {
        public int iCount = 0;
        public int GetParameters(out MFASync pdwFlags, out MFAsyncCallbackQueue pdwQueue)
        {
            pdwFlags = 0;
            pdwQueue = MFAsyncCallbackQueue.Undefined;
            return E_NotImplemented;
        }

        public int Invoke(IMFAsyncResult pAsyncResult)
        {
            iCount++;
            return 0;
        }
    }
}
