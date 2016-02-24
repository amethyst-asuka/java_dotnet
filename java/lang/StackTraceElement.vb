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

Namespace java.lang


	''' <summary>
	''' An element in a stack trace, as returned by {@link
	''' Throwable#getStackTrace()}.  Each element represents a single stack frame.
	''' All stack frames except for the one at the top of the stack represent
	''' a method invocation.  The frame at the top of the stack represents the
	''' execution point at which the stack trace was generated.  Typically,
	''' this is the point at which the throwable corresponding to the stack trace
	''' was created.
	''' 
	''' @since  1.4
	''' @author Josh Bloch
	''' </summary>
	<Serializable> _
	Public NotInheritable Class StackTraceElement
		' Normally initialized by VM (public constructor added in 1.5)
		Private declaringClass As String
		Private methodName As String
		Private fileName As String
		Private lineNumber As Integer

		''' <summary>
		''' Creates a stack trace element representing the specified execution
		''' point.
		''' </summary>
		''' <param name="declaringClass"> the fully qualified name of the class containing
		'''        the execution point represented by the stack trace element </param>
		''' <param name="methodName"> the name of the method containing the execution point
		'''        represented by the stack trace element </param>
		''' <param name="fileName"> the name of the file containing the execution point
		'''         represented by the stack trace element, or {@code null} if
		'''         this information is unavailable </param>
		''' <param name="lineNumber"> the line number of the source line containing the
		'''         execution point represented by this stack trace element, or
		'''         a negative number if this information is unavailable. A value
		'''         of -2 indicates that the method containing the execution point
		'''         is a native method </param>
		''' <exception cref="NullPointerException"> if {@code declaringClass} or
		'''         {@code methodName} is null
		''' @since 1.5 </exception>
		Public Sub New(ByVal declaringClass As String, ByVal methodName As String, ByVal fileName As String, ByVal lineNumber As Integer)
			Me.declaringClass = java.util.Objects.requireNonNull(declaringClass, "Declaring class is null")
			Me.methodName = java.util.Objects.requireNonNull(methodName, "Method name is null")
			Me.fileName = fileName
			Me.lineNumber = lineNumber
		End Sub

		''' <summary>
		''' Returns the name of the source file containing the execution point
		''' represented by this stack trace element.  Generally, this corresponds
		''' to the {@code SourceFile} attribute of the relevant {@code class}
		''' file (as per <i>The Java Virtual Machine Specification</i>, Section
		''' 4.7.7).  In some systems, the name may refer to some source code unit
		''' other than a file, such as an entry in source repository.
		''' </summary>
		''' <returns> the name of the file containing the execution point
		'''         represented by this stack trace element, or {@code null} if
		'''         this information is unavailable. </returns>
		Public Property fileName As String
			Get
				Return fileName
			End Get
		End Property

		''' <summary>
		''' Returns the line number of the source line containing the execution
		''' point represented by this stack trace element.  Generally, this is
		''' derived from the {@code LineNumberTable} attribute of the relevant
		''' {@code class} file (as per <i>The Java Virtual Machine
		''' Specification</i>, Section 4.7.8).
		''' </summary>
		''' <returns> the line number of the source line containing the execution
		'''         point represented by this stack trace element, or a negative
		'''         number if this information is unavailable. </returns>
		Public Property lineNumber As Integer
			Get
				Return lineNumber
			End Get
		End Property

		''' <summary>
		''' Returns the fully qualified name of the class containing the
		''' execution point represented by this stack trace element.
		''' </summary>
		''' <returns> the fully qualified name of the {@code Class} containing
		'''         the execution point represented by this stack trace element. </returns>
		Public Property className As String
			Get
				Return declaringClass
			End Get
		End Property

		''' <summary>
		''' Returns the name of the method containing the execution point
		''' represented by this stack trace element.  If the execution point is
		''' contained in an instance or class initializer, this method will return
		''' the appropriate <i>special method name</i>, {@code <init>} or
		''' {@code <clinit>}, as per Section 3.9 of <i>The Java Virtual
		''' Machine Specification</i>.
		''' </summary>
		''' <returns> the name of the method containing the execution point
		'''         represented by this stack trace element. </returns>
		Public Property methodName As String
			Get
				Return methodName
			End Get
		End Property

		''' <summary>
		''' Returns true if the method containing the execution point
		''' represented by this stack trace element is a native method.
		''' </summary>
		''' <returns> {@code true} if the method containing the execution point
		'''         represented by this stack trace element is a native method. </returns>
		Public Property nativeMethod As Boolean
			Get
				Return lineNumber = -2
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this stack trace element.  The
		''' format of this string depends on the implementation, but the following
		''' examples may be regarded as typical:
		''' <ul>
		''' <li>
		'''   {@code "MyClass.mash(MyClass.java:9)"} - Here, {@code "MyClass"}
		'''   is the <i>fully-qualified name</i> of the class containing the
		'''   execution point represented by this stack trace element,
		'''   {@code "mash"} is the name of the method containing the execution
		'''   point, {@code "MyClass.java"} is the source file containing the
		'''   execution point, and {@code "9"} is the line number of the source
		'''   line containing the execution point.
		''' <li>
		'''   {@code "MyClass.mash(MyClass.java)"} - As above, but the line
		'''   number is unavailable.
		''' <li>
		'''   {@code "MyClass.mash(Unknown Source)"} - As above, but neither
		'''   the file name nor the line  number are available.
		''' <li>
		'''   {@code "MyClass.mash(Native Method)"} - As above, but neither
		'''   the file name nor the line  number are available, and the method
		'''   containing the execution point is known to be a native method.
		''' </ul> </summary>
		''' <seealso cref=    Throwable#printStackTrace() </seealso>
		Public Overrides Function ToString() As String
			Return className & "." & methodName + (If(nativeMethod, "(Native Method)", (If(fileName IsNot Nothing AndAlso lineNumber >= 0, "(" & fileName & ":" & lineNumber & ")", (If(fileName IsNot Nothing, "(" & fileName & ")", "(Unknown Source)"))))))
		End Function

		''' <summary>
		''' Returns true if the specified object is another
		''' {@code StackTraceElement} instance representing the same execution
		''' point as this instance.  Two stack trace elements {@code a} and
		''' {@code b} are equal if and only if:
		''' <pre>{@code
		'''     equals(a.getFileName(), b.getFileName()) &&
		'''     a.getLineNumber() == b.getLineNumber()) &&
		'''     equals(a.getClassName(), b.getClassName()) &&
		'''     equals(a.getMethodName(), b.getMethodName())
		''' }</pre>
		''' where {@code equals} has the semantics of {@link
		''' java.util.Objects#equals(Object, Object) Objects.equals}.
		''' </summary>
		''' <param name="obj"> the object to be compared with this stack trace element. </param>
		''' <returns> true if the specified object is another
		'''         {@code StackTraceElement} instance representing the same
		'''         execution point as this instance. </returns>
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If obj Is Me Then Return True
			If Not(TypeOf obj Is StackTraceElement) Then Return False
			Dim e As StackTraceElement = CType(obj, StackTraceElement)
			Return e.declaringClass.Equals(declaringClass) AndAlso e.lineNumber = lineNumber AndAlso java.util.Objects.Equals(methodName, e.methodName) AndAlso java.util.Objects.Equals(fileName, e.fileName)
		End Function

		''' <summary>
		''' Returns a hash code value for this stack trace element.
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Dim result As Integer = 31*declaringClass.GetHashCode() + methodName.GetHashCode()
			result = 31*result + java.util.Objects.hashCode(fileName)
			result = 31*result + lineNumber
			Return result
		End Function

		Private Const serialVersionUID As Long = 6992337162326171013L
	End Class

End Namespace