using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pollux
{
    public class ValidationCollections
    {
        public List<Validation> ValidationsBody { get; set; }
        public List<Validation> ValidationsHeader { get; set; }
        public List<Validation> ValidationsFault { get; set; }

        public ValidationCollections()
        {
            ValidationsBody = new List<Validation>();
            ValidationsHeader = new List<Validation>();
            ValidationsFault = new List<Validation>();
        }
    }

    public class Validation
    {
        public string Tag { get; set; }
        public ValidationOperation Operation { get; set; }
        public List<string> Values { get; set; }
        public bool Status { get; set; }

        public Validation()
        {
            Values = new List<string>();
        }
    }

    public enum ValidationOperation
    {
        Equals,
        NotEquals,
        Major,
        Minor,
    }
}
