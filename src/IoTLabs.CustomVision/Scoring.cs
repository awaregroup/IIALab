using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.AI.MachineLearning;

using static EdgeModuleSamples.Common.AsyncHelper;

namespace SampleModule
{
    
    public sealed class ScoringInput
    {
        public ImageFeatureValue data; // BitmapPixelFormat: Bgra8, BitmapAlphaMode: Premultiplied, width: 227, height: 227
    }
    
    public sealed class ScoringOutput
    {
        public TensorString classLabel; // shape(-1,1)
        public IList<Dictionary<string,float>> loss;
    }
    
    public sealed class ScoringModel
    {
        private LearningModel model;
        private LearningModelSession session;
        private LearningModelBinding binding;
        public static async Task<ScoringModel> CreateFromStreamAsync(IRandomAccessStreamReference stream, bool UseGpu = false)
        {
            ScoringModel learningModel = new ScoringModel();
            learningModel.model = await AsAsync( LearningModel.LoadFromStreamAsync(stream));
            var device = new LearningModelDevice(UseGpu ? LearningModelDeviceKind.DirectXHighPerformance : LearningModelDeviceKind.Cpu );
            learningModel.session = new LearningModelSession(learningModel.model,device);
            learningModel.binding = new LearningModelBinding(learningModel.session);
            return learningModel;
        }
        public async Task<ScoringOutput> EvaluateAsync(ScoringInput input)
        {
            binding.Bind("data", input.data);
            var result = await AsAsync(session.EvaluateAsync(binding, "0"));
            var output = new ScoringOutput();
            output.classLabel = result.Outputs["classLabel"] as TensorString;
            output.loss = result.Outputs["loss"] as IList<Dictionary<string,float>>;
            return output;
        }
    }
}
