using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VmbNET
{
    [SkipLocalsInit, StructLayout(LayoutKind.Sequential, Size = Size)]
    [DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
    public unsafe struct VmbFrame
    {
        // TODO: Convert to normal C# struct:
        public const int Size = 112;
        //----- In -----
        public byte* buffer;                 //!< Comprises image and potentially chunk data
        public uint bufferSize;             //!< The size of the data buffer
        public void* context0;             //!< 4 void pointers that can be employed by the user (e.g. for storing handles)
        public void* context1;
        public void* context2;
        public void* context3;
        //----- Out -----
        public VmbFrameStatus receiveStatus;          //!< The resulting status of the receive operation
        public ulong frameID;                //!< Unique ID of this frame in this stream
        public ulong timestamp;              //!< The timestamp set by the camera
        public byte* imageData;              //!< The start of the image data, if present, or null
        VmbFrameFlags receiveFlags;           //!< Flags indicating which additional frame information is available
        public VmbPixelFormat pixelFormat;            //!< Pixel format of the image
        public uint width;                  //!< Width of an image
        public uint height;                 //!< Height of an image
        public uint offsetX;                //!< Horizontal offset of an image
        public uint offsetY;                //!< Vertical offset of an image
        VmbPayloadType payloadType;            //!< The type of payload
        public bool chunkDataPresent;       //!< True if the transport layer reported chunk data to be present in the buffer

        //public override readonly string ToString() =>
        //    FrameID + "; " + Width + "×" + Height + "; " + Timestamp + " ns";
    }
}
