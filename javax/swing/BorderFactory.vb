Imports javax.swing.border

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
Namespace javax.swing

	''' <summary>
	''' Factory class for vending standard <code>Border</code> objects.  Wherever
	''' possible, this factory will hand out references to shared
	''' <code>Border</code> instances.
	''' For further information and examples see
	''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/components/border.htmll">How
	''' to Use Borders</a>,
	''' a section in <em>The Java Tutorial</em>.
	''' 
	''' @author David Kloba
	''' </summary>
	Public Class BorderFactory

		''' <summary>
		''' Don't let anyone instantiate this class </summary>
		Private Sub New()
		End Sub


	'// LineBorder ///////////////////////////////////////////////////////////////
		''' <summary>
		''' Creates a line border withe the specified color.
		''' </summary>
		''' <param name="color">  a <code>Color</code> to use for the line </param>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createLineBorder(ByVal color As java.awt.Color) As Border
			Return New LineBorder(color, 1)
		End Function

		''' <summary>
		''' Creates a line border with the specified color
		''' and width. The width applies to all four sides of the
		''' border. To specify widths individually for the top,
		''' bottom, left, and right, use
		''' <seealso cref="#createMatteBorder(int,int,int,int,Color)"/>.
		''' </summary>
		''' <param name="color">  a <code>Color</code> to use for the line </param>
		''' <param name="thickness">  an integer specifying the width in pixels </param>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createLineBorder(ByVal color As java.awt.Color, ByVal thickness As Integer) As Border
			Return New LineBorder(color, thickness)
		End Function

		''' <summary>
		''' Creates a line border with the specified color, thickness, and corner shape.
		''' </summary>
		''' <param name="color">      the color of the border </param>
		''' <param name="thickness">  the thickness of the border </param>
		''' <param name="rounded">    whether or not border corners should be round </param>
		''' <returns> the {@code Border} object
		''' </returns>
		''' <seealso cref= LineBorder#LineBorder(Color, int, boolean)
		''' @since 1.7 </seealso>
		Public Shared Function createLineBorder(ByVal color As java.awt.Color, ByVal thickness As Integer, ByVal rounded As Boolean) As Border
			Return New LineBorder(color, thickness, rounded)
		End Function

	'// BevelBorder /////////////////////////////////////////////////////////////
	'/////////////////////////////////////////////////////////////////////////////
		Friend Shared ReadOnly sharedRaisedBevel As Border = New BevelBorder(BevelBorder.RAISED)
		Friend Shared ReadOnly sharedLoweredBevel As Border = New BevelBorder(BevelBorder.LOWERED)

		''' <summary>
		''' Creates a border with a raised beveled edge, using
		''' brighter shades of the component's current background color
		''' for highlighting, and darker shading for shadows.
		''' (In a raised border, highlights are on top and shadows
		'''  are underneath.)
		''' </summary>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createRaisedBevelBorder() As Border
			Return createSharedBevel(BevelBorder.RAISED)
		End Function

		''' <summary>
		''' Creates a border with a lowered beveled edge, using
		''' brighter shades of the component's current background color
		''' for highlighting, and darker shading for shadows.
		''' (In a lowered border, shadows are on top and highlights
		'''  are underneath.)
		''' </summary>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createLoweredBevelBorder() As Border
			Return createSharedBevel(BevelBorder.LOWERED)
		End Function

		''' <summary>
		''' Creates a beveled border of the specified type, using
		''' brighter shades of the component's current background color
		''' for highlighting, and darker shading for shadows.
		''' (In a lowered border, shadows are on top and highlights
		'''  are underneath.)
		''' </summary>
		''' <param name="type">  an integer specifying either
		'''                  <code>BevelBorder.LOWERED</code> or
		'''                  <code>BevelBorder.RAISED</code> </param>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createBevelBorder(ByVal type As Integer) As Border
			Return createSharedBevel(type)
		End Function

		''' <summary>
		''' Creates a beveled border of the specified type, using
		''' the specified highlighting and shadowing. The outer
		''' edge of the highlighted area uses a brighter shade of
		''' the highlight color. The inner edge of the shadow area
		''' uses a brighter shade of the shadow color.
		''' </summary>
		''' <param name="type">  an integer specifying either
		'''                  <code>BevelBorder.LOWERED</code> or
		'''                  <code>BevelBorder.RAISED</code> </param>
		''' <param name="highlight">  a <code>Color</code> object for highlights </param>
		''' <param name="shadow">     a <code>Color</code> object for shadows </param>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createBevelBorder(ByVal type As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color) As Border
			Return New BevelBorder(type, highlight, shadow)
		End Function

		''' <summary>
		''' Creates a beveled border of the specified type, using
		''' the specified colors for the inner and outer highlight
		''' and shadow areas.
		''' </summary>
		''' <param name="type">  an integer specifying either
		'''          <code>BevelBorder.LOWERED</code> or
		'''          <code>BevelBorder.RAISED</code> </param>
		''' <param name="highlightOuter">  a <code>Color</code> object for the
		'''                  outer edge of the highlight area </param>
		''' <param name="highlightInner">  a <code>Color</code> object for the
		'''                  inner edge of the highlight area </param>
		''' <param name="shadowOuter">     a <code>Color</code> object for the
		'''                  outer edge of the shadow area </param>
		''' <param name="shadowInner">     a <code>Color</code> object for the
		'''                  inner edge of the shadow area </param>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createBevelBorder(ByVal type As Integer, ByVal highlightOuter As java.awt.Color, ByVal highlightInner As java.awt.Color, ByVal shadowOuter As java.awt.Color, ByVal shadowInner As java.awt.Color) As Border
			Return New BevelBorder(type, highlightOuter, highlightInner, shadowOuter, shadowInner)
		End Function

		Friend Shared Function createSharedBevel(ByVal type As Integer) As Border
			If type = BevelBorder.RAISED Then
				Return sharedRaisedBevel
			ElseIf type = BevelBorder.LOWERED Then
				Return sharedLoweredBevel
			End If
			Return Nothing
		End Function

	'// SoftBevelBorder ///////////////////////////////////////////////////////////
	'//////////////////////////////////////////////////////////////////////////////

		Private Shared sharedSoftRaisedBevel As Border
		Private Shared sharedSoftLoweredBevel As Border

		''' <summary>
		''' Creates a beveled border with a raised edge and softened corners,
		''' using brighter shades of the component's current background color
		''' for highlighting, and darker shading for shadows.
		''' In a raised border, highlights are on top and shadows are underneath.
		''' </summary>
		''' <returns> the {@code Border} object
		''' 
		''' @since 1.7 </returns>
		Public Shared Function createRaisedSoftBevelBorder() As Border
			If sharedSoftRaisedBevel Is Nothing Then sharedSoftRaisedBevel = New SoftBevelBorder(BevelBorder.RAISED)
			Return sharedSoftRaisedBevel
		End Function

		''' <summary>
		''' Creates a beveled border with a lowered edge and softened corners,
		''' using brighter shades of the component's current background color
		''' for highlighting, and darker shading for shadows.
		''' In a lowered border, shadows are on top and highlights are underneath.
		''' </summary>
		''' <returns> the {@code Border} object
		''' 
		''' @since 1.7 </returns>
		Public Shared Function createLoweredSoftBevelBorder() As Border
			If sharedSoftLoweredBevel Is Nothing Then sharedSoftLoweredBevel = New SoftBevelBorder(BevelBorder.LOWERED)
			Return sharedSoftLoweredBevel
		End Function

		''' <summary>
		''' Creates a beveled border of the specified type with softened corners,
		''' using brighter shades of the component's current background color
		''' for highlighting, and darker shading for shadows.
		''' The type is either <seealso cref="BevelBorder#RAISED"/> or <seealso cref="BevelBorder#LOWERED"/>.
		''' </summary>
		''' <param name="type">  a type of a bevel </param>
		''' <returns> the {@code Border} object or {@code null}
		'''         if the specified type is not valid
		''' </returns>
		''' <seealso cref= BevelBorder#BevelBorder(int)
		''' @since 1.7 </seealso>
		Public Shared Function createSoftBevelBorder(ByVal type As Integer) As Border
			If type = BevelBorder.RAISED Then Return createRaisedSoftBevelBorder()
			If type = BevelBorder.LOWERED Then Return createLoweredSoftBevelBorder()
			Return Nothing
		End Function

		''' <summary>
		''' Creates a beveled border of the specified type with softened corners,
		''' using the specified highlighting and shadowing.
		''' The type is either <seealso cref="BevelBorder#RAISED"/> or <seealso cref="BevelBorder#LOWERED"/>.
		''' The outer edge of the highlight area uses
		''' a brighter shade of the {@code highlight} color.
		''' The inner edge of the shadow area uses
		''' a brighter shade of the {@code shadow} color.
		''' </summary>
		''' <param name="type">       a type of a bevel </param>
		''' <param name="highlight">  a basic color of the highlight area </param>
		''' <param name="shadow">     a basic color of the shadow area </param>
		''' <returns> the {@code Border} object
		''' </returns>
		''' <seealso cref= BevelBorder#BevelBorder(int, Color, Color)
		''' @since 1.7 </seealso>
		Public Shared Function createSoftBevelBorder(ByVal type As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color) As Border
			Return New SoftBevelBorder(type, highlight, shadow)
		End Function

		''' <summary>
		''' Creates a beveled border of the specified type with softened corners,
		''' using the specified colors for the inner and outer edges
		''' of the highlight and the shadow areas.
		''' The type is either <seealso cref="BevelBorder#RAISED"/> or <seealso cref="BevelBorder#LOWERED"/>.
		''' Note: The shadow inner and outer colors are switched
		''' for a lowered bevel border.
		''' </summary>
		''' <param name="type">            a type of a bevel </param>
		''' <param name="highlightOuter">  a color of the outer edge of the highlight area </param>
		''' <param name="highlightInner">  a color of the inner edge of the highlight area </param>
		''' <param name="shadowOuter">     a color of the outer edge of the shadow area </param>
		''' <param name="shadowInner">     a color of the inner edge of the shadow area </param>
		''' <returns> the {@code Border} object
		''' </returns>
		''' <seealso cref= BevelBorder#BevelBorder(int, Color, Color, Color, Color)
		''' @since 1.7 </seealso>
		Public Shared Function createSoftBevelBorder(ByVal type As Integer, ByVal highlightOuter As java.awt.Color, ByVal highlightInner As java.awt.Color, ByVal shadowOuter As java.awt.Color, ByVal shadowInner As java.awt.Color) As Border
			Return New SoftBevelBorder(type, highlightOuter, highlightInner, shadowOuter, shadowInner)
		End Function

	'// EtchedBorder ///////////////////////////////////////////////////////////

		Friend Shared ReadOnly sharedEtchedBorder As Border = New EtchedBorder
		Private Shared sharedRaisedEtchedBorder As Border

		''' <summary>
		''' Creates a border with an "etched" look using
		''' the component's current background color for
		''' highlighting and shading.
		''' </summary>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createEtchedBorder() As Border
			Return sharedEtchedBorder
		End Function

		''' <summary>
		''' Creates a border with an "etched" look using
		''' the specified highlighting and shading colors.
		''' </summary>
		''' <param name="highlight">  a <code>Color</code> object for the border highlights </param>
		''' <param name="shadow">     a <code>Color</code> object for the border shadows </param>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createEtchedBorder(ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color) As Border
			Return New EtchedBorder(highlight, shadow)
		End Function

		''' <summary>
		''' Creates a border with an "etched" look using
		''' the component's current background color for
		''' highlighting and shading.
		''' </summary>
		''' <param name="type">      one of <code>EtchedBorder.RAISED</code>, or
		'''                  <code>EtchedBorder.LOWERED</code> </param>
		''' <returns> the <code>Border</code> object </returns>
		''' <exception cref="IllegalArgumentException"> if type is not either
		'''                  <code>EtchedBorder.RAISED</code> or
		'''                  <code>EtchedBorder.LOWERED</code>
		''' @since 1.3 </exception>
		Public Shared Function createEtchedBorder(ByVal type As Integer) As Border
			Select Case type
			Case EtchedBorder.RAISED
				If sharedRaisedEtchedBorder Is Nothing Then sharedRaisedEtchedBorder = New EtchedBorder(EtchedBorder.RAISED)
				Return sharedRaisedEtchedBorder
			Case EtchedBorder.LOWERED
				Return sharedEtchedBorder
			Case Else
				Throw New System.ArgumentException("type must be one of EtchedBorder.RAISED or EtchedBorder.LOWERED")
			End Select
		End Function

		''' <summary>
		''' Creates a border with an "etched" look using
		''' the specified highlighting and shading colors.
		''' </summary>
		''' <param name="type">      one of <code>EtchedBorder.RAISED</code>, or
		'''                  <code>EtchedBorder.LOWERED</code> </param>
		''' <param name="highlight">  a <code>Color</code> object for the border highlights </param>
		''' <param name="shadow">     a <code>Color</code> object for the border shadows </param>
		''' <returns> the <code>Border</code> object
		''' @since 1.3 </returns>
		Public Shared Function createEtchedBorder(ByVal type As Integer, ByVal highlight As java.awt.Color, ByVal shadow As java.awt.Color) As Border
			Return New EtchedBorder(type, highlight, shadow)
		End Function

	'// TitledBorder ////////////////////////////////////////////////////////////
		''' <summary>
		''' Creates a new titled border with the specified title,
		''' the default border type (determined by the current look and feel),
		''' the default text position (determined by the current look and feel),
		''' the default justification (leading), and the default
		''' font and text color (determined by the current look and feel).
		''' </summary>
		''' <param name="title">      a <code>String</code> containing the text of the title </param>
		''' <returns> the <code>TitledBorder</code> object </returns>
		Public Shared Function createTitledBorder(ByVal title As String) As TitledBorder
			Return New TitledBorder(title)
		End Function

		''' <summary>
		''' Creates a new titled border with an empty title,
		''' the specified border object,
		''' the default text position (determined by the current look and feel),
		''' the default justification (leading), and the default
		''' font and text color (determined by the current look and feel).
		''' </summary>
		''' <param name="border">     the <code>Border</code> object to add the title to; if
		'''                   <code>null</code> the <code>Border</code> is determined
		'''                   by the current look and feel. </param>
		''' <returns> the <code>TitledBorder</code> object </returns>
		Public Shared Function createTitledBorder(ByVal border As Border) As TitledBorder
			Return New TitledBorder(border)
		End Function

		''' <summary>
		''' Adds a title to an existing border,
		''' with default positioning (determined by the current look and feel),
		''' default justification (leading) and the default
		''' font and text color (determined by the current look and feel).
		''' </summary>
		''' <param name="border">     the <code>Border</code> object to add the title to </param>
		''' <param name="title">      a <code>String</code> containing the text of the title </param>
		''' <returns> the <code>TitledBorder</code> object </returns>
		Public Shared Function createTitledBorder(ByVal border As Border, ByVal title As String) As TitledBorder
			Return New TitledBorder(border, title)
		End Function

		''' <summary>
		''' Adds a title to an existing border, with the specified
		''' positioning and using the default
		''' font and text color (determined by the current look and feel).
		''' </summary>
		''' <param name="border">      the <code>Border</code> object to add the title to </param>
		''' <param name="title">       a <code>String</code> containing the text of the title </param>
		''' <param name="titleJustification">  an integer specifying the justification
		'''        of the title -- one of the following:
		''' <ul>
		''' <li><code>TitledBorder.LEFT</code>
		''' <li><code>TitledBorder.CENTER</code>
		''' <li><code>TitledBorder.RIGHT</code>
		''' <li><code>TitledBorder.LEADING</code>
		''' <li><code>TitledBorder.TRAILING</code>
		''' <li><code>TitledBorder.DEFAULT_JUSTIFICATION</code> (leading)
		''' </ul> </param>
		''' <param name="titlePosition">       an integer specifying the vertical position of
		'''        the text in relation to the border -- one of the following:
		''' <ul>
		''' <li><code> TitledBorder.ABOVE_TOP</code>
		''' <li><code>TitledBorder.TOP</code> (sitting on the top line)
		''' <li><code>TitledBorder.BELOW_TOP</code>
		''' <li><code>TitledBorder.ABOVE_BOTTOM</code>
		''' <li><code>TitledBorder.BOTTOM</code> (sitting on the bottom line)
		''' <li><code>TitledBorder.BELOW_BOTTOM</code>
		''' <li><code>TitledBorder.DEFAULT_POSITION</code> (the title position
		'''  is determined by the current look and feel)
		''' </ul> </param>
		''' <returns> the <code>TitledBorder</code> object </returns>
		Public Shared Function createTitledBorder(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer) As TitledBorder
			Return New TitledBorder(border, title, titleJustification, titlePosition)
		End Function

		''' <summary>
		''' Adds a title to an existing border, with the specified
		''' positioning and font, and using the default text color
		''' (determined by the current look and feel).
		''' </summary>
		''' <param name="border">      the <code>Border</code> object to add the title to </param>
		''' <param name="title">       a <code>String</code> containing the text of the title </param>
		''' <param name="titleJustification">  an integer specifying the justification
		'''        of the title -- one of the following:
		''' <ul>
		''' <li><code>TitledBorder.LEFT</code>
		''' <li><code>TitledBorder.CENTER</code>
		''' <li><code>TitledBorder.RIGHT</code>
		''' <li><code>TitledBorder.LEADING</code>
		''' <li><code>TitledBorder.TRAILING</code>
		''' <li><code>TitledBorder.DEFAULT_JUSTIFICATION</code> (leading)
		''' </ul> </param>
		''' <param name="titlePosition">       an integer specifying the vertical position of
		'''        the text in relation to the border -- one of the following:
		''' <ul>
		''' <li><code> TitledBorder.ABOVE_TOP</code>
		''' <li><code>TitledBorder.TOP</code> (sitting on the top line)
		''' <li><code>TitledBorder.BELOW_TOP</code>
		''' <li><code>TitledBorder.ABOVE_BOTTOM</code>
		''' <li><code>TitledBorder.BOTTOM</code> (sitting on the bottom line)
		''' <li><code>TitledBorder.BELOW_BOTTOM</code>
		''' <li><code>TitledBorder.DEFAULT_POSITION</code> (the title position
		'''  is determined by the current look and feel)
		''' </ul> </param>
		''' <param name="titleFont">           a Font object specifying the title font </param>
		''' <returns> the TitledBorder object </returns>
		Public Shared Function createTitledBorder(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer, ByVal titleFont As java.awt.Font) As TitledBorder
			Return New TitledBorder(border, title, titleJustification, titlePosition, titleFont)
		End Function

		''' <summary>
		''' Adds a title to an existing border, with the specified
		''' positioning, font and color.
		''' </summary>
		''' <param name="border">      the <code>Border</code> object to add the title to </param>
		''' <param name="title">       a <code>String</code> containing the text of the title </param>
		''' <param name="titleJustification">  an integer specifying the justification
		'''        of the title -- one of the following:
		''' <ul>
		''' <li><code>TitledBorder.LEFT</code>
		''' <li><code>TitledBorder.CENTER</code>
		''' <li><code>TitledBorder.RIGHT</code>
		''' <li><code>TitledBorder.LEADING</code>
		''' <li><code>TitledBorder.TRAILING</code>
		''' <li><code>TitledBorder.DEFAULT_JUSTIFICATION</code> (leading)
		''' </ul> </param>
		''' <param name="titlePosition">       an integer specifying the vertical position of
		'''        the text in relation to the border -- one of the following:
		''' <ul>
		''' <li><code> TitledBorder.ABOVE_TOP</code>
		''' <li><code>TitledBorder.TOP</code> (sitting on the top line)
		''' <li><code>TitledBorder.BELOW_TOP</code>
		''' <li><code>TitledBorder.ABOVE_BOTTOM</code>
		''' <li><code>TitledBorder.BOTTOM</code> (sitting on the bottom line)
		''' <li><code>TitledBorder.BELOW_BOTTOM</code>
		''' <li><code>TitledBorder.DEFAULT_POSITION</code> (the title position
		'''  is determined by the current look and feel)
		''' </ul> </param>
		''' <param name="titleFont">   a <code>Font</code> object specifying the title font </param>
		''' <param name="titleColor">  a <code>Color</code> object specifying the title color </param>
		''' <returns> the <code>TitledBorder</code> object </returns>
		Public Shared Function createTitledBorder(ByVal border As Border, ByVal title As String, ByVal titleJustification As Integer, ByVal titlePosition As Integer, ByVal titleFont As java.awt.Font, ByVal titleColor As java.awt.Color) As TitledBorder
			Return New TitledBorder(border, title, titleJustification, titlePosition, titleFont, titleColor)
		End Function
	'// EmptyBorder ///////////////////////////////////////////////////////////
		Friend Shared ReadOnly emptyBorder As Border = New EmptyBorder(0, 0, 0, 0)

		''' <summary>
		''' Creates an empty border that takes up no space. (The width
		''' of the top, bottom, left, and right sides are all zero.)
		''' </summary>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createEmptyBorder() As Border
			Return emptyBorder
		End Function

		''' <summary>
		''' Creates an empty border that takes up space but which does
		''' no drawing, specifying the width of the top, left, bottom, and
		''' right sides.
		''' </summary>
		''' <param name="top">     an integer specifying the width of the top,
		'''                  in pixels </param>
		''' <param name="left">    an integer specifying the width of the left side,
		'''                  in pixels </param>
		''' <param name="bottom">  an integer specifying the width of the bottom,
		'''                  in pixels </param>
		''' <param name="right">   an integer specifying the width of the right side,
		'''                  in pixels </param>
		''' <returns> the <code>Border</code> object </returns>
		Public Shared Function createEmptyBorder(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer) As Border
			Return New EmptyBorder(top, left, bottom, right)
		End Function

	'// CompoundBorder ////////////////////////////////////////////////////////
		''' <summary>
		''' Creates a compound border with a <code>null</code> inside edge and a
		''' <code>null</code> outside edge.
		''' </summary>
		''' <returns> the <code>CompoundBorder</code> object </returns>
		Public Shared Function createCompoundBorder() As CompoundBorder
			Return New CompoundBorder
		End Function

		''' <summary>
		''' Creates a compound border specifying the border objects to use
		''' for the outside and inside edges.
		''' </summary>
		''' <param name="outsideBorder">  a <code>Border</code> object for the outer
		'''                          edge of the compound border </param>
		''' <param name="insideBorder">   a <code>Border</code> object for the inner
		'''                          edge of the compound border </param>
		''' <returns> the <code>CompoundBorder</code> object </returns>
		Public Shared Function createCompoundBorder(ByVal outsideBorder As Border, ByVal insideBorder As Border) As CompoundBorder
			Return New CompoundBorder(outsideBorder, insideBorder)
		End Function

	'// MatteBorder ////////////////////////////////////////////////////////
		''' <summary>
		''' Creates a matte-look border using a solid color. (The difference between
		''' this border and a line border is that you can specify the individual
		''' border dimensions.)
		''' </summary>
		''' <param name="top">     an integer specifying the width of the top,
		'''                          in pixels </param>
		''' <param name="left">    an integer specifying the width of the left side,
		'''                          in pixels </param>
		''' <param name="bottom">  an integer specifying the width of the right side,
		'''                          in pixels </param>
		''' <param name="right">   an integer specifying the width of the bottom,
		'''                          in pixels </param>
		''' <param name="color">   a <code>Color</code> to use for the border </param>
		''' <returns> the <code>MatteBorder</code> object </returns>
		Public Shared Function createMatteBorder(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer, ByVal color As java.awt.Color) As MatteBorder
			Return New MatteBorder(top, left, bottom, right, color)
		End Function

		''' <summary>
		''' Creates a matte-look border that consists of multiple tiles of a
		''' specified icon. Multiple copies of the icon are placed side-by-side
		''' to fill up the border area.
		''' <p>
		''' Note:<br>
		''' If the icon doesn't load, the border area is painted gray.
		''' </summary>
		''' <param name="top">     an integer specifying the width of the top,
		'''                          in pixels </param>
		''' <param name="left">    an integer specifying the width of the left side,
		'''                          in pixels </param>
		''' <param name="bottom">  an integer specifying the width of the right side,
		'''                          in pixels </param>
		''' <param name="right">   an integer specifying the width of the bottom,
		'''                          in pixels </param>
		''' <param name="tileIcon">  the <code>Icon</code> object used for the border tiles </param>
		''' <returns> the <code>MatteBorder</code> object </returns>
		Public Shared Function createMatteBorder(ByVal top As Integer, ByVal left As Integer, ByVal bottom As Integer, ByVal right As Integer, ByVal tileIcon As Icon) As MatteBorder
			Return New MatteBorder(top, left, bottom, right, tileIcon)
		End Function

	'// StrokeBorder //////////////////////////////////////////////////////////////
	'//////////////////////////////////////////////////////////////////////////////

		''' <summary>
		''' Creates a border of the specified {@code stroke}.
		''' The component's foreground color will be used to render the border.
		''' </summary>
		''' <param name="stroke">  the <seealso cref="BasicStroke"/> object used to stroke a shape </param>
		''' <returns> the {@code Border} object
		''' </returns>
		''' <exception cref="NullPointerException"> if the specified {@code stroke} is {@code null}
		''' 
		''' @since 1.7 </exception>
		Public Shared Function createStrokeBorder(ByVal stroke As java.awt.BasicStroke) As Border
			Return New StrokeBorder(stroke)
		End Function

		''' <summary>
		''' Creates a border of the specified {@code stroke} and {@code paint}.
		''' If the specified {@code paint} is {@code null},
		''' the component's foreground color will be used to render the border.
		''' </summary>
		''' <param name="stroke">  the <seealso cref="BasicStroke"/> object used to stroke a shape </param>
		''' <param name="paint">   the <seealso cref="Paint"/> object used to generate a color </param>
		''' <returns> the {@code Border} object
		''' </returns>
		''' <exception cref="NullPointerException"> if the specified {@code stroke} is {@code null}
		''' 
		''' @since 1.7 </exception>
		Public Shared Function createStrokeBorder(ByVal stroke As java.awt.BasicStroke, ByVal paint As java.awt.Paint) As Border
			Return New StrokeBorder(stroke, paint)
		End Function

	'// DashedBorder //////////////////////////////////////////////////////////////
	'//////////////////////////////////////////////////////////////////////////////

		Private Shared sharedDashedBorder As Border

		''' <summary>
		''' Creates a dashed border of the specified {@code paint}.
		''' If the specified {@code paint} is {@code null},
		''' the component's foreground color will be used to render the border.
		''' The width of a dash line is equal to {@code 1}.
		''' The relative length of a dash line and
		''' the relative spacing between dash lines are equal to {@code 1}.
		''' A dash line is not rounded.
		''' </summary>
		''' <param name="paint">  the <seealso cref="Paint"/> object used to generate a color </param>
		''' <returns> the {@code Border} object
		''' 
		''' @since 1.7 </returns>
		Public Shared Function createDashedBorder(ByVal paint As java.awt.Paint) As Border
			Return createDashedBorder(paint, 1.0f, 1.0f, 1.0f, False)
		End Function

		''' <summary>
		''' Creates a dashed border of the specified {@code paint},
		''' relative {@code length}, and relative {@code spacing}.
		''' If the specified {@code paint} is {@code null},
		''' the component's foreground color will be used to render the border.
		''' The width of a dash line is equal to {@code 1}.
		''' A dash line is not rounded.
		''' </summary>
		''' <param name="paint">    the <seealso cref="Paint"/> object used to generate a color </param>
		''' <param name="length">   the relative length of a dash line </param>
		''' <param name="spacing">  the relative spacing between dash lines </param>
		''' <returns> the {@code Border} object
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if the specified {@code length} is less than {@code 1}, or
		'''                                  if the specified {@code spacing} is less than {@code 0}
		''' @since 1.7 </exception>
		Public Shared Function createDashedBorder(ByVal paint As java.awt.Paint, ByVal length As Single, ByVal spacing As Single) As Border
			Return createDashedBorder(paint, 1.0f, length, spacing, False)
		End Function

		''' <summary>
		''' Creates a dashed border of the specified {@code paint}, {@code thickness},
		''' line shape, relative {@code length}, and relative {@code spacing}.
		''' If the specified {@code paint} is {@code null},
		''' the component's foreground color will be used to render the border.
		''' </summary>
		''' <param name="paint">      the <seealso cref="Paint"/> object used to generate a color </param>
		''' <param name="thickness">  the width of a dash line </param>
		''' <param name="length">     the relative length of a dash line </param>
		''' <param name="spacing">    the relative spacing between dash lines </param>
		''' <param name="rounded">    whether or not line ends should be round </param>
		''' <returns> the {@code Border} object
		''' </returns>
		''' <exception cref="IllegalArgumentException"> if the specified {@code thickness} is less than {@code 1}, or
		'''                                  if the specified {@code length} is less than {@code 1}, or
		'''                                  if the specified {@code spacing} is less than {@code 0}
		''' @since 1.7 </exception>
		Public Shared Function createDashedBorder(ByVal paint As java.awt.Paint, ByVal thickness As Single, ByVal length As Single, ByVal spacing As Single, ByVal rounded As Boolean) As Border
			Dim [shared] As Boolean = (Not rounded) AndAlso (paint Is Nothing) AndAlso (thickness = 1.0f) AndAlso (length = 1.0f) AndAlso (spacing = 1.0f)
			If [shared] AndAlso (sharedDashedBorder IsNot Nothing) Then Return sharedDashedBorder
			If thickness < 1.0f Then Throw New System.ArgumentException("thickness is less than 1")
			If length < 1.0f Then Throw New System.ArgumentException("length is less than 1")
			If spacing < 0.0f Then Throw New System.ArgumentException("spacing is less than 0")
			Dim cap As Integer = If(rounded, java.awt.BasicStroke.CAP_ROUND, java.awt.BasicStroke.CAP_SQUARE)
			Dim join As Integer = If(rounded, java.awt.BasicStroke.JOIN_ROUND, java.awt.BasicStroke.JOIN_MITER)
			Dim array As Single() = { thickness * (length - 1.0f), thickness * (spacing + 1.0f) }
			Dim border As Border = createStrokeBorder(New java.awt.BasicStroke(thickness, cap, join, thickness * 2.0f, array, 0.0f), paint)
			If [shared] Then sharedDashedBorder = border
			Return border
		End Function
	End Class

End Namespace