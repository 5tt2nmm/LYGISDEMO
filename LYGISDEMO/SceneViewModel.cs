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
using Esri.ArcGISRuntime.Tasks.Query;
using System.Timers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        public Timer NagTimer = new Timer(200);//0.2秒移动一下
        public Camera initCamera = new Camera(new MapPoint(116.3900, 39.4173, 30000, SpatialReferences.Wgs84), 0, 60);//默认视图，北京
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
            var ga = await layer.HitTestAsync(tagView, tagPoint);
            if (ga != null)
            {
                TagGraphic = ga;
            }
        }
    }
}
