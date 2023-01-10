using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCATimerWPF
{
    internal class Timer
    {
        public double createdAt { get; set; }
        public DateTime createdAtToDateTime()
        {
            var time = TimeSpan.FromMilliseconds(this.createdAt);
            return new DateTime(1970, 1, 1) + time;
        }
        public double endTime { get; set; }
        public DateTime endTimeToDateTime()
        {
            var time = TimeSpan.FromMilliseconds(this.endTime);
            return new DateTime(1970, 1, 1) + time;
        }
        public string? message { get; set; }
    }
}
