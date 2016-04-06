Imports System
Imports System.Collections
Imports Microsoft.VisualBasic

'
' * Copyright (c) 1998, 2013, Oracle and/or its affiliates. All rights reserved.
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
	''' This class handles the renderable aspects of an operation with help
	''' from its associated instance of a ContextualRenderedImageFactory.
	''' </summary>
	Public Class RenderableImageOp
		Implements RenderableImage

		''' <summary>
		''' A ParameterBlock containing source and parameters. </summary>
		Friend paramBlock As ParameterBlock

		''' <summary>
		''' The associated ContextualRenderedImageFactory. </summary>
		Friend myCRIF As ContextualRenderedImageFactory

		''' <summary>
		''' The bounding box of the results of this RenderableImageOp. </summary>
		Friend boundingBox As java.awt.geom.Rectangle2D


		''' <summary>
		''' Constructs a RenderedImageOp given a
		''' ContextualRenderedImageFactory object, and
		''' a ParameterBlock containing RenderableImage sources and other
		''' parameters.  Any RenderedImage sources referenced by the
		''' ParameterBlock will be ignored.
		''' </summary>
		''' <param name="CRIF"> a ContextualRenderedImageFactory object </param>
		''' <param name="paramBlock"> a ParameterBlock containing this operation's source
		'''        images and other parameters necessary for the operation
		'''        to run. </param>
		Public Sub New(  CRIF As ContextualRenderedImageFactory,   paramBlock As ParameterBlock)
			Me.myCRIF = CRIF
			Me.paramBlock = CType(paramBlock.clone(), ParameterBlock)
		End Sub

        ''' <summary>
        ''' Returns a vector of RenderableImages that are the sources of
        ''' image data for this RenderableImage. Note that this method may
        ''' return an empty vector, to indicate that the image has no sources,
        ''' or null, to indicate that no information is available.
        ''' </summary>
        ''' <returns> a (possibly empty) Vector of RenderableImages, or null. </returns>
        Public Overridable ReadOnly Property sources As List(Of RenderableImage) Implements RenderableImage.getSources
            Get
                Return renderableSources
            End Get
        End Property

        Private ReadOnly Property renderableSources As List(Of RenderableImage)
            Get
                If paramBlock.numSources = 0 Then Return Nothing

                Dim sources_Renamed As New List(Of RenderableImage)
                Dim i As Integer = 0
                Do While i < paramBlock.numSources
                    Dim o As Object = paramBlock.getSource(i)
                    If TypeOf o Is RenderableImage Then
                        sources_Renamed += DirectCast(o, RenderableImage)
                        i += 1
                    Else
                        Exit Do
                    End If
                Loop
                Return sources_Renamed
            End Get
        End Property

        ''' <summary>
        ''' Gets a property from the property set of this image.
        ''' If the property name is not recognized, java.awt.Image.UndefinedProperty
        ''' will be returned.
        ''' </summary>
        ''' <param name="name"> the name of the property to get, as a String. </param>
        ''' <returns> a reference to the property Object, or the value
        '''         java.awt.Image.UndefinedProperty. </returns>
        Public Overridable Function getProperty(  name As String) As Object Implements RenderableImage.getProperty
			Return myCRIF.getProperty(paramBlock, name)
		End Function

        ''' <summary>
        ''' Return a list of names recognized by getProperty. </summary>
        ''' <returns> a list of property names. </returns>
        Public Overridable ReadOnly Property propertyNames As String() Implements RenderableImage.getPropertyNames
            Get
                Return myCRIF.propertyNames
            End Get
        End Property

        ''' <summary>
        ''' Returns true if successive renderings (that is, calls to
        ''' createRendering() or createScaledRendering()) with the same arguments
        ''' may produce different results.  This method may be used to
        ''' determine whether an existing rendering may be cached and
        ''' reused.  The CRIF's isDynamic method will be called. </summary>
        ''' <returns> <code>true</code> if successive renderings with the
        '''         same arguments might produce different results;
        '''         <code>false</code> otherwise. </returns>
        Public Overridable ReadOnly Property dynamic As Boolean Implements RenderableImage.dynamic
            Get
                Return myCRIF.dynamic
            End Get
        End Property

        ''' <summary>
        ''' Gets the width in user coordinate space.  By convention, the
        ''' usual width of a RenderableImage is equal to the image's aspect
        ''' ratio (width divided by height).
        ''' </summary>
        ''' <returns> the width of the image in user coordinates. </returns>
        Public Overridable ReadOnly Property width As Single Implements RenderableImage.getWidth
            Get
                If boundingBox Is Nothing Then boundingBox = myCRIF.getBounds2D(paramBlock)
                Return CSng(boundingBox.width)
            End Get
        End Property

        ''' <summary>
        ''' Gets the height in user coordinate space.  By convention, the
        ''' usual height of a RenderedImage is equal to 1.0F.
        ''' </summary>
        ''' <returns> the height of the image in user coordinates. </returns>
        Public Overridable ReadOnly Property height As Single Implements RenderableImage.getHeight
            Get
                If boundingBox Is Nothing Then boundingBox = myCRIF.getBounds2D(paramBlock)
                Return CSng(boundingBox.height)
            End Get
        End Property

        ''' <summary>
        ''' Gets the minimum X coordinate of the rendering-independent image data.
        ''' </summary>
        Public Overridable ReadOnly Property minX As Single Implements RenderableImage.getMinX
            Get
                If boundingBox Is Nothing Then boundingBox = myCRIF.getBounds2D(paramBlock)
                Return CSng(boundingBox.minX)
            End Get
        End Property

        ''' <summary>
        ''' Gets the minimum Y coordinate of the rendering-independent image data.
        ''' </summary>
        Public Overridable ReadOnly Property minY As Single Implements RenderableImage.getMinY
            Get
                If boundingBox Is Nothing Then boundingBox = myCRIF.getBounds2D(paramBlock)
                Return CSng(boundingBox.minY)
            End Get
        End Property

        ''' <summary>
        ''' Change the current ParameterBlock of the operation, allowing
        ''' editing of image rendering chains.  The effects of such a
        ''' change will be visible when a new rendering is created from
        ''' this RenderableImageOp or any dependent RenderableImageOp.
        ''' </summary>
        ''' <param name="paramBlock"> the new ParameterBlock. </param>
        ''' <returns> the old ParameterBlock. </returns>
        ''' <seealso cref= #getParameterBlock </seealso>
        Public Overridable Function setParameterBlock(  paramBlock As ParameterBlock) As ParameterBlock
			Dim oldParamBlock As ParameterBlock = Me.paramBlock
			Me.paramBlock = CType(paramBlock.clone(), ParameterBlock)
			Return oldParamBlock
		End Function

        ''' <summary>
        ''' Returns a reference to the current parameter block. </summary>
        ''' <returns> the <code>ParameterBlock</code> of this
        '''         <code>RenderableImageOp</code>. </returns>
        ''' <seealso cref= #setParameterBlock(ParameterBlock) </seealso>
        Public Overridable ReadOnly Property parameterBlock As ParameterBlock
            Get
                Return paramBlock
            End Get
        End Property

        ''' <summary>
        ''' Creates a RenderedImage instance of this image with width w, and
        ''' height h in pixels.  The RenderContext is built automatically
        ''' with an appropriate usr2dev transform and an area of interest
        ''' of the full image.  All the rendering hints come from hints
        ''' passed in.
        ''' 
        ''' <p> If w == 0, it will be taken to equal
        ''' System.Math.round(h*(getWidth()/getHeight())).
        ''' Similarly, if h == 0, it will be taken to equal
        ''' System.Math.round(w*(getHeight()/getWidth())).  One of
        ''' w or h must be non-zero or else an IllegalArgumentException
        ''' will be thrown.
        ''' 
        ''' <p> The created RenderedImage may have a property identified
        ''' by the String HINTS_OBSERVED to indicate which RenderingHints
        ''' were used to create the image.  In addition any RenderedImages
        ''' that are obtained via the getSources() method on the created
        ''' RenderedImage may have such a property.
        ''' </summary>
        ''' <param name="w"> the width of rendered image in pixels, or 0. </param>
        ''' <param name="h"> the height of rendered image in pixels, or 0. </param>
        ''' <param name="hints"> a RenderingHints object containing hints. </param>
        ''' <returns> a RenderedImage containing the rendered data. </returns>
        Public Overridable Function createScaledRendering(  w As Integer,   h As Integer,   hints As java.awt.RenderingHints) As java.awt.image.RenderedImage
			' DSR -- code to try to get a unit scale
			Dim sx As Double = CDbl(w)/width
			Dim sy As Double = CDbl(h)/height
			If System.Math.Abs(sx/sy - 1.0) < 0.01 Then sx = sy
			Dim usr2dev As java.awt.geom.AffineTransform = java.awt.geom.AffineTransform.getScaleInstance(sx, sy)
			Dim newRC As New RenderContext(usr2dev, hints)
			Return createRendering(newRC)
		End Function

		''' <summary>
		''' Gets a RenderedImage instance of this image with a default
		''' width and height in pixels.  The RenderContext is built
		''' automatically with an appropriate usr2dev transform and an area
		''' of interest of the full image.  All the rendering hints come
		''' from hints passed in.  Implementors of this interface must be
		''' sure that there is a defined default width and height.
		''' </summary>
		''' <returns> a RenderedImage containing the rendered data. </returns>
		Public Overridable Function createDefaultRendering() As java.awt.image.RenderedImage
			Dim usr2dev As New java.awt.geom.AffineTransform ' Identity
			Dim newRC As New RenderContext(usr2dev)
			Return createRendering(newRC)
		End Function

        ''' <summary>
        ''' Creates a RenderedImage which represents this
        ''' RenderableImageOp (including its Renderable sources) rendered
        ''' according to the given RenderContext.
        ''' 
        ''' <p> This method supports chaining of either Renderable or
        ''' RenderedImage operations.  If sources in
        ''' the ParameterBlock used to construct the RenderableImageOp are
        ''' RenderableImages, then a three step process is followed:
        ''' 
        ''' <ol>
        ''' <li> mapRenderContext() is called on the associated CRIF for
        ''' each RenderableImage source;
        ''' <li> createRendering() is called on each of the RenderableImage sources
        ''' using the backwards-mapped RenderContexts obtained in step 1,
        ''' resulting in a rendering of each source;
        ''' <li> ContextualRenderedImageFactory.create() is called
        ''' with a new ParameterBlock containing the parameters of
        ''' the RenderableImageOp and the RenderedImages that were created by the
        ''' createRendering() calls.
        ''' </ol>
        ''' 
        ''' <p> If the elements of the source Vector of
        ''' the ParameterBlock used to construct the RenderableImageOp are
        ''' instances of RenderedImage, then the CRIF.create() method is
        ''' called immediately using the original ParameterBlock.
        ''' This provides a basis case for the recursion.
        ''' 
        ''' <p> The created RenderedImage may have a property identified
        ''' by the String HINTS_OBSERVED to indicate which RenderingHints
        ''' (from the RenderContext) were used to create the image.
        ''' In addition any RenderedImages
        ''' that are obtained via the getSources() method on the created
        ''' RenderedImage may have such a property.
        ''' </summary>
        ''' <param name="renderContext"> The RenderContext to use to perform the rendering. </param>
        ''' <returns> a RenderedImage containing the desired output image. </returns>
        Public Overridable Function createRendering(  renderContext As RenderContext) As RenderedImage
            Dim image_Renamed As RenderedImage = Nothing
            Dim rcOut As RenderContext = Nothing

            ' Clone the original ParameterBlock; if the ParameterBlock
            ' contains RenderableImage sources, they will be replaced by
            ' RenderedImages.
            Dim renderedParamBlock As ParameterBlock = CType(paramBlock.clone(), ParameterBlock)
            Dim sources_Renamed As ArrayList = renderableSources

            Try
                ' This assumes that if there is no renderable source, that there
                ' is a rendered source in paramBlock

                If sources_Renamed IsNot Nothing Then
                    Dim renderedSources As New ArrayList
                    For i As Integer = 0 To sources_Renamed.Count - 1
                        rcOut = myCRIF.mapRenderContext(i, renderContext, paramBlock, Me)
                        Dim rdrdImage As java.awt.image.RenderedImage = CType(sources_Renamed(i), RenderableImage).createRendering(rcOut)
                        If rdrdImage Is Nothing Then Return Nothing

                        ' Add this rendered image to the ParameterBlock's
                        ' list of RenderedImages.
                        renderedSources.Add(rdrdImage)
                    Next i

                    If renderedSources.Count > 0 Then renderedParamBlock.sources = renderedSources
                End If

                Return myCRIF.create(renderContext, renderedParamBlock)
            Catch e As ArrayIndexOutOfBoundsException
                ' This should never happen
                Return Nothing
            End Try
        End Function
    End Class

End Namespace