Imports System

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
	''' A <code>EnumControl</code> provides control over a set of
	''' discrete possible values, each represented by an object.  In a
	''' graphical user interface, such a control might be represented
	''' by a set of buttons, each of which chooses one value or setting.  For
	''' example, a reverb control might provide several preset reverberation
	''' settings, instead of providing continuously adjustable parameters
	''' of the sort that would be represented by <code><seealso cref="FloatControl"/></code>
	''' objects.
	''' <p>
	''' Controls that provide a choice between only two settings can often be implemented
	''' instead as a <code><seealso cref="BooleanControl"/></code>, and controls that provide
	''' a set of values along some quantifiable dimension might be implemented
	''' instead as a <code>FloatControl</code> with a coarse resolution.
	''' However, a key feature of <code>EnumControl</code> is that the returned values
	''' are arbitrary objects, rather than numerical or boolean values.  This means that each
	''' returned object can provide further information.  As an example, the settings
	''' of a <code><seealso cref="EnumControl.Type#REVERB REVERB"/></code> control are instances of
	''' <code><seealso cref="ReverbType"/></code> that can be queried for the parameter values
	''' used for each setting.
	''' 
	''' @author Kara Kytle
	''' @since 1.3
	''' </summary>
	Public MustInherit Class EnumControl
		Inherits Control


		' TYPE DEFINES


		' INSTANCE VARIABLES


		''' <summary>
		''' The set of possible values.
		''' </summary>
		Private values As Object()


		''' <summary>
		''' The current value.
		''' </summary>
		Private value As Object



		' CONSTRUCTORS


		''' <summary>
		''' Constructs a new enumerated control object with the given parameters.
		''' </summary>
		''' <param name="type"> the type of control represented this enumerated control object </param>
		''' <param name="values"> the set of possible values for the control </param>
		''' <param name="value"> the initial control value </param>
		Protected Friend Sub New(ByVal type As Type, ByVal values As Object(), ByVal value As Object)

			MyBase.New(type)

			Me.values = values
			Me.value = value
		End Sub



		' METHODS


		''' <summary>
		''' Sets the current value for the control.  The default implementation
		''' simply sets the value as indicated.  If the value indicated is not
		''' supported, an IllegalArgumentException is thrown.
		''' Some controls require that their line be open before they can be affected
		''' by setting a value. </summary>
		''' <param name="value"> the desired new value </param>
		''' <exception cref="IllegalArgumentException"> if the value indicated does not fall
		''' within the allowable range </exception>
		Public Overridable Property value As Object
			Set(ByVal value As Object)
				If Not isValueSupported(value) Then Throw New System.ArgumentException("Requested value " & value & " is not supported.")
    
				Me.value = value
			End Set
			Get
				Return value
			End Get
		End Property




		''' <summary>
		''' Returns the set of possible values for this control. </summary>
		''' <returns> the set of possible values </returns>
		Public Overridable Property values As Object()
			Get
    
				Dim localArray As Object() = New Object(values.Length - 1){}
    
				For i As Integer = 0 To values.Length - 1
					localArray(i) = values(i)
				Next i
    
				Return localArray
			End Get
		End Property


		''' <summary>
		''' Indicates whether the value specified is supported. </summary>
		''' <param name="value"> the value for which support is queried </param>
		''' <returns> <code>true</code> if the value is supported,
		''' otherwise <code>false</code> </returns>
		Private Function isValueSupported(ByVal value As Object) As Boolean

			For i As Integer = 0 To values.Length - 1
				'$$fb 2001-07-20: Fix for bug 4400392: setValue() in ReverbControl always throws Exception
				'if (values.equals(values[i])) {
				If value.Equals(values(i)) Then Return True
			Next i

			Return False
		End Function



		' ABSTRACT METHOD IMPLEMENTATIONS: CONTROL


		''' <summary>
		''' Provides a string representation of the control. </summary>
		''' <returns> a string description </returns>
		Public Overrides Function ToString() As String
			Return New String(type & " with current value: " & value)
		End Function


		' INNER CLASSES


		''' <summary>
		''' An instance of the <code>EnumControl.Type</code> inner class identifies one kind of
		''' enumerated control.  Static instances are provided for the
		''' common types.
		''' </summary>
		''' <seealso cref= EnumControl
		''' 
		''' @author Kara Kytle
		''' @since 1.3 </seealso>
		Public Class Type
			Inherits Control.Type


			' TYPE DEFINES

			''' <summary>
			''' Represents a control over a set of possible reverberation settings.
			''' Each reverberation setting is described by an instance of the
			''' <seealso cref="ReverbType"/> class.  (To access these settings,
			''' invoke <code><seealso cref="EnumControl#getValues"/></code> on an
			''' enumerated control of type <code>REVERB</code>.)
			''' </summary>
			Public Shared ReadOnly REVERB As New Type("Reverb")


			' CONSTRUCTOR


			''' <summary>
			''' Constructs a new enumerated control type. </summary>
			''' <param name="name">  the name of the new enumerated control type </param>
			Protected Friend Sub New(ByVal name As String)
				MyBase.New(name)
			End Sub
		End Class ' class Type

	End Class ' class EnumControl

End Namespace