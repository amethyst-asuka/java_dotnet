'
' * Copyright (c) 2005, 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' Interface for receiving diagnostics from tools.
	''' </summary>
	''' @param <S> the type of source objects used by diagnostics received
	''' by this listener
	''' 
	''' @author Jonathan Gibbons
	''' @author Peter von der Ah&eacute;
	''' @since 1.6 </param>
	Public Interface DiagnosticListener(Of S)
		''' <summary>
		''' Invoked when a problem is found.
		''' </summary>
		''' <param name="diagnostic"> a diagnostic representing the problem that
		''' was found </param>
		''' <exception cref="NullPointerException"> if the diagnostic argument is
		''' {@code null} and the implementation cannot handle {@code null}
		''' arguments </exception>
		Sub report(Of T1 As S)(ByVal diagnostic As Diagnostic(Of T1))
	End Interface

End Namespace