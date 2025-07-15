using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static log4net.Appender.ColoredConsoleAppender;


namespace RSACommon.GraphicsForm
{
    public class MatrixPanel: System.Windows.Forms.Control
    {
        List<Panel> PositionedItems = new List<Panel>();
        Panel _panelBox { get; set; }
        public static int SplittedHorizontallyBy => 15;
        public static int SplittedVerticallyBy => 5;
        private int _row = 0;
        private int _column = 0;
        private bool _error = false;
        private Color _color = Color.White; 
        public MatrixPanel(int x, int y, int width, int height, int row, int column, Control parent)
        {

            if (row == 0 || column == 0)
            {
                _error = true;
                return;
            }

            _color = Color.Gray;
            _row = row;
            _column = column;

            _panelBox = new Panel();
            _panelBox.Text = "";
            _panelBox.Size = new System.Drawing.Size(width, height);
            _panelBox.BackColor = System.Drawing.Color.Transparent;
            _panelBox.BackColor = _color;
            //_panelBox.BackColor = System.Drawing.Color.AntiqueWhite;
            _panelBox.BorderStyle = BorderStyle.FixedSingle;

            _panelBox.Visible = true;

            Controls.Add(_panelBox);
            //parent.Controls.Add(this);
            parent.Controls.Add(_panelBox);

            _panelBox.Location = new System.Drawing.Point(x, y);

            int boxWidth = (int)Math.Ceiling((double)(width - SplittedHorizontallyBy )/ column);
            int boxHeight = (int)Math.Ceiling((double)(height - SplittedVerticallyBy) / row);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    Panel box = new Panel();

                    box.Text = $"{(j+1)*(i+1)}Th";
                    box.Visible = true;
                    box.Size = new System.Drawing.Size(boxWidth, boxHeight);

                    int spacing = j == 0 ? 0 : SplittedHorizontallyBy;
                    int xLocation = j * boxWidth + spacing;
                    int yLocation = i * boxHeight;

                    box.Location = new System.Drawing.Point(xLocation, yLocation);

                    //box.BackColor = System.Drawing.Color.Azure;
                    _panelBox.BackColor = System.Drawing.Color.Transparent;
                    _panelBox.Controls.Add(box);

                    PositionedItems.Add(box);
                }
            }       

        }

        public ControlCollection MatrixControls
        {
            get
            {
                ControlCollection c = new ControlCollection(this);
                for (int i = 0; i < _row; i++)
                {
                    for (int j = 0; j < _column; j++)
                    {
                        foreach(Control ctrl in _panelBox.Controls[i + j].Controls)
                        {
                            c.Add(ctrl);
                        }
                    }
                }

                return c;
            }

        }

        public virtual bool AddElements(Control toAdd, int x = 0, int y = 0)
        {
            Panel groupFind = null;

            if (_error)
                return false;

            if((groupFind = PositionedItems.First(t => t.HasChildren == false)) != null)
            {
                //toAdd.BringToFront();
                groupFind.Controls.Add(toAdd);
                toAdd.Location = new System.Drawing.Point(x, y);
                return true;
            }

            return false;
        }


    }
}
