Imports System
Imports System.Text

'
' * Copyright (c) 1999, 2003, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.sound.sampled

	''' <summary>
	''' A <code>CompoundControl</code>, such as a graphic equalizer, provides control
	''' over two or more related properties, each of which is itself represented as
	''' a <code>Control</code>.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class CompoundControl
		Inherits Control


		' TYPE DEFINES


		' INSTANCE VARIABLES


		''' <summary>
		''' The set of member controls.
		''' </summary>
		Private controls As Control()



		' CONSTRUCTORS


		''' <summary>
		''' Constructs a new compound control object with the given parameters.
		''' </summary>
		''' <param name="type"> the type of control represented this compound control object </param>
		''' <param name="memberControls"> the set of member controls </param>
		Protected Friend Sub New(ByVal type As Type, ByVal memberControls As Control())

			MyBase.New(type)
			Me.controls = memberControls
		End Sub



		' METHODS


		''' <summary>
		''' Returns the set of member controls that comprise the compound control. </summary>
		''' <returns> the set of member controls. </returns>
		Public Overridable Property memberControls As Control()
			Get
    
				Dim localArray As Control() = New Control(controls.Length - 1){}
    
				For i As Integer = 0 To controls.Length - 1
					localArray(i) = controls(i)
				Next i
    
				Return localArray
			End Get
		End Property


		' ABSTRACT METHOD IMPLEMENTATIONS: CONTROL


		''' <summary>
		''' Provides a string representation of the control </summary>
		''' <returns> a string description </returns>
		Public Overrides Function ToString() As String

			Dim buf As New StringBuilder
			For i As Integer = 0 To controls.Length - 1
				If i <> 0 Then
					buf.Append(", ")
					If (i + 1) = controls.Length Then buf.Append("and ")
				End If
				buf.Append(controls(i).type)
			Next i

			Return New String(type & " Control containing " & buf & " Controls.")
		End Function


		' INNER CLASSES


		''' <summary>
		''' An instance of the <code>CompoundControl.Type</code> inner class identifies one kind of
		''' compound control.  Static instances are provided for the
		''' common types.
		''' 
		''' @author Kara Kytle
		''' @since 1.3
		''' </summary>
		Public Class Type
			Inherits Control.Type


			' TYPE DEFINES

			' CONSTRUCTOR


			''' <summary>
			''' Constructs a new compound control type. </summary>
			''' <param name="name">  the name of the new compound control type </param>
			Protected Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub
		End Class ' class Type

	End Class ' class CompoundControl

End Namespace