'
' * Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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

Namespace javax.print

	''' <summary>
	''' Services may optionally provide UIs which allow different styles
	''' of interaction in different roles.
	''' One role may be end-user browsing and setting of print options.
	''' Another role may be administering the print service.
	''' <p>
	''' Although the Print Service API does not presently provide standardised
	''' support for administering a print service, monitoring of the print
	''' service is possible and a UI may provide for private update mechanisms.
	''' <p>
	''' The basic design intent is to allow applications to lazily locate and
	''' initialize services only when needed without any API dependencies
	''' except in an environment in which they are used.
	''' <p>
	''' Swing UIs are preferred as they provide a more consistent {@literal L&F}
	''' and can support accessibility APIs.
	''' <p>
	''' Example usage:
	''' <pre>
	'''  ServiceUIFactory factory = printService.getServiceUIFactory();
	'''  if (factory != null) {
	'''      JComponent swingui = (JComponent)factory.getUI(
	'''                                         ServiceUIFactory.MAIN_UIROLE,
	'''                                         ServiceUIFactory.JCOMPONENT_UI);
	'''      if (swingui != null) {
	'''          tabbedpane.add("Custom UI", swingui);
	'''      }
	'''  }
	''' </pre>
	''' </summary>

	Public MustInherit Class ServiceUIFactory

		''' <summary>
		''' Denotes a UI implemented as a Swing component.
		''' The value of the String is the fully qualified classname :
		''' "javax.swing.JComponent".
		''' </summary>
		Public Const JCOMPONENT_UI As String = "javax.swing.JComponent"

		''' <summary>
		''' Denotes a UI implemented as an AWT panel.
		''' The value of the String is the fully qualified classname :
		''' "java.awt.Panel"
		''' </summary>
		Public Const PANEL_UI As String = "java.awt.Panel"

		''' <summary>
		''' Denotes a UI implemented as an AWT dialog.
		''' The value of the String is the fully qualified classname :
		''' "java.awt.Dialog"
		''' </summary>
		Public Const DIALOG_UI As String = "java.awt.Dialog"

		''' <summary>
		''' Denotes a UI implemented as a Swing dialog.
		''' The value of the String is the fully qualified classname :
		''' "javax.swing.JDialog"
		''' </summary>
		Public Const JDIALOG_UI As String = "javax.swing.JDialog"

		''' <summary>
		''' Denotes a UI which performs an informative "About" role.
		''' </summary>
		Public Const ABOUT_UIROLE As Integer = 1

		''' <summary>
		''' Denotes a UI which performs an administrative role.
		''' </summary>
		Public Const ADMIN_UIROLE As Integer = 2

		''' <summary>
		''' Denotes a UI which performs the normal end user role.
		''' </summary>
		Public Const MAIN_UIROLE As Integer = 3

		''' <summary>
		''' Not a valid role but role id's greater than this may be used
		''' for private roles supported by a service. Knowledge of the
		''' function performed by this role is required to make proper use
		''' of it.
		''' </summary>
		Public Const RESERVED_UIROLE As Integer = 99
		''' <summary>
		''' Get a UI object which may be cast to the requested UI type
		''' by the application and used in its user interface.
		''' <P> </summary>
		''' <param name="role"> requested. Must be one of the standard roles or
		''' a private role supported by this factory. </param>
		''' <param name="ui"> type in which the role is requested. </param>
		''' <returns> the UI role or null if the requested UI role is not available
		''' from this factory </returns>
		''' <exception cref="IllegalArgumentException"> if the role or ui is neither
		''' one of the standard ones, nor a private one
		''' supported by the factory. </exception>
		Public MustOverride Function getUI(ByVal role As Integer, ByVal ui As String) As Object

		''' <summary>
		''' Given a UI role obtained from this factory obtain the UI
		''' types available from this factory which implement this role.
		''' The returned Strings should refer to the static variables defined
		''' in this class so that applications can use equality of reference
		''' ("=="). </summary>
		''' <param name="role"> to be looked up. </param>
		''' <returns> the UI types supported by this class for the specified role,
		''' null if no UIs are available for the role. </returns>
		''' <exception cref="IllegalArgumentException"> is the role is a non-standard
		''' role not supported by this factory. </exception>
		Public MustOverride Function getUIClassNamesForRole(ByVal role As Integer) As String()



	End Class

End Namespace