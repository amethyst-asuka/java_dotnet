Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1997, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt.image


	''' <summary>
	''' The <code>Kernel</code> class defines a matrix that describes how a
	''' specified pixel and its surrounding pixels affect the value
	''' computed for the pixel's position in the output image of a filtering
	''' operation.  The X origin and Y origin indicate the kernel matrix element
	''' that corresponds to the pixel position for which an output value is
	''' being computed.
	''' </summary>
	''' <seealso cref= ConvolveOp </seealso>
	Public Class Kernel
		Implements Cloneable

		Private width As Integer
		Private height As Integer
		Private xOrigin As Integer
		Private yOrigin As Integer
		Private data As Single()

'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub
		Shared Sub New()
			ColorModel.loadLibraries()
			initIDs()
		End Sub

		''' <summary>
		''' Constructs a <code>Kernel</code> object from an array of floats.
		''' The first <code>width</code>*<code>height</code> elements of
		''' the <code>data</code> array are copied.
		''' If the length of the <code>data</code> array is less
		''' than width*height, an <code>IllegalArgumentException</code> is thrown.
		''' The X origin is (width-1)/2 and the Y origin is (height-1)/2. </summary>
		''' <param name="width">         width of the kernel </param>
		''' <param name="height">        height of the kernel </param>
		''' <param name="data">          kernel data in row major order </param>
		''' <exception cref="IllegalArgumentException"> if the length of <code>data</code>
		'''         is less than the product of <code>width</code> and
		'''         <code>height</code> </exception>
		Public Sub New(ByVal width As Integer, ByVal height As Integer, ByVal data As Single())
			Me.width = width
			Me.height = height
			Me.xOrigin = (width-1)>>1
			Me.yOrigin = (height-1)>>1
			Dim len As Integer = width*height
			If data.Length < len Then Throw New IllegalArgumentException("Data array too small " & "(is " & data.Length & " and should be " & len)
			Me.data = New Single(len - 1){}
			Array.Copy(data, 0, Me.data, 0, len)

		End Sub

		''' <summary>
		''' Returns the X origin of this <code>Kernel</code>. </summary>
		''' <returns> the X origin. </returns>
		Public Property xOrigin As Integer
			Get
				Return xOrigin
			End Get
		End Property

		''' <summary>
		''' Returns the Y origin of this <code>Kernel</code>. </summary>
		''' <returns> the Y origin. </returns>
		Public Property yOrigin As Integer
			Get
				Return yOrigin
			End Get
		End Property

		''' <summary>
		''' Returns the width of this <code>Kernel</code>. </summary>
		''' <returns> the width of this <code>Kernel</code>. </returns>
		Public Property width As Integer
			Get
				Return width
			End Get
		End Property

		''' <summary>
		''' Returns the height of this <code>Kernel</code>. </summary>
		''' <returns> the height of this <code>Kernel</code>. </returns>
		Public Property height As Integer
			Get
				Return height
			End Get
		End Property

		''' <summary>
		''' Returns the kernel data in row major order.
		''' The <code>data</code> array is returned.  If <code>data</code>
		''' is <code>null</code>, a new array is allocated. </summary>
		''' <param name="data">  if non-null, contains the returned kernel data </param>
		''' <returns> the <code>data</code> array containing the kernel data
		'''         in row major order or, if <code>data</code> is
		'''         <code>null</code>, a newly allocated array containing
		'''         the kernel data in row major order </returns>
		''' <exception cref="IllegalArgumentException"> if <code>data</code> is less
		'''         than the size of this <code>Kernel</code> </exception>
		Public Function getKernelData(ByVal data As Single()) As Single()
			If data Is Nothing Then
				data = New Single(Me.data.Length - 1){}
			ElseIf data.Length < Me.data.Length Then
				Throw New IllegalArgumentException("Data array too small " & "(should be " & Me.data.Length & " but is " & data.Length & " )")
			End If
			Array.Copy(Me.data, 0, data, 0, Me.data.Length)

			Return data
		End Function

		''' <summary>
		''' Clones this object. </summary>
		''' <returns> a clone of this object. </returns>
		Public Overridable Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function
	End Class

End Namespace