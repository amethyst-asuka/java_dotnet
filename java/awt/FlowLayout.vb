Imports System

'
' * Copyright (c) 1995, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' A flow layout arranges components in a directional flow, much
	''' like lines of text in a paragraph. The flow direction is
	''' determined by the container's <code>componentOrientation</code>
	''' property and may be one of two values:
	''' <ul>
	''' <li><code>ComponentOrientation.LEFT_TO_RIGHT</code>
	''' <li><code>ComponentOrientation.RIGHT_TO_LEFT</code>
	''' </ul>
	''' Flow layouts are typically used
	''' to arrange buttons in a panel. It arranges buttons
	''' horizontally until no more buttons fit on the same line.
	''' The line alignment is determined by the <code>align</code>
	''' property. The possible values are:
	''' <ul>
	''' <li><seealso cref="#LEFT LEFT"/>
	''' <li><seealso cref="#RIGHT RIGHT"/>
	''' <li><seealso cref="#CENTER CENTER"/>
	''' <li><seealso cref="#LEADING LEADING"/>
	''' <li><seealso cref="#TRAILING TRAILING"/>
	''' </ul>
	''' <p>
	''' For example, the following picture shows an applet using the flow
	''' layout manager (its default layout manager) to position three buttons:
	''' <p>
	''' <img src="doc-files/FlowLayout-1.gif"
	''' ALT="Graphic of Layout for Three Buttons"
	''' style="float:center; margin: 7px 10px;">
	''' <p>
	''' Here is the code for this applet:
	''' 
	''' <hr><blockquote><pre>
	''' import java.awt.*;
	''' import java.applet.Applet;
	''' 
	''' public class myButtons extends Applet {
	'''     Button button1, button2, button3;
	'''     public  Sub  init() {
	'''         button1 = new Button("Ok");
	'''         button2 = new Button("Open");
	'''         button3 = new Button("Close");
	'''         add(button1);
	'''         add(button2);
	'''         add(button3);
	'''     }
	''' }
	''' </pre></blockquote><hr>
	''' <p>
	''' A flow layout lets each component assume its natural (preferred) size.
	''' 
	''' @author      Arthur van Hoff
	''' @author      Sami Shaio
	''' @since       JDK1.0 </summary>
	''' <seealso cref= ComponentOrientation </seealso>
	<Serializable> _
	Public Class FlowLayout
		Implements LayoutManager

		''' <summary>
		''' This value indicates that each row of components
		''' should be left-justified.
		''' </summary>
		Public Const LEFT As Integer = 0

		''' <summary>
		''' This value indicates that each row of components
		''' should be centered.
		''' </summary>
		Public Const CENTER As Integer = 1

		''' <summary>
		''' This value indicates that each row of components
		''' should be right-justified.
		''' </summary>
		Public Const RIGHT As Integer = 2

		''' <summary>
		''' This value indicates that each row of components
		''' should be justified to the leading edge of the container's
		''' orientation, for example, to the left in left-to-right orientations.
		''' </summary>
		''' <seealso cref=     java.awt.Component#getComponentOrientation </seealso>
		''' <seealso cref=     java.awt.ComponentOrientation
		''' @since   1.2 </seealso>
		Public Const LEADING As Integer = 3

		''' <summary>
		''' This value indicates that each row of components
		''' should be justified to the trailing edge of the container's
		''' orientation, for example, to the right in left-to-right orientations.
		''' </summary>
		''' <seealso cref=     java.awt.Component#getComponentOrientation </seealso>
		''' <seealso cref=     java.awt.ComponentOrientation
		''' @since   1.2 </seealso>
		Public Const TRAILING As Integer = 4

		''' <summary>
		''' <code>align</code> is the property that determines
		''' how each row distributes empty space.
		''' It can be one of the following values:
		''' <ul>
		''' <li><code>LEFT</code>
		''' <li><code>RIGHT</code>
		''' <li><code>CENTER</code>
		''' </ul>
		''' 
		''' @serial </summary>
		''' <seealso cref= #getAlignment </seealso>
		''' <seealso cref= #setAlignment </seealso>
		Friend align As Integer ' This is for 1.1 serialization compatibility

		''' <summary>
		''' <code>newAlign</code> is the property that determines
		''' how each row distributes empty space for the Java 2 platform,
		''' v1.2 and greater.
		''' It can be one of the following three values:
		''' <ul>
		''' <li><code>LEFT</code>
		''' <li><code>RIGHT</code>
		''' <li><code>CENTER</code>
		''' <li><code>LEADING</code>
		''' <li><code>TRAILING</code>
		''' </ul>
		''' 
		''' @serial
		''' @since 1.2 </summary>
		''' <seealso cref= #getAlignment </seealso>
		''' <seealso cref= #setAlignment </seealso>
		Friend newAlign As Integer ' This is the one we actually use

		''' <summary>
		''' The flow layout manager allows a seperation of
		''' components with gaps.  The horizontal gap will
		''' specify the space between components and between
		''' the components and the borders of the
		''' <code>Container</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getHgap() </seealso>
		''' <seealso cref= #setHgap(int) </seealso>
		Friend hgap As Integer

		''' <summary>
		''' The flow layout manager allows a seperation of
		''' components with gaps.  The vertical gap will
		''' specify the space between rows and between the
		''' the rows and the borders of the <code>Container</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getHgap() </seealso>
		''' <seealso cref= #setHgap(int) </seealso>
		Friend vgap As Integer

		''' <summary>
		''' If true, components will be aligned on their baseline.
		''' </summary>
		Private alignOnBaseline As Boolean

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		 Private Const serialVersionUID As Long = -7262534875583282631L

		''' <summary>
		''' Constructs a new <code>FlowLayout</code> with a centered alignment and a
		''' default 5-unit horizontal and vertical gap.
		''' </summary>
		Public Sub New()
			Me.New(CENTER, 5, 5)
		End Sub

		''' <summary>
		''' Constructs a new <code>FlowLayout</code> with the specified
		''' alignment and a default 5-unit horizontal and vertical gap.
		''' The value of the alignment argument must be one of
		''' <code>FlowLayout.LEFT</code>, <code>FlowLayout.RIGHT</code>,
		''' <code>FlowLayout.CENTER</code>, <code>FlowLayout.LEADING</code>,
		''' or <code>FlowLayout.TRAILING</code>. </summary>
		''' <param name="align"> the alignment value </param>
		Public Sub New(  align As Integer)
			Me.New(align, 5, 5)
		End Sub

		''' <summary>
		''' Creates a new flow layout manager with the indicated alignment
		''' and the indicated horizontal and vertical gaps.
		''' <p>
		''' The value of the alignment argument must be one of
		''' <code>FlowLayout.LEFT</code>, <code>FlowLayout.RIGHT</code>,
		''' <code>FlowLayout.CENTER</code>, <code>FlowLayout.LEADING</code>,
		''' or <code>FlowLayout.TRAILING</code>. </summary>
		''' <param name="align">   the alignment value </param>
		''' <param name="hgap">    the horizontal gap between components
		'''                     and between the components and the
		'''                     borders of the <code>Container</code> </param>
		''' <param name="vgap">    the vertical gap between components
		'''                     and between the components and the
		'''                     borders of the <code>Container</code> </param>
		Public Sub New(  align As Integer,   hgap As Integer,   vgap As Integer)
			Me.hgap = hgap
			Me.vgap = vgap
			alignment = align
		End Sub

		''' <summary>
		''' Gets the alignment for this layout.
		''' Possible values are <code>FlowLayout.LEFT</code>,
		''' <code>FlowLayout.RIGHT</code>, <code>FlowLayout.CENTER</code>,
		''' <code>FlowLayout.LEADING</code>,
		''' or <code>FlowLayout.TRAILING</code>. </summary>
		''' <returns>     the alignment value for this layout </returns>
		''' <seealso cref=        java.awt.FlowLayout#setAlignment
		''' @since      JDK1.1 </seealso>
		Public Overridable Property alignment As Integer
			Get
				Return newAlign
			End Get
			Set(  align As Integer)
				Me.newAlign = align
    
				' this.align is used only for serialization compatibility,
				' so set it to a value compatible with the 1.1 version
				' of the class
    
				Select Case align
				Case LEADING
					Me.align = LEFT
				Case TRAILING
					Me.align = RIGHT
				Case Else
					Me.align = align
				End Select
			End Set
		End Property


		''' <summary>
		''' Gets the horizontal gap between components
		''' and between the components and the borders
		''' of the <code>Container</code>
		''' </summary>
		''' <returns>     the horizontal gap between components
		'''             and between the components and the borders
		'''             of the <code>Container</code> </returns>
		''' <seealso cref=        java.awt.FlowLayout#setHgap
		''' @since      JDK1.1 </seealso>
		Public Overridable Property hgap As Integer
			Get
				Return hgap
			End Get
			Set(  hgap As Integer)
				Me.hgap = hgap
			End Set
		End Property


		''' <summary>
		''' Gets the vertical gap between components and
		''' between the components and the borders of the
		''' <code>Container</code>.
		''' </summary>
		''' <returns>     the vertical gap between components
		'''             and between the components and the borders
		'''             of the <code>Container</code> </returns>
		''' <seealso cref=        java.awt.FlowLayout#setVgap
		''' @since      JDK1.1 </seealso>
		Public Overridable Property vgap As Integer
			Get
				Return vgap
			End Get
			Set(  vgap As Integer)
				Me.vgap = vgap
			End Set
		End Property


		''' <summary>
		''' Sets whether or not components should be vertically aligned along their
		''' baseline.  Components that do not have a baseline will be centered.
		''' The default is false.
		''' </summary>
		''' <param name="alignOnBaseline"> whether or not components should be
		'''                        vertically aligned on their baseline
		''' @since 1.6 </param>
		Public Overridable Property alignOnBaseline As Boolean
			Set(  alignOnBaseline As Boolean)
				Me.alignOnBaseline = alignOnBaseline
			End Set
			Get
				Return alignOnBaseline
			End Get
		End Property


		''' <summary>
		''' Adds the specified component to the layout.
		''' Not used by this class. </summary>
		''' <param name="name"> the name of the component </param>
		''' <param name="comp"> the component to be added </param>
		Public Overridable Sub addLayoutComponent(  name As String,   comp As Component) Implements LayoutManager.addLayoutComponent
		End Sub

		''' <summary>
		''' Removes the specified component from the layout.
		''' Not used by this class. </summary>
		''' <param name="comp"> the component to remove </param>
		''' <seealso cref=       java.awt.Container#removeAll </seealso>
		Public Overridable Sub removeLayoutComponent(  comp As Component) Implements LayoutManager.removeLayoutComponent
		End Sub

		''' <summary>
		''' Returns the preferred dimensions for this layout given the
		''' <i>visible</i> components in the specified target container.
		''' </summary>
		''' <param name="target"> the container that needs to be laid out </param>
		''' <returns>    the preferred dimensions to lay out the
		'''            subcomponents of the specified container </returns>
		''' <seealso cref= Container </seealso>
		''' <seealso cref= #minimumLayoutSize </seealso>
		''' <seealso cref=       java.awt.Container#getPreferredSize </seealso>
		Public Overridable Function preferredLayoutSize(  target As Container) As Dimension Implements LayoutManager.preferredLayoutSize
		  SyncLock target.treeLock
			Dim [dim] As New Dimension(0, 0)
			Dim nmembers As Integer = target.componentCount
			Dim firstVisibleComponent As Boolean = True
			Dim useBaseline As Boolean = alignOnBaseline
			Dim maxAscent As Integer = 0
			Dim maxDescent As Integer = 0

			For i As Integer = 0 To nmembers - 1
				Dim m As Component = target.getComponent(i)
				If m.visible Then
					Dim d As Dimension = m.preferredSize
					[dim].height = System.Math.Max([dim].height, d.height)
					If firstVisibleComponent Then
						firstVisibleComponent = False
					Else
						[dim].width += hgap
					End If
					[dim].width += d.width
					If useBaseline Then
						Dim baseline As Integer = m.getBaseline(d.width, d.height)
						If baseline >= 0 Then
							maxAscent = System.Math.Max(maxAscent, baseline)
							maxDescent = System.Math.Max(maxDescent, d.height - baseline)
						End If
					End If
				End If
			Next i
			If useBaseline Then [dim].height = System.Math.Max(maxAscent + maxDescent, [dim].height)
			Dim insets_Renamed As Insets = target.insets
			[dim].width += insets_Renamed.left + insets_Renamed.right + hgap*2
			[dim].height += insets_Renamed.top + insets_Renamed.bottom + vgap*2
			Return [dim]
		  End SyncLock
		End Function

		''' <summary>
		''' Returns the minimum dimensions needed to layout the <i>visible</i>
		''' components contained in the specified target container. </summary>
		''' <param name="target"> the container that needs to be laid out </param>
		''' <returns>    the minimum dimensions to lay out the
		'''            subcomponents of the specified container </returns>
		''' <seealso cref= #preferredLayoutSize </seealso>
		''' <seealso cref=       java.awt.Container </seealso>
		''' <seealso cref=       java.awt.Container#doLayout </seealso>
		Public Overridable Function minimumLayoutSize(  target As Container) As Dimension Implements LayoutManager.minimumLayoutSize
		  SyncLock target.treeLock
			Dim useBaseline As Boolean = alignOnBaseline
			Dim [dim] As New Dimension(0, 0)
			Dim nmembers As Integer = target.componentCount
			Dim maxAscent As Integer = 0
			Dim maxDescent As Integer = 0
			Dim firstVisibleComponent As Boolean = True

			For i As Integer = 0 To nmembers - 1
				Dim m As Component = target.getComponent(i)
				If m.visible Then
					Dim d As Dimension = m.minimumSize
					[dim].height = System.Math.Max([dim].height, d.height)
					If firstVisibleComponent Then
						firstVisibleComponent = False
					Else
						[dim].width += hgap
					End If
					[dim].width += d.width
					If useBaseline Then
						Dim baseline As Integer = m.getBaseline(d.width, d.height)
						If baseline >= 0 Then
							maxAscent = System.Math.Max(maxAscent, baseline)
							maxDescent = System.Math.Max(maxDescent, [dim].height - baseline)
						End If
					End If
				End If
			Next i

			If useBaseline Then [dim].height = System.Math.Max(maxAscent + maxDescent, [dim].height)

			Dim insets_Renamed As Insets = target.insets
			[dim].width += insets_Renamed.left + insets_Renamed.right + hgap*2
			[dim].height += insets_Renamed.top + insets_Renamed.bottom + vgap*2
			Return [dim]





		  End SyncLock
		End Function

		''' <summary>
		''' Centers the elements in the specified row, if there is any slack. </summary>
		''' <param name="target"> the component which needs to be moved </param>
		''' <param name="x"> the x coordinate </param>
		''' <param name="y"> the y coordinate </param>
		''' <param name="width"> the width dimensions </param>
		''' <param name="height"> the height dimensions </param>
		''' <param name="rowStart"> the beginning of the row </param>
		''' <param name="rowEnd"> the the ending of the row </param>
		''' <param name="useBaseline"> Whether or not to align on baseline. </param>
		''' <param name="ascent"> Ascent for the components. This is only valid if
		'''               useBaseline is true. </param>
		''' <param name="descent"> Ascent for the components. This is only valid if
		'''               useBaseline is true. </param>
		''' <returns> actual row height </returns>
		Private Function moveComponents(  target As Container,   x As Integer,   y As Integer,   width As Integer,   height As Integer,   rowStart As Integer,   rowEnd As Integer,   ltr As Boolean,   useBaseline As Boolean,   ascent As Integer(),   descent As Integer()) As Integer
			Select Case newAlign
			Case LEFT
				x += If(ltr, 0, width)
			Case CENTER
				x += width \ 2
			Case RIGHT
				x += If(ltr, width, 0)
			Case LEADING
			Case TRAILING
				x += width
			End Select
			Dim maxAscent As Integer = 0
			Dim nonbaselineHeight As Integer = 0
			Dim baselineOffset As Integer = 0
			If useBaseline Then
				Dim maxDescent As Integer = 0
				For i As Integer = rowStart To rowEnd - 1
					Dim m As Component = target.getComponent(i)
					If m.visible Then
						If ascent(i) >= 0 Then
							maxAscent = System.Math.Max(maxAscent, ascent(i))
							maxDescent = System.Math.Max(maxDescent, descent(i))
						Else
							nonbaselineHeight = System.Math.Max(m.height, nonbaselineHeight)
						End If
					End If
				Next i
				height = System.Math.Max(maxAscent + maxDescent, nonbaselineHeight)
				baselineOffset = (height - maxAscent - maxDescent) \ 2
			End If
			For i As Integer = rowStart To rowEnd - 1
				Dim m As Component = target.getComponent(i)
				If m.visible Then
					Dim cy As Integer
					If useBaseline AndAlso ascent(i) >= 0 Then
						cy = y + baselineOffset + maxAscent - ascent(i)
					Else
						cy = y + (height - m.height) \ 2
					End If
					If ltr Then
						m.locationion(x, cy)
					Else
						m.locationion(target.width - x - m.width, cy)
					End If
					x += m.width + hgap
				End If
			Next i
			Return height
		End Function

		''' <summary>
		''' Lays out the container. This method lets each
		''' <i>visible</i> component take
		''' its preferred size by reshaping the components in the
		''' target container in order to satisfy the alignment of
		''' this <code>FlowLayout</code> object.
		''' </summary>
		''' <param name="target"> the specified component being laid out </param>
		''' <seealso cref= Container </seealso>
		''' <seealso cref=       java.awt.Container#doLayout </seealso>
		Public Overridable Sub layoutContainer(  target As Container) Implements LayoutManager.layoutContainer
		  SyncLock target.treeLock
			Dim insets_Renamed As Insets = target.insets
			Dim maxwidth As Integer = target.width - (insets_Renamed.left + insets_Renamed.right + hgap*2)
			Dim nmembers As Integer = target.componentCount
			Dim x As Integer = 0, y As Integer = insets_Renamed.top + vgap
			Dim rowh As Integer = 0, start As Integer = 0

			Dim ltr As Boolean = target.componentOrientation.leftToRight

			Dim useBaseline As Boolean = alignOnBaseline
			Dim ascent As Integer() = Nothing
			Dim descent As Integer() = Nothing

			If useBaseline Then
				ascent = New Integer(nmembers - 1){}
				descent = New Integer(nmembers - 1){}
			End If

			For i As Integer = 0 To nmembers - 1
				Dim m As Component = target.getComponent(i)
				If m.visible Then
					Dim d As Dimension = m.preferredSize
					m.sizeize(d.width, d.height)

					If useBaseline Then
						Dim baseline As Integer = m.getBaseline(d.width, d.height)
						If baseline >= 0 Then
							ascent(i) = baseline
							descent(i) = d.height - baseline
						Else
							ascent(i) = -1
						End If
					End If
					If (x = 0) OrElse ((x + d.width) <= maxwidth) Then
						If x > 0 Then x += hgap
						x += d.width
						rowh = System.Math.Max(rowh, d.height)
					Else
						rowh = moveComponents(target, insets_Renamed.left + hgap, y, maxwidth - x, rowh, start, i, ltr, useBaseline, ascent, descent)
						x = d.width
						y += vgap + rowh
						rowh = d.height
						start = i
					End If
				End If
			Next i
			moveComponents(target, insets_Renamed.left + hgap, y, maxwidth - x, rowh, start, nmembers, ltr, useBaseline, ascent, descent)
		  End SyncLock
		End Sub

		'
		' the internal serial version which says which version was written
		' - 0 (default) for versions before the Java 2 platform, v1.2
		' - 1 for version >= Java 2 platform v1.2, which includes "newAlign" field
		'
		Private Const currentSerialVersion As Integer = 1
		''' <summary>
		''' This represent the <code>currentSerialVersion</code>
		''' which is bein used.  It will be one of two values :
		''' <code>0</code> versions before Java 2 platform v1.2..
		''' <code>1</code> versions after  Java 2 platform v1.2..
		''' 
		''' @serial
		''' @since 1.2
		''' </summary>
		Private serialVersionOnStream As Integer = currentSerialVersion

		''' <summary>
		''' Reads this object out of a serialization stream, handling
		''' objects written by older versions of the class that didn't contain all
		''' of the fields we use now..
		''' </summary>
		Private Sub readObject(  stream As java.io.ObjectInputStream)
			stream.defaultReadObject()

			If serialVersionOnStream < 1 Then alignment = Me.align
			serialVersionOnStream = currentSerialVersion
		End Sub

		''' <summary>
		''' Returns a string representation of this <code>FlowLayout</code>
		''' object and its values. </summary>
		''' <returns>     a string representation of this layout </returns>
		Public Overrides Function ToString() As String
			Dim str As String = ""
			Select Case align
			  Case LEFT
				  str = ",align=left"
			  Case CENTER
				  str = ",align=center"
			  Case RIGHT
				  str = ",align=right"
			  Case LEADING
				  str = ",align=leading"
			  Case TRAILING
				  str = ",align=trailing"
			End Select
			Return Me.GetType().name & "[hgap=" & hgap & ",vgap=" & vgap + str & "]"
		End Function


	End Class

End Namespace