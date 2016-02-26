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

Namespace javax.accessibility



	''' <summary>
	''' AccessibleContext represents the minimum information all accessible objects
	''' return.  This information includes the accessible name, description, role,
	''' and state of the object, as well as information about its parent and
	''' children.  AccessibleContext also contains methods for
	''' obtaining more specific accessibility information about a component.
	''' If the component supports them, these methods will return an object that
	''' implements one or more of the following interfaces:
	''' <P><ul>
	''' <li><seealso cref="AccessibleAction"/> - the object can perform one or more actions.
	''' This interface provides the standard mechanism for an assistive
	''' technology to determine what those actions are and tell the object
	''' to perform them.  Any object that can be manipulated should
	''' support this interface.
	''' <li><seealso cref="AccessibleComponent"/> - the object has a graphical representation.
	''' This interface provides the standard mechanism for an assistive
	''' technology to determine and set the graphical representation of the
	''' object.  Any object that is rendered on the screen should support
	''' this interface.
	''' <li><seealso cref=" AccessibleSelection"/> - the object allows its children to be
	''' selected.  This interface provides the standard mechanism for an
	''' assistive technology to determine the currently selected children of the object
	''' as well as modify its selection set.  Any object that has children
	''' that can be selected should support this interface.
	''' <li><seealso cref="AccessibleText"/> - the object presents editable textual information
	''' on the display.  This interface provides the standard mechanism for
	''' an assistive technology to access that text via its content, attributes,
	''' and spatial location.  Any object that contains editable text should
	''' support this interface.
	''' <li><seealso cref="AccessibleValue"/> - the object supports a numerical value.  This
	''' interface provides the standard mechanism for an assistive technology
	''' to determine and set the current value of the object, as well as obtain its
	''' minimum and maximum values.  Any object that supports a numerical value
	''' should support this interface.</ul>
	''' 
	''' 
	''' @beaninfo
	'''   attribute: isContainer false
	''' description: Minimal information that all accessible objects return
	''' 
	''' 
	''' @author      Peter Korn
	''' @author      Hans Muller
	''' @author      Willie Walker
	''' @author      Lynn Monsanto
	''' </summary>
	Public MustInherit Class AccessibleContext

		''' <summary>
		''' The AppContext that should be used to dispatch events for this
		''' AccessibleContext
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		Private targetAppContext As sun.awt.AppContext

		Shared Sub New()
'JAVA TO VB CONVERTER TODO TASK: Anonymous inner classes are not converted to VB if the base type is not defined in the code being converted:
'			sun.awt.AWTAccessor.setAccessibleContextAccessor(New sun.awt.AWTAccessor.AccessibleContextAccessor()
	'		{
	'			@Override public void setAppContext(AccessibleContext accessibleContext, AppContext appContext)
	'			{
	'				accessibleContext.targetAppContext = appContext;
	'			}
	'
	'			@Override public AppContext getAppContext(AccessibleContext accessibleContext)
	'			{
	'				Return accessibleContext.targetAppContext;
	'			}
	'		});
		End Sub

	   ''' <summary>
	   ''' Constant used to determine when the accessibleName property has
	   ''' changed.  The old value in the PropertyChangeEvent will be the old
	   ''' accessibleName and the new value will be the new accessibleName.
	   ''' </summary>
	   ''' <seealso cref= #getAccessibleName </seealso>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_NAME_PROPERTY As String = "AccessibleName"

	   ''' <summary>
	   ''' Constant used to determine when the accessibleDescription property has
	   ''' changed.  The old value in the PropertyChangeEvent will be the
	   ''' old accessibleDescription and the new value will be the new
	   ''' accessibleDescription.
	   ''' </summary>
	   ''' <seealso cref= #getAccessibleDescription </seealso>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_DESCRIPTION_PROPERTY As String = "AccessibleDescription"

	   ''' <summary>
	   ''' Constant used to determine when the accessibleStateSet property has
	   ''' changed.  The old value will be the old AccessibleState and the new
	   ''' value will be the new AccessibleState in the accessibleStateSet.
	   ''' For example, if a component that supports the vertical and horizontal
	   ''' states changes its orientation from vertical to horizontal, the old
	   ''' value will be AccessibleState.VERTICAL and the new value will be
	   ''' AccessibleState.HORIZONTAL.  Please note that either value can also
	   ''' be null.  For example, when a component changes from being enabled
	   ''' to disabled, the old value will be AccessibleState.ENABLED
	   ''' and the new value will be null.
	   ''' </summary>
	   ''' <seealso cref= #getAccessibleStateSet </seealso>
	   ''' <seealso cref= AccessibleState </seealso>
	   ''' <seealso cref= AccessibleStateSet </seealso>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_STATE_PROPERTY As String = "AccessibleState"

	   ''' <summary>
	   ''' Constant used to determine when the accessibleValue property has
	   ''' changed.  The old value in the PropertyChangeEvent will be a Number
	   ''' representing the old value and the new value will be a Number
	   ''' representing the new value
	   ''' </summary>
	   ''' <seealso cref= #getAccessibleValue </seealso>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_VALUE_PROPERTY As String = "AccessibleValue"

	   ''' <summary>
	   ''' Constant used to determine when the accessibleSelection has changed.
	   ''' The old and new values in the PropertyChangeEvent are currently
	   ''' reserved for future use.
	   ''' </summary>
	   ''' <seealso cref= #getAccessibleSelection </seealso>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_SELECTION_PROPERTY As String = "AccessibleSelection"

	   ''' <summary>
	   ''' Constant used to determine when the accessibleText caret has changed.
	   ''' The old value in the PropertyChangeEvent will be an
	   ''' integer representing the old caret position, and the new value will
	   ''' be an integer representing the new/current caret position.
	   ''' </summary>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_CARET_PROPERTY As String = "AccessibleCaret"

	   ''' <summary>
	   ''' Constant used to determine when the visual appearance of the object
	   ''' has changed.  The old and new values in the PropertyChangeEvent are
	   ''' currently reserved for future use.
	   ''' </summary>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_VISIBLE_DATA_PROPERTY As String = "AccessibleVisibleData"

	   ''' <summary>
	   ''' Constant used to determine when Accessible children are added/removed
	   ''' from the object.  If an Accessible child is being added, the old
	   ''' value will be null and the new value will be the Accessible child.  If an
	   ''' Accessible child is being removed, the old value will be the Accessible
	   ''' child, and the new value will be null.
	   ''' </summary>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_CHILD_PROPERTY As String = "AccessibleChild"

	   ''' <summary>
	   ''' Constant used to determine when the active descendant of a component
	   ''' has changed.  The active descendant is used for objects such as
	   ''' list, tree, and table, which may have transient children.  When the
	   ''' active descendant has changed, the old value of the property change
	   ''' event will be the Accessible representing the previous active child, and
	   ''' the new value will be the Accessible representing the current active
	   ''' child.
	   ''' </summary>
	   ''' <seealso cref= #addPropertyChangeListener </seealso>
	   Public Const ACCESSIBLE_ACTIVE_DESCENDANT_PROPERTY As String = "AccessibleActiveDescendant"

		''' <summary>
		''' Constant used to indicate that the table caption has changed
		''' The old value in the PropertyChangeEvent will be an Accessible
		''' representing the previous table caption and the new value will
		''' be an Accessible representing the new table caption. </summary>
		''' <seealso cref= Accessible </seealso>
		''' <seealso cref= AccessibleTable </seealso>
		Public Const ACCESSIBLE_TABLE_CAPTION_CHANGED As String = "accessibleTableCaptionChanged"

		''' <summary>
		''' Constant used to indicate that the table summary has changed
		''' The old value in the PropertyChangeEvent will be an Accessible
		''' representing the previous table summary and the new value will
		''' be an Accessible representing the new table summary. </summary>
		''' <seealso cref= Accessible </seealso>
		''' <seealso cref= AccessibleTable </seealso>
		Public Const ACCESSIBLE_TABLE_SUMMARY_CHANGED As String = "accessibleTableSummaryChanged"

		''' <summary>
		''' Constant used to indicate that table data has changed.
		''' The old value in the PropertyChangeEvent will be null and the
		''' new value will be an AccessibleTableModelChange representing
		''' the table change. </summary>
		''' <seealso cref= AccessibleTable </seealso>
		''' <seealso cref= AccessibleTableModelChange </seealso>
		Public Const ACCESSIBLE_TABLE_MODEL_CHANGED As String = "accessibleTableModelChanged"

		''' <summary>
		''' Constant used to indicate that the row header has changed
		''' The old value in the PropertyChangeEvent will be null and the
		''' new value will be an AccessibleTableModelChange representing
		''' the header change. </summary>
		''' <seealso cref= AccessibleTable </seealso>
		''' <seealso cref= AccessibleTableModelChange </seealso>
		Public Const ACCESSIBLE_TABLE_ROW_HEADER_CHANGED As String = "accessibleTableRowHeaderChanged"

		''' <summary>
		''' Constant used to indicate that the row description has changed
		''' The old value in the PropertyChangeEvent will be null and the
		''' new value will be an Integer representing the row index. </summary>
		''' <seealso cref= AccessibleTable </seealso>
		Public Const ACCESSIBLE_TABLE_ROW_DESCRIPTION_CHANGED As String = "accessibleTableRowDescriptionChanged"

		''' <summary>
		''' Constant used to indicate that the column header has changed
		''' The old value in the PropertyChangeEvent will be null and the
		''' new value will be an AccessibleTableModelChange representing
		''' the header change. </summary>
		''' <seealso cref= AccessibleTable </seealso>
		''' <seealso cref= AccessibleTableModelChange </seealso>
		Public Const ACCESSIBLE_TABLE_COLUMN_HEADER_CHANGED As String = "accessibleTableColumnHeaderChanged"

		''' <summary>
		''' Constant used to indicate that the column description has changed
		''' The old value in the PropertyChangeEvent will be null and the
		''' new value will be an Integer representing the column index. </summary>
		''' <seealso cref= AccessibleTable </seealso>
		Public Const ACCESSIBLE_TABLE_COLUMN_DESCRIPTION_CHANGED As String = "accessibleTableColumnDescriptionChanged"

		''' <summary>
		''' Constant used to indicate that the supported set of actions
		''' has changed.  The old value in the PropertyChangeEvent will
		''' be an Integer representing the old number of actions supported
		''' and the new value will be an Integer representing the new
		''' number of actions supported. </summary>
		''' <seealso cref= AccessibleAction </seealso>
		Public Const ACCESSIBLE_ACTION_PROPERTY As String = "accessibleActionProperty"

		''' <summary>
		''' Constant used to indicate that a hypertext element has received focus.
		''' The old value in the PropertyChangeEvent will be an Integer
		''' representing the start index in the document of the previous element
		''' that had focus and the new value will be an Integer representing
		''' the start index in the document of the current element that has
		''' focus.  A value of -1 indicates that an element does not or did
		''' not have focus. </summary>
		''' <seealso cref= AccessibleHyperlink </seealso>
		Public Const ACCESSIBLE_HYPERTEXT_OFFSET As String = "AccessibleHypertextOffset"

		''' <summary>
		''' PropertyChangeEvent which indicates that text has changed.
		''' <br>
		''' For text insertion, the oldValue is null and the newValue
		''' is an AccessibleTextSequence specifying the text that was
		''' inserted.
		''' <br>
		''' For text deletion, the oldValue is an AccessibleTextSequence
		''' specifying the text that was deleted and the newValue is null.
		''' <br>
		''' For text replacement, the oldValue is an AccessibleTextSequence
		''' specifying the old text and the newValue is an AccessibleTextSequence
		''' specifying the new text.
		''' </summary>
		''' <seealso cref= #getAccessibleText </seealso>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= AccessibleTextSequence </seealso>
		Public Const ACCESSIBLE_TEXT_PROPERTY As String = "AccessibleText"

		''' <summary>
		''' PropertyChangeEvent which indicates that a significant change
		''' has occurred to the children of a component like a tree or text.
		''' This change notifies the event listener that it needs to
		''' reacquire the state of the subcomponents. The oldValue is
		''' null and the newValue is the component whose children have
		''' become invalid.
		''' </summary>
		''' <seealso cref= #getAccessibleText </seealso>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= AccessibleTextSequence
		''' 
		''' @since 1.5 </seealso>
		Public Const ACCESSIBLE_INVALIDATE_CHILDREN As String = "accessibleInvalidateChildren"

		 ''' <summary>
		 ''' PropertyChangeEvent which indicates that text attributes have changed.
		 ''' <br>
		 ''' For attribute insertion, the oldValue is null and the newValue
		 ''' is an AccessibleAttributeSequence specifying the attributes that were
		 ''' inserted.
		 ''' <br>
		 ''' For attribute deletion, the oldValue is an AccessibleAttributeSequence
		 ''' specifying the attributes that were deleted and the newValue is null.
		 ''' <br>
		 ''' For attribute replacement, the oldValue is an AccessibleAttributeSequence
		 ''' specifying the old attributes and the newValue is an
		 ''' AccessibleAttributeSequence specifying the new attributes.
		 ''' </summary>
		 ''' <seealso cref= #getAccessibleText </seealso>
		 ''' <seealso cref= #addPropertyChangeListener </seealso>
		 ''' <seealso cref= AccessibleAttributeSequence
		 ''' 
		 ''' @since 1.5 </seealso>
		Public Const ACCESSIBLE_TEXT_ATTRIBUTES_CHANGED As String = "accessibleTextAttributesChanged"

	   ''' <summary>
	   ''' PropertyChangeEvent which indicates that a change has occurred
	   ''' in a component's bounds.
	   ''' The oldValue is the old component bounds and the newValue is
	   ''' the new component bounds.
	   ''' </summary>
	   ''' <seealso cref= #addPropertyChangeListener
	   '''  
	   ''' @since 1.5 </seealso>
		Public Const ACCESSIBLE_COMPONENT_BOUNDS_CHANGED As String = "accessibleComponentBoundsChanged"

		''' <summary>
		''' The accessible parent of this object.
		''' </summary>
		''' <seealso cref= #getAccessibleParent </seealso>
		''' <seealso cref= #setAccessibleParent </seealso>
		Protected Friend accessibleParent As Accessible = Nothing

		''' <summary>
		''' A localized String containing the name of the object.
		''' </summary>
		''' <seealso cref= #getAccessibleName </seealso>
		''' <seealso cref= #setAccessibleName </seealso>
		Protected Friend accessibleName As String = Nothing

		''' <summary>
		''' A localized String containing the description of the object.
		''' </summary>
		''' <seealso cref= #getAccessibleDescription </seealso>
		''' <seealso cref= #setAccessibleDescription </seealso>
		Protected Friend accessibleDescription As String = Nothing

		''' <summary>
		''' Used to handle the listener list for property change events.
		''' </summary>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= #removePropertyChangeListener </seealso>
		''' <seealso cref= #firePropertyChangeListener </seealso>
		Private accessibleChangeSupport As java.beans.PropertyChangeSupport = Nothing

		''' <summary>
		''' Used to represent the context's relation set </summary>
		''' <seealso cref= #getAccessibleRelationSet </seealso>
		Private relationSet As New AccessibleRelationSet

		Private nativeAXResource As Object

		''' <summary>
		''' Gets the accessibleName property of this object.  The accessibleName
		''' property of an object is a localized String that designates the purpose
		''' of the object.  For example, the accessibleName property of a label
		''' or button might be the text of the label or button itself.  In the
		''' case of an object that doesn't display its name, the accessibleName
		''' should still be set.  For example, in the case of a text field used
		''' to enter the name of a city, the accessibleName for the en_US locale
		''' could be 'city.'
		''' </summary>
		''' <returns> the localized name of the object; null if this
		''' object does not have a name
		''' </returns>
		''' <seealso cref= #setAccessibleName </seealso>
		Public Overridable Property accessibleName As String
			Get
				Return accessibleName
			End Get
			Set(ByVal s As String)
				Dim oldName As String = accessibleName
				accessibleName = s
				firePropertyChange(ACCESSIBLE_NAME_PROPERTY,oldName,accessibleName)
			End Set
		End Property


		''' <summary>
		''' Gets the accessibleDescription property of this object.  The
		''' accessibleDescription property of this object is a short localized
		''' phrase describing the purpose of the object.  For example, in the
		''' case of a 'Cancel' button, the accessibleDescription could be
		''' 'Ignore changes and close dialog box.'
		''' </summary>
		''' <returns> the localized description of the object; null if
		''' this object does not have a description
		''' </returns>
		''' <seealso cref= #setAccessibleDescription </seealso>
		Public Overridable Property accessibleDescription As String
			Get
				Return accessibleDescription
			End Get
			Set(ByVal s As String)
				Dim oldDescription As String = accessibleDescription
				accessibleDescription = s
				firePropertyChange(ACCESSIBLE_DESCRIPTION_PROPERTY, oldDescription,accessibleDescription)
			End Set
		End Property


		''' <summary>
		''' Gets the role of this object.  The role of the object is the generic
		''' purpose or use of the class of this object.  For example, the role
		''' of a push button is AccessibleRole.PUSH_BUTTON.  The roles in
		''' AccessibleRole are provided so component developers can pick from
		''' a set of predefined roles.  This enables assistive technologies to
		''' provide a consistent interface to various tweaked subclasses of
		''' components (e.g., use AccessibleRole.PUSH_BUTTON for all components
		''' that act like a push button) as well as distinguish between subclasses
		''' that behave differently (e.g., AccessibleRole.CHECK_BOX for check boxes
		''' and AccessibleRole.RADIO_BUTTON for radio buttons).
		''' <p>Note that the AccessibleRole class is also extensible, so
		''' custom component developers can define their own AccessibleRole's
		''' if the set of predefined roles is inadequate.
		''' </summary>
		''' <returns> an instance of AccessibleRole describing the role of the object </returns>
		''' <seealso cref= AccessibleRole </seealso>
		Public MustOverride ReadOnly Property accessibleRole As AccessibleRole

		''' <summary>
		''' Gets the state set of this object.  The AccessibleStateSet of an object
		''' is composed of a set of unique AccessibleStates.  A change in the
		''' AccessibleStateSet of an object will cause a PropertyChangeEvent to
		''' be fired for the ACCESSIBLE_STATE_PROPERTY property.
		''' </summary>
		''' <returns> an instance of AccessibleStateSet containing the
		''' current state set of the object </returns>
		''' <seealso cref= AccessibleStateSet </seealso>
		''' <seealso cref= AccessibleState </seealso>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		Public MustOverride ReadOnly Property accessibleStateSet As AccessibleStateSet

		''' <summary>
		''' Gets the Accessible parent of this object.
		''' </summary>
		''' <returns> the Accessible parent of this object; null if this
		''' object does not have an Accessible parent </returns>
		Public Overridable Property accessibleParent As Accessible
			Get
				Return accessibleParent
			End Get
			Set(ByVal a As Accessible)
				accessibleParent = a
			End Set
		End Property


		''' <summary>
		''' Gets the 0-based index of this object in its accessible parent.
		''' </summary>
		''' <returns> the 0-based index of this object in its parent; -1 if this
		''' object does not have an accessible parent.
		''' </returns>
		''' <seealso cref= #getAccessibleParent </seealso>
		''' <seealso cref= #getAccessibleChildrenCount </seealso>
		''' <seealso cref= #getAccessibleChild </seealso>
		Public MustOverride ReadOnly Property accessibleIndexInParent As Integer

		''' <summary>
		''' Returns the number of accessible children of the object.
		''' </summary>
		''' <returns> the number of accessible children of the object. </returns>
		Public MustOverride ReadOnly Property accessibleChildrenCount As Integer

		''' <summary>
		''' Returns the specified Accessible child of the object.  The Accessible
		''' children of an Accessible object are zero-based, so the first child
		''' of an Accessible child is at index 0, the second child is at index 1,
		''' and so on.
		''' </summary>
		''' <param name="i"> zero-based index of child </param>
		''' <returns> the Accessible child of the object </returns>
		''' <seealso cref= #getAccessibleChildrenCount </seealso>
		Public MustOverride Function getAccessibleChild(ByVal i As Integer) As Accessible

		''' <summary>
		''' Gets the locale of the component. If the component does not have a
		''' locale, then the locale of its parent is returned.
		''' </summary>
		''' <returns> this component's locale.  If this component does not have
		''' a locale, the locale of its parent is returned.
		''' </returns>
		''' <exception cref="IllegalComponentStateException">
		''' If the Component does not have its own locale and has not yet been
		''' added to a containment hierarchy such that the locale can be
		''' determined from the containing parent. </exception>
		Public MustOverride ReadOnly Property locale As java.util.Locale

		''' <summary>
		''' Adds a PropertyChangeListener to the listener list.
		''' The listener is registered for all Accessible properties and will
		''' be called when those properties change.
		''' </summary>
		''' <seealso cref= #ACCESSIBLE_NAME_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_DESCRIPTION_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_STATE_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_VALUE_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_SELECTION_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_TEXT_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_VISIBLE_DATA_PROPERTY
		''' </seealso>
		''' <param name="listener">  The PropertyChangeListener to be added </param>
		Public Overridable Sub addPropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			If accessibleChangeSupport Is Nothing Then accessibleChangeSupport = New java.beans.PropertyChangeSupport(Me)
			accessibleChangeSupport.addPropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Removes a PropertyChangeListener from the listener list.
		''' This removes a PropertyChangeListener that was registered
		''' for all properties.
		''' </summary>
		''' <param name="listener">  The PropertyChangeListener to be removed </param>
		Public Overridable Sub removePropertyChangeListener(ByVal listener As java.beans.PropertyChangeListener)
			If accessibleChangeSupport IsNot Nothing Then accessibleChangeSupport.removePropertyChangeListener(listener)
		End Sub

		''' <summary>
		''' Gets the AccessibleAction associated with this object that supports
		''' one or more actions.
		''' </summary>
		''' <returns> AccessibleAction if supported by object; else return null </returns>
		''' <seealso cref= AccessibleAction </seealso>
		Public Overridable Property accessibleAction As AccessibleAction
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the AccessibleComponent associated with this object that has a
		''' graphical representation.
		''' </summary>
		''' <returns> AccessibleComponent if supported by object; else return null </returns>
		''' <seealso cref= AccessibleComponent </seealso>
		Public Overridable Property accessibleComponent As AccessibleComponent
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the AccessibleSelection associated with this object which allows its
		''' Accessible children to be selected.
		''' </summary>
		''' <returns> AccessibleSelection if supported by object; else return null </returns>
		''' <seealso cref= AccessibleSelection </seealso>
		Public Overridable Property accessibleSelection As AccessibleSelection
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the AccessibleText associated with this object presenting
		''' text on the display.
		''' </summary>
		''' <returns> AccessibleText if supported by object; else return null </returns>
		''' <seealso cref= AccessibleText </seealso>
		Public Overridable Property accessibleText As AccessibleText
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the AccessibleEditableText associated with this object
		''' presenting editable text on the display.
		''' </summary>
		''' <returns> AccessibleEditableText if supported by object; else return null </returns>
		''' <seealso cref= AccessibleEditableText
		''' @since 1.4 </seealso>
		Public Overridable Property accessibleEditableText As AccessibleEditableText
			Get
				Return Nothing
			End Get
		End Property


		''' <summary>
		''' Gets the AccessibleValue associated with this object that supports a
		''' Numerical value.
		''' </summary>
		''' <returns> AccessibleValue if supported by object; else return null </returns>
		''' <seealso cref= AccessibleValue </seealso>
		Public Overridable Property accessibleValue As AccessibleValue
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the AccessibleIcons associated with an object that has
		''' one or more associated icons
		''' </summary>
		''' <returns> an array of AccessibleIcon if supported by object;
		''' otherwise return null </returns>
		''' <seealso cref= AccessibleIcon
		''' @since 1.3 </seealso>
		Public Overridable Property accessibleIcon As AccessibleIcon()
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Gets the AccessibleRelationSet associated with an object
		''' </summary>
		''' <returns> an AccessibleRelationSet if supported by object;
		''' otherwise return null </returns>
		''' <seealso cref= AccessibleRelationSet
		''' @since 1.3 </seealso>
		Public Overridable Property accessibleRelationSet As AccessibleRelationSet
			Get
				Return relationSet
			End Get
		End Property

		''' <summary>
		''' Gets the AccessibleTable associated with an object
		''' </summary>
		''' <returns> an AccessibleTable if supported by object;
		''' otherwise return null </returns>
		''' <seealso cref= AccessibleTable
		''' @since 1.3 </seealso>
		Public Overridable Property accessibleTable As AccessibleTable
			Get
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Support for reporting bound property changes.  If oldValue and
		''' newValue are not equal and the PropertyChangeEvent listener list
		''' is not empty, then fire a PropertyChange event to each listener.
		''' In general, this is for use by the Accessible objects themselves
		''' and should not be called by an application program. </summary>
		''' <param name="propertyName">  The programmatic name of the property that
		''' was changed. </param>
		''' <param name="oldValue">  The old value of the property. </param>
		''' <param name="newValue">  The new value of the property. </param>
		''' <seealso cref= java.beans.PropertyChangeSupport </seealso>
		''' <seealso cref= #addPropertyChangeListener </seealso>
		''' <seealso cref= #removePropertyChangeListener </seealso>
		''' <seealso cref= #ACCESSIBLE_NAME_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_DESCRIPTION_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_STATE_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_VALUE_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_SELECTION_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_TEXT_PROPERTY </seealso>
		''' <seealso cref= #ACCESSIBLE_VISIBLE_DATA_PROPERTY </seealso>
		Public Overridable Sub firePropertyChange(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			If accessibleChangeSupport IsNot Nothing Then
				If TypeOf newValue Is java.beans.PropertyChangeEvent Then
					Dim pce As java.beans.PropertyChangeEvent = CType(newValue, java.beans.PropertyChangeEvent)
					accessibleChangeSupport.firePropertyChange(pce)
				Else
					accessibleChangeSupport.firePropertyChange(propertyName, oldValue, newValue)
				End If
			End If
		End Sub
	End Class

End Namespace