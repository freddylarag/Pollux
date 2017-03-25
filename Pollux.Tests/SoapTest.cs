// <copyright file="SoapTestSoap.cs"></copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pollux;
using System.Collections.Generic;
using System.IO;

namespace Pollux.Tests
{
    /// <summary>This class contains parameterized unit tests for Soap</summary>
    [PexClass(typeof(Soap))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class SoapTest
    {
        #region XML con mujltiples esquemas

        [TestMethod]
        public void ValidateResponse_XMLConMultiplesEsquemas()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_MultipleEsquemas.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.Equals,
                    Tag="Envelope.Body.ConsultarEstadosFolioCajaResponse.ResultadoOperacion.Codigo",
                    Values=new List<string> { "OEX-00", "FUN-01", "FUN-02", "TEC-01", "TEC-03" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        #endregion

        #region XML Simple

        [TestMethod]
        public void ValidateResponse_XMLSimple_NotEqual_True()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.NotEquals,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "3200", "2800" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_NotEqual_False()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.NotEquals,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "1200", "2800" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Equal_True()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.Equals,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "1200", "2800" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Equal_False()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.Equals,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "200", "2800" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Major_True()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.Major,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "200", "800" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Major_False()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.Major,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "200", "10800" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }


        [TestMethod]
        public void ValidateResponse_XMLSimple_Minor_True()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.Minor,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "10200", "8800" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, true);
        }

        [TestMethod]
        public void ValidateResponse_XMLSimple_Minor_False()
        {
            string response = File.ReadAllText(@"D:\Proyectos Net\NuboServo\Pollux\Pollux.Tests\Response_simple.xml");
            List<Validation> validaciones = new List<Validation>()
            {
                new Validation {
                    Operation =ValidationOperation.Minor,
                    Tag="Envelope.Body.ObtenerProductoResponse.ObtenerProductoResult.Precio",
                    Values=new List<string> { "4", "3" },
                }
            };

            var estado = Soap.ValidateResponse(response, validaciones);
            Assert.AreEqual<bool>(estado, false);
        }

        #endregion
    }
}
