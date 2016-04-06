Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2011, Oracle and/or its affiliates. All rights reserved.
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

Namespace java.awt

	''' <summary>
	''' An <code>Insets</code> object is a representation of the borders
	''' of a container. It specifies the space that a container must leave
	''' at each of its edges. The space can be a border, a blank space, or
	''' a title.
	''' 
	''' @author      Arthur van Hoff
	''' @author      Sami Shaio </summary>
	''' <seealso cref=         java.awt.LayoutManager </seealso>
	''' <seealso cref=         java.awt.Container
	''' @since       JDK1.0 </seealso>
	<Serializable> _
	Public Class Insets
		Implements Cloneable

		''' <summary>
		''' The inset from the top.
		''' This value is added to the Top of the rectangle
		''' to yield a new location for the Top.
		''' 
		''' @serial </summary>
		''' <seealso cref= #clone() </seealso>
		Public top As Integer

		''' <summary>
		''' The inset from the left.
		''' This value is added to the Left of the rectangle
		''' to yield a new location for the Left edge.
		''' 
		''' @serial </summary>
		''' <seealso cref= #clone() </seealso>
		Public left As Integer

		''' <summary>
		''' The inset from the bottom.
		''' This value is subtracted from the Bottom of the rectangle
		''' to yield a new location for the Bottom.
		''' 
		''' @serial </summary>
		''' <seealso cref= #clone() </seealso>
		Public bottom As Integer

		''' <summary>
		''' The inset from the right.
		''' This value is subtracted from the Right of the rectangle
		''' to yield a new location for the Right edge.
		''' 
		''' @serial </summary>
		''' <seealso cref= #clone() </seealso>
		Public right As Integer

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = -2272572637695466749L

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' Creates and initializes a new <code>Insets</code> object with the
		''' specified top, left, bottom, and right insets. </summary>
		''' <param name="top">   the inset from the top. </param>
		''' <param name="left">   the inset from the left. </param>
		''' <param name="bottom">   the inset from the bottom. </param>
		''' <param name="right">   the inset from the right. </param>
		Public Sub New(  top As Integer,   left As Integer,   bottom As Integer,   right As Integer)
			Me.top = top
			Me.left = left
			Me.bottom = bottom
			Me.right = right
		End Sub

		''' <summary>
		''' Set top, left, bottom, and right to the specified values
		''' </summary>
		''' <param name="top">   the inset from the top. </param>
		''' <param name="left">   the inset from the left. </param>
		''' <param name="bottom">   the inset from the bottom. </param>
		''' <param name="right">   the inset from the right.
		''' @since 1.5 </param>
		Public Overridable Sub [set](  top As Integer,   left As Integer,   bottom As Integer,   right As Integer)
			Me.top = top
			Me.left = left
			Me.bottom = bottom
			Me.right = right
		End Sub

		''' <summary>
		''' Checks whether two insets objects are equal. Two instances
		''' of <code>Insets</code> are equal if the four integer values
		''' of the fields <code>top</code>, <code>left</code>,
		''' <code>bottom</code>, and <code>right</code> are all equal. </summary>
		''' <returns>      <code>true</code> if the two insets are equal;
		'''                          otherwise <code>false</code>.
		''' @since       JDK1.1 </returns>
		Public Overrides Function Equals(  obj As Object) As Boolean
			If TypeOf obj Is Insets Then
				Dim insets_Renamed As Insets = CType(obj, Insets)
				Return ((top = insets_Renamed.top) AndAlso (left = insets_Renamed.left) AndAlso (bottom = insets_Renamed.bottom) AndAlso (right = insets_Renamed.right))
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the hash code for this Insets.
		''' </summary>
		''' <returns>    a hash code for this Insets. </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim sum1 As Integer = left + bottom
			Dim sum2 As Integer = right + top
			Dim val1 As Integer = sum1 * (sum1 + 1)\2 + left
			Dim val2 As Integer = sum2 * (sum2 + 1)\2 + top
			Dim sum3 As Integer = val1 + val2
			Return sum3 * (sum3 + 1)\2 + val2
		End Function

		''' <summary>
		''' Returns a string representation of this <code>Insets</code> object.
		''' This method is intended to be used only for debugging purposes, and
		''' the content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>Insets</code> object. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[top=" & top & ",left=" & left & ",bottom=" & bottom & ",right=" & right & "]"
		End Function

		''' <summary>
		''' Create a copy of this object. </summary>
		''' <returns>     a copy of this <code>Insets</code> object. </returns>
		Public Overridable Function clone() As Object
			Try
				Return MyBase.clone()
			Catch e As CloneNotSupportedException
				' this shouldn't happen, since we are Cloneable
				Throw New InternalError(e)
			End Try
		End Function
		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

	End Class

End Namespace