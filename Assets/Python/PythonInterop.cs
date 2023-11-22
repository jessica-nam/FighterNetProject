// This sets up Python environment by initializing the Python Engine
// Brige between Unity and Python
// Contains method: RunPythonCode (executes Python scipts)
// testing 

using UnityEngine;
using Python.Runtime;
using System;

public static class PythonInterop
{
    private static bool isInitialized = false;

    public static void Initialize()
    {
        
        if (!isInitialized)
        {
            string pythonDll = @"C:\Python38\python38.dll";
            Environment.SetEnvironmentVariable("PYTHONHOME", @"C:\Python38", EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDll, EnvironmentVariableTarget.Process);
            PythonEngine.Initialize();
            Debug.Log("Python Engine Initialized");
            isInitialized = true;
        }
    }

    public static string InstallPackage(string packageName)
    {
        string installCode = $@"
import subprocess
process = subprocess.Popen(['C:\Python38\python.exe', '-m', 'pip', 'install', '{packageName}'], stdout=subprocess.PIPE, stderr=subprocess.PIPE)
version = subprocess.Popen(['python', '--version'], stdout=subprocess.PIPE, stderr=subprocess.PIPE)
out, err = process.communicate()
#print(out.decode('utf-8'))
#print(err.decode('utf-8'))
";
        return RunPythonCode(installCode);
    }


    public static string RunPythonCode(string pycode)
    {
        Initialize();
        using (Py.GIL())
        {
            try
            {
                using (var scope = Py.CreateScope())
                {
                    // PyObject version = Py.Import("sys").GetAttr("version");
                    // Debug.Log("Python Version: " + version.As<string>());

                    string setupCode = @"
import sys
from io import StringIO
sys.stdout = mystdout = StringIO()
#print('sys path')
#print(sys.executable)
";
                    setupCode = setupCode.TrimStart('\r', '\n');
                    scope.Exec(setupCode);

                    pycode = pycode.TrimStart('\r', '\n');
                    scope.Exec(pycode);

                    PyObject output = scope.Get("mystdout").InvokeMethod("getvalue");
                    return output.As<string>();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Python Execution Error: " + ex.Message);
                return null;
            }
        }
    }
}
