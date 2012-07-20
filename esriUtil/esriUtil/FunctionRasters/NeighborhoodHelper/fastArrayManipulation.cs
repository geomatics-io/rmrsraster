using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    public class fastArrayManipulation
    {
        private double sum = 0;
        private double n = 0;
        private enum arrayType { DOUBLE, FLOAT, INT, BOOL }
        public System.Array getSubsetFrom2DArray(double[,] inArr, int[] startloc, int[] endloc)
        {
            return getSubsetFrom2DArray((System.Array)inArr, startloc, endloc, arrayType.DOUBLE);

        }
        public System.Array getSubsetFrom2DArray(float[,] inArr, int[] startloc, int[] endloc)
        {
            return getSubsetFrom2DArray((System.Array)inArr, startloc, endloc, arrayType.FLOAT);

        }
        public System.Array getSubsetFrom2DArray(int[,] inArr, int[] startloc, int[] endloc)
        {
            return getSubsetFrom2DArray((System.Array)inArr, startloc, endloc, arrayType.INT);
        }
        private System.Array getSubsetFrom2DArray(System.Array inArr, int[] startloc, int[] endloc, arrayType arrType)
        {
            sum = 0;
            n = 0;
            int stX = startloc[0];
            int endX = endloc[0];
            int stY = startloc[1];
            int endY = endloc[1];
            int clms = endX - stX+1;
            int rws = endY - stY + 1;
            System.Array outArray = null;
            switch (arrType)
            {
                case arrayType.DOUBLE:
                    outArray = new double[clms, rws];
                    break;
                case arrayType.FLOAT:
                    outArray = new float[clms, rws];
                    break;
                case arrayType.INT:
                    outArray = new int[clms, rws];
                    break;
                default:
                    outArray = new bool[clms, rws];
                    break;
            }
            n = clms * rws;
            for (int i = 0; i < clms; i++)
            {
                int c = stX + i;
                for (int j = 0; j < rws; j++)
                {
                    int r = stY + j;
                    double vl = System.Convert.ToDouble(inArr.GetValue(c, r));
                    sum += vl;
                    outArray.SetValue(inArr.GetValue(c, r), i, j);
                }
            }
            return outArray;


        }

    }
}
