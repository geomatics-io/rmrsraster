using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;

namespace esriUtil.FunctionRasters.NeighborhoodHelper
{
    /// <summary>
    /// class used in conjunction with iterating through a pixel block of a given window size. This class allows you to increamentally add a row, column, or value to the window
    /// without rereading the previous rows. Should be faster than building the window for each cell. Many statics of the window are stored as properties of the class. These
    /// stats include mean, sum, variation, standard deviation, sum of squares, number of unique values, frequency of unique values, min, and max values
    /// </summary>
    public class neighborhoodhelper
    {
        /// <summary>
        /// Rectangle constructor for neighborhood helper.
        /// </summary>
        /// <param name="columns">number of columns of the window</param>
        /// <param name="rows">number of rows of the window</param>
        public neighborhoodhelper(int columns, int rows)
        {
            List<int[]> iter = new List<int[]>();
            window = rsUtil.createFocalWindowRectangle(columns, rows,out iter);
            setStartupValues();
        }
        /// <summary>
        /// Circle constructor for neighborhood helper.
        /// </summary>
        /// <param name="radius">number of cells that makeup the radius of a circle neighborhood</param>
        public neighborhoodhelper(int radius)
        {
            rad = true;
            List<int[]> iter = new List<int[]>();
            window = rsUtil.createFocalWindowCircle(radius, out iter);
            setStartupValues();
        }
        private void setStartupValues()
        {
            clms = window.GetUpperBound(0) + 1;
            rws = window.GetUpperBound(1) + 1;
            if (rad)
            {
                n = 0;
                for (int i = 0; i < clms; i++)
                {
                    List<int> oneList = new List<int>();
                    for (int j = 0; j < rws; j++)
                    {
                        int vl = window[i, j];
                        if (vl == 1) oneList.Add(j);
                        n = n + vl;
                    }
                    addColumns.Add(i, oneList.ToArray());
                }
            }
            else
            {
                n = clms * rws;
            }
            clmsArrSum = new double[clms];
            clmsArrSum2 = new double[clms];
            clmsArrMax = new double[clms];
            clmsArrMin = new double[clms];
            matrix = new double[clms, rws];
        }
        private bool rad = false;
        /// <summary>
        /// stores the position of zeros in the circle window used to subtract from rows and columns 
        /// </summary>
        private Dictionary<int, int[]> addColumns = new Dictionary<int, int[]>();
        private rasterUtil rsUtil = new rasterUtil();
        private int[,] window = null;
        /// <summary>
        /// retreive the current window
        /// </summary>
        public int[,] Window { get { return window; } }
        private bool sumUpdate = false;
        private int currentClm = 0;
        private int currentRw = 0;
        private int CurrentEditRow
        {
            get
            {
                return currentRw;
            }
            set
            {
                if (value < rws)
                {
                    currentRw = value;
                }
                else
                {
                    currentRw = 0;
                }
                sumUpdate = true;
            }
        }
        private int CurrentEditColumn
        {
            get
            {
                return currentClm;
            }
            set
            {
                if (value < clms)
                {
                    currentClm = value;
                }
                else
                {
                    currentClm = 0;
                }
                sumUpdate = true;
            }
        }
        private int clms;
        private int rws;
        private int n;
        private double[] clmsArrSum;
        private double[] clmsArrSum2;
        private double[] clmsArrMax;
        private double[] clmsArrMin;
        private double[,] matrix;
        private double totalsum2 = 0;
        private double totalsum = 0;
        private double tmin, tmax, median, mode;
        private Dictionary<int, int> tuniqvalues = new Dictionary<int, int>();
        private void updateSummaryValues()
        {
            if (sumUpdate == true)
            {
                if (rad)
                {
                    int center = rws / 2;
                    double vl = matrix[center, center];
                    tmin = vl;
                    tmax = vl;
                    totalsum = 0;
                    totalsum2 = 0;
                    tuniqvalues.Clear();
                    foreach (int c in addColumns.Keys)
                    {
                        int c2 = getWindowColumn(c);
                        foreach (int r in addColumns[c])
                        {
                            int r2 = getWindowRow(r);
                            vl = matrix[c2, r2];
                            int vli = System.Convert.ToInt32(vl);
                            if (vl > tmax) tmax = vl;
                            if (vl < tmin) tmin = vl;
                            totalsum += vl;
                            totalsum2 += vl * vl;
                            if (tuniqvalues.TryGetValue(vli, out r2))
                            {
                                tuniqvalues[vli] = r2 + 1;
                            }
                            else
                            {
                                tuniqvalues.Add(vli, 1);
                            }
                        }
                    }
                }
                else
                {
                    tmin = clmsArrMin.Min();
                    tmax = clmsArrMax.Max();
                }
            }
            sumUpdate = false;
        }
        /// <summary>
        /// gets the maximum value within the window
        /// </summary>
        public double WindowMin
        {
            get
            {
                updateSummaryValues();
                return tmin;
            }
        }
        /// <summary>
        /// gets the minimum value within the window
        /// </summary>
        public double WindowMax
        {
            get
            {
                updateSummaryValues();
                return tmax;
            }
        }
        public double WindowMedian
        {
            get
            {
                updateSummaryValues();
                int cntCompare = (n+1)/2;
                int counter = 0;
                List<int> keyLst = tuniqvalues.Keys.ToList();
                keyLst.Sort();
                foreach (int k in keyLst)
                {
                    int cnt = tuniqvalues[k];
                    counter= counter+cnt;
                    if (counter >= cntCompare)
                    {
                        median = k;
                        break;
                    }

                }
                return median;
            }
        }
        public double WindowMode
        {
            get
            {
                updateSummaryValues();
                int maxDicCntValue = tuniqvalues.Values.ToList().Max();
                foreach (KeyValuePair<int, int> kvp in tuniqvalues)
                {
                    int ky = kvp.Key;
                    int vl = kvp.Value;
                    if (vl == maxDicCntValue)
                    {
                        mode = ky;
                        break;
                    }
                }
                return mode;

            }
        }
        /// <summary>
        /// gets the variance within the window
        /// </summary>
        public double WindowVariance
        {
            get
            {
                updateSummaryValues();
                double vl = (totalsum2 - ((totalsum * totalsum) / n)) / n;
                return vl;
            }
        }
        /// <summary>
        /// gets the standard deviation within the window
        /// </summary>
        public double WindowStd
        {
            get
            {
                return Math.Sqrt(WindowVariance);
            }
        }
        /// <summary>
        /// gets the number of unique values within the window
        /// </summary>
        public double WindowUniqueValues
        {
            get
            {
                updateSummaryValues();
                return tuniqvalues.Keys.Count;
            }
        }
        public double WindowProbability
        {
            get
            {
                double tProb = 0;
                updateSummaryValues();
                foreach (int ent in tuniqvalues.Values)
                {
                    double prob = Math.Pow(System.Convert.ToDouble(ent) / n,2);
                    tProb = tProb + prob;
                }
                return tProb;
            }
        }
        public double WindowEntropyValues
        {
            get
            {
                double entropy = 0;
                updateSummaryValues();
                foreach (int ent in tuniqvalues.Values)
                {
                    double prob = System.Convert.ToDouble(ent) / n;
                    entropy = entropy + prob * Math.Log(prob);
                }
                return -1 * entropy;
            }
        }
        /// <summary>
        /// gets the number of unique values within the windwo and their associted counts
        /// </summary>
        public Dictionary<int, int> WindowUniqueValuesByCount { get { return tuniqvalues; } }
        /// <summary>
        /// gets the sum of all the values within the window
        /// </summary>
        public double WindowSum { get { updateSummaryValues(); return totalsum; } }
        /// <summary>
        /// returns the mean value of the window
        /// </summary>
        public double WindowMean { get { return (WindowSum / n); } }
        /// <summary>
        /// gets the sum of all squared values within the window
        /// </summary>
        public double WindowSumSquared { get { updateSummaryValues(); return totalsum2; } }
        private int getWindowColumn(int column)
        {
            int c = CurrentEditColumn + column;
            if (c > clms - 1)
            {
                c = c - clms;
            }
            return c;
        }
        private int getWindowRow(int row)
        {
            int r = CurrentEditRow + row;
            if (r > rws - 1)
            {
                r = r - rws;
            }
            return r;
        }
        /// <summary>
        /// gets the value within the window given a column row
        /// </summary>
        /// <param name="column">int column</param>
        /// <param name="row">int row</param>
        /// <returns>the value at that location</returns>
        public double getValue(int column, int row)
        {
            int c = getWindowColumn(column);
            int r = getWindowRow(row);
            return matrix[c, r];
        }
        /// <summary>
        /// appends a new row to the window
        /// </summary>
        /// <param name="rowArr"></param>
        public void appendRow(double[] rowArr)
        {
            double sumV = 0;
            double sumV2 = 0;
            double pRSum = 0;
            double pRSum2 = 0;
            double vl = 0;
            double oldvl = 0;
            double vl2 = 0;
            double oldvl2 = 0;
            int c, r;
            c = 0;
            r = 0;
            for (int i = 0; i < rowArr.Length; i++)
            {
                int cnt;
                c = getWindowColumn(i);
                r = CurrentEditRow;
                vl = rowArr[i];
                oldvl = matrix[c, r];
                vl2 = vl * vl;
                oldvl2 = oldvl * oldvl;
                int oldRV = System.Convert.ToInt32(oldvl);
                if (tuniqvalues.TryGetValue(oldRV, out cnt))
                {
                    int ncnt = cnt - 1;
                    if (ncnt == 0)
                    {
                        tuniqvalues.Remove(oldRV);
                    }
                    else
                    {
                        tuniqvalues[oldRV] = ncnt;
                    }
                }
                int vli = System.Convert.ToInt32(vl);
                if (tuniqvalues.TryGetValue(vli, out cnt))
                {
                    tuniqvalues[vli] = cnt + 1;
                }
                else
                {
                    tuniqvalues.Add(vli, 1);
                }
                sumV += vl;
                sumV2 += vl2;
                pRSum += oldvl;
                pRSum2 += oldvl2;
                matrix[c, r] = vl;
                clmsArrSum[c] = clmsArrSum[c] - oldvl + vl;
                clmsArrSum2[c] = clmsArrSum2[c] - (oldvl2) + (vl2);
                updateColumnMinMax(c);
            }
            totalsum = totalsum - (pRSum) + sumV;
            totalsum2 = totalsum2 - (pRSum2) + (sumV2);
            CurrentEditRow += 1;
        }
        /// <summary>
        /// appends a new column to the window
        /// </summary>
        /// <param name="columnArr"></param>
        public void appendColumn(double[] columnArr)
        {
            double max = columnArr[0];
            double min = columnArr[0];
            double sumV = 0;
            double sumV2 = 0;
            int c, r;
            c = 0;
            r = 0;
            for (int i = 0; i < columnArr.Length; i++)
            {
                c = CurrentEditColumn;
                r = getWindowRow(i);
                int cnt;
                double vl = columnArr[i];
                int oldRV = System.Convert.ToInt32(matrix[c, r]);
                if (tuniqvalues.TryGetValue(oldRV, out cnt))
                {
                    int ncnt = cnt - 1;
                    if (ncnt == 0)
                    {
                        tuniqvalues.Remove(oldRV);
                    }
                    else
                    {
                        tuniqvalues[oldRV] = ncnt;
                    }
                }
                int vli = System.Convert.ToInt32(vl);
                if (tuniqvalues.TryGetValue(vli, out cnt))
                {
                    tuniqvalues[vli] = cnt + 1;
                }
                else
                {
                    tuniqvalues.Add(vli, 1);
                }
                sumV += vl;
                sumV2 += vl * vl;
                if (vl > max) max = vl;
                if (vl < min) min = vl;

                matrix[c, r] = vl;
            }
            totalsum = (totalsum - clmsArrSum[c]) + sumV;
            totalsum2 = (totalsum2 - clmsArrSum2[c]) + sumV2;
            clmsArrSum[c] = sumV;
            clmsArrSum2[c] = sumV2;
            clmsArrMax[c] = max;
            clmsArrMin[c] = min;
            CurrentEditColumn += 1;
        }
        /// <summary>
        /// appends a value to the specified column and row
        /// </summary>
        /// <param name="vl">the value you want to update the window with</param>
        /// <param name="column">the column you want to update</param>
        /// <param name="row">the row you want to update</param>
        public void appendValue(double vl, int column, int row)
        {
            int cnt;
            double vl2 = vl * vl;
            int c = getWindowColumn(column);
            int r = getWindowRow(row);
            double oldVl = matrix[c, r];
            double oldVl2 = oldVl * oldVl;
            int oldRV = System.Convert.ToInt32(oldVl);
            if (tuniqvalues.TryGetValue(oldRV, out cnt))
            {
                int ncnt = cnt - 1;
                if (ncnt == 0)
                {
                    tuniqvalues.Remove(oldRV);
                }
                else
                {
                    tuniqvalues[oldRV] = ncnt;
                }

            }
            int vli = System.Convert.ToInt32(vl);
            if (tuniqvalues.TryGetValue(vli, out cnt))
            {
                tuniqvalues[vli] = cnt + 1;
            }
            else
            {
                tuniqvalues.Add(vli, 1);
            }


            matrix[c, r] = vl;
            totalsum = totalsum - oldVl + vl;
            totalsum2 = totalsum2 - oldVl2 + vl2;
            clmsArrSum[c] = clmsArrSum[c] - oldVl + vl;
            clmsArrSum2[c] = clmsArrSum2[c] - oldVl2 + vl2;
            updateColumnMinMax(c);
            sumUpdate = true;
        }
        private void updateColumnMinMax(int c)
        {
            double min, max;
            min = matrix[c, 0];
            max = min;
            for (int i = 0; i < rws; i++)
            {
                double vl = matrix[c, i];
                if (vl > max) max = vl;
                if (vl < min) min = vl;

            }
            clmsArrMax[c] = max;
            clmsArrMin[c] = min;
        }

    }
}
