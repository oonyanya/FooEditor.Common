using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Reflection;
using FooEditEngine;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.Effects;

#if WINUI
using CommunityToolkit.WinUI.Helpers;
#elif WINDOWS_UWP
using Microsoft.Toolkit.Uwp.Helpers;
#endif

namespace FooEditor
{
    public class AppSettingsBase : INotifyPropertyChanged
    {
        protected ObservableCollection<FileType> _FileTypeCollection;

        public AppSettingsBase()
        {
            SetDefalutSetting();
            if (_FileTypeCollection == null)
                LoadFileTypeCollection();
        }

        protected virtual void SetDefalutSetting()
        {
        }

        /// <summary>
        /// ファイルタイプごとの設定を取得します
        /// </summary>
        protected virtual void LoadFileTypeCollection()
        {
        }

        /// <summary>
        /// ファイルタイプごとの設定を保存します
        /// </summary>
        /// <returns>Taskオブジェクト</returns>
        public virtual async Task Save()
        {
            await Task.Delay(1);
        }

        public bool IndentBySpace
        {
            get
            {
                return (bool)GetEditorProperty("IndentBySpace");
            }
            set
            {
                SetEditorProperty("IndentBySpace", value);
                this.OnChangedSetting();
            }
        }

        public bool ShowFoundPattern
        {
            get
            {
                return (bool)GetEditorProperty("ShowFoundPattern");
            }
            set
            {
                SetEditorProperty("ShowFoundPattern", value);
                this.OnChangedSetting();
            }
        }

        public LineBreakMethod LineBreakMethod
        {
            get
            {
                return (LineBreakMethod)(int)GetEditorProperty("LineBreakMethod");
            }
            set
            {
                SetEditorProperty("LineBreakMethod", (int)value);
                this.OnChangedSetting();
            }
        }

        public int TopMargin
        {
            get
            {
                return (int)GetEditorProperty("TopMargin");
            }
            set
            {
                SetEditorProperty("TopMargin", value);
                this.OnChangedSetting();
            }
        }

        public string Header
        {
            get
            {
                return (string)GetEditorProperty("Header");
            }
            set
            {
                SetEditorProperty("Header", value);
                this.OnChangedSetting();
            }
        }

        public string Footer
        {
            get
            {
                return (string)GetEditorProperty("Footer");
            }
            set
            {
                SetEditorProperty("Footer", value);
                this.OnChangedSetting();
            }
        }

        public int RightMargin
        {
            get
            {
                return (int)GetEditorProperty("RightMargin");
            }
            set
            {
                SetEditorProperty("RightMargin", value);
                this.OnChangedSetting();
            }
        }

        public int BottomMargin
        {
            get
            {
                return (int)GetEditorProperty("BottomMargin");
            }
            set
            {
                SetEditorProperty("BottomMargin", value);
                this.OnChangedSetting();
            }
        }

        public int LeftMargin
        {
            get
            {
                return (int)GetEditorProperty("LeftMargin");
            }
            set
            {
                SetEditorProperty("LeftMargin", value);
                this.OnChangedSetting();
            }
        }

        public int LineBreakCount
        {
            get
            {
                return (int)GetEditorProperty("LineBreakCount");
            }
            set
            {
                SetEditorProperty("LineBreakCount", value);
                this.OnChangedSetting();
            }
        }

        public ObservableCollection<FileType> FileTypeCollection
        {
            get
            {
                return _FileTypeCollection;
            }
        }

        public string FontFamily
        {
            get
            {
                return (string)GetEditorProperty("FontFamily");
            }
            set
            {
                SetEditorProperty("FontFamily", value);
                this.OnChangedSetting();
            }
        }

        public double FontSize
        {
            get
            {
                return (double)GetEditorProperty("FontSize");
            }
            set
            {
                SetEditorProperty("FontSize", value);
                this.OnChangedSetting();
            }
        }

        public int TabChar
        {
            get
            {
                return (int)GetEditorProperty("TabChar");
            }
            set
            {
                SetEditorProperty("TabChar", value);
                this.OnChangedSetting();
            }
        }

        public bool IsRTL
        {
            get
            {
                return (bool)GetEditorProperty("IsRTL");
            }
            set
            {
                SetEditorProperty("IsRTL", value);
                this.OnChangedSetting();
            }
        }

        public bool ShowTab
        {
            get
            {
                return (bool)GetEditorProperty("ShowTab");
            }
            set
            {
                SetEditorProperty("ShowTab", value);
                this.OnChangedSetting();
            }
        }

        public bool ShowFullSpace
        {
            get
            {
                return (bool)GetEditorProperty("ShowFullSpace");
            }
            set
            {
                SetEditorProperty("ShowFullSpace", value);
                this.OnChangedSetting();
            }
        }

        public bool ShowLineBreak
        {
            get
            {
                return (bool)GetEditorProperty("ShowLineBreak");
            }
            set
            {
                SetEditorProperty("ShowLineBreak", value);
                this.OnChangedSetting();
            }
        }

        public bool ShowRuler
        {
            get
            {
                return (bool)GetEditorProperty("ShowRuler");
            }
            set
            {
                SetEditorProperty("ShowRuler", value);
                this.OnChangedSetting();
            }
        }

        public bool ShowLineNumber
        {
            get
            {
                return (bool)GetEditorProperty("ShowLineNumber");
            }
            set
            {
                SetEditorProperty("ShowLineNumber", value);
                this.OnChangedSetting();
            }
        }

        public bool ShowLineMarker
        {
            get
            {
                return (bool)GetEditorProperty("ShowLineMarker");
            }
            set
            {
                SetEditorProperty("ShowLineMarker", value);
                this.OnChangedSetting();
            }
        }

        public bool EnableAutoIndent
        {
            get
            {
                return (bool)GetEditorProperty("EnableAutoIndent");
            }
            set
            {
                SetEditorProperty("EnableAutoIndent", value);
                this.OnChangedSetting();
            }
        }

        public bool EnableAutoComplete
        {
            get
            {
                return (bool)GetEditorProperty("EnableAutoComplete");
            }
            set
            {
                SetEditorProperty("EnableAutoComplete", value);
                this.OnChangedSetting();
            }
        }

        public bool EnableAutoSave
        {
            get
            {
                return (bool)GetEditorProperty("EnableAutoSave");
            }
            set
            {
                SetEditorProperty("EnableAutoSave", value);
                this.OnChangedSetting();
            }
        }

        public bool EnableSyntaxHilight
        {
            get
            {
                return (bool)GetEditorProperty("EnableSyntaxHilight");
            }
            set
            {
                SetEditorProperty("EnableSyntaxHilight", value);
                this.OnChangedSetting();
            }
        }

        public bool EnableGenerateFolding
        {
            get
            {
                return (bool)GetEditorProperty("EnableGenerateFolding");
            }
            set
            {
                SetEditorProperty("EnableGenerateFolding", value);
                this.OnChangedSetting();
            }
        }

        System.Text.Encoding _DefaultEncoding;
        public System.Text.Encoding DefaultEncoding
        {
            get
            {
                if (_DefaultEncoding == null)
                {
                    var webname = (string)GetEditorProperty("DefaultEncoding");
                    _DefaultEncoding = EncodeDetect.DectingEncode.GetEncodingFromWebName(webname);
                }
                return _DefaultEncoding;
            }
            set
            {
                SetEditorProperty("DefaultEncoding", value.WebName);
                _DefaultEncoding = value;
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _ForegroundColor;
        public Windows.UI.Color ForegroundColor
        {
            get
            {
                if (_ForegroundColor == null)
                {
                    var colorcode = (string)GetEditorProperty("ForegroundColor");
                    _ForegroundColor = ColorHelper.ToColor(colorcode);
                }
                return _ForegroundColor.Value;
            }
            set
            {
                SetEditorProperty("ForegroundColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _BackgroundColor;
        public Windows.UI.Color BackgroundColor
        {
            get
            {
                if (_BackgroundColor == null)
                {
                    var colorcode = (string)GetEditorProperty("BackgroundColor");
                    _BackgroundColor = ColorHelper.ToColor(colorcode);
                }
                return _BackgroundColor.Value;
            }
            set
            {
                SetEditorProperty("BackgroundColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _KeywordColor;
        public Windows.UI.Color KeywordColor
        {
            get
            {
                if(_KeywordColor == null)
                {
                    var colorcode = (string)GetEditorProperty("KeywordColor");
                    _KeywordColor = ColorHelper.ToColor(colorcode);
                }
                return _KeywordColor.Value;
            }
            set
            {
                SetEditorProperty("KeywordColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _Keyword2Color;
        public Windows.UI.Color Keyword2Color
        {
            get
            {
                if (_Keyword2Color == null)
                {
                    var colorcode = (string)GetEditorProperty("Keyword2Color");
                    _Keyword2Color = ColorHelper.ToColor(colorcode);
                }
                return _Keyword2Color.Value;
            }
            set
            {
                SetEditorProperty("Keyword2Color", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _URLColor;
        public Windows.UI.Color URLColor
        {
            get
            {
                if (_URLColor == null)
                {
                    var colorcode = (string)GetEditorProperty("URLColor");
                    _URLColor = ColorHelper.ToColor(colorcode);
                }
                return _URLColor.Value;
            }
            set
            {
                SetEditorProperty("URLColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _ControlCharColor;
        public Windows.UI.Color ControlCharColor
        {
            get
            {
                if (_ControlCharColor == null)
                {
                    var colorcode = (string)GetEditorProperty("ControlCharColor");
                    _ControlCharColor = ColorHelper.ToColor(colorcode);
                }
                return _ControlCharColor.Value;
            }
            set
            {
                SetEditorProperty("ControlCharColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _CommentColor;
        public Windows.UI.Color CommentColor
        {
            get
            {
                if (_CommentColor == null)
                {
                    var colorcode = (string)GetEditorProperty("CommentColor");
                    _CommentColor = ColorHelper.ToColor(colorcode);
                }
                return _CommentColor.Value;
            }
            set
            {
                SetEditorProperty("CommentColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _LiteralColor;
        public Windows.UI.Color LiteralColor
        {
            get
            {
                if (_LiteralColor == null)
                {
                    var colorcode = (string)GetEditorProperty("LiteralColor");
                    _LiteralColor = ColorHelper.ToColor(colorcode);
                }
                return _LiteralColor.Value;
            }
            set
            {
                SetEditorProperty("LiteralColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _UpdateAreaColor;
        public Windows.UI.Color UpdateAreaColor
        {
            get
            {
                if (_UpdateAreaColor == null)
                {
                    var colorcode = (string)GetEditorProperty("UpdateAreaColor");
                    _UpdateAreaColor = ColorHelper.ToColor(colorcode);
                }
                return _UpdateAreaColor.Value;
            }
            set
            {
                SetEditorProperty("UpdateAreaColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        Windows.UI.Color? _LineMarkerColor;
        public Windows.UI.Color LineMarkerColor
        {
            get
            {
                if (_LineMarkerColor == null)
                {
                    var colorcode = (string)GetEditorProperty("LineMarkerColor");
                    _LineMarkerColor = ColorHelper.ToColor(colorcode);
                }
                return _LineMarkerColor.Value;
            }
            set
            {
                SetEditorProperty("LineMarkerColor", value.ToHex());
                this.OnChangedSetting();
            }
        }

        public double LineEmHeight
        {
            get {
                return (double)GetEditorProperty("LineEmHeight");
            }
            set
            {
                SetEditorProperty("LineEmHeight", value);
                this.OnChangedSetting();
            }
        }

        protected virtual object GetGlobalEditorProperty(string name)
        {
            return null;
        }

        protected virtual void SetGlobalEditorProperty(string name, object value)
        {
        }


        private object GetEditorProperty(string name)
        {
            if (this.FileType == null || !this.FileType.NoInherit)
                return this.GetGlobalEditorProperty(name);

            object target = this.FileType;
            TypeInfo typeInfo = target.GetType().GetTypeInfo();
            PropertyInfo propertyInfo = typeInfo.GetDeclaredProperty(ConvertName(name));
            if (propertyInfo == null)
                return this.GetGlobalEditorProperty(name);
            else
                return propertyInfo.GetValue(target);
        }

        private void SetEditorProperty(string name, object value, bool isGlobal = true)
        {
            if (isGlobal || this.FileType == null || !this.FileType.NoInherit)
            {
                SetGlobalEditorProperty(name, value);
                return;
            }
            else
            {
                object target = this.FileType;
                TypeInfo typeInfo = target.GetType().GetTypeInfo();
                PropertyInfo propertyInfo = typeInfo.GetDeclaredProperty(ConvertName(name));
                if (propertyInfo == null)
                    SetGlobalEditorProperty(name, value);
                else
                    propertyInfo.SetValue(target, value);
            }
        }

        private string ConvertName(string name)
        {
            //一部の設定だけFileTypeと食い違っているので修正する
            if (name == "TabChar")
                return "TabCharCount";
            return name;
        }

        /// <summary>
        /// ファイルタイプをセットした場合、一部の設定はファイルタイプに応じた設定を返します
        /// </summary>
        public FileType FileType
        {
            get;
            set;
        }

        public void OnChangedSetting([CallerMemberName] string caller = "")
        {
            if (ChangedSetting != null)
                ChangedSetting(this, null);
            var propertyChangedArgs = new PropertyChangedEventArgs(caller);
            if (PropertyChanged != null)
                PropertyChanged(this, propertyChangedArgs);
            WeakReferenceMessenger.Default.Send(propertyChangedArgs);
        }

        public event EventHandler ChangedSetting;
        public event PropertyChangedEventHandler PropertyChanged;
    }

    [DataContract]
    public class FileType : INotifyPropertyChanged
    {
        [DataMember]
        ObservableCollection<string> _ExtensionCollection;
        [DataMember]
        string _DocumentTypeName, _DocumentType;
        [DataMember]
        bool _ShowTab;
        [DataMember]
        bool _ShowFullSpace;
        [DataMember]
        bool _ShowRuler;
        [DataMember]
        bool _ShowLineNumber;
        [DataMember]
        bool _ShowLineBreak;
        [DataMember]
        bool _NoInherit;
        [DataMember]
        LineBreakMethod _LineBreakMethod;
        [DataMember]
        int _LineBreakCount;
        [DataMember]
        bool _IndentBySpace;
        [DataMember]
        int _TabCharCount;
        [DataMember]
        bool _EnableAutoIndent;
        [DataMember]
        bool _EnableAutoComplete;
        [DataMember]
        bool _EnableSyntaxHilight;
        [DataMember]
        bool _EnableGenerateFolding;

        //WPF版とメトロ版で仕様が違う
#if WPF
        public string Extension
        {
            get;
            set;
        }
#else
        public ObservableCollection<string> ExtensionCollection
        {
            get
            {
                return this._ExtensionCollection;
            }
        }
#endif

        public string DocumentTypeName
        {
            get
            {
                return _DocumentTypeName;
            }
            set
            {
                _DocumentTypeName = value;
            }
        }

        public string DocumentType
        {
            get
            {
                return this._DocumentType;
            }
            set
            {
                this._DocumentType = value;
                this.OnPropertyChanged();
            }
        }

        public bool NoInherit
        {
            get
            {
                return this._NoInherit;
            }
            set
            {
                this._NoInherit = value;
                this.OnPropertyChanged();
            }
        }

        public bool IndentBySpace
        {
            get
            {
                return _IndentBySpace;
            }
            set
            {
                _IndentBySpace = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShowTab
        {
            get
            {
                return this._ShowTab;
            }
            set
            {
                this._ShowTab = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShowFullSpace
        {
            get
            {
                return this._ShowFullSpace;
            }
            set
            {
                this._ShowFullSpace = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShowLineBreak
        {
            get
            {
                return this._ShowLineBreak;
            }
            set
            {
                this._ShowLineBreak = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShowRuler
        {
            get
            {
                return this._ShowRuler;
            }
            set
            {
                this._ShowRuler = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShowLineNumber
        {
            get
            {
                return this._ShowLineNumber;
            }
            set
            {
                this._ShowLineNumber = value;
                this.OnPropertyChanged();
            }
        }

        public LineBreakMethod LineBreakMethod
        {
            get
            {
                return this._LineBreakMethod;
            }
            set
            {
                this._LineBreakMethod = value;
                this.OnPropertyChanged();
            }
        }

        public int LineBreakCount
        {
            get
            {
                return this._LineBreakCount;
            }
            set
            {
                this._LineBreakCount = value;
                this.OnPropertyChanged();
            }
        }

        public int TabCharCount
        {
            get
            {
                return this._TabCharCount;
            }
            set
            {
                this._TabCharCount = value;
                this.OnPropertyChanged();
            }
        }

        public bool EnableAutoIndent
        {
            get
            {
                return this._EnableAutoIndent;
            }
            set
            {
                this._EnableAutoIndent = value;
                this.OnPropertyChanged();
            }
        }

        public bool EnableAutoComplete
        {
            get
            {
                return this._EnableAutoComplete;
            }
            set
            {
                this._EnableAutoComplete = value;
                this.OnPropertyChanged();
            }
        }

        public bool EnableSyntaxHilight
        {
            get
            {
                return this._EnableSyntaxHilight;
            }
            set
            {
                this._EnableSyntaxHilight = value;
                this.OnPropertyChanged();
            }
        }

        public bool EnableGenerateFolding
        {
            get
            {
                return this._EnableGenerateFolding;
            }
            set
            {
                this._EnableGenerateFolding = value;
                this.OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public FileType()
            : this(string.Empty, string.Empty)
        {
        }

        public FileType(string name, string doctype)
        {
            this._ExtensionCollection = new ObservableCollection<string>();
            this._DocumentTypeName = name;
            this._DocumentType = doctype;
            SetDefalutValue();
        }

        [OnDeserializing]
        private void OnDeserializingObject(StreamingContext sc)
        {
            SetDefalutValue();
        }

        private void SetDefalutValue()
        {
            this._NoInherit = false;
            this._ShowFullSpace = false;
            this._ShowLineNumber = false;
            this._ShowRuler = false;
            this._ShowTab = false;
            this._IndentBySpace = false;
            this._LineBreakMethod = FooEditEngine.LineBreakMethod.None;
            this._LineBreakCount = 80;
            this._TabCharCount = 4;
            this._EnableAutoIndent = false;
            this._EnableAutoComplete = false;
            this._EnableSyntaxHilight = true;
            this._EnableGenerateFolding = true;
        }

        public void OnPropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }
    }
}
