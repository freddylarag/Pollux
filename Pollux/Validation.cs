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
        private string tag;
        public string Tag {
            get {
                return tag;
            }
            set {
                if (value.IndexOf(Prefix) == 0)
                {
                    tag = value;
                }
                else
                {
                    tag = (Prefix + ".").Replace("..", ".") + value;
                }
            }
        }

        public ValidationOperation Operation { get; set; }
        public List<string> Values { get; set; }
        public bool Status { get; set; }
        public string Prefix { get; set; }


        public Validation(string prefijo)
        {
            Prefix = prefijo;
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
