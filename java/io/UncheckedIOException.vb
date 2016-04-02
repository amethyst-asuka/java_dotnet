'
' * Copyright (c) 2012, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace java.io


	''' <summary>
	''' Wraps an <seealso cref="IOException"/> with an unchecked exception.
	''' 
	''' @since   1.8
	''' </summary>
	Public Class UncheckedIOException
		Inherits RuntimeException

		Private Shadows Const serialVersionUID As Long = -8134305061645241065L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="message">
		'''          the detail message, can be null </param>
		''' <param name="cause">
		'''          the {@code IOException}
		''' </param>
		''' <exception cref="NullPointerException">
		'''          if the cause is {@code null} </exception>
		Public Sub New(ByVal message As String, ByVal cause As IOException)
			MyBase.New(message, java.util.Objects.requireNonNull(cause))
		End Sub

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="cause">
		'''          the {@code IOException}
		''' </param>
		''' <exception cref="NullPointerException">
		'''          if the cause is {@code null} </exception>
		Public Sub New(ByVal cause As IOException)
			MyBase.New(java.util.Objects.requireNonNull(cause))
		End Sub

		''' <summary>
		''' Returns the cause of this exception.
		''' </summary>
		''' <returns>  the {@code IOException} which is the cause of this exception. </returns>
		Public  Overrides ReadOnly Property  cause As IOException
			Get
				Return CType(MyBase.cause, IOException)
			End Get
		End Property

		''' <summary>
		''' Called to read the object from a stream.
		''' </summary>
		''' <exception cref="InvalidObjectException">
		'''          if the object is invalid or has a cause that is not
		'''          an {@code IOException} </exception>
		Private Sub readObject(ByVal s As ObjectInputStream)
			s.defaultReadObject()
			Dim cause_Renamed As Throwable = MyBase.cause
			If Not(TypeOf cause_Renamed Is IOException) Then Throw New InvalidObjectException("Cause must be an IOException")
		End Sub
	End Class

End Namespace