using NerualNetFrame.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetFrame
{
    public static class ActiveFunctions
    {
        public static double Active(ActiveTypes type, double value, bool derivative)
        {
            switch (type)
            {
                case ActiveTypes.None:
                    if (derivative)
                        return 1;
                    return value;
                case ActiveTypes.Sigmoid:
                    return Sigmoid(value, derivative);
                case ActiveTypes.ReLU:
                    return ReLU(value, derivative);
                default:
                    return value;
            }

        }

        public static Matrix Softmax(Matrix A)
        {
            Matrix B = new Matrix(A.Rows,A.Columns);
            double max = 0;
            for (int i = 0; i < A.Rows; i++)
            {
                for (int j = 0; j < A.Columns; j++)
                {
                     if (A[i, j] > max) 
                        max = A[i, j];
                }
            }
            double sum = 0;
            for (int i = 0; i < A.Rows; i++)
            {
                for (int j = 0; j < A.Columns; j++)
                {
                    sum += Math.Pow(Math.E, A[i, j] - max);
                }
            }
            for (int i = 0; i < A.Rows; i++)
            {
                for (int j = 0; j < A.Columns; j++)
                {
                    B[i,j] = Math.Pow(Math.E, A[i, j] - max) /sum;
                }
            }
            return B;
        }
        public static double Sigmoid(double x, bool derivative = false)
        {
            double r = 1 / (1 + Math.Pow(Math.E, -1 * x));
            if (derivative) return r * (1 - r);
            return r;
        }
        public static double LeakyReLU(double x, bool derivative = false)
        {
            if (derivative)
            {
                if (x < 0)
                    return 0.01;
                else
                    return 1;
            }
            if (x < 0)
                return 0.001 * x;
            else
                return x;
        }
        public static double ReLU(double x, bool derivative = false)
        {
            if (derivative)
            {
                if (x < 0)
                    return 0;
                else
                    return 1;
            }
            if (x < 0)
                return 0;
            else
                return x;
        }
    }
    public enum ActiveTypes
    {
        None,
        Sigmoid,
        ReLU,
        SoftMax,
    }
}
