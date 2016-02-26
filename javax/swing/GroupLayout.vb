Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Collections.Generic
Imports System.Text
import static javax.swing.LayoutStyle.ComponentPlacement
import static javax.swing.SwingConstants.HORIZONTAL
import static javax.swing.SwingConstants.VERTICAL

'
' * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' {@code GroupLayout} is a {@code LayoutManager} that hierarchically
	''' groups components in order to position them in a {@code Container}.
	''' {@code GroupLayout} is intended for use by builders, but may be
	''' hand-coded as well.
	''' Grouping is done by instances of the <seealso cref="Group Group"/> class. {@code
	''' GroupLayout} supports two types of groups. A sequential group
	''' positions its child elements sequentially, one after another. A
	''' parallel group aligns its child elements in one of four ways.
	''' <p>
	''' Each group may contain any number of elements, where an element is
	''' a {@code Group}, {@code Component}, or gap. A gap can be thought
	''' of as an invisible component with a minimum, preferred and maximum
	''' size. In addition {@code GroupLayout} supports a preferred gap,
	''' whose value comes from {@code LayoutStyle}.
	''' <p>
	''' Elements are similar to a spring. Each element has a range as
	''' specified by a minimum, preferred and maximum.  Gaps have either a
	''' developer-specified range, or a range determined by {@code
	''' LayoutStyle}. The range for {@code Component}s is determined from
	''' the {@code Component}'s {@code getMinimumSize}, {@code
	''' getPreferredSize} and {@code getMaximumSize} methods. In addition,
	''' when adding {@code Component}s you may specify a particular range
	''' to use instead of that from the component. The range for a {@code
	''' Group} is determined by the type of group. A {@code ParallelGroup}'s
	''' range is the maximum of the ranges of its elements. A {@code
	''' SequentialGroup}'s range is the sum of the ranges of its elements.
	''' <p>
	''' {@code GroupLayout} treats each axis independently.  That is, there
	''' is a group representing the horizontal axis, and a group
	''' representing the vertical axis.  The horizontal group is
	''' responsible for determining the minimum, preferred and maximum size
	''' along the horizontal axis as well as setting the x and width of the
	''' components contained in it. The vertical group is responsible for
	''' determining the minimum, preferred and maximum size along the
	''' vertical axis as well as setting the y and height of the
	''' components contained in it. Each {@code Component} must exist in both
	''' a horizontal and vertical group, otherwise an {@code IllegalStateException}
	''' is thrown during layout, or when the minimum, preferred or
	''' maximum size is requested.
	''' <p>
	''' The following diagram shows a sequential group along the horizontal
	''' axis. The sequential group contains three components. A parallel group
	''' was used along the vertical axis.
	''' <p style="text-align:center">
	''' <img src="doc-files/groupLayout.1.gif" alt="Sequential group along the horizontal axis in three components">
	''' <p>
	''' To reinforce that each axis is treated independently the diagram shows
	''' the range of each group and element along each axis. The
	''' range of each component has been projected onto the axes,
	''' and the groups are rendered in blue (horizontal) and red (vertical).
	''' For readability there is a gap between each of the elements in the
	''' sequential group.
	''' <p>
	''' The sequential group along the horizontal axis is rendered as a solid
	''' blue line. Notice the sequential group is the sum of the children elements
	''' it contains.
	''' <p>
	''' Along the vertical axis the parallel group is the maximum of the height
	''' of each of the components. As all three components have the same height,
	''' the parallel group has the same height.
	''' <p>
	''' The following diagram shows the same three components, but with the
	''' parallel group along the horizontal axis and the sequential group along
	''' the vertical axis.
	''' 
	''' <p style="text-align:center">
	''' <img src="doc-files/groupLayout.2.gif" alt="Sequential group along the vertical axis in three components">
	''' <p>
	''' As {@code c1} is the largest of the three components, the parallel
	''' group is sized to {@code c1}. As {@code c2} and {@code c3} are smaller
	''' than {@code c1} they are aligned based on the alignment specified
	''' for the component (if specified) or the default alignment of the
	''' parallel group. In the diagram {@code c2} and {@code c3} were created
	''' with an alignment of {@code LEADING}. If the component orientation were
	''' right-to-left then {@code c2} and {@code c3} would be positioned on
	''' the opposite side.
	''' <p>
	''' The following diagram shows a sequential group along both the horizontal
	''' and vertical axis.
	''' <p style="text-align:center">
	''' <img src="doc-files/groupLayout.3.gif" alt="Sequential group along both the horizontal and vertical axis in three components">
	''' <p>
	''' {@code GroupLayout} provides the ability to insert gaps between
	''' {@code Component}s. The size of the gap is determined by an
	''' instance of {@code LayoutStyle}. This may be turned on using the
	''' {@code setAutoCreateGaps} method.  Similarly, you may use
	''' the {@code setAutoCreateContainerGaps} method to insert gaps
	''' between components that touch the edge of the parent container and the
	''' container.
	''' <p>
	''' The following builds a panel consisting of two labels in
	''' one column, followed by two textfields in the next column:
	''' <pre>
	'''   JComponent panel = ...;
	'''   GroupLayout layout = new GroupLayout(panel);
	'''   panel.setLayout(layout);
	''' 
	'''   // Turn on automatically adding gaps between components
	'''   layout.setAutoCreateGaps(true);
	''' 
	'''   // Turn on automatically creating gaps between components that touch
	'''   // the edge of the container and the container.
	'''   layout.setAutoCreateContainerGaps(true);
	''' 
	'''   // Create a sequential group for the horizontal axis.
	''' 
	'''   GroupLayout.SequentialGroup hGroup = layout.createSequentialGroup();
	''' 
	'''   // The sequential group in turn contains two parallel groups.
	'''   // One parallel group contains the labels, the other the text fields.
	'''   // Putting the labels in a parallel group along the horizontal axis
	'''   // positions them at the same x location.
	'''   //
	'''   // Variable indentation is used to reinforce the level of grouping.
	'''   hGroup.addGroup(layout.createParallelGroup().
	'''            addComponent(label1).addComponent(label2));
	'''   hGroup.addGroup(layout.createParallelGroup().
	'''            addComponent(tf1).addComponent(tf2));
	'''   layout.setHorizontalGroup(hGroup);
	''' 
	'''   // Create a sequential group for the vertical axis.
	'''   GroupLayout.SequentialGroup vGroup = layout.createSequentialGroup();
	''' 
	'''   // The sequential group contains two parallel groups that align
	'''   // the contents along the baseline. The first parallel group contains
	'''   // the first label and text field, and the second parallel group contains
	'''   // the second label and text field. By using a sequential group
	'''   // the labels and text fields are positioned vertically after one another.
	'''   vGroup.addGroup(layout.createParallelGroup(Alignment.BASELINE).
	'''            addComponent(label1).addComponent(tf1));
	'''   vGroup.addGroup(layout.createParallelGroup(Alignment.BASELINE).
	'''            addComponent(label2).addComponent(tf2));
	'''   layout.setVerticalGroup(vGroup);
	''' </pre>
	''' <p>
	''' When run the following is produced.
	''' <p style="text-align:center">
	''' <img src="doc-files/groupLayout.example.png" alt="Produced horizontal/vertical form">
	''' <p>
	''' This layout consists of the following.
	''' <ul><li>The horizontal axis consists of a sequential group containing two
	'''         parallel groups.  The first parallel group contains the labels,
	'''         and the second parallel group contains the text fields.
	'''     <li>The vertical axis consists of a sequential group
	'''         containing two parallel groups.  The parallel groups are configured
	'''         to align their components along the baseline. The first parallel
	'''         group contains the first label and first text field, and
	'''         the second group consists of the second label and second
	'''         text field.
	''' </ul>
	''' There are a couple of things to notice in this code:
	''' <ul>
	'''   <li>You need not explicitly add the components to the container; this
	'''       is indirectly done by using one of the {@code add} methods of
	'''       {@code Group}.
	'''   <li>The various {@code add} methods return
	'''       the caller.  This allows for easy chaining of invocations.  For
	'''       example, {@code group.addComponent(label1).addComponent(label2);} is
	'''       equivalent to
	'''       {@code group.addComponent(label1); group.addComponent(label2);}.
	'''   <li>There are no public constructors for {@code Group}s; instead
	'''       use the create methods of {@code GroupLayout}.
	''' </ul>
	''' 
	''' @author Tomas Pavek
	''' @author Jan Stola
	''' @author Scott Violet
	''' @since 1.6
	''' </summary>
	Public Class GroupLayout
		Implements java.awt.LayoutManager2

		' Used in size calculations
		Private Const MIN_SIZE As Integer = 0

		Private Const PREF_SIZE As Integer = 1

		Private Const MAX_SIZE As Integer = 2

		' Used by prepare, indicates min, pref or max isn't going to be used.
		Private Const SPECIFIC_SIZE As Integer = 3

		Private Shared ReadOnly UNSET As Integer = Integer.MIN_VALUE

		''' <summary>
		''' Indicates the size from the component or gap should be used for a
		''' particular range value.
		''' </summary>
		''' <seealso cref= Group </seealso>
		Public Const DEFAULT_SIZE As Integer = -1

		''' <summary>
		''' Indicates the preferred size from the component or gap should
		''' be used for a particular range value.
		''' </summary>
		''' <seealso cref= Group </seealso>
		Public Const PREFERRED_SIZE As Integer = -2

		' Whether or not we automatically try and create the preferred
		' padding between components.
		Private autocreatePadding As Boolean

		' Whether or not we automatically try and create the preferred
		' padding between components the touch the edge of the container and
		' the container.
		Private autocreateContainerPadding As Boolean

		''' <summary>
		''' Group responsible for layout along the horizontal axis.  This is NOT
		''' the user specified group, use getHorizontalGroup to dig that out.
		''' </summary>
		Private horizontalGroup As Group

		''' <summary>
		''' Group responsible for layout along the vertical axis.  This is NOT
		''' the user specified group, use getVerticalGroup to dig that out.
		''' </summary>
		Private verticalGroup As Group

		' Maps from Component to ComponentInfo.  This is used for tracking
		' information specific to a Component.
		Private componentInfos As IDictionary(Of java.awt.Component, ComponentInfo)

		' Container we're doing layout for.
		Private host As java.awt.Container

		' Used by areParallelSiblings, cached to avoid excessive garbage.
		Private tmpParallelSet As [Set](Of Spring)

		' Indicates Springs have changed in some way since last change.
		Private springsChanged As Boolean

		' Indicates invalidateLayout has been invoked.
		Private isValid As Boolean

		' Whether or not any preferred padding (or container padding) springs
		' exist
		Private hasPreferredPaddingSprings As Boolean

		''' <summary>
		''' The LayoutStyle instance to use, if null the sharedInstance is used.
		''' </summary>
		Private layoutStyle As LayoutStyle

		''' <summary>
		''' If true, components that are not visible are treated as though they
		''' aren't there.
		''' </summary>
		Private honorsVisibility As Boolean


		''' <summary>
		''' Enumeration of the possible ways {@code ParallelGroup} can align
		''' its children.
		''' </summary>
		''' <seealso cref= #createParallelGroup(Alignment)
		''' @since 1.6 </seealso>
		Public Enum Alignment
			''' <summary>
			''' Indicates the elements should be
			''' aligned to the origin.  For the horizontal axis with a left to
			''' right orientation this means aligned to the left edge. For the
			''' vertical axis leading means aligned to the top edge.
			''' </summary>
			''' <seealso cref= #createParallelGroup(Alignment) </seealso>
			LEADING

			''' <summary>
			''' Indicates the elements should be aligned to the end of the
			''' region.  For the horizontal axis with a left to right
			''' orientation this means aligned to the right edge. For the
			''' vertical axis trailing means aligned to the bottom edge.
			''' </summary>
			''' <seealso cref= #createParallelGroup(Alignment) </seealso>
			TRAILING

			''' <summary>
			''' Indicates the elements should be centered in
			''' the region.
			''' </summary>
			''' <seealso cref= #createParallelGroup(Alignment) </seealso>
			CENTER

			''' <summary>
			''' Indicates the elements should be aligned along
			''' their baseline.
			''' </summary>
			''' <seealso cref= #createParallelGroup(Alignment) </seealso>
			''' <seealso cref= #createBaselineGroup(boolean,boolean) </seealso>
			BASELINE
		End Enum


		Private Shared Sub checkSize(ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer, ByVal isComponentSpring As Boolean)
			checkResizeType(min, isComponentSpring)
			If (Not isComponentSpring) AndAlso pref < 0 Then
				Throw New System.ArgumentException("Pref must be >= 0")
			ElseIf isComponentSpring Then
				checkResizeType(pref, True)
			End If
			checkResizeType(max, isComponentSpring)
			checkLessThan(min, pref)
			checkLessThan(pref, max)
		End Sub

		Private Shared Sub checkResizeType(ByVal type As Integer, ByVal isComponentSpring As Boolean)
			If type < 0 AndAlso ((isComponentSpring AndAlso type <> DEFAULT_SIZE AndAlso type <> PREFERRED_SIZE) OrElse ((Not isComponentSpring) AndAlso type <> PREFERRED_SIZE)) Then Throw New System.ArgumentException("Invalid size")
		End Sub

		Private Shared Sub checkLessThan(ByVal min As Integer, ByVal max As Integer)
			If min >= 0 AndAlso max >= 0 AndAlso min > max Then Throw New System.ArgumentException("Following is not met: min<=pref<=max")
		End Sub

		''' <summary>
		''' Creates a {@code GroupLayout} for the specified {@code Container}.
		''' </summary>
		''' <param name="host"> the {@code Container} the {@code GroupLayout} is
		'''        the {@code LayoutManager} for </param>
		''' <exception cref="IllegalArgumentException"> if host is {@code null} </exception>
		Public Sub New(ByVal host As java.awt.Container)
			If host Is Nothing Then Throw New System.ArgumentException("Container must be non-null")
			honorsVisibility = True
			Me.host = host
			horizontalGroup = createParallelGroup(Alignment.LEADING, True)
			verticalGroup = createParallelGroup(Alignment.LEADING, True)
			componentInfos = New Dictionary(Of java.awt.Component, ComponentInfo)
			tmpParallelSet = New HashSet(Of Spring)
		End Sub

		''' <summary>
		''' Sets whether component visibility is considered when sizing and
		''' positioning components. A value of {@code true} indicates that
		''' non-visible components should not be treated as part of the
		''' layout. A value of {@code false} indicates that components should be
		''' positioned and sized regardless of visibility.
		''' <p>
		''' A value of {@code false} is useful when the visibility of components
		''' is dynamically adjusted and you don't want surrounding components and
		''' the sizing to change.
		''' <p>
		''' The specified value is used for components that do not have an
		''' explicit visibility specified.
		''' <p>
		''' The default is {@code true}.
		''' </summary>
		''' <param name="honorsVisibility"> whether component visibility is considered when
		'''                         sizing and positioning components </param>
		''' <seealso cref= #setHonorsVisibility(Component,Boolean) </seealso>
		Public Overridable Property honorsVisibility As Boolean
			Set(ByVal honorsVisibility As Boolean)
				If Me.honorsVisibility <> honorsVisibility Then
					Me.honorsVisibility = honorsVisibility
					springsChanged = True
					isValid = False
					invalidateHost()
				End If
			End Set
			Get
				Return honorsVisibility
			End Get
		End Property


		''' <summary>
		''' Sets whether the component's visibility is considered for
		''' sizing and positioning. A value of {@code Boolean.TRUE}
		''' indicates that if {@code component} is not visible it should
		''' not be treated as part of the layout. A value of {@code false}
		''' indicates that {@code component} is positioned and sized
		''' regardless of it's visibility.  A value of {@code null}
		''' indicates the value specified by the single argument method {@code
		''' setHonorsVisibility} should be used.
		''' <p>
		''' If {@code component} is not a child of the {@code Container} this
		''' {@code GroupLayout} is managing, it will be added to the
		''' {@code Container}.
		''' </summary>
		''' <param name="component"> the component </param>
		''' <param name="honorsVisibility"> whether visibility of this {@code component} should be
		'''              considered for sizing and positioning </param>
		''' <exception cref="IllegalArgumentException"> if {@code component} is {@code null} </exception>
		''' <seealso cref= #setHonorsVisibility(Component,Boolean) </seealso>
		Public Overridable Sub setHonorsVisibility(ByVal component As java.awt.Component, ByVal honorsVisibility As Boolean?)
			If component Is Nothing Then Throw New System.ArgumentException("Component must be non-null")
			getComponentInfo(component).honorsVisibility = honorsVisibility
			springsChanged = True
			isValid = False
			invalidateHost()
		End Sub

		''' <summary>
		''' Sets whether a gap between components should automatically be
		''' created.  For example, if this is {@code true} and you add two
		''' components to a {@code SequentialGroup} a gap between the
		''' two components is automatically be created.  The default is
		''' {@code false}.
		''' </summary>
		''' <param name="autoCreatePadding"> whether a gap between components is
		'''        automatically created </param>
		Public Overridable Property autoCreateGaps As Boolean
			Set(ByVal autoCreatePadding As Boolean)
				If Me.autocreatePadding <> autoCreatePadding Then
					Me.autocreatePadding = autoCreatePadding
					invalidateHost()
				End If
			End Set
			Get
				Return autocreatePadding
			End Get
		End Property


		''' <summary>
		''' Sets whether a gap between the container and components that
		''' touch the border of the container should automatically be
		''' created. The default is {@code false}.
		''' </summary>
		''' <param name="autoCreateContainerPadding"> whether a gap between the container and
		'''        components that touch the border of the container should
		'''        automatically be created </param>
		Public Overridable Property autoCreateContainerGaps As Boolean
			Set(ByVal autoCreateContainerPadding As Boolean)
				If Me.autocreateContainerPadding <> autoCreateContainerPadding Then
					Me.autocreateContainerPadding = autoCreateContainerPadding
					horizontalGroup = createTopLevelGroup(horizontalGroup)
					verticalGroup = createTopLevelGroup(verticalGroup)
					invalidateHost()
				End If
			End Set
			Get
				Return autocreateContainerPadding
			End Get
		End Property


		''' <summary>
		''' Sets the {@code Group} that positions and sizes
		''' components along the horizontal axis.
		''' </summary>
		''' <param name="group"> the {@code Group} that positions and sizes
		'''        components along the horizontal axis </param>
		''' <exception cref="IllegalArgumentException"> if group is {@code null} </exception>
		Public Overridable Property horizontalGroup As Group
			Set(ByVal group As Group)
				If group Is Nothing Then Throw New System.ArgumentException("Group must be non-null")
				horizontalGroup = createTopLevelGroup(group)
				invalidateHost()
			End Set
			Get
				Dim index As Integer = 0
				If horizontalGroup.springs.Count > 1 Then index = 1
				Return CType(horizontalGroup.springs(index), Group)
			End Get
		End Property


		''' <summary>
		''' Sets the {@code Group} that positions and sizes
		''' components along the vertical axis.
		''' </summary>
		''' <param name="group"> the {@code Group} that positions and sizes
		'''        components along the vertical axis </param>
		''' <exception cref="IllegalArgumentException"> if group is {@code null} </exception>
		Public Overridable Property verticalGroup As Group
			Set(ByVal group As Group)
				If group Is Nothing Then Throw New System.ArgumentException("Group must be non-null")
				verticalGroup = createTopLevelGroup(group)
				invalidateHost()
			End Set
			Get
				Dim index As Integer = 0
				If verticalGroup.springs.Count > 1 Then index = 1
				Return CType(verticalGroup.springs(index), Group)
			End Get
		End Property


		''' <summary>
		''' Wraps the user specified group in a sequential group.  If
		''' container gaps should be generated the necessary springs are
		''' added.
		''' </summary>
		Private Function createTopLevelGroup(ByVal specifiedGroup As Group) As Group
			Dim group As SequentialGroup = createSequentialGroup()
			If autoCreateContainerGaps Then
				group.addSpring(New ContainerAutoPreferredGapSpring(Me))
				group.addGroup(specifiedGroup)
				group.addSpring(New ContainerAutoPreferredGapSpring(Me))
			Else
				group.addGroup(specifiedGroup)
			End If
			Return group
		End Function

		''' <summary>
		''' Creates and returns a {@code SequentialGroup}.
		''' </summary>
		''' <returns> a new {@code SequentialGroup} </returns>
		Public Overridable Function createSequentialGroup() As SequentialGroup
			Return New SequentialGroup(Me)
		End Function

		''' <summary>
		''' Creates and returns a {@code ParallelGroup} with an alignment of
		''' {@code Alignment.LEADING}.  This is a cover method for the more
		''' general {@code createParallelGroup(Alignment)} method.
		''' </summary>
		''' <returns> a new {@code ParallelGroup} </returns>
		''' <seealso cref= #createParallelGroup(Alignment) </seealso>
		Public Overridable Function createParallelGroup() As ParallelGroup
			Return createParallelGroup(Alignment.LEADING)
		End Function

		''' <summary>
		''' Creates and returns a {@code ParallelGroup} with the specified
		''' alignment.  This is a cover method for the more general {@code
		''' createParallelGroup(Alignment,boolean)} method with {@code true}
		''' supplied for the second argument.
		''' </summary>
		''' <param name="alignment"> the alignment for the elements of the group </param>
		''' <exception cref="IllegalArgumentException"> if {@code alignment} is {@code null} </exception>
		''' <returns> a new {@code ParallelGroup} </returns>
		''' <seealso cref= #createBaselineGroup </seealso>
		''' <seealso cref= ParallelGroup </seealso>
		Public Overridable Function createParallelGroup(ByVal alignment As Alignment) As ParallelGroup
			Return createParallelGroup(alignment, True)
		End Function

		''' <summary>
		''' Creates and returns a {@code ParallelGroup} with the specified
		''' alignment and resize behavior. The {@code
		''' alignment} argument specifies how children elements are
		''' positioned that do not fill the group. For example, if a {@code
		''' ParallelGroup} with an alignment of {@code TRAILING} is given
		''' 100 and a child only needs 50, the child is
		''' positioned at the position 50 (with a component orientation of
		''' left-to-right).
		''' <p>
		''' Baseline alignment is only useful when used along the vertical
		''' axis. A {@code ParallelGroup} created with a baseline alignment
		''' along the horizontal axis is treated as {@code LEADING}.
		''' <p>
		''' Refer to <seealso cref="GroupLayout.ParallelGroup ParallelGroup"/> for details on
		''' the behavior of baseline groups.
		''' </summary>
		''' <param name="alignment"> the alignment for the elements of the group </param>
		''' <param name="resizable"> {@code true} if the group is resizable; if the group
		'''        is not resizable the preferred size is used for the
		'''        minimum and maximum size of the group </param>
		''' <exception cref="IllegalArgumentException"> if {@code alignment} is {@code null} </exception>
		''' <returns> a new {@code ParallelGroup} </returns>
		''' <seealso cref= #createBaselineGroup </seealso>
		''' <seealso cref= GroupLayout.ParallelGroup </seealso>
		Public Overridable Function createParallelGroup(ByVal alignment As Alignment, ByVal resizable As Boolean) As ParallelGroup
			If alignment Is Nothing Then Throw New System.ArgumentException("alignment must be non null")

			If alignment = Alignment.BASELINE Then Return New BaselineGroup(Me, resizable)
			Return New ParallelGroup(Me, alignment, resizable)
		End Function

		''' <summary>
		''' Creates and returns a {@code ParallelGroup} that aligns it's
		''' elements along the baseline.
		''' </summary>
		''' <param name="resizable"> whether the group is resizable </param>
		''' <param name="anchorBaselineToTop"> whether the baseline is anchored to
		'''        the top or bottom of the group </param>
		''' <seealso cref= #createBaselineGroup </seealso>
		''' <seealso cref= ParallelGroup </seealso>
		Public Overridable Function createBaselineGroup(ByVal resizable As Boolean, ByVal anchorBaselineToTop As Boolean) As ParallelGroup
			Return New BaselineGroup(Me, resizable, anchorBaselineToTop)
		End Function

		''' <summary>
		''' Forces the specified components to have the same size
		''' regardless of their preferred, minimum or maximum sizes. Components that
		''' are linked are given the maximum of the preferred size of each of
		''' the linked components. For example, if you link two components with
		''' a preferred width of 10 and 20, both components are given a width of 20.
		''' <p>
		''' This can be used multiple times to force any number of
		''' components to share the same size.
		''' <p>
		''' Linked Components are not be resizable.
		''' </summary>
		''' <param name="components"> the {@code Component}s that are to have the same size </param>
		''' <exception cref="IllegalArgumentException"> if {@code components} is
		'''         {@code null}, or contains {@code null} </exception>
		''' <seealso cref= #linkSize(int,Component[]) </seealso>
		Public Overridable Sub linkSize(ParamArray ByVal components As java.awt.Component())
			linkSize(SwingConstants.HORIZONTAL, components)
			linkSize(SwingConstants.VERTICAL, components)
		End Sub

		''' <summary>
		''' Forces the specified components to have the same size along the
		''' specified axis regardless of their preferred, minimum or
		''' maximum sizes. Components that are linked are given the maximum
		''' of the preferred size of each of the linked components. For
		''' example, if you link two components along the horizontal axis
		''' and the preferred width is 10 and 20, both components are given
		''' a width of 20.
		''' <p>
		''' This can be used multiple times to force any number of
		''' components to share the same size.
		''' <p>
		''' Linked {@code Component}s are not be resizable.
		''' </summary>
		''' <param name="components"> the {@code Component}s that are to have the same size </param>
		''' <param name="axis"> the axis to link the size along; one of
		'''             {@code SwingConstants.HORIZONTAL} or
		'''             {@code SwingConstans.VERTICAL} </param>
		''' <exception cref="IllegalArgumentException"> if {@code components} is
		'''         {@code null}, or contains {@code null}; or {@code axis}
		'''          is not {@code SwingConstants.HORIZONTAL} or
		'''          {@code SwingConstants.VERTICAL} </exception>
		Public Overridable Sub linkSize(ByVal axis As Integer, ParamArray ByVal components As java.awt.Component())
			If components Is Nothing Then Throw New System.ArgumentException("Components must be non-null")
			For counter As Integer = components.Length - 1 To 0 Step -1
				Dim c As java.awt.Component = components(counter)
				If components(counter) Is Nothing Then Throw New System.ArgumentException("Components must be non-null")
				' Force the component to be added
				getComponentInfo(c)
			Next counter
			Dim glAxis As Integer
			If axis = SwingConstants.HORIZONTAL Then
				glAxis = HORIZONTAL
			ElseIf axis = SwingConstants.VERTICAL Then
				glAxis = VERTICAL
			Else
				Throw New System.ArgumentException("Axis must be one of " & "SwingConstants.HORIZONTAL or SwingConstants.VERTICAL")
			End If
			Dim master As LinkInfo = getComponentInfo(components(components.Length - 1)).getLinkInfo(glAxis)
			For counter As Integer = components.Length - 2 To 0 Step -1
				master.add(getComponentInfo(components(counter)))
			Next counter
			invalidateHost()
		End Sub

		''' <summary>
		''' Replaces an existing component with a new one.
		''' </summary>
		''' <param name="existingComponent"> the component that should be removed
		'''        and replaced with {@code newComponent} </param>
		''' <param name="newComponent"> the component to put in
		'''        {@code existingComponent}'s place </param>
		''' <exception cref="IllegalArgumentException"> if either of the components are
		'''         {@code null} or {@code existingComponent} is not being managed
		'''         by this layout manager </exception>
		Public Overridable Sub replace(ByVal existingComponent As java.awt.Component, ByVal newComponent As java.awt.Component)
			If existingComponent Is Nothing OrElse newComponent Is Nothing Then Throw New System.ArgumentException("Components must be non-null")
			' Make sure all the components have been registered, otherwise we may
			' not update the correct Springs.
			If springsChanged Then
				registerComponents(horizontalGroup, HORIZONTAL)
				registerComponents(verticalGroup, VERTICAL)
			End If
			Dim info As ComponentInfo = componentInfos.Remove(existingComponent)
			If info Is Nothing Then Throw New System.ArgumentException("Component must already exist")
			host.remove(existingComponent)
			If newComponent.parent IsNot host Then host.add(newComponent)
			info.component = newComponent
			componentInfos(newComponent) = info
			invalidateHost()
		End Sub

		''' <summary>
		''' Sets the {@code LayoutStyle} used to calculate the preferred
		''' gaps between components. A value of {@code null} indicates the
		''' shared instance of {@code LayoutStyle} should be used.
		''' </summary>
		''' <param name="layoutStyle"> the {@code LayoutStyle} to use </param>
		''' <seealso cref= LayoutStyle </seealso>
		Public Overridable Property layoutStyle As LayoutStyle
			Set(ByVal ___layoutStyle As LayoutStyle)
				Me.layoutStyle = ___layoutStyle
				invalidateHost()
			End Set
			Get
				Return layoutStyle
			End Get
		End Property


		Private Property layoutStyle0 As LayoutStyle
			Get
				Dim ___layoutStyle As LayoutStyle = layoutStyle
				If ___layoutStyle Is Nothing Then ___layoutStyle = LayoutStyle.instance
				Return ___layoutStyle
			End Get
		End Property

		Private Sub invalidateHost()
			If TypeOf host Is JComponent Then
				CType(host, JComponent).revalidate()
			Else
				host.invalidate()
			End If
			host.repaint()
		End Sub

		'
		' LayoutManager
		'
		''' <summary>
		''' Notification that a {@code Component} has been added to
		''' the parent container.  You should not invoke this method
		''' directly, instead you should use one of the {@code Group}
		''' methods to add a {@code Component}.
		''' </summary>
		''' <param name="name"> the string to be associated with the component </param>
		''' <param name="component"> the {@code Component} to be added </param>
		Public Overridable Sub addLayoutComponent(ByVal name As String, ByVal component As java.awt.Component)
		End Sub

		''' <summary>
		''' Notification that a {@code Component} has been removed from
		''' the parent container.  You should not invoke this method
		''' directly, instead invoke {@code remove} on the parent
		''' {@code Container}.
		''' </summary>
		''' <param name="component"> the component to be removed </param>
		''' <seealso cref= java.awt.Component#remove </seealso>
		Public Overridable Sub removeLayoutComponent(ByVal component As java.awt.Component)
			Dim info As ComponentInfo = componentInfos.Remove(component)
			If info IsNot Nothing Then
				info.Dispose()
				springsChanged = True
				isValid = False
			End If
		End Sub

		''' <summary>
		''' Returns the preferred size for the specified container.
		''' </summary>
		''' <param name="parent"> the container to return the preferred size for </param>
		''' <returns> the preferred size for {@code parent} </returns>
		''' <exception cref="IllegalArgumentException"> if {@code parent} is not
		'''         the same {@code Container} this was created with </exception>
		''' <exception cref="IllegalStateException"> if any of the components added to
		'''         this layout are not in both a horizontal and vertical group </exception>
		''' <seealso cref= java.awt.Container#getPreferredSize </seealso>
		Public Overridable Function preferredLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			checkParent(parent)
			prepare(PREF_SIZE)
			Return adjustSize(horizontalGroup.getPreferredSize(HORIZONTAL), verticalGroup.getPreferredSize(VERTICAL))
		End Function

		''' <summary>
		''' Returns the minimum size for the specified container.
		''' </summary>
		''' <param name="parent"> the container to return the size for </param>
		''' <returns> the minimum size for {@code parent} </returns>
		''' <exception cref="IllegalArgumentException"> if {@code parent} is not
		'''         the same {@code Container} that this was created with </exception>
		''' <exception cref="IllegalStateException"> if any of the components added to
		'''         this layout are not in both a horizontal and vertical group </exception>
		''' <seealso cref= java.awt.Container#getMinimumSize </seealso>
		Public Overridable Function minimumLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			checkParent(parent)
			prepare(MIN_SIZE)
			Return adjustSize(horizontalGroup.getMinimumSize(HORIZONTAL), verticalGroup.getMinimumSize(VERTICAL))
		End Function

		''' <summary>
		''' Lays out the specified container.
		''' </summary>
		''' <param name="parent"> the container to be laid out </param>
		''' <exception cref="IllegalStateException"> if any of the components added to
		'''         this layout are not in both a horizontal and vertical group </exception>
		Public Overridable Sub layoutContainer(ByVal parent As java.awt.Container)
			' Step 1: Prepare for layout.
			prepare(SPECIFIC_SIZE)
			Dim insets As java.awt.Insets = parent.insets
			Dim width As Integer = parent.width - insets.left - insets.right
			Dim height As Integer = parent.height - insets.top - insets.bottom
			Dim ltr As Boolean = leftToRight
			If autoCreateGaps OrElse autoCreateContainerGaps OrElse hasPreferredPaddingSprings Then
				' Step 2: Calculate autopadding springs
				calculateAutopadding(horizontalGroup, HORIZONTAL, SPECIFIC_SIZE, 0, width)
				calculateAutopadding(verticalGroup, VERTICAL, SPECIFIC_SIZE, 0, height)
			End If
			' Step 3: set the size of the groups.
			horizontalGroup.sizeize(HORIZONTAL, 0, width)
			verticalGroup.sizeize(VERTICAL, 0, height)
			' Step 4: apply the size to the components.
			For Each info As ComponentInfo In componentInfos.Values
				info.boundsnds(insets, width, ltr)
			Next info
		End Sub

		'
		' LayoutManager2
		'
		''' <summary>
		''' Notification that a {@code Component} has been added to
		''' the parent container.  You should not invoke this method
		''' directly, instead you should use one of the {@code Group}
		''' methods to add a {@code Component}.
		''' </summary>
		''' <param name="component"> the component added </param>
		''' <param name="constraints"> description of where to place the component </param>
		Public Overridable Sub addLayoutComponent(ByVal component As java.awt.Component, ByVal constraints As Object)
		End Sub

		''' <summary>
		''' Returns the maximum size for the specified container.
		''' </summary>
		''' <param name="parent"> the container to return the size for </param>
		''' <returns> the maximum size for {@code parent} </returns>
		''' <exception cref="IllegalArgumentException"> if {@code parent} is not
		'''         the same {@code Container} that this was created with </exception>
		''' <exception cref="IllegalStateException"> if any of the components added to
		'''         this layout are not in both a horizontal and vertical group </exception>
		''' <seealso cref= java.awt.Container#getMaximumSize </seealso>
		Public Overridable Function maximumLayoutSize(ByVal parent As java.awt.Container) As java.awt.Dimension
			checkParent(parent)
			prepare(MAX_SIZE)
			Return adjustSize(horizontalGroup.getMaximumSize(HORIZONTAL), verticalGroup.getMaximumSize(VERTICAL))
		End Function

		''' <summary>
		''' Returns the alignment along the x axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		''' <param name="parent"> the {@code Container} hosting this {@code LayoutManager} </param>
		''' <exception cref="IllegalArgumentException"> if {@code parent} is not
		'''         the same {@code Container} that this was created with </exception>
		''' <returns> the alignment; this implementation returns {@code .5} </returns>
		Public Overridable Function getLayoutAlignmentX(ByVal parent As java.awt.Container) As Single
			checkParent(parent)
			Return.5f
		End Function

		''' <summary>
		''' Returns the alignment along the y axis.  This specifies how
		''' the component would like to be aligned relative to other
		''' components.  The value should be a number between 0 and 1
		''' where 0 represents alignment along the origin, 1 is aligned
		''' the furthest away from the origin, 0.5 is centered, etc.
		''' </summary>
		''' <param name="parent"> the {@code Container} hosting this {@code LayoutManager} </param>
		''' <exception cref="IllegalArgumentException"> if {@code parent} is not
		'''         the same {@code Container} that this was created with </exception>
		''' <returns> alignment; this implementation returns {@code .5} </returns>
		Public Overridable Function getLayoutAlignmentY(ByVal parent As java.awt.Container) As Single
			checkParent(parent)
			Return.5f
		End Function

		''' <summary>
		''' Invalidates the layout, indicating that if the layout manager
		''' has cached information it should be discarded.
		''' </summary>
		''' <param name="parent"> the {@code Container} hosting this LayoutManager </param>
		''' <exception cref="IllegalArgumentException"> if {@code parent} is not
		'''         the same {@code Container} that this was created with </exception>
		Public Overridable Sub invalidateLayout(ByVal parent As java.awt.Container)
			checkParent(parent)
			' invalidateLayout is called from Container.invalidate, which
			' does NOT grab the treelock.  All other methods do.  To make sure
			' there aren't any possible threading problems we grab the tree lock
			' here.
			SyncLock parent.treeLock
				isValid = False
			End SyncLock
		End Sub

		Private Sub prepare(ByVal sizeType As Integer)
			Dim visChanged As Boolean = False
			' Step 1: If not-valid, clear springs and update visibility.
			If Not isValid Then
				isValid = True
				horizontalGroup.sizeize(HORIZONTAL, UNSET, UNSET)
				verticalGroup.sizeize(VERTICAL, UNSET, UNSET)
				For Each ci As ComponentInfo In componentInfos.Values
					If ci.updateVisibility() Then visChanged = True
					ci.clearCachedSize()
				Next ci
			End If
			' Step 2: Make sure components are bound to ComponentInfos
			If springsChanged Then
				registerComponents(horizontalGroup, HORIZONTAL)
				registerComponents(verticalGroup, VERTICAL)
			End If
			' Step 3: Adjust the autopadding. This removes existing
			' autopadding, then recalculates where it should go.
			If springsChanged OrElse visChanged Then
				checkComponents()
				horizontalGroup.removeAutopadding()
				verticalGroup.removeAutopadding()
				If autoCreateGaps Then
					insertAutopadding(True)
				ElseIf hasPreferredPaddingSprings OrElse autoCreateContainerGaps Then
					insertAutopadding(False)
				End If
				springsChanged = False
			End If
			' Step 4: (for min/pref/max size calculations only) calculate the
			' autopadding. This invokes for unsetting the calculated values, then
			' recalculating them.
			' If sizeType == SPECIFIC_SIZE, it indicates we're doing layout, this
			' step will be done later on.
			If sizeType <> SPECIFIC_SIZE AndAlso (autoCreateGaps OrElse autoCreateContainerGaps OrElse hasPreferredPaddingSprings) Then
				calculateAutopadding(horizontalGroup, HORIZONTAL, sizeType, 0, 0)
				calculateAutopadding(verticalGroup, VERTICAL, sizeType, 0, 0)
			End If
		End Sub

		Private Sub calculateAutopadding(ByVal group As Group, ByVal axis As Integer, ByVal sizeType As Integer, ByVal origin As Integer, ByVal size As Integer)
			group.unsetAutopadding()
			Select Case sizeType
				Case MIN_SIZE
					size = group.getMinimumSize(axis)
				Case PREF_SIZE
					size = group.getPreferredSize(axis)
				Case MAX_SIZE
					size = group.getMaximumSize(axis)
				Case Else
			End Select
			group.sizeize(axis, origin, size)
			group.calculateAutopadding(axis)
		End Sub

		Private Sub checkComponents()
			For Each info As ComponentInfo In componentInfos.Values
				If info.horizontalSpring Is Nothing Then Throw New IllegalStateException(info.component & " is not attached to a horizontal group")
				If info.verticalSpring Is Nothing Then Throw New IllegalStateException(info.component & " is not attached to a vertical group")
			Next info
		End Sub

		Private Sub registerComponents(ByVal group As Group, ByVal axis As Integer)
			Dim springs As IList(Of Spring) = group.springs
			For counter As Integer = springs.Count - 1 To 0 Step -1
				Dim spring As Spring = springs(counter)
				If TypeOf spring Is ComponentSpring Then
					CType(spring, ComponentSpring).installIfNecessary(axis)
				ElseIf TypeOf spring Is Group Then
					registerComponents(CType(spring, Group), axis)
				End If
			Next counter
		End Sub

		Private Function adjustSize(ByVal width As Integer, ByVal height As Integer) As java.awt.Dimension
			Dim insets As java.awt.Insets = host.insets
			Return New java.awt.Dimension(width + insets.left + insets.right, height + insets.top + insets.bottom)
		End Function

		Private Sub checkParent(ByVal parent As java.awt.Container)
			If parent IsNot host Then Throw New System.ArgumentException("GroupLayout can only be used with one Container at a time")
		End Sub

		''' <summary>
		''' Returns the {@code ComponentInfo} for the specified Component,
		''' creating one if necessary.
		''' </summary>
		Private Function getComponentInfo(ByVal component As java.awt.Component) As ComponentInfo
			Dim info As ComponentInfo = componentInfos(component)
			If info Is Nothing Then
				info = New ComponentInfo(Me, component)
				componentInfos(component) = info
				If component.parent IsNot host Then host.add(component)
			End If
			Return info
		End Function

		''' <summary>
		''' Adjusts the autopadding springs for the horizontal and vertical
		''' groups.  If {@code insert} is {@code true} this will insert auto padding
		''' springs, otherwise this will only adjust the springs that
		''' comprise auto preferred padding springs.
		''' </summary>
		Private Sub insertAutopadding(ByVal insert As Boolean)
			horizontalGroup.insertAutopadding(HORIZONTAL, New List(Of AutoPreferredGapSpring)(1), New List(Of AutoPreferredGapSpring)(1), New List(Of ComponentSpring)(1), New List(Of ComponentSpring)(1), insert)
			verticalGroup.insertAutopadding(VERTICAL, New List(Of AutoPreferredGapSpring)(1), New List(Of AutoPreferredGapSpring)(1), New List(Of ComponentSpring)(1), New List(Of ComponentSpring)(1), insert)
		End Sub

		''' <summary>
		''' Returns {@code true} if the two Components have a common ParallelGroup
		''' ancestor along the particular axis.
		''' </summary>
		Private Function areParallelSiblings(ByVal source As java.awt.Component, ByVal target As java.awt.Component, ByVal axis As Integer) As Boolean
			Dim sourceInfo As ComponentInfo = getComponentInfo(source)
			Dim targetInfo As ComponentInfo = getComponentInfo(target)
			Dim sourceSpring As Spring
			Dim targetSpring As Spring
			If axis = HORIZONTAL Then
				sourceSpring = sourceInfo.horizontalSpring
				targetSpring = targetInfo.horizontalSpring
			Else
				sourceSpring = sourceInfo.verticalSpring
				targetSpring = targetInfo.verticalSpring
			End If
			Dim sourcePath As [Set](Of Spring) = tmpParallelSet
			sourcePath.clear()
			Dim spring As Spring = sourceSpring.parent
			Do While spring IsNot Nothing
				sourcePath.add(spring)
				spring = spring.parent
			Loop
			spring = targetSpring.parent
			Do While spring IsNot Nothing
				If sourcePath.contains(spring) Then
					sourcePath.clear()
					Do While spring IsNot Nothing
						If TypeOf spring Is ParallelGroup Then Return True
						spring = spring.parent
					Loop
					Return False
				End If
				spring = spring.parent
			Loop
			sourcePath.clear()
			Return False
		End Function

		Private Property leftToRight As Boolean
			Get
				Return host.componentOrientation.leftToRight
			End Get
		End Property

		''' <summary>
		''' Returns a string representation of this {@code GroupLayout}.
		''' This method is intended to be used for debugging purposes,
		''' and the content and format of the returned string may vary
		''' between implementations.
		''' </summary>
		''' <returns> a string representation of this {@code GroupLayout}
		'''  </returns>
		Public Overrides Function ToString() As String
			If springsChanged Then
				registerComponents(horizontalGroup, HORIZONTAL)
				registerComponents(verticalGroup, VERTICAL)
			End If
			Dim buffer As New StringBuilder
			buffer.Append("HORIZONTAL" & vbLf)
			createSpringDescription(buffer, horizontalGroup, "  ", HORIZONTAL)
			buffer.Append(vbLf & "VERTICAL" & vbLf)
			createSpringDescription(buffer, verticalGroup, "  ", VERTICAL)
			Return buffer.ToString()
		End Function

		Private Sub createSpringDescription(ByVal buffer As StringBuilder, ByVal spring As Spring, ByVal indent As String, ByVal axis As Integer)
			Dim origin As String = ""
			Dim padding As String = ""
			If TypeOf spring Is ComponentSpring Then
				Dim cSpring As ComponentSpring = CType(spring, ComponentSpring)
				origin = Convert.ToString(cSpring.origin) & " "
				Dim name As String = cSpring.component.name
				If name IsNot Nothing Then origin = "name=" & name & ", "
			End If
			If TypeOf spring Is AutoPreferredGapSpring Then
				Dim paddingSpring As AutoPreferredGapSpring = CType(spring, AutoPreferredGapSpring)
				padding = ", userCreated=" & paddingSpring.userCreated & ", matches=" & paddingSpring.matchDescription
			End If
			buffer.Append(indent + spring.GetType().name & " " & Integer.toHexString(spring.GetHashCode()) & " " & origin & ", size=" & spring.size & ", alignment=" & spring.alignment & " prefs=[" & spring.getMinimumSize(axis) & " " & spring.getPreferredSize(axis) & " " & spring.getMaximumSize(axis) + padding & "]" & vbLf)
			If TypeOf spring Is Group Then
				Dim springs As IList(Of Spring) = CType(spring, Group).springs
				indent &= "  "
				For counter As Integer = 0 To springs.Count - 1
					createSpringDescription(buffer, springs(counter), indent, axis)
				Next counter
			End If
		End Sub


		''' <summary>
		''' Spring consists of a range: min, pref and max, a value some where in
		''' the middle of that, and a location. Spring caches the
		''' min/max/pref.  If the min/pref/max has internally changes, or needs
		''' to be updated you must invoke clear.
		''' </summary>
		Private MustInherit Class Spring
			Private ReadOnly outerInstance As GroupLayout

			Private size As Integer
			Private min As Integer
			Private max As Integer
			Private pref As Integer
			Private parent As Spring

			Private alignment As Alignment

			Friend Sub New(ByVal outerInstance As GroupLayout)
					Me.outerInstance = outerInstance
					max = UNSET
						pref = max
						min = pref
			End Sub

			''' <summary>
			''' Calculates and returns the minimum size.
			''' </summary>
			''' <param name="axis"> the axis of layout; one of HORIZONTAL or VERTICAL </param>
			''' <returns> the minimum size </returns>
			Friend MustOverride Function calculateMinimumSize(ByVal axis As Integer) As Integer

			''' <summary>
			''' Calculates and returns the preferred size.
			''' </summary>
			''' <param name="axis"> the axis of layout; one of HORIZONTAL or VERTICAL </param>
			''' <returns> the preferred size </returns>
			Friend MustOverride Function calculatePreferredSize(ByVal axis As Integer) As Integer

			''' <summary>
			''' Calculates and returns the minimum size.
			''' </summary>
			''' <param name="axis"> the axis of layout; one of HORIZONTAL or VERTICAL </param>
			''' <returns> the minimum size </returns>
			Friend MustOverride Function calculateMaximumSize(ByVal axis As Integer) As Integer

			''' <summary>
			''' Sets the parent of this Spring.
			''' </summary>
			Friend Overridable Property parent As Spring
				Set(ByVal parent As Spring)
					Me.parent = parent
				End Set
				Get
					Return parent
				End Get
			End Property


			' This is here purely as a convenience for ParallelGroup to avoid
			' having to track alignment separately.
			Friend Overridable Property alignment As Alignment
				Set(ByVal alignment As Alignment)
					Me.alignment = alignment
				End Set
				Get
					Return alignment
				End Get
			End Property


			''' <summary>
			''' Returns the minimum size.
			''' </summary>
			Friend Function getMinimumSize(ByVal axis As Integer) As Integer
				If min = UNSET Then min = constrain(calculateMinimumSize(axis))
				Return min
			End Function

			''' <summary>
			''' Returns the preferred size.
			''' </summary>
			Friend Function getPreferredSize(ByVal axis As Integer) As Integer
				If pref = UNSET Then pref = constrain(calculatePreferredSize(axis))
				Return pref
			End Function

			''' <summary>
			''' Returns the maximum size.
			''' </summary>
			Friend Function getMaximumSize(ByVal axis As Integer) As Integer
				If max = UNSET Then max = constrain(calculateMaximumSize(axis))
				Return max
			End Function

			''' <summary>
			''' Sets the value and location of the spring.  Subclasses
			''' will want to invoke super, then do any additional sizing.
			''' </summary>
			''' <param name="axis"> HORIZONTAL or VERTICAL </param>
			''' <param name="origin"> of this Spring </param>
			''' <param name="size"> of the Spring.  If size is UNSET, this invokes
			'''        clear. </param>
			Friend Overridable Sub setSize(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				Me.size = size
				If size = UNSET Then unset()
			End Sub

			''' <summary>
			''' Resets the cached min/max/pref.
			''' </summary>
			Friend Overridable Sub unset()
					max = UNSET
						pref = max
							min = pref
							size = min
			End Sub

			''' <summary>
			''' Returns the current size.
			''' </summary>
			Friend Overridable Property size As Integer
				Get
					Return size
				End Get
			End Property

			Friend Overridable Function constrain(ByVal value As Integer) As Integer
				Return Math.Min(value, Short.MaxValue)
			End Function

			Friend Overridable Property baseline As Integer
				Get
					Return -1
				End Get
			End Property

			Friend Overridable Property baselineResizeBehavior As BaselineResizeBehavior
				Get
					Return BaselineResizeBehavior.OTHER
				End Get
			End Property

			Friend Function isResizable(ByVal axis As Integer) As Boolean
				Dim min As Integer = getMinimumSize(axis)
				Dim pref As Integer = getPreferredSize(axis)
				Return (min <> pref OrElse pref <> getMaximumSize(axis))
			End Function

			''' <summary>
			''' Returns {@code true} if this spring will ALWAYS have a zero
			''' size. This should NOT check the current size, rather it's
			''' meant to quickly test if this Spring will always have a
			''' zero size.
			''' </summary>
			''' <param name="treatAutopaddingAsZeroSized"> if {@code true}, auto padding
			'''        springs should be treated as having a size of {@code 0} </param>
			''' <returns> {@code true} if this spring will have a zero size,
			'''         {@code false} otherwise </returns>
			Friend MustOverride Function willHaveZeroSize(ByVal treatAutopaddingAsZeroSized As Boolean) As Boolean
		End Class

		''' <summary>
		''' {@code Group} provides the basis for the two types of
		''' operations supported by {@code GroupLayout}: laying out
		''' components one after another (<seealso cref="SequentialGroup SequentialGroup"/>)
		''' or aligned (<seealso cref="ParallelGroup ParallelGroup"/>). {@code Group} and
		''' its subclasses have no public constructor; to create one use
		''' one of {@code createSequentialGroup} or
		''' {@code createParallelGroup}. Additionally, taking a {@code Group}
		''' created from one {@code GroupLayout} and using it with another
		''' will produce undefined results.
		''' <p>
		''' Various methods in {@code Group} and its subclasses allow you
		''' to explicitly specify the range. The arguments to these methods
		''' can take two forms, either a value greater than or equal to 0,
		''' or one of {@code DEFAULT_SIZE} or {@code PREFERRED_SIZE}. A
		''' value greater than or equal to {@code 0} indicates a specific
		''' size. {@code DEFAULT_SIZE} indicates the corresponding size
		''' from the component should be used.  For example, if {@code
		''' DEFAULT_SIZE} is passed as the minimum size argument, the
		''' minimum size is obtained from invoking {@code getMinimumSize}
		''' on the component. Likewise, {@code PREFERRED_SIZE} indicates
		''' the value from {@code getPreferredSize} should be used.
		''' The following example adds {@code myComponent} to {@code group}
		''' with specific values for the range. That is, the minimum is
		''' explicitly specified as 100, preferred as 200, and maximum as
		''' 300.
		''' <pre>
		'''   group.addComponent(myComponent, 100, 200, 300);
		''' </pre>
		''' The following example adds {@code myComponent} to {@code group} using
		''' a combination of the forms. The minimum size is forced to be the
		''' same as the preferred size, the preferred size is determined by
		''' using {@code myComponent.getPreferredSize} and the maximum is
		''' determined by invoking {@code getMaximumSize} on the component.
		''' <pre>
		'''   group.addComponent(myComponent, GroupLayout.PREFERRED_SIZE,
		'''             GroupLayout.PREFERRED_SIZE, GroupLayout.DEFAULT_SIZE);
		''' </pre>
		''' <p>
		''' Unless otherwise specified all the methods of {@code Group} and
		''' its subclasses that allow you to specify a range throw an
		''' {@code IllegalArgumentException} if passed an invalid range. An
		''' invalid range is one in which any of the values are &lt; 0 and
		''' not one of {@code PREFERRED_SIZE} or {@code DEFAULT_SIZE}, or
		''' the following is not met (for specific values): {@code min}
		''' &lt;= {@code pref} &lt;= {@code max}.
		''' <p>
		''' Similarly any methods that take a {@code Component} throw a
		''' {@code IllegalArgumentException} if passed {@code null} and any methods
		''' that take a {@code Group} throw an {@code NullPointerException} if
		''' passed {@code null}.
		''' </summary>
		''' <seealso cref= #createSequentialGroup </seealso>
		''' <seealso cref= #createParallelGroup
		''' @since 1.6 </seealso>
		Public MustInherit Class Group
			Inherits Spring

			Private ReadOnly outerInstance As GroupLayout

			' private int origin;
			' private int size;
			Friend springs As IList(Of Spring)

			Friend Sub New(ByVal outerInstance As GroupLayout)
					Me.outerInstance = outerInstance
				springs = New List(Of Spring)
			End Sub

			''' <summary>
			''' Adds a {@code Group} to this {@code Group}.
			''' </summary>
			''' <param name="group"> the {@code Group} to add </param>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addGroup(ByVal group As Group) As Group
				Return addSpring(group)
			End Function

			''' <summary>
			''' Adds a {@code Component} to this {@code Group}.
			''' </summary>
			''' <param name="component"> the {@code Component} to add </param>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addComponent(ByVal component As java.awt.Component) As Group
				Return addComponent(component, DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE)
			End Function

			''' <summary>
			''' Adds a {@code Component} to this {@code Group}
			''' with the specified size.
			''' </summary>
			''' <param name="component"> the {@code Component} to add </param>
			''' <param name="min"> the minimum size or one of {@code DEFAULT_SIZE} or
			'''            {@code PREFERRED_SIZE} </param>
			''' <param name="pref"> the preferred size or one of {@code DEFAULT_SIZE} or
			'''            {@code PREFERRED_SIZE} </param>
			''' <param name="max"> the maximum size or one of {@code DEFAULT_SIZE} or
			'''            {@code PREFERRED_SIZE} </param>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addComponent(ByVal component As java.awt.Component, ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As Group
				Return addSpring(New ComponentSpring(component, min, pref, max))
			End Function

			''' <summary>
			''' Adds a rigid gap to this {@code Group}.
			''' </summary>
			''' <param name="size"> the size of the gap </param>
			''' <returns> this {@code Group} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code size} is less than
			'''         {@code 0} </exception>
			Public Overridable Function addGap(ByVal size As Integer) As Group
				Return addGap(size, size, size)
			End Function

			''' <summary>
			''' Adds a gap to this {@code Group} with the specified size.
			''' </summary>
			''' <param name="min"> the minimum size of the gap </param>
			''' <param name="pref"> the preferred size of the gap </param>
			''' <param name="max"> the maximum size of the gap </param>
			''' <exception cref="IllegalArgumentException"> if any of the values are
			'''         less than {@code 0} </exception>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addGap(ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As Group
				Return addSpring(New GapSpring(min, pref, max))
			End Function

			Friend Overridable Function getSpring(ByVal index As Integer) As Spring
				Return springs(index)
			End Function

			Friend Overridable Function indexOf(ByVal spring As Spring) As Integer
				Return springs.IndexOf(spring)
			End Function

			''' <summary>
			''' Adds the Spring to the list of {@code Spring}s and returns
			''' the receiver.
			''' </summary>
			Friend Overridable Function addSpring(ByVal spring As Spring) As Group
				springs.Add(spring)
				spring.parent = Me
				If Not(TypeOf spring Is AutoPreferredGapSpring) OrElse (Not CType(spring, AutoPreferredGapSpring).userCreated) Then outerInstance.springsChanged = True
				Return Me
			End Function

			'
			' Spring methods
			'

			Friend Overrides Sub setSize(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				MyBase.sizeize(axis, origin, size)
				If size = UNSET Then
					For counter As Integer = springs.Count - 1 To 0 Step -1
						getSpring(counter).sizeize(axis, origin, size)
					Next counter
				Else
					validSizeize(axis, origin, size)
				End If
			End Sub

			''' <summary>
			''' This is invoked from {@code setSize} if passed a value
			''' other than UNSET.
			''' </summary>
			Friend MustOverride Sub setValidSize(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)

			Friend Overrides Function calculateMinimumSize(ByVal axis As Integer) As Integer
				Return calculateSize(axis, MIN_SIZE)
			End Function

			Friend Overrides Function calculatePreferredSize(ByVal axis As Integer) As Integer
				Return calculateSize(axis, PREF_SIZE)
			End Function

			Friend Overrides Function calculateMaximumSize(ByVal axis As Integer) As Integer
				Return calculateSize(axis, MAX_SIZE)
			End Function

			''' <summary>
			''' Calculates the specified size.  This is called from
			''' one of the {@code getMinimumSize0},
			''' {@code getPreferredSize0} or
			''' {@code getMaximumSize0} methods.  This will invoke
			''' to {@code operator} to combine the values.
			''' </summary>
			Friend Overridable Function calculateSize(ByVal axis As Integer, ByVal type As Integer) As Integer
				Dim count As Integer = springs.Count
				If count = 0 Then Return 0
				If count = 1 Then Return getSpringSize(getSpring(0), axis, type)
				Dim ___size As Integer = constrain([operator](getSpringSize(getSpring(0), axis, type), getSpringSize(getSpring(1), axis, type)))
				For counter As Integer = 2 To count - 1
					___size = constrain([operator](___size, getSpringSize(getSpring(counter), axis, type)))
				Next counter
				Return ___size
			End Function

			Friend Overridable Function getSpringSize(ByVal spring As Spring, ByVal axis As Integer, ByVal type As Integer) As Integer
				Select Case type
					Case MIN_SIZE
						Return spring.getMinimumSize(axis)
					Case PREF_SIZE
						Return spring.getPreferredSize(axis)
					Case MAX_SIZE
						Return spring.getMaximumSize(axis)
				End Select
				Debug.Assert(False)
				Return 0
			End Function

			''' <summary>
			''' Used to compute how the two values representing two springs
			''' will be combined.  For example, a group that layed things out
			''' one after the next would return {@code a + b}.
			''' </summary>
			Friend MustOverride Function [operator](ByVal a As Integer, ByVal b As Integer) As Integer

			'
			' Padding
			'

			''' <summary>
			''' Adjusts the autopadding springs in this group and its children.
			''' If {@code insert} is true this will insert auto padding
			''' springs, otherwise this will only adjust the springs that
			''' comprise auto preferred padding springs.
			''' </summary>
			''' <param name="axis"> the axis of the springs; HORIZONTAL or VERTICAL </param>
			''' <param name="leadingPadding"> List of AutopaddingSprings that occur before
			'''                       this Group </param>
			''' <param name="trailingPadding"> any trailing autopadding springs are added
			'''                        to this on exit </param>
			''' <param name="leading"> List of ComponentSprings that occur before this Group </param>
			''' <param name="trailing"> any trailing ComponentSpring are added to this
			'''                 List </param>
			''' <param name="insert"> Whether or not to insert AutopaddingSprings or just
			'''               adjust any existing AutopaddingSprings. </param>
			Friend MustOverride Sub insertAutopadding(ByVal axis As Integer, ByVal leadingPadding As IList(Of AutoPreferredGapSpring), ByVal trailingPadding As IList(Of AutoPreferredGapSpring), ByVal leading As IList(Of ComponentSpring), ByVal trailing As IList(Of ComponentSpring), ByVal insert As Boolean)

			''' <summary>
			''' Removes any AutopaddingSprings for this Group and its children.
			''' </summary>
			Friend Overridable Sub removeAutopadding()
				unset()
				For counter As Integer = springs.Count - 1 To 0 Step -1
					Dim ___spring As Spring = springs(counter)
					If TypeOf ___spring Is AutoPreferredGapSpring Then
						If CType(___spring, AutoPreferredGapSpring).userCreated Then
							CType(___spring, AutoPreferredGapSpring).reset()
						Else
							springs.RemoveAt(counter)
						End If
					ElseIf TypeOf ___spring Is Group Then
						CType(___spring, Group).removeAutopadding()
					End If
				Next counter
			End Sub

			Friend Overridable Sub unsetAutopadding()
				' Clear cached pref/min/max.
				unset()
				For counter As Integer = springs.Count - 1 To 0 Step -1
					Dim ___spring As Spring = springs(counter)
					If TypeOf ___spring Is AutoPreferredGapSpring Then
						___spring.unset()
					ElseIf TypeOf ___spring Is Group Then
						CType(___spring, Group).unsetAutopadding()
					End If
				Next counter
			End Sub

			Friend Overridable Sub calculateAutopadding(ByVal axis As Integer)
				For counter As Integer = springs.Count - 1 To 0 Step -1
					Dim ___spring As Spring = springs(counter)
					If TypeOf ___spring Is AutoPreferredGapSpring Then
						' Force size to be reset.
						___spring.unset()
						CType(___spring, AutoPreferredGapSpring).calculatePadding(axis)
					ElseIf TypeOf ___spring Is Group Then
						CType(___spring, Group).calculateAutopadding(axis)
					End If
				Next counter
				' Clear cached pref/min/max.
				unset()
			End Sub

			Friend Overrides Function willHaveZeroSize(ByVal treatAutopaddingAsZeroSized As Boolean) As Boolean
				For i As Integer = springs.Count - 1 To 0 Step -1
					Dim ___spring As Spring = springs(i)
					If Not ___spring.willHaveZeroSize(treatAutopaddingAsZeroSized) Then Return False
				Next i
				Return True
			End Function
		End Class


		''' <summary>
		''' A {@code Group} that positions and sizes its elements
		''' sequentially, one after another.  This class has no public
		''' constructor, use the {@code createSequentialGroup} method
		''' to create one.
		''' <p>
		''' In order to align a {@code SequentialGroup} along the baseline
		''' of a baseline aligned {@code ParallelGroup} you need to specify
		''' which of the elements of the {@code SequentialGroup} is used to
		''' determine the baseline.  The element used to calculate the
		''' baseline is specified using one of the {@code add} methods that
		''' take a {@code boolean}. The last element added with a value of
		''' {@code true} for {@code useAsBaseline} is used to calculate the
		''' baseline.
		''' </summary>
		''' <seealso cref= #createSequentialGroup
		''' @since 1.6 </seealso>
		Public Class SequentialGroup
			Inherits Group

			Private ReadOnly outerInstance As GroupLayout

			Private baselineSpring As Spring

			Friend Sub New(ByVal outerInstance As GroupLayout)
					Me.outerInstance = outerInstance
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addGroup(ByVal group As Group) As SequentialGroup
				Return CType(MyBase.addGroup(group), SequentialGroup)
			End Function

			''' <summary>
			''' Adds a {@code Group} to this {@code Group}.
			''' </summary>
			''' <param name="group"> the {@code Group} to add </param>
			''' <param name="useAsBaseline"> whether the specified {@code Group} should
			'''        be used to calculate the baseline for this {@code Group} </param>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addGroup(ByVal useAsBaseline As Boolean, ByVal group As Group) As SequentialGroup
				MyBase.addGroup(group)
				If useAsBaseline Then baselineSpring = group
				Return Me
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addComponent(ByVal component As java.awt.Component) As SequentialGroup
				Return CType(MyBase.addComponent(component), SequentialGroup)
			End Function

			''' <summary>
			''' Adds a {@code Component} to this {@code Group}.
			''' </summary>
			''' <param name="useAsBaseline"> whether the specified {@code Component} should
			'''        be used to calculate the baseline for this {@code Group} </param>
			''' <param name="component"> the {@code Component} to add </param>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addComponent(ByVal useAsBaseline As Boolean, ByVal component As java.awt.Component) As SequentialGroup
				MyBase.addComponent(component)
				If useAsBaseline Then baselineSpring = springs(springs.Count - 1)
				Return Me
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addComponent(ByVal component As java.awt.Component, ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As SequentialGroup
				Return CType(MyBase.addComponent(component, min, pref, max), SequentialGroup)
			End Function

			''' <summary>
			''' Adds a {@code Component} to this {@code Group}
			''' with the specified size.
			''' </summary>
			''' <param name="useAsBaseline"> whether the specified {@code Component} should
			'''        be used to calculate the baseline for this {@code Group} </param>
			''' <param name="component"> the {@code Component} to add </param>
			''' <param name="min"> the minimum size or one of {@code DEFAULT_SIZE} or
			'''            {@code PREFERRED_SIZE} </param>
			''' <param name="pref"> the preferred size or one of {@code DEFAULT_SIZE} or
			'''            {@code PREFERRED_SIZE} </param>
			''' <param name="max"> the maximum size or one of {@code DEFAULT_SIZE} or
			'''            {@code PREFERRED_SIZE} </param>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addComponent(ByVal useAsBaseline As Boolean, ByVal component As java.awt.Component, ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As SequentialGroup
				MyBase.addComponent(component, min, pref, max)
				If useAsBaseline Then baselineSpring = springs(springs.Count - 1)
				Return Me
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addGap(ByVal size As Integer) As SequentialGroup
				Return CType(MyBase.addGap(size), SequentialGroup)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addGap(ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As SequentialGroup
				Return CType(MyBase.addGap(min, pref, max), SequentialGroup)
			End Function

			''' <summary>
			''' Adds an element representing the preferred gap between two
			''' components. The element created to represent the gap is not
			''' resizable.
			''' </summary>
			''' <param name="comp1"> the first component </param>
			''' <param name="comp2"> the second component </param>
			''' <param name="type"> the type of gap; one of the constants defined by
			'''        {@code LayoutStyle} </param>
			''' <returns> this {@code SequentialGroup} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code type}, {@code comp1} or
			'''         {@code comp2} is {@code null} </exception>
			''' <seealso cref= LayoutStyle </seealso>
			Public Overridable Function addPreferredGap(ByVal comp1 As JComponent, ByVal comp2 As JComponent, ByVal type As ComponentPlacement) As SequentialGroup
				Return addPreferredGap(comp1, comp2, type, DEFAULT_SIZE, PREFERRED_SIZE)
			End Function

			''' <summary>
			''' Adds an element representing the preferred gap between two
			''' components.
			''' </summary>
			''' <param name="comp1"> the first component </param>
			''' <param name="comp2"> the second component </param>
			''' <param name="type"> the type of gap </param>
			''' <param name="pref"> the preferred size of the grap; one of
			'''        {@code DEFAULT_SIZE} or a value &gt;= 0 </param>
			''' <param name="max"> the maximum size of the gap; one of
			'''        {@code DEFAULT_SIZE}, {@code PREFERRED_SIZE}
			'''        or a value &gt;= 0 </param>
			''' <returns> this {@code SequentialGroup} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code type}, {@code comp1} or
			'''         {@code comp2} is {@code null} </exception>
			''' <seealso cref= LayoutStyle </seealso>
			Public Overridable Function addPreferredGap(ByVal comp1 As JComponent, ByVal comp2 As JComponent, ByVal type As ComponentPlacement, ByVal pref As Integer, ByVal max As Integer) As SequentialGroup
				If type Is Nothing Then Throw New System.ArgumentException("Type must be non-null")
				If comp1 Is Nothing OrElse comp2 Is Nothing Then Throw New System.ArgumentException("Components must be non-null")
				checkPreferredGapValues(pref, max)
				Return CType(addSpring(New PreferredGapSpring(comp1, comp2, type, pref, max)), SequentialGroup)
			End Function

			''' <summary>
			''' Adds an element representing the preferred gap between the
			''' nearest components.  During layout, neighboring
			''' components are found, and the size of the added gap is set
			''' based on the preferred gap between the components.  If no
			''' neighboring components are found the gap has a size of {@code 0}.
			''' <p>
			''' The element created to represent the gap is not
			''' resizable.
			''' </summary>
			''' <param name="type"> the type of gap; one of
			'''        {@code LayoutStyle.ComponentPlacement.RELATED} or
			'''        {@code LayoutStyle.ComponentPlacement.UNRELATED} </param>
			''' <returns> this {@code SequentialGroup} </returns>
			''' <seealso cref= LayoutStyle </seealso>
			''' <exception cref="IllegalArgumentException"> if {@code type} is not one of
			'''         {@code LayoutStyle.ComponentPlacement.RELATED} or
			'''         {@code LayoutStyle.ComponentPlacement.UNRELATED} </exception>
			Public Overridable Function addPreferredGap(ByVal type As ComponentPlacement) As SequentialGroup
				Return addPreferredGap(type, DEFAULT_SIZE, DEFAULT_SIZE)
			End Function

			''' <summary>
			''' Adds an element representing the preferred gap between the
			''' nearest components.  During layout, neighboring
			''' components are found, and the minimum of this
			''' gap is set based on the size of the preferred gap between the
			''' neighboring components.  If no neighboring components are found the
			''' minimum size is set to 0.
			''' </summary>
			''' <param name="type"> the type of gap; one of
			'''        {@code LayoutStyle.ComponentPlacement.RELATED} or
			'''        {@code LayoutStyle.ComponentPlacement.UNRELATED} </param>
			''' <param name="pref"> the preferred size of the grap; one of
			'''        {@code DEFAULT_SIZE} or a value &gt;= 0 </param>
			''' <param name="max"> the maximum size of the gap; one of
			'''        {@code DEFAULT_SIZE}, {@code PREFERRED_SIZE}
			'''        or a value &gt;= 0 </param>
			''' <returns> this {@code SequentialGroup} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code type} is not one of
			'''         {@code LayoutStyle.ComponentPlacement.RELATED} or
			'''         {@code LayoutStyle.ComponentPlacement.UNRELATED} </exception>
			''' <seealso cref= LayoutStyle </seealso>
			Public Overridable Function addPreferredGap(ByVal type As ComponentPlacement, ByVal pref As Integer, ByVal max As Integer) As SequentialGroup
				If type IsNot ComponentPlacement.RELATED AndAlso type IsNot ComponentPlacement.UNRELATED Then Throw New System.ArgumentException("Type must be one of " & "LayoutStyle.ComponentPlacement.RELATED or " & "LayoutStyle.ComponentPlacement.UNRELATED")
				checkPreferredGapValues(pref, max)
				outerInstance.hasPreferredPaddingSprings = True
				Return CType(addSpring(New AutoPreferredGapSpring(type, pref, max)), SequentialGroup)
			End Function

			''' <summary>
			''' Adds an element representing the preferred gap between an edge
			''' the container and components that touch the border of the
			''' container. This has no effect if the added gap does not
			''' touch an edge of the parent container.
			''' <p>
			''' The element created to represent the gap is not
			''' resizable.
			''' </summary>
			''' <returns> this {@code SequentialGroup} </returns>
			Public Overridable Function addContainerGap() As SequentialGroup
				Return addContainerGap(DEFAULT_SIZE, DEFAULT_SIZE)
			End Function

			''' <summary>
			''' Adds an element representing the preferred gap between one
			''' edge of the container and the next or previous {@code
			''' Component} with the specified size. This has no
			''' effect if the next or previous element is not a {@code
			''' Component} and does not touch one edge of the parent
			''' container.
			''' </summary>
			''' <param name="pref"> the preferred size; one of {@code DEFAULT_SIZE} or a
			'''              value &gt;= 0 </param>
			''' <param name="max"> the maximum size; one of {@code DEFAULT_SIZE},
			'''        {@code PREFERRED_SIZE} or a value &gt;= 0 </param>
			''' <returns> this {@code SequentialGroup} </returns>
			Public Overridable Function addContainerGap(ByVal pref As Integer, ByVal max As Integer) As SequentialGroup
				If (pref < 0 AndAlso pref <> DEFAULT_SIZE) OrElse (max < 0 AndAlso max <> DEFAULT_SIZE AndAlso max <> PREFERRED_SIZE) OrElse (pref >= 0 AndAlso max >= 0 AndAlso pref > max) Then Throw New System.ArgumentException("Pref and max must be either DEFAULT_VALUE " & "or >= 0 and pref <= max")
				outerInstance.hasPreferredPaddingSprings = True
				Return CType(addSpring(New ContainerAutoPreferredGapSpring(pref, max)), SequentialGroup)
			End Function

			Friend Overrides Function [operator](ByVal a As Integer, ByVal b As Integer) As Integer
				Return constrain(a) + constrain(b)
			End Function

			Friend Overrides Sub setValidSize(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				Dim pref As Integer = getPreferredSize(axis)
				If size = pref Then
					' Layout at preferred size
					For Each ___spring As Spring In springs
						Dim springPref As Integer = ___spring.getPreferredSize(axis)
						___spring.sizeize(axis, origin, springPref)
						origin += springPref
					Next ___spring
				ElseIf springs.Count = 1 Then
					Dim ___spring As Spring = getSpring(0)
					___spring.sizeize(axis, origin, Math.Min(Math.Max(size, ___spring.getMinimumSize(axis)), ___spring.getMaximumSize(axis)))
				ElseIf springs.Count > 1 Then
					' Adjust between min/pref
					validSizeNotPreferredred(axis, origin, size)
				End If
			End Sub

			Private Sub setValidSizeNotPreferred(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				Dim delta As Integer = size - getPreferredSize(axis)
				Debug.Assert(delta <> 0)
				Dim useMin As Boolean = (delta < 0)
				Dim springCount As Integer = springs.Count
				If useMin Then delta *= -1

				' The following algorithm if used for resizing springs:
				' 1. Calculate the resizability of each spring (pref - min or
				'    max - pref) into a list.
				' 2. Sort the list in ascending order
				' 3. Iterate through each of the resizable Springs, attempting
				'    to give them (pref - size) / resizeCount
				' 4. For any Springs that can not accommodate that much space
				'    add the remainder back to the amount to distribute and
				'    recalculate how must space the remaining springs will get.
				' 5. Set the size of the springs.

				' First pass, sort the resizable springs into the List resizable
				Dim ___resizable As IList(Of SpringDelta) = buildResizableList(axis, useMin)
				Dim resizableCount As Integer = ___resizable.Count

				If resizableCount > 0 Then
					' How much we would like to give each Spring.
					Dim sDelta As Integer = delta \ resizableCount
					' Remaining space.
					Dim slop As Integer = delta - sDelta * resizableCount
					Dim sizes As Integer() = New Integer(springCount - 1){}
					Dim sign As Integer = If(useMin, -1, 1)
					' Second pass, accumulate the resulting deltas (relative to
					' preferred) into sizes.
					For counter As Integer = 0 To resizableCount - 1
						Dim springDelta As SpringDelta = ___resizable(counter)
						If (counter + 1) = resizableCount Then sDelta += slop
						springDelta.delta = Math.Min(sDelta, springDelta.delta)
						delta -= springDelta.delta
						If springDelta.delta <> sDelta AndAlso counter + 1 < resizableCount Then
							' Spring didn't take all the space, reset how much
							' each spring will get.
							sDelta = delta \ (resizableCount - counter - 1)
							slop = delta - sDelta * (resizableCount - counter - 1)
						End If
						sizes(springDelta.index) = sign * springDelta.delta
					Next counter

					' And finally set the size of each spring
					For counter As Integer = 0 To springCount - 1
						Dim ___spring As Spring = getSpring(counter)
						Dim sSize As Integer = ___spring.getPreferredSize(axis) + sizes(counter)
						___spring.sizeize(axis, origin, sSize)
						origin += sSize
					Next counter
				Else
					' Nothing resizable, use the min or max of each of the
					' springs.
					For counter As Integer = 0 To springCount - 1
						Dim ___spring As Spring = getSpring(counter)
						Dim sSize As Integer
						If useMin Then
							sSize = ___spring.getMinimumSize(axis)
						Else
							sSize = ___spring.getMaximumSize(axis)
						End If
						___spring.sizeize(axis, origin, sSize)
						origin += sSize
					Next counter
				End If
			End Sub

			''' <summary>
			''' Returns the sorted list of SpringDelta's for the current set of
			''' Springs. The list is ordered based on the amount of flexibility of
			''' the springs.
			''' </summary>
			Private Function buildResizableList(ByVal axis As Integer, ByVal useMin As Boolean) As IList(Of SpringDelta)
				' First pass, figure out what is resizable
				Dim ___size As Integer = springs.Count
				Dim sorted As IList(Of SpringDelta) = New List(Of SpringDelta)(___size)
				For counter As Integer = 0 To ___size - 1
					Dim ___spring As Spring = getSpring(counter)
					Dim sDelta As Integer
					If useMin Then
						sDelta = ___spring.getPreferredSize(axis) - ___spring.getMinimumSize(axis)
					Else
						sDelta = ___spring.getMaximumSize(axis) - ___spring.getPreferredSize(axis)
					End If
					If sDelta > 0 Then sorted.Add(New SpringDelta(counter, sDelta))
				Next counter
				Collections.sort(sorted)
				Return sorted
			End Function

			Private Function indexOfNextNonZeroSpring(ByVal index As Integer, ByVal treatAutopaddingAsZeroSized As Boolean) As Integer
				Do While index < springs.Count
					Dim ___spring As Spring = springs(index)
					If Not ___spring.willHaveZeroSize(treatAutopaddingAsZeroSized) Then Return index
					index += 1
				Loop
				Return index
			End Function

			Friend Overrides Sub insertAutopadding(ByVal axis As Integer, ByVal leadingPadding As IList(Of AutoPreferredGapSpring), ByVal trailingPadding As IList(Of AutoPreferredGapSpring), ByVal leading As IList(Of ComponentSpring), ByVal trailing As IList(Of ComponentSpring), ByVal insert As Boolean)
				Dim newLeadingPadding As IList(Of AutoPreferredGapSpring) = New List(Of AutoPreferredGapSpring)(leadingPadding)
				Dim newTrailingPadding As IList(Of AutoPreferredGapSpring) = New List(Of AutoPreferredGapSpring)(1)
				Dim newLeading As IList(Of ComponentSpring) = New List(Of ComponentSpring)(leading)
				Dim newTrailing As IList(Of ComponentSpring) = Nothing
				Dim counter As Integer = 0
				' Warning, this must use springs.size, as it may change during the
				' loop.
				Do While counter < springs.Count
					Dim ___spring As Spring = getSpring(counter)
					If TypeOf ___spring Is AutoPreferredGapSpring Then
						If newLeadingPadding.Count = 0 Then
							' Autopadding spring. Set the sources of the
							' autopadding spring based on newLeading.
							Dim padding As AutoPreferredGapSpring = CType(___spring, AutoPreferredGapSpring)
							padding.sources = newLeading
							newLeading.Clear()
							counter = indexOfNextNonZeroSpring(counter + 1, True)
							If counter = springs.Count Then
								' Last spring in the list, add it to
								' trailingPadding.
								If Not(TypeOf padding Is ContainerAutoPreferredGapSpring) Then trailingPadding.Add(padding)
							Else
								newLeadingPadding.Clear()
								newLeadingPadding.Add(padding)
							End If
						Else
							counter = indexOfNextNonZeroSpring(counter + 1, True)
						End If
					Else
						' Not a padding spring
						If newLeading.Count > 0 AndAlso insert Then
							' There's leading ComponentSprings, create an
							' autopadding spring.
							Dim padding As New AutoPreferredGapSpring
							' Force the newly created spring to be considered
							' by NOT incrementing counter
							springs.Insert(counter, padding)
							Continue Do
						End If
						If TypeOf ___spring Is ComponentSpring Then
							' Spring is a Component, make it the target of any
							' leading AutopaddingSpring.
							Dim cSpring As ComponentSpring = CType(___spring, ComponentSpring)
							If Not cSpring.visible Then
								counter += 1
								Continue Do
							End If
							For Each gapSpring As AutoPreferredGapSpring In newLeadingPadding
								gapSpring.addTarget(cSpring, axis)
							Next gapSpring
							newLeading.Clear()
							newLeadingPadding.Clear()
							counter = indexOfNextNonZeroSpring(counter + 1, False)
							If counter = springs.Count Then
								' Last Spring, add it to trailing
								trailing.Add(cSpring)
							Else
								' Not that last Spring, add it to leading
								newLeading.Add(cSpring)
							End If
						ElseIf TypeOf ___spring Is Group Then
							' Forward call to child Group
							If newTrailing Is Nothing Then
								newTrailing = New List(Of ComponentSpring)(1)
							Else
								newTrailing.Clear()
							End If
							newTrailingPadding.Clear()
							CType(___spring, Group).insertAutopadding(axis, newLeadingPadding, newTrailingPadding, newLeading, newTrailing, insert)
							newLeading.Clear()
							newLeadingPadding.Clear()
							counter = indexOfNextNonZeroSpring(counter + 1, (newTrailing.Count = 0))
							If counter = springs.Count Then
								trailing.AddRange(newTrailing)
								trailingPadding.AddRange(newTrailingPadding)
							Else
								newLeading.AddRange(newTrailing)
								newLeadingPadding.AddRange(newTrailingPadding)
							End If
						Else
							' Gap
							newLeadingPadding.Clear()
							newLeading.Clear()
							counter += 1
						End If
					End If
				Loop
			End Sub

			Friend Property Overrides baseline As Integer
				Get
					If baselineSpring IsNot Nothing Then
						Dim ___baseline As Integer = baselineSpring.baseline
						If ___baseline >= 0 Then
							Dim ___size As Integer = 0
							For Each ___spring As Spring In springs
								If ___spring Is baselineSpring Then
									Return ___size + ___baseline
								Else
									___size += ___spring.getPreferredSize(VERTICAL)
								End If
							Next ___spring
						End If
					End If
					Return -1
				End Get
			End Property

			Friend Property Overrides baselineResizeBehavior As BaselineResizeBehavior
				Get
					If isResizable(VERTICAL) Then
						If Not baselineSpring.isResizable(VERTICAL) Then
							' Spring to use for baseline isn't resizable. In this case
							' baseline resize behavior can be determined based on how
							' preceding springs resize.
							Dim leadingResizable As Boolean = False
							For Each ___spring As Spring In springs
								If ___spring Is baselineSpring Then
									Exit For
								ElseIf ___spring.isResizable(VERTICAL) Then
									leadingResizable = True
									Exit For
								End If
							Next ___spring
							Dim trailingResizable As Boolean = False
							For i As Integer = springs.Count - 1 To 0 Step -1
								Dim ___spring As Spring = springs(i)
								If ___spring Is baselineSpring Then Exit For
								If ___spring.isResizable(VERTICAL) Then
									trailingResizable = True
									Exit For
								End If
							Next i
							If leadingResizable AndAlso (Not trailingResizable) Then
								Return BaselineResizeBehavior.CONSTANT_DESCENT
							ElseIf (Not leadingResizable) AndAlso trailingResizable Then
								Return BaselineResizeBehavior.CONSTANT_ASCENT
							End If
							' If we get here, both leading and trailing springs are
							' resizable. Fall through to OTHER.
						Else
							Dim brb As BaselineResizeBehavior = baselineSpring.baselineResizeBehavior
							If brb Is BaselineResizeBehavior.CONSTANT_ASCENT Then
								For Each ___spring As Spring In springs
									If ___spring Is baselineSpring Then Return BaselineResizeBehavior.CONSTANT_ASCENT
									If ___spring.isResizable(VERTICAL) Then Return BaselineResizeBehavior.OTHER
								Next ___spring
							ElseIf brb Is BaselineResizeBehavior.CONSTANT_DESCENT Then
								For i As Integer = springs.Count - 1 To 0 Step -1
									Dim ___spring As Spring = springs(i)
									If ___spring Is baselineSpring Then Return BaselineResizeBehavior.CONSTANT_DESCENT
									If ___spring.isResizable(VERTICAL) Then Return BaselineResizeBehavior.OTHER
								Next i
							End If
						End If
						Return BaselineResizeBehavior.OTHER
					End If
					' Not resizable, treat as constant_ascent
					Return BaselineResizeBehavior.CONSTANT_ASCENT
				End Get
			End Property

			Private Sub checkPreferredGapValues(ByVal pref As Integer, ByVal max As Integer)
				If (pref < 0 AndAlso pref <> DEFAULT_SIZE AndAlso pref <> PREFERRED_SIZE) OrElse (max < 0 AndAlso max <> DEFAULT_SIZE AndAlso max <> PREFERRED_SIZE) OrElse (pref >= 0 AndAlso max >= 0 AndAlso pref > max) Then Throw New System.ArgumentException("Pref and max must be either DEFAULT_SIZE, " & "PREFERRED_SIZE, or >= 0 and pref <= max")
			End Sub
		End Class


		''' <summary>
		''' Used by SequentialGroup in calculating resizability of springs.
		''' </summary>
		Private NotInheritable Class SpringDelta
			Implements IComparable(Of SpringDelta)

			' Original index.
			Public ReadOnly index As Integer
			' Delta, one of pref - min or max - pref.
			Public delta As Integer

			Public Sub New(ByVal index As Integer, ByVal delta As Integer)
				Me.index = index
				Me.delta = delta
			End Sub

			Public Function compareTo(ByVal o As SpringDelta) As Integer
				Return delta - o.delta
			End Function

			Public Overrides Function ToString() As String
				Return MyBase.ToString() & "[index=" & index & ", delta=" & delta & "]"
			End Function
		End Class


		''' <summary>
		''' A {@code Group} that aligns and sizes it's children.
		''' {@code ParallelGroup} aligns it's children in
		''' four possible ways: along the baseline, centered, anchored to the
		''' leading edge, or anchored to the trailing edge.
		''' <h3>Baseline</h3>
		''' A {@code ParallelGroup} that aligns it's children along the
		''' baseline must first decide where the baseline is
		''' anchored. The baseline can either be anchored to the top, or
		''' anchored to the bottom of the group. That is, the distance between the
		''' baseline and the beginning of the group can be a constant
		''' distance, or the distance between the end of the group and the
		''' baseline can be a constant distance. The possible choices
		''' correspond to the {@code BaselineResizeBehavior} constants
		''' {@link
		''' java.awt.Component.BaselineResizeBehavior#CONSTANT_ASCENT CONSTANT_ASCENT} and
		''' {@link
		''' java.awt.Component.BaselineResizeBehavior#CONSTANT_DESCENT CONSTANT_DESCENT}.
		''' <p>
		''' The baseline anchor may be explicitly specified by the
		''' {@code createBaselineGroup} method, or determined based on the elements.
		''' If not explicitly specified, the baseline will be anchored to
		''' the bottom if all the elements with a baseline, and that are
		''' aligned to the baseline, have a baseline resize behavior of
		''' {@code CONSTANT_DESCENT}; otherwise the baseline is anchored to the top
		''' of the group.
		''' <p>
		''' Elements aligned to the baseline are resizable if they have have
		''' a baseline resize behavior of {@code CONSTANT_ASCENT} or
		''' {@code CONSTANT_DESCENT}. Elements with a baseline resize
		''' behavior of {@code OTHER} or {@code CENTER_OFFSET} are not resizable.
		''' <p>
		''' The baseline is calculated based on the preferred height of each
		''' of the elements that have a baseline. The baseline is
		''' calculated using the following algorithm:
		''' {@code max(maxNonBaselineHeight, maxAscent + maxDescent)}, where the
		''' {@code maxNonBaselineHeight} is the maximum height of all elements
		''' that do not have a baseline, or are not aligned along the baseline.
		''' {@code maxAscent} is the maximum ascent (baseline) of all elements that
		''' have a baseline and are aligned along the baseline.
		''' {@code maxDescent} is the maximum descent (preferred height - baseline)
		''' of all elements that have a baseline and are aligned along the baseline.
		''' <p>
		''' A {@code ParallelGroup} that aligns it's elements along the baseline
		''' is only useful along the vertical axis. If you create a
		''' baseline group and use it along the horizontal axis an
		''' {@code IllegalStateException} is thrown when you ask
		''' {@code GroupLayout} for the minimum, preferred or maximum size or
		''' attempt to layout the components.
		''' <p>
		''' Elements that are not aligned to the baseline and smaller than the size
		''' of the {@code ParallelGroup} are positioned in one of three
		''' ways: centered, anchored to the leading edge, or anchored to the
		''' trailing edge.
		''' 
		''' <h3>Non-baseline {@code ParallelGroup}</h3>
		''' {@code ParallelGroup}s created with an alignment other than
		''' {@code BASELINE} align elements that are smaller than the size
		''' of the group in one of three ways: centered, anchored to the
		''' leading edge, or anchored to the trailing edge.
		''' <p>
		''' The leading edge is based on the axis and {@code
		''' ComponentOrientation}.  For the vertical axis the top edge is
		''' always the leading edge, and the bottom edge is always the
		''' trailing edge. When the {@code ComponentOrientation} is {@code
		''' LEFT_TO_RIGHT}, the leading edge is the left edge and the
		''' trailing edge the right edge. A {@code ComponentOrientation} of
		''' {@code RIGHT_TO_LEFT} flips the left and right edges. Child
		''' elements are aligned based on the specified alignment the
		''' element was added with. If you do not specify an alignment, the
		''' alignment specified for the {@code ParallelGroup} is used.
		''' <p>
		''' To align elements along the baseline you {@code createBaselineGroup},
		''' or {@code createParallelGroup} with an alignment of {@code BASELINE}.
		''' If the group was not created with a baseline alignment, and you attempt
		''' to add an element specifying a baseline alignment, an
		''' {@code IllegalArgumentException} is thrown.
		''' </summary>
		''' <seealso cref= #createParallelGroup() </seealso>
		''' <seealso cref= #createBaselineGroup(boolean,boolean)
		''' @since 1.6 </seealso>
		Public Class ParallelGroup
			Inherits Group

			Private ReadOnly outerInstance As GroupLayout

			' How children are layed out.
			Private ReadOnly childAlignment As Alignment
			' Whether or not we're resizable.
			Private ReadOnly resizable As Boolean

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal childAlignment As Alignment, ByVal resizable As Boolean)
					Me.outerInstance = outerInstance
				Me.childAlignment = childAlignment
				Me.resizable = resizable
			End Sub

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addGroup(ByVal group As Group) As ParallelGroup
				Return CType(MyBase.addGroup(group), ParallelGroup)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addComponent(ByVal component As java.awt.Component) As ParallelGroup
				Return CType(MyBase.addComponent(component), ParallelGroup)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addComponent(ByVal component As java.awt.Component, ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As ParallelGroup
				Return CType(MyBase.addComponent(component, min, pref, max), ParallelGroup)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addGap(ByVal pref As Integer) As ParallelGroup
				Return CType(MyBase.addGap(pref), ParallelGroup)
			End Function

			''' <summary>
			''' {@inheritDoc}
			''' </summary>
			Public Overrides Function addGap(ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As ParallelGroup
				Return CType(MyBase.addGap(min, pref, max), ParallelGroup)
			End Function

			''' <summary>
			''' Adds a {@code Group} to this {@code ParallelGroup} with the
			''' specified alignment. If the child is smaller than the
			''' {@code Group} it is aligned based on the specified
			''' alignment.
			''' </summary>
			''' <param name="alignment"> the alignment </param>
			''' <param name="group"> the {@code Group} to add </param>
			''' <returns> this {@code ParallelGroup} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code alignment} is
			'''         {@code null} </exception>
			Public Overridable Function addGroup(ByVal alignment As Alignment, ByVal group As Group) As ParallelGroup
				checkChildAlignment(alignment)
				group.alignment = alignment
				Return CType(addSpring(group), ParallelGroup)
			End Function

			''' <summary>
			''' Adds a {@code Component} to this {@code ParallelGroup} with
			''' the specified alignment.
			''' </summary>
			''' <param name="alignment"> the alignment </param>
			''' <param name="component"> the {@code Component} to add </param>
			''' <returns> this {@code Group} </returns>
			''' <exception cref="IllegalArgumentException"> if {@code alignment} is
			'''         {@code null} </exception>
			Public Overridable Function addComponent(ByVal component As java.awt.Component, ByVal alignment As Alignment) As ParallelGroup
				Return addComponent(component, alignment, DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE)
			End Function

			''' <summary>
			''' Adds a {@code Component} to this {@code ParallelGroup} with the
			''' specified alignment and size.
			''' </summary>
			''' <param name="alignment"> the alignment </param>
			''' <param name="component"> the {@code Component} to add </param>
			''' <param name="min"> the minimum size </param>
			''' <param name="pref"> the preferred size </param>
			''' <param name="max"> the maximum size </param>
			''' <exception cref="IllegalArgumentException"> if {@code alignment} is
			'''         {@code null} </exception>
			''' <returns> this {@code Group} </returns>
			Public Overridable Function addComponent(ByVal component As java.awt.Component, ByVal alignment As Alignment, ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer) As ParallelGroup
				checkChildAlignment(alignment)
				Dim ___spring As New ComponentSpring(component, min, pref, max)
				___spring.alignment = alignment
				Return CType(addSpring(___spring), ParallelGroup)
			End Function

			Friend Overridable Property resizable As Boolean
				Get
					Return resizable
				End Get
			End Property

			Friend Overrides Function [operator](ByVal a As Integer, ByVal b As Integer) As Integer
				Return Math.Max(a, b)
			End Function

			Friend Overrides Function calculateMinimumSize(ByVal axis As Integer) As Integer
				If Not resizable Then Return getPreferredSize(axis)
				Return MyBase.calculateMinimumSize(axis)
			End Function

			Friend Overrides Function calculateMaximumSize(ByVal axis As Integer) As Integer
				If Not resizable Then Return getPreferredSize(axis)
				Return MyBase.calculateMaximumSize(axis)
			End Function

			Friend Overrides Sub setValidSize(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				For Each ___spring As Spring In springs
					childSizeize(___spring, axis, origin, size)
				Next ___spring
			End Sub

			Friend Overridable Sub setChildSize(ByVal spring As Spring, ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				Dim ___alignment As Alignment = spring.alignment
				Dim ___springSize As Integer = Math.Min(Math.Max(spring.getMinimumSize(axis), size), spring.getMaximumSize(axis))
				If ___alignment Is Nothing Then ___alignment = childAlignment
				Select Case ___alignment
					Case Alignment.TRAILING
						spring.sizeize(axis, origin + size - ___springSize, ___springSize)
					Case Alignment.CENTER
						spring.sizeize(axis, origin + (size - ___springSize) \ 2,___springSize)
					Case Else ' LEADING, or BASELINE
						spring.sizeize(axis, origin, ___springSize)
				End Select
			End Sub

			Friend Overrides Sub insertAutopadding(ByVal axis As Integer, ByVal leadingPadding As IList(Of AutoPreferredGapSpring), ByVal trailingPadding As IList(Of AutoPreferredGapSpring), ByVal leading As IList(Of ComponentSpring), ByVal trailing As IList(Of ComponentSpring), ByVal insert As Boolean)
				For Each ___spring As Spring In springs
					If TypeOf ___spring Is ComponentSpring Then
						If CType(___spring, ComponentSpring).visible Then
							For Each gapSpring As AutoPreferredGapSpring In leadingPadding
								gapSpring.addTarget(CType(___spring, ComponentSpring), axis)
							Next gapSpring
							trailing.Add(CType(___spring, ComponentSpring))
						End If
					ElseIf TypeOf ___spring Is Group Then
						CType(___spring, Group).insertAutopadding(axis, leadingPadding, trailingPadding, leading, trailing, insert)
					ElseIf TypeOf ___spring Is AutoPreferredGapSpring Then
						CType(___spring, AutoPreferredGapSpring).sources = leading
						trailingPadding.Add(CType(___spring, AutoPreferredGapSpring))
					End If
				Next ___spring
			End Sub

			Private Sub checkChildAlignment(ByVal alignment As Alignment)
				checkChildAlignment(alignment, (TypeOf Me Is BaselineGroup))
			End Sub

			Private Sub checkChildAlignment(ByVal alignment As Alignment, ByVal allowsBaseline As Boolean)
				If alignment Is Nothing Then Throw New System.ArgumentException("Alignment must be non-null")
				If (Not allowsBaseline) AndAlso alignment = Alignment.BASELINE Then Throw New System.ArgumentException("Alignment must be one of:" & "LEADING, TRAILING or CENTER")
			End Sub
		End Class


		''' <summary>
		''' An extension of {@code ParallelGroup} that aligns its
		''' constituent {@code Spring}s along the baseline.
		''' </summary>
		Private Class BaselineGroup
			Inherits ParallelGroup

			Private ReadOnly outerInstance As GroupLayout

			' Whether or not all child springs have a baseline
			Private allSpringsHaveBaseline As Boolean

			' max(spring.getBaseline()) of all springs aligned along the baseline
			' that have a baseline
			Private prefAscent As Integer

			' max(spring.getPreferredSize().height - spring.getBaseline()) of all
			' springs aligned along the baseline that have a baseline
			Private prefDescent As Integer

			' Whether baselineAnchoredToTop was explicitly set
			Private baselineAnchorSet As Boolean

			' Whether the baseline is anchored to the top or the bottom.
			' If anchored to the top the baseline is always at prefAscent,
			' otherwise the baseline is at (height - prefDescent)
			Private baselineAnchoredToTop As Boolean

			' Whether or not the baseline has been calculated.
			Private calcedBaseline As Boolean

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal resizable As Boolean)
					Me.outerInstance = outerInstance
				MyBase.New(Alignment.LEADING, resizable)
					prefDescent = -1
					prefAscent = prefDescent
				calcedBaseline = False
			End Sub

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal resizable As Boolean, ByVal baselineAnchoredToTop As Boolean)
					Me.outerInstance = outerInstance
				Me.New(resizable)
				Me.baselineAnchoredToTop = baselineAnchoredToTop
				baselineAnchorSet = True
			End Sub

			Friend Overrides Sub unset()
				MyBase.unset()
					prefDescent = -1
					prefAscent = prefDescent
				calcedBaseline = False
			End Sub

			Friend Overrides Sub setValidSize(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				checkAxis(axis)
				If prefAscent = -1 Then
					MyBase.validSizeize(axis, origin, size)
				Else
					' do baseline layout
					baselineLayout(origin, size)
				End If
			End Sub

			Friend Overrides Function calculateSize(ByVal axis As Integer, ByVal type As Integer) As Integer
				checkAxis(axis)
				If Not calcedBaseline Then calculateBaselineAndResizeBehavior()
				If type = MIN_SIZE Then Return calculateMinSize()
				If type = MAX_SIZE Then Return calculateMaxSize()
				If allSpringsHaveBaseline Then Return prefAscent + prefDescent
				Return Math.Max(prefAscent + prefDescent, MyBase.calculateSize(axis, type))
			End Function

			Private Sub calculateBaselineAndResizeBehavior()
				' calculate baseline
				prefAscent = 0
				prefDescent = 0
				Dim baselineSpringCount As Integer = 0
				Dim resizeBehavior As BaselineResizeBehavior = Nothing
				For Each ___spring As Spring In springs
					If ___spring.alignment Is Nothing OrElse ___spring.alignment = Alignment.BASELINE Then
						Dim ___baseline As Integer = ___spring.baseline
						If ___baseline >= 0 Then
							If ___spring.isResizable(VERTICAL) Then
								Dim brb As BaselineResizeBehavior = ___spring.baselineResizeBehavior
								If resizeBehavior Is Nothing Then
									resizeBehavior = brb
								ElseIf brb IsNot resizeBehavior Then
									resizeBehavior = BaselineResizeBehavior.CONSTANT_ASCENT
								End If
							End If
							prefAscent = Math.Max(prefAscent, ___baseline)
							prefDescent = Math.Max(prefDescent, ___spring.getPreferredSize(VERTICAL) - ___baseline)
							baselineSpringCount += 1
						End If
					End If
				Next ___spring
				If Not baselineAnchorSet Then
					If resizeBehavior Is BaselineResizeBehavior.CONSTANT_DESCENT Then
						Me.baselineAnchoredToTop = False
					Else
						Me.baselineAnchoredToTop = True
					End If
				End If
				allSpringsHaveBaseline = (baselineSpringCount = springs.Count)
				calcedBaseline = True
			End Sub

			Private Function calculateMaxSize() As Integer
				Dim maxAscent As Integer = prefAscent
				Dim maxDescent As Integer = prefDescent
				Dim nonBaselineMax As Integer = 0
				For Each ___spring As Spring In springs
					Dim ___baseline As Integer
					Dim springMax As Integer = ___spring.getMaximumSize(VERTICAL)
					___baseline = ___spring.baseline
					If (___spring.alignment Is Nothing OrElse ___spring.alignment = Alignment.BASELINE) AndAlso ___baseline >= 0 Then
						Dim springPref As Integer = ___spring.getPreferredSize(VERTICAL)
						If springPref <> springMax Then
							Select Case ___spring.baselineResizeBehavior
								Case CONSTANT_ASCENT
									If baselineAnchoredToTop Then maxDescent = Math.Max(maxDescent, springMax - ___baseline)
								Case CONSTANT_DESCENT
									If Not baselineAnchoredToTop Then maxAscent = Math.Max(maxAscent, springMax - springPref + ___baseline)
								Case Else ' CENTER_OFFSET and OTHER, not resizable
							End Select
						End If
					Else
						' Not aligned along the baseline, or no baseline.
						nonBaselineMax = Math.Max(nonBaselineMax, springMax)
					End If
				Next ___spring
				Return Math.Max(nonBaselineMax, maxAscent + maxDescent)
			End Function

			Private Function calculateMinSize() As Integer
				Dim minAscent As Integer = 0
				Dim minDescent As Integer = 0
				Dim nonBaselineMin As Integer = 0
				If baselineAnchoredToTop Then
					minAscent = prefAscent
				Else
					minDescent = prefDescent
				End If
				For Each ___spring As Spring In springs
					Dim springMin As Integer = ___spring.getMinimumSize(VERTICAL)
					Dim ___baseline As Integer
					___baseline = ___spring.baseline
					If (___spring.alignment Is Nothing OrElse ___spring.alignment = Alignment.BASELINE) AndAlso ___baseline >= 0 Then
						Dim springPref As Integer = ___spring.getPreferredSize(VERTICAL)
						Dim brb As BaselineResizeBehavior = ___spring.baselineResizeBehavior
						Select Case brb
							Case CONSTANT_ASCENT
								If baselineAnchoredToTop Then
									minDescent = Math.Max(springMin - ___baseline, minDescent)
								Else
									minAscent = Math.Max(___baseline, minAscent)
								End If
							Case CONSTANT_DESCENT
								If Not baselineAnchoredToTop Then
									minAscent = Math.Max(___baseline - (springPref - springMin), minAscent)
								Else
									minDescent = Math.Max(springPref - ___baseline, minDescent)
								End If
							Case Else
								' CENTER_OFFSET and OTHER are !resizable, use
								' the preferred size.
								minAscent = Math.Max(___baseline, minAscent)
								minDescent = Math.Max(springPref - ___baseline, minDescent)
						End Select
					Else
						' Not aligned along the baseline, or no baseline.
						nonBaselineMin = Math.Max(nonBaselineMin, springMin)
					End If
				Next ___spring
				Return Math.Max(nonBaselineMin, minAscent + minDescent)
			End Function

			''' <summary>
			''' Lays out springs that have a baseline along the baseline.  All
			''' others are centered.
			''' </summary>
			Private Sub baselineLayout(ByVal origin As Integer, ByVal size As Integer)
				Dim ascent As Integer
				Dim descent As Integer
				If baselineAnchoredToTop Then
					ascent = prefAscent
					descent = size - ascent
				Else
					ascent = size - prefDescent
					descent = prefDescent
				End If
				For Each ___spring As Spring In springs
					Dim ___alignment As Alignment = ___spring.alignment
					If ___alignment Is Nothing OrElse ___alignment = Alignment.BASELINE Then
						Dim ___baseline As Integer = ___spring.baseline
						If ___baseline >= 0 Then
							Dim springMax As Integer = ___spring.getMaximumSize(VERTICAL)
							Dim springPref As Integer = ___spring.getPreferredSize(VERTICAL)
							Dim height As Integer = springPref
							Dim y As Integer
							Select Case ___spring.baselineResizeBehavior
								Case CONSTANT_ASCENT
									y = origin + ascent - ___baseline
									height = Math.Min(descent, springMax - ___baseline) + ___baseline
								Case CONSTANT_DESCENT
									height = Math.Min(ascent, springMax - springPref + ___baseline) + (springPref - ___baseline)
									y = origin + ascent + (springPref - ___baseline) - height
								Case Else ' CENTER_OFFSET & OTHER, not resizable
									y = origin + ascent - ___baseline
							End Select
							___spring.sizeize(VERTICAL, y, height)
						Else
							childSizeize(___spring, VERTICAL, origin, size)
						End If
					Else
						childSizeize(___spring, VERTICAL, origin, size)
					End If
				Next ___spring
			End Sub

			Friend Property Overrides baseline As Integer
				Get
					If springs.Count > 1 Then
						' Force the baseline to be calculated
						getPreferredSize(VERTICAL)
						Return prefAscent
					ElseIf springs.Count = 1 Then
						Return springs(0).baseline
					End If
					Return -1
				End Get
			End Property

			Friend Property Overrides baselineResizeBehavior As BaselineResizeBehavior
				Get
					If springs.Count = 1 Then Return springs(0).baselineResizeBehavior
					If baselineAnchoredToTop Then Return BaselineResizeBehavior.CONSTANT_ASCENT
					Return BaselineResizeBehavior.CONSTANT_DESCENT
				End Get
			End Property

			' If the axis is VERTICAL, throws an IllegalStateException
			Private Sub checkAxis(ByVal axis As Integer)
				If axis = HORIZONTAL Then Throw New IllegalStateException("Baseline must be used along vertical axis")
			End Sub
		End Class


		Private NotInheritable Class ComponentSpring
			Inherits Spring

			Private ReadOnly outerInstance As GroupLayout

			Private component As java.awt.Component
			Private origin As Integer

			' min/pref/max are either a value >= 0 or one of
			' DEFAULT_SIZE or PREFERRED_SIZE
			Private ReadOnly min As Integer
			Private ReadOnly pref As Integer
			Private ReadOnly max As Integer

			' Baseline for the component, computed as necessary.
			Private baseline As Integer = -1

			' Whether or not the size has been requested yet.
			Private installed As Boolean

			Private Sub New(ByVal outerInstance As GroupLayout, ByVal component As java.awt.Component, ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer)
					Me.outerInstance = outerInstance
				Me.component = component
				If component Is Nothing Then Throw New System.ArgumentException("Component must be non-null")

				checkSize(min, pref, max, True)

				Me.min = min
				Me.max = max
				Me.pref = pref

				' getComponentInfo makes sure component is a child of the
				' Container GroupLayout is the LayoutManager for.
				outerInstance.getComponentInfo(component)
			End Sub

			Friend Overrides Function calculateMinimumSize(ByVal axis As Integer) As Integer
				If isLinked(axis) Then Return getLinkSize(axis, MIN_SIZE)
				Return calculateNonlinkedMinimumSize(axis)
			End Function

			Friend Overrides Function calculatePreferredSize(ByVal axis As Integer) As Integer
				If isLinked(axis) Then Return getLinkSize(axis, PREF_SIZE)
				Dim min As Integer = getMinimumSize(axis)
				Dim pref As Integer = calculateNonlinkedPreferredSize(axis)
				Dim max As Integer = getMaximumSize(axis)
				Return Math.Min(max, Math.Max(min, pref))
			End Function

			Friend Overrides Function calculateMaximumSize(ByVal axis As Integer) As Integer
				If isLinked(axis) Then Return getLinkSize(axis, MAX_SIZE)
				Return Math.Max(getMinimumSize(axis), calculateNonlinkedMaximumSize(axis))
			End Function

			Friend Property visible As Boolean
				Get
					Return outerInstance.getComponentInfo(component).visible
				End Get
			End Property

			Friend Function calculateNonlinkedMinimumSize(ByVal axis As Integer) As Integer
				If Not visible Then Return 0
				If min >= 0 Then Return min
				If min = PREFERRED_SIZE Then Return calculateNonlinkedPreferredSize(axis)
				assert(min = DEFAULT_SIZE)
				Return getSizeAlongAxis(axis, component.minimumSize)
			End Function

			Friend Function calculateNonlinkedPreferredSize(ByVal axis As Integer) As Integer
				If Not visible Then Return 0
				If pref >= 0 Then Return pref
				assert(pref = DEFAULT_SIZE OrElse pref = PREFERRED_SIZE)
				Return getSizeAlongAxis(axis, component.preferredSize)
			End Function

			Friend Function calculateNonlinkedMaximumSize(ByVal axis As Integer) As Integer
				If Not visible Then Return 0
				If max >= 0 Then Return max
				If max = PREFERRED_SIZE Then Return calculateNonlinkedPreferredSize(axis)
				assert(max = DEFAULT_SIZE)
				Return getSizeAlongAxis(axis, component.maximumSize)
			End Function

			Private Function getSizeAlongAxis(ByVal axis As Integer, ByVal size As java.awt.Dimension) As Integer
				Return If(axis = HORIZONTAL, size.width, size.height)
			End Function

			Private Function getLinkSize(ByVal axis As Integer, ByVal type As Integer) As Integer
				If Not visible Then Return 0
				Dim ci As ComponentInfo = outerInstance.getComponentInfo(component)
				Return ci.getLinkSize(axis, type)
			End Function

			Friend Overrides Sub setSize(ByVal axis As Integer, ByVal origin As Integer, ByVal size As Integer)
				MyBase.sizeize(axis, origin, size)
				Me.origin = origin
				If size = UNSET Then baseline = -1
			End Sub

			Friend Property origin As Integer
				Get
					Return origin
				End Get
			End Property

			Friend Property component As java.awt.Component
				Set(ByVal component As java.awt.Component)
					Me.component = component
				End Set
				Get
					Return component
				End Get
			End Property


			Friend Property Overrides baseline As Integer
				Get
					If baseline = -1 Then
						Dim horizontalSpring As Spring = outerInstance.getComponentInfo(component).horizontalSpring
						Dim width As Integer = horizontalSpring.getPreferredSize(HORIZONTAL)
						Dim height As Integer = getPreferredSize(VERTICAL)
						If width > 0 AndAlso height > 0 Then baseline = component.getBaseline(width, height)
					End If
					Return baseline
				End Get
			End Property

			Friend Property Overrides baselineResizeBehavior As BaselineResizeBehavior
				Get
					Return component.baselineResizeBehavior
				End Get
			End Property

			Private Function isLinked(ByVal axis As Integer) As Boolean
				Return outerInstance.getComponentInfo(component).isLinked(axis)
			End Function

			Friend Sub installIfNecessary(ByVal axis As Integer)
				If Not installed Then
					installed = True
					If axis = HORIZONTAL Then
						outerInstance.getComponentInfo(component).horizontalSpring = Me
					Else
						outerInstance.getComponentInfo(component).verticalSpring = Me
					End If
				End If
			End Sub

			Friend Overrides Function willHaveZeroSize(ByVal treatAutopaddingAsZeroSized As Boolean) As Boolean
				Return Not visible
			End Function
		End Class


		''' <summary>
		''' Spring representing the preferred distance between two components.
		''' </summary>
		Private Class PreferredGapSpring
			Inherits Spring

			Private ReadOnly outerInstance As GroupLayout

			Private ReadOnly source As JComponent
			Private ReadOnly target As JComponent
			Private ReadOnly type As ComponentPlacement
			Private ReadOnly pref As Integer
			Private ReadOnly max As Integer

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal source As JComponent, ByVal target As JComponent, ByVal type As ComponentPlacement, ByVal pref As Integer, ByVal max As Integer)
					Me.outerInstance = outerInstance
				Me.source = source
				Me.target = target
				Me.type = type
				Me.pref = pref
				Me.max = max
			End Sub

			Friend Overrides Function calculateMinimumSize(ByVal axis As Integer) As Integer
				Return getPadding(axis)
			End Function

			Friend Overrides Function calculatePreferredSize(ByVal axis As Integer) As Integer
				If pref = DEFAULT_SIZE OrElse pref = PREFERRED_SIZE Then Return getMinimumSize(axis)
				Dim min As Integer = getMinimumSize(axis)
				Dim max As Integer = getMaximumSize(axis)
				Return Math.Min(max, Math.Max(min, pref))
			End Function

			Friend Overrides Function calculateMaximumSize(ByVal axis As Integer) As Integer
				If max = PREFERRED_SIZE OrElse max = DEFAULT_SIZE Then Return getPadding(axis)
				Return Math.Max(getMinimumSize(axis), max)
			End Function

			Private Function getPadding(ByVal axis As Integer) As Integer
				Dim position As Integer
				If axis = HORIZONTAL Then
					position = SwingConstants.EAST
				Else
					position = SwingConstants.SOUTH
				End If
				Return outerInstance.layoutStyle0.getPreferredGap(source, target, type, position, outerInstance.host)
			End Function

			Friend Overrides Function willHaveZeroSize(ByVal treatAutopaddingAsZeroSized As Boolean) As Boolean
				Return False
			End Function
		End Class


		''' <summary>
		''' Spring represented a certain amount of space.
		''' </summary>
		Private Class GapSpring
			Inherits Spring

			Private ReadOnly outerInstance As GroupLayout

			Private ReadOnly min As Integer
			Private ReadOnly pref As Integer
			Private ReadOnly max As Integer

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal min As Integer, ByVal pref As Integer, ByVal max As Integer)
					Me.outerInstance = outerInstance
				checkSize(min, pref, max, False)
				Me.min = min
				Me.pref = pref
				Me.max = max
			End Sub

			Friend Overrides Function calculateMinimumSize(ByVal axis As Integer) As Integer
				If min = PREFERRED_SIZE Then Return getPreferredSize(axis)
				Return min
			End Function

			Friend Overrides Function calculatePreferredSize(ByVal axis As Integer) As Integer
				Return pref
			End Function

			Friend Overrides Function calculateMaximumSize(ByVal axis As Integer) As Integer
				If max = PREFERRED_SIZE Then Return getPreferredSize(axis)
				Return max
			End Function

			Friend Overrides Function willHaveZeroSize(ByVal treatAutopaddingAsZeroSized As Boolean) As Boolean
				Return False
			End Function
		End Class


		''' <summary>
		''' Spring reprensenting the distance between any number of sources and
		''' targets.  The targets and sources are computed during layout.  An
		''' instance of this can either be dynamically created when
		''' autocreatePadding is true, or explicitly created by the developer.
		''' </summary>
		Private Class AutoPreferredGapSpring
			Inherits Spring

			Private ReadOnly outerInstance As GroupLayout

			Friend sources As IList(Of ComponentSpring)
			Friend source As ComponentSpring
			Private matches As IList(Of AutoPreferredGapMatch)
			Friend size As Integer
			Friend lastSize As Integer
			Private ReadOnly pref As Integer
			Private ReadOnly max As Integer
			' Type of gap
			Private type As ComponentPlacement
			Private userCreated As Boolean

			Private Sub New(ByVal outerInstance As GroupLayout)
					Me.outerInstance = outerInstance
				Me.pref = PREFERRED_SIZE
				Me.max = PREFERRED_SIZE
				Me.type = ComponentPlacement.RELATED
			End Sub

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal pref As Integer, ByVal max As Integer)
					Me.outerInstance = outerInstance
				Me.pref = pref
				Me.max = max
			End Sub

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal type As ComponentPlacement, ByVal pref As Integer, ByVal max As Integer)
					Me.outerInstance = outerInstance
				Me.type = type
				Me.pref = pref
				Me.max = max
				Me.userCreated = True
			End Sub

			Public Overridable Property source As ComponentSpring
				Set(ByVal source As ComponentSpring)
					Me.source = source
				End Set
			End Property

			Public Overridable Property sources As IList(Of ComponentSpring)
				Set(ByVal sources As IList(Of ComponentSpring))
					Me.sources = New List(Of ComponentSpring)(sources)
				End Set
			End Property

			Public Overridable Property userCreated As Boolean
				Set(ByVal userCreated As Boolean)
					Me.userCreated = userCreated
				End Set
				Get
					Return userCreated
				End Get
			End Property


			Friend Overrides Sub unset()
				lastSize = size
				MyBase.unset()
				size = 0
			End Sub

			Public Overridable Sub reset()
				size = 0
				sources = Nothing
				source = Nothing
				matches = Nothing
			End Sub

			Public Overridable Sub calculatePadding(ByVal axis As Integer)
				size = UNSET
				Dim maxPadding As Integer = UNSET
				If matches IsNot Nothing Then
					Dim p As LayoutStyle = outerInstance.layoutStyle0
					Dim position As Integer
					If axis = HORIZONTAL Then
						If outerInstance.leftToRight Then
							position = SwingConstants.EAST
						Else
							position = SwingConstants.WEST
						End If
					Else
						position = SwingConstants.SOUTH
					End If
					For i As Integer = matches.Count - 1 To 0 Step -1
						Dim match As AutoPreferredGapMatch = matches(i)
						maxPadding = Math.Max(maxPadding, calculatePadding(p, position, match.source, match.target))
					Next i
				End If
				If size = UNSET Then size = 0
				If maxPadding = UNSET Then maxPadding = 0
				If lastSize <> UNSET Then size += Math.Min(maxPadding, lastSize)
			End Sub

			Private Function calculatePadding(ByVal p As LayoutStyle, ByVal position As Integer, ByVal source As ComponentSpring, ByVal target As ComponentSpring) As Integer
				Dim delta As Integer = target.origin - (source.origin + source.size)
				If delta >= 0 Then
					Dim padding As Integer
					If (TypeOf source.component Is JComponent) AndAlso (TypeOf target.component Is JComponent) Then
						padding = p.getPreferredGap(CType(source.component, JComponent), CType(target.component, JComponent), type, position, outerInstance.host)
					Else
						padding = 10
					End If
					If padding > delta Then size = Math.Max(size, padding - delta)
					Return padding
				End If
				Return 0
			End Function

			Public Overridable Sub addTarget(ByVal spring As ComponentSpring, ByVal axis As Integer)
				Dim oAxis As Integer = If(axis = HORIZONTAL, VERTICAL, HORIZONTAL)
				If source IsNot Nothing Then
					If outerInstance.areParallelSiblings(source.component, spring.component, oAxis) Then addValidTarget(source, spring)
				Else
					Dim component As java.awt.Component = spring.component
					For counter As Integer = sources.Count - 1 To 0 Step -1
						Dim ___source As ComponentSpring = sources(counter)
						If outerInstance.areParallelSiblings(___source.component, component, oAxis) Then addValidTarget(___source, spring)
					Next counter
				End If
			End Sub

			Private Sub addValidTarget(ByVal source As ComponentSpring, ByVal target As ComponentSpring)
				If matches Is Nothing Then matches = New List(Of AutoPreferredGapMatch)(1)
				matches.Add(New AutoPreferredGapMatch(source, target))
			End Sub

			Friend Overrides Function calculateMinimumSize(ByVal axis As Integer) As Integer
				Return size
			End Function

			Friend Overrides Function calculatePreferredSize(ByVal axis As Integer) As Integer
				If pref = PREFERRED_SIZE OrElse pref = DEFAULT_SIZE Then Return size
				Return Math.Max(size, pref)
			End Function

			Friend Overrides Function calculateMaximumSize(ByVal axis As Integer) As Integer
				If max >= 0 Then Return Math.Max(getPreferredSize(axis), max)
				Return size
			End Function

			Friend Overridable Property matchDescription As String
				Get
					Return If(matches Is Nothing, "", matches.ToString())
				End Get
			End Property

			Public Overrides Function ToString() As String
				Return MyBase.ToString() & matchDescription
			End Function

			Friend Overrides Function willHaveZeroSize(ByVal treatAutopaddingAsZeroSized As Boolean) As Boolean
				Return treatAutopaddingAsZeroSized
			End Function
		End Class


		''' <summary>
		''' Represents two springs that should have autopadding inserted between
		''' them.
		''' </summary>
		Private NotInheritable Class AutoPreferredGapMatch
			Public ReadOnly source As ComponentSpring
			Public ReadOnly target As ComponentSpring

			Friend Sub New(ByVal source As ComponentSpring, ByVal target As ComponentSpring)
				Me.source = source
				Me.target = target
			End Sub

			Private Overrides Function ToString(ByVal spring As ComponentSpring) As String
				Return spring.component.name
			End Function

			Public Overrides Function ToString() As String
				Return "[" & ToString(source) & "-" & ToString(target) & "]"
			End Function
		End Class


		''' <summary>
		''' An extension of AutopaddingSpring used for container level padding.
		''' </summary>
		Private Class ContainerAutoPreferredGapSpring
			Inherits AutoPreferredGapSpring

			Private ReadOnly outerInstance As GroupLayout

			Private targets As IList(Of ComponentSpring)

			Friend Sub New(ByVal outerInstance As GroupLayout)
					Me.outerInstance = outerInstance
				MyBase.New()
				userCreated = True
			End Sub

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal pref As Integer, ByVal max As Integer)
					Me.outerInstance = outerInstance
				MyBase.New(pref, max)
				userCreated = True
			End Sub

			Public Overrides Sub addTarget(ByVal spring As ComponentSpring, ByVal axis As Integer)
				If targets Is Nothing Then targets = New List(Of ComponentSpring)(1)
				targets.Add(spring)
			End Sub

			Public Overrides Sub calculatePadding(ByVal axis As Integer)
				Dim p As LayoutStyle = outerInstance.layoutStyle0
				Dim maxPadding As Integer = 0
				Dim position As Integer
				size = 0
				If targets IsNot Nothing Then
					' Leading
					If axis = HORIZONTAL Then
						If outerInstance.leftToRight Then
							position = SwingConstants.WEST
						Else
							position = SwingConstants.EAST
						End If
					Else
						position = SwingConstants.SOUTH
					End If
					For i As Integer = targets.Count - 1 To 0 Step -1
						Dim targetSpring As ComponentSpring = targets(i)
						Dim padding As Integer = 10
						If TypeOf targetSpring.component Is JComponent Then
							padding = p.getContainerGap(CType(targetSpring.component, JComponent), position, outerInstance.host)
							maxPadding = Math.Max(padding, maxPadding)
							padding -= targetSpring.origin
						Else
							maxPadding = Math.Max(padding, maxPadding)
						End If
						size = Math.Max(size, padding)
					Next i
				Else
					' Trailing
					If axis = HORIZONTAL Then
						If outerInstance.leftToRight Then
							position = SwingConstants.EAST
						Else
							position = SwingConstants.WEST
						End If
					Else
						position = SwingConstants.SOUTH
					End If
					If sources IsNot Nothing Then
						For i As Integer = sources.Count - 1 To 0 Step -1
							Dim sourceSpring As ComponentSpring = sources(i)
							maxPadding = Math.Max(maxPadding, updateSize(p, sourceSpring, position))
						Next i
					ElseIf source IsNot Nothing Then
						maxPadding = updateSize(p, source, position)
					End If
				End If
				If lastSize <> UNSET Then size += Math.Min(maxPadding, lastSize)
			End Sub

			Private Function updateSize(ByVal p As LayoutStyle, ByVal sourceSpring As ComponentSpring, ByVal position As Integer) As Integer
				Dim padding As Integer = 10
				If TypeOf sourceSpring.component Is JComponent Then padding = p.getContainerGap(CType(sourceSpring.component, JComponent), position, outerInstance.host)
				Dim delta As Integer = Math.Max(0, parent.size - sourceSpring.size - sourceSpring.origin)
				size = Math.Max(size, padding - delta)
				Return padding
			End Function

			Friend Property Overrides matchDescription As String
				Get
					If targets IsNot Nothing Then Return "leading: " & targets.ToString()
					If sources IsNot Nothing Then Return "trailing: " & sources.ToString()
					Return "--"
				End Get
			End Property
		End Class


		' LinkInfo contains the set of ComponentInfosthat are linked along a
		' particular axis.
		Private Class LinkInfo
			Private ReadOnly axis As Integer
			Private ReadOnly linked As IList(Of ComponentInfo)
			Private size As Integer

			Friend Sub New(ByVal axis As Integer)
				linked = New List(Of ComponentInfo)
				size = UNSET
				Me.axis = axis
			End Sub

			Public Overridable Sub add(ByVal child As ComponentInfo)
				Dim childMaster As LinkInfo = child.getLinkInfo(axis, False)
				If childMaster Is Nothing Then
					linked.Add(child)
					child.linkInfonfo(axis, Me)
				ElseIf childMaster IsNot Me Then
					linked.AddRange(childMaster.linked)
					For Each childInfo As ComponentInfo In childMaster.linked
						childInfo.linkInfonfo(axis, Me)
					Next childInfo
				End If
				clearCachedSize()
			End Sub

			Public Overridable Sub remove(ByVal info As ComponentInfo)
				linked.Remove(info)
				info.linkInfonfo(axis, Nothing)
				If linked.Count = 1 Then linked(0).linkInfonfo(axis, Nothing)
				clearCachedSize()
			End Sub

			Public Overridable Sub clearCachedSize()
				size = UNSET
			End Sub

			Public Overridable Function getSize(ByVal axis As Integer) As Integer
				If size = UNSET Then size = calculateLinkedSize(axis)
				Return size
			End Function

			Private Function calculateLinkedSize(ByVal axis As Integer) As Integer
				Dim ___size As Integer = 0
				For Each info As ComponentInfo In linked
					Dim spring As ComponentSpring
					If axis = HORIZONTAL Then
						spring = info.horizontalSpring
					Else
						assert(axis = VERTICAL)
						spring = info.verticalSpring
					End If
					___size = Math.Max(___size, spring.calculateNonlinkedPreferredSize(axis))
				Next info
				Return ___size
			End Function
		End Class

		''' <summary>
		''' Tracks the horizontal/vertical Springs for a Component.
		''' This class is also used to handle Springs that have their sizes
		''' linked.
		''' </summary>
		Private Class ComponentInfo
			Private ReadOnly outerInstance As GroupLayout

			' Component being layed out
			Private component As java.awt.Component

			Friend horizontalSpring As ComponentSpring
			Friend verticalSpring As ComponentSpring

			' If the component's size is linked to other components, the
			' horizontalMaster and/or verticalMaster reference the group of
			' linked components.
			Private horizontalMaster As LinkInfo
			Private verticalMaster As LinkInfo

			Private visible As Boolean
			Private honorsVisibility As Boolean?

			Friend Sub New(ByVal outerInstance As GroupLayout, ByVal component As java.awt.Component)
					Me.outerInstance = outerInstance
				Me.component = component
				updateVisibility()
			End Sub

			Public Overridable Sub dispose()
				' Remove horizontal/vertical springs
				removeSpring(horizontalSpring)
				horizontalSpring = Nothing
				removeSpring(verticalSpring)
				verticalSpring = Nothing
				' Clean up links
				If horizontalMaster IsNot Nothing Then horizontalMaster.remove(Me)
				If verticalMaster IsNot Nothing Then verticalMaster.remove(Me)
			End Sub

			Friend Overridable Property honorsVisibility As Boolean?
				Set(ByVal honorsVisibility As Boolean?)
					Me.honorsVisibility = honorsVisibility
				End Set
			End Property

			Private Sub removeSpring(ByVal spring As Spring)
				If spring IsNot Nothing Then CType(spring.parent, Group).springs.Remove(spring)
			End Sub

			Public Overridable Property visible As Boolean
				Get
					Return visible
				End Get
			End Property

			''' <summary>
			''' Updates the cached visibility.
			''' </summary>
			''' <returns> true if the visibility changed </returns>
			Friend Overridable Function updateVisibility() As Boolean
				Dim ___honorsVisibility As Boolean
				If Me.honorsVisibility Is Nothing Then
					___honorsVisibility = outerInstance.honorsVisibility
				Else
					___honorsVisibility = Me.honorsVisibility
				End If
				Dim newVisible As Boolean = If(___honorsVisibility, component.visible, True)
				If visible <> newVisible Then
					visible = newVisible
					Return True
				End If
				Return False
			End Function

			Public Overridable Sub setBounds(ByVal insets As java.awt.Insets, ByVal parentWidth As Integer, ByVal ltr As Boolean)
				Dim x As Integer = horizontalSpring.origin
				Dim w As Integer = horizontalSpring.size
				Dim y As Integer = verticalSpring.origin
				Dim h As Integer = verticalSpring.size

				If Not ltr Then x = parentWidth - x - w
				component.boundsnds(x + insets.left, y + insets.top, w, h)
			End Sub

			Public Overridable Property component As java.awt.Component
				Set(ByVal component As java.awt.Component)
					Me.component = component
					If horizontalSpring IsNot Nothing Then horizontalSpring.component = component
					If verticalSpring IsNot Nothing Then verticalSpring.component = component
				End Set
				Get
					Return component
				End Get
			End Property


			''' <summary>
			''' Returns true if this component has its size linked to
			''' other components.
			''' </summary>
			Public Overridable Function isLinked(ByVal axis As Integer) As Boolean
				If axis = HORIZONTAL Then Return horizontalMaster IsNot Nothing
				assert(axis = VERTICAL)
				Return (verticalMaster IsNot Nothing)
			End Function

			Private Sub setLinkInfo(ByVal axis As Integer, ByVal linkInfo As LinkInfo)
				If axis = HORIZONTAL Then
					horizontalMaster = linkInfo
				Else
					assert(axis = VERTICAL)
					verticalMaster = linkInfo
				End If
			End Sub

			Public Overridable Function getLinkInfo(ByVal axis As Integer) As LinkInfo
				Return getLinkInfo(axis, True)
			End Function

			Private Function getLinkInfo(ByVal axis As Integer, ByVal create As Boolean) As LinkInfo
				If axis = HORIZONTAL Then
					If horizontalMaster Is Nothing AndAlso create Then CType(New LinkInfo(HORIZONTAL), LinkInfo).add(Me)
					Return horizontalMaster
				Else
					assert(axis = VERTICAL)
					If verticalMaster Is Nothing AndAlso create Then CType(New LinkInfo(VERTICAL), LinkInfo).add(Me)
					Return verticalMaster
				End If
			End Function

			Public Overridable Sub clearCachedSize()
				If horizontalMaster IsNot Nothing Then horizontalMaster.clearCachedSize()
				If verticalMaster IsNot Nothing Then verticalMaster.clearCachedSize()
			End Sub

			Friend Overridable Function getLinkSize(ByVal axis As Integer, ByVal type As Integer) As Integer
				If axis = HORIZONTAL Then
					Return horizontalMaster.getSize(axis)
				Else
					assert(axis = VERTICAL)
					Return verticalMaster.getSize(axis)
				End If
			End Function

		End Class
	End Class

End Namespace