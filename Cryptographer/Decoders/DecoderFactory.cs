using Cryptographer.Classes;
using Cryptographer.Decoders;

namespace Cryptographer
{
    static class DecoderFactory
    {
        private static readonly List<IDecoder> decoderList = new()
        {
            new Base64(), new Morse(), new Baconian(), new Binary(), new TapCode(),
            new DNA(), new Hexadecimal(), new Base32(), new Base85(), new Base62(),
            new Octal(), new Baudot(), new Trilateral(), new ROT47(), new uuencoding(),
            new A1Z26(), new ASCII(), new Brainfuck(), new Base58(), new Base45(),
            new Caesar(), new KeyboardSubstitution(), new Atbash(), new Reverse(), new ASCIIShift(),
            new Scytale(), new Vigenere(), new Beaufort(), new Polybius(), new Playfair()
        };

        private static readonly Dictionary<IDecoder, byte> decoderToId = new();
        private static readonly IDecoder[] IdToDecoder = new IDecoder[decoderList.Count + 1];

        private static readonly List<IDecoder> genericList = new();
        private static readonly List<IDecoder> keyedList = new();

        public static List<IDecoder> GetAll()
        {
            return decoderList;
        }

        public static List<IDecoder> GetKeyed()
        {
            return keyedList;
        }

        public static List<IDecoder> GetGeneric()
        {
            return genericList;
        }

        public static IDecoder FromId(byte id)
        {
            return IdToDecoder[id];
        }

        public static byte ToId(IDecoder decoder)
        {
            return decoderToId[decoder];
        }


        static DecoderFactory()
        {
            // id 0 = none
            IDecoder none = new None();
            IdToDecoder[0] = none;
            decoderToId[none] = 0;

            byte id = 1;
            foreach (IDecoder decoder in decoderList)
            {
                IdToDecoder[id] = decoder;
                decoderToId[decoder] = id++;

                if (decoder.RequiredKey == KeyLevel.Keyed)
                {
                    keyedList.Add(decoder);
                    continue;
                }

                genericList.Add(decoder);
            }
        }
    }
}