'
' * Copyright (c) 2010, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.nio.file


	''' <summary>
	''' Runtime exception thrown if an I/O error is encountered when iterating over
	''' the entries in a directory. The I/O error is retrieved as an {@link
	''' IOException} using the <seealso cref="#getCause() getCause()"/> method.
	''' 
	''' @since 1.7 </summary>
	''' <seealso cref= DirectoryStream </seealso>

	Public NotInheritable Class DirectoryIteratorException
		Inherits java.util.ConcurrentModificationException

		Private Shadows Const serialVersionUID As Long = -6012699886086212874L

		''' <summary>
		''' Constructs an instance of this class.
		''' </summary>
		''' <param name="cause">
		'''          the {@code IOException} that caused the directory iteration
		'''          to fail
		''' </param>
		''' <exception cref="NullPointerException">
		'''          if the cause is {@code null} </exception>
		Public Sub New(ByVal cause As java.io.IOException)
			MyBase.New(java.util.Objects.requireNonNull(cause))
		End Sub

		''' <summary>
		''' Returns the cause of this exception.
		''' </summary>
		''' <returns>  the cause </returns>
		Public Property Overrides cause As java.io.IOException
			Get
				Return CType(MyBase.cause, java.io.IOException)
			End Get
		End Property

		''' <summary>
		''' Called to read the object from a stream.
		''' </summary>
		''' <exception cref="InvalidObjectException">
		'''          if the object is invalid or has a cause that is not
		'''          an {@code IOException} </exception>
		Private Sub readObject(ByVal s As java.io.ObjectInputStream)
			s.defaultReadObject()
			Dim cause_Renamed As Throwable = MyBase.cause
			If Not(TypeOf cause_Renamed Is java.io.IOException) Then Throw New java.io.InvalidObjectException("Cause must be an IOException")
		End Sub
	End Class

End Namespace