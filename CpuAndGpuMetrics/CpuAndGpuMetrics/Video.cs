using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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

        public Codec CodecExt 
        {
            get { return codec; }
            set {  codec = value; }
        }

        public Chroma ChromaExt 
        {
            get { return chroma; } 
            set {  chroma = value; } 
        }

        public Resolution ResolutionExt 
        { 
            get { return resolution; } 
            set {  resolution = value; } 
        }

        public BitDepth BitDepthExt 
        { 
            get { return bitDepth; }
            set {  bitDepth = value; }
        }



        public Video(Codec codec, Chroma chroma, Resolution resolution, BitDepth bitDepth)
        {
            this.codec = codec;
            this.chroma = chroma;
            this.resolution = resolution;
            this.bitDepth = bitDepth;
        }

        public static Video FilenameToVideo(string filename)
        {
            Codec codec;
            Chroma chroma;
            Resolution resolution;
            BitDepth bitDepth;

            if (filename.Contains("H264") || filename.Contains("h264") || filename.Contains("libx264") || filename.Contains("x264") )
            {
                codec = Codec.H264;
            }
            else if (filename.Contains("H265") || filename.Contains("h265") || filename.Contains("hevc") || filename.Contains("x265"))
            { 
                codec= Codec.H265;
            }
            else
            {
                codec = Codec.Unknown;
            }

            if (filename.Contains("420"))
            {
                chroma = Chroma.Subsampling_420;
            }
            else if (filename.Contains("444"))
            {
                chroma = Chroma.Subsampling_444;
            }
            else
            {
                 chroma = Chroma.Unknown;
            }

            if (filename.Contains("UHD") || filename.Contains("4k") || filename.Contains("4K"))
            {
                resolution = Resolution.UHD;
            }
            else if (filename.Contains("HD") || filename.Contains("hd"))
            {
                resolution = Resolution.HD;
            }
            else 
            { 
                resolution = Resolution.Unknown; 
            }

            if (filename.Contains("8bit") || filename.Contains("b08"))
            {
                bitDepth = BitDepth.Bit_8;
            }
            else if (filename.Contains("10bit") || filename.Contains("b10"))
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
            UHD = 1,
            Unknown = 2,
        }
    }
}
