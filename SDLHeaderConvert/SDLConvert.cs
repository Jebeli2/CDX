namespace SDLHeaderConvert
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLConvert
    {

        //private readonly static char[] split = new char[] { ' ', '(', ')', '*', ';' };
        private StringBuilder converted = new StringBuilder();
        private StringTokenizer tokenizer;
        private string currentToken;
        public SDLConvert(string name)
        {
        }

        public void StartConvert(string name)
        {
            converted.Clear();
            converted.Append(GetStart(name, name, name));
        }

        public string EndConvert()
        {
            converted.Append(GetEnd());
            return converted.ToString();
        }

        public void AddFile(string name, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                converted.AppendLine(GetFromComment(name));
                tokenizer = new StringTokenizer(text, " (,)*;\n\r", true);
                while (NextToken())
                {
                    if (MatchStart("/*"))
                    {
                        SkipComment();
                    }
                    else
                    {
                        //Console.WriteLine($"\"{currentToken}\"");
                        if (Match("#define"))
                        {
                            ParseDefine();

                        }
                        else if (Match("extern"))
                        {
                            ParseExtern();
                        }
                    }
                }
            }
        }

        private void SkipComment()
        {
            do
            {

            }
            while (!NextMatchEnd("*/"));
        }

        private bool NextToken()
        {
            if (tokenizer.HasMoreTokens())
            {
                currentToken = tokenizer.NextToken();
                if (currentToken == " " || currentToken == "\t")
                {
                    return NextToken();
                }
                return true;
            }
            return false;
        }

        private bool MatchStart(string tok)
        {
            return currentToken.StartsWith(tok, StringComparison.Ordinal);
        }
        private bool MatchEnd(string tok)
        {
            return currentToken.EndsWith(tok, StringComparison.Ordinal);
        }

        private bool Match(string tok)
        {
            return currentToken.Equals(tok);
        }

        private bool NextMatchStart(string tok)
        {
            if (NextToken())
            {
                return MatchStart(tok);
            }
            return false;
        }

        private bool NextMatchEnd(string tok)
        {
            if (NextToken())
            {
                return MatchEnd(tok);
            }
            return false;
        }
        private bool NextMatch(string tok)
        {
            if (NextToken())
            {
                return Match(tok);
            }
            return false;
        }


        private void ParseDefine()
        {
            if (NextToken())
            {
                string name = currentToken;
                if (NextToken())
                {
                    string value = currentToken.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        converted.AppendLine($"\t\tpublic const uint {name} = {value};");
                    }
                }
            }
        }

        private void ParseExtern()
        {
            if (NextMatch("DECLSPEC"))
            {
                if (NextToken())
                {
                    string returnType = currentToken;
                    if (NextToken())
                    {
                        if (currentToken == "*")
                        {
                            returnType += "*";
                            if (!NextToken())
                            {
                                return;
                            }
                        }
                        if (currentToken == "SDLCALL")
                        {
                            if (NextToken())
                            {
                                string name = currentToken;
                                if (NextMatch("("))
                                {
                                    converted.AppendLine("\t\t[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]");
                                    converted.Append("\t\tpublic static extern ");
                                    converted.Append(MapType(returnType));
                                    converted.Append(" ");
                                    converted.Append(name);
                                    converted.Append("(");
                                    ParseParams();
                                    converted.AppendLine(");");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ParseParams()
        {
            IList<string> p = CollectTokens(",", ")");
            int numParams = 0;
            while (p.Count > 0)
            {
                ParseParam(p);
                numParams++;
                if (currentToken == ")")
                {
                    break;
                }
                p = CollectTokens(",", ")");
            }
            if (numParams > 0)
            {
                converted.Length -= 2;
            }
        }

        private void ParseParam(IList<string> tokens)
        {
            if (tokens.Count == 1 && tokens[0] == "void")
            {
                converted.Append("  ");
                return;
            }
            string name = tokens[tokens.Count - 1].Trim();
            string type = tokens[0].Trim();
            for (int i = 1; i < tokens.Count - 1; i++)
            {
                type += tokens[i].Trim();
            }
            converted.Append($"{MapType(type)} {name}, ");
        }

        private IList<string> CollectTokens(params string[] untilTokens)
        {
            List<string> tokens = new List<string>();
            while (NextToken())
            {
                if (untilTokens.Contains(currentToken))
                {
                    break;
                }
                tokens.Add(currentToken);
            }
            return tokens;
        }

        private string GetFromComment(string name)
        {
            return $"\t\t\\\\ From File {name}";
        }

        private static string MapType(string type)
        {
            switch (type)
            {
                case "constchar*": return "string";
            }
            if (type.EndsWith("*")) return "IntPtr";
            switch (type)
            {
                case "SDL_bool": return "bool";
                case "Uint8": return "byte";
                case "Uint16": return "ushort";
                case "Uint32": return "uint";
                case "Uint64": return "ulong";
                case "Sint16": return "short";
                case "Sint32": return "int";
                case "Sint64": return "long";
                case "size_t": return "IntPtr";
                default:
                    return type;
            }
        }

        private string GetStart(string nameSpaceName, string className, string libName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("namespace ");
            sb.AppendLine(nameSpaceName);
            sb.AppendLine("{");
            sb.Append("\tpublic static class ");
            sb.AppendLine(className);
            sb.AppendLine("\t{");
            sb.Append("\t\tprivate const string libName = \"");
            sb.Append(libName);
            sb.AppendLine("\";");

            return sb.ToString();
        }

        private string GetEnd()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
