using System;
using System.IO;
using static MicroAsmRuntime; // Import static members for easier access
using Godot;
public class MicroASMRunner
{
	public static int start()
	{
		GD.Print("MicroASM C# Host Test");

		// --- Configuration ---
		string bytecodeFile = "example.bin"; // Ensure this file exists and is copied to output
		int ramSize = 65536; // 64 KB
		bool debugMode = true;
		string[] scriptArgs = { "arg1_from_csharp", "arg2" }; // Example args for the script
		// --- End Configuration ---

		// Check if bytecode file exists
		if (!File.Exists(bytecodeFile))
		{
			Console.ForegroundColor = ConsoleColor.Red;
			GD.Print($"Error: Bytecode file not found: {Path.GetFullPath(bytecodeFile)}");
			GD.Print("Please ensure 'example.bin' is compiled and copied to the output directory.");
			Console.ResetColor();
			return 1;
		}

		MasmInterpreterHandle handle = MasmInterpreterHandle.Zero;
		try
		{
			// 1. Create Interpreter
			GD.Print("Creating interpreter...");
			handle = CreateInterpreter(ramSize, debugMode ? 1 : 0);
			if (handle.IsZero)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				GD.Print($"Failed to create interpreter: {GetLastError() ?? "Unknown error"}");
				GD.Print("Ensure the runtime DLL exports 'masm_create_interpreter' with the correct name and calling convention.");
				Console.ResetColor();
				return 1;
			}
			GD.Print("Interpreter created.");

			// 2. Load Bytecode
			GD.Print($"Loading bytecode from '{bytecodeFile}'...");
			MasmResult loadResult = LoadBytecode(handle, bytecodeFile);
			if (loadResult != MasmResult.Ok)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				GD.Print($"Failed to load bytecode: [{loadResult}] {GetLastError() ?? "Unknown error"}");
				Console.ResetColor();
				return 1;
			}
			GD.Print("Bytecode loaded.");

			// 3. Execute
			GD.Print("Executing bytecode...");
			GD.Print("--- Script Output Start ---");
			Console.ForegroundColor = ConsoleColor.Cyan; // Differentiate script output

			MasmResult execResult = Execute(handle, scriptArgs); // Use helper to pass args

			Console.ResetColor();
			GD.Print("--- Script Output End ---");

			if (execResult != MasmResult.Ok)
			{
				// Error message should have been Printed by the interpreter's execute or captured by GetLastError
				Console.ForegroundColor = ConsoleColor.Red;
				GD.Print($"Execution failed: [{execResult}] {GetLastError() ?? "Interpreter internal error"}");
				Console.ResetColor();
				// Don't exit immediately, try to get register info if possible
			}
			else
			{
				GD.Print("Execution finished (HLT likely reached).");
			}


			// 4. Example: Inspect a register after execution (e.g., R0)
			if (GetRegister(handle, 0, out int r0Value) == MasmResult.Ok)
			{
				GD.Print($"Value of R0 after execution: {r0Value}");
			}
			else
			{
				GD.Print($"Could not get R0 value: {GetLastError() ?? "Unknown error"}");
			}

			// 5. Example: Write/Read RAM
			int testAddress = 100; // Choose an address not likely used by stack/data
			int testValue = 12345;
			GD.Print($"Writing {testValue} to RAM address {testAddress}...");
			if (WriteRamInt(handle, testAddress, testValue) == MasmResult.Ok)
			{
				GD.Print($"Reading back from RAM address {testAddress}...");
				if (ReadRamInt(handle, testAddress, out int readValue) == MasmResult.Ok)
				{
					GD.Print($"Read value: {readValue} (Expected: {testValue})");
					if (readValue != testValue) GD.Print("RAM Read/Write Mismatch!");
				}
				else
				{
					GD.Print($"Failed to read RAM: {GetLastError() ?? "Unknown error"}");
				}
			}
			else
			{
				GD.Print($"Failed to write RAM: {GetLastError() ?? "Unknown error"}");
			}


		}
		catch (EntryPointNotFoundException ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			GD.Print($"Error: Entry point not found in the runtime DLL. {ex.Message}");
			GD.Print("Ensure the runtime DLL exports the required functions with the correct names.");
			Console.ResetColor();
			return 1;
		}
		catch (DllNotFoundException)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			GD.Print($"Error: Cannot find the MicroASM runtime DLL ('{DllName}.dll').");
			GD.Print("Ensure the C++ project is built as a DLL and it's in the same directory as the C# executable or in the system's PATH.");
			Console.ResetColor();
			return 1;
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			GD.Print($"An unexpected C# error occurred: {ex.Message}");
			GD.Print(ex.StackTrace);
			Console.ResetColor();
			return 1;
		}
		finally
		{
			// 6. Destroy Interpreter
			if (!handle.IsZero)
			{
				GD.Print("Destroying interpreter...");
				DestroyInterpreter(handle);
				GD.Print("Interpreter destroyed.");
			}
		}

		GD.Print("Host program finished.");
		return 0;
	}
}
