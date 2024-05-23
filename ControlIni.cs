using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace 任务状态
{
    class ControlIni
    {
        #region "声明变量"

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        #endregion
        public static void Write(string Key, string Value)
        {
            string strFilePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + "\\config.ini";//获取INI文件路径
            WritePrivateProfileString("参数", Key, Value, strFilePath);

        }
        public static string Read(string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            string strFilePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + "\\config.ini";//获取INI文件路径
            GetPrivateProfileString("参数", Key, "", temp, 1024, strFilePath);
            return temp.ToString();
        }
        public static Boolean ReadWrite(String key,ref string Value)
        {
            String r = Read(key);
            if (r=="")
            {
                Write(key,Value);
                return false;
            }else
            {
                Value = r;
                return true;
            }
        }
        public static Boolean ReadWrite(String key, ref int Value)
        {
            String r = Read(key);
            if (r == "")
            {
                Write(key, Value.ToString());
                return false;
            }
            else
            {
                Value =int.Parse(r);
                return true;
            }
        }
        public static void Save(Form form)
        {
            Dictionary<string, string> controlValues = new Dictionary<string, string>();
            foreach (Control control in form.Controls)
            {
                GetControlValues(control, controlValues);
            }
            foreach (var item in controlValues)
            {
                Write(item.Key, item.Value);
            }
        }

        public static void GetControlValues(Control control, Dictionary<string, string> controlValues)
        {
            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                controlValues.Add(textBox.Name, textBox.Text);
            }
            else if (control is CheckBox)
            {
                CheckBox checkBox = (CheckBox)control;
                controlValues.Add(checkBox.Name, checkBox.Checked.ToString());
            }
            else if (control is ComboBox)
            {
                ComboBox comboBox = (ComboBox)control;
                controlValues.Add(comboBox.Name, comboBox.Text.ToString());
                controlValues.Add(comboBox.Name+"_SelectedIndex", comboBox.SelectedIndex.ToString());
                String item = "\"";
                for(int i=0;i< comboBox.Items.Count;i++)
                {
                    //String ite = comboBox.Items[i].ToString();
                    //if (ite.Length >= 1)
                    //{
                    //    ite = ite.Replace("=", ":").Replace("\"", "");
                    //    ite = ite.Replace("\r", "").Replace("\n", "");
                    //    if (i < (comboBox.Items.Count - 1))
                    //    {
                    //        item += ite + "|";
                    //    }
                    //    else
                    //    {
                    //        item += ite + "\"";
                    //    }
                    //}
                    String ite = comboBox.Items[i].ToString();
                    if (i < (comboBox.Items.Count - 1))
                    {
                        item += ite + "|";
                    }
                    else
                    {
                        item += ite + "\"";
                    }
                }
                controlValues.Add(comboBox.Name + "_Items", item);
            }
            else if (control is RadioButton)
            {
                RadioButton radioButton = (RadioButton)control;
                controlValues.Add(radioButton.Name, radioButton.Checked.ToString());
            }

            // 递归调用获取子控件的值
            foreach (Control childControl in control.Controls)
            {
                GetControlValues(childControl, controlValues);
            }
        }
        public static void TraverseControls(Control control)
        {
            if (control is TextBox)
            {
                TextBox textBox = (TextBox)control;
                textBox.Text = ControlIni.Read(textBox.Name);
            }
            else if (control is ComboBox)
            {
                ComboBox comboBox = (ComboBox)control;
                comboBox.Text = ControlIni.Read(comboBox.Name);
                String[] items = ControlIni.Read(comboBox.Name + "_Items").Split('|');
                comboBox.Items.AddRange(items);
                int i = 0;
                if (int.TryParse(ControlIni.Read(comboBox.Name + "_SelectedIndex"), out i))
                {
                    if (i < comboBox.Items.Count)
                    {
                        comboBox.SelectedIndex = i;
                    }
                }
            }
            else if (control is CheckBox)
            {
                CheckBox checkBox = (CheckBox)control;
                bool v;
                if (Boolean.TryParse(ControlIni.Read(checkBox.Name), out v))
                {
                    checkBox.Checked = v;
                }
            }
            else if (control is RadioButton)
            {
                RadioButton radioButton = (RadioButton)control;
            }
            else
            {
            }
            foreach (Control childControl in control.Controls)
            {
                TraverseControls(childControl);
            }
        }
        public static void Load(Form form)
        {
            foreach (Control control in form.Controls)
            {
                TraverseControls(control);
            }
        }
    }
}
