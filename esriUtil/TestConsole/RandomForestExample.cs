using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestConsole
{
    class RandomForestExample
    {
        class Node
    {
        public Node left;
        public Node right;
        public int[] classifier;
        public double threshold;
        public int? category = null;
    }

        class Program
        {
            //test data
            static int[,] m_givenFigures = new int[,] {
            //category 0
            {15,13,11,9,10,11, 1,0,2,1,0,1, 3,0,0,0,1,0, 1,2,3,0,2,1, 0,0,1,1,2,1},  
            {11,12,10,7,9,12,  2,1,1,3,1,0, 1,2,2,1,4,1, 0,3,2,0,1,1, 1,2,3,1,0,0},  
            {9,8,7,12,20,10,   1,0,5,5,0,2, 0,1,4,4,3,0, 5,3,2,4,0,0, 7,1,4,1,2,1},  
            {5,7,12,10,8,5,    0,3,4,2,1,1, 1,4,0,0,1,5, 3,1,1,3,4,1, 1,2,2,0,1,2},  
            {8,8,11,11,9,12,   1,2,0,0,2,2, 2,2,1,0,0,0, 1,2,2,0,0,6, 6,0,0,1,0,1},

            //category 1
            {0,7,0,4,1,0, 10,9,8,6,10,11, 0,1,2,4,2,1, 5,2,1,3,0,0, 1,3,0,0,1,2}, 
            {1,2,5,0,6,9, 12,15,7,9,8,12, 1,3,4,0,0,1, 2,0,2,1,2,2, 0,0,1,2,2,0},  
            {2,1,1,3,2,5, 9,12,9,13,9,8,  1,2,1,2,0,0, 6,3,2,1,1,1, 0,0,3,3,3,4}, 
            {1,5,5,5,0,0, 11,11,9,4,5,10, 0,0,0,3,3,4, 0,5,4,4,0,1, 0,1,2,1,0,4},  
            {0,0,1,1,2,2, 9,8,10,9,15,8,  2,1,1,0,0,0, 1,2,0,0,3,3, 1,2,1,0,0,3},

            //category 2
            {1,3,3,5,0,0, 2,1,1,3,0,0, 8,10,12,9,10,10, 0,1,5,2,7,1, 0,1,2,2,0,0}, 
            {0,1,3,3,0,1, 2,3,0,7,1,0, 10,8,9,15,6,8,   0,0,4,4,1,4, 1,1,0,1,1,4},  
            {0,2,4,0,0,1, 0,3,1,8,0,1, 9,9,10,12,8,10,  0,5,5,2,2,0, 0,3,0,3,0,1}, 
            {1,1,3,3,4,0, 0,2,1,0,1,5, 10,8,9,12,14,5,  3,1,1,0,8,1, 1,0,0,2,1,0}, 
            {0,0,1,1,2,2, 1,0,4,4,2,0, 9,9,10,12,9,10,  1,0,0,1,3,0, 3,1,1,0,1,2}, 

           //category 3
            {0,2,1,1,1,0, 0,0,4,1,0,1, 1,0,0,2,1,1, 10,12,12,9,9,9,  1,1,0,0,2,3}, 
            {1,1,0,0,1,1, 2,2,1,4,1,0, 0,2,1,3,1,0, 12,11,11,8,12,9, 0,0,0,1,1,5}, 
            {0,0,1,2,2,0, 0,1,1,0,0,5, 1,0,3,3,1,1, 10,12,9,9,8,10,  0,1,1,1,0,0}, 
            {6,0,0,1,1,1, 2,1,0,4,4,0, 0,3,0,0,1,5, 12,11,9,9,10,8,  0,1,1,3,3,0}, 
            {1,1,2,2,0,0, 2,1,0,1,0,0, 1,2,2,2,0,1, 14,9,8,8,9,9,    2,2,3,3,0,5}, 

            //category 4
            {1,1,4,0,0,0, 1,1,3,0,1,0, 2,1,3,1,0,0, 1,0,3,3,1,0, 10,9,9,8,12,11}, 
            {0,2,2,4,0,1, 0,1,0,0,1,2, 1,2,0,1,0,1, 0,1,1,5,0,1, 9,9,12,12,10,7}, 
            {5,1,0,0,0,1, 1,2,2,1,1,1, 0,0,1,1,0,0, 1,1,0,0,4,4, 10,10,9,9,10,12}, 
            {1,1,1,4,0,0, 0,0,3,5,0,0, 1,1,1,2,2,3, 3,3,3,0,1,1, 10,10,12,8,8,8}, 
            {0,0,3,3,0,1, 1,3,1,0,1,1, 0,0,1,1,0,0, 1,1,0,0,3,1, 12,9,9,18,4,7}, 
        };
            //
            static int[] m_knownCategories = { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4 };
            static int[] m_trainingSample = { 0, 1, 5, 6, 10, 11, 15, 16, 20, 21 };
            const int m_nNoiseLevel = 5; //this limit for random integer to be added to every matrix element as noise
            //end

            private static int[][] m_data = null;
            private static int m_nRows;
            private static int m_nCols;
            private static int m_nCategories;
            private static int m_sizeOfRandomSample;

            //Printing functions
            private static void showData()
            {
                for (int i = 0; i < m_nRows; ++i)
                {
                    for (int j = 0; j < m_nCols; ++j)
                    {
                        Console.Write(" {0,2} ", m_data[i][j]);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            private static void showSample(int[] sample)
            {
                foreach (int x in sample)
                {
                    Console.Write(" {0,2} ", x);
                }
                Console.WriteLine();
            }

            private static void showLeft(int[] sample, byte[] indicator)
            {
                int cnt = 0;
                foreach (int x in sample)
                {
                    if (indicator[cnt] > 0)
                    {
                        Console.Write(" {0,2} ", x);
                    }
                    ++cnt;
                }
                Console.WriteLine();
            }

            private static void showRight(int[] sample, byte[] indicator)
            {
                int cnt = 0;
                foreach (int x in sample)
                {
                    if (indicator[cnt] == 0)
                    {
                        Console.Write(" {0,2} ", x);
                    }
                    ++cnt;
                }
                Console.WriteLine();
            }
            //end printing functions

            static private void InitializeData()
            {
                m_nRows = m_givenFigures.GetLength(0);
                m_nCols = m_givenFigures.GetLength(1);
                m_data = new int[m_nRows][];
                for (int i = 0; i < m_nRows; ++i)
                {
                    m_data[i] = new int[m_nCols];
                }
                Random rnd = new Random();
                for (int i = 0; i < m_nRows; ++i)
                {
                    for (int j = 0; j < m_nCols; ++j)
                    {
                        m_data[i][j] = m_givenFigures[i, j] + rnd.Next(m_nNoiseLevel);
                    }
                }
                int max = 0;
                for (int i = 0; i < m_knownCategories.Length; ++i)
                {
                    if (m_knownCategories[i] > max) max = m_knownCategories[i];
                }
                m_nCategories = max + 1;
                m_sizeOfRandomSample = (int)(0.64 * (double)(m_trainingSample.Length));
            }

            private static int[] getRandomSampleOutOfTrainingSet()
            {
                Random rnd = new Random();
                HashSet<int> hash = new HashSet<int>();
                hash.Clear();
                int[] sample = new int[m_sizeOfRandomSample];
                int cnt = 0;
                do
                {
                    int randomInt = rnd.Next(m_trainingSample.Length);
                    if (!hash.Contains(randomInt))
                    {
                        sample[cnt++] = m_trainingSample[randomInt];
                        hash.Add(randomInt);
                    }
                } while (cnt < m_sizeOfRandomSample);
                return sample;
            }

            private static int[] makeClassifier(int[] sample)
            {
                int[] counter = new int[m_nCategories];
                for (int i = 0; i < m_nCategories; ++i)
                {
                    counter[i] = 0;
                }
                foreach (int x in sample)
                {
                    ++counter[m_knownCategories[x]];
                }
                int selectedCategory = -1;
                int selectedCounter = -1;
                for (int i = 0; i < m_nCategories; ++i)
                {
                    if (counter[i] > selectedCounter)
                    {
                        selectedCounter = counter[i];
                        selectedCategory = i;
                    }
                }
                int cnt = 0;
                foreach (int x in sample)
                {
                    if (m_knownCategories[x] == selectedCategory)
                    {
                        ++cnt;
                    }
                }
                int[] vectors = new int[cnt];
                cnt = 0;
                foreach (int x in sample)
                {
                    if (m_knownCategories[x] == selectedCategory)
                    {
                        vectors[cnt++] = x;
                    }
                }

                int[] classifier = new int[m_nCols];
                for (int i = 0; i < m_nCols; ++i)
                {
                    classifier[i] = 0;
                    foreach (int x in vectors)
                    {
                        classifier[i] += m_data[x][i];
                    }
                }
                return classifier;
            }

            private static double getCosine(int[] s1, int[] s2)
            {
                double product = 0.0;
                double sqr1 = 0.0;
                double sqr2 = 0.0;
                for (int i = 0; i < s1.Length; ++i)
                {
                    product += (double)(s1[i]) * (double)(s2[i]);
                    sqr1 += (double)(s1[i]) * (double)(s1[i]);
                    sqr2 += (double)(s2[i]) * (double)(s2[i]);
                }
                return product / Math.Sqrt(sqr1) / Math.Sqrt(sqr2);
            }

            private static double getThreshold(int[] sample, int[] classifier)
            {
                double fmin = 1000.0;
                double fmax = -1.0;
                int[] s = new int[m_nCols];
                foreach (int x in sample)
                {
                    double f = getCosine(m_data[x], classifier);
                    if (f < fmin) fmin = f;
                    if (f > fmax) fmax = f;
                }
                return (fmax + fmin) / 2.0;
            }

            private static Node nodeFactory(int[] sample)
            {
                Node node = new Node();
                if (sample.Length == 1)
                {
                    node.category = m_knownCategories[sample[0]];
                    return node;
                }

                node.classifier = makeClassifier(sample);
                node.threshold = getThreshold(sample, node.classifier);
                return node;
            }

            private static byte[] splitSample(Node node, int[] sample)
            {
                byte[] indicator = new byte[sample.Length];
                int cnt = 0;
                foreach (int x in sample)
                {
                    double f = getCosine(m_data[x], node.classifier);
                    if (f >= node.threshold) indicator[cnt] = 1;
                    else indicator[cnt] = 0;
                    ++cnt;
                }

                bool isAllZeros = true;
                for (int i = 0; i < sample.Length; ++i)
                {
                    if (indicator[i] != 0)
                    {
                        isAllZeros = false;
                        break;
                    }
                }
                if (isAllZeros == true) indicator[0] = 1;

                bool isAllOnes = true;
                for (int i = 0; i < sample.Length; ++i)
                {
                    if (indicator[i] == 0)
                    {
                        isAllOnes = false;
                        break;
                    }
                }
                if (isAllOnes == true) indicator[0] = 0;

                return indicator;
            }

            private static void makeLeftAndRightNodes(Node node, int[] sample)
            {
                byte[] indicator = splitSample(node, sample);
                int nLeftVectors = 0;
                foreach (byte b in indicator)
                {
                    if (b > 0) ++nLeftVectors;
                }
                int nRightVectors = indicator.Length - nLeftVectors;
                int[] left = new int[nLeftVectors];
                int[] right = new int[nRightVectors];
                int cntL = 0;
                int cntR = 0;
                int cnt = 0;
                foreach (byte b in indicator)
                {
                    if (b > 0)
                    {
                        left[cntL++] = sample[cnt];
                    }
                    else
                    {
                        right[cntR++] = sample[cnt];
                    }
                    ++cnt;
                }
                node.left = nodeFactory(left);
                node.right = nodeFactory(right);
                if (node.left.category == null)
                {
                    makeLeftAndRightNodes(node.left, left);
                }
                if (node.right.category == null)
                {
                    makeLeftAndRightNodes(node.right, right);
                }
            }

            private static int? getCategoryFromTree(Node node, int[] data)
            {
                if (node.category != null)
                {
                    return node.category;
                }

                double f = getCosine(node.classifier, data);
                if (f > node.threshold)
                {
                    return getCategoryFromTree(node.left, data);
                }
                else
                {
                    return getCategoryFromTree(node.right, data);
                }
            }

            private static bool isTrainingSample(int k)
            {
                for (int i = 0; i < m_trainingSample.Length; ++i)
                {
                    if (k == m_trainingSample[i]) return true;
                }
                return false;
            }

            static void RunRandomForest(string[] args)
            {
                InitializeData();
                int[] randomSample = getRandomSampleOutOfTrainingSet();

                //next, we build a single tree, which is provided completely by top node
                Node topnode = nodeFactory(randomSample);
                if (topnode.category == null)
                {
                    makeLeftAndRightNodes(topnode, randomSample);
                }
                //the tree is completed at this point

                //this is categorization part
                //it is built for a single tree constructed for a single random sample out of training set 
                //one tree can't provide high accuracy, the forest is needed
                for (int i = 0; i < m_nRows; ++i)
                {
                    if (isTrainingSample(i)) continue;
                    int? category = getCategoryFromTree(topnode, m_data[i]);
                    Console.WriteLine("row = {0,3}, identified category = {1,3}, correct category = {2,3}", i, category, m_knownCategories[i]);
                }

                //for the real test the training data and categorization data
                //should be different, many random samples from training data
                //should be taken and the tree should be constructed for each
                //sample, each vector for categorization data shold be passed
                //via every tree and mode for all return categories should be
                //taked as the answer
            }
        }
    }
}
