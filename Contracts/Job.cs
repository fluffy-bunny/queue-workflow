using System;
using System.Text.Json.Serialization;

namespace Contracts
{
    public class Job
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime IssuedTime { get; set; }
    }
}
