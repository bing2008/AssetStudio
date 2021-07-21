using System;
using System.IO;

namespace AssetStudio.HY
{
    public class Decryption
    {
        protected const int headLength = 512;

        protected const int key = 1637968;

        protected static void WriteByte(byte[] bs, string path, int offset = 0, int count = 0)
        {
            BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(path));
            if (count == 0)
            {
                count = bs.Length;
            }
            binaryWriter.Seek(offset, SeekOrigin.Begin);
            binaryWriter.Write(bs, 0, count);
            binaryWriter.Close();
        }

        protected static int GetOffset(byte[] header)
        {
            return BitConverter.ToInt32(header, 512);
        }

        protected static void ResolveByte(byte[] key, byte[] source, int len = 0)
        {
            if (source.Length == 0)
            {
                throw new Exception("Invalid args.");
            }
            if (len <= 0)
            {
                len = source.Length;
            }
            int num = len / key.Length;
            int val = len % key.Length;
            for (int i = 0; i < num; i++)
            {
                int num2 = i * key.Length;
                for (int j = 0; j < key.Length; j++)
                {
                    source[num2 + j] ^= key[j];
                }
            }
            int num3 = num * key.Length;
            val = Math.Min(val, key.Length);
            for (int k = 0; k < val; k++)
            {
                source[num3 + k] ^= key[k];
            }
        }

        protected static ulong Resolve(string path, int resolveKey)
        {
            BinaryReader binaryReader = new BinaryReader(File.OpenRead(path));
            byte[] array = binaryReader.ReadBytes(516);
            binaryReader.Close();
            ResolveByte(BitConverter.GetBytes(1637968), array, 0);
            int num = BitConverter.ToInt32(array, 512);
            ResolveByte(BitConverter.GetBytes(resolveKey), array, 512);
            WriteByte(array, path, num + 512 + 4, 512);
            return (ulong)((long)num + 512L + 4);
        }

        protected static ulong Resolve(BinaryReader binaryReader, int resolveKey)
        {
            byte[] array = binaryReader.ReadBytes(516);
            //binaryReader.Close();
            ResolveByte(BitConverter.GetBytes(1637968), array, 0);
            int num = BitConverter.ToInt32(array, 512);
            ResolveByte(BitConverter.GetBytes(resolveKey), array, 512);
            //WriteByte(array, path, num + 512 + 4, 512);//do what£¿
            return (ulong)((long)num + 512L + 4);
        }

        public static ulong Decrypt(string path)
        {
            return Resolve(path, 0);
        }

        public static ulong Decrypt(BinaryReader binaryReader)
        {
            return Resolve(binaryReader, 0);
        }

        public static void Recover(string path)
        {
            Resolve(path, 11465776);
        }
    }
}