using Godot;
using System;
using System.Collections.Generic;

public partial class HighlightTextEdit : TextEdit
{
	// get the syntax highligher on the text edit
	public CodeEdit _textEdit;
	public SyntaxHighlighter _syntaxHighlighter;
	public string _text;
	public Dictionary<string, Color> _colors = new Dictionary<string, Color>();
	public override void _Ready()
	{
		_textEdit = this.GetChild<CodeEdit>(0);
	
		_text = _textEdit.Text; // cache this for later
		// connect the text changed signal to the _on_text_changed method
		_textEdit.TextChanged += _on_text_changed;
		// set the colors for the syntax highlighter
		_colors["mov"] = new Color(0.5f, 0.5f, 1.0f); // blue
		_colors["add"] = new Color(0.5f, 1.0f, 0.5f); // green
		_colors["sub"] = new Color(1.0f, 0.5f, 0.5f); // red
		_colors["mul"] = new Color(1.0f, 1.0f, 0.5f); // yellow
		_colors["div"] = new Color(1.0f, 0.5f, 1.0f); // purple
		_colors["jmp"] = new Color(0.5f, 1.0f, 1.0f); // cyan
		_colors["lbl"] = new Color(1.0f, 1.0f, 1.0f); // white
	   

	}
	public void _on_text_changed()
	{
		// get the text from the text edit
		_text = _textEdit.Text;
		// get the current line
		int currentLine = _textEdit.GetCaretLine();

	}
  
}
