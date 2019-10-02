using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SampleModule
{
    public class MessageBody
    {
        public LabelResult[] results;
        public Metrics metrics = new Metrics();
    }

    public class LabelResult
    {
        public string label { get; set; }
        public double confidence { get; set; }
    }

    public class Metrics
    {
        public int evaltimeinms { get; set; }        
        public int cycletimeinms { get; set; }        
    }

    public class ResultPayload
    {
        public OverallStats Statistics { get; set; } = new OverallStats();
        public MessageBody Result { get; set; } = null;
        public string ImageSnapshot { get; set; } = "";
    }

    public class OverallStats
    {
        public string Platform { get; set; } = "";
        public DateTimeOffset ModuleStartDate { get; set; } = DateTimeOffset.UtcNow;
        public string CurrentVideoDeviceId { get; set; } = "";
        public string CurrentOnnxModel { get; set; } = "";
        public bool OnnxModelLoaded { get; set; } = false;
        public bool IsGpu { get; set; } = false;
        public bool DeviceInitialized { get; set; } = false;
        public long TotalFrames { get; set; } = 0;
        public long TotalEvaluations { get; set; } = 0;
        public long TotalEvaluationSuccessCount { get; set; } = 0;
        public long TotalEvaluationFailCount { get; set; } = 0;
        public double AverageEvaluationMs { get; set; } = 0.0;
        public double AverageConfidence { get; set; } = 0.0;
        public List<LabelSummary> MatchSummary { get; set; } = new List<LabelSummary>();
    }

    public class LabelSummary
    {
        public string Label { get; set; } = "";
        public long TotalMatches { get; set; } = 0;
    }

}
