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
    class mergeFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the log Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "merge Function"; // Name of the log Function.
        private string myDescription = "Transforms a raster using merge transformation"; // Description of the log Function.
        private IRaster outrs = null;
        private IRaster[] inrs = null;
        private IFeatureClass ftrCls = null;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is mergeFunctionArguments)
            {
                mergeFunctionArguments arg = (mergeFunctionArguments)pArgument;
                inrs = arg.InRaster;
                outrs = arg.OutRaster;
                //Console.WriteLine("Number of Bands in outrs = " + ((IRasterBandCollection)outrs).Count.ToString());
                ftrCls = arg.Boundary;
                IRasterProps rsProp = (IRasterProps)outrs;
                myFunctionHelper.Bind(outrs);
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: mergeFunctionArguments");
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

                
                //Console.WriteLine("Before Read");
                myFunctionHelper.Read(pTlc, null, pRaster, pPixelBlock);
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                IRaster2 rs2 = (IRaster2)pRaster;
                double pX,pY;
                rs2.PixelToMap(System.Convert.ToInt32(pTlc.X), System.Convert.ToInt32(pTlc.Y), out pX, out pY);
                //Console.WriteLine("After Read");
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IEnvelope env = new EnvelopeClass();
                env.PutCoords(pX, pY - pBHeight, pX + pBWidth, pY);
                updateWithMergedValues(env,ipPixelBlock);

                
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Read method of merge Function. " + exc.Message, exc);
                Console.WriteLine(exc.ToString());
            }
        }

        private void updateWithMergedValues(IEnvelope env, IPixelBlock3 ipPixelBlock)
        {
            System.Array[] outPixelValuesArr = new System.Array[ipPixelBlock.Planes];
            List<System.Array[]> inPixelValuesArrLst = new List<System.Array[]>();
            List<System.Array> inPixelNoDataArrLst = new List<System.Array>();
            IPnt pntSize = new PntClass();
            pntSize.SetCoords(ipPixelBlock.Width, ipPixelBlock.Height);
            ISpatialFilter spFlt = new SpatialFilterClass();
            spFlt.Geometry = (IGeometry)env;
            spFlt.GeometryField = ftrCls.ShapeFieldName;
            spFlt.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
            IFeatureCursor fCur = ftrCls.Search(spFlt, false);
            int fIndex = ftrCls.FindField("catIndex");
            IFeature ftr = fCur.NextFeature();
            for (int i = 0; i < ipPixelBlock.Planes; i++)
            {
                outPixelValuesArr[i] = (System.Array)ipPixelBlock.get_PixelData(i);
            }
            while (ftr != null)
            {
                int rsIndex = System.Convert.ToInt32(ftr.get_Value(fIndex));
                IRaster rs = inrs[rsIndex];
                IPixelBlock inputPb = rs.CreatePixelBlock(pntSize);
                IRaster2 rs2 = (IRaster2)rs;
                int pClm, pRw;
                rs2.MapToPixel(env.XMin, env.YMax, out pClm, out pRw);
                IPnt tlc = new PntClass();
                tlc.SetCoords(pClm, pRw);
                rs.Read(tlc, inputPb);
                System.Array[] inPixelValuesArr = new System.Array[inputPb.Planes];
                for (int i = 0; i < inputPb.Planes; i++)
                {
                    inPixelValuesArr[i] = (System.Array)inputPb.get_SafeArray(i);
                }
                inPixelNoDataArrLst.Add((System.Array)((IRasterProps)rs).NoDataValue);
                inPixelValuesArrLst.Add(inPixelValuesArr);
                ftr = fCur.NextFeature();
            }
            for (int i = 0; i < outPixelValuesArr.Length; i++)
            {
                for (int r = 0; r < ipPixelBlock.Height; r++)
                {
                    for (int c = 0; c < ipPixelBlock.Width; c++)
                    {
                        double vl = getValue(i, c, r, inPixelValuesArrLst, inPixelNoDataArrLst);
                        outPixelValuesArr[i].SetValue(vl,c,r);
                    }
                }
            }
            for (int i = 0; i < outPixelValuesArr.Length; i++)
            {
                ipPixelBlock.set_PixelData(i, outPixelValuesArr[i]);
            }

        }

        private double getValue(int i, int c, int r, List<System.Array[]> inPixelValuesArrLst, List<System.Array> inPixelNoDataArrLst,int cntLst=0)
        {
            double noDataVl = System.Convert.ToDouble(inPixelNoDataArrLst[cntLst].GetValue(i));
            double inVl = System.Convert.ToDouble(inPixelValuesArrLst[cntLst][i].GetValue(c, r));
            if (rasterUtil.isNullData(inVl, noDataVl)&&(cntLst+1<inPixelValuesArrLst.Count))
            {
                inVl=getValue(i, c, r, inPixelValuesArrLst, inPixelNoDataArrLst, cntLst++);
            }
            return inVl;
        }
        public void Update()
        {
            try
            {
            }
            catch (Exception exc)
            {
                System.Exception myExc = new System.Exception("Exception caught in Update method of merge Function", exc);
                throw myExc;
            }
        }
    }
}