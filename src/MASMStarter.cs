using Godot;
using SharpMASM;
using System.IO;
using System.Buffers.Text;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;
public partial class MASMStarter : Control
{
	[Export] public HighlightTextEdit textEdit;
	[Export] public Button startButton;
	[Export] public Button stopButton;
	[Export] public Button clearoutButton;
	[Export] public Button clearerrButton;

	// Removed static field initialization referencing non-static field
	public static RichTextLabel output_label;
	public static RichTextLabel error_label;
	public string[] lines;

	public override void _Ready()
	{
		GD.Print("MASMStarter: _Ready() - Initializing...");
		GD.Print("Starting MicroASM script...");
		Console.SetOut(new printingout());
		Console.SetError(new printingerr());
		Functions.InitializeMemory();
		GD.Print("MASMStarter: Memory initialized.");
		startButton = (Button)FindChild("Run", true);
		GD.Print("MASMStarter: Start button assigned.");
		output_label = (RichTextLabel)FindChild("infolabel", true);
		GD.Print("MASMStarter: Output label assigned.");
		error_label = (RichTextLabel)FindChild("errorlabel", true);
		GD.Print("MASMStarter: Error label assigned.");
		textEdit = (HighlightTextEdit)FindChild("CodeEdit", true);
		GD.Print("MASMStarter: TextEdit assigned.");
		GD.Print("MASMStarter: _Ready() called");

		if (startButton != null)
		{
			GD.Print("Start button found");

		}
		else
		{
			GD.Print("Start button not found");
			throw new MASMException("Start button not found");
		}
		if (output_label != null)
		{
			GD.Print("Output label found");
		}
		else
		{
			GD.Print("Output label not found");
		}
		if (error_label != null)
		{
			GD.Print("Error label found");
		}
		else
		{
			GD.Print("Error label not found");
		}
		if (textEdit != null)
		{
			GD.Print("TextEdit found");
		}
		else
		{
			GD.Print("TextEdit not found");
		}
		GD.Print("assigning the button press to the shit");
		startButton.Pressed += _on_run_pressed;
		GD.Print("MASMStarter: Button press event connected.");
		GD.Print("MASMStarter: _Ready() - Initialization complete.");
		clearoutButton.Pressed += _on_clearout_pressed;
		GD.Print("MASMStarter: Clear output button event connected.");
	}

	public void _on_clearout_pressed()
	{
		GD.Print("MASMStarter: _on_clearout_pressed() - Clear output button pressed.");
		printingout.clearText(output_label);
	}
	public void _on_run_pressed()
	{
		var wrapper = new MASMWrapper(Core_Interpreter.Interpret);
		wrapper.ExecuteWithExceptionHandling(() =>
		{
			GD.Print("MASMStarter: _on_run_pressed() - Button pressed.");
			lines = textEdit.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
	.Select(line => line.Trim())
	.Where(line => !string.IsNullOrWhiteSpace(line))
	.ToArray();
			GD.Print("MASMStarter: Lines processed from TextEdit.");
			Console.SetOut(new printingout());
			Console.SetError(new printingerr());
			GD.Print("MASMStarter: Console output and error redirected.");
			Core_Interpreter.Instructions meow = Core_Interpreter.Process_File(lines);
			GD.Print("MASMStarter: Instructions processed.");
			Core_Interpreter.Interpret(meow);
			GD.Print("MASMStarter: Instructions interpreted.");
			GC.Collect();
		});
	}
}

public class printingout : TextWriter
{
	// Fix for CA1861: Use 'static readonly' for the Encoding property to avoid repeated instantiation.
	public static readonly Encoding Utf8Encoding = Encoding.UTF8;
	[Export] public RichTextLabel outputLabel;

	public printingout()
	{
		GD.Print("Creating printingout instance...");
		outputLabel = (RichTextLabel)MASMStarter.output_label;
		GD.Print("printingout: Output label assigned.");
	}

	public override Encoding Encoding => Utf8Encoding;

	public override void Write(char value)
	{
		GD.Print($"printingout: Writing char '{value}' to output label.");
		outputLabel.Text += value;
	}

	public override void Write(string? value)
	{
		if (value == null)
		{
			GD.Print("printingout: Null value received, skipping write.");
			return;
		}
		GD.Print($"printingout: Writing string '{value}' to output label.");
		outputLabel.Text += value;
	}

	public override void WriteLine(string value)
	{
		GD.Print($"printingout: Writing line '{value}' to output label.");
		if (outputLabel == null)
		{
			GD.Print("printingout: Output label is null, skipping write.");
			return;
		}
		else
		{
			outputLabel.Text += value + "\n";
		}
	}

	public static void clearText(RichTextLabel outputLabel)
	{
		GD.Print("Clearing output text...");
		// Clear the output label
		outputLabel.Text = string.Empty;
	}
}

public class printingerr : TextWriter
{
	// Fix for CA1861: Use 'static readonly' for the Encoding property to avoid repeated instantiation.
	public static readonly Encoding Utf8Encoding = Encoding.UTF8;
	[Export] public RichTextLabel errorLabel;

	public override Encoding Encoding => Utf8Encoding;

	public override void Write(char value)
	{
		GD.Print($"printingerr: Writing char '{value}' to error label.");
		errorLabel.Text += value;
	}

	public override void Write(string? value)
	{
		GD.Print($"printingerr: Writing string '{value}' to error label.");
		errorLabel.Text += value;
	}

	public override void WriteLine(string? value)
	{
		GD.Print($"printingerr: Writing line '{value}' to error label.");
		errorLabel.Text += value + "\n";
	}
}

public class printing : TextWriter
{
	// Fix for CA1861: Use 'static readonly' for the Encoding property to avoid repeated instantiation.
	public static readonly Encoding Utf8Encoding = Encoding.UTF8;

	public override Encoding Encoding => Utf8Encoding;

	public override void Write(char value)
	{
		GD.Print($"printing: Writing char '{value}' to console.");
		GD.Print(value);
	}

	public override void Write(string? value)
	{
		GD.Print($"printing: Writing string '{value}' to console.");
		GD.Print(value);
	}

	public override void WriteLine(string? value)
	{
		GD.Print($"printing: Writing line '{value}' to console.");
		GD.Print(value);
	}
}
