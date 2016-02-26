Imports System

'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.text


	''' <summary>
	''' An implementation of
	''' <code>JFormattedTextField.AbstractFormatterFactory</code>.
	''' <code>DefaultFormatterFactory</code> allows specifying a number of
	''' different <code>JFormattedTextField.AbstractFormatter</code>s that are to
	''' be used.
	''' The most important one is the default one
	''' (<code>setDefaultFormatter</code>). The default formatter will be used
	''' if a more specific formatter could not be found. The following process
	''' is used to determine the appropriate formatter to use.
	''' <ol>
	'''   <li>Is the passed in value null? Use the null formatter.
	'''   <li>Does the <code>JFormattedTextField</code> have focus? Use the edit
	'''       formatter.
	'''   <li>Otherwise, use the display formatter.
	'''   <li>If a non-null <code>AbstractFormatter</code> has not been found, use
	'''       the default formatter.
	''' </ol>
	''' <p>
	''' The following code shows how to configure a
	''' <code>JFormattedTextField</code> with two
	''' <code>JFormattedTextField.AbstractFormatter</code>s, one for display and
	''' one for editing.
	''' <pre>
	''' JFormattedTextField.AbstractFormatter editFormatter = ...;
	''' JFormattedTextField.AbstractFormatter displayFormatter = ...;
	''' DefaultFormatterFactory factory = new DefaultFormatterFactory(
	'''                 displayFormatter, displayFormatter, editFormatter);
	''' JFormattedTextField tf = new JFormattedTextField(factory);
	''' </pre>
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= javax.swing.JFormattedTextField
	''' 
	''' @since 1.4 </seealso>
	<Serializable> _
	Public Class DefaultFormatterFactory
		Inherits javax.swing.JFormattedTextField.AbstractFormatterFactory

		''' <summary>
		''' Default <code>AbstractFormatter</code> to use if a more specific one has
		''' not been specified.
		''' </summary>
		Private defaultFormat As javax.swing.JFormattedTextField.AbstractFormatter

		''' <summary>
		''' <code>JFormattedTextField.AbstractFormatter</code> to use for display.
		''' </summary>
		Private displayFormat As javax.swing.JFormattedTextField.AbstractFormatter

		''' <summary>
		''' <code>JFormattedTextField.AbstractFormatter</code> to use for editing.
		''' </summary>
		Private editFormat As javax.swing.JFormattedTextField.AbstractFormatter

		''' <summary>
		''' <code>JFormattedTextField.AbstractFormatter</code> to use if the value
		''' is null.
		''' </summary>
		Private nullFormat As javax.swing.JFormattedTextField.AbstractFormatter


		Public Sub New()
		End Sub

		''' <summary>
		''' Creates a <code>DefaultFormatterFactory</code> with the specified
		''' <code>JFormattedTextField.AbstractFormatter</code>.
		''' </summary>
		''' <param name="defaultFormat"> JFormattedTextField.AbstractFormatter to be used
		'''                      if a more specific
		'''                      JFormattedTextField.AbstractFormatter can not be
		'''                      found. </param>
		Public Sub New(ByVal defaultFormat As javax.swing.JFormattedTextField.AbstractFormatter)
			Me.New(defaultFormat, Nothing)
		End Sub

		''' <summary>
		''' Creates a <code>DefaultFormatterFactory</code> with the specified
		''' <code>JFormattedTextField.AbstractFormatter</code>s.
		''' </summary>
		''' <param name="defaultFormat"> JFormattedTextField.AbstractFormatter to be used
		'''                      if a more specific
		'''                      JFormattedTextField.AbstractFormatter can not be
		'''                      found. </param>
		''' <param name="displayFormat"> JFormattedTextField.AbstractFormatter to be used
		'''                      when the JFormattedTextField does not have focus. </param>
		Public Sub New(ByVal defaultFormat As javax.swing.JFormattedTextField.AbstractFormatter, ByVal displayFormat As javax.swing.JFormattedTextField.AbstractFormatter)
			Me.New(defaultFormat, displayFormat, Nothing)
		End Sub

		''' <summary>
		''' Creates a DefaultFormatterFactory with the specified
		''' JFormattedTextField.AbstractFormatters.
		''' </summary>
		''' <param name="defaultFormat"> JFormattedTextField.AbstractFormatter to be used
		'''                      if a more specific
		'''                      JFormattedTextField.AbstractFormatter can not be
		'''                      found. </param>
		''' <param name="displayFormat"> JFormattedTextField.AbstractFormatter to be used
		'''                      when the JFormattedTextField does not have focus. </param>
		''' <param name="editFormat">    JFormattedTextField.AbstractFormatter to be used
		'''                      when the JFormattedTextField has focus. </param>
		Public Sub New(ByVal defaultFormat As javax.swing.JFormattedTextField.AbstractFormatter, ByVal displayFormat As javax.swing.JFormattedTextField.AbstractFormatter, ByVal editFormat As javax.swing.JFormattedTextField.AbstractFormatter)
			Me.New(defaultFormat, displayFormat, editFormat, Nothing)
		End Sub

		''' <summary>
		''' Creates a DefaultFormatterFactory with the specified
		''' JFormattedTextField.AbstractFormatters.
		''' </summary>
		''' <param name="defaultFormat"> JFormattedTextField.AbstractFormatter to be used
		'''                      if a more specific
		'''                      JFormattedTextField.AbstractFormatter can not be
		'''                      found. </param>
		''' <param name="displayFormat"> JFormattedTextField.AbstractFormatter to be used
		'''                      when the JFormattedTextField does not have focus. </param>
		''' <param name="editFormat">    JFormattedTextField.AbstractFormatter to be used
		'''                      when the JFormattedTextField has focus. </param>
		''' <param name="nullFormat">    JFormattedTextField.AbstractFormatter to be used
		'''                      when the JFormattedTextField has a null value. </param>
		Public Sub New(ByVal defaultFormat As javax.swing.JFormattedTextField.AbstractFormatter, ByVal displayFormat As javax.swing.JFormattedTextField.AbstractFormatter, ByVal editFormat As javax.swing.JFormattedTextField.AbstractFormatter, ByVal nullFormat As javax.swing.JFormattedTextField.AbstractFormatter)
			Me.defaultFormat = defaultFormat
			Me.displayFormat = displayFormat
			Me.editFormat = editFormat
			Me.nullFormat = nullFormat
		End Sub

		''' <summary>
		''' Sets the <code>JFormattedTextField.AbstractFormatter</code> to use as
		''' a last resort, eg in case a display, edit or null
		''' <code>JFormattedTextField.AbstractFormatter</code> has not been
		''' specified.
		''' </summary>
		''' <param name="atf"> JFormattedTextField.AbstractFormatter used if a more
		'''            specific is not specified </param>
		Public Overridable Property defaultFormatter As javax.swing.JFormattedTextField.AbstractFormatter
			Set(ByVal atf As javax.swing.JFormattedTextField.AbstractFormatter)
				defaultFormat = atf
			End Set
			Get
				Return defaultFormat
			End Get
		End Property


		''' <summary>
		''' Sets the <code>JFormattedTextField.AbstractFormatter</code> to use if
		''' the <code>JFormattedTextField</code> is not being edited and either
		''' the value is not-null, or the value is null and a null formatter has
		''' has not been specified.
		''' </summary>
		''' <param name="atf"> JFormattedTextField.AbstractFormatter to use when the
		'''            JFormattedTextField does not have focus </param>
		Public Overridable Property displayFormatter As javax.swing.JFormattedTextField.AbstractFormatter
			Set(ByVal atf As javax.swing.JFormattedTextField.AbstractFormatter)
				displayFormat = atf
			End Set
			Get
				Return displayFormat
			End Get
		End Property


		''' <summary>
		''' Sets the <code>JFormattedTextField.AbstractFormatter</code> to use if
		''' the <code>JFormattedTextField</code> is being edited and either
		''' the value is not-null, or the value is null and a null formatter has
		''' has not been specified.
		''' </summary>
		''' <param name="atf"> JFormattedTextField.AbstractFormatter to use when the
		'''            component has focus </param>
		Public Overridable Property editFormatter As javax.swing.JFormattedTextField.AbstractFormatter
			Set(ByVal atf As javax.swing.JFormattedTextField.AbstractFormatter)
				editFormat = atf
			End Set
			Get
				Return editFormat
			End Get
		End Property


		''' <summary>
		''' Sets the formatter to use if the value of the JFormattedTextField is
		''' null.
		''' </summary>
		''' <param name="atf"> JFormattedTextField.AbstractFormatter to use when
		''' the value of the JFormattedTextField is null. </param>
		Public Overridable Property nullFormatter As javax.swing.JFormattedTextField.AbstractFormatter
			Set(ByVal atf As javax.swing.JFormattedTextField.AbstractFormatter)
				nullFormat = atf
			End Set
			Get
				Return nullFormat
			End Get
		End Property


		''' <summary>
		''' Returns either the default formatter, display formatter, editor
		''' formatter or null formatter based on the state of the
		''' JFormattedTextField.
		''' </summary>
		''' <param name="source"> JFormattedTextField requesting
		'''               JFormattedTextField.AbstractFormatter </param>
		''' <returns> JFormattedTextField.AbstractFormatter to handle
		'''         formatting duties. </returns>
		Public Overridable Function getFormatter(ByVal source As javax.swing.JFormattedTextField) As javax.swing.JFormattedTextField.AbstractFormatter
			Dim format As javax.swing.JFormattedTextField.AbstractFormatter = Nothing

			If source Is Nothing Then Return Nothing
			Dim value As Object = source.value

			If value Is Nothing Then format = nullFormatter
			If format Is Nothing Then
				If source.hasFocus() Then
					format = editFormatter
				Else
					format = displayFormatter
				End If
				If format Is Nothing Then format = defaultFormatter
			End If
			Return format
		End Function
	End Class

End Namespace