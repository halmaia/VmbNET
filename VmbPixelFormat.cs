using static VmbNET.VmbPixelType;
using static VmbNET.VmbPixelOccupyType;
namespace VmbNET;

public enum VmbPixelType : uint
{
    VmbPixelMono = 0x01000000,     //!< Monochrome pixel
    VmbPixelColor = 0x02000000      //!< Pixel bearing color information
}

public enum VmbPixelOccupyType : uint
{
    VmbPixelOccupy8Bit = 0x00080000,     //!< Pixel effectively occupies 8 bits
    VmbPixelOccupy10Bit = 0x000A0000,     //!< Pixel effectively occupies 10 bits
    VmbPixelOccupy12Bit = 0x000C0000,     //!< Pixel effectively occupies 12 bits
    VmbPixelOccupy14Bit = 0x000E0000,     //!< Pixel effectively occupies 14 bits
    VmbPixelOccupy16Bit = 0x00100000,     //!< Pixel effectively occupies 16 bits
    VmbPixelOccupy24Bit = 0x00180000,     //!< Pixel effectively occupies 24 bits
    VmbPixelOccupy32Bit = 0x00200000,     //!< Pixel effectively occupies 32 bits
    VmbPixelOccupy48Bit = 0x00300000,     //!< Pixel effectively occupies 48 bits
    VmbPixelOccupy64Bit = 0x00400000       //!< Pixel effectively occupies 64 bits}
}
public enum VmbPixelFormat : uint
{

    // mono formats
    VmbPixelFormatMono8 = VmbPixelMono | VmbPixelOccupy8Bit | 0x0001,  //!< Monochrome, 8 bits (PFNC:  Mono8)
    VmbPixelFormatMono10 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0003,  //!< Monochrome, 10 bits in 16 bits (PFNC:  Mono10)
    VmbPixelFormatMono10p = VmbPixelMono | VmbPixelOccupy10Bit | 0x0046,  //!< Monochrome, 10 bits in 16 bits (PFNC:  Mono10p)
    VmbPixelFormatMono12 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0005,  //!< Monochrome, 12 bits in 16 bits (PFNC:  Mono12)
    VmbPixelFormatMono12Packed = VmbPixelMono | VmbPixelOccupy12Bit | 0x0006,  //!< Monochrome, 2x12 bits in 24 bits (GEV:Mono12Packed)
    VmbPixelFormatMono12p = VmbPixelMono | VmbPixelOccupy12Bit | 0x0047,  //!< Monochrome, 2x12 bits in 24 bits (PFNC:  MonoPacked)
    VmbPixelFormatMono14 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0025,  //!< Monochrome, 14 bits in 16 bits (PFNC:  Mono14)
    VmbPixelFormatMono16 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0007,  //!< Monochrome, 16 bits (PFNC:  Mono16)

    // bayer formats
    VmbPixelFormatBayerGR8 = VmbPixelMono | VmbPixelOccupy8Bit | 0x0008,  //!< Bayer-color, 8 bits, starting with GR line (PFNC:  BayerGR8)
    VmbPixelFormatBayerRG8 = VmbPixelMono | VmbPixelOccupy8Bit | 0x0009,  //!< Bayer-color, 8 bits, starting with RG line (PFNC:  BayerRG8)
    VmbPixelFormatBayerGB8 = VmbPixelMono | VmbPixelOccupy8Bit | 0x000A,  //!< Bayer-color, 8 bits, starting with GB line (PFNC:  BayerGB8)
    VmbPixelFormatBayerBG8 = VmbPixelMono | VmbPixelOccupy8Bit | 0x000B,  //!< Bayer-color, 8 bits, starting with BG line (PFNC:  BayerBG8)
    VmbPixelFormatBayerGR10 = VmbPixelMono | VmbPixelOccupy16Bit | 0x000C,  //!< Bayer-color, 10 bits in 16 bits, starting with GR line (PFNC:  BayerGR10)
    VmbPixelFormatBayerRG10 = VmbPixelMono | VmbPixelOccupy16Bit | 0x000D,  //!< Bayer-color, 10 bits in 16 bits, starting with RG line (PFNC:  BayerRG10)
    VmbPixelFormatBayerGB10 = VmbPixelMono | VmbPixelOccupy16Bit | 0x000E,  //!< Bayer-color, 10 bits in 16 bits, starting with GB line (PFNC:  BayerGB10)
    VmbPixelFormatBayerBG10 = VmbPixelMono | VmbPixelOccupy16Bit | 0x000F,  //!< Bayer-color, 10 bits in 16 bits, starting with BG line (PFNC:  BayerBG10)
    VmbPixelFormatBayerGR12 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0010,  //!< Bayer-color, 12 bits in 16 bits, starting with GR line (PFNC:  BayerGR12)
    VmbPixelFormatBayerRG12 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0011,  //!< Bayer-color, 12 bits in 16 bits, starting with RG line (PFNC:  BayerRG12)
    VmbPixelFormatBayerGB12 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0012,  //!< Bayer-color, 12 bits in 16 bits, starting with GB line (PFNC:  BayerGB12)
    VmbPixelFormatBayerBG12 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0013,  //!< Bayer-color, 12 bits in 16 bits, starting with BG line (PFNC:  BayerBG12)
    VmbPixelFormatBayerGR12Packed = VmbPixelMono | VmbPixelOccupy12Bit | 0x002A,  //!< Bayer-color, 2x12 bits in 24 bits, starting with GR line (GEV:BayerGR12Packed)
    VmbPixelFormatBayerRG12Packed = VmbPixelMono | VmbPixelOccupy12Bit | 0x002B,  //!< Bayer-color, 2x12 bits in 24 bits, starting with RG line (GEV:BayerRG12Packed)
    VmbPixelFormatBayerGB12Packed = VmbPixelMono | VmbPixelOccupy12Bit | 0x002C,  //!< Bayer-color, 2x12 bits in 24 bits, starting with GB line (GEV:BayerGB12Packed)
    VmbPixelFormatBayerBG12Packed = VmbPixelMono | VmbPixelOccupy12Bit | 0x002D,  //!< Bayer-color, 2x12 bits in 24 bits, starting with BG line (GEV:BayerBG12Packed)
    VmbPixelFormatBayerGR10p = VmbPixelMono | VmbPixelOccupy10Bit | 0x0056,  //!< Bayer-color, 10 bits continuous packed, starting with GR line (PFNC:  BayerGR10p)
    VmbPixelFormatBayerRG10p = VmbPixelMono | VmbPixelOccupy10Bit | 0x0058,  //!< Bayer-color, 10 bits continuous packed, starting with RG line (PFNC:  BayerRG10p)
    VmbPixelFormatBayerGB10p = VmbPixelMono | VmbPixelOccupy10Bit | 0x0054,  //!< Bayer-color, 10 bits continuous packed, starting with GB line (PFNC:  BayerGB10p)
    VmbPixelFormatBayerBG10p = VmbPixelMono | VmbPixelOccupy10Bit | 0x0052,  //!< Bayer-color, 10 bits continuous packed, starting with BG line (PFNC:  BayerBG10p)
    VmbPixelFormatBayerGR12p = VmbPixelMono | VmbPixelOccupy12Bit | 0x0057,  //!< Bayer-color, 12 bits continuous packed, starting with GR line (PFNC:  BayerGR12p)
    VmbPixelFormatBayerRG12p = VmbPixelMono | VmbPixelOccupy12Bit | 0x0059,  //!< Bayer-color, 12 bits continuous packed, starting with RG line (PFNC:  BayerRG12p)
    VmbPixelFormatBayerGB12p = VmbPixelMono | VmbPixelOccupy12Bit | 0x0055,  //!< Bayer-color, 12 bits continuous packed, starting with GB line (PFNC:  BayerGB12p)
    VmbPixelFormatBayerBG12p = VmbPixelMono | VmbPixelOccupy12Bit | 0x0053,  //!< Bayer-color, 12 bits continuous packed, starting with BG line (PFNC: BayerBG12p)
    VmbPixelFormatBayerGR16 = VmbPixelMono | VmbPixelOccupy16Bit | 0x002E,  //!< Bayer-color, 16 bits, starting with GR line (PFNC: BayerGR16)
    VmbPixelFormatBayerRG16 = VmbPixelMono | VmbPixelOccupy16Bit | 0x002F,  //!< Bayer-color, 16 bits, starting with RG line (PFNC: BayerRG16)
    VmbPixelFormatBayerGB16 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0030,  //!< Bayer-color, 16 bits, starting with GB line (PFNC: BayerGB16)
    VmbPixelFormatBayerBG16 = VmbPixelMono | VmbPixelOccupy16Bit | 0x0031,  //!< Bayer-color, 16 bits, starting with BG line (PFNC: BayerBG16)

    // rgb formats
    VmbPixelFormatRgb8 = VmbPixelColor | VmbPixelOccupy24Bit | 0x0014,  //!< RGB, 8 bits x 3 (PFNC: RGB8)
    VmbPixelFormatBgr8 = VmbPixelColor | VmbPixelOccupy24Bit | 0x0015,  //!< BGR, 8 bits x 3 (PFNC: BGR8)
    VmbPixelFormatRgb10 = VmbPixelColor | VmbPixelOccupy48Bit | 0x0018,  //!< RGB, 12 bits in 16 bits x 3 (PFNC: RGB12)
    VmbPixelFormatBgr10 = VmbPixelColor | VmbPixelOccupy48Bit | 0x0019,  //!< RGB, 12 bits in 16 bits x 3 (PFNC: RGB12)
    VmbPixelFormatRgb12 = VmbPixelColor | VmbPixelOccupy48Bit | 0x001A,  //!< RGB, 12 bits in 16 bits x 3 (PFNC: RGB12)
    VmbPixelFormatBgr12 = VmbPixelColor | VmbPixelOccupy48Bit | 0x001B,  //!< RGB, 12 bits in 16 bits x 3 (PFNC: RGB12)
    VmbPixelFormatRgb14 = VmbPixelColor | VmbPixelOccupy48Bit | 0x005E,  //!< RGB, 14 bits in 16 bits x 3 (PFNC: RGB12)
    VmbPixelFormatBgr14 = VmbPixelColor | VmbPixelOccupy48Bit | 0x004A,  //!< RGB, 14 bits in 16 bits x 3 (PFNC: RGB12)
    VmbPixelFormatRgb16 = VmbPixelColor | VmbPixelOccupy48Bit | 0x0033,  //!< RGB, 16 bits x 3 (PFNC: RGB16)
    VmbPixelFormatBgr16 = VmbPixelColor | VmbPixelOccupy48Bit | 0x004B,  //!< RGB, 16 bits x 3 (PFNC: RGB16)

    // rgba formats
    VmbPixelFormatArgb8 = VmbPixelColor | VmbPixelOccupy32Bit | 0x0016,  //!< ARGB, 8 bits x 4 (PFNC: RGBa8)
    VmbPixelFormatRgba8 = VmbPixelFormatArgb8,                           //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatBgra8 = VmbPixelColor | VmbPixelOccupy32Bit | 0x0017,  //!< BGRA, 8 bits x 4 (PFNC: BGRa8)
    VmbPixelFormatRgba10 = VmbPixelColor | VmbPixelOccupy64Bit | 0x005F,  //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatBgra10 = VmbPixelColor | VmbPixelOccupy64Bit | 0x004C,  //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatRgba12 = VmbPixelColor | VmbPixelOccupy64Bit | 0x0061,  //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatBgra12 = VmbPixelColor | VmbPixelOccupy64Bit | 0x004E,  //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatRgba14 = VmbPixelColor | VmbPixelOccupy64Bit | 0x0063,  //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatBgra14 = VmbPixelColor | VmbPixelOccupy64Bit | 0x0050,  //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatRgba16 = VmbPixelColor | VmbPixelOccupy64Bit | 0x0064,  //!< RGBA, 8 bits x 4, legacy name
    VmbPixelFormatBgra16 = VmbPixelColor | VmbPixelOccupy64Bit | 0x0051,  //!< RGBA, 8 bits x 4, legacy name

    // yuv/ycbcr formats
    VmbPixelFormatYuv411 = VmbPixelColor | VmbPixelOccupy12Bit | 0x001E,  //!< YUV 4:1:1 with 8 bits (PFNC: YUV411_8_UYYVYY, GEV:YUV411Packed)
    VmbPixelFormatYuv422 = VmbPixelColor | VmbPixelOccupy16Bit | 0x001F,  //!< YUV 4:2:2 with 8 bits (PFNC: YUV422_8_UYVY, GEV:YUV422Packed)
    VmbPixelFormatYuv444 = VmbPixelColor | VmbPixelOccupy24Bit | 0x0020,  //!< YUV 4:4:4 with 8 bits (PFNC: YUV8_UYV, GEV:YUV444Packed)
    VmbPixelFormatYuv422_8 = VmbPixelColor | VmbPixelOccupy16Bit | 0x0032,  //!< YUV 4:2:2 with 8 bits Channel order YUYV (PFNC: YUV422_8)
    VmbPixelFormatYCbCr8_CbYCr = VmbPixelColor | VmbPixelOccupy24Bit | 0x003A,  //!< YCbCr 4:4:4 with 8 bits (PFNC: YCbCr8_CbYCr) - identical to VmbPixelFormatYuv444
    VmbPixelFormatYCbCr422_8 = VmbPixelColor | VmbPixelOccupy16Bit | 0x003B,  //!< YCbCr 4:2:2 8-bit YCbYCr (PFNC: YCbCr422_8)
    VmbPixelFormatYCbCr411_8_CbYYCrYY = VmbPixelColor | VmbPixelOccupy12Bit | 0x003C,  //!< YCbCr 4:1:1 with 8 bits (PFNC: YCbCr411_8_CbYYCrYY) - identical to VmbPixelFormatYuv411
    VmbPixelFormatYCbCr601_8_CbYCr = VmbPixelColor | VmbPixelOccupy24Bit | 0x003D,  //!< YCbCr601 4:4:4 8-bit CbYCrt (PFNC: YCbCr601_8_CbYCr)
    VmbPixelFormatYCbCr601_422_8 = VmbPixelColor | VmbPixelOccupy16Bit | 0x003E,  //!< YCbCr601 4:2:2 8-bit YCbYCr (PFNC: YCbCr601_422_8)
    VmbPixelFormatYCbCr601_411_8_CbYYCrYY = VmbPixelColor | VmbPixelOccupy12Bit | 0x003F,  //!< YCbCr601 4:1:1 8-bit CbYYCrYY (PFNC: YCbCr601_411_8_CbYYCrYY)
    VmbPixelFormatYCbCr709_8_CbYCr = VmbPixelColor | VmbPixelOccupy24Bit | 0x0040,  //!< YCbCr709 4:4:4 8-bit CbYCr (PFNC: YCbCr709_8_CbYCr)
    VmbPixelFormatYCbCr709_422_8 = VmbPixelColor | VmbPixelOccupy16Bit | 0x0041,  //!< YCbCr709 4:2:2 8-bit YCbYCr (PFNC: YCbCr709_422_8)
    VmbPixelFormatYCbCr709_411_8_CbYYCrYY = VmbPixelColor | VmbPixelOccupy12Bit | 0x0042,  //!< YCbCr709 4:1:1 8-bit CbYYCrYY (PFNC: YCbCr709_411_8_CbYYCrYY)
    VmbPixelFormatYCbCr422_8_CbYCrY = VmbPixelColor | VmbPixelOccupy16Bit | 0x0043,  //!< YCbCr 4:2:2 with 8 bits (PFNC: YCbCr422_8_CbYCrY) - identical to VmbPixelFormatYuv422
    VmbPixelFormatYCbCr601_422_8_CbYCrY = VmbPixelColor | VmbPixelOccupy16Bit | 0x0044,  //!< YCbCr601 4:2:2 8-bit CbYCrY (PFNC: YCbCr601_422_8_CbYCrY)
    VmbPixelFormatYCbCr709_422_8_CbYCrY = VmbPixelColor | VmbPixelOccupy16Bit | 0x0045,  //!< YCbCr709 4:2:2 8-bit CbYCrY (PFNC: YCbCr709_422_8_CbYCrY)
    VmbPixelFormatYCbCr411_8 = VmbPixelColor | VmbPixelOccupy12Bit | 0x005A,  //!< YCbCr 4:1:1 8-bit YYCbYYCr (PFNC: YCbCr411_8)
    VmbPixelFormatYCbCr8 = VmbPixelColor | VmbPixelOccupy24Bit | 0x005B,  //!< YCbCr 4:4:4 8-bit YCbCr (PFNC: YCbCr8)
}