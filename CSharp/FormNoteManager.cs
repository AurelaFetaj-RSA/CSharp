using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ProductionLaunch
{
    public partial class FormNoteManager : Form
    {
        public string strNote = "";
        public string strModel = "";
        public string strArticle = "";
        public string strTLN = "";
        public string strDLN = "";
        public string strLLN = "";
        public string strParam1 = "";
        public string strParam2 = "";
        public string strParam3 = "";
        public string strParam4 = "";
        public string strParam5 = "";

        NoteManager noteDetail = new NoteManager();
        public DataGrid gridDetail = null;
        DataGrid dataPointGridDetail = null;
        DataTable dtDataPointsDetail = null;

        public FormNoteManager()
        {
            InitializeComponent();
        }

        private void FormNoteManager_Load(object sender, EventArgs e)
        {
            noteDetail.SetNoteManagerFromDBRecord(strNote, strModel, strArticle, strTLN, strDLN, strLLN, strParam1, strParam2, strParam3, strParam4, strParam5);

            textBoxNoteDetails.Text = strNote;
            textBoxNoteModelNameDetail.Text = strModel;
            textBoxNoteArticleCodeDetail.Text = strArticle;


            //string strParam1 = "";
            //string strParam2 = "";
            //string strParam3 = "";
            //string strParam4 = "";
            //string strParam5 = "";
            InitNoteChartDetail();
            UpdateGlacialListNumberedDetail();
        }

        private void InitNoteChartDetail()
        {
            if (noteDetail.NoteTargetList.Count() == 0) return;
            pnGridDetail.Controls.Clear();

            dataPointGridDetail = new DataGrid();
            dtDataPointsDetail = new DataTable();

            DataColumn column = new DataColumn() { ColumnName = "paia target", DataType = typeof(int) };
            dtDataPointsDetail.Columns.Add(column);

            column = new DataColumn() { ColumnName = "paia caricate", DataType = typeof(int) };
            dtDataPointsDetail.Columns.Add(column);

            column = new DataColumn() { ColumnName = "paia prodotte", DataType = typeof(int) };
            dtDataPointsDetail.Columns.Add(column);

            for (int j = 0; j < noteDetail.NoteTargetList.Count(); j++)
            {
                try
                {
                    //xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    dtDataPointsDetail.Rows.Add(noteDetail.NoteTargetList[j].NPairs / 2, noteDetail.NoteLoadedList[j].NPairs / 2, noteDetail.NoteProducedList[j].NPairs / 2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            dataPointGridDetail.DataSource = dtDataPointsDetail;
            dataPointGridDetail.Width = pnChartDetail.Width;
            dataPointGridDetail.Height = pnChartDetail.Height;
            pnChartDetail.Controls.Add(dataPointGridDetail);

            gridDetail = dataPointGridDetail;

            pnChartDetail.Controls.Clear();
            pnChartDetail.Controls.Add(GetChartDetail());
        }

        private Chart GetChartDetail()
        {
            Chart chart = null;
            if (gridDetail == null)
            {
                return chart;
            }
            try
            {
                pnChartDetail.Controls.Clear();

                WindowsCharting charting = new WindowsCharting();
                DataTable dt = null;
                if (gridDetail != null)
                {
                    dt = gridDetail.DataSource as DataTable;
                }
                chart = charting.GenerateChart(dt, pnChartDetail.Width, pnChartDetail.Height, "Control", 10);

                chart.ChartAreas[0].Area3DStyle.Enable3D = true;
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Verdana", 6F, System.Drawing.FontStyle.Regular);
                chart.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Regular);
                chart.ChartAreas[0].AxisX.Maximum = 42.5;
                chart.ChartAreas[0].AxisX.Minimum = 33.5;
                chart.ChartAreas[0].AxisX.Interval = 0.5;

                chart.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = false;

                chart.ChartAreas[0].AxisX.Title = "Numero";
                chart.ChartAreas[0].AxisY.Title = "Paia";

                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.Interval = 1;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                //do nothing
            }
            return chart;
        }

        private void UpdateGlacialListNumberedDetail()
        {
            int k = 0;

            glacialListTargetNumberedDetail.Items.Clear();
            GlacialComponents.Controls.GLItem item = new GlacialComponents.Controls.GLItem();
            for (k = 0; k < noteDetail.NoteTargetList.Count(); k++)
            {

                Label lbPairs = new Label();
                //xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                lbPairs.Text = (noteDetail.NoteTargetList[k].NPairs / 2 - noteDetail.NoteLoadedList[k].NPairs / 2).ToString();

                item.SubItems[k].Control = lbPairs;
            }

            glacialListTargetNumberedDetail.Items.Insert(0, item);
            glacialListTargetNumberedDetail.Refresh();
        }

        private void buttonCloseDetail_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
