Imports System

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

'
' * (C) Copyright Taligent, Inc. 1996, 1997 - All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998 - All Rights Reserved
' *
' *   The original version of this source code and documentation is copyrighted
' * and owned by Taligent, Inc., a wholly-owned subsidiary of IBM. These
' * materials are provided under terms of a License Agreement between Taligent
' * and Sun. This technology is protected by multiple US and International
' * patents. This notice and attribution to Taligent may not be removed.
' *   Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.text


	''' <summary>
	''' <code>Format</code> is an abstract base class for formatting locale-sensitive
	''' information such as dates, messages, and numbers.
	''' 
	''' <p>
	''' <code>Format</code> defines the programming interface for formatting
	''' locale-sensitive objects into <code>String</code>s (the
	''' <code>format</code> method) and for parsing <code>String</code>s back
	''' into objects (the <code>parseObject</code> method).
	''' 
	''' <p>
	''' Generally, a format's <code>parseObject</code> method must be able to parse
	''' any string formatted by its <code>format</code> method. However, there may
	''' be exceptional cases where this is not possible. For example, a
	''' <code>format</code> method might create two adjacent integer numbers with
	''' no separator in between, and in this case the <code>parseObject</code> could
	''' not tell which digits belong to which number.
	''' 
	''' <h3>Subclassing</h3>
	''' 
	''' <p>
	''' The Java Platform provides three specialized subclasses of <code>Format</code>--
	''' <code>DateFormat</code>, <code>MessageFormat</code>, and
	''' <code>NumberFormat</code>--for formatting dates, messages, and numbers,
	''' respectively.
	''' <p>
	''' Concrete subclasses must implement three methods:
	''' <ol>
	''' <li> <code>format(Object obj, StringBuffer toAppendTo, FieldPosition pos)</code>
	''' <li> <code>formatToCharacterIterator(Object obj)</code>
	''' <li> <code>parseObject(String source, ParsePosition pos)</code>
	''' </ol>
	''' These general methods allow polymorphic parsing and formatting of objects
	''' and are used, for example, by <code>MessageFormat</code>.
	''' Subclasses often also provide additional <code>format</code> methods for
	''' specific input types as well as <code>parse</code> methods for specific
	''' result types. Any <code>parse</code> method that does not take a
	''' <code>ParsePosition</code> argument should throw <code>ParseException</code>
	''' when no text in the required format is at the beginning of the input text.
	''' 
	''' <p>
	''' Most subclasses will also implement the following factory methods:
	''' <ol>
	''' <li>
	''' <code>getInstance</code> for getting a useful format object appropriate
	''' for the current locale
	''' <li>
	''' <code>getInstance(Locale)</code> for getting a useful format
	''' object appropriate for the specified locale
	''' </ol>
	''' In addition, some subclasses may also implement other
	''' <code>getXxxxInstance</code> methods for more specialized control. For
	''' example, the <code>NumberFormat</code> class provides
	''' <code>getPercentInstance</code> and <code>getCurrencyInstance</code>
	''' methods for getting specialized number formatters.
	''' 
	''' <p>
	''' Subclasses of <code>Format</code> that allow programmers to create objects
	''' for locales (with <code>getInstance(Locale)</code> for example)
	''' must also implement the following class method:
	''' <blockquote>
	''' <pre>
	''' Public Shared Locale[] getAvailableLocales()
	''' </pre>
	''' </blockquote>
	''' 
	''' <p>
	''' And finally subclasses may define a set of constants to identify the various
	''' fields in the formatted output. These constants are used to create a FieldPosition
	''' object which identifies what information is contained in the field and its
	''' position in the formatted result. These constants should be named
	''' <code><em>item</em>_FIELD</code> where <code><em>item</em></code> identifies
	''' the field. For examples of these constants, see <code>ERA_FIELD</code> and its
	''' friends in <seealso cref="DateFormat"/>.
	''' 
	''' <h4><a name="synchronization">Synchronization</a></h4>
	''' 
	''' <p>
	''' Formats are generally not synchronized.
	''' It is recommended to create separate format instances for each thread.
	''' If multiple threads access a format concurrently, it must be synchronized
	''' externally.
	''' </summary>
	''' <seealso cref=          java.text.ParsePosition </seealso>
	''' <seealso cref=          java.text.FieldPosition </seealso>
	''' <seealso cref=          java.text.NumberFormat </seealso>
	''' <seealso cref=          java.text.DateFormat </seealso>
	''' <seealso cref=          java.text.MessageFormat
	''' @author       Mark Davis </seealso>
	<Serializable> _
	Public MustInherit Class Format
		Implements Cloneable

		Private Const serialVersionUID As Long = -299282585814624189L

		''' <summary>
		''' Sole constructor.  (For invocation by subclass constructors, typically
		''' implicit.)
		''' </summary>
		Protected Friend Sub New()
		End Sub

		''' <summary>
		''' Formats an object to produce a string. This is equivalent to
		''' <blockquote>
		''' <seealso cref="#format(Object, StringBuffer, FieldPosition) format"/><code>(obj,
		'''         new StringBuffer(), new FieldPosition(0)).toString();</code>
		''' </blockquote>
		''' </summary>
		''' <param name="obj">    The object to format </param>
		''' <returns>       Formatted string. </returns>
		''' <exception cref="IllegalArgumentException"> if the Format cannot format the given
		'''            object </exception>
		Public Function format(  obj As Object) As String
			Return format(obj, New StringBuffer, New FieldPosition(0)).ToString()
		End Function

		''' <summary>
		''' Formats an object and appends the resulting text to a given string
		''' buffer.
		''' If the <code>pos</code> argument identifies a field used by the format,
		''' then its indices are set to the beginning and end of the first such
		''' field encountered.
		''' </summary>
		''' <param name="obj">    The object to format </param>
		''' <param name="toAppendTo">    where the text is to be appended </param>
		''' <param name="pos">    A <code>FieldPosition</code> identifying a field
		'''               in the formatted text </param>
		''' <returns>       the string buffer passed in as <code>toAppendTo</code>,
		'''               with formatted text appended </returns>
		''' <exception cref="NullPointerException"> if <code>toAppendTo</code> or
		'''            <code>pos</code> is null </exception>
		''' <exception cref="IllegalArgumentException"> if the Format cannot format the given
		'''            object </exception>
		Public MustOverride Function format(  obj As Object,   toAppendTo As StringBuffer,   pos As FieldPosition) As StringBuffer

		''' <summary>
		''' Formats an Object producing an <code>AttributedCharacterIterator</code>.
		''' You can use the returned <code>AttributedCharacterIterator</code>
		''' to build the resulting String, as well as to determine information
		''' about the resulting String.
		''' <p>
		''' Each attribute key of the AttributedCharacterIterator will be of type
		''' <code>Field</code>. It is up to each <code>Format</code> implementation
		''' to define what the legal values are for each attribute in the
		''' <code>AttributedCharacterIterator</code>, but typically the attribute
		''' key is also used as the attribute value.
		''' <p>The default implementation creates an
		''' <code>AttributedCharacterIterator</code> with no attributes. Subclasses
		''' that support fields should override this and create an
		''' <code>AttributedCharacterIterator</code> with meaningful attributes.
		''' </summary>
		''' <exception cref="NullPointerException"> if obj is null. </exception>
		''' <exception cref="IllegalArgumentException"> when the Format cannot format the
		'''            given object. </exception>
		''' <param name="obj"> The object to format </param>
		''' <returns> AttributedCharacterIterator describing the formatted value.
		''' @since 1.4 </returns>
		Public Overridable Function formatToCharacterIterator(  obj As Object) As AttributedCharacterIterator
			Return createAttributedCharacterIterator(format(obj))
		End Function

		''' <summary>
		''' Parses text from a string to produce an object.
		''' <p>
		''' The method attempts to parse text starting at the index given by
		''' <code>pos</code>.
		''' If parsing succeeds, then the index of <code>pos</code> is updated
		''' to the index after the last character used (parsing does not necessarily
		''' use all characters up to the end of the string), and the parsed
		''' object is returned. The updated <code>pos</code> can be used to
		''' indicate the starting point for the next call to this method.
		''' If an error occurs, then the index of <code>pos</code> is not
		''' changed, the error index of <code>pos</code> is set to the index of
		''' the character where the error occurred, and null is returned.
		''' </summary>
		''' <param name="source"> A <code>String</code>, part of which should be parsed. </param>
		''' <param name="pos"> A <code>ParsePosition</code> object with index and error
		'''            index information as described above. </param>
		''' <returns> An <code>Object</code> parsed from the string. In case of
		'''         error, returns null. </returns>
		''' <exception cref="NullPointerException"> if <code>pos</code> is null. </exception>
		Public MustOverride Function parseObject(  source As String,   pos As ParsePosition) As Object

		''' <summary>
		''' Parses text from the beginning of the given string to produce an object.
		''' The method may not use the entire text of the given string.
		''' </summary>
		''' <param name="source"> A <code>String</code> whose beginning should be parsed. </param>
		''' <returns> An <code>Object</code> parsed from the string. </returns>
		''' <exception cref="ParseException"> if the beginning of the specified string
		'''            cannot be parsed. </exception>
		Public Overridable Function parseObject(  source As String) As Object
			Dim pos As New ParsePosition(0)
			Dim result As Object = parseObject(source, pos)
			If pos.index = 0 Then Throw New ParseException("Format.parseObject(String) failed", pos.errorIndex)
			Return result
		End Function

		''' <summary>
		''' Creates and returns a copy of this object.
		''' </summary>
		''' <returns> a clone of this instance. </returns>
		Public Overridable Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' will never happen
				Throw New InternalError(e)
			End Try
		End Function

		'
		' Convenience methods for creating AttributedCharacterIterators from
		' different parameters.
		'

		''' <summary>
		''' Creates an <code>AttributedCharacterIterator</code> for the String
		''' <code>s</code>.
		''' </summary>
		''' <param name="s"> String to create AttributedCharacterIterator from </param>
		''' <returns> AttributedCharacterIterator wrapping s </returns>
		Friend Overridable Function createAttributedCharacterIterator(  s As String) As AttributedCharacterIterator
			Dim [as] As New AttributedString(s)

			Return [as].iterator
		End Function

		''' <summary>
		''' Creates an <code>AttributedCharacterIterator</code> containing the
		''' concatenated contents of the passed in
		''' <code>AttributedCharacterIterator</code>s.
		''' </summary>
		''' <param name="iterators"> AttributedCharacterIterators used to create resulting
		'''                  AttributedCharacterIterators </param>
		''' <returns> AttributedCharacterIterator wrapping passed in
		'''         AttributedCharacterIterators </returns>
		Friend Overridable Function createAttributedCharacterIterator(  iterators As AttributedCharacterIterator()) As AttributedCharacterIterator
			Dim [as] As New AttributedString(iterators)

			Return [as].iterator
		End Function

		''' <summary>
		''' Returns an AttributedCharacterIterator with the String
		''' <code>string</code> and additional key/value pair <code>key</code>,
		''' <code>value</code>.
		''' </summary>
		''' <param name="string"> String to create AttributedCharacterIterator from </param>
		''' <param name="key"> Key for AttributedCharacterIterator </param>
		''' <param name="value"> Value associated with key in AttributedCharacterIterator </param>
		''' <returns> AttributedCharacterIterator wrapping args </returns>
		Friend Overridable Function createAttributedCharacterIterator(  [string] As String,   key As AttributedCharacterIterator.Attribute,   value As Object) As AttributedCharacterIterator
			Dim [as] As New AttributedString(string_Renamed)

			[as].addAttribute(key, value)
			Return [as].iterator
		End Function

		''' <summary>
		''' Creates an AttributedCharacterIterator with the contents of
		''' <code>iterator</code> and the additional attribute <code>key</code>
		''' <code>value</code>.
		''' </summary>
		''' <param name="iterator"> Initial AttributedCharacterIterator to add arg to </param>
		''' <param name="key"> Key for AttributedCharacterIterator </param>
		''' <param name="value"> Value associated with key in AttributedCharacterIterator </param>
		''' <returns> AttributedCharacterIterator wrapping args </returns>
		Friend Overridable Function createAttributedCharacterIterator(  [iterator] As AttributedCharacterIterator,   key As AttributedCharacterIterator.Attribute,   value As Object) As AttributedCharacterIterator
			Dim [as] As New AttributedString([iterator])

			[as].addAttribute(key, value)
			Return [as].iterator
		End Function


		''' <summary>
		''' Defines constants that are used as attribute keys in the
		''' <code>AttributedCharacterIterator</code> returned
		''' from <code>Format.formatToCharacterIterator</code> and as
		''' field identifiers in <code>FieldPosition</code>.
		''' 
		''' @since 1.4
		''' </summary>
		Public Class Field
			Inherits AttributedCharacterIterator.Attribute

			' Proclaim serial compatibility with 1.4 FCS
			Private Const serialVersionUID As Long = 276966692217360283L

			''' <summary>
			''' Creates a Field with the specified name.
			''' </summary>
			''' <param name="name"> Name of the attribute </param>
			Protected Friend Sub New(  name As String)
				MyBase.New(name)
			End Sub
		End Class


		''' <summary>
		''' FieldDelegate is notified by the various <code>Format</code>
		''' implementations as they are formatting the Objects. This allows for
		''' storage of the individual sections of the formatted String for
		''' later use, such as in a <code>FieldPosition</code> or for an
		''' <code>AttributedCharacterIterator</code>.
		''' <p>
		''' Delegates should NOT assume that the <code>Format</code> will notify
		''' the delegate of fields in any particular order.
		''' </summary>
		''' <seealso cref= FieldPosition#getFieldDelegate </seealso>
		''' <seealso cref= CharacterIteratorFieldDelegate </seealso>
		Friend Interface FieldDelegate
			''' <summary>
			''' Notified when a particular region of the String is formatted. This
			''' method will be invoked if there is no corresponding integer field id
			''' matching <code>attr</code>.
			''' </summary>
			''' <param name="attr"> Identifies the field matched </param>
			''' <param name="value"> Value associated with the field </param>
			''' <param name="start"> Beginning location of the field, will be >= 0 </param>
			''' <param name="end"> End of the field, will be >= start and <= buffer.length() </param>
			''' <param name="buffer"> Contains current formatted value, receiver should
			'''        NOT modify it. </param>
			Sub formatted(  attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer)

			''' <summary>
			''' Notified when a particular region of the String is formatted.
			''' </summary>
			''' <param name="fieldID"> Identifies the field by integer </param>
			''' <param name="attr"> Identifies the field matched </param>
			''' <param name="value"> Value associated with the field </param>
			''' <param name="start"> Beginning location of the field, will be >= 0 </param>
			''' <param name="end"> End of the field, will be >= start and <= buffer.length() </param>
			''' <param name="buffer"> Contains current formatted value, receiver should
			'''        NOT modify it. </param>
			Sub formatted(  fieldID As Integer,   attr As Format.Field,   value As Object,   start As Integer,   [end] As Integer,   buffer As StringBuffer)
		End Interface
	End Class

End Namespace