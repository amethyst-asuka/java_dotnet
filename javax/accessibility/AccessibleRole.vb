'
' * Copyright (c) 1997, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' <P>Class AccessibleRole determines the role of a component.  The role of a
	''' component describes its generic function. (E.G.,
	''' "push button," "table," or "list.")
	''' <p>The toDisplayString method allows you to obtain the localized string
	''' for a locale independent key from a predefined ResourceBundle for the
	''' keys defined in this class.
	''' <p>The constants in this class present a strongly typed enumeration
	''' of common object roles.  A public constructor for this class has been
	''' purposely omitted and applications should use one of the constants
	''' from this class.  If the constants in this class are not sufficient
	''' to describe the role of an object, a subclass should be generated
	''' from this class and it should provide constants in a similar manner.
	''' 
	''' @author      Willie Walker
	''' @author      Peter Korn
	''' @author      Lynn Monsanto
	''' </summary>
	Public Class AccessibleRole
		Inherits AccessibleBundle

	' If you add or remove anything from here, make sure you
	' update AccessibleResourceBundle.java.

		''' <summary>
		''' Object is used to alert the user about something.
		''' </summary>
		Public Shared ReadOnly ALERT As New AccessibleRole("alert")

		''' <summary>
		''' The header for a column of data.
		''' </summary>
		Public Shared ReadOnly COLUMN_HEADER As New AccessibleRole("columnheader")

		''' <summary>
		''' Object that can be drawn into and is used to trap
		''' events. </summary>
		''' <seealso cref= #FRAME </seealso>
		''' <seealso cref= #GLASS_PANE </seealso>
		''' <seealso cref= #LAYERED_PANE </seealso>
		Public Shared ReadOnly CANVAS As New AccessibleRole("canvas")

		''' <summary>
		''' A list of choices the user can select from.  Also optionally
		''' allows the user to enter a choice of their own.
		''' </summary>
		Public Shared ReadOnly COMBO_BOX As New AccessibleRole("combobox")

		''' <summary>
		''' An iconified internal frame in a DESKTOP_PANE. </summary>
		''' <seealso cref= #DESKTOP_PANE </seealso>
		''' <seealso cref= #INTERNAL_FRAME </seealso>
		Public Shared ReadOnly DESKTOP_ICON As New AccessibleRole("desktopicon")

		''' <summary>
		''' An object containing a collection of <code>Accessibles</code> that
		''' together represents <code>HTML</code> content.  The child
		''' <code>Accessibles</code> would include objects implementing
		''' <code>AccessibleText</code>, <code>AccessibleHypertext</code>,
		''' <code>AccessibleIcon</code>, and other interfaces. </summary>
		''' <seealso cref= #HYPERLINK </seealso>
		''' <seealso cref= AccessibleText </seealso>
		''' <seealso cref= AccessibleHypertext </seealso>
		''' <seealso cref= AccessibleHyperlink </seealso>
		''' <seealso cref= AccessibleIcon
		''' @since 1.6 </seealso>
		Public Shared ReadOnly HTML_CONTAINER As New AccessibleRole("htmlcontainer")

		''' <summary>
		''' A frame-like object that is clipped by a desktop pane.  The
		''' desktop pane, internal frame, and desktop icon objects are
		''' often used to create multiple document interfaces within an
		''' application. </summary>
		''' <seealso cref= #DESKTOP_ICON </seealso>
		''' <seealso cref= #DESKTOP_PANE </seealso>
		''' <seealso cref= #FRAME </seealso>
		Public Shared ReadOnly INTERNAL_FRAME As New AccessibleRole("internalframe")

		''' <summary>
		''' A pane that supports internal frames and
		''' iconified versions of those internal frames. </summary>
		''' <seealso cref= #DESKTOP_ICON </seealso>
		''' <seealso cref= #INTERNAL_FRAME </seealso>
		Public Shared ReadOnly DESKTOP_PANE As New AccessibleRole("desktoppane")

		''' <summary>
		''' A specialized pane whose primary use is inside a DIALOG </summary>
		''' <seealso cref= #DIALOG </seealso>
		Public Shared ReadOnly OPTION_PANE As New AccessibleRole("optionpane")

		''' <summary>
		''' A top level window with no title or border. </summary>
		''' <seealso cref= #FRAME </seealso>
		''' <seealso cref= #DIALOG </seealso>
		Public Shared ReadOnly WINDOW As New AccessibleRole("window")

		''' <summary>
		''' A top level window with a title bar, border, menu bar, etc.  It is
		''' often used as the primary window for an application. </summary>
		''' <seealso cref= #DIALOG </seealso>
		''' <seealso cref= #CANVAS </seealso>
		''' <seealso cref= #WINDOW </seealso>
		Public Shared ReadOnly FRAME As New AccessibleRole("frame")

		''' <summary>
		''' A top level window with title bar and a border.  A dialog is similar
		''' to a frame, but it has fewer properties and is often used as a
		''' secondary window for an application. </summary>
		''' <seealso cref= #FRAME </seealso>
		''' <seealso cref= #WINDOW </seealso>
		Public Shared ReadOnly DIALOG As New AccessibleRole("dialog")

		''' <summary>
		''' A specialized pane that lets the user choose a color.
		''' </summary>
		Public Shared ReadOnly COLOR_CHOOSER As New AccessibleRole("colorchooser")


		''' <summary>
		''' A pane that allows the user to navigate through
		''' and select the contents of a directory.  May be used
		''' by a file chooser. </summary>
		''' <seealso cref= #FILE_CHOOSER </seealso>
		Public Shared ReadOnly DIRECTORY_PANE As New AccessibleRole("directorypane")

		''' <summary>
		''' A specialized dialog that displays the files in the directory
		''' and lets the user select a file, browse a different directory,
		''' or specify a filename.  May use the directory pane to show the
		''' contents of a directory. </summary>
		''' <seealso cref= #DIRECTORY_PANE </seealso>
		Public Shared ReadOnly FILE_CHOOSER As New AccessibleRole("filechooser")

		''' <summary>
		''' An object that fills up space in a user interface.  It is often
		''' used in interfaces to tweak the spacing between components,
		''' but serves no other purpose.
		''' </summary>
		Public Shared ReadOnly FILLER As New AccessibleRole("filler")

		''' <summary>
		''' A hypertext anchor
		''' </summary>
		Public Shared ReadOnly HYPERLINK As New AccessibleRole("hyperlink")

		''' <summary>
		''' A small fixed size picture, typically used to decorate components.
		''' </summary>
		Public Shared ReadOnly ICON As New AccessibleRole("icon")

		''' <summary>
		''' An object used to present an icon or short string in an interface.
		''' </summary>
		Public Shared ReadOnly LABEL As New AccessibleRole("label")

		''' <summary>
		''' A specialized pane that has a glass pane and a layered pane as its
		''' children. </summary>
		''' <seealso cref= #GLASS_PANE </seealso>
		''' <seealso cref= #LAYERED_PANE </seealso>
		Public Shared ReadOnly ROOT_PANE As New AccessibleRole("rootpane")

		''' <summary>
		''' A pane that is guaranteed to be painted on top
		''' of all panes beneath it. </summary>
		''' <seealso cref= #ROOT_PANE </seealso>
		''' <seealso cref= #CANVAS </seealso>
		Public Shared ReadOnly GLASS_PANE As New AccessibleRole("glasspane")

		''' <summary>
		''' A specialized pane that allows its children to be drawn in layers,
		''' providing a form of stacking order.  This is usually the pane that
		''' holds the menu bar as well as the pane that contains most of the
		''' visual components in a window. </summary>
		''' <seealso cref= #GLASS_PANE </seealso>
		''' <seealso cref= #ROOT_PANE </seealso>
		Public Shared ReadOnly LAYERED_PANE As New AccessibleRole("layeredpane")

		''' <summary>
		''' An object that presents a list of objects to the user and allows the
		''' user to select one or more of them.  A list is usually contained
		''' within a scroll pane. </summary>
		''' <seealso cref= #SCROLL_PANE </seealso>
		''' <seealso cref= #LIST_ITEM </seealso>
		Public Shared ReadOnly LIST As New AccessibleRole("list")

		''' <summary>
		''' An object that presents an element in a list.  A list is usually
		''' contained within a scroll pane. </summary>
		''' <seealso cref= #SCROLL_PANE </seealso>
		''' <seealso cref= #LIST </seealso>
		Public Shared ReadOnly LIST_ITEM As New AccessibleRole("listitem")

		''' <summary>
		''' An object usually drawn at the top of the primary dialog box of
		''' an application that contains a list of menus the user can choose
		''' from.  For example, a menu bar might contain menus for "File,"
		''' "Edit," and "Help." </summary>
		''' <seealso cref= #MENU </seealso>
		''' <seealso cref= #POPUP_MENU </seealso>
		''' <seealso cref= #LAYERED_PANE </seealso>
		Public Shared ReadOnly MENU_BAR As New AccessibleRole("menubar")

		''' <summary>
		''' A temporary window that is usually used to offer the user a
		''' list of choices, and then hides when the user selects one of
		''' those choices. </summary>
		''' <seealso cref= #MENU </seealso>
		''' <seealso cref= #MENU_ITEM </seealso>
		Public Shared ReadOnly POPUP_MENU As New AccessibleRole("popupmenu")

		''' <summary>
		''' An object usually found inside a menu bar that contains a list
		''' of actions the user can choose from.  A menu can have any object
		''' as its children, but most often they are menu items, other menus,
		''' or rudimentary objects such as radio buttons, check boxes, or
		''' separators.  For example, an application may have an "Edit" menu
		''' that contains menu items for "Cut" and "Paste." </summary>
		''' <seealso cref= #MENU_BAR </seealso>
		''' <seealso cref= #MENU_ITEM </seealso>
		''' <seealso cref= #SEPARATOR </seealso>
		''' <seealso cref= #RADIO_BUTTON </seealso>
		''' <seealso cref= #CHECK_BOX </seealso>
		''' <seealso cref= #POPUP_MENU </seealso>
		Public Shared ReadOnly MENU As New AccessibleRole("menu")

		''' <summary>
		''' An object usually contained in a menu that presents an action
		''' the user can choose.  For example, the "Cut" menu item in an
		''' "Edit" menu would be an action the user can select to cut the
		''' selected area of text in a document. </summary>
		''' <seealso cref= #MENU_BAR </seealso>
		''' <seealso cref= #SEPARATOR </seealso>
		''' <seealso cref= #POPUP_MENU </seealso>
		Public Shared ReadOnly MENU_ITEM As New AccessibleRole("menuitem")

		''' <summary>
		''' An object usually contained in a menu to provide a visual
		''' and logical separation of the contents in a menu.  For example,
		''' the "File" menu of an application might contain menu items for
		''' "Open," "Close," and "Exit," and will place a separator between
		''' "Close" and "Exit" menu items. </summary>
		''' <seealso cref= #MENU </seealso>
		''' <seealso cref= #MENU_ITEM </seealso>
		Public Shared ReadOnly SEPARATOR As New AccessibleRole("separator")

		''' <summary>
		''' An object that presents a series of panels (or page tabs), one at a
		''' time, through some mechanism provided by the object.  The most common
		''' mechanism is a list of tabs at the top of the panel.  The children of
		''' a page tab list are all page tabs. </summary>
		''' <seealso cref= #PAGE_TAB </seealso>
		Public Shared ReadOnly PAGE_TAB_LIST As New AccessibleRole("pagetablist")

		''' <summary>
		''' An object that is a child of a page tab list.  Its sole child is
		''' the panel that is to be presented to the user when the user
		''' selects the page tab from the list of tabs in the page tab list. </summary>
		''' <seealso cref= #PAGE_TAB_LIST </seealso>
		Public Shared ReadOnly PAGE_TAB As New AccessibleRole("pagetab")

		''' <summary>
		''' A generic container that is often used to group objects.
		''' </summary>
		Public Shared ReadOnly PANEL As New AccessibleRole("panel")

		''' <summary>
		''' An object used to indicate how much of a task has been completed.
		''' </summary>
		Public Shared ReadOnly PROGRESS_BAR As New AccessibleRole("progressbar")

		''' <summary>
		''' A text object used for passwords, or other places where the
		''' text contents is not shown visibly to the user
		''' </summary>
		Public Shared ReadOnly PASSWORD_TEXT As New AccessibleRole("passwordtext")

		''' <summary>
		''' An object the user can manipulate to tell the application to do
		''' something. </summary>
		''' <seealso cref= #CHECK_BOX </seealso>
		''' <seealso cref= #TOGGLE_BUTTON </seealso>
		''' <seealso cref= #RADIO_BUTTON </seealso>
		Public Shared ReadOnly PUSH_BUTTON As New AccessibleRole("pushbutton")

		''' <summary>
		''' A specialized push button that can be checked or unchecked, but
		''' does not provide a separate indicator for the current state. </summary>
		''' <seealso cref= #PUSH_BUTTON </seealso>
		''' <seealso cref= #CHECK_BOX </seealso>
		''' <seealso cref= #RADIO_BUTTON </seealso>
		Public Shared ReadOnly TOGGLE_BUTTON As New AccessibleRole("togglebutton")

		''' <summary>
		''' A choice that can be checked or unchecked and provides a
		''' separate indicator for the current state. </summary>
		''' <seealso cref= #PUSH_BUTTON </seealso>
		''' <seealso cref= #TOGGLE_BUTTON </seealso>
		''' <seealso cref= #RADIO_BUTTON </seealso>
		Public Shared ReadOnly CHECK_BOX As New AccessibleRole("checkbox")

		''' <summary>
		''' A specialized check box that will cause other radio buttons in the
		''' same group to become unchecked when this one is checked. </summary>
		''' <seealso cref= #PUSH_BUTTON </seealso>
		''' <seealso cref= #TOGGLE_BUTTON </seealso>
		''' <seealso cref= #CHECK_BOX </seealso>
		Public Shared ReadOnly RADIO_BUTTON As New AccessibleRole("radiobutton")

		''' <summary>
		''' The header for a row of data.
		''' </summary>
		Public Shared ReadOnly ROW_HEADER As New AccessibleRole("rowheader")

		''' <summary>
		''' An object that allows a user to incrementally view a large amount
		''' of information.  Its children can include scroll bars and a viewport. </summary>
		''' <seealso cref= #SCROLL_BAR </seealso>
		''' <seealso cref= #VIEWPORT </seealso>
		Public Shared ReadOnly SCROLL_PANE As New AccessibleRole("scrollpane")

		''' <summary>
		''' An object usually used to allow a user to incrementally view a
		''' large amount of data.  Usually used only by a scroll pane. </summary>
		''' <seealso cref= #SCROLL_PANE </seealso>
		Public Shared ReadOnly SCROLL_BAR As New AccessibleRole("scrollbar")

		''' <summary>
		''' An object usually used in a scroll pane.  It represents the portion
		''' of the entire data that the user can see.  As the user manipulates
		''' the scroll bars, the contents of the viewport can change. </summary>
		''' <seealso cref= #SCROLL_PANE </seealso>
		Public Shared ReadOnly VIEWPORT As New AccessibleRole("viewport")

		''' <summary>
		''' An object that allows the user to select from a bounded range.  For
		''' example, a slider might be used to select a number between 0 and 100.
		''' </summary>
		Public Shared ReadOnly SLIDER As New AccessibleRole("slider")

		''' <summary>
		''' A specialized panel that presents two other panels at the same time.
		''' Between the two panels is a divider the user can manipulate to make
		''' one panel larger and the other panel smaller.
		''' </summary>
		Public Shared ReadOnly SPLIT_PANE As New AccessibleRole("splitpane")

		''' <summary>
		''' An object used to present information in terms of rows and columns.
		''' An example might include a spreadsheet application.
		''' </summary>
		Public Shared ReadOnly TABLE As New AccessibleRole("table")

		''' <summary>
		''' An object that presents text to the user.  The text is usually
		''' editable by the user as opposed to a label. </summary>
		''' <seealso cref= #LABEL </seealso>
		Public Shared ReadOnly TEXT As New AccessibleRole("text")

		''' <summary>
		''' An object used to present hierarchical information to the user.
		''' The individual nodes in the tree can be collapsed and expanded
		''' to provide selective disclosure of the tree's contents.
		''' </summary>
		Public Shared ReadOnly TREE As New AccessibleRole("tree")

		''' <summary>
		''' A bar or palette usually composed of push buttons or toggle buttons.
		''' It is often used to provide the most frequently used functions for an
		''' application.
		''' </summary>
		Public Shared ReadOnly TOOL_BAR As New AccessibleRole("toolbar")

		''' <summary>
		''' An object that provides information about another object.  The
		''' accessibleDescription property of the tool tip is often displayed
		''' to the user in a small "help bubble" when the user causes the
		''' mouse to hover over the object associated with the tool tip.
		''' </summary>
		Public Shared ReadOnly TOOL_TIP As New AccessibleRole("tooltip")

		''' <summary>
		''' An AWT component, but nothing else is known about it. </summary>
		''' <seealso cref= #SWING_COMPONENT </seealso>
		''' <seealso cref= #UNKNOWN </seealso>
		Public Shared ReadOnly AWT_COMPONENT As New AccessibleRole("awtcomponent")

		''' <summary>
		''' A Swing component, but nothing else is known about it. </summary>
		''' <seealso cref= #AWT_COMPONENT </seealso>
		''' <seealso cref= #UNKNOWN </seealso>
		Public Shared ReadOnly SWING_COMPONENT As New AccessibleRole("swingcomponent")

		''' <summary>
		''' The object contains some Accessible information, but its role is
		''' not known. </summary>
		''' <seealso cref= #AWT_COMPONENT </seealso>
		''' <seealso cref= #SWING_COMPONENT </seealso>
		Public Shared ReadOnly UNKNOWN As New AccessibleRole("unknown")

		''' <summary>
		''' A STATUS_BAR is an simple component that can contain
		''' multiple labels of status information to the user.
		''' </summary>
		Public Shared ReadOnly STATUS_BAR As New AccessibleRole("statusbar")

		''' <summary>
		''' A DATE_EDITOR is a component that allows users to edit
		''' java.util.Date and java.util.Time objects
		''' </summary>
		Public Shared ReadOnly DATE_EDITOR As New AccessibleRole("dateeditor")

		''' <summary>
		''' A SPIN_BOX is a simple spinner component and its main use
		''' is for simple numbers.
		''' </summary>
		Public Shared ReadOnly SPIN_BOX As New AccessibleRole("spinbox")

		''' <summary>
		''' A FONT_CHOOSER is a component that lets the user pick various
		''' attributes for fonts.
		''' </summary>
		Public Shared ReadOnly FONT_CHOOSER As New AccessibleRole("fontchooser")

		''' <summary>
		''' A GROUP_BOX is a simple container that contains a border
		''' around it and contains components inside it.
		''' </summary>
		Public Shared ReadOnly GROUP_BOX As New AccessibleRole("groupbox")

		''' <summary>
		''' A text header
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly HEADER As New AccessibleRole("header")

		''' <summary>
		''' A text footer
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly FOOTER As New AccessibleRole("footer")

		''' <summary>
		''' A text paragraph
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly PARAGRAPH As New AccessibleRole("paragraph")

		''' <summary>
		''' A ruler is an object used to measure distance
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly RULER As New AccessibleRole("ruler")

		''' <summary>
		''' A role indicating the object acts as a formula for
		''' calculating a value.  An example is a formula in
		''' a spreadsheet cell.
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly EDITBAR As New AccessibleRole("editbar")

		''' <summary>
		''' A role indicating the object monitors the progress
		''' of some operation.
		''' 
		''' @since 1.5
		''' </summary>
		Public Shared ReadOnly PROGRESS_MONITOR As New AccessibleRole("progressMonitor")


	' The following are all under consideration for potential future use.

	'    public static final AccessibleRole APPLICATION
	'            = new AccessibleRole("application");

	'    public static final AccessibleRole BORDER
	'            = new AccessibleRole("border");

	'    public static final AccessibleRole CHECK_BOX_MENU_ITEM
	'            = new AccessibleRole("checkboxmenuitem");

	'    public static final AccessibleRole CHOICE
	'            = new AccessibleRole("choice");

	'    public static final AccessibleRole COLUMN
	'            = new AccessibleRole("column");

	'    public static final AccessibleRole CURSOR
	'            = new AccessibleRole("cursor");

	'    public static final AccessibleRole DOCUMENT
	'            = new AccessibleRole("document");

	'    public static final AccessibleRole IMAGE
	'            = new AccessibleRole("Image");

	'    public static final AccessibleRole INDICATOR
	'            = new AccessibleRole("indicator");

	'    public static final AccessibleRole RADIO_BUTTON_MENU_ITEM
	'            = new AccessibleRole("radiobuttonmenuitem");

	'    public static final AccessibleRole ROW
	'            = new AccessibleRole("row");

	'    public static final AccessibleRole TABLE_CELL
	'          = new AccessibleRole("tablecell");

	'    public static final AccessibleRole TREE_NODE
	'            = new AccessibleRole("treenode");

		''' <summary>
		''' Creates a new AccessibleRole using the given locale independent key.
		''' This should not be a public method.  Instead, it is used to create
		''' the constants in this file to make it a strongly typed enumeration.
		''' Subclasses of this class should enforce similar policy.
		''' <p>
		''' The key String should be a locale independent key for the role.
		''' It is not intended to be used as the actual String to display
		''' to the user.  To get the localized string, use toDisplayString.
		''' </summary>
		''' <param name="key"> the locale independent name of the role. </param>
		''' <seealso cref= AccessibleBundle#toDisplayString </seealso>
		Protected Friend Sub New(ByVal key As String)
			Me.key = key
		End Sub
	End Class

End Namespace