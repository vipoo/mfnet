﻿#region license

/*
MediaFoundationLib - Provide access to MediaFoundation interfaces via .NET
Copyright (C) 2007
http://mfnet.sourceforge.net

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
using MediaFoundation.EVR;
using MediaFoundation.Transform;

namespace MediaFoundation
{

    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    [Flags, UnmanagedName("MFMEDIASOURCE_CHARACTERISTICS")]
    public enum MFMEDIASOURCE_CHARACTERISTICS
    {
        IsLive = 0x1,
        CanSeek = 0x2,
        CanPause = 0x4,
        HasSlowSeek = 0x8,
        HasMultiplePresentations = 0x10,
        CanSkipForward = 0x20,
        CanSkipBackward = 0x40,
        DoesNotUseNetwork = 0x80,
    }
    
    [UnmanagedName("ERole")]
    public enum ERole
    {
        eConsole = 0,
        eMultimedia = 1,
        eCommunications = 2,
        ERole_enum_count = 3,
    }

    [UnmanagedName("AUDIO_STREAM_CATEGORY")]
    public enum AUDIO_STREAM_CATEGORY
    {
        Other = 0,
        ForegroundOnlyMedia,
        BackgroundCapableMedia,
        Communications,
        Alerts,
        SoundEffects,
        GameEffects,
        GameMedia,
        GameChat,
        Speech,
        Movie,
        Media
    }
    
    [UnmanagedName("MF_MEDIA_ENGINE_ERR")]
    public enum MF_MEDIA_ENGINE_ERR : short
    {
        NoError = 0,
        Aborted = 1,
        Network = 2,
        Decode = 3,
        SrcNotSupported = 4,
        Encrypted = 5,
    }

    [UnmanagedName("MF_MEDIA_ENGINE_EVENT")]
    public enum MF_MEDIA_ENGINE_EVENT
    {
        LoadStart = 1,
        Progress = 2,
        Suspend = 3,
        Abort = 4,
        Error = 5,
        Emptied = 6,
        Stalled = 7,
        Play = 8,
        Pause = 9,
        LoadedMetadata = 10,
        LoadedData = 11,
        Waiting = 12,
        Playing = 13,
        CanPlay = 14,
        CanPlayThrough = 15,
        Seeking = 16,
        Seeked = 17,
        TimeUpdate = 18,
        Ended = 19,
        RateChange = 20,
        DurationChange = 21,
        VolumeChange = 22,

        FormatChange = 1000,
        PurgeQueuedEvents = 1001,
        TimelineMarker = 1002,
        BalanceChange = 1003,
        DownloadComplete = 1004,
        BufferingStarted = 1005,
        BufferingEnded = 1006,
        FrameStepCompleted = 1007,
        NotifyStableState = 1008,
        FirstFrameReady = 1009,
        TracksChange = 1010,
        OpmInfo = 1011,

        ResourceLost = 1012,
        DelayLoadEventChanged = 1013,
        StreamRenderingError = 1014,
    }

    [UnmanagedName("MF_MEDIA_ENGINE_NETWORK")]
    public enum MF_MEDIA_ENGINE_NETWORK : short
    {
        Empty = 0,
        Idle = 1,
        Loading = 2,
        NoSource = 3
    }

    [UnmanagedName("MF_MEDIA_ENGINE_READY")]
    public enum MF_MEDIA_ENGINE_READY : short
    {
        HaveNothing = 0,
        HaveMetadata = 1,
        HaveCurrentData = 2,
        HaveFutureData = 3,
        HaveEnoughData = 4
    }

    [UnmanagedName("MF_MEDIA_ENGINE_CANPLAY")]
    public enum MF_MEDIA_ENGINE_CANPLAY
    {
        NotSupported = 0,
        Maybe = 1,
        Probably = 2,
    }


    [UnmanagedName("MF_MEDIA_ENGINE_PRELOAD")]
    public enum MF_MEDIA_ENGINE_PRELOAD
    {
        Missing = 0,
        Empty = 1,
        None = 2,
        Metadata = 3,
        Automatic = 4
    }


    [UnmanagedName("MF_MEDIA_ENGINE_S3D_PACKING_MODE")]
    public enum MF_MEDIA_ENGINE_S3D_PACKING_MODE
    {
        None = 0,
        SideBySide = 1,
        TopBottom = 2

    }

    [UnmanagedName("MF_MEDIA_ENGINE_STATISTIC")]
    public enum MF_MEDIA_ENGINE_STATISTIC
    {
        FramesRendered = 0,
        FramesDropped = 1,
        BytesDownloaded = 2,
        BufferProgress = 3,
        FramesPerSecond = 4,
        PlaybackJitter = 5,
        FramesCorrupted = 6,
        TotalFrameDelay = 7,
    }

    [UnmanagedName("MF_MEDIA_ENGINE_SEEK_MODE")]
    public enum MF_MEDIA_ENGINE_SEEK_MODE
    {
        Normal = 0,
        Approximate = 1
    }

    [UnmanagedName("MF_MEDIA_ENGINE_EXTENSION_TYPE")]
    public enum MF_MEDIA_ENGINE_EXTENSION_TYPE
    {
        MediaSource = 0,
        ByteStream = 1
    }

    [Flags, UnmanagedName("MF_MEDIA_ENGINE_FRAME_PROTECTION_FLAGS")]
    public enum MF_MEDIA_ENGINE_FRAME_PROTECTION_FLAGS
    {
        None = 0x0,
        Protected = 0x01,
        RequiresSurfaceProtection = 0x02,
        RequiresAntiScreenScrapeProtection = 0x04
    }

    [Flags, UnmanagedName("MF_MEDIA_ENGINE_CREATEFLAGS")]
    public enum MF_MEDIA_ENGINE_CREATEFLAGS
    {
        None = 0,
        AudioOnly = 0x0001,
        WaitForStableState = 0x0002,
        ForceMute = 0x0004,
        RealTimeMode = 0x0008,
        DisableLocalPlugins = 0x0010,
        CreateFlagsMask = 0x001F
    }

    [Flags, UnmanagedName("MF_MEDIA_ENGINE_PROTECTION_FLAGS")]
    public enum MF_MEDIA_ENGINE_PROTECTION_FLAGS
    {
        EnableProtectedContent = 1,
        UsePMPForAllContent = 2,
        UseUnprotectedPMP = 4

    }

    [UnmanagedName("MF_MSE_READY")]
    public enum MF_MSE_READY
    {
        Closed = 1,
        Open = 2,
        Ended = 3,
    }

    [UnmanagedName("MF_MSE_ERROR")]
    public enum MF_MSE_ERROR
    {
        NoError = 0,
        Network = 1,
        Decode = 2,
        UnknownError = 3,
    }

    [UnmanagedName("MF_MEDIA_ENGINE_KEYERR")]
    public enum MF_MEDIA_ENGINE_KEYERR
    {
        Unknown = 1,
        Client = 2,
        Service = 3,
        Output = 4,
        HardwareChange = 5,
        Domain = 6,
    }

    [UnmanagedName("MF_MEDIA_ENGINE_OPM_STATUS")]
    public enum MF_MEDIA_ENGINE_OPM_STATUS
    {
        NotRequested = 0,
        Established = 1,
        FailedVM = 2,
        FailedBDA = 3,
        FailedUnsignedDriver = 4,
        Failed = 5,
    }

    public enum MF_MSE_APPEND_MODE
    {
        Segments = 0,
        Sequence = 1,
    }

    public enum MF_TIMED_TEXT_TRACK_KIND
    {
        Unknown = 0,
        Subtitles = 1,
        Captions = 2,
        Metadata = 3,
    }

    public enum MF_TIMED_TEXT_UNIT_TYPE
    {
        Pixels = 0,
        Percentage = 1
    }

    public enum MF_TIMED_TEXT_FONT_STYLE
    {
        Normal = 0,
        Oblique = 1,
        Italic = 2,
    }

    public enum MF_TIMED_TEXT_ALIGNMENT
    {
        Start = 0,
        End = 1,
        Center = 2,
    }

    public enum MF_TIMED_TEXT_DISPLAY_ALIGNMENT
    {
        Before = 0,
        After = 1,
        Center = 2,
    }

    [Flags]
    public enum MF_TIMED_TEXT_DECORATION
    {
        None = 0,
        Underline = 1,
        LineThrough = 2,
        Overline = 4,
    }

    public enum MF_TIMED_TEXT_WRITING_MODE
    {
        LRTB = 0,
        RLTB = 1,
        TBRL = 2,
        TBLR = 3,
        LR = 4,
        RL = 5,
        TB = 6,
    }

    public enum MF_TIMED_TEXT_SCROLL_MODE
    {
        PopOn = 0,
        RollUp = 1
    }

    public enum MF_TIMED_TEXT_ERROR_CODE
    {
        NoError = 0,
        Fatal = 1,
        DataFormat = 2,
        Network = 3,
        Internal = 4
    }

    public enum MF_TIMED_TEXT_CUE_EVENT
    {
        Active,
        Inactive,
        Clear,
    }

    public enum MF_TIMED_TEXT_TRACK_READY_STATE
    {
        None,
        Loading,
        Loaded,
        Error,
    }

    public enum MF_MEDIA_ENGINE_STREAMTYPE_FAILED
    {
        Unknown = 0,
        Audio = 1,
        Video = 2,
    }

#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("fc0e10d2-ab2a-4501-a951-06bb1075184c"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaError
    {
        [PreserveSig]
        MF_MEDIA_ENGINE_ERR GetErrorCode();

        [PreserveSig]
        int GetExtendedErrorCode();

        [PreserveSig]
        int SetErrorCode(
            MF_MEDIA_ENGINE_ERR error
            );

        [PreserveSig]
        int SetExtendedErrorCode(
            int error
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("db71a2fc-078a-414e-9df9-8c2531b0aa6c"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaTimeRange
    {
        [PreserveSig]
        int GetLength();

        [PreserveSig]
        int GetStart(
            int index,
            out double pStart
            );

        [PreserveSig]
        int GetEnd(
            int index,
            out  double pEnd
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool ContainsTime(
            double time
            );

        [PreserveSig]
        int AddRange(
            double startTime,
            double endTime
            );

        [PreserveSig]
        int Clear();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("fee7c112-e776-42b5-9bbf-0048524e2bd5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineNotify
    {
        [PreserveSig]
        int EventNotify(
            MF_MEDIA_ENGINE_EVENT eventid,
            IntPtr param1,
            int param2
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("7a5e5354-b114-4c72-b991-3131d75032ea"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineSrcElements
    {
        [PreserveSig]
        int GetLength();

        [PreserveSig]
        int GetURL(
            int index,
            [MarshalAs(UnmanagedType.BStr)] out string pURL
            );

        [PreserveSig]
        int GetType(
            int index,
            [MarshalAs(UnmanagedType.BStr)] out string pType
            );

        [PreserveSig]
        int GetMedia(
            int index,
            [MarshalAs(UnmanagedType.BStr)] out string pMedia
            );

        [PreserveSig]
        int AddElement(
            [MarshalAs(UnmanagedType.BStr)] string pURL,
            [MarshalAs(UnmanagedType.BStr)] string pType,
            [MarshalAs(UnmanagedType.BStr)] string pMedia
            );

        [PreserveSig]
        int RemoveAllElements();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("98a1b0bb-03eb-4935-ae7c-93c1fa0e1c93"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngine
    {
        [PreserveSig]
        int GetError(
            out IMFMediaError ppError
            );

        [PreserveSig]
        int SetErrorCode(
            MF_MEDIA_ENGINE_ERR error
            );

        [PreserveSig]
        int SetSourceElements(
            IMFMediaEngineSrcElements pSrcElements
            );

        [PreserveSig]
        int SetSource(
            [MarshalAs(UnmanagedType.BStr)] string pUrl
            );

        [PreserveSig]
        int GetCurrentSource(
            [MarshalAs(UnmanagedType.BStr)] out string ppUrl
            );

        [PreserveSig]
        MF_MEDIA_ENGINE_NETWORK GetNetworkState();

        [PreserveSig]
        MF_MEDIA_ENGINE_PRELOAD GetPreload();

        [PreserveSig]
        int SetPreload(
            MF_MEDIA_ENGINE_PRELOAD Preload
            );

        [PreserveSig]
        int GetBuffered(
            out IMFMediaTimeRange ppBuffered
            );

        [PreserveSig]
        int Load();

        [PreserveSig]
        int CanPlayType(
            [MarshalAs(UnmanagedType.BStr)] string type,
            out MF_MEDIA_ENGINE_CANPLAY pAnswer
            );

        [PreserveSig]
        MF_MEDIA_ENGINE_READY GetReadyState();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsSeeking();

        [PreserveSig]
        double GetCurrentTime();

        [PreserveSig]
        int SetCurrentTime(
            double seekTime
            );

        [PreserveSig]
        double GetStartTime();

        [PreserveSig]
        double GetDuration();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsPaused();

        [PreserveSig]
        double GetDefaultPlaybackRate();

        [PreserveSig]
        int SetDefaultPlaybackRate(
            double Rate
            );

        [PreserveSig]
        double GetPlaybackRate();

        [PreserveSig]
        int SetPlaybackRate(
            double Rate
            );

        [PreserveSig]
        int GetPlayed(
            out IMFMediaTimeRange ppPlayed
            );

        [PreserveSig]
        int GetSeekable(
            out IMFMediaTimeRange ppSeekable
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsEnded();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetAutoPlay();

        [PreserveSig]
        int SetAutoPlay(
            [MarshalAs(UnmanagedType.Bool)] bool AutoPlay
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetLoop();

        [PreserveSig]
        int SetLoop(
            [MarshalAs(UnmanagedType.Bool)] bool Loop
            );

        [PreserveSig]
        int Play();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetMuted();

        [PreserveSig]
        int SetMuted(
            [MarshalAs(UnmanagedType.Bool)] bool Muted
            );

        [PreserveSig]
        double GetVolume();

        [PreserveSig]
        int SetVolume(
            double Volume
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool HasVideo();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool HasAudio();

        [PreserveSig]
        int GetNativeVideoSize(
            out int cx,
            out int cy
            );

        [PreserveSig]
        int GetVideoAspectRatio(
            out int cx,
            out int cy
            );

        [PreserveSig]
        int Shutdown();

        [PreserveSig]
        int TransferVideoFrame(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pDstSurf,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFVideoNormalizedRect pSrc,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFRect pDst,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFARGB pBorderClr
            );

        [PreserveSig]
        int OnVideoStreamTick(
            out long pPts
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("83015ead-b1e6-40d0-a98a-37145ffe1ad1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineEx : IMFMediaEngine
    {
        #region IMFMediaEngine methods

        [PreserveSig]
        new int GetError(
            out IMFMediaError ppError
            );

        [PreserveSig]
        new int SetErrorCode(
            MF_MEDIA_ENGINE_ERR error
            );

        [PreserveSig]
        new int SetSourceElements(
            IMFMediaEngineSrcElements pSrcElements
            );

        [PreserveSig]
        new int SetSource(
            [MarshalAs(UnmanagedType.BStr)] string pUrl
            );

        [PreserveSig]
        new int GetCurrentSource(
            [MarshalAs(UnmanagedType.BStr)] out string ppUrl
            );

        [PreserveSig]
        new MF_MEDIA_ENGINE_NETWORK GetNetworkState();

        [PreserveSig]
        new MF_MEDIA_ENGINE_PRELOAD GetPreload();

        [PreserveSig]
        new int SetPreload(
            MF_MEDIA_ENGINE_PRELOAD Preload
            );

        [PreserveSig]
        new int GetBuffered(
            out IMFMediaTimeRange ppBuffered
            );

        [PreserveSig]
        new int Load();

        [PreserveSig]
        new int CanPlayType(
            [MarshalAs(UnmanagedType.BStr)] string type,
            out MF_MEDIA_ENGINE_CANPLAY pAnswer
            );

        [PreserveSig]
        new MF_MEDIA_ENGINE_READY GetReadyState();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool IsSeeking();

        [PreserveSig]
        new double GetCurrentTime();

        [PreserveSig]
        new int SetCurrentTime(
            double seekTime
            );

        [PreserveSig]
        new double GetStartTime();

        [PreserveSig]
        new double GetDuration();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool IsPaused();

        [PreserveSig]
        new double GetDefaultPlaybackRate();

        [PreserveSig]
        new int SetDefaultPlaybackRate(
            double Rate
            );

        [PreserveSig]
        new double GetPlaybackRate();

        [PreserveSig]
        new int SetPlaybackRate(
            double Rate
            );

        [PreserveSig]
        new int GetPlayed(
            out IMFMediaTimeRange ppPlayed
            );

        [PreserveSig]
        new int GetSeekable(
            out IMFMediaTimeRange ppSeekable
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool IsEnded();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool GetAutoPlay();

        [PreserveSig]
        new int SetAutoPlay(
            [MarshalAs(UnmanagedType.Bool)] bool AutoPlay
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool GetLoop();

        [PreserveSig]
        new int SetLoop(
            [MarshalAs(UnmanagedType.Bool)] bool Loop
            );

        [PreserveSig]
        new int Play();

        [PreserveSig]
        new int Pause();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool GetMuted();

        [PreserveSig]
        new int SetMuted(
            [MarshalAs(UnmanagedType.Bool)] bool Muted
            );

        [PreserveSig]
        new double GetVolume();

        [PreserveSig]
        new int SetVolume(
            double Volume
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool HasVideo();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        new bool HasAudio();

        [PreserveSig]
        new int GetNativeVideoSize(
            out int cx,
            out int cy
            );

        [PreserveSig]
        new int GetVideoAspectRatio(
            out int cx,
            out int cy
            );

        [PreserveSig]
        new int Shutdown();

        [PreserveSig]
        new int TransferVideoFrame(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pDstSurf,
            [In] MFVideoNormalizedRect pSrc,
            [In] MFRect pDst,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFARGB pBorderClr
            );

        [PreserveSig]
        new int OnVideoStreamTick(
            out long pPts
            );

        #endregion

        [PreserveSig]
        int SetSourceFromByteStream(
            IMFByteStream pByteStream,
            [MarshalAs(UnmanagedType.BStr)] string pURL
            );

        [PreserveSig]
        int GetStatistics(
            MF_MEDIA_ENGINE_STATISTIC StatisticID,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pStatistic
            );

        [PreserveSig]
        int UpdateVideoStream(
            [In] MFVideoNormalizedRect pSrc,
            [In] MFRect pDst,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFARGB pBorderClr
            );

        [PreserveSig]
        double GetBalance();

        [PreserveSig]
        int SetBalance(
            double balance
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsPlaybackRateSupported(
            double rate
            );

        [PreserveSig]
        int FrameStep(
            [MarshalAs(UnmanagedType.Bool)] bool Forward
            );

        [PreserveSig]
        int GetResourceCharacteristics(
            out MFMEDIASOURCE_CHARACTERISTICS pCharacteristics
            );

        [PreserveSig]
        int GetPresentationAttribute(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidMFAttribute,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvValue
            );

        [PreserveSig]
        int GetNumberOfStreams(
            out int pdwStreamCount
            );

        [PreserveSig]
        int GetStreamAttribute(
            int dwStreamIndex,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidMFAttribute,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pvValue
            );

        [PreserveSig]
        int GetStreamSelection(
            int dwStreamIndex,
            [MarshalAs(UnmanagedType.Bool)] out bool pEnabled
            );

        [PreserveSig]
        int SetStreamSelection(
            int dwStreamIndex,
            [MarshalAs(UnmanagedType.Bool)] bool Enabled
            );

        [PreserveSig]
        int ApplyStreamSelections();

        [PreserveSig]
        int IsProtected(
            [MarshalAs(UnmanagedType.Bool)] out bool pProtected
            );

        [PreserveSig]
        int InsertVideoEffect(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pEffect,
            [MarshalAs(UnmanagedType.Bool)] bool fOptional
            );

        [PreserveSig]
        int InsertAudioEffect(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pEffect,
            [MarshalAs(UnmanagedType.Bool)] bool fOptional
            );

        [PreserveSig]
        int RemoveAllEffects();

        [PreserveSig]
        int SetTimelineMarkerTimer(
            double timeToFire
            );

        [PreserveSig]
        int GetTimelineMarkerTimer(
            out double pTimeToFire
            );

        [PreserveSig]
        int CancelTimelineMarkerTimer();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsStereo3D();

        [PreserveSig]
        int GetStereo3DFramePackingMode(
            out MF_MEDIA_ENGINE_S3D_PACKING_MODE packMode
            );

        [PreserveSig]
        int SetStereo3DFramePackingMode(
            MF_MEDIA_ENGINE_S3D_PACKING_MODE packMode
            );

        [PreserveSig]
        int GetStereo3DRenderMode(
            out MF3DVideoOutputType outputType
            );

        [PreserveSig]
        int SetStereo3DRenderMode(
            MF3DVideoOutputType outputType
            );

        [PreserveSig]
        int EnableWindowlessSwapchainMode(
            [MarshalAs(UnmanagedType.Bool)] bool fEnable
            );

        [PreserveSig]
        int GetVideoSwapchainHandle(
            out IntPtr phSwapchain
            );

        [PreserveSig]
        int EnableHorizontalMirrorMode(
            [MarshalAs(UnmanagedType.Bool)] bool fEnable
            );

        [PreserveSig]
        int GetAudioStreamCategory(
            out AUDIO_STREAM_CATEGORY pCategory
            );

        [PreserveSig]
        int SetAudioStreamCategory(
            AUDIO_STREAM_CATEGORY category
            );

        [PreserveSig]
        int GetAudioEndpointRole(
            out ERole pRole
            );

        [PreserveSig]
        int SetAudioEndpointRole(
            ERole role
            );

        [PreserveSig]
        int GetRealTimeMode(
            [MarshalAs(UnmanagedType.Bool)] out bool pfEnabled
            );

        [PreserveSig]
        int SetRealTimeMode(
            [MarshalAs(UnmanagedType.Bool)] bool fEnable
            );

        [PreserveSig]
        int SetCurrentTimeEx(
            double seekTime,
            MF_MEDIA_ENGINE_SEEK_MODE seekMode
            );

        [PreserveSig]
        int EnableTimeUpdateTimer(
            [MarshalAs(UnmanagedType.Bool)] bool fEnableTimer
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("2f69d622-20b5-41e9-afdf-89ced1dda04e"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineExtension
    {
        [PreserveSig]
        int CanPlayType(
            [MarshalAs(UnmanagedType.Bool)] bool AudioOnly,
            [MarshalAs(UnmanagedType.BStr)] string MimeType,
            out MF_MEDIA_ENGINE_CANPLAY pAnswer
            );

        [PreserveSig]
        int BeginCreateObject(
            [MarshalAs(UnmanagedType.BStr)] string bstrURL,
            IMFByteStream pByteStream,
            MFObjectType type,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppIUnknownCancelCookie,
            IMFAsyncCallback pCallback,
            [In, MarshalAs(UnmanagedType.IUnknown)] object punkState
            );

        [PreserveSig]
        int CancelObjectCreation(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pIUnknownCancelCookie
            );

        [PreserveSig]
        int EndCreateObject(
            IMFAsyncResult pResult,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppObject
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("9f8021e8-9c8c-487e-bb5c-79aa4779938c"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineProtectedContent
    {
        [PreserveSig]
        int ShareResources(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkDeviceContext
            );

        [PreserveSig]
        int GetRequiredProtections(
            out MF_MEDIA_ENGINE_FRAME_PROTECTION_FLAGS pFrameProtectionFlags
            );

        [PreserveSig]
        int SetOPMWindow(
            IntPtr hwnd
            );

        [PreserveSig]
        int TransferVideoFrame(
            [In, MarshalAs(UnmanagedType.IUnknown)] object pDstSurf,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFVideoNormalizedRect pSrc,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFRect pDst,
            [In, MarshalAs(UnmanagedType.LPStruct)] MFARGB pBorderClr,
            out MF_MEDIA_ENGINE_FRAME_PROTECTION_FLAGS pFrameProtectionFlags
            );

        [PreserveSig]
        int SetContentProtectionManager(
            IMFContentProtectionManager pCPM
            );

        [PreserveSig]
        int SetApplicationCertificate(
            IntPtr pbBlob,
            int cbBlob
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("4D645ACE-26AA-4688-9BE1-DF3516990B93"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineClassFactory
    {
        [PreserveSig]
        int CreateInstance(
            MF_MEDIA_ENGINE_CREATEFLAGS dwFlags,
            IMFAttributes pAttr,
            out IMFMediaEngine ppPlayer
            );

        [PreserveSig]
        int CreateTimeRange(
            out IMFMediaTimeRange ppTimeRange
            );

        [PreserveSig]
        int CreateError(
            out IMFMediaError ppError
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("a7901327-05dd-4469-a7b7-0e01979e361d"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaSourceExtensionNotify
    {
        [PreserveSig]
        void OnSourceOpen();

        [PreserveSig]
        void OnSourceEnded();

        [PreserveSig]
        void OnSourceClose();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("24cd47f7-81d8-4785-adb2-af697a963cd2"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFBufferListNotify
    {
        [PreserveSig]
        void OnAddSourceBuffer();

        [PreserveSig]
        void OnRemoveSourceBuffer();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("87e47623-2ceb-45d6-9b88-d8520c4dcbbc"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSourceBufferNotify
    {
        [PreserveSig]
        void OnUpdateStart();

        [PreserveSig]
        void OnAbort();

        [PreserveSig]
        void OnError(int hr);

        [PreserveSig]
        void OnUpdate();

        [PreserveSig]
        void OnUpdateEnd();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("e2cd3a4b-af25-4d3d-9110-da0e6f8ee877"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSourceBuffer
    {
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool GetUpdating();

        [PreserveSig]
        int GetBuffered(
            out IMFMediaTimeRange ppBuffered
            );

        [PreserveSig]
        double GetTimeStampOffset();

        [PreserveSig]
        int SetTimeStampOffset(
            double offset
            );

        [PreserveSig]
        double GetAppendWindowStart();

        [PreserveSig]
        int SetAppendWindowStart(
            double time
            );

        [PreserveSig]
        double GetAppendWindowEnd();

        [PreserveSig]
        int SetAppendWindowEnd(
            double time
            );

        [PreserveSig]
        int Append(
            IntPtr pData,
            int len
            );

        [PreserveSig]
        int AppendByteStream(
            IMFByteStream pStream,
            long pMaxLen
            );

        [PreserveSig]
        int Abort();

        [PreserveSig]
        int Remove(
            double start,
            double end
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("249981f8-8325-41f3-b80c-3b9e3aad0cbe"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFSourceBufferList
    {
        [PreserveSig]
        int GetLength();

        [PreserveSig]
        IMFSourceBuffer GetSourceBuffer(
            int index
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("e467b94e-a713-4562-a802-816a42e9008a"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaSourceExtension
    {
        [PreserveSig]
        IMFSourceBufferList GetSourceBuffers();

        [PreserveSig]
        IMFSourceBufferList GetActiveSourceBuffers();

        [PreserveSig]
        MF_MSE_READY GetReadyState();

        [PreserveSig]
        double GetDuration();

        [PreserveSig]
        int SetDuration(
            double duration
            );

        [PreserveSig]
        int AddSourceBuffer(
            [MarshalAs(UnmanagedType.BStr)] string type,
            IMFSourceBufferNotify pNotify,
            out IMFSourceBuffer ppSourceBuffer
            );

        [PreserveSig]
        int RemoveSourceBuffer(
            IMFSourceBuffer pSourceBuffer
            );

        [PreserveSig]
        int SetEndOfStream(
            MF_MSE_ERROR error
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsTypeSupported(
            [MarshalAs(UnmanagedType.BStr)] string type
            );

        [PreserveSig]
        IMFSourceBuffer GetSourceBuffer(
            int dwStreamIndex
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("50dc93e4-ba4f-4275-ae66-83e836e57469"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineEME
    {
        [PreserveSig]
        int get_Keys(
           out IMFMediaKeys keys // check null
           );

        [PreserveSig]
        int SetMediaKeys(
           IMFMediaKeys keys
           );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("654a6bb3-e1a3-424a-9908-53a43a0dfda0"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineSrcElementsEx : IMFMediaEngineSrcElements
    {
        #region IMFMediaEngineSrcElements methods

        [PreserveSig]
        new int GetLength();

        [PreserveSig]
        new int GetURL(
            int index,
            [MarshalAs(UnmanagedType.BStr)] out string pURL
            );

        [PreserveSig]
        new int GetType(
            int index,
            [MarshalAs(UnmanagedType.BStr)] out string pType
            );

        [PreserveSig]
        new int GetMedia(
            int index,
            [MarshalAs(UnmanagedType.BStr)] out string pMedia
            );

        [PreserveSig]
        new int AddElement(
            [MarshalAs(UnmanagedType.BStr)] string pURL,
            [MarshalAs(UnmanagedType.BStr)] string pType,
            [MarshalAs(UnmanagedType.BStr)] string pMedia
            );

        [PreserveSig]
        new int RemoveAllElements();

        #endregion

        [PreserveSig]
        int AddElementEx(
            [MarshalAs(UnmanagedType.BStr)] string pURL,
            [MarshalAs(UnmanagedType.BStr)] string pType,
            [MarshalAs(UnmanagedType.BStr)] string pMedia,
            [MarshalAs(UnmanagedType.BStr)] string keySystem
            );

        [PreserveSig]
        int GetKeySystem(
            int index,
            [MarshalAs(UnmanagedType.BStr)] out string pType
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("46a30204-a696-4b18-8804-246b8f031bb1"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineNeedKeyNotify
    {
        [PreserveSig]
        void NeedKey(
           IntPtr initData,
           int cb
           );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("5cb31c05-61ff-418f-afda-caaf41421a38"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaKeys
    {
        [PreserveSig]
        int CreateSession(
           [MarshalAs(UnmanagedType.BStr)] string mimeType,
           IntPtr initData,
           int cb,
           IntPtr customData,
           int cbCustomData,
           IMFMediaKeySessionNotify notify,
           out IMFMediaKeySession ppSession
           );

        [PreserveSig]
        int get_KeySystem(
           [MarshalAs(UnmanagedType.BStr)] out string keySystem
           );

        [PreserveSig]
        int Shutdown();

        [PreserveSig]
        int GetSuspendNotify(
           out IMFCdmSuspendNotify notify
           );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("24fa67d5-d1d0-4dc5-995c-c0efdc191fb5"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaKeySession
    {
        [PreserveSig]
        int GetError(
           out short code,
           out int systemCode);

        [PreserveSig]
        int get_KeySystem(
           [MarshalAs(UnmanagedType.BStr)] out string keySystem
           );

        [PreserveSig]
        int get_SessionId(
           [MarshalAs(UnmanagedType.BStr)] out string sessionId
           );

        [PreserveSig]
        int Update(
           IntPtr key,
           int cb
           );

        [PreserveSig]
        int Close();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("6a0083f9-8947-4c1d-9ce0-cdee22b23135"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaKeySessionNotify
    {
        [PreserveSig]
        void KeyMessage(
           [MarshalAs(UnmanagedType.BStr)] string destinationURL,
           IntPtr message,
           int cb
           );

        [PreserveSig]
        void KeyAdded();

        [PreserveSig]
        void KeyError(
           short code,
           int systemCode
           );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("7a5645d2-43bd-47fd-87b7-dcd24cc7d692"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFCdmSuspendNotify
    {
        [PreserveSig]
        int Begin();

        [PreserveSig]
        int End();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("765763e6-6c01-4b01-bb0f-b829f60ed28c"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineOPMInfo
    {
        [PreserveSig]
        int GetOPMInfo(
            out MF_MEDIA_ENGINE_OPM_STATUS pStatus,
            [MarshalAs(UnmanagedType.Bool)] out bool pConstricted
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("c56156c6-ea5b-48a5-9df8-fbe035d0929e"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineClassFactoryEx : IMFMediaEngineClassFactory
    {
        #region IMFMediaEngineClassFactory methods

        [PreserveSig]
        new int CreateInstance(
            MF_MEDIA_ENGINE_CREATEFLAGS dwFlags,
            IMFAttributes pAttr,
            out IMFMediaEngine ppPlayer
            );

        [PreserveSig]
        new int CreateTimeRange(
            out IMFMediaTimeRange ppTimeRange
            );

        [PreserveSig]
        new int CreateError(
            out IMFMediaError ppError
            );

        #endregion

        [PreserveSig]
        int CreateMediaSourceExtension(
            int dwFlags,
            IMFAttributes pAttr,
            out IMFMediaSourceExtension ppMSE
            );

        [PreserveSig]
        int CreateMediaKeys(
            [MarshalAs(UnmanagedType.BStr)] string keySystem,
            [MarshalAs(UnmanagedType.BStr)] string cdmStorePath,
            out IMFMediaKeys ppKeys
            );

        [PreserveSig]
        int IsTypeSupported(
            [MarshalAs(UnmanagedType.BStr)] string type,
            [MarshalAs(UnmanagedType.BStr)] string keySystem,
            [MarshalAs(UnmanagedType.Bool)] out bool isSupported
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("09083cef-867f-4bf6-8776-dee3a7b42fca"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineClassFactory2
    {
        [PreserveSig]
        int CreateMediaKeys2(
            [MarshalAs(UnmanagedType.BStr)] string keySystem,
            [MarshalAs(UnmanagedType.BStr)] string defaultCdmStorePath,
            [MarshalAs(UnmanagedType.BStr)] string inprivateCdmStorePath,
            out IMFMediaKeys ppKeys
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("a724b056-1b2e-4642-a6f3-db9420c52908"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFMediaEngineSupportsSourceTransfer
    {
        [PreserveSig]
        int ShouldTransferSource(
          [MarshalAs(UnmanagedType.Bool)] out bool pfShouldTransfer
          );

        [PreserveSig]
        int DetachMediaSource(
            out IMFByteStream ppByteStream,
            out IMFMediaSource ppMediaSource,
            out IMFMediaSourceExtension ppMSE
            );

        [PreserveSig]
        int AttachMediaSource(
            IMFByteStream pByteStream,
            IMFMediaSource pMediaSource,
            IMFMediaSourceExtension pMSE
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("EBBAF249-AFC2-4582-91C6-B60DF2E84954")]
    public interface IAudioSourceProvider
    {
        [PreserveSig]
        int ProvideInput(
        int dwSampleCount,
        ref int pdwChannelCount,
        out float[] pInterleavedAudioData
        );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("ba2743a1-07e0-48ef-84b6-9a2ed023ca6c")]
    public interface IMFMediaEngineWebSupport
    {
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool ShouldDelayTheLoadEvent();

        [PreserveSig]
        int ConnectWebAudio(
            int dwSampleRate,
            out IAudioSourceProvider ppSourceProvider
            );

        [PreserveSig]
        int DisconnectWebAudio();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("19666fb4-babe-4c55-bc03-0a074da37e2a")]
    public interface IMFSourceBufferAppendMode
    {
        [PreserveSig]
        MF_MSE_APPEND_MODE GetAppendMode();

        [PreserveSig]
        int SetAppendMode(
            MF_MSE_APPEND_MODE mode
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("1f2a94c9-a3df-430d-9d0f-acd85ddc29af")]
    public interface IMFTimedText
    {
        [PreserveSig]
        int RegisterNotifications(
            IMFTimedTextNotify notify
            );

        [PreserveSig]
        int SelectTrack(
            int trackId,
            [MarshalAs(UnmanagedType.Bool)] bool selected
            );

        [PreserveSig]
        int AddDataSource(
            IMFByteStream byteStream,
            [In, MarshalAs(UnmanagedType.LPWStr)] string label,
            [In, MarshalAs(UnmanagedType.LPWStr)] string language,
            MF_TIMED_TEXT_TRACK_KIND kind,
            [MarshalAs(UnmanagedType.Bool)] bool isDefault,
            out int trackId
            );

        [PreserveSig]
        int AddDataSourceFromUrl(
            [In, MarshalAs(UnmanagedType.LPWStr)] string url,
            [In, MarshalAs(UnmanagedType.LPWStr)] string label,
            [In, MarshalAs(UnmanagedType.LPWStr)] string language,
            MF_TIMED_TEXT_TRACK_KIND kind,
            [MarshalAs(UnmanagedType.Bool)] bool isDefault,
            out int trackId
            );

        [PreserveSig]
        int AddTrack(
            [In, MarshalAs(UnmanagedType.LPWStr)] string label,
            [In, MarshalAs(UnmanagedType.LPWStr)] string language,
            MF_TIMED_TEXT_TRACK_KIND kind,
            out IMFTimedTextTrack track
            );

        [PreserveSig]
        int RemoveTrack(
            IMFTimedTextTrack track
            );

        [PreserveSig]
        int GetCueTimeOffset(
            out double offset
            );

        [PreserveSig]
        int SetCueTimeOffset(
            double offset
            );

        [PreserveSig]
        int GetTracks(
            out IMFTimedTextTrackList tracks
            );

        [PreserveSig]
        int GetActiveTracks(
            out IMFTimedTextTrackList activeTracks
            );

        [PreserveSig]
        int GetTextTracks(
            out IMFTimedTextTrackList textTracks
            );

        [PreserveSig]
        int GetMetadataTracks(
            out IMFTimedTextTrackList metadataTracks
            );

        [PreserveSig]
        int SetInBandEnabled(
            [MarshalAs(UnmanagedType.Bool)] bool enabled
        );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsInBandEnabled();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("df6b87b6-ce12-45db-aba7-432fe054e57d")]
    public interface IMFTimedTextNotify
    {
        void TrackAdded(
            int trackId
            );

        void TrackRemoved(
            int trackId
            );

        void TrackSelected(
            int trackId,
            [MarshalAs(UnmanagedType.Bool)] bool selected
            );

        void TrackReadyStateChanged(
            int trackId
            );

        void Error(
            MF_TIMED_TEXT_ERROR_CODE errorCode,
            int extendedErrorCode,
            int sourceTrackId
            );

        void Cue(
            MF_TIMED_TEXT_CUE_EVENT cueEvent,
            double currentTime,
            IMFTimedTextCue cue
            );

        void Reset();
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("8822c32d-654e-4233-bf21-d7f2e67d30d4")]
    public interface IMFTimedTextTrack
    {
        [PreserveSig]
        int GetId();

        [PreserveSig]
        int GetLabel(
            [MarshalAs(UnmanagedType.LPWStr)] out string label
            );

        [PreserveSig]
        int SetLabel(
            [In, MarshalAs(UnmanagedType.LPWStr)] string label
            );

        [PreserveSig]
        int GetLanguage(
            [MarshalAs(UnmanagedType.LPWStr)] out string language
            );

        [PreserveSig]
        MF_TIMED_TEXT_TRACK_KIND GetTrackKind();

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsInBand();

        [PreserveSig]
        int GetInBandMetadataTrackDispatchType(
            [MarshalAs(UnmanagedType.LPWStr)] out string dispatchType
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsActive();

        [PreserveSig]
        MF_TIMED_TEXT_ERROR_CODE GetErrorCode();

        [PreserveSig]
        int GetExtendedErrorCode();

        [PreserveSig]
        int GetDataFormat(out Guid format);

        [PreserveSig]
        MF_TIMED_TEXT_TRACK_READY_STATE GetReadyState();

        [PreserveSig]
        int GetCueList(
    out IMFTimedTextCueList cues
    );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("23ff334c-442c-445f-bccc-edc438aa11e2")]
    public interface IMFTimedTextTrackList
    {
        [PreserveSig]
        int GetLength();

        [PreserveSig]
        int GetTrack(
            int index,
            out IMFTimedTextTrack track
            );

        [PreserveSig]
        int GetTrackById(
            int trackId,
            out IMFTimedTextTrack track
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("1e560447-9a2b-43e1-a94c-b0aaabfbfbc9")]
    public interface IMFTimedTextCue
    {
        [PreserveSig]
        int GetId();

        [PreserveSig]
        int GetOriginalId(
            [MarshalAs(UnmanagedType.LPWStr)] out string originalId
            );

        [PreserveSig]
        MF_TIMED_TEXT_TRACK_KIND GetCueKind();

        [PreserveSig]
        double GetStartTime();

        [PreserveSig]
        double GetDuration();

        [PreserveSig]
        int GetTrackId();

        [PreserveSig]
        int GetData(
            out IMFTimedTextBinary data
            );

        [PreserveSig]
        int GetRegion(
            out IMFTimedTextRegion region
            );

        [PreserveSig]
        int GetStyle(
            out IMFTimedTextStyle style
            );

        [PreserveSig]
        int GetLineCount();

        [PreserveSig]
        int GetLine(
            int index,
            out IMFTimedTextFormattedText line
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("e13af3c1-4d47-4354-b1f5-e83ae0ecae60")]
    public interface IMFTimedTextFormattedText
    {
        [PreserveSig]
        int GetText(
            [MarshalAs(UnmanagedType.LPWStr)] out string text
            );

        [PreserveSig]
        int GetSubformattingCount();

        [PreserveSig]
        int GetSubformatting(
            int index,
            out int firstChar,
            out int charLength,
            out IMFTimedTextStyle style
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("09b2455d-b834-4f01-a347-9052e21c450e")]
    public interface IMFTimedTextStyle
    {
        [PreserveSig]
        int GetName(
            [MarshalAs(UnmanagedType.LPWStr)] out string name
            );

        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsExternal();

        [PreserveSig]
        int GetFontFamily(
            [MarshalAs(UnmanagedType.LPWStr)] out string fontFamily
            );

        [PreserveSig]
        int GetFontSize(
            out double fontSize,
            out MF_TIMED_TEXT_UNIT_TYPE unitType
            );

        [PreserveSig]
        int GetColor(
            out MFARGB color
            );

        [PreserveSig]
        int GetBackgroundColor(
            out MFARGB bgColor
            );

        [PreserveSig]
        int GetShowBackgroundAlways(
            [MarshalAs(UnmanagedType.Bool)] out bool showBackgroundAlways
            );

        [PreserveSig]
        int GetFontStyle(
            out MF_TIMED_TEXT_FONT_STYLE fontStyle
            );

        [PreserveSig]
        int GetBold(
            [MarshalAs(UnmanagedType.Bool)] out bool bold
            );

        [PreserveSig]
        int GetRightToLeft(
            [MarshalAs(UnmanagedType.Bool)] out bool rightToLeft
            );

        [PreserveSig]
        int GetTextAlignment(
            out MF_TIMED_TEXT_ALIGNMENT textAlign
            );

        [PreserveSig]
        int GetTextDecoration(
            out int textDecoration
            );

        [PreserveSig]
        int GetTextOutline(
            out MFARGB color,
            out double thickness,
            out double blurRadius,
            out MF_TIMED_TEXT_UNIT_TYPE unitType
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("c8d22afc-bc47-4bdf-9b04-787e49ce3f58")]
    public interface IMFTimedTextRegion
    {
        [PreserveSig]
        int GetName(
            [MarshalAs(UnmanagedType.LPWStr)] out string name
            );

        [PreserveSig]
        int GetPosition(
            out double pX,
            out double pY,
            out MF_TIMED_TEXT_UNIT_TYPE unitType
            );

        [PreserveSig]
        int GetExtent(
            out double pWidth,
            out double pHeight,
            out MF_TIMED_TEXT_UNIT_TYPE unitType
            );

        [PreserveSig]
        int GetBackgroundColor(
            out MFARGB bgColor
            );

        [PreserveSig]
        int GetWritingMode(
            out MF_TIMED_TEXT_WRITING_MODE writingMode
            );

        [PreserveSig]
        int GetDisplayAlignment(
            out MF_TIMED_TEXT_DISPLAY_ALIGNMENT displayAlign
            );

        [PreserveSig]
        int GetLineHeight(
            out double pLineHeight,
            out MF_TIMED_TEXT_UNIT_TYPE unitType
            );

        [PreserveSig]
        int GetClipOverflow(
            [MarshalAs(UnmanagedType.Bool)] out bool clipOverflow
            );

        [PreserveSig]
        int GetPadding(
            out double before,
            out double start,
            out double after,
            out double end,
            out MF_TIMED_TEXT_UNIT_TYPE unitType
            );

        [PreserveSig]
        int GetWrap(
            [MarshalAs(UnmanagedType.Bool)] out bool wrap
            );

        [PreserveSig]
        int GetZIndex(
            out int zIndex
            );

        [PreserveSig]
        int GetScrollMode(
            out MF_TIMED_TEXT_SCROLL_MODE scrollMode
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("4ae3a412-0545-43c4-bf6f-6b97a5c6c432")]
    public interface IMFTimedTextBinary
    {
        [PreserveSig]
        int GetData(
            out byte[] data,
            out int length
            );
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("ad128745-211b-40a0-9981-fe65f166d0fd")]
    public interface IMFTimedTextCueList
    {
        [PreserveSig]
        int GetLength();

        [PreserveSig]
        int GetCueByIndex(
            int index,
            out IMFTimedTextCue cue
            );

        [PreserveSig]
        int GetCueById(
            int id,
            out IMFTimedTextCue cue
            );

        [PreserveSig]
        int GetCueByOriginalId(
            [In, MarshalAs(UnmanagedType.LPWStr)] string originalId,
            out IMFTimedTextCue cue
            );

        [PreserveSig]
        int AddTextCue(
            double start,
            double duration,
            [In, MarshalAs(UnmanagedType.LPWStr)] string text,
            out IMFTimedTextCue cue
            );

        [PreserveSig]
        int AddDataCue(
            double start,
            double duration,
            byte[] data,
            int dataSize,
            out IMFTimedTextCue cue
            );

        [PreserveSig]
        int RemoveCue(
            IMFTimedTextCue cue
            );
    }

#endif

    #endregion

}
