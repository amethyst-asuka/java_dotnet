Imports System

'
' * Copyright (c) 2000, 2004, Oracle and/or its affiliates. All rights reserved.
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


Namespace javax.print.attribute


	''' <summary>
	''' Class TextSyntax is an abstract base class providing the common
	''' implementation of all attributes whose value is a string. The text attribute
	''' includes a locale to indicate the natural language. Thus, a text attribute
	''' always represents a localized string. Once constructed, a text attribute's
	''' value is immutable.
	''' <P>
	''' 
	''' @author  David Mendenhall
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class TextSyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = -8130648736378144102L

		''' <summary>
		''' String value of this text attribute.
		''' @serial
		''' </summary>
		Private value As String

		''' <summary>
		''' Locale of this text attribute.
		''' @serial
		''' </summary>
		Private locale As java.util.Locale

		''' <summary>
		''' Constructs a TextAttribute with the specified string and locale.
		''' </summary>
		''' <param name="value">   Text string. </param>
		''' <param name="locale">  Natural language of the text string. null
		''' is interpreted to mean the default locale for as returned
		''' by <code>Locale.getDefault()</code>
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>value</CODE> is null. </exception>
		Protected Friend Sub New(ByVal value As String, ByVal locale As java.util.Locale)
			Me.value = verify(value)
			Me.locale = verify(locale)
		End Sub

		Private Shared Function verify(ByVal value As String) As String
			If value Is Nothing Then Throw New NullPointerException(" value is null")
			Return value
		End Function

		Private Shared Function verify(ByVal locale As java.util.Locale) As java.util.Locale
			If locale Is Nothing Then Return java.util.Locale.default
			Return locale
		End Function

		''' <summary>
		''' Returns this text attribute's text string. </summary>
		''' <returns> the text string. </returns>
		Public Overridable Property value As String
			Get
				Return value
			End Get
		End Property

		''' <summary>
		''' Returns this text attribute's text string's natural language (locale). </summary>
		''' <returns> the locale </returns>
		Public Overridable Property locale As java.util.Locale
			Get
				Return locale
			End Get
		End Property

		''' <summary>
		''' Returns a hashcode for this text attribute.
		''' </summary>
		''' <returns>  A hashcode value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return value.GetHashCode() Xor locale.GetHashCode()
		End Function

		''' <summary>
		''' Returns whether this text attribute is equivalent to the passed in
		''' object. To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class TextSyntax.
		''' <LI>
		''' This text attribute's underlying string and <CODE>object</CODE>'s
		''' underlying string are equal.
		''' <LI>
		''' This text attribute's locale and <CODE>object</CODE>'s locale are
		''' equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this text
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return ([object] IsNot Nothing AndAlso TypeOf [object] Is TextSyntax AndAlso Me.value.Equals(CType([object], TextSyntax).value) AndAlso Me.locale.Equals(CType([object], TextSyntax).locale))
		End Function

		''' <summary>
		''' Returns a String identifying this text attribute. The String is
		''' the attribute's underlying text string.
		''' </summary>
		''' <returns>  A String identifying this object. </returns>
		Public Overrides Function ToString() As String
			Return value
		End Function

	End Class

End Namespace