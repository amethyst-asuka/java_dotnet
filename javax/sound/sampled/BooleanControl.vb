Imports System

'
' * Copyright (c) 1999, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A <code>BooleanControl</code> provides the ability to switch between
	''' two possible settings that affect a line's audio.  The settings are boolean
	''' values (<code>true</code> and <code>false</code>).  A graphical user interface
	''' might represent the control by a two-state button, an on/off switch, two
	''' mutually exclusive buttons, or a checkbox (among other possibilities).
	''' For example, depressing a button might activate a
	''' <code><seealso cref="BooleanControl.Type#MUTE MUTE"/></code> control to silence
	''' the line's audio.
	''' <p>
	''' As with other <code><seealso cref="Control"/></code> subclasses, a method is
	''' provided that returns string labels for the values, suitable for
	''' display in the user interface.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class BooleanControl
		Inherits Control


		' INSTANCE VARIABLES

		''' <summary>
		''' The <code>true</code> state label, such as "true" or "on."
		''' </summary>
		Private ReadOnly trueStateLabel As String

		''' <summary>
		''' The <code>false</code> state label, such as "false" or "off."
		''' </summary>
		Private ReadOnly falseStateLabel As String

		''' <summary>
		''' The current value.
		''' </summary>
		Private value As Boolean


		' CONSTRUCTORS


		''' <summary>
		''' Constructs a new boolean control object with the given parameters.
		''' </summary>
		''' <param name="type"> the type of control represented this float control object </param>
		''' <param name="initialValue"> the initial control value </param>
		''' <param name="trueStateLabel"> the label for the state represented by <code>true</code>,
		''' such as "true" or "on." </param>
		''' <param name="falseStateLabel"> the label for the state represented by <code>false</code>,
		''' such as "false" or "off." </param>
		Protected Friend Sub New(ByVal ___type As Type, ByVal initialValue As Boolean, ByVal trueStateLabel As String, ByVal falseStateLabel As String)

			MyBase.New(___type)
			Me.value = initialValue
			Me.trueStateLabel = trueStateLabel
			Me.falseStateLabel = falseStateLabel
		End Sub


		''' <summary>
		''' Constructs a new boolean control object with the given parameters.
		''' The labels for the <code>true</code> and <code>false</code> states
		''' default to "true" and "false."
		''' </summary>
		''' <param name="type"> the type of control represented by this float control object </param>
		''' <param name="initialValue"> the initial control value </param>
		Protected Friend Sub New(ByVal ___type As Type, ByVal initialValue As Boolean)
			Me.New(___type, initialValue, "true", "false")
		End Sub


		' METHODS


		''' <summary>
		''' Sets the current value for the control.  The default
		''' implementation simply sets the value as indicated.
		''' Some controls require that their line be open before they can be affected
		''' by setting a value. </summary>
		''' <param name="value"> desired new value. </param>
		Public Overridable Property value As Boolean
			Set(ByVal value As Boolean)
				Me.value = value
			End Set
			Get
				Return value
			End Get
		End Property





		''' <summary>
		''' Obtains the label for the specified state. </summary>
		''' <param name="state"> the state whose label will be returned </param>
		''' <returns> the label for the specified state, such as "true" or "on"
		''' for <code>true</code>, or "false" or "off" for <code>false</code>. </returns>
		Public Overridable Function getStateLabel(ByVal state As Boolean) As String
			Return (If(state = True, trueStateLabel, falseStateLabel))
		End Function



		' ABSTRACT METHOD IMPLEMENTATIONS: CONTROL


		''' <summary>
		''' Provides a string representation of the control </summary>
		''' <returns> a string description </returns>
		Public Overrides Function ToString() As String
			Return New String(MyBase.ToString() & " with current value: " & getStateLabel(value))
		End Function


		' INNER CLASSES


		''' <summary>
		''' An instance of the <code>BooleanControl.Type</code> class identifies one kind of
		''' boolean control.  Static instances are provided for the
		''' common types.
		''' 
		''' @author Kara Kytle
		''' @since 1.3
		''' </summary>
		Public Class Type
			Inherits Control.Type


			' TYPE DEFINES


			''' <summary>
			''' Represents a control for the mute status of a line.
			''' Note that mute status does not affect gain.
			''' </summary>
			Public Shared ReadOnly MUTE As New Type("Mute")

			''' <summary>
			''' Represents a control for whether reverberation is applied
			''' to a line.  Note that the status of this control not affect
			''' the reverberation settings for a line, but does affect whether
			''' these settings are used.
			''' </summary>
			Public Shared ReadOnly APPLY_REVERB As New Type("Apply Reverb")


			' CONSTRUCTOR


			''' <summary>
			''' Constructs a new boolean control type. </summary>
			''' <param name="name">  the name of the new boolean control type </param>
			Protected Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub
		End Class ' class Type
	End Class

End Namespace