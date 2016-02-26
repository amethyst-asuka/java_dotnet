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
	''' Class URISyntax is an abstract base class providing the common
	''' implementation of all attributes whose value is a Uniform Resource
	''' Identifier (URI). Once constructed, a URI attribute's value is immutable.
	''' <P>
	''' 
	''' @author  Alan Kaminsky
	''' </summary>
	<Serializable> _
	Public MustInherit Class URISyntax
		Implements ICloneable

		Private Const serialVersionUID As Long = -7842661210486401678L

		''' <summary>
		''' URI value of this URI attribute.
		''' @serial
		''' </summary>
		Private uri As java.net.URI

		''' <summary>
		''' Constructs a URI attribute with the specified URI.
		''' </summary>
		''' <param name="uri">  URI.
		''' </param>
		''' <exception cref="NullPointerException">
		'''     (unchecked exception) Thrown if <CODE>uri</CODE> is null. </exception>
		Protected Friend Sub New(ByVal uri As java.net.URI)
			Me.uri = verify(uri)
		End Sub

		Private Shared Function verify(ByVal uri As java.net.URI) As java.net.URI
			If uri Is Nothing Then Throw New NullPointerException(" uri is null")
			Return uri
		End Function

		''' <summary>
		''' Returns this URI attribute's URI value. </summary>
		''' <returns> the URI. </returns>
		Public Overridable Property uRI As java.net.URI
			Get
				Return uri
			End Get
		End Property

		''' <summary>
		''' Returns a hashcode for this URI attribute.
		''' </summary>
		''' <returns>  A hashcode value for this object. </returns>
		Public Overrides Function GetHashCode() As Integer
			Return uri.GetHashCode()
		End Function

		''' <summary>
		''' Returns whether this URI attribute is equivalent to the passed in
		''' object.
		''' To be equivalent, all of the following conditions must be true:
		''' <OL TYPE=1>
		''' <LI>
		''' <CODE>object</CODE> is not null.
		''' <LI>
		''' <CODE>object</CODE> is an instance of class URISyntax.
		''' <LI>
		''' This URI attribute's underlying URI and <CODE>object</CODE>'s
		''' underlying URI are equal.
		''' </OL>
		''' </summary>
		''' <param name="object">  Object to compare to.
		''' </param>
		''' <returns>  True if <CODE>object</CODE> is equivalent to this URI
		'''          attribute, false otherwise. </returns>
		Public Overrides Function Equals(ByVal [object] As Object) As Boolean
			Return ([object] IsNot Nothing AndAlso TypeOf [object] Is URISyntax AndAlso Me.uri.Equals(CType([object], URISyntax).uri))
		End Function

		''' <summary>
		''' Returns a String identifying this URI attribute. The String is the
		''' string representation of the attribute's underlying URI.
		''' </summary>
		''' <returns>  A String identifying this object. </returns>
		Public Overrides Function ToString() As String
			Return uri.ToString()
		End Function

	End Class

End Namespace