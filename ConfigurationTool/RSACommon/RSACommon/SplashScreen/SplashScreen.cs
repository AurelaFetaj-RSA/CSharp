using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSACommon.GraphicsForm
{
    public partial class SplashScreen : Form
    {
        private Bitmap MyImage;
        int SpacingX = 10;
        int SpacingY = 10;
        public SplashScreen(string ImagePath, int imageWidth, int imageHeight)
        {
            InitializeComponent();

            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Size = new Size(imageWidth, imageHeight);
            
            if(File.Exists(ImagePath))
            {
                MyImage = new Bitmap(ImagePath);
                ////pictureBox1.ClientSize = new Size(imageWidth, imageWidth);
                //pictureBox1.Location = new Point(SpacingX, SpacingY);
                ////pictureBox1.Size = new Size(imageWidth, imageWidth);
                //pictureBox1.Update();

                loadingTextbox.Location = new Point(SpacingX, SpacingY+ pictureBox1.ClientSize.Height + SpacingY);
                loadingTextbox.Width = pictureBox1.Width;
                loadingTextbox.Update();

                Size = new Size(pictureBox1.Width + 2 * SpacingX, Size.Height);
                Update();

                pictureBox1.Image = (Image)MyImage;
            }
        }

        public void WriteOnTextboxAsync(string textToAppend)
        {
            if (loadingTextbox.InvokeRequired)
            {
                loadingTextbox.Invoke((MethodInvoker)delegate
                {
                    loadingTextbox.AppendText(textToAppend + Environment.NewLine);
                });
            }
        }

    }
}


