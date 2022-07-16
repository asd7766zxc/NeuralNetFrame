using NerualNetFrame.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerualNetFrame
{
    [Serializable]
    public class Model
    {
        //Dataset
        List<Matrix> Input = new List<Matrix>();
        List<Matrix> Test = new List<Matrix>();

        //Parameters 
        List<Matrix[]> Neurons = new List<Matrix[]>();// Neurons -> [0] for z [1] for Active(z)
        List<Matrix> Weights = new List<Matrix>();
        List<Matrix> Bias = new List<Matrix>();

        //Derivatives
        List<Matrix> d_z = new List<Matrix>();
        List<Matrix> d_a = new List<Matrix>();
        List<Matrix> d_weights = new List<Matrix>();
        List<Matrix> d_bias = new List<Matrix>();

        //Optimization
        List<Matrix> weights_velocity = new List<Matrix>();
        List<Matrix> bias_velocity = new List<Matrix>();

        //Configs
        int TrainingIndex = 0;
        public void PutData(List<Matrix> _input, List<Matrix>  _test)
        {
            Input = _input;
            Test = _test;
        }
        public void Train(int index)
        {
            TrainingIndex = index;
            ForwardPass();
            BackPropagation();
        }
        public void BackPropagation()
        {
            //輸出層的 z 和 Loss Function 的偏微分 dL/dz
            int last = Neurons.Count - 1;
            d_z[last] = Neurons[last][1] - Test[TrainingIndex];
            d_weights[d_weights.Count - 1] += d_z[last] * Neurons[last - 1][1].Transpose();
            d_bias[last] = d_z[last];
            int windex = 0;
            for (int i = last; i >= 1; i--)
            {
                if (i - 1 == 0) break;
                d_a[i - 1] = Weights[Weights.Count - 1- windex].Transpose() * d_z[i];
                d_z[i - 1] = Active(Neurons[i - 1][0], true) & d_a[i - 1];
                d_weights[d_weights.Count - 2- windex] += d_z[i - 1] * Neurons[i - 2][1].Transpose();
                d_bias[i - 1] = d_z[i - 1];
                windex++;
            }
        }
        public void initialize(int[] layer)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                Matrix[] _Matrix = new Matrix[2];
                _Matrix[0] = new Matrix(layer[i], 1);
                _Matrix[1] = new Matrix(layer[i], 1);
                Neurons.Add(_Matrix);
                Bias.Add(new Matrix(layer[i], 1));
                d_bias.Add(new Matrix(layer[i], 1));
                bias_velocity.Add(new Matrix(layer[i], 1));
                d_z.Add(new Matrix(layer[i], 1));
                d_a.Add(new Matrix(layer[i], 1));
                if (i > 0)
                {
                    Matrix? _weight = new Matrix(layer[i], layer[i - 1]);
                    Global.InitWeight(ref _weight);
                    Weights.Add(_weight);
                    d_weights.Add(new Matrix(layer[i], layer[i - 1]));
                    weights_velocity.Add(new Matrix(layer[i], layer[i - 1]));
                }

            }
        }
        public void ForwardPass(Matrix? input = null)
        {
            if (input == null)
                Neurons[0][1] = Input[TrainingIndex];
            else
                Neurons[0][1] = input.Value;
            for (int i = 1; i < Neurons.Count; i++)
            {
                Neurons[i][0] = Weights[i - 1] * Neurons[i - 1][1] + Bias[i];
                if(i == Neurons.Count -1)
                    Neurons[i][1] = ActiveFunctions.Softmax(Neurons[i][0]);
                else
                    Neurons[i][1] = Active(Neurons[i][0]);
            }
        }
        public void UpdateParameter(int size,double lr,double beta)
        {
            for (int i = 0; i < Weights.Count; i++)
            {
                weights_velocity[i] = beta * weights_velocity[i] + (1 - beta) * (1.0 / size) * d_weights[i];
                Weights[i] = Weights[i] - lr * weights_velocity[i];
                d_weights[i] = 0 * d_weights[i];
            }
            for (int i = 0; i < Bias.Count; i++)
            {
                bias_velocity[i] = beta * bias_velocity[i] + (1 - beta) * (1.0 / size) * d_bias[i];
                Bias[i] = Bias[i] - lr * bias_velocity[i];
                d_bias[i] = 0 * d_bias[i];
            }
        }
        public Matrix Active(Matrix input, bool derivative = false)
        {
            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < input.Columns; j++)
                {
                    input[i, j] = ActiveFunctions.LeakyReLU(input[i, j], derivative);
                }
            }
            return input;
        }
        public double Cost()
        {
            var A = Neurons.LastOrDefault()[1] - Test[TrainingIndex];
            double L2 = 0;
            for(int i = 0; i < A.Rows; i++)
            {
                for(int j = 0; j < A.Columns; j++)
                {
                    L2 += Math.Pow(A[i, j],2);
                }
            }
            return Math.Sqrt(L2);
        }
        public Matrix Predict(Matrix _input)
        {
            ForwardPass(_input);
            double max = 0;
            int index_x = 0;
            int index_y = 0;
            var u = Neurons.LastOrDefault()[1];
            for(int i = 0; i < u.Rows; i++)
            {
                for(int j = 0;j < u.Columns; j++)
                {
                    if(u[i,j] > max)
                    {
                        max = u[i, j];
                        index_x = i;
                        index_y  = j;
                    }
                }
            }
            Matrix pred=new Matrix(u.Rows,u.Columns);
            pred[index_x,index_y] = 1;
            return pred;

        }
    }
}
