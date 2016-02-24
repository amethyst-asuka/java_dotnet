Imports System

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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

'
' * (C) Copyright Taligent, Inc. 1996 - 1997, All Rights Reserved
' * (C) Copyright IBM Corp. 1996 - 1998, All Rights Reserved
' *
' * The original version of this source code and documentation is
' * copyrighted and owned by Taligent, Inc., a wholly-owned subsidiary
' * of IBM. These materials are provided under terms of a License
' * Agreement between Taligent and Sun. This technology is protected
' * by multiple US and International patents.
' *
' * This notice and attribution to Taligent may not be removed.
' * Taligent is a registered trademark of Taligent, Inc.
' *
' 

Namespace java.awt.font


	''' <summary>
	''' The <code>TransformAttribute</code> class provides an immutable
	''' wrapper for a transform so that it is safe to use as an attribute.
	''' </summary>
	<Serializable> _
	Public NotInheritable Class TransformAttribute

		''' <summary>
		''' The <code>AffineTransform</code> for this
		''' <code>TransformAttribute</code>, or <code>null</code>
		''' if <code>AffineTransform</code> is the identity transform.
		''' </summary>
		Private transform As java.awt.geom.AffineTransform

		''' <summary>
		''' Wraps the specified transform.  The transform is cloned and a
		''' reference to the clone is kept.  The original transform is unchanged.
		''' If null is passed as the argument, this constructor behaves as though
		''' it were the identity transform.  (Note that it is preferable to use
		''' <seealso cref="#IDENTITY"/> in this case.) </summary>
		''' <param name="transform"> the specified <seealso cref="AffineTransform"/> to be wrapped,
		''' or null. </param>
		Public Sub New(ByVal transform As java.awt.geom.AffineTransform)
			If transform IsNot Nothing AndAlso (Not transform.identity) Then Me.transform = New java.awt.geom.AffineTransform(transform)
		End Sub

		''' <summary>
		''' Returns a copy of the wrapped transform. </summary>
		''' <returns> a <code>AffineTransform</code> that is a copy of the wrapped
		''' transform of this <code>TransformAttribute</code>. </returns>
		Public Property transform As java.awt.geom.AffineTransform
			Get
				Dim at As java.awt.geom.AffineTransform = transform
				Return If(at Is Nothing, New java.awt.geom.AffineTransform, New java.awt.geom.AffineTransform(at))
			End Get
		End Property

		''' <summary>
		''' Returns <code>true</code> if the wrapped transform is
		''' an identity transform. </summary>
		''' <returns> <code>true</code> if the wrapped transform is
		''' an identity transform; <code>false</code> otherwise.
		''' @since 1.4 </returns>
		Public Property identity As Boolean
			Get
				Return transform Is Nothing
			End Get
		End Property

		''' <summary>
		''' A <code>TransformAttribute</code> representing the identity transform.
		''' @since 1.6
		''' </summary>
		Public Shared ReadOnly IDENTITY As New TransformAttribute(Nothing)

		Private Sub writeObject(ByVal s As java.io.ObjectOutputStream)
			' sigh -- 1.3 expects transform is never null, so we need to always write one out
			If Me.transform Is Nothing Then Me.transform = New java.awt.geom.AffineTransform
			s.defaultWriteObject()
		End Sub

	'    
	'     * @since 1.6
	'     
		Private Function readResolve() As Object
			If transform Is Nothing OrElse transform.identity Then Return IDENTITY
			Return Me
		End Function

		' Added for serial backwards compatibility (4348425)
		Friend Const serialVersionUID As Long = 3356247357827709530L

		''' <summary>
		''' @since 1.6
		''' </summary>
		Public Overrides Function GetHashCode() As Integer
			Return If(transform Is Nothing, 0, transform.GetHashCode())
		End Function

		''' <summary>
		''' Returns <code>true</code> if rhs is a <code>TransformAttribute</code>
		''' whose transform is equal to this <code>TransformAttribute</code>'s
		''' transform. </summary>
		''' <param name="rhs"> the object to compare to </param>
		''' <returns> <code>true</code> if the argument is a <code>TransformAttribute</code>
		''' whose transform is equal to this <code>TransformAttribute</code>'s
		''' transform.
		''' @since 1.6 </returns>
		Public Overrides Function Equals(ByVal rhs As Object) As Boolean
			If rhs IsNot Nothing Then
				Try
					Dim that As TransformAttribute = CType(rhs, TransformAttribute)
					If transform Is Nothing Then Return that.transform Is Nothing
					Return transform.Equals(that.transform)
				Catch e As ClassCastException
				End Try
			End If
			Return False
		End Function
	End Class

End Namespace