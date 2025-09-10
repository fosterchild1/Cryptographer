<img src="https://github.com/fosterchild1/Cryptographer/blob/master/resources/icon.ico" width="64" height="64"> <img src="https://github.com/fosterchild1/Cryptographer/blob/master/resources/text.png" width="381" height="61"> 

A program that deciphers encoded strings, even if they have multiple layers of encryption. It currently supports <b>22</b> ciphers:
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
| uuencoding | |

# Console Arguments
Running the app through the terminal offers better customization than double clicking the .exe.
<br/>

| Argument | Data type | Default value | What it does |
| ---  | --- | --- | --- |
| input= | string | "" | self-explanatory, does not need to be wrapped in quotes |
| maxdepth= | int | 24 | sets the max depth |
| plaintext= | float | 10 | minimum score needed for an output to be considered plaintext |
| threads= | byte | Processor count |  amount of cpu cores to be used by the program |
