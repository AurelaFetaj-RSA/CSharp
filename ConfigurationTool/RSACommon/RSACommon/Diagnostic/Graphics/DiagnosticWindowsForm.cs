using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace RSACommon.GraphicsForm
{
    public class DiagnosticWindowsControl : System.Windows.Forms.Control
    {
        GroupBox group = new GroupBox();
        TextBox textBox = new TextBox();
        string groupboxName = "{0} Diagnostic";
        public static int GroupboxWidth => 600;
        public static int MAX_STRING_MEMORY = 10;

        public static int GroupboxHeight => 20* MAX_STRING_MEMORY;
        private static int X_BORDER => 10;
        private static int Y_BORDER => 10;
        public List<string> DiagnosticMemory = new List<string>();
        ILog Logger { get; set; } = null;

        public DiagnosticWindowsControl(int x, int y, string variableName, Control Parent, ILog logger) 
        {
            Logger = logger;
            group.Text = String.Format(groupboxName, variableName);

            group.Location = new System.Drawing.Point(x, y);
            group.Size = new System.Drawing.Size(GroupboxWidth, GroupboxHeight);

            Label label = new Label();
            label.Location = new System.Drawing.Point(X_BORDER, 3*Y_BORDER);
            label.Text = "Value";
            label.Size = new System.Drawing.Size(40, 20);

            int textBoxHeight = (int)(Math.Ceiling(Font.GetHeight()))* MAX_STRING_MEMORY;
            textBox.Text = "";
            textBox.Size = new Size(GroupboxWidth - 2*X_BORDER, textBoxHeight);
            textBox.Multiline = true;
            textBox.ReadOnly = true;
            textBox.BackColor = System.Drawing.Color.LightYellow;
            textBox.AllowDrop = false;
            textBox.WordWrap = false;
            textBox.BorderStyle = BorderStyle.Fixed3D;
            textBox.ScrollBars = ScrollBars.Vertical;   

            group.Controls.Add(textBox);

            textBox.TextChanged += TextBox_TextChanged;
            textBox.Location = new System.Drawing.Point(X_BORDER, 5* Y_BORDER);
            group.Controls.Add(label);

            Controls.Add(group);
            Size = group.Size;
            Location = group.Location;

            if(Parent != null)
                if(!Parent.Controls.Contains(this))
                    Parent.Controls.Add(group);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            textBox.SelectionStart = 0;
            textBox.ScrollToCaret();
        }

        public void ThreadSafeResetTextbox()
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke((MethodInvoker)delegate
                {
                    textBox.Text = "";
                });
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            StringFormat style = new StringFormat();

            // Call the DrawString method of the System.Drawing class to write
            // text. Text and ClientRectangle are properties inherited from
            // Control.
            e.Graphics.DrawString(
                Text,
                Font,
                new SolidBrush(ForeColor),
                ClientRectangle, style);
        }

        /// <summary>
        /// This will write text filling the textbox, no wrap, no horizontal scrollbar
        /// </summary>
        /// <param name="text"></param>
        public void ThreadSafeWriteMessage(string text)
        {
            try
            {
                Logger?.Warn(text);
                ThreadSafeResetTextbox();

                string textLimited =  $"{DateTime.Now.ToString("hh:mm:ss")} {text}";

                // Set string format.
                StringFormat newStringFormat = new StringFormat();
                newStringFormat.FormatFlags = StringFormatFlags.NoWrap;
                int charactersFitted;
                int linesFilled;


                //Graphics test = new Graphics();
                //Graphics.

                using (Graphics g = CreateGraphics())
                {
                    Size s = new Size(textBox.Size.Width - 60, textBox.Size.Height);
                    SizeF size = g.MeasureString(textLimited, textBox.Font, s, newStringFormat, out charactersFitted, out linesFilled);
                }

                if (textLimited.Length > charactersFitted)
                    textLimited = textLimited.Substring(0, charactersFitted) + "...";

                DiagnosticMemory.Insert(0, textLimited);

                if (DiagnosticMemory.Count > MAX_STRING_MEMORY)
                {
                    DiagnosticMemory.RemoveRange(10, DiagnosticMemory.Count - MAX_STRING_MEMORY);
                }

                string textToWriteAsync = "";
                foreach (var textToWrite in DiagnosticMemory)
                {
                    textToWriteAsync += textToWrite + Environment.NewLine;
                }

                if (textBox.InvokeRequired)
                {
                    textBox.Invoke((MethodInvoker)delegate
                    {
                        textBox.AppendText(textToWriteAsync);
                    });
                }

            }
            catch
            {
                Logger?.Warn("Error on writing real time Windows Diagnostic Control");
            }
        }
    }
}
