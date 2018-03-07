using System;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COM3D2.PathsDatMixer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string CanonicalPath(string path)
        {
            string cpath = Path.GetFullPath(path);
            if (cpath.EndsWith("\\"))
            {
                cpath = cpath.Remove(cpath.Length - 1);
            }
            return cpath;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegistryKey keyCOM3D2 = Registry.CurrentUser.OpenSubKey("Software\\KISS\\カスタムオーダーメイド3D2");
            RegistryKey keyCM3D2 = Registry.CurrentUser.OpenSubKey("Software\\KISS\\カスタムメイド3D2");
            String pathCOM3D2 = (String)keyCOM3D2?.GetValue("InstallPath");
            String pathCM3D2 = (String)keyCM3D2?.GetValue("InstallPath");

            if (pathCOM3D2 == null)
            {
                MessageBox.Show("カスタムオーダーメイド3D2のインストールが確認できません。");
                return;
            }
            if (pathCM3D2 == null)
            {
                MessageBox.Show("カスタムメイド3D2のインストールが確認できません。");
                return;
            }

            textBox1.Text = CanonicalPath(pathCOM3D2);
            textBox2.Text = CanonicalPath(pathCM3D2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = dialog.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = dialog.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string pathCOM3D2 = textBox1.Text;
            string pathCM3D2 = textBox2.Text;

            try
            {
                Program.Result result = Program.Startup(pathCOM3D2, pathCM3D2);
                if (result != null)
                {
                    MessageBox.Show("マージ処理を完了しました。バックアップ: " + result.pathBackup);
                }

            } catch (IOException ex)
            {
                MessageBox.Show("paths.datファイルの検索に失敗しました。ディレクトリの指定先を確認してください。");
            }
        }
    }
}
