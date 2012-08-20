using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters
{
    public abstract class focalSampleDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the focal Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "focal Sample Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using focal sample analysis"; // Description of the log Function.
        private IRaster inrs = null;
        private IRaster orig = null;
        private int rws = 0;
        private int clms = 0;
        private HashSet<string> offset = null;
        public List<int[]> offsetLst = new List<int[]>();
        private rasterUtil.focalType inop = rasterUtil.focalType.SUM;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public double noDataValue = Double.MinValue;
        public void Bind(object pArgument)
        {
            if (pArgument is focalSampleArguments)
            {
                focalSampleArguments args = (focalSampleArguments)pArgument;
                inrs = args.InRaster;
                orig = args.OriginalRaster;
                inop = args.Operation;
                offset = args.OffSets;
                getPlusWidthHeight();
                IRasterProps rsProp = (IRasterProps)inrs;
                myFunctionHelper.Bind(inrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: focalSampleFunctonArguments");
            }
        }

        private void getPlusWidthHeight()
        {
            double mX = 0;
            double mY = 0;
            IPnt pnt = ((IRasterProps)inrs).MeanCellSize();
            double cellSizeX = pnt.X;
            double cellSizeY = pnt.Y;
            foreach (string s in offset)
            {
                string[] sArr = s.Split(new char[] { ';' });
                double az = System.Convert.ToDouble(sArr[0]);
                double ds = System.Convert.ToDouble(sArr[1]);
                double rd = az * (Math.PI / 180);
                double x = (Math.Sin(rd) * ds)/cellSizeX;
                double y = (Math.Cos(rd) * ds)/cellSizeY;
                offsetLst.Add(new int[]{System.Convert.ToInt32(Math.Round(x)),System.Convert.ToInt32(Math.Round(y))});
                if (x > mX) mX = x;
                if (y > mY) mY = y;
            }
            clms = System.Convert.ToInt32(Math.Round(mX*2));
            rws = System.Convert.ToInt32(Math.Round(mY*2));
        }
        /// <summary>
        /// Read pixels from the input Raster and fill the PixelBlock provided with processed pixels.
        /// The RasterFunctionHelper object is used to handle pixel type conversion and resampling.
        /// The log raster is the natural log of the raster. 
        /// </summary>
        /// <param name="pTlc">Point to start the reading from in the Raster</param>
        /// <param name="pRaster">Reference Raster for the PixelBlock</param>
        /// <param name="pPixelBlock">PixelBlock to be filled in</param>
        public void Read(IPnt pTlc, IRaster pRaster, IPixelBlock pPixelBlock)
        {
            try
            {
                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                IPnt pbBigSize = new PntClass();
                IPnt pbBigLoc = new PntClass();
                int pbBigWd = pBWidth + clms - 1;
                int pbBigHt = pBHeight + rws - 1;
                int l, t;
                l = clms / 2;
                t = rws / 2;
                //Console.WriteLine("lt = " + (pTlc.X - l).ToString() + ":" + (pTlc.Y - t).ToString());
                pbBigSize.SetCoords(pbBigWd, pbBigHt);
                pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                IPixelBlock3 pbBig = (IPixelBlock3)orig.CreatePixelBlock(pbBigSize);
                orig.Read(pbBigLoc, (IPixelBlock)pbBig);
                for (int nBand = 0; nBand < pbBig.Planes; nBand++)
                {
                    noDataValue = System.Convert.ToDouble(noDataValueArr.GetValue(nBand));
                    System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                    System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                    for (int r = 0; r < pBHeight; r++)
                    {
                        for (int c = 0; c < pBWidth; c++)
                        {
                            double inVl = System.Convert.ToDouble(pixelValues.GetValue(c, r));

                            if (inVl == noDataValue)
                            {
                                continue;
                            }
                            else
                            {
                                object outVl = getTransformedValue(pixelValuesBig, c+l, r+t);
                                pixelValues.SetValue(outVl, c, r);
                            }
                        }

                    }
                    ((IPixelBlock3)pPixelBlock).set_PixelData(nBand, pixelValues);

                }

            }
            catch
            {
            }
        }
        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of focalSample Function", exc);
                throw myExc;
            }
        }

        public abstract object getTransformedValue(System.Array bigArr, int startClm, int startRw);
    }
}
