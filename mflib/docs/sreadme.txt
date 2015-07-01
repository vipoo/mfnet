Media Foundation Lib Samples 2015-07-01

http://mflib.SourceForge.net

These samples should not be considered commercial quality applications.  They are just 
intended to illustrate how to use some particular feature, or group of features in 
Media Foundation.  Feel free to polish them into applications of your own, or extract 
sections to use in your own code.  Each sample has (at least one) readme file.  
If you are looking for info regarding a sample, these are always a good place to start.

Also, while the Media Foundation library is licensed under LGPL or BSD, these samples are 
public domain.  Use them as you like.  Every one of these samples needs the MediaFoundation
library which is not included in this package.  Get the latest version of the library from 
the SourceForge website.

These samples have been updated to work with the v3.0 library.  As of 2015-04-14, they
have been updated to work with framework version 4.0.3.

The people who wrote these samples usually hang out in 
http://sourceforge.net/forum/forum.php?forum_id=711229.  If you have questions, 
comments, or just want to say thanks to the people who have taken the time to
create these, feel free to stop by.

Also, if you have samples you think would be useful (or would like to write some), 
that forum would be the place to get started.

=====================================================================================

This is the current list of samples along with a short description.  See the
readme.txt in the individual samples for more details.


Samples\BasicPlayer
--------------------
A c# implementation of the BasicPlayer sample that ships with the Vista PSDK.  It allows
you to use MF to play various media files.


Samples\WavSource
--------------------
A c# implementation of the WavSource sample that ships with the Vista PSDK.  It extends
Mediafoundation to include support for reading .wav files.


Samples\MFT
--------------------
A collection of Media Foundation Transforms (MFTs) written in c#, along with templates
and documention on how to create your own.


Samples\MFT_Grayscale
--------------------
A c# implementation of the MFT_Grayscale sample that ships with the Vista PSDK.  It allows
you to modify data as it passes down the topology.


Samples\PlaybackFX
--------------------
A c# implementation of the PlaybackFX sample that ships with the Vista PSDK.  It is the
same as the BasicPlayer sample, except that it loads the Grayscale MFT into the topology.


Samples\Playlist
--------------------
A c# implementation of the PlaybackFX sample that ships with the Vista PSDK.  It plays a
collection of media files, one after the other.


Samples\ProtectedPlayback
--------------------
A c# implementation of the ProtectedPlayback sample that ships with the Vista PSDK.  It is the
same as the BasicPlayer sample, except that it allows for playing protected content.


Samples\Splitter
--------------------
A c# implementation of the code on http://msdn2.microsoft.com/en-us/library/bb530124.aspx.  It
shows how to parse/process data from WM files.


Samples\WavSink
--------------------
A c# implementation of the WavSink sample that ships with the Vista PSDK.  Is the opposite
of the WavSource sample: It writes audio output to a .wav file.


Samples\EVRPresenter
--------------------
A c# implementation of the EVRPresenter sample that ships with the Vista PSDK.  This code replaces
the default renderer of the Enhanced Video Renderer (EVR).  To have any hope of understanding what
this code does and how it works, read the readmes included with the project.


Samples\Hack
--------------------
This c++ project allows c# code to work around flaws in c++ COM objects that don't correctly implement 
the IUnknown interface.  It is used by the EVRPresenter project, but is a general-purpose COM object
that can be used to work around similar problems in other COM objects.


Samples\MFCaptureToFile
--------------------
This sample is a c# version of the C++ MFCaptureToFile sample included in the
Media Foundation SDK.  It allows you to select a capture device, and specify
the file name to which to capture.


Samples\MFCaptureD3D
--------------------
MFCaptureD3D is an example to demonstrates how to preview video from a capture device, using 
Direct3D to draw the video frames.


Samples\MFCaptureAlt
--------------------
MFCaptureAlt is a combination of MFCaptureD3D and MFCaptureToFile.  It previews the output from the
capture device, and writes it to disk.

It also shows how to modify the capture buffer to add a watermark.


