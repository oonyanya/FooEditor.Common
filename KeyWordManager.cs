using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FooEditor
{
    /// <summary>
    /// キーワード管理クラス
    /// </summary>
    public sealed class SyntaxDefnition
    {
        public const string XmlHilighter = "xml";

        public const string CLangFolding = "clang";

        public SyntaxDefnition()
        {
            this.Clear();
            this.init();
        }
        /// <summary>
        /// キーワードのリストを作る
        /// </summary>
        /// <param name="xmlpath">解析対象となるXMLファイル</param>
        public void generateKeywordList(string xmlpath)
        {
            var root = XElement.Load(xmlpath);
            this.Hilighter = GetAttribute(root,"hilighter");
            this.SingleComment = GetStringItems(root, "singlelinecomment");
            this.CommentStart = GetStringItems(root, "mutilinecommentstart");
            this.CommentEnd = GetStringItems(root, "mutilinecommentend");
            this.Operators = GetChars(root, "operators");
            this.Literals = GetStringItems(root, "literals");
            this.Keywords = GetStringItems(root, "keywords");
            this.Keywords2 = GetStringItems(root, "keywords2");
            this.IntendStart = GetStringItems(root, "intendstart");
            this.IntendEnd = GetStringItems(root, "intendend");
            this.FoldingBegin = GetItem(root, "foldingbegin");
            this.FoldingEnd = GetItem(root, "foldingend");
            this.FoldingMethod = GetAttribute(root, "foldingmethod");
        }

        /// <summary>
        /// 使用するフォールディングの種類
        /// </summary>
        public string FoldingMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// 使用するハイライターの種類
        /// </summary>
        public string Hilighter
        {
            get;
            private set;
        }

        /// <summary>
        /// 行コメントを表す
        /// </summary>
        public string[] SingleComment
        {
            get;
            private set;
        }

        /// <summary>
        /// 複数行コメントの開始を表す
        /// </summary>
        public string[] CommentStart
        {
            get;
            private set;
        }

        /// <summary>
        /// 複数行コメントの終わりを表す
        /// </summary>
        public string[] CommentEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// 色付けするキーワードを指定する
        /// </summary>
        public string[] Keywords
        {
            get;
            private set;
        }

        /// <summary>
        /// 色付けするキーワード2を指定する
        /// </summary>
        public string[] Keywords2
        {
            get;
            private set;
        }

        /// <summary>
        /// 演算子もしくは区切り文字を表す
        /// </summary>
        public char[] Operators
        {
            get;
            private set;
        }

        /// <summary>
        /// 文字列リテラルの開始と終了を表す
        /// </summary>
        public string[] Literals
        {
            get;
            private set;
        }

        /// <summary>
        /// オートインテンドを開始する文字列を表す
        /// </summary>
        public string[] IntendStart
        {
            get;
            private set;
        }
        
        /// <summary>
        /// オートインテンドを終了させる文字列を表す
        /// </summary>
        public string[] IntendEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// 折り畳みの開始を表す
        /// </summary>
        public string[] FoldingBegin
        {
            get;
            private set;
        }

        /// <summary>
        /// 折り畳みの終了を表す
        /// </summary>
        public string[] FoldingEnd
        {
            get;
            private set;
        }

        /// <summary>
        /// 初期状態に戻す
        /// </summary>
        public void Clear()
        {
            this.SingleComment = new string[0];
            this.CommentStart = new string[0];
            this.CommentEnd = new string[0];
            this.IntendEnd = new string[0];
            this.IntendStart = new string[0];
            this.Keywords = new string[0];
            this.Keywords2 = new string[0];
            this.Literals = new string[0];
            this.Operators = new char[0];
            this.FoldingBegin = null;
            this.FoldingEnd = null;
            this.init();
        }

        #region
        void init()
        {
            this.Operators = new char[] { ' ', '\t', '\n' };
        }

        string GetAttribute(XElement element,string name)
        {
            XAttribute attr = element.Attribute(name);
            if (attr == null)
                return string.Empty;
            return attr.Value;
        }

        string[] GetItem(XElement root, string name)
        {
            IEnumerable<XElement> elements = root.Elements(name);

            if (elements == null)
                return new string[]{string.Empty};

            List<string> result = new List<string>();
            foreach (var element in elements)
                result.Add(element.Value);
            return result.ToArray();
        }

        char? GetCharItem(XElement root, string name)
        {
            XElement element = root.Element(name);

            if (element == null)
                return null;

            return element.Value[0];
        }

        string[] GetStringItems(XElement root, string name)
        {
            XElement element = root.Element(name);

            if (element == null)
                return new string[0];

            IEnumerable<XElement> item = element.Elements("item");

            List<string> to = new List<string>();

            foreach (XElement e in item)
                to.Add(e.Value);

            return to.ToArray();
        }

        char[] GetChars(XElement root,string name)
        {
            XElement element = root.Element(name);

            if (element == null)
                return new char[0];

            IEnumerable<XElement> item = element.Elements("item");

            List<char> to = new List<char>();
            to.Add(' ');
            to.Add('\t');
            to.Add('\n');

            foreach (XElement e in item)
                to.Add(e.Value[0]);

            return to.ToArray();
        }

        #endregion

    }

}
