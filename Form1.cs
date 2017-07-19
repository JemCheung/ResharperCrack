using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private static string serverName = "tjserver";
        private static string instsrv = Application.StartupPath + @"\instsrv.exe";
        private static string ide = Application.StartupPath;
        private static string srvany = Application.StartupPath + @"\srvany.exe";
        private static string reg = Application.StartupPath;
        public Form1()
        {
            InitializeComponent();
            this.textBox1.Text = Application.StartupPath;
            this.textBox2.Text = Application.StartupPath;
            this.textBox3.Text = Application.StartupPath;
            this.textBox6.Text = serverName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            instsrv = path.SelectedPath + @"\instsrv.exe";
            this.textBox1.Text = path.SelectedPath;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            ide = path.SelectedPath;
            this.textBox2.Text = path.SelectedPath;
        }
        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        static bool RunCmd(string cmdExe, string cmdStr)
        {
            bool result = false;
            try
            {
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(str);
                    myPro.StandardInput.AutoFlush = true;
                    myPro.WaitForExit();

                    result = true;
                }
            }
            catch
            {

            }
            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var path = ide.Replace(@"\", @"\\\\");
            reg = $@"
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{serverName}\Parameters]
""Application""=""{path}\\\\IntelliJIDEALicenseServer_windows_amd64.exe""
""AppDirectory"" = ""{path}""
""AppParameters"" = """"
";
            this.textBox4.Text = reg;
            RegistryKey rgK = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serverName}", true);
            if (rgK == null)
            {
                MessageBox.Show($@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{serverName} 注册表不存在
请用instsrv 重新生成服务");
                return;
            }
            if (rgK.OpenSubKey("Parameters") == null)
            {
                rgK = rgK.CreateSubKey("Parameters");
            }
            else
            {
                rgK = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serverName}", true);
            }
            if (rgK != null)
            {
                rgK.SetValue("Application", $@"{ide}\IntelliJIDEALicenseServer_windows_amd64.exe");
                rgK.SetValue("AppDirectory", $@"{ide}");
                rgK.SetValue("AppParameters", @"");
            }
            Debug.WriteLine(reg);
            this.textBox5.Text = @"127.0.0.1:41017";
            MessageBox.Show(
                @"resharper本地服务器搭建成功，在vs resharper插件->help->liscense information下选用use liscense server并添加服务器地址为
127.0.0.1:41017
然后退到上一界面启用该服务器地址即可，若仍有疑问请查看帮助文档或者联系管理员 辣鸡二路");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            srvany = path.SelectedPath + @"\srvany.exe";
            this.textBox3.Text = path.SelectedPath;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var cmd = RunCmd(instsrv, $@"{serverName}" + " " + srvany);
            RegistryKey rgK = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serverName}", true);

            MessageBox.Show(rgK != null ? "服务注册表生成成功，可以搭建服务器（只需生成一次以后可以直接搭建服务器）" : "服务 注册表生成失败(可尝试修改新的服务名重试)，请联系管理员（辣鸡二路）");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.ShowDialog();
            srvany = path.SelectedPath + @"\srvany.exe";
            instsrv = path.SelectedPath + @"\instsrv.exe";
            ide = path.SelectedPath;
            this.textBox3.Text = path.SelectedPath;
            this.textBox2.Text = path.SelectedPath;
            this.textBox1.Text = path.SelectedPath;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            serverName = textBox6.Text;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show($@"
本程序需以管理员模式启动
搭建本地reshaper服务器服务说明
管理员权限运行下面的指令
instsrv {serverName}(服务名) {{C:\Windows\System32\srvany.exe（程序的绝对路径）}}

执行成功之后 进入注册表编辑器（运行指令：RegEdit）
可以看到在该路径下生成
[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\tjserver\Parameters]
然后将以下指令保存为注册表（如文件importRegedit.reg）并执行（路径自己调整为IntelliJIDEALicenseServer_windows_amd64.exe的实际路径）
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\tjserver\Parameters]
""Application""=""C:\\\\Users\\\\greedy\\\\Dropbox\\\\破解\\\\IntelliJIDEALicenseServer_windows_amd64.exe""
""AppDirectory""=""C:\\\\Users\\\\greedy\\\\Dropbox\\\\破解""
""AppParameters""=""""

然后在vs中打开resharper注册添加服务器地址127.0.0.1:41017保存并启用即可（无视黄色叹号警告，这时候已经注册完毕了）
可以在右键我的电脑->管理->服务找到{serverName}启动服务并右键属性设置自动启动，登录->勾选允许与桌面交互 则可在任务管理器->服务中看到该服务
");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!File.Exists(instsrv))
            {
                MessageBox.Show(@"当前路径不存在 instsrv.exe 文件请添加文件到相同目录");
            }
            else if (!File.Exists(srvany))
            {
                MessageBox.Show(@"当前路径不存在 srvany.exe 文件请添加文件到相同目录");
            }
            else if (!File.Exists($@"{ide}\IntelliJIDEALicenseServer_windows_amd64.exe"))
            {
                MessageBox.Show(@"当前路径不存在 IntelliJIDEALicenseServer_windows_amd64.exe 文件请添加文件到相同目录");
            }
            else
            {
                if (Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serverName}", true) == null)
                {
                    var cmd = RunCmd(instsrv, $@"{serverName}" + " " + srvany);
                    RegistryKey rgK = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serverName}", true);
                    if (rgK == null)
                    {
                        MessageBox.Show("服务 注册表生成失败(可尝试修改新的服务名重试)，请联系管理员（辣鸡二路）");
                        return;
                    }
                }
                if (CreateServer())
                {
                    this.textBox5.Text = @"127.0.0.1:41017";
                    MessageBox.Show(SetConfig() ? @"配置成功请重启vs查看" : "配置失败请联系管理员");
                }
            }
        }
        public bool CreateServer()
        {
            var path = ide.Replace(@"\", @"\\\\");
            reg = $@"
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{serverName}\Parameters]
""Application""=""{path}\\\\IntelliJIDEALicenseServer_windows_amd64.exe""
""AppDirectory"" = ""{path}""
""AppParameters"" = """"
";
            this.textBox4.Text = reg;
            RegistryKey rgK = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serverName}", true);
            if (rgK == null)
            {
                MessageBox.Show($@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{serverName} 注册表不存在
请用instsrv 重新生成服务");
                return false;
            }
            if (rgK.OpenSubKey("Parameters") == null)
            {
                rgK = rgK.CreateSubKey("Parameters");
            }
            else
            {
                rgK = Registry.LocalMachine.OpenSubKey($@"SYSTEM\CurrentControlSet\Services\{serverName}", true);
            }
            if (rgK != null)
            {
                rgK.SetValue("Application", $@"{ide}\IntelliJIDEALicenseServer_windows_amd64.exe");
                rgK.SetValue("AppDirectory", $@"{ide}");
                rgK.SetValue("AppParameters", @"");
            }

            return true;
        }

        public bool SetConfig()
        {
            RegistryKey rgK = Registry.CurrentUser.OpenSubKey($@"Software\JetBrains\Shared\vAny", true);
            if (rgK == null)
            {
                MessageBox.Show($@"系统未安装 resharper 请安装后重试");
                return false;
            }
            rgK.SetValue("JBAccount", @"Nvn8jTCB1iOJDI+0f2HeqBnS1zS25MbQ5/O2MFKqwVTdTuCPoQUvaBwdSFw8zB+NruERiKypT1Kr1C5UKl/xXgH7bAu+X1TpQGFx0L6orlk/nwmnlTCgZ0g05yyJs0URdpxjEdXomvdJXu+dToL/rA==");
            rgK.SetValue("JBAccountSettings", @"vQQ16b3NgKI5adjFr6HG");
            rgK.SetValue("LicenseServer", @"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");
            rgK.SetValue("LicenseServerSettings", @"tdTn2BPVe66rXLAgEgAfT6L7coedbeo/HdSTC2SdT5XmX/vOpLuAXM6Mwygq1Njc78SW7Kd8v6+1AgU1Uwt1eOvHhcxAdXNehLGeAKNXTgbZwo65CT1k4w==");
            rgK.SetValue("UserLicenses", @"7qlO8YKA6IE=");
            rgK.SetValue("UserLicenseSettings", @"tdTn2BPVe65k+qLR1XHSM/zsbr0ujtxGgwycBkNobsc/k3ELEgYhAMvik3Z9wFdvHnCAGI4j2cDNUsWLcZsyGVqyaHL8g8Uh9z5EZo7RWkTPXLLqhnlUCClBiGrTw5Sk6lcMxNQ0eIlGBy36C/irqg==");
            rgK.SetValue("VKR", @"9m9rRVYdehS+B+UrmUUFi68WXhFvlpQu+K7VUGJMZAXMb8vF8//3uSAT7lENWkjW");
            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
