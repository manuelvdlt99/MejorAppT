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
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "Resources", "Raw", "model_general_quickTests.zip");
                            Training.Train(cleanJson, destinationPath);
                            break;
                        case 2:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_history_quickTests.json");
                            cleanJson = CleanJSONEvolutivo(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "Resources", "Raw", "model_history_quickTests.zip");
                            Training.TrainEvolutivo(cleanJson, destinationPath);
                            break;
                        case 3:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_general_fullTests.json");
                            cleanJson = CleanJSON(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "Resources", "Raw", "model_general_fullTests.zip");
                            Training.Train(cleanJson, destinationPath);
                            break;
                        case 4:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_history_fullTests.json");
                            cleanJson = CleanJSONEvolutivo(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "Resources", "Raw", "model_history_fullTests.zip");
                            Training.TrainEvolutivo(cleanJson, destinationPath);
                            break;
                        case 5:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_general_TCA.json");
                            cleanJson = CleanJSONTCA(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "Resources", "Raw", "model_general_TCATests.zip");
                            Training.TrainTCA(cleanJson, destinationPath);
                            break;
                        case 6:
                            valido = true;
                            json = File.ReadAllText("../../../Data/data_history_TCA.json");
                            cleanJson = CleanJSONEvolutivo(json);
                            solutionDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
                            destinationPath = Path.Combine(solutionDir, "MejorAppTG1", "Resources", "Raw", "model_history_TCATests.zip");
                            Training.TrainEvolutivo(cleanJson, destinationPath);
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
                bool hasAllProperties = data.TryGetProperty("Edad", out _) &&
                                        data.TryGetProperty("Factor01", out _) &&
                                        data.TryGetProperty("Factor02", out _) &&
                                        data.TryGetProperty("Factor03", out _) &&
                                        data.TryGetProperty("Genero", out _);

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
                bool hasAllProperties = data.TryGetProperty("Edad", out _) &&
                                        data.TryGetProperty("Puntuacion", out _) &&
                                        data.TryGetProperty("Genero", out _);

                if (hasAllProperties) {
                    JsonNode nodeData = JsonNode.Parse(data.GetRawText());
                    cleanJsonObject.Add(item.Name, nodeData);
                }
            }

            return cleanJsonObject.ToString();
        }

        /// <summary>
        /// Revisa cualquier JSON de evolución de tests, descarta todos los usuarios que tengan algún test sin edad, puntuación o género y devuelve un string con los datos correctos.
        /// </summary>
        /// <param name="json">El contenido del JSON.</param>
        /// <returns>El JSON con todos los datos válidos.</returns>
        private static string CleanJSONEvolutivo(string json)
        {
            var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var cleanJsonObject = new JsonObject();

            foreach (var user in root.EnumerateObject()) {
                var userTests = user.Value;
                bool allTestsValid = true;

                foreach (var test in userTests.EnumerateArray()) {
                    if (!test.TryGetProperty("Edad", out _) || !test.TryGetProperty("Genero", out _)) {
                        allTestsValid = false;
                        break;
                    }
                }

                if (allTestsValid) {
                    JsonNode nodeData = JsonNode.Parse(userTests.GetRawText());
                    cleanJsonObject.Add(user.Name, nodeData);
                }
            }

            return cleanJsonObject.ToString();
        }

    }
}