Imports Microsoft.VisualBasic

'
' * Copyright (c) 2008, 2012, Oracle and/or its affiliates. All rights reserved.
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
	''' A method handle whose behavior is determined only by its LambdaForm.
	''' @author jrose
	''' </summary>
	Friend NotInheritable Class SimpleMethodHandle
		Inherits BoundMethodHandle

		Private Sub New(  type As MethodType,   form As LambdaForm)
			MyBase.New(type, form)
		End Sub

		'non-public
	 Friend Shared Function make(  type As MethodType,   form As LambdaForm) As BoundMethodHandle
			Return New SimpleMethodHandle(type, form)
	 End Function

		'non-public
	 Friend Shadows Shared ReadOnly SPECIES_DATA As SpeciesData = SpeciesData.EMPTY

		'non-public
	 Public Overrides Function speciesData() As SpeciesData
				Return SPECIES_DATA
	 End Function

		Friend Overrides Function copyWith(  mt As MethodType,   lf As LambdaForm) As BoundMethodHandle
		'non-public
			Return make(mt, lf)
		End Function

		Friend Overrides Function internalProperties() As String
			Return vbLf & "& Class=" & Me.GetType().simpleName
		End Function

		Public Overrides Function fieldCount() As Integer
		'non-public
			Return 0
		End Function

		Friend NotOverridable Overrides Function copyWithExtendL(  mt As MethodType,   lf As LambdaForm,   narg As Object) As BoundMethodHandle
		'non-public
			Return BoundMethodHandle.bindSingle(mt, lf, narg) ' Use known fast path.
		End Function
		Friend NotOverridable Overrides Function copyWithExtendI(  mt As MethodType,   lf As LambdaForm,   narg As Integer) As BoundMethodHandle
		'non-public
			Try
				Return CType(SPECIES_DATA.extendWith(I_TYPE).constructor().invokeBasic(mt, lf, narg), BoundMethodHandle)
			Catch ex As Throwable
				Throw uncaughtException(ex)
			End Try
		End Function
		Friend NotOverridable Overrides Function copyWithExtendJ(  mt As MethodType,   lf As LambdaForm,   narg As Long) As BoundMethodHandle
		'non-public
			Try
				Return CType(SPECIES_DATA.extendWith(J_TYPE).constructor().invokeBasic(mt, lf, narg), BoundMethodHandle)
			Catch ex As Throwable
				Throw uncaughtException(ex)
			End Try
		End Function
		Friend NotOverridable Overrides Function copyWithExtendF(  mt As MethodType,   lf As LambdaForm,   narg As Single) As BoundMethodHandle
		'non-public
			Try
				Return CType(SPECIES_DATA.extendWith(F_TYPE).constructor().invokeBasic(mt, lf, narg), BoundMethodHandle)
			Catch ex As Throwable
				Throw uncaughtException(ex)
			End Try
		End Function
		Friend NotOverridable Overrides Function copyWithExtendD(  mt As MethodType,   lf As LambdaForm,   narg As Double) As BoundMethodHandle
		'non-public
			Try
				Return CType(SPECIES_DATA.extendWith(D_TYPE).constructor().invokeBasic(mt, lf, narg), BoundMethodHandle)
			Catch ex As Throwable
				Throw uncaughtException(ex)
			End Try
		End Function
	End Class

End Namespace