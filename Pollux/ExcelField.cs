using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class ExcelField
    {
        private List<ExcelFieldType> listGuid = new List<ExcelFieldType>
        {
            ExcelFieldType.XsdGuid,
        };
        private List<ExcelFieldType> listNumeric = new List<ExcelFieldType> {
            ExcelFieldType.XsdShort,
            ExcelFieldType.XsdInt,
            ExcelFieldType.XsdInteger,
            ExcelFieldType.XsdFloat,
            ExcelFieldType.XsdDouble,
            ExcelFieldType.XsdDecimal,
        };
        private List<ExcelFieldType> listText = new List<ExcelFieldType> {
            ExcelFieldType.XsdString,
        };
        private List<ExcelFieldType> listDate = new List<ExcelFieldType> {
            ExcelFieldType.XsdDateTime,
        };
        
        public string Name { get; set; }
        public string Value { get; set; }

        private ExcelFieldType type;
        public ExcelFieldType Type {
            get {
                return type;
            }
            set {
                type = value;
                if (listNumeric.Any(x => x == type))
                {
                    IsNumeric = true;
                }
                else if (listDate.Any(x => x == type))
                {
                    IsDate = true;
                }
                else if (listText.Any(x => x == type))
                {
                    IsText = true;
                }
                else if (listGuid.Any(x => x == type))
                {
                    IsGuid = true;
                }
                else
                {
                    //throw new NotImplementedException("Tipo no Implementado.");
                }
            }
        }

        public bool IsTester { get; set; }

        public override string ToString()
        {
            return Type.ToString().ToLower().Replace("xsd","xsd:");
        }

        public int MinNumericValue { get; set; }
        public int MaxNumericValue { get; set; }
        public int MinStringhValue { get; set; }
        public int MaxStringValue { get; set; }

        public string MinValue {
            get {
                if (IsNumeric)
                {
                    return "0";
                }
                else if (IsGuid)
                {
                    return Guid.NewGuid().ToString();
                }
                else if (IsDate)
                {
                    return DateTime.MinValue.ToLongDateString();
                }
                else if (IsText)
                {
                    return "A";
                }
                else
                {
                    return Excel.KeyNull;
                }
            }
        }

        public bool IsNumeric { get; private set; }
        public bool IsDate { get; private set; }
        public bool IsText { get; private set; }
        public bool IsGuid { get; private set; }
    }

    public enum ExcelFieldType
    {
        None,

        XsdGuid,

        XsdBoolean,

        XsdString,

        XsdDateTime,

        XsdShort,
        XsdInt,
        XsdInteger,
        XsdFloat,
        XsdDouble, 
        XsdDecimal,
    }
}
