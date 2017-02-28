using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pollux
{
    public class Validation
    {
        public string Tag { get; set; }
        public ValidationOperation Operation { get; set; }
        public string Value { get; set; }
        public bool Status { get; set; }
    }

    public enum ValidationOperation
    {
        Equals,
        NotEquals,
        Major,
        Minor,
    }
}
