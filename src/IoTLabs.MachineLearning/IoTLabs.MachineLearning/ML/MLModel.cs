using System;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Storage.Streams;

namespace IoTLabs.MachineLearning.ML
{
    public sealed class MLModel
    {
        private LearningModel _model;
        private LearningModelSession _session;
        private LearningModelBinding _binding;

        public async Task<MLModelVariable> EvaluateAsync(MLModelVariable input)
        {
            _binding.Bind("float_input", input.Variable);

            var id = Guid.NewGuid().ToString();
            var result = await _session.EvaluateAsync(_binding, id);

            return new MLModelVariable
            {
                Variable = result.Outputs["variable"] as TensorFloat
            };
        }

        public static async Task<MLModel> CreateFromStreamAsync(IRandomAccessStreamReference stream)
        {
            var device = new LearningModelDevice(LearningModelDeviceKind.Cpu);

            var model = new MLModel();
            model._model = await LearningModel.LoadFromStreamAsync(stream);
            model._session = new LearningModelSession(model._model, device);
            model._binding = new LearningModelBinding(model._session);

            return model;
        }
    }
}
