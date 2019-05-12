using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SampleModule
{
    public class MessageBody
    {
        public float result;
        public Metrics metrics = new Metrics();
    }

    public class Metrics
    {
        public int evaltimeinms { get; set; }        
        public int cycletimeinms { get; set; }        

    }
}
