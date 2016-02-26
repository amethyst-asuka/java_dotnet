Imports System

'
' * Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.border


	''' <summary>
	''' A class that implements an empty border with no size.
	''' This provides a convenient base class from which other border
	''' classes can be easily derived.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the <code>java.beans</code> package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' 
	''' @author David Kloba
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	<Serializable> _
	Public MustInherit Class AbstractBorder
		Implements Border

		''' <summary>
		''' This default implementation does no painting. </summary>
		''' <param name="c"> the component for which this border is being painted </param>
		''' <param name="g"> the paint graphics </param>
		''' <param name="x"> the x position of the painted border </param>
		''' <param name="y"> the y position of the painted border </param>
		''' <param name="width"> the width of the painted border </param>
		''' <param name="height"> the height of the painted border </param>
		Public Overridable Sub paintBorder(ByVal c As java.awt.Component, ByVal g As java.awt.Graphics, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) Implements Border.paintBorder
		End Sub

		''' <summary>
		''' This default implementation returns a new <seealso cref="Insets"/> object
		''' that is initialized by the <seealso cref="#getBorderInsets(Component,Insets)"/>
		''' method.
		''' By default the {@code top}, {@code left}, {@code bottom},
		''' and {@code right} fields are set to {@code 0}.
		''' </summary>
		''' <param name="c">  the component for which this border insets value applies </param>
		''' <returns> a new <seealso cref="Insets"/> object </returns>
		Public Overridable Function getBorderInsets(ByVal c As java.awt.Component) As java.awt.Insets Implements Border.getBorderInsets
			Return getBorderInsets(c, New java.awt.Insets(0, 0, 0, 0))
		End Function

		''' <summary>
		''' Reinitializes the insets parameter with this Border's current Insets. </summary>
		''' <param name="c"> the component for which this border insets value applies </param>
		''' <param name="insets"> the object to be reinitialized </param>
		''' <returns> the <code>insets</code> object </returns>
		Public Overridable Function getBorderInsets(ByVal c As java.awt.Component, ByVal insets As java.awt.Insets) As java.awt.Insets
				insets.bottom = 0
					insets.right = insets.bottom
						insets.top = insets.right
						insets.left = insets.top
			Return insets
		End Function

		''' <summary>
		''' This default implementation returns false. </summary>
		''' <returns> false </returns>
		Public Overridable Property borderOpaque As Boolean Implements Border.isBorderOpaque
			Get
				Return False
			End Get
		End Property

		''' <summary>
		''' This convenience method calls the static method. </summary>
		''' <param name="c"> the component for which this border is being computed </param>
		''' <param name="x"> the x position of the border </param>
		''' <param name="y"> the y position of the border </param>
		''' <param name="width"> the width of the border </param>
		''' <param name="height"> the height of the border </param>
		''' <returns> a <code>Rectangle</code> containing the interior coordinates </returns>
		Public Overridable Function getInteriorRectangle(ByVal c As java.awt.Component, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As java.awt.Rectangle
			Return getInteriorRectangle(c, Me, x, y, width, height)
		End Function

		''' <summary>
		''' Returns a rectangle using the arguments minus the
		''' insets of the border. This is useful for determining the area
		''' that components should draw in that will not intersect the border. </summary>
		''' <param name="c"> the component for which this border is being computed </param>
		''' <param name="b"> the <code>Border</code> object </param>
		''' <param name="x"> the x position of the border </param>
		''' <param name="y"> the y position of the border </param>
		''' <param name="width"> the width of the border </param>
		''' <param name="height"> the height of the border </param>
		''' <returns> a <code>Rectangle</code> containing the interior coordinates </returns>
		Public Shared Function getInteriorRectangle(ByVal c As java.awt.Component, ByVal b As Border, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer) As java.awt.Rectangle
			Dim insets As java.awt.Insets
			If b IsNot Nothing Then
				insets = b.getBorderInsets(c)
			Else
				insets = New java.awt.Insets(0, 0, 0, 0)
			End If
			Return New java.awt.Rectangle(x + insets.left, y + insets.top, width - insets.right - insets.left, height - insets.top - insets.bottom)
		End Function

		''' <summary>
		''' Returns the baseline.  A return value less than 0 indicates the border
		''' does not have a reasonable baseline.
		''' <p>
		''' The default implementation returns -1.  Subclasses that support
		''' baseline should override appropriately.  If a value &gt;= 0 is
		''' returned, then the component has a valid baseline for any
		''' size &gt;= the minimum size and <code>getBaselineResizeBehavior</code>
		''' can be used to determine how the baseline changes with size.
		''' </summary>
		''' <param name="c"> <code>Component</code> baseline is being requested for </param>
		''' <param name="width"> the width to get the baseline for </param>
		''' <param name="height"> the height to get the baseline for </param>
		''' <returns> the baseline or &lt; 0 indicating there is no reasonable
		'''         baseline </returns>
		''' <exception cref="IllegalArgumentException"> if width or height is &lt; 0 </exception>
		''' <seealso cref= java.awt.Component#getBaseline(int,int) </seealso>
		''' <seealso cref= java.awt.Component#getBaselineResizeBehavior()
		''' @since 1.6 </seealso>
		Public Overridable Function getBaseline(ByVal c As java.awt.Component, ByVal width As Integer, ByVal height As Integer) As Integer
			If width < 0 OrElse height < 0 Then Throw New System.ArgumentException("Width and height must be >= 0")
			Return -1
		End Function

		''' <summary>
		''' Returns an enum indicating how the baseline of a component
		''' changes as the size changes.  This method is primarily meant for
		''' layout managers and GUI builders.
		''' <p>
		''' The default implementation returns
		''' <code>BaselineResizeBehavior.OTHER</code>, subclasses that support
		''' baseline should override appropriately.  Subclasses should
		''' never return <code>null</code>; if the baseline can not be
		''' calculated return <code>BaselineResizeBehavior.OTHER</code>.  Callers
		''' should first ask for the baseline using
		''' <code>getBaseline</code> and if a value &gt;= 0 is returned use
		''' this method.  It is acceptable for this method to return a
		''' value other than <code>BaselineResizeBehavior.OTHER</code> even if
		''' <code>getBaseline</code> returns a value less than 0.
		''' </summary>
		''' <param name="c"> <code>Component</code> to return baseline resize behavior for </param>
		''' <returns> an enum indicating how the baseline changes as the border is
		'''         resized </returns>
		''' <seealso cref= java.awt.Component#getBaseline(int,int) </seealso>
		''' <seealso cref= java.awt.Component#getBaselineResizeBehavior()
		''' @since 1.6 </seealso>
		Public Overridable Function getBaselineResizeBehavior(ByVal c As java.awt.Component) As java.awt.Component.BaselineResizeBehavior
			If c Is Nothing Then Throw New NullPointerException("Component must be non-null")
			Return java.awt.Component.BaselineResizeBehavior.OTHER
		End Function

	'    
	'     * Convenience function for determining ComponentOrientation.
	'     * Helps us avoid having Munge directives throughout the code.
	'     
		Friend Shared Function isLeftToRight(ByVal c As java.awt.Component) As Boolean
			Return c.componentOrientation.leftToRight
		End Function

	End Class

End Namespace