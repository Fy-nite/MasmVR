extends Command

# This method is executed when the command is detected.
# The command is trimmed by spaces, and parameters are passed as `params`.
func _start(params: Array[String]) -> int:
	print("meow")
# This method executes when `_start()` returns `ExecuteStatus.RUNNING` and continues
# running until `_running()` itself returns `ExecuteStatus.DONE`.
	return ExecuteStatus.DONE
func _running() -> int:
	var text = AnsiString.new().background_rgb(112, 112, 112).foreground_256(3).italic().append("Hello World").de_italic().clear_style().append("\r\nHello You\r\n")

	# This will print `text` to the terminal. `text` is a special string that can contain
	# control characters and escape sequences for stylized output and cursor control.
	#
	# NOTICE: `echo` will `queue_free()` the AnsiString automatically.
	echo(text)
	return ExecuteStatus.DONE
	# Return values:
	# ExecuteStatus.DONE
	# ExecuteStatus.RUNNING
