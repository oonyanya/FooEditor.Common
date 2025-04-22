using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FooEditEngine;

namespace FooEditor
{
    public sealed class OutlineItem : FoldingItem
    {
        /// <summary>
        /// コンストラクター
        /// </summary>
        public OutlineItem(long start, long end, int level)
            : base(start, end)
        {
            this.Level = level;
        }

        /// <summary>
        /// アウトラインレベル
        /// </summary>
        public int Level
        {
            get;
            set;
        }
    }

    sealed class RegexFoldingGenerator : IFoldingStrategy
    {
        Regex[] BeginBracket, EndBracket;
        public RegexFoldingGenerator(string[] begin, string[] end)
        {
            if (begin == null || end == null)
                throw new ArgumentNullException();
            if (begin.Length != end.Length)
                throw new ArgumentException();

            this.BeginBracket = new Regex[begin.Length];
            this.EndBracket = new Regex[end.Length];

            for (int i = 0; i < begin.Length; i++)
            {
                this.BeginBracket[i] = new Regex(begin[i], RegexOptions.IgnoreCase);
                this.EndBracket[i] = new Regex(end[i], RegexOptions.IgnoreCase);
            }
        }

        public IEnumerable<FoldingItem> AnalyzeDocument(Document doc, long start, long end)
        {
            Stack<long> beginIndexs = new Stack<long>();
            int level = 0;
            long lineHeadIndex = start;
            foreach (string lineStr in doc.GetLines(start, end))
            {
                Match m = null;
                if(level < this.BeginBracket.Length)
                    m = this.BeginBracket[level].Match(lineStr);
                if (m != null && m.Success)
                {
                    beginIndexs.Push(lineHeadIndex + m.Index);
                    level++;
                }
                m = null;
                if(level < this.EndBracket.Length)
                    m = this.EndBracket[level].Match(lineStr);
                if (m != null && m.Success)
                {
                    if (beginIndexs.Count == 0)
                        continue;
                    long beginIndex = beginIndexs.Pop();
                    long endIndex = lineHeadIndex + m.Index + m.Length - 1;
                    if (beginIndex < endIndex)
                        yield return new OutlineItem(beginIndex, endIndex,level);
                    level--;
                }
                lineHeadIndex += lineStr.Length;
            }
        }
    }
}
