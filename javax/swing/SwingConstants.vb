'
' * Copyright (c) 1997, 2000, Oracle and/or its affiliates. All rights reserved.
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
	''' A collection of constants generally used for positioning and orienting
	''' components on the screen.
	''' 
	''' @author Jeff Dinkins
	''' @author Ralph Kar (orientation support)
	''' </summary>
	Public Interface SwingConstants

			''' <summary>
			''' The central position in an area. Used for
			''' both compass-direction constants (NORTH, etc.)
			''' and box-orientation constants (TOP, etc.).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int CENTER = 0;

			'
			' Box-orientation constant used to specify locations in a box.
			'
			''' <summary>
			''' Box-orientation constant used to specify the top of a box.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int TOP = 1;
			''' <summary>
			''' Box-orientation constant used to specify the left side of a box.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int LEFT = 2;
			''' <summary>
			''' Box-orientation constant used to specify the bottom of a box.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int BOTTOM = 3;
			''' <summary>
			''' Box-orientation constant used to specify the right side of a box.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int RIGHT = 4;

			'
			' Compass-direction constants used to specify a position.
			'
			''' <summary>
			''' Compass-direction North (up).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int NORTH = 1;
			''' <summary>
			''' Compass-direction north-east (upper right).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int NORTH_EAST = 2;
			''' <summary>
			''' Compass-direction east (right).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int EAST = 3;
			''' <summary>
			''' Compass-direction south-east (lower right).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int SOUTH_EAST = 4;
			''' <summary>
			''' Compass-direction south (down).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int SOUTH = 5;
			''' <summary>
			''' Compass-direction south-west (lower left).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int SOUTH_WEST = 6;
			''' <summary>
			''' Compass-direction west (left).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int WEST = 7;
			''' <summary>
			''' Compass-direction north west (upper left).
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int NORTH_WEST = 8;

			'
			' These constants specify a horizontal or
			' vertical orientation. For example, they are
			' used by scrollbars and sliders.
			'
			''' <summary>
			''' Horizontal orientation. Used for scrollbars and sliders. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int HORIZONTAL = 0;
			''' <summary>
			''' Vertical orientation. Used for scrollbars and sliders. </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int VERTICAL = 1;

			'
			' Constants for orientation support, since some languages are
			' left-to-right oriented and some are right-to-left oriented.
			' This orientation is currently used by buttons and labels.
			'
			''' <summary>
			''' Identifies the leading edge of text for use with left-to-right
			''' and right-to-left languages. Used by buttons and labels.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int LEADING = 10;
			''' <summary>
			''' Identifies the trailing edge of text for use with left-to-right
			''' and right-to-left languages. Used by buttons and labels.
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int TRAILING = 11;
			''' <summary>
			''' Identifies the next direction in a sequence.
			''' 
			''' @since 1.4
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int NEXT = 12;

			''' <summary>
			''' Identifies the previous direction in a sequence.
			''' 
			''' @since 1.4
			''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Interfaces cannot contain fields in .NET:
'			public static final int PREVIOUS = 13;
	End Interface

End Namespace