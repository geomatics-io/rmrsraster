using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms.DataVisualization;

namespace esriUtil.Forms.Stats
{
    public partial class frmChart : Form
    {
        public frmChart()
        {
            InitializeComponent();
        }

        private void frmHistogram_Resize(object sender, EventArgs e)
        {
            chrHistogram.Width = this.Width-9;
            chrHistogram.Height = this.Height-67;
            foreach (Control c in Controls)
            {
                if (c is System.Windows.Forms.DataVisualization.Charting.Chart)
                {
                }
                else
                {
                    c.Location = new Point(c.Location.X, chrHistogram.Height + 3);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog svD = new SaveFileDialog();
            svD.Filter = "CSV|*.csv";
            svD.AddExtension = true;
            svD.Title = "Export Data To";
            if (svD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (System.IO.StreamWriter wr = new System.IO.StreamWriter(svD.FileName))
                {
                    wr.WriteLine("Series,XValues,YValues");
                    for (int i = 0; i < chrHistogram.Series.Count; i++)
                    {
                        System.Windows.Forms.DataVisualization.Charting.Series chS = chrHistogram.Series[i];
                        string sName = chS.Name;
                        for (int j = 0; j < chS.Points.Count; j++)
                        {
                            System.Windows.Forms.DataVisualization.Charting.DataPoint chDp = chS.Points[j];
                            string xVl = chDp.XValue.ToString();
                            string[] yVlArr = (from double d in chDp.YValues select d.ToString()).ToArray();
                            string yVl = String.Join(";", yVlArr);
                            string ln = sName + "," + xVl + "," + yVl;
                            wr.WriteLine(ln);
                        }

                    }
                    wr.Close();
                }
            }
            MessageBox.Show("Finished writing data");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog svD = new SaveFileDialog();
            svD.Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp|TIFF|*.tif|GIF|*.gif";
            svD.AddExtension = true;
            svD.Title = "Export Data To";
            if (svD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int fIndex = svD.FilterIndex;
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                switch (fIndex)
	            {
                    case 1:
                        format = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    case 2:
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                    case 3:
                        format = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                    case 4:
                        format = System.Drawing.Imaging.ImageFormat.Tiff;
                        break;
                    case 5:
                        format = System.Drawing.Imaging.ImageFormat.Gif;
                        break;
		            default:
                        format = System.Drawing.Imaging.ImageFormat.Wmf;
                    break;
	            }
                chrHistogram.SaveImage(svD.FileName, format);
                MessageBox.Show("Finished saving image");
            }
        }
    }
}
