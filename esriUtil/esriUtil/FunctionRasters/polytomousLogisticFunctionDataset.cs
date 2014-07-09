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
    class polytomousLogisticFunctionDataset : IRasterFunction
    {
        public polytomousLogisticFunctionDataset()
        {
        }
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "Polytomous Logistic Regression Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using Polytomous Logistic regression transformation"; // Description of the log Function.
        private IFunctionRasterDataset outrs = null;
        private IFunctionRasterDataset inrsBandsCoef = null;
        private double[][] slopes = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperCoef = new RasterFunctionHelperClass();
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is polytomousLogisticFunctionArguments)
            {
                polytomousLogisticFunctionArguments arg = (polytomousLogisticFunctionArguments)pArgument;
                inrsBandsCoef = arg.InRasterCoefficients;
                slopes = arg.Slopes;
                outrs = arg.OutRaster;
                myFunctionHelper.Bind(outrs);
                myFunctionHelperCoef.Bind(inrsBandsCoef);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: regressionFunctionArguments");
            }
        }

        /// <summary>
        /// Read pixels from the input Raster and fill the PixelBlock provided with processed pixels.
        /// The RasterFunctionHelper object is used to handle pixel type conversion and resampeling.
        /// The log raster is the natural log of the raster. 
        /// </summary>
        /// <param name="pTlc">Point to start the reading from in the Raster</param>
        /// <param name="pRaster">Reference Raster for the PixelBlock</param>
        /// <param name="pPixelBlock">PixelBlock to be filled in</param>
        public void Read(IPnt pTlc, IRaster pRaster, IPixelBlock pPixelBlock)
        {
            try
            {
                // Call Read method of the Raster Function Helper object.

                //Console.WriteLine("Before Read");
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                //Console.WriteLine("After Read");
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)myFunctionHelperCoef.Raster.CreatePixelBlock(pbSize);//independent variables  
                myFunctionHelperCoef.Read(pTlc,null,myFunctionHelperCoef.Raster, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] cArr = new System.Array[ipPixelBlock.Planes];
                for (int outBand = 0; outBand < ipPixelBlock.Planes; outBand++)
                {
                    //float[,] td = new float[ipPixelBlock.Width, ipPixelBlock.Height];
                    System.Array pixelValues = (System.Array)ipPixelBlock.get_PixelData(outBand);//(System.Array)(td);
                    cArr[outBand] = pixelValues;
                }
                for (int i = pBRowIndex; i < pBHeight; i++)
                {
                    for (int k = pBColIndex; k < pBWidth; k++)
                    {
                        double[] expArr = new double[slopes.Length];
                        double sumExp = 0;
                        int catCnts = 0;
                        bool noDataCheck = true;
                        foreach (double[] IntSlpArr in slopes)
                        {
                            double sumVls = IntSlpArr[0];
                            for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                            {
                                object coefnVlObj = outPb.GetVal(coefnBand, k, i);
                                //Console.WriteLine("Slope X value = " + pixelValue.ToString());
                                if (coefnVlObj==null)
                                {
                                    noDataCheck = false;
                                    break;
                                }
                                double slp = IntSlpArr[coefnBand + 1];
                                //Console.WriteLine("x = " + pixelValue.ToString() + " slope = " + slp.ToString());
                                sumVls += System.Convert.ToSingle(coefnVlObj) * slp;
                            }
                            if (noDataCheck)
                            {
                                double expSum = Math.Exp(sumVls);
                                expArr[catCnts] = expSum;
                                sumExp += expSum;
                                catCnts += 1;
                                //Console.WriteLine("sumVls = " + sumVls.ToString());
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (noDataCheck)
                        {
                            sumExp += 1;
                            double sumProb = 0;
                            catCnts = 1;
                            foreach (double expVl in expArr)
                            {
                                double prob = expVl / sumExp;
                                //Console.WriteLine("Probability = " + prob.ToString());
                                sumProb += prob;
                                cArr[catCnts].SetValue(System.Convert.ToSingle(prob), k, i);
                                //Console.WriteLine("Probability = " + cArr[catCnts].GetValue(k,i).ToString());
                                catCnts += 1;

                            }
                            double lProb = 1 - sumProb;
                            cArr[0].SetValue(System.Convert.ToSingle(lProb), k, i);//base category  
                        }
                        else
                        {
                            for (int b = 0; b < ipPixelBlock.Planes; b++)
                            {
                                cArr[b].SetValue(0, k, i);
                            }
                            
                        }
                    }
                }
                for(int i=0;i<ipPixelBlock.Planes;i++)
                {
                    ipPixelBlock.set_PixelData(i,cArr[i]);
                }
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of Regression Function. " + exc.Message, exc);
                Console.WriteLine(exc.ToString());
            }
        }
        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of abs Function", exc);
                throw myExc;
            }
        }
    }
}
