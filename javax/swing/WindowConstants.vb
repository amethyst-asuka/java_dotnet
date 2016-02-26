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
	''' Constants used to control the window-closing operation.
	''' The <code>setDefaultCloseOperation</code> and
	''' <code>getDefaultCloseOperation</code> methods
	''' provided by <code>JFrame</code>,
	''' <code>JInternalFrame</code>, and
	''' <code>JDialog</code>
	''' use these constants.
	''' For examples of setting the default window-closing operation, see
	''' <a
	''' href="https://docs.oracle.com/javase/tutorial/uiswing/components/frame.html#windowevents">Responding to Window-Closing Events</a>,
	''' a section in <em>The Java Tutorial</em>. </summary>
	''' <seealso cref= JFrame#setDefaultCloseOperation(int) </seealso>
	''' <seealso cref= JDialog#setDefaultCloseOperation(int) </seealso>
	''' <seealso cref= JInternalFrame#setDefaultCloseOperation(int)
	''' 
	''' 
	''' @author Amy Fowler </seealso>
	Public Interface WindowConstants
		''' <summary>
		''' The do-nothing default window close operation.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DO_NOTHING_ON_CLOSE = 0;

		''' <summary>
		''' The hide-window default window close operation
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int HIDE_ON_CLOSE = 1;

		''' <summary>
		''' The dispose-window default window close operation.
		''' <p>
		''' <b>Note</b>: When the last displayable window
		''' within the Java virtual machine (VM) is disposed of, the VM may
		''' terminate.  See <a href="../../java/awt/doc-files/AWTThreadIssues.html">
		''' AWT Threading Issues</a> for more information. </summary>
		''' <seealso cref= java.awt.Window#dispose() </seealso>
		''' <seealso cref= JInternalFrame#dispose() </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DISPOSE_ON_CLOSE = 2;

		''' <summary>
		''' The exit application default window close operation. Attempting
		''' to set this on Windows that support this, such as
		''' <code>JFrame</code>, may throw a <code>SecurityException</code> based
		''' on the <code>SecurityManager</code>.
		''' It is recommended you only use this in an application.
		''' 
		''' @since 1.4 </summary>
		''' <seealso cref= JFrame#setDefaultCloseOperation </seealso>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int EXIT_ON_CLOSE = 3;

	End Interface

End Namespace