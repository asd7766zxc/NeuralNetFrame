using NerualNetFrame;
using NerualNetFrame.Tools;
using System.Diagnostics;

Matrix N = Matrix.Instance; //空矩陣
Matrix I = Matrix.Indentity(1000); //10x10的單位矩陣
Matrix A = new Matrix(1000); //10x10方陣
Stopwatch sw = new Stopwatch();
sw.Start();
//var B = I * A;
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds+"ms");

//Recording the training process
List<double> loss = new List<double>();



/// <summary>
/// Main Training Section 
/// </summary>
Model _model = new Model();
_model.initialize(new int[] { 784,128, 64, 10 });

List<Matrix> _test = new List<Matrix>();
List<Matrix> _input = new List<Matrix>();

var set = MNIST.Training_DataSet();
var seed = Global.RandomSequence(set[0].Count);
_input = set[0];
_test = set[1];
//Global.Shuffle(ref _test, seed);
//Global.Shuffle(ref _input, seed);
_model.PutData(_input, _test);
int training = 30000;
int epoch = 1;
for (int r = 0; r < epoch; r++)
{
    int s = 128;
    int rs = 128;
    int hee = 1000;
    int eeh = 1000;

    for (int i = 0; i < training; i++)
    {
        _model.Train(i);
        if (i > rs)
        {
            _model.UpdateParameter(s, 0.1, 0.9);
            rs += s;
        }
        if(i > eeh)
        {
            double los = _model.Cost();
            Console.WriteLine($"Epoch {r}/{epoch} Loss: " + los + " " + ((double)i / training)*100 + "%");
            eeh += hee;
        }
    }
}

int correct = 0;
int total = 0;
int testcount = 4000;
for (int i = training; i < training + testcount; i++)
{
    int a1 = OneHotToIndex(_model.Predict(_input[i]));
    int b1 = OneHotToIndex(_test[i]);
    total++;
    if(a1 == b1)
    {
        correct++;
    }
}
Console.WriteLine( "AC ratio : "+ (double)correct/ (double)total);

ModelSaver.SaveModel(_model, "OptimizedModel1.mdl");
string path = Directory.GetCurrentDirectory();
File.WriteAllLines(path + @"\Model\loss.txt", loss.Select((i,x)=>x.ToString() +","+ i));
int OneHotToIndex(Matrix m)
{
    int a = 0;
    for (int i = 0; i < m.Rows; i++)
    {
        if (m[i, 0] == 1) a = i;
    }
    return a;
}