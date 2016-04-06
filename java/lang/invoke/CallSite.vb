Imports Microsoft.VisualBasic
Imports System

'
' * Copyright (c) 2008, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A {@code CallSite} is a holder for a variable <seealso cref="MethodHandle"/>,
	''' which is called its {@code target}.
	''' An {@code invokedynamic} instruction linked to a {@code CallSite} delegates
	''' all calls to the site's current target.
	''' A {@code CallSite} may be associated with several {@code invokedynamic}
	''' instructions, or it may be "free floating", associated with none.
	''' In any case, it may be invoked through an associated method handle
	''' called its <seealso cref="#dynamicInvoker dynamic invoker"/>.
	''' <p>
	''' {@code CallSite} is an abstract class which does not allow
	''' direct subclassing by users.  It has three immediate,
	''' concrete subclasses that may be either instantiated or subclassed.
	''' <ul>
	''' <li>If a mutable target is not required, an {@code invokedynamic} instruction
	''' may be permanently bound by means of a <seealso cref="ConstantCallSite constant call site"/>.
	''' <li>If a mutable target is required which has volatile variable semantics,
	''' because updates to the target must be immediately and reliably witnessed by other threads,
	''' a <seealso cref="VolatileCallSite volatile call site"/> may be used.
	''' <li>Otherwise, if a mutable target is required,
	''' a <seealso cref="MutableCallSite mutable call site"/> may be used.
	''' </ul>
	''' <p>
	''' A non-constant call site may be <em>relinked</em> by changing its target.
	''' The new target must have the same <seealso cref="MethodHandle#type() type"/>
	''' as the previous target.
	''' Thus, though a call site can be relinked to a series of
	''' successive targets, it cannot change its type.
	''' <p>
	''' Here is a sample use of call sites and bootstrap methods which links every
	''' dynamic call site to print its arguments:
	''' <blockquote><pre>{@code
	''' static  Sub  test() throws Throwable {
	'''    // THE FOLLOWING LINE IS PSEUDOCODE FOR A JVM INSTRUCTION
	'''    InvokeDynamic[#bootstrapDynamic].baz("baz arg", 2, 3.14);
	''' }
	''' private static  Sub  printArgs(Object... args) {
	'''  System.out.println(java.util.Arrays.deepToString(args));
	''' }
	''' private static final MethodHandle printArgs;
	''' static {
	'''  MethodHandles.Lookup lookup = MethodHandles.lookup();
	'''  Class thisClass = lookup.lookupClass();  // (who am I?)
	'''  printArgs = lookup.findStatic(thisClass,
	'''      "printArgs", MethodType.methodType(void.class, Object[].class));
	''' }
	''' private static CallSite bootstrapDynamic(MethodHandles.Lookup caller, String name, MethodType type) {
	'''  // ignore caller and name, but match the type:
	'''  return new ConstantCallSite(printArgs.asType(type));
	''' }
	''' }</pre></blockquote>
	''' @author John Rose, JSR 292 EG
	''' </summary>
	Public MustInherit Class CallSite
		Shared Sub New()
			MethodHandleImpl.initStatics()
			Try
				GET_TARGET = IMPL_LOOKUP.findVirtual(GetType(CallSite), "getTarget", MethodType.methodType(GetType(MethodHandle)))
				THROW_UCS = IMPL_LOOKUP.findStatic(GetType(CallSite), "uninitializedCallSite", MethodType.methodType(GetType(Object), GetType(Object())))
			Catch e As ReflectiveOperationException
				Throw newInternalError(e)
			End Try
			Try
				TARGET_OFFSET = UNSAFE.objectFieldOffset(GetType(CallSite).getDeclaredField("target"))
			Catch ex As Exception
				Throw New [Error](ex)
			End Try
		End Sub

		' The actual payload of this call site:
		'package-private
		Friend target As MethodHandle ' Note: This field is known to the JVM.  Do not change.

		''' <summary>
		''' Make a blank call site object with the given method type.
		''' An initial target method is supplied which will throw
		''' an <seealso cref="IllegalStateException"/> if called.
		''' <p>
		''' Before this {@code CallSite} object is returned from a bootstrap method,
		''' it is usually provided with a more useful target method,
		''' via a call to <seealso cref="CallSite#setTarget(MethodHandle) setTarget"/>. </summary>
		''' <exception cref="NullPointerException"> if the proposed type is null </exception>
		'package-private
		Friend Sub New(  type As MethodType)
			target = makeUninitializedCallSite(type)
		End Sub

		''' <summary>
		''' Make a call site object equipped with an initial target method handle. </summary>
		''' <param name="target"> the method handle which will be the initial target of the call site </param>
		''' <exception cref="NullPointerException"> if the proposed target is null </exception>
		'package-private
		Friend Sub New(  target As MethodHandle)
			target.type() ' null check
			Me.target = target
		End Sub

		''' <summary>
		''' Make a call site object equipped with an initial target method handle. </summary>
		''' <param name="targetType"> the desired type of the call site </param>
		''' <param name="createTargetHook"> a hook which will bind the call site to the target method handle </param>
		''' <exception cref="WrongMethodTypeException"> if the hook cannot be invoked on the required arguments,
		'''         or if the target returned by the hook is not of the given {@code targetType} </exception>
		''' <exception cref="NullPointerException"> if the hook returns a null value </exception>
		''' <exception cref="ClassCastException"> if the hook returns something other than a {@code MethodHandle} </exception>
		''' <exception cref="Throwable"> anything else thrown by the hook function </exception>
		'package-private
		Friend Sub New(  targetType As MethodType,   createTargetHook As MethodHandle)
			Me.New(targetType)
			Dim selfCCS As ConstantCallSite = CType(Me, ConstantCallSite)
			Dim boundTarget As MethodHandle = CType(createTargetHook.invokeWithArguments(selfCCS), MethodHandle)
			checkTargetChange(Me.target, boundTarget)
			Me.target = boundTarget
		End Sub

		''' <summary>
		''' Returns the type of this call site's target.
		''' Although targets may change, any call site's type is permanent, and can never change to an unequal type.
		''' The {@code setTarget} method enforces this invariant by refusing any new target that does
		''' not have the previous target's type. </summary>
		''' <returns> the type of the current target, which is also the type of any future target </returns>
		Public Overridable Function type() As MethodType
			' warning:  do not call getTarget here, because CCS.getTarget can throw IllegalStateException
			Return target.type()
		End Function

		''' <summary>
		''' Returns the target method of the call site, according to the
		''' behavior defined by this call site's specific class.
		''' The immediate subclasses of {@code CallSite} document the
		''' class-specific behaviors of this method.
		''' </summary>
		''' <returns> the current linkage state of the call site, its target method handle </returns>
		''' <seealso cref= ConstantCallSite </seealso>
		''' <seealso cref= VolatileCallSite </seealso>
		''' <seealso cref= #setTarget </seealso>
		''' <seealso cref= ConstantCallSite#getTarget </seealso>
		''' <seealso cref= MutableCallSite#getTarget </seealso>
		''' <seealso cref= VolatileCallSite#getTarget </seealso>
		Public MustOverride Property target As MethodHandle


		Friend Overridable Sub checkTargetChange(  oldTarget As MethodHandle,   newTarget As MethodHandle)
			Dim oldType As MethodType = oldTarget.type()
			Dim newType As MethodType = newTarget.type() ' null check!
			If Not newType.Equals(oldType) Then Throw wrongTargetType(newTarget, oldType)
		End Sub

		Private Shared Function wrongTargetType(  target As MethodHandle,   type As MethodType) As WrongMethodTypeException
			Return New WrongMethodTypeException(Convert.ToString(target) & " should be of type " & type)
		End Function

		''' <summary>
		''' Produces a method handle equivalent to an invokedynamic instruction
		''' which has been linked to this call site.
		''' <p>
		''' This method is equivalent to the following code:
		''' <blockquote><pre>{@code
		''' MethodHandle getTarget, invoker, result;
		''' getTarget = MethodHandles.publicLookup().bind(this, "getTarget", MethodType.methodType(MethodHandle.class));
		''' invoker = MethodHandles.exactInvoker(this.type());
		''' result = MethodHandles.foldArguments(invoker, getTarget)
		''' }</pre></blockquote>
		''' </summary>
		''' <returns> a method handle which always invokes this call site's current target </returns>
		Public MustOverride Function dynamicInvoker() As MethodHandle

		'non-public
	 Friend Overridable Function makeDynamicInvoker() As MethodHandle
			Dim getTarget As MethodHandle = GET_TARGET.bindArgumentL(0, Me)
			Dim invoker As MethodHandle = MethodHandles.exactInvoker(Me.type())
			Return MethodHandles.foldArguments(invoker, getTarget)
	 End Function

		Private Shared ReadOnly GET_TARGET As MethodHandle
		Private Shared ReadOnly THROW_UCS As MethodHandle

		''' <summary>
		''' This guy is rolled into the default target if a MethodType is supplied to the constructor. </summary>
		Private Shared Function uninitializedCallSite(ParamArray   ignore As Object()) As Object
			Throw New IllegalStateException("uninitialized call site")
		End Function

		Private Function makeUninitializedCallSite(  targetType As MethodType) As MethodHandle
			Dim basicType As MethodType = targetType.basicType()
			Dim invoker As MethodHandle = basicType.form().cachedMethodHandle(MethodTypeForm.MH_UNINIT_CS)
			If invoker Is Nothing Then
				invoker = THROW_UCS.asType(basicType)
				invoker = basicType.form().cachedMethodHandledle(MethodTypeForm.MH_UNINIT_CS, invoker)
			End If
			' unchecked view is OK since no values will be received or returned
			Return invoker.viewAsType(targetType, False)
		End Function

		' unsafe stuff:
		Private Shared ReadOnly TARGET_OFFSET As Long

		'package-private
		Friend Overridable Property targetNormal As MethodHandle
			Set(  newTarget As MethodHandle)
				MethodHandleNatives.callSiteTargetNormalmal(Me, newTarget)
			End Set
		End Property
		'package-private
		Friend Overridable Property targetVolatile As MethodHandle
			Get
				Return CType(UNSAFE.getObjectVolatile(Me, TARGET_OFFSET), MethodHandle)
			End Get
			Set(  newTarget As MethodHandle)
				MethodHandleNatives.callSiteTargetVolatileile(Me, newTarget)
			End Set
		End Property
		'package-private

		' this implements the upcall from the JVM, MethodHandleNatives.makeDynamicCallSite:
		Shared Function makeSite(  bootstrapMethod As MethodHandle,   name As String,   type As MethodType,   info As Object,   callerClass As [Class]) As CallSite
								 ' Callee information:
								 ' Extra arguments for BSM, if any:
								 ' Caller information:
			Dim caller As MethodHandles.Lookup = IMPL_LOOKUP.in(callerClass)
			Dim site As CallSite
			Try
				Dim binding As Object
				info = maybeReBox(info)
				If info Is Nothing Then
					binding = bootstrapMethod.invoke(caller, name, type)
				ElseIf Not info.GetType().IsArray Then
					binding = bootstrapMethod.invoke(caller, name, type, info)
				Else
					Dim argv As Object() = CType(info, Object())
					maybeReBoxElements(argv)
					Select Case argv.Length
					Case 0
						binding = bootstrapMethod.invoke(caller, name, type)
					Case 1
						binding = bootstrapMethod.invoke(caller, name, type, argv(0))
					Case 2
						binding = bootstrapMethod.invoke(caller, name, type, argv(0), argv(1))
					Case 3
						binding = bootstrapMethod.invoke(caller, name, type, argv(0), argv(1), argv(2))
					Case 4
						binding = bootstrapMethod.invoke(caller, name, type, argv(0), argv(1), argv(2), argv(3))
					Case 5
						binding = bootstrapMethod.invoke(caller, name, type, argv(0), argv(1), argv(2), argv(3), argv(4))
					Case 6
						binding = bootstrapMethod.invoke(caller, name, type, argv(0), argv(1), argv(2), argv(3), argv(4), argv(5))
					Case Else
						Const NON_SPREAD_ARG_COUNT As Integer = 3 ' (caller, name, type)
						If NON_SPREAD_ARG_COUNT + argv.Length > MethodType.MAX_MH_ARITY Then Throw New BootstrapMethodError("too many bootstrap method arguments")
						Dim bsmType As MethodType = bootstrapMethod.type()
						Dim invocationType As MethodType = MethodType.genericMethodType(NON_SPREAD_ARG_COUNT + argv.Length)
						Dim typedBSM As MethodHandle = bootstrapMethod.asType(invocationType)
						Dim spreader As MethodHandle = invocationType.invokers().spreadInvoker(NON_SPREAD_ARG_COUNT)
						binding = spreader.invokeExact(typedBSM, CObj(caller), CObj(name), CObj(type), argv)
					End Select
				End If
				'System.out.println("BSM for "+name+type+" => "+binding);
				If TypeOf binding Is CallSite Then
					site = CType(binding, CallSite)
				Else
					Throw New ClassCastException("bootstrap method failed to produce a CallSite")
				End If
				If Not site.target.type().Equals(type) Then Throw wrongTargetType(site.target, type)
			Catch ex As Throwable
				Dim bex As BootstrapMethodError
				If TypeOf ex Is BootstrapMethodError Then
					bex = CType(ex, BootstrapMethodError)
				Else
					bex = New BootstrapMethodError("call site initialization exception", ex)
				End If
				Throw bex
			End Try
			Return site
		End Function

		Private Shared Function maybeReBox(  x As Object) As Object
			If TypeOf x Is Integer? Then
				Dim xi As Integer = CInt(Fix(x))
				If xi = CByte(xi) Then x = xi ' must rebox; see JLS 5.1.7
			End If
			Return x
		End Function
		Private Shared Sub maybeReBoxElements(  xa As Object())
			For i As Integer = 0 To xa.Length - 1
				xa(i) = maybeReBox(xa(i))
			Next i
		End Sub
	End Class

End Namespace