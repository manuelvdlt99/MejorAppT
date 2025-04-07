using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.ML;

namespace ML_Training
{
    class Program
    {
        /// <summary>
        /// Define el punto de entrada de la aplicación.
        /// </summary>
        static void Main()
        {
            var mlContext = new MLContext();

            string json = CleanJSON();
            var root = JsonDocument.Parse(json).RootElement;

            var dataList = new List<AnsiedadData>();
            foreach (var item in root.EnumerateObject()) {
                var entry = item.Value;
                dataList.Add(new AnsiedadData {
                    Edad = entry.GetProperty("Edad").GetInt32(),
                    Factor01 = entry.GetProperty("Factor01").GetInt32(),
                    Factor02 = entry.GetProperty("Factor02").GetInt32(),
                    Factor03 = entry.GetProperty("Factor03").GetInt32(),
                    Genero = entry.GetProperty("Genero").GetString(),
                    NivelAnsiedad = entry.GetProperty("Factor01").GetInt32()
                                   + entry.GetProperty("Factor02").GetInt32()
                                   + entry.GetProperty("Factor03").GetInt32()
                });
            }

            var data = mlContext.Data.LoadFromEnumerable(dataList);

            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("Genero")
                .Append(mlContext.Transforms.Concatenate("Features", "Edad", "Factor01", "Factor02", "Factor03", "Genero"))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(data);
            string path = @"..\..\..\Models\anxietyModel.zip";
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            mlContext.Model.Save(model, data.Schema, path);

            Console.WriteLine("Modelo entrenado y guardado en anxietyModel.zip");
            TestModel(mlContext, path);
        }

        /// <summary>
        /// Revisa el JSON con los datos, descarta todas las entradas que tengan datos faltantes y devuelve un string con los datos correctos.
        /// </summary>
        /// <returns>El JSON con todos los datos válidos.</returns>
        private static string CleanJSON()
        {
            string json = File.ReadAllText("../../../Data/data_quickTests.json");

            var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var cleanJsonObject = new JsonObject();

            foreach (var item in root.EnumerateObject()) {
                var data = item.Value;
                bool hasAllProperties = data.TryGetProperty("Edad", out var edadProp) &&
                                        data.TryGetProperty("Factor01", out var factor01Prop) &&
                                        data.TryGetProperty("Factor02", out var factor02Prop) &&
                                        data.TryGetProperty("Factor03", out var factor03Prop) &&
                                        data.TryGetProperty("Fecha", out var fechaProp) &&
                                        data.TryGetProperty("Genero", out var generoProp);

                if (hasAllProperties) {
                    JsonNode nodeData = JsonNode.Parse(data.GetRawText());
                    cleanJsonObject.Add(item.Name, nodeData);
                }
            }

            return cleanJsonObject.ToString();
        }

        /// <summary>
        /// Prueba muy sencilla del modelo.
        /// </summary>
        /// <param name="mlContext">El modelo de IA.</param>
        /// <param name="modelPath">La ruta donde está el zip del modelo.</param>
        private static void TestModel(MLContext mlContext, string modelPath)
        {
            ITransformer trainedModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<AnsiedadData, AnsiedadPrediction>(trainedModel);

            var testInput = new AnsiedadData {
                Edad = 30,
                Factor01 = 5,
                Factor02 = 0,
                Factor03 = 15,
                Genero = "Mujer"
            };

            var prediction = predEngine.Predict(testInput);
            Console.WriteLine($"Predicción de ansiedad para la entrada de prueba: {prediction.PredictedAnsiedad}");
        }
    }
}