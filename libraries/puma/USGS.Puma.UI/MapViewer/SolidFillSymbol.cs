using System;
using System.Collections.Generic;
using System.Text;

namespace USGS.Puma.UI.MapViewer
{
    public class SolidFillSymbol : ISolidFillSymbol
    {
        #region Static Methods
        public static ISymbolDrawingTool CreateDrawingTool(ISolidFillSymbol symbol)
        {
            ISymbolDrawingTool tool = LineSymbol.CreateDrawingTool(symbol.Outline);
            tool.FillBrush = new System.Drawing.SolidBrush(symbol.Color);
            return tool;
        }
        #endregion

        #region Fields
        private ILineSymbol _Outline;
        private bool _EnableOutline;
        private bool _OneColorForFillAndOutline;
        private bool _Filled = true;
        #endregion

        #region Constructors
        public SolidFillSymbol()
        {
            _Color = System.Drawing.Color.Black;
            _Outline = new LineSymbol();
            _EnableOutline = false;
            _Filled = true;
            OneColorForFillAndOutline = false;

        }
        public SolidFillSymbol(System.Drawing.Color fillColor) : this()
        {
            Color = fillColor;
        }
        #endregion

        #region IFillSymbol Members

        public bool Filled
        {
            get { return _Filled; }
            set { _Filled = value; }
        }

        public ILineSymbol Outline
        {
            get
            {
                return _Outline;
            }
        }

        public bool EnableOutline
        {
            get
            {
                return _EnableOutline;
            }
            set
            {
                _EnableOutline = value;
            }
        }

        public bool OneColorForFillAndOutline
        {
            get
            {
                return _OneColorForFillAndOutline;
            }
            set
            {
                _OneColorForFillAndOutline = value;
                if (_OneColorForFillAndOutline)
                    _Outline.Color = _Color;
            }
        }

        #endregion

        #region ISymbol Members
        private System.Drawing.Color _Color;
        public System.Drawing.Color Color
        {
            get
            {
                if (_OneColorForFillAndOutline)
                    _Color = _Outline.Color;
                return _Color;
            }
            set
            {
                _Color = value;
                if (_OneColorForFillAndOutline)
                    _Outline.Color = _Color;
            }
        }
        #endregion

        #region IFillSymbol Members

        #endregion
    }
}
