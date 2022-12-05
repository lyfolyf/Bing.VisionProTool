using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using Bing.IVisionTool;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using WeifenLuo.WinFormsUI.Docking;

namespace Bing.VisionProTool
{
    public class VisionProTask : IVisionTask
    {
        [Category("编辑"), DisplayName("名称")]
        public string Name { get; set; } = "VPTask";
        [Category("编辑"), DisplayName("描述")]
        [Editor(typeof(PropertyGridRichText), typeof(UITypeEditor))]
        public string Description { get; set; }
        [Category("编辑"), DisplayName("路径")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TaskFilePath { get; set; }
        public CogToolBlock GuideIns { get; set; } = new CogToolBlock();

        private CogImage8Grey image = new CogImage8Grey();
        public int TaskIndex { get; set; } = 1;
        [Browsable(false)]
        public object Image
        {
            get
            {
                return image;
            }
            set
            {
                if (value != null && value.GetType() == typeof(Bitmap))
                {
                    image = new CogImage8Grey((Bitmap)value);
                }
                else if (value.GetType() == typeof(CogImage8Grey))
                {
                    image = (CogImage8Grey)value;
                }
                else
                {
                    image = new CogImage8Grey();
                }
            }
        }

        private FrmView frmView;

        private bool isShow = false;

        FrmToolBlockEdit frmToolBlcokEdit;
        [Browsable(false)]
        public ICogRecord Record { get; set; }

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

        public VisionProTask()
        {
            GuideIns.Name = Name;
            frmToolBlcokEdit = new FrmToolBlockEdit();
        }

        [Category("终端"), DisplayName("输入"), ReadOnly(true)]
        public List<Terminal> Inputs { get; set; } = new List<Terminal>();
        [Category("终端"), DisplayName("输出"), ReadOnly(true)]
        public List<Terminal> Outputs { get; set; } = new List<Terminal>();

        private double runTime = 0;

        public event RunCompletedEventHandler RunCompleted;

        public void OnRunCompleted()
        {
            //RunCompleted?.Invoke(this, new RunningCompletedEventArgs());
            RunCompleted?.BeginInvoke(this, new RunningCompletedEventArgs
            {
                Name = Name,
                Inputs = Inputs,
                Outputs = Outputs,
                RunTime = RunTime
            }, null, null);
            SetView();
        }

        public DockContent GetFrmTaskEdit()
        {
            if (frmToolBlcokEdit.IsDisposed || frmToolBlcokEdit.Name == string.Empty)
            {
                frmToolBlcokEdit = new FrmToolBlockEdit(GuideIns, TaskFilePath, Name);
            }

            return frmToolBlcokEdit;
        }

        public bool LoadTaskFile()
        {
            try
            {
                GuideIns = CogSerializer.LoadObjectFromFile(TaskFilePath) as CogToolBlock;
                Inputs.Clear();
                for (int i = 0; i < GuideIns.Inputs.Count; i++)
                {
                    Inputs.Add(new Terminal
                    {
                        Id = i,
                        Name = GuideIns.Inputs[i].Name,
                        Value = GuideIns.Inputs[i].Value,
                        ValueType = GuideIns.Inputs[i].ValueType
                    });
                }
                realValue = new double[GuideIns.Outputs.Count];
                realValue2 = new double[GuideIns.Outputs.Count];
                realValue3 = new double[GuideIns.Outputs.Count];
                realValue4 = new double[GuideIns.Outputs.Count];
                GetOutputs();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        double[] realValue = null;
        double[] realValue2 = null;
        double[] realValue3 = null;
        double[] realValue4 = null;
        private void GetOutputs()
        {
            Outputs.Clear();
            for (int i = 0; i < GuideIns.Outputs.Count; i++)
            {
                try
                {
                    CogToolBlockTerminal t = GuideIns.Outputs[i] as CogToolBlockTerminal;
                    if (t.ValueType == typeof(double))
                    {
                        string[] strs = t.Description.Split('#');
                        double adjustCF = 0;
                        double exceptValue;
                        double tolMax;
                        double tolMin;
                        int flawType;
                        string description;
                        double adjustCF2 = 0;
                        double adjustCF3 = 0;
                        double adjustCF4 = 0;
                        if (strs.Length == 6)
                        {
                            adjustCF = Convert.ToDouble(strs[0]);
                            exceptValue = Convert.ToDouble(strs[1]);
                            tolMax = Convert.ToDouble(strs[2]);
                            tolMin = Convert.ToDouble(strs[3]);
                            flawType = Convert.ToInt32(strs[4]);
                            description = strs[5];
                            adjustCF2 = 1;
                            adjustCF3 = 1;
                            adjustCF4 = 1;
                        }
                        else if (strs.Length == 9)
                        {
                            adjustCF = Convert.ToDouble(strs[0]);
                            exceptValue = Convert.ToDouble(strs[1]);
                            tolMax = Convert.ToDouble(strs[2]);
                            tolMin = Convert.ToDouble(strs[3]);
                            flawType = Convert.ToInt32(strs[4]);
                            description = strs[5];
                            adjustCF2 = Convert.ToDouble(strs[6]);
                            adjustCF3 = Convert.ToDouble(strs[7]);
                            adjustCF4 = Convert.ToDouble(strs[8]);
                        }
                        else
                        {
                            adjustCF = 1;
                            exceptValue = 0;
                            tolMax = 0.1;
                            tolMin = -0.1;
                            flawType = 0;
                            description = "待描述";
                            adjustCF2 = 1;
                            adjustCF3 = 1;
                            adjustCF4 = 1;
                        }
                        string str = t.Value.ToString();
                        bool res = false;
                        switch (TaskIndex)
                        {
                            case 1:
                                realValue[i] = adjustCF * Convert.ToDouble(str);
                                res = (realValue[i] > exceptValue + tolMin) && (realValue[i] < exceptValue + tolMax);
                                break;
                            case 2:
                                realValue2[i] = adjustCF2 * Convert.ToDouble(str);
                                res = (realValue2[i] > exceptValue + tolMin) && (realValue2[i] < exceptValue + tolMax);
                                break;
                            case 3:
                                realValue3[i] = adjustCF3 * Convert.ToDouble(str);
                                res = (realValue3[i] > exceptValue + tolMin) && (realValue3[i] < exceptValue + tolMax);
                                break;
                            case 4:
                                realValue4[i] = adjustCF4 * Convert.ToDouble(str);
                                res = (realValue4[i] > exceptValue + tolMin) && (realValue4[i] < exceptValue + tolMax);
                                break;
                        }
                        //realValue = adjustCF * Convert.ToDouble(str);
                        Outputs.Add(new Terminal
                        {
                            Id = i,
                            Name = t.Name,
                            Value = realValue[i],
                            Value2 = realValue2[i],
                            Value3 = realValue3[i],
                            Value4 = realValue4[i],
                            ValueType = t.ValueType,
                            AdjustCF = adjustCF,
                            ExceptValue = exceptValue,
                            TolMax = tolMax,
                            TolMin = tolMin,
                            Result = res,
                            FlawType = flawType,
                            Description = description,
                            AdjustCF2 = adjustCF2,
                            AdjustCF3 = adjustCF3,
                            AdjustCF4 = adjustCF4,
                        });
                    }
                    if (t.ValueType == typeof(List<>))
                    {
                        Outputs.Add(new Terminal
                        {
                            Id = i,
                            Name = GuideIns.Outputs[i].Name,
                            Value = GuideIns.Outputs[i].Value,
                            ValueType = GuideIns.Outputs[i].ValueType
                        });
                    }
                }
                catch(Exception ex)
                {
                }
            }
        }
        public void SetOutput()
        {
            for (int i = 0; i < Outputs.Count; i++)
            {
                for (int j = 0; i < GuideIns.Outputs.Count; j++)
                {
                    CogToolBlockTerminal t = GuideIns.Outputs[j] as CogToolBlockTerminal;
                    if(Outputs[i].Name == t.Name)
                    {
                        string dsp = $"{Outputs[i].AdjustCF}#{Outputs[i].ExceptValue}#{Outputs[i].TolMax}#{Outputs[i].TolMin}#{Outputs[i].FlawType}#{Outputs[i].Description}#{Outputs[i].AdjustCF2}#{Outputs[i].AdjustCF3}#{Outputs[i].AdjustCF4}";
                        (GuideIns.Outputs[j] as CogToolBlockTerminal).Description = dsp;
                        break;
                    }
                }
            }
        }
        public virtual RunResult Run()
        {
            try
            {

                GuideIns?.Run();
                runTime = GuideIns.RunStatus.TotalTime;
                if (GuideIns.RunStatus.Result == CogToolResultConstants.Accept)
                {
                    Record = GuideIns.CreateLastRunRecord();
                    GetOutputs();
                    OnRunCompleted();
                    return RunResult.Accept;
                }
                else
                {
                    Record = GuideIns.CreateLastRunRecord();
                    GetOutputs();
                    OnRunCompleted();
                    return RunResult.Warning;
                }
            }
            catch (Exception)
            {

                return RunResult.Error;
            }
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

        public bool SaveTaskFile()
        {
            try
            {
                CogSerializer.SaveObjectToFile(GuideIns, TaskFilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public T GetInsValue<T>(string key)
        {
            if (GuideIns.Outputs.Contains(key))
                return (T)GuideIns.Outputs[key].Value;
            else
                return default(T);
        }

        public RunResult SetInsValue<T>(string key, T value)
        {
            try
            {
                if (GuideIns.Inputs.Contains(key))
                    GuideIns.Inputs[key].Value = value;
                return RunResult.Accept;
            }
            catch (Exception)
            {
                return RunResult.Error;
            }
        }
        private void SetView()
        {
            try
            {
                if (isShow)
                {
                    if (frmView == null || frmView.IsDisposed)
                    {
                        frmView = new FrmView();
                        frmView.Disposed += FrmView_Disposed;
                        frmView.FormClosing += FrmView_FormClosing;
                    }
                    //Image = GuideIns.Outputs["OutputImage"].Value;//测试使用
                    CogImage8Grey image = (CogImage8Grey)Image;
                    ICogRecord record = (ICogRecord)Record;
                    frmView.Image = image;
                    frmView.Record = record;
                }
            }
            catch { }
        }

        private void FrmView_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            isShow = false;
        }

        private void FrmView_Disposed(object sender, EventArgs e)
        {
            isShow = false;
        }

        public virtual void View()
        {
            isShow = true;
            SetView();
            frmView.TaskName = Name;
            frmView.Text = this.Name + " - Display";
            frmView.Show();
        }
        public void DisplayBitmap(Bitmap bitmap)
        {
            Cognex.VisionPro.ICogImage image = new Cognex.VisionPro.CogImage8Grey(bitmap);
            frmView.Image = image;
            frmView.Fit();
        }
        public virtual DockContent GetFrmTaskView()
        {
            isShow = true;
            SetView();
            frmView.TaskName = Name;
            frmView.Text = this.Name + " - Display";
            return frmView;
        }
    }
}
