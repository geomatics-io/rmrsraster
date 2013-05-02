using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Accord.Statistics.Testing;
using Accord.MachineLearning;

namespace esriUtil.Statistics
{
    public class dataPrepCompareVarCov
    {
        public static void PairedTTestPValues(double[] popMeans, double[,] popVarCov, double[] sampMeans, double[,] sampVarCov, out double meanPvalue, out double varPvalue)
        {
            double n = popMeans.Length;
            meanPvalue = 0;
            varPvalue = 0;
            double sumM = 0;
            double sumV = 0;
            double sumM2 = 0;
            double sumV2 = 0;
            for (int i = 0; i < n; i++)
            {
                double m1 = popMeans[i];
                double m2 = sampMeans[i];
                double dm = m1 - m2;
                double v1 = popVarCov[i, i];
                double v2 = sampVarCov[i, i];
                double dv = v1 - v2;
                sumM += dm;
                sumV += dv;
                sumM2 += dm * dm;
                sumV2 += dv * dv;
            }
            double mM = sumM / n;
            double mV = sumV / n;
            double seM = Math.Sqrt((sumM2 - ((sumM * sumM) / n)) / n) / Math.Sqrt(n);
            double seV = Math.Sqrt((sumV2 - ((sumV * sumV) / n)) / n) / Math.Sqrt(n);
            double mTstat = mM / seM;
            double vTstat = mV / seV;
            Accord.Statistics.Distributions.Univariate.TDistribution tDist = new Accord.Statistics.Distributions.Univariate.TDistribution(n - 1);
            double mCdf = tDist.DistributionFunction(mTstat);
            double vCdf = tDist.DistributionFunction(vTstat);
            if (mTstat > 0)
            {
                meanPvalue = (1 - mCdf) * 2;
            }
            else
            {
                meanPvalue = mCdf * 2;
            }
            if (vTstat > 0)
            {
                varPvalue = (1 - vCdf) * 2;
            }
            else
            {
                varPvalue = vCdf * 2;
            }
        }
        public static void CompareStratMeansVar(string StratModel1, string StratModel2, out List<string> labels, out double[] meanDiff, out double[] varDiff, out double[] meanPvalues, out double[] varPvalues)
        {
            meanPvalues = null;
            varPvalues = null;
            meanDiff = null;
            varDiff = null;
            labels = null;
            dataPrepCluster dpc1 = new dataPrepCluster();
            dpc1.buildModel(StratModel1);
            KMeans km1 = dpc1.Model;
            dataPrepCluster dpc2 = new dataPrepCluster();
            dpc2.buildModel(StratModel2);
            List<string> labels2 = dpc2.Labels;
            KMeans km2 = dpc2.Model;
            int nPv1 = km1.Clusters.Count;
            int nPv2 = km2.Clusters.Count;
            if (nPv1 != nPv2)
            {
                System.Windows.Forms.MessageBox.Show("Not the same number of strata! Models are not comparable!");
                return;
            }
            labels = dpc1.Labels;
            meanPvalues = new double[nPv1];
            varPvalues = new double[nPv2];
            meanDiff = new double[nPv1];
            varDiff = new double[nPv2];
            foreach (string l in labels)
            {
                int ind1 = labels.IndexOf(l);
                int ind2 = labels2.IndexOf(l);

                KMeansCluster kmC1 = km1.Clusters[ind1];
                KMeansCluster kmC2 = km2.Clusters[ind2];
                double[] means1 = kmC1.Mean;
                double[] means2 = kmC2.Mean;
                double[,] cov1 = kmC1.Covariance;
                double[,] cov2 = kmC2.Covariance;
                double[] meanDiffArr = new double[means1.Length];
                double[] varDiffArr = new double[means1.Length];
                for (int i = 0; i < means1.Length; i++)
                {
                    meanDiffArr[i] = means1[i] - means2[i];
                    varDiffArr[i] = cov1[i, i] - cov2[i, i];
                }
                meanDiff[ind1] = meanDiffArr.Average();
                varDiff[ind1] = varDiffArr.Average();
                double m, v;
                PairedTTestPValues(means1, cov1, means2, cov2, out m, out v);
                meanPvalues[ind1] = m;
                varPvalues[ind1] = v;
            }
        }
        public static void CompareStratMeansVar(KMeans km1, KMeans km2, out double[] meanPvalues, out double[] varPvalues)
        {
            meanPvalues = null;
            varPvalues = null;
            int nPv1 = km1.Clusters.Count;
            int nPv2 = km2.Clusters.Count;
            if (nPv1 != nPv2)
            {
                System.Windows.Forms.MessageBox.Show("Not the same number of strata! Models are not comparable!");
                return;
            }
            meanPvalues = new double[nPv1];
            varPvalues = new double[nPv2];
            for (int i = 0; i < nPv1; i++)
            {
                KMeansCluster kmC1 = km1.Clusters[i];
                KMeansCluster kmC2 = km2.Clusters[i];
                double[] means1 = kmC1.Mean;
                double[] means2 = kmC2.Mean;
                double[,] cov1 = kmC1.Covariance;
                double[,] cov2 = kmC2.Covariance;
                double m, v;
                PairedTTestPValues(means1, cov1, means2, cov2, out m, out v);
                meanPvalues[i] = m;
                varPvalues[i] = v;
            }
        } 
    }

}
