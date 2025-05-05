using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ML_Training
{
    /// <summary>
    /// Clase que alberga todos los métodos relativos al entrenamiento de modelos de inteligencia artificial.
    /// </summary>
    internal static class Training
    {
        /// <summary>
        /// Entrena un modelo de IA para resultados promedios genéricos a partir de los datos de un JSON dado y lo almacena en una ruta dada.
        /// </summary>
        /// <param name="json">El JSON del dataset.</param>
        /// <param name="modelPath">La ruta donde se ha de guardar el modelo entrenado.</param>
        public static void TrainAverage(string json, string modelPath)
        {
            var mlContext = new MLContext();

            var root = JsonDocument.Parse(json).RootElement;

            var dataList = new List<AIData>();
            if (modelPath.Contains("TCA")) {
                dataList.AddRange(
                    root.EnumerateObject()
                        .Select(item => item.Value)
                        .Select(entry => new AIData {
                            EdadRango = GetAgeRange(entry.GetProperty("Edad").GetInt32()),
                            Genero = entry.GetProperty("Genero").GetString(),
                            Puntuacion = entry.GetProperty("Puntuacion").GetInt32()
                        })
                );
            }
            else {
                dataList.AddRange(
                    root.EnumerateObject()
                        .Select(item => item.Value)
                        .Select(entry => new AIData {
                            EdadRango = GetAgeRange(entry.GetProperty("Edad").GetInt32()),
                            Genero = entry.GetProperty("Genero").GetString(),
                            Puntuacion = entry.GetProperty("Factor01").GetInt32()
                                       + entry.GetProperty("Factor02").GetInt32()
                                       + entry.GetProperty("Factor03").GetInt32()
                        })
                );
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
        }

        /// <summary>
        /// Entrena un modelo de IA para evoluciones de usuario a partir de los datos de un JSON dado y lo almacena en una ruta dada.
        /// </summary>
        /// <param name="json">El JSON del dataset.</param>
        /// <param name="modelPath">La ruta donde se ha de guardar el modelo entrenado.</param>
        public static void TrainEvolutivo(string json, string modelPath)
        {
            var mlContext = new MLContext();

            var root = JsonDocument.Parse(json).RootElement;

            var trainingList = new List<AIProgressiveData>();

            foreach (var user in root.EnumerateObject()) {
                var testList = user.Value.EnumerateArray()
                    .Select(entry => new {
                        Edad = entry.GetProperty("Edad").GetInt32(),
                        Genero = entry.GetProperty("Genero").GetString(),
                        Puntuacion = entry.GetProperty("Puntuacion").GetInt32()
                    })
                    .ToList();

                if (testList.Count < 4) continue;

                for (int i = 0; i <= testList.Count - 4; i++) {
                    var t1 = testList[i];
                    var t2 = testList[i + 1];
                    var t3 = testList[i + 2];
                    var target = testList[i + 3];

                    trainingList.Add(new AIProgressiveData {
                        Genero = target.Genero,
                        EdadRango = GetAgeRange(target.Edad),
                        P1 = t1.Puntuacion,
                        P2 = t2.Puntuacion,
                        P3 = t3.Puntuacion,
                        PuntuacionEvolucion = target.Puntuacion
                    });
                }
            }

            var data = mlContext.Data.LoadFromEnumerable(trainingList);

            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding("Genero")
                .Append(mlContext.Transforms.Concatenate("Features", "Genero", "EdadRango", "P1", "P2", "P3"))
                .Append(mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(data);
            string directory = Path.GetDirectoryName(modelPath);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            mlContext.Model.Save(model, data.Schema, modelPath);

            Console.WriteLine($"Modelo evolutivo entrenado y guardado en {modelPath}");
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
