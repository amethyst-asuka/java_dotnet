import static sun.invoke.util.Wrapper.forPrimitiveType
import static sun.invoke.util.Wrapper.forWrapperType
import static sun.invoke.util.Wrapper.isWrapperType

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
	''' Abstract implementation of a lambda metafactory which provides parameter
	''' unrolling and input validation.
	''' </summary>
	''' <seealso cref= LambdaMetafactory </seealso>
	' package 
	 Friend MustInherit Class AbstractValidatingLambdaMetafactory

	'    
	'     * For context, the comments for the following fields are marked in quotes
	'     * with their values, given this program:
	'     * interface II<T> {  Object foo(T x); }
	'     * interface JJ<R extends Number> extends II<R> { }
	'     * class CC {  String impl(int i) { return "impl:"+i; }}
	'     * class X {
	'     *     Public Shared  Sub  main(String[] args) {
	'     *         JJ<Integer> iii = (new CC())::impl;
	'     *         System.out.printf(">>> %s\n", iii.foo(44));
	'     * }}
	'     
		Friend ReadOnly targetClass As  [Class] ' The class calling the meta-factory via invokedynamic "class X"
		Friend ReadOnly invokedType As MethodType ' The type of the invoked method "(CC)II"
		Friend ReadOnly samBase As  [Class] ' The type of the returned instance "interface JJ"
		Friend ReadOnly samMethodName As String ' Name of the SAM method "foo"
		Friend ReadOnly samMethodType As MethodType ' Type of the SAM method "(Object)Object"
		Friend ReadOnly implMethod As MethodHandle ' Raw method handle for the implementation method
		Friend ReadOnly implInfo As MethodHandleInfo ' Info about the implementation method handle "MethodHandleInfo[5 CC.impl(int)String]"
		Friend ReadOnly implKind As Integer ' Invocation kind for implementation "5"=invokevirtual
		Friend ReadOnly implIsInstanceMethod As Boolean ' Is the implementation an instance method "true"
		Friend ReadOnly implDefiningClass As  [Class] ' Type defining the implementation "class CC"
		Friend ReadOnly implMethodType As MethodType ' Type of the implementation method "(int)String"
		Friend ReadOnly instantiatedMethodType As MethodType ' Instantiated erased functional interface method type "(Integer)Object"
		Friend ReadOnly isSerializable As Boolean ' Should the returned instance be serializable
		Friend ReadOnly markerInterfaces As  [Class]() ' Additional marker interfaces to be implemented
		Friend ReadOnly additionalBridges As MethodType() ' Signatures of additional methods to bridge


		''' <summary>
		''' Meta-factory constructor.
		''' </summary>
		''' <param name="caller"> Stacked automatically by VM; represents a lookup context
		'''               with the accessibility privileges of the caller. </param>
		''' <param name="invokedType"> Stacked automatically by VM; the signature of the
		'''                    invoked method, which includes the expected static
		'''                    type of the returned lambda object, and the static
		'''                    types of the captured arguments for the lambda.  In
		'''                    the event that the implementation method is an
		'''                    instance method, the first argument in the invocation
		'''                    signature will correspond to the receiver. </param>
		''' <param name="samMethodName"> Name of the method in the functional interface to
		'''                      which the lambda or method reference is being
		'''                      converted, represented as a String. </param>
		''' <param name="samMethodType"> Type of the method in the functional interface to
		'''                      which the lambda or method reference is being
		'''                      converted, represented as a MethodType. </param>
		''' <param name="implMethod"> The implementation method which should be called
		'''                   (with suitable adaptation of argument types, return
		'''                   types, and adjustment for captured arguments) when
		'''                   methods of the resulting functional interface instance
		'''                   are invoked. </param>
		''' <param name="instantiatedMethodType"> The signature of the primary functional
		'''                               interface method after type variables are
		'''                               substituted with their instantiation from
		'''                               the capture site </param>
		''' <param name="isSerializable"> Should the lambda be made serializable?  If set,
		'''                       either the target type or one of the additional SAM
		'''                       types must extend {@code Serializable}. </param>
		''' <param name="markerInterfaces"> Additional interfaces which the lambda object
		'''                       should implement. </param>
		''' <param name="additionalBridges"> Method types for additional signatures to be
		'''                          bridged to the implementation method </param>
		''' <exception cref="LambdaConversionException"> If any of the meta-factory protocol
		''' invariants are violated </exception>
		Friend Sub New(  caller As MethodHandles.Lookup,   invokedType As MethodType,   samMethodName As String,   samMethodType As MethodType,   implMethod As MethodHandle,   instantiatedMethodType As MethodType,   isSerializable As Boolean,   markerInterfaces As  [Class](),   additionalBridges As MethodType())
			If (caller.lookupModes() And MethodHandles.Lookup.PRIVATE) = 0 Then Throw New LambdaConversionException(String.Format("Invalid caller: {0}", caller.lookupClass().name))
			Me.targetClass = caller.lookupClass()
			Me.invokedType = invokedType

			Me.samBase = invokedType.returnType()

			Me.samMethodName = samMethodName
			Me.samMethodType = samMethodType

			Me.implMethod = implMethod
			Me.implInfo = caller.revealDirect(implMethod)
			Me.implKind = implInfo.referenceKind
			Me.implIsInstanceMethod = implKind = MethodHandleInfo.REF_invokeVirtual OrElse implKind = MethodHandleInfo.REF_invokeSpecial OrElse implKind = MethodHandleInfo.REF_invokeInterface
			Me.implDefiningClass = implInfo.declaringClass
			Me.implMethodType = implInfo.methodType
			Me.instantiatedMethodType = instantiatedMethodType
			Me.isSerializable = isSerializable
			Me.markerInterfaces = markerInterfaces
			Me.additionalBridges = additionalBridges

			If Not samBase.interface Then Throw New LambdaConversionException(String.Format("Functional interface {0} is not an interface", samBase.name))

			For Each c As  [Class] In markerInterfaces
				If Not c.interface Then Throw New LambdaConversionException(String.Format("Marker interface {0} is not an interface", c.name))
			Next c
		End Sub

		''' <summary>
		''' Build the CallSite.
		''' </summary>
		''' <returns> a CallSite, which, when invoked, will return an instance of the
		''' functional interface </returns>
		''' <exception cref="ReflectiveOperationException"> </exception>
		Friend MustOverride Function buildCallSite() As CallSite

		''' <summary>
		''' Check the meta-factory arguments for errors </summary>
		''' <exception cref="LambdaConversionException"> if there are improper conversions </exception>
		Friend Overridable Sub validateMetafactoryArgs()
			Select Case implKind
				Case MethodHandleInfo.REF_invokeInterface, MethodHandleInfo.REF_invokeVirtual, MethodHandleInfo.REF_invokeStatic, MethodHandleInfo.REF_newInvokeSpecial, MethodHandleInfo.REF_invokeSpecial
				Case Else
					Throw New LambdaConversionException(String.Format("Unsupported MethodHandle kind: {0}", implInfo))
			End Select

			' Check arity: optional-receiver + captured + SAM == impl
			Dim implArity As Integer = implMethodType.parameterCount()
			Dim receiverArity As Integer = If(implIsInstanceMethod, 1, 0)
			Dim capturedArity As Integer = invokedType.parameterCount()
			Dim samArity As Integer = samMethodType.parameterCount()
			Dim instantiatedArity As Integer = instantiatedMethodType.parameterCount()
			If implArity + receiverArity <> capturedArity + samArity Then Throw New LambdaConversionException(String.Format("Incorrect number of parameters for {0} method {1}; {2:D} captured parameters, {3:D} functional interface method parameters, {4:D} implementation parameters",If(implIsInstanceMethod, "instance", "static"), implInfo, capturedArity, samArity, implArity))
			If instantiatedArity <> samArity Then Throw New LambdaConversionException(String.Format("Incorrect number of parameters for {0} method {1}; {2:D} instantiated parameters, {3:D} functional interface method parameters",If(implIsInstanceMethod, "instance", "static"), implInfo, instantiatedArity, samArity))
			For Each bridgeMT As MethodType In additionalBridges
				If bridgeMT.parameterCount() <> samArity Then Throw New LambdaConversionException(String.Format("Incorrect number of parameters for bridge signature {0}; incompatible with {1}", bridgeMT, samMethodType))
			Next bridgeMT

			' If instance: first captured arg (receiver) must be subtype of class where impl method is defined
			Dim capturedStart As Integer
			Dim samStart As Integer
			If implIsInstanceMethod Then
				Dim receiverClass As  [Class]

				' implementation is an instance method, adjust for receiver in captured variables / SAM arguments
				If capturedArity = 0 Then
					' receiver is function parameter
					capturedStart = 0
					samStart = 1
					receiverClass = instantiatedMethodType.parameterType(0)
				Else
					' receiver is a captured variable
					capturedStart = 1
					samStart = 0
					receiverClass = invokedType.parameterType(0)
				End If

				' check receiver type
				If Not receiverClass.IsSubclassOf(implDefiningClass) Then Throw New LambdaConversionException(String.Format("Invalid receiver type {0}; not a subtype of implementation type {1}", receiverClass, implDefiningClass))

			   Dim implReceiverClass As  [Class] = implMethod.type().parameterType(0)
			   If implReceiverClass IsNot implDefiningClass AndAlso (Not receiverClass.IsSubclassOf(implReceiverClass)) Then Throw New LambdaConversionException(String.Format("Invalid receiver type {0}; not a subtype of implementation receiver type {1}", receiverClass, implReceiverClass))
			Else
				' no receiver
				capturedStart = 0
				samStart = 0
			End If

			' Check for exact match on non-receiver captured arguments
			Dim implFromCaptured As Integer = capturedArity - capturedStart
			For i As Integer = 0 To implFromCaptured - 1
				Dim implParamType As  [Class] = implMethodType.parameterType(i)
				Dim capturedParamType As  [Class] = invokedType.parameterType(i + capturedStart)
				If Not capturedParamType.Equals(implParamType) Then Throw New LambdaConversionException(String.Format("Type mismatch in captured lambda parameter {0:D}: expecting {1}, found {2}", i, capturedParamType, implParamType))
			Next i
			' Check for adaptation match on SAM arguments
			Dim samOffset As Integer = samStart - implFromCaptured
			For i As Integer = implFromCaptured To implArity - 1
				Dim implParamType As  [Class] = implMethodType.parameterType(i)
				Dim instantiatedParamType As  [Class] = instantiatedMethodType.parameterType(i + samOffset)
				If Not isAdaptableTo(instantiatedParamType, implParamType, True) Then Throw New LambdaConversionException(String.Format("Type mismatch for lambda argument {0:D}: {1} is not convertible to {2}", i, instantiatedParamType, implParamType))
			Next i

			' Adaptation match: return type
			Dim expectedType As  [Class] = instantiatedMethodType.returnType()
			Dim actualReturnType As  [Class] = If(implKind = MethodHandleInfo.REF_newInvokeSpecial, implDefiningClass, implMethodType.returnType())
			Dim samReturnType As  [Class] = samMethodType.returnType()
			If Not isAdaptableToAsReturn(actualReturnType, expectedType) Then Throw New LambdaConversionException(String.Format("Type mismatch for lambda return: {0} is not convertible to {1}", actualReturnType, expectedType))
			If Not isAdaptableToAsReturnStrict(expectedType, samReturnType) Then Throw New LambdaConversionException(String.Format("Type mismatch for lambda expected return: {0} is not convertible to {1}", expectedType, samReturnType))
			For Each bridgeMT As MethodType In additionalBridges
				If Not isAdaptableToAsReturnStrict(expectedType, bridgeMT.returnType()) Then Throw New LambdaConversionException(String.Format("Type mismatch for lambda expected return: {0} is not convertible to {1}", expectedType, bridgeMT.returnType()))
			Next bridgeMT
		End Sub

		''' <summary>
		''' Check type adaptability for parameter types. </summary>
		''' <param name="fromType"> Type to convert from </param>
		''' <param name="toType"> Type to convert to </param>
		''' <param name="strict"> If true, do strict checks, else allow that fromType may be parameterized </param>
		''' <returns> True if 'fromType' can be passed to an argument of 'toType' </returns>
		Private Function isAdaptableTo(  fromType As [Class],   toType As [Class],   [strict] As Boolean) As Boolean
			If fromType.Equals(toType) Then Return True
			If fromType.primitive Then
				Dim wfrom As sun.invoke.util.Wrapper = forPrimitiveType(fromType)
				If toType.primitive Then
					' both are primitive: widening
					Dim wto As sun.invoke.util.Wrapper = forPrimitiveType(toType)
					Return wto.isConvertibleFrom(wfrom)
				Else
					' from primitive to reference: boxing
					Return wfrom.wrapperType().IsSubclassOf(toType)
				End If
			Else
				If toType.primitive Then
					' from reference to primitive: unboxing
					Dim wfrom As sun.invoke.util.Wrapper
					wfrom = forWrapperType(fromType)
					If isWrapperType(fromType) AndAlso wfrom .primitiveType().primitive Then
						' fromType is a primitive wrapper; unbox+widen
						Dim wto As sun.invoke.util.Wrapper = forPrimitiveType(toType)
						Return wto.isConvertibleFrom(wfrom)
					Else
						' must be convertible to primitive
						Return Not [strict]
					End If
				Else
					' both are reference types: fromType should be a superclass of toType.
					Return (Not [strict]) OrElse fromType.IsSubclassOf(toType)
				End If
			End If
		End Function

		''' <summary>
		''' Check type adaptability for return types --
		''' special handling of  Sub  type) and parameterized fromType </summary>
		''' <returns> True if 'fromType' can be converted to 'toType' </returns>
		Private Function isAdaptableToAsReturn(  fromType As [Class],   toType As [Class]) As Boolean
			Return toType.Equals(GetType(void)) OrElse (Not fromType.Equals(GetType(void))) AndAlso isAdaptableTo(fromType, toType, False)
		End Function
		Private Function isAdaptableToAsReturnStrict(  fromType As [Class],   toType As [Class]) As Boolean
			If fromType.Equals(GetType(void)) Then Return toType.Equals(GetType(void))
			Return isAdaptableTo(fromType, toType, True)
		End Function


		''' <summary>
		'''********* Logging support -- for debugging only, uncomment as needed
		''' static final Executor logPool = Executors.newSingleThreadExecutor();
		''' protected static  Sub  log(final String s) {
		'''    MethodHandleProxyLambdaMetafactory.logPool.execute(new Runnable() {
		'''        @Override
		'''        public  Sub  run() {
		'''            System.out.println(s);
		'''        }
		'''    });
		''' }
		''' 
		''' protected static  Sub  log(final String s, final Throwable e) {
		'''    MethodHandleProxyLambdaMetafactory.logPool.execute(new Runnable() {
		'''        @Override
		'''        public  Sub  run() {
		'''            System.out.println(s);
		'''            e.printStackTrace(System.out);
		'''        }
		'''    });
		''' }
		''' **********************
		''' </summary>

	 End Class

End Namespace