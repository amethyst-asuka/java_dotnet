Imports System.Collections.Generic

'
' * Copyright (c) 2002, 2008, Oracle and/or its affiliates. All rights reserved.
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
	''' An immutable transient object containing contextual information about
	''' a <code>Region</code>. A <code>SynthContext</code> should only be
	''' considered valid for the duration
	''' of the method it is passed to. In other words you should not cache
	''' a <code>SynthContext</code> that is passed to you and expect it to
	''' remain valid.
	''' 
	''' @since 1.5
	''' @author Scott Violet
	''' </summary>
	Public Class SynthContext
		Private Shared ReadOnly queue As LinkedList(Of SynthContext) = New java.util.concurrent.ConcurrentLinkedQueue(Of SynthContext)

		Private component As javax.swing.JComponent
		Private ___region As Region
		Private style As SynthStyle
		Private state As Integer

		Shared Function getContext(ByVal c As javax.swing.JComponent, ByVal style As SynthStyle, ByVal state As Integer) As SynthContext
			Return getContext(c, SynthLookAndFeel.getRegion(c), style, state)
		End Function

		Shared Function getContext(ByVal component As javax.swing.JComponent, ByVal ___region As Region, ByVal style As SynthStyle, ByVal state As Integer) As SynthContext
			Dim ___context As SynthContext = queue.RemoveFirst()
			If ___context Is Nothing Then ___context = New SynthContext
			___context.reset(component, ___region, style, state)
			Return ___context
		End Function

		Shared Sub releaseContext(ByVal context As SynthContext)
			queue.AddLast(context)
		End Sub

		Friend Sub New()
		End Sub

		''' <summary>
		''' Creates a SynthContext with the specified values. This is meant
		''' for subclasses and custom UI implementors. You very rarely need to
		''' construct a SynthContext, though some methods will take one.
		''' </summary>
		''' <param name="component"> JComponent </param>
		''' <param name="region"> Identifies the portion of the JComponent </param>
		''' <param name="style"> Style associated with the component </param>
		''' <param name="state"> State of the component as defined in SynthConstants. </param>
		''' <exception cref="NullPointerException"> if component, region of style is null. </exception>
		Public Sub New(ByVal component As javax.swing.JComponent, ByVal ___region As Region, ByVal style As SynthStyle, ByVal state As Integer)
			If component Is Nothing OrElse ___region Is Nothing OrElse style Is Nothing Then Throw New NullPointerException("You must supply a non-null component, region and style")
			reset(component, ___region, style, state)
		End Sub


		''' <summary>
		''' Returns the hosting component containing the region.
		''' </summary>
		''' <returns> Hosting Component </returns>
		Public Overridable Property component As javax.swing.JComponent
			Get
				Return component
			End Get
		End Property

		''' <summary>
		''' Returns the Region identifying this state.
		''' </summary>
		''' <returns> Region of the hosting component </returns>
		Public Overridable Property region As Region
			Get
				Return ___region
			End Get
		End Property

		''' <summary>
		''' A convenience method for <code>getRegion().isSubregion()</code>.
		''' </summary>
		Friend Overridable Property subregion As Boolean
			Get
				Return region.subregion
			End Get
		End Property

		Friend Overridable Property style As SynthStyle
			Set(ByVal style As SynthStyle)
				Me.style = style
			End Set
			Get
				Return style
			End Get
		End Property


		Friend Overridable Property componentState As Integer
			Set(ByVal state As Integer)
				Me.state = state
			End Set
			Get
				Return state
			End Get
		End Property


		''' <summary>
		''' Resets the state of the Context.
		''' </summary>
		Friend Overridable Sub reset(ByVal component As javax.swing.JComponent, ByVal ___region As Region, ByVal style As SynthStyle, ByVal state As Integer)
			Me.component = component
			Me.___region = ___region
			Me.style = style
			Me.state = state
		End Sub

		Friend Overridable Sub dispose()
			Me.component = Nothing
			Me.style = Nothing
			releaseContext(Me)
		End Sub

		''' <summary>
		''' Convenience method to get the Painter from the current SynthStyle.
		''' This will NEVER return null.
		''' </summary>
		Friend Overridable Property painter As SynthPainter
			Get
				Dim ___painter As SynthPainter = style.getPainter(Me)
    
				If ___painter IsNot Nothing Then Return ___painter
				Return SynthPainter.NULL_PAINTER
			End Get
		End Property
	End Class

End Namespace