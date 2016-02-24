'
' * Copyright (c) 2003, 2011, Oracle and/or its affiliates. All rights reserved.
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
	''' An "IndexedPropertyChange" event gets delivered whenever a component that
	''' conforms to the JavaBeans&trade; specification (a "bean") changes a bound
	''' indexed property. This class is an extension of <code>PropertyChangeEvent</code>
	''' but contains the index of the property that has changed.
	''' <P>
	''' Null values may be provided for the old and the new values if their
	''' true values are not known.
	''' <P>
	''' An event source may send a null object as the name to indicate that an
	''' arbitrary set of if its properties have changed.  In this case the
	''' old and new values should also be null.
	''' 
	''' @since 1.5
	''' @author Mark Davidson
	''' </summary>
	Public Class IndexedPropertyChangeEvent
		Inherits PropertyChangeEvent

		Private Const serialVersionUID As Long = -320227448495806870L

		Private index As Integer

		''' <summary>
		''' Constructs a new <code>IndexedPropertyChangeEvent</code> object.
		''' </summary>
		''' <param name="source">  The bean that fired the event. </param>
		''' <param name="propertyName">  The programmatic name of the property that
		'''             was changed. </param>
		''' <param name="oldValue">      The old value of the property. </param>
		''' <param name="newValue">      The new value of the property. </param>
		''' <param name="index"> index of the property element that was changed. </param>
		Public Sub New(ByVal source As Object, ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object, ByVal index As Integer)
			MyBase.New(source, propertyName, oldValue, newValue)
			Me.index = index
		End Sub

		''' <summary>
		''' Gets the index of the property that was changed.
		''' </summary>
		''' <returns> The index specifying the property element that was
		'''         changed. </returns>
		Public Overridable Property index As Integer
			Get
				Return index
			End Get
		End Property

		Friend Overrides Sub appendTo(ByVal sb As StringBuilder)
			sb.append("; index=").append(index)
		End Sub
	End Class

End Namespace