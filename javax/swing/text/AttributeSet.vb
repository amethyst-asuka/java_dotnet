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
Namespace javax.swing.text


	''' <summary>
	''' A collection of unique attributes.  This is a read-only,
	''' immutable interface.  An attribute is basically a key and
	''' a value assigned to the key.  The collection may represent
	''' something like a style run, a logical style, etc.  These
	''' are generally used to describe features that will contribute
	''' to some graphical representation such as a font.  The
	''' set of possible keys is unbounded and can be anything.
	''' Typically View implementations will respond to attribute
	''' definitions and render something to represent the attributes.
	''' <p>
	''' Attributes can potentially resolve in a hierarchy.  If a
	''' key doesn't resolve locally, and a resolving parent
	''' exists, the key will be resolved through the parent.
	''' 
	''' @author  Timothy Prinzing </summary>
	''' <seealso cref= MutableAttributeSet </seealso>
	Public Interface AttributeSet

		''' <summary>
		''' This interface is the type signature that is expected
		''' to be present on any attribute key that contributes to
		''' the determination of what font to use to render some
		''' text.  This is not considered to be a closed set, the
		''' definition can change across version of the platform and can
		''' be amended by additional user added entries that
		''' correspond to logical settings that are specific to
		''' some type of content.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface FontAttribute
	'	{
	'	}

		''' <summary>
		''' This interface is the type signature that is expected
		''' to be present on any attribute key that contributes to
		''' presentation of color.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface ColorAttribute
	'	{
	'	}

		''' <summary>
		''' This interface is the type signature that is expected
		''' to be present on any attribute key that contributes to
		''' character level presentation.  This would be any attribute
		''' that applies to a so-called <i>run</i> of
		''' style.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface CharacterAttribute
	'	{
	'	}

		''' <summary>
		''' This interface is the type signature that is expected
		''' to be present on any attribute key that contributes to
		''' the paragraph level presentation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interface inner types are not converted:
'		public interface ParagraphAttribute
	'	{
	'	}

		''' <summary>
		''' Returns the number of attributes that are defined locally in this set.
		''' Attributes that are defined in the parent set are not included.
		''' </summary>
		''' <returns> the number of attributes &gt;= 0 </returns>
		ReadOnly Property attributeCount As Integer

		''' <summary>
		''' Checks whether the named attribute has a value specified in
		''' the set without resolving through another attribute
		''' set.
		''' </summary>
		''' <param name="attrName"> the attribute name </param>
		''' <returns> true if the attribute has a value specified </returns>
		Function isDefined(ByVal attrName As Object) As Boolean

		''' <summary>
		''' Determines if the two attribute sets are equivalent.
		''' </summary>
		''' <param name="attr"> an attribute set </param>
		''' <returns> true if the sets are equivalent </returns>
		Function isEqual(ByVal attr As AttributeSet) As Boolean

		''' <summary>
		''' Returns an attribute set that is guaranteed not
		''' to change over time.
		''' </summary>
		''' <returns> a copy of the attribute set </returns>
		Function copyAttributes() As AttributeSet

		''' <summary>
		''' Fetches the value of the given attribute. If the value is not found
		''' locally, the search is continued upward through the resolving
		''' parent (if one exists) until the value is either
		''' found or there are no more parents.  If the value is not found,
		''' null is returned.
		''' </summary>
		''' <param name="key"> the non-null key of the attribute binding </param>
		''' <returns> the value of the attribute, or {@code null} if not found </returns>
		Function getAttribute(ByVal key As Object) As Object

		''' <summary>
		''' Returns an enumeration over the names of the attributes that are
		''' defined locally in the set. Names of attributes defined in the
		''' resolving parent, if any, are not included. The values of the
		''' <code>Enumeration</code> may be anything and are not constrained to
		''' a particular <code>Object</code> type.
		''' <p>
		''' This method never returns {@code null}. For a set with no attributes, it
		''' returns an empty {@code Enumeration}.
		''' </summary>
		''' <returns> the names </returns>
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		ReadOnly Property attributeNames As System.Collections.IEnumerator(Of ?)

		''' <summary>
		''' Returns {@code true} if this set defines an attribute with the same
		''' name and an equal value. If such an attribute is not found locally,
		''' it is searched through in the resolving parent hierarchy.
		''' </summary>
		''' <param name="name"> the non-null attribute name </param>
		''' <param name="value"> the value </param>
		''' <returns> {@code true} if the set defines the attribute with an
		'''     equal value, either locally or through its resolving parent </returns>
		''' <exception cref="NullPointerException"> if either {@code name} or
		'''      {@code value} is {@code null} </exception>
		Function containsAttribute(ByVal name As Object, ByVal value As Object) As Boolean

		''' <summary>
		''' Returns {@code true} if this set defines all the attributes from the
		''' given set with equal values. If an attribute is not found locally,
		''' it is searched through in the resolving parent hierarchy.
		''' </summary>
		''' <param name="attributes"> the set of attributes to check against </param>
		''' <returns> {@code true} if this set defines all the attributes with equal
		'''              values, either locally or through its resolving parent </returns>
		''' <exception cref="NullPointerException"> if {@code attributes} is {@code null} </exception>
		Function containsAttributes(ByVal attributes As AttributeSet) As Boolean

		''' <summary>
		''' Gets the resolving parent.
		''' </summary>
		''' <returns> the parent </returns>
		ReadOnly Property resolveParent As AttributeSet

		''' <summary>
		''' Attribute name used to name the collection of
		''' attributes.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Object NameAttribute = StyleConstants.NameAttribute;

		''' <summary>
		''' Attribute name used to identify the resolving parent
		''' set of attributes, if one is defined.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final Object ResolveAttribute = StyleConstants.ResolveAttribute;

	End Interface

End Namespace