#if METRO || WPF
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FooEditor
{
    class FindFlyoutViewModel : ViewModelBase
    {
        IFindView findView;

        public FindFlyoutViewModel(IFindView view)
        {
            this.findView = view;
        }

        string _FindPattern;
        public string FindPattern
        {
            get
            {
                return this._FindPattern;
            }
            set
            {
                this._FindPattern = value;
                this.OnPropertyChanged();
            }
        }

        string _SelectedFindPattern;
        public string SelectedFindPattern
        {
            get
            {
                return this._SelectedFindPattern;
            }
            set
            {
                this._SelectedFindPattern = value;
                this.FindPattern = value;
                this.OnPropertyChanged();
            }
        }

        ObservableCollection<string> _FindHistroy;
        public ObservableCollection<string> FindHistroy
        {
            get
            {
                return this._FindHistroy;
            }
            set
            {
                this._FindHistroy = value;
                this.OnPropertyChanged();
            }
        }

        string _ReplacePattern;
        public string ReplacePattern
        {
            get
            {
                return this._ReplacePattern;
            }
            set
            {
                this._ReplacePattern = value;
                this.OnPropertyChanged();
            }
        }

        bool _UseRegEx;
        public bool UseRegEx
        {
            get
            {
                return this._UseRegEx;
            }
            set
            {
                this._UseRegEx = value;
                this.findView.Reset();
                this.OnPropertyChanged();
            }
        }

        bool _RestrictSearch;
        public bool RestrictSearch
        {
            get
            {
                return this._RestrictSearch;
            }
            set
            {
                this._RestrictSearch = value;
                this.findView.Reset();
                this.OnPropertyChanged();
            }
        }

        bool _UseGroup;
        public bool UseGroup
        {
            get
            {
                return this._UseGroup;
            }
            set
            {
                this._UseGroup = value;
                this.OnPropertyChanged();
            }
        }

        bool _AllDocuments;
        public bool AllDocuments
        {
            get
            {
                return this._AllDocuments;
            }
            set
            {
                this._AllDocuments = value;
                this.OnPropertyChanged();
            }
        }

        string _Result;
        public string Result
        {
            get
            {
                return this._Result;
            }
            set
            {
                this._Result = value;
                this.OnPropertyChanged();
            }
        }

        public DelegateCommand<object> FindNextCommand
        {
            get
            {
                return new DelegateCommand<object>((s) =>
                {
                    this.Result = string.Empty;
                    try
                    {
                        this.AddFindHistory(this.FindPattern);

                        RegexOptions opt = this.RestrictSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
                        this.findView.FindNext(this.FindPattern, this.UseRegEx, opt);
                    }
                    catch (Exception e)
                    {
                        this.Result = e.Message;
                    }
                });
            }
        }

        public DelegateCommand<object> ReplaceNextCommand
        {
            get
            {
                return new DelegateCommand<object>((s) =>
                {
                    this.Result = string.Empty;
                    try
                    {
                        this.findView.Replace(this.ReplacePattern, this.UseGroup);
                        RegexOptions opt = this.RestrictSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
                        this.findView.FindNext(this.FindPattern, this.UseRegEx, opt);
                    }
                    catch (Exception e)
                    {
                        this.Result = e.Message;
                    }
                });
            }
        }
        
        public DelegateCommand<object> ReplaceAllCommand
        {
            get
            {
                return new DelegateCommand<object>((s) => {
                    this.Result = string.Empty;
                    try
                    {
                        this.AddFindHistory(this.FindPattern);

                        RegexOptions opt = this.RestrictSearch ? RegexOptions.None : RegexOptions.IgnoreCase;
                        this.findView.ReplaceAll(this.FindPattern, this.ReplacePattern, this.UseGroup, this.UseRegEx, opt);
                    }
                    catch (Exception e)
                    {
                        this.Result = e.Message;
                    }
                });
            }
        }

        void AddFindHistory(string pattern)
        {
            if (this.FindHistroy == null)
                return;
            if (!this.FindHistroy.Contains(this.FindPattern))
                this.FindHistroy.Add(this.FindPattern);
        }
    }
    interface IFindView
    {
        void FindNext(string pattern, bool useregex, RegexOptions opt);
        void Replace(string newpattern, bool usegroup);
        void ReplaceAll(string pattern, string newpattern, bool usegroup, bool useregex, RegexOptions opt);
        void Reset();
    }
}
#endif