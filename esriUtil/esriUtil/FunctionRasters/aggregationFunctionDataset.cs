using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using esriUtil.FunctionRasters.NeighborhoodHelper;

namespace esriUtil.FunctionRasters
{
    public abstract class aggregationFunctionDataset : IRasterFunction
    {
        private IRasterInfo myRasterInfo; // Raster Info for the focal Function
        private rstPixelType myPixeltype = rstPixelType.PT_UNKNOWN; // Pixel Type of the log Function.
        private string myName = "aggregation Function"; // Name of the log Function.
        private string myDescription = "Aggregates a raster using specified number of cells"; // Description of the log Function.
        private IFunctionRasterDataset inrs = null;
        private IFunctionRasterDataset orig = null;
        private int cells;
        private IRasterFunctionHelper myFunctionHelper = new RasterFunctionHelperClass(); // Raster Function Helper object.
        private IRasterFunctionHelper myFunctionHelperOrig = new RasterFunctionHelperClass(); // Raster Function Helper object.
        public IRasterInfo RasterInfo { get { return myRasterInfo; } }
        public rstPixelType PixelType { get { return myPixeltype; } set { myPixeltype = value; } }
        public string Name { get { return myName; } set { myName = value; } }
        public string Description { get { return myDescription; } set { myDescription = value; } }
        public bool myValidFlag = false;
        public bool Valid { get { return myValidFlag; } }
        public void Bind(object pArgument)
        {
            if (pArgument is aggregationFunctionArguments)
            {
                aggregationFunctionArguments args = (aggregationFunctionArguments)pArgument;
                inrs = args.InRaster;
                orig = args.OriginalRaster;
                cells = args.Cells;
                myFunctionHelper.Bind(inrs);
                myFunctionHelperOrig.Bind(orig);
                //System.Windows.Forms.MessageBox.Show(orig.RasterInfo.CellSize.X.ToString()+"\n" + inrs.RasterInfo.CellSize.X.ToString());
                myRasterInfo = myFunctionHelper.RasterInfo;
                myPixeltype = myRasterInfo.PixelType;
                myValidFlag = true;
            }
            else
            {
                throw new System.Exception("Incorrect arguments object. Expected: aggregationFunctonArguments");
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
                //System.Windows.Forms.MessageBox.Show("Read resized raster");
                int pBHeight = pPixelBlock.Height;
                int pBWidth = pPixelBlock.Width;
                IPixelBlock3 ipPixelBlock = (IPixelBlock3)pPixelBlock;
                IPnt pbBigSize = new PntClass();
                IPnt pbBigLoc = new PntClass();
                int pbBigWd = pBWidth * cells;
                int pbBigHt = pBHeight * cells;
                pbBigSize.SetCoords(pbBigWd, pbBigHt);
                pbBigLoc.SetCoords((pTlc.X * cells), (pTlc.Y * cells));
                IPixelBlock pbOrig = myFunctionHelperOrig.Raster.CreatePixelBlock(pbBigSize);
                //System.Windows.Forms.MessageBox.Show("Read original Raster");
                myFunctionHelperOrig.Read(pbBigLoc, null, myFunctionHelperOrig.Raster, pbOrig);
                IPixelBlock3 pbBig = (IPixelBlock3)pbOrig;
                //int cnt = 0;
                for (int nBand = 0; nBand < ipPixelBlock.Planes; nBand++)
                {
                    //Console.WriteLine(ipPixelBlock.get_PixelType(nBand).ToString());
                    //object noDataValue = System.Convert.ToSingle(noDataValueArr.GetValue(nBand));
                    //System.Array pixelValuesBig = (System.Array)(pbBig.get_PixelData(nBand));
                    System.Array pixelValues = (System.Array)(ipPixelBlock.get_PixelData(nBand));
                    for (int r = 0; r < pBHeight; r++)
                    {
                        for (int c = 0; c < pBWidth; c++)
                        {
                            object objVl = ipPixelBlock.GetVal(nBand, c, r);
                            if (objVl != null)
                            {
                                object outVl = getTransformedValue(pbBig, nBand, c, r, cells);
                                //System.Windows.Forms.MessageBox.Show(outVl.ToString());
                                pixelValues.SetValue(outVl, c, r);

                            }
                        }
                    }
                    ipPixelBlock.set_PixelData(nBand, pixelValues);
                }
            }
            catch (Exception e)
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
                System.Exception myExc = new System.Exception("Exception caught in Update method of aggregation Function", exc);
                throw myExc;
            }
        }

        public abstract object getTransformedValue(IPixelBlock3 bigArr,int band, int startClms,int startRws,int cells);
    }
}