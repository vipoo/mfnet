using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaFoundation;
using MediaFoundation.EVR;
using System.Runtime.InteropServices.ComTypes;

namespace Testv30
{
    class ExternTest
    {
        public void DoTests()
        {
            int hr;

            IMFByteStream imfByteStreamIn = null;
            IMFByteStream imfByteStreamOut = null;
            IMFMediaType imfMediaTypeIn = null;
            IMFMediaType imfMediaTypeIn2 = null;
            IMFMediaType imfMediaTypeOut = null;
            IMFMediaSink imfMediaSinkOut = null;
            Guid g = Guid.Empty;
            Guid g2 = Guid.Empty;
            Guid gOut = Guid.Empty;
            IMFAttributes imfAttributesIn = null;
            object objectIn = null;
            object objectOut = null;
            IMFProtectedEnvironmentAccess imfProtectedEnvironmentAccessOut = null;
            string stringIn = null;
            IMFSignedLibrary imfSignedLibraryOut = null;
            IMFSystemId imfSystemIdOut = null;
            int intIn = 0;
            int intOut = 0;
            IMFMediaSource imfMediaSourceIn = null;
            IMFTranscodeProfile imfTranscodeProfileIn = null;
            IMFTopology imfTopologyOut = null;
            IMFTrackedSample imfTrackedSampleOut = null;
            IStream iStreamOut = null;
            IMFAsyncCallback imfAsyncCallbackIn = null;
            IMFAsyncResult imfAsyncResultIn = null;
            IntPtr intPtrIn = IntPtr.Zero;
            long longIn = 0;
            long longOut = 0;
            IMFDXGIDeviceManager imfDXGIDeviceManagerOut = null;
            bool boolIn = false;
            IMFMediaBuffer imfMediaBufferOut = null;
            IMFActivate imfActivateIn = null;

            hr = MFExtern.MFCreateAC3MediaSink(
                imfByteStreamIn,
                imfMediaTypeIn,
                out imfMediaSinkOut
            );

            hr = MFExtern.MFCreateADTSMediaSink(
                imfByteStreamIn,
                imfMediaTypeIn,
                out imfMediaSinkOut
            );

            hr = MFExtern.MFCreateMuxSink(
                g,
                imfAttributesIn,
                imfByteStreamIn,
                out imfMediaSinkOut
            );

            hr = MFExtern.MFCreateFMPEG4MediaSink(
                imfByteStreamIn,
                imfMediaTypeIn,
                imfMediaTypeIn2,
                out imfMediaSinkOut
            );

            hr = MFExtern.MFCreateTranscodeTopologyFromByteStream(
                imfMediaSourceIn,
                imfByteStreamIn,
                imfTranscodeProfileIn,
                out imfTopologyOut
            );

            hr = MFExtern.MFCreateTrackedSample(
                out imfTrackedSampleOut
            );

            hr = MFExtern.MFCreateStreamOnMFByteStream(
                imfByteStreamIn,
                out iStreamOut
            );

            hr = MFExtern.MFCreateMFByteStreamOnStreamEx(
                objectIn,
                out imfByteStreamOut
            );

            hr = MFExtern.MFCreateStreamOnMFByteStreamEx(
                imfByteStreamIn,
                g2,
                out objectOut
            );

            hr = MFExtern.MFCreateMediaTypeFromProperties(
                objectIn,
                out imfMediaTypeOut
            );

            hr = MFExtern.MFCreatePropertiesFromMediaType(
                imfMediaTypeIn,
                g2,
                out objectOut
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

            hr = MFExtern.MFPutWaitingWorkItem(
                intPtrIn,
                intIn,
                imfAsyncResultIn,
                out longOut
            );

            hr = MFExtern.MFAllocateSerialWorkQueue(
                intIn,
                out intOut
            );

            hr = MFExtern.MFBeginRegisterWorkQueueWithMMCSSEx(
                intIn,
                stringIn,
                intIn,
                intIn,
                imfAsyncCallbackIn,
                objectIn
            );

            hr = MFExtern.MFRegisterPlatformWithMMCSS(
                stringIn,
                ref intIn,
                intIn
            );

            hr = MFExtern.MFUnregisterPlatformFromMMCSS();

            hr = MFExtern.MFLockSharedWorkQueue(
                stringIn,
                intIn,
                ref intIn,
                out intOut
            );

            hr = MFExtern.MFGetWorkQueueMMCSSPriority(
                intIn,
                out intOut
            );

            hr = MFExtern.MFMapDX9FormatToDXGIFormat(
                intIn
            );

            hr = MFExtern.MFMapDXGIFormatToDX9Format(
                intIn
            );

            hr = MFExtern.MFLockDXGIDeviceManager(
                out intOut,
                out imfDXGIDeviceManagerOut
            );

            hr = MFExtern.MFUnlockDXGIDeviceManager();

            hr = MFExtern.MFCreateDXGISurfaceBuffer(
                g2,
                objectIn,
                intIn,
                boolIn,
                out imfMediaBufferOut
            );

            hr = MFExtern.MFCreateVideoSampleAllocatorEx(
                g2,
                out objectOut
            );

            hr = MFExtern.MFCreateDXGIDeviceManager(
                out intOut,
                out imfDXGIDeviceManagerOut
            );

            hr = MFExtern.MFRegisterLocalSchemeHandler(
                stringIn,
                imfActivateIn
            );

            hr = MFExtern.MFRegisterLocalByteStreamHandler(
                stringIn,
                stringIn,
                imfActivateIn
            );

            hr = MFExtern.MFCreateMFByteStreamWrapper(
                imfByteStreamIn,
                out imfByteStreamOut
            );

            hr = MFExtern.MFCreateMediaExtensionActivate(
                stringIn,
                objectIn,
                g2,
                out objectOut
            );

            hr = MFExtern.MFCreate2DMediaBuffer(
                intIn,
                intIn,
                intIn,
                boolIn,
                out imfMediaBufferOut
            );

            hr = MFExtern.MFCreateMediaBufferFromMediaType(
                imfMediaTypeIn,
                longIn,
                intIn,
                intIn,
                out imfMediaBufferOut
            );

            hr = MFExtern.MFGetContentProtectionSystemCLSID(
                g,
                out gOut
            );

            /////////////////

            hr = MFExtern.MFPutWorkItem2(
                intIn,
                intIn,
                imfAsyncCallbackIn,
                objectIn
            );

            hr = MFExtern.MFPutWorkItemEx2(
                intIn,
                intIn,
                imfAsyncResultIn
            );

            hr = MFExtern.MFCreateWICBitmapBuffer(
                g2,
                objectIn,
                out imfMediaBufferOut
            );

        }
    }
}
