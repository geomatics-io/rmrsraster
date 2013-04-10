using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters
{
    class mergeFunctionDatasetMean : mergeFunctionDatasetBase
    {
        public override object getValue(int i, int c, int r, List<ESRI.ArcGIS.Geodatabase.IPixelBlock> inPbValueLst, int cntLst = 0)
        {

            float meanVl = 0;
            float cnt = 0;
            foreach (ESRI.ArcGIS.Geodatabase.IPixelBlock pb in inPbValueLst)
            {
                object inVlobj = pb.GetVal(i, c, r);
                if (inVlobj == null) continue;
                else
                {
                    meanVl += System.Convert.ToSingle(inVlobj);
                    cnt += 1;
                }
            }
            if (cnt == 0) return noDataVl;
            else
            {
                meanVl = meanVl / cnt;
                return convertValueToType(meanVl);
            }
        }

        private object convertValueToType(float meanVl)
        {
            object vl = meanVl;
            switch (RasterInfo.PixelType)
            {
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_CHAR:
                    vl = System.Convert.ToSByte(meanVl);
                    break;
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_LONG:
                    vl = System.Convert.ToInt32(meanVl);
                    break;
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_SHORT:
                    vl = System.Convert.ToInt16(meanVl);
                    break;
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_U1:
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_U2:
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_U4:
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_UCHAR:
                    vl = System.Convert.ToByte(meanVl);
                    break;
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_ULONG:
                    vl = System.Convert.ToUInt32(meanVl);
                    break;
                case ESRI.ArcGIS.Geodatabase.rstPixelType.PT_USHORT:
                    vl = System.Convert.ToUInt16(meanVl);
                    break;
                default:
                    break;
            }
            return vl;
        }
    }
}
