Imports System.Collections.Generic

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.text


	''' <summary>
	''' An {@code AttributedCharacterIterator} allows iteration through both text and
	''' related attribute information.
	''' 
	''' <p>
	''' An attribute is a key/value pair, identified by the key.  No two
	''' attributes on a given character can have the same key.
	''' 
	''' <p>The values for an attribute are immutable, or must not be mutated
	''' by clients or storage.  They are always passed by reference, and not
	''' cloned.
	''' 
	''' <p>A <em>run with respect to an attribute</em> is a maximum text range for
	''' which:
	''' <ul>
	''' <li>the attribute is undefined or {@code null} for the entire range, or
	''' <li>the attribute value is defined and has the same non-{@code null} value for the
	'''     entire range.
	''' </ul>
	''' 
	''' <p>A <em>run with respect to a set of attributes</em> is a maximum text range for
	''' which this condition is met for each member attribute.
	''' 
	''' <p>When getting a run with no explicit attributes specified (i.e.,
	''' calling <seealso cref="#getRunStart()"/> and <seealso cref="#getRunLimit()"/>), any
	''' contiguous text segments having the same attributes (the same set
	''' of attribute/value pairs) are treated as separate runs if the
	''' attributes have been given to those text segments separately.
	''' 
	''' <p>The returned indexes are limited to the range of the iterator.
	''' 
	''' <p>The returned attribute information is limited to runs that contain
	''' the current character.
	''' 
	''' <p>
	''' Attribute keys are instances of <seealso cref="AttributedCharacterIterator.Attribute"/> and its
	''' subclasses, such as <seealso cref="java.awt.font.TextAttribute"/>.
	''' </summary>
	''' <seealso cref= AttributedCharacterIterator.Attribute </seealso>
	''' <seealso cref= java.awt.font.TextAttribute </seealso>
	''' <seealso cref= AttributedString </seealso>
	''' <seealso cref= Annotation
	''' @since 1.2 </seealso>

	Public Interface AttributedCharacterIterator
		Inherits CharacterIterator

		''' <summary>
		''' Defines attribute keys that are used to identify text attributes. These
		''' keys are used in {@code AttributedCharacterIterator} and {@code AttributedString}. </summary>
		''' <seealso cref= AttributedCharacterIterator </seealso>
		''' <seealso cref= AttributedString
		''' @since 1.2 </seealso>

'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public static class Attribute implements java.io.Serializable
	'	{
	'
	'		''' <summary>
	'		''' The name of this {@code Attribute}. The name is used primarily by {@code readResolve}
	'		''' to look up the corresponding predefined instance when deserializing
	'		''' an instance.
	'		''' @serial
	'		''' </summary>
	'		private String name;
	'
	'		' table of all instances in this [Class], used by readResolve
	'		private static final Map<String, Attribute> instanceMap = New HashMap<>(7);
	'
	'		''' <summary>
	'		''' Constructs an {@code Attribute} with the given name.
	'		''' </summary>
	'		''' <param name="name"> the name of {@code Attribute} </param>
	'		protected Attribute(String name)
	'		{
	'			Me.name = name;
	'			if (Me.getClass() == Attribute.class)
	'			{
	'				instanceMap.put(name, Me);
	'			}
	'		}
	'
	'		''' <summary>
	'		''' Compares two objects for equality. This version only returns true
	'		''' for {@code x.equals(y)} if {@code x} and {@code y} refer
	'		''' to the same object, and guarantees this for all subclasses.
	'		''' </summary>
	'		public final boolean equals(Object obj)
	'		{
	'			Return MyBase.equals(obj);
	'		}
	'
	'		''' <summary>
	'		''' Returns a hash code value for the object. This version is identical to
	'		''' the one in {@code Object}, but is also final.
	'		''' </summary>
	'		public final int hashCode()
	'		{
	'			Return MyBase.hashCode();
	'		}
	'
	'		''' <summary>
	'		''' Returns a string representation of the object. This version returns the
	'		''' concatenation of class name, {@code "("}, a name identifying the attribute
	'		''' and {@code ")"}.
	'		''' </summary>
	'		public String toString()
	'		{
	'			Return getClass().getName() + "(" + name + ")";
	'		}
	'
	'		''' <summary>
	'		''' Returns the name of the attribute.
	'		''' </summary>
	'		''' <returns> the name of {@code Attribute} </returns>
	'		protected String getName()
	'		{
	'			Return name;
	'		}
	'
	'		''' <summary>
	'		''' Resolves instances being deserialized to the predefined constants.
	'		''' </summary>
	'		''' <returns> the resolved {@code Attribute} object </returns>
	'		''' <exception cref="InvalidObjectException"> if the object to resolve is not
	'		'''                                an instance of {@code Attribute} </exception>
	'		protected Object readResolve() throws InvalidObjectException
	'		{
	'			if (Me.getClass() != Attribute.class)
	'			{
	'				throw New InvalidObjectException("subclass didn't correctly implement readResolve");
	'			}
	'
	'			Attribute instance = instanceMap.get(getName());
	'			if (instance != Nothing)
	'			{
	'				Return instance;
	'			}
	'			else
	'			{
	'				throw New InvalidObjectException("unknown attribute name");
	'			}
	'		}
	'
	'		''' <summary>
	'		''' Attribute key for the language of some text.
	'		''' <p> Values are instances of <seealso cref="java.util.Locale Locale"/>. </summary>
	'		''' <seealso cref= java.util.Locale </seealso>
	'		public static final Attribute LANGUAGE = New Attribute("language");
	'
	'		''' <summary>
	'		''' Attribute key for the reading of some text. In languages where the written form
	'		''' and the pronunciation of a word are only loosely related (such as Japanese),
	'		''' it is often necessary to store the reading (pronunciation) along with the
	'		''' written form.
	'		''' <p>Values are instances of <seealso cref="Annotation"/> holding instances of <seealso cref="String"/>.
	'		''' </summary>
	'		''' <seealso cref= Annotation </seealso>
	'		''' <seealso cref= java.lang.String </seealso>
	'		public static final Attribute READING = New Attribute("reading");
	'
	'		''' <summary>
	'		''' Attribute key for input method segments. Input methods often break
	'		''' up text into segments, which usually correspond to words.
	'		''' <p>Values are instances of <seealso cref="Annotation"/> holding a {@code null} reference. </summary>
	'		''' <seealso cref= Annotation </seealso>
	'		public static final Attribute INPUT_METHOD_SEGMENT = New Attribute("input_method_segment");
	'
	'		' make sure the serial version doesn't change between compiler versions
	'		private static final long serialVersionUID = -9142742483513960612L;
	'
	'	};

		''' <summary>
		''' Returns the index of the first character of the run
		''' with respect to all attributes containing the current character.
		''' 
		''' <p>Any contiguous text segments having the same attributes (the
		''' same set of attribute/value pairs) are treated as separate runs
		''' if the attributes have been given to those text segments separately.
		''' </summary>
		''' <returns> the index of the first character of the run </returns>
		ReadOnly Property runStart As Integer

		''' <summary>
		''' Returns the index of the first character of the run
		''' with respect to the given {@code attribute} containing the current character.
		''' </summary>
		''' <param name="attribute"> the desired attribute. </param>
		''' <returns> the index of the first character of the run </returns>
		Function getRunStart(ByVal attribute As Attribute) As Integer

		''' <summary>
		''' Returns the index of the first character of the run
		''' with respect to the given {@code attributes} containing the current character.
		''' </summary>
		''' <param name="attributes"> a set of the desired attributes. </param>
		''' <returns> the index of the first character of the run </returns>
		Function getRunStart(Of T1 As Attribute)(ByVal attributes As java.util.Set(Of T1)) As Integer

		''' <summary>
		''' Returns the index of the first character following the run
		''' with respect to all attributes containing the current character.
		''' 
		''' <p>Any contiguous text segments having the same attributes (the
		''' same set of attribute/value pairs) are treated as separate runs
		''' if the attributes have been given to those text segments separately.
		''' </summary>
		''' <returns> the index of the first character following the run </returns>
		ReadOnly Property runLimit As Integer

		''' <summary>
		''' Returns the index of the first character following the run
		''' with respect to the given {@code attribute} containing the current character.
		''' </summary>
		''' <param name="attribute"> the desired attribute </param>
		''' <returns> the index of the first character following the run </returns>
		Function getRunLimit(ByVal attribute As Attribute) As Integer

		''' <summary>
		''' Returns the index of the first character following the run
		''' with respect to the given {@code attributes} containing the current character.
		''' </summary>
		''' <param name="attributes"> a set of the desired attributes </param>
		''' <returns> the index of the first character following the run </returns>
		Function getRunLimit(Of T1 As Attribute)(ByVal attributes As java.util.Set(Of T1)) As Integer

		''' <summary>
		''' Returns a map with the attributes defined on the current
		''' character.
		''' </summary>
		''' <returns> a map with the attributes defined on the current character </returns>
		ReadOnly Property attributes As IDictionary(Of Attribute, Object)

		''' <summary>
		''' Returns the value of the named {@code attribute} for the current character.
		''' Returns {@code null} if the {@code attribute} is not defined.
		''' </summary>
		''' <param name="attribute"> the desired attribute </param>
		''' <returns> the value of the named {@code attribute} or {@code null} </returns>
		Function getAttribute(ByVal attribute As Attribute) As Object

		''' <summary>
		''' Returns the keys of all attributes defined on the
		''' iterator's text range. The set is empty if no
		''' attributes are defined.
		''' </summary>
		''' <returns> the keys of all attributes </returns>
		ReadOnly Property allAttributeKeys As java.util.Set(Of Attribute)
	End Interface

End Namespace