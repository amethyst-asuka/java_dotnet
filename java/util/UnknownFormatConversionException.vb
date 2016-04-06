'
' * Copyright (c) 2003, 2005, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.util

	''' <summary>
	''' Unchecked exception thrown when an unknown conversion is given.
	''' 
	''' <p> Unless otherwise specified, passing a <tt>null</tt> argument to
	''' any method or constructor in this class will cause a {@link
	''' NullPointerException} to be thrown.
	''' 
	''' @since 1.5
	''' </summary>
	Public Class UnknownFormatConversionException
		Inherits IllegalFormatException

		Private Shadows Const serialVersionUID As Long = 19060418L

		Private s As String

		''' <summary>
		''' Constructs an instance of this class with the unknown conversion.
		''' </summary>
		''' <param name="s">
		'''         Unknown conversion </param>
		Public Sub New(  s As String)
			If s Is Nothing Then Throw New NullPointerException
			Me.s = s
		End Sub

		''' <summary>
		''' Returns the unknown conversion.
		''' </summary>
		''' <returns>  The unknown conversion. </returns>
		Public Overridable Property conversion As String
			Get
				Return s
			End Get
		End Property

		' javadoc inherited from Throwable.java
		Public  Overrides ReadOnly Property  message As String
			Get
				Return String.Format("Conversion = '{0}'", s)
			End Get
		End Property
	End Class

End Namespace