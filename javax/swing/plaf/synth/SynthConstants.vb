Imports javax.swing

'
' * Copyright (c) 2002, 2003, Oracle and/or its affiliates. All rights reserved.
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
Namespace javax.swing.plaf.synth

	''' <summary>
	''' Constants used by Synth. Not all Components support all states. A
	''' Component will at least be in one of the primary states. That is, the
	''' return value from <code>SynthContext.getComponentState()</code> will at
	''' least be one of <code>ENABLED</code>, <code>MOUSE_OVER</code>,
	''' <code>PRESSED</code> or <code>DISABLED</code>, and may also contain
	''' <code>FOCUSED</code>, <code>SELECTED</code> or <code>DEFAULT</code>.
	''' 
	''' @since 1.5
	''' </summary>
	Public Interface SynthConstants
		''' <summary>
		''' Primary state indicating the component is enabled.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int ENABLED = 1 << 0;
		''' <summary>
		''' Primary state indicating the mouse is over the region.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int MOUSE_OVER = 1 << 1;
		''' <summary>
		''' Primary state indicating the region is in a pressed state. Pressed
		''' does not necessarily mean the user has pressed the mouse button.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int PRESSED = 1 << 2;
		''' <summary>
		''' Primary state indicating the region is not enabled.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DISABLED = 1 << 3;

		''' <summary>
		''' Indicates the region has focus.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int FOCUSED = 1 << 8;
		''' <summary>
		''' Indicates the region is selected.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int SELECTED = 1 << 9;
		''' <summary>
		''' Indicates the region is the default. This is typically used for buttons
		''' to indicate this button is somehow special.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		public static final int DEFAULT = 1 << 10;
	End Interface

End Namespace