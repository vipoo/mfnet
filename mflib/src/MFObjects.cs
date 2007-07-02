#region license

/*
MediaFoundationLib - Provide access to MediaFoundation interfaces via .NET
Copyright (C) 2007
http://sourceforge.net/projects/directshownet/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

#endregion

using System;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    #region Declarations

    [Flags, UnmanagedName("MFBYTESTREAM_* defines")]
    public enum MFByteStreamSeekFlags
    {
        None       =                0x00000000,
        IsReadable =                0x00000001,
        IsWritable =                0x00000002,
        IsSeekable =                0x00000004,
        IsRemote =                  0x00000008,
        IsDirectory =               0x00000080,
        HasSlowSeek =               0x00000100,
        IsPartiallyDownloaded =     0x00000200
    }

    [UnmanagedName("MF_ATTRIBUTE_TYPE")]
    public enum MF_AttributeType
    {
        Blob = 0x1011,
        Double = 5,
        Guid = 0x48,
        IUnknown = 13,
        String = 0x1f,
        Uint32 = 0x13,
        Uint64 = 0x15
    }

    [UnmanagedName("MF_ATTRIBUTES_MATCH_TYPE")]
    public enum MF_AttributesMatchType
    {
        OurItems,
        TheirItems,
        AllItems,
        InterSection,
        Smaller
    }

    [Flags, UnmanagedName("MF_MEDIATYPE_EQUAL_* defines")]
    public enum MFMediaEqual
    {
        None = 0,
        MajorTypes = 0x00000001,
        FormatTypes = 0x00000002,
        FormatData = 0x00000004,
        FormatUserData = 0x00000008
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8), UnmanagedName("MFVIDEOFORMAT")]
    public struct MFVideoFormat
    {
        public int dwSize;
        public MFVideoInfo videoInfo;
        public Guid guidFormat;
        public MFVideoCompressedInfo compressedInfo;
        public MFVideoSurfaceInfo surfaceInfo;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8), UnmanagedName("MFVideoInfo")]
    public struct MFVideoInfo
    {
        public int dwWidth;
        public int dwHeight;
        public MFRatio PixelAspectRatio;
        public MFVideoChromaSubsampling SourceChromaSubsampling;
        public MFVideoInterlaceMode InterlaceMode;
        public MFVideoTransferFunction TransferFunction;
        public MFVideoPrimaries ColorPrimaries;
        public MFVideoTransferMatrix TransferMatrix;
        public MFVideoLighting SourceLighting;
        public MFRatio FramesPerSecond;
        public MFNominalRange NominalRange;
        public MFVideoArea GeometricAperture;
        public MFVideoArea MinimumDisplayAperture;
        public MFVideoArea PanScanAperture;
        public long VideoFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8), UnmanagedName("MFVideoCompressedInfo")]
    public struct MFVideoCompressedInfo
    {
        public long AvgBitrate;
        public long AvgBitErrorRate;
        public int MaxKeyFrameSpacing;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("MFVideoSurfaceInfo")]
    public struct MFVideoSurfaceInfo
    {
        public int Format;
        public int PaletteEntries;
        public MFPaletteEntry[] Palette;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("MFRatio")]
    public struct MFRatio
    {
        public int Numerator;
        public int Denominator;
    }

    [UnmanagedName("MFVideoChromaSubsampling")]
    public enum MFVideoChromaSubsampling
    {
        Cosited = 7,
        DV_PAL = 6,
        ForceDWORD = 0x7fffffff,
        Horizontally_Cosited = 4,
        Last = 8,
        MPEG1 = 1,
        MPEG2 = 5,
        ProgressiveChroma = 8,
        Unknown = 0,
        Vertically_AlignedChromaPlanes = 1,
        Vertically_Cosited = 2
    }

    [UnmanagedName("MFVideoInterlaceMode")]
    public enum MFVideoInterlaceMode
    {
        FieldInterleavedLowerFirst = 4,
        FieldInterleavedUpperFirst = 3,
        FieldSingleLower = 6,
        FieldSingleUpper = 5,
        ForceDWORD = 0x7fffffff,
        Last = 8,
        MixedInterlaceOrProgressive = 7,
        Progressive = 2,
        Unknown = 0
    }

    [UnmanagedName("MFVideoTransferFunction")]
    public enum MFVideoTransferFunction
    {
        Func10 = 1,
        Func18 = 2,
        Func20 = 3,
        Func22 = 4,
        Func240M = 6,
        Func28 = 8,
        Func709 = 5,
        ForceDWORD = 0x7fffffff,
        Last = 9,
        sRGB = 7,
        Unknown = 0
    }

    [UnmanagedName("MFVideoPrimaries")]
    public enum MFVideoPrimaries
    {
        BT470_2_SysBG = 4,
        BT470_2_SysM = 3,
        BT709 = 2,
        EBU3213 = 7,
        ForceDWORD = 0x7fffffff,
        Last = 9,
        reserved = 1,
        SMPTE_C = 8,
        SMPTE170M = 5,
        SMPTE240M = 6,
        Unknown = 0
    }

    [UnmanagedName("MFVideoTransferMatrix")]
    public enum MFVideoTransferMatrix
    {
        BT601 = 2,
        BT709 = 1,
        ForceDWORD = 0x7fffffff,
        Last = 4,
        SMPTE240M = 3,
        Unknown = 0
    }

    [UnmanagedName("MFVideoLighting")]
    public enum MFVideoLighting
    {
        Bright = 1,
        Dark = 4,
        Dim = 3,
        ForceDWORD = 0x7fffffff,
        Last = 5,
        Office = 2,
        Unknown = 0
    }

    [UnmanagedName("MFNominalRange")]
    public enum MFNominalRange
    {
        MFNominalRange_0_255 = 1,
        MFNominalRange_16_235 = 2,
        MFNominalRange_48_208 = 3,
        MFNominalRange_ForceDWORD = 0x7fffffff,
        MFNominalRange_Last = 4,
        MFNominalRange_Normal = 1,
        MFNominalRange_Unknown = 0,
        MFNominalRange_Wide = 2
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4), UnmanagedName("MFVideoArea")]
    public struct MFVideoArea
    {
        public MFOffset OffsetX;
        public MFOffset OffsetY;
        public SIZE Area;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2), UnmanagedName("MFOffset")]
    public struct MFOffset
    {
        public short fract;
        public short Value;
    }

    [Flags, UnmanagedName("MFASYNC_* defines")]
    public enum MFASync
    {
        None = 0,
        FastIOProcessingCallback = 0x00000001,
        SignalCallback = 0x00000002
    }

    [UnmanagedName("unnamed enum")]
    public enum MediaEventType
    {
        MEUnknown = 0,
        MEError = (MEUnknown + 1),
        MEExtendedType = (MEError + 1),
        MESessionUnknown = 100,
        MESessionTopologySet = (MESessionUnknown + 1),
        MESessionTopologiesCleared = (MESessionTopologySet + 1),
        MESessionStarted = (MESessionTopologiesCleared + 1),
        MESessionPaused = (MESessionStarted + 1),
        MESessionStopped = (MESessionPaused + 1),
        MESessionClosed = (MESessionStopped + 1),
        MESessionEnded = (MESessionClosed + 1),
        MESessionRateChanged = (MESessionEnded + 1),
        MESessionScrubSampleComplete = (MESessionRateChanged + 1),
        MESessionCapabilitiesChanged = (MESessionScrubSampleComplete + 1),
        MESessionTopologyStatus = (MESessionCapabilitiesChanged + 1),
        MESessionNotifyPresentationTime = (MESessionTopologyStatus + 1),
        MENewPresentation = (MESessionNotifyPresentationTime + 1),
        MELicenseAcquisitionStart = (MENewPresentation + 1),
        MELicenseAcquisitionCompleted = (MELicenseAcquisitionStart + 1),
        MEIndividualizationStart = (MELicenseAcquisitionCompleted + 1),
        MEIndividualizationCompleted = (MEIndividualizationStart + 1),
        MEEnablerProgress = (MEIndividualizationCompleted + 1),
        MEEnablerCompleted = (MEEnablerProgress + 1),
        MEPolicyError = (MEEnablerCompleted + 1),
        MEPolicyReport = (MEPolicyError + 1),
        MEBufferingStarted = (MEPolicyReport + 1),
        MEBufferingStopped = (MEBufferingStarted + 1),
        MEConnectStart = (MEBufferingStopped + 1),
        MEConnectEnd = (MEConnectStart + 1),
        MEReconnectStart = (MEConnectEnd + 1),
        MEReconnectEnd = (MEReconnectStart + 1),
        MERendererEvent = (MEReconnectEnd + 1),
        MESessionStreamSinkFormatChanged = (MERendererEvent + 1),
        MESourceUnknown = 200,
        MESourceStarted = (MESourceUnknown + 1),
        MEStreamStarted = (MESourceStarted + 1),
        MESourceSeeked = (MEStreamStarted + 1),
        MEStreamSeeked = (MESourceSeeked + 1),
        MENewStream = (MEStreamSeeked + 1),
        MEUpdatedStream = (MENewStream + 1),
        MESourceStopped = (MEUpdatedStream + 1),
        MEStreamStopped = (MESourceStopped + 1),
        MESourcePaused = (MEStreamStopped + 1),
        MEStreamPaused = (MESourcePaused + 1),
        MEEndOfPresentation = (MEStreamPaused + 1),
        MEEndOfStream = (MEEndOfPresentation + 1),
        MEMediaSample = (MEEndOfStream + 1),
        MEStreamTick = (MEMediaSample + 1),
        MEStreamThinMode = (MEStreamTick + 1),
        MEStreamFormatChanged = (MEStreamThinMode + 1),
        MESourceRateChanged = (MEStreamFormatChanged + 1),
        MEEndOfPresentationSegment = (MESourceRateChanged + 1),
        MESourceCharacteristicsChanged = (MEEndOfPresentationSegment + 1),
        MESourceRateChangeRequested = (MESourceCharacteristicsChanged + 1),
        MESourceMetadataChanged = (MESourceRateChangeRequested + 1),
        MESequencerSourceTopologyUpdated = (MESourceMetadataChanged + 1),
        MESinkUnknown = 300,
        MEStreamSinkStarted = (MESinkUnknown + 1),
        MEStreamSinkStopped = (MEStreamSinkStarted + 1),
        MEStreamSinkPaused = (MEStreamSinkStopped + 1),
        MEStreamSinkRateChanged = (MEStreamSinkPaused + 1),
        MEStreamSinkRequestSample = (MEStreamSinkRateChanged + 1),
        MEStreamSinkMarker = (MEStreamSinkRequestSample + 1),
        MEStreamSinkPrerolled = (MEStreamSinkMarker + 1),
        MEStreamSinkScrubSampleComplete = (MEStreamSinkPrerolled + 1),
        MEStreamSinkFormatChanged = (MEStreamSinkScrubSampleComplete + 1),
        MEStreamSinkDeviceChanged = (MEStreamSinkFormatChanged + 1),
        MEQualityNotify = (MEStreamSinkDeviceChanged + 1),
        MESinkInvalidated = (MEQualityNotify + 1),
        MEAudioSessionNameChanged = (MESinkInvalidated + 1),
        MEAudioSessionVolumeChanged = (MEAudioSessionNameChanged + 1),
        MEAudioSessionDeviceRemoved = (MEAudioSessionVolumeChanged + 1),
        MEAudioSessionServerShutdown = (MEAudioSessionDeviceRemoved + 1),
        MEAudioSessionGroupingParamChanged = (MEAudioSessionServerShutdown + 1),
        MEAudioSessionIconChanged = (MEAudioSessionGroupingParamChanged + 1),
        MEAudioSessionFormatChanged = (MEAudioSessionIconChanged + 1),
        MEAudioSessionDisconnected = (MEAudioSessionFormatChanged + 1),
        MEAudioSessionExclusiveModeOverride = (MEAudioSessionDisconnected + 1),
        METrustUnknown = 400,
        MEPolicyChanged = (METrustUnknown + 1),
        MEContentProtectionMessage = (MEPolicyChanged + 1),
        MEPolicySet = (MEContentProtectionMessage + 1),
        MEWMDRMLicenseBackupCompleted = 500,
        MEWMDRMLicenseBackupProgress = 501,
        MEWMDRMLicenseRestoreCompleted = 502,
        MEWMDRMLicenseRestoreProgress = 503,
        MEWMDRMLicenseAcquisitionCompleted = 506,
        MEWMDRMIndividualizationCompleted = 508,
        MEWMDRMIndividualizationProgress = 513,
        MEWMDRMProximityCompleted = 514,
        MEWMDRMLicenseStoreCleaned = 515,
        MEWMDRMRevocationDownloadCompleted = 516,
        MEReservedMax = 10000
    }

    [Flags, UnmanagedName("MF_EVENT_FLAG_* defines")]
    public enum MFEventFlag
    {
        None = 0,
        NoWait = 0x00000001
    }

    [UnmanagedName("MFBYTESTREAM_SEEK_ORIGIN")]
    public enum MFByteStreamSeekOrigin
    {
        Begin,
        Current
    }

    #endregion

    #region Interfaces

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("a27003d0-2354-4f2a-8d6a-ab7cff15437e"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFRemoteAsyncCallback
    {
        [PreserveSig]
        int Invoke(
            int hr,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pRemoteResult
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2CD2D921-C447-44A7-A13C-4ADABFC247E3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFAttributes
    {
        [PreserveSig]
        int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MF_AttributeType pType
            );

        [PreserveSig]
        int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value,
            out int pbResult
            );

        [PreserveSig]
        int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MF_AttributesMatchType MatchType,
            out int pbResult
            );

        [PreserveSig]
        int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        int GetGuid(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] string pwszValue,
            int cchBufSize,
            [In, Out] ref int pcchLength
            );

        [PreserveSig]
        int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out byte pBuf, 
            int cbBufSize,
            [In, Out] ref int pcbBlobSize
            );

        [PreserveSig]
        int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out] IntPtr ppBuf,
            out int pcbSize
            );

        [PreserveSig]
        int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv
            );

        [PreserveSig]
        int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value
            );

        [PreserveSig]
        int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        int DeleteAllItems();

        [PreserveSig]
        int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In] ref byte pBuf,
            int cbBufSize
            );

        [PreserveSig]
        int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        int LockStore();

        [PreserveSig]
        int UnlockStore();

        [PreserveSig]
        int GetCount(
            out int pcItems
            );

        [PreserveSig]
        int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("045FA593-8799-42B8-BC8D-8968C6453507")]
    public interface IMFMediaBuffer
    {
        [PreserveSig]
        int Lock(
            out IntPtr ppbBuffer,
            out int pcbMaxLength,
            out int pcbCurrentLength
            );

        [PreserveSig]
        int Unlock();

        [PreserveSig]
        int GetCurrentLength(
            out int pcbCurrentLength
            );

        [PreserveSig]
        int SetCurrentLength(
            [In] int cbCurrentLength
            );

        [PreserveSig]
        int GetMaxLength(
            out int pcbMaxLength
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("C40A00F2-B93A-4D80-AE8C-5A1C634F58E4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSample : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MF_AttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value,
            out int pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MF_AttributesMatchType MatchType,
            out int pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGuid(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] string pwszValue,
            int cchBufSize,
            [In, Out] ref int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out byte pBuf,
            int cbBufSize,
            [In, Out] ref int pcbBlobSize
            );

        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out] IntPtr ppBuf,
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In] ref byte pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        int GetSampleFlags(
            out int pdwSampleFlags // Must be zero
            );

        [PreserveSig]
        int SetSampleFlags(
            [In] int dwSampleFlags // Must be zero
            );

        [PreserveSig]
        int GetSampleTime(
            out long phnsSampleTime
            );

        [PreserveSig]
        int SetSampleTime(
            [In] long hnsSampleTime
            );

        [PreserveSig]
        int GetSampleDuration(
            out long phnsSampleDuration
            );

        [PreserveSig]
        int SetSampleDuration(
            [In] long hnsSampleDuration
            );

        [PreserveSig]
        int GetBufferCount(
            out int pdwBufferCount
            );

        [PreserveSig]
        int GetBufferByIndex(
            [In] int dwIndex,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaBuffer ppBuffer
            );

        [PreserveSig]
        int ConvertToContiguousBuffer(
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaBuffer ppBuffer
            );

        [PreserveSig]
        int AddBuffer(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaBuffer pBuffer
            );

        [PreserveSig]
        int RemoveBufferByIndex(
            [In] int dwIndex
            );

        [PreserveSig]
        int RemoveAllBuffers();

        [PreserveSig]
        int GetTotalLength(
            out int pcbTotalLength
            );

        [PreserveSig]
        int CopyToBuffer(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaBuffer pBuffer
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("7DC9D5F9-9ED9-44EC-9BBF-0600BB589FBB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMF2DBuffer
    {
        [PreserveSig]
        int Lock2D(
            [Out] IntPtr pbScanline0,
            out int plPitch
            );

        [PreserveSig]
        int Unlock2D();

        [PreserveSig]
        int GetScanline0AndPitch(
            [Out] IntPtr pbScanline0,
            out int plPitch
            );

        [PreserveSig]
        int IsContiguousFormat(
            out int pfIsContiguous
            );

        [PreserveSig]
        int GetContiguousLength(
            out int pcbLength
            );

        [PreserveSig]
        int ContiguousCopyTo(
            out byte pbDestBuffer,
            [In] int cbDestBuffer
            );

        [PreserveSig]
        int ContiguousCopyFrom(
            [In] IntPtr pbSrcBuffer,
            [In] int cbSrcBuffer
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("44AE0FA8-EA31-4109-8D2E-4CAE4997C555")]
    public interface IMFMediaType : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MF_AttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value,
            out int pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MF_AttributesMatchType MatchType,
            out int pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGuid(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] string pwszValue,
            int cchBufSize,
            [In, Out] ref int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out byte pBuf,
            int cbBufSize,
            [In, Out] ref int pcbBlobSize
            );

        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out] IntPtr ppBuf,
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In] ref byte pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        int GetMajorType(
            out Guid pguidMajorType
            );

        [PreserveSig]
        int IsCompressedFormat(
            [MarshalAs(UnmanagedType.Bool)] out bool pfCompressed
            );

        [PreserveSig]
        int IsEqual(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pIMediaType,
            out MFMediaEqual pdwFlags
            );

        [PreserveSig]
        int GetRepresentation(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidRepresentation,
            out IntPtr ppvRepresentation
            );

        [PreserveSig]
        int FreeRepresentation(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidRepresentation,
            [In] IntPtr pvRepresentation
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("26A0ADC3-CE26-4672-9304-69552EDD3FAF"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFAudioMediaType : IMFMediaType
    {
        #region IMFMediaType methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MF_AttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value,
            out int pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MF_AttributesMatchType MatchType,
            out int pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGuid(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] string pwszValue,
            int cchBufSize,
            [In, Out] ref int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out byte pBuf,
            int cbBufSize,
            [In, Out] ref int pcbBlobSize
            );

        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out] IntPtr ppBuf,
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In] ref byte pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        [PreserveSig]
        new int GetMajorType(
            out Guid pguidMajorType
            );

        [PreserveSig]
        new int IsCompressedFormat(
            [MarshalAs(UnmanagedType.Bool)] out bool pfCompressed
            );

        [PreserveSig]
        new int IsEqual(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pIMediaType,
            out MFMediaEqual pdwFlags
            );

        [PreserveSig]
        new int GetRepresentation(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidRepresentation,
            out IntPtr ppvRepresentation
            );

        [PreserveSig]
        new int FreeRepresentation(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidRepresentation,
            [In] IntPtr pvRepresentation
            );

        #endregion

        [PreserveSig, Obsolete("To get the properties of the audio format, applications should use the media type attributes. If you need to convert the media type into a WAVEFORMATEX structure, call MFCreateWaveFormatExFromMFMediaType")]
        IntPtr GetAudioFormat();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("B99F381F-A8F9-47A2-A5AF-CA3A225A3890"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFVideoMediaType : IMFMediaType
    {
        #region IMFMediaType methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MF_AttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value,
            out int pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MF_AttributesMatchType MatchType,
            out int pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGuid(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] string pwszValue,
            int cchBufSize,
            [In, Out] ref int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out byte pBuf,
            int cbBufSize,
            [In, Out] ref int pcbBlobSize
            );

        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out] IntPtr ppBuf,
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In] ref byte pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        [PreserveSig]
        new int GetMajorType(
            out Guid pguidMajorType
            );

        [PreserveSig]
        new int IsCompressedFormat(
            [MarshalAs(UnmanagedType.Bool)] out bool pfCompressed
            );

        [PreserveSig]
        new int IsEqual(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaType pIMediaType,
            out MFMediaEqual pdwFlags
            );

        [PreserveSig]
        new int GetRepresentation(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidRepresentation,
            out IntPtr ppvRepresentation
            );

        [PreserveSig]
        new int FreeRepresentation(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidRepresentation,
            [In] IntPtr pvRepresentation
            );

        #endregion

        [PreserveSig]
        int GetVideoRepresentation(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidRepresentation,
            out IntPtr ppvRepresentation,
            [In] int lStride
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("AC6B7889-0740-4D51-8619-905994A55CC6")]
    public interface IMFAsyncResult
    {
        [PreserveSig]
        int GetState(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppunkState
            );

        [PreserveSig]
        int GetStatus();

        [PreserveSig]
        int SetStatus(
            [In, MarshalAs(UnmanagedType.Error)] int hrStatus
            );

        [PreserveSig]
        int GetObject(
            [MarshalAs(UnmanagedType.IUnknown)] out object ppObject
            );

        [return: MarshalAs(UnmanagedType.IUnknown)]
        [PreserveSig]
        object GetStateNoAddRef();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("A27003CF-2354-4F2A-8D6A-AB7CFF15437E")]
    public interface IMFAsyncCallback
    {
        [PreserveSig]
        int GetParameters(
            out MFASync pdwFlags,
            out int pdwQueue
            );

        [PreserveSig]
        int Invoke(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncResult pAsyncResult
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("DF598932-F10C-4E39-BBA2-C308F101DAA3"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEvent : IMFAttributes
    {
        #region IMFAttributes methods

        [PreserveSig]
        new int GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MF_AttributeType pType
            );

        [PreserveSig]
        new int CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value,
            out int pbResult
            );

        [PreserveSig]
        new int Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MF_AttributesMatchType MatchType,
            out int pbResult
            );

        [PreserveSig]
        new int GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        [PreserveSig]
        new int GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        [PreserveSig]
        new int GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        [PreserveSig]
        new int GetGuid(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        [PreserveSig]
        new int GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        [PreserveSig]
        new int GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] string pwszValue,
            int cchBufSize,
            [In, Out] ref int pcchLength
            );

        [PreserveSig]
        new int GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        [PreserveSig]
        new int GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        [PreserveSig]
        new int GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out byte pBuf,
            int cbBufSize,
            [In, Out] ref int pcbBlobSize
            );

        [PreserveSig]
        new int GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out] IntPtr ppBuf,
            out int pcbSize
            );

        [PreserveSig]
        new int GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv
            );

        [PreserveSig]
        new int SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            ref object Value
            );

        [PreserveSig]
        new int DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        [PreserveSig]
        new int DeleteAllItems();

        [PreserveSig]
        new int SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        [PreserveSig]
        new int SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        [PreserveSig]
        new int SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        [PreserveSig]
        new int SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        [PreserveSig]
        new int SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        [PreserveSig]
        new int SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In] ref byte pBuf,
            int cbBufSize
            );

        [PreserveSig]
        new int SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        new int LockStore();

        [PreserveSig]
        new int UnlockStore();

        [PreserveSig]
        new int GetCount(
            out int pcItems
            );

        [PreserveSig]
        new int GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out] ref object pValue
            );

        [PreserveSig]
        new int CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        [PreserveSig]
        int GetType(
            out MediaEventType pmet
            );

        [PreserveSig]
        int GetExtendedType(
            out Guid pguidExtendedType
            );

        [PreserveSig]
        int GetStatus(
            [MarshalAs(UnmanagedType.Error)] out int phrStatus
            );

        [PreserveSig]
        int GetValue(
            out object pvValue
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2CD0BD52-BCD5-4B89-B62C-EADC0C031E7D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEventGenerator
    {
        [PreserveSig]
        int GetEvent(
            [In] MFEventFlag dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaEvent ppEvent
            );

        [PreserveSig]
        int BeginGetEvent(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncCallback pCallback,
            [In, MarshalAs(UnmanagedType.IUnknown)] object o
            );

        [PreserveSig]
        int EndGetEvent(
            IMFAsyncResult pResult,
            out IMFMediaEvent ppEvent);

        [PreserveSig]
        int QueueEvent(
            [In] MediaEventType met,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidExtendedType,
            [In] int hrStatus,
            [In] object pvValue);
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("AD4C1B00-4BF7-422F-9175-756693D9130D")]
    public interface IMFByteStream
    {
        [PreserveSig]
        int GetCapabilities(
            out MFByteStreamSeekFlags pdwCapabilities
            );

        [PreserveSig]
        int GetLength(
            out long pqwLength
            );

        [PreserveSig]
        int SetLength(
            [In] long qwLength
            );

        [PreserveSig]
        int GetCurrentPosition(
            out long pqwPosition
            );

        [PreserveSig]
        int SetCurrentPosition(
            [In] long qwPosition
            );

        [PreserveSig]
        int IsEndOfStream(
            [MarshalAs(UnmanagedType.Bool)] out bool pfEndOfStream
            );

        [PreserveSig]
        int Read(
            [In] IntPtr pb,
            [In] int cb,
            out int pcbRead
            );

        [PreserveSig]
        int BeginRead(
            [In] IntPtr pb,
            [In] int cb,
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncCallback pCallback,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkState
            );

        [PreserveSig]
        int EndRead(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncResult pResult,
            out int pcbRead
            );

        [PreserveSig]
        int Write(
            [In] IntPtr pb,
            [In] int cb,
            out int pcbWritten
            );

        [PreserveSig]
        int BeginWrite(
            [In] IntPtr pb,
            [In] int cb,
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncCallback pCallback,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkState
            );

        [PreserveSig]
        int EndWrite(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncResult pResult,
            out int pcbWritten
            );

        [PreserveSig]
        int Seek(
            [In] MFByteStreamSeekOrigin SeekOrigin,
            [In] long llSeekOffset,
            [In] int dwSeekFlags,
            out long pqwCurrentPosition
            );

        [PreserveSig]
        int Flush();

        [PreserveSig]
        int Close();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("5BC8A76B-869A-46A3-9B03-FA218A66AEBE")]
    public interface IMFCollection
    {
        [PreserveSig]
        int GetElementCount(
            out int pcElements
            );

        [PreserveSig]
        int GetElement(
            [In] int dwElementIndex,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement
            );

        [PreserveSig]
        int AddElement(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkElement
            );

        [PreserveSig]
        int RemoveElement(
            [In] int dwElementIndex,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement
            );

        [PreserveSig]
        int InsertElementAt(
            [In] int dwIndex,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        [PreserveSig]
        int RemoveAllElements();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("36F846FC-2256-48B6-B58E-E2B638316581"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEventQueue
    {
        [PreserveSig]
        int GetEvent(
            [In] MFEventFlag dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaEvent ppEvent
            );

        [PreserveSig]
        int BeginGetEvent(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncCallback pCallback,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkState
            );

        [PreserveSig]
        int EndGetEvent(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAsyncResult pResult,
            [MarshalAs(UnmanagedType.Interface)] out IMFMediaEvent ppEvent
            );

        [PreserveSig]
        int QueueEvent(
            [In, MarshalAs(UnmanagedType.Interface)] IMFMediaEvent pEvent
            );

        [PreserveSig]
        int QueueEventParamVar(
            [In] MediaEventType met,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidExtendedType,
            [In, MarshalAs(UnmanagedType.Error)] int hrStatus,
            [In] IntPtr pvValue
            );

        [PreserveSig]
        int QueueEventParamUnk(
            [In] MediaEventType met,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidExtendedType,
            [In, MarshalAs(UnmanagedType.Error)] int hrStatus,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnk
            );

        [PreserveSig]
        int Shutdown();
    }

    #endregion

}
