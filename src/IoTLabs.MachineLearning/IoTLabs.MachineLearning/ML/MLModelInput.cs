using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;

namespace IoTLabs.MachineLearning.ML
{
    public sealed class MLModelInput
    {
        public TensorFloat FloatInput { get; set; } //shape(1,4)
    }
}
