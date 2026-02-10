using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using FooEditEngine;
using FooEditor.WinUI.Models;

namespace FooEditor
{
    enum TextParserMode
    {
        ScriptPart,
        SingleLineComment,
        MultiLineComment,
        MultiLineLiteral,
        Literal,
        TextPart,
    }

    /// <summary>
    /// トークンに分けるためのクラス
    /// </summary>
    sealed class GenericHilighter : HilighterLegacyBase
    {
        private SyntaxDefnition km;
        private StringBuilder token;
        private StringBuilder word;
        private TextParserMode mode = TextParserMode.TextPart;
        private StateTarnstionTable[] TranstionTable;

        public GenericHilighter()
        {
            this.token = new StringBuilder();
            this.word = new StringBuilder();
        }

        /// <summary>
        /// 登録されているキーワードのリスト
        /// </summary>
        public SyntaxDefnition KeywordManager
        {
            get
            {
                return km;
            }
            set
            {
                km = value;
                this.Reset();
                TranstionTable = new[]{
                    //CoffeeScriptだと複数行は###で、単一行は#なので、複数行コメントを優先させる
                    new StateTarnstionTable(TextParserMode.ScriptPart,TextParserMode.MultiLineComment,this.km.CommentStart,false,false),
                    new StateTarnstionTable(TextParserMode.ScriptPart,TextParserMode.SingleLineComment,this.km.SingleComment,false,false),
                    new StateTarnstionTable(TextParserMode.ScriptPart,TextParserMode.Literal,this.km.Literals,false,true),
                    new StateTarnstionTable(TextParserMode.MultiLineComment,TextParserMode.ScriptPart,this.km.CommentEnd,true,false),
                    new StateTarnstionTable(TextParserMode.Literal,TextParserMode.ScriptPart,this.km.Literals,true,true),
                };
            }
        }


        public override void Reset()
        {
            this.word.Remove(0, word.Length);
            this.mode = TextParserMode.ScriptPart;
        }

        public override int DoHilight(string text, int length, TokenSpilitHandeler action)
        {
            int encloserLevel = 0;

            //行をまたいでは困るリスト
            if (this.mode == TextParserMode.SingleLineComment)
                this.mode = TextParserMode.ScriptPart;

            this.word.Clear();
            this.token.Clear();

            bool isBreaked = false;

            int cur, wordPos = 0;
            TokenSpilitEventArgs e = new TokenSpilitEventArgs();
            for (cur = 0; cur < length; )
            {
                int i;
                bool ismathed = false;
                for (i = 0; i < TranstionTable.Length; i++)
                {
                    if (this.isMatchCollctions(text, TranstionTable[i].pattern, cur) && this.mode == TranstionTable[i].trigger)
                    {
                        if(TranstionTable[i].checkEscape && cur > 0 && text[cur-1] == '\\')
                            ismathed = false;
                        else
                            ismathed = true;
                        break;
                    }
                }
                if (ismathed || isMatchCollctions(text, km.Operators, cur) )
                {
                    if (this.word.Length > 0)
                    {
                        e.index = wordPos;
                        e.length = word.Length;
                        e.type = GetTokenType(this.mode, this.word.ToString());
                        action(e);
                        if (e.breaked)
                            break;
                        this.word.Clear();
                    }

                    if (ismathed && TranstionTable[i].end == false)
                    {
                        this.mode = TranstionTable[i].to;
                        encloserLevel++;
                    }

                    e.index = cur;
                    e.length = this.token.Length;
                    e.type = GetTokenType(this.mode, this.token.ToString());
                    action(e);

                    if (ismathed && TranstionTable[i].end)
                    {
                        this.mode = TranstionTable[i].to;
                        encloserLevel--;
                    }
                    if (this.mode == TextParserMode.SingleLineComment && FooEditEngine.Util.IsHasNewLine(text))
                    {
                        this.mode = TextParserMode.ScriptPart;
                        encloserLevel--;
                    }

                    if (e.breaked)
                        break;

                    cur += this.token.Length;
                }
                else
                {
                    if (word.Length == 0)
                        wordPos = cur;
                    this.word.Append(text[cur]);
                    cur++;
                }
            }
            if (this.word.ToString() != "" && isBreaked == false)
            {
                e.index = wordPos;
                e.length = word.Length;
                e.type = GetTokenType(this.mode, this.word.ToString());
                action(e);
            }

            return encloserLevel;
        }

        private struct StateTarnstionTable
        {
            public TextParserMode trigger;
            public TextParserMode to;
            public string[] pattern;
            public bool end;
            public bool checkEscape;
            public StateTarnstionTable(TextParserMode trigger, TextParserMode to, string[] pattern, bool end,bool escape)
            {
                this.trigger = trigger;
                this.to = to;
                this.pattern = pattern;
                this.end = end;
                this.checkEscape = escape;
            }
        }

        /// <summary>
        /// コレクションに含まれる文字と一致しているかどうかを調べる
        /// </summary>
        /// <param name="c"></param>
        /// <param name="collection"></param>
        /// <returns>一致した場合は真を返し、一致した文字がthis.tokenにセットされる</returns>
        private bool isMatchCollctions(string sb,char[] collection, int i)
        {
            if (collection == null)
                return false;
            if (collection.Contains(sb[i]))
            {
                this.token.Clear();
                this.token.Append(sb[i]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// コレクションに含まれる文字と一致しているかどうかを調べる
        /// </summary>
        /// <param name="c"></param>
        /// <param name="collection"></param>
        /// <returns>一致した場合は真を返し、一致した文字がstate.tokenにセットされる</returns>
        private bool isMatchCollctions(string sb, string[] collection, int index)
        {
            if (collection == null)
                return false;
            for (int i = 0; i < collection.Length; i++)
            {
                if (Match(sb,collection[i],index))
                {
                    this.token.Clear();
                    this.token.Append(collection[i]);
                    return true;
                }
            }
            return false;
        }

        bool Match(string sb, string pattern, int i)
        {
            if (pattern == null || i + pattern.Length > sb.Length)
                return false;
            int j;
            for (j = 0; j < pattern.Length && sb[i + j] == pattern[j]; j++) ;
            if (j == pattern.Length)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// トークンの属性を取得する
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        TokenType GetTokenType(TextParserMode mode, string word)
        {
            TokenType type = TokenType.None;
            switch (mode)
            {
                case TextParserMode.MultiLineComment:
                case TextParserMode.SingleLineComment:
                    type = TokenType.Comment;
                    break;
                case TextParserMode.ScriptPart:
                    if (this.KeywordManager.Keywords.Contains(word))
                        type = TokenType.Keyword1;
                    else if (this.KeywordManager.Keywords2.Contains(word))
                        type = TokenType.Keyword2;
                    break;
                case TextParserMode.Literal:
                    type = TokenType.Literal;
                    break;
            }
            return type;
        }

    }

}
