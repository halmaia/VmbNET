using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VmbNET
{
    [StructLayout(LayoutKind.Sequential, Size = Size)]
    [DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
    public unsafe readonly ref struct VmbFrame
    {
        public const int Size = 112;
        public static long TimeOffset { get; set; } = 0;

        public VmbFrame(uint bufferSize,
            byte* buffer = null,
            void* context0 = null, void* context1 = null, 
            void* context2 = null, void* context3 = null) : this()
        {
            Buffer = buffer;
            BufferSize = bufferSize;
            Context0 = context0;
            Context1 = context1;
            Context2 = context2;
            Context3 = context3;
        }

        //----- Init -----
        public readonly byte* Buffer { get; init; }                 //!< Comprises image and potentially chunk data
        public readonly uint BufferSize { get; init; }              //!< The size of the data buffer
        public readonly void* Context0 { get; init; }              //!< 4 void pointers that can be employed by the user (e.g. for storing handles)
        public readonly void* Context1 { get; init; }
        public readonly void* Context2 { get; init; }
        public readonly void* Context3 { get; init; }

        //----- Out -----
        public readonly VmbFrameStatus ReceiveStatus { get; }           //!< The resulting status of the receive operation
        public readonly ulong FrameID { get; }                 //!< Unique ID of this frame in this stream
        public readonly ulong Timestamp { get; }               //!< The timestamp set by the camera
        public readonly byte* ImageData { get; }               //!< The start of the image data, if present, or null
        public readonly VmbFrameFlags ReceiveFlags { get; }            //!< Flags indicating which additional frame information is available
        public readonly VmbPixelFormat PixelFormat { get; }             //!< Pixel format of the image
        public readonly uint Width { get; }                   //!< Width of an image
        public readonly uint Height { get; }                  //!< Height of an image
        public readonly uint OffsetX { get; }                 //!< Horizontal offset of an image
        public readonly uint OffsetY { get; }                 //!< Vertical offset of an image
        public readonly VmbPayloadType PayloadType { get; }             //!< The type of payload
        public readonly bool ChunkDataPresent { get; }        //!< True if the transport layer reported chunk data to be present in the buffer

        public readonly long OffsetTimestamp => (long)Timestamp + TimeOffset;

        public override readonly string ToString() =>
            FrameID + "; " + Width + '×' + Height + "; " + Timestamp + " ns";
    }
}
