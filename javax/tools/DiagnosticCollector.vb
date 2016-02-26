Imports System.Collections.Generic

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
	''' Provides an easy way to collect diagnostics in a list.
	''' </summary>
	''' @param <S> the type of source objects used by diagnostics received
	''' by this object
	''' 
	''' @author Peter von der Ah&eacute;
	''' @since 1.6 </param>
	Public NotInheritable Class DiagnosticCollector(Of S)
		Implements DiagnosticListener(Of S)

'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Private diagnostics As IList(Of Diagnostic(Of ? As S)) = java.util.Collections.synchronizedList(New List(Of Diagnostic(Of ? As S)))

		Public Sub report(Of T1 As S)(ByVal diagnostic As Diagnostic(Of T1)) Implements DiagnosticListener(Of S).report
			diagnostic.GetType() ' null check
			diagnostics.Add(diagnostic)
		End Sub

		''' <summary>
		''' Gets a list view of diagnostics collected by this object.
		''' </summary>
		''' <returns> a list view of diagnostics </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		Public Property diagnostics As IList(Of Diagnostic(Of ? As S))
			Get
				Return java.util.Collections.unmodifiableList(diagnostics)
			End Get
		End Property
	End Class

End Namespace