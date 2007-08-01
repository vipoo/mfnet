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
using System.Text;
using System.Runtime.InteropServices;

using MediaFoundation.Misc;

namespace MediaFoundation
{
    #region Declarations

#if ALLOW_UNTESTED_INTERFACES

    [UnmanagedName("ASF_SELECTION_STATUS")]
    public enum ASFSelectionStatus
    {	
        NotSelected	= 0,
	    CleanPointsOnly	= 1,
	    AllDataUnits	= 2
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("ASF_MUX_STATISTICS")]
    public struct ASFMuxStatistics
    {
        int cFramesWritten;
        int cFramesDropped;
    }

    [StructLayout(LayoutKind.Sequential), UnmanagedName("ASF_INDEX_IDENTIFIER")]
    public struct ASFIndexIdentifier
    {
        Guid guidIndexType;
        short wStreamNumber;
    }
#endif

    #endregion

    #region Interfaces

#if ALLOW_UNTESTED_INTERFACES

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("B1DCA5CD-D5DA-4451-8E9E-DB5C59914EAD"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFContentInfo
    {
        void GetHeaderSize(
            [In] IMFMediaBuffer pIStartOfContent,
            out long cbHeaderSize);
        
        void ParseHeader(
            [In] IMFMediaBuffer pIHeaderBuffer,
            [In] long cbOffsetWithinHeader);
        
        void GenerateHeader( 
            [In] IMFMediaBuffer pIHeader,
            out int pcbHeader);
        
        void GetProfile(
            out IMFASFProfile ppIProfile);
        
        void SetProfile(
            [In] IMFASFProfile pIProfile);
        
        void GeneratePresentationDescriptor(
            out IMFPresentationDescriptor ppIPresentationDescriptor);

        void GetEncodingConfigurationPropertyStore(
            [In] short wStreamNumber,
            out IPropertyStore ppIStore);
        
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("53590F48-DC3B-4297-813F-787761AD7B3E"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFIndexer
    {
        void SetFlags( 
            [In] int dwFlags);
        
        void GetFlags(
            out int pdwFlags);
        
        void Initialize(
            [In] IMFASFContentInfo pIContentInfo);
        
        void GetIndexPosition(
            [In] IMFASFContentInfo pIContentInfo,
            out long pcbIndexOffset);
        
        void SetIndexByteStreams(
            [In] IMFByteStream[] ppIByteStreams,
            [In] int cByteStreams);
        
        void GetIndexByteStreamCount(
            out int pcByteStreams);
        
        void GetIndexStatus(
            [In] ASFIndexIdentifier pIndexIdentifier,
            out bool pfIsIndexed,
            out byte pbIndexDescriptor,
            out int pcbIndexDescriptor);
        
        void SetIndexStatus(
            [In] IntPtr pbIndexDescriptor,
            [In] int cbIndexDescriptor,
            [In] bool fGenerateIndex);
        
        void GetSeekPositionForValue(
            [In] PropVariant pvarValue,
            [In] ASFIndexIdentifier pIndexIdentifier,
            out long pcbOffsetWithinData,
            out long phnsApproxTime,
            out int pdwPayloadNumberOfStreamWithinPacket);
        
        void GenerateIndexEntries(
            [In] IMFSample pIASFPacketSample);
        
        void CommitIndex(
            [In] IMFASFContentInfo pIContentInfo);
        
        void GetIndexWriteSpace(
            out long pcbIndexWriteSpace);
        
        void GetCompletedIndex(
            [In] IMFMediaBuffer pIIndexBuffer,
            [In] long cbOffsetWithinIndex);
        
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("57BDD80A-9B38-4838-B737-C58F670D7D4F"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFMultiplexer
    {
        void Initialize(
            [In] IMFASFContentInfo pIContentInfo);
        
        void SetFlags(
            [In] int dwFlags);
        
        void GetFlags(
            out int pdwFlags);
        
        void ProcessSample(
            [In] short wStreamNumber,
            [In] IMFSample pISample,
            [In] long hnsTimestampAdjust);
        
        void GetNextPacket(
            out int pdwStatusFlags,
            out IMFSample ppIPacket);
        
        void Flush( );
        
        void End( 
            [In] IMFASFContentInfo pIContentInfo);
        
        void GetStatistics(
            [In] short wStreamNumber,
            out ASFMuxStatistics pMuxStats);
        
        void SetSyncTolerance(
            [In] int msSyncTolerance);
        
    }
    
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("12558291-E399-11D5-BC2A-00B0D0F3F4AB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFMutualExclusion
    {
        void GetType(
            out Guid pguidType);
        
        void SetType( 
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidType);
        
        void GetRecordCount(
            out int pdwRecordCount);
        
        void GetStreamsForRecord(
            [In] int dwRecordNumber,
            out short pwStreamNumArray,
            out int pcStreams);
        
        void AddStreamForRecord(
            [In] int dwRecordNumber,
            [In] short wStreamNumber);
        
        void RemoveStreamFromRecord(
            [In] int dwRecordNumber,
            [In] short wStreamNumber);
        
        void RemoveRecord(
            [In] int dwRecordNumber);
        
        void AddRecord(
            out int pdwRecordNumber);

        void Clone(
            out IMFASFMutualExclusion ppIMutex);
        
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("D267BF6A-028B-4e0d-903D-43F0EF82D0D4"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFProfile : IMFAttributes
    {
        #region IMFAttributes methods

        new void GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        new void GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        new void CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] PropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        new void Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        new void GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        new void GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        new void GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        new void GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        new void GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        new void GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        new void GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        new void GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        new void GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        new void GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        new void GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        new void SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] PropVariant Value
            );

        new void DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        new void DeleteAllItems();

        new void SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        new void SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        new void SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        new void SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        new void SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        new void SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        new void SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        new void LockStore();

        new void UnlockStore();

        new void GetCount(
            out int pcItems
            );

        new void GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        new void CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        void GetStreamCount(
            out int pcStreams);
        
        void GetStream(
            [In] int dwStreamIndex,
            out short pwStreamNumber,
            out IMFASFStreamConfig ppIStream);
        
        void GetStreamByNumber(
            [In] short wStreamNumber,
            out IMFASFStreamConfig ppIStream);
        
        void SetStream(
            [In] IMFASFStreamConfig pIStream);
        
        void RemoveStream(
            [In] short wStreamNumber);
        
        void CreateStream(
            [In] IMFMediaType pIMediaType,
            out IMFASFStreamConfig ppIStream);
        
        void GetMutualExclusionCount(
            out int pcMutexs);
        
        void GetMutualExclusion(
            [In] int dwMutexIndex,
            out IMFASFMutualExclusion ppIMutex);
        
        void AddMutualExclusion(
            [In] IMFASFMutualExclusion pIMutex);
        
        void RemoveMutualExclusion(
            [In] int dwMutexIndex);
        
        void CreateMutualExclusion( 
            out IMFASFMutualExclusion ppIMutex);
        
        void GetStreamPrioritization(
            out IMFASFStreamPrioritization ppIStreamPrioritization);
        
        void AddStreamPrioritization(
            [In] IMFASFStreamPrioritization pIStreamPrioritization);
        
        void RemoveStreamPrioritization( );
        
        void CreateStreamPrioritization(
            out IMFASFStreamPrioritization ppIStreamPrioritization);
        
        void Clone(
            out IMFASFProfile ppIProfile);
        
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("12558295-E399-11D5-BC2A-00B0D0F3F4AB"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFSplitter
    {
        void Initialize(
            [In] IMFASFContentInfo pIContentInfo);
        
        void SetFlags(
            [In] int dwFlags);
        
        void GetFlags(
            out int pdwFlags);
        
        void SelectStreams(
            [In] short[] pwStreamNumbers,
            [In] short wNumStreams);
        
        void GetSelectedStreams( 
            [Out] short [] pwStreamNumbers,
            out short pwNumStreams);
        
        void ParseData(
            [In] IMFMediaBuffer pIBuffer,
            [In] int cbBufferOffset,
            [In] int cbLength);
        
        void GetNextSample(
            out int pdwStatusFlags,
            out short pwStreamNumber,
            out IMFSample ppISample);
        
        void Flush( );
        
        void GetLastSendTime(
            out int pdwLastSendTime);
        
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("9E8AE8D2-DBBD-4200-9ACA-06E6DF484913"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFStreamConfig : IMFAttributes
    {
        #region IMFAttributes methods

        new void GetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        new void GetItemType(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out MFAttributeType pType
            );

        new void CompareItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] PropVariant Value,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        new void Compare(
            [MarshalAs(UnmanagedType.Interface)] IMFAttributes pTheirs,
            MFAttributesMatchType MatchType,
            [MarshalAs(UnmanagedType.Bool)] out bool pbResult
            );

        new void GetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int punValue
            );

        new void GetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out long punValue
            );

        new void GetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out double pfValue
            );

        new void GetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out Guid pguidValue
            );

        new void GetStringLength(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcchLength
            );

        new void GetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszValue,
            int cchBufSize,
            out int pcchLength
            );

        new void GetAllocatedString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [MarshalAs(UnmanagedType.LPWStr)] out string ppwszValue,
            out int pcchLength
            );

        new void GetBlobSize(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out int pcbBlobSize
            );

        new void GetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pBuf,
            int cbBufSize,
            out int pcbBlobSize
            );

        // Use GetBlob instead of this
        new void GetAllocatedBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            out IntPtr ip,  // Read w/Marshal.Copy, Free w/Marshal.FreeCoTaskMem
            out int pcbSize
            );

        new void GetUnknown(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppv
            );

        new void SetItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] PropVariant Value
            );

        new void DeleteItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey
            );

        new void DeleteAllItems();

        new void SetUINT32(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            int unValue
            );

        new void SetUINT64(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            long unValue
            );

        new void SetDouble(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            double fValue
            );

        new void SetGUID(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidValue
            );

        new void SetString(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPWStr)] string wszValue
            );

        new void SetBlob(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pBuf,
            int cbBufSize
            );

        new void SetUnknown(
            [MarshalAs(UnmanagedType.LPStruct)] Guid guidKey,
            [In, MarshalAs(UnmanagedType.IUnknown)] object pUnknown
            );

        new void LockStore();

        new void UnlockStore();

        new void GetCount(
            out int pcItems
            );

        new void GetItemByIndex(
            int unIndex,
            out Guid pguidKey,
            [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(PVMarshaler))] PropVariant pValue
            );

        new void CopyAllItems(
            [In, MarshalAs(UnmanagedType.Interface)] IMFAttributes pDest
            );

        #endregion

        void GetStreamType(
            out Guid pguidStreamType);
        
        [PreserveSig]
        short GetStreamNumber( );
        
        void SetStreamNumber(
            [In] short wStreamNum);
        
        void GetMediaType(
            out IMFMediaType ppIMediaType);
        
        void SetMediaType(
            [In] IMFMediaType pIMediaType);
        
        void GetPayloadExtensionCount(
            out short pcPayloadExtensions);
        
        void GetPayloadExtension(
            [In] short wPayloadExtensionNumber,
            out Guid pguidExtensionSystemID,
            out short pcbExtensionDataSize,
            IntPtr pbExtensionSystemInfo,
            out int pcbExtensionSystemInfo);
        
        void AddPayloadExtension( 
            [In, MarshalAs(UnmanagedType.Struct)] Guid guidExtensionSystemID,
            [In] short cbExtensionDataSize,
            IntPtr pbExtensionSystemInfo,
            [In] int cbExtensionSystemInfo);
        
        void RemoveAllPayloadExtensions( );
        
        void Clone(
            out IMFASFStreamConfig ppIStreamConfig);
        
    }

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("699bdc27-bbaf-49ff-8e38-9c39c9b5e088"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFStreamPrioritization
    {
        void GetStreamCount(
            out int pdwStreamCount);
        
        void GetStream(
            [In] int dwStreamIndex,
            out short pwStreamNumber,
            out short pwStreamFlags);
        
        void AddStream(
            [In] short wStreamNumber,
            [In] short wStreamFlags);
        
        void RemoveStream(
            [In] int dwStreamIndex);

        void Clone(
            out IMFASFStreamPrioritization ppIStreamPrioritization);
        
    }
    
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid("d01bad4a-4fa0-4a60-9349-c27e62da9d41"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMFASFStreamSelector
    {
        void GetStreamCount(
            out int pcStreams);
        
        void GetOutputCount(
            out int pcOutputs);
        
        void GetOutputStreamCount(
            [In] int dwOutputNum,
            out int pcStreams);
        
        void GetOutputStreamNumbers(
            [In] int dwOutputNum,
            [Out] short [] rgwStreamNumbers);
        
        void GetOutputFromStream(
            [In] short wStreamNum,
            out int pdwOutput);
        
        void GetOutputOverride(
            [In] int dwOutputNum,
            out ASFSelectionStatus pSelection);
        
        void SetOutputOverride(
            [In] int dwOutputNum,
            [In] ASFSelectionStatus Selection);
        
        void GetOutputMutexCount(
            [In] int dwOutputNum,
            out int pcMutexes);
        
        void GetOutputMutex(
            [In] int dwOutputNum,
            [In] int dwMutexNum,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppMutex);
        
        void SetOutputMutexSelection(
            [In] int dwOutputNum,
            [In] int dwMutexNum,
            [In] short wSelectedRecord);
        
        void GetBandwidthStepCount( 
            out int pcStepCount);
        
        void GetBandwidthStep(
            [In] int dwStepNum,
            out int pdwBitrate,
            out short rgwStreamNumbers,
            out ASFSelectionStatus rgSelections);
        
        void BitrateToStepNumber(
            [In] int dwBitrate,
            out int pdwStepNum);
        
        void SetStreamSelectorFlags(
            [In] int dwStreamSelectorFlags);

    }

#endif

    #endregion
}
