using Cryptographer.Classes;
using Cryptographer.Decoders;

namespace Cryptographer
{
    class DecoderFactory
    {
        private static readonly List<IDecoder> decoderList = new()
        {
            new Base64(), new Morse(), new Baconian(), new Binary(), new TapCode(),
            new DNA(), new Hexadecimal(), new Base32(), new Base85(), new Base62(),
            new Octal(), new Baudot(), new Trilateral(), new ROT47(), new uuencoding(),
            new A1Z26(), new ASCII(), new Brainfuck(), new Base58(), new Base45(),
            new Caesar(), new KeyboardSubstitution(), new Atbash(), new Reverse(), new ASCIIShift(),
            new Scytale(), new Vigenere(), new Beaufort(), new Polybius(), new TapCode(),
            new Playfair()
        };
        public static List<IDecoder> GetAll()
        {
            return decoderList;
        }

    }
}