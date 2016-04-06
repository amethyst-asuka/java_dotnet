Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This is a support class to help build property editors.
	''' <p>
	''' It can be used either as a base class or as a delegate.
	''' </summary>

	Public Class PropertyEditorSupport
		Implements PropertyEditor

		''' <summary>
		''' Constructs a <code>PropertyEditorSupport</code> object.
		''' 
		''' @since 1.5
		''' </summary>
		Public Sub New()
			source = Me
		End Sub

		''' <summary>
		''' Constructs a <code>PropertyEditorSupport</code> object.
		''' </summary>
		''' <param name="source"> the source used for event firing
		''' @since 1.5 </param>
		Public Sub New(  source As Object)
			If source Is Nothing Then Throw New NullPointerException
			source = source
		End Sub

		''' <summary>
		''' Returns the bean that is used as the
		''' source of events. If the source has not
		''' been explicitly set then this instance of
		''' <code>PropertyEditorSupport</code> is returned.
		''' </summary>
		''' <returns> the source object or this instance
		''' @since 1.5 </returns>
		Public Overridable Property source As Object
			Get
				Return source
			End Get
			Set(  source As Object)
				Me.source = source
			End Set
		End Property


		''' <summary>
		''' Set (or change) the object that is to be edited.
		''' </summary>
		''' <param name="value"> The new target object to be edited.  Note that this
		'''     object should not be modified by the PropertyEditor, rather
		'''     the PropertyEditor should create a new object to hold any
		'''     modified value. </param>
		Public Overridable Property value Implements PropertyEditor.setValue As Object
			Set(  value As Object)
				Me.value = value
				firePropertyChange()
			End Set
			Get
				Return value
			End Get
		End Property


		'----------------------------------------------------------------------

		''' <summary>
		''' Determines whether the class will honor the paintValue method.
		''' </summary>
		''' <returns>  True if the class will honor the paintValue method. </returns>

		Public Overridable Property paintable As Boolean Implements PropertyEditor.isPaintable
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' Paint a representation of the value into a given area of screen
		''' real estate.  Note that the propertyEditor is responsible for doing
		''' its own clipping so that it fits into the given rectangle.
		''' <p>
		''' If the PropertyEditor doesn't honor paint requests (see isPaintable)
		''' this method should be a silent noop.
		''' </summary>
		''' <param name="gfx">  Graphics object to paint into. </param>
		''' <param name="box">  Rectangle within graphics object into which we should paint. </param>
		Public Overridable Sub paintValue(  gfx As java.awt.Graphics,   box As java.awt.Rectangle) Implements PropertyEditor.paintValue
		End Sub

		'----------------------------------------------------------------------

		''' <summary>
		''' This method is intended for use when generating Java code to set
		''' the value of the property.  It should return a fragment of Java code
		''' that can be used to initialize a variable with the current property
		''' value.
		''' <p>
		''' Example results are "2", "new Color(127,127,34)", "Color.orange", etc.
		''' </summary>
		''' <returns> A fragment of Java code representing an initializer for the
		'''          current value. </returns>
		Public Overridable Property javaInitializationString As String Implements PropertyEditor.getJavaInitializationString
			Get
				Return "???"
			End Get
		End Property

		'----------------------------------------------------------------------

		''' <summary>
		''' Gets the property value as a string suitable for presentation
		''' to a human to edit.
		''' </summary>
		''' <returns> The property value as a string suitable for presentation
		'''       to a human to edit.
		''' <p>   Returns null if the value can't be expressed as a string.
		''' <p>   If a non-null value is returned, then the PropertyEditor should
		'''       be prepared to parse that string back in setAsText(). </returns>
		Public Overridable Property asText As String Implements PropertyEditor.getAsText
			Get
				Return If(Me.value IsNot Nothing, Me.value.ToString(), Nothing)
			End Get
			Set(  text As String)
				If TypeOf value Is String Then
					value = text
					Return
				End If
				Throw New System.ArgumentException(text)
			End Set
		End Property


		'----------------------------------------------------------------------

		''' <summary>
		''' If the property value must be one of a set of known tagged values,
		''' then this method should return an array of the tag values.  This can
		''' be used to represent (for example) enum values.  If a PropertyEditor
		''' supports tags, then it should support the use of setAsText with
		''' a tag value as a way of setting the value.
		''' </summary>
		''' <returns> The tag values for this property.  May be null if this
		'''   property cannot be represented as a tagged value.
		'''  </returns>
		Public Overridable Property tags As String() Implements PropertyEditor.getTags
			Get
				Return Nothing
			End Get
		End Property

		'----------------------------------------------------------------------

		''' <summary>
		''' A PropertyEditor may chose to make available a full custom Component
		''' that edits its property value.  It is the responsibility of the
		''' PropertyEditor to hook itself up to its editor Component itself and
		''' to report property value changes by firing a PropertyChange event.
		''' <P>
		''' The higher-level code that calls getCustomEditor may either embed
		''' the Component in some larger property sheet, or it may put it in
		''' its own individual dialog, or ...
		''' </summary>
		''' <returns> A java.awt.Component that will allow a human to directly
		'''      edit the current property value.  May be null if this is
		'''      not supported. </returns>

		Public Overridable Property customEditor As java.awt.Component Implements PropertyEditor.getCustomEditor
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Determines whether the propertyEditor can provide a custom editor.
		''' </summary>
		''' <returns>  True if the propertyEditor can provide a custom editor. </returns>
		Public Overridable Function supportsCustomEditor() As Boolean Implements PropertyEditor.supportsCustomEditor
			Return False
		End Function

		'----------------------------------------------------------------------

		''' <summary>
		''' Adds a listener for the value change.
		''' When the property editor changes its value
		''' it should fire a <seealso cref="PropertyChangeEvent"/>
		''' on all registered <seealso cref="PropertyChangeListener"/>s,
		''' specifying the {@code null} value for the property name.
		''' If the source property is set,
		''' it should be used as the source of the event.
		''' <p>
		''' The same listener object may be added more than once,
		''' and will be called as many times as it is added.
		''' If {@code listener} is {@code null},
		''' no exception is thrown and no action is taken.
		''' </summary>
		''' <param name="listener">  the <seealso cref="PropertyChangeListener"/> to add </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub addPropertyChangeListener(  listener As PropertyChangeListener) Implements PropertyEditor.addPropertyChangeListener
			If listeners Is Nothing Then listeners = New List(Of )
			listeners.Add(listener)
		End Sub

		''' <summary>
		''' Removes a listener for the value change.
		''' <p>
		''' If the same listener was added more than once,
		''' it will be notified one less time after being removed.
		''' If {@code listener} is {@code null}, or was never added,
		''' no exception is thrown and no action is taken.
		''' </summary>
		''' <param name="listener">  the <seealso cref="PropertyChangeListener"/> to remove </param>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Sub removePropertyChangeListener(  listener As PropertyChangeListener) Implements PropertyEditor.removePropertyChangeListener
			If listeners Is Nothing Then Return
			listeners.Remove(listener)
		End Sub

		''' <summary>
		''' Report that we have been modified to any interested listeners.
		''' </summary>
		Public Overridable Sub firePropertyChange()
			Dim targets As List(Of PropertyChangeListener)
			SyncLock Me
				If listeners Is Nothing Then Return
				targets = unsafeClone(listeners)
			End SyncLock
			' Tell our listeners that "everything" has changed.
			Dim evt As New PropertyChangeEvent(source, Nothing, Nothing, Nothing)

			For i As Integer = 0 To targets.Count - 1
				Dim target As PropertyChangeListener = targets(i)
				target.propertyChange(evt)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
		Private Function unsafeClone(Of T)(  v As List(Of T)) As List(Of T)
			Return CType(v.clone(), List(Of T))
		End Function

		'----------------------------------------------------------------------

		Private value As Object
		Private source As Object
		Private listeners As List(Of PropertyChangeListener)
	End Class

End Namespace