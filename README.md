<img src="https://github.com/fosterchild1/Cryptographer/blob/master/resources/icon.ico" width="64" height="64"> <img src="https://github.com/fosterchild1/Cryptographer/blob/master/resources/text.png" width="381" height="61"> 

A program that deciphers encoded strings, even if they have multiple layers of encryption. It currently supports <b>30</b> ciphers:
<br/>

| Supported ciphers | Supported ciphers | Supported ciphers |
| ---  | --- | --- |
| ASCII | Base85 | Octal |
| ASCII Shift | Baudot | Playfair* |
| Atbash | Binary | Polybius* |
| A1Z26 | Beaufort* | Reverse |
| Baconian | Brainfuck | ROT-47 |
| Base32 | Caesar | Scytale |
| Base45 | DNA | Tap Code |
| Base58 | Hexadecimal | Trilateral |
| Base62 | Keyboard Substitution | uuencoding |
| Base64 | Morse | Vigenère* |

<b>A star</b> means that they only work when using the <b>usekey</b> config.

# CLI Arguments
The console offers some extra arguments that the config.ini file doesn't have. Any argument written in the console will override the one in config.ini. These are:
<br/>

| Argument | Data type | Default value | What it does |
| ---  | --- | --- | --- |
| in= | string | "" | the ciphertext. doesn't need to be wrapped in quotes. <b>can also be a .txt file placed in the same folder as the .exe</b> |
| key= | string | "" | the key that will be used. doesn't need to be wrapped in quotes |
| cfg= | string | config.ini | selects the config file used. <b>the file should be in the same folder as the .exe</b> |
<br/>
Plus the ones in config.ini. (eg. <code>Cryptographer.exe --in=encrypted.txt --maxdepth=1</code>)

# Build instructions (Windows, macOS & Linux)
### Build prerequisites: .NET SDK 9.0
1. Get the code: <code>git clone https://github.com/fosterchild1/Cryptographer.git</code> <b>&&</b> <code>cd Cryptographer</code>
2. Build: <code>dotnet build Cryptographer.sln -c Release</code>
