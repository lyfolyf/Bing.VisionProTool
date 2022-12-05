using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.IVisionTool;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.InteropServices;
using Cognex.VisionPro;
using System.ComponentModel;
using Cognex.VisionPro.ToolBlock;

namespace Bing.VisionProTool
{
    public class VisionProCamera : ICamera
    {
        public string CameraFilePath { get; set; }

        public double Exposure { get; set; }

        public double Gain { get; set; }

        public string Name { get; set; }

        private object image = null;
        private FrmView frmView = null;
        private FrmCameraEdit frmCameraEdit = null;
        private CogAcqFifoTool FifoTool = new CogAcqFifoTool();
        //private CogToolBlock toolBlock = new CogToolBlock();
        private double runTime;

        public object Image
        {
            get
            {
                return image;
            }
            private set
            {
                image = value;
            }
        }
        [Browsable(false)]
        public double RunTime
        {
            get
            {
                double time = runTime;
                runTime = 0;
                return Math.Round(time, 2);
            }
            set
            {
                runTime = value;
            }
        }

        public string Description { get; set; }

        public event ImageTakedEventHandler ImageTaked;

        private void OnImageTaked()
        {
            ImageTaked?.BeginInvoke(this, new ImageTakedEventArgs
            {
                Name = Name,
                Image = Image,
                RunTime = RunTime
            }, null, null);

            //if (frmView != null && frmView.IsDisposed)
            //{
            //    frmView.Image = (ICogImage)Image;
            //}

        }

        public DockContent GetFrmCamreaEdit()
        {
            if (frmCameraEdit == null || frmCameraEdit.IsDisposed)
            {
                frmCameraEdit = new FrmCameraEdit(FifoTool, CameraFilePath, Name);
            }

            return frmCameraEdit;
        }

        public virtual bool LoadCameraFile()
        {
            try
            {
                //toolBlock = CogSerializer.LoadObjectFromFile(CameraFilePath) as CogToolBlock;
                FifoTool = CogSerializer.LoadObjectFromFile(CameraFilePath) as CogAcqFifoTool;
                return true;
            }
            catch (Exception EX)
            {

                return false;
            }
        }

        public virtual RunResult Run()
        {
            try
            {
                FifoTool.Operator.OwnedExposureParams.Exposure = Exposure;
                FifoTool.Operator.OwnedContrastParams.Contrast = Gain;
                FifoTool.Run();
                if (FifoTool.RunStatus.Result == CogToolResultConstants.Accept)
                {
                    Image = FifoTool.OutputImage;
                    RunTime = FifoTool.RunStatus.TotalTime;

                    OnImageTaked();
                    return RunResult.Accept;
                }
                return RunResult.Warning;
            }
            catch (Exception)
            {

                return RunResult.Error;
            }
        }
        public virtual RunResult StartAcq()
        {
            try
            {
                //FifoTool.Operator.OwnedExposureParams.Exposure = Exposure;
                //FifoTool.Operator.OwnedContrastParams.Contrast = Gain;
                //FifoTool.Operator.OwnedStrobeParams.StrobeEnabled = true;
                FifoTool.Operator.Complete += Operator_Complete;
                //FifoTool.Run();
                
                return RunResult.Accept;
            }
            catch (Exception)
            {

                return RunResult.Error;
            }
        }
        int cou = 0;
        private void Operator_Complete(object sender, CogCompleteEventArgs e)
        {
            cou++;
            System.Diagnostics.Debug.WriteLine($"##############收到触发信号{cou}次##############\n");
            Image = FifoTool.OutputImage;
            OnImageTaked();
        }
        public virtual RunResult Run(Action action)
        {
            try
            {
                action();
                return Run();
            }
            catch (Exception)
            {

                return RunResult.Error;
            }
        }

        public DockContent GetFrmCamreaView()
        {
            if (frmView == null || frmView.IsDisposed)
            {
                frmView = new FrmView { Image = (ICogImage)image };
                frmView.IsAllowedLive = true;
                frmView.FiFo = FifoTool;
                frmView.Text = $"{Name} - Live";
            }
            return frmView;
        }

        public void StartLiveDisplay()
        {
            if (frmView == null || frmView.IsDisposed)
                return;
            frmView.StartLiveDisplay(FifoTool);
        }
        public void StopLiveDisplay()
        {
            if (frmView == null || frmView.IsDisposed)
                return;
            frmView.StopLiveDisplay();
        }
    }
}
