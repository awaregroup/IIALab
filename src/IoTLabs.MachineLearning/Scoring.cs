using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.AI.MachineLearning;

using static EdgeModuleSamples.Common.AsyncHelper;
using System.Threading;

namespace SampleModule
{
    
    public sealed class MLModelVariable
    {
        public TensorFloat Variable; 
    }
    
    public sealed class MLModel
    {
        private LearningModel _model;
        private LearningModelSession _session;
        private LearningModelBinding _binding;

        public async Task<MLModelVariable> EvaluateAsync(MLModelVariable input)
        {
            _binding.Bind("float_input", input.Variable);

            var id = Guid.NewGuid().ToString();
            var wait = _session.EvaluateAsync(_binding, id);
            while (wait.Status != Windows.Foundation.AsyncStatus.Completed) { Thread.Sleep(100); }
            var result = wait.GetResults();

            return new MLModelVariable
            {
                Variable = result.Outputs["variable"] as TensorFloat
            };
        }

        public static async Task<MLModel> CreateFromStreamAsync(IRandomAccessStreamReference stream)
        {
            var device = new LearningModelDevice(LearningModelDeviceKind.Cpu);

            var model = new MLModel();
            var load = LearningModel.LoadFromStreamAsync(stream);
            while (load.Status != Windows.Foundation.AsyncStatus.Completed) { Thread.Sleep(100); }
            model._model = load.GetResults();

            model._session = new LearningModelSession(model._model, device);
            model._binding = new LearningModelBinding(model._session);

            return model;
        }
    }
}
