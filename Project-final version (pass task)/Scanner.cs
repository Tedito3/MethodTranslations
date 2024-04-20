using System;
using System.IO;
using System.Text;

namespace CompilerSimpleCSharp
{
    class Scanner{
        const char EOF = '\u001a';
        const char CR = '\r';
        const char LF = '\n';

        private static readonly string keyWords = " scanf printf zoom ";
        private static readonly string specialSymbols1 = "();*/%";
        private static readonly string specialSymbols2 = "+=-";
        private static readonly string specialSymbols2Pairs = " ++ -- "; //Warning:Need space between pair

        private TextReader reader;
        private char ch;//пореден занк
        public Scanner(TextReader reader){
            this.reader = reader;
            ReadNextChar();
        }
        internal Token Next(){
            while (true){
                //Правим проверка за малка или голяма буква.
                if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z'){
                    StringBuilder s = new StringBuilder();
                    while (ch >= 'a' && ch <= 'z'|| ch >= 'A' && ch <= 'Z'|| ch >= '0' && ch <= '9'){
                        s.Append(ch);
                        ReadNextChar();
                    }
                    string id = s.ToString();
                    if (keyWords.Contains(string.Format($" {id} "))){
                        return new KeywordToken(id);
                    }
                    return new IdentToken(s.ToString());
                }
                //Проверка на число в диапазона от 0 до 9
                if (ch >= '0' && ch <= '9'){
                    StringBuilder s = new StringBuilder();
                    while (ch >= '0' && ch <= '9'){
                        s.Append(ch);
                        ReadNextChar();
                    }
                    return new NumberToken(Convert.ToInt64(s.ToString()));
                }
                //Проверка за празни пространства...
                if (ch == CR || ch == LF || ch == ' ' || ch == '\t'){
                    ReadNextChar();
                    continue;
                }
                //Проверка за първи специален символ...
                if (specialSymbols1.Contains(ch.ToString())){
                    char specialChar = ch;
                    ReadNextChar();
                    return new SpecialSymbolToken(specialChar.ToString());
                }
                //Проверка за втори специален символ...
                if (specialSymbols2.Contains(ch.ToString())){
                    char spCharOne = ch;
                    ReadNextChar();
                    char spCharTwo = ch;
                    if (specialSymbols2Pairs.Contains(" " +spCharOne + spCharTwo + " ")){
                        ReadNextChar();
                        return new SpecialSymbolToken(spCharOne.ToString() + spCharTwo.ToString());
                    }
                    return new SpecialSymbolToken(spCharOne.ToString());
                }
                if (ch == EOF){
                    return new EOFToken();
                }
                string str = ch.ToString();
                ReadNextChar();
                return new OtherToken(str);
            }
        }
        private void ReadNextChar(){
            int charOne = reader.Read();
            if (charOne < 0){
                ch = EOF;
            }
            else{
                ch = (char)charOne;
                if (ch == CR){
                }
                else if (ch == LF) {
                }
            }
        }
    }
}
