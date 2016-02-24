Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

'
' * Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.beans


	''' <summary>
	''' A MethodDescriptor describes a particular method that a Java Bean
	''' supports for external access from other components.
	''' </summary>

	Public Class MethodDescriptor
		Inherits FeatureDescriptor

		Private ReadOnly methodRef As New MethodRef

		Private paramNames As String()

		Private params As IList(Of WeakReference(Of [Class]))

		Private parameterDescriptors As ParameterDescriptor()

		''' <summary>
		''' Constructs a <code>MethodDescriptor</code> from a
		''' <code>Method</code>.
		''' </summary>
		''' <param name="method">    The low-level method information. </param>
		Public Sub New(ByVal method As Method)
			Me.New(method, Nothing)
		End Sub


		''' <summary>
		''' Constructs a <code>MethodDescriptor</code> from a
		''' <code>Method</code> providing descriptive information for each
		''' of the method's parameters.
		''' </summary>
		''' <param name="method">    The low-level method information. </param>
		''' <param name="parameterDescriptors">  Descriptive information for each of the
		'''                          method's parameters. </param>
		Public Sub New(ByVal method As Method, ByVal parameterDescriptors As ParameterDescriptor())
			name = method.name
			method = method
			Me.parameterDescriptors = If(parameterDescriptors IsNot Nothing, parameterDescriptors.clone(), Nothing)
		End Sub

		''' <summary>
		''' Gets the method that this MethodDescriptor encapsulates.
		''' </summary>
		''' <returns> The low-level description of the method </returns>
		<MethodImpl(MethodImplOptions.Synchronized)> _
		Public Overridable Property method As Method
			Get
				Dim method_Renamed As Method = Me.methodRef.get()
				If method_Renamed Is Nothing Then
					Dim cls As Class = class0
					Dim name_Renamed As String = name
					If (cls IsNot Nothing) AndAlso (name_Renamed IsNot Nothing) Then
						Dim params_Renamed As Class() = params
						If params_Renamed Is Nothing Then
							For i As Integer = 0 To 2
								' Find methods for up to 2 params. We are guessing here.
								' This block should never execute unless the classloader
								' that loaded the argument classes disappears.
								method_Renamed = Introspector.findMethod(cls, name_Renamed, i, Nothing)
								If method_Renamed IsNot Nothing Then Exit For
							Next i
						Else
							method_Renamed = Introspector.findMethod(cls, name_Renamed, params_Renamed.Length, params_Renamed)
						End If
						method = method_Renamed
					End If
				End If
				Return method_Renamed
			End Get
			Set(ByVal method As Method)
				If method Is Nothing Then Return
				If class0 Is Nothing Then class0 = method.declaringClass
				params = getParameterTypes(class0, method)
				Me.methodRef.set(method)
			End Set
		End Property


		<MethodImpl(MethodImplOptions.Synchronized)> _
'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Private Sub setParams(ByVal param As Class[]) 'JavaToDotNetTempPropertySetparams
		Private Property params As Class()
			Set(ByVal param As Class())
				If param Is Nothing Then Return
				paramNames = New String(param.Length - 1){}
				params = New List(Of )(param.Length)
				For i As Integer = 0 To param.Length - 1
					paramNames(i) = param(i).name
					params.Add(New WeakReference(Of [Class])(param(i)))
				Next i
			End Set
			Get
		End Property

		' pp getParamNames used as an optimization to avoid method.getParameterTypes.
		Friend Overridable Property paramNames As String()
			Get
				Return paramNames
			End Get
		End Property

			Dim clss As Class() = New [Class](params.Count - 1){}

			For i As Integer = 0 To params.Count - 1
'JAVA TO VB CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
				Dim ref As Reference(Of ? As Class) = CType(params(i), Reference(Of ? As Class))
				Dim cls As Class = ref.get()
				If cls Is Nothing Then
					Return Nothing
				Else
					clss(i) = cls
				End If
			Next i
			Return clss
		End Function

		''' <summary>
		''' Gets the ParameterDescriptor for each of this MethodDescriptor's
		''' method's parameters.
		''' </summary>
		''' <returns> The locale-independent names of the parameters.  May return
		'''          a null array if the parameter names aren't known. </returns>
		Public Overridable Property parameterDescriptors As ParameterDescriptor()
			Get
				Return If(Me.parameterDescriptors IsNot Nothing, Me.parameterDescriptors.clone(), Nothing)
			End Get
		End Property

		Private Shared Function resolve(ByVal oldMethod As Method, ByVal newMethod As Method) As Method
			If oldMethod Is Nothing Then Return newMethod
			If newMethod Is Nothing Then Return oldMethod
			Return If((Not oldMethod.synthetic) AndAlso newMethod.synthetic, oldMethod, newMethod)
		End Function

	'    
	'     * Package-private constructor
	'     * Merge two method descriptors.  Where they conflict, give the
	'     * second argument (y) priority over the first argument (x).
	'     * @param x  The first (lower priority) MethodDescriptor
	'     * @param y  The second (higher priority) MethodDescriptor
	'     

		Friend Sub New(ByVal x As MethodDescriptor, ByVal y As MethodDescriptor)
			MyBase.New(x, y)

			Me.methodRef.set(resolve(x.methodRef.get(), y.methodRef.get()))
			params = x.params
			If y.params IsNot Nothing Then params = y.params
			paramNames = x.paramNames
			If y.paramNames IsNot Nothing Then paramNames = y.paramNames

			parameterDescriptors = x.parameterDescriptors
			If y.parameterDescriptors IsNot Nothing Then parameterDescriptors = y.parameterDescriptors
		End Sub

	'    
	'     * Package-private dup constructor
	'     * This must isolate the new object from any changes to the old object.
	'     
		Friend Sub New(ByVal old As MethodDescriptor)
			MyBase.New(old)

			Me.methodRef.set(old.method)
			params = old.params
			paramNames = old.paramNames

			If old.parameterDescriptors IsNot Nothing Then
				Dim len As Integer = old.parameterDescriptors.Length
				parameterDescriptors = New ParameterDescriptor(len - 1){}
				For i As Integer = 0 To len - 1
					parameterDescriptors(i) = New ParameterDescriptor(old.parameterDescriptors(i))
				Next i
			End If
		End Sub

		Friend Overrides Sub appendTo(ByVal sb As StringBuilder)
			appendTo(sb, "method", Me.methodRef.get())
			If Me.parameterDescriptors IsNot Nothing Then
				sb.append("; parameterDescriptors={")
				For Each pd As ParameterDescriptor In Me.parameterDescriptors
					sb.append(pd).append(", ")
				Next pd
				sb.length = sb.length() - 2
				sb.append("}")
			End If
		End Sub
	End Class

End Namespace