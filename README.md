<img src="https://github.com/fosterchild1/Cryptographer/blob/master/resources/icon.ico" width="64" height="64"> <img src="https://github.com/fosterchild1/Cryptographer/blob/master/resources/text.png" width="381" height="61"> 

A program that deciphers encoded strings, even if they have multiple layers of encryption. It currently supports <b>23</b> ciphers:
<br/>

| Supported ciphers | Supported ciphers | Supported ciphers |
| ---  | --- | --- |
| Reverse | Binary | Baudot |
| DNA | Octal | Morse |
| Atbash | Hexadecimal | Tap Code |
| Baconian | ASCII | Base32 |
| Trilateral | Caesar | Base62 |
| ROT-47 | Keyboard Substitution | Base64 |
| ASCII Shift | A1Z26 | Base85 |
| uuencoding | Brainfuck |

# CLI Arguments
The console offers some extra arguments that the config.ini file doesn't have. Any argument written in the console will override the one in config.ini. These are:
<br/>

| Argument | Data type | Default value | What it does |
| ---  | --- | --- | --- |
| input= | string | | self-explanatory, doesn't need to be wrapped in quotes. <b>can also be a .txt file placed in the same folder as the .exe</b> |
| cfg= | string | config.ini | selects the config file used. <b>the file should be in the same folder as the .exe</b> |
<br/>
Plus the ones in config.ini. (eg. <code>Cryptographer.exe input=encrypted.txt maxdepth=1</code>)

# Build instructions
### Build prerequisites: .NET SDK 9.0
1. Get the code: <code>git clone https://github.com/fosterchild1/Cryptographer.git</code> <b>&&</b> <code>cd Cryptographer</code>
2. Build: <code>dotnet build Cryptographer.sln -c Release</code>
