Imports System

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
Namespace java.lang.invoke


	''' <summary>
	''' Serialized form of a lambda expression.  The properties of this class
	''' represent the information that is present at the lambda factory site, including
	''' static metafactory arguments such as the identity of the primary functional
	''' interface method and the identity of the implementation method, as well as
	''' dynamic metafactory arguments such as values captured from the lexical scope
	''' at the time of lambda capture.
	''' 
	''' <p>Implementors of serializable lambdas, such as compilers or language
	''' runtime libraries, are expected to ensure that instances deserialize properly.
	''' One means to do so is to ensure that the {@code writeReplace} method returns
	''' an instance of {@code SerializedLambda}, rather than allowing default
	''' serialization to proceed.
	''' 
	''' <p>{@code SerializedLambda} has a {@code readResolve} method that looks for
	''' a (possibly private) static method called
	''' {@code $deserializeLambda$(SerializedLambda)} in the capturing [Class], invokes
	''' that with itself as the first argument, and returns the result.  Lambda classes
	''' implementing {@code $deserializeLambda$} are responsible for validating
	''' that the properties of the {@code SerializedLambda} are consistent with a
	''' lambda actually captured by that class.
	''' </summary>
	''' <seealso cref= LambdaMetafactory </seealso>
	<Serializable> _
	Public NotInheritable Class SerializedLambda
		Private Const serialVersionUID As Long = 8025925345765570181L
		Private ReadOnly capturingClass As  [Class]
		Private ReadOnly functionalInterfaceClass As String
		Private ReadOnly functionalInterfaceMethodName As String
		Private ReadOnly functionalInterfaceMethodSignature As String
		Private ReadOnly implClass As String
		Private ReadOnly implMethodName As String
		Private ReadOnly implMethodSignature As String
		Private ReadOnly implMethodKind As Integer
		Private ReadOnly instantiatedMethodType As String
		Private ReadOnly capturedArgs As Object()

		''' <summary>
		''' Create a {@code SerializedLambda} from the low-level information present
		''' at the lambda factory site.
		''' </summary>
		''' <param name="capturingClass"> The class in which the lambda expression appears </param>
		''' <param name="functionalInterfaceClass"> Name, in slash-delimited form, of static
		'''                                 type of the returned lambda object </param>
		''' <param name="functionalInterfaceMethodName"> Name of the functional interface
		'''                                      method for the present at the
		'''                                      lambda factory site </param>
		''' <param name="functionalInterfaceMethodSignature"> Signature of the functional
		'''                                           interface method present at
		'''                                           the lambda factory site </param>
		''' <param name="implMethodKind"> Method handle kind for the implementation method </param>
		''' <param name="implClass"> Name, in slash-delimited form, for the class holding
		'''                  the implementation method </param>
		''' <param name="implMethodName"> Name of the implementation method </param>
		''' <param name="implMethodSignature"> Signature of the implementation method </param>
		''' <param name="instantiatedMethodType"> The signature of the primary functional
		'''                               interface method after type variables
		'''                               are substituted with their instantiation
		'''                               from the capture site </param>
		''' <param name="capturedArgs"> The dynamic arguments to the lambda factory site,
		'''                     which represent variables captured by
		'''                     the lambda </param>
		Public Sub New(  capturingClass As [Class],   functionalInterfaceClass As String,   functionalInterfaceMethodName As String,   functionalInterfaceMethodSignature As String,   implMethodKind As Integer,   implClass As String,   implMethodName As String,   implMethodSignature As String,   instantiatedMethodType As String,   capturedArgs As Object())
			Me.capturingClass = capturingClass
			Me.functionalInterfaceClass = functionalInterfaceClass
			Me.functionalInterfaceMethodName = functionalInterfaceMethodName
			Me.functionalInterfaceMethodSignature = functionalInterfaceMethodSignature
			Me.implMethodKind = implMethodKind
			Me.implClass = implClass
			Me.implMethodName = implMethodName
			Me.implMethodSignature = implMethodSignature
			Me.instantiatedMethodType = instantiatedMethodType
			Me.capturedArgs = java.util.Objects.requireNonNull(capturedArgs).clone()
		End Sub

		''' <summary>
		''' Get the name of the class that captured this lambda. </summary>
		''' <returns> the name of the class that captured this lambda </returns>
		Public Property capturingClass As String
			Get
				Return capturingClass.name.replace("."c, "/"c)
			End Get
		End Property

		''' <summary>
		''' Get the name of the invoked type to which this
		''' lambda has been converted </summary>
		''' <returns> the name of the functional interface class to which
		''' this lambda has been converted </returns>
		Public Property functionalInterfaceClass As String
			Get
				Return functionalInterfaceClass
			End Get
		End Property

		''' <summary>
		''' Get the name of the primary method for the functional interface
		''' to which this lambda has been converted. </summary>
		''' <returns> the name of the primary methods of the functional interface </returns>
		Public Property functionalInterfaceMethodName As String
			Get
				Return functionalInterfaceMethodName
			End Get
		End Property

		''' <summary>
		''' Get the signature of the primary method for the functional
		''' interface to which this lambda has been converted. </summary>
		''' <returns> the signature of the primary method of the functional
		''' interface </returns>
		Public Property functionalInterfaceMethodSignature As String
			Get
				Return functionalInterfaceMethodSignature
			End Get
		End Property

		''' <summary>
		''' Get the name of the class containing the implementation
		''' method. </summary>
		''' <returns> the name of the class containing the implementation
		''' method </returns>
		Public Property implClass As String
			Get
				Return implClass
			End Get
		End Property

		''' <summary>
		''' Get the name of the implementation method. </summary>
		''' <returns> the name of the implementation method </returns>
		Public Property implMethodName As String
			Get
				Return implMethodName
			End Get
		End Property

		''' <summary>
		''' Get the signature of the implementation method. </summary>
		''' <returns> the signature of the implementation method </returns>
		Public Property implMethodSignature As String
			Get
				Return implMethodSignature
			End Get
		End Property

		''' <summary>
		''' Get the method handle kind (see <seealso cref="MethodHandleInfo"/>) of
		''' the implementation method. </summary>
		''' <returns> the method handle kind of the implementation method </returns>
		Public Property implMethodKind As Integer
			Get
				Return implMethodKind
			End Get
		End Property

		''' <summary>
		''' Get the signature of the primary functional interface method
		''' after type variables are substituted with their instantiation
		''' from the capture site. </summary>
		''' <returns> the signature of the primary functional interface method
		''' after type variable processing </returns>
		Public Property instantiatedMethodType As String
			Get
				Return instantiatedMethodType
			End Get
		End Property

		''' <summary>
		''' Get the count of dynamic arguments to the lambda capture site. </summary>
		''' <returns> the count of dynamic arguments to the lambda capture site </returns>
		Public Property capturedArgCount As Integer
			Get
				Return capturedArgs.Length
			End Get
		End Property

		''' <summary>
		''' Get a dynamic argument to the lambda capture site. </summary>
		''' <param name="i"> the argument to capture </param>
		''' <returns> a dynamic argument to the lambda capture site </returns>
		Public Function getCapturedArg(  i As Integer) As Object
			Return capturedArgs(i)
		End Function

		Private Function readResolve() As Object
			Try
				Dim deserialize As Method = java.security.AccessController.doPrivileged(New PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)

				Return deserialize.invoke(Nothing, Me)
			Catch e As java.security.PrivilegedActionException
				Dim cause As Exception = e.exception
				If TypeOf cause Is ReflectiveOperationException Then
					Throw CType(cause, ReflectiveOperationException)
				ElseIf TypeOf cause Is RuntimeException Then
					Throw CType(cause, RuntimeException)
				Else
					Throw New RuntimeException("Exception in SerializedLambda.readResolve", e)
				End If
			End Try
		End Function

		Private Class PrivilegedExceptionActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedExceptionAction(Of T)

			Public Overrides Function run() As Method
				Dim m As Method = outerInstance.capturingClass.getDeclaredMethod("$deserializeLambda$", GetType(SerializedLambda))
				m.accessible = True
				Return m
			End Function
		End Class

		Public Overrides Function ToString() As String
			Dim implKind As String=MethodHandleInfo.referenceKindToString(implMethodKind)
			Return String.Format("SerializedLambda[{0}={1}, {2}={3}.{4}:{5}, " & "{6}={7} {8}.{9}:{10}, {11}={12}, {13}={14:D}]", "capturingClass", capturingClass, "functionalInterfaceMethod", functionalInterfaceClass, functionalInterfaceMethodName, functionalInterfaceMethodSignature, "implementation", implKind, implClass, implMethodName, implMethodSignature, "instantiatedMethodType", instantiatedMethodType, "numCaptured", capturedArgs.Length)
		End Function
	End Class

End Namespace