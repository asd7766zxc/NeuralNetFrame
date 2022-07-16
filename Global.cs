using NerualNetFrame.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetFrame
{
    public static class Global
    {
        public static Random rand = new Random();
        public static double learningRate = 0;
        public static double L2weightSum = 0;
        public static int weigthcount = 0;
        public static double lambda = 0.1;
        public static int inputCount = 0;
        public static int BatchSize = 0;

        public static double randNormal
        {
            get
            {
                double u1 = 1.0 - rand.NextDouble();
                double u2 = 1.0 - rand.NextDouble();
                return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); 
            }
            private set { }
        }
        public static void InitWeight(ref Matrix weight)
        {
            for (int i = 0; i < weight.Rows; i++)
            {
                for (int j = 0; j < weight.Columns; j++)
                {
                    weight[i, j] = randNormal * Math.Sqrt(1.0/ weight.Columns);
                }
            }
        }
        public static double Scalar { get { return rand.NextDouble() > 0.5 ? -1 : 1; } set { } }
        public static double Clip = 0.7;
        public static void Normalize(ref List<double> values)
        {
            double mean = values.Sum() / values.Count;
            double variance = 0;
            foreach (var i in values)
            {
                variance += (i - mean) * (i - mean);
            }
            double standardDeviation = Math.Sqrt(variance / values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = (values[i] - mean) / standardDeviation;
            }
        }
        public static void Shuffle<T>(ref List<T> values,List<int> reference)
        {
            for(int i = 0; i < values.Count; i++)
            {
                var a = values[i];
                values[i] = values[reference[i]];
                values[reference[i]] = a;
            }
        }
        /*public static void DropOut(ref Layer _layer , double rate)
        {
            var u = RandomSequence(_layer._neuron.Count);
            u.Remove((int)Math.Floor((1-rate))*u.Count);
            foreach(var i in u)
            {
                _layer._neuron[i]._disabled = true;
            }
        }*/
        public static List<int> RandomSequence(int length)
        {
            List<int> indexList = new List<int>();
            List<int> shuffledList = new List<int>();
            for(int i = 0; i <length; i++)
            {
                indexList.Add(i);
            }
            while (indexList.Count > 0)
            {
                int u = rand.Next(indexList.Count);
                shuffledList.Add(indexList[u]);
                indexList.RemoveAt(u);
            }
            return shuffledList;
        }
        public static double ScalingNormalize(ref List<double> values)
        {
            double variance = 0;
            foreach (var i in values)
            {
                variance += i * i;
            }
            double length = Math.Sqrt(variance);
            for (int i = 0; i < values.Count; i++)
            {
                values[i] = (values[i]) / length;
            }
            return length;
        }
    }
}
