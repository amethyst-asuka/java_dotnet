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

Namespace java.lang.invoke

	''' <summary>
	''' A {@code VolatileCallSite} is a <seealso cref="CallSite"/> whose target acts like a volatile variable.
	''' An {@code invokedynamic} instruction linked to a {@code VolatileCallSite} sees updates
	''' to its call site target immediately, even if the update occurs in another thread.
	''' There may be a performance penalty for such tight coupling between threads.
	''' <p>
	''' Unlike {@code MutableCallSite}, there is no
	''' <seealso cref="MutableCallSite#syncAll syncAll operation"/> on volatile
	''' call sites, since every write to a volatile variable is implicitly
	''' synchronized with reader threads.
	''' <p>
	''' In other respects, a {@code VolatileCallSite} is interchangeable
	''' with {@code MutableCallSite}. </summary>
	''' <seealso cref= MutableCallSite
	''' @author John Rose, JSR 292 EG </seealso>
	Public Class VolatileCallSite
		Inherits CallSite

		''' <summary>
		''' Creates a call site with a volatile binding to its target.
		''' The initial target is set to a method handle
		''' of the given type which will throw an {@code IllegalStateException} if called. </summary>
		''' <param name="type"> the method type that this call site will have </param>
		''' <exception cref="NullPointerException"> if the proposed type is null </exception>
		Public Sub New(  type As MethodType)
			MyBase.New(type)
		End Sub

		''' <summary>
		''' Creates a call site with a volatile binding to its target.
		''' The target is set to the given value. </summary>
		''' <param name="target"> the method handle that will be the initial target of the call site </param>
		''' <exception cref="NullPointerException"> if the proposed target is null </exception>
		Public Sub New(  target As MethodHandle)
			MyBase.New(target)
		End Sub

		''' <summary>
		''' Returns the target method of the call site, which behaves
		''' like a {@code volatile} field of the {@code VolatileCallSite}.
		''' <p>
		''' The interactions of {@code getTarget} with memory are the same
		''' as of a read from a {@code volatile} field.
		''' <p>
		''' In particular, the current thread is required to issue a fresh
		''' read of the target from memory, and must not fail to see
		''' a recent update to the target by another thread.
		''' </summary>
		''' <returns> the linkage state of this call site, a method handle which can change over time </returns>
		''' <seealso cref= #setTarget </seealso>
		Public Property NotOverridable Overrides target As MethodHandle
			Get
				Return targetVolatile
			End Get
			Set(  newTarget As MethodHandle)
				checkTargetChange(targetVolatile, newTarget)
				targetVolatile = newTarget
			End Set
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public NotOverridable Overrides Function dynamicInvoker() As MethodHandle
			Return makeDynamicInvoker()
		End Function
	End Class

End Namespace