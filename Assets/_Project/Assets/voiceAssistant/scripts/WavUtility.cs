using UnityEngine;
using System;
using System.IO;

public static class WavUtility
{
    // Convert an AudioClip to a WAV byte array
    public static byte[] FromAudioClip(AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] wav;
        using (var stream = new MemoryStream())
        {
            // RIFF header
            WriteString(stream, "RIFF");
            WriteInt(stream, 0); // placeholder for file size
            WriteString(stream, "WAVE");
            // fmt subchunk
            WriteString(stream, "fmt ");
            WriteInt(stream, 16);
            WriteShort(stream, 1);
            WriteShort(stream, (short)clip.channels);
            WriteInt(stream, clip.frequency);
            WriteInt(stream, clip.frequency * clip.channels * 2);
            WriteShort(stream, (short)(clip.channels * 2));
            WriteShort(stream, 16);
            // data subchunk
            WriteString(stream, "data");
            WriteInt(stream, samples.Length * 2);

            // write samples
            foreach (var f in samples)
            {
                short intData = (short)(f * short.MaxValue);
                var bytes = BitConverter.GetBytes(intData);
                stream.Write(bytes, 0, 2);
            }

            // go back and write file length
            stream.Seek(4, SeekOrigin.Begin);
            WriteInt(stream, (int)stream.Length - 8);

            wav = stream.ToArray();
        }

        return wav;
    }

    // Convert a WAV byte array to an AudioClip
    public static AudioClip ToAudioClip(byte[] wav, int offsetSamples = 0, string name = "wav")
    {
        // get header info
        int channels = BitConverter.ToInt16(wav, 22);
        int sampleRate = BitConverter.ToInt32(wav, 24);
        int pos = 12;
        while (!(wav[pos] == 'd' && wav[pos+1] == 'a' && wav[pos+2] == 't' && wav[pos+3] == 'a'))
            pos++;
        pos += 8;
        int dataSize = BitConverter.ToInt32(wav, pos - 4);

        int sampleCount = dataSize / 2; // 16 bit
        var samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            short s = BitConverter.ToInt16(wav, pos + 2 * i);
            samples[i] = s / (float)short.MaxValue;
        }

        var clip = AudioClip.Create(name, sampleCount / channels, channels, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private static void WriteString(Stream s, string str)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(str);
        s.Write(bytes, 0, bytes.Length);
    }

    private static void WriteInt(Stream s, int value)
    {
        var bytes = BitConverter.GetBytes(value);
        s.Write(bytes, 0, 4);
    }

    private static void WriteShort(Stream s, short value)
    {
        var bytes = BitConverter.GetBytes(value);
        s.Write(bytes, 0, 2);
    }
}
