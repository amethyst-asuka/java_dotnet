Imports System

'
' * Copyright (c) 1998, 2008, Oracle and/or its affiliates. All rights reserved.
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

' ********************************************************************
' **********************************************************************
' **********************************************************************
' *** COPYRIGHT (c) Eastman Kodak Company, 1997                      ***
' *** As  an unpublished  work pursuant to Title 17 of the United    ***
' *** States Code.  All rights reserved.                             ***
' **********************************************************************
' **********************************************************************
' *********************************************************************

Namespace java.awt.image.renderable

	''' <summary>
	''' A RenderContext encapsulates the information needed to produce a
	''' specific rendering from a RenderableImage.  It contains the area to
	''' be rendered specified in rendering-independent terms, the
	''' resolution at which the rendering is to be performed, and hints
	''' used to control the rendering process.
	''' 
	''' <p> Users create RenderContexts and pass them to the
	''' RenderableImage via the createRendering method.  Most of the methods of
	''' RenderContexts are not meant to be used directly by applications,
	''' but by the RenderableImage and operator classes to which it is
	''' passed.
	''' 
	''' <p> The AffineTransform parameter passed into and out of this class
	''' are cloned.  The RenderingHints and Shape parameters are not
	''' necessarily cloneable and are therefore only reference copied.
	''' Altering RenderingHints or Shape instances that are in use by
	''' instances of RenderContext may have undesired side effects.
	''' </summary>
	Public Class RenderContext
		Implements Cloneable

		''' <summary>
		''' Table of hints. May be null. </summary>
		Friend hints As RenderingHints

		''' <summary>
		''' Transform to convert user coordinates to device coordinates. </summary>
		Friend usr2dev As AffineTransform

		''' <summary>
		''' The area of interest.  May be null. </summary>
		Friend aoi As Shape

		' Various constructors that allow different levels of
		' specificity. If the Shape is missing the whole renderable area
		' is assumed. If hints is missing no hints are assumed.

		''' <summary>
		''' Constructs a RenderContext with a given transform.
		''' The area of interest is supplied as a Shape,
		''' and the rendering hints are supplied as a RenderingHints object.
		''' </summary>
		''' <param name="usr2dev"> an AffineTransform. </param>
		''' <param name="aoi"> a Shape representing the area of interest. </param>
		''' <param name="hints"> a RenderingHints object containing rendering hints. </param>
		Public Sub New(  usr2dev As AffineTransform,   aoi As Shape,   hints As RenderingHints)
			Me.hints = hints
			Me.aoi = aoi
			Me.usr2dev = CType(usr2dev.clone(), AffineTransform)
		End Sub

		''' <summary>
		''' Constructs a RenderContext with a given transform.
		''' The area of interest is taken to be the entire renderable area.
		''' No rendering hints are used.
		''' </summary>
		''' <param name="usr2dev"> an AffineTransform. </param>
		Public Sub New(  usr2dev As AffineTransform)
			Me.New(usr2dev, Nothing, Nothing)
		End Sub

		''' <summary>
		''' Constructs a RenderContext with a given transform and rendering hints.
		''' The area of interest is taken to be the entire renderable area.
		''' </summary>
		''' <param name="usr2dev"> an AffineTransform. </param>
		''' <param name="hints"> a RenderingHints object containing rendering hints. </param>
		Public Sub New(  usr2dev As AffineTransform,   hints As RenderingHints)
			Me.New(usr2dev, Nothing, hints)
		End Sub

		''' <summary>
		''' Constructs a RenderContext with a given transform and area of interest.
		''' The area of interest is supplied as a Shape.
		''' No rendering hints are used.
		''' </summary>
		''' <param name="usr2dev"> an AffineTransform. </param>
		''' <param name="aoi"> a Shape representing the area of interest. </param>
		Public Sub New(  usr2dev As AffineTransform,   aoi As Shape)
			Me.New(usr2dev, aoi, Nothing)
		End Sub

		''' <summary>
		''' Gets the rendering hints of this <code>RenderContext</code>. </summary>
		''' <returns> a <code>RenderingHints</code> object that represents
		''' the rendering hints of this <code>RenderContext</code>. </returns>
		''' <seealso cref= #setRenderingHints(RenderingHints) </seealso>
		Public Overridable Property renderingHints As RenderingHints
			Get
				Return hints
			End Get
			Set(  hints As RenderingHints)
				Me.hints = hints
			End Set
		End Property


		''' <summary>
		''' Sets the current user-to-device AffineTransform contained
		''' in the RenderContext to a given transform.
		''' </summary>
		''' <param name="newTransform"> the new AffineTransform. </param>
		''' <seealso cref= #getTransform </seealso>
		Public Overridable Property transform As AffineTransform
			Set(  newTransform As AffineTransform)
				usr2dev = CType(newTransform.clone(), AffineTransform)
			End Set
			Get
				Return CType(usr2dev.clone(), AffineTransform)
			End Get
		End Property

		''' <summary>
		''' Modifies the current user-to-device transform by prepending another
		''' transform.  In matrix notation the operation is:
		''' <pre>
		''' [this] = [modTransform] x [this]
		''' </pre>
		''' </summary>
		''' <param name="modTransform"> the AffineTransform to prepend to the
		'''        current usr2dev transform.
		''' @since 1.3 </param>
		Public Overridable Sub preConcatenateTransform(  modTransform As AffineTransform)
			Me.preConcetenateTransform(modTransform)
		End Sub

		''' <summary>
		''' Modifies the current user-to-device transform by prepending another
		''' transform.  In matrix notation the operation is:
		''' <pre>
		''' [this] = [modTransform] x [this]
		''' </pre>
		''' This method does the same thing as the preConcatenateTransform
		''' method.  It is here for backward compatibility with previous releases
		''' which misspelled the method name.
		''' </summary>
		''' <param name="modTransform"> the AffineTransform to prepend to the
		'''        current usr2dev transform. </param>
		''' @deprecated     replaced by
		'''                 <code>preConcatenateTransform(AffineTransform)</code>. 
		<Obsolete("    replaced by")> _
		Public Overridable Sub preConcetenateTransform(  modTransform As AffineTransform)
			usr2dev.preConcatenate(modTransform)
		End Sub

		''' <summary>
		''' Modifies the current user-to-device transform by appending another
		''' transform.  In matrix notation the operation is:
		''' <pre>
		''' [this] = [this] x [modTransform]
		''' </pre>
		''' </summary>
		''' <param name="modTransform"> the AffineTransform to append to the
		'''        current usr2dev transform.
		''' @since 1.3 </param>
		Public Overridable Sub concatenateTransform(  modTransform As AffineTransform)
			Me.concetenateTransform(modTransform)
		End Sub

		''' <summary>
		''' Modifies the current user-to-device transform by appending another
		''' transform.  In matrix notation the operation is:
		''' <pre>
		''' [this] = [this] x [modTransform]
		''' </pre>
		''' This method does the same thing as the concatenateTransform
		''' method.  It is here for backward compatibility with previous releases
		''' which misspelled the method name.
		''' </summary>
		''' <param name="modTransform"> the AffineTransform to append to the
		'''        current usr2dev transform. </param>
		''' @deprecated     replaced by
		'''                 <code>concatenateTransform(AffineTransform)</code>. 
		<Obsolete("    replaced by")> _
		Public Overridable Sub concetenateTransform(  modTransform As AffineTransform)
			usr2dev.concatenate(modTransform)
		End Sub


		''' <summary>
		''' Sets the current area of interest.  The old area is discarded.
		''' </summary>
		''' <param name="newAoi"> The new area of interest. </param>
		''' <seealso cref= #getAreaOfInterest </seealso>
		Public Overridable Property areaOfInterest As Shape
			Set(  newAoi As Shape)
				aoi = newAoi
			End Set
			Get
				Return aoi
			End Get
		End Property


		''' <summary>
		''' Makes a copy of a RenderContext. The area of interest is copied
		''' by reference.  The usr2dev AffineTransform and hints are cloned,
		''' while the area of interest is copied by reference.
		''' </summary>
		''' <returns> the new cloned RenderContext. </returns>
		Public Overridable Function clone() As Object
			Dim newRenderContext As New RenderContext(usr2dev, aoi, hints)
			Return newRenderContext
		End Function
	End Class

End Namespace