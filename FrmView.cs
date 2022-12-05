
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Bing.VisionProTool
{
    [Serializable]
    public partial class FrmView : DockContent, ISerializable
    {
        public string TaskName { get; set; } = string.Empty;
        public bool IsAllowedLive { get; set; } = false;
        public Cognex.VisionPro.CogAcqFifoTool FiFo { get; set; } = null;
        public FrmView()
        {
            InitializeComponent();
            this.Load += FrmView_Load;
        }

        private void FrmView_Load(object sender, EventArgs e)
        {
            if (IsAllowedLive)
            {
                btnLive.Visible = true;
            }
            else
            {
                btnLive.Visible = false;
            }
        }

        public Cognex.VisionPro.ICogImage Image
        {
            get { return cogRecordDisplay1.Image; }
            set { cogRecordDisplay1.Image = value; }
        }

        public void DisplayBitmap(Bitmap bitmap)
        {
            Cognex.VisionPro.ICogImage image = new Cognex.VisionPro.CogImage8Grey(bitmap);
            cogRecordDisplay1.Image = image;
            cogRecordDisplay1.Fit();
        }
        public Cognex.VisionPro.ICogRecord Record
        {
            get
            { return cogRecordDisplay1.Record; }
            set
            {
                cogRecordDisplay1.Record = value;
                cogRecordDisplay1.BackColor = Color.Gray;
            }
        }
        public void StartLiveDisplay(Cognex.VisionPro.CogAcqFifoTool fifo)
        {
            cogRecordDisplay1.StartLiveDisplay(fifo);
        }
        public void StopLiveDisplay()
        {
            cogRecordDisplay1.StopLiveDisplay();
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Image", Image);
            info.AddValue("Record", Record);
        }

        private void BtnLive_Click(object sender, EventArgs e)
        {
            try
            {
                if (FiFo == null)
                {
                    return;
                }
                if (btnLive.Text == "Live")
                {
                    StartLiveDisplay(FiFo);
                    btnLive.Text = "Stop";
                    btnLive.BackColor = Color.LightGreen;
                }
                else
                {
                    StopLiveDisplay();
                    btnLive.Text = "Live";
                    btnLive.BackColor = Color.Silver;
                }
            }
            catch
            {
                return;
            }
        }

        internal void Fit()
        {
            cogRecordDisplay1.Fit();
        }
    }
}
