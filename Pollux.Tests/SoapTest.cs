// <copyright file="SoapTestSoap.cs"></copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pollux;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Moq;

namespace Pollux.Tests
{
    /// <summary>This class contains parameterized unit tests for Soap</summary>
    //[PexClass(typeof(SoapManager))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    //[PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class SoapTest
    {
        static public string path = AppDomain.CurrentDomain.BaseDirectory;

        //#region Prueba completa

        //[TestMethod]
        //public void SoapManager()
        //{
        //    var input = new Parameters(new string[] { @"/Workspace:D:\Proyectos Net\NuboServo\Pollux\Test\TemplatesTest\WebService1Test\InsertarProducto" });
        //    Mock<IHttpHelper> httpMock = new Mock<IHttpHelper>();
        //    httpMock.Setup(x => x.HttpCall(It.IsAny<string[]>(), It.IsAny<Config>()))
        //        .Returns(new Response
        //        {
        //            Content = File.ReadAllText(Path.Combine(path, @"Files\Response_InsertarProducto_Ok.xml")),
        //            StatusCode = System.Net.HttpStatusCode.OK,
        //            IsSuccessStatusCode = true,
        //        });
        //    bool status=Program.Procesar(input, httpMock.Object,false);

        //    Assert.AreEqual<bool>(status, false);
        //}

        //#endregion

        #region Valida existencia de secciones en xml

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_ExisteSeccion_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            XDocument xml = XDocument.Parse(response);

            var body = new SoapManager(null).ExtractValue(xml, "Envelope.Body");
            var bodyCampo = new SoapManager(null).ExtractValue(xml, "Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo");

            var header = new SoapManager(null).ExtractValue(xml, "Envelope.Header");
            var headerCampo = new SoapManager(null).ExtractValue(xml, "Envelope.Header.Header.Transaccion.Canal");

            var fault = new SoapManager(null).ExtractValue(xml, "Envelope.Body.Fault");
            var faultCampo = new SoapManager(null).ExtractValue(xml, "Envelope.Body.Fault.faultcode");

            Assert.AreEqual<bool>(body.IsExist, true);
            Assert.AreEqual<bool>(bodyCampo.Value.Equals("OEX-00"), true);

            Assert.AreEqual<bool>(header.IsExist, true);
            Assert.AreEqual<bool>(headerCampo.Value.Equals("1"), true);

            Assert.AreEqual<bool>(fault.IsExist, false);
            Assert.AreEqual<bool>(faultCampo.Value == null, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLFault_ExisteSeccion_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_Fault.xml"));
            XDocument xml = XDocument.Parse(response);

            var body = new SoapManager(null).ExtractValue(xml, "Envelope.Body");
            var bodyCampo = new SoapManager(null).ExtractValue(xml, "Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo");

            var header = new SoapManager(null).ExtractValue(xml, "Envelope.Header");
            var headerCampo = new SoapManager(null).ExtractValue(xml, "Envelope.Header.Header.Transaccion.Canal");

            var fault = new SoapManager(null).ExtractValue(xml, "Envelope.Body.Fault");
            var faultCampo = new SoapManager(null).ExtractValue(xml, "Envelope.Body.Fault.faultcode");

            Assert.AreEqual<bool>(body.IsExist, true);
            Assert.AreEqual<bool>(bodyCampo.Value==null, true);

            Assert.AreEqual<bool>(header.IsExist, false);
            Assert.AreEqual<bool>(headerCampo.Value==null, true);

            Assert.AreEqual<bool>(fault.IsExist, true);
            Assert.AreEqual<bool>(faultCampo.Value.Equals("soap:Server"), true);
        }

        #endregion

        #region XML con mujltiples esquemas

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_Body_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation = ValidationOperation.Equals,
                        Tag="Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo",
                        Values=new List<string> { "OEX-00", "FUN-01", "FUN-02", "TEC-01", "TEC-03" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_Body_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.NotEquals,
                        Tag="Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo",
                        Values=new List<string> { "OEX-00" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_Header_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsHeader = new List<Validation>()
                {
                    new Validation("Envelope.Header") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Header.Header.Transaccion.Canal",
                        Values=new List<string> { "1" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_Header_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsHeader = new List<Validation>()
                {
                    new Validation("Envelope.Header") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Header.Header.Transaccion.Canal",
                        Values=new List<string> { "2" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_Body_And_Header_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsHeader = new List<Validation>()
                {
                    new Validation("Envelope.Header") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Header.Header.Transaccion.Canal",
                        Values=new List<string> { "1" },
                    }
                },
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo",
                        Values=new List<string> { "OEX-00", "FUN-01", "FUN-02", "TEC-01", "TEC-03" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_Body_And_Header_False_HeaderErroneo()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsHeader = new List<Validation>()
                {
                    new Validation("Envelope.Header") {
                        Operation =ValidationOperation.NotEquals,
                        Tag="Envelope.Header.Header.Transaccion.Canal",
                        Values=new List<string> { "1" },
                    }
                },
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo",
                        Values=new List<string> { "OEX-00", "FUN-01", "FUN-02", "TEC-01", "TEC-03" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas_Body_And_Header_False_Ambos()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_MultipleEsquemas.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsHeader = new List<Validation>()
                {
                    new Validation("Envelope.Header") {
                        Operation =ValidationOperation.NotEquals,
                        Tag="Envelope.Header.Header.Transaccion.Canal",
                        Values=new List<string> { "1" },
                    }
                },
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.NotEquals,
                        Tag="Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo",
                        Values=new List<string> { "OEX-00" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }
        #endregion

        #region Validacion de Fault

        [TestMethod]
        public void ValidateResponse_XMLFault_SinDefiniciones_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_Fault.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLFault_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_Fault.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsFault = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Body.Fault.faultcode",
                        Values=new List<string> { "soap:Server" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLFault_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_Fault.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsFault = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Body.Fault.faultcode",
                        Values=new List<string> { "cualquier cosa" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLFault_NombreCorto_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_Fault.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsFault = new List<Validation>()
                {
                    new Validation("Envelope.Body.Fault") {
                        Operation =ValidationOperation.Equals,
                        Tag="faultcode",
                        Values=new List<string> { "soap:Server" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        #endregion

        #region XML Simple

        [TestMethod]
        public void ValidateResponse_XMLSimple_Body_And_Header_NombreCorto_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsHeader = new List<Validation>()
                {
                    new Validation("Envelope.Header") {
                        Operation =ValidationOperation.Equals,
                        Tag="Header.Transaccion.Canal",
                        Values=new List<string> { "1" },
                    }
                },
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body")  {
                        Operation =ValidationOperation.NotEquals,
                        Tag="ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "3200", "2800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Body_And_Header_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsHeader = new List<Validation>()
                {
                    new Validation("Envelope.Header") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Header.Header.Transaccion.Canal",
                        Values=new List<string> { "1" },
                    }
                },
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body")  {
                        Operation =ValidationOperation.NotEquals,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "3200", "2800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }


        [TestMethod]
        public void ValidateResponse_XMLSimple_NotEqual_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.NotEquals,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "3200", "2800" },
                    }
                }   
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_NotEqual_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.NotEquals,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "1200", "2800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Equal_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "1200", "2800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Equal_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Equals,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "200", "2800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Major_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Major,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "200", "800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Major_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Major,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "200", "10800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }


        [TestMethod]
        public void ValidateResponse_XMLSimple_Minor_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Minor,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "10200", "8800" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Minor_False()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Minor,
                        Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { "4", "3" },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_ValoNulo1_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.Equals,
                        Tag="ObtenerProductoResponse.ObtenerProductoResult.SKU",
                        Values=new List<string> { Excel.KeyNull },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_ValoNulo2_True()
        {
            string response = File.ReadAllText(Path.Combine(path,@"Files\Response_simple.xml"));
            ValidationCollections validaciones = new ValidationCollections
            {
                ValidationsBody = new List<Validation>()
                {
                    new Validation("Envelope.Body") {
                        Operation =ValidationOperation.NotEquals,
                        Tag="ObtenerProductoResponse.ObtenerProductoResult.Precio",
                        Values=new List<string> { Excel.KeyNull },
                    }
                }
            };

            var estado = new SoapManager(null).ValidateSection(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        #endregion

        #region Multiples secciones de validacion en archivo de configuracion

        [TestMethod]
        public void ValidateConfigutation_TresSecciones()
        {
            var config = new Config(Path.Combine(path,@"Files\TresSecciones.config"), "http://localhost:33319/WebServiceTest1.asmx");

            Assert.AreEqual<bool>(config.Validations.ValidationsBody.Count == 1, true);
            Assert.AreEqual<bool>(config.Validations.ValidationsHeader.Count == 1, true);
            Assert.AreEqual<bool>(config.Validations.ValidationsFault.Count == 1, true);
        }

        #endregion
    }
}
