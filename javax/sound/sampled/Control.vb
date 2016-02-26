Imports System

'
' * Copyright (c) 1999, 2002, Oracle and/or its affiliates. All rights reserved.
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
	''' <seealso cref="Line Lines"/> often have a set of controls, such as gain and pan, that affect
	''' the audio signal passing through the line.  Java Sound's <code>Line</code> objects
	''' let you obtain a particular control object by passing its class as the
	''' argument to a <seealso cref="Line#getControl(Control.Type) getControl"/> method.
	''' <p>
	''' Because the various types of controls have different purposes and features,
	''' all of their functionality is accessed from the subclasses that define
	''' each kind of control.
	''' 
	''' @author Kara Kytle
	''' </summary>
	''' <seealso cref= Line#getControls </seealso>
	''' <seealso cref= Line#isControlSupported
	''' @since 1.3 </seealso>
	Public MustInherit Class Control


		' INSTANCE VARIABLES

		''' <summary>
		''' The control type.
		''' </summary>
		Private ReadOnly type As Type



		' CONSTRUCTORS

		''' <summary>
		''' Constructs a Control with the specified type. </summary>
		''' <param name="type"> the kind of control desired </param>
		Protected Friend Sub New(ByVal type As Type)
			Me.type = type
		End Sub


		' METHODS

		''' <summary>
		''' Obtains the control's type. </summary>
		''' <returns> the control's type. </returns>
		Public Overridable Property type As Type
			Get
				Return type
			End Get
		End Property


		' ABSTRACT METHODS

		''' <summary>
		''' Obtains a String describing the control type and its current state. </summary>
		''' <returns> a String representation of the Control. </returns>
		Public Overrides Function ToString() As String
			Return New String(type & " Control")
		End Function


		''' <summary>
		''' An instance of the <code>Type</code> class represents the type of
		''' the control.  Static instances are provided for the
		''' common types.
		''' </summary>
		Public Class Type

			' CONTROL TYPE DEFINES

			' INSTANCE VARIABLES

			''' <summary>
			''' Type name.
			''' </summary>
			Private name As String


			' CONSTRUCTOR

			''' <summary>
			''' Constructs a new control type with the name specified.
			''' The name should be a descriptive string appropriate for
			''' labelling the control in an application, such as "Gain" or "Balance." </summary>
			''' <param name="name">  the name of the new control type. </param>
			Protected Friend Sub New(ByVal name As String)
				Me.name = name
			End Sub


			' METHODS

			''' <summary>
			''' Finalizes the equals method
			''' </summary>
			Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
				Return MyBase.Equals(obj)
			End Function

			''' <summary>
			''' Finalizes the hashCode method
			''' </summary>
			Public NotOverridable Overrides Function GetHashCode() As Integer
				Return MyBase.GetHashCode()
			End Function

			''' <summary>
			''' Provides the <code>String</code> representation of the control type.  This <code>String</code> is
			''' the same name that was passed to the constructor.
			''' </summary>
			''' <returns> the control type name </returns>
			Public NotOverridable Overrides Function ToString() As String
				Return name
			End Function
		End Class ' class Type

	End Class ' class Control

End Namespace