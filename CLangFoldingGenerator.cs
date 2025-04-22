using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FooEditEngine;

namespace FooEditor
{
    sealed class CLangFoldingGenerator : IFoldingStrategy
    {
        struct TextLevelInfo
        {
            public long Index;
            public int Level;
            public TextLevelInfo(long index, int level)
            {
                this.Index = index;
                this.Level = level;
            }
        }

        Regex[] BeginFolding, EndFolding;
        char BeginBracket, EndBracket;
        public CLangFoldingGenerator(string[] begin, string[] end, char beginbracket, char endbracket)
        {
            if (begin == null || end == null)
                throw new ArgumentNullException();
            if (begin.Length != end.Length)
                throw new ArgumentException();

            this.BeginFolding = new Regex[begin.Length];
            this.EndFolding = new Regex[end.Length];
            
            for (int i = 0; i < begin.Length; i++)
            {
                this.BeginFolding[i] = new Regex(begin[i], RegexOptions.IgnoreCase);
                this.EndFolding[i] = new Regex(end[i], RegexOptions.IgnoreCase);
            }
            
            this.BeginBracket = beginbracket;
            this.EndBracket = endbracket;
        }

        public IEnumerable<FoldingItem> AnalyzeDocument(Document doc, long start, long end)
        {
            Stack<TextLevelInfo> beginIndexs = new Stack<TextLevelInfo>();
            long lineHeadIndex = start;
            int level = 0;
            foreach (string lineStr in doc.GetLines(start, end))
            {
                Match m = null;
                if(level >= 0 && level < this.BeginFolding.Length)
                    m = this.BeginFolding[level].Match(lineStr);
                if (m != null && m.Success)
                    beginIndexs.Push(new TextLevelInfo(lineHeadIndex + m.Index, level));

                foreach (char c in lineStr)
                {
                    if (this.BeginBracket == c)
                        level++;
                    else if (this.EndBracket == c)
                        level--;
                }

                if (beginIndexs.Count == 0)
                    goto loopend;

                TextLevelInfo begin = beginIndexs.Peek();

                if (begin.Level == level)
                {
                    m = null;
                    if(level < this.EndFolding.Length)
                        m = this.EndFolding[level].Match(lineStr);
                    if (m != null && m.Success)
                    {
                        beginIndexs.Pop();
                        long endIndex = lineHeadIndex + m.Index + m.Length - 1;
                        if (begin.Index < endIndex)
                            yield return new OutlineItem(begin.Index, endIndex,level);
                    }
                }
            loopend:
                lineHeadIndex += lineStr.Length;
            }
        }
    }
}
