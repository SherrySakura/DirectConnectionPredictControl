using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DirectConnectionPredictControl.CommenTool;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// SingleChart.xaml 的交互逻辑
    /// </summary>
    public partial class SingleChart : Window
    {

        public event closeWindowHandler CloseWindowEvent;
        public ChartValues<Point> speedAx1 = new ChartValues<Point>();
        private ChartValues<Point> refSpeedAx1 = new ChartValues<Point>();
        private double x;

        public SingleChart()
        {
            InitializeComponent();
            var maper = Mappers.Xy<Point>().X(model => model.X).Y(model => model.Y);
            Charting.For<Point>(maper);
            x = 0;
            DataContext = this;
            speedAX1.Values = speedAx1;
            Task.Factory.StartNew(Run);
        }

        #region busniss methods

        private void Run()
        {
            while (true)
            {
                Thread.Sleep(100);
                speedAx1.Add(new Point(x, 60));
                //speedAX1.Values.Add(new Point(x, 60)); 
                x = x + 0.1;
            }
            

        }

        //public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        //{
        //    x = x + 1;
        //    speedAx1.Add(new Point(x, (container_1.SpeedA1Shaft1 + container_1.SpeedA1Shaft2) / 2));
        //}

        #endregion

        #region window methods
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ChartWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3;
            this.Height = y1 * 4 / 5;
        }

        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximunBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        private void singleChart_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "single");
        }
    }
}
