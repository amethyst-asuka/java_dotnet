Imports System.Text
Imports javax.accessibility

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
	''' The main class for creating a dialog window. You can use this class
	''' to create a custom dialog, or invoke the many class methods
	''' in <seealso cref="JOptionPane"/> to create a variety of standard dialogs.
	''' For information about creating dialogs, see
	''' <em>The Java Tutorial</em> section
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/dialog.html">How
	''' to Make Dialogs</a>.
	''' 
	''' <p>
	''' 
	''' The {@code JDialog} component contains a {@code JRootPane}
	''' as its only child.
	''' The {@code contentPane} should be the parent of any children of the
	''' {@code JDialog}.
	''' As a convenience, the {@code add}, {@code remove}, and {@code setLayout}
	''' methods of this class are overridden, so that they delegate calls
	''' to the corresponding methods of the {@code ContentPane}.
	''' For example, you can add a child component to a dialog as follows:
	''' <pre>
	'''       dialog.add(child);
	''' </pre>
	''' And the child will be added to the contentPane.
	''' The {@code contentPane} is always non-{@code null}.
	''' Attempting to set it to {@code null} generates an exception.
	''' The default {@code contentPane} has a {@code BorderLayout}
	''' manager set on it.
	''' Refer to <seealso cref="javax.swing.RootPaneContainer"/>
	''' for details on adding, removing and setting the {@code LayoutManager}
	''' of a {@code JDialog}.
	''' <p>
	''' Please see the {@code JRootPane} documentation for a complete
	''' description of the {@code contentPane}, {@code glassPane},
	''' and {@code layeredPane} components.
	''' <p>
	''' In a multi-screen environment, you can create a {@code JDialog}
	''' on a different screen device than its owner.  See <seealso cref="java.awt.Frame"/> for
	''' more information.
	''' <p>
	''' <strong>Warning:</strong> Swing is not thread safe. For more
	''' information see <a
	''' href="package-summary.html#threading">Swing's Threading
	''' Policy</a>.
	''' <p>
	''' <strong>Warning:</strong>
	''' Serialized objects of this class will not be compatible with
	''' future Swing releases. The current serialization support is
	''' appropriate for short term storage or RMI between applications running
	''' the same version of Swing.  As of 1.4, support for long term storage
	''' of all JavaBeans&trade;
	''' has been added to the {@code java.beans} package.
	''' Please see <seealso cref="java.beans.XMLEncoder"/>.
	''' </summary>
	''' <seealso cref= JOptionPane </seealso>
	''' <seealso cref= JRootPane </seealso>
	''' <seealso cref= javax.swing.RootPaneContainer
	''' 
	''' @beaninfo
	'''      attribute: isContainer true
	'''      attribute: containerDelegate getContentPane
	'''    description: A toplevel window for creating dialog boxes.
	''' 
	''' @author David Kloba
	''' @author James Gosling
	''' @author Scott Violet </seealso>
	Public Class JDialog
		Inherits Dialog
		Implements WindowConstants, Accessible, RootPaneContainer, TransferHandler.HasGetTransferHandler

		''' <summary>
		''' Key into the AppContext, used to check if should provide decorations
		''' by default.
		''' </summary>
		Private Shared ReadOnly defaultLookAndFeelDecoratedKey As Object = New StringBuilder("JDialog.defaultLookAndFeelDecorated")

		Private defaultCloseOperation As Integer = HIDE_ON_CLOSE

		''' <seealso cref= #getRootPane </seealso>
		''' <seealso cref= #setRootPane </seealso>
		Protected Friend rootPane As JRootPane

		''' <summary>
		''' If true then calls to {@code add} and {@code setLayout}
		''' will be forwarded to the {@code contentPane}. This is initially
		''' false, but is set to true when the {@code JDialog} is constructed.
		''' </summary>
		''' <seealso cref= #isRootPaneCheckingEnabled </seealso>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend rootPaneCheckingEnabled As Boolean = False

		''' <summary>
		''' The {@code TransferHandler} for this dialog.
		''' </summary>
		Private transferHandler As TransferHandler

		''' <summary>
		''' Creates a modeless dialog without a title and without a specified
		''' {@code Frame} owner.  A shared, hidden frame will be
		''' set as the owner of the dialog.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' <p>
		''' NOTE: This constructor does not allow you to create an unowned
		''' {@code JDialog}. To create an unowned {@code JDialog}
		''' you must use either the {@code JDialog(Window)} or
		''' {@code JDialog(Dialog)} constructor with an argument of
		''' {@code null}.
		''' </summary>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New()
			Me.New(CType(Nothing, Frame), False)
		End Sub

		''' <summary>
		''' Creates a modeless dialog with the specified {@code Frame}
		''' as its owner and an empty title. If {@code owner}
		''' is {@code null}, a shared, hidden frame will be set as the
		''' owner of the dialog.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' <p>
		''' NOTE: This constructor does not allow you to create an unowned
		''' {@code JDialog}. To create an unowned {@code JDialog}
		''' you must use either the {@code JDialog(Window)} or
		''' {@code JDialog(Dialog)} constructor with an argument of
		''' {@code null}.
		''' </summary>
		''' <param name="owner"> the {@code Frame} from which the dialog is displayed </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Frame)
			Me.New(owner, False)
		End Sub

		''' <summary>
		''' Creates a dialog with an empty title and the specified modality and
		''' {@code Frame} as its owner. If {@code owner} is {@code null},
		''' a shared, hidden frame will be set as the owner of the dialog.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' <p>
		''' NOTE: This constructor does not allow you to create an unowned
		''' {@code JDialog}. To create an unowned {@code JDialog}
		''' you must use either the {@code JDialog(Window)} or
		''' {@code JDialog(Dialog)} constructor with an argument of
		''' {@code null}.
		''' </summary>
		''' <param name="owner"> the {@code Frame} from which the dialog is displayed </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If {@code true}, the modality type property is set to
		'''     {@code DEFAULT_MODALITY_TYPE}, otherwise the dialog is modeless. </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Frame, ByVal modal As Boolean)
			Me.New(owner, "", modal)
		End Sub

		''' <summary>
		''' Creates a modeless dialog with the specified title and
		''' with the specified owner frame.  If {@code owner}
		''' is {@code null}, a shared, hidden frame will be set as the
		''' owner of the dialog.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' <p>
		''' NOTE: This constructor does not allow you to create an unowned
		''' {@code JDialog}. To create an unowned {@code JDialog}
		''' you must use either the {@code JDialog(Window)} or
		''' {@code JDialog(Dialog)} constructor with an argument of
		''' {@code null}.
		''' </summary>
		''' <param name="owner"> the {@code Frame} from which the dialog is displayed </param>
		''' <param name="title">  the {@code String} to display in the dialog's
		'''                  title bar </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Frame, ByVal title As String)
			Me.New(owner, title, False)
		End Sub

		''' <summary>
		''' Creates a dialog with the specified title, owner {@code Frame}
		''' and modality. If {@code owner} is {@code null},
		''' a shared, hidden frame will be set as the owner of this dialog.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' <p>
		''' NOTE: Any popup components ({@code JComboBox},
		''' {@code JPopupMenu}, {@code JMenuBar})
		''' created within a modal dialog will be forced to be lightweight.
		''' <p>
		''' NOTE: This constructor does not allow you to create an unowned
		''' {@code JDialog}. To create an unowned {@code JDialog}
		''' you must use either the {@code JDialog(Window)} or
		''' {@code JDialog(Dialog)} constructor with an argument of
		''' {@code null}.
		''' </summary>
		''' <param name="owner"> the {@code Frame} from which the dialog is displayed </param>
		''' <param name="title">  the {@code String} to display in the dialog's
		'''     title bar </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If {@code true}, the modality type property is set to
		'''     {@code DEFAULT_MODALITY_TYPE} otherwise the dialog is modeless </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}.
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Frame, ByVal title As String, ByVal modal As Boolean)
			MyBase.New(If(owner Is Nothing, SwingUtilities.sharedOwnerFrame, owner), title, modal)
			If owner Is Nothing Then
				Dim ownerShutdownListener As WindowListener = SwingUtilities.sharedOwnerFrameShutdownListener
				addWindowListener(ownerShutdownListener)
			End If
			dialogInit()
		End Sub

		''' <summary>
		''' Creates a dialog with the specified title,
		''' owner {@code Frame}, modality and {@code GraphicsConfiguration}.
		''' If {@code owner} is {@code null},
		''' a shared, hidden frame will be set as the owner of this dialog.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' <p>
		''' NOTE: Any popup components ({@code JComboBox},
		''' {@code JPopupMenu}, {@code JMenuBar})
		''' created within a modal dialog will be forced to be lightweight.
		''' <p>
		''' NOTE: This constructor does not allow you to create an unowned
		''' {@code JDialog}. To create an unowned {@code JDialog}
		''' you must use either the {@code JDialog(Window)} or
		''' {@code JDialog(Dialog)} constructor with an argument of
		''' {@code null}.
		''' </summary>
		''' <param name="owner"> the {@code Frame} from which the dialog is displayed </param>
		''' <param name="title">  the {@code String} to display in the dialog's
		'''     title bar </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If {@code true}, the modality type property is set to
		'''     {@code DEFAULT_MODALITY_TYPE}, otherwise the dialog is modeless. </param>
		''' <param name="gc"> the {@code GraphicsConfiguration} of the target screen device;
		'''     if {@code null}, the default system {@code GraphicsConfiguration}
		'''     is assumed </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' @since 1.4 </seealso>
		Public Sub New(ByVal owner As Frame, ByVal title As String, ByVal modal As Boolean, ByVal gc As GraphicsConfiguration)
			MyBase.New(If(owner Is Nothing, SwingUtilities.sharedOwnerFrame, owner), title, modal, gc)
			If owner Is Nothing Then
				Dim ownerShutdownListener As WindowListener = SwingUtilities.sharedOwnerFrameShutdownListener
				addWindowListener(ownerShutdownListener)
			End If
			dialogInit()
		End Sub

		''' <summary>
		''' Creates a modeless dialog with the specified {@code Dialog}
		''' as its owner and an empty title.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the owner {@code Dialog} from which the dialog is displayed
		'''     or {@code null} if this dialog has no owner </param>
		''' <exception cref="HeadlessException"> {@code if GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Dialog)
			Me.New(owner, False)
		End Sub

		''' <summary>
		''' Creates a dialog with an empty title and the specified modality and
		''' {@code Dialog} as its owner.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the owner {@code Dialog} from which the dialog is displayed
		'''     or {@code null} if this dialog has no owner </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If {@code true}, the modality type property is set to
		'''     {@code DEFAULT_MODALITY_TYPE}, otherwise the dialog is modeless. </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Dialog, ByVal modal As Boolean)
			Me.New(owner, "", modal)
		End Sub

		''' <summary>
		''' Creates a modeless dialog with the specified title and
		''' with the specified owner dialog.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the owner {@code Dialog} from which the dialog is displayed
		'''     or {@code null} if this dialog has no owner </param>
		''' <param name="title">  the {@code String} to display in the dialog's
		'''                  title bar </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Dialog, ByVal title As String)
			Me.New(owner, title, False)
		End Sub

		''' <summary>
		''' Creates a dialog with the specified title, modality
		''' and the specified owner {@code Dialog}.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the owner {@code Dialog} from which the dialog is displayed
		'''     or {@code null} if this dialog has no owner </param>
		''' <param name="title">  the {@code String} to display in the dialog's
		'''     title bar </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If {@code true}, the modality type property is set to
		'''     {@code DEFAULT_MODALITY_TYPE}, otherwise the dialog is modeless </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale </seealso>
		Public Sub New(ByVal owner As Dialog, ByVal title As String, ByVal modal As Boolean)
			MyBase.New(owner, title, modal)
			dialogInit()
		End Sub

		''' <summary>
		''' Creates a dialog with the specified title, owner {@code Dialog},
		''' modality and {@code GraphicsConfiguration}.
		''' 
		''' <p>
		''' NOTE: Any popup components ({@code JComboBox},
		''' {@code JPopupMenu}, {@code JMenuBar})
		''' created within a modal dialog will be forced to be lightweight.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the owner {@code Dialog} from which the dialog is displayed
		'''     or {@code null} if this dialog has no owner </param>
		''' <param name="title">  the {@code String} to display in the dialog's
		'''     title bar </param>
		''' <param name="modal"> specifies whether dialog blocks user input to other top-level
		'''     windows when shown. If {@code true}, the modality type property is set to
		'''     {@code DEFAULT_MODALITY_TYPE}, otherwise the dialog is modeless </param>
		''' <param name="gc"> the {@code GraphicsConfiguration} of the target screen device;
		'''     if {@code null}, the default system {@code GraphicsConfiguration}
		'''     is assumed </param>
		''' <exception cref="HeadlessException"> if {@code GraphicsEnvironment.isHeadless()}
		'''     returns {@code true}. </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog.ModalityType#MODELESS </seealso>
		''' <seealso cref= java.awt.Dialog#DEFAULT_MODALITY_TYPE </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' @since 1.4 </seealso>
		Public Sub New(ByVal owner As Dialog, ByVal title As String, ByVal modal As Boolean, ByVal gc As GraphicsConfiguration)
			MyBase.New(owner, title, modal, gc)
			dialogInit()
		End Sub

		''' <summary>
		''' Creates a modeless dialog with the specified {@code Window}
		''' as its owner and an empty title.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the {@code Window} from which the dialog is displayed or
		'''     {@code null} if this dialog has no owner
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner} is not an instance of <seealso cref="java.awt.Dialog Dialog"/>
		'''     or <seealso cref="java.awt.Frame Frame"/> </exception>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner}'s {@code GraphicsConfiguration} is not from a screen device </exception>
		''' <exception cref="HeadlessException">
		'''     when {@code GraphicsEnvironment.isHeadless()} returns {@code true}
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(ByVal owner As Window)
			Me.New(owner, Dialog.ModalityType.MODELESS)
		End Sub

		''' <summary>
		''' Creates a dialog with an empty title and the specified modality and
		''' {@code Window} as its owner.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the {@code Window} from which the dialog is displayed or
		'''     {@code null} if this dialog has no owner </param>
		''' <param name="modalityType"> specifies whether dialog blocks input to other
		'''     windows when shown. {@code null} value and unsupported modality
		'''     types are equivalent to {@code MODELESS}
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner} is not an instance of <seealso cref="java.awt.Dialog Dialog"/>
		'''     or <seealso cref="java.awt.Frame Frame"/> </exception>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner}'s {@code GraphicsConfiguration} is not from a screen device </exception>
		''' <exception cref="HeadlessException">
		'''     when {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		''' <exception cref="SecurityException">
		'''     if the calling thread does not have permission to create modal dialogs
		'''     with the given {@code modalityType}
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(ByVal owner As Window, ByVal modalityType As ModalityType)
			Me.New(owner, "", modalityType)
		End Sub

		''' <summary>
		''' Creates a modeless dialog with the specified title and owner
		''' {@code Window}.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the {@code Window} from which the dialog is displayed or
		'''     {@code null} if this dialog has no owner </param>
		''' <param name="title"> the {@code String} to display in the dialog's
		'''     title bar or {@code null} if the dialog has no title
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner} is not an instance of <seealso cref="java.awt.Dialog Dialog"/>
		'''     or <seealso cref="java.awt.Frame Frame"/> </exception>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner}'s {@code GraphicsConfiguration} is not from a screen device </exception>
		''' <exception cref="HeadlessException">
		'''     when {@code GraphicsEnvironment.isHeadless()} returns {@code true}
		''' </exception>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(ByVal owner As Window, ByVal title As String)
			Me.New(owner, title, Dialog.ModalityType.MODELESS)
		End Sub

		''' <summary>
		''' Creates a dialog with the specified title, owner {@code Window} and
		''' modality.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the {@code Window} from which the dialog is displayed or
		'''     {@code null} if this dialog has no owner </param>
		''' <param name="title"> the {@code String} to display in the dialog's
		'''     title bar or {@code null} if the dialog has no title </param>
		''' <param name="modalityType"> specifies whether dialog blocks input to other
		'''     windows when shown. {@code null} value and unsupported modality
		'''     types are equivalent to {@code MODELESS}
		''' </param>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner} is not an instance of <seealso cref="java.awt.Dialog Dialog"/>
		'''     or <seealso cref="java.awt.Frame Frame"/> </exception>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner}'s {@code GraphicsConfiguration} is not from a screen device </exception>
		''' <exception cref="HeadlessException">
		'''     when {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		''' <exception cref="SecurityException">
		'''     if the calling thread does not have permission to create modal dialogs
		'''     with the given {@code modalityType}
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(ByVal owner As Window, ByVal title As String, ByVal modalityType As Dialog.ModalityType)
			MyBase.New(owner, title, modalityType)
			dialogInit()
		End Sub

		''' <summary>
		''' Creates a dialog with the specified title, owner {@code Window},
		''' modality and {@code GraphicsConfiguration}.
		''' <p>
		''' NOTE: Any popup components ({@code JComboBox},
		''' {@code JPopupMenu}, {@code JMenuBar})
		''' created within a modal dialog will be forced to be lightweight.
		''' <p>
		''' This constructor sets the component's locale property to the value
		''' returned by {@code JComponent.getDefaultLocale}.
		''' </summary>
		''' <param name="owner"> the {@code Window} from which the dialog is displayed or
		'''     {@code null} if this dialog has no owner </param>
		''' <param name="title"> the {@code String} to display in the dialog's
		'''     title bar or {@code null} if the dialog has no title </param>
		''' <param name="modalityType"> specifies whether dialog blocks input to other
		'''     windows when shown. {@code null} value and unsupported modality
		'''     types are equivalent to {@code MODELESS} </param>
		''' <param name="gc"> the {@code GraphicsConfiguration} of the target screen device;
		'''     if {@code null}, the default system {@code GraphicsConfiguration}
		'''     is assumed </param>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner} is not an instance of <seealso cref="java.awt.Dialog Dialog"/>
		'''     or <seealso cref="java.awt.Frame Frame"/> </exception>
		''' <exception cref="IllegalArgumentException">
		'''     if the {@code owner}'s {@code GraphicsConfiguration} is not from a screen device </exception>
		''' <exception cref="HeadlessException">
		'''     when {@code GraphicsEnvironment.isHeadless()} returns {@code true} </exception>
		''' <exception cref="SecurityException">
		'''     if the calling thread does not have permission to create modal dialogs
		'''     with the given {@code modalityType}
		''' </exception>
		''' <seealso cref= java.awt.Dialog.ModalityType </seealso>
		''' <seealso cref= java.awt.Dialog#setModal </seealso>
		''' <seealso cref= java.awt.Dialog#setModalityType </seealso>
		''' <seealso cref= java.awt.GraphicsEnvironment#isHeadless </seealso>
		''' <seealso cref= JComponent#getDefaultLocale
		''' 
		''' @since 1.6 </seealso>
		Public Sub New(ByVal owner As Window, ByVal title As String, ByVal modalityType As Dialog.ModalityType, ByVal gc As GraphicsConfiguration)
			MyBase.New(owner, title, modalityType, gc)
			dialogInit()
		End Sub

		''' <summary>
		''' Called by the constructors to init the {@code JDialog} properly.
		''' </summary>
		Protected Friend Overridable Sub dialogInit()
			enableEvents(AWTEvent.KEY_EVENT_MASK Or AWTEvent.WINDOW_EVENT_MASK)
			locale = JComponent.defaultLocale
			rootPane = createRootPane()
			background = UIManager.getColor("control")
			rootPaneCheckingEnabled = True
			If JDialog.defaultLookAndFeelDecorated Then
				Dim supportsWindowDecorations As Boolean = UIManager.lookAndFeel.supportsWindowDecorations
				If supportsWindowDecorations Then
					undecorated = True
					rootPane.windowDecorationStyle = JRootPane.PLAIN_DIALOG
				End If
			End If
			sun.awt.SunToolkit.checkAndSetPolicy(Me)
		End Sub

		''' <summary>
		''' Called by the constructor methods to create the default
		''' {@code rootPane}.
		''' </summary>
		Protected Friend Overridable Function createRootPane() As JRootPane
			Dim rp As New JRootPane
			' NOTE: this uses setOpaque vs LookAndFeel.installProperty as there
			' is NO reason for the RootPane not to be opaque. For painting to
			' work the contentPane must be opaque, therefor the RootPane can
			' also be opaque.
			rp.opaque = True
			Return rp
		End Function

		''' <summary>
		''' Handles window events depending on the state of the
		''' {@code defaultCloseOperation} property.
		''' </summary>
		''' <seealso cref= #setDefaultCloseOperation </seealso>
		Protected Friend Overridable Sub processWindowEvent(ByVal e As WindowEvent)
			MyBase.processWindowEvent(e)

			If e.iD = WindowEvent.WINDOW_CLOSING Then
				Select Case defaultCloseOperation
				  Case HIDE_ON_CLOSE
					 visible = False
				  Case DISPOSE_ON_CLOSE
					 Dispose()
					 Case Else
				End Select
			End If
		End Sub


		''' <summary>
		''' Sets the operation that will happen by default when
		''' the user initiates a "close" on this dialog.
		''' You must specify one of the following choices:
		''' <br><br>
		''' <ul>
		''' <li>{@code DO_NOTHING_ON_CLOSE}
		''' (defined in {@code WindowConstants}):
		''' Don't do anything; require the
		''' program to handle the operation in the {@code windowClosing}
		''' method of a registered {@code WindowListener} object.
		''' 
		''' <li>{@code HIDE_ON_CLOSE}
		''' (defined in {@code WindowConstants}):
		''' Automatically hide the dialog after
		''' invoking any registered {@code WindowListener}
		''' objects.
		''' 
		''' <li>{@code DISPOSE_ON_CLOSE}
		''' (defined in {@code WindowConstants}):
		''' Automatically hide and dispose the
		''' dialog after invoking any registered {@code WindowListener}
		''' objects.
		''' </ul>
		''' <p>
		''' The value is set to {@code HIDE_ON_CLOSE} by default. Changes
		''' to the value of this property cause the firing of a property
		''' change event, with property name "defaultCloseOperation".
		''' <p>
		''' <b>Note</b>: When the last displayable window within the
		''' Java virtual machine (VM) is disposed of, the VM may
		''' terminate.  See <a href="../../java/awt/doc-files/AWTThreadIssues.html">
		''' AWT Threading Issues</a> for more information.
		''' </summary>
		''' <param name="operation"> the operation which should be performed when the
		'''        user closes the dialog </param>
		''' <exception cref="IllegalArgumentException"> if defaultCloseOperation value
		'''         isn't one of the above valid values </exception>
		''' <seealso cref= #addWindowListener </seealso>
		''' <seealso cref= #getDefaultCloseOperation </seealso>
		''' <seealso cref= WindowConstants
		''' 
		''' @beaninfo
		'''   preferred: true
		'''       bound: true
		'''        enum: DO_NOTHING_ON_CLOSE WindowConstants.DO_NOTHING_ON_CLOSE
		'''              HIDE_ON_CLOSE       WindowConstants.HIDE_ON_CLOSE
		'''              DISPOSE_ON_CLOSE    WindowConstants.DISPOSE_ON_CLOSE
		''' description: The dialog's default close operation. </seealso>
		Public Overridable Property defaultCloseOperation As Integer
			Set(ByVal operation As Integer)
				If operation <> DO_NOTHING_ON_CLOSE AndAlso operation <> HIDE_ON_CLOSE AndAlso operation <> DISPOSE_ON_CLOSE Then Throw New System.ArgumentException("defaultCloseOperation must be one of: DO_NOTHING_ON_CLOSE, HIDE_ON_CLOSE, or DISPOSE_ON_CLOSE")
    
				Dim oldValue As Integer = Me.defaultCloseOperation
				Me.defaultCloseOperation = operation
				firePropertyChange("defaultCloseOperation", oldValue, operation)
			End Set
			Get
				Return defaultCloseOperation
			End Get
		End Property


		''' <summary>
		''' Sets the {@code transferHandler} property, which is a mechanism to
		''' support transfer of data into this component. Use {@code null}
		''' if the component does not support data transfer operations.
		''' <p>
		''' If the system property {@code suppressSwingDropSupport} is {@code false}
		''' (the default) and the current drop target on this component is either
		''' {@code null} or not a user-set drop target, this method will change the
		''' drop target as follows: If {@code newHandler} is {@code null} it will
		''' clear the drop target. If not {@code null} it will install a new
		''' {@code DropTarget}.
		''' <p>
		''' Note: When used with {@code JDialog}, {@code TransferHandler} only
		''' provides data import capability, as the data export related methods
		''' are currently typed to {@code JComponent}.
		''' <p>
		''' Please see
		''' <a href="https://docs.oracle.com/javase/tutorial/uiswing/dnd/index.html">
		''' How to Use Drag and Drop and Data Transfer</a>, a section in
		''' <em>The Java Tutorial</em>, for more information.
		''' </summary>
		''' <param name="newHandler"> the new {@code TransferHandler}
		''' </param>
		''' <seealso cref= TransferHandler </seealso>
		''' <seealso cref= #getTransferHandler </seealso>
		''' <seealso cref= java.awt.Component#setDropTarget
		''' @since 1.6
		''' 
		''' @beaninfo
		'''        bound: true
		'''       hidden: true
		'''  description: Mechanism for transfer of data into the component </seealso>
		Public Overridable Property transferHandler As TransferHandler
			Set(ByVal newHandler As TransferHandler)
				Dim oldHandler As TransferHandler = transferHandler
				transferHandler = newHandler
				SwingUtilities.installSwingDropTargetAsNecessary(Me, transferHandler)
				firePropertyChange("transferHandler", oldHandler, newHandler)
			End Set
			Get
				Return transferHandler
			End Get
		End Property


		''' <summary>
		''' Calls {@code paint(g)}.  This method was overridden to
		''' prevent an unnecessary call to clear the background.
		''' </summary>
		''' <param name="g">  the {@code Graphics} context in which to paint </param>
		Public Overridable Sub update(ByVal g As Graphics)
			paint(g)
		End Sub

	   ''' <summary>
	   ''' Sets the menubar for this dialog.
	   ''' </summary>
	   ''' <param name="menu"> the menubar being placed in the dialog
	   ''' </param>
	   ''' <seealso cref= #getJMenuBar
	   ''' 
	   ''' @beaninfo
	   '''      hidden: true
	   ''' description: The menubar for accessing pulldown menus from this dialog. </seealso>
		Public Overridable Property jMenuBar As JMenuBar
			Set(ByVal menu As JMenuBar)
				rootPane.menuBar = menu
			End Set
			Get
				Return rootPane.menuBar
			End Get
		End Property



		''' <summary>
		''' Returns whether calls to {@code add} and
		''' {@code setLayout} are forwarded to the {@code contentPane}.
		''' </summary>
		''' <returns> true if {@code add} and {@code setLayout}
		'''         are forwarded; false otherwise
		''' </returns>
		''' <seealso cref= #addImpl </seealso>
		''' <seealso cref= #setLayout </seealso>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend Overridable Property rootPaneCheckingEnabled As Boolean
			Get
				Return rootPaneCheckingEnabled
			End Get
			Set(ByVal enabled As Boolean)
				rootPaneCheckingEnabled = enabled
			End Set
		End Property



		''' <summary>
		''' Adds the specified child {@code Component}.
		''' This method is overridden to conditionally forward calls to the
		''' {@code contentPane}.
		''' By default, children are added to the {@code contentPane} instead
		''' of the frame, refer to <seealso cref="javax.swing.RootPaneContainer"/> for
		''' details.
		''' </summary>
		''' <param name="comp"> the component to be enhanced </param>
		''' <param name="constraints"> the constraints to be respected </param>
		''' <param name="index"> the index </param>
		''' <exception cref="IllegalArgumentException"> if {@code index} is invalid </exception>
		''' <exception cref="IllegalArgumentException"> if adding the container's parent
		'''                  to itself </exception>
		''' <exception cref="IllegalArgumentException"> if adding a window to a container
		''' </exception>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Protected Friend Overridable Sub addImpl(ByVal comp As Component, ByVal constraints As Object, ByVal index As Integer)
			If rootPaneCheckingEnabled Then
				contentPane.add(comp, constraints, index)
			Else
				MyBase.addImpl(comp, constraints, index)
			End If
		End Sub

		''' <summary>
		''' Removes the specified component from the container. If
		''' {@code comp} is not the {@code rootPane}, this will forward
		''' the call to the {@code contentPane}. This will do nothing if
		''' {@code comp} is not a child of the {@code JDialog} or
		''' {@code contentPane}.
		''' </summary>
		''' <param name="comp"> the component to be removed </param>
		''' <exception cref="NullPointerException"> if {@code comp} is null </exception>
		''' <seealso cref= #add </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Public Overridable Sub remove(ByVal comp As Component)
			If comp Is rootPane Then
				MyBase.remove(comp)
			Else
				contentPane.remove(comp)
			End If
		End Sub


		''' <summary>
		''' Sets the {@code LayoutManager}.
		''' Overridden to conditionally forward the call to the
		''' {@code contentPane}.
		''' Refer to <seealso cref="javax.swing.RootPaneContainer"/> for
		''' more information.
		''' </summary>
		''' <param name="manager"> the {@code LayoutManager} </param>
		''' <seealso cref= #setRootPaneCheckingEnabled </seealso>
		''' <seealso cref= javax.swing.RootPaneContainer </seealso>
		Public Overridable Property layout As LayoutManager
			Set(ByVal manager As LayoutManager)
				If rootPaneCheckingEnabled Then
					contentPane.layout = manager
				Else
					MyBase.layout = manager
				End If
			End Set
		End Property


		''' <summary>
		''' Returns the {@code rootPane} object for this dialog.
		''' </summary>
		''' <seealso cref= #setRootPane </seealso>
		''' <seealso cref= RootPaneContainer#getRootPane </seealso>
		Public Overridable Property rootPane As JRootPane Implements RootPaneContainer.getRootPane
			Get
				Return rootPane
			End Get
			Set(ByVal root As JRootPane)
				If rootPane IsNot Nothing Then remove(rootPane)
				rootPane = root
				If rootPane IsNot Nothing Then
					Dim checkingEnabled As Boolean = rootPaneCheckingEnabled
					Try
						rootPaneCheckingEnabled = False
						add(rootPane, BorderLayout.CENTER)
					Finally
						rootPaneCheckingEnabled = checkingEnabled
					End Try
				End If
			End Set
		End Property




		''' <summary>
		''' Returns the {@code contentPane} object for this dialog.
		''' </summary>
		''' <returns> the {@code contentPane} property
		''' </returns>
		''' <seealso cref= #setContentPane </seealso>
		''' <seealso cref= RootPaneContainer#getContentPane </seealso>
		Public Overridable Property contentPane As Container
			Get
				Return rootPane.contentPane
			End Get
			Set(ByVal contentPane As Container)
				rootPane.contentPane = contentPane
			End Set
		End Property



		''' <summary>
		''' Returns the {@code layeredPane} object for this dialog.
		''' </summary>
		''' <returns> the {@code layeredPane} property
		''' </returns>
		''' <seealso cref= #setLayeredPane </seealso>
		''' <seealso cref= RootPaneContainer#getLayeredPane </seealso>
		Public Overridable Property layeredPane As JLayeredPane Implements RootPaneContainer.getLayeredPane
			Get
				Return rootPane.layeredPane
			End Get
			Set(ByVal layeredPane As JLayeredPane)
				rootPane.layeredPane = layeredPane
			End Set
		End Property


		''' <summary>
		''' Returns the {@code glassPane} object for this dialog.
		''' </summary>
		''' <returns> the {@code glassPane} property
		''' </returns>
		''' <seealso cref= #setGlassPane </seealso>
		''' <seealso cref= RootPaneContainer#getGlassPane </seealso>
		Public Overridable Property glassPane As Component
			Get
				Return rootPane.glassPane
			End Get
			Set(ByVal glassPane As Component)
				rootPane.glassPane = glassPane
			End Set
		End Property


		''' <summary>
		''' {@inheritDoc}
		''' 
		''' @since 1.6
		''' </summary>
		Public Overridable Property graphics As Graphics
			Get
				JComponent.getGraphicsInvoked(Me)
				Return MyBase.graphics
			End Get
		End Property

		''' <summary>
		''' Repaints the specified rectangle of this component within
		''' {@code time} milliseconds.  Refer to {@code RepaintManager}
		''' for details on how the repaint is handled.
		''' </summary>
		''' <param name="time">   maximum time in milliseconds before update </param>
		''' <param name="x">    the <i>x</i> coordinate </param>
		''' <param name="y">    the <i>y</i> coordinate </param>
		''' <param name="width">    the width </param>
		''' <param name="height">   the height </param>
		''' <seealso cref=       RepaintManager
		''' @since     1.6 </seealso>
		Public Overridable Sub repaint(ByVal time As Long, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
			If RepaintManager.HANDLE_TOP_LEVEL_PAINT Then
				RepaintManager.currentManager(Me).addDirtyRegion(Me, x, y, width, height)
			Else
				MyBase.repaint(time, x, y, width, height)
			End If
		End Sub

		''' <summary>
		''' Provides a hint as to whether or not newly created {@code JDialog}s
		''' should have their Window decorations (such as borders, widgets to
		''' close the window, title...) provided by the current look
		''' and feel. If {@code defaultLookAndFeelDecorated} is true,
		''' the current {@code LookAndFeel} supports providing window
		''' decorations, and the current window manager supports undecorated
		''' windows, then newly created {@code JDialog}s will have their
		''' Window decorations provided by the current {@code LookAndFeel}.
		''' Otherwise, newly created {@code JDialog}s will have their
		''' Window decorations provided by the current window manager.
		''' <p>
		''' You can get the same effect on a single JDialog by doing the following:
		''' <pre>
		'''    JDialog dialog = new JDialog();
		'''    dialog.setUndecorated(true);
		'''    dialog.getRootPane().setWindowDecorationStyle(JRootPane.PLAIN_DIALOG);
		''' </pre>
		''' </summary>
		''' <param name="defaultLookAndFeelDecorated"> A hint as to whether or not current
		'''        look and feel should provide window decorations </param>
		''' <seealso cref= javax.swing.LookAndFeel#getSupportsWindowDecorations
		''' @since 1.4 </seealso>
		Public Shared Property defaultLookAndFeelDecorated As Boolean
			Set(ByVal defaultLookAndFeelDecorated As Boolean)
				If defaultLookAndFeelDecorated Then
					SwingUtilities.appContextPut(defaultLookAndFeelDecoratedKey, Boolean.TRUE)
				Else
					SwingUtilities.appContextPut(defaultLookAndFeelDecoratedKey, Boolean.FALSE)
				End If
			End Set
			Get
				Dim ___defaultLookAndFeelDecorated As Boolean? = CBool(SwingUtilities.appContextGet(defaultLookAndFeelDecoratedKey))
				If ___defaultLookAndFeelDecorated Is Nothing Then ___defaultLookAndFeelDecorated = Boolean.FALSE
				Return ___defaultLookAndFeelDecorated
			End Get
		End Property


		''' <summary>
		''' Returns a string representation of this {@code JDialog}.
		''' This method
		''' is intended to be used only for debugging purposes, and the
		''' content and format of the returned string may vary between
		''' implementations. The returned string may be empty but may not
		''' be {@code null}.
		''' </summary>
		''' <returns>  a string representation of this {@code JDialog}. </returns>
		Protected Friend Overridable Function paramString() As String
			Dim defaultCloseOperationString As String
			If defaultCloseOperation = HIDE_ON_CLOSE Then
				defaultCloseOperationString = "HIDE_ON_CLOSE"
			ElseIf defaultCloseOperation = DISPOSE_ON_CLOSE Then
				defaultCloseOperationString = "DISPOSE_ON_CLOSE"
			ElseIf defaultCloseOperation = DO_NOTHING_ON_CLOSE Then
				defaultCloseOperationString = "DO_NOTHING_ON_CLOSE"
			Else
				defaultCloseOperationString = ""
			End If
			Dim rootPaneString As String = (If(rootPane IsNot Nothing, rootPane.ToString(), ""))
			Dim rootPaneCheckingEnabledString As String = (If(rootPaneCheckingEnabled, "true", "false"))

			Return MyBase.paramString() & ",defaultCloseOperation=" & defaultCloseOperationString & ",rootPane=" & rootPaneString & ",rootPaneCheckingEnabled=" & rootPaneCheckingEnabledString
		End Function


	'///////////////
	' Accessibility support
	'//////////////

		Protected Friend ___accessibleContext As AccessibleContext = Nothing

		''' <summary>
		''' Gets the AccessibleContext associated with this JDialog.
		''' For JDialogs, the AccessibleContext takes the form of an
		''' AccessibleJDialog.
		''' A new AccessibleJDialog instance is created if necessary.
		''' </summary>
		''' <returns> an AccessibleJDialog that serves as the
		'''         AccessibleContext of this JDialog </returns>
		Public Overridable Property accessibleContext As AccessibleContext Implements Accessible.getAccessibleContext
			Get
				If ___accessibleContext Is Nothing Then ___accessibleContext = New AccessibleJDialog(Me)
				Return ___accessibleContext
			End Get
		End Property

		''' <summary>
		''' This class implements accessibility support for the
		''' {@code JDialog} class.  It provides an implementation of the
		''' Java Accessibility API appropriate to dialog user-interface
		''' elements.
		''' </summary>
		Protected Friend Class AccessibleJDialog
			Inherits AccessibleAWTDialog

			Private ReadOnly outerInstance As JDialog

			Public Sub New(ByVal outerInstance As JDialog)
				Me.outerInstance = outerInstance
			End Sub


			' AccessibleContext methods
			'
			''' <summary>
			''' Get the accessible name of this object.
			''' </summary>
			''' <returns> the localized name of the object -- can be null if this
			''' object does not have a name </returns>
			Public Overridable Property accessibleName As String
				Get
					If accessibleName IsNot Nothing Then
						Return accessibleName
					Else
						If title Is Nothing Then
							Return MyBase.accessibleName
						Else
							Return title
						End If
					End If
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
    
					If resizable Then states.add(AccessibleState.RESIZABLE)
					If focusOwner IsNot Nothing Then states.add(AccessibleState.ACTIVE)
					If modal Then states.add(AccessibleState.MODAL)
					Return states
				End Get
			End Property

		End Class ' inner class AccessibleJDialog
	End Class

End Namespace