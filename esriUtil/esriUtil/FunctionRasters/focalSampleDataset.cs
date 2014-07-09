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
        private IFunctionRasterDataset inrs = null;
        private IFunctionRasterDataset orig = null;
        private int rws = 0;
        private int clms = 0;
        private HashSet<string> offset = null;
        public List<int[]> offsetLst = new List<int[]>();
        private rasterUtil.focalType inop = rasterUtil.focalType.SUM;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperOrig = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public float noDataValue = Single.MinValue;
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
                myFunctionHelper.Bind(inrs);
                myFunctionHelperOrig.Bind(orig);
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
            double cellSizeX = inrs.RasterInfo.CellSize.X;
            double cellSizeY = inrs.RasterInfo.CellSize.Y;
            foreach (string s in offset)
            {
                string[] sArr = s.Split(new char[] { ';' });
                double az = System.Convert.ToDouble(sArr[0]);
                double ds = System.Convert.ToDouble(sArr[1]);
                double rd = az * (Math.PI / 180);
                double x = Math.Sin(rd) * (ds/cellSizeX);
                double y = Math.Cos(rd) * (ds/cellSizeY);
                offsetLst.Add(new int[]{System.Convert.ToInt32(x),System.Convert.ToInt32(y)});
                //Console.WriteLine("Adding offset " + x.ToString() + " : " + y.ToString());
                if (x > mX) mX = x;
                if (y > mY) mY = y;
            }
            clms = System.Convert.ToInt32(Math.Round(mX*(2+1)));
            rws = System.Convert.ToInt32(Math.Round(mY*2)+(2+1));
            //Console.WriteLine("Added Columns and rows  = " + clms.ToString() + " : " + rws.ToString());
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
                int pbBigWd = pBWidth + clms;
                int pbBigHt = pBHeight + rws;
                int l, t;
                l = (clms-1) / 2;
                t = (rws-1) / 2;
                //Console.WriteLine("lt = " + (pTlc.X - l).ToString() + ":" + (pTlc.Y - t).ToString());
                pbBigSize.SetCoords(pbBigWd, pbBigHt);
                pbBigLoc.SetCoords((pTlc.X - l), (pTlc.Y - t));
                IPixelBlock3 pbBig = (IPixelBlock3)myFunctionHelperOrig.Raster.CreatePixelBlock(pbBigSize);
                myFunctionHelperOrig.Read(pbBigLoc, null, myFunctionHelperOrig.Raster, (IPixelBlock)pbBig);
                for (int nBand = 0; nBand < pbBig.Planes; nBand++)
                {
                    
                    System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                    //System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                    for (int r = 0; r < pBHeight; r++)
                    {
                        for (int c = 0; c < pBWidth; c++)
                        {
                            object objVl = ipPixelBlock.GetVal(nBand,c, r);
                            if (objVl==null)
                            {
                                continue;
                            }
                            else
                            {
                                float outVl = System.Convert.ToSingle(getTransformedValue(pbBig, c+l, r+t,nBand));
                                //Console.WriteLine(outVl.ToString());
                                pixelValues.SetValue(outVl,c, r);
                            }
                        }

                    }
                    ((IPixelBlock3)pPixelBlock).set_PixelData(nBand, pixelValues);

                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
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

        public abstract object getTransformedValue(IPixelBlock3 bigPb, int startClm, int startRw, int nBand);
    }
}
