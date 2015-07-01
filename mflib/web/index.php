<?php

/* Redirect input urls from VS Help F1 to MSDN pages.  See "Microsoft Help Viewer SDK" at
https://msdn.microsoft.com/en-us/library/dd627473.aspx

Input urls look like this:

http://mfnet.sourceforge.net/help/?F1=MediaFoundation.IMFASFMultiplexer&locale=en-US&codeLang=csharp
http://mfnet.sourceforge.net/help/?F1=MediaFoundation.IMFASFMultiplexer.ProcessSample&locale=en-US&codeLang=csharp
http://mfnet.sourceforge.net/help/?F1=MediaFoundation.Transform.IMFTransform.GetStreamLimits&locale=en-US&codeLang=csharp

We need to redirect this to MSDN-formatted links that look like this:

https://msdn.microsoft.com/query/dev12.query?appId=Dev12IDEF1&l=EN-US&k=k(IMFASFMultiplexer)
https://msdn.microsoft.com/query/dev12.query?appId=Dev12IDEF1&l=EN-US&k=k(IMFASFMultiplexer::ProcessSample)
https://msdn.microsoft.com/query/dev12.query?appId=Dev12IDEF1&l=EN-US&k=k(IMFTransform::GetStream)

In theory, the codeLang could be processed too (MSDN: "k(DevLang-csharp)"), but since MediaFoundation has 
no support for c# et al, what's the point?

*/

// Log the request
$headers = getallheaders();
$logmsg = Date("YmdHis") . "\t" . $headers['X-Remote-Addr'] . "\t" . str_replace(array("\n", "\t", "\r"), "", $_SERVER['REQUEST_URI']);
$logmsg = substr($logmsg, 0, 254) . "\r\n";

@file_put_contents("/home/project-web/mfnet/persistent/log.txt", $logmsg, FILE_APPEND | LOCK_EX);

// Split out all the parts.  $symbols will have no more than 4 elements.
$symbols = explode(".", $_GET['F1'], 4);

if ($symbols[0] == 'NoHelp')
{
   print("This is the NoHelp test for https://connect.microsoft.com/VisualStudio/feedback/details/1219146<br><br>The fact that you are seeing this means that the redirection worked.<br>You requested url: " . $_SERVER['REQUEST_URI']);
   exit;
}

if ($symbols[0] != 'MediaFoundation')
{
   print("Unrecognized namespace in uRl: " . $_SERVER['REQUEST_URI']);
   exit;
}

// Remove 'MediaFoundation' that all requests should have.  $symbols now <= 3 elements.
array_shift($symbols);

// Special case the Alt namespace to remove the suffix from the interface names.
if ($symbols[0] == 'Alt')
{
   array_shift($symbols);
   $symbols[0] = substr($symbols[0], 0, -3);
}

// For nested namespaces, remove the namespace.
$skip1 = array('dxvahd', 'EVR', 'MFPlayer', 'Misc', 'OPM', 'ReadWrite', 'Transform');
if (array_search($symbols[0], $skip1) !== false)
{
   array_shift($symbols);
}

// $symbols now <= 2 elements.

// Remove static class names.  This code could be combined 
// with skip1 since they don't overlap, but for clarity...

$skip2 = array('DXVAHDETWGUID', 'OPMExtern', 'MFExtern', 'MF_MEDIA_ENGINE', 'MFAttributesClsid', 
   'MF_MEDIA_SHARING_ENGINE', 'MF_CAPTURE_ENGINE', 'MFTranscodeContainerType', 'MFConnector', 
   'MFTransformCategory', 'MFEnabletype', 'MFRepresentation', 'MFProperties', 'MFServices', 
   'MFPKEY', 'CLSID', 'MFMediaType', 'MFImageFormat', 'MFError', 'MFOpmStatusRequests', 
   'OpmConstants', 'MFPKEY_ASFMEDIASINK', 'MFASFSampleExtension');

if (array_search($symbols[0], $skip2) !== false)
{
   array_shift($symbols); // Remove static class name
}

$count = count($symbols);
if ($count > 1)
{
   // If there are still 2 left, we are looking at a method on an interface.  To get that to work,
   // every interface has to be added to the registry as a namespace.  Otherwise, VS won't recognize it.
   $use = $symbols[0] . "::" . $symbols[1];
}
else
{
   // Turns out, none of the struct/enum names start with 'I', but the interfaces (which already
   // have the correct names) do.  Skip if possible.
   if (substr($symbols[0], 0, 1) !== 'I')
   {
      $namemap = Array(
         "AMMediaType" => "AM_MEDIA_TYPE",
         "ASFIndexIdentifier" => "ASF_INDEX_IDENTIFIER",
         "ASFMuxStatistics" => "ASF_MUX_STATISTICS",
         "ASFSelectionStatus" => "ASF_SELECTION_STATUS",
         "ASFStatusFlags" => "ASF_STATUSFLAGS",
         "BitmapInfoHeaderWithData" => "BITMAPINFO",
         "DXVA2ProcAmpValues" => "DXVA2_ProcAmpValues",
         "DXVA2ValueRange" => "DXVA2_ValueRange",
         "DXVA2VideoProcessorCaps" => "DXVA2_VideoProcessorCaps",
         "MF_LeakyBucketPair" => "MF_LEAKY_BUCKET_PAIR",
         "MFAsfIndexerFlags" => "MFASF_INDEXERFLAGS",
         "MFASFMultiplexerFlags" => "MFASF_MULTIPLEXERFLAGS",
         "MFASFSplitterFlags" => "MFASF_SPLITTERFLAGS",
         "MFAsfStreamSelectorFlags" => "MFASF_STREAMSELECTORFLAGS",
         "MFAttributeSerializeOptions" => "MF_ATTRIBUTE_SERIALIZE_OPTIONS",
         "MFAttributesMatchType" => "MF_ATTRIBUTES_MATCH_TYPE",
         "MFAttributeType" => "MF_ATTRIBUTE_TYPE",
         "MFByteStreamBufferingParams" => "MFBYTESTREAM_BUFFERING_PARAMS",
         "MFByteStreamSeekOrigin" => "MFBYTESTREAM_SEEK_ORIGIN",
         "MFClockCharacteristicsFlags" => "MFCLOCK_CHARACTERISTICS_FLAGS",
         "MFClockProperties" => "MFCLOCK_PROPERTIES",
         "MFClockRelationalFlags" => "MFCLOCK_RELATIONAL_FLAGS",
         "MFClockState" => "MFCLOCK_STATE",
         "MFFileAccessMode" => "MF_FILE_ACCESSMODE",
         "MFFileFlags" => "MF_FILE_FLAGS",
         "MFFileOpenMode" => "MF_FILE_OPENMODE",
         "MFMediaSourceCharacteristics" => "MFMEDIASOURCE_CHARACTERISTICS",
         "MFNetSourceProtocolType" => "MFNETSOURCE_PROTOCOL_TYPE",
         "MFObjectType" => "MF_OBJECT_TYPE",
         "MFPluginType" => "MF_Plugin_Type",
         "MFPMPSessionCreationFlags" => "MFPMPSESSION_CREATION_FLAGS",
         "MFQualityAdviseFlags" => "MF_QUALITY_ADVISE_FLAGS",
         "MFQualityDropMode" => "MF_QUALITY_DROP_MODE",
         "MFQualityLevel" => "MF_QUALITY_LEVEL",
         "MFRateDirection" => "MFRATE_DIRECTION",
         "MFServiceLookupType" => "MF_SERVICE_LOOKUP_TYPE",
         "MFSessionGetFullTopologyFlags" => "MFSESSION_GETFULLTOPOLOGY_FLAGS",
         "MFSessionSetTopologyFlags" => "MFSESSION_SETTOPOLOGY_FLAGS",
         "MFShutdownStatus" => "MFSHUTDOWN_STATUS",
         "MFSize" => "SIZE",
         "MFStreamSinkMarkerType" => "MFSTREAMSINK_MARKER_TYPE",
         "MFT_EnumFlag" => "MFT_ENUM_FLAG",
         "MFTDrainType" => "_MFT_DRAIN_TYPE",
         "MFTimeFlags" => "MFTIMER_FLAGS",
         "MFTInputStatusFlags" => "_MFT_INPUT_STATUS_FLAGS",
         "MFTInputStreamInfo" => "MFT_INPUT_STREAM_INFO",
         "MFTInputStreamInfoFlags" => "_MFT_INPUT_STREAM_INFO_FLAGS",
         "MFTMessageType" => "MFT_MESSAGE_TYPE",
         "MFTopologyType" => "MF_TOPOLOGY_TYPE",
         "MFTopoNodeAttributeUpdate" => "MFTOPONODE_ATTRIBUTE_UPDATE",
         "MFTopoStatus" => "MF_TOPOSTATUS",
         "MFTOutputDataBuffer" => "MFT_OUTPUT_DATA_BUFFER",
         "MFTOutputDataBufferFlags" => "_MFT_OUTPUT_DATA_BUFFER_FLAGS",
         "MFTOutputStatusFlags" => "_MFT_OUTPUT_STATUS_FLAGS",
         "MFTOutputStreamInfo" => "MFT_OUTPUT_STREAM_INFO",
         "MFTOutputStreamInfoFlags" => "_MFT_OUTPUT_STREAM_INFO_FLAGS",
         "MFTProcessOutputFlags" => "_MFT_PROCESS_OUTPUT_FLAGS",
         "MFTranscodeSinkInfo" => "MF_TRANSCODE_SINK_INFO",
         "MFTRegisterTypeInfo" => "MFT_REGISTER_TYPE_INFO",
         "MFTSetTypeFlags" => "_MFT_SET_TYPE_FLAGS",
         "MFURLTrustStatus" => "MF_URL_TRUST_STATUS",
         "MFVPMessageType" => "MFVP_MESSAGE_TYPE",
         "MT_CustomVideoPrimaries" => "MT_CUSTOM_VIDEO_PRIMARIES",
         "ProcessOutputStatus" => "_MFT_PROCESS_OUTPUT_STATUS",
         "WaveFormatExtensibleWithData" => "WAVEFORMATEX",
         "WaveFormatExWithData" => "WAVEFORMATEX"
      );

      $newname = $namemap[$symbols[0]];
      if ($newname !== null)
      {
         $use = $newname;
      }
      else
      {
         $use = $symbols[0];
      }
   }
   else
   {
      $use = $symbols[0];
   }
}

// Process the locale if specified
$loc = $_GET['locale'];

if ($loc == false)
{
   $loc = "EN-US";
}

// Redirect browser
$app = "dev12";
$url = "http://msdn.microsoft.com/query/" . $app . ".query?appId=" . $app . "IDEF1&l=" . $loc . "&k=k(" . $use . ")";

//print("testing: <a href=\"" . $url . "\">" . $url . "</a>"); // Useful for debugging
header("Location: " . $url);
?>