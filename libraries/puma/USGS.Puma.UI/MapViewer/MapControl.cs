using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace USGS.Puma.UI.MapViewer
{
    public enum MapControlCursor
    {
        Pointer = 0,
        ReCenter = 1,
        ZoomIn = 2,
        ZoomOut = 3,
        EditSelect = 4,
        EditSelectVertices = 5,
        VertexSelected = 6
    }

    public class MapControl : System.Windows.Forms.Panel, IMap
    {
        #region Private Fields
        private Map _Map;
        private RendererHelper _RH = null;
        private System.Drawing.Image _Image = null;
        private bool _GenerateImage = true;
        private GeometryLayer _TrackingLayer = null;
        private GeometryLayer _GeometryLayer = null;
        #endregion

        #region Public Static Methods
        public static Cursor CreateCursor(MapControlCursor tool)
        {
            System.IO.MemoryStream stream = null;
            switch (tool)
            {
                case MapControlCursor.Pointer:
                    return System.Windows.Forms.Cursors.Default;
                case MapControlCursor.ReCenter:
                    stream = new System.IO.MemoryStream(USGS.Puma.UI.Properties.Resources.ReCenterCur);
                    return new Cursor(stream);
                case MapControlCursor.ZoomIn:
                    stream = new System.IO.MemoryStream(USGS.Puma.UI.Properties.Resources.ZoomInCur);
                    return new Cursor(stream);
                case MapControlCursor.ZoomOut:
                    stream = new System.IO.MemoryStream(USGS.Puma.UI.Properties.Resources.ZoomOutCur);
                    return new Cursor(stream);
                case MapControlCursor.EditSelect:
                    stream = new System.IO.MemoryStream(USGS.Puma.UI.Properties.Resources.EditSelectCur2);
                    return new Cursor(stream);
                case MapControlCursor.EditSelectVertices:
                    stream = new System.IO.MemoryStream(USGS.Puma.UI.Properties.Resources.EditSelectVertex);
                    return new Cursor(stream);
                case MapControlCursor.VertexSelected:
                    stream = new System.IO.MemoryStream(USGS.Puma.UI.Properties.Resources.VertexSelected2);
                    return new Cursor(stream);
                default:
                    throw new ArgumentException();
            }
        }

        #endregion


        #region Events and EventInitiators
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MapControlRefreshCompletedArgs> RefreshCompleted;
        protected virtual void OnMapControlRefreshCompletedArgs(MapControlRefreshCompletedArgs e)
        {
            EventHandler<MapControlRefreshCompletedArgs> handler = RefreshCompleted;
            if (handler != null)
            {
                RefreshCompleted(this, e);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler MapExtentChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnMapExtentChanged(EventArgs e)
        {
            EventHandler handler = MapExtentChanged;
            if (handler != null)
            {
                MapExtentChanged(this, e);
            }

        }

        #endregion

        #region Constructors
        public MapControl() : base()
        {
            _Map = new Map();
            //_Map.LayerVisibilityChanged += new EventHandler<LayerEventArgs>(_Map_LayerVisibilityChanged);

            _RH = new RendererHelper();
            this.BorderStyle = BorderStyle.None;
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            _Map.MapBackgroundColor = this.BackColor;
            this.ResizeRedraw = true;
        }

        //void _Map_LayerVisibilityChanged(object sender, LayerEventArgs e)
        //{
        //    OnLayerVisibilityChanged(e);
        //}
        public MapControl(IMap map)
            : base()
        {
            InitializeMap(map);
        }
        #endregion

        #region Public Events
        #endregion

        #region Overridden Panel Methods
        protected override void OnPaint(PaintEventArgs pe)
        {

            // Render map to image and draw image
            _Map.ViewportSize = this.ClientSize;

            if (_GenerateImage)
            {
                if (_Image != null)
                { _Image.Dispose(); }
                _Image = this.RenderAsImage();
            }
            if (_Image != null)
            { 
                pe.Graphics.DrawImage(_Image, 0f, 0f);
                
                if (GeometryLayer != null)
                {
                    if (GeometryLayer.Count > 0)
                    {
                        for (int i = 0; i < GeometryLayer.Count; i++)
                        {
                            if (GeometryLayer[i].Geometry is GeoAPI.Geometries.IGeometryCollection)
                            {
                                GeoAPI.Geometries.IGeometryCollection list = GeometryLayer[i].Geometry as GeoAPI.Geometries.IGeometryCollection;
                                for (int n = 0; n < list.Count; n++)
                                {
                                    DrawGeometrySymbolPair(list[n], GeometryLayer[i].Symbol, pe.Graphics);
                                }
                            }
                            else
                            {
                                DrawGeometrySymbolPair(GeometryLayer[i].Geometry, GeometryLayer[i].Symbol, pe.Graphics);
                            }
                        }
                    }
                }


                if (_TrackingLayer != null)
                {
                    if (_TrackingLayer.Count > 0)
                    {
                        for (int i = 0; i < _TrackingLayer.Count; i++)
                        {
                            DrawGeometrySymbolPair(_TrackingLayer[i].Geometry, _TrackingLayer[i].Symbol, pe.Graphics);
                        }
                    }
                }

            }

            // Execute the Panel OnPaint method
            base.OnPaint(pe);

            // Raise the RefreshCompleted event
            OnMapControlRefreshCompletedArgs(new MapControlRefreshCompletedArgs());

        }

        private void DrawGeometrySymbolPair(GeoAPI.Geometries.IGeometry geometry, ISymbol symbol, Graphics graphics)
        {
            ISymbolDrawingTool drawingTool = null;

            if (geometry is GeoAPI.Geometries.ILineString)
            {
                if (symbol != null)
                {
                    drawingTool = RendererHelper.CreateDrawingTool(symbol);
                }
                else
                {
                    ILineSymbol lineSymbol = new LineSymbol();
                    drawingTool = RendererHelper.CreateDrawingTool(lineSymbol);
                }

                _Map.GetViewport().DrawLineString(geometry.Coordinates, graphics, drawingTool.Pen);
                (drawingTool as IDisposable).Dispose();
                drawingTool = null;
            }
            else if (geometry is GeoAPI.Geometries.IMultiLineString)
            {
                if (symbol != null)
                {
                    drawingTool = RendererHelper.CreateDrawingTool(symbol);
                }
                else
                {
                    ILineSymbol lineSymbol = new LineSymbol();
                    drawingTool = RendererHelper.CreateDrawingTool(lineSymbol);
                }

                GeoAPI.Geometries.IMultiLineString mLine = geometry as GeoAPI.Geometries.IMultiLineString;
                _Map.GetViewport().DrawLineString(mLine[0].Coordinates, graphics, drawingTool.Pen);
                (drawingTool as IDisposable).Dispose();
                drawingTool = null;
            }
            else if (geometry is GeoAPI.Geometries.IPolygon)
            {
                if (symbol != null)
                {
                    drawingTool = RendererHelper.CreateDrawingTool(symbol);
                }
                else
                {
                    ISolidFillSymbol fillSymbol = new SolidFillSymbol();
                    fillSymbol.Filled = false;
                    fillSymbol.EnableOutline = true;
                    drawingTool = RendererHelper.CreateDrawingTool(fillSymbol);
                }

                _Map.GetViewport().DrawPolygon(geometry as GeoAPI.Geometries.IPolygon, graphics, drawingTool.Pen, drawingTool.FillBrush);
                (drawingTool as IDisposable).Dispose();
                drawingTool = null;
            }
            else if (geometry is GeoAPI.Geometries.IPoint)
            {
                IPointSymbol pointSymbol = null;
                if (symbol != null)
                {
                    drawingTool = RendererHelper.CreateDrawingTool(symbol);
                }
                else
                {
                    pointSymbol = new SimplePointSymbol();
                    drawingTool = RendererHelper.CreateDrawingTool(pointSymbol);
                }

                pointSymbol = drawingTool.Symbol as IPointSymbol;
                switch (pointSymbol.SymbolType)
                {
                    case PointSymbolTypes.Circle:
                        _Map.GetViewport().DrawCirclePoint(geometry as GeoAPI.Geometries.IPoint, graphics, drawingTool.Pen, drawingTool.FillBrush, pointSymbol.Size);
                        break;
                    case PointSymbolTypes.Square:
                        _Map.GetViewport().DrawSquarePoint(geometry as GeoAPI.Geometries.IPoint, graphics, drawingTool.Pen, drawingTool.FillBrush, pointSymbol.Size);
                        break;
                    default:
                        break;
                }
                (drawingTool as IDisposable).Dispose();
                drawingTool = null;
            }

        }

        //protected override void OnPaint(PaintEventArgs pe)
        //{

        //    // Render map to image and draw image
        //    _Map.ViewportSize = this.ClientSize;

        //    if (_GenerateImage)
        //    {
        //        if (_Image != null)
        //        { _Image.Dispose(); }
        //        _Image = this.RenderAsImage();
        //    }
        //    if (_Image != null)
        //    {
        //        pe.Graphics.DrawImage(_Image, 0f, 0f);

        //        if (_HighlightGeometry != null)
        //        {
        //            if (_HighlightGeometry is GeoAPI.Geometries.ILineString)
        //            {
        //                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 3.0f);
        //                _Map.GetViewport().DrawLineString(_HighlightGeometry.Coordinates, pe.Graphics, pen);
        //                pen.Dispose();
        //            }
        //            else if (_HighlightGeometry is GeoAPI.Geometries.IMultiLineString)
        //            {
        //                GeoAPI.Geometries.IMultiLineString mLine = _HighlightGeometry as GeoAPI.Geometries.IMultiLineString;
        //                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 3.0f);
        //                _Map.GetViewport().DrawLineString(mLine[0].Coordinates, pe.Graphics, pen);
        //                pen.Dispose();

        //            }
        //            else if (_HighlightGeometry is GeoAPI.Geometries.IPolygon)
        //            {
        //                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Cyan, 3.0f);
        //                _Map.GetViewport().DrawPolygon(_HighlightGeometry as GeoAPI.Geometries.IPolygon, pe.Graphics, pen, null);
        //                pen.Dispose();

        //            }
        //        }

        //        if (_TrackingGeometry != null)
        //        {
        //            if (_TrackingGeometry is GeoAPI.Geometries.ILineString)
        //            {
        //                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 1.0f);
        //                _Map.GetViewport().DrawLineString(_TrackingGeometry.Coordinates, pe.Graphics, pen);
        //                pen.Dispose();
        //            }
        //            else if (_TrackingGeometry is GeoAPI.Geometries.IPolygon)
        //            {
        //                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, 1.0f);
        //                _Map.GetViewport().DrawPolygon(_TrackingGeometry as GeoAPI.Geometries.IPolygon, pe.Graphics, pen, null);
        //                pen.Dispose();

        //            }

        //        }

        //    }

        //    // Execute the Panel OnPaint method
        //    base.OnPaint(pe);

        //    // Raise the RefreshCompleted event
        //    OnMapControlRefreshCompletedArgs(new MapControlRefreshCompletedArgs());

        //}


        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                _Map.MapBackgroundColor = base.BackColor;
            }
        }
        #endregion

        #region IMap Members
        [Browsable(false)]
        public Color MapBackgroundColor
        {
            get
            {
                return _Map.MapBackgroundColor;
            }
            set
            {
                _Map.MapBackgroundColor = value;
                base.BackColor = _Map.MapBackgroundColor;
                this.Refresh();
            }
        }

        [Browsable(false)]
        public Size ViewportSize
        {
            get
            {
                if (this.ClientSize != _Map.ViewportSize)
                { _Map.ViewportSize = this.ClientSize; }
                return _Map.ViewportSize;
            }
            set
            {
                // do nothing here
            }
        }

        [Browsable(false)]
        public GeoAPI.Geometries.IEnvelope MapExtent
        {
            get
            {
                return _Map.MapExtent;
            }
            set
            {
                _Map.MapExtent = value;
                this.Refresh();
                OnMapExtentChanged(new EventArgs());
            }
        }

        [Browsable(false)]
        public GeoAPI.Geometries.IEnvelope ViewportExtent
        {
            get { return _Map.ViewportExtent; }
        }

        [Browsable(false)]
        public GeoAPI.Geometries.ICoordinate Center
        {
            get
            {
                return _Map.Center;
            }
            set
            {
                _Map.Center = value;
                this.Refresh();
            }
        }

        public void SetViewport(Size size, GeoAPI.Geometries.IEnvelope targetExtent)
        {
            _Map.SetViewport(size, targetExtent);
            this.Refresh();
        }
        public void SetExtent(GeoAPI.Geometries.ICoordinate center, double width, double height)
        {
            _Map.SetExtent(center, width, height);
            this.Refresh();
            OnMapExtentChanged(new EventArgs());
        }
        public void SetExtent(GeoAPI.Geometries.ICoordinate center, double length)
        {
            _Map.SetExtent(center, length);
            this.Refresh();
            OnMapExtentChanged(new EventArgs());
        }
        public void SetExtent(double minX, double maxX, double minY, double maxY)
        {
            _Map.SetExtent(minX, maxX, minY, maxY);
            this.Refresh();
            OnMapExtentChanged(new EventArgs());
        }
        public void SizeToFullExtent()
        {
            _Map.SizeToFullExtent();
            this.Refresh();
            OnMapExtentChanged(new EventArgs());
        }
        public GeoAPI.Geometries.ICoordinate ToMapPoint(int x, int y)
        {
            return _Map.ToMapPoint(x, y);
        }
        public void Zoom(double factor)
        {
            _Map.Zoom(factor);
            this.Refresh();
            OnMapExtentChanged(new EventArgs());
        }
        public void Zoom(double factor, double x, double y)
        {
            _Map.Zoom(factor, x, y);
            this.Refresh();
            OnMapExtentChanged(new EventArgs());
        }
        public Image RenderAsImage()
        {
            return _Map.RenderAsImage();
        }
        public Image RenderAsImage(GeoAPI.Geometries.IEnvelope extent)
        {
            return _Map.RenderAsImage(extent);
        }
        public Image RenderAsImage(Size size)
        {
            return _Map.RenderAsImage(size);
        }
        public Image RenderAsImage(Size size, GeoAPI.Geometries.IEnvelope extent)
        {
            return _Map.RenderAsImage(size, extent);
        }
        #endregion

        #region IGraphicsLayers Members
        public void ClearLayers()
        {
            _Map.ClearLayers();
        }
        public void AddLayer(GraphicLayer layer)
        {
            _Map.AddLayer(layer);
        }
        public void RemoveLayer(int index)
        {
            _Map.RemoveLayer(index);
        }
        public void MoveToTop(int fromIndex)
        {
            _Map.MoveToTop(fromIndex);
        }
        public void MoveToBottom(int fromIndex)
        {
            _Map.MoveToBottom(fromIndex);
        }
        public void MoveUp(int fromIndex)
        {
            _Map.MoveUp(fromIndex);
        }
        public void MoveDown(int fromIndex)
        {
            _Map.MoveDown(fromIndex);
        }
        public GraphicLayer GetLayer(int index)
        {
            return _Map.GetLayer(index);
        }

        [Browsable(false)]
        public int LayerCount
        {
            get { return _Map.LayerCount; }
        }

        [Browsable(false)]
        public GeoAPI.Geometries.IEnvelope FullExtent
        {
            get { return _Map.FullExtent; }
        }
        #endregion

        #region Public Methods
        public void InitializeMap(IMap map)
        {
            if (map == null)
            { throw new ArgumentNullException("The specified map is null."); }
            
            _Map = new Map(map, this.ClientSize);
            this.BackColor = _Map.MapBackgroundColor;

        }

        public GeometryLayer GeometryLayer
        {
            get { return _GeometryLayer; }
            set { _GeometryLayer = value; }
        }

        public void DrawGeometryLayer()
        {
            try
            {
                _GenerateImage = false;
                this.Refresh();
                _GenerateImage = true;
            }
            finally
            {
                _GenerateImage = true;
            }
        }

        public void DrawTrackingLayer(GeometryLayer trackingLayer)
        {
            DrawTrackingLayer(trackingLayer, true);
        }

        public void DrawTrackingLayer(GeometryLayer trackingLayer, bool forceTrackingLayerDraw)
        {
            if (!forceTrackingLayerDraw)
            {
                if (trackingLayer == null) return;
                if (trackingLayer.Count == 0) return;
            }

            try
            {
                _GenerateImage = false;
                _TrackingLayer = trackingLayer;
                this.Refresh();
                _GenerateImage = true;
            }
            finally
            {
                _TrackingLayer = null;
                _GenerateImage = true;
            }
        }

        public void SizeToFullExtent(double inflationFactor)
        {
            if (FullExtent == null)
            { MapExtent = new USGS.Puma.NTS.Geometries.Envelope(0, 1, 0, 1); }
            else
            {
                double delta = (inflationFactor - 1.0) * (FullExtent.MaxX - FullExtent.MinX);
                USGS.Puma.NTS.Geometries.Envelope extent = new USGS.Puma.NTS.Geometries.Envelope(FullExtent);
                extent.ExpandBy(delta);
                MapExtent = extent;
            }

        }

        //public void DrawTrackingGeometry(GeoAPI.Geometries.IGeometry trackingGeometry)
        //{
        //    try
        //    {
        //        if (trackingGeometry == null)
        //        {
        //            return;
        //        }

        //        if (trackingGeometry is GeoAPI.Geometries.IPolygon)
        //        {
        //            _GenerateImage = false;
        //            _TrackingGeometry = trackingGeometry;
        //            this.Refresh();
        //        }
        //        else if (trackingGeometry is GeoAPI.Geometries.IEnvelope)
        //        {
        //            _GenerateImage = false;
        //            _TrackingGeometry = trackingGeometry;
        //            this.Refresh();
        //        }
        //        else if ((trackingGeometry is GeoAPI.Geometries.ILineString) || (trackingGeometry is GeoAPI.Geometries.IMultiLineString))
        //        {
        //            _GenerateImage = false;
        //            _TrackingGeometry = trackingGeometry;
        //            this.Refresh();
        //        }

        //        _GenerateImage = true;

        //    }
        //    finally
        //    {
        //        _GenerateImage = true;
        //        _TrackingGeometry = null;
        //    }

        //}

        //public void Refresh(bool raiseRefreshCompletedEvent)
        //{
        //    this.Refresh();
        //    if (raiseRefreshCompletedEvent)
        //    { OnMapControlRefreshCompletedArgs(new MapControlRefreshCompletedArgs()); }
        //}
        #endregion



        #region Protected and Private Methods
        #endregion
    }

    public class MapControlRefreshCompletedArgs : EventArgs
    {
        public MapControlRefreshCompletedArgs()
            : base()
        { }
    }

}
