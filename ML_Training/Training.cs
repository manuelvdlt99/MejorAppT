using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ML_Training
{
    internal class Training
    {
        /// <summary>
        /// Entrena un modelo de IA para ansiedad a partir de los datos de un JSON dado y lo almacena en una ruta dada.
        /// </summary>
        /// <param name="json">El JSON del dataset.</param>
        /// <param name="modelPath">La ruta donde se ha de guardar el modelo entrenado.</param>
        public static void Train(string json, string modelPath)
        {
            var mlContext = new MLContext();

            var root = JsonDocument.Parse(json).RootElement;

            var dataList = new List<AIData>();
            foreach (var item in root.EnumerateObject()) {
                var entry = item.Value;
                int edad = entry.GetProperty("Edad").GetInt32();
                int rangoEdad = GetAgeRange(edad);

                dataList.Add(new AIData {
                    EdadRango = rangoEdad,
                    Genero = entry.GetProperty("Genero").GetString(),
                    Puntuacion = entry.GetProperty("Factor01").GetInt32()
                                   + entry.GetProperty("Factor02").GetInt32()
                                   + entry.GetProperty("Factor03").GetInt32()
                });
            }


            var data = mlContext.Data.LoadFromEnumerable(dataList);

            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("Genero")
                .Append(mlContext.Transforms.Concatenate("Features", "Genero", "EdadRango"))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(data);
            string directory = Path.GetDirectoryName(modelPath);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            mlContext.Model.Save(model, data.Schema, modelPath);

            Console.WriteLine($"Modelo entrenado y guardado en {modelPath}");
            TestModel(mlContext, modelPath);
        }

        /// <summary>
        /// Entrena un modelo de IA para TCA a partir de los datos de un JSON dado y lo almacena en una ruta dada.
        /// </summary>
        /// <param name="json">El JSON del dataset.</param>
        /// <param name="modelPath">La ruta donde se ha de guardar el modelo entrenado.</param>
        public static void TrainTCA(string json, string modelPath)
        {
            var mlContext = new MLContext();

            var root = JsonDocument.Parse(json).RootElement;

            var dataList = new List<AIData>();
            foreach (var item in root.EnumerateObject()) {
                var entry = item.Value;
                int edad = entry.GetProperty("Edad").GetInt32();
                int rangoEdad = GetAgeRange(edad);

                dataList.Add(new AIData {
                    EdadRango = rangoEdad,
                    Genero = entry.GetProperty("Genero").GetString(),
                    Puntuacion = entry.GetProperty("Puntuacion").GetInt32()
                });
            }


            var data = mlContext.Data.LoadFromEnumerable(dataList);

            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("Genero")
                .Append(mlContext.Transforms.Concatenate("Features", "Genero", "EdadRango"))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(data);
            string directory = Path.GetDirectoryName(modelPath);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            mlContext.Model.Save(model, data.Schema, modelPath);

            Console.WriteLine($"Modelo entrenado y guardado en {modelPath}");
            TestModel(mlContext, modelPath);
        }

        /// <summary>
        /// Método variable y no definitivo usado para realizar pruebas sencillas y temporales de modelos recién creados de IA.
        /// </summary>
        /// <param name="mlContext">El modelo de IA.</param>
        /// <param name="modelPath">La ruta donde está el .zip del modelo.</param>
        private static void TestModel(MLContext mlContext, string modelPath)
        {
            ITransformer trainedModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<AIData, AIPrediction>(trainedModel);

            var testInput = new AIData {
                EdadRango = 17,
                Genero = "Mujer"
            };

            var prediction = predEngine.Predict(testInput);
            Console.WriteLine($"Predicción para la entrada de prueba: {prediction.PredictedResult}");
        }

        /// <summary>
        /// Convierte una edad en una edad simbólica de un rango determinado (14, 15 o 17) para facilitar el entrenamiento de la IA.
        /// </summary>
        /// <param name="age">La edad real.</param>
        /// <returns>La edad representativa del rango al que pertenece la edad pasada (14, 15 o 17).</returns>
        private static int GetAgeRange(int age)
        {
            if (age <= 14)
                return 14;
            else if (age <= 16)
                return 15;
            else
                return 17;
        }
    }
}
