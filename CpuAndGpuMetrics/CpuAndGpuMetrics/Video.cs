using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CpuAndGpuMetrics
{
    public class Video
    {
        private Codec codec;

        private Chroma chroma;

        private Resolution resolution;

        private BitDepth bitDepth;

        public Video(Codec codec, Chroma chroma, Resolution resolution, BitDepth bitDepth)
        {
            this.codec = codec;
            this.chroma = chroma;
            this.resolution = resolution;
            this.bitDepth = bitDepth;
        }

        public static Video FilenameToVideo(string filename)
        {
            string[] videoDetails = filename.Split('_');
            
            if (videoDetails.Length != 4)
            {
                throw new ArgumentException("Filename format is incorrect. Format: codec_chroma_resolution_bitcolor");
            }

            Codec codec;
            Chroma chroma;
            Resolution resolution;
            BitDepth bitDepth;

            if (videoDetails[0] == "H264")
            {
                codec = Codec.H264;
            } 
            else if (videoDetails[0] == "H265")
            {
                codec = Codec.H265;
            }
            else
            {
                codec = Codec.Unknown;
            }

            if (videoDetails[1] == "420")
            {
                chroma = Chroma.Subsampling_420;
            }
            else if (videoDetails[1] == "444")
            {
                chroma = Chroma.Subsampling_444;
            }
            else
            {
                chroma = Chroma.Unknown;
            }

            if (videoDetails[2] == "4K")
            {
                resolution = Resolution.K4;
            } 
            else if (videoDetails[2] == "HD")
            {
                resolution = Resolution.HD;
            }
            else
            {
                resolution = Resolution.Unknown;
            }

            if (videoDetails[3] == "8bit")
            {
                bitDepth = BitDepth.Bit_8;
            } 
            else if (videoDetails[3] == "10bit")
            {
                bitDepth = BitDepth.Bit_10;
            }
            else
            {
                bitDepth = BitDepth.Unknown;
            }

            return new Video(codec, chroma, resolution, bitDepth);
        }

        public enum Codec
        {
            H264 = 0,
            H265 = 1,
            Unknown = 2,
        }

        public enum Chroma
        {
            Subsampling_420 = 0,
            Subsampling_444 = 1,
            Unknown = 2,
        }

        public enum BitDepth
        {
            Bit_8 = 0,
            Bit_10 = 1,
            Unknown = 2,
        }

        public enum Resolution
        {
            HD = 0,
            K4 = 1,
            Unknown = 2,
        }
    }
}
