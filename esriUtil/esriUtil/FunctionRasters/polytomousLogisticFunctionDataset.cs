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
        private IRaster outrs = null;
        private IRaster inrsBandsCoef = null;
        private Dictionary<string,float[]> slopes = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
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
                IRasterProps rsProp = (IRasterProps)outrs;
                myFunctionHelper.Bind(outrs);
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
                // Call Read method of the Raster Function Helper object.

                System.Array noDataValueArr = (System.Array)((IRasterProps)pRaster).NoDataValue;
                float noDataVl = System.Convert.ToSingle(noDataValueArr.GetValue(0));
                //Console.WriteLine("Before Read");
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                //Console.WriteLine("After Read");
                noDataValueArr = (System.Array)((IRasterProps)inrsBandsCoef).NoDataValue;//inrsBandsCoef
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPnt pbSize = new PntClass();
                pbSize.SetCoords(pBWidth, pBHeight);
                IPixelBlock3 outPb = (IPixelBlock3)inrsBandsCoef.CreatePixelBlock(pbSize);//independent variables  
                inrsBandsCoef.Read(pTlc, (IPixelBlock)outPb);
                int pBRowIndex = 0;
                int pBColIndex = 0;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                System.Array[] pArr = new System.Array[outPb.Planes];
                System.Array[] cArr = new System.Array[ipPixelBlock.Planes];
                for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                {
                    System.Array pixelValues = (System.Array)(outPb.get_PixelData(coefnBand));
                    pArr[coefnBand] = pixelValues;
                }
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
                        float[] expArr = new float[slopes.Count];
                        float sumExp = 0;
                        int catCnts = 0;
                        foreach (float[] IntSlpArr in slopes.Values)
                        {
                            float sumVls = IntSlpArr[0];
                            for (int coefnBand = 0; coefnBand < outPb.Planes; coefnBand++)
                            {
                                float noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(coefnBand));
                                float pixelValue = Convert.ToSingle(pArr[coefnBand].GetValue(k, i));
                                //Console.WriteLine("Slope X value = " + pixelValue.ToString());
                                if (rasterUtil.isNullData(pixelValue, noDataValue))
                                {
                                    sumVls = noDataVl;
                                    break;
                                }

                                float slp = IntSlpArr[coefnBand + 1];
                                //Console.WriteLine("x = " + pixelValue.ToString() + " slope = " + slp.ToString());
                                sumVls += pixelValue * slp;
                            }
                            //Console.WriteLine("sumVls = " + sumVls.ToString());
                            float expSum = System.Convert.ToSingle(Math.Exp(sumVls));
                            
                            expArr[catCnts] = expSum;
                            sumExp += expSum;
                            catCnts +=1 ;
                        }
                        sumExp += 1;
                        float sumProb = 0;
                        float maxProbBand = 1;
                        float maxProb = 0;
                        catCnts = 2;
                        foreach (float expVl in expArr)
                        {
                            float prob = expVl / sumExp;
                            //Console.WriteLine("Probability = " + prob.ToString());
                            sumProb += prob;
                            if (prob > maxProb)
                            {
                                maxProb = prob;
                                maxProbBand = catCnts;
                            }
                            cArr[catCnts].SetValue(prob, k, i);
                            //Console.WriteLine("Probability = " + cArr[catCnts].GetValue(k,i).ToString());
                            catCnts += 1;
                            
                        }
                        float lProb = 1-sumProb;
                        cArr[1].SetValue(lProb,k,i);//base category
                        if (lProb > maxProb)
                        {
                            maxProb = lProb;
                            maxProbBand = 1;
                        }
                        cArr[0].SetValue(maxProbBand, k, i);//most likely class
                            
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
