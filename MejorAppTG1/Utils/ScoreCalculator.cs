using MejorAppTG1.Models;
using System.Numerics;

namespace MejorAppTG1.Utils
{
    public class ScoreCalculator
    {
        public static Factor CalculoFactores(List<Answer> preguntas, string factor, Test tipoTest)
        {
            int totalFactor = SumarPuntos(preguntas, factor, tipoTest);
            string nivel = CalcularNivel(tipoTest, factor, totalFactor);

            return new Factor(factor, totalFactor, nivel);
        }

        private static int SumarPuntos(List<Answer> preguntas, string factor, Test tipoTest)
        {
            int totalFactor = 0;
            if (tipoTest.Tipo == "str_EatingTest") {
                for (int i = 0; i < preguntas.Count; i++) {
                    if (preguntas[i].ValorRespuesta > 0) preguntas[i].ValorRespuesta -= 1;

                    if (i == 24) {
                        switch (preguntas[i].ValorRespuesta) {
                            case 0:
                                preguntas[i].ValorRespuesta = 3;
                                break;
                            case 1:
                                preguntas[i].ValorRespuesta = 2;
                                break;
                            case 2:
                                preguntas[i].ValorRespuesta = 1;
                                break;
                            case 3:
                            case 4:
                                preguntas[i].ValorRespuesta = 0;
                                break;
                        }
                    }
                    totalFactor += preguntas[i].ValorRespuesta;
                }
            }
            else {
                for (int i = 0; i < preguntas.Count; i++) {
                    if (factor == preguntas[i].Factor) {
                        totalFactor += preguntas[i].ValorRespuesta;
                    }
                }
            }

            return totalFactor;
        }

        private static string CalcularNivel(Test tipoTest, string factor, int totalFactor)
        {
            string nivel = "";
            switch (tipoTest.Tipo) {
                case "str_QuickTest":
                    nivel = NivelesRapido(totalFactor, factor);
                    break;
                case "str_FullTest":
                    nivel = NivelesCompleto(totalFactor, factor, tipoTest);
                    break;
                case "str_EatingTest":
                    nivel = NivelesTCA(totalFactor);
                    break;
            }
            return nivel;
        }

        private static string NivelesRapido(int totalFactor, string factor)
        {
            string nivel = "Bajo";
            switch (factor) {
                case "1":
                    if (totalFactor >= 15 && totalFactor <= 21) nivel = "Medio";
                    else if (totalFactor > 21) nivel = "Alto";
                    break;
                case "2":
                    if (totalFactor >= 15 && totalFactor <= 23) nivel = "Medio";
                    else if (totalFactor > 23) nivel = "Alto";
                    break;
                case "3":
                    if (totalFactor == 2 || totalFactor == 3) nivel = "Medio";
                    else if (totalFactor > 3) nivel = "Alto";
                    break;
            }
            return nivel;
        }

        private static string NivelesCompleto(int totalFactor, string factor, Test tipoTest)
        {
            string nivel = "Bajo";
            string genero = tipoTest.GeneroUser;
            int edad = tipoTest.EdadUser;

            //##############################
            //# CONDICIÓN PARA LAS MUJERES #
            //##############################

            if (genero == "str_Genders_Woman") {
                if (edad <= 14) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 16 && totalFactor <= 25) nivel = "Medio";
                            else if (totalFactor > 25) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor > 0) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 26 && totalFactor <= 34.1) nivel = "Medio";
                            else if (totalFactor > 34.1) nivel = "Alto";
                            break;
                    }
                }
                else if (edad == 15 || edad == 16) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 22 && totalFactor <= 27) nivel = "Medio";
                            else if (totalFactor > 17) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor == 1) nivel = "Medio";
                            else if (totalFactor > 1) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 28 && totalFactor <= 36) nivel = "Medio";
                            else if (totalFactor > 36) nivel = "Alto";
                            break;
                    }
                }
                else if (edad >= 17) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 23.5 && totalFactor <= 32) nivel = "Medio";
                            else if (totalFactor > 32) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor == 1) nivel = "Medio";
                            else if (totalFactor > 1) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 30 && totalFactor <= 35) nivel = "Medio";
                            else if (totalFactor > 35) nivel = "Alto";
                            break;
                    }
                }
            }

            //##############################
            //# CONDICIÓN PARA LOS HOMBRES #
            //##############################

            else if (genero == "str_Genders_Man") {
                if (edad <= 14) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 11 && totalFactor <= 17) nivel = "Medio";
                            else if (totalFactor > 17) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor > 0) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 19 && totalFactor <= 28) nivel = "Medio";
                            else if (totalFactor > 28) nivel = "Alto";
                            break;
                    }
                }
                else if (edad == 15 || edad == 16) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 12 && totalFactor <= 18) nivel = "Medio";
                            else if (totalFactor > 18) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor == 1) nivel = "Medio";
                            else if (totalFactor > 1) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 19 && totalFactor <= 26) nivel = "Medio";
                            else if (totalFactor > 26) nivel = "Alto";
                            break;
                    }
                }
                else if (edad >= 17) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 12 && totalFactor <= 18.4) nivel = "Medio";
                            else if (totalFactor > 18.4) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor <= 2 && totalFactor != 0) nivel = "Medio";
                            else if (totalFactor > 2) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 21 && totalFactor <= 26) nivel = "Medio";
                            else if (totalFactor > 26) nivel = "Alto";
                            break;
                    }
                }
            }

            //##############################
            //# CONDICIÓN PARA NO BINARIOS #
            //##############################

            else if (genero == "str_Genders_NB") {
                if (edad <= 14) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 13.5 && totalFactor <= 29.5) nivel = "Medio";
                            else if (totalFactor > 29.5) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor > 0) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 22.5 && totalFactor <= 27) nivel = "Medio";
                            else if (totalFactor > 27) nivel = "Alto";
                            break;
                    }
                }
                else if (edad == 15 || edad == 16) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 20.4 && totalFactor <= 32.4) nivel = "Medio";
                            else if (totalFactor > 32.4) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor <= 2 && totalFactor != 0) nivel = "Medio";
                            else if (totalFactor > 2) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 23.5 && totalFactor <= 31) nivel = "Medio";
                            else if (totalFactor > 31) nivel = "Alto";
                            break;
                    }
                }
                else if (edad >= 17) {
                    switch (factor) {
                        case "1":
                            if (totalFactor > 21.3 && totalFactor <= 30.2) nivel = "Medio";
                            else if (totalFactor > 30.2) nivel = "Alto";
                            break;
                        case "2":
                            if (totalFactor == 1) nivel = "Medio";
                            else if (totalFactor > 1) nivel = "Alto";
                            break;
                        case "3":
                            if (totalFactor > 25.5 && totalFactor <= 30.5) nivel = "Medio";
                            else if (totalFactor > 30.5) nivel = "Alto";
                            break;
                    }
                }
            }
            return nivel;
        }
        private static string NivelesTCA(int totalFactores)
        {
            string nivel = "Bajo";
            if (totalFactores >= 20) nivel = "Alto";
            else if (totalFactores >= 15) nivel = "Moderado-Alto";
            else if (totalFactores >= 11) nivel = "Leve-Moderado";
            return nivel;
        }
    }
}
