using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Accord.Statistics.Distributions.DensityKernels;
using Accord.MachineLearning;

namespace esriUtil.FunctionRasters
{
    class meanShiftFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "meanShift Function"; // Name of the log Function.
        private string myDescription = "segments raster based on mean shift"; // Description of the log Function.
        private IFunctionRasterDataset inrs = null;
        private IFunctionRasterDataset valrs = null;
        private double radius = 3;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelper2 = new RasterFunctionHelperClass(); // value raster function helper object
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public int NumClusters = 0;
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is meanShiftFunctionArguments)
            {
                meanShiftFunctionArguments args = (meanShiftFunctionArguments)pArgument;
                inrs = args.InRaster;
                radius = args.Radius;
                valrs = args.ValueRaster;
                myFunctionHelper.Bind(inrs);
                myFunctionHelper2.Bind(valrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: meanShiftArguments");
            }
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
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                IPnt pntSize = new PntClass();
                pntSize.SetCoords(pPixelBlock.Width, pPixelBlock.Height);
                IPixelBlock vPb = myFunctionHelper2.Raster.CreatePixelBlock(pntSize);
                myFunctionHelper2.Read(pTlc, null, myFunctionHelper2.Raster, vPb);
                IPixelBlock3 pb3 = (IPixelBlock3)pPixelBlock;
                object intArr = calcMeanShift((IPixelBlock3)vPb,pb3);
                pb3.set_PixelData(0, intArr);
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method Mean-shift Function. " + exc.Message, exc);
                throw myExc;
            }
        }

        private object calcMeanShift(IPixelBlock3 vPb,IPixelBlock3 pb3)
        {

            double[][] jaArr = pixelBlockToJaggedArray(vPb);
            int bands = vPb.Planes;
            UniformKernel kernel = new UniformKernel();
            //GaussianKernel kernel = new GaussianKernel(bands);
            MeanShift ms = new MeanShift(bands, kernel, radius);
            int[] vls = ms.Compute(jaArr, 0.05, 10);
            NumClusters = ms.Clusters.Count;
            Console.WriteLine(NumClusters);
            return splitArray(vls, pb3);
        }

        private object splitArray(int[] vls, IPixelBlock3 pb3)
        {
            int width = pb3.Width;
            int height = pb3.Height;
            int cnt = 0;
            System.Array outArr = (System.Array)pb3.get_PixelData(0);
            rstPixelType rsp = pb3.get_PixelType(0);
            foreach (int i in vls)
            {
                double div = System.Convert.ToDouble(cnt) / width;
                int r = (int)div;
                int c = cnt - (r * width);
                try
                {
                    object newvl = rasterUtil.getSafeValue(i, rsp);
                    outArr.SetValue(i, c, r);
                }
                catch
                {
                    object newvl = rasterUtil.getSafeValue(900, rsp);
                    outArr.SetValue(900, c, r);
                }
                cnt++;
            }
            return outArr;
        }

        private double[][] pixelBlockToJaggedArray(IPixelBlock3 pb3)
        {
            int width = pb3.Width;
            int height = pb3.Height;
            int bands = pb3.Planes;
            //Console.WriteLine("Pixel Block width and height = " + width.ToString() + "; " + height.ToString());
            double[][] outArr = new double[width*height][];
            for (int r = 0; r < height; r++)
            {
                int indexVlr = r*width;
                int indexVl = indexVlr;
                for (int c = 0; c < width; c++)
                {
                    indexVl = indexVlr + c;
                    double[] bndsArr = new double[bands];
                    for (int p = 0; p < bands; p++)
                    {
                        object objVl = pb3.GetVal(p, c, r);
                        if (objVl != null)
                        {
                            bndsArr[p] = System.Convert.ToDouble(objVl);
                        }
                    }
                    //Console.WriteLine("Index value = " + indexVl.ToString() + "; " + width.ToString() + "; " + height.ToString() +"; "+ r.ToString()+"; " + c.ToString());
                    outArr[indexVl] = bndsArr;
                    
                }
            }
            return outArr;
        }
        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of mean-shift Function", exc);
                throw myExc;
            }
        }
    }
}
