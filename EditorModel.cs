using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FooEditEngine;

namespace FooEditor
{
    struct PrintInfomation
    {
        public int PageNumber;
        public string Title;
    }
    static class EditorHelper
    {
        public static string ParseHF(string org,PrintInfomation info)
        {
            string str = org.Replace("%f", info.Title);
            return str.Replace("%p", info.PageNumber.ToString());
        }

        public static Tuple<IFoldingStrategy,IHilighter> GetFoldingAndHilight(string file_path)
        {
            IHilighter hilighter;
            IFoldingStrategy folding;

            SyntaxDefnition SynataxDefnition = new SyntaxDefnition();
            SynataxDefnition.generateKeywordList(file_path);

            if (SynataxDefnition.Hilighter == FooEditor.SyntaxDefnition.XmlHilighter)
            {
                XmlHilighter Hilighter = new XmlHilighter();
                Hilighter.KeywordManager = SynataxDefnition;
                hilighter = Hilighter;
            }
            else
            {
                GenericHilighter Hilighter = new GenericHilighter();
                Hilighter.KeywordManager = SynataxDefnition;
                hilighter = Hilighter;
            }

            if (SynataxDefnition.FoldingBegin != null && SynataxDefnition.FoldingEnd != null)
            {
                if (SynataxDefnition.FoldingMethod == FooEditor.SyntaxDefnition.CLangFolding)
                    folding = new CLangFoldingGenerator(SynataxDefnition.FoldingBegin, SynataxDefnition.FoldingEnd, '{', '}');
                else
                    folding = new RegexFoldingGenerator(SynataxDefnition.FoldingBegin, SynataxDefnition.FoldingEnd);
            }
            else
            {
                folding = null;
            }

            return new Tuple<IFoldingStrategy, IHilighter>(folding, hilighter);
        }

    }
}
