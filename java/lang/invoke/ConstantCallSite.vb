'
' * Copyright (c) 2010, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A {@code ConstantCallSite} is a <seealso cref="CallSite"/> whose target is permanent, and can never be changed.
	''' An {@code invokedynamic} instruction linked to a {@code ConstantCallSite} is permanently
	''' bound to the call site's target.
	''' @author John Rose, JSR 292 EG
	''' </summary>
	Public Class ConstantCallSite
		Inherits CallSite

		Private ReadOnly isFrozen As Boolean

		''' <summary>
		''' Creates a call site with a permanent target. </summary>
		''' <param name="target"> the target to be permanently associated with this call site </param>
		''' <exception cref="NullPointerException"> if the proposed target is null </exception>
		Public Sub New(ByVal target As MethodHandle)
			MyBase.New(target)
			isFrozen = True
		End Sub

		''' <summary>
		''' Creates a call site with a permanent target, possibly bound to the call site itself.
		''' <p>
		''' During construction of the call site, the {@code createTargetHook} is invoked to
		''' produce the actual target, as if by a call of the form
		''' {@code (MethodHandle) createTargetHook.invoke(this)}.
		''' <p>
		''' Note that user code cannot perform such an action directly in a subclass constructor,
		''' since the target must be fixed before the {@code ConstantCallSite} constructor returns.
		''' <p>
		''' The hook is said to bind the call site to a target method handle,
		''' and a typical action would be {@code someTarget.bindTo(this)}.
		''' However, the hook is free to take any action whatever,
		''' including ignoring the call site and returning a constant target.
		''' <p>
		''' The result returned by the hook must be a method handle of exactly
		''' the same type as the call site.
		''' <p>
		''' While the hook is being called, the new {@code ConstantCallSite}
		''' object is in a partially constructed state.
		''' In this state,
		''' a call to {@code getTarget}, or any other attempt to use the target,
		''' will result in an {@code IllegalStateException}.
		''' It is legal at all times to obtain the call site's type using the {@code type} method.
		''' </summary>
		''' <param name="targetType"> the type of the method handle to be permanently associated with this call site </param>
		''' <param name="createTargetHook"> a method handle to invoke (on the call site) to produce the call site's target </param>
		''' <exception cref="WrongMethodTypeException"> if the hook cannot be invoked on the required arguments,
		'''         or if the target returned by the hook is not of the given {@code targetType} </exception>
		''' <exception cref="NullPointerException"> if the hook returns a null value </exception>
		''' <exception cref="ClassCastException"> if the hook returns something other than a {@code MethodHandle} </exception>
		''' <exception cref="Throwable"> anything else thrown by the hook function </exception>
		Protected Friend Sub New(ByVal targetType As MethodType, ByVal createTargetHook As MethodHandle)
			MyBase.New(targetType, createTargetHook)
			isFrozen = True
		End Sub

		''' <summary>
		''' Returns the target method of the call site, which behaves
		''' like a {@code final} field of the {@code ConstantCallSite}.
		''' That is, the target is always the original value passed
		''' to the constructor call which created this instance.
		''' </summary>
		''' <returns> the immutable linkage state of this call site, a constant method handle </returns>
		''' <exception cref="IllegalStateException"> if the {@code ConstantCallSite} constructor has not completed </exception>
		Public Property NotOverridable Overrides target As MethodHandle
			Get
				If Not isFrozen Then Throw New IllegalStateException
				Return target
			End Get
			Set(ByVal ignore As MethodHandle)
				Throw New UnsupportedOperationException
			End Set
		End Property


		''' <summary>
		''' Returns this call site's permanent target.
		''' Since that target will never change, this is a correct implementation
		''' of <seealso cref="CallSite#dynamicInvoker CallSite.dynamicInvoker"/>. </summary>
		''' <returns> the immutable linkage state of this call site, a constant method handle </returns>
		''' <exception cref="IllegalStateException"> if the {@code ConstantCallSite} constructor has not completed </exception>
		Public NotOverridable Overrides Function dynamicInvoker() As MethodHandle
			Return target
		End Function
	End Class

End Namespace