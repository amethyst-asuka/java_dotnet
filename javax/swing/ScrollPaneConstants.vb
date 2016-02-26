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
	''' Constants used with the JScrollPane component.
	''' 
	''' @author Hans Muller
	''' </summary>
	Public Interface ScrollPaneConstants
		''' <summary>
		''' Identifies a "viewport" or display area, within which
		''' scrolled contents are visible.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String VIEWPORT = "VIEWPORT";
		''' <summary>
		''' Identifies a vertical scrollbar. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String VERTICAL_SCROLLBAR = "VERTICAL_SCROLLBAR";
		''' <summary>
		''' Identifies a horizontal scrollbar. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String HORIZONTAL_SCROLLBAR = "HORIZONTAL_SCROLLBAR";
		''' <summary>
		''' Identifies the area along the left side of the viewport between the
		''' upper left corner and the lower left corner.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String ROW_HEADER = "ROW_HEADER";
		''' <summary>
		''' Identifies the area at the top the viewport between the
		''' upper left corner and the upper right corner.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String COLUMN_HEADER = "COLUMN_HEADER";
		''' <summary>
		''' Identifies the lower left corner of the viewport. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String LOWER_LEFT_CORNER = "LOWER_LEFT_CORNER";
		''' <summary>
		''' Identifies the lower right corner of the viewport. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String LOWER_RIGHT_CORNER = "LOWER_RIGHT_CORNER";
		''' <summary>
		''' Identifies the upper left corner of the viewport. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String UPPER_LEFT_CORNER = "UPPER_LEFT_CORNER";
		''' <summary>
		''' Identifies the upper right corner of the viewport. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String UPPER_RIGHT_CORNER = "UPPER_RIGHT_CORNER";

		''' <summary>
		''' Identifies the lower leading edge corner of the viewport. The leading edge
		''' is determined relative to the Scroll Pane's ComponentOrientation property.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String LOWER_LEADING_CORNER = "LOWER_LEADING_CORNER";
		''' <summary>
		''' Identifies the lower trailing edge corner of the viewport. The trailing edge
		''' is determined relative to the Scroll Pane's ComponentOrientation property.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String LOWER_TRAILING_CORNER = "LOWER_TRAILING_CORNER";
		''' <summary>
		''' Identifies the upper leading edge corner of the viewport.  The leading edge
		''' is determined relative to the Scroll Pane's ComponentOrientation property.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String UPPER_LEADING_CORNER = "UPPER_LEADING_CORNER";
		''' <summary>
		''' Identifies the upper trailing edge corner of the viewport. The trailing edge
		''' is determined relative to the Scroll Pane's ComponentOrientation property.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String UPPER_TRAILING_CORNER = "UPPER_TRAILING_CORNER";

		''' <summary>
		''' Identifies the vertical scroll bar policy property. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String VERTICAL_SCROLLBAR_POLICY = "VERTICAL_SCROLLBAR_POLICY";
		''' <summary>
		''' Identifies the horizontal scroll bar policy property. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		String HORIZONTAL_SCROLLBAR_POLICY = "HORIZONTAL_SCROLLBAR_POLICY";

		''' <summary>
		''' Used to set the vertical scroll bar policy so that
		''' vertical scrollbars are displayed only when needed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int VERTICAL_SCROLLBAR_AS_NEEDED = 20;
		''' <summary>
		''' Used to set the vertical scroll bar policy so that
		''' vertical scrollbars are never displayed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int VERTICAL_SCROLLBAR_NEVER = 21;
		''' <summary>
		''' Used to set the vertical scroll bar policy so that
		''' vertical scrollbars are always displayed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int VERTICAL_SCROLLBAR_ALWAYS = 22;

		''' <summary>
		''' Used to set the horizontal scroll bar policy so that
		''' horizontal scrollbars are displayed only when needed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int HORIZONTAL_SCROLLBAR_AS_NEEDED = 30;
		''' <summary>
		''' Used to set the horizontal scroll bar policy so that
		''' horizontal scrollbars are never displayed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int HORIZONTAL_SCROLLBAR_NEVER = 31;
		''' <summary>
		''' Used to set the horizontal scroll bar policy so that
		''' horizontal scrollbars are always displayed.
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'		int HORIZONTAL_SCROLLBAR_ALWAYS = 32;
	End Interface

End Namespace