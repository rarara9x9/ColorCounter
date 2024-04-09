using Microsoft.VisualBasic.Devices;
using System;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace ColorCounter
{
    public partial class MainForm : Form
    {
        private readonly string[] Header = { "0", "30", "60", "90", "120", "150", "180", "210", "240", "270", "300", "330", "W", "B" };
        private List<TextBox> textBoxCs;
        private List<TextBox> textBoxEs;
        private List<TextBox> textBoxNs;
        private List<Color> baseColor;
        private int count;
        private string imageFolder;
        private string TimeFolder;


        public class ItemSet
        {
            // DisplayMemberとValueMemberにはプロパティで指定する仕組み
            public String ItemDisp { get; set; }
            public int ItemValue { get; set; }

            // プロパティをコンストラクタでセット
            public ItemSet(int v, String s)
            {
                ItemDisp = s;
                ItemValue = v;
            }
        }

        public MainForm()
        {
            InitializeComponent();

            textBoxCs = new List<TextBox>() { this.textBoxc1, this.textBoxc2, this.textBoxc3, this.textBoxc4,
                                              this.textBoxc5, this.textBoxc6, this.textBoxc7, this.textBoxc8,
                                              this.textBoxc9, this.textBoxc10,this.textBoxc11,this.textBoxc12,
                                              this.textBoxc13,this.textBoxc14  };
            textBoxEs = new List<TextBox>() { this.textBoxE1, this.textBoxE2, this.textBoxE3, this.textBoxE4,
                                              this.textBoxE5, this.textBoxE6, this.textBoxE7, this.textBoxE8,
                                              this.textBoxE9, this.textBoxE10,this.textBoxE11,this.textBoxE12,
                                              this.textBoxE13,this.textBoxE14  };

            textBoxNs = new List<TextBox>() { this.textBoxN1, this.textBoxN2, this.textBoxN3, this.textBoxN4,
                                              this.textBoxN5, this.textBoxN6, this.textBoxN7, this.textBoxN8,
                                              this.textBoxN9, this.textBoxN10,this.textBoxN11,this.textBoxN12,
                                              this.textBoxN13,this.textBoxN14  };


            baseColor = new List<Color>() { Color.FromArgb(255, 0, 0), Color.FromArgb(255, 128, 0), Color.FromArgb(255, 255, 0), Color.FromArgb(128, 255, 0),
                                            Color.FromArgb(0, 255, 0), Color.FromArgb(0, 255, 128), Color.FromArgb(0, 255, 255), Color.FromArgb(0, 128, 255),
                                            Color.FromArgb(0, 0, 255), Color.FromArgb(128, 0, 255), Color.FromArgb(255, 0, 255), Color.FromArgb(255, 0, 128),
                                            Color.FromArgb(255, 255, 255), Color.FromArgb(0, 0, 0)};

            for (int i = textBoxCs.Count - 1; i >= 0; --i)
            {
                //textBoxCs[i].BackColor = baseColor[i];
                //textBoxEs[i].BackColor = baseColor[i];
                textBoxNs[i].BackColor = baseColor[i];
                textBoxNs[i].Text = Header[i];
            }
            this.textBoxN9.ForeColor = Color.FromArgb(255, 255, 255);
            this.textBoxN14.ForeColor = Color.FromArgb(255, 255, 255);
            this.textBox1.Text = "FileName\t" + String.Join("\t", Header) + "\r\n";
            count = 1;

            this.checkBox1.Checked = true;

            imageFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image");
            Folder_ReCreate();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsImage())
            {
                return;
            }

            double[] countColors = new double[textBoxCs.Count];

            Bitmap bmp = (Bitmap)Clipboard.GetImage();

            for (int x = bmp.Width - 1; x >= 0; --x)
            {
                for (int y = bmp.Height - 1; y >= 0; --y)
                {
                    Color c = bmp.GetPixel(x, y);

                    if (this.checkBox1.Checked && ((c.R == 0 && c.G == 0 && c.B == 0) || (c.R == 255 && c.G == 255 && c.B == 255)))
                    {
                        //純白純黒はカウント対象外
                        continue;
                    }

                    float brightness = c.GetBrightness();
                    float hue = c.GetHue();
                    float saturation = c.GetSaturation();

                    //前後15度を対象とするため、15を加算
                    //あふれ分はModをとって消し込む
                    int idx = (int)(((hue + 15.0) % 360.0) / 30.0);

                    if (Properties.Settings.Default.C1_Brightness <= brightness)
                    {
                        //白
                        ++countColors[textBoxCs.Count - 2];
                    }
                    else if (brightness <= Properties.Settings.Default.C2_Brightness)
                    {
                        //黒
                        ++countColors[textBoxCs.Count - 1];
                    }
                    else if (saturation <= Properties.Settings.Default.C3_Saturation && hue <= Properties.Settings.Default.C3_Hue)
                    {
                        //灰色
                        countColors[textBoxCs.Count - 1] += (1.0 - Properties.Settings.Default.C3_Value);
                        countColors[textBoxCs.Count - 2] += Properties.Settings.Default.C3_Value;
                    }
                    else if ((Properties.Settings.Default.C4_Brightness_Low <= brightness) && (brightness <= Properties.Settings.Default.C4_Brightness_High))
                    {
                        //相関色　純色
                        ++countColors[idx];
                    }
                    else if (saturation <= Properties.Settings.Default.C5_Saturation)
                    {
                        //黒白との混色
                        countColors[idx] += Properties.Settings.Default.C5_Value;
                        if (0.5 <= brightness)
                        {
                            countColors[textBoxCs.Count - 2] += (1.0 - Properties.Settings.Default.C5_Value);
                        }
                        else
                        {
                            countColors[textBoxCs.Count - 1] += (1.0 - Properties.Settings.Default.C5_Value);
                        }
                    }
                    else
                    {
                        //相関色との混色
                        countColors[idx] += Properties.Settings.Default.C6_Value;
                        if ((baseColor[idx].GetHue() - hue) <= 0)
                        {
                            //プラス側の相関色へ加算する。
                            countColors[(idx + 1) % 12] += (1.0 - Properties.Settings.Default.C6_Value);
                        }
                        else
                        {
                            //マイナス側の相関色へ加算する。
                            //マイナスは処理しずらいので加算＋Modで減算をする。
                            countColors[(idx + 11) % 12] += (1.0 - Properties.Settings.Default.C6_Value);
                        }
                    }
                }
            }
            ImageResize imageResize = new ImageResize();

            PictureBox1.Image = imageResize.ResizeImageWhileMaintainingAspectRatio(Clipboard.GetImage(), PictureBox1.Size.Width, PictureBox1.Size.Height);

            List<int> countColorsInt = countColors.Select(x => (int)x).ToList();
            int[] EnInt = new int[14];
            for (int i = textBoxCs.Count - 3; i >= 0; --i)
            {
                EnInt[i] = (countColorsInt[(i + 11) % 12] / 2) + countColorsInt[i] + (countColorsInt[(i + 1) % 12] / 2);
            }
            EnInt[12] = countColorsInt[12] * 2;
            EnInt[13] = countColorsInt[13] * 2;

            int sum = EnInt.Sum(x => x);

            for (int i = textBoxCs.Count - 1; i >= 0; --i)
            {
                textBoxCs[i].Text = countColorsInt[i].ToString();
                textBoxEs[i].Text = Math.Round(Math.Log((double)EnInt[i] / sum, 2.71828182845904) * -1, 3).ToString();
            }

            if (string.IsNullOrWhiteSpace(this.textBox1.Text))
            {
                Folder_ReCreate();
                count = 1;
            }

            string filename = string.Format(@"{0}\{1:D3}.png", TimeFolder, count);
            Clipboard.GetImage().Save(Path.Combine(imageFolder, filename), System.Drawing.Imaging.ImageFormat.Png);
            this.textBox1.Text += filename + "\t" + String.Join("\t", countColorsInt) + "\r\n";
            ++count;
        }

        private void Folder_ReCreate()
        {
            TimeFolder = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            this.textBoxOutput.Text = Path.Combine(imageFolder, TimeFolder) + @"\";
            this.textBox1.Text = "FileName\t" + String.Join("\t", Header) + "\r\n";
            if (Directory.Exists(textBoxOutput.Text))
            {
                Delete(textBoxOutput.Text);
            }
            else
            {
                Directory.CreateDirectory(textBoxOutput.Text);
            }
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        public static void Delete(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }
        }

        private void buttonOpenOutput_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("EXPLORER.EXE", String.Format(@"""{0}""", this.textBoxOutput.Text));
        }
    }
}