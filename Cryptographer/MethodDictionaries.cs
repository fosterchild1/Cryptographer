using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptographer
{
    internal class MethodDictionaries
    {
        public static Dictionary<string, string> Morse = new()
        {
            ["/"] = " ",
            ["-----"] = "0",
            [".----"] = "1",
            ["..---"] = "2",
            ["...--"] = "3",
            ["....-"] = "4",
            ["....."] = "5",
            ["-...."] = "6",
            ["--..."] = "7",
            ["---.."] = "8",
            ["----."] = "9",
            [".-"] = "a",
            ["-..."] = "b",
            ["-.-."] = "c",
            ["-.."] = "d",
            ["."] = "e",
            ["..-."] = "f",
            ["--."] = "g",
            ["...."] = "h",
            [".."] = "i",
            [".---"] = "j",
            ["-.-"] = "k",
            [".-.."] = "l",
            ["--"] = "m",
            ["-."] = "n",
            ["---"] = "o",
            [".--."] = "p",
            ["--.-"] = "q",
            [".-."] = "r",
            ["..."] = "s",
            ["-"] = "t",
            ["..-"] = "u",
            ["...-"] = "v",
            [".--"] = "w",
            ["-..-"] = "x",
            ["-.--"] = "y",
            ["--.."] = "z",
            [".-.-.-"] = ".",
            ["--..--"] = ",",
            ["..--.."] = "?",
            ["-.-.--"] = "!",
            ["-....-"] = "-",
            ["-..-."] = "/",
            [".--.-."] = "@",
            ["-.--."] = "(",
            ["-.--.-"] = ")",
        };

        public static Dictionary<string, string> Baconian26 = new()
        {
            ["AAAAA"] = "A",
            ["AAAAB"] = "B",
            ["AAABA"] = "C",
            ["AAABB"] = "D",
            ["AABAA"] = "E",
            ["AABAB"] = "F",
            ["AABBA"] = "G",
            ["AABBB"] = "H",
            ["ABAAA"] = "I",
            ["ABAAB"] = "J",
            ["ABABA"] = "K",
            ["ABABB"] = "L",
            ["ABBAA"] = "M",
            ["ABBAB"] = "N",
            ["ABBBA"] = "O",
            ["ABBBB"] = "P",
            ["BAAAA"] = "Q",
            ["BAAAB"] = "R",
            ["BAABA"] = "S",
            ["BAABB"] = "T",
            ["BABAA"] = "U",
            ["BABAB"] = "V",
            ["BABBA"] = "W",
            ["BABBB"] = "X",
            ["BBAAA"] = "Y",
            ["BBAAB"] = "Z"
        };

        public static Dictionary<string, string> Baconian24 = new()
        {
            ["AAAAA"] = "A",
            ["AAAAB"] = "B",
            ["AAABA"] = "C",
            ["AAABB"] = "D",
            ["AABAA"] = "E",
            ["AABAB"] = "F",
            ["AABBA"] = "G",
            ["AABBB"] = "H",
            ["ABAAA"] = "I",
            ["ABAAA"] = "J", // same as I
            ["ABAAB"] = "K",
            ["ABABA"] = "L",
            ["ABABB"] = "M",
            ["ABBAA"] = "N",
            ["ABBAB"] = "O",
            ["ABBBA"] = "P",
            ["ABBBB"] = "Q",
            ["BAAAA"] = "R",
            ["BAAAB"] = "S",
            ["BAABA"] = "T",
            ["BAABB"] = "U",
            ["BAABB"] = "V", // same as U
            ["BABAA"] = "W",
            ["BABAB"] = "X",
            ["BABBA"] = "Y",
            ["BABBB"] = "Z"
        };
    }
}
