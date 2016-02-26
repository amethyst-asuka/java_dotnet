'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
' * ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' *
' 

Namespace javax.tools

	''' <summary>
	''' Interface for recognizing options.
	''' 
	''' @author Peter von der Ah&eacute;
	''' @since 1.6
	''' </summary>
	Public Interface OptionChecker

		''' <summary>
		''' Determines if the given option is supported and if so, the
		''' number of arguments the option takes.
		''' </summary>
		''' <param name="option"> an option </param>
		''' <returns> the number of arguments the given option takes or -1 if
		''' the option is not supported </returns>
		Function isSupportedOption(ByVal [option] As String) As Integer

	End Interface

End Namespace