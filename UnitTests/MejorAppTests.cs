using MejorAppTG1;
using MejorAppTG1.Models;
using MejorAppTG1.Utils;
using Microsoft.VisualBasic;
using System.Text.Json;

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
        public void TestFactoresCompleto(FactorTestCase testCase)
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

        /// <summary>
        /// Prueba el método que recupera los consejos para el test de ansiedad rápido recorriendo una serie de casos de prueba con todas las posibilidades.
        /// </summary>
        /// <param name="testCase">El caso de prueba actual.</param>
        [Theory]
        [MemberData(nameof(ConsejosTestDataRapido))]
        public void TestConsejosRapido(AdviceTestCase testCase)
        {
            var json = File.ReadAllText(App.JSON_ADVICES_FULL);
            var consejos = JsonSerializer.Deserialize<List<Advice>>(json);

            var resultado = AdviceManager.AnsiedadRapidoConsejos(
                consejos,
                testCase.PuntuacionTotal,
                testCase.Factor1,
                testCase.Factor2,
                testCase.Factor3            
            );

            Assert.True(testCase.Expected.Count == resultado.Count, $"Número de categorías esperado: {testCase.Expected.Count}, pero se obtuvo: {resultado.Count}");

            for (int i = 0; i < testCase.Expected.Count; i++) {
                var expectedCategory = testCase.Expected[i];
                var actualCategory = resultado[i];

                Assert.True(expectedCategory.ImagePath == actualCategory.ImagePath, $"Categoría {actualCategory.Name}: se esperaba ImagePath '{expectedCategory.ImagePath}', pero se obtuvo '{actualCategory.ImagePath}'");
                Assert.True(expectedCategory.Advices.Count == actualCategory.Advices.Count, $"Categoría {actualCategory.Name}: se esperaban {expectedCategory.Advices.Count} consejos, pero se obtuvo {actualCategory.Advices.Count}");

                var expectedIds = expectedCategory.Advices.Select(a => a.Id).OrderBy(id => id);
                var actualIds = actualCategory.Advices.Select(a => a.Id).OrderBy(id => id);

                Assert.Equal(expectedIds, actualIds);
            }
        }

        /// <summary>
        /// Prueba el método que recupera los consejos para el test de TCA recorriendo una serie de casos de prueba con todas las posibilidades.
        /// </summary>
        /// <param name="testCase">El caso de prueba actual.</param>
        [Theory]
        [MemberData(nameof(ConsejosTestDataTCA))]
        public void TestConsejosTCA(AdviceTestCase testCase)
        {
            var resultado = AdviceManager.TCAConsejos(
                testCase.Factor1
            );

            Assert.True(testCase.Expected.Count == resultado.Count, $"Número de categorías esperado: {testCase.Expected.Count}, pero se obtuvo: {resultado.Count}");

            for (int i = 0; i < testCase.Expected.Count; i++) {
                var expectedCategory = testCase.Expected[i];
                var actualCategory = resultado[i];

                Assert.True(expectedCategory.ImagePath == actualCategory.ImagePath, $"Categoría {actualCategory.Name}: se esperaba ImagePath '{expectedCategory.ImagePath}', pero se obtuvo '{actualCategory.ImagePath}'");
                Assert.True(expectedCategory.Advices.Count == actualCategory.Advices.Count, $"Categoría {actualCategory.Name}: se esperaban {expectedCategory.Advices.Count} consejos, pero se obtuvo {actualCategory.Advices.Count}");

                var expectedIds = expectedCategory.Advices.Select(a => a.Id).OrderBy(id => id);
                var actualIds = actualCategory.Advices.Select(a => a.Id).OrderBy(id => id);

                Assert.Equal(expectedIds, actualIds);
            }
        }

        /// <summary>
        /// Prueba el método que recupera los consejos para el test de ansiedad completo recorriendo una serie de casos de prueba con todas las posibilidades.
        /// </summary>
        /// <param name="testCase">El caso de prueba actual.</param>
        [Theory]
        [MemberData(nameof(ConsejosTestDataCompleto))]
        public void TestConsejosCompleto(AdviceTestCase testCase)
        {
            var json = File.ReadAllText(App.JSON_ADVICES_FULL);
            var consejos = JsonSerializer.Deserialize<List<Advice>>(json);

            var resultado = AdviceManager.AnsiedadCompletoConsejos(
                consejos,
                testCase.PuntuacionTotal,
                testCase.Factor1,
                testCase.Factor2,
                testCase.Factor3
            );

            Assert.True(testCase.Expected.Count == resultado.Count, $"Número de categorías esperado: {testCase.Expected.Count}, pero se obtuvo: {resultado.Count}");

            for (int i = 0; i < testCase.Expected.Count; i++) {
                var expectedCategory = testCase.Expected[i];
                var actualCategory = resultado[i];

                Assert.True(expectedCategory.ImagePath == actualCategory.ImagePath, $"Categoría {actualCategory.Name}: se esperaba ImagePath '{expectedCategory.ImagePath}', pero se obtuvo '{actualCategory.ImagePath}'");
                Assert.True(expectedCategory.Advices.Count == actualCategory.Advices.Count, $"Categoría {actualCategory.Name}: se esperaban {expectedCategory.Advices.Count} consejos, pero se obtuvo {actualCategory.Advices.Count}");

                var expectedIds = expectedCategory.Advices.Select(a => a.Id).OrderBy(id => id);
                var actualIds = actualCategory.Advices.Select(a => a.Id).OrderBy(id => id);

                Assert.Equal(expectedIds, actualIds);
            }
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

        /// <summary>
        /// Recupera los casos de prueba de consejos de tests de ansiedad rápidos uno a uno.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> ConsejosTestDataRapido()
        {
            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 1: Consejos para todo bien",
                    PuntuacionTotal = 29,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 14, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 14, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "thumbsup_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 2: Consejos para controlar respuesta factor cognitivo",
                    PuntuacionTotal = 45,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 14, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 30, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 1 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 4 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "seemore_icon.png",
                            Advices =
                            [
                                new() { Id = 18 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 3: Controlar respuestas factor fisiológico",
                    PuntuacionTotal = 45,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 30, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 14, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 7 },
                                new() { Id = 8 },
                                new() { Id = 9 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 4: Controlar respuestas factor de evitación",
                    PuntuacionTotal = 33,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 14, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 5, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 14, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 19 },
                                new() { Id = 20 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 5: Controlar respuestas factor cognitivo, fisiológico y de evitación",
                    PuntuacionTotal = 65,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 30, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 5, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 30, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 6: Controlar respuestas factor cognitivo y fisiológico",
                    PuntuacionTotal = 61,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 30, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 1, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 30, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 8 },
                                new() { Id = 9 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 7: Controlar respuestas factor cognitivo y de evitación",
                    PuntuacionTotal = 49,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 14, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 5, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 30, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 },
                                new() { Id = 10 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "CASO 8: Controlar respuestas factor fisiológico y de evitación",
                    PuntuacionTotal = 49,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 30, Nivel = string.Empty },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 5, Nivel = string.Empty },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 14, Nivel = string.Empty },
                    TipoTest = App.QUICK_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };
        }

        public static IEnumerable<object[]> ConsejosTestDataTCA()
        {
            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "Bajo riesgo",
                    PuntuacionTotal = 10,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 10, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.TCA_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 1 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "Riesgo leve-moderado",
                    PuntuacionTotal = 14,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 14, Nivel = App.FACTORS_LEVEL_LOW_MEDIUM },
                    TipoTest = App.TCA_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 1 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "Riesgo moderado-alto",
                    PuntuacionTotal = 19,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 19, Nivel = App.FACTORS_LEVEL_MEDIUM_HIGH },
                    TipoTest = App.TCA_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 1 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "Riesgo alto",
                    PuntuacionTotal = 20,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 20, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.TCA_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 1 }
                            ]
                        }
                    ]
                }
            };
        }

        public static IEnumerable<object[]> ConsejosTestDataCompleto()
        {
            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Bajo - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "thumbsup_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Bajo - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 1 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 4 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "seemore_icon.png",
                            Advices =
                            [
                                new() { Id = 18 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Bajo - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 1 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 4 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "seemore_icon.png",
                            Advices =
                            [
                                new() { Id = 18 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Bajo - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 7 },
                                new() { Id = 8 },
                                new() { Id = 9 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Bajo - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 7 },
                                new() { Id = 8 },
                                new() { Id = 9 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Medio - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 19 },
                                new() { Id = 20 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Alto - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 19 },
                                new() { Id = 20 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Medio - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Medio - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Alto - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Alto - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Medio - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Medio - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Alto - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Alto - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 12 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Bajo - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 8 },
                                new() { Id = 9 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Bajo - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 8 },
                                new() { Id = 9 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Bajo - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 8 },
                                new() { Id = 9 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Bajo - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 2 },
                                new() { Id = 3 },
                                new() { Id = 8 },
                                new() { Id = 9 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 10 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Medio - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 },
                                new() { Id = 10 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Medio - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 },
                                new() { Id = 10 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Alto - F03: Medio",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 },
                                new() { Id = 10 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Bajo - F02: Alto - F03: Alto",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 3 },
                                new() { Id = 11 },
                                new() { Id = 10 },
                                new() { Id = 5 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Medio - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Medio - F02: Alto - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Medio - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_MEDIUM },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };

            yield return new object[]
            {
                new AdviceTestCase
                {
                    Name = "F01: Alto - F02: Alto - F03: Bajo",
                    PuntuacionTotal = 0,
                    Factor1 = new() { NumeroFactor = App.FACTORS_1, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor2 = new() { NumeroFactor = App.FACTORS_2, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_HIGH },
                    Factor3 = new() { NumeroFactor = App.FACTORS_3, Puntuacion = 0, Nivel = App.FACTORS_LEVEL_LOW },
                    TipoTest = App.FULL_TEST_KEY,
                    Expected =
                    [
                        new() {
                            ImagePath = "advice_icon.png",
                            Advices =
                            [
                                new() { Id = 6 },
                                new() { Id = 8 },
                                new() { Id = 11 },
                                new() { Id = 10 }
                            ]
                        },
                        new() {
                            ImagePath = "prevent_icon.png",
                            Advices =
                            [
                                new() { Id = 13 },
                                new() { Id = 14 },
                                new() { Id = 15 },
                                new() { Id = 21 },
                                new() { Id = 22 },
                                new() { Id = 23 }
                            ]
                        },
                        new() {
                            ImagePath = "burdened_icon.png",
                            Advices =
                            [
                                new() { Id = 16 },
                                new() { Id = 17 }
                            ]
                        }
                    ]
                }
            };
        }
        #endregion
    }
}