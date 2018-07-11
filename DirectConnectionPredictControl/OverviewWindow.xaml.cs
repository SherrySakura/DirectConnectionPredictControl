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

namespace DirectConnectionPredictControl
{
    /// <summary>
    /// OverviewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OverviewWindow : Window
    {
        /// <summary>
        /// 数据组
        /// </summary>
        private MainDevDataContains container_1 = new MainDevDataContains();
        private MainDevDataContains container_6 = new MainDevDataContains();
        private SliverDataContainer container_2 = new SliverDataContainer();
        private SliverDataContainer container_3 = new SliverDataContainer();
        private SliverDataContainer container_4 = new SliverDataContainer();
        private SliverDataContainer container_5 = new SliverDataContainer();

        /// <summary>
        /// 线程组
        /// </summary>
        private Thread uiThread;


        public event closeWindowHandler CloseWindowEvent;
        public OverviewWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            uiThread = new Thread(UpdateUI);
            uiThread.Start();
        }

        #region bussiness method
        private void UpdateUI()
        {
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    UpdateAO();
                    UpdateDI();
                    UpdateDO();
                    UpdateFA();
                });
                Thread.Sleep(50);
            }
            
        }

        /// <summary>
        /// 更新主UI模拟量
        /// </summary>
        private void UpdateAO()
        {
            row_1_column_1.Content = container_1.RefSpeed;
            row_2_column_1.Content = container_1.BrakeLevel;
            row_3_column_1.Content = container_1.TrainBrakeForce;

            //生命信号
            row_4_column_1.Content = container_1.LifeSig;
            row_4_column_2.Content = container_2.LifeSig;
            row_4_column_3.Content = container_3.LifeSig;
            row_4_column_4.Content = container_4.LifeSig;
            row_4_column_5.Content = container_5.LifeSig;
            row_4_column_6.Content = container_6.LifeSig;

            //速度信号
            row_5_column_1.Content = container_1.SpeedA1Shaft1;
            row_5_column_2.Content = container_2.SpeedShaft1;
            row_5_column_3.Content = container_3.SpeedShaft1;
            row_5_column_4.Content = container_4.SpeedShaft1;
            row_5_column_5.Content = container_5.SpeedShaft1;
            row_5_column_6.Content = container_6.SpeedA1Shaft1;
            
            row_6_column_1.Content = container_1.SpeedA1Shaft2;
            row_6_column_2.Content = container_2.SpeedShaft2;
            row_6_column_3.Content = container_3.SpeedShaft2;
            row_6_column_4.Content = container_4.SpeedShaft2;
            row_6_column_5.Content = container_5.SpeedShaft2;
            row_6_column_6.Content = container_6.SpeedA1Shaft2;

            //空气制动目标值
            row_7_column_1.Content = container_1.AbTargetValueAx1;
            row_7_column_2.Content = container_1.AbTargetValueAx2;
            row_7_column_3.Content = container_1.AbTargetValueAx3;
            row_7_column_4.Content = container_1.AbTargetValueAx4;
            row_7_column_5.Content = container_1.AbTargetValueAx5;
            row_7_column_6.Content = container_1.AbTargetValueAx6;

            //1/6架制动风缸压力
            row_8_column_1.Content = container_1.BrakeCylinderSourcePressure;
            row_8_column_2.Content = container_2.BrakeCylinderSourcePressure;
            row_8_column_3.Content = container_3.BrakeCylinderSourcePressure;
            row_8_column_4.Content = container_4.BrakeCylinderSourcePressure;
            row_8_column_5.Content = container_5.BrakeCylinderSourcePressure;
            row_8_column_6.Content = container_6.BrakeCylinderSourcePressure;

            //1/6架空簧压力
            row_9_column_1.Content = container_1.AirSpring1PressureA1Car1;
            row_9_column_2.Content = container_2.AirSpringPressure1;
            row_9_column_3.Content = container_3.AirSpringPressure1;
            row_9_column_4.Content = container_4.AirSpringPressure1;
            row_9_column_5.Content = container_5.AirSpringPressure1;
            row_9_column_6.Content = container_6.AirSpring1PressureA1Car1;

            row_10_column_1.Content = container_1.AirSpring2PressureA1Car1;
            row_10_column_2.Content = container_2.AirSpringPressure2;
            row_10_column_3.Content = container_3.AirSpringPressure2;
            row_10_column_4.Content = container_4.AirSpringPressure2;
            row_10_column_5.Content = container_5.AirSpringPressure2;
            row_10_column_6.Content = container_6.AirSpring2PressureA1Car1;

            //停放压力
            row_11_column_1.Content = container_1.ParkPressureA1;
            row_11_column_3.Content = container_3.ParkPressure;
            row_11_column_5.Content = container_5.ParkPressure;

            //VLD实际
            row_12_column_1.Content = container_1.VldRealPressureAx1;
            row_12_column_2.Content = container_2.VldRealPressure;
            row_12_column_3.Content = container_3.VldRealPressure;
            row_12_column_4.Content = container_4.VldRealPressure;
            row_12_column_5.Content = container_5.VldRealPressure;
            row_12_column_6.Content = container_6.VldRealPressureAx1;

            //1/6架制动缸压力
            row_13_column_1.Content = container_1.Bcp1PressureAx1;
            row_13_column_2.Content = container_2.Bcp1Pressure;
            row_13_column_3.Content = container_3.Bcp1Pressure;
            row_13_column_4.Content = container_4.Bcp1Pressure;
            row_13_column_5.Content = container_5.Bcp1Pressure;
            row_13_column_6.Content = container_6.Bcp1PressureAx1;

            row_14_column_1.Content = container_1.Bcp2PressureAx2;
            row_14_column_2.Content = container_2.Bcp2Pressure;
            row_14_column_3.Content = container_3.Bcp2Pressure;
            row_14_column_4.Content = container_4.Bcp2Pressure;
            row_14_column_5.Content = container_5.Bcp2Pressure;
            row_14_column_6.Content = container_6.Bcp2PressureAx2;

            //VLD设定
            row_15_column_1.Content = container_1.VldPressureSetupAx1;
            row_15_column_2.Content = container_2.VldSetupPressure;
            row_15_column_3.Content = container_3.VldSetupPressure;
            row_15_column_4.Content = container_4.VldSetupPressure;
            row_15_column_5.Content = container_5.VldSetupPressure;
            row_15_column_6.Content = container_6.VldPressureSetupAx1;

            //1/6架载荷
            row_16_column_1.Content = container_1.MassA1;
            row_16_column_2.Content = container_2.MassValue;
            row_16_column_3.Content = container_3.MassValue;
            row_16_column_4.Content = container_4.MassValue;
            row_16_column_5.Content = container_5.MassValue;
            row_16_column_6.Content = container_6.MassA1;

            //架1自检目标设定值
            row_17_column_1.Content = container_1.SelfTestSetup;
            row_17_column_2.Content = container_2.SelfTestSetup;
            row_17_column_3.Content = container_3.SelfTestSetup;
            row_17_column_4.Content = container_4.SelfTestSetup;
            row_17_column_5.Content = container_5.SelfTestSetup;
            row_17_column_6.Content = container_6.SelfTestSetup;

            //VCM & DCU
            row_18_column_1.Content = container_1.VCMLifeSig;
            row_19_column_1.Content = container_1.DcuLifeSig[0];
            row_20_column_1.Content = container_1.DcuLifeSig[1];
            row_21_column_1.Content = container_1.DcuLifeSig[2];
            row_22_column_1.Content = container_1.DcuLifeSig[3];
            row_23_column_1.Content = container_1.DcuEbRealValue[0];
            row_24_column_1.Content = container_1.DcuMax[0];
            row_25_column_1.Content = container_1.DcuEbRealValue[1];
            row_26_column_1.Content = container_1.DcuMax[1];
            row_27_column_1.Content = container_1.DcuEbRealValue[2];
            row_28_column_1.Content = container_1.DcuMax[2];
            row_29_column_1.Content = container_1.DcuEbRealValue[3];
            row_30_column_1.Content = container_1.DcuMax[3];

            //空气制动能力值
            row_31_column_1.Content = container_1.AbCapacity[0];
            row_31_column_2.Content = container_1.AbCapacity[1];
            row_31_column_3.Content = container_1.AbCapacity[2];
            row_31_column_4.Content = container_1.AbCapacity[3];
            row_31_column_5.Content = container_1.AbCapacity[4];
            row_31_column_6.Content = container_1.AbCapacity[5];

            //空气制动实际值
            row_32_column_1.Content = container_1.AbRealValue[0];
            row_32_column_2.Content = container_1.AbRealValue[1];
            row_32_column_3.Content = container_1.AbRealValue[2];
            row_32_column_4.Content = container_1.AbRealValue[3];
            row_32_column_5.Content = container_1.AbRealValue[4];
            row_32_column_6.Content = container_1.AbRealValue[5];

            //软件版本
            row_33_column_1.Content = container_1.SoftVersion;
            row_34_column_4.Content = container_4.ParkPressure;
        }

        private void UpdateDI()
        {
            //制动命令
            row_1_column_1_DI.Fill = GetBrush(container_1.BrakeCmd);

            //牵引命令
            row_2_column_1_DI.Fill = GetBrush(container_1.DriveCmd);

            //惰行命令
            row_3_column_1_DI.Fill = GetBrush(container_1.LazyCmd);

            //快速制动命令
            row_4_column_1_DI.Fill = GetBrush(container_1.FastBrakeCmd);

            //紧急制动命令
            row_5_column_1_DI.Fill = GetBrush(container_1.EmergencyBrakeCmd);

            //紧急制动激活
            row_6_column_1_DI.Fill = GetBrush(container_1.EmergencyBrakeActiveA1);
            row_6_column_2_DI.Fill = GetBrush(container_2.EmergencyBrakeActive);
            row_6_column_3_DI.Fill = GetBrush(container_3.EmergencyBrakeActive);
            row_6_column_4_DI.Fill = GetBrush(container_4.EmergencyBrakeActive);
            row_6_column_5_DI.Fill = GetBrush(container_5.EmergencyBrakeActive);
            row_6_column_6_DI.Fill = GetBrush(container_6.EmergencyBrakeActiveA1);

            //空气制动施加
            row_7_column_1_DI.Fill = GetBrush(container_1.AbActive);
            row_7_column_2_DI.Fill = GetBrush(container_2.AbBrakeActive);
            row_7_column_3_DI.Fill = GetBrush(container_3.AbBrakeActive);
            row_7_column_4_DI.Fill = GetBrush(container_4.AbBrakeActive);
            row_7_column_5_DI.Fill = GetBrush(container_5.AbBrakeActive);
            row_7_column_6_DI.Fill = GetBrush(container_6.AbActive);

            //停放制动缓解
            row_8_column_1_DI.Fill = GetBrush(container_1.ParkBreakRealease);
            row_8_column_3_DI.Fill = GetBrush(container_3.ParkBrakeRealease);
            row_8_column_5_DI.Fill = GetBrush(container_5.ParkBrakeRealease);

            //硬线牵引
            row_9_column_1_DI.Fill = GetBrush(container_1.HardDriveCmd);

            //硬线制动
            row_10_column_1_DI.Fill = GetBrush(container_1.HardBrakeCmd);

            //第二列
            //硬线快速制动
            row_1_column_8_DI.Fill = GetBrush(container_1.HardFastBrakeCmd);

            //硬线紧急制动
            //row_2_column_8_DI.Fill = GetBrush(container_);

            //硬线紧急牵引
            row_3_column_8_DI.Fill = GetBrush(container_1.HardEmergencyDriveCmd);

            //网络牵引
            row_4_column_8_DI.Fill = GetBrush(container_1.NetDriveCmd);

            //网络制动
            row_5_column_8_DI.Fill = GetBrush(container_1.NetBrakeCmd);

            //保持制动缓解
            row_6_column_8_DI.Fill = GetBrush(container_1.HoldBrakeRealease);

            //CAN单元
            row_7_column_8_DI.Fill = GetBrush(container_1.CanUintSelfTestCmd_A);
            row_8_column_8_DI.Fill = GetBrush(container_1.CanUintSelfTestCmd_B);

            //自检
            row_9_column_8_DI.Fill = GetBrush(container_1.SelfTestCmd);

            //紧急制动命令
            row_10_column_8_DI.Fill = GetBrush(container_1.EmergencyBrakeCmd);
            
        }

        private void UpdateDO()
        {
            row_1_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.NORMAL_MODE));
            row_2_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.EMERGENCY_DRIVE_MODE));
            row_3_column_1_DO.Fill = GetBrush(container_1.Mode.Equals(MainDevDataContains.CALLBACK_MODE));
            row_4_column_1_DO.Fill = GetBrush(container_1.LazyCmd);
            row_5_column_1_DO.Fill = GetBrush(container_1.KeepBrakeState);
            row_6_column_1_DO.Fill = GetBrush(container_1.LazyState);
            row_7_column_1_DO.Fill = GetBrush(container_1.DriveState);
            row_8_column_1_DO.Fill = GetBrush(container_1.NormalBrakeState);
            row_9_column_1_DO.Fill = GetBrush(container_1.EmergencyBrakeState);
            row_10_column_1_DO.Fill = GetBrush(container_1.ZeroSpeed);
            row_11_column_1_DO.Fill = GetBrush(container_1.SelfTestFail);
            row_12_column_1_DO.Fill = GetBrush(container_1.UnSelfTest24);
            row_13_column_1_DO.Fill = GetBrush(container_1.UnSelfTest26);
            row_14_column_1_DO.Fill = GetBrush(container_1.GateValveState);
            row_15_column_1_DO.Fill = GetBrush(container_1.CanUnitSelfTestOn);
            row_16_column_1_DO.Fill = GetBrush(container_1.ValveCanEmergencyActive);
            row_17_column_1_DO.Fill = GetBrush(container_1.CanUintSelfTestOver);
            row_18_column_1_DO.Fill = GetBrush(container_1.TowingMode);
            row_19_column_1_DO.Fill = GetBrush(container_1.ATOMode1);
            row_20_column_1_DO.Fill = GetBrush(container_1.BrakeLevelEnable);
            row_21_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[0]);
            row_22_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[0]);
            row_23_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[0]);
            row_24_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[1]);
            row_25_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[1]);
            row_26_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[1]);
            row_27_column_1_DO.Fill = GetBrush(container_1.DcuEbOK[2]);
            row_28_column_1_DO.Fill = GetBrush(container_1.DcuEbFadeout[2]);
            row_29_column_1_DO.Fill = GetBrush(container_1.DcuEbSlip[2]);

            //第二列
            row_1_column_8_DO.Fill = GetBrush(container_1.VCM_MVBConnectionState);

            row_2_column_8_DO.Fill = GetBrush(container_1.SlipA1);
            row_2_column_9_DO.Fill = GetBrush(container_2.Slip);
            row_2_column_10_DO.Fill = GetBrush(container_3.Slip);
            row_2_column_11_DO.Fill = GetBrush(container_4.Slip);
            row_2_column_12_DO.Fill = GetBrush(container_5.Slip);
            row_2_column_13_DO.Fill = GetBrush(container_6.SlipA1);

            row_3_column_8_DO.Fill = GetBrush(container_1.NotZeroSpeed);

            row_4_column_8_DO.Fill = GetBrush(container_1.BSRLowA11);
            row_4_column_9_DO.Fill = GetBrush(container_2.BSRLow1);
            row_4_column_10_DO.Fill = GetBrush(container_3.BSRLow1);
            row_4_column_11_DO.Fill = GetBrush(container_4.BSRLow1);
            row_4_column_12_DO.Fill = GetBrush(container_5.BSRLow1);
            row_4_column_13_DO.Fill = GetBrush(container_6.BSRLowA11);

            row_5_column_8_DO.Fill = GetBrush(container_1.AbStatuesA1);
            row_5_column_9_DO.Fill = GetBrush(container_2.AbBrakeSatet);
            row_5_column_10_DO.Fill = GetBrush(container_3.AbBrakeSatet);
            row_5_column_11_DO.Fill = GetBrush(container_4.AbBrakeSatet);
            row_5_column_12_DO.Fill = GetBrush(container_5.AbBrakeSatet);
            row_5_column_13_DO.Fill = GetBrush(container_6.AbStatuesA1);
            
            row_6_column_8_DO.Fill = GetBrush(container_1.MassSigValid);
            row_6_column_9_DO.Fill = GetBrush(container_2.MassSigValid);
            row_6_column_10_DO.Fill = GetBrush(container_3.MassSigValid);
            row_6_column_11_DO.Fill = GetBrush(container_4.MassSigValid);
            row_6_column_12_DO.Fill = GetBrush(container_5.MassSigValid);
            row_6_column_13_DO.Fill = GetBrush(container_6.MassSigValid);

            row_7_column_8_DO.Fill = GetBrush(container_1.SelfTestFail);
            row_8_column_8_DO.Fill = GetBrush(container_1.SelfTestActive);
            row_9_column_8_DO.Fill = GetBrush(container_1.SelfTestSuccess);
            row_10_column_8_DO.Fill = GetBrush(container_1.EdFadeOut);
            row_11_column_8_DO.Fill = GetBrush(container_1.TrainBrakeEnable);
            row_12_column_8_DO.Fill = GetBrush(container_1.ZeroSpeed);
            row_13_column_8_DO.Fill = GetBrush(container_1.EdOffB1);
            row_14_column_8_DO.Fill = GetBrush(container_1.EdOffC1);
            row_15_column_8_DO.Fill = GetBrush(container_1.WheelInputState);

            row_16_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestActive);
            row_16_column_9_DO.Fill = GetBrush(container_2.SelfTestActive);
            row_16_column_10_DO.Fill = GetBrush(container_3.SelfTestActive);
            row_16_column_11_DO.Fill = GetBrush(container_4.SelfTestActive);
            row_16_column_12_DO.Fill = GetBrush(container_5.SelfTestActive);
            row_16_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestActive);

            row_17_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestOver);
            row_17_column_9_DO.Fill = GetBrush(container_2.SelfTestOver);
            row_17_column_10_DO.Fill = GetBrush(container_3.SelfTestOver);
            row_17_column_11_DO.Fill = GetBrush(container_4.SelfTestOver);
            row_17_column_12_DO.Fill = GetBrush(container_5.SelfTestOver);
            row_17_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestOver);
            
            row_18_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestSuccess);
            row_18_column_9_DO.Fill = GetBrush(container_2.SelfTestSuccess);
            row_18_column_10_DO.Fill = GetBrush(container_3.SelfTestSuccess);
            row_18_column_11_DO.Fill = GetBrush(container_4.SelfTestSuccess);
            row_18_column_12_DO.Fill = GetBrush(container_5.SelfTestSuccess);
            row_18_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestSuccess);
            
            row_19_column_8_DO.Fill = GetBrush(container_1.Ax1SelfTestFail);
            row_19_column_9_DO.Fill = GetBrush(container_2.SelfTestFail);
            row_19_column_10_DO.Fill = GetBrush(container_3.SelfTestFail);
            row_19_column_11_DO.Fill = GetBrush(container_4.SelfTestFail);
            row_19_column_12_DO.Fill = GetBrush(container_5.SelfTestFail);
            row_19_column_13_DO.Fill = GetBrush(container_6.Ax1SelfTestFail);
            
            row_20_column_8_DO.Fill = GetBrush(container_1.DcuEbOK[3]);
            row_21_column_8_DO.Fill = GetBrush(container_1.DcuEbFadeout[3]);
            row_22_column_8_DO.Fill = GetBrush(container_1.DcuEbSlip[3]);
        }

        private void UpdateFA()
        {
            row_1_column_1_FA.Fill = GetBrush(container_1.BSSRSenorFault);
            row_1_column_2_FA.Fill = GetBrush(container_2.BSSRSenorFault);
            row_1_column_3_FA.Fill = GetBrush(container_3.BSSRSenorFault);
            row_1_column_4_FA.Fill = GetBrush(container_4.BSSRSenorFault);
            row_1_column_5_FA.Fill = GetBrush(container_5.BSSRSenorFault);
            row_1_column_6_FA.Fill = GetBrush(container_6.BSSRSenorFault);

            row_2_column_1_FA.Fill = GetBrush(container_1.AirSpringSenorFault_1);
            row_2_column_2_FA.Fill = GetBrush(container_2.AirSpringSenorFault_1);
            row_2_column_3_FA.Fill = GetBrush(container_3.AirSpringSenorFault_1);
            row_2_column_4_FA.Fill = GetBrush(container_4.AirSpringSenorFault_1);
            row_2_column_5_FA.Fill = GetBrush(container_5.AirSpringSenorFault_1);
            row_2_column_6_FA.Fill = GetBrush(container_6.AirSpringSenorFault_1);

            row_3_column_1_FA.Fill = GetBrush(container_1.AirSpringSenorFault_2);
            row_3_column_2_FA.Fill = GetBrush(container_2.AirSpringSenorFault_2);
            row_3_column_3_FA.Fill = GetBrush(container_3.AirSpringSenorFault_2);
            row_3_column_4_FA.Fill = GetBrush(container_4.AirSpringSenorFault_2);
            row_3_column_5_FA.Fill = GetBrush(container_5.AirSpringSenorFault_2);
            row_3_column_6_FA.Fill = GetBrush(container_6.AirSpringSenorFault_2);

            row_4_column_1_FA.Fill = GetBrush(container_1.ParkCylinderSenorFault);
            row_4_column_2_FA.Fill = GetBrush(container_2.ParkCylinderSenorFault);
            row_4_column_3_FA.Fill = GetBrush(container_3.ParkCylinderSenorFault);
            row_4_column_4_FA.Fill = GetBrush(container_4.ParkCylinderSenorFault);
            row_4_column_5_FA.Fill = GetBrush(container_5.ParkCylinderSenorFault);
            row_4_column_6_FA.Fill = GetBrush(container_6.ParkCylinderSenorFault);

            row_5_column_1_FA.Fill = GetBrush(container_1.VLDSensorFault);
            row_5_column_2_FA.Fill = GetBrush(container_2.VLDSensorFault);
            row_5_column_3_FA.Fill = GetBrush(container_3.VLDSensorFault);
            row_5_column_4_FA.Fill = GetBrush(container_4.VLDSensorFault);
            row_5_column_5_FA.Fill = GetBrush(container_5.VLDSensorFault);
            row_5_column_6_FA.Fill = GetBrush(container_6.VLDSensorFault);

            row_6_column_1_FA.Fill = GetBrush(container_1.BSRSenorFault_1);
            row_6_column_2_FA.Fill = GetBrush(container_2.BSRSenorFault_1);
            row_6_column_3_FA.Fill = GetBrush(container_3.BSRSenorFault_1);
            row_6_column_4_FA.Fill = GetBrush(container_4.BSRSenorFault_1);
            row_6_column_5_FA.Fill = GetBrush(container_5.BSRSenorFault_1);
            row_6_column_6_FA.Fill = GetBrush(container_6.BSRSenorFault_1);

            row_7_column_1_FA.Fill = GetBrush(container_1.BSRSenorFault_2);
            row_7_column_2_FA.Fill = GetBrush(container_2.BSRSenorFault_2);
            row_7_column_3_FA.Fill = GetBrush(container_3.BSRSenorFault_2);
            row_7_column_4_FA.Fill = GetBrush(container_4.BSRSenorFault_2);
            row_7_column_5_FA.Fill = GetBrush(container_5.BSRSenorFault_2);
            row_7_column_6_FA.Fill = GetBrush(container_6.BSRSenorFault_2);

            row_8_column_1_FA.Fill = GetBrush(container_1.AirSpringOverflow_1);
            row_8_column_2_FA.Fill = GetBrush(container_2.AirSpringOverflow_1);
            row_8_column_3_FA.Fill = GetBrush(container_3.AirSpringOverflow_1);
            row_8_column_4_FA.Fill = GetBrush(container_4.AirSpringOverflow_1);
            row_8_column_5_FA.Fill = GetBrush(container_5.AirSpringOverflow_1);
            row_8_column_6_FA.Fill = GetBrush(container_6.AirSpringOverflow_1);

            row_9_column_1_FA.Fill = GetBrush(container_1.AirSpringOverflow_2);
            row_9_column_2_FA.Fill = GetBrush(container_2.AirSpringOverflow_2);
            row_9_column_3_FA.Fill = GetBrush(container_3.AirSpringOverflow_2);
            row_9_column_4_FA.Fill = GetBrush(container_4.AirSpringOverflow_2);
            row_9_column_5_FA.Fill = GetBrush(container_5.AirSpringOverflow_2);
            row_9_column_6_FA.Fill = GetBrush(container_6.AirSpringOverflow_2);

            row_10_column_1_FA.Fill = GetBrush(container_1.BSRLowA11);
            row_10_column_2_FA.Fill = GetBrush(container_2.BSRLow1);
            row_10_column_3_FA.Fill = GetBrush(container_3.BSRLow1);
            row_10_column_4_FA.Fill = GetBrush(container_4.BSRLow1);
            row_10_column_5_FA.Fill = GetBrush(container_5.BSRLow1);
            row_10_column_6_FA.Fill = GetBrush(container_6.BSRLowA11);

            row_11_column_1_FA.Fill = GetBrush(container_1.BCUFail_Serious);
            row_11_column_2_FA.Fill = GetBrush(container_2.BCUFail_Serious);
            row_11_column_3_FA.Fill = GetBrush(container_3.BCUFail_Serious);
            row_11_column_4_FA.Fill = GetBrush(container_4.BCUFail_Serious);
            row_11_column_5_FA.Fill = GetBrush(container_5.BCUFail_Serious);
            row_11_column_6_FA.Fill = GetBrush(container_6.BCUFail_Serious);

            row_12_column_1_FA.Fill = GetBrush(container_1.BCUFail_Middle);
            row_12_column_2_FA.Fill = GetBrush(container_2.BCUFail_Middle);
            row_12_column_3_FA.Fill = GetBrush(container_3.BCUFail_Middle);
            row_12_column_4_FA.Fill = GetBrush(container_4.BCUFail_Middle);
            row_12_column_5_FA.Fill = GetBrush(container_5.BCUFail_Middle);
            row_12_column_6_FA.Fill = GetBrush(container_6.BCUFail_Middle);

            row_13_column_1_FA.Fill = GetBrush(container_1.BCUFail_Slight);
            row_13_column_2_FA.Fill = GetBrush(container_2.BCUFail_Slight);
            row_13_column_3_FA.Fill = GetBrush(container_3.BCUFail_Slight);
            row_13_column_4_FA.Fill = GetBrush(container_4.BCUFail_Slight);
            row_13_column_5_FA.Fill = GetBrush(container_5.BCUFail_Slight);
            row_13_column_6_FA.Fill = GetBrush(container_6.BCUFail_Slight);

            row_14_column_1_FA.Fill = GetBrush(container_1.EmergencyBrakeFault);
            row_14_column_2_FA.Fill = GetBrush(container_2.EmergencyBrakeFault);
            row_14_column_3_FA.Fill = GetBrush(container_3.EmergencyBrakeFault);
            row_14_column_4_FA.Fill = GetBrush(container_4.EmergencyBrakeFault);
            row_14_column_5_FA.Fill = GetBrush(container_5.EmergencyBrakeFault);
            row_14_column_6_FA.Fill = GetBrush(container_6.EmergencyBrakeFault);

            row_15_column_1_FA.Fill = GetBrush(container_1.CanASPEnable);
            row_16_column_1_FA.Fill = GetBrush(container_1.BCPLowA);
            row_17_column_1_FA.Fill = GetBrush(container_1.BCPLowB);
            row_18_column_1_FA.Fill = GetBrush(container_1.BCPLowC);

            //第二列
            row_1_column_8_FA.Fill = GetBrush(container_1.SpeedSenorFault_1);
            row_1_column_9_FA.Fill = GetBrush(container_2.SpeedSenorFault_1);
            row_1_column_10_FA.Fill = GetBrush(container_3.SpeedSenorFault_1);
            row_1_column_11_FA.Fill = GetBrush(container_4.SpeedSenorFault_1);
            row_1_column_12_FA.Fill = GetBrush(container_5.SpeedSenorFault_1);
            row_1_column_13_FA.Fill = GetBrush(container_6.SpeedSenorFault_1);

            row_2_column_8_FA.Fill = GetBrush(container_1.SpeedSenorFault_2);
            row_2_column_9_FA.Fill = GetBrush(container_2.SpeedSenorFault_2);
            row_2_column_10_FA.Fill = GetBrush(container_3.SpeedSenorFault_2);
            row_2_column_11_FA.Fill = GetBrush(container_4.SpeedSenorFault_2);
            row_2_column_12_FA.Fill = GetBrush(container_5.SpeedSenorFault_2);
            row_2_column_13_FA.Fill = GetBrush(container_6.SpeedSenorFault_2);

            row_3_column_8_FA.Fill = GetBrush(container_1.WSPFault_1);
            row_3_column_9_FA.Fill = GetBrush(container_2.WSPFault_1);
            row_3_column_10_FA.Fill = GetBrush(container_3.WSPFault_1);
            row_3_column_11_FA.Fill = GetBrush(container_4.WSPFault_1);
            row_3_column_12_FA.Fill = GetBrush(container_5.WSPFault_1);
            row_3_column_13_FA.Fill = GetBrush(container_6.WSPFault_1);

            row_4_column_8_FA.Fill = GetBrush(container_1.WSPFault_2);
            row_4_column_9_FA.Fill = GetBrush(container_2.WSPFault_2);
            row_4_column_10_FA.Fill = GetBrush(container_3.WSPFault_2);
            row_4_column_11_FA.Fill = GetBrush(container_4.WSPFault_2);
            row_4_column_12_FA.Fill = GetBrush(container_5.WSPFault_2);
            row_4_column_13_FA.Fill = GetBrush(container_6.WSPFault_2);

            row_5_column_8_FA.Fill = GetBrush(container_1.CodeConnectorFault);
            row_5_column_9_FA.Fill = GetBrush(container_2.CodeConnectorFault);
            row_5_column_10_FA.Fill = GetBrush(container_3.CodeConnectorFault);
            row_5_column_11_FA.Fill = GetBrush(container_4.CodeConnectorFault);
            row_5_column_12_FA.Fill = GetBrush(container_5.CodeConnectorFault);
            row_5_column_13_FA.Fill = GetBrush(container_6.CodeConnectorFault);

            row_6_column_8_FA.Fill = GetBrush(container_1.AirSpringLimit);
            row_6_column_9_FA.Fill = GetBrush(container_2.AirSpringLimit);
            row_6_column_10_FA.Fill = GetBrush(container_3.AirSpringLimit);
            row_6_column_11_FA.Fill = GetBrush(container_4.AirSpringLimit);
            row_6_column_12_FA.Fill = GetBrush(container_5.AirSpringLimit);
            row_6_column_13_FA.Fill = GetBrush(container_6.AirSpringLimit);

            row_7_column_8_FA.Fill = GetBrush(container_1.BrakeNotRealease);
            row_7_column_9_FA.Fill = GetBrush(container_2.BrakeNotRealease);
            row_7_column_10_FA.Fill = GetBrush(container_3.BrakeNotRealease);
            row_7_column_11_FA.Fill = GetBrush(container_4.BrakeNotRealease);
            row_7_column_12_FA.Fill = GetBrush(container_5.BrakeNotRealease);
            row_7_column_13_FA.Fill = GetBrush(container_6.BrakeNotRealease);

            row_8_column_8_FA.Fill = GetBrush(container_1.BCPLowA11);
            row_8_column_9_FA.Fill = GetBrush(container_2.BCPLow1);
            row_8_column_10_FA.Fill = GetBrush(container_3.BCPLow1);
            row_8_column_11_FA.Fill = GetBrush(container_4.BCPLow1);
            row_8_column_12_FA.Fill = GetBrush(container_5.BCPLow1);
            row_8_column_13_FA.Fill = GetBrush(container_6.BCPLowA11);

            row_9_column_8_FA.Fill = GetBrush(container_1.SpeedDetection);
            row_10_column_8_FA.Fill = GetBrush(container_1.CanBusFail1);
            row_11_column_8_FA.Fill = GetBrush(container_1.CanBusFail2);
            row_12_column_8_FA.Fill = GetBrush(container_1.HardDifferent);
            row_13_column_8_FA.Fill = GetBrush(container_1.EventHigh);
            row_14_column_8_FA.Fill = GetBrush(container_1.EventMid);
            row_15_column_8_FA.Fill = GetBrush(container_1.EventLow);
        }

        private SolidColorBrush GetBrush(bool b)
        {
            return b == true ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.LightGray);
        }

        
        public void UpdateData(MainDevDataContains container_1, SliverDataContainer container_2, SliverDataContainer container_3, SliverDataContainer container_4, SliverDataContainer container_5, MainDevDataContains container_6)
        {
            this.container_1 = container_1;
            this.container_2 = container_2;
            this.container_3 = container_3;
            this.container_4 = container_4;
            this.container_5 = container_5;
            this.container_6 = container_6;
        }



        #endregion



        #region window method
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

        private void OverviewWindow_Closed(object sender, EventArgs e)
        {
            CloseWindowEvent?.Invoke(true, "overview");
        }
        #endregion
    }
}
