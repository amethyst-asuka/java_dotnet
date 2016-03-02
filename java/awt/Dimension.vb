Imports Microsoft.VisualBasic
Imports System
Imports System.Runtime.InteropServices

'
' * Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' The <code>Dimension</code> class encapsulates the width and
	''' height of a component (in integer precision) in a single object.
	''' The class is
	''' associated with certain properties of components. Several methods
	''' defined by the <code>Component</code> class and the
	''' <code>LayoutManager</code> interface return a
	''' <code>Dimension</code> object.
	''' <p>
	''' Normally the values of <code>width</code>
	''' and <code>height</code> are non-negative integers.
	''' The constructors that allow you to create a dimension do
	''' not prevent you from setting a negative value for these properties.
	''' If the value of <code>width</code> or <code>height</code> is
	''' negative, the behavior of some methods defined by other objects is
	''' undefined.
	''' 
	''' @author      Sami Shaio
	''' @author      Arthur van Hoff </summary>
	''' <seealso cref=         java.awt.Component </seealso>
	''' <seealso cref=         java.awt.LayoutManager
	''' @since       1.0 </seealso>
	<Serializable> _
	Public Class Dimension
		Inherits java.awt.geom.Dimension2D

        '     * JDK 1.1 serialVersionUID
        '     
        Private Const serialVersionUID As Long = 4723952579491349524L

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' Creates an instance of <code>Dimension</code> with a width
		''' of zero and a height of zero.
		''' </summary>
		Public Sub New()
			Me.New(0, 0)
		End Sub

		''' <summary>
		''' Creates an instance of <code>Dimension</code> whose width
		''' and height are the same as for the specified dimension.
		''' </summary>
		''' <param name="d">   the specified dimension for the
		'''               <code>width</code> and
		'''               <code>height</code> values </param>
		Public Sub New(ByVal d As Dimension)
			Me.New(d.width, d.height)
		End Sub

		''' <summary>
		''' Constructs a <code>Dimension</code> and initializes
		''' it to the specified width and specified height.
		''' </summary>
		''' <param name="width"> the specified width </param>
		''' <param name="height"> the specified height </param>
		Public Sub New(ByVal width As Integer, ByVal height As Integer)
			Me.width = width
			Me.height = height
		End Sub

        ''' <summary>
        ''' {@inheritDoc}
        ''' @since 1.2
        ''' </summary>
        Public Overrides ReadOnly Property width As Double

        ''' <summary>
        ''' {@inheritDoc}
        ''' @since 1.2
        ''' </summary>
        Public Overrides ReadOnly Property height As Double

        ''' <summary>
        ''' Sets the size of this <code>Dimension</code> object to
        ''' the specified width and height in double precision.
        ''' Note that if <code>width</code> or <code>height</code>
        ''' are larger than <code> java.lang.[Integer].MAX_VALUE</code>, they will
        ''' be reset to <code> java.lang.[Integer].MAX_VALUE</code>.
        ''' </summary>
        ''' <param name="width">  the new width for the <code>Dimension</code> object </param>
        ''' <param name="height"> the new height for the <code>Dimension</code> object
        ''' @since 1.2 </param>
        Public Overrides Sub setSize(ByVal width As Double, ByVal height As Double)
            Me._width = CInt(Fix(System.Math.Ceiling(width)))
            Me._height = CInt(Fix(System.Math.Ceiling(height)))
        End Sub

        ''' <summary>
        ''' Gets the size of this <code>Dimension</code> object.
        ''' This method is included for completeness, to parallel the
        ''' <code>getSize</code> method defined by <code>Component</code>.
        ''' </summary>
        ''' <returns>   the size of this dimension, a new instance of
        '''           <code>Dimension</code> with the same width and height </returns>
        ''' <seealso cref=      java.awt.Dimension#setSize </seealso>
        ''' <seealso cref=      java.awt.Component#getSize
        ''' @since    1.1 </seealso>
        Public Overridable Property size As Dimension
            Get
                Return New Dimension(width, height)
            End Get
            Set(ByVal d As Dimension)
                sizeize(d.width, d.height)
            End Set
        End Property


        ''' <summary>
        ''' Sets the size of this <code>Dimension</code> object
        ''' to the specified width and height.
        ''' This method is included for completeness, to parallel the
        ''' <code>setSize</code> method defined by <code>Component</code>.
        ''' </summary>
        ''' <param name="width">   the new width for this <code>Dimension</code> object </param>
        ''' <param name="height">  the new height for this <code>Dimension</code> object </param>
        ''' <seealso cref=      java.awt.Dimension#getSize </seealso>
        ''' <seealso cref=      java.awt.Component#setSize
        ''' @since    1.1 </seealso>
        Public Sub setSize(ByVal width As Integer, ByVal height As Integer)
            Me._width = width
            Me._height = height
        End Sub

        ''' <summary>
        ''' Checks whether two dimension objects have equal values.
        ''' </summary>
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
			If TypeOf obj Is Dimension Then
				Dim d As Dimension = CType(obj, Dimension)
				Return (width = d.width) AndAlso (height = d.height)
			End If
			Return False
		End Function

		''' <summary>
		''' Returns the hash code for this <code>Dimension</code>.
		''' </summary>
		''' <returns>    a hash code for this <code>Dimension</code> </returns>
		Public Overrides Function GetHashCode() As Integer
			Dim sum As Integer = width + height
			Return sum * (sum + 1)\2 + width
		End Function

		''' <summary>
		''' Returns a string representation of the values of this
		''' <code>Dimension</code> object's <code>height</code> and
		''' <code>width</code> fields. This method is intended to be used only
		''' for debugging purposes, and the content and format of the returned
		''' string may vary between implementations. The returned string may be
		''' empty but may not be <code>null</code>.
		''' </summary>
		''' <returns>  a string representation of this <code>Dimension</code>
		'''          object </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name & "[width=" & width & ",height=" & height & "]"
		End Function
	End Class

End Namespace