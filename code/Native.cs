using System;
using System.IO;
using System.Runtime.InteropServices;
using Godot;

public static class NativeLoader
{
    // Windows API functions
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetDllDirectory(string lpPathName);

    // Linux dlopen (via libdl.so)
    [DllImport("libdl.so.2")]
    private static extern IntPtr dlopen(string filename, int flags);

    private const int RTLD_NOW = 2; // For dlopen

    public static void LoadNativeLibrary(string relativePath)
    {
        string fullPath = ProjectSettings.GlobalizePath(Path.Combine("res://", relativePath));
        string directory = Path.GetDirectoryName(fullPath);
        string libraryName = Path.GetFileName(fullPath);
        GD.Print($"Loading native library: {libraryName} from {directory}");

        try
        {
            if (OS.GetName() == "Windows")
            {
                // Windows loading
                if (!string.IsNullOrEmpty(directory))
                {
                    SetDllDirectory(directory);
                }

                try
                {
                    // Attempt to load from the specified directory
                    NativeLibrary.Load(directory + '\\' + libraryName);
                    GD.Print($"Windows: Loaded {libraryName} from {directory}");
                }
                catch (DllNotFoundException)
                {
                    // Fallback to loading from system PATH
                    GD.Print($"Windows: Attempting to load {libraryName} from system PATH...");
                    NativeLibrary.Load(libraryName);
                    GD.Print($"Windows: Successfully loaded {libraryName} from system PATH.");
                }
            }
            else if (OS.GetName() == "X11") // Linux
            {
                // Linux loading
                string linuxLibName = libraryName.Replace(".dll", ".so");
                string linuxPath = Path.Combine(directory, linuxLibName);

                IntPtr handle = IntPtr.Zero;

                if (!string.IsNullOrEmpty(directory))
                {
                    // First try loading with the full path
                    handle = dlopen(linuxPath, RTLD_NOW);
                }

                if (handle == IntPtr.Zero)
                {
                    // Fallback to loading from system PATH
                    GD.Print($"Linux: Attempting to load {linuxLibName} from system PATH...");
                    handle = dlopen(linuxLibName, RTLD_NOW);
                    if (handle == IntPtr.Zero)
                    {
                        throw new Exception($"Failed to load library: {Marshal.PtrToStringAnsi(dlerror())}");
                    }
                    GD.Print($"Linux: Successfully loaded {linuxLibName} from system PATH.");
                }
                else
                {
                    GD.Print($"Linux: Loaded {linuxPath}");
                }
            }
            else
            {
                GD.PrintErr($"Unsupported platform: {OS.GetName()}");
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load native library: {e.Message}");
        }
    }
    // For Linux error reporting
    [DllImport("libdl.so.2")]
    private static extern IntPtr dlerror();

}