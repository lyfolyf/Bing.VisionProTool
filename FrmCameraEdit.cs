using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.Internal;
using WeifenLuo.WinFormsUI.Docking;

namespace Bing.VisionProTool
{
    public partial class FrmCameraEdit : DockContent
    {

        private CogAcqFifoTool FifoTool { get; set; } = new CogAcqFifoTool();
        private string MFileName { get; set; } = string.Empty;
        public string CameraName { get; set; } = string.Empty;
        public FrmCameraEdit(CogAcqFifoTool fifoTool, string filePath, string name)
        {
            InitializeComponent();
            FifoTool = fifoTool;
            MFileName = filePath;
            CameraName = name;
            Text = $"{CameraName} - Camera";
            cogAcqFifoEditV21.Subject = FifoTool;
            this.Load += FrmCameraEdit_Load;
        }

        private void FrmCameraEdit_Load(object sender, EventArgs e)
        {
            tsBtnStatus.BackColor = Color.Gray;
        }

        private void TsBtnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否保存当前 Vision Tool？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                FifoTool = cogAcqFifoEditV21.Subject;
                CogSerializer.SaveObjectToFile(FifoTool, MFileName);
                tsBtnStatus.BackColor = Color.Lime;
            }
            else if (result == DialogResult.No)
            {
                return;
            }
        }
    }
}
