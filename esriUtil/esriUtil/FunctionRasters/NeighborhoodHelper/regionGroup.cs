using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    public class regionGroup
    {
        private geoDatabaseUtility geoUtil = new geoDatabaseUtility();
        private rasterUtil rsUtil = new rasterUtil();
        private int pbwidth = 512;
        private int pbheight = 512;
        private int noDataVl = 256;
        public string OutRasterName { get; set; }
        private IWorkspace vWks = null;
        private IWorkspace wks = null;
        public IWorkspace OutWorkspace 
        {
            get
            {
                return wks;
            }
            set 
            {
                wks = value;
                vWks = geoUtil.OpenWorkSpace(OutWorkspace.PathName);
            }
        }
        private Dictionary<string, System.Array[]> outArrDic = new Dictionary<string, System.Array[]>();
        public int PixelBlockWidth { get { return pbwidth; } set { pbwidth = value; } }
        public int PixelBlockHeight { get { return pbheight; } set { pbheight = value; } }
        private IRaster inRs = null;
        private IRasterProps rsProp = null;
        private IRasterProps rsProps2 = null;
        private int noDataVl2 = Int32.MinValue;
        private int rws = 0;
        private int clms = 0;
        int counter = Int32.MinValue;
        private ITable vatTable = null;
        public object InRaster
        {
            get
            {
                return inRs;
            }
            set
            {
                inRs = rsUtil.returnRaster(value);
                rsProp = (IRasterProps)inRs;
                if (rsProp.PixelType == rstPixelType.PT_DOUBLE || rsProp.PixelType == rstPixelType.PT_FLOAT)
                {
                    inRs = rsUtil.convertToDifFormatFunction(inRs, rstPixelType.PT_LONG);
                    rsProp = (IRasterProps)inRs;
                }
                System.Array noDataValues = (System.Array)rsProp.NoDataValue;
                noDataVl = System.Convert.ToInt32(noDataValues.GetValue(0));
                rws = rsProp.Height;
                clms = rsProp.Width;
            }
        }
        private IRaster outraster = null;
        public IRaster OutRaster
        {
            get
            {
                return outraster;
            }
        }
        
        public void executeRegionGroup()
        {
            createOutRaster();
            if (OutRaster == null)
            {
                Console.WriteLine("not all inputs specified");
                return;
            }
            IWorkspaceEdit wksE = (IWorkspaceEdit)vWks;
            bool weEdit = true;
            if (wksE.IsBeingEdited())
            {
                weEdit = false;
            }
            else
            {
                wksE.StartEditing(false);
            }
            wksE.StartEditOperation();
            Console.WriteLine("Counter = " + counter.ToString());
            int readclmsStep = PixelBlockWidth+2;
            int readrwsStep = PixelBlockHeight+2;

            IPnt outloc = new PntClass();
            IPnt inloc = new PntClass();
            try
            {
                for (int rp = 0; rp < rws; rp += PixelBlockHeight)//rws
                {
                    for (int cp = 0; cp < clms; cp += PixelBlockWidth)//clms
                    {
                        Console.WriteLine("Write Raster location = " + cp.ToString() + ":" + rp.ToString());
                        Console.WriteLine("Read Raster location = " + (cp-1).ToString() + ":" + (rp-1).ToString());
                        outloc.SetCoords(cp, rp);
                        inloc.SetCoords(cp-1,rp-1);
                        middleRowColumn(outloc);
                    }
                }
                IRaster2 rs2 = (IRaster2)OutRaster;
                IRasterDataset rsDset = rs2.RasterDataset;
                IRasterDatasetEdit2 rsDsetE = (IRasterDatasetEdit2)rsDset;
                rsDsetE.AlterAttributeTable(vatTable);
                wksE.StopEditOperation();
                if(weEdit) wksE.StopEditing(true);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
            }
            return;

        }
        int valueIndex = -1;
        int countIndex = -1;
        int permIndex = -1;
        int rCnt = 0;
        int rPerm = 0;
        private void middleRowColumn(IPnt loc, int[] startRows, int[] startColumns)
        {
            double x = loc.X;
            double y = loc.Y;
            string outLocStr = x.ToString() + ":" + y.ToString();
            //string inLocStr = (x - 1).ToString() + ":" + (y - 1).ToString();
            IPnt readLoc = new PntClass();
            readLoc.SetCoords(x - 1, y - 1);
            IRasterEdit regRsE = (IRasterEdit)OutRaster;
            IPnt writePbSize = new PntClass();
            writePbSize.SetCoords(pbwidth, pbheight);
            IPnt readPbSize = new PntClass();
            readPbSize.SetCoords(pbwidth + 2, pbheight + 2);
            IPixelBlock pb = inRs.CreatePixelBlock(readPbSize);
            IPixelBlock pb2 = OutRaster.CreatePixelBlock(writePbSize);
            System.Array[] inOutArr;
            System.Array inArr = null;
            System.Array outArr = null;
            if (outArrDic.TryGetValue(outLocStr, out inOutArr))
            {
                inArr = inOutArr[0];
                outArr = inOutArr[1];
            }
            else
            {
                //Console.WriteLine(outLocStr);
                OutRaster.Read(loc, pb2);
                inRs.Read(readLoc, pb);
                OutRaster.Read(loc, pb2);
                inArr = (System.Array)pb.get_SafeArray(0);
                outArr = (System.Array)pb2.get_SafeArray(0);
                outArrDic[outLocStr] = new System.Array[] { inArr, outArr };
            }
            int height = pb2.Height;
            int width = pb2.Width;
            for (int si = 0; si < startRows.Length; si++)
            {
                int c = startColumns[si];
                int r = startRows[si];
                int ic = c + 1;
                int ir = r + 1;
                //Console.WriteLine(cr[0]);
                int inVl = System.Convert.ToInt32(inArr.GetValue(ic, ir));
                
                if ((inVl == noDataVl)||(inVl == (noDataVl-1)))
                {
                    Console.WriteLine("Invalue form 2 middle = " + inVl.ToString());
                    continue;
                }
                else
                {
                    int outVl32 = System.Convert.ToInt32(outArr.GetValue(c, r));
                    if (outVl32 != noDataVl2)
                    {
                    }
                    else
                    {
                        List<string> cr = new List<string>();
                        cr.Add(c.ToString() + ":" + r.ToString());
                        outArr.SetValue(counter, c, r);
                        List<int>[] nextArray = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };//determines if the next pixel block must be queried {left,top,right,bottom}   
                        while (cr.Count > 0)
                        {
                            rCnt++;
                            rPerm += findRegion(inVl, counter, noDataVl2, inArr, outArr, cr, nextArray);
                        }
                        for (int i = 0; i < nextArray.Length; i++)
                        {
                            List<int> pbNextLst = nextArray[i];
                            if (pbNextLst.Count > 0)
                            {
                                int[] startClms = new int[pbNextLst.Count];
                                int[] startRws = new int[pbNextLst.Count];
                                IPnt newLoc = new PntClass();
                                double nClP = loc.X;
                                double nRwP = loc.Y;
                                switch (i)
                                {
                                    case 0:
                                        nClP = nClP - pbwidth;
                                        startRws = pbNextLst.ToArray();
                                        int stcl = pbwidth - 1;
                                        for (int k = 0; k < startRws.Length; k++)
                                        {
                                            startClms[k] = stcl;
                                        }
                                        break;
                                    case 1:
                                        nRwP = nRwP - pbheight;
                                        startClms = pbNextLst.ToArray();//rws=pbHeight-1
                                        int strw = pbheight - 1;
                                        for (int k = 0; k < startClms.Length; k++)
                                        {
                                            startRws[k] = strw;
                                        }
                                        break;
                                    case 2:
                                        nClP = nClP + pbwidth;
                                        startRws = pbNextLst.ToArray();//clms=0;
                                        break;
                                    default:
                                        nRwP = nRwP + pbheight;
                                        startClms = pbNextLst.ToArray();//rws = 0;
                                        break;
                                }
                                if ((nClP >= 0 && nRwP >= 0 & nClP <= rsProps2.Width && nRwP <= rsProps2.Height))
                                {
                                    newLoc.SetCoords(nClP, nRwP);
                                    middleRowColumn(newLoc, startRws, startClms);
                                }
                            }
                        }
                    }
                }
            }
            
            return;
        }

        private void middleRowColumn(IPnt loc)
        {
            double x = loc.X;
            double y = loc.Y;
            string outLocStr = x.ToString() + ":" + y.ToString();
            IPnt readLoc = new PntClass();
            readLoc.SetCoords(x - 1, y - 1);
            IRasterEdit regRsE = (IRasterEdit)OutRaster;
            IPnt writePbSize = new PntClass();
            writePbSize.SetCoords(pbwidth, pbheight);
            IPnt readPbSize = new PntClass();
            readPbSize.SetCoords(pbwidth + 2, pbheight + 2);
            IPixelBlock pb = inRs.CreatePixelBlock(readPbSize);
            IPixelBlock pb2 = OutRaster.CreatePixelBlock(writePbSize);
            System.Array[] inOutArr;
            System.Array inArr = null;
            System.Array outArr = null;
            if (outArrDic.TryGetValue(outLocStr, out inOutArr))
            {
                inArr = inOutArr[0];
                outArr = inOutArr[1];
            }
            else
            {
                OutRaster.Read(loc, pb2);
                inRs.Read(readLoc, pb);
                OutRaster.Read(loc, pb2);
                inArr = (System.Array)pb.get_SafeArray(0);
                outArr = (System.Array)pb2.get_SafeArray(0);
                outArrDic[outLocStr] = new System.Array[] { inArr, outArr };
            }
            int height = pb2.Height;
            int width = pb2.Width;
            for (int c = 0; c < width; c++)
            {
                int ic = c + 1;
                for (int r = 0; r < height; r++)
                {
                    List<string> cr = new List<string>();
                    cr.Add(c.ToString()+":"+r.ToString());
                    int ir = r + 1;
                    int inVl = System.Convert.ToInt32(inArr.GetValue(ic, ir));
                    //Console.WriteLine("Invalue = " + inVl.ToString());
                    if ((inVl == noDataVl) || (inVl == (noDataVl - 1)))
                    {
                        Console.WriteLine("Invalue = " + inVl.ToString());
                        continue;
                    }
                    else
                    {
                        int outVl32 = System.Convert.ToInt32(outArr.GetValue(c, r));
                        if (outVl32 == noDataVl2)
                        {
                            rCnt = 0;
                            rPerm = 0;
                            outArr.SetValue(counter, c, r);
                            List<int>[] nextArray = { new List<int>(), new List<int>(), new List<int>(), new List<int>() };//determines if the next pixel block must be queried {left,top,right,bottom}
                            while (cr.Count > 0)
                            {
                                rCnt++;
                                rPerm += findRegion(inVl, counter, noDataVl2, inArr, outArr, cr, nextArray);
                            }
                            for (int i = 0; i < nextArray.Length; i++)
                            {
                                List<int> pbNextLst = nextArray[i];
                                if (pbNextLst.Count > 0)
                                {
                                    int[] startClms = new int[pbNextLst.Count];
                                    int[] startRws = new int[pbNextLst.Count];
                                    IPnt newLoc = new PntClass();
                                    double nClP = loc.X;
                                    double nRwP = loc.Y;
                                    switch (i)
                                    {
                                        case 0:
                                            nClP = nClP - pbwidth;
                                            startRws = pbNextLst.ToArray();
                                            int stcl = pbwidth - 1;
                                            for (int k = 0; k < startRws.Length; k++)
                                            {
                                                startClms[k] = stcl;
                                            }
                                            break;
                                        case 1:
                                            nRwP = nRwP - pbheight;
                                            startClms = pbNextLst.ToArray();//rws=pbHeight-1
                                            int strw = pbheight - 1;
                                            for (int k = 0; k < startClms.Length; k++)
                                            {
                                                startRws[k] = strw;
                                            }
                                            break;
                                        case 2:
                                            nClP = nClP + pbwidth;
                                            startRws = pbNextLst.ToArray();//clms=0;
                                            break;
                                        default:
                                            nRwP = nRwP + pbheight;
                                            startClms = pbNextLst.ToArray();//rws = 0;
                                            break;
                                    }
                                    if ((nClP >= 0 && nRwP >= 0 & nClP <= rsProps2.Width && nRwP <= rsProps2.Height))
                                    {
                                        newLoc.SetCoords(nClP, nRwP);
                                        middleRowColumn(newLoc, startRws, startClms);
                                    }
                                }
                            }
                            IRow row = vatTable.CreateRow();
                            row.set_Value(valueIndex, counter);
                            row.set_Value(countIndex, rCnt);
                            row.set_Value(permIndex, rPerm);
                            row.Store();
                            counter++;
                        }
                        else
                        {
                        }
                    }
                }
            }
            pb2.set_SafeArray(0, (System.Array)outArr);
            regRsE.Write(loc, pb2);
            outArrDic.Remove(outLocStr);
        }
        private int findRegion(int inValue, int newValue, int noDataValue, System.Array inArr, System.Array outArr, List<string> columnrow, List<int>[] nextArray)
        {
            int permEdges = 0;
            string ccr = columnrow[0];
            string[] ccrArr = ccr.Split(new char[]{':'});
            int clm = System.Convert.ToInt32(ccrArr[0]);
            int rw = System.Convert.ToInt32(ccrArr[1]);
            int maxC = outArr.GetUpperBound(0);
            int maxR = outArr.GetUpperBound(1);
            int minCR = 0;
            for (int i = 0; i < 4; i++)
            {
                int cPlus = clm;
                int rPlus = rw;
                switch (i)
                {
                    case 0:
                        cPlus += 1;
                        break;
                    case 1:
                        rPlus += 1;
                        break;
                    case 2:
                        cPlus -= 1;
                        break;
                    default:
                        rPlus -= 1;
                        break;
                }
                int pVl = System.Convert.ToInt32(inArr.GetValue(cPlus + 1, rPlus + 1));
                bool inValueCheck = (pVl==inValue);
                bool rPlusMinCheck = (rPlus < minCR);
                bool rPlusMaxCheck = (rPlus > maxR);
                bool cPlusMinCheck = (cPlus < minCR);
                bool cPlusMaxCheck = (cPlus > maxC);
                if ((rPlusMinCheck || rPlusMaxCheck || cPlusMinCheck || cPlusMaxCheck))
                {
                    if (inValueCheck)
                    {
                        if (rPlusMinCheck)
                        {
                            nextArray[0].Add(cPlus);
                        }
                        else if (rPlusMaxCheck)
                        {
                            nextArray[1].Add(cPlus);
                        }
                        if (cPlusMinCheck)
                        {
                            nextArray[2].Add(rPlus);
                        }
                        if (cPlusMaxCheck)
                        {
                            nextArray[3].Add(rPlus);
                        }
                    }
                }
                else
                {
                    //try
                    //{
                        string cr = cPlus.ToString()+":"+rPlus.ToString();
                        //Console.WriteLine(cr);
                        int cVl = System.Convert.ToInt32(outArr.GetValue(cPlus, rPlus));
                        if (cVl == noDataValue)
                        {

                            if (inValueCheck)
                            {
                                try
                                {
                                    outArr.SetValue(newValue, cPlus, rPlus);
                                    if(!columnrow.Contains(cr))
                                    {
                                        columnrow.Add(cr);
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                permEdges++;
                            }
                        }
                        else
                        {
                        }
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e.ToString());
                    //}

                }
                //try
                //{
                    columnrow.Remove(ccr);
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.ToString());
                //    columnrow.Clear();
                //}
            }
            return permEdges;

        }
        private void createOutRaster()
        {
            OutRasterName = rsUtil.getSafeOutputName(wks, OutRasterName);
            outraster = rsUtil.returnRaster(rsUtil.createNewRaster(inRs, wks, OutRasterName,rasterUtil.rasterType.IMAGINE));
            rsProps2 = (IRasterProps)OutRaster;
            System.Array noDataValues2 = (System.Array)rsProps2.NoDataValue;
            noDataVl2 = System.Convert.ToInt32(noDataValues2.GetValue(0));
            if (noDataVl2 >= 0)
            {

                counter = Int32.MinValue + 2;
            }
            else
            {
                counter = Int32.MinValue + 2;
            }
            #region create VAT Table
            IFields flds = new FieldsClass();
            IField fld = new FieldClass();
            IFieldsEdit fldsE = (IFieldsEdit)flds;
            IFieldEdit fldE = (IFieldEdit)fld;
            fldE.Name_2 = "Value";
            fldE.Type_2 = esriFieldType.esriFieldTypeInteger;
            fldE.Precision_2 = 50;
            fldsE.AddField(fld);
            IField fld2 = new FieldClass();
            IFieldEdit fld2E = (IFieldEdit)fld2;
            fld2E.Name_2 = "Count";
            fld2E.Type_2 = esriFieldType.esriFieldTypeInteger;
            fldE.Precision_2 = 50;
            fldsE.AddField(fld2);
            IField fld3 = new FieldClass();
            IFieldEdit fld3E = (IFieldEdit)fld3;
            fld3E.Name_2 = "Perimeter";
            fld3E.Type_2 = esriFieldType.esriFieldTypeInteger;
            fld3E.Precision_2 = 50;
            fldsE.AddField(fld3);
            vatTable = geoUtil.createTable(vWks, OutRasterName + "_vat", flds);
            valueIndex = vatTable.FindField("Value");
            countIndex = vatTable.FindField("Count");
            permIndex = vatTable.FindField("Perimeter");
            #endregion

        }

    }
}
