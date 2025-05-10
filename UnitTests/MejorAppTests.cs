using MejorAppTG1;
using MejorAppTG1.Models;
using MejorAppTG1.Utils;

namespace UnitTests
{
    /// <summary>
    /// Clase que alberga todos los métodos relativos a la realización de pruebas unitarias para la aplicación principal.
    /// </summary>
    public class MejorAppTests
    {
        #region "Métodos de realización de pruebas"        
        /// <summary>
        /// Prueba el método de cálculo de factores para el test de ansiedad rápido recorriendo una serie de casos de prueba con todas las posibilidades.
        /// </summary>
        /// <param name="testCase">El caso de prueba actual.</param>
        [Theory]
        [MemberData(nameof(FactoresTestDataRapido))]
        public void TestFactoresRapido(FactorTestCase testCase)
        {
            var result = ScoreCalculator.CalculoFactores(testCase.Answers, testCase.Factor, testCase.TestData);
            Assert.Equal(testCase.Expected, result);
        }

        /// <summary>
        /// Prueba el método de cálculo de factores para el test de ansiedad completo recorriendo una serie de casos de prueba con todas las posibilidades.
        /// </summary>
        /// <param name="testCase">El caso de prueba actual.</param>
        [Theory]
        [MemberData(nameof(FactoresTestDataCompleto))]
        public void TestDataCompleto(FactorTestCase testCase)
        {
            var result = ScoreCalculator.CalculoFactores(testCase.Answers, testCase.Factor, testCase.TestData);
            Assert.Equal(testCase.Expected, result);
        }

        /// <summary>
        /// Prueba el método de cálculo de factores para el test de TCA recorriendo una serie de casos de prueba con todas las posibilidades.
        /// </summary>
        /// <param name="testCase">El caso de prueba actual.</param>
        [Theory]
        [MemberData(nameof(FactoresTestDataTCA))]
        public void TestFactoresTCA(FactorTestCase testCase)
        {
            var result = ScoreCalculator.CalculoFactores(testCase.Answers, testCase.Factor, testCase.TestData);
            Assert.Equal(testCase.Expected, result);
        }
        #endregion

        #region Métodos de casos de prueba        
        /// <summary>
        /// Recupera los casos de prueba de tests de ansiedad rápidos uno a uno.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> FactoresTestDataRapido()
        {
            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 14 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 14, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 14 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 14, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 23 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 23, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 2 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 3 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 3, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 21 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 21, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 24 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 24, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 4 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 4, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 22 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.QUICK_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 22, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };
        }

        /// <summary>
        /// Recupera los casos de prueba de tests de ansiedad completos uno a uno.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> FactoresTestDataCompleto()
        {
            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 16 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 16, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 25 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 25, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 26 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 26, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 26 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 26, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 34 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 34, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (12-14) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 35 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 35, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 22 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 22, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 27 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 27, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 28 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 28, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 2 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 2 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 2, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 28 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 28, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 36 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 36, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (15-16) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 37 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 37, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 23 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 23, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 32 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 32, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 33 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 33, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 2 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 2 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 2, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 30 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 30, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 35 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 35, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Mujer (17-18) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 36 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_FEMALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 36, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
{
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 11 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 11, Nivel = App.FACTORS_LEVEL_LOW }
                }
};

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 17 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 17, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 18 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 18, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 19 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 19, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 28 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 28, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (12-14) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 29 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 29, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 12 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 12, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 18 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 18, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 19 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 19, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 2 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 2 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 2, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 19 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 19, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 26 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 26, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (15-16) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 27 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 27, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 12 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 12, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 18 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 18, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 19 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 19, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 2 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 2 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 2, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 3 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 3, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 21 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 21, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 26 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 26, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Hombre (17-18) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 27 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 27, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
{
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 13 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 13, Nivel = App.FACTORS_LEVEL_LOW }
                }
};

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 29 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 29, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 30 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 30, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 22 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 22, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 27 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 27, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (12-14) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 28 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 14, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 28, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 20 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 20, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 32 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 32, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 33 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 33, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 2 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 2 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 2, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 3 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 3, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 23 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 23, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 31 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 31, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (15-16) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 32 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 16, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 32, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 1 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 21 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 21, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 1 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 30 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 30, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 1 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 31 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 31, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 2 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 0 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 2 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 1 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 2 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_2, ValorRespuesta = 2 },
                    ],
                    Factor = App.FACTORS_2,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_2, Puntuacion = 2, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 3 - Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 25 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 25, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 3 - Puntuación media",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 30 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 30, Nivel = App.FACTORS_LEVEL_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "No binario (17-18) - Factor 3 - Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_3, ValorRespuesta = 31 },
                    ],
                    Factor = App.FACTORS_3,
                    TestData = new Test { EdadUser = 17, GeneroUser = App.GENDERS_NB_KEY, Tipo = App.FULL_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_3, Puntuacion = 31, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };
        }

        /// <summary>
        /// Recupera los casos de prueba de tests de TCA uno a uno.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> FactoresTestDataTCA()
        {
            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Puntuación baja",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 11 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.TCA_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 10, Nivel = App.FACTORS_LEVEL_LOW }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Puntuación leve-moderada",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 15 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.TCA_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 14, Nivel = App.FACTORS_LEVEL_LOW_MEDIUM }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Puntuación moderada-alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 20 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.TCA_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 19, Nivel = App.FACTORS_LEVEL_MEDIUM_HIGH }
                }
            };

            yield return new object[]
            {
                new FactorTestCase
                {
                    Name = "Puntuación alta",
                    Answers =
                    [
                        new() { Factor = App.FACTORS_1, ValorRespuesta = 21 },
                    ],
                    Factor = App.FACTORS_1,
                    TestData = new Test { EdadUser = 25, GeneroUser = App.GENDERS_MALE_KEY, Tipo = App.TCA_TEST_KEY },
                    Expected = new Factor { NumeroFactor = App.FACTORS_1, Puntuacion = 20, Nivel = App.FACTORS_LEVEL_HIGH }
                }
            };
        }
        #endregion
    }
}