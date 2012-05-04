using System.ServiceModel;

namespace ServerDemo
{
    public class MathModule
    {
        [OperationContract]
        public int Sum(int x, int y)
        {
            return x + y;
        }
    }
}