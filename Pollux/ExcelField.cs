using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux
{
    public class ExcelField
    {
        private List<ExcelFieldType> listNumeric = new List<ExcelFieldType> {
            ExcelFieldType.XsdDecimal,
            ExcelFieldType.XsdDouble,
            ExcelFieldType.XsdFloat,
            ExcelFieldType.XsdInteger,
        };
        private List<ExcelFieldType> listText = new List<ExcelFieldType> {
            ExcelFieldType.XsdChar,
            ExcelFieldType.XsdString,
        };
        private List<ExcelFieldType> listDate = new List<ExcelFieldType> {
            ExcelFieldType.XsdDate,
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
                else
                {
                    throw new NotImplementedException("Tipo no Implementado.");
                }
            }
        }

        public override string ToString()
        {
            return Type.ToString().ToLower().Replace("xsd","xsd:");
        }

        public int MinNumericValue { get; set; }
        public int MaxNumericValue { get; set; }
        public int MinStringhValue { get; set; }
        public int MaxStringValue { get; set; }

        public bool IsNumeric { get; set; }
        public bool IsDate { get; set; }
        public bool IsText { get; set; }
    }

    public enum ExcelFieldType
    {
        None,
        XsdString,
        XsdChar,
        XsdInteger,
        XsdDate,
        XsdDateTime, 
        XsdFloat,
        XsdDouble, 
        XsdDecimal,
    }
}
