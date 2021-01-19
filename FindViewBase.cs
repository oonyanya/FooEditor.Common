#if METRO || WPF
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#if METRO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FooEditEngine.Metro;
#endif
#if WPF
using System.Windows;
using System.Windows.Controls;
using FooEditEngine.WPF;
#endif
using FooEditEngine;

namespace FooEditor
{
    public class FindViewBase : UserControl,IFindView
    {
        static Color foundMarkerColor = new Color(64, 128, 128, 128);
        bool canReplaceNext = false;
        IEnumerator<Tuple<FooTextBox, SearchResult>> iterator;
        FindFlyoutViewModel model;
        public const int FoundMarkerID = 2;

        internal FindFlyoutViewModel FindViewModel
        {
            get
            {
                return this.model;
            }
        }

        public Color FoundMarkerColor
        {
            get
            {
                return foundMarkerColor;
            }
            set
            {
                foundMarkerColor = value;
                foreach (FooTextBox textBox in this.GetTextBoxs())
                {
                    textBox.MarkerPatternSet.Remove(FoundMarkerID);
                    if (this.ShowFoundPattern && this.iterator != null)
                    {
                        textBox.MarkerPatternSet.Add(FoundMarkerID, textBox.Document.CreateWatchDogByFindParam(HilightType.Select, foundMarkerColor));
                    }
                    textBox.Refresh();
                }
            }
        }

        public FindViewBase()
        {
            this.model = new FindFlyoutViewModel(this);
            this.model.PropertyChanged += model_PropertyChanged;
            this.DataContext = this.model;
        }

        void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FindPattern" || e.PropertyName == "AllDocuments")
            {
                this.iterator = null;
            }
        }

        protected virtual bool ShowFoundPattern
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected virtual string NotFoundInDocumentMessage
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void FindNext(string pattern, bool useregex, RegexOptions opt)
        {
            if (this.iterator == null)
            {
                this.iterator = this.GetSearchResult((textbox) =>
                {
                    textbox.Document.SetFindParam(pattern, useregex, opt);
                    if (this.ShowFoundPattern)
                    {
                        textbox.MarkerPatternSet.Remove(FoundMarkerID);
                        textbox.MarkerPatternSet.Add(FoundMarkerID, textbox.Document.CreateWatchDogByFindParam(HilightType.Select, foundMarkerColor));
                    }
                    return textbox.Document.Find();
                });
            }
            if (!this.iterator.MoveNext())
            {
                this.canReplaceNext = false;
                this.iterator = null;
                foreach (FooTextBox textBox in this.GetTextBoxs())
                    textBox.MarkerPatternSet.Remove(FoundMarkerID);
                throw new Exception(this.NotFoundInDocumentMessage);
            }
            else
            {
                SearchResult sr = this.iterator.Current.Item2;
                FooTextBox textBox = this.iterator.Current.Item1;
                textBox.JumpCaret(sr.End + 1);
                textBox.Select(sr.Start, sr.End - sr.Start + 1);
                textBox.Refresh();
                this.canReplaceNext = true;
            }
        }

        public void Replace(string newpattern, bool usegroup)
        {
            if (!this.canReplaceNext)
                return;
            if (newpattern == null)
                newpattern = string.Empty;
            SearchResult sr = this.iterator.Current.Item2;
            FooTextBox textBox = this.iterator.Current.Item1;
            var newText = usegroup ? sr.Result(newpattern) : newpattern;
            textBox.Document.Replace(textBox.Selection.Index, textBox.Selection.Length, newText);
            textBox.Refresh();
        }

        public void ReplaceAll(string pattern, string newpattern, bool usegroup, bool useregex, RegexOptions opt)
        {
            if (newpattern == null)
                newpattern = string.Empty;
            foreach (FooTextBox textBox in this.GetTextBoxs())
            {
                textBox.Document.FireUpdateEvent = false;
                try
                {
                    if (useregex)
                    {
                        textBox.Document.SetFindParam(pattern, useregex, opt);
                        textBox.Document.ReplaceAll(newpattern, usegroup);
                    }
                    else
                    {
                        textBox.Document.ReplaceAll2(pattern, newpattern, (opt & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase);
                    }
                }
                finally
                {
                    textBox.Document.FireUpdateEvent = true;
                }
                textBox.Refresh();
            }
        }

        public void Reset()
        {
            this.iterator = null;
        }

        /// <summary>
        /// イテレーターを生成する
        /// </summary>
        /// <returns>イテレーター</returns>
        protected virtual IEnumerable<FooTextBox> GetTextBoxs()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// イテレーターを生成する
        /// </summary>
        /// <param name="FindStartFunc">検索開始時に実行される関数</param>
        /// <returns>Tuple<FooTextBox, SearchResult>イテレーター</returns>
        /// <remarks>必ずオーバーライトする必要があります。オーバーライトする際はFindStartFuncを呼び出してください</remarks>
        protected virtual IEnumerator<Tuple<FooTextBox, SearchResult>> GetSearchResult(Func<FooTextBox, IEnumerator<SearchResult>> FindStartFunc)
        {
            throw new NotImplementedException();
        }
    }
}
#endif