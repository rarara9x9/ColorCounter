using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorCounter
{
    internal class ImageResize
    {

        /// <summary>
        /// 画像ファイルのアスペクト比を維持してサイズを変更します
        /// </summary>
        /// <param name="sourceFile">サイズ変更する画像ファイル</param>
        /// <param name="imageFormat">画像ファイル形式</param>
        /// <param name="width">変更する幅</param>
        /// <param name="height">変更する高さ</param>
        public Image ResizeImageWhileMaintainingAspectRatio(Image sourceImage,
                                                            int width,
                                                            int height)
        {
            // 変更倍率を取得する
            float scale = Math.Min((float)width / (float)sourceImage.Width, (float)height / (float)sourceImage.Height);

            if (scale >= 1.0)
            {
                return sourceImage;
            }
            // サイズ変更した画像を作成する
            using (Bitmap bitmap = new Bitmap(width, height))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // 変更サイズを取得する
                int widthToScale = (int)(sourceImage.Width * scale);
                int heightToScale = (int)(sourceImage.Height * scale);

                //// 背景色を塗る
                //SolidBrush solidBrush = new SolidBrush(Color.Black);
                //graphics.FillRectangle(solidBrush, new RectangleF(0, 0, width, height));

                // サイズ変更した画像に、左上を起点に変更する画像を描画する
                graphics.DrawImage(sourceImage, 0, 0, widthToScale, heightToScale);

                // サイズ変更した画像を保存する
                return (Image)bitmap.Clone();
            }
        }
    }
}
