using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Symbology.SceneSymbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System.Timers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

/* ==============================================================================
* 功能描述：SceneViewModel  
* 创 建 者：LY
* 创建日期：2015/11/4 16:33:25
* 说明：DEMO的UI控制逻辑，对应于MVVM模式中的VIEWMODEL
* ==============================================================================*/
namespace LYGISDEMO
{
	/// <summary>
	///	日期：2015/11/4 16:33:25
    /// SceneViewModel
    /// </summary>
    class SceneViewModel:INotifyPropertyChanged
    {
        public SceneViewModel()
        {
            this.scene = App.Current.Resources["IncidentScene"] as Scene;
            Layerlegendtemplate = App.Current.Resources["layerlegendtemplate"] as HierarchicalDataTemplate;
            ToDefaultViewCommand = new DelegateCommand(ToDefaultView);
            NavigateStartCommand = new DelegateCommand(NavigateStart);
            NavigateStopCommand = new DelegateCommand(NavigateStop);
            LoadedCommand = new DelegateCommand(Loaded);
            CameraChangedCommand = new DelegateCommand(CameraChanged);
            LoadPointLayerCommand = new DelegateCommand(LoadPointLayer);
            Load3DPointLayerCommand = new DelegateCommand(Load3DPointLayer);
            LoadLineLayerCommand = new DelegateCommand(LoadLineLayer);
            Load3DLineLayerCommand = new DelegateCommand(Load3DLineLayer);
            LoadPolygonLayerCommand = new DelegateCommand(LoadPolygonLayer);
            Load3DPolygonLayerCommand = new DelegateCommand(Load3DPolygonLayer);
            MouseDoubleClickCommand = new DelegateCommand(MouseDoubleClick);
            
            MouseLeftButtonDownCommand = new DelegateCommand(MouseLeftButtonDown);
            MouseRightButtonUpCommand = new DelegateCommand(MouseRightButtonUp);

            DrawPointCommand = new DelegateCommand(DrawPoint);
            DrawLineCommand = new DelegateCommand(DrawLine);
            DrawPolygonCommand = new DelegateCommand(DrawPolygon);

           
            OnPointWinCancleCommand= new DelegateCommand(OnPointWinCancle);
            OnPointWinOKCommand = new DelegateCommand(OnPointWinOK);

            OnLineWinCancleCommand = new DelegateCommand(OnLineWinCancle);
            OnLineWinOKCommand = new DelegateCommand(OnLineWinOK);

            OnPolygonWinCancleCommand = new DelegateCommand(OnPolygonWinCancle);
            OnPolygonWinOKCommand = new DelegateCommand(OnPolygonWinOK);

            ClearGraphicsCommand= new DelegateCommand(ClearGraphics);

            //this.drawPointGraphic = App.Current.Resources["res_drawpoint"] as Graphic;
        }
        private SceneView tagView;//将UI中的sceneView在初始化事件中加载到ViewModel中方便操作调用

        public event PropertyChangedEventHandler PropertyChanged;
        public DelegateCommand ToDefaultViewCommand { get; set; }
        public DelegateCommand NavigateStartCommand { get; set; }
        public DelegateCommand NavigateStopCommand { get; set; }
        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand CameraChangedCommand { get; set; }
        public DelegateCommand LoadPointLayerCommand { get; set; }
        public DelegateCommand Load3DPointLayerCommand { get; set; }
        public DelegateCommand LoadLineLayerCommand { get; set; }
        public DelegateCommand Load3DLineLayerCommand { get; set; }
        public DelegateCommand LoadPolygonLayerCommand { get; set; }
        public DelegateCommand Load3DPolygonLayerCommand { get; set; }
        public DelegateCommand MouseDoubleClickCommand { get; set; }
        public DelegateCommand MouseMoveCommand { get; set; }
        public DelegateCommand MouseLeftButtonDownCommand { get; set; }
        public DelegateCommand MouseRightButtonUpCommand { get; set; }
        public DelegateCommand DrawPointCommand { get; set; }
        public DelegateCommand DrawLineCommand { get; set; }
        public DelegateCommand DrawPolygonCommand { get; set; }

       
        public DelegateCommand OnPointWinCancleCommand { get; set; }
        public DelegateCommand OnPointWinOKCommand { get; set; }

        public DelegateCommand OnLineWinCancleCommand { get; set; }
        public DelegateCommand OnLineWinOKCommand { get; set; }

        public DelegateCommand OnPolygonWinCancleCommand { get; set; }
        public DelegateCommand OnPolygonWinOKCommand { get; set; }

        public DelegateCommand ClearGraphicsCommand { get; set; }

        public Timer NagTimer = new Timer(200);//0.2秒移动一下
        public Camera initCamera = new Camera(new MapPoint(116.3900, 39.4173, 30000, SpatialReferences.Wgs84), 0, 60);//默认视图，北京
        
        public GraphicsOverlay _graphicsOverlay;

       
        PointToolWin pointWin ;
        LineToolWin LineWin;
        PolygonToolWin PolygonWin;
        public bool readytodrawPoint=false; public bool pointwinshowing = false;
        public bool readytodrawLine = false;public bool linewinshowing = false;
        public bool readytodrawPolygon = false; public bool Polygonwinshowing = false;

      

        private string pointgraphictext;
        public string Pointgraphictext
        {
            get
            {
                return this.pointgraphictext;
            }
            set
            {
                this.pointgraphictext = value;
                this.RaiseNotifyPropertyChanged();
            }
        }
        private string linegraphictext;
        public string Linegraphictext
        {
            get
            {
                return this.linegraphictext;
            }
            set
            {
                this.linegraphictext = value;
                this.RaiseNotifyPropertyChanged();
            }
        }

        private string polygongraphictext;
        public string Polygongraphictext
        {
            get
            {
                return this.polygongraphictext;
            }
            set
            {
                this.polygongraphictext = value;
                this.RaiseNotifyPropertyChanged();
            }
        }

        private Graphic drawPointGraphic = new Graphic(new MapPoint(0, 0, 0));
        public Graphic DrawPointGraphic
        {
            get { return this.drawPointGraphic; }
            set { this.drawPointGraphic = value; this.RaiseNotifyPropertyChanged(); }
        }

        private double drawpointX;

        public double DrawpointX
        {
            get { return (this.drawpointX); }
            set { drawpointX = value; this.RaiseNotifyPropertyChanged(); }
        }
        private double drawpointY;
        public double DrawpointY
        {
            get { return (this.drawpointY); }
            set { drawpointY = value; this.RaiseNotifyPropertyChanged(); }
        }
        private double drawpointZ;
        public double DrawpointZ
        {
            get { return (this.drawpointZ); }
            set { drawpointZ = value; this.RaiseNotifyPropertyChanged(); }
        }

        private SolidColorBrush pointColor;
        public SolidColorBrush PointColor
        {
            get { return (this.pointColor); }
            set { pointColor = value; this.RaiseNotifyPropertyChanged(); }
        }

        private Graphic drawLineGraphic;
        public Graphic DrawLineGraphic
        {
            get { return this.drawLineGraphic; }
            set { this.drawLineGraphic = value; this.RaiseNotifyPropertyChanged(); }
        }
        private double drawLineZ;
        public double DrawLineZ
        {
            get { return (this.drawLineZ); }
            set { drawLineZ = value; this.RaiseNotifyPropertyChanged(); }
        }

      

        public Esri.ArcGISRuntime.Geometry.PointCollection  Linepointcollec=new Esri.ArcGISRuntime.Geometry.PointCollection();
        private SolidColorBrush lineColor = new SolidColorBrush();
        public SolidColorBrush LineColor
        {
            get { return (this.lineColor); }
            set { lineColor = value; this.RaiseNotifyPropertyChanged(); }
        }
        private double linewidth=10;
        public double Linewidth
        {
            get { return (this.linewidth); }
            set { linewidth = value; this.RaiseNotifyPropertyChanged(); }
        }


        private Graphic drawPolygonGraphic;
        public Graphic DrawPolygonGraphic
        {
            get { return this.drawPolygonGraphic; }
            set { this.drawPolygonGraphic = value; this.RaiseNotifyPropertyChanged(); }
        }
        private double drawPolygonZ;
        public double DrawPolygonZ
        {
            get { return (this.drawPolygonZ); }
            set { drawPolygonZ = value; this.RaiseNotifyPropertyChanged(); }
        }
        public Esri.ArcGISRuntime.Geometry.PointCollection Polygonpointcollec = new Esri.ArcGISRuntime.Geometry.PointCollection();
        private SolidColorBrush polygonColor = new SolidColorBrush();
        public SolidColorBrush PolygonColor
        {
            get { return (this.polygonColor); }
            set { polygonColor = value; this.RaiseNotifyPropertyChanged(); }
        }

        private string CameraString;
        public string CurrentCameraString
        {
            get
            {
                return this.CameraString;
            }
            set
            {
                this.CameraString = value;
                this.RaiseNotifyPropertyChanged();
            }
        }
        private Scene scene;
        public Scene IncidentScene
        {
            get { return this.scene; }
            set { this.scene = value; }
        }

        public IEnumerable<Layer> ListLayers
        {
            get { return scene.Layers; }
        }

        private HierarchicalDataTemplate Layerlegendtemplate;
        public HierarchicalDataTemplate layerlegendtemplate
        {
            get { return this.Layerlegendtemplate; }
            set { this.Layerlegendtemplate = value; }
        }
        private Graphic tagGraphic;
        public Graphic TagGraphic
        {
            get { return this.tagGraphic; }
            set { this.tagGraphic = value; this.RaiseNotifyPropertyChanged(); }
        }

      

        private void RaiseNotifyPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
            }
        }

        /// <summary>
        /// SceneView相关消息控制
        /// </summary>       
        public void Loaded(object parameter)
        {
            this.tagView = parameter as SceneView;
            ToDefaultView(parameter);
        }
        public void CameraChanged(object parameter)
        {
            CurrentCameraString=string.Format("当前相机信息：相机位置：经度{0:F3} ，纬度 {1:F3} ， 高度{2:F0} ， 仰角:{3:F2} ，倾角:{4:F0}",
                                           tagView.Camera.Location.X, tagView.Camera.Location.Y, tagView.Camera.Location.Z, tagView.Camera.Pitch,tagView.Camera.Heading);
        }
        /// <summary>
        /// 导航及默认视图操作的托管控制
        /// </summary>
        private async void ToDefaultView(object parameter)
        {
            await tagView.SetViewAsync(initCamera, 5);
        }
        private async void NavigateStart(object parameter )
        {
            Camera curCamera = new Camera(new MapPoint(0, 0, 10000000), 0, 0);//赤道旋转展示
            await tagView.SetViewAsync(curCamera, 15);

            NagTimer.Elapsed += new ElapsedEventHandler(onTimer);
            NagTimer.Start();
        }
        private void NavigateStop(object parameter)
        {
            NagTimer.Stop();
        }
        private void onTimer(object source, ElapsedEventArgs e)
        {
            Camera oldcamera = tagView.Camera;
            Camera curCamera = new Camera(new MapPoint((oldcamera.Location.X + 1), oldcamera.Location.Y, oldcamera.Location.Z), 0, 0);
            tagView.SetView(curCamera);
        }

        /// <summary>
        /// 菜单操作
        /// </summary>
          private  void LoadPointLayer(object parameter )
        {
            CsvLayer PointCsvLayer = new CsvLayer();
            PointCsvLayer.ServiceUri = "http://www.cnlightning.cn/videos/beijing0616.csv";
            PointCsvLayer.XFieldName = "LONGITUDE";
            PointCsvLayer.YFieldName = "LATITUDE";
            PointCsvLayer.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
            PointCsvLayer.SourceSpatialReference = SpatialReferences.Wgs84;
            PointCsvLayer.Renderer = App.Current.Resources["UniqueVauleRender"] as Renderer;
            PointCsvLayer.DisplayName = "点图层";
            PointCsvLayer.ID = "PointLayer"; 
            scene.Layers.Add(PointCsvLayer);
        }
        private void Load3DPointLayer(object parameter)
        {
            GraphicsLayer PointLayer = scene.Layers["PointLayer"] as GraphicsLayer;
            GraphicsLayer PointLayer3d =new GraphicsLayer();
            for (int i = 0; i < PointLayer.Graphics.Count; i++)
            {
                MapPoint point = PointLayer.Graphics.ElementAt(i).Geometry as MapPoint;
                string Zstr = PointLayer.Graphics.ElementAt(i).Attributes["HEIGHT"] as string;
                double zval = double.Parse(Zstr) * 1000;
                MapPoint tagpoint = new MapPoint(point.X, point.Y, zval);
                PointLayer3d.Graphics.Add(new Graphic(tagpoint, PointLayer.Graphics.ElementAt(i).Attributes));
            }
            PointLayer3d.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
            PointLayer3d.ShowLegend = false;
            PointLayer3d.Renderer = App.Current.Resources["3DRenderer"] as Renderer;
            PointLayer3d.DisplayName = "3D点图层";
            PointLayer3d.ID = "3DPointLayer";
            scene.Layers.Add(PointLayer3d);
        }
        private async void LoadLineLayer(object parameter)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Shapefiles (*.shp)|*.shp";
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Select Shapefile";
                string localpath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                openFileDialog.InitialDirectory = localpath;
                if (openFileDialog.ShowDialog() == true)
                {
                    await LoadLineShapefile(openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sample Error");
            }
        }
        private async Task LoadLineShapefile(string path)
        {
            ShapefileTable shapefile = await ShapefileTable.OpenAsync(path);

            var flayer = new FeatureLayer(shapefile);
            flayer.DisplayName = "线文件";
            flayer.ID = "LineLayer";
            SpatialReference sp = flayer.DefaultSpatialReference;
            scene.Layers.Add(flayer);
        }
        private void Load3DLineLayer(object parameter)
        {
            var pointcollec = new Esri.ArcGISRuntime.Geometry.PointCollection();

            pointcollec.Add(new MapPoint(116.587, 39.852, 3000));
            pointcollec.Add(new MapPoint(116.587, 39.952, 3000));
            pointcollec.Add(new MapPoint(116.687, 39.952, 3000));
            pointcollec.Add(new MapPoint(116.687, 40.052, 3000));

            var line = new Esri.ArcGISRuntime.Geometry.Polyline(pointcollec, SpatialReferences.Wgs84);

            var graphicLayer = new GraphicsLayer();
            graphicLayer.DisplayName = "3D线图层";
            graphicLayer.ShowLegend = false;
            graphicLayer.Graphics.Add(new Graphic(line));
            graphicLayer.Renderer = App.Current.Resources["LineSimpleRenderer"] as SimpleRenderer;
            graphicLayer.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
            graphicLayer.ID = "3DLineLayer";
            scene.Layers.Add(graphicLayer);
        }
        private async void LoadPolygonLayer(object parameter)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Shapefiles (*.shp)|*.shp";
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "Select Shapefile";
                string localpath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                openFileDialog.InitialDirectory = localpath;
                if (openFileDialog.ShowDialog() == true)
                {
                    await LoadPolygonShapefile(openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sample Error");
            }
        }
        private async Task LoadPolygonShapefile(string path)
        {
            ShapefileTable shapefile = await ShapefileTable.OpenAsync(path);

            var flayer = new FeatureLayer(shapefile);
            flayer.DisplayName = "面图层";
            flayer.ID = "PolygonLayer";
            SpatialReference sp = flayer.DefaultSpatialReference;
            scene.Layers.Add(flayer);
        }
        private async void Load3DPolygonLayer(object parameter)
        {
            try
            {
                var queryTask = new QueryTask(new Uri("http://119.2.29.21:6080/arcgis/rest/services/ChinaMap/MapServer/0"));
                Query query = new Query("1=1");
                query.OutFields.Add("POPU");//人口数
                var result = await queryTask.ExecuteAsync(query);

                var res = new GraphicCollection();
                foreach (var state in result.FeatureSet.Features)
                    res.Add(new Graphic(state.Geometry, state.Attributes));

                var graphicLayer = new GraphicsLayer();
                graphicLayer.DisplayName = "3D面图层";
                graphicLayer.GraphicsSource = res;
                graphicLayer.Renderer = App.Current.Resources["3DPolygonRenderer"] as Renderer;
                graphicLayer.SceneProperties.SurfacePlacement = SurfacePlacement.Relative;
                graphicLayer.ID = "3DPolygonLayer";
                scene.Layers.Add(graphicLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sample Error");
            }
            
        }
        private async void MouseDoubleClick(object parameter)
        {
            Point tagPoint = Mouse.GetPosition(tagView as FrameworkElement);
           
            GraphicsLayer layer = scene.Layers["PointLayer"] as GraphicsLayer;
            if (layer == null) return;
            var ga = await layer.HitTestAsync(tagView, tagPoint);
            if (ga != null)
            {
                TagGraphic = ga;
            }
        }

        //private async void MouseMove(object parameter)
        //{

        //    Point curPoint = Mouse.GetPosition(tagView as FrameworkElement);
        //    _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
        //    if (b_onpointmoving && b_onpoint)
        //    {

        //        var tempgraphic = drawPointGraphic;
        //        _graphicsOverlay.Graphics.Remove(drawPointGraphic);

        //        MapPoint point = tagView.ScreenToLocation(curPoint);
        //        drawPointGraphic = new Graphic(new MapPoint(point.X, point.Y, (tempgraphic.Geometry as MapPoint).Z), tempgraphic.Attributes, tempgraphic.Symbol);
        //        _graphicsOverlay.Graphics.Add(drawPointGraphic);
        //        return;
        //    }

        //    var ga = await _graphicsOverlay.HitTestAsync(tagView, curPoint);
        //    if (ga != null)
        //    {
        //        b_onpoint = true;
        //        Mouse.SetCursor(Cursors.Hand);
        //    }
        //    else
        //    {
        //        b_onpoint = false;
        //        Mouse.SetCursor(Cursors.Arrow);
        //    }
        //}
        private  void MouseLeftButtonDown(object parameter)
        {
           if(readytodrawPoint)
            {
                Point curPoint = Mouse.GetPosition(tagView as FrameworkElement);
                MapPoint tagPoint = tagView.ScreenToLocation(curPoint);
                drawpointX = tagPoint.X;drawpointY = tagPoint.Y;

                drawPointGraphic = new Graphic(tagPoint, App.Current.Resources["drawtoolpoint"] as Symbol);

                Pointgraphictext = "InputLabel";
                drawPointGraphic.Attributes["Label"] = pointgraphictext;

                pointColor = new SolidColorBrush((drawPointGraphic.Symbol as SphereMarkerSymbol).Color);
                

                _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
                _graphicsOverlay.Graphics.Add(drawPointGraphic);

                if(!pointwinshowing)
                {
                    pointWin = new PointToolWin();
                    //pointWin.Owner = tagView.Parent as Window;
                    pointwinshowing = true;
                    pointWin.Show();

                    readytodrawPoint = false;
                }
               
            }
           else if(readytodrawLine)
            {
                Point curPoint = Mouse.GetPosition(tagView as FrameworkElement);
                MapPoint tagPoint = tagView.ScreenToLocation(curPoint);

                

                _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
                if(linewinshowing)
                {
                    _graphicsOverlay.Graphics.Remove(drawLineGraphic);
                }
                linegraphictext = "Inputlabel";

                Linepointcollec.Add(new MapPoint(tagPoint.X,tagPoint.Y,drawLineZ));
                var linesym = new SimpleLineSymbol();
                lineColor = new SolidColorBrush((App.Current.Resources["drawtoolpoint"] as SphereMarkerSymbol).Color);
                linesym.Color = LineColor.Color;
                linesym.Width = linewidth; 
                 drawLineGraphic = new Graphic(new Polyline(Linepointcollec), linesym);
                _graphicsOverlay.Graphics.Add(drawLineGraphic);
                if (!linewinshowing)
                {
                    LineWin = new LineToolWin();
                    LineWin.Show();
                    linewinshowing = true;
                }
                
            }
            else if (readytodrawPolygon)
            {
                Point curPoint = Mouse.GetPosition(tagView as FrameworkElement);
                MapPoint tagPoint = tagView.ScreenToLocation(curPoint);



                _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
                if (Polygonwinshowing)
                {
                    _graphicsOverlay.Graphics.Remove(drawPolygonGraphic);
                }
                polygongraphictext = "Inputlabel";

                Polygonpointcollec.Add(new MapPoint(tagPoint.X, tagPoint.Y, drawPolygonZ));
                var Polygonsym = new SimpleFillSymbol();
                PolygonColor = new SolidColorBrush((App.Current.Resources["drawtoolpoint"] as SphereMarkerSymbol).Color);
                Polygonsym.Color = PolygonColor.Color;
                drawPolygonGraphic = new Graphic(new Polygon(Polygonpointcollec), Polygonsym);
                _graphicsOverlay.Graphics.Add(drawPolygonGraphic);
                if (!Polygonwinshowing)
                {
                    PolygonWin = new PolygonToolWin();
                    PolygonWin.Show();
                    Polygonwinshowing = true;
                }
            }

        }
        private  async void MouseRightButtonUp(object parameter)
        {
            Point curPoint = Mouse.GetPosition(tagView as FrameworkElement);
            MapPoint tagPoint = tagView.ScreenToLocation(curPoint);
            _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
            var ga = await _graphicsOverlay.HitTestAsync(tagView, curPoint);
            if (ga != null)
            {
                if(ga.Geometry.GeometryType==GeometryType.Point)
                {
                    drawPointGraphic = ga;
                    drawpointX = (drawPointGraphic.Geometry as MapPoint).X;
                    drawpointY = (drawPointGraphic.Geometry as MapPoint).Y;
                    drawpointZ = (drawPointGraphic.Geometry as MapPoint).Z;

                    if(pointwinshowing==false)
                    {
                        pointWin = new PointToolWin();
                        pointwinshowing = true;
                        pointWin.ShowDialog();
                    }
                   
                }
                if (ga.Geometry.GeometryType == GeometryType.Polyline)
                {
                    drawLineGraphic = ga;
                    linegraphictext = drawLineGraphic.Attributes["Label"] as string;
                    linewidth = (drawLineGraphic.Symbol as SimpleLineSymbol).Width;
                    Esri.ArcGISRuntime.Geometry.PointCollection tagcol = new Esri.ArcGISRuntime.Geometry.PointCollection();
                    foreach (var part in (drawLineGraphic.Geometry as Polyline).Parts)
                    {
                        for(int i=0;i<part.Count;i++)
                        {
                            var linepart = part.ElementAt(i);
                            tagcol.Add(linepart.StartPoint);
                            if(i==(part.Count-1)) tagcol.Add(linepart.EndPoint);
                            drawLineZ = linepart.StartPoint.Z;
                        }
                    }
                    Linepointcollec = tagcol;

                    if(linewinshowing==false)
                    {
                        LineWin = new LineToolWin();
                        linewinshowing = true;
                        LineWin.ShowDialog();

                    }
                    
                }
                if (ga.Geometry.GeometryType == GeometryType.Polygon)
                {
                    drawPolygonGraphic = ga;
                    polygongraphictext = drawPolygonGraphic.Attributes["Label"] as string;
                    Esri.ArcGISRuntime.Geometry.PointCollection tagcol = new Esri.ArcGISRuntime.Geometry.PointCollection();
                    foreach (var part in (drawPolygonGraphic.Geometry as Polygon).Parts)
                    {
                        for (int i = 0; i < part.Count; i++)
                        {
                            var Polygonpart = part.ElementAt(i);
                            tagcol.Add(Polygonpart.StartPoint);
                            if (i == (part.Count - 1)) tagcol.Add(Polygonpart.EndPoint);
                            drawPolygonZ = Polygonpart.StartPoint.Z;
                        }
                    }
                    Polygonpointcollec = tagcol;

                    if(Polygonwinshowing==false)
                    {
                        PolygonWin = new PolygonToolWin();
                        Polygonwinshowing = true;
                        PolygonWin.ShowDialog();
                    }                   
                }
            }
        }


        private void DrawPoint(object parameter)
        {
            readytodrawPoint = !readytodrawPoint;
            readytodrawLine=false;
            readytodrawPolygon=false;

            if (LineWin != null)
            {
                if (LineWin.IsVisible)
                {
                    LineWin.Close();
                    linewinshowing = false;
                }
            }
            if (PolygonWin != null)
            {
                if (PolygonWin.IsVisible)
                {
                    PolygonWin.Close();
                    Polygonwinshowing = false;
                }
            }
        }
        private void DrawLine(object parameter)
        {
            readytodrawPoint = false ;
            readytodrawLine = !readytodrawLine;
            readytodrawPolygon = false;
            Linepointcollec.Clear();

            if (pointWin!=null)
            {
                if (pointWin.IsVisible)
                {
                    pointwinshowing = false;
                    pointWin.Close();
                } 
            }
            if (PolygonWin!=null)
            {
                if(PolygonWin.IsVisible)
                {
                    PolygonWin.Close();
                    Polygonwinshowing = false;
                }               
            }
        }
        private void DrawPolygon(object parameter)
        {
            readytodrawPoint = false;
            readytodrawLine = false;
            readytodrawPolygon = !readytodrawPolygon;

            if (pointWin != null)
            {
                if (pointWin.IsVisible)
                {
                    pointwinshowing = false;
                    pointWin.Close();
                }
            }
            if (LineWin != null)
            {
                if (LineWin.IsVisible)
                {
                    LineWin.Close();
                    linewinshowing = false;
                }
            }
        }

        //public void PointInit(object parameter)
        //{
        //    if (drawPointGraphic == null)//新建点
        //    {
        //        _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];

        //        Viewpoint vpoint = tagView.GetCurrentViewpoint(ViewpointType.CenterAndScale);
        //        Esri.ArcGISRuntime.Geometry.Geometry tagGemotry = vpoint.TargetGeometry;
        //        MapPoint tagPoint = new MapPoint((tagGemotry.Extent.XMax + tagGemotry.Extent.XMin) / 2, (tagGemotry.Extent.YMax + tagGemotry.Extent.YMin) / 2, 0);

        //        drawPointGraphic = new Graphic(tagPoint, App.Current.Resources["drawtoolpoint"] as Symbol);

        //        Pointgraphictext = "InputLabel";
        //        drawPointGraphic.Attributes["Label"] = pointgraphictext;

        //        _graphicsOverlay.Graphics.Add(drawPointGraphic);

        //        DrawpointX = (drawPointGraphic.Geometry as MapPoint).X;
        //        DrawpointY = (drawPointGraphic.Geometry as MapPoint).Y;

        //    }
        //    else//编辑点
        //    {

        //    }


        //}
        public void OnPointWinCancle(object parameter)
        {
            pointWin.Close();
            pointwinshowing = false;
            readytodrawPoint = false;
        }
        public void OnPointWinOK(object parameter)
        {
             _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
            var tempgraphic = drawPointGraphic;
            _graphicsOverlay.Graphics.Remove(drawPointGraphic);

            SphereMarkerSymbol tagsymbol = App.Current.Resources["drawtoolpoint"] as SphereMarkerSymbol;
            pointColor=parameter as SolidColorBrush;
            tagsymbol.Color = pointColor.Color;

            drawPointGraphic = new Graphic(new MapPoint(drawpointX, drawpointY, drawpointZ), tempgraphic.Attributes, tempgraphic.Symbol);
            drawPointGraphic.Attributes["Label"] = pointgraphictext;
            _graphicsOverlay.Graphics.Add(drawPointGraphic);
            drawPointGraphic = new Graphic();

            pointwinshowing = false;
            pointWin.Close();

            return;
          
        }

        public void OnLineWinCancle(object parameter)
        {
            readytodrawLine = false;
            linewinshowing = false;
            LineWin.Close();
        }
        public void OnLineWinOK(object parameter)
        {
            readytodrawLine = false;
            _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
            var tempgraphic = drawLineGraphic;
            _graphicsOverlay.Graphics.Remove(drawLineGraphic);

            SphereMarkerSymbol tagsymbol = App.Current.Resources["drawtoolLine"] as SphereMarkerSymbol;
            LineColor = parameter as SolidColorBrush;
            var linesym = new SimpleLineSymbol();
            
            linesym.Color = LineColor.Color;
            linesym.Width = linewidth;

            var sPointcollect = new Esri.ArcGISRuntime.Geometry.PointCollection();

            foreach(MapPoint tPoint in Linepointcollec)
            {
                MapPoint sPoint = new MapPoint(tPoint.X, tPoint.Y, drawLineZ);
                sPointcollect.Add(sPoint);
            }

            drawLineGraphic = new Graphic(new Polyline(sPointcollect), linesym);
            drawLineGraphic.Attributes["Label"] = Linegraphictext;
            _graphicsOverlay.Graphics.Add(drawLineGraphic);

            drawLineZ = 0;
            Linepointcollec.Clear();
            linewinshowing = false;
            LineWin.Close();

            return;

        }

        public void OnPolygonWinCancle(object parameter)
        {
            readytodrawPolygon = false;
            Polygonwinshowing = false;
            PolygonWin.Close();
        }
        public void OnPolygonWinOK(object parameter)
        {
            readytodrawPolygon = false;
            _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
            var tempgraphic = drawPolygonGraphic;
            _graphicsOverlay.Graphics.Remove(drawPolygonGraphic);

            SphereMarkerSymbol tagsymbol = App.Current.Resources["drawtoolPolygon"] as SphereMarkerSymbol;
            PolygonColor = parameter as SolidColorBrush;
            var Polygonsym = new SimpleFillSymbol();

            Polygonsym.Color = PolygonColor.Color;

            var sPointcollect = new Esri.ArcGISRuntime.Geometry.PointCollection();

            foreach (MapPoint tPoint in Polygonpointcollec)
            {
                MapPoint sPoint = new MapPoint(tPoint.X, tPoint.Y, drawPolygonZ);
                sPointcollect.Add(sPoint);
            }

            drawPolygonGraphic = new Graphic(new Polygon(sPointcollect), Polygonsym);
            drawPolygonGraphic.Attributes["Label"] = Polygongraphictext;
            _graphicsOverlay.Graphics.Add(drawPolygonGraphic);

            drawPolygonZ = 0;
            Polygonpointcollec.Clear();
            Polygonwinshowing = false;
            PolygonWin.Close();

            return;

        }

        public void ClearGraphics(object parameter)
        {
            _graphicsOverlay = tagView.GraphicsOverlays["drawGraphicsOverlay"];
            _graphicsOverlay.Graphics.Clear();
        }
    }
}
