using Bing.IVisionTool;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Bing.VisionProTool
{
    public class VisionProCalibTool
    {

        private CogImage8Grey inputImage = new CogImage8Grey();
        private CogImage8Grey calibrationImage = new CogImage8Grey();
        private CogImage8Grey outputImage = new CogImage8Grey();
        [Browsable(false)]
        public CogCalibCheckerboardTool CalibCheckerboardTool { get; set; } = new CogCalibCheckerboardTool();
        [Browsable(false)]
        public bool IsLoaded { get; set; } = false;

        [Category("校正模式"), DisplayName("校正类型")]
        public CogCalibFixComputationModeConstants ComputationMode
        {
            get
            {
                return CalibCheckerboardTool.Calibration.ComputationMode;
            }
            set
            {
                if (IsLoaded)
                {
                    CalibCheckerboardTool.Calibration.ComputationMode = value;
                }
            }
        }

        [Category("校正板"), DisplayName("基准符号")]
        public CogCalibCheckerboardFiducialConstants FiducialMark
        {
            get
            {
                return CalibCheckerboardTool.Calibration.FiducialMark;
            }
            set
            {
                if (IsLoaded)
                {
                    CalibCheckerboardTool.Calibration.FiducialMark = value;
                }
            }
        }
        [Category("校正模式"), DisplayName("要计算的自由度")]
        public CogCalibCheckerboardDOFConstants DOFsToCompute
        {
            get
            {
                return CalibCheckerboardTool.Calibration.DOFsToCompute;
            }
            set
            {
                if (IsLoaded)
                {
                    CalibCheckerboardTool.Calibration.DOFsToCompute = value;
                }
            }
        }
        [Category("校正板"), DisplayName("特征搜寻器")]
        public CogCalibCheckerboardFeatureFinderConstants FeatureFinder
        {
            get
            {
                return CalibCheckerboardTool.Calibration.FeatureFinder;
            }
            set
            {
                if (IsLoaded)
                {
                    CalibCheckerboardTool.Calibration.FeatureFinder = value;
                }
            }
        }
        [Category("校正板"), DisplayName("块尺寸 X(mm)")]
        public double PhysicalTileSizeX
        {
            get
            {
                return CalibCheckerboardTool.Calibration.PhysicalTileSizeX;
            }
            set
            {
                if (IsLoaded)
                {
                    CalibCheckerboardTool.Calibration.PhysicalTileSizeX = value;
                }
            }
        }
        [Category("校正板"), DisplayName("块尺寸 Y(mm)")]
        public double PhysicalTileSizeY
        {
            get
            {
                return CalibCheckerboardTool.Calibration.PhysicalTileSizeY;
            }
            set
            {
                if (IsLoaded)
                {
                    CalibCheckerboardTool.Calibration.PhysicalTileSizeY = value;
                }
            }
        }

        [Category("坐标系"), DisplayName("切换左右手坐标系")]
        public bool SwapCalibratedHandedness
        {
            get
            {
                return CalibCheckerboardTool.Calibration.SwapCalibratedHandedness;
            }
            set
            {
                if (IsLoaded)
                {
                    CalibCheckerboardTool.Calibration.SwapCalibratedHandedness = value;
                }
            }
        }

        [Category("校正结果"), DisplayName("已校正")]
        public bool IsCalibrated
        {
            get
            {
                return CalibCheckerboardTool.Calibration.Calibrated;
            }
        }
        [Browsable(false)]
        public CogImage8Grey InputImage
        {
            get
            {
                return inputImage;
            }
            set
            {
                //if (value != null && value.GetType() == typeof(Bitmap))
                //{
                //    inputImage = new CogImage8Grey((Bitmap)value);
                //}
                //else if (value.GetType() == typeof(CogImage8Grey))
                //{
                //    inputImage = (CogImage8Grey)value;
                //}
                //else
                //{
                //    inputImage = new CogImage8Grey();
                //}
                inputImage = value;
            }
        }
        [Browsable(false)]
        public CogImage8Grey OutputImage
        {
            get
            {
                return outputImage;
            }
            private set
            {
                //outputImage = (CogImage8Grey)value;
                outputImage = value;
            }
        }
        [Browsable(false)]
        public CogImage8Grey CalibrationImage
        {
            private get
            {
                return calibrationImage;
            }
            set
            {
                //if (value != null && value.GetType() == typeof(Bitmap))
                //{
                //    calibrationImage = new CogImage8Grey((Bitmap)value);
                //}
                //else if (value.GetType() == typeof(CogImage8Grey))
                //{
                //    calibrationImage = (CogImage8Grey)value;
                //}
                //else
                //{
                //    calibrationImage = new CogImage8Grey();
                //}
                calibrationImage = value;
            }
        }
        [Category("信息"), DisplayName("路径")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CalibFilePath { get; set; }

        public RunResult Calibretion()
        {
            try
            {
                CalibCheckerboardTool.Calibration.CalibrationImage = (CogImage8Grey)CalibrationImage;
                CalibCheckerboardTool.Calibration.Uncalibrate();
                CalibCheckerboardTool.Calibration.Calibrate();
                if (CalibCheckerboardTool.Calibration.Calibrated)
                    return RunResult.Accept;
                else
                    return RunResult.Warning;
            }
            catch
            {
                return RunResult.Error;
            }
        }

        public RunResult Run()
        {
            try
            {
                if (IsCalibrated)
                {
                    CalibCheckerboardTool.InputImage = (CogImage8Grey)InputImage;
                    CalibCheckerboardTool.Run();
                    OutputImage = CalibCheckerboardTool.OutputImage as CogImage8Grey;
                    return RunResult.Accept;
                }
                else
                {
                    return RunResult.Warning;
                }
            }
            catch
            {
                return RunResult.Error;
            }

        }

        public Control GetControl()
        {
            CogCalibCheckerboardEditV2 cogCalibCheckerboardEditV2 = new CogCalibCheckerboardEditV2
            {
                Subject = CalibCheckerboardTool
            };
            return cogCalibCheckerboardEditV2;

        }

        public bool LoadCalibFile()
        {
            try
            {
                CalibCheckerboardTool = CogSerializer.LoadObjectFromFile(CalibFilePath) as CogCalibCheckerboardTool;
                IsLoaded = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveCalibFile()
        {
            try
            {
                CogSerializer.SaveObjectToFile(CalibCheckerboardTool, CalibFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
