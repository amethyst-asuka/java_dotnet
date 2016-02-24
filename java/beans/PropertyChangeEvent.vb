'
' * Copyright (c) 1996, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans


	''' <summary>
	''' A "PropertyChange" event gets delivered whenever a bean changes a "bound"
	''' or "constrained" property.  A PropertyChangeEvent object is sent as an
	''' argument to the PropertyChangeListener and VetoableChangeListener methods.
	''' <P>
	''' Normally PropertyChangeEvents are accompanied by the name and the old
	''' and new value of the changed property.  If the new value is a primitive
	''' type (such as int or boolean) it must be wrapped as the
	''' corresponding java.lang.* Object type (such as Integer or Boolean).
	''' <P>
	''' Null values may be provided for the old and the new values if their
	''' true values are not known.
	''' <P>
	''' An event source may send a null object as the name to indicate that an
	''' arbitrary set of if its properties have changed.  In this case the
	''' old and new values should also be null.
	''' </summary>
	Public Class PropertyChangeEvent
		Inherits java.util.EventObject

		Private Const serialVersionUID As Long = 7042693688939648123L

		''' <summary>
		''' Constructs a new {@code PropertyChangeEvent}.
		''' </summary>
		''' <param name="source">        the bean that fired the event </param>
		''' <param name="propertyName">  the programmatic name of the property that was changed </param>
		''' <param name="oldValue">      the old value of the property </param>
		''' <param name="newValue">      the new value of the property
		''' </param>
		''' <exception cref="IllegalArgumentException"> if {@code source} is {@code null} </exception>
		Public Sub New(ByVal source As Object, ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			MyBase.New(source)
			Me.propertyName = propertyName
			Me.newValue = newValue
			Me.oldValue = oldValue
		End Sub

		''' <summary>
		''' Gets the programmatic name of the property that was changed.
		''' </summary>
		''' <returns>  The programmatic name of the property that was changed.
		'''          May be null if multiple properties have changed. </returns>
		Public Overridable Property propertyName As String
			Get
				Return propertyName
			End Get
		End Property

		''' <summary>
		''' Gets the new value for the property, expressed as an Object.
		''' </summary>
		''' <returns>  The new value for the property, expressed as an Object.
		'''          May be null if multiple properties have changed. </returns>
		Public Overridable Property newValue As Object
			Get
				Return newValue
			End Get
		End Property

		''' <summary>
		''' Gets the old value for the property, expressed as an Object.
		''' </summary>
		''' <returns>  The old value for the property, expressed as an Object.
		'''          May be null if multiple properties have changed. </returns>
		Public Overridable Property oldValue As Object
			Get
				Return oldValue
			End Get
		End Property

		''' <summary>
		''' Sets the propagationId object for the event.
		''' </summary>
		''' <param name="propagationId">  The propagationId object for the event. </param>
		Public Overridable Property propagationId As Object
			Set(ByVal propagationId As Object)
				Me.propagationId = propagationId
			End Set
			Get
				Return propagationId
			End Get
		End Property


		''' <summary>
		''' name of the property that changed.  May be null, if not known.
		''' @serial
		''' </summary>
		Private propertyName As String

		''' <summary>
		''' New value for property.  May be null if not known.
		''' @serial
		''' </summary>
		Private newValue As Object

		''' <summary>
		''' Previous value for property.  May be null if not known.
		''' @serial
		''' </summary>
		Private oldValue As Object

		''' <summary>
		''' Propagation ID.  May be null.
		''' @serial </summary>
		''' <seealso cref= #getPropagationId </seealso>
		Private propagationId As Object

		''' <summary>
		''' Returns a string representation of the object.
		''' </summary>
		''' <returns> a string representation of the object
		''' 
		''' @since 1.7 </returns>
		Public Overrides Function ToString() As String
			Dim sb As New StringBuilder(Me.GetType().name)
			sb.append("[propertyName=").append(propertyName)
			appendTo(sb)
			sb.append("; oldValue=").append(oldValue)
			sb.append("; newValue=").append(newValue)
			sb.append("; propagationId=").append(propagationId)
			sb.append("; source=").append(source)
			Return sb.append("]").ToString()
		End Function

		Friend Overridable Sub appendTo(ByVal sb As StringBuilder)
		End Sub
	End Class

End Namespace