namespace Cryptographer.Utils
{
    internal class MethodDictionaries
    {
        // Morse Code
        public static Dictionary<string, string> Morse = new()
        {
            ["/"] = " ", [ "-----"] = "0", [".----"] = "1", ["..---"] = "2", ["...--"] = "3",
            ["....-"] = "4", ["....."] = "5", ["-...."] = "6", ["--..."] = "7", ["---.."] = "8",
            ["----."] = "9", [".-"] = "a", ["-..."] = "b", ["-.-."] = "c", ["-.."] = "d",
            ["."] = "e", ["..-."] = "f", ["--."] = "g", ["...."] = "h", [".."] = "i",
            [".---"] = "j", ["-.-"] = "k", [".-.."] = "l", ["--"] = "m", ["-."] = "n",
            ["---"] = "o", [".--."] = "p", ["--.-"] = "q", [".-."] = "r", ["..."] = "s",
            ["-"] = "t", ["..-"] = "u", ["...-"] = "v", [".--"] = "w", ["-..-"] = "x",
            ["-.--"] = "y", ["--.."] = "z", [".-.-.-"] = ".", ["--..--"] = ",", ["..--.."] = "?",
            ["-.-.--"] = "!", ["-....-"] = "-", ["-..-."] = "/", [".--.-."] = "@", ["-.--."] = "(",
            ["-.--.-"] = ")"
        };

        // Baconian
        public static Dictionary<string, string> Baconian26 = new()
        {
            ["AAAAA"] = "A", ["AAAAB"] = "B", ["AAABA"] = "C", ["AAABB"] = "D", ["AABAA"] = "E",
            ["AABAB"] = "F", ["AABBA"] = "G", ["AABBB"] = "H", ["ABAAA"] = "I", ["ABAAB"] = "J",
            ["ABABA"] = "K", ["ABABB"] = "L", ["ABBAA"] = "M", ["ABBAB"] = "N", ["ABBBA"] = "O",
            ["ABBBB"] = "P", ["BAAAA"] = "Q", ["BAAAB"] = "R", ["BAABA"] = "S", ["BAABB"] = "T",
            ["BABAA"] = "U", ["BABAB"] = "V", ["BABBA"] = "W", ["BABBB"] = "X", ["BBAAA"] = "Y",
            ["BBAAB"] = "Z"
        };

        public static Dictionary<string, string> Baconian24 = new()
        {
            ["AAAAA"] = "A", ["AAAAB"] = "B", ["AAABA"] = "C", ["AAABB"] = "D", ["AABAA"] = "E",
            ["AABAB"] = "F", ["AABBA"] = "G", ["AABBB"] = "H", ["ABAAA"] = "I", ["ABAAA"] = "J",
            ["ABAAB"] = "K", ["ABABA"] = "L", ["ABABB"] = "M", ["ABBAA"] = "N", ["ABBAB"] = "O",
            ["ABBBA"] = "P", ["ABBBB"] = "Q", ["BAAAA"] = "R", ["BAAAB"] = "S", ["BAABA"] = "T",
            ["BAABB"] = "U", ["BAABB"] = "V", ["BABAA"] = "W", ["BABAB"] = "X", ["BABBA"] = "Y",
            ["BABBB"] = "Z"
        };

        // Keyboard substituition
        // even though qwertz and azerty exist, they only change the position of 2 letters and are less likely to have a meaningful impact
        public static string QwertyLayout = """`1234567890-=qwertyuiop[]\asdfghjkl;'zxcvbnm,./~!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKL:"ZXCVBNM<>?""";
        public static string DvorakLayout = """`1234567890[]',.pyfgcrl/=\aoeuidhtns-;qjkxbmwvz~!@#$%^&*(){}"<>PYFGCRL?+|AOEUIDHTNS_:QJKXBMWVZ""";
        public static string ColemakLayout = """`1234567890-=qwfpgjluy;[]\arstdhneio'zxcvbkm,./~!@#$%^&*()_+QWFPGJLUY:{}|ARSTDHNEIO"ZXCVBKM<>?""";
        public static string WorkmanLayout = """`1234567890-=qdrwbjfup;[]\ashtgyneoi'zxmcvkl,./~!@#$%^&*()_+QDRWBJFUP:{}|ASHTGYNEOI"ZXMCVKL<>?""";

        // Tap code
        public static List<List<string>> TapCode = new()
        {
            new List<string>() {"A", "B", "C", "D", "E"},
            new List<string>() {"F", "G", "H", "I", "J"},
            new List<string>() {"L", "M", "N", "O", "P"},
            new List<string>() {"Q", "R", "S", "T", "U"},
            new List<string>() {"V", "W", "X", "Y", "Z"}
        };

        // DNA cipher
        public static Dictionary<string, string> DNA = new()
        {
            ["AAA"] = "A", ["AAC"] = "B", ["AAG"] = "C", ["AAT"] = "D", ["ACA"] = "E",
            ["ACC"] = "F", ["ACG"] = "G", ["ACT"] = "H", ["AGA"] = "I", ["AGC"] = "J",
            ["AGG"] = "K", ["AGT"] = "L", ["ATA"] = "M", ["ATC"] = "N",  ["ATG"] = "O",
            ["ATT"] = "P", ["CAA"] = "Q", ["CAC"] = "R", ["CAG"] = "S", ["CAT"] = "T",
            ["CCA"] = "U", ["CCC"] = "V", ["CCG"] = "W", ["CCT"] = "X", ["CGA"] = "Y",
            ["CGC"] = "Z", ["CGG"] = " "
        };

        // Base32
        public static Dictionary<char, int> Base32Map = new()
        {
            {'A', 0}, {'B', 1}, {'C', 2}, {'D', 3}, {'E', 4}, {'F', 5}, {'G', 6}, {'H', 7},
            {'I', 8}, {'J', 9}, {'K', 10}, {'L', 11}, {'M', 12}, {'N', 13}, {'O', 14}, {'P', 15},
            {'Q', 16}, {'R', 17}, {'S', 18}, {'T', 19}, {'U', 20}, {'V', 21}, {'W', 22}, {'X', 23},
            {'Y', 24}, {'Z', 25}, {'2', 26}, {'3', 27}, {'4', 28}, {'5', 29}, {'6', 30}, {'7', 31}
        };

        // Base58 alphabets
        public static string Base58BTC = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        public static string Base58Flickr = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
        public static string Base58Ripple = "rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz";

        // Base62
        public static Dictionary<char, int> Base62Map = new()
        {
            ['0'] = 0,  ['1'] = 1,  ['2'] = 2,  ['3'] = 3,  ['4'] = 4,
            ['5'] = 5,  ['6'] = 6,  ['7'] = 7,  ['8'] = 8,  ['9'] = 9,
            ['A'] = 10, ['B'] = 11, ['C'] = 12, ['D'] = 13, ['E'] = 14,
            ['F'] = 15, ['G'] = 16, ['H'] = 17, ['I'] = 18, ['J'] = 19,
            ['K'] = 20, ['L'] = 21, ['M'] = 22, ['N'] = 23, ['O'] = 24,
            ['P'] = 25, ['Q'] = 26, ['R'] = 27, ['S'] = 28, ['T'] = 29,
            ['U'] = 30, ['V'] = 31, ['W'] = 32, ['X'] = 33, ['Y'] = 34,
            ['Z'] = 35, ['a'] = 36, ['b'] = 37, ['c'] = 38, ['d'] = 39,
            ['e'] = 40, ['f'] = 41, ['g'] = 42, ['h'] = 43, ['i'] = 44,
            ['j'] = 45, ['k'] = 46, ['l'] = 47, ['m'] = 48, ['n'] = 49,
            ['o'] = 50, ['p'] = 51, ['q'] = 52, ['r'] = 53, ['s'] = 54,
            ['t'] = 55, ['u'] = 56, ['v'] = 57, ['w'] = 58, ['x'] = 59,
            ['y'] = 60, ['z'] = 61
        };

        // Base64
        public static Dictionary<char, int> Base64Map = new()
        {
            ['A'] = 0, ['B'] = 1, ['C'] = 2, ['D'] = 3, ['E'] = 4,
            ['F'] = 5, ['G'] = 6, ['H'] = 7, ['I'] = 8, ['J'] = 9,
            ['K'] = 10, ['L'] = 11, ['M'] = 12, ['N'] = 13, ['O'] = 14,
            ['P'] = 15, ['Q'] = 16, ['R'] = 17, ['S'] = 18, ['T'] = 19,
            ['U'] = 20, ['V'] = 21, ['W'] = 22, ['X'] = 23, ['Y'] = 24,
            ['Z'] = 25, ['a'] = 26, ['b'] = 27, ['c'] = 28, ['d'] = 29,
            ['e'] = 30, ['f'] = 31, ['g'] = 32, ['h'] = 33, ['i'] = 34,
            ['j'] = 35, ['k'] = 36, ['l'] = 37, ['m'] = 38, ['n'] = 39,
            ['o'] = 40, ['p'] = 41, ['q'] = 42, ['r'] = 43, ['s'] = 44,
            ['t'] = 45, ['u'] = 46, ['v'] = 47, ['w'] = 48, ['x'] = 49,
            ['y'] = 50, ['z'] = 51, ['0'] = 52, ['1'] = 53, ['2'] = 54,
            ['3'] = 55, ['4'] = 56, ['5'] = 57, ['6'] = 58, ['7'] = 59,
            ['8'] = 60, ['9'] = 61, ['+'] = 62, ['/'] = 63
        };


        // Base85
        public static Dictionary<char, int> Base85Map = new()
        {
            {'!', 0}, {'"', 1}, {'#', 2}, {'$', 3}, {'%', 4}, {'&', 5}, {'\'', 6}, {'(', 7},
            {')', 8}, {'*', 9}, {'+', 10}, {',', 11}, {'-', 12}, {'.', 13}, {'/', 14},
            {'0', 15}, {'1', 16}, {'2', 17}, {'3', 18}, {'4', 19}, {'5', 20}, {'6', 21}, {'7', 22},
            {'8', 23}, {'9', 24}, {':', 25}, {';', 26}, {'<', 27}, {'=', 28}, {'>', 29}, {'?', 30},
            {'@', 31}, {'A', 32}, {'B', 33}, {'C', 34}, {'D', 35}, {'E', 36}, {'F', 37}, {'G', 38},
            {'H', 39}, {'I', 40}, {'J', 41}, {'K', 42}, {'L', 43}, {'M', 44}, {'N', 45}, {'O', 46},
            {'P', 47}, {'Q', 48}, {'R', 49}, {'S', 50}, {'T', 51}, {'U', 52}, {'V', 53}, {'W', 54},
            {'X', 55}, {'Y', 56}, {'Z', 57}, {'[', 58}, {'\\', 59}, {']', 60}, {'^', 61}, {'_', 62},
            {'`', 63}, {'a', 64}, {'b', 65}, {'c', 66}, {'d', 67}, {'e', 68}, {'f', 69}, {'g', 70},
            {'h', 71}, {'i', 72}, {'j', 73}, {'k', 74}, {'l', 75}, {'m', 76}, {'n', 77}, {'o', 78},
            {'p', 79}, {'q', 80}, {'r', 81}, {'s', 82}, {'t', 83}, {'u', 84}
        };

        // Baudot
        public static Dictionary<string, char> Baudot = new()
        {
            // letters
            ["00011"] = 'A', ["11001"] = 'B', ["01110"] = 'C', ["01001"] = 'D', ["00001"] = 'E',
            ["01101"] = 'F', ["11010"] = 'G', ["10100"] = 'H', ["00110"] = 'I', ["01011"] = 'J',
            ["01111"] = 'K', ["10010"] = 'L', ["11100"] = 'M', ["01100"] = 'N', ["11000"] = 'O',
            ["10110"] = 'P', ["10111"] = 'Q', ["01010"] = 'R', ["00101"] = 'S', ["10000"] = 'T',
            ["00111"] = 'U', ["11110"] = 'V', ["10011"] = 'W', ["11101"] = 'X', ["10101"] = 'Y',
            ["10001"] = 'Z', ["00100"] = ' ',

            // figures (has _)
            ["10110_"] = '0', ["00011_"] = '1', ["11001_"] = '2', ["01110_"] = '3', ["01001_"] = '4',
            ["00001_"] = '5', ["01101_"] = '6', ["11010_"] = '7', ["10100_"] = '8', ["00110_"] = '9',
            ["01111_"] = '-', ["11000_"] = '?', ["10011_"] = ':', ["11101_"] = '(', ["10101_"] = ')',
            ["11100_"] = '.', ["10010_"] = ',', ["11110_"] = '\'', ["10001_"] = '/', ["00101_"] = '=',
            ["01010_"] = '+',

            // others
            ["00000"] = '\0', ["01000"] = '\n'
            // ["11011"] = "swicth to figures", ["11111"] = "switch to letters"
        };

        // Trilateral
        public static Dictionary<string, char> TrilateralNormal = new() 
        {
            ["AAA"] = 'A', ["AAB"] = 'B', ["AAC"] = 'C', ["ABA"] = 'D', ["ABB"] = 'E',
            ["ABC"] = 'F', ["ACA"] = 'G', ["ACB"] = 'H', ["ACC"] = 'I', ["BAA"] = 'J',
            ["BAB"] = 'K', ["BAC"] = 'L', ["BBA"] = 'M', ["BBB"] = 'N', ["BBC"] = 'O',
            ["BCA"] = 'P', ["BCB"] = 'Q', ["BCC"] = 'R', ["CAA"] = 'S', ["CAB"] = 'T',
            ["CAC"] = 'U', ["CBA"] = 'V', ["CBB"] = 'W', ["CBC"] = 'X', ["CCA"] = 'Y',
            ["CCB"] = 'Z',
            ["CCC"] = ' '
        };

        public static Dictionary<string, char> TrilateralSwapped = new()
        {
            ["AAA"] = ' ', ["AAB"] = 'A', ["AAC"] = 'B', ["ABA"] = 'C', ["ABB"] = 'D',
            ["ABC"] = 'E', ["ACA"] = 'F', ["ACB"] = 'G', ["ACC"] = 'H', ["BAA"] = 'I',
            ["BAB"] = 'J', ["BAC"] = 'K', ["BBA"] = 'L', ["BBB"] = 'M', ["BBC"] = 'N', 
            ["BCA"] = 'O', ["BCB"] = 'P', ["BCC"] = 'Q', ["CAA"] = 'R', ["CAB"] = 'S',
            ["CAC"] = 'T', ["CBA"] = 'U', ["CBB"] = 'V', ["CCA"] = 'X', ["CCB"] = 'Y',
            ["CCC"] = 'Z'
        }; // the space in this one is swapped, and also P is the same as W for some reason. i chose P cause its more common
    }
}
