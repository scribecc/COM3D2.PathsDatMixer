using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace COM3D2.PathsDatMixer
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static Result Startup(string pathCOM3D2, string pathCM3D2)
        {
            Result result;

            String fullPathCOM3D2 = Path.Combine(pathCOM3D2, "GameData\\paths.dat");
            while (true)
            {
                String fullPathCOM3D2Backup = fullPathCOM3D2 + "." + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                if (!File.Exists(fullPathCOM3D2Backup))
                {
                    File.Copy(fullPathCOM3D2, fullPathCOM3D2Backup);
                    result = new Result(fullPathCOM3D2Backup);
                    break;
                }
                Thread.Sleep(1000);             
            }

            PathsDatFile fileCOM3D2 = new PathsDatFile();
            fileCOM3D2.Load(fullPathCOM3D2);

            PathsDatFile fileCM3D2 = new PathsDatFile();
            fileCM3D2.Load(Path.Combine(pathCM3D2, "GameData\\paths.dat"));

            fileCOM3D2.Merge(fileCM3D2);
            fileCOM3D2.Save(fullPathCOM3D2);

            return result;
        }

        public class Result
        {
            private string _pathBackup;

            public string pathBackup { get { return _pathBackup; } }

            public Result(string backup)
            {
                this._pathBackup = backup;
            }
        }

        class PathsDatFile
        {
            Int32 code;
            String[] paths;

            public void Load(String fname)
            {
                FileStream fs = new FileStream(fname, FileMode.Open);
                BinaryReader binr = new BinaryReader(fs);
                binr.BaseStream.Seek(12, SeekOrigin.Begin);

                this.code = binr.ReadInt32();
                Int32 count = binr.ReadInt32();

                String[] paths = new String[count];
                for (int i = 0; i < count; i++)
                {
                    byte b = binr.ReadByte();
                    char[] s = binr.ReadChars(b);

                    paths[i] = new String(s);
                }
                binr.Close();
                fs.Close();
                this.paths = paths;
            }

            public void Save(String fname)
            {
                FileStream fs = new FileStream(fname, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                BinaryWriter binw = new BinaryWriter(fs);

                String str = "CM3D2_PATHS";
                binw.Write((byte)str.Length);
                binw.Write((char[])str.ToCharArray());

                binw.Write(this.code);
                binw.Write((Int32)paths.Length);
                for (int i = 0; i < paths.Length; i++)
                {
                    binw.Write((byte)paths[i].Length);
                    binw.Write((char[])paths[i].ToCharArray());
                }
                binw.Close();
                fs.Close();
            }

            public void Merge(PathsDatFile file)
            {
                String[] paths0 = this.paths;
                String[] paths1 = file.paths;

                IEnumerable<String> newpaths = paths0.Union(paths1);
                paths0 = newpaths.ToArray<String>();
                Array.Sort(paths0);

                this.paths = paths0;
            }
        }
    }
}
