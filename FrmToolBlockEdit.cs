
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Bing.VisionProTool
{
    [Serializable]
    public partial class FrmToolBlockEdit : DockContent
    {
        private CogToolBlock InputTB { get; set; } = null;

        private string MFileName { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public FrmToolBlockEdit(CogToolBlock tb, string fileName, string taskName)
        {
            InitializeComponent();
            MFileName = fileName;
            InputTB = tb;
            TaskName = taskName;
            this.Text = taskName + " - Task";
            this.FormClosing += FrmToolBlockEdit_FormClosing;
            this.Load += FrmToolBlockEdit_Load;
        }
        public FrmToolBlockEdit() { }
        private void FrmToolBlockEdit_Load(object sender, EventArgs e)
        {
            cogToolBlockEditV21.Subject = InputTB;
            this.Icon = ((Icon)(new ComponentResourceManager(typeof(FrmToolBlockEdit)).GetObject("$this.Icon")));
            tsBtnStatus.BackColor = Color.Gray;
        }

        private void FrmToolBlockEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Dispose();
            //GC.Collect();
        }

        private void TsBtnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("是否保存当前 Vision Tool？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                InputTB = cogToolBlockEditV21.Subject;
                CogSerializer.SaveObjectToFile(InputTB, MFileName);
                tsBtnStatus.BackColor = Color.Lime;
            }
            else if (result == DialogResult.No)
            {
                return;
            }
        }
    }
}
