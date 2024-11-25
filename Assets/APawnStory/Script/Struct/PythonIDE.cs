using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Betadron.Struct
{
    public class PythonIDE : MonoBehaviour
    {
        private static string pythonPath;
        private static ProcessStartInfo pythonIDE;

        private static int minXValue=-22;
        private static int maxXValue=22;
        private static int minYValue = -22;
        private static int maxYValue = 22;
        private static int cicles=100;
        private static int population=10;

        public static int MinXValue { get => minXValue; set => minXValue = value; }
        public static int MaxXValue { get => maxXValue; set => maxXValue = value; }
        public static int MinYValue { get => minYValue; set => minYValue = value; }
        public static int MaxYValue { get => maxYValue; set => maxYValue = value; }
        public static int Cicles { get => cicles; set => cicles = value; }
        public static int Population { get => population; set => population = value; }
        public static void LoadPath()
        {
            pythonPath= Path.GetFullPath("../") + @"APawnAdventure\Assets\APawnStory\Script\Python\Flower_Pollinization_CSharp.py";
        }

        public static void CreatePythonIDE()
        {
            pythonIDE = new ProcessStartInfo();
        }

        public static void LoadPythonIDE(float timeOffset=0)
        {
            if (pythonIDE == null)
                CreatePythonIDE();

            pythonIDE.FileName = "python"; // O "python3" dependiendo de tu sistema
            // Pasar argumentos al script
            pythonIDE.Arguments = $"{pythonPath} {minXValue} {maxXValue} {minYValue} " +
                $"{maxYValue} {cicles} {population} {MapFunctions.GameTime+ timeOffset}"; 
            pythonIDE.UseShellExecute = false;
            pythonIDE.RedirectStandardOutput = true;
        }

        public static PythonRef ExecuteIDE()
        {
            PythonRef value =new();
            // Ejecutar el proceso
            using (Process process = Process.Start(pythonIDE))
            {
                using StreamReader reader = process.StandardOutput;
                string result = reader.ReadToEnd();
                print($"Resultado desde Python: {result}");
                value = JsonUtility.FromJson<PythonRef>(result);
            }
            return value;
        }

    }
    [System.Serializable]
    public class PythonRef
    {
        public float value;
        public List<float> coord;

        public float Value { get; set; }
        public List<float> Coord { get; set; }
    }
}
