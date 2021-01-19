#if METRO || WPF
using System;
#if WPF
using FooEditEngine.WPF;
#endif
#if METRO
using FooEditEngine.Metro;
#endif

namespace FooEditor
{
    class LineJumpViewModel : ViewModelBase
    {
        int _ToRow, _MaxRow;
        string _Result;
        ILineJumpView lineJumpView;
        public LineJumpViewModel(ILineJumpView view)
        {
            this.lineJumpView = view;
            this.ToRow = view.CaretPostionRow + 1;
            this.MaxRow = view.AvailableMaxRow;
        }
        public int ToRow
        {
            get
            {
                return this._ToRow;
            }
            set
            {
                this._ToRow = value;
                this.OnPropertyChanged();
            }
        }
        public int MaxRow
        {
            get
            {
                return this._MaxRow;
            }
            set
            {
                this._MaxRow = value;
                this.OnPropertyChanged();
            }
        }
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
        public bool JumpCaretCommand()
        {
            this.Result = string.Empty;
            if (this._ToRow <= 0 || this._ToRow > this._MaxRow)
                return true;
            this.lineJumpView.JumpCaret(this._ToRow - 1);
            return false;
        }
    }
    interface ILineJumpView
    {
        int CaretPostionRow { get; }
        int AvailableMaxRow { get; }
        void JumpCaret(int row);
    }
}
#endif