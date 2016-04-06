Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports javax.accessibility

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
	''' A Dialog is a top-level window with a title and a border
	''' that is typically used to take some form of input from the user.
	''' 
	''' The size of the dialog includes any area designated for the
	''' border.  The dimensions of the border area can be obtained
	''' using the <code>getInsets</code> method, however, since
	''' these dimensions are platform-dependent, a valid insets
	''' value cannot be obtained until the dialog is made displayable
	''' by either calling <code>pack</code> or <code>show</code>.
	''' Since the border area is included in the overall size of the
	''' dialog, the border effectively obscures a portion of the dialog,
	''' constraining the area available for rendering and/or displaying
	''' subcomponents to the rectangle which has an upper-left corner
	''' location of <code>(insets.left, insets.top)</code>, and has a size of
	''' <code>width - (insets.left + insets.right)</code> by
	''' <code>height - (insets.top + insets.bottom)</code>.
	''' <p>
	''' The default layout for a dialog is <code>BorderLayout</code>.
	''' <p>
	''' A dialog may have its native decorations (i.e. Frame &amp; Titlebar) turned off
	''' with <code>setUndecorated</code>.  This can only be done while the dialog
	''' is not <seealso cref="Component#isDisplayable() displayable"/>.
	''' <p>
	''' A dialog may have another window as its owner when it's constructed.  When
	''' the owner window of a visible dialog is minimized, the dialog will
	''' automatically be hidden from the user. When the owner window is subsequently
	''' restored, the dialog is made visible to the user again.
	''' <p>
	''' In a multi-screen environment, you can create a <code>Dialog</code>
	''' on a different screen device than its owner.  See <seealso cref="java.awt.Frame"/> for
	''' more information.
	''' <p>
	''' A dialog can be either modeless (the default) or modal.  A modal
	''' dialog is one which blocks input to some other top-level windows
	''' in the application, except for any windows created with the dialog
	''' as their owner. See <a href="doc-files/Modality.html">AWT Modality</a>
	''' specification for details.
	''' <p>
	''' Dialogs are capable of generating the following
	''' <code>WindowEvents</code>:
	''' <code>WindowOpened</code>, <code>WindowClosing</code>,
	''' <code>WindowClosed</code>, <code>WindowActivated</code>,
	''' <code>WindowDeactivated</code>, <code>WindowGainedFocus</code>,
	''' <code>WindowLostFocus</code>.
	''' </summary>
	''' <seealso cref= WindowEvent </seealso>
	''' <seealso cref= Window#addWindowListener
	''' 
	''' @author      Sami Shaio
	''' @author      Arthur van Hoff
	''' @since       JDK1.0 </seealso>
	Public Class Dialog
		Inherits Window

		Shared Sub New()
			' ensure that the necessary native libraries are loaded 
			Toolkit.loadLibraries()
			If Not GraphicsEnvironment.headless Then initIDs()
		End Sub

		''' <summary>
		''' A dialog's resizable property. Will be true
		''' if the Dialog is to be resizable, otherwise
		''' it will be false.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setResizable(boolean) </seealso>
		Friend resizable As Boolean = True


		''' <summary>
		''' This field indicates whether the dialog is undecorated.
		''' This property can only be changed while the dialog is not displayable.
		''' <code>undecorated</code> will be true if the dialog is
		''' undecorated, otherwise it will be false.
		''' 
		''' @serial </summary>
		''' <seealso cref= #setUndecorated(boolean) </seealso>
		''' <seealso cref= #isUndecorated() </seealso>
		''' <seealso cref= Component#isDisplayable()
		''' @since 1.4 </seealso>
		Friend undecorated As Boolean = False

		<NonSerialized> _
		Private initialized As Boolean = False

		''' <summary>
		''' Modal dialogs block all input to some top-level windows.
		''' Whether a particular window is blocked depends on dialog's type
		''' of modality; this is called the "scope of blocking". The
		''' <code>ModalityType</code> enum specifies modal types and their
		''' associated scopes.
		''' </summary>
		''' <seealso cref= Dialog#getModalityType </seealso>
		''' <seealso cref= Dialog#setModalityType </seealso>
		''' <seealso cref= Toolkit#isModalityTypeSupported
		''' 
		''' @since 1.6 </seealso>
		Public Enum ModalityType
			''' <summary>
			''' <code>MODELESS</code> dialog doesn't block any top-level windows.
			''' </summary>
			MODELESS
			''' <summary>
			''' A <code>DOCUMENT_MODAL</code> dialog blocks input to all top-level windows
			''' from the same document except those from its own child hierarchy.
			''' A document is a top-level window without an owner. It may contain child
			''' windows that, together with the top-level window are treated as a single
			''' solid document. Since every top-level window must belong to some
			''' document, its root can be found as the top-nearest window without an owner.
			''' </summary>
			DOCUMENT_MODAL
			''' <summary>
			''' An <code>APPLICATION_MODAL</code> dialog blocks all top-level windows
			''' from the same Java application except those from its own child hierarchy.
			''' If there are several applets launched in a browser, they can be
			''' treated either as separate applications or a single one. This behavior
			''' is implementation-dependent.
			''' </summary>
			APPLICATION_MODAL
			''' <summary>
			''' A <code>TOOLKIT_MODAL</code> dialog blocks all top-level windows run
			''' from the same toolkit except those from its own child hierarchy. If there
			''' are several applets launched in a browser, all of them run with the same
			''' toolkit; thus, a toolkit-modal dialog displayed by an applet may affect
			''' other applets and all windows of the browser instance which embeds the
			''' Java runtime environment for this toolkit.
			''' Special <code>AWTPermission</code> "toolkitModality" must be granted to use
			''' toolkit-modal dialogs. If a <code>TOOLKIT_MODAL</code> dialog is being created
			''' and this permission is not granted, a <code>SecurityException</code> will be
			''' thrown, and no dialog will be created. If a modality type is being changed
			''' to <code>TOOLKIT_MODAL</code> and this permission is not granted, a
			''' <code>SecurityException</code> will be thrown, and the modality type will
			''' be left unchanged.
			''' </summary>
			TOOLKIT_MODAL
		End Enum

		''' <summary>
		''' Default modality type for modal dialogs. The default modality type is
		''' <code>APPLICATION_MODAL</code>. Calling the oldstyle <code>setModal(true)</code>
		''' is equal to <code>setModalityType(DEFAULT_MODALITY_TYPE)</code>.
		''' </summary>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog#setModal
		''' 
		''' @since 1.6 </seealso>
		Public Const DEFAULT_MODALITY_TYPE As ModalityType = ModalityType.APPLICATION_MODAL

		''' <summary>
		''' True if this dialog is modal, false is the dialog is modeless.
		''' A modal dialog blocks user input to some application top-level
		''' windows. This field is kept only for backwards compatibility. Use the
		''' <seealso cref="Dialog.ModalityType ModalityType"/> enum instead.
		''' 
		''' @serial
		''' </summary>
		''' <seealso cref= #isModal </seealso>
		''' <seealso cref= #setModal </seealso>
		''' <seealso cref= #getModalityType </seealso>
		''' <seealso cref= #setModalityType </seealso>
		''' <seealso cref= ModalityType </seealso>
		''' <seealso cref= ModalityType#MODELESS </seealso>
		''' <seealso cref= #DEFAULT_MODALITY_TYPE </seealso>
		Friend modal As Boolean

		''' <summary>
		''' Modality type of this dialog. If the dialog's modality type is not
		''' <seealso cref="Dialog.ModalityType#MODELESS ModalityType.MODELESS"/>, it blocks all
		''' user input to some application top-level windows.
		''' 
		''' @serial
		''' </summary>
		''' <seealso cref= ModalityType </seealso>
		''' <seealso cref= #getModalityType </seealso>
		''' <seealso cref= #setModalityType
		''' 
		''' @since 1.6 </seealso>
		Friend modalityType As ModalityType

		''' <summary>
		''' Any top-level window can be marked not to be blocked by modal
		''' dialogs. This is called "modal exclusion". This enum specifies
		''' the possible modal exclusion types.
		''' </summary>
		''' <seealso cref= Window#getModalExclusionType </seealso>
		''' <seealso cref= Window#setModalExclusionType </seealso>
		''' <seealso cref= Toolkit#isModalExclusionTypeSupported
		''' 
		''' @since 1.6 </seealso>
		Public Enum ModalExclusionType
			''' <summary>
			''' No modal exclusion.
			''' </summary>
			NO_EXCLUDE
			''' <summary>
			''' <code>APPLICATION_EXCLUDE</code> indicates that a top-level window
			''' won't be blocked by any application-modal dialogs. Also, it isn't
			''' blocked by document-modal dialogs from outside of its child hierarchy.
			''' </summary>
			APPLICATION_EXCLUDE
			''' <summary>
			''' <code>TOOLKIT_EXCLUDE</code> indicates that a top-level window
			''' won't be blocked by  application-modal or toolkit-modal dialogs. Also,
			''' it isn't blocked by document-modal dialogs from outside of its
			''' child hierarchy.
			''' The "toolkitModality" <code>AWTPermission</code> must be granted
			''' for this exclusion. If an exclusion property is being changed to
			''' <code>TOOLKIT_EXCLUDE</code> and this permission is not granted, a
			''' <code>SecurityEcxeption</code> will be thrown, and the exclusion
			''' property will be left unchanged.
			''' </summary>
			TOOLKIT_EXCLUDE
		End Enum

		' operations with this list should be synchronized on tree lock
		<NonSerialized> _
		Friend Shared modalDialogs As New sun.awt.util.IdentityArrayList(Of Dialog)

		<NonSerialized> _
		Friend blockedWindows As New sun.awt.util.IdentityArrayList(Of Window)

		''' <summary>
		''' Specifies the title of the Dialog.
		''' This field can be null.
		''' 
		''' @serial </summary>
		''' <seealso cref= #getTitle() </seealso>
		''' <seealso cref= #setTitle(String) </seealso>
		Friend title As String

		<NonSerialized> _
		Private modalFilter As ModalEventFilter
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Private secondaryLoop As SecondaryLoop

	'    
	'     * Indicates that this dialog is being hidden. This flag is set to true at
	'     * the beginning of hide() and to false at the end of hide().
	'     *
	'     * @see #hide()
	'     * @see #hideAndDisposePreHandler()
	'     * @see #hideAndDisposeHandler()
	'     * @see #shouldBlock()
	'     
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend isInHide As Boolean = False

	'    
	'     * Indicates that this dialog is being disposed. This flag is set to true at
	'     * the beginning of doDispose() and to false at the end of doDispose().
	'     *
	'     * @see #hide()
	'     * @see #hideAndDisposePreHandler()
	'     * @see #hideAndDisposeHandler()
	'     * @see #doDispose()
	'     
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
		<NonSerialized> _
		Friend isInDispose As Boolean = False

		Private Const base As String = "dialog"
		Private Shared nameCounter As Integer = 0

	'    
	'     * JDK 1.1 serialVersionUID
	'     
		Private Const serialVersionUID As Long = 5920926903803293709L

		''' <summary>
		''' Constructs an initially invisible, modeless <code>Dialog</code> with
		''' the specified owner <code>Frame</code> and an empty title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if
		'''     this dialog has no owner </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible </seealso>
		 Public Sub New(  owner As Frame)
			 Me.New(owner, "", False)
		 End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the specified
		''' owner <code>Frame</code> and modality and an empty title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if
		'''     this dialog has no owner </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		'''     if <code>true</code>, the modality type property is set to
		'''     <code>DEFAULT_MODALITY_TYPE</code> </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		 Public Sub New(  owner As Frame,   modal As Boolean)
			 Me.New(owner, "", modal)
		 End Sub

		''' <summary>
		''' Constructs an initially invisible, modeless <code>Dialog</code> with
		''' the specified owner <code>Frame</code> and title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if
		'''     this dialog has no owner </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this dialog
		'''     has no title </param>
		''' <exception cref="IllegalArgumentException"> if the <code>owner</code>'s
		'''     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible </seealso>
		 Public Sub New(  owner As Frame,   title As String)
			 Me.New(owner, title, False)
		 End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the
		''' specified owner <code>Frame</code>, title and modality.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if
		'''     this dialog has no owner </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this dialog
		'''     has no title </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		'''     if <code>true</code>, the modality type property is set to
		'''     <code>DEFAULT_MODALITY_TYPE</code> </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible </seealso>
		 Public Sub New(  owner As Frame,   title As String,   modal As Boolean)
			 Me.New(owner, title,If(modal, DEFAULT_MODALITY_TYPE, ModalityType.MODELESS))
		 End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the specified owner
		''' <code>Frame</code>, title, modality, and <code>GraphicsConfiguration</code>. </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if this dialog
		'''     has no owner </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this dialog
		'''     has no title </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		'''     if <code>true</code>, the modality type property is set to
		'''     <code>DEFAULT_MODALITY_TYPE</code> </param>
		''' <param name="gc"> the <code>GraphicsConfiguration</code> of the target screen device;
		'''     if <code>null</code>, the default system <code>GraphicsConfiguration</code>
		'''     is assumed </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if <code>gc</code>
		'''     is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible
		''' @since 1.4 </seealso>
		 Public Sub New(  owner As Frame,   title As String,   modal As Boolean,   gc As GraphicsConfiguration)
			 Me.New(owner, title,If(modal, DEFAULT_MODALITY_TYPE, ModalityType.MODELESS), gc)
		 End Sub

		''' <summary>
		''' Constructs an initially invisible, modeless <code>Dialog</code> with
		''' the specified owner <code>Dialog</code> and an empty title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if this
		'''     dialog has no owner </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since 1.2 </seealso>
		 Public Sub New(  owner As Dialog)
			 Me.New(owner, "", False)
		 End Sub

		''' <summary>
		''' Constructs an initially invisible, modeless <code>Dialog</code>
		''' with the specified owner <code>Dialog</code> and title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if this
		'''     has no owner </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this dialog
		'''     has no title </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' @since 1.2 </seealso>
		 Public Sub New(  owner As Dialog,   title As String)
			 Me.New(owner, title, False)
		 End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the
		''' specified owner <code>Dialog</code>, title, and modality.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if this
		'''     dialog has no owner </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this
		'''     dialog has no title </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		'''     if <code>true</code>, the modality type property is set to
		'''     <code>DEFAULT_MODALITY_TYPE</code> </param>
		''' <exception cref="IllegalArgumentException"> if the <code>owner</code>'s
		'''    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' 
		''' @since 1.2 </seealso>
		 Public Sub New(  owner As Dialog,   title As String,   modal As Boolean)
			 Me.New(owner, title,If(modal, DEFAULT_MODALITY_TYPE, ModalityType.MODELESS))
		 End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the
		''' specified owner <code>Dialog</code>, title, modality and
		''' <code>GraphicsConfiguration</code>.
		''' </summary>
		''' <param name="owner"> the owner of the dialog or <code>null</code> if this
		'''     dialog has no owner </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this
		'''     dialog has no title </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If <code>false</code>, the dialog is <code>MODELESS</code>;
		'''     if <code>true</code>, the modality type property is set to
		'''     <code>DEFAULT_MODALITY_TYPE</code> </param>
		''' <param name="gc"> the <code>GraphicsConfiguration</code> of the target screen device;
		'''     if <code>null</code>, the default system <code>GraphicsConfiguration</code>
		'''     is assumed </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if <code>gc</code>
		'''    is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= Component#setSize </seealso>
		''' <seealso cref= Component#setVisible
		''' 
		''' @since 1.4 </seealso>
		 Public Sub New(  owner As Dialog,   title As String,   modal As Boolean,   gc As GraphicsConfiguration)
			 Me.New(owner, title,If(modal, DEFAULT_MODALITY_TYPE, ModalityType.MODELESS), gc)
		 End Sub

		''' <summary>
		''' Constructs an initially invisible, modeless <code>Dialog</code> with the
		''' specified owner <code>Window</code> and an empty title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog. The owner must be an instance of
		'''     <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		'''     of their descendents or <code>null</code>
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		'''     is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		'''     java.awt.Frame Frame} </exception>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(  owner As Window)
			Me.New(owner, "", ModalityType.MODELESS)
		End Sub

		''' <summary>
		''' Constructs an initially invisible, modeless <code>Dialog</code> with
		''' the specified owner <code>Window</code> and title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog. The owner must be an instance of
		'''    <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		'''    of their descendents or <code>null</code> </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this dialog
		'''    has no title
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		'''    is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		'''    java.awt.Frame Frame} </exception>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code>
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(  owner As Window,   title As String)
			Me.New(owner, title, ModalityType.MODELESS)
		End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the
		''' specified owner <code>Window</code> and modality and an empty title.
		''' </summary>
		''' <param name="owner"> the owner of the dialog. The owner must be an instance of
		'''    <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		'''    of their descendents or <code>null</code> </param>
		''' <param name="modalityType"> specifies whether dialog blocks input to other
		'''    windows when shown. <code>null</code> value and unsupported modality
		'''    types are equivalent to <code>MODELESS</code>
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		'''    is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		'''    java.awt.Frame Frame} </exception>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''    <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''    <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <exception cref="SecurityException"> if the calling thread does not have permission
		'''    to create modal dialogs with the given <code>modalityType</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= java.awt.Toolkit#isModalityTypeSupported
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(  owner As Window,   modalityType As ModalityType)
			Me.New(owner, "", modalityType)
		End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the
		''' specified owner <code>Window</code>, title and modality.
		''' </summary>
		''' <param name="owner"> the owner of the dialog. The owner must be an instance of
		'''     <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		'''     of their descendents or <code>null</code> </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this dialog
		'''     has no title </param>
		''' <param name="modalityType"> specifies whether dialog blocks input to other
		'''    windows when shown. <code>null</code> value and unsupported modality
		'''    types are equivalent to <code>MODELESS</code>
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		'''     is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		'''     java.awt.Frame Frame} </exception>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>'s
		'''     <code>GraphicsConfiguration</code> is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <exception cref="SecurityException"> if the calling thread does not have permission
		'''     to create modal dialogs with the given <code>modalityType</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= java.awt.Toolkit#isModalityTypeSupported
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(  owner As Window,   title As String,   modalityType As ModalityType)
			MyBase.New(owner)

			If (owner IsNot Nothing) AndAlso Not(TypeOf owner Is Frame) AndAlso Not(TypeOf owner Is Dialog) Then Throw New IllegalArgumentException("Wrong parent window")

			Me.title = title
			modalityType = modalityType
			sun.awt.SunToolkit.checkAndSetPolicy(Me)
			initialized = True
		End Sub

		''' <summary>
		''' Constructs an initially invisible <code>Dialog</code> with the
		''' specified owner <code>Window</code>, title, modality and
		''' <code>GraphicsConfiguration</code>.
		''' </summary>
		''' <param name="owner"> the owner of the dialog. The owner must be an instance of
		'''     <seealso cref="java.awt.Dialog Dialog"/>, <seealso cref="java.awt.Frame Frame"/>, any
		'''     of their descendents or <code>null</code> </param>
		''' <param name="title"> the title of the dialog or <code>null</code> if this dialog
		'''     has no title </param>
		''' <param name="modalityType"> specifies whether dialog blocks input to other
		'''    windows when shown. <code>null</code> value and unsupported modality
		'''    types are equivalent to <code>MODELESS</code> </param>
		''' <param name="gc"> the <code>GraphicsConfiguration</code> of the target screen device;
		'''     if <code>null</code>, the default system <code>GraphicsConfiguration</code>
		'''     is assumed
		''' </param>
		''' <exception cref="java.lang.IllegalArgumentException"> if the <code>owner</code>
		'''     is not an instance of <seealso cref="java.awt.Dialog Dialog"/> or {@link
		'''     java.awt.Frame Frame} </exception>
		''' <exception cref="java.lang.IllegalArgumentException"> if <code>gc</code>
		'''     is not from a screen device </exception>
		''' <exception cref="HeadlessException"> when
		'''     <code>GraphicsEnvironment.isHeadless()</code> returns <code>true</code> </exception>
		''' <exception cref="SecurityException"> if the calling thread does not have permission
		'''     to create modal dialogs with the given <code>modalityType</code>
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= java.awt.Toolkit#isModalityTypeSupported
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(  owner As Window,   title As String,   modalityType As ModalityType,   gc As GraphicsConfiguration)
			MyBase.New(owner, gc)

			If (owner IsNot Nothing) AndAlso Not(TypeOf owner Is Frame) AndAlso Not(TypeOf owner Is Dialog) Then Throw New IllegalArgumentException("wrong owner window")

			Me.title = title
			modalityType = modalityType
			sun.awt.SunToolkit.checkAndSetPolicy(Me)
			initialized = True
		End Sub

		''' <summary>
		''' Construct a name for this component.  Called by getName() when the
		''' name is null.
		''' </summary>
		Friend Overrides Function constructComponentName() As String
			SyncLock GetType(Dialog)
					Dim tempVar As Integer = nameCounter
					nameCounter += 1
					Return base + tempVar
			End SyncLock
		End Function

		''' <summary>
		''' Makes this Dialog displayable by connecting it to
		''' a native screen resource.  Making a dialog displayable will
		''' cause any of its children to be made displayable.
		''' This method is called internally by the toolkit and should
		''' not be called directly by programs. </summary>
		''' <seealso cref= Component#isDisplayable </seealso>
		''' <seealso cref= #removeNotify </seealso>
		Public Overrides Sub addNotify()
			SyncLock treeLock
				If parent IsNot Nothing AndAlso parent.peer Is Nothing Then parent.addNotify()

				If peer Is Nothing Then peer = toolkit.createDialog(Me)
				MyBase.addNotify()
			End SyncLock
		End Sub

		''' <summary>
		''' Indicates whether the dialog is modal.
		''' <p>
		''' This method is obsolete and is kept for backwards compatibility only.
		''' Use <seealso cref="#getModalityType getModalityType()"/> instead.
		''' </summary>
		''' <returns>    <code>true</code> if this dialog window is modal;
		'''            <code>false</code> otherwise
		''' </returns>
		''' <seealso cref=       java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref=       java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref=       java.awt.Dialog#setModal </seealso>
		''' <seealso cref=       java.awt.Dialog#getModalityType </seealso>
		''' <seealso cref=       java.awt.Dialog#setModalityType </seealso>
		Public Overridable Property modal As Boolean
			Get
				Return modal_NoClientCode
			End Get
			Set(  modal As Boolean)
				Me.modal = modal
				modalityType = If(modal, DEFAULT_MODALITY_TYPE, ModalityType.MODELESS)
			End Set
		End Property
		Friend Property modal_NoClientCode As Boolean
			Get
				Return modalityType <> ModalityType.MODELESS
			End Get
		End Property


		''' <summary>
		''' Returns the modality type of this dialog.
		''' </summary>
		''' <returns> modality type of this dialog
		''' </returns>
		''' <seealso cref= java.awt.Dialog#setModalityType
		''' 
		''' @since 1.6 </seealso>
		Public Overridable Property modalityType As ModalityType
			Get
				Return modalityType
			End Get
			Set(  type As ModalityType)
				If type Is Nothing Then type = Dialog.ModalityType.MODELESS
				If Not Toolkit.defaultToolkit.isModalityTypeSupported(type) Then type = Dialog.ModalityType.MODELESS
				If modalityType = type Then Return
    
				checkModalityPermission(type)
    
				modalityType = type
				modal = (modalityType <> ModalityType.MODELESS)
			End Set
		End Property


		''' <summary>
		''' Gets the title of the dialog. The title is displayed in the
		''' dialog's border. </summary>
		''' <returns>    the title of this dialog window. The title may be
		'''            <code>null</code>. </returns>
		''' <seealso cref=       java.awt.Dialog#setTitle </seealso>
		Public Overridable Property title As String
			Get
				Return title
			End Get
			Set(  title As String)
				Dim oldTitle As String = Me.title
    
				SyncLock Me
					Me.title = title
					Dim peer_Renamed As java.awt.peer.DialogPeer = CType(Me.peer, java.awt.peer.DialogPeer)
					If peer_Renamed IsNot Nothing Then peer_Renamed.title = title
				End SyncLock
				firePropertyChange("title", oldTitle, title)
			End Set
		End Property


		''' <returns> true if we actually showed, false if we just called toFront() </returns>
		Private Function conditionalShow(  toFocus As Component,   time As java.util.concurrent.atomic.AtomicLong) As Boolean
			Dim retval As Boolean

			closeSplashScreen()

			SyncLock treeLock
				If peer Is Nothing Then addNotify()
				validateUnconditionally()
				If visible Then
					toFront()
					retval = False
				Else
						retval = True
						visible = retval

					' check if this dialog should be modal blocked BEFORE calling peer.show(),
					' otherwise, a pair of FOCUS_GAINED and FOCUS_LOST may be mistakenly
					' generated for the dialog
					If Not modal Then
						checkShouldBeBlocked(Me)
					Else
						modalDialogs.add(Me)
						modalShow()
					End If

					If toFocus IsNot Nothing AndAlso time IsNot Nothing AndAlso focusable AndAlso enabled AndAlso (Not modalBlocked) Then
						' keep the KeyEvents from being dispatched
						' until the focus has been transfered
						time.set(Toolkit.eventQueue.mostRecentKeyEventTime)
						KeyboardFocusManager.currentKeyboardFocusManager.enqueueKeyEvents(time.get(), toFocus)
					End If

					' This call is required as the show() method of the Dialog class
					' does not invoke the super.show(). So wried... :(
					mixOnShowing()

					peer.visible = True ' now guaranteed never to block
					If modalBlocked Then modalBlocker.toFront()

					locationByPlatform = False
					For i As Integer = 0 To ownedWindowList.Count - 1
						Dim child As Window = ownedWindowList(i).get()
						If (child IsNot Nothing) AndAlso child.showWithParent Then
							child.show()
							child.showWithParent = False
						End If ' endif
					Next i ' endfor
					Window.updateChildFocusableWindowState(Me)

					createHierarchyEvents(HierarchyEvent.HIERARCHY_CHANGED, Me, parent, HierarchyEvent.SHOWING_CHANGED, Toolkit.enabledOnToolkit(AWTEvent.HIERARCHY_EVENT_MASK))
					If componentListener IsNot Nothing OrElse (eventMask And AWTEvent.COMPONENT_EVENT_MASK) <> 0 OrElse Toolkit.enabledOnToolkit(AWTEvent.COMPONENT_EVENT_MASK) Then
						Dim e As New ComponentEvent(Me, ComponentEvent.COMPONENT_SHOWN)
						Toolkit.eventQueue.postEvent(e)
					End If
				End If
			End SyncLock

			If retval AndAlso (state And OPENED) = 0 Then
				postWindowEvent(WindowEvent.WINDOW_OPENED)
				state = state Or OPENED
			End If

			Return retval
		End Function

		''' <summary>
		''' Shows or hides this {@code Dialog} depending on the value of parameter
		''' {@code b}. </summary>
		''' <param name="b"> if {@code true}, makes the {@code Dialog} visible,
		''' otherwise hides the {@code Dialog}.
		''' If the dialog and/or its owner
		''' are not yet displayable, both are made displayable.  The
		''' dialog will be validated prior to being made visible.
		''' If {@code false}, hides the {@code Dialog} and then causes {@code setVisible(true)}
		''' to return if it is currently blocked.
		''' <p>
		''' <b>Notes for modal dialogs</b>.
		''' <ul>
		''' <li>{@code setVisible(true)}:  If the dialog is not already
		''' visible, this call will not return until the dialog is
		''' hidden by calling {@code setVisible(false)} or
		''' {@code dispose}.
		''' <li>{@code setVisible(false)}:  Hides the dialog and then
		''' returns on {@code setVisible(true)} if it is currently blocked.
		''' <li>It is OK to call this method from the event dispatching
		''' thread because the toolkit ensures that other events are
		''' not blocked while this method is blocked.
		''' </ul> </param>
		''' <seealso cref= java.awt.Window#setVisible </seealso>
		''' <seealso cref= java.awt.Window#dispose </seealso>
		''' <seealso cref= java.awt.Component#isDisplayable </seealso>
		''' <seealso cref= java.awt.Component#validate </seealso>
		''' <seealso cref= java.awt.Dialog#isModal </seealso>
		Public Overrides Property visible As Boolean
			Set(  b As Boolean)
				MyBase.visible = b
			End Set
		End Property

	   ''' <summary>
	   ''' Makes the {@code Dialog} visible. If the dialog and/or its owner
	   ''' are not yet displayable, both are made displayable.  The
	   ''' dialog will be validated prior to being made visible.
	   ''' If the dialog is already visible, this will bring the dialog
	   ''' to the front.
	   ''' <p>
	   ''' If the dialog is modal and is not already visible, this call
	   ''' will not return until the dialog is hidden by calling hide or
	   ''' dispose. It is permissible to show modal dialogs from the event
	   ''' dispatching thread because the toolkit will ensure that another
	   ''' event pump runs while the one which invoked this method is blocked. </summary>
	   ''' <seealso cref= Component#hide </seealso>
	   ''' <seealso cref= Component#isDisplayable </seealso>
	   ''' <seealso cref= Component#validate </seealso>
	   ''' <seealso cref= #isModal </seealso>
	   ''' <seealso cref= Window#setVisible(boolean) </seealso>
	   ''' @deprecated As of JDK version 1.5, replaced by
	   ''' <seealso cref="#setVisible(boolean) setVisible(boolean)"/>. 
		<Obsolete("As of JDK version 1.5, replaced by")> _
		Public Overrides Sub show()
			If Not initialized Then Throw New IllegalStateException("The dialog component " & "has not been initialized properly")

			beforeFirstShow = False
			If Not modal Then
				conditionalShow(Nothing, Nothing)
			Else
				Dim showAppContext As sun.awt.AppContext = sun.awt.AppContext.appContext

				Dim time As New java.util.concurrent.atomic.AtomicLong
				Dim predictedFocusOwner As Component = Nothing
				Try
					predictedFocusOwner = mostRecentFocusOwner
					If conditionalShow(predictedFocusOwner, time) Then
						modalFilter = ModalEventFilter.createFilterForDialog(Me)
						Dim cond As Conditional = New ConditionalAnonymousInnerClassHelper

						' if this dialog is toolkit-modal, the filter should be added
						' to all EDTs (for all AppContexts)
						If modalityType = ModalityType.TOOLKIT_MODAL Then
							Dim it As IEnumerator(Of sun.awt.AppContext) = sun.awt.AppContext.appContexts.GetEnumerator()
							Do While it.MoveNext()
								Dim appContext As sun.awt.AppContext = it.Current
								If appContext Is showAppContext Then Continue Do
								Dim eventQueue_Renamed As EventQueue = CType(appContext.get(sun.awt.AppContext.EVENT_QUEUE_KEY), EventQueue)
								' it may occur that EDT for appContext hasn't been started yet, so
								' we post an empty invocation event to trigger EDT initialization
								Dim createEDT As Runnable = New RunnableAnonymousInnerClassHelper
								eventQueue_Renamed.postEvent(New InvocationEvent(Me, createEDT))
								Dim edt As EventDispatchThread = eventQueue_Renamed.dispatchThread
								edt.addEventFilter(modalFilter)
							Loop
						End If

						modalityPushed()
						Try
							Dim eventQueue_Renamed As EventQueue = java.security.AccessController.doPrivileged(New PrivilegedActionAnonymousInnerClassHelper(Of T)
							secondaryLoop = eventQueue_Renamed.createSecondaryLoop(cond, modalFilter, 0)
							If Not secondaryLoop.enter() Then secondaryLoop = Nothing
						Finally
							modalityPopped()
						End Try

						' if this dialog is toolkit-modal, its filter must be removed
						' from all EDTs (for all AppContexts)
						If modalityType = ModalityType.TOOLKIT_MODAL Then
							Dim it As IEnumerator(Of sun.awt.AppContext) = sun.awt.AppContext.appContexts.GetEnumerator()
							Do While it.MoveNext()
								Dim appContext As sun.awt.AppContext = it.Current
								If appContext Is showAppContext Then Continue Do
								Dim eventQueue_Renamed As EventQueue = CType(appContext.get(sun.awt.AppContext.EVENT_QUEUE_KEY), EventQueue)
								Dim edt As EventDispatchThread = eventQueue_Renamed.dispatchThread
								edt.removeEventFilter(modalFilter)
							Loop
						End If

						If windowClosingException IsNot Nothing Then
							windowClosingException.fillInStackTrace()
							Throw windowClosingException
						End If
					End If
				Finally
					If predictedFocusOwner IsNot Nothing Then KeyboardFocusManager.currentKeyboardFocusManager.dequeueKeyEvents(time.get(), predictedFocusOwner)
				End Try
			End If
		End Sub

		Private Class ConditionalAnonymousInnerClassHelper
			Implements Conditional

			Public Overrides Function evaluate() As Boolean Implements Conditional.evaluate
				Return outerInstance.windowClosingException Is Nothing
			End Function
		End Class

		Private Class RunnableAnonymousInnerClassHelper
			Implements Runnable

			Public Overridable Sub run() Implements Runnable.run
			End Sub
		End Class

		Private Class PrivilegedActionAnonymousInnerClassHelper(Of T)
			Implements java.security.PrivilegedAction(Of T)

			Public Overridable Function run() As EventQueue
				Return Toolkit.defaultToolkit.systemEventQueue
			End Function
		End Class

		Friend Sub modalityPushed()
			Dim tk As Toolkit = Toolkit.defaultToolkit
			If TypeOf tk Is sun.awt.SunToolkit Then
				Dim stk As sun.awt.SunToolkit = CType(tk, sun.awt.SunToolkit)
				stk.notifyModalityPushed(Me)
			End If
		End Sub

		Friend Sub modalityPopped()
			Dim tk As Toolkit = Toolkit.defaultToolkit
			If TypeOf tk Is sun.awt.SunToolkit Then
				Dim stk As sun.awt.SunToolkit = CType(tk, sun.awt.SunToolkit)
				stk.notifyModalityPopped(Me)
			End If
		End Sub

		Friend Overridable Sub interruptBlocking()
			If modal Then
				disposeImpl()
			ElseIf windowClosingException IsNot Nothing Then
				windowClosingException.fillInStackTrace()
				Console.WriteLine(windowClosingException.ToString())
				Console.Write(windowClosingException.StackTrace)
				windowClosingException = Nothing
			End If
		End Sub

		Private Sub hideAndDisposePreHandler()
			isInHide = True
			SyncLock treeLock
				If secondaryLoop IsNot Nothing Then
					modalHide()
					' dialog can be shown and then disposed before its
					' modal filter is created
					If modalFilter IsNot Nothing Then modalFilter.disable()
					modalDialogs.remove(Me)
				End If
			End SyncLock
		End Sub
		Private Sub hideAndDisposeHandler()
			If secondaryLoop IsNot Nothing Then
				secondaryLoop.exit()
				secondaryLoop = Nothing
			End If
			isInHide = False
		End Sub

		''' <summary>
		''' Hides the Dialog and then causes {@code show} to return if it is currently
		''' blocked. </summary>
		''' <seealso cref= Window#show </seealso>
		''' <seealso cref= Window#dispose </seealso>
		''' <seealso cref= Window#setVisible(boolean) </seealso>
		''' @deprecated As of JDK version 1.5, replaced by
		''' <seealso cref="#setVisible(boolean) setVisible(boolean)"/>. 
		<Obsolete("As of JDK version 1.5, replaced by")> _
		Public Overrides Sub hide()
			hideAndDisposePreHandler()
			MyBase.hide()
			' fix for 5048370: if hide() is called from super.doDispose(), then
			' hideAndDisposeHandler() should not be called here as it will be called
			' at the end of doDispose()
			If Not isInDispose Then hideAndDisposeHandler()
		End Sub

		''' <summary>
		''' Disposes the Dialog and then causes show() to return if it is currently
		''' blocked.
		''' </summary>
		Friend Overrides Sub doDispose()
			' fix for 5048370: set isInDispose flag to true to prevent calling
			' to hideAndDisposeHandler() from hide()
			isInDispose = True
			MyBase.doDispose()
			hideAndDisposeHandler()
			isInDispose = False
		End Sub

		''' <summary>
		''' {@inheritDoc}
		''' <p>
		''' If this dialog is modal and blocks some windows, then all of them are
		''' also sent to the back to keep them below the blocking dialog.
		''' </summary>
		''' <seealso cref= java.awt.Window#toBack </seealso>
		Public Overrides Sub toBack()
			MyBase.toBack()
			If visible Then
				SyncLock treeLock
					For Each w As Window In blockedWindows
						w.toBack_NoClientCode()
					Next w
				End SyncLock
			End If
		End Sub

		''' <summary>
		''' Indicates whether this dialog is resizable by the user.
		''' By default, all dialogs are initially resizable. </summary>
		''' <returns>    <code>true</code> if the user can resize the dialog;
		'''            <code>false</code> otherwise. </returns>
		''' <seealso cref=       java.awt.Dialog#setResizable </seealso>
		Public Overridable Property resizable As Boolean
			Get
				Return resizable
			End Get
			Set(  resizable As Boolean)
				Dim testvalid As Boolean = False
    
				SyncLock Me
					Me.resizable = resizable
					Dim peer_Renamed As java.awt.peer.DialogPeer = CType(Me.peer, java.awt.peer.DialogPeer)
					If peer_Renamed IsNot Nothing Then
						peer_Renamed.resizable = resizable
						testvalid = True
					End If
				End SyncLock
    
				' On some platforms, changing the resizable state affects
				' the insets of the Dialog. If we could, we'd call invalidate()
				' from the peer, but we need to guarantee that we're not holding
				' the Dialog lock when we call invalidate().
				If testvalid Then invalidateIfValid()
			End Set
		End Property



		''' <summary>
		''' Disables or enables decorations for this dialog.
		''' <p>
		''' This method can only be called while the dialog is not displayable. To
		''' make this dialog decorated, it must be opaque and have the default shape,
		''' otherwise the {@code IllegalComponentStateException} will be thrown.
		''' Refer to <seealso cref="Window#setShape"/>, <seealso cref="Window#setOpacity"/> and {@link
		''' Window#setBackground} for details
		''' </summary>
		''' <param name="undecorated"> {@code true} if no dialog decorations are to be
		'''         enabled; {@code false} if dialog decorations are to be enabled
		''' </param>
		''' <exception cref="IllegalComponentStateException"> if the dialog is displayable </exception>
		''' <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		'''      {@code false}, and this dialog does not have the default shape </exception>
		''' <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		'''      {@code false}, and this dialog opacity is less than {@code 1.0f} </exception>
		''' <exception cref="IllegalComponentStateException"> if {@code undecorated} is
		'''      {@code false}, and the alpha value of this dialog background
		'''      color is less than {@code 1.0f}
		''' </exception>
		''' <seealso cref=    #isUndecorated </seealso>
		''' <seealso cref=    Component#isDisplayable </seealso>
		''' <seealso cref=    Window#getShape </seealso>
		''' <seealso cref=    Window#getOpacity </seealso>
		''' <seealso cref=    Window#getBackground
		''' 
		''' @since 1.4 </seealso>
		Public Overridable Property undecorated As Boolean
			Set(  undecorated As Boolean)
				' Make sure we don't run in the middle of peer creation.
				SyncLock treeLock
					If displayable Then Throw New IllegalComponentStateException("The dialog is displayable.")
					If Not undecorated Then
						If opacity < 1.0f Then Throw New IllegalComponentStateException("The dialog is not opaque")
						If shape IsNot Nothing Then Throw New IllegalComponentStateException("The dialog does not have a default shape")
						Dim bg As Color = background
						If (bg IsNot Nothing) AndAlso (bg.alpha < 255) Then Throw New IllegalComponentStateException("The dialog background color is not opaque")
					End If
					Me.undecorated = undecorated
				End SyncLock
			End Set
			Get
				Return undecorated
			End Get
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property opacity As Single
			Set(  opacity As Single)
				SyncLock treeLock
					If (opacity < 1.0f) AndAlso (Not undecorated) Then Throw New IllegalComponentStateException("The dialog is decorated")
					MyBase.opacity = opacity
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property shape As Shape
			Set(  shape As Shape)
				SyncLock treeLock
					If (shape IsNot Nothing) AndAlso (Not undecorated) Then Throw New IllegalComponentStateException("The dialog is decorated")
					MyBase.shape = shape
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' {@inheritDoc}
		''' </summary>
		Public Overrides Property background As Color
			Set(  bgColor As Color)
				SyncLock treeLock
					If (bgColor IsNot Nothing) AndAlso (bgColor.alpha < 255) AndAlso (Not undecorated) Then Throw New IllegalComponentStateException("The dialog is decorated")
					MyBase.background = bgColor
				End SyncLock
			End Set
		End Property

		''' <summary>
		''' Returns a string representing the state of this dialog. This
		''' method is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not be
		''' <code>null</code>.
		''' </summary>
		''' <returns>    the parameter string of this dialog window. </returns>
		Protected Friend Overrides Function paramString() As String
			Dim str As String = MyBase.paramString() & "," & modalityType
			If title IsNot Nothing Then str &= ",title=" & title
			Return str
		End Function

		''' <summary>
		''' Initialize JNI field and method IDs
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Replace 'unknown' with the appropriate dll name:
		<DllImport("unknown")> _
		Private Shared Sub initIDs()
		End Sub

	'    
	'     * --- Modality support ---
	'     *
	'     

	'    
	'     * This method is called only for modal dialogs.
	'     *
	'     * Goes through the list of all visible top-level windows and
	'     * divide them into three distinct groups: blockers of this dialog,
	'     * blocked by this dialog and all others. Then blocks this dialog
	'     * by first met dialog from the first group (if any) and blocks all
	'     * the windows from the second group.
	'     
		Friend Overridable Sub modalShow()
			' find all the dialogs that block this one
			Dim blockers As New sun.awt.util.IdentityArrayList(Of Dialog)
			For Each d As Dialog In modalDialogs
				If d.shouldBlock(Me) Then
					Dim w As Window = d
					Do While (w IsNot Nothing) AndAlso (w IsNot Me)
						w = w.owner_NoClientCode
					Loop
					If (w Is Me) OrElse (Not shouldBlock(d)) OrElse (modalityType.CompareTo(d.modalityType) < 0) Then blockers.add(d)
				End If
			Next d

			' add all blockers' blockers to blockers :)
			For i As Integer = 0 To blockers.size() - 1
				Dim blocker As Dialog = blockers.get(i)
				If blocker.modalBlocked Then
					Dim blockerBlocker As Dialog = blocker.modalBlocker
					If Not blockers.contains(blockerBlocker) Then blockers.add(i + 1, blockerBlocker)
				End If
			Next i

			If blockers.size() > 0 Then blockers.get(0).blockWindow(Me)

			' find all windows from blockers' hierarchies
			Dim blockersHierarchies As New sun.awt.util.IdentityArrayList(Of Window)(blockers)
			Dim k As Integer = 0
			Do While k < blockersHierarchies.size()
				Dim w As Window = blockersHierarchies.get(k)
				Dim ownedWindows_Renamed As Window() = w.ownedWindows_NoClientCode
				For Each win As Window In ownedWindows_Renamed
					blockersHierarchies.add(win)
				Next win
				k += 1
			Loop

			Dim toBlock As IList(Of Window) = New sun.awt.util.IdentityLinkedList(Of Window)
			' block all windows from scope of blocking except from blockers' hierarchies
			Dim unblockedWindows As sun.awt.util.IdentityArrayList(Of Window) = Window.allUnblockedWindows
			For Each w As Window In unblockedWindows
				If shouldBlock(w) AndAlso (Not blockersHierarchies.contains(w)) Then
					If (TypeOf w Is Dialog) AndAlso CType(w, Dialog).modal_NoClientCode Then
						Dim wd As Dialog = CType(w, Dialog)
						If wd.shouldBlock(Me) AndAlso (modalDialogs.IndexOf(wd) > modalDialogs.IndexOf(Me)) Then Continue For
					End If
					toBlock.Add(w)
				End If
			Next w
			blockWindows(toBlock)

			If Not modalBlocked Then updateChildrenBlocking()
		End Sub

	'    
	'     * This method is called only for modal dialogs.
	'     *
	'     * Unblocks all the windows blocked by this modal dialog. After
	'     * each of them has been unblocked, it is checked to be blocked by
	'     * any other modal dialogs.
	'     
		Friend Overridable Sub modalHide()
			' we should unblock all the windows first...
			Dim save As New sun.awt.util.IdentityArrayList(Of Window)
			Dim blockedWindowsCount As Integer = blockedWindows.size()
			For i As Integer = 0 To blockedWindowsCount - 1
				Dim w As Window = blockedWindows.get(0)
				save.add(w)
				unblockWindow(w) ' also removes w from blockedWindows
			Next i
			' ... and only after that check if they should be blocked
			' by another dialogs
			For i As Integer = 0 To blockedWindowsCount - 1
				Dim w As Window = save.get(i)
				If (TypeOf w Is Dialog) AndAlso CType(w, Dialog).modal_NoClientCode Then
					Dim d As Dialog = CType(w, Dialog)
					d.modalShow()
				Else
					checkShouldBeBlocked(w)
				End If
			Next i
		End Sub

	'    
	'     * Returns whether the given top-level window should be blocked by
	'     * this dialog. Note, that the given window can be also a modal dialog
	'     * and it should block this dialog, but this method do not take such
	'     * situations into consideration (such checks are performed in the
	'     * modalShow() and modalHide() methods).
	'     *
	'     * This method should be called on the getTreeLock() lock.
	'     
		Friend Overridable Function shouldBlock(  w As Window) As Boolean
			If (Not visible_NoClientCode) OrElse ((Not w.visible_NoClientCode) AndAlso (Not w.isInShow)) OrElse isInHide OrElse (w Is Me) OrElse (Not modal_NoClientCode) Then Return False
			If (TypeOf w Is Dialog) AndAlso CType(w, Dialog).isInHide Then Return False
			' check if w is from children hierarchy
			' fix for 6271546: we should also take into consideration child hierarchies
			' of this dialog's blockers
			Dim blockerToCheck As Window = Me
			Do While blockerToCheck IsNot Nothing
				Dim c As Component = w
				Do While (c IsNot Nothing) AndAlso (c IsNot blockerToCheck)
					c = c.parent_NoClientCode
				Loop
				If c Is blockerToCheck Then Return False
				blockerToCheck = blockerToCheck.modalBlocker
			Loop
			Select Case modalityType
				Case ModalityType.MODELESS
					Return False
				Case ModalityType.DOCUMENT_MODAL
					If w.isModalExcluded(ModalExclusionType.APPLICATION_EXCLUDE) Then
						' application- and toolkit-excluded windows are not blocked by
						' document-modal dialogs from outside their children hierarchy
						Dim c As Component = Me
						Do While (c IsNot Nothing) AndAlso (c IsNot w)
							c = c.parent_NoClientCode
						Loop
						Return c Is w
					Else
						Return documentRoot Is w.documentRoot
					End If
				Case ModalityType.APPLICATION_MODAL
					Return (Not w.isModalExcluded(ModalExclusionType.APPLICATION_EXCLUDE)) AndAlso (appContext Is w.appContext)
				Case ModalityType.TOOLKIT_MODAL
					Return Not w.isModalExcluded(ModalExclusionType.TOOLKIT_EXCLUDE)
			End Select

			Return False
		End Function

	'    
	'     * Adds the given top-level window to the list of blocked
	'     * windows for this dialog and marks it as modal blocked.
	'     * If the window is already blocked by some modal dialog,
	'     * does nothing.
	'     
		Friend Overridable Sub blockWindow(  w As Window)
			If Not w.modalBlocked Then
				w.modalBlockedked(Me, True, True)
				blockedWindows.add(w)
			End If
		End Sub

		Friend Overridable Sub blockWindows(  toBlock As IList(Of Window))
			Dim dpeer As java.awt.peer.DialogPeer = CType(peer, java.awt.peer.DialogPeer)
			If dpeer Is Nothing Then Return
			Dim it As IEnumerator(Of Window) = toBlock.GetEnumerator()
			Do While it.MoveNext()
				Dim w As Window = it.Current
				If Not w.modalBlocked Then
					w.modalBlockedked(Me, True, False)
				Else
					it.remove()
				End If
			Loop
			dpeer.blockWindows(toBlock)
			blockedWindows.addAll(toBlock)
		End Sub

	'    
	'     * Removes the given top-level window from the list of blocked
	'     * windows for this dialog and marks it as unblocked. If the
	'     * window is not modal blocked, does nothing.
	'     
		Friend Overridable Sub unblockWindow(  w As Window)
			If w.modalBlocked AndAlso blockedWindows.contains(w) Then
				blockedWindows.remove(w)
				w.modalBlockedked(Me, False, True)
			End If
		End Sub

	'    
	'     * Checks if any other modal dialog D blocks the given window.
	'     * If such D exists, mark the window as blocked by D.
	'     
		Friend Shared Sub checkShouldBeBlocked(  w As Window)
			SyncLock w.treeLock
				For i As Integer = 0 To modalDialogs.size() - 1
					Dim modalDialog As Dialog = modalDialogs.get(i)
					If modalDialog.shouldBlock(w) Then
						modalDialog.blockWindow(w)
						Exit For
					End If
				Next i
			End SyncLock
		End Sub

		Private Sub checkModalityPermission(  mt As ModalityType)
			If mt = ModalityType.TOOLKIT_MODAL Then
				Dim sm As SecurityManager = System.securityManager
				If sm IsNot Nothing Then sm.checkPermission(sun.security.util.SecurityConstants.AWT.TOOLKIT_MODALITY_PERMISSION)
			End If
		End Sub

		Private Sub readObject(  s As java.io.ObjectInputStream)
			GraphicsEnvironment.checkHeadless()

			Dim fields As java.io.ObjectInputStream.GetField = s.readFields()

			Dim localModalityType As ModalityType = CType(fields.get("modalityType", Nothing), ModalityType)

			Try
				checkModalityPermission(localModalityType)
			Catch ace As java.security.AccessControlException
				localModalityType = DEFAULT_MODALITY_TYPE
			End Try

			' in 1.5 or earlier modalityType was absent, so use "modal" instead
			If localModalityType Is Nothing Then
				Me.modal = fields.get("modal", False)
				modal = modal
			Else
				Me.modalityType = localModalityType
			End If

			Me.resizable = fields.get("resizable", True)
			Me.undecorated = fields.get("undecorated", False)
			Me.title = CStr(fields.get("title", ""))

			blockedWindows = New sun.awt.util.IdentityArrayList(Of )

			sun.awt.SunToolkit.checkAndSetPolicy(Me)

			initialized = True

		End Sub

	'    
	'     * --- Accessibility Support ---
	'     *
	'     

		''' <summary>
		''' Gets the AccessibleContext associated with this Dialog.
		''' For dialogs, the AccessibleContext takes the form of an
		''' AccessibleAWTDialog.
		''' A new AccessibleAWTDialog instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleAWTDialog that serves as the
		'''         AccessibleContext of this Dialog
		''' @since 1.3 </returns>
		Public  Overrides ReadOnly Property  accessibleContext As AccessibleContext
			Get
				If accessibleContext Is Nothing Then accessibleContext = New AccessibleAWTDialog(Me)
				Return accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' <code>Dialog</code> class.  It provides an implementation of the
		''' Java Accessibility API appropriate to dialog user-interface elements.
		''' @since 1.3
		''' </summary>
		Protected Friend Class AccessibleAWTDialog
			Inherits AccessibleAWTWindow

			Private ReadOnly outerInstance As Dialog

			Public Sub New(  outerInstance As Dialog)
				Me.outerInstance = outerInstance
			End Sub

	'        
	'         * JDK 1.3 serialVersionUID
	'         
			Private Const serialVersionUID As Long = 4837230331833941201L

			''' <summary>
			''' Get the role of this object.
			''' </summary>
			''' <returns> an instance of AccessibleRole describing the role of the
			''' object </returns>
			''' <seealso cref= AccessibleRole </seealso>
			Public Overridable Property accessibleRole As AccessibleRole
				Get
					Return AccessibleRole.DIALOG
				End Get
			End Property

			''' <summary>
			''' Get the state of this object.
			''' </summary>
			''' <returns> an instance of AccessibleStateSet containing the current
			''' state set of the object </returns>
			''' <seealso cref= AccessibleState </seealso>
			Public Overridable Property accessibleStateSet As AccessibleStateSet
				Get
					Dim states As AccessibleStateSet = MyBase.accessibleStateSet
					If outerInstance.focusOwner IsNot Nothing Then states.add(AccessibleState.ACTIVE)
					If outerInstance.modal Then states.add(AccessibleState.MODAL)
					If outerInstance.resizable Then states.add(AccessibleState.RESIZABLE)
					Return states
				End Get
			End Property

		End Class ' inner class AccessibleAWTDialog
	End Class

End Namespace