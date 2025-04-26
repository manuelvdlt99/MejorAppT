using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ML_Training
{
    class Program
    {
        /// <summary>
        /// Define el punto de entrada de la aplicación. Permite al usuario elegir un tipo de entrenamiento y entrena el modelo seleccionado.
        /// </summary>
        static void Main()
        {
            Console.WriteLine("Elige un modo de entrenamiento de IA:" +
                "\n\t1. Ansiedad rápido - resultados generales" +
                "\n\t2. Ansiedad rápido - evolución del usuario" +
                "\n\t3. Ansiedad completo - resultados generales" +
                "\n\t4. Ansiedad completo - evolución del usuario" +
                "\n\t5. TCA - resultados generales" +
                "\n\t6. TCA - evolución del usuario");
            bool valido;
            do {
                if (Int32.TryParse(Console.ReadLine(), out int mode)) {
                    string json, cleanJson, solutionDir, destinationPath;
                    switch (mode) {
                        case 1:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_general_quickTests.json");
                            cleanJson = CleanJSON(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "AI_Models", "model_general_quickTests.zip");
                            Training.Train(cleanJson, destinationPath);
                            break;
                        case 2:
                            valido = true;
                            break;
                        case 3:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_general_fullTests.json");
                            cleanJson = CleanJSON(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "AI_Models", "model_general_fullTests.zip");
                            Training.Train(cleanJson, destinationPath);
                            break;
                        case 4:
                            valido = true;
                            break;
                        case 5:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_general_TCA.json");
                            cleanJson = CleanJSONTCA(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "AI_Models", "model_general_TCATests.zip");
                            Training.TrainTCA(cleanJson, destinationPath);
                            break;
                        case 6:
                            valido = true;
                            break;
                        default:
                            valido = false;
                            Console.Write("Modo no encontrado, prueba de nuevo: ");
                            break;
                    }
                } else {
                    valido = false;
                    Console.Write("Carácter no válido introducido, prueba de nuevo: ");
                }
            } while (!valido);
        }

        /// <summary>
        /// Revisa el JSON de ansiedad con los datos, descarta todas las entradas que tengan datos faltantes y devuelve un string con los datos correctos.
        /// </summary>
        /// <param name="json">El contenido del JSON.</param>
        /// <returns>El JSON con todos los datos válidos.</returns>
        private static string CleanJSON(string json)
        {
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
        /// Revisa el JSON de TCA con los datos, descarta todas las entradas que tengan datos faltantes y devuelve un string con los datos correctos.
        /// </summary>
        /// <param name="json">El contenido del JSON.</param>
        /// <returns>El JSON con todos los datos válidos.</returns>
        private static string CleanJSONTCA(string json)
        {
            var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var cleanJsonObject = new JsonObject();

            foreach (var item in root.EnumerateObject()) {
                var data = item.Value;
                bool hasAllProperties = data.TryGetProperty("Edad", out var edadProp) &&
                                        data.TryGetProperty("Puntuacion", out var factor01Prop) &&
                                        data.TryGetProperty("Fecha", out var fechaProp) &&
                                        data.TryGetProperty("Genero", out var generoProp);

                if (hasAllProperties) {
                    JsonNode nodeData = JsonNode.Parse(data.GetRawText());
                    cleanJsonObject.Add(item.Name, nodeData);
                }
            }

            return cleanJsonObject.ToString();
        }
    }
}