using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using DirectConnectionPredictControl.IO;
using System.Windows.Media.Animation;
using System.Threading;
using DirectConnectionPredictControl.CommenTool;
using System.Diagnostics;
using System.Configuration;

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] recvData;
        private delegate void updateUI(MainDevDataContains mainDevDataContains);
        private Thread updateUIHandler;
        private Thread recvThread;
        private CanHelper canHelper;

        //数据组
        private MainDevDataContains mainDevDataContainersCar1;
        private SliverDataContainer sliverDataContainerCar2;
        private SliverDataContainer sliverDataContainerCar3;
        private SliverDataContainer sliverDataContainerCar4;
        private SliverDataContainer sliverDataContainerCar5;
        private MainDevDataContains mainDevDataContainersCar6;

        //窗口组
        private DetailWindow detailWindowCar1;
        private SlaveDetailWindow slaveDetailWindowCar2;
        private SlaveDetailWindow slaveDetailWindowCar3;
        private SlaveDetailWindow slaveDetailWindowCar4;
        private SlaveDetailWindow slaveDetailWindowCar5;
        private DetailWindow slaveDetailWindowCar6;
        private RealTimeSpeedChartWindow speedChartWindow;
        private RealTimePressureChartWindow pressureChartWindow;
        private RealTimeOtherWindow otherWindow;
        private OverviewWindow overviewWindow;
        private ChartWindow chartWindow;
        private SingleChart singleChartWindow;
        
        //测试组
        private Thread testThread;

        private UserDateTime userDateTime;
        public MainWindow()
        {
            InitializeComponent();

            //用代码调动StoryBoard
            Storyboard storyBoard = (Storyboard)MyWindow.Resources["open"];
            if (storyBoard != null)
            {
                storyBoard.Begin();
            }
            //Init();
            Test();
        }

        #region 测试用
        /// <summary>
        /// 测试用例
        /// </summary>
        private void Test()
        {
            mainDevDataContainersCar1 = new MainDevDataContains();
            sliverDataContainerCar2 = new SliverDataContainer();
            sliverDataContainerCar3 = new SliverDataContainer();
            sliverDataContainerCar4 = new SliverDataContainer();
            sliverDataContainerCar5 = new SliverDataContainer();
            mainDevDataContainersCar6 = new MainDevDataContains();
            testThread = new Thread(TestHandler);
            testThread.Start();
        }

        /// <summary>
        /// 测试线程
        /// 
        /// </summary>
        private void TestHandler()
        {
            Random random = new Random();

            while (true)
            {
                Thread.Sleep(1000);
                int speedValue = random.Next(140);
                int accSetup = random.Next(4);
                int air = random.Next(2000);
                mainDevDataContainersCar1.SpeedA1Shaft1 = speedValue;
                mainDevDataContainersCar1.SpeedA1Shaft2 = speedValue;
                mainDevDataContainersCar1.AccSetupValue = accSetup;
                sliverDataContainerCar2.SpeedShaft1 = speedValue;
                sliverDataContainerCar2.SpeedShaft2 = speedValue;
                sliverDataContainerCar2.BrakeCylinderSourcePressure = air / 2;
                sliverDataContainerCar3.BrakeCylinderSourcePressure = air / 3;
                sliverDataContainerCar4.AirSpringPressure1 = air;
                sliverDataContainerCar4.MassValue = air / 2;
                sliverDataContainerCar2.AbBrakeActive = true;
                mainDevDataContainersCar1.BrakeCmd = true;
                updateUIMethod(mainDevDataContainersCar1);
            }
        }
        #endregion

        private void Init()
        {
            mainDevDataContainersCar1 = new MainDevDataContains();
            sliverDataContainerCar2 = new SliverDataContainer();
            sliverDataContainerCar3 = new SliverDataContainer();
            sliverDataContainerCar4 = new SliverDataContainer();
            sliverDataContainerCar5 = new SliverDataContainer();
            mainDevDataContainersCar6 = new MainDevDataContains();
            userDateTime = new UserDateTime()
            {
                Year = 2018,
                Month = 3,
                Day = 10,
                Hour = 0,
                Minute = 0,
                Second = 0
            };
            canHelper = new CanHelper();
            recvThread = new Thread(RecvDataAsync);
            recvThread.IsBackground = true;
            recvThread.Start();
            updateUIHandler = new Thread(UpdateUIHandlerMethod);
            updateUIHandler.IsBackground = true;
            updateUIHandler.Start();
        }

        private void UpdateUIHandlerMethod()
        {
            updateUI update = new updateUI(updateUIMethod);
            while (true)
            {
                Thread.Sleep(50);
                update.Invoke(mainDevDataContainersCar1);
            }
        }

        /// <summary>
        /// 窗口拖动函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void OpenFile(string filePath)
        {
            string fileName;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.RestoreDirectory = true;
            ofd.Filter = ".vv(vv文件)";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = ofd.FileName;
                
            }
        }
        /// <summary>
        /// 最小化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miniumBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 最大化按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maximun_Click(object sender, RoutedEventArgs e) 
        {
            if (this.WindowState == WindowState.Maximized)
            {

                this.WindowState = WindowState.Normal;
                this.MainDashboard.dashboard.Width = 250;
                this.MainDashboard.dashboard.Height = 250;
                this.MainDashboard.speedtext.FontSize = 12;
                this.MainDashboard.speed.FontSize = 40;
                this.MainDashboard.Kmphtext.FontSize = 12;
                this.MainDashboard.dashTextStack.Margin = new Thickness(0, 0, 0, 80);
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                this.MainDashboard.dashboard.Width = 350;
                this.MainDashboard.dashboard.Height = 350;
                this.MainDashboard.speedtext.FontSize = 16;
                this.MainDashboard.speed.FontSize = 48;
                this.MainDashboard.Kmphtext.FontSize = 16;
                this.MainDashboard.dashTextStack.Margin = new Thickness(0, 0, 0, 110);
            }
           
        }

        /// <summary>
        /// 菜单栏-文件点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileItem_Click(object sender, RoutedEventArgs e)
        {

        }

        //点击关闭按钮执行完退出动画后执行
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
                System.Windows.Controls.ToolBar toolBar = sender as System.Windows.Controls.ToolBar;
                  var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
             if (overflowGrid != null)
              {
                     overflowGrid.Visibility = Visibility.Collapsed;
              }
          var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
          {
                mainPanelBorder.Margin = new Thickness(0);
                   }
        }
        /// <summary>
        /// 主窗口加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            double x = SystemParameters.WorkArea.Width;
            double y = SystemParameters.WorkArea.Height;
            double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度
            this.Width = x1 * 2 / 3 ;
            this.Height = y1 * 4 / 5;
            //Thread recvThread = new Thread(RecvDataAsync);
            //recvThread.Start();
        }

        /// <summary>
        /// 异步方式接收，500毫秒一次
        /// </summary>
        unsafe private void RecvDataAsync()
        {
            canHelper.Open();
            canHelper.Init();
            canHelper.Start();
            while (true)
            {
                Thread.Sleep(16);
                List<CanDTO> list = canHelper.Recv();
                if (list.Count <= 0)
                {
                    continue;
                }
                CanDTO dTO = list[list.Count - 1];
                FormatData(dTO);
            }
        }

        private void CheckZero(byte[] data, int index)
        {
            if ((data[index] & 0x80) == 0x80)
            {
                data[index] = 0;
                data[index + 1] = 0;
            }
        }

        /// <summary>
        /// 格式化接收的数据至类中
        /// </summary>
        /// <param name="recvData"></param>
        private void FormatData(CanDTO dTO)
        {
            //设置can数据指针
            
            int point = 0;

            byte[] recvData = dTO.Data;

            //判断数据来源
            uint canID = dTO.Id;
            canID = dTO.Id >> 21;
            uint canIdHigh = (canID & 0xf0) >> 4;
            uint canIdLow = canID & 0x0f;

            #region 解析CAN数据包中的8个字节，根据CAN ID来决定字段含义
            switch (canIdHigh)
            {
                case 1:
                    #region 主设备A1车CAN消息（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                mainDevDataContainersCar1.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                mainDevDataContainersCar1.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                mainDevDataContainersCar1.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            mainDevDataContainersCar1.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion
                            
                            #region byte1
                            mainDevDataContainersCar1.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte2~3
                            mainDevDataContainersCar1.RefSpeed = (recvData[2] * 256 + recvData[3]) / 10.0;
                            #endregion

                            #region byte4~5
                            mainDevDataContainersCar1.BrakeLevel = recvData[4] * 256 + recvData[5];
                            #endregion

                            #region byte6~7
                            mainDevDataContainersCar1.TrainBrakeForce = recvData[6] * 256 + recvData[7];
                            #endregion

                            #endregion
                            break;
                        case 1:
                            #region TPDO1(Checked)
                            mainDevDataContainersCar1.LifeSig = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar1.SpeedA1Shaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar1.SpeedA1Shaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            mainDevDataContainersCar1.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.MassSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                            #endregion

                        case 2:
                            #region TPDO2(Checked)
                            mainDevDataContainersCar1.AbTargetValueAx1 = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar1.AbTargetValueAx2 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar1.AbTargetValueAx3 = recvData[point + 4] * 256 + recvData[point + 5];
                            mainDevDataContainersCar1.AbTargetValueAx4 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion
                        case 3:
                            #region TPDO3(Checked)
                            mainDevDataContainersCar1.AbTargetValueAx5 = recvData[point] * 256 + recvData[point];
                            mainDevDataContainersCar1.AbTargetValueAx6 = recvData[point + 2] * 256 + recvData[point + 3];

                            mainDevDataContainersCar1.SelfTestInt = (recvData[4] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.SelfTestActive = (recvData[4] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.SelfTestSuccess = (recvData[4] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.SelfTestFail = (recvData[4] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.UnSelfTest24 = (recvData[4] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.UnSelfTest26 = (recvData[4] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.GateValveState = (recvData[4] & 0x40) == 0x40 ? true : false;

                            mainDevDataContainersCar1.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            mainDevDataContainersCar1.NetDriveCmd = (recvData[6] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.NetBrakeCmd = (recvData[6] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.NetFastBrakeCmd = (recvData[6] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.TowingMode = (recvData[6] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.HoldBrakeRealease = (recvData[6] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.CanUintSelfTestCmd_A = (recvData[6] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.CanUintSelfTestCmd_B = (recvData[6] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.ATOMode1 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            mainDevDataContainersCar1.BrakeLevelEnable = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.SelfTestCmd = (recvData[7] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.EdFadeOut = (recvData[7] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.TrainBrakeEnable = (recvData[7] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.ZeroSpeed = (recvData[7] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.EdOffB1 = (recvData[7] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.EdOffC1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.WheelInputState = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            mainDevDataContainersCar1.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar1.AirSpring1PressureA1Car1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar1.AirSpring2PressureA1Car1 = recvData[point + 4] * 256 + recvData[point + 5];
                            mainDevDataContainersCar1.ParkPressureA1 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            mainDevDataContainersCar1.VldRealPressureAx1 = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar1.Bcp1PressureAx1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar1.Bcp2PressureAx2 = recvData[point + 4] * 256 + recvData[point + 5];

                            mainDevDataContainersCar1.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x20 ? true : false;
                            mainDevDataContainersCar1.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            mainDevDataContainersCar1.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            mainDevDataContainersCar1.VldPressureSetupAx1 = recvData[point] * 256 + recvData[point];
                            mainDevDataContainersCar1.MassA1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar1.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            mainDevDataContainersCar1.SelfTestSetup = recvData[0] * 256 + recvData[1];

                            mainDevDataContainersCar1.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            

                            break;
                        #endregion
                    }
                    #endregion
                    break;

                case 3:
                    #region 从设备2车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            sliverDataContainerCar2.LifeSig = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar2.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar2.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            sliverDataContainerCar2.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar2.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            sliverDataContainerCar2.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            sliverDataContainerCar2.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar2.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar2.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar2.MassSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            sliverDataContainerCar2.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar2.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar2.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            sliverDataContainerCar2.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            sliverDataContainerCar2.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            sliverDataContainerCar2.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            sliverDataContainerCar2.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar2.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar2.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar2.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar2.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar2.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar2.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            sliverDataContainerCar2.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            sliverDataContainerCar2.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar2.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            sliverDataContainerCar2.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar2.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar2.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar2.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;

                            sliverDataContainerCar2.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar2.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar2.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar2.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            sliverDataContainerCar2.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar2.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar2.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar2.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            sliverDataContainerCar2.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            sliverDataContainerCar2.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar2.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar2.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar2.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;
                            
                            break;
                            #endregion
                    }
                    #endregion
                    break;
                
                case 4:
                    #region 从设备3车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            sliverDataContainerCar3.LifeSig = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar3.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar3.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            sliverDataContainerCar3.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar3.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            sliverDataContainerCar3.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            sliverDataContainerCar3.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar3.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar3.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar3.MassSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            sliverDataContainerCar3.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar3.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar3.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            sliverDataContainerCar3.ParkPressure = recvData[6] * 256 + recvData[7];

                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            sliverDataContainerCar3.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            sliverDataContainerCar3.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            sliverDataContainerCar3.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            sliverDataContainerCar3.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar3.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar3.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar3.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar3.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar3.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar3.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            sliverDataContainerCar3.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            sliverDataContainerCar3.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar3.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            sliverDataContainerCar3.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar3.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar3.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar3.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;

                            sliverDataContainerCar3.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar3.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar3.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar3.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            sliverDataContainerCar3.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar3.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar3.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar3.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            sliverDataContainerCar3.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            sliverDataContainerCar3.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar3.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar3.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar3.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;

                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 5:
                    #region 从设备4车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 2节点数据包1(Checked)
                            sliverDataContainerCar4.LifeSig = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar4.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar4.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            sliverDataContainerCar4.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar4.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            sliverDataContainerCar4.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            sliverDataContainerCar4.BSRLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar4.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar4.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar4.MassSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 4:
                            #region 2节点数据包2(Checked)
                            sliverDataContainerCar4.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar4.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar4.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            break;
                        #endregion

                        case 5:
                            #region 2节点数据包3(Checked)
                            sliverDataContainerCar4.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            sliverDataContainerCar4.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            sliverDataContainerCar4.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];

                            sliverDataContainerCar4.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar4.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar4.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar4.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar4.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar4.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar4.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            sliverDataContainerCar4.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region 2节点数据包4
                            sliverDataContainerCar4.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar4.MassValue = recvData[point + 2] * 256 + recvData[point + 3];

                            sliverDataContainerCar4.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar4.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar4.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar4.EmergencyBrakeFault = (recvData[6] & 0x08) == 0x08 ? true : false;

                            sliverDataContainerCar4.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar4.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar4.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar4.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            sliverDataContainerCar4.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar4.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar4.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar4.BCPLow1 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            sliverDataContainerCar4.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            sliverDataContainerCar4.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar4.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar4.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar4.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;

                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 6:
                    #region 从设备5车数据（4个数据包）
                    switch (canIdLow)
                    {
                        case 1:
                            #region 5节点数据包1
                            sliverDataContainerCar5.LifeSig = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar5.SpeedShaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar5.SpeedShaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            sliverDataContainerCar5.Slip = (recvData[7] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar5.EmergencyBrakeActive = (recvData[7] & 0x02) == 0x20 ? true : false;
                            sliverDataContainerCar5.AbBrakeActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            sliverDataContainerCar5.BCPLow1 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            sliverDataContainerCar5.ParkBrakeRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            sliverDataContainerCar5.AbBrakeSatet = (recvData[7] & 0x40) == 0x40 ? true : false;
                            sliverDataContainerCar5.MassSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 4:
                            #region 5节点数据包2
                            sliverDataContainerCar5.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar5.AirSpringPressure1 = recvData[point + 2] * 256 + recvData[point + 3];
                            sliverDataContainerCar5.AirSpringPressure2 = recvData[point + 4] * 256 + recvData[point + 5];
                            sliverDataContainerCar5.ParkPressure = recvData[6] * 256 + recvData[7];
                            break;
                        #endregion

                        case 5:
                            #region 5节点数据包3
                            sliverDataContainerCar5.VldRealPressure = recvData[point] * 256 + recvData[point + 1];
                            CheckZero(recvData, point + 2);
                            sliverDataContainerCar5.Bcp1Pressure = recvData[point + 2] * 256 + recvData[point + 3];
                            CheckZero(recvData, point + 4);
                            sliverDataContainerCar5.Bcp2Pressure = recvData[point + 4] * 256 + recvData[point + 5];
                            break;
                        #endregion

                        case 6:
                            #region 5节点数据包4
                            sliverDataContainerCar5.VldSetupPressure = recvData[point] * 256 + recvData[point + 1];
                            sliverDataContainerCar5.MassValue = recvData[point + 2] * 256 + recvData[point + 3];
                            break;
                        #endregion

                        case 7:
                            #region 2节点数据包5
                            sliverDataContainerCar5.SelfTestSetup = recvData[0] * 256 + recvData[1];
                            sliverDataContainerCar5.SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            sliverDataContainerCar5.SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            sliverDataContainerCar5.SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            sliverDataContainerCar5.SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;

                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 2:
                    #region 备6车数据（6个数据包）
                    switch (canIdLow)
                    {
                        case 0:
                            #region TPDO0(Checked)

                            #region byte0
                            if ((recvData[0] & 0x01) == 0x01)
                            {
                                mainDevDataContainersCar6.Mode = MainDevDataContains.NORMAL_MODE;
                            }
                            else if ((recvData[0] & 0x02) == 0x02)
                            {
                                mainDevDataContainersCar6.Mode = MainDevDataContains.EMERGENCY_DRIVE_MODE;
                            }
                            else if ((recvData[0] & 0x04) == 0x04)
                            {
                                mainDevDataContainersCar6.Mode = MainDevDataContains.CALLBACK_MODE;
                            }
                            mainDevDataContainersCar6.BrakeCmd = (recvData[0] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.DriveCmd = (recvData[0] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.LazyCmd = (recvData[0] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.FastBrakeCmd = (recvData[0] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.EmergencyBrakeCmd = (recvData[0] & 0x80) == 0x80 ? true : false;
                            #endregion

                            #region byte1
                            mainDevDataContainersCar6.KeepBrakeState = (recvData[1] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.LazyState = (recvData[1] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.DriveState = (recvData[1] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.NormalBrakeState = (recvData[1] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.EmergencyBrakeState = (recvData[1] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.ZeroSpeedCan = (recvData[1] & 0x20) == 0x20 ? true : false;
                            #endregion

                            #region byte2~3
                            mainDevDataContainersCar6.RefSpeed = recvData[2] * 256 + recvData[3];
                            #endregion

                            #region byte4~5
                            mainDevDataContainersCar6.BrakeLevel = recvData[4] * 256 + recvData[5];
                            #endregion

                            #region byte6~7
                            mainDevDataContainersCar6.TrainBrakeForce = recvData[6] * 256 + recvData[7];
                            #endregion

                            #endregion
                            break;
                        case 1:
                            #region TPDO1(Checked)
                            mainDevDataContainersCar6.LifeSig = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar6.SpeedA1Shaft1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar6.SpeedA1Shaft2 = recvData[point + 4] * 256 + recvData[point + 5];

                            mainDevDataContainersCar6.SlipA1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.EmergencyBrakeActiveA1 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.NotZeroSpeed = (recvData[7] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.AbActive = (recvData[7] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.BCPLowA11 = (recvData[7] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.ParkBreakRealease = (recvData[7] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.AbStatuesA1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.MassSigValid = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 2:
                            #region TPDO2(Checked)
                            mainDevDataContainersCar6.AbTargetValueAx1 = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar6.AbTargetValueAx2 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar6.AbTargetValueAx3 = recvData[point + 4] * 256 + recvData[point + 5];
                            mainDevDataContainersCar6.AbTargetValueAx4 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion
                        case 3:
                            #region TPDO3(Checked)
                            mainDevDataContainersCar6.AbTargetValueAx5 = recvData[point] * 256 + recvData[point];
                            mainDevDataContainersCar6.AbTargetValueAx6 = recvData[point + 2] * 256 + recvData[point + 3];

                            mainDevDataContainersCar6.SelfTestInt = (recvData[4] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.SelfTestActive = (recvData[4] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.SelfTestSuccess = (recvData[4] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.SelfTestFail = (recvData[4] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.UnSelfTest24 = (recvData[4] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.UnSelfTest26 = (recvData[4] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.GateValveState = (recvData[4] & 0x40) == 0x40 ? true : false;

                            mainDevDataContainersCar6.HardDriveCmd = (recvData[5] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.HardBrakeCmd = (recvData[5] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.HardFastBrakeCmd = (recvData[5] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.HardEmergencyBrake = (recvData[5] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.HardEmergencyDriveCmd = (recvData[5] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.CanUnitSelfTestOn = (recvData[5] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.ValveCanEmergencyActive = (recvData[5] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.CanUintSelfTestOver = (recvData[5] & 0x80) == 0x80 ? true : false;

                            mainDevDataContainersCar6.NetDriveCmd = (recvData[6] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.NetBrakeCmd = (recvData[6] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.NetFastBrakeCmd = (recvData[6] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.TowingMode = (recvData[6] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.HoldBrakeRealease = (recvData[6] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.CanUintSelfTestCmd_A = (recvData[6] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.CanUintSelfTestCmd_B = (recvData[6] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.ATOMode1 = (recvData[6] & 0x80) == 0x80 ? true : false;

                            mainDevDataContainersCar6.BrakeLevelEnable = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.SelfTestCmd = (recvData[7] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.EdFadeOut = (recvData[7] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.TrainBrakeEnable = (recvData[7] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.ZeroSpeed = (recvData[7] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.EdOffB1 = (recvData[7] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.EdOffC1 = (recvData[7] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.WheelInputState = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 4:
                            #region TPDO4(Checked)
                            mainDevDataContainersCar6.BrakeCylinderSourcePressure = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar6.AirSpring1PressureA1Car1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar6.AirSpring2PressureA1Car1 = recvData[point + 4] * 256 + recvData[point + 5];
                            mainDevDataContainersCar6.ParkPressureA1 = recvData[point + 6] * 256 + recvData[point + 7];
                            break;
                        #endregion

                        case 5:
                            #region TPDO5(Checked)
                            mainDevDataContainersCar6.VldRealPressureAx1 = recvData[point] * 256 + recvData[point + 1];
                            mainDevDataContainersCar6.Bcp1PressureAx1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar6.Bcp2PressureAx2 = recvData[point + 4] * 256 + recvData[point + 5];

                            mainDevDataContainersCar6.BSSRSenorFault = (recvData[6] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.AirSpringSenorFault_1 = (recvData[6] & 0x02) == 0x20 ? true : false;
                            mainDevDataContainersCar6.AirSpringSenorFault_2 = (recvData[6] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.ParkCylinderSenorFault = (recvData[6] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.VLDSensorFault = (recvData[6] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.BSRSenorFault_1 = (recvData[6] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.BSRSenorFault_2 = (recvData[6] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.AirSpringOverflow_1 = (recvData[6] & 0x80) == 0x80 ? true : false;
                            mainDevDataContainersCar6.AirSpringOverflow_2 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.BSRLowA11 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            break;
                        #endregion

                        case 6:
                            #region TPDO6(Checked)
                            mainDevDataContainersCar6.VldPressureSetupAx1 = recvData[point] * 256 + recvData[point];
                            mainDevDataContainersCar6.MassA1 = recvData[point + 2] * 256 + recvData[point + 3];
                            mainDevDataContainersCar6.BCUFail_Serious = (recvData[6] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.BCUFail_Middle = (recvData[6] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.BCUFail_Slight = (recvData[6] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.SpeedSenorFault_1 = (recvData[7] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.SpeedSenorFault_2 = (recvData[7] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.WSPFault_1 = (recvData[7] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.WSPFault_2 = (recvData[7] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.CodeConnectorFault = (recvData[7] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.AirSpringLimit = (recvData[7] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.BrakeNotRealease = (recvData[7] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.BCPLowA11 = (recvData[7] & 0x80) == 0x80 ? true : false;
                            break;
                        #endregion

                        case 7:
                            #region TPDO7(Checked)

                            mainDevDataContainersCar6.SelfTestSetup = recvData[0] * 256 + recvData[1];

                            mainDevDataContainersCar6.Ax1SelfTestActive = (recvData[2] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.Ax1SelfTestOver = (recvData[2] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.Ax1SelfTestSuccess = (recvData[2] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.Ax1SelfTestFail = (recvData[2] & 0x08) == 0x08 ? true : false;


                            break;
                            #endregion
                    }
                    #endregion
                    break;

                case 7:
                    #region 1车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 1车附加1数据(Checked)
                            mainDevDataContainersCar1.VCMLifeSig = recvData[1];
                            mainDevDataContainersCar1.DcuLifeSig[0] = recvData[2];
                            mainDevDataContainersCar1.DcuLifeSig[1] = recvData[3];

                            mainDevDataContainersCar1.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;

                            mainDevDataContainersCar1.DcuLifeSig[2] = recvData[6];
                            mainDevDataContainersCar1.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 1车附加2数据(Checked)
                            mainDevDataContainersCar1.DcuEbRealValue[0] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar1.DcuMax[0] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar1.DcuEbRealValue[1] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar1.DcuMax[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 3:
                            #region 1车附加3数据(Checked)
                            mainDevDataContainersCar1.DcuEbRealValue[2] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar1.DcuMax[2] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar1.DcuEbRealValue[3] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar1.DcuMax[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 4:
                            #region 1车附加4数据(Checked)
                            mainDevDataContainersCar1.AbCapacity[0] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar1.AbCapacity[1] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar1.AbCapacity[2] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar1.AbCapacity[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 5:
                            #region 1车附加5数据
                            mainDevDataContainersCar1.AbCapacity[4] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar1.AbCapacity[5] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar1.AbRealValue[0] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar1.AbRealValue[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 6:
                            #region 1车附加6数据(Checked)
                            mainDevDataContainersCar1.AbRealValue[2] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar1.AbRealValue[3] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar1.AbRealValue[4] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar1.AbRealValue[5] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 8:
                            #region 1车附加7数据(Checked)
                            mainDevDataContainersCar1.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar1.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar1.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar1.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar1.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar1.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            mainDevDataContainersCar1.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar1.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar1.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            mainDevDataContainersCar1.SoftVersion = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 9:
                            #region 1车附加9数据(Checked)
                            mainDevDataContainersCar1.UnixHour = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar1.UnixMinute = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar1.UnixTimeValid = (recvData[4] & 0x20) == 0x20 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;

                case 8:
                    #region 6车附加数据
                    switch (canIdLow)
                    {
                        case 1:
                            #region 6车附加1数据(Checked)
                            mainDevDataContainersCar6.VCMLifeSig = recvData[1];
                            mainDevDataContainersCar6.DcuLifeSig[0] = recvData[2];
                            mainDevDataContainersCar6.DcuLifeSig[1] = recvData[3];

                            mainDevDataContainersCar6.DcuEbOK[0] = (recvData[4] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.DcuEbFadeout[0] = (recvData[4] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.DcuEbSlip[0] = (recvData[4] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.DcuEbOK[1] = (recvData[4] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.DcuEbFadeout[1] = (recvData[4] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.DcuEbSlip[1] = (recvData[4] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.DcuEbOK[2] = (recvData[5] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.DcuEbFadeout[2] = (recvData[5] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.DcuEbSlip[2] = (recvData[5] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.DcuEbOK[3] = (recvData[5] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.DcuEbFadeout[3] = (recvData[5] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.DcuEbSlip[3] = (recvData[5] & 0x40) == 0x40 ? true : false;

                            mainDevDataContainersCar6.DcuLifeSig[2] = recvData[6];
                            mainDevDataContainersCar6.DcuLifeSig[3] = recvData[7];
                            #endregion
                            break;
                        case 2:
                            #region 6车附加2数据(Checked)
                            mainDevDataContainersCar6.DcuEbRealValue[0] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar6.DcuMax[0] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar6.DcuEbRealValue[1] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar6.DcuMax[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 3:
                            #region 6车附加3数据(Checked)
                            mainDevDataContainersCar6.DcuEbRealValue[2] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar6.DcuMax[2] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar6.DcuEbRealValue[3] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar6.DcuMax[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 4:
                            #region 6车附加4数据(Checked)
                            mainDevDataContainersCar6.AbCapacity[0] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar6.AbCapacity[1] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar6.AbCapacity[2] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar6.AbCapacity[3] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 5:
                            #region 6车附加5数据
                            mainDevDataContainersCar6.AbCapacity[4] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar6.AbCapacity[5] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar6.AbRealValue[0] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar6.AbRealValue[1] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 6:
                            #region 6车附加6数据(Checked)
                            mainDevDataContainersCar6.AbRealValue[2] = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar6.AbRealValue[3] = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar6.AbRealValue[4] = recvData[4] * 256 + recvData[5];
                            mainDevDataContainersCar6.AbRealValue[5] = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 8:
                            #region 6车附加7数据(Checked)
                            mainDevDataContainersCar6.SpeedDetection = (recvData[0] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.CanBusFail1 = (recvData[0] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.CanBusFail2 = (recvData[0] & 0x04) == 0x04 ? true : false;
                            mainDevDataContainersCar6.HardDifferent = (recvData[0] & 0x08) == 0x08 ? true : false;
                            mainDevDataContainersCar6.EventHigh = (recvData[0] & 0x10) == 0x10 ? true : false;
                            mainDevDataContainersCar6.EventMid = (recvData[0] & 0x20) == 0x20 ? true : false;
                            mainDevDataContainersCar6.EventLow = (recvData[0] & 0x40) == 0x40 ? true : false;
                            mainDevDataContainersCar6.CanASPEnable = (recvData[0] & 0x80) == 0x80 ? true : false;

                            mainDevDataContainersCar6.BCPLowA = (recvData[1] & 0x01) == 0x01 ? true : false;
                            mainDevDataContainersCar6.BCPLowB = (recvData[1] & 0x02) == 0x02 ? true : false;
                            mainDevDataContainersCar6.BCPLowC = (recvData[1] & 0x04) == 0x04 ? true : false;

                            mainDevDataContainersCar6.SoftVersion = recvData[6] * 256 + recvData[7];
                            #endregion
                            break;
                        case 9:
                            #region 6车附加9数据(Checked)
                            mainDevDataContainersCar6.UnixHour = recvData[0] * 256 + recvData[1];
                            mainDevDataContainersCar6.UnixMinute = recvData[2] * 256 + recvData[3];
                            mainDevDataContainersCar6.UnixTimeValid = (recvData[4] & 0x20) == 0x20 ? true : false;
                            #endregion
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                    
            }
            #endregion
        }
        

        #region 更新所有已实例化的窗口的UI
        /// <summary>
        /// 更新主UI
        /// </summary>
        /// <param name="mainDevDataContains">需要向全体窗口通知的数据DTO</param>
        private void updateUIMethod(MainDevDataContains mainDevDataContains)
        {
            //MainDashboard.slider.Value = (mainDevDataContainers.SpeedA1Shaft1 + mainDevDataContainers.SpeedA1Shaft2) / 2;
            this.Dispatcher.Invoke(new Action(() => {
                MainDashboard.slider.Value = (mainDevDataContainersCar1.SpeedA1Shaft1 + mainDevDataContainersCar1.SpeedA1Shaft2) / 2;
            }));
            if(detailWindowCar1 != null)
            {
                detailWindowCar1.UpdateData(mainDevDataContainersCar1);
            }
            if(slaveDetailWindowCar2 != null)
            {
                slaveDetailWindowCar2.UpdateData(sliverDataContainerCar2);
            }
            if (slaveDetailWindowCar3 != null)
            {
                slaveDetailWindowCar3.UpdateData(sliverDataContainerCar3);
            }
            if (slaveDetailWindowCar4 != null)
            {
                slaveDetailWindowCar4.UpdateData(sliverDataContainerCar4);
            }
            if (slaveDetailWindowCar5 != null)
            {
                slaveDetailWindowCar5.UpdateData(sliverDataContainerCar5);
            }
            if (slaveDetailWindowCar6 != null)
            {
                slaveDetailWindowCar6.UpdateData(mainDevDataContainersCar6);
            }
            if (speedChartWindow != null)
            {
                speedChartWindow.UpdateData(mainDevDataContainersCar1, sliverDataContainerCar2, sliverDataContainerCar3, sliverDataContainerCar4, sliverDataContainerCar5, mainDevDataContainersCar6);
            }
            if (pressureChartWindow != null)
            {
                pressureChartWindow.UpdateData(mainDevDataContainersCar1, sliverDataContainerCar2, sliverDataContainerCar3, sliverDataContainerCar4, sliverDataContainerCar5, mainDevDataContainersCar6);
            }
            if (otherWindow != null)
            {
                otherWindow.UpdateData(mainDevDataContainersCar1, sliverDataContainerCar2, sliverDataContainerCar3, sliverDataContainerCar4, sliverDataContainerCar5, mainDevDataContainersCar6);
            }
            if (overviewWindow != null)
            {
                overviewWindow.UpdateData(mainDevDataContainersCar1, sliverDataContainerCar2, sliverDataContainerCar3, sliverDataContainerCar4, sliverDataContainerCar5, mainDevDataContainersCar6);
            }
            if (chartWindow != null)
            {
                chartWindow.UpdateData(mainDevDataContainersCar1, sliverDataContainerCar2, sliverDataContainerCar3, sliverDataContainerCar4, sliverDataContainerCar5, mainDevDataContainersCar6);
            }
            if (singleChartWindow != null)
            {
                //singleChartWindow.UpdateData(mainDevDataContainersCar1, sliverDataContainerCar2, sliverDataContainerCar3, sliverDataContainerCar4, sliverDataContainerCar5, mainDevDataContainersCar6);
            }
        }
        #endregion

        #region 按键事件处理器，根据事件发出控件的 Name 属性来决定动作
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(trainHeaderFirstBtn)) {
                if(detailWindowCar1 == null)
                {
                    detailWindowCar1 = new DetailWindow(mainDevDataContainersCar1, "EBCU1");
                    detailWindowCar1.CloseWindowEvent += OtherWindowClosedHandler;
                }
                detailWindowCar1.Show();
            }
            else if (sender.Equals(trainHeaderSecondBtn)) {
                if (slaveDetailWindowCar2 == null)
                {
                    slaveDetailWindowCar2 = new SlaveDetailWindow(sliverDataContainerCar2, "EBCU2");
                    slaveDetailWindowCar2.CloseWindowEvent += OtherWindowClosedHandler;                                                     
                }
                slaveDetailWindowCar2.Show();
            }
            else if (sender.Equals(trainMiddleFirstBtn))
            {
                if (slaveDetailWindowCar3 == null)
                {
                    slaveDetailWindowCar3 = new SlaveDetailWindow(sliverDataContainerCar3, "EBCU3");
                    slaveDetailWindowCar3.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar3.Show();
            }
            else if (sender.Equals(trainMiddleSecondBtn))
            {
                if (slaveDetailWindowCar4 == null)
                {
                    slaveDetailWindowCar4 = new SlaveDetailWindow(sliverDataContainerCar4, "EBCU4");
                    slaveDetailWindowCar4.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar4.Show();
            }
            else if (sender.Equals(trainTailFirstBtn))
            {
                if (slaveDetailWindowCar5 == null)
                {
                    slaveDetailWindowCar5 = new SlaveDetailWindow(sliverDataContainerCar5, "EBCU5");
                    slaveDetailWindowCar5.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar5.Show();
            }
            else if (sender.Equals(trainTailSecondBtn))
            {
                if (slaveDetailWindowCar6 == null)
                {
                    slaveDetailWindowCar6 = new DetailWindow(mainDevDataContainersCar6, "EBCU6");
                    slaveDetailWindowCar6.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar6.Show();
            }
            else if (sender.Equals(car1View))
            {
                if (detailWindowCar1 == null)
                {
                    detailWindowCar1 = new DetailWindow(mainDevDataContainersCar1, "EBCU1");
                    detailWindowCar1.CloseWindowEvent += OtherWindowClosedHandler;
                }
                detailWindowCar1.Show();
            }
            else if (sender.Equals(car2View))
            {
                if (slaveDetailWindowCar2 == null)
                {
                    slaveDetailWindowCar2 = new SlaveDetailWindow(sliverDataContainerCar2, "EBCU2");
                    slaveDetailWindowCar2.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar2.Show();
            }
            else if (sender.Equals(car3View))
            {
                if (slaveDetailWindowCar3 == null)
                {
                    slaveDetailWindowCar3 = new SlaveDetailWindow(sliverDataContainerCar3, "EBCU3");
                    slaveDetailWindowCar3.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar3.Show();
            }
            else if (sender.Equals(car4View))
            {
                if (slaveDetailWindowCar4 == null)
                {
                    slaveDetailWindowCar4 = new SlaveDetailWindow(sliverDataContainerCar4, "EBCU4");
                    slaveDetailWindowCar4.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar4.Show();
            }
            else if (sender.Equals(car5View))
            {
                if (slaveDetailWindowCar5 == null)
                {
                    slaveDetailWindowCar5 = new SlaveDetailWindow(sliverDataContainerCar5, "EBCU5");
                    slaveDetailWindowCar5.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar5.Show();
            }
            else if (sender.Equals(car6View))
            {
                if (slaveDetailWindowCar6 == null)
                {
                    slaveDetailWindowCar6 = new DetailWindow(mainDevDataContainersCar6, "EBCU6");
                    slaveDetailWindowCar6.CloseWindowEvent += OtherWindowClosedHandler;
                }
                slaveDetailWindowCar6.Show();
            }
            else if (sender.Equals(nodeViewItem))
            {
                NodeWindow nodeWindow = new NodeWindow(mainDevDataContainersCar1.LifeSig, sliverDataContainerCar2.LifeSig, sliverDataContainerCar3.LifeSig, sliverDataContainerCar4.LifeSig, sliverDataContainerCar5.LifeSig, mainDevDataContainersCar6.LifeSig);
                nodeWindow.Show();
            }
            else if (sender.Equals(wheelDiaItem))
            {
                ParameterSetWindow parameterSetWindow = new ParameterSetWindow();
                parameterSetWindow.ShowDialog();
            }
            else if (sender.Equals(uploadItem))
            {
                DownloadExe();
            }
            else if (sender.Equals(closeBtn))
            {
                CloseDevice();
                App.Current.Shutdown();
            }
            else if (sender.Equals(showSpeedChartItem))
            {
                //显示实时曲线窗体
                if (speedChartWindow == null)
                {
                    speedChartWindow = new RealTimeSpeedChartWindow();
                    speedChartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                speedChartWindow.Show();
            }
            else if (sender.Equals(showPressureChartItem))
            {
                if (pressureChartWindow == null)
                {
                    pressureChartWindow = new RealTimePressureChartWindow();
                    pressureChartWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                pressureChartWindow.Show();
            }
            else if (sender.Equals(showOtherChartItem))
            {
                if (otherWindow == null)
                {
                    otherWindow = new RealTimeOtherWindow();
                    otherWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                otherWindow.Show();
            }
            else if (sender.Equals(OverViewItem))
            {
                if (overviewWindow == null)
                {
                    overviewWindow = new OverviewWindow();
                    overviewWindow.CloseWindowEvent += OtherWindowClosedHandler;
                }
                overviewWindow.Show();
            }
            else if (sender.Equals(chartViewItem))
            {
                if (chartWindow == null)
                {
                    chartWindow = new ChartWindow();
                }
                chartWindow.Show();
            }
            else if (sender.Equals(singleViewItem))
            {
                if (singleChartWindow == null)
                {
                    singleChartWindow = new SingleChart();
                }
                singleChartWindow.Show();
            }
        }

        private void CloseDevice()
        {
            //CanHelper.DeviceState res = canHelper.Close();
        }

        private void OtherWindowClosedHandler(bool winState, string name)
        {
            if ("EBCU1".Equals(name))                                     
            {
                detailWindowCar1 = null;
            }
            else if ("EBCU2".Equals(name))
            {
                slaveDetailWindowCar2 = null;
            }
            else if ("EBCU3".Equals(name))
            {
                slaveDetailWindowCar3 = null;
            }
            else if ("EBCU4".Equals(name))
            {
                slaveDetailWindowCar4 = null;
            }
            else if ("EBCU5".Equals(name))
            {
                slaveDetailWindowCar5 = null;
            }
            else if ("EBCU6".Equals(name))
            {
                slaveDetailWindowCar6 = null;
            }
            else if ("speedChart".Equals(name))
            {
                speedChartWindow = null;
            }
            else if ("pressureChart".Equals(name))
            {
                pressureChartWindow = null;
            }
            else if ("otherChart".Equals(name))
            {
                otherWindow = null;
            }
            else if ("overview".Equals(name))
            {
                overviewWindow = null;
            }
            else if ("chart".Equals(name))
            {
                chartWindow = null;
            }
            else if ("single".Equals(name))
            {
                singleChartWindow = null;
            }
        }
        #endregion

        /// <summary>
        /// 打开外部下载程序
        /// </summary>
        private void DownloadExe()
        {
            //string path = ConfigurationManager.AppSettings["text"];
            //if (path == null || path == "")
            //{
            //    System.Windows.Forms.MessageBox.Show("外部程序路径配置错误" + path, "exe error!");
            //    return;
            //}
            //System.Windows.Forms.MessageBox.Show(path + "", "exe error!");
            Process.Start(@"CANJieMianMFC.exe");
        }
    }

}
