'
' * Copyright (c) 1996, 2000, Oracle and/or its affiliates. All rights reserved.
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
Namespace org.omg.CORBA

	''' <summary>
	''' An object that indicates whether a method had completed running
	''' when a <code>SystemException</code> was thrown.
	''' <P>
	''' The class <code>CompletionStatus</code>
	''' contains three <code>CompletionStatus</code> instances, which are constants
	''' representing each
	''' possible completion status: <code>COMPLETED_MAYBE</code>,
	''' <code>COMPLETED_NO</code>, and <code>COMPLETED_YES</code>.
	''' It also contains
	''' three <code>int</code> members, each a constant corresponding to one of
	''' the <code>CompletionStatus</code> instances.  These <code>int</code>
	''' members make it possible to use a <code>switch</code> statement.
	''' <P>
	''' The class also contains two methods:
	''' <UL>
	''' <LI><code>public int <bold>value</bold>()</code> -- which accesses the
	''' <code>value</code> field of a <code>CompletionStatus</code> object
	''' <LI><code>public static CompletionStatus
	''' <bold>from_int</bold>(int i)</code> --
	''' for creating an instance from one of the <code>int</code> members
	''' </UL> </summary>
	''' <seealso cref=     org.omg.CORBA.SystemException
	''' @since   JDK1.2 </seealso>

	Public NotInheritable Class CompletionStatus
		Implements org.omg.CORBA.portable.IDLEntity

	''' <summary>
	''' The constant indicating that a method completed running
	''' before a <code>SystemException</code> was thrown.
	''' </summary>
		Public Const _COMPLETED_YES As Integer = 0, _COMPLETED_NO As Integer = 1, _COMPLETED_MAYBE As Integer = 2
	''' <summary>
	''' The constant indicating that a method had not completed running
	''' when a <code>SystemException</code> was thrown.
	''' </summary>
	''' <summary>
	''' The constant indicating that it is unknown whether a method had
	''' completed running when a <code>SystemException</code> was thrown.
	''' </summary>


	''' <summary>
	''' An instance of <code>CompletionStatus</code> initialized with
	''' the constant <code>_COMPLETED_YES</code>.
	''' </summary>
		Public Shared ReadOnly COMPLETED_YES As New CompletionStatus(_COMPLETED_YES)

	''' <summary>
	''' An instance of <code>CompletionStatus</code> initialized with
	''' the constant <code>_COMPLETED_NO</code>.
	''' </summary>
		Public Shared ReadOnly COMPLETED_NO As New CompletionStatus(_COMPLETED_NO)

		''' <summary>
		''' An instance of <code>CompletionStatus</code> initialized with
		''' the constant <code>_COMPLETED_MAYBE</code>.
		''' </summary>
		Public Shared ReadOnly COMPLETED_MAYBE As New CompletionStatus(_COMPLETED_MAYBE)

		''' <summary>
		''' Retrieves the value of this <code>CompletionStatus</code> object.
		''' </summary>
		''' <returns>  one of the possible <code>CompletionStatus</code> values:
		'''          <code>_COMPLETED_YES</code>, <code>_COMPLETED_NO</code>, or
		'''          <code>_COMPLETED_MAYBE</code>
		'''  </returns>
		Public Function value() As Integer
			Return _value
		End Function

	''' <summary>
	''' Creates a <code>CompletionStatus</code> object from the given <code>int</code>.
	''' </summary>
	''' <param name="i">  one of <code>_COMPLETED_YES</code>, <code>_COMPLETED_NO</code>, or
	'''          <code>_COMPLETED_MAYBE</code>
	''' </param>
	''' <returns>  one of the possible <code>CompletionStatus</code> objects
	'''          with values:
	'''          <code>_COMPLETED_YES</code>, <code>_COMPLETED_NO</code>, or
	'''          <code>_COMPLETED_MAYBE</code>
	''' </returns>
	''' <exception cref="org.omg.CORBA.BAD_PARAM">  if the argument given is not one of the
	'''            <code>int</code> constants defined in <code>CompletionStatus</code> </exception>
		Public Shared Function from_int(ByVal i As Integer) As CompletionStatus
			Select Case i
			Case _COMPLETED_YES
				Return COMPLETED_YES
			Case _COMPLETED_NO
				Return COMPLETED_NO
			Case _COMPLETED_MAYBE
				Return COMPLETED_MAYBE
			Case Else
				Throw New org.omg.CORBA.BAD_PARAM
			End Select
		End Function


	''' <summary>
	''' Creates a <code>CompletionStatus</code> object from the given <code>int</code>.
	''' </summary>
	''' <param name="_value">  one of <code>_COMPLETED_YES</code>, <code>_COMPLETED_NO</code>, or
	'''          <code>_COMPLETED_MAYBE</code>
	'''  </param>
		Private Sub New(ByVal _value As Integer)
			Me._value = _value
		End Sub

		Private _value As Integer
	End Class

End Namespace