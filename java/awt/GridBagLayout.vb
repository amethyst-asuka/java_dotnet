Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

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
	''' The <code>GridBagLayout</code> class is a flexible layout
	''' manager that aligns components vertically, horizontally or along their
	''' baseline without requiring that the components be of the same size.
	''' Each <code>GridBagLayout</code> object maintains a dynamic,
	''' rectangular grid of cells, with each component occupying
	''' one or more cells, called its <em>display area</em>.
	''' <p>
	''' Each component managed by a <code>GridBagLayout</code> is associated with
	''' an instance of <seealso cref="GridBagConstraints"/>.  The constraints object
	''' specifies where a component's display area should be located on the grid
	''' and how the component should be positioned within its display area.  In
	''' addition to its constraints object, the <code>GridBagLayout</code> also
	''' considers each component's minimum and preferred sizes in order to
	''' determine a component's size.
	''' <p>
	''' The overall orientation of the grid depends on the container's
	''' <seealso cref="ComponentOrientation"/> property.  For horizontal left-to-right
	''' orientations, grid coordinate (0,0) is in the upper left corner of the
	''' container with x increasing to the right and y increasing downward.  For
	''' horizontal right-to-left orientations, grid coordinate (0,0) is in the upper
	''' right corner of the container with x increasing to the left and y
	''' increasing downward.
	''' <p>
	''' To use a grid bag layout effectively, you must customize one or more
	''' of the <code>GridBagConstraints</code> objects that are associated
	''' with its components. You customize a <code>GridBagConstraints</code>
	''' object by setting one or more of its instance variables:
	''' 
	''' <dl>
	''' <dt><seealso cref="GridBagConstraints#gridx"/>,
	''' <seealso cref="GridBagConstraints#gridy"/>
	''' <dd>Specifies the cell containing the leading corner of the component's
	''' display area, where the cell at the origin of the grid has address
	''' <code>gridx&nbsp;=&nbsp;0</code>,
	''' <code>gridy&nbsp;=&nbsp;0</code>.  For horizontal left-to-right layout,
	''' a component's leading corner is its upper left.  For horizontal
	''' right-to-left layout, a component's leading corner is its upper right.
	''' Use <code>GridBagConstraints.RELATIVE</code> (the default value)
	''' to specify that the component be placed immediately following
	''' (along the x axis for <code>gridx</code> or the y axis for
	''' <code>gridy</code>) the component that was added to the container
	''' just before this component was added.
	''' <dt><seealso cref="GridBagConstraints#gridwidth"/>,
	''' <seealso cref="GridBagConstraints#gridheight"/>
	''' <dd>Specifies the number of cells in a row (for <code>gridwidth</code>)
	''' or column (for <code>gridheight</code>)
	''' in the component's display area.
	''' The default value is 1.
	''' Use <code>GridBagConstraints.REMAINDER</code> to specify
	''' that the component's display area will be from <code>gridx</code>
	''' to the last cell in the row (for <code>gridwidth</code>)
	''' or from <code>gridy</code> to the last cell in the column
	''' (for <code>gridheight</code>).
	''' 
	''' Use <code>GridBagConstraints.RELATIVE</code> to specify
	''' that the component's display area will be from <code>gridx</code>
	''' to the next to the last cell in its row (for <code>gridwidth</code>
	''' or from <code>gridy</code> to the next to the last cell in its
	''' column (for <code>gridheight</code>).
	''' 
	''' <dt><seealso cref="GridBagConstraints#fill"/>
	''' <dd>Used when the component's display area
	''' is larger than the component's requested size
	''' to determine whether (and how) to resize the component.
	''' Possible values are
	''' <code>GridBagConstraints.NONE</code> (the default),
	''' <code>GridBagConstraints.HORIZONTAL</code>
	''' (make the component wide enough to fill its display area
	''' horizontally, but don't change its height),
	''' <code>GridBagConstraints.VERTICAL</code>
	''' (make the component tall enough to fill its display area
	''' vertically, but don't change its width), and
	''' <code>GridBagConstraints.BOTH</code>
	''' (make the component fill its display area entirely).
	''' <dt><seealso cref="GridBagConstraints#ipadx"/>,
	''' <seealso cref="GridBagConstraints#ipady"/>
	''' <dd>Specifies the component's internal padding within the layout,
	''' how much to add to the minimum size of the component.
	''' The width of the component will be at least its minimum width
	''' plus <code>ipadx</code> pixels. Similarly, the height of
	''' the component will be at least the minimum height plus
	''' <code>ipady</code> pixels.
	''' <dt><seealso cref="GridBagConstraints#insets"/>
	''' <dd>Specifies the component's external padding, the minimum
	''' amount of space between the component and the edges of its display area.
	''' <dt><seealso cref="GridBagConstraints#anchor"/>
	''' <dd>Specifies where the component should be positioned in its display area.
	''' There are three kinds of possible values: absolute, orientation-relative,
	''' and baseline-relative
	''' Orientation relative values are interpreted relative to the container's
	''' <code>ComponentOrientation</code> property while absolute values
	''' are not.  Baseline relative values are calculated relative to the
	''' baseline.  Valid values are:
	''' 
	''' <center><table BORDER=0 WIDTH=800
	'''        SUMMARY="absolute, relative and baseline values as described above">
	''' <tr>
	''' <th><P style="text-align:left">Absolute Values</th>
	''' <th><P style="text-align:left">Orientation Relative Values</th>
	''' <th><P style="text-align:left">Baseline Relative Values</th>
	''' </tr>
	''' <tr>
	''' <td>
	''' <ul style="list-style-type:none">
	''' <li><code>GridBagConstraints.NORTH</code></li>
	''' <li><code>GridBagConstraints.SOUTH</code></li>
	''' <li><code>GridBagConstraints.WEST</code></li>
	''' <li><code>GridBagConstraints.EAST</code></li>
	''' <li><code>GridBagConstraints.NORTHWEST</code></li>
	''' <li><code>GridBagConstraints.NORTHEAST</code></li>
	''' <li><code>GridBagConstraints.SOUTHWEST</code></li>
	''' <li><code>GridBagConstraints.SOUTHEAST</code></li>
	''' <li><code>GridBagConstraints.CENTER</code> (the default)</li>
	''' </ul>
	''' </td>
	''' <td>
	''' <ul style="list-style-type:none">
	''' <li><code>GridBagConstraints.PAGE_START</code></li>
	''' <li><code>GridBagConstraints.PAGE_END</code></li>
	''' <li><code>GridBagConstraints.LINE_START</code></li>
	''' <li><code>GridBagConstraints.LINE_END</code></li>
	''' <li><code>GridBagConstraints.FIRST_LINE_START</code></li>
	''' <li><code>GridBagConstraints.FIRST_LINE_END</code></li>
	''' <li><code>GridBagConstraints.LAST_LINE_START</code></li>
	''' <li><code>GridBagConstraints.LAST_LINE_END</code></li>
	''' </ul>
	''' </td>
	''' <td>
	''' <ul style="list-style-type:none">
	''' <li><code>GridBagConstraints.BASELINE</code></li>
	''' <li><code>GridBagConstraints.BASELINE_LEADING</code></li>
	''' <li><code>GridBagConstraints.BASELINE_TRAILING</code></li>
	''' <li><code>GridBagConstraints.ABOVE_BASELINE</code></li>
	''' <li><code>GridBagConstraints.ABOVE_BASELINE_LEADING</code></li>
	''' <li><code>GridBagConstraints.ABOVE_BASELINE_TRAILING</code></li>
	''' <li><code>GridBagConstraints.BELOW_BASELINE</code></li>
	''' <li><code>GridBagConstraints.BELOW_BASELINE_LEADING</code></li>
	''' <li><code>GridBagConstraints.BELOW_BASELINE_TRAILING</code></li>
	''' </ul>
	''' </td>
	''' </tr>
	''' </table></center>
	''' <dt><seealso cref="GridBagConstraints#weightx"/>,
	''' <seealso cref="GridBagConstraints#weighty"/>
	''' <dd>Used to determine how to distribute space, which is
	''' important for specifying resizing behavior.
	''' Unless you specify a weight for at least one component
	''' in a row (<code>weightx</code>) and column (<code>weighty</code>),
	''' all the components clump together in the center of their container.
	''' This is because when the weight is zero (the default),
	''' the <code>GridBagLayout</code> object puts any extra space
	''' between its grid of cells and the edges of the container.
	''' </dl>
	''' <p>
	''' Each row may have a baseline; the baseline is determined by the
	''' components in that row that have a valid baseline and are aligned
	''' along the baseline (the component's anchor value is one of {@code
	''' BASELINE}, {@code BASELINE_LEADING} or {@code BASELINE_TRAILING}).
	''' If none of the components in the row has a valid baseline, the row
	''' does not have a baseline.
	''' <p>
	''' If a component spans rows it is aligned either to the baseline of
	''' the start row (if the baseline-resize behavior is {@code
	''' CONSTANT_ASCENT}) or the end row (if the baseline-resize behavior
	''' is {@code CONSTANT_DESCENT}).  The row that the component is
	''' aligned to is called the <em>prevailing row</em>.
	''' <p>
	''' The following figure shows a baseline layout and includes a
	''' component that spans rows:
	''' <center><table summary="Baseline Layout">
	''' <tr ALIGN=CENTER>
	''' <td>
	''' <img src="doc-files/GridBagLayout-baseline.png"
	'''  alt="The following text describes this graphic (Figure 1)." style="float:center">
	''' </td>
	''' </table></center>
	''' This layout consists of three components:
	''' <ul><li>A panel that starts in row 0 and ends in row 1.  The panel
	'''   has a baseline-resize behavior of <code>CONSTANT_DESCENT</code> and has
	'''   an anchor of <code>BASELINE</code>.  As the baseline-resize behavior
	'''   is <code>CONSTANT_DESCENT</code> the prevailing row for the panel is
	'''   row 1.
	''' <li>Two buttons, each with a baseline-resize behavior of
	'''   <code>CENTER_OFFSET</code> and an anchor of <code>BASELINE</code>.
	''' </ul>
	''' Because the second button and the panel share the same prevailing row,
	''' they are both aligned along their baseline.
	''' <p>
	''' Components positioned using one of the baseline-relative values resize
	''' differently than when positioned using an absolute or orientation-relative
	''' value.  How components change is dictated by how the baseline of the
	''' prevailing row changes.  The baseline is anchored to the
	''' bottom of the display area if any components with the same prevailing row
	''' have a baseline-resize behavior of <code>CONSTANT_DESCENT</code>,
	''' otherwise the baseline is anchored to the top of the display area.
	''' The following rules dictate the resize behavior:
	''' <ul>
	''' <li>Resizable components positioned above the baseline can only
	''' grow as tall as the baseline.  For example, if the baseline is at 100
	''' and anchored at the top, a resizable component positioned above the
	''' baseline can never grow more than 100 units.
	''' <li>Similarly, resizable components positioned below the baseline can
	''' only grow as high as the difference between the display height and the
	''' baseline.
	''' <li>Resizable components positioned on the baseline with a
	''' baseline-resize behavior of <code>OTHER</code> are only resized if
	''' the baseline at the resized size fits within the display area.  If
	''' the baseline is such that it does not fit within the display area
	''' the component is not resized.
	''' <li>Components positioned on the baseline that do not have a
	''' baseline-resize behavior of <code>OTHER</code>
	''' can only grow as tall as {@code display height - baseline + baseline of component}.
	''' </ul>
	''' If you position a component along the baseline, but the
	''' component does not have a valid baseline, it will be vertically centered
	''' in its space.  Similarly if you have positioned a component relative
	''' to the baseline and none of the components in the row have a valid
	''' baseline the component is vertically centered.
	''' <p>
	''' The following figures show ten components (all buttons)
	''' managed by a grid bag layout.  Figure 2 shows the layout for a horizontal,
	''' left-to-right container and Figure 3 shows the layout for a horizontal,
	''' right-to-left container.
	''' 
	''' <center><table WIDTH=600 summary="layout">
	''' <tr ALIGN=CENTER>
	''' <td>
	''' <img src="doc-files/GridBagLayout-1.gif" alt="The preceding text describes this graphic (Figure 1)." style="float:center; margin: 7px 10px;">
	''' </td>
	''' <td>
	''' <img src="doc-files/GridBagLayout-2.gif" alt="The preceding text describes this graphic (Figure 2)." style="float:center; margin: 7px 10px;">
	''' </td>
	''' <tr ALIGN=CENTER>
	''' <td>Figure 2: Horizontal, Left-to-Right</td>
	''' <td>Figure 3: Horizontal, Right-to-Left</td>
	''' </tr>
	''' </table></center>
	''' <p>
	''' Each of the ten components has the <code>fill</code> field
	''' of its associated <code>GridBagConstraints</code> object
	''' set to <code>GridBagConstraints.BOTH</code>.
	''' In addition, the components have the following non-default constraints:
	''' 
	''' <ul>
	''' <li>Button1, Button2, Button3: <code>weightx&nbsp;=&nbsp;1.0</code>
	''' <li>Button4: <code>weightx&nbsp;=&nbsp;1.0</code>,
	''' <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	''' <li>Button5: <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	''' <li>Button6: <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.RELATIVE</code>
	''' <li>Button7: <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	''' <li>Button8: <code>gridheight&nbsp;=&nbsp;2</code>,
	''' <code>weighty&nbsp;=&nbsp;1.0</code>
	''' <li>Button9, Button 10:
	''' <code>gridwidth&nbsp;=&nbsp;GridBagConstraints.REMAINDER</code>
	''' </ul>
	''' <p>
	''' Here is the code that implements the example shown above:
	''' 
	''' <hr><blockquote><pre>
	''' import java.awt.*;
	''' import java.util.*;
	''' import java.applet.Applet;
	''' 
	''' public class GridBagEx1 extends Applet {
	''' 
	'''     protected  Sub  makebutton(String name,
	'''                               GridBagLayout gridbag,
	'''                               GridBagConstraints c) {
	'''         Button button = new Button(name);
	'''         gridbag.setConstraints(button, c);
	'''         add(button);
	'''     }
	''' 
	'''     public  Sub  init() {
	'''         GridBagLayout gridbag = new GridBagLayout();
	'''         GridBagConstraints c = new GridBagConstraints();
	''' 
	'''         setFont(new Font("SansSerif", Font.PLAIN, 14));
	'''         setLayout(gridbag);
	''' 
	'''         c.fill = GridBagConstraints.BOTH;
	'''         c.weightx = 1.0;
	'''         makebutton("Button1", gridbag, c);
	'''         makebutton("Button2", gridbag, c);
	'''         makebutton("Button3", gridbag, c);
	''' 
	'''         c.gridwidth = GridBagConstraints.REMAINDER; //end row
	'''         makebutton("Button4", gridbag, c);
	''' 
	'''         c.weightx = 0.0;                //reset to the default
	'''         makebutton("Button5", gridbag, c); //another row
	''' 
	'''         c.gridwidth = GridBagConstraints.RELATIVE; //next-to-last in row
	'''         makebutton("Button6", gridbag, c);
	''' 
	'''         c.gridwidth = GridBagConstraints.REMAINDER; //end row
	'''         makebutton("Button7", gridbag, c);
	''' 
	'''         c.gridwidth = 1;                //reset to the default
	'''         c.gridheight = 2;
	'''         c.weighty = 1.0;
	'''         makebutton("Button8", gridbag, c);
	''' 
	'''         c.weighty = 0.0;                //reset to the default
	'''         c.gridwidth = GridBagConstraints.REMAINDER; //end row
	'''         c.gridheight = 1;               //reset to the default
	'''         makebutton("Button9", gridbag, c);
	'''         makebutton("Button10", gridbag, c);
	''' 
	'''         setSize(300, 100);
	'''     }
	''' 
	'''     Public Shared  Sub  main(String args[]) {
	'''         Frame f = new Frame("GridBag Layout Example");
	'''         GridBagEx1 ex1 = new GridBagEx1();
	''' 
	'''         ex1.init();
	''' 
	'''         f.add("Center", ex1);
	'''         f.pack();
	'''         f.setSize(f.getPreferredSize());
	'''         f.show();
	'''     }
	''' }
	''' </pre></blockquote><hr>
	''' <p>
	''' @author Doug Stein
	''' @author Bill Spitzak (orignial NeWS &amp; OLIT implementation) </summary>
	''' <seealso cref=       java.awt.GridBagConstraints </seealso>
	''' <seealso cref=       java.awt.GridBagLayoutInfo </seealso>
	''' <seealso cref=       java.awt.ComponentOrientation
	''' @since JDK1.0 </seealso>
	<Serializable> _
	Public Class GridBagLayout
		Implements LayoutManager2

		Friend Const EMPIRICMULTIPLIER As Integer = 2
		''' <summary>
		''' This field is no longer used to reserve arrays and kept for backward
		''' compatibility. Previously, this was
		''' the maximum number of grid positions (both horizontal and
		''' vertical) that could be laid out by the grid bag layout.
		''' Current implementation doesn't impose any limits
		''' on the size of a grid.
		''' </summary>
		Protected Friend Const MAXGRIDSIZE As Integer = 512

		''' <summary>
		''' The smallest grid that can be laid out by the grid bag layout.
		''' </summary>
		Protected Friend Const MINSIZE As Integer = 1
		''' <summary>
		''' The preferred grid size that can be laid out by the grid bag layout.
		''' </summary>
		Protected Friend Const PREFERREDSIZE As Integer = 2

		''' <summary>
		''' This hashtable maintains the association between
		''' a component and its gridbag constraints.
		''' The Keys in <code>comptable</code> are the components and the
		''' values are the instances of <code>GridBagConstraints</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= java.awt.GridBagConstraints </seealso>
		Protected Friend comptable As Dictionary(Of Component, GridBagConstraints)

		''' <summary>
		''' This field holds a gridbag constraints instance
		''' containing the default values, so if a component
		''' does not have gridbag constraints associated with
		''' it, then the component will be assigned a
		''' copy of the <code>defaultConstraints</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getConstraints(Component) </seealso>
		''' <seealso cref= #setConstraints(Component, GridBagConstraints) </seealso>
		''' <seealso cref= #lookupConstraints(Component) </seealso>
		Protected Friend defaultConstraints As GridBagConstraints

		''' <summary>
		''' This field holds the layout information
		''' for the gridbag.  The information in this field
		''' is based on the most recent validation of the
		''' gridbag.
		''' If <code>layoutInfo</code> is <code>null</code>
		''' this indicates that there are no components in
		''' the gridbag or if there are components, they have
		''' not yet been validated.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLayoutInfo(Container, int) </seealso>
		Protected Friend layoutInfo As GridBagLayoutInfo

		''' <summary>
		''' This field holds the overrides to the column minimum
		''' width.  If this field is non-<code>null</code> the values are
		''' applied to the gridbag after all of the minimum columns
		''' widths have been calculated.
		''' If columnWidths has more elements than the number of
		''' columns, columns are added to the gridbag to match
		''' the number of elements in columnWidth.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLayoutDimensions() </seealso>
		Public columnWidths As Integer()

		''' <summary>
		''' This field holds the overrides to the row minimum
		''' heights.  If this field is non-<code>null</code> the values are
		''' applied to the gridbag after all of the minimum row
		''' heights have been calculated.
		''' If <code>rowHeights</code> has more elements than the number of
		''' rows, rows are added to the gridbag to match
		''' the number of elements in <code>rowHeights</code>.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getLayoutDimensions() </seealso>
		Public rowHeights As Integer()

		''' <summary>
		''' This field holds the overrides to the column weights.
		''' If this field is non-<code>null</code> the values are
		''' applied to the gridbag after all of the columns
		''' weights have been calculated.
		''' If <code>columnWeights[i]</code> &gt; weight for column i, then
		''' column i is assigned the weight in <code>columnWeights[i]</code>.
		''' If <code>columnWeights</code> has more elements than the number
		''' of columns, the excess elements are ignored - they do
		''' not cause more columns to be created.
		''' 
		''' @serial
		''' </summary>
		Public columnWeights As Double()

		''' <summary>
		''' This field holds the overrides to the row weights.
		''' If this field is non-<code>null</code> the values are
		''' applied to the gridbag after all of the rows
		''' weights have been calculated.
		''' If <code>rowWeights[i]</code> &gt; weight for row i, then
		''' row i is assigned the weight in <code>rowWeights[i]</code>.
		''' If <code>rowWeights</code> has more elements than the number
		''' of rows, the excess elements are ignored - they do
		''' not cause more rows to be created.
		''' 
		''' @serial
		''' </summary>
		Public rowWeights As Double()

		''' <summary>
		''' The component being positioned.  This is set before calling into
		''' <code>adjustForGravity</code>.
		''' </summary>
		Private componentAdjusting As Component

		''' <summary>
		''' Creates a grid bag layout manager.
		''' </summary>
		Public Sub New()
			comptable = New Dictionary(Of Component, GridBagConstraints)
			defaultConstraints = New GridBagConstraints
		End Sub

		''' <summary>
		''' Sets the constraints for the specified component in this layout. </summary>
		''' <param name="comp"> the component to be modified </param>
		''' <param name="constraints"> the constraints to be applied </param>
		Public Overridable Sub setConstraints(ByVal comp As Component, ByVal constraints As GridBagConstraints)
			comptable(comp) = CType(constraints.clone(), GridBagConstraints)
		End Sub

		''' <summary>
		''' Gets the constraints for the specified component.  A copy of
		''' the actual <code>GridBagConstraints</code> object is returned. </summary>
		''' <param name="comp"> the component to be queried </param>
		''' <returns>      the constraint for the specified component in this
		'''                  grid bag layout; a copy of the actual constraint
		'''                  object is returned </returns>
		Public Overridable Function getConstraints(ByVal comp As Component) As GridBagConstraints
			Dim constraints_Renamed As GridBagConstraints = comptable(comp)
			If constraints_Renamed Is Nothing Then
				constraintsnts(comp, defaultConstraints)
				constraints_Renamed = comptable(comp)
			End If
			Return CType(constraints_Renamed.clone(), GridBagConstraints)
		End Function

		''' <summary>
		''' Retrieves the constraints for the specified component.
		''' The return value is not a copy, but is the actual
		''' <code>GridBagConstraints</code> object used by the layout mechanism.
		''' <p>
		''' If <code>comp</code> is not in the <code>GridBagLayout</code>,
		''' a set of default <code>GridBagConstraints</code> are returned.
		''' A <code>comp</code> value of <code>null</code> is invalid
		''' and returns <code>null</code>.
		''' </summary>
		''' <param name="comp"> the component to be queried </param>
		''' <returns>      the constraints for the specified component </returns>
		Protected Friend Overridable Function lookupConstraints(ByVal comp As Component) As GridBagConstraints
			Dim constraints_Renamed As GridBagConstraints = comptable(comp)
			If constraints_Renamed Is Nothing Then
				constraintsnts(comp, defaultConstraints)
				constraints_Renamed = comptable(comp)
			End If
			Return constraints_Renamed
		End Function

		''' <summary>
		''' Removes the constraints for the specified component in this layout </summary>
		''' <param name="comp"> the component to be modified </param>
		Private Sub removeConstraints(ByVal comp As Component)
			comptable.Remove(comp)
		End Sub

		''' <summary>
		''' Determines the origin of the layout area, in the graphics coordinate
		''' space of the target container.  This value represents the pixel
		''' coordinates of the top-left corner of the layout area regardless of
		''' the <code>ComponentOrientation</code> value of the container.  This
		''' is distinct from the grid origin given by the cell coordinates (0,0).
		''' Most applications do not call this method directly. </summary>
		''' <returns>     the graphics origin of the cell in the top-left
		'''             corner of the layout grid </returns>
		''' <seealso cref=        java.awt.ComponentOrientation
		''' @since      JDK1.1 </seealso>
		Public Overridable Property layoutOrigin As Point
			Get
				Dim origin As New Point(0,0)
				If layoutInfo IsNot Nothing Then
					origin.x = layoutInfo.startx
					origin.y = layoutInfo.starty
				End If
				Return origin
			End Get
		End Property

		''' <summary>
		''' Determines column widths and row heights for the layout grid.
		''' <p>
		''' Most applications do not call this method directly. </summary>
		''' <returns>     an array of two arrays, containing the widths
		'''                       of the layout columns and
		'''                       the heights of the layout rows
		''' @since      JDK1.1 </returns>
		Public Overridable Property layoutDimensions As Integer()()
			Get
				If layoutInfo Is Nothing Then Return New Integer(1)(0){}
    
				Dim [dim] As Integer()() = New Integer (1)(){}
				[dim](0) = New Integer(layoutInfo.width - 1){}
				[dim](1) = New Integer(layoutInfo.height - 1){}
    
				Array.Copy(layoutInfo.minWidth, 0, [dim](0), 0, layoutInfo.width)
				Array.Copy(layoutInfo.minHeight, 0, [dim](1), 0, layoutInfo.height)
    
				Return [dim]
			End Get
		End Property

		''' <summary>
		''' Determines the weights of the layout grid's columns and rows.
		''' Weights are used to calculate how much a given column or row
		''' stretches beyond its preferred size, if the layout has extra
		''' room to fill.
		''' <p>
		''' Most applications do not call this method directly. </summary>
		''' <returns>      an array of two arrays, representing the
		'''                    horizontal weights of the layout columns
		'''                    and the vertical weights of the layout rows
		''' @since       JDK1.1 </returns>
		Public Overridable Property layoutWeights As Double()()
			Get
				If layoutInfo Is Nothing Then Return New Double(1)(0){}
    
				Dim weights As Double()() = New Double (1)(){}
				weights(0) = New Double(layoutInfo.width - 1){}
				weights(1) = New Double(layoutInfo.height - 1){}
    
				Array.Copy(layoutInfo.weightX, 0, weights(0), 0, layoutInfo.width)
				Array.Copy(layoutInfo.weightY, 0, weights(1), 0, layoutInfo.height)
    
				Return weights
			End Get
		End Property

		''' <summary>
		''' Determines which cell in the layout grid contains the point
		''' specified by <code>(x,&nbsp;y)</code>. Each cell is identified
		''' by its column index (ranging from 0 to the number of columns
		''' minus 1) and its row index (ranging from 0 to the number of
		''' rows minus 1).
		''' <p>
		''' If the <code>(x,&nbsp;y)</code> point lies
		''' outside the grid, the following rules are used.
		''' The column index is returned as zero if <code>x</code> lies to the
		''' left of the layout for a left-to-right container or to the right of
		''' the layout for a right-to-left container.  The column index is returned
		''' as the number of columns if <code>x</code> lies
		''' to the right of the layout in a left-to-right container or to the left
		''' in a right-to-left container.
		''' The row index is returned as zero if <code>y</code> lies above the
		''' layout, and as the number of rows if <code>y</code> lies
		''' below the layout.  The orientation of a container is determined by its
		''' <code>ComponentOrientation</code> property. </summary>
		''' <param name="x">    the <i>x</i> coordinate of a point </param>
		''' <param name="y">    the <i>y</i> coordinate of a point </param>
		''' <returns>     an ordered pair of indexes that indicate which cell
		'''             in the layout grid contains the point
		'''             (<i>x</i>,&nbsp;<i>y</i>). </returns>
		''' <seealso cref=        java.awt.ComponentOrientation
		''' @since      JDK1.1 </seealso>
		Public Overridable Function location(ByVal x As Integer, ByVal y As Integer) As Point
			Dim loc As New Point(0,0)
			Dim i, d As Integer

			If layoutInfo Is Nothing Then Return loc

			d = layoutInfo.startx
			If Not rightToLeft Then
				For i = 0 To layoutInfo.width - 1
					d += layoutInfo.minWidth(i)
					If d > x Then Exit For
				Next i
			Else
				For i = layoutInfo.width-1 To 0 Step -1
					If d > x Then Exit For
					d += layoutInfo.minWidth(i)
				Next i
				i += 1
			End If
			loc.x = i

			d = layoutInfo.starty
			For i = 0 To layoutInfo.height - 1
				d += layoutInfo.minHeight(i)
				If d > y Then Exit For
			Next i
			loc.y = i

			Return loc
		End Function

		''' <summary>
		''' Has no effect, since this layout manager does not use a per-component string.
		''' </summary>
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal comp As Component) Implements LayoutManager.addLayoutComponent
		End Sub

		''' <summary>
		''' Adds the specified component to the layout, using the specified
		''' <code>constraints</code> object.  Note that constraints
		''' are mutable and are, therefore, cloned when cached.
		''' </summary>
		''' <param name="comp">         the component to be added </param>
		''' <param name="constraints">  an object that determines how
		'''                          the component is added to the layout </param>
		''' <exception cref="IllegalArgumentException"> if <code>constraints</code>
		'''            is not a <code>GridBagConstraint</code> </exception>
		Public Overridable Sub addLayoutComponent(ByVal comp As Component, ByVal constraints As Object) Implements LayoutManager2.addLayoutComponent
			If TypeOf constraints Is GridBagConstraints Then
				constraintsnts(comp, CType(constraints, GridBagConstraints))
			ElseIf constraints IsNot Nothing Then
				Throw New IllegalArgumentException("cannot add to layout: constraints must be a GridBagConstraint")
			End If
		End Sub

		''' <summary>
		''' Removes the specified component from this layout.
		''' <p>
		''' Most applications do not call this method directly. </summary>
		''' <param name="comp">   the component to be removed. </param>
		''' <seealso cref=      java.awt.Container#remove(java.awt.Component) </seealso>
		''' <seealso cref=      java.awt.Container#removeAll() </seealso>
		Public Overridable Sub removeLayoutComponent(ByVal comp As Component) Implements LayoutManager.removeLayoutComponent
			removeConstraints(comp)
		End Sub

		''' <summary>
		''' Determines the preferred size of the <code>parent</code>
		''' container using this grid bag layout.
		''' <p>
		''' Most applications do not call this method directly.
		''' </summary>
		''' <param name="parent">   the container in which to do the layout </param>
		''' <seealso cref=       java.awt.Container#getPreferredSize </seealso>
		''' <returns> the preferred size of the <code>parent</code>
		'''  container </returns>
		Public Overridable Function preferredLayoutSize(ByVal parent As Container) As Dimension Implements LayoutManager.preferredLayoutSize
			Dim info As GridBagLayoutInfo = getLayoutInfo(parent, PREFERREDSIZE)
			Return getMinSize(parent, info)
		End Function

		''' <summary>
		''' Determines the minimum size of the <code>parent</code> container
		''' using this grid bag layout.
		''' <p>
		''' Most applications do not call this method directly. </summary>
		''' <param name="parent">   the container in which to do the layout </param>
		''' <seealso cref=       java.awt.Container#doLayout </seealso>
		''' <returns> the minimum size of the <code>parent</code> container </returns>
		Public Overridable Function minimumLayoutSize(ByVal parent As Container) As Dimension Implements LayoutManager.minimumLayoutSize
			Dim info As GridBagLayoutInfo = getLayoutInfo(parent, MINSIZE)
			Return getMinSize(parent, info)
		End Function

		''' <summary>
		''' Returns the maximum dimensions for this layout given the components
		''' in the specified target container. </summary>
		''' <param name="target"> the container which needs to be laid out </param>
		''' <seealso cref= Container </seealso>
		''' <seealso cref= #minimumLayoutSize(Container) </seealso>
		''' <seealso cref= #preferredLayoutSize(Container) </seealso>
		''' <returns> the maximum dimensions for this layout </returns>
		Public Overridable Function maximumLayoutSize(ByVal target As Container) As Dimension Implements LayoutManager2.maximumLayoutSize
			Return New Dimension( java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value)
		End Function

		''' <summary>
		''' Returns the alignment along the x axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' <p> </summary>
		''' <returns> the value <code>0.5f</code> to indicate centered </returns>
		Public Overridable Function getLayoutAlignmentX(ByVal parent As Container) As Single Implements LayoutManager2.getLayoutAlignmentX
			Return 0.5f
		End Function

		''' <summary>
		''' Returns the alignment along the y axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' <p> </summary>
		''' <returns> the value <code>0.5f</code> to indicate centered </returns>
		Public Overridable Function getLayoutAlignmentY(ByVal parent As Container) As Single Implements LayoutManager2.getLayoutAlignmentY
			Return 0.5f
		End Function

		''' <summary>
		''' Invalidates the layout, indicating that if the layout manager
		''' has cached information it should be discarded.
		''' </summary>
		Public Overridable Sub invalidateLayout(ByVal target As Container) Implements LayoutManager2.invalidateLayout
		End Sub

		''' <summary>
		''' Lays out the specified container using this grid bag layout.
		''' This method reshapes components in the specified container in
		''' order to satisfy the constraints of this <code>GridBagLayout</code>
		''' object.
		''' <p>
		''' Most applications do not call this method directly. </summary>
		''' <param name="parent"> the container in which to do the layout </param>
		''' <seealso cref= java.awt.Container </seealso>
		''' <seealso cref= java.awt.Container#doLayout </seealso>
		Public Overridable Sub layoutContainer(ByVal parent As Container) Implements LayoutManager.layoutContainer
			arrangeGrid(parent)
		End Sub

		''' <summary>
		''' Returns a string representation of this grid bag layout's values. </summary>
		''' <returns>     a string representation of this grid bag layout. </returns>
		Public Overrides Function ToString() As String
			Return Me.GetType().name
		End Function

		''' <summary>
		''' Print the layout information.  Useful for debugging.
		''' </summary>

	'     DEBUG
	'     *
	'     *  protected  Sub  dumpLayoutInfo(GridBagLayoutInfo s) {
	'     *    int x;
	'     *
	'     *    System.out.println("Col\tWidth\tWeight");
	'     *    for (x=0; x<s.width; x++) {
	'     *      System.out.println(x + "\t" +
	'     *                   s.minWidth[x] + "\t" +
	'     *                   s.weightX[x]);
	'     *    }
	'     *    System.out.println("Row\tHeight\tWeight");
	'     *    for (x=0; x<s.height; x++) {
	'     *      System.out.println(x + "\t" +
	'     *                   s.minHeight[x] + "\t" +
	'     *                   s.weightY[x]);
	'     *    }
	'     *  }
	'     

		''' <summary>
		''' Print the layout constraints.  Useful for debugging.
		''' </summary>

	'     DEBUG
	'     *
	'     *  protected  Sub  dumpConstraints(GridBagConstraints constraints) {
	'     *    System.out.println(
	'     *                 "wt " +
	'     *                 constraints.weightx +
	'     *                 " " +
	'     *                 constraints.weighty +
	'     *                 ", " +
	'     *
	'     *                 "box " +
	'     *                 constraints.gridx +
	'     *                 " " +
	'     *                 constraints.gridy +
	'     *                 " " +
	'     *                 constraints.gridwidth +
	'     *                 " " +
	'     *                 constraints.gridheight +
	'     *                 ", " +
	'     *
	'     *                 "min " +
	'     *                 constraints.minWidth +
	'     *                 " " +
	'     *                 constraints.minHeight +
	'     *                 ", " +
	'     *
	'     *                 "pad " +
	'     *                 constraints.insets.bottom +
	'     *                 " " +
	'     *                 constraints.insets.left +
	'     *                 " " +
	'     *                 constraints.insets.right +
	'     *                 " " +
	'     *                 constraints.insets.top +
	'     *                 " " +
	'     *                 constraints.ipadx +
	'     *                 " " +
	'     *                 constraints.ipady);
	'     *  }
	'     

		''' <summary>
		''' Fills in an instance of <code>GridBagLayoutInfo</code> for the
		''' current set of managed children. This requires three passes through the
		''' set of children:
		''' 
		''' <ol>
		''' <li>Figure out the dimensions of the layout grid.
		''' <li>Determine which cells the components occupy.
		''' <li>Distribute the weights and min sizes among the rows/columns.
		''' </ol>
		''' 
		''' This also caches the minsizes for all the children when they are
		''' first encountered (so subsequent loops don't need to ask again).
		''' <p>
		''' This method should only be used internally by
		''' <code>GridBagLayout</code>.
		''' </summary>
		''' <param name="parent">  the layout container </param>
		''' <param name="sizeflag"> either <code>PREFERREDSIZE</code> or
		'''   <code>MINSIZE</code> </param>
		''' <returns> the <code>GridBagLayoutInfo</code> for the set of children
		''' @since 1.4 </returns>
		Protected Friend Overridable Function getLayoutInfo(ByVal parent As Container, ByVal sizeflag As Integer) As GridBagLayoutInfo
			Return GetLayoutInfo(parent, sizeflag)
		End Function

	'    
	'     * Calculate maximum array sizes to allocate arrays without ensureCapacity
	'     * we may use preCalculated sizes in whole class because of upper estimation of
	'     * maximumArrayXIndex and maximumArrayYIndex.
	'     

		Private Function preInitMaximumArraySizes(ByVal parent As Container) As Long()
			Dim components As Component() = parent.components
			Dim comp As Component
			Dim constraints_Renamed As GridBagConstraints
			Dim curX, curY As Integer
			Dim curWidth, curHeight As Integer
			Dim preMaximumArrayXIndex As Integer = 0
			Dim preMaximumArrayYIndex As Integer = 0
			Dim returnArray As Long() = New Long(1){}

			For compId As Integer = 0 To components.Length - 1
				comp = components(compId)
				If Not comp.visible Then Continue For

				constraints_Renamed = lookupConstraints(comp)
				curX = constraints_Renamed.gridx
				curY = constraints_Renamed.gridy
				curWidth = constraints_Renamed.gridwidth
				curHeight = constraints_Renamed.gridheight

				' -1==RELATIVE, means that column|row equals to previously added component,
				' since each next Component with gridx|gridy == RELATIVE starts from
				' previous position, so we should start from previous component which
				' already used in maximumArray[X|Y]Index calculation. We could just increase
				' maximum by 1 to handle situation when component with gridx=-1 was added.
				If curX < 0 Then
					preMaximumArrayYIndex += 1
					curX = preMaximumArrayYIndex
				End If
				If curY < 0 Then
					preMaximumArrayXIndex += 1
					curY = preMaximumArrayXIndex
				End If
				' gridwidth|gridheight may be equal to RELATIVE (-1) or REMAINDER (0)
				' in any case using 1 instead of 0 or -1 should be sufficient to for
				' correct maximumArraySizes calculation
				If curWidth <= 0 Then curWidth = 1
				If curHeight <= 0 Then curHeight = 1

				preMaximumArrayXIndex = System.Math.Max(curY + curHeight, preMaximumArrayXIndex)
				preMaximumArrayYIndex = System.Math.Max(curX + curWidth, preMaximumArrayYIndex)
			Next compId 'for (components) loop
			' Must specify index++ to allocate well-working arrays.
	'         fix for 4623196.
	'         * now return long array instead of Point
	'         
			returnArray(0) = preMaximumArrayXIndex
			returnArray(1) = preMaximumArrayYIndex
			Return returnArray
		End Function 'PreInitMaximumSizes

		''' <summary>
		''' This method is obsolete and supplied for backwards
		''' compatibility only; new code should call {@link
		''' #getLayoutInfo(java.awt.Container, int) getLayoutInfo} instead.
		''' This method is the same as <code>getLayoutInfo</code>;
		''' refer to <code>getLayoutInfo</code> for details on parameters
		''' and return value.
		''' </summary>
		Protected Friend Overridable Function GetLayoutInfo(ByVal parent As Container, ByVal sizeflag As Integer) As GridBagLayoutInfo
			SyncLock parent.treeLock
				Dim r As GridBagLayoutInfo
				Dim comp As Component
				Dim constraints_Renamed As GridBagConstraints
				Dim d As Dimension
				Dim components As Component() = parent.components
				' Code below will address index curX+curWidth in the case of yMaxArray, weightY
				' ( respectively curY+curHeight for xMaxArray, weightX ) where
				'  curX in 0 to preInitMaximumArraySizes.y
				' Thus, the maximum index that could
				' be calculated in the following code is curX+curX.
				' EmpericMultier equals 2 because of this.

				Dim layoutWidth, layoutHeight As Integer
				Dim xMaxArray As Integer()
				Dim yMaxArray As Integer()
				Dim compindex, i, k, px, py, pixels_diff, nextSize As Integer
				Dim curX As Integer = 0 ' constraints.gridx
				Dim curY As Integer = 0 ' constraints.gridy
				Dim curWidth As Integer = 1 ' constraints.gridwidth
				Dim curHeight As Integer = 1 ' constraints.gridheight
				Dim curRow, curCol As Integer
				Dim weight_diff, weight As Double
				Dim maximumArrayXIndex As Integer = 0
				Dim maximumArrayYIndex As Integer = 0
				Dim anchor As Integer

	'            
	'             * Pass #1
	'             *
	'             * Figure out the dimensions of the layout grid (use a value of 1 for
	'             * zero or negative widths and heights).
	'             

					layoutHeight = 0
					layoutWidth = layoutHeight
					curCol = -1
					curRow = curCol
				Dim arraySizes As Long() = preInitMaximumArraySizes(parent)

	'             fix for 4623196.
	'             * If user try to create a very big grid we can
	'             * get NegativeArraySizeException because of integer value
	'             * overflow (EMPIRICMULTIPLIER*gridSize might be more then  java.lang.[Integer].MAX_VALUE).
	'             * We need to detect this situation and try to create a
	'             * grid with  java.lang.[Integer].MAX_VALUE size instead.
	'             
				maximumArrayXIndex = If(EMPIRICMULTIPLIER * arraySizes(0) >  java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value, EMPIRICMULTIPLIER*CInt(arraySizes(0)))
				maximumArrayYIndex = If(EMPIRICMULTIPLIER * arraySizes(1) >  java.lang.[Integer].Max_Value,  java.lang.[Integer].Max_Value, EMPIRICMULTIPLIER*CInt(arraySizes(1)))

				If rowHeights IsNot Nothing Then maximumArrayXIndex = System.Math.Max(maximumArrayXIndex, rowHeights.Length)
				If columnWidths IsNot Nothing Then maximumArrayYIndex = System.Math.Max(maximumArrayYIndex, columnWidths.Length)

				xMaxArray = New Integer(maximumArrayXIndex - 1){}
				yMaxArray = New Integer(maximumArrayYIndex - 1){}

				Dim hasBaseline As Boolean = False
				For compindex = 0 To components.Length - 1
					comp = components(compindex)
					If Not comp.visible Then Continue For
					constraints_Renamed = lookupConstraints(comp)

					curX = constraints_Renamed.gridx
					curY = constraints_Renamed.gridy
					curWidth = constraints_Renamed.gridwidth
					If curWidth <= 0 Then curWidth = 1
					curHeight = constraints_Renamed.gridheight
					If curHeight <= 0 Then curHeight = 1

					' If x or y is negative, then use relative positioning: 
					If curX < 0 AndAlso curY < 0 Then
						If curRow >= 0 Then
							curY = curRow
						ElseIf curCol >= 0 Then
							curX = curCol
						Else
							curY = 0
						End If
					End If
					If curX < 0 Then
						px = 0
						For i = curY To (curY + curHeight) - 1
							px = System.Math.Max(px, xMaxArray(i))
						Next i

						curX = px - curX - 1
						If curX < 0 Then curX = 0
					ElseIf curY < 0 Then
						py = 0
						For i = curX To (curX + curWidth) - 1
							py = System.Math.Max(py, yMaxArray(i))
						Next i
						curY = py - curY - 1
						If curY < 0 Then curY = 0
					End If

	'                 Adjust the grid width and height
	'                 *  fix for 5005945: unneccessary loops removed
	'                 
					px = curX + curWidth
					If layoutWidth < px Then layoutWidth = px
					py = curY + curHeight
					If layoutHeight < py Then layoutHeight = py

					' Adjust xMaxArray and yMaxArray 
					For i = curX To (curX + curWidth) - 1
						yMaxArray(i) =py
					Next i
					For i = curY To (curY + curHeight) - 1
						xMaxArray(i) = px
					Next i


					' Cache the current slave's size. 
					If sizeflag = PREFERREDSIZE Then
						d = comp.preferredSize
					Else
						d = comp.minimumSize
					End If
					constraints_Renamed.minWidth = d.width
					constraints_Renamed.minHeight = d.height
					If calculateBaseline(comp, constraints_Renamed, d) Then hasBaseline = True

	'                 Zero width and height must mean that this is the last item (or
	'                 * else something is wrong). 
					If constraints_Renamed.gridheight = 0 AndAlso constraints_Renamed.gridwidth = 0 Then
							curCol = -1
							curRow = curCol
					End If

					' Zero width starts a new row 
					If constraints_Renamed.gridheight = 0 AndAlso curRow < 0 Then
						curCol = curX + curWidth

					' Zero height starts a new column 
					ElseIf constraints_Renamed.gridwidth = 0 AndAlso curCol < 0 Then
						curRow = curY + curHeight
					End If
				Next compindex 'for (components) loop


	'            
	'             * Apply minimum row/column dimensions
	'             
				If columnWidths IsNot Nothing AndAlso layoutWidth < columnWidths.Length Then layoutWidth = columnWidths.Length
				If rowHeights IsNot Nothing AndAlso layoutHeight < rowHeights.Length Then layoutHeight = rowHeights.Length

				r = New GridBagLayoutInfo(layoutWidth, layoutHeight)

	'            
	'             * Pass #2
	'             *
	'             * Negative values for gridX are filled in with the current x value.
	'             * Negative values for gridY are filled in with the current y value.
	'             * Negative or zero values for gridWidth and gridHeight end the current
	'             *  row or column, respectively.
	'             

					curCol = -1
					curRow = curCol

				java.util.Arrays.fill(xMaxArray, 0)
				java.util.Arrays.fill(yMaxArray, 0)

				Dim maxAscent As Integer() = Nothing
				Dim maxDescent As Integer() = Nothing
				Dim baselineType As Short() = Nothing

				If hasBaseline Then
						maxAscent = New Integer(layoutHeight - 1){}
						r.maxAscent = maxAscent
						maxDescent = New Integer(layoutHeight - 1){}
						r.maxDescent = maxDescent
						baselineType = New Short(layoutHeight - 1){}
						r.baselineType = baselineType
					r.hasBaseline_Renamed = True
				End If


				For compindex = 0 To components.Length - 1
					comp = components(compindex)
					If Not comp.visible Then Continue For
					constraints_Renamed = lookupConstraints(comp)

					curX = constraints_Renamed.gridx
					curY = constraints_Renamed.gridy
					curWidth = constraints_Renamed.gridwidth
					curHeight = constraints_Renamed.gridheight

					' If x or y is negative, then use relative positioning: 
					If curX < 0 AndAlso curY < 0 Then
						If curRow >= 0 Then
							curY = curRow
						ElseIf curCol >= 0 Then
							curX = curCol
						Else
							curY = 0
						End If
					End If

					If curX < 0 Then
						If curHeight <= 0 Then
							curHeight += r.height - curY
							If curHeight < 1 Then curHeight = 1
						End If

						px = 0
						For i = curY To (curY + curHeight) - 1
							px = System.Math.Max(px, xMaxArray(i))
						Next i

						curX = px - curX - 1
						If curX < 0 Then curX = 0
					ElseIf curY < 0 Then
						If curWidth <= 0 Then
							curWidth += r.width - curX
							If curWidth < 1 Then curWidth = 1
						End If

						py = 0
						For i = curX To (curX + curWidth) - 1
							py = System.Math.Max(py, yMaxArray(i))
						Next i

						curY = py - curY - 1
						If curY < 0 Then curY = 0
					End If

					If curWidth <= 0 Then
						curWidth += r.width - curX
						If curWidth < 1 Then curWidth = 1
					End If

					If curHeight <= 0 Then
						curHeight += r.height - curY
						If curHeight < 1 Then curHeight = 1
					End If

					px = curX + curWidth
					py = curY + curHeight

					For i = curX To (curX + curWidth) - 1
						yMaxArray(i) = py
					Next i
					For i = curY To (curY + curHeight) - 1
						xMaxArray(i) = px
					Next i

					' Make negative sizes start a new row/column 
					If constraints_Renamed.gridheight = 0 AndAlso constraints_Renamed.gridwidth = 0 Then
							curCol = -1
							curRow = curCol
					End If
					If constraints_Renamed.gridheight = 0 AndAlso curRow < 0 Then
						curCol = curX + curWidth
					ElseIf constraints_Renamed.gridwidth = 0 AndAlso curCol < 0 Then
						curRow = curY + curHeight
					End If

					' Assign the new values to the gridbag slave 
					constraints_Renamed.tempX = curX
					constraints_Renamed.tempY = curY
					constraints_Renamed.tempWidth = curWidth
					constraints_Renamed.tempHeight = curHeight

					anchor = constraints_Renamed.anchor
					If hasBaseline Then
						Select Case anchor
						Case GridBagConstraints.BASELINE, GridBagConstraints.BASELINE_LEADING, GridBagConstraints.BASELINE_TRAILING
							If constraints_Renamed.ascent >= 0 Then
								If curHeight = 1 Then
									maxAscent(curY) = System.Math.Max(maxAscent(curY), constraints_Renamed.ascent)
									maxDescent(curY) = System.Math.Max(maxDescent(curY), constraints_Renamed.descent)
								Else
									If constraints_Renamed.baselineResizeBehavior = Component.BaselineResizeBehavior.CONSTANT_DESCENT Then
										maxDescent(curY + curHeight - 1) = System.Math.Max(maxDescent(curY + curHeight - 1), constraints_Renamed.descent)
									Else
										maxAscent(curY) = System.Math.Max(maxAscent(curY), constraints_Renamed.ascent)
									End If
								End If
								If constraints_Renamed.baselineResizeBehavior = Component.BaselineResizeBehavior.CONSTANT_DESCENT Then
									baselineType(curY + curHeight - 1) = baselineType(curY + curHeight - 1) Or (1 << constraints_Renamed.baselineResizeBehavior.ordinal())
								Else
									baselineType(curY) = baselineType(curY) Or (1 << constraints_Renamed.baselineResizeBehavior.ordinal())
								End If
							End If
						Case GridBagConstraints.ABOVE_BASELINE, GridBagConstraints.ABOVE_BASELINE_LEADING, GridBagConstraints.ABOVE_BASELINE_TRAILING
							' Component positioned above the baseline.
							' To make the bottom edge of the component aligned
							' with the baseline the bottom inset is
							' added to the descent, the rest to the ascent.
							pixels_diff = constraints_Renamed.minHeight + constraints_Renamed.insets.top + constraints_Renamed.ipady
							maxAscent(curY) = System.Math.Max(maxAscent(curY), pixels_diff)
							maxDescent(curY) = System.Math.Max(maxDescent(curY), constraints_Renamed.insets.bottom)
						Case GridBagConstraints.BELOW_BASELINE, GridBagConstraints.BELOW_BASELINE_LEADING, GridBagConstraints.BELOW_BASELINE_TRAILING
							' Component positioned below the baseline.
							' To make the top edge of the component aligned
							' with the baseline the top inset is
							' added to the ascent, the rest to the descent.
							pixels_diff = constraints_Renamed.minHeight + constraints_Renamed.insets.bottom + constraints_Renamed.ipady
							maxDescent(curY) = System.Math.Max(maxDescent(curY), pixels_diff)
							maxAscent(curY) = System.Math.Max(maxAscent(curY), constraints_Renamed.insets.top)
						End Select
					End If
				Next compindex

				r.weightX = New Double(maximumArrayYIndex - 1){}
				r.weightY = New Double(maximumArrayXIndex - 1){}
				r.minWidth = New Integer(maximumArrayYIndex - 1){}
				r.minHeight = New Integer(maximumArrayXIndex - 1){}


	'            
	'             * Apply minimum row/column dimensions and weights
	'             
				If columnWidths IsNot Nothing Then Array.Copy(columnWidths, 0, r.minWidth, 0, columnWidths.Length)
				If rowHeights IsNot Nothing Then Array.Copy(rowHeights, 0, r.minHeight, 0, rowHeights.Length)
				If columnWeights IsNot Nothing Then Array.Copy(columnWeights, 0, r.weightX, 0, System.Math.Min(r.weightX.Length, columnWeights.Length))
				If rowWeights IsNot Nothing Then Array.Copy(rowWeights, 0, r.weightY, 0, System.Math.Min(r.weightY.Length, rowWeights.Length))

	'            
	'             * Pass #3
	'             *
	'             * Distribute the minimun widths and weights:
	'             

				nextSize =  java.lang.[Integer].Max_Value

				i = 1
				Do While i <>  java.lang.[Integer].Max_Value
					For compindex = 0 To components.Length - 1
						comp = components(compindex)
						If Not comp.visible Then Continue For
						constraints_Renamed = lookupConstraints(comp)

						If constraints_Renamed.tempWidth = i Then
							px = constraints_Renamed.tempX + constraints_Renamed.tempWidth ' right column

	'                        
	'                         * Figure out if we should use this slave\'s weight.  If the weight
	'                         * is less than the total weight spanned by the width of the cell,
	'                         * then discard the weight.  Otherwise split the difference
	'                         * according to the existing weights.
	'                         

							weight_diff = constraints_Renamed.weightx
							For k = constraints_Renamed.tempX To px - 1
								weight_diff -= r.weightX(k)
							Next k
							If weight_diff > 0.0 Then
								weight = 0.0
								For k = constraints_Renamed.tempX To px - 1
									weight += r.weightX(k)
								Next k
								k = constraints_Renamed.tempX
								Do While weight > 0.0 AndAlso k < px
									Dim wt As Double = r.weightX(k)
									Dim dx As Double = (wt * weight_diff) / weight
									r.weightX(k) += dx
									weight_diff -= dx
									weight -= wt
									k += 1
								Loop
								' Assign the remainder to the rightmost cell 
								r.weightX(px-1) += weight_diff
							End If

	'                        
	'                         * Calculate the minWidth array values.
	'                         * First, figure out how wide the current slave needs to be.
	'                         * Then, see if it will fit within the current minWidth values.
	'                         * If it will not fit, add the difference according to the
	'                         * weightX array.
	'                         

							pixels_diff = constraints_Renamed.minWidth + constraints_Renamed.ipadx + constraints_Renamed.insets.left + constraints_Renamed.insets.right

							For k = constraints_Renamed.tempX To px - 1
								pixels_diff -= r.minWidth(k)
							Next k
							If pixels_diff > 0 Then
								weight = 0.0
								For k = constraints_Renamed.tempX To px - 1
									weight += r.weightX(k)
								Next k
								k = constraints_Renamed.tempX
								Do While weight > 0.0 AndAlso k < px
									Dim wt As Double = r.weightX(k)
									Dim dx As Integer = CInt(Fix((wt * (CDbl(pixels_diff))) / weight))
									r.minWidth(k) += dx
									pixels_diff -= dx
									weight -= wt
									k += 1
								Loop
								' Any leftovers go into the rightmost cell 
								r.minWidth(px-1) += pixels_diff
							End If
						ElseIf constraints_Renamed.tempWidth > i AndAlso constraints_Renamed.tempWidth < nextSize Then
							nextSize = constraints_Renamed.tempWidth
						End If


						If constraints_Renamed.tempHeight = i Then
							py = constraints_Renamed.tempY + constraints_Renamed.tempHeight ' bottom row

	'                        
	'                         * Figure out if we should use this slave's weight.  If the weight
	'                         * is less than the total weight spanned by the height of the cell,
	'                         * then discard the weight.  Otherwise split it the difference
	'                         * according to the existing weights.
	'                         

							weight_diff = constraints_Renamed.weighty
							For k = constraints_Renamed.tempY To py - 1
								weight_diff -= r.weightY(k)
							Next k
							If weight_diff > 0.0 Then
								weight = 0.0
								For k = constraints_Renamed.tempY To py - 1
									weight += r.weightY(k)
								Next k
								k = constraints_Renamed.tempY
								Do While weight > 0.0 AndAlso k < py
									Dim wt As Double = r.weightY(k)
									Dim dy As Double = (wt * weight_diff) / weight
									r.weightY(k) += dy
									weight_diff -= dy
									weight -= wt
									k += 1
								Loop
								' Assign the remainder to the bottom cell 
								r.weightY(py-1) += weight_diff
							End If

	'                        
	'                         * Calculate the minHeight array values.
	'                         * First, figure out how tall the current slave needs to be.
	'                         * Then, see if it will fit within the current minHeight values.
	'                         * If it will not fit, add the difference according to the
	'                         * weightY array.
	'                         

							pixels_diff = -1
							If hasBaseline Then
								Select Case constraints_Renamed.anchor
								Case GridBagConstraints.BASELINE, GridBagConstraints.BASELINE_LEADING, GridBagConstraints.BASELINE_TRAILING
									If constraints_Renamed.ascent >= 0 Then
										If constraints_Renamed.tempHeight = 1 Then
											pixels_diff = maxAscent(constraints_Renamed.tempY) + maxDescent(constraints_Renamed.tempY)
										ElseIf constraints_Renamed.baselineResizeBehavior <> Component.BaselineResizeBehavior.CONSTANT_DESCENT Then
											pixels_diff = maxAscent(constraints_Renamed.tempY) + constraints_Renamed.descent
										Else
											pixels_diff = constraints_Renamed.ascent + maxDescent(constraints_Renamed.tempY + constraints_Renamed.tempHeight - 1)
										End If
									End If
								Case GridBagConstraints.ABOVE_BASELINE, GridBagConstraints.ABOVE_BASELINE_LEADING, GridBagConstraints.ABOVE_BASELINE_TRAILING
									pixels_diff = constraints_Renamed.insets.top + constraints_Renamed.minHeight + constraints_Renamed.ipady + maxDescent(constraints_Renamed.tempY)
								Case GridBagConstraints.BELOW_BASELINE, GridBagConstraints.BELOW_BASELINE_LEADING, GridBagConstraints.BELOW_BASELINE_TRAILING
									pixels_diff = maxAscent(constraints_Renamed.tempY) + constraints_Renamed.minHeight + constraints_Renamed.insets.bottom + constraints_Renamed.ipady
								End Select
							End If
							If pixels_diff = -1 Then pixels_diff = constraints_Renamed.minHeight + constraints_Renamed.ipady + constraints_Renamed.insets.top + constraints_Renamed.insets.bottom
							For k = constraints_Renamed.tempY To py - 1
								pixels_diff -= r.minHeight(k)
							Next k
							If pixels_diff > 0 Then
								weight = 0.0
								For k = constraints_Renamed.tempY To py - 1
									weight += r.weightY(k)
								Next k
								k = constraints_Renamed.tempY
								Do While weight > 0.0 AndAlso k < py
									Dim wt As Double = r.weightY(k)
									Dim dy As Integer = CInt(Fix((wt * (CDbl(pixels_diff))) / weight))
									r.minHeight(k) += dy
									pixels_diff -= dy
									weight -= wt
									k += 1
								Loop
								' Any leftovers go into the bottom cell 
								r.minHeight(py-1) += pixels_diff
							End If
						ElseIf constraints_Renamed.tempHeight > i AndAlso constraints_Renamed.tempHeight < nextSize Then
							nextSize = constraints_Renamed.tempHeight
						End If
					Next compindex
					i = nextSize
					nextSize =  java.lang.[Integer].Max_Value
				Loop
				Return r
			End SyncLock
		End Function 'getLayoutInfo()

		''' <summary>
		''' Calculate the baseline for the specified component.
		''' If {@code c} is positioned along it's baseline, the baseline is
		''' obtained and the {@code constraints} ascent, descent and
		''' baseline resize behavior are set from the component; and true is
		''' returned. Otherwise false is returned.
		''' </summary>
		Private Function calculateBaseline(ByVal c As Component, ByVal constraints As GridBagConstraints, ByVal size As Dimension) As Boolean
			Dim anchor As Integer = constraints.anchor
			If anchor = GridBagConstraints.BASELINE OrElse anchor = GridBagConstraints.BASELINE_LEADING OrElse anchor = GridBagConstraints.BASELINE_TRAILING Then
				' Apply the padding to the component, then ask for the baseline.
				Dim w As Integer = size.width + constraints.ipadx
				Dim h As Integer = size.height + constraints.ipady
				constraints.ascent = c.getBaseline(w, h)
				If constraints.ascent >= 0 Then
					' Component has a baseline
					Dim baseline As Integer = constraints.ascent
					' Adjust the ascent and descent to include the insets.
					constraints.descent = h - constraints.ascent + constraints.insets.bottom
					constraints.ascent += constraints.insets.top
					constraints.baselineResizeBehavior = c.baselineResizeBehavior
					constraints.centerPadding = 0
					If constraints.baselineResizeBehavior = Component.BaselineResizeBehavior.CENTER_OFFSET Then
						' Component has a baseline resize behavior of
						' CENTER_OFFSET, calculate centerPadding and
						' centerOffset (see the description of
						' CENTER_OFFSET in the enum for detais on this
						' algorithm).
						Dim nextBaseline As Integer = c.getBaseline(w, h + 1)
						constraints.centerOffset = baseline - h \ 2
						If h Mod 2 = 0 Then
							If baseline <> nextBaseline Then constraints.centerPadding = 1
						ElseIf baseline = nextBaseline Then
							constraints.centerOffset -= 1
							constraints.centerPadding = 1
						End If
					End If
				End If
				Return True
			Else
				constraints.ascent = -1
				Return False
			End If
		End Function

		''' <summary>
		''' Adjusts the x, y, width, and height fields to the correct
		''' values depending on the constraint geometry and pads.
		''' This method should only be used internally by
		''' <code>GridBagLayout</code>.
		''' </summary>
		''' <param name="constraints"> the constraints to be applied </param>
		''' <param name="r"> the <code>Rectangle</code> to be adjusted
		''' @since 1.4 </param>
		Protected Friend Overridable Sub adjustForGravity(ByVal constraints As GridBagConstraints, ByVal r As Rectangle)
			AdjustForGravity(constraints, r)
		End Sub

		''' <summary>
		''' This method is obsolete and supplied for backwards
		''' compatibility only; new code should call {@link
		''' #adjustForGravity(java.awt.GridBagConstraints, java.awt.Rectangle)
		''' adjustForGravity} instead.
		''' This method is the same as <code>adjustForGravity</code>;
		''' refer to <code>adjustForGravity</code> for details
		''' on parameters.
		''' </summary>
		Protected Friend Overridable Sub AdjustForGravity(ByVal constraints As GridBagConstraints, ByVal r As Rectangle)
			Dim diffx, diffy As Integer
			Dim cellY As Integer = r.y
			Dim cellHeight As Integer = r.height

			If Not rightToLeft Then
				r.x += constraints.insets.left
			Else
				r.x -= r.width - constraints.insets.right
			End If
			r.width -= (constraints.insets.left + constraints.insets.right)
			r.y += constraints.insets.top
			r.height -= (constraints.insets.top + constraints.insets.bottom)

			diffx = 0
			If (constraints.fill <> GridBagConstraints.HORIZONTAL AndAlso constraints.fill <> GridBagConstraints.BOTH) AndAlso (r.width > (constraints.minWidth + constraints.ipadx)) Then
				diffx = r.width - (constraints.minWidth + constraints.ipadx)
				r.width = constraints.minWidth + constraints.ipadx
			End If

			diffy = 0
			If (constraints.fill <> GridBagConstraints.VERTICAL AndAlso constraints.fill <> GridBagConstraints.BOTH) AndAlso (r.height > (constraints.minHeight + constraints.ipady)) Then
				diffy = r.height - (constraints.minHeight + constraints.ipady)
				r.height = constraints.minHeight + constraints.ipady
			End If

			Select Case constraints.anchor
			  Case GridBagConstraints.BASELINE
				  r.x += diffx\2
				  alignOnBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.BASELINE_LEADING
				  If rightToLeft Then r.x += diffx
				  alignOnBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.BASELINE_TRAILING
				  If Not rightToLeft Then r.x += diffx
				  alignOnBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.ABOVE_BASELINE
				  r.x += diffx\2
				  alignAboveBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.ABOVE_BASELINE_LEADING
				  If rightToLeft Then r.x += diffx
				  alignAboveBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.ABOVE_BASELINE_TRAILING
				  If Not rightToLeft Then r.x += diffx
				  alignAboveBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.BELOW_BASELINE
				  r.x += diffx\2
				  alignBelowBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.BELOW_BASELINE_LEADING
				  If rightToLeft Then r.x += diffx
				  alignBelowBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.BELOW_BASELINE_TRAILING
				  If Not rightToLeft Then r.x += diffx
				  alignBelowBaseline(constraints, r, cellY, cellHeight)
			  Case GridBagConstraints.CENTER
				  r.x += diffx\2
				  r.y += diffy\2
			  Case GridBagConstraints.PAGE_START, GridBagConstraints.NORTH
				  r.x += diffx\2
			  Case GridBagConstraints.NORTHEAST
				  r.x += diffx
			  Case GridBagConstraints.EAST
				  r.x += diffx
				  r.y += diffy\2
			  Case GridBagConstraints.SOUTHEAST
				  r.x += diffx
				  r.y += diffy
			  Case GridBagConstraints.PAGE_END, GridBagConstraints.SOUTH
				  r.x += diffx\2
				  r.y += diffy
			  Case GridBagConstraints.SOUTHWEST
				  r.y += diffy
			  Case GridBagConstraints.WEST
				  r.y += diffy\2
			  Case GridBagConstraints.NORTHWEST
			  Case GridBagConstraints.LINE_START
				  If rightToLeft Then r.x += diffx
				  r.y += diffy\2
			  Case GridBagConstraints.LINE_END
				  If Not rightToLeft Then r.x += diffx
				  r.y += diffy\2
			  Case GridBagConstraints.FIRST_LINE_START
				  If rightToLeft Then r.x += diffx
			  Case GridBagConstraints.FIRST_LINE_END
				  If Not rightToLeft Then r.x += diffx
			  Case GridBagConstraints.LAST_LINE_START
				  If rightToLeft Then r.x += diffx
				  r.y += diffy
			  Case GridBagConstraints.LAST_LINE_END
				  If Not rightToLeft Then r.x += diffx
				  r.y += diffy
			  Case Else
				  Throw New IllegalArgumentException("illegal anchor value")
			End Select
		End Sub

		''' <summary>
		''' Positions on the baseline.
		''' </summary>
		''' <param name="cellY"> the location of the row, does not include insets </param>
		''' <param name="cellHeight"> the height of the row, does not take into account
		'''        insets </param>
		''' <param name="r"> available bounds for the component, is padded by insets and
		'''        ipady </param>
		Private Sub alignOnBaseline(ByVal cons As GridBagConstraints, ByVal r As Rectangle, ByVal cellY As Integer, ByVal cellHeight As Integer)
			If cons.ascent >= 0 Then
				If cons.baselineResizeBehavior = Component.BaselineResizeBehavior.CONSTANT_DESCENT Then
					' Anchor to the bottom.
					' Baseline is at (cellY + cellHeight - maxDescent).
					' Bottom of component (maxY) is at baseline + descent
					' of component. We need to subtract the bottom inset here
					' as the descent in the constraints object includes the
					' bottom inset.
					Dim maxY As Integer = cellY + cellHeight - layoutInfo.maxDescent(cons.tempY + cons.tempHeight - 1) + cons.descent - cons.insets.bottom
					If Not cons.verticallyResizable Then
						' Component not resizable, calculate y location
						' from maxY - height.
						r.y = maxY - cons.minHeight
						r.height = cons.minHeight
					Else
						' Component is resizable. As brb is constant descent,
						' can expand component to fill region above baseline.
						' Subtract out the top inset so that components insets
						' are honored.
						r.height = maxY - cellY - cons.insets.top
					End If
				Else
					' BRB is not constant_descent
					Dim baseline As Integer ' baseline for the row, relative to cellY
					' Component baseline, includes insets.top
					Dim ascent As Integer = cons.ascent
					If layoutInfo.hasConstantDescent(cons.tempY) Then
						' Mixed ascent/descent in same row, calculate position
						' off maxDescent
						baseline = cellHeight - layoutInfo.maxDescent(cons.tempY)
					Else
						' Only ascents/unknown in this row, anchor to top
						baseline = layoutInfo.maxAscent(cons.tempY)
					End If
					If cons.baselineResizeBehavior = Component.BaselineResizeBehavior.OTHER Then
						' BRB is other, which means we can only determine
						' the baseline by asking for it again giving the
						' size we plan on using for the component.
						Dim fits As Boolean = False
						ascent = componentAdjusting.getBaseline(r.width, r.height)
						If ascent >= 0 Then ascent += cons.insets.top
						If ascent >= 0 AndAlso ascent <= baseline Then
							' Components baseline fits within rows baseline.
							' Make sure the descent fits within the space as well.
							If baseline + (r.height - ascent - cons.insets.top) <= cellHeight - cons.insets.bottom Then
								' It fits, we're good.
								fits = True
							ElseIf cons.verticallyResizable Then
								' Doesn't fit, but it's resizable.  Try
								' again assuming we'll get ascent again.
								Dim ascent2 As Integer = componentAdjusting.getBaseline(r.width, cellHeight - cons.insets.bottom - baseline + ascent)
								If ascent2 >= 0 Then ascent2 += cons.insets.top
								If ascent2 >= 0 AndAlso ascent2 <= ascent Then
									' It'll fit
									r.height = cellHeight - cons.insets.bottom - baseline + ascent
									ascent = ascent2
									fits = True
								End If
							End If
						End If
						If Not fits Then
							' Doesn't fit, use min size and original ascent
							ascent = cons.ascent
							r.width = cons.minWidth
							r.height = cons.minHeight
						End If
					End If
					' Reset the components y location based on
					' components ascent and baseline for row. Because ascent
					' includes the baseline
					r.y = cellY + baseline - ascent + cons.insets.top
					If cons.verticallyResizable Then
						Select Case cons.baselineResizeBehavior
						Case Component.BaselineResizeBehavior.CONSTANT_ASCENT
							r.height = System.Math.Max(cons.minHeight,cellY + cellHeight - r.y - cons.insets.bottom)
						Case Component.BaselineResizeBehavior.CENTER_OFFSET
								Dim upper As Integer = r.y - cellY - cons.insets.top
								Dim lower As Integer = cellY + cellHeight - r.y - cons.minHeight - cons.insets.bottom
								Dim delta As Integer = System.Math.Min(upper, lower)
								delta += delta
								If delta > 0 AndAlso (cons.minHeight + cons.centerPadding + delta) \ 2 + cons.centerOffset <> baseline Then delta -= 1
								r.height = cons.minHeight + delta
								r.y = cellY + baseline - (r.height + cons.centerPadding) \ 2 - cons.centerOffset
						Case Component.BaselineResizeBehavior.OTHER
							' Handled above
						Case Else
						End Select
					End If
				End If
			Else
				centerVertically(cons, r, cellHeight)
			End If
		End Sub

		''' <summary>
		''' Positions the specified component above the baseline. That is
		''' the bottom edge of the component will be aligned along the baseline.
		''' If the row does not have a baseline, this centers the component.
		''' </summary>
		Private Sub alignAboveBaseline(ByVal cons As GridBagConstraints, ByVal r As Rectangle, ByVal cellY As Integer, ByVal cellHeight As Integer)
			If layoutInfo.hasBaseline(cons.tempY) Then
				Dim maxY As Integer ' Baseline for the row
				If layoutInfo.hasConstantDescent(cons.tempY) Then
					' Prefer descent
					maxY = cellY + cellHeight - layoutInfo.maxDescent(cons.tempY)
				Else
					' Prefer ascent
					maxY = cellY + layoutInfo.maxAscent(cons.tempY)
				End If
				If cons.verticallyResizable Then
					' Component is resizable. Top edge is offset by top
					' inset, bottom edge on baseline.
					r.y = cellY + cons.insets.top
					r.height = maxY - r.y
				Else
					' Not resizable.
					r.height = cons.minHeight + cons.ipady
					r.y = maxY - r.height
				End If
			Else
				centerVertically(cons, r, cellHeight)
			End If
		End Sub

		''' <summary>
		''' Positions below the baseline.
		''' </summary>
		Private Sub alignBelowBaseline(ByVal cons As GridBagConstraints, ByVal r As Rectangle, ByVal cellY As Integer, ByVal cellHeight As Integer)
			If layoutInfo.hasBaseline(cons.tempY) Then
				If layoutInfo.hasConstantDescent(cons.tempY) Then
					' Prefer descent
					r.y = cellY + cellHeight - layoutInfo.maxDescent(cons.tempY)
				Else
					' Prefer ascent
					r.y = cellY + layoutInfo.maxAscent(cons.tempY)
				End If
				If cons.verticallyResizable Then r.height = cellY + cellHeight - r.y - cons.insets.bottom
			Else
				centerVertically(cons, r, cellHeight)
			End If
		End Sub

		Private Sub centerVertically(ByVal cons As GridBagConstraints, ByVal r As Rectangle, ByVal cellHeight As Integer)
			If Not cons.verticallyResizable Then r.y += System.Math.Max(0, (cellHeight - cons.insets.top - cons.insets.bottom - cons.minHeight - cons.ipady) \ 2)
		End Sub

		''' <summary>
		''' Figures out the minimum size of the
		''' master based on the information from <code>getLayoutInfo</code>.
		''' This method should only be used internally by
		''' <code>GridBagLayout</code>.
		''' </summary>
		''' <param name="parent"> the layout container </param>
		''' <param name="info"> the layout info for this parent </param>
		''' <returns> a <code>Dimension</code> object containing the
		'''   minimum size
		''' @since 1.4 </returns>
		Protected Friend Overridable Function getMinSize(ByVal parent As Container, ByVal info As GridBagLayoutInfo) As Dimension
			Return GetMinSize(parent, info)
		End Function

		''' <summary>
		''' This method is obsolete and supplied for backwards
		''' compatibility only; new code should call {@link
		''' #getMinSize(java.awt.Container, GridBagLayoutInfo) getMinSize} instead.
		''' This method is the same as <code>getMinSize</code>;
		''' refer to <code>getMinSize</code> for details on parameters
		''' and return value.
		''' </summary>
		Protected Friend Overridable Function GetMinSize(ByVal parent As Container, ByVal info As GridBagLayoutInfo) As Dimension
			Dim d As New Dimension
			Dim i, t As Integer
			Dim insets_Renamed As Insets = parent.insets

			t = 0
			For i = 0 To info.width - 1
				t += info.minWidth(i)
			Next i
			d.width = t + insets_Renamed.left + insets_Renamed.right

			t = 0
			For i = 0 To info.height - 1
				t += info.minHeight(i)
			Next i
			d.height = t + insets_Renamed.top + insets_Renamed.bottom

			Return d
		End Function

		<NonSerialized> _
		Friend rightToLeft As Boolean = False

		''' <summary>
		''' Lays out the grid.
		''' This method should only be used internally by
		''' <code>GridBagLayout</code>.
		''' </summary>
		''' <param name="parent"> the layout container
		''' @since 1.4 </param>
		Protected Friend Overridable Sub arrangeGrid(ByVal parent As Container)
			ArrangeGrid(parent)
		End Sub

		''' <summary>
		''' This method is obsolete and supplied for backwards
		''' compatibility only; new code should call {@link
		''' #arrangeGrid(Container) arrangeGrid} instead.
		''' This method is the same as <code>arrangeGrid</code>;
		''' refer to <code>arrangeGrid</code> for details on the
		''' parameter.
		''' </summary>
		Protected Friend Overridable Sub ArrangeGrid(ByVal parent As Container)
			Dim comp As Component
			Dim compindex As Integer
			Dim constraints_Renamed As GridBagConstraints
			Dim insets_Renamed As Insets = parent.insets
			Dim components As Component() = parent.components
			Dim d As Dimension
			Dim r As New Rectangle
			Dim i, diffw, diffh As Integer
			Dim weight As Double
			Dim info As GridBagLayoutInfo

			rightToLeft = Not parent.componentOrientation.leftToRight

	'        
	'         * If the parent has no slaves anymore, then don't do anything
	'         * at all:  just leave the parent's size as-is.
	'         
			If components.Length = 0 AndAlso (columnWidths Is Nothing OrElse columnWidths.Length = 0) AndAlso (rowHeights Is Nothing OrElse rowHeights.Length = 0) Then Return

	'        
	'         * Pass #1: scan all the slaves to figure out the total amount
	'         * of space needed.
	'         

			info = getLayoutInfo(parent, PREFERREDSIZE)
			d = getMinSize(parent, info)

			If parent.width < d.width OrElse parent.height < d.height Then
				info = getLayoutInfo(parent, MINSIZE)
				d = getMinSize(parent, info)
			End If

			layoutInfo = info
			r.width = d.width
			r.height = d.height

	'        
	'         * DEBUG
	'         *
	'         * DumpLayoutInfo(info);
	'         * for (compindex = 0 ; compindex < components.length ; compindex++) {
	'         * comp = components[compindex];
	'         * if (!comp.isVisible())
	'         *      continue;
	'         * constraints = lookupConstraints(comp);
	'         * DumpConstraints(constraints);
	'         * }
	'         * System.out.println("minSize " + r.width + " " + r.height);
	'         

	'        
	'         * If the current dimensions of the window don't match the desired
	'         * dimensions, then adjust the minWidth and minHeight arrays
	'         * according to the weights.
	'         

			diffw = parent.width - r.width
			If diffw <> 0 Then
				weight = 0.0
				For i = 0 To info.width - 1
					weight += info.weightX(i)
				Next i
				If weight > 0.0 Then
					For i = 0 To info.width - 1
						Dim dx As Integer = CInt(Fix(((CDbl(diffw)) * info.weightX(i)) / weight))
						info.minWidth(i) += dx
						r.width += dx
						If info.minWidth(i) < 0 Then
							r.width -= info.minWidth(i)
							info.minWidth(i) = 0
						End If
					Next i
				End If
				diffw = parent.width - r.width

			Else
				diffw = 0
			End If

			diffh = parent.height - r.height
			If diffh <> 0 Then
				weight = 0.0
				For i = 0 To info.height - 1
					weight += info.weightY(i)
				Next i
				If weight > 0.0 Then
					For i = 0 To info.height - 1
						Dim dy As Integer = CInt(Fix(((CDbl(diffh)) * info.weightY(i)) / weight))
						info.minHeight(i) += dy
						r.height += dy
						If info.minHeight(i) < 0 Then
							r.height -= info.minHeight(i)
							info.minHeight(i) = 0
						End If
					Next i
				End If
				diffh = parent.height - r.height

			Else
				diffh = 0
			End If

	'        
	'         * DEBUG
	'         *
	'         * System.out.println("Re-adjusted:");
	'         * DumpLayoutInfo(info);
	'         

	'        
	'         * Now do the actual layout of the slaves using the layout information
	'         * that has been collected.
	'         

			info.startx = diffw\2 + insets_Renamed.left
			info.starty = diffh\2 + insets_Renamed.top

			For compindex = 0 To components.Length - 1
				comp = components(compindex)
				If Not comp.visible Then Continue For
				constraints_Renamed = lookupConstraints(comp)

				If Not rightToLeft Then
					r.x = info.startx
					For i = 0 To constraints_Renamed.tempX - 1
						r.x += info.minWidth(i)
					Next i
				Else
					r.x = parent.width - (diffw\2 + insets_Renamed.right)
					For i = 0 To constraints_Renamed.tempX - 1
						r.x -= info.minWidth(i)
					Next i
				End If

				r.y = info.starty
				For i = 0 To constraints_Renamed.tempY - 1
					r.y += info.minHeight(i)
				Next i

				r.width = 0
				For i = constraints_Renamed.tempX To (constraints_Renamed.tempX + constraints_Renamed.tempWidth) - 1
					r.width += info.minWidth(i)
				Next i

				r.height = 0
				For i = constraints_Renamed.tempY To (constraints_Renamed.tempY + constraints_Renamed.tempHeight) - 1
					r.height += info.minHeight(i)
				Next i

				componentAdjusting = comp
				adjustForGravity(constraints_Renamed, r)

				' fix for 4408108 - components were being created outside of the container 
				' fix for 4969409 "-" replaced by "+"  
				If r.x < 0 Then
					r.width += r.x
					r.x = 0
				End If

				If r.y < 0 Then
					r.height += r.y
					r.y = 0
				End If

	'            
	'             * If the window is too small to be interesting then
	'             * unmap it.  Otherwise configure it and then make sure
	'             * it's mapped.
	'             

				If (r.width <= 0) OrElse (r.height <= 0) Then
					comp.boundsnds(0, 0, 0, 0)
				Else
					If comp.x <> r.x OrElse comp.y <> r.y OrElse comp.width <> r.width OrElse comp.height <> r.height Then comp.boundsnds(r.x, r.y, r.width, r.height)
				End If
			Next compindex
		End Sub

		' Added for serial backwards compatibility (4348425)
		Friend Const serialVersionUID As Long = 8838754796412211005L
	End Class

End Namespace