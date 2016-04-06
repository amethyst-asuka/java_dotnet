Imports System

'
' * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
	''' The {@code GridBagLayoutInfo} is an utility class for
	''' {@code GridBagLayout} layout manager.
	''' It stores align, size and baseline parameters for every component within a container.
	''' <p> </summary>
	''' <seealso cref=       java.awt.GridBagLayout </seealso>
	''' <seealso cref=       java.awt.GridBagConstraints
	''' @since 1.6 </seealso>
	<Serializable> _
	Public Class GridBagLayoutInfo
	'    
	'     * serialVersionUID
	'     
		Private Const serialVersionUID As Long = -4899416460737170217L

		Friend width, height As Integer ' number of  cells: horizontal and vertical
		Friend startx, starty As Integer ' starting point for layout
		Friend minWidth As Integer() ' largest minWidth in each column
		Friend minHeight As Integer() ' largest minHeight in each row
		Friend weightX As Double() ' largest weight in each column
		Friend weightY As Double() ' largest weight in each row
		Friend hasBaseline_Renamed As Boolean ' Whether or not baseline layout has been
	'                                 * requested and one of the components
	'                                 * has a valid baseline. 
		' These are only valid if hasBaseline is true and are indexed by
		' row.
		Friend baselineType As Short() ' The type of baseline for a particular
	'                                 * row.  A mix of the BaselineResizeBehavior
	'                                 * constants (1 << ordinal()) 
		Friend maxAscent As Integer() ' Max ascent (baseline).
		Friend maxDescent As Integer() ' Max descent (height - baseline)

		''' <summary>
		''' Creates an instance of GridBagLayoutInfo representing {@code GridBagLayout}
		''' grid cells with it's own parameters. </summary>
		''' <param name="width"> the columns </param>
		''' <param name="height"> the rows
		''' @since 6.0 </param>
		Friend Sub New(  width As Integer,   height As Integer)
			Me.width = width
			Me.height = height
		End Sub

		''' <summary>
		''' Returns true if the specified row has any component aligned on the
		''' baseline with a baseline resize behavior of CONSTANT_DESCENT.
		''' </summary>
		Friend Overridable Function hasConstantDescent(  row As Integer) As Boolean
			Return ((baselineType(row) And (1 << Component.BaselineResizeBehavior.CONSTANT_DESCENT.ordinal())) <> 0)
		End Function

		''' <summary>
		''' Returns true if there is a baseline for the specified row.
		''' </summary>
		Friend Overridable Function hasBaseline(  row As Integer) As Boolean
			Return (hasBaseline_Renamed AndAlso baselineType(row) <> 0)
		End Function
	End Class

End Namespace