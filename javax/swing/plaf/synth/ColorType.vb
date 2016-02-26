Imports System

'
' * Copyright (c) 2002, 2005, Oracle and/or its affiliates. All rights reserved.
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
	''' A typesafe enumeration of colors that can be fetched from a style.
	''' <p>
	''' Each <code>SynthStyle</code> has a set of <code>ColorType</code>s that
	''' are accessed by way of the
	''' <seealso cref="SynthStyle#getColor(SynthContext, ColorType)"/> method.
	''' <code>SynthStyle</code>'s <code>installDefaults</code> will install
	''' the <code>FOREGROUND</code> color
	''' as the foreground of
	''' the Component, and the <code>BACKGROUND</code> color to the background of
	''' the component (assuming that you have not explicitly specified a
	''' foreground and background color). Some components
	''' support more color based properties, for
	''' example <code>JList</code> has the property
	''' <code>selectionForeground</code> which will be mapped to
	''' <code>FOREGROUND</code> with a component state of
	''' <code>SynthConstants.SELECTED</code>.
	''' <p>
	''' The following example shows a custom <code>SynthStyle</code> that returns
	''' a red Color for the <code>DISABLED</code> state, otherwise a black color.
	''' <pre>
	''' class MyStyle extends SynthStyle {
	'''     private Color disabledColor = new ColorUIResource(Color.RED);
	'''     private Color color = new ColorUIResource(Color.BLACK);
	'''     protected Color getColorForState(SynthContext context, ColorType type){
	'''         if (context.getComponentState() == SynthConstants.DISABLED) {
	'''             return disabledColor;
	'''         }
	'''         return color;
	'''     }
	''' }
	''' </pre>
	''' 
	''' @since 1.5
	''' @author Scott Violet
	''' </summary>
	Public Class ColorType
		''' <summary>
		''' ColorType for the foreground of a region.
		''' </summary>
		Public Shared ReadOnly FOREGROUND As New ColorType("Foreground")

		''' <summary>
		''' ColorType for the background of a region.
		''' </summary>
		Public Shared ReadOnly BACKGROUND As New ColorType("Background")

		''' <summary>
		''' ColorType for the foreground of a region.
		''' </summary>
		Public Shared ReadOnly TEXT_FOREGROUND As New ColorType("TextForeground")

		''' <summary>
		''' ColorType for the background of a region.
		''' </summary>
		Public Shared ReadOnly TEXT_BACKGROUND As New ColorType("TextBackground")

		''' <summary>
		''' ColorType for the focus.
		''' </summary>
		Public Shared ReadOnly FOCUS As New ColorType("Focus")

		''' <summary>
		''' Maximum number of <code>ColorType</code>s.
		''' </summary>
		Public Shared ReadOnly MAX_COUNT As Integer

		Private Shared nextID As Integer

		Private description As String
		Private index As Integer

		Shared Sub New()
			MAX_COUNT = Math.Max(FOREGROUND.iD, Math.Max(BACKGROUND.iD, FOCUS.iD)) + 1
		End Sub

		''' <summary>
		''' Creates a new ColorType with the specified description.
		''' </summary>
		''' <param name="description"> String description of the ColorType. </param>
		Protected Friend Sub New(ByVal description As String)
			If description Is Nothing Then Throw New NullPointerException("ColorType must have a valid description")
			Me.description = description
			SyncLock GetType(ColorType)
				Me.index = nextID
				nextID += 1
			End SyncLock
		End Sub

		''' <summary>
		''' Returns a unique id, as an integer, for this ColorType.
		''' </summary>
		''' <returns> a unique id, as an integer, for this ColorType. </returns>
		Public Property iD As Integer
			Get
				Return index
			End Get
		End Property

		''' <summary>
		''' Returns the textual description of this <code>ColorType</code>.
		''' This is the same value that the <code>ColorType</code> was created
		''' with.
		''' </summary>
		''' <returns> the description of the string </returns>
		Public Overrides Function ToString() As String
			Return description
		End Function
	End Class

End Namespace