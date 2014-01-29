using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Accord.Statistics.Testing.Power;

namespace esriUtil.Statistics
{
    public class dataPrepSampleSize
    {
        public static double zScore(double alpha=0.05)
        {
            Accord.Statistics.Distributions.Univariate.NormalDistribution nd = new Accord.Statistics.Distributions.Univariate.NormalDistribution();
            return nd.InverseDistributionFunction(1 - (alpha / 2)); 
        }
        public static int sampleSizeKappa(string AccuracyAssessmentModel, double proportionOfKappa=0.1, double alpha = 0.05)
        {
            double z = zScore(alpha);
            dataGeneralConfusionMatirx gc= new dataGeneralConfusionMatirx();
            gc.getXTable(AccuracyAssessmentModel);
            double pe = gc.GeneralConfusionMatrix.ChanceAgreement;
            double k = gc.Kappa;
            double n = (k*(1-k))/(1-pe)*Math.Pow((z/proportionOfKappa),2);
            return System.Convert.ToInt32(n);
        }
        public static int sampleSizeKappa(double kappa, double chanceAggreement, double proportionOfKappa=0.1, double alpha=0.05)
        {
            double z = zScore(alpha);
            double n = (kappa * (1 - kappa)) / (1 - chanceAggreement) * Math.Pow((z / proportionOfKappa), 2);
            return System.Convert.ToInt32(n);
        }
        public static int sampleSizeMean(double mean, double standardDeviation, double proportionOfMean,double alpha=0.05)
        {
            double z = zScore(alpha);
            double cv = standardDeviation / mean;
            //double width = mean * proportionOfMean;
            return System.Convert.ToInt32(Math.Pow((z*cv/proportionOfMean),2));
        }

        
        public static int[] sampleSizeMaxMean(double[,] varCov, double[] means, double proportionOfMean=0.1, double alpha=0.05)
        {
            int n = means.Length;
            int maxSample = 0;
            int varIndex = 0;
            for (int i = 0; i < n; i++)
            {
                double mean = means[i];
                double std = Math.Sqrt(varCov[i, i]);
                int ms = sampleSizeMean(mean, std, proportionOfMean, alpha);
                if (ms > maxSample)
                {
                    maxSample = ms;
                    varIndex = i;
                }
            }
            return new []{maxSample,varIndex};
        }
        public static int sampleSizeMaxMean(double[] means, double[] stds, double proportionOfMean=0.1, double alpha = 0.05)
        {
            int n = means.Length;
            int maxSample = 0;
            for (int i = 0; i < n; i++)
            {
                double mean = means[i];
                double std = stds[i];
                int ms = sampleSizeMean(mean, std, proportionOfMean, alpha);
                if (ms > maxSample) maxSample = ms;
            }
            return maxSample;
        }
        public static int[] sampleSizeMaxCluster(string clusterModelPath, double proportionOfMean = 0.1, double alpha = 0.05)
        {
            dataPrepClusterKmean cls = new dataPrepClusterKmean();
            cls.buildModel(clusterModelPath);
            int nClusters = ((Accord.MachineLearning.KMeans)cls.Model).Clusters.Count;
            int[] maxN = new int[nClusters];
            for (int i = 0; i < nClusters; i++)
            {
                Accord.MachineLearning.KMeansCluster k = ((Accord.MachineLearning.KMeans)cls.Model).Clusters[i];
                int mx = sampleSizeMaxMean(k.Covariance, k.Mean, proportionOfMean, alpha)[0];
                maxN[i] = mx;
            }
            return maxN;
        }
        public static double[] clusterProportions(string clusterModelPath)
        {
            dataPrepClusterKmean cls = new dataPrepClusterKmean();
            cls.buildModel(clusterModelPath);
            int nClusters = ((Accord.MachineLearning.KMeans)cls.Model).Clusters.Count;
            double[] prop = new double[nClusters];
            for (int i = 0; i < nClusters; i++)
            {
                Accord.MachineLearning.KMeansCluster k = ((Accord.MachineLearning.KMeans)cls.Model).Clusters[i];
                prop[i] = k.Proportion;
            }
            return prop;
        }

        public static void getReport(string modelPath,double proportion=0.1,double alpha=0.05)
        {
            esriUtil.Forms.RunningProcess.frmRunningProcessDialog rp = new Forms.RunningProcess.frmRunningProcessDialog(false);
            rp.addMessage("Sample size for " + modelPath + " based on variation");
            rp.stepPGBar(50);
            rp.Show();
            dataPrepBase.modelTypes mType = ModelHelper.getModelType(modelPath);
            rp.addMessage("Model Type = " + mType.ToString());
            try
            {
                switch (mType)
                {
                    case dataPrepBase.modelTypes.Accuracy:
                        fillAaReport(modelPath, rp, proportion, alpha);
                        break;
                    case dataPrepBase.modelTypes.LinearRegression:
                        fillLrReport(modelPath, rp, proportion, alpha);
                        break;
                    case dataPrepBase.modelTypes.MvlRegression:
                        fillMvrReport(modelPath, rp, proportion, alpha);
                        break;
                    case dataPrepBase.modelTypes.LogisticRegression:
                        fillLogisticReport(modelPath, rp, proportion, alpha);
                        break;
                    case dataPrepBase.modelTypes.PLR:
                        fillPlrReport(modelPath, rp, proportion, alpha);
                        break;
                    case dataPrepBase.modelTypes.CovCorr:
                        fillCovCorr(modelPath, rp, proportion, alpha);
                        break;
                    case dataPrepBase.modelTypes.PCA:
                        fillPcaReport(modelPath, rp, proportion, alpha);
                        break;
                    case dataPrepBase.modelTypes.Cluster:
                        fillCluserReport(modelPath, rp, proportion, alpha);
                        break;
                    default:
                        rp.addMessage("Can't estimate sample size for this type of model!");
                        break;
                }
            }
            catch (Exception e)
            {
                rp.addMessage(e.ToString());
            }
            finally
            {
                rp.stepPGBar(100);
                rp.enableClose();
            }


        }

        private static void fillCluserReport(string modelPath, Forms.RunningProcess.frmRunningProcessDialog rp, double proportion = 0.1, double alpha = 0.05)
        {
            dataPrepClusterKmean clus = new dataPrepClusterKmean();
            clus.buildModel(modelPath);
            List<string> lbl = clus.Labels;
            rp.addMessage("Samples by class (Cluster; number of samples)");
            rp.addMessage("-".PadRight(45, '-'));
            int[] samples = sampleSizeMaxCluster(modelPath, proportion, alpha);
            for (int i = 0; i < samples.Length; i++)
            {
                rp.addMessage("\t"+lbl[i] + "; " + samples[i].ToString());
                
            }
            rp.addMessage("-".PadRight(45, '-'));
            rp.addMessage("Total number of samples = " + samples.Sum().ToString()); 
        }

        private static void fillPcaReport(string modelPath, Forms.RunningProcess.frmRunningProcessDialog rp, double proportion = 0.1, double alpha = 0.05)
        {
            dataPrepPrincipleComponents pca = new dataPrepPrincipleComponents();
            pca.buildModel(modelPath);
            double[] meanVector = pca.MeanVector;
            double[] std = pca.StdVector;
            int numSamp = sampleSizeMaxMean(meanVector, std, proportion, alpha);
            rp.addMessage("\nTotal Number of Samples = " + numSamp.ToString());
        }

        private static void fillCovCorr(string modelPath, Forms.RunningProcess.frmRunningProcessDialog rp, double proportion = 0.1, double alpha = 0.05)
        {
            dataPrepVarCovCorr covCorr = new dataPrepVarCovCorr();
            covCorr.buildModel(modelPath);
            double[,] covCorrArr = covCorr.CovarianceMatrix;
            double[] means = covCorr.MeanVector;
            string[] varFieldNames = covCorr.VariableFieldNames;
            int[] nAndIndex = sampleSizeMaxMean(covCorrArr,means, proportion, alpha);
            string vName = varFieldNames[nAndIndex[1]];
            double mean = means[nAndIndex[1]];
            double std = covCorr.StdVector[nAndIndex[1]];
            rp.addMessage("\nTotal Number of Samples = " + nAndIndex[0].ToString() + "\n\nMax sample from variable " + vName + "\n\tMean = " + mean.ToString() + "\n\tSTD = " +std.ToString());
        }

        private static void fillPlrReport(string modelPath,Forms.RunningProcess.frmRunningProcessDialog rp,double proportion=0.1, double alpha=0.05)
        {
            throw new NotImplementedException();
        }

        private static void fillLogisticReport(string modelPath,Forms.RunningProcess.frmRunningProcessDialog rp,double proportion=0.1, double alpha=0.05)
        {
            throw new NotImplementedException();
        }

        private static void fillMvrReport(string modelPath,Forms.RunningProcess.frmRunningProcessDialog rp,double proportion=0.1, double alpha=0.05)
        {
            throw new NotImplementedException();
        }

        private static void fillLrReport(string modelPath,Forms.RunningProcess.frmRunningProcessDialog rp,double proportion=0.1, double alpha=0.05)
        {
            throw new NotImplementedException();
        }

        private static void fillAaReport(string modelPath,Forms.RunningProcess.frmRunningProcessDialog rp,double proportion=0.1, double alpha=0.05)
        {
            rp.addMessage("\nTotal Number of Samples = " + sampleSizeKappa(modelPath, proportion,alpha).ToString());
        }
        
    }
}
