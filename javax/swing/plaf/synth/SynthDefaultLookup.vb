'
' * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth


	''' <summary>
	''' SynthDefaultLookup redirects all lookup calls to the SynthContext.
	''' 
	''' @author Scott Violet
	''' </summary>
	Friend Class SynthDefaultLookup
		Inherits sun.swing.DefaultLookup

		Public Overridable Function getDefault(ByVal c As javax.swing.JComponent, ByVal ui As javax.swing.plaf.ComponentUI, ByVal key As String) As Object
			If Not(TypeOf ui Is SynthUI) Then
				Dim value As Object = MyBase.getDefault(c, ui, key)
				Return value
			End If
			Dim context As SynthContext = CType(ui, SynthUI).getContext(c)
			Dim value As Object = context.style.get(context, key)
			context.Dispose()
			Return value
		End Function
	End Class

End Namespace