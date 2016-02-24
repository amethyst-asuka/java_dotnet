Imports Microsoft.VisualBasic

'
' * Copyright (c) 2014, Oracle and/or its affiliates. All rights reserved.
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
	''' A method handle whose invocation behavior is determined by a target.
	''' The delegating MH itself can hold extra "intentions" beyond the simple behavior.
	''' @author jrose
	''' </summary>
	'non-public
	Friend MustInherit Class DelegatingMethodHandle
		Inherits MethodHandle

		Protected Friend Sub New(ByVal target As MethodHandle)
			Me.New(target.type(), target)
		End Sub

		Protected Friend Sub New(ByVal type As MethodType, ByVal target As MethodHandle)
			MyBase.New(type, chooseDelegatingForm(target))
		End Sub

		Protected Friend Sub New(ByVal type As MethodType, ByVal form As LambdaForm)
			MyBase.New(type, form)
		End Sub

		''' <summary>
		''' Define this to extract the delegated target which supplies the invocation behavior. </summary>
		Protected Friend MustOverride ReadOnly Property target As MethodHandle

		Friend MustOverride Overrides Function asTypeUncached(ByVal newType As MethodType) As MethodHandle

		Friend Overrides Function internalMemberName() As MemberName
			Return target.internalMemberName()
		End Function

		Friend Property Overrides invokeSpecial As Boolean
			Get
				Return target.invokeSpecial
			End Get
		End Property

		Friend Overrides Function internalCallerClass() As Class
			Return target.internalCallerClass()
		End Function

		Friend Overrides Function copyWith(ByVal mt As MethodType, ByVal lf As LambdaForm) As MethodHandle
			' FIXME: rethink 'copyWith' protocol; it is too low-level for use on all MHs
			Throw newIllegalArgumentException("do not use this")
		End Function

		Friend Overrides Function internalProperties() As String
			Return vbLf & "& Class=" & Me.GetType().simpleName+ vbLf & "& Target=" & target.debugString()
		End Function

		Friend Overrides Function rebind() As BoundMethodHandle
			Return target.rebind()
		End Function

		Private Shared Function chooseDelegatingForm(ByVal target As MethodHandle) As LambdaForm
			If TypeOf target Is SimpleMethodHandle Then Return target.internalForm() ' no need for an indirection
			Return makeReinvokerForm(target, MethodTypeForm.LF_DELEGATE, GetType(DelegatingMethodHandle), NF_getTarget)
		End Function

		Friend Shared Function makeReinvokerForm(ByVal target As MethodHandle, ByVal whichCache As Integer, ByVal constraint As Object, ByVal getTargetFn As NamedFunction) As LambdaForm
			Dim debugString As String
			Select Case whichCache
				Case MethodTypeForm.LF_REBIND
					debugString = "BMH.reinvoke"
				Case MethodTypeForm.LF_DELEGATE
					debugString = "MH.delegate"
				Case Else
					debugString = "MH.reinvoke"
			End Select
			' No pre-action needed.
			Return makeReinvokerForm(target, whichCache, constraint, debugString, True, getTargetFn, Nothing)
		End Function
		''' <summary>
		''' Create a LF which simply reinvokes a target of the given basic type. </summary>
		Friend Shared Function makeReinvokerForm(ByVal target As MethodHandle, ByVal whichCache As Integer, ByVal constraint As Object, ByVal debugString As String, ByVal forceInline As Boolean, ByVal getTargetFn As NamedFunction, ByVal preActionFn As NamedFunction) As LambdaForm
			Dim mtype As MethodType = target.type().basicType()
			Dim customized As Boolean = (whichCache < 0 OrElse mtype.parameterSlotCount() > MethodType.MAX_MH_INVOKER_ARITY)
			Dim hasPreAction As Boolean = (preActionFn IsNot Nothing)
			Dim form As LambdaForm
			If Not customized Then
				form = mtype.form().cachedLambdaForm(whichCache)
				If form IsNot Nothing Then Return form
			End If
			Const THIS_DMH As Integer = 0
			Const ARG_BASE As Integer = 1
			Dim ARG_LIMIT As Integer = ARG_BASE + mtype.parameterCount()
			Dim nameCursor As Integer = ARG_LIMIT
				Dim PRE_ACTION As Integer = If(hasPreAction, nameCursor, -1)
				nameCursor += 1
				Dim NEXT_MH As Integer = If(customized, -1, nameCursor)
				nameCursor += 1
			Dim REINVOKE As Integer = nameCursor
			nameCursor += 1
			Dim names As LambdaForm.Name() = LambdaForm.arguments(nameCursor - ARG_LIMIT, mtype.invokerType())
			assert(names.Length = nameCursor)
			names(THIS_DMH) = names(THIS_DMH).withConstraint(constraint)
			Dim targetArgs As Object()
			If hasPreAction Then names(PRE_ACTION) = New LambdaForm.Name(preActionFn, names(THIS_DMH))
			If customized Then
				targetArgs = java.util.Arrays.copyOfRange(names, ARG_BASE, ARG_LIMIT, GetType(Object()))
				names(REINVOKE) = New LambdaForm.Name(target, targetArgs) ' the invoker is the target itself
			Else
				names(NEXT_MH) = New LambdaForm.Name(getTargetFn, names(THIS_DMH))
				targetArgs = java.util.Arrays.copyOfRange(names, THIS_DMH, ARG_LIMIT, GetType(Object()))
				targetArgs(0) = names(NEXT_MH) ' overwrite this MH with next MH
				names(REINVOKE) = New LambdaForm.Name(mtype, targetArgs)
			End If
			form = New LambdaForm(debugString, ARG_LIMIT, names, forceInline)
			If Not customized Then form = mtype.form().cachedLambdaFormorm(whichCache, form)
			Return form
		End Function

		Friend Shared ReadOnly NF_getTarget As NamedFunction
		Shared Sub New()
			Try
				NF_getTarget = New NamedFunction(GetType(DelegatingMethodHandle).getDeclaredMethod("getTarget"))
			Catch ex As ReflectiveOperationException
				Throw newInternalError(ex)
			End Try
		End Sub
	End Class

End Namespace