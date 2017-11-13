using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;

namespace esriUtil
{
    public class photoAnalysis
    {
        public enum extensionType { jpg, bmp, gif, tiff, png };
        public photoAnalysis(string InputDir, string PolygonPath, string OutPutCSV, extensionType Extension = extensionType.jpg, esriUtil.Forms.RunningProcess.frmRunningProcessDialog rpd = null)
        {
            photoDir = InputDir;
            polyPath = PolygonPath;
            outCSV = OutPutCSV;
            ext = Extension;
            frmRpd = rpd;
        }
        esriUtil.Forms.RunningProcess.frmRunningProcessDialog frmRpd = null;
        extensionType ext = extensionType.jpg;
        string photoDir = "";
        string polyPath = "";
        string outCSV = "";
        public string PhotoDir { get { return photoDir; } }
        public string PolygonPath { get { return polyPath; } }
        public string OutCSV { get { return outCSV; } }
        esriUtil.geoDatabaseUtility geoUtil = new esriUtil.geoDatabaseUtility();
        esriUtil.rasterUtil rsUtil = new esriUtil.rasterUtil();
        public void runAnalysis()
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outCSV))
            {
                string hd = "ROI,DIR,NAME,Year,Month,Day,Time,Red,Green,Blue,GCC,ExG,VIgreen,RCC,RGB,Area";//add RCC, RGB, Area
                sw.WriteLine(hd);
                IFeatureClass ftrCls = geoUtil.getFeatureClass(polyPath);
                IFeatureCursor fCur = ftrCls.Search(null, true);
                IFeature ftr = fCur.NextFeature();
                int cnt = 1;
                while (ftr != null)
                {
                    IGeometry geo = ftr.Shape;
                    string[] lnArr = new string[16];
                    lnArr[0] = cnt.ToString();
                    lnArr[1] = photoDir.Split(new char[] { '\\' }).Last();
                    foreach (string flPath in System.IO.Directory.GetFiles(photoDir, "*." + ext))
                    {
                        string nm = System.IO.Path.GetFileNameWithoutExtension(flPath);
                        if (frmRpd != null)
                        {
                            frmRpd.addMessage("Working On " + nm);
                            frmRpd.stepPGBar(1);
                            frmRpd.Refresh();
                        }

                        lnArr[2] = nm;
                        //Console.WriteLine(flPath);

                        System.IO.FileStream fs = new System.IO.FileStream(flPath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                        BitmapSource img = BitmapFrame.Create(fs);
                        BitmapMetadata md = (BitmapMetadata)img.Metadata;
                        string timeStamp = "";
                        if (md != null)
                        {
                            timeStamp = md.DateTaken;
                            DateTime dt = Convert.ToDateTime(timeStamp);
                            lnArr[3] = dt.Year.ToString();
                            lnArr[4] = dt.Month.ToString();
                            lnArr[5] = dt.Day.ToString();
                            lnArr[6] = dt.TimeOfDay.ToString();



                            //Console.WriteLine(date);
                        }
                        else
                        {
                            //Console.WriteLine("md=null");
                        }
                        IFunctionRasterDataset rsDset = rsUtil.clipRasterFunction(flPath, geo, esriRasterClippingType.esriRasterClippingOutside);
                        double r, g, b;
                        bool cntCheck = getRGB(rsDset, out r, out g, out b);
                        lnArr[7] = r.ToString();
                        lnArr[8] = g.ToString();
                        lnArr[9] = b.ToString();
                        double GCC=0;
                        double ExG=0;
                        double VIgreen=0;
                        double RCC = 0;
                        double RGB = 0;
                        double Area = ((IArea)geo).Area;
                        if(!cntCheck)
                        {
                            RGB = r + g + b;
                            GCC = g / RGB;
                            ExG = 2 * g - (r + b);
                            VIgreen = (g - r) / (g + r);
                            RCC = r / RGB;
                        }
                        lnArr[10] = GCC.ToString();
                        lnArr[11] = ExG.ToString();
                        lnArr[12] = VIgreen.ToString();
                        lnArr[13] = RCC.ToString();
                        lnArr[14] = RGB.ToString();
                        lnArr[15] = Area.ToString();
                        sw.WriteLine(String.Join(",", lnArr));
                    }
                    ftr = fCur.NextFeature();
                }
                sw.Close();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(fCur);
            }

        }

        private bool getRGB(IFunctionRasterDataset rs, out double r, out double g, out double b)
        {
            r = 0;
            g = 0;
            b = 0;
            IRasterBandCollection rsbc = (IRasterBandCollection)rs;
            double[] vlArr = new double[rsbc.Count];
            int[] cntArr = new int[vlArr.Length];
            IRaster2 rs2 = (IRaster2)rsUtil.createRaster(rs);
            IRasterCursor rsCur = rs2.CreateCursorEx(null);
            do
            {
                IPixelBlock pb = rsCur.PixelBlock;
                for (int bd = 0; bd < vlArr.Length; bd++)
                {
                    for (int rw = 0; rw < pb.Height; rw++)
                    {
                        for (int cl = 0; cl < pb.Width; cl++)
                        {
                            object vlObj = pb.GetVal(bd, cl, rw);
                            if (vlObj != null)
                            {
                                vlArr[bd] += System.Convert.ToDouble(vlObj);
                                cntArr[bd] += 1;
                            }

                        }
                    }
                }

            } while (rsCur.Next());
            int cntr = cntArr[0];
            int cntg = cntArr[1];
            int cntb = cntArr[2];
            int bcheck = 0;
            bool cntZerro = false;
            if (cntr > 0)
            {
                r = vlArr[0] / cntr;
            }
            else
            {
                bcheck += 1;
            }
            if (cntg > 0)
            {
                g = vlArr[1] / cntg;
            }
            else
            {
                bcheck += 1;
            }
            if (cntb > 0)
            {
                b = vlArr[2] / cntb;
            }
            if (bcheck == 2)
            {
                cntZerro = true;
            }
            return cntZerro;
        }

    }
}
